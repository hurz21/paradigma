
Public Class clsRichtextbox
    Private steuer As String, textkoerper As String, ueberschrift As String
    Private flwMain As New FlowDocument
    Public Property originalfilename As String = ""
    Private rtb1 As New RichTextBox
    Sub New(ByVal steuerparam As String)
        ' InitializeComponent()
        steuer = steuerparam
    End Sub

    Private Property insArchiv As Boolean = True

    Private Property erfolg As Boolean

    Private Shared Sub hinweisAudDasNeueDokumentImArchiv(ByVal filenameImLokalenCache As String)
        If Not filenameImLokalenCache.StartsWith("Fehler") Then
            MsgBox("Die neue Datei wurde erzeugt und befindet sich nun im Archiv unter dem Reiter <Dokumente>.")
        End If
    End Sub
    <Obsolete>
    Private Function Ereignis_2(ByRef filenameImLokalenCache As String, ByVal a As clsAktennotiz, ByRef oeffnen As Boolean) As String
        Dim ueberschrift As String
        Dim mesred As New MessageBoxResult

        mesred = MessageBox.Show("Es wird nun ein Word-Dokument erzeugt. Dieses wird im Archiv abgepeichert.  " & vbCrLf & vbCrLf & vbCrLf,
                                 "Word Dokument erzeugen",
                     MessageBoxButton.OK, MessageBoxImage.Exclamation)
        insArchiv = True 'If(mesred = MessageBoxResult.Yes, True, False)

        ueberschrift = String.Format("{0}: {1}", myglobalz.sitzung.aktEreignis.Art, myglobalz.sitzung.aktEreignis.Beschreibung)
        If myglobalz.sitzung.aktEreignis.istRTF Then
            Dim datei As String = EreignisRTFhelp.getFokumenttextPfadVonRtfTextfromEreignis()
            'textkoerper = EreignisRTFhelp.NotizRtfInhaltLadenExtracted(datei)
            'a.generateHeaderUndBodyEreignis(flwMain, ueberschrift, textkoerper)
            'rtb1.Document = flwMain
            a.RTFdateispeichernEreignisRTF(datei, filenameImLokalenCache)
            If Not insArchiv Then
                glob2.OpenDocument(filenameImLokalenCache)
            Else
                hinweisAudDasNeueDokumentImArchiv(filenameImLokalenCache)
            End If
            originalfilename = filenameImLokalenCache
        Else
            'Laden aus der Datei
            textkoerper = myglobalz.sitzung.aktEreignis.Notiz
            '   a.generateHeaderUndBody(flwMain, ueberschrift, textkoerper)
            rtb1.Document = flwMain
            a.RTFdateispeichernEreignis(rtb1, filenameImLokalenCache)
            If Not insArchiv Then
                glob2.OpenDocument(filenameImLokalenCache)
            Else
                hinweisAudDasNeueDokumentImArchiv(filenameImLokalenCache)
            End If
            originalfilename = filenameImLokalenCache
        End If

        Return ueberschrift
    End Function

    Public Sub init(ueberschrift As String)
        Dim filenameImLokalenCache As String = ""
        Dim a As New clsAktennotiz(ueberschrift)
        Dim oeffnen As Boolean = True
        If steuer = "1" Then
            'textkoerper = "Thema: "
            'ueberschrift = "Hier Text einfügen"
            'a.generateHeaderUndBody(flwMain, textkoerper, ueberschrift)
            'a.RTFdateispeichernImArchiv(rtb1)
            'rtb1.Document = flwMain
            'a.RTFdateispeichernImArchiv(rtb1)
            'AktenotizMitEreignisErzeugen(ueberschrift)
        End If
        If steuer = "2" Then     'ereignis
            'ist obsolet
#Disable Warning BC40008 ' 'Private Function Ereignis_2(ByRef filenameImLokalenCache As String, a As clsAktennotiz, ByRef oeffnen As Boolean) As String' is obsolete.
            ueberschrift = Ereignis_2(filenameImLokalenCache, a, oeffnen)
