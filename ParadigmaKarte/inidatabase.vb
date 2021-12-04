Public Class inidatabase
    Public Shared halo_MYDB As New clsDatenbankZugriff
    Public Shared Webgis_MYDB As New clsDatenbankZugriff
    Public Shared ARC_MYDB As New clsDatenbankZugriff
    Public Shared Raumbezug_MYDB As New clsDatenbankZugriff
    Public Shared vorgang_MYDB As New clsDatenbankZugriff
    Public Shared temp_MYDB As New clsDatenbankZugriff
    Shared Sub iniall()
        ' ini_DBserverNames_LOKALE_INSEL()
        'ini_DBserverNames()
        ini_haloREC()
        ini_raumbezug()
        ini_vorgangrec()
        ini_temprec()
        ini_ARCrec()
        ini_WebgisREC()
    End Sub

    Shared Sub ini_WebgisREC()
        With Webgis_MYDB
            .Host = CType(myGlobalz.iniDict("WebgisDB.MySQLServer"), String)
            .Schema = CType(myGlobalz.iniDict("WebgisDB.Schema"), String)
            .Tabelle = CType(myGlobalz.iniDict("WebgisDB.Tabelle"), String)
            .ServiceName = CType(myGlobalz.iniDict("WebgisDB.ServiceName"), String)
            .username = CType(myGlobalz.iniDict("WebgisDB.username"), String)
            .password = CType(myGlobalz.iniDict("WebgisDB.password"), String)
            .dbtyp = CType(myGlobalz.iniDict("WebgisDB.dbtyp"), String)
            myGlobalz.webgisREC = setDbRecTyp(Webgis_MYDB)
            myGlobalz.webgisREC.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_haloREC()
        With halo_MYDB
            .Host = CType(myGlobalz.iniDict("HaloDB.MySQLServer"), String)
            .Schema = CType(myGlobalz.iniDict("HaloDB.Schema"), String)
            .Tabelle = CType(myGlobalz.iniDict("HaloDB.Tabelle"), String)
            .ServiceName = CType(myGlobalz.iniDict("HaloDB.ServiceName"), String)
            .username = CType(myGlobalz.iniDict("HaloDB.username"), String)
            .password = CType(myGlobalz.iniDict("HaloDB.password"), String)
            .dbtyp = CType(myGlobalz.iniDict("HaloDB.dbtyp"), String)
            myGlobalz.haloREC = setDbRecTyp(halo_MYDB)
            myGlobalz.haloREC.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_ARCrec()
        'myGlobalz.ArcRec.mydb.Host = paradigmaServer
        'myGlobalz.ArcRec.mydb.Schema = paradigma_schema$
        'myGlobalz.ArcRec.mydb.Tabelle = "dokumente"
        'myGlobalz.ArcRec.mydb.username = paradigma_user
        'myGlobalz.ArcRec.mydb.password = paradigma_PW
        'myGlobalz.ArcRec.mydb.dbtyp = paradigmadatentyp$
        With ARC_MYDB
            .Host = CType(myGlobalz.iniDict("ArchivDB.MySQLServer"), String)
            .Schema = CType(myGlobalz.iniDict("ArchivDB.Schema"), String)
            .Tabelle = CType(myGlobalz.iniDict("ArchivDB.Tabelle"), String)
            .ServiceName = CType(myGlobalz.iniDict("ArchivDB.ServiceName"), String)
            .username = CType(myGlobalz.iniDict("ArchivDB.username"), String)
            .password = CType(myGlobalz.iniDict("ArchivDB.password"), String)
            .dbtyp = CType(myGlobalz.iniDict("ArchivDB.dbtyp"), String)
            myGlobalz.ArcRec = setDbRecTyp(ARC_MYDB)
            myGlobalz.ArcRec.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_temprec()
        'myGlobalz.tempREC.mydb.Host = paradigmaServer
        'myGlobalz.tempREC.mydb.Schema = paradigma_schema$
        'myGlobalz.tempREC.mydb.Tabelle = "vorgang"
        'myGlobalz.tempREC.mydb.username = paradigma_user
        'myGlobalz.tempREC.mydb.password = paradigma_PW
        'myGlobalz.tempREC.mydb.dbtyp = paradigmadatentyp$
        With temp_MYDB
            .Host = CType(myGlobalz.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(myGlobalz.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CType(myGlobalz.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(myGlobalz.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(myGlobalz.iniDict("VorgangDB.username"), String)
            .password = CType(myGlobalz.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(myGlobalz.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.tempREC = setDbRecTyp(temp_MYDB)
            myGlobalz.tempREC.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
    End Sub

    Shared Sub ini_vorgangrec()
        With vorgang_MYDB
            .Host = CType(myGlobalz.iniDict("VorgangDB.MySQLServer"), String)
            .Schema = CType(myGlobalz.iniDict("VorgangDB.Schema"), String)
            .Tabelle = CType(myGlobalz.iniDict("VorgangDB.Tabelle"), String)
            .ServiceName = CType(myGlobalz.iniDict("VorgangDB.ServiceName"), String)
            .username = CType(myGlobalz.iniDict("VorgangDB.username"), String)
            .password = CType(myGlobalz.iniDict("VorgangDB.password"), String)
            .dbtyp = CType(myGlobalz.iniDict("VorgangDB.dbtyp"), String)
            myGlobalz.VorgangREC = setDbRecTyp(vorgang_MYDB)
            myGlobalz.VorgangREC.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
        'myGlobalz.VorgangREC.mydb.Host = paradigmaServer
        'myGlobalz.VorgangREC.mydb.Schema = paradigma_schema$
        'myGlobalz.VorgangREC.mydb.Tabelle = "vorgang"
        'myGlobalz.VorgangREC.mydb.username = paradigma_user
        'myGlobalz.VorgangREC.mydb.password = paradigma_PW
        'myGlobalz.VorgangREC.mydb.dbtyp = paradigmadatentyp$
    End Sub

    Shared Sub ini_raumbezug()
        With Raumbezug_MYDB
            .Host = CType(myGlobalz.iniDict("RaumbezugDB.MySQLServer"), String)
            .Schema = CType(myGlobalz.iniDict("RaumbezugDB.Schema"), String)
            .Tabelle = CType(myGlobalz.iniDict("RaumbezugDB.Tabelle"), String)
            .ServiceName = CType(myGlobalz.iniDict("RaumbezugDB.ServiceName"), String)
            .username = CType(myGlobalz.iniDict("RaumbezugDB.username"), String)
            .password = CType(myGlobalz.iniDict("RaumbezugDB.password"), String)
            .dbtyp = CType(myGlobalz.iniDict("RaumbezugDB.dbtyp"), String)
            myGlobalz.raumbezugsRec = setDbRecTyp(Raumbezug_MYDB)
            myGlobalz.raumbezugsRec.mydb = CType(.Clone, clsDatenbankZugriff)
        End With
        'myGlobalz.raumbezugsRec.mydb.Host = paradigmaServer
        'myGlobalz.raumbezugsRec.mydb.Schema = paradigma_schema$
        'myGlobalz.raumbezugsRec.mydb.Tabelle = "raumbezug"
        'myGlobalz.raumbezugsRec.mydb.username = paradigma_user
        'myGlobalz.raumbezugsRec.mydb.password = paradigma_PW
        'myGlobalz.raumbezugsRec.mydb.dbtyp = paradigmadatentyp$
    End Sub

    Public Shared Function setDbRecTyp(ByVal dummyDB As clsDatenbankZugriff) As IDB_grundfunktionen
        Dim dummREC As IDB_grundfunktionen
        Try
            If dummyDB.dbtyp = "oracle" Then
                dummREC = New LIBoracle.clsDBspecOracle
            End If
            If dummyDB.dbtyp = "mysql" Then
                dummREC = New clsDBspecMYSQL
            End If
            If dummREC Is Nothing Then
                glob2.nachricht("Fehler: setDbRecTyp, ggf. ist der db-Typ unbekannt:" & dummyDB.getDBinfo(""))
            End If
            Return dummREC
        Catch ex As Exception
            glob2.nachricht_und_Mbox("Fehler: setDbRecTyp2,  :" & ex.ToString)
            Return Nothing
        End Try
    End Function
   
End Class
