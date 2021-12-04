Public Class winAzdefine
    Private Property modus$
    Private Sub speichern(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    End Sub

    Private Sub btnGetSGnr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim nnn As New win_sgtree
        nnn.ShowDialog()
    End Sub

    Sub New(ByVal _modus$)
        modus = _modus
    End Sub
    Private Sub winAzdefine_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        comboBearbeiterInit()
        tbBearbeiter.Text = myGlobalz.sitzung.Bearbeiter.Kontakt.elektr.Fax2
        If modus.ToLower = "neu" Then
            myGlobalz.sitzung.Vorgang.Stammdaten.az.vorgangsbeschreibung = ""

        End If
    End Sub

    Function comboBearbeiterInit() As Boolean
        Try
            Dim sql$ = "select LOWER(Initial) as Initial,Name from " & "Bearbeiter" & " order by name asc"
            myGlobalz.sitzung.BearbeiterREC.dt = DBactionParadigma.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql$).Copy
            myGlobalz.sitzung.BearbeiterREC.dt.Rows.Add("alle", "alle")
            cmbUserInitial.DataContext = myGlobalz.sitzung.BearbeiterREC.dt

            'For Each ddd As DataRow In myGlobalz.sitzung.BearbeiterREC.dt.AsEnumerable
            '    Console.WriteLine(ddd.Item("Initial").ToString & " " & ddd.Item("Name").ToString)
            'Next
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                cmbUserInitial.SelectedValue = Nothing
            Else
                cmbUserInitial.SelectedValue = myGlobalz.sitzung.Bearbeiter.Initiale.ToLower
            End If

        Catch ex As Exception
            glob2.nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Function

    Private Sub cmbUserInitial_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbUserInitial.SelectionChanged
        If cmbUserInitial.SelectedValue Is Nothing Then Exit Sub
        glob2.nachricht("Ausgewählte cmbUserInitial " & cmbUserInitial.SelectedValue.ToString)
        ' aktSachgebietnr = cmbSachgebietnr.SelectedValue.ToString
        tbBearbeiter.Text = cmbUserInitial.SelectedValue.ToString


        e.Handled = True
    End Sub

    Private Sub tbSachgebiet_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbSachgebiet.TextChanged
        calcAzGesamt()
    End Sub
    Public Sub calcAzGesamt()
        myGlobalz.sitzung.Vorgang.Stammdaten.az.gesamt = ""
    End Sub
End Class
