

Partial Public Class WINBox

    Public Property dokumentWurdeGeoeffnet As Boolean = False
    Shared Property DateitypeText As String
    Public Property WordDateityp As String
    Private _quelle$ = ""
    Public readOnlyDoxsInTxtCrtlOeffnenLOKAL As Boolean = False
    Private aktThumbnailNotiz As String = ""

    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
        tbFilename.Background = myGlobalz.GetSecondBackground()
    End Sub

    Sub New(ByVal quelle As String, _readOnlyDoxsInTxtCrtlOeffnen As Boolean, _aktThumbnailNotiz As String)
        InitializeComponent()
        _quelle = quelle
        readOnlyDoxsInTxtCrtlOeffnenLOKAL = _readOnlyDoxsInTxtCrtlOeffnen
        aktThumbnailNotiz = _aktThumbnailNotiz
    End Sub

    Public Property knopfnummer As Integer
    Public Property datumThumbnail As String

    Private Sub WINBox_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Dim ad As New clsPresDokumente
        ad = ad.dokument2Presdokument(myGlobalz.sitzung.aktDokument)
        'ad.thumbnailMSGtext = myGlobalz.sitzung.aktEreignis.Notiz
        ad.thumbnailMSGtext = aktThumbnailNotiz
        Title = detailsTools.settitle("Dokument behandeln " & myGlobalz.sitzung.aktDokument.DateinameMitExtension)
        myimagerefresh.DataContext = GetPicturePath()
        tbFilename.Text = GetText()
        displayDokument(myGlobalz.sitzung.aktDokument.FullnameImArchiv)
        'If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.PDF Then
        '    spZuConject17.IsEnabled = False
        'End If
        If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.DOC Or
            myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.RTF Then
            btnWordReadonly.Visibility = Visibility.Visible
            'btnOffnen.Visibility = Visibility.Collapsed
            If isArchivberechtigt() Then
                textcontrolImArchivOeffnen.Visibility = Visibility.Visible
                'If myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.Contains("-tc_") Then
                'btnOffnen.Visibility = Visibility.Visible
                '    btnOffnenPower.Visibility = Visibility.Collapsed
                'End If
                If myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.EndsWith(".doc") Then radioAlsDOC.IsChecked = True
                If myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.EndsWith(".docx") Then radioAlsDOCX.IsChecked = True
                If myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.EndsWith(".rtf") Then radioAlsRTF.IsChecked = True
            Else
                textcontrolImArchivOeffnen.Visibility = Visibility.Collapsed
            End If
            If myGlobalz.sitzung.aktDokument.revisionssicher Then
                btnOffnen.Visibility = Visibility.Collapsed
                btnOffnenPower.Visibility = Visibility.Collapsed
            End If
            If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.DOC Then
                If detailsTools.istIrgendeinDokumentGeoeffnet(DokumentenTyp.DOC) Then
                    btnOffnen.Visibility = Visibility.Collapsed
                    btnOffnenPower.Visibility = Visibility.Collapsed
                End If
            End If
        Else
            btnWordReadonly.Visibility = Visibility.Collapsed
            btnOffnen.Content = "Öffnen"
            textcontrolImArchivOeffnen.Visibility = Visibility.Collapsed
            btnOffnen.Height = 50
        End If
        If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.XLS Then
            btnExcelVerbot.Visibility = Visibility.Collapsed
            If myGlobalz.sitzung.aktDokument.revisionssicher Then
                btnOffnen.Visibility = Visibility.Collapsed
                btnExcelVerbot.Visibility = Visibility.Visible
                btnExcelVerbot.Content = "---"
                ExceldateirevisionsischerHinweis.Visibility = Visibility.Visible
            End If
            If detailsTools.istIrgendeinDokumentGeoeffnet(DokumentenTyp.XLS) Then
                btnOffnen.Visibility = Visibility.Collapsed
                btnExcelVerbot.Visibility = Visibility.Visible

            End If
        End If

        If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.MSG Or
            myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.EML Then
            myimagerefresh.Visibility = Visibility.Collapsed
            tbMSGTEXT.Visibility = Visibility.Visible
            tbMSGTEXT.Text = ad.thumbnailMSGtext
        Else
            If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.TXT Then
                myimagerefresh.Visibility = Visibility.Collapsed
                tbMSGTEXT.Visibility = Visibility.Visible
                tbMSGTEXT.Text = detailsTools.getTextINhalt(ad.makeFullname_ImArchiv(myGlobalz.Arc.rootDir))
            End If
            If Not datumThumbnail.IsNothingOrEmpty Then
                tbFilename.Text = tbFilename.Text + Environment.NewLine &
                  "Vorschau erzeugt: " & datumThumbnail
                If istveraltet() Then
                    tbFilename.Text = tbFilename.Text + Environment.NewLine &
                                     "Vorschau ist  aktuell ! "
                Else
                    tbFilename.Text = tbFilename.Text + Environment.NewLine &
                                      "Vorschau ist NICHT aktuell ! "
                    myimagerefresh.Opacity = 10
                    myCanvas.Background = Brushes.DarkRed
                End If
            End If
        End If
        makeTextForButton()
        gastLayout()
        e.Handled = True
    End Sub

    Private Shared Function isArchivberechtigt() As Boolean
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "feinen_j" Then Return True
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "weyers_g" Then Return True
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "ploesser_b" Then Return True
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "nehler_u" Then Return True
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "thieme_m" Then Return True
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "kroemmelbein_m" Then Return True
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "kuhn_p" Then Return True
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower = "stich_k" Then Return True
        Return False
    End Function

    Private Function istveraltet() As Boolean
        Dim datThumnail As Date
        Try
            datThumnail = CDate(datumThumbnail)
            Return myGlobalz.sitzung.aktDokument.Filedatum < datThumnail
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub btnOffnen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnOffnen.Click
        e.Handled = True
        knopfnummer = 1
        Me.Close()
    End Sub

    Private Sub btnBeschreibung_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBeschreibung.Click
        e.Handled = True
        knopfnummer = 2
        Me.Close()
    End Sub



    Private Sub btnMailen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnMailen.Click
        e.Handled = True
        knopfnummer = 4
        Me.Close()
        '  Archiv_DokumentMailen()
    End Sub

    Private Sub btnnachOkopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnnachOkopieren.Click
        knopfnummer = 5
        '  NachZielKopieren("o:")
        Me.Close()
    End Sub

    Private Sub btnnachCkopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnnachCkopieren.Click
        Me.Close()
        knopfnummer = 6
        '  NachZielKopieren(System.Environment.SpecialFolder.MyDocuments.ToString)
    End Sub

    'Private Sub btnnachPkopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    MsgBox("Baustelle")
    'End Sub


    Private Sub Window_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

    End Sub

    'Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    e.Handled = True
    '    knopfnummer = 3
    '    Me.Close()
    'End Sub


    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschen.Click
        displayDokument("O:\UMWELT\B\GISDatenEkom\div\showup\showupeulen2.jpg")
        'mybitmap.UriSource = New Uri("O:\UMWELT -PARADIGMA\div\showup\showupeulen2.jpg")
        'mybitmap.StreamSource = Nothing
        'mybitmap = Nothing
        'myimagerefresh.Source = Nothing
        'myimagerefresh.DataContext = Nothing
        'myimagerefresh = Nothing
        GC.Collect()
        Threading.Thread.Sleep(1000)
        knopfnummer = 3

        'If myimagerefresh IsNot Nothing Then

        'End If
        e.Handled = True
        Me.Close()
    End Sub

    Private Shared Function GetPicturePath() As String
        'JPG = 1
        'PDF = 2
        'DOC = 3
        'RTF = 4
        'TXT = 5
        'ZIP = 6
        'DGN = 7
        'XLS = 8
        'VCF = 9
        'GIF = 10
        'MSG = 11
        'GA = 12 ' 3ga
        'AVI = 13
        'MPG = 14
        'UNKNOWN = 99

        Dim path As String = ""
        path = "/paradigma;component/details/icons/"
        Select Case CInt(myGlobalz.sitzung.aktDokument.DokTyp)
            Case 1
                path = path & "foto.jpg"
                DateitypeText = "Foto"
            Case 2
                path = path & "pdf.jpg"
                DateitypeText = "Portable Document Format"
            Case 3
                path = path & "doc.jpg"
                DateitypeText = "Textverarbeitungsformat"
            Case 4
                path = path & "rtf.jpg"
                DateitypeText = "Textverarbeitungsformat"
            Case 5
                path = path & "doc.jpg"
                DateitypeText = "Textverarbeitungsformat"
            Case 6
                path = path & "zip.jpg"
                DateitypeText = "Dateienpaket"
            Case 7
                path = path & "unbekannt.jpg"
            Case 8
                path = path & "xls.jpg"
                DateitypeText = "Tabellenkalkulationsformat"
            Case 9
                path = path & "vcf.jpg"
                DateitypeText = "Elektronische Visitenkarte"
            Case 10
                path = path & "foto.jpg"
                DateitypeText = "Foto / Grafik"
            Case 11
                path = path & "outlook.jpg"
                DateitypeText = "eMail"
            Case 12
                path = path & "microfon.jpg"
                DateitypeText = "Audiodatei"
            Case 13
                path = path & "avi.jpg"
                DateitypeText = "Videodatei"
            Case 14
                path = path & "avi.jpg"
                DateitypeText = "Videodatei"
            Case 15
                path = path & "xls.jpg"
                DateitypeText = "Tabellenkalkulationsformat"
        End Select
        Return path
    End Function
    Private Shared Function GetText() As String
        Return vbCrLf & "Typ des Dokumentes: " & DateitypeText & vbCrLf & vbCrLf &
                  myGlobalz.sitzung.aktDokument.DateinameMitExtension & vbCrLf &
                "Schlagworte: " & myGlobalz.sitzung.aktDokument.Beschreibung & vbCrLf &
                "Revisionssicher: " & GetTextExtracted() & vbCrLf &
                "Quelle: " & myGlobalz.sitzung.aktDokument.Initiale & vbCrLf &
                "Aufnahme ins Archiv: " & myGlobalz.sitzung.aktDokument.Checkindatum & vbCrLf &
                "Letzte Änderung: " & myGlobalz.sitzung.aktDokument.Filedatum & vbCrLf
    End Function
    Private Shared Function GetTextExtracted() As String
        If myGlobalz.sitzung.aktDokument.revisionssicher Then
            Return "ja"
        Else
            Return "nein"
        End If

    End Function


    Private Sub btnMailenAnBa_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        knopfnummer = 7
        Me.Close()
    End Sub

    Private Sub btnimArchivkopieren_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 8
        Me.Close()
    End Sub

    Private Sub btnsofortdruck_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 9
        Me.Close()
    End Sub

    Private Sub btnRevisionssichern_Click(sender As Object, e As RoutedEventArgs)
        knopfnummer = 10
        e.Handled = True
        Me.Close()
    End Sub

    'Private Sub wbMousedown(sender As Object , e As MouseButtonEventArgs)
    '        MsgBox("md")
    'e.Handled=true
    '    End Sub

    Private Sub btnDokumentZuEreignis_Click(sender As Object, e As RoutedEventArgs)
        knopfnummer = 11
        e.Handled = True
        Me.Close()
    End Sub

    Private Sub btnEreignisErstellen_Click(sender As Object, e As RoutedEventArgs)
        knopfnummer = 12
        e.Handled = True
        Me.Close()
    End Sub

    Private Sub displayImageExtracted(datei As String)
        Try
            Dim bmi As New BitmapImage()
            bmi.BeginInit()
            bmi.CacheOption = BitmapCacheOption.OnLoad
            bmi.UriSource = New Uri(datei)
            bmi.EndInit()
            myimagerefresh.Source = bmi
            bmi = Nothing
        Catch ex As Exception
            nachricht("fehler in displayImageExtracted", ex)
        End Try
    End Sub
    Private Sub displayDokument(datei As String)
        If istRasterBild(myGlobalz.sitzung.aktDokument.DateinameMitExtension) Then
            displayImageExtracted(datei) 'ihah problem beim löschen der datei
        Else
            If istPDF(myGlobalz.sitzung.aktDokument) Then
                datumThumbnail = displayPDFTumbnails(myGlobalz.sitzung.aktDokument)
            End If
            If istWordDatei(myGlobalz.sitzung.aktDokument) Then
                datumThumbnail = displayPDFTumbnails(myGlobalz.sitzung.aktDokument)
                ' displayWordUndPdtTumbnails(myglobalz.sitzung.aktDokument)
            End If
        End If
    End Sub

    Private Function displayPDFTumbnails(aktDokument As Dokument) As String
        Dim datei As String
        Dim crt As Date
        'datei = "\\file-paradigma\paradigma\test\thumbnails\" & myglobalz.sitzung.aktVorgangsID & "\" & aktDokument.DocID & ".jpg"
        datei = initP.getValue("Haupt.ThumbNailsRoot") & myGlobalz.sitzung.aktVorgangsID & "\" & aktDokument.DocID & ".jpg"
        Dim fi As New IO.FileInfo(datei)
        If fi.Exists Then
            displayImageExtracted(datei)
            crt = fi.CreationTime
            fi = Nothing
            Return crt.ToString()
        Else
            fi = Nothing
            Return ""
        End If
    End Function

    Private Function istPDF(aktDokument As Dokument) As Boolean
        If aktDokument.DateinameMitExtension.ToLower.EndsWith(".pdf") Then Return True
        Return False
    End Function

    'Private Sub displayWordUndPdtTumbnails(aktDokument As Dokument)
    '    Dim datei As String
    '    datei = "\\w2gis02\gdvell\cache\paradigma\thumbnails\" & myGlobalz.sitzung.aktVorgangsID & "\" & aktDokument.DocID & ".png"
    '    Dim fi As New IO.FileInfo(datei)
    '    If fi.Exists Then
    '        fi = Nothing
    '        displayImageExtracted(datei)
    '    Else
    '        fi = Nothing
    '    End If
    'End Sub

    Private Function istRasterBild(p1 As String) As Boolean
        If p1.IsNothingOrEmpty Then Return False
        If p1.ToLower.EndsWith(".jpg") Then Return True
        If p1.ToLower.EndsWith(".png") Then Return True
        If p1.ToLower.EndsWith(".gif") Then Return True
        If p1.ToLower.EndsWith(".tif") Then Return True
        Return False
    End Function


    Private Sub btnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub makeTextForButton()
        btnDokumentZuEreignis.ToolTip = "Rufen sie zuerst das gewünschte Ereignis auf, " & Environment.NewLine &
            "schliessen Sie es sofort wieder." & Environment.NewLine &
            "Drücken Sie nun diese Taste um die Zuordnung zu erstellen. " & Environment.NewLine &
            "(Sie können so auch mehrere Dokumente dem letzten Ereignis zuordnen)" & Environment.NewLine &
              Environment.NewLine &
               Environment.NewLine &
             "Letztes Ereignis ist:" & Environment.NewLine &
              myGlobalz.sitzung.aktEreignis.Art & ": " & myGlobalz.sitzung.aktEreignis.Beschreibung
    End Sub

    Private Sub btnimArchivNachPDFkopieren_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 13
        Me.Close()
    End Sub

    'Private Sub btnAbbruch_Click_1(sender As Object, e As RoutedEventArgs)
    '    e.Handled = True
    '    knopfnummer = 14
    '    Me.Close()
    'End Sub

    Private Sub btnWordReadonly_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True 'PDF
        readOnlyDoxsInTxtCrtlOeffnenLOKAL = False
        knopfnummer = 14
        Me.Close()
    End Sub
    Private Sub btnWordReadonlyTXTCTL_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 14
        readOnlyDoxsInTxtCrtlOeffnenLOKAL = True
        Me.Close()
    End Sub
    Private Sub btnExcelVerbot_Click(sender As Object, e As RoutedEventArgs)
        'MsgBox("")
        e.Handled = True
    End Sub

    Private Sub btnOffnenPower_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 15
        If radioAlsDOC.IsChecked Then WordDateityp = ".doc"
        If radioAlsDOCX.IsChecked Then WordDateityp = ".docx"
        If radioAlsRTF.IsChecked Then WordDateityp = ".rtf"
        Me.Close()
    End Sub

    Private Sub btnBackupVerlaufanzeigen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 16
        Me.Close()
    End Sub

    Private Sub BtnNachConject_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 17
        Me.Close()
    End Sub


End Class
