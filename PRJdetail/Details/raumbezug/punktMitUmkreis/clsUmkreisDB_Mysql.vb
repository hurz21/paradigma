 Imports MySql
Imports MySql.Data
Public Class clsUmkreisDB_Mysql
        Implements IDisposable
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub



    Shared Function setsqlbodyUmkreisRB() As String
        Return " set " & _
         " radiusM=@radiusM" & _
         ",Beschreibung=@Beschreibung"
    End Function

    Shared Sub setSQLparamsUmkreisRB(ByVal com As MySqlCommand, ByVal sekid%)
        '	com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.myconn)
        com.Parameters.AddWithValue("@radiusM", myGlobalz.sitzung.aktPMU.Radius)
        com.Parameters.AddWithValue("@Beschreibung", myGlobalz.sitzung.aktPMU.Name)
        com.Parameters.AddWithValue("@ID", sekid)
    End Sub



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
