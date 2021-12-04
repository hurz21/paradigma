Imports System.Data
Public Class winUserList
    Private Sub winUserList_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        userTools.initbearbeiterDT()
        DataGrid1.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
        e.Handled = True
    End Sub

    Private Sub DataGrid1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DataGrid1.SelectionChanged
        Dim item As DataRowView
        Try
            item = CType(DataGrid1.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        item = CType(DataGrid1.SelectedItem, DataRowView)
        'Dim item = dgStamm.SelectedItem
        If item Is Nothing Then
            item = CType(DataGrid1.SelectedItem, DataRowView)
            If item Is Nothing Then Return
        End If

        Dim auswahlid$ = item("BEARBEITERID").ToString()
        Dim beschreibung$ = item("USERNAME").ToString()
        Dim testbearbeiter As New clsBearbeiter
        If userTools.initBearbeiterByUserid_ausParadigmadb(testbearbeiter, "BEARBEITERID", auswahlid) Then
            Dim edit As New winUserDetail(testbearbeiter)
            edit.ShowDialog()
            userTools.initbearbeiterDT()
        Else
            MsgBox("not found")
        End If
        DataGrid1.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub btnNeuerUserAnlegen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        '  Dim testbearbeiter As New clsBearbeiter
        Dim edit As New winUserDetail()
        edit.ShowDialog()
        userTools.initbearbeiterDT()
        e.Handled = True
    End Sub

    Private Sub btnabbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        userTools.initbearbeiterDT()
        DataGrid1.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
        e.Handled = True
    End Sub
End Class
