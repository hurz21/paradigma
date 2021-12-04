Public Class clsMailsenden
    Private Shared mailserver_internet$, mailserver_intranet$, mailkonto_internet$, mailkonto_intranet$
    Private Shared mailpw_internet$, mailpw_intranet$
    'Private Shared inifile$ '= "c:\appsconfig\mail.xml"

    Public Overloads Shared Function mailrausSMTP(ByVal von$,
                                                 ByVal an$,
                                                 ByVal betreff$,
                                                 ByVal nachrichttext$,
                                                 ByVal anHang$,
                                                 ByVal iminternet As Boolean,
                                                 ByRef hinweis$,
                                                 ByVal inifile$,
                                                 ByVal CC As String) As Boolean
        'My.Log.WriteEntry("inmailraus1   iminternet:" & iminternet)
        '  nachricht("mailraus -------------------------")
        Dim test As Boolean = mailrausSMTP(von:=von,
                                            an:=an,
                                            betreff:=betreff,
                                            nachrichttext:=nachrichttext,
                                            anHang:=anHang,
                                            iminternet:=iminternet,
                                            mailserverinternet:="",
                                            mailserverintranet:="",
                                            hinweis:=hinweis,
                                            inifile:=inifile,
                                            CC:=CC)
        My.Log.WriteEntry("inmailraus1: " & test & vbCrLf & hinweis)
        Return test
    End Function

    Public Overloads Shared Function mailrausSMTP(ByVal von As String,
                                                 ByVal an As String,
                                                 ByVal betreff As String,
                                                 ByVal nachrichttext As String,
                                                 ByVal anHang As String,
                                                 ByVal iminternet As Boolean,
                                                 ByVal mailserverinternet As String,
                                                 ByVal mailserverintranet As String,
                                                 ByRef hinweis As String,
                                                 ByVal inifile As String,
                                                 ByVal CC As String) As Boolean
        Dim smtp As System.Net.Mail.SmtpClient
        Dim msg As System.Net.Mail.MailMessage
        Dim userid As String = ""
        Dim userpw As String = ""
        Dim mailserver As String = ""
        Try
            If iminternet Then
                If mailserverinternet$ = String.Empty Then
                    mailserver$ = mailserver_internet   '"mail.gmx.net"
                    userid = mailkonto_internet '"j.feinen@gmx.net"
                    userpw = mailpw_internet '"thuyan19"
                Else
                    mailserver$ = mailserverinternet$   '"mail.gmx.net"
                    userid = mailkonto_internet '"j.feinen@gmx.net"
                    userpw = mailpw_internet '"thuyan19"
                End If
            Else
                If mailserverintranet$ = String.Empty Then
                    mailserver$ = mailserver_intranet
                    userid = mailkonto_intranet '"dr.j.feinen@kreis-offenbach.de"
                    userpw = mailpw_intranet '"vinsan21"
                Else
                    mailserver$ = mailserverintranet$   '"mail.gmx.net"
                    userid = mailkonto_intranet '"dr.j.feinen@kreis-offenbach.de"
                    userpw = mailpw_intranet '"vinsan21"
                End If
            End If
            '  von = userid
            mailserver = CType(CLstart.mycSimple.iniDict("Beide.Mailserverintranet_standard"), String)
            Debug.Print(von)
            If String.IsNullOrEmpty(CType(CLstart.mycSimple.iniDict("Beide.Mailserverintranet_standard"), String)) Then Return False


            msg = New System.Net.Mail.MailMessage(von$, an$)
            msg.Subject = betreff$
            msg.Body = nachrichttext
            msg.IsBodyHtml = True
            ' Add a carbon copy recipient.
            If Not String.IsNullOrEmpty(CC) Then
                Dim copie As System.Net.Mail.MailAddress = New System.Net.Mail.MailAddress(CC)
                msg.CC.Add(copie)
            End If

            smtp = New System.Net.Mail.SmtpClient(mailserver, 25)
            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network

            smtp.Credentials = New System.Net.NetworkCredential(von, myGlobalz.AdminErrorMailPw)
            anhang_handhaben(anHang, msg)
            Return MailSenden(hinweis, smtp, msg)
        Catch ex As Exception
            nachricht("fehler in: mailrausSMTP ", ex)
            Return False
        End Try
    End Function

    Private Shared Function MailSenden(ByRef hinweis$, ByVal smtp As System.Net.Mail.SmtpClient, ByVal msg As System.Net.Mail.MailMessage) As Boolean
        Try
            '  nachricht("nach anhang")
            smtp.Send(msg)
            msg.Dispose()
            '  nachricht(" mailraus erfolgreich ---------------------------------------:")
            hinweis = "ok"
            smtp.Dispose()

            Return True
        Catch smt As Exception
            hinweis = "FEHLER in MAILSENDEN " & smt.Message + " " + smt.Source & smt.Message
            nachricht(" smt-exception: fehler1$:" & hinweis)
            Return False
        End Try
    End Function
    Private Shared Sub anhang_handhaben(ByRef anHang As String, ByVal msg As System.Net.Mail.MailMessage)
        Try
            If anHang.IsNothingOrEmpty() Then
                Exit Sub
            End If
            If anHang.EndsWith(",") Then anHang = anHang.Substring(0, anHang.Length - 1)
            If anHang.StartsWith(",") Then anHang = anHang.Substring(1, anHang.Length - 1)
            '  nachricht("vor anhang")
            If anHang.Length > 1 Then
                If anHang.Contains(",") Then
                    'mehrfachanhang
                    Dim filelist$() = anHang.Split(","c)
                    For Each datei As String In filelist
                        If Not String.IsNullOrEmpty(datei) Then msg.Attachments.Add(New System.Net.Mail.Attachment(datei)) '
                    Next
                Else
                    If Not String.IsNullOrEmpty(anHang) Then msg.Attachments.Add(New System.Net.Mail.Attachment(anHang)) '
                End If
            End If
        Catch ex As Exception
            nachricht("Fehler bei anhang: " & vbCrLf & anHang & vbCrLf ,ex)
        End Try
    End Sub
    'Public Shared Function initEnvironment_MAILPARAMS(ByRef mailserver_internet$, ByRef mailserver_intranet$,
    ' ByRef mailkonto_internet$, ByRef mailkonto_intranet$,
    ' ByRef mailpw_internet$, ByRef mailpw_intranet$,
    ' ByRef hinweis$, ByVal inifile As String) As Boolean
    '    '   Dim stmp As String = ""
    '    '	Dim myini As clsINIXMLParadigma
    '    If String.IsNullOrEmpty(inifile) Or inifile = "inifile" Then
    '        inifile = "c:\appsconfig\mail.xml"
    '        Dim test As New IO.FileInfo(inifile)
    '        If Not test.Exists Then
    '            inifile = myGlobalz.appdataDir & "mail.xml"
    '            '	inifile = myGlobalz.Paradigma_root &  "\Paradigma\mail.xml"
    '        End If
    '        test = New IO.FileInfo(inifile)
    '        If Not test.Exists Then
    '            inifile = myGlobalz.appdataDir & "mail.xml"
    '            nachricht("Fehler Mail-inifile nicht vorhanden! " & inifile)
    '            '	inifile = myGlobalz.Paradigma_root &  "\Paradigma\mail.xml"
    '            mailserver_intranet = CType(CLstart.mycSimple.iniDict("Beide.Mailserverintranet_standard"), String)
    '            nachricht("Abbruch mit standardanhame: " & mailserver_intranet)
    '            test = Nothing
    '            Return True
    '        End If
    '        test = Nothing
    '    End If

    '    nachricht("vor einlesen der xmldatei inifile: " & inifile)
    '    Dim myini = New CLstart.clsINIXML(inifile)
    '    nachricht("nach einlesen der xmldatei inifile: " & inifile)
    '    Try
    '        nachricht("vor mappen des dictionarys ")
    '        mailserver_internet$ = CType(myini.iniDict("INTERNET.mailserver"), String)
    '        nachricht("01")

    '        mailserver_intranet$ = CType(myini.iniDict("INTRANET.mailserver"), String)
    '        nachricht("04")
    '        mailkonto_intranet$ = CType(myini.iniDict("INTRANET.userid"), String)
    '        nachricht("05")
    '        mailpw_intranet$ = CType(myini.iniDict("INTRANET.userpw"), String)


    '        mailkonto_internet$ = CType(myini.iniDict("INTERNET.userid"), String)
    '        nachricht("02")
    '        mailpw_internet$ = CType(myini.iniDict("INTERNET.userpw"), String)
    '        nachricht("03")

    '        nachricht("nach mappen des dictionarys ")
    '        Return True
    '    Catch ex As Exception
    '        hinweis$ &= "Fehler: " & ex.Message
    '        nachricht(" hinweis$: " & hinweis$)
    '        Return False
    '    End Try
    'End Function

    Shared Function mailrausOutlook(ByVal an$, ByVal betreff$, ByVal nachricht$, ByVal anHang$, ByVal CC$,
                                    ByVal anhangtrenner As String, ByVal outlookAnzeigen As Boolean) As String

        Dim erfolg As String = MeinOutlook.meinsendmail(an, betreff, nachricht, CC, "", anHang, anhangtrenner, outlookAnzeigen)
        Return erfolg

    End Function
End Class