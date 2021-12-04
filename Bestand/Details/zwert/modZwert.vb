
Imports System.Net
Imports System.IO
Imports System.Text

Module modZwert
    Public PaareTrenner As Char = CChar("_")
    Public xyTrenner As Char = CChar(";")

 

    Function bildeaufruf(newpoint As myPoint, p2 As String) As String
        Dim a As String
        Try
            a = "http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/zwertServer/zwertServer.cgi?username=" &
                            myGlobalz.sitzung.aktBearbeiter.username &
                            "&rechts=" & Math.Round(newpoint.X) &
                            "&hoch=" & Math.Round(newpoint.Y) &
                            "&mitdom=1" &
                            "&mitdgm=1"

            Return a
        Catch ex As Exception
            nachricht("fehler in bildeaufruf: ",ex)
            Return ""
        End Try
    End Function
    
End Module


