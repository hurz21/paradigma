
Namespace StammToolsNs

    Class test
           Private Sub new
        End Sub
        Shared Function getSGtextZuSgNr(ByVal sachgebietsnummer As String,
                                        ByVal xmlfile As String) As String
            Dim sgtext As String = ""
            sgtext = clsBaumbilden.sucheSGNRInXML(xmlfile, sachgebietsnummer, sgtext)
            Return sgtext
        End Function

    End Class
End Namespace
