Module modKarteiBeteiligte



    Public Sub avoidNUlls()
        If String.IsNullOrEmpty(aktPerson.Kontakt.Anschrift.PostfachPLZ) Then aktPerson.Kontakt.Anschrift.PostfachPLZ = ""
        If String.IsNullOrEmpty(aktPerson.Name) Then aktPerson.Name = " "
        If String.IsNullOrEmpty(aktPerson.Vorname) Then aktPerson.Vorname = " "
        If String.IsNullOrEmpty(aktPerson.Bezirk) Then aktPerson.Bezirk = " "
    End Sub
    Function setSQLbody() As String
        Return " SET NACHNAME=:NACHNAME" & _
         ",VORNAME=:VORNAME" & _
         ",BEMERKUNG=:BEMERKUNG " & _
         ",NAMENSZUSATZ=:NAMENSZUSATZ " & _
         ",ANREDE=:ANREDE " & _
         ",QUELLE=:QUELLE " & _
         ",GEMEINDENAME=:GEMEINDENAME " & _
         ",STRASSE=:STRASSE " & _
         ",HAUSNR=:HAUSNR " & _
         ",PLZ=:PLZ" & _
         ",POSTFACH=:POSTFACH" & _
         ",POSTFACHPLZ=:POSTFACHPLZ" & _
         ",FFTELEFON1=:FFTELEFON1 " & _
         ",FFTELEFON2=:FFTELEFON2 " & _
         ",FFFAX1=:FFFAX1 " & _
         ",FFFAX2=:FFFAX2 " & _
         ",FFMOBILFON=:FFMOBILFON " & _
         ",FFEMAIL=:FFEMAIL " & _
         ",FFHOMEPAGE=:FFHOMEPAGE " & _
         ",GESELLFUNKTION=:GESELLFUNKTION " & _
         ",ORGNAME=:ORGNAME" & _
         ",ORGZUSATZ=:ORGZUSATZ" & _
         ",ORGTYP1=:ORGTYP1 " & _
         ",ORGTYP2=:ORGTYP2 " & _
         ",ORGEIGENTUEMER=:ORGEIGENTUEMER " & _
         ",ROLLE=:ROLLE " & _
         ",KASSENKONTO=:KASSENKONTO " & _
         ",BEZIRK=:BEZIRK " & _
         ",VORGANGSID=:VORGANGSID "
    End Function
    Sub setSQLParams(ByRef com As OracleCommand, ByVal vid%, ByVal lpers As LIBMensch.Person)
        avoidNUlls()
        Try

            With lpers
                com.Parameters.AddWithValue(":NACHNAME", .Name)
                com.Parameters.AddWithValue(":VORNAME", .Vorname)
                com.Parameters.AddWithValue(":BEMERKUNG", .Bemerkung.Trim)
                com.Parameters.AddWithValue(":NAMENSZUSATZ", .Namenszusatz.Trim)
                com.Parameters.AddWithValue(":ANREDE", .Anrede.Trim)
                com.Parameters.AddWithValue(":QUELLE", .Quelle.Trim)
                com.Parameters.AddWithValue(":GEMEINDENAME", .Kontakt.Anschrift.Gemeindename.Trim)
                com.Parameters.AddWithValue(":STRASSE", .Kontakt.Anschrift.Strasse.Trim)
                com.Parameters.AddWithValue(":HAUSNR", .Kontakt.Anschrift.Hausnr.Trim)
                com.Parameters.AddWithValue(":PLZ", .Kontakt.Anschrift.PLZ.ToString.Trim)
                com.Parameters.AddWithValue(":POSTFACH", .Kontakt.Anschrift.Postfach.Trim)
                com.Parameters.AddWithValue(":POSTFACHPLZ", .Kontakt.Anschrift.PostfachPLZ.Trim)
                com.Parameters.AddWithValue(":FFTELEFON1", .Kontakt.elektr.Telefon1.Trim)
                com.Parameters.AddWithValue(":FFTELEFON2", .Kontakt.elektr.Telefon2.Trim)
                com.Parameters.AddWithValue(":FFFAX1", .Kontakt.elektr.Fax1.Trim)
                com.Parameters.AddWithValue(":FFFAX2", .Kontakt.elektr.Fax2.Trim)
                com.Parameters.AddWithValue(":FFMOBILFON", .Kontakt.elektr.MobilFon.Trim)
                com.Parameters.AddWithValue(":FFEMAIL", .Kontakt.elektr.Email.Trim)
                com.Parameters.AddWithValue(":FFHOMEPAGE", .Kontakt.elektr.Homepage.Trim)
                com.Parameters.AddWithValue(":GESELLFUNKTION", .Kontakt.GesellFunktion.Trim)
                com.Parameters.AddWithValue(":ORGNAME", .Kontakt.Org.Name.Trim)
                com.Parameters.AddWithValue(":ORGZUSATZ", .Kontakt.Org.Zusatz.Trim)
                com.Parameters.AddWithValue(":ORGTYP1", .Kontakt.Org.Typ1.Trim)
                com.Parameters.AddWithValue(":ORGTYP2", .Kontakt.Org.Typ2.Trim)
                com.Parameters.AddWithValue(":ORGEIGENTUEMER", .Kontakt.Org.Eigentuemer.Trim)
                com.Parameters.AddWithValue(":ROLLE", .Rolle.Trim)
                com.Parameters.AddWithValue(":BEZIRK", .Bezirk.Trim)
                com.Parameters.AddWithValue(":KASSENKONTO", .Kassenkonto.Trim)
            End With
            com.Parameters.AddWithValue(":VORGANGSID", vid)
            '  com.Parameters.AddWithVALUE(":BVTITEL", myGlobalz.sitzung.aktPerson.Kontakt.Bankkonto.Titel.trim)
            '  com.Parameters.AddWithVALUE(":KONTONR", myGlobalz.sitzung.aktPerson.Kontakt.Bankkonto.KontoNr.trim)
            '  com.Parameters.AddWithVALUE(":BLZ", MYGLobalz.sitzung.aktPerson.Kontakt.Bankkonto.BLZ.trim)
            '  com.Parameters.AddWithVALUE(":BVNAME", MyGlobalz.sitzung.aktPerson.Kontakt.Bankkonto.Name.trim)
        Catch ex As Exception
            'glob2.nachricht("Fehler in setSQLParams beteiligte: " & ex.ToString)
        End Try
    End Sub

    Public Function Beteiligte_abspeichern_Neu() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        initconnection()
        Try
            beteiligterec.mydb.Tabelle = "BETEILIGTE"
            Dim SQLUPDATE$ = _
         String.Format("INSERT INTO {0} (VORGANGSID,NACHNAME,VORNAME,BEMERKUNG,NAMENSZUSATZ,ANREDE,QUELLE,GEMEINDENAME,STRASSE,HAUSNR,POSTFACH,POSTFACHPLZ,FFTELEFON1,FFTELEFON2,FFFAX1," +
                       "FFFAX2,FFMOBILFON,FFEMAIL,FFHOMEPAGE,GESELLFUNKTION,ORGNAME,ORGZUSATZ,ORGTYP1,ORGTYP2,ORGEIGENTUEMER,ROLLE,KASSENKONTO,PLZ,BEZIRK) " +
                               " VALUES (:VORGANGSID,:NACHNAME,:VORNAME,:BEMERKUNG,:NAMENSZUSATZ,:ANREDE,:QUELLE,:GEMEINDENAME,:STRASSE,:HAUSNR,:POSTFACH,:POSTFACHPLZ,:FFTELEFON1,:FFTELEFON2,:FFFAX1," +
                               ":FFFAX2,:FFMOBILFON,:FFEMAIL,:FFHOMEPAGE,:GESELLFUNKTION,:ORGNAME,:ORGZUSATZ,:ORGTYP1,:ORGTYP2,:ORGEIGENTUEMER,:ROLLE,:KASSENKONTO,:PLZ,:BEZIRK)",
                                 beteiligterec.mydb.Tabelle)

            SQLUPDATE$ = SQLUPDATE$ & " RETURNING PERSONENID INTO :R1"
            MeineDBConnection.Open()
            com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
            setSQLParams(com, vorgangid, aktPerson)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLUPDATE)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
        Catch ex As Exception
            'glob2.nachricht_und_Mbox("Bet5 Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

End Module
