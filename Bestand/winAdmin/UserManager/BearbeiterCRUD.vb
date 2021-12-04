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
        myGlobalz.temp_MYDB.Tabelle = "Bearbeiter"
        myGlobalz.temp_MYDB.ServiceName = myGlobalz.vorgang_MYDB.ServiceName
        myGlobalz.temp_MYDB.password = myGlobalz.vorgang_MYDB.password
        myGlobalz.temp_MYDB.username = myGlobalz.vorgang_MYDB.username
        myGlobalz.temp_MYDB.dbtyp = myGlobalz.vorgang_MYDB.dbtyp
    End Sub
    Public Function BearbeiterAbspeichernNeu_AlleDB(ByVal lpers As clsBearbeiter) As Integer
        Dim personenid%
        DBsetzen()
        If (myGlobalz.temp_MYDB.dbtyp = "oracle") Then
            Dim zzz As New bearbeiterORACLE(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.temp_MYDB))
            personenid% = zzz.Bearbeiter_abspeichern_Neu(lpers)
            zzz.Dispose
        End If
        Return personenid
    End Function

    Public Function BearbeiterAbspeichernEDIT_AlleDB(ByVal lpers As clsBearbeiter) As Integer
        Dim personenid%
        DBsetzen()
        If (myGlobalz.temp_MYDB.dbtyp = "oracle") Then
            Dim zzz As New bearbeiterORACLE(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.temp_MYDB))
            personenid% = zzz.bearbeiter_abspeichern_EditExtracted(lpers)
            zzz.Dispose
        End If
        Return personenid
    End Function
End Module
