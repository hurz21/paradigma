Imports System.Data
Module flaecheqmKopieren
    Private Property rid As Integer

    Sub rbsuchen()
        '7248 1482 (7249=
        'von 7249 flst haben 1482 kein polygon
        vorgangrec = New LIBoracle.clsDBspecOracle
        ereignisRec = New LIBoracle.clsDBspecOracle
        beteiligterec = New LIBoracle.clsDBspecOracle

        vorgangrec.mydb.Host = "ora-clu-vip-003"
        vorgangrec.mydb.Schema = "paradigma"
        vorgangrec.mydb.ServiceName = "paradigma.kreis-of.local"
        vorgangrec.mydb.username = "paradigma"
        vorgangrec.mydb.password = "luftikus12"

        ereignisRec.mydb.Host = "ora-clu-vip-003"
        ereignisRec.mydb.Schema = "paradigma"
        ereignisRec.mydb.ServiceName = "paradigma.kreis-of.local"
        ereignisRec.mydb.username = "paradigma"
        ereignisRec.mydb.password = "luftikus12"

        beteiligterec.mydb.Host = "ora-clu-vip-003"
        beteiligterec.mydb.Schema = "paradigma"
        beteiligterec.mydb.ServiceName = "paradigma.kreis-of.local"
        beteiligterec.mydb.username = "paradigma"
        beteiligterec.mydb.password = "luftikus12"

        'Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
        'MeineDBConnection = CType(conn, OracleConnection)

        MeineDBConnection = New OracleConnection("Data Source=(DESCRIPTION=" & _
     "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & vorgangrec.mydb.Host & ")(PORT=1521)))" & _
      "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" & _
     "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & vorgangrec.mydb.ServiceName & ")));" & _
     "User Id=" & vorgangrec.mydb.username & ";Password=" & vorgangrec.mydb.password & ";")



        ' vorgangrec.mydb.SQL = "SELECT * FROM stammdaten s,vorgang v where s.vorgangsid=v.vorgangsid"
        vorgangrec.mydb.SQL = "select  p.areaqm,r.flaecheqm,r.raumbezugsid from raumbezug r,raumbezug2geopolygon  p " &
                                " where r.flaecheqm<1 " &
                                " and p.areaqm>0 " &
                                " and r.raumbezugsid=p.raumbezugsid  "
        Dim hinweis As String = vorgangrec.getDataDT()

        Dim actionheroe As String = "", art As String = ""
        Dim vid As Integer
        Dim sw As New IO.StreamWriter("c:\vergleich.sql")
        Dim ANZgleich As Integer = 0
        Dim ungueltig As Integer = 0
        Dim updates As Integer = 0
        Dim anzahlOhnePolygon As Integer = 0
        Dim anzahl As Integer = vorgangrec.dt.Rows.Count
        Dim i As Integer = 0
        Dim ifixed As Integer = 0
        Dim ohneFS As Integer = 0
        Dim gemcode, fs, sekid As String
        Dim areaqm,flaecheqm As double

        For Each drr As DataRow In vorgangrec.dt.Rows
            '   vid = CInt(drr.Item("vorgangsid"))
            rid = CInt(drr.Item("raumbezugsid"))
             areaqm = CInt(drr.Item("areaqm"))
             flaecheqm = CInt(drr.Item("flaecheqm"))
            sw.WriteLine("update raumbezug set flaecheqm=" & areaqm & 
                         " where raumbezugsid=" & rid & ";")

            
            i += 1
        Next
        sw.flush
        sw.close
        Debug.Print("OhnePolygon " & vorgangrec.dt.Rows.Count & " ds sind gleich: " & anzahlOhnePolygon & " fixed: " & ifixed & " ohneFS: " & ohneFS)
        '  MsgBox("von " & vorgangrec.dt.Rows.Count & " ds sind gleich: " & ANZgleich & " ungültig: " & ungueltig & " updates: " & updates)
    End Sub

    Private Function getPolygonFromShapeFile(gemcode As String, fs As String, vid As Integer) As Boolean
        Dim serial As String = ""
        Dim XMLclientConfigDir As String
        Dim FsPositionInShapeFile As String = "1"
        Dim area As Double
        Dim erfolg As Boolean
        Try
            XMLclientConfigDir = "L:\paradigma\" 'C:\Users\Feinen_j\AppData\Roaming\Paradigma\config\combos\"
            erfolg = getSerialFromShapeOHneDLL(XMLclientConfigDir &
                                    "gemcode\" &
                                     gemcode & ".shp", _
                                    CStr(FsPositionInShapeFile), _
                                   fs,
                                    serial,
                                    area)

            If erfolg Then
                RB_FLST_Serial_abspeichern_Neu(vid, rid, serial, 2, area)
                Return True
            End If
            Return False
        Catch ex As Exception
            Debug.Print("fehler in getPolygonFromShapeFile: " & ex.ToString)
            Return False
        End Try
    End Function
    ''' <summary>
    ''' hier wird noch nicht die DLL benutzt. die folgt eine ebene tiefer
    ''' </summary>
    ''' <param name="shapefile"></param>
    ''' <param name="Key">spaltennummer im shapeattribut , 0-basiert</param>
    ''' <param name="Value">string</param>
    ''' <param name="serial">string</param>
    ''' <returns>true on success</returns>
    ''' <remarks></remarks>
    Function getSerialFromShapeOHneDLL(ByVal shapefile As String,
                                              ByVal Key As String,
                                              ByVal Value As String,
                                              ByRef serial As String,
                                              ByRef area As Double) As Boolean
        Try
            Dim td As New LIBmyMapWindowGis.MyMapWinGisTools
            Dim erfolg As Boolean = td.getSerialFromShape(shapefile, Key, Value) '"D:\fkatbig\alk\shape\data\basis.shp", CStr(3), "FS0607490100004204400")
            If erfolg Then
                'td.getShapeFromSerial(td.serializedShape)
                serial = td.serializedShape
                area = td.area
                Return True
            Else
                '  MsgBox("fehler")
                serial = ""
                Return False
            End If
        Catch ex As Exception
            nachricht("1Fehler in getSerialFromShape:" & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function
    Sub nachricht(text As String)
        Debug.Print(text)
    End Sub

    Public Function RB_FLST_Serial_abspeichern_Neu(ByVal vid%,
                                                 ByVal rbid%,
                                                 ByVal serial$,
                                                 ByVal typ%,
                                                 ByVal area As Double) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
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

            com = New OracleCommand(SQLupdate$, MeineDBConnection)

            setSQLParamsFLST_serial(com, vid%, rbid%, serial$, 0, typ, area)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate$)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate$)

        Catch mex As OracleException
            '  glob2.nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu mex: " & vbCrLf & mex.ToString)
            Return -2
        Catch ex As Exception
            ' glob2.nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu: " & vbCrLf & ex.ToString)
            Return -2
            'Finally
            '  lokrec = Nothing
        End Try
    End Function

    Sub setSQLParamsFLST_serial(ByVal com As OracleCommand, ByVal vid%, ByVal rbid%, ByVal serial$, ByVal id%, ByVal Typ%, ByVal areaqm As Double)
        com.Parameters.AddWithValue(":RAUMBEZUGSID", rbid%)
        com.Parameters.AddWithValue(":VORGANGSID", vid%)
        com.Parameters.AddWithValue(":SERIALSHAPE", serial$)
        com.Parameters.AddWithValue(":TYP", Typ)
        com.Parameters.AddWithValue(":AREAQM", areaqm)
        '   com.Parameters.AddWithValue(":ID", id)
    End Sub
End Module


