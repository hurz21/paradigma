Imports System.Data

Partial Public Class winBeteiligteDetail
    Private Property _modus$
    Sub New(ByVal modus$)
        InitializeComponent()
        _modus$ = modus
    End Sub

    Private Sub winBeteiligteDetail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichernPerson.IsEnabled Then
            red = MessageBox.Show("Sie haben Daten in dieser Maske geändert! Abspeichern ?", "Personen", _
            MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.OK)
            If Not red = MessageBoxResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub winBeteiligteDetail_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        starteForm()
        btnSpeichernPerson.IsEnabled = False
        clsParadigmaRechte.buttons_schalten(btnSpeichernPerson, btnLoeschenPerson)
    End Sub

    Private Sub starteForm()
        myGlobalz.sitzung.aktADR.clear()
        setComboboxBeteiligte()
        setcmbNamenszusatz()
        setcmbAnrede()
        inicmbFunktion()
        initGemeindeCombo()
        If myGlobalz.sitzung.BeteiligteModus = "edit" Then
            btnSpeichernPerson.IsEnabled = False
            btnLoeschenPerson.IsEnabled = True
            If myGlobalz.sitzung.aktPerson.Status = 1 Then
                Me.IsEnabled = False
            End If
        End If
        If myGlobalz.sitzung.BeteiligteModus = "neu" Then
            ComboBoxBeteiligte.IsDropDownOpen = True
        End If
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.BeteiligteModus, "Beteiligte")
    End Sub

    Private Sub setcmbAnrede()
        cmbAnrede.Items.Add("Herr")
        cmbAnrede.Items.Add("Frau")
        cmbAnrede.Items.Add("Eheleute")
        cmbAnrede.Items.Add("Firma")
    End Sub

    Private Sub setcmbNamenszusatz()
        cmbNamenszusatz.Items.Add("Dr.")
        cmbNamenszusatz.Items.Add("Prof.")
    End Sub

    Private Sub setComboboxBeteiligte()
        Dim filename As String = myGlobalz.appdataDir & "\config\Combos\Detail_Beteiligte_Rollen.xml"
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxbeteiligteRollen"), XmlDataProvider)
        existing.Source = New Uri(filename)
        ComboBoxBeteiligte.SelectedIndex = 0
    End Sub

    Sub inicmbFunktion()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxbeteiligteFunktion"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\beteiligte_Funktion.xml")
    End Sub





    'Private Shared Function istEingabe_vorhanden() As Boolean
    '    If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Rolle) Then
    '        MessageBox.Show("Sie müssen eine Rolle angeben!", "Rolle fehlt!", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '        Return False
    '    End If
    '    If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Name) Then
    '        MessageBox.Show("Sie müssen einen Namen angeben!", "Name fehlt!", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '        Return False
    '    End If
    '    Return True
    'End Function


    Sub BeteiligtenAbspeichern()
        'If CInt(myGlobalz.sitzung.aktPerson.Status) = 1 Then
        '    MsgBox("Es handelt sich um die Person eines Verwandten Vorgangs. Sie kann nicht geändert werden!")
        '    Exit Sub
        'End If
        'If Not istEingabe_vorhanden() Then Exit Sub
        'If myGlobalz.sitzung.BeteiligteModus = "neu" Then

        '    glob2.NeueBeteiligteAbspeichern(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktPerson)
        '    ' adresseAlsRaumbezugsAdresseAbspeichern()
        '    myGlobalz.sitzung.aktPerson.clear()
        '    myGlobalz.sitzung.aktADR.clear()
        '    btnSpeichernPerson.IsEnabled = False
        '    Me.Close()
        'End If
        'If myGlobalz.sitzung.BeteiligteModus = "edit" Then

        '    Dim erfolg% = DB_Oracle_sharedfunctions.Beteiligte_abspeichern_Edit(myGlobalz.sitzung.aktPerson.PersonenID, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktPerson)
        '    If erfolg% > 0 Then
        '        ' adresseAlsRaumbezugsAdresseAbspeichern()
        '        myGlobalz.sitzung.aktPerson.anychange = False
        '        myGlobalz.sitzung.aktPerson.clear()
        '        myGlobalz.sitzung.aktADR.clear()
        '        btnSpeichernPerson.IsEnabled = False
        '        Me.Close()
        '    Else
        '        nachricht("Problem beim Abspeichernu!")
        '    End If
        'End If
    End Sub

    'Sub adresseAlsRaumbezugsAdresseAbspeichern()
    '    If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename.Trim) Then Exit Sub
    '    Dim messi As MessageBoxResult
    '    messi = MessageBox.Show("Möchten Sie die Adresse zu den Raumbezügen übernehmen? " & vbCrLf &
    '                            "Sie sparen sich ggf. Tipparbeit", "Adresse zu Raumbezug" & vbCrLf &
    '                            " (im Regefall ist dies nicht sinnvoll!) ",
    '                              MessageBoxButton.YesNo,
    '                            MessageBoxImage.Question, MessageBoxResult.No)
    '    If messi = MessageBoxResult.Yes Then
    '        glob2.adresseAlsRaumbezugsAdresseAbspeichernExtracted()
    '    End If
    'End Sub

    Private Sub tbRolle_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbRolle.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(145, tbBemerkung)
        e.Handled = True
    End Sub

    Private Sub tbName_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbName.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(150, tbName)
        e.Handled = True
    End Sub

    Private Sub tbVname_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVname.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(140, tbVname)
        e.Handled = True
    End Sub

    Private Sub tbNamenszusatz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbNamenszusatz.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbNamenszusatz)
        e.Handled = True
    End Sub

    Private Sub tbAnrede_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbAnrede.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbAnrede)
        e.Handled = True
    End Sub

    Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBemerkung.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(250, tbBemerkung)
        e.Handled = True
    End Sub

    Private Sub tbPLZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPLZ.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(7, tbPLZ)
        e.Handled = True
    End Sub

    Private Sub tbgemeinde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbgemeinde.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbgemeinde)
        e.Handled = True
    End Sub

    Private Sub tbstrasse_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbstrasse.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbstrasse)
        e.Handled = True
    End Sub

    Private Sub tbHausnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHausnr.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbHausnr)
        e.Handled = True
    End Sub

    Private Sub tbPostfach_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPostfach.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbPostfach)
        e.Handled = True
    End Sub

    Private Sub tbFunktion_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFunktion.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(150, tbFunktion)
        e.Handled = True
    End Sub

    Private Sub tbOrg_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbOrg.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbOrg)
        e.Handled = True
    End Sub

    Private Sub tbOrgzusatz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbOrgzusatz.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbOrgzusatz)
        e.Handled = True
    End Sub

    Private Sub tbTyp1_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTyp1.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbTyp1)
        e.Handled = True
    End Sub

    Private Sub tbTyp2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTyp2.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbTyp2)
        e.Handled = True
    End Sub

    Private Sub tbEigentuemer_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEigentuemer.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbEigentuemer)
        e.Handled = True
    End Sub

    Private Sub tbEmail_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEmail.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbEmail)
        e.Handled = True
    End Sub

    Private Sub tbTelefon_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTelefon.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbTelefon)
        e.Handled = True
    End Sub

    Private Sub tbTelefon2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTelefon2.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbTelefon2)
        e.Handled = True
    End Sub

    Private Sub tbMobil_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbMobil.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbMobil)
        e.Handled = True
    End Sub

    Private Sub tbFax_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFax.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbFax)
        e.Handled = True
    End Sub

    Private Sub tbHomepage_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHomepage.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbHomepage)
        'If tbHomepage.Text.trim.Length>0 Then
        '    btnShowHomepage.IsEnabled=True
        '    Else
        '    btnShowHomepage.IsEnabled=false
        'End If
        e.Handled = True
    End Sub

    Private Sub tbKassenkonto_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbKassenkonto.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(545, tbKassenkonto)
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnLoeschenPerson_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschenPerson.Click
        'personAusListeLoeschen()
        e.Handled = True
    End Sub

    Private Sub cmbFunktion_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbFunktion.SelectionChanged
        Dim item2 As String = CType(cmbFunktion.SelectedValue, String)
        If item2 Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktPerson.Kontakt.GesellFunktion = item2
        e.Handled = True
    End Sub

    Private Sub ComboBoxBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBoxBeteiligte.SelectionChanged
        If ComboBoxBeteiligte.SelectedValue Is Nothing Then Exit Sub
        If ComboBoxBeteiligte.SelectedValue.ToString.StartsWith("Hinzuf") Then Exit Sub
        Dim item2 As String = CType(ComboBoxBeteiligte.SelectedValue, String)
        Dim speichertest$ = item2
        myGlobalz.sitzung.aktPerson.Rolle = item2
        If _modus = "neu" Then
            If pruefenObRollenliste(item2) Then
                RollenListeAnbieten(item2)
                auswahlUbernehmen()
            End If
        End If
        myGlobalz.sitzung.aktPerson.Rolle = speichertest$
        ComboBoxBeteiligte.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub cmbNamenszusatz_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbNamenszusatz.SelectionChanged
        If cmbNamenszusatz.SelectedValue Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktPerson.Namenszusatz &= cmbNamenszusatz.SelectedValue.ToString & " "
        e.Handled = True
    End Sub

    Private Sub cmbAnrede_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbAnrede.SelectionChanged
        If cmbAnrede.SelectedValue Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktPerson.Anrede &= cmbAnrede.SelectedValue.ToString & " "
        e.Handled = True
    End Sub

    Private Sub gemeindechanged()
        'Dim selob As New KeyValuePair(Of String, String)
        'selob = CType(cmbGemeinde.SelectedItem, KeyValuePair(Of String, String))
        'Dim selKey$ = selob.Key
        'Dim selvalue$ = selob.Value
        Dim myvali$ = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali) - 438000
        tbgemeinde.Text = myvals$

        'Dim item2 As DataRowView = CType(cmbGemeinde.SelectedItem, DataRowView)
        'Dim item3$ = item2.Row.ItemArray(0).ToString
        'Me.tbgemeinde.Text = item2.Row.ItemArray(1).ToString
        'myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = tbgemeinde.Text
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = tbgemeinde.Text
        'Dim buchstabe As String = ""
        'initStrassenCombo(buchstabe)
        stckBuchstaben1.IsEnabled = True : stckBuchstaben2.IsEnabled = True
        myGlobalz.sitzung.aktADR.PLZ = (glob2.getPLZfromGemeinde(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName))
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PLZ = myGlobalz.sitzung.aktADR.PLZ
        ' e.Handled = True
    End Sub

    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbGemeinde.SelectionChanged
        If cmbGemeinde.SelectedValue Is Nothing Then Exit Sub
        gemeindechanged()
        '  cmbStrasse.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Sub initStrassenCombo(buchstabe As String)
        Dim strassseninstanz As New List(Of AdressTools.strassenUiObj)
        Dim att As New AdressTools
        att.buildStrassenListe(buchstabe, strassseninstanz)
        cmbStrasse.DataContext = strassseninstanz 'myGlobalz.sitzung.haloREC.dt
        'DB_Oracle.holeStrasseDTausHalo(buchstabe)
        'cmbStrasse.DataContext = myGlobalz.sitzung.haloREC.dt
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemeinden.xml")
        'Dim gemeindeDict As New Dictionary(Of String, String)
        'glob2.holeGemeindeDT(gemeindeDict)
        'cmbGemeinde.ItemsSource = gemeindeDict
        'glob2.holeGemeindeDT()
        'cmbGemeinde.DataContext = myGlobalz.sitzung.AlbRec.dt
    End Sub

    Private Sub cmbStrasse_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStrasse.SelectionChanged
        strassegewaehlt()
        e.Handled = True
    End Sub

    Sub initHausNRCombo()
        DB_Oracle_sharedfunctions.DBholeHausnrDT()
        cmbHausnr.DataContext = myGlobalz.sitzung.postgresREC.dt
    End Sub

    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbHausnr.SelectionChanged
        hausnrgewaehlt()
        e.Handled = True
    End Sub

    Private Sub hausnrgewaehlt()
        Dim item2 As DataRowView = CType(cmbHausnr.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbHausnr.Text = item2.Row.ItemArray(1).ToString
        Dim halo_id% = CInt(item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Hausnr = tbHausnr.Text
        glob2.hole_AdressKoordinaten(halo_id%)
        'If myGlobalz.sitzung.aktADR.punkt.X < 1 Or myGlobalz.sitzung.aktADR.punkt.Y < 1 Then
        '    MsgBox("Ein Fall für Google")
        'End If
    End Sub

    Private Sub strassegewaehlt()
        Try
            Dim auswahlStrasse As AdressTools.strassenUiObj = CType(cmbStrasse.SelectedItem, AdressTools.strassenUiObj)
            If auswahlStrasse Is Nothing Then Exit Sub
            If auswahlStrasse.quelle = "mehr" Then Exit Sub

            If auswahlStrasse.quelle = "fehlt" Then
                MsgBox("Bitte geben Sie die Strasse und die Hausnummer von Hand ein." & Environment.NewLine & Environment.NewLine &
                       "Wenn die Hausnummer unbekannt oder unsicher ist, lassen Sie sie einfach weg.")
                myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "fehlt"
                tbstrasse.Background = Brushes.PaleVioletRed
                tbHausnr.Background = Brushes.PaleVioletRed
                Exit Sub
            End If

            If auswahlStrasse.quelle = "lage" Then
                tbstrasse.Text = auswahlStrasse.sname ' item2.Row.ItemArray(1).ToString
                ' myGlobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(auswahlStrasse.strcode) 'erzeugt überlauf
                myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "lage"
                myGlobalz.sitzung.aktADR.Gisadresse.strasseName = tbstrasse.Text
                myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Strasse = tbstrasse.Text
                MsgBox("Bitte geben Sie die Hausnummer von Hand ein." & Environment.NewLine & Environment.NewLine &
                "Wenn die Hausnummer unbekannt oder unsicher ist, lassen Sie sie einfach weg.")
                tbHausnr.Background = Brushes.PaleVioletRed
            End If

            If auswahlStrasse.quelle = "halo" Then
                myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "halo"
                tbstrasse.Text = auswahlStrasse.sname ' item2.Row.ItemArray(1).ToString
                myGlobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(auswahlStrasse.strcode)
                myGlobalz.sitzung.aktADR.Gisadresse.strasseName = tbstrasse.Text
                myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Strasse = tbstrasse.Text
                initHausNRCombo()
                cmbHausnr.IsDropDownOpen = True
            End If

        Catch ex As Exception
            nachricht("fehler in strassegewaehlt: " & ex.ToString)
        End Try


    End Sub
 

  

 
    'Private Shared Function abfragetextbilden(ByVal neuorg As w_organisation) As String
    '    nachricht("in abfragetextbilden")
    '    Dim sb As New Text.StringBuilder
    '    sb.Append("Möchten Sie diese Organisation in die Kontaktdaten übernehmen ? " & vbCrLf)
    '    sb.Append("Name: " & CStr(neuorg.Name) & vbCrLf)
    '    sb.Append("Zusatz: " & CStr(neuorg.Zusatz) & vbCrLf)
    '    sb.Append("Typ1: " & CStr(neuorg.Typ1) & vbCrLf)
    '    sb.Append("Typ2: " & CStr(neuorg.Typ2) & vbCrLf)
    '    sb.Append("Eigentümer: " & CStr(neuorg.Eigentuemer) & vbCrLf)
    '    Return sb.ToString
    'End Function


    'Private Shared Sub ORGmapping2tempkontakt(ByVal neuorg As w_organisation, ByVal neuorgID%)
    '    nachricht("ORGmapping2tempkontakt    neuorgID% " & neuorgID%)
    '    myGlobalz.sitzung.aktPerson.Kontakt.OrgID = CInt(neuorgID%)
    '    myGlobalz.sitzung.aktPerson.Kontakt.Org.Name = neuorg.Name
    '    myGlobalz.sitzung.aktPerson.Kontakt.Org.Zusatz = neuorg.Zusatz
    '    myGlobalz.sitzung.aktPerson.Kontakt.Org.Typ1 = neuorg.Typ1
    '    myGlobalz.sitzung.aktPerson.Kontakt.Org.Typ2 = neuorg.Typ2
    '    myGlobalz.sitzung.aktPerson.Kontakt.Org.Eigentuemer = neuorg.Eigentuemer
    '    myGlobalz.sitzung.aktPerson.Kontakt.Org.Anschriftid = neuorg.Anschriftid
    'End Sub

    Function OrgDTaufOBJabbilden(ByVal neuorgID As Integer) As w_organisation
        Dim neuOrg As New w_organisation
        Dim vgl%
        For Each row As DataRow In myGlobalz.sitzung.kontaktdatenREC.dt.AsEnumerable
            vgl = CInt(row.Item("OrgID"))
            If CInt(row.Item("OrgID")) = neuorgID% Then
                neuOrg.Name = CStr(row.Item("name"))
                neuOrg.Zusatz = CStr(row.Item("Zusatz"))
                neuOrg.Typ1 = CStr(row.Item("Typ1"))
                neuOrg.Typ2 = CStr(row.Item("Typ2"))
                neuOrg.Eigentuemer = CStr(row.Item("Eigentuemer"))
                neuOrg.Anschriftid = CInt(row.Item("Anschriftid"))
                Exit For
            End If
        Next
        Return neuOrg
    End Function

    'Function sollORGuebernommenWerden(ByVal neuorgID as integer) as  Boolean
    '    Try
    '        Dim neuorg = OrgDTaufOBJabbilden(neuorgID%)
    '        If Not neuorg Is Nothing Then
    '            nachricht("gewählte Org ist  vorhanden")
    '            Dim messi As New MessageBoxResult
    '            messi = MessageBox.Show(abfragetextbilden(neuorg), "Daten aus Bestand übernehmen ?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
    '            If messi = MessageBoxResult.Yes Then
    '                nachricht("gewählte org  übernehmen")
    '                ORGmapping2tempkontakt(neuorg, neuorgID%)
    '                If myGlobalz.sitzung.aktPerson.Kontakt.Org.Anschriftid > 0 Then
    '                    Dim frage$ = "Zu der Organisation ist auch eine Adresse vorhanden. Soll die Adresse ebenfalls übernommen werden ?"
    '                    messi = MessageBox.Show(frage$, "Daten aus Bestand übernehmen ?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
    '                    If messi = MessageBoxResult.Yes Then
    '                        nachricht("adresse übernehmen true")
    '                        If DB_Oracle.initAnschriftDT(myGlobalz.sitzung.aktPerson.Kontakt.Org.Anschriftid) Then 'kontaktdatenREC
    '                            KontaktdatenDTKontaktMapping(myGlobalz.sitzung.aktPerson)
    '                        Else
    '                            nachricht("adresse ließ sich nicht initialisieren ")
    '                        End If
    '                    Else
    '                        nachricht("adresse übernehmen false")
    '                    End If
    '                    nachricht("es war schon ein adresse vorhanden ")
    '                Else
    '                    nachricht("es war keine ein adresse vorhanden")
    '                End If
    '            Else
    '                nachricht("gewählte org nicht übernehmen")
    '            End If
    '            nachricht("gewählte Org ist nicht vorhanden")
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        My.Log.WriteEntry(ex.ToString)
    '        Return False
    '    End Try
    'End Function

    'Private Shared Sub KontaktdatenDTKontaktMapping(ByRef aktperson As Person)
    '    nachricht("adresse ließ sich  initialisieren ")
    '    With aktperson.Kontakt.Anschrift 'myGlobalz.sitzung.aktPerson.Kontakt.Anschrift
    '        myGlobalz.sitzung.aktPerson.Kontakt.AnschriftID = myGlobalz.sitzung.aktPerson.Kontakt.Org.Anschriftid
    '        .Gemeindename = clsDBtools.fieldvalue(myGlobalz.sitzung.kontaktdatenREC.dt.Rows(0).Item("gemeindename"))
    '        .Strasse = clsDBtools.fieldvalue(myGlobalz.sitzung.kontaktdatenREC.dt.Rows(0).Item("Strasse"))
    '        .Hausnr = clsDBtools.fieldvalue(myGlobalz.sitzung.kontaktdatenREC.dt.Rows(0).Item("Hausnr"))
    '        .PLZ = (clsDBtools.fieldvalue(myGlobalz.sitzung.kontaktdatenREC.dt.Rows(0).Item("PLZ"))) 'CInt wurde wg. ddr und urbanke entfernt
    '        .Postfach = clsDBtools.fieldvalue(myGlobalz.sitzung.kontaktdatenREC.dt.Rows(0).Item("Postfach"))
    '        .Bemerkung = clsDBtools.fieldvalue(myGlobalz.sitzung.kontaktdatenREC.dt.Rows(0).Item("Bemerkung"))
    '        .Quelle = clsDBtools.fieldvalue(myGlobalz.sitzung.kontaktdatenREC.dt.Rows(0).Item("Quelle"))
    '    End With
    'End Sub


    'Sub personAusListeLoeschen()
    '    Dim messi As New MessageBoxResult
    '    messi = MessageBox.Show(String.Format("Beteiligten wirklich löschen ?{0}{1}", vbCrLf, myGlobalz.sitzung.aktPerson.tostring),
    '                            "Beteiligten löschen ?",
    '                            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
    '    If messi = MessageBoxResult.Yes Then
    '        clsBeteiligteBUSI.personAusVorgangEntfernen(myGlobalz.sitzung.aktPerson.PersonenID, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktPerson.Status)
    '        btnSpeichernPerson.IsEnabled = False
    '    End If
    '    Me.Close()
    'End Sub

    'Private Function personAusVorgangEntfernen() As Boolean
    '    If CInt(myGlobalz.sitzung.aktPerson.Status) = 0 Then
    '        Dim erfolg% = clsBeteiligteBUSI.Beteiliten_loeschen_AlleDB(myGlobalz.sitzung.aktPerson.PersonenID)
    '        If erfolg < 1 Then
    '            nachricht_und_Mbox(String.Format("Problem beim Entkoppeln von Vorgang und Person: {0} {1}", myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktPerson.PersonenID))
    '            Return False
    '        Else
    '            Return True
    '        End If
    '    Else
    '        'Entkoppeln durch löschen aus der kopplungstabelle
    '        'ist gedacht für die verwandten vorgänge(status <>1)
    '        If (myGlobalz.beteiligte_MYDB.dbtyp = "mysql") Then
    '            nachricht(clsBeteiligteDBCRUD_MYSQL.Entkoppelung_Beteiligte_Vorgang(myGlobalz.sitzung.aktPerson.PersonenID, myGlobalz.sitzung.aktVorgangsID).ToString)
    '        End If
    '        If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
    '            nachricht(clsBeteiligteDBCRUD_ORACLE.Entkoppelung_Beteiligte_Vorgang(myGlobalz.sitzung.aktPerson.PersonenID, myGlobalz.sitzung.aktVorgangsID).ToString)
    '        End If
    '    End If
    'End Function



    Sub vorwaerts()
        Me.DialogResult = True
    End Sub
    Sub rueckwaerts()
        Me.DialogResult = False
    End Sub

    'Private Sub Blaetternabbruch(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    'Me.DialogResult = Nothing
    '    myGlobalz.sitzung.kontaktdatenDT = Nothing
    '    nachricht("Blaetternabbruch: Daten Keiner Person wurden übernommen")
    '    Me.Close()
    'End Sub

    'Private Sub Blaetternspeichern(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    BeteiligtenAbspeichern()
    '    myGlobalz.sitzung.kontaktdatenDT = Nothing
    '    nachricht("Blaetternspeichern: Daten einer Person wurden übernommen")
    '    Me.Close()
    'End Sub



    Private Sub tbPostfachPLZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPostfachPLZ.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbPostfachPLZ)
        e.Handled = True
    End Sub

    Private Sub btnKontaktNachOutlookUebernehmen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("btnKontaktNachOutlookUebernehmen_Click -------------------------------------")
        MsgBox("Baustelle")
        'Dim neuu As New LIBOutlook2.MeinOutlook
        'Dim erfolg As Boolean = neuu.OutlookKontakt("D " & myGlobalz.sitzung.Bearbeiter.Name,
        '                                           ereig.Beschreibung,
        '                                           ereig.Datum,
        '                                           60,
        '                                           True, False, False, False,
        '                                           False)
        'If erfolg Then
        '    MsgBox("Der Kontakt wurde in Outlook übernommen.")
        'Else
        '    MsgBox("Der Kontakt wurde NICHT in Outlook übernommen.")
        'End If
        'Return erfolg
    End Sub
    'Sub printdialogs()
    '    Dim pd As New PrintDialog
    '    If pd.ShowDialog Then
    '        Dim oldtransf = Me.RenderTransform
    '        Dim transgroup As New TransformGroup
    '        transgroup.Children.Add(New ScaleTransform(2, 2))
    '        transgroup.Children.Add(New TranslateTransform(96 / 2.54 * 0.5, 96 / 2.54 * 0.5))
    '        Me.RenderTransform = transgroup
    '        Me.UpdateLayout()
    '        pd.PrintVisual(Me, "Screenshot")
    '        Me.RenderTransform = oldtransf
    '    End If
    'End Sub

    'Private Sub btnprinttest_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    printdialogs()
    'End Sub

    Private Sub btnPersonenuernehmen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim winpers As New Window_Person("")
        winpers.ShowDialog()
    End Sub

    Private Function pruefenObRollenliste(ByRef rolle As String) As Boolean
        If rolle.ToLower.Contains("schornsteinfeger") Then
            rolle = "feger"
            Return True
        End If
        If rolle.ToLower.Contains("bauauf") Then
            rolle = "bauauf"
            Return True
        End If
        If rolle.ToLower.Contains("gemeinde") Then
            rolle = "gemeinde"
            Return True
        End If
        If rolle.ToLower.Contains("naturschutzb") Then
            rolle = "naturschutzbeirat"
            Return True
        End If
        If rolle.ToLower.Contains("fd umwelt") Then
            rolle = "fd umwelt"
            Return True
        End If
        'If rolle.ToLower.Contains("veter") Then
        '    rolle = "veter"
        '    Return True
        'End If
        'If rolle.ToLower.Contains("rp") Then
        '    rolle = "rp"
        '    Return True
        'End If
        'If rolle.ToLower.Contains("alr") Then
        '    rolle = "alr"
        '    Return True
        'End If
        'If rolle.ToLower.Contains("ortsland") Then
        '    rolle = "ortsland"
        '    Return True
        'End If
        'If rolle.ToLower.Contains("ordnungs") Then
        '    rolle = "ordnungs"
        '    Return True
        'End If
        'If rolle.ToLower.Contains("gutachter") Then
        '    rolle = "gutachter"
        '    Return True
        'End If
        'If rolle.ToLower.Contains("forst") Then
        '    rolle = "forst"
        '    Return True
        'End If
        Return False
    End Function

    Private Sub RollenListeAnbieten(ByVal rolle As String)
        Dim winpers As New Window_Person(rolle)
        winpers.ShowDialog()
        Dim aaa = myGlobalz.sitzung.aktPerson
    End Sub

    Private Sub auswahlUbernehmen()

    End Sub

    Private Sub tbBezirk_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBezirk.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(150, tbBezirk)
        e.Handled = True
    End Sub


    Private Sub BTNanschirftgenerieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        clsBeteiligteBUSI.Anschrift_generieren()
        e.Handled = True
    End Sub

    Private Sub btnstreet(sender As Object, e As RoutedEventArgs)
        Dim a As String = (sender.ToString)
        a = a.Replace("System.Windows.Controls.Button:", "")
        a = a.Trim.ToLower
        'strasseninit mit und ohne hausnummer
        '  MsgBox(a)
        initStrassenCombo(a)
        cmbStrasse.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Private Sub btnSpeichernPerson_Click(sender As Object, e As RoutedEventArgs)
        BeteiligtenAbspeichern()
        btnSpeichernPerson.IsEnabled = False
        e.Handled = True
    End Sub

    Private Sub btnShowHomepage_Click(sender As Object, e As RoutedEventArgs)
        detailsTools.startURL(tbHomepage.Text.Trim)
        e.Handled = True
    End Sub

    'Private Sub btnBank_Click(sender As Object , e As RoutedEventArgs)
    '        Dim bv As New WinBankverb(myglobalz.sitzung.aktPerson.PersonenID)
    '        bv.ShowDialog
    '          e.Handled = True
    '    End Sub
End Class
