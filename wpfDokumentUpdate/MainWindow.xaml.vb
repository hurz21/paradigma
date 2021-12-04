Imports System.ComponentModel
Imports System.IO
Imports wpfDokumentUpdate
Imports System.Data.SqlClient

Class MainWindow
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\232475" tempEditDatei="~$07-9609-12_2017-09-28_074503.docx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\232475\3307-9609-12_2017-09-28_074503.docx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\9609\232475" revisionssicher="False" dokid="232475" vid="9609"
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074" tempEditDatei="~$Massstab.xlsx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074\Massstab.xlsx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\9609\233074" revisionssicher="False" dokid="0" vid="9609"
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074" tempEditDatei="~$Massstab.xlsx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\9609\233074\Massstab.xlsx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\9609\233074" revisionssicher="False" dokid="0" vid="9609"
    'watchPfad="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\26352\235003" tempEditDatei="~$STAENDIGKEITEN_FUER_HAUSNUMMERN.docx" FullnameCheckout="C:\Users\feinen_j\Desktop\Paradigma\Archiv_Checkout\26352\235003\ZUSTAENDIGKEITEN_FUER_HAUSNUMMERN.docx" FullnameImArchiv="\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv\1\0\2012\26352\235003" revisionssicher="False" dokid="235003" vid="26352"
    Public word As New Microsoft.Office.Interop.Word.Application
    Public doc As New Microsoft.Office.Interop.Word.Document 'habe hier new ergänzt ????
    Public obj As Object
    Public mgisVersion As String = ""
    Public Property VorgangREC() As IDB_grundfunktionen
    Public Shared vorgang_MYDB As New clsDatenbankZugriff
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
    Public nurlesen As Boolean = False
    'Public sw As IO.StreamWriter
    Property qfile As String
    Public Property sleeptime As Integer = 1 '8000
    Public Property kompressed As Boolean = False

    Sub New()
        InitializeComponent()
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        IO.Directory.SetCurrentDirectory("C:\kreisoffenbach\pumuckel")
        mgisVersion = My.Resources.BuildDate.Trim.Replace(vbCrLf, "")
        With My.Log.DefaultFileLogWriter
            .BaseFileName = "c:\kreisoffenbach\pumuckel\pumuckel_" & gettimestamp()
            .AutoFlush = False
            .Append = False
        End With
        qfile = My.Log.DefaultFileLogWriter.FullLogFileName
        Dim arguments As String()
        Try

            l("MainWindow_Loaded---------------------- anfang")
            arguments = Environment.GetCommandLineArgs()
            l("arguments.Count: " & arguments.Count)
            'Threading.Thread.Sleep(sleeptime)

            mapAllArguments(arguments)
            Title = FullnameCheckout
            Dim fi As New IO.FileInfo(FullnameCheckout)
            If fi.Exists Then
                tbvid.Text = vid.ToString
                tbfilename.Text = fi.Name
                Title = "Pumuckel: " & vid.ToString & ", " & fi.Name & " (Vers: " & mgisVersion & ")"
                fi = Nothing
                If revisionssicher Then nurlesen = True
                Threading.Thread.Sleep(2000) 'paradigma legt ja grad ne kopie an. könnte sich überschneiden
                worddateiOeffnen(FullnameCheckout, nurlesen)
            End If
            l("MainWindow_Loaded---------------------- ende")
        Catch ex As Exception
            If istOfficeProblem(ex) Then
                MessageBox.Show("Vermutlich ist Ihre Officeinstallation fehlerhaft und muss Office neu installiert werden. " & Environment.NewLine &
                                 "Falls das Problem anhält bitte beim Admin oder ersatzweise bei der IT-Hotline melden und eine Neuinstallation von Office anfordern!")
            End If
            l("vehler in MainWindow_Loaded: " & nurlesen & "," & Environment.UserName & ex.ToString())
        End Try
        e.Handled = True
    End Sub

    Private Function istOfficeProblem(fehler As Exception) As Boolean
        Dim summe As String = ""
        Try
            l(" MOD istOfficeProblem anfang")
            summe = fehler.ToString
            If summe.Contains("Das COM-Objekt des Typs") And
                    summe.Contains("Interop.Word.DocumentClass") And
                    summe.Contains("kann nicht in den Schnittstellentyp") And
                    summe.Contains("Interop.Word._Document") And
                    summe.Contains("QueryInterface-Aufruf") Then
                Return True
            End If
            l(" MOD istOfficeProblem ende")
            Return False
        Catch ex As Exception
            l("Fehler in istOfficeProblem: " & ex.ToString())
            Return False
        End Try
    End Function

    Sub worddateiOeffnen(vorlageFullname As String, nurlesen As Boolean)
        Dim fcode As Integer = 0
        Try
            obj = vorlageFullname
            'doc = word.Documents.Open2000(obj)
            If nurlesen Then
                fcode = 11
                doc = word.Documents.Open(obj,, True)
            Else
                fcode = 12
                doc = word.Documents.Open(obj,, False)
            End If
            fcode = 2
            word.Windows.Application.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMaximize
            fcode = 3
            word.WindowState = CType(WindowState.Maximized, Microsoft.Office.Interop.Word.WdWindowState)
            fcode = 4
            word.Visible = (True) 'Word sehen zum Test?
            fcode = 5
            doc.Activate()
            fcode = 6
            doc.ActiveWindow.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMaximize
            fcode = 7
        Catch ex As Exception
            nachricht("vehler bei starten der word datei: Datei nicht vorhanden. fehlercode:" & fcode & Environment.UserName & Environment.NewLine &
                      ex.ToString)
            doc.Close()
            doc = Nothing
            word.Application.Quit()
            word = Nothing
        Finally
            'doc.Close()
            'doc = Nothing
            'word.Application.Quit()
            'word = Nothing
            'ReleaseComObj(word)
            'ReleaseComObj(doc)
            '' Die Speichert freigeben
            'GC.Collect()
            'GC.WaitForPendingFinalizers()
            'GC.Collect()
            'GC.WaitForPendingFinalizers()
        End Try
    End Sub
    'Public Sub checkopen(doc As Microsoft.Office.Interop.Word.Document, ByRef cancle As Boolean)
    '    cancle = True
    'End Sub
    Private Sub ReleaseComObj(o As Object)
        Try
            Dim i As Integer
            Do
                i = System.Runtime.InteropServices.Marshal.ReleaseComObject(o)
            Loop While i > 0
        Catch
        Finally
            o = Nothing
        End Try
    End Sub
    Private Sub altedateimethode()
        Dim fi As New IO.FileInfo(FullnameCheckout)
        Dim ftemp As New IO.FileInfo(watchpfad & "\" & tempEditDatei)
        'If ftemp.Exists Then
        If fi.Exists Then
            dateinamemitextension = fi.Name
            geloeschteDatei = watchpfad & "\" & tempEditDatei
            fi = Nothing
            watch(dateinamemitextension)
            Title = "Pumuckel is watching: " & FullnameCheckout & ", " & sleeptime
        Else
        End If
    End Sub

    Private Function gettimestamp() As String
        Return Now.ToString("yyyyMMddhhmmss_ffff")
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
                kandidat.DocID = dokid
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
        Dim neuerFullName As String = ""
        Dim fi As IO.FileInfo
        Dim fcode As Integer = 0
        Try
            fi = New IO.FileInfo(dok.FullnameImArchiv)
            fcode = 1
            neuerFullName = getVersionsBackupName(dok.FullnameCheckout, fi.Name, fi.Extension, fi.DirectoryName)
            fcode = 2
            '  neuerFullName = fi.DirectoryName & "\" & neuerFullName
            fi = Nothing
            nachricht("ArchivDateiNachVersionBackupUmbenennen altername: " & dok.FullnameImArchiv & neuerFullName & "neuerFullName")
            fcode = 3
            My.Computer.FileSystem.RenameFile(dok.FullnameImArchiv, neuerFullName)
            fcode = 4
            Return True
        Catch ex As Exception
            MessageBox.Show("fehler in ArchivDateiNachVersionBackupUmbenennen: " & fcode & Environment.NewLine &
                            dok.FullnameImArchiv & Environment.NewLine &
                            neuerFullName & Environment.NewLine &
                            ex.ToString)
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
                    MsgBox("geänderte Datei wurde übernommen")

                Else
                    'Übernahme ins archiv gescheitert 
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
        Dim fcode As Integer = 0
        Try
            l("geaenderteDateiInsArchivUebernehmen ---------------------- anfang")
            nachricht("geaenderteDateiInsArchivUebernehmen: " & dok.FullnameImArchiv)
            If CHeckOut_test.Exists Then
                fcode = 1
                Dim fullname As String = dok.FullnameImArchiv
                inputFileReadonlyEntfernen(dok.FullnameImArchiv)
                'If ArchivDateiNachVersionBackupUmbenennen(dok) Then
                '    fcode = 21
                '    nachricht("archivdatei umbenannt")
                'Else
                '    fcode = 22
                '    nachricht("fehler archivdatei konnte nicht umbenannt werden (unwichtig)" & dok.FullnameImArchiv)
                'End If
                fcode = 3
                If imarchivSichernErfolgreich(CHeckOut_test, fullname) Then
                    fcode = 4
                    If updateZeitstempel() Then
                        fcode = 5
                        Return True  'weiter zur DB
                    Else
                        fcode = 6
                        Return False  'weiter zur DB
                    End If
                Else
                    fcode = 7
                    Return False
                End If
            End If
            fcode = 8
            Return True
            l("geaenderteDateiInsArchivUebernehmen---------------------- ende")
        Catch ex As Exception
            MessageBox.Show("fehler in geaenderteDateiInsArchivUebernehmen: Code: ")
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

        'wordDocWatcher.EnableRaisingEvents = False
        'wordDocWatcher.Dispose()
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
            ' MsgBox(ianz & " updateZeitstempel ")
            If ianz > 0 Then

                Return True
            Else
                Return False
            End If

            Return True
            nachricht("---------------------- ende")
        Catch ex As Exception
            MessageBox.Show("Fehler beim aktualisieren des Zeitstempels")
            nachricht("Fehler in updateZeitstempel: " & ex.ToString())

            Return False
        End Try

    End Function

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        'Dim mesres As New MessageBoxResult
        'mesres = MessageBox.Show("Wenn Sie dieses Fenster schließen,  " & Environment.NewLine &
        '        "verlieren Sie die Änderungen im Worddokument" & Environment.NewLine &
        '        " (das Fenster schließt sich automatisch sobald Sie das Dokument schließen" & Environment.NewLine & Environment.NewLine &
        '        "      trotzdem schliessen = JA " & Environment.NewLine &
        '        "      abbrechen          = NEIN " & Environment.NewLine & Environment.NewLine &
        '        " " & Environment.NewLine,
        '                       "Vorsicht ", MessageBoxButton.YesNo,
        '                       MessageBoxImage.Exclamation
        '                       )
        'If mesres = MessageBoxResult.Yes Then
        '    nachricht(" hier wurde was verworfen: ")
        '    e.Cancel = False
        'Else
        '    e.Cancel = True
        '    Exit Sub
        'End If
    End Sub

    Private Sub btnWorddateischliessen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        l(" btnWorddateischliessen_Click ---------------------- anfang")
        Try
            l(" MOD ---------------------- anfang")
            If Not IsNothing(doc) Then
                doc.Save()
                doc.Close()
                doc = Nothing
                ReleaseComObj(doc)
            End If
            If Not IsNothing(word) Then
                Try
                    word.Quit()
                Catch ex As Exception
                    'ignore
                End Try
                ReleaseComObj(word)
                word = Nothing
            End If
            GC.Collect()
            GC.WaitForPendingFinalizers()
            l(" MOD ---------------------- ende")
        Catch ex As Exception
            '   MessageBox.Show("Die Datei ist nicht mehr im Zugriff und muss daher nicht geschlossen werden!")
            l("vehler ist nicht mehr im zugriff in MOD: " & ex.ToString())
        End Try
        worddateisichern()
    End Sub

    Private Function worddateisichern() As Boolean
        Dim dateiIstNeuerAlsImArchiv As Boolean
        Dim fcode As Integer = 0
        Try
            kandidat = New Dokument
            kandidat.FullnameCheckout = FullnameCheckout
            kandidat.FullnameImArchiv = FullnameImArchiv
            kandidat.tempEditDatei = tempEditDatei
            kandidat.DateinameMitExtension = dateinamemitextension
            kandidat.revisionssicher = revisionssicher
            kandidat.kompressed = kompressed
            kandidat.DocID = dokid
            fcode = 1
            ' hatKandidat = True
            dateiIstNeuerAlsImArchiv = getDateiistneuerAlsImArchiv(kandidat.FullnameCheckout, kandidat.FullnameImArchiv)
            fcode = 2
            nachricht("dateiIstNeuerAlsImArchiv: " & dateiIstNeuerAlsImArchiv)
            If kandidat.revisionssicher Then
                fcode = 21
                nachricht("Dokument ist revisionssicher. Sie können die Änderungen also nicht direkt ins Archiv übernehmen.")
                MessageBox.Show("Dokument ist revisionssicher. Sie können die Änderungen also nicht direkt ins Archiv übernehmen.",
                                        kandidat.DateinameMitExtension, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
                beenden()
            End If
            fcode = 3
            If dateiIstNeuerAlsImArchiv Then
                fcode = 31
                Dim CO_test As IO.FileInfo = New IO.FileInfo(kandidat.FullnameCheckout)
                If geaenderteDateiInsArchivUebernehmen(CO_test, kandidat) Then
                    nachricht("geänderte Datei wurde übernommen")
                    fcode = 311
                    MessageBox.Show("Geänderte Datei wurde erfolgreich ins Archiv übernommen")
                Else
                    'Übernahme ins archiv gescheitert 
                    MessageBox.Show("Die Übernahme ins Archiv ist gescheitert. Bitte sichern Sie die folgende Datei:" & Environment.NewLine &
                                    CO_test.FullName & Environment.NewLine &
                                    "Das Verzeichnis wird nun geöffnet um Ihnen die Sicherung zu erleichtern!",
                                    "Sicherung im Archiv gescheitert",
                                    MessageBoxButton.OK, MessageBoxImage.Error)
                    ')
                    fcode = 312
                    Process.Start(CO_test.DirectoryName)
                End If
                CO_test = Nothing
                fcode = 313
            Else
                fcode = 32
                l("Die Datei ist nicht neuer als die im Archiv - keine Aktion. ")
                'MessageBox.Show("Die Datei ist nicht neuer als die im Archiv - keine Aktion. ",
                '               "Übernahme der geänderten Datei ins Archiv", MessageBoxButton.OK, MessageBoxImage.Error)
            End If
            fcode = 34
            beenden()
            l(" btnWorddateischliessen_Click ---------------------- ende")
        Catch ex As Exception
            l("Fehler in btnWorddateischliessen_Click: " & ex.ToString())
            MessageBox.Show("Fehler bei der Übernahme der Datei ins Archiv " & fcode)
        End Try

        Return dateiIstNeuerAlsImArchiv
    End Function

    Private Sub btnWorddateinichtsichern_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
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
