Imports System.Data
Module expDokumente
    Sub dokumente_CsvListeErstellen(datei As String)
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(myGlobalz.sitzung.aktVorgangsID), "keinefotos", True, 0) ' nach myGlobalz.Arc.ArcRec
        If bresult Then
            Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Dokumente", myGlobalz.Arc.ArcRec.dt,
                                                     myGlobalz.sitzung.aktVorgangsID,
                                                    datei, CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
            nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
            handcsv.Dispose()
        End If
    End Sub

    Sub Dokumente_einzelObjekteErstellen(pfad_dokumente As String,
                                         nomsg As Boolean,
                                         pfad_raumbezug As String,
                                         msg2html As Boolean,
                                         word2PDF As Boolean)
        Dim zielDatei, quelldatei As String
        Dim FIquelle As IO.FileInfo
        Dim kompress As Boolean = False
        '.ToString("yyyy-MM-dd_HH_mm_ss")
        For Each aDok As DataRow In myGlobalz.Arc.ArcRec.dt.Rows
            zielDatei = GetDateinameDokumente(pfad_dokumente, aDok, 50)
            kompress = CBool(aDok.Item("KOMPRESS"))

            kompress = False
            If istGISPDFkarte(aDok) Then
                zielDatei = GetDateinameRaumbezug(pfad_raumbezug, aDok)
            End If
            entfDoppelteTrenner(zielDatei)
            quelldatei = getQuelldatei(aDok)
            FIquelle = New IO.FileInfo(quelldatei)
            'If nomsg And isMSGoutlookdatei(quelldatei) Then
            '    '  Continue For
            'End If
            DokArc.ueberkopierenNachCheckout(zielDatei, FIquelle, kompress)
            FIquelle = Nothing
            If istWordDatei(zielDatei) Then
                If word2PDF Then
                    FileArchivTools.inputFileReadonlyEntfernen(zielDatei)
                    'Dim lw = New WordReplaceTextmarken()
                    If wordInterop.dok2pdf(zielDatei) Then
                        IO.File.Delete(zielDatei)
                    End If
                Else
                    'kein delete
                End If
            End If

            If isMSGoutlookdatei(zielDatei) Then
                If msg2html Then
                    Dim outfile As String
                    outfile = zielDatei.Trim.Replace(".msg", "").Replace(".MSG", "") & ".html"
                    If MeinOutlook.msg2html(zielDatei, outfile) Then
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
        dateiname = dateiname & LIBgemeinsames.clsString.date2string(CDate(zeile.Item("FILEDATUM")), 3).Trim
        dateiname = dateiname & trennerInFileNames & LIBgemeinsames.clsString.kuerzeTextauf(
            LIBgemeinsames.clsString.normalize_Filename(zeile.Item("BESCHREIBUNG").ToString, " "), 50).Replace(".", "")
        dateiname = dateiname & trennerInFileNames & LIBgemeinsames.clsString.normalize_Filename(zeile.Item("DATEINAMEEXT").ToString)
        Return dateiname
    End Function
    Private Sub buildMitteAusWorte(ByRef mitte As String, ByVal worte As String())
        For Each wort In worte
            mitte = mitte & wort
        Next
    End Sub
    'Public Shared GetDateinameDokumenteID()
    Private Function GetDateinameDokumente(pfad_dokumente As String, zeile As DataRow, limit As Int16) As String
        Dim dateiname, mitte As String
        'Dim NewSaveMode As Boolean = CBool(zeile.Item("NEWSAVEMODE")) 
        dateiname = (pfad_dokumente & "\").Trim
        dateiname = dateiname & LIBgemeinsames.clsString.date2string(CDate(zeile.Item("FILEDATUM")), 3).Trim
        mitte = trennerInFileNames & LIBgemeinsames.clsString.normalize_Filename(zeile.Item("DATEINAMEEXT").ToString).Trim
        mitte = mitte & "#" & trennerInFileNames & LIBgemeinsames.clsString.kuerzeTextauf(
                                                        LIBgemeinsames.clsString.normalize_Filename(zeile.Item("BESCHREIBUNG").ToString, " "),
                                                        limit).Replace(".", "").Trim
        'neu
        mitte = mitte.Replace("|", "_").Trim
        mitte = mitte.Replace("_", " ").Trim
        mitte = mitte.Replace("." & zeile.Item("TYP").ToString.ToUpper, "")
        mitte = mitte.Replace("." & zeile.Item("TYP").ToString.ToLower, "")
        mitte = mitte.Replace("Email " & myGlobalz.sitzung.aktVorgangsID & " " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale, "")

        mitte = mitte.Replace("  ", " ").Replace("  ", " ").Replace("  ", " ")
        Dim worte() As String = mitte.Split(" "c)
        mitte = ""

        capitalizeWorte(worte)
        buildMitteAusWorte(mitte, worte)

        dateiname = dateiname & "_" & mitte.Trim & "." & zeile.Item("TYP").ToString.ToLower.Trim
        dateiname = dateiname.Replace("#", "").Trim

        Return dateiname.Trim
    End Function

    Private Sub capitalizeWorte(ByVal worte As String())
        For Each wort In worte
            wort = LIBgemeinsames.clsString.Capitalize(wort)
        Next
    End Sub
    Sub Fotos_einzelObjekteErstellen(pfad_dokumente As String, vid As Integer, nomsg As Boolean)
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(vid), "nurfotos", True, 0) ' nach myGlobalz.Arc.ArcRec
        If bresult Then
            Dim zielDatei, quelldatei As String
            Dim FIquelle As IO.FileInfo
            Dim kompress As Boolean = False
            For Each zeile As DataRow In myGlobalz.Arc.ArcRec.dt.Rows
                zielDatei = GetDateinameDokumente(pfad_dokumente, zeile, 50)
                entfDoppelteTrenner(zielDatei)
                quelldatei = getQuelldatei(zeile)
                kompress = CBool(zeile.Item("kompress"))
                kompress = False
                'If nomsg And isMSGoutlookdatei(quelldatei) Then
                '    Continue For
                'End If
                FIquelle = New IO.FileInfo(quelldatei)
                DokArc.ueberkopierenNachCheckout(zielDatei, FIquelle, kompress)
                FIquelle = Nothing
            Next
        End If
    End Sub

    Private Function getQuelldatei(zeile As DataRow) As String

        Dim quelle As String
        quelle = myGlobalz.Arc.rootDir.ToString & zeile.Item("relativpfad").ToString.Replace("/", "\")
        Dim NewSaveMode As Boolean = CBool(zeile.Item("NEWSAVEMODE"))
        If NewSaveMode Then
            quelle = quelle & "\" & zeile.Item("DOKUMENTID").ToString
        Else
            quelle = quelle & "\" & zeile.Item("dateinameext").ToString
        End If
        '   Dim quelle As String = myGlobalz.Arc.rootDir.ToString & zeile.Item("relativpfad").ToString.Replace("/", "\")
        '  quelle = quelle & "\" & zeile.Item("dateinameext").ToString
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
        If zielDatei.ToLower.Trim.EndsWith("docx") OrElse
            zielDatei.ToLower.Trim.EndsWith("rtf") OrElse
            zielDatei.ToLower.Trim.EndsWith("doc") Then
            Return True
        End If
        Return False
    End Function
    Public Function istWordDatei(dok As Dokument) As Boolean
        If dok.DokTyp = DokumentenTyp.DOC Or dok.DokTyp = DokumentenTyp.RTF Then
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
