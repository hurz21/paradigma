Imports System.Data

Module naturegTools
    Sub populateNatureg(ByVal nat As clsNatureg, paramListe As List(Of clsSqlparam))
        Try
            With nat
                paramListe.Add(New clsSqlparam("VORGANGSID", .VorgangsID))
                paramListe.Add(New clsSqlparam("NUMMER", .nummer))
                paramListe.Add(New clsSqlparam("ART", .art))
                paramListe.Add(New clsSqlparam("TYP", .typ))
                paramListe.Add(New clsSqlparam("BESCHREIBUNG", .beschreibung))
                paramListe.Add(New clsSqlparam("MASSNAHMENNR", .MassnahmenNr))
                paramListe.Add(New clsSqlparam("QUELLE", .Quelle.Trim))
                paramListe.Add(New clsSqlparam("NOTIZ", .notiz.Trim))

            End With
            '  com.Parameters.AddWithValue(":VORGANGSID", vid)
        Catch ex As Exception
            nachricht("Fehler in setSQLParams beteiligte: " ,ex)
        End Try
    End Sub
    Public Function naturegAbspeichernNeu_AlleDB(ByVal lnat As clsNatureg) As Integer
        Dim natid%
        Dim querie As String
        'werteDBsicherMachenEreignis(ereignis)
        clsSqlparam.paramListe.Clear()
        populateNatureg(lnat, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New LibDB.sqlparam("eid", 0))
        querie = "INSERT INTO  " & CLstart.myViewsNTabs.tabNatureg & "  (VORGANGSID,NUMMER,ART,TYP,BESCHREIBUNG,QUELLE,NOTIZ,MASSNAHMENNR) " +
                               " VALUES (@VORGANGSID,@NUMMER,@ART,@TYP,@BESCHREIBUNG,@QUELLE,@NOTIZ,@MASSNAHMENNR)"
        natid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
        Return natid
    End Function

    Public Function naturegAbspeichernEdit_AlleDB(ByVal lnat As clsNatureg) As Integer
        Dim anzahl As Integer

        Dim querie As String
        'werteDBsicherMachenEreignis(ereignis)
        clsSqlparam.paramListe.Clear()
        populateNatureg(lnat, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        querie = "UPDATE  " & CLstart.myViewsNTabs.tabNatureg & "  " & " SET VORGANGSID=@VORGANGSID" &
                         ",NUMMER=@NUMMER" &
                         ",ART=@ART " &
                         ",TYP=@TYP " &
                         ",QUELLE=@QUELLE " &
                         ",NOTIZ=@NOTIZ " &
                         ",MASSNAHMENNR=@MASSNAHMENNR " &
                         ",BESCHREIBUNG=@BESCHREIBUNG " & " WHERE ID=@ID"
        clsSqlparam.paramListe.Add(New clsSqlparam("ID", lnat.ID))
        anzahl = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
        Return anzahl
    End Function

    Function speichernNatreg(lnat As clsNatureg, modus As String, vid As Integer) As Boolean
        Select Case LCase(modus)
            Case "neu"
                lnat.VorgangsID = vid
                Dim lnatid As Integer = naturegAbspeichernNeu_AlleDB(lnat)
                If lnatid > 0 Then
                    lnat.ID = lnatid
                    Return True
                Else
                    nachricht("Problem beim speichernNatreg!")
                    Return False
                End If
            Case "edit"
                lnat.Quelle = myglobalz.sitzung.aktBearbeiter.Initiale
                Dim erfolg% = naturegAbspeichernEdit_AlleDB(lnat)
                If erfolg% > 0 Then
                    Return True
                Else
                    nachricht("Problem beim Abspeichern! speichernNatreg")
                    Return False
                End If
            Case Else
                Return False
        End Select
    End Function

    Public Sub naturegitem2Obj(item As DataRowView, aktnatureg As clsNatureg)
        Try
            With aktnatureg
                .nummer = CStr(clsDBtools.fieldvalue(item("NUMMER")))
                .notiz = CStr(clsDBtools.fieldvalue(item("NOTIZ")))
                .MassnahmenNr = CStr(clsDBtools.fieldvalue(item("MASSNAHMENNR")))
                .typ = CStr(clsDBtools.fieldvalue(item("TYP")))
                .VorgangsID = CInt(clsDBtools.fieldvalue(item("VORGANGSID")))
                .ID = CInt(clsDBtools.fieldvalue(item("ID")))
                .Quelle = CStr(clsDBtools.fieldvalue(item("QUELLE")))
                .timestamp = CDate(clsDBtools.fieldvalue(item("TS")))
                .beschreibung = CStr(clsDBtools.fieldvalue(item("BESCHREIBUNG")))
                .art = "M"
            End With
        Catch ex As Exception
            nachricht_und_Mbox("1 fehler in naturegitem2Obj: :" ,ex)
        End Try
    End Sub

    Function loeschenNatreg(lnat As clsNatureg) As Integer
        Dim hinweis As String = ""
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from  " & CLstart.myViewsNTabs.tabNatureg & "  where id=" & lnat.ID
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
        Return 1
    End Function
End Module
