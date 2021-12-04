Namespace nachZielKopieren
    Public Class NachZielKopieren
        Public Shared Sub exe(ByVal ldok As Dokument, ByVal ziel$) 'myGlobalz.sitzung.aktDokument
            Try
                nachricht(checkout.checkout(ldok, myglobalz.sitzung.aktVorgangsID))
                Dim selectedpfad As String = ""
                Dim objDialog As New Forms.FolderBrowserDialog
                objDialog.Description = "Bitte wählen Sie das Verzeichnis aus!"
                objDialog.SelectedPath = ziel$
                Dim objResult As Forms.DialogResult
                objResult = objDialog.ShowDialog()
                selectedpfad = objDialog.SelectedPath

                objDialog.Dispose()
                objDialog = Nothing

                If objResult = System.Windows.Forms.DialogResult.OK Then
                    'System.Windows.MessageBox.Show(objDialog.SelectedPath)
                    Dim QuellDatei As New IO.FileInfo(ldok.FullnameCheckout)
                    Dim ZielDatei As New IO.FileInfo((IO.Path.Combine(selectedpfad, QuellDatei.Name)))
                    '   Dim destfile$ = objDialog.SelectedPath & "\" & QuellDatei.Name
                    If QuellDatei.Exists Then
                        If ZielDatei.Exists Then
                            QuellDatei.CopyTo(ZielDatei.FullName, True)
                        Else
                            QuellDatei.CopyTo(ZielDatei.FullName)
                        End If
                    End If
                    MsgBox("Das ausgewählte Verzeichnis wird nun geöffnet", MsgBoxStyle.Information)
                    Process.Start(selectedpfad)
                Else
                    MsgBox("Es wurde keine Auswahl getroffen!")
                End If
            Catch ex As Exception
                nachricht_und_Mbox("problem beim kopieren nach o:" & ex.ToString)
            End Try
        End Sub
    End Class
End Namespace

