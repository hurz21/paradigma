Imports System.IO
Imports System.Text.RegularExpressions
Imports DocumentFormat.OpenXml.Packaging
Imports System.Linq
Imports DocumentFormat.OpenXml.Wordprocessing
'aufgegeben weil es nicht zuverlässig funzt, schade
Public Class wordOpenXML
    Public Shared Sub dokreplaceHashNEU(ByRef tauschergebnis As String, vdatei As String)
        nachricht("In replaceAll------------------")
        Dim erfolg As New System.Text.StringBuilder
        Dim fehlt As New System.Text.StringBuilder
        Dim COMFehler As New System.Text.StringBuilder
        Dim result As Integer
        Dim regexText As Regex
        Dim temp As String
        Try
            If WordReplaceTextmarken.BMliste Is Nothing OrElse WordReplaceTextmarken.BMliste.Count < 1 Then
                nachricht("bookmarkliste ist leer")
                tauschergebnis = "bookmarkliste ist leer"
                Exit Sub
            End If
            Dim wordDoc As WordprocessingDocument = WordprocessingDocument.Open(vdatei, True)
            Using (wordDoc)
                Dim docText As String = Nothing
                Dim sr As IO.StreamReader = New IO.StreamReader(wordDoc.MainDocumentPart.GetStream)

                Using (sr)
                    docText = sr.ReadToEnd
                End Using


                For Each ttt In WordReplaceTextmarken.BMliste
                    '     result = changeBookmark(ttt.Key, ttt.Value, doc)
                    temp = ttt.Value.Trim.Replace("""", "")
                    If ttt.Key = "Nachname" Then
                        Debug.Print("")
                    End If
                    If temp.IsNothingOrEmpty OrElse temp = "0" Then
                        regexText = New Regex("#" & ttt.Key & "#")
                        docText = regexText.Replace(docText, "")
                        result = 0
                    Else
                        regexText = New Regex("#" & ttt.Key & "#")
                        docText = regexText.Replace(docText, ttt.Value)
                        result = 1
                    End If


                    Select Case result
                        Case 1
                            If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then erfolg.Append(ttt.Key & ", ")
                        Case 0
                            If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then fehlt.Append(ttt.Key & ", ")
                        Case -1
                            If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then COMFehler.Append(ttt.Key & ", ")
                    End Select
                Next
                nachricht("Tausch erfolgreich")
                tauschergebnis = erfolg.ToString & Environment.NewLine &
                    "Fehlende Textmarken (Hätten verwendet werden können): " & vbCrLf & fehlt.ToString & vbCrLf &
                 vbCrLf & COMFehler.ToString
                erfolg = Nothing
                COMFehler = Nothing
                fehlt = Nothing

                'Dim regexText As Regex = New Regex(old1)
                'docText = regexText.Replace(docText, new1)
                Dim sw As StreamWriter = New StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create))

                Using (sw)
                    sw.Write(docText)
                End Using
            End Using



            nachricht("Tausch erfolgreich")
            'tauschergebnis = "Fehlende Textmarken (Hätten verwendet werden können): " & vbCrLf & fehlt.ToString & vbCrLf &
            '     vbCrLf & COMFehler.ToString
            'erfolg = Nothing
            'COMFehler = Nothing
            'fehlt = Nothing
        Catch ex As Exception
            nachricht(String.Format("Fehler in replaceAll: {0}{1}", vbCrLf, ex))
        End Try
    End Sub
    Public Shared Sub dokreplaceHash(ByRef tauschergebnis As String, vdatei As String)
        nachricht("In replaceAll------------------")
        Dim erfolg As New System.Text.StringBuilder
        Dim fehlt As New System.Text.StringBuilder
        Dim COMFehler As New System.Text.StringBuilder
        Dim result As Integer
        Dim regexText As Regex
        Dim temp As String
        Try
            If WordReplaceTextmarken.BMliste Is Nothing OrElse WordReplaceTextmarken.BMliste.Count < 1 Then
                nachricht("bookmarkliste ist leer")
                tauschergebnis = "bookmarkliste ist leer"
                Exit Sub
            End If
            Dim wordDoc As WordprocessingDocument = WordprocessingDocument.Open(vdatei, True)
            Using (wordDoc)



                For Each ttt In WordReplaceTextmarken.BMliste
                    '     result = changeBookmark(ttt.Key, ttt.Value, doc)
                    temp = ttt.Value.Trim.Replace("""", "")
                    If ttt.Key = "Nachname" Then
                        Debug.Print("")
                    End If
                    Dim res = From bm In wordDoc.MainDocumentPart.Document.Body.Descendants()
                              Where bm.InnerText <> String.Empty AndAlso bm.InnerText.Contains("#" & ttt.Key & "#") AndAlso bm.HasChildren = True
                              Select bm

                    For Each item In res
                        If temp.IsNothingOrEmpty OrElse temp = "0" Then
                            item.InsertAfterSelf(New Text(item.InnerText.Replace("#" & ttt.Key & "#", " ")))
                            item.Remove()
                        Else
                            item.InsertAfterSelf(New Text(item.InnerText.Replace("#" & ttt.Key & "#", temp)))
                            item.Remove()
                        End If

                    Next


                Next
                nachricht("Tausch erfolgreich")
                tauschergebnis = erfolg.ToString & Environment.NewLine &
                    "Fehlende Textmarken (Hätten verwendet werden können): " & vbCrLf & fehlt.ToString & vbCrLf &
                 vbCrLf & COMFehler.ToString
                erfolg = Nothing
                COMFehler = Nothing
                fehlt = Nothing

                'Dim regexText As Regex = New Regex(old1)
                'docText = regexText.Replace(docText, new1)
                'Dim sw As StreamWriter = New StreamWriter(wordDoc.MainDocumentPart.GetStream(FileMode.Create))


            End Using



            nachricht("Tausch erfolgreich")
            'tauschergebnis = "Fehlende Textmarken (Hätten verwendet werden können): " & vbCrLf & fehlt.ToString & vbCrLf &
            '     vbCrLf & COMFehler.ToString
            'erfolg = Nothing
            'COMFehler = Nothing
            'fehlt = Nothing
        Catch ex As Exception
            nachricht(String.Format("Fehler in replaceAll: {0}{1}", vbCrLf, ex))
        End Try
    End Sub
End Class

