Imports System.Data
Imports Layer2shpFileServer.LIBDB

Public Class myGlobalz
    Public Shared raumbezugsRec As IDB_grundfunktionen
    Public Shared haloREC As IDB_grundfunktionen
    Public Shared webgisREC As IDB_grundfunktionen
    Public Shared tempREC As IDB_grundfunktionen
    Public Shared VorgangREC As IDB_grundfunktionen
    Public Shared RaumbezugsIDsDT As New DataTable
    Public Shared DokumentIDsDT As New DataTable
    Public Shared VerwandteDT As New DataTable
    Public Shared ArcRec As IDB_grundfunktionen
    Public Shared gis_serverD As String = "\\w2gis02\gdvell"
    Public Shared GIS_WebServer As String = "w2gis02.kreis-of.local" '"KIS"
    Public Shared ArcrootDir As String = "O:\UMWELT-PARADIGMA\gis\GIS\div\backup\archiv"
    Public Shared VorgangsID As Integer
    Public Shared Bearbeiter As String = System.Environment.GetEnvironmentVariable("username")
    Public Shared enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
    Public Shared iniDict As New Dictionary(Of String, String)
End Class
