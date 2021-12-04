Public Class SQL_Beteiligte
    Property name As String
    Property vorname As String
    Property stadt As String
    Property strasse As String
    Property result As String
    Private Property _dbtyp$
    Sub New(ByVal dbtyp$)
        _dbtyp = dbtyp
    End Sub
    Public Sub compoze(Optional nurInitiale As String = "")
        nachricht("in SQLerzeugenStamm  compoze-------------------------")
        Try
            Dim anfang$ = GetAnfang(nurInitiale)
            Dim sqlname$ = GetSqlname()
            Dim sqlvorname$ = GetSqlvorname()
            Dim sqlstadt$ = GetSqlstadt()
            Dim sqlstrasse$ = GetSqlstrasse()
            Dim limitstring As String = GetLimitstring(anfang$, "50")
            Dim orderstring$ = " order by nachname,vorname,gemeindename,strasse "
            result = anfang & sqlname & sqlvorname & sqlstadt$ & sqlstrasse & orderstring$ & limitstring
            nachricht(String.Format("1compoze: {0}{1}", vbCrLf, result))
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in SQLerzeugen: " & ex.ToString)
        End Try
    End Sub
    'Private Shared Function GetLimitstring() As String
    '    Return " limit 50 "
    'End Function


    Private Function GetLimitstring$(ByRef anfang$, ByVal LIMIT_NR$)
        If _dbtyp = "mysql" Then
            Dim limitstring$ = String.Format(" limit {0} ", LIMIT_NR$)
            Return limitstring
        End If
        If _dbtyp = "oracle" Then
            'Dim limitstring$ = String.Format(" and rownum< {0} ", CStr(Val(LIMIT_NR$) + 1)) 'beginnt bei 0
            'anfang = anfang & limitstring
            Return ""
        End If
        Return "" 'limitstring
    End Function


    Private Function GetSqlname$()
        Dim sqlname$
        If String.IsNullOrEmpty(name) Then
            sqlname = ""
        Else
            sqlname = String.Format(" and lower(b.nachname) like '%{0}%'", name$.ToLower)
        End If
        Return sqlname
    End Function

    Private Function GetSqlvorname$()
        Dim sqlvorname$
        If String.IsNullOrEmpty(vorname) Then
            sqlvorname = ""
        Else
            sqlvorname = String.Format(" and lower(b.vorname) like '%{0}%'", vorname.ToLower)
        End If
        Return sqlvorname
    End Function

    Private Function GetSqlstadt$()
        Dim sqlstadt$
        If String.IsNullOrEmpty(stadt) Then
            sqlstadt$ = ""
        Else
            sqlstadt$ = String.Format(" and lower(b.gemeindename) like '{0}%'", stadt.ToLower)
        End If
        Return sqlstadt$
    End Function

    Private Function GetSqlstrasse$()
        Dim sqlstrasse$
        If String.IsNullOrEmpty(strasse) Then
            sqlstrasse$ = ""
        Else
            sqlstrasse$ = String.Format(" and lower(b.strasse) like '%{0}%'", strasse.ToLower)
        End If
        Return sqlstrasse$
    End Function

    Private Shared Function GetAnfang$(Optional nurInitiale As String = "")
        Dim anfang As String
        If nurInitiale.IsNothingOrEmpty Then
            anfang = "select distinct  s.VorgangsID, s.Ortstermin,s.Stellungnahme, v.AZ, s.Beschreibung,s.az2," + _
                       " s.LetzteBearbeitung,s.LastActionHeroe, s.Eingang ," &
                       "b.anrede,b.NACHNAME as NACHNAME,b.Vorname,b.Gemeindename, b.Strasse,b.FFemail" &
                       " from beteiligte b, vorgang v,stammdaten s " & _
                       " where b.vorgangsid=v.vorgangsid " & _
                       " and b.vorgangsid=s.vorgangsid "
        Else
            anfang = "select distinct  s.VorgangsID, s.Ortstermin,s.Stellungnahme, v.AZ, s.Beschreibung,s.az2," + _
                       " s.LetzteBearbeitung,s.LastActionHeroe, s.Eingang ," &
                       "b.anrede,b.NACHNAME as NACHNAME,b.Vorname,b.Gemeindename, b.Strasse,b.FFemail" &
                       " from beteiligte b, vorgang v,stammdaten s " & _
                       " where b.vorgangsid=v.vorgangsid " & _
                       " and b.vorgangsid=s.vorgangsid " &
                       " and ((lower(s.bearbeiter)= '" & nurInitiale.ToLower & "') or (s.weiterebearb like '%" & nurInitiale.ToLower & "%'))"
        End If

        Return anfang
    End Function
End Class
