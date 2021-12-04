Imports System.Data
Imports System.Windows.Forms
Imports pgis

Imports CefSharp
Imports CefSharp.Wpf
'https://bing.com/maps/default.aspx?cp=50.0175765535362~8.78544383388318&lvl=110&style=h

'https://bing.com/maps/default.aspx?cp=50.0275765535362~8.78544383388318&lvl=15&style=o&dir=180

'https://docs.microsoft.com/en-us/bingmaps/articles/create-a-custom-map-url

Partial Class MainWindow
    Public wb3D As CefSharp.Wpf.ChromiumWebBrowser

    Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'initdb()
        'Dim sets As New CefSharp.Wpf.CefSettings
        'sets.UserAgent = " Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)"
        'CefSharp.Cef.Initialize(sets)

        clsTools.iminternet = clsTools.getIminternet()
        If clsTools.iminternet Then
            clsTools.webserver = "https://buergergis.kreis-offenbach.de"
        End If
        fehlerlog = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\pgislog\" & "log.txt"
        IO.Directory.CreateDirectory(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\pgislog\")
        setDefKoord(bbox)
        'CefSharpSettings.LegacyJavascriptBindingEnabled = True
        ProxyString = getproxystring()
        googleVogel()

        wb3D = New CefSharp.Wpf.ChromiumWebBrowser
        tiGOogle3D.Children.Add(wb3D)
        AddHandler wb3D.IsBrowserInitializedChanged, AddressOf OnIsBrowserInitializedChanged3D

        'wbgooglenormal = New CefSharp.Wpf.ChromiumWebBrowser
        'dpgooglenormal.Children.Add(wbgooglenormal)
        'AddHandler wbgooglenormal.IsBrowserInitializedChanged, AddressOf OnIsBrowserInitializedChangedGooglenormal

        initGemeindeCombo()
        initGemarkungsCombo()
        l("starte")
        tbrechts.Text = rechts
        tbhoch.Text = hoch
        'Protokollausgabe_aller_Zugriff("nein")
        If clsTools.iminternet Then
            gbEigentuemer.Visibility = Visibility.Visible
        Else
            If NSfstmysql.ADtools.istUserAlbBerechtigt(Environment.UserName, fdkurz) Then
                gbEigentuemer.Visibility = Visibility.Visible
            Else
                gbEigentuemer.Visibility = Visibility.Collapsed
            End If
        End If

    End Sub

    'Private Sub OnIsBrowserInitializedChangedGooglenormal(sender As Object, e As DependencyPropertyChangedEventArgs)
    '    Dim result = starteWebbrowserControl(CInt(rechts), CInt(hoch), 100, False)
    '    If result.StartsWith("http") Or result.Contains("\") Then
    '        wbgooglenormal.Load((result))
    '    Else
    '        wbgooglenormal.Load((result))
    '    End If
    'End Sub

    Private Sub OnIsBrowserInitializedChanged3D(sender As Object, e As DependencyPropertyChangedEventArgs)

        Dim result = starteWebbrowserControl(CInt(rechts), CInt(hoch) - 200, 100, False)
        If result.StartsWith("http") Or result.Contains("\") Then
            wb3D.Load((result))
        Else
            wb3D.Load((result))
            'wbgooglenormal.Navigate(New Uri(result))
        End If
    End Sub
    Private Sub googleVogel()
        'Dim result As String = starteWebbrowserControl(CInt(rechts), CInt(hoch) - 300, 100, False)
        Dim result = starteWebbrowserControl(CInt(rechts), CInt(hoch), 100, True)
        If result.StartsWith("http") Or result.Contains("\") Then
            wbVogel.Load(result)
            'wbgooglenormal.Navigate(New Uri(result))
        Else
            wbVogel.Load((result))
            'wbgooglenormal.NavigateToString((result))
        End If
    End Sub

    Private Shared Sub setDefKoord(bbox As clsRange)
        'kreishaus
        rechts = "484629"
        hoch = "5540607"
    End Sub


    Private Function starteWebbrowserControl(rechts As Integer, hoch As Integer, radius As Integer, htmlformat As Boolean) As String
        Dim zbox As New clsRange
        calcBbox(CType(rechts, String), CType(hoch, String), zbox, 100)
        Return macheGooglebild(zbox, htmlformat)
    End Function

    Function macheGooglebild(zbox As clsRange, htmlformat As Boolean) As String
        Try
            nachricht("USERAKTION: googlekarte  vogel")
            Dim gis As New clsGISfunctions
            Dim result As String
            result = gis.GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(zbox, htmlformat, longitude, latitude)
            If result = "fehler" Or result = "" Then
            Else
                Return result
                'If result.StartsWith("http") Then
                '    '   wbnormalgoogle.Navigate(New Uri(result))
                'Else
                '    '   wbnormalgoogle.NavigateToString((result))
                'End If
            End If
            gis = Nothing
        Catch ex As Exception
            nachricht("fehler in starteWebbrowserControl1: " & ex.ToString)
            Return ""
        End Try
    End Function
    'Sub macheGooglebild2(zbox As clsRange)
    '    Try
    '        nachricht("USERAKTION: googlekarte  vogel")
    '        Dim gis As New clsGISfunctions
    '        Dim result As String
    '        result = gis.GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(zbox, True, longitude, latitude)
    '        If result = "fehler" Or result = "" Then
    '        Else
    '            '  gis.starten(result)
    '            '  GMtemplates.templateStarten(result)
    '            If result.StartsWith("http") Or result.Contains("\") Then
    '                wbVogel.Navigate(New Uri(result))
    '            Else
    '                wbVogel.NavigateToString((result))
    '            End If
    '        End If
    '            gis = Nothing
    '    Catch ex As Exception
    '        nachricht("fehler in starteWebbrowserControl1: " & ex.ToString)
    '    End Try
    'End Sub

    Private Sub btnaktualisiernvogel_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub btngoogle3d_Click()
        Try
            nachricht("USERAKTION: googlekarte  vogel btn click")
            Dim gis As New clsGISfunctions
            Dim result As String
            Dim nbox As New clsRange
            ' calcBbox(rechts, hoch, nbox, 900)
            Dim radius = 300
            nbox.xl = CInt(rechts) - radius
            nbox.yl = CInt(hoch) - (radius * 2)
            nbox.xh = CInt(rechts) + radius
            nbox.yh = CInt(hoch)
            result = gis.GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(nbox, False, longitude, latitude)
            If result = "fehler" Or result = "" Then
            Else
                Process.Start("iexplore", result)
            End If
            gis = Nothing
            'Protokollausgabe_aller_Zugriff("ja")
        Catch ex As Exception
            nachricht("fehler in starteWebbrowserControl: " & ex.ToString)
        End Try
    End Sub
    Private Sub cmbgemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbgemeinde.SelectedItem Is Nothing Then Exit Sub
        gemeindebigNRstring = CStr(cmbgemeinde.SelectedValue)
        Dim myvalx = CType(cmbgemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        tbGemeinde.Text = myvals
        gemeindestring = myvals
        'aktADR.gemeindeNr = CInt(CStr(myvali))
        'aktADR.gemeindeName = tbGemeinde.Text
        adrREC.mydb.Host = "w2gis02"
        initStrassenCombo()
        cmbStrasse.DataContext = adrREC.dt
        cmbStrasse.IsDropDownOpen = True
        e.Handled = True
    End Sub
    Sub initGemeindeCombo()
        Dim path = IO.Directory.GetCurrentDirectory
        Dim testfi As New IO.FileInfo(path & "\xml\" & gemeinde_verz)
        If Not testfi.Exists Then
            'MessageBox.Show("Die Gemarkungsliste konnte nicht gefunden werden! " & Environment.NewLine &
            '                gemeinde_verz)
        End If
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        Try
            existing.Source = New Uri(testfi.FullName) 'erz'".\daten\gemarkungen.xml")
        Catch ex As Exception
            Debug.Print(ex.ToString)
        End Try
    End Sub
    Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStrasse.SelectionChanged
        If cmbStrasse.SelectedItem Is Nothing Then Exit Sub
        Dim myvali$ = CStr(cmbStrasse.SelectedValue)
        Dim item2 As DataRowView = CType(cmbStrasse.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        sname = item2.Row.ItemArray(0).ToString.Trim
        strcode = item2.Row.ItemArray(1).ToString.Trim
        'sname = item2.Row.ItemArray(2).ToString.Trim


        Dim zeigtauf, summentext As String
        'Dim gemeindestring As String = item3$.Replace("06438" & , "001")
        '  gemeindestring = item3$.Substring(5, 3).Trim

        cmbHausnr.IsEnabled = True
        tbStrasse.Text = sname ' item2.Row.ItemArray(0).ToString 
        'myGlobalz.aktADR.strasseCode = CInt(item4)
        'myGlobalz.aktADR.strasseName = item5

        'myGlobalz.adrREC.mydb.Host = "kis"
        'myGlobalz.adrREC.mydb.Schema = "albnas"

        inithausnrCombo()
        If adrREC.dt.Rows.Count < 1 Then
            Dim infotext As String
            Dim mesresult As New MessageBoxResult

            infotext = "Hinweis: Es konnten keine Hausnummern zu dieser Straße gefunden werden." & Environment.NewLine &
                "Entweder es handelt sich um ein Gewann / Flurbezeichnung, oder " & Environment.NewLine &
                " es gibt keine bewohnten Adressen in der Straße." & Environment.NewLine &
                "" & Environment.NewLine &
                "" & Environment.NewLine &
                " Sie können hier abbrechen      (Abbruch)" & Environment.NewLine &
                "oder sich die zu dieser Gewann gehörigen Flurstücke auflisten lassen (OK)"
            mesresult = CType(MessageBox.Show(infotext, "Keine Hausnummern gefunden"), MessageBoxResult)

            If mesresult = MessageBoxResult.Cancel Then
                Exit Sub
                e.Handled = True
            Else
                tbHausnr.Text = ""
                'buttonenDISablen()

                ' inithausnrCombo2(gemeindestring)
                'zeigtauf = myGlobalz.adrREC.dt.Rows(0).Item("gml_id").ToString
                'getflurstueckWeistauf(zeigtauf, "zeigtauf")
                'mapFlurstueck(myGlobalz.fstREC, myGlobalz.aktFST, summentext)
                'MessageBox.Show(summentext)
                'NennerVerarbeiten(myGlobalz.aktFST.nenner.ToString)
                'buttonEnablen()
            End If
        Else

            cmbHausnr.DataContext = adrREC.dt
            cmbHausnr.IsDropDownOpen = True
        End If
        e.Handled = True
    End Sub
    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbHausnr.SelectionChanged
        If cmbHausnr.SelectedItem Is Nothing Then Exit Sub
        Dim hausnrkombi$ = CStr(cmbHausnr.SelectedValue)
        Dim item2 As DataRowView = CType(cmbHausnr.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim summentext As String = "" ' wird hier ignoriert
        Dim HausKombi = hausnrkombi
        Dim weistauf = item2.Row.ItemArray(1).ToString
        rechts = item2.Row.ItemArray(2).ToString
        hoch = item2.Row.ItemArray(3).ToString
        tbHausnr.Text = hausnrkombi
        calcBbox(rechts, hoch, bbox, 100)
        Dim result = starteWebbrowserControl(CInt(rechts), CInt(hoch), 100, True)
        e.Handled = True
    End Sub

    Private Sub calcBbox(rechts As String, hoch As String, bbox As clsRange, radius As Integer)
        bbox.xl = CInt(rechts) - radius
        bbox.yl = CInt(hoch) - radius
        bbox.xh = CInt(rechts) + radius
        bbox.yh = CInt(hoch) + radius
    End Sub

    Private Sub btnInfo_Click(sender As Object, e As RoutedEventArgs)
        Dim aaa As New winRTF
        aaa.ShowDialog()
        e.Handled = True
    End Sub
    Sub initGemarkungsCombo()
        Dim path = IO.Directory.GetCurrentDirectory
        Dim testfi As New IO.FileInfo(path & "\xml\" & _verz)
        'Dim testfi As New IO.FileInfo(_verz)
        If Not testfi.Exists Then
            'MessageBox.Show("Die Gemarkungsliste konnte nicht gefunden werden! " & Environment.NewLine &
            '                _verz)
        End If
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemarkungen"), XmlDataProvider)
        Try
            existing.Source = New Uri(testfi.FullName) 'erz'".\daten\gemarkungen.xml")
        Catch ex As Exception
            Debug.Print(ex.ToString)
        End Try
    End Sub


    Private Sub cmbNenner_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbNenner.SelectionChanged
        Dim item2 As DataRowView = CType(cmbNenner.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Try
        Catch ex As Exception
            Exit Sub
        End Try
        tbNenner.Text = item2.Row.ItemArray(0).ToString
        NennerVerarbeiten(tbNenner.Text)
        aktFST.FS = aktFST.buildFS
        rechtsHochwertHolen(aktFST)
        rechts = CType(CInt(aktFST.GKrechts), String)
        hoch = CType(CInt(aktFST.GKhoch), String)
        calcBbox(rechts, hoch, bbox, 100)
        Dim result = starteWebbrowserControl(CInt(rechts), CInt(hoch), 100, True)
        e.Handled = True
    End Sub
    Private Sub cmbFlur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbFlur.SelectionChanged
        Dim item2 As DataRowView = CType(cmbFlur.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub

        cmbZaehler.IsEnabled = True
        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbFlur.Text = item2.Row.ItemArray(0).ToString
        aktFST.flur = CInt(item3$)
        initZaehlerCombo()
        cmbZaehler.IsDropDownOpen = True
    End Sub
    Sub initZaehlerCombo()
        holeZaehlerDT()
        cmbZaehler.DataContext = fstREC.dt
    End Sub
    Private Sub cmbgemarkung_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbgemarkung.SelectedItem Is Nothing Then Exit Sub
        Dim myvali$ = CStr(cmbgemarkung.SelectedValue)
        Dim myvalx = CType(cmbgemarkung.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        tbGemarkung.Text = myvals
        aktFST.gemcode = CInt(myvali)
        aktFST.gemarkungstext = tbGemarkung.Text
        initFlureCombo()
        cmbFlur.IsDropDownOpen = True
        e.Handled = True
    End Sub
    Sub initFlureCombo()
        holeFlureDT()
        cmbFlur.DataContext = fstREC.dt
    End Sub
    Private Sub cmbZaehler_SelectionChanged(ByVal sender As System.Object,
                                           ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbZaehler.SelectionChanged
        Dim item2 As DataRowView = CType(cmbZaehler.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        cmbNenner.IsEnabled = True
        tbZaehler.Text = item2.Row.ItemArray(0).ToString
        aktFST.zaehler = CInt(item3$)
        aktFST.nenner = Nothing
        initNennerCombo()
        tbWeitergabeVerbot.Text = verbotsString
        If fstREC.dt.Rows.Count = 1 Then
            tbNenner.Text = fstREC.dt.Rows(0).Item(0).ToString
            aktFST.nenner = CInt(tbNenner.Text)
            aktFST.FS = aktFST.buildFS
            rechtsHochwertHolen(aktFST)
            rechts = CType(CInt(aktFST.GKrechts), String)
            hoch = CType(CInt(aktFST.GKhoch), String)
            calcBbox(rechts, hoch, bbox, 100)
            Dim result = starteWebbrowserControl(CInt(rechts), CInt(hoch), 100, True)
        Else
            cmbNenner.IsDropDownOpen = True
        End If
        e.Handled = True
    End Sub
    Sub initNennerCombo()
        holeNennerDT()
        cmbNenner.DataContext = fstREC.dt
    End Sub
    Public Shared Sub nennerUndFSPruefen()
        aktFST.FS = aktFST.buildFS()
        aktFST.fstueckKombi = aktFST.buildFstueckkombi
    End Sub
    Private Sub NennerVerarbeiten(ByVal nennertext As String)
        aktFST.nenner = CInt(nennertext)
        nennerUndFSPruefen()
    End Sub

    Private Sub startKoord_Click(sender As Object, e As RoutedEventArgs)
        rechts = CType(tbrechts.Text, String)
        hoch = CType(tbhoch.Text, String)
        calcBbox(rechts, hoch, bbox, 100)
        Dim result = starteWebbrowserControl(CInt(rechts), CInt(hoch), 100, True)
        e.Handled = True
    End Sub

    Private Sub btnEigentuemer_Click(sender As Object, e As RoutedEventArgs)
        'MsgBox("Coming soon")
        'SchnellausgabeMitProtokoll()
        e.Handled = True
    End Sub
    'Private Sub SchnellausgabeMitProtokoll()
    '    'If schonSchnellAusgegeben Then Exit Sub
    '    Dim grund As String = tbGrund.Text

    '    If grund Is Nothing OrElse grund.Trim.Length < 2 OrElse grund = "Aktenzeichen" Then
    '        MsgBox("Bitte eine Begründung (z.B. das Aktenzeichen) eingeben!")
    '        FocusManager.SetFocusedElement(Me, tbGrund)
    '        Exit Sub
    '    End If
    '    Dim info As String

    '    info = "Eigentümer in Kurzform: " & Environment.NewLine &
    '                                getSchnellbatch(aktFST.FS)
    '    tbWeitergabeVerbot.Text = info
    '    'schonSchnellAusgegeben = True
    '    Protokollausgabe_aller_Parameter(aktFST.FS, grund)
    'End Sub

    Private Sub tbGrund_SelectionChanged(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub
End Class
