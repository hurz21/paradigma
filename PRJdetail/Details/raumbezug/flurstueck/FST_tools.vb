Imports System.Data

Public Class FST_tools



    Public Shared Sub RB_Und_Vorgang_Entkoppeln(ByVal erfolg%)
        My.Log.WriteEntry("RB_Und_Vorgang_Entkoppeln")
        erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myglobalz.sitzung.aktFST.RaumbezugsID), myglobalz.sitzung.aktVorgangsID)

        If erfolg > 0 Then
            My.Log.WriteEntry("Objekt wurde erfolgreich gelöscht")
        Else
            My.Log.WriteEntry("Objekt wurde erfolgreich gelöscht")
            nachricht_und_Mbox("Problem beim Löschen des Raumbezugs aus dem Vorgang. Abbruch.")
        End If
    End Sub

    Public Shared Sub loeschenRBFlurstueckExtracted()
        nachricht("loeschenRBFlurstueckExtracted --------------------------------------------------")
        Dim erfolg As Integer
        If CInt(myglobalz.sitzung.aktFST.Status) = 0 Then
            erfolg = FST_DBTool_Oracle.RB_Flurstueck_loeschen(CInt(myglobalz.sitzung.aktFST.SekID))
            If erfolg <= 0 Then
                nachricht("Problem beim Löschen der Raumbezugs-Flurstücks. Abbruch.")
            End If
            erfolg = RBtoolsns.Raumbezug_loeschen_byid_alleDB.execute(CInt(myglobalz.sitzung.aktFST.RaumbezugsID))
            If erfolg <= 0 Then
                My.Log.WriteEntry("Problem beim Löschen des Raumbezugs . Abbruch.")
                nachricht_und_Mbox("Problem beim Löschen des Raumbezugs. Abbruch.")
            End If
            FST_tools.RB_Und_Vorgang_Entkoppeln(erfolg)
            ' FST_serialShape_mysql.RB_Flurstueck_Serial_loeschen(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
            If RBtoolsns.RB_Flurstueck_Serial_loeschen.exe(CInt(myglobalz.sitzung.aktFST.RaumbezugsID)) < 0 Then
                nachricht("Fehler oder keine RBs vorhanden: vorgang: " & myglobalz.sitzung.aktVorgangsID & " rbsekid:" & myglobalz.sitzung.aktFST.SekID)
            End If
        Else
            FST_tools.RB_Und_Vorgang_Entkoppeln(erfolg)
        End If
    End Sub

    Public Shared Sub flurstueck_speichernExtracted(ByVal Radius As Double, isMapEnabled As Boolean)
        myglobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Flurstueck

        myGlobalz.sitzung.aktFST.typ = RaumbezugsTyp.Flurstueck
        myGlobalz.sitzung.aktFST.isMapEnabled = isMapEnabled
        myglobalz.sitzung.aktFST.normflst.fstueckKombi = myglobalz.sitzung.aktFST.normflst.buildFstueckkombi()
        NeuesFSTspeichern(Radius)
        altesFSTspeichern(Radius)
        AdressTools.setzeeNeuesGemKRZ(myglobalz.sitzung)
        '   myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = Now
        myglobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
        detailsTools.Edit_singleUpdate_Stammdaten(myglobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
        nachricht("hatraumbezug imn stammdaten setzen!!")
        '    glob2.EDIT_VorgangStamm_2DBOk()
        'If AdressTools.setzeeNeuesGemKRZ(myGlobalz.sitzung) Then
        '    glob2.EDIT_VorgangStamm_2DBOk()
        'End If
    End Sub


    Public Shared Function RBFlstNeu_alleDB() As Integer
        Dim result As Integer = 0
        Dim querie As String
        '  werteDBsicherMachenEreignis(ereignis)
        clsSqlparam.paramListe.Clear()
        '   populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        querie = "INSERT INTO " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " (GEMCODE,FLUR,ZAEHLER,NENNER,ZNKOMBI,GEMARKUNGSTEXT,FS,FLAECHEQM) " +
                            " VALUES (@GEMCODE,@FLUR,@ZAEHLER,@NENNER,@ZNKOMBI,@GEMARKUNGSTEXT,@FS,@FLAECHEQM)"
        populateParamsRbFlst()
        result = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")

        Return result
    End Function

    Private Shared Sub populateParamsRbFlst()
        clsSqlparam.paramListe.Add(New clsSqlparam("GEMCODE", myglobalz.sitzung.aktFST.normflst.gemcode))
        clsSqlparam.paramListe.Add(New clsSqlparam("FLUR", myglobalz.sitzung.aktFST.normflst.flur))
        clsSqlparam.paramListe.Add(New clsSqlparam("ZAEHLER", myglobalz.sitzung.aktFST.normflst.zaehler))
        clsSqlparam.paramListe.Add(New clsSqlparam("NENNER", myglobalz.sitzung.aktFST.normflst.nenner))
        clsSqlparam.paramListe.Add(New clsSqlparam("ZNKOMBI", myglobalz.sitzung.aktFST.normflst.fstueckKombi))
        clsSqlparam.paramListe.Add(New clsSqlparam("GEMARKUNGSTEXT", myglobalz.sitzung.aktFST.normflst.gemarkungstext))
        clsSqlparam.paramListe.Add(New clsSqlparam("FS", myglobalz.sitzung.aktFST.normflst.FS))
        clsSqlparam.paramListe.Add(New clsSqlparam("FLAECHEQM", myglobalz.sitzung.aktFST.normflst.flaecheqm))
    End Sub

    Public Shared Function RBFlstEdit_alleDB(ByVal sekid As Integer) As Integer
        Dim result As Integer = 0
        Dim querie As String
        Try
            l(" RBFlstEdit_alleDB ---------------------- anfang")
            '  werteDBsicherMachenEreignis(ereignis)
            clsSqlparam.paramListe.Clear()
            '   populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
            'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "UPDATE " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " & " SET " &
                        " GEMCODE=@GEMCODE" &
                        ",FLUR=@FLUR" &
                        ",ZAEHLER=@ZAEHLER" &
                        ",NENNER=@NENNER" &
                        ",ZNKOMBI=@ZNKOMBI" &
                        ",GEMARKUNGSTEXT=@GEMARKUNGSTEXT" &
                        ",FLAECHEQM=@FLAECHEQM" &
                        ",FS=@FS" &
                        " WHERE ID=@ID"
            populateParamsRbFlst()
        clsSqlparam.paramListe.Add(New clsSqlparam("ID", sekid))
        result = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")

            If result < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myglobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(result)
            End If
            l(" RBFlstEdit_alleDB ---------------------- ende")
            Return result
        Catch ex As Exception
            l("Fehler in RBFlstEdit_alleDB: " ,ex)
            Return -1
        End Try
    End Function
    Public Shared Sub NeuesFSTspeichern(ByVal Radius As Double)

        If myglobalz.sitzung.raumbezugsmodus = "neu" Then
            'Dim sekID% = FST_DBTool_mysql.RB_FLST_abspeichern_Neu()
            Dim sekid% = RBFlstNeu_alleDB()
            If sekid > 0 Then
                myglobalz.sitzung.aktFST.SekID = sekid
                myglobalz.sitzung.aktFST.defineAbstract()
                DBraumbezug_Mysql.defineBBOX(Radius, myglobalz.sitzung.aktFST)
                Dim raumbezugsID% = RBtoolsns.Raumbezug_abspeichern_Neu_alleDB.execute(myGlobalz.sitzung.aktFST)
                nachricht("kooplungsid: " & RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(raumbezugsID, myglobalz.sitzung.aktVorgangsID, 0).ToString)
                myGlobalz.sitzung.aktFST.RaumbezugsID = raumbezugsID
                glob2.generateAndSaveSerialShapeInDb()
            Else
                nachricht("NeuesFSTspeichern: Problem beim Abspeichernd!")
            End If
        End If
    End Sub

    Private Shared Sub altesFSTspeichern(ByVal Radius As Double)
        If myglobalz.sitzung.raumbezugsmodus = "edit" Then
            myglobalz.sitzung.aktFST.defineAbstract()
            DBraumbezug_Mysql.defineBBOX(Radius, myglobalz.sitzung.aktFST)
            'nachricht("SekID: " & FST_DBTool_mysql.RB_FLST_abspeichern_Edit(CInt(myGlobalz.sitzung.aktFST.SekID)).ToString)
            nachricht("SekID: " & RBFlstEdit_alleDB(CInt(myglobalz.sitzung.aktFST.SekID)).ToString)

            RBtoolsns.Raumbezug_edit_alleDB.execute(CInt(myglobalz.sitzung.aktFST.RaumbezugsID), myglobalz.sitzung.aktFST)
            'DBraumbezug_Mysql.Raumbezug_abspeichern_Edit(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID), myGlobalz.sitzung.aktFST)

            'rbid festellen : myGlobalz.sitzung.aktFST.RaumbezugsID
            'altes serialshap löschen
            ' FST_serialShape_mysql.RB_Flurstueck_Serial_loeschen(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))


            If RBtoolsns.RB_Flurstueck_Serial_loeschen.exe(CInt(myglobalz.sitzung.aktFST.RaumbezugsID)) < 0 Then
                nachricht("Fehler oder keine RBs vorhanden: vorgang: " & myglobalz.sitzung.aktVorgangsID &
                                " rbsekid:" & myglobalz.sitzung.aktFST.SekID & " rbid: " & myglobalz.sitzung.aktFST.RaumbezugsID)
            End If
            'dann komplett neu anlegen
            glob2.generateAndSaveSerialShapeInDb()

        End If
    End Sub


    ''' <summary>
    ''' enthält NICHT den bezug zum ALB!!! gebucht....
    ''' </summary>
    ''' <param name="afst"></param>
    ''' <param name="meinDT"></param>
    Public Shared Sub BASIS_vonDTnachObjekt(ByRef afst As ParaFlurstueck, ByVal meinDT As DataTable)
        Try
            With afst
                .Gid = CInt(meinDT.Rows(0).Item("gid"))
                .normflst.zeigtauf = CStr((meinDT.Rows(0).Item("zeigtauf"))).Trim
                .normflst.weistauf = CStr((meinDT.Rows(0).Item("weistauf"))).Trim
                .normflst.gebucht = CStr((meinDT.Rows(0).Item("istgebucht"))).Trim
                .normflst.flaecheqm = CDbl(CStr(meinDT.Rows(0).Item("gisarea")).Replace(".", ","))
                .normflst.fsgml = CStr((meinDT.Rows(0).Item("fsgml"))).Trim
                .normflst.FS = CStr((meinDT.Rows(0).Item("fs"))).Trim
            End With
        Catch ex As Exception
            nachricht("FEHLER in BASIS_vonDTnachObjekt: " ,ex)
        End Try
    End Sub

    Public Shared Sub hole_FSTKoordinaten_undZuweisePunkt(ByVal afst As ParaFlurstueck)
        'POSTGIS!!!
        Try
            DB_Oracle_sharedfunctions.hole_FSTKoordinatenExtracted(afst.normflst.FS)
            If Not myglobalz.sitzung.postgresREC.dt.IsNothingOrEmpty Then
                BASIS_vonDTnachObjekt(afst, myglobalz.sitzung.postgresREC.dt)
                myglobalz.sitzung.aktFST.FlaecheQm = CDbl(CStr(myglobalz.sitzung.postgresREC.dt.Rows(0).Item("gisarea")).Replace(".", ","))

                Dim aktBOX As String = NSpostgis.clsPostgis.holeKoordinatenFuerGID(afst.Gid, "basis_f", "flurkarte", False)
                myglobalz.sitzung.aktPolygon.box.BBOX = aktBOX
                myglobalz.sitzung.aktPolygon.box.bbox_split()
                myglobalz.sitzung.aktPolygon.box.CalcCenter()
                afst.punkt.X = myglobalz.sitzung.aktPolygon.box.xcenter
                afst.punkt.Y = myglobalz.sitzung.aktPolygon.box.ycenter
            Else
                afst.punkt.X = 0
                afst.punkt.Y = 0
            End If
        Catch ex As Exception
            afst.punkt.X = 0
            afst.punkt.Y = 0
            afst.normflst.FS = ""
            nachricht_und_Mbox("Fehler1 bei der Er  (=:0)")
        End Try
    End Sub

    Public Shared Sub eigentuemerALKISALT(ByVal afst As ParaFlurstueck, insArchiv As Boolean, mitEreignis As Boolean, EreignisLocationAbstract As String) '
        Dim dateisystemPDF As String = "", archivfullname As String = ""
        If Not FST_tools.sindFlurstuecksDatenVollstaendig(afst.normflst) Then
            FST_tools.hole_FSTKoordinaten_undZuweisePunkt(afst)
        End If
        Dim etools As New Eigentuemer_Tools.RTF(afst.normflst)
        Dim neuereignis As New clsEreignis

        Dim erfolg As Boolean = etools.send_Shellbatch_EIGENTUEMER(String.Format("eigentuemer_alkis_{0}_.rtf", myGlobalz.sitzung.aktBearbeiter.username),
                                                                    dateisystemPDF,
                                                                    myGlobalz.sitzung.aktBearbeiter.username,
                                                                    myGlobalz.sitzung.aktVorgangsID)
        System.Threading.Thread.Sleep(20000)
        If erfolg Then
            If mitEreignis Then
                initEreignis(neuereignis, "Eigentümer: " & EreignisLocationAbstract)
                clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)
                CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
                    myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
            End If
            If insArchiv Then
                Dim numdir As String = ""
                myGlobalz.sitzung.aktDokument.newSaveMode = True
                erfolg = myGlobalz.Arc.checkINDoku(dateisystemPDF,
                                                   "Eigentümer: " & EreignisLocationAbstract,
                                                   neuereignis.ID,
                                                   False,
                                                   archivfullname,
                                                   numdir,
                                                   myGlobalz.sitzung.aktVorgangsID,
                                                   False,
                                                   Now,
                                                   myGlobalz.sitzung.aktDokument.DocID,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                                   myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID)
                If erfolg Then
                    MsgBox(glob2.getMsgboxText("eigentuemerAuskunft", New List(Of String)(New String() {})), MsgBoxStyle.OkOnly, "Eigentümerauskunft als RTF-Datei")
                End If
                glob2.OpenDocument(dateisystemPDF)
            Else
                glob2.OpenDocument(dateisystemPDF)
            End If
        Else
            MsgBox("Es konnte keine RTF-Datei erstellt werden!!")
        End If
    End Sub
    Shared Function erzeugeUndOeffneEigentuemerPDF(text As String, afst As ParaFlurstueck) As String
        Dim lokalitaet As String
        lokalitaet = getlokalitaetstring(afst)
        Dim ausgabedatei As String = calcEigentuemerAusgabeFile()

        wrapItextSharp.createSchnellEigentuemer(text, ausgabedatei, glob2.albverbotsString, lokalitaet)
        Return ausgabedatei
    End Function

    Friend Shared Function calcEigentuemerAusgabeFile() As String
        Dim EigentuemerPDF As String
        Dim ausgabeDIR As String ' = My.Computer.FileSystem.SpecialDirectories.Temp '& "" & aid
        Try
            l("calcEigentuemerAusgabeFile---------------------- anfang")
            ausgabeDIR = System.IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.MyDocuments, "Eigentuemer")
            l("ausgabeDIR anlegen " & ausgabeDIR)
            IO.Directory.CreateDirectory(ausgabeDIR)
            l("calcEigentuemerAusgabeFile---------------------- ende")
            EigentuemerPDF = ausgabeDIR & "\eigentuemer" & Format(Now, "ddMMyyyy_hhmmss") & ".pdf"
            l("EigentuemerPDF " & EigentuemerPDF)
            Return EigentuemerPDF
        Catch ex As Exception
            l("Fehler in calcEigentuemerAusgabeFile: " ,ex)
            Return ""
        End Try
    End Function
    Shared Function getlokalitaetstring(aktFST As ParaFlurstueck) As String
        Dim summe As String = ""
        Dim trenner As String = ", "
        aktFST.normflst.fstueckKombi = aktFST.normflst.buildFstueckkombi()
        summe = summe & aktFST.normflst.gemarkungstext & trenner
        summe = summe & "Flur: " & aktFST.normflst.flur & trenner
        summe = summe & "Flurstueck: " & aktFST.normflst.fstueckKombi & trenner
        Return summe
    End Function
    Public Shared Sub eigentuemerALKIS(ByVal afst As ParaFlurstueck, insArchiv As Boolean, mitEreignis As Boolean, EreignisLocationAbstract As String) '
        Dim dateisystemPDF As String = "", archivfullname As String = ""
        If Not FST_tools.sindFlurstuecksDatenVollstaendig(afst.normflst) Then
            FST_tools.hole_FSTKoordinaten_undZuweisePunkt(afst)
        End If
        'Dim etools As New Eigentuemer_Tools.RTF(afst.normflst)
        Dim neuereignis As New clsEreignis

        Dim eigSDB As New clsEigentuemerschnell
        Dim dt As DataTable = Nothing
        Dim mycount As Integer
        Dim eigentumerKurzinfo, Eigentuemernameundadresse As String
        Dim erfolg As Boolean = False

        If eigSDB.getEigentuemerdata(afst.normflst.FS, eigentumerKurzinfo, Eigentuemernameundadresse, mycount, dt) Then
            dateisystemPDF = erzeugeUndOeffneEigentuemerPDF(Eigentuemernameundadresse, afst)
            erfolg = True
        End If

        If erfolg Then
            If mitEreignis Then
                initEreignis(neuereignis, "Eigentümer: " & EreignisLocationAbstract)
                clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)
                CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
                    myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
            End If
            If insArchiv Then
                Dim numdir As String = ""
                myGlobalz.sitzung.aktDokument.newSaveMode = True
                erfolg = myGlobalz.Arc.checkINDoku(dateisystemPDF,
                                                   "Eigentümer: " & EreignisLocationAbstract,
                                                   neuereignis.ID,
                                                   False,
                                                   archivfullname,
                                                   numdir,
                                                   myGlobalz.sitzung.aktVorgangsID,
                                                   False,
                                                   Now,
                                                   myGlobalz.sitzung.aktDokument.DocID,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                                   myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                                   myGlobalz.sitzung.aktDokument.kompressed,
                                                    myGlobalz.sitzung.aktBearbeiter.ID)
                If erfolg Then
                    MsgBox(glob2.getMsgboxText("eigentuemerAuskunft", New List(Of String)(New String() {})), MsgBoxStyle.OkOnly, "Eigentümerauskunft als RTF-Datei")
                End If
                glob2.OpenDocument(dateisystemPDF)
            Else
                glob2.OpenDocument(dateisystemPDF)
            End If
        Else
            MsgBox("Es konnte keine RTF-Datei erstellt werden!!")
        End If
    End Sub

    Shared Sub warDaWasFST(machVerwandte As Boolean)
        Dim erfolg As Boolean = FST_tools.AlleSekidsZuDiesermFlurstueckholen()
        Dim bestandVerwandte As DataTable
        bestandVerwandte = getBestandVerwandte(myGlobalz.sitzung.aktVorgangsID)
        If erfolg Then
            If machVerwandte Then
                Dim anzahl As Integer = AdressTools.machverwandte(myGlobalz.sitzung.VorgangREC.dt, myGlobalz.sitzung.aktVorgangsID, bestandVerwandte)
            Else
                Dim datei As String = IO.Path.Combine(myGlobalz.Arc.lokalerCheckoutcache, "warDaWasFlurstueck.txt")
                AdressTools.schreibeVorgangslisteInDatei(datei, myGlobalz.sitzung.VorgangREC.dt)
                glob2.OpenDocument(datei)
            End If
        Else
            Dim messagetext As String = "Es wurden keine weiteren Vorgänge auf dieser Adresse gefunden! " & Environment.NewLine
            MessageBox.Show(messagetext)
        End If
    End Sub

    Shared Function getBestandVerwandte(aktVorgangsID As Integer) As DataTable
        Try
            l(" MOD ---------------------- anfang")
            Dim sql As String = "select * from " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG & "  where vorgangsid=" & aktVorgangsID
            VerwandteTools.erzeugeVerwandtenlistezuVorgang.exe(sql, myGlobalz.sitzung.tempREC)
            l(" MOD ---------------------- ende")
            Return myGlobalz.sitzung.tempREC.dt
        Catch ex As Exception
            l("Fehler in MOD: " ,ex)
            Return Nothing
        End Try
    End Function

    Private Shared Function warDaWas2VerwandteFST(dt As DataTable, aktVorgangsID As Integer) As Integer
        Throw New NotImplementedException()
    End Function

    Public Shared Sub initEreignis(ByVal neuereignis As clsEreignis, Beschreibung As String)
        With neuereignis
            .Art = "info"
            .Beschreibung = Beschreibung
            .Datum = Now
            .istRTF = False
            .Notiz = ""
            .DokumentID = 0
            .Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
            .Richtung = ""
        End With
    End Sub
    Public Shared Function sindFlurstuecksDatenVollstaendig(ByVal aktfs As clsFlurstueck) As Boolean

        Return False
        'Dim voll As Boolean
        'voll = Not String.IsNullOrEmpty(aktfs.gebucht)

        'Return voll
    End Function


    Shared Function DTaufFSTObjektabbilden(ByVal meineDT As DataTable, ByVal aFST As ParaFlurstueck) As Boolean 'myGlobalz.sitzung.tempREC.dt,myGlobalz.sitzung.aktFST
        Try
            With aFST.normflst
                .gemcode = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("gemcode")))
                .flur = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("flur")))
                .zaehler = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("zaehler")))
                .nenner = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("nenner")))
                .fstueckKombi = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("znkombi")))
                .gemarkungstext = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("gemarkungstext")))
                .FS = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("fs")))
                If myglobalz.sitzung.aktFST.FlaecheQm < 1 And
                    CDbl(clsDBtools.fieldvalue(meineDT.Rows(0).Item("flaecheqm"))) > 0 Then
                    .flaecheqm = CDbl((clsDBtools.fieldvalue(meineDT.Rows(0).Item("flaecheqm"))))
                End If
            End With
            Return True
        Catch ex As Exception
            nachricht(String.Format("Fehler in DTaufFSTObjektabbilden {0}{0}{1}", vbCrLf, ex))
            Return False
        End Try
    End Function

    Public Shared Sub nennerUndFSPruefen()
        myglobalz.sitzung.aktFST.normflst.FS = myglobalz.sitzung.aktFST.normflst.buildFS()
        FST_tools.hole_FSTKoordinaten_undZuweisePunkt(myglobalz.sitzung.aktFST)

    End Sub

    Public Shared Function csvlisteNachREC(ByVal datei As String, ByRef rec As String()) As String
        ''dateioffnen
        Dim alles$
        Dim testt As New IO.FileInfo(datei)
        Dim sr As IO.StreamReader
        If Not testt.Exists Then
            testt = Nothing
            Return "Datei angegebene ist nicht vorhanden: " & datei
        Else
            testt = Nothing
        End If
        Try
            sr = New IO.StreamReader(datei)
            alles$ = sr.ReadToEnd()
            rec = alles.Split(CChar(vbCrLf))
            sr.Dispose()

            Return ""
        Catch ex As Exception
            nachricht("Fehler in csvlisteNachFST: " ,ex)
            Return "Fehler in csvlisteNachFST"
        End Try
    End Function

    'Shared Function rec2FST(ByVal titel As String, ByVal radius As Double, ByVal rec As String()) As Boolean
    '    Dim ergebnis As String = "Datei wurde eingelesen. " & Environment.NewLine
    '    ergebnis &= csv_zeilen_zuFST_verarbeiten(rec, radius, titel)
    '    Return True
    'End Function

    Public Shared Sub spaltenZuFST(ByVal spalten$(), ByVal startspalte As Integer)
        Dim gemparms As New clsGemarkungsParams
        gemparms.init() : Dim result$ = "ERROR"
        With myglobalz.sitzung.aktFST.normflst
            Try
                .gemcode = CInt(spalten(startspalte))
                .flur = CInt(spalten(startspalte + 1))
                .zaehler = CInt(spalten(startspalte + 2))
                .nenner = CInt(spalten(startspalte + 3))
                Dim a = From item In gemparms.parms Where item.gemcode = .gemcode Select item.gemarkungstext
                If a.ToArray.Length > 0 Then result$ = a.ToList(0).ToString
                .gemarkungstext = result
                .gemeindeNr = CInt(spalten(startspalte - 1))
                .flaecheqm = CInt(spalten(startspalte + 4))
                .grundbuchblattnr = spalten(startspalte + 10)


            Catch ex As Exception

            End Try

            .FS = .buildFS()
        End With
    End Sub



    Public Shared Function AlleSekidsZuDiesermFlurstueckholen() As Boolean
        'alleSekids zu dieserm flurstückholen
        'instring bilden
        'alle vids zu den sekids hoolen
        'instring bilden
        'alle vorgänge zu dem instring holen
        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer
        Try
            myglobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Flurstueck
            DB_Oracle_sharedfunctions.holeSekidListeDTinVorgaengenZuFlurstueckSQL() 'ist eig. doppelt!?
            'myglobalz.sitzung.tempREC.mydb.Tabelle ="paraflurstueck"
            erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
            If erfolg > 0 Then
                myglobalz.sitzung.VorgangREC.dt = resultdt.Copy
                'myglobalz.sitzung.VorgangREC.mydb.Tabelle ="PF_SEKID2VID"
                Dim instring$ = clsDBtools.bildINstring(resultdt, 0)
                myGlobalz.sitzung.VorgangREC.mydb.SQL = "select vorgangsid from PF_SEKID2VID " &
                                                        " where SEKID IN(" & instring & ")"
                erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
                myGlobalz.sitzung.tempREC.dt = resultdt.Copy
                instring$ = clsDBtools.bildINstring(resultdt, 0)
                myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from  " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & " s " &
                    " where v.vorgangsid IN (" & instring & ") " &
                    " and v.vorgangsid=s.vorgangsid"
                erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
                l(myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString) ' = resultdt.Copy
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler in AlleSekidsZuDiesermFlurstueckholen: " ,ex)
            Return False
        End Try
    End Function

    Shared Function FSTSchonInVorgangVorhanden(ByVal paraFlurstueck As ParaFlurstueck, ByVal vid As Integer) As Boolean
        myGlobalz.sitzung.tempREC.mydb.SQL =
              "select * from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & "  p , pf_sekid2vid s where gemcode=" & paraFlurstueck.normflst.gemcode &
              " and flur=" & paraFlurstueck.normflst.flur &
              " and zaehler=" & paraFlurstueck.normflst.zaehler &
              " and nenner=" & paraFlurstueck.normflst.nenner &
              " and znkombi='" & paraFlurstueck.normflst.fstueckKombi & "'" &
              " and s.vorgangsid= " & vid &
              " and s.sekid=p.id"
        nachricht("adresseSchonVorhanden sql: " & myglobalz.sitzung.tempREC.mydb.SQL)
        Dim hinweis As String = myglobalz.sitzung.tempREC.getDataDT
        If Not myglobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            nachricht("flurstueck ist schon in vorgang vorhanden")
            Return True
        Else
            nachricht("flurstueck ist noch nicht in vorgang vorhanden")
            Return False
        End If
    End Function

    Shared Function bildePufferFuerFlurstueck(FS As String, pufferinMeter As Double) As Boolean
        'select area(geom) from flurkarte.basis_f where fs='FS0607430240037804900'
        'PufferAlsRaumbezugSpeichern
        Dim anychange As Boolean = False
        Dim aktGID As Integer
        Dim aktarea As Double
        Dim akttabelle, aktschema, aktBOX, aktPolygon As String
        akttabelle = "basis_f"
        aktschema = "flurkarte"
        aktGID = NSpostgis.clsPostgis.holeGID4Fs(akttabelle, aktschema, FS)
        aktBOX = NSpostgis.clsPostgis.holeKoordinatenFuerGID(aktGID, akttabelle, aktschema, False)
        aktPolygon = NSpostgis.clsPostgis.holePUFFERPolygonFuerGID(aktGID, akttabelle, aktschema, pufferinMeter, False)
        aktarea = NSpostgis.clsPostgis.holeAreaFuerGID(aktGID, akttabelle, aktschema, False)


        '       myGlobalz.sitzung.aktPolygon.name="<Puffer [m]: " & pufferinMeter
        'myGlobalz.sitzung.aktPolygon.freitext = "Puffer [m]: " & pufferinMeter
        If aktPolygon.IsNothingOrEmpty Then
            MsgBox("Puffer für flurstueck konnte nicht berechnet werden")
            anychange = False
        Else
            myglobalz.sitzung.aktPolygon.myPoly = New Polygon
            myglobalz.sitzung.aktPolygon.myPoly.Name = "myPoly"
            anychange = NSpostgis.clsPostgis.ObjektAlsRaumbezugspeichern(aktPolygon, aktarea, aktBOX, "Puffer [m]: " & pufferinMeter)
        End If
        Return anychange
    End Function

    Shared Function bildePufferFuerPunkt(myPoint As myPoint, pufferinMeter As Double) As Boolean
        ' ST_MakePoint(-71.063526, 42.35785) 
        Dim anychange As Boolean = False
        Dim aktarea As Double
        Dim aktBOX, aktPolygon As String
        'aktGID = nspostgis.clsPostgis.holeGID4Fs(akttabelle, aktschema, FS)
        Try
            aktPolygon = NSpostgis.clsPostgis.holePUFFERPolygonFuerPoint(myPoint, pufferinMeter)
            aktBOX = NSpostgis.clsPostgis.holeKoordinatenFuerUmkreis(aktPolygon) 'reihenfolge geändert
            aktarea = NSpostgis.clsPostgis.holeAreaFuerUmkreis(aktPolygon)

            If aktPolygon.IsNothingOrEmpty Then
                MsgBox("Puffer für Punkt konnte nicht berechnet werden")
                anychange = False
            Else
                myglobalz.sitzung.aktPolygon.myPoly = New Polygon
                myglobalz.sitzung.aktPolygon.myPoly.Name = "myPoly"
                anychange = NSpostgis.clsPostgis.ObjektAlsRaumbezugspeichern(aktPolygon, aktarea, aktBOX, "Puffer [m]: " & pufferinMeter)
            End If
            Return anychange
        Catch ex As Exception
            nachricht("fehler in bildePufferFuerPunkt: " ,ex)
            Return False
        End Try
    End Function

    Shared Function bildePufferFuerPolygon(clsParapolygon As clsParapolygon, pufferinMeter As Double) As Boolean
        Dim anychange As Boolean = False
        Dim aktarea As Double
        Dim aktBOX, aktPuffer, pgPolygon As String
        'aktGID = clsPostgis.holeGID4Fs(akttabelle, aktschema, FS)
        Try
            If clsParapolygon.IstWKT(clsParapolygon.originalQuellString) Then
                pgPolygon = clsParapolygon.originalQuellString
            Else
                pgPolygon = LIBpostgistools.nondbtools.serialGKStringnachWKT(clsParapolygon.ShapeSerial)
            End If

            aktPuffer = NSpostgis.clsPostgis.holePUFFERPolygonFuerPolygon(pgPolygon, pufferinMeter)
            aktBOX = NSpostgis.clsPostgis.holeKoordinatenFuerUmkreis(aktPuffer) 'reihenfolge geändert
            aktarea = NSpostgis.clsPostgis.holeAreaFuerUmkreis(aktPuffer)

            If aktPuffer.IsNothingOrEmpty Then
                MsgBox("Puffer für Polygon konnte nicht berechnet werden")
                anychange = False
            Else
                myglobalz.sitzung.aktPolygon.myPoly = New Polygon
                myglobalz.sitzung.aktPolygon.myPoly.Name = "myPoly"
                anychange = NSpostgis.clsPostgis.ObjektAlsRaumbezugspeichern(aktPuffer, aktarea, aktBOX, "Puffer [m]: " & pufferinMeter)
            End If
            Return anychange
        Catch ex As Exception
            nachricht("fehler in bildePufferFuerPunkt: " ,ex)
            Return False
        End Try
    End Function

    Shared Function bildeFSTListeFuerPolygon(clsParapolygon As clsParapolygon, pufferinmeter As Double) As String
        Dim anychange As Boolean = False
        Dim aktPolygon, pgPolygon As String
        Try
            If clsParapolygon.IstWKT(clsParapolygon.originalQuellString) Then
                pgPolygon = clsParapolygon.originalQuellString.Replace(";", " ")
            Else
                pgPolygon = LIBpostgistools.nondbtools.serialGKStringnachWKT(clsParapolygon.ShapeSerial)
            End If
            aktPolygon = NSpostgis.clsPostgis.holePUFFERPolygonFuerPolygon(pgPolygon, pufferinmeter)
            Dim newDat As DataTable = NSpostgis.clsPostgis.holeFSTlistFuerPolygon(aktPolygon, pufferinmeter)
            If newDat Is Nothing Then
                newDat = Nothing
                Return ""
            End If
            Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("beliebig", newDat, myGlobalz.sitzung.aktVorgangsID, "", CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
            Dim exportfile As String = handcsv.CscDateiAusgeben()
            handcsv.Dispose()
            Return exportfile
            Return ""
        Catch ex As Exception
            nachricht("fehler in bildeFSTListeFuerPolygon: " ,ex)
            Return ""
        End Try
    End Function
    Shared Sub puffernFST(tbpufferinmeter_Text As String, fs As String)
        Dim pufferinMeter As Double = CDbl(tbpufferinmeter_Text)
        Dim puffererzeugt As Boolean = FST_tools.bildePufferFuerFlurstueck(fs, pufferinMeter)
        GC.Collect()
        If puffererzeugt Then
            MsgBox("Das Puffer-Objekt wurde erzeugt und unter 'Raumbezüge'' abgelegt.")
        End If
    End Sub


End Class
