Public Class WinVorlagenListe
    Property pfadAllgemein As String
    Property pfadNurSachgebiet As String
    Property pfadPermanent As String

    Property sgnr As String
    Property sgHeader As String
    Property _akteZiehenModus As Boolean = False

    Sub New(_sgnr As String, _sgHeader As String, akteZiehenModus As Boolean)
        InitializeComponent()
        sgnr = _sgnr
        sgHeader = _sgHeader
        _akteZiehenModus = akteZiehenModus
    End Sub

    Private Sub WinVorlagenListe_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If _akteZiehenModus Then
            Dim item As String = "Bitte Akte ziehen.docx"
            ckbEditVorlagenDatei.IsChecked = False
                Dim divorlagenVerzeichnis As IO.DirectoryInfo = Nothing
                clsVorlagenTools.berechneVorlagenverzeichnisAllgemein(divorlagenVerzeichnis, "allgemein", pfadallgemein)
            dgVorlagenAllgemein_SelectionChangedExtracted(item)
        Else
            refreshVorlagenListe(sgnr)
            SetTitle()
            If myGlobalz.sitzung.aktVorgangsID < 1 Then
                ckbEditVorlagenDatei.IsChecked = True
                ckbEditVorlagenDatei.IsEnabled = False
                MessageBox.Show("Im Moment ist kein Vorgang aktiv und " & Environment.NewLine &
                       "                      kein Sachgebiet ausgewählt." & Environment.NewLine & Environment.NewLine &
                       "Sie können daher entweder die allgemeingültigen Vorlagen ändern" & Environment.NewLine &
                       "    oder " & Environment.NewLine &
                       "sie wählen Sie zunächst ein Sachgebiet aus (Knopf 'Sachgebiet wechseln')",
                       "Vorlagen ändern",
                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
                tiAllgemein.IsSelected = True
            End If
        End If

        e.Handled = True
    End Sub

    Private Sub SetTitle()
        tiSachgebiet.Header = "Nur für Sachgebiet: " & sgnr
        tiSachgebiet.ToolTip = "Nur für Sachgebiet: " & sgHeader 'myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header

        tiAllgemein.ToolTip = "Diese Vorlagen sind unabhängig vom Sachgebiet"
        tiPermanent.ToolTip = "Diese Vorlagen bleiben hier an Ort und Stelle. Sie dienen dazu um Informationen für das gesamte Sachgebiet - also unabhängig vom Einzelfall - zu sammeln und bereitzustellen "
        tbsgNR.Text = sgHeader
        btnExplorer.Content = sgnr & "-Verzeichnis öffnen"
        Title = "Formular: Vorlagenliste, Sachgebiet: " & sgnr & ", " & sgHeader
    End Sub

    Private Sub dgVorlagen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgVorlagen.SelectionChanged
        Dim item As String 'nurSachgebiet
        Try
            If dgVorlagen.SelectedItem Is Nothing Then Return
            item = dgVorlagen.SelectedItem.ToString
            dgVorlagen.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            pfadNurSachgebiet = pfadNurSachgebiet.Replace("\\", "\")
            Dim datei As String = IO.Path.Combine(pfadNurSachgebiet, item).Replace("\\", "\")
            If ckbEditVorlagenDatei.IsChecked Or datei.ToLower.Contains("_##permanent##_") Then
                'DateiOffnen
                MessageBox.Show("Die Datei wird NICHT ins Archiv übernommen." & Environment.NewLine &
                       "Sie wird zur direkten Änderung geöffnet und sollte wieder am gleichen Ort gespeichert werden! " & Environment.NewLine &
                       "(" & datei & ")",
                       "Permanent-Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
                Process.Start(datei)
            Else
                If myGlobalz.sitzung.aktVorgangsID > 0 Then
                    vorlageVerwursten(item, datei, pfadNurSachgebiet, False)
                End If
            End If
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagen_SelectionChanged: {0}", ex)))
        End Try
    End Sub


    Private Sub vorlageVerwursten(ByVal item As String, ByVal datei As String, pfad As String, akteZiehenmodus As Boolean)
        Dim testendung As New IO.FileInfo(item)
        Dim ergebnis As String
        If testendung.Extension.ToLower.Contains("xls") Or
            testendung.Extension.ToLower.Contains("pdf") Then
            Dim vsteuer As New winOtherVorlagensteuerung(item, pfad)
            vsteuer.ShowDialog()
            If vsteuer.abbruch Then
                If vsteuer.cmbModus.SelectedIndex = 0 Then
                    glob2.OpenDocument(datei)
                Else
                    Dim neuu As New clsVorlagedokumente(datei)
                    myGlobalz.sitzung.aktDokument.DateinameMitExtension = neuu.neuenNamenVerwendenBilden(testendung.Extension)
                    myGlobalz.sitzung.aktDokument.Beschreibung = testendung.Name.Replace(testendung.Extension, "")
                    myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                    Dokument.createCheckoutDir(myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID)
                    neuu._VorlageDateiImArchiv.CopyTo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                    myGlobalz.sitzung.aktDokument.Filedatum = Now
                    ergebnis = clsVorlagenTools.XLSGetEinCheckErgebnis(myGlobalz.sitzung.aktDokument.Filedatum)
                    If Not String.IsNullOrEmpty(ergebnis) Then MsgBox(ergebnis)
                End If
            Else
                myGlobalz.sitzung.aktDokument.Beschreibung = vsteuer.dokumentschlagworte
                Dim outfile As String = "", erfolg As Boolean
                Dim ereignisid As Integer = 0
                ereignisid = If(vsteuer.mitereignis, myGlobalz.sitzung.aktEreignis.ID, 0)
                erfolg = clsVorlagedokumente.VorlageImArchivVerwendenXLSPDF(outdatei:=outfile,
                                                                          vdatei:=datei,
                                                                          ereignisanlegen:=vsteuer.mitereignis,
                                                                          Schlagworte:=myGlobalz.sitzung.aktDokument.Beschreibung,
                                                                          quellpfad:="",
                                                                          ereignisart:=vsteuer.ereingistyp,
                                                                          ereignistitel:=vsteuer.ereingistitel,
                                                                          dateityp:=testendung.Extension.ToLower,
                                                                          ereignisid:=ereignisid)
                MessageBox.Show("Die Datei wurde in das Archiv übernommen. (siehe Reiter 'Dokumente')", "Vorlagendatei übernommen", MessageBoxButton.OK, MessageBoxImage.Information)
            End If
        Else
            'word
            Dim vsteuer As New WinWordVorlageSteuerung(item, pfad,akteZiehenmodus )
            vsteuer.ShowDialog()
        End If
    End Sub



    Private Sub refreshVorlagenListe(sgnr As String)
        Dim caunt As Integer '= initVorlagenDatatable("allgemein")
        caunt = initVorlagenDatatable2("allgemein", dgVorlagenAllgemein, pfadAllgemein, sgnr)
        caunt = initVorlagenDatatable2("fuersachgebiet", dgVorlagen, pfadNurSachgebiet, sgnr)
        caunt = initVorlagenDatatable2("_##permanent##_", dgVorlagenPermanant, pfadPermanent, sgnr)


        If caunt% >= 0 Then
            '	dgVorlagen.DataContext = myGlobalz.sitzung.VorlagenREC.dt
            '	tabheaderVorlagen.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
            ' tabheaderVorlagen.Header = "Vorlagen " & caunt%
        Else
            '	dgVorlagen.DataContext = Nothing
            'tabheaderVorlagen.SetValue(TextElement.FontWeightProperty, FontWeights.normal)
            'tabheaderVorlagen.Header = "Vorlagen " '& 		myGlobalz.sitzung.VorlagenREC.rows.count 
        End If
    End Sub




    Private Sub tbTestmarkenliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        clsVorlagenTools.zeigeTextmarkenListe()
        e.Handled = True
    End Sub


    Private Sub btnRatgeber_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim info$ = "Sollte das Formular nicht (vollständig) ausgefüllt sein , kann dies verschiedene Ursachen haben." & vbCrLf &
            "" & vbCrLf &
            "1. Schreibfehler bei den Textmarken" & vbCrLf &
            "    Bitte informieren Sie sich über die exakte Schreibweise der Textmarke. " & vbCrLf &
            "    Infos zu den Textmarken bekommt man durch drücken " & vbCrLf &
            "    des Knopfes ganz links ('Alle Textmarken')." & vbCrLf &
            "    Sollte es nicht an der Schreibweise liegen, versuchen Sie eine nummerierte" & vbCrLf &
            "    Version der Schreibweise (AZ1 statt AZ)." & vbCrLf &
            "    Nummerierung kann von 0 bis 10 stattfinden (AZ0 - AZ10)." & vbCrLf &
            "" & vbCrLf &
            "" & vbCrLf &
            "2. Das Dokument darf nicht geschützt sein und sollte keine Formulare (z.B. Pulldowns) enthalten" & vbCrLf &
            "    Formulare stören schlicht und in einem geschützten Dokument lassen sich auch keine Textmarken ändern!" & vbCrLf &
            "" & vbCrLf &
            "" & vbCrLf &
            "3. Verwenden Sie die Standardbezeichnungen. z.B. Sollte die Rolle eines Beteiligten möglichst aus der Pulldownliste genommen werden." & vbCrLf &
            "    Andernfalls kann diese Rolle nicht für Textmarken verwendet werden! (Beispiel: 'Antragsteller/in' wird von der Textmarke verwendet. " & vbCrLf &
            "    Aber 'Antragst.' wird ignoriert.)" & vbCrLf &
            "  " & vbCrLf &
            "Bei Problemen bitte an Frau Weyers wenden." & vbCrLf &
            "" & vbCrLf &
            "" & vbCrLf

        MessageBox.Show(info, "Probleme mit Textmarken vermeiden", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Function initVorlagenDatatable2(modus As String, dataGrid As DataGrid, ByRef pfad As String, sgnr As String) As Integer
        Dim ListeAllerVorlagenImVerz As New List(Of IO.FileInfo)
        Dim ListeAllerVorlagenImVerzSort As New List(Of IO.FileInfo)

        Dim anzahlVorlagen As Integer
        Dim divorlagenVerzeichnis As IO.DirectoryInfo = Nothing
        nachricht("initVorlagenDatatable")
        ListeAllerVorlagenImVerz.Clear()
        Try

            anzahlVorlagen = clsVorlagenTools.presentVorlagenListe(ListeAllerVorlagenImVerz, divorlagenVerzeichnis, modus, pfad, sgnr)
            Dim results = From zeile As IO.FileInfo In ListeAllerVorlagenImVerz
                       Order By zeile.Name
            For Each strra As IO.FileInfo In results
                ListeAllerVorlagenImVerzSort.Add(strra)
            Next
            If ListeAllerVorlagenImVerz.Count > 0 Then
                dataGrid.DataContext = ListeAllerVorlagenImVerzSort
            Else
                dataGrid.DataContext = Nothing
            End If
            Return anzahlVorlagen
        Catch ex As Exception
            dgVorlagen.DataContext = Nothing
            Return -1
        End Try
    End Function

    Private Sub dgVorlagenAllgemein_SelectionChangedExtracted(ByVal item As String)
        pfadAllgemein = pfadAllgemein.Replace("\\", "\")
        Dim datei As String = IO.Path.Combine(pfadAllgemein, item).Replace("\\", "\")
        If ckbEditVorlagenDatei.IsChecked Then
            'DateiOffnen
            MessageBox.Show("Die Datei wird NICHT ins Archiv übernommen." & Environment.NewLine &
                   "Sie wird zur direkten Änderung geöffnet und sollte wieder am gleichen Ort gespeichert werden! " & Environment.NewLine &
                   "(" & datei & ")",
                   "Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
            Process.Start(datei)
        Else
            If myGlobalz.sitzung.aktVorgangsID > 0 Then
                Dim divorlagenVerzeichnis As IO.DirectoryInfo = Nothing
                'clsVorlagenTools.berechneVorlagenverzeichnisAllgemein(divorlagenVerzeichnis, "allgemein", pfad)
                If _akteZiehenModus Then
                    Close
                End If
                vorlageVerwursten(item, datei, pfadAllgemein, _akteZiehenModus)
            End If
        End If
    End Sub
    Private Sub dgVorlagenAllgemein_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgVorlagenAllgemein.SelectionChanged
        Dim item As String
        Try
            If dgVorlagenAllgemein.SelectedItem Is Nothing Then Return
            item = dgVorlagenAllgemein.SelectedItem.ToString
            dgVorlagenAllgemein.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            '  dgVorlagenAllgemein_SelectionChangedExtracted(item, pfadAllgemein)

            dgVorlagenAllgemein_SelectionChangedExtracted(item)
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagen_SelectionChanged: {0}", ex)))
        End Try
    End Sub

    Private Sub dgVorlagenPermanant_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgVorlagenPermanant.SelectionChanged
        Dim item As String
        Try
            If dgVorlagenPermanant.SelectedItem Is Nothing Then Return
            item = dgVorlagenPermanant.SelectedItem.ToString
            dgVorlagenPermanant.SelectedItem = Nothing

            pfadPermanent = pfadPermanent.Replace("\\", "\")
            Dim datei As String = IO.Path.Combine(pfadPermanent, item).Replace("\\", "\")

            'DateiOffnen
            MessageBox.Show("Die Datei wird NICHT ins Archiv übernommen." & Environment.NewLine &
                   "Sie wird zur direkten Änderung geöffnet und sollte wieder am gleichen Ort gespeichert werden! " & Environment.NewLine &
                   "(" & datei & ")",
                   "Permanent-Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
            Process.Start(datei)


            e.Handled = True
            Me.Close()
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagenPermanant_SelectionChanged: {0}", ex)))
        End Try
    End Sub

    Private Sub btnExplorer_Click(sender As Object, e As RoutedEventArgs)
        Dim vorlagenVerzeichnis As IO.DirectoryInfo
        Dim pfad As String = ""
        '         berechneVorlagenverzeichnisAllgemein(di, modus, pfad)
        clsVorlagenTools.berechneVorlagenverzeichnisSachgebiet(vorlagenVerzeichnis, "fuersachgebiet", pfad, "")
        '  berechneVorlagenverzeichnisPermanent(di, modus, pfad, sgnr)

        '   clsVorlagenTools.berechneVorlagenverzeichnis(vorlagenVerzeichnis, "fuersachgebiet", pfad, "")
        Dim diras As String = pfad
        Process.Start(pfad)
        e.Handled = True
    End Sub

    Private Sub btnSGexplorer_Click(sender As Object, e As RoutedEventArgs)
        If Not String.IsNullOrEmpty(tbzuSG.Text) Then
            sgnr = tbzuSG.Text
            sgHeader = ""
            tbzuSG.Text = ""
        Else
            Dim sgt As New win_sgtree(myGlobalz.Paradigma_Sachgebietsdatei)
            sgt.ShowDialog()
            sgnr = sgt.publicNR
            sgHeader = sgt.publicsgHeader
            sgt = Nothing
        End If
        refreshVorlagenListe(sgnr)
        SetTitle()

        GC.Collect()

        e.Handled = True
    End Sub

End Class
