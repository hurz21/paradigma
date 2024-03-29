﻿Public Class wordInterop
    Public _dokumentbookmarks As New List(Of String)
    'Sub New(dokumentbookmarks As List(Of String))
    '    For Each ele As String In _dokumentbookmarks
    '        _dokumentbookmarks.Add(ele)
    '    Next
    'End Sub
    Sub New()

    End Sub

    Public Function getBookmarksFromDocument(quelldatei As String) As List(Of String)
        nachricht("In dokreplace------------------")
        Dim word As New Microsoft.Office.Interop.Word.Application
        Dim doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
        Dim obj As Object
        Dim summe As New List(Of String)
        'doc = word.Documents
        Try
            obj = quelldatei
            'word.Visible = (True) 'Word sehen zum Test?
            doc = word.Documents.Open2000(obj)
            doc.Activate()
            For Each bm As Microsoft.Office.Interop.Word.Bookmark In doc.Bookmarks
                summe.Add(bm.Name.ToString)
            Next
            Return summe
        Catch ex As Exception
            nachricht("fehler in dokreplace: Datei nicht vorhanden.", ex)
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing

            Return summe
        Finally
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            ReleaseComObj(word)
            ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try

    End Function
    Public Shared Function dokreplace(ByRef tauschergebnis As String,
                                      quelldatei As String,
                                      zieldatei As String,
                                      bmliste As Dictionary(Of String, String),
                                      dokumentBookmarks As List(Of String)) As Boolean
        nachricht("In dokreplace------------------")
        Dim word As New Microsoft.Office.Interop.Word.Application
        Dim doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
        Dim obj As Object
        Try
            obj = quelldatei
            'word.Visible = (True) 'Word sehen zum Test?
            doc = word.Documents.Open2000(obj)
            doc.Activate()
            replaceAll(doc, tauschergebnis, bmliste) ', dokumentBookmarks)
            If quelldatei.ToLower = zieldatei.ToLower Then
                doc.Save()
            Else
                obj = zieldatei
                doc.SaveAs(obj)
            End If
        Catch ex As Exception
            nachricht("fehler in dokreplace: Datei nicht vorhanden.", ex)
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            Return False
        Finally
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            ReleaseComObj(word)
            ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
        Return True
    End Function

    Friend Function cleanBookmarksFromDocument(dokumentBookmarks As List(Of String)) As List(Of String)
        Dim newlist As New List(Of String)
        Dim nummer As String = ""
        Try

            l(" MOD cleanBookmarksFromDocument anfang")
            If dokumentBookmarks Is Nothing Then Return newlist
            For Each ele As String In dokumentBookmarks
                nummer = ele.Replace("Text", "")
                If ele.StartsWith("Text") And (IsNumeric(nummer)) Then
                    Continue For
                Else
                    newlist.Add(ele)
                End If
            Next
            l(" MOD cleanBookmarksFromDocument ende")
            Return newlist
        Catch ex As Exception
            l("Fehler in cleanBookmarksFromDocument: " & ex.ToString())
            Return newlist
        End Try
    End Function

    Public Shared Function dokreplacealt(ByRef tauschergebnis As String,
                                      quelldatei As String,
                                      zieldatei As String, bmliste As Dictionary(Of String, String)) As Boolean
        nachricht("In dokreplace------------------")
        Dim word As New Microsoft.Office.Interop.Word.Application
        Dim doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
        Dim obj As Object
        Try
            obj = quelldatei
            'word.Visible = (True) 'Word sehen zum Test?
            doc = word.Documents.Open2000(obj)
            doc.Activate()
            replaceAll(doc, tauschergebnis, bmliste)
            If quelldatei.ToLower = zieldatei.ToLower Then
                doc.Save()
            Else
                obj = zieldatei
                doc.SaveAs(obj)
            End If
        Catch ex As Exception
            nachricht("fehler in dokreplace: Datei nicht vorhanden.", ex)
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            Return False
        Finally
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            ReleaseComObj(word)
            ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
        Return True
    End Function
    Shared Function changeBookmark(ByVal textmarke As String, ByVal textm_value As String, ByVal doc As Microsoft.Office.Interop.Word.Document) As Integer
        Try
            '   nachricht("In changeBookmark------------------")
            Dim test = textm_value.Trim.Replace("""", "")

            If test = "0" Then
                Return 0
            End If
            If doc.Range.Bookmarks.Exists(textmarke) Then
                doc.Bookmarks().Item(textmarke).Range.Text = textm_value
                Return 1
            Else
                '  nachricht("Warnung:changeBookmark: Textmarke nicht vorhanden: " & textmarke)
                Return 0
            End If
        Catch ex As Exception
            nachricht("Fehler in changeBookmark: textmarke: " & textmarke & " textm_value: " & textm_value & vbCrLf, ex)
            Return -1
        End Try
    End Function
    Private Shared Sub replaceAllNEU(ByVal doc As Microsoft.Office.Interop.Word.Document,
                           ByRef tauschergebnis As String,
                           BMliste As Dictionary(Of String, String),
                           dokumentBookmarks As List(Of String))
        nachricht("In replaceAll------------------")
        Dim erfolg As New System.Text.StringBuilder
        Dim fehlt As New System.Text.StringBuilder
        Dim COMFehler As New System.Text.StringBuilder
        Dim result As Integer
        Try
            If BMliste Is Nothing OrElse BMliste.Count < 1 Then
                nachricht("bookmarklisteValues ist leer")
                tauschergebnis = "bookmarklistevalues ist leer"
                Exit Sub
            End If
            If dokumentBookmarks Is Nothing OrElse dokumentBookmarks.Count < 1 Then
                nachricht("bookmarklistenames ist leer")
                tauschergebnis = "bookmarklistenames ist leer"
                Exit Sub
            End If
            Dim www As New wordInterop
            www._dokumentbookmarks = dokumentBookmarks
            For Each ttt In BMliste
                If www.isInVorlageDokumentVorhanden(ttt.Key, dokumentBookmarks) Then
                    result = wordInterop.changeBookmark(ttt.Key, ttt.Value, doc)
                    Select Case result
                        Case 1
                            If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then erfolg.Append(ttt.Key & ", ")
                        Case 0
                            If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then fehlt.Append(ttt.Key & ", ")
                        Case -1
                            If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then COMFehler.Append(ttt.Key & ", ")
                    End Select
                Else
                End If

                ' nachricht(String.Format("replaceAll Tausche:{0} | {1}", ttt.Key, ttt.Value))
            Next
            nachricht("Tausch erfolgreich")
            tauschergebnis = "Fehlende Textmarken (Hätten verwendet werden können): " & vbCrLf & fehlt.ToString & vbCrLf &
                 vbCrLf & COMFehler.ToString
            erfolg = Nothing
            COMFehler = Nothing
            fehlt = Nothing
        Catch ex As Exception
            nachricht(String.Format("Fehler in replaceAll: {0}{1}", vbCrLf, ex))
        End Try
    End Sub

    Function isInVorlageDokumentVorhanden(key As String, vorlageListe As List(Of String)) As Boolean
        Try
            l(" MOD isInVorlageDokumentVorhanden anfang")
            For Each ele As String In vorlageListe
                If key.ToLower.Trim = ele.ToLower.Trim Then
                    Return True
                End If
            Next
            l(" MOD isInVorlageDokumentVorhanden ende")
            Return False
        Catch ex As Exception
            l("Fehler in isInVorlageDokumentVorhanden: " & ex.ToString())
            Return False
        End Try
    End Function
    Function isInVorlageDokumentVorhanden(key As String) As Boolean
        Try
            l(" MOD isInVorlageDokumentVorhanden anfang")
            For Each ele As String In _dokumentbookmarks
                If key.ToLower.Trim = ele.ToLower.Trim Then
                    Return True
                End If
            Next
            l(" MOD isInVorlageDokumentVorhanden ende")
            Return False
        Catch ex As Exception
            l("Fehler in isInVorlageDokumentVorhanden: " & ex.ToString())
            Return False
        End Try
    End Function

    Private Shared Sub replaceAll(ByVal doc As Microsoft.Office.Interop.Word.Document,
                           ByRef tauschergebnis As String,
                           BMliste As Dictionary(Of String, String))
        nachricht("In replaceAll------------------")
        Dim erfolg As New System.Text.StringBuilder
        Dim fehlt As New System.Text.StringBuilder
        Dim COMFehler As New System.Text.StringBuilder
        Dim result As Integer
        Try
            If BMliste Is Nothing OrElse BMliste.Count < 1 Then
                nachricht("bookmarkliste ist leer")
                tauschergebnis = "bookmarkliste ist leer"
                Exit Sub
            End If
            For Each ttt In BMliste
                result = wordInterop.changeBookmark(ttt.Key, ttt.Value, doc)
                Select Case result
                    Case 1
                        If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then erfolg.Append(ttt.Key & ", ")
                    Case 0
                        If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then fehlt.Append(ttt.Key & ", ")
                    Case -1
                        If String.IsNullOrEmpty(ttt.Value.ToString.Trim) Then COMFehler.Append(ttt.Key & ", ")
                End Select
                ' nachricht(String.Format("replaceAll Tausche:{0} | {1}", ttt.Key, ttt.Value))
            Next
            nachricht("Tausch erfolgreich")
            tauschergebnis = "Fehlende Textmarken (Hätten verwendet werden können): " & vbCrLf & fehlt.ToString & vbCrLf &
                 vbCrLf & COMFehler.ToString
            erfolg = Nothing
            COMFehler = Nothing
            fehlt = Nothing
        Catch ex As Exception
            nachricht(String.Format("Fehler in replaceAll: {0}{1}", vbCrLf, ex))
        End Try
    End Sub

    Private Shared Sub ReleaseComObj(o As Object)
        Try
            Dim i As Integer
            Do
                If o IsNot Nothing Then

                    i = System.Runtime.InteropServices.Marshal.ReleaseComObject(o)
                End If
            Loop While i > 0
        Catch
        Finally
            o = Nothing
        End Try
    End Sub
    Public Sub TM_ernteBookmarksAusVorlagenDoc(ByRef bookmarkArray() As String, quelldoc As String) 'liefert leere bookmarks
        nachricht("cropBookmarksList ---------------------- ")
        Dim word As New Microsoft.Office.Interop.Word.Application 'habe hier new ergänzt ????
        Dim doc As New Microsoft.Office.Interop.Word.Document
        'doc = word.Documents
        Dim obj As Object
        Try
            Dim int As Integer
            nachricht("cropBookmarksList vor öffnen ")
            obj = quelldoc
            doc = word.Documents.Open2000(obj)
            doc.Activate()
            nachricht("cropBookmarksList nach activate - vor schleife")
            nachricht("cropBookmarksList anzahl textmarken: " & doc.Bookmarks.Count)
            With doc
                ReDim bookmarkArray(.Bookmarks.Count - 1)
                For int = 1 To .Bookmarks.Count
                    bookmarkArray(int - 1) = .Bookmarks(int).Name
                    nachricht("Textmarke gefunden: " & .Bookmarks(int).Name)
                Next
            End With
        Catch ex As Exception
            nachricht("cropBookmarksList: ", ex)
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
        Finally
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            ReleaseComObj(word)
            ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            'GC.WaitForPendingFinalizers()
        End Try
    End Sub
    Public Shared Function dok2pdf(ByRef dokname As String) As Boolean
        nachricht("In dokreplace------------------")
        Dim outfile As Object
        Dim word As New Microsoft.Office.Interop.Word.Application
        Dim doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
        Dim obj As Object
        Try
            'word.Visible = (True) 'Word sehen zum Test?
            dokname = dokname.Trim
            obj = dokname.Trim
            doc = word.Documents.Open2000(obj)
            doc.Activate()
            outfile = WordReplaceTextmarken.GetPDFOutFilename(dokname)
            doc.SaveAs2(outfile, 17)
            Return True
        Catch ex As Exception
            nachricht("fehler  bei der umwandlung in dokreplace: Datei nicht vorhanden.", ex)
            Return False
        Finally
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            ReleaseComObj(word)
            ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Function
    Public Shared Function rtf2doc(ByRef dokname As String) As String
        nachricht("In rtf2doc------------------")
        Dim outfile As Object
        Dim word As New Microsoft.Office.Interop.Word.Application
        Dim doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
        Dim obj As Object
        Try
            obj = dokname
            doc = word.Documents.Open2000(obj)
            doc.Activate()
            outfile = dokname.Replace("rtf", "").Replace("doc", "") & "doc"
            doc.SaveAs2(outfile, Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument)
            Return CType(outfile, String)
        Catch ex As Exception
            nachricht("fehler in rtf2doc: Datei nicht vorhanden.", ex)
            Return ""
        Finally
            If doc IsNot Nothing Then
                doc.Close()
                doc = Nothing
            End If
            If word IsNot Nothing Then
                word.Application.Quit()
                word = Nothing
            End If
            ReleaseComObj(word)
            ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Function

    Public Shared Function dok2pdfA(ByRef dokname As String, outfile As String) As Boolean
        nachricht("In dokreplace------------------")
        'Dim outfile As String
        Dim word As New Microsoft.Office.Interop.Word.Application
        Dim doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
        Dim obj As Object
        Try
            obj = dokname
            doc = word.Documents.Open2000(obj)
            doc.Activate()
            Dim m As Object = Type.Missing
            ' outfile = GetPDFOutFilename(dokname)
            doc.ExportAsFixedFormat(outfile,
                                    Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF,
                                    False,
, , , , , , , , , , True,  )
            Return True
        Catch ex As Exception
            nachricht("fehler in dok2pdfA: Datei nicht vorhanden.", ex)
            Return False
        Finally
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
            ReleaseComObj(word)
            ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
    End Function

End Class
