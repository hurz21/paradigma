

Public Class mailMensch
    Public Property name As String
    Public Property email As String
    Public Property shortemail As String
    Public Property organization As String
    Public Property literalname As String
    Public Property telefon As String = ""
    'Public Sub New()
    '    name = ""
    '    email = ""
    '    shortemail = ""
    '    organization = ""
    '    literalname = ""
    '    telefon = ""
    'End Sub
End Class
Public Class mailmenschTools
    Public Shared Sub initTelefontest(ByRef telefonTest As String())
        ReDim telefonTest(11)
        telefonTest(0) = "telefon:"
        telefonTest(1) = "tel:"
        telefonTest(2) = "fon:"
        telefonTest(3) = "telefon "
        telefonTest(4) = "tel "
        telefonTest(5) = "fon "
        telefonTest(6) = "phone:"
        telefonTest(7) = "phone "
        telefonTest(8) = "hotline"
        telefonTest(9) = "telefon"
        telefonTest(10) = "handy"
        telefonTest(11) = "tel."
    End Sub
    Public Shared Function grabTelefonString(mailBodyastext As String) As String
        Dim telefonTest() As String = Nothing
        Dim ret As String = ""
        initTelefontest(telefonTest)
        Dim zeilen As String()
        Dim cand As String
        Try
            If String.IsNullOrEmpty(mailBodyastext) Then
                Return ""
            End If
            zeilen = mailBodyastext.Split(CChar(Environment.NewLine))
            For i = 0 To zeilen.Count - 1
                cand = zeilen(i).ToLower.Trim
                If IsNumeric(cand) Then
                    ret = ret & " oder " & cand
                Else
                    For j = 0 To telefonTest.Length - 1
                        If cand.Contains(telefonTest(CInt(j))) Then
                            ret = ret & " oder " & cand
                        End If
                    Next
                End If
            Next
            ret = ret.Trim
            If ret.StartsWith("oder ") Then ret = ret.Replace("oder ", "")
            Return ret
        Catch ex As system.Exception
            nachricht("Fehler in grabTelefonString:" ,ex)
            Return ""
        End Try
    End Function
    Shared Sub nachricht(t As String)
        '  My.Application.Log.WriteEntry(t)
    End Sub
    Public Shared Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = text & ToLogString(ex, text)
        'myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        'mitFehlerMail(text, anhang)
    End Sub
End Class
