Imports System.Data
Imports System.Data.SqlClient
Imports koloReport

Public Class clsSQLS
    'Public Property eigentschnellKonn As New Devart.Common.DbConnectionStringBuilder
    Public Property EigentuemerSchnellDB As SqlConnection = New SqlConnection

    Sub oeffneConnectionSQLS()
        Try
            EigentuemerSchnellDB = New SqlConnection("Server=msql01;Database=Paradigma;User=sgis;Pwd=WinterErschranzt.74;")
            EigentuemerSchnellDB.Open()
        Catch ex As Exception
            l("Fehler in oeffneConnectionEigentuemer: " & ex.ToString)
        End Try
    End Sub


    Function getDt(query As String) As DataTable
        Dim com As SqlCommand
        Dim _mycount As Long
        Try
            l("in paradigma sqls getDt ")
            Dim dt As DataTable

            com = New SqlCommand(query, EigentuemerSchnellDB)
            Dim da As New SqlDataAdapter(com)
            'da.MissingSchemaAction = MissingSchemaAction.AddWithKey
            dt = New DataTable
            _mycount = da.Fill(dt)
            If _mycount < 1 Then
                l("kein treffer")
                Return dt
            End If
            l("in paradigma sqls getDt fertig")
            Return dt
        Catch ex As Exception
            l("fehler in in paradigma sqls getDt:" & ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Sub schliesseConnectionSQLS()
        EigentuemerSchnellDB.Close()
    End Sub

    Friend Shared Function getDTSQLS(queryString As String, ByRef dtRBplus As DataTable) As Integer
        l("getDTSQLS-------------------------")
        Dim eigSDB As New clsSQLS
        'Dim dt As DataTable
        Try
            eigSDB.oeffneConnectionSQLS()
            dtRBplus = eigSDB.getDt(queryString)
            eigSDB.schliesseConnectionSQLS()
            Return dtRBplus.Rows.Count
        Catch ex As Exception
            l("fehler in getDTSQLS: " & ex.ToString)
            Return Nothing
        End Try
    End Function

    Friend Shared Function getDTSQLS(queryString As String) As DataTable
        l("getDTSQLS-------------------------")
        Dim eigSDB As New clsSQLS
        Dim dt As New DataTable
        Try
            eigSDB.oeffneConnectionSQLS()
            dt = eigSDB.getDt(queryString)
            eigSDB.schliesseConnectionSQLS()
            Return dt
        Catch ex As Exception
            l("fehler in getDTSQLS: " & ex.ToString)
            Return Nothing
        End Try
    End Function

    Friend Shared Function Neu_speichern_EreignisSQLS(zielvorgangsid As Integer, modus As String, Ereignis As clsEreignis) As Integer
        l("Neu_speichern_Ereignis -----------------------------------------------------")
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As SqlCommand
        Dim SQLupdate$ = ""
        Dim msqls As New clsSQLS
        '  Dim mysqls As New SqlConnection
        Try
            '  myglobalz.sitzung.VorgangREC.mydb.Tabelle = 
            If String.IsNullOrEmpty(modus) Then
                l("Fehler: ereingismodus ist nicht nothing")
                Return -3
            End If
            If modus.ToLower = "neu" Then
                SQLupdate$ = "INSERT INTO " & tabEreignis & "(VORGANGSID,BESCHREIBUNG,DATUM,ART,RICHTUNG,NOTIZ,DOKUMENTID,PERSONENID,QUELLE,ISTRTF,TYPNR) " +
                                      " VALUES (@VORGANGSID,@BESCHREIBUNG,@DATUM,@ART,@RICHTUNG,@NOTIZ,@DOKUMENTID,@PERSONENID,@QUELLE,@ISTRTF,@TYPNR)"

                SQLupdate$ = SQLupdate$ & ";SELECT CAST(scope_identity() AS int);"
                'SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"
                modOracle.avoidEreignisNulls(Ereignis)
                l("nach setSQLbody : " & SQLupdate)
                '   mysqls.oeffneConnectionSQLS()
                'makeConnectionOracle(myoracle)
                msqls.oeffneConnectionSQLS
                l("nach dboeffnen  ")

                com = New SqlCommand(SQLupdate$, msqls.EigentuemerSchnellDB)
                l("vor setParams  ")
                setParamEreignisSQLS(com, 0, zielvorgangsid, Ereignis)
                Dim kobjssss = com.ExecuteScalar()
                If kobjssss Is Nothing Then
                    newid = 0
                Else
                    newid = CLng(kobjssss.ToString)
                End If
                'mysqls.Close()
                msqls.EigentuemerSchnellDB.Close()
            End If
            If newid < 1 Then
                l("Problem beim Abspeichern des Ereignisses: " & SQLupdate)
                Return -1
            Else
                Ereignis.ID = CInt(newid)
                l("Neu_speichern_Ereignis funzt")
                Ereignis.ID = CInt(newid)
                Return CInt(newid)
            End If
        Catch ex As Exception
            l("Fehler beim Abspeichern: " & ex.ToString)
            Ereignis.ID = 0I
            Return -2
        End Try
    End Function

    Private Shared Sub setParamEreignisSQLS(com As SqlCommand, v As Integer, zielvorgangsid As Integer, ereignis As clsEreignis)
        Dim zeiger As Integer
        Try
            '    With ereignis 'myGlobalz.sitzung.aktEreignis
            com.Parameters.AddWithValue("@VORGANGSID", zielvorgangsid) : zeiger = 2
            com.Parameters.AddWithValue("@BESCHREIBUNG", ereignis.Beschreibung) : zeiger = 3
            com.Parameters.AddWithValue("@DATUM", CDate(ereignis.Datum)) : zeiger = 4
            com.Parameters.AddWithValue("@ART", ereignis.Art) : zeiger = 5
            com.Parameters.AddWithValue("@RICHTUNG", ereignis.Richtung) : zeiger = 6
            com.Parameters.AddWithValue("@NOTIZ", ereignis.Notiz) : zeiger = 7
            com.Parameters.AddWithValue("@TYPNR", ereignis.typnr) : zeiger = 7

            'com.Parameters.AddWithValue("@NOTIZ",  ereignis.Notiz.Substring(0,10)) : zeiger = 7

            com.Parameters.AddWithValue("@DOKUMENTID", (ereignis.DokumentID)) : zeiger = 8
            com.Parameters.AddWithValue("@PERSONENID", 0) : zeiger = 9
            com.Parameters.AddWithValue("@QUELLE", ereignis.Quelle) : zeiger = 10
            com.Parameters.AddWithValue("@ISTRTF", CInt(ereignis.istRTF)) : zeiger = 11
            l(" Len(.Notiz) " & Len(ereignis.Notiz))
            l("zielvorgangsid " & zielvorgangsid)
            l("BESCHREIBUNG " & ereignis.Beschreibung)
            l("DATUM " & ereignis.Datum)
            l("DOKUMENTID " & ereignis.DokumentID)
            l("PERSONENID " & 0)
            l("CInt(.istRTF) " & CInt(ereignis.istRTF))
            l("zielvorgangsid " & zielvorgangsid)
            l("zielvorgangsid " & zielvorgangsid)
            '    End With


            ' com.Parameters.AddWithValue("@ID", eid%) : zeiger = 11
            l("ereignis setParams ---------------- ende ----------------- 11? " & zeiger)
        Catch ex As Exception
            l("fehler inereignis setParams " & "zeiger: " & zeiger & vbCrLf & ex.ToString)
        End Try
    End Sub

    Friend Shared Function checkInDBDokuSQLS(aktdoku As Dokument) As Integer
        l("in checkInDBDoku -------------------------------------")
        'Dim host, datenbank, schema, tabelle, dbuser, dbpw, dbport As String
        Dim dt As New DataTable
        Dim msqls As New clsSQLS
        Dim myoracle As SqlConnection
        Dim newid& = -1
        Dim Sql As String =
         "INSERT INTO dokumente (RELATIVPFAD,DATEINAMEEXT,TYP,BESCHREIBUNG,CHECKINDATUM,FILEDATUM,EXIFDATUM,EXIFLONG,EXIFLAT,EXIFDIR," +
                            "EXIFHERSTELLER,ORIGINALFULLNAME,INITIAL_,REVISIONSSICHER,ORIGINALNAME,VID,EID,NEWSAVEMODE,BEARBEITERID) " +
                 " VALUES (@RELATIVPFAD,@DATEINAMEEXT,@TYP,@BESCHREIBUNG,@CHECKINDATUM,@FILEDATUM,@EXIFDATUM,@EXIFLONG,@EXIFLAT,@EXIFDIR," +
                           "@EXIFHERSTELLER,@ORIGINALFULLNAME,@INITIAL_,@REVISIONSSICHER,@ORIGINALNAME,@VID,@EID,@NEWSAVEMODE,@BEARBEITERID)"

        Sql = Sql & ";SELECT CAST(scope_identity() AS int);"
        l("nach setSQLbody : " & Sql)
        msqls.oeffneConnectionSQLS()
        Dim com As New SqlCommand(Sql, msqls.EigentuemerSchnellDB)
        Dim da As New SqlDataAdapter(com)
        'da.MissingSchemaAction = MissingSchemaAction.AddWithKey
        Dim erfolg As Boolean
        erfolg = setSQLParamsDokSQLS(aktdoku, com)
        Dim kobjssss = com.ExecuteScalar()
        If kobjssss Is Nothing Then
            newid = 0
        Else
            newid = CLng(kobjssss.ToString)
        End If
        msqls.schliesseConnectionSQLS()
        'newid = clsOracleIns.GetNewid(com, Sql)
        'myoracle.Close()
        'Return clsOracleIns.gebeNeuIDoderFehler(newid, Sql)
        Return CInt(newid)
        Try
            com.Dispose()
            da.Dispose()
            'msqls.Close()
            'myoracle.Dispose()
            ' Return dt
        Catch oex As sqlException
            l("Fehler in GetNewid&:" & oex.ToString & " / " & Sql)
            Return Nothing
        Catch ex As Exception
            l("Fehler in checkInDBDoku&:" & ex.ToString & " / " & Sql)
            Return Nothing
        Finally
            myoracle.Close()
        End Try
    End Function

    Private Shared Function setSQLParamsDokSQLS(aktdoku As Dokument, com As SqlCommand) As Boolean
        Try
            com.Parameters.AddWithValue("@RELATIVPFAD", aktdoku.dokumentPfad.Replace("\", "/"))
            com.Parameters.AddWithValue("@DATEINAMEEXT", aktdoku.DateinameMitExtension)
            com.Parameters.AddWithValue("@TYP", aktdoku.Typ)
            com.Parameters.AddWithValue("@BESCHREIBUNG", aktdoku.Beschreibung)
            com.Parameters.AddWithValue("@CHECKINDATUM", DateTime.Now())
            com.Parameters.AddWithValue("@FILEDATUM", DateTime.Now)
            com.Parameters.AddWithValue("@EXIFDATUM", DateTime.Now)
            com.Parameters.AddWithValue("@EXIFLONG", "")
            com.Parameters.AddWithValue("@EXIFLAT", "")
            com.Parameters.AddWithValue("@EXIFDIR", "")
            com.Parameters.AddWithValue("@EXIFHERSTELLER", "")
            com.Parameters.AddWithValue("@ORIGINALFULLNAME", aktdoku.OriginalFullname)
            com.Parameters.AddWithValue("@INITIAL_", aktdoku.Initiale)
            com.Parameters.AddWithValue("@REVISIONSSICHER", aktdoku.revisionssicher)
            com.Parameters.AddWithValue("@NEWSAVEMODE", CInt(aktdoku.newSaveMode))
            com.Parameters.AddWithValue("@ORIGINALNAME", aktdoku.OriginalName)
            com.Parameters.AddWithValue("@VID", aktdoku.VorgangsID)
            com.Parameters.AddWithValue("@EID", aktdoku.EreignisID)
            com.Parameters.AddWithValue("@BEARBEITERID", aktdoku.bearbeiterid)
            Return True
        Catch ex As Exception
            l("fehler in setSQLParams" & ex.ToString)
            Return False
        End Try
    End Function
End Class


