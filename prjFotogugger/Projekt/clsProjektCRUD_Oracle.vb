'Imports Devart.Data.Oracle
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data
Public Class clsProjektCRUD_Oracle
      Implements IDisposable
    Public aktProjekt As clstart.clsProjektAllgemein
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection, ByVal _ap As clstart.clsProjektAllgemein)
        MeineDBConnection = CType(conn, OracleConnection)
        aktProjekt = _ap
    End Sub
    Shared Function setSQLbody() As String
        Return " SET KATEGORIE1=:KATEGORIE1" & _
         ",KATEGORIE2=:KATEGORIE2" & _
         ",BEZEICHNUNGKURZ=:BEZEICHNUNGKURZ " & _
         ",BEZEICHNUNGLAN=:BEZEICHNUNGLAN " & _
         ",VONDATUM=:VONDATUM " & _
         ",BISDATUM=:BISDATUM " & _
         ",QUELLE=:QUELLE " & _
         ",WIEDERVORLAGEID=:WIEDERVORLAGEID " & _
         ",REFNR=:REFNR " & _
         ",GEMEINDE=:GEMEINDE "
    End Function

    Sub defaultwertesetzen()
        If String.IsNullOrEmpty(aktProjekt.Gemeinde) Then aktProjekt.Gemeinde = ""
        If String.IsNullOrEmpty(aktProjekt.Kategorie1) Then aktProjekt.Kategorie1 = ""
        If String.IsNullOrEmpty(aktProjekt.Kategorie2) Then aktProjekt.Kategorie2 = ""
        If String.IsNullOrEmpty(aktProjekt.BezeichnungKurz) Then aktProjekt.BezeichnungKurz = ""
        If String.IsNullOrEmpty(aktProjekt.BezeichnungLang) Then aktProjekt.BezeichnungLang = ""
        If String.IsNullOrEmpty(aktProjekt.Quelle) Then aktProjekt.Quelle = ""
        If String.IsNullOrEmpty(aktProjekt.refnr) Then aktProjekt.refnr = ""
    End Sub
    Sub setSQLParams(ByRef com As OracleCommand)
        Try
            defaultwertesetzen()

            com.Parameters.AddWithValue(":KATEGORIE1", aktProjekt.Kategorie1)
            com.Parameters.AddWithValue(":KATEGORIE2", aktProjekt.Kategorie2)
            com.Parameters.AddWithValue(":BEZEICHNUNGKURZ", aktProjekt.BezeichnungKurz)
            com.Parameters.AddWithValue(":BEZEICHNUNGLAN", aktProjekt.BezeichnungLang)
            com.Parameters.AddWithValue(":VONDATUM", aktProjekt.von)
            com.Parameters.AddWithValue(":BISDATUM", aktProjekt.bis)
            com.Parameters.AddWithValue(":QUELLE", aktProjekt.Quelle)
            com.Parameters.AddWithValue(":WIEDERVORLAGEID", aktProjekt.WiedervorlageID) 
            com.Parameters.AddWithValue(":GEMEINDE", aktProjekt.Gemeinde)   
            com.Parameters.AddWithValue(":REFNR", aktProjekt.REFNR)
        Catch ex As Exception
            nachricht("Fehler in setSQLParams beteiligte: " ,ex)
        End Try
    End Sub














    'Private Shared Sub bildeHinweis(ByRef hinweis$)
    '    For Each ritem As DataRow In myGlobalz.sitzung.VorgangREC.dt.Rows
    '        hinweis &= ritem.Item(0).ToString & ", "
    '    Next
    'End Sub



#Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose
                aktProjekt = nothing
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


