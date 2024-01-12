Public Class Win_Stamm
    Private Property LokaleStamm As New Stamm(CLstart.mycSimple.MeinNULLDatumAlsDate)
    Private Property MerkerStamm As New Stamm(CLstart.mycSimple.MeinNULLDatumAlsDate)
    Private _titel, _gemKRZ, _ProbaugAZ As String
    Private Property anyChange As Boolean = False
    Private _modus As String = "edit"
    Public Property abbruch As Boolean = False
    Private Sub gastLayout()
        Background = myglobalz.GetSecondBackground()
    End Sub

    Sub New(ByVal modus As String, ByRef meinAZ As Stamm, titel As String, gemKRZ As String, ProbaugAZ As String)
        InitializeComponent()
        _modus = modus
        StammToolsNs.stammObjektKopieren.exe(meinAZ, LokaleStamm)
        StammToolsNs.stammObjektKopieren.exe(meinAZ, MerkerStamm)
        _titel = titel
        _gemKRZ = gemKRZ
        _ProbaugAZ = ProbaugAZ
    End Sub

    Private Sub Win_Stamm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'es gibt nur den edit mode
        e.Handled = True
        dockMain.DataContext = LokaleStamm
        glob2.initGemKRZCombo(Me)
        glob2.initCMBParagraf(Me)
        glob2.initRaumNrCombo(Me)
        setComboboxStatus()
        setComboboxStatusObjekt(_modus)
        If _modus = "neu" Then
            myGlobalz.sitzung.aktVorgangsID = 0
            LokaleStamm.clear()
            MerkerStamm.clear()
            LokaleStamm.Eingangsdatum = Now
            LokaleStamm.LetzteBearbeitung = Now 'myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung
            LokaleStamm.Aufnahmedatum = Now
            FocusManager.SetFocusedElement(Me, btnchangeAZneu)
            btnchangeAZneu.Content = "erzeugen"
            'changeAZ.Visibility = Windows.Visibility.Hidden
            changeAZneu_ClickExtracted()

            btnchangeAZneu.Content = "Ändern"
            If myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer Or
                 (myGlobalz.sitzung.aktBearbeiter.ID = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID) Then

                LokaleStamm.Probaugaz = _ProbaugAZ
                LokaleStamm.Beschreibung = _titel
                LokaleStamm.GemKRZ = _gemKRZ
            End If
            'MerkerStamm.WeitereBearbeiter = CLstart.myc.userIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")
            'LokaleStamm.WeitereBearbeiter = MerkerStamm.WeitereBearbeiter
            'myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = MerkerStamm.WeitereBearbeiter
            Dim tempuserIniProfile = New CLstart.clsINIDatei(IO.Path.Combine(initP.getValue("Haupt.UserInis"), myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale & ".ini"))
            myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = tempuserIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")

            Debug.Print(myGlobalz.sitzung.aktBearbeiter.Initiale)

            MerkerStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
            LokaleStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
        End If
        If _modus = "edit" Then
            StammToolsNs.stammObjektKopieren.exe(LokaleStamm, myGlobalz.sitzung.aktVorgang.Stammdaten)
            setWeitereBearbeiterAuswahlsichtbar()
            btnSpeichern.IsEnabled = False
            anyChange = False
            FocusManager.SetFocusedElement(Me, btnAbbrechen)
            btnchangeAZneu.Content = "bearbeiten"
            'changeAZ.Visibility = Windows.Visibility.Visible
            ' clsParadigmaRechte.buttons_schalten(btnSpeichern, btnSpeichern)
            Dim tempuserIniProfile = New CLstart.clsINIDatei(IO.Path.Combine(initP.getValue("Haupt.UserInis"), myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale & ".ini"))
            If myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.IsNothingOrEmpty Then
                myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = tempuserIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")
            End If
            MerkerStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
            LokaleStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter

            If Not myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
                btnSpeichern.Visibility = Visibility.Collapsed
            End If
            If StammToolsNs.istAzNachAltemAktenplan.exe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then
                hinweisInfosEinschalten()
            End If
        End If
        Dim info As String = ""
        If DS_Tools.istHauptBearbeiter(info) Or
            myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer Then
            btnchangeAZneu.IsEnabled = True
        Else
            btnchangeAZneu.IsEnabled = False
            btnchangeAZneu.ToolTip = "Bitte wenden Sie sich an die HauptsachbearbeiterIn oder an die FD-Assistenz !"
        End If
        Title = StammToolsNs.setWindowTitel.exe(_modus, "Stammdaten")
        gastLayout()
    End Sub


    Private Sub tbBeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        glob2.istTextzulang(540, tbBeschreibung)
        LokaleStamm.Beschreibung = tbBeschreibung.Text
        If LokaleStamm.Beschreibung <> MerkerStamm.Beschreibung Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub tbProbaugAZ2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        glob2.istTextzulang(145, tbProbaugAZ2)
        LokaleStamm.Probaugaz = tbProbaugAZ2.Text
        If LokaleStamm.Probaugaz <> MerkerStamm.Probaugaz Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub tbAltAzAllgemein_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)

        glob2.istTextzulang(145, tbAltAzAllgemein)
        LokaleStamm.AltAz = tbAltAzAllgemein.Text
        If LokaleStamm.AltAz <> MerkerStamm.AltAz Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub cmbGemKRZ_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbGemKRZ.SelectedItem Is Nothing Then Exit Sub
        Dim item As String = CType(cmbGemKRZ.SelectedValue, String)
        LokaleStamm.GemKRZ = cmbGemKRZ.SelectedValue.ToString
        tbgemkrz.Text = LokaleStamm.GemKRZ
        If cmbGemKRZ.SelectedValue.ToString = "ALLE-" Then LokaleStamm.GemKRZ = "" '
        If LokaleStamm.GemKRZ <> MerkerStamm.GemKRZ Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub btnWeitereBearbeiterListen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl("mehrfachauswahlInitial", CLstart.myc.AZauswahl.WeitereBearbeiter)
        bearbeiterauswahlbox.ShowDialog()

        If bearbeiterauswahlbox.mehrfachauswahlsumme = "####" Then
            'CLstart.myc.AZauswahl.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
            CLstart.myc.AZauswahl.WeitereBearbeiter = tbWeitereBearbeiter2.Text
        Else
            CLstart.myc.AZauswahl.WeitereBearbeiter = bearbeiterauswahlbox.mehrfachauswahlsumme
        End If

        tbWeitereBearbeiter2.Text = CLstart.myc.AZauswahl.WeitereBearbeiter
        LokaleStamm.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
        'glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub
    Private Sub changeAZneu_ClickExtracted()
        Dim nnn As New winAZdefineNEU(_modus, LokaleStamm)
        nnn.ShowDialog()
        If nnn.abbruch Then
            abbruch = True
            Close()
            Exit Sub
        End If
        If LokaleStamm.az.gesamt <> MerkerStamm.az.gesamt Then glob2.schliessenButton_einschalten(btnSpeichern)
        If LokaleStamm.az.sachgebiet.Zahl Is Nothing Then
            Exit Sub
        End If
        If LokaleStamm.az.sachgebiet.Zahl.Trim = "3311" Then
            cmbParagraf.IsDropDownOpen = True
            tbParagraf.Background = New SolidColorBrush(Colors.LightPink)
        End If
    End Sub

    Private Sub changeAZneu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If _modus = "edit" Then
            If StammToolsNs.istAzNachAltemAktenplan.exe(myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then
                MessageBox.Show(glob2.getMsgboxText("alterAktenplan", New List(Of String)(New String() {CStr(myglobalz.sitzung.aktVorgangsID)})),
                                "Vorsicht: Alter Aktenplan erforderlich", MessageBoxButton.OK, MessageBoxImage.Error)
                Exit Sub
            End If
        End If
        changeAZneu_ClickExtracted()
        e.Handled = True
    End Sub

    'Private Sub changeAZ_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles changeAZ.Click
    '    e.Handled = True
    '    If Not StammToolsNs.istAzNachAltemAktenplan.exe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then
    '        MessageBox.Show(glob2.getMsgboxText("alterAktenplan", New List(Of String)(New String() {CStr(myGlobalz.sitzung.aktVorgangsID)})),
    '                        "Vorsicht: Neuer Aktenplan", MessageBoxButton.OK, MessageBoxImage.Error)
    '        Exit Sub
    '    End If
    '    Dim neuesaz As New Win_Azaenderneinzeln
    '    neuesaz.ShowDialog()
    'End Sub

    Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBemerkung.TextChanged
        glob2.istTextzulang(540, tbBemerkung)
        LokaleStamm.Bemerkung = tbBemerkung.Text
        If LokaleStamm.Bemerkung <> MerkerStamm.Bemerkung Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    'Private Sub btnAllgemeinLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    '  anyChange = True ': glob2.schliessenButton_einschalten(btnSpeichern)
    'End Sub

    Private Sub tbgemkrz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        anyChange = True ': glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub
    Private Sub setComboboxStatus()
        cmbStatus.Items.Add("unerledigt")
        cmbStatus.Items.Add("erledigt")
        'cmbStatus.SelectedIndex = 1
    End Sub

    Private Sub setComboboxStatusObjekt(ByVal _modus$)
        If _modus = "neu" Then
            LokaleStamm.erledigt = False
        End If
        'If _modus = "edit" Then

        'End If
        If LokaleStamm.erledigt Then
            cmbStatus.SelectedIndex = 1
        Else
            cmbStatus.SelectedIndex = 0
        End If
    End Sub

    'Private Sub btnAllgemein_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichern.Click
    '    If Not speichernAllgemein() Then Exit Sub
    '    btnSpeichern.IsEnabled = False
    '    e.Handled = True
    '    Me.Close()
    'End Sub

    Private Sub DatePickerLetzteBearbeitung_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePickerLetzteBearbeitung.SelectedDateChanged
        e.Handled = True
        If LokaleStamm.LetzteBearbeitung <> MerkerStamm.LetzteBearbeitung Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub DatePickerEingang_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePickerEingang.SelectedDateChanged
        e.Handled = True
        LokaleStamm.Eingangsdatum = CDate(DatePickerEingang.SelectedDate)
        If LokaleStamm.Eingangsdatum <> MerkerStamm.Eingangsdatum Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub DatePickerAnlage_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePickerAnlage.SelectedDateChanged
        e.Handled = True
        If LokaleStamm.Aufnahmedatum <> MerkerStamm.Aufnahmedatum Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    'Private Sub cmbStatus_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStatus.SelectionChanged
    '    anyChange = True ': glob2.schliessenButton_einschalten(btnSpeichern)
    'End Sub

    Private Sub cmbStatus_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStatus.SelectionChanged
        e.Handled = True
        Try
            Dim item As String = CType(cmbStatus.SelectedValue, String)
            If item = "erledigt" Then
                LokaleStamm.erledigt = True
                If LokaleStamm.erledigt <> MerkerStamm.erledigt Then
                    '    LokaleStamm.LetzteBearbeitung = CDate(Now) 'wunsch von frau weyers: raus damit
                End If
            Else
                LokaleStamm.erledigt = False
            End If
            If LokaleStamm.erledigt <> MerkerStamm.erledigt Then glob2.schliessenButton_einschalten(btnSpeichern)
        Catch ex As Exception
            nachricht_und_Mbox("cmbStatus_SelectionChanged. " ,ex)
        End Try
    End Sub

    Private Sub ckbGutachtenvorhanden_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbGutachtenvorhanden.Checked
        e.Handled = True
        If LokaleStamm.meinGutachten.existiert <> MerkerStamm.meinGutachten.existiert Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub ckbGutachtenInDokumente_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbGutachtenInDokumente.Checked
        e.Handled = True
        If LokaleStamm.meinGutachten.UnterDokumente <> MerkerStamm.meinGutachten.UnterDokumente Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub cmbProduktgruppe_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbProduktgruppe.SelectionChanged
        e.Handled = True
        ' If LokaleStamm. <> MerkerStamm.Aufnahmedatum Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub cmbRaumNr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        e.Handled = True
        If cmbRaumNr.SelectedValue Is Nothing Then Exit Sub
        Dim item As String = CType(cmbRaumNr.SelectedValue, String)
        LokaleStamm.Standort.RaumNr = cmbRaumNr.SelectedValue.ToString
        anyChange = True ': glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub tbRaumnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        e.Handled = True
        LokaleStamm.Standort.RaumNr = tbRaumnr.Text
        If LokaleStamm.Standort.RaumNr <> MerkerStamm.Standort.RaumNr Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub



    Private Sub StandortTitel_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        e.Handled = True
        If StandortTitel IsNot Nothing Then
            LokaleStamm.Standort.Titel = StandortTitel.Text
            If LokaleStamm.Standort.Titel <> MerkerStamm.Standort.Titel Then glob2.schliessenButton_einschalten(btnSpeichern)
        End If
    End Sub

    Private Sub btnAbbrechen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        LokaleStamm = Nothing
        Me.Close()
    End Sub

    Function speichernStammdaten() As Boolean
        Try

            If Not NEU_eingabenOk() Then Return False

        Catch ex As Exception
            l(ex.ToString())
        End Try
        If Not NEUform2objok() Then Return False
        l(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
        If Not persistiereVorgangStammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten) Then
            MsgBox("Problem beim Abspeichern der Stammdaten")
            DialogResult = False
            Return False
        End If
        If _modus = "neu" Then
            DialogResult = True 'regelt den direktaufruf des 
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(myGlobalz.sitzung.aktVorgangsID.ToString,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)
            '     HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2,myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

        Else
            DialogResult = False
        End If
        Return True
    End Function

    Function NEU_eingabenOk() As Boolean
        If String.IsNullOrEmpty(LokaleStamm.az.gesamt) Then
            MessageBox.Show("Sie müssen noch ein Aktenzeichen festlegen. Sonst kann der Vorgang nicht angelegt werden. " &
                            "Klicken Sie auf den blinkenden Knopf!",
                            "Aktenzeichen fehlt", MessageBoxButton.OK, MessageBoxImage.Information)
            FocusManager.SetFocusedElement(Me, btnchangeAZneu)
            Return False
        End If
        Try

            'If LokaleStamm.az.sachgebiet.Zahl = "3311" Then
            '    If String.IsNullOrEmpty(LokaleStamm.Paragraf.Trim) Then
            '        MessageBox.Show("Bei Vorgängen aus dem Sachgebiet '3311' MUSS ein relevanter Paragraf ($ 34, $ 35) angegeben werden!" &
            '                    "", "Paragraf fehlt", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '        tbParagraf.Background = New SolidColorBrush(Colors.LightPink)
            '        cmbParagraf.IsDropDownOpen = True
            '        Return False
            '    End If
            'End If

        Catch ex As Exception
            l(ex.ToString())
        End Try
        Return True
    End Function

    Function NEUform2objok() As Boolean

        ' myGlobalz.sitzung.Vorgang.Stammdaten = CType(LokaleStamm.Clone, Stamm)
        StammToolsNs.stammObjektKopieren.exe(LokaleStamm, myglobalz.sitzung.aktVorgang.Stammdaten)
        Return True
    End Function


    Private Function persistiereVorgangStammdaten(ByVal _meinstamm As Stamm) As Boolean 'tbStammbeschreibung.Text, tbBeschreibung_nummer.Text
        btnSpeichern.IsEnabled = False
        nachricht("Eingabe ist ok")
        l(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
        l(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID.ToString)
        If _modus = "neu" Then
            glob2.NEU_VorgangStamm_2DBOk()  'abspeichern in der db
            myglobalz.sitzung.modus = "edit"
            Return True

        Else
            glob2.EDIT_VorgangStamm_2DBOk()  'abspeichern in der db
            myglobalz.sitzung.modus = "edit"
            setWeitereBearbeiterAuswahlsichtbar()
            Return True
        End If
        Return False
    End Function

    Private Sub setWeitereBearbeiterAuswahlsichtbar()
        btnWeitereBearbeiterListen.IsEnabled = False
        btnWeitereBearbeiterListen.IsEnabled = If(myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter), True, False)
        If myglobalz.sitzung.aktBearbeiter.Initiale.ToLower = myglobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale.ToLower Then
            btnWeitereBearbeiterListen.IsEnabled = True
        End If
        l(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
    End Sub

    Private Sub tbWeitereBearbeiter2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        LokaleStamm.WeitereBearbeiter = tbWeitereBearbeiter2.Text
        If LokaleStamm.WeitereBearbeiter <> MerkerStamm.WeitereBearbeiter Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub chkdarfnichtvernichtetwerden_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkdarfnichtvernichtetwerden.Checked, chkdarfnichtvernichtetwerden.Unchecked
        If LokaleStamm.darfNichtVernichtetWerden <> MerkerStamm.darfNichtVernichtetWerden Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        If Not myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
            MsgBox("Sie sind nicht authorisiertÄnderungen vorzunehmen.")
            e.Handled = True
            Me.Close()
            Exit Sub
        End If
        l(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
        If Not speichernStammdaten() Then Exit Sub
        btnSpeichern.IsEnabled = False
        e.Handled = True
        Me.Close()
    End Sub

    Private Sub ckbAbgabeBA_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbAbgabeBA.Checked, ckbAbgabeBA.Unchecked
        If LokaleStamm.AbgabeBA <> MerkerStamm.AbgabeBA Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub cmbParagraf_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbParagraf.SelectedItem Is Nothing Then Exit Sub
        Dim item As String = CType(cmbParagraf.SelectedValue, String)
        tbParagraf.Text = cmbParagraf.SelectedValue.ToString
        LokaleStamm.Paragraf = cmbParagraf.SelectedValue.ToString
        If LokaleStamm.Paragraf <> MerkerStamm.Paragraf Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbParagraf_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        glob2.istTextzulang(145, tbParagraf)
        LokaleStamm.Paragraf = tbParagraf.Text
        If LokaleStamm.Paragraf <> MerkerStamm.Paragraf Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbInterneNr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        glob2.istTextzulang(235, tbInterneNr)
        LokaleStamm.InterneNr = tbInterneNr.Text
        If LokaleStamm.InterneNr <> MerkerStamm.InterneNr Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub hinweisInfosEinschalten()
        btnHinweisAlterAktenPlan.Visibility = Windows.Visibility.Visible
    End Sub


    Private Sub btnHinweisAlterAktenPlan_Click(sender As Object, e As RoutedEventArgs)
        MsgBox(glob2.getMsgboxText("altesAktenzeichenKonvertieren", New List(Of String)(New String() {})))
        e.Handled = True
    End Sub
End Class
