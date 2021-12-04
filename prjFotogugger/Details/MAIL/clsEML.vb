
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
    Private Shared Sub l(text As String)
        My.Log.WriteEntry(text)
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
                                           ByRef telefon As String,
                                           ByRef problemMitAnhang As Boolean) As List(Of String)
        Dim strm As ADODB.Stream
        Dim mail As New CDO.Message
        Dim HTMLBody As String = ""
        Try
            If String.IsNullOrEmpty(emlFullpath) Then
                MsgBox("Es wurde keine eml übergeben! Abbruch!")
                l("Es wurde keine eml übergeben!Abbruch! " & emlFullpath)
                Return Nothing
            End If
            Dim testfile As New IO.FileInfo(emlFullpath)
            If Not testfile.Exists Then
                l("Warnung Die Datei existiert nicht: " & emlFullpath)
                testfile = Nothing
                Return Nothing
            Else
                l("Die Datei existiert  : " & emlFullpath)
                testfile = Nothing
            End If
            strm = mail.GetStream()
            strm.Type = ADODB.StreamTypeEnum.adTypeText
            strm.LoadFromFile(emlFullpath)
            strm.Flush()
            senderbetreff = mail.Subject
            sender.name = mail.Sender

            erhaltenAm = getDatumOrNow(mail)

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
                problemMitanhang = mailInDateienSpeichern(sender.name, mail, dateien, pfad, dateivorname, emlFullpath)
            Else
                dateien = Nothing
            End If
            mail = Nothing
            strm.Close()
            strm = Nothing
            Return dateien
        Catch ex As Exception
            l(ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Shared Function getDatumOrNow(mail As Message) As Date
        Dim erhaltenAm As Date

        If mail.ReceivedTime < CDate("1970-01-01") Then
            erhaltenAm = Now
        Else
            erhaltenAm = mail.ReceivedTime
        End If

        Return erhaltenAm
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
            l("Fehler: buildReciepientlist: " & ex.ToString)
        End Try
    End Sub

    Private Shared Function mailInDateienSpeichern(ByVal sendername As String,
                            ByVal myMail As Message,
                            ByVal dateien As List(Of String),
                            ByVal pfad As String,
                            ByVal dateivorname As String,
                            ByVal emlFullpath As String) As Boolean
        nachricht("mailInDateienSpeichern-----------------------------------------")
        nachricht("pfad$: " & pfad$)
        nachricht("Dateivorname$ :" & dateivorname$)
        Dim attach As IBodyPart
        Dim testee As String
        Try
            MSGDateiSpeichernUndListen(sendername, myMail, dateien, pfad, dateivorname, emlFullpath)
            IO.Directory.CreateDirectory(pfad)
            For Each attach In myMail.Attachments
                If attach.FileName.IsNothingOrEmpty Then
                    Return True 'fehler
                End If
                testee$ = String.Format("{0}\{1}", pfad, attach.FileName)
                If testee.EndsWith("=") Then testee = testee.Replace("=", "")
                If testee.Contains(vbCrLf) Then testee = testee.Replace(vbCrLf, "")
                If testee.Contains("UTF-8Q") Then testee = testee.Replace("UTF-8Q", "")
                If testee.Contains(" UTF-8Q ") Then testee = testee.Replace(" UTF-8Q ", "")
                If testee.Contains("UTF-8") Then testee = testee.Replace("UTF-8", "")
                'If testee.Contains("      ") Then testee = testee.Replace("     ", "")
                'If testee.Contains("     ") Then testee = testee.Replace("   ", "")
                'If testee.Contains("    ") Then testee = testee.Replace("   ", "")
                'If testee.Contains("   ") Then testee = testee.Replace("   ", "")
                'If testee.Contains("   ") Then testee = testee.Replace(" ", "")
                'If testee.Contains(" ") Then testee = testee.Replace(" ", "_")
                'If testee.Contains(" ") Then testee = testee.Replace("     ", "_")
                'If testee.Contains(" ") Then testee = testee.Replace("    ", "_")
                'If testee.Contains(" ") Then testee = testee.Replace("   ", "_")
                'If testee.Contains(" ") Then testee = testee.Replace("  ", "_")
                If testee.Contains(" ") Then testee = testee.Replace(" ", "_")
                If testee.Contains("__") Then testee = testee.Replace("__", "_")

                attach.SaveToFile(testee$)
                dateien.Add(testee$)
            Next
            Return False
        Catch ex As Exception
            l("fehler in maildateienspeichern" & ex.ToString)
            Return True
        End Try
    End Function


    Private Shared Sub nachricht(ByVal p1 As String)
        l(p1)
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

