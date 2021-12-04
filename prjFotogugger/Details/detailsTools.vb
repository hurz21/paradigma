Imports System.Data
Imports System.IO
Imports paradigmaDetail


Public Class detailsTools
    Public Shared Function mitMiniMapDarstellen() As Boolean
        Try
            Dim wert As String
            If initP.getValue("Haupt.MINIMAP") = "0" Then
                Return False
            End If
            wert = CLstart.myc.userIniProfile.WertLesen("NOMAP", "VID")
            If Not String.IsNullOrEmpty(wert) Then
                Dim b As String
                b = CLstart.myc.userIniProfile.WertLesen("NOMAP", "VID")
                If LIBgemeinsames.clsString.isinarray(b, myGlobalz.sitzung.aktVorgangsID.ToString, ",") Then
                    Return False
                End If
            End If
            wert = CLstart.myc.userIniProfile.WertLesen("NOMAP", "neverShowMinimap")
            If Not String.IsNullOrEmpty(wert) Then
                If wert.Trim = "1" Then Return False
            End If
            Return True
        Catch ex As Exception
            nachricht("fehler in presentMapExtracted: ", ex)
            Return True ' !sic!
        End Try
    End Function
    Shared Sub VorgangLocking(ByVal modus As String)
        Dim lk As clsVorgangLocking
        Dim lockuser As String
        Try
            l("VorgangLocking---------------------- anfang")
            l("VorgangLocking---------------------- modus: " & modus)
            'myGlobalz.paradigmaDateiServerRoot & "\div\config\locks\",        
            lk = New clsVorgangLocking(initP.getValue("Haupt.LOCKINGFile"),
                                        myGlobalz.sitzung.aktVorgangsID,
                                        myGlobalz.sitzung.aktBearbeiter.Initiale)
            If modus = "ein" Then
                lockuser = lk.lockingPruefen()
                If String.IsNullOrEmpty(lockuser) Then
                    nachricht("Nicht gelockt")
                    'locken
                    If lk.lockingSetzen() Then
                        nachricht("Vorgang wurde erfolgreich gelockt")
                    Else
                        MsgBox(String.Format("! Der Vorgang konnte für Anwender: {0}' nicht exklusiv geöffnet werden.", myGlobalz.sitzung.aktBearbeiter.Initiale))
                    End If
                Else
                    Dim text = (String.Format("Kollision: Der Vorgang ist evtl. bereits von Anwender {0} geöffnet. " &
                                               "Dies kann beim Ändern von Dokumenten problematisch sein." &
                                               "Bitte warten Sie mit Änderungen bis der Anwender den Fall verläßt.",
                                               lockuser))
                    nachricht(text)
                    HinweisFenster_Vorgang_im_Zugriff(lockuser)
                    If lk.lockingSetzen() Then
                        nachricht("Vorgang wurde erfolgreich gelockt")
                    Else
                        '  MsgBox(String.Format("- Der Vorgang konnte für Anwender: {0}' nicht exklusiv geöffnet werden.", myGlobalz.sitzung.Bearbeiter.Initiale))
                    End If
                End If
            End If
            If modus = "aus" Then
                lk.LockingLoesen()
            End If
            l("VorgangLocking---------------------- ende")
        Catch ex As Exception
            l("Fehler in VorgangLocking: ", ex)
        End Try
    End Sub

    Friend Shared Function schreibeTextDateiInsArchiv(neuerText As String, _aktdoku As Dokument) As Boolean
        'text = detailsTools.getTextINhalt(aktdoku.makeFullname_ImArchiv(myglobalz.Arc.rootDir))
        Dim datei As String
        datei = _aktdoku.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        'My.Computer.FileSystem.WriteAllText(datei, neuerText, False, CLstart.mycSimple.enc)
        My.Computer.FileSystem.WriteAllText(datei, neuerText, False, System.Text.Encoding.UTF8)
        'System.Text.Encoding.GetEncoding()
        Return True
    End Function

    Friend Shared Function getTextINhalt(fullnameCheckout As String) As String
        Dim lestext As String
        Dim fi As IO.StreamReader
        Try
            fi = New IO.StreamReader(fullnameCheckout)
            lestext = fi.ReadToEnd
            fi.Close()
            fi.Dispose()
            fi = Nothing
            Return lestext
        Catch ex As Exception
            nachricht("Fehler in getTextINhalt: ", ex)
            Return "kein text"
        End Try
    End Function



    Public Shared Sub EreignisExcel_ClickExtracted()
        Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Ereignisse", myGlobalz.sitzung.EreignisseRec.dt, myGlobalz.sitzung.aktVorgangsID, "",
                                          CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub

    Public Shared Sub BeteiligteExcel_ClickExtracted()
        Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Beteiligte", myGlobalz.sitzung.beteiligteREC.dt, myGlobalz.sitzung.aktVorgangsID, "", CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub


    Public Shared Sub DokumenteExcel_clickExtracted()
        Debug.Print(Psession.presDokus.Count.ToString)
        Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Dokumente", Psession.presDokus, myGlobalz.sitzung.aktVorgangsID, "", CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub
    Public Shared Function fuelleVerwandteDT(ByVal sql As String) As Boolean
        VerwandteTools.erzeugeVerwandtenlistezuVorgang.exe(sql, myGlobalz.sitzung.tempREC)
        myGlobalz.sitzung.VerwandteDTServer.Clear()
        myGlobalz.sitzung.VerwandteDTServer = myGlobalz.sitzung.tempREC.dt.Copy
        If Not myGlobalz.sitzung.VerwandteDTServer.IsNothingOrEmpty Then
            'ist verlinkt!!
            Return True
        Else
            'ist nicht verlinkt
            Return False
        End If
    End Function

    Friend Shared Sub EreignisseOhnedokusAuchKopieren(quellVorgangsid As Integer, aktVorgangsID As Integer, doppelteDokusAuchKopieren As Boolean)
        Dim hinweis As String = ""
        Dim lokereignisDokListe As New List(Of clsEreignisDok)
        Dim erfolg As Boolean
        Try
            myGlobalz.sitzung.EreignisseRec.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabEreignis & "  e where VorgangsID=" & aktVorgangsID & " order by datum desc"
            hinweis = myGlobalz.sitzung.EreignisseRec.getDataDT()

            If myGlobalz.sitzung.EreignisseRec.mycount < 1 Then
                nachricht("Keine Ereignisse gespeichert c!")
                Exit Sub
            End If
            Dim anzahlDoksDT As New DataTable

            anzahlDoksDT = detailsTools.getAnzahlDoksproEreignis(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.VorgangREC)
            If anzahlDoksDT Is Nothing OrElse anzahlDoksDT.IsNothingOrEmpty Then
                Debug.Print("")
            Else
                RBtoolsns.statusSpalteErgaenzenUndFuellen.execute(myGlobalz.sitzung.EreignisseRec.dt, anzahlDoksDT,
                                                                  "ANZAHL",
                                                                  "ID")
            End If
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))

            'If ereignisdocexpand Then
            '    myGlobalz.sitzung.tempREC2.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.view_VORG2DOKEREIGNIS2 & " as viii where VorgangsID=" & vid &
            '                                          " and dateinameext is not null"
            '    myGlobalz.sitzung.tempREC2.dt = getDT4Query(myGlobalz.sitzung.tempREC2.mydb.SQL, myGlobalz.sitzung.tempREC2, hinweis)
            '    'hinweis = myglobalz.sitzung.tempREC2.getDataDT()
            'End If
            'erfolg = AlleEreignisseAufListeSetzen(myGlobalz.sitzung.EreignisseRec.dt,
            '                                    ereignisDokListe,
            '                                    myGlobalz.sitzung.tempREC2.dt,
            '                                    ereignisdocexpand)
            'Return CInt(myGlobalz.sitzung.EreignisseRec.mycount)
        Catch ex As Exception
            nachricht("fehler in refreshEreignisseListe: ", ex)
        End Try
    End Sub

    'Public Shared Sub clearCheckoutDokulist()
    '    myglobalz.sitzung.checkoutDokuList.Clear()
    'End Sub
    'Shared Function sindDokumenteImZugriff(ByVal checkoutDokuList As List(Of Dokument), ByRef idokInArbeit As String) As Boolean
    '    nachricht("sindDokumenteImZugriff: --------------------------------------")
    '    Dim CO_test As IO.FileInfo
    '    Try
    '        If checkoutDokuList Is Nothing Then
    '            nachricht("checkoutDokuList ist leer")
    '            Return False
    '        End If
    '        For Each dok As Dokument In checkoutDokuList
    '            CO_test = New IO.FileInfo(dok.FullnameCheckout)
    '            If Not IsFileWritable(CO_test.FullName) Then
    '                idokInArbeit = CO_test.Name
    '                'MsgBox("Eine Datei ist noch geöffnet. Bitte schließen: " & CO_test.FullName & Environment.NewLine &
    '                '       "Falls Paradigma über das Schließen der Datei nicht korrekt unterrichtet wurde: " & Environment.NewLine &
    '                '       "      >>> Schließen und Öffnen Sie den Vorgang. ")
    '                ' l("fehler lokal geänderte datei kann nichtchrieben werden. IsFileWritable: " & CO_test.FullName)
    '                Return True
    '            End If
    '        Next
    '        Return False
    '    Catch ex As Exception
    '        nachricht("fehler in sindDokumenteImZugriff: ", ex)
    '        Return True
    '    End Try

    'End Function
    'Shared Function wurdenDokumenteGeaendert(geloeschteDatei As String) As Boolean
    '    Dim meincount As Integer = 0
    '    Dim errorout As String = "errorout>"
    '    Dim kandidat As New Dokument
    '    Try
    '        nachricht("wurdenDokumenteGeaendert: --------------------------------------")
    '        If geloeschteDatei.IsNothingOrEmpty Then
    '            Return False
    '        Else
    '            MessageBox.Show("Eine Datei wurde geändert und soll ins Archiv eingecheckt werden." & Environment.NewLine &
    '                    geloeschteDatei & " <<" & Environment.NewLine &
    '                    " Geöffnet ist: " & myglobalz.sitzung.wordDateiImEditModus.DateinameMitExtension & Environment.NewLine &
    '                    "", "Paradigma")
    '        End If

    '        'todo logging aller dokuarbeiten aufnehmen
    '        meincount = 1
    '        ' If myglobalz.sitzung.checkoutDokuList.Count < 1 Then Return False

    '        Dim hatKandidat As Boolean = False
    '        Dim dateiIstNeuerAlsImArchiv As Boolean
    '        If istIrgendeinDokumentGeoeffnet(DokumentenTyp.XLS) Then bildeKandidatExcel(geloeschteDatei, kandidat, hatKandidat)
    '        If istIrgendeinDokumentGeoeffnet(DokumentenTyp.DOC) Then bildeKandidatWord(geloeschteDatei, kandidat, hatKandidat)
    '        If Not hatKandidat Then
    '            l("wurdenDokumenteGeaendert hatKandidat ist false")
    '            Return False
    '        End If

    '        dateiIstNeuerAlsImArchiv = getDateiistneuerAlsImArchiv(kandidat.FullnameCheckout, kandidat.FullnameImArchiv)
    '        If kandidat.revisionssicher Then
    '            meincount = 8 : nachricht("Dokument ist revisionssicher. Sie können die Änderungen also nicht direkt ins Archiv übernehmen.")
    '            MessageBox.Show(glob2.getMsgboxText("infotextRevisionssicherheit", New List(Of String)(New String() {})),
    '                                kandidat.DateinameMitExtension, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
    '            Return False
    '        End If

    '        If dateiIstNeuerAlsImArchiv Then
    '            meincount = 11
    '            Threading.Thread.Sleep(1000) 'um die datei loszulassen
    '            Dim CO_test As IO.FileInfo = New IO.FileInfo(kandidat.FullnameCheckout)
    '            If geaenderteDateiInsArchivUebernehmen(CO_test, kandidat) Then
    '                l("geänderte Datei wurde übernommen")
    '                'MessageBox.Show("Die geänderte Datei: " & Environment.NewLine & Environment.NewLine &
    '                '                     CO_test.Name & Environment.NewLine & Environment.NewLine &
    '                '                    "wurde erfolgreich ins Archiv übernommen. " & Environment.NewLine & Environment.NewLine,
    '                '                    "Übernahme der geänderten Datei ins Archiv",
    '                '                    MessageBoxButton.OK, MessageBoxImage.Information)
    '                Dim backupdatei As String = BackupAnlegen(CO_test, kandidat.DocID)
    '                OfficemerkerLoeschen(kandidat)
    '                Return True
    '            Else
    '                'Übernahme ins archiv gescheitert
    '                'datei sichern
    '                Dim backupdatei As String = BackupAnlegen(CO_test, kandidat.DocID)
    '                MessageBox.Show("Die Übernahme ins Archiv ist gescheitert. Es wurde eine Sicherungskopie angelegt! " & Environment.NewLine & Environment.NewLine &
    '                                "Sie können die Kopie finden unter Desktop/Paradigma/archivcheckouts/backup" & Environment.NewLine &
    '                                "Der Name der Datei lautet: " & Environment.NewLine & Environment.NewLine &
    '                                backupdatei & Environment.NewLine & Environment.NewLine &
    '                                "Es wird nun das Backupverzeichnis geöffnet. Sie können die Datei dann normal ins Paradigma einfügen",
    '                                "Übernahme der geänderten Datei ins Archiv", MessageBoxButton.OK, MessageBoxImage.Error
    '                                    )
    '                Process.Start(CO_test.DirectoryName)
    '            End If
    '            CO_test = Nothing
    '            '    BackupAnlegen(CO_test, dok)
    '        Else
    '            'datei wurde NICHT geändert
    '            OfficemerkerLoeschen(kandidat)
    '        End If
    '        meincount = 18
    '        'nachricht("checkoutDokuList2: checkoutDokuList wurde gelöscht")
    '        'checkoutDokuList.Clear()
    '        meincount = 19
    '        Return False
    '    Catch ex As Exception
    '        nachricht("Fehler2 inwurdenDokumenteGeaendert: meincount: " & meincount & ", " & errorout & vbCrLf ,ex)
    '        Return False
    '    End Try
    'End Function

    'Public Shared Sub OfficemerkerLoeschen(kandidat As Dokument)
    '    If kandidat Is Nothing Then
    '        myglobalz.sitzung.wordDateiImEditModus.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
    '        myglobalz.sitzung.excelDateiImEditModus.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
    '        myglobalz.WordSperreeschonAktiv = False
    '        myglobalz.ExcelSperreschonAktiv = False
    '        Exit Sub
    '    End If
    '    If kandidat.getDokTyp = DokumentenTyp.DOC Then
    '        myglobalz.sitzung.wordDateiImEditModus.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
    '        myglobalz.WordSperreeschonAktiv = False
    '    End If
    '    If kandidat.getDokTyp = DokumentenTyp.XLS Then
    '        myglobalz.sitzung.excelDateiImEditModus.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
    '        myglobalz.ExcelSperreschonAktiv = False
    '    End If
    'End Sub

    'Private Shared Sub bildeKandidatExcel(geloeschteDatei As String, ByRef kandidat As Dokument, ByRef hatKandidat As Boolean)
    '    Try
    '        If geloeschteDatei.IsNothingOrEmpty Then
    '            nachricht("fehler in bildeKandidatExcel, geloeschteDatei is nothingoremty")
    '        End If
    '        If myglobalz.sitzung.excelDateiImEditModus.tempEditDatei.IsNothingOrEmpty Then
    '            nachricht("fehler in bildeKandidatExcel, exceldatei.tempEditDatei is nothingoremty")
    '        End If
    '        If geloeschteDatei.ToLower.EndsWith(".xls") Or
    '       geloeschteDatei.EndsWith(".xlsx") Then
    '            If geloeschteDatei.ToLower.Contains(myglobalz.sitzung.excelDateiImEditModus.tempEditDatei.ToLower) Then
    '                nachricht("exceldatei wurde geschlossen")
    '                kandidat = CType(myglobalz.sitzung.excelDateiImEditModus.Clone, Dokument)
    '                kandidat.FullnameCheckout = myglobalz.sitzung.excelDateiImEditModus.FullnameCheckout
    '                kandidat.FullnameImArchiv = myglobalz.sitzung.excelDateiImEditModus.FullnameImArchiv
    '                kandidat.tempEditDatei = myglobalz.sitzung.excelDateiImEditModus.tempEditDatei
    '                kandidat.DateinameMitExtension = myglobalz.sitzung.excelDateiImEditModus.DateinameMitExtension
    '                kandidat.revisionssicher = myglobalz.sitzung.excelDateiImEditModus.revisionssicher
    '                hatKandidat = True
    '            End If
    '        End If
    '    Catch ex As Exception
    '        nachricht("fehler in bildeKandidatExcel: ", ex)
    '    End Try
    'End Sub

    'Private Shared Sub bildeKandidatWord(geloeschteDatei As String, ByRef kandidat As Dokument, ByRef hatKandidat As Boolean)
    '    Try
    '        If geloeschteDatei.IsNothingOrEmpty Then
    '            nachricht("fehler in bildeKandidatWord, geloeschteDatei is nothingoremty")
    '            hatKandidat = False
    '            Exit Sub
    '        End If
    '        If myglobalz.sitzung.wordDateiImEditModus.tempEditDatei.IsNothingOrEmpty Then
    '            nachricht("fehler in bildeKandidatWord, wordDateiImEditModus.tempEditDatei is nothingoremty")

    '            Exit Sub
    '        End If
    '        If geloeschteDatei.ToLower.EndsWith(".doc") Or
    '           geloeschteDatei.ToLower.EndsWith(".docx") Then
    '            If geloeschteDatei.ToLower.Contains(myglobalz.sitzung.wordDateiImEditModus.tempEditDatei.ToLower) Then
    '                nachricht("gelöschteDatei entspricht der editdatei")
    '                nachricht("worddatei wurde geschlossen")
    '                kandidat = CType(myglobalz.sitzung.wordDateiImEditModus.Clone, Dokument)
    '                kandidat.FullnameCheckout = myglobalz.sitzung.wordDateiImEditModus.FullnameCheckout
    '                kandidat.FullnameImArchiv = myglobalz.sitzung.wordDateiImEditModus.FullnameImArchiv
    '                kandidat.tempEditDatei = myglobalz.sitzung.wordDateiImEditModus.tempEditDatei
    '                kandidat.DateinameMitExtension = myglobalz.sitzung.wordDateiImEditModus.DateinameMitExtension
    '                kandidat.revisionssicher = myglobalz.sitzung.wordDateiImEditModus.revisionssicher
    '                hatKandidat = True
    '            End If
    '        End If
    '    Catch ex As Exception
    '        nachricht("fehler in bildeKandidatWord: ", ex)
    '    End Try
    'End Sub

    'Private Shared Function getDateiistneuerAlsImArchiv(fullnamecheckout As String, FullnameImArchiv As String) As Boolean
    '    Dim CO_test, AR_test As IO.FileInfo
    '    CO_test = New IO.FileInfo(fullnamecheckout)
    '    AR_test = New IO.FileInfo(FullnameImArchiv)

    '    nachricht("Alt: " & AR_test.LastWriteTime.ToString)
    '    nachricht("Neu: " & CO_test.LastWriteTime.ToString)
    '    Return AR_test.LastWriteTime < CO_test.LastWriteTime
    '    ' nachricht("dateiWurdeGeaendert: " & dateiIstNeuerAlsImArchiv)
    '    AR_test = Nothing ' sonst läßt sie sich nicht überschreiben
    '    CO_test = Nothing
    'End Function

    Friend Shared Function IllegalerEintragVorhanden(aktVorgangsID As Integer) As Boolean
        Dim oldIlleg As New clsIllegaleHuette
        oldIlleg = modIllegaleHuette.getIllegale4Vid(myGlobalz.sitzung.aktVorgangsID)
        If oldIlleg Is Nothing OrElse oldIlleg.illegID < 1 Then
            Return False
        Else
            Return True
        End If
    End Function
    Public Shared Sub SG3307_eintragAnlegen(sgnr As String)
        If sgnr.Trim = "3307" Then
            'existiert schon ein eintrag in der illegalenTabelle?
            If Not detailsTools.IllegalerEintragVorhanden(myGlobalz.sitzung.aktVorgangsID) Then
                nachricht("illegale Hütte, es fehlt der Eintrag im Modul. ")
                ' illeg.illegID = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("illegid")))
                'illeg.gebiet = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("gebiet")))
                'illeg.vermerk = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("vermerk")))
                'illeg.anhoerung = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("anhoerung")))
                'illeg.raeumungBisDatum = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("raeumungBisDatum")))
                'illeg.raeumung = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("raeumung")))
                'illeg.raeumungsTyp = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("raeumungstyp")))
                'illeg.verfuegung = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("verfuegung")))
                'illeg.fallerledigt = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("fallerledigt")))
                'illeg.eid_anhoerung = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("eid_anhoerung")))
                'illeg.eid_raeumung = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("eid_raeumung")))
                'illeg.eid_verfuegung = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("eid_verfuegung")))
                Dim illeg As New clsIllegaleHuette With {
                    .status = "4",
                    .vid = myGlobalz.sitzung.aktVorgangsID,
                    .quelle = CStr(myGlobalz.sitzung.aktBearbeiter.Initiale)
                }
                'illeg.ts = now
                illeg.illegID = modIllegaleHuette.huettespeichern("neu", illeg)
                If illeg.illegID > 0 Then
                    nachricht("Hütte wurde erfolgreich angelegt")
                Else
                    nachricht("Hütte wurde NICHT erfolgreich angelegt")
                End If
            End If
        End If
    End Sub
    Public Shared Function IsFileWritable(ByVal path As String) As Boolean
        Try
            If Not IO.File.Exists(path) Then
                Return True
            End If
            Using stream As System.IO.FileStream = IO.File.OpenWrite(path)
                stream.Close()
            End Using
            Return True
        Catch generatedExceptionName As Exception
            nachricht("  in IsFileWritable: ", generatedExceptionName)
            Return False
        Finally

        End Try
    End Function

    Private Shared Function KopiereZu(ByVal CO_test As IO.FileInfo, ByVal fullname As String) As Boolean
        Try
            ' Return False
            nachricht("KopiereZu: " & CO_test.FullName & " nach: " & fullname)
            CO_test.CopyTo(fullname, True)
            Return True
        Catch ex As Exception
            nachricht("fehler in KopiereZu, übernahme der änderung ins archiv nicht geglückt: " & ex.ToString & fullname)
            Return False
        End Try
    End Function

    Public Shared Sub darstellen(ByVal dokumentWurdeGeoeffnet As Boolean)
        If Not myGlobalz.sitzung.aktDokument.isTypeEditable Then
            Exit Sub
        End If
    End Sub

    Shared Function geaenderteDateiInsArchivUebernehmen(ByVal CHeckOut_test As IO.FileInfo, ByVal dok As Dokument) As Boolean
        'Ins Archiv übernhemen
        'todo frag ob als neue version ins Archiv übernehmen
        l("geaenderteDateiInsArchivUebernehmen-------------------------------------")
        Try
            If myGlobalz.sitzung.aktDokument.revisionssicher Then
                nachricht("warnung revisionssichere dateien dürfen nicht geändert werden!")
                '   MsgBox("Die revisionssichere Datei darf nicht geändert werden: " & myGlobalz.sitzung.aktDokument.DateinameMitExtension & " Abbruch")
                Return False
            End If
            If CHeckOut_test.Exists Then
                'CHeckOut_test ist quelle
                Threading.Thread.Sleep(1000)
                Dim fullname As String = dok.FullnameImArchiv
                inputFileReadonlyEntfernen(dok.FullnameImArchiv)
                If ArchivDateiNachVersionBackupUmbenennen(dok) Then
                    nachricht("archivdatei umbenannt")
                Else
                    nachricht("fehler archivdatei konnte nicht umbenannt werden " & dok.FullnameImArchiv)
                End If
                glob2.MeinGarbage()

                If imarchivSichernErfolgreich(CHeckOut_test, fullname) Then
                    ' Return true  weiter zur DB
                Else
                    Return False
                End If
            End If

            CLstart.myc.aLog.komponente = "Dokumente"
            CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktDokument.DocID & " " &
                                    myGlobalz.sitzung.aktDokument.DateinameMitExtension & ": geaenderta"
            CLstart.myc.aLog.log()
            myGlobalz.sitzung.aktDokument.Filedatum = Now
            Dim result As Integer = DokArcTools.dokUpdate.execute(myGlobalz.sitzung.aktDokument.DocID,
                                                                  myGlobalz.sitzung.aktDokument.istVeraltet,
                                                                  myGlobalz.sitzung.aktDokument.Beschreibung,
                                                                  myGlobalz.sitzung.aktDokument.revisionssicher,
                                                                  myGlobalz.sitzung.aktDokument.Filedatum,
                                                                  myGlobalz.sitzung.aktDokument.EXIFlat,
                                                                  myGlobalz.sitzung.aktDokument.EXIFlon)
            CLstart.myc.aLog.log() ' zuletzt, weil fehler vorkommen!!!
            Return True
        Catch ex As Exception
            nachricht("Fehler: in geaenderteDateiInsArchivUebernehmen: ", ex)
            Return False
        End Try
    End Function

    Private Shared Function ArchivDateiNachVersionBackupUmbenennen(dok As Dokument) As Boolean
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
            nachricht("fehler in ArchivDateiNachVersionBackupUmbenennen: ", ex)
            Return False
        End Try
    End Function



    Private Shared Function getVersionsBackupName(fullnameCheckout As String, name As String, extension As String, directory As String) As String
        Dim versname As String = ""
        Try
            versname = name.ToLower
            If extension.IsNothingOrEmpty Then
                versname = versname
                versname = versname & "_VersBackup_" & Format(Now, "yyyyMMddhhmmss_ffff")
            Else
                versname = versname.Replace(extension, "")
                versname = versname & "_VersBackup_" & Format(Now, "yyyyMMddhhmmss_ffff") & "." & extension
            End If
            '   versname = directory & "\" & versname die My.Computer.FileSystem.RenameFile meth benötigt hier kei directory
            Return versname
        Catch ex As Exception
            nachricht("fehler in getVersionsBackupName: " & fullnameCheckout, ex)
            '  Return False
            Return "fehler"
        End Try
    End Function

    'Private Shared Function ArchivDateiLoeschen(dok As Dokument) As Boolean
    '    Try
    '        Dim fi As New IO.FileInfo(dok.FullnameImArchiv)
    '        fi.Delete()
    '        fi = Nothing
    '        Return True
    '    Catch ex As Exception
    '        nachricht("fehler in ArchivDateiLoeschen: ", ex)
    '        Return False
    '    End Try
    'End Function

    Private Shared Function imarchivSichernErfolgreich(cHeckOut_test As FileInfo, fullname As String) As Boolean
        For i = 1 To 5
            If KopiereZu(cHeckOut_test, fullname) Then
                'alles prima
                nachricht("geänderte Datei wurde im archiv aktualisiert, zaehler : " & i)
                Return True
            Else
                nachricht("fehler geänderte Datei wurde im archiv NICHT aktualisiert" & i)
                ' MsgBox(glob2.getMsgboxText("dateiNichtUebernommen", New List(Of String)(New String() {})), MsgBoxStyle.Critical, "Datei: " & CHeckOut_test.Name)
                Return False
            End If
        Next
        Return False
    End Function

    'Private Shared Function lokaleKopieLoeschenErfolgreich(ByVal CO_test As IO.FileInfo, ByVal dok As Dokument) As Boolean
    '    Try
    '        If CO_test.Exists Then CO_test.Delete() 'In jedem Falle das Dokument löschen
    '        Return True
    '    Catch ex As Exception
    '        If Not CO_test.Extension.ToLower.Contains("jpg") Then
    '            MsgBox(glob2.getMsgboxText("BitteDateiSchliessen", New List(Of String)(New String() {dok.DateinameMitExtension})),
    '               MsgBoxStyle.OkOnly, "Office-Dokument schließen!")
    '            'ihah CO_test.Delete() 'In jedem Falle das Dokument löschen
    '        End If
    '        Return False
    '    End Try
    'End Function
    Public Shared Sub eEreignisstarten(ByVal item As String, readOnlyDoxsInTxtCrtlOeffnen As Boolean)
        nachricht("USERAKTION:   ereignis aktennotiz hinzufügen pulldown")
        If ereignisauswahlistOK((item)) Then
            starteEreignisdetail((item), readOnlyDoxsInTxtCrtlOeffnen)
        End If
    End Sub


    Shared Function ereignisauswahlistOK(ByVal typ As String) As Boolean
        If typ.ToLower <> "hinzufügen" Then Return True
        Return False
    End Function



    Public Shared Sub startestandardereignis(ByVal typ As String, readOnlyDoxsInTxtCrtlOeffnen As Boolean)
        Dim wzereignisdetail As New Window_Ereignis_Detail(typ, (readOnlyDoxsInTxtCrtlOeffnen))
        wzereignisdetail.ShowDialog()
    End Sub

    Shared Sub starteEreignisdetail(ByVal typ As String, readOnlyDoxsInTxtCrtlOeffnen As Boolean)
        myGlobalz.sitzung.Ereignismodus = "neu"
        myGlobalz.sitzung.aktEreignis.Datum = Now
        myGlobalz.sitzung.aktEreignis.Art = typ
        myGlobalz.sitzung.aktEreignis.istRTF = False

        Select Case typ.ToLower
            Case "wiedervorlage"
                wiedervorlagestarten()
            Case "aktennotiz schreiben"
                Aktenotizerstellen(False)
                'Aktenotizerstellen(True)
            Case "email schreiben"
                '  myGlobalz.Arc.AllesAuscheckenVorgang(False)
                glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email, "", "", "",
                                       myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, False)
            Case "outlookemail übernehmen"
                outlookemnailUebernehmen(myGlobalz.sitzung.aktVorgangsID)
            Case "zahlung"
                glob2.ZahlungFormoeffen()
            Case "notiz"
                startestandardereignis(typ, readOnlyDoxsInTxtCrtlOeffnen)
            Case "weblink"
                glob2.webLinkedit("neu")
            Case Else
                startestandardereignis(typ, readOnlyDoxsInTxtCrtlOeffnen)
        End Select
        clsEreignisTools.setLetztesEreignisText(myGlobalz.sitzung.aktEreignis)
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LASTACTIONHEROE")

        ' EditspeichernStammdaten_alledb(myGlobalz.sitzung.aktEreignis.Datum)
    End Sub

    Shared Function setSQLBodySingleUpdate(SUfieldname As String) As String
        If String.IsNullOrEmpty(SUfieldname) Then Return ""
        Return " set " & " " & SUfieldname & "=@" & SUfieldname & ""
    End Function
    Public Shared Sub Edit_singleUpdate_Stammdaten(ByVal zeitstempel As Date, singleUpdateFieldName As String)
        Try
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            querie = "UPDATE " & CLstart.myViewsNTabs.TABSTAMMDATEN & " " & setSQLBodySingleUpdate(singleUpdateFieldName) & "  where VorgangsID=@VorgangsID"
            clsSqlparam.paramListe.Add(New clsSqlparam("VorgangsID", myGlobalz.sitzung.aktVorgangsID))


            Select Case singleUpdateFieldName
                Case "STELLUNGNAHME"
                    clsSqlparam.paramListe.Add(New clsSqlparam("STELLUNGNAHME", Convert.ToInt16(myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme)))
                Case "ORTSTERMIN"
                    clsSqlparam.paramListe.Add(New clsSqlparam("ORTSTERMIN", Convert.ToInt16(myGlobalz.sitzung.aktVorgang.Stammdaten.Ortstermin)))

                Case "BESCHREIBUNG"
                    clsSqlparam.paramListe.Add(New clsSqlparam("BESCHREIBUNG", myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung))
                Case "HATRAUMBEZUG"
                    clsSqlparam.paramListe.Add(New clsSqlparam("HATRAUMBEZUG", myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug))
                Case "BEMERKUNG"
                    clsSqlparam.paramListe.Add(New clsSqlparam("BEMERKUNG", myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung))
                Case "GEMKRZ"
                    clsSqlparam.paramListe.Add(New clsSqlparam("GEMKRZ", myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ))
                Case "WEITEREBEARB"
                    clsSqlparam.paramListe.Add(New clsSqlparam("WEITEREBEARB", myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter))
                Case "PROBAUGAZ"
                    clsSqlparam.paramListe.Add(New clsSqlparam("PROBAUGAZ", myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz))
                Case "ALTAZ"
                    clsSqlparam.paramListe.Add(New clsSqlparam("ALTAZ", myGlobalz.sitzung.aktVorgang.Stammdaten.AltAz))
                Case "INTERNENR"
                    clsSqlparam.paramListe.Add(New clsSqlparam("INTERNENR", myGlobalz.sitzung.aktVorgang.Stammdaten.InterneNr))
                Case "PARAGRAF"
                    clsSqlparam.paramListe.Add(New clsSqlparam("PARAGRAF", myGlobalz.sitzung.aktVorgang.Stammdaten.Paragraf))
                Case "STORAUMNR"
                    clsSqlparam.paramListe.Add(New clsSqlparam("STORAUMNR", myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.RaumNr))
                Case "GUTACHTENMIT"
                    clsSqlparam.paramListe.Add(New clsSqlparam("GUTACHTENMIT", Convert.ToInt16(myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.existiert)))
                Case "GUTACHTENDRIN"
                    clsSqlparam.paramListe.Add(New clsSqlparam("GUTACHTENDRIN", Convert.ToInt16(myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.UnterDokumente)))
                Case "DARFNICHTVERNICHTETWERDEN"
                    clsSqlparam.paramListe.Add(New clsSqlparam("DARFNICHTVERNICHTETWERDEN", Convert.ToInt16(myGlobalz.sitzung.aktVorgang.Stammdaten.darfNichtVernichtetWerden)))
                Case "ERLEDIGT"
                    clsSqlparam.paramListe.Add(New clsSqlparam("ERLEDIGT", Convert.ToInt16(myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt)))
                Case "LETZTEBEARBEITUNG"
                    clsSqlparam.paramListe.Add(New clsSqlparam("LETZTEBEARBEITUNG",
                                                      clsDBtools.makedateMssqlConform(zeitstempel, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))

                Case "LASTACTIONHEROE"
                    If myGlobalz.sitzung.aktVorgang.Stammdaten.LastActionHeroe.Length > 545 Then
                        myGlobalz.sitzung.aktVorgang.Stammdaten.LastActionHeroe = myGlobalz.sitzung.aktVorgang.Stammdaten.LastActionHeroe.Substring(0, 540)
                    End If
                    clsSqlparam.paramListe.Add(New clsSqlparam("LASTACTIONHEROE", CStr(myGlobalz.sitzung.aktVorgang.Stammdaten.LastActionHeroe)))
                Case "EINGANG"
                    clsSqlparam.paramListe.Add(New clsSqlparam("EINGANG",
                                                                                         clsDBtools.makedateMssqlConform(CDate(myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum),
                                                                                                                             myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))

            End Select

            Dim res = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
            nachricht("stammdatenupdate:")
        Catch ex As Exception
            nachricht("Fehler Edit_singleUpdate_Stammdaten ", ex)
        End Try
    End Sub

    Public Shared Sub Aktenotizerstellen(ueberVorlage As Boolean)
        If ueberVorlage Then
            Dim pfadAllgemein As String = ""
            Dim quelldatei As String
            Dim vorlagenVerzeichnis As IO.DirectoryInfo = Nothing
            Dim endung As String = ".docx"
            clsVorlagenTools.berechneVorlagenverzeichnis(vorlagenVerzeichnis, "Allgemein", pfadAllgemein)
            pfadAllgemein = pfadAllgemein.Replace("\\", "\")
            'quelldatei = pfadAllgemein & "\" & "Aktennotiz.docx"
            Dim vorl As New WinVorlagenListe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl,
                                       myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header,
                                       True, 0, "Aktennotiz.docx")
            vorl.ShowDialog()
            nachricht("WinVorlageSteuerung weiter: 7")
            nachricht("WinVorlageSteuerung weiter: ENDE")
        Else
            makeReportKoloman("modus=aktennotiz")
        End If
        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " & myGlobalz.sitzung.aktEreignis.Beschreibung & ": neueAktennotiz als RTF angelegt" : CLstart.myc.aLog.log()
    End Sub

    Public Shared Sub outlookemnailUebernehmen(aktvid As Integer)
        Dim fotoZuRaumbezug, erfolg As Boolean
        Dim memailtools As New clsOutlookEmail
        memailtools.Aufnahme(fotoZuRaumbezug, aktvid, erfolg)
        If erfolg Then
            If memailtools.returnAction.ToLower.StartsWith("dokushinzufuegen") Then
                Dim aktion As String = memailtools.returnAction
                Dim initialdir, a(), filenames(), pdfdatei As String
                Dim ereignisID As Integer
                a = aktion.Split("#"c)
                pdfdatei = a(1)
                Dim fi As New IO.FileInfo(a(1))
                initialdir = fi.DirectoryName
                Dim dcc As New winDokumenteEinchecken(filenames, ereignisID, initialdir, pdfdatei, "", False)
                dcc.ShowDialog()
            End If
        End If
        memailtools = Nothing
        GC.Collect()
    End Sub
    Public Shared Sub EMLemnailUebernehmen(ByVal emlfullpath As String, aktvid As Integer, ByRef problemMitanhang As Boolean)
        Dim fotoZuRaumbezug, erfolg As Boolean
        Dim memailtools As New clsEMLemail(emlfullpath)
        memailtools.Aufnahme(fotoZuRaumbezug, aktvid, erfolg, problemMitanhang)
        memailtools = Nothing
    End Sub

    Public Shared Sub wiedervorlagestarten()
        myGlobalz.sitzung.Wiedervorlagemodus = "neu"
        Dim wv As New Window_WiedervorlageDetail
        wv.ShowDialog()
    End Sub

    Public Shared Sub initErgeinistypCombo(ByVal dumm As System.Windows.Window, ByVal datei As String, ByVal xamlRescourceName As String) '"XMLSourceComboBoxEreignisse"
        Dim filename As String = myGlobalz.appdataDir & "\config\Combos\" & datei
        nachricht("COMBOXML: " & filename)
        Debug.Print(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.ToString)
        Dim existing As XmlDataProvider = TryCast(dumm.Resources(xamlRescourceName), XmlDataProvider)
        existing.Source = New Uri(filename$)
    End Sub
    Shared Sub DTaufFotoObjektabbilden(ByVal dokulok As Dokument, ByVal meineDT As DataTable) 'myGlobalz.sitzung.tempREC.dt
        Try
            With dokulok
                .DocID = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("dokumentid")))
                .dokumentPfad = clsDBtools.fieldvalue(meineDT.Rows(0).Item("relativpfad"))
                .DateinameMitExtension = clsDBtools.fieldvalue(meineDT.Rows(0).Item("dateinameext"))
                .Typ = clsDBtools.fieldvalue(meineDT.Rows(0).Item("typ"))
                .Beschreibung = clsDBtools.fieldvalue(meineDT.Rows(0).Item("beschreibung"))
                .Filedatum = CDate(clsDBtools.fieldvalue(meineDT.Rows(0).Item("Filedatum")))
                .Checkindatum = CDate(clsDBtools.fieldvalue(meineDT.Rows(0).Item("Checkindatum")))
                .istVeraltet = CBool(clsDBtools.toBool(meineDT.Rows(0).Item("veraltet")))
                .ExifDatum = CDate(clsDBtools.fieldvalue(meineDT.Rows(0).Item("ExifDatum")))
                .EXIFlon = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFlong")))
                .EXIFlat = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFlat")))
                .EXIFdir = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFdir")))
                .EXIFhersteller = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFhersteller")))

            End With
        Catch ex As Exception
            nachricht("Fehler2: DTaufFotoObjektabbilden " & vbCrLf & vbCrLf, ex)
        End Try
    End Sub

    Public Shared Sub AlleBeteiligtenLoeschen()
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the Await operator to the result of the call.
        clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(myGlobalz.sitzung.aktVorgangsID) ' myGlobalz.sitzung.beteiligteREC.dt
