Public Class verwandteDokumente
    Public Property verwandteID As Integer

    Sub New(ByVal _verwandteID%)
        InitializeComponent()
        verwandteID = _verwandteID
    End Sub
    

    Private Sub verwandteDokumente_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If verwandteID < 1 Then
            Close()
        End If
        MsgBox("Dokumente von vid öffnen:" & verwandteID)
    End Sub

    Private Sub dgVorgangDokumente_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

    End Sub

    Private Sub gislink_click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
