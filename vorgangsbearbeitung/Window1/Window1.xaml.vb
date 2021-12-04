Imports System.Data
'Imports Paradigma_start1

Class Window1
    Private Property timetoolaktiv As Boolean = False
    Property ladevorgangabgeschlossen As Boolean = False
    Property adminModus As Boolean = False
    Public Property curContentMousePoint As Point
    'Property gisappsDir As String = INI_Databases.getXmlTagValue("GisServer.gisappsDir") '
    Private az As String, header As String
    Public Shared kookieFenster As Kookieliste

    Public alter_titel As String
    Public alter_probaugAz As String
    Public altergemKRZ As String
    Dim LastCell As New DataGridCell
    Dim logfile As String
    Public startModus As String

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        e.Handled = True
        'IO.Directory.SetCurrentDirectory("C:\ptest\main")
        IO.Directory.CreateDirectory("C:\kreisoffenbach\main")
        IO.Directory.SetCurrentDirectory("C:\kreisoffenbach\main")
        If Environment.UserName = "zahnlückenpimpf" Or Environment.UserName.ToLower = "feinen" Or Environment.UserName.ToLower = "nhi" Then
            myGlobalz.zuhause = True
        End If
        Left = 1 : Top = 100
        Startroutine("")
        myGlobalz.nurEinBildschirm = read_nurEinBildschirm()
        If Not myGlobalz.nurEinBildschirm Then
            Me.Top = CLstart.formposition.getPosition("diverse", "winHauptformpositiontop", Me.Top)
            Me.Left = CLstart.formposition.getPosition("diverse", "winHauptformpositionleft", Me.Left)

        End If
        Me.Width = CLstart.formposition.getPosition("diverse", "winHauptformpositionwidth", Me.Width)
        Me.Height = CLstart.formposition.getPosition("diverse", "winHauptformpositionheight", Me.Height)
        'Me.Width = CLstart.formposition.setPosition("diverse", "winHauptformpositionwidth", Me.Width)
        'Me.Height = CLstart.formposition.setPosition("diverse", "winHauptformpositionheight", Me.Height)
        'If startModus = "neu" Then
        '    KillKookieFenster()
        '    clsStartup.NeuerVorgang2()
        'End If
        'initimetool()
        'If timetoolaktiv Then clsArbeitszeit.GesamtTages = clsArbeitszeit.fromFile(Now)
        'If timetoolaktiv Then refreshArbeitszeitSaldo()
        'If timetoolaktiv Then clsArbeitszeit.justStarted(DateTime.Now)
        'MsgBox(Width.ToString)
        If istInParadigmaTabelleAktiv() Then
            'alles ok
        Else
            MsgBox("Die Bearbeiterin >" & myGlobalz.sitzung.aktBearbeiter.username & "< ist nicht Teil des Umweltamtes " & Environment.NewLine &
                ". Daher hat er/sie keine vollen Rechte. " & Environment.NewLine &
                "Wenn gewünscht bitte Email an IT, mit der Bitte um Zuweisung der Person an den FD Umwelt im ActiveDirectory.")
            Close()
        End If
        'userImUmweltamtLautAD()
        ladevorgangabgeschlossen = True
        FocusManager.SetFocusedElement(Me, tbzuVorgang)
    End Sub



    Private Shared Function istInParadigmaTabelleAktiv() As Boolean
        Return (Not String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.Rang)) And myGlobalz.sitzung.aktBearbeiter.Status = 1
    End Function

    Private Sub userImUmweltamtLautAD()
        Dim lokdt As New DataTable
        'Dim testuser As String = "Mueller_b"
        'Dim testuser As String = Environment.UserName '"paul_j"
        Dim testuser As String = myGlobalz.sitzung.aktBearbeiter.username


        'Dim loklist As New List(Of clsTupelString)
        Dim realDepartment As String = ""
        Dim filter As String = testuser ' Environment.UserName
        If Environment.UserName = "Mueller_b" Then
            Debug.Print("")
        End If
        filter = LIBgemeinsames.clsString.umlaut2ue(filter)
        lokdt = JFactiveDirectory.clsActiveDir.sucheperson(filter)
        If lokdt Is Nothing OrElse lokdt.Rows.Count < 1 Then
            'dgPersonal.DataContext = Nothing
        Else
            Dim test As Integer = JFactiveDirectory.umweltamt.istImUmweltamt(lokdt, realDepartment)
            Select Case test
                Case -1
                    MsgBox("Die Bearbeiterin >" & filter & "< ist nicht Teil des Umweltamtes " & Environment.NewLine &
                           "(z.Z. im FD: " & realDepartment.Trim & Environment.NewLine &
                           "). Daher hat er/sie keine vollen Rechte. " & Environment.NewLine &
                           "Wenn gewünscht bitte Email an IT, mit der Bitte um Zuweisung der Person an den FD Umwelt im ActiveDirectory.")
                Case 0
                    MsgBox("Die Bearbeiterin >" & filter & "< ist nicht Teil des Umweltamtes " & Environment.NewLine &
                           "(z.Z. im FD: " & realDepartment.Trim & Environment.NewLine &
                           "). Daher hat er/sie keine vollen Rechte. " & Environment.NewLine &
                           "Wenn gewünscht bitte Email an IT, mit der Bitte um Zuweisung der Person an den FD Umwelt im ActiveDirectory.")
                Case 1
                    'alles ok
            End Select
            'dgPersonal.DataContext = lokdt
        End If
    End Sub

    'Private Sub initimetool()
    '    If CLstart.myc.userIniProfile.WertLesen("GISSTART", "timetoolaktiv") = "1" Then
    '        timetoolaktiv = True
    '    Else
    '        timetoolaktiv = False
    '        spArbeitszeit.Visibility = Visibility.Collapsed
    '    End If
    'End Sub

    'Private Sub refreshArbeitszeitSaldo()
    '    Dim azminder As String = ""
    '    'clsArbeitszeit.fromFile(Now)
    '    tbArbeitszeitSaldo2.Text = clsArbeitszeit.calcAktArbeitszeit("30", azminder)
    '    tbAZminder.Text = azminder
    '    tbArbeitszeitSaldo2.Text = clsArbeitszeit.calcAktArbeitszeit("45", azminder)
    '    tbAZminder90.Text = azminder
    'End Sub
    Friend Sub mapAllArguments(arguments() As String)
        Try
            'l("mapAllArguments---------------------- anfang")
            For Each sttelement In arguments
                If sttelement.Contains("modus=neu") Then
                    l("modus=paradigma also gesetzt")
                    startModus = "neu"
                End If
            Next
            'l("mapAllArguments---------------------- ende")
        Catch ex As Exception
            l("Fehler in mapAllArguments: " & ex.ToString())
        End Try
        'modus=probaug suchmodus=flurstueck gemarkung=dietzenbach flur=5 fstueck=490/0"
    End Sub
    Private Sub Startroutine(ByVal userid As String)
        Try
            'If Environment.UserName.ToLower = "petersdorff_l" Then
            '    MsgBox("version 1114")
            'End If
            Dim arguments As String() = Environment.GetCommandLineArgs()
            mapAllArguments(arguments)
            CLstart.wintools2.umgebungstest()
            tbVersion.Text = String.Format("{0}{1}, Rev.: ",
                                           System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString,
                                           Environment.NewLine)
            'CLstart.DMStools.setArchivLaufWerkBuchstabe("\\file-paradigma\paradigma\test")

            If String.IsNullOrEmpty(userid) Then
                adminModus = True
                myGlobalz.sitzung.aktBearbeiter.GISPassword = ""
            End If

            CLstart.MeinLogging.LoggingInit("Start", Environment.UserName, logfile)
            'Paradigma_start.Win1Tools.LoggingInit("Start", Environment.UserName)

            l("Startroutine: logstart :" & Now.ToString)
            'Dim localAppDataParadigmaDir As String = System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma"
            Dim localAppDataParadigmaDir As String = "C:\kreisoffenbach\common"

            l("in Startroutine")
            'Dim paradigmaXML As String = CLstart.ZeigeraufXMLinitdatei.get() '

            'Dim paradigmaXML As String = CLstart.ZeigeraufXMLinitdatei.get(System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma\config\zeiger.txt",
            '                             CLstart.DMStools._DMSLaufWerkBuchstabe & "\paradigmaArchiv\div\xml\paradigma_sqlsO.xml") '


            Dim paradigmaXML As String = CLstart.ZeigeraufXMLinitdatei.get("c:\kreisoffenbach\common\Paradigma\zeiger.txt",
                                         "\\file-paradigma\paradigma\test" & "\paradigmaArchiv\div\xml\paradigma_sqlsO.xml") '

            Dim testfile As New IO.FileInfo(paradigmaXML)
            hatZugriffOderEnde(testfile)
            CLstart.mycSimple.iniDict = CLstart.clsINIXML.XMLiniReader(xml_inifile_fullpath:=paradigmaXML) '"g:\appsconfig\paradigma.xml")

            Paradigma_start.Win1Tools.Myglobalz_init(localAppDataParadigmaDir:=localAppDataParadigmaDir)
            kfatools.KFAeingangsDir = initP.getValue("Haupt.KFAroot")
            Title = clsStartup2.getTitle("") : nachricht("winBestandStammdaten_Loaded d")