#Enable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the Await operator to the result of the call.
        Dim messi As New MessageBoxResult
        If Not myGlobalz.sitzung.beteiligteREC.dt.IsNothingOrEmpty Then
            messi = MessageBox.Show("Alle Beteiligten wirklich löschen ?" & vbCrLf,
                              "Alle Beteiligten löschen ?",
                              MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
                    clsBeteiligteBUSI.personAusVorgangEntfernen(CInt(zeile.Item("PersonenID")), myGlobalz.sitzung.aktVorgangsID, CInt(zeile.Item("Status")))
                Next
            End If
        Else
            MessageBox.Show("Es sind noch keine Beteiligten erfasst worden!")
        End If
    End Sub

    Friend Shared Function getMSGNotizText(ereignisID As Integer, ereignisDokListe As List(Of clsEreignisDok)) As String
        Try
            For Each edok As clsEreignisDok In ereignisDokListe
                If edok.ID = ereignisID Then
                    Return edok.Notiz
                End If
            Next
            Return ""
        Catch ex As Exception
            nachricht("fehler in getMSGNotizText: ", ex)
            Return ""
        End Try
    End Function

    Public Shared Sub AlleBeteiligtenKopieren(ByVal quellvorgangsid As Integer, ByVal Zielvorgangsid%)
        '  clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(quellvorgangsid) 'myGlobalz.sitzung.VorgangsID) ' myGlobalz.sitzung.beteiligteREC.dt
        clsBeteiligteBUSI.initBeteiligteDatatable(quellvorgangsid, myGlobalz.sitzung.beteiligteREC) 'myGlobalz.sitzung.beteiligteREC wird aktualisert

        clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", "System.Int16")
        clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", 1)

        Dim messi As New MessageBoxResult

        If Not myGlobalz.sitzung.beteiligteREC.dt.IsNothingOrEmpty Then
            For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
                myGlobalz.sitzung.aktPerson = clsBeteiligteBUSI.convertItem2person(zeile)
                'clsBeteiligteBUSI.BeteiligtenRec2Obj(zeile, myGlobalz.sitzung.aktPerson)
                clsBeteiligteBUSI.personZuZielvorgangKopieren(CInt(zeile.Item("PersonenID")), CInt(zeile.Item("Status")), quellvorgangsid, Zielvorgangsid)
            Next
        Else
            MessageBox.Show("Es sind noch keine Beteiligten erfasst worden!")
        End If
    End Sub



    Shared Function AlleDokumentenKopieren(ByVal quellVorgangsID As Integer, ByVal ZielvorgangsIdInput As Integer, doppelteDokusAuchKopieren As Boolean,
                                           allebilder As Boolean) As String
        'alledokus auflisten
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(quellVorgangsID), "beides", allebilder, 0) ' nach myGlobalz.Arc.ArcRec
        Dim kopiert = 0
        Dim kopierteEreignisse As Integer = 0
        Dim result As String
        Dim NumDir As String = ""
        'NumDir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.defineArchivVorgangsDir(ZielvorgangsIdInput))
        If bresult Then
            For i = 0 To myGlobalz.Arc.ArcRec.dt.Rows.Count - 1
                DokArc.DokuZeile2OBJ(i, myGlobalz.sitzung.aktDokument, myGlobalz.Arc.ArcRec.dt)
                'korrekturFehlendesExifDatum
                If myGlobalz.sitzung.aktDokument.ExifDatum < CDate("1970-01-01") Then
                    myGlobalz.sitzung.aktDokument.ExifDatum = myGlobalz.sitzung.aktDokument.Filedatum
                End If
                Dim ausgabeVerzeichnis As String = ""
                myGlobalz.sitzung.aktDokument.makeFullname_Checkout(quellVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
                checkout.checkout(myGlobalz.sitzung.aktDokument, quellVorgangsID)   'checkout findet IMMER statt
                If Not doppelteDokusAuchKopieren Then
                    If dokumentschonImVorgangvorhanden(myGlobalz.sitzung.aktDokument, ZielvorgangsIdInput) Then
                        Continue For
                    End If
                End If


                Dim datei As String = myGlobalz.sitzung.aktDokument.FullnameCheckout
                Dim Beschreibung As String = myGlobalz.sitzung.aktDokument.Beschreibung
                Dim ereignisid As Integer = myGlobalz.sitzung.aktDokument.EreignisID ' ihah  DokArcOracle.getEreignisID4DokId(myGlobalz.sitzung.aktDokument.DocID)
                ' Dim exifdatum As date = myGlobalz.sitzung.aktDokument.ExifDatum ' ihah  DokArcOracle.getEreignisID4DokId(myGlobalz.sitzung.aktDokument.DocID)
                Dim filedatum As Date = myGlobalz.sitzung.aktDokument.Filedatum ' ihah  DokArcOracle.getEreignisID4DokId(myGlobalz.sitzung.aktDokument.DocID)

                myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)

                myGlobalz.sitzung.aktDokument.newSaveMode = True 'weil dokument neu erzeugt wird
                ' myGlobalz.sitzung.aktEreignis.ID ist unbekannt
                If ereignisid > 0 Then
                    'zu diesem Dokument gibt es ein Ereignis
                    Dim erfolg As Boolean = clsEreignisTools.Ereigniskopieren(ereignisid, ZielvorgangsIdInput, False)
                    If erfolg Then
                        myGlobalz.sitzung.aktDokument.EreignisID = myGlobalz.sitzung.aktEreignis.ID
                        kopierteEreignisse += 1
                    Else
                        myGlobalz.sitzung.aktDokument.EreignisID = 0
                    End If
                End If

                Dim erfolgreich As Boolean = myGlobalz.Arc.checkINDoku(datei,
                                                                       myGlobalz.sitzung.aktDokument.EreignisID,
                                                                       Beschreibung,
                                                                       ZielvorgangsIdInput,
                                                                       False,
                                                                       NumDir,
                                                                       filedatum,
                                                                       myGlobalz.sitzung.aktDokument.DocID,
                                                                       myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID)
                If erfolgreich Then
                    nachricht("Checkin  erfolgreich:  " & datei)
                    kopiert += 1
                Else
                    nachricht_und_Mbox("Checkin nicht erfolgreich: " & datei)
                End If
            Next
            result = "Kopieren von Dokumenten: Es wurden " & kopiert & " Dokumente kopiert"
        Else
            MsgBox("Es sind noch keine Dokumente im Vorgang gespeichert!", MsgBoxStyle.Information, "Dokumente kopieren")
            result = "Es sind noch keine Dokumente im Vorgang gespeichert"
        End If
        Return result
    End Function

    Public Shared Sub AlleEreignisseLoeschen(ereignisDokListe As List(Of clsEreignisDok))
        Dim hinweis As String = ""
        Try
