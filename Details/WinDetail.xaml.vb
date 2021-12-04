Imports System.Data
Imports System.IO
Partial Public Class Window_Detail
    Public Property retcode As String  ' 0=niente "zurBestandsUebersicht"=bestand aufrufen
    Private RubberbandStartpt As Point?
    'Private RubberbandStartpt As Point? = Nothing

    Private RubberbandEndpt As Point?
    'Private RubberbandEndpt As Point? = Nothing
    Private rubberbox As Rectangle
    Private myPolyVertexCount As Integer
    Private KoordinateKLickpt As Point?
    'Private KoordinateKLickpt As Point? = Nothing
    Private Property CanvasClickModus As String
    Private Property schlagworteWurdeGeaendert As Boolean
    'Private Property schlagworteWurdeGeaendert As Boolean = False

    Private Property ladevorgangAbgeschlossen As Boolean
    'Private Property ladevorgangAbgeschlossen As Boolean = False

    Delegate Sub watcherCallBackToUIThread(ByVal e As FileSystemEventArgs)
    Public gifKartenwatcher As FileSystemWatcher
    Public officeDocWatcher As FileSystemWatcher
    Private ableitungskreis As clsAbleitungskreis
    Public Property ereignisDokListe As List(Of clsEreignisDok)
    'Public Property ereignisDokListe As New List(Of clsEreignisDok)
    Public Property ereignisDokExpand As Boolean


    Private Shared Sub prepareActionlog()
        l("prepareActionlog-----------------------------------")
        l("myGlobalz.sitzung.VorgangsID " & myGlobalz.sitzung.aktVorgangsID)
        myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir = myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID) 'glob2.archicsubdirfeststellen()
        l("myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir " & myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
        l("myGlobalz.Arc.rootDir.ToString: " & myGlobalz.Arc.rootDir.ToString)

        '    Dim tempo$ = myGlobalz.Arc.rootDir.ToString & myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir
        Dim erfolg As Boolean = myGlobalz.sitzung.aktVorgang.Stammdaten.createArchivsubdir(myGlobalz.Arc.rootDir.ToString, myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)

        nachricht(If(Not erfolg,
                           "Fehler beim erzeugen des createArchivsubdir: (ARCHIVFEHLER!!! ggf. ist das Archiv nivht mehr im Zugriff!!!!)" & myGlobalz.sitzung.aktBearbeiter.username & " " & myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                           "createArchivsubdir erfolgreich"))
        Dim sd$ = myGlobalz.Arc.rootDir.ToString & myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir.Replace("/", "\")
        'clstart.myc.aLog = New ActionLog(String.Format("{0}\{1}_NEWactionlog.csv", sd, myGlobalz.sitzung.Bearbeiter.Initiale))
        CLstart.myc.aLog = New CLstart.ActionLog(String.Format("{0}\NEWactionlog.csv", sd))
    End Sub


    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
        stackp1.Background = myGlobalz.GetSecondBackground()
        kopp.Background = myGlobalz.GetSecondBackground()
        grpStammdatenUebersicht.Background = myGlobalz.GetSecondBackground()
    End Sub

    Private Function starteDetails() As Boolean
        Try

            stckmehrfachtools.Visibility = Windows.Visibility.Collapsed : stckmehrfachfotos.Visibility = Windows.Visibility.Collapsed
            Psession.presDokus = New List(Of clsPresDokumente)
            gastLayout()
            CLstart.myc.kartengen.aktMap.Vgrund = ""
            grpStammdatenUebersicht.DataContext = myGlobalz.sitzung.aktVorgang.Stammdaten
            initTabcontrolsMaxheight()
            setCanvasSize()
            glob3.allAktobjReset.execute(myGlobalz.sitzung)
            nachricht("starteDetails")
            alte_gridsloeschen()
            alteDTsloeschen()
            detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
            ' detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0

            detailsTools.initErgeinistypCombo(Me, "detail_GISHintergrund.xml", "XMLSourceComboBoxGISHintergrund") : cmbGISHintergrund.SelectedIndex = 0
            setComboboxRaumbezugtyp()
            glob2.initGemKRZCombo(Me)
            glob2.initCMBParagraf(Me)
            glob2.initRaumNrCombo(Me)

            cmbGemKRZ.SelectedIndex = 46
            If Not Stammdateneinlesen() Then
                Me.Close()
                Return False
            End If
            setTitelleiste()
            initStammBearbeiterTooltip()
            initStammCheckBoxen()
            setzeErledigtflagfarbe()
            detailsTools.clearCheckoutDokulist()
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
            refreshverwandteServer(myGlobalz.sitzung.aktVorgangsID)
            refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
            refreshProjekt(myGlobalz.sitzung.aktVorgangsID)
            refreshKosten(myGlobalz.sitzung.aktVorgangsID)
            initCombosVerwandte()
            initKostenFeld()
            If myGlobalz.sitzung.modus = "neu" Then
                TabControl1.SelectedIndex = 6
            End If
            SichtbarkeitGISKnopf()
            If clsParadigmaRechte.binEignerOderAdmin() Then
                btnAllgemeinLoeschen.IsEnabled = True
            Else
                btnAllgemeinLoeschen.IsEnabled = False
            End If
            ' pruefeDeckblatt() 
            setWeitereBearbeiterListeDarstellen()
            detailsTools.VorgangLocking("ein")

            projektDatenholen()
            prepareActionlog()
            CLstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
            CLstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
            CLstart.myc.aLog.komponente = "detail"
            CLstart.myc.aLog.aktion = "vorgang geoeffnet"
            CLstart.myc.aLog.log()
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            schlagworteWurdeGeaendert = False
            schlagworteEinfaerben()
            'natureg_button_sichtbarmachen()
            chkExpandereignis.IsChecked = True
            VerlaufmitDokumentenSetzen()
            cmbGemKRZ.SelectedItem = "Gemeinde"

            '  detailsTools.hatraumbezugDarstellen()
            nachricht("FORMSTART ERFOLGFREICH DURCHGEFÜHRT detail ######################################################### detail")
            ladevorgangAbgeschlossen = True
            Protokollzugriffschalten()

            Return True
        Catch ex As Exception
            MsgBox(String.Format("Schwerer Fehler in der Startroutine! Bitte Admin informieren.{0}{1}", vbCrLf, ex))
            nachricht("Fehler Schwerer Fehler in der Startroutine! Bitte Admin informieren.", ex)
        End Try
    End Function

    Shared Function deckblattvorhanden() As Boolean
        If myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl = "562" Then Return True
        Return False
    End Function

    Private Sub setCanvasSize()
        Try
            myCanvas.Height = initcanvasHeight() ' TabControl1.Height
            myCanvas.Width = CLng(System.Windows.SystemParameters.PrimaryScreenWidth) - CLng(TabControl1.Width)
            '   grpMinimapSteuerung.Width = myCanvas.Width
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Berechung der MINIGIS-Zeichenfläche")
            myCanvas.Height = 100
            myCanvas.Width = 150
        End Try
    End Sub

    Sub initCombosVerwandte()
        Try
            cmbDokuverwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            cmbBeteiligteVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            cmbRBVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            cmbVerlaufVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
        Catch ex As Exception
            MsgBox("Fehler in initCombosVerwandte." & ex.ToString)
            nachricht("Fehler initCombosVerwandte.", ex)
        End Try
    End Sub

    Sub alteDTsloeschen()
        Try
            If myGlobalz.sitzung.tempREC.dt IsNot Nothing Then myGlobalz.sitzung.tempREC.dt.Clear()
            If myGlobalz.sitzung.beteiligteREC.dt IsNot Nothing Then myGlobalz.sitzung.beteiligteREC.dt.Clear()
            If myGlobalz.sitzung.EreignisseRec.dt IsNot Nothing Then myGlobalz.sitzung.EreignisseRec.dt.Clear()
            If myGlobalz.sitzung.raumbezugsRec.dt IsNot Nothing Then myGlobalz.sitzung.raumbezugsRec.dt.Clear()
            If myGlobalz.Arc.vorgangDocDt IsNot Nothing Then myGlobalz.Arc.vorgangDocDt.Clear()
            If myGlobalz.Arc.ArcRec.dt IsNot Nothing Then myGlobalz.Arc.ArcRec.dt.Clear()
            If myGlobalz.sitzung.VerwandteDT IsNot Nothing Then myGlobalz.sitzung.VerwandteDT.Clear()
        Catch ex As Exception
            MsgBox("Fehler in alteDTsloeschen." & ex.ToString)
        End Try
    End Sub


    Private Sub refreshBeteiligteListe(ByVal vid%)
        Try
            clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(vid)
            displayBeteiligtenListe()
        Catch ex As Exception
            nachricht("fehler in refreshBeteiligteListe: " & ex.ToString)
        End Try
    End Sub

    'Private Sub setComboboxBeteiligte()
    '    Dim filename As String = myGlobalz.appdataDir & "\config\Combos\Detail_Beteiligte_Rollen.xml"
    '    Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxBeteiligte"), XmlDataProvider)
    '    existing.Source = New Uri(filename)
    '    '  ComboBoxBeteiligte.SelectedIndex = 0
    'End Sub

    Private Sub setComboboxRaumbezugtyp()
        ComboBoxRaumbezug.Items.Add("Hinzufügen")
        ComboBoxRaumbezug.Items.Add("Adresse")
        ComboBoxRaumbezug.Items.Add("Flurstück")
        ComboBoxRaumbezug.Items.Add("Punkt mit Umkreis")
        ComboBoxRaumbezug.Items.Add("Polygon")
        ComboBoxRaumbezug.SelectedIndex = 0
    End Sub

    Sub setTitelleiste()
        If myGlobalz.sitzung.modus = "neu" Then Title = "Paradigma: Neuen Vorgang anlegen"
        If myGlobalz.sitzung.modus = "edit" Then
            Title = "Paradigma: Vorgangsbearbeitung Nr. " & myGlobalz.sitzung.aktVorgangsID &
                ", Eingang: " & myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum &
                ", Angelegt: " & myGlobalz.sitzung.aktVorgang.Stammdaten.Aufnahmedatum
        Else

        End If
    End Sub

    Sub speichernAllgemein()
        If myGlobalz.sitzung.modus = "neu" Then
            If Not NEU_eingabenOk() Then Exit Sub
            If Not NEUform2objok() Then Exit Sub
            If Not glob2.NEU_VorgangStamm_2DBOk() Then Exit Sub
            'myGlobalz.sitzung.modus = "edit"
            'btnAllgemein.IsEnabled=false
            Me.Close()
        End If
        If myGlobalz.sitzung.modus = "edit" Then
            'ggf_bearbeiterAendern()
            If Not NEU_eingabenOk() Then Exit Sub
            If Not NEUform2objok() Then Exit Sub
            If Not glob2.EDIT_VorgangStamm_2DBOk() Then Exit Sub
        End If
    End Sub

    Shared Function NEU_eingabenOk() As Boolean
        Return True
    End Function

    Shared Function NEUform2objok() As Boolean
        Return True
    End Function

    Shared Function Stammdateneinlesen() As Boolean
        Return clsVorgangCTRL.leseVorgangvonDBaufObjekt(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten, myGlobalz.sitzung.VorgangREC)
    End Function

    Public Shared Function ToObj_Wiedervorlage() As Boolean
        If Not myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
            myGlobalz.sitzung.aktWiedervorlage.datum = _
             CDate(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item("Datum"))
        End If
    End Function

    Private Sub cmbVerlaufAuswahl_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbVerlaufAuswahl.SelectionChanged
        Try
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower = "hinzufügen" Then Exit Sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            detailsTools.eEreignisstarten(item)
            cmbVerlaufAuswahl.SelectedValue = ""
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            '     cmbStatus.SelectedValue = myGlobalz.sitzung.aktVorgang.Stammdaten.status ihah
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" & ex.ToString)
        End Try
    End Sub








    Private Sub refreshEreignisseListe(ByVal vid As Integer)
        Dim hinweis As String = ""
        ereignisDokExpand = CBool(chkExpandereignis.IsChecked)
        ereignisDokListe.Clear()
        Try
            dgEreignisse.DataContext = Nothing
        Catch ex As Exception
        End Try
        Try

            If detailsTools.initEreigisseDatatable(hinweis, vid, ereignisDokExpand, ereignisDokListe) > 0 Then
                Try
                    dgEreignisse.DataContext = ereignisDokListe ' myGlobalz.sitzung.EreignisseRec.dt
                Catch ex As Exception
                End Try
                tabheaderVerlauf.Header = "Verlauf " & myGlobalz.sitzung.EreignisseRec.dt.Rows.Count
            Else
                Try
                    dgEreignisse.DataContext = ereignisDokListe ' myGlobalz.sitzung.EreignisseRec.dt
                Catch ex As Exception
                End Try
                tabheaderVerlauf.Header = "Verlauf "
            End If

        Catch ex As Exception
            nachricht("fehler in refreshEreignisseListe: " & ex.ToString)
        End Try
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: refresh ereignisse  ")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub


    Public Sub refreshRaumbezugsListe(ByRef vid As Integer, istnurverwandt As Boolean)
        Dim hinweis As String = ""
        Dim statusWert As Int16
        Try
            If istnurverwandt Then
                statusWert = 1
            Else
                statusWert = 0
            End If
            dgRaumbezug.DataContext = Nothing
            Dim erfolg As Boolean = RBtoolsns.initraumbezugsDT_alleDB.exe(vid)
            If erfolg Then RBtoolsns.statusSpalteErgaenzenUndFuellen.statusSpalteErgaenzenUndMitStandardFuellen(myGlobalz.sitzung.raumbezugsRec.dt,
                "STATUS", statusWert)
            'If erfolg Then RBtoolsns.statusSpalteErgaenzenUndFuellen.execute(myGlobalz.sitzung.raumbezugsRec.dt,
            '                                                    myGlobalz.sitzung.RaumbezugsIDsDT,
            '                                                    "Status", "RaumbezugsID")
            If erfolg Then
                dgRaumbezug.DataContext = myGlobalz.sitzung.raumbezugsRec.dt
                'tabheaderRaumbezug.Header = "Raumbezug " & myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count
                '  tabheaderRaumbezug.Header = detailsTools.getRBheadertext("Raumbezug ", myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count)
            Else
                dgRaumbezug.DataContext = Nothing
                'tabheaderRaumbezug.Header = "Raumbezug "
                ' tabheaderRaumbezug.Header = detailsTools.getRBheadertext("Raumbezug ", 0)
            End If
            tabheaderRaumbezug.Header = detailsTools.getRBheadertext("Raumbezug ", myGlobalz.sitzung.raumbezugsRec.dt)

            SichtbarkeitGISKnopf()
            If myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug Then
                chkRaumbezuegeObsolet.IsChecked = True
            Else
                chkRaumbezuegeObsolet.IsChecked = False
            End If
            INITMiniMapPresentation(myGlobalz.sitzung.raumbezugsRec.dt)
            glob2.generateLayerWithShapes()
        Catch ex As Exception
            nachricht("fehler in refreshRaumbezugsListe: " & ex.ToString)
        End Try
    End Sub

    Public Sub gislink_click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim a = CType(e.Source, Button)
        showGIS4Raumbezug(CInt(a.Content))
        e.Handled = True
    End Sub

    Sub zum_Ergeignis_Editmode()
        myGlobalz.sitzung.Ereignismodus = "edit"
        _Ergeignis_edit()
    End Sub

    Sub _Ergeignis_edit()
        clsEreignisTools.leseEreignisByID(myGlobalz.sitzung.aktEreignis.ID)
        clsEreignisTools.ToObj_Ereignis()
        Dim speicherung As Date = myGlobalz.sitzung.aktEreignis.Datum
        Select Case myGlobalz.sitzung.aktEreignis.Art.ToLower
            Case "wiedervorlage"
                If WiedervorlageFormStarten() Then Exit Sub
            Case "zahlung"
                If Zahlungedit() Then Exit Sub
            Case "weblink"
                If glob2.webLinkedit("edit") Then Exit Sub
            Case Else
                ' MsgBox("e " & myGlobalz.sitzung.aktEreignis.Datum)
                Dim winEreignis_detail As New Window_Ereignis_Detail("")
                winEreignis_detail.ShowDialog()
                '    MsgBox(speicherung & ", g " & myGlobalz.sitzung.aktEreignis.Datum)
        End Select
    End Sub

    Private Function Zahlungedit() As Boolean
        Dim wvw As New WINzahlungdetail("edit")
        wvw.ShowDialog()
        Return True
    End Function



    Private Function WiedervorlageFormStarten() As Boolean
        myGlobalz.sitzung.Wiedervorlagemodus = "edit"
        If myGlobalz.sitzung.aktEreignis.DokumentID < 1 Then
            nachricht_und_Mbox("Die Wiedervorlage mit der Nummer 0 kann nicht gefunden werden.")
            ' Me.Close()
            Return True
        End If
        myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID = myGlobalz.sitzung.aktEreignis.DokumentID
        ' btnSpeichernEreignis.IsEnabled = False
        '  Me.Close()
        Dim wvw As New Window_WiedervorlageDetail
        wvw.ShowDialog()
        Return True
    End Function


    ''' <summary>
    ''' sollte immer zusammen mit  refreshFotos(myGlobalz.sitzung.VorgangsID)
    ''' verwendet werden
    ''' </summary>
    ''' <param name="VorgangsID"></param>
    ''' <remarks></remarks>
    Private Sub refreshDokumente(ByVal VorgangsID As Integer)
        Try
            'myGlobalz.sitzung.VorgangsID,
            Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(VorgangsID, "keinefotos") ' nach myGlobalz.Arc.ArcRec
            If bresult Then
                Psession.presDokus = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                'myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
                dgVorgangDokumente.DataContext = Psession.presDokus 'myGlobalz.Arc.vorgangDocDt
                tabheaderDokumente.Header = "Dokumente " & myGlobalz.Arc.ArcRec.dt.Rows.Count
            Else
                Psession.presDokus = Nothing
                myGlobalz.Arc.vorgangDocDt = Nothing
                dgVorgangDokumente.DataContext = Nothing
                tabheaderDokumente.Header = "Dokumente"
            End If
        Catch ex As Exception
            nachricht("fehler in refreshDokumente: " & ex.ToString)
        End Try
    End Sub

    Private Sub refreshFotos(ByVal VorgangsID As Integer)
        Try
            Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(VorgangsID, "nurfotos") ' nach myGlobalz.Arc.ArcRec
            If bresult Then
                Psession.presFotos = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                ' myGlobalz.Arc.vorgangFotoDt = myGlobalz.Arc.ArcRec.dt.Copy
                dgVorgangFotos.DataContext = Psession.presFotos 'myGlobalz.Arc.vorgangFotoDt
                tabheaderFotos.Header = "Fotos " & Psession.presFotos.Count
            Else
                Psession.presFotos = Nothing
                myGlobalz.Arc.vorgangFotoDt = Nothing
                dgVorgangFotos.DataContext = Nothing
                tabheaderFotos.Header = "Fotos"
            End If
        Catch ex As Exception
            nachricht("fehler in refreshFotos: " & ex.ToString)
        End Try
    End Sub


    Private Sub dokuhinzufuegenUndRefresh()
        If glob2.FktDokumentehinzu(0) Then
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        End If

    End Sub
    Private Sub btnDokumentehinzu2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDokumentehinzu2.Click
        nachricht("USERAKTION: dokus hinzu ")
        dokuhinzufuegenUndRefresh()
        e.Handled = True
    End Sub

    Shared Function RaumbezugauswahlistOK(ByVal typ As String) As Boolean
        If typ.ToLower <> "hinzufügen" Then Return True
        Return False
    End Function

    Function starteRaumbezugdetail(ByVal typ As String) As Boolean
        Dim anychange As Boolean = True
        myGlobalz.sitzung.raumbezugsmodus = "neu"
        ' myGlobalz.sitzung.aktEreignis.Datum = Now
        Select Case typ$.ToLower
            Case "adresse"
                Dim winadr As New Window_RB_Adresse
                winadr.ShowDialog()
                anychange = CBool(winadr.DialogResult)
            Case "flurstück"
                Dim winflur As New Window_Flurstuecksauswahl
                winflur.ShowDialog()
                anychange = CBool(winflur.DialogResult)
            Case "punkt mit umkreis"
                Dim winflur As New Win_punktMitUmkreis(tbMinimapCoordinate.Text)
                winflur.ShowDialog()
                anychange = CBool(winflur.DialogResult)
            Case "polygon"
                'Dim winflur As New Win_punktMitUmkreis(tbMinimapCoordinate.Text)
                'winflur.ShowDialog()
                'anychange = CBool(winflur.DialogResult)
                MsgBox("Sie können neue Polygone über die 'Fläche-Messen'- Funktion der Minimap erstellen.", MsgBoxStyle.Exclamation, "Neues Polygon hizufügen")
                anychange = False
            Case Else
                MsgBox("Fehler: Raumbezugstyp:" & typ & " ist unbekannt!")
                anychange = False
        End Select
        'btnAllgemein.IsEnabled = False 'wg dem gemeindekürzel, das ja teil der stammdaten ist
        myGlobalz.sitzung.aktEreignis.Art = typ
        Return anychange
    End Function

    Private Sub btnAllcheckout_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAllcheckout.Click
        nachricht("USERAKTION: alles zum PC ")
        myGlobalz.Arc.AllesAuscheckenVorgang(True, True)
        e.Handled = True
    End Sub

    Sub showGIS4Raumbezug(ByVal id%)
        'todo
        If id > 0 Then
            'über die ID die Koordinaten holen
            'Dim pt As myPoint = DBraumbezug_Mysql.getCoords4ID_Raumbezug(id%)
            Dim pt As myPoint = RBtoolsns.getCoords4ID_Raumbezug_alleDB.exe(id%)
            Dim gis As New clsGISfunctions
            gis.GISAufruf_Mittelpunkt(pt)
        End If
    End Sub


    Public Function zum_dgFlurstueck_Editmode() As Boolean
        Dim sekid$ = CStr(myGlobalz.sitzung.aktFST.SekID)
        ' DBraumbezug_Mysql.RB_Flurstueck_holen(sekid$)  'auf temprec
        RBtoolsns.RB_Flurstueck_holen_alleDB.exe(sekid)
        FST_tools.DTaufFSTObjektabbilden(myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktFST)

        'form aufrufen
        Dim wfst As New Window_Flurstuecksauswahl
        wfst.ShowDialog()
        Return CBool(wfst.DialogResult)
    End Function

    Public Sub zum_dgFotoEditmode()
        Dim sekid = CStr(myGlobalz.sitzung.aktParaFoto.SekID)
        ' DBraumbezug_Mysql.RB_ParaFoto_holen(sekid$)
        RBtoolsns.RB_ParaFoto_holen_alleDB.exe(sekid$)
        myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)                                                          'hire wird zum dokument verzweigt. NICHt zum Raumbezug
        myGlobalz.sitzung.aktDokument.DocID = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("dokumentid")))
        myGlobalz.sitzung.aktDokument.EXIFlon = myGlobalz.sitzung.aktParaFoto.punkt.X.ToString
        myGlobalz.sitzung.aktDokument.EXIFlat = myGlobalz.sitzung.aktParaFoto.punkt.Y.ToString

        '   DBraumbezug_Mysql.einzelDokument_holen(myGlobalz.sitzung.aktDokument.DocID.ToString)
        RBtoolsns.einzelDokument_holen_alleDB.exe(myGlobalz.sitzung.aktDokument.DocID.ToString)

        detailsTools.DTaufFotoObjektabbilden(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.tempREC.dt)
        myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
        'form aufrufen
        '	DokumentenArchiv.JPG_handeln(myGlobalz.sitzung.aktDokument)

        '	DokumentenArchiv.Archiv_definiereAktdokument(item)
        '	dgVorgangDokumente.SelectedItem = Nothing
        Dim handlenr As Integer = 0
        Dim darst As Boolean = detailsTools.Archiv_aktiviere_Dokument()

        detailsTools.darstellen(darst)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
    End Sub




    'Private Sub dgRaumbezug_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    dgRaumbezug_SelectionChanged_1Extracted()
    'End Sub

    'Private Sub ComboBoxBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    Try
    '        Dim item As String = CType(ComboBoxBeteiligte.SelectedValue, String)
    '        If item = "Hinzufügen" Then Exit Sub
    '        Dim auswahl$ = item.ToString
    '        If ComboBoxBeteiligteauswahlistOK((auswahl$)) Then
    '            starteComboBoxBeteiligtedetail()
    '        End If
    '        ComboBoxBeteiligte.SelectedItem = 0
    '    Catch ex As Exception
    '        nachricht_und_Mbox("ComboBoxBeteiligte_SelectionChanged. " & ex.ToString)
    '    End Try
    'End Sub

    Shared Function ComboBoxBeteiligteauswahlistOK(ByVal typ As String) As Boolean
        If typ.ToLower <> "hinzufügen" Then Return True
        Return False
    End Function

    Public Shared Sub starteComboBoxBeteiligtedetail()
        myGlobalz.sitzung.BeteiligteModus = "neu"
        myGlobalz.sitzung.aktEreignis.Datum = Now
        Dim winadr As New Window_RB_Adresse
        winadr.ShowDialog()
    End Sub

    Private Sub dgRaumbezug_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgRaumbezug.SelectionChanged
        If dgRaumbezug.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgRaumbezug.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        dgRaumbezug_SelectionChanged_1Extracted()
    End Sub

    Private Sub dgRaumbezug_SelectionChanged_1Extracted()
        Dim anyChange As Boolean = True
        Try
            Dim item As DataRowView = CType(dgRaumbezug.SelectedItem, DataRowView)
            If item Is Nothing Then Return
            myGlobalz.sitzung.raumbezugsmodus = "edit"
            Dim rbtyp As String = item("TYP").ToString
            Select Case rbtyp
                Case CInt(RaumbezugsTyp.Adresse).ToString
                    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktADR)
                    myGlobalz.sitzung.aktADR.setcoordsAbstract()
                    glob2.zum_dgAdresse_Editmode()
                Case CInt(RaumbezugsTyp.Flurstueck).ToString
                    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktFST)
                    myGlobalz.sitzung.aktFST.setcoordsAbstract()
                    anyChange = zum_dgFlurstueck_Editmode()
                Case CInt(RaumbezugsTyp.Foto).ToString
                    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktParaFoto)
                    '    myGlobalz.sitzung.aktParaFoto.dokumentid = CStr(item("DOKUMENTID"))
                    myGlobalz.sitzung.aktParaFoto.setcoordsAbstract()
                    zum_dgFotoEditmode()
                Case CInt(RaumbezugsTyp.Umkreis).ToString
                    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktPMU)
                    myGlobalz.sitzung.aktParaFoto.setcoordsAbstract()
                    ParaUmkreisTools.zum_dgUmkreisEditmode(tbMinimapCoordinate.Text)
                Case CInt(RaumbezugsTyp.Polygon).ToString, CInt(RaumbezugsTyp.Polyline).ToString
                    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktPolygon)
                    PolygonTools.zum_dgPolygonEditmode()

            End Select
            dgRaumbezug.SelectedItem = Nothing
            refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
        Catch ex As Exception
            MessageBox.Show(String.Format("dgRaumbezug_SelectionChanged: {0}", ex))
        End Try
    End Sub

    Private Sub raumbezugHinzufuegenUndRefresh(ByVal rbtyp$)
        If Raumbezug_hinzufuegen(rbtyp) Then
            refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)

            ' HatRaumbezug in den  Stammdaten speichern 
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
            '  Dim erfolg As Boolean = VSTTools.editStammdaten_alleDB.exe(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten)
        End If
    End Sub



    Private Sub ComboBoxRaumbezug_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBoxRaumbezug.SelectionChanged
        If ComboBoxRaumbezug.SelectedItem Is Nothing Then Exit Sub
        If ComboBoxRaumbezug.SelectedValue.ToString.ToLower = "hinzufügen" Then Exit Sub
        nachricht("USERAKTION: raumbezug hinzu ")
        Dim rbtyp$ = CType(ComboBoxRaumbezug.SelectedValue, String)
        raumbezugHinzufuegenUndRefresh(rbtyp)
        ComboBoxRaumbezug.SelectedIndex = 0
    End Sub

    Private Function Raumbezug_hinzufuegen(ByVal auswahl As String) As Boolean
        Dim anychange As Boolean = True
        Try
            If RaumbezugauswahlistOK((auswahl)) Then
                anychange = starteRaumbezugdetail((auswahl))
            End If
            ComboBoxRaumbezug.SelectedItem = 0
            Return anychange
        Catch ex As Exception
            nachricht("Fehler: Raumbezug_hinzufuegen. " & auswahl)
            Return False
        End Try
    End Function

    Function BeteiligteauswahlistOK(ByVal typ As String) As Boolean
        If typ.ToLower <> "hinzufügen" Then Return True
        Return False
    End Function

    Sub starteBeteiligtedetail() 'ByVal Rolle$)
        myGlobalz.sitzung.BeteiligteModus = "neu"
        myGlobalz.sitzung.aktPerson.clear()
        'Dim winpers As New Window_Person
        'winpers.ShowDialog()
        Dim winpersneu As New winBeteiligteDetail("neu")
        winpersneu.ShowDialog()
        refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
    End Sub


    Private Sub BeteiligtenAusgewaehlt()
        Try
            Dim item As DataRowView
            item = CType(dgBeteiligte.SelectedItem, DataRowView)
            If item Is Nothing Then Return
            myGlobalz.sitzung.BeteiligteModus = "edit"
            clsBeteiligteBUSI.BeteiligtenRec2Obj(item, myGlobalz.sitzung.aktPerson)
            dgBeteiligte.SelectedItem = Nothing
            zum_Beteiligte_Editmode()
            refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        Catch ex As Exception
            nachricht("BeteiligtenAusgewaehlt: " & String.Format("BeteiligtenAusgewaehlt: {0}", ex))
        End Try
    End Sub

    Private Sub dgBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgBeteiligte.SelectionChanged
        Dim item As DataRowView
        Try
            item = CType(dgBeteiligte.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        BeteiligtenAusgewaehlt()
        e.Handled = True
    End Sub

    Sub zum_Beteiligte_Editmode()
        myGlobalz.sitzung.BeteiligteModus = "edit"
        _Beteiligte_edit()
    End Sub

    Sub _Beteiligte_edit()
        'clsGlobalz.sitzung.Vorgang.clear()
        Dim winpersneu As New winBeteiligteDetail("edit")
        winpersneu.ShowDialog()
        refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub btnExplorer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnExplorer.Click
        nachricht("USERAKTION: explorer  ")
        IO.Directory.CreateDirectory(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
        System.Diagnostics.Process.Start(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
        e.Handled = True
    End Sub

    Shared Sub Zugriffsrechte()
        glob2.setzeZugriffsrechte()
    End Sub

    Private Sub dgVorgangDokumente_SelectionChanged_1Extracted()
        If detailsTools.istDateiNameInordnung(myGlobalz.sitzung.aktDokument.DateinameMitExtension) Then
            Dim darst As Boolean = detailsTools.Archiv_aktiviere_Dokument()
            detailsTools.darstellen(darst)
            If darst Then
                OfficeCreateNewFileSystemWatcherAndSetItsProperties()
            End If
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        Else
            MsgBox("Hinweis: Der Dateiname des ausgewählten Dokumentes ist nicht in Ordnung (enthält unerlaubte Zeichen): " & Environment.NewLine &
                   myGlobalz.sitzung.aktDokument.DateinameMitExtension & Environment.NewLine &
                   "Die Datei kann so nicht aufgerufen werden.")
        End If
    End Sub

    Private Sub dgVorgangDokumente_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgVorgangDokumente.SelectionChanged
        If dgVorgangDokumente.SelectedItem Is Nothing Then Exit Sub
        Dim item As New clsPresDokumente
        Try
            item = CType(dgVorgangDokumente.SelectedItem, clsPresDokumente)

        Catch ex As Exception
            nachricht(ex.ToString)
            Exit Sub
        End Try
        If Not auswahlspalteDokus.Visibility = Windows.Visibility.Visible Then
            myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            ' DokArc.  DokumentDatarowView2Obj(item, myGlobalz.sitzung.aktDokument)
            myGlobalz.sitzung.aktDokument = CType(item.Clone, Dokument)
            DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktVorgangsID)
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
            dgVorgangDokumente_SelectionChanged_1Extracted()
        End If
        e.Handled = True
    End Sub

    'Private Sub dgEreignisse_SelectionChanged(sender As Object , e As SelectionChangedEventArgs)

    'End Sub

    Private Sub dgEreignisse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgEreignisse.SelectionChanged
        Try
            Dim item As clsEreignisDok
            Try
                item = CType(dgEreignisse.SelectedItem, clsEreignisDok)
            Catch ex As Exception
                e.Handled = True
                Exit Sub
            End Try
            item = CType(dgEreignisse.SelectedItem, clsEreignisDok)
            If item Is Nothing Then Return
            Dim ereignisID As String = item.ID.ToString()
            Dim dokID As String = CStr(item.DokumentID)
            Dim EreignisDokTyp As String = CStr(item.EreignisDokTyp)
            If EreignisDokTyp = "0" Or EreignisDokTyp = "3" Then
                'als ereignisladen
                myGlobalz.sitzung.aktEreignis.ID = CInt(ereignisID)
                dgEreignisse.SelectedItem = Nothing
                zum_Ergeignis_Editmode()
                refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            End If
            If EreignisDokTyp = "1" Then
                'als Dokument laden
                myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                detailsTools.Dokument2Obj(item, myGlobalz.sitzung.aktDokument)
                DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktVorgangsID)
                myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                dgVorgangDokumente_SelectionChanged_1Extracted()
            End If
            e.Handled = True
        Catch ex As Exception
            nachricht_und_Mbox(String.Format("Fehler: dgEreignisse_SelectionChanged: {0}", ex))
        End Try
    End Sub

    Public Sub btnFotoGucker_ClickExtracted()
        If DokArc.aktiviereFotoGucker(myGlobalz.sitzung.aktVorgangsID, myGlobalz.OhneObsoletenDokus, myGlobalz.sitzung.aktEreignis.ID) > 0 Then
            Dim winfotoguck = New winFotoGucker
            winfotoguck.ShowDialog()
            dgVorgangFotos.DataContext = Psession.presFotos 'myGlobalz.Arc.vorgangFotoDt        
        Else
            nachricht_und_Mbox("Keine Fotos gefunden")
        End If
    End Sub

    'Private Sub btnFotoGucker_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    btnFotoGucker_ClickExtracted()
    '    e.Handled = True
    'End Sub



    Private Sub btnBeteiligteRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBeteiligteRefresh.Click
        resetBeteiligteliste_ClickExtracted()
    End Sub

    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        resetVerlaufliste_ClickExtracted()
    End Sub



    'Private Sub btnAllgemein_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAllgemein.Click
    '    speichernAllgemein()
    '    btnAllgemein.IsEnabled = False
    '    e.Handled = True
    'End Sub



    'Private Sub tbSachbearbeiter_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbSachbearbeiter.TextChanged
    '    'BearbeiterNeuSetzen()
    '    ' glob2.schliessenButton_einschalten(btnAllgemein)
    'End Sub

    'Private Sub Radiodokumeta_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    'End Sub

    'Private Sub ohneObsDoku_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ohneObsDoku.Checked
    '    If ohneObsDoku.IsChecked Then
    '        myGlobalz.OhneObsoletenDokus = True
    '    Else
    '        myGlobalz.OhneObsoletenDokus = False
    '    End If

    'End Sub

    'Private Sub mitObsDoku_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles mitObsDoku.Checked
    '    If ohneObsDoku.IsChecked Then
    '        myGlobalz.OhneObsoletenDokus = True
    '    Else
    '        myGlobalz.OhneObsoletenDokus = False
    '    End If
    'End Sub

    'Private Sub standardworkflow_beteiligte_anzeigen(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
    '    Dim aa As New winzugriff("standardworkflow_beteiligte")
    '    aa.ShowDialog()
    'End Sub

    'Private Sub standardworkflow_verlauf_anzeigen(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
    '    Dim aa As New winzugriff("standardworkflow_verlauf")
    '    aa.ShowDialog()
    'End Sub

    Private Sub Window_Detail_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        If Not starteDetails() Then
            myGlobalz.einVorgangistgeoeffnet = False
            Me.Close()
        Else
            myGlobalz.einVorgangistgeoeffnet = True
        End If
        e.Handled = True
        cmbVerlaufAuswahl.SelectedIndex = 0
        '    ComboBoxBeteiligte.SelectedIndex = 0

        nachricht(cmbVerlaufAuswahl.Items.Count.ToString)
        '  nachricht(ComboBoxBeteiligte.Items.Count.ToString)
        schlagworteWurdeGeaendert = False
    End Sub



    Private Sub btnAllgemeinLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAllgemeinLoeschen.Click
        e.Handled = True
        If Not myGlobalz.sitzung.EreignisseRec.dt.IsNothingOrEmpty Then 'Rows.Count > 0 Then
            MessageBox.Show("Dem Vorgang sind noch Ereignisse zugeordnet. " + vbCrLf & _
             "Bitte löschen Sie zuerst alle Ereignisse. " + vbCrLf & _
             "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If
        If Not myGlobalz.sitzung.raumbezugsRec.dt.IsNothingOrEmpty Then
            MessageBox.Show("Dem Vorgang sind noch Raumbezüge zugeordnet. " + vbCrLf & _
             "Bitte löschen Sie zuerst alle Raumbezüge. " + vbCrLf & _
             "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If
        If myGlobalz.Arc.vorgangDocDt.IsNothingOrEmpty Then
            'löschenOK	 				
        Else
            MessageBox.Show("Dem Vorgang sind noch Dokumente zugeordnet. " + vbCrLf & _
             "Bitte löschen Sie zuerst alle Dokumente. " + vbCrLf & _
             "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
            Exit Sub
            '	MessageBox.Show("Dem Vorgang sind noch Dokumente zugeordnet. " + vbCrLf & _
            '"Bitte löschen Sie zuerst alle Dokumente. " + vbCrLf & _
            '"Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
        End If

        If Not myGlobalz.sitzung.beteiligteREC.dt.IsNothingOrEmpty Then
            MessageBox.Show("Dem Vorgang sind noch Beteiligte zugeordnet. " + vbCrLf & _
             "Bitte löschen Sie zuerst alle Beteiligten. " + vbCrLf & _
             "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If

        If Not String.IsNullOrEmpty(CLstart.myc.aktprojekt.BezeichnungLang) Then
            MessageBox.Show("Dem Vorgang ist noch ein Projekt zugeordnet. " + vbCrLf & _
        "Bitte löschen Sie Verbindung zum Projekt (Unter Stammdaten). " + vbCrLf & _
        "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If

        If Not glob2.istloeschenErnstgemeint Then Exit Sub
        clsVorgangCTRL.LoescheVorgang()
        'btnAllgemein.IsEnabled = False
        e.Handled = True
        Me.Close()
    End Sub


    Private Sub myCanvas_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles myCanvas.MouseDown
        nachricht("myCanvas_MouseDown  not imple")
        e.Handled = True
    End Sub


    Private Sub Verwandte_hinzufuegen(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: verwandte hinzu ")
        Verwandte_hinzufuegenExtracted()
        e.Handled = True
    End Sub

    Private Sub Verwandte_hinzufuegenExtracted()
        Dim neuLInk As New winlinken
        neuLInk.ShowDialog()
        If CBool(neuLInk.DialogResult) Then
            refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
        End If
    End Sub
    Sub refreshverwandte(ByVal vid As Integer)
        Try
            Dim sql As String = "select * from vorgang2fremdvorgang where vorgangsid=" & vid
            'clsVerwandte_mysql.erzeugeVerwandtenlistezuVorgang(sql$)
            VerwandteTools.erzeugeVerwandtenlistezuVorgang.exe(sql)
            myGlobalz.sitzung.VerwandteDT.Clear()
            myGlobalz.sitzung.VerwandteDT = myGlobalz.sitzung.tempREC.dt.Copy
            dgVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            initCombosVerwandte()
            If Not myGlobalz.sitzung.VerwandteDT.IsNothingOrEmpty Then
                tabheaderVerwandte.Header = "Verwandte " & myGlobalz.sitzung.VerwandteDT.Rows.Count
                VerwandteGroupboxenEnabled(True)
            Else
                tabheaderVerwandte.Header = "Verwandte "
                VerwandteGroupboxenEnabled(False)
            End If
        Catch ex As Exception
            nachricht("fehler in refreshverwandte: " & ex.ToString)
        End Try
    End Sub

    Sub VerwandteGroupboxenEnabled(ByVal modus As Boolean)
        If modus Then
            grpBVwBeteiligte.Visibility = Visibility.Visible
            grpBVwDokumente.Visibility = Visibility.Visible
            grpBVwRaumbezug.Visibility = Visibility.Visible
            grpBVwVErlauf.Visibility = Visibility.Visible
            grpBVwBeteiligte.Visibility = Visibility.Visible
        Else
            grpBVwBeteiligte.Visibility = Visibility.Collapsed
            grpBVwDokumente.Visibility = Visibility.Collapsed
            grpBVwRaumbezug.Visibility = Visibility.Collapsed
            grpBVwVErlauf.Visibility = Visibility.Collapsed
            grpBVwBeteiligte.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub displayBeteiligtenListe()
        If myGlobalz.sitzung.beteiligteREC.dt.IsNothingOrEmpty Then
            dgBeteiligte.DataContext = Nothing
            tabheaderBeteiligte.Header = "Beteiligte "
        Else
            dgBeteiligte.DataContext = myGlobalz.sitzung.beteiligteREC.dt
            tabheaderBeteiligte.Header = "Beteiligte " & myGlobalz.sitzung.beteiligteREC.dt.Rows.Count
        End If
    End Sub

    Private Sub Verwandte_refresh(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: verwandte refresh ")
        refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub dgVerwandte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        nachricht("USERAKTION: zu verwandtem wechslen ")
        dgVerwandte_SelectionChangedExtracted()
        e.Handled = True
    End Sub

    Private Sub dgVerwandte_SelectionChangedExtracted()
        Try
            '
            If dgVerwandte.SelectedItem Is Nothing Then Exit Sub
            Dim item As DataRowView = CType(dgVerwandte.SelectedItem, DataRowView)

            geaenderteStammdatenSpeichern()

            '    Dim kopplungsid% = CInt(item("vorgangsid"))
            Dim vid$ = item("FREMDVORGANGSID").ToString
            Dim koppelingsid As Integer = CInt(item("ID").ToString)
            ' dgVerwandte.SelectedItem = Nothing
            'dgVerwandte.SelectedIndex = 0
            Verwandte_verarbeiten(koppelingsid, CInt(vid$), item("Titel").ToString)
        Catch ex As Exception
            nachricht("Sie haben in eine leere Zeile geklickt. Bitte versuchen Sie es nochmal." & ex.ToString)
        End Try
    End Sub

    Sub Verwandte_verarbeiten(ByVal kopplungsid%, ByVal verwandteID%, ByVal verText$)
        Dim neuVerwandteManager As New winVErwandte(kopplungsid%, verwandteID%, verText$)

        neuVerwandteManager.ShowDialog()
        schlagworteWurdeGeaendert = False
        geaenderteStammdatenSpeichern()

        schlagworteWurdeGeaendert = False ' sonst werden die Stammdaten in der closing routine gelöscht!!!
        'returncode = 1 'wechseln zum verw
        'returncode = 2 'löschen des verwandten
        'returncode = 3 'abbruch keine verwantenaktion
        'returncode = 4 'kopieren
        Select Case neuVerwandteManager.returncode
            Case "", "abbruch" '0, 3
                'keine aktion
            Case "wechseln" '1

                Me.Close()
                glob2.editVorgang(CInt(myGlobalz.sitzung.aktVorgangsID))
            Case "loeschen" '2
                'löschen wurde schon im formular winVErwandte ausgeführt
                'daher keine aktion
            Case "kopieren" '4
                refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
            Case Else
                'keine aktion
        End Select
        'If CBool(neuVerwandteManager.DialogResult) Then
        '    '   btnAllgemein.IsEnabled = False
        '    Me.Close()
        '    glob2.editVorgang(CInt(myGlobalz.sitzung.VorgangsID))
        'Else
        '    refreshverwandte(myGlobalz.sitzung.VorgangsID)
        'End If
    End Sub



    Private Sub cmbDokuverwandte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbDokuverwandte.SelectedItem Is Nothing Then Exit Sub
        If cmbDokuverwandte.SelectedValue Is Nothing Then Exit Sub
        refreshDokumente(CInt(cmbDokuverwandte.SelectedValue.ToString))
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub resetDokuliste_ClickExtracted()
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        cmbDokuverwandte.SelectedItem = Nothing
    End Sub
    Private Sub resetDokuliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        resetDokuliste_ClickExtracted()
        e.Handled = True
    End Sub

    Private Sub resetVerlaufliste_ClickExtracted()
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        cmbVerlaufVerwandte.SelectedItem = Nothing
    End Sub
    Private Sub resetVerlaufliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        resetVerlaufliste_ClickExtracted()
    End Sub

    Private Sub cmbVerlaufverwandte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbVerlaufVerwandte.SelectedItem Is Nothing Then Exit Sub
        If cmbVerlaufVerwandte.SelectedValue Is Nothing Then Exit Sub
        nachricht("USERAKTION: verwandte aktivieren")
        refreshEreignisseListe(CInt(cmbVerlaufVerwandte.SelectedValue.ToString))
    End Sub

    Private Sub cmbBeteiligteverwandte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbBeteiligteVerwandte.SelectedItem Is Nothing Then Exit Sub
        If cmbBeteiligteVerwandte.SelectedValue Is Nothing Then Exit Sub
        refreshBeteiligteListe(CInt(cmbBeteiligteVerwandte.SelectedValue.ToString))
        e.Handled = True
    End Sub

    Private Sub resetBeteiligteliste_ClickExtracted()
        refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        cmbBeteiligteVerwandte.SelectedItem = Nothing
    End Sub
    'Private Sub resetBeteiligteliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    resetBeteiligteliste_ClickExtracted()
    'End Sub

    Private Sub cmbRBverwandte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbRBVerwandte.SelectedItem Is Nothing Then Exit Sub
        If cmbRBVerwandte.SelectedValue Is Nothing Then Exit Sub
        refreshRaumbezugsListe(CInt(cmbRBVerwandte.SelectedValue.ToString), True)
    End Sub

    Private Sub resetRBliste_ClickExtracted()
        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
        cmbRBVerwandte.SelectedItem = Nothing
    End Sub
    Private Sub resetRBliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        resetRBliste_ClickExtracted()
        e.Handled = True
    End Sub

    Private Sub refreshRaumbezugsListe(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        resetRBliste_ClickExtracted()
        e.Handled = True
    End Sub

    Private Sub copyRBListe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If cmbRBVerwandte.SelectedValue Is Nothing Then
            MessageBox.Show("Sie müssen zuerst einen verwandten Vorgang auswählen!", "Daten aus verwandtem Vorgang übernehmen", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        Dim quellVid% = CInt(cmbRBVerwandte.SelectedValue.ToString)
        If RBtoolsns.KopierenVonRaumbezuegen_alleDB.exe(quellVid, myGlobalz.sitzung.aktVorgangsID) Then
        End If
        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
        e.Handled = True
    End Sub

    Sub berechneGlobalRange(ByVal globalrange As LibGISmapgenerator.clsRange)
        nachricht("berechneGlobalRange ----------------------------")
        '470531, 503699, 5532582, 5553740
        globalrange.xl = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Xmin")) ' myGlobalz.GIS_Rahmen_Fit_Xmin
        globalrange.yl = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Ymin")) 'myGlobalz.GIS_Rahmen_Fit_Ymin
        globalrange.xh = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Xmax")) 'myGlobalz.GIS_Rahmen_Fit_Xmax
        globalrange.yh = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_ymax")) 'myGlobalz.GIS_Rahmen_Fit_ymax
        nachricht("berechneGlobalRange ---------------ende -------------")
    End Sub

    Sub INITMiniMapPresentation(ByVal mydt As DataTable) 'myGlobalz.sitzung.raumbezugsRec.dt
        nachricht("INITMiniMapPresentation ----------------------------")
        Try
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            RBtoolsns.berechneRaumbezugsrange.execute(CLstart.myc.raumberange, mydt)
            berechneGlobalRange(CLstart.myc.globalrange)
            nachricht("lesecanvaasize  --------------------anfang --------")
            CLstart.myc.kartengen.aktMap.aktcanvas.w = CLng(myCanvas.Width)
            CLstart.myc.kartengen.aktMap.aktcanvas.h = CLng(myCanvas.Height)
            nachricht("lesecanvaasize  --------------------ende --------")
            clsMiniMapTools.initMinimapParameter(CLstart.myc.globalrange, CLstart.myc.raumberange)
            setzeVordergrundThemaUndRefresh("realshapeopak;")
            nachricht("INITMiniMapPresentation -------------------ende ---------")
        Catch ex As Exception
            nachricht("Fehler in INITMiniMapPresentation: " & ex.ToString)
        End Try
    End Sub

    Sub alte_gridsloeschen()
        nachricht("gridsloeschen")
        dgRaumbezug.DataContext = Nothing
        dgEreignisse.DataContext = Nothing
        dgVorgangDokumente.DataContext = Nothing
        dgBeteiligte.DataContext = Nothing
    End Sub


    Private Sub SichtbarkeitGISKnopf()
        nachricht("SichtbarkeitGISKnopf not implemented")

    End Sub

    Private Sub copyBeteiligteListe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If cmbBeteiligteVerwandte.SelectedValue Is Nothing Then
            MessageBox.Show("Sie müssen zuerst einen verwandten Vorgang auswählen!", "Daten aus verwandtem Vorgang übernehmen", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        'MsgBox("Baustelle")
        Dim quellVid% = CInt(cmbBeteiligteVerwandte.SelectedValue.ToString)
        '  clsBeteiligteBUSI.verlinkenVonBeteiligten(quellVid, myGlobalz.sitzung.VorgangsID)
        If vid_istOk(quellVid) Then detailsTools.AlleBeteiligtenKopieren(quellVid, myGlobalz.sitzung.aktVorgangsID) ' myGlobalz.sitzung.beteiligteREC.dt
        refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub




    Private Sub globalfit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        CLstart.myc.kartengen.aktMap.aktrange.rangekopierenVon(CLstart.myc.kartengen.FitGlobal)
        setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
        e.Handled = True
    End Sub

    Private Sub rbfit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim erfolg As Boolean = clsMiniMapTools.killMapCookie(myGlobalz.sitzung.aktVorgangsID)
        INITMiniMapPresentation(myGlobalz.sitzung.raumbezugsRec.dt)
        '  setBoundingRefresh(clstart.myc.kartengen.FitRaumbezuege)
        e.Handled = True
    End Sub

    Sub setzeVordergrundThemaUndRefresh(ByVal thema As String)
        nachricht("setzeVordergrundThemaUndRefresh: ---------------------- ")
        Try
            clsMiniMapTools.setzeVHgrund(thema)
            Dim erfolg As Boolean = clsMiniMapTools.getMapCookie_alleDB(CLstart.myc.kartengen.aktMap, myGlobalz.sitzung.aktVorgangsID)
            ' clsMiniMapTools.addiereAktvorgang(clstart.myc.kartengen.aktMap, myGlobalz.sitzung.VorgangsID)
            setzeHintergrundTextInMiniMap()
            presentMap()
        Catch ex As Exception
            nachricht("Fehler in setzeVordergrundThemaUndRefresh: " & ex.ToString)
        End Try
    End Sub

    Sub setBoundingRefresh(ByVal myrange As LibGISmapgenerator.clsRange) 'ByVal xl As Double, ByVal xh As Double, ByVal yl As Double, ByVal yh As Double)
        CLstart.myc.kartengen.aktMap.aktrange.rangekopierenVon(myrange)
        'xdifKorrektur
        If CLstart.myc.kartengen.aktMap.aktrange.xdif() < 1 Then CLstart.myc.kartengen.aktMap.aktrange.xh += 1
        If CLstart.myc.kartengen.aktMap.aktrange.ydif() < 1 Then CLstart.myc.kartengen.aktMap.aktrange.yh += 1
        presentMap()
    End Sub

    Sub presentMap()
        Dim cachedir As String = initP.getValue("GisServer.gisCacheDir")
        If initP.getValue("MiniMap.MapStatus") = "nomap" Then
            setNomapBitmap()
            Exit Sub
        End If

        If myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count < 1 Then
            'setNomapBitmap()
            'Exit Sub
        End If
        nachricht("presentMap: ---------------------- ")
        Try
            drawprogressbar()
            'clsMiniMap.korrigiereAktrange(clstart.myc.kartengen.aktMap.aktrange, myCanvas)
            Dim pixcanvas As New clsCanvas
            pixcanvas.w = CLng(myCanvas.Width)
            pixcanvas.h = CLng(myCanvas.Height)
            Dim handle As LIBmapScaling.clsScalierung
            handle = New LIBmapScaling.clsScalierung
            nachricht("presentMap: vor skaliereung ")
            LIBmapScaling.clsScalierung.Skalierung(72, "ZB", 1, CLstart.myc.kartengen.aktMap.aktrange, CInt(myCanvas.Width), CInt(myCanvas.Height), 1, CLstart.myc.kartengen.aktMap.aktrange, pixcanvas)
            nachricht("presentMap: nach skaliereung ")

            If Not LibGISmapgenerator.clsAufrufgenerator.istCacheOK(cachedir) Then Exit Sub
            CLstart.myc.kartengen.mapcred.DateinamensSchwanz = clsMiniMapTools.makeOutfileschwanz
            CLstart.myc.kartengen.mapcred.username = myGlobalz.sitzung.aktBearbeiter.username
            CLstart.myc.kartengen.genOutfileFullName(cachedir, ".png")
            ' webmapStream(aufruf$)
            glob2.webmapBrows(CLstart.myc.kartengen.genaufruf)
            CreateNewFileSystemWatcherAndSetItsProperties()
            Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myGlobalz.sitzung.aktVorgangsID)
            nachricht("presentMap: ---------------------- ")
        Catch ex As Exception
            nachricht_und_Mbox("fehler in presentMap: ---------------------- " & ex.ToString)
        End Try
    End Sub



    Private Sub CreateNewFileSystemWatcherAndSetItsProperties()
        nachricht("CreateNewFileSystemWatcherAndSetItsProperties --------------------------")
        Try
            ' Create a new FileSystemWatcher and set its properties.
            Dim test As New IO.FileInfo(CLstart.myc.kartengen.gifKartenDateiFullName)
            nachricht("clstart.myc.kartengen.gifKartenDateiFullName :" & CLstart.myc.kartengen.gifKartenDateiFullName)
            gifKartenwatcher = New FileSystemWatcher
            gifKartenwatcher.Path = test.DirectoryName
            ' Watch for changes in LastAccess and LastWrite times, and            ' the renaming of files or directories.
            gifKartenwatcher.NotifyFilter = (NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.DirectoryName)
            gifKartenwatcher.Filter = test.Name
            AddHandler gifKartenwatcher.Changed, AddressOf OnChangedFileSystemCacheGIS
            ' Begin watching.
            gifKartenwatcher.EnableRaisingEvents = True
            nachricht("CreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------")
        Catch ex As Exception
            nachricht("Fehler in : CreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------" & ex.ToString)
        End Try
    End Sub

    Public Sub NotifyUIThreadOfChange(ByVal e As FileSystemEventArgs)
        nachricht("NotifyUIThreadOfChange --------------------------")
        Try
            myCanvas.Children.Clear()
            ' drawimageincanvas()
            DrawRectangle(myCanvas)
            refreshBitmap()
            clsMiniMapTools.refreshEllipsen(myCanvas, CLstart.myc.kartengen.aktMap.aktrange, ableitungskreis)
            Dim istIMAPVorhanden As Boolean = clsMiniMapTools.imageMapDateiVorhanden(CLstart.myc.kartengen.imagemapDateifullname)
            If istIMAPVorhanden Then
                myGlobalz.mapEigentuemerAktiv = True
                tbAktiveEbene.Text = "Aktiv: " & glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.ActiveLayer)
                If myGlobalz.mapEigentuemerAktiv Then
                    ' clstart.myc.kartengen.imagemapDateifullname=clsMiniMapTools.imageMap2Eigentuemermap(clstart.myc.kartengen.imagemapDateifullname)
                    clsMiniMapTools.imageMap2PolygonMap(CLstart.myc.kartengen.imagemapDateifullname, myCanvas)
                Else
                    clsMiniMapTools.imageMap2PolygonMap(CLstart.myc.kartengen.imagemapDateifullname, myCanvas)
                End If
            End If

            tbAktiveEbene.Text = "Aktiv: " & glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.ActiveLayer)

            gifKartenwatcher.Dispose()
            'ProgressBar1.Visibility=Windows.Visibility.Collapsed
            nachricht("NotifyUIThreadOfChange --------------------------")
        Catch ex As Exception
            nachricht("Fehler in : NotifyUIThreadOfChange ---------ende-----------------" & ex.ToString)
        End Try
    End Sub

    'Private Sub drawimageincanvas()
    '    nachricht("Fehler in : drawimageincanvas ---------anfang-----------------")
    '    Try
    '        Dim myimage As Image = New Image
    '        myimage.Name = "imgKarte"
    '        myimage.Width = myCanvas.Width
    '        myimage.Height = myCanvas.Height
    '        'myCanvas.Children.Insert(0, myimage)
    '        myCanvas.Children.Add(myimage)
    '        Canvas.SetZIndex(myimage, 10)
    '    Catch ex As Exception
    '        nachricht("Fehler in : drawimageincanvas ---------ende-----------------" & ex.ToString)
    '    End Try
    'End Sub

    Private Sub DrawRectangle(ByVal mycanvas As Canvas)
        rubberbox = New Rectangle
        rubberbox.Name = "rubberbox"
        Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 0, 250)) 'transparenz ist der erste wert
        rubberbox.Stroke = Brushes.Black
        rubberbox.StrokeThickness = 2
        rubberbox.Opacity = 90
        rubberbox.Fill = myBrush
        Panel.SetZIndex(rubberbox, 100)
        Canvas.SetZIndex(rubberbox, 100)
        mycanvas.Children.Add(rubberbox)
        Panel.SetZIndex(rubberbox, 100)
        Canvas.SetZIndex(rubberbox, 100)
    End Sub
    Sub drawprogressbar()
        nachricht("drawprogressbar: ---------------------- ")
        Try
            '<ProgressBar Canvas.Left="95" Canvas.Top="1" Height="10" Name="ProgressBar1" Width="100" Panel.ZIndex="100000"   Visibility="Collapsed"/>
            Dim myprogressbar As ProgressBar = New ProgressBar
            myprogressbar.Name = "ProgressBar1"
            myprogressbar.Width = 100
            myprogressbar.Height = 10
            myprogressbar.IsIndeterminate = True
            'myCanvas.Children.Insert(0, myimage)
            myCanvas.Children.Add(myprogressbar)
            Canvas.SetZIndex(myprogressbar, 1000)
            Canvas.SetLeft(myprogressbar, 95)
            Canvas.SetTop(myprogressbar, 1)
        Catch ex As Exception
            nachricht("fehler in drawprogressbar " & ex.ToString)
        End Try
        nachricht("drawprogressbar: ----------ende------------ ")
    End Sub

    Private Sub OnChangedFileSystemCacheGIS(ByVal source As Object, ByVal e As FileSystemEventArgs)
        Try
            'Call back to the UI thread
            nachricht("OnChangedFileSystemCacheGIS --------------------------vor invoke")
            Me.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                      New watcherCallBackToUIThread(AddressOf NotifyUIThreadOfChange), e)
            gifKartenwatcher.Dispose()
            nachricht("OnChangedFileSystemCacheGIS -------------ende-------------")
        Catch ex As Exception
            nachricht("fehler in OnChangedFileSystemCacheGIS " & ex.ToString)
        End Try
    End Sub


    Public Sub setNomapBitmap()
        Try
            myCanvas.Background = Nothing
            Dim showup As String = initP.getValue("Haupt.paradigmaDateiServerRoot") & "\div\showup\default.jpg"
            Dim test As New IO.FileInfo(showup)
            If Not test.Exists Then
                nachricht("Die Grafik kann nicht gefunden werden!" & showup)
                Exit Sub
            Else
                nachricht("Die Grafik wurde gefunden !" & showup)
            End If
            Dim myBitmapImage As New BitmapImage()
            myBitmapImage.BeginInit()
            myBitmapImage.UriSource = New Uri(showup)
            '		myBitmapImage.DecodePixelWidth = 200
            myBitmapImage.EndInit()
            Dim brusch As New ImageBrush
            brusch.ImageSource = myBitmapImage
            myCanvas.Background = brusch ' myBitmapImage	 
            'scalieren
            If myBitmapImage.Width > myCanvas.Width Then
                myCanvas.Height = myCanvas.Height * (myCanvas.Width / myBitmapImage.Width)
            End If
            If myBitmapImage.Height > myCanvas.Height Then
                'myCanvas.Width = myCanvas.Width * (myCanvas.Height / myBitmapImage.Width)
            End If

            myCanvas.ToolTip = "Es sind noch keine Raumbezüge zum Vorgang erfasst worden. Bitte ergänzen Sie mind. einen Raumbezug."
            '   imgKarte.Source = myBitmapImage
            '  myBitmapImage = Nothing
            brusch = Nothing
        Catch ex As Exception
            nachricht_und_Mbox("Datei war noch nicht freigegeben. Bitte nochmal versuchen!") 'ex.ToString)
        End Try
    End Sub

    Public Sub refreshBitmap()
        Try
            myCanvas.Background = Nothing
            System.Threading.Thread.Sleep(900)
            'Dim neucanvas As New Windows.Controls.Canvas
            'neucanvas = myCanvas
            Dim test As New IO.FileInfo(CLstart.myc.kartengen.gifKartenDateiFullName)
            If Not test.Exists Then
                nachricht("Die Grafik kann nicht gefunden werden!" & CLstart.myc.kartengen.gifKartenDateiFullName)
                Exit Sub
            Else
                nachricht("Die Grafik wurde gefunden !" & CLstart.myc.kartengen.gifKartenDateiFullName)
            End If
            Dim myBitmapImage As New BitmapImage()
            myBitmapImage.BeginInit()
            myBitmapImage.UriSource = New Uri(CLstart.myc.kartengen.gifKartenDateiFullName)
            '		myBitmapImage.DecodePixelWidth = 200
            myBitmapImage.EndInit()
            Dim brusch As New ImageBrush
            brusch.ImageSource = myBitmapImage
            myCanvas.Background = brusch ' myBitmapImage	 
            '   imgKarte.Source = myBitmapImage
            '  myBitmapImage = Nothing
            brusch = Nothing
        Catch ex As Exception
            nachricht_und_Mbox("Datei war noch nicht freigegeben. Bitte nochmal versuchen!") 'ex.ToString)
        End Try
    End Sub

    Private Sub zoomin_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'neue range berechnen
        'darstellen
        Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
        CLstart.myc.kartengen.aktMap.aktrange.xl = CLstart.myc.kartengen.aktMap.aktrange.xl + (breite / 3)
        CLstart.myc.kartengen.aktMap.aktrange.xh = CLstart.myc.kartengen.aktMap.aktrange.xh - (breite / 3)
        Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()
        CLstart.myc.kartengen.aktMap.aktrange.yl = CLstart.myc.kartengen.aktMap.aktrange.yl + (hohe / 3)
        CLstart.myc.kartengen.aktMap.aktrange.yh = CLstart.myc.kartengen.aktMap.aktrange.yh - (hohe / 3)
        presentMap()
        e.Handled = True
    End Sub

    Private Sub zoomout_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
        CLstart.myc.kartengen.aktMap.aktrange.xl = CLstart.myc.kartengen.aktMap.aktrange.xl - (breite / 3)
        CLstart.myc.kartengen.aktMap.aktrange.xh = CLstart.myc.kartengen.aktMap.aktrange.xh + (breite / 3)
        Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()
        CLstart.myc.kartengen.aktMap.aktrange.yl = CLstart.myc.kartengen.aktMap.aktrange.yl - (hohe / 3)
        CLstart.myc.kartengen.aktMap.aktrange.yh = CLstart.myc.kartengen.aktMap.aktrange.yh + (hohe / 3)
        presentMap()
        e.Handled = True
    End Sub

    Private Sub btnAuschnitt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim test As String = CLstart.myc.userIniProfile.WertLesen("Minimap", "Ausschnitt_info")
        If String.IsNullOrEmpty(test) OrElse test = "1" Then
            MsgBox("Wählen sie den Ausschnitt in der Karte (Maustaste drücken und ziehen, loslassen)")
        End If
        CLstart.myc.userIniProfile.WertSchreiben("Minimap", "Ausschnitt_info", "0")
        CanvasClickModus = "Ausschnitt"
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
        DrawRectangle(myCanvas)
        e.Handled = True
    End Sub

    Private Sub Links_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
        CLstart.myc.kartengen.aktMap.aktrange.xl = CLstart.myc.kartengen.aktMap.aktrange.xl - (breite / 3)
        CLstart.myc.kartengen.aktMap.aktrange.xh = CLstart.myc.kartengen.aktMap.aktrange.xh - (breite / 3)
        presentMap()
        e.Handled = True
    End Sub

    Private Sub Unten_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()
        CLstart.myc.kartengen.aktMap.aktrange.yl = CLstart.myc.kartengen.aktMap.aktrange.yl - (hohe / 3)
        CLstart.myc.kartengen.aktMap.aktrange.yh = CLstart.myc.kartengen.aktMap.aktrange.yh - (hohe / 3)
        presentMap()
        e.Handled = True
    End Sub

    Private Sub Oben_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()
        CLstart.myc.kartengen.aktMap.aktrange.yl = CLstart.myc.kartengen.aktMap.aktrange.yl + (hohe / 3)
        CLstart.myc.kartengen.aktMap.aktrange.yh = CLstart.myc.kartengen.aktMap.aktrange.yh + (hohe / 3)
        presentMap()
        e.Handled = True
    End Sub

    Private Sub Rechts_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
        CLstart.myc.kartengen.aktMap.aktrange.xl = CLstart.myc.kartengen.aktMap.aktrange.xl + (breite / 3)
        CLstart.myc.kartengen.aktMap.aktrange.xh = CLstart.myc.kartengen.aktMap.aktrange.xh + (breite / 3)
        presentMap()
        e.Handled = True
    End Sub

    Private Sub myCanvas_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles myCanvas.MouseLeftButtonDown

        Select Case CanvasClickModus
            Case "Ausschnitt"
                If btnAusschnitt.IsChecked Then RubberbandStart(e)
        End Select
    End Sub

    Private Sub canvas1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles myCanvas.MouseMove
        Select Case CanvasClickModus
            Case "Ausschnitt"
                If btnAusschnitt.IsChecked Then RubberbandMove(e)
        End Select
    End Sub

    Private Sub canvas1_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles myCanvas.MouseLeftButtonUp
        Select Case CanvasClickModus
            Case "Ausschnitt"
                If btnAusschnitt.IsChecked Then RubberbandFinish()
                CanvasClickModus = ""
            Case "Koordinate"
                Me.Cursor = Nothing
                Mouse.Capture(Nothing)
                clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
                KoordinateKLickpt = e.GetPosition(myCanvas)
                ' MsgBox("u p " & KoordinateKLickpt.ToString)
                CanvasClickModus = ""
                koordinateKlickBerechnen(KoordinateKLickpt)
                If KoordinateAlsNeuenRaumbezugAnbieten() Then
                    myGlobalz.sitzung.raumbezugsmodus = "neu"
                    Dim winflur As New Win_punktMitUmkreis(tbMinimapCoordinate.Text)
                    winflur.ShowDialog()
                    If CBool(winflur.DialogResult) Then
                        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
                    End If
                End If
            Case "ZWERT"
                Me.Cursor = Nothing
                Mouse.Capture(Nothing)
                clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
                KoordinateKLickpt = e.GetPosition(myCanvas)
                ' MsgBox("u p " & KoordinateKLickpt.ToString)
                CanvasClickModus = ""
                MsgBox("Die Wartezeit kann bis zu 5 Min. dauern. Solange ist Paradigma blockiert!")
                LaserScan.mod3dtools.koordinateKlickBerechnenZWERT(KoordinateKLickpt)
                'If KoordinateAlsNeuenRaumbezugAnbieten() Then
                '    myGlobalz.sitzung.raumbezugsmodus = "neu"
                '    Dim winflur As New Win_punktMitUmkreis(tbMinimapCoordinate.Text)
                '    winflur.ShowDialog()
                '    If CBool(winflur.DialogResult) Then
                '        refreshRaumbezugsListe(myGlobalz.sitzung.VorgangsID, False)
                '    End If
                'End If
            Case "Ableitungskreis"
                Me.Cursor = Nothing
                Mouse.Capture(Nothing)
                clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
                KoordinateKLickpt = e.GetPosition(myCanvas)
                ' MsgBox("u p " & KoordinateKLickpt.ToString)
                CanvasClickModus = ""
                '   koordinateKlickBerechnen(KoordinateKLickpt)
                ableitungskreis.punktUTM = getkoordinateKlickBerechnen(KoordinateKLickpt)
                ableitungskreis.punktScreen = CType(KoordinateKLickpt, Point)

                clsMiniMapTools.refreshEllipsen(myCanvas, CLstart.myc.kartengen.aktMap.aktrange, ableitungskreis)

                '  ableitungskreis.punktUTM.GKhoch = getkoordinateKlickBerechnen(KoordinateKLickpt).Y
                'If KoordinateAlsNeuenRaumbezugAnbieten() Then
                '    myGlobalz.sitzung.raumbezugsmodus = "neu"
                '    Dim winflur As New Win_punktMitUmkreis(tbMinimapCoordinate.Text)
                '    winflur.ShowDialog()
                '    If CBool(winflur.DialogResult) Then
                '        refreshRaumbezugsListe(myGlobalz.sitzung.VorgangsID)
                '    End If
                'End If
            Case "Flaeche"
                Dim tempPT As Point? = Nothing
                Dim winpt As New Point
                tempPT = e.GetPosition(myCanvas)
                winpt.X = tempPT.Value.X
                winpt.Y = tempPT.Value.Y
                myGlobalz.sitzung.aktPolygon.myPoly.Points.Add(winpt)
                myPolyVertexCount% += 1

            Case "Strecke"
                Dim tempPT As Point? = Nothing
                Dim winpt, utmpt As New Point
                Dim delim As String = ";"
                tempPT = e.GetPosition(myCanvas)
                winpt.X = tempPT.Value.X
                winpt.Y = tempPT.Value.Y

                myGlobalz.sitzung.aktPolyline.myLine.Points.Add(winpt)

                utmpt.X = (clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(winpt).X)
                utmpt.Y = (clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(winpt).Y)

                myGlobalz.sitzung.aktPolyline.Distanz = myGlobalz.sitzung.aktPolyline.Distanz +
                    PolygonTools.calcDistanz(utmpt, myGlobalz.sitzung.aktPolyline.alterPunkt)
                myGlobalz.sitzung.aktPolyline.GKstring = myGlobalz.sitzung.aktPolyline.GKstring &
                  CDbl(utmpt.X) & delim & CDbl(utmpt.Y) & delim




                myGlobalz.sitzung.aktPolyline.alterPunkt.X = utmpt.X
                myGlobalz.sitzung.aktPolyline.alterPunkt.Y = utmpt.Y
                tbMinimapLinie.Text = myGlobalz.sitzung.aktPolyline.Distanz.ToString("########.##")
                myPolyVertexCount += 1

                '   If btnAusschnitt.IsChecked Then myPolyFinish()
                '   CanvasClickModus = ""
        End Select
    End Sub

    Function FlaecheAlsNeuenRaumbezugAnbieten(Text As String) As Boolean
        Dim res As New MessageBoxResult
        res = MessageBox.Show(
                    "Möchten Sie die " & Text & " als neuen Raumbezug übernehmen ? " & vbCrLf & _
                    "  " & vbCrLf, "Neuer Raumbezug '" & Text & "' ?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Cancel)
        Return If(res = MessageBoxResult.No, False, True)
    End Function

    Function KoordinateAlsNeuenRaumbezugAnbieten() As Boolean
        Dim res As New MessageBoxResult
        res = MessageBox.Show(
                    "Möchten Sie die Koordinate als neuen Raumbezug übernehmen ? " & vbCrLf &
                    "  ", "Neuer Raumbezug 'Punkt mit Umkreis' ?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Cancel)
        Return If(res = MessageBoxResult.No, False, True)
    End Function

    Sub koordinateKlickBerechnen(ByVal KoordinateKLickpt As Point?)
        Dim newpoint2 As New LibGISmapgenerator.myPoint
        Dim newpoint As New LibGISmapgenerator.myPoint
        newpoint2.X = CDbl(KoordinateKLickpt.Value.X)
        newpoint2.Y = CDbl(KoordinateKLickpt.Value.Y)
        newpoint = LibGISmapgenerator.clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(newpoint2, CLstart.myc.kartengen.aktMap.aktrange, CLstart.myc.kartengen.aktMap.aktcanvas)
        newpoint.SetToInteger()
        'Dim zwert As Double = detailsTools.getZWert(newpoint)
        tbMinimapCoordinate.Text = newpoint.toString
        newpoint2 = Nothing
        newpoint = Nothing
    End Sub



    Function getkoordinateKlickBerechnen(ByVal KoordinateKLickpt As Point?) As LibGISmapgenerator.myPoint
        Dim newpoint2 As New LibGISmapgenerator.myPoint
        Dim newpoint As New LibGISmapgenerator.myPoint
        newpoint2.X = CDbl(KoordinateKLickpt.Value.X)
        newpoint2.Y = CDbl(KoordinateKLickpt.Value.Y)
        newpoint = LibGISmapgenerator.clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(newpoint2, CLstart.myc.kartengen.aktMap.aktrange, CLstart.myc.kartengen.aktMap.aktcanvas)
        '  newpoint.SetToInteger()
        Return newpoint
        'tbMinimapCoordinate.Text = newpoint.toString
        'newpoint2 = Nothing
        'newpoint = Nothing
    End Function

    Private Sub Window1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown
        If e.Key = Key.Escape Then canvas1_MouseLeftButtonUp(sender, Nothing)
    End Sub

    Private Sub RubberbandFinish()
        btnAusschnitt.IsChecked = False
        clsMiniMapTools.ausschnittNeuBerechnen(RubberbandStartpt, RubberbandEndpt)
        setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
        rubberbox.Width = 0
        rubberbox.Height = 0
        Mouse.Capture(Nothing)
        RubberbandStartpt = Nothing
        RubberbandEndpt = Nothing
        Me.Cursor = Nothing
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    End Sub

    Private Sub RubberbandMove(ByVal e As System.Windows.Input.MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And RubberbandStartpt.HasValue Then
            Dim endpt = e.GetPosition(myCanvas)
            RubberbandEndpt = endpt
            Dim x0, y0, w, h As Double
            x0 = Math.Min(RubberbandStartpt.Value.X, endpt.X)
            y0 = Math.Min(RubberbandStartpt.Value.Y, endpt.Y)
            w = Math.Abs(endpt.X - RubberbandStartpt.Value.X)
            h = Math.Abs(endpt.Y - RubberbandStartpt.Value.Y)
            Canvas.SetLeft(rubberbox, x0)
            Canvas.SetTop(rubberbox, y0)
            Canvas.SetZIndex(rubberbox, 1000)
            rubberbox.Width = w
            rubberbox.Height = h
        End If
    End Sub

    Private Sub RubberbandStart(ByVal e As System.Windows.Input.MouseButtonEventArgs)
        RubberbandStartpt = e.GetPosition(myCanvas)
        Me.Cursor = Cursors.Cross
    End Sub

    Private Sub refreshminimap(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        INITMiniMapPresentation(myGlobalz.sitzung.raumbezugsRec.dt)
        e.Handled = True
    End Sub

    Private Sub geaenderteStammdatenSpeichern()
        If schlagworteWurdeGeaendert Then
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "BEMERKUNG")
            schlagworteWurdeGeaendert = False
        End If
        If detailsTools.letztesEreignisWurdeGeaendert Then
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LETZTEBEARBEITUNG")
        End If
        If detailsTools.stellungnahmeWurdeGeaendert Then
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "STELLUNGNAHME")
        End If
    End Sub

    Private Sub Window_Detail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim test As Boolean = detailsTools.wurdenDokumenteGeaendert(myGlobalz.sitzung.checkoutDokuList)
        geaenderteStammdatenSpeichern()
        setMitDokumenten()
        detailsTools.VorgangLocking("aus")
        clsVorgangLocking.alleLocksDesUsersLoesen()
        Dim mesres As New MessageBoxResult
        If test Then
            mesres = MessageBox.Show("Ein Dokument wurde geändert. Die Änderungen werden verloren gehen, wenn Sie nicht zuerst das Dokument speichern und schliessen." &
                            "" & vbCrLf & vbCrLf &
                            "Möchten Sie die Änderungen am Dokument verwerfen ? " & vbCrLf &
                            "Ja   -  Änderungen verwerfen" & vbCrLf & vbCrLf &
                            "Nein - Im Vorgang bleiben",
                            "Dokument wurde geändert", MessageBoxButton.YesNo, MessageBoxImage.Error)
            If mesres = MessageBoxResult.Yes Then
                e.Cancel = False
            Else
                e.Cancel = True
            End If
        Else

        End If
        myGlobalz.einVorgangistgeoeffnet = False
        nachricht("Dokument wurde geändert?: " & test.ToString)
        detailsTools.VorgangLocking("aus")
        CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(myGlobalz.sitzung.aktVorgangsID.ToString,
                                                myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung,
                                                myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt,
                                                myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz,
                                                myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

        If gifKartenwatcher IsNot Nothing Then
            gifKartenwatcher.Dispose()
        End If
        ' detailsTools.hatraumbezugDarstellen()
        Try
            CLstart.myc.aLog.endlog()
        Catch ex As Exception
            'tritt auf, wenn vorgangsnummer gar nicht existiert
        End Try
    End Sub

    Private Sub EreignisExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles EreignisExcel.Click
        e.Handled = True
        nachricht("USERAKTION:   ereignis zu excel")
        detailsTools.EreignisExcel_ClickExtracted()
    End Sub

    Private Sub BeteiligteExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        detailsTools.BeteiligteExcel_ClickExtracted()
    End Sub


    Private Sub RaumbezugExcel_click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        detailsTools.RaumbezugExcel_clickExtracted()
    End Sub


    Private Sub DokumenteExcel_click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        detailsTools.DokumenteExcel_clickExtracted()
    End Sub

    Sub refreshverwandteServer(ByVal vid As Integer)
        Try
            '   txtTitel.Text = "Der aktuelle Vorgang wird von den aufgelisteten Vorgängen als <Verwandter Vorgang> verwendet:"
            Dim dientVorgangAlsServer As Boolean = detailsTools.fuelleVerwandteDT("select * from vorgang2fremdvorgang where fremdvorgangsid=" & vid)
            dgVerwandteServer.DataContext = myGlobalz.sitzung.VerwandteDTServer
            '   initCombosVerwandte()
            If dientVorgangAlsServer Then
                tabheaderVerwandte.Header = "Verwandte " & myGlobalz.sitzung.VerwandteDT.Rows.Count
                ' VerwandteGroupboxenEnabled(True)
                tabheaderVerwandte.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
                tabheaderVerwandte.SetValue(TextElement.ToolTipProperty, "Der aktuelle Vorgang wird von den aufgelisteten Vorgängen als <Verwandter Vorgang> verwendet")
                '	tabheaderVorlagen.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
            Else
                tabheaderVerwandte.Header = "Verwandte "
                VerwandteGroupboxenEnabled(False)
                tabheaderVerwandte.SetValue(TextElement.FontWeightProperty, FontWeights.Normal)
                tabheaderVerwandte.SetValue(TextElement.ToolTipProperty, "")
            End If
        Catch ex As Exception
            nachricht("fehler in refreshverwandteServer: " & ex.ToString)
        End Try
    End Sub



    'Private Sub Verlinkung_pruefen(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    refreshverwandteServer(myGlobalz.sitzung.aktVorgangsID)
    'End Sub

    Private Sub kurzdossier_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: dossier ")
        presDokusAusgewaehltMarkieren(True, Psession.presFotos)
        presDokusAusgewaehltMarkieren(True, Psession.presDokus)
        clsBerichte.fotobucherstellen(myCanvas, False,
                                      detail_dokuauswahl.dokListenMergen(Psession.presDokus, Psession.presFotos),
                                      CLstart.myc.kartengen.aktMap.aktrange)
        e.Handled = True
    End Sub

    'Private Sub btnFotoGucker_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    btnFotoGucker_ClickExtracted()
    'End Sub

    'Private Sub tbGEMKRZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbGEMKRZ.TextChanged
    '    glob2.istTextzulang(5, tbGEMKRZ)
    '    glob2.schliessenButton_einschalten(btnAllgemein)
    'End Sub




    Private Sub btnGetCoordinates_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: koordinate messen ")
        MsgBox("Wählen sie den Punkt in der Karte (Linke Maustaste drücken)")
        CanvasClickModus = "Koordinate"
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
        e.Handled = True
    End Sub

    Private Sub btnGetFlaeche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: fläche messen ")
        MsgBox("Wählen sie die Fläche in der Karte indem Sie die umgrenzenden Punkte anklicken (Linke Maustaste drücken)")
        CanvasClickModus = "Flaeche"
        btnGetFlaeche.IsEnabled = False
        btnGetFlaecheEnde.IsEnabled = True
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
        DrawPolygon(myCanvas)
        e.Handled = True
    End Sub

    Private Sub DrawPolygon(ByVal mycanvas As Canvas)
        myGlobalz.sitzung.aktPolygon.myPoly = New Polygon
        myGlobalz.sitzung.aktPolygon.myPoly.Name = "myPoly"
        Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 100, 250)) 'transparenz ist der erste wert
        myGlobalz.sitzung.aktPolygon.myPoly.Stroke = Brushes.DarkBlue
        myGlobalz.sitzung.aktPolygon.myPoly.StrokeThickness = 2
        myGlobalz.sitzung.aktPolygon.myPoly.Opacity = 90
        myGlobalz.sitzung.aktPolygon.myPoly.Fill = myBrush
        Panel.SetZIndex(myGlobalz.sitzung.aktPolygon.myPoly, 100)
        Canvas.SetZIndex(myGlobalz.sitzung.aktPolygon.myPoly, 100)
        mycanvas.Children.Add(myGlobalz.sitzung.aktPolygon.myPoly)
        Panel.SetZIndex(myGlobalz.sitzung.aktPolygon.myPoly, 100)
        Canvas.SetZIndex(myGlobalz.sitzung.aktPolygon.myPoly, 100)
        myPolyVertexCount% = 0
    End Sub

    Private Sub myPolyFinish(typ As String)
        btnAusschnitt.IsChecked = False
        If typ = "flaeche" Then
            myGlobalz.sitzung.aktPolygon.myPoly.Width = 0
            myGlobalz.sitzung.aktPolygon.myPoly.Height = 0
        End If
        If typ = "strecke" Then
            myGlobalz.sitzung.aktPolyline.myLine.Width = 0
            myGlobalz.sitzung.aktPolyline.myLine.Height = 0
        End If
        Mouse.Capture(Nothing)
        Me.Cursor = Nothing
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    End Sub

    Private Sub btnGetFlaecheEnde_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnGetFlaeche.IsEnabled = True
        btnGetFlaecheEnde.IsEnabled = False
        btnGetFlaecheEnde_ClickExtracted()
        e.Handled = True
    End Sub



    Private Sub btnGetFlaecheEnde_ClickExtracted()
        Dim anyChange As Boolean
        If myPolyVertexCount% > 2 Then
            If clsMiniMapTools.PolygonAufbereiten(myGlobalz.sitzung.aktPolygon) Then
                clsMiniMapTools.GK_FlaecheErmitteln()
            End If
            tbMinimapFlaeche.Text = CLng(myGlobalz.sitzung.aktPolygon.Area).ToString
            If FlaecheAlsNeuenRaumbezugAnbieten("Fläche") Then
                ' MsgBox("Baustelle")
                myGlobalz.sitzung.raumbezugsmodus = "neu"
                'myGlobalz.sitzung.aktPolygon.clear() sonst geht fläche verloren
                myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polygon
                Dim geoedit As New Win_Polygon("flaeche") 'myGlobalz.sitzung.aktPolygon.myPoly)
                geoedit.ShowDialog()
                anyChange = CBool(geoedit.DialogResult)
            End If
        Else
            MsgBox("Zu wenig Punkte für eine Flächenberechnung. Mind. 3 Punkte sind erforderlich!")
        End If
        myPolyFinish("flaeche")
        If anyChange Then refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
    End Sub
    Private Sub btnThemenauswahl(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: themen ändern ")
        starteThemenauswahl()
        e.Handled = True
    End Sub

    Private Sub starteThemenauswahl()
        Dim objResult As Boolean
        Dim themex As New WinThemen("maplayer_referenced")
        objResult = CBool(themex.ShowDialog())

        If objResult Then presentMap()
    End Sub


    Sub DragFeedback(ByVal e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effects = DragDropEffects.Move
            e.Handled = True
        Else
            e.Effects = DragDropEffects.None
            e.Handled = True
        End If
    End Sub

    Private Sub Window_Detail_PreviewDragEnter(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles Me.PreviewDragEnter
        DragFeedback(e)
    End Sub


    Private Sub Window_Detail_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles Me.Drop
        Dim filenames As String()

        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            filenames = CType(e.Data.GetData(DataFormats.FileDrop), String())
            If Not String.IsNullOrEmpty(filenames(0)) AndAlso
                filenames(0).ToLower.EndsWith(".eml") Then
                'thunderbird mails
                detailsTools.EMLemnailUebernehmen(filenames(0).ToLower, myGlobalz.sitzung.aktVorgangsID)
                refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
            Else
                If glob2.DokumentehinzuDragDrop(0, filenames) Then
                    refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                    refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                    refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
                End If
            End If
        ElseIf e.Data.GetDataPresent("FileGroupDescriptor") Then
            detailsTools.outlookemnailUebernehmen(myGlobalz.sitzung.aktVorgangsID)
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        End If
        e.Handled = True
    End Sub

    'Private Sub btnDokupruefer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    nachricht("Dokument wurde geändert?: " & detailsTools.wurdenDokumenteGeaendert(myGlobalz.sitzung.checkoutDokuList).ToString)
    '    'gbxDateiInBenutzung.Visibility = Windows.Visibility.Collapsed
    '    e.Handled = True
    'End Sub

    Private Sub setWeitereBearbeiterListeDarstellen()
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
            tbWeitereBearbeiter.Visibility = Visibility.Collapsed
        Else
            tbWeitereBearbeiter.Visibility = Visibility.Visible
        End If
    End Sub




    Private Sub btnHinzufuegenBeteiligte_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: beteiliten hinzu ")
        starteBeteiligtedetail()
        e.Handled = True
    End Sub


    Private Sub btnSTammChange_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: stammdaten ändern")
        Dim neustamm As New Win_Stamm("edit", myGlobalz.sitzung.aktVorgang.Stammdaten, "", "", "")
        neustamm.ShowDialog()
        ' MsgBox(myGlobalz.sitzung.Vorgang.Stammdaten)
        StammDatenAnzeigeAktualisieren()
        setzeErledigtflagfarbe()
    End Sub

    Private Sub StammDatenAnzeigeAktualisieren()
        tbAltAz.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.AltAz
        tbProbaugAZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz
        tbBeschreibung2.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung
        tbWeitereBearbeiter.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
        tbGEMKRZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ

    End Sub

    Private Sub setzeErledigtflagfarbe()
        Dim ffff As New Color
        If myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt Then
            lblVorgangsID.Background = New SolidColorBrush(Color.FromArgb(200, 0, 250, 0))
            Label16.Background = New SolidColorBrush(Color.FromArgb(200, 0, 250, 0))
        Else
            lblVorgangsID.Background = New SolidColorBrush(Color.FromArgb(0, 100, 100, 100))
            Label16.Background = New SolidColorBrush(Color.FromArgb(0, 100, 100, 100))
        End If
    End Sub




    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: wiedervorlage hinzufügen ")
        detailsTools.eEreignisstarten("wiedervorlage")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnOutlookemailuebernehmen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: outlookemail hinzufügen ")
        detailsTools.eEreignisstarten("outlookemail übernehmen")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnStandardereignis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: stdereignis hinzufügen ")
        detailsTools.eEreignisstarten("Notiz")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub


    Private Sub btnNachVorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: vorlage hinzufügen ")
        Dim vorl As New WinVorlagenListe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl,
                                         myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header,
                                         False)
        vorl.ShowDialog()
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnAktennotiz_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: aktennotiz hinzufügen ")
        detailsTools.eEreignisstarten("aktennotiz schreiben")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub


    Private Sub btnemailschreiben_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: emailschreiben ")
        detailsTools.eEreignisstarten("email schreiben")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnZahlung_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: zahlung hinzufügen ")
        detailsTools.eEreignisstarten("zahlung")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnStandardDokuHinzufuegen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: doku hinzufügen ")
        dokuhinzufuegenUndRefresh()
        e.Handled = True
    End Sub


    Private Sub btnPunktmitumkreis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: raumbezug hinzufügen punkt mit umkreis")
        raumbezugHinzufuegenUndRefresh("punkt mit umkreis")
        e.Handled = True
    End Sub

    Private Sub btnFlurstueck_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: raumbezug hinzufügen flurstück")
        raumbezugHinzufuegenUndRefresh("flurstück")
        e.Handled = True
    End Sub

    Private Sub btnAdresse_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: raumbezug hinzufügen adresse")
        raumbezugHinzufuegenUndRefresh("adresse")
        e.Handled = True
    End Sub

    Private Sub btnPolygon_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: raumbezug hinzufügen polygon")
        MsgBox("Bitte benutzen Sie hierzu die Funktion: Minimap-Fläche messen. " & vbCrLf &
       "Sobald Sie nach der Messung auf STOP drücken wird IHnen die Möglichkeit geboten die gemessene Fläche als Raumbezug zu übernehmen!", MsgBoxStyle.Information)
        e.Handled = True
    End Sub


    Private Sub btnbeteiligteHinzudirekt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: beteiligte hinzufügen ")
        starteBeteiligtedetail()
        e.Handled = True
    End Sub


    Private Sub btnVerlinkung_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: verwandte hinzufügen ")
        Verwandte_hinzufuegenExtracted()
        e.Handled = True
    End Sub

    'Private Sub btnallgVorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Dim vorl As New WinVorlagenListe()
    '    vorl.ShowDialog()
    '    refreshEreignisseListe(myGlobalz.sitzung.VorgangsID)
    '    refreshDokumente(myGlobalz.sitzung.VorgangsID)
    '    refreshFotos(myGlobalz.sitzung.VorgangsID)
    '    e.Handled = True
    'End Sub

    Private Sub Projekt_RefreshDetailEintrag(ByVal aktprojekt As CLstart.clsProjektAllgemein)
        tbProjektAbstract.Text = aktprojekt.bildeTextFuerDetails
        tbProjektAbstract.ToolTip = aktprojekt.bildeTooltipFuerDetails
    End Sub

    Private Sub btnProjekt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        aktprojektInit()
        e.Handled = True
    End Sub

    Private Sub aktprojektInit()
        CLstart.myc.aktprojekt = ProjektAufrufen(True)
        Projekt_RefreshDetailEintrag(CLstart.myc.aktprojekt)
        refreshProjekt(myGlobalz.sitzung.aktVorgangsID)
        Dim vorhanden As Boolean = NSprojekt.ladeProjekt.byProjektId(CLstart.myc.aktprojekt)
    End Sub
    Private Function ProjektAufrufen(ByVal anbieten As Boolean) As CLstart.clsProjektAllgemein
        CLstart.myc.aktprojekt = New CLstart.clsProjektAllgemein(myGlobalz.sitzung.aktVorgangsID)
        NSprojekt.Kopplung.byvorgangsidtId(CLstart.myc.aktprojekt)
        Dim vorhanden As Boolean = NSprojekt.ladeProjekt.byProjektId(CLstart.myc.aktprojekt)
        If Not vorhanden Then
            If Not anbieten Then Return CLstart.myc.aktprojekt
            'liste der projekte anzeigen frage NEU  oder auswählen
            If NSprojekt.ProjektAusgewaehlt.exe(CLstart.myc.aktprojekt) Then
                'Dim prj As New WinProjekt("edit", aktprojekt)
                'prj.ShowDialog()
                If NSprojekt.projektMitVorgangKoppeln.exe(CLstart.myc.aktprojekt) Then
                    MsgBox("Vorgang wurde mit Projekt verbunden!")
                Else
                    MsgBox("Es wurde versucht Vorgang und Projekt zu koppeln: nicht erfolgreich!")
                End If
            Else
                nachricht("Es wurde kein Projekt ausgewählt!")
            End If
        Else
            If Not anbieten Then Return CLstart.myc.aktprojekt
            ProjektVorhanden(CLstart.myc.aktprojekt)
        End If
        Return CLstart.myc.aktprojekt
    End Function

    Private Sub ProjektVorhanden(ByVal aktprojekt As CLstart.clsProjektAllgemein)
        'projekt anzeigen
        Dim prj As New WinProjekt("koppeln", aktprojekt)
        prj.ShowDialog()
        aktprojekt.clear()
        Projekt_RefreshDetailEintrag(aktprojekt)
    End Sub

    Function projektDatenholen() As Boolean
        CLstart.myc.aktprojekt = ProjektAufrufen(False)
        tbProjektAbstract.Text = CLstart.myc.aktprojekt.bildeTextFuerDetails
        tbProjektAbstract.ToolTip = CLstart.myc.aktprojekt.bildeTooltipFuerDetails
    End Function




    Private Sub cmbBeteiligteFunktionen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbBeteiligteFunktionen.SelectedItem Is Nothing Then Exit Sub
        If cmbBeteiligteFunktionen.SelectedValue Is Nothing Then Exit Sub
        Dim zielid As String = ""
        If cmbBeteiligteFunktionen.SelectedValue.ToString.ToLower.Contains("alle beteiligten löschen") Then
            detailsTools.AlleBeteiligtenLoeschen()
            refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        End If
        If cmbBeteiligteFunktionen.SelectedValue.ToString.ToLower.Contains("alle beteiligten kopieren") Then
            If detail_dokuauswahl.zielvorgangsidistOK(zielid$) Then detailsTools.AlleBeteiligtenKopieren(myGlobalz.sitzung.aktVorgangsID, CInt(zielid$)) ' myGlobalz.sitzung.beteiligteREC.dt
        End If
        cmbBeteiligteFunktionen.SelectedIndex = 0
        e.Handled = True
    End Sub

    Private Sub cmbDokumenteFunktionen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbDokumenteFunktionen.SelectedItem Is Nothing Then Exit Sub
        If cmbDokumenteFunktionen.SelectedValue Is Nothing Then Exit Sub
        Dim zielid As String = ""
        If cmbDokumenteFunktionen.SelectedValue.ToString.ToLower.Contains("alle dokumente und fotos löschen") Then
            nachricht("USERAKTION: alle dokumente und fotos löschen")

            Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides") ' nach myGlobalz.Arc.ArcRec
            If bresult Then
                Psession.presDokus = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                presDokusAusgewaehltMarkieren(True, Psession.presDokus)
                Dim vorheranzahl As Integer = Psession.presDokus.Count
                Dim icount As Integer = Dokus_MehrfachLoeschen()
                MessageBox.Show("Es wurden " & icount & " von " & vorheranzahl & " Dokumenten gelöscht.")
            End If
            ' detailsTools.AlleDokumenteLoeschenALT()
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        End If
        If cmbDokumenteFunktionen.SelectedValue.ToString.ToLower.Contains("alle dokumente und fotos zu anderem vorgang kopieren") Then
            nachricht("USERAKTION: alle dokumente und fotos zu anderem vorgang kopieren")
            Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides") ' nach myGlobalz.Arc.ArcRec
            If bresult Then
                Psession.presDokus = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                presDokusAusgewaehltMarkieren(True, Psession.presDokus)
                Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachKopieren()
                MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten kopiert.")
            End If
            'If detailsTools.zielvorgangsidistOK(zielid) Then           
            ' detailsTools.AlleDokumentenKopieren(myGlobalz.sitzung.aktVorgangsID, CInt(zielid)) ' myGlobalz.sitzung.beteiligteREC.dt
            'End If       
        End If
        If cmbDokumenteFunktionen.SelectedValue.ToString.ToLower.Contains("alle dokumente und fotos revisionssicher speichern") Then
            nachricht("USERAKTION: alle dokumente revisionssicher speichern")
            Dim icount As Integer = clsEreignisTools.AlleDokumentenRevisionssicherMachen(myGlobalz.sitzung.aktVorgangsID, "")
            MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten revisionsgesichert.")
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        End If
        e.Handled = True
    End Sub

    Private Sub cmbVerlaufFunktionen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbVerlaufFunktionen.SelectedItem Is Nothing Then Exit Sub
        If cmbVerlaufFunktionen.SelectedValue Is Nothing Then Exit Sub
        If cmbVerlaufFunktionen.SelectedIndex = 0 Then Exit Sub

        If cmbVerlaufFunktionen.SelectedValue.ToString.ToLower.Contains("alle ereignisse löschen") Then
            nachricht("USERAKTION: alle ereignisse löschen ")
            'dgEreignisse.ItemsSource=nothing
            dgEreignisse.DataContext = Nothing
            detailsTools.AlleEreignisseLoeschen(ereignisDokListe)
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        End If
        cmbVerlaufFunktionen.SelectedIndex = 0
        e.Handled = True
    End Sub

    Private Sub cmbRaumbezugsFunktionen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbRaumbezugsFunktionen.SelectedItem Is Nothing Then Exit Sub
        If cmbRaumbezugsFunktionen.SelectedValue Is Nothing Then Exit Sub
        'Dim zielid$
        If cmbRaumbezugsFunktionen.SelectedIndex = 0 Then Exit Sub
        If cmbRaumbezugsFunktionen.SelectedValue.ToString.ToLower.Contains("alle raumbezüge löschen") Then
            detailsTools.AlleRaumbezuegeLoeschen(myGlobalz.sitzung.aktVorgangsID)
        End If
        If cmbRaumbezugsFunktionen.SelectedValue.ToString.ToLower.Contains("eigentümerliste erstellen") Then
            Dim erfolg As Boolean = detailsTools.erstelleCSVausgabeDerFlurstuecke(myGlobalz.sitzung.aktVorgangsID)
            Process.Start(initP.getValue("ExterneAnwendungen.APPLICATION_MULTIFST2CSV"))
        End If

        If cmbRaumbezugsFunktionen.SelectedValue.ToString.ToLower.Contains("flurstücksraumbezüge") Then
            Dim csvlisteerstellen As New WinCsvliste
            csvlisteerstellen.ShowDialog()
        End If
        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
        e.Handled = True
    End Sub

    Private Sub cmbGISHintergrund_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If cmbGISHintergrund.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbGISHintergrund.SelectedValue, String)
            If String.IsNullOrEmpty(item) Then Exit Sub
            nachricht("USERAKTION: hintergrund ändern ")
            Dim a$() = item.Split("#"c)

            If a(0) = "nullover" Then
                CLstart.myc.kartengen.aktMap.Hgrund = "" & ";"
                tbHGRUND.Text = "Kein Hintergrund"
            Else
                tbHGRUND.Text = a(1)
                CLstart.myc.kartengen.aktMap.Hgrund = a(0) & ";"
            End If
            CLstart.myc.kartengen.aktMap.Hgrund = CLstart.myc.kartengen.aktMap.Hgrund.Replace("nullover", "")
            CLstart.myc.kartengen.aktMap.Vgrund = detailsTools.getNewVgrundString()
            myGlobalz.availablePresentationObjects.Clear()
            presentMap()
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbGISHintergrund_SelectionChanged" & ex.ToString)
        End Try
    End Sub

    Private Sub btnFstsuche(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: flst suchen ")
        Dim flst As New WinDetailSucheFST("ort")
        Dim ergebnis As Boolean = CBool(flst.ShowDialog())
        If ergebnis Then
            setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
        Else
        End If
        e.Handled = True
    End Sub

    Private Sub btnAdrSuche(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: adr suchen ")
        Dim adrs As New winDetailAdressSuche
        Dim ergebnis As Boolean = CBool(adrs.ShowDialog())
        If ergebnis Then
            setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
        Else
        End If
        e.Handled = True
    End Sub

    Private Sub btnEigentuemer(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: eigentümer ")
        Dim flst As New WinDetailSucheFST("eigentuemer")
        Dim ergebnis As Boolean = CBool(flst.ShowDialog())
        If ergebnis Then
            setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
        Else
        End If
        e.Handled = True
    End Sub


    Private Sub fstPaint_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: paint ")
        paintTools.DateiFeststellenUndPainten()
        e.Handled = True
    End Sub


    Private Sub dgVorgangFotos_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If dgVorgangFotos.SelectedItem Is Nothing Then Exit Sub
        Dim item As New clsPresDokumente
        Try
            item = CType(dgVorgangFotos.SelectedItem, clsPresDokumente)
        Catch ex As Exception
            nachricht(ex.ToString)
            Exit Sub
        End Try
        If Not auswahlspalteFotos.Visibility = Windows.Visibility.Visible Then
            myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            ' DokArc.  DokumentDatarowView2Obj(item, myGlobalz.sitzung.aktDokument)
            myGlobalz.sitzung.aktDokument = CType(item.Clone, Dokument)
            DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktVorgangsID)
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
            dgVorgangDokumente_SelectionChanged_1Extracted()
        Else
            '  MsgBox("huuhu")
        End If
        e.Handled = True
    End Sub

    Private Sub btnTextbausteine_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Process.Start(initP.getValue("ExterneAnwendungen.APPLICATION_textbaustein"))
        e.Handled = True
    End Sub

    Private Sub btnfotogugger2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: fotogugger ")
        dgVorgangFotos.DataContext = Nothing
        btnFotoGucker_ClickExtracted()
        dgVorgangFotos.DataContext = Psession.presFotos
        mitMehrfachauswahlFotos.IsChecked = True
        ' Dim a=
        e.Handled = True
    End Sub

    Private Sub btnfotobuch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: fotodruck ")
        clsBerichte.fotobucherstellen(myCanvas, True, detail_dokuauswahl.dokListenMergen(Psession.presDokus, Psession.presFotos), CLstart.myc.kartengen.aktMap.aktrange)
        e.Handled = True
    End Sub

    Private Sub setzeHintergrundTextInMiniMap()
        tbHGRUND.Text = glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.Hgrund)
    End Sub


    Private Sub printmap(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim druck As New winDruck(myCanvas, ableitungskreis)
        druck.ShowDialog()
        '  clsBerichte.erstelleKartendruck(myCanvas, ableitungskreis)
        e.Handled = True
    End Sub

    Private Sub btnAusschnitt_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAusschnitt.Checked

    End Sub

    Private Sub dgVerwandteServer_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If dgVerwandteServer.SelectedItem Is Nothing Then Exit Sub
            Dim item As DataRowView = CType(dgVerwandteServer.SelectedItem, DataRowView)
            geaenderteStammdatenSpeichern()
            Dim kopplungsid As Integer = CInt(item("id"))
            Dim vid As String = item("vorgangsid").ToString
            dgVerwandteServer.SelectedItem = Nothing
            'dgVerwandte.SelectedIndex = 0
            Verwandte_verarbeiten(kopplungsid, CInt(vid), item("Titel").ToString)
            e.Handled = True
        Catch ex As Exception
            nachricht("Sie haben in eine leere Zeile geklickt. Bitte versuchen Sie es nochmal." & ex.ToString)
        End Try
    End Sub

    Private Function vid_istOk(ByVal quellVid As Integer) As Boolean
        Try
            If IsNumeric(quellVid) Then
                Return True
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub copydokListe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If cmbDokuverwandte.SelectedValue Is Nothing Then
            MessageBox.Show("Sie müssen zuerst einen verwandten Vorgang auswählen!", "Daten aus verwandtem Vorgang übernehmen", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        Dim quellVid% = CInt(cmbDokuverwandte.SelectedValue.ToString)
        detailsTools.AlleDokumentenKopieren(quellVid%, myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        cmbDokuverwandte.SelectedItem = Nothing
        e.Handled = True
    End Sub



    Public Sub OfficeCreateNewFileSystemWatcherAndSetItsProperties()
        nachricht("OfficeCreateNewFileSystemWatcherAndSetItsProperties --------------------------")
        Try
            ' Create a new FileSystemWatcher and set its properties.
            Dim test As New IO.FileInfo(myGlobalz.Arc.lokalerCheckoutcache)
            nachricht("officeDateiFullName :" & CLstart.myc.kartengen.gifKartenDateiFullName)
            officeDocWatcher = New FileSystemWatcher
            officeDocWatcher.Path = test.DirectoryName & "\" & myGlobalz.sitzung.aktVorgangsID
            ' Watch for changes in LastAccess and LastWrite times, and            ' the renaming of files or directories.
            officeDocWatcher.NotifyFilter = (NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.DirectoryName)
            officeDocWatcher.Filter = "~$*.*"
            AddHandler officeDocWatcher.Deleted, AddressOf OnDeletedFileSystemCacheOFFICE
            ' Begin watching.
            officeDocWatcher.EnableRaisingEvents = True
            nachricht("OfficeCreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------")
        Catch ex As Exception
            nachricht("Fehler in : OfficeCreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------" & ex.ToString)
        End Try
    End Sub

    Public Sub OnDeletedFileSystemCacheOFFICE(ByVal source As Object, ByVal e As FileSystemEventArgs)
        Try
            nachricht("1OnChangedFileSystemCacheOFFICE --------------------------vor invoke")
            Me.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                                      New watcherCallBackToUIThread(AddressOf NotifyUIThreadOfChangeOFFICE), e)
            officeDocWatcher.Dispose()
            nachricht("Dokument wurde geändert?: " & detailsTools.wurdenDokumenteGeaendert(myGlobalz.sitzung.checkoutDokuList).ToString)
            nachricht("2OnChangedFileSystemCacheOFFICE -------------ende------------- Office-datei wurde geschlossen und ins archiv integriert")
            '  refreshDokumente(myGlobalz.sitzung.VorgangsID) geht nicht wg. falscher Thread
        Catch ex As Exception
            nachricht("fehler in OnChangedFileSystemCacheOFFICE " & ex.ToString)
        End Try
    End Sub

    Public Sub NotifyUIThreadOfChangeOFFICE(ByVal e As FileSystemEventArgs)
        ' MsgBox(e.ToString)
    End Sub

    Private Sub btnFotoshinzu2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFotoshinzu2.Click
        nachricht("USERAKTION: fotos hinzufügen ")
        dokuhinzufuegenUndRefresh()
        e.Handled = True
    End Sub

    Private Sub btnGetCoordinates4Kreis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: kreis ")
        Dim radius$ = Microsoft.VisualBasic.Interaction.InputBox("Bitte geben Sie hier den gewünschten Radius [m] ein:" & vbCrLf & _
                                                  "Anschließend klicken Sie bitte auf den Punkt in der Karte auf dem dere Kreis plaziert werden soll." & vbCrLf,
                                                  "Radius definieren",
                                                  "15")
        '   der Radius ist in wirklichkeit ien Durchmesser, deshalb die verdopplung
        If Not IsNumeric(radius) Then
            ableitungskreis.radius = 30
        Else
            ableitungskreis.radius = Val(radius) * 2
        End If
        CanvasClickModus = "Ableitungskreis"
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
        e.Handled = True
    End Sub

    Private Sub initTabcontrolsMaxheight()
        Dim maxheight As Integer = 500

        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
            maxheight = 800 '600
        End If
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
            maxheight = 700 '500
        End If
        TabControl1.MaxHeight = maxheight
    End Sub

    Private Function initcanvasHeight() As Double
        Dim maxheight As Double = 680
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
            maxheight = 670
        End If
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
            maxheight = 570
        End If
        Return maxheight
    End Function










    Private Sub dgProjekt_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If dgProjekt.SelectedItem Is Nothing Then Exit Sub
            Dim item As DataRowView = CType(dgProjekt.SelectedItem, DataRowView)
            '    Dim kopplungsid% = CInt(item("vorgangsid"))
            Dim vid As String = item("vorgangsid").ToString
            dgProjekt.SelectedItem = Nothing
            detailsTools.VorgangLocking("aus")
            geaenderteStammdatenSpeichern()
            myGlobalz.sitzung.aktVorgangsID = CInt(vid)

            Me.Close()
            glob2.editVorgang(CInt(myGlobalz.sitzung.aktVorgangsID))
        Catch ex As Exception
            nachricht("Sie haben in eine leere Zeile geklickt. Bitte versuchen Sie es nochmal." & ex.ToString)
        End Try
        e.Handled = True
    End Sub

    Private Sub refreshProjekt(ByVal vid As Integer)
        Try
            CLstart.myc.aktprojekt = New CLstart.clsProjektAllgemein(vid)
            CLstart.myc.aktprojekt.id = CInt(NSprojekt.Projekt_holeprojektnummer.exe(vid, CLstart.myc.aktprojekt))
            NSprojekt.Projekt_holeprojektnummer.ProjektlisteAnzeigenExtracted(dgProjekt, CLstart.myc.aktprojekt)
            setzeHeaderText()
        Catch ex As Exception
            nachricht("fehler in refreshProjekt: " & ex.ToString)
        End Try
    End Sub

    Private Sub setzeHeaderText()
        If CLstart.myc.aktprojekt.id > 0 Then
            If Not myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                tabheaderProjekt.Header = "Projekt " & myGlobalz.sitzung.VorgangREC.dt.Rows.Count
            Else
                tabheaderProjekt.Header = "Projekt "
            End If
        Else
            tabheaderProjekt.Header = "Projekt "
        End If
    End Sub

    Private Sub rbHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        jfHilfe.starthelp("detail_rb")
        e.Handled = True
    End Sub

    Private Sub projektHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        jfHilfe.starthelp("detail_projekt")
        e.Handled = True
    End Sub

    Private Sub verlaufHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        jfHilfe.starthelp("detail_verlauf")
        e.Handled = True
    End Sub

    Private Sub BeteiligteHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        jfHilfe.starthelp("detail_beteiligte")
        e.Handled = True
    End Sub

    Private Sub fotosHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        jfHilfe.starthelp("detail_fotos")
        e.Handled = True
    End Sub

    Private Sub verwandteHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        jfHilfe.starthelp("detail_verwandte")
        e.Handled = True
    End Sub

    Private Sub dokumentHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        jfHilfe.starthelp("detail_dokumente")
        e.Handled = True
    End Sub


    Private Sub createPojekt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim aktprojekt = New CLstart.clsProjektAllgemein(myGlobalz.sitzung.aktVorgangsID)
        Dim prj As New WinProjekt("neu", aktprojekt)
        prj.ShowDialog()
        'DialogResult = False
        ' Close()
        e.Handled = True
    End Sub

    Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        glob2.istTextzulang(540, tbBemerkungReiter)
        schlagworteWurdeGeaendert = True
        e.Handled = True
    End Sub

    Sub New() 'bestandIstGeoeffnet As boolean
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        tbBemerkungReiter.DataContext = myGlobalz.sitzung.aktVorgang.Stammdaten
        schlagworteWurdeGeaendert = False
        retcode = ""
        RubberbandStartpt = Nothing
        RubberbandEndpt = Nothing
        KoordinateKLickpt = Nothing
        schlagworteWurdeGeaendert = False
        ladevorgangAbgeschlossen = False
        ableitungskreis = New clsAbleitungskreis
        ereignisDokExpand = True
        ereignisDokListe = New List(Of clsEreignisDok)
        ' _bestandIstGeoeffnet=bestandIstGeoeffnet
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
    End Sub

    Private Sub schlagworteEinfaerben()
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung) Then
            tabheaderSchlagworte.SetValue(TextElement.FontWeightProperty, FontWeights.Normal)
        Else
            tabheaderSchlagworte.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
        End If
    End Sub

    '    Private Sub btnBestand_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '        'pruefen ob die erste bestandsform existiert, name=WinBestandStammdaten
    '        'If myGlobalz.BestandsFensterIstgeoeffnet Then
    '        '    MessageBox.Show("Das Bestandsfenster ist bereits geöffnet. Es erscheint, sobald Sie den Vorgang schließen.",
    '        '                     "Bestandsfenster öffnen", MessageBoxButton.OK, MessageBoxImage.Information)
    '        'Else
    '        Application.Current.MainWindow.Topmost=true
    '        Application.Current.MainWindow.Activate()
    '        Application.Current.MainWindow.Focus()
    '      '   FocusManager.SetFocusedElement(Me, tbFilter)
    '      'FocusManager.SetFocusedElement(  Window1,Nothing)
    ''MsgBox("Bitte wechseln sie über die Taskbar in Hauptmenü und wäheln Sie dort 'Bestand'")
    '            'Close()
    '           ' retcode = "zurBestandsUebersicht"
    '        'End If
    '        e.Handled = True
    '    End Sub

    Private Sub initStammBearbeiterTooltip()
        tbBearbeiter.ToolTip = Paradigma_start.Win1Tools.BildeBearbeiterProfilalsString(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter)
        ' tbBearbeiterKuerzel.ToolTip = tbBearbeiter.ToolTip
    End Sub

    Private Sub Verteiler_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim verteiler As String
        verteiler = mailTools.emailVerteilerBilden(myGlobalz.anhangtrenner)
        Clipboard.Clear()
        Clipboard.SetText(verteiler)
        MsgBox("Der Verteiler befindet sich nun in Ihrer Windows-Zwischenablage. " & vbCrLf & _
         "Sie können Sie mit Strg-v in Ihr Dokument einfügen." & vbCrLf)
        e.Handled = True
    End Sub

    Private Sub btnBestand_Click(sender As Object, e As RoutedEventArgs)
        If myGlobalz.BestandsFensterIstgeoeffnet Then
            'WinBestandStammdaten.Topmost = True
            'Application.Current.MainWindow.Topmost = True
            'Application.Current.MainWindow.Activate()
            'Application.Current.MainWindow.Focus()
            MsgBox("Es ist bereits eine Bestandsübersicht geöffnet. Sie können Sie direkt über die Taskbar von Windows aufrufen.", MsgBoxStyle.Information, "Zur Bestandsliste")
        Else
            'retcode = "zurBestandsUebersicht"
            Close()
            clsStartup.FormularBestandStammdaten(False, False)
        End If
        e.Handled = True
    End Sub



    'Private Sub btnNatureg_Click_1(sender As Object, e As RoutedEventArgs)
    '    nachricht("USERAKTION: zu natureg ")
    '    Dim ggg As New VorgangUebersicht
    '    ggg.ShowDialog()
    '    e.Handled = True
    'End Sub

    'Private Sub natureg_button_sichtbarmachen()
    '    If myGlobalz.sitzung.Bearbeiter.Bemerkung.ToLower.Contains("naturschutz") Or
    '        myGlobalz.sitzung.Bearbeiter.Bemerkung.ToLower.Contains("graph") Or
    '        clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
    '        btnNatureg.Visibility = Windows.Visibility.Visible
    '    Else
    '        btnNatureg.Visibility = Windows.Visibility.Hidden
    '    End If
    'End Sub

    '<Obsolete>
    'Private Sub ErstelleParameterdateiunderstellekarte(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    LocalParameterFiles.erzeugeParameterDatei(True, True)
    '    glob2.neueKarteerstellen()
    '    e.Handled = True
    'End Sub

    Private Sub btnZumGis(sender As Object, e As RoutedEventArgs)
        nachricht("USERAKTION: zum webgis ")
        Dim layer As String
        Dim aktmap As New clsMapSpec
        clsMiniMapTools.setzeAbteilungsThemen(aktmap, myGlobalz.sitzung.aktBearbeiter.Bemerkung.Trim)
        layer = aktmap.Vgrund & ";" & aktmap.Hgrund
        Dim gis As New clsGISfunctions
        gis.mittelpunktsAufruf(CLstart.myc.raumberange, layer, initP.getValue("GisServer.GIS_WebServer"))
        'starten als unabhängigen prozess
    End Sub



    Private Sub kostenggemeinsam()
        myGlobalz.sitzung.aktVorgang.KostenStatus.QUELLE = myGlobalz.sitzung.aktBearbeiter.Initiale
        Dim lKostenToolspeichern As Boolean = kostenTool.speichern(myGlobalz.sitzung.aktVorgang.KostenStatus, myGlobalz.sitzung.aktBearbeiter.Initiale, myGlobalz.sitzung.aktVorgangsID)
        If lKostenToolspeichern Then aktualisiereKostenTooltip()
    End Sub


    Private Sub chkboxInterneZahlungNEU(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.InterneZahlung = CBool(chkboxInterneZahlung.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub refreshKosten(vid As Integer)
        Try
            myGlobalz.sitzung.aktVorgang.KostenStatus.clear()
            myGlobalz.sitzung.aktVorgang.KostenStatus.vorgangsid = vid
            If kostenTool.getKostenOjbFromDb(vid) Then
                kostenstatusDarstellen()
            Else
                ' kostenstatusDarstellen()
            End If
        Catch ex As Exception
            nachricht("fehler in refreshKosten: " & ex.ToString)
        End Try
    End Sub

    Private Sub aktualisiereKostenTooltip()
        grpBoxKosten.ToolTip = "Letzte Änderung: " & myGlobalz.sitzung.aktVorgang.KostenStatus.QUELLE & ", am: " & myGlobalz.sitzung.aktVorgang.KostenStatus.timestamp
    End Sub

    Private Sub kostenstatusDarstellen()
        Try
            aktualisiereKostenTooltip()
            chkboxInterneZahlung.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.InterneZahlung, True, False)
            chkboxverwaltungsgebuehr.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.verwaltungsgebuehr, True, False)
            verwaltungsgebuehrBezahlt.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.verwaltungsgebuehrBezahlt, True, False)
            ersatzgeld.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.ersatzgeld, True, False)
            ersatzgeldBezahlt.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.ersatzgeldBezahlt, True, False)
            sicherheit.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.sicherheit, True, False)
            sicherheitBezahlt.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.sicherheitBezahlt, True, False)
        Catch ex As Exception
            nachricht("fehler in kostenstatusDarstellen: " & ex.ToString)
        End Try
    End Sub

    Private Sub chkboxverwaltungsgebuehr_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.verwaltungsgebuehr = CBool(chkboxverwaltungsgebuehr.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub verwaltungsgebuehrBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.verwaltungsgebuehrBezahlt = CBool(verwaltungsgebuehrBezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub ersatzgeld_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.ersatzgeld = CBool(ersatzgeld.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub ersatzgeldBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.ersatzgeldBezahlt = CBool(ersatzgeldBezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub sicherheit_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.sicherheit = CBool(sicherheit.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub sicherheitBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.sicherheitBezahlt = CBool(sicherheitBezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub initKostenFeld()
        Try
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                grpBoxKosten.IsEnabled = True
            Else
                grpBoxKosten.IsEnabled = False
            End If
        Catch ex As Exception
            nachricht("fehler in initKostenFeld: " & ex.ToString)
        End Try
    End Sub


    Private Sub BUSSGELDBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.BUSSGELDBezahlt = CBool(BUSSGELDBezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub BUSSGELD_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.BUSSGELD = CBool(BUSSGELD.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub VERWARNUNGSGELD_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.VERWARNUNGSGELD = CBool(VERWARNUNGSGELD.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub VerwarnungsgeldBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.VerwarnungsgeldBezahlt = CBool(VerwarnungsgeldBezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub


    Private Sub btnGetLinie_Click(sender As Object, e As RoutedEventArgs)
        nachricht("USERAKTION: strecke messen ")
        MsgBox("Wählen sie die Strecke in der Karte indem Sie die Punkte anklicken (Linke Maustaste drücken)")
        CanvasClickModus = "Strecke"
        btnGetLinie.IsEnabled = False
        btnGetLinieEnde.IsEnabled = True
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
        '  DrawPolygon(myCanvas)
        myGlobalz.sitzung.aktPolyline.clear()
        DrawPolylinie(myCanvas)
        e.Handled = True
    End Sub

    Private Sub btnGetLinieEnde_Click(sender As Object, e As RoutedEventArgs)
        btnGetLinie.IsEnabled = True
        btnGetLinieEnde.IsEnabled = False
        btnGetlinieEnde_ClickExtracted()
        e.Handled = True
    End Sub

    Private Sub DrawPolylinie(canvas As Canvas)
        myGlobalz.sitzung.aktPolyline.myLine = New Polyline
        myGlobalz.sitzung.aktPolyline.myLine.Name = "myLine"
        Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 100, 250)) 'transparenz ist der erste wert
        myGlobalz.sitzung.aktPolyline.myLine.Stroke = Brushes.DarkBlue
        myGlobalz.sitzung.aktPolyline.myLine.StrokeThickness = 4
        myGlobalz.sitzung.aktPolyline.myLine.Opacity = 90
        myGlobalz.sitzung.aktPolyline.myLine.Fill = myBrush
        Panel.SetZIndex(myGlobalz.sitzung.aktPolyline.myLine, 100)
        canvas.SetZIndex(myGlobalz.sitzung.aktPolyline.myLine, 100)
        myCanvas.Children.Add(myGlobalz.sitzung.aktPolyline.myLine)
        Panel.SetZIndex(myGlobalz.sitzung.aktPolyline.myLine, 100)
        canvas.SetZIndex(myGlobalz.sitzung.aktPolyline.myLine, 100)
        myPolyVertexCount = 0
    End Sub

    Private Sub btnGetlinieEnde_ClickExtracted()
        Dim anyChange As Boolean
        If myPolyVertexCount > 1 Then
            'If clsMiniMapTools.PolylineAufbereiten(myGlobalz.sitzung.aktPolyline) Then
            '    'clsMiniMapTools.GK_StreckeErmitteln()
            'End If
            ' tbMinimapLinie.Text = CLng(myGlobalz.sitzung.aktPolyline.Distanz).ToString
            If FlaecheAlsNeuenRaumbezugAnbieten("Strecke") Then
                myGlobalz.sitzung.raumbezugsmodus = "neu"
                myGlobalz.sitzung.aktPolyline.Typ = RaumbezugsTyp.Polyline
                Dim geoedit As New Win_Polygon("strecke")
                geoedit.ShowDialog()
                anyChange = CBool(geoedit.DialogResult)
            End If

        Else
            MsgBox("Zu wenig Punkte für eine Flächenberechnung. Mind. 3 Punkte sind erforderlich!")
        End If
        myPolyFinish("strecke")
        myGlobalz.sitzung.aktPolyline.Distanz = 0
        If anyChange Then refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False)
    End Sub



    Private Sub pdfTestClick(sender As Object, e As RoutedEventArgs)
        nachricht("USERAKTION: pdf erzeugen ")
        Dim aaa As New winPDFDruck
        aaa.ShowDialog()
        If aaa.chkEreignisMap.IsChecked Then
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        End If
        e.Handled = True
    End Sub






    'Private Sub TextBlock_MouseDown(sender As Object, e As MouseButtonEventArgs)

    'End Sub


    Private Sub btnExportClick(sender As Object, e As RoutedEventArgs)
        nachricht("USERAKTION: export ")
        Dim export As New WinExport(myGlobalz.sitzung.aktVorgangsID)
        export.Show()
        GC.Collect()
        e.Handled = True
    End Sub



    Private Sub setMitDokumenten()
        Try
            If chkExpandereignis.IsChecked Then
                ereignisDokExpand = True
                CLstart.myc.userIniProfile.WertSchreiben("Verlauf", "mitDokumenten", "1")
            Else
                ereignisDokExpand = False
                CLstart.myc.userIniProfile.WertSchreiben("Verlauf", "mitDokumenten", "0")
            End If
        Catch ex As Exception
            nachricht("fehler in setMitDokumenten: " & ex.ToString)
        End Try
    End Sub
    Private Sub ckeckExpander(sender As Object, e As RoutedEventArgs)
        ' setMitDokumenten()
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnGoogleMaps(sender As Object, e As RoutedEventArgs)
        nachricht("USERAKTION: googlekarte ")
        Dim gis As New clsGISfunctions
        gis.GoogleMapsAufruf_Mittelpunkt(CLstart.myc.kartengen.aktMap.aktrange)
        e.Handled = True
    End Sub

    Private Sub btnDokumenteZwischenablage_Click(sender As Object, e As RoutedEventArgs)
        If meinClipboard.getContentFromZwischenablage Then
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        End If
        e.Handled = True
    End Sub

    Private Sub VerlaufmitDokumentenSetzen()
        Try
            If CInt(CLstart.myc.userIniProfile.WertLesen("Verlauf", "mitDokumenten")) = 0 Then
                chkExpandereignis.IsChecked = False
            Else
                chkExpandereignis.IsChecked = True
            End If

        Catch ex As Exception
            nachricht("Fehler in VerlaufmitDokumentenSetzen: " & ex.ToString)
            chkExpandereignis.IsChecked = True
        End Try
    End Sub

    Private Sub chkRaumbezuegeObsolet_Checked(sender As Object, e As RoutedEventArgs) Handles chkRaumbezuegeObsolet.Checked, chkRaumbezuegeObsolet.Unchecked

        e.Handled = True
    End Sub

    Private Sub chkRaumbezuegeObsolet_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If chkRaumbezuegeObsolet.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = False
        End If
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "HATRAUMBEZUG")
        e.Handled = True
    End Sub

    Private Sub cmbGemKRZ_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If cmbGemKRZ.SelectedItem Is Nothing Then Exit Sub
        Dim item As String = CType(cmbGemKRZ.SelectedValue, String)
        myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = item.Trim
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "GEMKRZ")
        e.Handled = True
    End Sub

    Private Sub cmbGemKRZ_MouseMove(sender As Object, e As MouseEventArgs)
        cmbGemKRZ.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub

    'Private Sub cmbGemKRZ_MouseEnter(sender As Object, e As MouseEventArgs)
    '    cmbGemKRZ.Visibility = Windows.Visibility.Visible
    '    e.Handled = True
    'End Sub

    Private Sub tbGEMKRZ_MouseEnter(sender As Object, e As MouseEventArgs)
        cmbGemKRZ.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub

    Private Sub btnWeitereBearbeiterListen_Click(sender As Object, e As RoutedEventArgs)
        glob2.setzeZugriffsrechte()
        tbWeitereBearbeiter.Text = CLstart.myc.AZauswahl.WeitereBearbeiter
        myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "WEITEREBEARB")
        e.Handled = True
    End Sub



    Private Sub tbWeitereBearbeiter_Mousemove(sender As Object, e As MouseEventArgs)
        btnWeitereBearbeiterListen.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub


    Private Sub tbBeschreibung2_MouseDown(sender As Object, e As MouseButtonEventArgs)
        tbBeschreibung2.IsReadOnly = False
        e.Handled = True
    End Sub

    Private Sub tbBeschreibung2_SelectionChanged(sender As Object, e As RoutedEventArgs)
        If tbBeschreibung2.IsReadOnly = False Then
            tbBeschreibung2.Background = New SolidColorBrush(Colors.Pink)
            btnConfirmBeschreibung2.IsEnabled = True
        End If
        e.Handled = True
    End Sub

    Private Sub btnConfirmBeschreibung2_Click(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = tbBeschreibung2.Text
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "BESCHREIBUNG")
        tbBeschreibung2.Background = New SolidColorBrush(Colors.Silver)
        tbBeschreibung2.IsReadOnly = True
        btnConfirmBeschreibung2.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub beihilfeBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.BEIHILFEBezahlt = CBool(beihilfeBezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub beihilfe_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.BEIHILFE = CBool(beihilfe.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub ersatzgeldausgezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.ersatzgeldAUSGEzahlt = CBool(ersatzgeldausgezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub ZwangsgeldBezahlt_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.ZWANGSGELDBezahlt = CBool(ZwangsgeldBezahlt.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub Zwangsgeld_Click_1(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.KostenStatus.ZWANGSGELD = CBool(Zwangsgeld.IsChecked)
        kostenggemeinsam()
        e.Handled = True
    End Sub

    Private Sub cmbSachgebietsFormulare_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If cmbSachgebietsFormulare.SelectedItem Is Nothing Then Exit Sub
        If cmbSachgebietsFormulare.SelectedValue Is Nothing Then Exit Sub
        Dim item As String = cmbSachgebietsFormulare.SelectedValue.ToString
        item = item.Replace("System.Windows.Controls.ComboBoxItem: ", "").Trim.ToLower
        detailsTools.zumSGformular(item, myGlobalz.sitzung.aktVorgangsID)
        '     myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = item.Trim
        'detailsTools.Edit_singleUpdate_Stammdaten(Now, "GEMKRZ")
        e.Handled = True
    End Sub

    Private Sub tbBemerkung_MouseDown(sender As Object, e As MouseButtonEventArgs)
        tbBemerkung.IsReadOnly = False
        e.Handled = True
    End Sub

    Private Sub tbBemerkung_SelectionChanged(sender As Object, e As RoutedEventArgs)
        If tbBemerkung.IsReadOnly = False Then
            tbBemerkung.Background = New SolidColorBrush(Colors.Pink)
            btnConfirmBemerkung.IsEnabled = True
        End If
        e.Handled = True
    End Sub

    Private Sub btnConfirmBemerkung_Click(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung = tbBemerkung.Text
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "BEMERKUNG")
        tbBemerkung.Background = New SolidColorBrush(Colors.Silver)
        tbBemerkung.IsReadOnly = True
        btnConfirmBemerkung.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbProbaugAZ_SelectionChanged(sender As Object, e As RoutedEventArgs)
        If tbProbaugAZ.IsReadOnly = False Then
            tbProbaugAZ.Background = New SolidColorBrush(Colors.Pink)
            btnConfirmtbProbaugAZ.IsEnabled = True
        End If
        e.Handled = True
    End Sub

    Private Sub tbProbaugAZ_MouseDown(sender As Object, e As MouseButtonEventArgs)
        tbProbaugAZ.IsReadOnly = False
        e.Handled = True
    End Sub

    Private Sub btnConfirmtbProbaugAZ_Click(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz = tbProbaugAZ.Text
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "PROBAUGAZ")
        tbProbaugAZ.Background = New SolidColorBrush(Colors.Silver)
        tbProbaugAZ.IsReadOnly = True
        btnConfirmtbProbaugAZ.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbAltAz_MouseDown(sender As Object, e As MouseButtonEventArgs)
        tbAltAz.IsReadOnly = False
        e.Handled = True
    End Sub

    Private Sub tbAltAz_SelectionChanged(sender As Object, e As RoutedEventArgs)
        If tbAltAz.IsReadOnly = False Then
            tbAltAz.Background = New SolidColorBrush(Colors.Pink)
            btnConfirmtbAltAz.IsEnabled = True
        End If
        e.Handled = True
    End Sub



    Private Sub btnConfirmtbAltAz_Click(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.Stammdaten.AltAz = tbAltAz.Text
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "ALTAZ")
        tbAltAz.Background = New SolidColorBrush(Colors.Silver)
        tbAltAz.IsReadOnly = True
        btnConfirmtbAltAz.IsEnabled = False
        e.Handled = True
    End Sub

    Private Sub btnConfirmtbInternenr_Click(sender As Object, e As RoutedEventArgs)
        myGlobalz.sitzung.aktVorgang.Stammdaten.AltAz = tbAltAz.Text
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "INTERNENR")
        tbInternenr.Background = New SolidColorBrush(Colors.Silver)
        tbInternenr.IsReadOnly = True
        btnConfirmtbInternenr.IsEnabled = False
        e.Handled = True
    End Sub

    Private Sub tbtbInternenr_MouseDown(sender As Object, e As MouseButtonEventArgs)
        tbInternenr.IsReadOnly = False
        e.Handled = True
    End Sub

    Private Sub tbtbInternenr_SelectionChanged(sender As Object, e As RoutedEventArgs)
        If tbInternenr.IsReadOnly = False Then
            tbInternenr.Background = New SolidColorBrush(Colors.Pink)
            btnConfirmtbInternenr.IsEnabled = True
        End If
        e.Handled = True
    End Sub
    Private Sub cmbParagraf_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbParagraf.SelectedItem Is Nothing Then Exit Sub
        Dim item As String = CType(cmbParagraf.SelectedValue, String)
        tbParagraph.Text = cmbParagraf.SelectedValue.ToString
        myGlobalz.sitzung.aktVorgang.Stammdaten.Paragraf = cmbParagraf.SelectedValue.ToString
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "PARAGRAF")
        e.Handled = True
    End Sub


    Private Sub cmbRaumNr_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As String = CType(cmbRaumNr.SelectedValue, String)
        myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.RaumNr = cmbRaumNr.SelectedValue.ToString
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "STORAUMNR")
        e.Handled = True
    End Sub

    Private Sub ckbGutachtenvorhanden_Checked(sender As Object, e As RoutedEventArgs) Handles ckbGutachtenvorhanden.Checked, ckbGutachtenvorhanden.Unchecked
        If ladevorgangAbgeschlossen = False Then Return
        If ckbGutachtenvorhanden.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.existiert = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.existiert = False
        End If
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "GUTACHTENMIT")
        e.Handled = True
    End Sub

    Private Sub ckbGutachtenInDokumente_Checked(sender As Object, e As RoutedEventArgs) Handles ckbGutachtenInDokumente.Checked, ckbGutachtenInDokumente.Unchecked
        If ladevorgangAbgeschlossen = False Then Return
        If ckbGutachtenInDokumente.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.UnterDokumente = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.UnterDokumente = False
        End If
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "GUTACHTENDRIN")
        e.Handled = True
    End Sub

    Private Sub chkdarfnichtvernichtetwerden_Checked(sender As Object, e As RoutedEventArgs) Handles chkdarfnichtvernichtetwerden.Checked, chkdarfnichtvernichtetwerden.Unchecked
        If ladevorgangAbgeschlossen = False Then Return

        If chkdarfnichtvernichtetwerden.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.darfNichtVernichtetWerden = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.darfNichtVernichtetWerden = False
        End If
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "DARFNICHTVERNICHTETWERDEN")
        e.Handled = True
    End Sub

    Private Sub initStammCheckBoxen()
        If myGlobalz.sitzung.aktVorgang.Stammdaten.darfNichtVernichtetWerden = True Then
            chkdarfnichtvernichtetwerden.IsChecked = True
        Else
            chkdarfnichtvernichtetwerden.IsChecked = False
        End If

        If myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.UnterDokumente = True Then
            ckbGutachtenInDokumente.IsChecked = True
        Else
            ckbGutachtenInDokumente.IsChecked = False
        End If

        If myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.existiert = True Then
            ckbGutachtenvorhanden.IsChecked = True
        Else
            ckbGutachtenvorhanden.IsChecked = False
        End If
    End Sub

    Private Sub btnZWERT_Click(sender As Object, e As RoutedEventArgs)
        nachricht("USERAKTION: zwert messen ")
        MsgBox("Wählen sie den Punkt in der Karte (Linke Maustaste drücken)")
        CanvasClickModus = "ZWERT"
        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
        e.Handled = True
    End Sub

    Private Sub btnzuVorgang_Click(sender As Object, e As RoutedEventArgs)
        Close()
        'Application.Current.MainWindow.Topmost = True
        'Application.Current.MainWindow.Activate()
        'Application.Current.MainWindow.Focus()
        GC.Collect()
        myGlobalz.einVorgangistgeoeffnet = False
        Dim az$ = "", header$ = ""
        clsStartup.suchenNachVorgaengen(az, header)
        e.Handled = True
    End Sub

    Private Sub btnNeuerVorgang_Click(sender As Object, e As RoutedEventArgs)
        Close()
        GC.Collect()
        clsStartup.NeuerVorgang2()
        GC.Collect()
        e.Handled = True
    End Sub

    Private Sub lblVorgangsID_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = False
            MsgBox("Der Status des Vorgangs wurde auf >UNERLEDIGT< geändert.")
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = True
            MsgBox("Der Status des Vorgangs wurde auf >ERLEDIGT< geändert.")
        End If

        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "ERLEDIGT")
        setzeErledigtflagfarbe()
        e.Handled = True
    End Sub

    Private Sub dokuauswahlein(sender As Object, e As RoutedEventArgs)
        auswahlspalteDokus.Visibility = Windows.Visibility.Visible
        stckmehrfachtools.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub

    Private Sub dokuauswahlaus(sender As Object, e As RoutedEventArgs)
        auswahlspalteDokus.Visibility = Windows.Visibility.Collapsed
        stckmehrfachtools.Visibility = Windows.Visibility.Collapsed
        e.Handled = True
    End Sub

    Private Sub btnMehrfachLoeschen_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlt(Psession.presDokus) > 0 Then
            Dim icount As Integer = Dokus_MehrfachLoeschen()
            MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten gelöscht.")
        Else
            MsgBox("Sie haben keine Auswahl getroffen.")
        End If
        e.Handled = True
    End Sub





    Private Shared Sub presDokusAusgewaehltMarkieren(valju As Boolean, dlist As List(Of clsPresDokumente))
        For Each ele As clsPresDokumente In dlist
            ele.ausgewaehlt = valju
        Next
    End Sub

    Private Sub chkAllesMarkieren_Click(sender As Object, e As RoutedEventArgs)
        If Not chkAllesMarkieren.IsChecked Then
            presDokusAusgewaehltMarkieren(False, Psession.presDokus)
        Else
            For Each ele As clsPresDokumente In Psession.presDokus
                presDokusAusgewaehltMarkieren(True, Psession.presDokus)
            Next
        End If
        e.Handled = True
    End Sub

    Private Sub btnMehrfachKopieren_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlt(Psession.presDokus) > 0 Then
            Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachKopieren()
            MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten kopiert.")
        Else
            MsgBox("Sie haben keine Auswahl getroffen.")
        End If

        e.Handled = True
    End Sub
    Private Function Dokus_MehrfachLoeschen() As Integer
        Dim messi As New MessageBoxResult
        If Psession.presDokus.Count > 0 Then
            messi = MessageBox.Show("Objekte wirklich löschen ?" & vbCrLf,
                              " Dokumente löschen ?",
                              MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                Dim icount As Integer
                icount = detail_dokuauswahl.AlleMarkiertenDokumenteLoeschen(Psession.presDokus)
                nachricht(icount & " Objekte gelöscht")
                nachricht("USERAKTION: " & icount & " dokus wurden gelöscht")

                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                Return icount
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function

    Private Sub btnMehrfachRevisionssichern_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlt(Psession.presDokus) > 0 Then
            Dim messi As New MessageBoxResult
            If Psession.presDokus.Count > 0 Then
                messi = MessageBox.Show("Dokumente wirklich revisionssicher machen ?" & vbCrLf,
                                  " Dokumente revisionssichern ?",
                                  MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
                If messi = MessageBoxResult.Yes Then
                    Dim icount As Integer
                    icount = detail_dokuauswahl.AlleMarkiertenDokumenteRevisionssichern(Psession.presDokus)
                    nachricht(icount & " Objekte revisionsgesichert")
                    nachricht("USERAKTION: " & icount & " dokus wurden revisionsgesichert")

                    refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                    refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                End If
            Else
                MessageBox.Show("Es sind noch keine Dokumente erfasst worden!")
            End If
        Else
            MsgBox("Sie haben keine Auswahl getroffen!")
        End If
        e.Handled = True
    End Sub

    Private Sub btnProtokoll(sender As Object, e As RoutedEventArgs)
        detailsTools.vorgangsprotokollanzeigen()
        e.Handled = True
    End Sub

    Private Sub fotoauswahlein(sender As Object, e As RoutedEventArgs)
        auswahlspalteFotos.Visibility = Windows.Visibility.Visible
        stckmehrfachfotos.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub

    Private Sub fotoauswahlaus(sender As Object, e As RoutedEventArgs)
        auswahlspalteFotos.Visibility = Windows.Visibility.Collapsed
        stckmehrfachfotos.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub btnMehrfachFotosLoeschen_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlt(Psession.presFotos) > 0 Then
            Dim icount As Integer = Fotos_MehrfachLoeschen()
            MessageBox.Show("Es wurden " & icount & " Fotos gelöscht.")
            'MessageBox.Show("Es wurden " & icount & " von " & Sitzung.presFotos.Count & " Fotos gelöscht.")
        Else
            MessageBox.Show("Es wurden nichts ausgwählt.")
        End If
        e.Handled = True
    End Sub



    Private Function Fotos_MehrfachLoeschen() As Integer
        Dim messi As New MessageBoxResult
        If Psession.presFotos.Count > 0 Then
            messi = MessageBox.Show("Objekte wirklich löschen ?" & vbCrLf,
                              " Fotos löschen ?",
                              MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                Dim icount As Integer
                icount = detail_dokuauswahl.AlleMarkiertenDokumenteLoeschen(Psession.presFotos)
                nachricht(icount & " Objekte gelöscht")
                nachricht("USERAKTION: " & icount & " fotos wurden gelöscht")

                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                Return icount
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function
    Private Sub chkAllesFotosMarkieren_Click(sender As Object, e As RoutedEventArgs)
        If Not chkAllesMarkierenFotos.IsChecked Then
            presDokusAusgewaehltMarkieren(False, Psession.presFotos)
        Else
            For Each ele As clsPresDokumente In Psession.presDokus
                presDokusAusgewaehltMarkieren(True, Psession.presFotos)
            Next
        End If
        e.Handled = True
    End Sub

    Private Sub btnMehrfachFotosRevisionssichern_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlt(Psession.presFotos) > 0 Then

            Dim messi As New MessageBoxResult
            If Psession.presFotos.Count > 0 Then
                messi = MessageBox.Show("Fotos wirklich revisionssicher machen ?" & vbCrLf,
                                  " Fotos revisionssichern ?",
                                  MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
                If messi = MessageBoxResult.Yes Then
                    Dim icount As Integer
                    icount = detail_dokuauswahl.AlleMarkiertenDokumenteRevisionssichern(Psession.presFotos)
                    nachricht(icount & " Objekte revisionsgesichert")
                    nachricht("USERAKTION: " & icount & " fotos wurden revisionsgesichert")

                    refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                    refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                End If
            Else
                MessageBox.Show("Es sind noch keine Dokumente erfasst worden!")
            End If
        Else
            MsgBox("Es wurde ncihts ausgewählt.")
        End If
        e.Handled = True
    End Sub

    Private Sub btnMehrfachFotosMailen_Click(sender As Object, e As RoutedEventArgs)
        If (detail_dokuauswahl.anzahlAusgewaehlt(Psession.presFotos) + detail_dokuauswahl.anzahlAusgewaehlt(Psession.presDokus)) > 0 Then
            Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachMailen()
            MessageBox.Show("Es wurden " & icount & " Dokumente gemailt.")
            'MessageBox.Show("Es wurden " & icount & " von " & Sitzung.presDokus.Count & " Dokumenten gemailt.")
        Else
            MsgBox("Sie haben nichts ausgewählt.")
        End If

        e.Handled = True
    End Sub



    Private Sub btnMehrfachFotosKopieren_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub btnFotosRefresh2_Click(sender As Object, e As RoutedEventArgs) Handles btnFotosRefresh2.Click
        nachricht("USERAKTION:fotos refresh ")
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        cmbDokuverwandte.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub btnDokumenteRefresh2_Click_1(sender As Object, e As RoutedEventArgs)
        resetDokuliste_ClickExtracted()
        e.Handled = True
    End Sub

    Private Sub btnAkteziehen_Click(sender As Object, e As RoutedEventArgs)

        Dim pfadAllgemein As String = ""
        Dim quelldatei As String
        Dim vorlagenVerzeichnis As IO.DirectoryInfo
        Dim endung As String = ".docx"
        '  Dim ergebnis As String=""
        clsVorlagenTools.berechneVorlagenverzeichnisAllgemein(vorlagenVerzeichnis, "Allgemein", pfadAllgemein)
        pfadAllgemein = pfadAllgemein.Replace("\\", "\")
        quelldatei = pfadAllgemein & "\" & "Bitte Akte ziehen.docx"
        'If myGlobalz.sitzung.aktVorgangsID > 0 Then
        '    Dim neuu As New clsVorlagedokumente(quelldatei)
        '    myGlobalz.sitzung.aktDokument.DateinameMitExtension = clsVorlagedokumente.neuenNamenVerwendenBilden(endung)
        '    ' myGlobalz.sitzung.aktDokument.Beschreibung = endung.Name.Replace(endung, "")
        '    myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
        '    Dokument.createCheckoutDir(myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID)
        '    clsVorlagedokumente._VorlageDateiImArchiv.CopyTo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        '    myGlobalz.sitzung.aktDokument.Filedatum = Now
        'End If
        '################
        Dim vorl As New WinVorlagenListe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl,
                                       myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header,
                                       True)
        vorl.ShowDialog()
        ' Dim ereignisanlegen As Boolean = False
        ' nachricht("WinVorlageSteuerung weiter: ")
        ' Dim insarchiv As Boolean = False
        ' nachricht("insarchiv: " & insarchiv)
        ' Dim resultstring$ = ""
        ' Dim ereignisid As Integer=0
        ' ereignisanlegen = False

        ' nachricht("WinVorlageSteuerung weiter: 4")
        ' Dim replaceTextMarkenDict As New Dictionary(Of String, String)
        ' nachricht("WinVorlageSteuerung weiter: vor prepareSelectetedVorlageDokument")
        ' Dim tauschergebnis As String = ""

        '         nachricht("WinVorlageSteuerung weiter: vor TM_fuelleMarkenDictionary")
        ' clsVorlagedokumente.TM_fuelleMarkenDictionary(replaceTextMarkenDict, "")
        ' nachricht("WinVorlageSteuerung weiter: vor TM_ausgebenMarkenDictionary")
        ' clsVorlagedokumente.TM_ausgebenMarkenDictionary(replaceTextMarkenDict)

        ' Dim ereignisart =""
        ' Dim ereignistitel =""
        ' Dim dateityp=""
        'Dim vorl As clsVorlagedokumente
        ' Try
        '     vorl = New clsVorlagedokumente(quelldatei)
        '    Dim  quellpfad As string=pfadallgemein
        '    ' vorl.aktbeteiligter = myGlobalz.sitzung.aktPerson
        '     Dim outfile As String=""'=quelldatei.Replace(endung,"_c" & endung)
        '     Dim Schlagworte As string=""
        '     'temp$ = vorl.SpezialPersonBilden("Antragsteller/in", organisation, zusatz, bezirk)
        '     'tauscheOderStandard(replaceWordDict, "Antragsteller", temp)
        '     Dim erfolg As Boolean = vorl.VorlageBestimmenUndBearbeiten(insarchiv, resultstring, outfile, replaceTextMarkenDict,
        '                                                                ereignisanlegen, Schlagworte,
        '                                                                quellpfad, ereignisart, ereignistitel, tauschergebnis, dateityp,
        '                                                                ereignisid)
        '     If erfolg Then
        '         nachricht(String.Format("clsVorlagenTools:  erfolgreich{0} {1}", outfile, resultstring))
        '         'hier könnte das Datum des Dokumentes auf NOW gesetzt werden
        '         Process.Start(outfile)
        '     Else
        '         nachricht(String.Format("clsVorlagenTools: nicht erfolgreich{0} {1}", outfile, resultstring))
        '     End If

        ' Catch ex As Exception
        '     nachricht("Fehler in prepareSelectetedVorlageDokument: " & ex.ToString)

        ' End Try




        nachricht("WinVorlageSteuerung weiter: 7")
        nachricht("WinVorlageSteuerung weiter: ENDE")
        e.Handled = True
    End Sub

    Private Sub Protokollzugriffschalten()
        Try
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                btnProtokkkoll.Visibility = Windows.Visibility.Visible
            Else
                btnProtokkkoll.Visibility = Windows.Visibility.Collapsed
            End If
        Catch ex As Exception

        Finally
        End Try
    End Sub


End Class