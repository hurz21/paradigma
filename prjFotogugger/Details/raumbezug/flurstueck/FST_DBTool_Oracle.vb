'Imports Devart.Data.Oracle
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

Public Class FST_DBTool_Oracle
    
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
    Shared Function setSQLBodyFLST() As String
        Return " SET " & _
         " GEMCODE=:GEMCODE" & _
         ",FLUR=:FLUR" & _
         ",ZAEHLER=:ZAEHLER" & _
         ",NENNER=:NENNER" & _
         ",ZNKOMBI=:ZNKOMBI" & _
         ",GEMARKUNGSTEXT=:GEMARKUNGSTEXT" & _
         ",FLAECHEQM=:FLAECHEQM" & _
         ",FS=:FS"
    End Function
    Shared Sub SETSQLPARAMSFLST(ByVal COM As OracleCommand)
        COM.Parameters.AddWithValue(":GEMCODE", myGlobalz.sitzung.aktFST.normflst.gemcode)
        COM.Parameters.AddWithValue(":FLUR", myGlobalz.sitzung.aktFST.normflst.flur)
        COM.Parameters.AddWithValue(":ZAEHLER", myGlobalz.sitzung.aktFST.normflst.zaehler)
        COM.Parameters.AddWithValue(":NENNER", myGlobalz.sitzung.aktFST.normflst.nenner)
        COM.Parameters.AddWithValue(":ZNKOMBI", myGlobalz.sitzung.aktFST.normflst.fstueckKombi)
        COM.Parameters.AddWithValue(":GEMARKUNGSTEXT", myGlobalz.sitzung.aktFST.normflst.gemarkungstext)
        COM.Parameters.AddWithValue(":FS", myGlobalz.sitzung.aktFST.normflst.FS)
        COM.Parameters.AddWithValue(":FLAECHEQM", myGlobalz.sitzung.aktFST.normflst.flaecheqm)
        ' com.Parameters.AddWithValue(":ID", sekid)
    End Sub



    Public Shared Function RB_Flurstueck_loeschen(ByVal flurstuecksid As Integer) As Integer
        Try
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & "   where id=" & flurstuecksid%
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
            If hinweis = "" Then
                Return 1
            Else
                Return -1
            End If
        Catch ex As Exception
            nachricht_und_Mbox("warnung beim löschen: " ,ex)
            Return -2
        End Try
    End Function

End Class
