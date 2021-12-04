﻿''' <summary>
''' initialisierung von Paradigma über die XML-Datei
''' die daten werden an ein globales dictionary übergeben (inidict)
''' </summary>
''' <remarks></remarks>
Public Class initP
    'Private Shared paradigmaServer As String
    'Private Shared paradigma_user, paradigma_PW, webgis_PW, webgis_user, paradigmadatentyp As String
    'Private Shared paradigma_schema As String
    'Private Shared paradigma_servicename As String
    'Private Shared halo_server As String
    'Private Shared halo_schema As String
    'Private Shared probaug_user, probaug_pw As String


    Shared Sub INI_All(ByVal localAppDataParadigmaDir As String)
        ini_beide(localAppDataParadigmaDir)
        ini_DBserverNames(localAppDataParadigmaDir)
        dbInstanzenbilden()
        dbInstanzenInitialisieren()
    End Sub

    Shared Sub ini_beide(ByVal localAppDataParadigmaDir As String)
        myGlobalz.ProxyString = getproxystring()
        'myGlobalz.coordZiel = CType(clstart.mycsimple.iniDict("Beide.coordZiel"), String) '  myGlobalz.coordZiel = "UTM_google" 'GK_google/UTM_google
        myGlobalz.ClientCookieDir = localAppDataParadigmaDir & CType(CLstart.mycSimple.iniDict("Beide.ClientCookieDir"), String)        'myGlobalz.ClientCookieDir = localAppDataParadigmaDir & "\cookies\"
        '  myGlobalz.INIFILEOldStyle = myGlobalz.ClientCookieDir & "user.ini"
        myGlobalz.Infotext_revisionssicherheit = _
            "Das Dokument ist revisionssicher. Sie können die Änderungen also nicht direkt ins Archiv übernehmen." & Environment.NewLine & Environment.NewLine &
            "Hinweis: " & Environment.NewLine & Environment.NewLine &
            "Falls Sie das geänderte Dokument doch ins Archiv nehmen wollen, müssen Sie es als neues " & Environment.NewLine &
            "Dokument (= neue Version) aufnehmen. Gehen Sie wie folgt vor:" & Environment.NewLine &
            " 1 - Speichern Sie das geänderte Dokument unter einem neuen Namen an einen Ihnen bekannten Ort z.B. C:\neueVersion.doc" & Environment.NewLine &
            " 2 - Fügen Sie das neue Dokument dem Vorgang hinzu indem Sie unter Dokumente auf 'Hinzufügen' gehen und das neue dokument anwählen." & Environment.NewLine
        'iniGISRahmen()
    End Sub

    Shared Sub ini_DBserverNames(ByVal localAppDataParadigmaDir As String)
        '  myGlobalz.paradigmaDateiServerRoot = CLstart.mycsimple.getparadigmaDateiServerRoot()'
        myGlobalz.textmarkenUeberSichtsDatei = CType(CLstart.mycSimple.iniDict("Haupt.textmarkenUeberSichtsDatei"), String)

        myGlobalz.LOGFILEKOPIE = CLstart.mycSimple.getparadigmaDateiServerRoot() &
                                 CType(CLstart.mycSimple.iniDict("Haupt.LOGFILEKOPIE"), String) &
                                 myGlobalz.sitzung.aktBearbeiter.username & ".log"
        myGlobalz.ActionLogDir = CLstart.mycSimple.getparadigmaDateiServerRoot() &
                                 CType(CLstart.mycSimple.iniDict("Haupt.ActionLogDir"), String)
        myGlobalz.XMLserverConfigDir = CLstart.mycSimple.getparadigmaDateiServerRoot() &
                                 CType(CLstart.mycSimple.iniDict("Haupt.XMLserverConfigDir"), String)
        myGlobalz.XMLclientConfigDir = localAppDataParadigmaDir & CType(CLstart.mycSimple.iniDict("Haupt.XMLclientConfigDir"), String)

        CLstart.myc.kartengen = New LibGISmapgenerator.clsAufrufgenerator("http://" &
                                initP.getValue("GisServer.GIS_WebServer"))   'GIS_WebServer
        myGlobalz.Paradigma_Sachgebietsdatei = getParadigmaSachgebietsdateiName(myGlobalz.XMLclientConfigDir)
    End Sub



    Shared Function getproxystring() As String
        Dim wert$ = "-1"
        Dim a$ = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\" &
                    "Microsoft\Windows\CurrentVersion\Internet Settings",
                    "ProxyServer", wert).ToString
        If a = "-1" Then
            a = ""
        Else
            a = "http://" & a$
        End If
        nachricht("myGlobalz.ProxyString$: " & a)
        Return a
    End Function

    Shared Sub ini_fstREC()
        With myGlobalz.fst_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("FlurstuecksDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("FlurstuecksDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("FlurstuecksDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("FlurstuecksDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("FlurstuecksDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("FlurstuecksDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("FlurstuecksDB.dbtyp"), String)
            myGlobalz.sitzung.fstREC = setDbRecTyp(myGlobalz.fst_MYDB)
            myGlobalz.sitzung.fstREC.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Private Shared Sub ini_probaugREC()

        With myGlobalz.probaug_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("ProbaugDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("ProbaugDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("ProbaugDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("ProbaugDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("ProbaugDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("ProbaugDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("ProbaugDB.dbtyp"), String)
            myGlobalz.sitzung.probaugREC = setDbRecTyp(myGlobalz.probaug_MYDB)
            myGlobalz.sitzung.probaugREC.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_webgisREC()
        With myGlobalz.webgis_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("WebgisDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("WebgisDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("WebgisDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("WebgisDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("WebgisDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("WebgisDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("WebgisDB.dbtyp"), String)
            myGlobalz.sitzung.webgisREC = setDbRecTyp(myGlobalz.webgis_MYDB)
            myGlobalz.sitzung.webgisREC.mydb = CType(myGlobalz.webgis_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub
    Shared Sub ini_haloREC()
        With myGlobalz.halo_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("HaloDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("HaloDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("HaloDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("HaloDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("HaloDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("HaloDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("HaloDB.dbtyp"), String)
            myGlobalz.sitzung.haloREC = setDbRecTyp(myGlobalz.halo_MYDB)
            myGlobalz.sitzung.haloREC.mydb = CType(myGlobalz.halo_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Public Shared Function setDbRecTyp(ByVal dummyDB As clsDatenbankZugriff) As IDB_grundfunktionen
        Dim dummREC As IDB_grundfunktionen = Nothing
        Try
            If dummyDB.dbtyp = "oracle" Then
                dummREC = New LIBoracle.clsDBspecOracle
            End If
            If dummyDB.dbtyp = "mysql" Then
                dummREC = New clsDBspecMYSQL
            End If

            If dummREC Is Nothing Then
                nachricht("Fehler: setDbRecTyp, ggf. ist der db-Typ unbekannt:" & dummyDB.getDBinfo(""))
            End If
            Return dummREC
        Catch ex As Exception
            nachricht_und_Mbox("Fehler: setDbRecTyp2,  :" & ex.ToString)
            Return Nothing
        End Try
    End Function

    Shared Sub ini_bearbeiterrec()
        myGlobalz.bearbeiter_MYDB.Host = CType(CLstart.mycSimple.iniDict("BearbeiterDB.MySQLServer"), String) ' "localhost"
        myGlobalz.bearbeiter_MYDB.Schema = CType(CLstart.mycSimple.iniDict("BearbeiterDB.Schema"), String) ' "paradigma"
        myGlobalz.bearbeiter_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("BearbeiterDB.ServiceName"), String) ' ""
        myGlobalz.bearbeiter_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("BearbeiterDB.Tabelle"), String) ' "bearbeiter"
        myGlobalz.bearbeiter_MYDB.username = CType(CLstart.mycSimple.iniDict("BearbeiterDB.username"), String) ' "Paradigma"
        myGlobalz.bearbeiter_MYDB.password = CType(CLstart.mycSimple.iniDict("BearbeiterDB.password"), String) '"lkof4"
        myGlobalz.bearbeiter_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("BearbeiterDB.dbtyp"), String) ' "oracle"

        myGlobalz.sitzung.BearbeiterREC = setDbRecTyp(myGlobalz.bearbeiter_MYDB)
        myGlobalz.sitzung.BearbeiterREC.mydb = CType(myGlobalz.bearbeiter_MYDB.Clone, clsDatenbankZugriff)
    End Sub

    Shared Sub ini_temprec()

        With myGlobalz.temp_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("Temp.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("Temp.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("Temp.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("Temp.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("Temp.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("Temp.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("Temp.dbtyp"), String)
            myGlobalz.sitzung.tempREC = setDbRecTyp(myGlobalz.temp_MYDB)
            myGlobalz.sitzung.tempREC.mydb = CType(myGlobalz.temp_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_Alb()
        myGlobalz.alb_MYDB.Host = CType(CLstart.mycSimple.iniDict("AlbDB.MySQLServer"), String) '  paradigmaServer
        myGlobalz.alb_MYDB.Schema = CType(CLstart.mycSimple.iniDict("AlbDB.Schema"), String) '"alb"
        myGlobalz.alb_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("AlbDB.Tabelle"), String) '"gmde"
        myGlobalz.alb_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("AlbDB.ServiceName"), String)
        myGlobalz.alb_MYDB.username = CType(CLstart.mycSimple.iniDict("AlbDB.username"), String) ' webgis_user
        myGlobalz.alb_MYDB.password = CType(CLstart.mycSimple.iniDict("AlbDB.password"), String) ' webgis_PW
        myGlobalz.alb_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("AlbDB.dbtyp"), String) 'paradigmadatentyp$

        myGlobalz.sitzung.AlbRec = setDbRecTyp(myGlobalz.alb_MYDB)
        myGlobalz.sitzung.AlbRec.mydb = CType(myGlobalz.alb_MYDB.Clone, clsDatenbankZugriff)
    End Sub

    Shared Sub ini_Kontaktdaten()
        myGlobalz.kontaktdaten_MYDB.Host = CType(CLstart.mycSimple.iniDict("KontaktDB.MySQLServer"), String) '  paradigmaServer
        myGlobalz.kontaktdaten_MYDB.Schema = CType(CLstart.mycSimple.iniDict("KontaktDB.Schema"), String) '"alb"
        myGlobalz.kontaktdaten_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("KontaktDB.Tabelle"), String) '"gmde"
        myGlobalz.kontaktdaten_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("KontaktDB.ServiceName"), String)
        myGlobalz.kontaktdaten_MYDB.username = CType(CLstart.mycSimple.iniDict("KontaktDB.username"), String) ' webgis_user
        myGlobalz.kontaktdaten_MYDB.password = CType(CLstart.mycSimple.iniDict("KontaktDB.password"), String) ' webgis_PW
        myGlobalz.kontaktdaten_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("KontaktDB.dbtyp"), String) 'paradigmadatentyp$
        myGlobalz.sitzung.kontaktdatenREC = setDbRecTyp(myGlobalz.kontaktdaten_MYDB)
        myGlobalz.sitzung.kontaktdatenREC.mydb = CType(myGlobalz.kontaktdaten_MYDB.Clone, clsDatenbankZugriff)
    End Sub

    Shared Sub ini_vorgang()
        myGlobalz.vorgang_MYDB.Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String) '  paradigmaServer
        myGlobalz.vorgang_MYDB.Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String) '"alb"
        myGlobalz.vorgang_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) '"gmde"
        myGlobalz.vorgang_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
        myGlobalz.vorgang_MYDB.username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String) ' webgis_user
        myGlobalz.vorgang_MYDB.password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String) ' webgis_PW
        myGlobalz.vorgang_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String) 'paradigmadatentyp$ 
        myGlobalz.sitzung.VorgangREC = setDbRecTyp(myGlobalz.vorgang_MYDB)
        myGlobalz.sitzung.VorgangREC.mydb = CType(myGlobalz.vorgang_MYDB.Clone, clsDatenbankZugriff)
    End Sub
    Shared Sub ini_raumbezug()
        myGlobalz.raumbezug_MYDB.Host = CType(CLstart.mycSimple.iniDict("RaumbezugDB.MySQLServer"), String) '  paradigmaServer
        myGlobalz.raumbezug_MYDB.Schema = CType(CLstart.mycSimple.iniDict("RaumbezugDB.Schema"), String) '"alb"
        myGlobalz.raumbezug_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("RaumbezugDB.Tabelle"), String) '"gmde"
        myGlobalz.raumbezug_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("RaumbezugDB.ServiceName"), String)
        myGlobalz.raumbezug_MYDB.username = CType(CLstart.mycSimple.iniDict("RaumbezugDB.username"), String) ' webgis_user
        myGlobalz.raumbezug_MYDB.password = CType(CLstart.mycSimple.iniDict("RaumbezugDB.password"), String) ' webgis_PW
        myGlobalz.raumbezug_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("RaumbezugDB.dbtyp"), String) 'paradigmadatentyp$
        myGlobalz.sitzung.raumbezugsRec = setDbRecTyp(myGlobalz.raumbezug_MYDB)
        myGlobalz.sitzung.raumbezugsRec.mydb = CType(myGlobalz.raumbezug_MYDB.Clone, clsDatenbankZugriff)
    End Sub

    Shared Sub ini_Ereignis()
        With myGlobalz.Ereignisse_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("EreignisDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("EreignisDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("EreignisDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("EreignisDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("EreignisDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("EreignisDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("EreignisDB.dbtyp"), String)
            myGlobalz.sitzung.EreignisseRec = setDbRecTyp(myGlobalz.Ereignisse_MYDB)
            myGlobalz.sitzung.EreignisseRec.mydb = CType(myGlobalz.Ereignisse_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub
    Shared Sub ini_Archiv()
        With myGlobalz.ARC_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("ArchivDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("ArchivDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("ArchivDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("ArchivDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("ArchivDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("ArchivDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("ArchivDB.dbtyp"), String)
            myGlobalz.Arc.ArcRec = setDbRecTyp(myGlobalz.ARC_MYDB)
            myGlobalz.Arc.ArcRec.mydb = CType(myGlobalz.ARC_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_Wiedervorlage()
        With myGlobalz.wiedervorlage_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("Wiedervorlage.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("Wiedervorlage.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("Wiedervorlage.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("Wiedervorlage.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("Wiedervorlage.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("Wiedervorlage.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("Wiedervorlage.dbtyp"), String)
            myGlobalz.sitzung.DBWiedervorlageREC = setDbRecTyp(myGlobalz.wiedervorlage_MYDB)
            myGlobalz.sitzung.DBWiedervorlageREC.mydb = CType(myGlobalz.wiedervorlage_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_Zahlung()
        With myGlobalz.zahlung_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("Zahlung.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("Zahlung.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("Zahlung.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("Zahlung.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("Zahlung.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("Zahlung.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("Zahlung.dbtyp"), String)
            myGlobalz.sitzung.zahlungsREC = setDbRecTyp(myGlobalz.zahlung_MYDB)
            myGlobalz.sitzung.zahlungsREC.mydb = CType(myGlobalz.zahlung_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_Beteiligte()
        With myGlobalz.beteiligte_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("Beteiligte.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("Beteiligte.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("Beteiligte.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("Beteiligte.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("Beteiligte.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("Beteiligte.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("Beteiligte.dbtyp"), String)
            myGlobalz.sitzung.beteiligteREC = setDbRecTyp(myGlobalz.beteiligte_MYDB)
            myGlobalz.sitzung.beteiligteREC.mydb = CType(myGlobalz.beteiligte_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_BeteiligteVerwandt()
        With myGlobalz.beteiligte_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("Beteiligte.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("Beteiligte.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("Beteiligte.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("Beteiligte.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("Beteiligte.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("Beteiligte.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("Beteiligte.dbtyp"), String)
            myGlobalz.sitzung.beteiligteRECVerwandt = setDbRecTyp(myGlobalz.beteiligte_MYDB)
            myGlobalz.sitzung.beteiligteRECVerwandt.mydb = CType(myGlobalz.beteiligte_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_vorgangsbeteiligteAuswahlREC()
        With myGlobalz.vorgangsbeteiligte_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangsBeteiligte.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangsBeteiligte.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("VorgangsBeteiligte.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangsBeteiligte.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangsBeteiligte.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangsBeteiligte.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangsBeteiligte.dbtyp"), String)
            myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC = setDbRecTyp(myGlobalz.vorgangsbeteiligte_MYDB)
            myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb = CType(myGlobalz.vorgangsbeteiligte_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Private Shared Sub dbInstanzenbilden()
    End Sub

    Private Shared Sub dbInstanzenInitialisieren()
        ' ini_bearbeiterrec()
        ini_bearbeiterrec()
        ini_Alb()
        ini_vorgang()
        ini_Kontaktdaten()
        ini_raumbezug()
        ini_Ereignis()
        ini_Archiv()
        ini_Wiedervorlage()
        ini_temprec()
        ini_Beteiligte()
        ini_BeteiligteVerwandt()
        ini_haloREC()
        ini_fstREC()
        ini_probaugREC()
        ini_vorgangsbeteiligteAuswahlREC()
        ini_Zahlung()
        ini_webgisREC()
    End Sub

    'Shared Sub iniGISRahmen()
    '    myGlobalz.GIS_Rahmen_Fit_Xmin = CType(clstart.mycsimple.iniDict("Beide.GIS_Rahmen_Fit_Xmin"), Double) '470531
    '    myGlobalz.GIS_Rahmen_Fit_Ymin = CType(clstart.mycsimple.iniDict("Beide.GIS_Rahmen_Fit_Ymin"), Double) '5532582
    '    myGlobalz.GIS_Rahmen_Fit_Xmax = CType(clstart.mycsimple.iniDict("Beide.GIS_Rahmen_Fit_Xmax"), Double) '503699
    '    myGlobalz.GIS_Rahmen_Fit_ymax = CType(clstart.mycsimple.iniDict("Beide.GIS_Rahmen_Fit_ymax"), Double) '5553740
    'End Sub

    Public Shared Function getParadigmaSachgebietsdateiName(verzeichnis As String) As String
        ' (myGlobalz.XMLclientConfigDir & CType(clstart.mycsimple.iniDict("Haupt.Paradigma_Sachgebietsdatei"), String) )
        If verzeichnis.IsNothingOrEmpty Then
            nachricht("fehler getParadigmaSachgebietsdateiName, verzeichnis ist nicht angegeben")
        End If
        Return verzeichnis & CType(CLstart.mycSimple.iniDict("Haupt.Paradigma_Sachgebietsdatei"), String)
    End Function
    ''' <summary>
    ''' get the value for XmlTag
    ''' xmltag ist 2teilig: kategorie.tag 
    ''' </summary>
    ''' <param name="TagsName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function getValue(TagsName As String) As String
        Dim a As String
        Try
            If TagsName.IsNothingOrEmpty Then
                nachricht("warnung getXmlTagValue , Wert ist leer: TagsName: " & TagsName)
                Return ""
            End If
            a = CType(CLstart.mycSimple.iniDict(TagsName), String)
            If a.IsNothingOrEmpty Then
                MsgBox("Wert ist leer: " & TagsName)
                nachricht("fehler getXmlTagValue , Wert ist leer: " & TagsName)
            End If
            Return a
        Catch ex As Exception
            nachricht("fehler getXmlTagValue ,  : TagsName: " & TagsName & ex.ToString)
            Return ""
        End Try
    End Function
End Class
