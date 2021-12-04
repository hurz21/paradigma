Imports System.ComponentModel
Imports System.IO
Imports System.Data
Partial Public Class winFotoGucker
    Implements INotifyPropertyChanged
    Private Property isInMaxFotoMode As Boolean = False
    Private aktpoint As myPoint
    Public worker As New BackgroundWorker
    Private _myBitmapImage As BitmapImage
    Public Property aktfoto As New Dokument
    Public Property tb As New TransformedBitmap()
    Property thumbNailWidth As Integer = 128
    Property thumbNailHeight As Integer = 90

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged
    Private ladevorgangabgeschlossen As Boolean = False
    Private Property angeklickt As Boolean = False
    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
        'spMini.Background = myGlobalz.GetSecondBackground()
    End Sub

    Protected Sub OnPropertyChanget(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Private _gesamtcursor As Integer
    Private mini_anzahl_preview As Integer = 5


    Private Sub winFotoGucker_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        starteForm2()
        initStartPositionOnScreen()
        setFotoSize()
        resizeImage()
        ladevorgangabgeschlossen = True
    End Sub

    Private Sub setFotoSize()
        If Not ladevorgangabgeschlossen Then Exit Sub
        sprechts.Width = ActualWidth - spMini.ActualWidth '
        stpFotoContainer.Height = ActualHeight - Desc.ActualHeight - 100
        stpFotoContainer.Width = sprechts.Width
        sprechts.Height = ActualHeight
    End Sub
    Private Sub resizeImage()
        If Image1.Width > ActualWidth - spMini.ActualWidth Then
            Image1.Width = ActualWidth - spMini.ActualWidth
        End If
        Exit Sub
        'Image1.Width = sprechts.ActualWidth - 100 : Image1.Height = sprechts.ActualHeight - 150
    End Sub
    Private Sub starteForm2()
        GC.Collect()

        MainListBox.DataContext = Nothing
        If aktfoto IsNot Nothing Then initHandleCursor(CLstart.myc.collFotos, aktfoto)
        initSetEtikett(CLstart.myc.collFotos)
        initKoordinatenknopf()
        If aktfoto IsNot Nothing Then
            If String.IsNullOrEmpty(aktfoto.DateinameMitExtension) Then
                starteForm()
            Else
                Gesamtcursor = getCursor4foto(aktfoto, CLstart.myc.collFotos)
                starteForm()
            End If
        Else
        End If
        gastLayout()
        initMaxheight()
    End Sub

    Sub New()
        InitializeComponent()
    End Sub
    Sub New(ByVal dok As Dokument)
        InitializeComponent()
        aktfoto = dok
    End Sub

    Private Sub starteForm()
        initWorker()
        initKoordinatenknopf()
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
        fillListbox()
        MainListBox.MaxHeight = winfotogucker.Height - 50
        Title = StammToolsNs.setWindowTitel.exe("edit", "Fotogugger")
        GRDmetadata.DataContext = aktfoto
        thumbnails4Liste(False, thumbNailWidth, thumbNailHeight)
    End Sub


    Sub zeigeInMain(ByVal auswahlDok As Dokument)
        guggaTools.kopiereDokMetadaten(aktfoto, auswahlDok)
        Dim test As IO.FileInfo
        If auswahlDok.newSaveMode Then
            test = New IO.FileInfo(auswahlDok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir))
        Else
            test = New IO.FileInfo(auswahlDok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir))
        End If
        displayCursorNr()
        zeigeMetadaten(aktfoto) 'binding funzt nicht!!!
        zeigeFoto(auswahlDok, test)
        test = Nothing
        initKoordinatenknopf()
        aktpoint = New myPoint
        'Dim test = DBraumbezug_Mysql.getRaumbezugsCoords_2dokument(myGlobalz.sitzung.aktDokument.DocID)
        Dim test2 As myPoint = RBtoolsns.getRaumbezugsCoords_2dokument_alledb.exe(aktfoto.DocID)
        If Not test2 Is Nothing Then
            aktpoint = CType(test2, myPoint)
            'raumbezug ist vorhanden
            'label einschalten
            'gk-koordinaten holen
            'Else
            'raumbezug ist nicht vorhanden
            'label ausschalten
        End If
    End Sub


    Private Sub btnVor_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnVor.Click
        vor()
    End Sub

    Sub vor()
        Dim test% = Gesamtcursor% + 1
        If test% > CLstart.myc.collFotos.Count - 1 Then
            info("Sie haben das obere Ende erreicht")
            Gesamtcursor% = 0
        Else
            Gesamtcursor% += 1
        End If
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
    End Sub
    Sub back()
        Dim test% = Gesamtcursor% - 1
        If test% < 0 Then
            info("Sie haben das untere Ende erreicht")
            Gesamtcursor% = CLstart.myc.collFotos.Count - 1
        Else
            Gesamtcursor% -= 1
        End If
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
    End Sub
    Sub info(ByVal text$)
        tbINFO.Text = text
    End Sub

    Private Sub btnBack_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBack.Click
        back()
    End Sub

    Public Property Gesamtcursor%()
        Get
            Return _gesamtcursor
        End Get
        Set(ByVal value%)
            _gesamtcursor = value
            OnPropertyChanget("Gesamtcursor1")
        End Set
    End Property

    Private Sub btnfastBack_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnfastBack.Click
        Dim test% = Gesamtcursor% - mini_anzahl_preview%
        If test% < 0 Then
            info("Sie haben das untere Ende erreicht")
            Gesamtcursor% = CLstart.myc.collFotos.Count - 1
        Else
            Gesamtcursor% -= mini_anzahl_preview%
        End If
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    Private Sub btnende_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnende.Click
        info("Sie haben das untere Ende erreicht")
        Gesamtcursor% = CLstart.myc.collFotos.Count - 1
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    Private Sub btnAnfang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAnfang.Click
        info("Sie haben das untere Ende erreicht")
        Gesamtcursor% = 0
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    Private Sub btnfastVor_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnfastVor.Click
        Dim test% = Gesamtcursor% + mini_anzahl_preview%
        If test% > CLstart.myc.collFotos.Count - 1 Then
            info("Sie haben das obere Ende erreicht")
            Gesamtcursor% = CLstart.myc.collFotos.Count - 1
        Else
            Gesamtcursor% += mini_anzahl_preview%
        End If
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    'Private Sub btnGotoNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnGotoNr.Click
    '    If CInt(tbGcursorPOS.Text) > myGlobalz.collFotos.Count - 1 Then Gesamtcursor = myGlobalz.collFotos.Count - 1
    '    If CInt(tbGcursorPOS.Text) < 1 Then Gesamtcursor = 0
    '    zeigeInMain(myGlobalz.collFotos.Item(Gesamtcursor))
    'End Sub



    Public Sub zeigeFoto(ByVal auswahlDok As Dokument, ByVal test As IO.FileInfo)
        Try
            GC.Collect()
            Dim bmp As New System.Drawing.Bitmap(auswahlDok.FullnameImArchiv)
            Console.WriteLine("Width: " & bmp.Width.ToString() + " > Height: " & bmp.Height)
            tbformat.Text = bmp.Width.ToString() + " x " & bmp.Height & " Pixel"
            bmp.Dispose()
            bmp = Nothing

            If test.Exists Then
                _myBitmapImage = New BitmapImage()
                _myBitmapImage.BeginInit()
                _myBitmapImage.UriSource = New Uri(auswahlDok.FullnameImArchiv)


                'Debug.Print(_myBitmapImage.DecodePixelWidth.ToString & " : " & _myBitmapImage.PixelWidth)
                'If isInMaxFotoMode Then
                '    _myBitmapImage.DecodePixelWidth = 0
                'Else
                '    _myBitmapImage.DecodePixelWidth = 600
                'End If
                If auswahlDok.DateinameMitExtension.ToLower.EndsWith(".jpg") Then
                    _myBitmapImage.DecodePixelWidth = 600
                    _myBitmapImage.DecodePixelHeight = 600
                Else
                    _myBitmapImage.DecodePixelWidth = 0
                    _myBitmapImage.DecodePixelHeight = 0
                End If
                _myBitmapImage.CacheOption = BitmapCacheOption.OnLoad ' verhindert fehler beim löschen

                'Ursache:
                '                Beim Laden eines Fotos in ein WPF Image zur Anzeige wurde dieses direkt mit der originalen Source geladen. 
                'Dabei verbleibt das angezeigte Image im gesperrten Zustand, da es ja 1: 1 angezeigt wird.  
                'Lösung:
                '                Thumbnail und Cache beim Laden 
                'Beim Laden des Image Controls zur Laufzeitmit einer Bitmapsource stellt man die Source auf Cache und Thumbnail Modus ein. 
                'Dadurch wird quasi nur ein Abbild geladen und die Quelle bleibt anschliessend verschlossen. 
                'Anschliessend kann man Kopier- und Verschiebe-Funktionen mit dem Original Bild ausführen ohne eine Fehlermeldung zu erzeugen.  
                '//----< load+add Image >---- 
                'Image newImage = New Image();  
                '//-< source >- 
                'BitmapImage src = New BitmapImage(); 
                'src.BeginInit(); 
                'src.UriSource = New Uri(Image_with_Path, UriKind.Absolute); 
                '//< thumbnail > 
                'src.DecodePixelWidth = 200; 
                'src.CacheOption = BitmapCacheOption.OnLoad; 
                '//</ thumbnail > 


                _myBitmapImage.EndInit()
                Image1.Source = _myBitmapImage
                Dim testa As Boolean = isImageOrientationLandscape(auswahlDok.FullnameImArchiv)
                '    _myBitmapImage = Nothing muss vorhanden sein sonst funzt drehen nicht

            Else
                nachricht_und_Mbox("Diese Datei fehlt: " & test.Name)
            End If

        Catch ex As Exception
            nachricht_und_Mbox(ex.ToString)
        End Try
    End Sub

    Private Sub btnLinks_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLinks.Click
        ImageLinksdrehen()
        e.Handled = True
    End Sub
    Private Sub btnRechts_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRechts.Click
        ImageRechtsdrehen()
        e.Handled = True
    End Sub

    Private Sub ImageLinksdrehen()
        Dim tb As New TransformedBitmap()
        tb.BeginInit()
        tb.Source = _myBitmapImage
        Dim transform As New RotateTransform(-90)
        tb.Transform = transform
        tb.EndInit()
        Image1.Source = tb
        Image1.Stretch = Stretch.Uniform
    End Sub
    Private Sub ImageRechtsdrehen()
        Dim tb As New TransformedBitmap()
        Dim transform As New RotateTransform(90)
        Try
            l(" MOD ImageRechtsdrehen anfang")
            If _myBitmapImage Is Nothing Then Exit Sub
            tb.BeginInit()
            tb.Source = _myBitmapImage

            tb.Transform = transform
            tb.EndInit()
            Image1.Source = tb
            Image1.Stretch = Stretch.Uniform

            l(" MOD ImageRechtsdrehen ende")
        Catch ex As Exception
            l("Fehler in ImageRechtsdrehen: ", ex)
        End Try
    End Sub
    Sub displayCursorNr()
        tbGcursorPOS.Text = Gesamtcursor.ToString
    End Sub

    Private Sub btnStandardviewer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnStandardviewer.Click
        e.Handled = True
        btnStandardviewer_ClickExtracted(Gesamtcursor)

    End Sub

    Private Sub winFotoGucker_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        If angeklickt Then
            Dim myCanvas As New Canvas 'dummy durch nurfotos=true
            Psession.presFotos.Clear()
            Dim newdok As clsPresDokumente
            For Each fto As clsFotoDokument In CLstart.myc.collFotos
                ' If fto.ausgewaehlt Then
                newdok = detail_dokuauswahl.fotodokumentNachPresDokumentKonvertieren(fto)
                If newdok IsNot Nothing Then Psession.presFotos.Add(newdok)
                'End If
            Next

        End If

        savePosition()
        worker.CancelAsync()
    End Sub
    Private Sub savePosition()
        Try
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "fodoguggatop", CType(Me.Top, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "fodoguggaleft", CType(Me.Left, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "fodoguggawidth", CType(Me.ActualWidth, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "fodoguggaheight", CType(Me.ActualHeight, String))
        Catch ex As Exception
            l("fehler in saveposition  windb", ex)
        End Try
    End Sub
    Private Sub initStartPositionOnScreen()
        If myGlobalz.nureinbildschirm Then Exit Sub
        Dim topval = (CLstart.formposition.getPosition("diverse", "fodoguggatop", Me.Top))
        If topval < 0 Then
            Me.Top = 0
        Else
            Me.Top = topval
        End If
        Me.Left = CLstart.formposition.getPosition("diverse", "fodoguggaleft", Me.Left)
        Me.Width = CLstart.formposition.getPosition("diverse", "fodoguggawidth", Me.Width)
        Me.Height = CLstart.formposition.getPosition("diverse", "fodoguggaheight", Me.Height)
    End Sub
    Private Sub fillListbox()
        MainListBox.ItemsSource = CLstart.myc.collFotos
    End Sub

    Private Sub MainListBox_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        e.Handled = True
        If MainListBox.SelectedItem Is Nothing Then Exit Sub
        If MainListBox.SelectedValue Is Nothing Then Exit Sub
        aktfoto = CType(MainListBox.SelectedValue, Dokument)
        If aktfoto.Initiale = "loeschmich" Then Exit Sub
        Gesamtcursor = aktfoto.Handlenr
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))

    End Sub

    Private Sub zeigeMetadaten(ByVal dokument As Dokument)
        tbBeschreibung.Text = dokument.Beschreibung
        tbdir.Text = dokument.EXIFdir
        tbEXIFhersteller.Text = dokument.EXIFhersteller
        tbGcursorPOS.Text = CStr(Gesamtcursor)
        tblat.Text = dokument.EXIFlat
        tblong.Text = dokument.EXIFlon
        tbCheckindatum.Text = dokument.Checkindatum.ToString
        tbFiledatum.Text = dokument.Checkindatum.ToString
        If dokument.ExifDatum < CDate("1970-01-01") Then
            tbExifDatum.Text = dokument.Filedatum.ToString
        Else
            tbExifDatum.Text = dokument.ExifDatum.ToString
        End If
        ' tbExifDatum.Text = dokument.ExifDatum.ToString
        tbDateinameMitExtension.Text = dokument.DateinameMitExtension
    End Sub



    Private Sub initWorker()
        AddHandler worker.DoWork, AddressOf worker_DoWork
        worker.WorkerSupportsCancellation = True
        worker.RunWorkerAsync()
    End Sub

    Private Sub worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        thumbnails4Liste(True, thumbNailWidth, thumbNailHeight)
        If worker.CancellationPending Then
            e.Cancel = True
            Exit Sub
        End If
        e.Result = "huhu"
    End Sub


    Private Sub visualPrint()
        _myBitmapImage = New BitmapImage()
        _myBitmapImage.BeginInit()
        _myBitmapImage.UriSource = New Uri(CLstart.myc.collFotos.Item(Gesamtcursor).FullnameImArchiv)
        _myBitmapImage.DecodePixelWidth = 800
        _myBitmapImage.EndInit()
        Image1.Source = _myBitmapImage
        Dim vis = New DrawingVisual()
        Dim dc = vis.RenderOpen()
        Dim nrec As New Rect
        nrec.Width = _myBitmapImage.Width
        nrec.Height = _myBitmapImage.Height
        dc.DrawImage(_myBitmapImage, nrec)
        dc.Close()
        Dim pdialog = New PrintDialog()
        If pdialog.ShowDialog() = True Then
            pdialog.PrintVisual(vis, "My Image")
        End If
    End Sub
    Private Sub btnDruckaktuell_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        visualPrint()
        e.Handled = True
    End Sub


    Private Sub btnfotobuch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim myCanvas As New Canvas 'dummy durch nurfotos=true
        Psession.presFotos.Clear()
        Dim newdok As clsPresDokumente
        For Each fto As clsFotoDokument In CLstart.myc.collFotos
            If fto.ausgewaehlt Then
                newdok = detail_dokuauswahl.fotodokumentNachPresDokumentKonvertieren(fto)
                If newdok IsNot Nothing Then Psession.presFotos.Add(newdok)
            End If
        Next
        clsBerichte.fotobucherstellen(myCanvas, True, Psession.presFotos, CLstart.myc.kartengen.aktMap.aktrange, True, True)

    End Sub

    'Private Sub Hyperlink_RequestNavigate(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
    '    Dim gis As New clsGISfunctions()
    '    gis.WebGISmittelpunktsAufruf(aktpoint, initP.getValue("GisServer.GIS_WebServer"))
    '    e.Handled = True
    'End Sub





    Private Sub Abbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Image1.Source = Nothing
        If _myBitmapImage IsNot Nothing Then
            _myBitmapImage.UriSource = Nothing
            _myBitmapImage = Nothing
        End If
        e.Handled = True
    End Sub

    Public Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If EingabeistOK(tbBeschreibung.Text) Then
            If tbBeschreibung.Text.Count > 0 Then
                aktfoto.Beschreibung = tbBeschreibung.Text
            End If
            guggaTools.speichernFotoDetails(aktfoto)
            'speichernFotoDetails(aktfoto)
        End If
        e.Handled = True
        Me.Close()

    End Sub
    Function EingabeistOK(text As String) As Boolean
        Return True
    End Function

    Private Sub btnKillCoords_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KoordinatenGgfLoeschen(aktfoto)
        e.Handled = True
    End Sub

    Private Sub initKoordinatenknopf()
        If aktfoto IsNot Nothing Then

            If aktfoto.hatKoordinaten() Then
                btnKillCoords.IsEnabled = True
            Else
                btnKillCoords.IsEnabled = False
            End If
        End If
    End Sub

    Private Function isImageOrientationLandscape(fullname As String) As Boolean
        Dim exifQ As ExifWorksQuick
        exifQ = New ExifWorksQuick(fullname)
        ' MsgBox(exifQ.Orientation)
        Select Case exifQ.Orientation
            Case ExifWorksQuick.Orientations.LftBottom
                ImageLinksdrehen()
                setFotoSize()
                Return True
            Case ExifWorksQuick.Orientations.RightTop
                ImageRechtsdrehen()
                setFotoSize()
                Return True
        End Select
        Return False
    End Function

    Private Sub initMaxheight()
        Dim maxheight As Integer = 700
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
            'maxheight = 1008 '600
            'Height = 1008 '600
            '  MaxWidth = 1356
            '   Height = Height-30
        End If
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
            'maxheight = 900 '500
            'Height = maxheight
            ' MaxWidth = 1356
            Height = 950
        End If
        '  WindowState=Windows.WindowState.Maximized       
    End Sub

    Private Sub chkauswahlgeaendert(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        angeklickt = True
    End Sub

    Private Sub btnMarkierteFotosLoeschen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Psession.presFotos.Clear()
        Dim newdok As clsPresDokumente
        For Each fto As clsFotoDokument In CLstart.myc.collFotos
            If fto.ausgewaehlt Then
                newdok = detail_dokuauswahl.fotodokumentNachPresDokumentKonvertieren(fto)
                If newdok IsNot Nothing Then
                    guggaTools.fotoloeschen(newdok, False)
                End If
            End If
        Next
        Close()
    End Sub

    Private Sub TbBeschreibung_TextChanged(sender As Object, e As TextChangedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        btnSpeichern.IsEnabled = True
    End Sub

    Private Sub Image1_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub


        btnStandardviewer_ClickExtracted(Gesamtcursor)
    End Sub

    Private Sub BtnMaxSizeMode_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True

        'If isInMaxFotoMode Then
        '    spMini.Width = 500
        '    'GRDmetadata.Height = 150
        '    'Image1.Width = 600 : Image1.Height = 600
        '    isInMaxFotoMode = False
        '    'btnMaxSizeMode.Content = "Fotos in groß sichten"
        'Else
        '    spMini.Width = 500
        '    'GRDmetadata.Height = 1
        '    'Image1.Width = Me.ActualWidth - 100 : Image1.Height = Me.ActualHeight - 150
        '    'isInMaxFotoMode = True
        '    'btnMaxSizeMode.Content = "Fotos verkleinert sichten"
        'End If
        zeigeInMain(CLstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    Private Sub winFotoGucker_SizeChanged(sender As Object, e As SizeChangedEventArgs) Handles Me.SizeChanged

    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        'If isInMaxFotoMode Then
        '    'spMini.Width = 1
        '    'GRDmetadata.Height = 1
        '    setFotoSize()
        '    resizeImage()
        'Else
        '    Debug.Print("")
        '    setFotoSize()
        '    resizeImage()
        'End If
        setFotoSize()
        resizeImage()
    End Sub

    Private Sub Window_StateChanged(sender As Object, e As EventArgs)

        If Not ladevorgangabgeschlossen Then Exit Sub
        setFotoSize()
        'Select Case WindowState
        '    Case WindowState.Maximized
        '        isInMaxFotoMode = True
        '        'spMini.Width = 1
        '        GRDmetadata.Height = 1
        '        resizeImage()
        '    Case WindowState.Minimized
        '    Case WindowState.Normal

        'End Select
    End Sub


End Class
