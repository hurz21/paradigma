Imports MySql.Data.MySqlClient

Public Class clsWiedervorlageDB_CRUD_MYSQL
    Implements IWiedervorlageCRUD
    Implements IDisposable
#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose()
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub
    Private Shared Sub setParams(ByVal com As MySqlCommand, ByVal wvid%) '	 myGlobalz.sitzung.aktWiedervorlage
        com.Parameters.AddWithValue("@VorgangsID", myGlobalz.sitzung.aktVorgangsID)
        com.Parameters.AddWithValue("@ToDo", myGlobalz.sitzung.aktWiedervorlage.ToDo)
        com.Parameters.AddWithValue("@Bemerkung", myGlobalz.sitzung.aktWiedervorlage.Bemerkung)
        com.Parameters.AddWithValue("@wartenauf", myGlobalz.sitzung.aktWiedervorlage.WartenAuf)
        'com.Parameters.AddWithValue("@Bearbeiter", myGlobalz.sitzung.Bearbeiter.Initiale)
        com.Parameters.AddWithValue("@Bearbeiter", myGlobalz.sitzung.aktWiedervorlage.Bearbeiter)
        com.Parameters.AddWithValue("@datum", Format(myGlobalz.sitzung.aktWiedervorlage.datum, "yyyy-MM-dd HH:mm:ss"))
        com.Parameters.AddWithValue("@erledigtAm", Format(myGlobalz.sitzung.aktWiedervorlage.erledigtAm, "yyyy-MM-dd HH:mm:ss"))
        com.Parameters.AddWithValue("@erledigt", CBool(myGlobalz.sitzung.aktWiedervorlage.Erledigt))
        com.Parameters.AddWithValue("@ID", wvid%)
    End Sub

    Public Function edit_speichern_Wiedervorlage(ByVal wvid As Integer) As Integer 'myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As MySqlCommand
        'Der bearbeiter wurde aus dem update rausgenommen weil  sonst 
        '    die reine wv-liste nicht mehr stimmt
        If wvid < 1 Then
            nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
            Return -1
        End If
        Try
            myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle = myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle
            If myGlobalz.sitzung.aktWiedervorlage.anychange Then
                myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL =
                 String.Format("update {0}{1} where ID=@ID", myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle, setSQLbody())
                myGlobalz.sitzung.DBWiedervorlageREC.dboeffnen(hinweis$)
                com = New MySqlCommand(myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL, MeineDBConnection)

                setParams(com, wvid%)
                anzahlTreffer& = CInt(com.ExecuteNonQuery)
                myGlobalz.sitzung.DBWiedervorlageREC.dbschliessen(hinweis$)
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
            nachricht_und_Mbox("Fehler beim Abspeichern: ", ex)
            Return -5
        End Try
    End Function

    Private Function setSQLbody() As String
        Return " set " &
         " VorgangsID=@VorgangsID" &
         ",ToDo=@ToDo" &
          ",Bearbeiter=@Bearbeiter" &
         ",Bemerkung=@Bemerkung" &
         ",wartenauf=@wartenauf" &
         ",datum=@datum" &
         ",erledigtAm=@erledigtAm" &
         ",erledigt=@erledigt"
    End Function

    Public Function Neu_speichern_Wiedervorlage() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand

        Try
            myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle = myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle

            myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL =
             String.Format("insert into {0}{1}", myGlobalz.sitzung.DBWiedervorlageREC.mydb.Tabelle, setSQLbody())
            myGlobalz.sitzung.DBWiedervorlageREC.dboeffnen(hinweis$)
            com = New MySqlCommand(myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL, MeineDBConnection)
            setParams(com, 0)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            com.CommandText = "Select LAST_INSERT_ID()"
            newid = CLng(com.ExecuteScalar)
            myGlobalz.sitzung.DBWiedervorlageREC.dbschliessen(hinweis$)
            If anzahlTreffer < 1 Then
                Return CInt(anzahlTreffer)
            Else

                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim Neu_speichern_Wiedervorlage: " & vbCrLf, ex)
            Return -1
        End Try
    End Function



    Public Function create(ByVal wv As clsWiedervorlage) As Integer Implements IWiedervorlageCRUD.create        'myGlobalz.sitzung.aktWiedervorlage
        wv.Erledigt = False
        wv.erledigtAm = CLstart.mycSimple.MeinNULLDatumAlsDate
        If myGlobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then myGlobalz.sitzung.aktWiedervorlage.Bearbeiter = myGlobalz.sitzung.aktBearbeiter.Initiale
        Dim db As New clsWiedervorlageDB_CRUD_MYSQL(MeineDBConnection)
        Dim newid% = db.Neu_speichern_Wiedervorlage()
        db.Dispose()
        myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID = CInt(newid)
        Return newid
    End Function


    Public Function update(ByVal wv As clsWiedervorlage) As Integer Implements IWiedervorlageCRUD.update
        If myGlobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then
            myGlobalz.sitzung.aktWiedervorlage.Bearbeiter = myGlobalz.sitzung.aktBearbeiter.Initiale
        End If

        Dim anzahl As Integer = edit_speichern_Wiedervorlage(wv.WiedervorlageID)
        Return anzahl%
    End Function



    Public Function getWV(ByVal id As Integer) As clsWiedervorlage Implements IWiedervorlageCRUD.getWV
        Return Nothing
    End Function

    Public Function getWVs(ByVal SQL As String) As System.Data.DataTable Implements IWiedervorlageCRUD.getWVs
        Return Nothing
    End Function
End Class
