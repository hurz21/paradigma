Public Class winClipBoard
    Public grabpicture As System.Drawing.Image
    Public grabtext As String
    Private Sub btnTextUebernehmenClick(sender As Object, e As RoutedEventArgs)
        meinClipboard.getContentFromZwischenablage()
        Close()
        e.Handled = True
    End Sub

    Private Sub btnBildUebernehmenClick(sender As Object, e As RoutedEventArgs)
        meinClipboard.getContentFromZwischenablage()
        Close()
        e.Handled = True
    End Sub

    Private Sub winClipBoard_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not Clipboard.ContainsText Then
            btnTextUebernehmen.IsEnabled = False
            tbClip.Text = "--- Kein Text in Zwischenablage vorhanden ---"
        Else
            schowText()
        End If
        If Not Clipboard.ContainsImage Then
            btnBildUebernehmen.IsEnabled = False
        Else
            showbild()
        End If
    End Sub

    Private Sub showbild()
        grabpicture = meinClipboard.getPictureFromCB()
        Dim erfolg As Boolean
        If grabpicture IsNot Nothing Then
            Dim datei As String = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "Paradigma")
            datei = datei & "\myClip.png"
            erfolg = meinClipboard.BildTemporaerSpeichern(grabpicture, datei)
            Dim myBitmapImage As New BitmapImage()
            myBitmapImage.BeginInit()
            myBitmapImage.UriSource = New Uri(datei)
            myBitmapImage.EndInit()
            imgClip.Source = myBitmapImage
        End If
    End Sub

    Private Sub schowText()
        grabtext = meinClipboard.getTextFromCB()
        tbClip.Text = grabtext
    End Sub
End Class
