Imports System.Data
'Imports Paradigma_start1

Class Window1
    Property adminModus As Boolean = False
    'Property gisappsDir As String = INI_Databases.getXmlTagValue("GisServer.gisappsDir") '
    Private az As String, header As String
    Public Shared kookieFenster As Kookieliste

    Public alter_titel As String
    Public alter_probaugAz As String
    Public altergemKRZ As String
    Dim LastCell As New DataGridCell
    Dim logfile As String

    Private Sub Startroutine(ByVal userid As String)
        Try
            CLstart.CLstart.wintools2.umgebungstest()
            tbVersion.Text = String.Format("{0}{1}, Rev.: ",
                                           System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString,
                                           Environment.NewLine)
            CLstart.CLstart.DMStools.setArchivLaufWerkBuchstabe("\\netapp02\paradigma\test")

            If String.IsNullOrEmpty(userid) Then
                adminModus = True
                myGlobalz.sitzung.aktBearbeiter.GISPassword = ""
            End If

            CLstart.CLstart.MeinLogging.LoggingInit("Start", Environment.UserName, logfile)
            'Paradigma_start.Win1Tools.LoggingInit("Start", Environment.UserName)

            My.Log.WriteEntry("Startroutine: logstart :" & Now.ToString, 0)
            Dim localAppDataParadigmaDir As String = System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma"

            My.Log.WriteEntry("in Startroutine")
            Dim paradigmaXML As String = CLstart.CLstart.ZeigeraufXMLinitdatei.get() '
            Dim testfile As New IO.FileInfo(paradigmaXML)
            hatZugriffOderEnde(testfile)
            clstart.mycsimple.iniDict = CLstart.clsINIXML.XMLiniReader(xml_inifile_fullpath:=paradigmaXML) '"g:\appsconfig\paradigma.xml")

            Paradigma_start.Win1Tools.Myglobalz_init(localAppDataParadigmaDir:=localAppDataParadigmaDir)
            Paradigma_start.Win1Tools.defineAktuellenBearbeiter(userid:=userid)
            gastLayout()

            nachricht("clstart.mycsimple.iniDict hat Einträge: " & clstart.mycsimple.iniDict.Count)
            My.Log.WriteEntry("in Startroutine: vor INI_Databases ")
            initP.INI_All(localAppDataParadigmaDir:=localAppDataParadigmaDir)

            'clsStartup.userdaten_aus_lokalerdateilesen(adminModus)

            If String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.Rang) Then
                'dies is de  first access to the database
                Dim DatabaseIsOk As Boolean = Paradigma_start.Win1Tools.initAktuellenBearbeiter()
                If Not DatabaseIsOk Then
                    nachricht_und_Mbox("Datenbank ist nicht erreichbar! " & Environment.NewLine &
                            "Programm wird beendet: " & myGlobalz.sitzung.aktBearbeiter.username)
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
            glob2.genmailXML()
            If clsStartupBatch.GenCopyConfBatch() Then
                tbTextparadigma.Foreground = Brushes.Black
            Else
                tbTextparadigma.Foreground = Brushes.Blue
            End If
            My.Log.WriteEntry("in Startroutine: vor init_archiv ")
            Paradigma_start.Win1Tools.init_archiv()
            Dim akuteWV% = clsWVTOOLS.getWiedervorlageAkut(myGlobalz.sitzung.aktBearbeiter.Initiale)
            If akuteWV > 0 Then
                WVbuttonHntergrundAufRot()
            End If

            My.Log.WriteEntry("in Startroutine: vor catch ")
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                btnAdmin.Visibility = Windows.Visibility.Visible
                btnStake.Visibility = Windows.Visibility.Visible
                btnZahlungen.Visibility = Windows.Visibility.Visible
                stckPanle.Height = 590
                Me.Height = 590
                dockP.Height = 590
            End If
            Paradigma_start.Win1Tools.UserinifileAnlegen()
            If Application.givenVID > 1 Then
                glob2.editVorgang(CInt(Application.givenVID))
            End If
            clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel,
                                      alter_probaugAz,
                                      altergemKRZ)
            tbzuVorgang.Text = clsStartup.vorgangsvorschlag
            btnZurNr.ToolTip = "Zuletzt bearbeitet: " & vbCrLf & header & vbCrLf & "(" & az & ")"

            CLstart.CLstart.DMStools.buildDMSexchangePath()
            CLstart.CLstart.DMStools.initDMStools(myGlobalz.sitzung.aktBearbeiter.username,
                                               "\\netapp02\paradigma\test", myGlobalz.sitzung.aktVorgangsID)
            CLstart.CLstart.DMStools.prepareExchangeDir()
            If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "feinen_j" Then CLstart.clsPlattenplatz.clsPlattenplatz("j:")
            clsVorgangLocking.alleLocksDesUsersLoesen()
            '
            'direktes oeffnen
                glob2.editVorgang(CInt(tbzuVorgang.Text))
                clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)

        Catch ex As Exception
            nachricht_und_Mbox("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & " " & ex.ToString)
        End Try
    End Sub

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

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        'MsgBox("sdjsadioas")
        Startroutine("")
        e.Handled = True
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
        KillKookieFenster()
        If myGlobalz.BestandsFensterIstgeoeffnet Then
            MsgBox("Es ist bereits eine Bestandsübersicht geöffnet. Sie können Sie direkt über die Taskbar von Windows aufrufen.", MsgBoxStyle.Information, "Zur Bestandsliste")
            Exit Sub
        End If
        clsStartup.FormularBestandStammdaten(False, False)
        e.Handled = True
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
            If Application.zweiteInstanz Then
                Startroutine("gast")
            Else
                Startroutine(Uid)
            End If
        End If
    End Sub

    Private Sub btnZahlungen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnZahlungen.Click
        KillKookieFenster()
        Dim zahlungsliste As New WINzahlungenListe
        zahlungsliste.ShowDialog()
        e.Handled = True
    End Sub


    Private Sub Window1_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        CLstart.CLstart.MeinLogging.LoggingEnde(logfile, myGlobalz.LOGFILEKOPIE)
        'Paradigma_start.Win1Tools.LoggingEnde(logfile)
        Try
            clsVorgangLocking.alleLocksDesUsersLoesen()
            If clstart.myc.aLog IsNot Nothing Then
                clstart.myc.aLog.endlog()
            End If

            '  clstart.myc.aLog.copyToServer(myGlobalz.ActionLogDir)
        Catch ex As Exception
            nachricht("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & ": ", ex)
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
                glob2.editVorgang(CInt(tbzuVorgang.Text))
                clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)
            End If
        End If
        e.Handled = True
    End Sub

    Private Sub btnNeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KillKookieFenster()
        clsStartup.NeuerVorgang2()
        e.Handled = True
    End Sub

    'Private Sub btntest222_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Dim neustamm As New Stamm
    '    Dim nnn As New winAZdefineNEU("edit", neustamm)
    '    nnn.ShowDialog()
    'End Sub

    Private Sub tblastvorgangsid_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If String.IsNullOrEmpty(CStr(tblastvorgangsid.Content)) Then Exit Sub
        glob2.editVorgang(CInt(tblastvorgangsid.Content))
    End Sub




    Private Sub btnProjekte_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KillKookieFenster()
        Projekte_liste_anzeigen()

        e.Handled = True
    End Sub

    Private Sub Projekte_liste_anzeigen()
        Dim aktprojekt As New clstart.clsProjektAllgemein(0)
        If NSprojekt.ProjektAusgewaehlt.exe(aktprojekt) Then
            Dim prj As New WinProjekt("edit", aktprojekt)
            prj.ShowDialog()
            aktprojekt.clear()
            ' Projekt_RefreshDetailEintrag(aktprojekt)
        End If
    End Sub


    Private Sub stake(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Process.Start(initP.getValue("ExterneAnwendungen.Application_Stakeholder"))
        e.Handled = True
    End Sub

    Private Sub Handbuch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KillKookieFenster()
        glob2.OpenDocument(initP.getValue("Myglobalz.paradigmaHandbuchFullName"))
        e.Handled = True
    End Sub

    Private Sub btnEigentuemer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KillKookieFenster()
        Process.Start(initP.getValue("ExterneAnwendungen.Application_Eigentuemer"))
        e.Handled = True
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

    Private Sub gastLayout()
        If glob2.userIstinGastModus Then
            Background = New SolidColorBrush(Colors.Red)
            dockP.Background = New SolidColorBrush(Colors.Red)
            Left = 200
        End If
    End Sub

    Private Sub btnBplaeneClick(sender As Object, e As RoutedEventArgs)
        KillKookieFenster()
        Dim aufruf As String = initP.getValue("GisServer.URLbplankataster") ' 
        Try
            Process.Start(aufruf)
        Catch ex As Exception
            nachricht("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & ": ", ex)
        End Try
        e.Handled = True
    End Sub

    Private Sub tbzuVorgang_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbzuVorgang.TextChanged
        btnmeineinArbeit.IsDefault = False
        btnZurNr.IsDefault = True

        e.Handled = True
    End Sub

    Private Sub btnZurNr_MouseEnter(sender As Object, e As MouseEventArgs)
        If kookieFenster Is Nothing Then
            kookieFenster = New Kookieliste
            kookieFenster.Show()
        End If
        e.Handled = True
    End Sub



    Private Sub tbzuVorgang_MouseEnter(sender As Object, e As MouseEventArgs)
        e.Handled = True
    End Sub



    Private Sub btnVorlagenClick(sender As Object, e As RoutedEventArgs)
        KillKookieFenster()
        myGlobalz.sitzung.aktVorgangsID = 0
        Dim ttrtrt As New WinVorlagenListe("", "", False)
        ttrtrt.ShowDialog()
        e.Handled = True
    End Sub


End Class


