'Module jfHilfe
'Sub starthelp(ByVal form_position As String)
'    Dim link As String = getLinkFromFile(form_position)
'    If link = "" Then
'        MsgBox("Hilfe ist im Moment nicht erreichbar. Bitte Neustart (Paradigma) und nochmal probieren.")
'    Else
'        Process.Start(link) '"#reiterraumbezug")
'    End If
'End Sub

'Private Function lesedateiHolestring() As String
'    Dim datei As String = myGlobalz.XMLclientConfigDir & "hilfe.txt"
'    'readroot
'    Dim root As String
'    Using sr As New IO.StreamReader(datei)
'        root = sr.ReadToEnd
'    End Using
'    Return root
'End Function

'Private Function getLinkRoot(ByRef recs As String()) As String
'    Dim root As String
'    Try
'        'If recs.IsNothingOrEmpty() Then
'        '    Return "Keine Root Url gefunden"
'        'End If
'        root = recs(0)
'        Return root
'    Catch ex As Exception
'        nachricht("Keine Root Url gefunden, Fehler in getLinkRoot " & ex.ToString)
'        Return "Keine Root Url gefunden"
'    End Try
'End Function

'Private Function getLinkFromFile(ByVal form_position As String) As String
'    Dim datei As New IO.FileInfo(myGlobalz.XMLclientConfigDir & "hilfe.txt")
'    If Not datei.Exists Then Return ""
'    Dim recs As String()
'    Dim root As String = lesedateiHolestring()
'    recs = root.Split(CChar(vbCrLf))
'    root = getLinkRoot(recs)
'    Dim anker As String = sucheAnker(form_position, recs)
'    If anker = "" Then Return ""
'    Dim link As String = glue(root, anker)
'    Return link
'End Function

'Private Function glue(ByVal root As String, ByVal anker As String) As String
'    'gluelink
'    Dim link As String = root & "#" & anker
'    Return link
'End Function

'Private Function sucheAnker(ByVal form_position As String, ByVal recs As String()) As String
'    form_position = form_position.ToLower
'    Dim teile As String()
'    recs(0) = "" 'damit es keine missverständnisse gibt
'    For Each zeile In recs
'        zeile = zeile.ToLower.Trim
'        If String.IsNullOrEmpty(zeile) OrElse zeile.StartsWith("#") Then Continue For
'        teile = zeile.Split("#"c)
'        If teile(0) = form_position Then
'            'treffer
'            Return teile(1)
'        End If
'    Next
'    Return ""
'End Function
'End Module
