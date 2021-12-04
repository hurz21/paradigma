Partial Public Class WINzahlungdetail
    Private _modus As string
    Private Property formIstGeladen As Boolean = False
    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub

    Sub New(ByVal modus As String)
        InitializeComponent()
        _modus = modus
    End Sub
    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        e.Handled = True
        formIstGeladen = True
        InitCombos()
        initBeteiligtenCMB()
        clsParadigmaRechte.buttons_schalten(btnSpeichern, btnLoeschen)
        gastLayout()
        If _modus = "neu" Then
            'dpAngeordnet.DisplayDate = Now
            'dpEingang.DisplayDate = Now
            'VerschicktAm.DisplayDate = Now 
            myGlobalz.sitzung.aktEreignis.clearValues()
            myGlobalz.sitzung.aktEreignis.Datum = Now
            myGlobalz.sitzung.aktZahlung.clear()
            myGlobalz.sitzung.aktZahlung.VerschicktAm = Now
            'myGlobalz.sitzung.aktZahlung.AngeordnetAm = CLstart.mycSimple.MeinNULLDatumAlsDate
            myGlobalz.sitzung.aktZahlung.Eingang = True
            myGlobalz.sitzung.aktEreignis.Richtung = "Eingang"
            myGlobalz.sitzung.aktZahlung.Typ = "Gebühr"
            myGlobalz.sitzung.aktZahlung.Eingang = True
            myGlobalz.sitzung.aktZahlung.Betrag = 0.00
            btnLoeschen.IsEnabled = False
            'datumsWerteLoeschen()
            VerschicktAm.Value = myGlobalz.sitzung.aktZahlung.VerschicktAm
        End If

        If _modus = "edit" Then
            If myGlobalz.sitzung.aktZahlung.Eingang = True Then
                richtungein.IsChecked = True
                Me.Background = New SolidColorBrush(Colors.PaleTurquoise)
            Else
                richtungaus.IsChecked = True
                Me.Background = New SolidColorBrush(Color.FromRgb(&HC4, &HF0, &HC4))
            End If
            Me.Title &= " " & _modus
            clsEreignisTools.leseEreignisByID(myGlobalz.sitzung.aktEreignis.ID)
            clsEreignisTools.ToObj_Ereignis(myGlobalz.sitzung.tempREC.dt)
            ZahlungToolsNs.leseZahlung.execute(myGlobalz.sitzung.aktEreignis.ID, myGlobalz.sitzung.aktVorgangsID)
            If myGlobalz.sitzung.tempREC.dt.Rows.Count < 1 Then
                MessageBox.Show("Die Zahlung wurde nicht vollständig abgespeichert. Bitte löschen und neu anlegen!")
                Exit Sub
            End If
            ZahlungToolsNs.zahlungDTtoOBJ.execute(myGlobalz.sitzung.tempREC.dt.Rows(0), myGlobalz.sitzung.aktZahlung)
            myGlobalz.sitzung.aktEreignis.Notiz = myGlobalz.sitzung.aktZahlung.Notiz
            tbBetrag.Text = CType(myGlobalz.sitzung.aktZahlung.Betrag, String).Replace(".", ",")
            'datumsWerteLoeschen()
            VerschicktAm.Value = myGlobalz.sitzung.aktZahlung.VerschicktAm
            If glob2.IstDatumSinnvoll(myGlobalz.sitzung.aktZahlung.AngeordnetAm) Then dpAngeordnet.Value = myGlobalz.sitzung.aktZahlung.AngeordnetAm
            If glob2.IstDatumSinnvoll(myGlobalz.sitzung.aktZahlung.EingangAm) Then dpEingang.Value = myGlobalz.sitzung.aktZahlung.EingangAm
            If glob2.IstDatumSinnvoll(myGlobalz.sitzung.aktZahlung.VerschicktAm) Then VerschicktAm.Value = myGlobalz.sitzung.aktZahlung.VerschicktAm

            Title = detailsTools.settitle("Zahlung, " & _modus)
            End If
            ' VerschicktAm.SelectedDate = myGlobalz.sitzung.aktEreignis.Datum
            'Uhrzeitsetzen()
            'If myGlobalz.sitzung.aktZahlung.EingangAm < CDate("1900-01-01") Then tbEingangAm.Text = "" ihah
            'If myGlobalz.sitzung.aktZahlung.AngeordnetAm < CDate("1900-01-01") Then tbAngeordnetAm.Text = "" ihah
            btnSpeichern.IsEnabled = False
        Me.DataContext = myGlobalz.sitzung

    End Sub
    Sub initBeteiligtenCMB()
        'gemeindeDT
        clsBeteiligteBUSI.holeBeteiligte()
        cmbBeteiligte.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub

    Private Sub WINzahlungdetail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show(glob2.getMsgboxText("zahlungsDetailClosing", New List(Of String)(New String() {})),
                       "Zahlungsdetails", _
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Exclamation,
                       MessageBoxResult.OK)
            If Not red = MessageBoxResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub

    'Private Sub datumsWerteLoeschen()
    '    If not glob2.IstDatumSinnvoll(myGlobalz.sitzung.aktZahlung.AngeordnetAm  ) Then
    '        dpAngeordnet.Text = ""
    '        dpAngeordnet.DisplayDate = Now
    '    End If
    '    If not glob2.IstDatumSinnvoll(myGlobalz.sitzung.aktZahlung.EingangAm  ) Then
    '        dpEingang.Text = ""
    '        dpEingang.DisplayDate = Now
    '    End If
    '    If not glob2.IstDatumSinnvoll(myGlobalz.sitzung.aktZahlung.VerschicktAm ) Then
    '        VerschicktAm.Text = ""
    '        VerschicktAm.DisplayDate = Now
    '    End If
    'End Sub


    'Private Sub Uhrzeitsetzen()
    '    tbVerschicktAmStunde.Text = myGlobalz.sitzung.aktZahlung.VerschicktAm.Hour.ToString
    '    tbVerschicktAmMinute.Text = myGlobalz.sitzung.aktZahlung.VerschicktAm.Minute.ToString

    '    tbAngeordnetAmStunde.Text = myGlobalz.sitzung.aktZahlung.AngeordnetAm.Hour.ToString
    '    tbAngeordnetAmMinute.Text = myGlobalz.sitzung.aktZahlung.AngeordnetAm.Minute.ToString
    '    tbEingangAmStunde.Text = myGlobalz.sitzung.aktZahlung.EingangAm.Hour.ToString
    '    tbEingangAmMinute.Text = myGlobalz.sitzung.aktZahlung.EingangAm.Minute.ToString
    'End Sub
    Private Sub abbruch(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub



    'Private Sub dpEingang_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dpEingang.SelectedDateChanged
    '    If Not formIstGeladen Then Exit Sub
    '    'Dim c As Date = (CDate(dpEingang.SelectedDate) + Now.TimeOfDay)
    '    'myGlobalz.sitzung.aktZahlung.EingangAm = c
    '    btnSpeichern.IsEnabled = True
    '    e.Handled = True
    'End Sub

    Private Sub cmbTyp_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbTyp.SelectionChanged
        myGlobalz.sitzung.aktZahlung.Typ = cmbTyp.SelectedValue.ToString
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub
    Private Sub cmbHHST_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbHHST.SelectionChanged
        myGlobalz.sitzung.aktZahlung.HausHaltsstelle = cmbHHST.SelectedValue.ToString
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub InitCombos()
        detailsTools.initErgeinistypCombo(Me, "Zahlung_typ.xml", "XMLSourceComboBoxTyp") : cmbTyp.SelectedIndex = 0

        'cmbTyp.Items.Add("Abgabe")
        'cmbTyp.Items.Add("Beihilfe")
        'cmbTyp.Items.Add("Bußgeld")
        'cmbTyp.Items.Add("Ersatzgeld")
        'cmbTyp.Items.Add("Gebühr")
        'cmbTyp.Items.Add("Rechnung")
        'cmbTyp.Items.Add("Sicherheitsleistung")
        'cmbTyp.Items.Add("Sonstige")
        'cmbTyp.Items.Add("Schornsteinfegergebühr")
        'cmbTyp.Items.Add("Verwaltungsgebühr")
        'cmbTyp.Items.Add("Walderhaltungsabgabe")
        'cmbTyp.Items.Add("Zwangsgeld")





        cmbHHST.Items.Add("67.01.01.51000010")
        cmbHHST.Items.Add("67.01.01.51500010")
        cmbHHST.Items.Add("67.01.03.51000010")
        cmbHHST.Items.Add("67.01.03.51500010")
        cmbHHST.Items.Add("67.01.03/4011.82900216")
    End Sub

    'Private Sub datumsEingabeGgfAufNothingSetzen()
    '    If VerschicktAm.SelectedDate Is Nothing Then
    '        'VerschicktAm.SelectedDate = CDate("#1:01:00 AM#")
    '    End If
    '    If dpAngeordnet.SelectedDate Is Nothing Then
    '        'dpAngeordnet.SelectedDate = CDate("#1:01:00 AM#")
    '    End If
    '    If dpEingang.SelectedDate Is Nothing Then
    '        'dpEingang.SelectedDate = CDate("#1:01:00 AM#")
    '    End If
    'End Sub
    'Private Sub datumsPropertiesBerechnen()
    '    myGlobalz.sitzung.aktZahlung.VerschicktAm = clsEreignisTools.GetGesammelteDatumUhrzeit(CDate(VerschicktAm.SelectedDate).Date, tbVerschicktAmStunde.Text, tbVerschicktAmMinute.Text)
    '    myGlobalz.sitzung.aktZahlung.AngeordnetAm = clsEreignisTools.GetGesammelteDatumUhrzeit(CDate(dpAngeordnet.SelectedDate).Date, tbAngeordnetAmStunde.Text, tbAngeordnetAmMinute.Text)
    '    myGlobalz.sitzung.aktZahlung.EingangAm = clsEreignisTools.GetGesammelteDatumUhrzeit(CDate(dpEingang.SelectedDate).Date, tbEingangAmStunde.Text, tbEingangAmMinute.Text)
    'End Sub
    Private Sub btnZahlungSpeichernClick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnSpeichern.IsEnabled = False
        If tbBetrag.Text.IsNothingOrEmpty Then
            MessageBox.Show("Bitte einen Betrag eingeben!")
            Exit Sub
        End If
        myGlobalz.sitzung.aktZahlung.Betrag = CDbl(tbBetrag.Text.Replace(".", ","))
        If _modus = "neu" Then
            If Not alleZahlungsEingabenOK() Then Exit Sub
            myGlobalz.sitzung.aktZahlung.Notiz = myGlobalz.sitzung.aktEreignis.Notiz
            nachricht(" speichern ")
            myGlobalz.sitzung.aktZahlung.VerschicktAm = glob2.getDatumFromControl(VerschicktAm.Value)
            myGlobalz.sitzung.aktZahlung.AngeordnetAm = glob2.getDatumFromControl(dpAngeordnet.Value)
            myGlobalz.sitzung.aktZahlung.EingangAm = glob2.getDatumFromControl(dpEingang.Value)

            VerschicktAm.Value = myGlobalz.sitzung.aktEreignis.Datum
            myGlobalz.sitzung.aktEreignis.typnr = 3
            ZahlungToolsNs.zahlung_und_ereignis_speichern_alleDB.execute(_modus, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktEreignis)

            nachricht(" zahlungsid% = " & myGlobalz.sitzung.aktZahlung.ZahlungsID)
            '  Dim d = myGlobalz.sitzung.aktEreignis.Notiz
            CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & ": neue Zahlung angelegt" : CLstart.myc.aLog.log()
            btnSpeichern.IsEnabled = False
            Me.Close()
        End If
        If _modus = "edit" Then
            If Not alleZahlungsEingabenOK() Then Exit Sub
            myGlobalz.sitzung.aktZahlung.Notiz = myGlobalz.sitzung.aktEreignis.Notiz
            'VerschicktAm.Value = myGlobalz.sitzung.aktEreignis.Datum
            myGlobalz.sitzung.aktZahlung.VerschicktAm = glob2.getDatumFromControl(VerschicktAm.Value)
            myGlobalz.sitzung.aktZahlung.AngeordnetAm = glob2.getDatumFromControl(dpAngeordnet.Value)
            myGlobalz.sitzung.aktZahlung.EingangAm = glob2.getDatumFromControl(dpEingang.Value)
            ZahlungToolsNs.zahlung_und_ereignis_speichern_alleDB.execute(_modus, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktEreignis)
            'nachricht("zahlungsid% = " & ZahlungToolsNs.ZahlungEdit_alleDB.execute(myGlobalz.sitzung.aktZahlung.ZahlungsID).ToString)
            CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & ": Zahlung editiert" : CLstart.myc.aLog.log()
            IsEnabled = False
            Me.Close()
        End If
        e.Handled = True
    End Sub



    Shared Function alleZahlungsEingabenOK() As Boolean
        If myGlobalz.sitzung.aktZahlung.Betrag = 0 Then
            MessageBox.Show("Sie haben keinen Betrag eingegeben!")
            Return False
        End If
        Return True
    End Function



    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zahlungUndEreignisLoeschen()
        e.Handled = True
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
        If tbBetrag.Text.Contains(".") Then
            MsgBox("Bitte das Komma als Dezimalzeichen verwenden")
        End If
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbBeschreibung_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    'Private Sub tbEingang_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEingang.TextChanged
    '    glob2.schliessenButton_einschalten(btnSpeichern)
    '    e.Handled = True
    'End Sub



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
        glob2.EmailFormOEffnen("umwelt@kreis-offenbach.de",
                              " = Bitte eine Annahmeordnung erstellen. ",
                              String.Format(" Bitte eine Annahmeordnung erstellen. VorgangsNr: {0}{1}", myGlobalz.sitzung.aktVorgangsID, vbCrLf),
                              "",
                              myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, False)
        e.Handled = True
    End Sub


    Private Sub btnExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ZahlungToolsNs.ExcelausgabeExtracted.execute() '  clsZAHLUNGDB_Mysql.ExcelausgabeExtracted()
        e.Handled = True
    End Sub

    'Private Sub dpAngeordnet_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
    '    Dim c As Date = (CDate(dpAngeordnet.SelectedDate) + Now.TimeOfDay)
    '    myGlobalz.sitzung.aktZahlung.AngeordnetAm = c
    '    btnSpeichern.IsEnabled = True
    '    e.Handled = True
    'End Sub



    Private Sub tbVerschicktAmStunde_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not formIstGeladen Then Exit Sub
        btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbVerschicktAmMinute_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not formIstGeladen Then Exit Sub
        btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub
    'Private Sub VerschicktAm_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles VerschicktAm.SelectedDateChanged
    '    glob2.schliessenButton_einschalten(btnSpeichern)
    '    e.Handled = True
    'End Sub
    'Private Sub Angeordnet_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) _
    '    Handles dpAngeordnet.SelectedDateChanged
    '    glob2.schliessenButton_einschalten(btnSpeichern)
    '    e.Handled = True
    'End Sub
    'Private Sub Eingang_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) _
    '    Handles dpEingang.SelectedDateChanged
    '    glob2.schliessenButton_einschalten(btnSpeichern)
    '    e.Handled = True
    'End Sub
    '    Private Sub dpVerschickt_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dpVerschickt.SelectedDateChanged
    '    Dim c As Date = (CDate(dpVerschickt.SelectedDate) + Now.TimeOfDay)
    '    myGlobalz.sitzung.aktZahlung.VerschicktAm = c
    '    e.Handled = True
    'End Sub
    '    Private Sub tbVerschickt_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVerschickt.TextChanged
    '    glob2.schliessenButton_einschalten(btnSpeichern)
    '    e.Handled = True
    'End Sub

    Private Sub tbAngeordnetAmStunde_TextChanged(sender As Object, e As TextChangedEventArgs)
        btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbAngeordnetAmMinute_TextChanged(sender As Object, e As TextChangedEventArgs)
        btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbEingangAmStunde_TextChanged(sender As Object, e As TextChangedEventArgs)
        btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbEingangAmMinute_TextChanged(sender As Object, e As TextChangedEventArgs)
        btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub dpAngeordnet_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
        e.Handled = True
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub dpEingang_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
        e.Handled = True
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub VerschicktAm_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
        e.Handled = True
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub
End Class
