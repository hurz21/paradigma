Class Window1
    Public _shapeModul As clsKartenerstellungShape
    Private Paradigma_root$
    Public Property Datei_einzelvorgang As String
    Public Property Datei_vorgangsliste As String
    Public Property modus$
    Private logfile$
    Public Property sgnummer() As String
    Public Property verbose As Boolean
    Public Property mitgis As Boolean
    Public Property vid As Integer
    Public Property paradigmaXML As String
    Property GISusername As string


    Private Sub starteEinzelKarte(ByVal Mitverwandten As Boolean, ByVal modus As String, ByVal mitetikett As Boolean,GISusername As string)
        KarteErstellen(CInt(vid), sgnummer$, Mitverwandten, modus, mitetikett,GISusername)
    End Sub

    Sub lesekonfig()
        Dim paradigmaXML$ = "O:\UMWELT-PARADIGMA\gis\GIS\div\backup\archiv\1\0\1\paradigma.xml"
        Dim testfile As New IO.FileInfo(paradigmaXML)
        If Not testfile.Exists Then
            ' paradigmaXML$ = "d:\paradigma.xml"
            MsgBox("Die Konfiguration konnte nicht gefunden werden")
            End
        End If
        myGlobalz.iniDict = clsINIXML.XMLiniReader(paradigmaXML$) '"g:\appsconfig\paradigma.xml")
    End Sub
    Private Sub startRoutine()
        LoggingInit()
        glob2.nachricht(String.Format("--------------------PARADIGMAKARTE -----------start: {0}{1}", Date.Today, TimeOfDay))
        Paradigma_root$ = Environment.GetFolderPath((System.Environment.SpecialFolder.DesktopDirectory)) & "\Paradigma"
        Datei_einzelvorgang = String.Format("{0}\aktvorgang.txt", Paradigma_root)
        Datei_vorgangsliste = String.Format("{0}\aktvorgangsListe.txt", Paradigma_root)
        modus = holeModus(Datei_einzelvorgang, Datei_vorgangsliste)
        GISusername=Environment.UserName
        Dim sgnummer$ = ""

        Select Case modus
            Case "einzeln"
                chkMitverwandten.IsEnabled = False
                einzelModusInit()
                tbvorgangsid.Text = vid.ToString
            Case "liste"
                chkMitverwandten.IsEnabled = False
                tbvorgangsid.Text = myGlobalz.Bearbeiter.ToString
                tbSGNummer.Text = "Liste von Vorgängen"
                listenModusINit()
            Case "auswahl"
                auswahlmodusInit()
            Case Else
                auswahlmodusInit()
        End Select

    End Sub

    Private Sub auswahlmodusInit()
        glob2.nachricht("auswahlmodusInit ---------------- " & Datei_einzelvorgang$)
        ' Throw New System.NotImplementedException()
    End Sub

    Private Sub listenModusINit()
        glob2.nachricht("listenModusINit ---------------- " & Datei_einzelvorgang$)
        '   starteUmwandlung()
    End Sub

    Sub einzelModusInit()
        Datei_einzelvorgang_parameterEinlesen(Datei_einzelvorgang, logfile, vid, sgnummer, verbose, mitgis)
        glob2.nachricht("einzelModusInit ---------------- " & Datei_einzelvorgang)
    End Sub


    Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        startRoutine()
        verbose = true
        If Not verbose Then
            starteUmwandlung(GISusername)
            End
        End If
        e.Handled = True
    End Sub


    Sub KarteErstellen(ByVal vorgangsid As Integer,
                       ByVal sgnummer As String,
                       ByVal Mitverwandten As Boolean,
                       ByVal modus As String,
                       ByVal mitetikett As Boolean,
                       GISusername As string)
        My.Log.WriteEntry("in Startroutine")
        Dim paradigmaXML As String = My.Resources.Resources.ParadigmaKonfigFile
        Dim testfile As New IO.FileInfo(paradigmaXML)
        If Not testfile.Exists Then
            MsgBox("Die Konfiguration konnte nicht gefunden werden")
            End
        End If
        myGlobalz.iniDict = clsINIXML.XMLiniReader(paradigmaXML) '"g:\appsconfig\paradigma.xml")

        inidatabase.iniall()
        Dim sql, titel As String
        Dim ebenen(0) As Integer
        _shapeModul = New clsKartenerstellungShape()
        Select Case modus
            Case "einzeln"
                getAlleVorgaengeFuerEinzelVorgang(Mitverwandten, ebenen, vid)
                _shapeModul.appendix = CStr(myGlobalz.Bearbeiter) 'CStr(vid) '& "_" & myGlobalz.Bearbeiter '& "_" & clsString.normalize(Now.ToString)
            Case "liste"
                getAlleVorgaengeFuerListe(Datei_vorgangsliste, ebenen, vid, sql, titel)
                _shapeModul.appendix = CStr(myGlobalz.Bearbeiter) '& "_" & myGlobalz.Bearbeiter '& "_" & clsString.normalize(Now.ToString)
        End Select


        tbResult.Text & = ebenen.Count & " Vorgänge werden bearbeitet!   Bitte warten!"& Environment.NewLine



        _shapeModul.KartenMapfileTemplate = myGlobalz.gis_serverD & "/paradigmacache/vorlagen/raumbezugSHP.map"
        _shapeModul.KartenHTMfileTemplate = myGlobalz.gis_serverD & "/paradigmacache/vorlagen/raumbezug.htm"
        'shapeModul.KartenRoot$ = myGlobalz.gis_serverD$ & "\fkat\temp"
        _shapeModul.KartenRoot = myGlobalz.gis_serverD & "\paradigmacache"

        'shapeModul.KartenJPGDir$ = myGlobalz.gis_serverD$ & "\cache\gis\"

        _shapeModul.KartenJPGDir = myGlobalz.gis_serverD & "\paradigmacache\"

        _shapeModul.Kartenprojektdir = _shapeModul.KartenRoot & "\" & _shapeModul.appendix
        clsKartenerstellungShape.kartenDatadir = _shapeModul.Kartenprojektdir & "\data"
        _shapeModul.KartenVorgangsDir = _shapeModul.KartenRoot & "\VORGANG\"
        'KartenJPGDir$ = KartenVorgangsDir$ & CStr(vid) & "\Files"
        _shapeModul.KartenIMGDir = _shapeModul.Kartenprojektdir & "\images"
        _shapeModul.KartenMAPfile = _shapeModul.Kartenprojektdir & "\raumbezug.map"

        Verzeichnisse_anlegen(_shapeModul)

        tbResult.Text &= exekuteEinzelVorgang(_shapeModul, _shapeModul.aktBox, ebenen) & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0)


        
        tbResult.Text &= "Die Shape-Dateien liegen unter: " & clsKartenerstellungShape.kartenDatadir & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0)



        MsgBox("Die Shape-Dateien liegen unter: " & clsKartenerstellungShape.kartenDatadir  )
        glob2.nachricht("WebGis - Preparieren")
             tbResult.Text &= "WebGis - Preparieren"  & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0)


        Dim KartenEbenenName As String = _shapeModul.webgisPreparieren(mitetikett,GISusername)
        glob2.nachricht("KartenEbenenName$: " & KartenEbenenName)
        'aufruf des gis
               tbResult.Text &= "Webgis starten" & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0)
        Dim hinweis$ = "Kartenmodul wird beendet. WebGIS wird gestartet!"
        If mitgis Or modus = "liste" Then glob2.mittelpunktsAufruf(_shapeModul.aktBox, KartenEbenenName, sgnummer$, myGlobalz.GIS_WebServer$)

                       tbResult.Text &= "Fertig" & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0)
    End Sub

    Private Sub starteUmwandlung(GISusername As string)
        If modus = "einzel" Then
            vid = CInt(tbvorgangsid.Text)
        End If
        tbResult.Text &= "Der folgende Vorgang kann einige Minuten dauern!   Bitte warten!"& Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0)
        starteEinzelKarte(If(chkMitverwandten.IsChecked, True, False), modus, If(chkMitEtikett.IsChecked, True, False),GISusername)
        'Me.Close()
    End Sub
    Private Sub btnStart_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'MessageBox.Show("Der folgende Vorgang kann einige Minuten dauern!" & vbCrLf & "Bitte warten!", "Paradigma-Kartenmodul", MessageBoxButton.OK)
        starteUmwandlung(GISusername)
        e.Handled = True
    End Sub

    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
        End
    End Sub

    Private Sub showlogfile_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Process.Start(My.Log.DefaultFileLogWriter.FullLogFileName)
        ' Process.Start(Environment.GetFolderPath(CType(My.Log.DefaultFileLogWriter.Location, Environment.SpecialFolder)))
        e.Handled = True
    End Sub
End Class
