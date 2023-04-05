Namespace Paradigma_bestandssuche


    Public Class SQL_Stamm
        Property fuerAlleBearbeiter As Boolean 'fuerAlleBearbeiter
        Property GEMKRZ_selitem As New Object 'GEMKRZ_selitem
        Property GEMKRZ_selval As New Object 'cmbGemKRZ.SelectedValue
        Property text_alle As String
        Property tbsachgebietnr_Text As String 'tbsachgebietnr.Text
        Property LIMIT_NR$
        Property fuerBearbeiterInitiale As String
        Property fuerBearbeiterKuerzel As String
        Property fuerBearbeiterID As Integer
        Property erledigteauswahl As String
        Property mittextfilter As Boolean
        Property tbFilter_Text As String 'tbFilter.Text
        Property result As String ' myGlobalz.sitzung.VorgangREC.mydb.SQL

        Property datummodus As String
        Property datumvon As String
        Property datumbis As String

        Property kosten_ersatzgeld As Boolean
        Property kosten_ersatzgeld_bezahlt As Boolean
        Property kosten_sicherheit As Boolean
        Property kosten_sicherheit_bezahlt As Boolean
        Property kosten_verwaltungsgebuehr As Boolean
        Property kosten_verwaltungsgebuehr_bezahlt As Boolean

        'Property kosten_ersatzgeld_ausgezahlt As Boolean
        '      Property kosten_sicherheit As Boolean
        '      Property kosten_sicherheit_bezahlt As Boolean
        '      Property kosten_verwaltungsgebuehr As Boolean
        '      Property kosten_verwaltungsgebuehr_bezahlt As Boolean




        Property kosten_interneZahlung As Boolean
        Property kosten_Verknuepfung As String

        Property kosten_VERWARNUNGSGELD As Boolean
        Property kosten_VERWARNUNGSGELD_bezahlt As Boolean
        Property kosten_bussGELD As Boolean
        Property kosten_bussGELD_bezahlt As Boolean
        Property stellungnahmeerfolgt As Boolean

        Property beteiligtenSuchen As Boolean
        Property ereignisSuchen As Boolean

        Private Property _dbtyp As String
        Sub New(ByVal dbtyp As String)
            _dbtyp = dbtyp
        End Sub



        Public Sub compozeStamm(ByVal rownum As String, zusatzSuche As String, undMitarbeiter As Boolean)
            nachricht("in SQLerzeugenStamm  compoze-------------------------")
            beteiligtenSuchen = False
            ereignisSuchen = False
            Try
                If zusatzSuche = "beteiligten" Then
                    beteiligtenSuchen = True
                    ereignisSuchen = False
                End If
                If zusatzSuche = "ereignis" Then
                    beteiligtenSuchen = False
                    ereignisSuchen = True
                End If
                Dim anfang As String = GetAnfang(rownum)
                Dim orderstring As String = "" 'GetOrderstring()
                Dim bearbeiterString = GetBearbeiterString(undMitarbeiter)
                Dim GemKRZstring As String = GetGemKRZstring()
                Dim Sachgebietsnrstring As String = GetSachgebietsnrstring()
                Dim erledigtstring As String = GetErledigtstring()
                Dim Filterstring As String = GetFilterstring()
                Dim datumsString As String = GetDatumsStringSQLS() 'GetDatumsStringOracle()
                Dim kostenstring As String = getKostenstring()
                Dim rntext As String = "" 'GetRownumTextOracle(rownum)
                rntext = "" ' wg paginierung nach oben verlegt
                Dim stellungnahmeString As String = getstellungnahme()
                result = anfang & bearbeiterString & datumsString & GemKRZstring & Sachgebietsnrstring &
                         erledigtstring & kostenstring & stellungnahmeString &
                         Filterstring & getBeteiligtenverKnuepfung() & getEreignisverKnuepfung() & rntext & orderstring '& limitstring$
                nachricht(String.Format("compoze: {0}{1}", vbCrLf, result))

            Catch ex As Exception
                nachricht_und_Mbox("Fehler in SQLerzeugen: " & ex.ToString)
            End Try
        End Sub

        Private Function GetDatumsStringSQLS() As String
            Dim datumsfeld As String = ""
            Dim ret As String = ""
            If datummodus.Contains("kein") Then Return ""
            If datummodus.ToLower.Contains("letzte") Then datumsfeld = "LETZTEBEARBEITUNG"
            If datummodus.ToLower.Contains("eingang") Then datumsfeld = "EINGANG"
            If datumvon = "" And datumbis = "" Then
                Return ""
            End If

            If Not String.IsNullOrEmpty(datumvon) And datumbis = "" Then
                'nurvon
                ret = " and (" & datumsfeld & " > CONVERT(datetime,'" & datumvon & "' )) "
            End If
            If Not String.IsNullOrEmpty(datumbis) And datumvon = "" Then
                'nurbis
                ret = " and (" & datumsfeld & " < CONVERT(datetime,'" & datumbis & "' ) "
            End If

            If Not String.IsNullOrEmpty(datumbis) And Not String.IsNullOrEmpty(datumvon) Then
                'bei
                ret = " and (" & datumsfeld & " > CONVERT(datetime,'" & datumvon & "' ) and " & datumsfeld &
                    " < CONVERT(datetime,'" & datumbis & "' )) "
            End If
            Return ret
        End Function

        Function getBeteiligtenverKnuepfung() As String
            If beteiligtenSuchen Then
                Return "   and b.vorgangsid=v.vorgangsid  "
            Else
                Return ""
            End If
        End Function

        Function getEreignisverKnuepfung() As String
            If ereignisSuchen Then
                Return "   and e.vorgangsid=v.vorgangsid  "
            Else
                Return ""
            End If
        End Function
        Private Function GetDatumsStringOracle() As String
            Dim datumsfeld As String = ""
            Dim ret As String = ""
            If datummodus.Contains("kein") Then Return ""
            If datummodus.ToLower.Contains("letzte") Then datumsfeld = "LETZTEBEARBEITUNG"
            If datummodus.ToLower.Contains("eingang") Then datumsfeld = "EINGANG"
            If datumvon = "" And datumbis = "" Then
                Return ""
            End If

            If Not String.IsNullOrEmpty(datumvon) And datumbis = "" Then
                'nurvon
                ret = " and (" & datumsfeld & " > to_date('" & datumvon & "','DD.MM.YYYY')) "
            End If
            If Not String.IsNullOrEmpty(datumbis) And datumvon = "" Then
                'nurbis
                ret = " and (" & datumsfeld & " < to_date('" & datumbis & "','DD.MM.YYYY')) "
            End If

            If Not String.IsNullOrEmpty(datumbis) And Not String.IsNullOrEmpty(datumvon) Then
                'bei
                ret = " and (" & datumsfeld & " > to_date('" & datumvon & "','DD.MM.YYYY') and " & datumsfeld & " < to_date('" & datumbis & "','DD.MM.YYYY')) "
            End If
            Return ret
        End Function


        Private Function GetAnfang(rownum As String) As String
            Dim anfang, beteiligte, ereignis As String
            If beteiligtenSuchen Then
                beteiligte = ", , " & CLstart.myViewsNTabs.tabBeteiligte & " b "
            Else
                beteiligte = " "
            End If
            If ereignisSuchen Then
                ereignis = ",  " & CLstart.myViewsNTabs.tabEreignis & "  e "
            Else
                ereignis = " "
            End If
            Dim topnum As String = getTopNum(rownum)
            anfang = " select distinct v.vorgangsid FROM (" & CLstart.myViewsNTabs.view_vsk_d & ") v" & beteiligte & ereignis & " where v.vorgangsid>0 "
            Return anfang
        End Function

        Private Function getTopNum(rownum As String) As String
            Try
                If String.IsNullOrEmpty(rownum) Then
                    nachricht("Max. Anzahl der Treffer ist leer: " & rownum)
                    Return " top  1000 " & " "
                    ' Return " "
                End If
                If rownum = "alle" Then
                    Return " "
                Else
                    Return " top  " & rownum & " "
                End If
                If Not IsNumeric(rownum) Then
                    nachricht("Max. Anzahl der Treffer ist keine Zahl und daher ungültig: " & rownum)
                    Return " "
                End If
            Catch ex As Exception
                nachricht("fehler in getTopNum:" & ex.ToString)
                Return " "
            End Try
        End Function

        Private Function GetRownumTextOracle(ByVal rownum As String) As String
            Try
                If String.IsNullOrEmpty(rownum) Then
                    nachricht("Max. Anzahl der Treffer ist leer: " & rownum)
                    Return " and ROWNUM <=  1000" & " "
                    ' Return " "
                End If
                If Not IsNumeric(rownum) Then
                    nachricht("Max. Anzahl der Treffer ist keine Zahl und daher ungültig: " & rownum)
                    Return " "
                End If
                If rownum = "alle" Then
                    Return " "
                Else
                    Return " and ROWNUM <=  " & rownum & " "
                End If
            Catch ex As Exception
                nachricht("fehler in GetRownumText:" & ex.ToString)
                Return " "
            End Try
        End Function
        Private Function GetBearbeiterString(undMitarbeit As Boolean) As String
            Dim bearbeiterString = ""
            'Dim undMitarbeit As Boolean = True
            If fuerAlleBearbeiter Then
                bearbeiterString = " "
            Else
                If String.IsNullOrEmpty(fuerBearbeiterKuerzel) Then
                    If undMitarbeit Then
                        bearbeiterString = String.Format(" and (bearbeiterid={0};)) ",
                                                         fuerBearbeiterID)
                    Else
                        bearbeiterString = String.Format(" and ((Bearbeiterid)={0}) ",
                                                         fuerBearbeiterID)
                    End If

                Else
                    If undMitarbeit Then
                        bearbeiterString = String.Format(" and (bearbeiterid={2} or lower(weiterebearb) like '%{0};%'  ) ",
                                                         fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower, fuerBearbeiterID)
                        'bearbeiterString = String.Format(" and (bearbeiterid={2} or lower(weiterebearb) like '%{0};%'  or lower(weiterebearb) like '%{1};%') ",
                        '                             fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower, fuerBearbeiterID)
                    Else
                        bearbeiterString = String.Format(" and (bearbeiterid={0}) ",
                                                         fuerBearbeiterID)
                    End If
                End If
            End If
            Return bearbeiterString
        End Function

        Private Function GetBearbeiterStringALT(undMitarbeit As Boolean) As String
            Dim bearbeiterString = ""
            'Dim undMitarbeit As Boolean = True
            If fuerAlleBearbeiter Then
                bearbeiterString = " "
            Else
                If String.IsNullOrEmpty(fuerBearbeiterKuerzel) Then
                    If undMitarbeit Then
                        bearbeiterString = String.Format(" and ((lower(Bearbeiter)='{0}') or (lower(weiterebearb) like '%{0};%')) ",
                                                         fuerBearbeiterInitiale.ToLower)
                    Else
                        bearbeiterString = String.Format(" and ((lower(Bearbeiter)='{0}') ",
                                                         fuerBearbeiterInitiale.ToLower)
                    End If

                Else
                    If undMitarbeit Then
                        bearbeiterString = String.Format(" and (lower(Bearbeiter)='{0}' or lower(Bearbeiter)='{1}'  or lower(weiterebearb) like '%{0};%'  or lower(weiterebearb) like '%{1};%') ",
                                                         fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                    Else
                        bearbeiterString = String.Format(" and (lower(Bearbeiter)='{0}' or lower(Bearbeiter)='{1}') ",
                                                         fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower)
                    End If
                End If
            End If
            Return bearbeiterString
        End Function

        'Private Function GetOrderstring() As String
        '    Dim orderstring As String = " order by LetzteBearbeitung desc"
        '    'If _dbtyp = "oracle" Then
        '    '    orderstring = ""
        '    '    Return ""
        '    'End If
        '    Return orderstring
        'End Function

        'Private Function GetLimitstring(ByRef anfang As String) As String
        '    If _dbtyp = "mysql" Then
        '        Dim limitstring = String.Format(" limit {0} ", LIMIT_NR)
        '        Return limitstring
        '    End If
        '    If _dbtyp = "oracle" Then
        '        '  Dim limitstring$ = String.Format(" and rownum< {0} ", CStr(Val(LIMIT_NR$) + 1)) 'beginnt bei 0
        '        ' limitstring = ""
        '        'anfang = anfang & limitstring
        '        Return ""
        '    End If
        '    Return "" 'limitstring
        'End Function

        Private Function GetGemKRZstring() As String
            Dim GemKRZstring As String = ""
            If GEMKRZ_selitem Is Nothing Then
                GemKRZstring = ""
            Else
                If GEMKRZ_selval Is Nothing OrElse GEMKRZ_selval.ToString = text_alle Then
                    GemKRZstring = ""
                Else
                    GemKRZstring = String.Format(" and lower(GemKRZ) like '{0}%' ", GEMKRZ_selval.ToString.Trim.ToLower)
                End If
            End If
            Return GemKRZstring
        End Function

        Private Function GetErledigtstring() As String
            Dim erledigtstring As String = ""
            Dim tinyWahr As String = "", tinyFalse As String = ""
            getTinywahr(_dbtyp, tinyWahr, tinyFalse)
            If erledigteauswahl = "erledigte" Then erledigtstring = " and erledigt=" & tinyWahr & " "
            If erledigteauswahl = "unerledigte" Then erledigtstring = " and erledigt=" & tinyFalse & " "
            If erledigteauswahl = "beides" Then erledigtstring = "   "
            Return erledigtstring
        End Function

        Private Function GetFilterstring() As String
            tbFilter_Text = tbFilter_Text.Trim
            Dim Filterstring As String = ""
            Dim a As String()
            a = tbFilter_Text.Split(" "c)
            If mittextfilter Then
                Filterstring = " and ( "
                For i = 0 To a.Count - 1
                    Dim ft As String = String.Format(" like '%{0}%' ", a(i).ToLower)
                    If bestandTools.verschiedenes.mitZusatzSuche(beteiligtenSuchen, ereignisSuchen) Then
                        If beteiligtenSuchen Then
                            Filterstring = String.Format(" and (lower(b.nachname){0} or lower(b.orgname){0}) ", ft)
                        End If
                        If ereignisSuchen Then
                            Filterstring = String.Format(" and (LOWER(e.beschreibung) {0}  or LOWER(e.notiz)  {0}) ", ft)
                        End If
                    Else
                        If i > 0 Then
                            Filterstring = Filterstring &
                      String.Format(" and  (lower(v.beschreibung) {0} or lower(v.VorgangsGegenstand){0} or lower(AZ){0} " &
                                       "or lower(v.Probaugaz){0} or lower(Sachgebietstext){0} or lower(v.Bemerkung){0}) ", ft)
                        Else
                            Filterstring = Filterstring &
                                                  String.Format("   (lower(v.beschreibung) {0} or lower(v.VorgangsGegenstand){0} or lower(AZ){0} " &
                                                                   "or lower(v.Probaugaz){0} or lower(Sachgebietstext){0} or lower(v.Bemerkung){0}) ", ft)
                        End If

                    End If
                Next
                Filterstring = Filterstring & ")"
            Else
                Filterstring = " "
            End If
            Return Filterstring
        End Function

        Private Function GetSachgebietsnrstring() As String
            Dim Sachgebietsnrstring As String = ""
            If tbsachgebietnr_Text Is Nothing OrElse
                String.IsNullOrEmpty(tbsachgebietnr_Text) Then
                Sachgebietsnrstring = ""
            Else
                If tbsachgebietnr_Text Is Nothing OrElse tbsachgebietnr_Text = text_alle Then
                    Sachgebietsnrstring = ""
                Else
                    Sachgebietsnrstring = String.Format(" and lower(Sachgebietnr)='{0}' ", tbsachgebietnr_Text.ToLower)
                End If
            End If
            Return Sachgebietsnrstring
        End Function

        Private Sub getTinywahr(ByVal dbtyp As String, ByRef tinyWahr As String, ByRef tinyFalse As String)
            If String.IsNullOrEmpty(_dbtyp) Then
                nachricht("Fehler in getTinywahr: dbtyp is null")
            End If
            If dbtyp = "oracle" Then
                tinyWahr = "1"
                tinyFalse = "0"
            End If
            If dbtyp = "mysql" Then
                tinyWahr = "true"
                tinyFalse = "false"
            End If
            If dbtyp = "sqls" Then
                tinyWahr = "1"
                tinyFalse = "0"
            End If
        End Sub

        Private Function keineKosten() As Boolean
            Return Not (kosten_ersatzgeld Or
                        kosten_ersatzgeld_bezahlt Or
                        kosten_interneZahlung Or
                        kosten_sicherheit Or
                        kosten_sicherheit_bezahlt Or
                        kosten_verwaltungsgebuehr Or
                        kosten_verwaltungsgebuehr_bezahlt Or
                         kosten_VERWARNUNGSGELD Or
                        kosten_VERWARNUNGSGELD_bezahlt Or
                         kosten_bussGELD Or
                        kosten_bussGELD_bezahlt)
        End Function
        Private Function getKostenstring() As String
            If keineKosten() Then Return ""
            Dim kostenstring As String = " and ("
            Dim fortsetzung As Boolean = False

            If kosten_ersatzgeld Then
                kostenstring = kostenstring & " ersatzgeld=1 " : fortsetzung = True
            End If
            If kosten_ersatzgeld_bezahlt Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "ersatzgeld_bezahlt=1 "
                Else
                    kostenstring = kostenstring & " ersatzgeld_bezahlt=1 "
                End If
                fortsetzung = True
            End If
            If kosten_sicherheit Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "sicherheit=1 "
                Else
                    kostenstring = kostenstring & " sicherheit=1 "
                End If
                fortsetzung = True
            End If
            If kosten_sicherheit_bezahlt Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "sicherheit_bezahlt=1 "
                Else
                    kostenstring = kostenstring & " sicherheit_bezahlt=1 "
                End If
                fortsetzung = True
            End If
            If kosten_verwaltungsgebuehr Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "verwaltungsgebuehr=1 "
                Else
                    kostenstring = kostenstring & " verwaltungsgebuehr=1 "
                End If
                fortsetzung = True
            End If
            If kosten_verwaltungsgebuehr_bezahlt Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "verwaltungsgebuehr_bezahlt=1 "
                Else
                    kostenstring = kostenstring & " verwaltungsgebuehr_bezahlt=1 "
                End If
                fortsetzung = True
            End If
            If kosten_interneZahlung Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "internezahlung=1 "
                Else
                    kostenstring = kostenstring & " internezahlung=1 "
                End If
                fortsetzung = True
            End If



            If kosten_VERWARNUNGSGELD Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "VERWARNUNGSGELD=1 "
                Else
                    kostenstring = kostenstring & " VERWARNUNGSGELD=1 "
                End If
                fortsetzung = True
            End If
            If kosten_VERWARNUNGSGELD_bezahlt Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "VERWARNUNGSGELD_bezahlt=1 "
                Else
                    kostenstring = kostenstring & " VERWARNUNGSGELD_bezahlt=1 "
                End If
                fortsetzung = True
            End If


            If kosten_bussGELD Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "bussGELD=1 "
                Else
                    kostenstring = kostenstring & " bussGELD=1 "
                End If
                fortsetzung = True
            End If
            If kosten_bussGELD_bezahlt Then
                If fortsetzung Then
                    kostenstring = kostenstring & "" & kosten_Verknuepfung & "bussGELD_bezahlt=1 "
                Else
                    kostenstring = kostenstring & " bussGELD_bezahlt=1 "
                End If
                fortsetzung = True
            End If



            kostenstring = kostenstring & ") "
            Return kostenstring
        End Function

        Private Function getstellungnahme() As String
            Dim st As String = " and ("
            If stellungnahmeerfolgt Then
                Return " and (stellungnahme=1) "
            Else
                Return ""
            End If
        End Function

    End Class

End Namespace
