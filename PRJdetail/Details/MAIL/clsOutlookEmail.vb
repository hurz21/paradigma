Imports System.Data

Public Class clsOutlookEmail
    Private _VSFinfotext As String
    Private Const INT_Constant1maxCharBeschreibung As Integer = 500
    Public Property returnAction As String = ""
    Public Property fotoZuRaumbezug As Boolean
    'Public shared Property DruckerScannerString As String = "oki-mb470;canon3112;3.1.12"


    Private Sub getVCFinfotext(ByVal sender As mailMensch)
        If istVCFinteressant(sender.email) Then
            _VSFinfotext = "Diese Email enthält eine elektronische Visitenkarte (VCF-Datei)" & vbCrLf &
        "Sie können daraus Beteiligte erzeugen, indem Sie die Datei unter" & vbCrLf &
        "Dokumente anklicken und mit Paradigma öffnen!"
        Else
            _VSFinfotext = ""
        End If
    End Sub
    Public Shared Function GetAnzahlAnhaenge(ByVal dateien As List(Of String)) As Integer
        Dim anzahl%
        If dateien IsNot Nothing Then
            anzahl% = dateien.Count
        Else
            anzahl = 0
        End If
        Return anzahl
    End Function
    Private Shared Sub bildeRecipientString(ByVal reciepients As List(Of mailMensch), ByRef recipientString As String)
        For Each cand As mailMensch In reciepients
            mailTools.getOrganisationAndNameFromMailstring(cand)
            recipientString &= cand.email & ";"
        Next
    End Sub
    Public Shared Function SenderIstDruckerOderScanner(ByVal sender As mailMensch, senderbetreff As String) As Boolean
#Disable Warning BC42024 ' Unused local variable: 'b'.
        Dim b() As String
