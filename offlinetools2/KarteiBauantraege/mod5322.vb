Module mod5322

    Public Sub stammobjekterzeugen()
        kartei.stamm.clear()
        kartei.stamm.Aufnahmedatum = Now
        makeBearbeiter(12)
        makeEingang(11)
        makebeschreibung()
        makebemerkung()
        kartei.stamm.erledigt = True
        makeLetztebearbeitung(18)
        makeOrtsterminBool(17)
        makeOrtsterminDate(17)
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
        kartei.stamm.az.sachgebiet.Zahl = "5322"
        kartei.stamm.az.sachgebiet.Header = kartei.stamm.az.sachgebiet.Zahl & "-Rauchgasbeschwerden"
        kartei.stamm.az.sachgebiet.isImmischionschutz = True
        kartei.stamm.az.sachgebiet.isUNB = False
        kartei.stamm.az.Vorgangsnummer = 0
        kartei.stamm.az.Prosa = ""
    End Sub
    Public Sub makeBearbeiter(ByVal nummer As Integer)
        If p(nummer).ToLower.StartsWith("pl") Then
            kartei.stamm.Bearbeiter = "plöb"
        Else
            kartei.stamm.Bearbeiter = "klib"
        End If
    End Sub
    Public Sub makeEingang(ByVal nummer As Integer)
        Try
            kartei.stamm.Eingangsdatum = Nothing
            If String.IsNullOrEmpty(p(nummer)) Then Exit Sub
            If p(nummer) = "" Then
                Exit Sub
            End If
            kartei.stamm.Eingangsdatum = CDate(p(nummer))
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
        kartei.stamm.Beschreibung = p(3) & ", " & p(4) & "-" & p(5) 
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
        kartei.stamm.Bemerkung = "Verursacher: " & p(2) & ", " & p(3) & ", " & p(4) & ", " & p(5) & ", " & p(6) & " " & Environment.NewLine &
            "Beschwerdeführer: " & p(7) & ", " & p(8) & ", " & p(9) & ", " & p(10) & " " & Environment.NewLine &
            "Eingang: " & p(11) & Environment.NewLine &
            "Anhörung: " & p(13) & Environment.NewLine &
            "Verfügung: " & p(14) & Environment.NewLine &
            "Widerspruch: " & p(15) & Environment.NewLine &
            "Vollstr.Maßn.: " & p(16) & Environment.NewLine &
            "Ortstermin: " & p(17) & Environment.NewLine &
            "Erledigt: " & p(18) & Environment.NewLine &
            "  " & p(19) & Environment.NewLine  
        reduce(540, kartei.stamm.Bemerkung)
    End Sub

    Public Sub makeLetztebearbeitung(ByVal nummer As Integer)
        kartei.stamm.LetzteBearbeitung = Nothing
        If String.IsNullOrEmpty(p(nummer)) Then
            kartei.stamm.LetzteBearbeitung = kartei.stamm.Eingangsdatum
            Exit Sub
        End If

        Try
            Dim test As Date = CDate(p(nummer))
            kartei.stamm.LetzteBearbeitung = test
        Catch ex As Exception
            'zurück
            kartei.stamm.LetzteBearbeitung = Nothing
        Finally
            If kartei.stamm.LetzteBearbeitung = Nothing Then
                kartei.stamm.LetzteBearbeitung = kartei.stamm.Eingangsdatum
            End If
        End Try
    End Sub

    Public Sub makeOrtsterminBool(ByVal nummer As Integer)
        If String.IsNullOrEmpty(p(nummer)) Then
            kartei.stamm.Ortstermin = False
        Else
            kartei.stamm.Ortstermin = True
        End If

    End Sub

    Public Sub makeOrtsterminDate(ByVal nummer As Integer)
        If String.IsNullOrEmpty(p(16)) Then
            'stamm.LastActionHeroe = False
        Else
            kartei.stamm.LastActionHeroe = "Ortstermin: " & p(16)
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
        If Not String.IsNullOrEmpty(p(4)) Then
            gemarkung = p(4)
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



