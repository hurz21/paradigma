Imports System.Data

Public Class detailsTools
    Shared Sub VorgangLocking(ByVal modus As String)
        'myGlobalz.paradigmaDateiServerRoot & "\div\config\locks\", 
        Dim datei = initP.getValue("Haupt.LOCKINGFile")
        Dim lk As New clsVorgangLocking(datei,
                                        myGlobalz.sitzung.aktVorgangsID,
                                        myGlobalz.sitzung.aktBearbeiter.Initiale)
        If modus = "ein" Then
            Dim lockuser = lk.lockingPruefen()
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
    End Sub

    Public Shared Sub EreignisExcel_ClickExtracted()
        Dim handcsv As New clsCSVausgaben("Ereignisse", myGlobalz.sitzung.EreignisseRec.dt, myGlobalz.sitzung.aktVorgangsID)
        nachricht(" exportfile:" & handcsv.ausgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub

    Public Shared Sub BeteiligteExcel_ClickExtracted()
        Dim handcsv As New clsCSVausgaben("Beteiligte", myGlobalz.sitzung.beteiligteREC.dt, myGlobalz.sitzung.aktVorgangsID)
        nachricht(" exportfile:" & handcsv.ausgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub


    Public Shared Sub DokumenteExcel_clickExtracted()
        Dim handcsv As New clsCSVausgaben("Dokumente", myGlobalz.Arc.vorgangDocDt, myGlobalz.sitzung.aktVorgangsID)
        nachricht(" exportfile:" & handcsv.ausgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub
    Public Shared Function fuelleVerwandteDT(ByVal sql As String) As Boolean
        '  Dim sql = 
        'clsVerwandte_mysql.erzeugeVerwandtenlistezuVorgang(sql$)
        VerwandteTools.erzeugeVerwandtenlistezuVorgang.exe(sql)
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

    Public Shared Sub clearCheckoutDokulist()
        myGlobalz.sitzung.checkoutDokuList.Clear()
    End Sub

    'Private Shared Function GetMesresDOK(ByVal dok As LIBArchiv.Dokument) As MessageBoxResult
    '    Dim mesres As MessageBoxResult
    '    mesres = MessageBox.Show("Sie haben das Dokument >" & dok.DateinameMitExtension & "< geändert!" & Environment.NewLine &
    '                    " " & Environment.NewLine &
    '                    "Soll die geänderte Datei ins Archiv übernommen werden ?" & Environment.NewLine &
    '                    " " & Environment.NewLine &
    '                    "  Ja  - ins Archiv übernehmen " & Environment.NewLine &
    '                    " Nein - geänderte Datei LÖSCHEN !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" & Environment.NewLine & Environment.NewLine &
    '                    "  " & Environment.NewLine & Environment.NewLine &
    '                    "(Hinweis: Bitte schließen Sie in jedem Falle alle Officedokumente bevor Sie fortfahren! Es könnten sonst Daten verloren gehen !!!)" & Environment.NewLine & Environment.NewLine,
    '                    "Archiv: Ein Dokument wurde geändert! Um Änderungen zu sichern bitte hier auf JA drücken!",
    '                    MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
    '    Return mesres
    'End Function

    Shared Function wurdenDokumenteGeaendert(ByVal checkoutDokuList As List(Of LIBArchiv.Dokument)) As Boolean
        Dim meincount As Integer = 0
        Dim errorout As String = "errorout>"
        Try
            nachricht("wurdenDokumenteGeaendert: --------------------------------------")
            'todo logging aller dokuarbeiten aufnehmen
            meincount = 1
            If myGlobalz.sitzung.checkoutDokuList.Count < 1 Then Return False
            meincount = 2
            Dim CO_test, AR_test As IO.FileInfo
            For Each dok As LIBArchiv.Dokument In checkoutDokuList
                meincount = 3
                'checkoutnamen bilden
                If String.IsNullOrEmpty(dok.FullnameCheckout) Then
                    meincount = 4
                    nachricht("dok.FullnameCheckout : " & dok.FullnameCheckout)
                    nachricht("warnung: da ist ein filename leer FullnameCheckout")
                    Continue For
                End If
                If String.IsNullOrEmpty(dok.FullnameImArchiv) Then
                    meincount = 44
                    nachricht("dok.FullnameImArchiv : " & dok.FullnameImArchiv)
                    nachricht("warnung: da ist ein filename leer FullnameImArchiv")
                    Continue For
                End If
                meincount = 5
                errorout = errorout & ", dok.OriginalName: " & dok.OriginalName
                errorout = errorout & ", dok.FullnameCheckout: " & dok.FullnameCheckout
                errorout = errorout & ", dok.FullnameImArchiv: " & dok.FullnameImArchiv
                System.Threading.Thread.Sleep(1000)
                ' nachricht(errorout)
                CO_test = New IO.FileInfo(dok.FullnameCheckout)
                AR_test = New IO.FileInfo(dok.FullnameImArchiv)
                meincount = 6
                nachricht("Alt: " & AR_test.LastWriteTime.ToString)
                nachricht("Neu: " & CO_test.LastWriteTime.ToString)
                meincount = 7
                If dok.revisionssicher Then
                    meincount = 8
                    nachricht("Dokument ist revisionssicher. Sie können die Änderungen also nicht direkt ins Archiv übernehmen.")
                    MessageBox.Show(myGlobalz.Infotext_revisionssicherheit, dok.DateinameMitExtension, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
                    Continue For
                End If
                meincount = 9
                Dim dateiWurdeGeaendert As Boolean = AR_test.LastWriteTime < CO_test.LastWriteTime
                meincount = 10
                If dateiWurdeGeaendert Then
                    meincount = 11
                    geaenderteDateiInsArchivUebernehmen(CO_test, dok)
                End If
                meincount = 12
                If Not IsFileWritable(CO_test.FullName) Then
                    meincount = 13
                    Return True
                End If
                meincount = 14
                If Not lokaleKopieLoeschenErfolgreich(CO_test, dok) Then
                    meincount = 15
                    checkoutDokuList.Clear()
                    Return False 'ihah
                Else
                    meincount = 16
                End If
            Next
            meincount = 17
            checkoutDokuList.Clear()
            meincount = 18
            nachricht("checkoutDokuList: checkoutDokuList wurde gelöscht")
            checkoutDokuList.Clear()
            meincount = 19
            Return False
        Catch ex As Exception
            nachricht("2 Fehler inwurdenDokumenteGeaendert: meincount: " & meincount & ", " & errorout & vbCrLf & ex.ToString)
        End Try
    End Function

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
            Return False
        End Try
    End Function

    Private Shared Function KopiereZu(ByVal CO_test As IO.FileInfo, ByVal dok As LIBArchiv.Dokument) As Boolean
        Try
            CO_test.CopyTo(dok.FullnameImArchiv, True)
            Return True
        Catch ex As Exception
            nachricht("Fehler in KopiereZu: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Shared Sub darstellen(ByVal dokumentWurdeGeoeffnet As Boolean)
        If Not myGlobalz.sitzung.aktDokument.isTypeEditable Then
            Exit Sub
        End If
        'If dokumentWurdeGeoeffnet Then
        '    tbDocumentOpened.Text = myGlobalz.sitzung.aktDokument.DateinameMitExtension
        '    tbDocumentOpened.ToolTip = myGlobalz.sitzung.aktDokument.DateinameMitExtension
        'Else
        '    gbxDateiInBenutzung.Visibility = Windows.Visibility.Collapsed
        '    tbDocumentOpened.Text = ""
        '    tbDocumentOpened.ToolTip = ""
        'End If
    End Sub

    Private Shared Sub geaenderteDateiInsArchivUebernehmen(ByVal CO_test As IO.FileInfo, ByVal dok As LIBArchiv.Dokument)
        'Ins Archiv übernhemen
        'todo frag ob als neue version ins Archiv übernehmen
        Try
            If CO_test.Exists Then
                If KopiereZu(CO_test, dok) Then
                    'alles prima
                Else
                    MsgBox("Warnung: Die geänderte Datei wurde nicht übernommen. " & Environment.NewLine &
                           "Bitte öffnen Sie den Vorgang erneut und " & Environment.NewLine &
                           "prüfen Sie ob die Änderungen in der Datei vorhanden sind!", MsgBoxStyle.Critical, "Datei: " & CO_test.Name)
                    Exit Sub
                End If
            End If

            CLstart.myc.aLog.komponente = "Dokumente"
            CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktDokument.DocID & " " &
                                    myGlobalz.sitzung.aktDokument.DateinameMitExtension & ": geaendert"
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
        Catch ex As Exception
            nachricht("Fehler: in geaenderteDateiInsArchivUebernehmen: " & ex.ToString)
        End Try
    End Sub
    Private Shared Function lokaleKopieLoeschenErfolgreich(ByVal CO_test As IO.FileInfo, ByVal dok As LIBArchiv.Dokument) As Boolean
        Try
            If CO_test.Exists Then CO_test.Delete() 'In jedem Falle das Dokument löschen
            Return True
        Catch ex As Exception
            If Not CO_test.Extension.ToLower.Contains("jpg") Then
                MsgBox("Sie haben die Office-Datei '" & dok.DateinameMitExtension & "' noch geöffnet. " & Environment.NewLine &
                   "Bitte schließen Sie die Datei. " & Environment.NewLine &
                   "Drücken sie erst dann auf 'OK' !", MsgBoxStyle.OkOnly, "Office-Dokument schließen!")
                'ihah CO_test.Delete() 'In jedem Falle das Dokument löschen
            End If
            Return False
        End Try
    End Function
    Public Shared Sub eEreignisstarten(ByVal item As String)
        nachricht("USERAKTION:   ereignis hinzufügen pulldown")
        If ereignisauswahlistOK((item)) Then
            starteEreignisdetail((item))
        End If
    End Sub


    Shared Function ereignisauswahlistOK(ByVal typ As String) As Boolean
        If typ.ToLower <> "hinzufügen" Then Return True
        Return False
    End Function



    Private Shared Sub startestandardereignis(ByVal typ$)
        Dim wzereignisdetail As New Window_Ereignis_Detail(typ)
        wzereignisdetail.ShowDialog()
    End Sub

    Shared Sub starteEreignisdetail(ByVal typ As String)
        myGlobalz.sitzung.Ereignismodus = "neu"
        myGlobalz.sitzung.aktEreignis.Datum = Now
        myGlobalz.sitzung.aktEreignis.Art = typ
        myGlobalz.sitzung.aktEreignis.istRTF = False

        Select Case typ.ToLower
            Case "wiedervorlage"
                wiedervorlagestarten()
            Case "aktennotiz schreiben"
                Aktenotizerstellen()
            Case "email schreiben"
                '  myGlobalz.Arc.AllesAuscheckenVorgang(False)
                glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email, "", "", "", myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, False)
            Case "outlookemail übernehmen"
                outlookemnailUebernehmen(myGlobalz.sitzung.aktVorgangsID)
            Case "zahlung"
                glob2.ZahlungFormoeffen()
            Case "notiz"
                startestandardereignis(typ)
            Case "weblink"
                glob2.webLinkedit("neu")
            Case Else
                startestandardereignis(typ)
        End Select
        clsEreignisTools.setLetztesEreignisText(myGlobalz.sitzung.aktEreignis)
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LASTACTIONHEROE")
        ' EditspeichernStammdaten_alledb(myGlobalz.sitzung.aktEreignis.Datum)
    End Sub

    'Public Shared Sub EditspeichernStammdaten_alledb(ByVal zeitstempel As Date)
    '    If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
    '        nachricht("myGlobalz.vorgang_MYDB.dbtyp : " & myGlobalz.vorgang_MYDB.dbtyp)
    '        Dim zzz As New clsStammCRUD_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
    '        If zzz.EDIT_speichern_stammdaten(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.aktVorgang.Stammdaten) Then
    '            nachricht("Stammdaten wurden angepasst")
    '        Else
    '            nachricht("Fehler Stammdaten wurden nicht angepasst")
    '        End If
    '    End If
    '    If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
    '        nachricht("myGlobalz.vorgang_MYDB.dbtyp : " & myGlobalz.vorgang_MYDB.dbtyp)
    '        Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
    '        If zzz.EDIT_speichern_stammdaten(myGlobalz.sitzung.aktVorgangsID,
    '                                         myGlobalz.sitzung.VorgangREC,
    '                                         myGlobalz.sitzung.aktVorgang.Stammdaten,
    '                                         zeitstempel) Then
    '            nachricht("Stammdaten wurden angepasst")
    '        Else
    '            nachricht("aFehler Stammdaten wurden nicht angepasst")
    '        End If
    '    End If
    'End Sub

    Public Shared Sub Edit_singleUpdate_Stammdaten(ByVal zeitstempel As Date, singleUpdateFieldName As String)
        Try
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                nachricht("myGlobalz.vorgang_MYDB.dbtyp : " & myGlobalz.vorgang_MYDB.dbtyp)
                Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                If zzz.EDIT_singleupdate_stammdaten(myGlobalz.sitzung.aktVorgangsID,
                                                 myGlobalz.sitzung.VorgangREC,
                                                 myGlobalz.sitzung.aktVorgang.Stammdaten,
                                                 zeitstempel, singleUpdateFieldName) Then
                    nachricht("Stammdaten wurden angepasst")
                Else
                    nachricht("bFehler Stammdaten wurden nicht angepasst")
                End If
            End If
        Catch ex As Exception
            nachricht("Fehler Edit_singleUpdate_Stammdaten ", ex)
        End Try
    End Sub

    Public Shared Sub Aktenotizerstellen()
        Dim wz As New clsRichtextbox("1")
        wz.init("")
        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " & myGlobalz.sitzung.aktEreignis.Beschreibung & ": neueAktennotiz als RTF angelegt" : CLstart.myc.aLog.log()
    End Sub

    Public Shared Sub outlookemnailUebernehmen(aktvid As Integer)
        Dim fotoZuRaumbezug As Boolean
        Dim memailtools As New clsOutlookEmail
        '  fotoZuRaumbezug = memailtools.fotoZuRaumbezug
        memailtools.Aufnahme(fotoZuRaumbezug, aktvid)
        memailtools = Nothing
    End Sub
    Public Shared Sub EMLemnailUebernehmen(ByVal emlfullpath As String, aktvid As Integer)
        Dim fotoZuRaumbezug As Boolean
        Dim memailtools As New clsEMLemail(emlfullpath)
        '  fotoZuRaumbezug = memailtools.fotoZuRaumbezug
        memailtools.Aufnahme(fotoZuRaumbezug, aktvid)
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
                .istVeraltet = CBool(clsDBtools.fieldvalue(meineDT.Rows(0).Item("veraltet")))
                .ExifDatum = CDate(clsDBtools.fieldvalue(meineDT.Rows(0).Item("ExifDatum")))
                .EXIFlon = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFlong")))
                .EXIFlat = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFlat")))
                .EXIFdir = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFdir")))
                .EXIFhersteller = CStr(clsDBtools.fieldvalue(meineDT.Rows(0).Item("EXIFhersteller")))

            End With
        Catch ex As Exception
            nachricht("Fehler2: DTaufFotoObjektabbilden " & vbCrLf & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Shared Sub AlleBeteiligtenLoeschen()
        clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(myGlobalz.sitzung.aktVorgangsID) ' myGlobalz.sitzung.beteiligteREC.dt
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

    Public Shared Sub AlleBeteiligtenKopieren(ByVal quellvorgangsid As Integer, ByVal Zielvorgangsid%)
        '  clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(quellvorgangsid) 'myGlobalz.sitzung.VorgangsID) ' myGlobalz.sitzung.beteiligteREC.dt
        clsBeteiligteBUSI.initBeteiligteDatatable(quellvorgangsid, myGlobalz.sitzung.beteiligteREC) 'myGlobalz.sitzung.beteiligteREC wird aktualisert

        clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", "System.Int16")
        clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", 1)

        Dim messi As New MessageBoxResult

        If Not myGlobalz.sitzung.beteiligteREC.dt.IsNothingOrEmpty Then
            For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
                clsBeteiligteBUSI.BeteiligtenRec2Obj(zeile, myGlobalz.sitzung.aktPerson)
                clsBeteiligteBUSI.personZuZielvorgangKopieren(CInt(zeile.Item("PersonenID")), CInt(zeile.Item("Status")), quellvorgangsid, Zielvorgangsid)
            Next
        Else
            MessageBox.Show("Es sind noch keine Beteiligten erfasst worden!")
        End If
    End Sub



    Shared Function AlleDokumentenKopieren(ByVal quellVorgangsID As Integer, ByVal ZielvorgangsIdInput As Integer) As String
        'alledokus auflisten
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(quellVorgangsID, "beides") ' nach myGlobalz.Arc.ArcRec
        Dim kopiert = 0
        Dim kopierteEreignisse As Integer = 0
        Dim result As String
        Dim NumDir As String = ""
        NumDir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.defineArchivVorgangsDir(ZielvorgangsIdInput))
        If bresult Then
            For i = 0 To myGlobalz.Arc.ArcRec.dt.Rows.Count - 1
                DokArc.DokuZeile2OBJ(i, myGlobalz.sitzung.aktDokument, myGlobalz.Arc.ArcRec.dt) 'select *
                myGlobalz.sitzung.aktDokument.makeFullname_Checkout(quellVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                checkout.checkout(myGlobalz.sitzung.aktDokument, quellVorgangsID)   'checkout findet IMMER statt
                If dokumentschonImVorgangvorhanden(myGlobalz.sitzung.aktDokument, ZielvorgangsIdInput) Then
                    Continue For
                End If

                Dim datei As String = myGlobalz.sitzung.aktDokument.FullnameCheckout
                Dim Beschreibung As String = myGlobalz.sitzung.aktDokument.Beschreibung
                Dim ereignisid As Integer = DokArcOracle.getEreignisID4DokId(myGlobalz.sitzung.aktDokument.DocID)

                myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                ' myGlobalz.sitzung.aktEreignis.ID ist unbekannt
                If ereignisid > 0 Then
                    'zu diesem Dokument gibt es ein Ereignis
                    Dim erfolg As Boolean = clsEreignisTools.Ereigniskopieren(ereignisid, ZielvorgangsIdInput, False)
                    myGlobalz.sitzung.aktDokument.EreignisID = myGlobalz.sitzung.aktEreignis.ID
                    kopierteEreignisse += 1
                End If

                Dim erfolgreich As Boolean = myGlobalz.Arc.checkINDoku(datei,
                                                                       myGlobalz.sitzung.aktDokument.EreignisID,
                                                                       Beschreibung,
                                                                       ZielvorgangsIdInput,
                                                                       False,
                                                                       NumDir,
                                                                       myGlobalz.sitzung.aktDokument.Filedatum,
                                                                       myGlobalz.sitzung.aktDokument.DocID,
                                                                       myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
                If erfolgreich Then
                    nachricht("Checkin  erfolgreich: " & datei)
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
            detailsTools.initEreigisseDatatable(hinweis, myGlobalz.sitzung.aktVorgangsID, False, ereignisDokListe)
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
            nachricht("fehler in AlleEreignisseLoeschen: " & ex.ToString)
        End Try
    End Sub

    Private Shared Function getDokid4RB(ByVal zeile As DataRow) As Integer
        Dim DocID As Integer
        Try
            DocID = CInt(clsDBtools.fieldvalue(zeile.Item("dokumentid")))
            Return DocID
        Catch ex As Exception
            nachricht("fehler in getDokid4RB: " & ex.ToString)
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
        Else
            MessageBox.Show("Es sind noch keine Ereignisse erfasst worden!")
        End If
    End Sub

    Public Shared Function getNewVgrundString() As String
        Dim sb As New System.Text.StringBuilder

        If myGlobalz.layerListControlObjekts.Count < 1 Then
            Return ""
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

    Private Shared Function bildeEmailAnBAtext() As String
        Return "Hier die Stellungnahme des Immissionsschutzes / der Unteren Naturschutzbehörde / der Unteren Wasserbehörde zu o.g. Angelegenheit." & Environment.NewLine &
                                           "Bitte senden Sie uns Ihre Bau-/Abbruchgenehmigung bzw. Versagung der Bau-/Abbruchgenehmigung auch als PDF zu," & Environment.NewLine &
                                           "Danke!"
    End Function
    Public Shared Function Archiv_aktiviere_Dokument() As Boolean
        Dim box As New WINBox("dokument")
        box.ShowDialog()
        '  MsgBox("knopfnummer:" & box.knopfnummer)
        If box.knopfnummer = 1 Then
            nachricht("USERAKTION: Einzelnes Dokument öffnen")
            box.dokumentWurdeGeoeffnet = True
            ' Me.Close()
            If DokArc.machCheckout("zeige") Then
                DokArc.Archiv_aktiviere_dokument(myGlobalz.sitzung.aktDokument)
                DokArc.zulisteAddieren()
            End If
        End If
        If box.knopfnummer = 2 Then
            Dim modus = "metaedit"
            nachricht("USERAKTION: Einzelnes Dokument metaedit")
            ' If DokArc.machCheckout(modus) Then ' wozu checkout beim metaedit????
            box.dokumentWurdeGeoeffnet = False
            DokArc.editDokumentMetadata()
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
                DokArc.ausCheckoutlisteEntfernen(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.checkoutDokuList)
                CLstart.myc.aLog.komponente = "Dokumente" : CLstart.myc.aLog.aktion = String.Format("{0} {1}: geloescht", myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.DateinameMitExtension) : CLstart.myc.aLog.log()
                Return False
            End If
        End If
        If box.knopfnummer = 4 Then
            nachricht("USERAKTION: Einzelnes Dokument mailen")
            myGlobalz.Arc.einzeldokument_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID,
                                                    myGlobalz.sitzung.aktDokument)

            glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email, "", "",
                                   myGlobalz.sitzung.aktDokument.FullnameCheckout,
                                   myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email,
                                   False)
        End If
        If box.knopfnummer = 5 Then
            nachricht("USERAKTION: Einzelnes Dokument kopieren")
            nachZielKopieren.NachZielKopieren.exe(myGlobalz.sitzung.aktDokument, "o:")
        End If
        If box.knopfnummer = 6 Then
            nachricht("USERAKTION: Einzelnes Dokument kopieren nach dokumente")
            nachZielKopieren.NachZielKopieren.exe(myGlobalz.sitzung.aktDokument, Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments))
        End If
        If box.knopfnummer = 7 Then
            nachricht("USERAKTION: Einzelnes Dokument mailen an ba")
            myGlobalz.Arc.einzeldokument_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID,
                                                myGlobalz.sitzung.aktDokument)
            myGlobalz.Arc.einzeldokument_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID,
                                                    myGlobalz.sitzung.aktDokument)
            glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email, "",
                                   bildeEmailAnBAtext(),
                                   myGlobalz.sitzung.aktDokument.FullnameCheckout, myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, True)
        End If
        If box.knopfnummer = 8 Then
            nachricht("USERAKTION: Einzelnes Dokument im archiv kopieren")
            If DokArc.machCheckout("zeige") Then
                nachZielKopieren.DolumentImArchivKopieren.exe(myGlobalz.sitzung.aktDokument)
            End If
        End If
        If box.knopfnummer = 9 Then
            nachricht("USERAKTION: Einzelnes Dokument direkt drucken")
            If DokArc.machCheckout("drucke") Then
                nachZielKopieren.dokumentdrucken.exe(myGlobalz.sitzung.aktDokument)
            End If
        End If
        If box.knopfnummer = 10 Then
            nachricht("USERAKTION: Einzelnes Dokument revisionssicher machen")
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
                        CLstart.myc.aLog.komponente = "Dokumente" : CLstart.myc.aLog.aktion = String.Format("{0} {1}: metadaten geaendert", myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.DateinameMitExtension) : CLstart.myc.aLog.log()
                    Else
                        MsgBox("Speichern nicht erfolgreich. Formular schließen")
                    End If
                End If
            End If
        End If
        Return box.dokumentWurdeGeoeffnet
    End Function
    Public Shared Sub RaumbezugExcel_clickExtracted()
        Dim handcsv As New clsCSVausgaben("Raumbezuege", myGlobalz.sitzung.raumbezugsRec.dt, myGlobalz.sitzung.aktVorgangsID)
        nachricht(" exportfile:" & handcsv.ausgeben())
        handcsv.start()
        handcsv.Dispose()
    End Sub

    Shared Function erstelleCSVausgabeDerFlurstuecke(ByVal p1 As Integer) As Boolean
        Dim handcsv As New clsCSVausgaben("FlurstueckeFuerEigentuemer", myGlobalz.sitzung.raumbezugsRec.dt, myGlobalz.sitzung.aktVorgangsID)
        nachricht(" exportfile:" & handcsv.ausgeben())
        handcsv.Dispose()
    End Function

    Public Shared Function dokumentschonImVorgangvorhanden(ByVal dokument As Dokument, ByVal ZielvorgangsidInput As Integer) As Boolean

        myGlobalz.sitzung.tempREC.mydb.SQL =
                  "SELECT * from dok2vid where vorgangsid=" & ZielvorgangsidInput &
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
        Dim message As String = String.Format("Der Vorgang ist  bereits von AnwenderIn '{0} 'geöffnet. " & Environment.NewLine &
                                           "Dies ist weitgehend unproblematisch." & Environment.NewLine &
                                           "Nur beim ÄNDERN von bereits vorhandenen Dokumenten kann es zu Problemen kommen." & Environment.NewLine &
                                           "Bitte warten Sie mit solchen Änderungen bis die Anwenderin den Fall verläßt, " & Environment.NewLine &
                                           "oder sprechen Sie sich mit Ihr ab.",
                                           lockuser)
        MessageBox.Show(message, "Dieser Vorgang ist bereits von einer anderen Person geöffnet!", MessageBoxButton.OK, MessageBoxImage.Information,
                         MessageBoxResult.OK)
    End Sub

    Public Shared Function vorgangzuDokidFinden(ByVal dokument As Dokument, ByVal dokumentid As Integer) As Integer
        Dim vid As Integer
        If dokument Is Nothing OrElse dokumentid < 1 Then
            Return 0
        End If
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * from dok2vid where dokumentid=" & dokumentid
        nachricht("dokumentschonImVorgangvorhanden sql: " & myGlobalz.sitzung.tempREC.mydb.SQL)
        Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
        If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            nachricht("fehler vorgangzuDokidFinden  nicht gefunden")
            vid = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Vorgangsid")))
            Return vid
        Else
            nachricht("vorgangzuDokidFinden gefunden")
            Return 0
        End If
    End Function

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
            myGlobalz.sitzung.EreignisseRec.mydb.SQL = "select * from ereignis where datum=(select max(datum)   from ereignis  where vorgangsid=" &
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
            nachricht("Fehler in letztesEreignisWurdeGeaendert :" & ex.ToString)
        End Try
    End Function

    Shared Function stellungnahmeWurdeGeaendert() As Boolean
        Dim neutext As String = "", hinweis As String = ""
        myGlobalz.sitzung.EreignisseRec.mydb.SQL = "select * from stammdaten where " &
                     " vorgangsid in " &
                     " (select s.vorgangsid  from ereignis e,stammdaten s " &
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

    Public Shared Sub ErzeugeUnterVerzeichnisse(ByVal AusgabeRootDir As String)
        IO.Directory.CreateDirectory(AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username)
        IO.Directory.CreateDirectory(AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username & "\data")
        IO.Directory.CreateDirectory(AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username & "\images")
    End Sub
    Public Shared Sub MapfileTemplateBearbeiten(ByVal AusgabeRootDir As String)
        Dim KartenMapfileTemplate, kartenmapfile As String
        kartenmapfile = AusgabeRootDir & myGlobalz.sitzung.aktBearbeiter.username & "\raumbezug.map"
        KartenMapfileTemplate = AusgabeRootDir & "vorlagen\raumbezug.map"
        LibGISmapgenerator.tools.makeMapFile(KartenMapfileTemplate, kartenmapfile, myGlobalz.sitzung.aktBearbeiter.username, True, CLstart.mycSimple.enc,
                                             GISusername:=myGlobalz.sitzung.aktBearbeiter.username)
    End Sub

    Public Shared Function GetPdfZielFilename() As String
        Dim pdfZielFilename As String
        pdfZielFilename = CLstart.myc.kartengen.gifKartenDateiFullName '.Replace("Paradigma", "")
        pdfZielFilename = pdfZielFilename.Replace("png", "pdf")
        pdfZielFilename = pdfZielFilename.Replace("_.pdf", ".pdf")
        Return pdfZielFilename
    End Function

    Shared Function getAnzahlDoksproEreignis(vorgangsid As Integer) As DataTable
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from anzahldoksproereignis where vorgangsid=" & vorgangsid 'myGlobalz.sitzung.VorgangsID
            myGlobalz.sitzung.tempREC.getDataDT()
            If myGlobalz.sitzung.tempREC.dt.Rows.Count < 1 Then
                Return Nothing
            Else
                Return myGlobalz.sitzung.tempREC.dt
            End If
        Catch ex As Exception
            nachricht("fehler in getAnzahlDoksproEreignis: " & ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Shared Function initEreigisseDatatable(ByVal hinweis As String, ByVal vid As Integer,
                                                  ereignisdocexpand As Boolean,
                                                  ereignisDokListe As List(Of clsEreignisDok)) As Integer
        Dim erfolg As Boolean
        myGlobalz.sitzung.EreignisseRec.mydb.SQL = "SELECT * FROM ereignis " & " where VorgangsID=" & vid & " order by datum desc"
        hinweis = myGlobalz.sitzung.EreignisseRec.getDataDT()

        If myGlobalz.sitzung.EreignisseRec.mycount < 1 Then
            nachricht("Keine Ereignisse gespeichert c!")
            Return 0
        End If
        Dim anzahlDoks As New DataTable
        Try
            anzahlDoks = detailsTools.getAnzahlDoksproEreignis(myGlobalz.sitzung.aktVorgangsID)
            If anzahlDoks Is Nothing OrElse anzahlDoks.IsNothingOrEmpty Then
            Else
                RBtoolsns.statusSpalteErgaenzenUndFuellen.execute(myGlobalz.sitzung.EreignisseRec.dt, anzahlDoks,
                                                                  "ANZAHL",
                                                                  "ID")
            End If
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))

            If ereignisdocexpand Then
                glob2.initTemprecAusVorgangRecOracle()
                myGlobalz.sitzung.tempREC2.mydb.SQL = "SELECT * FROM vorg2DokEreignis " & " where VorgangsID=" & vid &
                                                                        " and dateinameext is not null"
                hinweis = myGlobalz.sitzung.tempREC2.getDataDT()
            End If
            erfolg = AlleEreignisseAufListeSetzen(myGlobalz.sitzung.EreignisseRec.dt,
                                                ereignisDokListe,
                                                myGlobalz.sitzung.tempREC2.dt,
                                                ereignisdocexpand)
            Return CInt(myGlobalz.sitzung.EreignisseRec.mycount)
        Catch ex As Exception
            nachricht("Fehler in initEreigisseDatatable: " & ex.ToString)
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
            anz = CInt(clsDBtools.fieldvalue(ereig.Item("ANZAHL")))
            Return anz
        Catch ex As Exception
            nachricht("Warnung in GetAnzahlDoksZumEreignis: " & ex.ToString)
            Return 0
        End Try
    End Function

    Private Shared Sub AlleEreignisseAufListeSetzenExtracted(ByVal ereignisDokListe As List(Of clsEreignisDok),
                                                             ByVal dokumenteDatatable As DataTable,
                                                             ByRef aktereig As clsEreignisDok)
        Try
            For Each dok As DataRow In dokumenteDatatable.Rows
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
                Try
                    aktereig.revisionssicher = CInt(clsDBtools.fieldvalue(dok.Item("REVISIONSSICHER")))
                Catch ex As Exception

                End Try

                ' aktereig.Richtung = clsDBtools.fieldvalue(ereig.Item("RICHTUNG"))
                ' aktereig.Richtung = " " 'wird zur farbsteuerung verwendet
                ' aktereig.Art = clsDBtools.fieldvalue(dok.Item("TYP"))
                aktereig.Datum = clsDBtools.fieldvalueDate(dok.Item("FILEDATUM"))
                'dokumentanteil
                aktereig.dok.dokumentPfad = (clsDBtools.fieldvalue(dok.Item("RELATIVPFAD")))
                aktereig.dok.DateinameMitExtension = (clsDBtools.fieldvalue(dok.Item("DATEINAMEEXT")))
                aktereig.dok.Beschreibung = (clsDBtools.fieldvalue(dok.Item("D_BESCHREIBUNG")))
                aktereig.dokBeschreibung = (clsDBtools.fieldvalue(dok.Item("D_BESCHREIBUNG")))
                aktereig.dok.DocID = CInt((clsDBtools.fieldvalue(dok.Item("DOKUMENTID"))))

                aktereig.EreignisDokTyp = 1
                ereignisDokListe.Add(aktereig)
            Next

        Catch ex As Exception
            nachricht("Fehler in AlleEreignisseAufListeSetzenExtracted: " & ex.ToString)
        End Try
    End Sub
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
                End If
            Next
            Return True
        Catch ex As Exception
            nachricht("Fehler in AlleEreignisseAufListeSetzen: " & ex.ToString)
            Return False
        End Try
    End Function
    Private Shared Sub getEreinigsAnteil(ByVal aktereig As clsEreignisDok, ByVal ereig As DataRow)
        Try
            aktereig.Notiz = clsDBtools.fieldvalue(ereig.Item("NOTIZ"))
            aktereig.Beschreibung = clsDBtools.fieldvalue(ereig.Item("BESCHREIBUNG"))
            aktereig.ID = CInt(clsDBtools.fieldvalue(ereig.Item("ID")))
            aktereig.DokumentID = CInt(clsDBtools.fieldvalue(ereig.Item("DOKUMENTID")))
            aktereig.istRTF = CBool(clsDBtools.fieldvalue(ereig.Item("ISTRTF")))
            aktereig.Quelle = clsDBtools.fieldvalue(ereig.Item("QUELLE"))
            aktereig.Richtung = clsDBtools.fieldvalue(ereig.Item("RICHTUNG"))
            aktereig.Art = clsDBtools.fieldvalue(ereig.Item("ART"))
            aktereig.Datum = clsDBtools.fieldvalueDate(ereig.Item("DATUM"))
            aktereig.dokBeschreibung = clsDBtools.fieldvalue(ereig.Item("NOTIZ"))

        Catch ex As Exception
            nachricht("Fehler in getEreinigsAnteil: " & ex.ToString)
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
            dokument.istVeraltet = CBool(item.dok.istVeraltet)
            dokument.ExifDatum = CDate(item.dok.ExifDatum)
            dokument.EXIFlon = CStr(item.dok.EXIFlon)
            dokument.EXIFlat = CStr(item.dok.EXIFlat)
            dokument.EXIFdir = CStr(item.dok.EXIFdir)
            dokument.EXIFhersteller = CStr(item.dok.EXIFhersteller)
            dokument.revisionssicher = CBool(item.dok.revisionssicher)
            dokument.Initiale = CStr(item.dok.Initiale)
            Try
                dokument.istNurVerwandt = True 'CBool(item.dok.status)
            Catch ex As Exception
            End Try
        Catch ex As Exception
            nachricht("DokumentDatarowView2Obj " & vbCrLf & vbCrLf & ex.ToString)
        End Try
    End Sub

    Private Shared Function dokumentIstGeoeffnet(dok As Dokument) As Boolean
        If dok Is Nothing Then Exit Function
        If myGlobalz.sitzung.checkoutDokuList Is Nothing Then Exit Function
        If DokArc.listeEnthaeltDokument(dok, myGlobalz.sitzung.checkoutDokuList) Then
            Return True
        End If
        Return False
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
        Dim result As String = glob2.sendjobExtracted(aufruf, CLstart.mycSimple.enc, 18000000)
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
        Dim protokolldatei As String
        Try
            protokolldatei = CLstart.myc.aLog._logfile
            'DokArc.DokumentStarten(protokolldatei)
            System.Diagnostics.Process.Start(protokolldatei)
        Catch ex As Exception
            nachricht("fehler in vorgangsprotokollanzeigen: " & ex.ToString)
        End Try
    End Sub



End Class
