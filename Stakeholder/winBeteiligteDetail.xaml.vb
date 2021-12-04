Imports System.Data

Partial Public Class winBeteiligteDetail
    Private Property _modus As String
    Property _oldperson As New Person
    Private ladevorgangabgeschlossen As Boolean = False
    Sub New(ByVal modus As string, oldperson As Person, ByRef carry As Boolean)
        InitializeComponent()
        _modus = modus
        _oldperson = clsTools.personenKopieVon(oldperson)
        If modus = "neu" And carry Then
            aktPerson = clsTools.personenKopieVon(oldperson)
        Else
            '  aktPerson.clear()
        End If
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
        e.Handled = True
        starteForm()
        btnSpeichernPerson.IsEnabled = False
        '  clsParadigmaRechte.buttons_schalten(btnSpeichernPerson, btnLoeschenPerson)
        ladevorgangabgeschlossen = True
    End Sub

    Private Sub starteForm()
        setComboboxBeteiligte()
        setcmbNamenszusatz()
        setcmbAnrede()
        inicmbFunktion()
        initGemeindeCombo()

        If _modus = "edit" Then
            btnSpeichernPerson.IsEnabled = False
            btnLoeschenPerson.IsEnabled = True
        End If
        If _modus = "neu" Then
            '  ComboBoxBeteiligte.IsDropDownOpen = True
            'aktPerson.clear()            
            'aktPerson=clsTools.personenKopieVon(_oldperson)
        End If

        dockp.DataContext = aktPerson
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
        Dim filename As String = IO.Path.Combine(appdir, "config", "Combos", "Detail_Beteiligte_Rollen.xml")
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxbeteiligteRollen"), XmlDataProvider)
        existing.Source = New Uri(filename)
        ' ComboBoxBeteiligte.SelectedIndex = 0
    End Sub

    Sub inicmbFunktion()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxbeteiligteFunktion"), XmlDataProvider)
        Dim filen$ = IO.Path.Combine(appdir, "config\Combos\beteiligte_Funktion.xml")
        existing.Source = New Uri(filen)
    End Sub

    Sub Anschrift_generieren()
        'Dim text$ = clsBeteiligteBUSI.Anschrift_Text_erzeugen(aktPerson)
        'Clipboard.Clear()
        'Clipboard.SetText(text)
        'MsgBox(text$ & vbCrLf & vbCrLf & "Die Anschrift befindet sich nun in Ihrer Zwischenablage. " & vbCrLf & _
        ' "Sie können Sie mit Strg-v in Ihr Word-Dokument einfügen." & vbCrLf)
    End Sub

    Private Sub btnSpeichernPerson_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichernPerson.Click
        e.Handled = True
        BeteiligtenAbspeichern()
        btnSpeichernPerson.IsEnabled = False

    End Sub

    Private Shared Function istEingabe_vorhanden() As Boolean
        'If String.IsNullOrEmpty(aktPerson.Rolle) Then
        '    MessageBox.Show("Sie müssen eine Rolle angeben!", "Rolle fehlt!", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        '    Return False
        'End If
        If String.IsNullOrEmpty(aktPerson.Name) Then
            MessageBox.Show("Sie müssen einen Namen angeben!", "Name fehlt!", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Return False
        End If
        Return True
    End Function

    Sub avoidNUlls()
        If String.IsNullOrEmpty(aktPerson.Kontakt.Anschrift.PostfachPLZ) Then aktPerson.Kontakt.Anschrift.PostfachPLZ = ""
        If String.IsNullOrEmpty(aktPerson.Name) Then aktPerson.Name = " "
        If String.IsNullOrEmpty(aktPerson.Vorname) Then aktPerson.Vorname = " "
    End Sub
    Sub BeteiligtenAbspeichern()
        Dim querie As String
        Dim result As Integer = -1
        If Not istEingabe_vorhanden() Then Exit Sub
        If _modus = "neu" Then
            '-------------------------------
            clsSqlparam.paramListe.Clear()
            querie = "INSERT INTO stakeholder (VORGANGSID,NACHNAME,VORNAME,BEMERKUNG,NAMENSZUSATZ,ANREDE,QUELLE,GEMEINDENAME,STRASSE,HAUSNR,POSTFACH,POSTFACHPLZ,FFTELEFON1,FFTELEFON2,FFFAX1," +
                           "FFFAX2,FFMOBILFON,FFEMAIL,FFHOMEPAGE,GESELLFUNKTION,ORGNAME,ORGZUSATZ,ORGTYP1,ORGTYP2,ORGEIGENTUEMER,ROLLE,KASSENKONTO,PLZ,BEZIRK) " +
                                   " VALUES (@VORGANGSID,@NACHNAME,@VORNAME,@BEMERKUNG,@NAMENSZUSATZ,@ANREDE,@QUELLE,@GEMEINDENAME,@STRASSE,@HAUSNR,@POSTFACH,@POSTFACHPLZ,@FFTELEFON1,@FFTELEFON2,@FFFAX1," +
                                   "@FFFAX2,@FFMOBILFON,@FFEMAIL,@FFHOMEPAGE,@GESELLFUNKTION,@ORGNAME,@ORGZUSATZ,@ORGTYP1,@ORGTYP2,@ORGEIGENTUEMER,@ROLLE,@KASSENKONTO,@PLZ,@BEZIRK)"
            avoidNUlls()
            populateParamListe()
            result = ParaRec.manipquerie(querie, clsSqlparam.paramListe, True, "PERSONENID")
            l("result: " & result)
            '---------------------------------
            '  Dim erfolg% = NSdbtools.NSdbtools.Stakeholder_abspeichern_Neu(aktPerson)
            _oldperson = clsTools.personenKopieVon(aktPerson)
            aktPerson.clear()
            btnSpeichernPerson.IsEnabled = False
            Me.Close()
        End If
        If _modus = "edit" Then
            Debug.Print(aktPerson.Kontakt.Anschrift.PLZ)
            clsSqlparam.paramListe.Clear()
            '------------------
            querie = "UPDATE stakeholder " & setSQLbodyUpdate() & " WHERE PERSONENID=@PERSONENID"
            populateParamListe()
            clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENID", aktPerson.PersonenID))
            result = ParaRec.manipquerie(querie, clsSqlparam.paramListe, False, "PERSONENID")
            'Return result
            '-----------------
            'Dim erfolg% = NSdbtools.NSdbtools.Stakeholder_abspeichern_EditExtracted(aktPerson)
            If result > 0 Then
                aktPerson.anychange = False
                _oldperson = clsTools.personenKopieVon(aktPerson)
                aktPerson.clear()
                btnSpeichernPerson.IsEnabled = False
                Me.Close()
            Else
                l("Problem beim Abspeichern! BeteiligtenAbspeichern")
            End If
        End If
        Debug.Print(_oldperson.Name)
    End Sub

    Private Shared Sub populateParamListe()
        With aktPerson
            clsSqlparam.paramListe.Add(New clsSqlparam("NACHNAME", .Name))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORNAME", .Vorname))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEMERKUNG", .Bemerkung.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("NAMENSZUSATZ", .Namenszusatz.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ANREDE", .Anrede.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", .Quelle.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("GEMEINDENAME", .Kontakt.Anschrift.Gemeindename.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("STRASSE", .Kontakt.Anschrift.Strasse.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("HAUSNR", .Kontakt.Anschrift.Hausnr.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("PLZ", CInt(.Kontakt.Anschrift.PLZ.ToString.Trim)))
            clsSqlparam.paramListe.Add(New clsSqlparam("POSTFACH", .Kontakt.Anschrift.Postfach.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("POSTFACHPLZ", .Kontakt.Anschrift.PostfachPLZ.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFTELEFON1", .Kontakt.elektr.Telefon1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFTELEFON2", .Kontakt.elektr.Telefon2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFFAX1", .Kontakt.elektr.Fax1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFFAX2", .Kontakt.elektr.Fax2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFMOBILFON", .Kontakt.elektr.MobilFon.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFEMAIL", .Kontakt.elektr.Email.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFHOMEPAGE", .Kontakt.elektr.Homepage.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("GESELLFUNKTION", .Kontakt.GesellFunktion.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGNAME", .Kontakt.Org.Name.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGZUSATZ", .Kontakt.Org.Zusatz.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGTYP1", .Kontakt.Org.Typ1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGTYP2", .Kontakt.Org.Typ2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGEIGENTUEMER", .Kontakt.Org.Eigentuemer.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ROLLE", .Rolle.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("KASSENKONTO", .Kassenkonto.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEZIRK", .Bezirk.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", 0))
        End With
    End Sub

    Function setSQLbodyUpdate() As String
        Return " SET NACHNAME=@NACHNAME" &
             ",VORNAME=@VORNAME" &
             ",BEMERKUNG=@BEMERKUNG " &
             ",NAMENSZUSATZ=@NAMENSZUSATZ " &
             ",ANREDE=@ANREDE " &
             ",QUELLE=@QUELLE " &
             ",GEMEINDENAME=@GEMEINDENAME " &
             ",STRASSE=@STRASSE " &
             ",HAUSNR=@HAUSNR " &
             ",PLZ=@PLZ" &
             ",POSTFACH=@POSTFACH" &
             ",POSTFACHPLZ=@POSTFACHPLZ" &
             ",FFTELEFON1=@FFTELEFON1 " &
             ",FFTELEFON2=@FFTELEFON2 " &
             ",FFFAX1=@FFFAX1 " &
             ",FFFAX2=@FFFAX2 " &
             ",FFMOBILFON=@FFMOBILFON " &
             ",FFEMAIL=@FFEMAIL " &
             ",FFHOMEPAGE=@FFHOMEPAGE " &
             ",GESELLFUNKTION=@GESELLFUNKTION " &
             ",ORGNAME=@ORGNAME" &
             ",ORGZUSATZ=@ORGZUSATZ" &
             ",ORGTYP1=@ORGTYP1 " &
             ",ORGTYP2=@ORGTYP2 " &
             ",ORGEIGENTUEMER=@ORGEIGENTUEMER " &
             ",ROLLE=@ROLLE " &
             ",KASSENKONTO=@KASSENKONTO " &
             ",BEZIRK=@BEZIRK " &
             ",VORGANGSID=@VORGANGSID"
    End Function

    Private Sub tbRolle_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbRolle.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(145, tbBemerkung)
    End Sub

    Private Sub tbName_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbName.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(150, tbName)
    End Sub

    Private Sub tbVname_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVname.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(140, tbVname)
    End Sub

    Private Sub tbNamenszusatz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbNamenszusatz.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(45, tbNamenszusatz)
    End Sub

    Private Sub tbAnrede_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbAnrede.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(45, tbAnrede)
    End Sub

    Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBemerkung.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(250, tbBemerkung)
    End Sub

    Private Sub tbPLZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPLZ.TextChanged
        clsTools.istTextzulang(7, tbPLZ)
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        'End If

    End Sub

    Private Sub tbgemeinde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbgemeinde.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbgemeinde)
    End Sub

    Private Sub tbstrasse_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbstrasse.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbstrasse)
    End Sub

    Private Sub tbHausnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHausnr.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(45, tbHausnr)
    End Sub

    Private Sub tbPostfach_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPostfach.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(45, tbPostfach)
    End Sub

    Private Sub tbFunktion_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFunktion.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(150, tbFunktion)
    End Sub

    Private Sub tbOrg_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbOrg.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbOrg)
    End Sub

    Private Sub tbOrgzusatz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbOrgzusatz.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbOrgzusatz)
    End Sub

    Private Sub tbTyp1_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTyp1.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbTyp1)
    End Sub

    Private Sub tbTyp2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTyp2.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbTyp2)
    End Sub

    Private Sub tbEigentuemer_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEigentuemer.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbEigentuemer)
    End Sub

    Private Sub tbEmail_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEmail.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbEmail)
    End Sub

    Private Sub tbTelefon_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTelefon.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbTelefon)
    End Sub

    Private Sub tbMobil_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbMobil.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbMobil)
    End Sub

    Private Sub tbFax_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFax.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbFax)
    End Sub

    Private Sub tbHomepage_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHomepage.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(100, tbHomepage)
    End Sub

    Private Sub tbKassenkonto_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbKassenkonto.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(545, tbKassenkonto)
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        Me.Close()
    End Sub

    Private Sub btnLoeschenPerson_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschenPerson.Click
        personAusListeLoeschen()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub cmbFunktion_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbFunktion.SelectionChanged
        Dim item2 As String = CType(cmbFunktion.SelectedValue, String)
        If item2 Is Nothing Then Exit Sub
        aktPerson.Kontakt.GesellFunktion = item2
    End Sub



    Private Sub cmbNamenszusatz_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbNamenszusatz.SelectionChanged
        If cmbNamenszusatz.SelectedValue Is Nothing Then Exit Sub
        aktPerson.Namenszusatz &= cmbNamenszusatz.SelectedValue.ToString & " "
    End Sub

    Private Sub cmbAnrede_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbAnrede.SelectionChanged
        If cmbAnrede.SelectedValue Is Nothing Then Exit Sub
        aktPerson.Anrede &= cmbAnrede.SelectedValue.ToString & " "
    End Sub



    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbGemeinde.SelectionChanged
        If cmbGemeinde.SelectedValue Is Nothing Then Exit Sub
        gemeindechanged()
        cmbStrasse.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Sub initStrassenCombo()
        clsTools.holeStrasseDT()
        cmbStrasse.DataContext = HALOREC.dt
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(IO.Path.Combine(appdir, "config\Combos\gemeinden.xml"))
    End Sub

    Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStrasse.SelectionChanged
        strassegewaehlt()
        e.Handled = True
    End Sub

    Sub initHausNRCombo()
        clsTools.DBholeHausnrDT()
        cmbHausnr.DataContext = HALOREC.dt
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
        aktADR.Gisadresse.HausKombi = tbHausnr.Text
        aktPerson.Kontakt.Anschrift.Hausnr = tbHausnr.Text
        '   glob2.hole_AdressKoordinaten(halo_id%)
        'IfaktADR.punkt.X < 1 OraktADR.punkt.Y < 1 Then
        '    MsgBox("Ein Fall für Google")
        'End If
    End Sub

    Private Sub strassegewaehlt()
        Dim item2 As DataRowView = CType(cmbStrasse.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbstrasse.Text = item2.Row.ItemArray(1).ToString.Trim
        aktADR.Gisadresse.strasseCode = CInt(item3$)
        aktADR.Gisadresse.strasseName = tbstrasse.Text.Trim
        aktPerson.Kontakt.Anschrift.Strasse = tbstrasse.Text.Trim
        initHausNRCombo()
        cmbHausnr.IsDropDownOpen = True
    End Sub


    'Private Shared Function abfragetextbilden(ByVal neuorg As w_organisation) As String
    '    '  glob2.nachricht("in abfragetextbilden")
    '    Dim sb As New Text.StringBuilder
    '    sb.Append("Möchten Sie diese Organisation in die Kontaktdaten übernehmen ? " & vbCrLf)
    '    sb.Append("Name: " & CStr(neuorg.Name) & vbCrLf)
    '    sb.Append("Zusatz: " & CStr(neuorg.Zusatz) & vbCrLf)
    '    sb.Append("Typ1: " & CStr(neuorg.Typ1) & vbCrLf)
    '    sb.Append("Typ2: " & CStr(neuorg.Typ2) & vbCrLf)
    '    sb.Append("Eigentümer: " & CStr(neuorg.Eigentuemer) & vbCrLf)
    '    Return sb.ToString
    'End Function


    Sub personAusListeLoeschen()
        Dim messi As New MessageBoxResult
        messi = MessageBox.Show(String.Format("Beteiligten wirklich löschen ?{0}{1}", vbCrLf, aktPerson.tostring),
                                "Beteiligten löschen ?",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
        If messi = MessageBoxResult.Yes Then
            If StakeholderLoeschen(aktPerson.PersonenID) > 0 Then

            End If
            'If NSdbtools.NSdbtools.Stakeholder_loeschen(aktPerson.PersonenID, ParaRec) > 0 Then

            'End If
            btnSpeichernPerson.IsEnabled = False
        End If
    End Sub

    Private Function StakeholderLoeschen(personenid As Integer) As Integer
        Dim query, hinweis As String
        Dim result% = 1
        query = "delete from stakeholder where  personenid=" & personenid
        l(query)
        ParaRec.dt = getDT4Query(query, ParaRec, hinweis)
        l(hinweis)
        Return result
    End Function

    Private Sub tbPostfachPLZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPostfachPLZ.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(45, tbPostfachPLZ)
    End Sub

    Private Sub tbBezirk_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBezirk.TextChanged
        clsTools.schliessenButton_einschalten(btnSpeichernPerson)
        clsTools.istTextzulang(150, tbBezirk)
    End Sub
    Private Sub gemeindechanged()
        Dim myvali$ = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        aktADR.Gisadresse.gemeindeNr = CInt(myvali) - 438000
        tbgemeinde.Text = myvals$

        aktADR.Gisadresse.gemeindeName = tbgemeinde.Text
        aktPerson.Kontakt.Anschrift.Gemeindename = tbgemeinde.Text
        initStrassenCombo()
        aktADR.PLZ = (getPLZfromGemeinde(aktADR.Gisadresse.gemeindeName))
        aktPerson.Kontakt.Anschrift.PLZ = aktADR.PLZ
    End Sub

    Private Sub ComboBoxBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)

    End Sub

    Private Sub ComboBoxrolle_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)

    End Sub

Private Sub btnAbbruch_Click_1(sender As Object , e As RoutedEventArgs)

    End Sub

Private Sub btnSpeichernPerson_Click_1(sender As Object , e As RoutedEventArgs)

    End Sub


End Class
