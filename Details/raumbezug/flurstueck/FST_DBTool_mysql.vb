Imports MySql.Data.MySqlClient
Public Class FST_DBTool_mysql
    
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
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub
    Shared Function setSQLBodyFLST() As String
        Return " set " & _
         " gemcode=@gemcode" & _
         ",flur=@flur" & _
         ",zaehler=@zaehler" & _
         ",nenner=@nenner" & _
         ",ZNkombi=@ZNkombi" & _
         ",Gemarkungstext=@Gemarkungstext" & _
         ",FS=@FS"
    End Function
    Shared Sub setSQLParamsFLST(ByVal com As MySqlCommand, ByVal sekid%)
        com.Parameters.AddWithValue("@gemcode", myGlobalz.sitzung.aktFST.normflst.gemcode)
        com.Parameters.AddWithValue("@flur", myGlobalz.sitzung.aktFST.normflst.flur)
        com.Parameters.AddWithValue("@zaehler", myGlobalz.sitzung.aktFST.normflst.zaehler)
        com.Parameters.AddWithValue("@nenner", myGlobalz.sitzung.aktFST.normflst.nenner)
        com.Parameters.AddWithValue("@ZNkombi", myGlobalz.sitzung.aktFST.normflst.fstueckKombi)
        com.Parameters.AddWithValue("@Gemarkungstext", myGlobalz.sitzung.aktFST.normflst.gemarkungstext)
        com.Parameters.AddWithValue("@FS", myGlobalz.sitzung.aktFST.normflst.FS)
        com.Parameters.AddWithValue("@ID", sekid)
    End Sub

    Public Function RB_FLST_abspeichern_Edit(ByVal sekid as integer) as  Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand
        Try
            If sekid < 1 Then
                nachricht_und_Mbox("fehler updateid<1) RB_FLST_abspeichern_Edit")
                Return 0
            End If
            glob2.initTemprecAusVorgangRecMysql()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaFlurstueck"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "update " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             setSQLBodyFLST() & _
             " where id=@ID"
            myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
            com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.tempREC.myconn)
            setSQLParamsFLST(com, sekid)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)
            '	anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function RB_FLST_abspeichern_Neu() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand
        Try
            glob2.initTemprecAusVorgangRecMysql()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaFlurstueck"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "insert into " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             setSQLBodyFLST()
            myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
            com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
            setSQLParamsFLST(com, 0)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            com.CommandText = "Select LAST_INSERT_ID()"
            newid = CLng(com.ExecuteScalar)
            myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)

            'anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Shared Function RB_Flurstueck_loeschen(ByVal flurstuecksid as integer) as  Integer
        Dim anzahlTreffer&, newid&
        Try
            glob2.initTemprecAusVorgangRecMysql()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaFlurstueck"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             " where id=" & flurstuecksid%
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
            Return -2
        End Try
    End Function

End Class
