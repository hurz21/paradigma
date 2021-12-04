Namespace CLstart
    Public Class clsMsgboxText
        Private Shared Function GetDateiname(ByVal pfad As String, ByVal textid As String) As String
            Try
                Return IO.Path.Combine(pfad, textid & ".txt")
            Catch ex As Exception
                Return "Fehler1 clsMsgboxText, combine: " & IO.Path.Combine(pfad, textid & ".txt")
            End Try
        End Function
        Private Shared Function GetTest(ByVal dateiname As String) As String
            Dim test As String = ""
            Try
                If IO.File.Exists(dateiname) Then
                    test = My.Computer.FileSystem.ReadAllText(dateiname, mycSimple.enc)
                End If
                Return test
            Catch ex As Exception
                Return "Fehler2 clsMsgboxText, einlesen, " & test
            End Try
        End Function
        Shared Function getMsgboxText(pfad As String, textid As String, meineListe As List(Of String)) As String
            If pfad Is Nothing OrElse pfad = "" Then Return "Fehler: Kein pfad " & pfad
            If textid Is Nothing OrElse textid = "" Then Return "Fehler: Keine Textid " & textid
            Dim dateiname As String = GetDateiname(pfad, textid)
            If dateiname.ToLower.StartsWith("fehler") Then
                Return dateiname
            End If
            Dim rohText As String
            rohText = GetTest(dateiname)
            rohText = tauscheVariablen(rohText, meineListe)
            Return rohText
        End Function

        Private Shared Function tauscheVariablen(rohText As String, meineListe As List(Of String)) As String
            Try
                Dim izaehl As Integer = 1
                For Each mtext As String In meineListe
                    If String.IsNullOrEmpty(mtext) Then
                        Return rohText
                    Else
                        rohText = rohText.Replace("_#" & izaehl & "#_", mtext)
                        izaehl += 1
                    End If
                Next
                'If String.IsNullOrEmpty(p1) Then Return rohText 
                Return rohText
            Catch ex As Exception
                Return "fehler in tauscheVariablen: " '& rohText
            End Try
        End Function

    End Class
End Namespace