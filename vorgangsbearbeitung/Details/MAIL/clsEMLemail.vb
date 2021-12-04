
'Imports System.Data
'Public Class clsEMLemail
'    Private _VSFinfotext As String
'    Private Const INT_Constant1maxCharBeschreibung As Integer = 500
'    Public Property fotoZuRaumbezug As Boolean
'    Property emlfullpath As String
'    Sub New(ByVal _emlfullpath As String)
'        emlfullpath = _emlfullpath
'    End Sub
'    Public Sub Aufnahme(ByVal fotoZuRaumbezug As Boolean, aktvid As Integer)
'        Dim sender As New LIBmailmensch.mailMensch
'        Dim senderbetreff$ = "", senderText$ = "", erhaltenAm As Date
'        Dim Beschreibung As String = ""
'        Dim createAttachmentFiles As Boolean = True, createEreignis As Boolean = True, emailBodyAlsDokument As Boolean = False
'        Dim reciepients As New List(Of LIBmailmensch.mailMensch), cc As New List(Of LIBmailmensch.mailMensch), bcc As New List(Of LIBmailmensch.mailMensch)

'        'test-----------
'        Dim dateien As List(Of String) = LIBeml.clsEML.AnhangSpeichern(
'                                            myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID,
'                                           "Email_" & myGlobalz.sitzung.aktVorgangsID & "_" & myGlobalz.sitzung.aktBearbeiter.Initiale & "_",
'                                            sender,
'                                            senderbetreff,
'                                            senderText,
'                                            erhaltenAm,
'                                            createAttachmentFiles,
'                                            emlfullpath,
'                                            cc,
'                                            bcc,
'                                            reciepients,
'                                            sender.telefon)


'        If clsOutlookEmail.emailEnthaeltVCF(dateien) Then
'            If clsOutlookEmail.istVCFinteressant(sender.email) Then
'                _VSFinfotext = "Diese Email enthält eine elektronische Visitenkarte (VCF-Datei)" & vbCrLf &
'            "Sie können daraus Beteiligte erzeugen, indem Sie die Datei unter" & vbCrLf &
'            "Dokumente anklicken und mit Paradigma öffnen!"
'            Else
'                _VSFinfotext = ""
'            End If
'        Else
'            _VSFinfotext = ""
'        End If

'        Dim anzahlDateien As Integer
'        If dateien IsNot Nothing Then
'            anzahlDateien = dateien.Count
'        End If
'        clsOutlookEmail.EreignisPreparieren(sender.name, senderbetreff, erhaltenAm, createEreignis, senderText, sender.email, reciepients)
'        ' Dim person_name As String, person_organisation As String, person_email As String = """"


'        Dim person_name As String = ""
'        ' mailTools.getOrganisationAndNameFromMailstring(sender.email, sender.organization, person_name, sender.email, literalname)
'        mailTools.getOrganisationAndNameFromMailstring(sender)


'        Dim recipientString As String = ""
'        For Each cand As LIBmailmensch.mailMensch In reciepients
'            '  mailTools.getOrganisationAndNameFromMailstring(cand.email, cand.organization, cand.name, cand.shortemail, cand.literalname)
'            mailTools.getOrganisationAndNameFromMailstring(cand)
'            recipientString &= cand.email & ";"
'        Next

'        Dim outl As New winOutlookEmailUebernehmen(anzahlDateien - 1, sender.name, senderbetreff, _VSFinfotext,
'                                                   sender.email, sender.organization, person_name,
'                                                   reciepients.Count, recipientString, Beschreibung,
'                                                   myGlobalz.sitzung.aktEreignis.Richtung, erhaltenAm, dateien, aktvid)

'        outl.ShowDialog()
'        If Not String.IsNullOrEmpty(outl.tbBeschreibung.Text) Then
'            myGlobalz.sitzung.aktEreignis.Beschreibung = LIBgemeinsames.clsString.noWhiteSpace(outl.tbBeschreibung.Text)
'        End If
'        fotoZuRaumbezug = outl.fotosalsRaumbezug
'        Dim result As Boolean = CBool(outl.DialogResult)
'        If Not result Then
'            'abbruch
'            Exit Sub
'        End If

'        mailTools.SenderAlsBeteiligtenUebernehmen(CBool(outl.chkSenderBeteiligtenanlegen.IsChecked), sender.email, person_name, sender.organization, sender.telefon)
'        mailTools.ReciepientsAlsBeteiligteUebernehmen(CBool(outl.chkRecipientsBeteiligtenanlegen.IsChecked), reciepients)

'        createEreignis = CBool(outl.mitVerlauf.IsChecked)
'        Dim erfolgEreingis As Boolean
'        If createEreignis Then
'            erfolgEreingis = clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", myGlobalz.sitzung.aktEreignis)
'            If Not (erfolgEreingis) Then
'                MsgBox("Ereignis konnte nicht erzeugt werden. Tipp: Falls die Email defekt ist, schicken Sie die Email an ihre eigene Emailadresse. Dann sollte der Import funktionieren!")
'            End If
'        End If
'        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
'                    myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
'        createAttachmentFiles = CBool(outl.mitAnhang.IsChecked)

'        emailBodyAlsDokument = CBool(outl.mitEmailBody.IsChecked)
'        Beschreibung = outl.tbSchlagworte.Text
'        If Not createAttachmentFiles And Not createEreignis Then Exit Sub

'        If outl.anhangsauswahlListe IsNot Nothing Then
'            If erfolgEreingis Then
'                clsOutlookEmail.MaildateienEincheckenUndMitEreignisKoppeln(outl.anhangsauswahlListe, sender.name, senderbetreff, senderText, erhaltenAm,
'                                                                   createAttachmentFiles, createEreignis,
'                                                                   emailBodyAlsDokument, Beschreibung, fotoZuRaumbezug,
'                                                                   erhaltenAm,
'                                                                   myGlobalz.sitzung.aktEreignis.ID, aktvid)
'            End If
'        Else
'            MessageBox.Show("Hinweis:" & vbCrLf & vbCrLf & "Sie müssen Outlook starten und die gewünschte Email öffnen!", "Übernahme von Emails aus Outlook", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
'        End If
'    End Sub


'End Class
