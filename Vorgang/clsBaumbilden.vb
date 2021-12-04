Public Class clsBaumbilden
    Private Shared sgsumme As String, tiefetag(20) As String
    Private Shared olddepth As Integer
    Private Shared itemsArray(10) As TreeViewItem
    Shared Sub ladeXML(ByVal datei As String, ByVal tv As System.Windows.Controls.TreeView)
        olddepth = 0
        Dim test As New IO.FileInfo(datei)
        If test.Exists Then
            XMLiniReader(datei, tv)
        Else
            nachricht_und_Mbox("Fataler Fehler: Die Sachgebietsdatei kann nicht gefunden werden! Bitte starten Sie Paradigma neu. Wenn das Problem nochmal auftritt informieren Sie den Admin!")
            Exit Sub
        End If
    End Sub

    Public Shared Function XMLiniReader(ByVal xml_inifile_fullpath As String,
                                        ByVal tv As System.Windows.Controls.TreeView) As Dictionary(Of String, String)
        Dim iniDict As New Dictionary(Of String, String)
        Dim header As String = "", tabsumme As String = "", tiefe As Integer = 0, meintipp As String = ""
        Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath$)
            Do While XMLReader.Read ' Es sind noch Daten vorhanden          
                Select Case XMLReader.NodeType
                    Case XmlNodeType.Element
                        If XMLReader.AttributeCount > 0 Then
                            While XMLReader.MoveToNextAttribute ' nächstes 
                                If XMLReader.Name = "Tag" Then
                                    tiefetag(CInt(XMLReader.Depth)) = XMLReader.Value
                                    sgsumme$ = addtags(CInt(XMLReader.Depth))
                                End If
                                If XMLReader.Name = "Header" Then
                                    header$ = XMLReader.Value
                                End If
                                If XMLReader.Name = "Alt" Then
                                    meintipp$ = XMLReader.Value
                                End If
                            End While
                            For i = 1 To CInt(XMLReader.Depth)
                                tabsumme &= vbTab
                            Next
                            If String.IsNullOrEmpty(sgsumme) Then sgsumme = ""
                            Dim newChild As TreeViewItem = New TreeViewItem
                            If String.IsNullOrEmpty(sgsumme.Trim) Then
                                newChild.Header = CStr(sgsumme).Trim
                                'newChild.Header = String.Format("{1}", CStr(sgsumme).Trim)
                                'newChild.Header = String.Format("{1}", CStr(sgsumme).Trim, header)
                            Else
                                newChild.Header = String.Format("{0}-{1}", CStr(sgsumme).Trim, header)
                            End If

                            newChild.Tag = sgsumme
                            If Not String.IsNullOrEmpty(meintipp$) Then
                                newChild.ToolTip = "Alte Nr.: " & meintipp$
                            End If

                            tiefe% = XMLReader.Depth

                            If itemsArray(XMLReader.Depth - 1) Is Nothing Then
                                tv.Items.Add(newChild)
                                itemsArray(XMLReader.Depth) = newChild
                            Else
                                itemsArray(XMLReader.Depth - 1).Items.Add(newChild)
                                itemsArray(XMLReader.Depth) = newChild
                            End If
                            If XMLReader.Depth = olddepth Then
                                itemsArray(XMLReader.Depth - 2) = newChild
                            End If
                            If XMLReader.Depth < olddepth Then
                                itemsArray(XMLReader.Depth - 3) = newChild
                            End If
                            tabsumme = ""
                            meintipp$ = ""
                        End If
                        ' Ein Text 
                    Case XmlNodeType.Text
                        '  Console.WriteLine("Es folgt ein Text: " & .Value)
                        ' Ein Kommentar 
                    Case XmlNodeType.Comment
                        ' Console.WriteLine("Es folgt ein Kommentar: " & .Value)
                End Select
                tabsumme = ""
                meintipp$ = ""
            Loop
            ' Weiter nach Daten schauen 
'XMLReader.Close()
            ' XMLTextReader schließen 
        End Using
        Return iniDict
    End Function


    Shared Function addtags(ByVal tiefe As Integer) As String
        Dim summe As String = ""
        For i = 1 To tiefe
            summe = summe & tiefetag(i)
        Next
        Return summe
    End Function

    Public Shared Function sucheStichwortInXML(ByVal xml_inifile_fullpath As String, _
                                                ByVal stichwort As String, _
                                                ByVal trefferliste As System.Windows.Controls.ListBox) As Dictionary(Of String, String)
        Dim iniDict As New Dictionary(Of String, String)
        Dim header As String = "", tabsumme As String = ""
        Dim info As String = ""
        Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath)
            Do While XMLReader.Read ' Es sind noch Daten vorhanden          
                Select Case XMLReader.NodeType
                    Case XmlNodeType.Element
                        If XMLReader.AttributeCount > 0 Then
                            While XMLReader.MoveToNextAttribute ' nächstes 
                                If XMLReader.Name = "Tag" Then
                                    tiefetag(CInt(XMLReader.Depth)) = XMLReader.Value
                                    sgsumme = addtags(CInt(XMLReader.Depth))
                                End If
                                If XMLReader.Name = "Header" Then
                                    header = XMLReader.Value
                                End If
                                If XMLReader.Name = "Info" Then
                                    info = XMLReader.Value
                                End If
                            End While
                            For i = 1 To CInt(XMLReader.Depth)
                                tabsumme &= vbTab
                            Next
                            If header.ToLower.Contains(stichwort) Or _
                               info.ToLower.Contains(stichwort) Then
                                trefferliste.Items.Add(sgsumme & "-" & header)
                            End If
                            tabsumme = ""
                        End If
                        ' Ein Text 
                    Case XmlNodeType.Text
                        'Console.WriteLine("Es folgt ein Text: " & .Value)
                        ' Ein Kommentar 
                    Case XmlNodeType.Comment
                        'Console.WriteLine("Es folgt ein Kommentar: " & .Value)
                End Select
            Loop
            ' Weiter nach Daten schauen 
