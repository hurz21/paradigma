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




End Class
