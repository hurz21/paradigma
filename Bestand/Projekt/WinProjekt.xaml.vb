Imports System.Data

Public Class WinProjekt
    Public aktProjekt As clstart.clsProjektAllgemein
    Public modus As String

    Sub New(ByVal Emodus$, ByVal Eaktprojekt As clstart.clsProjektAllgemein)
        InitializeComponent()
        modus = Emodus
        aktProjekt = Eaktprojekt
    End Sub
    Private Sub Abbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Abbruch.Click
        Me.Close()
        e.Handled = True
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemeinden.xml")
    End Sub

    Private Sub WinProjekt_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initCMBs()
        If modus = "neu" Then
            btnLoeschen.IsEnabled = False
            aktProjekt.von = Now
            aktProjekt.bis = Now
            aktProjekt.Kategorie1 = ""
            aktProjekt.Kategorie2 = ""
            aktProjekt.BezeichnungLang = ""
            aktProjekt.BezeichnungKurz = ""
            cmbObergruppe.SelectedValue = ""
            cmbUntergruppe.SelectedValue = ""
            btnEntkoppeln.Visibility = Windows.Visibility.Collapsed
        End If
        If modus = "edit" Then
            cmbObergruppe.SelectedValue = aktProjekt.Kategorie1
            btnLoeschen.IsEnabled = True
            btnEntkoppeln.Visibility = Windows.Visibility.Collapsed
        End If
        If modus = "koppeln" Then
            btnEntkoppeln.Visibility = Windows.Visibility.Visible
            btnLoeschen.Visibility = Windows.Visibility.Collapsed
            Speichern.Visibility = Windows.Visibility.Collapsed
        End If
        Title = StammToolsNs.setWindowTitel.exe(modus, "Projekt")
        gridMain.DataContext = aktProjekt
    End Sub

    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim liste As String = NSprojekt.Projekt_gekoppelteListe.exe(aktProjekt)
        If Not String.IsNullOrEmpty(liste.Trim) Then
            MessageBox.Show("HINWEIS: Das Projekt ist an Vorgänge gekoppelt." & vbCrLf & vbCrLf &
                            "Vorgänge: " & liste & vbCrLf & vbCrLf &
                            "Wenn Sie es löschen wollen müssen Sie zuerst die einzelnen Vorgänge vom Projekt entkoppeln!", "Projekt ist gekoppelt", MessageBoxButton.OK, MessageBoxImage.Information)
        Else
            NSprojekt.loeschen.exe(aktProjekt)
        End If
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub Speichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        aktProjekt.Quelle = myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig
        aktProjekt.vorgangsid = myGlobalz.sitzung.aktVorgangsID
        If Not EingabeistOk() Then
            Exit Sub
        End If
        NSprojekt.speichern.exe(modus, aktProjekt)
        DialogResult = False
        Me.Close()
        e.Handled = True
    End Sub

    'Private Function getKategorie1() As String
    '    If cmbObergruppe.SelectedValue Is Nothing Then
    '        Return ""
    '    End If
    '    Return cmbObergruppe.SelectedValue.ToString
    'End Function

    'Private Function getKategorie2() As String
    '    If cmbUntergruppe.SelectedValue Is Nothing Then
    '        Return ""
    '    End If
    '    Return cmbUntergruppe.SelectedValue.ToString
    'End Function

    Private Function EingabeistOk() As Boolean
        If String.IsNullOrEmpty(aktProjekt.BezeichnungKurz) Then
            MsgBox("Bitte eine Kurzbezeichnung eingeben!")
            Return False
        End If
        If String.IsNullOrEmpty(aktProjekt.BezeichnungLang) Then
            MsgBox("Bitte eine Langbezeichnung eingeben!")
            Return False
        End If
        If String.IsNullOrEmpty(aktProjekt.Kategorie1) Then
            MsgBox("Bitte eine Obergruppe anwählen !")
            Return False
        End If
        If String.IsNullOrEmpty(aktProjekt.Gemeinde) Then
            MsgBox("Bitte den Bereich / Gemeinde angeben !")
            Return False
        End If
        Return True
    End Function

    Private Sub cmbObergruppe_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbObergruppe.SelectionChanged
        e.Handled = True
    End Sub

    Private Sub cmbUntergruppe_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbUntergruppe.SelectionChanged
        If cmbUntergruppe.SelectedValue Is Nothing Then Exit Sub
        e.Handled = True
    End Sub

    Private Sub initCMBs()
        cmbObergruppe.Items.Add("B-Plan")
        cmbObergruppe.Items.Add("Bauantrag")
        cmbObergruppe.Items.Add("Kompensation")
        cmbObergruppe.Items.Add("Bauantrag")
        cmbObergruppe.Items.Add("Artbeobachtung")
        cmbObergruppe.Items.Add("Ersatzgeld")
        cmbObergruppe.Items.Add("Planfeststellung")
        cmbObergruppe.Items.Add("unbestimmt")

        initGemeindeCombo()
    End Sub

    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        myGlobalz.sitzung.aktWiedervorlage.clear()
        myGlobalz.sitzung.aktWiedervorlage.ToDo = aktProjekt.BezeichnungKurz
        myGlobalz.sitzung.aktWiedervorlage.Bemerkung = aktProjekt.BezeichnungLang
        myGlobalz.sitzung.aktWiedervorlage.datum = aktProjekt.bis
        myGlobalz.sitzung.aktWiedervorlage.VorgangsID = myGlobalz.sitzung.aktVorgangsID

        Dim lResult As Boolean = clsWVTOOLS.WVneuOKExtracted(myGlobalz.sitzung.aktVorgangsID)
        aktProjekt.WiedervorlageID = myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID
        e.Handled = True
    End Sub

    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Dim myvali$ = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        aktProjekt.Gemeinde = myvals$
    End Sub

    Private Sub tbBezeichnungLang_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        e.Handled = True
    End Sub

    Private Sub btnEntkoppeln_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If NSprojekt.Kopplung.aufheben(aktProjekt) Then

        End If
        e.Handled = True
        Close()
    End Sub
End Class