#Disable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the Await operator to the result of the call.
            detailsTools.initEreigisseDatatable(hinweis, myGlobalz.sitzung.aktVorgangsID, False, ereignisDokListe)
#Enable Warning BC42358 ' Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the Await operator to the result of the call.
            Dim wirklichLoeschen As Boolean = False

            If Not myGlobalz.sitzung.EreignisseRec.dt.IsNothingOrEmpty Then
                wirklichLoeschen = getUserInput("Alle Ereignisse wirklich löschen ?")
                If wirklichLoeschen Then
                    For Each zeile As DataRow In myGlobalz.sitzung.EreignisseRec.dt.Rows
                        clsEreignisTools.Ereignis_und_Dokumente_entkoppeln(CInt(zeile.Item("ID")))
                        clsEreignisTools.ereignisLoeschen_alleDB(CInt(zeile.Item("ID")))
                    Next
                End If
            Else
                MessageBox.Show("Es sind noch keine Ereignisse erfasst worden!")
            End If
        Catch ex As Exception
            nachricht("fehler in AlleEreignisseLoeschen: ", ex)
        End Try
    End Sub

    Private Shared Function getDokid4RB(ByVal zeile As DataRow) As Integer
        Dim DocID As Integer
        Try
            DocID = CInt(clsDBtools.fieldvalue(zeile.Item("dokumentid")))
            Return DocID
        Catch ex As Exception
            nachricht("fehler in getDokid4RB: ", ex)
            Return -1
        End Try
    End Function

    Public Shared Sub AlleRaumbezuegeLoeschen(vid As Integer)
        nachricht("AlleRaumbezuegeLoeschen----------------------")
        Dim erfolg As Boolean = RBtoolsns.initraumbezugsDT_alleDB.exe(vid)
        If erfolg Then RBtoolsns.statusSpalteErgaenzenUndFuellen.execute(myGlobalz.sitzung.raumbezugsRec.dt,
                                                                         myGlobalz.sitzung.RaumbezugsIDsDT,
                                     "Status", "RaumbezugsID") 'landet in myGlobalz.sitzung.raumbezugsRec.dt       
        Dim messi As New MessageBoxResult
        If Not myGlobalz.sitzung.raumbezugsRec.dt.IsNothingOrEmpty Then
            messi = MessageBox.Show("Alle Raumbezüge wirklich löschen ?" & vbCrLf,
                              "Alle Raumbezüge löschen ?",
                              MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                For Each zeile As DataRow In myGlobalz.sitzung.raumbezugsRec.dt.Rows
                    nachricht(zeile.Item("TYP").ToString & " " & zeile.Item("Sekid").ToString)
                    Dim item As String = zeile.Item("TYP").ToString
                    Select Case item.ToString
                        Case CInt(RaumbezugsTyp.Adresse).ToString
                            RBtoolsns.raumbezugsDT2Obj.exe(zeile, myGlobalz.sitzung.aktADR)
                            AdressTools.loeschenRBAdresseOhneNachfrage()
                        Case CInt(RaumbezugsTyp.Flurstueck).ToString
                            RBtoolsns.raumbezugsDT2Obj.exe(zeile, myGlobalz.sitzung.aktFST)
                            FST_tools.loeschenRBFlurstueckExtracted()
                        Case CInt(RaumbezugsTyp.Foto).ToString
                            RBtoolsns.raumbezugsDT2Obj.exe(zeile, myGlobalz.sitzung.aktParaFoto)

                            fotoTool.loeschenRBFotoBULK(getDokid4RB(zeile))
                        Case CInt(RaumbezugsTyp.Umkreis).ToString
                            RBtoolsns.raumbezugsDT2Obj.exe(zeile, myGlobalz.sitzung.aktPMU)
                            ParaUmkreisTools.loeschenAktPMU()
                        Case CInt(RaumbezugsTyp.Polygon).ToString
                            RBtoolsns.raumbezugsDT2Obj.exe(zeile, myGlobalz.sitzung.aktPolygon)
                            ParaUmkreisTools.loeschenAktPolygon(CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID))

                        Case CInt(RaumbezugsTyp.Polyline).ToString
                            RBtoolsns.raumbezugsDT2Obj.exe(zeile, myGlobalz.sitzung.aktPolyline)
                            If myGlobalz.sitzung.aktPolyline.RaumbezugsID < 1 Then
                                myGlobalz.sitzung.aktPolyline.RaumbezugsID = myGlobalz.sitzung.aktPolygon.RaumbezugsID
                            End If
                            ParaUmkreisTools.loeschenAktPolygon(CInt(myGlobalz.sitzung.aktPolyline.RaumbezugsID))
                    End Select
                Next
            End If
            paradigmaInstanzGISproc = Nothing
        Else
            MessageBox.Show("Es sind noch keine Ereignisse erfasst worden!")
        End If
    End Sub

    Public Shared Function getNewVgrundString(bestand As String) As String
        Dim sb As New System.Text.StringBuilder
        If myGlobalz.layerListControlObjekts.Count < 1 Then
            Return bestand
        End If
        For Each ele As clsLayerListItem In myGlobalz.layerListControlObjekts
            If ele.IsChecked Then
                sb.Append(ele.Titel & ";")
            End If
            'If Not ele.IsChecked Then
            '    sb.Append(ele.Titel & ";")
            'End If
            If ele.istAktiveEbene Then
                CLstart.myc.kartengen.aktMap.ActiveLayer = ele.Titel
                nachricht("in getNewVgrundString: aktive Ebene: " & ele.Name & ", " & ele.Titel & ", " & ele.Id)
            End If
        Next
        Return sb.ToString
    End Function

    Shared Function feststehendeTexteLaden(datei As String) As String
        Dim root As String = "O:\UMWELT\B\Vordruck_paradigma\feststehendeTexte\"
        Dim result As String = ""
        'Dim datei As String = ""
        Try
            l(" feststehendeTexte ---------------------- anfang")
            datei = IO.Path.Combine(root, datei)
            result = IO.File.ReadAllText(datei)
            l(" feststehendeTexte ---------------------- ende")
            Return result
        Catch ex As Exception
            l("Fehler in feststehendeTexte: ", ex)
            Return result
        End Try
        'Return "hier die Stellungnahme des Immissionsschutzes / der Unteren Naturschutzbehörde / der Unteren Wasserbehörde zu o.g. Angelegenheit." & Environment.NewLine &
        '                                   "Bitte senden Sie uns Ihre Bau-/Abbruchgenehmigung bzw. Versagung der Bau-/Abbruchgenehmigung auch als PDF zu, " &
        '                                   "Danke!"
    End Function
    Friend Shared Function getAktThumbNailNotiz(ereignisID As String, aktThumbnailNotiz As String, ereignisDokListe As List(Of clsEreignisDok)) As String
        Try
            l(" MOD getAktThumbNailNotiz anfang")
            For Each ele As clsEreignis In ereignisDokListe
                If ele.ID = CInt(ereignisID) Then
                    If ele.Notiz.IsNothingOrEmpty Then Continue For
                    aktThumbnailNotiz = ele.Beschreibung & Environment.NewLine & Environment.NewLine & ele.Notiz
                End If
            Next
            l(" MOD getAktThumbNailNotiz ende")
            Return aktThumbnailNotiz
        Catch ex As Exception
            l("Fehler in getAktThumbNailNotiz: ", ex)
            Return ""
        End Try
    End Function
    Public Shared Sub editDokumentMetadata()
        Dim metadoku As New WINdokumentMetaEdit
        metadoku.ShowDialog()
    End Sub
    Public Shared Function Archiv_aktiviere_Dokument(dokumenteRitemousekeypressed As Boolean, readOnlyDoxsInTxtCrtlOeffnen As Boolean, aktThumbnailNotiz As String,
                                                     alleBilder As Boolean, eid As Integer) As Boolean
        Dim box As WINBox
        Dim lfehler As String = ""
        If Not dokumenteRitemousekeypressed Then
            box = New WINBox("dokument", readOnlyDoxsInTxtCrtlOeffnen, aktThumbnailNotiz) With {
                .knopfnummer = 1
            }
        Else
            box = New WINBox("dokument", readOnlyDoxsInTxtCrtlOeffnen, aktThumbnailNotiz)
            box.ShowDialog()
        End If
        GC.Collect()
        '  MsgBox("knopfnummer:" & box.knopfnummer)
        If box.knopfnummer = 1 Then
            nachricht("USERAKTION: Einzelnes Dokument öffnen")
            box.dokumentWurdeGeoeffnet = False            ' Me.Close()
            myGlobalz.sitzung.aktDokument.nurzumlesen = Not myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter)
            If myGlobalz.sitzung.aktDokument.nurzumlesen Then
                If myGlobalz.sitzung.aktDokument.Initiale.ToLower.Trim = myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower.Trim Then
                    myGlobalz.sitzung.aktDokument.nurzumlesen = False
                Else
                    MessageBox.Show("Sie haben keine ausreichenden Rechte um die Datei zu ändern.", "Nur Vorschau möglich")
                End If
                'Return False
            End If
            If DokArc.machCheckout(lfehler) Then
                nachricht("machCheckout erfolgreich")
                FileArchivTools.inputFileReadonlyEntfernen(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                l("nurzumlesen1:  " & myGlobalz.sitzung.aktDokument.nurzumlesen & myGlobalz.sitzung.aktDokument.FullnameCheckout)
                DokArc.Archiv_aktiviere_dokument(myglobalz.sitzung.aktDokument, readOnlyDoxsInTxtCrtlOeffnen, alleBilder, eid, Application.givenVIDList)
                If myGlobalz.sitzung.aktDokument.nurzumlesen Then box.dokumentWurdeGeoeffnet = False
                Return True
            Else
                nachricht("machCheckout NICHT erfolgreich")
                If lfehler.StartsWith("fehlerquellefehlt") Then
                    MessageBox.Show("Die Word/Exceldatei: " & myGlobalz.sitzung.aktDokument.FullnameCheckout & Environment.NewLine &
                              " ließ sich nicht aus dem Archiv holen, weil " & Environment.NewLine & Environment.NewLine &
                              " die Quelldatei fehlt !!!" & Environment.NewLine &
                              "   " & Environment.NewLine &
                              " Bitte klären Sie erst warum die Quelldatei fehlt! " & Environment.NewLine,
                              " Quelldatei nicht vorhanden")
                Else
                    MessageBox.Show("Die Word/Exceldatei: " & myGlobalz.sitzung.aktDokument.FullnameCheckout & Environment.NewLine &
                              " ließ sich nicht aus dem Archiv holen, weil " & Environment.NewLine & Environment.NewLine &
                              " eine gleichnamige Datei im Moment geöffnet ist !" & Environment.NewLine &
                              " Bitte schließen sie zuerst die offene Datei. " & Environment.NewLine &
                              " Es kann sonst zu Datenverlusten kommen. " & Environment.NewLine,
                              " Datei ist geöffnet (" & lfehler & ")")
                End If

                Return False
            End If
        End If


        If box.knopfnummer = 2 Then
            Dim modus = "metaedit"
            nachricht("USERAKTION: Einzelnes Dokument metaedit")
            ' If DokArc.machCheckout(modus) Then ' wozu checkout beim metaedit????
            box.dokumentWurdeGeoeffnet = False
            editDokumentMetadata()
            Return False
            '  DokArc.zulisteAddieren()
            'End If
        End If
        If box.knopfnummer = 3 Then
            nachricht("USERAKTION: Einzelnes Dokument löschen")
            If myGlobalz.sitzung.aktDokument.istNurVerwandt Then
                MessageBox.Show("Es handelt sich um ein verwandtes Dokument. Sie können verwandte Dokumente nicht löschen!")
                Return False
            End If
            If myGlobalz.sitzung.aktDokument.revisionssicher Then
                MessageBox.Show("Es handelt sich um ein revisionssicheres Dokument. Sie können revisionssichere Dokumente nicht löschen!")
                Return False
            End If
            If dokumentIstGeoeffnet(myGlobalz.sitzung.aktDokument) Then
                MessageBox.Show("Das Dokument ist noch geöffnet. " & Environment.NewLine &
                                "Sie können geöffnete Dokumente nicht löschen!" & Environment.NewLine &
                                "Bitte schließen Sie zuerst das Dokument.",
                                myGlobalz.sitzung.aktDokument.DateinameMitExtension)
                Return False
            End If
            If glob2.istloeschenErnstgemeint() Then
                DokArc.aktDokumentLoschen(myGlobalz.sitzung.aktDokument)
                'DokArc.ausCheckoutlisteEntfernen(myglobalz.sitzung.aktDokument, myglobalz.sitzung.checkoutDokuList)
                CLstart.myc.aLog.komponente = "Dokumente" : CLstart.myc.aLog.aktion = String.Format("{0} {1}: geloescht", myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.DateinameMitExtension) : CLstart.myc.aLog.log()
                Return False
            End If
        End If
        If box.knopfnummer = 4 Then
            nachricht("USERAKTION: Einzelnes Dokument mailen")
            ' vorher myglobalz.Arc.lokalerCheckoutcache & myglobalz.sitzung.aktVorgangsID

            myGlobalz.Arc.einzeldokument_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID & "\" & myGlobalz.sitzung.aktDokument.DocID,
                                                    myGlobalz.sitzung.aktDokument)
            FileArchivTools.inputFileReadonlyEntfernen(myGlobalz.sitzung.aktDokument.FullnameCheckout)

            glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email, "", "",
                                   myGlobalz.sitzung.aktDokument.FullnameCheckout,
                                   myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email,
                                   False)
        End If
        If box.knopfnummer = 5 Then
            nachricht("USERAKTION: Einzelnes Dokument kopieren")
            nachZielKopieren.NachZielKopieren.exe(myGlobalz.sitzung.aktDokument, "O:\UMWELT\B\2-neue Struktur")
        End If
        If box.knopfnummer = 6 Then
            nachricht("USERAKTION: Einzelnes Dokument kopieren nach dokumente")
            nachZielKopieren.NachZielKopieren.exe(myGlobalz.sitzung.aktDokument, Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments))
        End If
        If box.knopfnummer = 7 Then
            nachricht("USERAKTION: Einzelnes Dokument mailen an ba")
            'vorher myglobalz.Arc.lokalerCheckoutcache & myglobalz.sitzung.aktVorgangsID 
            myGlobalz.Arc.einzeldokument_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID & "\" & myGlobalz.sitzung.aktDokument.DocID,
                                                    myGlobalz.sitzung.aktDokument)
            'glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email,
            '                       "",
            '                       feststehendeTexteLaden("mailanBaAbspann.txt"),
            '                       myGlobalz.sitzung.aktDokument.FullnameCheckout,
            '                       myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email,
            '                       True)

            glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email,
                            "",
                            "",
                            myGlobalz.sitzung.aktDokument.FullnameCheckout,
                            myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email,
                            True)
        End If
        If box.knopfnummer = 8 Then
            nachricht("USERAKTION: Einzelnes Dokument im archiv kopieren")
            If DokArc.machCheckout(lfehler) Then
                If nachZielKopieren.DolumentImArchivKopieren.exe(myGlobalz.sitzung.aktDokument) Then
                    'erfolgreich
                Else
                    'nicht erfolgreich
                    MessageBox.Show("Fehler: Daten konnte nicht kopiert werden. " & Environment.NewLine &
                    "Vermutliche Ursache: Datei war noch im Zugriff!", "Datei ist noch im Zugriff?", MessageBoxButton.OK)
                End If
            Else
                MessageBox.Show("Fehler: Daten konnte nicht kopiert werden. " & Environment.NewLine &
              "Vermutliche Ursache: Datei war noch im Zugriff!" & Environment.NewLine &
              "Bitte schliessen sie die Datei !", "Datei ist noch im Zugriff?", MessageBoxButton.OK)
            End If
        End If
        If box.knopfnummer = 9 Then
            nachricht("USERAKTION: Einzelnes Dokument direkt drucken schnelldruck")
            If DokArc.machCheckout(lfehler) Then
                '"drucke"
                'nachZielKopieren.dokumentdrucken.exe(myGlobalz.sitzung.aktDokument) 'funzt net mehr
                If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.PDF Then
                    nachZielKopieren.dokumentdrucken.printbatchPDF(myGlobalz.sitzung.aktDokument,
                                                            "C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe",
                                                    System.Text.Encoding.ASCII,
                                                    myGlobalz.sitzung.aktDokument.FullnameCheckout)
                End If
                If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.DOC Then
                    nachZielKopieren.dokumentdrucken.printbatchDOCX(myGlobalz.sitzung.aktDokument,
                                                            "WINWORD",
                                                    System.Text.Encoding.ASCII,
                                                    myGlobalz.sitzung.aktDokument.FullnameCheckout)
                End If
                If myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.MSG Or
                    myGlobalz.sitzung.aktDokument.DokTyp = DokumentenTyp.EML Then
                    MsgBox("MSG-Dateien können so nicht gedruckt werden. Bitte drucken Sie über Outlook (anklicken)!", MsgBoxStyle.OkOnly, "Kein Schnelldruck für MSG")
                End If
            Else
                MessageBox.Show("Fehler: Daten konnte nicht kopiert werden. " & Environment.NewLine &
              "Vermutliche Ursache: Datei war noch im Zugriff!" & Environment.NewLine &
              "Bitte schliessen sie die Datei !", "Datei ist noch im Zugriff?", MessageBoxButton.OK)
            End If
        End If

        If box.knopfnummer = 10 Then
            nachricht("USERAKTION: Einzelnes Dokument revisionssicher machen")
            If myGlobalz.sitzung.aktDokument.revisionssicher = True Then
                MsgBox("Dokument ist bereits revisionssicher!")
                Return False
            End If
            Dim messi As New MessageBoxResult
            messi = MessageBox.Show("Möchten Sie wirklich das Dokument revisionssicher machen ?" & vbCrLf,
                                    "Dokument revisionssichern ?",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                If myGlobalz.sitzung.aktDokument.revisionssicher = True Then
                    MsgBox("War bereits revisionssicher!")
                Else
                    myGlobalz.sitzung.aktDokument.revisionssicher = True
                    Dim result As Integer = DokArcTools.dokUpdate.execute(myGlobalz.sitzung.aktDokument.DocID,
                                                          myGlobalz.sitzung.aktDokument.istVeraltet,
                                                          myGlobalz.sitzung.aktDokument.Beschreibung,
                                                          myGlobalz.sitzung.aktDokument.revisionssicher,
                                                          myGlobalz.sitzung.aktDokument.Filedatum,
                                                          myGlobalz.sitzung.aktDokument.EXIFlat,
                                                          myGlobalz.sitzung.aktDokument.EXIFlon)
                    If result > 0 Then
                        ' MsgBox("Speichern erfolgreich. Formular schließen")
                        CLstart.myc.aLog.komponente = "Dokumente" : CLstart.myc.aLog.aktion = String.Format("{0} {1}: metadaten geaendert b", myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.DateinameMitExtension) : CLstart.myc.aLog.log()
                    Else
                        MsgBox("Speichern nicht erfolgreich. Formular schließen")
                    End If
                    'readonlymodus setzen
                    Dim datei As String = myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                    FileArchivTools.inputFileReadonlysetzen(datei)
                End If
            End If
        End If
        If box.knopfnummer = 11 Then
            Dim messi As New MessageBoxResult
            messi = MessageBox.Show(glob2.getMsgboxText("DokumentDemEreignisZuordnen",
                                                        New List(Of String)(New String() {CStr(myGlobalz.sitzung.aktEreignis.ID), myGlobalz.sitzung.aktEreignis.Beschreibung})),
                                    "Dokument wird diesem Ereignis zugeordnet!",
                                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                DokumentDemEreignisHinzufuegen(myGlobalz.sitzung.aktEreignis.ID, myGlobalz.sitzung.aktDokument)
                Return False
            End If
        End If
        If box.knopfnummer = 12 Then
            ereignisAusDokumentErzeugen(myGlobalz.sitzung.aktDokument)
        End If

        If box.knopfnummer = 13 Then
            nachricht("USERAKTION: Einzelnes Dokument im archiv als PDFA kopieren")
            If DokArc.machCheckout(lfehler) Then
                If nachZielKopieren.AlsPDFAkopieren.exe(myGlobalz.sitzung.aktDokument, False, "Stellungnahme_FD_Umwelt_") Then
                    MsgBox("PDF wurde unter 'Dokumente' abgelegt. Bitte auffrischen!")
                Else
                    MessageBox.Show("Kopieren nach PDF/a hat nicht geklappt. Datei schon vorhanden, geöffnet, schreibgeschützt, schon gelöscht ... ???", "Fail")
                End If
            End If
        End If
        If box.knopfnummer = 14 Then
            nachricht("USERAKTION: Einzelnes WordDokument nur zum lesen öffnen")
            box.dokumentWurdeGeoeffnet = False
            '6 readOnlyDoxsInTxtCrtlOeffnen = CBool(box.readOnlyDoxsInTxtCrtlOeffnenLOKAL)
            myGlobalz.sitzung.aktDokument.nurzumlesen = True
            If CBool(box.readOnlyDoxsInTxtCrtlOeffnenLOKAL) Then
                DokArc.readOnlyDoxsInTextCrtlOeffnen(myGlobalz.sitzung.aktDokument.FullnameImArchiv)
            Else
                If DokArc.machCheckout(lfehler) Then
                    DokArc.Archiv_aktiviere_dokument(myglobalz.sitzung.aktDokument, CBool(box.readOnlyDoxsInTxtCrtlOeffnenLOKAL), alleBilder, eid, Application.givenVIDList)
                Else
                    box.dokumentWurdeGeoeffnet = False
                    Return False
                End If
            End If
        End If

        If box.knopfnummer = 15 Then
            nachricht("USERAKTION: Einzelnes Dokument im archiv öffnen")
            box.dokumentWurdeGeoeffnet = True            ' Me.Close()
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            If checkout.ArchivDateinameGueltig(myGlobalz.sitzung.aktDokument) = "fehler" Then
                nachricht("fehler in ")
            End If
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            If Not myGlobalz.sitzung.aktDokument.FullnameCheckout.IsNothingOrEmpty Then
                box.dokumentWurdeGeoeffnet = False
                '    Dim textcontrol As String = "C:\Program Files\Text Control GmbH\TX Text Control 24.0.NET for WPF\Samples\Demo\x64\TXTextControlWords_WPF_Ribbon.exe "
                'glob2.OpenDocument(myGlobalz.sitzung.aktDokument.FullnameImArchiv)
                ' System.Diagnostics.Process.Start("TXTextControlWords_WPF_Ribbon.exe", myGlobalz.sitzung.aktDokument.FullnameImArchiv)
                'System.Diagnostics.Process.Start("WINWORD.exe", myGlobalz.sitzung.aktDokument.FullnameImArchiv)
                Dim param As String
                '1 = dateiname
                '2 = extension
                '3 = docid
                '4 = vid
                '5 = Titel
                param = myGlobalz.sitzung.aktDokument.FullnameImArchiv & " " & box.WordDateityp & " " &
                        myGlobalz.sitzung.aktDokument.DocID & " " &
                        myGlobalz.sitzung.aktVorgangsID & " """ &
                        myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt & ""
                ' System.Diagnostics.Process.Start("C:\ptest\txtctrl\TXTextControlWords_WPF_Ribbon.exe", param)
                l("TXparam: " & param)
                Using p As New Process()
                    p.StartInfo.FileName = initP.getValue("ExterneAnwendungen.APPLICATION_TextControl") ' "C:\ptest\txtctrl\ParadigmaWordEditor.exe"
                    p.StartInfo.Arguments = param
                    p.StartInfo.UseShellExecute = False
                    p.Start()
                    l("TXfilename: " & initP.getValue("ExterneAnwendungen.APPLICATION_TextControl"))
                End Using
            End If

        End If
        If box.knopfnummer = 16 Then
            Dim modus = "sicherungen anzeigen"
            nachricht("USERAKTION: sicherungen anzeigen")
            Debug.Print(myGlobalz.sitzung.aktDokument.FullnameCheckout)
            Dim backupdir As New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
            IO.Directory.CreateDirectory(backupdir.DirectoryName)
            System.Diagnostics.Process.Start(backupdir.DirectoryName)
            box.dokumentWurdeGeoeffnet = False
            backupdir = Nothing
            Return False
        End If
        If box.knopfnummer = 17 Then
            nachricht("USERAKTION: conject")
            box.dokumentWurdeGeoeffnet = False
            If DokArc.machCheckout(lfehler) Then
                nachricht("machCheckout erfolgreich")
                Dim conjectform As New winToConject
                conjectform.ShowDialog()

                Dim conjectName As String
                Dim typ As String = ""


                'If box.cmbConjectDokumentTyp.SelectedIndex = 0 Then
                '    typ = "Stellungnahme_FD_Umwelt_"
                'End If
                'If box.cmbConjectDokumentTyp.SelectedIndex = 1 Then
                '    typ = "Nachforderung _FD_Umwelt_"
                'End If
                'conjectName = typ & box.cmbConjectDokumentAbteilung.Text & "_vom_" & myGlobalz.sitzung.aktDokument.getTimestamp
                'If nachZielKopieren.AlsPDFAkopieren.exe(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgang.istConjectVorgang, conjectName) Then
                '    If myGlobalz.sitzung.aktVorgang.istConjectVorgang Then
                '        MessageBox.Show("Stellungnahme-PDF wurde unter 'Dokumente' abgelegt. Bitte auffrischen!" & Environment.NewLine &
                '                      "                  Verzeichnis wird geöffnet, " & Environment.NewLine &
                '                      "                  Verzeichnisname wurde in Zwischenablage kopiert !", "Übernahme nach Conject vorbereitet")
                '    Else
                '        MsgBox("PDF wurde unter 'Dokumente' abgelegt. Bitte auffrischen!")
                '    End If
                'End If


                'FileArchivTools.inputFileReadonlyEntfernen(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                'myGlobalz.sitzung.aktDokument.nurzumlesen = False
                'l("nurzumlesen1:  " & myGlobalz.sitzung.aktDokument.nurzumlesen & myGlobalz.sitzung.aktDokument.FullnameCheckout)
                'Dim fi As New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                'System.Diagnostics.Process.Start(fi.DirectoryName)
                'Clipboard.Clear()
                'Clipboard.SetText(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                'fi = Nothing
            Else
                MessageBox.Show("Vermutlich haben Sie die Datei noch geöffnet. Damit blockieren sie den erneuten Checkout! ", "Fehler beim Checkout")
                nachricht("fehler beim checkout: " & lfehler)
            End If
            Return False
        End If
        Return box.dokumentWurdeGeoeffnet
    End Function
    Public Shared Sub RaumbezugExcel_clickExtracted()
        Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Raumbezuege", myGlobalz.sitzung.raumbezugsRec.dt, myGlobalz.sitzung.aktVorgangsID, "", CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub
    Public Shared Function ohneSemikolon(ByRef p1 As String) As String
        Try
            If String.IsNullOrEmpty(p1) Then
                Return ""
            End If
            Dim temp$ = p1
            temp = temp.Trim
            temp = temp.Replace(";", "_")
            temp = temp.Replace(vbCrLf, "")
            Return temp
        Catch ex As Exception
            nachricht("Fehler in ohneSemikolon: ", ex)
            Return ""
        End Try
    End Function
    Shared Function erstelleCSVausgabeDerFlurstuecke(ByVal vid As Integer) As Boolean
        'Dim handcsv As New clsCSVausgaben("FlurstueckeFuerEigentuemer", myglobalz.sitzung.raumbezugsRec.dt, myglobalz.sitzung.aktVorgangsID, "",
        '                                  CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        'nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
        'handcsv.Dispose()
        'Public Sub FlurstueckeFuerEigentuemer(ByVal vid As Integer, myGlobalz_sitzung_raumbezugsRec As DataTable)
        Dim cnt As Integer
        Try
            Dim exportFileFuerEigentuemer As String
            exportFileFuerEigentuemer = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
                                         "Paradigma\csv\FlurstueckeFuerEigentuemer.csv")
            Dim delim = ";"
            Dim swe As IO.StreamWriter = New IO.StreamWriter(exportFileFuerEigentuemer, False, CLstart.mycSimple.enc)
            myGlobalz.sitzung.raumbezugsRec.mydb.SQL = "select * from " & CLstart.myViewsNTabs.TABPARAFLURSTUECK & "  " &
                "where id in (select sekid from pf_sekid2vid where vorgangsid=" & vid & ")" &
                 " order by gemcode,flur,zaehler,nenner"
            Dim hinweis As String = myGlobalz.sitzung.raumbezugsRec.getDataDT()
            swe.WriteLine(
                        "gemcode" & delim &
                        "flur" & delim &
                        "zaehler" & delim &
                        "nenner" & delim &
                        "fs" & delim
           )
            'TAB = myGlobalz_sitzung_raumbezugsRec
            For i = 0 To myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count - 1
                swe.WriteLine(
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("gemcode").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("flur").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("zaehler").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("nenner").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("fs").ToString) & delim)
                cnt += 1
            Next
            swe.Dispose()
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabee" & vbCrLf, ex)
        End Try

        Return True
    End Function

    Public Shared Function dokumentschonImVorgangvorhanden(ByVal dokument As Dokument, ByVal ZielvorgangsidInput As Integer) As Boolean

        myGlobalz.sitzung.tempREC.mydb.SQL =
                  "SELECT * from " & CLstart.myViewsNTabs.tabDokumente & " where vid=" & ZielvorgangsidInput &
                  " and dateinameext='" & dokument.DateinameMitExtension & "'" &
                  " and typ='" & dokument.Typ & "'" &
                  " and beschreibung='" & dokument.Beschreibung & "'"
        nachricht("dokumentschonImVorgangvorhanden sql: " & myGlobalz.sitzung.tempREC.mydb.SQL)
        Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
        If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            nachricht("dokumentschonImVorgangvorhanden ist schon in vorgang vorhanden")
            Return True
        Else
            nachricht("dokumentschonImVorgangvorhanden ist noch nicht in vorgang vorhanden")
            Return False
        End If
    End Function



    Private Shared Sub HinweisFenster_Vorgang_im_Zugriff(ByVal lockuser As String)
        'If lockuser = "feij" Then Exit Sub
        'MessageBox.Show(glob2.getMsgboxText("vorgangBereitsOffen", New List(Of String)(New String() {lockuser})),
        '                "Dieser Vorgang ist bereits von einer anderen Person geöffnet!", MessageBoxButton.OK, MessageBoxImage.Information,
        '                 MessageBoxResult.OK)
    End Sub



    Shared Function getRBheadertext(ByVal fixtext As String, ByVal dt As DataTable) As String
        If dt.IsNothingOrEmpty() Then
            Return fixtext
        Else
            Return fixtext & dt.Rows.Count
        End If
    End Function



    Shared Function letztesEreignisWurdeGeaendert() As Boolean
        'letztes ereignis filtern
        'letzes erignis bilden
        'geändert?
        'ja : text und datum übernehmen
        'nein: false
        nachricht("letztesEreignisWurdeGeaendert ---------------------------")
        Dim neutext As String = "", hinweis As String = ""
        Dim neudatum, altdatum As Date
        Try
            myGlobalz.sitzung.EreignisseRec.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabEreignis &
                "  where datum=(select max(datum)   from " & CLstart.myViewsNTabs.tabEreignis & " where vorgangsid=" &
                 myGlobalz.sitzung.aktVorgangsID & ")"
            hinweis = myGlobalz.sitzung.EreignisseRec.getDataDT()
            nachricht(hinweis)
            If myGlobalz.sitzung.EreignisseRec.dt.Rows.Count < 1 Then
                nachricht("letztesEreignisWurdeGeaendert : keine ereignisse vorhanden")
                Return False
            Else
                'das letzte ereignis wurde gefunden
                altdatum = myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung
                neudatum = clsDBtools.fieldvalueDate(myGlobalz.sitzung.EreignisseRec.dt.Rows(0).Item("DATUM"))
                If altdatum <> neudatum Then
                    myGlobalz.sitzung.aktEreignis.Art = clsDBtools.fieldvalue(myGlobalz.sitzung.EreignisseRec.dt.Rows(0).Item("ART"))
                    myGlobalz.sitzung.aktEreignis.Beschreibung = clsDBtools.fieldvalue(myGlobalz.sitzung.EreignisseRec.dt.Rows(0).Item("BESCHREIBUNG"))
                    myGlobalz.sitzung.aktEreignis.Datum = clsDBtools.fieldvalueDate(myGlobalz.sitzung.EreignisseRec.dt.Rows(0).Item("DATUM"))
                    clsEreignisTools.setLetztesEreignisText(myGlobalz.sitzung.aktEreignis)
                    myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = myGlobalz.sitzung.aktEreignis.Datum
                    Return True
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            nachricht("Fehler in letztesEreignisWurdeGeaendert :", ex)
            Return False
        End Try
    End Function

    Shared Function stellungnahmeWurdeGeaendert() As Boolean
        Dim neutext As String = "", hinweis As String = ""
        myGlobalz.sitzung.EreignisseRec.mydb.SQL = "select * from " & CLstart.myViewsNTabs.TABSTAMMDATEN & " where " &
                     " vorgangsid in " &
                     " (select s.vorgangsid  from " & CLstart.myViewsNTabs.tabEreignis & "  e," & CLstart.myViewsNTabs.TABSTAMMDATEN & " s " &
                     " where art like '%tellungn%' " &
                     " and e.vorgangsid=s.vorgangsid " &
                     " and s.stellungnahme=0 " &
                      " and  s.vorgangsid=" &
                myGlobalz.sitzung.aktVorgangsID & ")"
        hinweis = myGlobalz.sitzung.EreignisseRec.getDataDT()
        nachricht(hinweis)
        If myGlobalz.sitzung.EreignisseRec.dt.Rows.Count < 1 Then
            nachricht("alles ok ")
            Return False
        Else
            'stellungnahme auf 1 setzen
            myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = True
            Return True
        End If
    End Function

    Shared Function istDateiNameInordnung(dateiname As String) As Boolean
        If String.IsNullOrEmpty(dateiname) Then Return False
        If dateiname.Contains("?") Then Return False
        Return True
    End Function

    'Public Shared Sub ErzeugeUnterVerzeichnisse(ByVal AusgabeRootDir As String)
    '    IO.Directory.CreateDirectory(AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username)
    '    IO.Directory.CreateDirectory(AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username & "\data")
    '    IO.Directory.CreateDirectory(AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username & "\images")
    'End Sub
    'Public Shared Sub MapfileTemplateBearbeiten(ByVal AusgabeRootDir As String, userlayeraidNKATDIR As String)
    '    Dim KartenMapfileTemplate, kartenmapfile As String
    '    kartenmapfile = AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username & "\raumbezug.map"
    '    KartenMapfileTemplate = AusgabeRootDir & "vorlagen\raumbezug.map"
    '    mapgeneratortools.makeMapFilePostgis(KartenMapfileTemplate, kartenmapfile,
    '                                        myGlobalz.sitzung.aktBearbeiter.username,
    '                                        True,
    '                                        CLstart.mycSimple.enc,
    '                                        tableName:=myGlobalz.sitzung.aktBearbeiter.username, userlayeraidNKATDIR:=userlayeraidNKATDIR)
    'End Sub
    'Public Shared Function GetPdfZielFilename() As String
    '    Dim pdfZielFilename As String
    '    pdfZielFilename = CLstart.myc.kartengen.gifKartenDateiFullName '.Replace("Paradigma", "")
    '    pdfZielFilename = pdfZielFilename.Replace("png", "pdf")
    '    pdfZielFilename = pdfZielFilename.Replace("_.pdf", ".pdf")
    '    Return pdfZielFilename
    'End Function
    Shared Function getAnzahlDoksproEreignis(vorgangsid As Integer, irec As IDB_grundfunktionen) As DataTable
        Try
            Dim dt As DataTable : Dim hinweis As String = ""
            irec.mydb.SQL = "select * from " & CLstart.myViewsNTabs.view_anzahldoksproereignis2 & "  as anz where vorgangsid=" & vorgangsid 'myGlobalz.sitzung.VorgangsID
            dt = getDT4Query(irec.mydb.SQL, irec, hinweis)
            'myGlobalz.sitzung.tempREC.getDataDT()
            If dt.Rows.Count < 1 Then
                Return Nothing
            Else
                Return dt
            End If
        Catch ex As Exception
            nachricht("fehler in getAnzahlDoksproEreignis: ", ex)
            Return Nothing
        End Try
    End Function
    Public Shared Async Function initEreigisseDatatable(ByVal hinweis As String, ByVal vid As Integer,
                                                  ereignisdocexpand As Boolean,
                                                  ereignisDokListe As List(Of clsEreignisDok)) As System.Threading.Tasks.Task(Of Integer)
        Dim erfolg As Boolean
        myGlobalz.sitzung.EreignisseRec.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabEreignis & "  e where VorgangsID=" & vid & " order by datum desc"
        hinweis = myGlobalz.sitzung.EreignisseRec.getDataDT()

        If myGlobalz.sitzung.EreignisseRec.mycount < 1 Then
            nachricht("Keine Ereignisse gespeichert c!")
            Return 0
        End If
        Dim anzahlDoksDT As New DataTable
        Try
            anzahlDoksDT = detailsTools.getAnzahlDoksproEreignis(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.VorgangREC)
            If anzahlDoksDT Is Nothing OrElse anzahlDoksDT.IsNothingOrEmpty Then
            Else
                RBtoolsns.statusSpalteErgaenzenUndFuellen.execute(myGlobalz.sitzung.EreignisseRec.dt, anzahlDoksDT,
                                                                  "ANZAHL",
                                                                  "ID")
            End If
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))

            If ereignisdocexpand Then
                myGlobalz.sitzung.tempREC2.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.view_VORG2DOKEREIGNIS2 & " as viii where VorgangsID=" & vid &
                                                      " and dateinameext is not null"
                myGlobalz.sitzung.tempREC2.dt = getDT4Query(myGlobalz.sitzung.tempREC2.mydb.SQL, myGlobalz.sitzung.tempREC2, hinweis)
                'hinweis = myglobalz.sitzung.tempREC2.getDataDT()
            End If
            erfolg = AlleEreignisseAufListeSetzen(myGlobalz.sitzung.EreignisseRec.dt,
                                                ereignisDokListe,
                                                myGlobalz.sitzung.tempREC2.dt,
                                                ereignisdocexpand)
            Return CInt(myGlobalz.sitzung.EreignisseRec.mycount)
        Catch ex As Exception
            nachricht("Fehler in initEreigisseDatatable: ", ex)
            Return 0
        End Try
    End Function

    Public Shared Sub startURL(ByVal url As String)
        If url.Trim.Length > 0 Then
            url = url.ToLower
            If Not url.StartsWith("http") Then
                url = "http://" & url
            End If
            Process.Start(url)
        End If
    End Sub

    Private Shared Function GetAnzahlDoksZumEreignis(ByVal ereig As DataRow) As Integer
        Dim anz As Integer
        Try
            If ereig.Table.Columns.Contains("ANZAHL") = True Then
                anz = CInt(clsDBtools.fieldvalue(ereig.Item("ANZAHL")))
                Return anz
            Else
                Return 0
            End If
        Catch ex As Exception
            nachricht("Warnung in GetAnzahlDoksZumEreignis: ", ex)
            Return 0
        End Try
    End Function

    Private Shared Sub AlleEreignisseAufListeSetzenExtracted(ByVal ereignisDokListe As List(Of clsEreignisDok),
                                                             ByVal dokumenteDatatable As DataTable,
                                                             ByRef aktereig As clsEreignisDok)
        Try
            For Each dok As DataRow In dokumenteDatatable.Rows
