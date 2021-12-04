Imports System.Data

Public Class win_DSGruppenauswahl
    Public _bearbeiterid As Integer
    Public Property auswahlid As String
    Public Property beschreibung As String
    Public Property gruppenName As String
    Public _modus As String

    Sub New(bearbeiterid As Integer, modus As String)
        InitializeComponent()
        _bearbeiterid = bearbeiterid
        _modus = modus
    End Sub

    Private Sub win_DSGruppenauswahl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        userTools.getGruppeDatatable(myGlobalz.sitzung.VorgangREC, _bearbeiterid, _modus)
        dgGruppenDS.DataContext = myGlobalz.sitzung.VorgangREC.dt
    End Sub

    Private Sub dgGruppenDS_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgGruppenDS.SelectionChanged
        Dim item As DataRowView
        Try
            item = CType(dgGruppenDS.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        item = CType(dgGruppenDS.SelectedItem, DataRowView)
        'Dim item = dgStamm.SelectedItem
        If item Is Nothing Then
            item = CType(dgGruppenDS.SelectedItem, DataRowView)
            If item Is Nothing Then Return
        End If

        auswahlid = item("GRUPPENID").ToString()
        beschreibung = item("BESCHREIBUNG").ToString()
        gruppenName = item("NAME").ToString()

        Close()
        e.Handled = True
    End Sub

    'Private Sub dgGruppenDS_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

    'End Sub
End Class
