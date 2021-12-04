Imports System.Data

Public Class clsOutlookEmail
    Private _VSFinfotext As String
    Private Const INT_Constant1maxCharBeschreibung As Integer = 500
    Public Property fotoZuRaumbezug As Boolean
    'Public shared Property DruckerScannerString As String = "oki-mb470;canon3112;3.1.12"


    Private Sub getVCFinfotext(ByVal sender As LIBmailmensch.mailMensch)
        If istVCFinteressant(sender.email) Then
            _VSFinfotext = "Diese Email enthält eine elektronische Visitenkarte (VCF-Datei)" & vbCrLf &
        "Sie können daraus Beteiligte erzeugen, indem Sie die Datei unter" & vbCrLf &
        "Dokumente anklicken und mit Paradigma öffnen!"
        Else
            _VSFinfotext = ""
        End If
    End Sub
    Private Shared Function GetAnzahlAnhaenge%(ByVal dateien As List(Of String))
        Dim anzahl%
        If dateien IsNot Nothing Then
            anzahl% = dateien.Count
        Else
            anzahl = 0
        End If
        Return anzahl
    End Function
    Private Shared Sub bildeRecipientString(ByVal reciepients As List(Of LIBmailmensch.mailMensch), ByRef recipientString As String)
        For Each cand As LIBmailmensch.mailMensch In reciepients
            mailTools.getOrganisationAndNameFromMailstring(cand)
            recipientString &= cand.email & ";"
        Next
    End Sub
    Public Shared Function SenderIstDruckerOderScanner(ByVal sender As LIBmailmensch.mailMensch) As Boolean
        Dim b() As String
        Try
            b = initP.getValue("Haupt.DruckerScannerString").Split(";"c)
            For Each drucker In b
                If sender.email.ToLower.Contains(drucker.ToLower) Then
                    Return True
                End If
            Next
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
    Sub Aufnahme(ByVal fotoZuRaumbezug As Boolean, aktvid As Integer)
        Dim sender As New LIBmailmensch.mailMensch
        Dim senderbetreff$ = "", senderText$ = "", erhaltenAmDatum As Date, Beschreibung As String
        Dim createAttachmentFiles As Boolean = True, createEreignis As Boolean = True, emailBodyAlsDokument As Boolean = False
        Dim reciepients As New List(Of LIBmailmensch.mailMensch), cc As New List(Of LIBmailmensch.mailMensch), bcc As New List(Of LIBmailmensch.mailMensch)
        Dim istPARADIGMAemail As Boolean = False
        'test-----------
        Dim dateien As List(Of String) = LIBOutlook2.MeinOutlook.outlookEmailAuswerten(myGlobalz.Arc.lokalerCheckoutcache,
                                           "Email_" & aktvid & "_" & myGlobalz.sitzung.aktBearbeiter.Initiale & "_",
                                            sender, _
                                            senderbetreff, _
                                            senderText,
                                            erhaltenAmDatum,
                                            createAttachmentFiles,
                                             cc, bcc, reciepients,
                                             sender.telefon, aktvid,
                                             istPARADIGMAemail)
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
        EreignisPreparieren(sender.name, senderbetreff, erhaltenAmDatum, createEreignis, senderText, sender.email, reciepients)

        Dim person_name = "", literalname As String = ""
        mailTools.getOrganisationAndNameFromMailstring(sender)

        Dim recipientString As String = ""
        bildeRecipientString(reciepients, recipientString)

        Dim outl As New winOutlookEmailUebernehmen(anzahlAnhaenge - 1, sender.name, senderbetreff, _VSFinfotext,
                                                   sender.email, sender.organization, person_name,
                                                   reciepients.Count, recipientString, myGlobalz.sitzung.aktEreignis.Beschreibung,
                                                   myGlobalz.sitzung.aktEreignis.Richtung, erhaltenAmDatum, dateien, aktvid)
        outl.ShowDialog()
        If Not String.IsNullOrEmpty(outl.tbBeschreibung.Text) Then
            myGlobalz.sitzung.aktEreignis.Beschreibung = clsString.noWhiteSpace(outl.tbBeschreibung.Text)
        End If
        fotoZuRaumbezug = outl.fotosalsRaumbezug
        Dim result As Boolean = CBool(outl.DialogResult)
        If Not result Then
            'abbruch
            Exit Sub
        End If


        If istPARADIGMAemail Then
            createEreignis = True
            'myGlobalz.sitzung.aktEreignis.Quelle="Ortstermin"
            bildeParadigmaEmailEreignis(senderbetreff)

        Else
            If Not SenderIstDruckerOderScanner(sender) Then
                mailTools.SenderAlsBeteiligtenUebernehmen(CBool(outl.chkSenderBeteiligtenanlegen.IsChecked), sender.email, person_name, sender.organization,
                                                          sender.telefon)
            End If
            mailTools.ReciepientsAlsBeteiligteUebernehmen(CBool(outl.chkRecipientsBeteiligtenanlegen.IsChecked), reciepients)

            createEreignis = CBool(outl.mitVerlauf.IsChecked)
        End If
        If createEreignis Then clsEreignisTools.NeuesEreignisSpeichern_alleDB(aktvid, "neu", myGlobalz.sitzung.aktEreignis)
        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
            myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
        createAttachmentFiles = CBool(outl.mitAnhang.IsChecked)

        emailBodyAlsDokument = CBool(outl.mitEmailBody.IsChecked)
        Beschreibung = outl.tbSchlagworte.Text
        If Not createAttachmentFiles And Not createEreignis Then Exit Sub

        If outl.anhangsauswahlListe IsNot Nothing Then
            MaildateienEincheckenUndMitEreignisKoppeln(outl.anhangsauswahlListe, sender.name, senderbetreff, senderText, erhaltenAmDatum,
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
                                                 ByVal erhaltenAm As Date,
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
                    maildatum, ereignisid, aktvid)
                nachricht("MaildateienEincheckenUndEreignisbilden ---------------------------")
            Else
                nachricht("dateien is nothing. Es muss nichts eingecheckt werden!")
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in MaildateienEincheckenUndEreignisbilden: " & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Shared Sub EreignisPreparieren(ByVal sendername As String, ByVal senderbetreff As String, ByVal erhaltenAm As Date,
                                          ByVal createEreignis As Boolean, ByVal senderText As String, ByVal senderemailAdress As String,
                                          reciepients As List(Of LIBmailmensch.mailMensch))
        myGlobalz.sitzung.aktEreignis = EmailEreignisAnpassen(0, sendername, senderbetreff, senderText$, erhaltenAm, senderemailAdress,
                                                            reciepients)
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
            NumDir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
            For Each datei As AnhangsdateiAuswahl In anhangsauswahlListe
                zi += 1
                If Not datei.gewaehlt Then Continue For
                If istEmailBody(emailBodyAlsDokument, zi) Then Continue For
                nachricht("datei akt: " & datei.dateiname)
                myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                Dim schlagworte As String = Beschreibung '& " " & FileArchivTools.buildSchlagworteAusDateiname(datei)
                Dim erfolgreich As Boolean = myGlobalz.Arc.checkINDoku(datei.dateiname, ereignisid, schlagworte,
                                                                       aktvid, fotoZuRaumbezug,
                                                                       NumDir,
                                                                       maildatum,
                                                                       myGlobalz.sitzung.aktDokument.DocID,
                                                                       myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
                If erfolgreich Then
                    nachricht("Checkin  erfolgreich: " & datei.dateiname)
                Else
                    nachricht_und_Mbox("Checkin nicht erfolgreich: " & datei.dateiname)
                End If
            Next
            nachricht("DokumenteEinchecken -------------- ende--------------")
        Catch ex As Exception
            nachricht("fehler in DokumenteEinchecken: " & ex.ToString)
        End Try
    End Sub

    Private Shared Function EreigBeschreibung(ByVal sendername As String,
                                              ByVal senderBetreff As String,
                                              ereig As clsEreignis,
                                              reciepients As List(Of LIBmailmensch.mailMensch)) As String
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
                             adressat = adressat & ", " & sepp.name  & ", "
                            else
                             adressat = adressat & ", " & sepp.name & " " & sepp.email & ", "
                         End If
                       
                    End If
                    
                End If

            Next
            Return ("Email an: " & adressat.ToString & ", wg: " & senderBetreff)
        End If

    End Function
    Public Shared Function EmailEreignisAnpassen(ByVal eid As Integer,
                                                    ByVal sendername As String,
                                                    ByVal senderBetreff As String,
                                                    ByVal senderText As String,
                                                    ByVal erhaltenAm As Date,
                                                    ByVal senderemailAdress As String,
                                                    reciepients As List(Of LIBmailmensch.mailMensch)) As clsEreignis
        Try
            Dim ereig As New clsEreignis
            Dim maxCharNotiz% = 10000
            Dim maxCharBeschreibung = 500
            ereig.clearValues()
            nachricht("EmailEreignisAnpassen-----------------------------------------")
            nachricht("sendername$:" & sendername$)
            nachricht("senderBetreff$$:" & senderBetreff$)
            nachricht("senderText$$:" & senderText$)
            nachricht("maxCharNotiz%$$:" & maxCharNotiz%)
            nachricht("maxCharBeschreibung$$:" & maxCharBeschreibung)
            nachricht("senderemailAdress:" & senderemailAdress)
            If sendername$ Is Nothing Then sendername$ = ""
            If senderBetreff$ Is Nothing Then senderBetreff$ = ""
            If senderText Is Nothing Then senderText = ""
            If eid > 0 Then ereig.ID = eid
            ereig.Datum = erhaltenAm 'Now
            ereig.Art = "Email-Eingang"
            EmailEreignisAnpassenEINAUSGANG(sendername, ereig)
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
            ereig.Beschreibung = EreigBeschreibung(sendername, senderBetreff, ereig, reciepients)
            'clsString.replaceEuroSign(ereig.Beschreibung)
            If ereig.Beschreibung.Length > INT_Constant1maxCharBeschreibung Then
                ereig.Beschreibung = ereig.Beschreibung.Substring(0, INT_Constant1maxCharBeschreibung)
            End If
            If senderText.StartsWith("(Von Microsoft Outlook-Server") Then
                myGlobalz.sitzung.aktEreignis.Art = "Zustellungsbestätigung"
            Else
                myGlobalz.sitzung.aktEreignis.Art = "Email"
            End If

            myGlobalz.sitzung.aktEreignis.Beschreibung = ereig.Beschreibung
            nachricht("EmailEreignisAnpassen---------------- ende -------------------------")
            Return ereig
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in NeuesEreigniserzeugen: " & vbCrLf & ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Shared Sub EmailEreignisAnpassenEINAUSGANG(ByVal sendername As String, ByVal ereig As clsEreignis)
        If sendername.ToLower.Trim.Contains(clsString.umlaut2ue(myGlobalz.sitzung.aktBearbeiter.Name.ToLower.Trim)) Then
            ereig.Richtung = "Ausgang"
            ereig.Art = "Email-Ausgang"
        Else
            ereig.Richtung = "Eingang"
            ereig.Art = "Email-Eingang"
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
        test = test.Replace("#", "")
        Return test
    End Function



End Class