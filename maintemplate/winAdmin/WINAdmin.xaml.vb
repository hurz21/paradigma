Imports System.Data

Partial Public Class WINAdmin

    Private Sub btnWVAdmin_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        glob2.zeigeWiedervorlageAdminTabelle()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        '  initComboBearbeiter()
        'cmbgemarkung.IsDropDownOpen = True
        Title = clsStartup2.getTitle("admin") : nachricht("winBestandStammdaten_Loaded d")
        e.Handled = True
    End Sub
    'Sub initComboBearbeiter()
    '    Dim sql$ = "select username,nachName from " & CLstart.myViewsNTabs.tabBearbeiter & "  order by nachname"
    '    Dim bdt As New DataTable

    '    bdt = userTools.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql$)
    '    If bdt Is Nothing Then
    '        MsgBox("es konnten keine Bearbeiter gefunden werden")
    '    Else
    '        cmbgemarkung.DataContext = bdt
    '    End If

    'End Sub



    Private Sub btnStatistik_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("Statistikmodul:" & initP.getValue("ExterneAnwendungen.APPLICATION_STatistik"))
        Dim test As New IO.FileInfo(initP.getValue("ExterneAnwendungen.APPLICATION_STatistik"))
        If test.Exists Then
            Process.Start(initP.getValue("ExterneAnwendungen.APPLICATION_STatistik"))
        Else
            nachricht_und_Mbox("Das Statistikmodul ist nicht installiert!")
        End If
        e.Handled = True
    End Sub

    Private Sub btnWVakutAdmin_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        glob2.zeigeWiedervorlageakutAdminTabelle()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnBearbeiterListe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim bearb As New winUserList
        bearb.ShowDialog()
        Close()
        e.Handled = True
    End Sub

    Private Sub btnPulldowns_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("adjustor:" & initP.getValue("ExterneAnwendungen.APPLICATION_ParadigmaAdjustor"))
        Dim test As New IO.FileInfo(initP.getValue("ExterneAnwendungen.APPLICATION_ParadigmaAdjustor"))
        If test.Exists Then
            Process.Start(initP.getValue("ExterneAnwendungen.APPLICATION_ParadigmaAdjustor"))
        Else
            nachricht_und_Mbox("Das APPLICATION_ParadigmaAdjustor ist nicht installiert!")
        End If
        e.Handled = True
    End Sub

    Private Sub btnBearbeiterauswahl_Click_1(sender As Object, e As RoutedEventArgs)
        userTools.setzeAktuellenBearbeiter()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnNamenSuchModun_Click(sender As Object, e As RoutedEventArgs)
        Dim modus As New winNamenSuchModus
        modus.ShowDialog()
        e.Handled = True
    End Sub
End Class
