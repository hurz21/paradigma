Partial Public Class WINzahlungdetail
    Private _modus$

    Sub New(ByVal modus$)
        InitializeComponent()
        _modus = modus
    End Sub
    Sub initBeteiligtenCMB()
        'gemeindeDT
        clsBeteiligteBUSI.holeBeteiligte()
        cmbBeteiligte.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub

    Private Sub WINzahlungdetail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show(String.Format("Sie haben Daten in dieser Maske geändert! {0}" +
                                                "Wenn Sie diese Änderungen {0}" +
                                                " - prüfen und ggf. speichern möchten wählen Sie 'JA'{0}" +
                                                " - verwerfen möchten wählen Sie 'Nein'{0}" +
                                                "Prüfen und abspeichern ?", vbCrLf),
                       "Zahlungsdetails", _
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Exclamation,
                       MessageBoxResult.OK)
            If Not red = MessageBoxResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub
    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        InitCombos()
        initBeteiligtenCMB()
        clsParadigmaRechte.buttons_schalten(btnSpeichern, btnLoeschen)
        If _modus = "neu" Then
            myGlobalz.sitzung.aktEreignis.clearValues()
            myGlobalz.sitzung.aktEreignis.Datum = Now
            myGlobalz.sitzung.aktZahlung.clear()
            myGlobalz.sitzung.aktZahlung.VerschicktAm = Now
            myGlobalz.sitzung.aktZahlung.AngeordnetAm = CLstart.mycsimple.MeinNULLDatumAlsDate
            myGlobalz.sitzung.aktZahlung.Eingang = True
            myGlobalz.sitzung.aktEreignis.Richtung = "Eingang"
            myGlobalz.sitzung.aktZahlung.Typ = "Gebühr"
            myGlobalz.sitzung.aktZahlung.Eingang = True
            btnLoeschen.IsEnabled = False
        End If

        If _modus = "edit" Then
            If myGlobalz.sitzung.aktZahlung.Eingang = True Then
                richtungein.IsChecked = True
            Else
                richtungaus.IsChecked = True
            End If
            Me.Title &= " " & _modus
            clsEreignisTools.leseEreignisByID(myGlobalz.sitzung.aktEreignis.ID)
            clsEreignisTools.ToObj_Ereignis()
            ZahlungToolsNs.leseZahlung.execute(myGlobalz.sitzung.aktEreignis.ID, myGlobalz.sitzung.aktVorgangsID)
            ZahlungToolsNs.zahlungDTtoOBJ.execute(myGlobalz.sitzung.tempREC.dt.Rows(0), myGlobalz.sitzung.aktZahlung)
            myGlobalz.sitzung.aktEreignis.Notiz = myGlobalz.sitzung.aktZahlung.Notiz
            Title = StammToolsNs.setWindowTitel.exe(_modus, "Zahlung")
        End If
        If myGlobalz.sitzung.aktZahlung.EingangAm < CDate("1900-01-01") Then tbEingangAm.Text = ""
        If myGlobalz.sitzung.aktZahlung.AngeordnetAm < CDate("1900-01-01") Then tbAngeordnetAm.Text = ""
        btnSpeichern.IsEnabled = False
    End Sub

    Private Sub abbruch()
        Me.Close()
    End Sub

    Private Sub dpVerschickt_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dpVerschickt.SelectedDateChanged
        Dim c As Date = (CDate(dpVerschickt.SelectedDate) + Now.TimeOfDay)
        myGlobalz.sitzung.aktZahlung.VerschicktAm = c
         e.Handled=true
    End Sub

    Private Sub dpEingang_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dpEingang.SelectedDateChanged
        Dim c As Date = (CDate(dpEingang.SelectedDate) + Now.TimeOfDay)
        myGlobalz.sitzung.aktZahlung.EingangAm = c
         e.Handled=true
    End Sub

    Private Sub cmbTyp_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbTyp.SelectionChanged
        myGlobalz.sitzung.aktZahlung.Typ = cmbTyp.SelectedValue.ToString
        glob2.schliessenButton_einschalten(btnSpeichern)
         e.Handled=true
    End Sub
    Private Sub cmbHHST_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbHHST.SelectionChanged
        myGlobalz.sitzung.aktZahlung.HausHaltsstelle = cmbHHST.SelectedValue.ToString
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled=true
    End Sub

    Private Sub InitCombos()
        cmbTyp.Items.Add("Abgabe")
        cmbTyp.Items.Add("Beihilfe")
        cmbTyp.Items.Add("Bußgeld")
        cmbTyp.Items.Add("Ersatzgeld")
        cmbTyp.Items.Add("Gebühr")
        cmbTyp.Items.Add("Rechnung")
        cmbTyp.Items.Add("Sicherheitsleistung")
        cmbTyp.Items.Add("Sonstige")
        cmbTyp.Items.Add("Schornsteinfegergebühr")
        cmbTyp.Items.Add("Verwaltungsgebühr")
        cmbTyp.Items.Add("Zwangsgeld")

        cmbHHST.Items.Add("67.01.03.51000010")
        cmbHHST.Items.Add("67.01.03/4011.82900216")
    End Sub

    Private Sub btnZahlungSpeichernClick()
        btnSpeichern.IsEnabled = False
        If _modus = "neu" Then
            If Not alleZahlungsEingabenOK() Then Exit Sub
            ' nachricht(" zahlungsid% = " & clsZAHLUNGDB_Mysql.zahlungspeichern(_modus).ToString)

            myGlobalz.sitzung.aktZahlung.Notiz = myGlobalz.sitzung.aktEreignis.Notiz
            nachricht(" speichern ")

            ZahlungToolsNs.zahlung_und_ereignis_speichern_alleDB.execute(_modus, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktEreignis)

            nachricht(" zahlungsid% = " & myGlobalz.sitzung.aktZahlung.ZahlungsID)
            '  Dim d = myGlobalz.sitzung.aktEreignis.Notiz
            clstart.myc.aLog.komponente = "Ereignis" : clstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & ": neue Zahlung angelegt" : clstart.myc.aLog.log()
            btnSpeichern.IsEnabled = False
            Me.Close()
        End If
        If _modus = "edit" Then
            If Not alleZahlungsEingabenOK() Then Exit Sub
            myGlobalz.sitzung.aktZahlung.Notiz = myGlobalz.sitzung.aktEreignis.Notiz
            ' myGlobalz.sitzung.aktEreignis.Datum= myGlobalz.sitzung.aktZahlung.VerschicktAm
            ' Dim d = myGlobalz.sitzung.aktEreignis.Notiz
            ' nachricht("zahlungsid% = " & clsZAHLUNGDB_Mysql.zahlungspeichern(_modus).ToString)
            'myGlobalz.sitzung.aktEreignis.Beschreibung = String.Format("Zahlung: {0}, {1}: {2} Euro, , Verschickt: {3}",
            '                                myGlobalz.sitzung.aktZahlung.Zahler,
            '                                myGlobalz.sitzung.aktZahlung.Typ,
            '                                myGlobalz.sitzung.aktZahlung.Betrag,
            '                                Format(myGlobalz.sitzung.aktZahlung.VerschicktAm,
            '                                       "yyyy-MM-dd"))
            ZahlungToolsNs.zahlung_und_ereignis_speichern_alleDB.execute(_modus, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktEreignis)
            'nachricht("zahlungsid% = " & ZahlungToolsNs.ZahlungEdit_alleDB.execute(myGlobalz.sitzung.aktZahlung.ZahlungsID).ToString)
            clstart.myc.aLog.komponente = "Ereignis" : clstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & ": Zahlung editiert" : clstart.myc.aLog.log()
            Me.Close()
        End If
    End Sub

    Shared Function alleZahlungsEingabenOK() As Boolean
        If myGlobalz.sitzung.aktZahlung.Betrag < 1 Then
            MessageBox.Show("Sie haben keinen Betrag eingegeben!")
            Return False
        End If
        Return True
    End Function



    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zahlungUndEreignisLoeschen()
    End Sub

    Sub zahlungUndEreignisLoeschen()
        If Not glob2.istloeschenErnstgemeint() Then Exit Sub
        ZahlungToolsNs.Zahlung_loeschen_alledb.execute(myGlobalz.sitzung.aktZahlung.ZahlungsID)
        clsEreignisTools.ereignisLoeschen_alleDB(myGlobalz.sitzung.aktEreignis.ID)
        Me.Close()
    End Sub

    Private Sub cmbBeteiligte_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbBeteiligte.SelectionChanged
        If cmbBeteiligte.SelectedValue Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktZahlung.Zahler = cmbBeteiligte.SelectedValue.ToString
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub richtung(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If richtungein.IsChecked = True Then
            myGlobalz.sitzung.aktZahlung.Eingang = True
            tblZahler.Text = "Zahler"
            btnAnnahmeanordnung.Content = "Annahmeanordnung"
            glob2.schliessenButton_einschalten(btnSpeichern)
        End If
        If richtungaus.IsChecked = True Then
            myGlobalz.sitzung.aktZahlung.Eingang = False
            tblZahler.Text = "Empfänger"
            btnAnnahmeanordnung.Content = "Auszahlungsanordnung"
            glob2.schliessenButton_einschalten(btnSpeichern)
        End If
        '	Case true
        '		tblZahler.Text = "Zahler"
        '		btnAnnahmeanordnung.Content = "Annahmeanordnung"
        '	Case false
        '		tblZahler.Text = "Empfänger"
        '		btnAnnahmeanordnung.Content = "Auszahlungsanordnung"
    End Sub

    Private Sub tbBetrag_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBetrag.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbBeschreibung_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbEingangAm_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEingangAm.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbVerschickt_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVerschickt.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbNotiz_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbNotiz.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbZahler_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbZahler.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub


    Private Sub btnAnnahmeanordnung_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        myGlobalz.Arc.AllesAuscheckenVorgang(False, True)
        glob2.EmailFormOEffnen("umwelt@kreis-offenbach.de", _
         " = Bitte eine Annahmeordnung erstellen. ", _
         String.Format(" Bitte eine Annahmeordnung erstellen. VorgangsNr: {0}{1}", myGlobalz.sitzung.aktVorgangsID, vbCrLf), "", myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, False)
        e.Handled = True
    End Sub


    Private Sub btnExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ZahlungToolsNs.ExcelausgabeExtracted.execute() '  clsZAHLUNGDB_Mysql.ExcelausgabeExtracted()
        e.Handled = True
    End Sub

    Private Sub dpAngeordnet_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim c As Date = (CDate(dpAngeordnet.SelectedDate) + Now.TimeOfDay)
        myGlobalz.sitzung.aktZahlung.AngeordnetAm = c
    End Sub
End Class