#If DEBUG Then

#End If
                If CInt(clsDBtools.fieldvalue(dok.Item("ID"))) <> aktereig.ID Then
                    'gehört nicht zum ereignis
                    Continue For
                End If
                aktereig = New clsEreignisDok

                aktereig.Notiz = ""
                aktereig.Beschreibung = clsDBtools.fieldvalue(dok.Item("DATEINAMEEXT"))
                aktereig.ID = CInt(clsDBtools.fieldvalue(dok.Item("ID")))
                aktereig.DokumentID = CInt(clsDBtools.fieldvalue(dok.Item("DOKUMENTID")))
                aktereig.Quelle = clsDBtools.fieldvalue(dok.Item("QUELLE"))
                aktereig.typnr = CInt(clsDBtools.fieldvalue(dok.Item("TYPNR")))
                Try
                    aktereig.Datum = CDate(clsDBtools.fieldvalueDate(dok.Item("DATUM")))
                    aktereig.dok.Filedatum = CDate(clsDBtools.fieldvalueDate(dok.Item("DATUM")))
                    aktereig.Datum = clsDBtools.fieldvalueDate(dok.Item("FILEDATUM"))
                Catch ex As Exception
                    Debug.Print("ss")
                End Try
                Try
                    aktereig.revisionssicher = CInt(clsDBtools.fieldvalue(dok.Item("REVISIONSSICHER")))
                    aktereig.dok.sizeMb = CInt(clsDBtools.fieldvalue(dok.Item("MB")))
                    aktereig.dok.kompressed = CBool((clsDBtools.fieldvalue(dok.Item("KOMPRESS"))))
                Catch ex As Exception

                End Try

                ' aktereig.Richtung = clsDBtools.fieldvalue(ereig.Item("RICHTUNG"))
                ' aktereig.Richtung = " " 'wird zur farbsteuerung verwendet
                ' aktereig.Art = clsDBtools.fieldvalue(dok.Item("TYP"))

                'dokumentanteil
                aktereig.dok.dokumentPfad = (clsDBtools.fieldvalue(dok.Item("RELATIVPFAD")))
                aktereig.dok.DateinameMitExtension = (clsDBtools.fieldvalue(dok.Item("DATEINAMEEXT")))
                aktereig.dok.Beschreibung = (clsDBtools.fieldvalue(dok.Item("BESCHREIBUNG"))).Trim
                aktereig.dokBeschreibung = (clsDBtools.fieldvalue(dok.Item("D_BESCHREIBUNG"))).Trim
                aktereig.dok.DocID = CInt((clsDBtools.fieldvalue(dok.Item("DOKUMENTID"))))
                aktereig.dok.EreignisID = CInt((clsDBtools.fieldvalue(dok.Item("EID"))))
                aktereig.dok.VorgangsID = CInt((clsDBtools.fieldvalue(dok.Item("VID"))))
                aktereig.dok.Initiale = ((clsDBtools.fieldvalue(dok.Item("INITIAL_"))))

                aktereig.dok.newSaveMode = CBool((clsDBtools.fieldvalue(dok.Item("NEWSAVEMODE"))))
                aktereig.dok.getDokTyp()
                aktereig.thumbnailFullPath = getThumbnailfullpath(initP.getValue("Haupt.ThumbNailsRoot"), myglobalz.sitzung.aktVorgangsID, aktereig.DokumentID, aktereig.dok.DokTyp)
                aktereig.thumbnailFullPath = clsTools.tools.korrigiereThumbnail(aktereig.thumbnailFullPath)

                aktereig.thumbnailMSGtext = ""
                If aktereig.dok.DokTyp = DokumentenTyp.MSG Or aktereig.dok.DokTyp = DokumentenTyp.EML Then
                    aktereig.thumbnailMSGtext = detailsTools.getMSGNotizText(aktereig.ID, ereignisDokListe)
                End If
                If aktereig.dok.DokTyp = DokumentenTyp.TXT Then
                    aktereig.thumbnailMSGtext = detailsTools.getMSGNotizText(aktereig.ID, ereignisDokListe)
                End If
                'If aktereig.dok.DokTyp = DokumentenTyp.JPG Or aktereig.dok.DokTyp = DokumentenTyp.PNG Or aktereig.dok.DokTyp = DokumentenTyp.GIF Then
                If DokArc.istFoto(aktereig.dok.DateinameMitExtension) Then
                    aktereig.thumbnailFullPath = aktereig.dok.makeFullname_ImArchiv(myglobalz.Arc.rootDir)
                End If
                If aktereig.dok.DokTyp = DokumentenTyp.TIF Then
                    aktereig.thumbnailFullPath = aktereig.dok.makeFullname_ImArchiv(myglobalz.Arc.rootDir)
                End If
                aktereig.EreignisDokTyp = 1

                If aktereig.dok.VorgangsID < 1 Then
                    Debug.Print("ss")
                End If

                ereignisDokListe.Add(aktereig)
            Next

        Catch ex As Exception
            nachricht("Fehler in AlleEreignisseAufListeSetzenExtracted: ", ex)
        End Try
    End Sub


    Private Shared Function getThumbnailfullpath(thumbnailRootDir As String, aktVorgangsID As Integer, dokumentID As Integer, doktyp As DokumentenTyp) As String
        Dim thumbnailFullPath As String = ""

        Try
            l(" getThumbnailfullpath ---------------------- anfang")

            If doktyp = DokumentenTyp.PDF Or
                        doktyp = DokumentenTyp.RTF Or
                        doktyp = DokumentenTyp.DOC Then
                thumbnailFullPath = thumbnailRootDir & aktVorgangsID & "\" & dokumentID & ".jpg"
                thumbnailFullPath = clsTools.tools.korrigiereThumbnail(thumbnailFullPath)

            Else
                If doktyp = DokumentenTyp.MSG Or doktyp = DokumentenTyp.EML Then
                    'thumbnailMSGtext = detailsTools.getMSGNotizText(EreignisID, ereignisDokListe)
                Else
                    thumbnailFullPath = ""
                End If
            End If
            l(" getThumbnailfullpath ---------------------- ende")
            Return thumbnailFullPath
        Catch ex As Exception
            l("Fehler in getThumbnailfullpath: ", ex)
            Return ""
        End Try
    End Function

    Private Shared Function AlleEreignisseAufListeSetzen(ereignisDataTable As DataTable,
                             ereignisDokListe As List(Of clsEreignisDok),
                             dokumenteDatatable As DataTable, expandEreignisDok As Boolean) As Boolean
        Dim aktereig As clsEreignisDok
        Dim anzahl As Integer
        Try
            If ereignisDataTable Is Nothing Then
                nachricht("Fehler in AlleEreignisseAufListeSetzen: ereignisDataTable is nothing")
                Return False
            End If
            If ereignisDokListe Is Nothing Then
                nachricht("Fehler in AlleEreignisseAufListeSetzen: ereignisDokListe is nothing")
                Return False
            End If
            For Each ereig As DataRow In ereignisDataTable.Rows 'myGlobalz.sitzung.EreignisseRec.dt.Rows
                aktereig = New clsEreignisDok
                getEreinigsAnteil(aktereig, ereig)
                anzahl = GetAnzahlDoksZumEreignis(ereig)
                aktereig.EreignisDokTyp = If(expandEreignisDok, 0, 3)
                ereignisDokListe.Add(aktereig)
                If expandEreignisDok And anzahl > 0 Then
                    AlleEreignisseAufListeSetzenExtracted(ereignisDokListe, dokumenteDatatable, aktereig)
                Else
                    aktereig.thumbnailMSGtext = aktereig.Notiz
                End If
            Next
            Return True
        Catch ex As Exception
            nachricht("Fehler in AlleEreignisseAufListeSetzen: ", ex)
            Return False
        End Try
    End Function
    Private Shared Sub getEreinigsAnteil(ByVal aktereig As clsEreignisDok, ByVal ereig As DataRow)
        Try
            aktereig.Notiz = clsDBtools.fieldvalue(ereig.Item("NOTIZ")).Trim
            aktereig.Beschreibung = clsDBtools.fieldvalue(ereig.Item("BESCHREIBUNG"))
            aktereig.ID = CInt(clsDBtools.fieldvalue(ereig.Item("ID")))
            aktereig.DokumentID = CInt(clsDBtools.fieldvalue(ereig.Item("DOKUMENTID")))
            aktereig.istRTF = CBool(clsDBtools.toBool(ereig.Item("ISTRTF")))
            aktereig.Quelle = clsDBtools.fieldvalue(ereig.Item("QUELLE")).Trim
            aktereig.Richtung = clsDBtools.fieldvalue(ereig.Item("RICHTUNG")).Trim
            aktereig.Art = clsDBtools.fieldvalue(ereig.Item("ART")).Trim
            aktereig.Datum = clsDBtools.fieldvalueDate(ereig.Item("DATUM"))
            aktereig.dokBeschreibung = clsDBtools.fieldvalue(ereig.Item("NOTIZ")).Trim
            aktereig.typnr = CInt(clsDBtools.fieldvalue(ereig.Item("TYPNR")).Trim)

        Catch ex As Exception
            nachricht("Fehler in getEreinigsAnteil: ", ex)
        End Try
    End Sub

    Public Shared Sub Dokument2Obj(item As clsEreignisDok, dokument As Dokument)
        Try
            dokument.DocID = CInt(item.dok.DocID)
            dokument.dokumentPfad = item.dok.dokumentPfad
            dokument.DateinameMitExtension = item.dok.DateinameMitExtension
            dokument.Typ = item.dok.Typ
            dokument.Beschreibung = item.dok.Beschreibung
            dokument.Filedatum = CDate(item.dok.Filedatum)
            dokument.Checkindatum = CDate(item.dok.Checkindatum)
            dokument.istVeraltet = CBool(clsDBtools.toBool(item.dok.istVeraltet))
            dokument.ExifDatum = CDate(item.dok.ExifDatum)
            dokument.EXIFlon = CStr(item.dok.EXIFlon)
            dokument.EXIFlat = CStr(item.dok.EXIFlat)
            dokument.EXIFdir = CStr(item.dok.EXIFdir)
            dokument.EXIFhersteller = CStr(item.dok.EXIFhersteller)
            If CBool(clsDBtools.toBool(item.revisionssicher) Or CBool(clsDBtools.toBool(item.dok.revisionssicher))) Then
                dokument.revisionssicher = True
            Else
                dokument.revisionssicher = False
            End If
            dokument.sizeMb = CDbl(clsDBtools.fieldvalue(item.dok.sizeMb))
            dokument.kompressed = CBool(clsDBtools.toBool(item.dok.kompressed))

            dokument.Initiale = CStr(item.dok.Initiale)
            dokument.EreignisID = (item.dok.EreignisID)
            dokument.VorgangsID = CInt(item.dok.VorgangsID)
            dokument.newSaveMode = CBool(clsDBtools.toBool(item.dok.newSaveMode))
            Try
                dokument.istNurVerwandt = True 'CBool(clsDBtools.toBool(item.dok.status)
            Catch ex As Exception
            End Try
        Catch ex As Exception
            nachricht("DokumentDatarowView2Obj " & vbCrLf & vbCrLf, ex)
        End Try
    End Sub
    Public Shared Function istIrgendeinDokumentGeoeffnet(doktyp As DokumentenTyp) As Boolean
        Try
            If doktyp = DokumentenTyp.DOC Then
                If myGlobalz.sitzung.wordDateiImEditModus Is Nothing Then
                    Return False
                End If
                If myGlobalz.sitzung.wordDateiImEditModus.DateinameMitExtension.IsNothingOrEmpty Then
                    Return False
                Else
                    Return True
                End If
            End If
            If doktyp = DokumentenTyp.XLS Then
                If myGlobalz.sitzung.excelDateiImEditModus Is Nothing Then
                    Return False
                End If
                If myGlobalz.sitzung.excelDateiImEditModus.DateinameMitExtension.IsNothingOrEmpty Then
                    Return False
                Else
                    Return True
                End If
            End If
            Return False
        Catch ex As Exception
            nachricht("fehler in istIrgendeinDokumentGeoeffnet " & vbCrLf & vbCrLf, ex)
            Return False
        End Try
    End Function
    Public Shared Function dokumentIstGeoeffnet(dok As Dokument) As Boolean
        Try
            If dok Is Nothing Then
                Return False
            End If
            If dok.nurzumlesen Then
                Return False
            End If
            If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.DOC Then
                If myGlobalz.sitzung.wordDateiImEditModus Is Nothing Then
                    Return False
                End If
                If dok.DateinameMitExtension = myGlobalz.sitzung.wordDateiImEditModus.DateinameMitExtension Then
                    Return True
                End If
            End If
            If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.XLS Then
                If myGlobalz.sitzung.excelDateiImEditModus Is Nothing Then
                    Return False
                End If
                If dok.DateinameMitExtension = myGlobalz.sitzung.excelDateiImEditModus.DateinameMitExtension Then
                    Return True
                End If
            End If
            Return False

        Catch ex As Exception
            nachricht("dokumentIstGeoeffnet " & vbCrLf & vbCrLf, ex)
            Return False
        End Try
    End Function

    Shared Sub zumSGformular(item As String, vorgangsid As Integer)
        Select Case item.ToLower
            Case "natureg"
                nachricht("USERAKTION: zu natureg ")
                Dim ggg As New VorgangUebersicht
                ggg.ShowDialog()
        End Select
    End Sub

    Shared Function getZWert(newpoint As myPoint) As String
        Dim aufruf As String = modZwert.bildeaufruf(newpoint, myGlobalz.sitzung.aktBearbeiter.username)
        'Dim a=   modZwert.getzfromproxy(myGlobalz.ProxyString, aufruf)
        Dim result As String = CLstart.meineHttpNet.sendjobExtracted(aufruf, CLstart.mycSimple.enc, 18000000)
        'Dim a() As String
        'a=result.Split("#"c)
        Return result
    End Function

    Private Shared Function getUserInput(p1 As String) As Boolean
        Dim messi As New MessageBoxResult
        messi = MessageBox.Show(p1 & vbCrLf,
                          p1,
                          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
        If messi = MessageBoxResult.Yes Then
            Return True
        Else
            Return False
        End If
    End Function

    Shared Sub vorgangsprotokollanzeigen()
        Try
            System.Diagnostics.Process.Start(CLstart.myc.aLog._logfile)
        Catch ex As Exception
            nachricht("fehler in vorgangsprotokollanzeigen: ", ex)
        End Try
    End Sub

    Shared Function settitle(modultitel As String) As String
        Return myGlobalz.sitzung.aktVorgangsID & ", " & modultitel & ", Akt.: " & myGlobalz.sitzung.aktBearbeiter.Name &
       ", (Hauptbearbeiter/in= " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.username & ") " & " / Vers. " &
       CLstart.mycSimple.ParadigmaVersion
        '& ", DBHOST: " & myGlobalz.sitzung.VorgangREC.mydb.Host
    End Function

    'Private Shared Function getUeberschriftAktennotiz() As String
    '    Dim ueb As String = Microsoft.VisualBasic.Interaction.InputBox(glob2.getMsgboxText("AktennotizUeberschrift", New List(Of String)(New String() {})),
    '                                                                   "Bitte eine Überschrift eingeben", "")
    '    If ueb.Trim.Length > 3 Then
    '        Return ueb
    '    Else
    '        Return ""
    '    End If
    'End Function

    Shared Sub hatraumbezugDarstellen()
        Try
            l("hatraumbezugDarstellen---------------------- anfang")
            If myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True And
               myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count < 1 Then
                myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = False
                detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
                l("HATRAUMBEZUG korrigiert: 1")
            End If
            If myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = False And
                 myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count > 0 Then
                myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
                detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
                l("HATRAUMBEZUG korrigiert: 2")
            End If
            If myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kuerzel2Stellig = "fe" Or
                myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kuerzel2Stellig = "th" Then
                myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
                detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
                l("HATRAUMBEZUG korrigiert: fe th")
            End If
            l("hatraumbezugDarstellen---------------------- ende")
        Catch ex As Exception
            l("Fehler in hatraumbezugDarstellen: ", ex)
        End Try
    End Sub



    Friend Shared Sub DokumentDemEreignisHinzufuegen(eid As Integer, dokument As Dokument)
        Dim dockument As New clsPresDokumente
        Dim hinweis As String = ""
        Try
            l("---------------------- anfang")
            '  dockument = CType(dokument, clsPresDokumente)
            l("DokumentDemEreignisHinzufuegen----------------")
            dockument = dockument.dokument2Presdokument(dokument)
            If Not (dockument.istNurVerwandt) Then
                If detail_dokuauswahl.dokumentHatSchonEreigniskopplung(dockument) Then
                    hinweis = hinweis & "-> Dokument ist schon an ein anderes Ereignis gekoppelt: " & dockument.DateinameMitExtension & Environment.NewLine
                Else
                    If detail_dokuauswahl.ausgewDokusDemAktEreignisHinzufuegenExtracted(eid, dockument) > 0 Then
                        hinweis = hinweis & "hinzugefügt: " & dockument.DateinameMitExtension & Environment.NewLine
                    Else
                        hinweis = hinweis & "Nichts hinzugefügt: " & dockument.DateinameMitExtension & Environment.NewLine
                    End If
                End If
            Else
                hinweis = hinweis & "-> Dokument ist verwandt und kann daher nicht angekoppelt werden: " & dockument.DateinameMitExtension & Environment.NewLine
            End If
            MsgBox(hinweis)
            l("DokumentDemEreignisHinzufuegen---------------------- ende")
        Catch ex As Exception
            l("Fehler in DokumentDemEreignisHinzufuegen: ", ex)
        End Try
    End Sub

    Private Shared Sub ereignisAusDokumentErzeugen(dokument As Dokument)
        Dim neuereignis As New clsEreignis
        FST_tools.initEreignis(neuereignis, "aus dokument erzeugtes Ereignis")
        clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)
        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
            myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()

        DokumentDemEreignisHinzufuegen(neuereignis.ID, myGlobalz.sitzung.aktDokument)
    End Sub

    Shared Function IstWinwordNochGeoeffnet() As Boolean
        Try
            Return Process.GetProcesses().Any(Function(p) p.ProcessName.Contains("WINWORD"))
        Catch ex As Exception
            Return True
        End Try
    End Function

    'Friend Shared Function SollAlsStandardSetzen(weitereBearbeiter As String) As Boolean
    '    Dim immereintragen As Boolean = getUserInput("Sollen diese Bearbeiter (" & weitereBearbeiter & ") " & Environment.NewLine &
    '                                                 " IMMER als 'Weitere Bearbeiter' in neue Vorgänge eingetragen werden ?" & Environment.NewLine &
    '                                                 " - " & Environment.NewLine)
    '    Return immereintragen
    'End Function

    Friend Shared Sub WeitereBearbeiterImCookieSpeichern(weitereBearbeiter As String)
        CLstart.myc.userIniProfile.WertSchreiben("Stammdaten", "weiterebearbeiter", weitereBearbeiter)
    End Sub

    Public Shared Sub thumbNailsFotosHinzuFuegen(alle As List(Of clsPresDokumente), ereignisDokListe As List(Of clsEreignisDok))
        Try
            For Each presdok In alle
                If DokArc.istFoto(presdok.DateinameMitExtension) Then
                    presdok.thumbnailFullPath = presdok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir) ' arcRootDir & presdok.VorgangsID & "\" & presdok.DocID & ".jpg" 
                Else
                    If presdok.DokTyp = DokumentenTyp.MSG Or presdok.DokTyp = DokumentenTyp.EML Then
                        presdok.thumbnailMSGtext = detailsTools.getMSGNotizText(presdok.EreignisID, ereignisDokListe)
                    Else
                        presdok.thumbnailFullPath = ""
                    End If
                End If
            Next
        Catch ex As Exception
            nachricht("fehler in thumbNailsHinzuFuegen b: ", ex)
        Finally
        End Try
    End Sub
    Public Shared Sub thumbNailsHinzuFuegen(alle As List(Of clsPresDokumente),
                                            thumbnailRootDir As String,
                                            ereignisDokListe As List(Of clsEreignisDok))

        Try
            For Each presdok In alle
                If presdok.DokTyp = DokumentenTyp.PDF Or
                        presdok.DokTyp = DokumentenTyp.RTF Or
                        presdok.DokTyp = DokumentenTyp.DOC Then
                    presdok.thumbnailFullPath = thumbnailRootDir & presdok.VorgangsID & "\" & presdok.DocID & ".jpg"
                    presdok.thumbnailFullPath = clsTools.tools.korrigiereThumbnail(presdok.thumbnailFullPath)
                    Continue For
                End If
                If presdok.DokTyp = DokumentenTyp.TIF Then
                    presdok.thumbnailFullPath = presdok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir) ' arcRootDir & presdok.VorgangsID & "\" & presdok.Do
                    Continue For
                End If

                If presdok.DokTyp = DokumentenTyp.MSG Or presdok.DokTyp = DokumentenTyp.EML Then
                    presdok.thumbnailMSGtext = detailsTools.getMSGNotizText(presdok.EreignisID, ereignisDokListe)
                    Continue For
                End If
            Next
        Catch ex As Exception
            nachricht("fehler in thumbNailsHinzuFuegen c: ", ex)
        Finally
        End Try
    End Sub
    Public Shared Function BackupAnlegen(quelldatei As FileInfo, ByVal dokid As Integer) As String
        Try
            Dim BackupDatei As String
            Dim backupRoot As String
            Dim backupDir As String
            backupRoot = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
                                         "Paradigma\Archiv_Checkout\" & myGlobalz.sitzung.aktVorgangsID & "\Backup\")
            '  Backupdir = backupRoot & "\" & myglobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum.ToString("yyyy")
            backupDir = backupRoot ' & "\" & myglobalz.sitzung.aktVorgangsID & "\"
            IO.Directory.CreateDirectory(backupDir)
            BackupDatei = backupDir & dokid & Now.ToString("_yyyyMMddHHmmss") & quelldatei.Extension
            quelldatei.CopyTo(BackupDatei)
            Return BackupDatei
        Catch ex As Exception
            nachricht("fehler in BackupAnlegen: ", ex)
            Return "fehler"
        End Try
    End Function

    Friend Shared Function sindDokumenteImZugriff() As Boolean
        Dim wordInEdit As Boolean = True
        Dim excelInEdit As Boolean = True
        Try
            If myGlobalz.sitzung.wordDateiImEditModus Is Nothing Then
                wordInEdit = False
            Else
                If myGlobalz.sitzung.wordDateiImEditModus.DateinameMitExtension.IsNothingOrEmpty Then
                    wordInEdit = False
                Else
                    If myGlobalz.sitzung.wordDateiImEditModus.DateinameMitExtension = String.Empty Then
                        wordInEdit = False
                    Else
                        wordInEdit = True
                    End If
                End If
            End If
            If myGlobalz.sitzung.excelDateiImEditModus Is Nothing Then
                excelInEdit = False
            Else
                If myGlobalz.sitzung.excelDateiImEditModus.DateinameMitExtension.IsNothingOrEmpty Then
                    excelInEdit = False
                Else
                    If myGlobalz.sitzung.excelDateiImEditModus.DateinameMitExtension = String.Empty Then
                        excelInEdit = False
                    Else
                        excelInEdit = True
                    End If
                End If
            End If
            If wordInEdit Or excelInEdit Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("fehler in sindDokumenteImZugriff ", ex)
        End Try
