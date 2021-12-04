Public Class WinCsvliste
    Public auswahldatei As String
    Public _filenamen$()
    Property _exportfile As String
    Sub New(exportfile As String)
        InitializeComponent()
        _exportfile = exportfile
    End Sub
    Private Sub gastLayout()
        Background = myglobalz.GetSecondBackground()
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
    End Sub

    Private Sub btnAuswahldatei_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnAuswahldatei_ClickExtracted()
        e.Handled = True
    End Sub

    Private Sub btnAuswahldatei_ClickExtracted()
        If glob2.DokumenteAuswaehlen(_filenamen, initP.getValue("Haupt.ScansParadigma")) Then
        Else
            MsgBox("Fehler bei der Eingabe")
            Exit Sub
        End If
        If _filenamen.Count > 1 Then
            MsgBox("Sie dürfen nur eine Datei auswählen!")
            Exit Sub
        End If
        If _filenamen.Count < 1 Then
            MsgBox("Sie müssen exakt eine Datei auswählen!")
            Exit Sub
        End If
        auswahldatei = _filenamen(0)
        Dim testt As New IO.FileInfo(auswahldatei)
        If Not testt.Exists Then
            testt = Nothing
            MsgBox("Die Datei kann nicht gefunden werden !")
            Exit Sub
        End If
        testt = Nothing
        tbAuswahldatei.Text = auswahldatei
    End Sub

    Private Sub btnEinlesen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        IsEnabled = False
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dim messresult As New MessageBoxResult
        messresult = MessageBox.Show(glob2.getMsgboxText("CSVlisteFlustueck", New List(Of String)(New String() {})),
                                     "Flurstücksdaten einlesen", MessageBoxButton.YesNo
                                      )
        If Not messresult = MessageBoxResult.Yes Then
            IsEnabled = True
            e.Handled = True
            Exit Sub
        End If
        Dim startspalte As Integer = 3
        If Not String.IsNullOrEmpty(tbStartspalte.Text) Then
            If IsNumeric(tbStartspalte.Text) Then
                If CInt(tbStartspalte.Text) - 1 > 0 Then
                    startspalte = CInt(tbStartspalte.Text) - 1 'wg array optionbase 0
                Else
                    MsgBox("Startspalte muss größer als 0 sein")
                    Exit Sub
                End If
            Else
                MsgBox("Bitte geben Sie hier keinen Text ein. Gebraucht wird eine ganzzahlige Zahl!")
                Exit Sub
            End If
        Else
            ' der standrdwert (3) wird übernommen
        End If
        tbFortschritt.Background = New SolidColorBrush(Colors.LightGreen)

        Dim funktion As String = "  "
        If Not String.IsNullOrEmpty(tbFunktion.Text) Then
            funktion = tbFunktion.Text
        End If
        Dim rec As String() = Nothing
        Dim Ergebnis$ = FST_tools.csvlisteNachREC(tbAuswahldatei.Text, rec)

        Dim fehlerprotokoll As New IO.FileInfo(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "temp.txt"))
        Dim ebool As String = csv_zeilen_zuFST_verarbeiten(rec, CDbl(initP.getValue("MiniMap.radiusAdresse")), funktion, fehlerprotokoll, startspalte)
        ' MsgBox(Ergebnis )

        glob2.OpenDocument(fehlerprotokoll.FullName)
        fehlerprotokoll = Nothing
        IsEnabled = True
        Me.Close()
        e.Handled = True
    End Sub

    Private Function csv_zeilen_zuFST_verarbeiten(ByVal rec As String(), ByVal radius As Double, ByVal titel As String, ByVal fehlerprotokoll As IO.FileInfo, ByVal startspalte As Integer) As String
        Dim spalten As String()
        Dim sw As New IO.StreamWriter(fehlerprotokoll.FullName)
        Try
            For i = 1 To rec.GetUpperBound(0) - 1
                tbFortschritt.Text = "Flurstück " & i & " (" & rec.GetUpperBound(0) & ") "
                Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

                spalten = rec(i).Split(";"c)
                FST_tools.spaltenZuFST(spalten, startspalte)
                myglobalz.sitzung.aktFST.Freitext = ""
                FST_tools.hole_FSTKoordinaten_undZuweisePunkt(myglobalz.sitzung.aktFST)
                If myglobalz.sitzung.aktFST.punkt.X < 1 Then
                    sw.WriteLine("Fehler: Flurstück konnte nicht gefunden werden: " & myglobalz.sitzung.aktFST.normflst.toString(" "))
                End If
                myglobalz.sitzung.aktFST.name = titel
                FST_tools.flurstueck_speichernExtracted(radius, True)
                nachricht(i & " von " & rec.GetUpperBound(0) - 1)
            Next
            sw.Close()
            Return ("Datei wurde erfolgreich verarbeitet. Anzahl der Flurstücke: " & (rec.GetUpperBound(0) - 1))
        Catch ex As Exception
            nachricht("Fehler in csv_zeilen_zuFST_verarbeiten: " ,ex)
            Return "Fehler in csv_zeilen_zuFST_verarbeiten: " & ex.ToString
        End Try
    End Function

    Private Sub WinCsvliste_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        tbAuswahldatei.Text = _exportfile

        e.Handled = True
    End Sub

    Private Sub btnDateistruktur_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        MessageBox.Show(glob2.getMsgboxText("csvDateiStruktur", New List(Of String)(New String() {})),
                        "Dateistruktur für Flurstücksimport")
        e.Handled = True
    End Sub


    Private Sub btnDateistrukturBeispiel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        glob2.OpenDocument("notepad", initP.getValue("Myglobalz.flurstueckListenDateiBeispiel"))
        e.Handled = True
    End Sub
End Class
