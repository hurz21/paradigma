#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
''' <summary>
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
        myGlobalz.ClientCookieDir = localAppDataParadigmaDir & CType(CLstart.mycSimple.iniDict("Beide.ClientCookieDir"), String)        'myGlobalz.ClientCookieDir = localAppDataParadigmaDir & "\cookies\"
    End Sub

    Shared Sub ini_DBserverNames(ByVal localAppDataParadigmaDir As String)
        '  myGlobalz.paradigmaDateiServerRoot = CLstart.mycsimple.getparadigmaDateiServerRoot()'
        myGlobalz.textmarkenUeberSichtsDatei = CType(CLstart.mycSimple.iniDict("Haupt.textmarkenUeberSichtsDatei"), String)

        myGlobalz.LOGFILEKOPIE = CLstart.mycSimple.getparadigmaDateiServerRoot() &
                                 CType(CLstart.mycSimple.iniDict("Haupt.LOGFILEKOPIE"), String) &
                                 myGlobalz.sitzung.aktBearbeiter.username & "Kopie.log"
        myGlobalz.ActionLogDir = CLstart.mycSimple.getparadigmaDateiServerRoot() &
                                 CType(CLstart.mycSimple.iniDict("Haupt.ActionLogDir"), String)
        myGlobalz.XMLserverConfigDir = CLstart.mycSimple.getparadigmaDateiServerRoot() &
                                 CType(CLstart.mycSimple.iniDict("Haupt.XMLserverConfigDir"), String)
        myGlobalz.XMLclientConfigDir = localAppDataParadigmaDir & CType(CLstart.mycSimple.iniDict("Haupt.XMLclientConfigDir"), String)
        'CLstart.myc.MAINiniFile = New CLstart.clsINIDatei(myGlobalz.appdataDir & "\config\Combos\mainINI\main.ini")

        CLstart.myc.kartengen = New clsAufrufgenerator(initP.getValue("GisServer.ServerHTTPdomainIntranet"))   'GIS_WebServer
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

 

    Private Shared Sub ini_probaugREC()
        Try
            l("---------------------- anfang")
            With myGlobalz.probaug_MYDB
                .Host = CType(CLstart.mycSimple.iniDict("ProbaugDB.MySQLServer"), String)
                .Schema = CType(CLstart.mycSimple.iniDict("ProbaugDB.Schema"), String)
                .Tabelle = CType(CLstart.mycSimple.iniDict("ProbaugDB.Tabelle"), String)
                .ServiceName = CType(CLstart.mycSimple.iniDict("ProbaugDB.ServiceName"), String)
                .username = CType(CLstart.mycSimple.iniDict("ProbaugDB.username"), String)
                .password = CType(CLstart.mycSimple.iniDict("ProbaugDB.password"), String)
                .dbtyp = CType(CLstart.mycSimple.iniDict("ProbaugDB.dbtyp"), String)
                myGlobalz.sitzung.probaugREC = setDbRecTyp(myGlobalz.probaug_MYDB)
                myGlobalz.sitzung.probaugREC.mydb = CType(myGlobalz.probaug_MYDB.Clone, clsDatenbankZugriff)
            End With
            l("---------------------- ende")
        Catch ex As Exception
            l("Fehler in : " ,ex)
        End Try
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


    Public Shared Function setDbRecTyp(ByVal dummyDB As clsDatenbankZugriff) As IDB_grundfunktionen
        Dim dummREC As IDB_grundfunktionen = Nothing
        Try
            If dummyDB.dbtyp = "oracle" Then
                dummREC = New clsDBspecOracle
            End If
            If dummyDB.dbtyp = "mysql" Then
                dummREC = New clsDBspecMYSQL
            End If
            If dummyDB.dbtyp = "postgres" Then
                dummREC = New clsDBspecPostgres
            End If
            If dummyDB.dbtyp = "sqls" Then
                dummREC = New clsDBspecMSSQL
            End If
            If dummREC Is Nothing Then
                nachricht("Fehler: setDbRecTyp, ggf. ist der db-Typ unbekannt:" & dummyDB.getDBinfo(""))
            End If
            Return dummREC
        Catch ex As Exception
            nachricht_und_Mbox("Fehler: setDbRecTyp2,  :" ,ex)
            Return Nothing
        End Try
    End Function

    Shared Sub ini_bearbeiterrec()
        myglobalz.bearbeiter_MYDB.Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String) ' "localhost" 
        myglobalz.bearbeiter_MYDB.Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String) ' "paradigma"
        myglobalz.bearbeiter_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String) ' ""
        myglobalz.bearbeiter_MYDB.Tabelle = "bearbeiter" 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) ' "bearbeiter"
        myglobalz.bearbeiter_MYDB.username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String) ' "Paradigma"
        myglobalz.bearbeiter_MYDB.password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String) '"lkof4"
        myglobalz.bearbeiter_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String) ' "oracle" 
        myglobalz.sitzung.BearbeiterREC = setDbRecTyp(myglobalz.bearbeiter_MYDB)
        myglobalz.sitzung.BearbeiterREC.mydb = CType(myglobalz.bearbeiter_MYDB.Clone, clsDatenbankZugriff)
    End Sub

    Shared Sub ini_temprec()

        With myGlobalz.temp_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.sitzung.tempREC = setDbRecTyp(myGlobalz.temp_MYDB)
            myGlobalz.sitzung.tempREC.mydb = CType(myGlobalz.temp_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_temprec2()

        With myGlobalz.temp_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.sitzung.tempREC2 = setDbRecTyp(myGlobalz.temp_MYDB)
            myGlobalz.sitzung.tempREC2.mydb = CType(myGlobalz.temp_MYDB.Clone, clsDatenbankZugriff)
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
        myGlobalz.kontaktdaten_MYDB.Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String) '  paradigmaServer
        myGlobalz.kontaktdaten_MYDB.Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String) '"alb"
        myGlobalz.kontaktdaten_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) '"gmde"
        myGlobalz.kontaktdaten_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
        myGlobalz.kontaktdaten_MYDB.username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String) ' webgis_user
        myGlobalz.kontaktdaten_MYDB.password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String) ' webgis_PW
        myGlobalz.kontaktdaten_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String) 'paradigmadatentyp$
        myGlobalz.sitzung.kontaktdatenREC = setDbRecTyp(myGlobalz.kontaktdaten_MYDB)
        myGlobalz.sitzung.kontaktdatenREC.mydb = CType(myGlobalz.kontaktdaten_MYDB.Clone, clsDatenbankZugriff)
    End Sub
    Shared Sub ini_rechtsdb()
        myglobalz.gesetzdb_MYDB.Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String) '  paradigmaServer
        myglobalz.gesetzdb_MYDB.Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String) '"alb"
        myglobalz.gesetzdb_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) '"gmde"
        myglobalz.gesetzdb_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
        myglobalz.gesetzdb_MYDB.username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String) ' webgis_user
        myglobalz.gesetzdb_MYDB.password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String) ' webgis_PW
        myglobalz.gesetzdb_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String) 'paradigmadatentyp$ 
        myglobalz.sitzung.gesetzesdbREC = setDbRecTyp(myglobalz.gesetzdb_MYDB)
        myglobalz.sitzung.gesetzesdbREC.mydb = CType(myglobalz.gesetzdb_MYDB.Clone, clsDatenbankZugriff)
    End Sub
    Shared Sub ini_vorgang()
        myGlobalz.vorgang_MYDB.Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String) '  paradigmaServer
        myGlobalz.vorgang_MYDB.Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String) '"alb"
        myGlobalz.vorgang_MYDB.Tabelle = "vorgang" 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) '"gmde"
        myGlobalz.vorgang_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
        myGlobalz.vorgang_MYDB.username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String) ' webgis_user
        myGlobalz.vorgang_MYDB.password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String) ' webgis_PW
        myGlobalz.vorgang_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String) 'paradigmadatentyp$ 
        myGlobalz.sitzung.VorgangREC = setDbRecTyp(myGlobalz.vorgang_MYDB)
        myGlobalz.sitzung.VorgangREC.mydb = CType(myGlobalz.vorgang_MYDB.Clone, clsDatenbankZugriff)
    End Sub
    Shared Sub ini_raumbezug()
        myGlobalz.raumbezug_MYDB.Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String) '  paradigmaServer
        myGlobalz.raumbezug_MYDB.Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String) '"alb"
        myGlobalz.raumbezug_MYDB.Tabelle = "vorgang" 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) '"gmde"
        myGlobalz.raumbezug_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
        myGlobalz.raumbezug_MYDB.username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String) ' webgis_user
        myGlobalz.raumbezug_MYDB.password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String) ' webgis_PW
        myGlobalz.raumbezug_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String) 'paradigmadatentyp$
        myGlobalz.sitzung.raumbezugsRec = setDbRecTyp(myGlobalz.raumbezug_MYDB)
        myGlobalz.sitzung.raumbezugsRec.mydb = CType(myGlobalz.raumbezug_MYDB.Clone, clsDatenbankZugriff)
    End Sub

    Shared Sub ini_Ereignis()
        With myGlobalz.Ereignisse_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            .defaultSelectColumnString="ID,VORGANGSID,BESCHREIBUNG,DATUM,ART,RICHTUNG,NOTIZ,WVFAELLIG,QUELLE,PERSONENID,ISTRTF"
            myGlobalz.sitzung.EreignisseRec = setDbRecTyp(myGlobalz.Ereignisse_MYDB)
            myGlobalz.sitzung.EreignisseRec.mydb = CType(myGlobalz.Ereignisse_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub
    Shared Sub ini_Archiv()
        With myGlobalz.ARC_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = "dokumente" 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.Arc.ArcRec = setDbRecTyp(myGlobalz.ARC_MYDB)
            myGlobalz.Arc.ArcRec.mydb = CType(myGlobalz.ARC_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_Wiedervorlage()
        With myGlobalz.wiedervorlage_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = "wv" 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.sitzung.DBWiedervorlageREC = setDbRecTyp(myGlobalz.wiedervorlage_MYDB)
            myGlobalz.sitzung.DBWiedervorlageREC.mydb = CType(myGlobalz.wiedervorlage_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_Zahlung()
        With myGlobalz.zahlung_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = " " & CLstart.myViewsNTabs.tabZahlungen & " " ' CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.sitzung.zahlungsREC = setDbRecTyp(myGlobalz.zahlung_MYDB)
            myGlobalz.sitzung.zahlungsREC.mydb = CType(myGlobalz.zahlung_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_Beteiligte()
        With myGlobalz.beteiligte_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CLstart.myViewsNTabs.tabBeteiligte 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.sitzung.beteiligteREC = setDbRecTyp(myGlobalz.beteiligte_MYDB)
            myGlobalz.sitzung.beteiligteREC.mydb = CType(myGlobalz.beteiligte_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_BeteiligteVerwandt()
        With myGlobalz.beteiligte_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.sitzung.beteiligteRECVerwandt = setDbRecTyp(myGlobalz.beteiligte_MYDB)
            myGlobalz.sitzung.beteiligteRECVerwandt.mydb = CType(myGlobalz.beteiligte_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_vorgangsbeteiligteAuswahlREC()
        With myGlobalz.vorgangsbeteiligte_MYDB
            .Host = CType(CLstart.mycSimple.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(CLstart.mycSimple.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(CLstart.mycSimple.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(CLstart.mycSimple.iniDict("VorgangDB.username"), String)
            .password = CType(CLstart.mycSimple.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(CLstart.mycSimple.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC = setDbRecTyp(myGlobalz.vorgangsbeteiligte_MYDB)
            myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb = CType(myGlobalz.vorgangsbeteiligte_MYDB.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Private Shared Sub dbInstanzenbilden()
    End Sub

    Private Shared Sub dbInstanzenInitialisieren()
        ' ini_bearbeiterrec()
        ini_bearbeiterrec()
        ini_postgresrec()
        ini_Alb()
        ini_vorgang()
        ini_rechtsdb()
        ini_Kontaktdaten()
        ini_raumbezug()
        ini_Ereignis()
        ini_Archiv()
        ini_Wiedervorlage()
        ini_temprec()
        ini_temprec2()
        ini_Beteiligte()
        ini_BeteiligteVerwandt() 
        ini_probaugREC()
        ini_vorgangsbeteiligteAuswahlREC()
        ini_Zahlung()
        ini_webgisREC()
    End Sub  

    Public Shared Function getParadigmaSachgebietsdateiName(verzeichnis As String) As String
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
            nachricht("fehler getXmlTagValue ,  : TagsName: " & TagsName ,ex)
            Return ""
        End Try
    End Function

    Private Shared Sub ini_postgresrec()
        myGlobalz.postgres_MYDB.Host = CType(CLstart.mycSimple.iniDict("postgres_MYDB.MySQLServer"), String) ' "localhost"
        myGlobalz.postgres_MYDB.Schema = CType(CLstart.mycSimple.iniDict("postgres_MYDB.Schema"), String) ' "paradigma"
        myGlobalz.postgres_MYDB.ServiceName = CType(CLstart.mycSimple.iniDict("postgres_MYDB.ServiceName"), String) ' ""
        myGlobalz.postgres_MYDB.Tabelle = CType(CLstart.mycSimple.iniDict("postgres_MYDB.Tabelle"), String) ' "bearbeiter"
        myGlobalz.postgres_MYDB.username = CType(CLstart.mycSimple.iniDict("postgres_MYDB.username"), String) ' "Paradigma"
        myGlobalz.postgres_MYDB.password = CType(CLstart.mycSimple.iniDict("postgres_MYDB.password"), String) '"lkof4"
        myGlobalz.postgres_MYDB.dbtyp = CType(CLstart.mycSimple.iniDict("postgres_MYDB.dbtyp"), String) ' "oracle"
        myGlobalz.sitzung.postgresREC = setDbRecTyp(myGlobalz.postgres_MYDB)
        myGlobalz.sitzung.postgresREC.mydb = CType(myGlobalz.postgres_MYDB.Clone, clsDatenbankZugriff)
    End Sub

End Class