#If DEBUG Then
            'userid = "Roetzel_L"
            'userid = "el-amraoui_n"
#Else
            userid = Environment.UserName

#End If

            Paradigma_start.Win1Tools.defineAktuellenBearbeiter(userid:=userid)

#If DEBUG Then
            'myGlobalz.sitzung.aktBearbeiter.username = "Nehler_u"
#End If

            'gastLayout()

            nachricht("clstart.mycsimple.iniDict hat Einträge: " & CLstart.mycSimple.iniDict.Count)
            My.Log.WriteEntry("in Startroutine: vor INI_Databases ")
            initP.INI_All(localAppDataParadigmaDir:=localAppDataParadigmaDir)

            'clsStartup.userdaten_aus_lokalerdateilesen(adminModus)

            If String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.Rang) Then
                'dies is de  first access to the database
                Dim DatabaseIsOk As Boolean = Paradigma_start.Win1Tools.initAktuellenBearbeiter()
                If Not DatabaseIsOk Then
                    nachricht_und_Mbox("Datenbank ist nicht erreichbar für username: " & Environment.UserName & Environment.NewLine &
                            "Programm wird beendet: " & myGlobalz.sitzung.aktBearbeiter.username & Environment.NewLine & Environment.NewLine & Environment.NewLine &
                            "Falls Sie Teil des FD Umwelt sind wenden Sie sich bitte an das Vorzimmer " & Environment.NewLine &
                            "zwecks Registierung/Freischaltung von Paradigma!")
                    End
                Else
                    nachricht("Datenbank ist erreichbar")
                End If
            End If

            My.Log.WriteEntry("in Startroutine: vor displayAktuellenBearbeiter ")
            displayAktuellenBearbeiter()
            If String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.GISPassword) Then
                Paradigma_start.Win1Tools.initgisuser(GISuser:=myGlobalz.sitzung.aktBearbeiter.username)
            End If

            'clsStartup.userdaten_in_lokalerdateischreiben(adminModus)

            My.Log.WriteEntry("in Startroutine: vor genmailXML ")
            'glob2.genmailXML()
            If clsStartupBatch.GenCopyConfBatch() Then
                tbTextparadigma.Foreground = Brushes.Black
            Else
                tbTextparadigma.Foreground = Brushes.Blue
            End If
            My.Log.WriteEntry("in Startroutine: vor init_archiv ")
            Paradigma_start.Win1Tools.init_archiv()
            Dim akuteWV% = clsWVTOOLS.getWiedervorlageAkut(myGlobalz.sitzung.aktBearbeiter.Initiale)

            My.Log.WriteEntry("in Startroutine: vor catch ")
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                btnAdmin.Visibility = Windows.Visibility.Visible
                btnStake.Visibility = Windows.Visibility.Visible

                btnZahlungen.Visibility = Windows.Visibility.Visible
                stckPanle.Height = 790
                Me.Height = 590
                dockP.Height = 590
            Else
                kfasAdmin1.Visibility = Visibility.Collapsed
                btnExplorer.Visibility = Visibility.Collapsed
            End If
            'If myGlobalz.zuhause Then
            '    kfasAdmin1.Visibility = Visibility.Collapsed
            '    btnExplorer.Visibility = Visibility.Collapsed
            'End If
            'Paradigma_start.Win1Tools.UserinifileAnlegen()
            'myc.userIniProfile = New clsINIDatei(IO.Path.Combine(initP.getValue("Haupt.UserInis"), myGlobalz.sitzung.aktBearbeiter.Initiale & ".ini"))

            CLstart.clsINIDatei.UserinifileAnlegen(myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT,
                                                   IO.Path.Combine(initP.getValue("Haupt.UserInis")),
                                                                   myGlobalz.sitzung.aktBearbeiter.Initiale)
            If akuteWV > 0 Then
                WVbuttonHntergrundAufRot()
                wiedervorlagepoppen()
            End If
            If Application.givenVID > 1 Then
                glob2.editVorgang(CInt(Application.givenVID), myGlobalz.testmode)
            End If
            clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel,
                                      alter_probaugAz,
                                      altergemKRZ)
            tbzuVorgang.Text = clsStartup.vorgangsvorschlag
            btnZurNr.ToolTip = "Zuletzt bearbeitet: " & vbCrLf & header & vbCrLf & "(" & az & ")"

            'CLstart.DMStools.buildDMSexchangePath()
            'CLstart.DMStools.initDMStools(myGlobalz.sitzung.aktBearbeiter.username,
            '                                   "\\file-paradigma\paradigma\test", myGlobalz.sitzung.aktVorgangsID)
            'CLstart.DMStools.prepareExchangeDir()
            If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "feinen_j" Then CLstart.clsPlattenplatz.clsPlattenplatz("j:")
            clsVorgangLocking.alleLocksDesUsersLoesen()
            If myGlobalz.testmode Then
                'stckPanle.Background = Brushes.DarkBlue
                'Background = Brushes.DarkBlue
                'btnStartbis.Background = Brushes.DarkBlue

            End If

            ' timereinrichten()
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & " " & ex.ToString)
        End Try
    End Sub
    'private sub dispatcherTimer_Tick()

    '// Updating the Label which displays the current second
    ''lblSeconds.Content = DateTime.Now.Second;

    '// Forcing the CommandManager to raise the RequerySuggested event
    ''CommandManager.InvalidateRequerySuggested()
    '    end sub

    Private Shared Sub hatZugriffOderEnde(ByVal testfile As IO.FileInfo)
        If Not testfile.Exists Then
            MessageBox.Show("Die Paradigmaresource ist nicht Ihrem Zugriff. " & Environment.NewLine &
                            "Bitte " & Environment.NewLine &
                            "1. Rechner neu starten oder " & Environment.NewLine &
                            "2. die IT-Abteilung informieren!",
                            "",
                             MessageBoxButton.OK,
                              MessageBoxImage.Error)
            End
        End If
    End Sub
    'Private Sub setzeLastVorgangsLinkLabelText()
    '    Dim vorgangsvorschlag$ = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)
    '    If Not String.IsNullOrEmpty(vorgangsvorschlag) Then
    '        tblastvorgangsid.Content = vorgangsvorschlag
    '    End If
    'End Sub

    Private Sub Window1_IsStylusCaptureWithinChanged(sender As Object, e As DependencyPropertyChangedEventArgs) Handles Me.IsStylusCaptureWithinChanged

    End Sub



    Sub WVbuttonHntergrundAufRot()
        btnWiedervorlage.Background = New SolidColorBrush(Colors.Red)
    End Sub

    Sub displayAktuellenBearbeiter()
        aktBearbeiter.Text = myGlobalz.sitzung.aktBearbeiter.Initiale
    End Sub

    'Private Shared Sub initVorgangNeuForm()
    '    myGlobalz.h_WINvorgangNeu2 = New WINvorgangNeu2
    '    myGlobalz.h_WINvorgangNeu2.Hide()
    'End Sub

    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnWiedervorlage.Click
        KillKookieFenster()
        clsStartup.FormularBestandWvFilter(False)
        e.Handled = True
    End Sub

    Private Sub btnBestand_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        nachricht("BESTAND")
        KillKookieFenster()
        '  Dim procExists as Boolean = Process.GetProcesses().Any(Function(p) p.ProcessName.Contains("Bestand-"))
        If myGlobalz.BestandsFensterIstgeoeffnet Then
            ' MsgBox("Es ist bereits eine Bestandsübersicht geöffnet. Sie können Sie direkt über die Taskbar von Windows aufrufen.", MsgBoxStyle.Information, "Zur Bestandsliste")
            Exit Sub
        End If
        nachricht("BESTAND2")
        clsStartup.FormularBestandStammdaten(False, False)

        'refreshArbeitszeitSaldo()


    End Sub

    Private Sub aktBearbeiter_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles aktBearbeiter.MouseDown
        nachricht_und_Mbox(String.Format("Ihre Daten als Bearbeiter:{0}{1}", vbCrLf, Paradigma_start.Win1Tools.BildeBearbeiterProfilalsString(myGlobalz.sitzung.aktBearbeiter)))
        e.Handled = True
    End Sub


    Private Sub btnAdmin_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAdmin.Click
        KillKookieFenster()
        initAdmin()
        e.Handled = True
    End Sub

    Sub initAdmin()
        Dim windia As New WINAdmin
        windia.ShowDialog()
        Uid$ = myGlobalz.sitzung.aktBearbeiter.username
        If Not String.IsNullOrEmpty(Uid) Then
            'If Application.zweiteInstanz Then
            '    Startroutine("gast")
            'Else
            Startroutine(Uid)
            'End If
        End If
    End Sub

    Private Sub btnZahlungen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnZahlungen.Click
        KillKookieFenster()
        Dim zahlungsliste As New WINzahlungenListe
        zahlungsliste.ShowDialog()
        e.Handled = True
    End Sub


    Private Sub Window1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        CLstart.MeinLogging.LoggingEnde(logfile, myGlobalz.LOGFILEKOPIE)
        'Paradigma_start.Win1Tools.LoggingEnde(logfile)
        Try
            clsVorgangLocking.alleLocksDesUsersLoesen()
            If CLstart.myc.aLog IsNot Nothing Then
                CLstart.myc.aLog.endlog()
            End If
            savePosition()
            'If timetoolaktiv Then clsArbeitszeit.saveShutdown(Now)
            '  clstart.myc.aLog.copyToServer(myGlobalz.ActionLogDir)
        Catch ex As Exception
            nachricht("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & ": ", ex)
        End Try
    End Sub
    Private Sub savePosition()
        If myGlobalz.nurEinBildschirm Then Exit Sub
        Try
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winHauptformpositiontop", CType(Me.Top, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winHauptformpositionleft", CType(Me.Left, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winHauptformpositionwidth", CType(Me.Width, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winHauptformpositionheight", CType(Me.Height, String))
        Catch ex As Exception
            l("fehler in saveposition  windb" & ex.ToString)
        End Try
    End Sub
    Private Sub showLogFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        glob2.OpenDocument(My.Log.DefaultFileLogWriter.FullLogFileName)
    End Sub

    Private Sub showCopyrite_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        MessageBox.Show("Paradigma (c) 2009 Dr. Jörg Feinen", "Copyright", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
    End Sub

    Private Sub btnKarte_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Dim stringf$ = ";raumbez;bauantraege;"
        Dim gis As New clsGISfunctions
        gis.allevorgaengeimKreis()
    End Sub

    Private Sub Konfigurieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If clsStartupBatch.batchfileExecute() Then
            tbTextparadigma.Foreground = Brushes.Black
        Else
            tbTextparadigma.Foreground = Brushes.Blue
        End If
        ' loescheClientUserConfigFile()
    End Sub
    'Public Shared Sub loescheClientUserConfigFile()
    '    Dim testfile = New IO.FileInfo(myGlobalz.ClientUserConfigFile)
    '    If testfile.Exists Then testfile.Delete()
    'End Sub

    Private Shared Sub KillKookieFenster()
        If kookieFenster IsNot Nothing Then
            kookieFenster.Close()
            kookieFenster = Nothing
        End If
    End Sub

    Private Sub btnZurNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If Not String.IsNullOrEmpty(tbzuVorgang.Text) Then
            If IsNumeric(tbzuVorgang.Text) Then
                KillKookieFenster()
                LocalParameterFiles.erzeugeParameterDatei(False, False)
                glob2.editVorgang(CInt(tbzuVorgang.Text), True)
                clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)
            End If
        End If
        e.Handled = True
    End Sub

    Private Sub btnNeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KillKookieFenster()
        e.Handled = True
        CLstart.mycSimple.neuerVorgang3("modus=normal")
        'Process.Start(initP.getValue("ExterneAnwendungen.Application_Stakeholder"))

        'clsStartup.neuerVorgang3()
        'clsStartup.NeuerVorgang2()
        e.Handled = True
    End Sub



    'Private Sub btntest222_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Dim neustamm As New Stamm
    '    Dim nnn As New winAZdefineNEU("edit", neustamm)
    '    nnn.ShowDialog()
    'End Sub

    Private Sub tblastvorgangsid_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If String.IsNullOrEmpty(CStr(tblastvorgangsid.Content)) Then Exit Sub
        glob2.editVorgang(CInt(tblastvorgangsid.Content), myGlobalz.testmode)
    End Sub




    Private Sub btnProjekte_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KillKookieFenster()
        Projekte_liste_anzeigen()

        e.Handled = True
    End Sub

    Private Sub Projekte_liste_anzeigen()
        Dim aktprojekt As New CLstart.clsProjektAllgemein(0)
        If NSprojekt.ProjektAusgewaehlt.exe(aktprojekt) Then
            Dim prj As New WinProjekt("edit", aktprojekt)
            prj.ShowDialog()
            aktprojekt.clear()
            ' Projekt_RefreshDetailEintrag(aktprojekt)
        End If
    End Sub


    Private Sub stake(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        'Process.Start(initP.getValue("ExterneAnwendungen.Application_Stakeholder"))

        Dim si As New ProcessStartInfo
        si.FileName = initP.getValue("ExterneAnwendungen.Application_Stakeholder")
        si.WorkingDirectory = "O:\UMWELT\B\GISDatenEkom\div\deploy\paradigma\steakholder"
        si.Arguments = ""
        Process.Start(si)
        si = Nothing

    End Sub

    Private Sub Handbuch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KillKookieFenster()
        glob2.OpenDocument(initP.getValue("Myglobalz.paradigmaHandbuchFullName"))
        e.Handled = True
    End Sub

    Private Sub btnEigentuemer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        KillKookieFenster()
        'Process.Start(initP.getValue("ExterneAnwendungen.Application_Eigentuemer"))
        CLstart.mycSimple.startbplankataster()

    End Sub


    'Private Sub DataGrid_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs)
    '    'Casting the OriginalSource as a DependencyObject
    '    Dim dep As DependencyObject = DirectCast(e.OriginalSource, DependencyObject)

    '    'Stepping through the visual tree
    '    While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridCell)
    '        dep = VisualTreeHelper.GetParent(dep)
    '    End While
    '    If dep Is Nothing Or Not (TypeOf dep Is DataGridCell) Then
    '        'The cursor is outside Window1
    '        'Set the last cell to white
    '        If LastCell IsNot Nothing Then
    '            LastCell.Background = Brushes.White
    '        End If

    '        LastCell = CType(Nothing, DataGridCell)
    '        Exit Sub
    '    Else
    '        'The visualtreehelper found the e.OriginalSource as a DataGridCell
    '        Dim cell As DataGridCell = TryCast(dep, DataGridCell)
    '        'Test: Is the last Cell different from the current cell?
    '        If cell IsNot LastCell Then
    '            'Current cell Background
    '            cell.Background = Brushes.Tomato
    '            'Reset last cell Background
    '            If LastCell IsNot Nothing Then
    '                LastCell.Background = Brushes.White
    '            End If
    '            'Need to cast it. Can't type
    '            ' LastCell = Cell
    '            LastCell = CType(cell, DataGridCell)
    '        End If
    '    End If

    'End Sub

    Private Sub zeigeVersion(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Try
            'MsgBox("v " & My.Application.Info.Version.ToString() & vbCrLf & "args: " &
            '       Application.activationData(0) & vbCrLf & "l: " & Application.activationData.Length)
            '  MsgBox(Application.Current.Properties.Version.ToString())
        Catch ex As Exception
            nachricht("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & ": ", ex)
        End Try
        e.Handled = True
    End Sub

    'Private Sub gastLayout()
    '    If glob2.userIstinGastModus Then
    '        Background = New SolidColorBrush(Colors.Red)
    '        dockP.Background = New SolidColorBrush(Colors.Red)
    '        Left = 200
    '    End If
    'End Sub

    'Private Sub btnBplaeneClick(sender As Object, e As RoutedEventArgs)
    '    KillKookieFenster()
    '    Dim aufruf As String = initP.getValue("GisServer.URLbplankataster") ' 
    '    Try
    '        Process.Start(aufruf)
    '    Catch ex As Exception
    '        nachricht("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & ": ", ex)
    '    End Try
    '    e.Handled = True
    'End Sub



    Private Sub btnZurNr_MouseEnter(sender As Object, e As MouseEventArgs)
        Dim breiteKookiefenster = 849
        If kookieFenster Is Nothing Then
            'Dim leftneu = berechneLeft(CInt(Left), breiteKookiefenster)
            'kookieFenster = New Kookieliste(leftneu, CInt(Top))
            kookieFenster = New Kookieliste()
            kookieFenster.Show()
        End If
        e.Handled = True
    End Sub

    'Private Function berechneLeft(v As Integer, width As Double) As Integer
    '    Dim res As Integer
    '    Try
    '        l(" MOD ---------------------- anfang")
    '        If v + width > System.Windows.SystemParameters.PrimaryScreenWidth Then
    '            res = CInt(Left - width )
    '        Else
    '            res = CInt(Left + 128)
    '        End If
    '        l(" MOD ---------------------- ende")
    '        Return res
    '    Catch ex As Exception
    '        l("Fehler in MOD: " & ex.ToString())
    '        Return v
    '    End Try
    'End Function

    Private Sub tbzuVorgang_MouseEnter(sender As Object, e As MouseEventArgs)
        'FocusManager.SetFocusedElement(Me, btnZurNr)
        e.Handled = True
    End Sub
    Private Sub wiedervorlagepoppen()
        Try
            If CBool(CLstart.myc.userIniProfile.WertLesen("Boot", "wiedervorlagenpoppen")) Then
                KillKookieFenster()
                clsStartup.FormularBestandWvFilter(False)
            Else
                Exit Sub
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub NeuGis_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        KillKookieFenster()
        Dim si As New ProcessStartInfo
        si.FileName = initP.getValue("GisServer.mgis")
        Process.Start(si)
        si = Nothing
    End Sub

    Private Sub btnRefreshArbeitszeit_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim datei = "c:\kreisoffenbach\arbeitszeit\arbeitszeit.exe"
        Process.Start(datei)
        'refreshArbeitszeitSaldo()
    End Sub

    Private Sub TbzuVorgang_TextChanged(sender As Object, e As TextChangedEventArgs)
        e.Handled = True
        btnmeineinArbeit.IsDefault = False
        btnZurNr.IsDefault = True

        e.Handled = True
    End Sub

    Private Sub MenuItem_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        'If e.ChangedButton = MouseButton.Left Then
        '    curContentMousePoint = e.GetPosition()
        '    DragMove()
        'End If
    End Sub

    Private Sub MenuItem_MouseMove(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub btnOnlineforms_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim dg As New winKfa2(initP.getValue("Haupt.KFAroot"))
        dg.ShowDialog()
#If DEBUG Then

#End If
    End Sub

    Private Sub cbNurEinBildschirm_Checked(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        If cbNurEinBildschirm.IsChecked Then
            myGlobalz.nurEinBildschirm = True
            merkdateiAnlegen(True)
        Else
            myGlobalz.nurEinBildschirm = False
            merkdateiAnlegen(False)
        End If
    End Sub

    Private Sub merkdateiAnlegen(v As Boolean)
        Dim datei = "c:\kreisoffenbach\nureinbildschrm.txt"
        Try
            IO.File.WriteAllText(datei, cbNurEinBildschirm.ToString)
        Catch ex As Exception
            l("Fehler: ", ex)
        End Try
    End Sub

    Private Sub cbNurEinBildschirm_Unchecked(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        If cbNurEinBildschirm.IsChecked Then
            myGlobalz.nurEinBildschirm = True
            merkdateiAnlegen(True)
        Else
            myGlobalz.nurEinBildschirm = False
            merkdateiAnlegen(False)
        End If
    End Sub

    'Private Sub btnHeic2jpg_Click(sender As Object, e As RoutedEventArgs)
    '    KillKookieFenster()
    '    Dim si As New ProcessStartInfo
    '    'si.FileName = initP.getValue("GisServer.mgis")
    '    si.FileName = "c:\kreisoffenbach\heic\heic2jpg.exe"
    '    Process.Start(si)
    '    si = Nothing
    'End Sub

    Private Sub btnrefresh_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel,
                                         alter_probaugAz,
                                         altergemKRZ)
        tbzuVorgang.Text = clsStartup.vorgangsvorschlag
    End Sub



    Private Function read_nurEinBildschirm() As Boolean
        Dim datei = "c:\kreisoffenbach\nureinbildschrm.txt"
        Dim summe As String = ""
        Try
            Dim fi As New IO.FileInfo(datei)
            If fi.Exists Then
                summe = IO.File.ReadAllText(datei)
                If summe.ToLower.Contains("true") Then
                    cbNurEinBildschirm.IsChecked = True
                    Return True
                Else
                    cbNurEinBildschirm.IsChecked = False
                    Return False
                End If
            End If
        Catch ex As Exception
            l("Fehler: ", ex)
            Return False
        End Try
    End Function
    Private Sub TextBlock_Drop(sender As Object, e As DragEventArgs)
        e.Handled = True
        Dim www As New kfatools
        www.handleDrop(e)
        www = Nothing
    End Sub
    Private Sub btnExplorer_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Process.Start(kfatools.KFAeingangsDir)
    End Sub
End Class


