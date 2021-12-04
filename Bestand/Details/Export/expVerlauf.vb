Imports System.Data

Module expVerlauf
    Sub VERLAUF_CsvListeErstellen(datei As String)
        Dim handcsv As New clsCSVausgaben("Ereignisse", myGlobalz.sitzung.EreignisseRec.dt,
                                          myGlobalz.sitzung.aktVorgangsID,
                                         datei, CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
         handcsv.Dispose
    End Sub

    Private Function GetDateinameVerlauf(ByVal pfad_verlauf As String, ByVal zeile As DataRow, endung As String) As String
        Dim dateiname As String

        dateiname = (pfad_verlauf & "\").trim
        dateiname = dateiname &  clsString.date2string(CDate(zeile.Item("DATUM")), 2).trim
        dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(zeile.Item("ART").ToString).trim
        dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(clsString.kuerzeTextauf(zeile.Item("BESCHREIBUNG").ToString, 50)).trim
        dateiname = dateiname & endung.trim
        Return dateiname.trim
    End Function

    Sub VERLAUF_einzelObjekteErstellen(pfad_verlauf As String, ausgabeformat As String)
        Dim dateiname As String
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(1252)
        '.ToString("yyyy-MM-dd_HH_mm_ss")
        If ausgabeformat = "txt" Then
            For Each zeile As DataRow In myGlobalz.sitzung.EreignisseRec.dt.Rows
                dateiname = GetDateinameVerlauf(pfad_verlauf, zeile, ".txt")
                entfDoppelteTrenner(dateiname)
                If Not zeile.Item("NOTIZ").ToString.IsNothingOrEmpty() Then
                    Using raus As New IO.StreamWriter(dateiname)
                        raus.WriteLine(zeile.Item("NOTIZ").ToString)
                    End Using
                End If
            Next
        End If
        If ausgabeformat = "html" Then
            Dim br As String = "</br>"
            For Each zeile As DataRow In myGlobalz.sitzung.EreignisseRec.dt.Rows
                dateiname = GetDateinameVerlauf(pfad_verlauf, zeile, ".html")
                entfDoppelteTrenner(dateiname)
                If Not zeile.Item("NOTIZ").ToString.IsNothingOrEmpty() Then
                    Using raus As New IO.StreamWriter(dateiname, False, enc)
                        raus.WriteLine("<HTML>" & Environment.NewLine)
                        raus.WriteLine("<HEAD>" & Environment.NewLine)
                        raus.WriteLine("</HEAD>" & Environment.NewLine)
                        raus.WriteLine("<BODY>" & Environment.NewLine)
                        raus.WriteLine("<H2>" & Environment.NewLine)
                        raus.WriteLine(zeile.Item("BESCHREIBUNG").ToString & Environment.NewLine)
                        raus.WriteLine("</H2>" & Environment.NewLine)

                        raus.WriteLine("<b>" & Environment.NewLine)
                        raus.WriteLine("Am    : " & zeile.Item("DATUM").ToString & br & Environment.NewLine)
                        raus.WriteLine("Art   : " & zeile.Item("ART").ToString & br & Environment.NewLine)
                        raus.WriteLine("Anhang: " & zeile.Item("ANZAHL").ToString & br & br & Environment.NewLine)
                        raus.WriteLine("<b>" & Environment.NewLine & Environment.NewLine)
                        raus.WriteLine(getBRforCR(zeile.Item("NOTIZ").ToString) & Environment.NewLine)
                        raus.WriteLine("</BODY>" & Environment.NewLine)
                        raus.WriteLine("</HTML>" & Environment.NewLine)
                    End Using
                End If
            Next
        End If
    End Sub

    Private Function getBRforCR(txt As String) As String
        Try
            If txt.IsNothingOrEmpty() Then Return ""
            txt = txt.Replace(vbCr, "</br>")
            txt = txt.Replace(vbLf, "</br>")
            txt = txt.Replace(vbCrLf, "</br>")
            txt = txt.Replace("</br></br>", "</br>")
            txt = txt.Replace("</br></br>", "</br>")
            txt = txt.Replace("</br></br>", "</br>")
            Return txt
        Catch ex As Exception
            nachricht("fehler in getBRforCR: " & ex.ToString)
            Return ""
        End Try
    End Function

End Module
