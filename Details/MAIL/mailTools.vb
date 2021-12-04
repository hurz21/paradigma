Imports System.Data

Module mailTools

    Sub getOrganisationAndNameFromMailstring(ByRef sender As LIBmailmensch.mailMensch)

        Try
            If String.IsNullOrEmpty(sender.email) Then Exit Sub
            sender.email = sender.email.Replace("""", "").Trim
            If sender.email.Contains("<") Then
                MitKleinerals(sender)
            Else
                ohneKleinerals(sender)
            End If
        Catch ex As Exception
            nachricht("Fehler 3 in getOrganisationAndNameFromMailstring:  sender.email:" & sender.email & Environment.NewLine &
                            ex.ToString)
        End Try
    End Sub


    Private Sub ohneKleinerals(ByVal sender As LIBmailmensch.mailMensch)
        Dim a() As String
        Try
            sender.shortemail = sender.email.Replace("<", "").Replace(">", "").Trim
            If emailIstInternKreisOffenbach(sender.shortemail) Then
                sender.shortemail = clsOutlookEmail.makeInterneEmailAdresse(sender.name)
                sender.email = sender.shortemail
                sender.organization = "Kreis Offenbach"
                sender.literalname = sender.name
            Else
                If sender.shortemail.Contains("@") Then
                    a = sender.shortemail.Split("@"c)
                    sender.name = a(0).Trim
                    If a.Length = 1 Then
                        sender.organization = a(1)
                    Else
                        sender.organization = ""
                    End If
                    If isJustProvider(sender.organization) Then
                        sender.organization = ""
                    End If
                    sender.literalname = ""
                Else
                    sender.shortemail = ""
                    sender.email = ""
                    sender.organization = ""
                    sender.literalname = ""
                End If
            End If
        Catch ex As Exception
            nachricht("Fehler 3 in getOrganisationAndNameFromMailstring:  sender.email:" & sender.email & Environment.NewLine &
                  ex.ToString)
        End Try
    End Sub
    Private Sub MitKleinerals(ByVal sender As LIBmailmensch.mailMensch)
        Dim a(), b() As String

        Try
            a = sender.email.Split("<"c)
            sender.literalname = getLiteralpart(a(0))
            sender.shortemail = a(1).Replace(">", "").Trim
            b = sender.shortemail.Split("@"c)
            If Not String.IsNullOrEmpty(sender.literalname) Then
                sender.name = sender.literalname
            Else
                sender.name = b(0).Trim
            End If
            sender.organization = b(1).Trim
            If isJustProvider(sender.organization) Then
                sender.organization = ""
            End If
            sender.email = sender.shortemail
        Catch ex As Exception
            nachricht("Fehler 3 in getOrganisationAndNameFromMailstring:  sender.email:" & sender.email & Environment.NewLine &
                         ex.ToString)
        End Try
    End Sub
    Sub beteiligtenSenderAnlegen(ByVal person_name As String, ByVal organization As String, ByVal senderemailAdress As String,
                       ByVal rolle As String,
                       telefon As String)
        Dim neuePerson As New Person
        neuePerson.clear()
        neuePerson.Name = person_name
        neuePerson.Kontakt.Org.Name = organization
        neuePerson.Kontakt.elektr.Email = senderemailAdress
        neuePerson.Kontakt.elektr.Telefon1 = telefon
        neuePerson.Rolle = rolle ' 
        glob2.NeueBeteiligteAbspeichern(myGlobalz.sitzung.aktVorgangsID, neuePerson)
        '  Dim PersonenID% = clsBeteiligteBUSI.BeteiligteAbspeichernNeu_AlleDB(myGlobalz.sitzung.VorgangsID, neuePerson)
    End Sub

    Function emailIstSchonVorhanden(ByVal email As String) As Boolean
        Try
            For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
                If clsDBtools.fieldvalue(zeile.Item("FFEMAIL")).Trim.ToLower = email.Trim.ToLower Then
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            nachricht("Fehler in emailIstSchonVorhanden: " & ex.ToString)
            Return False
        End Try

    End Function
    'Function emailLeer(ByVal p1 As String) As Boolean
    '    If String.IsNullOrEmpty(p1) Then
    '        Return True
    '    Else
    '        Return False
    '    End If
    'End Function

    Function emailIstInternKreisOffenbach(ByVal emailadresse As String) As Boolean
        If emailadresse.Contains("/O=KREISOFFENBACH") Then
            Return True
        End If
    End Function

    Private Function getLiteralpart(ByVal teil1 As String) As String
        If String.IsNullOrEmpty(teil1) Then Return ""
        teil1 = teil1.Trim
        If teil1.Contains("@") Then
            Return ""
        End If
        Return teil1
    End Function

    Private Sub generateBlacklist(ByVal blacklist As List(Of String))
        blacklist.Add("t-online.de")
        blacklist.Add("gmx.de")
        blacklist.Add("web.de")
        blacklist.Add("gmx.net")
        blacklist.Add("arcor.de")
        blacklist.Add("hotmail.de")
        blacklist.Add("gmail.com")
        blacklist.Add("googlemail.com")
        blacklist.Add("yahoo.de")
        blacklist.Add("yahoo.com")
    End Sub

    Private Function isInBlacklist(ByVal organisation As String, ByVal blacklist As List(Of String)) As Boolean
        Try
            For Each eintrag In blacklist
                If organisation.ToLower.Contains(eintrag) Then
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            nachricht("Fehler  isInToBlacklist: " & ex.ToString)
        End Try
    End Function

    Private Function isJustProvider(ByVal organisation As String) As Boolean
        Dim blacklist As New List(Of String)
        generateBlacklist(blacklist)
        Return isInBlacklist(organisation, blacklist)
    End Function

    Sub SenderAlsBeteiligtenUebernehmen(ByVal gewuenscht As Boolean,
                                        ByVal senderemailAdress As String,
                                        ByVal person_name As String,
                                        ByVal organization As String,
                                        telefon As String)
        If gewuenscht Then
            If Not emailIstSchonVorhanden(senderemailAdress) Then
                mailTools.beteiligtenSenderAnlegen(person_name, organization, senderemailAdress, "EmailSender", telefon)
            End If
        End If
    End Sub

    Sub ReciepientsAlsBeteiligteUebernehmen(ByVal gewuenscht As Boolean, ByVal reciepients As List(Of LIBmailmensch.mailMensch))
        If gewuenscht Then
            For Each mm As LIBmailmensch.mailMensch In reciepients
                If Not emailIstSchonVorhanden(mm.email) Then
                    If emailIstInternKreisOffenbach(mm.email) Then
                        mm.email = clsOutlookEmail.makeInterneEmailAdresse(mm.email)

                    End If
                    mailTools.beteiligtenSenderAnlegen(mm.name, mm.organization, mm.email, "EmailEmpfänger", mm.telefon)
                End If
            Next
        End If
    End Sub
    Function emailVerteilerBilden(ByVal trenner As String) As String
        Dim summe As String = ""
        Try
            For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
                If Not String.IsNullOrEmpty(clsDBtools.fieldvalue(zeile.Item("FFEMAIL"))) Then
                    summe = summe & trenner & "" & clsDBtools.fieldvalue(zeile.Item("FFEMAIL"))
                End If
            Next
            If summe.StartsWith(trenner) Then
                summe = summe.Substring(1, summe.Length - 1)
            End If
            Return summe
        Catch ex As Exception
            nachricht("Fehler in emailVerteilerBilden: " & ex.ToString)
            Return ""
        End Try
    End Function

    Public Function anhangsdateienNachObj(list As List(Of String)) As List(Of AnhangsdateiAuswahl)
        Dim objListe As New List(Of AnhangsdateiAuswahl)
        Dim objekt As New AnhangsdateiAuswahl
        Dim fi As IO.FileInfo
        Try
            If list Is Nothing Then
                nachricht("Warnung in anhangsdateienNachObj 1: liste ist leeer ")
                Return Nothing
            End If
            For Each ele In list
                objekt = New AnhangsdateiAuswahl
                objekt.dateiname = ele
                fi = New IO.FileInfo(ele)
                objekt.dateinamekurz = fi.Name
                objekt.gewaehlt = True
                objListe.Add(objekt)
            Next
            Return objListe
        Catch ex As Exception
            nachricht("Fehler in anhangsdateienNachObj 2: " & ex.ToString)
            Return Nothing
        End Try
    End Function

    Function anhangEnthaeltFotos(anhang As String) As Boolean
        Try
            If anhang.ToLower.Contains(".jpg") Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("fehler in anhangEnthaeltFotos: " & ex.ToString)
            Return False
        End Try
    End Function

    Private Sub dateilisteErstellen(ByVal anhang As String, ByVal anhangtrenner As String, ByRef alleDateien As List(Of String))
        Dim a() As String
        a = anhang.Split(CChar(anhangtrenner))
        For i = 0 To a.GetUpperBound(0)
            If Not a(i).IsNothingOrEmpty Then alleDateien.Add(a(i))
        Next
    End Sub

    'Private Function wordsDokusRaussuchen(alleDateien As List(Of String)) As List(Of String)
    '    Dim nurWord As New List(Of String)
    '    For Each ele In alleDateien
    '        If expDokumente.istWordDatei(ele) Then
    '            nurWord.Add(ele)
    '        End If
    '    Next
    '    Return nurWord
    'End Function

    Sub worddokusNachPdfKonvertierenUndStringAnpassen(anhang As String, anhangtrenner As String)
        Dim alleDateien As New List(Of String)
        anhang = anhang.ToLower.Trim
        dateilisteErstellen(anhang, anhangtrenner, alleDateien)
        '  alleDateien = wordsDokusRaussuchen(alleDateien)
        alleDateien = konvertiereNachPDF(alleDateien)
        anhang = BildeanhangStringAusListe(alleDateien, myGlobalz.anhangtrenner)
        myGlobalz.sitzung.SendMail.Anhang = anhang
    End Sub

    Private Function konvertiereNachPDF(alleDateien As List(Of String)) As List(Of String)
        Dim konvertiten As New List(Of String)
        For Each ele In alleDateien
            If expDokumente.istWordDatei(ele) Then
                Dim lw = New LIBwordvorlage.WordReplaceTextmarken()
                If lw.dok2pdf(ele) Then
                    konvertiten.Add(lw.GetPDFOutfile(ele))
                    Else
                     konvertiten.Add(ele)
                End If
            Else
                konvertiten.Add(ele)
            End If
        Next
        Return konvertiten
    End Function

    Private Function BildeanhangStringAusListe(alleDateien As List(Of String), anhangtrenner As String) As String
        Dim a As String = ""
        For Each ele In alleDateien
            a = a & ele & anhangtrenner
        Next
        a = a.Substring(0, a.Length - 1)
        Return a
    End Function



End Module
