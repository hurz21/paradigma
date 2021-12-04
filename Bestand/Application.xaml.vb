Class Application
    ''Public Shared Property zweiteInstanz As Boolean = False
    'Public Shared Property givenVID As Integer = 0
    'Shared Property UserIsNoAdmin As Boolean = False
    '' Public Shared Property activationData As String()
    '' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    '' können in dieser Datei verarbeitet werden.


    'Private Sub Application_DispatcherUnhandledException(ByVal sender As Object, ByVal e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
    '    '  MsgBox(String.Format("Allgemeiner Fehler!  {0}{1}", vbCrLf, e))
    '    MessageBox.Show(e.Exception.Message, "Exception Caught", MessageBoxButton.OK, MessageBoxImage.Error)
    '    e.Handled = True
    'End Sub
    ''Dim mg As New mg	

    'Private Sub Application_Startup(ByVal sender As Object, ByVal e As System.Windows.StartupEventArgs) Handles Me.Startup
    '    'Get the ActivationArguments from the SetupInformation property of the domain.
    '    ' activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData
    '    'MsgBox("Application_Startup")
    '    ' Application is running
    '    ' Process command line args
    '    'Dim startMinimized As Boolean = False
    '    'Dim i As Integer = 0
    '    'Do While i <> e.Args.Length
    '    '    If e.Args(i).StartsWith("/vid=") Then
    '    '        givenVID = CInt(e.Args(i).Replace("/vid=", ""))
    '    '    Else
    '    '        ' MsgBox(e.Args(i))
    '    '    End If
    '    '    If e.Args(i).ToLower.Contains("/userisnoadmin") Then
    '    '        UserIsNoAdmin = True
    '    '    End If
    '    '    i += 1
    '    'Loop


    '    'For Each p As System.Diagnostics.Process In Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName)
    '    '    If p.Id <> Process.GetCurrentProcess.Id Then
    '    '        Try
    '    '            AppActivate(p.Id)
    '    '            'MsgBox(String.Format("Eine Paradigma-Instanz läuft schon !!! Gastmodus! {0} Sie sollten " & vbCrLf &
    '    '            '                     "- diese Paradigma Instanz schnell wieder verlassen und " & vbCrLf &
    '    '            '                     "- keine Daten oder Dokumente ändern. ", vbCrLf), , "Paradigma läuft schon !!!")
    '    '            'zweiteInstanz = True
    '    '            MsgBox("Sie können diese Instanz nur einmal starten! Programm wird beendet!")
    '    '            end
    '    '        Catch ex As Exception
    '    '            MsgBox(ex.ToString)
    '    '        End Try
    '    '    End If
    '    'Next
    '    'test zweiteInstanz = True
    'End Sub

End Class
