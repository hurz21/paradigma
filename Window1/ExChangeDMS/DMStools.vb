Module DMStools
    Public userdir, jobdefdir, responseDir As String
    Sub prepareExchangeDir()
        Try
            'If Not memoExists(myGlobalz.ExchangeRoot) Then                                creatememo(myGlobalz.ExchangeRoot)
            userdir = builduserdirName(myGlobalz.ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username)
            jobdefdir = buildjobdefdirName(myGlobalz.ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username, myGlobalz.sitzung.aktVorgangsID)
            responseDir = buildResponseDirName(myGlobalz.ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username, myGlobalz.sitzung.aktVorgangsID)
            buildDirectories(jobdefdir, responseDir)
        Catch ex As Exception
            nachricht("fehler in prepareExchangeDir: " & ex.ToString)
        End Try
    End Sub

    Sub setArchivLaufWerkBuchstabe(text As String)
        myGlobalz.DMSLaufWerkBuchstabe = text
    End Sub 

    Sub buildDMSexchangePath()
        'der DMSLaufWerkBuchstabe wird hier missbraucht
        ' das DMS ist bei service-System nicht sichtbar
        myGlobalz.ExchangeRoot = IO.Path.Combine(myGlobalz.DMSLaufWerkBuchstabe, "paradigma", "div", "DMSexchange") 
    End Sub

    Function builduserdirName(Root As String, username As String) As String
        Dim userdir As String = IO.Path.Combine(Root & username)
        Return userdir
    End Function

    Function buildjobdefdirName(Root As String, username As String, vid As Integer) As String
        Dim userdir As String
        If vid < 1 Then
            userdir = IO.Path.Combine(Root, username, "jobdef")
        Else
            userdir = IO.Path.Combine(Root, username, vid.ToString, "jobdef")
        End If
        Return userdir
    End Function

    Function buildResponseDirName(Root As String, username As String, vid As Integer) As String
        Dim userdir As String
        If vid < 1 Then
            userdir = IO.Path.Combine(Root, username, "result")
        Else
            userdir = IO.Path.Combine(Root, username, vid.ToString, "result")
        End If
        Return userdir
    End Function
    Public Function buildDirectories(jobdefdir As String, resultdir As String) As Boolean
        nachricht("buildDirectories -------------------------")
        Try
            Dim lNewVariable1 As String = IO.Path.Combine(jobdefdir, "files") & "\"
            Dim lNewVariable As String = IO.Path.Combine(resultdir, "files") & "\"
            nachricht("lNewVariable1:" & lNewVariable1)
            nachricht("lNewVariable:" & lNewVariable)

            If Not IO.Directory.Exists(lNewVariable1) Then
                IO.Directory.CreateDirectory(lNewVariable1)
            End If
            If Not IO.Directory.Exists(lNewVariable) Then
                IO.Directory.CreateDirectory(lNewVariable)
            End If
            nachricht("buildDirectories ok")
            Return True
        Catch ex As Exception
            nachricht("Fehler in buildDirectories: " & ex.ToString)
            Return False
        End Try
    End Function

    Private Function memoExists(ExchangeRoot As String) As Boolean
        Dim datei As String
        datei = IO.Path.Combine(ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username, "memo.txt")
        Try
            Dim iiii As New IO.FileInfo(datei)
            If iiii.Exists Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler in memoExists:" & ex.ToString)
            Return False
        End Try
    End Function

    Private Sub creatememo(ExchangeRoot As String)
        Dim datei As String
        datei = IO.Path.Combine(ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username, "memo.txt")
        Try
            Dim iiii As New IO.FileInfo(datei)
            iiii.Create()


        Catch ex As Exception
            nachricht("Fehler in memoExists:" & ex.ToString)

        End Try
    End Sub

End Module
