Imports System.Data
Imports LibDB.LIBDB
Public Class winfilterProjekte
    Property _nurZumKuckenModus As Boolean
    Public Property auswahlid As String
    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
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

    Private Sub btnProjektauswahl_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        NSprojekt.Projekt_holeprojektnummer.ProjektlisteAnzeigen(dgProjekte)
        tbVorgangsProjekteTreffer.Text = myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString
        e.Handled = True
    End Sub

    Private Sub NeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        CLstart.mycSimple.neuerVorgang3("modus=normal")
        e.Handled = True
    End Sub

    Private Sub ZuvorgangsNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim az As String = "", header As String = ""
        clsStartup.suchenNachVorgaengen(az, header)
        Close()
        e.Handled = True
    End Sub

    Private Sub dgProjekte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If dgProjekte.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgProjekte.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        Try
            item = CType(dgProjekte.SelectedItem, DataRowView)
            If item Is Nothing Then
                item = CType(dgProjekte.SelectedItem, DataRowView)
                If item Is Nothing Then Return
            End If
            If _nurZumKuckenModus Then
                myGlobalz.sitzung.BestandsAuswahlVID = CInt(clsDBtools.fieldvalue(item("vorgangsid")))
                e.Handled = True
                Close()
                Exit Sub
            End If


            glob3.allAktobjReset.execute(myGlobalz.sitzung)
            Dim auswahlid$ = item("vorgangsid").ToString()
            Dim beschreibung$ = item("BESCHREIBUNG").ToString()
            Dim az2$ = item("AZ2").ToString()
            'HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid$, beschreibung$, az2$)
              CLstart.  HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2,myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
                  LocalParameterFiles.erzeugeParameterDatei( False, False)
            glob2.editVorgang(CInt(auswahlid), myglobalz.testmode)

            e.Handled = True
        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Private Sub initDGMaxHeight()
        dgProjekte.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Private Sub winfilterProjekte_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
             myGlobalz.BestandsFensterIstgeoeffnet = True
        'gastLayout()
        initDGMaxHeight()
         bestandTools.verschiedenes.beteiligteFilternAktivieren(btnBeteiligteFiltern)
        e.Handled = True
    End Sub

    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
             myGlobalz.BestandsFensterIstgeoeffnet = False
        detailsTools.VorgangLocking("aus")
    End Sub

    'Private Sub gastLayout()
    '    If glob2.userIstinGastModus Then
    '        Background = New SolidColorBrush(Colors.Red)
    '        stckp1.Background = New SolidColorBrush(Colors.Red)
    '    End If
    'End Sub

    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandWvFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub
End Class
