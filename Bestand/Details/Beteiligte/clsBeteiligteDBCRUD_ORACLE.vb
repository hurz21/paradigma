'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data

Public Class clsBeteiligteDBCRUD_ORACLE
    Implements IDisposable
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, OracleConnection)
    End Sub

    Private Shared Sub avoidNUlls(ByVal lpers As Person)
        If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
        If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
        If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
        If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
        If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
    End Sub
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
         ",BEZIRK=:BEZIRK " & _
         ",LASTCHANGE=:LASTCHANGE " & _
         ",VORGANGSID=:VORGANGSID "
    End Function
    Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal vid%, ByVal lpers As Person)
        avoidNUlls(lpers)
      
        If String.IsNullOrEmpty(lpers.Bemerkung.Trim) Then
            If lpers.Kontakt.elektr.Telefon1.Length > 240 Then
                lpers.Bemerkung = lpers.Kontakt.elektr.Telefon1.Substring(0, 240)
            Else
                lpers.Bemerkung = lpers.Kontakt.elektr.Telefon1
            End If

        End If
        If lpers.Kontakt.elektr.Telefon1.Length > 99 Then
            lpers.Kontakt.elektr.Telefon1 = lpers.Kontakt.elektr.Telefon1.Substring(0, 98)
        End If
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
                com.Parameters.AddWithValue(":LASTCHANGE", Now)
            End With
            com.Parameters.AddWithValue(":VORGANGSID", vid)
        Catch ex As Exception
            nachricht("Fehler in setSQLParams beteiligte: " & ex.ToString)
        End Try

    End Sub

    Public Function Beteiliten_loeschen(ByVal personenID as integer) as  Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "beteiligte"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             "  where PersonenID=" & personenID%
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Beteiliten_loeschen:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Beteiliten_loeschen: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function Beteiligte_abspeichern_EditExtracted(ByVal pid%, ByVal vid%, ByVal lpers As Person) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        Try
            If lpers.PersonenID < 1 Then
                nachricht_und_Mbox("FEHLER:Beteiligte_abspeichern_EditExtracted updateid =0. Abbruch")
                Return 0
            End If
            myGlobalz.sitzung.beteiligteREC.mydb.Tabelle = "Beteiligte"
            myGlobalz.sitzung.beteiligteREC.mydb.SQL = _
             "UPDATE  " & myGlobalz.sitzung.beteiligteREC.mydb.Tabelle & setSQLbody() & " WHERE PERSONENID=:PERSONENID"

            MeineDBConnection.Open()
            com = New OracleCommand(myGlobalz.sitzung.beteiligteREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
            setSQLParams(com, vid, lpers)
            com.Parameters.AddWithValue(":PERSONENID", pid%)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()

            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernq:" & myGlobalz.sitzung.beteiligteREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Bet4 Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function Beteiligte_abspeichern_Neu(ByVal vid%, ByVal lpers As Person) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            myGlobalz.sitzung.beteiligteREC.mydb.Tabelle = "BETEILIGTE"
            Dim SQLUPDATE$ = _
         String.Format("INSERT INTO {0} (VORGANGSID,NACHNAME,VORNAME,BEMERKUNG,NAMENSZUSATZ,ANREDE,QUELLE,GEMEINDENAME,STRASSE,HAUSNR,POSTFACH,POSTFACHPLZ,FFTELEFON1,FFTELEFON2,FFFAX1," +
                       "FFFAX2,FFMOBILFON,FFEMAIL,FFHOMEPAGE,GESELLFUNKTION,ORGNAME,ORGZUSATZ,ORGTYP1,ORGTYP2,ORGEIGENTUEMER,ROLLE,KASSENKONTO,PLZ,BEZIRK,LASTCHANGE) " +
                               " VALUES (:VORGANGSID,:NACHNAME,:VORNAME,:BEMERKUNG,:NAMENSZUSATZ,:ANREDE,:QUELLE,:GEMEINDENAME,:STRASSE,:HAUSNR,:POSTFACH,:POSTFACHPLZ,:FFTELEFON1,:FFTELEFON2,:FFFAX1," +
                               ":FFFAX2,:FFMOBILFON,:FFEMAIL,:FFHOMEPAGE,:GESELLFUNKTION,:ORGNAME,:ORGZUSATZ,:ORGTYP1,:ORGTYP2,:ORGEIGENTUEMER,:ROLLE,:KASSENKONTO,:PLZ,:BEZIRK,:LASTCHANGE)",
                                 myGlobalz.sitzung.beteiligteREC.mydb.Tabelle)

            SQLUPDATE$ = SQLUPDATE$ & " RETURNING PERSONENID INTO :R1"
            MeineDBConnection.Open()
            com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
            setSQLParams(com, vid, lpers)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLUPDATE)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
        Catch ex As Exception
            nachricht_und_Mbox("Bet5 Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Shared Function Entkoppelung_Beteiligte_Vorgang(ByVal BeteiligteID%, ByVal VorgangsID as integer) as  Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            If BeteiligteID% > 0 And VorgangsID% > 0 Then
                myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
                myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
                myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
                myGlobalz.sitzung.tempREC.mydb.Tabelle = "Beteiligte2Vorgang"
                myGlobalz.sitzung.tempREC.mydb.SQL = _
                 "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
                 " where  BeteiligteID=" & BeteiligteID% & _
                 " and VorgangsID=" & VorgangsID%
                anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
                If anzahlTreffer < 1 Then
                    nachricht_und_Mbox("Problem beim Abspeichernr:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                    Return -1
                Else
                    Return CInt(anzahlTreffer)
                End If
            Else
                nachricht("  Entkoppelung_Beteiligte_Vorgang /  nicht Möglich")
                Return -3
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Entkoppelung_Beteiligte_Vorgang Problem beim Abspeicherns: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function Koppelung_Beteiligte_Vorgang(ByVal BeteiligteID%, ByVal VorgangsID%, ByVal status as integer) as  Integer
        Dim newid& = -1
        Try
            If BeteiligteID% > 0 And VorgangsID% > 0 Then
                myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
                myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
                myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
                myGlobalz.sitzung.tempREC.mydb.Tabelle = "Beteiligte2Vorgang"

                myGlobalz.sitzung.tempREC.mydb.SQL = "INSERT INTO " & myGlobalz.sitzung.tempREC.mydb.Tabelle &
                                 " (BETEILIGTEID,VORGANGSID,STATUS) VALUES (:BETEILIGTEID,:VORGANGSID,:STATUS) " &
                                 " RETURNING ID INTO :R1"
                Dim com As OracleCommand
                MeineDBConnection.Open()
                nachricht("nach dboeffnen  ")
                com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
                com.Parameters.AddWithValue(":BETEILIGTEID", BeteiligteID)
                com.Parameters.AddWithValue(":VORGANGSID", VorgangsID)
                com.Parameters.AddWithValue(":STATUS", status)

                newid = LIBoracle.clsOracleIns.GetNewid(com, myGlobalz.sitzung.tempREC.mydb.SQL)
                MeineDBConnection.Close()
                Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, myGlobalz.sitzung.tempREC.mydb.SQL)
            Else
                nachricht("Koppelung Koppelung_Vorgang_Raumbezug / person nicht Möglich")
                Return -3
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Koppelung_Vorgang_Raumbezug Problem beim Abspeichernt: " &
                         ex.ToString & vbCrLf & myGlobalz.sitzung.tempREC.mydb.SQL)
            Return -2
        End Try
    End Function
    #Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