#Enable Warning BC40008 ' 'Private Function Ereignis_2(ByRef filenameImLokalenCache As String, a As clsAktennotiz, ByRef oeffnen As Boolean) As String' is obsolete.
            oeffnen = False
        End If
        If steuer = "22" Then     'ereignis
            ueberschrift = Ereignis_22_nurDrucken_ohneArchiv(filenameImLokalenCache, a, oeffnen)
        End If

        If steuer = "3" Then     'ereignis
            ueberschrift = String.Format("{0}: {1}", myglobalz.sitzung.aktEreignis.Art, myglobalz.sitzung.aktEreignis.Beschreibung)
            textkoerper$ = myglobalz.sitzung.aktEreignis.Notiz
            'ihah      a.generateHeaderUndBody(flwMain, ueberschrift, textkoerper)
            rtb1.Document = flwMain
            a.RTFdateierzeugenEreignis(rtb1, filenameImLokalenCache, ueberschrift)
            originalfilename = filenameImLokalenCache
            insArchiv = False
        End If
        If insArchiv And steuer <> "1" Then
            Dim docFile As String = wordInterop.rtf2doc(filenameImLokalenCache) 'glob2.ConvertRtf2Doc(filenameImLokalenCache)
            If Not docFile.IsNothingOrEmpty Then
                IO.File.Delete(filenameImLokalenCache)
                filenameImLokalenCache = docFile
                insArchivUndOeffnen(filenameImLokalenCache, oeffnen)
            End If
        Else
        End If
    End Sub


    Private Sub insArchivUndOeffnen(ByVal filenameImLokalenCache As String, oeffnen As Boolean)
        Dim numdir As String
        Dim archivfullname As String = ""
        Dim fi As New IO.FileInfo(filenameImLokalenCache)
        myglobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
        myglobalz.sitzung.aktDokument.DateinameMitExtension = fi.Name
        fi = Nothing
        myglobalz.sitzung.aktDokument.Filedatum = Now
        myglobalz.sitzung.aktDokument.Checkindatum = Now
        myglobalz.sitzung.aktDokument.newSaveMode = True
        myglobalz.sitzung.aktDokument.Beschreibung = myglobalz.sitzung.aktEreignis.Beschreibung & ", Druckversion"
        myglobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.DOC
        myglobalz.sitzung.aktDokument.Initiale = myglobalz.sitzung.aktBearbeiter.Initiale
        myglobalz.sitzung.aktDokument.istVeraltet = False

        myGlobalz.sitzung.aktDokument.revisionssicher = False
        myGlobalz.sitzung.aktDokument.kompressed = False

        numdir = myglobalz.Arc.getFreshNumDir(myglobalz.sitzung.defineArchivVorgangsDir(myglobalz.sitzung.aktVorgangsID))
        erfolg = myGlobalz.Arc.checkINDoku(filenameImLokalenCache,
                                           myGlobalz.sitzung.aktDokument.Beschreibung,
                                           myGlobalz.sitzung.aktEreignis.ID,
                                           False,
                                           archivfullname,
                                           numdir,
                                           myGlobalz.sitzung.aktVorgangsID,
                                           False,
                                           Now,
                                           myGlobalz.sitzung.aktDokument.DocID,
                                           myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                           myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID)
        If erfolg Then
            myglobalz.sitzung.aktDokument.makeFullname_ImArchiv(myglobalz.Arc.rootDir)
            'If oeffnen Then
            '    If DokArc.machCheckout("zeige") Then
            '        If DokArc.zulisteAddieren() Then
            '            myglobalz.sitzung.aktDokument.nurzumlesen = False
            '            DokArc.Archiv_aktiviere_dokument(myglobalz.sitzung.aktDokument)
            '        End If
            '    End If
            'End If
        End If
    End Sub
    <Obsolete>
    Private Function Ereignis_22_nurDrucken_ohneArchiv(ByRef filenameImLokalenCache As String, ByVal a As clsAktennotiz, ByRef oeffnen As Boolean) As String
        MsgBox("Diese Datei wird nicht im Archiv gespeichert !!!!!!", , "Diese Datei wird nicht im Archiv gespeichert !!!!!!")
        insArchiv = False 'If(mesred = MessageBoxResult.Yes, True, False)
        ueberschrift = String.Format("{0}: {1}", myGlobalz.sitzung.aktEreignis.Art, myGlobalz.sitzung.aktEreignis.Beschreibung)
        If myGlobalz.sitzung.aktEreignis.istRTF Then
            Dim datei As String = EreignisRTFhelp.getFokumenttextPfadVonRtfTextfromEreignis()
            a.RTFdateispeichernEreignisRTF(datei, filenameImLokalenCache)
            glob2.OpenDocument(filenameImLokalenCache)
            originalfilename = filenameImLokalenCache
        Else
            'Laden aus der Datei
            textkoerper = myGlobalz.sitzung.aktEreignis.Notiz
            'ihah         a.generateHeaderUndBody(flwMain, ueberschrift, textkoerper)
            rtb1.Document = flwMain
            a.RTFdateispeichernEreignis(rtb1, filenameImLokalenCache)
            glob2.OpenDocument(filenameImLokalenCache)
            originalfilename = filenameImLokalenCache
        End If
        oeffnen = False
        Return ueberschrift
    End Function

End Class
