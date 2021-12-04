Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.


    Public Sub New()
        InitializeComponent()
        'MsgBox("sd " & System.Environment.CommandLine.ToString)  
        'For Each gfg As String In Environment.GetCommandLineArgs() 
        '	MsgBox("args : " & gfg)		 
        'Next
    End Sub
End Class
