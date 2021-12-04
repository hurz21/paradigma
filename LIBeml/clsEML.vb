
Imports CDO
Public Class clsEML
    Shared Sub getZusatzInfos(ByVal myMail As CDO.Message, ByRef cc As String, ByRef bcc As String, ByRef organisation As String, ByRef HTMLBody As String)
        ' die folgenden attribute werden nicht genutzt:
        Try
            cc = CStr(IIf(String.IsNullOrEmpty(myMail.CC), "", myMail.CC))
            bcc = CStr(IIf(String.IsNullOrEmpty(myMail.BCC), "", myMail.BCC))
            organisation = CStr(IIf(String.IsNullOrEmpty(myMail.Organization), "", myMail.Organization))

            'bcc = myMail.BCC
            'organisation = myMail.Organization
            HTMLBody = myMail.HTMLBody
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Function AnhangSpeichern(ByRef pfad As String,
                                           ByRef dateivorname As String,
                                           ByVal sender As mailMensch,
                                           ByRef senderbetreff As String,
                                           ByRef senderText As String,
                                           ByRef erhaltenAm As Date,
                                           ByRef createAttachmentFiles As Boolean,
                                           ByVal emlFullpath As String,
                                           ByRef cc As List(Of mailMensch),
                                           ByRef bcc As List(Of mailMensch),
                                           ByRef reciepients As List(Of mailMensch),
                                           ByRef telefon As String) As List(Of String)
        Dim strm As ADODB.Stream
        Dim mail As New CDO.Message
        Dim HTMLBody As String = ""
        Try
            If String.IsNullOrEmpty(emlFullpath) Then
                MsgBox("Es wurde keine eml übergeben! Abbruch!")
                My.Application.Log.WriteEntry("Es wurde keine eml übergeben!Abbruch! " & emlFullpath)
                Return Nothing
            End If
            Dim testfile As New IO.FileInfo(emlFullpath)
            If Not testfile.Exists Then
                My.Application.Log.WriteEntry("Warnung Die Datei existiert nicht: " & emlFullpath)
                testfile = Nothing
                Return Nothing
            Else
                My.Application.Log.WriteEntry("Die Datei existiert  : " & emlFullpath)
                testfile = Nothing
            End If
            strm = mail.GetStream()
            strm.Type = ADODB.StreamTypeEnum.adTypeText
            strm.LoadFromFile(emlFullpath)
            strm.Flush()
            senderbetreff = mail.Subject
            sender.name = mail.Sender
            erhaltenAm = mail.ReceivedTime
            senderText = mail.TextBody
            sender.email = mail.From
            sender.telefon = mailmenschTools.grabTelefonString(mail.TextBody)
            telefon = sender.telefon
            buildReciepientlist(mail.To, reciepients)

            'bis hierher alles wie bei outlook
            Dim ccString As String = ""
            Dim bccString As String = ""
            getZusatzInfos(mail, ccString, bccString, sender.organization, HTMLBody)

            buildReciepientlist(ccString, cc)
            buildReciepientlist(bccString, bcc)

            Dim dateien As New List(Of String)
            If createAttachmentFiles Then
                mailInDateienSpeichern(sender.name, mail, dateien, pfad, dateivorname, emlFullpath)
            Else
                dateien = Nothing
            End If
            mail = Nothing
            strm.Close()
            strm = Nothing
            Return dateien
        Catch ex As Exception
            My.Application.Log.WriteEntry(ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Shared Sub buildReciepientlist(ByVal mailto As String, ByVal reciepients As List(Of mailMensch))
        Dim newone As New mailMensch
        Dim a As String()
        Try
            reciepients.Clear()
            If String.IsNullOrEmpty(mailto) Then
                Exit Sub
            End If
            Dim liste As String()
            liste = mailto.Split(","c)
            If liste.Count > 0 Then
                For Each nase As String In liste
                    newone = New mailMensch

                    a = nase.Split("<"c)

                    nase = a(1).Replace(">", "").Trim

                    newone.name = ""
                    newone.email = nase
                    newone.shortemail = nase
                    reciepients.Add(newone)
                Next
            Else
                reciepients = Nothing
            End If

        Catch ex As System.Exception
            My.Application.Log.WriteEntry("Fehler: buildReciepientlist: " & ex.ToString)
        End Try
    End Sub

    Private Shared Sub mailInDateienSpeichern(ByVal sendername As String,
                            ByVal myMail As Message,
                            ByVal dateien As List(Of String),
                            ByVal pfad As String,
                            ByVal dateivorname As String,
                            ByVal emlFullpath As String)
        nachricht("mailInDateienSpeichern-----------------------------------------")
        nachricht("pfad$: " & pfad$)
        nachricht("Dateivorname$ :" & dateivorname$)
        Dim attach As IBodyPart
        Try
            MSGDateiSpeichernUndListen(sendername, myMail, dateien, pfad, dateivorname, emlFullpath)
            Dim testee As String
            For Each attach In myMail.Attachments
                testee$ = String.Format("{0}\{1}", pfad$, attach.FileName)
                attach.SaveToFile(testee$)
                dateien.Add(testee$)

                '   MainAnhangAlsDateiSpeichernUndListen(dateien, testee$)
            Next
        Catch ex As Exception

        End Try
    End Sub


    Private Shared Sub nachricht(ByVal p1 As String)
        My.Application.Log.WriteEntry(p1)
    End Sub

    Private Shared Sub MSGDateiSpeichernUndListen(ByVal sendername As String, ByVal myMail As Message,
                                                  ByVal dateien As List(Of String), ByVal pfad As String,
                                                  ByVal dateivorname As String,
                                                  ByVal emlFullpath As String)
        Try

            dateien.Add(emlFullpath)
            nachricht("MSGDateiSpeichern erfolgreich")

        Catch ex As System.Exception
            nachricht("Fehler in Maildateispeichern: " & ex.ToString)
        End Try
    End Sub




End Class
