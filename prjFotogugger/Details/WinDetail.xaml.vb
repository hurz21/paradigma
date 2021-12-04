Imports System.Data
Imports System.IO
Imports paradigmaDetail


Partial Public Class Window_Detail
    Private gigabyte As String = ""

    Private hashtagvorlagenVerzeihnis As Boolean = False
    'Public webView As DotNetBrowser.BrowserView
    Private Property alteMarkierungen As New List(Of Integer)
    Private Property rrechtsdbARTcoll As New List(Of ClsSimpleCmb)
    Private Property rrechtsdbHerkunftcoll As New List(Of ClsSimpleCmb)
    Property GesetzListefirstTime As Boolean = True
    Public Property retcode As String  ' 0=niente "zurBestandsUebersicht"=bestand aufrufen
    Private RubberbandStartpt As Point?
    Private RubberbandEndpt As Point?
    Private rubberbox As Rectangle
    Private myPolyVertexCount As Integer
    Private KoordinateKLickpt As Point?
    'Private Property CanvasClickModus As String = ""
    Private Property schlagworteWurdeGeaendert As Boolean
    'Private Property ladevorgangAbgeschlossen As Boolean = False
    'Private Property isDraggingFlag As Boolean
    Private origContentMousePoint As Point
    Private canvasImage As New Image
    'Private Property curContentMousePoint As Point
    'Private Property zeichneImageMapGlob As Boolean = True
    'Private Property zeichneOverlaysGlob As Boolean = True
    Delegate Sub watcherCallBackToUIThread(ByVal e As FileSystemEventArgs)
    'Public gifKartenwatcher As FileSystemWatcher
    Private ableitungskreis As clsAbleitungskreis
    Private dokumenteRitemousekeypressed As Boolean
    Private fotosRitemousekeypressed As Boolean
    Public Property ereignisDokListe As List(Of clsEreignisDok)
    Public Property ereignisDokExpand As Boolean

    'Public Property mapHandlerSchonAktiv As Boolean = False
    Public Property ladevorgangAbgeschlossen As Boolean = False
    'Property useMapserverMapmode As Boolean = False
    Property useExternGis As Boolean = True
    'Public Property dokumenteRitemousekeypressed As Boolean = False 
    Private Shared Sub prepareActionlog()
        l("prepareActionlog-----------------------------------")
        Dim alogdir As String
        alogdir = initP.getValue("Haupt.ActionLogRoot")
        alogdir = alogdir & myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID)
        l("alogdir " & alogdir)
        'myglobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir = myglobalz.sitzung.defineArchivVorgangsDir(myglobalz.sitzung.aktVorgangsID) 'glob2.archicsubdirfeststellen()
        'l("myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir " & myglobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
        'l("myGlobalz.Arc.rootDir.ToString: " & myglobalz.Arc.rootDir.ToString)

        ''    Dim tempo$ = myGlobalz.Arc.rootDir.ToString & myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir
        'Dim erfolg As Boolean = myglobalz.sitzung.aktVorgang.Stammdaten.createArchivsubdir(myglobalz.Arc.rootDir.ToString, myglobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)

        'nachricht(If(Not erfolg,
        '                   "Fehler beim erzeugen des createArchivsubdir: (ARCHIVFEHLER!!! ggf. ist das Archiv nivht mehr im Zugriff!!!!)" & myglobalz.sitzung.aktBearbeiter.username & " " & myglobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
        '                   "createArchivsubdir erfolgreich"))
        ' Dim sd As String = myglobalz.Arc.rootDir.ToString & myglobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir.Replace("/", "\")
        'clstart.myc.aLog = New ActionLog(String.Format("{0}\{1}_NEWactionlog.csv", sd, myGlobalz.sitzung.Bearbeiter.Initiale))
        CLstart.myc.aLog = New CLstart.ActionLog(String.Format("{0}\NEWactionlog.csv", alogdir), alogdir)
    End Sub



    Private Function starteDetails() As Boolean
        Try

            Debug.Print("givenVID" & Application.givenVID)
            Debug.Print("givenVIDLIST" & Application.givenVIDList)
            Debug.Print("givenVID" & Application.givenVID)
            Debug.Print("givenEID" & Application.givenEID)
            Debug.Print("givendocID" & Application.givenDOCID)
            Debug.Print("nurverwandte" & Application.nurverwandte)

            Debug.Print("aktuser" & myglobalz.sitzung.aktBearbeiter.username)
            Debug.Print("aktuser" & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.username)
            If myGlobalz.sitzung.modus.IsNothingOrEmpty Then myGlobalz.sitzung.modus = "edit"
            stckmehrfachtools.Visibility = Windows.Visibility.Collapsed : stckmehrfachfotos.Visibility = Windows.Visibility.Collapsed : stckBeteiligteMehrfachtools.Visibility = Visibility.Collapsed
            Psession.presDokus = New List(Of clsPresDokumente)

            CLstart.myc.kartengen.aktMap.Vgrund = ""

            glob2.dina4InMM.w = 297 : glob2.dina4InMM.h = 210
            glob2.dina3InMM.w = 420 : glob2.dina3InMM.h = 297

            grpStammdatenUebersicht.DataContext = myGlobalz.sitzung.aktVorgang.Stammdaten
            initTabcontrolsMaxheight()
            'setCanvasSize()
            nachricht("starteDetails")
            alte_gridsloeschen()
            alteDTsloeschen()

            tbWeitereBearbeiterStandard.Text = CLstart.myc.userIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")
            setComboboxRaumbezugtyp()
            glob2.initGemKRZCombo(Me)
            glob2.initCMBParagraf(Me)
            glob2.initRaumNrCombo(Me)

            'cmbGemKRZ.SelectedIndex = 46???
            'myglobalz.sitzung.aktVorgangsID = 9609

            If Not Stammdateneinlesen() Then
                Me.Close()
                Return False
            End If
            detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
            'detailsTools.initErgeinistypCombo(Me, "detail_GISHintergrund.xml", "XMLSourceComboBoxGISHintergrund") : cmbGISHintergrund.SelectedIndex = 0
            '####################### datenschutz 
            Dim lesezugriffErlaubt As Boolean = False
            If myGlobalz.zuhause Then
                lesezugriffErlaubt = True
            End If
            Dim HauptBearbeiter As String = ""
            Dim erlaubnisGrund As Integer = GetErlaubnisGrund(lesezugriffErlaubt, HauptBearbeiter)
            If Not lesezugriffErlaubt Then
                MsgBox(glob2.getMsgboxText("DSkeinZugriffAuf", New List(Of String)(New String() {CStr(myGlobalz.sitzung.aktVorgangsID), HauptBearbeiter})),
                    , "Datenschutz: Zugriff verweigert")
                nachricht("fehler Datenschutz_zugriff verweitert " & myGlobalz.sitzung.aktVorgangsID & ", " & myGlobalz.sitzung.aktBearbeiter.username)
                DS_Tools.DS_protokoll("Detailstartup", erlaubnisGrund, "-1")
                End
            End If
            '####################### datenschutz 
            setTitelleiste()
            initStammBearbeiterTooltip()
            initStammCheckBoxen()
            setzeErledigtflagfarbe()

            'detailsTools.OfficemerkerLoeschen(Nothing)
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, True)
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
            refreshverwandteServer(myGlobalz.sitzung.aktVorgangsID)
            refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
            refreshProjekt(myGlobalz.sitzung.aktVorgangsID)
            refreshKosten(myGlobalz.sitzung.aktVorgangsID)
            refreshIstConject(myGlobalz.sitzung.aktVorgangsID)
            initCombosVerwandte()
            initKostenFeld()
            If myGlobalz.sitzung.modus = "neu" Then
                TabControl1.SelectedIndex = 6
            End If

            If myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
                btnAllgemeinLoeschen.IsEnabled = True
            Else
                btnAllgemeinLoeschen.IsEnabled = False
            End If
            ' pruefeDeckblatt() 
            setWeitereBearbeiterListeDarstellen()
            detailsTools.VorgangLocking("ein")
            alleButtonsAusschalten()
            sachgebietsDBknopfEinschalten(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
            detailsTools.SG3307_eintragAnlegen(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
            projektDatenholen()
            prepareActionlog()
            CLstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
            CLstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
            CLstart.myc.aLog.komponente = "detail"
            CLstart.myc.aLog.aktion = "vorgang geoeffnet"
            CLstart.myc.aLog.log()
            LocalParameterFiles.erzeugeParameterDateiAktvorgang_txt(False, False)
            schlagworteWurdeGeaendert = False
            schlagworteEinfaerben()

            VerlaufmitDokumentenSetzen()
            cmbGemKRZ.SelectedItem = "Gemeinde"

            '  detailsTools.hatraumbezugDarstellen()
            nachricht("FORMSTART ERFOLGFREICH DURCHGEFÜHRT detail ######################################################### detail")
            initOptionen()
            initPumuckelVersion()
            If myGlobalz.PumuckelVersion = 2 Then
                '2 = kein pumuckel
                If myGlobalz.zuhause Then
                    btnNachVorlage.Visibility = Visibility.Visible
                Else
                    btnNachVorlage.Visibility = Visibility.Visible
                End If
                btnAktennotiz.Visibility = Visibility.Collapsed
                btnAkteziehen.Visibility = Visibility.Collapsed
            End If
            'ladevorgangAbgeschlossen = True
            Protokollzugriffschalten()
            'myGlobalz.historyMy = New CLstart.HistoryKookie.HistoryItem(myGlobalz.ClientCookieDir & "verlaufscookies")
            CLstart.HistoryKookie.HistoryItem.verlaufsCookieDir = myGlobalz.ClientCookieDir & "verlaufscookies"
            starteThumbnailer()
            tbClipText.Text = getTextFromCB()
            'If Application.zweiteInstanz Then
            '    stackDokuhinzu.IsEnabled = False
            '    btnAkteziehen.IsEnabled = False
            '    btnDokumentehinzu2.IsEnabled = False
            '    btnPDFScan1.IsEnabled = False
            '    btnPDFA.IsEnabled = False
            '    btnFotoshinzu2.IsEnabled = False
            'End If
            'starteWebbrowserControl()
            tbSuchSGnr.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl


            'initRechteDBControls(rrechtsdbARTcoll, "Select * from t36 as rechtsdb_art order by reihenf")
            'initRechteDBControls(rrechtsdbHerkunftcoll, "Select * from t37 as rechtsdb_herkunft order by reihenf")

            'initRechteDBControls(rrechtsdbARTcoll, "Select * from t36 as rechtsdb_art  ")
            'initRechteDBControls(rrechtsdbHerkunftcoll, "Select * from t37 as rechtsdb_herkunft ")
            cmbArt.SelectedValue = 6 ' 6 ist null
            cmbHerkunft.SelectedValue = 7 ' 7 ist null
            cmbArt.DataContext = rrechtsdbARTcoll : cmbHerkunft.DataContext = rrechtsdbHerkunftcoll
            spGesetzeSuchfunktion.Visibility = Visibility.Collapsed
            '   refreshGesetzdb(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl, GesetzListefirstTime)
            myGlobalz.colBearbeiterFDU = userTools.getBearbeiterCollection("select * from " & CLstart.myViewsNTabs.tabBearbeiter & "  where aktiv=1 order by  aktiv desc, nachNAME asc")
            userTools.getOnlineStatus(myGlobalz.colBearbeiterFDU)
            userTools.MakeKapitelsKontakte(myGlobalz.colBearbeiterFDU)
            MainListBox.ItemsSource = myGlobalz.colBearbeiterFDU


            myGlobalz.colBearbeiterBA = userTools.getBearbeiterCollectionBA("select personenid,nachname,vorname,fftelefon1,ffemail,bezirk,orgzusatz from " & CLstart.myViewsNTabs.TABSTAKEHOLDER & " as stakeholder" &
                                                                            " where rolle='Bauaufsicht' order by NACHNAME")

            l("nach myGlobalz.colBearbeiterBA")
            userTools.MakeKapitelsKontakte(myGlobalz.colBearbeiterBA)
            l("nach myGlobalz.colBearbeiterBA2")
            ListboxBauaufsicht.ItemsSource = myGlobalz.colBearbeiterBA
            l("nach myGlobalz.colBearbeiterBA3")
            'If Environment.UserName.ToLower = "weyers_g" Or
            '        Environment.UserName.ToLower = "nehler_u" Or
            '        Environment.UserName.ToLower = "feinen_j" Or
            '        Environment.UserName.ToLower = "klingler_b" Then
            '    hashtagvorlagenVerzeihnis = True
            'End If
            'If hashtagvorlagenVerzeihnis Then
            '    If Not myGlobalz.VorlagenRoot.Contains("hashtag") Then

            '        myGlobalz.VorlagenRoot = myGlobalz.VorlagenRoot.Replace("Vordruck_paradigma", "Vordruck_paradigma_hashtag")
            '    End If
            'End If
            tbuserAbsteract.Text = myGlobalz.sitzung.aktBearbeiter.getString(Environment.NewLine)

            'myGlobalz.zuhause = getZuhauseFromInifile()
            tbVorlagenRoot.Text = getVorlagenrootFromIniFile()
            tbVorlagenWork.Text = getVorlagenworkFromIniFile
            Return True
        Catch ex As Exception
            MsgBox(String.Format("Schwerer Fehler in der Startroutine! Bitte Admin informieren.{0}{1}", vbCrLf, ex))
            nachricht("Fehler Schwerer Fehler in der Startroutine! Bitte Admin informieren.", ex)
            Return False
        End Try
    End Function

    Private Function getVorlagenworkFromIniFile() As String
        Try
            l("getVorlagenworkFromIniFile---------------------- anfang")
            Dim wert As String
            wert = CLstart.myc.userIniProfile.WertLesen("diverse", "Vorlagenwork")
            If Not String.IsNullOrEmpty(wert) Then
                If wert.IsNothingOrEmpty Then
                    wert = ""
                Else
                    Return wert
                End If
            Else
                'standardwert setzen
                CLstart.myc.userIniProfile.WertSchreiben("diverse", "Vorlagenwork", wert)
                Return wert
            End If
        Catch ex As Exception
            nachricht("Fehler getVorlagenworkFromIniFile fehler", ex)
            Return ""
        End Try
    End Function

    Private Function getVorlagenrootFromIniFile() As String
        Try
            l("getVorlagenrootFromIniFile---------------------- anfang")
            Dim wert As String
            wert = CLstart.myc.userIniProfile.WertLesen("diverse", "Vorlagenroot")
            If Not String.IsNullOrEmpty(wert) Then
                If wert.IsNothingOrEmpty Then
                    wert = ""
                Else
                    Return wert
                End If
            Else
                'standardwert setzen
                CLstart.myc.userIniProfile.WertSchreiben("diverse", "Vorlagenroot", wert)
                Return wert
            End If
        Catch ex As Exception
            nachricht("Fehler getVorlagenrootFromIniFile fehler", ex)
            Return ""
        End Try
    End Function

    Private Function getZuhauseFromInifile() As Boolean
        Try
            l("getZuhauseFromInifile---------------------- anfang")
            Dim wert As String
            wert = CLstart.myc.userIniProfile.WertLesen("diverse", "zuhause")
            If Not String.IsNullOrEmpty(wert) Then
                If wert = "1" Then
                    cbzuhause.IsChecked = True
                    Return True
                Else
                    cbzuhause.IsChecked = False
                    Return False
                End If
            Else
                'standardwert setzen
                CLstart.myc.userIniProfile.WertSchreiben("diverse", "zuhause", CType(0, String))
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler getZuhauseFromInifile fehler", ex)
            Return False
        End Try
    End Function

    Private Sub refreshIstConject(aktVorgangsID As Integer)
        Try
            l(" MOD ---------------------- anfang")
            myGlobalz.sitzung.aktVorgang.istConjectVorgang = clsStammTools.getIstConjectVorgang(aktVorgangsID)
            If myGlobalz.sitzung.aktVorgang.istConjectVorgang Then
                If ladevorgangAbgeschlossen Then zeigeIstConjectFarbe()
                cbIstConject.IsChecked = True
            Else
                cbIstConject.IsChecked = False
                If ladevorgangAbgeschlossen Then zeigeIstNichtConjectFarbe()
            End If
            'grpStammdatenUebersicht.Background = Brushes.NavajoWhite
            'grpStammdatenUebersicht.ToolTip = "Wichtiger Hinweis: Dieser Vorgang wird auch in 'Bauantgrag-Online' bearbeitet. Uploaden Sie die Stellungnahme auch dorthin."

            l(" MOD ---------------------- ende")
        Catch ex As Exception
            l("Fehler in MOD: ", ex)
        End Try
    End Sub

    Private Sub initPumuckelVersion()
        Try
            l(" initPumuckelVersion ---------------------- anfang")
            Dim wert As String
            wert = CLstart.myc.userIniProfile.WertLesen("PUMUCKEL", "interop")
            If Not String.IsNullOrEmpty(wert) Then
                If wert = "1" Then
                    rbpumuckelversion1.IsChecked = CType(1, Boolean?)
                    'myGlobalz.PumuckelInteropVersionNutzen = 1
                    rbpumuckelversion1.IsChecked = True
                    myGlobalz.PumuckelVersion = 1
                End If
                If wert = "0" Then
                    rbpumuckelversion0.IsChecked = CType(0, Boolean?)
                    'myGlobalz.PumuckelInteropVersionNutzen = 0
                    rbpumuckelversion0.IsChecked = True
                    myGlobalz.PumuckelVersion = 0
                End If
                If wert = "2" Then
                    rbpumuckelversion0.IsChecked = CType(2, Boolean?)
                    'myGlobalz.PumuckelInteropVersionNutzen = 0
                    rbpumuckelversion2.IsChecked = True
                    myGlobalz.PumuckelVersion = 2
                End If
            Else
                CLstart.myc.userIniProfile.WertSchreiben("PUMUCKEL", "interop", CType(0, String))
                rbpumuckelversion0.IsChecked = CType(0, Boolean?)
                'myGlobalz.PumuckelInteropVersionNutzen = 0
                myGlobalz.PumuckelVersion = 0
            End If
            l(" initPumuckelVersion ---------------------- ende")
        Catch ex As Exception
            l("Fehler in initPumuckelVersion: ", ex)
        End Try
    End Sub



    'Private Sub starteWebbrowserControl()
    '    Try
    '        nachricht("USERAKTION: googlekarte  vogel")
    '        Dim gis As New clsGISfunctions
    '        Dim result As String
    '        result = gis.GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(CLstart.myc.kartengen.aktMap.aktrange, True)
    '        If result = "fehler" Or result = "" Then
    '        Else
    '            '  gis.starten(result)
    '            '  GMtemplates.templateStarten(result)
    '            wbSample.Navigate(New Uri(result))
    '        End If
    '        gis = Nothing
    '    Catch ex As Exception
    '        nachricht("fehler in starteWebbrowserControl1: ", ex)
    '    End Try
    'End Sub



    Private Sub starteThumbnailer()
        'Try
        '    nachricht("starteThumbnailer: ----------------------")
        '    nachricht("starteThumbnailer: " & initP.getValue("ExterneAnwendungen.APPLICATION_Thumbnailer"))
        '    Process.Start(initP.getValue("ExterneAnwendungen.APPLICATION_Thumbnailer"))
        'Catch ex As Exception
        '    nachricht("fehler in starteThumbnailer: " & initP.getValue("ExterneAnwendungen.APPLICATION_Thumbnailer"), ex)
        'End Try
    End Sub



    Private Shared Function GetErlaubnisGrund(ByRef lesezugriffErlaubt As Boolean, ByVal HauptBearbeiter As String) As Integer
        Dim erlaubnisGrund As Integer = 0
        If DS_Tools.istWeitererBearbeiter Then erlaubnisGrund = 1
        If DS_Tools.istHauptBearbeiter(HauptBearbeiter) Then erlaubnisGrund = 2
        If ds1Tools.istFachdienstLeitung(myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.aktBearbeiter.username, trenn) Then erlaubnisGrund = 3
        If DS_Tools.istFachdienstAssistenz Then erlaubnisGrund = 4
        If DS_Tools.istEinzelerlaubnis(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktBearbeiter.username) Then erlaubnisGrund = 5
        Dim gruppe As String = ""
        If DS_Tools.aktuserIstTeilDerGruppe(gruppe) Then erlaubnisGrund = 6

        If erlaubnisGrund > 0 Then
            If DS_Tools.istPersonalVorgang(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then
                If ds1Tools.istFachdienstLeitung(myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.aktBearbeiter.username, trenn) Then
                    erlaubnisGrund = 1500
                    lesezugriffErlaubt = True
                Else
                    'verweigert
                    erlaubnisGrund = -1500
                    lesezugriffErlaubt = False
                End If
            Else
                'ok zugriff reteilt
                lesezugriffErlaubt = True
            End If
        End If
        If Not lesezugriffErlaubt Then
            DS_Tools.DS_protokoll("Detailstartup", erlaubnisGrund, gruppe)
        End If
        Return erlaubnisGrund
    End Function
    Shared Function deckblattvorhanden() As Boolean
        If myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl = "562" Then Return True
        Return False
    End Function

    'Private Sub setCanvasSize()
    '    Try
    '        myCanvas.Height = initcanvasHeight() ' TabControl1.Heightddd
    '        myCanvas.Width = CLng(System.Windows.SystemParameters.PrimaryScreenWidth) - CLng(TabControl1.Width) - 15
    '        myCanvas2.Height = CLng(System.Windows.SystemParameters.PrimaryScreenHeight) - CLng(stckGISKopf.Height) - 100
    '        myCanvas2.Width = CLng(System.Windows.SystemParameters.PrimaryScreenWidth) - 20
    '        '   grpMinimapSteuerung.Width = myCanvas.Width
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler bei der Berechung der MINIGIS-Zeichenfläche")
    '        myCanvas.Height = 100
    '        myCanvas.Width = 150
    '    End Try
    'End Sub

    Sub initCombosVerwandte()
        Try
            cmbDokuverwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            cmbBeteiligteVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            cmbRBVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            cmbVerlaufVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
        Catch ex As Exception
            'MsgBox("Fehler in initCombosVerwandte." ,ex)
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
            l("Fehler in alteDTsloeschen.", ex)
        End Try
    End Sub


    Private Async Sub refreshBeteiligteListe(ByVal vid%)
        Try
            'Dim erfolg As Boolean = Await clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(vid)
            'Dim tast = New System.Threading.Tasks.Task(AddressOf clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(vid))
            Dim task As System.Threading.Tasks.Task(Of Boolean) = clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(vid)
            Dim result As Boolean = Await task
            displayBeteiligtenListe()
        Catch ex As Exception
            nachricht("fehler in refreshBeteiligteListe:  ", ex)
        End Try
    End Sub
    Private Sub setComboboxRaumbezugtyp()
        ComboBoxRaumbezug.Items.Add("Hinzufügen")
        ComboBoxRaumbezug.Items.Add("Adresse")
        ComboBoxRaumbezug.Items.Add("Flurstück")
        ComboBoxRaumbezug.Items.Add("Raumbezüge aus anderen Vorgängen")
        'ComboBoxRaumbezug.Items.Add("Punkt mit Umkreis")
        'ComboBoxRaumbezug.Items.Add("Polygon")
        ComboBoxRaumbezug.SelectedIndex = 0
    End Sub

    Sub setTitelleiste()
        If Environment.UserName.ToLower = "feinen_j" Then
            Title = detailsTools.settitle("Stammformular__, " &
", Eingang: " & myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum &
", Angelegt: " & myGlobalz.sitzung.aktVorgang.Stammdaten.Aufnahmedatum & " . Restplatz auf Server[GB]: " & gigabyte)
        Else
            Title = detailsTools.settitle("Stammformular__, " &
", Eingang: " & myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum &
", Angelegt: " & myGlobalz.sitzung.aktVorgang.Stammdaten.Aufnahmedatum)
        End If

        'If Application.zweiteInstanz Then
        '    Title = Title & "  Zweite Instanz"
        'End If
    End Sub


    Shared Function Stammdateneinlesen() As Boolean
        Return clsVorgangCTRL.leseVorgangvonDBaufObjekt(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten, myGlobalz.sitzung.VorgangREC)
    End Function



    Private Sub cmbVerlaufAuswahl_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbVerlaufAuswahl.SelectionChanged
        Try
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower = "hinzufügen" Or
            cmbVerlaufAuswahl.SelectedValue.ToString.ToLower = "" Then Exit Sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            detailsTools.eEreignisstarten(item, CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
            ' cmbVerlaufAuswahl.SelectedValue = ""
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            '     cmbStatus.SelectedValue = myGlobalz.sitzung.aktVorgang.Stammdaten.status ihah
            setzeErledigtflagfarbe()
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged", ex)
        End Try
    End Sub


    Private Async Sub refreshEreignisseListe(ByVal vid As Integer)
        Dim hinweis As String = ""
        ereignisDokExpand = CBool(chkExpandereignis.IsChecked)
        ereignisDokListe.Clear()
        Try
            dgEreignisse.DataContext = Nothing
        Catch ex As Exception
        End Try
        Try
            'Dim anzahl = detailsTools.initEreigisseDatatable(hinweis, vid, ereignisDokExpand, ereignisDokListe)
            Dim task As System.Threading.Tasks.Task(Of Integer) = detailsTools.initEreigisseDatatable(hinweis, vid, ereignisDokExpand, ereignisDokListe)
            Dim anzahl As Integer = Await task

            If anzahl > 0 Then
                Try
                    dgEreignisse.DataContext = ereignisDokListe ' myGlobalz.sitzung.EreignisseRec.dt
                Catch ex As Exception
                End Try
                tabheaderVerlauf.Header = "Verlauf " & myGlobalz.sitzung.EreignisseRec.dt.Rows.Count
            Else
                Try
                Catch ex As Exception
                End Try
                tabheaderVerlauf.Header = "Verlauf "
                dgEreignisse.DataContext = ereignisDokListe ' myGlobalz.sitzung.EreignisseRec.dt
            End If

        Catch ex As Exception
            nachricht("fehler in refreshEreignisseListe: ", ex)
        End Try
    End Sub

    Private Sub btnRefreshEreignisse_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: refresh ereignisse  ")
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        'refreshEreignisseListe(myglobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub


    Public Sub refreshRaumbezugsListe(ByRef vid As Integer, istnurverwandt As Boolean, firsttime As Boolean)
        Dim hinweis As String = ""
        Dim VerwandtschaftsStatusWert As Int16
        Try
            If Not myGlobalz.zuhause Then
                OptionMIniMapDarstellen()
            End If
            '  If Not CBool(chkMiniMapDarstellen.IsChecked) Then Exit Sub
            If istnurverwandt Then
                VerwandtschaftsStatusWert = 1
            Else
                VerwandtschaftsStatusWert = 0
            End If
            dgRaumbezug.DataContext = Nothing
            Dim erfolg As Boolean = RBtoolsns.initraumbezugsDT_alleDB.exe(vid)
            If erfolg Then RBtoolsns.statusSpalteErgaenzenUndFuellen.VerwandtschaftsStatusSpalteErgaenzenUndMitStandardFuellen(myGlobalz.sitzung.raumbezugsRec.dt,
                "STATUS", VerwandtschaftsStatusWert)
            If erfolg Then
                dgRaumbezug.DataContext = myGlobalz.sitzung.raumbezugsRec.dt
            Else
                dgRaumbezug.DataContext = Nothing
            End If
            tabheaderRaumbezug.Header = detailsTools.getRBheadertext("Raumbezug ", myGlobalz.sitzung.raumbezugsRec.dt)


            If myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug Then
                chkRaumbezuegeObsolet.IsChecked = True
            Else
                chkRaumbezuegeObsolet.IsChecked = False
            End If
            Dim _uselastrange As Boolean
            If firsttime Then
                _uselastrange = False
            Else
                _uselastrange = True
            End If

            INITMiniMapPresentation(myGlobalz.sitzung.raumbezugsRec.dt, _uselastrange)
            If Not myGlobalz.zuhause Then
                glob2.generateLayerWithShapes()

                '  If CBool(paradigmaKILLALLGIS.IsChecked) Then
                If CLstart.myc.userIniProfile.WertLesen("GISSTART", "allegissekillen") = "1" Then
                    alleFremdGISSEAbschiessen("MGIS") ': gisProcListe.Clear()
                End If
                If Not ladevorgangAbgeschlossen Then
                    If detailsTools.mitMiniMapDarstellen() Then
                        mgisStarten(CBool(paradigmaKILLALLGIS.IsChecked))
                    End If
                Else
                    If detailsTools.mitMiniMapDarstellen() Then
                        mgisStarten(CBool(paradigmaKILLALLGIS.IsChecked))
                    End If
                End If
            End If
            'starteWebbrowserControl()
        Catch ex As Exception
            nachricht("fehler in refreshRaumbezugsListe: ", ex)
        End Try
    End Sub

    'Public Sub gislink_click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Dim a = CType(e.Source, Button)
    '    showGIS4Raumbezug(CInt(a.Content))
    '    e.Handled = True
    'End Sub

    Sub zum_Ergeignis_Editmode()
        myGlobalz.sitzung.Ereignismodus = "edit"
        glob2._Ergeignis_edit(CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
    End Sub
    ''' <summary>
    ''' sollte immer zusammen mit  refreshFotos(myGlobalz.sitzung.VorgangsID)
    ''' verwendet werden
    ''' </summary>
    ''' <param name="VorgangsID"></param>
    ''' <remarks></remarks>
    Private Async Sub refreshDokumente(ByVal VorgangsID As Integer)
        Dim referenzvorgangsID As Integer = 0
        Dim anzahlEigeneDokumente As Integer = 0
        Dim anzahlReferenzDokumente As Integer = 0
        Try
            Dim task As System.Threading.Tasks.Task(Of Boolean)
            If Application.nurverwandte = 1 Then
                task = DokArcTools.dokusVonVorgangHolen.executeAsync(CStr(Application.givenVIDList), "keinefotos", alleBilder:=True, 0)
            Else

                task = DokArcTools.dokusVonVorgangHolen.executeAsync(CStr(VorgangsID), "keinefotos", alleBilder:=True, 0)
            End If
            Dim bresult As Boolean = Await task

            If bresult Then
                Psession.presDokus = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                anzahlEigeneDokumente = myGlobalz.Arc.ArcRec.dt.Rows.Count
            Else
                Psession.presDokus.Clear()
                dgVorgangDokumente.DataContext = Nothing
                myGlobalz.Arc.vorgangDocDt = Nothing
            End If
            referenzvorgangsID = VerwandteTools.divers.getReferenzvorgangsId(VorgangsID)

            If referenzvorgangsID > 0 Then
                Dim task2 As System.Threading.Tasks.Task(Of Boolean) = DokArcTools.dokusVonVorgangHolen.executeAsync(CStr(referenzvorgangsID), "keinefotos", alleBilder:=True, 0)
                Dim bresult2 As Boolean = Await task2
                Dim presdokVerwandte As New List(Of clsPresDokumente)
                presdokVerwandte = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                'Psession.presDokus = New List(Of clsPresDokumente)
                For Each dok As clsPresDokumente In presdokVerwandte
                    dok.istNurVerwandt = True
                    Psession.presDokus.Add(dok)

                    anzahlReferenzDokumente = myGlobalz.Arc.ArcRec.dt.Rows.Count
                Next
            End If

            detailsTools.thumbNailsHinzuFuegen(Psession.presDokus, initP.getValue("Haupt.ThumbNailsRoot"), ereignisDokListe)
            If mitMehrfachauswahl.IsChecked Then
                If alteMarkierungen.Count > 0 Then
                    altemarkierungen_anwenden(Psession.presDokus, alteMarkierungen)
                End If
            End If
            Psession.presDokus = Psession.presDokus.OrderByDescending(Function(x) x.Filedatum).ToList()

            dgVorgangDokumente.DataContext = Psession.presDokus 'myGlobalz.Arc.vorgangDocDt
            tabheaderDokumente.Header = "Dokumente " & anzahlEigeneDokumente
            If anzahlReferenzDokumente > 0 Then
                tabheaderDokumente.Header = "Dokumente " & anzahlEigeneDokumente & " + " & anzahlReferenzDokumente & " referenziert"
            End If
        Catch ex As Exception
            nachricht("fehler in refreshDokumente: ", ex)
        End Try
    End Sub



    Private Sub altemarkierungen_anwenden(presDokus As List(Of clsPresDokumente), alteMarkierungen As List(Of Integer))
        Try
            l(" MOD altemarkierungen_anwenden anfang")
            For Each dok As clsPresDokumente In presDokus
                dok.ausgewaehlt = False
                If istInMarkierungEnthalten(dok.DocID, alteMarkierungen) Then
                    dok.ausgewaehlt = True
                End If
            Next
            l(" MOD altemarkierungen_anwenden ende")
        Catch ex As Exception
            l("Fehler in altemarkierungen_anwenden: ", ex)
        End Try
    End Sub

    Private Function istInMarkierungEnthalten(docID As Integer, alteMarkierungen As List(Of Integer)) As Boolean
        Try
            l(" MOD istInMarkierungEnthalten anfang")
            For Each myint In alteMarkierungen
                If myint = docID Then
                    Return True
                End If
            Next
            l(" MOD istInMarkierungEnthalten ende")
            Return False
        Catch ex As Exception
            l("Fehler in istInMarkierungEnthalten: ", ex)
            Return False
        End Try
    End Function

    Private Async Sub refreshFotos(ByVal VorgangsID As Integer)
        Try
            'Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(VorgangsID, "nurfotos") ' nach myGlobalz.Arc.ArcRec
            Dim task As System.Threading.Tasks.Task(Of Boolean) = DokArcTools.dokusVonVorgangHolen.executeAsync(CStr(VorgangsID), "nurfotos", alleBilder:=True, 0)
            Dim bresult As Boolean = Await task
            If bresult Then
                Psession.presFotos = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                ' myGlobalz.Arc.vorgangFotoDt = myGlobalz.Arc.ArcRec.dt.Copy
                detailsTools.thumbNailsFotosHinzuFuegen(Psession.presFotos, ereignisDokListe)
                dgVorgangFotos.DataContext = Psession.presFotos 'myGlobalz.Arc.vorgangFotoDt
                tabheaderFotos.Header = "Fotos " & Psession.presFotos.Count
            Else
                Psession.presFotos = Nothing
                myGlobalz.Arc.vorgangFotoDt = Nothing
                dgVorgangFotos.DataContext = Nothing
                tabheaderFotos.Header = "Fotos"
            End If
        Catch ex As Exception
            nachricht("fehler in refreshFotos: ", ex)
        End Try
    End Sub


    Private Sub dokuhinzufuegenUndRefresh()
        Dim sollconject As Boolean = CBool(cbIstConject.IsChecked)
        If glob2.FktDokumentehinzu(0, sollconject) Then
            If sollconject Then
                cbIstConject.IsChecked = True
                myGlobalz.sitzung.aktVorgang.istConjectVorgang = clsStammTools.getIstConjectVorgang(myGlobalz.sitzung.aktVorgangsID)
                If myGlobalz.sitzung.aktVorgang.istConjectVorgang Then
                Else
                    clsStammTools.saveIstConject(myGlobalz.sitzung.aktVorgangsID, 1)
                    myGlobalz.sitzung.aktVorgang.istConjectVorgang = True
                End If

                zeigeIstConjectFarbe()
            End If
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
                refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
            Case "flurstück"
                Dim winflur As New Window_Flurstuecksauswahl
                winflur.ShowDialog()
                anychange = CBool(winflur.DialogResult)
                'Case "punkt mit umkreis"
                '    Dim winflur As New Win_punktMitUmkreis(UTMCoordinate.Text)
                '    winflur.ShowDialog()
                '    anychange = CBool(winflur.DialogResult)
                'Case "polygon"
                '    '  MsgBox("Sie können neue Polygone über die 'Fläche-Messen'- Funktion der Minimap erstellen.", MsgBoxStyle.Exclamation, "Neues Polygon hizufügen")
                '    'inputGetFlaeche()
                '    anychange = False
            Case "raumbezüge aus anderen vorgängen"
                Dim frb As New winFremdRBs
                frb.ShowDialog()
                refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, False)
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
        myGlobalz.Arc.AllesDokusAuscheckenVorgang(True, True)
        e.Handled = True
    End Sub

    'Sub showGIS4Raumbezug(ByVal id%)
    '    'todo
    '    If id > 0 Then
    '        'über die ID die Koordinaten holen
    '        'Dim pt As myPoint = DBraumbezug_Mysql.getCoords4ID_Raumbezug(id%)
    '        Dim pt As myPoint = RBtoolsns.getCoords4ID_Raumbezug_alleDB.exe(id%)
    '        Dim gis As New clsGISfunctions
    '        gis.GISAufruf_Mittelpunkt(pt)
    '    End If
    'End Sub


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
        Dim ausgabeVerzeichnis As String = ""
        myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
        'form aufrufen
        '	DokumentenArchiv.JPG_handeln(myGlobalz.sitzung.aktDokument)

        '	DokumentenArchiv.Archiv_definiereAktdokument(item)
        '	dgVorgangDokumente.SelectedItem = Nothing
        Dim handlenr As Integer = 0
        l("fotoeditmode")
        Dim darst As Boolean = detailsTools.Archiv_aktiviere_Dokument(False, CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked), "", alleBilder:=True, myGlobalz.sitzung.aktDokument.EreignisID)

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
    '        nachricht_und_Mbox("ComboBoxBeteiligte_SelectionChanged. " ,ex)
    '    End Try
    'End Sub

    Shared Function ComboBoxBeteiligteauswahlistOK(ByVal typ As String) As Boolean
        If typ.ToLower <> "hinzufügen" Then Return True
        Return False
    End Function

    'Public Shared Sub starteComboBoxBeteiligtedetail()
    '    myGlobalz.sitzung.BeteiligteModus = "neu"
    '    myGlobalz.sitzung.aktEreignis.Datum = Now
    '    Dim winadr As New Window_RB_Adresse
    '    winadr.ShowDialog()
    'End Sub

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
        altesRaumbezugsobjektLoeschen()
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
                    ParaUmkreisTools.zum_dgUmkreisEditmode("")'UTMCoordinate.Text)
                Case CInt(RaumbezugsTyp.Polygon).ToString, CInt(RaumbezugsTyp.Polyline).ToString
                    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktPolygon)
                    PolygonTools.zum_dgPolygonEditmode()

            End Select
            dgRaumbezug.SelectedItem = Nothing
            refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, True)
        Catch ex As Exception
            MessageBox.Show(String.Format("dgRaumbezug_SelectionChanged: {0}", ex))
        End Try
    End Sub

    Private Sub raumbezugHinzufuegenUndRefresh(ByVal rbtyp$)
        If Raumbezug_hinzufuegen(rbtyp) Then
            refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, True)

            ' HatRaumbezug in den  Stammdaten speichern 
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
            '  Dim erfolg As Boolean = VSTTools.editStammdaten_alleDB.exe(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten)
        End If
    End Sub



    Private Sub ComboBoxRaumbezug_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ComboBoxRaumbezug.SelectionChanged
        If ComboBoxRaumbezug.SelectedItem Is Nothing Then Exit Sub
        If ComboBoxRaumbezug.SelectedValue.ToString.ToLower = "hinzufügen" Then Exit Sub
        nachricht("USERAKTION: raumbezug hinzu ")
        Dim rbtyp As String = CType(ComboBoxRaumbezug.SelectedValue, String)
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
            'Dim item As DataRowView
            'item = CType(dgBeteiligte.SelectedItem, DataRowView)
            'If item Is Nothing Then Return
            myGlobalz.sitzung.BeteiligteModus = "edit"
            'clsBeteiligteBUSI.BeteiligtenRec2Obj(item, myGlobalz.sitzung.aktPerson)

            zum_Beteiligte_Editmode()
            refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        Catch ex As Exception
            nachricht("BeteiligtenAusgewaehlt: " & String.Format("BeteiligtenAusgewaehlt: {0}", ex))
        End Try
    End Sub

    Private Sub dgBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgBeteiligte.SelectionChanged
        e.Handled = True
        If dgBeteiligte.SelectedItem Is Nothing Then Exit Sub
        If ladevorgangAbgeschlossen = False Then Exit Sub
        Dim item As New Person
        Try
            myGlobalz.sitzung.aktPerson = CType(dgBeteiligte.SelectedItem, Person)
        Catch ex As Exception
            nachricht(ex.ToString)
            Exit Sub
        End Try
        If auswahlspalteBeteiligte.Visibility = Visibility.Collapsed Then
            BeteiligtenAusgewaehlt()
        End If

        dgBeteiligte.SelectedItem = Nothing
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

    'Private Sub dgVorgangDokumente_MouseRightButtonDown(sender As Object, e As MouseButtonEventArgs)
    '    'If Not formWindetailIsLoaded Then Exit Sub
    '    'Dim item As New clsPresDokumente
    '    'Try
    '    '    'item = CType(dgVorgangDokumente.SelectedItem, clsPresDokumente)
    '    '    item = CType(sender, clsPresDokumente)

    '    'Catch ex As Exception
    '    '    nachricht(ex.ToString)
    '    '    Exit Sub
    '    'End Try
    '    'Dim nck As TextBlock = CType(sender, TextBlock)
    '    'stContext.Visibility = Visibility.Collapsed
    '    'panningAusschalten()
    '    ''aktaid = CInt(nck.Tag)
    '    'Dim nlay As New clsLayerPres
    '    'nlay.aid = CInt(nck.Tag)
    '    'pgisTools.getStamm4aid(nlay)
    '    'showFreiLegende4Aid(nlay)
    '    e.Handled = True
    'End Sub

    Private Sub dgVorgangDokumente_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgVorgangDokumente.SelectionChanged
        e.Handled = True
        If dgVorgangDokumente.SelectedItem Is Nothing Then Exit Sub
        If ladevorgangAbgeschlossen = False Then Exit Sub
        Dim item As New clsPresDokumente
        Try
            item = CType(dgVorgangDokumente.SelectedItem, clsPresDokumente)
        Catch ex As Exception
            nachricht(ex.ToString)
            Exit Sub
        End Try

        'dokumenteRitemousekeypressed = False
        If Not auswahlspalteDokus.Visibility = Windows.Visibility.Visible Then
            myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            myGlobalz.sitzung.aktDokument = CType(item.Clone, Dokument)
            DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID)
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            Dim ausgabeVerzeichnis As String = ""
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
            Debug.Print(cbIstConject.IsChecked.ToString)
            dgVorgangDokumente_SelectionChanged_1Extracted("", True)
        End If

        dgVorgangDokumente.SelectedItem = Nothing
        dgVorgangFotos.SelectedItem = Nothing
        dgEreignisse.SelectedItem = Nothing
        e.Handled = True
    End Sub

    'Private Sub dgEreignisse_SelectionChanged(sender As Object , e As SelectionChangedEventArgs)

    'End Sub

    Private Sub dgEreignisse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgEreignisse.SelectionChanged
        e.Handled = True
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
                'refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            End If
            If EreignisDokTyp = "1" Then
                Dim aktThumbnailNotiz As String = ""
                'als Dokument laden
                aktThumbnailNotiz = detailsTools.getAktThumbNailNotiz(ereignisID, aktThumbnailNotiz, ereignisDokListe)

                myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                detailsTools.Dokument2Obj(item, myGlobalz.sitzung.aktDokument)
                DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID)
                myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                Dim ausgabeVerzeichnis As String = ""
                Debug.Print(myGlobalz.sitzung.aktDokument.EreignisID.ToString)
                myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
                dgVorgangDokumente_SelectionChanged_1Extracted(aktThumbnailNotiz, False)
            End If
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            'refreshDokumente(myglobalz.sitzung.aktVorgangsID)
            'refreshFotos(myglobalz.sitzung.aktVorgangsID)
            dgVorgangDokumente.SelectedItem = Nothing
            dgVorgangFotos.SelectedItem = Nothing
            'dgEreignisse.SelectedItem = Nothing
            e.Handled = True
        Catch ex As Exception
            nachricht(String.Format("Fehler: dgEreignisse_SelectionChanged 1: {0}", ex))
        End Try
    End Sub



    Public Sub btnFotoGucker_ClickExtracted()
        If DokArc.aktiviereFotoGucker(myglobalz.sitzung.aktVorgangsID, Application.givenVIDList, myglobalz.OhneObsoletenDokus, myglobalz.sitzung.aktEreignis.ID, alleBilder:=True) > 0 Then
            Dim winfotoguck = New winFotoGucker
            winfotoguck.ShowDialog()
            glob2.MeinGarbage()
            refreshFotos(myglobalz.sitzung.aktVorgangsID)
            'dgVorgangFotos.DataContext = Psession.presFotos 'myGlobalz.Arc.vorgangFotoDt        
        Else
            nachricht_und_Mbox("Keine Fotos gefunden")
        End If
    End Sub
    Private Sub btnBeteiligteRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBeteiligteRefresh.Click
        resetBeteiligteliste_ClickExtracted()
    End Sub
    Private Sub SetzeStammdatenExplizitInsUI()
        Debug.Print(myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        lblAZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt
        Label14.Text = bildeLabel14()
        tbBeschreibung2.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung
        lblVorgangsID.Text = CStr(myGlobalz.sitzung.aktVorgangsID)
        tbBemerkung.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung
        tbStandort.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.RaumNr
        tbParagraph.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Paragraf
        tbProbaugAZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz
        tbAltAz.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.AltAz
        tbGEMKRZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ
        tbInternenr.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.InterneNr
        tbBearbeiter.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale
        tbWeitereBearbeiter.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
    End Sub

    Private Shared Function bildeLabel14() As String
        Dim retval As String = ""
        If myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header.StartsWith(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then
            retval = myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header
        Else
            retval = myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl & "-" &
         myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header
        End If
        Return retval
    End Function
    Private Function read_nurEinBildschirm() As Boolean
        Dim datei = "c:\kreisoffenbach\nureinbildschrm.txt"
        Dim summe As String = ""
        Try
            Dim fi As New IO.FileInfo(datei)
            If fi.Exists Then
                summe = IO.File.ReadAllText(datei)
                If summe.ToLower.Contains("true") Then
                    'cbNurEinBildschirm.IsChecked = True
                    Return True
                Else
                    'cbNurEinBildschirm.IsChecked = False
                    Return False
                End If
            End If
        Catch ex As Exception
            l("Fehler: ", ex)
            Return False
        End Try
    End Function
    Private Sub Window_Detail_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True

        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "feinen_j" Then
            btnTxtfileanlegen.Visibility = Visibility.Visible
        Else
            btnTxtfileanlegen.Visibility = Visibility.Collapsed
        End If
        Debug.Print("givenVID" & Application.givenVID)
        Debug.Print("givenEID" & Application.givenEID)
        Debug.Print("givendocID" & Application.givenDOCID)
        Debug.Print("givenVIDList" & Application.givenVIDList)
        Debug.Print("nurverwandte" & Application.nurverwandte)

        IO.Directory.SetCurrentDirectory("C:\kreisoffenbach\prjfotogugger")
        If Not read_nurEinBildschirm() Then

            initStartPositionOnScreen()
        End If
        mgistools.mgisAktualisieren()
        Debug.Print(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kontakt.elektr.Email)
        If Not starteDetails() Then
            myGlobalz.einVorgangistgeoeffnet = False
            Me.Close()
        Else
            myGlobalz.einVorgangistgeoeffnet = True
        End If
        cmbVerlaufAuswahl.SelectedIndex = 0
        SetzeStammdatenExplizitInsUI()
        nachricht(cmbVerlaufAuswahl.Items.Count.ToString)
        schlagworteWurdeGeaendert = False
        resizeWindow(3 / 5)
        initTimetool()
        initcbreadOnlyDoxsInTxtCrtlOeffnen()
        'initDarkTheme()
        kopp.DataContext = myGlobalz.sitzung
        ladevorgangAbgeschlossen = True
        Me.Activate()
        Me.Width = 10 : Me.Height = 10
        '
        myglobalz.sitzung.aktEreignis.ID = Application.givenEID
        myglobalz.sitzung.aktVorgangsID = CInt(Application.givenVID)
        myglobalz.sitzung.aktDokument.DocID = Application.givenDOCID

        l("loaded Application.givenEID" & Application.givenEID)
        l("loaded Application.givenVID" & Application.givenVID)
        l("loaded Application.givenVIDlist" & Application.givenVIDList)
        l("loaded Application.givenDOCID" & Application.givenDOCID)
        l("loaded Application.nurverwandte" & Application.nurverwandte)
        Dim allebilder As Boolean = False
        If Application.givenEID < 1 Then
            allebilder = True
        End If
        Dim aktdoku As New Dokument
        If Application.nurverwandte = 0 Then
            aktdoku = DokArcTools.divers.getDokumentforDocid(Application.givenDOCID, Psession.presFotos)
        End If

        If DokArc.aktiviereFotoGucker(myglobalz.sitzung.aktVorgangsID, Application.givenVIDList, myglobalz.OhneObsoletenDokus,
                                      myglobalz.sitzung.aktEreignis.ID, allebilder) > 0 Then
            If aktdoku Is Nothing Then
                Dim winfotoguck = New winFotoGucker
                winfotoguck.ShowDialog()
            Else
                Dim winfotoguck = New winFotoGucker(aktdoku)
                winfotoguck.ShowDialog()
            End If

            glob2.MeinGarbage()
        Else
            nachricht_und_Mbox("Keine Fotos gefunden")
        End If
        End
    End Sub

    Private Sub initDarkTheme()
        If CLstart.myc.userIniProfile.WertLesen("GISSTART", "cbDarkTheme") = "1" Then
            cbDarkTheme.IsChecked = True
            dunkelThemePrepare(True)
        Else
            cbDarkTheme.IsChecked = False
        End If
    End Sub

    Private Sub dunkelThemePrepare(dunkelTheme As Boolean)
        If dunkelTheme Then
            Dim dunkelHgrund, vorneGrund As SolidColorBrush
            'gründunkel
            dunkelHgrund = New SolidColorBrush(Windows.Media.Color.FromRgb(20, 50, 20))
            vorneGrund = Brushes.LightGreen 'New SolidColorBrush(Windows.Media.Color.FromRgb(20, 50, 20))
            'graudunkel
            dunkelHgrund = New SolidColorBrush(Windows.Media.Color.FromRgb(60, 60, 60))
            vorneGrund = New SolidColorBrush(Windows.Media.Color.FromRgb(190, 190, 190)) 'Brushes.LightGray ' 
            dunklesThema(dunkelHgrund, vorneGrund)
        End If
    End Sub

    Private Sub dunklesThema(dunkelHgrund As SolidColorBrush, vorneGrund As SolidColorBrush)
        'Exit Sub
        tabheaderVerlauf.Style = CType(Me.FindResource("TabNeu"), Style)

        tiGeoportal.Background = CType(Me.FindResource("SelectedTabTextBrush"), Brush)
        grpStammdatenUebersicht.Background = dunkelHgrund : grpStammdatenUebersicht.Foreground = vorneGrund
        tbControl1.Background = dunkelHgrund ': tbControl1.Foreground = vorneGrund tbControl1

        tiSachdaten.Background = dunkelHgrund
        'tiSachdaten.Foreground = vorneGrund

        tiGeoportal.Background = dunkelHgrund
        'tiGeoportal.Foreground = vorneGrund

        tiOptionen.Background = dunkelHgrund
        'tiOptionen.Foreground = vorneGrund 
        'tiWerkzeuge.Foreground = vorneGrund
        'tiOptionen.Foreground = vorneGrund 
        'dgRowStyleDokument 
        dgVorgangFotos.Background = dunkelHgrund : dgVorgangFotos.Foreground = vorneGrund
        dgVorgangDokumente.Background = dunkelHgrund : dgVorgangDokumente.Foreground = vorneGrund
        dgRaumbezug.Background = dunkelHgrund : dgRaumbezug.Foreground = vorneGrund
        dgBeteiligte.Background = dunkelHgrund : dgBeteiligte.Foreground = vorneGrund
        dgVerwandte.Background = dunkelHgrund : dgVerwandte.Foreground = vorneGrund
        dgVerwandteServer.Background = dunkelHgrund : dgVerwandteServer.Foreground = vorneGrund
        dgEreignisse.Background = dunkelHgrund : dgEreignisse.Foreground = vorneGrund

        dgEreignisse.RowStyle = CType(Me.FindResource("dgRowStyleVerlaufDARK"), Style)
        dgVorgangFotos.RowStyle = dgEreignisse.RowStyle
        dgBeteiligte.RowStyle = dgEreignisse.RowStyle
        dgRaumbezug.RowStyle = dgEreignisse.RowStyle
        dgVorgangDokumente.RowStyle = dgEreignisse.RowStyle


        grpbauantrag.Style = CType(Me.FindResource("gbDark"), Style)
        grpStammdatenUebersicht.Style = grpbauantrag.Style
        grpBoxKosten.Style = grpbauantrag.Style
        grpBVwBeteiligte.Style = grpbauantrag.Style
        grpBVwDokumente.Style = grpbauantrag.Style
        grpBVwRaumbezug.Style = grpbauantrag.Style
        grpBVwVErlauf.Style = grpbauantrag.Style
        grpEreignishinzu.Style = grpbauantrag.Style
        grpGisstart.Style = grpbauantrag.Style
        grpDokumenthinzu.Style = grpbauantrag.Style

        grpNeu.Style = grpbauantrag.Style
        grpKontakte.Style = grpbauantrag.Style
        grpFotoMehrfachauswahl.Style = grpbauantrag.Style
        'MainListBox.Style = grpbauantrag.Style





        stackDokuhinzu.Background = dunkelHgrund


        StackPanelVerlauf.Background = dunkelHgrund
        stpVerschiedenes.Background = dunkelHgrund
        stpBauaufischt.Background = dunkelHgrund
        dpBeteiligte.Background = dunkelHgrund
        dpRB.Background = dunkelHgrund
        dpDokus.Background = dunkelHgrund
        tiverwDatenquellen.Background = dunkelHgrund
        ListboxBauaufsicht.Background = dunkelHgrund


        kopp.Background = dunkelHgrund
        Werkzeuge.Background = dunkelHgrund
        stackp1.Background = dunkelHgrund

        lblAZ.Background = dunkelHgrund
        tbBeschreibung2.Background = dunkelHgrund
        tbBemerkung.Background = dunkelHgrund
        tbProbaugAZ.Background = dunkelHgrund
        tbAltAz.Background = dunkelHgrund
        tbInternenr.Background = dunkelHgrund

        lblAZ.Foreground = vorneGrund
        tbBeschreibung2.Foreground = vorneGrund
        tbBemerkung.Foreground = vorneGrund
        tbProbaugAZ.Foreground = vorneGrund
        tbAltAz.Foreground = vorneGrund
        tbInternenr.Foreground = vorneGrund
        tbneuerVorgang.Foreground = vorneGrund

        'btnConfirmBeschreibung2.Background = dunkelHgrund
        'btnConfirmBeschreibung2.Foreground = vorneGrund




        TabControl2.Background = dunkelHgrund
        dgEreignisse.Background = dunkelHgrund
        dgVorgangDokumente.Background = dunkelHgrund
        dgRaumbezug.Background = dunkelHgrund
        dgBeteiligte.Background = dunkelHgrund
        dgVerwandte.Background = dunkelHgrund
        dgVorgangFotos.Background = dunkelHgrund

        ckbGutachtenInDokumente.Foreground = vorneGrund
        ckbGutachtenvorhanden.Foreground = vorneGrund
        chkdarfnichtvernichtetwerden.Foreground = vorneGrund
        chkAktVorgangSchliessen.Foreground = vorneGrund
        chkExpandereignis.Foreground = vorneGrund
        cbIstConject.Foreground = vorneGrund
        tbConjectDropText.Foreground = dunkelHgrund
        mitMehrfachauswahlFotos.Foreground = vorneGrund
        'gpFotoMehrfachauswahl.Foreground = vorneGrund
        'grpBVwVErlauf.Foreground = vorneGrund
        'tbVerlaugf01.Foreground = vorneGrund
        'gbEreignishinzu.Foreground = vorneGrund
        'gbDokumenthinzu.Foreground = vorneGrund
        'gbGisstart.Foreground = vorneGrund
        'grpbauantrag.Foreground = vorneGrund

        'mäuseklavier
        chkboxInterneZahlung.Foreground = vorneGrund
        chkboxverwaltungsgebuehr.Foreground = vorneGrund
        verwaltungsgebuehrBezahlt.Foreground = vorneGrund
        VERWARNUNGSGELD.Foreground = vorneGrund
        VerwarnungsgeldBezahlt.Foreground = vorneGrund
        BUSSGELD.Foreground = vorneGrund
        BUSSGELDBezahlt.Foreground = vorneGrund
        Zwangsgeld.Foreground = vorneGrund
        ZwangsgeldBezahlt.Foreground = vorneGrund
        ersatzgeld.Foreground = vorneGrund
        ersatzgeldBezahlt.Foreground = vorneGrund
        ersatzgeldausgezahlt.Foreground = vorneGrund
        sicherheit.Foreground = vorneGrund
        sicherheitBezahlt.Foreground = vorneGrund
        beihilfe.Foreground = vorneGrund
        beihilfeBezahlt.Foreground = vorneGrund

        MainListBox.Foreground = vorneGrund
        MainListBox.Background = dunkelHgrund

        MainListBox.Foreground = vorneGrund
        ListboxBauaufsicht.Foreground = vorneGrund



        grpBoxKosten.Foreground = vorneGrund

        'optionen
        radWindowsSchriftKlein.Foreground = vorneGrund
        radWindowsSchriftMittel.Foreground = vorneGrund
        chkMiniMapDarstellen.Foreground = vorneGrund
        chkNoGISever.Foreground = vorneGrund
        chkGISimmerbeenden.Foreground = vorneGrund
        tbWeitereBearbeiterStandard.Foreground = vorneGrund
        tbhinewris23.Foreground = vorneGrund
        tbuserAbsteract.Foreground = vorneGrund
        paradigmaKILLALLGIS.Foreground = vorneGrund
        rbpumuckelversion0.Foreground = vorneGrund
        rbpumuckelversion1.Foreground = vorneGrund
        rbpumuckelversion2.Foreground = vorneGrund
        cbreadOnlyDoxsInTxtCrtlOeffnen.Foreground = vorneGrund
        tb34.Foreground = vorneGrund
        tb35.Foreground = vorneGrund
        tb36.Foreground = vorneGrund
        tb37.Foreground = vorneGrund
        tb38.Foreground = vorneGrund

        grpBVwBeteiligte.Foreground = vorneGrund
        tb39.Foreground = vorneGrund
        'tb40.Foreground = vorneGrund  soll blau bleiben
        tb41.Foreground = vorneGrund
        mitBeteiligteMehrfachauswahl.Foreground = vorneGrund

        grpBVwRaumbezug.Foreground = vorneGrund
        tb42.Foreground = vorneGrund
        grpBVwDokumente.Foreground = vorneGrund
        mitMehrfachauswahl.Foreground = vorneGrund
        tb43.Foreground = vorneGrund
        tb44.Foreground = vorneGrund

        gbverwDatenQuelle.Foreground = dunkelHgrund

        'tiVerwandteServer.Foreground = vorneGrund
        'tiverwDatenquellen.Foreground = vorneGrund

        tiVerwandteServer.Background = dunkelHgrund
        tiverwDatenquellen.Background = dunkelHgrund
        Tab_Verwandte.Background = dunkelHgrund

        gb01.Foreground = vorneGrund
        gb02.Foreground = vorneGrund
        gb03.Foreground = vorneGrund
        gb04.Foreground = vorneGrund
        gb05.Foreground = vorneGrund
        gb06.Foreground = vorneGrund
        gb07.Foreground = vorneGrund
        gb09.Foreground = vorneGrund
        gb08.Foreground = vorneGrund

    End Sub

    Private Sub initTimetool()
        'If Not ladevorgangAbgeschlossen Then Exit Sub
        If CLstart.myc.userIniProfile.WertLesen("GISSTART", "timetoolaktiv") = "1" Then
            cbtimetool.IsChecked = True
        Else
            cbtimetool.IsChecked = False
        End If
    End Sub
    Private Sub initcbreadOnlyDoxsInTxtCrtlOeffnen()
        'If Not ladevorgangAbgeschlossen Then Exit Sub
        If CLstart.myc.userIniProfile.WertLesen("GISSTART", "readOnlyDoxsInTxtCrtlOeffnen") = "1" Then
            cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked = True
        Else
            cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked = False
        End If
    End Sub


    Private Sub initStartPositionOnScreen()
        If myGlobalz.nureinbildschirm Then Exit Sub
        If Environment.UserName.ToLower = "petersdorff_l" Then
            'MsgBox("version 111")
            Me.Width = CLstart.formposition.getPosition("diverse", "winfotoguggerformpositionwidth", Me.Width)
            Me.Height = CLstart.formposition.getPosition("diverse", "winfotoguggerformpositionheight", Me.Height)
        End If

        Dim topval = (CLstart.formposition.getPosition("diverse", "winfotoguggerformpositiontop", Me.Top))
        If topval < 0 Then
            Me.Top = 0
        Else
            Me.Top = topval
        End If
        Me.Left = CLstart.formposition.getPosition("diverse", "winfotoguggerformpositionleft", Me.Left)
        Me.Width = CLstart.formposition.getPosition("diverse", "winfotoguggerformpositionwidth", Me.Width)
        Me.Height = CLstart.formposition.getPosition("diverse", "winfotoguggerformpositionheight", Me.Height)
    End Sub

    Private Sub btnAllgemeinLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAllgemeinLoeschen.Click
        e.Handled = True
        If Not myGlobalz.sitzung.EreignisseRec.dt.IsNothingOrEmpty Then 'Rows.Count > 0 Then
            MessageBox.Show("Dem Vorgang sind noch Ereignisse zugeordnet. " + vbCrLf &
         "Bitte löschen Sie zuerst alle Ereignisse. " + vbCrLf &
         "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If
        If Not myGlobalz.sitzung.raumbezugsRec.dt.IsNothingOrEmpty Then
            MessageBox.Show("Dem Vorgang sind noch Raumbezüge zugeordnet. " + vbCrLf &
         "Bitte löschen Sie zuerst alle Raumbezüge. " + vbCrLf &
         "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If
        If myGlobalz.Arc.vorgangDocDt.IsNothingOrEmpty Then
            'löschenOK	 				  
        Else
            MessageBox.Show("Dem Vorgang sind noch Dokumente zugeordnet. " + vbCrLf &
         "Bitte löschen Sie zuerst alle Dokumente. " + vbCrLf &
         "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
            Exit Sub
            '	MessageBox.Show("Dem Vorgang sind noch Dokumente zugeordnet. " + vbCrLf & _
            '"Bitte löschen Sie zuerst alle Dokumente. " + vbCrLf & _
            '"Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
        End If

        If Not myGlobalz.sitzung.beteiligteREC.dt.IsNothingOrEmpty Then
            MessageBox.Show("Dem Vorgang sind noch Beteiligte zugeordnet. " + vbCrLf &
         "Bitte löschen Sie zuerst alle Beteiligten. " + vbCrLf &
         "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If

        If Not String.IsNullOrEmpty(CLstart.myc.aktprojekt.BezeichnungLang) Then
            MessageBox.Show("Dem Vorgang ist noch ein Projekt zugeordnet. " + vbCrLf &
    "Bitte löschen Sie Verbindung zum Projekt (Unter Stammdaten). " + vbCrLf &
    "Danach kann der Vorgang gelöscht werden!", "Vorgang löschen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)

            Exit Sub
        End If

        If Not glob2.istloeschenErnstgemeint Then Exit Sub
        clsVorgangCTRL.LoescheVorgang()
        'btnAllgemein.IsEnabled = False
        e.Handled = True
        Me.Close()
    End Sub
    Private Sub Verwandte_hinzufuegen(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: verwandte hinzu ")
        Verwandte_hinzufuegenExtracted()
    End Sub

    Private Sub Verwandte_hinzufuegenExtracted()
        ' Dim dumm1 As String = ""
        '  clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(dumm1, dumm1, alter_titel, alter_probaugAz, altergemKRZ)
        Dim neuLInk As New winlinken(clsStartup.vorherigerVorgang)
        neuLInk.ShowDialog()
        If neuLInk.istConject Then
            cbIstConject.IsChecked = True
            myGlobalz.sitzung.aktVorgang.istConjectVorgang = True
            clsStammTools.saveIstConject(myGlobalz.sitzung.aktVorgangsID, 1)
            zeigeIstConjectFarbe()
        End If
        If CBool(neuLInk.DialogResult) Then
            refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
        End If
    End Sub
    Sub refreshverwandte(ByVal vid As Integer)
        Try
            Dim sql As String = "select * from  " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG & "  where vorgangsid=" & vid
            VerwandteTools.erzeugeVerwandtenlistezuVorgang.exe(sql, myGlobalz.sitzung.tempREC)
            myGlobalz.sitzung.VerwandteDT.Clear()
            myGlobalz.sitzung.VerwandteDT = myGlobalz.sitzung.tempREC.dt.Copy
            dgVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
            initCombosVerwandte()
            If Not myGlobalz.sitzung.VerwandteDT.IsNothingOrEmpty Then
                'tabheaderVerwandte.Header = "Verwandte " & myGlobalz.sitzung.VerwandteDT.Rows.Count
                tabheaderVerwandte.Header = tabheaderVerwandte.Header.ToString & "/" & myGlobalz.sitzung.VerwandteDT.Rows.Count
                VerwandteGroupboxenEnabled(True)
            Else
                tabheaderVerwandte.Header = "Verwandte "
                VerwandteGroupboxenEnabled(False)
            End If
        Catch ex As Exception
            nachricht("fehler in refreshverwandte: ", ex)
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
            'Dim BeteilitenObjListe As New List(Of Person)
            'BeteilitenObjListe = clsBeteiligteBUSI.ConvertDatatable2Personenliste(myGlobalz.sitzung.beteiligteREC.dt)
            Psession.presBeteiligte = clsBeteiligteBUSI.ConvertDatatable2Personenliste(myGlobalz.sitzung.beteiligteREC.dt) 'detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
            'dgBeteiligte.DataContext = myGlobalz.sitzung.beteiligteREC.dt         
            dgBeteiligte.DataContext = Psession.presBeteiligte
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
        refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub dgVerwandte_SelectionChangedExtracted()
        Try
            If dgVerwandte.SelectedItem Is Nothing Then Exit Sub
            Dim item As DataRowView = CType(dgVerwandte.SelectedItem, DataRowView)
            geaenderteStammdatenSpeichern()
            Dim vid$ = item("FREMDVORGANGSID").ToString
            Dim koppelingsid As Integer = CInt(item("ID").ToString)
            Verwandte_verarbeiten(koppelingsid, CInt(vid$), item("Titel").ToString)
        Catch ex As Exception
            nachricht("Sie haben in eine leere Zeile geklickt. Bitte versuchen Sie es nochmal.", ex)
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
    End Sub



    Private Sub cmbDokuverwandte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbDokuverwandte.SelectedItem Is Nothing Then Exit Sub
        If cmbDokuverwandte.SelectedValue Is Nothing Then Exit Sub
        refreshDokumente(CInt(cmbDokuverwandte.SelectedValue.ToString))
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub resetDokuliste_ClickExtracted()


        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        'refreshDokumente(myglobalz.sitzung.aktVorgangsID)
        'refreshFotos(myglobalz.sitzung.aktVorgangsID)

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


    Private Sub cmbRBverwandte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbRBVerwandte.SelectedItem Is Nothing Then Exit Sub
        If cmbRBVerwandte.SelectedValue Is Nothing Then Exit Sub
        refreshRaumbezugsListe(CInt(cmbRBVerwandte.SelectedValue.ToString), True, True)
    End Sub

    Private Sub resetRBliste_ClickExtracted()
        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, True)
        cmbRBVerwandte.SelectedItem = Nothing
    End Sub
    Private Sub resetRBliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
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
        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, True)
        e.Handled = True
    End Sub

    Sub berechneGlobalRange(ByVal globalrange As clsRange)
        nachricht("berechneGlobalRange ----------------------------")
        '470531, 503699, 5532582, 5553740
        globalrange.xl = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Xmin")) ' myGlobalz.GIS_Rahmen_Fit_Xmin
        globalrange.yl = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Ymin")) 'myGlobalz.GIS_Rahmen_Fit_Ymin
        globalrange.xh = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Xmax")) 'myGlobalz.GIS_Rahmen_Fit_Xmax
        globalrange.yh = CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_ymax")) 'myGlobalz.GIS_Rahmen_Fit_ymax
        nachricht("berechneGlobalRange ---------------ende -------------")
    End Sub

    Sub INITMiniMapPresentation(ByVal mydt As DataTable, useLastRange As Boolean) 'myGlobalz.sitzung.raumbezugsRec.dt
        nachricht("INITMiniMapPresentation ----------------------------")
        Try

            If useLastRange Then
                'CLstart.myc.kartengen.aktMap.aktrange bleibt unverändert
                'berechneGlobalRange(CLstart.myc.globalrange)
                'clsMiniMapTools.initMinimapParameter(CLstart.myc.globalrange, CLstart.myc.raumberange)
            Else
                RBtoolsns.berechneRaumbezugsrange.execute(CLstart.myc.raumberange, mydt)
                berechneGlobalRange(CLstart.myc.globalrange)
                'clsMiniMapTools.initMinimapParameter(CLstart.myc.globalrange, CLstart.myc.raumberange)
            End If

            nachricht("lesecanvaasize  --------------------anfang --------")
            'If chkBIGGIS.IsChecked Then
            '    CLstart.myc.kartengen.aktMap.aktcanvas.w = CLng(myCanvas2.Width)
            '    CLstart.myc.kartengen.aktMap.aktcanvas.h = CLng(myCanvas2.Height)
            'Else
            '    CLstart.myc.kartengen.aktMap.aktcanvas.w = CLng(myCanvas.Width)
            '    CLstart.myc.kartengen.aktMap.aktcanvas.h = CLng(myCanvas.Height)
            'End If
            nachricht("lesecanvaasize  --------------------ende --------")
            'setzeVordergrundThemaUndRefresh("realshapeopak;")
            nachricht("INITMiniMapPresentation -------------------ende ---------")
        Catch ex As Exception
            nachricht("Fehler in INITMiniMapPresentation: ", ex)
        End Try
    End Sub

    Sub alte_gridsloeschen()
        nachricht("gridsloeschen")
        dgRaumbezug.DataContext = Nothing
        dgEreignisse.DataContext = Nothing
        dgVorgangDokumente.DataContext = Nothing
        dgBeteiligte.DataContext = Nothing
    End Sub




    Private Sub copyBeteiligteListe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If cmbBeteiligteVerwandte.SelectedValue Is Nothing Then
            MessageBox.Show("Sie müssen zuerst einen verwandten Vorgang auswählen!", "Daten aus verwandtem Vorgang übernehmen", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            Exit Sub
        End If
        Dim quellVid% = CInt(cmbBeteiligteVerwandte.SelectedValue.ToString)
        '  clsBeteiligteBUSI.verlinkenVonBeteiligten(quellVid, myGlobalz.sitzung.VorgangsID)
        If vid_istOk(quellVid) Then detailsTools.AlleBeteiligtenKopieren(quellVid, myGlobalz.sitzung.aktVorgangsID) ' myGlobalz.sitzung.beteiligteREC.dt
        refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub






    Sub setBoundingRefresh(ByVal myrange As clsRange) 'ByVal xl As Double, ByVal xh As Double, ByVal yl As Double, ByVal yh As Double)
        CLstart.myc.kartengen.aktMap.aktrange.rangekopierenVon(myrange)
        'xdifKorrektur
        If CLstart.myc.kartengen.aktMap.aktrange.xdif() < 1 Then CLstart.myc.kartengen.aktMap.aktrange.xh += 1
        If CLstart.myc.kartengen.aktMap.aktrange.ydif() < 1 Then CLstart.myc.kartengen.aktMap.aktrange.yh += 1
        presentMapOLD()
    End Sub



    Sub presentMapOLD()
        Exit Sub
        'Dim cachedir As String = initP.getValue("GisServer.gisCacheDir")
        'If Not mitMiniMapDarstellen() Then
        '    setNomapBitmap()
        '    Exit Sub
        'End If

        'If myglobalz.sitzung.raumbezugsRec.dt.Rows.Count < 1 Then
        '    'setNomapBitmap()
        '    'Exit Sub
        'End If
        'nachricht("presentMap: ---------------------- ")
        'Try
        '    zeigeKartenbreiteTextInMinimap()
        '    drawprogressbar()
        '    'clsMiniMap.korrigiereAktrange(clstart.myc.kartengen.aktMap.aktrange, myCanvas)
        '    Dim pixcanvas As New clsCanvas
        '    If chkBIGGIS.IsChecked Then
        '        pixcanvas.w = CLng(myCanvas2.Width)
        '        pixcanvas.h = CLng(myCanvas2.Height)
        '    Else
        '        pixcanvas.w = CLng(myCanvas.Width)
        '        pixcanvas.h = CLng(myCanvas.Height)
        '    End If

        '    Dim handle As New LIBmapScaling.clsScalierung
        '    'handle = New LIBmapScaling.clsScalierung
        '    nachricht("presentMap: vor skaliereung ")
        '    LIBmapScaling.clsScalierung.Skalierung(72, "ZB", 1, CLstart.myc.kartengen.aktMap.aktrange, CInt(pixcanvas.w), CInt(pixcanvas.h), 1,
        '                                       CLstart.myc.kartengen.aktMap.aktrange, pixcanvas)
        '    nachricht("presentMap: nach skaliereung ")

        '    If Not clsAufrufgenerator.istCacheOK(cachedir) Then Exit Sub
        '    CLstart.myc.kartengen.mapcred.DateinamensSchwanz = clsMiniMapTools.makeOutfileschwanz
        '    CLstart.myc.kartengen.mapcred.username = myglobalz.sitzung.aktBearbeiter.username
        '    CLstart.myc.kartengen.genOutfileFullName(cachedir, ".png")
        '    ' webmapStream(aufruf$)
        '    ' glob2.webmapBrows(CLstart.myc.kartengen.genaufruf)

        '    'l("aufruf: " & aufruf)
        '    'MapModeAbschicken(aufruf)
        '    If useMapserverMapmode Then
        '        gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
        '    Else
        '        ' Dim aufruf As String = CLstart.myc.kartengen.BildGenaufruf
        '        glob2.webmapBrows(CLstart.myc.kartengen.genaufruf)
        '        CreateNewFileSystemWatcherAndSetItsProperties()
        '    End If

        '    Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myglobalz.sitzung.aktVorgangsID)
        '    nachricht("presentMap: ---------------------- " & erfolg)
        'Catch ex As Exception
        '    nachricht_und_Mbox("fehler in presentMap: ---------------------- " ,ex)
        'End Try
    End Sub

    'Private Sub MapModeAbschicken(aufruf As String)
    '    Dim myBitmapImage As New BitmapImage()
    '    Try
    '        myBitmapImage.BeginInit()
    '        myBitmapImage.UriSource = New Uri(aufruf, UriKind.Absolute)
    '        myBitmapImage.EndInit()
    '        If chkBIGGIS.IsChecked Then
    '            canvasImage.Source = myBitmapImage
    '        Else
    '            canvasImage.Source = myBitmapImage
    '        End If
    '    Catch ex As Exception
    '        l("fehler in MapModeAbschicken: " & aufruf & " /// " ,ex)
    '    End Try
    'End Sub

    'Private Sub CreateNewFileSystemWatcherAndSetItsProperties()
    '    nachricht("CreateNewFileSystemWatcherAndSetItsProperties --------------------------")
    '    Try
    '        ' Create a new FileSystemWatcher and set its properties.
    '        Dim test As New IO.FileInfo(CLstart.myc.kartengen.gifKartenDateiFullName)
    '        nachricht("clstart.myc.kartengen.gifKartenDateiFullName :" & CLstart.myc.kartengen.gifKartenDateiFullName)
    '        gifKartenwatcher = New FileSystemWatcher
    '        gifKartenwatcher.Path = test.DirectoryName
    '        ' Watch for changes in LastAccess and LastWrite times, and   ' the renaming of files or directories.
    '        gifKartenwatcher.NotifyFilter = (NotifyFilters.LastAccess Or NotifyFilters.LastWrite Or NotifyFilters.FileName Or NotifyFilters.DirectoryName)
    '        gifKartenwatcher.Filter = test.Name
    '        test = Nothing
    '        mapHandlerSchonAktiv = False
    '        AddHandler gifKartenwatcher.Changed, AddressOf OnChangedFileSystemCacheGIS
    '        ' Begin watching.
    '        gifKartenwatcher.EnableRaisingEvents = True
    '        nachricht("CreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------")
    '    Catch ex As Exception
    '        nachricht("Fehler in : CreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------" ,ex)
    '    End Try
    'End Sub

    'Public Sub NotifyUIThreadOfChangeGIS(ByVal e As FileSystemEventArgs)
    '    nachricht("NotifyUIThreadOfChange --------------------------")
    '    gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    'End Sub

    'Private Sub gisDarstellenAlleEbenen(zeichneOverlay As Boolean, zeichneImageMap As Boolean, eigentuemerFunktionAktiv As Boolean)
    '    Try
    '        Dim myCanvas As Canvas
    '        If useExternGis Then
    '            'ihah hier alles aus
    '        End If
    '        myCanvas.Children.Clear()
    '        myCanvas2.Children.Clear()
    '        canvasImage = New Image
    '        canvasImage.Name = "canvasImage"
    '        Dim canvasLeer As New Image
    '        canvasLeer = getLeerImage(canvasLeer)
    '        If chkBIGGIS.IsChecked Then
    '            myCanvas2.Children.Add(canvasImage)
    '            DrawRectangle(myCanvas2)
    '            myCanvas.Children.Add(canvasLeer)
    '        Else
    '            myCanvas.Children.Add(canvasImage)
    '            myCanvas.SetZIndex(canvasImage, 1)
    '            DrawRectangle(myCanvas)
    '        End If
    '        If useMapserverMapmode Then
    '            'Dim mapfile As String = "/inetpub/wwwroot/mapservtest/mapfile/testbod.map"
    '            Dim mapfile As String = "/inetpub/wwwroot/mapservtest/mapfile/testalk.map"

    '            'Dim aufruf As String = CLstart.myc.kartengen.BildGenaufruf
    '            Dim BILDaufruf As String = CLstart.myc.kartengen.BildGenaufrufMAPserver(mapfile)
    '            Dim IMAPaufruf As String = CLstart.myc.kartengen.ImapGenaufrufMAPserver(mapfile)
    '            l("aufruf: " & BILDaufruf)
    '            MapModeAbschicken(BILDaufruf)
    '            Dim hinweis As String
    '            CLstart.myc.kartengen.imageMap = CLstart.meineHttpNet.meinHttpJob(myglobalz.ProxyString, IMAPaufruf, hinweis)
    '            nachricht(hinweis)
    '        Else
    '            refreshBitmap()
    '            Threading.Thread.Sleep(900)
    '        End If
    '        zeichneOverlay = True 'ihah
    '        If zeichneOverlay Then
    '            If chkBIGGIS.IsChecked Then
    '                clsMiniMapTools.refreshMapOverlay(myCanvas2, CLstart.myc.kartengen.aktMap.aktrange, ableitungskreis, eigentuemerFunktionAktiv)
    '            Else
    '                clsMiniMapTools.refreshMapOverlay(myCanvas, CLstart.myc.kartengen.aktMap.aktrange, ableitungskreis, eigentuemerFunktionAktiv)
    '            End If
    '        End If
    '        If zeichneImageMap Then
    '            Dim istIMAPVorhanden As Boolean
    '            Dim imagemapTemp As String
    '            If useMapserverMapmode Then
    '                istIMAPVorhanden = clsMiniMapTools.imageMapDateiVorhanden(CLstart.myc.kartengen.imageMap)
    '                imagemapTemp = CLstart.myc.kartengen.imageMap
    '            Else
    '                istIMAPVorhanden = clsMiniMapTools.imageMapDateiVorhanden(CLstart.myc.kartengen.imagemapDateifullname)
    '                imagemapTemp = CLstart.myc.kartengen.imagemapDateifullname
    '            End If

    '            'istIMAPVorhanden = False 'ihah

    '            If istIMAPVorhanden Then
    '                myglobalz.mapEigentuemerAktiv = True
    '                tbAktiveEbene.Text = "Aktiv: " & glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.ActiveLayer)
    '                tbAktiveEbene2.Text = "Aktiv: " & glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.ActiveLayer)
    '                If myglobalz.mapEigentuemerAktiv Then
    '                    ' clstart.myc.kartengen.imagemapDateifullname=clsMiniMapTools.imageMap2Eigentuemermap(clstart.myc.kartengen.imagemapDateifullname)
    '                    If chkBIGGIS.IsChecked Then
    '                        If useMapserverMapmode Then
    '                            clsMiniMapTools.imageMap2PolygonMapMAPSERVER(
    '                                imagemapTemp, myCanvas2, CBool(ckbeigentuemerFunktion.IsChecked))
    '                        Else
    '                            clsMiniMapTools.imageMap2PolygonMap(
    '                                imagemapTemp, myCanvas2, CBool(ckbeigentuemerFunktion.IsChecked), useMapserverMapmode)
    '                        End If

    '                    Else
    '                        If useMapserverMapmode Then
    '                            clsMiniMapTools.imageMap2PolygonMapMAPSERVER(imagemapTemp, myCanvas, CBool(ckbeigentuemerFunktion.IsChecked))
    '                        Else
    '                            clsMiniMapTools.imageMap2PolygonMap(imagemapTemp, myCanvas, CBool(ckbeigentuemerFunktion.IsChecked), useMapserverMapmode)
    '                        End If

    '                    End If
    '                Else
    '                    If chkBIGGIS.IsChecked Then
    '                        If useMapserverMapmode Then
    '                            clsMiniMapTools.imageMap2PolygonMapMAPSERVER(imagemapTemp, myCanvas2, CBool(ckbeigentuemerFunktion.IsChecked))
    '                        Else
    '                            clsMiniMapTools.imageMap2PolygonMap(imagemapTemp, myCanvas2, CBool(ckbeigentuemerFunktion.IsChecked), useMapserverMapmode)
    '                        End If

    '                    Else
    '                        If useMapserverMapmode Then
    '                            clsMiniMapTools.imageMap2PolygonMapMAPSERVER(imagemapTemp, myCanvas, CBool(ckbeigentuemerFunktion.IsChecked))
    '                        Else
    '                            clsMiniMapTools.imageMap2PolygonMap(imagemapTemp, myCanvas, CBool(ckbeigentuemerFunktion.IsChecked), useMapserverMapmode)
    '                        End If

    '                    End If
    '                End If
    '            End If
    '        End If
    '        tbAktiveEbene.Text = "Aktiv: " & glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.ActiveLayer)
    '        tbAktiveEbene2.Text = "Aktiv: " & glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.ActiveLayer)
    '        If Not useMapserverMapmode Then
    '            gifKartenwatcher.Dispose()
    '        End If
    '        nachricht("gisDarstellenAlleEbenen --------------------------")
    '    Catch ex As Exception
    '        nachricht("Fehler in : gisDarstellenAlleEbenen ---------ende-----------------" ,ex)
    '    End Try
    'End Sub

    'Private Shared Function getLeerImage(canvasLeer As Image) As Image
    '    Dim myBitmapImage As New BitmapImage()
    '    Try
    '        myBitmapImage.BeginInit()
    '        myBitmapImage.UriSource = New Uri("O:\UMWELT -PARADIGMA\div\showup\showupeulen2.jpg")
    '        '		myBitmapImage.DecodePixelWidth = 200
    '        myBitmapImage.EndInit()
    '        canvasLeer.Source = myBitmapImage
    '        Return canvasLeer
    '    Catch ex As Exception
    '        nachricht("Fehler in : getLeerImage ---------ende-----------------" ,ex)
    '    End Try
    'End Function

    'Private Sub DrawRectangle(ByVal mycanvas As Canvas)
    '    rubberbox = New Rectangle
    '    rubberbox.Name = "rubberbox"
    '    Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 0, 250)) 'transparenz ist der erste wert
    '    rubberbox.Stroke = Brushes.Black
    '    rubberbox.StrokeThickness = 2
    '    rubberbox.Opacity = 90
    '    rubberbox.Fill = myBrush
    '    Panel.SetZIndex(rubberbox, 100)
    '    Canvas.SetZIndex(rubberbox, 100)
    '    mycanvas.Children.Add(rubberbox)
    '    Panel.SetZIndex(rubberbox, 100)
    '    Canvas.SetZIndex(rubberbox, 100)
    'End Sub
    'Sub drawprogressbar()
    '    nachricht("drawprogressbar: ---------------------- ")
    '    Try
    '        Dim myprogressbar As ProgressBar = New ProgressBar
    '        myprogressbar.Name = "ProgressBar1"

    '        myprogressbar.IsIndeterminate = True
    '        If chkBIGGIS.IsChecked Then
    '            myprogressbar.Width = 300
    '            myprogressbar.Height = 25
    '            myCanvas2.Children.Add(myprogressbar)
    '        Else
    '            myprogressbar.Width = 100
    '            myprogressbar.Height = 10
    '            myCanvas.Children.Add(myprogressbar)
    '        End If
    '        Canvas.SetZIndex(myprogressbar, 1000)
    '        If chkBIGGIS.IsChecked Then
    '            Canvas.SetLeft(myprogressbar, 695)
    '            Canvas.SetTop(myprogressbar, 400)
    '        Else
    '            Canvas.SetLeft(myprogressbar, 95)
    '            Canvas.SetTop(myprogressbar, 1)
    '        End If

    '    Catch ex As Exception
    '        nachricht("fehler in drawprogressbar " ,ex)
    '    End Try
    '    nachricht("drawprogressbar: ----------ende------------ ")
    'End Sub

    'Private Sub OnChangedFileSystemCacheGIS(ByVal source As Object, ByVal e As FileSystemEventArgs)
    '    Try
    '        If mapHandlerSchonAktiv Then
    '            nachricht("mapHandlerSchonAktiv deshalb keine aktion")
    '        Else
    '            mapHandlerSchonAktiv = True
    '            nachricht("mapHandlerSchonAktiv false deshalb  aktion")
    '            'officeDocWatcher.EnableRaisingEvents = False
    '            'officeDocWatcher.Dispose()
    '            'Call back to the UI thread

    '            nachricht("OnChangedFileSystemCacheGIS --------------------------vor invoke")
    '            Me.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
    '                                  New watcherCallBackToUIThread(AddressOf NotifyUIThreadOfChangeGIS), e)
    '            'gifKartenwatcher.Dispose()
    '        End If
    '        nachricht("OnChangedFileSystemCacheGIS -------------ende-------------")
    '    Catch ex As Exception
    '        nachricht("fehler in OnChangedFileSystemCacheGIS " ,ex)
    '    End Try
    'End Sub


    Public Sub setNomapBitmap()


    End Sub

    'Public Sub refreshBitmap()
    '    Try
    '        Dim dauer As Integer = 900
    '        myCanvas.Background = Nothing
    '        System.Threading.Thread.Sleep(dauer)
    '        Dim test As New IO.FileInfo(CLstart.myc.kartengen.gifKartenDateiFullName)
    '        If Not test.Exists Then
    '            nachricht("Die Grafik kann nicht gefunden werden!" & CLstart.myc.kartengen.gifKartenDateiFullName)
    '            test = Nothing
    '            System.Threading.Thread.Sleep(dauer)
    '            If Not test.Exists Then
    '                nachricht("Die Grafik kann nicht gefunden werden!" & CLstart.myc.kartengen.gifKartenDateiFullName)
    '                test = Nothing
    '                Exit Sub
    '            End If
    '        Else
    '            nachricht("Die Grafik wurde gefunden !" & CLstart.myc.kartengen.gifKartenDateiFullName)
    '            test = Nothing
    '        End If
    '        Dim myBitmapImage As New BitmapImage()
    '        myBitmapImage.BeginInit()
    '        myBitmapImage.UriSource = New Uri(CLstart.myc.kartengen.gifKartenDateiFullName)
    '        myBitmapImage.EndInit()
    '        If chkBIGGIS.IsChecked Then
    '            canvasImage.Source = myBitmapImage
    '        Else
    '            canvasImage.Source = myBitmapImage
    '        End If
    '        Canvas.SetZIndex(canvasImage, 1)
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Minimap-Datei war noch nicht freigegeben. Bitte nochmal versuchen!")
    '        nachricht("fehler in refreshbitmap: " ,ex)
    '    End Try
    'End Sub








    'Private Sub myCanvas_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles myCanvas.MouseLeftButtonDown, myCanvas2.MouseLeftButtonDown

    '    Select Case CanvasClickModus.ToLower
    '        Case "ausschnitt"
    '            If chkBIGGIS.IsChecked Then
    '                If btnAusschnitt2.IsChecked Then RubberbandStart(e)
    '            Else
    '                If btnAusschnitt.IsChecked Then RubberbandStart(e)
    '            End If
    '        Case "pan"
    '            isDraggingFlag = True
    '            If chkBIGGIS.IsChecked Then
    '                origContentMousePoint = e.GetPosition(myCanvas2)
    '            Else
    '                origContentMousePoint = e.GetPosition(myCanvas)
    '            End If
    '    End Select
    'End Sub

    'Private Sub canvas1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles myCanvas.MouseMove, myCanvas2.MouseMove
    '    Select Case CanvasClickModus.ToLower
    '        Case "ausschnitt"
    '            If chkBIGGIS.IsChecked Then
    '                If btnAusschnitt2.IsChecked Then RubberbandMove(e)
    '            Else
    '                If btnAusschnitt.IsChecked Then RubberbandMove(e)
    '            End If
    '        Case "pan"
    '            If isDraggingFlag Then
    '                Dim dragOffset As Vector
    '                If chkBIGGIS.IsChecked Then
    '                    curContentMousePoint = e.GetPosition(myCanvas2)
    '                    'UTMCoordinate.Text = myCanvas2.Width & ":" & myCanvas2.Height
    '                    dragOffset = curContentMousePoint - origContentMousePoint
    '                    Canvas.SetTop(canvasImage, dragOffset.Y)
    '                    Canvas.SetLeft(canvasImage, dragOffset.X)
    '                Else
    '                    curContentMousePoint = e.GetPosition(myCanvas)
    '                    'UTMCoordinate.Text = myCanvas.Width & ":" & myCanvas.Height
    '                    dragOffset = curContentMousePoint - origContentMousePoint
    '                    Canvas.SetTop(canvasImage, dragOffset.Y)
    '                    Canvas.SetLeft(canvasImage, dragOffset.X)
    '                End If


    '                ' tbMinimapFlaeche.Text = curContentMousePoint.X & ":" & curContentMousePoint.Y
    '            End If
    '    End Select
    'End Sub

    'Private Sub canvas1_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles myCanvas.MouseLeftButtonUp, myCanvas2.MouseLeftButtonUp
    '    Select Case CanvasClickModus.ToLower
    '        Case "ausschnitt"
    '            If chkBIGGIS.IsChecked Then
    '                If btnAusschnitt2.IsChecked Then RubberbandFinish()
    '            Else
    '                If btnAusschnitt.IsChecked Then RubberbandFinish()
    '            End If
    '            zeichneOverlaysGlob = True
    '            zeichneImageMapGlob = True
    '            CanvasClickModus = ""
    '        Case "koordinate"
    '            Me.Cursor = Nothing
    '            Mouse.Capture(Nothing)
    '            If chkBIGGIS.IsChecked Then
    '                clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Visible)
    '                KoordinateKLickpt = e.GetPosition(myCanvas2)
    '            Else
    '                clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    '                KoordinateKLickpt = e.GetPosition(myCanvas)
    '            End If
    '            CanvasClickModus = ""
    '            UTMCoordinate.Text = koordinateKlickBerechnen(KoordinateKLickpt) & " [m]"
    '            tbMinimapCoordinate2.Text = koordinateKlickBerechnen(KoordinateKLickpt) & " [m]"
    '            If KoordinateAlsNeuenRaumbezugAnbieten() Then
    '                myglobalz.sitzung.raumbezugsmodus = "neu"
    '                Dim winflur As New Win_punktMitUmkreis(UTMCoordinate.Text)
    '                winflur.ShowDialog()
    '                If CBool(winflur.DialogResult) Then
    '                    refreshRaumbezugsListe(myglobalz.sitzung.aktVorgangsID, False, True)
    '                End If
    '            End If
    '            zeichneOverlaysGlob = True
    '            zeichneImageMapGlob = True
    '        Case "zwert"
    '            Me.Cursor = Nothing
    '            Mouse.Capture(Nothing)
    '            If chkBIGGIS.IsChecked Then
    '                clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Visible)
    '                KoordinateKLickpt = e.GetPosition(myCanvas2)
    '            Else
    '                clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    '                KoordinateKLickpt = e.GetPosition(myCanvas)
    '            End If

    '            ' MsgBox("u p " & KoordinateKLickpt.ToString)
    '            CanvasClickModus = ""
    '            MsgBox("Die Wartezeit kann bis zu 5 Min. dauern. Solange ist Paradigma blockiert!")
    '            LaserScan.mod3dtools.koordinateKlickBerechnenZWERT(KoordinateKLickpt)
    '            zeichneOverlaysGlob = True
    '            zeichneImageMapGlob = True
    '        'Case "Ableitungskreis"
    '        '    Me.Cursor = Nothing
    '        '    Mouse.Capture(Nothing)
    '        '    If chkBIGGIS.IsChecked Then
    '        '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Visible)
    '        '        KoordinateKLickpt = e.GetPosition(myCanvas2)
    '        '    Else
    '        '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    '        '        KoordinateKLickpt = e.GetPosition(myCanvas)
    '        '    End If

    '        '    CanvasClickModus = ""
    '        '    '   koordinateKlickBerechnen(KoordinateKLickpt)
    '        '    ableitungskreis.punktUTM = getkoordinateKlickBerechnen(KoordinateKLickpt)
    '        '    ableitungskreis.punktScreen = CType(KoordinateKLickpt, Point)
    '        '    If chkBIGGIS.IsChecked Then
    '        '        clsMiniMapTools.refreshEllipsen(myCanvas2, CLstart.myc.kartengen.aktMap.aktrange, ableitungskreis)
    '        '    Else
    '        '        clsMiniMapTools.refreshEllipsen(myCanvas, CLstart.myc.kartengen.aktMap.aktrange, ableitungskreis)
    '        '    End If 
    '        Case "flaeche"
    '            Dim tempPT As Point? = Nothing
    '            Dim winpt As New Point
    '            If chkBIGGIS.IsChecked Then
    '                tempPT = e.GetPosition(myCanvas2)
    '            Else
    '                tempPT = e.GetPosition(myCanvas)
    '            End If

    '            winpt.X = tempPT.Value.X
    '            winpt.Y = tempPT.Value.Y
    '            myglobalz.sitzung.aktPolygon.myPoly.Points.Add(winpt)
    '            myPolyVertexCount% += 1
    '            zeichneOverlaysGlob = True
    '            zeichneImageMapGlob = True
    '        Case "strecke"
    '            Dim tempPT As Point? = Nothing
    '            Dim winpt, utmpt As New Point
    '            Dim delim As String = ";"
    '            If chkBIGGIS.IsChecked Then
    '                tempPT = e.GetPosition(myCanvas2)
    '            Else
    '                tempPT = e.GetPosition(myCanvas)
    '            End If
    '            winpt.X = tempPT.Value.X
    '            winpt.Y = tempPT.Value.Y

    '            myglobalz.sitzung.aktPolyline.myLine.Points.Add(winpt)

    '            utmpt.X = CInt((clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(winpt).X) * 100) / 100
    '            utmpt.Y = CInt((clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(winpt).Y) * 100) / 100

    '            myglobalz.sitzung.aktPolyline.Distanz = myglobalz.sitzung.aktPolyline.Distanz +
    '                          PolygonTools.calcDistanz(utmpt, myglobalz.sitzung.aktPolyline.alterPunkt)
    '            myglobalz.sitzung.aktPolyline.GKstring = myglobalz.sitzung.aktPolyline.GKstring &
    '                         CDbl(utmpt.X) & delim & CDbl(utmpt.Y) & delim




    '            myglobalz.sitzung.aktPolyline.alterPunkt.X = utmpt.X
    '            myglobalz.sitzung.aktPolyline.alterPunkt.Y = utmpt.Y
    '            tbMinimapLinie.Text = myglobalz.sitzung.aktPolyline.Distanz.ToString("########.##")
    '            tbMinimapCoordinate2.Text = myglobalz.sitzung.aktPolyline.Distanz.ToString("########.##") & " [m]"
    '            myPolyVertexCount += 1
    '            zeichneOverlaysGlob = True
    '            zeichneImageMapGlob = True

    '        'Case "newcenter"
    '        '    Me.Cursor = Cursors.Cross
    '        '    Mouse.Capture(Nothing)
    '        '    If chkBIGGIS.IsChecked Then
    '        '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Visible)
    '        '        KoordinateKLickpt = e.GetPosition(myCanvas2)
    '        '    Else
    '        '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    '        '        KoordinateKLickpt = e.GetPosition(myCanvas)
    '        '    End If
    '        '    ' CanvasClickModus = ""
    '        '    tbMinimapCoordinate.Text = koordinateKlickBerechnen(KoordinateKLickpt)
    '        '    Dim a As String()
    '        '    a = tbMinimapCoordinate.Text.Split(","c)
    '        '    Dim newpoint As New myPoint
    '        '    newpoint.X = CDbl((a(0)))
    '        '    newpoint.Y = CDbl((a(1)))

    '        '    Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
    '        '    Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()

    '        '    CLstart.myc.kartengen.aktMap.aktrange.xl = newpoint.X - (breite / 2)
    '        '    CLstart.myc.kartengen.aktMap.aktrange.xh = newpoint.X + (breite / 2)

    '        '    CLstart.myc.kartengen.aktMap.aktrange.yl = newpoint.Y - (hohe / 2)
    '        '    CLstart.myc.kartengen.aktMap.aktrange.yh = newpoint.Y + (hohe / 2)
    '        '    presentMap()
    '        Case "pan"
    '            CanvasClickModus = "pan" ' bleibt!!
    '            isDraggingFlag = False

    '            Dim dragOffset As Vector = curContentMousePoint - origContentMousePoint
    '            Dim neuerBildschirmMittelPunktInPoints As Point
    '            If chkBIGGIS.IsChecked Then
    '                neuerBildschirmMittelPunktInPoints = New Point() With {.X = (myCanvas2.Width / 2) - dragOffset.X,
    '                                                                   .Y = (myCanvas2.Height / 2) - dragOffset.Y}
    '            Else
    '                neuerBildschirmMittelPunktInPoints = New Point() With {.X = (myCanvas.Width / 2) - dragOffset.X,
    '                                                                   .Y = (myCanvas.Height / 2) - dragOffset.Y}
    '            End If
    '            dragOffset = Nothing

    '            UTMCoordinate.Text = koordinateKlickBerechnen(neuerBildschirmMittelPunktInPoints)
    '            Dim a As String()
    '            a = UTMCoordinate.Text.Split(","c)
    '            Dim neuerMittelPunktInUTM As New myPoint
    '            neuerMittelPunktInUTM.X = CDbl((a(0)))
    '            neuerMittelPunktInUTM.Y = CDbl((a(1)))

    '            Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
    '            Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()

    '            CLstart.myc.kartengen.aktMap.aktrange.xl = neuerMittelPunktInUTM.X - (breite / 2)
    '            CLstart.myc.kartengen.aktMap.aktrange.xh = neuerMittelPunktInUTM.X + (breite / 2)

    '            CLstart.myc.kartengen.aktMap.aktrange.yl = neuerMittelPunktInUTM.Y - (hohe / 2)
    '            CLstart.myc.kartengen.aktMap.aktrange.yh = neuerMittelPunktInUTM.Y + (hohe / 2)
    '            presentMapOLD()
    '            Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myglobalz.sitzung.aktVorgangsID)
    '            zeichneOverlaysGlob = True
    '            zeichneImageMapGlob = False
    '            neuerMittelPunktInUTM = Nothing
    '    End Select
    'End Sub

    Function FlaecheAlsNeuenRaumbezugAnbieten(Text As String) As Boolean
        Dim res As New MessageBoxResult
        res = MessageBox.Show(
                "Möchten Sie die " & Text & " als neuen Raumbezug übernehmen ? " & vbCrLf &
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

    Function koordinateKlickBerechnen(ByVal KoordinateKLickpt As Point?) As String
        Dim newpoint2 As New myPoint
        Dim newpoint As New myPoint
        newpoint2.X = CDbl(KoordinateKLickpt.Value.X)
        newpoint2.Y = CDbl(KoordinateKLickpt.Value.Y)
        newpoint = clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(newpoint2, CLstart.myc.kartengen.aktMap.aktrange,
                                                                                      CLstart.myc.kartengen.aktMap.aktcanvas)
        newpoint.SetToInteger()
        Return newpoint.toString
        newpoint2 = Nothing
        newpoint = Nothing
    End Function



    Function getkoordinateKlickBerechnen(ByVal KoordinateKLickpt As Point?) As myPoint
        Dim newpoint2 As New myPoint
        Dim newpoint As New myPoint
        newpoint2.X = CDbl(KoordinateKLickpt.Value.X)
        newpoint2.Y = CDbl(KoordinateKLickpt.Value.Y)
        newpoint = clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(newpoint2, CLstart.myc.kartengen.aktMap.aktrange, CLstart.myc.kartengen.aktMap.aktcanvas)
        '  newpoint.SetToInteger()
        Return newpoint
        'tbMinimapCoordinate.Text = newpoint.toString
        'newpoint2 = Nothing
        'newpoint = Nothing
    End Function

    'Private Sub Window1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyDown
    '    If e.Key = Key.Escape Then canvas1_MouseLeftButtonUp(sender, Nothing)
    'End Sub

    'Private Sub RubberbandFinish()
    '    If chkBIGGIS.IsChecked Then
    '        btnAusschnitt2.IsChecked = False
    '    Else
    '        btnAusschnitt.IsChecked = False
    '    End If
    '    clsMiniMapTools.ausschnittNeuBerechnen(RubberbandStartpt, RubberbandEndpt)
    '    setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
    '    rubberbox.Width = 0
    '    rubberbox.Height = 0
    '    Mouse.Capture(Nothing)
    '    RubberbandStartpt = Nothing
    '    RubberbandEndpt = Nothing
    '    Me.Cursor = Nothing
    '    If chkBIGGIS.IsChecked Then
    '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Visible)
    '    Else
    '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    '    End If
    'End Sub



    Private Sub geaenderteStammdatenSpeichern()
        Try
            l("geaenderteStammdatenSpeichern---------------------- anfang")
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
            l("geaenderteStammdatenSpeichern---------------------- ende")
        Catch ex As Exception
            l("Fehler in geaenderteStammdatenSpeichern: ", ex)
        End Try
    End Sub

    Private Sub Window_Detail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim dokusInArbeit As Boolean = False
        Dim mesres As New MessageBoxResult
        Dim dokInArbeit As String = ""
        l("USERAKTION: CLOSING")
        'schliesse3Dbrowser
        dokusInArbeit = detailsTools.sindDokumenteImZugriff()
        l("dokusInArbeit " & dokusInArbeit)
        dokusInArbeit = False
        nachricht("sindDokumenteImZugriff " & dokusInArbeit)
        If dokusInArbeit Then
            mesres = detailsTools.schliessenAbbrechen1(mesres, dokInArbeit)
            If Not myGlobalz.sitzung.wordDateiImEditModus.FullnameCheckout.IsNothingOrEmpty Then
                Dim quell As New IO.FileInfo(myGlobalz.sitzung.wordDateiImEditModus.FullnameCheckout)
                Dim backupdatei As String = detailsTools.BackupAnlegen(quell, myGlobalz.sitzung.wordDateiImEditModus.DocID)
            End If
            If Not myGlobalz.sitzung.excelDateiImEditModus.FullnameCheckout.IsNothingOrEmpty Then
                Dim quell As New IO.FileInfo(myGlobalz.sitzung.excelDateiImEditModus.FullnameCheckout)
                Dim backupdatei As String = detailsTools.BackupAnlegen(quell, myGlobalz.sitzung.excelDateiImEditModus.DocID)
            End If
            If mesres = MessageBoxResult.Yes Then
                l("fehler hier wurde was verworfen: " & dokInArbeit & ", " & myGlobalz.sitzung.aktVorgangsID)
                e.Cancel = False
            Else
                e.Cancel = True
                Exit Sub
            End If
        Else

        End If
        geaenderteStammdatenSpeichern()
        setMitDokumenten()
        detailsTools.VorgangLocking("aus")
        clsVorgangLocking.LockingLoesen(initP.getValue("Haupt.LOCKINGFile"),
                                    myGlobalz.sitzung.aktVorgangsID,
                                    myGlobalz.sitzung.aktBearbeiter.Initiale)

        'If myglobalz.didEverOpenAWordDocInSession Then
        '    l("didEverOpenAWordDocInSession=" & myglobalz.didEverOpenAWordDocInSession)
        '    If glob2.isWordRunning Then
        '        mesres = schliessenAbbrechen(mesres)
        '        If mesres = MessageBoxResult.Yes Then
        '            e.Cancel = False
        '        Else
        '            e.Cancel = True
        '            Exit Sub
        '        End If
        '    End If
        'End If
        myGlobalz.einVorgangistgeoeffnet = False
        nachricht("CLOSING Dokument wurde geändert?:   " & dokusInArbeit.ToString)
        detailsTools.VorgangLocking("aus")
        CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(myGlobalz.sitzung.aktVorgangsID.ToString,
                                            myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung,
                                            myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt,
                                            myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz,
                                            myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)
        'If Not useMapserverMapmode Then
        '    If gifKartenwatcher IsNot Nothing Then
        '        gifKartenwatcher.Dispose()
        '    End If
        'End If

        detailsTools.hatraumbezugDarstellen()
        mgistools.mgisSchliessen()
        savePosition()

        Try
            CLstart.MeinLogging.LoggingEnde(logfile, myGlobalz.LOGFILEKOPIE)
            CLstart.myc.aLog.endlog()
        Catch ex As Exception
            'tritt auf, wenn vorgangsnummer gar nicht existiert
        End Try
        clsVorgangLocking.LockingLoesen(initP.getValue("Haupt.LOCKINGFile"),
                             myGlobalz.sitzung.aktVorgangsID,
                             myGlobalz.sitzung.aktBearbeiter.Initiale)
        CLstart.VIDuebergabe.erzeugeParameterDatei(CInt(myGlobalz.sitzung.aktVorgangsID), myGlobalz.sitzung.aktBearbeiter.username, CLstart.mycSimple.Paradigma_local_root, "vorherigerVorgang")
    End Sub
    Private Sub savePosition()
        If myGlobalz.nureinbildschirm Then Exit Sub
        Try
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winfotoguggerformpositiontop", CType(Me.Top, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winfotoguggerformpositionleft", CType(Me.Left, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winfotoguggerpositionwidth", CType(Me.ActualWidth, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winfotoguggerpositionheight", CType(Me.ActualHeight, String))
        Catch ex As Exception
            l("fehler in saveposition  windb", ex)
        End Try
    End Sub
    'Private Sub schliesse3Dbrowser()
    '    Try
    '        webView.Browser.Dispose()
    '        webView.Dispose()
    '    Catch ex As Exception

    '    End Try
    'End Sub

    'Private Shared Function schliessenAbbrechen(mesres As MessageBoxResult) As MessageBoxResult
    '    l("isWordRunning=true")
    '    mesres = MessageBox.Show("Vorsicht: " & Environment.NewLine & Environment.NewLine &
    '                                 "Word läuft noch !!! " & Environment.NewLine &
    '                                 " >>> Haben Sie wirklich alles abgespeichert und sichergestellt, daß nichts verloren geht ?" & Environment.NewLine &
    '                                 " Sie sollten VOR dem Schließen des Vorgangs alle Dokumente dieses Vorgangs geschlossen haben." & Environment.NewLine &
    '                                 " " & Environment.NewLine &
    '                                 " Möchten Sie den Vorgang trotzdem verlassen ?" & Environment.NewLine &
    '                                 "   Ja   - Vorgang verlassen" & Environment.NewLine &
    '                                 "   Nein - Im Vorgang bleiben um Worddokument zu schliessen.",
    '                        "Word läuft noch !!!", MessageBoxButton.YesNo, MessageBoxImage.Error)
    '    nachricht("warnung, KEIN Fehler. user schließt vorgang obwohl word noch geöffnet ist. " &
    '              myGlobalz.sitzung.aktVorgangsID &
    '              myGlobalz.sitzung.aktBearbeiter.Initiale &
    '              MessageBoxResult.Yes
    '              )
    '    Return mesres
    'End Function

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
            Dim sql As String
            ' sql = "select * from  " & CLstart.myViewsNTabs.tabVORGANG2FREMDVORGANG & "  where fremdvorgangsid=" & vid
            sql = "SELECT VF.ID,VF.VORGANGSID,VF.FREMDVORGANGSID,S.AZ2 AS TITEL FROM  " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG & "   VF, " & CLstart.myViewsNTabs.TABSTAMMDATEN & " S " &
                    " where s.vorgangsid=vf.vorgangsid" &
                    " and vf.fremdvorgangsid=" & vid
            Dim dientVorgangAlsServer As Boolean = detailsTools.fuelleVerwandteDT(sql)
            dgVerwandteServer.DataContext = myGlobalz.sitzung.VerwandteDTServer
            '   initCombosVerwandte()
            If dientVorgangAlsServer Then
                'tabheaderVerwandte.Header = "Verwandte " & myGlobalz.sitzung.VerwandteDT.Rows.Count
                ' VerwandteGroupboxenEnabled(True)
                'tabheaderVerwandte.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
                tabheaderVerwandte.SetValue(TextElement.ToolTipProperty, "Der aktuelle Vorgang wird von den aufgelisteten Vorgängen als <Verwandter Vorgang> verwendet")
                '	tabheaderVorlagen.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
                Dim VerwServerVorschau As String
                VerwServerVorschau = VerwandteTools.divers.getVerwServerVorschau(myGlobalz.sitzung.VerwandteDTServer)
                tabheaderVerwandte.Header = "Verwandte " & myGlobalz.sitzung.VerwandteDTServer.Rows.Count
                tiVerwandteServer.Header = "Datenquelle von: " & VerwServerVorschau
            Else
                tabheaderVerwandte.Header = "Verwandte "
                VerwandteGroupboxenEnabled(False)
                tabheaderVerwandte.SetValue(TextElement.FontWeightProperty, FontWeights.Normal)
                tabheaderVerwandte.SetValue(TextElement.ToolTipProperty, "")
            End If
        Catch ex As Exception
            nachricht("fehler in refreshverwandteServer: ", ex)
        End Try
    End Sub



    Private Sub kurzdossier_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: dossier ")
        presDokusAusgewaehltMarkieren(True, Psession.presFotos)
        presDokusAusgewaehltMarkieren(True, Psession.presDokus)
        Dim mycanvas As New Canvas
        mycanvas.Width = 300
        mycanvas.Height = 250
        clsBerichte.fotobucherstellen(mycanvas, False,
                                  detail_dokuauswahl.dokListenMergen(Psession.presDokus, Psession.presFotos),
                                  CLstart.myc.kartengen.aktMap.aktrange, CBool(chkDossierohne.IsChecked), CBool(chkDossierohne.IsChecked))
        e.Handled = True
    End Sub





    'Private Shared Function aktBOXwechseln(ByVal themex As WinThemen) As Boolean
    '    Return Not themex.aktBOX.IsNothingOrEmpty AndAlso themex.aktBOXwechseln = True
    'End Function
    'Private Sub starteThemenauswahl()
    '    Dim radius As Double = 500
    '    Dim karteErneuern As Boolean
    '    Dim themex As New WinThemen("maplayer_referenced")
    '    karteErneuern = CBool(themex.ShowDialog())
    '    If karteErneuern Then
    '        If aktBOXwechseln(themex) Then
    '            CLstart.myc.kartengen.aktMap.aktrange.postgisBOX2range(themex.aktBOX)
    '            fallsPUnkteDannRadiusAddieren(CLstart.myc.kartengen.aktMap.aktrange, radius)
    '        End If
    '        If themex.alsRaumbezugspeichern Then
    '            refreshRaumbezugsListe(myglobalz.sitzung.aktVorgangsID, False, True)
    '        End If
    '        CLstart.myc.kartengen.aktMap.Vgrund = themex.VGRUNDtemp
    '        CLstart.myc.kartengen.aktMap.Hgrund = themex.HGRUNDtemp
    '        presentMapOLD()
    '        refreshRaumbezugsListe(myglobalz.sitzung.aktVorgangsID, False, True)
    '    End If
    'End Sub


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
        e.Handled = True
        Dim sollConject As Boolean = False
        Dim filenames As String()
        Dim listeZippedFiles, listeNOnZipFiles, allFeiles As New List(Of String)
        Dim titelVorschlag As String = ""
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            filenames = CType(e.Data.GetData(DataFormats.FileDrop), String())
            If clsDropTools.hasZipfiles(filenames) Then
                If clsDropTools.hasConjectfiles(filenames) Then
                    filenames = clsDropTools.getAllFilenames(filenames, listeZippedFiles, listeNOnZipFiles, allFeiles)
                    If filenames.Count < 1 Then
                        MessageBox.Show("Es befinden sich keine Dateien in diesem ZIP-File, bzw. in der obersten Ebene!")
                    End If
                Else
                    Dim mesresult As MessageBoxResult
                    mesresult = MessageBox.Show("Sollen die ZIP-Archive ausgepackt werden bevor sie ins Archiv übernommen werden ?",
                                                "ZIP-Archive auspacken ?",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Question)
                    If mesresult = MessageBoxResult.Yes Then
                        filenames = clsDropTools.getAllFilenames(filenames, listeZippedFiles, listeNOnZipFiles, allFeiles)
                        If filenames.Count < 1 Then
                            MessageBox.Show("Es befinden sich keine Dateien in diesem ZIP-File, bzw. in der obersten Ebene!")
                        End If
                    End If
                End If
            Else

            End If

            'If clsDropTools.hasZipfiles(filenames) And clsDropTools.hasConjectfiles(filenames) Then
            '    filenames = clsDropTools.getAllFilenames(filenames, listeZippedFiles, listeNOnZipFiles, allFeiles)
            'Else
            '    'weiter
            'End If
            If filenames Is Nothing Then
                MsgBox("fehler bei zippen")
                Exit Sub
            End If
            If filenames.Count < 1 Then
                MessageBox.Show("Keine Dateien ausgewählt!")
                Exit Sub
            End If
            If filenames(0).ToLower.EndsWith(".eml") Then
                'thunderbird mails
                Dim problemMitanhang As Boolean = False
                detailsTools.EMLemnailUebernehmen(filenames(0).ToLower, myGlobalz.sitzung.aktVorgangsID, problemMitanhang)
                If problemMitanhang Then
                    MessageBox.Show("Es gab ein Problem mit dem Anhang. Bitte prüfen ob der Anhang vollständig rüberkam!", "Problem")
                End If
                refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
            Else
                If glob2.DokumentehinzuDragDrop(0, filenames, sollConject) Then
                    If sollConject Then
                        cbIstConject.IsChecked = True
                        myGlobalz.sitzung.aktVorgang.istConjectVorgang = clsStammTools.getIstConjectVorgang(myGlobalz.sitzung.aktVorgangsID)
                        If myGlobalz.sitzung.aktVorgang.istConjectVorgang Then
                        Else
                            clsStammTools.saveIstConject(myGlobalz.sitzung.aktVorgangsID, 1)
                            myGlobalz.sitzung.aktVorgang.istConjectVorgang = True
                        End If
                        zeigeIstConjectFarbe()
                    End If

                End If
                refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
                refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
            End If
        ElseIf e.Data.GetDataPresent("FileGroupDescriptor") Then
            detailsTools.outlookemnailUebernehmen(myGlobalz.sitzung.aktVorgangsID)
            refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
        End If
    End Sub
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
        Dim alterErledigtWert As Boolean = myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt
        Dim neustamm As New Win_Stamm("edit", myGlobalz.sitzung.aktVorgang.Stammdaten, "", "", "")
        neustamm.ShowDialog()
        StammDatenAnzeigeAktualisieren()
        setzeErledigtflagfarbe()
        SetzeStammdatenExplizitInsUI()
        If myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt And
       alterErledigtWert = False Then
            If glob2.UserFragenObNach_PDFA_Kopieren() Then
                alleWordDokusNachPdfaKopieren()
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            End If
        End If
    End Sub

    Private Sub StammDatenAnzeigeAktualisieren()
        tbAltAz.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.AltAz
        tbProbaugAZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz
        tbBeschreibung2.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung
        tbWeitereBearbeiter.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter
        tbGEMKRZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ
    End Sub

    Private Sub setzeErledigtflagfarbe()
        'Dim ffff As New Color
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
        detailsTools.eEreignisstarten("wiedervorlage", CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnOutlookemailuebernehmen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: outlookemail hinzufügen ")
        detailsTools.eEreignisstarten("outlookemail übernehmen", CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnStandardereignis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: stdereignis hinzufügen ")
        detailsTools.eEreignisstarten("Notiz", CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub


    Private Sub btnNachVorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: vorlage hinzufügen ")
        If myGlobalz.zuhause Or myGlobalz.PumuckelVersion = 2 Then
            '= 2 = kein pumuckel
            Dim wurzelverzeichnis As String = ""
            Dim zielverzeichnis As String = ""
            If tbVorlagenRoot.Text.IsNothingOrEmpty Then
                MessageBox.Show("Sie müssen zuerst unter Optionen das Wurzelverzeichnis für die Vorlagen eintragen !! (Abbruch)")
                Exit Sub
            Else
                wurzelverzeichnis = tbVorlagenRoot.Text
            End If
            If tbVorlagenWork.Text.IsNothingOrEmpty Then
                zielverzeichnis = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) & "\Paradigma"
            Else
                zielverzeichnis = tbVorlagenWork.Text
            End If

            IO.Directory.CreateDirectory(zielverzeichnis)
            Dim vorl As New winStatisch(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl,
                                         myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header,
                                         ereignisid:=0, wurzelverzeichnis, zielverzeichnis)
            vorl.ShowDialog()
        Else
            Dim vorl As New WinVorlagenListe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl,
                                         myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header,
                                         akteZiehenModus:=False, 0, "")
            vorl.ShowDialog()
        End If

        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub btnAktennotiz_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: aktennotiz hinzufügen ")
        detailsTools.eEreignisstarten("aktennotiz schreiben", CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
    End Sub


    Private Sub btnemailschreiben_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: emailschreiben ")
        detailsTools.eEreignisstarten("email schreiben", CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub btnZahlung_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        nachricht("USERAKTION: zahlung hinzufügen ")
        detailsTools.eEreignisstarten("zahlung", CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
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
        Return True
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

        Dim optAuswahl As String = cmbDokumenteFunktionen.SelectedValue.ToString.ToLower
        DokumenteTools(optAuswahl)
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
        'If cmbRaumbezugsFunktionen.SelectedValue.ToString.ToLower.Contains("kartenausschnitt einpassen") Then
        '    clsMiniMapTools.boundingboxComplettNeuErmitteln()
        '    Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myglobalz.sitzung.aktVorgangsID)
        '    INITMiniMapPresentation(myglobalz.sitzung.raumbezugsRec.dt, False)
        'End If

        If cmbRaumbezugsFunktionen.SelectedValue.ToString.ToLower.Contains("eigentümerliste erstellen") Then
            Dim erfolg As Boolean = detailsTools.erstelleCSVausgabeDerFlurstuecke(myGlobalz.sitzung.aktVorgangsID)
            '"O:\UMWELT -PARADIGMA\div\deploy\paradigma\eigentuemerListe\multiFST2CSV.application"
            Process.Start(initP.getValue("ExterneAnwendungen.APPLICATION_MULTIFST2CSV"))



        End If

        If cmbRaumbezugsFunktionen.SelectedValue.ToString.ToLower.Contains("flurstücksraumbezüge") Then
            Dim csvlisteerstellen As New WinCsvliste("")
            csvlisteerstellen.ShowDialog()
        End If
        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, True)
        e.Handled = True
    End Sub




    'Private Sub btnFstsuche(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    panningAusschalten()
    '    btnFstsucheExtracted()
    '    e.Handled = True
    'End Sub

    'Private Sub btnAdrSuche(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    panningAusschalten()
    '    btnAdrSucheExtracted()
    '    e.Handled = True
    'End Sub

    'Private Sub btnEigentuemer(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    '    nachricht("USERAKTION: eigentümer ")
    '    'panningAusschalten()
    '    Dim flst As New WinDetailSucheFST("eigentuemer")
    '    Dim ergebnis As Boolean = CBool(flst.ShowDialog())
    '    If ergebnis Then
    '        setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
    '    Else
    '    End If
    '    e.Handled = True
    'End Sub





    Private Sub dgVorgangFotos_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        e.Handled = True
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
            DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID)
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            Dim ausgabeVerzeichnis As String = ""
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
            dgVorgangDokumente_SelectionChanged_1Extracted("", True)

        End If
        dgVorgangDokumente.SelectedItem = Nothing
        dgVorgangFotos.SelectedItem = Nothing
        dgEreignisse.SelectedItem = Nothing
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
        Dim mycanvas As New Canvas
        mycanvas.Width = 300
        mycanvas.Height = 250
        clsBerichte.fotobucherstellen(mycanvas, True, detail_dokuauswahl.dokListenMergen(Psession.presDokus, Psession.presFotos),
                                  CLstart.myc.kartengen.aktMap.aktrange, True, True)
        e.Handled = True
    End Sub

    'Private Sub setzeHintergrundTextInMiniMap()
    '    tbHGRUND.Text = glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.Hgrund)
    '    tbHGRUND2.Text = glob2.setzeHintergrundTextInMiniMapExtracted(CLstart.myc.kartengen.aktMap.Hgrund)
    'End Sub


    'Private Sub printmap(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Dim druck As New winDruck(myCanvas, ableitungskreis)
    '    druck.ShowDialog()
    '    '  clsBerichte.erstelleKartendruck(myCanvas, ableitungskreis)
    '    e.Handled = True
    'End Sub



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
            nachricht("Sie haben in eine leere Zeile geklickt. Bitte versuchen Sie es nochmal.", ex)
        End Try
    End Sub

    Private Function vid_istOk(ByVal quellVid As Integer) As Boolean
        Try
            Return IsNumeric(quellVid)
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
        detailsTools.AlleDokumentenKopieren(quellVid%, myGlobalz.sitzung.aktVorgangsID, False, allebilder:=True)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        cmbDokuverwandte.SelectedItem = Nothing
        e.Handled = True
    End Sub







    Public Sub NotifyUIThreadOfChangeOFFICE(ByVal e As FileSystemEventArgs)
        '  MsgBox(e.ToString)
    End Sub

    Private Sub btnFotoshinzu2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFotoshinzu2.Click
        nachricht("USERAKTION: fotos hinzufügen ")
        dokuhinzufuegenUndRefresh()
        e.Handled = True
    End Sub

    'Private Sub btnGetCoordinates4Kreis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    nachricht("USERAKTION: kreis ")
    '    If tbKartenGroesseWechsel.Text.Contains("großen") Then
    '        'zur großenkarte
    '        grpMinimapSteuerung.IsEnabled = False
    '        tbKartenGroesseWechsel.Text = "Zur kleinen Karte"
    '        chkBIGGIS.IsChecked = True
    '        tiMap.IsSelected = True
    '    Else
    '        'zur kleinen karte
    '        chkBIGGIS.IsChecked = False
    '        '  tiMap.IsSelected = True
    '        grpMinimapSteuerung.IsEnabled = True
    '        tbKartenGroesseWechsel.Text = "Zur großen Karte"
    '    End If
    '    refreshRaumbezugsListe(myglobalz.sitzung.aktVorgangsID, False, False)
    '    e.Handled = True
    'End Sub

    Private Sub initTabcontrolsMaxheight()
        Dim maxheight As Integer = 500
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
            maxheight = 800 '600
        End If
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
            maxheight = 700 '500
            ' MaxWidth = 1500
            TabControl1.Width = 900
            Top = 1
            'WindowState = WindowState.Maximized
        End If
        '  TabControl1.MaxHeight = maxheight
        'WindowState = Windows.WindowState.Maximized
    End Sub

    'Private Function initcanvasHeight() As Double
    '    Dim maxheight As Double = 670
    '    If myglobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
    '        maxheight = 650 - 25 '25=tbNachricht.Height
    '    End If
    '    If myglobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
    '        maxheight = 399 'TabControl1.Height ' 570
    '    End If
    '    Return maxheight
    'End Function

    Private Function OptionWindowsFont() As Boolean
        Try
            If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
                radWindowsSchriftKlein.IsChecked = True
                radWindowsSchriftMittel.IsChecked = False
            End If
            If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
                radWindowsSchriftKlein.IsChecked = False
                radWindowsSchriftMittel.IsChecked = True
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function initOptionen() As Boolean
        OptionWindowsFont()
        OptionMIniMapDarstellen()
        Return True
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
            nachricht("Sie haben in eine leere Zeile geklickt. Bitte versuchen Sie es nochmal.", ex)
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
            nachricht("fehler in refreshProjekt: ", ex)
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

    'Private Sub rbHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    jfHilfe.starthelp("detail_rb")
    '    e.Handled = True
    'End Sub

    'Private Sub projektHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    jfHilfe.starthelp("detail_projekt")
    '    e.Handled = True
    'End Sub

    'Private Sub verlaufHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    jfHilfe.starthelp("detail_verlauf")
    '    e.Handled = True
    'End Sub

    'Private Sub BeteiligteHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    jfHilfe.starthelp("detail_beteiligte")
    '    e.Handled = True
    'End Sub

    'Private Sub fotosHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    jfHilfe.starthelp("detail_fotos")
    '    e.Handled = True
    'End Sub

    'Private Sub verwandteHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    jfHilfe.starthelp("detail_verwandte")
    '    e.Handled = True
    'End Sub

    'Private Sub dokumentHilfe_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    jfHilfe.starthelp("detail_dokumente")
    '    e.Handled = True
    'End Sub


    Private Sub createPojekt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim aktprojekt = New CLstart.clsProjektAllgemein(myGlobalz.sitzung.aktVorgangsID)
        Dim prj As New WinProjekt("neu", aktprojekt)
        prj.ShowDialog()
        'DialogResult = False
        ' Close()
        e.Handled = True
    End Sub

    Private Sub tbBemerkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        e.Handled = True
        glob2.istTextzulang(540, tbBemerkungReiter)
        schlagworteWurdeGeaendert = True

    End Sub

    Sub New() 'bestandIstGeoeffnet As boolean
        ' Dieser Aufruf ist für den Designer erforderlich.
        Try
            InitializeComponent()

        Catch ex As Exception
            Debug.Print(ex.ToString)
        End Try


        gigabyte = modstartup.Startroutine()
        myglobalz.zuhause = getZuhauseFromInifile()
        'If Environment.UserName = "zahnlückenpimpf" Or
        '   Environment.UserName.ToLower = "feinen_js" Or
        '   Environment.UserName.ToLower = "feinen" Or
        '   Environment.UserName.ToLower = "nhi" Then
        '    myGlobalz.zuhause = True
        'End If
        If myGlobalz.zuhause Then
            suppressUmweltkram()
        End If
        initDarkTheme()
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

        If (CLstart.myc.userIniProfile.WertLesen("GISSTART", "immerbeenden")) = "1" Then
            chkGISimmerbeenden.IsChecked = True
        Else
            chkGISimmerbeenden.IsChecked = False
        End If
    End Sub

    Private Sub suppressUmweltkram()
        grpBoxKosten.Visibility = Visibility.Collapsed
        btnAkteziehen.Visibility = Visibility.Collapsed
        grpbauantrag.Visibility = Visibility.Collapsed
        grpKontakte.Visibility = Visibility.Collapsed
        tabheaderProjekt.Visibility = Visibility.Collapsed
        btngooglestarten.Visibility = Visibility.Visible
        btngislayer.Visibility = Visibility.Collapsed
        btnOutlookemailuebernehmen.Visibility = Visibility.Collapsed

        btnemailschreiben.Visibility = Visibility.Collapsed

        btnStartBplan.Visibility = Visibility.Collapsed
        btnStartBplan.Visibility = Visibility.Collapsed
        btnStartBplan.Visibility = Visibility.Collapsed
    End Sub

    Private Sub schlagworteEinfaerben()
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung) Then
            tabheaderSchlagworte.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
        Else
            tabheaderSchlagworte.SetValue(TextElement.FontWeightProperty, FontWeights.Normal)
        End If
    End Sub

    Private Sub btnBestand_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        MsgBox("Bitte wechseln sie über die Windows-Taskleiste in Hauptmenü und wählen Sie dort 'Bestand'")
        'Close()
        ' retcode = "zurBestandsUebersicht"
        'End If
        e.Handled = True
    End Sub

    Private Sub initStammBearbeiterTooltip()
        '  tbMAtooltip.Text = Paradigma_start.Win1Tools.BildeBearbeiterProfilalsString(myglobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter)
        ' tbBearbeiterKuerzel.ToolTip = tbBearbeiter.ToolTip


        'Dim logo As BitmapImage = New BitmapImage()
        'logo.BeginInit()
        'Try
        '    logo.UriSource = New Uri("O:/UMWELT -PARADIGMA/div/images/" & myglobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale &
        '                       ".jpg")
        '    logo.EndInit()

        '    imgMA.Source = logo
        'Catch ex As Exception

        'End Try

    End Sub

    Private Sub Verteiler_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim verteiler As String
        verteiler = mailTools.emailVerteilerBilden(myGlobalz.anhangtrenner)
        Clipboard.Clear()
        Clipboard.SetText(verteiler)
        MsgBox(glob2.getMsgboxText("Verteiler", New List(Of String)(New String() {})))
    End Sub




    Private Sub btnNatureg_Click_1(sender As Object, e As RoutedEventArgs)
        nachricht("USERAKTION: zu natureg ")
        Dim ggg As New VorgangUebersicht
        ggg.ShowDialog()
        e.Handled = True
    End Sub

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





    Private Sub kostenggemeinsam()
        myGlobalz.sitzung.aktVorgang.KostenStatus.QUELLE = myGlobalz.sitzung.aktBearbeiter.Initiale
        Dim lKostenToolspeichern As Boolean = kostenTool.speichern(myGlobalz.sitzung.aktVorgang.KostenStatus,
                                                               myGlobalz.sitzung.aktBearbeiter.Initiale,
                                                               myGlobalz.sitzung.aktVorgangsID)
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
            nachricht("fehler in refreshKosten: ", ex)
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

            VERWARNUNGSGELD.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.VERWARNUNGSGELD, True, False)
            VerwarnungsgeldBezahlt.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.VerwarnungsgeldBezahlt, True, False)

            BUSSGELD.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.BUSSGELD, True, False)
            BUSSGELDBezahlt.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.BUSSGELDBezahlt, True, False)

            Zwangsgeld.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.ZWANGSGELD, True, False)
            ZwangsgeldBezahlt.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.ZWANGSGELDBezahlt, True, False)

            beihilfe.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.BEIHILFE, True, False)
            beihilfeBezahlt.IsChecked = If(myGlobalz.sitzung.aktVorgang.KostenStatus.BEIHILFEBezahlt, True, False)

        Catch ex As Exception
            nachricht("fehler in kostenstatusDarstellen: ", ex)
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
            If myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer Then
                grpBoxKosten.IsEnabled = True
            Else
                grpBoxKosten.IsEnabled = False
            End If
        Catch ex As Exception
            nachricht("fehler in initKostenFeld: ", ex)
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
            l("setMitDokumenten")
            If chkExpandereignis.IsChecked Then
                ereignisDokExpand = True
                CLstart.myc.userIniProfile.WertSchreiben("Verlauf", "mitDokumenten", "1")
            Else
                ereignisDokExpand = False
                CLstart.myc.userIniProfile.WertSchreiben("Verlauf", "mitDokumenten", "0")
            End If
            l("setMitDokumenten ende")
        Catch ex As Exception
            nachricht("fehler in setMitDokumenten: ", ex)
        End Try
    End Sub
    Private Sub ckeckExpander(sender As Object, e As RoutedEventArgs)
        ' setMitDokumenten()
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub


    Private Sub btnDokumenteZwischenablage_Click(sender As Object, e As RoutedEventArgs)
        handleZwischenablage()
        e.Handled = True
    End Sub

    Private Sub handleZwischenablage()
        Try
            Dim clipper As New winClipBoard
            clipper.ShowDialog()
            tbClipText.Text = clipper.grabtext
            'If meinClipboard.getContentFromZwischenablage Then
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            'End If
        Catch ex As Exception
            nachricht("fehler in handleZwischenablage: ", ex)
        End Try
    End Sub

    Private Sub VerlaufmitDokumentenSetzen()
        Try
            If CInt(CLstart.myc.userIniProfile.WertLesen("Verlauf", "mitDokumenten")) = 0 Then
                chkExpandereignis.IsChecked = False
            Else
                chkExpandereignis.IsChecked = True
            End If
        Catch ex As Exception
            nachricht("warnung in VerlaufmitDokumentenSetzen: ", ex)
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
        If Not ladevorgangAbgeschlossen Then Exit Sub
        If cmbGemKRZ.SelectedItem Is Nothing Then Exit Sub
        Dim item As String = CType(cmbGemKRZ.SelectedValue, String)
        myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = item.Trim
        tbGEMKRZ.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "GEMKRZ")
        e.Handled = True
    End Sub

    Private Sub cmbGemKRZ_MouseMove(sender As Object, e As MouseEventArgs)
        cmbGemKRZ.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub



    Private Sub tbGEMKRZ_MouseEnter(sender As Object, e As MouseEventArgs)
        cmbGemKRZ.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub

    Private Sub btnWeitereBearbeiterListen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
            MsgBox("Diese Änderung darf nur vom Eigentümer, Admin oder einem bestehenden 'Weiteren Bearbeiter' durch geführt werden. ",
               MsgBoxStyle.OkOnly,
               "Keine Rechte: Abbruch")
            Exit Sub
        End If
        '   glob2.setzeZugriffsrechte()

        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl("mehrfachauswahlInitial", CLstart.myc.AZauswahl.WeitereBearbeiter)
        bearbeiterauswahlbox.ShowDialog()

        If bearbeiterauswahlbox.mehrfachauswahlsumme = "####" Then
            CLstart.myc.AZauswahl.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
        Else
            CLstart.myc.AZauswahl.WeitereBearbeiter = bearbeiterauswahlbox.mehrfachauswahlsumme
        End If
        'CLstart.myc.AZauswahl.WeitereBearbeiter = bearbeiterauswahlbox.mehrfachauswahlsumme 'getMehrereBearbeiter(bearbeiterauswahlbox.mehrfachauswahlsumme, CLstart.myc.AZauswahl.WeitereBearbeiter)


        tbWeitereBearbeiter.Text = CLstart.myc.AZauswahl.WeitereBearbeiter
        'If detailsTools.SollAlsStandardSetzen(CLstart.myc.AZauswahl.WeitereBearbeiter) Then
        '    detailsTools.WeitereBearbeiterImCookieSpeichern(CLstart.myc.AZauswahl.WeitereBearbeiter)
        'End If
        myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "WEITEREBEARB")
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

    'Private Sub cmbSachgebietsFormulare_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    '    If cmbSachgebietsFormulare.SelectedItem Is Nothing Then Exit Sub
    '    If cmbSachgebietsFormulare.SelectedValue Is Nothing Then Exit Sub
    '    Dim item As String = cmbSachgebietsFormulare.SelectedValue.ToString
    '    item = item.Replace("System.Windows.Controls.ComboBoxItem: ", "").Trim.ToLower
    '    detailsTools.zumSGformular(item, myGlobalz.sitzung.aktVorgangsID)
    '    '     myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = item.Trim
    '    'detailsTools.Edit_singleUpdate_Stammdaten(Now, "GEMKRZ")
    '    e.Handled = True
    'End Sub

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
        tbParagraph.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Paragraf
        e.Handled = True
    End Sub


    Private Sub cmbRaumNr_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As String = CType(cmbRaumNr.SelectedValue, String)
        myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.RaumNr = cmbRaumNr.SelectedValue.ToString
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "STORAUMNR")
        tbStandort.Text = myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.RaumNr
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
    Private Sub btnzuVorgang_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        GC.Collect()
        myGlobalz.einVorgangistgeoeffnet = False
        Dim az$ = "", header$ = ""
        Dim vorgangsid = clsStartup.suchenNachVorgaengen(az, header)
        e.Handled = True
        If IsNumeric(vorgangsid) Then
            If chkAktVorgangSchliessen.IsChecked Then
                Close()
                myGlobalz.sitzung.aktVorgangsID = CInt(vorgangsid)
                LocalParameterFiles.erzeugeParameterDateiAktvorgang_txt(False, False)
                myGlobalz.sitzung.modus = "edit"
                CLstart.VIDuebergabe.holedetailVonVorgang(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktBearbeiter.Initiale)
            Else
                Dim cvt As New VerwandteTools.divers
                cvt.startNewVorgang(CInt(vorgangsid))
                cvt = Nothing
            End If
        End If
    End Sub

    Private Sub btnNeuerVorgang_Click(sender As Object, e As RoutedEventArgs)
        'GC.Collect() 
        e.Handled = True
        CLstart.VIDuebergabe.erzeugeParameterDatei(CInt(myGlobalz.sitzung.aktVorgangsID), myGlobalz.sitzung.aktBearbeiter.username, CLstart.mycSimple.Paradigma_local_root, "vorherigerVorgang")
        'Close()
        CLstart.mycSimple.neuerVorgang3("normal")
        GC.Collect()
        If chkAktVorgangSchliessen2.IsChecked Then
            Close()
        End If
        'If clsStartup.abbruch Then
        'Else
        '    '   Close()
        'End If
    End Sub

    Private Sub lblVorgangsID_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = False
            MsgBox("Der Status des Vorgangs wurde auf >UNERLEDIGT< geändert.")
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = True
            'MsgBox("Der Status des Vorgangs wurde auf >ERLEDIGT< geändert.")
            If glob2.UserFragenObNach_PDFA_Kopieren() Then
                alleWordDokusNachPdfaKopieren()
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            End If
        End If
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "ERLEDIGT")
        setzeErledigtflagfarbe()
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
        e.Handled = True
        If detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presDokus) > 0 Then
            Dim icount As Integer = Dokus_MehrfachLoeschen()

            If icount > 0 Then
                MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten gelöscht.")
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            Else
                MessageBox.Show("Es wurden " & icount & " Dokumente gelöscht.")
            End If

        Else
            MsgBox("Sie haben keine Auswahl getroffen.")
        End If
    End Sub
    Private Sub chkAllesMarkieren_Click(sender As Object, e As RoutedEventArgs)
        If Not chkAllesMarkieren.IsChecked Then
            presDokusAusgewaehltMarkieren(False, Psession.presDokus)
        Else
            alteMarkierungen.Clear()
            For Each ele As clsPresDokumente In Psession.presDokus
                presDokusAusgewaehltMarkieren(True, Psession.presDokus)
            Next
        End If
        e.Handled = True
    End Sub

    Private Sub btnMehrfachKopieren_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presDokus) > 0 Then
            Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachKopieren(Psession.presDokus, alteMarkierungen)
            MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten kopiert.")
        Else
            MsgBox("Sie haben keine Auswahl getroffen.")
        End If
        'die mehrfachauswahl darf nur einmal benutzt werden, weil die cdokus in der collection nach dem ersten 
        'kopieren völlig falsche informationen enthalten, z.B. falsche dokid!!!!
        'mitMehrfachauswahl.IsChecked = False
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
    End Sub
    Private Sub btnMehrfachRevisionssichern_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presDokus) > 0 Then
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
        e.Handled = True
        If detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presFotos) > 0 Then
            Dim icount As Integer = Fotos_MehrfachLoeschen()
            MessageBox.Show("Es wurden " & icount & " Fotos gelöscht.")
            'MessageBox.Show("Es wurden " & icount & " von " & Sitzung.presFotos.Count & " Fotos gelöscht.")
        Else
            MessageBox.Show("Es wurden nichts ausgwählt.")
        End If

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
        e.Handled = True
        If Not chkAllesMarkierenFotos.IsChecked Then
            presDokusAusgewaehltMarkieren(False, Psession.presFotos)
        Else
            For Each ele As clsPresDokumente In Psession.presFotos
                presDokusAusgewaehltMarkieren(True, Psession.presFotos)
            Next
        End If

    End Sub

    Private Sub btnMehrfachFotosRevisionssichern_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presFotos) > 0 Then

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
        e.Handled = True
        If (detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presFotos) + detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presDokus)) > 0 Then
            Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachMailen()
            MessageBox.Show("Es wurden " & icount & " Dokumente gemailt.")
            'MessageBox.Show("Es wurden " & icount & " von " & Sitzung.presDokus.Count & " Dokumenten gemailt.")
        Else
            MsgBox("Sie haben nichts ausgewählt.")
        End If

    End Sub



    Private Sub btnMehrfachFotosKopieren_Click(sender As Object, e As RoutedEventArgs)
        If detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presFotos) > 0 Then
            Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachKopieren(Psession.presFotos, alteMarkierungen)
            MessageBox.Show("Es wurden " & icount & " von " & Psession.presFotos.Count & " Fotos kopiert.")
        Else
            MsgBox("Sie haben keine Auswahl getroffen.")
        End If
        e.Handled = True
    End Sub

    Private Sub btnFotosRefresh2_Click(sender As Object, e As RoutedEventArgs) Handles btnFotosRefresh2.Click
        nachricht("USERAKTION:fotos refresh ")
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
        cmbDokuverwandte.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub btnDokumenteRefresh2_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        tbFilter.Text = ""
        resetDokuliste_ClickExtracted()
    End Sub

    Private Sub btnAkteziehen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim pfadAllgemein As String = ""
        Dim quelldatei As String
        Dim vorlagenVerzeichnis As IO.DirectoryInfo = Nothing
        Dim endung As String = ".docx"
        clsVorlagenTools.berechneVorlagenverzeichnis(vorlagenVerzeichnis, "Allgemein", pfadAllgemein)
        pfadAllgemein = pfadAllgemein.Replace("\\", "\")
        'quelldatei = pfadAllgemein & "\" & "Bitte Akte ziehen.docx"
        quelldatei = "Bitte Akte ziehen.docx"
        Dim vorl As New WinVorlagenListe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl,
                                   myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header,
                                   True, 0, quelldatei)
        vorl.ShowDialog()
        nachricht("WinVorlageSteuerung weiter: 7")
        nachricht("WinVorlageSteuerung weiter: ENDE")
    End Sub

    Private Sub Protokollzugriffschalten()
        Try
            If myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer Then
                btnProtokkkoll.Visibility = Windows.Visibility.Visible
            Else
                btnProtokkkoll.Visibility = Windows.Visibility.Collapsed
            End If
        Catch ex As Exception

        Finally
        End Try
    End Sub

    'Private Sub markiereGeoeffneteDokumente(alle As List(Of clsPresDokumente), checkouts As List(Of Dokument))
    '    Try
    '        For Each presdok In alle
    '            presdok.istGeoeffnet = False
    '        Next
    '        For Each presdok In alle
    '            For Each checkdok In checkouts
    '                If presdok.DateinameMitExtension = checkdok.DateinameMitExtension Then
    '                    presdok.istGeoeffnet = True
    '                    Continue For
    '                End If
    '            Next
    '        Next
    '    Finally

    '    End Try
    'End Sub





    'Private Sub setzeKartenbreiteAuf(item As String)
    '    Dim breite As Double = CDbl(item)
    '    CLstart.myc.kartengen.aktMap.aktrange.CalcCenter()
    '    CLstart.myc.kartengen.aktMap.aktrange.xl = CLstart.myc.kartengen.aktMap.aktrange.xcenter - (breite / 2)
    '    CLstart.myc.kartengen.aktMap.aktrange.xh = CLstart.myc.kartengen.aktMap.aktrange.xcenter + (breite / 2)
    '    Dim hohe As Double = breite / 2
    '    CLstart.myc.kartengen.aktMap.aktrange.yl = CLstart.myc.kartengen.aktMap.aktrange.ycenter - (hohe / 2)
    '    CLstart.myc.kartengen.aktMap.aktrange.yh = CLstart.myc.kartengen.aktMap.aktrange.ycenter + (hohe / 2)
    '    presentMapOLD()
    'End Sub

    Private Sub btnFotoscheckout_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: alles fotos zum PC ")
        'myGlobalz.Arc.AllesAuscheckenVorgang(True, True)
        myGlobalz.Arc.AlleFotosAuscheckenVorgang(True, True)

    End Sub

    Private Sub altesRaumbezugsobjektLoeschen()
        myGlobalz.sitzung.aktPolygon.ShapeSerial = ""
        myGlobalz.sitzung.aktPolygon.GKstring = ""
        'myGlobalz.sitzung.aktPolygon.shapefile = ""
        myGlobalz.sitzung.aktPolygon.originalQuellString = ""
    End Sub


    'Private Sub chkBIGGIS_Click(sender As Object, e As RoutedEventArgs) Handles chkBIGGIS.Click
    '    If chkBIGGIS.IsChecked Then
    '        refreshRaumbezugsListe(myglobalz.sitzung.aktVorgangsID, False, False)
    '    Else
    '        tiSachdaten.IsSelected = True
    '        refreshRaumbezugsListe(myglobalz.sitzung.aktVorgangsID, False, False)
    '    End If
    '    e.Handled = True
    'End Sub





    'Private Sub btnFstsucheExtracted()
    '    nachricht("USERAKTION: flst suchen ")
    '    Dim flst As New WinDetailSucheFST("ort")
    '    Dim ergebnis As Boolean = CBool(flst.ShowDialog())
    '    If ergebnis Then
    '        setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
    '    Else
    '    End If
    '    cmbSuchen.SelectedIndex = 0
    'End Sub
    'Private Sub btnAdrSucheExtracted()
    '    nachricht("USERAKTION: adr suchen ")
    '    Dim adrs As New winDetailAdressSuche
    '    Dim ergebnis As Boolean = CBool(adrs.ShowDialog())
    '    If ergebnis Then
    '        setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
    '    Else
    '    End If
    '    cmbSuchen.SelectedIndex = 0
    'End Sub



    Private Sub btnDS_gruppen_Click(sender As Object, e As RoutedEventArgs)
        Dim grp As New WinDS_gruppen
        grp.ShowDialog()
        e.Handled = True
    End Sub


    Private Sub btnScanClick(sender As Object, e As RoutedEventArgs)
        btnScanClickExtracted()
        e.Handled = True
    End Sub




    Private Sub btnScanClickExtracted()
        GC.Collect()
        Dim PDF_TIFFdateiname As String
        PDF_TIFFdateiname = PDF_TIFFdateinameErzeugenUndUEbergeben()
        warteschleifeScanner(initP.getValue("ExterneAnwendungen.APPLICATION_Scanner"))
        GC.Collect()
        'dateiInsArchiv
        If dateiFromScanInsArchiv(PDF_TIFFdateiname) Then
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        End If
    End Sub

    'Private Sub fallsPUnkteDannRadiusAddieren(clsRange As clsRange, radius As Double)
    '    If clsRange.xl = clsRange.xh Then
    '        clsRange.xl = clsRange.xl - radius
    '        clsRange.xh = clsRange.xh + radius
    '        clsRange.yl = clsRange.yl - radius
    '        clsRange.yh = clsRange.yh + radius
    '    End If
    'End Sub

    Sub sachgebietsDBknopfEinschalten(sgnr As String)
        If sgnr.Trim = "3307" Then
            buttonIllegaleBautenEinschalten()
        Else
            buttonIllegaleBautenausschalten()
        End If
        If sgnr.Trim.StartsWith("3") Or sgnr.Trim.StartsWith("3") Then
            buttonNaturegEinschalten()
            'buttonNaturegviewerEinschalten()
        Else
            buttonNaturegausschalten()
            'buttonNaturegviewerausschalten()
        End If
    End Sub
    Sub buttonNaturegausschalten()
        btnNatureg.Visibility = Windows.Visibility.Hidden
    End Sub
    'Sub buttonNaturegviewerausschalten()
    '    btnNaturegv.Visibility = Windows.Visibility.Hidden
    'End Sub
    Private Sub buttonIllegaleBautenausschalten()
        btnIllegale.Visibility = Windows.Visibility.Hidden
    End Sub
    Private Sub buttonIllegaleBautenEinschalten()
        btnIllegale.Visibility = Windows.Visibility.Visible
    End Sub
    Sub buttonNaturegEinschalten()
        btnNatureg.Visibility = Windows.Visibility.Visible
    End Sub
    'Sub buttonNaturegviewerEinschalten()
    '    btnNaturegv.Visibility = Windows.Visibility.Visible
    'End Sub
    Private Sub btnIllegale_Click(sender As Object, e As RoutedEventArgs)
        Dim ill As New WinIllegaleDetail(CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
        ill.ShowDialog()
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    Private Sub alleButtonsAusschalten()
        btnIllegale.Visibility = Windows.Visibility.Collapsed
    End Sub


    Private Sub radWindowsSchriftKlein_Checked(sender As Object, e As RoutedEventArgs) Handles radWindowsSchriftKlein.Checked
        If ladevorgangAbgeschlossen Then
            If radWindowsSchriftKlein.IsChecked Then
                CLstart.myc.userIniProfile.WertSchreiben("WINDOWS_SYSTEM_ANZEIGE", "FONT", "0")
            Else
                CLstart.myc.userIniProfile.WertSchreiben("WINDOWS_SYSTEM_ANZEIGE", "FONT", "1")
            End If
        End If
    End Sub

    Private Sub radWindowsSchriftMittel_Checked(sender As Object, e As RoutedEventArgs) Handles radWindowsSchriftMittel.Checked
        If ladevorgangAbgeschlossen Then
            If radWindowsSchriftMittel.IsChecked Then
                CLstart.myc.userIniProfile.WertSchreiben("WINDOWS_SYSTEM_ANZEIGE", "FONT", "1")
            Else
                CLstart.myc.userIniProfile.WertSchreiben("WINDOWS_SYSTEM_ANZEIGE", "FONT", "0")
            End If
        End If
    End Sub

    Private Sub OptionMIniMapDarstellen()
        If useExternGis Then
            '  chkMiniMapDarstellen.IsChecked = False
            'grpMinimapSteuerung.Visibility = Windows.Visibility.Collapsed
            'canvasborder.Visibility = Windows.Visibility.Collapsed
            '  TabControl1.Width = 1920

            getCheckboxGisstart()
        Else
            If Not detailsTools.mitMiniMapDarstellen() Then
                chkMiniMapDarstellen.IsChecked = False
                'grpMinimapSteuerung.Visibility = Windows.Visibility.Collapsed
                'canvasborder.Visibility = Windows.Visibility.Collapsed
                'TabControl1.Width = 1920
            Else
                chkMiniMapDarstellen.IsChecked = True
                'grpMinimapSteuerung.Visibility = Windows.Visibility.Visible
                'canvasborder.Visibility = Windows.Visibility.Visible
                TabControl1.Width = 783
            End If
        End If

    End Sub

    Private Sub getCheckboxGisstart()
        Try
            l("getCheckboxGisstart---------------------- anfang")
            Dim wert As String
            wert = CLstart.myc.userIniProfile.WertLesen("NOMAP", "neverShowMinimap")
            If Not String.IsNullOrEmpty(wert) Then
                If wert = "1" Then
                    chkNoGISever.IsChecked = CType(1, Boolean?)
                    chkMiniMapDarstellen.IsEnabled = False
                Else
                    chkNoGISever.IsChecked = CType(0, Boolean?)
                    chkMiniMapDarstellen.IsEnabled = True
                End If
            Else
                CLstart.myc.userIniProfile.WertSchreiben("NOMAP", "neverShowMinimap", CType(1, String))
            End If
            '----------------------------
            wert = CLstart.myc.userIniProfile.WertLesen("GISSTART", "allegissekillen")
            If wert = "1" Then
                paradigmaKILLALLGIS.IsChecked = True
            Else
                paradigmaKILLALLGIS.IsChecked = False
            End If
            '----------------------------
            If chkNoGISever.IsChecked Then Exit Sub
            wert = CLstart.myc.userIniProfile.WertLesen("NOMAP", "VID")
            If Not String.IsNullOrEmpty(wert) Then
                Dim b As String
                b = CLstart.myc.userIniProfile.WertLesen("NOMAP", "VID")
                If LIBgemeinsames.clsString.isinarray(b, myGlobalz.sitzung.aktVorgangsID.ToString, ",") Then
                    chkMiniMapDarstellen.IsChecked = CType(0, Boolean?)
                Else
                    chkMiniMapDarstellen.IsChecked = CType(1, Boolean?)
                End If
            Else
                ' CLstart.myc.userIniProfile.WertSchreiben("NOMAP", "zweiterbildschirmvorhanden", CType(0, String))
            End If
            l("getCheckboxGisstart---------------------- ende")
        Catch ex As Exception
            l("Fehler in getCheckboxGisstart: ", ex)
        End Try
    End Sub

    Private Sub chkMiniMapDarstellen_Checked(sender As Object, e As RoutedEventArgs)
        If ladevorgangAbgeschlossen = False Then Return
        If Not detailsTools.mitMiniMapDarstellen() Then
            Dim summe As String
            summe = CLstart.myc.userIniProfile.WertLesen("NOMAP", "VID")
            summe = summe.Replace(myGlobalz.sitzung.aktVorgangsID.ToString, " ")
            summe = LIBgemeinsames.clsString.nodoubleStrings(summe, CChar(","))
            summe = summe.Replace(", ,", "")
            CLstart.myc.userIniProfile.WertSchreiben("NOMAP", "VID", summe)
            'grpMinimapSteuerung.Visibility = Windows.Visibility.Visible
            'canvasborder.Visibility = Windows.Visibility.Visible
            'TabControl1.Width = 783
        End If
        e.Handled = True
    End Sub

    Private Sub chkMiniMapDarstellen_Unchecked(sender As Object, e As RoutedEventArgs)
        If ladevorgangAbgeschlossen = False Then Return
        If detailsTools.mitMiniMapDarstellen() Then
            Dim summe As String
            summe = CLstart.myc.userIniProfile.WertLesen("NOMAP", "VID") & "," & myGlobalz.sitzung.aktVorgangsID
            summe = summe.Replace(", ,", "")
            CLstart.myc.userIniProfile.WertSchreiben("NOMAP", "VID", summe)
            'grpMinimapSteuerung.Visibility = Windows.Visibility.Collapsed
            'canvasborder.Visibility = Windows.Visibility.Collapsed
            'TabControl1.Width = 1920
        End If
        e.Handled = True
    End Sub

    Private Sub DokumenteTools(ByVal optAuswahl As String)
        If optAuswahl.Contains("alle fotos löschen") Then
            Dim icount As Integer = alleDokusUndFotosLoeschen("nurfotos")
            If icount > 0 Then
                'refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            End If
        End If
        If optAuswahl.Contains("alle dokumente löschen") Then
            Dim icount As Integer = alleDokusUndFotosLoeschen("keinefotos")
            If icount > 0 Then
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                'refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            End If
        End If
        If optAuswahl.Contains("alle dokumente und fotos löschen") Then
            Dim icount As Integer = alleDokusUndFotosLoeschen("beides")
            If icount > 0 Then
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                refreshFotos(myGlobalz.sitzung.aktVorgangsID)
            End If
        End If
        If optAuswahl.Contains("alle dokumente und fotos zu anderem vorgang kopieren") Then
            alleDokuUndFotosZuFremdvorgangKopieren(alteMarkierungen)
        End If
        If optAuswahl.Contains("alle dokumente und fotos revisionssicher speichern") Then
            alleDokusRevisionssicherMachen()
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        End If
        If optAuswahl.Contains("alle worddokumente nach pdf/a kopieren") Then
            alleWordDokusNachPdfaKopieren()
            refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        End If
    End Sub

    Private Sub btnAllPDFAClick(sender As Object, e As RoutedEventArgs)
        DokumenteTools("alle worddokumente nach pdf/a kopieren")
        e.Handled = True
    End Sub

    Private Sub btnWeitereBAuswahl_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        'Dim winzu As New winEmailListe("weitereBearbeiterStandard")
        'winzu.ShowDialog()

        If Not myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
            MsgBox("Diese Änderung darf nur vom Eigentümer, Admin oder einem bestehenden 'Weiteren Bearbeiter' durch geführt werden. ",
               MsgBoxStyle.OkOnly,
               "Keine Rechte: Abbruch")
            Exit Sub
        End If
        '   glob2.setzeZugriffsrechte()

        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl("mehrfachauswahlInitial", CLstart.myc.AZauswahl.WeitereBearbeiter)
        bearbeiterauswahlbox.ShowDialog()

        If bearbeiterauswahlbox.mehrfachauswahlsumme = "####" Then
            CLstart.myc.AZauswahl.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
        Else
            CLstart.myc.AZauswahl.WeitereBearbeiter = bearbeiterauswahlbox.mehrfachauswahlsumme
        End If
        'CLstart.myc.AZauswahl.WeitereBearbeiter = bearbeiterauswahlbox.mehrfachauswahlsumme 'getMehrereBearbeiter(bearbeiterauswahlbox.mehrfachauswahlsumme, CLstart.myc.AZauswahl.WeitereBearbeiter)


        tbWeitereBearbeiter.Text = CLstart.myc.AZauswahl.WeitereBearbeiter
        'If detailsTools.SollAlsStandardSetzen(CLstart.myc.AZauswahl.WeitereBearbeiter) Then
        '    detailsTools.WeitereBearbeiterImCookieSpeichern(CLstart.myc.AZauswahl.WeitereBearbeiter)
        'End If
        myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = CLstart.myc.AZauswahl.WeitereBearbeiter
        detailsTools.Edit_singleUpdate_Stammdaten(Now, "WEITEREBEARB")


        tbWeitereBearbeiterStandard.Text = CLstart.myc.AZauswahl.WeitereBearbeiter
        'If detailsTools.SollAlsStandardSetzen(CLstart.myc.AZauswahl.WeitereBearbeiter) Then
        detailsTools.WeitereBearbeiterImCookieSpeichern(tbWeitereBearbeiterStandard.Text)
        'End If
    End Sub

    'Private Sub ckbeigentuemerFunktion_Click(sender As Object, e As RoutedEventArgs)
    '    panningAusschalten()

    '    INITMiniMapPresentation(myglobalz.sitzung.raumbezugsRec.dt, useLastRange:=True)
    '    e.Handled = True
    'End Sub

    'Private Sub btnAktiveEbene_Click(sender As Object, e As RoutedEventArgs)
    '    Dim mesresult As MessageBoxResult
    '    mesresult = MessageBox.Show("Soll der Hintergrund aktiv geschaltet werden ?" & Environment.NewLine &
    '                   "     ok = Ja " & Environment.NewLine,
    '                  "Hintergrund aktivieren (z.Z.: )" & CLstart.myc.kartengen.aktMap.ActiveLayer,
    '                             MessageBoxButton.OKCancel)
    '    If CBool(mesresult) Then
    '        CLstart.myc.kartengen.aktMap.ActiveLayer = CLstart.myc.kartengen.aktMap.Hgrund.Replace(";", "")
    '        tbAktiveEbene2.Text = CLstart.myc.kartengen.aktMap.Hgrund
    '        eigentuemerfunktionAusschalten()
    '        panningAusschalten()
    '        presentMapOLD()
    '        'INITMiniMapPresentation(myglobalz.sitzung.raumbezugsRec.dt, useLastRange:=True)
    '    Else
    '    End If
    '    e.Handled = True
    'End Sub

    Private Sub btnFremdDokus_Click(sender As Object, e As RoutedEventArgs)
        Dim fremddokus As New winFremdDokus
        fremddokus.ShowDialog()
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub



    'Private Sub btnNaturegv_Click(sender As Object, e As RoutedEventArgs)
    '    Try
    '        Process.Start("http://natureg.hessen.de")
    '    Catch ex As Exception

    '    End Try
    '    e.Handled = True
    'End Sub

    Private Sub btnTxtfileanlegen_Click(sender As Object, e As RoutedEventArgs)
        Dim txtDok As New Dokument
        txtDok.DateinameMitExtension = "Notiz_" & Now.ToString("yyMMddhhmmss") & ".txt" 'neuername
        txtDok.Filedatum = Now
        txtDok.Checkindatum = Now
        txtDok.Beschreibung = "Notiz" ''Schlagworte
        txtDok.newSaveMode = True
        txtDok.dokumentPfad = myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir
        Dim ausgabeVerzeichnis As String = ""
        txtDok.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, txtDok.DocID, ausgabeVerzeichnis)

        Dokument.createCheckoutDir(myGlobalz.Arc.lokalerCheckoutcache, txtDok.DocID, myGlobalz.sitzung.aktVorgangsID)

        If txtTools.leeresTextFileErzeugen(txtDok.FullnameCheckout) Then
            If txtTools.checkinausfuehren(txtDok) Then
                refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
                DokArc.TXT_handeln(txtDok, "neu")
            End If
        End If
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
        e.Handled = True
    End Sub

    'Private Sub btngoogle3d_Click(sender As Object, e As RoutedEventArgs)
    '    Try
    '        nachricht("USERAKTION: googlekarte  vogel btn click")
    '        Dim gis As New clsGISfunctions
    '        Dim result As String
    '        result = gis.GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(CLstart.myc.kartengen.aktMap.aktrange, False)
    '        If result = "fehler" Or result = "" Then
    '        Else
    '            '  gis.starten(result)
    '            '  GMtemplates.templateStarten(result)
    '            ' wbSample.Navigate(New Uri(result))
    '            Process.Start("C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", result)
    '        End If
    '        gis = Nothing
    '    Catch ex As Exception
    '        nachricht("fehler in starteWebbrowserControl: ", ex)
    '        e.Handled = True
    '    End Try
    'End Sub



    'Private Sub btnaktualisiernvogel_Click(sender As Object, e As RoutedEventArgs)
    '    starteWebbrowserControl()
    '    e.Handled = True
    'End Sub



    Private Sub btnwrrl_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("http://wrrl.hessen.de/")
        e.Handled = True
    End Sub

    Private Sub btnBoden_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("http://bodenviewer.hessen.de/mapapps/resources/apps/bodenviewer/index.html?lang=de")
        e.Handled = True
    End Sub

    Private Sub btnhalm_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("http://halm.hessen.de/")
        e.Handled = True
    End Sub

    Private Sub btnhochw_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("http://www.hlnug.de/static/pegel/wiskiweb2/")
        e.Handled = True
    End Sub

    Private Sub btnlaerm_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("http://laerm.hessen.de/")
        e.Handled = True
    End Sub



    Private Sub btnUeber_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub btnnatureg2_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("http://natureg.hessen.de/")
        e.Handled = True
    End Sub

    'Private Sub dgrechts_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

    'End Sub



    'Private Sub btnAllgemein_Click(sender As Object, e As RoutedEventArgs)
    '    'http://www.geoportal.hessen.de/portal/karten.html
    '    webbrowserGeoportal("http://www.geoportal.hessen.de/portal/karten.html")
    '    e.Handled = True
    'End Sub
    'Private Sub webbrowserGeoportal(ziel As String)
    '    Try
    '        nachricht("USERAKTION: googlekarte  vogel")
    '        Dim gis As New clsGISfunctions
    '        Dim result As String
    '        '  result = gis.GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(CLstart.myc.kartengen.aktMap.aktrange, True)
    '        If ziel = "fehler" Or ziel = "" Then
    '        Else
    '            '  gis.starten(result)
    '            '  GMtemplates.templateStarten(result)
    '            wbgeoportal.Navigate(New Uri(ziel))
    '        End If
    '        gis = Nothing
    '    Catch ex As Exception
    '        nachricht("fehler in webbrowserGeoportal", ex)
    '    End Try
    'End Sub
    'Private Sub dgrechtsdb_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

    'End Sub

    Private Sub dgrechtsgrundlagen_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            Dim item As New clsgesetzesManagerDok
            item = CType(dgrechtsgrundlagen.SelectedItem, clsgesetzesManagerDok)
            If item Is Nothing Then Return
            Dim aktauswahl As String
            aktauswahl = CType(item.stammid, String)
            If chkgesetzmanagerModusLoeschenAendern.IsChecked Then
                'edit mode
                Dim editGesetz As New winRechtsDBMan(item)
                editGesetz.ShowDialog()
            Else
                'aufrufmodus, öffnen
                glob2.OpenDocument(modrechtsdb.GesetzesDateiausChecken(item))
            End If
            e.Handled = True
        Catch ex As Exception
            nachricht("BeteiligtenAusgewaehlt: " & String.Format("BeteiligtenAusgewaehlt: {0}", ex))
        End Try
        e.Handled = True
    End Sub

    Private Sub btnHinzuRechtsdb_Click(sender As Object, e As RoutedEventArgs)
        Dim rechtsb As New winRechtsDBMan
        rechtsb.ShowDialog()
        e.Handled = True
    End Sub
    'Private Sub btnfilterRechtsdb_Click(sender As Object, e As RoutedEventArgs)
    '    'MsgBox("baustelle")
    '    e.Handled = True
    'End Sub

    Private Sub btnRefreshGesetze_Click(sender As Object, e As RoutedEventArgs)
        refreshGesetzdb(tbSuchSGnr.Text.Trim, False)
        e.Handled = True
    End Sub

    Private Sub chkgesetzmanagerModusLoeschenAendern_Click(sender As Object, e As RoutedEventArgs)
        refreshGesetzdb(tbSuchSGnr.Text.Trim, False)
        If chkgesetzmanagerModusLoeschenAendern.IsChecked Then
            btnHinzuRechtsdb.IsEnabled = False
            MsgBox("Klicken Sie das zu-ändernde-Dokument an")
        Else
            btnHinzuRechtsdb.Content = "Hinzufügen"
            btnHinzuRechtsdb.IsEnabled = True
        End If
        refreshGesetzdb(tbSuchSGnr.Text.Trim, False)
        e.Handled = True
    End Sub

    Private Sub btnaddSachgebiet_Click(sender As Object, e As RoutedEventArgs)

        Dim nnn As New win_sgtree(myGlobalz.Paradigma_Sachgebietsdatei, "einstellig")
        nnn.ShowDialog()
        If Not nnn.publicNR.IsNothingOrEmpty Then
            tbSuchSGnr.Text = nnn.publicNR
        End If

        'Dim newAZ As New AktenzeichenSachgebiet
        'newAZ.Zahl = nnn.publicNR
        'newAZ.Header = nnn.publicsgHeader
        'bestandsSachgebiete.Add(newAZ)
        'dgsachgebietZugeorndet.DataContext = bestandsSachgebiete
        'btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True

    End Sub

    Private Sub cmbHerkunft_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not ladevorgangAbgeschlossen Then Exit Sub
        'MsgBox("Baustelle")
        e.Handled = True
    End Sub

    Private Sub cmbArt_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not ladevorgangAbgeschlossen Then Exit Sub
        'MsgBox("baustelle")
        e.Handled = True
    End Sub

    Private Sub datepGueltig_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not ladevorgangAbgeschlossen Then Exit Sub
        'MsgBox("baustelle")
        e.Handled = True
    End Sub
    Private Sub refreshGesetzdb(sgzahl As String, GesetzListefirstTime As Boolean)
        Try
            dgrechtsgrundlagen.DataContext = Nothing
            Dim gesetzobejkte As New List(Of clsgesetzesManagerDok)
            Dim artid, herkunftid As Integer
            If cmbArt.SelectedValue Is Nothing Then
                artid = 0
            Else
                artid = CInt(cmbArt.SelectedValue)
            End If

            If cmbHerkunft.SelectedValue Is Nothing Then
                herkunftid = 0
            Else
                herkunftid = CInt(cmbHerkunft.SelectedValue)
            End If
            Dim sql As String
            sql = modGesetzSQL.compose(sgzahl, artid, herkunftid,
                                       chkistgueltig, datepGueltig.Text, tbSuchfilter.Text, GesetzListefirstTime)
            modrechtsdb.getrechtsDT(sql)
            modrechtsdb.getrechtsobjekte(gesetzobejkte)
            modrechtsdb.artUndHerkunftWandeln(gesetzobejkte, rrechtsdbARTcoll, rrechtsdbHerkunftcoll)

            If gesetzobejkte.Count > 0 Then
                dgrechtsgrundlagen.DataContext = gesetzobejkte
                tiRechtsdb.Header = "Rechtsgrundlagen " & gesetzobejkte.Count
            Else
                tiRechtsdb.Visibility = Visibility.Visible
            End If
        Catch ex As Exception
            nachricht("fehler in ", ex)
        End Try
    End Sub

    Private Sub btnGesetzsucheeinschalten_Click(sender As Object, e As RoutedEventArgs)
        spGesetzeSuchfunktion.Visibility = Visibility.Visible
        dgrechtsgrundlagen.DataContext = Nothing
        GesetzListefirstTime = False
        e.Handled = True
    End Sub

    Private Sub dgVorgangDokumente_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        MessageBox.Show("Bitte keine Doppelklicks bei Dokumenten verwenden.")
        'l("fehler Doppelklick " & Environment.UserName)
    End Sub

    'Private Sub chkBoxPan_Click(sender As Object, e As RoutedEventArgs)
    '    If Not formWindetailIsLoaded Then Exit Sub
    '    eigentuemerfunktionAusschalten()
    '    If chkBoxPan.IsChecked Then
    '        zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '        gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '        CanvasClickModus = "pan"
    '        Mouse.OverrideCursor = Cursors.Hand
    '    End If
    '    If Not chkBoxPan.IsChecked Then
    '        zeichneOverlaysGlob = True : zeichneImageMapGlob = True
    '        Mouse.OverrideCursor = Cursors.Arrow
    '        gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '        CanvasClickModus = ""
    '        Mouse.OverrideCursor = Cursors.Arrow
    '    End If

    '    e.Handled = True
    'End Sub


    ''' <summary>
    ''' Method to start the application on the secondary screen
    ''' import system.windows.forms
    ''' </summary>
    'Private Sub ShowOnSecondaryScreen()
    '    Dim secondary As System.Windows.Forms.Screen = Nothing

    '    ' check if there is a secondary screen
    '    If System.Windows.Forms.Screen.AllScreens.GetUpperBound(0) > 0 Then
    '        ' get the secondary screen
    '        secondary = System.Windows.Forms.Screen.AllScreens(1)
    '    End If

    '    'If secondary IsNot Nothing Then
    '    '    ' set the screen location as form location
    '    '    Location = secondary.Bounds.Location
    '    'End If

    '    '' maximize the window
    '    'Me.WindowState = Maximized
    'End Sub

    'Private Sub btnGisStartInfoSpeichern_Click(sender As Object, e As RoutedEventArgs)
    '    '
    '    Dim datei As String = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
    '                                     "Paradigma\gisstart.txt")
    '    Dim out As String
    '    If zweiterBildschirm.IsChecked Then
    '        out = "1" & Environment.NewLine
    '    Else
    '        out = "0" & Environment.NewLine
    '    End If

    '    If gisSollaufzweitemSchirmOeffnen.IsChecked Then
    '        out = out & "1" & Environment.NewLine
    '    Else
    '        out = out & "0" & Environment.NewLine
    '    End If
    '    If paradigmaStehtLinks.IsChecked Then
    '        out = out & "1" & Environment.NewLine
    '    Else
    '        out = out & "0" & Environment.NewLine
    '    End If
    '    My.Computer.FileSystem.WriteAllText(datei, out, False, System.Text.Encoding.UTF8)
    '    e.Handled = True
    'End Sub

    'Private Sub zweiterBildschirm_Click(sender As Object, e As RoutedEventArgs)
    '    If zweiterBildschirm.IsChecked Then
    '        paradigmaStehtLinks.IsEnabled = True
    '        gisSollaufzweitemSchirmOeffnen.IsEnabled = True
    '        CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "zweiterbildschirmvorhanden", "1")
    '    Else
    '        paradigmaStehtLinks.IsEnabled = False
    '        gisSollaufzweitemSchirmOeffnen.IsEnabled = False
    '        CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "zweiterbildschirmvorhanden", "0")
    '    End If
    'End Sub

    'Private Sub btnEigentuemer_click(sender As Object, e As RoutedEventArgs)

    'End Sub

    'Private Sub refreshRaumbezugsListe()
    '    resetRBliste_ClickExtracted()

    'End Sub

    Private Sub btnRaumbezugRefresh_Click(sender As Object, e As RoutedEventArgs)
        resetRBliste_ClickExtracted()
        e.Handled = True
    End Sub

    Private Sub dgVorgangFotos_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        Dim dep As DependencyObject = DirectCast(e.OriginalSource, DependencyObject)
        While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridCell)
            dep = VisualTreeHelper.GetParent(dep)
        End While
        If dep Is Nothing Then
            Return
        End If

        If TypeOf dep Is DataGridCell Then
            Dim cell As DataGridCell = TryCast(dep, DataGridCell)
            cell.Focus()

            While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridRow)
                dep = VisualTreeHelper.GetParent(dep)
            End While
            Dim row As DataGridRow = TryCast(dep, DataGridRow)
            fotosRitemousekeypressed = True
            dgVorgangFotos.SelectedItem = row.DataContext
        End If
    End Sub

    Private Sub dgVorgangDokumente_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim dep As DependencyObject = DirectCast(e.OriginalSource, DependencyObject)
        While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridCell)
            dep = VisualTreeHelper.GetParent(dep)
        End While
        If dep Is Nothing Then
            Return
        End If

        If TypeOf dep Is DataGridCell Then
            Dim cell As DataGridCell = TryCast(dep, DataGridCell)
            cell.Focus()

            While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridRow)
                dep = VisualTreeHelper.GetParent(dep)
            End While
            Dim row As DataGridRow = TryCast(dep, DataGridRow)
            dokumenteRitemousekeypressed = True
            dgVorgangDokumente.SelectedItem = row.DataContext
        End If

        e.Handled = True
    End Sub

    Private Sub paradigmaKILLALLGIS_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        If paradigmaKILLALLGIS.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "allegissekillen", "1")
        Else
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "allegissekillen", "0")
        End If

    End Sub



    'Private Sub gisSollaufzweitemSchirmOeffnen_Click(sender As Object, e As RoutedEventArgs)
    '    If Not formWindetailIsLoaded Then Exit Sub
    '    If gisSollaufzweitemSchirmOeffnen.IsChecked Then
    '        CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "gisaufzweitenbildschirm", "1")
    '    Else
    '        CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "gisaufzweitenbildschirm", "0")
    '    End If
    '    e.Handled = True
    'End Sub

    'Private Sub paradigmaStehtLinks_Click(sender As Object, e As RoutedEventArgs)
    '    If Not formWindetailIsLoaded Then Exit Sub
    '    If paradigmaStehtLinks.IsChecked Then
    '        CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "hauptbildschirmlinks", "1")
    '    Else
    '        CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "hauptbildschirmlinks", "0")
    '    End If
    '    e.Handled = True

    'End Sub

    Private Sub chkNoGISever_Checked(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If ladevorgangAbgeschlossen = False Then Return
        If Not chkNoGISever.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("NOMAP", "neverShowMinimap", "0")
        Else
            CLstart.myc.userIniProfile.WertSchreiben("NOMAP", "neverShowMinimap", "1")
        End If
    End Sub

    Private Sub chkGISimmerbeenden_Checked(sender As Object, e As RoutedEventArgs)
        If ladevorgangAbgeschlossen = False Then Return
        If chkGISimmerbeenden.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "immerbeenden", "1")
        Else
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "immerbeenden", "0")
        End If
        e.Handled = True
    End Sub

    Private Sub dgEreignisse_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        Dim dep As DependencyObject = DirectCast(e.OriginalSource, DependencyObject)
        While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridCell)
            dep = VisualTreeHelper.GetParent(dep)
        End While
        If dep Is Nothing Then
            Return
        End If
        If TypeOf dep Is DataGridCell Then
            Dim cell As DataGridCell = TryCast(dep, DataGridCell)
            cell.Focus()

            While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridRow)
                dep = VisualTreeHelper.GetParent(dep)
            End While
            Dim row As DataGridRow = TryCast(dep, DataGridRow)
            dokumenteRitemousekeypressed = True
            dgEreignisse.SelectedItem = row.DataContext
        End If
    End Sub

    Private Sub dgVorgangDokumente_SelectionChanged_1Extracted(aktThumbnailNotiz As String, allebilder As Boolean)
        l("dgVorgangDokumente_SelectionChanged_1Extracted")
        If detailsTools.istDateiNameInordnung(myGlobalz.sitzung.aktDokument.DateinameMitExtension) Then
            Dim darst As Boolean = detailsTools.Archiv_aktiviere_Dokument(dokumenteRitemousekeypressed, CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked),
                                                                          aktThumbnailNotiz, allebilder, myGlobalz.sitzung.aktDokument.EreignisID)
            dokumenteRitemousekeypressed = False
            detailsTools.darstellen(darst)
            detailsTools.FSW_instantiieren(darst, myGlobalz.PumuckelVersion)
        Else
            MsgBox(glob2.getMsgboxText("DateiNameIstNichtInOrdnung",
                                   New List(Of String)(New String() {myGlobalz.sitzung.aktDokument.DateinameMitExtension})
                                  ))
        End If
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshFotos(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub btnStartBplan_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        'Process.Start(initP.getValue("ExterneAnwendungen.Application_Eigentuemer"))
        CLstart.mycSimple.startbplankataster()
    End Sub

    Private Sub rbpumuckelversion_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        If rbpumuckelversion0.IsChecked Then
            myGlobalz.PumuckelVersion = 0
            CLstart.myc.userIniProfile.WertSchreiben("PUMUCKEL", "interop", "0")
        End If
        If rbpumuckelversion1.IsChecked Then
            myGlobalz.PumuckelVersion = 1
            CLstart.myc.userIniProfile.WertSchreiben("PUMUCKEL", "interop", "1")
        End If
        If rbpumuckelversion2.IsChecked Then
            myGlobalz.PumuckelVersion = 2
            CLstart.myc.userIniProfile.WertSchreiben("PUMUCKEL", "interop", "2")
            MessageBox.Show("Ab sofort sind managen Sie ihre geänderten Dokumente eigenverantwortlich!", "Herzlichen Glückwunsch", MessageBoxButton.OK)
        End If
    End Sub

    'Private Sub dgVorgangDokumente_MouseDoubleClick_1(sender As Object, e As MouseButtonEventArgs)

    'End Sub

    'Private Sub dgVorgangDokumente_MouseDoubleClick_2(sender As Object, e As MouseButtonEventArgs)

    'End Sub

    'Private Sub dgVorgangDokumente_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles dgVorgangDokumente.SizeChanged
    '    e.Handled = True

    'End Sub
    Private Sub Window_StateChanged(sender As Object, e As EventArgs)
        If Not ladevorgangAbgeschlossen Then Exit Sub
        Dim quotient As Double = 3 / 5
        Select Case WindowState
            Case WindowState.Maximized
                Width = (System.Windows.SystemParameters.PrimaryScreenWidth)
                Height = (System.Windows.SystemParameters.PrimaryScreenHeight)
                resizeWindow(quotient)
            Case WindowState.Minimized
            Case WindowState.Normal
                resizeWindow(quotient)
        End Select
        resizeWindow(quotient)
    End Sub

    Private Sub resizeWindow(quotient As Double)
        Debug.Print(Width & "," & Height)
        TabControl1.Width = (Me.Width * quotient)
        TabControl2.Width = (Me.Width * (1 - quotient))
        dgVorgangFotos.Width = (Me.Width * (1 - quotient)) - 50

    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        Dim quotient As Double = 3 / 5
        resizeWindow(quotient)
    End Sub

    Private Sub dgEreignisse_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        MessageBox.Show("Bitte keine Doppelklicks bei Ereignissen/Dokumenten verwenden.")
        'l("fehler Doppelklick " & Environment.UserName)
    End Sub

    Private Sub dgVorgangFotos_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        MessageBox.Show("Bitte keine Doppelklicks bei Dokumenten verwenden.")
        'l("fehler Doppelklick " & Environment.UserName)
    End Sub

    Private Sub BtnZuConject_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True

        Dim chromeFile As String = "CHROME.EXE" '"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
        Dim link As String = "https://ng.conject.com/ng"
        Process.Start(chromeFile, link)
    End Sub

    Private Sub BtnDownloaddir_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim dirr As String = IO.Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads")
        Process.Start(dirr)
    End Sub

    Private Sub MainListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        Dim auswahl As TextBlock
        auswahl = CType(sender, TextBlock)
    End Sub

    Private Sub txtitel_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        Dim auswahl As TextBlock
        auswahl = CType(sender, TextBlock)
        Dim userid As Integer = CInt(auswahl.Tag)
        Dim email As String
        email = userTools.getEmailFromColBearbeiter(myGlobalz.colBearbeiterFDU, userid)
        glob2.EmailFormOEffnen(email, "", "", "",
                                   myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, False)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub txtitel_MouseRightButtonDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        Dim auswahl As TextBlock
        auswahl = CType(sender, TextBlock)
    End Sub

    Private Sub BtnKurzeBAliste_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Process.Start("O:\UMWELT\B\GISDatenEkom\div\kontaktedoc\ba.docx")
    End Sub
    Private Sub ButtonBALANG_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Process.Start("O:\UMWELT\B\GISDatenEkom\div\kontaktedoc\ba1.docx")
    End Sub

    Private Sub Buttonkommunen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Process.Start("O:\UMWELT\B\GISDatenEkom\div\kontaktedoc\kommunen.docx")
    End Sub

    Private Sub ListboxBauaufsicht_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True

    End Sub

    Private Sub txtitelBA_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        Dim auswahl As TextBlock
        auswahl = CType(sender, TextBlock)
        Dim userid As Integer = CInt(auswahl.Tag)
        Dim email As String
        email = userTools.getEmailFromColBearbeiter(myGlobalz.colBearbeiterBA, userid)
        glob2.EmailFormOEffnen(email, "", "", "",
                                   myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, False)
        refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
        refreshEreignisseListe(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    'Private Sub TabItem_MouseDown(sender As Object, e As MouseButtonEventArgs)

    'End Sub

    Private Sub TabItemFDU_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        userTools.getOnlineStatus(myGlobalz.colBearbeiterFDU)
        userTools.MakeKapitelsKontakte(myGlobalz.colBearbeiterFDU)
        MainListBox.ItemsSource = myGlobalz.colBearbeiterFDU
    End Sub

    Private Sub CbIstConject_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        myGlobalz.sitzung.aktVorgang.istConjectVorgang = clsStammTools.getIstConjectVorgang(myGlobalz.sitzung.aktVorgangsID)
        If cbIstConject.IsChecked Then
            myGlobalz.sitzung.aktVorgang.istConjectVorgang = True
            clsStammTools.saveIstConject(myGlobalz.sitzung.aktVorgangsID, 1)
            zeigeIstConjectFarbe()
        Else
            myGlobalz.sitzung.aktVorgang.istConjectVorgang = False
            clsStammTools.saveIstConject(myGlobalz.sitzung.aktVorgangsID, 0)
            zeigeIstNichtConjectFarbe()
        End If
    End Sub

    Private Sub zeigeIstNichtConjectFarbe()
        grpStammdatenUebersicht.Background = Brushes.LightGray
        grpStammdatenUebersicht.ToolTip = ""
    End Sub

    Private Sub zeigeIstConjectFarbe()
        grpStammdatenUebersicht.Background = Brushes.NavajoWhite
        grpStammdatenUebersicht.ToolTip = "Wichtiger Hinweis: Dieser Vorgang wird auch in 'Bauantrag-Online' bearbeitet. Uploaden Sie die Stellungnahme auch dorthin."
    End Sub

    Private Sub Cbtimetool_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        If cbtimetool.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "timetoolaktiv", "1")
        Else
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "timetoolaktiv", "0")
        End If
    End Sub

    Private Sub ChkAlleBeteiligtenMarkieren_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not chkAlleBeteiligtenMarkieren.IsChecked Then
            presBeteiligtenAusgewaehltMarkieren(False, Psession.presBeteiligte)
        Else
            alteMarkierungen.Clear()
            For Each ele As Person In Psession.presBeteiligte
                presBeteiligtenAusgewaehltMarkieren(True, Psession.presBeteiligte)
            Next
        End If
    End Sub

    Private Sub BtnBeteiligteMehrfachLoeschen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        'If detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presDokus) > 0 Then
        If detail_dokuauswahl.anzahlAusgewaehlteBeteiligte(Psession.presBeteiligte) > 0 Then
            Dim icount As Integer
            icount = Beteiligte_MehrfachLoeschen()
            If icount > 0 Then
                MessageBox.Show("Es wurden " & icount & " von " & Psession.presBeteiligte.Count & " Beteiligte gelöscht.")
                refreshBeteiligteListe(myGlobalz.sitzung.aktVorgangsID)
            Else
                MessageBox.Show("Es wurden " & icount & " Beteiligte gelöscht.")
            End If
        Else
            MsgBox("Sie haben keine Auswahl getroffen.")
        End If
    End Sub

    Private Sub cbBeteiligteauswahlein(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        auswahlspalteBeteiligte.Visibility = Windows.Visibility.Visible
        stckBeteiligteMehrfachtools.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub cbBeteiligteauswahlaus(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        auswahlspalteBeteiligte.Visibility = Windows.Visibility.Collapsed
        stckBeteiligteMehrfachtools.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub BtnFremdRBs_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim frb As New winFremdRBs
        frb.ShowDialog()
        refreshRaumbezugsListe(myGlobalz.sitzung.aktVorgangsID, False, False)
        'MsgBox("men at work")
    End Sub

    Private Sub BtnWeblink_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        detailsTools.eEreignisstarten("weblink", CBool(cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked))
    End Sub

    Private Sub tbFilter_TextChanged(sender As Object, e As TextChangedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        Dim filter As String
        Dim neueListe As New List(Of clsPresDokumente)
        If tbFilter.Text.Length > 3 Then
            filter = tbFilter.Text.Trim
            For Each datei As clsPresDokumente In Psession.presDokus
                If (datei.DateinameMitExtension.Trim.ToLower.Contains(filter) Or datei.Beschreibung.Trim.ToLower.Contains(filter)) Then
                    neueListe.Add(datei)

                End If
            Next
            dgVorgangDokumente.DataContext = neueListe
        End If
    End Sub

    Private Sub CbreadOnlyDoxsInTxtCrtlOeffnen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        If cbreadOnlyDoxsInTxtCrtlOeffnen.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "readOnlyDoxsInTxtCrtlOeffnen", "1")
        Else
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "readOnlyDoxsInTxtCrtlOeffnen", "0")
        End If
    End Sub

    Private Sub VerwandteAdressen_hinzufuegen(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: verwandte adressen hinzu ")
        AdressTools.Verwandte_Adressen_hinzufuegen(myGlobalz.sitzung.aktVorgangsID)
        refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub Verwandte_loeschen(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: verwandte adressen löschen ")
        AdressTools.Verwandte_loeschen(myGlobalz.sitzung.aktVorgangsID)
        refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub Verwandte_FST_hinzufuegen(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: verwandte flurstuecke hinzu ")
        'AdressTools.Verwandte_Adressen_hinzufuegen(myGlobalz.sitzung.aktVorgangsID)
        AdressTools.Verwandte_FST_hinzufuegen(myGlobalz.sitzung.aktVorgangsID)
        refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub Verwandte_alleRB_hinzufuegen(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: verwandte adressen hinzu ")
        AdressTools.Verwandte_Adressen_hinzufuegen(myGlobalz.sitzung.aktVorgangsID)
        nachricht("USERAKTION: verwandte flurstuecke hinzu ")

        AdressTools.Verwandte_FST_hinzufuegen(myGlobalz.sitzung.aktVorgangsID)
        refreshverwandte(myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Sub btnChangeUserSetting_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim aendern As New winUserEdit
        aendern.ShowDialog()
        tbuserAbsteract.Text = myGlobalz.sitzung.aktBearbeiter.getString(Environment.NewLine)
    End Sub

    Private Sub btnMehrfachDrucken_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim sd As New winSchnelldruck
        sd.Show()


    End Sub

    Private Sub btnNitrat_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Process.Start("https://gis.uba.de/maps/resources/apps/nitratbericht_eu_richtlinie/index.html?lang=de/")
    End Sub

    Private Sub btnRROP_Click(sender As Object, e As RoutedEventArgs)

        e.Handled = True
        Process.Start("https://www.region-frankfurt.de/Services/Geoportal//")
    End Sub

    Private Sub cbDarkTheme_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        If cbDarkTheme.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "cbDarkTheme", "1")
        Else
            CLstart.myc.userIniProfile.WertSchreiben("GISSTART", "cbDarkTheme", "0")
        End If
    End Sub

    Private Sub btngooglestarten_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim goo As String = "https://maps.google.de?city=dietzenbach"
        Process.Start(goo)
    End Sub

    Private Sub cbzuhause_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Return
        If Not cbzuhause.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "zuhause", "0")
        Else
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "zuhause", "1")
        End If
    End Sub

    Private Sub btnVorlagenRoot_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Return
        CLstart.myc.userIniProfile.WertSchreiben("diverse", "Vorlagenroot", tbVorlagenRoot.Text.Trim)
        btnVorlagenRoot.IsEnabled = False
    End Sub

    Private Sub btnVorlagenWork_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Return
        CLstart.myc.userIniProfile.WertSchreiben("diverse", "Vorlagenwork", tbVorlagenWork.Text.Trim)
        btnVorlagenWork.IsEnabled = False
    End Sub

    Private Sub tbVorlagenWork_TextChanged(sender As Object, e As TextChangedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        btnVorlagenWork.IsEnabled = True
    End Sub

    Private Sub tbVorlagenRoot_TextChanged(sender As Object, e As TextChangedEventArgs)
        e.Handled = True
        If Not ladevorgangAbgeschlossen Then Exit Sub
        btnVorlagenRoot.IsEnabled = True
    End Sub

    'Private Sub btnMehrfachDokumenteDrucken_Click(sender As Object, e As RoutedEventArgs)
    '    e.Handled = True
    '    If (detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presFotos) + detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presDokus)) > 0 Then
    '        Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachMailen()
    '        MessageBox.Show("Es wurden " & icount & " Dokumente gemailt.")
    '        'MessageBox.Show("Es wurden " & icount & " von " & Sitzung.presDokus.Count & " Dokumenten gemailt.")
    '    Else
    '        MsgBox("Sie haben nichts ausgewählt.")
    '    End If
    'End Sub

    'Private Sub btnMehrfachDrucken_Click(sender As Object, e As RoutedEventArgs)
    '    e.Handled = True
    '    If (detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presFotos) + detail_dokuauswahl.anzahlAusgewaehlteDokumente(Psession.presDokus)) > 0 Then
    '        Dim icount As Integer
    '        icount = detail_dokuauswahl.Dokus_MehrfachDrucken(dlist, myGlobalz.sitzung.aktVorgangsID, zielvorgang, alteMarkierungen)
    '        MessageBox.Show("Es wurden " & icount & " Dokumente gemailt.")
    '        'MessageBox.Show("Es wurden " & icount & " von " & Sitzung.presDokus.Count & " Dokumenten gemailt.")
    '    Else
    '        MsgBox("Sie haben nichts ausgewählt.")
    '    End If
    'End Sub
End Class

