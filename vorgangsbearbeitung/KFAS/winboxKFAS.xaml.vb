Public Class winboxKFAS
    Public Property action As String = ""
    Sub New(info As String)
        InitializeComponent()
        _info = info
    End Sub

    Dim _info As String = ""
    Private Sub btnVorgang_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        action = "zumvorgang"
        Close()
    End Sub

    Private Sub btndelete_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        action = "dateiloeschen"
        Close()
    End Sub

    Private Sub btnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
    End Sub

    Private Sub winboxKFAS_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        tbinfo.Text = _info
    End Sub
End Class
