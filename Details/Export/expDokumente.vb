Imports System.Data
Module expDokumente
    Sub dokumente_CsvListeErstellen(datei As String)
        Dim handcsv As New clsCSVausgaben("Dokumente", myGlobalz.Arc.vorgangDocDt,
                                          myGlobalz.sitzung.aktVorgangsID,
                                         datei)
        nachricht(" exportfile:" & handcsv.ausgeben())
         handcsv.Dispose
    End Sub

    Sub Dokumente_einzelObjekteErstellen(pfad_dokumente As String,
                                         nomsg As Boolean,
                                         pfad_raumbezug As String,
                                         msg2html As Boolean,
                                         word2PDF As Boolean)
        Dim zielDatei, quelldatei As String
        Dim FIquelle As IO.FileInfo
        '.ToString("yyyy-MM-dd_HH_mm_ss")
        For Each aDok As DataRow In myGlobalz.Arc.vorgangDocDt.Rows
            zielDatei = GetDateinameDokumente(pfad_dokumente, aDok, 50)
            If istGISPDFkarte(aDok) Then
                zielDatei = GetDateinameRaumbezug(pfad_raumbezug, aDok)
            End If
            entfDoppelteTrenner(zielDatei)
            quelldatei = getQuelldatei(aDok)
            FIquelle = New IO.FileInfo(quelldatei)
            'If nomsg And isMSGoutlookdatei(quelldatei) Then
            '    '  Continue For
            'End If
            DokArc.ueberkopieren(quelldatei, zielDatei, FIquelle)

            If istWordDatei(zielDatei) Then
                If word2PDF Then
                    Dim lw = New LIBwordvorlage.WordReplaceTextmarken()
                    If lw.dok2pdf(zielDatei) Then
                        IO.File.Delete(zielDatei)
                    End If
                Else
                    'kein delete
                End If

            End If

            If isMSGoutlookdatei(zielDatei) Then
                If msg2html Then
                    If LIBOutlook2.MeinOutlook.msg2html(zielDatei) Then
                        IO.File.Delete(zielDatei)
                    End If
                Else
                    'kein delete
                End If

            End If
        Next
    End Sub


    Private Function GetDateinameRaumbezug(pfad_raumbezug As String, zeile As DataRow) As String
        Dim dateiname As String
        dateiname = pfad_raumbezug & "\"
        dateiname = dateiname & clsString.date2string(CDate(zeile.Item("FILEDATUM")), 3).Trim
        dateiname = dateiname & trennerInFileNames & clsString.kuerzeTextauf(
            clsString.normalize_Filename(zeile.Item("BESCHREIBUNG").ToString, " "), 50).Replace(".", "")
        dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(zeile.Item("DATEINAMEEXT").ToString)
        Return dateiname
    End Function
    Private Sub buildMitteAusWorte(ByRef mitte As String, ByVal worte As String())
        For Each wort In worte
            mitte = mitte & wort
        Next
    End Sub
    Private Function GetDateinameDokumente(pfad_dokumente As String, zeile As DataRow, limit As Int16) As String
        Dim dateiname, mitte As String
        dateiname = (pfad_dokumente & "\").trim
        dateiname = dateiname & clsString.date2string(CDate(zeile.Item("FILEDATUM")), 3).Trim
        mitte = trennerInFileNames & clsString.normalize_Filename(zeile.Item("DATEINAMEEXT").ToString).trim
        mitte = mitte & "#" & trennerInFileNames & clsString.kuerzeTextauf(
                            clsString.normalize_Filename(zeile.Item("BESCHREIBUNG").ToString, " "), limit).Replace(".", "").trim


        '    dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(zeile.Item("DATEINAMEEXT").ToString)
        '    dateiname = dateiname & trennerInFileNames & clsString.kuerzeTextauf(
        'clsString.normalize_Filename(zeile.Item("BESCHREIBUNG").ToString, " "), 50).Replace(".", "")

        'neu
        mitte = mitte.Replace("_", " ").Trim
        mitte = mitte.Replace("." & zeile.Item("TYP").ToString.ToUpper, "") 
        mitte = mitte.Replace("." & zeile.Item("TYP").ToString.ToLower, "")
        mitte = mitte.Replace("Email " & myGlobalz.sitzung.aktVorgangsID & " " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale, "")

        mitte = mitte.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ")
        Dim worte() As String = mitte.Split(" "c)
        mitte = ""

        capitalizeWorte(worte)
        buildMitteAusWorte(mitte, worte)
        
        dateiname = dateiname & " " & mitte.trim & "." & zeile.Item("TYP").ToString.ToLower.trim
        dateiname = dateiname.Replace("#", " ").trim

        Return dateiname.trim
    End Function

    Private Sub capitalizeWorte(ByVal worte As String())
        For Each wort In worte
            wort = clsString.Capitalize(wort)
        Next
    End Sub
    Sub Fotos_einzelObjekteErstellen(pfad_dokumente As String, vid As Integer, nomsg As Boolean)
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(vid, "nurfotos") ' nach myGlobalz.Arc.ArcRec
        If bresult Then
            Dim zielDatei, quelldatei As String
            Dim FIquelle As IO.FileInfo
            For Each zeile As DataRow In myGlobalz.Arc.ArcRec.dt.Rows
                zielDatei = GetDateinameDokumente(pfad_dokumente, zeile, 50)
                entfDoppelteTrenner(zielDatei)
                quelldatei = getQuelldatei(zeile)
                'If nomsg And isMSGoutlookdatei(quelldatei) Then
                '    Continue For
                'End If
                FIquelle = New IO.FileInfo(quelldatei)
                DokArc.ueberkopieren(quelldatei, zielDatei, FIquelle)
            Next
        End If
    End Sub

    Private Function getQuelldatei(zeile As DataRow) As String
        Dim quelle As String = myGlobalz.Arc.rootDir.ToString & zeile.Item("relativpfad").ToString.Replace("/", "\")
        quelle = quelle & "\" & zeile.Item("dateinameext").ToString
        nachricht("  quelle: " & quelle)
        Return quelle
    End Function

    Private Function isMSGoutlookdatei(quelldatei As String) As Boolean
        If quelldatei.Trim.ToLower.EndsWith(".msg") Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function istWordDatei(zielDatei As String) As Boolean
        If zielDatei.ToLower.Trim.EndsWith("docx") OrElse zielDatei.ToLower.Trim.EndsWith("doc") Then
            Return True
        End If
        Return False
    End Function

    Private Function istGISPDFkarte(aDok As DataRow) As Boolean
        Try
            Dim mname As String = aDok.Item("DATEINAMEEXT").ToString
            If mname.ToLower.Contains("_merge_paradigmapdf.pdf") Then
                Return True
            End If
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

End Module
