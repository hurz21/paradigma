Public Class winAZdefineNEU
    Private Property modus$
    Private Property _meinStamm As New Stamm

    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub


    Sub New(ByVal _modus$, ByRef meinAZ As Stamm)
        InitializeComponent()
        modus = _modus
        _meinStamm = CType(meinAZ.Clone, Stamm)

    End Sub

    Private Sub InitialComboFuerAdminSichtbarmachen()
        btnBearbeiterauswahl.Visibility = If(clsParadigmaRechte.istUser_admin_oder_vorzimmer(), Visibility.Visible, Visibility.Collapsed)
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
            btnGetSGnr_ClickExtracted()
            '  Title = "Neues Aktenzeichen festlegen"
            '  stpTitel.Visibility = Visibility.Visible
        End If
        If modus.ToLower = "edit" Then
            '  Title = "Aktenzeichen für Vorgang: " & myGlobalz.sitzung.VorgangsID & " festlegen."
            ' clsVorgangCTRL.leseVorgangvonDBaufObjekt(myGlobalz.sitzung.VorgangsID, _meinStamm, myGlobalz.sitzung.VorgangREC)
            tbBearbeiterKuerzel.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kuerzel2Stellig
            tbBearbeiterInitial.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale
            getstamm67Text()
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
        e.Handled = True
    End Sub

    'Private Sub alteStartRoutine()
    '    glob3.allAktobjReset.execute(myGlobalz.sitzung)
    '    comboBearbeiterInit()
    '    tbBearbeiter.Text = myGlobalz.sitzung.Bearbeiter.Kuerzel2Stellig
    '    If modus.ToLower = "neu" Then
    '        _meinStamm.az.Prosa = ""
    '        tbJahr.Text = Format(Now, "yy")
    '        tbVorgangsID.Text = "XXXXX"
    '        Title = "Neues Aktenzeichen festlegen"
    '        stpTitel.Visibility = Visibility.Visible
    '    End If
    '    If modus.ToLower = "edit" Then
    '        Title = "Aktenzeichen für Vorgang: " & myGlobalz.sitzung.VorgangsID & " festlegen."
    '        clsVorgangCTRL.leseVorgangvonDBaufObjekt(myGlobalz.sitzung.VorgangsID, _meinStamm, myGlobalz.sitzung.VorgangREC)

    '        getstammText()
    '        tbSachgebietZahl.Text = _meinStamm.az.sachgebiet.Zahl
    '        tbSachgebietHeader.Text = _meinStamm.az.sachgebiet.Header
    '        tbTitel.Text = _meinStamm.Beschreibung
    '        tbProsa.Text = _meinStamm.az.Prosa
    '        tbBearbeiter.Text = _meinStamm.Bearbeiter.Initiale
    '        tbAzgesamt.Text = _meinStamm.az.gesamt
    '        getJahr()
    '        tbVorgangsID.Text = myGlobalz.sitzung.VorgangsID.ToString
    '        stpTitel.Visibility = Visibility.Collapsed
    '    End If
    '    calcAzGesamt()
    '    InitialComboFuerAdminSichtbarmachen()
    '    btnSpeichern.IsEnabled = False
    'End Sub
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
    'Function comboBearbeiterInit() As Boolean
    '    Try
    '        Dim sql$ = "select LOWER(Kuerzel1) as INITIALE,NACHNAME from " & "Bearbeiter" & " order by NACHname asc"
    '        '    Dim sql$ = "select LOWER(Kuerzel1) as KUERZEL1,NAME from " & "Bearbeiter" & " order by name asc"
    '        myGlobalz.sitzung.BearbeiterREC.dt = userTools.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql$).Copy
    '        cmbUserInitial.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
    '        'For Each ddd As DataRow In myGlobalz.sitzung.BearbeiterREC.dt.AsEnumerable
    '        '    Console.WriteLine(ddd.Item("Initiale").ToString & " " & ddd.Item("Name").ToString)
    '        'Next
    '        If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
    '            '   cmbUserInitial.SelectedValue = Nothing
    '            cmbUserInitial.SelectedValue = myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower
    '        Else
    '            cmbUserInitial.SelectedValue = myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower
    '        End If

    '    Catch ex As Exception
    '        nachricht_und_Mbox("" & ex.ToString)
    '    End Try
    'End Function

    'Private Sub cmbUserInitial_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbUserInitial.SelectionChanged
    '    If cmbUserInitial.SelectedValue Is Nothing Then Exit Sub
    '    nachricht("Ausgewählte cmbUserInitial " & cmbUserInitial.SelectedValue.ToString)
    '    ' aktSachgebietnr = cmbSachgebietnr.SelectedValue.ToString
    '    tbBearbeiter.Text = cmbUserInitial.SelectedValue.ToString
    '    btnSpeichern.IsEnabled = True
    '    calcAzGesamt()
    '    e.Handled = True
    'End Sub

    'Private Sub tbSachgebiet_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbSachgebiet.TextChanged
    '    btnSpeichern.IsEnabled = True
    '    calcAzGesamt()
    'End Sub

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

    'Private Sub tbBearbeiter_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBearbeiter.con
    '    calcAzGesamt()
    'End Sub

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


        'Die daten werden an das globale objekt übergeben
        CLstart.myc.AZauswahl.az = CType(_meinStamm.az.Clone, clsAktenzeichen)
        CLstart.myc.AZauswahl.hauptBearbeiter.Initiale = _meinStamm.hauptBearbeiter.Initiale
        CLstart.myc.AZauswahl.Beschreibung = _meinStamm.Beschreibung
        Me.Close() ' Me.Hide()        
    End Sub

    Private Sub btnGetSGnr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnGetSGnr_ClickExtracted()
        e.Handled = True
    End Sub


    Private Sub btnGetSGnr_ClickExtracted()
        Dim nnn As New win_sgtree(myGlobalz.Paradigma_Sachgebietsdatei)
        nnn.ShowDialog()

        CLstart.myc.AZauswahl.az.sachgebiet.Zahl = nnn.publicNR
        CLstart.myc.AZauswahl.az.sachgebiet.Header = nnn.publicsgHeader


        tbSachgebietZahl.Text = CLstart.myc.AZauswahl.az.sachgebiet.Zahl
        tbSachgebietHeader.Text = CLstart.myc.AZauswahl.az.sachgebiet.Header
        _meinStamm.az.sachgebiet.Zahl = CLstart.myc.AZauswahl.az.sachgebiet.Zahl
        _meinStamm.az.sachgebiet.Header = CLstart.myc.AZauswahl.az.sachgebiet.Header
        btnSpeichern.IsEnabled = True
        calcAzGesamt()
    End Sub


    Private Sub btnBearbeiterauswahl_Click_1(sender As Object, e As RoutedEventArgs)
        'tbBearbeiter.Text = cmbUserInitial.SelectedValue.ToString
        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl
        bearbeiterauswahlbox.ShowDialog()
        If Not String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlInitiale) Then
            'myGlobalz.sitzung.aktBearbeiter.username = bearbeiterauswahlbox.auswahlUSERNAME.ToString
            'myGlobalz.sitzung.aktBearbeiter.Name = bearbeiterauswahlbox.auswahlNAchname.ToString
            'myGlobalz.sitzung.aktBearbeiter.Rang = bearbeiterauswahlbox.auswahlRang.ToString
            'myGlobalz.sitzung.aktBearbeiter.Vorname = bearbeiterauswahlbox.auswahlVorname.ToString
            'myGlobalz.sitzung.aktBearbeiter.Initiale = bearbeiterauswahlbox.auswahlInitiale.ToString
            tbBearbeiterInitial.Text = bearbeiterauswahlbox.auswahlInitiale.ToString
            tbBearbeiterKuerzel.Text = bearbeiterauswahlbox.auswahlKUERZEL1.ToString
            btnSpeichern.IsEnabled = True
            calcAzGesamt()
        End If
        e.Handled = True
    End Sub
End Class
