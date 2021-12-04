Public Class SQL_Dokumente
    Property sqldateiname As String
    Property sqlbeschreibung As String
    Property sqldoktyp As String
    Property sqloriginalfullname As String = ""
    Property doktypfilter As String = ""
    Property doktextfilter As String = ""
    Property sachgebietnr As String = ""
    Property result As String
    Private Property _dbtyp$
    Public Property undMitarbeiter As Boolean = False
    Public Property inBeschreibungSuchen As Boolean = False
    Public Property inDateinamesuchen As Boolean = False
    Public Property fuerBearbeiterName As String = ""
    Public Property fuerBearbeiterId As Integer = 0

    Sub New(ByVal dbtyp$)
        _dbtyp = dbtyp
    End Sub
    Public Sub compoze()
        nachricht("in SQLerzeugenStamm  compoze-------------------------")
        Try
            Dim anfang$ = GetAnfang()
            Dim sqldateiname$ = GetSqldateiname(inDateinamesuchen)
            Dim beschreibungText = getBeschreibungText(inBeschreibungSuchen)
            Dim bearbeiterString = GetBearbeiterString(undMitarbeiter)
            Dim textsuche As String = ""
            If sqldateiname.IsNothingOrEmpty Then
                If beschreibungText.IsNothingOrEmpty Then
                    textsuche = ""
                Else
                    textsuche = " and (" & beschreibungText & ")"
                End If
            Else
                If beschreibungText.IsNothingOrEmpty Then
                    textsuche = " and (" & beschreibungText & ")"
                Else
                    textsuche = " and (" & beschreibungText & " or " & sqldateiname & ")"
                End If
            End If

            Dim sqldoktyp = GetSqldateityp()
            Dim sachgebietText = getSachgebietText()
            Dim limitstring As String = GetLimitstring(anfang$, "50")
            Dim orderstring$ = " order by nachname,vorname,gemeindename,strasse "
            orderstring = " order by checkindatum desc"
            result = anfang & textsuche & sqldoktyp & sachgebietText & sqloriginalfullname & bearbeiterString & orderstring$ & limitstring
            nachricht(String.Format("1compoze: {0}{1}", vbCrLf, result))
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in SQLerzeugen: " & ex.ToString)
        End Try
    End Sub
    Private Function GetBearbeiterString(undMitarbeit As Boolean) As String
        Dim bearbeiterString = ""
        'Dim undMitarbeit As Boolean = True
        If fuerAlleBearbeiter Then
            bearbeiterString = " "
        Else
            If String.IsNullOrEmpty(fuerBearbeiterKuerzel) Then
                If undMitarbeit Then
                    bearbeiterString = String.Format(" and (bearbeiterid={0};)) ",
                                                         fuerBearbeiterId)
                Else
                    bearbeiterString = String.Format(" and ((Bearbeiterid)={0}) ",
                                                         fuerBearbeiterId)
                End If

            Else
                If undMitarbeit Then
                    bearbeiterString = String.Format(" and (bearbeiterid={2} or lower(weiterebearb) like '%{0};%'  or lower(weiterebearb) like '%{1};%') ",
                                                         fuerBearbeiterInitiale.ToLower, fuerBearbeiterKuerzel.ToLower, fuerBearbeiterId)
                Else
                    bearbeiterString = String.Format(" and (bearbeiterid={0}) ",
                                                         fuerBearbeiterId)
                End If
            End If
        End If
        Return bearbeiterString
    End Function
    Private Function getBeschreibungText(inBeschreibungsuchen As Boolean) As String
        If Not inBeschreibungsuchen Then Return ""

        If String.IsNullOrEmpty(doktextfilter) Then
            Return ""
        Else
            Return String.Format("   lower(d.beschreibung) like '%{0}%'", doktextfilter.ToLower.Trim)
        End If

    End Function

    Private Function getSachgebietText() As String
        Dim sg As String
        If sachgebietnr = "0" Or sachgebietnr = "" Then
            Return ""
        Else
            Return String.Format(" and D.SACHGEBIETNR like '{0}'", sachgebietnr)
        End If
        Return sqldoktyp
    End Function

    Private Shared Function GetAnfang() As String
        'Dim ANFANG$ = "SELECT DISTINCT  DV.VORGANGSID, D.TYP,D.DATEINAMEEXT,D.DOKUMENTID, D.FILEDATUM,D.CHECKINDATUM,D.ORIGINALFULLNAME" + _
        '         " FROM " & CLstart.myViewsNTabs.tabdokumente & "  D, DOKUMENT2VORGANG DV " & _
        '         " WHERE DV.DOKUMENTID=D.DOKUMENTID "
        'Dim ANFANG$ = "SELECT   d.*, DV.VORGANGSID " +
        '    " FROM " & CLstart.myViewsNTabs.tabdokumente & "  D, DOKUMENT2VORGANG DV " &
        '    " WHERE DV.DOKUMENTID=D.DOKUMENTID "
        Dim ANFANG As String = "SELECT top 100   * " +
            " FROM (" & CLstart.myViewsNTabs.view_dokumentplussg & "    " &
            ") d WHERE  dokumentid>0 "
        Return ANFANG
    End Function

    Private Function GetSqldateityp() As String
        Dim sqldoktyp As String
        If String.IsNullOrEmpty(doktypfilter) OrElse doktypfilter.ToLower = "alle" Then
            sqldoktyp = ""
        Else
            sqldoktyp = String.Format(" and LOWER(D.TYP) like '%{0}%'", doktypfilter.ToLower)
        End If
        Return sqldoktyp
    End Function

    Private Function GetSqldateiname(indateinamesuchen As Boolean) As String
        If Not indateinamesuchen Then Return ""
        Dim sqldateiname As String
        If String.IsNullOrEmpty(doktextfilter) Then
            sqldateiname = ""
        Else
            sqldateiname = String.Format("   lower(d.dateinameext) like '%{0}%'", doktextfilter.ToLower.Trim)
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