#Disable Warning BC42353 ' Function 'sindDokumenteImZugriff' doesn't return a value on all code paths. Are you missing a 'Return' statement?
    End Function
#Enable Warning BC42353 ' Function 'sindDokumenteImZugriff' doesn't return a value on all code paths. Are you missing a 'Return' statement?


    Public Shared Sub WordCreateNePumuckel(docid As Integer, pumuckelversion As Integer)
        nachricht("WordCreateNePumuckel --------------------------")
        Try
            Dim test As New IO.FileInfo(myGlobalz.Arc.lokalerCheckoutcache)
            myGlobalz.WordSperreeschonAktiv = False
            nachricht("officeDateiFullName : " & myGlobalz.sitzung.aktDokument.DateinameMitExtension)
            nachricht("aktVorgangsID : " & myGlobalz.sitzung.aktVorgangsID)
            nachricht("docid : " & docid)
            Dim watchpfad = test.DirectoryName & "\" & myGlobalz.sitzung.aktVorgangsID & "\" & docid
            test = Nothing
            If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.DOC Then
                l("if")
                myGlobalz.sitzung.wordDateiImEditModus = CType(myGlobalz.sitzung.aktDokument.Clone, Dokument)
                myGlobalz.sitzung.wordDateiImEditModus.tempEditDatei = calcTempDateiNameDocxXlsx(myGlobalz.sitzung.aktDokument)
                myGlobalz.sitzung.wordDateiImEditModus.DateinameMitExtension = myGlobalz.sitzung.aktDokument.DateinameMitExtension
                Dim ausgabeVerzeichnis As String = ""
                myGlobalz.sitzung.wordDateiImEditModus.FullnameCheckout = myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
                myGlobalz.sitzung.wordDateiImEditModus.FullnameImArchiv = myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            End If
            l("vor")
            worddateiueberwachen(watchpfad, pumuckelversion)
            myGlobalz.sitzung.wordDateiImEditModus = Nothing
            nachricht("WordCreateNePumuckel ---------ende-----------------")
        Catch ex As Exception
            nachricht("Fehler in : WordCreateNePumuckel ---------ende-----------------", ex)
        End Try
    End Sub
    Private Shared Sub pptxdateiueberwachen(watchpfad As String, PumuckelVersion As Integer)
        l("pptxdateiueberwachen")
        Dim gaens As String = Chr(34)
        Dim arguments As String = "watchPfad=" & gaens & watchpfad & gaens & " " &
                        "tempEditDatei=" & gaens & myGlobalz.sitzung.pptxDateiImEditModus.tempEditDatei & gaens & " " &
                        "FullnameCheckout=" & gaens & myGlobalz.sitzung.pptxDateiImEditModus.FullnameCheckout & gaens & " " &
                        "FullnameImArchiv=" & gaens & myGlobalz.sitzung.pptxDateiImEditModus.FullnameImArchiv & gaens & " " &
                        "revisionssicher=" & gaens & myGlobalz.sitzung.pptxDateiImEditModus.revisionssicher & gaens & " " &
                        "dokid=" & gaens & myGlobalz.sitzung.pptxDateiImEditModus.DocID & gaens & " " &
                        "kompress=" & gaens & "0" & gaens & " " &
                        "vid=" & gaens & myGlobalz.sitzung.aktVorgangsID & gaens
        l("arguments " & arguments)
        Dim Process As Process = New Process()
        Process.StartInfo.FileName = "c:\kreisoffenbach\pumuckel\pumuckel.exe "
        If myGlobalz.PumuckelVersion = 0 Then
            Process.StartInfo.FileName = "c:\kreisoffenbach\pumuckel\pumuckel.exe "
        End If
        If myGlobalz.PumuckelVersion = 2 Then
            Process.Start(watchpfad)
            Exit Sub
        End If
        Process.StartInfo.Arguments = arguments
        Process.StartInfo.ErrorDialog = True
        Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        l("vor start")
        Try
            l("pptxdateiueberwachen---------------------- anfang")
            Process.Start()
            l("pptxdateiueberwachen---------------------- ende")
        Catch ex As Exception
            l("Fehler in pptxdateiueberwachen: ", ex)
        End Try
    End Sub

    Private Shared Sub worddateiueberwachen(watchpfad As String, pumuckelversion As Integer)
        l("worddateiueberwachen")
        Dim gaens As String = Chr(34)
        Dim arguments As String = "watchPfad=" & gaens & watchpfad & gaens & " " &
                        "tempEditDatei=" & gaens & myGlobalz.sitzung.wordDateiImEditModus.tempEditDatei & gaens & " " &
                        "FullnameCheckout=" & gaens & myGlobalz.sitzung.wordDateiImEditModus.FullnameCheckout & gaens & " " &
                        "FullnameImArchiv=" & gaens & myGlobalz.sitzung.wordDateiImEditModus.FullnameImArchiv & gaens & " " &
                        "revisionssicher=" & gaens & myGlobalz.sitzung.wordDateiImEditModus.revisionssicher & gaens & " " &
                        "dokid=" & gaens & myGlobalz.sitzung.wordDateiImEditModus.DocID & gaens & " " &
                        "kompress=" & gaens & Math.Abs(CInt(myGlobalz.sitzung.aktDokument.kompressed)) & gaens & " " &
                        "vid=" & gaens & myGlobalz.sitzung.aktVorgangsID & gaens
        l("arguments " & arguments)
        Dim Process As Process = New Process()
        If myGlobalz.PumuckelVersion = 1 Then
            Process.StartInfo.FileName = "c:\kreisoffenbach\pumuckel\pumuckelinterop.exe "
        End If
        If myGlobalz.PumuckelVersion = 0 Then
            Process.StartInfo.FileName = "c:\kreisoffenbach\pumuckel\pumuckel.exe "
        End If
        If myGlobalz.PumuckelVersion = 2 Then
            Process.Start(watchpfad)
            Exit Sub
        End If
        Process.StartInfo.Arguments = arguments
        Process.StartInfo.ErrorDialog = True
        Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        l("vor start")
        Try
            l("worddateiueberwachen---------------------- anfang")
            Process.Start()
            l("worddateiueberwachen---------------------- ende")
        Catch ex As Exception
            l("Fehler in worddateiueberwachen: ", ex)
        End Try
    End Sub

    Private Shared Sub XLSdateiueberwachen(watchpfad As String, PumuckelVersion As Integer)
        Dim gaens As String = Chr(34)
        Dim arguments As String = "watchPfad=" & gaens & watchpfad & gaens & " " &
                        "tempEditDatei=" & gaens & myGlobalz.sitzung.excelDateiImEditModus.tempEditDatei & gaens & " " &
                        "FullnameCheckout=" & gaens & myGlobalz.sitzung.excelDateiImEditModus.FullnameCheckout & gaens & " " &
                        "FullnameImArchiv=" & gaens & myGlobalz.sitzung.excelDateiImEditModus.FullnameImArchiv & gaens & " " &
                        "revisionssicher=" & gaens & myGlobalz.sitzung.excelDateiImEditModus.revisionssicher & gaens & " " &
                        "dokid=" & gaens & myGlobalz.sitzung.excelDateiImEditModus.DocID & gaens & " " &
                        "kompress=" & gaens & "0" & gaens & " " &
                        "vid=" & gaens & myGlobalz.sitzung.aktVorgangsID & gaens
        Dim Process As Process = New Process()
        Process.StartInfo.FileName = "c:\kreisoffenbach\pumuckel\pumuckel.exe "


        If myGlobalz.PumuckelVersion = 0 Then
            Process.StartInfo.FileName = "c:\kreisoffenbach\pumuckel\pumuckel.exe "
        End If
        If myGlobalz.PumuckelVersion = 2 Then
            Process.Start(watchpfad)
            Exit Sub
        End If


        Process.StartInfo.Arguments = arguments
        Process.StartInfo.ErrorDialog = True
        Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        Process.Start()
    End Sub

    Private Shared Function calcTempDateiNameDocxXlsx(datei As Dokument) As String
        Dim ttt As String = ""
        Dim abschnitt As Integer = 2
        Dim lenge As Integer = 2
        Dim fi As IO.FileInfo
        Try
            fi = New IO.FileInfo(datei.DateinameMitExtension)
            ttt = fi.Name.Replace(fi.Extension, "")
            lenge = ttt.Count
            If lenge > 7 Then
                abschnitt = 2
                Return "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 2)
            End If
            If lenge = 7 Then
                abschnitt = 1
                Return "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 1)
            End If
            If lenge < 7 Then
                abschnitt = 0
                Return "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 0)
            End If
        Catch ex As Exception
            nachricht("fehler in calcTempDateiName: ", ex)
            Return ""
        End Try
        ' Return "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 2)
