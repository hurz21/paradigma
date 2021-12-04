Imports LibDB.LIBDB
Partial Public Class Window_WiedervorlageDetail

    Private Sub Window_WiedervorlageDetail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show(
                String.Format("Sie haben Daten in dieser Maske geändert! {0}" +
                              "Wenn Sie diese Änderungen {0}" +
                              " - prüfen und ggf. speichern möchten wählen Sie 'JA'{0}" +
                              " - verwerfen möchten wählen Sie 'Nein'{0}Prüfen und abspeichern ?",
                              vbCrLf),
                  "Ereignisdetails", _
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Exclamation,
                  MessageBoxResult.OK)
            If Not red = MessageBoxResult.No Then
                'btnSpeichernEreignis.IsEnabled = False
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub Window_WiedervorlageDetail_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        ' MsgBox(myGlobalz.sitzung.aktVorgang.Stammdaten.Bearbeiter)
        setcmbWiedervorlageAuswahl(cmbWiedervorlageAuswahl)
        setComboboxStatus()
        iniComboDelegat()
        If myGlobalz.sitzung.Wiedervorlagemodus = "neu" Then
            myGlobalz.sitzung.aktWiedervorlage.clear()
            setzeFaelligkeitStandard()
            initBeleitigtencombo()
        End If
        If myGlobalz.sitzung.Wiedervorlagemodus = "edit" Then
            'einlesen der wiedervorlage
            DB_Oracle_sharedfunctions.leseWiedervorlageRecord(myGlobalz.sitzung.DBWiedervorlageREC, myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID)
            myGlobalz.sitzung.aktWiedervorlage.clear()
            If clsWVTOOLS.WVrecord2OBJ(myGlobalz.sitzung.aktWiedervorlage, myGlobalz.sitzung.DBWiedervorlageREC.dt) Then
                DatePickerWiederVorlage.SelectedDate = myGlobalz.sitzung.aktWiedervorlage.datum
            Else
                MsgBox("Es ist ein Fehler beim Lesen der Wiedervorlage aufgetreten. Abbruch!")
                e.Handled = True
                Close()
            End If

        End If
        setComboboxStatusObjekt()
        btnSpeichern.IsEnabled = False
        clsParadigmaRechte.buttons_schalten(btnSpeichern)
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.Wiedervorlagemodus, "Wiedervorlage")
        e.Handled = True
    End Sub

    Shared Sub WVrecord2OBJ(ByRef wvl As clsWiedervorlage, ByRef sitz As Psession)     'myGlobalz.sitzung.aktWiedervorlage
        wvl.Bemerkung = clsDBtools.fieldvalue(sitz.DBWiedervorlageREC.dt.Rows(0).Item("Bemerkung"))
        wvl.ToDo = clsDBtools.fieldvalue(sitz.DBWiedervorlageREC.dt.Rows(0).Item("ToDo"))
        wvl.WartenAuf = clsDBtools.fieldvalue(sitz.DBWiedervorlageREC.dt.Rows(0).Item("WartenAuf"))
        wvl.datum = clsDBtools.fieldvalueDate(sitz.DBWiedervorlageREC.dt.Rows(0).Item("datum"))
        wvl.erledigtAm = clsDBtools.fieldvalueDate(sitz.DBWiedervorlageREC.dt.Rows(0).Item("erledigtAm"))
        wvl.Erledigt = CBool(sitz.DBWiedervorlageREC.dt.Rows(0).Item("Erledigt"))
        wvl.WiedervorlageID = CInt(sitz.DBWiedervorlageREC.dt.Rows(0).Item("id"))
    End Sub

    Sub initBeleitigtencombo()
        cmbBeteiligterAuswahl.DataContext = myGlobalz.sitzung.beteiligteREC.dt
    End Sub

    Sub setzeFaelligkeitStandard() '1 woche
        myGlobalz.sitzung.aktWiedervorlage.datum = DateAdd("ww", +1, Now)
        tbWiedervorlage.Text = myGlobalz.sitzung.aktWiedervorlage.datum.ToString
        DatePickerWiederVorlage.SelectedDate = myGlobalz.sitzung.aktWiedervorlage.datum
    End Sub

    Shared Sub setcmbWiedervorlageAuswahl(ByVal cmbWVL As ComboBox)
        cmbWVL.Items.Add("")
        cmbWVL.Items.Add("In 10 Jahren")
        cmbWVL.Items.Add("In 5 Jahren")
        cmbWVL.Items.Add("In 1 Jahr")
        cmbWVL.Items.Add("In 4 Wochen")
        cmbWVL.Items.Add("In 3 Wochen")
        cmbWVL.Items.Add("In 2 Wochen")
        cmbWVL.Items.Add("In 1 Wochen")
        cmbWVL.Items.Add("In 3 Tagen")
        cmbWVL.SelectedIndex = 0
    End Sub


    Private Sub cmbWiedervorlageAuswahl_SelectionChangedExtracted(ByVal lCmbWiedervorlageAuswahlSelectedItemToString As String)
        btnSpeichern.IsEnabled = True
        Try
            myGlobalz.sitzung.aktWiedervorlage.datum = clsWVTOOLS.setzeNeuesWVDatum(lCmbWiedervorlageAuswahlSelectedItemToString)
            tbWiedervorlage.Text = myGlobalz.sitzung.aktWiedervorlage.datum.ToString
            DatePickerWiederVorlage.SelectedDate = myGlobalz.sitzung.aktWiedervorlage.datum
        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub
    Private Sub cmbWiedervorlageAuswahl_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbWiedervorlageAuswahl.SelectionChanged
        cmbWiedervorlageAuswahl_SelectionChangedExtracted(cmbWiedervorlageAuswahl.SelectedItem.ToString)
    End Sub

    Private Sub DatePickerWiederVorlage_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePickerWiederVorlage.SelectedDateChanged
        tbWiedervorlage.Text = Format(DatePickerWiederVorlage.SelectedDate, "dd.MM.yyy")
    End Sub

    Private Sub cmbBeteiligterAuswahl_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbBeteiligterAuswahl.SelectionChanged
        tbWartenaufBeteiligten.Text &= cmbBeteiligterAuswahl.SelectedValue.ToString & ", "
        cmbBeteiligterAuswahl.SelectedValue = Nothing
    End Sub


    Private Sub WVSpeichern()
        btnSpeichern.IsEnabled = False
        If myGlobalz.sitzung.Wiedervorlagemodus = "neu" Then
            If WVneuOK(myGlobalz.sitzung.aktVorgangsID) Then Me.Close()
        End If
        If myGlobalz.sitzung.Wiedervorlagemodus = "edit" Then
            If editOK(myGlobalz.sitzung.aktVorgangsID) Then Me.Close()
        End If
    End Sub

    Function WVneuOK(ByVal zielvorgangsid As Integer) As Boolean
        If Not wvNEU_eingabenOk() Then Return False
        If Not WVNEUform2objok() Then Return False
        Dim lResult As Boolean = clsWVTOOLS.WVneuOKExtracted(zielvorgangsid)
        Me.Close()
        Return lResult
        Exit Function
    End Function

    Shared Function editOK(ByVal zielvorgangsid As Integer) As Boolean
        If Not EDIT_eingabenOk() Then Return False
        If Not EDITform2objok() Then Return False
        If Not EDITobj2DBOk_WV() Then
            nachricht_und_Mbox("Problem beim Abspeichern der geänderten Wiedervorlage")
        End If

        clstart.myc.aLog.komponente = "Wiedervorlage" : clstart.myc.aLog.aktion = myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID & ": editiert" : clstart.myc.aLog.log()

        myGlobalz.sitzung.Ereignismodus = "edit"
        clsWVTOOLS.WVneuOKExtracted(myGlobalz.sitzung.aktWiedervorlage, myGlobalz.sitzung.aktEreignis.ID)

        clsEreignisTools.EDITobj2DBOk_Ereignis_alledb(myGlobalz.sitzung.aktEreignis.ID, zielvorgangsid, myGlobalz.sitzung.aktEreignis)

        'If clsEreignisDB.Edit_speichern_Ereignis(myGlobalz.sitzung.aktEreignis.ID) Then
        '    nachricht("Daten wurden gespeichert!")
        'End If
        Return True
    End Function

    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichern.Click
        WVSpeichern()
        Close()
        e.Handled = True
    End Sub

    Shared Function EDITform2objok() As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktWiedervorlage.Bearbeiter) Then
            myGlobalz.sitzung.aktWiedervorlage.Bearbeiter = myGlobalz.sitzung.aktBearbeiter.Initiale
        End If
        Return True
    End Function

    Shared Function EDIT_eingabenOk() As Boolean
        Return True
    End Function

    Shared Function wvNEU_eingabenOk() As Boolean
        Return True
    End Function

    Shared Function WVNEUform2objok() As Boolean
        myGlobalz.sitzung.aktWiedervorlage.VorgangsID = myGlobalz.sitzung.aktVorgangsID
        Return True
    End Function

    Shared Function EDITobj2DBOk_WV() As Boolean
        'NEUobj2DBOk_Ereignis

        If myGlobalz.sitzung.aktWiedervorlage.updateWV() > 0 Then
            Return True
        Else
            Return False
        End If
    End Function



    Private Sub cmbStatus_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStatus.SelectionChanged
        btnSpeichern.IsEnabled = True
        Try
            Dim item As String = CType(cmbStatus.SelectedValue, String)
            If item = "erledigt" Then
                myGlobalz.sitzung.aktWiedervorlage.Erledigt = True
            Else
                myGlobalz.sitzung.aktWiedervorlage.Erledigt = False
            End If
            Exit Sub
        Catch ex As Exception
            nachricht_und_Mbox("cmbStatus_SelectionChanged. " & ex.ToString)
        End Try
    End Sub

    Private Sub setComboboxStatus()
        cmbStatus.Items.Add("unerledigt")
        cmbStatus.Items.Add("erledigt")
        cmbStatus.SelectedIndex = 0
    End Sub

    Private Sub setComboboxStatusObjekt()
        If myGlobalz.sitzung.aktWiedervorlage.Erledigt Then
            cmbStatus.SelectedIndex = 1
        Else
            cmbStatus.SelectedIndex = 0
        End If
    End Sub

    Private Sub tbTodo_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTodo.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(1200, tbWartenaufBeteiligten)
    End Sub

    Private Sub tbWartenaufBeteiligten_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbWartenaufBeteiligten.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(1100, tbWartenaufBeteiligten)
    End Sub

    Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBemerkung.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(1200, tbWartenaufBeteiligten)
    End Sub

    Private Sub tbWiedervorlage_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbWiedervorlage.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschen.Click
        If Not glob2.istloeschenErnstgemeint Then Exit Sub
        Dim clsdbwv As New clsWiedervorlageDB_CRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.wiedervorlage_MYDB))
        If clsdbwv.Wiedervorlage_loeschen(myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID) > 0 Then
            clsEreignisTools.ereignisLoeschen_alleDB(myGlobalz.sitzung.aktEreignis.ID)
            'If clsEreignisDB.Ereignis_loeschen(myGlobalz.sitzung.aktEreignis.ID) < 1 Then
            '    nachricht("Ereignis konnte nicht gelöscht werden: " & myGlobalz.sitzung.aktEreignis.ID)
            'End If
        Else
            nachricht("wiedervorlage konnte nicht nicht gelöscht werden!")
        End If
        clsdbwv.Dispose
        e.Handled = True
        Me.Close()
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        e.Handled = True
        Me.Close()
    End Sub

    Private Sub cmbDelegat_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        glob2.schliessenButton_einschalten(btnSpeichern)
        myGlobalz.sitzung.aktWiedervorlage.Bearbeiter = cmbDelegat.SelectedValue.ToString
    End Sub

    Sub iniComboDelegat()
        Dim bekannt As Boolean
        bekannt = NSBearbeiter.BearbeiterTools.istUserBekannt()
        If Not bekannt Then
            nachricht("fehler User ist unbekannt in iniComboDelegat: " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.username & vbCrLf & 
                            " Bitte informieren sie den Admin." & vbCrLf)
        End If
        cmbDelegat.Items.Add(myGlobalz.sitzung.aktBearbeiter.Initiale)
        cmbDelegat.Items.Add(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
        cmbDelegat.Items.Add("NehU")
        cmbDelegat.Items.Add("KuhP")
        cmbDelegat.Items.Add("WeyG")
        plusWeitereBearbeiter(cmbDelegat)
    End Sub

    Private Sub btnNachOutlookUebernehmen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        clsWVTOOLS.ggfTerminNachOutlookUebernehmen(myGlobalz.sitzung.aktWiedervorlage, myGlobalz.sitzung.aktEreignis, True)
    End Sub

    Private Sub plusWeitereBearbeiter(ByVal comboBox As ComboBox)
        Try
            If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.Trim) Then Exit Sub
            Dim a As String() = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.Trim.Split(";"c)
            For Each teil In a
                If Not String.IsNullOrEmpty(teil.Trim) Then cmbDelegat.Items.Add(teil)
            Next
        Catch ex As Exception
            nachricht("Fehler in : " & ex.ToString)
        End Try
    End Sub

End Class
