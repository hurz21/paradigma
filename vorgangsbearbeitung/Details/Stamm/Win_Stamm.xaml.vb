Imports paradigma.CLstart

Public Class Win_Stamm
    Private Property LokaleStamm As New Stamm(CLstart.mycSimple.MeinNULLDatumAlsDate)
    Private Property MerkerStamm As New Stamm(CLstart.mycSimple.MeinNULLDatumAlsDate)
    Private _titel, _gemKRZ, _ProbaugAZ As String
    Private Property anyChange As Boolean = False
    Private _modus As String = "edit"

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
        Debug.Print(myGlobalz.sitzung.aktBearbeiter.ID.ToString)
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
            'LokaleStamm.WeitereBearbeiter = CLstart.myc.userIniProfile.WertLesen("Stammdaten", "weiterebearbeiter") 
            'myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = CLstart.myc.userIniProfile.WertLesen("Stammdaten", "weiterebearbeiter") 
            LokaleStamm.LetzteBearbeitung = Now 'myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung
            LokaleStamm.Aufnahmedatum = Now
            FocusManager.SetFocusedElement(Me, changeAZneu)
            changeAZneu.Content = "erzeugen"
            'changeAZ.Visibility = Windows.Visibility.Hidden
            If changeAZneu_ClickExtracted() Then
                MsgBox("Es wurde kein Aktenzeichen oder Sachgebiet angewählt. Bitte abbrechen!")
            End If
            changeAZneu.Content = "Ändern"
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                LokaleStamm.Probaugaz = _ProbaugAZ
                LokaleStamm.Beschreibung = _titel
                LokaleStamm.GemKRZ = _gemKRZ
            End If
            'MerkerStamm.WeitereBearbeiter = CLstart.myc.userIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")
            'LokaleStamm.WeitereBearbeiter = MerkerStamm.WeitereBearbeiter
            'myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = MerkerStamm.WeitereBearbeiter

            Dim tempuserIniProfile = New clsINIDatei(IO.Path.Combine(initP.getValue("Haupt.UserInis"), myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale & ".ini"))
            myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = tempuserIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")
            MerkerStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
            LokaleStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
            'myGlobalz.sitzung.aktVorgangsID = LokaleStamm.

        End If
        If _modus = "edit" Then
            StammToolsNs.stammObjektKopieren.exe(LokaleStamm, myGlobalz.sitzung.aktVorgang.Stammdaten)
            MerkerStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
            LokaleStamm.WeitereBearbeiter = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
            setWeitereBearbeiterAuswahlsichtbar()
            btnSpeichern.IsEnabled = False
            anyChange = False
            FocusManager.SetFocusedElement(Me, btnAbbrechen)
            changeAZneu.Content = "bearbeiten"
            'changeAZ.Visibility = Windows.Visibility.Visible
            ' clsParadigmaRechte.buttons_schalten(btnSpeichern, btnSpeichern)
            If Not clsParadigmaRechte.binEignerOderAdmin() Then
                btnSpeichern.Visibility = Visibility.Collapsed
            End If
        End If

        Title = StammToolsNs.setWindowTitel.exe(_modus, "Stammdaten")
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
        e.Handled = True
        If cmbGemKRZ.SelectedItem Is Nothing Then Exit Sub
        Dim item As String = CType(cmbGemKRZ.SelectedValue, String)
        LokaleStamm.GemKRZ = cmbGemKRZ.SelectedValue.ToString
        tbgemkrz.Text = LokaleStamm.GemKRZ
        If cmbGemKRZ.SelectedValue.ToString = "ALLE-" Then LokaleStamm.GemKRZ = "" '
        If LokaleStamm.GemKRZ <> MerkerStamm.GemKRZ Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub btnWeitereBearbeiterListen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        glob2.setzeZugriffsrechte()
        tbWeitereBearbeiter2.Text = CLstart.myc.AZauswahl.WeitereBearbeiter
        LokaleStamm.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
        'glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub
    Private Function changeAZneu_ClickExtracted() As Boolean
        Dim nnn As New winAZdefineNEU(_modus, LokaleStamm)
        nnn.ShowDialog()
        If LokaleStamm.az.gesamt <> MerkerStamm.az.gesamt Then glob2.schliessenButton_einschalten(btnSpeichern)
        If LokaleStamm.az.sachgebiet.Zahl Is Nothing Then
            Return False
        End If
        If LokaleStamm.az.sachgebiet.Zahl.Trim = "3311" Then
            cmbParagraf.IsDropDownOpen = True
            tbParagraf.Background = New SolidColorBrush(Colors.LightPink)
        End If
    End Function

    Private Sub changeAZneu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If _modus = "edit" Then
            If StammToolsNs.istAzNachAltemAktenplan.exe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then
                MessageBox.Show("Es handelt sich bei Vorgang Nr.: " & myGlobalz.sitzung.aktVorgangsID & " um ein Az nach dem alten Aktenplan!" & vbCrLf &
                                "Bitte benutzen Sie zur Änderung das entsprechende Werkzeug. " & vbCrLf &
                                "Bei Unklarheiten wenden Sie sich bitte an den Admin.",
                                "Vorsicht: Alter Aktenplan erforderlich", MessageBoxButton.OK, MessageBoxImage.Error)
                Exit Sub
            End If
        End If
        If myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.IsNothingOrEmpty OrElse
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Contains("XXXX") Then
            MessageBox.Show("Bitte erst die Stammdaten speichern. Dann können Sie sie wieder ändern.",
                            "Stammdaten ungesichert", MessageBoxButton.OK, MessageBoxImage.Error)
            Exit Sub
        End If
        changeAZneu_ClickExtracted()
        e.Handled = True
    End Sub

    'Private Sub changeAZ_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles changeAZ.Click
    '  '  MsgBox("bitte wenden sie sich hierzu an den ")
    '    If Not StammToolsNs.istAzNachAltemAktenplan.exe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then
    '        MessageBox.Show("Es handelt sich bei Vorgang Nr.: " & myGlobalz.sitzung.aktVorgangsID & " um ein Az nach dem neuen Aktenplan!" & vbCrLf &
    '                        "Bitte benutzen Sie zur Änderung das entsprechende Werkzeug. " & vbCrLf &
    '                        "Bei Unklarheiten wenden Sie sich bitte an den Admin.",
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

    Private Sub btnAllgemein_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichern.Click
        e.Handled = True
        'speichern
        Dim dummy As Boolean
        Dim st As New clsStammTools
        If Not NEU_eingabenOk() Then
            btnSpeichern.IsEnabled = False
            e.Handled = True
            Me.Close()
        End If
        If Not NEUform2objok() Then
            btnSpeichern.IsEnabled = False
            e.Handled = True
            Me.Close()
        End If
        btnSpeichern.IsEnabled = False
        If Not st.speichernAllgemein(dummy, _modus) Then

            Exit Sub
        End If
        setWeitereBearbeiterAuswahlsichtbar()
        btnSpeichern.IsEnabled = False
        DialogResult = True
        Me.Close()
    End Sub

    Private Sub DatePickerLetzteBearbeitung_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePickerLetzteBearbeitung.SelectedDateChanged
        If LokaleStamm.LetzteBearbeitung <> MerkerStamm.LetzteBearbeitung Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub DatePickerEingang_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePickerEingang.SelectedDateChanged
        LokaleStamm.Eingangsdatum = CDate(DatePickerEingang.SelectedDate)
        If LokaleStamm.Eingangsdatum <> MerkerStamm.Eingangsdatum Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub DatePickerAnlage_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePickerAnlage.SelectedDateChanged
        If LokaleStamm.Aufnahmedatum <> MerkerStamm.Aufnahmedatum Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    'Private Sub cmbStatus_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStatus.SelectionChanged
    '    anyChange = True ': glob2.schliessenButton_einschalten(btnSpeichern)
    'End Sub

    Private Sub cmbStatus_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStatus.SelectionChanged
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
            nachricht_und_Mbox("cmbStatus_SelectionChanged. " & ex.ToString)
        End Try
    End Sub

    Private Sub ckbGutachtenvorhanden_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbGutachtenvorhanden.Checked
        If LokaleStamm.meinGutachten.existiert <> MerkerStamm.meinGutachten.existiert Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub ckbGutachtenInDokumente_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbGutachtenInDokumente.Checked
        If LokaleStamm.meinGutachten.UnterDokumente <> MerkerStamm.meinGutachten.UnterDokumente Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub cmbProduktgruppe_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbProduktgruppe.SelectionChanged
        ' If LokaleStamm. <> MerkerStamm.Aufnahmedatum Then glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub cmbRaumNr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Dim item As String = CType(cmbRaumNr.SelectedValue, String)
        LokaleStamm.Standort.RaumNr = cmbRaumNr.SelectedValue.ToString
        anyChange = True ': glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbRaumnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        LokaleStamm.Standort.RaumNr = tbRaumnr.Text
        If LokaleStamm.Standort.RaumNr <> MerkerStamm.Standort.RaumNr Then glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub



    Private Sub StandortTitel_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        If StandortTitel IsNot Nothing Then
            LokaleStamm.Standort.Titel = StandortTitel.Text
            If LokaleStamm.Standort.Titel <> MerkerStamm.Standort.Titel Then glob2.schliessenButton_einschalten(btnSpeichern)
        End If
        e.Handled = True
    End Sub

    Private Sub btnAbbrechen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        LokaleStamm = Nothing
        Me.Close()
        e.Handled = True
    End Sub



    Function NEU_eingabenOk() As Boolean
        If String.IsNullOrEmpty(LokaleStamm.az.gesamt) Then
            MessageBox.Show("Sie müssen noch ein Aktenzeichen festlegen. Sonst kann der Vorgang nicht angelegt werden. Klicken Sie auf den blinkenden Knopf!", "Aktenzeichen fehlt", MessageBoxButton.OK, MessageBoxImage.Information)
            FocusManager.SetFocusedElement(Me, changeAZneu)
            Return False
        End If

        If LokaleStamm.az.sachgebiet.Zahl = "3311" Then
            If String.IsNullOrEmpty(LokaleStamm.Paragraf.Trim) Then
                MessageBox.Show("Bei Vorgängen aus dem Sachgebiet '3311' MUSS ein relevanter Paragraf ($ 34,$ 35) angegeben werden!" &
                                "", "Paragraf fehlt", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK)
                tbParagraf.Background = New SolidColorBrush(Colors.LightPink)
                cmbParagraf.IsDropDownOpen = True
                Return False
            End If
        End If
        Return True
    End Function

    Function NEUform2objok() As Boolean

        ' myGlobalz.sitzung.Vorgang.Stammdaten = CType(LokaleStamm.Clone, Stamm)
        StammToolsNs.stammObjektKopieren.exe(LokaleStamm, myGlobalz.sitzung.aktVorgang.Stammdaten)
        Return True
    End Function




    Private Sub setWeitereBearbeiterAuswahlsichtbar()
        btnWeitereBearbeiterListen.IsEnabled = False
        btnWeitereBearbeiterListen.IsEnabled = If(clsParadigmaRechte.binEignerOderAdmin(), True, False)
        If myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale.ToLower Then
            btnWeitereBearbeiterListen.IsEnabled = True
        End If
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
        If Not clsParadigmaRechte.binEignerOderAdmin Then
            MsgBox("Sie sind nicht authorisiert Änderungen vorzunehmen.")
            e.Handled = True
            Me.Close()
            Exit Sub
        End If
        Dim dummy As Boolean
        Dim st As New clsStammTools
        If Not NEU_eingabenOk() Then
            btnSpeichern.IsEnabled = False
            e.Handled = True
            Me.Close()
        End If
        If Not NEUform2objok() Then
            btnSpeichern.IsEnabled = False
            e.Handled = True
            Me.Close()
        End If
        btnSpeichern.IsEnabled = False
        If Not st.speichernAllgemein(dummy, _modus) Then Exit Sub
        setWeitereBearbeiterAuswahlsichtbar()
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

    'Private Sub changeAZ_Click_1(sender As Object , e As RoutedEventArgs)

    '    End Sub
End Class
