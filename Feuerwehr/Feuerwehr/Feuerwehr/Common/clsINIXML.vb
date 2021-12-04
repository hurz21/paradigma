Imports System.Xml
Public Class clsINIXML
	Public iniDict As Dictionary(Of String, String)
	Public Function XMLiniReader(ByVal xml_inifile_fullpath$) As Dictionary(Of String, String)
		'beisüpielaufruf
		'1. Dim iniDict As Dictionary(Of String, String) = clsXML_ini.XMLiniReader(meininifile$)
		'2. Dim iminternet As Boolean = CType(iniDict("ServerSpezifisch.iminternet"), Boolean)
		'Dim entry As KeyValuePair(Of String, String)
		'Console.WriteLine("==============================")
		'For Each entry In iniDict
		'  Console.WriteLine(entry.Key & "=" & entry.Value)
		'Next

		Dim iniDict As New Dictionary(Of String, String)
		Dim _xlsminifile$
		Dim gruppe$
		' Wir benötigen einen XmlReader für das Auslesen der XML-Datei 

		Using XMLReader As XmlReader = New XmlTextReader(xml_inifile_fullpath$)
			' Es folgt das Auslesen der XML-Datei 
			With XMLReader
				Do While .Read ' Es sind noch Daten vorhanden 
					' Welche Art von Daten liegt an? 
					Select Case .NodeType
						' Ein Element 
						Case XmlNodeType.Element
							gruppe = .Name
							' Alle Attribute (Name-Wert-Paare) abarbeiten 
							If .AttributeCount > 0 Then
								' Es sind noch weitere Attribute vorhanden 
								While .MoveToNextAttribute ' nächstes 
									iniDict.Add(gruppe & "." & .Name, .Value)
								End While
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
				.Close()
				' XMLTextReader schließen 
			End With
		End Using
		Return iniDict
	End Function
	Public Sub XMLWriter(ByVal datei$)
		' Auswahl einer Kodierungsart für die Zeichenablage 
		Dim enc As New System.Text.UnicodeEncoding
		' XmlTextWriter-Objekt für unsere Ausgabedatei erzeugen: 
		Using XMLobj As System.Xml.XmlTextWriter = New System.Xml.XmlTextWriter(datei$, enc)
			With XMLobj
				' Formatierung: 4er-Einzüge verwenden 
				.Formatting = System.Xml.Formatting.Indented
				.Indentation = 4
				' Dann fangen wir mal an: 
				.WriteStartDocument()
				' Beginn eines Elements "Personen". Darin werden wir mehrere 
				' Elemente "Person" unterbringen. 
				.WriteStartElement("Personen")
				' Hier kommt das erste Element "Person". Eine Person hat 
				' in unserem einfachen Beispiel einen Titel, einen Nach- 
				' namen und einen Vornamen. Als Demo soll uns das genügen. 
				.WriteStartElement("Person") ' <Person 
				.WriteAttributeString("Titel", "Dr.")
				.WriteAttributeString("Name", "Meyer")
				.WriteAttributeString("Vorname", "Hans")
				.WriteEndElement() ' Person /> 
				' Hier kommt (noch immer im Element "Personen" das zweite 
				' Element "Person". 
				.WriteStartElement("Person") ' <Person 
				.WriteAttributeString("Titel", "")
				.WriteAttributeString("Name", "Schmidt")
				.WriteAttributeString("Vorname", "Carlos")
				.WriteEndElement() ' Person /> 
				' Nachdem das Element "Personen" zwei Elemente "Person" 
				' erhalten hat, beenden wir die Ausgabe für "Personen"... 
				.WriteEndElement() ' </Personen> 
				' ... und schließen das XML-Dokument (und die Datei) 
				.Close() ' Document 
			End With
		End Using
	End Sub
	Public Sub New(ByVal xlsminifile$)
		iniDict = XMLiniReader(xlsminifile$)
	End Sub
End Class
