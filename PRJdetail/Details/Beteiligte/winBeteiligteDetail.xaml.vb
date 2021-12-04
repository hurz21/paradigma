Imports System.Data

Partial Public Class winBeteiligteDetail
    Private Property _modus As String
    Property formIstGeladen As Boolean = False
    Private Sub gastLayout()
        Background = myglobalz.GetSecondBackground()
        grdMainKontakt.Background = myglobalz.GetSecondBackground()
    End Sub

    Sub New(ByVal modus As String)
        InitializeComponent()
        _modus = modus
    End Sub

    Private Sub winBeteiligteDetail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichernPerson.IsEnabled Then
            red = MessageBox.Show("Sie haben Daten in dieser Maske geändert! Abspeichern ?", "Personen",
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
        gastLayout()
        initPersonenVorlageCombo()
        formIstGeladen = True
        e.Handled = True
    End Sub

    Private Sub starteForm()
        myglobalz.sitzung.aktADR.clear()
        setComboboxBeteiligte()
        setcmbNamenszusatz()
        setcmbAnrede()
        inicmbFunktion()
        initGemeindeCombo()

        If myglobalz.sitzung.BeteiligteModus = "edit" Then
            btnSpeichernPerson.IsEnabled = False
            btnLoeschenPerson.IsEnabled = True
            If myglobalz.sitzung.aktPerson.Status = 1 Then
                Me.IsEnabled = False
            End If
            Dim iban As String
            iban = clsBeteiligteBUSI.hatBankverbindung(myglobalz.sitzung.aktPerson.PersonenID)
            If Not iban.IsNothingOrEmpty Then
                tbIBAN.Background = Brushes.Aquamarine
                tbIBAN.Text = iban
            Else
                tbIBAN.Text = ""
            End If
        End If
        If myglobalz.sitzung.BeteiligteModus = "neu" Then
            ComboBoxBeteiligte.IsDropDownOpen = True
            btnBank.IsEnabled = False
            btnBank.ToolTip = "Sie müssen die Person erst abspeichern bevor Sie eine Bankverbindung zuordnen können!!!"
        End If
        ' Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.BeteiligteModus, "Beteiligte")
        Title = detailsTools.settitle("Beteiligte, " & myglobalz.sitzung.BeteiligteModus)

    End Sub

    Private Sub setcmbAnrede()
        cmbAnrede.Items.Add("Herr")
        cmbAnrede.Items.Add("Frau")
        cmbAnrede.Items.Add("Frau und Herrn")
        cmbAnrede.Items.Add("Eheleute")
        cmbAnrede.Items.Add("Firma")
    End Sub

    Private Sub setcmbNamenszusatz()
        cmbNamenszusatz.Items.Add("Dr.")
        cmbNamenszusatz.Items.Add("Prof.")
    End Sub

    Private Sub setComboboxBeteiligte()
        Dim filename As String = myglobalz.appdataDir & "\config\Combos\Detail_Beteiligte_Rollen.xml"
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxbeteiligteRollen"), XmlDataProvider)
        existing.Source = New Uri(filename)
        ComboBoxBeteiligte.SelectedIndex = 0
    End Sub

    Sub inicmbFunktion()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxbeteiligteFunktion"), XmlDataProvider)
        existing.Source = New Uri(myglobalz.appdataDir & "\config\Combos\beteiligte_Funktion.xml")
    End Sub





    Private Shared Function istEingabe_vorhanden() As Boolean
        If String.IsNullOrEmpty(myglobalz.sitzung.aktPerson.Rolle) Then
            MessageBox.Show("Sie müssen eine Rolle angeben!", "Rolle fehlt!", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Return False
        End If
        If String.IsNullOrEmpty(myglobalz.sitzung.aktPerson.Name) Then
            MessageBox.Show("Sie müssen einen Namen angeben!", "Name fehlt!", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Return False
        End If
        Return True
    End Function


    Private Function GetPersonenVorlage(derIndex As Integer) As Integer
        Return CInt(derIndex)
    End Function
    Sub BeteiligtenAbspeichern()
        If CInt(myglobalz.sitzung.aktPerson.Status) = 1 Then
            MsgBox("Es handelt sich um die Person eines Verwandten Vorgangs. Sie kann nicht geändert werden!")
            Exit Sub
        End If
        myglobalz.sitzung.aktPerson.PersonenVorlage = GetPersonenVorlage(cmbPersonenVorlage.SelectedIndex)
        If Not istEingabe_vorhanden() Then Exit Sub
        If myglobalz.sitzung.BeteiligteModus = "neu" Then

            glob2.NeueBeteiligteAbspeichern(myglobalz.sitzung.aktVorgangsID, myglobalz.sitzung.aktPerson)
            ' adresseAlsRaumbezugsAdresseAbspeichern()
            myglobalz.sitzung.aktPerson.clear()
            myglobalz.sitzung.aktADR.clear()
            btnSpeichernPerson.IsEnabled = False
            Me.Close()
        End If
        If myglobalz.sitzung.BeteiligteModus = "edit" Then

            Dim erfolg% = DB_Oracle_sharedfunctions.Beteiligte_abspeichern_Edit(myglobalz.sitzung.aktPerson.PersonenID, myglobalz.sitzung.aktVorgangsID, myglobalz.sitzung.aktPerson)
            If erfolg% > 0 Then
                ' adresseAlsRaumbezugsAdresseAbspeichern()
                myglobalz.sitzung.aktPerson.anychange = False
                myglobalz.sitzung.aktPerson.clear()
                myglobalz.sitzung.aktADR.clear()
                btnSpeichernPerson.IsEnabled = False
                Me.Close()
            Else
                nachricht("Problem beim Abspeichernu!")
            End If
        End If
    End Sub



    Private Sub tbRolle_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbRolle.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(145, tbBemerkung)
        e.Handled = True
    End Sub

    Private Sub tbName_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbName.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        If tbName.Text.Length > 2 Then
            If tbName.Text.Length Mod 2 = 0 Then
                btnNamenAuswaehlen.Background = Brushes.RosyBrown
            Else
                btnNamenAuswaehlen.Background = Brushes.Black
            End If
        End If
        glob2.istTextzulang(150, tbName)
        e.Handled = True
    End Sub

    Private Sub tbVname_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVname.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(140, tbVname)
        e.Handled = True
    End Sub

    Private Sub tbNamenszusatz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbNamenszusatz.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbNamenszusatz)
        e.Handled = True
    End Sub

    Private Sub tbAnrede_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbAnrede.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbAnrede)
        e.Handled = True
    End Sub

    Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBemerkung.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(250, tbBemerkung)
        e.Handled = True
    End Sub

    Private Sub tbPLZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPLZ.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(7, tbPLZ)
        e.Handled = True
    End Sub

    Private Sub tbgemeinde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbgemeinde.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbgemeinde)
        e.Handled = True
    End Sub

    Private Sub tbstrasse_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbstrasse.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbstrasse)
        e.Handled = True
    End Sub

    Private Sub tbHausnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHausnr.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbHausnr)
        e.Handled = True
    End Sub

    Private Sub tbPostfach_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPostfach.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(45, tbPostfach)
        e.Handled = True
    End Sub

    Private Sub tbFunktion_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFunktion.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(150, tbFunktion)
        e.Handled = True
    End Sub

    Private Sub tbOrg_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbOrg.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbOrg)
        e.Handled = True
    End Sub

    Private Sub tbOrgzusatz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbOrgzusatz.TextChanged
        If Not formIstGeladen Then Exit Sub
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
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbTyp2)
        e.Handled = True
    End Sub

    Private Sub tbEigentuemer_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEigentuemer.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbEigentuemer)
        e.Handled = True
    End Sub

    Private Sub tbEmail_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEmail.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbEmail)
        e.Handled = True
    End Sub

    Private Sub tbTelefon_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTelefon.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbTelefon)
        e.Handled = True
    End Sub

    Private Sub tbTelefon2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTelefon2.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbTelefon2)
        e.Handled = True
    End Sub

    Private Sub tbMobil_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbMobil.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbMobil)
        e.Handled = True
    End Sub

    Private Sub tbFax_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFax.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbFax)
        e.Handled = True
    End Sub

    Private Sub tbHomepage_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHomepage.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(100, tbHomepage)
        e.Handled = True
    End Sub

    Private Sub tbKassenkonto_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbKassenkonto.TextChanged
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(545, tbKassenkonto)
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnLoeschenPerson_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschenPerson.Click
        personAusListeLoeschen()
        e.Handled = True
    End Sub

    Private Sub cmbFunktion_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbFunktion.SelectionChanged
        If Not formIstGeladen Then Exit Sub
        Dim item2 As String = CType(cmbFunktion.SelectedValue, String)
        If item2 Is Nothing Then Exit Sub
        myglobalz.sitzung.aktPerson.Kontakt.GesellFunktion = item2
        e.Handled = True
    End Sub

    Private Sub ComboBoxBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBoxBeteiligte.SelectionChanged
        If Not formIstGeladen Then Exit Sub
        If ComboBoxBeteiligte.SelectedValue Is Nothing Then Exit Sub
        If ComboBoxBeteiligte.SelectedValue.ToString.StartsWith("Hinzuf") Then Exit Sub
        Dim item2 As String = CType(ComboBoxBeteiligte.SelectedValue, String)
        Dim speichertest$ = item2
        myglobalz.sitzung.aktPerson.Rolle = item2
        If _modus = "neu" Then
            If pruefenObRollenliste(item2) Then
                RollenListeAnbieten(item2)
            End If
        End If
        myglobalz.sitzung.aktPerson.Rolle = speichertest$
        ComboBoxBeteiligte.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub cmbNamenszusatz_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbNamenszusatz.SelectionChanged
        If Not formIstGeladen Then Exit Sub
        If cmbNamenszusatz.SelectedValue Is Nothing Then Exit Sub
        myglobalz.sitzung.aktPerson.Namenszusatz &= cmbNamenszusatz.SelectedValue.ToString & " "
        e.Handled = True
    End Sub

    Private Sub cmbAnrede_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbAnrede.SelectionChanged
        If Not formIstGeladen Then Exit Sub
        If cmbAnrede.SelectedValue Is Nothing Then Exit Sub
        'myGlobalz.sitzung.aktPerson.Anrede &= cmbAnrede.SelectedValue.ToString & " "
        myGlobalz.sitzung.aktPerson.Anrede = cmbAnrede.SelectedValue.ToString & " "
        e.Handled = True
    End Sub

    Private Sub gemeindechanged()
        Dim myvali$ = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        myglobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali) - 438000
        tbgemeinde.Text = myvals.Trim


        myglobalz.sitzung.aktADR.Gisadresse.gemeindeName = tbgemeinde.Text
        myglobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = tbgemeinde.Text

        stckBuchstaben1.IsEnabled = True : stckBuchstaben2.IsEnabled = True
        myglobalz.sitzung.aktADR.PLZ = (glob2.getPLZfromGemeinde(myglobalz.sitzung.aktADR.Gisadresse.gemeindeName))
        myglobalz.sitzung.aktPerson.Kontakt.Anschrift.PLZ = myglobalz.sitzung.aktADR.PLZ

    End Sub

    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbGemeinde.SelectionChanged
        If Not formIstGeladen Then Exit Sub
        If cmbGemeinde.SelectedValue Is Nothing Then Exit Sub
        gemeindechanged()
        e.Handled = True
    End Sub

    Sub initStrassenCombo(buchstabe As String)

        Dim strassseninstanz As New List(Of AdressTools.strassenUiObj)
        Dim att As New AdressTools
        att.buildStrassenListe(buchstabe, strassseninstanz)
        cmbStrasse.DataContext = strassseninstanz

    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myglobalz.appdataDir & "\config\Combos\gemeinden.xml")

    End Sub

    Private Sub cmbStrasse_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStrasse.SelectionChanged
        If Not formIstGeladen Then Exit Sub
        strassegewaehlt()
        e.Handled = True
    End Sub

    Sub initHausNRCombo()
        DB_Oracle_sharedfunctions.DBholeHausnrDT()
        cmbHausnr.DataContext = myglobalz.sitzung.postgresREC.dt
    End Sub

    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbHausnr.SelectionChanged
        If Not formIstGeladen Then Exit Sub
        hausnrgewaehlt()
        e.Handled = True
    End Sub

    Private Sub hausnrgewaehlt()
        Dim item2 As DataRowView = CType(cmbHausnr.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbHausnr.Text = item2.Row.ItemArray(1).ToString
        Dim halo_id As String = CStr(item3)
        myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text
        myglobalz.sitzung.aktPerson.Kontakt.Anschrift.Hausnr = tbHausnr.Text
        glob2.hole_AdressKoordinaten(halo_id)
    End Sub

    Private Sub strassegewaehlt()
        Try
            Dim auswahlStrasse As AdressTools.strassenUiObj = CType(cmbStrasse.SelectedItem, AdressTools.strassenUiObj)
            If auswahlStrasse Is Nothing Then Exit Sub
            If auswahlStrasse.quelle = "mehr" Then Exit Sub
            auswahlStrasse.sname = auswahlStrasse.sname.Trim
            auswahlStrasse.quelle = auswahlStrasse.quelle.Trim
            auswahlStrasse.strcode = auswahlStrasse.strcode.Trim
            If auswahlStrasse.quelle = "fehlt" Then
                MsgBox(glob2.getMsgboxText("strasseUndHausnrVonHand", New List(Of String)(New String() {})))
                myglobalz.sitzung.aktADR.Gisadresse.Quelle = "fehlt"
                tbstrasse.Background = Brushes.PaleVioletRed
                tbHausnr.Background = Brushes.PaleVioletRed
                Exit Sub
            End If

            If auswahlStrasse.quelle = "lage" Then
                tbstrasse.Text = auswahlStrasse.sname ' item2.Row.ItemArray(1).ToString
                ' myGlobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(auswahlStrasse.strcode) 'erzeugt überlauf
                myglobalz.sitzung.aktADR.Gisadresse.Quelle = "lage"
                myglobalz.sitzung.aktADR.Gisadresse.strasseName = tbstrasse.Text
                myglobalz.sitzung.aktPerson.Kontakt.Anschrift.Strasse = tbstrasse.Text
                MsgBox(glob2.getMsgboxText("hausNrEingeben", New List(Of String)(New String() {})))
                tbHausnr.Background = Brushes.PaleVioletRed
            End If

            If auswahlStrasse.quelle = "halo" Then
                myglobalz.sitzung.aktADR.Gisadresse.Quelle = "halo"
                tbstrasse.Text = auswahlStrasse.sname ' item2.Row.ItemArray(1).ToString
                myglobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(auswahlStrasse.strcode)
                myglobalz.sitzung.aktADR.Gisadresse.strasseName = tbstrasse.Text
                myglobalz.sitzung.aktPerson.Kontakt.Anschrift.Strasse = tbstrasse.Text
                initHausNRCombo()
                cmbHausnr.IsDropDownOpen = True
            End If

        Catch ex As Exception
            nachricht("fehler in strassegewaehlt: " ,ex)
        End Try


    End Sub

    Function OrgDTaufOBJabbilden(ByVal neuorgID As Integer) As w_organisation
        Dim neuOrg As New w_organisation
        Dim vgl%
        For Each row As DataRow In myglobalz.sitzung.kontaktdatenREC.dt.AsEnumerable
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




    Sub personAusListeLoeschen()
        Dim messi As New MessageBoxResult
        messi = MessageBox.Show(String.Format("Beteiligten wirklich löschen ?{0}{1}", vbCrLf, myglobalz.sitzung.aktPerson.tostring),
                                "Beteiligten löschen ?",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
        If messi = MessageBoxResult.Yes Then
            clsBeteiligteBUSI.personAusVorgangEntfernen(myglobalz.sitzung.aktPerson.PersonenID, myglobalz.sitzung.aktVorgangsID, myglobalz.sitzung.aktPerson.Status)
            btnSpeichernPerson.IsEnabled = False
        End If
        Me.Close()
    End Sub


    Private Sub tbPostfachPLZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPostfachPLZ.TextChanged
        If Not formIstGeladen Then Exit Sub
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


    Private Sub btnPersonenuernehmen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim winpers As New Window_Person("", "")
        winpers.ShowDialog()
        e.Handled = True
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

        Return False
    End Function

    Private Sub RollenListeAnbieten(ByVal rolle As String)
        Dim winpers As New Window_Person(rolle, "")
        winpers.ShowDialog()
        Dim aaa = myglobalz.sitzung.aktPerson
    End Sub



    Private Sub tbBezirk_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBezirk.TextChanged
        If Not formIstGeladen Then Exit Sub
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

    Private Sub btnBank_Click(sender As Object, e As RoutedEventArgs)
        Dim bv As New WinBankverb(myglobalz.sitzung.aktPerson.PersonenID)
        bv.ShowDialog()
        e.Handled = True
    End Sub

    Private Sub btnNamenAuswaehlen_click(sender As Object, e As RoutedEventArgs)
        Dim alteRolle As String = tbRolle.Text
        Dim winpers As New Window_Person("", tbName.Text)
        winpers.ShowDialog()
        If Not alteRolle.IsNothingOrEmpty Then
            If myglobalz.sitzung.aktPerson.Rolle.IsNothingOrEmpty Then
                myglobalz.sitzung.aktPerson.Rolle = alteRolle
            End If
        End If
        e.Handled = True
    End Sub

    Private Sub initPersonenVorlageCombo()
        Try
            cmbPersonenVorlage.SelectedIndex = myglobalz.sitzung.aktPerson.PersonenVorlage
            initPersonenVorlageComboExtracted()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub cmbPersonenVorlage_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formIstGeladen Then Exit Sub
        initPersonenVorlageComboExtracted()
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        e.Handled = True
    End Sub
    Private Sub initPersonenVorlageComboExtracted()
        If cmbPersonenVorlage.SelectedIndex = 0 Or cmbPersonenVorlage.SelectedIndex = 2 Then
            Background = New SolidColorBrush(Colors.LightGray)
        End If
        If cmbPersonenVorlage.SelectedIndex = 1 Then
            Background = New SolidColorBrush(Colors.LightSkyBlue)
        End If
    End Sub

    Private Sub tbVertretendurch_TextChanged(sender As Object, e As TextChangedEventArgs)
        If Not formIstGeladen Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichernPerson)
        glob2.istTextzulang(198, tbVertretendurch)
        e.Handled = True
    End Sub

    'Private Sub cmbAnrede_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

    'End Sub
End Class