#Disable Warning BC42105 ' Function 'calcTempDateiNameDocxXlsx' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.
    End Function
#Enable Warning BC42105 ' Function 'calcTempDateiNameDocxXlsx' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.

    'Public Shared Sub excelCreateNewFileSystemWatcherAndSetItsProperties(docid As Integer)
    '    nachricht("OfficeCreateNewFileSystemWatcherAndSetItsProperties --------------------------")
    '    Try
    '        Dim test As New IO.FileInfo(myglobalz.Arc.lokalerCheckoutcache)
    '        myglobalz.ExcelSperreschonAktiv = False
    '        nachricht("officeDateiFullName : " & myglobalz.sitzung.aktDokument.DateinameMitExtension)
    '        CLstart.mycSimple.excelDocWatcher = New FileSystemWatcher
    '        'CLstart.mycSimple.excelDocWatcher.Path = test.DirectoryName & "\" & myGlobalz.sitzung.aktVorgangsID
    '        CLstart.mycSimple.excelDocWatcher.Path = test.DirectoryName & "\" & myglobalz.sitzung.aktVorgangsID & "\" & docid
    '        test = Nothing
    '        If myglobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.XLS Then
    '            myglobalz.sitzung.excelDateiImEditModus.tempEditDatei = "~$" & myglobalz.sitzung.aktDokument.DateinameMitExtension
    '            'hier wird nix abgeschnitten "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 2)
    '            myglobalz.sitzung.excelDateiImEditModus = CType(myglobalz.sitzung.excelDateiImEditModus.Clone, Dokument)
    '            myglobalz.sitzung.excelDateiImEditModus.DateinameMitExtension = myglobalz.sitzung.aktDokument.DateinameMitExtension
    '            myglobalz.sitzung.excelDateiImEditModus.FullnameCheckout = myglobalz.sitzung.aktDokument.makeFullname_Checkout(myglobalz.sitzung.aktVorgangsID, myglobalz.Arc.lokalerCheckoutcache, myglobalz.sitzung.aktDokument.DocID)
    '            myglobalz.sitzung.excelDateiImEditModus.FullnameImArchiv = myglobalz.sitzung.aktDokument.makeFullname_ImArchiv(myglobalz.Arc.rootDir)
    '            CLstart.mycSimple.excelDocWatcher.Filter = myglobalz.sitzung.excelDateiImEditModus.tempEditDatei
    '            AddHandler CLstart.mycSimple.excelDocWatcher.Deleted, AddressOf OnDeletedFileSystemCacheExcel
    '            CLstart.mycSimple.excelDocWatcher.EnableRaisingEvents = True
    '        End If
    '        nachricht("excelCreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------")
    '    Catch ex As Exception
    '        nachricht("Fehler in : excelCreateNewFileSystemWatcherAndSetItsProperties ---------ende-----------------" ,ex)
    '    End Try
    'End Sub

    Public Shared Sub ExcelCreateNePumuckel(docid As Integer, PumuckelVersion As Integer)
        nachricht("ExcelCreateNePumuckel --------------------------")
        Try
            l("ExcelCreateNePumuckel---------------------- anfang")
            Dim test As New IO.FileInfo(myGlobalz.Arc.lokalerCheckoutcache)
            nachricht("officeDateiFullName : " & myGlobalz.sitzung.aktDokument.DateinameMitExtension)
            myGlobalz.ExcelSperreschonAktiv = False
            nachricht("officeDateiFullName : " & myGlobalz.sitzung.aktDokument.DateinameMitExtension)
            nachricht("aktVorgangsID : " & myGlobalz.sitzung.aktVorgangsID)
            nachricht("docid : " & docid)
            'CLstart.mycSimple.excelDocWatcher = New FileSystemWatcher
            ''CLstart.mycSimple.excelDocWatcher.Path = test.DirectoryName & "\" & myGlobalz.sitzung.aktVorgangsID
            'CLstart.mycSimple.excelDocWatcher.Path = test.DirectoryName & "\" & myglobalz.sitzung.aktVorgangsID & "\" & docid
            Dim watchpfad = test.DirectoryName & "\" & myGlobalz.sitzung.aktVorgangsID & "\" & docid
            l("watchpfad: " & watchpfad)
            test = Nothing
            If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.XLS Then
                l("myglobalz.sitzung.aktDokument.getDokTyp: " & myGlobalz.sitzung.aktDokument.getDokTyp)
                myGlobalz.sitzung.excelDateiImEditModus = CType(myGlobalz.sitzung.aktDokument.Clone, Dokument)

                '     myglobalz.sitzung.excelDateiImEditModus = CType(myglobalz.sitzung.excelDateiImEditModus.Clone, Dokument)
                myGlobalz.sitzung.excelDateiImEditModus.tempEditDatei = "~$" & myGlobalz.sitzung.aktDokument.DateinameMitExtension
                'hier wird nix abgeschnitten "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 2)
                Dim ausgabeVerzeichnis As String = ""
                myGlobalz.sitzung.excelDateiImEditModus.DateinameMitExtension = myGlobalz.sitzung.aktDokument.DateinameMitExtension
                myGlobalz.sitzung.excelDateiImEditModus.FullnameCheckout = myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
                myGlobalz.sitzung.excelDateiImEditModus.FullnameImArchiv = myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                'CLstart.mycSimple.excelDocWatcher.Filter = myglobalz.sitzung.excelDateiImEditModus.tempEditDatei
                'AddHandler CLstart.mycSimple.excelDocWatcher.Deleted, AddressOf OnDeletedFileSystemCacheExcel
                'CLstart.mycSimple.excelDocWatcher.EnableRaisingEvents = True
            End If
            l("Vor überwachen")
            XLSdateiueberwachen(watchpfad, PumuckelVersion)
            myGlobalz.sitzung.excelDateiImEditModus = Nothing

            l("ExcelCreateNePumuckel---------------------- ende")
        Catch ex As Exception
            l("Fehler in ExcelCreateNePumuckel: ", ex)
        End Try
    End Sub
    'Public Shared Sub OnDeletedFileSystemCacheWord(ByVal source As Object, ByVal e As FileSystemEventArgs)
    '    Try
    '        'myglobalz.WordSperreeschonAktiv = False
    '        If myglobalz.WordSperreeschonAktiv Then
    '            nachricht("delteschonAktiv deshalb keine aktion")
    '        Else
    '            myglobalz.WordSperreeschonAktiv = True
    '            nachricht("delteschonAktiv false deshalb  aktion")
    '            Dim geloeschteDatei As String = e.FullPath.ToString
    '            Dim erfolg As Boolean
    '            'Dim dokupdate As New winDokumentUpdateArchiv(geloeschteDatei)
    '            'dokupdate.Show()

    '            erfolg = detailsTools.wurdenDokumenteGeaendert(geloeschteDatei)
    '            If erfolg Then
    '                MessageBox.Show("Die geänderte Word-Datei wurde erfolgreich ins Archiv übernommen. " & Environment.NewLine & Environment.NewLine,
    '                                    "Übernahme der geänderten Datei ins Archiv",
    '                                    MessageBoxButton.OK, MessageBoxImage.Information)
    '            End If
    '            nachricht("OnDeletedFileSystemCacheOFFICE -------------ende------------- Office-datei wurde geschlossen und ins archiv integriert")
    '        End If
    '        CLstart.mycSimple.wordDocWatcher.EnableRaisingEvents = False
    '        CLstart.mycSimple.wordDocWatcher.Dispose()
    '        nachricht("OnDeletedFileSystemCacheOFFICE --------------------------vor invoke_neu")
    '        source = Nothing
    '    Catch ex As Exception
    '        nachricht("fehler in OnDeletedFileSystemCacheOFFICE " ,ex)
    '    End Try
    'End Sub
    'Public Shared Sub OnDeletedFileSystemCacheExcel(ByVal source As Object, ByVal e As FileSystemEventArgs)
    '    Try
    '        If myglobalz.ExcelSperreschonAktiv Then
    '            nachricht("delteschonAktiv deshalb keine aktion")
    '        Else
    '            myglobalz.ExcelSperreschonAktiv = True
    '            nachricht("delteschonAktiv false deshalb  aktion")
    '            Dim geloeschteDatei As String = e.FullPath.ToString
    '            'nachricht("Dokument wurde geändert?: " & detailsTools.wurdenDokumenteGeaendert(geloeschteDatei).ToString)
    '            Dim erfolg As Boolean
    '            'Dim dokupdate As New winDokumentUpdateArchiv(geloeschteDatei)
    '            'dokupdate.Show()
    '            erfolg = detailsTools.wurdenDokumenteGeaendert(geloeschteDatei)
    '            If erfolg Then
    '                MessageBox.Show("Die geänderte Excel-Datei wurde erfolgreich ins Archiv übernommen. " & Environment.NewLine & Environment.NewLine,
    '                                    "Übernahme der geänderten Datei ins Archiv",
    '                                    MessageBoxButton.OK, MessageBoxImage.Information)
    '            End If
    '            nachricht("OnDeletedFileSystemCacheExcel -------------ende------------- Office-datei wurde geschlossen und ins archiv integriert")
    '        End If
    '        CLstart.mycSimple.excelDocWatcher.EnableRaisingEvents = False
    '        CLstart.mycSimple.excelDocWatcher.Dispose()
    '        nachricht("OnDeletedFileSystemCacheExcel --------------------------vor invoke_neu")
    '        source = Nothing
    '    Catch ex As Exception
    '        nachricht("fehler in OnDeletedFileSystemCacheExcel " ,ex)
    '    End Try
    'End Sub
    Public Shared Function schliessenAbbrechen1(mesres As MessageBoxResult, dokInArbeit As String) As MessageBoxResult
        mesres = MessageBox.Show("Es sind noch Word/Exceldokumente im Zugriff: " & Environment.NewLine & Environment.NewLine &
                                 "  >>>  " & dokInArbeit & Environment.NewLine & Environment.NewLine &
                                 "Bitte abspeichern und schließen." & Environment.NewLine &
                                 "Möchten Sie die Änderungen am Dokument verwerfen ?" & Environment.NewLine &
                                 "   Ja   - Änderungen verwerfen" & Environment.NewLine &
                                 "   Nein - Im Vorgang bleiben um Wordokument zu schliessen.",
                                 "Dokumente sind noch geöffnet", MessageBoxButton.YesNo, MessageBoxImage.Error)
        Return mesres
    End Function
    Public Shared Sub FSW_instantiieren(darst As Boolean, pumuckelversion As Integer)
        Try
            If darst Then
                If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.DOC Then
                    If (myGlobalz.sitzung.aktDokument.nurzumlesen Or myGlobalz.sitzung.aktDokument.revisionssicher) Then
                    Else
                        detailsTools.WordCreateNePumuckel(myGlobalz.sitzung.aktDokument.DocID, pumuckelversion)
                    End If
                End If
                If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.PPT Then
                    If Not myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.EndsWith("pptx") Then
                        MessageBox.Show("Achtung: Dateien mit der Endung .ppt können nach Änderung nicht mehr " & Environment.NewLine &
                                            "          automatisch übernommen werden. (Hintergrund ist eine Änderung im aktuellen PPT)" & Environment.NewLine &
                                            "" & Environment.NewLine &
                                            "Ausweg: Ändern Sie den Dateityp nach .pptx   !" & Environment.NewLine &
                                            "        Dann kann Paradigma wieder die Änderungen registrieren und die geänderte Datei automatisch ins Archiv übernehmen.")
                    Else
                        If (myGlobalz.sitzung.aktDokument.nurzumlesen Or myGlobalz.sitzung.aktDokument.revisionssicher) Then
                        Else
                            detailsTools.PptxCreateNePumuckel(myGlobalz.sitzung.aktDokument.DocID, pumuckelversion)
                        End If
                    End If
                End If
                If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.XLS Then
                    If (myGlobalz.sitzung.aktDokument.nurzumlesen Or myGlobalz.sitzung.aktDokument.revisionssicher) Then
                    Else
                        If myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.EndsWith(".xls") Then
                            MessageBox.Show("Achtung: Dateien mit der Endung .xls können nach Änderung nicht mehr " & Environment.NewLine &
                                            "          automatisch übernommen werden. (Hintergrund ist eine Änderung im aktuellen Excel)" & Environment.NewLine &
                                            "" & Environment.NewLine &
                                            "Ausweg: Ändern Sie den Dateityp nach .xlsx   !" & Environment.NewLine &
                                            "        Dann kann Paradigma wieder die Änderungen registrieren und die geänderte Datei automatisch ins Archiv übernehmen.")
                        Else
                            detailsTools.ExcelCreateNePumuckel(myGlobalz.sitzung.aktDokument.DocID, pumuckelversion)
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            nachricht("fehler in FSW_instantiieren ", ex)
        End Try
    End Sub

    Private Shared Sub PptxCreateNePumuckel(docID As Integer, PumuckelVersion As Integer)
        nachricht("pptxCreateNePumuckel --------------------------")
        Try
            Dim test As New IO.FileInfo(myGlobalz.Arc.lokalerCheckoutcache)
            'myGlobalz.WordSperreeschonAktiv = False
            nachricht("officeDateiFullName : " & myGlobalz.sitzung.aktDokument.DateinameMitExtension)
            nachricht("aktVorgangsID : " & myGlobalz.sitzung.aktVorgangsID)
            nachricht("docid : " & docID)
            Dim watchpfad = test.DirectoryName & "\" & myGlobalz.sitzung.aktVorgangsID & "\" & docID
            test = Nothing
            If myGlobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.PPT Then
                l("if")
                myGlobalz.sitzung.pptxDateiImEditModus = CType(myGlobalz.sitzung.aktDokument.Clone, Dokument)
                myGlobalz.sitzung.pptxDateiImEditModus.tempEditDatei = calcTempDateiNamePptx(myGlobalz.sitzung.aktDokument)
                myGlobalz.sitzung.pptxDateiImEditModus.DateinameMitExtension = myGlobalz.sitzung.aktDokument.DateinameMitExtension
                Dim ausgabeVerzeichnis As String = ""
                myGlobalz.sitzung.pptxDateiImEditModus.FullnameCheckout = myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
                myGlobalz.sitzung.pptxDateiImEditModus.FullnameImArchiv = myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            End If
            l("vor")
            pptxdateiueberwachen(watchpfad, PumuckelVersion)
            myGlobalz.sitzung.pptxDateiImEditModus = Nothing
            nachricht("pptxCreateNePumuckel ---------ende-----------------")
        Catch ex As Exception
            nachricht("Fehler in : pptxCreateNePumuckel ---------ende-----------------", ex)
        End Try
    End Sub

    Private Shared Function calcTempDateiNamePptx(datei As Dokument) As String
        Dim ttt As String = ""
        Dim abschnitt As Integer = 2
        Dim lenge As Integer = 2
        Dim fi As IO.FileInfo
        Try
            fi = New IO.FileInfo(datei.DateinameMitExtension)
            ttt = fi.Name.Replace(fi.Extension, "")
            lenge = ttt.Count
            Return "~$" & datei.DateinameMitExtension
            'If lenge > 7 Then
            '    abschnitt = 2
            '    Return "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 2)
            'End If
            'If lenge = 7 Then
            '    abschnitt = 1
            '    Return "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 1)
            'End If
            'If lenge < 7 Then
            '    abschnitt = 0
            '    Return "~$" & datei.DateinameMitExtension.Substring(abschnitt, Len(datei.DateinameMitExtension) - 0)
            'End If
        Catch ex As Exception
            nachricht("fehler in calcTempDateiName: ", ex)
            Return ""
        End Try
    End Function

    Friend Shared Sub thumbNailsPDFerzeugen(alle As List(Of clsPresDokumente),
                                        thumbnailRootDir As String,
                                        ereignisDokListe As List(Of clsEreignisDok))
