Imports System.Data

Module kartei
    Public stamm As New Stamm
    Public aktPerson As New LIBMensch.Person
    Public paraadress As New ParaAdresse

    Public p() As String
    Public cnt As Integer = 0
    Public vorgangid As Integer = 0
    Public beteiligtenid As Integer = 0
    Public stammid As Integer = 0
    Public rbid As Integer = 0
    Public adrid As Integer = 0
    Public paraadressid As Integer = 0

    Private Property koppid As Integer = 0

    Sub procFile()
        'datei einlesen
        Dim roottdir, csvinput, csvprotokoll As String
        roottdir = "C:\Users\Feinen_j\Desktop\Paradigma\Archiv_Checkout\10195\"


        'bauantraege
        csvinput = roottdir & "KARTEI2.csv"
        csvinput = roottdir & "kleinerRest.csv"
        csvprotokoll = roottdir & "outkleinerRest.csv"

        'gaststaetten 5333
        csvinput = roottdir & "5333.csv"
        csvprotokoll = roottdir & "out5333.csv"


        'laermbeschwerden 5820
        csvinput = roottdir & "5820.csv"
        csvprotokoll = roottdir & "out5820.csv"


        'laermbeschwerden 5820
        csvinput = roottdir & "5322.csv"
        csvprotokoll = roottdir & "out5322.csv"


        Dim line As String
        Dim enc As Text.Encoding
        enc = Text.Encoding.Default
        Dim sw As New IO.StreamWriter(csvprotokoll)
        sw.AutoFlush = True
        Using sr As New IO.StreamReader(csvinput, enc)
            line = sr.ReadLine 'header
            line = sr.ReadLine 'header
            Do
                line = sr.ReadLine
                If line.IsNothingOrEmpty Then Exit Do
                p = line.Split(";"c)
                cnt += 1
                'If p.Length <> 22 Then Console.WriteLine(CStr(cnt), ";", line)
                If procZeileBauantrag(p) Then
                    sw.WriteLine("OK" & ";" & CStr(cnt) & ";" & vorgangid & ";" &
                                 stammid & ";" & beteiligtenid & ";" &
                                 adrid & ";" & rbid & ";" & koppid & ";" & line)
                Else
                    sw.WriteLine("--" & ";" & CStr(cnt) & ";" & vorgangid & ";" &
                                 stammid & ";" & beteiligtenid & ";" &
                                 adrid & ";" & rbid & ";" & koppid & ";" & line)
                End If
                ' End
            Loop
        End Using
    End Sub
     

    Private Sub makeneuesAZ(ByVal p As String(), ByVal vorgangid As Integer, ByVal bearbeiterkuerzelnr As Integer)
        Dim jahr As String = p(1)
        If jahr.Length > 2 Then
            jahr = jahr.Substring(2, 2)
        End If
        stamm.az.gesamt = "II-67-" & stamm.az.sachgebiet.Zahl & "-" & vorgangid & "-" & jahr & "-" & p(bearbeiterkuerzelnr).ToLower
    End Sub

    Private Function procZeileBauantrag(ByVal p As String()) As Boolean
        vorgangid = 0
        '  mod5333.stammobjekterzeugen()'1493
        ' mod5820.stammobjekterzeugen()'1494
        mod5322.stammobjekterzeugen() '1495
        makeneuesAZ(p, vorgangid, (11))
        vorgangid = vorgangAnlegenundVIDholen()
        If vorgangid < 1 Then
            Console.WriteLine(cnt & ": vorgang konnte nicht angelegt werden!!!")
            Return False
        End If
        stamm.ArchivSubdir = "\1\0\1495\VID".Replace("VID", vorgangid.ToString) ' & vid
        stamm.hatraumbezug = True
        makeneuesAZ(p, vorgangid, (11))
        If Not stammdatenanlegen() Then
            Return False
        End If
        beteiligtenid = beteiligten(p(10), p(7), p(8), p(9), "Beschwerdeführer/in")
        beteiligtenid = beteiligten(p(6), p(2), p(3), p(4), "Verursacher/in")
        RBadresse(p(3), p(5), p(2), p(4))
        Return True
    End Function

    Private Function vorgangAnlegenundVIDholen() As Integer
        vorgangid = 0
        Neu_speichern_Vorgang()
        Return vorgangid
    End Function

    Private Function stammdatenanlegen() As Boolean
        Return Neu_speichern_stammdaten(vorgangid)
    End Function

    Private Function beteiligten(ByVal name As String,
                                    ByVal plz As String,
                                    ByVal gemeinde As String,
                                    ByVal strasse As String,
                                    ByVal rolle As String) As Integer
        Try
            aktPerson.clear()
            aktPerson.Name = name ' CStr(p(6)) 'nachname
            If String.IsNullOrEmpty(plz.Trim) Then
                aktPerson.Kontakt.Anschrift.PLZ = "0"""
            Else
                aktPerson.Kontakt.Anschrift.PLZ = (plz) 'CInt(CStr(p(7)))
            End If

            aktPerson.Kontakt.Anschrift.Gemeindename = gemeinde ' CStr(p(8))
            aktPerson.Kontakt.Anschrift.Strasse = strasse ' CStr(p(9))
            aktPerson.Kontakt.Anschrift.Hausnr = ""

            Dim strasseneu As String = "", hausnr As String = ""
            If trennHausnrStrasse(aktPerson.Kontakt.Anschrift.Strasse, strasseneu, hausnr) Then
                aktPerson.Kontakt.Anschrift.Strasse = strasseneu
                aktPerson.Kontakt.Anschrift.Hausnr = hausnr
            End If

            aktPerson.Rolle = rolle
            If String.IsNullOrEmpty(aktPerson.Name) Then
                Return 0
            End If
            Return modKarteiBeteiligte.Beteiligte_abspeichern_Neu()
        Catch ex As Exception
            Console.WriteLine("Fehler in beteiligten: " & ex.ToString)
            Return -1
        End Try

    End Function

    Private Function RBadresse(ByVal gemeinde As String,
                                ByVal strasse As String,
                                ByVal plz As String,
                                ByVal freitext As String) As Integer
        Try
            adrid = 0 : rbid = 0
            paraadress.clear()
            paraadress.Gisadresse.gemeindeName = gemeinde ' CStr(p(3))
            paraadress.Gisadresse.strasseName = strasse ' CStr(p(4))
            paraadress.Gisadresse.HausKombi = " "

            Dim strasseneu As String = "", hausnr As String = ""
            If trennHausnrStrasse(paraadress.Gisadresse.strasseName, strasseneu, hausnr) Then
                paraadress.Gisadresse.strasseName = strasseneu
                paraadress.Gisadresse.HausKombi = hausnr
            End If


            paraadress.Adresstyp = 0
            paraadress.PLZ = CInt(CStr(plz)) 'p(2)))
            paraadress.Typ = RaumbezugsTyp.Adresse
            paraadress.Name = "Konzession"
            If String.IsNullOrEmpty(paraadress.Gisadresse.gemeindeName) Then Return -1
            If String.IsNullOrEmpty(paraadress.Gisadresse.strasseName) Then Return -1

            adrid = modKarteAdresse.RB_Adresse_abspeichern_Neu
            If adrid < 1 Then Return -1

            paraadress.SekID = adrid
            paraadress.abstract = paraadress.Name & ": " & paraadress.Gisadresse.gemeindeName & "," & paraadress.Gisadresse.strasseName
            paraadress.punkt.X = 0 : paraadress.punkt.Y = 0
            paraadress.box.xl = 0 : paraadress.box.yl = 0 : paraadress.box.xh = 0 : paraadress.box.yh = 0
            '  paraadress.Freitext = p(5) & ", Fl." & p(7) & ", Fst." & p(8)
            paraadress.Freitext = freitext


            rbid = modRB.Raumbezug_abspeichern_Neu(paraadress)
            If rbid < 1 Then Return -1
            'koppeln mit vorgang
            koppid = Koppelung_Raumbezug_Vorgang(rbid, vorgangid, 0)
            Return koppid
        Catch ex As Exception
            Console.WriteLine("Fehler in RBadresse: " & ex.ToString)
            Return -1
        End Try
    End Function

    Public Function trennHausnrStrasse(ByVal strhn As String, ByRef strasseneu As String, ByRef hausnrkombineu As String) As Boolean
        If String.IsNullOrEmpty(strhn) Then Return False
        strhn = strhn.Trim
        If String.IsNullOrEmpty(strhn) Then Return False
        Dim test As String
        Try
            For i = 0 To strhn.Length - 1
                test = strhn.Substring(i, 1)
                If IsNumeric(test) Then
                    strasseneu = strhn.Substring(0, i - 1).Trim
                    hausnrkombineu = strhn.Substring(i - 1, strhn.Length - (i - 1)).Trim
                    Console.WriteLine("geändert: " & strasseneu)
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            'Console.WriteLine("Fehler in trennHausnrStrasse: " & ex.ToString)
            Console.WriteLine("Fehler in trennHausnrStrasse: " & strhn)
            Return False
        End Try
    End Function

End Module

