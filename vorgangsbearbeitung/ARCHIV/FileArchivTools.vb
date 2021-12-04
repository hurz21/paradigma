'Module FileArchivTools
'    'Public Function saveNEU(OriginalFullname As String,
'    '                                  ByRef archivDateiFullname As String,
'    '                                  ByRef erfolgreich As Boolean,
'    '                                   subdircount As String) As String
'    '    Dim myProcess As New Process()
'    '    Dim Exitcode As Integer
'    '    Dim aktion As String = ""
'    '    With myProcess.StartInfo
'    '        .FileName = "C:\a6\NEUPara\DMSserver\bin\Debug\dmsserver.exe "
'    '        .Arguments = " " & aktion & " " & OriginalFullname & " " & archivDateiFullname & " " & subdircount
'    '        .CreateNoWindow = True
'    '    End With
'    '    myProcess.Start()
'    '    myProcess.WaitForExit()
'    '    Exitcode = myProcess.ExitCode

'    '    If Exitcode > 0 And Not myProcess.HasExited Then
'    '        myProcess.Kill()
'    '        myProcess.Dispose()
'    '        erfolgreich = False
'    '    End If
'    '    If Exitcode = 0 Then
'    '        erfolgreich = True
'    '        Return "ok"
'    '    End If
'    '    If Exitcode = 1 Then
'    '        'datei existiert schon
'    '        Return "existiertschon"
'    '    End If
'    '    Return "undef"
'    'End Function



'    'Function buildSchlagworteAusDateiname(datei As String) As String
'    '    '_ zu blank wandeln
'    '    'endung abtrennen
'    '    Dim neuername As String
'    '    If String.IsNullOrEmpty(datei) Then Return ""
'    '    Try
'    '        Dim fi As New IO.FileInfo(datei)
'    '        neuername = fi.Name
'    '        neuername = neuername.Replace(fi.Extension, "")
'    '        fi = Nothing
'    '        neuername = neuername.Replace("_", " ")
'    '        neuername = neuername.Replace("/", " ")
'    '        neuername = neuername.Replace("\", " ")
'    '        neuername = neuername.Replace("'", " ")
'    '        neuername = neuername.Replace(":", " ")
'    '        neuername = neuername.Replace(".", " ")
'    '        neuername = neuername.Replace("?", " ")
'    '        neuername = neuername.Replace("!", " ")
'    '        Return neuername
'    '    Catch ex As Exception
'    '        nachricht("fehler in buildSchlagworteAusDateiname: " & ex.ToString)
'    '        Return ""
'    '    End Try
'    'End Function

'    'Function pruefeBeschreibung(beschreibung As String) As String
'    '    Dim neue As String
'    '    Try
'    '        If String.IsNullOrEmpty(beschreibung) Then Return ""
'    '        neue = beschreibung
'    '        neue = LIBgemeinsames.clsString.noWhiteSpace(neue, " ")
'    '        Return neue
'    '    Catch ex As Exception
'    '        nachricht("fehler in pruefeBeschreibung: " & ex.ToString)
'    '        Return ""
'    '    End Try
'    'End Function

'End Module
