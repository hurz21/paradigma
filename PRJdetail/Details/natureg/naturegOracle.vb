#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

Public Class naturegOracle
    
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

    Private Shared Sub avoidNUlls(ByVal nat As clsNatureg)
        'If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
        'If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
        'If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
        'If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
        'If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.Bearbeiter.Initiale
    End Sub
    Shared Function setSQLbody() As String
        Return " SET VORGANGSID=:VORGANGSID" & _
             ",NUMMER=:NUMMER" & _
             ",ART=:ART " & _
             ",TYP=:TYP " & _
             ",QUELLE=:QUELLE " & _
             ",NOTIZ=:NOTIZ " & _
             ",MASSNAHMENNR=:MASSNAHMENNR " & _
             ",BESCHREIBUNG=:BESCHREIBUNG "
    End Function
    Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal nat As clsNatureg)
        avoidNUlls(nat)
        Try
            With nat
                com.Parameters.AddWithValue(":VORGANGSID", .VorgangsID)
                com.Parameters.AddWithValue(":NUMMER", .nummer)
                com.Parameters.AddWithValue(":ART", .art)
                com.Parameters.AddWithValue(":TYP", .typ)
                com.Parameters.AddWithValue(":BESCHREIBUNG", .beschreibung)
                com.Parameters.AddWithValue(":MASSNAHMENNR", .MassnahmenNr)
                com.Parameters.AddWithValue(":QUELLE", .Quelle.Trim)
                com.Parameters.AddWithValue(":NOTIZ", .notiz.Trim)

            End With
            '  com.Parameters.AddWithValue(":VORGANGSID", vid)
        Catch ex As Exception
            nachricht("Fehler in setSQLParams beteiligte: " ,ex)
        End Try

    End Sub




    Shared Function getNaturegDatatable(vorgangsid As Integer) As DataTable
        Dim hinweis As String
        Try
            'myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabNatureg & ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from  " & CLstart.myViewsNTabs.tabNatureg & "  where vorgangsid=" & vorgangsid
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            Return myGlobalz.sitzung.VorgangREC.dt
        Catch ex As Exception
            nachricht("fehler in getNaturegDatatable : " ,ex)
            Return Nothing
        End Try
    End Function

End Class


