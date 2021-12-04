Imports System.Data
Imports System.ComponentModel
Imports LibDB.LIBDB

Public Class winBestandStammdaten
    Implements INotifyPropertyChanged
    Public filterBearbeiterInitiale As String = ""
    Private text_alle As String = "ALLE-"
    Public odlsel$
    Private datummodus As String, datumvon As String, datumbis As String
    Private Property geladen As Boolean
    Public anychange As Boolean
    Private Property LIMIT_NR As String = "50" 'anzahl der zeilen die in der tab dargestellt werden sollen (vgl. limit in mysql)
    Private mittextfilter As Boolean
    Private fuerBearbeiterName$ = "alle", fuerAlleBearbeiter As Boolean = True, fuerBearbeiterKuerzel$ = ""
    Private erledigteauswahl$ = "beides"
    Private _makeSQL As Boolean
    Private _treffer As Integer
    Private aktSachgebietnr As String = text_alle, aktGMZ$ = text_alle
    Property _nurZumKuckenModus As Boolean = False

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
 Implements INotifyPropertyChanged.PropertyChanged

    Public Property auswahlid As String

    'Private Property indexVorgangsid As Integer = 0

    Protected Sub OnPropertyChanget(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Public Property Treffer() As Integer
        Get
            Return _treffer
        End Get
        Set(ByVal Value As Integer)
            _treffer = Value
            OnPropertyChanget("Treffer")
        End Set
    End Property

    Sub New(ByVal makeSQL As Boolean, nurzumkuckenmodus As Boolean)
        InitializeComponent()
        _makeSQL = makeSQL
        _nurZumKuckenModus = nurzumkuckenmodus
    End Sub

    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        detailsTools.VorgangLocking("aus")
    End Sub



    Private Sub winBestandStammdaten_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        myGlobalz.BestandsFensterIstgeoeffnet = True
        gastLayout()
        initDGMaxHeight()
        initLimitCombo()
        initErledigtCombo()
        iniitBearbeiter()
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisArt")
        glob2.initGemKRZCombo(Me)
        ' refresh_WINvorgaengeListe(_makeSQL, "50") ' eig. überflüssig, weil aber die events unten 
        ''                                           manchmal nicht feuern, muss es bleiben sonst ist die Liste Leer
        'initSachgebietnrCombo(myGlobalz.sitzung.VorgangREC.dt) 'ueberflüssig
        '  init_WINVORGAENGECombos()

        tbTreffer.DataContext = Me
        aktSachgebietnr = text_alle
        aktGMZ = text_alle
        geladen = True
        'System.Threading.Thread.Sleep(900)
        'AddHandler cmbSachgebietnr.SelectionChanged, AddressOf cmbSachgebietnr_SelectionChanged
        'AddHandler cmbLimit.SelectionChanged, AddressOf cmbLimit_SelectionChanged
        AddHandler cmbErledigt.SelectionChanged, AddressOf cmbErledigt_SelectionChanged_2
        'AddHandler tbsachgebietnr.TextChanged, AddressOf tbsachgebietnr_TextChanged
        '    AddHandler cmbUserInitial.SelectionChanged, AddressOf cmbUserInitial_SelectionChanged_1
        setTitle()
        changeDatagrid()

        setzeBearbeiterText(myGlobalz.sitzung.aktBearbeiter.Name, myGlobalz.sitzung.aktBearbeiter.Vorname)
        suchentaste()
        bestandTools.verschiedenes.beteiligteFilternAktivieren(btnBeteiligteFiltern)
        e.Handled = True
    End Sub


    Private Sub gastLayout()
        If glob2.userIstinGastModus Then
            Background = New SolidColorBrush(Colors.Red)
            stckp1.Background = New SolidColorBrush(Colors.Red)
        End If
    End Sub
    Private Sub abbruchclick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnBestandtoExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        bestandTools.btnBestandtoExcel_ClickExtracted.exe()
        e.Handled = True
    End Sub
    Private Sub btnSgtree2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim sgt As New win_sgtree(myGlobalz.Paradigma_Sachgebietsdatei)
        sgt.ShowDialog()

        If sgt.publicNR IsNot Nothing Then
            CLstart.myc.AZauswahl.az.sachgebiet.Zahl = sgt.publicNR
            CLstart.myc.AZauswahl.az.sachgebiet.Header = sgt.publicsgHeader
            tbsachgebietnr.Text = CLstart.myc.AZauswahl.az.sachgebiet.Zahl.ToString
        End If
        sgt = Nothing
        GC.Collect()
        dgStammListeClear()
        ' suchentaste()
        e.Handled = True
    End Sub

    Private Sub suchentaste()
        If Not geladen Then Exit Sub
        dgStammListeClear()
        myGlobalz.sitzung.DBWiedervorlageREC.dt.Clear()

        mittextfilter = If(String.IsNullOrEmpty(tbFilter.Text), False, True)
        Dim trefferzahl As Integer = refresh_WINvorgaengeListe(True, LIMIT_NR)
        If trefferzahl < 1 Then
            MsgBox("Ggf. haben Sie zuviele Einschränkungen verwendet. " & Environment.NewLine &
                   "Tipp: Setzen Sie 'Bearbeiter' auf 'alle' !", MsgBoxStyle.Information, "Keine Treffer")
        End If
        trefferdarstellung(trefferzahl)
        GC.Collect()
    End Sub


    Private Sub trefferdarstellung(ByVal trefferzahl As Integer)
        Try
            If LIMIT_NR = "alle" Then
                tbTreffer.Foreground = Brushes.Black
                tbTreffer.ToolTip = "Anzahl der Treffer"
            Else
                If trefferzahl >= CInt(LIMIT_NR) Then
                    tbTreffer.Foreground = Brushes.Red
                    tbTreffer.ToolTip = "Rot = Es gibt noch mehr als " & LIMIT_NR & " Treffer (" & trefferzahl & "). Wählen Sie ggf. unter 'max. Treffer' eine größere Zahl."
                Else
                    tbTreffer.Foreground = Brushes.Black
                    tbTreffer.ToolTip = "Schwarz = Es sind  weniger als " & LIMIT_NR & " Treffer zu diesem Filter gefunden worden."
                End If
            End If
        Catch ex As Exception
            MsgBox("Fehler in der trefferdarstellung: " & vbCrLf & "Bitte den Paradigma-Admin informieren!")
        End Try
    End Sub
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
        dgStammListeClear()
        e.Handled = True
    End Sub

    Function refresh_WINvorgaengeListe(ByVal makesqlstring As Boolean, ByVal rownum As String) As Integer
        ' rownum = "alle"

        Try
            ' fuerBearbeiterKuerzel = myGlobalz.sitzung.Bearbeiter.Kuerzel2Stellig
            fuerBearbeiterKuerzel = getKuerzelForInitial(fuerBearbeiterName) 'sonst wirds verfälscht
            If tbFilter.Text.IsNothingOrEmpty OrElse tbFilter.Text.Length < 3 Then
                chkboxmitBeteiligtensuche.IsChecked = False
                chkboxmitEreignissuche.IsChecked = False
            End If
            'indexVorgangsid = 1
            If makesqlstring Then
                'indexVorgangsid = 0
                Dim compozeStammSQL As New Paradigma_bestandssuche.SQL_Stamm(myGlobalz.vorgang_MYDB.dbtyp) With
                                                {.GEMKRZ_selitem = cmbGemKRZ.SelectedItem,
                                                 .GEMKRZ_selval = cmbGemKRZ.SelectedValue,
                                                 .text_alle = text_alle,
                                                 .tbsachgebietnr_Text = tbsachgebietnr.Text,
                                                 .LIMIT_NR = LIMIT_NR,
                                                 .fuerAlleBearbeiter = fuerAlleBearbeiter,
                                                 .fuerBearbeiterInitiale = fuerBearbeiterName,
                                                 .fuerBearbeiterKuerzel = fuerBearbeiterKuerzel,
                                                 .erledigteauswahl = erledigteauswahl,
                                                 .mittextfilter = mittextfilter,
                                                 .tbFilter_Text = tbFilter.Text,
                                                 .datummodus = datummodus,
                                                 .datumvon = datumvon,
                                                 .datumbis = datumbis,
                                                 .kosten_ersatzgeld = CBool(ersatzgeld.IsChecked),
                                                .kosten_ersatzgeld_bezahlt = CBool(ersatzgeldBezahlt.IsChecked),
                                                .kosten_sicherheit = CBool(sicherheit.IsChecked),
                                                .kosten_sicherheit_bezahlt = CBool(sicherheitBezahlt.IsChecked),
                                                .kosten_verwaltungsgebuehr = CBool(chkboxverwaltungsgebuehr.IsChecked),
                                                .kosten_verwaltungsgebuehr_bezahlt = CBool(verwaltungsgebuehrBezahlt.IsChecked),
                                                .kosten_interneZahlung = CBool(chkboxInterneZahlung.IsChecked),
                                                .kosten_Verknuepfung = getKostenverknuepfung(),
                                                .kosten_VERWARNUNGSGELD = CBool(VERWARNUNGSGELD.IsChecked),
                                                .kosten_VERWARNUNGSGELD_bezahlt = CBool(VerwarnungsgeldBezahlt.IsChecked),
                                                .kosten_bussGELD = CBool(BUSSGELD.IsChecked),
                                                .kosten_bussGELD_bezahlt = CBool(BUSSGELDBezahlt.IsChecked),
                                                .stellungnahmeerfolgt = CBool(chkboxStellungnahme.IsChecked),
                                                 .beteiligtenSuchen = CBool(chkboxmitBeteiligtensuche.IsChecked),
                                                 .ereignisSuchen = CBool(chkboxmitEreignissuche.IsChecked)
                                                  }

                Dim temp As String = ""
                myGlobalz.sitzung.VorgangREC.mydb.SQL = ""
                If Not bestandTools.verschiedenes.mitZusatzSuche(CBool(chkboxmitBeteiligtensuche.IsChecked), CBool(chkboxmitEreignissuche.IsChecked)) Then
                    compozeStammSQL.compozeStamm(rownum, "")
                    temp = compozeStammSQL.result
                    myGlobalz.sitzung.VorgangREC.mydb.SQL = temp
                Else
                    compozeStammSQL.compozeStamm(rownum, "")
                    temp = compozeStammSQL.result
                    myGlobalz.sitzung.VorgangREC.mydb.SQL = " (" & temp & " ) "

                    If CBool(chkboxmitBeteiligtensuche.IsChecked) Then
                        compozeStammSQL.compozeStamm(rownum, "beteiligten")
                        temp = compozeStammSQL.result
                        myGlobalz.sitzung.VorgangREC.mydb.SQL = myGlobalz.sitzung.VorgangREC.mydb.SQL & " UNION " & " (" & temp & " ) "
                    End If
                    If CBool(chkboxmitEreignissuche.IsChecked) Then
                        compozeStammSQL.compozeStamm(rownum, "ereignis")
                        temp = compozeStammSQL.result
                        myGlobalz.sitzung.VorgangREC.mydb.SQL = myGlobalz.sitzung.VorgangREC.mydb.SQL & " UNION " & " (" & temp & " ) "
                    End If
                    'union
                End If
                compozeStammSQL = Nothing
            Else
                nachricht("BESTAND:   " & myGlobalz.sitzung.VorgangREC.mydb.SQL)
            End If


            Dim innersql As String = " select * from vsk_d  where vorgangsid IN (" &
                                                    myGlobalz.sitzung.VorgangREC.mydb.SQL & ") "
            Dim pageAnfang As Integer = 0
            Dim pageEnde As Integer = getAnzahlGewuenschterRecs(LIMIT_NR)
            Dim huelle As String
            Dim orderstring As String = " order by LetzteBearbeitung desc "
            huelle = "   select *   from ( select /*+ FIRST_ROWS(n) */ " &
                            " a.*, ROWNUM rnum " &
                            " from ( inner ) a " &
                            " where ROWNUM <= " &
                            " pageEnde ) " &
                            " where rnum  >= pageAnfang" &
                            " orderstring"

            huelle = huelle.Replace("pageAnfang", pageAnfang.ToString)
            huelle = huelle.Replace("pageEnde", pageEnde.ToString)
            huelle = huelle.Replace("orderstring", orderstring.ToString)
            huelle = huelle.Replace("inner", innersql.ToString)

            'myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from vsk_d  where vorgangsid IN(" &
            '                                        myGlobalz.sitzung.VorgangREC.mydb.SQL &
            '                                        ")" & " order by LetzteBearbeitung desc"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = huelle
            myGlobalz.sitzung.VorgangREC.getDataDT()
            Treffer = myGlobalz.sitzung.VorgangREC.dt.Rows.Count

            Dim faelligeWV() As Integer = Nothing
            Dim Instring As String = bestandTools.vorgangidListe.fuelleFaelligeMitAllenVorgaengen(myGlobalz.sitzung.VorgangREC.dt, faelligeWV).Trim

            clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.VorgangREC.dt, "WVFAELLIG", "System.Int16")
            clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.VorgangREC.dt, "WVFAELLIG", 0)

            Array.Sort(faelligeWV)
            bestandTools.holeWVfuerVorgangsids.exe(myGlobalz.sitzung.DBWiedervorlageREC, myGlobalz.wiedervorlage_MYDB)
            bestandTools.holeWVfuerVorgangsids.korrigiereFaelligenArray(faelligeWV, myGlobalz.sitzung.DBWiedervorlageREC.dt)
            bestandTools.setzeWVfaelligTag.istFealligMarkierenAlsEins(faelligeWV, myGlobalz.sitzung.VorgangREC.dt)

            Dim adrtemp As New DataTable
            adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
            dgStamm.DataContext = adrtemp
            ' dgStamm.DataContext =nothing   

            dgStamm.CanUserAddRows = False       'verhindert eine reihe von Fehlermwldungen  
            ListeInsGIS.Visibility = Windows.Visibility.Visible
            dgStamm.Visibility = Windows.Visibility.Visible
            Return Treffer
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in refresh: " & ex.ToString)
        End Try
    End Function

    Private Sub cmbErledigt_SelectionChangedExtracted()
        Try
            If cmbErledigt.SelectedItem Is Nothing Then Exit Sub
            If cmbErledigt.SelectedItem.ToString = "erledigte" Then erledigteauswahl = "erledigte"
            If cmbErledigt.SelectedItem.ToString = "unerledigte" Then erledigteauswahl = "unerledigte"
            If cmbErledigt.SelectedItem.ToString = "beides" Then erledigteauswahl = "beides"
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in cmbErledigt_SelectionChanged: " & ex.ToString)
        End Try
    End Sub


    Private Sub SetLIMIT_NR()
        Dim selob As New KeyValuePair(Of String, String)
        selob = CType(cmbLimit.SelectedItem, KeyValuePair(Of String, String))
        LIMIT_NR$ = selob.Key
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


    Private Sub pickVon_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        holeVonBis()
        e.Handled = True
    End Sub

    Private Sub pickBis_SelectedDateChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        holeVonBis()
        e.Handled = True
    End Sub

    Private Sub tbFilter_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFilter.TextChanged
        If tbFilter Is Nothing Then Exit Sub
        Dim aktiv As Boolean = Not String.IsNullOrEmpty(tbFilter.Text)
        textboxRahmenAktiveinfaerben(aktiv, tbFilter)
        If tbFilter.Text.IsNothingOrEmpty Then
            chkboxmitBeteiligtensuche.IsEnabled = False
            chkboxmitEreignissuche.IsEnabled = False
            chkboxmitBeteiligtensuche.IsChecked = False
        Else
            If tbFilter.Text.Trim.Length > 2 Then
                chkboxmitBeteiligtensuche.IsEnabled = True
                chkboxmitEreignissuche.IsEnabled = True
                chkboxmitBeteiligtensuche.IsChecked = True
                stckBZusatz.IsEnabled = True
            End If
        End If
        dgStammListeClear()
        e.Handled = True
    End Sub


    Private Sub cmbGemKRZ_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbGemKRZ.SelectedItem Is Nothing Then Exit Sub
        Dim aktiv As Boolean = Not cmbGemKRZ.SelectedValue.ToString = "ALLE-"
        comboboxRahmenAktiveinfaerben(aktiv, cmbGemKRZ)
        dgStammListeClear()
        ' suchentaste()
        'e.Handled = True
    End Sub



    Private Function textboxRahmenAktiveinfaerben(ByVal aktiv As Boolean, ByVal txb As System.Windows.Controls.TextBox) As Boolean
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
            myThickness.Bottom = 3
            myThickness.Left = 3
            myThickness.Right = 3
            myThickness.Top = 3
            txb.BorderThickness = myThickness
        End If
    End Function
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

    Private Sub dgStamm_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgStamm.SelectionChanged
        Try
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

            If _nurZumKuckenModus Then
                myGlobalz.sitzung.BestandsAuswahlVID = CInt(clsDBtools.fieldvalue(item("vorgangsid")))
                e.Handled = True
                Close()
                Exit Sub
            End If
            glob3.allAktobjReset.execute(myGlobalz.sitzung)

            auswahlid = (clsDBtools.fieldvalue(item("vorgangsid")))
            Dim beschreibung As String = item("BESCHREIBUNG").ToString()
            Dim az2 As String = item("AZ2").ToString()
            'HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2)
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2, myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)


            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)

            e.Handled = True
            glob2.editVorgang(CInt(auswahlid))

        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub




    Private Sub ListeInsGIS_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        LocalParameterFiles.erzeugeVorgangsListenDatei(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC.dt)
        glob2.neueKarteerstellen()
        e.Handled = True
    End Sub

    Private Function getKuerzelForInitial(ByVal Initiale As String) As String
        If String.IsNullOrEmpty(Initiale.ToLower) Then Return ""
        If Initiale.ToLower = "alle" Then Return ""
        Dim testbearbeiter As New clsBearbeiter
        If userTools.initBearbeiterByUserid_ausParadigmadb(testbearbeiter, "INITIAL_", Initiale) Then
            Return testbearbeiter.Kuerzel2Stellig
        Else
            Return ""
        End If
    End Function

    Private Sub NeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        clsStartup.NeuerVorgang2()
        e.Handled = True
    End Sub

    Private Sub ZuvorgangsNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim az$ = "", header$ = ""
        clsStartup.suchenNachVorgaengen(az, header)
        Close()
        e.Handled = True
    End Sub




    Private Sub cmbUserChange(auswahlBearbeiter As String)
        Dim item As String = CType(auswahlBearbeiter, String)
        If item Is Nothing Then
            fuerBearbeiterName = ""
            fuerAlleBearbeiter = True
        Else
            fuerBearbeiterName = item
            If fuerBearbeiterName.ToLower = "alle" Then
                fuerAlleBearbeiter = True
            Else
                fuerAlleBearbeiter = False
            End If
        End If
    End Sub


    Private Sub cmbErledigt_SelectionChanged_2(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) 'Handles cmbErledigt.SelectionChanged
        If Not IsLoaded Then Exit Sub
        If cmbErledigt.SelectedItem Is Nothing Then Exit Sub

        cmbErledigt_SelectionChangedExtracted()
        ' suchentaste()
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub cmbLimit_Loaded(sender As Object, e As RoutedEventArgs) Handles cmbLimit.Loaded

    End Sub

    Private Sub cmbLimit_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbLimit.SelectionChanged
        If cmbLimit.SelectedItem Is Nothing Then Exit Sub
        SetLIMIT_NR()
        '  suchentaste()
        dgStammListeClear()
        e.Handled = True
    End Sub

    'Private Sub cmbSachgebietnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) ' Handles cmbSachgebietnr.SelectionChanged
    '    If cmbSachgebietnr.SelectedValue Is Nothing Then Exit Sub
    '    nachricht("Ausgewählte Sachgebietsnr" & cmbSachgebietnr.SelectedValue.ToString)
    '    tbsachgebietnr.Text = cmbSachgebietnr.SelectedValue.ToString
    '    '  suchentaste()
    '    e.Handled = True
    'End Sub

    Sub initSachgebietnrCombo(meindt As DataTable) 'myGlobalz.sitzung.VorgangREC.dt
        Try
            cmbSachgebietnr.Items.Clear()
            Dim results = From datar As DataRow In meindt.AsEnumerable
                          Order By datar.Field(Of String)("Sachgebietnr")
                          Select datar.Field(Of String)("Sachgebietnr") Distinct
            cmbSachgebietnr.Items.Add(text_alle)
            For Each strra As String In results
                cmbSachgebietnr.Items.Add(strra.ToString)
            Next
            cmbSachgebietnr.SelectedValue = aktSachgebietnr
        Catch ex As Exception
            nachricht("fehler in in initSachgebietnrCombo: ", ex)
        End Try
    End Sub

    Private Sub initErledigtCombo()
        cmbErledigt.Items.Add("unerledigte")
        cmbErledigt.Items.Add("erledigte")
        cmbErledigt.Items.Add("beides")
        cmbErledigt.SelectedValue = If(clsParadigmaRechte.istUser_admin_oder_vorzimmer(), "beides", "beides")
    End Sub

    Private Sub initDGMaxHeight()
        dgStamm.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Private Sub initLimitCombo()
        Dim limit As New Dictionary(Of String, String)
        limit.Add("50", "50 Treffer")
        limit.Add("100", "100 Treffer")
        limit.Add("500", "500 Treffer")
        limit.Add("1000", "1000 Treffer")
        limit.Add("alle", "alle Treffer")
        cmbLimit.ItemsSource = limit
        cmbLimit.SelectedIndex = If(clsParadigmaRechte.istUser_admin_oder_vorzimmer(), 1, 0)
        LIMIT_NR = If(clsParadigmaRechte.istUser_admin_oder_vorzimmer(), "100", "50")
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

    Private Sub btnProjektsuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandProjektFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnDokusuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandDokuFilter(_nurZumKuckenModus)
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
    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandWvFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnNatureg_Click_1(sender As Object, e As RoutedEventArgs)
        Close()
        clsStartup.FormularBestandNaturegFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub changeDatagrid()
        changeDG4abteilungen()
    End Sub

    Private Sub changeDG4abteilungen()
        If myGlobalz.sitzung.aktBearbeiter.Bemerkung = "Immissionsschutz" Or
            myGlobalz.sitzung.aktBearbeiter.Bemerkung = "Immisionsschutz" Or
            myGlobalz.sitzung.aktBearbeiter.Bemerkung = "Schornsteinfegerwesen" Then
            'If myGlobalz.sitzung.Bearbeiter.Bemerkung = "Graphische Datenverarbeitung" Then
            '   Dim index = dgStamm.Columns((Function(c) c.Header = "GUTACHTENMIT").DisplayIndex)
            dgStamm.Columns(1).DisplayIndex = 16
            dgStamm.Columns(17).DisplayIndex = 7
            ' dgStamm.Columns(1).Visibility = Windows.Visibility.Collapsed
            ' dgStamm.Columns(1).Width = 300
        End If
    End Sub
    Sub fktKostenNachVorneHolen()
        dgStamm.Columns(18).DisplayIndex = 1
        dgStamm.Columns(19).DisplayIndex = 2
        dgStamm.Columns(20).DisplayIndex = 3
        dgStamm.Columns(21).DisplayIndex = 4
        dgStamm.Columns(22).DisplayIndex = 5
        dgStamm.Columns(23).DisplayIndex = 6
        dgStamm.Columns(24).DisplayIndex = 7
        dgStamm.Columns(25).DisplayIndex = 8
        dgStamm.Columns(26).DisplayIndex = 9
        dgStamm.Columns(27).DisplayIndex = 10
        dgStamm.Columns(28).DisplayIndex = 11

        dgStamm.Columns(29).DisplayIndex = 12
        dgStamm.Columns(30).DisplayIndex = 13
        dgStamm.Columns(31).DisplayIndex = 14
        dgStamm.Columns(32).DisplayIndex = 15
        dgStamm.Columns(33).DisplayIndex = 16
    End Sub

    'Private Sub setzeBearbeiterText(ByVal bearbeiterauswahlbox As WinBearbeiterauswahl)
    '    If String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlVorname) Then
    '        btnBearbeiterauswahl.Content = bearbeiterauswahlbox.auswahlNAchname
    '    Else
    '        btnBearbeiterauswahl.Content = bearbeiterauswahlbox.auswahlNAchname & ", " & bearbeiterauswahlbox.auswahlVorname
    '    End If
    'End Sub
    Private Sub btnBearbeiterauswahl_Click_1(sender As Object, e As RoutedEventArgs)
        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl
        bearbeiterauswahlbox.ShowDialog()
        If Not String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlInitiale) Then
            If bearbeiterauswahlbox.auswahlNAchname.ToString = "alle" Then
                fuerBearbeiterName = "alle"
            End If
            cmbUserChange(bearbeiterauswahlbox.auswahlInitiale)
            If String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlVorname) Then
                btnBearbeiterauswahl.Content = bearbeiterauswahlbox.auswahlNAchname
            Else
                btnBearbeiterauswahl.Content = bearbeiterauswahlbox.auswahlNAchname & ", " & bearbeiterauswahlbox.auswahlVorname
            End If
            '  suchentaste()
        End If
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub setTitle()
        Title = "Paradigma - Suchen nach Vorgängen; " & myGlobalz.sitzung.aktBearbeiter.Name
    End Sub

    Private Sub setzeBearbeiterText(nn As String, vn As String)
        If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
            btnBearbeiterauswahl.Content = "alle"
        Else
            If String.IsNullOrEmpty(vn) Then
                btnBearbeiterauswahl.Content = nn
            Else
                btnBearbeiterauswahl.Content = nn & ", " & vn
            End If
        End If
    End Sub

    Private Sub iniitBearbeiter()
        If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
            filterBearbeiterInitiale = "alle"
        Else
            filterBearbeiterInitiale = myGlobalz.sitzung.aktBearbeiter.Initiale
        End If
        cmbUserChange(filterBearbeiterInitiale)
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.Vorname) Then
            btnBearbeiterauswahl.Content = myGlobalz.sitzung.aktBearbeiter.Name
        Else
            btnBearbeiterauswahl.Content = myGlobalz.sitzung.aktBearbeiter.Name & ", " & myGlobalz.sitzung.aktBearbeiter.Vorname
        End If
    End Sub

    Private Sub chkboxInterneZahlungNEU(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub chkboxverwaltungsgebuehr_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub verwaltungsgebuehrBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub ersatzgeld_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub ersatzgeldBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub sicherheit_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub sicherheitBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub kostenNachvorneHolen_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        fktKostenNachVorneHolen()
        e.Handled = True
    End Sub

    Private Function getKostenverknuepfung() As String
        Try
            Dim a As ComboBoxItem
            a = CType(cmbAndOrKosten.SelectedItem, ComboBoxItem)
            Dim auswahl As String = a.Tag.ToString
            Return auswahl
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub BUSSGELD_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub BUSSGELDBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub VERWARNUNGSGELD_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub VerwarnungsgeldBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub chkboxStellungnahmeclick(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub chkboxmitBeteiligtensuche_Click(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    'Private Function addiereBeteiligtensuche(altsql As String, textfilter As String) As String
    '    Dim neu As String
    '    altsql = altsql.Replace("order by LetzteBearbeitung desc", "")
    '    neu = " union " &
    '        " (select s.*,v.*,k.VERWALTUNGSGEBUEHR,k.VERWALTUNGSGEBUEHR_BEZAHLT," &
    '        " k.ERSATZGELD,k.ERSATZGELD_BEZAHLT," &
    '        " k.SICHERHEIT,k.SICHERHEIT_BEZAHLT,k.INTERNEZAHLUNG,k.quelle as kquelle, k.ts as kts," &
    '        " k.verwarnungsgeld,k.verwarnungsgeld_bezahlt,k.bussgeld,k.bussgeld_bezahlt" &
    '        " from beteiligte b, vorgang v,stammdaten s  ,kosten  k" &
    '        " where b.vorgangsid=v.vorgangsid  and b.vorgangsid=s.vorgangsid  and lower(b.nachname) like '%" & textfilter & "%' " &
    '        " and  s.VorgangsID=k.vorgangsid  (+)" &
    '        ")"
    '    neu = altsql & neu
    '    Return neu
    'End Function






    Private Sub chkboxmitEreignissuche_Checked(sender As Object, e As RoutedEventArgs) Handles chkboxmitEreignissuche.Checked
        If chkboxmitEreignissuche.IsChecked Then
            gibPopup()
        End If

        e.Handled = True
    End Sub

    Private Shared Sub gibPopup()
        MessageBox.Show("Die Suche in den Ereignissen ist sehr zeitaufwendig. " & Environment.NewLine &
                        "Daher sollten vorher alle gewünschten Filtereinstellungen getroffen werden." & Environment.NewLine &
                        "Paradigma ist während der Suche nicht nutzbar." & Environment.NewLine &
                        "" & Environment.NewLine &
                        "Bitte nutzen Sie diese Möglichkeit nur dann wenn sie sie wirklich benötigen!")
    End Sub

    Private Sub chkboxmitEreignissuche_Click(sender As Object, e As RoutedEventArgs)
        ' gibPopup()
        e.Handled = True
    End Sub

    Private Sub chkMitStammdatenSuche_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub tbsachgebietnr_TextChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbsachgebietnr.TextChanged
        If tbsachgebietnr Is Nothing Then Exit Sub
        Dim aktiv As Boolean = Not String.IsNullOrEmpty(tbsachgebietnr.Text)
        textboxRahmenAktiveinfaerben(aktiv, tbsachgebietnr)
        aktSachgebietnr = tbsachgebietnr.Text
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Function getAnzahlGewuenschterRecs(p1 As String) As Integer
        Try
            If IsNumeric(p1) Then
                Return CInt(p1)
            Else
                Return 100000
            End If
        Catch ex As Exception
            nachricht("fehler in getAnzahlGewuenschterRecs: ", ex)
        End Try

    End Function

    Private Sub Btntest_Click(sender As Object, e As RoutedEventArgs)
        dgStammListeClear()
        e.Handled = True
    End Sub

    Private Sub dgStammListeClear()
        dgStamm.DataContext = Nothing
    End Sub

End Class
