Public Class winDokumentUpdateArchiv
    Property geloeschteDatei As String
    Property erfolgreich As Boolean = False
    Sub New(_geloeschteDatei As String)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        geloeschteDatei = _geloeschteDatei
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub
    Sub bildeKandidatWord(geloeschteDatei As String, ByRef kandidat As Dokument, ByRef hatKandidat As Boolean)
        Try
            If geloeschteDatei.IsNothingOrEmpty Then
                nachricht("fehler in bildeKandidatWord, geloeschteDatei is nothingoremty")
            End If
            If myglobalz.sitzung.wordDateiImEditModus.tempEditDatei.IsNothingOrEmpty Then
                nachricht("fehler in bildeKandidatWord, wordDateiImEditModus.tempEditDatei is nothingoremty")
            End If
            If geloeschteDatei.ToLower.EndsWith(".doc") Or
               geloeschteDatei.ToLower.EndsWith(".docx") Then
                If geloeschteDatei.ToLower.Contains(myglobalz.sitzung.wordDateiImEditModus.tempEditDatei.ToLower) Then
                    nachricht("gelöschteDatei entspricht der editdatei")
                    nachricht("worddatei wurde geschlossen")
                    kandidat = CType(myglobalz.sitzung.wordDateiImEditModus.Clone, Dokument)
                    kandidat.FullnameCheckout = myglobalz.sitzung.wordDateiImEditModus.FullnameCheckout
                    kandidat.FullnameImArchiv = myglobalz.sitzung.wordDateiImEditModus.FullnameImArchiv
                    kandidat.tempEditDatei = myglobalz.sitzung.wordDateiImEditModus.tempEditDatei
                    kandidat.DateinameMitExtension = myglobalz.sitzung.wordDateiImEditModus.DateinameMitExtension
                    kandidat.revisionssicher = myGlobalz.sitzung.wordDateiImEditModus.revisionssicher
                    kandidat.kompressed = myGlobalz.sitzung.wordDateiImEditModus.kompressed
                    hatKandidat = True
                End If
            End If
        Catch ex As Exception
            nachricht("fehler in bildeKandidatWord: ", ex)
        End Try
    End Sub

    Sub bildeKandidatExcel(geloeschteDatei As String, ByRef kandidat As Dokument, ByRef hatKandidat As Boolean)
        Try
            If geloeschteDatei.IsNothingOrEmpty Then
                nachricht("fehler in bildeKandidatExcel, geloeschteDatei is nothingoremty")
            End If
            If myglobalz.sitzung.excelDateiImEditModus.tempEditDatei.IsNothingOrEmpty Then
                nachricht("fehler in bildeKandidatExcel, exceldatei.tempEditDatei is nothingoremty")
            End If
            If geloeschteDatei.ToLower.EndsWith(".xls") Or
           geloeschteDatei.EndsWith(".xlsx") Then
                If geloeschteDatei.ToLower.Contains(myglobalz.sitzung.excelDateiImEditModus.tempEditDatei.ToLower) Then
                    nachricht("exceldatei wurde geschlossen")
                    kandidat = CType(myglobalz.sitzung.excelDateiImEditModus.Clone, Dokument)
                    kandidat.FullnameCheckout = myglobalz.sitzung.excelDateiImEditModus.FullnameCheckout
                    kandidat.FullnameImArchiv = myglobalz.sitzung.excelDateiImEditModus.FullnameImArchiv
                    kandidat.tempEditDatei = myglobalz.sitzung.excelDateiImEditModus.tempEditDatei
                    kandidat.DateinameMitExtension = myglobalz.sitzung.excelDateiImEditModus.DateinameMitExtension
                    kandidat.revisionssicher = myGlobalz.sitzung.excelDateiImEditModus.revisionssicher
                    kandidat.kompressed = myGlobalz.sitzung.excelDateiImEditModus.kompressed
                    hatKandidat = True
                End If
            End If
        Catch ex As Exception
            nachricht("fehler in bildeKandidatExcel: ", ex)
        End Try
    End Sub
    Function getDateiistneuerAlsImArchiv(fullnamecheckout As String, FullnameImArchiv As String) As Boolean
        Dim CO_test, AR_test As IO.FileInfo
        CO_test = New IO.FileInfo(fullnamecheckout)
        AR_test = New IO.FileInfo(FullnameImArchiv)

        nachricht("Alt: " & AR_test.LastWriteTime.ToString)
        nachricht("Neu: " & CO_test.LastWriteTime.ToString)
        Return AR_test.LastWriteTime < CO_test.LastWriteTime
        ' nachricht("dateiWurdeGeaendert: " & dateiIstNeuerAlsImArchiv)
        AR_test = Nothing ' sonst läßt sie sich nicht überschreiben
        CO_test = Nothing
    End Function
End Class
