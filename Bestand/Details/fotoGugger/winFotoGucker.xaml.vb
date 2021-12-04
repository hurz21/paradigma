Imports System.ComponentModel
Imports System.IO
Imports System.Data
Partial Public Class winFotoGucker
    Implements INotifyPropertyChanged
    Private aktpoint As myPoint
    Public worker As New BackgroundWorker
    Private _myBitmapImage As BitmapImage
    Public Property aktfoto As New Dokument
    Public Property tb As New TransformedBitmap()
    Property thumbNailWidth As Integer = 128
    Property thumbNailHeight As Integer = 90

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged

    Private Property angeklickt As Boolean = False

    Protected Sub OnPropertyChanget(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Private _gesamtcursor As Integer
    Private mini_anzahl_preview As Integer = 5


    Private Sub winFotoGucker_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initHandleCursor(clstart.myc.collFotos, aktfoto)
        initSetEtikett(clstart.myc.collFotos)
        initKoordinatenknopf()
        If String.IsNullOrEmpty(aktfoto.DateinameMitExtension) Then
            starteForm()
        Else
            Gesamtcursor = getCursor4foto(aktfoto, clstart.myc.collFotos)
            starteForm()
        End If
        e.Handled = True
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
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
        fillListbox()
        MainListBox.MaxHeight = winfotogucker.Height - 50
        Title = StammToolsNs.setWindowTitel.exe("edit", "Fotogugger")
        GRDmetadata.DataContext = aktfoto
        thumbnails4Liste(False, thumbNailWidth, thumbNailHeight)
    End Sub


    Sub zeigeInMain(ByVal auswahlDok As Dokument)
        guggaTools.kopiereDokMetadaten(aktfoto, auswahlDok)
        Dim test As New IO.FileInfo(auswahlDok.FullnameImArchiv)
        displayCursorNr()
        zeigeMetadaten(aktfoto) 'binding funzt nicht!!!
        zeigeFoto(auswahlDok, test)
        initKoordinatenknopf()
        aktpoint = New myPoint
        'Dim test = DBraumbezug_Mysql.getRaumbezugsCoords_2dokument(myGlobalz.sitzung.aktDokument.DocID)
        Dim test2 = RBtoolsns.getRaumbezugsCoords_2dokument_alledb.exe(aktfoto.DocID)
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
        If test% > clstart.myc.collFotos.Count - 1 Then
            info("Sie haben das obere Ende erreicht")
            Gesamtcursor% = 0
        Else
            Gesamtcursor% += 1
        End If
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
    End Sub
    Sub back()
        Dim test% = Gesamtcursor% - 1
        If test% < 0 Then
            info("Sie haben das untere Ende erreicht")
            Gesamtcursor% = clstart.myc.collFotos.Count - 1
        Else
            Gesamtcursor% -= 1
        End If
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
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
            Gesamtcursor% = clstart.myc.collFotos.Count - 1
        Else
            Gesamtcursor% -= mini_anzahl_preview%
        End If
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    Private Sub btnende_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnende.Click
        info("Sie haben das untere Ende erreicht")
        Gesamtcursor% = clstart.myc.collFotos.Count - 1
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    Private Sub btnAnfang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAnfang.Click
        info("Sie haben das untere Ende erreicht")
        Gesamtcursor% = 0
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    Private Sub btnfastVor_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnfastVor.Click
        Dim test% = Gesamtcursor% + mini_anzahl_preview%
        If test% > clstart.myc.collFotos.Count - 1 Then
            info("Sie haben das obere Ende erreicht")
            Gesamtcursor% = clstart.myc.collFotos.Count - 1
        Else
            Gesamtcursor% += mini_anzahl_preview%
        End If
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
    End Sub

    'Private Sub btnGotoNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnGotoNr.Click
    '    If CInt(tbGcursorPOS.Text) > myGlobalz.collFotos.Count - 1 Then Gesamtcursor = myGlobalz.collFotos.Count - 1
    '    If CInt(tbGcursorPOS.Text) < 1 Then Gesamtcursor = 0
    '    zeigeInMain(myGlobalz.collFotos.Item(Gesamtcursor))
    'End Sub



    Public Sub zeigeFoto(ByVal auswahlDok As Dokument, ByVal test As IO.FileInfo)
        Try
            'If _myBitmapImage IsNot Nothing Then
            '    _myBitmapImage = Nothing
            'End If
            'If Image1 IsNot Nothing Then
            '    Image1 = Nothing
            'End If
            'Image1 = New Image
            GC.Collect()
            If test.Exists Then
                _myBitmapImage = New BitmapImage()
                _myBitmapImage.BeginInit()
                _myBitmapImage.UriSource = New Uri(auswahlDok.FullnameImArchiv)
                _myBitmapImage.DecodePixelWidth = 800
                _myBitmapImage.EndInit()
                Image1.Source = _myBitmapImage
                Dim testa As Boolean = isImageOrientationLandscape(auswahlDok.FullnameImArchiv)


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
        tb.BeginInit()
        tb.Source = _myBitmapImage
        Dim transform As New RotateTransform(90)
        tb.Transform = transform
        tb.EndInit()
        Image1.Source = tb
        Image1.Stretch = Stretch.Uniform
    End Sub
    Sub displayCursorNr()
        tbGcursorPOS.Text = Gesamtcursor.ToString
    End Sub

    Private Sub btnStandardviewer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnStandardviewer.Click
        btnStandardviewer_ClickExtracted(Gesamtcursor)
        e.Handled = True
    End Sub

    Private Sub winFotoGucker_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        If angeklickt Then
            Dim myCanvas As New Canvas 'dummy durch nurfotos=true
            Psession.presFotos.Clear()
            Dim newdok As clsPresDokumente
            For Each fto As clsFotoDokument In clstart.myc.collFotos
               ' If fto.ausgewaehlt Then
                    newdok = detail_dokuauswahl.fotodokumentNachPresDokumentKonvertieren(fto)
                    If newdok IsNot Nothing Then Psession.presFotos.Add(newdok)
                'End If
            Next
           
        End If
        worker.CancelAsync()
    End Sub

    Private Sub fillListbox()
        MainListBox.ItemsSource = clstart.myc.collFotos
    End Sub

    Private Sub MainListBox_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If MainListBox.SelectedItem Is Nothing Then Exit Sub
        If MainListBox.SelectedValue Is Nothing Then Exit Sub
        aktfoto = CType(MainListBox.SelectedValue, Dokument)
        If aktfoto.Initiale = "loeschmich" Then Exit Sub
        Gesamtcursor = aktfoto.Handlenr
        zeigeInMain(clstart.myc.collFotos.Item(Gesamtcursor))
        e.Handled = True
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
        tbExifDatum.Text = dokument.ExifDatum.ToString
        tbDateinameMitExtension.Text = dokument.DateinameMitExtension
    End Sub

    'Private Sub btnThumbnails_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    thumbnails4Liste(True, thumbNailWidth, thumbNailHeight)
    '    e.Handled = True
    'End Sub

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
    'Private Sub worker_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs) 'Handles worker.RunWorkerCompleted
    '    If e.Error IsNot Nothing Then
    '        MsgBox("fehler: " & e.Error.Message)
    '    End If
    'End Sub     

    Private Sub visualPrint()
        _myBitmapImage = New BitmapImage()
        _myBitmapImage.BeginInit()
        _myBitmapImage.UriSource = New Uri(clstart.myc.collFotos.Item(Gesamtcursor).FullnameImArchiv)
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
        Dim myCanvas As New Canvas 'dummy durch nurfotos=true
        Psession.presFotos.Clear()
        Dim newdok As clsPresDokumente
        For Each fto As clsFotoDokument In clstart.myc.collFotos
            If fto.ausgewaehlt Then
                newdok = detail_dokuauswahl.fotodokumentNachPresDokumentKonvertieren(fto)
                If newdok IsNot Nothing Then Psession.presFotos.Add(newdok)
            End If
        Next
        clsBerichte.fotobucherstellen(myCanvas, True, Psession.presFotos, clstart.myc.kartengen.aktMap.aktrange)
        e.Handled = True
    End Sub

    Private Sub Hyperlink_RequestNavigate(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
        Dim gis As New clsGISfunctions()
        gis.mittelpunktsAufruf(aktpoint, initP.getValue("GisServer.GIS_WebServer"))
        e.Handled = True
    End Sub





    Private Sub Abbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Image1.Source = Nothing
        If _myBitmapImage IsNot Nothing Then
            _myBitmapImage.UriSource = Nothing
            _myBitmapImage = Nothing
        End If
        e.Handled = True
    End Sub

    Public Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If EingabeistOK() Then
            guggaTools.speichernFotoDetails(aktfoto)
            'speichernFotoDetails(aktfoto)
        End If
        e.Handled = True
        Me.Close()

    End Sub
    Function EingabeistOK() As Boolean
        Return True
    End Function

    Private Sub btnKillCoords_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        KoordinatenGgfLoeschen(aktfoto)
        e.Handled = True
    End Sub

    Private Sub initKoordinatenknopf()
        If aktfoto.hatKoordinaten() Then
            btnKillCoords.IsEnabled = True
        Else
            btnKillCoords.IsEnabled = False
        End If
    End Sub

    Private Function isImageOrientationLandscape(fullname As String) As Boolean
        Dim exifQ As ExifWorksQuick
        exifQ = New ExifWorksQuick(fullname)
        ' MsgBox(exifQ.Orientation)
        Select Case exifQ.Orientation
            Case ExifWorksQuick.Orientations.LftBottom
                ImageLinksdrehen()
            Case ExifWorksQuick.Orientations.RightTop
                ImageRechtsdrehen()
        End Select

    End Function



    Private Sub chkauswahlgeaendert(sender As Object, e As RoutedEventArgs)
        angeklickt = True
    End Sub
End Class
