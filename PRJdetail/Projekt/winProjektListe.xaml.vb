Imports System.Data

Public Class winProjektListe
    Public ladevorgangabgeschlossen As Boolean = False
    Public aktprojekt As CLstart.clsProjektAllgemein
    Private Sub winProjektListe_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'alle projekte laden
        NSprojekt.ladeProjekt.alleProjekte("select * from " & CLstart.myViewsNTabs.tabPROJEKT & "    order by ts desc")
        dgProjektliste.DataContext = myGlobalz.sitzung.VorgangREC.dt
        gastLayout()
        ladevorgangabgeschlossen = True
        e.Handled = True
    End Sub
    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub
    Private Sub btnNeuesProjekt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        aktprojekt = New CLstart.clsProjektAllgemein(myGlobalz.sitzung.aktVorgangsID)
        Dim prj As New WinProjekt("neu", aktprojekt)
        prj.ShowDialog()
        DialogResult = False
        Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = False
        Close()
        e.Handled = True
    End Sub



    Private Sub dgProjektliste_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgProjektliste.SelectionChanged
        Dim item As DataRowView = Nothing
        Try
            item = CType(dgProjektliste.SelectedItem, DataRowView)
        Catch ex As Exception
            nachricht(ex.ToString)
        End Try
        If item Is Nothing Then Return

        aktprojekt = New CLstart.clsProjektAllgemein(myGlobalz.sitzung.aktVorgangsID)
        projektDatarowView2Obj(item)
        Me.DialogResult = True
        Close()
        'dgProjektliste.SelectedItem = Nothing
        e.Handled = False
    End Sub

    Private Sub projektDatarowView2Obj(ByVal item As DataRowView)
        Try
            With aktprojekt
                .id = CInt(item("ID"))
                .Kategorie1 = item("KATEGORIE1").ToString
                .Kategorie2 = item("KATEGORIE2").ToString
                .BezeichnungKurz = item("BEZEICHNUNGKURZ").ToString
                .BezeichnungLang = item("BEZEICHNUNGLAN").ToString
                .von = CDate(item("VONDATUM").ToString)
                .bis = CDate(item("BISDATUM").ToString)
                .TS = CDate(item("TS").ToString)
                .Quelle = item("QUELLE").ToString
                .Gemeinde = item("GEMEINDE").ToString
                .WiedervorlageID = CInt(item("WIEDERVORLAGEID"))
            End With
        Catch ex As Exception
            nachricht("projektDatarowView2Obj " & vbCrLf & vbCrLf ,ex)
        End Try
    End Sub

    Public Sub New(ByVal vorgangsid As Integer)
        InitializeComponent()
        If vorgangsid > 0 Then
            btnNeuesProjekt.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub btnRefreshPrj_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If tbfilter.Text.Trim.Count > 2 Then
            Dim where, filter As String
            filter = tbfilter.Text.Trim.ToLower
            where = " where lower(bezeichnungkurz) like '%" & filter & "%' or  bezeichnunglan like '%" & filter & "%'"
            NSprojekt.ladeProjekt.alleProjekte("select * from " & CLstart.myViewsNTabs.tabPROJEKT & "    " & where & " order by ts desc")
        Else
            NSprojekt.ladeProjekt.alleProjekte("select * from " & CLstart.myViewsNTabs.tabPROJEKT & "    order by ts desc")
        End If

        dgProjektliste.DataContext = myGlobalz.sitzung.VorgangREC.dt

    End Sub
    Private Sub cmbObergruppe_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        Dim auswahl As ComboBoxItem
        auswahl = CType(cmbObergruppe.SelectedItem, ComboBoxItem)
        Dim where As String = ""

        where = " where kategorie1 = '" & CStr(auswahl.Tag) & "'"
        If CStr(auswahl.Tag).IsNothingOrEmpty Then
            where = ""
        End If

        NSprojekt.ladeProjekt.alleProjekte("select * from " & CLstart.myViewsNTabs.tabPROJEKT & "    " & where & " order by ts desc")
        dgProjektliste.DataContext = myGlobalz.sitzung.VorgangREC.dt
    End Sub

    Private Sub cmbUntergruppe_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        Dim auswahl As ComboBoxItem
        auswahl = CType(cmbUntergruppe.SelectedItem, ComboBoxItem)
        Dim where As String = ""

        where = " where kategorie2 = '" & CStr(auswahl.Tag) & "'"
        If CStr(auswahl.Tag).IsNothingOrEmpty Then
            where = ""
        End If
        NSprojekt.ladeProjekt.alleProjekte("select * from " & CLstart.myViewsNTabs.tabPROJEKT & "    " & where & " order by ts desc")
        dgProjektliste.DataContext = myGlobalz.sitzung.VorgangREC.dt
    End Sub
End Class
