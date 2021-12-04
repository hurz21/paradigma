Namespace nachZielKopieren
    Public Class NachZielKopieren
        Public Shared Sub exe(ByVal ldok As Dokument, ByVal ziel As String) 'myGlobalz.sitzung.aktDokument
            Dim selectedpfad As String = ""
            Try
                nachricht(checkout.checkout(ldok, myGlobalz.sitzung.aktVorgangsID))
                Dim objDialog As New Forms.FolderBrowserDialog
                objDialog.Description = "Bitte wählen Sie das Verzeichnis aus!"
                objDialog.SelectedPath = ziel
                Dim objResult As Forms.DialogResult
                objResult = objDialog.ShowDialog()
                selectedpfad = objDialog.SelectedPath
                objDialog.Dispose()
                objDialog = Nothing
                If objResult = System.Windows.Forms.DialogResult.OK Then
                    Dim QuellDatei As New IO.FileInfo(ldok.FullnameCheckout)
                    Dim ZielDatei As New IO.FileInfo((IO.Path.Combine(selectedpfad, QuellDatei.Name)))
                    If QuellDatei.Exists Then
                        If ZielDatei.Exists Then
                            QuellDatei.CopyTo(ZielDatei.FullName, True)
                        Else
                            QuellDatei.CopyTo(ZielDatei.FullName)
                        End If
                    End If
                    QuellDatei = Nothing : ZielDatei = Nothing
                    MsgBox("Das ausgewählte Verzeichnis wird nun geöffnet", MsgBoxStyle.Information)
                    Process.Start(selectedpfad)
                Else
                    MsgBox("Es wurde keine Auswahl getroffen!")
                End If
            Catch ex As Exception
                nachricht_und_Mbox("Problem beim Kopieren nach " & selectedpfad &
                                   ". Sie verfügen nicht über das Schreibrecht auf " & selectedpfad & " !!! " & Environment.NewLine &
                                   "----------------------------------------------------------------------------------" & Environment.NewLine &
                                   ex.ToString)
            End Try
        End Sub
    End Class
End Namespace

