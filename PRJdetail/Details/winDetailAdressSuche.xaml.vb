Imports System.Data

Public Class winDetailAdressSuche
    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemeinden.xml")
    End Sub
    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbGemeinde.SelectedItem Is Nothing Then Exit Sub
        gemeindechanged()
    End Sub
    Sub gemeindechanged()
        'Dim selob As New KeyValuePair(Of String, String)
        'selob = CType(cmbGemeinde.SelectedItem, KeyValuePair(Of String, String))
        'Dim selKey$ = selob.Key
        'Dim selvalue$ = selob.Value
        Dim myvali$ = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali) - 438000
        Dim buchstabe As String = ""
        'Dim item2 As DataRowView = CType(cmbGemeinde.SelectedItem, DataRowView)
        'Dim item3$ = item2.Row.ItemArray(0).ToString
        tbGemeinde.Text = myvals$ ' item2.Row.ItemArray(1).ToString
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali$) 'item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = tbGemeinde.Text
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = tbGemeinde.Text
        initStrassenCombo(buchstabe)
        myGlobalz.sitzung.aktADR.PLZ = (glob2.getPLZfromGemeinde(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName)) 'ddr
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PLZ = myGlobalz.sitzung.aktADR.PLZ
        cmbStrasse.IsDropDownOpen = True
    End Sub
    Sub initStrassenCombo(buchstabe As String)
        'gemeindeDT
        DB_Oracle_sharedfunctions.holeStrasseDTausHalo(buchstabe)
        cmbStrasse.DataContext = myGlobalz.sitzung.postgresREC.dt 
    End Sub

    Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Dim item2 As DataRowView = CType(cmbStrasse.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub

        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbStrasse.Text = item2.Row.ItemArray(1).ToString
        myGlobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.strasseName = tbStrasse.Text
        initHausNRCombo()
        cmbHausnr.IsDropDownOpen = True
    End Sub

    Sub initHausNRCombo()
        DB_Oracle_sharedfunctions.DBholeHausnrDT()
        cmbHausnr.DataContext = myGlobalz.sitzung.postgresREC.dt
    End Sub

    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Dim item2 As DataRowView = CType(cmbHausnr.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbHausnr.Text = item2.Row.ItemArray(1).ToString
        Dim halo_id As String = CStr(item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text
        glob2.hole_AdressKoordinaten(halo_id)
        If myGlobalz.sitzung.aktADR.punkt.X < 1 Or myGlobalz.sitzung.aktADR.punkt.Y < 1 Then
            MsgBox("Ein Fall für Google")
        End If

        CLstart.myc.kartengen.aktMap.aktrange.point2range(myGlobalz.sitzung.aktADR.punkt, 200)
        DialogResult = True
        e.Handled = True
        Me.Close()
        'lblFS.Text = myGlobalz.sitzung.aktADR.FS
        'lblCoords.Content = myGlobalz.sitzung.aktADR.punkt.X & " , " & myGlobalz.sitzung.aktADR.punkt.Y
        'btnWindrosen.IsEnabled = True
        'cmbFunktionsvorschlaege.IsDropDownOpen = True
    End Sub

    Private Sub winDetailAdressSuche_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initGemeindeCombo()
        gastLayout()
        cmbGemeinde.IsDropDownOpen=true
        e.Handled = True
    End Sub

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        DialogResult = False
        Me.Close()
    End Sub
End Class
