Imports System.Data

Partial Public Class winStandortDetail
	Private stammid%
	Sub New(ByVal _id%)
		InitializeComponent()
		stammid% = _id
		Me.Title = If(stammid < 1, "Neuaufnahme", "Änderung der Standortnr: " & stammid)
	End Sub

	Private Sub winStandortDetail_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
		initcombos()
		If stammid = 0 Then
			'stammmode	 NEU
			Loeschen.IsEnabled = False
		Else
			'editmode
			refreshDokuliste()
		End If
		setDatacontext()

		Speichern.IsEnabled = False
	End Sub
	Sub initcombos()
        glob.halodtFuellen("select    KZ,NAME from gmd order by name asc")
		cmbGemeinde.DataContext = glob.haloDBREC.dt
	End Sub


	Sub setDatacontext()
		tbTitel.DataContext = aktStandort
		tbBemerkung.DataContext = aktStandort
		tbHinweis.DataContext = aktStandort
		tbRechts.DataContext = aktStandort.pt
		tbHoch.DataContext = aktStandort.pt
		tbGemeinde.DataContext = aktStandort.adr
		tbStrasse.DataContext = aktStandort.adr
		tbHausnr.DataContext = aktStandort.adr
	End Sub


	Private Sub refreshDokuliste()
		glob.dtFuellen("select * from medien where stammid=" & stammid%)
		dgDokumente.DataContext = glob.StandortDBREC.dt
	End Sub

	Function keineMedienMehr() As Boolean
		glob.dtFuellen("select * from medien where stammid=" & stammid%)
		If glob.StandortDBREC.dt.Rows.Count > 0 Then
			Return False
		Else
			Return True
		End If
	End Function

	Private Sub btnStandortloeschen(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		If keineMedienMehr() Then
			If StandortWirklichLoschen() Then
				aktStandort.loeschen(stammid)
				aktStandort.clear()
				Me.Close()
			End If
		Else
			MessageBox.Show("Sie müssen zuerst die Medien löschen die diesem Standort zugeordnet sind!", "Standort löschen", MessageBoxButton.OK, MessageBoxImage.Exclamation)
		End If
	End Sub
	Function StandortWirklichLoschen() As Boolean
		Dim res As New MessageBoxResult
		res = MessageBox.Show("Wollen Sie das Objekt wirklich löschen?", "Standort löschen", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
		If res = MessageBoxResult.Yes Then
			Return True
		Else
			Return False
		End If
	End Function

	Private Sub btnStandortspeichern(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		If EingabeStandortOK() Then
			aktStandort.speichern(stammid)
		Else
			MessageBox.Show("Gemeindename und Straße sind erforderlich. Am besten auch die Hausnummer!")
		End If
	End Sub

	Function EingabeStandortOK() As Boolean
		If String.IsNullOrEmpty(aktStandort.adr.Gemeindename) Then Return False
		If String.IsNullOrEmpty(aktStandort.adr.Strassenname) Then Return False
		If String.IsNullOrEmpty(aktStandort.pt.X.ToString) Then
			MessageBox.Show("Sie können die Koordinaten aus dem BürgerGIS entnehmen (Messen-Koordinaten)")
		End If
		If String.IsNullOrEmpty(aktStandort.Titel) Then
			MessageBox.Show("Vergeben Sie nach Möglichkeit einen Titel. Er erleichtert das Wiederfinden des Stanortes in der Tabelle!")
		End If
		Return True
	End Function

	Private Sub btnStandortAbbruch(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
	End Sub

	Private Sub dgDokumente_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgDokumente.SelectionChanged
		Try
			If dgDokumente.SelectedItem Is Nothing Then Exit Sub
			Dim item As DataRowView
			item = CType(dgDokumente.SelectedItem, DataRowView)
			Dim id% = CInt(item("ID"))
			Dim titel$ = CStr(item("Titel"))
			Dim dateiname$ = CStr(item("Dateiname"))
			Dim relativpfad$ = CStr(item("Relativpfad"))
			Dim medienDatei$ = glob.StandortDBREC.mydb.MySQLServer & glob.pdfStammroot$ & relativpfad & "\" & dateiname
			medienDatei$ = medienDatei$.Replace("/", "\")
			dgDokumente.SelectedItem = Nothing
			If RBanschauen.IsChecked Then
				glob.OpenDocument(medienDatei)
			End If
			If RBloeschen.IsChecked Then
				glob.killDocument(id, medienDatei)
				refreshDokuliste()
			End If
		Catch ex As Exception
			glob.nachricht_und_Mbox(ex.ToString)
		End Try
	End Sub

	Private Sub dokuhinzu(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		Dim filenames$() = Nothing
		If glob.Medienauswaehlen(filenames, glob.MedienInitialDir$) Then
			glob.medienHinzufuegen(filenames)
		Else
		End If
		refreshDokuliste()
	End Sub

	Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
		If cmbGemeinde.SelectedValue Is Nothing Then Exit Sub
		Dim item As DataRowView = CType(cmbGemeinde.SelectedItem, DataRowView)
        If item Is Nothing Then Return
		aktStandort.adr.gemnrbig = CInt(cmbGemeinde.SelectedValue.ToString)
		aktStandort.adr.Gemeindename = item(1).ToString.Trim
		holestrassen()
	End Sub

	Sub holestrassen()
        Dim lNewVariable As String = "select distinct TRIM([sname]) as sname from halofsneu where gemeindenr=" & aktStandort.adr.gemnrbig '& " order by sname"
		glob.halodtFuellen(lNewVariable)
		cmbStrasse.DataContext = glob.haloDBREC.dt
    End Sub

	Private Sub tbTitel_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbTitel.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.istTextzulang(250, tbTitel)
	End Sub

	Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBemerkung.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.istTextzulang(250, tbBemerkung)
	End Sub

	Private Sub tbHinweis_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHinweis.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.istTextzulang(250, tbHinweis)
	End Sub

	Private Sub tbRechts_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbRechts.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.pruefeObZahl(tbRechts)
	End Sub

	Private Sub tbHoch_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHoch.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.pruefeObZahl(tbHoch)
	End Sub

	Private Sub tbGemeinde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbGemeinde.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.istTextzulang(150, tbGemeinde)
	End Sub

	Private Sub tbStrasse_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbStrasse.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.istTextzulang(250, tbStrasse)
	End Sub

	Private Sub tbHausnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHausnr.TextChanged
		glob.schliessenButton_einschalten(Speichern)
		glob.istTextzulang(50, tbHausnr)
	End Sub

	Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
		If cmbStrasse.SelectedValue Is Nothing Then Exit Sub
		Dim item As DataRowView = CType(cmbStrasse.SelectedItem, DataRowView)
        If item Is Nothing Then Return

        aktStandort.adr.Strassenname = (item(0)).ToString.Trim
        aktStandort.adr.Strassencode = item(1).ToString ' CInt(cmbStrasse.SelectedValue)

		holehausnr()
	End Sub

	Sub holehausnr()
        Dim lNewVariable As String = "select distinct ID,  TRIM([hausnr]) & TRIM([zusatz]) as kombi from halofsneu " & _
   " where gemeindenr=" & aktStandort.adr.gemnrbig & _
   " and strcode='" & aktStandort.adr.Strassencode & "'"
		' " order by kombi"
		glob.halodtFuellen(lNewVariable)
		cmbHausnr.DataContext = glob.haloDBREC.dt
	End Sub

	Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
		If cmbHausnr.SelectedValue Is Nothing Then Exit Sub
		Dim id% = CInt(cmbHausnr.SelectedValue.ToString)

		If cmbHausnr.SelectedValue Is Nothing Then Exit Sub
		Dim item As DataRowView = CType(cmbHausnr.SelectedItem, DataRowView)
		If item Is Nothing Then Return
		aktStandort.adr.HausnrKombi = (item(1)).ToString.Trim
		holeKoordinaten(id%)
	End Sub
	Sub holeKoordinaten(ByVal id%)
        Dim lNewVariable As String = "select distinct rechts,hoch from halofsneu " & _
  " where id=" & id
		' " order by kombi"
		glob.halodtFuellen(lNewVariable)
		aktStandort.pt.X = CDbl(glob.haloDBREC.dt.Rows(0).Item("rechts"))
		aktStandort.pt.Y = CDbl(glob.haloDBREC.dt.Rows(0).Item("hoch"))
	End Sub
End Class
