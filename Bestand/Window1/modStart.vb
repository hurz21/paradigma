Module modStart
        Property adminModus As Boolean = False
    'Property gisappsDir As String = INI_Databases.getXmlTagValue("GisServer.gisappsDir") '
    Private az As String, header As String
    Public   kookieFenster As Kookieliste

    Public alter_titel As String
    Public alter_probaugAz As String
    Public altergemKRZ As String
    Dim LastCell As New DataGridCell
    Dim logfile As String
        Public Sub Main(ByVal userid As String)
        Try
            CLstart.CLstart.wintools2.umgebungstest()
         
            CLstart.CLstart.DMStools.setArchivLaufWerkBuchstabe("\\netapp02\paradigma\test")

            If String.IsNullOrEmpty(userid) Then
                adminModus = True
                myGlobalz.sitzung.aktBearbeiter.GISPassword = ""
            End If

            CLstart.CLstart.MeinLogging.LoggingInit("Bestand", Environment.UserName, logfile)
            'Paradigma_start.Win1Tools.LoggingInit("Start", Environment.UserName)

            My.Log.WriteEntry("Startroutine: logstart Bestand:" & Now.ToString, 0)
            Dim localAppDataParadigmaDir As String = System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma"

            My.Log.WriteEntry("in Startroutine")
            Dim paradigmaXML As String = CLstart.CLstart.ZeigeraufXMLinitdatei.get() '
            Dim testfile As New IO.FileInfo(paradigmaXML)
            hatZugriffOderEnde(testfile)
            CLstart.mycSimple.iniDict = CLstart.clsINIXML.XMLiniReader(xml_inifile_fullpath:=paradigmaXML) '"g:\appsconfig\paradigma.xml")

            Paradigma_start.Win1Tools.Myglobalz_init(localAppDataParadigmaDir:=localAppDataParadigmaDir)
            Paradigma_start.Win1Tools.defineAktuellenBearbeiter(userid:=userid)
          

            nachricht("clstart.mycsimple.iniDict hat Einträge: " & CLstart.mycSimple.iniDict.Count)
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
          
            If String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.GISPassword) Then
                Paradigma_start.Win1Tools.initgisuser(GISuser:=myGlobalz.sitzung.aktBearbeiter.username)
            End If

            'clsStartup.userdaten_in_lokalerdateischreiben(adminModus)

            My.Log.WriteEntry("in Startroutine: vor genmailXML ")
            glob2.genmailXML()
            If clsStartupBatch.GenCopyConfBatch() Then
                'tbTextparadigma.Foreground = Brushes.Black
            Else
                'tbTextparadigma.Foreground = Brushes.Blue
            End If
            My.Log.WriteEntry("in Startroutine: vor init_archiv ")
            Paradigma_start.Win1Tools.init_archiv()
            'Dim akuteWV% = clsWVTOOLS.getWiedervorlageAkut(myGlobalz.sitzung.aktBearbeiter.Initiale)


            My.Log.WriteEntry("in Startroutine: vor catch ")
            'If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
            '    btnAdmin.Visibility = Windows.Visibility.Visible
            '    btnStake.Visibility = Windows.Visibility.Visible
            '    btnZahlungen.Visibility = Windows.Visibility.Visible
            '    stckPanle.Height = 590
            '    Me.Height = 590
            '    dockP.Height = 590
            'End If
            Paradigma_start.Win1Tools.UserinifileAnlegen()
            'If akuteWV > 0 Then
            '    WVbuttonHntergrundAufRot()
            '    wiedervorlagepoppen()
            'End If
            'If Application.givenVID > 1 Then
            '    glob2.editVorgang(CInt(Application.givenVID))
            'End If
            clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel,
                                      alter_probaugAz,
                                      altergemKRZ)
            'tbzuVorgang.Text = clsStartup.vorgangsvorschlag
            'btnZurNr.ToolTip = "Zuletzt bearbeitet: " & vbCrLf & header & vbCrLf & "(" & az & ")"

            CLstart.CLstart.DMStools.buildDMSexchangePath()
            CLstart.CLstart.DMStools.initDMStools(myGlobalz.sitzung.aktBearbeiter.username,
                                               "\\netapp02\paradigma\test", myGlobalz.sitzung.aktVorgangsID)
            CLstart.CLstart.DMStools.prepareExchangeDir()
            If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "feinen_j" Then CLstart.clsPlattenplatz.clsPlattenplatz("j:")
            clsVorgangLocking.alleLocksDesUsersLoesen()


                    clsStartup.FormularBestandStammdaten(False, False)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & " " & ex.ToString)
        End Try
    End Sub
        Private   Sub hatZugriffOderEnde(ByVal testfile As IO.FileInfo)
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
End Module
