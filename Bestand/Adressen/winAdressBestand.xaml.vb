Imports System.Data

Public Class winAdressBestand
    Property _nurZumKuckenModus As Boolean
    Public Property auswahlid As String
    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
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
    Private Sub dgAdressen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            Dim item As DataRowView
            Try
                item = CType(dgAdressen.SelectedItem, DataRowView)
            Catch ex As Exception
                e.Handled = True
                Exit Sub
            End Try

            item = CType(dgAdressen.SelectedItem, DataRowView)
            'Dim item = dgStamm.SelectedItem
            If item Is Nothing Then
                item = CType(dgAdressen.SelectedItem, DataRowView)
                If item Is Nothing Then
                    e.Handled = True
                    Exit Sub
                End If
                Return
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
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2,
                                                     myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, 
                                                     myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)
            'myGlobalz.ClientCookieDir & "verlaufscookies" 
            'HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2,
            '                                         myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, 
            '                                         myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            e.Handled = True
            glob2.editVorgang(CInt(auswahlid))
            e.Handled = True

        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
            e.Handled = True
        End Try
    End Sub
    Private Sub btnClearAdresse_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        cmbGemeinde.SelectedValue = ""
        cmbStrasse.SelectedValue = ""
        cmbHausnr.SelectedValue = ""
        dgAdressen.DataContext = Nothing
        e.Handled = True
    End Sub

    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbGemeinde.SelectedItem Is Nothing Then Exit Sub
        gemeindechanged()
        cmbStrasse.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Sub gemeindechanged()
        setGemeindeNrNameFromCombobox()
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName
        initStrassenCombo()
        myGlobalz.sitzung.aktADR.PLZ = glob2.getPLZfromGemeinde(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName)
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PLZ = myGlobalz.sitzung.aktADR.PLZ
    End Sub

    Private Sub setGemeindeNrNameFromCombobox()
        Dim myvali$, myvals$
        myvali = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        myvals = myvalx.Attributes(1).Value.ToString
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali)
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = myvals
    End Sub

    Sub initStrassenCombo()
        'gemeindeDT
        DB_Oracle_sharedfunctions.holeStrasseDT4Vorgaenge()
        cmbStrasse.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub

    Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbStrasse.SelectedItem Is Nothing Then Exit Sub
        setStrassennameFromCombo()
        setGemeindeNrNameFromCombobox()
        initHausNRCombo()
        initErsteAdressliste()
        '  cmbHausnr.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Private Sub setStrassennameFromCombo()
        Dim item2 As DataRowView = CType(cmbStrasse.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktADR.Gisadresse.strasseName = item2.Row.ItemArray(0).ToString
    End Sub

    Sub initHausNRCombo()
        DB_Oracle_sharedfunctions.DBholeHausnrDTVorgaenge_alledb()
        cmbHausnr.DataContext = myGlobalz.sitzung.tempREC2.dt
    End Sub

    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbHausnr.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As DataRowView = CType(cmbHausnr.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        setGemeindeNrNameFromCombobox()
        setStrassennameFromCombo()
        myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = item2.Row.ItemArray(0).ToString
        dgAdressen.DataContext = Nothing
        VorgaengeAnzeigenFuerHausnummer(myGlobalz.sitzung.aktADR)
        e.Handled = True
    End Sub

    Sub VorgaengeAnzeigenFuerHausnummer(ByVal adr As ParaAdresse)
        'bestandTools.zeigeVorgaenge.VorgaengeAnzeigenFuerHausnummerExtracted()
        'Dim adrtemp As New DataTable
        'adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
        'dgAdressen.DataContext = adrtemp
        DB_Oracle_sharedfunctions.DBholeAdressenFuerDatagridHNR()
        dgAdressen.DataContext = myGlobalz.sitzung.tempREC2.dt
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
    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandWvFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub
    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        detailsTools.VorgangLocking("aus")
    End Sub

    Private Sub initDGMaxHeight()
        dgAdressen.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Private Sub gastLayout()
        If glob2.userIstinGastModus Then
            Background = New SolidColorBrush(Colors.Red)
            stckp1.Background = New SolidColorBrush(Colors.Red)
        End If
    End Sub
    Private Sub winAdressBestand_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        myGlobalz.BestandsFensterIstgeoeffnet = True
        glob2.initTemprecAusVorgangRecOracle()
        gastLayout()
        initDGMaxHeight()
        initGemeindeCombo()
        bestandTools.verschiedenes.beteiligteFilternAktivieren(btnBeteiligteFiltern)
        e.Handled = True
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemeinden.xml")
    End Sub

    Private Sub initErsteAdressliste()
        DB_Oracle_sharedfunctions.DBholeAdressenFuerDatagridStrasse()
        Dim bestAdrTemp As New DataTable ' sonst wird die DT nach gebrauch gelöscht!
        bestAdrTemp = myGlobalz.sitzung.tempREC2.dt.Copy
        dgAdressen.DataContext = bestAdrTemp
    End Sub

End Class
