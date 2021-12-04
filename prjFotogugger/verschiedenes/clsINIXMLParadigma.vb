Imports System.Xml
Public Class clsINIXMLParadigma
	Public iniDict As Dictionary(Of String, String)
	Private sgsumme$, tiefetag$(20)
	Function addtags(ByVal tiefe as integer) as  String
		Dim summe$ = ""
		For i = 1 To tiefe
			summe = summe & tiefetag$(i)
		Next
		Return summe
	End Function
	Public Function XMLiniReaderParadigma(ByVal xml_inifile_fullpath as string) as  Dictionary(Of String, String)
		Dim iniDict As New Dictionary(Of String, String)
		Dim gruppe$ = "", header$ = "", tabsumme$ = ""
		' Wir benötigen einen XmlReader für das Auslesen der XML-Datei 

		Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath$)
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
                                If XMLReader.Name = "Tag" Then
                                    tiefetag$(CInt(XMLReader.Depth)) = XMLReader.Value
                                    sgsumme$ = addtags(CInt(XMLReader.Depth))
                                End If
                                If XMLReader.Name = "Header" Then
                                    header$ = XMLReader.Value
                                End If
                                'iniDict.Add(gruppe & "." & .Name, .Value)
                            End While
                            For i = 1 To CInt(XMLReader.Depth)
                                tabsumme &= vbTab
                            Next
                            Dim newChild As TreeViewItem = New TreeViewItem()
                            newChild.Header = "Item"
                            '   Me.TreeView1.Items.Add(newChild)


                            tabsumme = ""
                        End If
                        ' Ein Text 
                    Case XmlNodeType.Text
                        ' Console.WriteLine("Es folgt ein Text: " & .Value)
                        ' Ein Kommentar 
                    Case XmlNodeType.Comment
                        'Console.WriteLine("Es folgt ein Kommentar: " & .Value)
                End Select
            Loop
            ' Weiter nach Daten schauen 
          '  XMLReader.Close()
            ' XMLTextReader schließen 
		End Using
		Return iniDict
	End Function

	Public Sub New(ByVal xlsminifile$)
		iniDict = XMLiniReaderParadigma(xlsminifile$)
	End Sub
End Class
