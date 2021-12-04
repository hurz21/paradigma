Imports System.Data

Public Class WinBearbeiterauswahl

    Public Property auswahlUSERNAME As String
    Public Property auswahlVorname As String
    Public Property auswahlInitiale As String
    Public Property auswahlNAchname As String
    Public Property auswahlRang As String
    Public Property _modus As String = "einzelauswahl"
    Public Property mehrfachauswahlsumme As String = ""

    Public Property auswahlKUERZEL1 As String
    Public Property auswahlBearbeiterid As Integer = 0
    Public Property _VIDgruppentext As String
    Property bearbeiterListe As New List(Of clsBearbeiter)

    Sub New(modus As String, Optional VIDgruppentext As String = "")
        InitializeComponent()
        _modus = modus
        _VIDgruppentext = VIDgruppentext
    End Sub

    Private Property selectstring As String

    Private Sub WinBearbeiterauswahl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        '  myGlobalz.sitzung.BearbeiterREC.dt' initDb()
        'Dim sql As String = "select id,NAMENSZUSATZ,NACHNAME,VORNAME,ABTEILUNG,USERNAME,RANG,TELEFON,EMAIL,LOWER(INITIAL_) as ""INITIALE"",KUERZEL1, " &
        '                    " id as BEARBEITERID " &
        '                    "from " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
        '                     "where nachname<>'Gast' and aktiv=1 order by abteilung, NACHNAME asc"
        auswahlBearbeiterid = myGlobalz.sitzung.aktBearbeiter.ID
        selectstring = "select *  "
        Dim sql As String = selectstring &
                    "from " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
                     "where nachname<>'Gast' and aktiv=1 order by abteilung, NACHNAME asc"


        refreshListe(sql)
        If _modus = "mehrfachauswahlInitial" Then
            auswahlspalteDokus.Visibility = Windows.Visibility.Visible
            btnAlle.Visibility = Visibility.Collapsed
            btnMehrfachauswahlFertig.Visibility = Visibility.Visible
        End If
        If _modus = "mehrfachauswahl" Then
            auswahlspalteDokus.Visibility = Windows.Visibility.Visible
            btnAlle.Visibility = Visibility.Collapsed
            btnMehrfachauswahlFertig.Visibility = Visibility.Visible
        End If
        If _modus = "einzelauswahl" Then
            btnMehrfachauswahlFertig.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub refreshListe(ByVal sql As String)
        Try
            Dim hinweis As String = ""
            dgStamm.DataContext = Nothing
            myglobalz.sitzung.BearbeiterREC.dt = Nothing
            myglobalz.sitzung.BearbeiterREC.dt = getDT4Query(sql, myglobalz.sitzung.BearbeiterREC, hinweis)
            bearbeiterObjektlisteErstellen(myglobalz.sitzung.BearbeiterREC.dt)
            objektlisteBearbeiten(_VIDgruppentext)
            dgStamm.DataContext = bearbeiterListe
        Catch ex As Exception
            nachricht_und_Mbox("" ,ex)
        End Try
    End Sub

    Private Sub dgStamm_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As clsBearbeiter
        Try
            item = CType(dgStamm.SelectedItem, clsBearbeiter)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        item = CType(dgStamm.SelectedItem, clsBearbeiter)
        If item Is Nothing Then Return
        If _modus.StartsWith("mehrfachauswahl") Then
            e.Handled = True
            Exit Sub
        End If
        If _modus = "einzelauswahl" Then
            If Not item.Name.ToLower = "gast" Then
                auswahlInitiale = item.Initiale
                auswahlNAchname = item.Name
                auswahlVorname = item.Vorname
                auswahlUSERNAME = item.username
                auswahlRang = item.Rang
                auswahlKUERZEL1 = item.Kuerzel2Stellig
                auswahlBearbeiterid = item.ID
                Close()
            End If
        End If
        Close()
        e.Handled = True
    End Sub

    Private Sub chkaktiv_Click_1(sender As Object, e As RoutedEventArgs)
        Dim sql As String
        selectstring = "select NAMENSZUSATZ,NACHNAME,VORNAME,ABTEILUNG,USERNAME,RANG,TELEFON,EMAIL,LOWER(INITIAL_) as ""INITIALE"",KUERZEL1 "
        selectstring = "select * "
        If chkaktiv.IsChecked Then

            sql = selectstring &
                 " from " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
                  " where nachname<>'Gast' and aktiv=1 order by abteilung, NACHNAME asc"
            refreshListe(sql)
        Else
            sql = selectstring &
             " from " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
              " where nachname<>'Gast' order by abteilung, NACHNAME asc"
            refreshListe(sql)
        End If
        e.Handled = True
    End Sub

    Private Sub btnMehrfachauswahlFertigClick(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If _modus = "mehrfachauswahl" Then
            For Each bearb In bearbeiterListe
                If bearb.istausgewaehlt Then
                    mehrfachauswahlsumme = mehrfachauswahlsumme & bearb.username & trenn
                End If
            Next
        End If
        If _modus = "mehrfachauswahlInitial" Then
            trenn = ";"
            For Each bearb In bearbeiterListe
                If bearb.istausgewaehlt Then
                    mehrfachauswahlsumme = mehrfachauswahlsumme & bearb.Initiale & trenn
                End If
            Next
        End If
    End Sub


    'Private Sub btnMehrfachauswahlFertigClick(sender As Object, e As RoutedEventArgs)
    '    If _modus = "mehrfachauswahl" Then
    '        For Each zeile In dgStamm.Items
    '            Dim a = CType(dgStamm.Columns(0).GetCellContent(zeile), System.Windows.Controls.CheckBox)
    '            Debug.Print(CStr(a.IsChecked))

    '            Dim b = CType(dgStamm.Columns(8).GetCellContent(zeile), System.Windows.Controls.TextBlock)
    '            If a.IsChecked Then
    '                mehrfachauswahlsumme = mehrfachauswahlsumme & b.Text & trenn
    '            End If
    '            Debug.Print(b.Text)
    '        Next
    '    End If
    '    e.Handled = True
    'End Sub

    Private Sub btnAlleclick(sender As Object, e As RoutedEventArgs)
        If _modus = "einzelauswahl" Then
            auswahlInitiale = "alle"
            auswahlNAchname = "alle"
            auswahlVorname = ""
            Close()
            e.Handled = True
        End If
    End Sub

    Private Sub bearbeiterObjektlisteErstellen(dt As DataTable)
        Dim neub As New clsBearbeiter
        For I = 0 To dt.Rows.Count - 1
            neub = New clsBearbeiter
            userTools.bearbeiterDTzuOBJEKT(neub, dt, I)
            bearbeiterListe.Add(neub)
        Next
    End Sub

    Private Sub objektlisteBearbeiten(vidGruppenstring As String)
        For Each bearb As clsBearbeiter In bearbeiterListe
            If vidGruppenstring.ToLower.Trim.Contains(bearb.username.ToLower.Trim) Then
                bearb.istausgewaehlt = True
            Else
                bearb.istausgewaehlt = False
            End If
            bearb.ImageFilePath = "C:\Users\Feinen_j\Pictures\" & bearb.username & ".jpg"
        Next
    End Sub
    Private Sub btnAbbruch(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        auswahlInitiale = ""
        If _modus.StartsWith("mehrfachauswahl") Then
            mehrfachauswahlsumme = "####"
        End If
        Close()
    End Sub

    Private Sub dgStamm_MouseEnter(sender As Object, e As MouseEventArgs)
        'Debug.Print(sender.ToString)
        'Dim item = CType(sender, clsBearbeiter)
        '  item = CType(dgStamm.SelectedItem, clsBearbeiter)
    End Sub
End Class