'XMLReader.Close()
            ' XMLTextReader schließen 
        End Using
        Return iniDict
    End Function

    Public Shared Function GetRumpf() As String
        Dim rumpf$ = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Replace(myGlobalz.sitzung.aktVorgang.Stammdaten.az.stamm, "")
        Dim sgrtest$ = "-" & myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl & "-"
        Dim sinitial$ = "-" & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale
        rumpf$ = rumpf$.Replace(sgrtest$, "")
        rumpf$ = rumpf$.Replace(sinitial, "")
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa) Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa = rumpf
        End If
        Return rumpf
    End Function

    Public Shared Function AZistNeu() As Boolean
    
        Dim azOhneUser3$ = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Trim
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from stammdaten " & " where az2 = '" & azOhneUser3$ & "'"
        nachricht("hinweis: " & myGlobalz.sitzung.VorgangREC.getDataDT())
        Return If(myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty, True, False)
    End Function

    Public Shared Function sucheSGNRInXML_erweitert(ByVal xml_inifile_fullpath As String, _
                                            ByVal SGNR As String) As String
        Dim header As String = "", tabsumme As String = ""
        Dim info As String = ""
        Dim ebene2Text As String=""
        Dim a() As Char
        a = SGNR.ToCharArray()
        Dim eins, zwei As String
        eins = a(0).ToString
        zwei = a(1).ToString
        Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath)
            Do While XMLReader.Read ' Es sind noch Daten vorhanden          
                Select Case XMLReader.NodeType
                    Case XmlNodeType.Element

                        If XMLReader.AttributeCount > 0 Then
                            While XMLReader.MoveToNextAttribute ' nächstes 
                                If XMLReader.Name = "Tag" Then
                                    tiefetag(CInt(XMLReader.Depth)) = XMLReader.Value
                                    sgsumme = addtags(CInt(XMLReader.Depth))
                                End If
                                If XMLReader.Name = "Header" Then
                                    header = XMLReader.Value

                                End If
                                If XMLReader.Name = "Info" Then
                                    info = XMLReader.Value
                                End If
                            End While
                            For i = 1 To CInt(XMLReader.Depth)
                                tabsumme &= vbTab
                            Next
                            Try
                                If tiefetag(2) = eins And tiefetag(3).StartsWith(zwei) And ebene2Text.IsNothingOrEmpty Then
                                    ebene2Text = header
                                End If
                            Catch ex As Exception

                            End Try

                            If sgsumme = SGNR Then
                                If header.Trim <> ebene2Text.Trim Then
                                    Return ebene2Text & "-" & header
                                Else
                                    Return ebene2Text
                                End If
                            End If
                            'If header.ToLower.Contains(SGNR) Or _
                            ' info$.ToLower.Contains(SGNR) Then
                            '    '   trefferliste.Items.Add(sgsumme & "-" & header)
                            'End If

                            tabsumme = ""
                        End If
                        ' Ein Text 
                    Case XmlNodeType.Text
                        'Console.WriteLine("Es folgt ein Text: " & .Value)
                        ' Ein Kommentar 
                    Case XmlNodeType.Comment
                        'Console.WriteLine("Es folgt ein Kommentar: " & .Value)
                End Select
            Loop
            ' Weiter nach Daten schauen 
'XMLReader.Close()
            ' XMLTextReader schließen 
        End Using
        Return ""
    End Function
    Public Shared Function sucheSGNRInXML(ByVal xml_inifile_fullpath As String, _
                                         ByVal SGNR As String,
                                         ByVal sgtext As String) As String
        Dim header As String = "", tabsumme As String = "", h2 As String = ""
        Dim info As String = ""
        Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath)
            Do While XMLReader.Read ' Es sind noch Daten vorhanden          
                Select Case XMLReader.NodeType
                    Case XmlNodeType.Element

                        If XMLReader.AttributeCount > 0 Then
                            While XMLReader.MoveToNextAttribute ' nächstes 
                                If XMLReader.Name = "Tag" Then
                                    tiefetag(CInt(XMLReader.Depth)) = XMLReader.Value
                                    sgsumme = addtags(CInt(XMLReader.Depth))
                                End If
                                If XMLReader.Name = "Header" Then
                                    header = XMLReader.Value
                                    h2 = header
                                End If
                                If XMLReader.Name = "Info" Then
                                    info = XMLReader.Value
                                End If
                            End While
                            For i = 1 To CInt(XMLReader.Depth)
                                tabsumme &= vbTab
                            Next
                            If sgsumme = SGNR Then
                                Return header
                            End If
                            'If header.ToLower.Contains(SGNR) Or _
                            ' info$.ToLower.Contains(SGNR) Then
                            '    '   trefferliste.Items.Add(sgsumme & "-" & header)
                            'End If

                            tabsumme = ""
                        End If
                        ' Ein Text 
                    Case XmlNodeType.Text
                        'Console.WriteLine("Es folgt ein Text: " & .Value)
                        ' Ein Kommentar 
                    Case XmlNodeType.Comment
                        'Console.WriteLine("Es folgt ein Kommentar: " & .Value)
                End Select
            Loop
            ' Weiter nach Daten schauen 
            'XMLReader.Close()
            ' XMLTextReader schließen 
        End Using
        Return ""
    End Function

End Class
