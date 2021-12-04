Public Class DB_fork
    Public Shared Sub forkiniraumbezugsDT(ByRef erfolg As Boolean)

        If inidatabase.vorgang_MYDB.dbtyp = "oracle" Then
            erfolg = karte_oracle.initRaumbezugsDT(myGlobalz.VorgangsID)
        End If
        If inidatabase.vorgang_MYDB.dbtyp = "mysql" Then
            erfolg = karte_mysql.initRaumbezugsDT(myGlobalz.VorgangsID)
        End If

    End Sub
    Public Shared Function forkviakopplung_dok_vorgang(ByRef vid%) As Boolean 'myGlobalz.VorgangsID.ToString
        Dim erfolg As Boolean
        If inidatabase.vorgang_MYDB.dbtyp = "mysql" Then
            erfolg = karte_mysql.viaKopplung_DokumentIDs_VorgangID(CStr(vid))
        End If
        If inidatabase.vorgang_MYDB.dbtyp = "oracle" Then
            erfolg = karte_oracle.viaKopplung_DokumentIDs_VorgangID(CStr(vid))
        End If
        Return erfolg
    End Function

    Public Shared Sub insertFeatureClassIntoWebgiscontrolDB_alledb(ByVal KartenEbenenName$, ByVal appendix$, ByVal directory$, ByVal dbpfad$, ByVal halorec As IDB_grundfunktionen)
        If inidatabase.Webgis_MYDB.dbtyp = "mysql" Then
            Dim webgisc As New clsWebgiscontrol_Mysql(clsDBspecMYSQL.getConnection(inidatabase.Webgis_MYDB))
            webgisc.insertFeatureClassIntoWebgiscontrolDB(KartenEbenenName, appendix$, directory$, dbpfad$, "rbpoint.dbf", "paradigma", "intranet", halorec) '"umwelt,uwb,immi,probaug,denkmal")
        End If
        If inidatabase.Webgis_MYDB.dbtyp = "oracle" Then
            Dim webgisc As New clsWebgiscontrol_Oracle(clsDBspecOracle.getConnection(inidatabase.Webgis_MYDB))
            webgisc.insertFeatureClassIntoWebgiscontrolDB(KartenEbenenName, appendix$, directory$, dbpfad$, "rbpoint.dbf", "paradigma", "intranet", halorec) '"umwelt,uwb,immi,probaug,denkmal")
        End If
    End Sub

    Public Shared Function holePasswordFuerGisUser_dballe() As String
        Dim pw$
        If inidatabase.Webgis_MYDB.dbtyp = "mysql" Then
            Dim wbc As New clsWebgiscontrol_Mysql(clsDBspecMYSQL.getConnection(inidatabase.Webgis_MYDB))
            pw$ = wbc.holePasswordFuerGisUser(myGlobalz.Bearbeiter, myGlobalz.haloREC)
        End If
        If inidatabase.Webgis_MYDB.dbtyp = "oracle" Then
            Dim wbc As New clsWebgiscontrol_Oracle(clsDBspecMYSQL.getConnection(inidatabase.Webgis_MYDB))
            pw$ = wbc.holePasswordFuerGisUser(myGlobalz.Bearbeiter, myGlobalz.haloREC)
        End If
        Return pw
    End Function
End Class
