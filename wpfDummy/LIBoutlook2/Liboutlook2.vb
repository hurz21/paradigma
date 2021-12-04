Imports Microsoft.Office.Interop.Outlook

Public Class MeinOutlook
    ''' <summary>
    ''' verschickt die email
    ''' </summary>
    ''' <param name="EmailTo"></param>
    ''' <param name="Subject"></param>
    ''' <param name="MailBody"></param>
    ''' <param name="EmailCC"></param>
    ''' <param name="EmailBCC"></param>
    ''' <param name="Anhang"></param>
    ''' <returns>true erfolgreich</returns>
    ''' <remarks>false nciht erfolgreich</remarks>
    Public Shared Function meinsendmail(ByVal EmailTo As String, ByVal Subject As String,
                          ByVal MailBody As String,
                          ByVal EmailCC As String,
                          ByVal EmailBCC As String,
                          ByVal Anhang As String,
                          ByVal anhangtrenner As String,
                          ByVal outlookAnzeigen As Boolean) As String
        Try
            Dim lo_NSpace As Microsoft.Office.Interop.Outlook.NameSpace
            Dim lo_Folder As Microsoft.Office.Interop.Outlook.MAPIFolder
            Dim lo_OutApp As Microsoft.Office.Interop.Outlook.Application
            Dim lo_Item As Microsoft.Office.Interop.Outlook.MailItem





            nachricht("meinoutlook sendmail ---------------------------------------------------------------")
            lo_OutApp = New Microsoft.Office.Interop.Outlook.Application()
            lo_NSpace = lo_OutApp.GetNamespace("MAPI")
            lo_Folder = lo_NSpace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderSentMail)
            lo_Item = DirectCast(lo_Folder.Items.Add(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem), Microsoft.Office.Interop.Outlook.MailItem)
            lo_Item.[To] = EmailTo
            nachricht("1")
            If Not anhangerstellen(Anhang, lo_Item, anhangtrenner) = "0" Then
                Return "Anhang"
            End If
            nachricht("2")
            'Wenn man cc oder bcc verschicken will
            If Not String.IsNullOrEmpty(EmailCC) Then lo_Item.CC = EmailCC
            If Not String.IsNullOrEmpty(EmailBCC) Then lo_Item.BCC = EmailBCC
            nachricht("3")
            'wenn man möchte kann man noch Flaggen und Fälligkeit definieren
            'lo_Item.FlagStatus = Microsoft.Office.Interop.Outlook.OlFlagStatus.olFlagMarked;
            'lo_Item.FlagIcon = Microsoft.Office.Interop.Outlook.OlFlagIcon.olRedFlagIcon;
            'lo_Item.FlagDueBy = DateTime.Now.AddDays(7);

            lo_Item.Subject = Subject
            nachricht("4")
            'lo_Item.Body = MailBody
            'NachrichtenFormat
            lo_Item.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML
            lo_Item.HTMLBody = MailBody
            nachricht("5")
            'Anzeigen modal
            outlookAnzeigen = setOutlookAnzeigen(outlookAnzeigen, lo_Item)
            nachricht("6")
            'Senden der Mail
            If Not outlookAnzeigen Then lo_Item.Send()
            nachricht("7")
            lo_Item = Nothing
            lo_OutApp = Nothing
            lo_Folder = Nothing
            lo_NSpace = Nothing
            nachricht("meinoutlook sendmail ---------------------------ende------------------------------------")
            Return "0"
        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("1fehler meinsendmail: " & outlookAnzeigen)
            nachricht("fehler meinsendmail: " & comex.ToString)
            outlookAnzeigen = False
            Return comex.ToString
        Catch ex As System.Exception
            nachricht("Fehler in meinsendmail: " ,ex)
            Return "2Fehler in meinsendmail. Logfile anschauen"
        End Try
    End Function

    Private Shared Function setOutlookAnzeigen(outlookAnzeigen As Boolean, lo_Item As MailItem) As Boolean
        nachricht("outlookAnzeigen: " & outlookAnzeigen)
        Try
            lo_Item.Display(outlookAnzeigen)
            Return outlookAnzeigen
        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei outlookdisplay: " & outlookAnzeigen)
            nachricht("2fehler bei outlookdisplay: " & comex.ToString)
            'If comex.ErrorCode = CInt("0x80004005") Then
            '    MsgBox("Fehler:  Ein Dialogfeld ist geöffnet. Schließen Sie das Dialogfeld und versuchen Sie es erneut. ")
            'End If
            Return False
        Catch ex As System.Exception
            nachricht("afehler bei outlookdisplay: " & outlookAnzeigen)
            nachricht("a2fehler bei outlookdisplay: " ,ex)
            Return False
        End Try
    End Function

    Private Shared Function anhangerstellen(ByRef Anhang As String,
                                            ByVal lo_Item As Microsoft.Office.Interop.Outlook.MailItem,
                                            ByVal anhangtrenner As String) As String
        nachricht("anhangerstellen-----------------------------")
        Try
            If Not String.IsNullOrEmpty(Anhang) Then
                nachricht("a")
                Anhang = Anhang.Replace(anhangtrenner & anhangtrenner, anhangtrenner) : Anhang = Anhang.Replace(anhangtrenner & anhangtrenner, anhangtrenner) : Anhang = Anhang.Replace(anhangtrenner & anhangtrenner, anhangtrenner)
                If Anhang.EndsWith(anhangtrenner) Then
                    Anhang = Anhang.Substring(0, Anhang.Length - 1)
                End If
                nachricht("b")
                If Anhang.Contains(anhangtrenner) Then
                    'es sind mehrere anhänge!!!
                    Dim einzelnanhang As String() = Anhang.Split(CChar(anhangtrenner))
                    For Each datei In einzelnanhang
                        lo_Item.Attachments.Add(datei)
                        nachricht("addiert: " & datei)
                    Next
                Else
                    lo_Item.Attachments.Add(Anhang)
                    nachricht("c")
                End If
            End If
            nachricht("d")
            Return "0"

        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei anhangerstellen: a ")
            nachricht("2fehler bei anhangerstellen: " & comex.ToString)
            Return ""
        Catch ex As System.Exception
            nachricht("Fehler in anhangerstellen:  b " ,ex)
            Return "Fehler in anhangerstellen"
        End Try
    End Function
    Private Shared Sub MSGDateiSpeichernUndListen(ByRef sendername As String,
                                                  ByVal mail As Microsoft.Office.Interop.Outlook.MeetingItem,
                                                  ByVal dateien As List(Of String),
                                                  ByVal pfad As String,
                                                  ByVal Dateivorname As String)
        Dim dateil_fullname As String
        Dim speichernName As String
        Try
            nachricht("MSGDateiSpeichern -----------------------------")
            nachricht(String.Format("pfad$:{0}", pfad))
            nachricht(String.Format("Dateivorname${0}", Dateivorname))
            nachricht(String.Format("sendername:{0}", sendername))
            IO.Directory.CreateDirectory(pfad)
            nachricht("Pfad wurde erzeugt: " & pfad)

            speichernName = sendername.Replace("/", "_").Replace("-", "_").Replace(",", "_").Replace(" ", "_").Replace("__", "_").Replace(":", "_").Replace("*", "_").Replace("(", "_").Replace(")", "_")
            speichernName = speichernName.Replace(Chr(34), "").Replace(Chr(39), "").Replace("|", "_")
            speichernName = speichernName.Replace("?", "")
            Dateivorname = Dateivorname.Replace("?", "")
            dateil_fullname = String.Format("{0}\{1}{2}.msg", pfad, Dateivorname, speichernName)
            'MsgBox(dateil_fullname)
            nachricht("datei wird gespeichert: " & dateil_fullname)
            mail.SaveAs(dateil_fullname, OlSaveAsType.olMSG)
            nachricht("datei wurde gespeichert: " & dateil_fullname)
            dateien.Add(dateil_fullname)
            nachricht("MSGDateiSpeichernUndListen erfolgreich")
        Catch comex As System.Runtime.InteropServices.COMException
            'nachricht("fehler bei MSGDateiSpeichernUndListen1: ")
            nachricht("fehler bei MSGDateiSpeichernUndListen1: " & Dateivorname & " / " & sendername & " // " & pfad & " /// " & comex.ToString)
        Catch ex As System.Exception
            nachricht("Fehler in MSGDateiSpeichernUndListen2: " & Dateivorname & " / " & sendername & " // " & pfad & " /// ", ex)
        End Try
    End Sub
    Private Shared Sub MSGDateiSpeichernUndListen(ByRef sendername As String,
                                                  ByVal mail As Microsoft.Office.Interop.Outlook.MailItem,
                                                  ByVal dateien As List(Of String),
                                                  ByVal pfad As String,
                                                  ByVal Dateivorname As String)
        Dim dateil_fullname As String
        Dim speichernName As String
        Try
            nachricht("MSGDateiSpeichern -----------------------------")
            nachricht(String.Format("pfad$:{0}", pfad))
            nachricht(String.Format("Dateivorname${0}", Dateivorname))
            nachricht(String.Format("sendername:{0}", sendername))
            IO.Directory.CreateDirectory(pfad)
            nachricht("Pfad wurde erzeugt: " & pfad)

            speichernName = sendername.Replace("/", "_").Replace("-", "_").Replace(",", "_").Replace(" ", "_").Replace("__", "_").Replace(":", "_").Replace("*", "_").Replace("(", "_").Replace(")", "_")
            speichernName = speichernName.Replace(Chr(34), "").Replace(Chr(39), "").Replace("|", "_")
            speichernName = speichernName.Replace("?", "")
            Dateivorname = Dateivorname.Replace("?", "")
            dateil_fullname = String.Format("{0}\{1}{2}.msg", pfad, Dateivorname, speichernName)
            'MsgBox(dateil_fullname)
            nachricht("datei wird gespeichert: " & dateil_fullname)
            mail.SaveAs(dateil_fullname, OlSaveAsType.olMSG)
            nachricht("datei wurde gespeichert: " & dateil_fullname)
            dateien.Add(dateil_fullname)
            nachricht("MSGDateiSpeichernUndListen erfolgreich")
        Catch comex As System.Runtime.InteropServices.COMException
            'nachricht("fehler bei MSGDateiSpeichernUndListen1: ")
            nachricht("fehler bei MSGDateiSpeichernUndListen1: " & Dateivorname & " / " & sendername & " // " & pfad & " /// " & comex.ToString)
        Catch ex As System.Exception
            nachricht("Fehler in MSGDateiSpeichernUndListen2: " & Dateivorname & " / " & sendername & " // " & pfad & " /// " ,ex)
        End Try
    End Sub
    Private Shared Sub meetAnhangAlsDateiSpeichernUndListen(ByVal meet As Microsoft.Office.Interop.Outlook.MeetingItem,
                            ByRef dateien As List(Of String),
                            ByVal i As Integer,
                            ByVal testee As String)
        Try
            nachricht("MainAnhangAlsDateiSpeichernUndListen------------------------------")
            nachricht(String.Format("MainAnhangAlsDateiSpeichernUndListen i, testee:{0} {1}", i, testee))
            ' LIBgemeinsames.clsString.normalize_Filename(testee)
            testee = fallsSchonExistiertDannUmbenennen(testee)
            meet.Attachments.Item(i).SaveAsFile(testee)
            dateien.Add(testee)
            nachricht("MainAnhangAlsDateiSpeichernUndListen erfolgreich")
        Catch ex As System.Exception
            nachricht("Fehler in MainAnhangAlsDateiSpeichernUndListen: " & testee, ex)
        End Try
    End Sub
    Private Shared Sub MainAnhangAlsDateiSpeichernUndListen(ByVal mail As Microsoft.Office.Interop.Outlook.MailItem,
                            ByVal dateien As List(Of String),
                            ByVal i As Integer,
                            ByVal testee As String)
        Try
            nachricht("MainAnhangAlsDateiSpeichernUndListen------------------------------")
            nachricht(String.Format("MainAnhangAlsDateiSpeichernUndListen i, testee:{0} {1}", i, testee))
            ' LIBgemeinsames.clsString.normalize_Filename(testee)
            testee = fallsSchonExistiertDannUmbenennen(testee)
            mail.Attachments.Item(i).SaveAsFile(testee)
            dateien.Add(testee)
            nachricht("MainAnhangAlsDateiSpeichernUndListen erfolgreich")
        Catch ex As System.Exception
            nachricht("Fehler in MainAnhangAlsDateiSpeichernUndListen: " & testee ,ex)
        End Try
    End Sub
    Shared Function getTimestamp() As String
        Return Now.ToString("yyyy-MM-dd_HHmmss")
    End Function
    Public Shared Sub meetInDateienSpeichern(ByRef sendername As String,
               ByVal meet As Microsoft.Office.Interop.Outlook.MeetingItem,
               ByRef dateien As List(Of String),
               ByVal pfad As String,
               ByVal Dateivorname As String, nurAnhang As Boolean)
        nachricht("mailInDateienSpeichern-----------------------------------------")
        nachricht("pfad: " & pfad)
        nachricht("Dateivorname$ :" & Dateivorname$)
        Dim pfadExt As String = pfad & "\" & getTimestamp()
        nachricht("pfadExt " & pfadExt)
        Try
            IO.Directory.CreateDirectory(pfadExt)
        Catch ex2 As System.Exception

        End Try
        'mail.SaveAs(pfad$ & dateivorname$ & sendername & ".txt", OlSaveAsType.olTXT)
        'dateien.Add(pfad$ & dateivorname$ & sendername & ".txt")
        Try
            If nurAnhang Then
            Else
                MSGDateiSpeichernUndListen(sendername.Trim, meet, dateien, pfadExt, Dateivorname.Trim)
            End If
            Dim iAttachCnt As Integer
            Dim i As Integer
            Dim testee$
            If TypeName(meet).ToLower = "meetingitem" Then
                iAttachCnt = meet.Attachments.Count
                If iAttachCnt > 0 Then
                    For i = 1 To iAttachCnt
                        testee$ = String.Format("{0}\{1}", pfadExt, meet.Attachments.Item(i).FileName)
                        meetAnhangAlsDateiSpeichernUndListen(meet, dateien, i, testee)
                    Next i
                End If
            End If
            nachricht("anzahl aTTachments: " & meet.Attachments.Count)
            nachricht("mailInDateienSpeichern erfolgreich")
        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei mailInDateienSpeichern: ")
            nachricht("2fehler bei mailInDateienSpeichern: " & comex.ToString)
        Catch ex As System.Exception
            nachricht("Fehler in mailInDateienSpeichern: ", ex)
        End Try
    End Sub
    Public Shared Sub mailInDateienSpeichern(ByRef sendername As String,
               ByVal mail As Microsoft.Office.Interop.Outlook.MailItem,
               ByVal dateien As List(Of String),
               ByVal pfad As String,
               ByVal Dateivorname As String, nurAnhang As Boolean)
        nachricht("mailInDateienSpeichern-----------------------------------------")
        nachricht("pfad: " & pfad)
        nachricht("Dateivorname$ :" & Dateivorname$)
        Dim pfadExt As String = pfad & "\" & getTimestamp()
        nachricht("pfadExt " & pfadExt)
        Try
            IO.Directory.CreateDirectory(pfadExt)
        Catch ex2 As System.Exception

        End Try
        'mail.SaveAs(pfad$ & dateivorname$ & sendername & ".txt", OlSaveAsType.olTXT)
        'dateien.Add(pfad$ & dateivorname$ & sendername & ".txt")
        Try
            If nurAnhang Then
            Else
                MSGDateiSpeichernUndListen(sendername.Trim, mail, dateien, pfadExt, Dateivorname.Trim)
            End If
            Dim iAttachCnt As Integer
            Dim i As Integer
            Dim testee$
            If TypeName(mail) = "MailItem" Then
                iAttachCnt = mail.Attachments.Count
                If iAttachCnt > 0 Then
                    For i = 1 To iAttachCnt
                        testee$ = String.Format("{0}\{1}", pfadExt, mail.Attachments.Item(i).FileName)
                        MainAnhangAlsDateiSpeichernUndListen(mail, dateien, i, testee)
                    Next i
                End If
            End If
            nachricht("anzahl aTTachments: " & mail.Attachments.Count)
            nachricht("mailInDateienSpeichern erfolgreich")
        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei mailInDateienSpeichern: ")
            nachricht("2fehler bei mailInDateienSpeichern: " & comex.ToString)
        Catch ex As System.Exception
            nachricht("Fehler in mailInDateienSpeichern: " ,ex)
        End Try
    End Sub

    Private Shared Sub reportAuswerten(ByVal sender As mailMensch,
                                       ByVal rep As Microsoft.Office.Interop.Outlook.ReportItem,
                                      ByRef senderbetreff As String,
                                        ByRef erhaltenAm As Date,
                                        ByRef telefon As String,
                                        ByRef sendertext As String,
                                        ByRef createAttachmentFiles As Boolean)
        nachricht("objekt ist ein report, z.B. lese- oder empfangsbestätigung")
        Try
            createAttachmentFiles = False
            sendertext = rep.Body
            senderbetreff = rep.Subject
            erhaltenAm = rep.CreationTime
            If rep.Subject.Contains("Zugestellt") Then
                sender.name = ""
                senderbetreff = senderbetreff
                sendertext = "(Von Microsoft Outlook-Server zugestellt am: " & erhaltenAm & ") " & Environment.NewLine & rep.Body
            End If

            ' sender.name = rep.SenderName

            sender.organization = rep.Companies
            'sender.telefon = LIBmailmensch.tools.grabTelefonString(rep.Body) 
            'telefon = rep.telefon
            'buildReciepientlist(mail, reciepients)


        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei OutlookAnhangSpeichern report: ")
            nachricht("2fehler bei OutlookAnhangSpeichern report: " & comex.ToString)
        Catch ex As System.Exception
            l(ex.ToString)
        End Try
    End Sub
    Private Shared Function meetingItemAuswerten(pfad As String, dateivorname As String, ByVal sender As mailMensch,
                                       ByVal meet As Microsoft.Office.Interop.Outlook.MeetingItem,
                                      ByRef senderbetreff As String,
                                        ByRef erhaltenAm As Date,
                                        ByRef telefon As String,
                                        ByRef sendertext As String,
                                        ByRef createAttachmentFiles As Boolean) As List(Of String)
        nachricht("objekt ist ein report, z.B. lese- oder empfangsbestätigung")
        Try
            Dim meetApo As Microsoft.Office.Interop.Outlook.AppointmentItem = meet.GetAssociatedAppointment(False) ' TryCast(meet, Microsoft.Office.Interop.Outlook.AppointmentItem)
            createAttachmentFiles = True
            sendertext = meet.Body
            senderbetreff = meet.Subject
            erhaltenAm = meet.CreationTime
            If meet.Subject.Contains("Zugestellt") Then
                sender.name = ""
                senderbetreff = senderbetreff
                sendertext = "(Von Microsoft Outlook-Server zugestellt am: " & erhaltenAm & ") " & Environment.NewLine & meet.Body
            End If

            sender.name = meet.SenderName
            sender.email = meet.SenderEmailAddress
            sender.organization = meet.Companies


            sendertext = "Ort: " & meetApo.Location & vbCrLf & ", Wann: " & meet.ReminderTime.ToString & vbCrLf & ", Dauer: " & meetApo.Duration & " [min] " & vbCrLf & meet.Body

            Dim dateien As New List(Of String)
            If createAttachmentFiles Then
                meetInDateienSpeichern(sender.name, meet, dateien, pfad, dateivorname.Trim, nurAnhang:=False)
            Else
                dateien = Nothing
            End If

            meetApo = Nothing
            Return dateien
        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei OutlookAnhangSpeichern meeting: ")
            nachricht("2fehler bei OutlookAnhangSpeichern meeting: " & comex.ToString)
        Catch ex As System.Exception
            l(ex.ToString)
        End Try
    End Function
    Public Shared Function mailObjektAuswerten(pfad As String, ByRef dateivorname As String,
                                                ByVal sender As mailMensch,
                                                ByRef senderbetreff As String,
                                                ByRef senderText As String,
                                                ByRef erhaltenAm As Date,
                                                ByRef createAttachmentFiles As Boolean,
                                                ByRef reciepients As List(Of mailMensch),
                                                ByRef telefon As String,
                                                ByVal objOutlook As Microsoft.Office.Interop.Outlook._Application,
                                                ByVal mail As Microsoft.Office.Interop.Outlook.MailItem,
                                                  dateien As List(Of String),
                                                 ByRef aktvid As Integer,
                                                 ByRef istPARADIGMAemail As Boolean) As List(Of String)
        Try
            sender.name = mail.SenderName
            erhaltenAm = mail.ReceivedTime
            sender.organization = mail.Companies
            sender.telefon = mailmenschTools.grabTelefonString(mail.Body) : telefon = sender.telefon
            buildReciepientlist(mail, reciepients)
            senderbetreff = mail.Subject
            If senderbetreff Is Nothing Then
                senderbetreff = ""
            End If
            If senderbetreff.ToUpper.StartsWith("@PARADIGMA") Then
                Dim a = senderbetreff.ToUpper.Replace("@PARADIGMA#", "")
                Dim b() As String
                b = a.Split("#"c)
                aktvid = CInt(b(0))
                istPARADIGMAemail = True
            End If
            pfad = pfad & aktvid
            senderText = mail.Body
            If createAttachmentFiles Then
                mailInDateienSpeichern(sender.name, mail, dateien, pfad, dateivorname.Trim, nurAnhang:=False)
            Else
                dateien = Nothing
            End If

            sender.email = mail.SenderEmailAddress
            mail = Nothing
            deletetest(objOutlook)
            nachricht("meinoutlook OutlookAnhangSpeichern --------------------- ende mail --")
            Return dateien
        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei OutlookAnhangSpeichern mail: ")
            nachricht("2fehler bei OutlookAnhangSpeichern mail: " & comex.ToString)
            Return Nothing
        Catch ex As System.Exception
            l(ex.ToString)
            Return Nothing
        End Try
        Return Nothing
    End Function
    'Private Shared Sub deleteMailgruppenmarkierung2()
    '    Try
    '        Dim myexplorer As Outlook.Explorer = New Outlook.Explorer
    '        Dim mySelection As Outlook.Selection = myexplorer.Selection
    '        For Each tesrt As Outlook.Selection In mySelection
    '            tesrt = Nothing
    '        Next
    '        mySelection = Nothing
    '        myexplorer = Nothing
    '    Catch comex As System.Runtime.InteropServices.COMException
    '        nachricht("fehler bei deleteMailgruppenmarkierung2 : ")
    '        nachricht("2fehler bei deleteMailgruppenmarkierung2: " & comex.ToString)
    '    Catch ex As System.Exception
    '        nachricht("Fehler in deleteMailgruppenmarkierung2: " ,ex)
    '    End Try
    'End Sub


    'Private Shared Sub deleteMailgruppenmarkierung(ByVal mail As Microsoft.Office.Interop.Outlook.MailItem)
    '    Dim up As UserProperty = mail.UserProperties.Find("MyProp")
    '    Try
    '        If (up IsNot Nothing) Then
    '            up.Delete()
    '        End If
    '        mail.Save()

    '    Catch comex As System.Runtime.InteropServices.COMException
    '        nachricht("fehler bei deleteMailgruppenmarkierung: ")
    '        nachricht("2fehler bei deleteMailgruppenmarkierung: " & comex.ToString)
    '    Catch ex As System.Exception
    '        nachricht("Fehler in deleteMailgruppenmarkierung: " ,ex)
    '    End Try
    'End Sub
    ''' <summary>
    ''' emailbody wird als text und msg datei abgespeichert, der anhang im jeweils richtigen format
    ''' Die dateinamen werden als list zurückgegeben
    ''' (c) Dr. Feinen 2010
    ''' </summary>
    ''' <param name="pfad"></param>
    ''' <param name="dateivorname"></param>
    ''' <returns>liste der erzeugten dateien</returns>
    ''' <remarks>als list of (string)</remarks>
    Public Shared Function outlookEmailAuswerten(pfad As String,
                                            ByRef dateivorname As String,
                                            ByVal sender As mailMensch,
                                            ByRef senderbetreff As String,
                                            ByRef senderText As String,
                                            ByRef erhaltenAm As Date,
                                            ByRef createAttachmentFiles As Boolean,
                                            ByRef cc As List(Of mailMensch),
                                            ByRef bcc As List(Of mailMensch),
                                            ByRef reciepients As List(Of mailMensch),
                                            ByRef telefon As String,
                                            ByRef aktvid As Integer,
                                            ByRef istPARADIGMAemail As Boolean,
                                                 ByRef emailTyp As String,
                                              ByRef fehler As String
                                            ) As List(Of String)
        '   Dim dateien As List(Of String) = OutlookAnhangSpeichern("c:\", "nr12321!")
        nachricht("meinoutlook OutlookAnhangSpeichern ---------------------------------------------------------------")
        Dim idx As Integer = 0
        Dim objOutlook As Microsoft.Office.Interop.Outlook._Application
        idx = 1
        objOutlook = New Microsoft.Office.Interop.Outlook.Application
        idx = 2
        Dim mail As Microsoft.Office.Interop.Outlook.MailItem
        idx = 3
        fehler = ""
        idx = 4
        Dim dateien As New List(Of String)
        sender.organization = ""
        Try
            idx = 4
            If objOutlook.ActiveExplorer.Selection Is Nothing OrElse objOutlook.ActiveExplorer.Selection.Count < 1 Then
                l("4aa")
                l("fehler objOutlook.ActiveExplorer.Selection Is Nothing ")
                emailTyp = "email"

                fehler = "fehler: kein email-objekt ausgewählt"
                idx = 10
                objOutlook = Nothing
                mail = Nothing
                Return dateien
            End If
            If objOutlook.ActiveExplorer.Selection.Count > 1 Then
                nachricht("Mehr als eine Email war ausgewählt. Werden anschließend gelöscht!")
                'MsgBox(objOutlook.ActiveExplorer.Selection.Count)
                fehler = "fehler: mehr als ein email-objekt ausgewählt"
            End If
            mail = Nothing
            idx = 5

            If objektIstMail(mail, objOutlook.ActiveExplorer.Selection) Then
                idx = 9
                nachricht("objekt ist eine mail")
                dateien = mailObjektAuswerten(pfad, dateivorname, sender, senderbetreff, senderText, erhaltenAm, createAttachmentFiles,
                                              reciepients, telefon, objOutlook, mail, dateien, aktvid, istPARADIGMAemail)
                emailTyp = "email"
                idx = 10
                objOutlook = Nothing
                mail = Nothing
                Return dateien
            End If
            idx = 6
            Dim rep As Microsoft.Office.Interop.Outlook.ReportItem
            If objektIstReport(rep, objOutlook.ActiveExplorer.Selection) Then
                idx = 7
                reportAuswerten(sender, rep, senderbetreff, erhaltenAm, telefon, senderText, createAttachmentFiles)
                emailTyp = "report"
                rep = Nothing
                objOutlook = Nothing
                mail = Nothing
                Return Nothing
            End If
            rep = Nothing
            idx = 8
            'nachricht("Fehler in AnhangSpeichern: " & " Objekt ist weder mail noch report")

            Dim meet As Microsoft.Office.Interop.Outlook.MeetingItem
            If objektIstMeeting(meet, objOutlook.ActiveExplorer.Selection) Then
                idx = 81
                'Dim meetApo As Microsoft.Office.Interop.Outlook.AppointmentItem = TryCast(objOutlook.ActiveExplorer.Selection, Microsoft.Office.Interop.Outlook.AppointmentItem)
                idx = 82
                dateien = meetingItemAuswerten(pfad, dateivorname, sender, meet, senderbetreff, erhaltenAm, telefon, senderText, createAttachmentFiles)
                emailTyp = "meeting"
                meet = Nothing
                objOutlook = Nothing
                mail = Nothing
                Return dateien
            End If

        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei OutlookAnhangSpeichern hurz: " & idx & ",  " & comex.ToString)
            Return Nothing
        Catch ex As System.Exception
            l("fehler outlook system zugriff fehler " & idx & ", " ,ex)
            Return Nothing
        Finally
            l(" & idx sollte 8 sein " & idx)
            objOutlook = Nothing
            mail = Nothing
        End Try
    End Function

    Private Shared Function objektIstMeeting(ByRef rep As MeetingItem, selection As Selection) As Boolean
        Try
            rep = CType(selection.Item(1), Microsoft.Office.Interop.Outlook.MeetingItem)
            Return True
        Catch comex As System.Runtime.InteropServices.COMException
            Return False
        Catch ex As System.Exception
            Return False
        End Try
    End Function
    Public Shared Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = text & ToLogString(ex, text)
        'myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        'mitFehlerMail(text, anhang)
    End Sub
    Shared Sub nachricht(ByVal text$)
        l(text)
    End Sub
    Public Shared Function ToLogString(ByVal ex As System.Exception, ByVal additionalMessage As String) As String
        Dim msg As New Text.StringBuilder()

        If Not String.IsNullOrEmpty(additionalMessage) Then
            msg.Append(additionalMessage)
            msg.Append(Environment.NewLine)
        End If

        If ex IsNot Nothing Then
            Try
                Dim orgEx As System.Exception = ex
                msg.Append("Exception:")
                msg.Append(Environment.NewLine)
                While orgEx IsNot Nothing
                    msg.Append("Message: " & orgEx.Message)
                    msg.Append(Environment.NewLine)
                    orgEx = orgEx.InnerException
                End While

                If ex.Data IsNot Nothing Then
                    For Each i As Object In ex.Data
                        msg.Append("Data :")
                        msg.Append(i.ToString())
                        msg.Append(Environment.NewLine)
                    Next
                End If

                If ex.StackTrace IsNot Nothing Then
                    msg.Append("StackTrace:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.StackTrace.ToString())
                    msg.Append(Environment.NewLine)
                End If

                If ex.Source IsNot Nothing Then
                    msg.Append("Source:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.Source)
                    msg.Append(Environment.NewLine)
                End If

                If ex.TargetSite IsNot Nothing Then
                    msg.Append("TargetSite:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.TargetSite.ToString())
                    msg.Append(Environment.NewLine)
                End If

                Dim baseException As System.Exception = ex.GetBaseException()
                If baseException IsNot Nothing Then
                    msg.Append("BaseException:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.GetBaseException())
                End If
            Finally
            End Try
        End If
        Return msg.ToString()
    End Function
    'Public Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
    '    Dim anhang As String = ""
    '    text = text & ToLogString(ex, text)
    '    myGlobalz.sitzung.nachrichtenText = text
    '    My.Log.WriteEntry(text)
    '    'mitFehlerMail(text, anhang)
    'End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="subject"></param>
    ''' <param name="Body">"Ein Termin"</param>
    ''' <param name="Start">#9/5/2010 12:00:00 PM# 'Je nach Ländereinstellung</param>
    ''' <param name="duration">Dauer in Minuten z.b. 20</param>
    ''' <param name="alldayEvent"> ganztägig?</param>
    ''' <param name="sofortzeigen"></param>
    ''' <param name="OutlookSchliessen"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function OutlookTermin(ByVal subject$,
                            ByVal Body$,
                            ByVal Start As Date,
                            ByVal duration%,
                            ByVal alldayEvent As Boolean,
                            ByVal sofortzeigen As Boolean,
                            ByVal OutlookSchliessen As Boolean,
                            ByVal mitErinnerung As Boolean) As Boolean
        Try
            Dim objOutlook As Microsoft.Office.Interop.Outlook._Application
            Dim objFolder As Microsoft.Office.Interop.Outlook.MAPIFolder = Nothing
            l("OutlookTermin ----------- anfang")
            objOutlook = New Microsoft.Office.Interop.Outlook.Application

            Dim objNS As Microsoft.Office.Interop.Outlook._NameSpace = objOutlook.Session


            Dim objRecipient As Microsoft.Office.Interop.Outlook.Recipient = objNS.CreateRecipient("Dienstgang Umwelt")
            If objRecipient.Resolve Then
                objFolder = objNS.GetSharedDefaultFolder(objRecipient, Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar)
                'Console.Write(objFolder.Name)
                ' Else
                'Console.Write("Recipient could not be resolved.")
            Else
                nachricht("Fehler: Outlooktermin, objRecipient.Resolve nicht erfolgreich ")
            End If

            Dim termin As Microsoft.Office.Interop.Outlook.AppointmentItem

            termin = CType(objFolder.Items.Add(OlItemType.olAppointmentItem), AppointmentItem)
            If Not termin Is Nothing Then
                With termin
                    .ReminderSet = mitErinnerung
                    .AllDayEvent = alldayEvent
                    .Body = Body ' "Ein Termin"
                    .Duration = duration '20
                    .Start = Start
                    .Subject = subject$ '"Test"
                    If sofortzeigen Then termin.Display() 'Das Zeigt dir den Termin gleich an
                    termin.Save()
                End With
            End If


            ' termin.Close(vbCancel)

            termin.Close(OlInspectorClose.olDiscard)
            termin = Nothing
            If OutlookSchliessen Then objOutlook.Quit()
            objOutlook = Nothing
            l("OutlookTermin ----------- Ende")
            Return True

        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei OutlookTermin: ")
            nachricht("2fehler bei OutlookTermin: " & comex.ToString)
            Return False
        Catch ex As System.Exception
            l("Fehler: OutlookTermin ----------- ggf. war Outlook nicht gestartet: " ,ex)
            Return False
        End Try
    End Function

    Friend Shared Function msg2rtf(MSGfilename As String, outfile As String) As Boolean
        Dim oApp As New Microsoft.Office.Interop.Outlook.Application
        Dim OMItem As Microsoft.Office.Interop.Outlook.MailItem
        Dim dateien() As String
        Try
            nachricht("Reading the .msg file" & vbNewLine)
            OMItem = CType(oApp.CreateItemFromTemplate(MSGfilename), Microsoft.Office.Interop.Outlook.MailItem)
            nachricht("Writing as HTML file" & vbNewLine)
            Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(1252)
            'Dim enc As System.Text.Encoding = System.Text.Encoding.UTF8
            'Dim SW As IO.StreamWriter = New IO.StreamWriter(MSGfilename.Trim.Replace(".msg", "").Replace(".MSG", "") & ".html", False, enc)
            Dim SW As IO.StreamWriter = New IO.StreamWriter(outfile, False, enc)
            SW.WriteLine(" Von: " & OMItem.SenderName)
            SW.WriteLine(" An: " & OMItem.To)
            If Not OMItem.CC.IsNothingOrEmpty Then SW.WriteLine(" CC: " & OMItem.CC)
            If Not OMItem.BCC.IsNothingOrEmpty Then SW.WriteLine(" BCC: " & OMItem.BCC)
            SW.WriteLine(" Verschickt: " & OMItem.ReceivedTime)

            Dim neu As String = OMItem.SenderEmailAddress
            neu = neu.Replace("/O=KREISOFFENBACH/OU=EXCHANGE ADMINISTRATIVE GROUP", "")
            neu = neu.Replace("/CN=RECIPIENTS/CN=", "")
            SW.WriteLine(" Adresse: " & neu)
            SW.WriteLine(" Betreff: " & OMItem.Subject)
            'OMItem.BodyFormat = OlBodyFormat.olFormatRichText
            OMItem.BodyFormat = OlBodyFormat.olFormatPlain
            Dim text As String
            text = OMItem.Body
            'text = text.Replace(Environment.NewLine & Environment.NewLine, Environment.NewLine)
            'text = text.Replace(Environment.NewLine, "<br></br>")
            SW.Write(text)

            'MSGDateiSpeichernUndListen(OMItem.SenderName, OMItem, dateien, pfadExt, Dateivorname.Trim)

            SW.Close()
            'oApp.Quit()
            OMItem = Nothing
            'oApp = Nothing
            Return True
        Catch ex As System.Runtime.InteropServices.COMException
            MsgBox("Ist Outlook korrekt geöffnet? " & Environment.NewLine &
                   "Bitte schließen Sie ggf. den Mail-Mill Dialog." & Environment.NewLine &
                   "Bitte schließen Sie ggf. alle geöffneten Mails im Outlook! ", MsgBoxStyle.Exclamation)
            nachricht("fehler bei msg2html: ", ex)
            Return False
        Catch ex1 As System.Exception
            l("fehler " & ex1.ToString)
        Finally
            OMItem = Nothing
            oApp = Nothing
        End Try
    End Function

    Public Function OutlookTerminPersoenlich(ByVal subject$,
                        ByVal Body$,
                        ByVal Start As Date,
                        ByVal duration%,
                        ByVal alldayEvent As Boolean,
                        ByVal sofortzeigen As Boolean,
                        ByVal OutlookSchliessen As Boolean,
                        ByVal mitErinnerung As Boolean) As Boolean
        Try
            Dim objOutlook As Microsoft.Office.Interop.Outlook._Application
            '   Dim objFolder As Outlook.MAPIFolder
            l("WiedervorlageOutlookTermin ----------- anfang")
            objOutlook = New Microsoft.Office.Interop.Outlook.Application



            Dim termin As Microsoft.Office.Interop.Outlook.AppointmentItem
            termin = CType(objOutlook.CreateItem(OlItemType.olAppointmentItem), AppointmentItem)
            '  termin = objFolder.Items.Add(OlItemType.olAppointmentItem)

            With termin
                .ReminderSet = mitErinnerung
                .AllDayEvent = alldayEvent
                .Body = Body ' "Ein Termin"
                .Duration = duration '20
                .Start = Start
                .Subject = subject$ '"Test"
                If sofortzeigen Then termin.Display() 'Das Zeigt dir den Termin gleich an
                termin.Save()
            End With

            termin.Close(OlInspectorClose.olDiscard)
            termin = Nothing
            If OutlookSchliessen Then objOutlook.Quit()
            objOutlook = Nothing
            l("WiedervorlageOutlookTermin ----------- Ende")
            Return True

        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei OutlookTerminPersoenlich: ")
            nachricht("2fehler bei OutlookTerminPersoenlich: " & comex.ToString)
            Return False
        Catch ex As System.Exception
            l("Fehler: WiedervorlageOutlookTermin ----------- ggf. war Outlook nicht gestartet: ", ex)
            Return False
        End Try
    End Function

    'Dim oContact As Microsoft.Office.Interop.Outlook.ContactItem

    '' Kontakt-Ordner verwenden
    'oContact.LastName()

    Private Shared Sub deletetest(ByVal objOutlook As _Application)
        Try
            objOutlook.ActiveExplorer.CurrentView = Nothing
            '  objOutlook.Session.Categories = Nothing
            objOutlook = Nothing

        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei deletetest: ")
            nachricht("2fehler bei deletetest: " & comex.ToString)
        Catch ex As System.Exception
            l("Fehler: deletetest: ", ex)
        End Try
    End Sub

    Private Shared Sub buildReciepientlist(ByVal mail As MailItem, ByVal reciepients As List(Of mailMensch))
        Dim newone As New mailMensch
        Try
            reciepients.Clear()
            If mail.Recipients.Count > 0 Then
                For Each nase As Recipient In mail.Recipients
                    newone = New mailMensch
                    newone.name = nase.Name
                    newone.email = nase.Address
                    newone.shortemail = nase.Address
                    reciepients.Add(newone)
                Next
            Else
                reciepients = Nothing
            End If
        Catch comex As System.Runtime.InteropServices.COMException
            nachricht("fehler bei buildReciepientlist: ")
            nachricht("2fehler bei buildReciepientlist: " & comex.ToString)
        Catch ex As System.Exception
            l("Fehler: buildReciepientlist: ", ex)
        End Try
    End Sub

    'Private Shared Sub buildCClist(ByVal mail As MailItem, ByVal cc As List(Of mailMensch))
    '    Dim newone As New mailMensch
    '    Try
    '        cc.Clear()
    '        If mail.CC.Count > 0 Then
    '            For Each nase As MailItem.cc.item In mail.CC
    '                newone.name = nase.Name
    '                newone.email = nase.Address
    '                newone.shortemail = nase.Address
    '                cc.Add(newone)
    '            Next
    '        Else
    '            cc = Nothing
    '        End If
    '    Catch comex As System.Runtime.InteropServices.COMException
    '        nachricht("fehler bei buildReciepientlist: ")
    '        nachricht("2fehler bei buildReciepientlist: " & comex.ToString)
    '    Catch ex As System.Exception
    '        My.Application.Log.WriteEntry("Fehler: buildReciepientlist: " ,ex)
    '    End Try
    'End Sub

    Public Shared Function msg2html(MSGfilename As String, outfile As String) As Boolean
        Dim oApp As New Microsoft.Office.Interop.Outlook.Application
        Dim OMItem As Microsoft.Office.Interop.Outlook.MailItem
        Dim dateien() As String
        Try
            nachricht("Reading the .msg file" & vbNewLine)
            OMItem = CType(oApp.CreateItemFromTemplate(MSGfilename), Microsoft.Office.Interop.Outlook.MailItem)
            nachricht("Writing as HTML file" & vbNewLine)
            'Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(1252)
            Dim enc As System.Text.Encoding = System.Text.Encoding.UTF8
            'Dim SW As IO.StreamWriter = New IO.StreamWriter(MSGfilename.Trim.Replace(".msg", "").Replace(".MSG", "") & ".html", False, enc)
            Dim SW As IO.StreamWriter = New IO.StreamWriter(outfile, False, enc)
            SW.Write(" Von: " & OMItem.SenderName)
            SW.Write(" Adresse: " & OMItem.SenderEmailAddress)
            OMItem.BodyFormat = OlBodyFormat.olFormatHTML
            Dim text As String
            text = OMItem.Body
            text = text.Replace(Environment.NewLine & Environment.NewLine, Environment.NewLine)
            text = text.Replace(Environment.NewLine, "<br></br>")
            SW.Write(text)
            'MSGDateiSpeichernUndListen(OMItem.SenderName, OMItem, dateien, pfadExt, Dateivorname.Trim)

            SW.Close()
            oApp.Quit()
            OMItem = Nothing
            oApp = Nothing
            Return True
        Catch ex As System.Runtime.InteropServices.COMException
            MsgBox("Ist Outlook korrekt geöffnet? " & Environment.NewLine &
                   "Bitte schließen Sie ggf. den Mail-Mill Dialog." & Environment.NewLine &
                   "Bitte schließen Sie ggf. alle geöffneten Mails im Outlook! ", MsgBoxStyle.Exclamation)
            nachricht("fehler bei msg2html: ", ex)
            Return False
        Catch ex1 As System.Exception
            l("fehler " & ex1.ToString)
        Finally
            OMItem = Nothing
            oApp = Nothing
        End Try
    End Function

    Public Shared Function objektIstMail(ByRef mail As MailItem, selection As Selection) As Boolean
        Try
            mail = CType(selection.Item(1), Microsoft.Office.Interop.Outlook.MailItem)
            Return True

        Catch comex As System.Runtime.InteropServices.COMException
            Return False
        Catch ex As System.Exception
            Return False
        End Try
    End Function

    Private Shared Function objektIstReport(ByRef rep As ReportItem, selection As Selection) As Boolean
        Try
            rep = CType(selection.Item(1), Microsoft.Office.Interop.Outlook.ReportItem)
            Return True
        Catch comex As System.Runtime.InteropServices.COMException
            Return False
        Catch ex As System.Exception
            Return False
        End Try
    End Function

    Private Shared Function fallsSchonExistiertDannUmbenennen(testee As String) As String
        Dim Fziel As IO.FileInfo = New IO.FileInfo(testee)
        If Fziel.Exists Then
            testee = getNeuenNamenFuerDouble(testee, Fziel.DirectoryName).Trim
        End If
        Fziel = Nothing
        Return testee.Trim
    End Function
    Private Shared Function getNeuenNamenFuerDouble(ByRef dateiname As String, ByVal ZielGesamtpfad As String) As String ', ByRef ZielDateiFullName As String
        'function existiert auch in dokarc von paradigma
        Try
            Dim testname As String = dateiname
            Dim f As IO.FileInfo
            Dim ZielDateiFullName As String
            For i = 1 To 10000
                f = New IO.FileInfo(testname)
                ZielDateiFullName = ZielGesamtpfad & "\v" & i.ToString & "_" & f.Name
                Dim testt As New IO.FileInfo(ZielDateiFullName)
                If testt.Exists Then
                    f = Nothing
                    Continue For
                Else
                    f = Nothing
                    Return ZielDateiFullName
                End If
            Next
            nachricht("fehler in getNeuenNamenFuerDouble: schleife mit mehr als 10000 turns")
            Return dateiname
        Catch ex As System.Exception
            nachricht("Fehler: in	getNeuenNamenFuerDouble: " & dateiname & " " & vbCrLf ,ex)
            Return dateiname
        End Try
    End Function
    Public Shared Function getEmailAnhangDateien(ausgabePfad As String) As List(Of String)
        Dim oApp As Microsoft.Office.Interop.Outlook.Application = New Microsoft.Office.Interop.Outlook.Application
        Dim mail As Microsoft.Office.Interop.Outlook.MailItem
        Dim dateien As New List(Of String)
        Try
            nachricht(" MOD getEmailAnhangDateien anfang")
            oApp = New Microsoft.Office.Interop.Outlook.Application()
            If oApp.ActiveExplorer.Selection.Count > 0 Then
                If MeinOutlook.objektIstMail(mail, oApp.ActiveExplorer.Selection) Then
                    MeinOutlook.mailInDateienSpeichern("Dragdrop", mail, dateien, ausgabePfad, "AnhangScan", nurAnhang:=True)
                Else
                End If
            End If
            nachricht(" MOD getEmailAnhangDateien ende")
            Return dateien
        Catch ex As System.Exception
            nachricht("Fehler in getEmailAnhangDateien: ", ex)
            Return dateien
        End Try
    End Function
End Class

