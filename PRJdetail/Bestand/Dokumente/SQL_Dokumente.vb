Public Class SQL_Dokumente
    Property sqldateiname As String
    Property sqlbeschreibung As String
    Property sqldoktyp As String
    Property sqloriginalfullname As String = ""
    Property doktypfilter As String = ""
    Property doktextfilter As String = ""
    Property result As String
    Private Property _dbtyp$
    Sub New(ByVal dbtyp$)
        _dbtyp = dbtyp
    End Sub
    Public Sub compoze()
        nachricht("in SQLerzeugenStamm  compoze-------------------------")
        Try
            Dim anfang$ = GetAnfang()
            Dim sqldateiname$ = GetSqldateiname()
            Dim sqldoktyp = GetSqldateityp()
            Dim limitstring As String = GetLimitstring(anfang$, "50")
            Dim orderstring$ = " order by nachname,vorname,gemeindename,strasse "
            orderstring = "order by checkindatum desc"
            result = anfang & sqldateiname & sqldoktyp & sqlbeschreibung & sqloriginalfullname & orderstring$ & limitstring
            nachricht(String.Format("1compoze: {0}{1}", vbCrLf, result))
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in SQLerzeugen: " & ex.ToString)
        End Try
    End Sub
    Private Shared Function GetAnfang() As String
        'Dim ANFANG$ = "SELECT DISTINCT  DV.VORGANGSID, D.TYP,D.DATEINAMEEXT,D.DOKUMENTID, D.FILEDATUM,D.CHECKINDATUM,D.ORIGINALFULLNAME" + _
        '         " FROM DOKUMENTE D, DOKUMENT2VORGANG DV " & _
        '         " WHERE DV.DOKUMENTID=D.DOKUMENTID "
        Dim ANFANG$ = "SELECT   d.*, DV.VORGANGSID " + _
            " FROM DOKUMENTE D, DOKUMENT2VORGANG DV " & _
            " WHERE DV.DOKUMENTID=D.DOKUMENTID "
        Return ANFANG
    End Function

    Private Function GetSqldateityp() As String
        Dim sqldoktyp As String
        If String.IsNullOrEmpty(doktypfilter) OrElse doktypfilter.ToLower = "alle-" Then
            sqldoktyp = ""
        Else
            sqldoktyp = String.Format(" and LOWER(D.TYP) like '%{0}%'", doktypfilter.ToLower)
        End If
        Return sqldoktyp
    End Function

    Private Function GetSqldateiname() As String
        Dim sqldateiname As String
        If String.IsNullOrEmpty(doktextfilter) Then
            sqldateiname = ""
        Else
            sqldateiname = String.Format(" and lower(d.dateinameext) like '%{0}%'", doktextfilter.ToLower)
        End If
        Return sqldateiname
    End Function
    'Private Function GetSqlbeschreibung() As String
    '    Dim beschreibung As String
    '    If String.IsNullOrEmpty(doktextfilter) Then
    '        beschreibung = ""
    '    Else
    '        beschreibung = String.Format(" and lower(d.beschreibung) like '%{0}%'", doktextfilter.ToLower)
    '    End If
    '    Return beschreibung
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
End Class
