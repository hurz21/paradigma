Imports MySql.Data.MySqlClient

Public Class clsEreignisDB_Mysql
      Implements IDisposable
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub

    Shared Function setSQLbody$()
        Return " set " &
         " VorgangsID=@VorgangsID" &
         ",Beschreibung=@Beschreibung" &
         ",Datum=@Datum " &
         ",Art=@Art " &
         ",Richtung=@Richtung " &
         ",Notiz=@Notiz " &
         ",DokumentID=@DokumentID " &
         ",pERSONENID=@pERSONENID " &
         ",QUELLE=@QUELLE " &
         ",TYPNR=@TYPNR "
    End Function

    Private Shared Sub setParams(ByVal com As MySqlCommand, ByVal eid%, ByVal zielvorgangsid As Integer)
        nachricht("ereignis setParams ---------------------------------")
        nachricht("ereignis setParams :")
        nachricht("ereignis setParams :")
        nachricht("ereignis setParams :")
        If myGlobalz.sitzung Is Nothing Then nachricht("1")
        If myGlobalz.sitzung.aktEreignis Is Nothing Then nachricht("2")
        If myGlobalz.sitzung.aktBearbeiter Is Nothing Then nachricht("3")
        Dim zeiger = 1
        Try
            com.Parameters.AddWithValue("@VorgangsID", zielvorgangsid) : zeiger = 2
            com.Parameters.AddWithValue("@Beschreibung", myGlobalz.sitzung.aktEreignis.Beschreibung) : zeiger = 3
            com.Parameters.AddWithValue("@Datum", Convert.ToDateTime(Format(myGlobalz.sitzung.aktEreignis.Datum, "yyyy-MM-dd HH:mm:ss"))) : zeiger = 4
            com.Parameters.AddWithValue("@Art", myGlobalz.sitzung.aktEreignis.Art) : zeiger = 5
            com.Parameters.AddWithValue("@Richtung", myGlobalz.sitzung.aktEreignis.Richtung) : zeiger = 6
            com.Parameters.AddWithValue("@Notiz", myGlobalz.sitzung.aktEreignis.Notiz) : zeiger = 7
            com.Parameters.AddWithValue("@DokumentID", (myGlobalz.sitzung.aktEreignis.DokumentID)) : zeiger = 8
            com.Parameters.AddWithValue("@pERSONENID", myGlobalz.sitzung.aktBearbeiter.PersonenID) : zeiger = 9
            com.Parameters.AddWithValue("@QUELLE", myGlobalz.sitzung.aktBearbeiter.Initiale) : zeiger = 10
            com.Parameters.AddWithValue("@TYPNR", myGlobalz.sitzung.aktEreignis.typnr) : zeiger = 11

            com.Parameters.AddWithValue("@ID", eid%) : zeiger = 12
            nachricht("ereignis setParams ---------------- ende ----------------- 11? " & zeiger)
        Catch ex As Exception
            nachricht("fehler inereignis setParams " & "zeiger: " & zeiger & vbCrLf ,ex)
        End Try
    End Sub

    Public Function Edit_speichern_Ereignis(ByVal eid As Integer, ByVal zielvorgangsid As Integer) As Boolean  'myGlobalz.sitzung.aktEreignis.ID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As MySqlCommand
        Dim SQLupdate$ = ""
        Try
            'myGlobalz.sitzung.EreignisseRec.mydb.Tabelle ="ereignis"
            If eid < 1 Then
                nachricht_und_Mbox("FEHLER: Edit_speichern_Ereignis updateid =0. Abbruch. in Edit_speichern_Ereignis2")
                Return False
            End If

            If myGlobalz.sitzung.aktEreignis.anychange Then
                SQLupdate$ = "update " & CLstart.myViewsNTabs.tabEreignis & " " & setSQLbody() & " where id=@id"
                myGlobalz.sitzung.EreignisseRec.dboeffnen(hinweis$)
                com = New MySqlCommand(SQLupdate$, MeineDBConnection) 'myGlobalz.sitzung.EreignisseRec.myconn)
                setParams(com, eid, zielvorgangsid)

                anzahlTreffer& = CInt(com.ExecuteNonQuery)
                myGlobalz.sitzung.EreignisseRec.dbschliessen(hinweis$)
            Else
                Return True 'keine änderungen vorhanden     
            End If
            If anzahlTreffer < 1 Then
                nachricht("Problem beim Abspeichern:" & myGlobalz.sitzung.EreignisseRec.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht("Fehler beim Abspeichern: " ,ex)
            Return False
        End Try
    End Function


    Public Function Neu_speichern_Ereignis(ByVal zielvorgangsid As Integer) As Integer
        nachricht("Neu_speichern_Ereignis -----------------------------------------------------")
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand
        Dim SQLupdate$ = ""
        Try
            'myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="ereignis"
            If myGlobalz.sitzung.Ereignismodus = "neu" Then
                SQLupdate$ = "insert into " & CLstart.myViewsNTabs.tabEreignis & "  " & setSQLbody()
                nachricht("nach setSQLbody : " & SQLupdate)
                myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
                nachricht("nach dboeffnen  ")
                com = New MySqlCommand(SQLupdate$, MeineDBConnection)
                nachricht("vor setParams  ")
                setParams(com, 0, zielvorgangsid)
                nachricht("nach setParams  ")
                anzahlTreffer& = CInt(com.ExecuteNonQuery)
                com.CommandText = "Select LAST_INSERT_ID()"
                newid = CLng(com.ExecuteScalar)
                myGlobalz.sitzung.VorgangREC.dbschliessen(hinweis$)
            End If
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
                Return -1
            Else
                myGlobalz.sitzung.aktEreignis.ID = CInt(newid)
                nachricht("Neu_speichern_Ereignis funzt")
                Return -2
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " ,ex)
            myGlobalz.sitzung.aktEreignis.ID = 0I
            Return -3
        End Try
    End Function
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
End Class
