Module FileArchivTools
    Public Sub inputFileReadonlyEntfernen(inputfile As String)
        Try
            Dim fi As New IO.FileInfo(inputfile)
            If fi.Exists Then
                If CBool(fi.Attributes And IO.FileAttributes.ReadOnly) Then
                    ' Datei ist schreibgeschützt
                    ' Jetzt Schreibschutz-Attribut entfernen
                    fi.IsReadOnly = False
                    'fi.Attributes = fi.Attributes Xor IO.FileAttributes.ReadOnly
                End If
            End If
            fi = Nothing
        Catch ex As Exception
            'Debug.Print(ex.ToString)
            nachricht("fehler in inputFileReadonlyEntfernen: " & inputfile, ex)
        End Try
    End Sub
    Public Sub inputFileReadonlysetzen(inputfile As String)
        Try
            Dim fi As New IO.FileInfo(inputfile)
            If fi.Exists Then
                If CBool(fi.Attributes And Not IO.FileAttributes.ReadOnly) Then
                    ' Datei ist nicht schreibgeschützt
                    ' Jetzt Schreibschutz-Attribut setzen
                    fi.IsReadOnly = True
                    ' fi.Attributes = fi.Attributes Or IO.FileAttributes.ReadOnly

                End If
            End If
            fi = Nothing
        Catch ex As Exception
            nachricht("inputFileReadonlysetzen " & inputfile & " / " ,ex)
        End Try
    End Sub
    Function pruefeBeschreibung(beschreibung As String) As String
        Dim neue As String
        Try
            If String.IsNullOrEmpty(beschreibung) Then Return ""
            neue = beschreibung
            neue = LIBgemeinsames.clsString.noWhiteSpace(neue, " ")
            Return neue
        Catch ex As Exception
            nachricht("fehler in pruefeBeschreibung: " ,ex)
            Return ""
        End Try
    End Function

End Module
