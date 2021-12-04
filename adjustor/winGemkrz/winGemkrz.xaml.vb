Public Class winGemkrz

    Property dataPfad As String
    Property xmlfile As String
    Property custViewModel As GEMKRZenenViewModel
    Sub New(ByVal _datapfad As String, ByVal _xmlfile As String)
        InitializeComponent()
        dataPfad = _datapfad
        xmlfile = _xmlfile
    End Sub

    Private Sub winNameId_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim mesres As New MessageBoxResult
        If Not custViewModel.anychange Then
            e.Cancel = False
        Else
            mesres = MessageBox.Show("Ein Dokument wurde geändert. Die Änderungen werden verloren gehen, wenn Sie nicht zuerst das Dokument speichern und schliessen." &
                            "" & vbCrLf & vbCrLf &
                            "Möchten Sie die Änderungen am Dokument verwerfen ? " & vbCrLf &
                            "Ja   -  Änderungen verwerfen" & vbCrLf & vbCrLf &
                            "Nein - Im Vorgang bleiben",
                            "Dokument wurde geändert", MessageBoxButton.YesNo, MessageBoxImage.Error)
            If mesres = MessageBoxResult.Yes Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Application.fullpath = IO.Path.Combine(dataPfad, xmlfile)
        custViewModel = New GEMKRZenenViewModel
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
        Try
            Process.Start("C:\Program Files (x86)\PSPad editor\PSPad.exe", Application.fullpath)
        Catch ex As Exception
            MsgBox("Fehler : " & ex.ToString)
        End Try
        e.Handled = True
    End Sub

    Private Sub easyXML_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            Process.Start("C:\Program Files (x86)\Easy XML Editor\easyxmleditor.exe", Application.fullpath)
        Catch ex As Exception
            MsgBox("Fehler : " & ex.ToString)
        End Try
        e.Handled = True
    End Sub
End Class

