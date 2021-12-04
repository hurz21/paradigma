Imports System.Data


Public Class winProjektListe
    Property ladevorgangabgeschlossen As Boolean = False
    Sub New()
        InitializeComponent()
    End Sub
    Public aktprojekt As CLstart.clsProjektAllgemein

    Private Sub btnNeuesProjekt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        aktprojekt = New CLstart.clsProjektAllgemein(myglobalz.sitzung.aktVorgangsID)
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

    Private Sub winProjektListe_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        'alle projekte laden
        NSprojekt.ladeProjekt.alleProjekte("select * from " & CLstart.myViewsNTabs.tabPROJEKT & "    order by ts desc")
        dgProjektliste.DataContext = myglobalz.sitzung.VorgangREC.dt

        ladevorgangabgeschlossen = True
    End Sub

    Private Sub dgProjektliste_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgProjektliste.SelectionChanged
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        Dim item As DataRowView = Nothing
        Try
            item = CType(dgProjektliste.SelectedItem, DataRowView)
        Catch ex As Exception
            nachricht(ex.ToString)
        End Try
        If item Is Nothing Then Return
        e.Handled = False
        aktprojekt = New CLstart.clsProjektAllgemein(myglobalz.sitzung.aktVorgangsID)
        projektDatarowView2Obj(item)
        Try
            Me.DialogResult = True
            Close()
        Catch ex As Exception

        End Try

        'dgProjektliste.SelectedItem = Nothing
        e.Handled = False
    End Sub

    Private Sub projektDatarowView2Obj(ByVal item As DataRowView)
        Try
            With aktprojekt
                .id = CInt(clsDBtools.fieldvalue(item("ID")))
                .Kategorie1 = clsDBtools.fieldvalue(item("KATEGORIE1")).ToString
                .Kategorie2 = clsDBtools.fieldvalue(item("KATEGORIE2")).ToString
                .BezeichnungKurz = clsDBtools.fieldvalue(item("BEZEICHNUNGKURZ")).ToString
                .BezeichnungLang = clsDBtools.fieldvalue(item("BEZEICHNUNGLAN")).ToString
                .von = CDate(clsDBtools.fieldvalueDate(item("VONDATUM")).ToString)
                .bis = CDate(clsDBtools.fieldvalueDate(item("BISDATUM")).ToString)
                .TS = CDate(clsDBtools.fieldvalueDate(item("TS")).ToString)
                .Quelle = clsDBtools.fieldvalue(item("QUELLE")).ToString
                .Gemeinde = clsDBtools.fieldvalue(item("GEMEINDE")).ToString
                .WiedervorlageID = CInt(clsDBtools.fieldvalue(item("WIEDERVORLAGEID")))
                .refnr = CStr((clsDBtools.fieldvalue(item("REFNR"))))
            End With
        Catch ex As Exception
            nachricht("projektDatarowView2Obj " & vbCrLf & vbCrLf & ex.ToString)
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

        dgProjektliste.DataContext = myglobalz.sitzung.VorgangREC.dt
    End Sub

    'Private Sub dgProjektliste_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

    'End Sub

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
        dgProjektliste.DataContext = myglobalz.sitzung.VorgangREC.dt
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
        dgProjektliste.DataContext = myglobalz.sitzung.VorgangREC.dt
    End Sub
End Class
