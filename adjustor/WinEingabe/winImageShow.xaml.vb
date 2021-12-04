Public Class winImageShow
    Property pic As String
    Sub New(ByVal _pic As String) 
        InitializeComponent() 
        pic = _pic
    End Sub

    Private Sub winImageShow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Dim myBitmapImage As New BitmapImage()
        myBitmapImage.BeginInit()
        myBitmapImage.UriSource = New Uri(pic)
        myBitmapImage.EndInit()
        Dim brusch As New ImageBrush
        brusch.ImageSource = myBitmapImage
        Background = brusch
        brusch = Nothing
        e.Handled = True
    End Sub

    Private Sub abbrruch(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub
End Class
