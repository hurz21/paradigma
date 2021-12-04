
Public Class WordReplaceTextmarken
    Public Property vorlageFullname$ = ""
    Public Property OUTFullname$ = ""
    Public Shared Property BMliste As Dictionary(Of String, String)

    Sub New()
        'vorlageFullname$ = ""
        'OUTFullname$ = ""
        If Not istOK() Then
            nachricht("  Dateien sind nicht OK1 ------------------------------")
            nachricht("es wurden keine dateinamen übergeben. folgt sicher später------")
            nachricht("vorlageFullname: " & vorlageFullname)
            nachricht("OUTFullname: " & OUTFullname)
        End If

    End Sub

    Sub New(ByVal _INvorlageFullname As String, ByVal _OUTFullname As String, ByVal _liste As Dictionary(Of String, String))
        vorlageFullname = _INvorlageFullname
        OUTFullname = _OUTFullname
        BMliste = _liste
        If Not istOK() Then nachricht("FEHLER:  Dateien sind nicht OK2 ------------------------------")
    End Sub

    Function istOK() As Boolean
        If String.IsNullOrEmpty(vorlageFullname) Then Return False
        If String.IsNullOrEmpty(OUTFullname) Then Return False
        '   If liste Is Nothing OrElse liste.Count < 1 Then Return False
        Return True
    End Function


    Public Function openReadOnly(ByRef datei As String) As Boolean
        nachricht("In openReadOnly------------------")
        Dim word As New Microsoft.Office.Interop.Word.Application
        Dim doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
        Dim obj As Object
        'doc = word.Documents
        Try
            obj = vorlageFullname

            word.Visible = (True) 'Word sehen zum Test?
            doc = word.Documents.Open(obj,, True)
            doc.Activate()

            ''replaceAll(doc, tauschergebnis)
            'If vorlageFullname.ToLower = OUTFullname.ToLower Then
            '    doc.Save()
            'Else
            '    obj = OUTFullname
            '    doc.SaveAs(obj)
            'End If

        Catch ex As Exception
            nachricht("fehler in openReadOnly: Datei nicht vorhanden." ,ex)
            'doc.Close()
            'doc = Nothing
            'word.Application.Quit()
            'word = Nothing

            Return False
        Finally
            'doc.Close()
            'doc = Nothing
            'word.Application.Quit()
            'word = Nothing
            'ReleaseComObj(word)
            'ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try
        Return True
    End Function






    Shared Sub nachricht(ByVal text As String)
        My.Log.WriteEntry(text)
    End Sub
    Public Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = text & ToLogString(ex, text)
        'myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        'mitFehlerMail(text, anhang)
    End Sub

    Public Shared Function GetPDFOutFilename(ByVal dokname As String) As String
        Return dokname.Replace(".docx", "").Replace(".doc", "") & ".pdf"
    End Function



    '    Sub excelNachPDF
    '        ' Ort der Exceldatei
    'Dim QuelleExcelDatei As String = "C:\ExcelDatei.xls"
    '' Speichertort der PDF Datei
    'Dim ZielPdfDatei As String = "C:\Export.pdf"

    '' Leere Datentyp für die Com Schnittstelle
    'Dim m As Object = Type.Missing

    '' Instanz der Excel Anwendunng erstellen
    'Dim app As New Microsoft.Office.Interop.Excel.Application Class()

    '' Instand der Exceldatei
    'Dim workbook As Microsoft.Office.Interop.Excel.Workbook = app.Workbooks.Open(QuelleExcelDatei, m, m, m, m, m, _
    '	m, m, m, m, m, m, _
    '	m, m, m)

    '' Export in das Zielformat und Position
    'workbook.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, ZielPdfDatei, m, m, m, m, _
    '	m, m, m)

    '' Die Exceldatei und Anwendung schließen
    'workbook.Close(m, m, m)
    'app.Quit()

    '' Die Com Objekte freigeben
    'ReleaseComObj(workbook)
    'ReleaseComObj(app)

    '' Die Speichert freigeben
    'GC.Collect()
    'GC.WaitForPendingFinalizers()
    'GC.Collect()
    'GC.WaitForPendingFinalizers()
    '    End Sub
End Class
Public Class WordTools
    Public Shared Function isWordDocumentOpen() As Boolean
        Dim anzahl As Integer = 0
        'Dim wordApp As Word.Application = Nothing
        Dim word As New Microsoft.Office.Interop.Word.Application
        Try
            For Each p As System.Diagnostics.Process In Process.GetProcessesByName("WINWORD")
                If p.Id <> Process.GetCurrentProcess.Id Then
                    Try
                        Debug.Print(p.ProcessName)
                        ' p.CloseMainWindow()
                        word = CType(System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application.14"), Microsoft.Office.Interop.Word.Application) 'As New Microsoft.Office.Interop.Word.Application'   // Word 2010
                        If word.Documents.Count > 0 Then
                            '    Documents.Item(0).Name

                            Return True
                        Else
                            Return False
                        End If
                    Catch ex As Exception
                        ' MsgBox(ex.ToString)
                    End Try
                End If
            Next
            Return False
        Catch ex As Exception
            Return False
        Finally
            'doc.Close()
            'doc = Nothing
            'word.Application.Quit()
            'word = Nothing
            'ReleaseComObj(word)
            'ReleaseComObj(doc)
            ' Die Speichert freigeben
            GC.Collect()
            GC.WaitForPendingFinalizers()
            GC.Collect()
            GC.WaitForPendingFinalizers()
        End Try


    End Function
    'Private Shared Sub ReleaseComObj(o As Object)
    '    Try
    '        Dim i As Integer
    '        Do
    '            i = System.Runtime.InteropServices.Marshal.ReleaseComObject(o)
    '        Loop While i > 0
    '    Catch
    '    Finally
    '        o = Nothing
    '    End Try
    'End Sub

End Class
