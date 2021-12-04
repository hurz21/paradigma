''Imports Devart.Data.Oracle
'Imports LibDB
'Imports System.Data

'Public Class clsWiedervorlageDB_CRUD_ORACLE
'    Implements IWiedervorlageCRUD
'    Implements IDisposable
'#Region "IDisposable Support"
'    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
'    Protected Overridable Sub Dispose(disposing As Boolean)
'        If Not Me.disposedValue Then
'            If disposing Then
'                MeineDBConnection.Dispose()
'            End If
'        End If
'        Me.disposedValue = True
'    End Sub
'    Public Sub Dispose() Implements IDisposable.Dispose
'        Dispose(True)
'        GC.SuppressFinalize(Me)
'    End Sub
'#End Region
'    Public MeineDBConnection As New OracleConnection
'    Sub New(ByVal conn As System.Data.Common.DbConnection)
'        MeineDBConnection = CType(conn, OracleConnection)
'    End Sub
'    Private Shared Sub setParams(ByRef com As OracleCommand, ByVal wvid%) '	 myGlobalz.sitzung.aktWiedervorlage
'        com.Parameters.AddWithValue(":VORGANGSID", myglobalz.sitzung.aktVorgangsID)
'        com.Parameters.AddWithValue(":TODO", myglobalz.sitzung.aktWiedervorlage.ToDo)
'        com.Parameters.AddWithValue(":BEMERKUNG", myglobalz.sitzung.aktWiedervorlage.Bemerkung)
'        com.Parameters.AddWithValue(":WARTENAUF", myglobalz.sitzung.aktWiedervorlage.WartenAuf)
'        com.Parameters.AddWithValue(":BEARBEITER", myglobalz.sitzung.aktWiedervorlage.Bearbeiter)
'        com.Parameters.AddWithValue(":DATUM", myglobalz.sitzung.aktWiedervorlage.datum)
'        com.Parameters.AddWithValue(":ERLEDIGTAM", myglobalz.sitzung.aktWiedervorlage.erledigtAm)
'        com.Parameters.AddWithValue(":ERLEDIGT", Math.Abs(CInt(myglobalz.sitzung.aktWiedervorlage.Erledigt)))




'        'com.Parameters.AddWithValue(":ERLEDIGTAM", Format(myGlobalz.sitzung.aktWiedervorlage.erledigtAm, "yyyy-MM-dd HH:mm:ss"))
'        'com.Parameters.AddWithValue(":DATUM", Format(myGlobalz.sitzung.aktWiedervorlage.datum, "yyyy-MM-dd HH:mm:ss"))
'        'com.Parameters.AddWithValue(":ID", wvid%)
'        'Convert.ToBoolean(stamm.status.erledigt))
'    End Sub

'    Public Function edit_speichern_Wiedervorlage(ByVal wvid As Integer) As Integer 'myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID
'        Dim anzahlTreffer& = 0, hinweis$ = ""
'        Dim com As OracleCommand
'        'Der bearbeiter wurde aus dem update rausgenommen weil  sonst 
'        '    die reine wv-liste nicht mehr stimmt
'        If wvid < 1 Then
'            nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
'            Return -1
'        End If
'        Try
'            myglobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle = myglobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle
'            If myglobalz.sitzung.aktWiedervorlage.anychange Then
'                myglobalz.sitzung.DBWiedervorlageREC.mydb.SQL =
'                 String.Format("update {0}{1} where ID=:ID", myglobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle, setSQLbody())

'                MeineDBConnection.Open()
'                com = New OracleCommand(myglobalz.sitzung.DBWiedervorlageREC.mydb.SQL, MeineDBConnection)
'                com.Parameters.AddWithValue(":ID", wvid%)
'                setParams(com, wvid%)
'                anzahlTreffer& = CInt(com.ExecuteNonQuery)
'                MeineDBConnection.Close()
'                Return CInt(anzahlTreffer)
'            Else
'                Return -2 'keine änderungen vorhanden
'            End If
'            If anzahlTreffer < 1 Then
'                nachricht_und_Mbox("Problem beim Abspeichern:" & myglobalz.sitzung.DBWiedervorlageREC.mydb.SQL)
'                Return -3
'            Else
'                Return -4
'            End If
'        Catch ex As Exception
'            nachricht_und_Mbox("Fehler beim Abspeichern: " ,ex)
'            Return -5
'        End Try
'    End Function

'    Private Function setSQLbody() As String
'        Return " set " &
'         " VORGANGSID=:VORGANGSID" &
'         ",TODO=:TODO" &
'          ",BEARBEITER=:BEARBEITER" &
'         ",BEMERKUNG=:BEMERKUNG" &
'         ",WARTENAUF=:WARTENAUF" &
'         ",DATUM=:DATUM" &
'         ",ERLEDIGTAM=:ERLEDIGTAM" &
'         ",ERLEDIGT=:ERLEDIGT"
'    End Function

'    'Public Function Neu_speichern_Wiedervorlage() As Integer
'    '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'    '    Dim com As OracleCommand
'    '    Try
'    '        Dim sqlupdate$ = "INSERT INTO " & myglobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle.ToUpper & "   " &
'    '             " (VORGANGSID,TODO,BEARBEITER,BEMERKUNG,WARTENAUF,DATUM,ERLEDIGTAM,ERLEDIGT) VALUES " &
'    '             "(:VORGANGSID,:TODO,:BEARBEITER,:BEMERKUNG,:WARTENAUF,:DATUM,:ERLEDIGTAM,:ERLEDIGT) " &
'    '             " RETURNING ID INTO :R1"
'    '        MeineDBConnection.Open()
'    '        nachricht("nach dboeffnen  ")
'    '        com = New OracleCommand(myglobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
'    '        setParams(com, 0)

'    '        newid = clsOracleIns.GetNewid(com, sqlupdate)
'    '        MeineDBConnection.Close()
'    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, sqlupdate)

'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("Problem beim Neu_speichern_Wiedervorlage: " & vbCrLf ,ex)
'    '        Return -1
'    '    End Try
'    'End Function



'    'Public Function create(ByVal wv As clsWiedervorlage) As Integer Implements IWiedervorlageCRUD.create        'myGlobalz.sitzung.aktWiedervorlage
'    '    wv.Erledigt = False
'    '    wv.erledigtAm = CLstart.mycSimple.MeinNULLDatumAlsDate
'    '    If myglobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then myglobalz.sitzung.aktWiedervorlage.Bearbeiter = myglobalz.sitzung.aktBearbeiter.Initiale
'    '    Dim db As New clsWiedervorlageDB_CRUD_ORACLE(MeineDBConnection)
'    '    Dim newid% = db.Neu_speichern_Wiedervorlage()
'    '    db.Dispose()
'    '    myglobalz.sitzung.aktWiedervorlage.WiedervorlageID = CInt(newid)
'    '    Return newid
'    'End Function


'    Public Function update(ByVal wv As clsWiedervorlage) As Integer Implements IWiedervorlageCRUD.update
'        If myglobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then myglobalz.sitzung.aktWiedervorlage.Bearbeiter = myglobalz.sitzung.aktBearbeiter.Initiale

'        Dim anzahl% = edit_speichern_Wiedervorlage(wv.WiedervorlageID)
'        Return anzahl%
'    End Function



'    Public Function getWV(ByVal id As Integer) As clsWiedervorlage Implements IWiedervorlageCRUD.getWV
'        Return Nothing
'    End Function

'    Public Function getWVs(ByVal SQL As String) As System.Data.DataTable Implements IWiedervorlageCRUD.getWVs
'        Return Nothing
'    End Function
'End Class
