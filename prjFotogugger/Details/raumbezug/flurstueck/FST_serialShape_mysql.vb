Imports MySql.Data.MySqlClient

Public Class FST_serialShape_mysql
    
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



#Region "Serial"

    Shared Function setSQLBodyFLST_serial() As String
        Return " set " & _
         " RaumbezugsID=@RaumbezugsID" & _
         ",VorgangsID=@VorgangsID" & _
          ",Typ=@Typ" & _
           ",AreaQm=@AreaQm" & _
         ",SerialShape=@SerialShape"
    End Function

    Shared Sub setSQLParamsFLST_serial(ByVal com As MySqlCommand, ByVal vid%, ByVal rbid%, ByVal serial$, ByVal id%, ByVal Typ%, ByVal areaqm As Double)
        com.Parameters.AddWithValue("@RaumbezugsID", rbid%)
        com.Parameters.AddWithValue("@VorgangsID", vid%)
        com.Parameters.AddWithValue("@SerialShape", serial$)
        com.Parameters.AddWithValue("@Typ", Typ)
        com.Parameters.AddWithValue("@AreaQm", areaqm)
        com.Parameters.AddWithValue("@ID", id)
    End Sub


#End Region
End Class
