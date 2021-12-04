Imports System.Data
'wichtig : für deplay das release verz nach o:\.... kopieren
Class MainWindow
    Public Shared enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
    Property tools As New clsTools
    Property rollenfilter$ = ""
    Property orderstring$ = ""
    Property wherestring$ = ""
    Property sql$ = ""
    Property oldperson As New Person

    Property delim As String = ";"
    Private ladevorgangabgeschlossen As Boolean = False
    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        IO.Directory.SetCurrentDirectory("O:\UMWELT\B\GISDatenEkom\div\deploy\paradigma\steakholder")
        Dim logfile As String = "O:\UMWELT\B\GISDatenEkom\stakeholder_" & Environment.UserName & ".log"
        sw = New IO.StreamWriter(logfile)
        sw.AutoFlush = False
        l(Now.ToString)
        l("Vor initdb")
        tools.initDB()
        aktADR = New ParaAdresse
        refreshpersonenREC()
        refreshAnzeige()
        For i = 0 To ParaRec.dt.Rows.Count - 1
            If CStr(ParaRec.dt.Rows(i).Item("Nachname")) = "Luley" Then
                Dim a = CStr(ParaRec.dt.Rows(i).Item("plz"))
            End If
        Next
        setComboboxBeteiligte()
        chkCarry.Visibility = Visibility.Hidden

        ladevorgangabgeschlossen = True
    End Sub

    Private Sub refreshAnzeige()
        Dim anzahl As Integer = ParaRec.dt.Rows.Count
        tbanzahl.Text = "Bestand: " & anzahl & " Personen"
        dgPersonen.DataContext = ParaRec.dt
    End Sub
    Private Sub refreshpersonenREC()
        refreshSQL()
        ParaRec.mydb.SQL = sql '"select * from " & myrec.mydb.Tabelle & " order by gesellfunktion,nachname ,vorname"
        l("refreshpersonenREC: " & ParaRec.mydb.SQL)
        ParaRec.getDataDT()
        l("nach  myrec.getDataDT: " & ParaRec.dt.Rows.Count)
    End Sub

    Private Sub dgPersonen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgPersonen.SelectionChanged
        personvorschlagen()
        ' DialogResult = True
        e.Handled = True
    End Sub

    Sub personvorschlagen()
        Try
            Dim item As DataRowView = CType(dgPersonen.SelectedItem, DataRowView)
            If item Is Nothing Then Return
            clsTools.PersonUebernehmen(item, aktPerson)
            oldperson = clsTools.personenKopieVon(aktPerson)
            oldperson.Name = ""

            Dim winpersneu As New winBeteiligteDetail("edit", oldperson, CBool(chkCarry.IsChecked))
            winpersneu.ShowDialog()
            oldperson = clsTools.personenKopieVon(winpersneu._oldperson)
            oldperson.Name = ""
            refreshpersonenREC()
            refreshAnzeige()
        Catch ex As Exception
            l(String.Format("FEHLER personvorschlagen: {0}", ex))
        End Try
    End Sub


    Private Sub btnNeu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If Not chkCarry.IsChecked Then
            oldperson.clear()
        End If
        Dim winpersneu As New winBeteiligteDetail("neu", oldperson, CBool(chkCarry.IsChecked))
        winpersneu.ShowDialog()
        oldperson = clsTools.personenKopieVon(winpersneu._oldperson)
        oldperson.Name = ""
        refreshpersonenREC()
        refreshAnzeige()
        e.Handled = True
    End Sub

    Private Sub setComboboxBeteiligte()
        Dim filename As String = IO.Path.Combine(appdir, "config", "Combos", "Stakeholder_Rollen.xml")
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxbeteiligteRollen"), XmlDataProvider)
        existing.Source = New Uri(filename)
        ' ComboBoxBeteiligte.SelectedIndex = 0
    End Sub
    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub ComboBoxBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If ComboBoxBeteiligte.SelectedValue Is Nothing Then Exit Sub
        If ComboBoxBeteiligte.SelectedValue.ToString.StartsWith("Hinzuf") Then Exit Sub
        Dim item2 As String = CType(ComboBoxBeteiligte.SelectedValue, String)

        rollenfilter$ = " lower(rolle)='" & item2.ToLower & "' "


        refreshpersonenREC()
        '   ausgabe(myrec.dt)
        refreshAnzeige()
        e.Handled = True
    End Sub

    Private Sub refreshSQL()
        sql$ = "select * from stakeholder "
        buildwherestring()
        buildOrderstring()
        sql = sql & wherestring & orderstring
    End Sub

    Private Sub buildwherestring()
        If Not String.IsNullOrEmpty(rollenfilter) Then
            wherestring = " where " & rollenfilter
        End If
    End Sub

    Private Sub buildOrderstring()
        ' orderstring = " order by rolle,nachname,vorname"
        If String.IsNullOrEmpty(orderstring) Then
            'orderstring = " order by rolle,bezirk,nachname,vorname"
            orderstring = " order by nachname,vorname"
        Else

        End If
    End Sub



    Private Sub btnExcel_Click(sender As Object, e As RoutedEventArgs)
        ausgabe(ParaRec.dt)
        e.Handled = True
    End Sub

    Private Sub ausgabe(datas As DataTable)
        Dim ts As String = Format(Now, "yyyyMMdd_hhmmss")
        Dim datei As String = "C:\Users\" & Environment.UserName & "\Desktop\Paradigma\stakeholders" & ts & ".csv"
        sw = New IO.StreamWriter(datei, False, enc)
        Ereignisse(datas)
        sw.Close()
        sw.Dispose()
        Process.Start(datei)
    End Sub

    Public Sub Ereignisse(datas As DataTable)
        Dim line As String
        Try
            'sw.WriteLine( _
            '    "Datum" & delim & _
            '    "Art" & delim & _
            '    "Beschreibung" & delim
            '     )
            For Each p As DataRow In datas.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable

                line =
                 ohneSemikolon(p.Item("Nachname").ToString) & delim &
                 ohneSemikolon(p.Item("vorname").ToString) & delim &
                 ohneSemikolon(p.Item("Namenszusatz").ToString) & delim &
                 ohneSemikolon(p.Item("ffemail").ToString) & delim &
                 ohneSemikolon(p.Item("fdkurz").ToString) & delim &
                 ohneSemikolon(p.Item("anrede").ToString) & delim &
                 ohneSemikolon(p.Item("gemeindename").ToString) & delim &
                 ohneSemikolon(p.Item("strasse").ToString) & delim &
                 ohneSemikolon(p.Item("hausnr").ToString) & delim &
                 ohneSemikolon(p.Item("PLZ").ToString) & delim &
                 ohneSemikolon(p.Item("postfach").ToString) & delim &
                 ohneSemikolon(p.Item("anschriftbemerkung").ToString) & delim &
                 ohneSemikolon(p.Item("orgname").ToString) & delim &
                 ohneSemikolon(p.Item("orgzusatz").ToString) & delim &
                 ohneSemikolon(p.Item("orgtyp1").ToString) & delim &
                 ohneSemikolon(p.Item("orgtyp2").ToString) & delim &
                 ohneSemikolon(p.Item("orgeigentuemer").ToString) & delim &
                 ohneSemikolon(p.Item("quelle").ToString) & delim &
                ohneSemikolon(p.Item("bemerkung").ToString) & delim
                sw.WriteLine(line)
            Next
        Catch ex As Exception
            MsgBox("Fehler bei der Excelausgabea" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Private Function ohneSemikolon(ByRef p1 As String) As String
        Try
            If String.IsNullOrEmpty(p1) Then
                Return ""
            End If
            Dim temp$ = p1
            temp = temp.Trim
            temp = temp.Replace(";", "_")
            temp = temp.Replace(vbCrLf, "")
            Return temp
        Catch ex As Exception
            l("Fehler in ohneSemikolon: " & ex.ToString)
            Return ""
        End Try
    End Function
End Class
