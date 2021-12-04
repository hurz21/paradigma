Imports System.Data

Module updateDokumenteVidEid
    Public Sub exe()
        vorgangrec = New LIBoracle.clsDBspecOracle
        ereignisRec = New LIBoracle.clsDBspecOracle
        rbrec = New LIBoracle.clsDBspecOracle

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


        rbrec.mydb.Host = "ora-clu-vip-003"
        rbrec.mydb.Schema = "paradigma"
        rbrec.mydb.ServiceName = "paradigma.kreis-of.local"
        rbrec.mydb.username = "paradigma"
        rbrec.mydb.password = "luftikus12"

        'Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
        'MeineDBConnection = CType(conn, OracleConnection)

        MeineDBConnection = New OracleConnection("Data Source=(DESCRIPTION=" & _
     "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & vorgangrec.mydb.Host & ")(PORT=1521)))" & _
      "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" & _
     "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & vorgangrec.mydb.ServiceName & ")));" & _
     "User Id=" & vorgangrec.mydb.username & ";Password=" & vorgangrec.mydb.password & ";")



        vorgangrec.mydb.SQL = "select * from dokumente where vid is null order by dokumentid desc"
        Dim hinweis As String = vorgangrec.getDataDT()
        'Dim datum, altdatum As Date
        Dim actionheroe As String = "", art As String = ""
        Dim vid ,did,eid As Integer
        Dim sw As New IO.StreamWriter("c:\vergleich.txt")
        Dim ANZgleich As Integer = 0
        Dim ungueltig As Integer = 0
        Dim updates As Integer = 0
        Dim anzahl As Integer = 0
        
        For Each drr As DataRow In vorgangrec.dt.Rows
            did = CInt(drr.Item("dokumentid"))
            'altdatum = CDate(drr.Item("LETZTEBEARBEITUNG"))

            Debug.Print("aktuell vid=" & vid)
            'ereignis holen
            ereignisRec.mydb.SQL = " (select vorgangsid  from  dokument2vorgang where dokumentid=" & did & " )" &
                                    " union " &
                                    "(select ereignisid    from  dokument2ereignis where dokumentid=" & did & ") "
            hinweis = ereignisRec.getDataDT()
            If ereignisRec.dt.IsNothingOrEmpty Then
                anzahl = 0
            Else
                anzahl = 1
                vid = CInt(ereignisRec.dt.Rows(0).Item(0))
                Try
                     eid = CInt(ereignisRec.dt.Rows(1).Item(0))
                Catch ex As Exception
                    eid=0
                End Try
               
      
            End If
            art = Nothing


            'update der stammdaten durchführen
            vorgangrec.mydb.SQL = "update dokumente set " &
                                  " VID=" & vid.ToString  &
                                  ",EID=" & eid.ToString &
                                  " where dokumentid=" & did.tostring

            vorgangrec.mydb.Tabelle = "dokumentid"
            'Dim SQLupdate$ = String.Format("UPDATE  {0}{1}  where VorgangsID=:VorgangsID", vorgangrec.mydb.Tabelle, setSQLBodyOHnePermanente())

            MeineDBConnection.Open()

            com = New OracleCommand(vorgangrec.mydb.SQL, MeineDBConnection)
            'com.Parameters.AddWithValue(":HATRAUMBEZUG", Convert.ToInt16(anzahl)) 'ooooooooooo was ist wenn anzahl > 1
            'com.Parameters.AddWithValue(":VorgangsID", vid)

            Dim anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()

            If anzahlTreffer < 1 Then
                Debug.Print("problem")
            Else
                Debug.Print("ok")
                updates += 1
            End If
        Next
        Debug.Print("von " & vorgangrec.dt.Rows.Count & " ds sind gleich: " & ANZgleich & " ungültig: " & ungueltig & " updates: " & updates)
        MsgBox("von " & vorgangrec.dt.Rows.Count & " ds sind gleich: " & ANZgleich & " ungültig: " & ungueltig & " updates: " & updates)
    End Sub
End Module
