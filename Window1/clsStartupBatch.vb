Public Class clsStartupBatch
    Public Shared Function CreateZielVerzeichnis$(ByVal rootDir$) ' myGlobalz.appdataDir
        Dim inifile$ = rootDir 'System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma"
        nachricht("inifile$: " & inifile$)
        If Not IO.Directory.Exists(inifile$) Then IO.Directory.CreateDirectory(inifile$)
        nachricht("CreateDirectory$: " & inifile$)
        If Not IO.Directory.Exists(inifile$ & "\config") Then IO.Directory.CreateDirectory(inifile$ & "\config")
        nachricht("CreateDirectory$: " & inifile$)
        If Not IO.Directory.Exists(inifile$ & "\config") Then IO.Directory.CreateDirectory(inifile$ & "\config\combos\")
        nachricht(String.Format("CreateDirectory$: {0}\config", inifile$))
        Return inifile
    End Function

    Private Shared Function setBatchFileName$()
        Dim batchFile$ = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Startup),
                                         "ParadigmaCopyConf.bat") '= XMLZielverzeichnis$ & batchdateiname
        Return batchFile
    End Function

    Private Shared Function BatchFileAnlegen(ByVal batchFile as string) as  Boolean
        Dim test As New IO.FileInfo(batchFile$)
        Dim firsttime As Boolean = True
        If test.Exists Then
            test.Delete()
            firsttime = False
        Else
            firsttime = True
        End If
        nachricht("inifile gelöscht$: ")
        If Not test.Exists Then
            writeBatchFile(batchFile$)
        End If
        Return firsttime
    End Function

    Private Shared Sub writeBatchFile(ByVal inifile$)
        Try
            nachricht("BatchFileanlegen : " & inifile$)
            Dim sr As New IO.StreamWriter(inifile)
            sr.WriteLine("rem Paradigma")

            Dim temp$ = String.Format("xcopy {0}*.* {1}{2}*.*{1}  /s /d /Y",
                                      myGlobalz.XMLserverConfigDir,
                                      Chr(34),
                                      myGlobalz.XMLclientConfigDir)
            sr.WriteLine(temp$)
            temp$ = String.Format("xcopy {0}\*.* {1}{2}*.*{1}  /s /d /Y",
                                      myGlobalz.XMLserverConfigDir,
                                      Chr(34),
                                      myGlobalz.XMLclientConfigDir)
            sr.WriteLine(temp$)
            '  sr.WriteLine("pause")
            sr.Flush()
            sr.Dispose()
            nachricht(temp$)
            nachricht("batchFileanlegen: erfolgreich")
        Catch ex As Exception
            nachricht_und_Mbox(String.Format("Fehler in BatchFileanlegen:{0}{1}", vbCrLf, ex))
        End Try
    End Sub

    Shared Function GenCopyConfBatch() As Boolean
        Try
            nachricht("GenCopyConfBatch: start-------------")
            Dim XMLZielverzeichnis$ = CreateZielVerzeichnis(myGlobalz.appdataDir)
            nachricht("XMLZielverzeichnis$ wurde angelegt: " & XMLZielverzeichnis$)

            Dim batchFile$ = setBatchFileName()
            nachricht("neu$: " & batchFile$)
            If BatchFileAnlegen(batchFile$) Then
                Return batchfileExecute()    'zum ersten mal
            End If

            nachricht("GenCopyConfBatch: end")
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in GenCopyConfBatch" & vbCrLf & ex.ToString)
        End Try
    End Function

    Shared Function batchfileExecute() As Boolean
        Try
            nachricht("batchfileExecute -----------------")
            Dim batchFile$ = setBatchFileName()
            nachricht("batchFile$$: " & batchFile$)
            Return BatchFileanausfuehren(batchFile$)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in batchfileExecute: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Public Shared Function BatchFileanausfuehren(ByVal inifile as string) as  Boolean
        Try
            nachricht("BatchFileanausfuehren " & inifile$)
            inifile = Chr(34) & inifile & Chr(34)
            If inifile.Contains("""") Then inifile = inifile.Replace("""", "")
            Dim testdatei As New IO.FileInfo(inifile)
            If testdatei.Exists Then
                nachricht("BatchFileanausfuehren Korrektur: " & inifile$)
                Microsoft.VisualBasic.Shell(inifile, AppWinStyle.MinimizedFocus)
                Return True
            Else
                nachricht("Fehler BatchFileanausfuehren Korrektur nicht möglich. datei fehlt: " & inifile$)
                Return False
            End If
            nachricht("nach shell ")
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in BatchFileanausfuehren:" & vbCrLf & ex.ToString & vbCrLf & inifile)
            Return False
        End Try
    End Function
End Class
