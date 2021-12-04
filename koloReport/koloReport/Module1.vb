

Imports System.Data
Imports koloReport

Module Module1
    Public tabBearbeiter As String = "t05"
    Public tabEreignis As String = "t16"
    Public ParadigmaDBtyp As String = "sqls" ' "sqls" '""oracle" '
    Public logf As IO.StreamWriter
    Public dossierordner As String
    Public aktdoku As Dokument
    Public view_vs_d As String =
   " (SELECT TOP (100) PERCENT s.ID, s.VORGANGSID, s.BEARBEITER, s.EINGANG, s.BESCHREIBUNG, s.BEMERKUNG, s.ERLEDIGT, s.LETZTEBEARBEITUNG, s.ORTSTERMIN, s.STELLUNGNAHME, " &
   "             s.LASTACTIONHEROE, s.ISTINVALID, s.PROBAUGAZ, s.GEMKRZ, s.AUFNAHME, s.ALTAZ, s.AZ2, s.WEITEREBEARB, s.ARCDIR, s.DARFNICHTVERNICHTETWERDEN, s.STORAUMNR, s.STOTITEL,  " &
   "             s.GUTACHTENMIT, s.GUTACHTENDRIN, s.ABGABEBA, s.PARAGRAF, s.HATRAUMBEZUG, s.INTERNENR, v.VORGANGSID AS mid, v.SACHGEBIETNR, v.VORGANGSNR, v.VORGANGSGEGENSTAND,  " &
   "             v.SACHGEBIETSTEXT, v.ISTUNB, v.AZ, v.TS ,s.BEARBEITERID " &
   " FROM   paradigma.dbo.t41 AS s INNER JOIN " &
   "             paradigma.dbo.t43 AS v ON s.VORGANGSID = v.VORGANGSID " &
   " ORDER BY s.LETZTEBEARBEITUNG DESC) "
    ' Public ausgabeDateiFormat As String = ".docx" ' oder .pdf
    Function koloReport(ereignisid As String,
                        vid As String,
                        fotosMitUnterTitel As Boolean,
                        anzahlfotosproseite As Integer,
                        mitGisKarte As String,
                        ausgabeDocx As String,
                        mitfotos As Boolean,
                        dokArcPfad As String,
                        scalierfaktor As Double,
                        zweispaltig As Boolean,
                        fototitelSize As String,
                        modusFotoUntertitel As String,
                        standardfontsize As String,
                        mitExtraZeilenumbruch As Boolean) As Boolean

        Dim jpgliste() As String = Nothing
        Dim dateiname() As String = Nothing
        Dim fotountertitel() As String = Nothing
        Dim kopftexte As String() = Nothing
        Dim erfolg As Boolean
        l("koloReport----------")
        l("mitGisKarte " & mitGisKarte)
        erfolg = holeTextundBildDatenAusOracle(vid, ereignisid, kopftexte, jpgliste, fotountertitel,
                                               dokArcPfad,
                                               modusFotoUntertitel,
                                               mitfotos, dateiname)
        If erfolg Then
            erfolg = ErzeugeAktennotizMitBildern(ausgabeDocx, jpgliste, fotountertitel, anzahlfotosproseite, fotosMitUnterTitel,
                                                 mitGisKarte, mitfotos, kopftexte, vid, scalierfaktor,
                                                 zweispaltig, fototitelSize, standardfontsize, mitExtraZeilenumbruch, dateiname)
            If erfolg Then
                l("koloReport---------- ende true")
                Return True
            End If
        End If
        l("koloReport---------- ende false")
        '  System.Diagnostics.Process.Start("winword", ausgabeDocx)
        Return False
    End Function
    Public Function FotoDokumentDatenHolen(eid As String) As DataTable
        Dim Sql As String
        Dim dt As New DataTable
        Try
            Sql = "SELECT * FROM dokumente where eid=  " & eid &
                  " and (upper(typ)='JPG' or upper(typ)='PNG') " &
                  " order by filedatum asc "
            'MsgBox(Sql)
            'If ParadigmaDBtyp = "oracle" Then
            '    dt = getDTOracle(Sql)
            'End If
            If ParadigmaDBtyp = "sqls" Then
                dt = clsSQLS.getDTSQLS(Sql)
            End If
            'MsgBox(dt.Rows.Count)
            l("nach FotoDokumentDatenHolen")
            Return dt
        Catch ex As Exception
            l("fehler in dokumentDatenHolen----------  e" & ex.ToString)
            Return Nothing
        End Try
    End Function
    Private Function holeTextundBildDatenAusOracle(vid As String,
                                                   ereignisid As String,
                                                  ByRef kopftexte() As String,
                                                  ByRef jpgliste() As String,
                                                  ByRef fotountertitel() As String,
                                                   dokArcPfad As String,
                                                   modusFotoUntertitel As String,
                                                   mitfotos As Boolean,
                                                   ByRef dateiname() As String) As Boolean
        Dim erfolg As Boolean
        Dim dokumentDT As DataTable
        Dim stammDT As DataTable
        Dim e_notiz As String
        Dim e_beschreibung As String
        Dim e_art As String
        Dim e_datum As String

        Dim s_Bearbeiter As String
        Dim s_BearbeiterID As String
        Dim s_az As String
        Dim s_Sachgebiet As String
        Dim s_beschreibung As String

        ReDim kopftexte(7)
        Try
            l("holeTextundBildDatenAusOracle----------  ")

            '  erfolg = bildeEreignisKopf
            If mitfotos Then
                dokumentDT = FotoDokumentDatenHolen(ereignisid)
            End If
            l("holeTextundBildDatenAusOracle----------  1")
            stammDT = kopftexteausStammdatenHolen(CInt(vid))
            l("holeTextundBildDatenAusOracle----------  2")
            s_az = LIBDB.clsDBtools.fieldvalue(stammDT.Rows(0).Item("az2"))
            l("holeTextundBildDatenAusOracle----------  3")
            If mitfotos Then
                erfolg = bildeliste(dokumentDT, jpgliste, fotountertitel, dokArcPfad, s_az, modusFotoUntertitel, dateiname)
            End If

            l("holeTextundBildDatenAusOracle----------  4")
            Dim ereignisDT As DataTable
            ereignisDT = kopftexteausEreignisHolen(CInt(ereignisid))
            l("holeTextundBildDatenAusOracle----------  5")

            If ereignisDT.Rows.Count > 0 Then
                e_notiz = LIBDB.clsDBtools.fieldvalue(ereignisDT.Rows(0).Item("notiz"))
                e_beschreibung = LIBDB.clsDBtools.fieldvalue(ereignisDT.Rows(0).Item("beschreibung"))
                e_art = LIBDB.clsDBtools.fieldvalue(ereignisDT.Rows(0).Item("art"))
                e_datum = CStr(CDate(LIBDB.clsDBtools.fieldvalue(ereignisDT.Rows(0).Item("datum"))).ToString("dd.MM.yyyy"))
            Else
                e_notiz = ""
                e_beschreibung = ""
                e_art = ""
                e_datum = ""
            End If

            If stammDT.Rows.Count > 0 Then
                s_Bearbeiter = LIBDB.clsDBtools.fieldvalue(stammDT.Rows(0).Item("bearbeiter"))
                s_BearbeiterID = LIBDB.clsDBtools.fieldvalue(stammDT.Rows(0).Item("bearbeiterID"))
                s_az = LIBDB.clsDBtools.fieldvalue(stammDT.Rows(0).Item("az2"))
                s_Sachgebiet = LIBDB.clsDBtools.fieldvalue(stammDT.Rows(0).Item("sachgebietstext"))
                s_beschreibung = LIBDB.clsDBtools.fieldvalue(stammDT.Rows(0).Item("beschreibung"))
            Else
                s_Bearbeiter = ""
                s_az = ""
                s_Sachgebiet = ""
                s_beschreibung = ""
            End If





            s_Bearbeiter = getVorzuNameVonBearbeiter(s_Bearbeiter, s_BearbeiterID)

            kopftexte(0) = s_Bearbeiter
            kopftexte(1) = s_az
            kopftexte(2) = s_Sachgebiet
            kopftexte(3) = s_beschreibung

            kopftexte(4) = e_datum
            kopftexte(5) = e_art
            kopftexte(6) = e_beschreibung
            kopftexte(7) = e_notiz

            l("holeTextundBildDatenAusOracle----------  ende")
            Return True
        Catch ex As Exception
            l("fehler in holeTextundBildDatenAusOracle: " & ex.ToString)
            Return False
        End Try
    End Function



    Private Function bildeliste(dokumentDT As DataTable,
                                ByRef jpgliste() As String,
                                ByRef fotountertitel() As String,
                                dokArcPfad As String,
                                az As String,
                                modusFotoUntertitel As String,
                                ByRef dateiname() As String) As Boolean
        Dim pfad As String = ""
        ReDim jpgliste(dokumentDT.Rows.Count - 1)
        ReDim fotountertitel(dokumentDT.Rows.Count - 1)
        ReDim dateiname(dokumentDT.Rows.Count - 1)
        Dim temponame As String
        Try
            l("bildeliste----------  ")
            For i = 0 To dokumentDT.Rows.Count - 1
                pfad = dokArcPfad & pfad & "\" & CType(dokumentDT.Rows(i).Item("relativpfad"), String)
                jpgliste(i) = getDateiFullname(dokumentDT, pfad, i, temponame)
                dateiname(i) = temponame
                fotountertitel(i) = getFotoBeschreibung(dokumentDT, i, az, modusFotoUntertitel)
                pfad = ""
            Next
            l("bildeliste----------  ende")
            Return True
        Catch ex As Exception
            l("fehler in bildeliste: " & ex.ToString)
            Return False
        End Try

    End Function

    Private Function getFotoBeschreibung(dt As DataTable, i As Integer,
                                         az As String,
                                         modus As String) As String
        Dim result As String
        l("getFotoBeschreibung----------  ")
        If modus = "lang" Then
            result = CType(LIBDB.clsDBtools.fieldvalue(dt.Rows(i).Item("dateinameext")), String) & " " &
                CType(LIBDB.clsDBtools.fieldvalue(dt.Rows(i).Item("exifhersteller")), String) & " " &
                CType(LIBDB.clsDBtools.fieldvalue(dt.Rows(i).Item("filedatum")), String) & " " &
                CType(LIBDB.clsDBtools.fieldvalue(dt.Rows(i).Item("Beschreibung")), String)
        End If
        If modus = "kurz" Then
            result = CType(LIBDB.clsDBtools.fieldvalue(dt.Rows(i).Item("filedatum")), String) & " " &
               az
        End If

        result = CType(LIBDB.clsDBtools.fieldvalue(dt.Rows(i).Item("filedatum")), String) & " " &
                 az
        result = result.Replace(" ", "_")
        result = result.Replace(".", "_")
        result = result.Replace(",", "_")
        l(result)
        l("getFotoBeschreibung----------  e")
        Return result
    End Function

    Private Function getDateiFullname(dt As DataTable, pfad As String, i As Integer, ByRef dateiname As String) As String
        Dim result As String
        l("getDateiFullname----------  ")
        dateiname = CType(dt.Rows(i).Item("dateinameext"), String).ToLower
        result = (pfad & "\" & CType(dt.Rows(i).Item("dokumentid"), String)).Replace("/", "\").Replace("archiv\\", "archiv\").Replace(".jpg", "").Replace(".JPG", "")
        result = result.Replace(".png", "").Replace(".PNG", "")
        l(result)
        l("getDateiFullname----------  e")
        Return result
    End Function

    Private Function kopftexteausEreignisHolen(eid As Integer) As DataTable
        'Dim erfolg As Boolean
        Dim dt As DataTable
        Dim Sql As String
        Try
            l("kopftexteausEreignisHolen")
            Sql = "SELECT * FROM " & tabEreignis & " where id=  " & eid
            'If ParadigmaDBtyp = "oracle" Then
            '    dt = getDTOracle(Sql)
            'End If
            If ParadigmaDBtyp = "sqls" Then
                dt = clsSQLS.getDTSQLS(Sql)
            End If
            l("nach getDT")
            Return dt
        Catch ex As Exception
            l("fehler in kopftexteausEreignisHolen----------  e" & ex.ToString)
            Return Nothing
        End Try
    End Function
    Private Function kopftexteausStammdatenHolen(vid As Integer) As DataTable
        'Dim erfolg As Boolean
        Dim dt As DataTable
        Dim Sql As String
        Try
            l("kopftexteausStammdatenHolen")
            Sql = "SELECT * FROM " & view_vs_d & " vs where   vorgangsid=  " & vid
            'If ParadigmaDBtyp = "oracle" Then
            '    dt = getDTOracle(Sql)
            'End If
            If ParadigmaDBtyp = "sqls" Then
                dt = clsSQLS.getDTSQLS(Sql)
            End If
            l("nach getDT")
            Return dt
        Catch ex As Exception
            l("fehler in kopftexteausStammdatenHolen----------  e" & ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Function getVorzuNameVonBearbeiter(s_Bearbeiter As String, bearbeiterid As String) As String
        'Dim erfolg As Boolean
        Dim dt As DataTable
        Dim Sql As String = ""
        Try
            l("getVorzuNameVonBearbeiter---------------")
            Sql = "SELECT nachname + ', ' + vorname FROM " & tabBearbeiter & " where   bearbeiterid= " & bearbeiterid.ToLower & ""
            l(Sql)
            'If s_Bearbeiter.Length = 2 Then
            '    Sql = "SELECT nachname + ', ' + vorname FROM " & tabBearbeiter & " where   lower(KUERZEL1)=  '" & s_Bearbeiter.ToLower & "'"
            'Else
            '    Sql = "SELECT nachname + ', ' + vorname FROM  " & tabBearbeiter & " where   lower(initial_)=  '" & s_Bearbeiter.ToLower & "'"
            'End If
            l("sql=" & Sql)
            'If ParadigmaDBtyp = "oracle" Then
            '    dt = getDTOracle(Sql)
            'End If
            If ParadigmaDBtyp = "sqls" Then
                dt = clsSQLS.getDTSQLS(Sql)
            End If
            l("nach getDT")
            l("getVorzuNameVonBearbeiter normal ende")
            l(LIBDB.clsDBtools.fieldvalue(dt.Rows(0).Item(0)).ToString)
            Return LIBDB.clsDBtools.fieldvalue(dt.Rows(0).Item(0)).ToString
        Catch ex As Exception
            l("fehler in getVorzuNameVonBearbeiter: " & Sql & Environment.NewLine & ex.ToString)
            Return Nothing
        End Try
    End Function
    Public Function insarchivDamit(ausgabeDocx As String, vid As String, ereignisid As String,
                                   dokArcPfad As String, beschreibung As String, bearbeiterid As String) As String
        Dim hinweis As String = ""
        l("insarchivDamit-----------------------------")
        'dokument bilden
        If Not bildeDokumentInstanz(ausgabeDocx, vid, ereignisid, beschreibung) Then 'aktdoku
            hinweis = "Fehler beim dokumentbilden" : Return hinweis
        End If
        'erst datenbank
        l("vor checkin")
        aktdoku.bearbeiterid = CInt(bearbeiterid)
        If aktdoku.Beschreibung = "" Then aktdoku.Beschreibung = beschreibung
        'If ParadigmaDBtyp = "oracle" Then
        '    aktdoku.DocID = modOracle.checkInDBDokuOracle(aktdoku)
        'End If
        If ParadigmaDBtyp = "sqls" Then
            aktdoku.DocID = clsSQLS.checkInDBDokuSQLS(aktdoku)
        End If
        If aktdoku.DocID < 1 Then
            hinweis &= "Fehler beim speichern in datenbank" : Return hinweis
        End If
        'datei im Archiv erzeugen 
        Dim erfolg As Boolean
        erfolg = DateiImArchivspeichern(ausgabeDocx, aktdoku, dokArcPfad)
        l("insarchivDamit-----------------fertif-----")
        Return "ok"
    End Function

    Private Function DateiImArchivspeichern(ausgabeDocx As String, aktdoku As Dokument, dokArcPfad As String) As Boolean
        'dateinameImarchiv
        l("DateiImArchivspeichern----------------------")
        Dim archivdatei As String
        archivdatei = aktdoku.makeFullname_ImArchiv(New IO.DirectoryInfo(dokArcPfad))
        If Datei_moven(ausgabeDocx, archivdatei) Then
            l("DateiImArchivspeichern-----------------fertif-----")
            Return True
        Else
            l("fehler bei moven der datei ins archiv")
        End If
        l("DateiImArchivspeichern-----------------fertif-----")
        Return False
    End Function

    Private Function Datei_moven(ausgabeDocx As String, archivdatei As String) As Boolean
        Dim fi As IO.FileInfo
        Try
            l("Datei_moven----------------------")
            fi = New IO.FileInfo(ausgabeDocx)
            fi.CopyTo(archivdatei)
            Return True
        Catch ex As Exception
            l("fehler in " & ex.ToString)
            Return False
        End Try
    End Function

    Private Function bildeDokumentInstanz(ausgabeDocx As String, vid As String, ereignisid As String, beschreibung As String) As Boolean
        Dim stammDT As DataTable
        Try
            l("bildeDokumentInstanz-----------------------------")
            aktdoku.VorgangsID = CInt(vid)
            aktdoku.EreignisID = CInt(ereignisid)
            stammDT = kopftexteausStammdatenHolen(CInt(vid))
            aktdoku.dokumentPfad = LIBDB.clsDBtools.fieldvalue(stammDT.Rows(0).Item("arcdir"))
            aktdoku.Beschreibung = beschreibung '& ereignisid
            aktdoku.Checkindatum = Now
            Dim fi As New IO.FileInfo(ausgabeDocx)
            aktdoku.DateinameMitExtension = fi.Name
            fi = Nothing
            aktdoku.DokTyp = DokumentenTyp.DOC
            aktdoku.Filedatum = Now
            aktdoku.Initiale = Environment.UserName.Substring(0, 3) &
                           Environment.UserName.Substring(Environment.UserName.Length - 1, 1)
            aktdoku.newSaveMode = True
            aktdoku.OriginalFullname = ausgabeDocx
            aktdoku.OriginalName = aktdoku.DateinameMitExtension
            aktdoku.revisionssicher = False
            aktdoku.Typ = "docx"
            l("bildeDokumentInstanz-----------------------------ende")
            Return True
        Catch ex As Exception
            l("fehler in bildeDokumentInstanz" & ex.ToString)
            Return False
        End Try
    End Function

    Public Function erzeugePDFA(quelldokument As String, zielDatei As String) As Boolean
        Try
            Dim lw = New LIBwordvorlage.WordReplaceTextmarken()
            If lw.dok2pdfA(quelldokument, zielDatei) Then
                lw = Nothing
                Return True
            End If
            lw = Nothing
            Return False
        Catch ex As Exception
            l("fehler in: erzeugePDFA " & ex.ToString)
            Return False
        End Try
    End Function
    Sub MeinGarbage()
        GC.Collect()
        GC.WaitForPendingFinalizers()
    End Sub
    Public Function NeuesEreigniserzeugen(ueberschrift As String) As clsEreignis
        Dim aktereignis As New clsEreignis
        aktereignis.clearValues()
        With aktereignis
            .Datum = Now
            .Art = "Aktennotiz"
            .Richtung = ""
            .Beschreibung = ueberschrift
            '.Notiz = "Hinweis: Diese Datei wird nicht automatisch im Archiv gespeichert. Wenn Sie dies Wünschen: 1. Schliessen sie die Datei und 2. Betätigen Sie den Knopf: 'Ins Archiv übernehmen'"
            .Notiz = ""
            .typnr = 1
            'todo hier kann man mehr machen
        End With
        Return aktereignis
    End Function
    ''' <summary> 
    ''' <para>Creates a log-string from the Exception.</para>
    ''' <para>The result includes the stacktrace, innerexception et cetera, separated by <seealso cref="Environment.NewLine"/>.</para>
    ''' </summary>
    ''' <param name="ex">The exception to create the string from.</param>
    ''' <param name="additionalMessage">Additional message to place at the top of the string, maybe be empty or null.</param>
    ''' <returns></returns>
    <System.Runtime.CompilerServices.Extension()>
    Public Function ToLogString(ByVal ex As Exception, ByVal additionalMessage As String) As String
        Dim msg As New Text.StringBuilder()

        If Not String.IsNullOrEmpty(additionalMessage) Then
            msg.Append(additionalMessage)
            msg.Append(Environment.NewLine)
        End If

        If ex IsNot Nothing Then
            Try
                Dim orgEx As Exception = ex
                msg.Append("Exception:")
                msg.Append(Environment.NewLine)
                While orgEx IsNot Nothing
                    msg.Append("Message: " & orgEx.Message)
                    msg.Append(Environment.NewLine)
                    orgEx = orgEx.InnerException
                End While

                If ex.Data IsNot Nothing Then
                    For Each i As Object In ex.Data
                        msg.Append("Data :")
                        msg.Append(i.ToString())
                        msg.Append(Environment.NewLine)
                    Next
                End If

                If ex.StackTrace IsNot Nothing Then
                    msg.Append("StackTrace:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.StackTrace.ToString())
                    msg.Append(Environment.NewLine)
                End If

                If ex.Source IsNot Nothing Then
                    msg.Append("Source:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.Source)
                    msg.Append(Environment.NewLine)
                End If

                If ex.TargetSite IsNot Nothing Then
                    msg.Append("TargetSite:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.TargetSite.ToString())
                    msg.Append(Environment.NewLine)
                End If

                Dim baseException As Exception = ex.GetBaseException()
                If baseException IsNot Nothing Then
                    msg.Append("BaseException:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.GetBaseException())
                End If
            Finally
            End Try
        End If
        Return msg.ToString()
    End Function

End Module
