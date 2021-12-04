Imports System.Xml
Namespace CLstart
    Public Class clsINIXML
        Shared Sub nachricht(text As String)
            My.Log.WriteEntry(text)
        End Sub
        Public Shared Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
            Dim anhang As String = ""
            text = text & ToLogString(ex, text)
            'myGlobalz.sitzung.nachrichtenText = text
            My.Log.WriteEntry(text)
            'mitFehlerMail(text, anhang)
        End Sub
        Public iniDict As Dictionary(Of String, String)
        Public Shared Function XMLiniReader2(ByVal xml_inifile_fullpath As String) As Dictionary(Of String, String)
            'beisüpielaufruf
            '1. Dim iniDict As Dictionary(Of String, String) = clsXML_ini.XMLiniReader(meininifile$)
            '2. Dim iminternet As Boolean = CType(iniDict("ServerSpezifisch.iminternet"), Boolean)
            'Dim entry As KeyValuePair(Of String, String)
            'Console.WriteLine("==============================")
            'For Each entry In iniDict
            '  Console.WriteLine(entry.Key & "=" & entry.Value)
            'Next
            nachricht("1in XmlTextReader------------------------")
            Dim iniDict As New Dictionary(Of String, String)
            '		Dim _xlsminifile$
            Dim gruppe, tag, wert As String
            ' Wir benötigen einen XmlReader für das Auslesen der XML-Datei 
            nachricht("333 Vor XmlTextReader" & xml_inifile_fullpath)
            Try
                Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath)
                    nachricht("333 nach XmlTextReader" & xml_inifile_fullpath)
                    ' Es folgt das Auslesen der XML-Datei 
                    'Try
                    '    XMLReader.Read()

                    'Catch ex As Exception

                    'End Try
                    Do While XMLReader.Read  ' Es sind noch Daten vorhanden 
                        ' Welche Art von Daten liegt an? 
                        Select Case XMLReader.NodeType
                        ' Ein Element 
                            Case XmlNodeType.Element
                                gruppe = XMLReader.Name
                                ' Alle Attribute (Name-Wert-Paare) abarbeiten 
                                If XMLReader.AttributeCount > 0 Then
                                    ' Es sind noch weitere Attribute vorhanden 
                                    While XMLReader.MoveToNextAttribute ' nächstes 
                                        'iniDict.Add(String.Format("{0}.{1}", gruppe, XMLReader.Name), XMLReader.Value)
                                    End While
                                End If
                                wert = XMLReader.ReadInnerXml
                                iniDict.Add(String.Format("{0}", gruppe), wert)
                            Case XmlNodeType.Text
                                gruppe = XMLReader.Name
                                ' Alle Attribute (Name-Wert-Paare) abarbeiten 
                                If XMLReader.AttributeCount > 0 Then
                                    ' Es sind noch weitere Attribute vorhanden 
                                    While XMLReader.MoveToNextAttribute ' nächstes 
                                        iniDict.Add(String.Format("{0}", XMLReader.Name), XMLReader.Value)
                                    End While
                                End If
                            Case XmlNodeType.EndElement
                                If XMLReader.AttributeCount > 0 Then
                                    ' Es sind noch weitere Attribute vorhanden 
                                    While XMLReader.MoveToNextAttribute ' nächstes 
                                        iniDict.Add(String.Format("{0}.{1}", gruppe, XMLReader.Name), XMLReader.Value)
                                    End While
                                End If
                            Case XmlNodeType.Comment
                        End Select
                    Loop
                End Using
                nachricht("vor dem return in xmlinireader")
                Return iniDict
            Catch ex As Exception
                nachricht("fehler in xmlinireader:  " ,ex)
                Return Nothing
            End Try
        End Function
        Public Shared Function XMLiniReader(ByVal xml_inifile_fullpath As String) As Dictionary(Of String, String)
            'beisüpielaufruf
            '1. Dim iniDict As Dictionary(Of String, String) = clsXML_ini.XMLiniReader(meininifile$)
            '2. Dim iminternet As Boolean = CType(iniDict("ServerSpezifisch.iminternet"), Boolean)
            'Dim entry As KeyValuePair(Of String, String)
            'Console.WriteLine("==============================")
            'For Each entry In iniDict
            '  Console.WriteLine(entry.Key & "=" & entry.Value)
            'Next
            nachricht("1in XmlTextReader------------------------")
            Dim iniDict As New Dictionary(Of String, String)
            '		Dim _xlsminifile$
            Dim gruppe As String
            ' Wir benötigen einen XmlReader für das Auslesen der XML-Datei 
            nachricht("333 Vor XmlTextReader" & xml_inifile_fullpath)
            Try
                Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath)
                    nachricht("333 nach XmlTextReader" & xml_inifile_fullpath)
                    ' Es folgt das Auslesen der XML-Datei 
                    Do While XMLReader.Read ' Es sind noch Daten vorhanden 
                        ' Welche Art von Daten liegt an? 
                        Select Case XMLReader.NodeType
                        ' Ein Element 
                            Case XmlNodeType.Element
                                gruppe = XMLReader.Name
                                ' Alle Attribute (Name-Wert-Paare) abarbeiten 
                                If XMLReader.AttributeCount > 0 Then
                                    ' Es sind noch weitere Attribute vorhanden 
                                    While XMLReader.MoveToNextAttribute ' nächstes 
                                        iniDict.Add(String.Format("{0}.{1}", gruppe, XMLReader.Name), XMLReader.Value)
                                    End While
                                End If
                            Case XmlNodeType.Text
                            Case XmlNodeType.Comment
                        End Select
                    Loop
                End Using
                nachricht("vor dem return in xmlinireader")
                Return iniDict
            Catch ex As Exception
                nachricht("fehler in xmlinireader:  " ,ex)
                Return Nothing
            End Try
        End Function

        Public Sub New(ByVal xlsminifile As String)
            nachricht("2 in XmlTextReader-----------" & xlsminifile)
            iniDict = XMLiniReader(xlsminifile)
        End Sub
    End Class
End Namespace