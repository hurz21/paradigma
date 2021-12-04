Public Class clsWindokueincheckenTOOL
    Public Shared Sub QuellverzeichnisseEinlesen(ByRef _dasdir$)
        nachricht("QuellverzeichnisseEinlesen -------------------------------")
        Try
            Dim test2 As String = CLstart.myc.userIniProfile.WertLesen("Dokumente", "windowkuEinchecken_quelldir")
            If String.IsNullOrEmpty(test2) Then
                Exit Sub
            End If

            Dim testdir = New IO.DirectoryInfo(test2)
            If Not testdir.Exists Then Exit Sub
            _dasdir = test2
            testdir = Nothing
            nachricht("QuellverzeichnisseEinlesen emde: " & _dasdir)
        Catch ex As Exception
            nachricht("Fehler in QuellverzeichnisseEinlesen emde: " & _dasdir)
        End Try
    End Sub

    Shared Sub quellverzeichnisspeichern(ByVal dasdir$)
        Try
            nachricht("quellverzeichnisspeichern -------------------------------------")
            nachricht("quellverzeichnisspeichern -- schreiben")
            If Not String.IsNullOrEmpty(dasdir) Then CLstart.myc.userIniProfile.WertSchreiben("Dokumente", "windowkuEinchecken_quelldir", dasdir)
            nachricht("quellverzeichnisspeichern ende")
        Catch ex As Exception
            nachricht(" Fehler in quellverzeichnisspeichern -- schreiben" & ex.ToString)
        End Try
    End Sub

    Shared Function verzeichnis_isolieren(ByVal filenamen$()) As String
        If filenamen Is Nothing Then Return ""
        If filenamen.Count > 0 Then
            Dim test As New IO.FileInfo(filenamen(0))
            Return test.DirectoryName
        Else
            Return ""
        End If
    End Function
End Class
