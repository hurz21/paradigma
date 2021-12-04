Imports System.Data

Public Class WinBearbeiterauswahl

    Public Property auswahlUSERNAME As String
    Public Property auswahlVorname As String
    Public Property auswahlInitiale As String
    Public Property auswahlNAchname As String
    Public Property auswahlRang As String
    Public Property auswahlKUERZEL1 As String
    Public Property auswahlBearbeiterid As Integer = 0

    Private Sub WinBearbeiterauswahl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        '  myGlobalz.sitzung.BearbeiterREC.dt' initDb()
        Dim sql As String = "select NAMENSZUSATZ,NACHNAME,VORNAME,ABTEILUNG,USERNAME,RANG,TELEFON,EMAIL,LOWER(INITIAL_) as ""INITIALE"",KUERZEL1,stdgrants,BEARBEITERID   from " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
              "where nachname<>'Gast' and aktiv=1 order by abteilung, NACHNAME asc"
        refreshListe(sql)
    End Sub

    Private Sub refreshListe(ByVal sql As String)
        Try
            dgStamm.DataContext = Nothing
            myGlobalz.sitzung.BearbeiterREC.dt = Nothing
            myGlobalz.sitzung.BearbeiterREC.dt = userTools.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql).Copy
            dgStamm.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Private Sub dgStamm_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As DataRowView
        Try
            item = CType(dgStamm.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        item = CType(dgStamm.SelectedItem, DataRowView)
        'Dim item = dgStamm.SelectedItem
        If item Is Nothing Then
            item = CType(dgStamm.SelectedItem, DataRowView)
            If item Is Nothing Then Return
        End If
        If Not item("NACHNAME").ToString().ToLower = "gast" Then
            auswahlInitiale = item("INITIALE").ToString()
            auswahlNAchname = item("NACHNAME").ToString()
            auswahlVorname = item("VORNAME").ToString()
            auswahlUSERNAME = item("USERNAME").ToString()
            auswahlRang = item("RANG").ToString()
            auswahlBearbeiterid = CInt(item("BEARBEITERID"))
            auswahlKUERZEL1 = item("KUERZEL1").ToString()
            Close()
        End If
        Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch(sender As Object, e As RoutedEventArgs)
        auswahlInitiale = ""
        Close()
        e.Handled = True
    End Sub

    Private Sub btnAlle(sender As Object, e As RoutedEventArgs)
        auswahlInitiale = "alle"
        auswahlNAchname = "alle"
        auswahlVorname = ""
        auswahlUSERNAME = ""
        Close()
        e.Handled = True
    End Sub

    Private Sub chkaktiv_Click_1(sender As Object, e As RoutedEventArgs)
        Dim sql As String
        If chkaktiv.IsChecked Then
            sql = "select NAMENSZUSATZ,NACHNAME,VORNAME,ABTEILUNG,USERNAME,RANG,TELEFON,EMAIL,LOWER(INITIAL_) as ""INITIALE"",KUERZEL1,stdgrants,BEARBEITERID  from " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
                  " where nachname<>'Gast' and aktiv=1 order by abteilung, NACHNAME asc"
            refreshListe(sql)
        Else
            sql = "select NAMENSZUSATZ,NACHNAME,VORNAME,ABTEILUNG,USERNAME,RANG,TELEFON,EMAIL,LOWER(INITIAL_) as ""INITIALE"",KUERZEL1,stdgrants,BEARBEITERID from " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
                  " where nachname<>'Gast' order by abteilung, NACHNAME asc"
            refreshListe(sql)
        End If
        e.Handled = True
    End Sub

    'Private Sub mausenter(sender As Object, e As MouseEventArgs) Handles dgStamm.MouseEnter
    '    Dim item As DataRowView
    '    Try
    '        item = CType(dgStamm.MoveFocus, DataRowView)
    '    Catch ex As Exception
    '        e.Handled = True
    '        Exit Sub
    '    End Try
    '    item = CType(dgStamm.MoveFocus, DataRowView)
    '    'Dim item = dgStamm.SelectedItem
    '    If item Is Nothing Then
    '        item = CType(dgStamm.MoveFocus, DataRowView)
    '        If item Is Nothing Then Return
    '    End If
    '    If Not item("NACHNAME").ToString().ToLower = "gast" Then
    '        auswahlInitiale = item("INITIALE").ToString()
    '        auswahlNAchname = item("NACHNAME").ToString()
    '        auswahlVorname = item("VORNAME").ToString()
    '        auswahlUSERNAME = item("USERNAME").ToString()
    '        auswahlRang = item("RANG").ToString()
    '        Close()
    '    End If

    '    e.Handled = True
    'End Sub

 
End Class
