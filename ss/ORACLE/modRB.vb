Imports Devart
Module modRB


    'Public ereignisRec As LibDB.IDB_grundfunktionen
    Private ereignisRec As clsDBspecOracle
    Public MeineDBConnection As New Devart.Data.Oracle.OracleConnection
    Public com As New Devart.Data.Oracle.OracleCommand
    Function dodatenbank2() As Integer
        host = "ora-clu-vip-003"
        schema = "paradigma"
        Dim ServiceName = "paradigma.kreis-of.local"
        dbuser = "paradigma"
        dbpw = "luftikus12"

        MeineDBConnection = New Devart.Data.Oracle.OracleConnection("Data Source=(DESCRIPTION=" & _
                            "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & host & ")(PORT=1521)))" & _
                             "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" & _
                            "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & ServiceName & ")));" & _
                            "User Id=" & dbuser & ";Password=" & dbpw & ";")


        MeineDBConnection.Open()
        Dim SQLupdate$ =String.Format("INSERT INTO {0} (RAUMBEZUGSID,VORGANGSID,TYP,AREAQM,SERIALSHAPE) " +
                  " VALUES (:RAUMBEZUGSID,:VORGANGSID,:TYP,:AREAQM,:SERIALSHAPE)",
                   "RAUMBEZUG2GEOPOLYGON")
        SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"
        com = New Devart.Data.Oracle.OracleCommand(SQLupdate$, MeineDBConnection)

        com.Parameters.AddWithValue(":RAUMBEZUGSID", rid)
        com.Parameters.AddWithValue(":VORGANGSID", vid)
        com.Parameters.AddWithValue(":SERIALSHAPE", serial)
        com.Parameters.AddWithValue(":TYP", 2)
        com.Parameters.AddWithValue(":AREAQM", area)

        Dim newid = clsOracleIns.GetNewid(com, SQLupdate$)
        MeineDBConnection.Close()

    End Function
    Function doDatenbank() As Integer
        Dim ret As Integer
        ' My.Application.Log.WriteEntry("asdasd")
        l("doDatenbank -------------------------------------")
        Try
            ereignisRec = New clsDBspecOracle
            ereignisRec.mydb.Host = "ora-clu-vip-003"
            ereignisRec.mydb.Schema = "paradigma"
            ereignisRec.mydb.ServiceName = "paradigma.kreis-of.local"
            ereignisRec.mydb.username = "paradigma"
            ereignisRec.mydb.password = "luftikus12"

            MeineDBConnection = New Devart.Data.Oracle.OracleConnection("Data Source=(DESCRIPTION=" & _
                                "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & ereignisRec.mydb.Host & ")(PORT=1521)))" & _
                                 "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" & _
                                "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & ereignisRec.mydb.ServiceName & ")));" & _
                                "User Id=" & ereignisRec.mydb.username & ";Password=" & ereignisRec.mydb.password & ";")
            ret = RB_FLST_Serial_abspeichern_Neu(CInt(vid), CInt(rid), serial, 2, area)
            l("dberfolg ret: " & ret)
            l("doDatenbank -------------------------------------")
            Return ret
        Catch ex As Exception
            l("fehler in doDatenbank: " & ex.ToString)
            Return -1
        End Try
    End Function
    Public Function RB_FLST_Serial_abspeichern_Neu(ByVal vid%,
                                             ByVal rbid%,
                                             ByVal serial$,
                                             ByVal typ%,
                                             ByVal area As Double) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As Devart.Data.Oracle.OracleCommand
        '   Dim lokrec As New LIBoracle.clsDBspecOracle
        '   glob2.nachricht("RB_FLST_Serial_abspeichern_Neu -------------------------------------")
        '  lokrec = CType(myGlobalz.sitzung.tempREC.Clone(), clsDBspecMYSQL)'ihah
        Try

            ereignisRec.mydb.Tabelle = "RAUMBEZUG2GEOPOLYGON"

            Dim SQLupdate$ =
  String.Format("INSERT INTO {0} (RAUMBEZUGSID,VORGANGSID,TYP,AREAQM,SERIALSHAPE) " +
                        " VALUES (:RAUMBEZUGSID,:VORGANGSID,:TYP,:AREAQM,:SERIALSHAPE)",
                         ereignisRec.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"


            MeineDBConnection.Open()

            com = New Devart.Data.Oracle.OracleCommand(SQLupdate$, MeineDBConnection)

            setSQLParamsFLST_serial(com, vid%, rbid%, serial$, 0, typ, area)

            newid = clsOracleIns.GetNewid(com, SQLupdate$)
            MeineDBConnection.Close()
            Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate$)

        Catch mex As Devart.Data.Oracle.OracleException
            '  glob2.nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu mex: " & vbCrLf & mex.ToString)
            l("fehler in RB_FLST_Serial_abspeichern_Neu: " & mex.ToString)
            Return -2
        Catch ex As Exception
            ' glob2.nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu: " & vbCrLf & ex.ToString)
            l("fehler in RB_FLST_Serial_abspeichern_Neu: " & ex.ToString)
            Return -3
            'Finally
            '  lokrec = Nothing
        End Try
    End Function

    Sub setSQLParamsFLST_serial(ByVal com As Devart.Data.Oracle.OracleCommand, ByVal vid%, ByVal rbid%, ByVal serial$, ByVal id%, ByVal Typ%, ByVal areaqm As Double)
        com.Parameters.AddWithValue(":RAUMBEZUGSID", rbid%)
        com.Parameters.AddWithValue(":VORGANGSID", vid%)
        com.Parameters.AddWithValue(":SERIALSHAPE", serial$)
        com.Parameters.AddWithValue(":TYP", Typ)
        com.Parameters.AddWithValue(":AREAQM", areaqm)
        '   com.Parameters.AddWithValue(":ID", id)
    End Sub
End Module