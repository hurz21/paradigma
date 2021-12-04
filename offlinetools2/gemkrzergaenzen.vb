Imports System.Data

Module gemkrzergaenzen
    Property abstrakt As String
    Property vid As Integer
    Sub machen()



        vorgangrec = New LIBoracle.clsDBspecOracle
        ereignisRec = New LIBoracle.clsDBspecOracle
        beteiligterec = New LIBoracle.clsDBspecOracle

        vorgangrec.mydb.Host = "ora-clu-vip-003"
        vorgangrec.mydb.Schema = "paradigma"
        vorgangrec.mydb.ServiceName = "paradigma.kreis-of.local"
        vorgangrec.mydb.username = "paradigma"
        vorgangrec.mydb.password = "luftikus12"
        vorgangrec.mydb.dbtyp="oracle"

        ereignisRec.mydb.Host = "ora-clu-vip-003"
        ereignisRec.mydb.Schema = "paradigma"
        ereignisRec.mydb.ServiceName = "paradigma.kreis-of.local"
        ereignisRec.mydb.username = "paradigma"
        ereignisRec.mydb.password = "luftikus12"
            ereignisRec.mydb.dbtyp="oracle"

        beteiligterec.mydb.Host = "ora-clu-vip-003"
        beteiligterec.mydb.Schema = "paradigma"
        beteiligterec.mydb.ServiceName = "paradigma.kreis-of.local"
        beteiligterec.mydb.username = "paradigma"
        beteiligterec.mydb.password = "luftikus12"
            beteiligterec.mydb.dbtyp="oracle"

        'Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
        'MeineDBConnection = CType(conn, OracleConnection)

        MeineDBConnection = New OracleConnection("Data Source=(DESCRIPTION=" & _
     "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & vorgangrec.mydb.Host & ")(PORT=1521)))" & _
      "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" & _
     "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & vorgangrec.mydb.ServiceName & ")));" & _
     "User Id=" & vorgangrec.mydb.username & ";Password=" & vorgangrec.mydb.password & ";")



        ' vorgangrec.mydb.SQL = "SELECT * FROM stammdaten s,vorgang v where s.vorgangsid=v.vorgangsid"
        ' vorgangrec.mydb.SQL = "select * from raumbezug where typ=2 order by raumbezugsid desc" &
        vorgangrec.mydb.SQL = "select s.vorgangsid,p.abstract from raumbezugplus p,stammdaten s where s.gemkrz is null " &
       " and p.vorgangsid =s.vorgangsid " &
       " and p.abstract not like  'Polygon%'"

        '       " and  to_char(s.aufnahme, 'YYYY')='2013' " &




        Dim hinweis As String = vorgangrec.getDataDT()

        Dim actionheroe As String = "", art As String = ""
        Dim vid As Integer
        Dim sw As New IO.StreamWriter("c:\vergleich.txt")
        Dim ANZgleich As Integer = 0
        Dim ungueltig As Integer = 0
        Dim updates As Integer = 0
        Dim anzahlOhnePolygon As Integer = 0
        Dim anzahl As Integer = vorgangrec.dt.Rows.Count
        Dim i As Integer = 0
        Dim ifixed As Integer = 0
        Dim ohneFS As Integer = 0
        Dim gemcode, fs, sekid As String
        Dim gemparms As New clsGemarkungsParams
        gemparms.init() : Dim result$ = "ERROR"
        Dim gem1, gem2, gem3, gemkrz As String
        'Dim a = From item In gemparms.parms Where item.gemarkungstext.ToLower = "disetesheim" Select item.gemeindetext
        'If a.ToArray.Length > 0 Then result$ = a.ToList(0).ToString
        For Each drr As DataRow In vorgangrec.dt.Rows
            '   vid = CInt(drr.Item("vorgangsid"))
            vid = CInt(drr.Item("vorgangsid"))
            abstrakt = CStr((drr.Item("abstract")))
            gem2 = "" : gemkrz = "" : gem1 = "" : gem3 = ""
            For Each gemp As clsGemarkungsParams In gemparms.parms
                If abstrakt.ToLower.Contains(gemp.gemarkungstext.ToLower) Then
                    gem2 = gemp.gemarkungskuerzel
                     gem2 = gemp.gemarkungstext.Substring(0, 2)
                    gem3 = gemp.gemeindetext.Substring(0, 2)
                    Exit For
                End If
            Next
            For Each gemp As clsGemarkungsParams In gemparms.parms
                If abstrakt.ToLower.Contains(gemp.gemeindetext.ToLower) Then
                    If gem2 = "" Then gem2 = gemp.gemeindetext.Substring(0, 2)
                    gem1 = gemp.gemeindetext.Substring(0, 2)
                    Exit For
                End If
            Next
            If gem1 = "" Then gem1 = gem3
            gemkrz = gem1 & "-" & gem2
            gemkrz = gemkrz.ToUpper
            If gemkrz.Length<>5 Then
                Debug.Print("keine gemkrz berechnet für " & vid & ", " & abstrakt)
                Continue for
            End If
            beteiligterec.mydb.SQL = "update stammdaten" &
                " set gemkrz='" & gemkrz & "' " &
                " where vorgangsid=" & vid

            Dim newid As Long
          
            beteiligterec.sqlexecute(newid)
            i += 1
        Next
        Debug.Print("OhnePolygon " & vorgangrec.dt.Rows.Count & " ds sind gleich: " & anzahlOhnePolygon & " fixed: " & ifixed & " ohneFS: " & ohneFS)

    End Sub

End Module
