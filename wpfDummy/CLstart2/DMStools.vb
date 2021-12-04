'Namespace CLstart
'    Public Class DMStools
'        Public Shared userdir, jobdefdir, responseDir As String
'        Public Shared _ExchangeRoot, _Bearbeiter_username, _DMSLaufWerkBuchstabe As String
'        Private Shared _aktVorgangsID As Integer
'        Shared Sub nachricht(text As String)
'            My.Log.WriteEntry(text)
'        End Sub
'        Public Shared Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
'            Dim anhang As String = ""
'            text = text & ToLogString(ex, text)
'            'myGlobalz.sitzung.nachrichtenText = text
'            My.Log.WriteEntry(text)
'            'mitFehlerMail(text, anhang)
'        End Sub
'        Shared Sub initDMStools(Bearbeiter_username As String, DMSLaufWerkBuchstabe As String, aktVorgangsID As Integer)
'            _Bearbeiter_username = Bearbeiter_username
'            _DMSLaufWerkBuchstabe = DMSLaufWerkBuchstabe
'            _aktVorgangsID = aktVorgangsID
'        End Sub
'        Shared Sub prepareExchangeDir()
'            Try
'                'If Not memoExists(myGlobalz.ExchangeRoot) Then                                creatememo(myGlobalz.ExchangeRoot)
'                'userdir = builduserdirName(myGlobalz.ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username)
'                'jobdefdir = buildjobdefdirName(myGlobalz.ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username, myGlobalz.sitzung.aktVorgangsID)
'                'responseDir = buildResponseDirName(myGlobalz.ExchangeRoot, myGlobalz.sitzung.Bearbeiter.username, myGlobalz.sitzung.aktVorgangsID)
'                userdir = builduserdirName(_ExchangeRoot, _Bearbeiter_username)
'                jobdefdir = buildjobdefdirName(_ExchangeRoot, _Bearbeiter_username, _aktVorgangsID)
'                responseDir = buildResponseDirName(_ExchangeRoot, _Bearbeiter_username, _aktVorgangsID)
'                buildDirectories(jobdefdir, responseDir)
'            Catch ex As Exception
'                nachricht("fehler in prepareExchangeDir: " ,ex)
'            End Try
'        End Sub

'        Shared Sub setArchivLaufWerkBuchstabe(text As String)
'            _DMSLaufWerkBuchstabe = text
'        End Sub

'        Shared Sub buildDMSexchangePath()
'            'der DMSLaufWerkBuchstabe wird hier missbraucht
'            ' das DMS ist bei service-System nicht sichtbar
'            _ExchangeRoot = IO.Path.Combine(_DMSLaufWerkBuchstabe, "paradigma", "div", "DMSexchange")
'        End Sub

'        Shared Function builduserdirName(Root As String, username As String) As String
'            Dim userdir As String = IO.Path.Combine(Root, username)
'            Return userdir
'        End Function

'        Shared Function buildjobdefdirName(Root As String, username As String, vid As Integer) As String
'            Dim userdir As String
'            If vid < 1 Then
'                userdir = IO.Path.Combine(Root, username, "jobdef")
'            Else
'                userdir = IO.Path.Combine(Root, username, vid.ToString, "jobdef")
'            End If
'            Return userdir
'        End Function

'        Shared Function buildResponseDirName(Root As String, username As String, vid As Integer) As String
'            Dim userdir As String
'            If vid < 1 Then
'                userdir = IO.Path.Combine(Root, username, "result")
'            Else
'                userdir = IO.Path.Combine(Root, username, vid.ToString, "result")
'            End If
'            Return userdir
'        End Function
'        Public Shared Function buildDirectories(jobdefdir As String, resultdir As String) As Boolean
'            nachricht("buildDirectories -------------------------")
'            Try
'                Dim lNewVariable1 As String = IO.Path.Combine(jobdefdir, "files") & "\"
'                Dim lNewVariable As String = IO.Path.Combine(resultdir, "files") & "\"
'                nachricht("lNewVariable1:" & lNewVariable1)
'                nachricht("lNewVariable:" & lNewVariable)

'                If Not IO.Directory.Exists(lNewVariable1) Then
'                    IO.Directory.CreateDirectory(lNewVariable1)
'                End If
'                If Not IO.Directory.Exists(lNewVariable) Then
'                    IO.Directory.CreateDirectory(lNewVariable)
'                End If
'                nachricht("buildDirectories ok")
'                Return True
'            Catch ex As Exception
'                nachricht("Fehler in buildDirectories: " ,ex)
'                Return False
'            End Try
'        End Function

'        'Private Shared Function memoExists(ExchangeRoot As String) As Boolean
'        '    Dim datei As String
'        '    datei = IO.Path.Combine(ExchangeRoot, _Bearbeiter_username, "memo.txt")
'        '    Try
'        '        Dim iiii As New IO.FileInfo(datei)
'        '        If iiii.Exists Then
'        '            iiii = Nothing
'        '            Return True
'        '        Else
'        '            iiii = Nothing
'        '            Return False
'        '        End If
'        '    Catch ex As Exception
'        '        nachricht("Fehler in memoExists:" ,ex)
'        '        Return False
'        '    End Try
'        'End Function

'        'Private Shared Sub creatememo(ExchangeRoot As String)
'        '    Dim datei As String
'        '    datei = IO.Path.Combine(ExchangeRoot, _Bearbeiter_username, "memo.txt")
'        '    Try
'        '        Dim iiii As New IO.FileInfo(datei)
'        '        iiii.Create()
'        '        iiii = Nothing

'        '    Catch ex As Exception
'        '        nachricht("Fehler in memoExists:" ,ex)

'        '    End Try
'        'End Sub

'    End Class
'End Namespace