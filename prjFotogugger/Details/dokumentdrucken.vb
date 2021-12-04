Imports System.Drawing.Printing

Namespace nachZielKopieren
    Class dokumentdrucken
        Private Sub New()

        End Sub
        Shared Sub printbatchRTF(dok As Dokument, winwordpath As String, enc As Text.Encoding, docxdatei As String)
            Dim param, batchFile, batchfileDir As String
            Try
                l(" MOD printbatch anfang")
                Dim fi As New IO.FileInfo(docxdatei)
                batchfileDir = fi.DirectoryName
                fi = Nothing
                param = """%ProgramFiles%\Windows NT\Accessories\wordpad.exe   ""  /p """ & docxdatei & """"
                batchFile = batchfileDir & "\print_" & dok.DocID & ".bat"
                My.Computer.FileSystem.WriteAllText(batchFile, param, False, enc)
                Microsoft.VisualBasic.Shell(batchFile)
                l(" MOD printbatch ende")
            Catch ex As Exception
                l("Fehler in printbatch: ", ex)
            End Try
        End Sub
        Shared Sub printbatchDOCX(dok As Dokument, winwordpath As String, enc As Text.Encoding, docxdatei As String)
            Dim param, batchFile, batchfileDir As String
            Try
                l(" MOD printbatch anfang")
                Dim fi As New IO.FileInfo(docxdatei)
                batchfileDir = fi.DirectoryName
                fi = Nothing
                param = "start /B ""Drucken"" """ & winwordpath & """ /t /mFilePrintDefault """ & docxdatei & """"
                batchFile = batchfileDir & "\print_" & dok.DocID & ".bat"
                My.Computer.FileSystem.WriteAllText(batchFile, param, False, enc)
                Microsoft.VisualBasic.Shell(batchFile)
                l(" MOD printbatch ende")
            Catch ex As Exception
                l("Fehler in printbatch: ", ex)
            End Try
        End Sub
        Shared Sub printbatchPDF(dok As Dokument, pdfreader As String, enc As Text.Encoding, pdfdatei As String)
            Dim param, batchFile, batchfileDir As String
            Try
                l(" MOD printbatch anfang")
                Dim fi As New IO.FileInfo(pdfdatei)
                batchfileDir = fi.DirectoryName
                fi = Nothing
                param = "start /B ""Drucken"" """ & pdfreader & """ /t """ & pdfdatei & """"
                batchFile = batchfileDir & "\print_" & dok.DocID & ".bat"
                My.Computer.FileSystem.WriteAllText(batchFile, param, False, enc)
                Microsoft.VisualBasic.Shell(batchFile)
                l(" MOD printbatch ende")
            Catch ex As Exception
                l("Fehler in printbatch: ", ex)
            End Try
        End Sub
        Private Shared Sub exeExtracted(ByVal dokument As Dokument, ByVal info As ProcessStartInfo)
            Try
                Process.Start(info)
            Catch ex As Exception
                MsgBox("Für diesen Dateityp ist keine Standardanwendung festgelegt. " &
                       "Zum Drucken legen Sie eine Standardanwendung fest!" & Environment.NewLine &
                       Environment.NewLine & "(" &
                       ex.ToString & ")")
                ' Debug.Print(ex.ToString)
                nachricht("fehler 2ind dokumentdrucken--" & dokument.FullnameCheckout)
            End Try
        End Sub
        Shared Sub exe(dokument As Dokument)
            nachricht("dokumentdrucken--")
            nachricht("datei : " & dokument.FullnameCheckout)
            Dim printDoc As String
            Dim info As ProcessStartInfo= Nothing
            Try

                printDoc = dokument.FullnameCheckout '"L:\websys\mapfiles\cache\kuhn_p.map"
                info = New ProcessStartInfo(printDoc)
                If printDoc.ToLower.EndsWith(".msg") Then
                    info.Verb = "open"
                Else
                    info.Verb = "PrintTo"
                End If

                info.CreateNoWindow = True
                info.WindowStyle = ProcessWindowStyle.Hidden
                nachricht("vor try : ")
            Catch
                nachricht("fehler 1ind dokumentdrucken--" & dokument.FullnameCheckout)
            End Try

            exeExtracted(dokument, info)
        End Sub

    End Class
End Namespace
