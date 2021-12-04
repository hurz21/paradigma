Imports System.Data

Public Class winUserDetail
    Private Const STR_Aktiv As String = "aktiv"
    Private Const STR_InAktiv As String = "inaktiv"
    'Private Const STR_User As String = "user"
    'Const STR_Vorzimmer As String = "vorzimmer"
    Property _testUser As clsBearbeiter
    Property modus As String
    Property uc As New UserControl
    Sub New(ByVal testUser As clsBearbeiter)
        InitializeComponent()
        _testUser = testUser
        modus = "edit"
    End Sub
    Sub New()
        InitializeComponent()
        modus = "neu"
        _testUser = New clsBearbeiter
        inittestuser()
    End Sub
    Private Sub winUserDetail_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        dockpBearbeiter.DataContext = _testUser
        setzeStaTUS()

        btnSpeichern.IsEnabled = False
        '   clsParadigmaRechte.buttons_schalten(btnSpeichernPerson, btnLoeschenPerson)
        uc = New UserControl()
        popUp.Child = uc
        refreshADeintrag()


        userTools.getUsersGruppeDatatable(_testUser.ID, myGlobalz.sitzung.VorgangREC)
        dgUserDS.DataContext = myGlobalz.sitzung.VorgangREC.dt
        e.Handled = True
    End Sub

    'Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    e.Handled = True
    'End Sub

    Private Sub ComboBox2_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBox2.SelectionChanged
        If ComboBox2.SelectedValue Is Nothing Then Exit Sub
        Dim item As System.Windows.Controls.ComboBoxItem = CType(ComboBox2.SelectedItem, System.Windows.Controls.ComboBoxItem)
        If String.IsNullOrEmpty(item.Content.ToString.Trim) Then Exit Sub
        Dim auswahl As String = item.Content.ToString.Trim '.Split("#"c)
        tbRang.Text = auswahl
        e.Handled = True
    End Sub

    Private Sub ComboBox3_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBox3.SelectionChanged
        If ComboBox3.SelectedValue Is Nothing Then Exit Sub
        Dim item As System.Windows.Controls.ComboBoxItem = CType(ComboBox3.SelectedItem, System.Windows.Controls.ComboBoxItem)
        If String.IsNullOrEmpty(item.Content.ToString.Trim) Then Exit Sub
        Dim auswahl As String = item.Content.ToString.Trim '.Split("#"c)
        tbRolle.Text = auswahl
        e.Handled = True
    End Sub

    Private Sub ComboBox4_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBox4.SelectionChanged
        If ComboBox4.SelectedValue Is Nothing Then Exit Sub
        Dim item As System.Windows.Controls.ComboBoxItem = CType(ComboBox4.SelectedItem, System.Windows.Controls.ComboBoxItem)
        If String.IsNullOrEmpty(item.Content.ToString.Trim) Then Exit Sub
        Dim auswahl As String = item.Content.ToString.Trim '.Split("#"c)
        tbAbteilung.Text = auswahl
        e.Handled = True
    End Sub

    Private Sub tbRang_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbRang.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbRang)
        e.Handled = True
    End Sub

    Private Sub tbVorname_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVorname.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbVorname)
        tbName_TextChangedExtracted()
        e.Handled = True
    End Sub

    Private Function getVornameBuchstabe() As String
        If String.IsNullOrEmpty(tbVorname.Text) Then
            Return ""
        End If

        If tbVorname.Text.Length > 0 Then
            Return tbVorname.Text.Substring(0, 1)
        End If
        Return "?"
    End Function
    Private Sub tbName_TextChangedExtracted()
        If modus = "neu" Then
            testid.Text = LIBgemeinsames.clsString.umlaut2ue(tbName.Text) & "_" & getVornameBuchstabe()
        End If
    End Sub
    Private Sub tbName_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbName.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbName)
        tbName_TextChangedExtracted()
        e.Handled = True
    End Sub

    Private Sub tbNamenszusatz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbNamenszusatz.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbNamenszusatz)
        e.Handled = True
    End Sub

    Private Sub tbINITIAL_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbINITIAL.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbINITIAL)
        e.Handled = True
    End Sub

    Private Sub tbKUERZEL1_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbKUERZEL1.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(2, tbKUERZEL1)
        e.Handled = True
    End Sub

    Private Sub tbTelefon_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTelefon.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(145, tbTelefon)
        e.Handled = True
    End Sub

    Private Sub tbFax_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFax.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(145, tbFax)
        e.Handled = True
    End Sub

    Private Sub tbEmail_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbEmail.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbEmail)
        e.Handled = True
    End Sub

    Private Sub tbRolle_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbRolle.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(145, tbRolle)
        e.Handled = True
    End Sub

    Private Sub tbAbteilung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbAbteilung.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbAbteilung)
        e.Handled = True
    End Sub
    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If modus = "neu" Then
            MessageBox.Show("Bitte beachten:" & Environment.NewLine &
                        "" & Environment.NewLine &
                        "Sie müssen dem neuen User*In auch ausreichend Gruppenzugehörigkeiten  " & Environment.NewLine &
                        "zuweisen. " & Environment.NewLine &
                        "Sonst kann er viele Vorgänge nicht aufrufen!" & Environment.NewLine &
                        " " & Environment.NewLine &
                        "Tipp: Nehmen Sie einen ähnlichen User als Vorbild." & Environment.NewLine &
                        "" & Environment.NewLine)
        End If
        Dim ok As String = istEingabeOk()
        If Not ok = "ok" Then
            MessageBox.Show("Die Eingaben waren nicht in Ordnung: " & ok)
            Exit Sub
        End If
        If userPropSetzen(_testUser) Then
            BearbeiterCRUD.userSpeichern(_testUser, _modus)
            btnSpeichern.IsEnabled = False
            If modus = "neu" Then
                Dim text As String = "Hinweis:" & Environment.NewLine &
                    " " & Environment.NewLine &
                    "Um einen neuen Anwender voll zu integrieren sind folgende weitere Schritte notwendig:" & Environment.NewLine &
                    "" & Environment.NewLine &
                    "1 - Die IT muss den Anwender als Teil des FD Umwelt markieren" & Environment.NewLine &
                    "    Dies ist daran erkennbar, dass der Anwender nach dem Login das Laufwerk " & Environment.NewLine &
                    "           O:\UMWELT\B\GISDatenEkom sehen kann" & Environment.NewLine &
                    "" & Environment.NewLine &
                    "2 - Sobald (1) erledigt ist muss das Programm zweimal ausgeführt werden." & Environment.NewLine &
                    "" & Environment.NewLine &
                    "Jetzt erst ist Paradigma einsatzbereit." & Environment.NewLine &
                    "" & Environment.NewLine
                MessageBox.Show(text, "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information)
            End If
        End If
        Close()
        e.Handled = True
    End Sub

    Private Function istEingabeOk() As String
        If String.IsNullOrEmpty(_testUser.username) Then
            Return "Username/ UserID fehlt. Abbruch"
        End If

        If String.IsNullOrEmpty(_testUser.Name) Then
            Return "Name fehlt. Abbruch"
        End If
        If String.IsNullOrEmpty(_testUser.Vorname) Then
            Return "Vorname fehlt. Abbruch"
        End If
        If String.IsNullOrEmpty(_testUser.Rang) Then
            Return "Rang fehlt. Abbruch"
        End If
        'If String.IsNullOrEmpty(_testUser.Rites) Then
        '    Return "Rites fehlt. Abbruch"
        'End If
        If String.IsNullOrEmpty(_testUser.Initiale) Then
            Return "Initial fehlt. Abbruch"
        End If

        If String.IsNullOrEmpty(_testUser.Bemerkung) Then
            Return "Abteilungfehlt. Abbruch"
        End If
        If String.IsNullOrEmpty(_testUser.Kontakt.elektr.Telefon1) Then
            Return "Telefon fehlt. Abbruch"
        End If
        If String.IsNullOrEmpty(_testUser.Kontakt.elektr.Fax1) Then
            Return "Fax fehlt. Abbruch"
        End If
        If String.IsNullOrEmpty(_testUser.Kontakt.elektr.Email) Then
            Return "Email fehlt. Abbruch"
        End If
        If String.IsNullOrEmpty(_testUser.Rolle) Then
            Return "Rolle fehlt. Abbruch"
        End If
        If String.IsNullOrEmpty(_testUser.ExpandHeaderInSachgebiet) Then
            Return "Explorerangabe fehlt. Wurde die Abteilung geändert? Abbruch."
        End If
        If String.IsNullOrEmpty(_testUser.Kuerzel2Stellig) Then
            Return "Kürzel fehlt. Abbruch"
        End If
        Return "ok"
    End Function

    Private Function userPropSetzen(ByVal clsBearbeiter As clsBearbeiter) As Boolean
        If (tbAktiv.Text) = STR_Aktiv Then
            _testUser.Status = 1
        End If
        If (tbAktiv.Text) = STR_InAktiv Then
            _testUser.Status = 0
        End If
        Return True
    End Function

    Private Sub setzeStaTUS()
        If _testUser.Status = 1 Then
            tbAktiv.Text = CStr(STR_Aktiv)
        End If
        If _testUser.Status = 0 Then
            tbAktiv.Text = CStr(STR_InAktiv)
        End If
    End Sub

    Private Sub ComboBox1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBox1.SelectionChanged
        If ComboBox1.SelectedValue Is Nothing Then Exit Sub
        Dim item As System.Windows.Controls.ComboBoxItem = CType(ComboBox1.SelectedItem, System.Windows.Controls.ComboBoxItem)
        If String.IsNullOrEmpty(item.Content.ToString.Trim) Then Exit Sub
        Dim auswahl As String = item.Content.ToString.Trim '.Split("#"c)
        If auswahl = STR_Aktiv Then
            tbAktiv.Text = CStr(STR_Aktiv)
        End If
        If auswahl = STR_InAktiv Then
            tbAktiv.Text = CStr(STR_InAktiv)
        End If
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub cmbExplorer_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbExplorer.SelectionChanged
        If cmbExplorer.SelectedValue Is Nothing Then Exit Sub
        Dim item As System.Windows.Controls.ComboBoxItem = CType(cmbExplorer.SelectedItem, System.Windows.Controls.ComboBoxItem)
        If String.IsNullOrEmpty(item.Content.ToString.Trim) Then Exit Sub
        Dim auswahl As String = item.Content.ToString.Trim '.Split("#"c)
        tbExplorer.Text = auswahl
        e.Handled = True
    End Sub

    Private Sub inittestuser()
        With _testUser
            .Name = "????"
            .Vorname = "????"
            .Rang = "user"
            .Raum = ""
            .Initiale = "????"
            .Bemerkung = "Untere Naturschutzbehörde"
            .Status = 1
            .Kontakt.elektr.Fax1 = "4910"
            .ExpandHeaderInSachgebiet = "3-Naturschutz"
            .Kontakt.elektr.Email = "????@kreis-offenbach.de"
            .Kontakt.elektr.Telefon1 = "????"
            .Kuerzel2Stellig = "??"
            .Namenszusatz = ""
            .Rolle = "??"
            .Anrede = "Frau"
        End With
    End Sub


    Private Sub tbExplorer_MouseEnter(sender As Object, e As MouseEventArgs)
        '  uc.Content = "Ich bin ein Hilfetext für eine TextBox" & Environment.NewLine &
        '"Ich bin ein Hilfetext für eine TextBox" & Environment.NewLine &
        '"Ich bin ein Hilfetext für eine TextBox" & Environment.NewLine &
        '"Ich bin ein Hilfetext für eine TextBox" & Environment.NewLine
        '  popUp.IsOpen = True
        e.Handled = True
    End Sub

    Private Sub popUp_MouseDown(sender As Object, e As MouseButtonEventArgs)
        popUp.IsOpen = False
        e.Handled = True
    End Sub


    Private Sub refreshADeintrag()
        Dim lokdt As New DataTable
        'Dim loklist As New List(Of clsTupelString)
        Dim realDepartment As String = ""
        Dim filter As String = _testUser.username
        If _testUser.username = "Mueller" Then
            Debug.Print("")
        End If
        filter = LIBgemeinsames.clsString.umlaut2ue(filter)
        lokdt = JFactiveDirectory.clsActiveDir.sucheperson(filter)
        If lokdt Is Nothing OrElse lokdt.Rows.Count < 1 Then
            dgPersonal.DataContext = Nothing
        Else
            Select Case JFactiveDirectory.umweltamt.istImUmweltamt(lokdt, realDepartment)
                Case -1
                    MsgBox("Die Bearbeiterin " & filter & " ist nicht Teil des Umweltamtes (" & realDepartment &
                           "). Daher hat er keine vollen Rechte. Wenn gewünscht bitte Email an IT, mit der Bitte um Zuweisung der Person an den FD Umwelt im ActiveDirectory.")
                Case 0
                    MsgBox("Die Bearbeiterin  " & filter & " ist noch nicht erfasst (Abt: " & realDepartment & ")")
                Case 1
                    'alles ok
            End Select
            dgPersonal.DataContext = lokdt
        End If
    End Sub


    Private Sub btnAdd_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppen As New win_DSGruppenauswahl(_testUser.ID, "add")
        gruppen.ShowDialog()
        If _testUser.ID < 1 OrElse
            gruppen.auswahlid.IsNothingOrEmpty OrElse
            gruppen.auswahlid = Nothing Then
        Else
            If gruppen.auswahlid = "9" Then 'fachdienstleitung
                If Environment.UserName.ToLower = "weyers_g" Or
                    Environment.UserName.ToLower = "nehler_u" Or
                    Environment.UserName.ToLower = "feinen_j" Or
                    Environment.UserName.ToLower = "nitsch_j" Or
                    Environment.UserName.ToLower = "hopfgarten_p" Then
                    If modDS_tools.DS_users_Add_gruppe(_testUser.ID, gruppen.auswahlid) Then
                        MsgBox("Gruppe erfolgreich hinzugefügt. Form wird geschlossen!")
                    Else
                        MsgBox("problem bei DS_users_Add_gruppe: admin informieren.Form wird geschlossen!")
                    End If

                Else
                    MsgBox("Sieh haben nicht das Recht dieses Recht zu vergeben!")
                End If
            Else
                If modDS_tools.DS_users_Add_gruppe(_testUser.ID, gruppen.auswahlid) Then
                    MsgBox("Gruppe erfolgreich hinzugefügt. Form wird geschlossen!")
                Else
                    MsgBox("problem bei DS_users_Add_gruppe: admin informieren.Form wird geschlossen!")
                End If
            End If
        End If
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppen As New win_DSGruppenauswahl(_testUser.ID, "remove")
        gruppen.ShowDialog()
        If _testUser.ID < 1 OrElse
            gruppen.auswahlid.IsNothingOrEmpty OrElse
            gruppen.auswahlid = Nothing Then
        Else
            If modDS_tools.DS_users_Remove_gruppe(_testUser.ID, gruppen.auswahlid) Then
                MsgBox("Gruppe erfolgreich entfernt. Form wird geschlossen!")
            Else
                MsgBox("problem bei DS_users_remove_gruppe: admin informieren.Form wird geschlossen!")
            End If
        End If
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub cmbExplorerAnrede_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If cmbExplorerAnrede.SelectedValue Is Nothing Then Exit Sub
        Dim item As System.Windows.Controls.ComboBoxItem = CType(cmbExplorerAnrede.SelectedItem, System.Windows.Controls.ComboBoxItem)
        If String.IsNullOrEmpty(item.Content.ToString.Trim) Then Exit Sub
        Dim auswahl As String = item.Content.ToString.Trim '.Split("#"c)
        tbanrede.Text = auswahl
    End Sub
End Class
