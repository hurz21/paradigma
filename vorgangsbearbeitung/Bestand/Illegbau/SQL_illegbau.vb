Public Class SQL_illegbau
    Property name As String
    Property vorname As String
    Property stadt As String
    Property strasse As String
    Property result As String
    Private Property _tablename As String
    Sub New(ByVal tablename As String)
        _tablename = tablename
    End Sub
    Public Sub compoze(_suchtext As String, statusPlanmaessig As Boolean,
                       statuslaufend As Boolean,
                       statuserledigt As Boolean,
                       statusrecherche As Boolean)
        nachricht("in SQLerzeugenStamm  compoze  -------------------------")
        Try
            Dim whereString As String = ""
            whereString = buildWherestring(_suchtext, statusPlanmaessig,
                       statuslaufend,
                       statuserledigt,
                       statusrecherche)
            result = "select * from " & _tablename & " di1 " & whereString
            nachricht(String.Format("1compoze: {0}{1}", vbCrLf, result))
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in SQLerzeugen: " & ex.ToString)
        End Try
    End Sub

    Private Shared Function buildWherestring(_suchtext As String,
                       statusPlanmaessig As Boolean,
                       statuslaufend As Boolean,
                       statuserledigt As Boolean,
                       statusrecherche As Boolean) As String
        Dim whereString As String = " where vorgangsid >0 "

        whereString = whereString & bildeSuchtext(_suchtext)
        whereString = whereString &
                      bildeStatus(statusPlanmaessig,
                       statuslaufend,
                       statuserledigt,
                       statusrecherche)
        Return whereString
    End Function

    Private Shared Function bildeStatus(statusPlanmaessig As Boolean, statuslaufend As Boolean, statuserledigt As Boolean, statusrecherche As Boolean) As String
        Dim recherchestring As String
        Dim gesamt As String = " and ("
        If statusrecherche Then
            recherchestring = "  status=4 "
        Else
            recherchestring = ""
        End If
        gesamt = gesamt & recherchestring
        If statuserledigt Then
            If gesamt.EndsWith("(") Then
                recherchestring = "   status=3 "
            Else
                recherchestring = " or status=3 "
            End If

        Else
            recherchestring = ""
        End If
        gesamt = gesamt & recherchestring
        If statuslaufend Then
            If gesamt.EndsWith("(") Then
                recherchestring = "   status=2 "
            Else
                recherchestring = " or status=2 "
            End If
        Else
            recherchestring = ""
        End If
        gesamt = gesamt & recherchestring
        If statusPlanmaessig Then
            If gesamt.EndsWith("(") Then
                recherchestring = "   status=1 "
            Else
                recherchestring = " or status=1 "
            End If
        Else
            recherchestring = ""
        End If
        gesamt = gesamt & recherchestring & ")"
        Return gesamt
    End Function

    Private Shared Function bildeSuchtext(_suchtext As String) As String

        Dim suchtext As String
        If _suchtext = String.Empty Then
            suchtext = ""
        Else
            suchtext = " and lower(beschreibung) like '%" & _suchtext.ToLower & "%' or " &
                "  lower(az2) like '%" & _suchtext.ToLower & "%'   "
        End If

        Return suchtext
    End Function

    'Private Function GetSqlname() As String
    '    Dim sqlname$
    '    If String.IsNullOrEmpty(name) Then
    '        sqlname = ""
    '    Else
    '        sqlname = String.Format(" and lower(b.nachname) like '%{0}%'", name$.ToLower)
    '    End If
    '    Return sqlname
    'End Function

    'Private Function GetSqlvorname$()
    '    Dim sqlvorname$
    '    If String.IsNullOrEmpty(vorname) Then
    '        sqlvorname = ""
    '    Else
    '        sqlvorname = String.Format(" and lower(b.vorname) like '%{0}%'", vorname.ToLower)
    '    End If
    '    Return sqlvorname
    'End Function

    'Private Function GetSqlstadt$()
    '    Dim sqlstadt$
    '    If String.IsNullOrEmpty(stadt) Then
    '        sqlstadt$ = ""
    '    Else
    '        sqlstadt$ = String.Format(" and lower(b.gemeindename) like '{0}%'", stadt.ToLower)
    '    End If
    '    Return sqlstadt$
    'End Function

    'Private Function GetSqlstrasse$()
    '    Dim sqlstrasse$
    '    If String.IsNullOrEmpty(strasse) Then
    '        sqlstrasse$ = ""
    '    Else
    '        sqlstrasse$ = String.Format(" and lower(b.strasse) like '%{0}%'", strasse.ToLower)
    '    End If
    '    Return sqlstrasse$
    'End Function

    'Private Shared Function GetAnfang$(Optional nurInitiale As String = "")
    '    Dim anfang As String
    '    If nurInitiale.IsNothingOrEmpty Then
    '        anfang = "select distinct  s.VorgangsID, s.Ortstermin,s.Stellungnahme, v.AZ, s.Beschreibung,s.az2," +
    '                   " s.LetzteBearbeitung,s.LastActionHeroe, s.Eingang ," &
    '                   "b.anrede,b.NACHNAME as NACHNAME,b.Vorname,b.Gemeindename, b.Strasse,b.FFemail" &
    '                   " from  " & CLstart.myViewsNTabs.tabBeteiligte & " b, " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & " s " &
    '                   " where b.vorgangsid=v.vorgangsid " &
    '                   " and b.vorgangsid=s.vorgangsid "
    '    Else
    '        anfang = "select distinct  s.VorgangsID, s.Ortstermin,s.Stellungnahme, v.AZ, s.Beschreibung,s.az2," +
    '                   " s.LetzteBearbeitung,s.LastActionHeroe, s.Eingang ," &
    '                   "b.anrede,b.NACHNAME as NACHNAME,b.Vorname,b.Gemeindename, b.Strasse,b.FFemail" &
    '                   " from  " & CLstart.myViewsNTabs.tabBeteiligte & " b, " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & " s " &
    '                   " where b.vorgangsid=v.vorgangsid " &
    '                   " and b.vorgangsid=s.vorgangsid " &
    '                   " and ((lower(s.bearbeiter)= '" & nurInitiale.ToLower & "') or (s.weiterebearb like '%" & nurInitiale.ToLower & "%'))"
    '    End If

    '    Return anfang
    'End Function
End Class


