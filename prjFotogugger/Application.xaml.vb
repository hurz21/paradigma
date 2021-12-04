Imports System.IO

Class Application
    'Public Shared Property zweiteInstanz As Boolean = False
    Public Shared Property givenVID As Integer = 0
    Public Shared Property givenVIDList As String = ""
    Public Shared Property givenEID As Integer = 0
    Public Shared Property givenDOCID As Integer = 0
    Public Shared Property nurverwandte As Integer = 0
    Public Shared anzahlInstanzen As Integer = 0
    'Public Shared instanzNummer As Short = 0
    Shared Property UserIsNoAdmin As Boolean = False
    ' Public Shared Property activationData As String()
    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.


    Private Sub Application_DispatcherUnhandledException(ByVal sender As Object, ByVal e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        '  MsgBox(String.Format("Allgemeiner Fehler!  {0}{1}", vbCrLf, e))
        MessageBox.Show(e.Exception.ToString, "Allgemeiner Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        e.Handled = True
    End Sub
    Public Shared Function Application_StartupExtracted() As Integer
        Dim anzahl As Integer = 0
        For Each p As System.Diagnostics.Process In Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName)
            If p.Id <> Process.GetCurrentProcess.Id Then
                Try
                    ' zweiteInstanz = True
                    anzahl += 1
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
            End If
            Return anzahl
        Next
        Return 0
    End Function


    Private Sub Application_Startup(ByVal sender As Object, ByVal e As System.Windows.StartupEventArgs) Handles Me.Startup
        Dim i As Integer = 0
        Do While i <> e.Args.Length
            If e.Args(i).Contains("/vid=") Then
                If e.Args(i).Contains(",") Then
                    givenVIDList = (e.Args(i).Replace("/vid=", "")).Trim
                    givenVID = 0
                Else
                    givenVID = CInt(e.Args(i).Replace("/vid=", ""))
                End If
            End If
            If e.Args(i).Contains("/eid=") Then
                givenEID = CInt(e.Args(i).Replace("/eid=", ""))
            End If
            If e.Args(i).Contains("/docid=") Then
                givenDOCID = CInt(e.Args(i).Replace("/docid=", ""))
            End If
            If e.Args(i).Contains("/nurverwandte=1") Then
                nurverwandte = 1 'CInt(e.Args(i).Replace("/nurverwandte=1", "")) 

            End If
            If e.Args(i).ToLower.Contains("/userisnoadmin") Then
                UserIsNoAdmin = True
            End If
            i += 1
        Loop
        'Threading.Thread.Sleep(1000) ' um ein grünes Paradigma zu vermeiden
        '  MsgBox(Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName).Count.ToString)
        'For Each p As System.Diagnostics.Process In Process.GetProcessesByName(Process.GetCurrentProcess.ProcessName)
        '    If p.Id <> Process.GetCurrentProcess.Id Then
        '        Try
        '            ' zweiteInstanz = True
        '            Application.anzahlInstanzen += 1
        '            ' End
        '        Catch ex As Exception
        '            MsgBox(ex.ToString)
        '        End Try
        '    End If
        'Next
        'If anzahlInstanzen = 2 Then
        '    MsgBox("Sie haben bereits zwei Instanzen von Paradigma geöffnet. Mehr geht nicht. Programm wird beendet!")
        '    End
        'End If 
    End Sub


End Class
