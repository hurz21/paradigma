'Imports Devart.Data.Oracle
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

Public Class FST_serialShape_Oracle
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



#Region "Serial"


    Shared Function setSQLBodyFLST_serial() As String
        Return " SET " & _
         " RAUMBEZUGSID=:RAUMBEZUGSID" & _
         ",VORGANGSID=:VORGANGSID" & _
         ",TYP=:TYP" & _
         ",AREAQM=:AREAQM" & _
         ",SERIALSHAPE=:SERIALSHAPE"
    End Function

    Shared Sub setSQLParamsFLST_serial(ByVal com As OracleCommand, ByVal vid As Integer, ByVal rbid As Integer, ByVal serial As String, ByVal id As Integer, ByVal Typ As Integer, ByVal areaqm As Double)
        com.Parameters.AddWithValue(":RAUMBEZUGSID", rbid)
        com.Parameters.AddWithValue(":VORGANGSID", vid)
        com.Parameters.AddWithValue(":SERIALSHAPE", serial)
        com.Parameters.AddWithValue(":TYP", Typ)
        com.Parameters.AddWithValue(":AREAQM", areaqm)
        '   com.Parameters.AddWithValue(":ID", id)
    End Sub

    'Sub serialInDbSpeichern(ByVal vid As Integer, ByVal rbid As Integer, ByVal typ As Integer, ByVal serial As String, ByVal Area As Double)
    '    'vid,rbid%,typ%,serial$
    '    nachricht("serialSpeichern: vid%:" & vid & "rbid: " & rbid & "serial: " & serial)
    '    RB_FLST_Serial_abspeichern_Neu(vid, rbid, serial, typ, Area)
    'End Sub

#End Region
End Class
