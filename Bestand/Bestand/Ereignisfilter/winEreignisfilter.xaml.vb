Imports System.Data

Public Class winEreignisfilter
    Private datummodus As String, datumvon As String, datumbis As String
    Private _makeSQL As Boolean
    Private fuerBearbeiterName$ = "alle", fuerAlleBearbeiter As Boolean = True, fuerBearbeiterKuerzel$ = ""
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

    Private Sub btnClearEreignisse_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        tbFilterEreignisse.Text = ""
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisArt")
        cmbEreignisart.SelectedValue = ""
        tbsachgebietnr.Text = ""
        chkmitnotiz.IsChecked = False
        dgEreignisse.DataContext = ""
        e.Handled = True
        e.Handled = True
    End Sub

    Private Sub btnstartSucheEreignisse_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If String.IsNullOrEmpty(tbFilterEreignisse.Text) Then
            tbFilterEreignisse.Text = ""
            'MsgBox("Sie müssen einen Suchtext eingeben")
            'Exit Sub
        End If
        Dim art As String
        dgEreignisse.DataContext = Nothing 'tabelle_leer_machen
        If cmbEreignisart.SelectedValue Is Nothing Then
            art = ""
        Else
            art = cmbEreignisart.SelectedValue.ToString
        End If

        Dim sql As String = SQL_Ereignis.sql_4EreignisErstellen(tbFilterEreignisse.Text, art, CBool(chkmitnotiz.IsChecked),
                                                                tbsachgebietnr.Text,
                                                                datummodus, datumvon, datumbis,
                                                                CBool(chkNurProjekt.IsChecked), fuerBearbeiterName)
        myGlobalz.sitzung.VorgangREC.mydb.SQL = sql
        nachricht(myGlobalz.sitzung.VorgangREC.mydb.SQL)
        bestandTools.zeigeVorgaenge.exe()
        If myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
            dgEreignisse.DataContext = Nothing
            tbTrefferEreignis.Text = "Treffer: 0"
        Else
            Dim adrtemp As New DataTable
            adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
            dgEreignisse.DataContext = adrtemp
            tbTrefferEreignis.Text = "Treffer: " & myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString
        End If
        e.Handled = True
    End Sub

    Private Sub cmbEreignisart_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        e.Handled = True
    End Sub

    Private Sub dgEreigisse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If dgEreignisse.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgEreignisse.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        Try
            item = CType(dgEreignisse.SelectedItem, DataRowView)
            If item Is Nothing Then
                item = CType(dgEreignisse.SelectedItem, DataRowView)
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
           ' HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid$, beschreibung$, az2$)
                CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2,myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
                  LocalParameterFiles.erzeugeParameterDatei( False, False)
            e.Handled = True
            glob2.editVorgang(CInt(auswahlid))

        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
        e.Handled = True
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
    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        detailsTools.VorgangLocking("aus")
    End Sub

    Private Sub gastLayout()
        If glob2.userIstinGastModus Then
            Background = New SolidColorBrush(Colors.Red)
            stckp1.Background = New SolidColorBrush(Colors.Red)
        End If
    End Sub

    Private Sub initDGMaxHeight()
        dgEreignisse.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Private Sub winEreignisfilter_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        myGlobalz.BestandsFensterIstgeoeffnet = True
        gastLayout()
        initDGMaxHeight()
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisArt")
        tbTrefferEreignis.DataContext = Me
        comboBearbeiterInit()
        AddHandler cmbUserInitial.SelectionChanged, AddressOf cmbUserInitial_SelectionChanged_1
        bestandTools.verschiedenes.beteiligteFilternAktivieren(btnBeteiligteFiltern)
        e.Handled = True
    End Sub

    Private Sub btnfstSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandFlurstueckfilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnadrSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandAdressFilter(_nurZumKuckenModus)
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

    Private Sub btnSgtree2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim sgt As New win_sgtree(myGlobalz.Paradigma_Sachgebietsdatei)
        sgt.ShowDialog()
        CLstart.myc.AZauswahl.az.sachgebiet.Zahl = sgt.publicNR
        CLstart.myc.AZauswahl.az.sachgebiet.Header = sgt.publicsgHeader
        tbsachgebietnr.Text = CLstart.myc.AZauswahl.az.sachgebiet.Zahl.ToString
        e.Handled = True
    End Sub

    Private Sub pickVon_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        holeVonBis()
        e.Handled = True
    End Sub

    Private Sub pickBis_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        holeVonBis()
        e.Handled = True
    End Sub
    Private Function holeVonBis() As Boolean
        Try
            If pickVon.SelectedDate Is Nothing Then
                datumvon = ""
            Else
                datumvon = (Format(CDate(pickVon.SelectedDate), "dd/MM/yyyy"))

            End If
            If pickBis.SelectedDate Is Nothing Then
                datumbis = ""
            Else
                datumbis = (Format(CDate(pickBis.SelectedDate), "dd/MM/yyyy"))
            End If
            Return True
        Catch ex As System.Exception
            Return False
        End Try
    End Function
    Private Sub cmbDatumstyp_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbDatumstyp.SelectionChanged
        If cmbDatumstyp.SelectedItem Is Nothing Then Exit Sub
        ' MsgBox(cmbDatumstyp.Background.ToString)
        '  Dim a As ComboBoxItem
        datummodus = getDatumsauswahlModus()
        e.Handled = True
        Dim aktiv As Boolean = Not datummodus.Contains("kein")
        comboboxRahmenAktiveinfaerben(aktiv, cmbDatumstyp)

        If Not aktiv Then
            datumvon = "" : datumbis = ""
            Exit Sub
        End If
        datumKlarmachen(datummodus)
    End Sub
    Private Function getDatumsauswahlModus() As String
        Try
            Dim a As ComboBoxItem
            a = CType(cmbDatumstyp.SelectedItem, ComboBoxItem)
            Dim auswahl$ = a.Tag.ToString
            Return auswahl
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub datumKlarmachen(ByVal auswahl$)
        If holeVonBis() Then
        End If
    End Sub

    Private Function comboboxRahmenAktiveinfaerben(ByVal aktiv As Boolean, ByVal txb As System.Windows.Controls.ComboBox) As Boolean
        Dim myThickness As New Thickness()
        If Not aktiv Then
            txb.BorderBrush = Brushes.Black
            myThickness.Bottom = 1
            myThickness.Left = 1
            myThickness.Right = 1
            myThickness.Top = 1
            txb.BorderThickness = myThickness
        Else
            txb.BorderBrush = Brushes.Orange
            myThickness.Bottom = 9
            myThickness.Left = 3
            myThickness.Right = 3
            myThickness.Top = 3
            txb.BorderThickness = myThickness
        End If
    End Function
    Function comboBearbeiterInit() As Boolean
        Try
            Dim sql$ = "select LOWER(INITIAL_) as ""INITIALE"",NACHNAME from Bearbeiter where aktiv=1 order by abteilung, NACHNAME asc"
            myGlobalz.sitzung.BearbeiterREC.dt = userTools.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql$).Copy
            myGlobalz.sitzung.BearbeiterREC.dt.Rows.Add("alle", "alle")
            cmbUserInitial.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                cmbUserInitial.SelectedValue = "alle"
            Else
                cmbUserInitial.SelectedValue = myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower
            End If
        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Function

    Private Sub cmbUserInitial_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) ' Handles cmbUserInitial.SelectionChanged
        If cmbUserInitial.SelectedItem Is Nothing Then Exit Sub
        cmbUserChange()
        '   suchentaste()
        e.Handled = True
    End Sub
    Private Sub cmbUserChange()
        Dim item As String = CType(cmbUserInitial.SelectedValue, String)
        If item Is Nothing Then
            fuerBearbeiterName$ = ""
            fuerAlleBearbeiter = True
        Else
            fuerBearbeiterName$ = item
            If fuerBearbeiterName.ToLower = "alle" Then
                fuerAlleBearbeiter = True
            Else
                fuerAlleBearbeiter = False
            End If
        End If
    End Sub
End Class
