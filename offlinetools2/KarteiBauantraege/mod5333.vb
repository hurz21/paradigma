Module mod5333
    Public Sub stammobjekterzeugen()
        kartei.stamm.clear()
        kartei.stamm.Aufnahmedatum = Now
        makeBearbeiter()
        makeEingang()
        makebeschreibung()
        makebemerkung()
        kartei.stamm.erledigt = True
        makeLetztebearbeitung()
        makeOrtsterminBool()
        makeOrtsterminDate()
        makegemkrz()
        kartei.stamm.Stellungnahme = True
        makeProbaugAZ()
        kartei.stamm.Aufnahmedatum = Now
        makeAltAZ()

        kartei.stamm.WeitereBearbeiter = ""

        kartei.stamm.darfNichtVernichtetWerden = True
        kartei.stamm.Standort.RaumNr = "U.2.02"
        kartei.stamm.Standort.Titel = "Keller"
        kartei.stamm.meinGutachten.existiert = False
        kartei.stamm.meinGutachten.UnterDokumente = False
        kartei.stamm.AbgabeBA = False
        kartei.stamm.Paragraf = ""

        kartei.stamm.InterneNr = ""
        kartei.stamm.az.sachgebiet.Zahl = "5333"
        kartei.stamm.az.sachgebiet.Header = kartei.stamm.az.sachgebiet.Zahl & "-Gaststättenkonzessionen"
        kartei.stamm.az.sachgebiet.isImmischionschutz = True
        kartei.stamm.az.sachgebiet.isUNB = False
        kartei.stamm.az.Vorgangsnummer = 0
        kartei.stamm.az.Prosa = ""
    End Sub
    Public Sub makeBearbeiter()
        If p(11).ToLower.StartsWith("pl") Then
            kartei.stamm.Bearbeiter = "plöb"
        Else
            kartei.stamm.Bearbeiter = "klib"
        End If
    End Sub
    Public Sub makeEingang()
        Try
            kartei.stamm.Eingangsdatum = Nothing
            If String.IsNullOrEmpty(p(10)) Then Exit Sub
            If p(10) = "" Then
                Exit Sub
            End If
            kartei.stamm.Eingangsdatum = CDate(p(10))
        Catch ex As Exception
            kartei.stamm.Eingangsdatum = Nothing
        End Try
    End Sub

    Public Sub reduce(ByVal laenge As Int16, ByVal wort As String)
        If String.IsNullOrEmpty(wort) Then
            Exit Sub
        End If
        If wort.Length > 540 Then
            wort = wort.Substring(0, 540)
        End If
    End Sub

    Public Sub makebeschreibung()
        'kartei.stamm.Beschreibung = p(9) & " (Bauh.:" & p(10) & "," & p(11) & "," & p(12) & "," & p(13) & ")"
        ' kartei.stamm.Beschreibung = p(9) & " (AltAz: " & p(0) & "-" & p(1) & ")"
        kartei.stamm.Beschreibung = p(2) & ", " & p(3) & "-" & p(4) &
            "," & p(5)
        reduce(540, kartei.stamm.Beschreibung)
    End Sub

    Private Function GetNachforderung() As String
        If String.IsNullOrEmpty(p(12).Trim) Then
            Return ""
        Else
            Return "Nachforderung: " & p(12) & "," & Environment.NewLine
        End If

    End Function
    Private Function GetBeanstandung() As String
        If String.IsNullOrEmpty(p(13).Trim) Then
            Return ""
        Else
            Return "Beanstandung: " & p(13) & "," & Environment.NewLine
        End If
    End Function
    Private Function GetBemerkung() As String
        If String.IsNullOrEmpty(p(14).Trim) Then
            Return ""
        Else
            Return "Frist: " & p(14) & "," & Environment.NewLine
        End If
    End Function
    Private Function GetOrtsbesichtigung() As String
        If String.IsNullOrEmpty(p(15).Trim) Then
            Return ""
        Else
            Return "Ortsbesichtigung: " & p(15) & "," & Environment.NewLine
        End If
    End Function
    Private Function Getueberpruefung() As String
        If String.IsNullOrEmpty(p(17).Trim) Then
            Return ""
        Else
            Return "Überprüfung: " & p(17) & "," & Environment.NewLine
        End If
    End Function
    Private Function GetBeanstandung2() As String
        If String.IsNullOrEmpty(p(18).Trim) Then
            Return ""
        Else
            Return "Beanstandung nachher: " & p(18) & "," & Environment.NewLine
        End If
    End Function
    Private Function GetFachfirma() As String
        If String.IsNullOrEmpty(p(19).Trim) Then
            Return ""
        Else
            Return "Fachfirma: " & p(19) & "," & Environment.NewLine
        End If
    End Function
    Private Function GetBemerkung1() As String
        If String.IsNullOrEmpty(p(20).Trim) Then
            Return ""
        Else
            Return "Akte vernichtet: " & p(20) & "" & Environment.NewLine
        End If
    End Function
    Private Function vernichtet() As String
        If String.IsNullOrEmpty(p(16).Trim) Then
            Return ""
        Else
            Return "Erledigt: " & p(16) & "" & Environment.NewLine
        End If
    End Function
    Public Sub makebemerkung()
        kartei.stamm.Bemerkung = "Antragsteller: " & p(6) & ", " & p(7) & ", " & p(8) & ", " & p(9) & " " & Environment.NewLine &
                                GetNachforderung() &
                                GetBeanstandung() &
                                GetBemerkung() &
                                GetOrtsbesichtigung() &
                                Getueberpruefung() &
                                GetBeanstandung2() &
                                GetFachfirma() &
                                vernichtet() &
                                GetBemerkung1()
        reduce(540, kartei.stamm.Bemerkung)
    End Sub

    Public Sub makeLetztebearbeitung()
        kartei.stamm.LetzteBearbeitung = Nothing
        If String.IsNullOrEmpty(p(16)) Then Exit Sub
        Try
            Dim test As Date = CDate(p(16))
            kartei.stamm.LetzteBearbeitung = test
        Catch ex As Exception
            'zurück
            kartei.stamm.LetzteBearbeitung = Nothing
        End Try
    End Sub

    Public Sub makeOrtsterminBool()
        If String.IsNullOrEmpty(p(15)) Then
            kartei.stamm.Ortstermin = False
        Else
            kartei.stamm.Ortstermin = True
        End If

    End Sub

    Public Sub makeOrtsterminDate()

        If String.IsNullOrEmpty(p(15)) Then
            'stamm.LastActionHeroe = False
        Else
            kartei.stamm.LastActionHeroe = "Ortstermin: " & p(15)
        End If
    End Sub

    Public Sub makeProbaugAZ()
        kartei.stamm.Probaugaz = ""
    End Sub

    Public Sub makeAltAZ()
        If Not String.IsNullOrEmpty(p(0)) Then
            kartei.stamm.AltAz = p(0) & "-" & p(1)
        End If
    End Sub



    Private Sub makegemkrzWerteholen(ByRef gemarkung As String, ByRef gemeinde As String)
        gemarkung = ""
        If Not String.IsNullOrEmpty(p(3)) Then
            gemeinde = p(3)
        End If
    End Sub
    Public Sub makegemkrz()
        Dim gemarkung As String = ""
        Dim gemeinde As String = ""
        Dim result As String
        Try
            makegemkrzWerteholen(gemarkung, gemeinde)
            'umsetzen
            result = clsGEMKRZXML.LoadVariablenGemarkung(gemarkung)
            If result.IsNothingOrEmpty Then
                result = clsGEMKRZXML.LoadVariablenGemeinde(gemeinde)
                If result.IsNothingOrEmpty Then
                    kartei.stamm.GemKRZ = ""
                Else
                    kartei.stamm.GemKRZ = result
                End If
            Else
                kartei.stamm.GemKRZ = result
            End If
        Catch ex As Exception
            kartei.stamm.GemKRZ = ""
        End Try
    End Sub
End Module
