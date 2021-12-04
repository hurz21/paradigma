Public Class INI_Databases
    Private Shared paradigmaServer As String
    Private Shared GIS_Server$, GIS_WebServer$, raumdbServer$, raumdbUser$, raumdbPW$
    Private Shared paradigma_user$, paradigma_PW$, webgis_PW$, webgis_user$, paradigmadatentyp$
    Private Shared paradigma_schema$
    Private Shared halo_server$
    Private Shared halo_schema$
    'Private Shared halo_user$
    'Private Shared halo_pw$
    Private Shared probaug_user$, probaug_pw$

    Shared Sub INI_All(ByVal modus$)
        Try
            myGlobalz.nachricht("in INIall")
            If modus$ = "zuhause" Then
                ini_DBserverNames_LOKALE_INSEL()
            Else
                ini_DBserverNames()
            End If
            myGlobalz.callREC = New LIBoracle.clsDBspecOracle
            myGlobalz.raumdbREC = New callmanagerDAL.clsDBspecOracle
            myGlobalz.tempREC = New callmanagerDAL.clsDBspecOracle
            myGlobalz.personenRec = New callmanagerDAL.clsDBspecOracle
            ini_vorgang()
            Ini_raumdbREC()
            ini_temprec()
            ini_personenrec()
            ini_loesung()
        Catch ex As System.Exception
            myGlobalz.nachricht("Fehler in INI_All: " & ex.ToString)
        End Try
    End Sub

    Shared Sub ini_DBserverNames_LOKALE_INSEL()
        paradigmaServer = "localhost"
        GIS_Server = "w2gis02"
        GIS_WebServer = "w2gis02.kreis-of.local" '"127.0.0.1"
        paradigma_user = "callmanager"
        paradigma_PW = "lkof4" ' "callme"
        webgis_user = "webgis"
        webgis_PW = "luftikus"
        paradigmadatentyp$ = "oracle"
        paradigma_schema$ = "callmanager"
        halo_server$ = paradigmaServer
        halo_schema$ = "halosort"
        probaug_user$ = "probaug"
        probaug_pw$ = "morrojable"
        myGlobalz.Paradigma_Sachgebietsdatei = "c:\appsconfig\sachgebiet.xml"
        raumdbServer$ = paradigmaServer
        raumdbUser$ = "root"
        raumdbPW$ = "lkof4"
    End Sub

    Shared Sub ini_DBserverNames()
        paradigmaServer = "ora-clu-vip-004"
        GIS_Server = "w2gis02"
        GIS_WebServer = "w2gis02.kreis-of.local" '"127.0.0.1"
        paradigma_user = "gis"
        paradigma_PW = "A604l6rrpn"
        webgis_user = "webgis"
        webgis_PW = "luftikus"
        paradigmadatentyp$ = "oracle"
        paradigma_schema$ = "gis.kreis-of.local"
        halo_server$ = "w2gis02"
        halo_schema$ = "halosort"
        probaug_user$ = "probaug"
        probaug_pw$ = "morrojable"
        Try
            myGlobalz.Paradigma_Sachgebietsdatei$ = "\\w2gis02\gdvell\callmanager\sachgebiet.xml"
            raumdbServer$ = "KIS"
            raumdbUser$ = "root"
            raumdbPW$ = "lkof4"
        Catch ex As System.Exception
            myGlobalz.nachricht("Fehler in ini_DBserverNames: " & ex.ToString)
        End Try
    End Sub

    Shared Sub ini_personenrec()
        Try
            myGlobalz.personenRec.mydb.MySQLServer = paradigmaServer
            myGlobalz.personenRec.mydb.Schema = paradigma_schema$
            myGlobalz.personenRec.mydb.Tabelle = "person"
            myGlobalz.personenRec.mydb.username = paradigma_user
            myGlobalz.personenRec.mydb.password = paradigma_PW
            myGlobalz.personenRec.mydb.dbtyp = paradigmadatentyp$
        Catch ex As System.Exception
            myGlobalz.nachricht("Fehler in ini_personenrec: " & ex.ToString)
        End Try
    End Sub
    Shared Sub ini_loesung()
        Try
            myGlobalz.LoesungsRec.mydb.MySQLServer = paradigmaServer
            myGlobalz.LoesungsRec.mydb.Schema = paradigma_schema$
            myGlobalz.LoesungsRec.mydb.Tabelle = "call"
            myGlobalz.LoesungsRec.mydb.username = paradigma_user
            myGlobalz.LoesungsRec.mydb.password = paradigma_PW
            myGlobalz.LoesungsRec.mydb.dbtyp = paradigmadatentyp$
        Catch ex As System.Exception
            myGlobalz.nachricht("Fehler in LoesungsRec: " & ex.ToString)
        End Try
    End Sub
    Shared Sub ini_vorgang()
        Try
            myGlobalz.callREC.mydb.MySQLServer = paradigmaServer
            myGlobalz.callREC.mydb.Schema = paradigma_schema$
            myGlobalz.callREC.mydb.Tabelle = "call"
            myGlobalz.callREC.mydb.username = paradigma_user
            myGlobalz.callREC.mydb.password = paradigma_PW
            myGlobalz.callREC.mydb.dbtyp = paradigmadatentyp$
        Catch ex As System.Exception
            myGlobalz.nachricht("Fehler in ini_vorgang: " & ex.ToString)
        End Try
    End Sub
    Shared Sub Ini_raumdbREC()
        Try
            myGlobalz.raumdbREC.mydb.MySQLServer = raumdbServer
            myGlobalz.raumdbREC.mydb.Schema = "raumdatenbank"
            myGlobalz.raumdbREC.mydb.Tabelle = "personal"
            myGlobalz.raumdbREC.mydb.username = raumdbUser
            myGlobalz.raumdbREC.mydb.password = raumdbPW
            myGlobalz.raumdbREC.mydb.dbtyp = paradigmadatentyp$
        Catch ex As System.Exception
            myGlobalz.nachricht("Fehler in Ini_raumdbREC: " & ex.ToString)
        End Try
    End Sub
    Shared Sub ini_temprec()
        Try
            myGlobalz.tempREC.mydb.MySQLServer = paradigmaServer
            myGlobalz.tempREC.mydb.Schema = paradigma_schema$
            myGlobalz.tempREC.mydb.Tabelle = "call"
            myGlobalz.tempREC.mydb.username = paradigma_user
            myGlobalz.tempREC.mydb.password = paradigma_PW
            myGlobalz.tempREC.mydb.dbtyp = paradigmadatentyp$
        Catch ex As System.Exception
            myGlobalz.nachricht("Fehler in ini_temprec: " & ex.ToString)
        End Try
    End Sub
End Class
