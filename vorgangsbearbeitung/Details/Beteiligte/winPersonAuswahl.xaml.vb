Imports System.Data
Imports LibDB.LIBDB
Partial Public Class Window_Person
    Public lastpersonDB$
    Private Property _sql$

    Private Sub Window_Person_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        DialogResult = False
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Name) Then
            myGlobalz.sitzung.aktPerson.Name = tbFilter.Text
        End If
    End Sub
    Sub New(ByVal sql$)
        InitializeComponent()

        _sql = sql
    End Sub
    Private Sub Window_Person_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initcmbPersonalDB()
        FocusManager.SetFocusedElement(Me, tbFilter)
        Title = StammToolsNs.setWindowTitel.exe("edit", "Personenauswahl")
        If Not String.IsNullOrEmpty(_sql) Then
            tbFilter.Text = _sql
            suchePerson(tbFilter.Text, "Vorgangs-DBGesellfunktion", tbFilterOrt.Text)
            '  suchePerson(tbFilter.Text, "Fachdienst-DB", tbFilterOrt.Text)
        End If
    End Sub

    Sub initcmbPersonalDB()
        cmbPersonalDB.Items.Add("Fachdienst-DB")
        'cmbPersonalDB.Items.Add("Kreismitarbeiter")
        cmbPersonalDB.Items.Add("ProBauG-DB")
        cmbPersonalDB.Items.Add("Vorgangs-DB")
        cmbPersonalDB.Items.Add("Fachdienst IT")
        'cmbPersonalDB.Items.Add("ALB")
        cmbPersonalDB.SelectedIndex = 0
    End Sub

    Private Sub suchePerson(ByVal filter As String, ByVal personalDBname$, ByVal filterort$)
        Dim pDT As New DataTable
        Dim tempdat As New DataTable
        filter = filter.Trim()
        Try
            ' nachricht_und_Mbox(personalDBname$)
            Select Case personalDBname$
                Case "Fachdienst-DB"
                    myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = "stakeholder"
                    tempdat = DB_Oracle_sharedfunctions.suchePersonNachFilterDT_Like(filter)

                Case "ProBauG-DB"
                    myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = myGlobalz.probaug_MYDB.Tabelle
                    tempdat = DB_Oracle_sharedfunctions.suchePersonNachFilterDT_inProbaugDB(filter, filterort$)

                Case "Vorgangs-DB"
                    myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = myGlobalz.beteiligte_MYDB.Tabelle
                    tempdat = DB_Oracle_sharedfunctions.suchePersonNachFilterDT_inVorgangsDB(filter)

                Case "Vorgangs-DBGesellfunktion"
                    myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = "stakeholder" 'myGlobalz.beteiligte_MYDB.Tabelle
                    stckMain.Visibility = Windows.Visibility.Collapsed
                    tempdat = DB_Oracle_sharedfunctions.suchePersonNachRolleDT_inVorgangsDB(filter)

                Case "Fachdienst IT"
                    filter = LIBgemeinsames.clsString.umlaut2ue(filter)
                    myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = "Fachdienst IT"
                    tempdat = JFactiveDirectory.clsActiveDir.sucheperson(filter)
                    clsADtools.renameADColumns(tempdat)
            End Select
            If tempdat IsNot Nothing Then
                pDT = tempdat.Copy
            Else
                pDT = Nothing
            End If

            lastpersonDB$ = personalDBname$
            If pDT Is Nothing Then
                dgPersonenvorschlaege.DataContext = Nothing
                lblTreffer.Content = String.Format(" keine Treffer, in {0} für <{1}>",
                                         personalDBname$,
                                         filter)
            Else
                dgPersonenvorschlaege.DataContext = pDT
                lblTreffer.Content = String.Format(" {0}, in {1} für <{2}>",
                                         pDT.Rows.Count,
                                         personalDBname$,
                                         filter)
            End If
            pbarSuche.Visibility = Windows.Visibility.Collapsed
            lbltrefferinfo.Visibility = Windows.Visibility.Visible
        Catch ex As Exception
            My.Log.WriteEntry("Fehler suchePerson Keine Treffer:	" & ex.ToString)
        Finally
            If tempdat IsNot Nothing Then tempdat.Dispose()
        End Try
    End Sub


    Private Sub btnClearFilter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnClearFilter.Click
        tbFilter.Text = ""
    End Sub

    Private Sub startePersonensuche(ByVal Filter$)
        If Filter.Length > 2 Then
            pbarSuche.Visibility = Windows.Visibility.Visible
            'todo in backgroundworker verpacken
            suchePerson(tbFilter.Text, cmbPersonalDB.SelectedValue.ToString, tbFilterOrt.Text)
            lbltrefferinfo.Visibility = Windows.Visibility.Visible
            pbarSuche.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub btnStarteSuche_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnStarteSuche.Click
        startePersonensuche(tbFilter.Text.Trim)
        e.Handled = True
    End Sub

    Private Sub cmbPersonalDB_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbPersonalDB.SelectionChanged
        startePersonensuche(tbFilter.Text.Trim)
        e.Handled = True
    End Sub

    Private Sub dgPersonenvorschlaege_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgPersonenvorschlaege.SelectionChanged
        personvorschlagen()
        DialogResult = True
        e.Handled = True
        Me.Close()
    End Sub



    Private Function PersonUebernehmen(ByVal name$, ByVal vorname$, ByVal item As DataRowView) As Boolean
        Try
            myGlobalz.sitzung.BeteiligteModus = "neu"
            Select Case cmbPersonalDB.SelectedValue.ToString
                Case "Fachdienst-DB"
                    Dim lShouldReturn As Boolean
                    Dim lResult As Boolean = PersonausFachdienstDBUebernehmen(item, lShouldReturn)
                    If lShouldReturn Then
                        Return lResult
                    End If
                Case "ProBauG-DB"
                    'prüfen ob person schon in Paradigmadb existiert
                    If personExistiertInParadigmaDB(name$, vorname$) Then
                        If Not personDoubleAnlegen() Then
                            Return False
                        End If
                    End If
                    PersonAusProbaugNachParadigmadbUEbernehmen(item)
                Case "Vorgangs-DB"
                    'prüfen ob person schon in Paradigmadb existiert
                    If personExistiertInParadigmaDB(name$, vorname$) Then
                        If Not personDoubleAnlegen() Then
                            Return False
                        End If
                    End If
                    PersonAusVorgangsDBUebernehmen(item)
                Case "Fachdienst IT"
                    clsADtools.PersonAusFachdienstITDBUebernehmen(item)
            End Select
            Return True
        Catch ex As Exception
            My.Log.WriteEntry("PersonUebernehmen" & ex.ToString)
            Return False
        End Try
    End Function

    'Private Shared Sub auswahl2Obj(ByVal item As DataRowView, ByVal altepersonenID%)
    '    altepersonenID% = CInt(clsDBtools.fieldvalue(item(0)).ToString())
    '    myGlobalz.sitzung.aktPerson.Name = clsDBtools.fieldvalue(item("nachname")).ToString()
    '    myGlobalz.sitzung.aktPerson.Vorname = clsDBtools.fieldvalue(item("vorname")).ToString()
    '    myGlobalz.sitzung.aktPerson.Bemerkung = clsDBtools.fieldvalue(item("bemerkung")).ToString()
    '    myGlobalz.sitzung.aktPerson.Namenszusatz = clsDBtools.fieldvalue(item("namenszusatz")).ToString()
    '    myGlobalz.sitzung.aktPerson.Anrede = clsDBtools.fieldvalue(item("anrede")).ToString()
    'End Sub

    Public Shared Sub auswahl2Obj(ByVal item As DataRowView, ByVal aktperson As Person)
        Try
            With aktperson
                .clear()
                .Name = CStr(clsDBtools.fieldvalue(item("NachName"))).ToString
                .Vorname = CStr(clsDBtools.fieldvalue(item("Vorname"))).ToString
                .Bemerkung = ""
                .Namenszusatz = CStr(clsDBtools.fieldvalue(item("Namenszusatz"))).ToString()
                .Anrede = CStr(clsDBtools.fieldvalue(item("Anrede"))).ToString()
                .Bezirk = CStr(clsDBtools.fieldvalue(item("Bezirk"))).ToString()
                .Rolle = CStr(clsDBtools.fieldvalue(item("Rolle"))).ToString()
                Try
                    .Kassenkonto = CStr(clsDBtools.fieldvalue(item("KASSENKONTO"))).ToString()
                Catch ex As Exception

                End Try

                .Kontakt.clear()
                .Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GesellFunktion"))).ToString()
                .Kontakt.Bemerkung = "Quelle: VorgangsDB"
                .Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("Gemeindename"))).ToString()
                .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("Strasse"))).ToString()
                .Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("Hausnr"))).ToString()
                .Kontakt.Anschrift.PLZ = (CStr(clsDBtools.fieldvalue(item("PLZ"))).ToString()) 'ddr
                .Kontakt.elektr.Telefon1 = (CStr(clsDBtools.fieldvalue(item("fftelefon1"))).ToString())
                .Kontakt.elektr.Telefon2 = (CStr(clsDBtools.fieldvalue(item("fftelefon2"))).ToString())
                .Kontakt.elektr.Fax1 = (CStr(clsDBtools.fieldvalue(item("fffax1"))).ToString())
                .Kontakt.elektr.Fax2 = (CStr(clsDBtools.fieldvalue(item("fffax2"))).ToString())

                .Kontakt.elektr.MobilFon = (CStr(clsDBtools.fieldvalue(item("FFMobilFon"))).ToString())
                .Kontakt.elektr.Homepage = (CStr(clsDBtools.fieldvalue(item("FFHomepage"))).ToString())

                .Kontakt.elektr.Email = (CStr(clsDBtools.fieldvalue(item("ffemail"))).ToString())
                .Kontakt.Org.Name = (CStr(clsDBtools.fieldvalue(item("orgname"))).ToString())
                .Kontakt.Org.Zusatz = (CStr(clsDBtools.fieldvalue(item("orgzusatz"))).ToString())

                .changed_Anschrift = True
                .PersonenID = CInt(clsDBtools.fieldvalue(item("personenid")))
            End With
        Catch ex As Exception
            nachricht("fehler auswahl2Obj Fehler bei der Übernahme von Daten aus der Vorgangsdatenbank!")
            MsgBox("auswahl2Obj Fehler bei der Übernahme von Daten aus der Vorgangsdatenbank!")
        End Try
    End Sub
    Private Function PersonausFachdienstDBUebernehmen(ByVal item As DataRowView, ByRef shouldReturn As Boolean) As Boolean
        shouldReturn = False
        myGlobalz.sitzung.aktPerson.clear()
        auswahl2Obj(item, myGlobalz.sitzung.aktPerson)
        Return False
    End Function

    Private Shared Sub PersonAusVorgangsDBUebernehmen(ByVal item As DataRowView)
        Try
            With myGlobalz.sitzung.aktPerson
                .clear()
                .Name = CStr(clsDBtools.fieldvalue(item("NachName"))).ToString
                .Bezirk = CStr(clsDBtools.fieldvalue(item("bezirk"))).ToString
                .Vorname = CStr(clsDBtools.fieldvalue(item("Vorname"))).ToString
                .Bemerkung = ""
                .Namenszusatz = CStr(clsDBtools.fieldvalue(item("Namenszusatz"))).ToString()
                .Anrede = CStr(clsDBtools.fieldvalue(item("Anrede"))).ToString()
                .Kontakt.clear()
                .Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GesellFunktion"))).ToString()
                .Kontakt.Bemerkung = "Quelle: VorgangsDB"
                .Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("Gemeindename"))).ToString()
                .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("Strasse"))).ToString()
                .Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("Hausnr"))).ToString()
                .Kontakt.Anschrift.PLZ = (CStr(clsDBtools.fieldvalue(item("PLZ"))).ToString()) 'ddr
                .Kontakt.elektr.Telefon1 = (CStr(clsDBtools.fieldvalue(item("fftelefon1"))).ToString())
                .Kontakt.elektr.Telefon2 = (CStr(clsDBtools.fieldvalue(item("fftelefon2"))).ToString())
                .Kontakt.elektr.Fax1 = (CStr(clsDBtools.fieldvalue(item("fffax1"))).ToString())
                .Kontakt.elektr.Fax2 = (CStr(clsDBtools.fieldvalue(item("fffax2"))).ToString())

                .Kontakt.elektr.MobilFon = (CStr(clsDBtools.fieldvalue(item("FFMobilFon"))).ToString())
                .Kontakt.elektr.Homepage = (CStr(clsDBtools.fieldvalue(item("FFHomepage"))).ToString())

                .Kontakt.elektr.Email = (CStr(clsDBtools.fieldvalue(item("ffemail"))).ToString())
                .Kontakt.Org.Name = (CStr(clsDBtools.fieldvalue(item("orgname"))).ToString())
                .Kontakt.Org.Zusatz = (CStr(clsDBtools.fieldvalue(item("orgzusatz"))).ToString())

                .changed_Anschrift = True
            End With
        Catch ex As Exception
            MsgBox("Fehler bei der Übernahme von Daten aus der Vorgangsdatenbank! in PersonAusVorgangsDBUebernehmen" & ex.ToString)
        End Try
    End Sub


    Private Shared Sub PersonAusProbaugNachParadigmadbUEbernehmen(ByVal item As DataRowView)
        'person nach Paradigmadb übernehmen
        myGlobalz.sitzung.aktPerson.clear()
        myGlobalz.sitzung.aktPerson.Name = CStr(clsDBtools.fieldvalue(item("NACHNAME").ToString()))
        myGlobalz.sitzung.aktPerson.Vorname = CStr(clsDBtools.fieldvalue(item("VORNAME").ToString()))
        myGlobalz.sitzung.aktPerson.Bemerkung = "" 'CStr(clsDBtools.fieldvalue(item(3).ToString()
        myGlobalz.sitzung.aktPerson.Namenszusatz = CStr(clsDBtools.fieldvalue(item("NAMENSZUSATZ").ToString()))
        myGlobalz.sitzung.aktPerson.Quelle = "ProBauG"

        'Neue Kontaktdatenanlegen
        myGlobalz.sitzung.aktPerson.Kontakt.clear()
        myGlobalz.sitzung.aktPerson.Kontakt.GesellFunktion = "Privat"
        myGlobalz.sitzung.aktPerson.Kontakt.Bemerkung = "Quelle: ProBauG"
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("GEMEINDENAME").ToString()))
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("STRASSE").ToString()))
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("HAUSNR").ToString()))
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PLZ = (CStr(clsDBtools.fieldvalue(item("PLZ").ToString()))) 'ddr
        myGlobalz.sitzung.aktPerson.changed_Anschrift = True
    End Sub

    Function personDoubleAnlegen() As Boolean
        Return True
        'auf wunsch von klib rausgenommen
        'Dim red As MessageBoxResult = MessageBox.Show("Diese Person existiert schon in der Paradigmadatenbank. " & vbCrLf & _
        ' "Es wird empfohlen die Person nicht noch einmal in der ParadigmaDB anzulegen ('Double')" & vbCrLf & _
        ' "Wollen Sie die Person ein zweites mal in der ParadigmaDB anlegen ?", _
        ' "Person ist schon vorhanden", _
        ' MessageBoxButton.YesNo, _
        ' MessageBoxImage.Question, _
        ' MessageBoxResult.No)
        'If red = MessageBoxResult.Yes Then
        '    Return True
        'Else
        '    Return False
        'End If
    End Function

    Function personExistiertInParadigmaDB(ByVal name$, ByVal vorname As String) As Boolean
        Dim sqltext = ""
        Try
            If String.IsNullOrEmpty(name) Or String.IsNullOrEmpty(vorname) Then
                nachricht_und_Mbox("Warnung: Name is nothing")
            End If
            myGlobalz.sitzung.tempREC.mydb.Tabelle = myGlobalz.vorgangsbeteiligte_MYDB.Tabelle
            sqltext$ = "SELECT * FROM " & myGlobalz.vorgangsbeteiligte_MYDB.Tabelle & _
             " where upper(NACHNAME) = '" & name.ToUpper & "'" & _
             " and upper(Vorname)= '" & vorname.ToUpper & "'"
            DB_Oracle_sharedfunctions.suchePersonNachFilterDT_istgleich(sqltext$)
            If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                Return True
            Else
                Return False
            End If
            Return True
        Catch ex As Exception
            MsgBox("Fehler bei personExistiertInParadigmaDB!  " & sqltext$)
        End Try
    End Function

    Sub personvorschlagen()
        Try
            Dim item As DataRowView = CType(dgPersonenvorschlaege.SelectedItem, DataRowView)
            If item Is Nothing Then Return
            Dim name As String = ""
            Dim vorname As String = ""
            'todo das auswahlkriterium sollte erweitert werden um eindeutig zu treffen
            If myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = myGlobalz.probaug_MYDB.Tabelle Then
                name$ = item(0).ToString()
                vorname$ = item(1).ToString()
            End If

            If myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = myGlobalz.beteiligte_MYDB.Tabelle Then
                name$ = item(2).ToString()
                vorname$ = item(3).ToString()
            End If

            If myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = "stakeholder" Then
                name$ = item(1).ToString()
                vorname$ = item(2).ToString()
            End If

            If myGlobalz.vorgangsbeteiligte_MYDB.Tabelle = "Fachdienst IT" Then
                name$ = item(4).ToString()
                vorname$ = item(1).ToString()
            End If

            'dgPersonenvorschlaege.SelectedItem = Nothing
            PersonUebernehmen(name$, vorname$, item)
        Catch ex As Exception
            nachricht_und_Mbox(String.Format("dgEreignisse_SelectionChanged: {0}", ex))
        End Try
    End Sub







    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        DialogResult = False
    End Sub
End Class
