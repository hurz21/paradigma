Public Class clsWord
    Public Shared Sub WordReadonlyDokumentStarten(ByVal aktdoku As Dokument)
        System.Threading.Thread.Sleep(1000)
        Dim myproc As New System.Diagnostics.Process
        Try
            Dim zieldatei As String
            zieldatei = aktdoku.FullnameCheckout.ToLower.Replace(".docx", ".pdf").Replace(".doc", ".pdf")
            Dim lw = New WordReplaceTextmarken(aktdoku.FullnameCheckout, "", Nothing)
            'FileArchivTools.inputFileReadonlyEntfernen(aktdoku.FullnameCheckout)
            If wordInterop.dok2pdfA(aktdoku.FullnameCheckout, zieldatei) Then
                lw = Nothing
                Process.Start(zieldatei)
            End If
            lw = Nothing
        Catch ex1 As Exception
            MsgBox("Der wordviewere ist noch nicht installiert. bitte beim admin melden !!! ")
            System.Threading.Thread.Sleep(2000)
            Try
                System.Diagnostics.Process.Start(aktdoku.FullnameCheckout)
            Catch ex As Exception
                l("Fehler in WordReadonlyDokumentStarten: " & aktdoku.FullnameCheckout & Environment.NewLine &
                                aktdoku.FullnameImArchiv & Environment.NewLine &
                                ex.ToString)
            End Try
        End Try
    End Sub
End Class
