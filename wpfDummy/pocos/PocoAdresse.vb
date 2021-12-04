Public Class PocoAdresse
    Public gemparms As New clsGemarkungsParams
    Property geom As String
    Property Quelle As String 'halo oder lage oder fehlt
    Public Property strassennameNORM As String
    Public Property raum As String
    Public Property erster_buchstabe As String
    Public Property gemeindeName As String
    Public Property gemeindeLiegtIMKreis As Boolean
    Public Property gemeindeNr As Integer
    Public Property strasseName As String
    Public Property strasseCode As Integer
    Public Property hausNr As Integer
    Public Property hausZusatz As String
    Public Property HausKombi As String
    Public Property GKrechts As Integer
    Public Property GKhoch As Integer
    Public Shadows Function toString(ByVal delim$) As String
        Try
            Dim a$, wert$
            Dim summe$ = ""
            For Each pi As System.Reflection.PropertyInfo In Me.GetType().GetProperties()
                a$ = pi.Name
                wert$ = "=" & pi.GetValue(Me, Nothing).ToString
                summe &= a$ & wert$ & vbCrLf & delim$
            Next
            Return summe
        Catch ex As Exception
            Return "ERROR"
        End Try
    End Function
    Public Property gemeindebigNRstring As String
    Public Function gemeindeNrBig() As String
        Dim tbignr = "4380"
        Dim s$ = gemeindeNr.ToString
        Try
            If s$.StartsWith("438") Then Return s$ 'ist schon big
            If Val(s$) > 9 Then
                tbignr = "4380" & Val(s$).ToString
            Else
                tbignr = "43800" & Val(s$).ToString
            End If
            Return tbignr
        Catch ex As Exception
            Return "ERROR"
        End Try
    End Function
    Public Sub hauskombiZerlegen()
        Dim HK$ = _HausKombi.Trim
        Dim a$()
        Try
            If HK.Contains("-") Then
                a = HK.Split("-"c)
                If IsNumeric(a(0)) Then
                    _hausNr = CInt(a(0))
                    _hausZusatz = a(1)
                    Exit Sub
                End If
            End If

            If HK.Contains(" ") Then
                a = HK.Split(" "c)
                If IsNumeric(a(0)) Then
                    _hausNr = CInt(a(0))
                    _hausZusatz = a(1)
                    Exit Sub
                End If
            End If

            For i = 1 To Len(HK)
                If Not IsNumeric(Mid$(HK, i, 1)) Then
                    _hausNr = CInt(Mid$(HK, 1, i - 1))
                    _hausZusatz = Mid$(HK, i, HK.Length - i + 1)
                    Exit Sub
                End If
            Next

        Catch ex As Exception
            Dim FehlerHinweis$ = "Error / Fehler: " & vbCrLf +
             ex.Message + " " & vbCrLf +
             ex.StackTrace + " " & vbCrLf +
             ex.Source + " "
        End Try
    End Sub
    Sub clear()
        gemeindeName = ""
        gemeindeNr = 0
        strasseName = ""
        HausKombi = ""
        gemeindeNr = 0
        strasseCode = 0
        hausNr = 0
        hausZusatz = ""

    End Sub
End Class
