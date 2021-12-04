Module SQL_Ereignis
    Dim summe As String
    Dim anfang As String
    Dim text_alle As String = ""
    Dim artstring As String
    Dim beschreibungSTRING As String
    Dim notizSTRING As String
    Dim orderString, datumstring As String
    Dim mitnotiz As Boolean
    Dim sachgebietnr_text As String
    Dim bearbeiterString As String
    Property fuerAlleBearbeiter As Boolean =false'fuerAlleBearbeiter
    Property fuerBearbeiterInitiale As String
    Property fuerBearbeiterKuerzel As String

    Function sql_4EreignisErstellen(ByVal textfilter As String, ByVal art As String, ByVal _mitnotiz As Boolean,
                                    sachgebietnr As String,
                                    datummodus As String,
                                    datumvon As String,
                                    datumbis As String,
                                    Nurprojekt As Boolean,
                                    fuerBearbeiterName As String) As String
        fuerBearbeiterInitiale = fuerBearbeiterName
        fuerBearbeiterKuerzel = getKuerzelForInitial(fuerBearbeiterName)

        bearbeiterString = GetBearbeiterString()
        mitnotiz = _mitnotiz
        anfang = getanfang(Nurprojekt, " top 100 ")
        artstring = GetArtstring(art)
        beschreibungSTRING = getBeschreibung(textfilter)
        notizSTRING = ""
        'notizSTRING = buildnotizString(textfilter)
        orderString = buildOrderString(Nurprojekt)
        datumstring = buildDatumString(datummodus, datumvon, datumbis)
        sachgebietnr_text = buildSachgebietNrText(sachgebietnr)
        If Nurprojekt Then
            summe = anfang & artstring & sachgebietnr_text & bearbeiterString & datumstring & orderString
        Else
            summe = anfang & artstring & sachgebietnr_text & bearbeiterString & datumstring & beschreibungSTRING & notizSTRING & orderString
        End If
        Return summe
    End Function
    Private Function getKuerzelForInitial(ByVal Initiale As String) As String
        If String.IsNullOrEmpty(Initiale.ToLower) Then Return ""
        If Initiale.ToLower = "alle" Then Return ""
        Dim testbearbeiter As New clsBearbeiter
        'If userTools.initBearbeiterByUserid_ausParadigmadb(testbearbeiter, "INITIAL_", Initiale) Then
        If userTools.initBearbeiterByUserid_ausParadigmadb(testbearbeiter) Then
            Return testbearbeiter.Kuerzel2Stellig
        Else
            Return ""
        End If
    End Function
    Private Function GetArtstring(ByVal art As String) As String
        Dim artstring As String = ""
        If art Is Nothing Then
            artstring = "Hinzufügen"
        Else
            If art Is Nothing OrElse art.ToString = text_alle Then
                artstring = ""
            Else
                artstring = String.Format(" and lower(e.art) like '{0}%' ", art.ToString.Trim.ToLower)
            End If
        End If
        Return artstring
    End Function

    Private Function getBeschreibung(ByVal textfilter As String) As String
        Dim a As String()
        If textfilter.IsNothingOrEmpty Then
            beschreibungSTRING = ""
            Return beschreibungSTRING
        End If
        a = textfilter.Split(" "c)
        beschreibungSTRING = " and ("
        For i = 0 To a.Count - 1
            If i > 0 Then
                beschreibungSTRING = beschreibungSTRING &
            " and ((LOWER(e.beschreibung)  like '%" & a(i) & "%') or " &
            "  (LOWER(s.beschreibung)  like '%" & a(i) & "%')  " &
             "  "
            Else
                beschreibungSTRING = beschreibungSTRING &
            " ((LOWER(e.beschreibung)  like '%" & a(i) & "%') or " &
            "  (LOWER(s.beschreibung)  like '%" & a(i) & "%')  " &
             "  "
            End If

        Next
        '(LOWER(e.notiz)  like '%" & textfilter & "%')
        beschreibungSTRING = beschreibungSTRING & ")) "
        Return beschreibungSTRING
    End Function

    Private Function getanfang(Nurprojekt As Boolean, TopLimit As String) As String
        Dim anfang As String
        anfang = "select distinct  " & TopLimit & " s.VorgangsID, s.Ortstermin,s.Stellungnahme, v.AZ, s.Beschreibung, s.az2," +
                   " s.LetzteBearbeitung,s.LastActionHeroe, s.Eingang , e.art,e.beschreibung as ebeschreibung,e.datum,bearbeiter," &
                   "weiterebearb,v.sachgebietnr,s.eingang,s.letztebearbeitung " &
                   " from " & CLstart.myViewsNTabs.tabEreignis & "  e, " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & " s " &
                   " where e.vorgangsid=v.vorgangsid " &
                   " and v.vorgangsid=s.vorgangsid "
        If Nurprojekt Then

            anfang = "select distinct " & TopLimit & "s.VorgangsID, s.Ortstermin,s.Stellungnahme, v.AZ, s.Beschreibung, s.az2," +
                 " s.LetzteBearbeitung,s.LastActionHeroe, s.Eingang ,  bearbeiter," &
                 "weiterebearb,v.sachgebietnr,s.eingang,s.letztebearbeitung " &
                 " from " & CLstart.myViewsNTabs.tabEreignis & "  e, " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & " s " &
                 " where e.vorgangsid=v.vorgangsid " &
                 " and v.vorgangsid=s.vorgangsid "

        End If
        Return anfang
    End Function

    Private Function buildOrderString(Nurprojekt As Boolean) As String
        If Nurprojekt Then
            Return " order by s.vorgangsid desc "
        Else
            Return " order by s.vorgangsid desc, e.datum desc"
        End If
    End Function

    'Private Function buildnotizString(ByVal textfilter As String) As String
    '    If mitnotiz Then
    '        textfilter = textfilter.ToLower.Trim
    '        notizSTRING = " or LOWER(e.notiz)  like '%" & textfilter & "%' "
    '        Return notizSTRING
    '    Else
    '        Return ""
    '    End If
    'End Function

    Private Function buildSachgebietNrText(sachgebietnr As String) As String
        If String.IsNullOrEmpty(sachgebietnr) Then
            Return ""
        End If
        Dim Sachgebietsnrstring As String = ""

        If sachgebietnr Is Nothing OrElse sachgebietnr = text_alle Then
            Sachgebietsnrstring = ""
        Else
            Sachgebietsnrstring = String.Format(" and lower(Sachgebietnr)='{0}' ", sachgebietnr.ToLower)
        End If

        Return Sachgebietsnrstring
    End Function

    Private Function buildDatumString(datummodus As String, datumvon As String, datumbis As String) As String

        Dim datumsfeld$ = ""
        Dim ret$ = ""
        If datummodus.Contains("kein") Then Return ""
        If datummodus.ToLower.Contains("letzte") Then datumsfeld = "LETZTEBEARBEITUNG"
        If datummodus.ToLower.Contains("eingang") Then datumsfeld = "EINGANG"
        If datummodus.ToLower.Contains("ereignis") Then datumsfeld = "e.datum"
        If datumvon = "" And datumbis = "" Then
            Return ""
        End If

        If Not String.IsNullOrEmpty(datumvon) And datumbis = "" Then
            'nurvon
            ret$ = " and (" & datumsfeld & " > to_date('" & datumvon & "','DD.MM.YYYY')) "
        End If
        If Not String.IsNullOrEmpty(datumbis) And datumvon = "" Then
            'nurbis
            ret$ = " and (" & datumsfeld & " < to_date('" & datumbis & "','DD.MM.YYYY')) "
        End If

        If Not String.IsNullOrEmpty(datumbis) And Not String.IsNullOrEmpty(datumvon) Then
            'bei
            ret$ = " and (" & datumsfeld & " > to_date('" & datumvon & "','DD.MM.YYYY') and " & datumsfeld & " < to_date('" & datumbis & "','DD.MM.YYYY')) "
        End If
        Return ret

    End Function

    Private Function GetBearbeiterString() As String
        Dim bearbeiterString = ""
        Dim undMitarbeit As Boolean = False
        If fuerAlleBearbeiter Then
            bearbeiterString = " "
        Else
            If String.IsNullOrEmpty(fuerBearbeiterKuerzel) Then
                Return ""
                'If undMitarbeit Then
                '    bearbeiterString = String.Format(" and ((lower(Bearbeiter)='{0}') or (lower(weiterebearb) like '%{0};%')) ", fuerBearbeiterInitiale.ToLower)
                'Else
                '    bearbeiterString = String.Format(" and ((lower(Bearbeiter)='{0}') ", fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                'End If

            Else
                If undMitarbeit Then
                    bearbeiterString = String.Format(" and (lower(Bearbeiter)='{0}' or lower(Bearbeiter)='{1}'  or lower(weiterebearb) like '%{0};%'  or lower(weiterebearb) like '%{1};%') ", fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                Else
                    bearbeiterString = String.Format(" and (lower(Bearbeiter)='{0}' or lower(Bearbeiter)='{1}') ", fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                End If
            End If
        End If
        Return bearbeiterString
    End Function
End Module
