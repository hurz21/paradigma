Module mod5332Obj
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
        kartei.stamm.az.sachgebiet.Zahl = "5332"
        kartei.stamm.az.sachgebiet.Header = kartei.stamm.az.sachgebiet.Zahl & "-Stellungnahme zu Bauanträgen"
        kartei.stamm.az.sachgebiet.isImmischionschutz = True
        kartei.stamm.az.sachgebiet.isUNB = False
        kartei.stamm.az.Vorgangsnummer = 0
        kartei.stamm.az.Prosa = ""
    End Sub
    Public Sub makeBearbeiter()
        If p(15).ToLower = "pl" Then
            kartei.stamm.Bearbeiter = "plöb"
        Else
            kartei.stamm.Bearbeiter = "klib"
        End If
    End Sub
    Public Sub makeEingang()
        Try
            kartei.stamm.Eingangsdatum = Nothing
            If String.IsNullOrEmpty(p(14)) Then Exit Sub
            If p(14) = "" Then
                Exit Sub
            End If
            kartei.stamm.Eingangsdatum = CDate(p(14))
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
        kartei.stamm.Beschreibung = p(9) & " (AltAz: " & p(0) & "-" & p(1) & ")" &
            " (Bauh.:" & p(10) & "," & p(11) & "," & p(12) & "," & p(13) & ")"
        reduce(540, kartei.stamm.Beschreibung)
    End Sub

    Public Sub makebemerkung()
        kartei.stamm.Bemerkung = " (Bauort:" & p(4) & "," & p(5) & "," & p(6) & ", Fl." & p(7) & ", Fst." & p(8) & ")"
        reduce(540, kartei.stamm.Bemerkung)
    End Sub

    Public Sub makeLetztebearbeitung()
        kartei.stamm.LetzteBearbeitung = Nothing
        If String.IsNullOrEmpty(p(19)) Then Exit Sub
        Try
            Dim test As Date = CDate(p(19))
            kartei.stamm.LetzteBearbeitung = test
        Catch ex As Exception
            'zurück
            kartei.stamm.LetzteBearbeitung = Nothing
        End Try
    End Sub

    Public Sub makeOrtsterminBool()
        If String.IsNullOrEmpty(p(18)) Then
            kartei.stamm.Ortstermin = False
        Else
            kartei.stamm.Ortstermin = True
        End If

    End Sub

    Public Sub makeOrtsterminDate()

        If String.IsNullOrEmpty(p(18)) Then
            'stamm.LastActionHeroe = False
        Else
            kartei.stamm.LastActionHeroe = "Ortstermin: " & p(18)
        End If
    End Sub

    Public Sub makeProbaugAZ()
        If Not String.IsNullOrEmpty(p(2)) Then
            kartei.stamm.Probaugaz = p(2)
        End If
    End Sub

    Public Sub makeAltAZ()
        If Not String.IsNullOrEmpty(p(0)) Then
            kartei.stamm.AltAz = p(0) & "-" & p(1)
        End If
    End Sub



    Private Sub makegemkrzWerteholen(ByRef gemarkung As String, ByRef gemeinde As String)
        If Not String.IsNullOrEmpty(p(6)) Then
            gemarkung = p(5)
        End If
        If Not String.IsNullOrEmpty(p(5)) Then
            gemeinde = p(4)
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
