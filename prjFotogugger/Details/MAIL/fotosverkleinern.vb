Module fotosverkleinern
    Private Sub erstesSemikolonKillen(ByRef neueranhang As String)
        Try
            If neueranhang.StartsWith(";") Then
                neueranhang = neueranhang.Substring(1, neueranhang.Length - 1)
            End If
        Catch ex As Exception
            nachricht("fehler in erstesSemikolonKillen: " ,ex)
        End Try

    End Sub
    Function bildethumbs(anhang As String, thumbW As Int16, thumbH As Int16) As String
        Dim a() As String
        Dim ai As String = ""
        Dim neuername As String = ""
        Dim neueranhang As String = ""
        Dim anzahlVerkleinert As Int16 = 0
        Try
            anhang = anhang.Replace("&", "")
            anhang = anhang.Replace(vbCrLf, "")
            a = anhang.Split(CChar(myGlobalz.anhangtrenner))
            For i = 0 To a.GetUpperBound(0)
                ai = a(i).Replace("&", "").Trim.Replace(vbCrLf, "").Trim
                If ai.ToLower.EndsWith(".jpg") Then
                    neuername = ai.ToLower.Replace(".jpg", "_P" & thumbW & "x" & thumbH & ".jpg")
                    guggaTools.createMiniThumb(ai, neuername, thumbW, thumbH)
                    neueranhang = neueranhang & myGlobalz.anhangtrenner & neuername
                Else
                    neueranhang = neueranhang & myGlobalz.anhangtrenner & ai
                End If
            Next
            erstesSemikolonKillen(neueranhang)
            Return neueranhang
        Catch ex As Exception
            nachricht("fehler in bildethumbs: " ,ex)
            Return ""
        End Try
    End Function

    Function takeSizestring(p1 As String) As String
        Dim a As String()
        Try
            a = p1.Split(":"c)
            Return a(1).Trim
        Catch ex As Exception
            nachricht("fehler in takeSizestring: " & p1)
            Return ""
        End Try
    End Function

    Sub sizeString2wh(sizeString As String, ByRef breite As int16, ByRef hoehe As int16)
        Dim a As String()
        Try
            a = sizeString.Split("x"c)
            breite = CShort((a(0).Trim))
            hoehe = CShort(a(1).Trim)
        Catch ex As Exception
            breite = 1280 : hoehe = 1024
            nachricht("fehler in sizeString2wh: " & sizeString)
        End Try
    End Sub

End Module
