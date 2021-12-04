Imports System.Data

Public Class VorgangUebersicht
    Property naturegListe As New List(Of clsNatureg)
    Property aktNatureg As New clsNatureg

        Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground() 
    End Sub

    Private Sub btnHinzufuegenNR_Click_1(sender As Object, e As RoutedEventArgs)
        Dim www As New NaturegDetail("neu", aktNatureg)
        www.ShowDialog()
        refreshNaturegListe()
        e.Handled = True
    End Sub

    Private Sub VorgangUebersicht_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        refreshNaturegListe()
          gastLayout()
        e.Handled = True
    End Sub

    Private Sub refreshNaturegListe()
        Dim naturegDT As New DataTable
        naturegDT = naturegOracle.getNaturegDatatable(myGlobalz.sitzung.aktVorgangsID)
        If naturegDT IsNot Nothing Then
            dgNatureg.DataContext = naturegDT
        Else
            dgNatureg.DataContext = Nothing
        End If
    End Sub

    Private Sub dgNatureg_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgNatureg.SelectionChanged
        Dim item As DataRowView
        Try
            item = CType(dgNatureg.SelectedItem, DataRowView)
            If item Is Nothing Then Return
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        Dim aktnatureg As New clsNatureg
        naturegTools.naturegitem2Obj(item, aktnatureg)
        Dim www As New NaturegDetail("edit", aktnatureg)
        www.ShowDialog()
        refreshNaturegListe()
        e.Handled = True
        dgNatureg.SelectedItem = Nothing
    End Sub





End Class
