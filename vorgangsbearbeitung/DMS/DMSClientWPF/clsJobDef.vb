Public Class clsDMSJobDef
    Property aktion As String
    Property fehlercode As Integer
    Property username As String
    Property inputfiles As String()
    Property attributliste As String()
    Property vid As String
    Property dateiname As String
    Property relativdir As String
    Property numericSubDir As String

    Function buildHttpString(DMSserverUrl As String) As String
        Dim url As String
        url = DMSserverUrl
        url = url & username
        url = url & "&vid=" & vid
        url = url & "&aktion=" & aktion
        url = url & "&relativdir=" & relativdir
        url = url & "&numericSubDir=" & numericSubDir
        Return url
    End Function
    Function FileAbstract(trenner As String) As String
        Dim fa As New Text.StringBuilder
        Dim fii As IO.FileInfo
        Try
            For i = 0 To inputfiles.GetUpperBound(0)
                fii = New IO.FileInfo(inputfiles(i))
                fa.Append(fii.Name & trenner)
            Next
            Return fa.ToString
        Catch ex As Exception
            Return "Fehler beim zusammenstellen der Dateien"
        End Try
    End Function
End Class
