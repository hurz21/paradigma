Imports System.Data

Partial Public Class WINsuchen

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        refreshGrid()
        initcombos()
        'Dim gemparms As New clsGemarkungsParams
        'gemparms.init() : Dim result$ = "ERROR"
        ''	Dim a = From item In gemparms.parms Where item.gemarkungstext.ToLower = "disetesheim" Select item.gemeindetext
        'Dim a = From item In gemparms.parms Select  item.gemeindetext, item.gemarkungskuerzel		distinct
        'If a.ToArray.Length > 0 Then
        '	cmbGemeinde.DataContext = a
        'Else
        '	cmbGemeinde.DataContext = Nothing
        'End If
    End Sub

	Sub refreshGrid()
		'glob.dtFuellen("select * from view1 order by gemeindename,strassenname,hausnummer ")
		glob.dtFuellen("select * from ohneMedien order by Ort,Strasse,Hausnr")
		dgPlaene.DataContext = glob.StandortDBREC.dt
	End Sub

	Function DRV_2_obj(ByVal item As DataRowView, ByVal aktStandort As clsStandortPlan) As clsStandortPlan
		Try
			'Dim aktStandort As New clsStandortPlan
			aktStandort.StammID = CInt(item("StammID"))
			aktStandort.Titel = CStr(item("titel"))
			aktStandort.Hinweis1 = CStr(item("hinweis1"))
			aktStandort.Hinweis2 = CStr(item("hinweis2"))
			aktStandort.adr.Gemeindename = CStr(item("ort"))
			aktStandort.adr.Strassenname = CStr(item("strasse"))
			aktStandort.adr.HausnrKombi = CStr(item("hausnr"))
			aktStandort.pt.X = CDbl(item("rechts"))
			aktStandort.pt.Y = CDbl(item("hoch"))
			Return aktStandort
		Catch ex As Exception
			MsgBox(ex.ToString)
			Return Nothing
		End Try
	End Function

	Private Sub dgPlaene_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgPlaene.SelectionChanged
		Try
			If dgPlaene.SelectedItem Is Nothing Then Exit Sub
			Dim item As DataRowView
			item = CType(dgPlaene.SelectedItem, DataRowView)
			aktStandort = New clsStandortPlan(dbcrudpl)
			aktStandort = DRV_2_obj(item, aktStandort)
			dgPlaene_SelectionChangedExtracted(item)
			dgPlaene.SelectedItem = Nothing
            '	If rbDetail.IsChecked Then refreshGrid()
		Catch ex As Exception
			MsgBox(ex.ToString)
		End Try
	End Sub

	Private Sub dgPlaene_SelectionChangedExtracted(ByVal item As DataRowView)
		If rbView.IsChecked Then
			zeigeMedienListe(aktStandort.StammID)
		End If
		If rbDetail.IsChecked Then
			glob.editStandort(aktStandort.StammID)
		End If
		If rbBGIS.IsChecked Then
			glob.zeigeStandortImBgis(aktStandort)
		End If
		If rbGoogleMaps.IsChecked Then
			Dim ww As New WINwebrowser
			ww.Show()
			ww.wb2.Navigate(New Uri(glob.googlemaps(item), UriKind.RelativeOrAbsolute))
		End If
	End Sub
	Sub zeigeMedienListe(ByVal stammID%)
		glob.dtFuellen("select * from medien where stammid=" & stammID%)
		dgMeien.DataContext = glob.StandortDBREC.dt
	End Sub

	Private Sub dgMeien_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgMeien.SelectionChanged
		Try
			If dgMeien.SelectedItem Is Nothing Then Exit Sub
			Dim item As DataRowView
			item = CType(dgMeien.SelectedItem, DataRowView)
			Dim titel$ = CStr(item("Titel"))
			Dim dateiname$ = CStr(item("Dateiname"))
			Dim relativpfad$ = CStr(item("Relativpfad"))
			Dim medienDatei$ = glob.StandortDBREC.mydb.MySQLServer & glob.pdfStammroot$ & relativpfad & "\" & dateiname
			medienDatei$ = medienDatei$.Replace("/", "\")
			dgMeien.SelectedItem = Nothing
			glob.OpenDocument(medienDatei)
		Catch ex As Exception
			glob.nachricht_und_Mbox(ex.ToString)
		End Try
	End Sub

	Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
		gemeindechanged()
	End Sub

	Sub gemeindechanged()
		If cmbGemeinde.SelectedValue Is Nothing Then Exit Sub

		Dim item2 As String = (cmbGemeinde.SelectedValue).ToString
		glob.dtFuellen("select * from ohneMedien" & _
						" where Ort='" & item2 & "'" & _
						" order by Ort,Strasse,Hausnr")
		dgPlaene.DataContext = glob.StandortDBREC.dt
	End Sub

	Private Sub tbFilter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		MsgBox("baustelle")
	End Sub

	Private Sub btnAbbruch(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		Me.Close()
	End Sub

	Private Sub btnRefresh(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		refreshGrid()
	End Sub

	Sub initcombos()
		glob.dtFuellen("select distinct  Ort from adresse order by Ort asc")
		cmbGemeinde.DataContext = glob.StandortDBREC.dt
	End Sub
End Class
