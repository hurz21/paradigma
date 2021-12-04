
'Public Class clsRichtextbox
'    Private steuer$, textkoerper$, ueberschrift$
'    Private flwMain As New FlowDocument
'    'Public Property originalfilename As String = ""
'    Private rtb1 As New RichTextBox
'    'Sub New(ByVal steuerparam$)
'    '    ' InitializeComponent()
'    '    steuer = steuerparam
'    'End Sub

'    'Private Property insArchiv As Boolean = True

'    'Private Property erfolg As Boolean

'    'Private Shared Sub hinweisAudDasNeueDokumentImArchiv(ByVal filenameImLokalenCache As String)
'    '    If Not filenameImLokalenCache.StartsWith("Fehler") Then
'    '        MsgBox("Die neue Datei wurde erzeugt und befindet sich nun im Archiv unter dem Reiter <Dokumente>.")
'    '    End If
'    'End Sub
'    'Public Sub init(ueberschrift As String)
'    '    Dim filenameImLokalenCache As String = ""
'    '    Dim a As New clsAktennotiz()
'    '    Dim oeffnen As Boolean = True
'    '    If steuer = "1" Then
'    '        textkoerper = "Thema: "
'    '        ueberschrift = "Hier Text einfügen"
'    '        a.generateHeaderUndBody(flwMain, textkoerper, ueberschrift)
'    '        'a.RTFdateispeichernImArchiv(rtb1)
'    '        rtb1.Document = flwMain
'    '        a.RTFdateispeichernImArchiv(rtb1)
'    '    End If
'    '    If steuer = "2" Then     'ereignis
'    '        Dim mesred As New MessageBoxResult
'    '        'mesred = MessageBox.Show("Das im Folgenden erzeugte Dokument dient der Druck-Ausgabe. " & vbCrLf &
'    '        '                "Die Datei wird ins Archiv übernommen und dem Ereignis zugeordnet.  " & vbCrLf & vbCrLf &
'    '        '                "Sie können jederzeit ein Word-Dokument neu erzeugen!", "Ausgabe als Worddokument (.rtf)",
'    '        '                MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
'    '        mesred = MessageBox.Show("Es wird nun ein Word-Dokument erzeugt. Dieses kann im Archiv abgepeichert werden.  " & vbCrLf & vbCrLf & vbCrLf &
'    '                     "Ja   - im Archiv abspeichern  " & vbCrLf & vbCrLf &
'    '                     "Nein - nur zum Ducken verwenden, dann löschen" & vbCrLf & vbCrLf,
'    '                     "Ausgabe als Worddokument (.rtf)",
'    '                     MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
'    '        insArchiv = If(mesred = MessageBoxResult.Yes, True, False)

'    '        ueberschrift = String.Format("{0}: {1}", myGlobalz.sitzung.aktEreignis.Art, myGlobalz.sitzung.aktEreignis.Beschreibung)
'    '        If myGlobalz.sitzung.aktEreignis.istRTF Then
'    '            Dim datei As String = EreignisRTFhelp.getFokumenttextPfadVonRtfTextfromEreignis()
'    '            'textkoerper = EreignisRTFhelp.NotizRtfInhaltLadenExtracted(datei)
'    '            'a.generateHeaderUndBodyEreignis(flwMain, ueberschrift, textkoerper)
'    '            'rtb1.Document = flwMain
'    '            a.RTFdateispeichernEreignisRTF(datei, filenameImLokalenCache)
'    '            If Not insArchiv Then
'    '                glob2.OpenDocument(filenameImLokalenCache)
'    '            Else
'    '                hinweisAudDasNeueDokumentImArchiv(filenameImLokalenCache)
'    '            End If
'    '            originalfilename = filenameImLokalenCache
'    '        Else
'    '            'Laden aus der Datei
'    '            textkoerper = myGlobalz.sitzung.aktEreignis.Notiz
'    '            a.generateHeaderUndBody(flwMain, ueberschrift, textkoerper)
'    '            rtb1.Document = flwMain
'    '            a.RTFdateispeichernEreignis(rtb1, filenameImLokalenCache)
'    '            If Not insArchiv Then
'    '                glob2.OpenDocument(filenameImLokalenCache)
'    '            Else
'    '                hinweisAudDasNeueDokumentImArchiv(filenameImLokalenCache)
'    '            End If
'    '            originalfilename = filenameImLokalenCache
'    '        End If
'    '        oeffnen = False
'    '    End If

'    '    If steuer = "3" Then     'ereignis
'    '        ueberschrift = String.Format("{0}: {1}", myGlobalz.sitzung.aktEreignis.Art, myGlobalz.sitzung.aktEreignis.Beschreibung)
'    '        textkoerper$ = myGlobalz.sitzung.aktEreignis.Notiz
'    '        a.generateHeaderUndBody(flwMain, ueberschrift, textkoerper)
'    '        rtb1.Document = flwMain
'    '        a.RTFdateierzeugenEreignis(rtb1, filenameImLokalenCache, ueberschrift)
'    '        originalfilename = filenameImLokalenCache
'    '        insArchiv = False
'    '    End If
'    '    If insArchiv And steuer <> "1" Then
'    '        Dim docFile As String = glob2.ConvertRtf2Doc(filenameImLokalenCache)
'    '        If Not docFile.IsNothingOrEmpty Then
'    '            IO.File.Delete(filenameImLokalenCache)
'    '            filenameImLokalenCache = docFile
'    '            insArchivUndOeffnen(filenameImLokalenCache, oeffnen)
'    '        End If
'    '    Else
'    '    End If
'    'End Sub


'    'Private Sub insArchivUndOeffnen(ByVal filenameImLokalenCache As String, oeffnen As Boolean)
'    '    Dim numdir As String
'    '    Dim archivfullname As String = ""
'    '    Dim fi As New IO.FileInfo(filenameImLokalenCache)
'    '    myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
'    '    myGlobalz.sitzung.aktDokument.DateinameMitExtension = fi.Name

'    '    myGlobalz.sitzung.aktDokument.Filedatum = Now
'    '    myGlobalz.sitzung.aktDokument.Checkindatum = Now
'    '    myGlobalz.sitzung.aktDokument.Beschreibung = myGlobalz.sitzung.aktEreignis.Beschreibung & ", Druckversion"
'    '    myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.DOC
'    '    myGlobalz.sitzung.aktDokument.Initiale = myGlobalz.sitzung.aktBearbeiter.Initiale
'    '    myGlobalz.sitzung.aktDokument.istVeraltet = False

'    '    myGlobalz.sitzung.aktDokument.revisionssicher = False

'    '    numdir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID))
'    '    erfolg = myGlobalz.Arc.checkINDoku(filenameImLokalenCache,
'    '                                       myGlobalz.sitzung.aktDokument.Beschreibung,
'    '                                       myGlobalz.sitzung.aktEreignis.ID,
'    '                                       False,
'    '                                       archivfullname,
'    '                                       numdir,
'    '                                       myGlobalz.sitzung.aktVorgangsID,
'    '                                       False,
'    '                                       Now,
'    '                                       myGlobalz.sitzung.aktDokument.DocID,
'    '                                       myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
'    '    If erfolg Then
'    '        myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
'    '        If oeffnen Then
'    '            If DokArc.machCheckout("zeige") Then
'    '                DokArc.Archiv_aktiviere_dokument(myGlobalz.sitzung.aktDokument)
'    '                DokArc.zulisteAddieren()
'    '            End If
'    '        End If
'    '    End If
'    'End Sub
'End Class
