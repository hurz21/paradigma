Module Module1

    Sub Main()
        Dim verz As String = "O:\UMWELT-PARADIGMA\div\deployxcdetail\bin\debugALT"
        Dim datum As Date
        Dim dateien As String() = IO.Directory.GetFiles(verz)
        datum = New Date(2008, 5, 15, 13, 12, 11)
        datum = New Date(2015, 8, 28, 1, 12, 11)
        For Each datei As String In dateien
            dateidatumAendern(datei, datum)
        Next

    End Sub
    Private Sub dateidatumAendern(ByVal sFile As String, datum As Date)
        ' FileInfo-Objekt erstellen
        With New System.IO.FileInfo(sFile)
            ' Datums- und Zeitangaben auslesen
            Debug.Print("Erstellungsdatum: " & .CreationTime)
            Debug.Print("Letzter Zugriff: " & .LastAccessTime)
            Debug.Print("Letzte Änderung: " & .LastWriteTime)

            ' Erstellungsdatum ändern
            .CreationTime = datum

            ' Datumsangabe "Letzter Zugriff" ändern
            .LastAccessTime = datum

            ' Datum "Letzte Änderung" ändern
            .LastWriteTime = datum
        End With
    End Sub
End Module
