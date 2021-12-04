'Imports Devart.Data.Oracle
'Imports Devart.Data
Imports System.Data
Imports koloReport

Module modOracle
    '    Function getDTOracle(sql As String) As DataTable
    '        l("in getDT -------------------------------------")
    '        'Dim host, datenbank, schema, tabelle, dbuser, dbpw, dbport As String
    '        Dim dt As New DataTable
    '        Dim myoracle As Oracle.OracleConnection
    '        makeConnectionOracle(myoracle)
    '        myoracle.Open()
    '        Dim com As New OracleCommand(sql, myoracle)
    '        Dim da As New OracleDataAdapter(com)
    '        'da.MissingSchemaAction = MissingSchemaAction.AddWithKey
    '        Dim mycount As Integer
    '        mycount = da.Fill(dt)
    '        Try
    '            com.Dispose()
    '            da.Dispose()
    '            myoracle.Close()
    '            myoracle.Dispose()
    '            Return dt
    '        Catch oex As OracleException
    '            l("Fehler in GetNewid&:" & oex.ToString & " / " & sql)
    '            Return Nothing
    '        Catch ex As Exception
    '            l("Fehler in getDT&:" & ex.ToString & " / " & sql)
    '            Return Nothing
    '        Finally
    '            myoracle.Close()
    '        End Try
    '    End Function

    '    Private Sub makeConnectionOracle(ByRef myoracle As OracleConnection)
    '        Dim ServiceName As String
    '        Dim host As String = "ora-clu-vip-003"
    '        Dim schema As String = "paradigma"
    '        ServiceName = "paradigma.kreis-of.local"
    '        Dim dbuser As String = "paradigma"
    '        Dim dbpw As String = "luftikus12"
    '        Dim v As String = "Data Source=(DESCRIPTION=" &
    '                            "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & host & ")(PORT=1521)))" &
    '                             "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" &
    '                            "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & ServiceName & ")));" &
    '                            "User Id=" & dbuser & ";Password=" & dbpw & ";direct=true;"
    '        myoracle = New Devart.Data.Oracle.OracleConnection(v)
    '    End Sub

    'Friend Function checkInDBDokuOracle(aktdoku As Dokument) As Integer
    '    l("in checkInDBDoku -------------------------------------")
    '    Dim host, datenbank, schema, tabelle, dbuser, dbpw, dbport As String
    '    Dim dt As New DataTable
    '    Dim myoracle As Oracle.OracleConnection
    '    Dim Sql As String =
    '     String.Format("INSERT INTO {0} (RELATIVPFAD,DATEINAMEEXT,TYP,BESCHREIBUNG,CHECKINDATUM,FILEDATUM,EXIFDATUM,EXIFLONG,EXIFLAT,EXIFDIR," +
    '                        "EXIFHERSTELLER,ORIGINALFULLNAME,INITIAL_,REVISIONSSICHER,ORIGINALNAME,VID,EID,NEWSAVEMODE) " +
    '             " VALUES (:RELATIVPFAD,:DATEINAMEEXT,:TYP,:BESCHREIBUNG,:CHECKINDATUM,:FILEDATUM,:EXIFDATUM,:EXIFLONG,:EXIFLAT,:EXIFDIR," +
    '                       ":EXIFHERSTELLER,:ORIGINALFULLNAME,:INITIAL_,:REVISIONSSICHER,:ORIGINALNAME,:VID,:EID,:NEWSAVEMODE)",
    '                       "dokumente")
    '    Sql = Sql & " RETURNING DOKUMENTID INTO :R1"

    '    l("nach setSQLbody : " & Sql)


    '    makeConnectionOracle(myoracle)
    '    myoracle.Open()
    '    Dim com As New OracleCommand(Sql, myoracle)
    '    Dim da As New OracleDataAdapter(com)
    '    Dim newid& = -1
    '    da.MissingSchemaAction = MissingSchemaAction.AddWithKey

    '    Dim erfolg As Boolean
    '    erfolg = setSQLParamsDokOracle(aktdoku, com)
    '    newid = clsOracleIns.GetNewid(com, Sql)
    '    myoracle.Close()
    '    Return clsOracleIns.gebeNeuIDoderFehler(newid, Sql)
    '    Try
    '        com.Dispose()
    '        da.Dispose()
    '        myoracle.Close()
    '        myoracle.Dispose()
    '        Return dt
    '    Catch oex As OracleException
    '        l("Fehler in GetNewid&:" & oex.ToString & " / " & Sql)
    '        Return Nothing
    '    Catch ex As Exception
    '        l("Fehler in checkInDBDoku&:" & ex.ToString & " / " & Sql)
    '        Return Nothing
    '    Finally
    '        myoracle.Close()
    '    End Try

    'End Function

    '    Private Function setSQLParamsDokOracle(aktdoku As Dokument, com As OracleCommand) As Boolean
    '        Try
    '            com.Parameters.AddWithValue(":RELATIVPFAD", aktdoku.dokumentPfad.Replace("\", "/"))
    '            com.Parameters.AddWithValue(":DATEINAMEEXT", aktdoku.DateinameMitExtension)
    '            com.Parameters.AddWithValue(":TYP", aktdoku.Typ)
    '            com.Parameters.AddWithValue(":BESCHREIBUNG", aktdoku.Beschreibung)
    '            com.Parameters.AddWithValue(":CHECKINDATUM", DateTime.Now())
    '            com.Parameters.AddWithValue(":FILEDATUM", DateTime.Now)
    '            com.Parameters.AddWithValue(":EXIFDATUM", DateTime.Now)
    '            com.Parameters.AddWithValue(":EXIFLONG", "")
    '            com.Parameters.AddWithValue(":EXIFLAT", "")
    '            com.Parameters.AddWithValue(":EXIFDIR", "")
    '            com.Parameters.AddWithValue(":EXIFHERSTELLER", "")
    '            com.Parameters.AddWithValue(":ORIGINALFULLNAME", aktdoku.OriginalFullname)
    '            com.Parameters.AddWithValue(":INITIAL_", aktdoku.Initiale)
    '            com.Parameters.AddWithValue(":REVISIONSSICHER", aktdoku.revisionssicher)
    '            com.Parameters.AddWithValue(":NEWSAVEMODE", CInt(aktdoku.newSaveMode))
    '            com.Parameters.AddWithValue(":ORIGINALNAME", aktdoku.OriginalName)
    '            com.Parameters.AddWithValue(":VID", aktdoku.VorgangsID)
    '            com.Parameters.AddWithValue(":EID", aktdoku.EreignisID)
    '            Return True
    '        Catch ex As Exception
    '            l("fehler in setSQLParams" & ex.ToString)
    '            Return False
    '        End Try
    '    End Function
    '    '  Private Sub NeuesEreignisSpeichern_alleDB(vid As String, v As String, aktEreignis As clsEreignis)
    'Public Function Neu_speichern_EreignisOracle(ByVal zielvorgangsid As Integer, ByVal modus As String,
    '                                             ereignis As clsEreignis) As Integer ' myGlobalz.sitzung.Ereignismodus
    '    l("Neu_speichern_Ereignis -----------------------------------------------------")
    '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
    '    Dim com As OracleCommand
    '    Dim SQLupdate$ = ""
    '    Dim myoracle As Oracle.OracleConnection
    '    Try
    '        '  myglobalz.sitzung.VorgangREC.mydb.Tabelle = 
    '        If String.IsNullOrEmpty(modus) Then
    '            l("Fehler: ereingismodus ist nicht nothing")
    '            Return -3
    '        End If
    '        If modus.ToLower = "neu" Then
    '            SQLupdate$ = String.Format("INSERT INTO {0} (VORGANGSID,BESCHREIBUNG,DATUM,ART,RICHTUNG,NOTIZ,DOKUMENTID,PERSONENID,QUELLE,ISTRTF) " +
    '                                  " VALUES (:VORGANGSID,:BESCHREIBUNG,:DATUM,:ART,:RICHTUNG,:NOTIZ,:DOKUMENTID,:PERSONENID,:QUELLE,:ISTRTF)",
    '                                    "ereignis")
    '            SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"
    '            avoidEreignisNulls(ereignis)
    '            l("nach setSQLbody : " & SQLupdate)
    '            makeConnectionOracle(myoracle)
    '            myoracle.Open()
    '            l("nach dboeffnen  ")

    '            com = New OracleCommand(SQLupdate$, myoracle)
    '            l("vor setParams  ")
    '            setParamEreignisoracle(com, 0, zielvorgangsid, ereignis)
    '            newid = clsOracleIns.GetNewid(com, SQLupdate)
    '            myoracle.Close()
    '        End If
    '        If newid < 1 Then
    '            l("Problem beim Abspeichern des Ereignisses: " & SQLupdate)
    '            Return -1
    '        Else
    '            ereignis.ID = CInt(newid)
    '            l("Neu_speichern_Ereignis funzt")
    '            ereignis.ID = CInt(newid)
    '            Return CInt(newid)
    '        End If
    '    Catch ex As Exception
    '        l("Fehler beim Abspeichern: " & ex.ToString)
    '        ereignis.ID = 0I
    '        Return -2
    '    End Try
    'End Function

    Sub avoidEreignisNulls(ereignis As clsEreignis)
        With ereignis 'myGlobalz.sitzung.aktEreignis
            If String.IsNullOrEmpty(.Notiz) Then .Notiz = " "
            If String.IsNullOrEmpty(.Beschreibung) Then .Beschreibung = ""
            If String.IsNullOrEmpty(.Quelle) Then .Quelle = getInitial(Environment.UserName)
            If String.IsNullOrEmpty(.Art) Then .Art = ""

            If .Art.Length > 41 Then .Art = .Art.Substring(0, 40)
            If .Quelle.Length > 41 Then .Art = .Quelle.Substring(0, 40)
            If .Beschreibung.Length > 399 Then .Beschreibung = .Beschreibung.Substring(0, 398)
        End With
    End Sub

    'Private Sub setParamEreignisoracle(ByVal com As OracleCommand,
    '                      ByVal eid As Integer,
    '                      ByVal zielvorgangsid As Integer,
    '                      ereignis As clsEreignis)
    '    l("ereignis setParams ---------------------------------")
    '    Dim zeiger = 1
    '    'datum umsetzen für oracle
    '    With ereignis 'myGlobalz.sitzung.aktEreignis
    '        If String.IsNullOrEmpty(.Notiz) Then .Notiz = " "
    '        If String.IsNullOrEmpty(.Beschreibung) Then .Beschreibung = ""
    '        If String.IsNullOrEmpty(.Quelle) Then .Quelle = getInitial(Environment.UserName)
    '        If String.IsNullOrEmpty(.Art) Then .Art = ""

    '        If .Art.Length > 41 Then .Art = .Art.Substring(0, 40)
    '        If .Quelle.Length > 41 Then .Art = .Quelle.Substring(0, 40)
    '        If .Beschreibung.Length > 399 Then .Beschreibung = .Beschreibung.Substring(0, 398)
    '    End With

    '    Try
    '        '    With ereignis 'myGlobalz.sitzung.aktEreignis
    '        com.Parameters.AddWithValue(":VORGANGSID", zielvorgangsid) : zeiger = 2
    '        com.Parameters.AddWithValue(":BESCHREIBUNG", ereignis.Beschreibung) : zeiger = 3
    '        com.Parameters.AddWithValue(":DATUM", CDate(ereignis.Datum)) : zeiger = 4
    '        com.Parameters.AddWithValue(":ART", ereignis.Art) : zeiger = 5
    '        com.Parameters.AddWithValue(":RICHTUNG", ereignis.Richtung) : zeiger = 6
    '        com.Parameters.AddWithValue(":NOTIZ", ereignis.Notiz) : zeiger = 7

    '        'com.Parameters.AddWithValue(":NOTIZ",  ereignis.Notiz.Substring(0,10)) : zeiger = 7

    '        com.Parameters.AddWithValue(":DOKUMENTID", (ereignis.DokumentID)) : zeiger = 8
    '        com.Parameters.AddWithValue(":PERSONENID", 0) : zeiger = 9
    '        com.Parameters.AddWithValue(":QUELLE", ereignis.Quelle) : zeiger = 10
    '        com.Parameters.AddWithValue(":ISTRTF", CInt(ereignis.istRTF)) : zeiger = 11
    '        l(" Len(.Notiz) " & Len(ereignis.Notiz))
    '        l("zielvorgangsid " & zielvorgangsid)
    '        l("BESCHREIBUNG " & ereignis.Beschreibung)
    '        l("DATUM " & ereignis.Datum)
    '        l("DOKUMENTID " & ereignis.DokumentID)
    '        l("PERSONENID " & 0)
    '        l("CInt(.istRTF) " & CInt(ereignis.istRTF))
    '        l("zielvorgangsid " & zielvorgangsid)
    '        l("zielvorgangsid " & zielvorgangsid)
    '        '    End With


    '        ' com.Parameters.AddWithValue(":ID", eid%) : zeiger = 11
    '        l("ereignis setParams ---------------- ende ----------------- 11? " & zeiger)
    '    Catch ex As Exception
    '        l("fehler inereignis setParams " & "zeiger: " & zeiger & vbCrLf & ex.ToString)
    '    End Try
    'End Sub

    Private Function getInitial(userName As String) As String
        Dim ret As String = ""
        Try
            l(" MOD getInitial anfang")
            ret = userName.Substring(0, 3).ToLower & userName.Substring(userName.Length - 1, 1)
            Return ret
        Catch ex As Exception
            l("Fehler in getInitial: " & ex.ToString())
            Return ""
        End Try
    End Function
End Module
