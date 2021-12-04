Imports System.ComponentModel
Imports System.Data

Public Class winFremdRBs
    Private ladevorgangAbgeschlossen As Boolean = False
    Private gemkrz As String = ""
    Private textfilter As String = ""
    Private geometrieTyp As String = "3"
    Private topString As String = "100"
    Private bearbeiter As String = ""
    Sub New()
        InitializeComponent()
    End Sub

    Private Sub winFremdRBs_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        refreshPolygonListe()
        glob2.initGemKRZCombo(Me)
        ladevorgangAbgeschlossen = True
    End Sub

    Private Sub refreshPolygonListe()
        myGlobalz.sitzung.VorgangREC.mydb.SQL = fremdRBtools.genFremdRB(gemkrz, tbFilter.Text, geometrieTyp, topString, bearbeiter) '
        Dim hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
        dgRaumbezug.DataContext = myGlobalz.sitzung.VorgangREC.dt
        If myGlobalz.sitzung.VorgangREC.dt.Rows.Count = 100 Then
            tbAnzTreffer.Text = myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString & "++"
        Else
            tbAnzTreffer.Text = myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString
        End If

    End Sub

    Private Sub winFremdRBs_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub dgRaumbezug_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If dgRaumbezug.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgRaumbezug.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        fremdRBtools.handleChosenRB(item)
        dgRaumbezug.SelectedItem = Nothing
    End Sub

    Private Sub cmbGemKRZ_MouseMove(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub cmbGemKRZ_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        If cmbGemKRZ.SelectedItem Is Nothing Then Exit Sub
        gemkrz = CType(cmbGemKRZ.SelectedValue, String)

        tbGEMKRZ.Text = gemkrz
        'myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = item.Trim
        'tbGEMKRZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ
        '  detailsTools.Edit_singleUpdate_Stammdaten(Now, "GEMKRZ")
        'refreshPolygonListe()
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        refreshPolygonListe()
    End Sub

    Private Sub CmbTyp_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        Dim res As ComboBoxItem = CType(cmbTyp.SelectedItem, ComboBoxItem)
        geometrieTyp = res.Tag.ToString
    End Sub

    Private Sub BtnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
    End Sub
End Class
