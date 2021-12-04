Imports System.Data
Imports System.Threading.Tasks
Imports LibDB

'Imports System.Data.OracleClient
Module Module1
    Public vorgangrec As IDB_grundfunktionen
    Public rbrec As IDB_grundfunktionen
    Public beteiligterec As IDB_grundfunktionen
    Public ereignisRec As IDB_grundfunktionen
    Public MeineDBConnection As New OracleConnection
    Public com As New OracleCommand

    Private Property anzahlTreffer As Integer
    <Runtime.CompilerServices.Extension()> _
    Public Function IsNothingOrEmpty(ByRef dt As DataTable) As Boolean
        Dim result As Boolean = (dt Is Nothing)
        If Not result Then result = dt.Rows.Count = 0
        Return result
    End Function
    <Runtime.CompilerServices.Extension()> _
    Public Function IsNothingOrEmpty(ByRef text As String) As Boolean
        Return String.IsNullOrEmpty(text)
    End Function
    <Runtime.CompilerServices.Extension()> _
    Public Function IsNothingOrEmpty(ByRef icoll As ICollection) As Boolean
        Return icoll Is Nothing Or icoll.Count = 0
    End Function
    Sub main()
        getAmph()
        'kartei.procFile()
        '   updateLastEreigniss()
        ' modThumbnail.getAllTn(9609)
        ' updateHatRaumbezug()
        '  hausnummerkorrekturBeteiligte()
        ' hausnummernkorrekturAdresse()
        ' rbgigs.flstOhnePolygonSuchen()
        'gemkrzergaenzen.machen
        'flaecheqmKopieren.rbsuchen
        'updateDokumenteVidEid.exe
        'modHuetten. huettenTDaufraeumen
    End Sub

    Private Sub getAmph()
        '"http://w2gis02.kreis-of.local/cgi-bin/apps/gis/getrecord/getrecord4template.cgi?lookup=true&aktive_ebene=amphibien&object_id=138&templatefile=MSKamphibien_2.htm&activelayer=amphibien&apppfad=/profile/register/"
        Dim url As String = "http://w2gis02.kreis-of.local/cgi-bin/apps/gis/getrecord/getrecord4template.cgi?lookup=true&aktive_ebene=amphibien&object_id=138&templatefile=MSKamphibien_2.htm&activelayer=amphibien&apppfad=/profile/register/"
        Dim hinweis As String = ""

        Dim result As String = CLstart.meineHttpNet.meinHttpJob("", url, hinweis)
    End Sub

    'erwartet als Parameter ein Feld eines Datensatzes (datarow!spaltenname)
    'liefert den Wert des Parameters oder Nothing, wenn der Parameter DBNull.Value enthält
    Public Function fieldvalue(ByVal obj As Object) As String
        If obj Is DBNull.Value Then
            Return ""
        Else
            Return obj.ToString
        End If
    End Function

    Sub updateLastEreigniss()
        vorgangrec = New clsDBspecOracle
        ereignisRec = New clsDBspecOracle

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

        'Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
        'MeineDBConnection = CType(conn, OracleConnection)

        MeineDBConnection = New OracleConnection("Data Source=(DESCRIPTION=" & _
     "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & vorgangrec.mydb.Host & ")(PORT=1521)))" & _
      "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" & _
     "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & vorgangrec.mydb.ServiceName & ")));" & _
     "User Id=" & vorgangrec.mydb.username & ";Password=" & vorgangrec.mydb.password & ";")



        ' vorgangrec.mydb.SQL = "SELECT * FROM stammdaten s,vorgang v where s.vorgangsid=v.vorgangsid"
        vorgangrec.mydb.SQL = "select s.vorgangsid,s.letztebearbeitung,e.lastone " &
                                "from stammdaten s ,letztesereignisdatumim_vorgang e " &
                                "       where s.vorgangsid = e.vorgangsid " &
                                "and  to_date(s.letztebearbeitung)   != to_date(e.lastone) order by s.vorgangsid desc"
        Dim hinweis As String = vorgangrec.getDataDT()
        Dim datum, altdatum As Date
        Dim actionheroe As String = "", art As String = ""
        Dim vid As Integer
        Dim sw As New IO.StreamWriter("c:\vergleich.txt")
        Dim ANZgleich As Integer = 0
        Dim ungueltig As Integer = 0
        Dim updates As Integer = 0

        For Each drr As DataRow In vorgangrec.dt.Rows
            vid = CInt(drr.Item("vorgangsid"))
            altdatum = CDate(drr.Item("LETZTEBEARBEITUNG"))

            Debug.Print("aktuell vid=" & vid)
            'ereignis holen
            ereignisRec.mydb.SQL = " select datum,art,Beschreibung,id from ereignis" &
                                      "  where datum=(select  max(datum) from ereignis " &
                                      "  where vorgangsid=" & vid & ")"
            hinweis = ereignisRec.getDataDT()
            art = Nothing
            Try
                datum = CDate(ereignisRec.dt.Rows(0).Item("datum"))
                Dim eid% = CInt(ereignisRec.dt.Rows(0).Item("id"))
                art = fieldvalue(ereignisRec.dt.Rows(0).Item("Art"))
                actionheroe = art & ": " & CStr(ereignisRec.dt.Rows(0).Item("Beschreibung"))
            Catch ex As Exception
                Debug.Print(ex.ToString)
            End Try
            If art Is Nothing Then
                ungueltig += 1
                Continue For
            End If

            If Year(altdatum) = Year(datum) And
               Month(altdatum) = Month(datum) And
               Day(altdatum) = Day(datum) And
               Hour(altdatum) = Hour(datum) Then
                ANZgleich += 1
                Continue For
            End If


            If String.IsNullOrEmpty(actionheroe) Then
                Continue For
            End If

            'update der stammdaten durchführen
            ' com.Parameters.AddWithValue("@LetzteBearbeitung", Convert.ToDateTime(Format(Now, "yyyy-MM-dd HH:mm:ss")))
            vorgangrec.mydb.SQL = "update stammdaten set " &
                                " LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " &
                                ",LASTACTIONHEROE=:LASTACTIONHEROE " &
                                " where VorgangsID=:VorgangsID"

            vorgangrec.mydb.Tabelle = "stammdaten"
            'Dim SQLupdate$ = String.Format("UPDATE  {0}{1}  where VorgangsID=:VorgangsID", vorgangrec.mydb.Tabelle, setSQLBodyOHnePermanente())

            MeineDBConnection.Open()

            com = New OracleCommand(vorgangrec.mydb.SQL, MeineDBConnection)
            com.Parameters.AddWithValue(":LETZTEBEARBEITUNG", Convert.ToDateTime(datum))
            com.Parameters.AddWithValue(":LASTACTIONHEROE", actionheroe)
            com.Parameters.AddWithValue(":VorgangsID", vid)

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
    Function setSQLBody() As String
        Return " set " & _
        " VORGANGSID=:VORGANGSID" & _
        ",BEARBEITER=:BEARBEITER" & _
        ",BEMERKUNG=:BEMERKUNG " & _
        ",BESCHREIBUNG=:BESCHREIBUNG " & _
        ",EINGANG=:EINGANG " & _
        ",AUFNAHME=:AUFNAHME " & _
        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
        ",ERLEDIGT=:ERLEDIGT " & _
        ",ORTSTERMIN=:ORTSTERMIN " & _
        ",STELLUNGNAHME=:STELLUNGNAHME " & _
        ",PROBAUGAZ=:PROBAUGAZ " & _
        ",ALTAZ=:ALTAZ " & _
        ",GEMKRZ=:GEMKRZ " & _
        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
        ",AZ2=:AZ2 " & _
        ",WEITEREBEARB=:WEITEREBEARB " & _
        ",ARCDIR=:ARCDIR " & _
        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
        ",ABGABEBA=:ABGABEBA " &
        ",GUTACHTENMIT=:GUTACHTENMIT " &
        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
        ",STORAUMNR=:STORAUMNR " &
        ",STOTITEL=:STOTITEL " &
        ",PARAGRAF=:PARAGRAF " &
        ",INTERNENR=:INTERNENR "
    End Function

    Function setSQLBodyOHnePermanente() As String
        Return " set " & _
        " VORGANGSID=:VORGANGSID" & _
        ",BEARBEITER=:BEARBEITER" & _
        ",BEMERKUNG=:BEMERKUNG " & _
        ",BESCHREIBUNG=:BESCHREIBUNG " & _
        ",EINGANG=:EINGANG " & _
        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
        ",ERLEDIGT=:ERLEDIGT " & _
        ",ORTSTERMIN=:ORTSTERMIN " & _
        ",STELLUNGNAHME=:STELLUNGNAHME " & _
        ",PROBAUGAZ=:PROBAUGAZ " & _
        ",ALTAZ=:ALTAZ " & _
        ",GEMKRZ=:GEMKRZ " & _
        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
        ",AZ2=:AZ2 " & _
        ",WEITEREBEARB=:WEITEREBEARB " & _
        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
        ",ABGABEBA=:ABGABEBA " &
        ",GUTACHTENMIT=:GUTACHTENMIT " &
        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
        ",STORAUMNR=:STORAUMNR " &
        ",STOTITEL=:STOTITEL " &
        ",PARAGRAF=:PARAGRAF " &
        ",INTERNENR=:INTERNENR "
        '  ",ARCDIR=:ARCDIR " & _  sollte nicht mehr verändert werden
        '    ",AUFNAHME=:AUFNAHME " & _
    End Function


    Private Sub updateHatRaumbezug()
        vorgangrec = New clsDBspecOracle
        ereignisRec = New clsDBspecOracle
        rbrec = New clsDBspecOracle

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



        vorgangrec.mydb.SQL = "SELECT * FROM stammdaten s,vorgang v where s.vorgangsid=v.vorgangsid and s.hatraumbezug=0"
        Dim hinweis As String = vorgangrec.getDataDT()
        'Dim datum, altdatum As Date
        Dim actionheroe As String = "", art As String = ""
        Dim vid As Integer
        Dim sw As New IO.StreamWriter("c:\vergleich.txt")
        Dim ANZgleich As Integer = 0
        Dim ungueltig As Integer = 0
        Dim updates As Integer = 0
        Dim anzahl As Integer = 0

        For Each drr As DataRow In vorgangrec.dt.Rows
            vid = CInt(drr.Item("vorgangsid"))
            'altdatum = CDate(drr.Item("LETZTEBEARBEITUNG"))

            Debug.Print("aktuell vid=" & vid)
            'ereignis holen
            ereignisRec.mydb.SQL = " select s.vorgangsid from stammdaten s " &
                                    " where(s.vorgangsid = " & vid & ")" &
                                    " and s.vorgangsid in " &
                                    " (select rv.vorgangsid  from raumbezug2vorgang rv, raumbezug r " &
                                    " where rv.raumbezugsid=r.raumbezugsid) "
            hinweis = ereignisRec.getDataDT()
            If ereignisRec.dt.IsNothingOrEmpty Then
                anzahl = 0
                Continue for
            Else
                anzahl = 1

            End If
            art = Nothing


            'update der stammdaten durchführen
            vorgangrec.mydb.SQL = "update stammdaten set " &
                                  " HATRAUMBEZUG=:HATRAUMBEZUG " &
                                  " where VorgangsID=:VorgangsID"

            vorgangrec.mydb.Tabelle = "stammdaten"
            'Dim SQLupdate$ = String.Format("UPDATE  {0}{1}  where VorgangsID=:VorgangsID", vorgangrec.mydb.Tabelle, setSQLBodyOHnePermanente())

            MeineDBConnection.Open()

            com = New OracleCommand(vorgangrec.mydb.SQL, MeineDBConnection)
            com.Parameters.AddWithValue(":HATRAUMBEZUG", Convert.ToInt16(anzahl)) 'ooooooooooo was ist wenn anzahl > 1
            com.Parameters.AddWithValue(":VorgangsID", vid)

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

    Private Sub hausnummerkorrekturBeteiligte()
        Dim t = Task.Factory.StartNew(Sub() hausnummernKorrekturBeteiligte.korr())
        t.Wait()
    End Sub

    Private Sub hausnummernkorrekturAdresse()
        Dim t = Task.Factory.StartNew(Sub() haunummernorrekturPARAADRESSE.korr())
        t.Wait()
    End Sub

End Module
