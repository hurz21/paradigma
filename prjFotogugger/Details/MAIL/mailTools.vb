Imports System.Data

Module mailTools

    Sub getOrganisationAndNameFromMailstring(ByRef sender As mailMensch)
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
    Private Sub ohneKleinerals(ByVal sender As mailMensch)
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
    Private Sub MitKleinerals(ByVal sender As mailMensch)
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

    Function emailAdressIstSchonVorhanden(ByVal emailAdress As String) As Boolean
        l("emailAdressIstSchonVorhanden " & emailAdress)
        Try
            If emailAdress.IsNothingOrEmpty Then
                l("emailadress ist leer")
                Return True 'damit das nicht verwendet wird
            Else
                If myGlobalz.sitzung.beteiligteREC.dt Is Nothing Then Return False
                If myGlobalz.sitzung.beteiligteREC.dt.Rows.Count < 1 Then Return False
                For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
                    If clsDBtools.fieldvalue(zeile.Item("FFEMAIL")).Trim.ToLower = emailAdress.Trim.ToLower Then
                        Return True
                    End If
                Next
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler in emailAdressIstSchonVorhanden: " & emailAdress & "___" ,ex)
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

        Try
            l(" MOD ---------------------- anfang")
            If emailadresse.IsNothingOrEmpty Then
                Return False

            End If
            If emailadresse.Contains("/O=KREISOFFENBACH") Then
                Return True
            End If
            Return False
            l(" MOD ---------------------- ende")
            Return True
        Catch ex As Exception
            l("Fehler in MOD: " ,ex)
            Return False
        End Try
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
            nachricht("Fehler  isInToBlacklist: " ,ex)
            Return False
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
            If Not emailAdressIstSchonVorhanden(senderemailAdress) Then
                mailTools.beteiligtenSenderAnlegen(person_name, organization, senderemailAdress, "EmailSender", telefon)
            End If
        End If
    End Sub

    Sub ReciepientsAlsBeteiligteUebernehmen(ByVal gewuenscht As Boolean, ByVal reciepients As List(Of mailMensch))
        Try
            l(" MOD ReciepientsAlsBeteiligteUebernehmen anfang")
            If gewuenscht Then
                For Each mm As mailMensch In reciepients
                    If Not emailAdressIstSchonVorhanden(mm.email) Then
                        If emailIstInternKreisOffenbach(mm.email) Then
                            mm.email = clsOutlookEmail.makeInterneEmailAdresse(mm.email)
                        End If
                        If emailAdressIsValid(mm.email) Then
                            mailTools.beteiligtenSenderAnlegen(mm.name, mm.organization, mm.email, "EmailEmpfänger", mm.telefon)
                        End If
                    End If
                Next
            End If
            l(" MOD ReciepientsAlsBeteiligteUebernehmen ende")
        Catch ex As Exception
            l("Fehler in MOD: " ,ex)
        End Try
    End Sub

    Private Function emailAdressIsValid(email As String) As Boolean
        Try
            l(" MOD emailAdressIsValid anfang")
            email = email.Trim
            If email.Contains("#") Then Return False
            If email.Contains(" ") Then Return False
            If email.Contains("/") Then Return False

            Return True
            l(" MOD emailAdressIsValid ende")
            Return True
        Catch ex As Exception
            l("Fehler in emailAdressIsValid: " ,ex)
            Return False
        End Try
    End Function

    Function emailVerteilerBilden(ByVal trenner As String) As String
        Dim summe As String = ""
        Try
            If detail_dokuauswahl.anzahlAusgewaehlteBeteiligte(Psession.presBeteiligte) > 0 Then
                For Each perso As Person In Psession.presBeteiligte
                    If perso.ausgewaehlt Then
                        summe = summe & trenner & "" & clsDBtools.fieldvalue(perso.Kontakt.elektr.Email)      '
                    End If
                Next
                '    For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
                '    If Not String.IsNullOrEmpty(clsDBtools.fieldvalue(zeile.Item("FFEMAIL"))) Then
                '        summe = summe & trenner & "" & clsDBtools.fieldvalue(zeile.Item("FFEMAIL"))
                '    End If
                'Next
                If summe.StartsWith(trenner) Then
                    summe = summe.Substring(1, summe.Length - 1)
                End If
            End If
            Return summe
        Catch ex As Exception
            nachricht("Fehler in emailVerteilerBilden: " ,ex)
            Return ""
        End Try
    End Function

    Public Function anhangsdateienNachObj(list As List(Of String)) As List(Of AnhangsdateiAuswahl)
        Dim objListe As New List(Of AnhangsdateiAuswahl)
        Dim objekt As New AnhangsdateiAuswahl
        Dim fi As IO.FileInfo
        Try
            l("anhangsdateienNachObj")
            If list Is Nothing Then
                nachricht("nörmal in anhangsdateienNachObj 1: liste ist leeer ")
                Return Nothing
            End If
            For Each ele In list
                If ele.IsNothingOrEmpty Then Continue For
                l("ele " & (ele).Trim)
                objekt = New AnhangsdateiAuswahl
                'objekt.dateiname = LIBgemeinsames.clsString.normalize_Filename(ele).Trim
                objekt.dateiname = (ele).Trim
                Try
                    fi = New IO.FileInfo(objekt.dateiname)
                    'fi.Directory.Create(fi.DirectoryName)
                    IO.Directory.CreateDirectory(fi.DirectoryName)
                    'fi = Nothing
                Catch ex1 As Exception
                    l("Fehler Dateiname war nicht ok " & Environment.NewLine &
                       ex1.ToString & Environment.NewLine &
                        objekt.dateiname & Environment.NewLine &
                        fi.DirectoryName)
                    Continue For
                End Try
                objekt.dateinamekurz = fi.Name
                fi = Nothing
                objListe.Add(objekt)
            Next

            Return objListe
        Catch ex As Exception
            nachricht("Fehler in anhangsdateienNachObj 2: " & objekt.dateiname & "///" ,ex)
            Return Nothing
        End Try
    End Function
    Friend Sub setGewaehltStatus(temp As List(Of AnhangsdateiAuswahl))
        Try
            l(" MOD setGewaehltStatus anfang")
            For Each ele In temp
                If mailTools.isObsoleteImage(ele.dateinamekurz) Then
                    ele.gewaehlt = False
                Else
                    ele.gewaehlt = True
                End If
            Next
            l(" MOD setGewaehltStatus ende")
        Catch ex As Exception
            l("Fehler in setGewaehltStatus: " ,ex)
        End Try
    End Sub
    Friend Function isObsoleteImage(objekt As String) As Boolean
        Dim retval As Boolean = False
        Dim datei As String = objekt.Trim.ToLower
        Try
            l(" MOD isObsoleteImage anfang")
            If datei.EndsWith(".jpg") Or datei.EndsWith(".jpeg") Or datei.EndsWith(".png") Or datei.EndsWith(".gif") Then
                If datei.ToLower.StartsWith("image") Then
                    Return True
                Else
                    Return datei.ToLower.StartsWith("att0")
                End If
            Else
                Return False
            End If

            l(" MOD isObsoleteImage ende")
            Return False
        Catch ex As Exception
            l("Fehler in isObsoleteImage: " ,ex)
            Return False
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
            nachricht("fehler in anhangEnthaeltFotos: " ,ex)
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
                'Dim lw = New WordReplaceTextmarken()
                If wordInterop.dok2pdf(ele) Then
                    konvertiten.Add(WordReplaceTextmarken.GetPDFOutFilename(ele))
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

    Friend Sub KontrollausgabeMailEreignis(aktEreignis As clsEreignis)
        Try
            nachricht("KontrollausgabeEreignis--------------- ")
            nachricht("art: " & aktEreignis.Art)
            nachricht("Beschreibung: " & aktEreignis.Beschreibung)
            nachricht("Datum: " & aktEreignis.Datum)
            nachricht("DokumentID: " & aktEreignis.DokumentID)
            nachricht("ID: " & aktEreignis.ID)
            nachricht("istRTF: " & aktEreignis.istRTF)
            nachricht("Notiz: " & aktEreignis.Notiz)
            nachricht("Quelle: " & aktEreignis.Quelle)
            nachricht("Richtung: " & aktEreignis.Richtung)
            nachricht("KontrollausgabeEreignis--------------- ende")
        Catch ex As Exception
            nachricht("KontrollausgabeMailEreignis", ex)
        End Try
    End Sub

    Friend Sub KorrekturMailEreignis(aktEreignis As clsEreignis)
        Try
            nachricht("KorrekturMailEreignis--------------- ")
            If aktEreignis.Art.IsNothingOrEmpty() Then aktEreignis.Art = "Email"
            aktEreignis.Beschreibung = getKorrekturMailBeschreibung(aktEreignis.Beschreibung)

            aktEreignis.Datum = getKorrekturDatum(aktEreignis.Datum)
            ' If aktEreignis.DokumentID.IsNothingOrEmpty() Then aktEreignis.Art = "Email"
            'If aktEreignis.i.IsNothingOrEmpty() Then aktEreignis.Art = "Email"
            aktEreignis.Notiz = getKorrekturMailNotiz(aktEreignis.Notiz)

            If aktEreignis.Quelle.IsNothingOrEmpty() Then
                aktEreignis.Quelle = myGlobalz.sitzung.aktBearbeiter.getInitial
            End If
            If aktEreignis.Richtung.IsNothingOrEmpty() Then aktEreignis.Richtung = "Eingang"

            nachricht("KorrekturMailEreignis--------------- ende")
        Catch ex As Exception
            nachricht("KorrekturMailEreignis", ex)
        End Try
    End Sub

    Private Function getKorrekturMailBeschreibung(beschreibung As String) As String
        If beschreibung.IsNothingOrEmpty() Then beschreibung = " "
        If beschreibung.Length > 400 Then
            beschreibung = LIBgemeinsames.clsString.kuerzeTextauf(beschreibung, 400)
        End If
        Return beschreibung
    End Function

    Private Function getKorrekturMailNotiz(notiz As String) As String
        If notiz.IsNothingOrEmpty Then Return " "
        If notiz.Length > 3000 Then
            '  notiz = LIBgemeinsames.clsString.kuerzeTextauf(notiz, 2999)
            notiz = LIBgemeinsames.clsString.kuerzeTextauf(notiz, 2600)
            notiz = notiz & vbCrLf & "<<<< Email war zu lang! Hier wurde abgeschnitten. Die vollständige Email findet sich in der .MSG-Datei!"
        End If
        Return notiz
    End Function

    Private Function getKorrekturDatum(datum As Date) As Date
        Try
            If CDate(datum) < CDate("1970-01-01") Then
                Return Now
            End If
            Return CDate(datum)
        Catch ex As Exception
            Return Now
        End Try
    End Function

End Module
