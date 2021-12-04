Public Class winAZdefineNEU
    Private Property modus$
    Public Property abbruch As Boolean = False
    Private Property _meinStamm As New Stamm
    Public Property auswahlBearbeiterid As Integer = 0

    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub


    Sub New(ByVal _modus$, ByRef meinAZ As Stamm)
        InitializeComponent()
        modus = _modus
        _meinStamm = CType(meinAZ.Clone, Stamm)

    End Sub

    Private Sub InitialComboFuerAdminSichtbarmachen()
        btnBearbeiterauswahl.Visibility = If(myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer, Visibility.Visible, Visibility.Collapsed)
    End Sub

    Private Sub winAzdefine_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        '  comboBearbeiterInit()

        ' alteStartRoutine()
        If modus.ToLower = "neu" Then
            tbBearbeiterKuerzel.Text = myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig
            tbBearbeiterInitial.Text = myGlobalz.sitzung.aktBearbeiter.Initiale
            ' tbBearbeiterKuerzel.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kuerzel2Stellig
            _meinStamm.az.Prosa = ""
            tbJahr.Text = Format(Now, "yy")
            tbVorgangsID.Text = "XXXXX"
            btnSpeichern.IsEnabled = False
            auswahlBearbeiterid = myGlobalz.sitzung.aktBearbeiter.ID
            btnGetSGnr_ClickExtracted()
            If abbruch Then
                e.Handled = True
                Close()
                Exit Sub
            End If
            '  Title = "Neues Aktenzeichen festlegen"
            '  stpTitel.Visibility = Visibility.Visible
        End If
        If modus.ToLower = "edit" Then
            '  Title = "Aktenzeichen für Vorgang: " & myGlobalz.sitzung.VorgangsID & " festlegen."
            ' clsVorgangCTRL.leseVorgangvonDBaufObjekt(myGlobalz.sitzung.VorgangsID, _meinStamm, myGlobalz.sitzung.VorgangREC)
            tbBearbeiterKuerzel.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kuerzel2Stellig
            tbBearbeiterInitial.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale
            getstamm67Text()
            If auswahlBearbeiterid = 0 Then
                auswahlBearbeiterid = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID
            End If
            tbSachgebietZahl.Text = _meinStamm.az.sachgebiet.Zahl
            tbSachgebietHeader.Text = _meinStamm.az.sachgebiet.Header
            ' tbTitel.Text = _meinStamm.Beschreibung
            tbProsa.Text = _meinStamm.az.Prosa
            '   tbBearbeiter.Text = _meinStamm.Bearbeiter.Initiale
            '  tbBearbeiter.Text = myGlobalz.sitzung.Bearbeiter.Kuerzel2Stellig
            tbAzgesamt.Text = _meinStamm.az.gesamt
            getJahr()
            tbVorgangsID.Text = myGlobalz.sitzung.aktVorgangsID.ToString
            btnSpeichern.IsEnabled = False
            '   stpTitel.Visibility = Visibility.Collapsed
        End If
        Title = StammToolsNs.setWindowTitel.exe(modus, "Aktenzeichen")
        calcAzGesamt()
        InitialComboFuerAdminSichtbarmachen()
        gastLayout()
        hinweisIllegale
        e.Handled = True
    End Sub

    Private Sub hinweisIllegale()
        If tbSachgebietZahl.Text.Trim = "3307" Then
            MsgBox("Falls Sie für diesen Vorgang einen Caterpillar definiert haben, müssen Sie diesen dort erst löschen, sonst entstehen Waisenkinder in der Caterpillar-Datenbank!",
                   MsgBoxStyle.OkOnly, "Wichtiger Hinweis")
        End If
    End Sub

    Private Sub getJahr()
        tbJahr.Text = Format(_meinStamm.Aufnahmedatum, "yyyy")
        tbJahr.Text = tbJahr.Text.Substring(2, 2)
    End Sub
    Private Sub getstamm67Text()
        If String.IsNullOrEmpty(_meinStamm.az.stamm) Then
            _meinStamm.az.stamm = "II-67"
        End If
        tbstamm.Text = _meinStamm.az.stamm
    End Sub

    Private Shared Function kickWhiteSpaces(ByVal meintext As String) As String
        meintext = meintext.Replace(vbCrLf, "")
        meintext = meintext.Replace(vbCr, "")
        meintext = meintext.Replace(vbLf, "")
        meintext = meintext.Replace(vbTab, "")
        Return meintext
    End Function

    Public Sub calcAzGesamt()
        Dim d$ = "-"
        Dim prosa$ = ""
        kickWhiteSpaces(tbProsa.Text)
        If String.IsNullOrEmpty(tbProsa.Text.Trim) Then
            prosa = ""
        Else
            prosa = d$ & tbProsa.Text.Trim
        End If
        tbAzgesamt.Text = tbstamm.Text & d$ & tbSachgebietZahl.Text & d$ & tbVorgangsID.Text & d$ & tbJahr.Text & d$ & tbBearbeiterKuerzel.Text & prosa.Trim '& d$ & s
        tbAzgesamt.Text = tbAzgesamt.Text.Trim
    End Sub

    Private Sub tbBeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbProsa.TextChanged
        calcAzGesamt()
        btnSpeichern.IsEnabled = True
    End Sub


    Private Function istEingabeOK(ByVal jahr$, ByVal sgnr As String) As Boolean
        If String.IsNullOrEmpty(jahr) Then
            MessageBox.Show("Die Angabe zum Jahr des Anlegens des Vorgangs ist leer. Bitte geben Sie eine Zweistellige Jahresangabe ein!")
            Return False
        End If
        If (tbJahr.Text.Length <> 2) Then
            MessageBox.Show("Die Angabe zum Jahr des Anlegens des Vorgangs ist falsch. " & vbCrLf &
                            "Bitte geben Sie eine Zweistellige Jahresangabe ein!" & vbCrLf &
                            "z.b. 10 für 2010")
            Return False
        End If
        If String.IsNullOrEmpty(sgnr$) Then
            MessageBox.Show("Sie müssen ein Sachgebiet auswählen!")
            Return False
        End If
        Return True
    End Function

    Private Sub schliessenFormular()
        Me.Close() ' Me.Hide()
        tbSachgebietZahl.Text = ""
        btnSpeichern.IsEnabled = False
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        schliessenFormular()
    End Sub

    Sub speichern()
        If Not istEingabeOK(tbJahr.Text, tbSachgebietZahl.Text) Then Exit Sub
        kickWhiteSpaces(tbProsa.Text)
        '  kickWhiteSpaces(tbTitel.Text)

        _meinStamm.az.stamm = tbstamm.Text
        _meinStamm.az.sachgebiet.Zahl = tbSachgebietZahl.Text
        _meinStamm.az.sachgebiet.Header = tbSachgebietHeader.Text
        _meinStamm.az.Prosa = tbProsa.Text
        _meinStamm.az.gesamt = tbAzgesamt.Text
        _meinStamm.hauptBearbeiter.Initiale = tbBearbeiterInitial.Text
        _meinStamm.hauptBearbeiter.Kuerzel2Stellig = tbBearbeiterKuerzel.Text
        _meinStamm.hauptBearbeiter.ID = auswahlBearbeiterid


        'Die daten werden an das globale objekt übergeben
        CLstart.myc.AZauswahl.az = CType(_meinStamm.az.Clone, clsAktenzeichen)
        CLstart.myc.AZauswahl.hauptBearbeiter.Initiale = _meinStamm.hauptBearbeiter.Initiale
        CLstart.myc.AZauswahl.hauptBearbeiter.ID = _meinStamm.hauptBearbeiter.ID
        CLstart.myc.AZauswahl.hauptBearbeiter.Kuerzel2Stellig = _meinStamm.hauptBearbeiter.Kuerzel2Stellig
        CLstart.myc.AZauswahl.Beschreibung = _meinStamm.Beschreibung
        Me.Close() ' Me.Hide()        
    End Sub

    Private Sub btnGetSGnr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnGetSGnr_ClickExtracted()
        e.Handled = True
    End Sub


    Private Sub btnGetSGnr_ClickExtracted()

        Dim nnn As New win_sgtree(myGlobalz.Paradigma_Sachgebietsdatei, "vierstellig")
        nnn.ShowDialog()
        If nnn.abbruch Then
            abbruch = True
        Else
            CLstart.myc.AZauswahl.az.sachgebiet.Zahl = nnn.publicNR
            CLstart.myc.AZauswahl.az.sachgebiet.Header = nnn.publicsgHeader


            tbSachgebietZahl.Text = CLstart.myc.AZauswahl.az.sachgebiet.Zahl
            tbSachgebietHeader.Text = CLstart.myc.AZauswahl.az.sachgebiet.Header
            _meinStamm.az.sachgebiet.Zahl = CLstart.myc.AZauswahl.az.sachgebiet.Zahl
            _meinStamm.az.sachgebiet.Header = CLstart.myc.AZauswahl.az.sachgebiet.Header
            btnSpeichern.IsEnabled = True
            calcAzGesamt()
        End If
    End Sub


    Private Sub btnBearbeiterauswahl_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        'tbBearbeiter.Text = cmbUserInitial.SelectedValue.ToString
        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl("einzelauswahl")
        bearbeiterauswahlbox.ShowDialog()
        If Not String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlInitiale) Then
            If bearbeiterauswahlbox.auswahlInitiale = "alle" Then
                Exit Sub
            End If
            'myGlobalz.sitzung.aktBearbeiter.username = bearbeiterauswahlbox.auswahlUSERNAME.ToString
            'myGlobalz.sitzung.aktBearbeiter.Name = bearbeiterauswahlbox.auswahlNAchname.ToString
            'myGlobalz.sitzung.aktBearbeiter.Rang = bearbeiterauswahlbox.auswahlRang.ToString
            'myGlobalz.sitzung.aktBearbeiter.Vorname = bearbeiterauswahlbox.auswahlVorname.ToString
            'myGlobalz.sitzung.aktBearbeiter.Initiale = bearbeiterauswahlbox.auswahlInitiale.ToString
            tbBearbeiterInitial.Text = bearbeiterauswahlbox.auswahlInitiale.ToString
            tbBearbeiterKuerzel.Text = bearbeiterauswahlbox.auswahlKUERZEL1.ToString
            auswahlBearbeiterid = bearbeiterauswahlbox.auswahlBearbeiterid
            btnSpeichern.IsEnabled = True
            calcAzGesamt()
        End If
    End Sub
End Class
