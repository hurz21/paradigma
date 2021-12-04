
Imports System.IO
    Imports System.ComponentModel

    Class MainWindow
        'Private Property filenames As String()
        Property aktfoto As New clsFoto
        Property Bilder As New List(Of clsFoto)
        Property appVerzeichnis As String = ""
        Property ausgabeVerzeichnis As String = ""
        Property jpgVerzeichnis As String = ""
        Property thumbVerzeichnis As String = ""
        Property FileListString As String = ""
        Public Property endung As String = ".jpg"
        Public worker As New BackgroundWorker
        Public Property ladevorgangabgeschlossen As Boolean = False
        Sub New()
            InitializeComponent()
        End Sub
        Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            e.Handled = True
            Dim dirr As New clstools
            dirr.setLogfile() : l("Start " & Now) ': l("mgisversion:" & mgisVersion)
            'initWorker()
            appVerzeichnis = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "heic2jpg")

            ausgabeVerzeichnis = IO.Path.Combine(appVerzeichnis, "ausgabe")
            jpgVerzeichnis = IO.Path.Combine(appVerzeichnis, "jpg")
            thumbVerzeichnis = IO.Path.Combine(appVerzeichnis, "thumb")
            l("appVerzeichnis " & appVerzeichnis)
            l("ausgabeVerzeichnis " & ausgabeVerzeichnis)
            l("jpgVerzeichnis " & jpgVerzeichnis)
            l("thumbVerzeichnis " & thumbVerzeichnis)

            dirr.erzeugeVerzeichnisse(appVerzeichnis)
            dirr.erzeugeVerzeichnisse(ausgabeVerzeichnis)
            dirr.erzeugeVerzeichnisse(jpgVerzeichnis)
            dirr.erzeugeVerzeichnisse(thumbVerzeichnis)
            dirr.alteInhalteloeschen(appVerzeichnis)
            dirr.alteInhalteloeschen(ausgabeVerzeichnis)
            dirr.alteInhalteloeschen(jpgVerzeichnis)
            dirr.alteInhalteloeschen(thumbVerzeichnis)
            ladevorgangabgeschlossen = True
        End Sub


        'Private Sub initWorker()
        '    AddHandler worker.DoWork, AddressOf worker_DoWork
        '    worker.WorkerSupportsCancellation = True
        '    worker.RunWorkerAsync()
        'End Sub

        'Private Sub worker_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        '    'thumbnails4Liste(True, thumbNailWidth, thumbNailHeight)
        '    generieren(Bilder.Count, 2560)

        '    If worker.CancellationPending Then
        '        e.Cancel = True
        '        Exit Sub
        '    End If
        '    e.Result = "huhu"
        'End Sub
        Private Function heic2jpg(endung As String, bilder As List(Of clsFoto), verzeichnis As String, maxPixels As Integer) As Boolean
            'q16 https://github.com/dlemstra/Magick.NET/blob/master/docs/Readme.md 
            Dim erfolg As Boolean
            Dim neuliste As String = ""
            Dim ic As Integer = 0
            Dim abstract As String = ""
            Dim conv As New clstools
            Try
                l(" MOD heic2jpg anfang")
                For Each bild As clsFoto In bilder
                    Dim fi As New IO.FileInfo(bild.originalFile)
                    If fi.Exists Then
                        l("heic2jpg endung " & endung & ", bild.originalFile)  " & bild.originalFile)
                        bild.outfile = conv.calcOutfileName(endung, fi.Name, verzeichnis)
                        erfolg = conv.conv2jpg(bild.originalFile, bild.outfile, maxPixels, abstract)
                        bild.abstract = abstract
                        l("heic2jpg erfolg " & erfolg & ", bild.originalFile)  " & bild.originalFile)
                        ic += 1
                        neuliste = ic & ": " & bild.shortname & " " & " fertig. " & Environment.NewLine & neuliste
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                        tbInputlist.Text = neuliste
                    End If
                Next
                l(" MOD heic2jpg ende")
                conv = Nothing
                Return True
            Catch ex As Exception
                l("Fehler in heic2jpg: " & ex.ToString())
                Return False
            End Try
        End Function



        Private Sub l(text As String)
            text = text.Replace("DefaultSource	Information	0	", "")
            'text = text & Environment.NewLine & ToLogString(exec, "")

            'text = text & exec.ToString
            'If myglobalz.minErrorMessages Then
            '    If Not (text.ToLower.StartsWith("fehler") Or text.ToLower.StartsWith("warnung")) Then
            '        Exit Sub
            '    End If
            'End If
            My.Log.WriteEntry(text)
        End Sub



        Private Sub resizeImage()
            imgMAIN.Width = Me.ActualWidth - sv1.ActualWidth
            imgMAIN.Height = Me.ActualHeight - 100
            MainListBox.Height = Me.ActualHeight - 150
            sv1.Height = Me.ActualHeight - 140
        End Sub

        Private Function nachJPGumwandeln(endung As String, verzeichnis As String, maxpixels As Integer, lbilder As List(Of clsFoto)) As List(Of String)
            'tbInputlist.Text = " S T A R T ========================="
            Try
                l(" MOD nachJPFumwandeln anfang")
                l("nachJPFumwandeln endung: " & endung & ", verz  " & verzeichnis & ", maxpixels " & maxpixels)
                Dim erfolg = heic2jpg(endung, lbilder, verzeichnis, maxpixels)
                l("erfolg: " & erfolg)
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                'tbInputlist.Text = tbInputlist.Text & Environment.NewLine &
                '    " F E R T I G ========================="
                l("nachJPFumwandeln fertig ")

                l(" MOD nachJPFumwandeln ende")
            Catch ex As Exception
                l("Fehler in nachJPFumwandeln: " & ex.ToString())
            End Try
        End Function
        Sub textausgabe(text As String)
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            tbInputlist.Text = text & Environment.NewLine & tbInputlist.Text
        End Sub

        Private Sub gpMain_Drop(sender As Object, e As DragEventArgs)
            e.Handled = True
            l("gpMain_Drop")
            Dim filenames() As String
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                filenames = CType(e.Data.GetData(DataFormats.FileDrop), String())
            End If

            resetDisplay()

            l("filenames " & filenames.Count)
            handleInputarray(filenames, 2560)
        End Sub

        Private Sub resetDisplay()
            Bilder.Clear()
            imgMAIN.Source = Nothing
            MainListBox.ItemsSource = Nothing
            tbInputlist.Text = "Clean"
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        End Sub

        Private Sub handleInputarray(filenames() As String, maxpixels As Integer) ', bilder As List(Of clsFoto))
            Dim conv As New clstools

            Try
                l(" MOD handleInputarray anfang")
                filenames = conv.ArrayBereinigen(filenames)
                If filenames Is Nothing Then
                    MessageBox.Show("Die ausgewählten Dateien waren keine Fotos! Abbruch!")
                    Exit Sub
                End If
                textausgabe("Es werden " & filenames.Count & " Dateien umgewandelt")
                FileListString = makeFilelistString(filenames)
                getBilderListOriginalShortname(filenames) : l("shortnames " & filenames.Count)

                nameJPGFiles(conv) : l("jpgFile " & filenames.Count)
                nameThumbNails(conv) : l("thumbnail " & filenames.Count)
                textausgabe("Bitte um Geduld. Bereite Umwandlung vor!")
                Dim dummy = nachJPGumwandeln(".jpg", jpgVerzeichnis, maxpixels, Bilder)
                textausgabe("Jetzt Vorschaubilder erzeugen")
                Dim dummy2 = nachJPGumwandeln(".jpg", thumbVerzeichnis, 90, Bilder)

                textausgabe("Alles darstellen")
                l("display " & Bilder.Item(0).jpgFile)
                conv.koordinatenHolen(Bilder)
                Dim mana As New fotomanager
                aktfoto = mana.kopiere(Bilder.Item(0))
                mana = Nothing
                displayMainImage(aktfoto.jpgFile)
                If aktfoto.hatkoordinaten = "hat Koordinaten" Then
                    btnGmaps.IsEnabled = True
                End If
                MainListBox.ItemsSource = Bilder
                tbtreffer.Text = Bilder.Count & " Quelldateien. "
                'tbInputlist.Text = FileListString

                conv = Nothing
                l(" MOD handleInputarray ende")
            Catch ex As Exception
                l("Fehler in handleInputarray: " & ex.ToString())
            End Try
        End Sub

        Private Sub nameThumbNails(conv As clstools)
            For Each bild As clsFoto In Bilder
                bild.thumbnail = conv.calcOutfileName(".jpg", bild.jpgFile, thumbVerzeichnis)
            Next
        End Sub

        Private Sub nameJPGFiles(conv As clstools)
            For Each bild As clsFoto In Bilder
                bild.jpgFile = conv.calcOutfileName(".jpg", bild.originalFile, jpgVerzeichnis)
            Next
        End Sub

        Private Function getBilderListOriginalShortname(filenames() As String) As Boolean
            Dim fi As IO.FileInfo
            Dim neuBild As New clsFoto
            Try
                l(" MOD getBilderListOriginalShortname anfang")
                For i = 0 To filenames.Count - 1
                    neuBild = New clsFoto
                    neuBild.originalFile = filenames(i)
                    fi = New FileInfo(neuBild.originalFile)
                    neuBild.shortname = fi.Name
                    Bilder.Add(neuBild)
                Next
                l(" MOD getBilderListOriginalShortname ende")
                Return True
            Catch ex As Exception
                l("Fehler in getBilderListOriginalShortname: " & ex.ToString())
                Return False
            End Try
        End Function

        'Private Sub generieren(anzahl As Integer, maxpixels As Integer)

        '    Dim thumbs = nachJPFumwandeln(".jpg", thumbVerzeichnis, 90)

        'End Sub

        Private Sub displayMainImage(bilddatei As String)
            Dim bmi As New BitmapImage()
            Try
                l(" MOD displayMainImage anfang")
                bmi.BeginInit()
                bmi.CacheOption = BitmapCacheOption.OnLoad
                bmi.UriSource = New Uri(bilddatei)
                'bmi.DecodePixelWidth = 200
                bmi.CacheOption = BitmapCacheOption.OnLoad ' verhindert fehler beim 
                bmi.EndInit()
                imgMAIN.Source = bmi
                l(" MOD displayMainImage ende")
            Catch ex As Exception
                l("Fehler in displayMainImage: " & ex.ToString())
            End Try
            Select Case aktfoto.Orientation
                Case ExifWorksQuick.Orientations.LftBottom
                    'ImageLinksdrehen()
                    Dim tb As New TransformedBitmap()
                    tb.BeginInit()
                    tb.Source = bmi
                    Dim transform As New RotateTransform(-90)
                    tb.Transform = transform
                    tb.EndInit()
                    imgMAIN.Source = tb
                    imgMAIN.Stretch = Stretch.Uniform
                Case ExifWorksQuick.Orientations.RightTop
                    'ImageRechtsdrehen()
                    Dim tb As New TransformedBitmap()
                    tb.BeginInit()
                    tb.Source = bmi
                    Dim transform As New RotateTransform(90)
                    tb.Transform = transform
                    tb.EndInit()
                    imgMAIN.Source = tb
                    imgMAIN.Stretch = Stretch.Uniform
                Case Else

            End Select
        End Sub
        'Private Sub displayImageExtracted(datei As String)
        '    Try
        '        Dim bmi As New BitmapImage()
        '        bmi.BeginInit()
        '        bmi.CacheOption = BitmapCacheOption.OnLoad
        '        bmi.UriSource = New Uri(datei)
        '        bmi.EndInit()
        '        myimagerefresh.Source = bmi
        '        bmi = Nothing
        '    Catch ex As Exception
        '        nachricht("fehler in displayImageExtracted", ex)
        '    End Try
        'End Sub

        Private Function makeFilelistString(filenames As String()) As String
            Dim sb As New Text.StringBuilder
            For Each Texst In filenames
                sb.Append(Texst & Environment.NewLine)
            Next
            Return sb.ToString
        End Function

        Private Sub btnFiledialog_Click(sender As Object, e As RoutedEventArgs)
            e.Handled = True
            resetDisplay()
            Dim filenames() As String
            Try
                l(" MOD ---------------------- anfang")
                Bilder.Clear()
                Dim fileDialog = New System.Windows.Forms.OpenFileDialog()
                fileDialog.Multiselect = True
                fileDialog.Filter =
            "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF;*.HEIC|" +
            "All files (*.*)|*.*"
                Dim result = fileDialog.ShowDialog()
                Select Case result
                    Case System.Windows.Forms.DialogResult.OK
                        filenames = fileDialog.FileNames
                        Dim liste = makeFilelistString(filenames)
                        tbtreffer.Text = filenames.Count & " Quelldateien. "
                        tbInputlist.Text = liste
                    Case System.Windows.Forms.DialogResult.Cancel
                        tbInputlist.Text = ""
                        tbInputlist.ToolTip = ""
                End Select
                If filenames IsNot Nothing Then
                    handleInputarray(filenames, 2560)
                End If
                l(" MOD ---------------------- ende")
            Catch ex As Exception
                l("Fehler in MOD: " & ex.ToString())
            End Try
        End Sub

        Private Sub btnsave_Click(sender As Object, e As RoutedEventArgs)
            e.Handled = True
            Dim jj As ComboBoxItem = CType(cmbFormat.SelectedItem, ComboBoxItem)
            endung = jj.Tag.ToString
            Dim jpgliste = nachJPGumwandeln(endung, ausgabeVerzeichnis, 2560, Bilder)
            Dim conv As New clstools
            conv.verzeichnisOeffnen(ausgabeVerzeichnis)
            conv = Nothing
        End Sub

        Private Sub cmbFormat_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            e.Handled = True
            If ladevorgangabgeschlossen Then
                Dim jj As ComboBoxItem = CType(cmbFormat.SelectedItem, ComboBoxItem)
                endung = jj.Tag.ToString
            Else
                endung = ".jpg"
            End If
        End Sub

        Private Sub btnshowDir_Click(sender As Object, e As RoutedEventArgs)
            e.Handled = True
            Dim conv As New clstools
            conv.verzeichnisOeffnen(ausgabeVerzeichnis)
            conv = Nothing
        End Sub

        Private Sub chkauswahlgeaendert(sender As Object, e As RoutedEventArgs)

        End Sub

        Private Sub MainListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            e.Handled = True
            If MainListBox.SelectedItem Is Nothing Then Exit Sub
            If MainListBox.SelectedValue Is Nothing Then Exit Sub
            aktfoto = CType(MainListBox.SelectedValue, clsFoto)
            displayMainImage(aktfoto.jpgFile)
            If aktfoto.hatkoordinaten = "hat Koordinaten" Then
                btnGmaps.IsEnabled = True
            Else
                btnGmaps.IsEnabled = False
            End If
            tbInputlist.Text = aktfoto.Exifgpslongitude & "," & aktfoto.Exifgpslatitude & Environment.NewLine &
            aktfoto.shortname & Environment.NewLine &
            aktfoto.ExifDatum
        End Sub

        Private Sub btnsave1_Click(sender As Object, e As RoutedEventArgs)
            e.Handled = True
            Dim jj As ComboBoxItem = CType(cmbFormat.SelectedItem, ComboBoxItem)
            endung = jj.Tag.ToString
            Dim neuliste As New List(Of clsFoto)
            neuliste.Add(aktfoto)
            Dim jpgliste = nachJPGumwandeln(endung, ausgabeVerzeichnis, 2560, neuliste)
            Dim conv As New clstools
            conv.verzeichnisOeffnen(ausgabeVerzeichnis)
            conv = Nothing
        End Sub

        Private Sub btnGmaps_Click(sender As Object, e As RoutedEventArgs)
            e.Handled = True
            Process.Start(aktfoto.mapsurl)
        End Sub
    End Class


