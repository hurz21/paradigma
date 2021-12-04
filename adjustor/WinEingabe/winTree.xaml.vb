Public Class winTree
    Property dataPfad As String
    Property xmlfile As String
    Property custViewModel As VariablenViewModel
    Sub New(ByVal _datapfad As String, ByVal _xmlfile As String)
        InitializeComponent()
        dataPfad = _datapfad
        xmlfile = _xmlfile
    End Sub

    Private Sub winTree_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Application.fullpath = IO.Path.Combine(dataPfad, xmlfile)
        custViewModel = New VariablenViewModel
        Me.DataContext = custViewModel
        Title = "Verändern von: " & Application.fullpath
    End Sub

    Private Sub AbbruchButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub EditorButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Process.Start("notepad.exe ", Application.fullpath)
        e.Handled = True
    End Sub

    Private Sub pspadButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Process.Start("C:\Program Files (x86)\PSPad editor\PSPad.exe", Application.fullpath)
        e.Handled = True
    End Sub
End Class