#Disable Warning BC42024 ' Unused local variable: 'aktdokdatei'.
        Dim txetdatei, aktdokdatei, zeile, worddateien As String
#Enable Warning BC42024 ' Unused local variable: 'aktdokdatei'.
        Dim icount As Integer = 0

        txetdatei = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\paradigma\pdfthumb\makePNGfromPDF.txt"
        worddateien = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\paradigma\pdfthumb\makePNGfromWORD.txt"
        'exepdf2png = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\paradigma\pdfthumb\exepdf2png.exe"
        Try
            IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\paradigma\pdfthumb")
            Using sw As New IO.StreamWriter(txetdatei)
                For Each presdok In alle
                    If presdok.DokTyp = DokumentenTyp.PDF Then
                        zeile = presdok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir) & "#"
                        presdok.thumbnailFullPath = thumbnailRootDir & presdok.VorgangsID & "\" & presdok.DocID & ".jpg"
                        presdok.thumbnailFullPath = clsTools.tools.korrigiereThumbnail(presdok.thumbnailFullPath)

                        zeile = zeile & presdok.thumbnailFullPath '& "#"
                        sw.WriteLine(zeile)
                        icount += 1
                    End If
                Next
                If icount > 0 Then
                    'exepdf2png starten
                End If
            End Using
            Using sw As New IO.StreamWriter(worddateien)
                For Each presdok In alle
                    If presdok.DokTyp = DokumentenTyp.DOC Or presdok.DokTyp = DokumentenTyp.RTF Then
                        zeile = presdok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir) & "#"
                        presdok.thumbnailFullPath = thumbnailRootDir & presdok.VorgangsID & "\" & presdok.DocID & ".jpg"
                        presdok.thumbnailFullPath = clsTools.tools.korrigiereThumbnail(presdok.thumbnailFullPath)
                        zeile = zeile & presdok.thumbnailFullPath '& "#"
                        sw.WriteLine(zeile)
                        icount += 1
                    End If
                Next
                If icount > 0 Then
                    'exepdf2png starten
                End If
            End Using
        Catch ex As Exception
            nachricht("fehler in thumbNailsHinzuFuegen a: ", ex)
        Finally
        End Try
    End Sub

    Friend Shared Function sindEreignisDokumenteImZugriff(ereignisid As Integer) As Boolean
        Dim wordInEdit As Boolean = True
        Dim excelInEdit As Boolean = True
        Try
            If myGlobalz.sitzung.wordDateiImEditModus Is Nothing Then
                wordInEdit = False
            Else
                If myGlobalz.sitzung.wordDateiImEditModus.DateinameMitExtension.IsNothingOrEmpty Then
                    wordInEdit = False
                Else
                    If myGlobalz.sitzung.wordDateiImEditModus.DateinameMitExtension = String.Empty Then
                        wordInEdit = False
                    Else
                        If myGlobalz.sitzung.wordDateiImEditModus.EreignisID = ereignisid Then
                            wordInEdit = True
                        Else
                            wordInEdit = False
                        End If

                    End If
                End If
            End If
            If myGlobalz.sitzung.excelDateiImEditModus Is Nothing Then
                excelInEdit = False
            Else
                If myGlobalz.sitzung.excelDateiImEditModus.DateinameMitExtension.IsNothingOrEmpty Then
                    excelInEdit = False
                Else
                    If myGlobalz.sitzung.excelDateiImEditModus.DateinameMitExtension = String.Empty Then
                        excelInEdit = False
                    Else
                        If myGlobalz.sitzung.excelDateiImEditModus.EreignisID = ereignisid Then
                            excelInEdit = False
                        Else
                            excelInEdit = True
                        End If

                    End If
                End If
            End If
            If wordInEdit Or excelInEdit Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("fehler in sindDokumenteImZugriff ", ex)
        End Try
    End Function
End Class
