Imports System.Data

Public Class FST_tools


    Shared Sub nachricht(ByVal text$)
        My.Log.WriteEntry(text)
    End Sub

    'Public Shared Sub RB_Und_Vorgang_Entkoppeln(ByVal erfolg%)
    '    My.Log.WriteEntry("RB_Und_Vorgang_Entkoppeln")
    '    erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID)

    '    If erfolg > 0 Then
    '        My.Log.WriteEntry("Objekt wurde erfolgreich gelöscht")
    '    Else
    '        My.Log.WriteEntry("Objekt wurde erfolgreich gelöscht")
    '        nachricht_und_Mbox("Problem beim Löschen des Raumbezugs aus dem Vorgang. Abbruch.")
    '    End If
    'End Sub

    Public Shared Sub loeschenRBFlurstueckExtracted()
        'nachricht("loeschenRBFlurstueckExtracted --------------------------------------------------")
        'Dim erfolg As Integer
        'If CInt(myGlobalz.sitzung.aktFST.Status) = 0 Then
        '    erfolg = FST_DBTool_Oracle.RB_Flurstueck_loeschen(CInt(myGlobalz.sitzung.aktFST.SekID))
        '    If erfolg <= 0 Then
        '        My.Log.WriteEntry("Problem beim Löschen der Raumbezugs-Flurstücks. Abbruch.")
        '        nachricht("Problem beim Löschen der Raumbezugs-Flurstücks. Abbruch.")
        '    End If
        '    erfolg = RBtoolsns.Raumbezug_loeschen_byid_alleDB.execute(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
        '    ' erfolg = DBraumbezug_Mysql.RB_loeschenByID(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
        '    If erfolg <= 0 Then
        '        My.Log.WriteEntry("Problem beim Löschen des Raumbezugs . Abbruch.")
        '        nachricht_und_Mbox("Problem beim Löschen des Raumbezugs. Abbruch.")
        '    End If
        '    'FST_tools.RB_Und_Vorgang_Entkoppeln(erfolg)
        '    ' FST_serialShape_mysql.RB_Flurstueck_Serial_loeschen(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
        '    If RBtoolsns.RB_Flurstueck_Serial_loeschen.exe(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID)) < 0 Then
        '        nachricht("Fehler oder keine RBs vorhanden: vorgang: " & myGlobalz.sitzung.aktVorgangsID & " rbsekid:" & myGlobalz.sitzung.aktFST.SekID)
        '    End If
        'Else
        '    'FST_tools.RB_Und_Vorgang_Entkoppeln(erfolg)
        'End If
    End Sub

    Public Shared Sub flurstueck_speichernExtracted(ByVal Radius As Double, isMapEnabled As Boolean)
        myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Flurstueck
        myGlobalz.sitzung.aktFST.typ = RaumbezugsTyp.Flurstueck
        myGlobalz.sitzung.aktFST.isMapEnabled = isMapEnabled
        myGlobalz.sitzung.aktFST.normflst.fstueckKombi = myGlobalz.sitzung.aktFST.normflst.buildFstueckkombi()
        NeuesFSTspeichern(Radius)
        altesFSTspeichern(Radius)
        AdressTools.setzeeNeuesGemKRZ(myGlobalz.sitzung)
        '   myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = Now
        myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
        nachricht("hatraumbezug imn stammdaten setzen!!")
        '    glob2.EDIT_VorgangStamm_2DBOk()
        'If AdressTools.setzeeNeuesGemKRZ(myGlobalz.sitzung) Then
        '    glob2.EDIT_VorgangStamm_2DBOk()
        'End If
    End Sub


    Public Shared Function RBFlstNeu_alleDB() As Integer
        'Dim result% = 0
        'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
        '    Dim zzz As New FST_DBTool_mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
        '    result% = zzz.RB_FLST_abspeichern_Neu()
        '    zzz.Dispose()
        'End If
        'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New FST_DBTool_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
        '    result% = zzz.RB_FLST_abspeichern_Neu()
        '    zzz.Dispose()
        'End If
        'Return result
    End Function

    Public Shared Function RBFlstEdit_alleDB(ByVal sekid As Integer) As Integer
        'Dim result% = 0
        'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
        '    Dim zzz As New FST_DBTool_mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
        '    result% = zzz.RB_FLST_abspeichern_Edit(sekid)
        '    zzz.Dispose()
        'End If
        'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New FST_DBTool_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
        '    result% = zzz.RB_FLST_abspeichern_Edit(sekid)
        '    zzz.Dispose()
        'End If
        'Return result
    End Function
    Public Shared Sub NeuesFSTspeichern(ByVal Radius As Double)

        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            'Dim sekID% = FST_DBTool_mysql.RB_FLST_abspeichern_Neu()
            Dim sekid% = RBFlstNeu_alleDB()
            If sekid > 0 Then
                myGlobalz.sitzung.aktFST.SekID = sekid
                myGlobalz.sitzung.aktFST.defineAbstract()
                DBraumbezug_Mysql.defineBBOX(Radius, myGlobalz.sitzung.aktFST)
                Dim raumbezugsID% = RBtoolsns.Raumbezug_abspeichern_Neu_alleDB.execute(myGlobalz.sitzung.aktFST)
                '  Dim raumbezugsID% = DBraumbezug_Mysql.Raumbezug_abspeichern_Neu(myGlobalz.sitzung.aktFST)
                nachricht("kooplungsid: " &
                                RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(raumbezugsID, myGlobalz.sitzung.aktVorgangsID, 0).ToString)
                myGlobalz.sitzung.aktFST.RaumbezugsID = raumbezugsID
                'Dim serialLesenUndSpeichernThread As System.Threading.Thread
                'serialLesenUndSpeichernThread = New System.Threading.Thread(AddressOf glob2.generateAndSaveSerialShapeInDb)
                'serialLesenUndSpeichernThread.Start()
                glob2.generateAndSaveSerialShapeInDb()
            Else
                nachricht("NeuesFSTspeichern: Problem beim Abspeichernd!")
            End If
        End If
    End Sub

    Private Shared Sub altesFSTspeichern(ByVal Radius As Double)
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            myGlobalz.sitzung.aktFST.defineAbstract()
            DBraumbezug_Mysql.defineBBOX(Radius, myGlobalz.sitzung.aktFST)
            'nachricht("SekID: " & FST_DBTool_mysql.RB_FLST_abspeichern_Edit(CInt(myGlobalz.sitzung.aktFST.SekID)).ToString)
            nachricht("SekID: " & RBFlstEdit_alleDB(CInt(myGlobalz.sitzung.aktFST.SekID)).ToString)

            RBtoolsns.Raumbezug_edit_alleDB.execute(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID), myGlobalz.sitzung.aktFST)
            'DBraumbezug_Mysql.Raumbezug_abspeichern_Edit(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID), myGlobalz.sitzung.aktFST)

            'rbid festellen : myGlobalz.sitzung.aktFST.RaumbezugsID
            'altes serialshap löschen
            ' FST_serialShape_mysql.RB_Flurstueck_Serial_loeschen(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))


            If RBtoolsns.RB_Flurstueck_Serial_loeschen.exe(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID)) < 0 Then
                nachricht("Fehler oder keine RBs vorhanden: vorgang: " & myGlobalz.sitzung.aktVorgangsID &
                                " rbsekid:" & myGlobalz.sitzung.aktFST.SekID & " rbid: " & myGlobalz.sitzung.aktFST.RaumbezugsID)
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
                .punkt.X = CDbl(meinDT.Rows(0).Item("rechts"))
                .punkt.Y = CDbl(meinDT.Rows(0).Item("hoch"))
                .normflst.zeigtauf = CStr((meinDT.Rows(0).Item("zeigtauf")))
                .normflst.weistauf = CStr((meinDT.Rows(0).Item("weistauf")))
                .normflst.gebucht = CStr((meinDT.Rows(0).Item("gebucht")))
                .normflst.flaecheqm = CDbl((meinDT.Rows(0).Item("gisarea")))
                .normflst.fsgml = CStr((meinDT.Rows(0).Item("fsgml")))
                .normflst.FS = CStr((meinDT.Rows(0).Item("fs")))
            End With
        Catch ex As Exception
            nachricht("FEHLER in BASIS_vonDTnachObjekt: " & ex.ToString)
        End Try
    End Sub

    Public Shared Sub hole_FSTKoordinaten_undZuweisePunkt(ByVal afst As ParaFlurstueck)
        Try
            DB_Oracle_sharedfunctions.hole_FSTKoordinatenExtracted(afst.normflst.FS)
            If Not myGlobalz.sitzung.postgresREC.dt.IsNothingOrEmpty Then
                BASIS_vonDTnachObjekt(afst, myGlobalz.sitzung.postgresREC.dt)
                myGlobalz.sitzung.aktFST.FlaecheQm = CDbl((myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("gisarea")))
                'myGlobalz.sitzung.aktFST.normflst.FS = CStr(myGlobalz.sitzung.fstREC.dt.Rows(0).Item(2))
            Else
                afst.punkt.X = 0
                afst.punkt.Y = 0
            End If

        Catch ex As Exception
            afst.punkt.X = 0
            afst.punkt.Y = 0
            afst.normflst.FS = ""
            nachricht_und_Mbox("Fehler bei der Ermittlung der Koordinaten! (=:0)")
        End Try
    End Sub

    'Public Shared Sub eigentuemerALKIS(ByVal afst As ParaFlurstueck, insArchiv As Boolean, mitEreignis As Boolean, EreignisLocationAbstract As String) '
    '    Dim dateisystemPDF As String = "", archivfullname As String = ""
    '    If Not FST_tools.sindFlurstuecksDatenVollstaendig(afst.normflst) Then
    '        FST_tools.hole_FSTKoordinaten_undZuweisePunkt(afst)
    '    End If
    '    Dim etools As New Eigentuemer_Tools.RTF(afst.normflst)
    '    Dim neuereignis As New clsEreignis

    '    Dim erfolg As Boolean = etools.send_Shellbatch_EIGENTUEMER(String.Format("eigentuemer_alkis_{0}_.rtf", myGlobalz.sitzung.aktBearbeiter.username), _
    '                                                                dateisystemPDF,
    '                                                                myGlobalz.sitzung.aktBearbeiter.username,
    '                                                                myGlobalz.sitzung.aktVorgangsID)
    '    System.Threading.Thread.Sleep(20000)
    '    If erfolg Then
    '        If mitEreignis Then
    '            initEreignis(neuereignis, "Eigentümer: " & EreignisLocationAbstract)
    '            clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)
    '            clstart.myc.aLog.komponente = "Ereignis" : clstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
    '                myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : clstart.myc.aLog.log()
    '        End If
    '        If insArchiv Then
    '            Dim numdir As String
    '            numdir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID))
    '            erfolg = myGlobalz.Arc.checkINDoku(dateisystemPDF,
    '                                               "Eigentümer: " & EreignisLocationAbstract,
    '                                               neuereignis.ID,
    '                                               False,
    '                                               archivfullname,
    '                                               numdir,
    '                                               myGlobalz.sitzung.aktVorgangsID,
    '                                               False,
    '                                               Now,
    '                                               myGlobalz.sitzung.aktDokument.DocID,
    '                                               myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
    '            If erfolg Then
    '                MsgBox("Die Eigentümerauskunft wurde ins Dokumentenarchiv aufgenommen." & vbCrLf & vbCrLf &
    '                       "Sie müssen die Dokumentenliste auffrischen um die Datei zu sehen !", MsgBoxStyle.OkOnly, "Eigentümerauskunft als RTF-Datei")
    '            End If
    '            Process.Start(New ProcessStartInfo(dateisystemPDF))
    '        Else
    '            Process.Start(New ProcessStartInfo(dateisystemPDF))
    '        End If
    '    Else
    '        MsgBox("Es konnte keine RTF-Datei erstellt werden!!")
    '    End If
    'End Sub

    'Private Shared Sub initEreignis(ByVal neuereignis As clsEreignis, Beschreibung As String)
    '    With neuereignis
    '        .Art = "info"
    '        .Beschreibung = Beschreibung
    '        .Datum = Now
    '        .istRTF = False
    '        .Notiz = ""
    '        .DokumentID = 0
    '        .Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
    '        .Richtung = ""
    '    End With
    'End Sub
    Public Shared Function sindFlurstuecksDatenVollstaendig(ByVal aktfs As clsFlurstueck) As Boolean

        Return False
        'Dim voll As Boolean
        'voll = Not String.IsNullOrEmpty(aktfs.gebucht)

        'Return voll
    End Function


    'Shared Function DTaufFSTObjektabbilden(ByVal meineDT As DataTable, ByVal aFST As ParaFlurstueck) As Boolean 'myGlobalz.sitzung.tempREC.dt,myGlobalz.sitzung.aktFST
    '    Try
    '        With aFST.normflst
    '            .gemcode = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("gemcode")))
    '            .flur = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("flur")))
    '            .zaehler = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("zaehler")))
    '            .nenner = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("nenner")))
    '            .fstueckKombi = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("znkombi")))
    '            .gemarkungstext = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("gemarkungstext")))
    '            .FS = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("fs")))
    '            If myGlobalz.sitzung.aktFST.FlaecheQm < 1 And 
    '                CDbl(clsDBtools.fieldvalue(meineDT.Rows(0).Item("flaecheqm"))) > 0 Then
    '                .flaecheqm = CDbl((clsDBtools.fieldvalue(meineDT.Rows(0).Item("flaecheqm"))))
    '            End If                
    '        End With
    '        Return True
    '    Catch ex As Exception
    '        nachricht(String.Format("Fehler in DTaufFSTObjektabbilden {0}{0}{1}", vbCrLf, ex))
    '        Return False
    '    End Try
    'End Function

    Public Shared Sub nennerUndFSPruefen()
        myGlobalz.sitzung.aktFST.normflst.FS = myGlobalz.sitzung.aktFST.normflst.buildFS()
        FST_tools.hole_FSTKoordinaten_undZuweisePunkt(myGlobalz.sitzung.aktFST)

    End Sub

    Public Shared Function csvlisteNachREC(ByVal datei As String, ByRef rec As String()) As String
        ''dateioffnen
        Dim alles$
        Dim testt As New IO.FileInfo(datei)
        Dim sr As IO.StreamReader
        If Not testt.Exists Then
            Return "Datei angegebene ist nicht vorhanden: " & datei
        End If
        Try
            sr = New IO.StreamReader(datei)
            alles$ = sr.ReadToEnd()
            rec = alles.Split(CChar(vbCrLf))
            sr.Dispose()

            Return ""
        Catch ex As Exception
            nachricht("Fehler in csvlisteNachFST: " & ex.ToString)
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
        With myGlobalz.sitzung.aktFST.normflst
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



    Public Shared Function AlleSekidsZuDiesermFlurstueckholen(modus As String) As Boolean
        'alleSekids zu dieserm flurstückholen
        'instring bilden
        'alle vids zu den sekids hoolen
        'instring bilden
        'alle vorgänge zu dem instring holen
        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer
        Try
            myglobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Flurstueck
            If modus = "flst" Then
                DB_Oracle_sharedfunctions.holeSekidListeDTinVorgaengenZuFlurstueckSQL() 'ist eig. doppelt!?
            End If
            If modus = "zaehler" Then
                DB_Oracle_sharedfunctions.holeSekidListeDTinVorgaengenZuZaehlerSQL() 'ist eig. doppelt!?
            End If
            If modus = "flur" Then
                DB_Oracle_sharedfunctions.holeSekidListeDTinVorgaengenZuFlurSQL() 'ist eig. doppelt!?
            End If
            'myglobalz.sitzung.tempREC.mydb.Tabelle ="paraflurstueck"
            erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
            If erfolg > 0 Then
                myglobalz.sitzung.VorgangREC.dt = resultdt.Copy
                'myglobalz.sitzung.VorgangREC.mydb.Tabelle ="PF_SEKID2VID"
                Dim instring$ = clsDBtools.bildINstring(resultdt, 0)
                myGlobalz.sitzung.VorgangREC.mydb.SQL = "select vorgangsid from PF_SEKID2VID where SEKID IN(" & instring & ")"
                erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
                myGlobalz.sitzung.tempREC.dt = resultdt.Copy
                instring$ = clsDBtools.bildINstring(resultdt, 0)
                myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & " s " &
                    " where v.vorgangsid IN (" & instring & ") " &
                    " and v.vorgangsid=s.vorgangsid"
                erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
                myGlobalz.sitzung.VorgangREC.dt = resultdt.Copy
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler in AlleSekidsZuDiesermFlurstueckholen: " & ex.ToString)
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
        nachricht("adresseSchonVorhanden sql: " & myGlobalz.sitzung.tempREC.mydb.SQL)
        Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
        If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            nachricht("flurstueck ist schon in vorgang vorhanden")
            Return True
        Else
            nachricht("flurstueck ist noch nicht in vorgang vorhanden")
            Return False
        End If
    End Function



End Class
