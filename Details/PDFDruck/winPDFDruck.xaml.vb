Public Class winPDFDruck

    Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub
    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        btnPDFdruck.Content = "Bitte warten ..."
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        PDFDruckabschicken(30000)
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub PDFDruckabschicken(dauerInSekunden As Int16)

        Dim aaa As New clsAufrufgenerator
        aaa.Domainstring = initP.getValue("GisServer.ServerHTTPdomainIntranet") '  
        Dim AusgabeRootDir As String = initP.getValue("Myglobalz.paradigmaCache")
        detailsTools.ErzeugeUnterVerzeichnisse(AusgabeRootDir)
        detailsTools.MapfileTemplateBearbeiten(AusgabeRootDir)

        aaa.aktMap = clstart.myc.kartengen.aktMap
        aaa.aktMap.Vgrund = clstart.myc.kartengen.aktMap.Vgrund
        aaa.aktMap.Hgrund = clstart.myc.kartengen.aktMap.Hgrund
        aaa.mapcred.username = myGlobalz.sitzung.aktBearbeiter.username
        aaa.mapcred.pw = clstart.myc.kartengen.mapcred.pw
        aaa.aktMap.aktrange.xl = clstart.myc.kartengen.aktMap.aktrange.xl
        aaa.aktMap.aktrange.xh = clstart.myc.kartengen.aktMap.aktrange.xh
        aaa.aktMap.aktrange.yl = clstart.myc.kartengen.aktMap.aktrange.yl
        aaa.aktMap.aktrange.yh = clstart.myc.kartengen.aktMap.aktrange.yh

        clstart.myc.kartengen.mapcred.DateinamensSchwanz = "ParadigmaPDF" 'clsMiniMapTools.makeOutfileschwanz
        aaa.mapcred.DateinamensSchwanz = clstart.myc.kartengen.mapcred.DateinamensSchwanz 'clsMiniMapTools.makeOutfileschwanz
        clstart.myc.kartengen.genOutfileFullName(initP.getValue("GisServer.gisCacheDir"), ".pdf")
        Dim kas As String = aaa.genaufruf4PDF(tbOrtsname.Text, tbBemerkung.Text, getRadioMethodeZustand, getmitlegende, getmitdoku)
        glob2.webmapBrows(kas)

        Dim pdfZielFilename As String
        pdfZielFilename = detailsTools.GetPdfZielFilename()
        System.Threading.Thread.Sleep(dauerInSekunden)
        glob2.OpenDocument(pdfZielFilename)
        If CBool(chkEreignisMap.IsChecked) Then
            PDFDruckTool.PDFKarteEreignisErzeugen(tbBeschreibung.Text, pdfZielFilename)
        Else
            PDFDruckTool.PDFKarteInsArchiv(tbBeschreibung.Text, pdfZielFilename)
        End If

    End Sub

    Private Sub winPDFDruck_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim ortsteil As String = getOrtsteil()
        Dim bemerkung As String = getbemerkung()
        tbOrtsname.Text = ortsteil
        tbBemerkung.Text = bemerkung
        e.Handled = True
    End Sub
    Private Function getOrtsteil() As String
        Dim temp As String
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ) Then
            temp = ""
        Else
            temp = myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ
        End If
        Return temp
    End Function

    Private Function getbemerkung() As String
        Return "Paradigma: " & myGlobalz.sitzung.aktVorgangsID
    End Function

    Private Function getRadioMethodeZustand() As String
        If klassisch.IsChecked Then
            Return "0"
        End If
        Return "1"
    End Function

    Private Function getmitlegende() As String
        If chkmitLegende.IsChecked Then
            Return "1"
        Else
            Return "0"
        End If
    End Function

    Private Function getmitdoku() As String
        If chkmitDoku.IsChecked Then
            Return "1"
        Else
            Return "0"
        End If
    End Function

    Private Sub neu_Checked(sender As Object, e As RoutedEventArgs) Handles neu.Checked
        grpZusatz.Visibility = Windows.Visibility.Hidden
        e.Handled = True
    End Sub

    Private Sub klassisch_Checked(sender As Object, e As RoutedEventArgs) Handles klassisch.Checked
        If grpZusatz Is Nothing Then
            e.Handled = True
            Exit Sub

        End If
        grpZusatz.Visibility = Windows.Visibility.Visible
        e.Handled = True
    End Sub

    Private Sub chkEreignisMap_Click_1(sender As Object, e As RoutedEventArgs)
        chkEreignisMap_CheckedExtracted()
        e.Handled = True
    End Sub

    Private Sub chkEreignisMap_CheckedExtracted()
        If chkEreignisMap.IsChecked Then
            'Me.Height = 460
            'grpEreignis.Height = 125
            tbBeschreibung.IsEnabled = True
            If tbBeschreibung.Text.IsNothingOrEmpty Then
                tbBeschreibung.Text = "Pdf-Karte"
            End If
        Else
            'Me.Height = 460
            'grpEreignis.Height = 25
        End If
    End Sub
    Private Sub chkEreignisMap_Checked(sender As Object, e As RoutedEventArgs) Handles chkEreignisMap.Checked
        chkEreignisMap_CheckedExtracted()
        e.Handled = True
    End Sub
End Class
