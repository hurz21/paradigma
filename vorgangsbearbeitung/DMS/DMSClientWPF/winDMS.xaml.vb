Public Class winDMS
    'Property debugmode As Boolean = False
    Property _quellDatei As String
    Property _archivname As String
    Property _relativdir As String
    Property _numDir As String
    Property _erfolgreich As Boolean
    Property _aktion As String

    Sub New(quellDatei As String,
            archivname As String,
            relativdir As String,
            numdir As String,
            aktion As String)
        InitializeComponent()
        _archivname = archivname
        _relativdir = relativdir
        _numDir = numdir
        _aktion = aktion
    End Sub

    Public Sub DMSPrep()
        exchangeRoot = "" 'myGlobalz.ExchangeRoot '"l:\paradigma\div\DMSexchange\" ist bis auf weiteres überflüssig
        DMSserverUrl = initP.getValue("GisServer.URLdmsServer")
        aktDMSJob.username = myGlobalz.sitzung.aktBearbeiter.Name '"feinen_j"
        'Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        createVIDverzeichnis()
        'initPUT()
        'initDelete()
        'initGet()
        initUpdate()
    End Sub
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        DMSPrep()
        ' Close()
    End Sub


    Private Sub initPUT()
        Dim inputfiles As String()
        Dim istKopiert As Boolean
        Dim istListeErzeugt As Boolean
        '
        'eingabeparameter setzen
        aktDMSJob.vid = "4711"
        aktDMSJob.aktion = "put"
        aktDMSJob.relativdir = "2013/4711"
        aktDMSJob.numericSubDir = "" 'wird vom server ermittelt
        inputfiles = buildInputfilesArrayPut()

        createVIDverzeichnis()
        ExchangeDirLeeraeumen(VIDdir)

        istKopiert = KopiereDokumenteNachExchange("", VIDdir)
        istListeErzeugt = erzeugeFileListe(aktDMSJob.inputfiles, aktDMSJob.aktion)
        Dim url As String = aktDMSJob.buildHttpString(DMSserverUrl)
        'Darstellung init-------------------------------------
        tbAktion.Text = aktDMSJob.aktion
        tbDateien.Text = aktDMSJob.FileAbstract(Environment.NewLine)
        '------------------------------
        Dim antwortstring As String = sendjobURL(url)
        DMSergebnis.getergebnis(antwortstring)
        If DMSergebnis.status = "ok" Then
            tbStatus.Text = DMSergebnis.status & " Dateien wurden übernommen"
            'der relativpfad muss nun an die DB weitergegeben werden
        Else
            tbStatus.Text = DMSergebnis.status & " Dateien wurden NICHT übernommen"
        End If
        tbReturn.Text = DMSergebnis.inhalt
        tbWarten.Text = "Job done."
        GbErgebnis.Visibility = Windows.Visibility.Visible
        tbFehler.Text = DMSergebnis.errortext
        tbDateienAnzahl.Text = DMSergebnis.numberOfFiles.ToString
    End Sub



    Private Sub initDelete()
        'löschen im Filesystem erfordert die root,dokumentpfad und dateinamemitextension (fullnameimArchiv)
        Dim istListeErzeugt As Boolean
        aktDMSJob.username = "feinen_j"
        aktDMSJob.vid = "4711"
        aktDMSJob.aktion = "delete"
        aktDMSJob.relativdir = "2013/4711"
        aktDMSJob.numericSubDir = "9"
        cleandokumentpfadString()
        createVIDverzeichnis()

        aktDMSJob.inputfiles = buildInputfilesArrayDelete(aktDMSJob.relativdir, aktDMSJob.numericSubDir) 'hier aufgehört

        'aktJob.inputfiles muss die dateinamen mit vorangestellten subdir und relativpfad enthalten
        istListeErzeugt = erzeugeFileListe(aktDMSJob.inputfiles, aktDMSJob.aktion)
        Dim url As String = aktDMSJob.buildHttpString(DMSserverUrl)
        'Darstellung init-------------------------------------
        tbAktion.Text = aktDMSJob.aktion
        tbDateien.Text = aktDMSJob.FileAbstract(Environment.NewLine)

        Dim antwortstring As String = sendjobURL(url)
        DMSergebnis.getergebnis(antwortstring)
        If DMSergebnis.status = "ok" Then
            tbStatus.Text = DMSergebnis.status & DMSergebnis.numberOfFiles & " Dateien wurden gelöscht"
        Else
            tbStatus.Text = DMSergebnis.status & " Dateien wurden NICHT übernommen"
        End If
        tbReturn.Text = DMSergebnis.inhalt
        tbWarten.Text = "Job done."
        GbErgebnis.Visibility = Windows.Visibility.Visible
        tbFehler.Text = DMSergebnis.errortext
        tbDateienAnzahl.Text = DMSergebnis.numberOfFiles.ToString
    End Sub

    Private Sub initGet() 'immer nur für eine einzige Datei, bulk-version: getBulk später bis dahin konventionell
        'erfordert 
        '  rootDir,          dokumentPfad,         DateinameMitExtension
        'macht immer checkout über makefullnameIMarchiv
        Dim istListeErzeugt As Boolean
        aktDMSJob.username = "feinen_j"
        aktDMSJob.vid = "4711"
        aktDMSJob.aktion = "get"

        createVIDverzeichnis()
        aktDMSJob.inputfiles = buildInputfilesArrayGET() 'hier aufgehört
        '##################### _files_.txt #################
        'beispiel
        '2013\4711\6\Liebe Kolleginnen und Kollegen.docx
        '2013\4711\6\oracleInfo.docx
        '##############################
        istListeErzeugt = erzeugeFileListe(aktDMSJob.inputfiles, "get")

        'Darstellung init-------------------------------------
        tbAktion.Text = aktDMSJob.aktion
        tbDateien.Text = aktDMSJob.FileAbstract(Environment.NewLine)
        '------------------------------
        Dim url As String = aktDMSJob.buildHttpString(DMSserverUrl)
        Dim antwortstring As String = sendjobURL(url)
        DMSergebnis.getergebnis(antwortstring)
        If DMSergebnis.status = "ok" Then
            tbStatus.Text = DMSergebnis.status & " Dateien wurden geliefert"
            'der relativpfad muss nun an die DB weitergegeben werden
        Else
            tbStatus.Text = DMSergebnis.status & " Dateien wurden NICHT geliefert"
        End If
        tbReturn.Text = DMSergebnis.inhalt
        tbWarten.Text = "Job done."
        GbErgebnis.Visibility = Windows.Visibility.Visible
        tbFehler.Text = DMSergebnis.errortext
        tbDateienAnzahl.Text = DMSergebnis.numberOfFiles.ToString
    End Sub

    Private Sub initUpdate()
        Dim istListeErzeugt As Boolean
        aktDMSJob.username = "feinen_j"
        aktDMSJob.vid = "4711"
        aktDMSJob.aktion = "update"
        aktDMSJob.relativdir = "2013/4711"
        aktDMSJob.numericSubDir = "10"
        cleandokumentpfadString()
        createVIDverzeichnis()

        aktDMSJob.inputfiles = buildInputfilesArrayPut()

        '2013\4711\10\Liebe Kolleginnen und Kollegen.docx
        '2013\4711\10\oracleInfo.docx

        'zuerst löschen über die fileliste !!! kann man sich doch sparen
        ' aktJob.inputfiles = buildInputfilesArrayDelete(FileArchiv.aktJob.relativdir, FileArchiv.aktJob.numericSubDir) 'hier aufgehört
        'aktJob.inputfiles muss die dateinamen mit vorangestellten subdir und relativpfad enthalten
        istListeErzeugt = erzeugeFileListe(aktDMSJob.inputfiles, aktDMSJob.aktion)
        'dann putten als quelldateien !!!

        Dim istKopiert = KopiereDokumenteNachExchange("", VIDdir)
        'Darstellung init-------------------------------------
        tbAktion.Text = aktDMSJob.aktion
        tbDateien.Text = aktDMSJob.FileAbstract(Environment.NewLine)
        '------------------------------
        Dim url As String = aktDMSJob.buildHttpString(DMSserverUrl)
        Dim antwortstring As String = sendjobURL(url)
        DMSergebnis.getergebnis(antwortstring)
        If DMSergebnis.status = "ok" Then
            tbStatus.Text = DMSergebnis.status & " Dateien wurden erneuert"
            'der relativpfad muss nun an die DB weitergegeben werden
        Else
            tbStatus.Text = DMSergebnis.status & " Dateien wurden NICHT erneuert"
        End If
        tbReturn.Text = DMSergebnis.inhalt
        tbWarten.Text = "Job done."
        GbErgebnis.Visibility = Windows.Visibility.Visible
        tbFehler.Text = DMSergebnis.errortext
        tbDateienAnzahl.Text = DMSergebnis.numberOfFiles.ToString
    End Sub


    Private Sub btnWeiter_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

End Class

