Imports System.Data
Public Class clsTools
    Public Shared ReadOnly tabBeteiligte As String = "T06" 'DB   "beteiligte" ' "t06" ' 
    'Public Shared Property perso As New PocoPerson
    Function getTitle(vid As String, jahr As String, nummer As String) As String
        Return "Probaug Schnitzstelle für Vorgang: " & vid & ", Jahr: " & jahr & ", Nummer: " & nummer & ", " & Environment.UserName
    End Function
    Function initGisview1(myconn As SqlClient.SqlConnection, sql As String) As DataTable
        Dim dt As New DataTable
        Dim obj As New clsMSSQL
        Try
            l(" MOD initGisview1 anfang")
            dt = obj.getDT(sql, myconn)
            l(" MOD ----------initGisview1------------ ende")
            Return dt
        Catch ex As Exception
            l("Fehler in MOD: " & ex.ToString())
            Return Nothing
        End Try
    End Function
    Function erstelleTextListe(gv1DT As DataTable) As String
        Dim tool As New clsTools
        Dim sb As New Text.StringBuilder
        Dim spaltenname() As String
        l("erstelleTextListe " & gv1DT.Rows.Count & " Treffer ")
        spaltenname = tool.getGV1Titel()
        For j = 0 To gv1DT.Rows.Count - 1
            sb.Append("################################" & Environment.NewLine)
            sb.Append("Es folgt der " & j & " Eintrag! " & " (" & gv1DT.Rows.Count & ")" & Environment.NewLine)
            sb.Append("################################" & Environment.NewLine)
            For i = 0 To gv1DT.Columns.Count - 1
                sb.Append(spaltenname(i) & ": " & vbTab & gv1DT.Rows(0).Item(i).ToString & Environment.NewLine)
            Next
        Next
        tool = Nothing
        Return sb.ToString
    End Function
    Sub getallParams(kommando As String, ByRef vid As String, ByRef jahr As String, ByRef nummer As String,
                      ByRef bid As String, ByRef bname As String, ByRef initial As String)
        If kommando.Contains("vid") Then
            Dim aaa As New clsTools
            aaa.getParams(kommando, vid, jahr, nummer, bid, bname, initial)
            '  Debug.Print(">>>> " & Environment.CommandLine)
        Else
            'vid = "33599"
            'vid = "33382"
            'jahr = "108755"
            'nummer = "108769" 
        End If
        l("vid " & vid)
        l("jahr " & jahr)
        l("nummer " & nummer)
        l("bid " & bid)
        l("bname " & bname)
    End Sub

    Friend Function makeADRESSEstring(adr As PocoAdresse) As String
        Dim summe As String = ""
        Try
            l(" MOD ---------------------- anfang")
            summe = summe & "Adresse --------------------" & Environment.NewLine
            summe = summe & "Gemeinde: " & adr.gemeindeName & Environment.NewLine
            summe = summe & "Strasse: " & adr.strasseName & " " & adr.HausKombi & Environment.NewLine
            l(" MOD ---------------------- ende")
            Return summe
        Catch ex As Exception
            l("Fehler in MOD: " & ex.ToString())
            Return summe
        End Try
    End Function

    Friend Function makeASstring(perso As PocoPerson) As String
        Dim summe As String = ""
        Try
            l(" MOD ---------------------- anfang")
            'summe = summe & "Adresse --------------------" & Environment.NewLine
            summe = summe & perso.Anrede & " " & perso.akademischerTitel & " " & perso.Vorname & " " & perso.Name & Environment.NewLine
            summe = summe & perso.Kontakt.Anschrift.Strasse & " " & perso.Kontakt.Anschrift.Hausnr & Environment.NewLine
            summe = summe & perso.Kontakt.Anschrift.PLZ & " " & perso.Kontakt.Anschrift.Gemeindename & " " & Environment.NewLine
            l(" MOD ---------------------- ende")
            Return summe
        Catch ex As Exception
            l("Fehler in MOD: " & ex.ToString())
            Return summe
        End Try
    End Function

    Friend Function makeFSTstring(fst As PocoFlurstueck) As String
        Dim summe As String = ""
        Try
            l(" MOD ---------------------- anfang")
            summe = summe & "Flurstück  --------------------" & Environment.NewLine
            summe = summe & "Gemeinde: " & fst.gemeindename & Environment.NewLine
            summe = summe & "Gemarkung: " & fst.gemarkungstext & Environment.NewLine
            summe = summe & "Flur: " & fst.flur & ", " & fst.fstueckKombi & Environment.NewLine
            l(" MOD ---------------------- ende")
            Return summe
        Catch ex As Exception
            l("Fehler in MOD: " & ex.ToString())
            Return summe
        End Try
    End Function

    Function getGV1Titel() As String()
        Dim feld(39) As String
        feld(0) = "Jahr "
        feld(1) = "Obergruppe "
        feld(2) = "lfd. Nr. "
        feld(3) = "Vorhaben Text 1 "
        feld(4) = "Vorhaben Text 2 "
        feld(5) = "Vorhaben Text 3 "
        feld(6) = "Vorhaben Text 4 "
        feld(7) = "Vorhaben "
        feld(8) = "Verfahrensart "
        feld(9) = "Vorhabensmerkmal "
        feld(10) = "Bauort "
        feld(11) = "Gemarkung "
        feld(12) = "Flur "
        feld(13) = "Flurstück "
        feld(14) = "Anrede "
        feld(15) = "Titel "
        feld(16) = "Zusatz 1 "
        feld(17) = "Zusatz 2 "
        feld(18) = "Vorname "
        feld(19) = "Nachname "
        feld(20) = "Straße "
        feld(21) = "Hausnummer "
        feld(22) = "PLZ "
        feld(23) = "Ort (KLAR) "
        feld(24) = "Straßenname (KLAR) "
        feld(25) = "Hausnummer "
        feld(26) = "OBJVG.HSchl "
        feld(27) = "Gemarkungsbezeichnung "
        feld(28) = "Hochwert "
        feld(29) = "Rechtswert "
        feld(30) = "zust. SB "
        feld(31) = "Ortsteil (KLAR) "
        feld(32) = "Ort (Katasterangaben KLAR) "
        feld(33) = "Ortsteil Antragsteller "
        feld(34) = "Kennziffer Verfahrensart "
        feld(35) = "Kennziffer Vorhaben "
        feld(36) = "Kennziffer Vorhaben-Merkmal "
        feld(37) = "Gemarkung_Kataster "
        feld(38) = "OrderId "
        Return feld
    End Function

    Friend Shared Function beteiligtenSpeichernNEU(perso As PocoPerson, initial As String, vid As Integer, dbtyp As String) As Integer
        Try
            l(" MOD beteiligtenSpeichernNEU anfang")
            Dim personenid As Integer = 0
            Dim querie As String
            werteDBsicherMachenBeteiligte(perso, initial)
            clsSqlparam.paramListe.Clear()
            querie = "INSERT INTO " & tabBeteiligte & " (VORGANGSID,NACHNAME,VORNAME,BEMERKUNG,NAMENSZUSATZ,ANREDE,QUELLE,GEMEINDENAME,STRASSE,HAUSNR,POSTFACH,POSTFACHPLZ,FFTELEFON1,FFTELEFON2,FFFAX1," +
                        "FFFAX2,FFMOBILFON,FFEMAIL,FFHOMEPAGE,GESELLFUNKTION,ORGNAME,ORGZUSATZ,ORGTYP1,ORGTYP2,ORGEIGENTUEMER,ROLLE,KASSENKONTO,PLZ,BEZIRK,LASTCHANGE,PERSONENVORLAGE,VERTRETENDURCH) " +
                        " VALUES (@VORGANGSID,@NACHNAME,@VORNAME,@BEMERKUNG,@NAMENSZUSATZ,@ANREDE,@QUELLE,@GEMEINDENAME,@STRASSE,@HAUSNR,@POSTFACH,@POSTFACHPLZ,@FFTELEFON1,@FFTELEFON2,@FFFAX1," +
                        "@FFFAX2,@FFMOBILFON,@FFEMAIL,@FFHOMEPAGE,@GESELLFUNKTION,@ORGNAME,@ORGZUSATZ,@ORGTYP1,@ORGTYP2,@ORGEIGENTUEMER,@ROLLE,@KASSENKONTO,@PLZ,@BEZIRK,@LASTCHANGE,@PERSONENVORLAGE,@VERTRETENDURCH)"

            populateBeteiligte(vid, perso, "mssql")
            Dim clsdb As New clsMSSQL
            personenid = clsdb.manipquerie(querie, clsSqlparam.paramListe, True, "PERSONENID")


            Return personenid
            l(" MOD beteiligtenSpeichernNEU ende")

        Catch ex As Exception
            l("Fehler in beteiligtenSpeichernNEU: " & ex.ToString())
            Return 0
        End Try
    End Function
    Shared Sub populateBeteiligte(vid As Integer, lpers As PocoPerson, dbtyp As String)
        With lpers
            clsSqlparam.paramListe.Add(New clsSqlparam("NACHNAME", .Name))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORNAME", .Vorname))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEMERKUNG", .Bemerkung.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("NAMENSZUSATZ", .Namenszusatz.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ANREDE", .Anrede.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", .Quelle.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("GEMEINDENAME", .Kontakt.Anschrift.Gemeindename.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("STRASSE", .Kontakt.Anschrift.Strasse.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("HAUSNR", .Kontakt.Anschrift.Hausnr.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("PLZ", .Kontakt.Anschrift.PLZ.ToString.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("POSTFACH", .Kontakt.Anschrift.Postfach.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("POSTFACHPLZ", .Kontakt.Anschrift.PostfachPLZ.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFTELEFON1", .Kontakt.elektr.Telefon1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFTELEFON2", .Kontakt.elektr.Telefon2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFFAX1", .Kontakt.elektr.Fax1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFFAX2", .Kontakt.elektr.Fax2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFMOBILFON", .Kontakt.elektr.MobilFon.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFEMAIL", .Kontakt.elektr.Email.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFHOMEPAGE", .Kontakt.elektr.Homepage.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("GESELLFUNKTION", .Kontakt.GesellFunktion.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGNAME", .Kontakt.Org.Name.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGZUSATZ", .Kontakt.Org.Zusatz.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGTYP1", .Kontakt.Org.Typ1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGTYP2", .Kontakt.Org.Typ2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGEIGENTUEMER", .Kontakt.Org.Eigentuemer.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ROLLE", .Rolle.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEZIRK", .Bezirk.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("KASSENKONTO", .Kassenkonto.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("LASTCHANGE",
                                                                                clsDBtools.makedateMssqlConform(Now, dbtyp)))

            clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENVORLAGE", .PersonenVorlage))
            clsSqlparam.paramListe.Add(New clsSqlparam("VERTRETENDURCH", .VERTRETENDURCH))
        End With
        'clsSqlparam.paramListe.Add(New clsSqlparam("KASSENKONTO", lpers.Kassenkonto))
        clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", vid))
    End Sub
    Shared Sub werteDBsicherMachenBeteiligte(lpers As PocoPerson, bearbeiterInitial As String)
        If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
        If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
        If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
        If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
        If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = bearbeiterInitial 'myGlobalz.sitzung.aktBearbeiter.Initiale

        If String.IsNullOrEmpty(lpers.Bemerkung.Trim) Then
            If lpers.Kontakt.elektr.Telefon1.Length > 240 Then
                lpers.Bemerkung = lpers.Kontakt.elektr.Telefon1.Substring(0, 240)
            Else
                lpers.Bemerkung = lpers.Kontakt.elektr.Telefon1
            End If

        End If
        If lpers.Namenszusatz.Length > 46 Then
            lpers.Namenszusatz = lpers.Namenszusatz.Substring(0, 45)
        End If

        If lpers.Kontakt.elektr.Telefon1.Length > 99 Then
            lpers.Kontakt.elektr.Telefon1 = lpers.Kontakt.elektr.Telefon1.Substring(0, 98)
        End If
    End Sub

    Sub getParams(commandLine As String, ByRef vid As String, ByRef jahr As String, ByRef nummer As String,
                  ByRef bid As String, ByRef bname As String, ByRef initial As String)
        Dim a$()
        Dim b$()
        l("getParams " & commandLine)
        Try
            a = commandLine.Split("/"c)
            b = a(1).Split("#"c)
            vid = b(0).Replace("vid=", "").Trim.Replace("#", "")
            jahr = a(2).Replace("jahr=", "").Trim.Replace("#", "")
            nummer = a(3).Replace("nummer=", "").Trim
            bid = a(4).Replace("bid=", "").Trim
            bname = a(5).Replace("bname=", "").Trim
            initial = a(6).Replace("initial=", "").Trim
        Catch ex As Exception
            l("fehler in getParams----------  e" & ex.ToString)
        End Try
    End Sub
    Shared Sub l(text As String)
        My.Log.WriteEntry(text)

    End Sub




    Sub initKatasterGemarkungtext(ByRef katastergem() As String)
        katastergem(0) = "Bieber                             ;725"
        katastergem(1) = "Buchschlag                         ;726"
        katastergem(2) = "Bürgel                             ;727"
        katastergem(3) = "Dietesheim                         ;728"
        katastergem(4) = "Dietzenbach                        ;729"
        katastergem(5) = "Dreieichenhain                     ;730"
        katastergem(6) = "Dudenhofen                         ;731"
        katastergem(7) = "Egelsbach                          ;732"
        katastergem(8) = "Froschhausen                       ;733"
        katastergem(9) = "Götzenhain                         ;734"
        katastergem(10) = "Hainhausen                         ;735"
        katastergem(11) = "Hainstadt                          ;736"
        katastergem(12) = "Hausen                             ;737"
        katastergem(13) = "Heusenstamm                        ;738"
        katastergem(14) = "Jügesheim                          ;739"
        katastergem(15) = "Klein-Krotzenburg                  ;740"
        katastergem(16) = "Klein-Welzheim                     ;741"
        katastergem(17) = "Lämmerspiel                        ;742"
        katastergem(18) = "Langen                             ;743"
        katastergem(19) = "Mainflingen                        ;744"
        katastergem(20) = "Messenhausen                       ;745"
        katastergem(21) = "Mühlheim                           ;746"
        katastergem(22) = "Nieder-Roden                       ;747"
        katastergem(23) = "Neu-Isenburg                       ;748"
        katastergem(24) = "Ober-Roden                         ;749"
        katastergem(25) = "Offenbach                          ;751"
        katastergem(26) = "Offenthal                          ;752"
        katastergem(27) = "Rembrücken                         ;753"
        katastergem(28) = "Rumpenheim                         ;754"
        katastergem(29) = "Seligenstadt                       ;755"
        katastergem(30) = "Sprendlingen                       ;756"
        katastergem(31) = "Urberach                           ;757"
        katastergem(32) = "Weiskirchen                        ;758"
        katastergem(33) = "Zellhausen                         ;759"
        katastergem(34) = "Zeppelinheim                       ;760"
        katastergem(35) = "Obertshausen                       ;750"

    End Sub
    Sub initProbaugNrProbaugGemarkungtext(ByRef gem() As String)
        gem(0) = "4	Dreieichenhain"
        gem(1) = "5	Sprendlingen"
        gem(2) = "6	Offenthal"
        gem(3) = "7	Götzenhain"
        gem(4) = "8	Buchschlag"
        gem(5) = "9	Hainstadt"
        gem(6) = "10 Klein-Krotzenburg"
        gem(7) = "11 Rembrücken"
        gem(8) = "12 Mainflingen"
        gem(9) = "13 Zellhausen"
        gem(10) = "14	Lämmerspiel"
        gem(11) = "15	Dietesheim"
        gem(12) = "16	Obertshausen"
        gem(13) = "17	Hausen"
        gem(14) = "18	Zeppelinheim"
        gem(15) = "20	Jügesheim"
        gem(16) = "21	Dudenhofen"
        gem(17) = "22	Nieder-Roden"
        gem(18) = "23	Hainhausen"
        gem(19) = "24	Weiskirchen"
        gem(20) = "25	Urberach"
        gem(21) = "26	Ober-Roden"
        gem(22) = "28	Messenhausen"
        gem(23) = "29	Froschhausen"
        gem(24) = "30	Klein-Welzheim"
        gem(25) = "32	Heusenstamm"
        gem(26) = "34	Seligenstadt"
        gem(27) = "35	Egelsbach"
        gem(28) = "36	Mühlheim"
        gem(29) = "40	Dietzenbach"
        gem(30) = "41	Langen"
        gem(31) = "42	Neu-Isenburg"
        gem(32) = "2	Bayerseich"
        gem(33) = "60	Im-Brühl"
        gem(34) = "27	Unbekannt27"
        gem(35) = "3	Unbekannt3"
        gem(36) = "33	Unbekannt33"
        gem(37) = "0	Unbekannt0"
    End Sub
    Function initgemeinde(gemeinde() As String) As String()
        gemeinde(0) = "1 ;Dietzenbach                        "
        gemeinde(1) = "2 ;Dreieich                           "
        gemeinde(2) = "3 ;Egelsbach                          "
        gemeinde(3) = "4 ;Hainburg                           "
        gemeinde(4) = "5 ;Heusenstamm                        "
        gemeinde(5) = "6 ;Langen                             "
        gemeinde(6) = "7 ;Mainhausen                         "
        gemeinde(7) = "8 ;Mühlheim                           "
        gemeinde(8) = "9 ;Neu-Isenburg                       "
        gemeinde(9) = "10;Obertshausen                       "
        gemeinde(10) = "0 ;Offenbach                          "
        gemeinde(11) = "11;Rodgau                             "
        gemeinde(12) = "12;Rödermark                          "
        gemeinde(13) = "13;Seligenstadt                       "
        'gemeinde(14) = "8 ;Muehlheim                          "
        Return gemeinde
    End Function

    Function splitgem(gem() As String) As Dictionary(Of Integer, String)
        Dim dict As New Dictionary(Of Integer, String)
        Dim a() As String
        For i = 0 To gem.Count - 1
            a = gem(i).Replace(vbTab, " ").Split(" "c)
            dict.Add(CInt(a(0).Trim), a(1).Trim)
        Next
        Return dict
    End Function
    Function splitKatasterGemarkung(katasterGem() As String) As List(Of myComboBoxItem)
        Dim dict As New List(Of myComboBoxItem)
        Dim a() As String
        Dim my As New myComboBoxItem
        For i = 0 To katasterGem.Count - 1
            my = New myComboBoxItem
            a = katasterGem(i).Replace(vbTab, " ").Split(";"c)
            my.myindex = a(1).Trim
            my.mySttring = (a(0).Trim)
            dict.Add(my)
        Next
        Return dict
    End Function
    Function splitgemeinde(gemeinde() As String) As Dictionary(Of Integer, String)
        Dim dict As New Dictionary(Of Integer, String)
        Dim a() As String
        For i = 0 To gemeinde.Count - 1
            a = gemeinde(i).Trim.Replace(vbTab, "").Split(";"c)
            dict.Add(CInt(a(0).Trim), a(1).Trim)
        Next
        Return dict
    End Function
    Sub machDicts(ByRef probaugGemarkungsdict As Dictionary(Of Integer, String), ByRef katasterGemarkungslist As List(Of myComboBoxItem), ByRef gemeindedict As Dictionary(Of Integer, String))
        Dim gem(37) As String
        Dim gemeinde(13) As String
        Dim katasterGem(35) As String

        gemeinde = initgemeinde(gemeinde)
        initKatasterGemarkungtext(katasterGem)
        initProbaugNrProbaugGemarkungtext(gem)

        katasterGemarkungslist = splitKatasterGemarkung(katasterGem)
        probaugGemarkungsdict = splitgem(gem)
        gemeindedict = splitgemeinde(gemeinde)
    End Sub
End Class
