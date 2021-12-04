Module modstartup
    'Property adminModus As Boolean = False
    Private az As String, header As String
    'Public kookieFenster As Kookieliste

    Public alter_titel As String
    Public alter_probaugAz As String
    Public altergemKRZ As String
    Dim LastCell As New DataGridCell
    Public logfile As String

    Private Sub getvidFromCommandLine(ByRef userid As String)
        If Application.givenVID > 1 Then
            myglobalz.sitzung.aktVorgangsID = CInt(Application.givenVID)
            myglobalz.sitzung.aktEreignis.ID = CInt(Application.givenEID)
            clsStartup.vorgangsvorschlag = CStr(Application.givenVID)
            Debug.Print(Application.givenDOCID.ToString)
        Else
            clsStartup.vorgangsvorschlag = CStr(CLstart.VIDuebergabe.parameterEinlesen(userid, CLstart.mycSimple.Paradigma_local_root, "aktvorgang2"))
        End If
    End Sub

    Public Sub LoggingInit(modulname As String, username As String, ByRef qfile As String)
        Try
            My.Log.DefaultFileLogWriter.CustomLocation = "c:\kreisoffenbach\prjfotogugger"
            With My.Log.DefaultFileLogWriter
                .BaseFileName = "Paradigma_" & modulname & "_" & username
                .AutoFlush = False
                .Append = False
            End With
            qfile = My.Log.DefaultFileLogWriter.FullLogFileName
            My.Log.WriteEntry("My.Log.DefaultFileLogWriter: " & My.Log.DefaultFileLogWriter.FullLogFileName)
            My.Log.WriteEntry("startlog: " & Now.ToString)

        Catch ex As Exception
            nachricht("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & ": ", ex)
        End Try
    End Sub
    Public Function Startroutine() As String
        Try
            CLstart.mycSimple.ParadigmaVersion = My.Resources.BuildDate.Trim.Replace(vbCrLf, "")
            Dim userid As String = ""
            CLstart.wintools2.umgebungstest()
            ' MsgBox(Application.givenVID)

            'clsStartup.vorherigerVorgang = CType(CLstart.VIDuebergabe.parameterEinlesen(userid, CLstart.mycSimple.Paradigma_local_root, "aktvorgang2"), String)
            'CLstart.DMStools.setArchivLaufWerkBuchstabe("\\file-paradigma\paradigma\test")

            LoggingInit("detail", Environment.UserName, logfile)
            My.Log.WriteEntry("Startroutine: logstart :" & Now.ToString, 0)
            l("CLstart.mycSimple.ParadigmaVersion: " & CLstart.mycSimple.ParadigmaVersion)


            'Dim localAppDataParadigmaDir As String = System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma"
            Dim localAppDataParadigmaDir As String = "C:\kreisoffenbach\common"

            My.Log.WriteEntry("in Startroutine")
            'Dim paradigmaXML As String = CLstart.ZeigeraufXMLinitdatei.get(System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma\config\zeiger.txt",
            '                             CLstart.DMStools._DMSLaufWerkBuchstabe & "\paradigmaArchiv\div\xml\paradigma_sqlsO.xml") '
            Dim paradigmaXML As String = CLstart.ZeigeraufXMLinitdatei.get("c:\kreisoffenbach\common\Paradigma\zeiger.txt",
                                         "\\file-paradigma\paradigma\test" & "\paradigmaArchiv\div\xml\paradigma_sqlsO.xml") '
#If DEBUG Then
            'paradigmaXML = CLstart.DMStools._DMSLaufWerkBuchstabe & "\paradigmaArchiv\div\xml\paradigma_PG.xml"
#End If
            hatZugriffOderEnde(paradigmaXML)

            CLstart.mycSimple.iniDict = CLstart.clsINIXML.XMLiniReader(xml_inifile_fullpath:=paradigmaXML)
#If DEBUG Then
            '# einlesen aus dem formular von frau klotz
            Dim bla As New Dictionary(Of String, String)
            bla = CLstart.clsINIXML.XMLiniReader2("C:\Users\feinen_j\Desktop\formualr\klingler.xml")
#End If

            Paradigma_start.Win1Tools.Myglobalz_init(localAppDataParadigmaDir:=localAppDataParadigmaDir)
            clsStartup.vorherigerVorgang = CType(CLstart.VIDuebergabe.parameterEinlesen(userid, CLstart.mycSimple.Paradigma_local_root, "vorherigerVorgang"), String)


            Paradigma_start.Win1Tools.defineAktuellenBearbeiter(userid:=userid)
            ' gastLayout()
            nachricht("clstart.mycsimple.iniDict hat Einträge: " & CLstart.mycSimple.iniDict.Count)
            nachricht("in Startroutine: vor INI_Databases ")
            initP.INI_All(localAppDataParadigmaDir:=localAppDataParadigmaDir)

            'clsStartup.userdaten_aus_lokalerdateilesen(adminModus)

            If String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.Rang) Then
                'dies is de  first access to the database
                Dim DatabaseIsOk As Boolean = Paradigma_start.Win1Tools.initAktuellenBearbeiter()
                If Not DatabaseIsOk Then
                    nachricht_und_Mbox("Datenbank ist nicht erreichbar! für username: " & Environment.UserName & Environment.NewLine &
                            "Programm wird beendet: " & myGlobalz.sitzung.aktBearbeiter.username)
                    End
                Else
                    nachricht("Datenbank ist erreichbar")
                End If
            End If
            Debug.Print(myGlobalz.sitzung.aktBearbeiter.ID.ToString)
            My.Log.WriteEntry("in Startroutine: vor displayAktuellenBearbeiter ")
            'displayAktuellenBearbeiter()
            'If String.IsNullOrEmpty(myglobalz.sitzung.aktBearbeiter.GISPassword) Then
            '    Paradigma_start.Win1Tools.initgisuser(GISuser:=myglobalz.sitzung.aktBearbeiter.username)
            'End If

            'clsStartup.userdaten_in_lokalerdateischreiben(adminModus)

            My.Log.WriteEntry("in Startroutine: vor genmailXML ")
            'glob2.genmailXML()
            If clsStartupBatch.GenCopyConfBatch() Then

            Else

            End If
            My.Log.WriteEntry("in Startroutine: vor init_archiv ")
            Paradigma_start.Win1Tools.init_archiv()

            CLstart.clsINIDatei.UserinifileAnlegen(myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT,
                                                   initP.getValue("Haupt.UserInis"), myGlobalz.sitzung.aktBearbeiter.Initiale)



            getvidFromCommandLine(userid)

            'CLstart.DMStools.buildDMSexchangePath()
            'CLstart.DMStools.initDMStools(myGlobalz.sitzung.aktBearbeiter.username,
            '                                   "\\file-paradigma\paradigma\test", myGlobalz.sitzung.aktVorgangsID)
            'CLstart.DMStools.prepareExchangeDir()
            Dim gb As String = CLstart.clsPlattenplatz.clsPlattenplatz("j:")
            myGlobalz.sitzung.aktVorgangsID = CInt(clsStartup.vorgangsvorschlag)
            clsStartup.vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)
            Return gb
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & " ", ex)
        End Try
    End Function
    Public Sub hatZugriffOderEnde(ByVal paradigmaXML As String)
        Dim testfile As New IO.FileInfo(paradigmaXML)
        If Not testfile.Exists Then
            MessageBox.Show("Die Paradigmaresource (\\file-) ist nicht Ihrem Zugriff. " & Environment.NewLine &
                            "Bitte " & Environment.NewLine &
                            "1. Rechner neu starten oder " & Environment.NewLine &
                            "2. die IT-Abteilung informieren!",
                            "",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error)
            End
        End If
        testfile = Nothing
    End Sub
End Module

