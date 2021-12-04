'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data

Public Class clsWiedervorlageDB_CRUD_ORACLE
    Implements IWiedervorlageCRUD
      Implements IDisposable
   #Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, OracleConnection)
    End Sub
    Private Shared Sub setParams(ByRef com As OracleCommand, ByVal wvid%) '	 myGlobalz.sitzung.aktWiedervorlage
        com.Parameters.AddWithValue(":VORGANGSID", myGlobalz.sitzung.aktVorgangsID)
        com.Parameters.AddWithValue(":TODO", myGlobalz.sitzung.aktWiedervorlage.ToDo)
        com.Parameters.AddWithValue(":BEMERKUNG", myGlobalz.sitzung.aktWiedervorlage.Bemerkung)
        com.Parameters.AddWithValue(":WARTENAUF", myGlobalz.sitzung.aktWiedervorlage.WartenAuf)
        com.Parameters.AddWithValue(":BEARBEITER", myGlobalz.sitzung.aktWiedervorlage.Bearbeiter)
        com.Parameters.AddWithValue(":DATUM", myGlobalz.sitzung.aktWiedervorlage.datum)
        com.Parameters.AddWithValue(":ERLEDIGTAM", myGlobalz.sitzung.aktWiedervorlage.erledigtAm)
        com.Parameters.AddWithValue(":ERLEDIGT", Math.Abs(CInt(myGlobalz.sitzung.aktWiedervorlage.Erledigt)))




        'com.Parameters.AddWithValue(":ERLEDIGTAM", Format(myGlobalz.sitzung.aktWiedervorlage.erledigtAm, "yyyy-MM-dd HH:mm:ss"))
        'com.Parameters.AddWithValue(":DATUM", Format(myGlobalz.sitzung.aktWiedervorlage.datum, "yyyy-MM-dd HH:mm:ss"))
        'com.Parameters.AddWithValue(":ID", wvid%)
        'Convert.ToBoolean(stamm.status.erledigt))
    End Sub

    Public Function edit_speichern_Wiedervorlage(ByVal wvid as integer) as  Integer 'myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        'Der bearbeiter wurde aus dem update rausgenommen weil  sonst 
        '    die reine wv-liste nicht mehr stimmt
        If wvid < 1 Then
            nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
            Return -1
        End If
        Try
            myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle = myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle
            If myGlobalz.sitzung.aktWiedervorlage.anychange Then
                myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL = _
                 String.Format("update {0}{1} where ID=:ID", myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle, setSQLbody())

                MeineDBConnection.Open()
                com = New OracleCommand(myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL, MeineDBConnection)
                com.Parameters.AddWithValue(":ID", wvid%)
                setParams(com, wvid%)
                anzahlTreffer& = CInt(com.ExecuteNonQuery)
                MeineDBConnection.Close()
                Return CInt(anzahlTreffer)
            Else
                Return -2 'keine änderungen vorhanden
            End If
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL)
                Return -3
            Else
                Return -4
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -5
        End Try
    End Function

    Private Function setSQLbody() As String
        Return " set " & _
         " VORGANGSID=:VORGANGSID" & _
         ",TODO=:TODO" & _
          ",BEARBEITER=:BEARBEITER" & _
         ",BEMERKUNG=:BEMERKUNG" & _
         ",WARTENAUF=:WARTENAUF" & _
         ",DATUM=:DATUM" & _
         ",ERLEDIGTAM=:ERLEDIGTAM" & _
         ",ERLEDIGT=:ERLEDIGT"
    End Function

    Public Function Neu_speichern_Wiedervorlage() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            Dim sqlupdate$ = "INSERT INTO " & myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle.ToUpper & "   " &
                 " (VORGANGSID,TODO,BEARBEITER,BEMERKUNG,WARTENAUF,DATUM,ERLEDIGTAM,ERLEDIGT) VALUES " &
                 "(:VORGANGSID,:TODO,:BEARBEITER,:BEMERKUNG,:WARTENAUF,:DATUM,:ERLEDIGTAM,:ERLEDIGT) " &
                 " RETURNING ID INTO :R1" 
            MeineDBConnection.Open()
            nachricht("nach dboeffnen  ")
            com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
            setParams(com, 0)

            newid = LIBoracle.clsOracleIns.GetNewid(com, sqlupdate)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, sqlupdate)

        Catch ex As Exception
            nachricht_und_Mbox("Problem beim Neu_speichern_Wiedervorlage: " & vbCrLf & ex.ToString)
            Return -1
        End Try
    End Function

    Public Function Wiedervorlage_loeschen(ByVal wiedervorlageid as integer) as  Integer
        'myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID
        Dim anzahlTreffer&, newid&
        Try
            myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL = _
             "delete from " & myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle & _
             " where id=" & wiedervorlageid%.ToString
            anzahlTreffer = myGlobalz.sitzung.DBWiedervorlageREC.sqlexecute(newid) ', myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Wiedervorlage:" & myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL)
                Return -1
            Else
                nachricht("Wiedervorlage wurde gelöscht! id: " & wiedervorlageid%)
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Wiedervorlage löschen: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function create(ByVal wv As clsWiedervorlage) As Integer Implements IWiedervorlageCRUD.create        'myGlobalz.sitzung.aktWiedervorlage
        wv.Erledigt = False
        wv.erledigtAm = CLstart.mycsimple.MeinNULLDatumAlsDate
        If myGlobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then myGlobalz.sitzung.aktWiedervorlage.Bearbeiter = myGlobalz.sitzung.aktBearbeiter.Initiale
        Dim db As New clsWiedervorlageDB_CRUD_ORACLE(MeineDBConnection)
        Dim newid% = db.Neu_speichern_Wiedervorlage()
        db.Dispose
        myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID = CInt(newid)
        Return newid
    End Function


    Public Function update(ByVal wv As clsWiedervorlage) As Integer Implements IWiedervorlageCRUD.update
        If myGlobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then myGlobalz.sitzung.aktWiedervorlage.Bearbeiter = myGlobalz.sitzung.aktBearbeiter.Initiale

        Dim anzahl% = edit_speichern_Wiedervorlage(wv.WiedervorlageID)
        Return anzahl%
    End Function

    Public Function delete(ByVal wv As clsWiedervorlage) As Integer Implements IWiedervorlageCRUD.delete
        Dim newid% = Wiedervorlage_loeschen(wv.WiedervorlageID)
    End Function

    Public Function getWV(ByVal id As Integer) As clsWiedervorlage Implements IWiedervorlageCRUD.getWV
        Return Nothing
    End Function

    Public Function getWVs(ByVal SQL As String) As System.Data.DataTable Implements IWiedervorlageCRUD.getWVs
        Return Nothing
    End Function
End Class
