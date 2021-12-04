Module BearbeiterCRUD
    Sub userSpeichern(ByVal clsBearbeiter As clsBearbeiter, ByVal modus As String)
        If modus = "edit" Then
            BearbeiterAbspeichernEDIT_AlleDB(clsBearbeiter)
        End If
        If modus = "neu" Then
            BearbeiterAbspeichernNeu_AlleDB(clsBearbeiter)
        End If
    End Sub
    Private Sub DBsetzen()
        myGlobalz.temp_MYDB.Host = myGlobalz.vorgang_MYDB.Host
        myGlobalz.temp_MYDB.Schema = myGlobalz.vorgang_MYDB.Schema
        myGlobalz.temp_MYDB.Tabelle = CLstart.myViewsNTabs.tabBearbeiter
        myGlobalz.temp_MYDB.ServiceName = myGlobalz.vorgang_MYDB.ServiceName
        myGlobalz.temp_MYDB.password = myGlobalz.vorgang_MYDB.password
        myGlobalz.temp_MYDB.username = myGlobalz.vorgang_MYDB.username
        myGlobalz.temp_MYDB.dbtyp = myGlobalz.vorgang_MYDB.dbtyp
    End Sub
    Public Function BearbeiterAbspeichernNeu_AlleDB(ByVal lpers As clsBearbeiter) As Integer
        Dim personenid%
        DBsetzen()
        clsSqlparam.paramListe.Clear()
        Dim returnIdentity As Boolean = True
        Dim querie As String =
         "INSERT INTO " & CLstart.myViewsNTabs.tabBearbeiter & "  (USERNAME,NACHNAME,VORNAME,RANG,RITES,STDGRANTS,INITIAL_,AKTIV,ABTEILUNG,TELEFON,FAX," +
                       "KUERZEL1,NAMENSZUSATZ,EMAIL,ROLLE,EXPANDHEADERINSACHGEBIET,ANREDE) " +
                               " VALUES (@USERNAME,@NACHNAME,@VORNAME,@RANG,@RITES,@STDGRANTS,@INITIAL_,@AKTIV,@ABTEILUNG,@TELEFON,@FAX," +
                       "@KUERZEL1,@NAMENSZUSATZ,@EMAIL,@ROLLE,@EXPANDHEADERINSACHGEBIET,@ANREDE)"

        If lpers.STDGRANTS.Trim.IsNothingOrEmpty Then
            lpers.STDGRANTS = " "
        End If
        If lpers.Raum.Trim.IsNothingOrEmpty Then
            lpers.Raum = " "
        End If

        populateBearbeiter(lpers)
        personenid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "BEARBEITERID")
        'If (myglobalz.temp_MYDB.dbtyp = "oracle") Then
        '    Dim zzz As New bearbeiterORACLE(clsDBspecOracle.getConnection(myglobalz.temp_MYDB))
        '    personenid% = zzz.Bearbeiter_abspeichern_Neu(lpers)
        '    zzz.Dispose()
        'End If
        Return personenid
    End Function

    Private Sub populateBearbeiter(lpers As clsBearbeiter)
        With lpers
            clsSqlparam.paramListe.Add(New clsSqlparam("USERNAME", .username.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("NACHNAME", .Name.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORNAME", .Vorname.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("RANG", .Rang.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("RITES", .Raum.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("STDGRANTS", .STDGRANTS.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("INITIAL_", .Initiale.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("AKTIV", CInt(.Status)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ABTEILUNG", .Bemerkung))
            clsSqlparam.paramListe.Add(New clsSqlparam("TELEFON", .Kontakt.elektr.Telefon1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FAX", .Kontakt.elektr.Fax1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("KUERZEL1", .Kuerzel2Stellig.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("NAMENSZUSATZ", .Namenszusatz.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("EMAIL", .Kontakt.elektr.Email.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ROLLE", .Rolle.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("EXPANDHEADERINSACHGEBIET", .ExpandHeaderInSachgebiet.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ANREDE", .Anrede.Trim))
        End With
    End Sub
    Public Function BearbeiterAbspeichernEDIT_AlleDB(ByVal lpers As clsBearbeiter) As Integer
        Dim personenid%
        DBsetzen()
        Dim querie As String =
         "UPDATE  " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
          " SET USERNAME=@USERNAME" &
         ",VORNAME=@VORNAME" &
         ",NACHNAME=@NACHNAME " &
         ",RANG=@RANG " &
         ",RITES=@RITES " &
         ",STDGRANTS=@STDGRANTS " &
         ",INITIAL_=@INITIAL_ " &
         ",AKTIV=@AKTIV " &
         ",ABTEILUNG=@ABTEILUNG " &
         ",TELEFON=@TELEFON " &
         ",FAX=@FAX " &
         ",KUERZEL1=@KUERZEL1 " &
         ",NAMENSZUSATZ=@NAMENSZUSATZ " &
         ",EMAIL=@EMAIL " &
         ",ROLLE=@ROLLE " &
         ",EXPANDHEADERINSACHGEBIET=@EXPANDHEADERINSACHGEBIET " &
         ",ANREDE=@ANREDE " &
        " WHERE BEARBEITERID=@BEARBEITERID"  'MYGLOBALZ.SITZUNG.AKTPERSON.PERSONENID
        populateBearbeiter(lpers)
        clsSqlparam.paramListe.Add(New clsSqlparam("BEARBEITERID", CInt(lpers.ID)))
        personenid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "BEARBEITERID")
        Return personenid
    End Function
End Module
