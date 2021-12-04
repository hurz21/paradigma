Imports System.Data
Imports LibDB.LIBDB
Public Class WinWvFilter
    Property _nurZumKuckenModus As Boolean
    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
    End Sub
    Shared Sub einfaerbenDerRowsvorbereiten()
        For i = 0 To myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows.Count - 1
            Dim lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem As Object = myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("datum")
            Dim lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem1 As Date = CType(lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem, Date)
            If lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem1 < Now Then
                If CType(myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("erledigtAm"), Date) > CLstart.mycsimple.MeinNULLDatumAlsDate Then
                    myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 2
                Else
                    myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 1
                End If

            Else
                myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 0
            End If
        Next
    End Sub



    Private Sub dgWVliste_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgWVliste.SelectionChanged
        Try
            e.Handled = True
            Dim item As DataRowView = CType(dgWVliste.SelectedItem, DataRowView)
            'Dim item = dg.SelectedItem
            If item Is Nothing Then Return
            Dim auswahlid$ = item(0).ToString()
            ' nachricht_und_Mbox(auswahlid$)
            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            'Me.Close()

            Me.Close()
            glob2.editVorgang(myGlobalz.sitzung.aktVorgangsID)
            dgWVliste.SelectedItem = Nothing
            e.Handled = True
        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Private Sub NeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        clsStartup.NeuerVorgang2()
        e.Handled = True
    End Sub

    Private Sub ZuvorgangsNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim az As String = "", header As String = ""
        clsStartup.suchenNachVorgaengen(az, header)
        Close()
        e.Handled = True
    End Sub



    Private Sub WVExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim handcsv As New clsCSVausgaben("Wiedervorlagen", myGlobalz.sitzung.DBWiedervorlageREC.dt, 0)
        nachricht(" exportfile$ = " & handcsv.ausgeben())
        handcsv.start()
         handcsv.Dispose
        e.Handled = True
    End Sub

    Private Sub WinWvFilter_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.DBWiedervorlageREC.dt, "faelligSymbol", "System.Int16")
        einfaerbenDerRowsvorbereiten()
        clsDBtools.TabellenKopfausgeben(myGlobalz.sitzung.DBWiedervorlageREC.dt)
        DataContext = myGlobalz.sitzung.DBWiedervorlageREC.dt
        dgWVliste.CanUserAddRows = False
        initDGMaxHeight()
        e.Handled = True
    End Sub
    Private Sub initDGMaxHeight()
        dgWVliste.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Private Sub btnStammdatenFiltern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandStammdaten(False)
        e.Handled = True
    End Sub
    Private Sub btnBeteiligteFiltern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        clsStartup.FormularBestandBeteiligte(_nurZumKuckenModus)
        e.Handled = True
    End Sub


    Private Sub btnEreignisfilter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandEreignis(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnadrSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandAdressFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnfstSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandFlurstueckfilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnDokusuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandDokuFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub
    Private Sub btnProjektsuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandProjektFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub
End Class
