Public Class clsTools
    Public Shared webserver As String = "http://w2gis02.kreis-of.local"
    Public Shared Property iminternet As Boolean = True

    Friend Shared Function getIminternet() As Boolean
        Try
            Dim datei = "\\w2gis02\gdvell\inetpub\wwwroot\index.htm"
            Dim fi As New IO.FileInfo(datei)
            If fi.Exists Then
                Return False
            End If
            Return True
        Catch ex As Exception
            Return True
        End Try
    End Function
End Class
