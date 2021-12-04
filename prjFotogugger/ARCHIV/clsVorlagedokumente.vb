
Imports System.Data

Public Class clsVorlagedokumente
    Public _VorlageDateiImArchiv As IO.FileInfo

    Public Property aktbeteiligter As Person


    Function VorlageBestimmenUndBearbeiten(ByVal insArchiv As Boolean,
                                            ByRef tmresultstring As String,
                                            ByRef outfile As String,
                                            ByVal replaceTextMarkenDict As Dictionary(Of String, String),
                                            ByVal Schlagworte As String,
                                            ByVal quellpfad As String,
                                            ByRef tauschergebnis As String,
                                            dateityp As String,
                                           dateiVorname As String,
                                           dokumentBookmarks As List(Of String)) As Boolean
        Dim erfolg As Boolean
        Dim vorlagedatei As String = ""
        If Not _VorlageDateiImArchiv.FullName.Trim.IsNothingOrEmpty Then
            vorlagedatei = _VorlageDateiImArchiv.FullName

        End If

        If insArchiv Then
            'hier wird das outfile festgelegt
            ' erfolg = DocxVorlageVerwenden(outfile, vorlagedatei, Schlagworte, quellpfad, dateityp, dateiVorname)
            erfolg = True
        Else
            erfolg = nurAnschauenNeu(outfile, vorlagedatei)
        End If
        If erfolg Then
            vorlagedatei = outfile
            tmresultstring = handleTextmarken(outfile, vorlagedatei, replaceTextMarkenDict, tauschergebnis, dokumentBookmarks)
            Return True
        Else
            MsgBox("Das Dokument kann nicht neu erzeugt werden. Vermutlich haben Sie das alte Dokument noch geöffnet! " &
                    Environment.NewLine &
                    "Abhilfe: Schließen Sie das alte Worddokument und versuchen Sie es nochmal!")
            tmresultstring = "kein erfolg"
            outfile$ = ""
            Return False
        End If
    End Function

    Function nurAnschauenNeu(ByRef outdatei As String, ByRef vdatei As String) As Boolean
        Try
            Dim neuername As String = neuenNamenAnschauenBilden()
            myGlobalz.sitzung.aktDokument.DateinameMitExtension = neuername
            myGlobalz.sitzung.aktDokument.FullnameCheckout =
                                         Dokument.VorlageMakeFullname_Checkout(
                                         myGlobalz.sitzung.aktVorgangsID,
                                         True,
                                         myGlobalz.sitzung.aktDokument.DateinameMitExtension,
                                         myGlobalz.Arc.lokalerCheckoutcache)

            If Not tempoDateiLoeschenOk(myGlobalz.sitzung.aktDokument.FullnameCheckout) Then
                MessageBox.Show(glob2.getMsgboxText("nurAnschauen", New List(Of String)(New String() {})))
            End If



            Dim erfolgreich As Boolean = Dokument.VorlagecreateCheckoutDir(myGlobalz.sitzung.aktDokument.FullnameCheckout,
                  myGlobalz.Arc.lokalerCheckoutcache)
            If erfolgreich Then
                _VorlageDateiImArchiv.CopyTo(myGlobalz.sitzung.aktDokument.FullnameCheckout, True)
                vdatei = myGlobalz.sitzung.aktDokument.FullnameCheckout
                outdatei = myGlobalz.sitzung.aktDokument.FullnameCheckout
                Return True
            Else
                nachricht("Fehler: nurAnschauenNeu: createCheckoutDir ergebnis ist nothing: " &
                                   myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID)
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler: ", ex)
            Return False
        End Try
    End Function

    Function handleTextmarken(ByVal vdatei As String,
                                      ByVal outdatei As String,
                                      ByVal replaceTextMarkenDict As Dictionary(Of String, String),
                                      ByRef tauschergebnis As String,
                                        dokumentBookmarks As List(Of String)) As String
        Dim TextMarken$() : ReDim TextMarken(0)
        Dim lw As WordReplaceTextmarken
        Dim result As String = ""
        Try
            nachricht("handleTextmarken vdatei: " & vdatei)
            lw = New WordReplaceTextmarken(vdatei, outdatei, replaceTextMarkenDict)
            'lw.TM_ernteBookmarksAusVorlagenDoc(TextMarken)
            '   result = bookmarkArray2BookmarkListe(replaceTextMarkenDict, TextMarken)
            'If myGlobalz.VorlagenRoot.Contains("hash") Then
            '    wordOpenXML.dokreplaceHash2(tauschergebnis, vdatei)
            'Else
            wordInterop.dokreplace(tauschergebnis, vdatei, outdatei, replaceTextMarkenDict, dokumentBookmarks)
            'End If
            result = result & Environment.NewLine & tauschergebnis
            lw = Nothing
            Return result
        Catch ex As Exception
            nachricht("Fehler in handleTextmarken vdatei: " & vdatei & " /// ", ex)
            Return "Fehler"
        Finally
            lw = Nothing
        End Try
    End Function



    Function bookmarkArray2BookmarkListe(ByVal replaceWordDict As Dictionary(Of String, String), ByVal bookmarkArray() As String) As String
        Using strW As New System.IO.StringWriter()
            'Dim int As Integer = 0
            Dim wert As String = ""
            Dim temp As String = ""
            Dim leerhinweis$ = "------------------ "
            'KEIN WERT VORHANDEN -----------"
            Try
                strW.WriteLine("Verwendete Textmarken ", vbTab)
                strW.WriteLine("Textmarken{0}| Werte ", vbTab)
                strW.WriteLine("_______________________________ ", vbTab)
                For i = 0 To bookmarkArray.GetUpperBound(0)
                    If replaceWordDict.TryGetValue(bookmarkArray(i), temp) Then
                        If String.IsNullOrEmpty(temp) Then
                            wert = leerhinweis
                        Else
                            wert = temp
                        End If
                    Else
                        wert = leerhinweis
                    End If
                    strW.WriteLine("{0}{1}| {2} ", bookmarkArray(i), vbTab, wert)
                Next
                Return strW.ToString
            Catch ex As Exception
                nachricht(ex.ToString)
                Return ex.ToString
            End Try
        End Using
    End Function


    Private Sub vorlagendateiInsLokCheckoutDirKopieren(ByVal vdatei As String, ziel As String)
        Dim tempdatei As IO.FileInfo
        Try
            tempdatei = New IO.FileInfo(vdatei)
            tempdatei.CopyTo(ziel)
        Catch ex As Exception
            nachricht("Fehler in vorlagendateiInsDokumentarchivKopieren: ", ex)
        Finally
            tempdatei = Nothing
        End Try
    End Sub
    Public Shared Function vornameDerZielDateiBilden(ByVal vdatei As String) As String
        Dim vorname As String
        Dim fi As New IO.FileInfo(vdatei)
        vorname = fi.Name
        vorname = vorname.Replace(fi.Extension, "")
        vorname = LIBgemeinsames.clsString.normalize_Filename(vorname)
        fi = Nothing
        Return vorname
    End Function
    Shared Function DocxVorlageVerwenden(ByRef outdatei As String,
                                            ByRef vdatei As String,
                                            ByVal Schlagworte As String,
                                            ByVal quellpfad As String,
                                            dateityp As String,
                                            dateivorname As String) As Boolean
        Dim neuu As New clsVorlagedokumente(vdatei)
        Dim erfolg As Boolean = False
        Dim fi As New IO.FileInfo(vdatei)
        Dim neuername As String = neuu.neuenNamenVerwendenBilden(dateityp, fi.Name.ToLower.Replace(fi.Extension, "")) '".doc") 
        'ereignisid = vorlageEreignisAnlegen(ereignisanlegen, ereignisart, ereignistitel, ereignisid)
        'ins archiv einchecken mit ereignisID
        myGlobalz.sitzung.aktDokument.DateinameMitExtension = neuername
        myGlobalz.sitzung.aktDokument.Filedatum = Now
        myGlobalz.sitzung.aktDokument.Checkindatum = Now
        'myglobalz.sitzung.aktDokument.Beschreibung = Schlagworte & " " &
        '          vorname.Replace("_", " ").Replace("-", " ").Replace(":", " ").Replace(",", " ").Replace(":", " ").Replace("  ", " ")
        myGlobalz.sitzung.aktDokument.Beschreibung = Schlagworte
        myGlobalz.sitzung.aktDokument.newSaveMode = True
        Dim ausgabeVerzeichnis As String = ""
        myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)

        Dokument.createCheckoutDir(myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktVorgangsID)
        If dateityp = ".doc" Or dateityp = ".docx" Then
            neuu._VorlageDateiImArchiv.CopyTo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        Else
            neuu.vorlagendateiInsLokCheckoutDirKopieren(vdatei, myGlobalz.sitzung.aktDokument.FullnameCheckout)
        End If
        outdatei = myGlobalz.sitzung.aktDokument.FullnameCheckout
        Return True
        ''Dim NumDir As String = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
        'erfolg = myGlobalz.Arc.checkINDoku(myGlobalz.sitzung.aktDokument.FullnameCheckout,
        '                             myGlobalz.sitzung.aktDokument.Beschreibung,
        '                             ereignisid,
        '                             False,
        '                             myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
        '                             "",
        '                             myGlobalz.sitzung.aktVorgangsID,
        '                             False, Now, myGlobalz.sitzung.aktDokument.DocID,
        '                             myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
        '                             myGlobalz.sitzung.aktDokument.newSaveMode, False,
        '                             myGlobalz.sitzung.aktDokument.kompressed)
        'If erfolg Then
        '    myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        '    vdatei = myGlobalz.sitzung.aktDokument.FullnameImArchiv
        '    outdatei = myGlobalz.sitzung.aktDokument.FullnameImArchiv
        '    Return True
        'Else
        '    nachricht_und_Mbox("Fehler beim Speichern im Archiv!")
        '    Return False
        'End If
    End Function

    Friend Shared Function vorlageEreignisAnlegen(ereignisanlegen As Boolean, ereignisart As String, ereignistitel As String, ereignisid As Integer) As Integer
        If ereignisanlegen Then
            NeuesEreigniserzeugen(ereignisart, ereignistitel)
            clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID,
                                                           myGlobalz.sitzung.Ereignismodus,
                                                           myGlobalz.sitzung.aktEreignis) '
            If ereignisart.ToLower.StartsWith("stellungnahme") Then
                myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = True
                detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "STELLUNGNAHME")
            End If
            ereignisid = myGlobalz.sitzung.aktEreignis.ID
        Else
            If ereignisid > 0 Then
                nachricht("Das bestehende Ereignis wird verwendet: " & ereignisid)
            Else
                ereignisid = 0
            End If
        End If

        Return ereignisid
    End Function

    Function neuenNamenAnschauenBilden() As String
        Try
            Dim filename As String = "loeschmich.doc"
            filename = LIBgemeinsames.clsString.normalize_Filename(filename)
            Return filename
        Catch ex As Exception
            nachricht("FEHLER: Vorlage: neuenNamenBilden: ", ex)
            Return ex.ToString
        End Try
    End Function

    ''' <summary>
    ''' Nimmt DateinamensStamm und fügt timestamp an
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function neuenNamenVerwendenBilden(ByVal endungMitPunkt As String, dateivorname As String) As String
        Try
            Dim filename As String, a As String()
            a = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Split("-"c)
            If dateivorname.IsNothingOrEmpty Then
                filename = zusammensetzenFilename(a)
            Else
                filename = dateivorname & "_" & myGlobalz.sitzung.aktVorgangsID '& "_" & zusammensetzenFilename(a)
            End If
            filename = filename & "_" & Now.ToString("yyyy-MM-dd_HHmmss") & endungMitPunkt
            filename = filename
            'filename = vorname & "_" & filename alteversi9on
            filename = LIBgemeinsames.clsString.normalize_Filename(filename)
            Return filename
        Catch ex As Exception
            nachricht("FEHLER: Vorlage: neuenNamenBilden: ", ex)
            Return ex.ToString
        End Try
    End Function

    Private Shared Function zusammensetzenFilename(ByVal a$()) As String
        Dim filename As String = ""
        Try
            If a Is Nothing Then Return ""
            If a.Length < 3 Then Return ""
            If a.Length = 3 Then filename = a(2) & "-" & a(3) '& "-" & a(3) '& "_" & a(4)
            If a.Length > 3 Then filename = a(2) & "-" & a(3) & "-" & a(4) '& "_" & a(4)
            Return filename
        Catch ex As Exception
            nachricht("Fehler in zusammensetzenFilename: ", ex)
            Return "fehlerzusammensetzenFilename"
        End Try
    End Function

    Sub New(ByVal dateiname As String)
        _VorlageDateiImArchiv = New IO.FileInfo(dateiname)
    End Sub

    Private Shared Sub NeuesEreigniserzeugen(ByVal ereignisart As String, ByVal ereignistitel As String)
        myGlobalz.sitzung.aktEreignis.clearValues()
        myGlobalz.sitzung.Ereignismodus = "neu"
        With myGlobalz.sitzung
            '  Dim erledigttext$ = ""
            .aktEreignis.Datum = Now
            .aktEreignis.Art = ereignisart '"Brief"
            .aktEreignis.Richtung = "Ausgang"
            .aktEreignis.Beschreibung = ereignistitel
            .aktEreignis.Quelle = myGlobalz.sitzung.aktBearbeiter.getInitial
        End With
    End Sub


    Private Shared Sub TM_AnredeAktPerson(ByRef replaceWordDict As Dictionary(Of String, String), documentbookmarks As List(Of String))
        Try
            Dim anrede As String = myGlobalz.sitzung.aktPerson.Anrede
            If anrede Is Nothing Then
                anrede = ""
                myGlobalz.sitzung.aktPerson.Anrede = anrede
                replaceWordDict.Add("Anrede1", "geehrte")
                replaceWordDict.Add("Anrede2", "Damen und Herren")
                Exit Sub
            End If
            If anrede.ToLower.Trim = "frau" Then
                replaceWordDict.Add("Anrede", "Frau")
                replaceWordDict.Add("Anrede1", "geehrte")
                replaceWordDict.Add("Anrede2", myGlobalz.sitzung.aktPerson.Anrede)
                laufendenNRInTextMarkenListe(replaceWordDict, "Frau", "Anrede", documentbookmarks)
                Exit Sub
            End If

            If anrede.ToLower.Trim = "herr" Then
                replaceWordDict.Add("Anrede", "Herr")
                replaceWordDict.Add("Anrede1", "geehrter")
                replaceWordDict.Add("Anrede2", "Herr")
                laufendenNRInTextMarkenListe(replaceWordDict, "Herr", "Anrede", documentbookmarks)
                Exit Sub
            End If


            If anrede.ToLower.Trim = "firma" Then
                replaceWordDict.Add("Anrede", "Firma")
                replaceWordDict.Add("Anrede1", "geehrte")
                replaceWordDict.Add("Anrede2", "Damen und Herren")
                laufendenNRInTextMarkenListe(replaceWordDict, "Firma", "Anrede", documentbookmarks)
                Exit Sub
            End If

            If anrede.ToLower.Trim = "eheleute" Then
                replaceWordDict.Add("Anrede", "Eheleute")
                replaceWordDict.Add("Anrede1", "geehrte")
                replaceWordDict.Add("Anrede2", "Eheleute")
                laufendenNRInTextMarkenListe(replaceWordDict, "Eheleute", "Anrede", documentbookmarks)
                Exit Sub
            End If

            If String.IsNullOrEmpty(anrede) Then
                replaceWordDict.Add("Anrede1", "geehrte")
                replaceWordDict.Add("Anrede2", "Damen und Herren")
                Exit Sub
            End If

        Catch ex As Exception
            nachricht("Fehler in Anrede: " & myGlobalz.sitzung.aktPerson.Anrede & vbCrLf, ex)
            replaceWordDict.Add("Anrede1", "geehrte")
            replaceWordDict.Add("Anrede2", "Damen und Herren")
        End Try
    End Sub

    Private Shared Sub TM_Sachbearbeiter(ByRef replaceWordDict As Dictionary(Of String, String), ByRef raumnrDesHauptsachbearbeiters As String,
                                         documentbookmarks As List(Of String))
        Dim sachbearbeiter As String = ""
        Dim rolle As String = ""
        getSachbearbeiter(sachbearbeiter, rolle, raumnrDesHauptsachbearbeiters)
        tauscheOderStandard(replaceWordDict, "Rolle", rolle, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Sachbearbeiter", sachbearbeiter, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Sachbearbeiter2", sachbearbeiter, documentbookmarks)

        'replaceWordDict.Add("Sachbearbeiter", sachbearbeiter$)
        'replaceWordDict.Add("Sachbearbeiter2", sachbearbeiter$)
    End Sub
    Private Shared Sub TM_Sachbearbeiterkurz(ByRef replaceWordDict As Dictionary(Of String, String), ByRef raumnrDesHauptsachbearbeiters As String, documentbookmarks As List(Of String))
        Dim sachbearbeiter As String = ""
        Dim rolle As String = ""
        getSachbearbeiter(sachbearbeiter, rolle, raumnrDesHauptsachbearbeiters)
        tauscheOderStandard(replaceWordDict, "Rolle", rolle, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Sachbearbeiterkurz", sachbearbeiter, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Sachbearbeiter2kurz", sachbearbeiter, documentbookmarks)

        'replaceWordDict.Add("Sachbearbeiter", sachbearbeiter$)
        'replaceWordDict.Add("Sachbearbeiter2", sachbearbeiter$)
    End Sub
    Shared Sub TM_fuelleMarkenDictionary(ByRef replaceWordDict As Dictionary(Of String, String), ByVal flurstueckstext As String, documentbookmarks As List(Of String))
        Dim temp As String
        Dim raumnrDesHauptsachbearbeiters As String = ""
        Try

            IllgealgeHuettenMarken(replaceWordDict, myGlobalz.sitzung.aktVorgangsID, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Anschrift", clsBeteiligteBUSI.Anschrift_Text_erzeugen(myGlobalz.sitzung.aktPerson), documentbookmarks)

            TM_Sachbearbeiter(replaceWordDict, raumnrDesHauptsachbearbeiters, documentbookmarks)
            TM_Sachbearbeiterkurz(replaceWordDict, raumnrDesHauptsachbearbeiters, documentbookmarks)

            tauscheOderStandard(replaceWordDict, "SBAnrede", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Anrede, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "SBNachname", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Name, documentbookmarks)

            tauscheOderStandard(replaceWordDict, "Durchwahl", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kontakt.elektr.Telefon1, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Email", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kontakt.elektr.Email, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Fax", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kontakt.elektr.Fax1, documentbookmarks)

            tauscheOderStandard(replaceWordDict, "Gemarkungskuerzel", myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ, documentbookmarks)

            tauscheOderStandard(replaceWordDict, "Aktenzeichen", getAktenzeichen, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Az", getAktenzeichen, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Aktenzeichen1", getAktenzeichenOhneSachbearbeiter, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "AktenzeichenOhnePros", getAktenzeichenOhneProsa, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "AzOhnePros", getAktenzeichenOhneProsa, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Prosa", getAktenzeichenNurProsa, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Azkurz", getAktenzeichenKurz, documentbookmarks)
            replaceWordDict.Add("Aktenzeichen2", getAktenzeichenOhneSachbearbeiter)
            'B-Plan
            If Not String.IsNullOrEmpty(CLstart.myc.aktprojekt.BezeichnungKurz) Then tauscheOderStandard(replaceWordDict, "PlanNr", CLstart.myc.aktprojekt.BezeichnungKurz.ToString.Trim, documentbookmarks)
            If Not String.IsNullOrEmpty(CLstart.myc.aktprojekt.BezeichnungLang) Then tauscheOderStandard(replaceWordDict, "PlanTitel", CLstart.myc.aktprojekt.BezeichnungLang.ToString.Trim, documentbookmarks)
            If Not String.IsNullOrEmpty(CLstart.myc.aktprojekt.Gemeinde) Then tauscheOderStandard(replaceWordDict, "PlanGemeinde", CLstart.myc.aktprojekt.Gemeinde.ToString.Trim, documentbookmarks)
            If Not String.IsNullOrEmpty(CStr(CLstart.myc.aktprojekt.bis)) Then tauscheOderStandard(replaceWordDict, "PlanFrist", Format(CLstart.myc.aktprojekt.bis, "dd.MM.yyyy"), documentbookmarks) '  CLstart.myc.aktprojekt.bis.ToString.Trim)
            'datum und abteilung
            tauscheOderStandard(replaceWordDict, "Datum", Format(DateTime.Now(), "dd.MM.yyyy"), documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Abteilung", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Bemerkung, documentbookmarks)


            tauscheOderStandard(replaceWordDict, "InterneNr", myGlobalz.sitzung.aktVorgang.Stammdaten.InterneNr.ToString, documentbookmarks)

            TM_AnredeAktPerson(replaceWordDict, documentbookmarks)
            'tauscheOderStandard(replaceWordDict, "Anrede", myGlobalz.sitzung.aktPerson.Name.Trim, documentbookmarks)

            If myGlobalz.sitzung.aktPerson.Anrede.ToLower.Trim = "firma" Then
                replaceWordDict.Add("Nachname", "")
            Else
                tauscheOderStandard(replaceWordDict, "Nachname", myGlobalz.sitzung.aktPerson.Name.Trim, documentbookmarks)
            End If
            If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Kassenkonto.ToLower.Trim) Then
                tauscheOderStandard(replaceWordDict, "Kassenkonto", myGlobalz.sitzung.aktPerson.Kassenkonto.Trim, documentbookmarks)
            Else
                tauscheOderStandard(replaceWordDict, "Kassenkonto", myGlobalz.sitzung.aktPerson.Kassenkonto.Trim, documentbookmarks)
            End If
            tauscheOderStandard(replaceWordDict, "Funktion", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Rolle, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Sachgebiet", getSachgebietstext_erweitert, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Standort", makeStandortAkte(), documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Raumnr", raumnrDesHauptsachbearbeiters, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Probaugaz", myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Liegenschaft", myGlobalz.sitzung.textmarkeLiegenschaft, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "PLZ", myGlobalz.sitzung.aktADR.PLZ, documentbookmarks)
            Dim organisation As String = "", zusatz As String = "", bezirk As String = ""
            Dim bzm As String = SpezialPersonBilden("Schornsteinfeger/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Bezirk", bezirk, documentbookmarks)

            'Dim task As System.Threading.Tasks.Task(Of String) = bzmBilden("Schornsteinfeger/in") ', organisation, zusatz$, bezirk)
            'Dim result As String = Await task
            Dim bzmName As String = ""

            bzmName = bzmBilden("Schornsteinfeger/in")
            getbzmName(organisation, zusatz, bzmName, bzmName)

            tauscheOderStandard(replaceWordDict, "bzmName", bzmName, documentbookmarks)
            If Not String.IsNullOrEmpty(organisation) Then
                tauscheOderStandard(replaceWordDict, "Organisation", organisation, documentbookmarks)
                Dim vd As String
                vd = organisation & ", vertr. d. " & myGlobalz.sitzung.aktPerson.Anrede & " "
                tauscheOderStandard(replaceWordDict, "OrganisationVD", vd, documentbookmarks)
            End If


            If Not String.IsNullOrEmpty(zusatz) Then tauscheOderStandard(replaceWordDict, "Zusatz", zusatz, documentbookmarks)


            tauscheOderStandard(replaceWordDict, "Bzm", bzm, documentbookmarks)

            temp$ = SpezialPersonBilden("Gemeindevertreter/in", organisation, zusatz, bezirk)
            If Not String.IsNullOrEmpty(organisation) Then tauscheOderStandard(replaceWordDict, "Organisation", organisation, documentbookmarks)
            If Not String.IsNullOrEmpty(zusatz) Then tauscheOderStandard(replaceWordDict, "Zusatz", zusatz, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Gemeindevertreter", temp, documentbookmarks)

            temp$ = SpezialPersonBilden("Gemeindevertreter/in", organisation, zusatz, False, False, True, True, False)
            tauscheOderStandard(replaceWordDict, "OOGemeindevertreterOhneTE", temp, documentbookmarks)

            temp$ = SpezialPersonBilden("Beschwerdeführer/in", organisation, zusatz, False, False, False, False, True)
            tauscheOderStandard(replaceWordDict, "BeschwerdefuehrerOhneTE", temp, documentbookmarks)


            '####
            temp$ = SpezialPersonBilden("Beschwerdeführer/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Beschwerdefuehrer", temp, documentbookmarks)

            temp$ = SpezialPersonBilden("Verursacher/in", organisation, zusatz, False, False, False, False, True)
            tauscheOderStandard(replaceWordDict, "VerursacherOhneTE", temp, documentbookmarks)

            temp$ = SpezialPersonBilden("Auftragnehmer/in", organisation, zusatz, False, False, True, True, False)
            tauscheOderStandard(replaceWordDict, "OOAuftragnehmerOhneTE", temp, documentbookmarks)

            'Erweiterungen fürs deckblatt
            tauscheOderStandard(replaceWordDict, "Eingang", Format(myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum, "dd.MM.yyyy"), documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Vorgang", myGlobalz.sitzung.aktVorgangsID.ToString, documentbookmarks)
            '####
            temp$ = SpezialPersonBilden("Auftragnehmer/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Auftragnehmer", temp, documentbookmarks)
            '####
            temp$ = SpezialPersonBilden("Eigentümer/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Eigentuemer", temp, documentbookmarks)

            '####
            temp$ = SpezialPersonBilden("Antragsteller/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Antragsteller", temp, documentbookmarks)


            '####
            temp$ = SpezialPersonBilden("Auftragnehmer/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Auftragnehmer", temp, documentbookmarks)

            '####
            temp$ = SpezialPersonBilden("Betreiber/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Betreiber", temp, documentbookmarks)

            '####
            temp$ = SpezialPersonBilden("Verursacher/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Verursacher", temp, documentbookmarks)

            '####
            'temp$ = SpezialPersonBilden("Verursacher/in", organisation, zusatz, bezirk, True)

            temp$ = SpezialPersonBilden("Verursacher/in", organisation, zusatz, False, False, True, True, True)
            temp = temp.Replace(",", "").Trim
            tauscheOderStandard(replaceWordDict, "Verursacher1", temp, documentbookmarks)

            '####
            temp$ = SpezialPersonBilden("Antragsteller/in", organisation, zusatz, False, False, False, False, False)
            tauscheOderStandard(replaceWordDict, "AntragstellerOhneTE", temp, documentbookmarks)

            '####
            temp$ = SpezialPersonBilden("Antragsteller/in", organisation, zusatz, False, False, True, True, False)
            tauscheOderStandard(replaceWordDict, "OOAntragstellerOhneTE", temp, documentbookmarks)

            If Not String.IsNullOrEmpty(organisation) Then tauscheOderStandard(replaceWordDict, "Organisation", organisation, documentbookmarks)
            If Not String.IsNullOrEmpty(zusatz$) Then tauscheOderStandard(replaceWordDict, "Zusatz", zusatz, documentbookmarks)
            '--------------------
            temp$ = SpezialPersonBilden("Verursacher/in", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "Verursacher", temp, documentbookmarks)

            temp$ = SpezialPersonBilden("Verursacher/in", organisation, zusatz, bezirk)
            temp = temp.Replace(",", "").Trim
            tauscheOderStandard(replaceWordDict, "Verursacher1", temp, documentbookmarks)

            Dim BAname As String = SpezialPersonBilden("Bauaufsicht", organisation, zusatz, bezirk)
            tauscheOderStandard(replaceWordDict, "BauaufsichtName", BAname, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "BAName", BAname, documentbookmarks)
            '    If Not String.IsNullOrEmpty(organisation) Then tauscheOderStandard(replaceWordDict, "Organisation", organisation)
            If Not String.IsNullOrEmpty(zusatz) Then tauscheOderStandard(replaceWordDict, "Zusatz", zusatz, documentbookmarks)
            '

            '     tauscheOderStandard(replaceWordDict, "Antragsteller", temp)
            tauscheOderStandard(replaceWordDict, "Vorhaben", myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung, documentbookmarks)
            tauscheOderStandard(replaceWordDict, "Titel", myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung, documentbookmarks)

            holeRBfelder(replaceWordDict, documentbookmarks)
        Catch ex As Exception
            nachricht("Fehler: fillWordDirectory: ", ex)
        End Try
    End Sub

    Private Shared Sub getbzmName(ByRef organisation As String, ByRef zusatz As String, result As String, ByRef bzmName As String)
        Dim a() As String
        Try
            l(" MOD getbzmName anfang")
            a = result.Trim.Split("#"c)
            bzmName = a(0)
            organisation = a(1)
            zusatz = a(2)
            l(" MOD getbzmName ende")
        Catch ex As Exception
            l("Fehler in getbzmName: ", ex)
        End Try
    End Sub

    Private Shared Sub holeRBfelder(ByRef replaceWordDict As Dictionary(Of String, String), documentbookmarks As List(Of String))
        '  Dim erfolg As Boolean = False
        Dim gemeindename As String = "", gemarkungsname As String = "", gemarkungsAbstract As String = "", Flurstuecksliste As String = ""
        calcGemeindename(gemeindename)
        gemarkungsname = calcGemarkungsname(gemeindename)
        gemarkungsAbstract = calcGemarkungsAbstract()
        Flurstuecksliste = bildeFlurstuecksliste(myGlobalz.sitzung.aktVorgangsID)

        tauscheOderStandard(replaceWordDict, "FLISTE", Flurstuecksliste, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Stadt", gemeindename, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Gemeinde", gemeindename, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Strasse", myGlobalz.sitzung.aktADR.Gisadresse.strasseName.Trim & " " & myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.Trim, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Straße", myGlobalz.sitzung.aktADR.Gisadresse.strasseName.Trim & " " & myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.Trim, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Hausnr", myGlobalz.sitzung.aktADR.Gisadresse.HausKombi, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Flur", myGlobalz.sitzung.aktFST.normflst.flur.ToString, documentbookmarks)


        'Dim test = myGlobalz.sitzung.textmarkeLiegenschaft
        'tauscheOderStandard(replaceWordDict, "Flurstück", myGlobalz.sitzung.textmarkeLiegenschaft.Trim)
        tauscheOderStandard(replaceWordDict, "Flurstück", myGlobalz.sitzung.aktFST.normflst.fstueckKombi.ToString, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Gemarkung", gemarkungsname, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "GemarkungsText", gemarkungsAbstract, documentbookmarks)
    End Sub

    Private Shared Function calcGemarkungsAbstract() As String
        Dim gemarkungsAbstract As String
        gemarkungsAbstract = myGlobalz.sitzung.aktFST.normflst.gemarkungstext & ", Flur: " & myGlobalz.sitzung.aktFST.normflst.flur &
                          ", Flurstück: " & myGlobalz.sitzung.aktFST.normflst.fstueckKombi
        Return gemarkungsAbstract
    End Function

    Private Shared Function calcGemarkungsname(ByVal gemeindename As String) As String
        Dim gemarkungsname As String
        gemarkungsname = myGlobalz.sitzung.aktFST.normflst.gemarkungstext
        If String.IsNullOrEmpty(gemarkungsname) Then
            Dim result As String = clsVorlagenTools.getGemarkungstextFromGEMKRZ(myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)
            If String.IsNullOrEmpty(result) Then
                gemarkungsname = gemeindename
            Else
                gemarkungsname = result
            End If
        End If
        Return gemarkungsname
    End Function

    Private Shared Sub calcGemeindename(ByRef gemeindename As String)
        gemeindename = myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName
        If String.IsNullOrEmpty(gemeindename) Then
            gemeindename = myGlobalz.sitzung.aktFST.normflst.gemeindename
        End If

        If String.IsNullOrEmpty(gemeindename) Then
            Dim result As String = clsVorlagenTools.getGemeindetextFromGEMKRZ(myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)
            If String.IsNullOrEmpty(result) Then
                gemeindename = ""
            Else
                gemeindename = result
            End If
        End If
    End Sub
    Private Shared Sub laufendenNRInTextMarkenListe(ByRef d As Dictionary(Of String, String), ByVal wert As String, ByVal template As String, documentbookmarks As List(Of String))
        For i = 0 To 10
            If template.ToLower.StartsWith("Anrede") Then
                Debug.Print("")
            End If
            If documentbookmarks.Contains(template & "_" & i) Then
                d.Add(template & "_" & i, wert)
            End If
        Next
    End Sub

    Shared Sub tauscheOderStandarddefekt(ByRef d As Dictionary(Of String, String), ByVal template As String, ByVal wert As String, documentbookmarks As List(Of String))
        Dim standard$ = " "
        Try
            'Dim www As New wordInterop
            'www._dokumentbookmarks = documentbookmarks
            '    If www.isInVorlageDokumentVorhanden(template) Then

            If String.IsNullOrEmpty(wert) Then wert = standard
            If Not d.ContainsKey(template) Then
                d.Add(template, wert)
                laufendenNRInTextMarkenListe(d, wert, template, documentbookmarks)
            Else
                nachricht("2 warnung abgefangen in tauscheOderStandard: key ist schon vorhanden: " & template & " / " & wert)
            End If            '  End If
            'www = Nothing
        Catch ex As Exception
            nachricht("FEhler beim tauscheOderStandard: ", ex)
        End Try
    End Sub


    Shared Sub tauscheOderStandard(ByRef d As Dictionary(Of String, String), ByVal template As String, ByVal wert As String, documentbookmarks As List(Of String))
        Dim standard$ = " "
        Try
            If String.IsNullOrEmpty(wert) Then wert = standard
            If Not d.ContainsKey(template) Then
                d.Add(template, wert)
                laufendenNRInTextMarkenListe(d, wert, template, documentbookmarks)
            Else
                nachricht("2 warnung abgefangen in tauscheOderStandard: key ist schon vorhanden: " & template & " / " & wert)
            End If
        Catch ex As Exception
            nachricht("FEhler beim tauscheOderStandard: " & ex.ToString)
        End Try
    End Sub

    Private Shared Sub NamenBildenFuerAnschrift(ByVal ap As Person, ByVal sb As Text.StringBuilder)
        If Not String.IsNullOrEmpty(ap.Namenszusatz.Trim) Then sb.Append(ap.Namenszusatz.Trim & " ")
        If Not String.IsNullOrEmpty(ap.Vorname.Trim) Then sb.Append(ap.Vorname.Trim & " ")
        If Not String.IsNullOrEmpty(ap.Name.Trim) Then sb.Append(ap.Name.Trim & ", " & vbCrLf)
    End Sub

    Public Shared Function Anschrift_BZM_erzeugen(ByVal ap As Person,
                                                  ByVal mitTelnr As Boolean,
                                                  ByVal mitEmail As Boolean) As String 'myGlobalz.sitzung.aktPerson
        Try
            Dim sb As New Text.StringBuilder
            NamenBildenFuerAnschrift(ap, sb)
            If Not String.IsNullOrEmpty(ap.VERTRETENDURCH.Trim) Then sb.Append(" " & ap.VERTRETENDURCH.Trim & ", ")

            If Not String.IsNullOrEmpty(ap.Kontakt.Anschrift.Strasse.Trim) Then sb.Append(ap.Kontakt.Anschrift.Strasse.Trim & " ")
            If Not String.IsNullOrEmpty(ap.Kontakt.Anschrift.Hausnr.Trim) Then
                sb.Append(ap.Kontakt.Anschrift.Hausnr.Trim & ", " & vbCrLf)
            Else
                sb.Append(vbCrLf)
            End If
            If Not String.IsNullOrEmpty(ap.Kontakt.Anschrift.Postfach.Trim) Then sb.Append("Postfach " & (ap.Kontakt.Anschrift.Postfach.Trim & vbCrLf))
            If Not String.IsNullOrEmpty(ap.Kontakt.Anschrift.PostfachPLZ.Trim) Then sb.Append("PostfachPLZ " & (ap.Kontakt.Anschrift.PostfachPLZ.Trim & vbCrLf))
            If Not String.IsNullOrEmpty(CStr(ap.Kontakt.Anschrift.PLZ.ToString.Trim)) Then sb.Append(ap.Kontakt.Anschrift.PLZ.ToString & " ")
            If Not String.IsNullOrEmpty(ap.Kontakt.Anschrift.Gemeindename.Trim) Then sb.Append(ap.Kontakt.Anschrift.Gemeindename.Trim & " " & vbCrLf)
            If Not String.IsNullOrEmpty(ap.Kontakt.elektr.Telefon1.Trim) Then
                If mitTelnr Then sb.Append("Tel.: " & ap.Kontakt.elektr.Telefon1.Trim & ", " & vbCrLf)
            End If

            If Not String.IsNullOrEmpty(ap.Kontakt.elektr.Email.Trim) Then
                If mitEmail Then sb.Append("E-Mail.: " & ap.Kontakt.elektr.Email.Trim)
            End If

            sb.Append("")
            sb.Append("")
            Dim a = sb.ToString.Trim.Replace("  ", " ").Replace(",,", ",")
            'If a.EndsWith(",") Then
            '    a = a.Substring(0, a.Length - 2)
            'End If
            'a = LIBgemeinsames.clsString.
            Return a.Trim
        Catch ex As Exception
            nachricht("Fehler: Anschrift_Text_erzeugen: ", ex)
            Return " -------------------- "
        End Try
    End Function

    Private Shared Sub OrganisationEinbinden(ByRef organisation As String, ByRef orgzusatz As String, ByVal spezialPerson1 As Person)
        Try
            If Not String.IsNullOrEmpty(spezialPerson1.Kontakt.Org.Name.Trim) Then organisation = spezialPerson1.Kontakt.Org.Name.Trim
            If Not String.IsNullOrEmpty(spezialPerson1.Kontakt.Org.Zusatz.Trim) Then orgzusatz = spezialPerson1.Kontakt.Org.Zusatz.Trim
        Catch ex As Exception
            nachricht("Fehler: SpezialPersonBildenExtracted: ", ex)
        End Try
    End Sub

    'Private Overloads Shared Async Function bzmBilden(ByVal personenTyp As String, ByRef organisation As String, ByRef orgzusatz As String, ByRef bezirk As String) As System.Threading.Tasks.Task(Of String)
    Private Overloads Shared Function bzmBilden(ByVal personenTyp As String) As String
        Dim spezialPerson1 As New Person
        Dim organisation, orgzusatz As String
        spezialPerson1.Kontakt = New Kontaktdaten
        'spezialPerson1 = holeSpezialPerson(personenTyp)
        spezialPerson1 = holeSpezialPerson(personenTyp)
        Dim result$ = Anschrift_BZM_erzeugen(spezialPerson1, True, True)

        OrganisationEinbinden(organisation, orgzusatz, spezialPerson1)

        result = result.Replace(vbCrLf, " ")
        LIBgemeinsames.clsString.leerzeichenRaus(result)
        result = spezialPerson1.Name & "#" & organisation & "#" & orgzusatz 'bzmname
        'If personenTyp = "Schornsteinfeger/in" Then
        '    result = spezialPerson1.Name
        '    Return result
        'End If
        'If Not String.IsNullOrEmpty(spezialPerson1.Bezirk) Then
        '    bezirk = spezialPerson1.Bezirk
        'End If
        LIBgemeinsames.clsString.leerzeichenRaus(result)
        spezialPerson1 = Nothing
        Return result
    End Function

    Public Overloads Shared Function SpezialPersonBilden(ByVal personenTyp As String,
                                                         ByRef organisation As String,
                                                         ByRef orgzusatz As String,
                                                         ByRef bezirk As String) As String
        Dim spezialPerson1 As New Person
        spezialPerson1.Kontakt = New Kontaktdaten
        spezialPerson1 = holeSpezialPerson(personenTyp)
        Dim result$ = Anschrift_BZM_erzeugen(spezialPerson1, True, True)
        OrganisationEinbinden(organisation, orgzusatz, spezialPerson1)


        result = result.Replace(vbCrLf, " ") : LIBgemeinsames.clsString.leerzeichenRaus(result)

        If personenTyp = "Bauaufsicht" Then
            result = spezialPerson1.Anrede & " " & spezialPerson1.Name
            Return result
        End If
        'If personenTyp = "Schornsteinfeger/in" Then
        '    result = spezialPerson1.Name
        '    Return result
        'End If
        If Not String.IsNullOrEmpty(spezialPerson1.Bezirk) Then
            bezirk = spezialPerson1.Bezirk
        End If
        result = result.Replace(vbCrLf, " ") : LIBgemeinsames.clsString.leerzeichenRaus(result)
        spezialPerson1 = Nothing
        Return result
    End Function

    Private Overloads Shared Function SpezialPersonBilden(ByVal personenTyp As String, ByRef organisation As String, ByRef orgzusatz As String,
                                                          ByVal mitEmail As Boolean,
                                                          ByVal mitTelnr As Boolean,
                                                          ByVal mitOrgname As Boolean,
                                                          ByVal mitOrgzusatz As Boolean,
                                                          mitZeilenTrennung As Boolean) As String
        Dim spezialPerson1 As New Person
        spezialPerson1.Kontakt = New Kontaktdaten
        spezialPerson1 = holeSpezialPerson(personenTyp)
        Dim result As String = Anschrift_BZM_erzeugen(spezialPerson1, mitTelnr, mitEmail)

        OrganisationEinbinden(organisation, orgzusatz, spezialPerson1)

        Dim orgresult As String
        If mitOrgname Then
            If mitOrgzusatz Then
                orgresult$ = organisation & " " & orgzusatz & ", "
            Else
                orgresult$ = organisation & ", "
            End If
        Else
            orgresult = ""
        End If
        result = orgresult & result
        If Not mitZeilenTrennung Then
            result = result.Replace(vbCrLf, " ")
        End If
        LIBgemeinsames.clsString.leerzeichenRaus(result)
        If personenTyp = "Bauaufsicht" Then
            result = spezialPerson1.Anrede & " " & spezialPerson1.Name
            Return result
        End If
        LIBgemeinsames.clsString.leerzeichenRaus(result)
        spezialPerson1 = Nothing
        Return result
    End Function



    'Shared Function holeSpezialPerson(ByVal Rolle As String) As System.Threading.Tasks.Task(Of Person)
    Shared Function holeSpezialPerson(ByVal Rolle As String) As Person
        Dim tmpPers As New Person
        Dim lokliste As New List(Of Person)
        Try
            clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(myGlobalz.sitzung.aktVorgangsID)
            myGlobalz.sitzung.beteiligteREC.dt.Copy()
            lokliste = clsBeteiligteBUSI.ConvertDatatable2Personenliste(myGlobalz.sitzung.beteiligteREC.dt)
            For Each item As Person In lokliste
                If item.Rolle.ToString.ToLower = Rolle.ToLower Then
                    Return item
                End If
            Next
            Return tmpPers
        Catch ex As Exception
            nachricht("holeSpezialPerson: ", ex)
            Return Nothing
        End Try
    End Function

    Private Shared Function getAktenzeichen() As String
        Dim az$ = ""
        Try
            Return myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt

        Catch ex As Exception
            nachricht("getAktenzeichen: ", ex)
            Return "1Fehler- an admin wenden"
        End Try
    End Function

    Public Shared Function getAktenzeichenOhneProsa() As String
        Dim az$ = ""
        Try
            Dim eins$ = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt
            Dim a$() = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Split("-"c)
            Dim summe As String = ""
            For i = 0 To 5
                summe = summe & a$(i) & "-"
            Next
            summe = summe.Substring(0, summe.Length - 1)
            Return summe

        Catch ex As Exception
            nachricht("getAktenzeichen: ", ex)
            Return "2Fehler- an admin wenden"
        End Try
    End Function
    Private Shared Function getAktenzeichenKurz() As String
        Dim az$ = ""
        Try
            Dim eins$ = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt
            Dim a$() = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Split("-"c)
            Dim summe As String = ""

            If a.Length > 3 Then
                For i = 2 To 4
                    summe = summe & a$(i) & "-"
                Next
            End If
            summe = summe.Substring(0, summe.Length - 1)
            Return summe
        Catch ex As Exception
            nachricht("getAktenzeichen: ", ex)
            Return "3Fehler- an admin wenden"
        End Try
    End Function
    Private Shared Function getAktenzeichenNurProsa() As String
        Dim az$ = ""
        Dim summe As String = ""
        Try
            Dim eins$ = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt
            Dim a$() = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Split("-"c)

            If a.Length > 6 Then
                For i = 6 To a.Length - 1
                    summe = summe & a$(i) & "-"
                Next
                summe = summe.Substring(0, summe.Length - 1)
            Else
                summe = ""
            End If
            Return summe
        Catch ex As Exception
            nachricht("4Fehler- an admin wenden  getAktenzeichen: ", ex)
            Return "4Fehler- an admin wenden"
        End Try
    End Function

    Private Shared Function getAktenzeichenOhneSachbearbeiter() As String
        Dim az$ = ""
        Try
            Return myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt

        Catch ex As Exception
            nachricht("getAktenzeichen: ", ex)
            Return "5Fehler- an admin wenden"
        End Try
    End Function


    Shared Sub detailsDesBearbeitersHolen(ByVal user As clsBearbeiter)
        'Diese Funktion sollte auf Linq umgestellt werden
        l("user: " & user.tostring)
        If Not userTools.initBearbeiterByUserid_ausParadigmadb(user) Then

        End If
    End Sub

    Private Shared Sub getSachbearbeiter(ByRef sachbearbeiter As String, ByRef rolle As String, ByRef raumnrDesHauptsachbearbeiters As String)
        l("getSachbearbeiter              ")
        Dim vorname As String
        Try
            vorname = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Vorname.Trim.Substring(0, 1) & "."
            l("myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter: " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.tostring)
            detailsDesBearbeitersHolen(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter)
            Dim zusatz As String = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Namenszusatz.Trim & " "
            If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Namenszusatz.Trim) Then zusatz = ""
            raumnrDesHauptsachbearbeiters = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Raum
            sachbearbeiter = zusatz &
                vorname & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Name.Trim

            sachbearbeiter = zusatz & vorname & " " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Name.Trim
            If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Rolle) Then
                rolle = "Technische/r Angestellte/r"
            Else
                rolle = (myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Rolle)
            End If
        Catch ex As Exception
            nachricht("fehler in getSachbearbeiter: ", ex)
        End Try
    End Sub


    ''' <summary>
    ''' gibt ALLE textmaklken aus, also auch die mit index
    ''' </summary>
    ''' <param name="replaceTextMarkenDict"></param>
    ''' <remarks></remarks>
    Shared Sub TM_ausgebenMarkenDictionary(ByVal replaceTextMarkenDict As Dictionary(Of String, String))
        Try
            For Each ttt In replaceTextMarkenDict
                '  changeBookmark(ttt.Key, ttt.Value, doc)
                '  nachricht(String.Format("TM_ausgebenMarkenDictionary Tausche:{0} | {1}", ttt.Key, ttt.Value))
            Next
        Catch ex As Exception
            '   nachricht(String.Format("Fehler  : {0}{1}", ttt.Key, ttt.Value))
            nachricht(String.Format("Fehler in TM_ausgebenMarkenDictionary: {0}{1}", vbCrLf, ex))
        End Try
    End Sub

    Private Shared Function bildeFlurstuecksliste(ByVal vid As Integer) As String
        Dim sb As New Text.StringBuilder
        Dim tab As DataTable
        Dim delim As String = " "
        Dim Gemarklungstext As String = "ERROR"
        Dim altegemarkung As String = ""
        myGlobalz.sitzung.raumbezugsRec.mydb.SQL =
            "select * from " & CLstart.myViewsNTabs.TABPARAFLURSTUECK & "  where id in (select sekid from pf_sekid2vid" &
            " where vorgangsid=" & vid & ")" &
            " order by gemcode,flur,zaehler,nenner"
        Dim hinweis As String = myGlobalz.sitzung.raumbezugsRec.getDataDT()
        tab = myGlobalz.sitzung.raumbezugsRec.dt
        For i = 0 To myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count - 1
            If tab IsNot Nothing AndAlso tab.AsEnumerable IsNot Nothing AndAlso tab.AsEnumerable.Count > 0 Then
                Gemarklungstext = getGemarkungstext(i, myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("gemcode").ToString)
                If altegemarkung = Gemarklungstext Then
                    sb.Append("")
                Else
                    If altegemarkung <> "" Then sb.Append("; ") 'trennzeichen für versch.Gemarkungen ist der ;
                    sb.Append(Gemarklungstext)
                End If
                If i = 0 Then
                    myGlobalz.sitzung.aktFST.clear()
                    myGlobalz.sitzung.aktFST.normflst.clear()
                    ' myGlobalz.sitzung.aktFST.normflst.gemarkungstext=
                    myGlobalz.sitzung.aktFST.normflst.gemarkungstext = Gemarklungstext
                    myGlobalz.sitzung.aktFST.normflst.flur = CInt(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("flur"))
                    myGlobalz.sitzung.aktFST.normflst.zaehler = CInt(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("zaehler"))
                    myGlobalz.sitzung.aktFST.normflst.nenner = CInt(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("nenner"))
                    myGlobalz.sitzung.aktFST.normflst.fstueckKombi = myGlobalz.sitzung.aktFST.normflst.buildFstueckkombi()
                End If
                sb.Append(", Flur: ")
                sb.Append(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("flur").ToString & ", Flurstück: ")
                sb.Append(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("zaehler").ToString & "/")
                sb.Append(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("nenner").ToString)
                altegemarkung = Gemarklungstext
            End If
        Next
        tab = Nothing
        Return sb.ToString
    End Function

    Private Shared Function getGemarkungstext(ByVal i As Integer, ByVal gemcode As String) As String
        Dim gemparms As New clsGemarkungsParams
        gemparms.init()
        Dim a = From item In gemparms.parms Where item.gemcode.ToString = gemcode
                Select item.gemarkungstext
        If a.ToArray.Length > 0 Then
            Return a.ToList(0).ToString
        Else
            Return ""
        End If
    End Function

    Private Shared Function getSachgebietstext_erweitert() As String
        Return clsBaumbilden.sucheSGNRInXML_erweitert(myGlobalz.Paradigma_Sachgebietsdatei,
                                                      myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
    End Function

    Private Shared Function makeStandortAkte() As String
        Dim a As String
        Try
            a = myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.RaumNr &
                   " " &
                   myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.Titel
            Return a
        Catch ex As Exception
            nachricht("fehler in makeStandortAkte: ", ex)
            Return ""
        End Try
    End Function

    Private Function tempoDateiLoeschenOk(diedatei As String) As Boolean
        If diedatei.IsNothingOrEmpty() Then Return False
        Dim fi As New IO.FileInfo(diedatei)
        If fi.Exists Then
            Try
                fi.Delete()
                fi = Nothing
                Return True
            Catch ex As Exception
                Return False
            End Try
        Else
            fi = Nothing
            Return True
        End If
    End Function
    Private Shared Sub IllgealgeHuettenMarken(replaceWordDict As Dictionary(Of String, String), vid As Integer, documentbookmarks As List(Of String))
        If myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.Trim <> "3307" Then Return
        Dim oldIlleg As New clsIllegaleHuette
        oldIlleg = modIllegaleHuette.getIllegale4Vid(myGlobalz.sitzung.aktVorgangsID)
        If oldIlleg Is Nothing Then Return
        Dim status As String = clsIllegbauTools.statusIndexNachText(oldIlleg.status)
        Dim Gebiet As String = clsIllegbauTools.gebietsIndexNachText(oldIlleg.gebiet)
        Dim Raeumung As String = clsIllegbauTools.RaeumungsTypIndexNachText(oldIlleg.raeumungsTyp)
        Dim Anhoerung As String = Format(oldIlleg.anhoerung, "dd.MM.yyyy")

        tauscheOderStandard(replaceWordDict, "Gebiet", Gebiet.Trim, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Status", status.Trim, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Raeumung", Raeumung.Trim, documentbookmarks)
        tauscheOderStandard(replaceWordDict, "Anhoerung", Anhoerung.Trim, documentbookmarks)

        oldIlleg = Nothing
    End Sub


End Class
