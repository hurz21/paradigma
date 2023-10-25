Imports Devart.Data.Oracle

Namespace NSdbtools


    Public Class NSdbtools

        Private Shared Sub avoidNUlls()
            If String.IsNullOrEmpty(aktPerson.Kontakt.Anschrift.PostfachPLZ) Then aktPerson.Kontakt.Anschrift.PostfachPLZ = ""
            If String.IsNullOrEmpty(aktPerson.Name) Then aktPerson.Name = " "
            If String.IsNullOrEmpty(aktPerson.Vorname) Then aktPerson.Vorname = " "
        End Sub
        'Public Shared Function Stakeholder_loeschen(ByVal personenid As Integer, ByVal myrec As clsDBspecOracle) As Integer
        '    Dim anzahlTreffer&
        '    Dim newid& = -1
        '    Try
        '        If personenid > 0 Then
        '            myrec.mydb.Tabelle = "stakeholder"
        '            myrec.mydb.SQL = "delete from " & myrec.mydb.Tabelle & _
        '                                   " where  personenid=" & personenid
        '            anzahlTreffer = myrec.sqlexecute(newid) ', myGlobalz.mylog)
        '            If anzahlTreffer < 1 Then
        '                Return -1
        '            Else
        '                Return CInt(anzahlTreffer)
        '            End If
        '        Else
        '            l("  Entkoppelung_Stakeholder_Vorgang /  nicht Möglich")
        '            Return -3
        '        End If
        '    Catch ex As Exception
        '        l("fehler Entkoppelung_Stakeholder_Vorgang Problem beim Abspeichern: " & vbCrLf & ex.ToString)
        '        Return -2
        '    End Try
        'End Function

        Shared Sub l(ByVal text$)
            Debug.Print(text)
        End Sub

        'Public Shared Function Stakeholder_abspeichern_EditExtracted(ByVal lpers As Person) As Integer  'myGlobalz.sitzung.aktPerson.PersonenID
        '    Dim anzahlTreffer& = 0, hinweis$ = ""
        '    Dim com As OracleCommand
        '    Try
        '        If aktPerson.PersonenID < 1 Then
        '            l("FEHLER: updateid =0. Abbruch")
        '            Return 0
        '        End If
        '        ParaRec.mydb.Tabelle = "stakeholder"
        '        ParaRec.mydb.SQL = _
        '           "UPDATE  " & ParaRec.mydb.Tabelle & setSQLbody() & " WHERE PERSONENID=:PERSONENID"  'MYGLOBALZ.SITZUNG.AKTPERSON.PERSONENID

        '        Dim res As String = ""
        '        ParaRec.dboeffnen(res)
        '        com = New OracleCommand(ParaRec.mydb.SQL, ParaRec.myconn) ' myGlobalz.sitzung.personenRec.myconn)
        '        setSQLParams(com, lpers)
        '        com.Parameters.AddWithValue(":PERSONENID", aktPerson.PersonenID)
        '        anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '        ParaRec.myconn.Commit()
        '        ParaRec.myconn.Close()

        '        If anzahlTreffer < 1 Then
        '            l("Problem beim Abspeichern:" & ParaRec.mydb.SQL)
        '            Return -1
        '        Else
        '            Return CInt(anzahlTreffer)
        '        End If
        '    Catch ex As Exception
        '        l("Fehler beim Abspeichern: " & ex.ToString)
        '        Return -2
        '    End Try
        'End Function





        'Public Shared Function Stakeholder_abspeichern_Neu(ByVal lpers As Person) As Integer
        '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        '    Dim com As OracleCommand
        '    Try
        '        ParaRec.mydb.Tabelle = "Stakeholder"

        '        Dim SQLUPDATE$ = _
        '     String.Format("INSERT INTO {0} (VORGANGSID,NACHNAME,VORNAME,BEMERKUNG,NAMENSZUSATZ,ANREDE,QUELLE,GEMEINDENAME,STRASSE,HAUSNR,POSTFACH,POSTFACHPLZ,FFTELEFON1,FFTELEFON2,FFFAX1," +
        '                   "FFFAX2,FFMOBILFON,FFEMAIL,FFHOMEPAGE,GESELLFUNKTION,ORGNAME,ORGZUSATZ,ORGTYP1,ORGTYP2,ORGEIGENTUEMER,ROLLE,KASSENKONTO,PLZ,BEZIRK) " +
        '                           " VALUES (:VORGANGSID,:NACHNAME,:VORNAME,:BEMERKUNG,:NAMENSZUSATZ,:ANREDE,:QUELLE,:GEMEINDENAME,:STRASSE,:HAUSNR,:POSTFACH,:POSTFACHPLZ,:FFTELEFON1,:FFTELEFON2,:FFFAX1," +
        '                           ":FFFAX2,:FFMOBILFON,:FFEMAIL,:FFHOMEPAGE,:GESELLFUNKTION,:ORGNAME,:ORGZUSATZ,:ORGTYP1,:ORGTYP2,:ORGEIGENTUEMER,:ROLLE,:KASSENKONTO,:PLZ,:BEZIRK)",
        '                           ParaRec.mydb.Tabelle)

        '        SQLUPDATE$ = SQLUPDATE$ & " RETURNING PERSONENID INTO :R1"
        '        Dim res As String = ""
        '        ParaRec.dboeffnen(res)
        '        com = New OracleCommand(SQLUPDATE$, ParaRec.myconn) ' myGlobalz.sitzung.personenRec.myconn)
        '        setSQLParams(com, lpers)
        '        com.Parameters.AddWithValue(":VORGANGSID", 0)


        '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
        '        ParaRec.myconn.Close()
        '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
        '    Catch ex As Exception
        '        l("Fehler beim Abspeichern: " & ex.ToString)
        '        Return -2
        '    End Try
        'End Function


        Shared Function setSQLbody() As String
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
             ",BEZIRK=:BEZIRK " &
             ",VORGANGSID=:VORGANGSID"
        End Function

        'Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal lpers As Person)
        '    avoidNUlls()
        '    Try
        '        With lpers
        '            com.Parameters.AddWithValue(":NACHNAME", .Name)
        '            com.Parameters.AddWithValue(":VORNAME", .Vorname)
        '            com.Parameters.AddWithValue(":BEMERKUNG", .Bemerkung.Trim)
        '            com.Parameters.AddWithValue(":NAMENSZUSATZ", .Namenszusatz.Trim)
        '            com.Parameters.AddWithValue(":ANREDE", .Anrede.Trim)
        '            com.Parameters.AddWithValue(":QUELLE", .Quelle.Trim)
        '            com.Parameters.AddWithValue(":GEMEINDENAME", .Kontakt.Anschrift.Gemeindename.Trim)
        '            com.Parameters.AddWithValue(":STRASSE", .Kontakt.Anschrift.Strasse.Trim)
        '            com.Parameters.AddWithValue(":HAUSNR", .Kontakt.Anschrift.Hausnr.Trim)
        '            com.Parameters.AddWithValue(":PLZ", CInt(.Kontakt.Anschrift.PLZ.ToString.Trim))
        '            com.Parameters.AddWithValue(":POSTFACH", .Kontakt.Anschrift.Postfach.Trim)
        '            com.Parameters.AddWithValue(":POSTFACHPLZ", .Kontakt.Anschrift.PostfachPLZ.Trim)
        '            com.Parameters.AddWithValue(":FFTELEFON1", .Kontakt.elektr.Telefon1.Trim)
        '            com.Parameters.AddWithValue(":FFTELEFON2", .Kontakt.elektr.Telefon2.Trim)
        '            com.Parameters.AddWithValue(":FFFAX1", .Kontakt.elektr.Fax1.Trim)
        '            com.Parameters.AddWithValue(":FFFAX2", .Kontakt.elektr.Fax2.Trim)
        '            com.Parameters.AddWithValue(":FFMOBILFON", .Kontakt.elektr.MobilFon.Trim)
        '            com.Parameters.AddWithValue(":FFEMAIL", .Kontakt.elektr.Email.Trim)
        '            com.Parameters.AddWithValue(":FFHOMEPAGE", .Kontakt.elektr.Homepage.Trim)
        '            com.Parameters.AddWithValue(":GESELLFUNKTION", .Kontakt.GesellFunktion.Trim)
        '            com.Parameters.AddWithValue(":ORGNAME", .Kontakt.Org.Name.Trim)
        '            com.Parameters.AddWithValue(":ORGZUSATZ", .Kontakt.Org.Zusatz.Trim)
        '            com.Parameters.AddWithValue(":ORGTYP1", .Kontakt.Org.Typ1.Trim)
        '            com.Parameters.AddWithValue(":ORGTYP2", .Kontakt.Org.Typ2.Trim)
        '            com.Parameters.AddWithValue(":ORGEIGENTUEMER", .Kontakt.Org.Eigentuemer.Trim)
        '            com.Parameters.AddWithValue(":ROLLE", .Rolle.Trim)
        '            com.Parameters.AddWithValue(":KASSENKONTO", .Kassenkonto.Trim)
        '            com.Parameters.AddWithValue(":BEZIRK", .Bezirk.Trim)
        '            com.Parameters.AddWithValue(":VORGANGSID", 0)
        '        End With

        '        '  com.Parameters.AddWithVALUE(":BVTITEL", myGlobalz.sitzung.aktPerson.Kontakt.Bankkonto.Titel.trim)
        '        '  com.Parameters.AddWithVALUE(":KONTONR", myGlobalz.sitzung.aktPerson.Kontakt.Bankkonto.KontoNr.trim)
        '        '  com.Parameters.AddWithVALUE(":BLZ", MYGLobalz.sitzung.aktPerson.Kontakt.Bankkonto.BLZ.trim)
        '        '  com.Parameters.AddWithVALUE(":BVNAME", MyGlobalz.sitzung.aktPerson.Kontakt.Bankkonto.Name.trim)
        '    Catch ex As Exception
        '        l("Fehler in setSQLParams Stakeholder: " & ex.ToString)
        '    End Try

        'End Sub


    End Class
End Namespace
