Module modKarteAdresse
    Public Function RB_Adresse_abspeichern_Neu() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            initconnection()
            vorgangrec.mydb.Tabelle = "ParaAdresse"

            Dim SQLupdate$ =
            String.Format("INSERT INTO {0} (GEMEINDENR,GEMEINDETEXT,STRASSENNAME,STRCODE,FS,HAUSNRKOMBI,PLZ,POSTFACH,ADRESSTYP) " +
                                  " VALUES (:GEMEINDENR,:GEMEINDETEXT,:STRASSENNAME,:STRCODE,:FS,:HAUSNRKOMBI,:PLZ,:POSTFACH,:ADRESSTYP)",
                                 vorgangrec.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"

            MeineDBConnection.Open()
            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            SETSQLPARAMSADRESSERB(com, 0)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)
        Catch ex As Exception
            Return -2
        End Try
    End Function

    Function SETSQLBODYADRESSERB() As String
        Return " SET " & _
         " GEMEINDENR=:GEMEINDENR" & _
         ",GEMEINDETEXT=:GEMEINDETEXT" & _
         ",STRASSENNAME=:STRASSENNAME" & _
         ",STRCODE=:STRCODE" & _
         ",FS=:FS" & _
         ",HAUSNRKOMBI=:HAUSNRKOMBI" & _
         ",PLZ=:PLZ" & _
         ",POSTFACH=:POSTFACH" & _
         ",ADRESSTYP=:ADRESSTYP"
    End Function
    Sub SETSQLPARAMSADRESSERB(ByVal COM As OracleCommand, ByVal SEKID%)
        '	COM = NEW ORACLECOMMAND(MYGLOBALZ.SITZUNG.TEMPREC.MYDB.SQL, MYGLOBALZ.SITZUNG.TEMPREC.MYCONN)
        COM.Parameters.AddWithValue(":GEMEINDENR", paraadress.Gisadresse.gemeindeNrBig())
        COM.Parameters.AddWithValue(":GEMEINDETEXT", paraadress.Gisadresse.gemeindeName)
        COM.Parameters.AddWithValue(":STRASSENNAME", paraadress.Gisadresse.strasseName)
        COM.Parameters.AddWithValue(":STRCODE", paraadress.Gisadresse.strasseCode)
        COM.Parameters.AddWithValue(":FS", paraadress.FS)
        COM.Parameters.AddWithValue(":HAUSNRKOMBI", paraadress.Gisadresse.HausKombi)
        COM.Parameters.AddWithValue(":PLZ", CInt(paraadress.PLZ))
        COM.Parameters.AddWithValue(":POSTFACH", paraadress.Postfach)
        COM.Parameters.AddWithValue(":ADRESSTYP", CInt(paraadress.Adresstyp))
        '  com.Parameters.AddWithValue(":ID", sekid)
    End Sub
End Module
