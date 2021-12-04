
Imports System.Data
Public Class clsEMLemail
    Private _VSFinfotext As String
    Private Const INT_Constant1maxCharBeschreibung As Integer = 500
    Public Property fotoZuRaumbezug As Boolean
    Property emlfullpath As String
    Sub New(ByVal _emlfullpath As String)
        emlfullpath = _emlfullpath
    End Sub
    Public Sub Aufnahme(ByVal fotoZuRaumbezug As Boolean, aktvid As Integer, ByRef erfolg As Boolean, ByRef problemMitanhang As Boolean)
        Dim sender As New mailMensch
        Dim senderbetreff$ = "", senderText$ = "", erhaltenAmDatum As Date
        Dim Beschreibung As String = ""
        Dim createAttachmentFiles As Boolean = True, createEreignis As Boolean = True, emailBodyAlsDokument As Boolean = False
        Dim reciepients As New List(Of mailMensch), cc As New List(Of mailMensch), bcc As New List(Of mailMensch)
        Dim dateien As List(Of String)
        Dim emailTyp As String = "email"
        dateien = clsEML.AnhangSpeichern(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID,
                                           "Email_" & myGlobalz.sitzung.aktVorgangsID & "_" & myGlobalz.sitzung.aktBearbeiter.Initiale & "_",
                                            sender,
                                            senderbetreff,
                                            senderText,
                                            erhaltenAmDatum,
                                            createAttachmentFiles,
                                            emlfullpath,
                                            cc,
                                            bcc,
                                            reciepients,
                                            sender.telefon,
                                            problemMitanhang)

        'If problemMitanhang Then
        '    MessageBox.Show("Es gab ein Problem mit dem Anhang. Bitte prüfen ob der Anhang vollständig rüberkam!", "Problem")
        'End If
        If clsOutlookEmail.emailEnthaeltVCF(dateien) Then
            If clsOutlookEmail.istVCFinteressant(sender.email) Then
                _VSFinfotext = "Diese Email enthält eine elektronische Visitenkarte (VCF-Datei)" & vbCrLf &
            "Sie können daraus Beteiligte erzeugen, indem Sie die Datei unter" & vbCrLf &
            "Dokumente anklicken und mit Paradigma öffnen!"
            Else
                _VSFinfotext = ""
            End If
        Else
            _VSFinfotext = ""
        End If

        Dim anzahlDateien As Integer
        If dateien IsNot Nothing Then
            anzahlDateien = dateien.Count
        End If


        EreignisPreparieren(sender.name, senderbetreff, erhaltenAmDatum, createEreignis, senderText, sender.email, reciepients, emailTyp)

        ' folgende routing ewurde aus bequemlichkeit ausgeschaltet
        'clsOutlookEmail.EreignisPreparieren(sender.name, senderbetreff, erhaltenAm, createEreignis, senderText, sender.email, reciepients)


        Dim person_name As String = ""
        mailTools.getOrganisationAndNameFromMailstring(sender)


        Dim recipientString As String = ""
        For Each cand As mailMensch In reciepients
            '  mailTools.getOrganisationAndNameFromMailstring(cand.email, cand.organization, cand.name, cand.shortemail, cand.literalname)
            mailTools.getOrganisationAndNameFromMailstring(cand)
            recipientString &= cand.email & ";"
        Next

        Dim outl As New winOutlookEmailUebernehmen(anzahlDateien - 1, sender.name, senderbetreff, _VSFinfotext,
                                                   sender.email, sender.organization, person_name,
                                                   reciepients.Count, recipientString, Beschreibung,
                                                   myGlobalz.sitzung.aktEreignis.Richtung, erhaltenAmDatum, dateien, aktvid)

        outl.ShowDialog()
        If Not String.IsNullOrEmpty(outl.tbBeschreibung.Text) Then
            myGlobalz.sitzung.aktEreignis.Beschreibung = LIBgemeinsames.clsString.noWhiteSpace(outl.tbBeschreibung.Text)
        End If
        fotoZuRaumbezug = outl.fotosalsRaumbezug
        Dim result As Boolean = CBool(outl.DialogResult)
        If Not result Then
            'abbruch
            Exit Sub
        End If

        mailTools.SenderAlsBeteiligtenUebernehmen(CBool(outl.chkSenderBeteiligtenanlegen.IsChecked), sender.email, person_name, sender.organization, sender.telefon)
        mailTools.ReciepientsAlsBeteiligteUebernehmen(CBool(outl.chkRecipientsBeteiligtenanlegen.IsChecked), reciepients)

        createEreignis = CBool(outl.mitVerlauf.IsChecked)
        If createEreignis Then clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", myGlobalz.sitzung.aktEreignis)
        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
            myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
        createAttachmentFiles = CBool(outl.mitAnhang.IsChecked)

        emailBodyAlsDokument = CBool(outl.mitEmailBody.IsChecked)
        Beschreibung = outl.tbSchlagworte.Text
        If Not createAttachmentFiles And Not createEreignis Then Exit Sub

        Dim dokdat As Date

        'geändert  myGlobalz.sitzung.aktEreignis.Datum   statt erhaltenAmDatum
        If myGlobalz.zuhause Then
            dokdat = myGlobalz.sitzung.aktEreignis.Datum
        Else
            dokdat = erhaltenAmDatum
        End If
        If outl.anhangsauswahlListe IsNot Nothing Then
            clsOutlookEmail.MaildateienEincheckenUndMitEreignisKoppeln(outl.anhangsauswahlListe, sender.name, senderbetreff, senderText, dokdat,
                                                                   createAttachmentFiles, createEreignis,
                                                                   emailBodyAlsDokument, Beschreibung, fotoZuRaumbezug,
                                                                   erhaltenAmDatum,
                                                                   myGlobalz.sitzung.aktEreignis.ID, aktvid)
        Else
            MessageBox.Show("Hinweis:" & vbCrLf & vbCrLf & "Sie müssen Outlook starten und die gewünschte Email öffnen!", "Übernahme von Emails aus Outlook", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
        End If
    End Sub

    Public Shared Sub EreignisPreparieren(ByVal sendername As String, ByVal senderbetreff As String, ByVal erhaltenAm As Date,
                                      ByVal createEreignis As Boolean, ByVal senderText As String, ByVal senderemailAdress As String,
                                      reciepients As List(Of mailMensch), emailtyp As String)
        myGlobalz.sitzung.aktEreignis = EmailEreignisAnpassen(0, sendername, senderbetreff, senderText$, erhaltenAm, senderemailAdress,
                                                            reciepients, emailtyp)
        ' If clsEreignisDB.Neu_speichern_Ereignis() Then nachricht("Daten wurden gespeichert!")

    End Sub
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
    Private Shared Function makecr(senderText As String) As String
        Dim test As String
        test = senderText.Replace(vbCrLf, "#").Replace(vbCr, "#").Replace(vbLf, "#").Replace(vbTab, "#")
        test = test.Replace("## ##", vbCrLf)
        test = test.Replace("##", vbCrLf)
        test = test.Replace("#", "")
        Return test
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
End Class
