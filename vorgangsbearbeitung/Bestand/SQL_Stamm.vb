Public Class SQL_Stamm
    Property fuerAlleBearbeiter As Boolean 'fuerAlleBearbeiter
    Property GEMKRZ_selitem As New Object 'GEMKRZ_selitem
    Property GEMKRZ_selval As New Object 'cmbGemKRZ.SelectedValue
    Property text_alle As String
    Property tbsachgebietnr_Text As String 'tbsachgebietnr.Text
    Property LIMIT_NR$
    Property fuerBearbeiterInitiale As String
    Property fuerBearbeiterKuerzel As String
    Property erledigteauswahl As String
    Property mittextfilter As Boolean
    Property tbFilter_Text As String 'tbFilter.Text
    Property result As String ' myGlobalz.sitzung.VorgangREC.mydb.SQL
 
    Property datummodus As String
    Property datumvon As String
    Property datumbis As String

    Private Property _dbtyp$
    Sub New(ByVal dbtyp$)
        _dbtyp = dbtyp
    End Sub

    Public Sub compoze(ByVal rownum As String)
        glob2.nachricht("in SQLerzeugenStamm  compoze-------------------------")
        Try
     
            Dim anfang$ = GetAnfang()
            Dim orderstring$ = GetOrderstring()
            ' Dim limitstring$ = GetLimitstring(anfang)
            Dim bearbeiterString$ = GetBearbeiterString()
            Dim GemKRZstring$ = GetGemKRZstring()
            Dim Sachgebietsnrstring$ = GetSachgebietsnrstring()
            Dim erledigtstring$ = GetErledigtstring()
            Dim Filterstring$ = GetFilterstring()
            Dim datumsString$ = GetDatumsString()
            Dim rntext As String = GetRownumText(rownum)
            '  rntext = ""
            result = anfang$ & bearbeiterString & datumsString & GemKRZstring & Sachgebietsnrstring$ & erledigtstring &
                Filterstring$ & rntext & orderstring$ '& limitstring$
            'If _dbtyp = "oracle" Then
            '    Dim NEUAUSWAHL = getSelectauswahl().Replace("S.VORGANGSID AS ", "VORGANGSID")
            '    NEUAUSWAHL = NEUAUSWAHL.Replace("VORGANGSIDVORGANGSID", "VORGANGSID")
            '    NEUAUSWAHL = NEUAUSWAHL.Replace(",ROWNUM  AS RN", " ")
            '    result = "select " & NEUAUSWAHL & " from (" & result & ") " 'where rn<10"
            'End If

            glob2.nachricht(String.Format("compoze: {0}{1}", vbCrLf, result))
        Catch ex As Exception
            glob2.nachricht_und_Mbox("Fehler in SQLerzeugen: " & ex.ToString)
        End Try
    End Sub

    Private Function GetDatumsString() As String
        Dim datumsfeld$ = ""
        Dim ret$ = ""
        If datummodus.Contains("kein") Then Return ""
        If datummodus.ToLower.Contains("letzte") Then datumsfeld = "LETZTEBEARBEITUNG"
        If datummodus.ToLower.Contains("eingang") Then datumsfeld = "EINGANG"
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

    Private Function GetAnfang() As String
        Dim anfang As String
        ' anfang = "select * FROM stammdaten s, Vorgang v where s.VorgangsID=v.vorgangsid "
        anfang = "select * FROM vs_d where vorgangsid>0 "
        Return anfang$
    End Function

    Private Function GetRownumText(ByVal rownum As String) As String
        Try
            If String.IsNullOrEmpty(rownum) Then
                glob2.nachricht("Max. Anzahl der Treffer ist leer: " & rownum)
                Return " "
            End If
            If Not IsNumeric(rownum) Then
                glob2.nachricht("Max. Anzahl der Treffer ist keine Zahl und daher ungültig: " & rownum)
                Return " "
            End If
            If rownum = "alle" Then
                Return " "
            Else
                Return " and ROWNUM <=  " & rownum & " "
            End If
        Catch ex As Exception
            glob2.nachricht("fehler in GetRownumText:" & ex.ToString)
        End Try
    End Function
 


    Private Function GetBearbeiterString$()
        Dim bearbeiterString$ = ""
        Dim undMitarbeit As Boolean = True
        If fuerAlleBearbeiter Then
            bearbeiterString$ = " "
        Else
            ' bearbeiterString$ = String.Format(" and Bearbeiter='{0}' ", fuerBearbeiterName)
            If String.IsNullOrEmpty(fuerBearbeiterKuerzel) Then
                If undMitarbeit Then
                    bearbeiterString$ = String.Format(" and ((lower(Bearbeiter)='{0}') or (lower(weiterebearb) like '%{0};%')) ", fuerBearbeiterInitiale.ToLower)
                Else
                    bearbeiterString$ = String.Format(" and ((lower(Bearbeiter)='{0}') ", fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                End If

            Else
                If undMitarbeit Then
                    bearbeiterString$ = String.Format(" and (lower(Bearbeiter)='{0}' or lower(Bearbeiter)='{1}'  or lower(weiterebearb) like '%{0};%'  or lower(weiterebearb) like '%{1};%') ", fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                Else
                    bearbeiterString$ = String.Format(" and (lower(Bearbeiter)='{0}' or lower(Bearbeiter)='{1}') ", fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                End If
            End If
        End If
        Return bearbeiterString
    End Function

    Private Function GetOrderstring$()
        Dim orderstring$ = " order by LetzteBearbeitung desc"
        'If _dbtyp = "oracle" Then
        '    orderstring = ""
        '    Return ""
        'End If
        Return orderstring
    End Function

    Private Function GetLimitstring$(ByRef anfang$)
        If _dbtyp = "mysql" Then
            Dim limitstring$ = String.Format(" limit {0} ", LIMIT_NR$)
            Return limitstring
        End If
        If _dbtyp = "oracle" Then
            '  Dim limitstring$ = String.Format(" and rownum< {0} ", CStr(Val(LIMIT_NR$) + 1)) 'beginnt bei 0
            ' limitstring = ""
            'anfang = anfang & limitstring
            Return ""
        End If
        Return "" 'limitstring
    End Function

    Private Function GetGemKRZstring$()
        Dim GemKRZstring$ = ""
        If GEMKRZ_selitem Is Nothing Then
            GemKRZstring$ = ""
        Else
            If GEMKRZ_selval Is Nothing OrElse GEMKRZ_selval.ToString = text_alle Then
                GemKRZstring = ""
            Else
                GemKRZstring = String.Format(" and lower(GemKRZ) like '{0}%' ", GEMKRZ_selval.ToString.Trim.ToLower)
            End If
        End If
        Return GemKRZstring
    End Function

    Private Function GetErledigtstring$()
        Dim erledigtstring$ = ""
        Dim tinyWahr$ = "", tinyFalse$ = ""
        getTinywahr(_dbtyp$, tinyWahr$, tinyFalse$)
        If erledigteauswahl = "erledigte" Then erledigtstring$ = " and erledigt=" & tinyWahr & " "
        If erledigteauswahl = "unerledigte" Then erledigtstring$ = " and erledigt=" & tinyFalse & " "
        If erledigteauswahl = "beides" Then erledigtstring$ = "   "
        Return erledigtstring
    End Function

    Private Function GetFilterstring$()
        Dim Filterstring$ = ""
        If mittextfilter Then
            Dim ft$ = String.Format(" like '%{0}%' ", tbFilter_Text.ToLower)
            'Filterstring$ = String.Format(" and (lower(s.beschreibung) {0} or lower(v.VorgangsGegenstand){0} or lower(v.AZ){0} or lower(s.Probaugaz){0} or lower(v.Sachgebietstext){0} or lower(s.Bemerkung){0})",
            '                              ft$)
            Filterstring$ = String.Format(" and (lower(beschreibung) {0} or lower(VorgangsGegenstand){0} or lower(AZ){0} or lower(Probaugaz){0} or lower(Sachgebietstext){0} or lower(Bemerkung){0})",
                                     ft$)
        Else
            Filterstring$ = " "
        End If
        Return Filterstring
    End Function

    Private Function GetSachgebietsnrstring$()
        Dim Sachgebietsnrstring$ = ""
        If tbsachgebietnr_Text Is Nothing OrElse
            String.IsNullOrEmpty(tbsachgebietnr_Text) Then
            Sachgebietsnrstring$ = ""
        Else
            If tbsachgebietnr_Text Is Nothing OrElse tbsachgebietnr_Text = text_alle Then
                Sachgebietsnrstring$ = ""
            Else
                Sachgebietsnrstring$ = String.Format(" and lower(Sachgebietnr)='{0}' ", tbsachgebietnr_Text.ToLower)
            End If
        End If
        Return Sachgebietsnrstring
    End Function

    Private Sub getTinywahr(ByVal dbtyp As String, ByRef tinyWahr As String, ByRef tinyFalse As String)
        If String.IsNullOrEmpty(_dbtyp) Then
            glob2.nachricht("Fehler in getTinywahr: dbtyp is null")
        End If
        If dbtyp = "oracle" Then
            tinyWahr = "1"
            tinyFalse = "0"
        End If
        If dbtyp = "mysql" Then
            tinyWahr = "true"
            tinyFalse = "false"
        End If
    End Sub

End Class
