Public Class clsKoordumrechnung
    Shared Sub nachricht(ByVal text$)
        My.Application.Log.WriteEntry(text$)
    End Sub
    Shared Sub nachricht_und_Mbox(ByVal text$)
        My.Application.Log.WriteEntry(text)
        ' MsgBox(text)
    End Sub
    Public Shared Function NormiereWGSstring(ByVal wgsstring$) As String
        Try
            nachricht("NormiereWGSstring----------------------------------")
            Dim ricoh$() = wgsstring$.Split("#"c) 'aktJPG.Exifgpslongitude.Split("#"c)
            Dim grad$ = ricoh(0)
            Dim minute$, sekunde$
            If Val(ricoh(1)) < 10 Then
                minute$ = "0" & ricoh(1)
            Else
                minute$ = ricoh(1)
            End If
            If CDbl(ricoh(2)) < 10 Then
                sekunde$ = "0" & ricoh(2)
            Else
                sekunde$ = ricoh(2)
            End If
            nachricht(String.Format("NormiereWGSstring----------------------------------ende {0}{1}{2}", grad$, minute, sekunde))
            Return grad$ & minute & sekunde
        Catch ex As Exception
            nachricht(String.Format("FEHLER: NormiereWGSstring:{0}{1}", vbCrLf, ex.Message))
            Return ex.Message
        End Try
    End Function


    'Public Shared Function berechneGaussKrueger_ausGoogleGeografisch(ByRef wgstext As clsGEOPoint, ByVal quellenotation%) As Boolean 'quellenotation=1 bei fotos , 6 bei googlecoordinaten
    '    Try
    '        nachricht("berechneGaussKrueger_ausGoogleGeografisch------------------------------------------")

    '        'Dim laenge, breite', rechts, hoch As Double
    '        Dim ergebnis$
    '        ergebnis = class_kootrans.freischalten()
    '        'If aktbiom.debuck Then mylog.log("freischaltung: " & ergebnis)
    '        If ergebnis = "nicht freigeschaltet" Then
    '            nachricht("berechneGaussKrueger_ausGoogleGeografisch------------------------------------------ende false freischaltung")
    '            Return False
    '        Else
    '            wgstext.X = CDbl(wgstext.laenge_string.Replace(".", ","))
    '            wgstext.Y = CDbl(wgstext.breite_string.Replace(".", ","))
    '            ergebnis = class_kootrans.geografischZuGausskrueger3Grad(wgstext.X, wgstext.Y, 3, wgstext.GKrechts, wgstext.GKhoch, quellenotation)
    '            nachricht("berechneGaussKrueger_ausGoogleGeografisch------------------------------------------ende true")
    '            Return True
    '        End If
    '        nachricht("berechneGaussKrueger_ausGoogleGeografisch------------------------------------------ ende")
    '    Catch ex As Exception
    '        nachricht("Fehler in berechneGaussKrueger_ausGoogleGeografisch: " & ex.Message)
    '        Return False
    '    End Try
    'End Function



    'Public Shared Function berechneUTM32_ausgk2(ByRef wgstext As clsGEOPoint) As Boolean 'quellenotation=1 bei fotos , 6 bei googlecoordinaten
    '    Try
    '        nachricht("berechneUTM32_ausgk2  ---------------------------------")
    '        Dim kootrans = New class_kootrans()
    '        'Dim laenge, breite', rechts, hoch As Double
    '        Dim ergebnis$
    '        ergebnis = class_kootrans.freischalten()
    '        nachricht("berechneUTM32_ausgk2 freischaltung")
    '        nachricht(ergebnis.ToString)
    '        'If aktbiom.debuck Then mylog.log("freischaltung: " & ergebnis)
    '        If ergebnis = "nicht freigeschaltet" Then
    '            nachricht("berechneUTM32_ausgk2  Return False")
    '            Return False
    '        Else
    '            wgstext.X = CDbl(wgstext.laenge_string.Replace(".", ","))
    '            wgstext.Y = CDbl(wgstext.breite_string.Replace(".", ","))
    '            kootrans.gk2UTM(wgstext.X, wgstext.Y, wgstext.GKrechts, wgstext.GKhoch)
    '            ' ergebnis = kootrans.geografischZuGausskrueger3Grad(wgstext.X, wgstext.Y, 3, wgstext.GKrechts, wgstext.GKhoch, quellenotation)
    '            nachricht(String.Format("{0},{1}", wgstext.GKrechts, wgstext.GKhoch))
    '            nachricht("berechneUTM32_ausgk2  Return true")
    '            Return True
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in berechneUTM32_ausgk2: " & ex.Message)
    '        Return False
    '    End Try
    'End Function

    Public Shared Function berechneUTM32_ausGoogleGeografisch(ByRef wgstext As clsGEOPoint, ByVal quellenotation%) As Boolean 'quellenotation=1 bei fotos , 6 bei googlecoordinaten
        Try
            nachricht("berechneUTM32 ---------------------------------")
            Dim kootrans = New class_kootrans()
            'Dim laenge, breite', rechts, hoch As Double
            Dim ergebnis$
            ergebnis = class_kootrans.freischalten()
            nachricht("berechneUTM32 freischaltung")
            nachricht(ergebnis.ToString)
            'If aktbiom.debuck Then mylog.log("freischaltung: " & ergebnis)
            If ergebnis = "nicht freigeschaltet" Then
                Return False
            Else
                wgstext.X = CDbl(wgstext.laenge_string.Replace(".", ","))
                wgstext.Y = CDbl(wgstext.breite_string.Replace(".", ","))
                kootrans.geografischgoogle2UTM(wgstext.X, wgstext.Y, wgstext.GKrechts, wgstext.GKhoch, quellenotation)
                If wgstext.GKrechts > 32000000 Then
                    wgstext.GKrechts = Math.Abs(32000000 - CInt(wgstext.GKrechts))
                End If
                ' ergebnis = kootrans.geografischZuGausskrueger3Grad(wgstext.X, wgstext.Y, 3, wgstext.GKrechts, wgstext.GKhoch, quellenotation)
                nachricht(String.Format("{0},{1}", wgstext.GKrechts, wgstext.GKhoch))
                nachricht("berechneUTM32_ausGoogleGeografisch  Return true")
                Return True
            End If
        Catch ex As Exception
            nachricht("Fehler in berechneUTM32: " & ex.Message)
            Return False
        End Try
    End Function

    Public Shared Function berechneGoogleGeografisch_aus_UTM32(ByRef wgstext As clsGEOPoint, ByVal quellenotation As Integer) As Boolean 'quellenotation=1 bei fotos , 6 bei googlecoordinaten
        Try
            nachricht("berechneUTM32 ---------------------------------")
            Dim kootrans = New class_kootrans()
            'Dim laenge, breite', rechts, hoch As Double
            Dim ergebnis$
            ergebnis = class_kootrans.freischalten()
            nachricht("berechneUTM32 freischaltung")
            nachricht(ergebnis.ToString)
            'If aktbiom.debuck Then mylog.log("freischaltung: " & ergebnis)
            If ergebnis = "nicht freigeschaltet" Then
                Return False
            Else
                'wgstext.X = CDbl(wgstext.gkrechts.Replace(".", ","))
                'wgstext.Y = CDbl(wgstext.gkhoch.Replace(".", ","))
                ' kootrans.geografischgoogle2UTM(wgstext.X, wgstext.Y, wgstext.GKrechts, wgstext.GKhoch, quellenotation)
                kootrans.UTM2geografischgoogle(wgstext.GKrechts, wgstext.GKhoch, wgstext.X, wgstext.Y, quellenotation)
                'If wgstext.GKrechts > 32000000 Then
                '    wgstext.GKrechts = Math.Abs(32000000 - CInt(wgstext.GKrechts))
                'End If
                ' ergebnis = kootrans.geografischZuGausskrueger3Grad(wgstext.X, wgstext.Y, 3, wgstext.GKrechts, wgstext.GKhoch, quellenotation)
                nachricht(String.Format("{0},{1}", wgstext.X, wgstext.Y))
                nachricht("berechneUTM32_ausGoogleGeografisch  Return true")
                Return True
            End If
        Catch ex As Exception
            nachricht("Fehler in berechneUTM32: " & ex.Message)
            Return False
        End Try
    End Function

    Public Shared Function umrechneninUTM32_ausWGS84(ByVal Exifgpslongitude$, ByVal Exifgpslatitude$, ByRef rechts As String, ByRef hoch As String, ByVal quellnotation%) As Boolean 'akJPG.Exifgpslongitude,akJPG.Exifgpslatitude 'akJPG As clsMyJPG
        Try
            nachricht("umrechneninGK_ausWGS84---------------------------------------")
            Dim wgs84 As New clsGEOPoint() With {.laenge_string = NormiereWGSstring(Exifgpslongitude),
                                                 .breite_string = NormiereWGSstring(Exifgpslatitude)}
            If berechneUTM32_ausGoogleGeografisch(wgs84, quellnotation) Then

                'End If
                'If berechneGaussKrueger_ausGoogleGeografisch(wgs84, 1) Then
                rechts = CType(wgs84.GKrechts, Integer).ToString
                hoch = CType(wgs84.GKhoch, Integer).ToString
                nachricht("umrechneninGK_ausWGS84 ende true")
                Return True
            Else
                nachricht("umrechneninGK_ausWGS84 ende false")
                Return False
            End If
            nachricht("umrechneninGK_ausWGS84---------------------------------------")
        Catch ex As Exception
            nachricht("in umrechneninGK_ausWGS84: " & ex.ToString)
            Return False
        End Try
    End Function
End Class
