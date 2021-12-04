Imports System.Data

Module naturegTools
    Public Function naturegAbspeichernNeu_AlleDB(ByVal lnat As clsNatureg) As Integer
        Dim natid% 
        If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
            Dim zzz As New naturegOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
            natid = zzz.natureg_abspeichern_Neu(lnat)
            zzz.dispose
        End If
        Return natid
    End Function

    Public Function naturegAbspeichernEdit_AlleDB(ByVal lnat As clsNatureg) As Integer
        Dim anzahl As Integer
        If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
            Dim zzz As New naturegOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
            anzahl = zzz.Natureg_abspeichern_EditExtracted(lnat)
            zzz.dispose
        End If
        Return anzahl
    End Function

    Function speichernNatreg(lnat As clsNatureg, modus As String, vid As Integer) As Boolean
        If modus = "neu" Then
            lnat.VorgangsID = vid
            Dim lnatid As Integer = naturegAbspeichernNeu_AlleDB(lnat)
            If lnatid > 0 Then
                lnat.ID = lnatid
                Return True
            Else
                nachricht("Problem beim speichernNatreg!")
                Return False
            End If
        End If
        If modus = "edit" Then
            lnat.Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
            Dim erfolg% = naturegAbspeichernEdit_AlleDB(lnat)
            If erfolg% > 0 Then
                Return True
            Else
                nachricht("speichernNatreg Problem beim Abspeichern!")
                Return False
            End If
        End If
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
            nachricht_und_Mbox("1 fehler in naturegitem2Obj: ggf. fehlt die rolle:" & ex.ToString)
        End Try
    End Sub

    Function loeschenNatreg(lnat As clsNatureg) As Integer
        Dim anzahl As Integer 
        If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
            Dim zzz As New naturegOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
            anzahl = zzz.Natureg_loeschen(lnat.ID)
            zzz.dispose
        End If
        Return anzahl
    End Function

End Module