#Enable Warning BC42024 ' Unused local variable: 'b'.
        Try
            'b = initP.getValue("Haupt.DruckerScannerString").Split(";"c)
            If senderbetreff.ToLower.Contains("ihr scanauftrag") Then
                Return True
            End If
            'For Each drucker In b
            '    If sender.email.ToLower.Contains(drucker.ToLower) Or
            '        senderbetreff.ToLower.StartsWith("ihr scanauftrag") Then
            '        Return True
            '    End If
            'Next
            Return False
        Catch ex As Exception
            nachricht("fehler in SenderIstDruckerOderScanner:", ex)
            Return False
        End Try
    End Function
    Private Shared Sub bildeParadigmaEmailEreignis(ByVal senderbetreff As String)
        Dim b() As String
        Try
            b = senderbetreff.Replace("@PARADIGMA#", "").Split("#"c)

            myGlobalz.sitzung.aktEreignis.Art = "Ortstermin"
            myGlobalz.sitzung.aktEreignis.Beschreibung = "Notiz zu " & myGlobalz.sitzung.aktEreignis.Art
            myGlobalz.sitzung.aktEreignis.Quelle = "pMail"


            myGlobalz.sitzung.aktEreignis.Beschreibung = b(2)
            myGlobalz.sitzung.aktEreignis.Quelle = b(3)


            myGlobalz.sitzung.aktEreignis.Art = b(1)
            myGlobalz.sitzung.aktEreignis.Beschreibung = b(2)
            myGlobalz.sitzung.aktEreignis.Quelle = b(3)

            '@PARADIGMA#9609#Ortstermin#titeltest#Ploesser#asdsadsad#
            If myGlobalz.sitzung.aktEreignis.Beschreibung.IsNothingOrEmpty Then
                myGlobalz.sitzung.aktEreignis.Beschreibung = "Notiz zu " & myGlobalz.sitzung.aktEreignis.Art
            End If
        Catch ex As Exception
            nachricht("fehler in bildeParadigmaEmailEreignis: ", ex)
        End Try
    End Sub
    Sub Aufnahme(ByVal fotoZuRaumbezug As Boolean, aktvid As Integer, ByRef erfolg As Boolean)
        Dim sender As New mailMensch
        Dim senderbetreff$ = "", senderText$ = "", erhaltenAmDatum As Date, Beschreibung As String
        Dim createAttachmentFiles As Boolean = True, createEreignis As Boolean = True, emailBodyAlsDokument As Boolean = False
        Dim reciepients As New List(Of mailMensch), cc As New List(Of mailMensch), bcc As New List(Of mailMensch)
        Dim istPARADIGMAemail As Boolean = False
        Dim emailTyp As String = "email"
        Dim fehler As String = ""


        erfolg = True
        'test-----------
        Dim dateien As List(Of String) =
        MeinOutlook.outlookEmailAuswerten(myGlobalz.Arc.lokalerCheckoutcache,
                                           "Email_" & aktvid & "_" & myGlobalz.sitzung.aktBearbeiter.Initiale & "_",
                                            sender,
                                            senderbetreff,
                                            senderText,
                                            erhaltenAmDatum,
                                            createAttachmentFiles,
                                             cc, bcc, reciepients,
                                             sender.telefon, aktvid,
                                             istPARADIGMAemail, emailTyp, fehler)
        Dim ftool As New clsWindokueincheckenTOOL
        Dim loeschliste As New List(Of String)
        'Dim neueliste As New List(Of String)
        dateien = ftool.fotoListeUmwandeln(dateien, loeschliste)

        If fehler.StartsWith("fehler") Then
            erfolg = False
            MessageBox.Show("Abbruch der Übernahme der Outlook-Email mit " & fehler)
            Exit Sub
        End If

        If emailIstInternKreisOffenbach(sender.email) Then
            sender.email = makeInterneEmailAdresse(sender.name)
        End If

        If emailEnthaeltVCF(dateien) Then
            getVCFinfotext(sender)
        Else
            _VSFinfotext = ""
        End If
        Dim anzahlAnhaenge As Integer
        anzahlAnhaenge = GetAnzahlAnhaenge(dateien)
        EreignisPreparieren(sender.name, senderbetreff, erhaltenAmDatum, createEreignis, senderText, sender.email, reciepients, emailTyp)
        Debug.Print(myGlobalz.sitzung.aktEreignis.Art)
        Dim person_name = ""
        mailTools.getOrganisationAndNameFromMailstring(sender)

        Dim recipientString As String = ""
        bildeRecipientString(reciepients, recipientString)
        If SenderIstDruckerOderScanner(sender, senderbetreff) Then
            Dim mesresult As New MessageBoxResult
            mesresult = MessageBox.Show("Die Email kommt vom Scanner. " & Environment.NewLine &
                            "Soll nur das gescannte PDF-Dokument übernommen werden ? " & Environment.NewLine &
                            Environment.NewLine &
                            "Ja    - nur das PDF-Dokument übernehmen" & Environment.NewLine &
                            "Nein - ganze Email übernehmen",
                                        "Sender ist der Scanner. ", MessageBoxButton.YesNo, MessageBoxImage.Question)
            If mesresult = MessageBoxResult.Yes Then
                '       pdfUebernehmen
                '    pdfUebernehmen(dateien, aktvid)
                Dim ScanPDFFullpath As String
                ScanPDFFullpath = getRealScan(dateien)
                returnAction = "dokushinzufuegen#" & ScanPDFFullpath
                Exit Sub
            End If

        End If

        Dim winOutlook As New winOutlookEmailUebernehmen(anzahlAnhaenge - 1, sender.name, senderbetreff, _VSFinfotext,
                                                   sender.email, sender.organization, person_name,
                                                   reciepients.Count, recipientString, myGlobalz.sitzung.aktEreignis.Beschreibung,
                                                   myGlobalz.sitzung.aktEreignis.Richtung, erhaltenAmDatum, dateien, aktvid)
        winOutlook.ShowDialog()
        If Not String.IsNullOrEmpty(winOutlook.tbBeschreibung.Text) Then
            myGlobalz.sitzung.aktEreignis.Beschreibung = LIBgemeinsames.clsString.noWhiteSpace(winOutlook.tbBeschreibung.Text)
        End If
        fotoZuRaumbezug = winOutlook.fotosalsRaumbezug
        Dim result As Boolean = CBool(winOutlook.DialogResult)
        If Not result Then
            'abbruch
            Exit Sub
        End If
        Dim dokumentDatum As Date = CDate(winOutlook.DatePickerDokument.Value)

        If istPARADIGMAemail Then
            createEreignis = True
            'myGlobalz.sitzung.aktEreignis.Quelle="Ortstermin"
            bildeParadigmaEmailEreignis(senderbetreff)
        Else
            If Not SenderIstDruckerOderScanner(sender, senderbetreff) Then
                mailTools.SenderAlsBeteiligtenUebernehmen(CBool(winOutlook.chkSenderBeteiligtenanlegen.IsChecked), sender.email, person_name, sender.organization,
                                                          sender.telefon)
            End If
            mailTools.ReciepientsAlsBeteiligteUebernehmen(CBool(winOutlook.chkRecipientsBeteiligtenanlegen.IsChecked), reciepients)
            createEreignis = CBool(winOutlook.mitVerlauf.IsChecked)
        End If
        Dim erzeugtEreignis As Boolean = False
        If createEreignis Then
            mailTools.KontrollausgabeMailEreignis(myGlobalz.sitzung.aktEreignis)
            mailTools.KorrekturMailEreignis(myGlobalz.sitzung.aktEreignis)
            mailTools.KontrollausgabeMailEreignis(myGlobalz.sitzung.aktEreignis)
            erzeugtEreignis = clsEreignisTools.NeuesEreignisSpeichern_alleDB(aktvid, "neu", myGlobalz.sitzung.aktEreignis)
            If Not (erzeugtEreignis) Then
                MsgBox("Ereignis konnte nicht erzeugt werden. Tipp: Falls die Email defekt ist, schicken Sie die Email an ihre eigene Emailadresse. Dann sollte der Import funktionieren!")
            End If



            '  erzeugtEreignis = clsEreignisTools.NeuesEreignisSpeichern_alleDB(aktvid, "neu", myGlobalz.sitzung.aktEreignis)
        End If
        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
            myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
        createAttachmentFiles = CBool(winOutlook.mitAnhang.IsChecked)

        emailBodyAlsDokument = CBool(winOutlook.mitEmailBody.IsChecked)
        Beschreibung = winOutlook.tbSchlagworte.Text
        If Not createAttachmentFiles And Not createEreignis Then Exit Sub

        If winOutlook.anhangsauswahlListe IsNot Nothing Then
            MaildateienEincheckenUndMitEreignisKoppeln(winOutlook.anhangsauswahlListe, sender.name, senderbetreff, senderText, dokumentDatum,
                                                   createAttachmentFiles, createEreignis,
                                                   emailBodyAlsDokument, Beschreibung, fotoZuRaumbezug, erhaltenAmDatum,
                                                   myGlobalz.sitzung.aktEreignis.ID, aktvid)
        Else
            MessageBox.Show("Hinweis:" & vbCrLf & vbCrLf & "Sie müssen Outlook starten und die gewünschte Email auswählen!",
                            "Übernahme von Emails aus Outlook", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
        End If
        If aktvid = myGlobalz.sitzung.aktVorgangsID Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = Now
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LETZTEBEARBEITUNG")
            ' glob2.EDIT_VorgangStamm_2DBOk()
        End If

    End Sub

    '    Private Sub pdfUebernehmen(dateien As List(Of String), aktvid As Integer)
    '        Dim ScanPDFFullpath As String
    '        ScanPDFFullpath = getRealScan(dateien)
    '#Disable Warning BC42024 ' Unused local variable: 'eincheckenErfolgreich'.
    '        Dim eincheckenErfolgreich As Boolean
    '#Enable Warning BC42024 ' Unused local variable: 'eincheckenErfolgreich'.
    '        myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)

    '        myGlobalz.sitzung.aktDokument.newSaveMode = True
    '        'Dim schlagworte As String = "Scandatei" '& " " & FileArchivTools.buildSchlagworteAusDateiname(datei)
    '        Dim erfolgreich As Boolean = myGlobalz.Arc.checkINDoku(ScanPDFFullpath, 0, "Scandatei",
    '                                                               aktvid, False,
    '                                                               "",
    '                                                               Now,
    '                                                               myGlobalz.sitzung.aktDokument.DocID,
    '                                                               myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir, False)
    '        If erfolgreich Then
    '            nachricht("Checkin  erfolgreich: " & ScanPDFFullpath)
    '        Else
    '            nachricht_und_Mbox("Checkin nicht erfolgreich: " & ScanPDFFullpath)
    '        End If
    '    End Sub

    Public Shared Function getRealScan(dateien As List(Of String)) As String
        Dim ScanPDFFullpath As String
        Try
            l("pdfUebernehmen---------------------- anfang")
            For i = 0 To dateien.Count - 1
                If dateien(i).ToLower.EndsWith(".pdf") Then
                    ScanPDFFullpath = dateien(i)
                    Return ScanPDFFullpath
                End If
            Next
            Return ""
            l("pdfUebernehmen---------------------- ende")
        Catch ex As Exception
            l("Fehler in pdfUebernehmen : ", ex)
        End Try

#Disable Warning BC42104 ' Variable 'ScanPDFFullpath' is used before it has been assigned a value. A null reference exception could result at runtime.
        Return ScanPDFFullpath
#Enable Warning BC42104 ' Variable 'ScanPDFFullpath' is used before it has been assigned a value. A null reference exception could result at runtime.
    End Function

    Public Shared Function makeInterneEmailAdresse(ByVal sendername As String) As String
        Dim senderemailAdress As String
        Dim a() As String
        Dim anfang As String
        Try
            If sendername.Contains(",") Then
                a = sendername.Split(","c)
                anfang = a(1).Trim.Substring(0, 1) & "." & a(0).Trim
                senderemailAdress = anfang & "@kreis-offenbach.de"
                Return senderemailAdress
            Else
                senderemailAdress = sendername & "@kreis-offenbach.de"
                Return senderemailAdress
            End If

        Catch ex As Exception
            Return "@kreis-offenbach.de"
        End Try
    End Function
    Public Shared Sub MaildateienEincheckenUndMitEreignisKoppeln(ByVal anhangsauswahlListe As List(Of AnhangsdateiAuswahl),
                                                 ByVal sendername As String,
                                                 ByVal senderbetreff As String,
                                                 ByVal senderText As String,
                                                 ByVal dokumentdatum As Date,
                                                 ByVal createAttachmentFiles As Boolean,
                                                 ByVal createEreignis As Boolean,
                                                 ByVal emailBodyAlsDokument As Boolean,
                                                 ByVal Beschreibung As String,
                                                  ByVal fotoZuRaumbezug As Boolean,
                                                  maildatum As Date,
                                                  ereignisid As Integer,
                                                  aktvid As Integer)
        nachricht("MaildateienEincheckenUndEreignisbilden ---------------------------")
        Try
            If anhangsauswahlListe IsNot Nothing Then
                'Ereignis anlegen

                'ereignis nummer festhalten
                'dokumente einchecken
                If createAttachmentFiles Then MailDokumenteEinchecken(anhangsauswahlListe, emailBodyAlsDokument, Beschreibung, fotoZuRaumbezug,
                    dokumentdatum, ereignisid, aktvid)
                nachricht("MaildateienEincheckenUndEreignisbilden ---------------------------")
            Else
                nachricht("dateien is nothing. Es muss nichts eingecheckt werden!")
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in MaildateienEincheckenUndEreignisbilden: " & vbCrLf, ex)
        End Try
    End Sub

    Public Shared Sub EreignisPreparieren(ByVal sendername As String, ByVal senderbetreff As String, ByVal erhaltenAm As Date,
                                          ByVal createEreignis As Boolean, ByVal senderText As String, ByVal senderemailAdress As String,
                                          reciepients As List(Of mailMensch), emailtyp As String)
        myGlobalz.sitzung.aktEreignis = EmailEreignisAnpassen(0, sendername, senderbetreff, senderText$, erhaltenAm, senderemailAdress,
                                                            reciepients, emailtyp)
        ' If clsEreignisDB.Neu_speichern_Ereignis() Then nachricht("Daten wurden gespeichert!")

    End Sub

    Private Shared Function istEmailBody(ByVal emailBodyAlsDokument As Boolean, ByVal zi As Integer) As Boolean
        Return zi = 0 And Not emailBodyAlsDokument
    End Function

    Private Shared Sub MailDokumenteEinchecken(ByVal anhangsauswahlListe As List(Of AnhangsdateiAuswahl),
                                               ByVal emailBodyAlsDokument As Boolean,
                                               ByVal Beschreibung As String,
                                                ByVal fotoZuRaumbezug As Boolean,
                                                maildatum As Date,
                                                ereignisid As Integer,
                                                aktvid As Integer)
        nachricht("DokumenteEinchecken ----------------------------")
        nachricht("dateien count ----------------------------" & anhangsauswahlListe.Count)
        Dim zi% = 0
        Dim NumDir As String
        Try
            '  NumDir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
            NumDir = ""
            For Each anhangDatei As AnhangsdateiAuswahl In anhangsauswahlListe
                zi += 1
                If Not anhangDatei.gewaehlt Then Continue For
                If istEmailBody(emailBodyAlsDokument, zi) Then Continue For
                nachricht("datei akt: " & anhangDatei.dateiname)
                myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                myGlobalz.sitzung.aktDokument.newSaveMode = True

                Dim schlagworte As String = Beschreibung '& " " & FileArchivTools.buildSchlagworteAusDateiname(datei)
                Dim erfolgreich As Boolean = myGlobalz.Arc.checkINDoku(anhangDatei.dateiname, ereignisid, schlagworte,
                                                                       aktvid, fotoZuRaumbezug,
                                                                       NumDir,
                                                                       maildatum,
                                                                       myGlobalz.sitzung.aktDokument.DocID,
                                                                       myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir, False,
                                                                       myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID)
                If erfolgreich Then
                    nachricht("Checkin  erfolgreich: " & anhangDatei.dateiname)
                Else
                    nachricht_und_Mbox("Checkin nicht erfolgreich: " & anhangDatei.dateiname)
                End If
            Next
            nachricht("DokumenteEinchecken -------------- ende--------------")
        Catch ex As Exception
            nachricht("fehler in DokumenteEinchecken: ", ex)
        End Try
    End Sub

    Private Shared Function EreigBeschreibung(ByVal sendername As String,
                                              ByVal senderBetreff As String,
                                              ereig As clsEreignis,
                                              reciepients As List(Of mailMensch),
                                              emailTyp As String) As String
        If ereig.Richtung = "Eingang" Then
            Return (sendername & ": " & senderBetreff)
        Else
            Dim adressat As String = ""
            For Each sepp In reciepients
                If emailIstInternKreisOffenbach(sepp.email) Then
                    sepp.email = makeInterneEmailAdresse(sepp.name)
                End If

                If sepp.name.Trim.ToLower.Replace("'", "") = sepp.email.Trim.ToLower Then
                    If adressat.IsNothingOrEmpty Then
                        adressat = sepp.email & ", "
                    Else
                        adressat = adressat & ", " & sepp.email & ", "
                    End If
                Else
                    If adressat.IsNothingOrEmpty Then
                        If sepp.name.Length > 5 Then
                            adressat = sepp.name & ", "
                        Else
                            adressat = sepp.name & " " & sepp.email & ", "
                        End If
                    Else
                        If sepp.name.Length > 5 Then
                            adressat = adressat & ", " & sepp.name & ", "
                        Else
                            adressat = adressat & ", " & sepp.name & " " & sepp.email & ", "
                        End If
                    End If
                End If
            Next
            Dim anfang As String = "Email an:"
            Select Case emailTyp
                Case "email"
                    anfang = "Email an: "
                Case "report"
                    anfang = "Terminübernahme: "
                Case "meeting"
                    anfang = "Einladung zur Besprechung: "
                Case Else
                    anfang = "Email: "
            End Select
            Return (anfang & adressat.ToString & " wg: " & senderBetreff)
        End If

    End Function
    Public Shared Function EmailEreignisAnpassen(ByVal eid As Integer,
                                                    ByVal sendername As String,
                                                    ByVal senderBetreff As String,
                                                    ByVal senderText As String,
                                                    ByVal erhaltenAm As Date,
                                                    ByVal senderemailAdress As String,
                                                    reciepients As List(Of mailMensch),
                                                 emailtyp As String) As clsEreignis
        Try
            Dim ereig As New clsEreignis
            Dim maxCharNotiz% = 10000
            Dim maxCharBeschreibung = 500
            ereig.clearValues()
            nachricht("EmailEreignisAnpassen-----------------------------------------")
            nachricht("sendername$:" & sendername$)
            nachricht("senderBetreff:" & senderBetreff$)
            nachricht(":" & senderText$)
            nachricht("maxCharNotiz%:" & maxCharNotiz%)
            nachricht(":" & maxCharBeschreibung)
            nachricht("senderemailAdress:" & senderemailAdress)
            If sendername$ Is Nothing Then sendername$ = ""
            If senderBetreff$ Is Nothing Then senderBetreff$ = ""
            If senderText Is Nothing Then senderText = ""
            If eid > 0 Then ereig.ID = eid
            ereig.Datum = erhaltenAm 'Now
            'ereig.Art = "Email-Eingang"
            Select Case emailtyp
                Case "email"
                    ereig.Art = "Email-Eingang"
                Case "report"
                    ereig.Art = "Termin"
                Case "meeting"
                    ereig.Art = "Besprechung"
                Case Else
                    ereig.Art = "Email"
            End Select
            EmailEreignisAnpassenEINAUSGANG(sendername, ereig)
            senderText = makecr(senderText)
            If senderText.Length > maxCharNotiz Then
                ereig.Notiz = senderText.Substring(0, maxCharNotiz%)
                nachricht("Hinweis: Die Notiz wird auf " & maxCharNotiz% & " Zeichen beschränkt / abgeschnitten ")
            Else
                Dim test As String = makecr(senderText)
                ereig.Notiz = test
            End If
            'clsString.replaceEuroSign(ereig.Notiz)
            'clsString.replaceSpecialSymbols(ereig.Notiz)
            '   ereig.Beschreibung = (sendername & " (" & senderemailAdress & "): " & senderBetreff)
            ereig.Beschreibung = EreigBeschreibung(sendername, senderBetreff, ereig, reciepients, emailtyp)
            'clsString.replaceEuroSign(ereig.Beschreibung)
            If ereig.Beschreibung.Length > INT_Constant1maxCharBeschreibung Then
                ereig.Beschreibung = ereig.Beschreibung.Substring(0, INT_Constant1maxCharBeschreibung)
            End If
            If senderText.StartsWith("(Von Microsoft Outlook-Server") Then
                myGlobalz.sitzung.aktEreignis.Art = "Zustellungsbestätigung"
                'Else
                '    myGlobalz.sitzung.aktEreignis.Art = "Email"
            End If


            myGlobalz.sitzung.aktEreignis.Richtung = ereig.Richtung
            myGlobalz.sitzung.aktEreignis.Beschreibung = ereig.Beschreibung
            nachricht("EmailEreignisAnpassen---------------- ende -------------------------")
            Return ereig
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in NeuesEreigniserzeugen: " & vbCrLf, ex)
            Return Nothing
        End Try
    End Function

    Private Shared Sub EmailEreignisAnpassenEINAUSGANG(ByVal sendername As String, ByVal ereig As clsEreignis)
        If ereig.Art.ToLower.StartsWith("email") Then
            If sendername.ToLower.Trim.Contains(LIBgemeinsames.clsString.umlaut2ue(myGlobalz.sitzung.aktBearbeiter.Name.ToLower.Trim)) Then
                ereig.Richtung = "Ausgang"
                ereig.Art = "Email-Ausgang"
            Else
                ereig.Richtung = "Eingang"
                ereig.Art = "Email-Eingang"
            End If
        End If
        If ereig.Art.ToLower.StartsWith("besprechung") Then
            If sendername.ToLower.Trim.Contains(LIBgemeinsames.clsString.umlaut2ue(myGlobalz.sitzung.aktBearbeiter.Name.ToLower.Trim)) Then
                ereig.Richtung = "Ausgang"
            Else
                ereig.Richtung = "Eingang"
            End If
        End If
    End Sub

    Public Shared Function emailEnthaeltVCF(ByVal dateien As List(Of String)) As Boolean
        If dateien Is Nothing Then Return False
        For Each dat In dateien
            If dat.ToLower.Contains(".vcf") Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Shared Function istVCFinteressant(ByVal senderemailAdress As String) As Boolean
        If String.IsNullOrEmpty(senderemailAdress) Then
            Return False
        End If
        If senderemailAdress.ToLower.Contains("@kreis-offenbach") Then
            Return False
        End If
        Return True
    End Function

    Private Shared Function makecr(senderText As String) As String
        Dim test As String
        test = senderText.Replace(vbCrLf, "#").Replace(vbCr, "#").Replace(vbLf, "#").Replace(vbTab, "#")
        test = test.Replace("## ##", vbCrLf)
        test = test.Replace("##", vbCrLf)
        test = test.Replace("#", " ")
        Return test
    End Function



End Class