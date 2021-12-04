Class clsDMSErgebnis
    Property status As String
    Property modus As String
    Property numberOfFiles As Integer 'anzahl der betroffenen dateien
    Property inhalt As String
    Property errortext As String
    Sub getergebnis(antwortstring As String)
        Dim a As String()
        Try
            a = antwortstring.Split("#"c)
            status = a(0).Trim.ToLower
            modus = a(1).Trim.ToLower
            numberOfFiles = CInt(a(2).Trim.ToLower)
            inhalt = a(3).Trim
            errortext = a(4).Trim.ToLower
        Catch ex As Exception
            l("fehler in getergebnis: " & ex.ToString)
        End Try
    End Sub
    Overrides Function tostring() As String
        Return "Status: " & status & ", " & "Inhalt: " & inhalt & ", " &
            "Fehlertext:" & errortext & ", Modus: " & modus &
            ", Anzahl der Dateien: " & numberOfFiles
    End Function
End Class
