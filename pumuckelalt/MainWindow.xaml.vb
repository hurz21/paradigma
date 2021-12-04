Imports System.ComponentModel
Imports System.IO
Imports wpfDokumentUpdate
Imports System.Data.SqlClient

Class MainWindow
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\232475" tempEditDatei="~$07-9609-12_2017-09-28_074503.docx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\232475\3307-9609-12_2017-09-28_074503.docx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\9609\232475" revisionssicher="False" dokid="232475" vid="9609"
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074" tempEditDatei="~$Massstab.xlsx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074\Massstab.xlsx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\9609\233074" revisionssicher="False" dokid="0" vid="9609"
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074" tempEditDatei="~$Massstab.xlsx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074\Massstab.xlsx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\9609\233074" revisionssicher="False" dokid="0" vid="9609"
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\26352\235003" tempEditDatei="~$STAENDIGKEITEN_FUER_HAUSNUMMERN.docx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\26352\235003\ZUSTAENDIGKEITEN_FUER_HAUSNUMMERN.docx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\26352\235003" revisionssicher="False" dokid="235003" vid="26352"


    Public Property VorgangREC() As IDB_grundfunktionen
    Public Shared vorgang_MYDB As New clsDatenbankZugriff
    Public Property kompressed As Boolean = False
    Property dokid As Integer
    Property geloeschteDatei As String
    Property vid As Integer
    Property watchpfad As String
    Property wordDocWatcher As FileSystemWatcher
    Property tempEditDatei As String
    Public Property FullnameCheckout As String
    Public Property FullnameImArchiv As String
    Public Property revisionssicher As Boolean
    Public dateinamemitextension As String = ""
    Public kandidat As Dokument
    'Public sw As IO.StreamWriter
    Property qfile As String
    Public Property sleeptime As Integer = 1 '8000

    Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        With My.Log.DefaultFileLogWriter
            .BaseFileName = "c:\kreisoffenbach\pumuckel\pumuckel_" & gettimestamp()
            .AutoFlush = True
            .Append = False
        End With
        qfile = My.Log.DefaultFileLogWriter.FullLogFileName
        Dim arguments As String()

        Try
            l("MainWindow_Loaded---------------------- anfang")
            arguments = Environment.GetCommandLineArgs()
            l("arguments.Count: " & arguments.Count)
            Threading.Thread.Sleep(sleeptime)
            mapAllArguments(arguments)
            tbvid.Text = vid.ToString

            Dim fi As New IO.FileInfo(FullnameCheckout)
            Dim ftemp As New IO.FileInfo(watchpfad & "\" & tempEditDatei)
            'If ftemp.Exists Then
            If fi.Exists Then
                dateinamemitextension = fi.Name
                geloeschteDatei = watchpfad & "\" & tempEditDatei
                tbvid.Text = vid.ToString
                tbfilename.Text = fi.Name
                Title = "Pumuckel: " & vid.ToString & ", " & fi.Name
                fi = Nothing
                watch(dateinamemitextension)


            Else
            End If
            'Else
            '    tbInfo.Text = Environment.NewLine & Environment.NewLine & fi.Name & Environment.NewLine &
            '         Environment.NewLine & Environment.NewLine &
            '      "Achtung die Datei ist schreibgeschützt!!!" & Environment.NewLine & Environment.NewLine &
            '      "Entfernen sie zuerst den Schreibschutz (Bearbeitung aktivieren)" & Environment.NewLine &
            '      "und schließen Sie die Datei wieder." & Environment.NewLine &
            '      "Dann erst erneut über Paradigma öffnen und bearbeiten." & Environment.NewLine &
            '           Environment.NewLine &
            '       "Bei Nicht-Beachtung besteht die Gefahr des Datenverlustes." & Environment.NewLine
            '    MessageBox.Show(tbInfo.Text, "Pumuckel warnt:  W I C H T I G !!!!! ", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '    Title = "Pumuckel is NOT watching: " & FullnameCheckout
            '    End
            'End If




            l("MainWindow_Loaded---------------------- ende")
        Catch ex As Exception
            l("Fehler in MainWindow_Loaded: " & ex.ToString())
        End Try
        e.Handled = True
    End Sub

    Private Function gettimestamp() As String
        Return Now.ToString("yyyyMMdd_mmss")
    End Function

    Private Sub watch(dateiname As String)
        Try
            nachricht("watch---------------------- anfang")
            wordDocWatcher = New FileSystemWatcher
            wordDocWatcher.Path = watchpfad
            wordDocWatcher.Filter = tempEditDatei
            AddHandler wordDocWatcher.Deleted, AddressOf OnDeletedFileSystemCacheWord
            ' Begin watching.
            wordDocWatcher.EnableRaisingEvents = True
            tbInfo.Text = "Dieses Programm (Pumuckel) achtet darauf, dass die Word - Dateien nachdem " &
                "Sie sie geschlossen haben  - wieder zurück ins Archiv gespeichert werden." &
                "Sie sollten dieses Programm nicht abbrechen und in Word niemals die Funktion 'Speichern unter' verwenden!" &
                Environment.NewLine & Environment.NewLine & FullnameCheckout & Environment.NewLine &
                " wird im Moment bearbeitet und beobachtet." & Environment.NewLine & Environment.NewLine &
              "  Dieses Fenster schließt sich automatisch sobald Sie das Dokument schließen"
            nachricht("watch---------------------- ende")
        Catch ex As Exception
            nachricht("Fehler in watch: " & ex.ToString())
        End Try
    End Sub




    Friend Sub mapAllArguments(arguments() As String)
        Try
            'dokid=   vid=9609 geloeschtedatei= 
            nachricht("mapAllArguments---------------------- anfang")
            For Each sttelement In arguments
                If sttelement.Contains("dokid=") Then
                    dokid = CInt(sttelement.Replace("dokid=", "").Trim.ToLower)
                    nachricht("dokid " & dokid)
                End If
                If sttelement.Contains("kompress=") Then
                    kompressed = CBool(CInt(sttelement.Replace("kompress=", "").Trim.ToLower))
                    nachricht("kompressed " & dokid)
                End If
                If sttelement.Contains("vid=") Then
                    vid = CInt(sttelement.Replace("vid=", "").Trim.ToLower)
                    nachricht("vid " & vid)
                End If
                If sttelement.Contains("watchPfad=") Then
                    watchpfad = sttelement.Replace("watchPfad=", "").Trim.ToLower
                    nachricht("watchpfad " & watchpfad)
                End If
                If sttelement.Contains("tempEditDatei=") Then
                    tempEditDatei = sttelement.Replace("tempEditDatei=", "").Trim
                    nachricht("tempEditDatei " & tempEditDatei)
                End If

                If sttelement.Contains("FullnameCheckout=") Then
                    FullnameCheckout = sttelement.Replace("FullnameCheckout=", "").Trim.ToLower
                    nachricht("FullnameCheckout " & FullnameCheckout)
                End If
                If sttelement.Contains("FullnameImArchiv=") Then
                    FullnameImArchiv = sttelement.Replace("FullnameImArchiv=", "").Trim.ToLower
                    nachricht("FullnameImArchiv " & FullnameImArchiv)
                End If
                If sttelement.Contains("revisionssicher=") Then
                    revisionssicher = CBool(sttelement.Replace("revisionssicher=", "").Trim.ToLower)
                    nachricht("revisionssicher " & revisionssicher)
                End If
            Next
            nachricht("mapAllArguments---------------------- ende")
        Catch ex As Exception
            nachricht("Fehler in mapAllArguments: " & ex.ToString())
        End Try

        'modus=probaug suchmodus=flurstueck gemarkung=dietzenbach flur=5 fstueck=490/0"
    End Sub
    Sub nachricht(text As String)
        'text = "fehler " & text
        My.Log.WriteEntry(text)
        If text.ToLower.Contains("fehler") Then

            Dim test As Boolean = clsMailsenden.mailrausSMTP("dr.j.feinen@kreis-offenbach.de", "dr.j.feinen@kreis-offenbach.de",
                                                  "" & " in pumuckel, Vorgang: " & vid &
                                                  ", Initial: " & "",
                                                  text.Replace(vbCrLf, "<br>"),
                                                  "", False, "", "", "")
        End If
        '  sw.WriteLine(text)
    End Sub
    Public Function istIrgendeinDokumentGeoeffnet(doktyp As DokumentenTyp) As Boolean
        Try
            Return True
            If doktyp = DokumentenTyp.DOC Then
                If String.IsNullOrEmpty(dateinamemitextension) Then
                    Return False
                Else
                    Return True
                End If
            End If

            Return False
        Catch ex As Exception
            nachricht("istIrgendeinDokumentGeoeffnet " & vbCrLf & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function
    Private Function getVersionsBackupName(fullnameCheckout As String, name As String, extension As String,
                                           directory As String) As String
        Dim versname As String = ""
        Try
            versname = name.ToLower
            If extension Is Nothing OrElse String.IsNullOrEmpty(extension) Then
                versname = versname
                versname = versname & "_VersBackup_" & Now.ToString("yyyyMMddhhmmss_ffff")
            Else
                versname = versname.Replace(extension, "")
                versname = versname & "_VersBackup_" & Now.ToString("yyyyMMddhhmmss_ffff") & "." & extension
            End If
            '   versname = directory & "\" & versname die My.Computer.FileSystem.RenameFile meth benötigt hier kei directory
            Return versname
        Catch ex As Exception
            nachricht("fehler in getVersionsBackupName: " & fullnameCheckout & ex.ToString)
            '  Return False
            Return "fehler"
        End Try
    End Function
    Function getDateiistneuerAlsImArchiv(fullnamecheckout As String, FullnameImArchiv As String) As Boolean
        Dim CO_test, AR_test As IO.FileInfo
        Try
            l("getDateiistneuerAlsImArchiv---------------------- anfang")
            l("fullnamecheckout--- " & fullnamecheckout)
            l("FullnameImArchiv--- " & FullnameImArchiv)
            CO_test = New IO.FileInfo(fullnamecheckout)
            AR_test = New IO.FileInfo(FullnameImArchiv)

            nachricht("Alt: " & AR_test.LastWriteTime.ToString)
            nachricht("Neu: " & CO_test.LastWriteTime.ToString)
            Return AR_test.LastWriteTime < CO_test.LastWriteTime
            ' nachricht("dateiWurdeGeaendert: " & dateiIstNeuerAlsImArchiv)
            'AR_test = Nothing ' sonst läßt sie sich nicht überschreiben
            'CO_test = Nothing 
            Return True
            l("getDateiistneuerAlsImArchiv---------------------- ende")
        Catch ex As Exception
            l("Fehler in getDateiistneuerAlsImArchiv: " & ex.ToString())
            Return False
        End Try
    End Function
    Sub l(text As String)
        nachricht(text)
    End Sub

    Sub bildeKandidatWord(geloeschteDatei As String, ByRef kandidat As Dokument, ByRef hatKandidat As Boolean)
        Try
            l("bildeKandidatWord---------------------- anfang")
            'If geloeschteDatei.ToLower.EndsWith(".doc") Or
            '   geloeschteDatei.ToLower.EndsWith(".docx") Then
            If geloeschteDatei.ToLower.Contains(tempEditDatei.ToLower) Then
                nachricht("gelöschteDatei entspricht der editdatei")
                nachricht("worddatei wurde geschlossen")
                kandidat = New Dokument
                kandidat.FullnameCheckout = FullnameCheckout
                kandidat.FullnameImArchiv = FullnameImArchiv
                kandidat.tempEditDatei = tempEditDatei
                kandidat.DateinameMitExtension = dateinamemitextension
                kandidat.revisionssicher = revisionssicher
                kandidat.kompressed = kompressed
                hatKandidat = True
            End If
            'End If
            l("bildeKandidatWord---------------------- ende")
        Catch ex As Exception
            l("Fehler in bildeKandidatWord: " & ex.ToString())
        End Try
    End Sub
    Public Sub inputFileReadonlyEntfernen(inputfile As String)
        Dim fi As IO.FileInfo
        Try
            fi = New IO.FileInfo(inputfile)
            If fi.Exists Then
                If CBool(fi.Attributes And IO.FileAttributes.ReadOnly) Then
                    ' Datei ist schreibgeschützt
                    ' Jetzt Schreibschutz-Attribut entfernen
                    fi.IsReadOnly = False
                    'fi.Attributes = fi.Attributes Xor IO.FileAttributes.ReadOnly
                End If
            End If
            fi = Nothing
        Catch ex As Exception
            'Debug.Print(ex.ToString)
            nachricht("fehler in inputFileReadonlyEntfernen: (unwichtig)" & inputfile & ex.ToString)
            fi = Nothing
        End Try
    End Sub
    Function ArchivDateiNachVersionBackupUmbenennen(dok As Dokument) As Boolean
        Dim neuerFullName As String
        Dim fi As IO.FileInfo
        Try
            fi = New IO.FileInfo(dok.FullnameImArchiv)
            neuerFullName = getVersionsBackupName(dok.FullnameCheckout, fi.Name, fi.Extension, fi.DirectoryName)
            '  neuerFullName = fi.DirectoryName & "\" & neuerFullName
            fi = Nothing
            nachricht("ArchivDateiNachVersionBackupUmbenennen altername: " & dok.FullnameImArchiv)
            nachricht("ArchivDateiNachVersionBackupUmbenennen neuerFullName: " & neuerFullName)
            My.Computer.FileSystem.RenameFile(dok.FullnameImArchiv, neuerFullName)
            Return True
        Catch ex As Exception
            nachricht("fehler in ArchivDateiNachVersionBackupUmbenennen: " & ex.ToString)
            fi = Nothing
            Return False
        End Try
    End Function
    Public Sub OnDeletedFileSystemCacheWord(ByVal source As Object, ByVal e As FileSystemEventArgs)
        Dim geloeschteDatei As String = e.FullPath.ToString
        Dim hatkandidat As Boolean = False
        Dim dateiIstNeuerAlsImArchiv As Boolean
        Try
            l("OnDeletedFileSystemCacheWord---------------------- anfang")
            nachricht("Start: " & Now)
            If istIrgendeinDokumentGeoeffnet(DokumentenTyp.DOC) Then bildeKandidatWord(geloeschteDatei, kandidat, hatkandidat)
            If Not hatkandidat Then
                ' MsgBox("wurdenDokumenteGeaendert hatKandidat ist false")

            End If
            dateiIstNeuerAlsImArchiv = getDateiistneuerAlsImArchiv(kandidat.FullnameCheckout, kandidat.FullnameImArchiv)
            nachricht("dateiIstNeuerAlsImArchiv: " & dateiIstNeuerAlsImArchiv)
            If kandidat.revisionssicher Then
                nachricht("Dokument ist revisionssicher. Sie können die Änderungen also nicht direkt ins Archiv übernehmen.")
                MessageBox.Show("Dokument ist revisionssicher. Sie können die Änderungen also nicht direkt ins Archiv übernehmen.",
                                        kandidat.DateinameMitExtension, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
                beenden()
            End If

            If dateiIstNeuerAlsImArchiv Then
                Dim CO_test As IO.FileInfo = New IO.FileInfo(kandidat.FullnameCheckout)
                If geaenderteDateiInsArchivUebernehmen(CO_test, kandidat) Then
                    nachricht("geänderte Datei wurde übernommen")
                    'MessageBox.Show("Die geänderte Datei: " & Environment.NewLine & Environment.NewLine &
                    '                     CO_test.Name & Environment.NewLine & Environment.NewLine &
                    '                    "wurde erfolgreich ins Archiv übernommen. " & Environment.NewLine & Environment.NewLine,
                    '                   dateinamemitextension,
                    '                    MessageBoxButton.OK, MessageBoxImage.Information)
                    'Dim backupdatei As String = BackupAnlegen(CO_test, kandidat.DocID)


                Else
                    'Übernahme ins archiv gescheitert
                    'datei sichern
                    'Dim backupdatei As String = BackupAnlegen(CO_test, kandidat.DocID)
                    MessageBox.Show("Die Übernahme ins Archiv ist gescheitert. Bitte sichern Sie die folgende Datei:" & Environment.NewLine &
                                    CO_test.FullName & Environment.NewLine &
                                    "Das Verzeichnis wird nun geöffnet um Ihnen die Sicherung zu erleichtern!",
                                    "Sicherung im Archiv gescheitert",
                                    MessageBoxButton.OK, MessageBoxImage.Error)
                    ')
                    Process.Start(CO_test.DirectoryName)
                End If
                CO_test = Nothing
            Else
                l("Die Datei ist nicht neuer als die im Archiv - keine Aktion. ")
                'MessageBox.Show("Die Datei ist nicht neuer als die im Archiv - keine Aktion. ",
                '               "Übernahme der geänderten Datei ins Archiv", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
            beenden()


            l("OnDeletedFileSystemCacheWord---------------------- ende")
        Catch ex As Exception
            l("Fehler in OnDeletedFileSystemCacheWord: " & ex.ToString())
        End Try
    End Sub

    Private Function geaenderteDateiInsArchivUebernehmen(CHeckOut_test As FileInfo, dok As Dokument) As Boolean
        Try
            l("geaenderteDateiInsArchivUebernehmen ---------------------- anfang")
            nachricht("geaenderteDateiInsArchivUebernehmen: " & dok.FullnameImArchiv)
            If CHeckOut_test.Exists Then
                Dim fullname As String = dok.FullnameImArchiv
                inputFileReadonlyEntfernen(dok.FullnameImArchiv)
                If ArchivDateiNachVersionBackupUmbenennen(dok) Then
                    nachricht("archivdatei umbenannt")
                Else
                    nachricht("fehler archivdatei konnte nicht umbenannt werden (unwichtig)" & dok.FullnameImArchiv)
                End If
                If imarchivSichernErfolgreich(CHeckOut_test, fullname) Then
                    If updateZeitstempel() Then
                        Return True  'weiter zur DB
                    Else
                        Return False  'weiter zur DB
                    End If
                Else
                    Return False
                End If
            End If
            Return True
            l("geaenderteDateiInsArchivUebernehmen---------------------- ende")
        Catch ex As Exception
            l("Fehler in geaenderteDateiInsArchivUebernehmen : " & ex.ToString())
            Return False
        End Try
    End Function



    Private Function imarchivSichernErfolgreich(cHeckOut_test As FileInfo, fullname As String) As Boolean
        nachricht("imarchivSichernErfolgreich---------------------- anfang")
        Try
            If kopieErfolgreich(cHeckOut_test, fullname) Then Return True
            Threading.Thread.Sleep(5000)
            If kopieErfolgreich(cHeckOut_test, fullname) Then Return True
            Threading.Thread.Sleep(5000)
            If kopieErfolgreich(cHeckOut_test, fullname) Then Return True
            Threading.Thread.Sleep(5000)
            If kopieErfolgreich(cHeckOut_test, fullname) Then Return True
            Threading.Thread.Sleep(5000)
            If kopieErfolgreich(cHeckOut_test, fullname) Then Return True
            Threading.Thread.Sleep(5000)
            If kopieErfolgreich(cHeckOut_test, fullname) Then Return True
            l("mehrfach kopierversucht, gescheitert")
            Return False
        Catch ex As Exception
            nachricht("Fehler in imarchivSichernErfolgreich " & ex.ToString)
            Return False
        End Try
    End Function

    Private Function kopieErfolgreich(cHeckOut_test As FileInfo, fullname As String) As Boolean
        Try
            If kompressed And Environment.UserName.ToLower = "feinen_j" Then
                CLstart.AesCrypt.FileEncrypt(cHeckOut_test.FullName, fullname, CLstart.AesCrypt.normpw)
            Else
                cHeckOut_test.CopyTo(fullname, True)
            End If
            'cHeckOut_test.CopyTo(fullname, True)
            nachricht("kopieErfolgreich: " & fullname)
            Return True
            nachricht("kopieErfolgreich---------------------- ende")
        Catch ex2 As Exception
            nachricht("Fehler in kopieErfolgreich: " & ex2.ToString())
            Return False
        End Try
    End Function

    Private Sub beenden()
        ' MsgBox(geloeschteDatei & " wurde gerade geschlossen und im Archiv gespeichert")

        wordDocWatcher.EnableRaisingEvents = False
        wordDocWatcher.Dispose()
        End
    End Sub

    Function updateZeitstempel() As Boolean
        Try
            nachricht("updateZeitstempel ---------------------- anfang")

            nachricht("updateZeitstempel: " & updateZeitstempel)
            'vorgang_MYDB.Host = "ora-clu-vip-003" '  paradigmaServer
            'vorgang_MYDB.Schema = "paradigma"
            'vorgang_MYDB.Tabelle = "vorgang" 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) '"gmde"
            'vorgang_MYDB.ServiceName = "paradigma.kreis-of.local"
            'vorgang_MYDB.username = "paradigma"
            'vorgang_MYDB.password = "luftikus12"
            'vorgang_MYDB.dbtyp = "oracle" 'paradigmadatentyp$ 
            'VorgangREC = New clsDBspecOracle
            '---------------
            vorgang_MYDB.Host = "msql01" '  paradigmaServer
            vorgang_MYDB.Schema = "Paradigma"
            vorgang_MYDB.Tabelle = "vorgang" 'CType(CLstart.mycSimple.iniDict("VorgangDB.Tabelle"), String) '"gmde"
            vorgang_MYDB.ServiceName = ""
            vorgang_MYDB.username = "sgis"
            vorgang_MYDB.password = "WinterErschranzt.74"
            vorgang_MYDB.dbtyp = "sqls" 'paradigmadatentyp$ 
            VorgangREC = New clsDBspecMSSQL
            '---------------
            VorgangREC.mydb = CType(vorgang_MYDB.Clone, clsDatenbankZugriff)
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            querie = "UPDATE dokumente SET " &
                    " FILEDATUM=@FILEDATUM" &
                    " WHERE DOKUMENTID=@DOKUMENTID"
            nachricht("updateZeitstempel: " & querie)
            clsSqlparam.paramListe.Add(New clsSqlparam("FILEDATUM", Now))
            clsSqlparam.paramListe.Add(New clsSqlparam("DOKUMENTID", dokid))
            nachricht("updateZeitstempel: " & "vor manip")

            Dim ianz As Integer = VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "DOKUMENTID")
            nachricht("updateZeitstempel: " & "nach manip")
            If ianz > 0 Then
                Return True
            Else
                Return False
            End If

            Return True
            nachricht("---------------------- ende")
        Catch ex As Exception
            nachricht("Fehler in updateZeitstempel: " & ex.ToString())

            Return False
        End Try

    End Function

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim mesres As New MessageBoxResult
        mesres = MessageBox.Show("Wenn Sie dieses Fenster schließen,  " & Environment.NewLine &
                "verlieren Sie die Änderungen im Worddokument" & Environment.NewLine &
                " (das Fenster schließt sich automatisch sobald Sie das Dokument schließen" & Environment.NewLine & Environment.NewLine &
                "      trotzdem schliessen = JA " & Environment.NewLine &
                "      abbrechen          = NEIN " & Environment.NewLine & Environment.NewLine &
                " " & Environment.NewLine,
                               "Vorsicht ", MessageBoxButton.YesNo,
                               MessageBoxImage.Exclamation
                               )
        If mesres = MessageBoxResult.Yes Then
            nachricht(" hier wurde was verworfen: ")
            e.Cancel = False
        Else
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub btnSicherungsverzeichnis_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim fi As IO.FileInfo
        Try
            l(" MOD btnSicherungsverzeichnis_Click anfang")
            fi = New IO.FileInfo(FullnameCheckout)
            IO.Directory.CreateDirectory(fi.Directory.ToString)
            System.Diagnostics.Process.Start(fi.Directory.ToString)
            l(" MOD btnSicherungsverzeichnis_Click ende")
        Catch ex As Exception
            l("Fehler in btnSicherungsverzeichnis_Click: " & ex.ToString())
        End Try
    End Sub
End Class
