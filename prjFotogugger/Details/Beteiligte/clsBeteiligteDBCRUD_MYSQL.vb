Imports MySql.Data.MySqlClient

Public Class clsBeteiligteDBCRUD_MYSQL
    Implements IDisposable
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub


    Shared Function setSQLbody() As String
        Return " set  Name=@Name" &
         ",Vorname=@Vorname" &
         ",Bemerkung=@Bemerkung " &
         ",Namenszusatz=@Namenszusatz " &
         ",Anrede=@Anrede " &
         ",Quelle=@Quelle " &
         ",Gemeindename=@Gemeindename " &
         ",Strasse=@Strasse " &
         ",Hausnr=@Hausnr " &
         ",PLZ=@PLZ" &
         ",Postfach=@Postfach" &
         ",PostfachPLZ=@PostfachPLZ" &
         ",FFTelefon1=@FFTelefon1 " &
         ",FFTelefon2=@FFTelefon2 " &
         ",FFFax1=@FFFax1 " &
         ",FFFax2=@FFFax2 " &
         ",FFMobilfon=@FFMobilfon " &
         ",FFemail=@FFemail " &
         ",FFHomepage=@FFHomepage " &
         ",GesellFunktion=@GesellFunktion " &
         ",OrgName=@OrgName" &
         ",OrgZusatz=@OrgZusatz" &
         ",OrgTyp1=@OrgTyp1 " &
         ",OrgTyp2=@OrgTyp2 " &
         ",OrgEigentuemer=@OrgEigentuemer " &
         ",Rolle=@Rolle " &
         ",Kassenkonto=@Kassenkonto " &
         ",personenvorlage=@personenvorlage " &
         ",VERTRETENDURCH=@VERTRETENDURCH " &
         ",VorgangsID=@VorgangsID "
    End Function
    Shared Sub setSQLParams(ByVal com As MySqlCommand, ByVal vid%, ByVal lpers As Person)
        With lpers
            com.Parameters.AddWithValue("@Name", .Name)
            com.Parameters.AddWithValue("@Vorname", .Vorname)
            com.Parameters.AddWithValue("@Bemerkung", .Bemerkung)
            com.Parameters.AddWithValue("@Namenszusatz", .Namenszusatz)
            com.Parameters.AddWithValue("@Anrede", .Anrede)
            com.Parameters.AddWithValue("@Quelle", .Quelle)
            com.Parameters.AddWithValue("@Gemeindename", .Kontakt.Anschrift.Gemeindename)
            com.Parameters.AddWithValue("@Strasse", .Kontakt.Anschrift.Strasse)
            com.Parameters.AddWithValue("@Hausnr", .Kontakt.Anschrift.Hausnr)
            com.Parameters.AddWithValue("@PLZ", .Kontakt.Anschrift.PLZ.ToString)
            com.Parameters.AddWithValue("@Postfach", .Kontakt.Anschrift.Postfach)
            com.Parameters.AddWithValue("@PostfachPLZ", .Kontakt.Anschrift.PostfachPLZ)
            com.Parameters.AddWithValue("@FFTelefon1", .Kontakt.elektr.Telefon1)
            com.Parameters.AddWithValue("@FFTelefon2", .Kontakt.elektr.Telefon2)
            com.Parameters.AddWithValue("@FFFax1", .Kontakt.elektr.Fax1)
            com.Parameters.AddWithValue("@FFFax2", .Kontakt.elektr.Fax2)
            com.Parameters.AddWithValue("@FFMobilfon", .Kontakt.elektr.MobilFon)
            com.Parameters.AddWithValue("@FFemail", .Kontakt.elektr.Email)
            com.Parameters.AddWithValue("@FFHomepage", .Kontakt.elektr.Homepage)
            com.Parameters.AddWithValue("@GesellFunktion", .Kontakt.GesellFunktion)
            com.Parameters.AddWithValue("@OrgName", .Kontakt.Org.Name)
            com.Parameters.AddWithValue("@OrgZusatz", .Kontakt.Org.Zusatz)
            com.Parameters.AddWithValue("@OrgTyp1", .Kontakt.Org.Typ1)
            com.Parameters.AddWithValue("@OrgTyp2", .Kontakt.Org.Typ2)
            com.Parameters.AddWithValue("@OrgEigentuemer", .Kontakt.Org.Eigentuemer)
            com.Parameters.AddWithValue("@Rolle", .Rolle)
            com.Parameters.AddWithValue("@VorgangsID", vid)
            com.Parameters.AddWithValue("@personenvorlage", .PersonenVorlage)
            com.Parameters.AddWithValue("@VERTRETENDURCH", .VERTRETENDURCH)
            'com.Parameters.AddWithValue("@BVTitel", .Kontakt.Bankkonto.Titel)
            'com.Parameters.AddWithValue("@IBAN", .Kontakt.Bankkonto.IBAN)
            'com.Parameters.AddWithValue("@BIC", .Kontakt.Bankkonto.BIC)
            'com.Parameters.AddWithValue("@BANKNAME", .Kontakt.Bankkonto.BankName)
            com.Parameters.AddWithValue("@Kassenkonto", .Kassenkonto)
        End With
    End Sub
    Public Function Beteiligte_abspeichern_EditExtracted(ByVal pid%, ByVal vid%, ByVal lpers As Person) As Integer  'myGlobalz.sitzung.aktPerson.PersonenID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As MySqlCommand
        Try
            If myglobalz.sitzung.aktPerson.PersonenID < 1 Then
                nachricht_und_Mbox("FEHLER:Beteiligte_abspeichern_EditExtracted updateid =0. Abbruch")
                Return 0
            End If
            'myglobalz.sitzung.beteiligteREC.mydb.Tabelle ="Beteiligte"
            myGlobalz.sitzung.beteiligteREC.mydb.SQL =
             "update " & CLstart.myViewsNTabs.tabBeteiligte & " " &
             setSQLbody() &
             " where personenid=" & pid% 'myGlobalz.sitzung.aktPerson.PersonenID

            myGlobalz.sitzung.beteiligteREC.dboeffnen(hinweis$)

            If myglobalz.sitzung.beteiligteREC.mydb.dbtyp = "mysql" Then
                com = New MySqlCommand(myglobalz.sitzung.beteiligteREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
                setSQLParams(com, vid, lpers)
                anzahlTreffer& = CInt(com.ExecuteNonQuery)
            End If



            myglobalz.sitzung.beteiligteREC.dbschliessen(hinweis$)

            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernk:" & myglobalz.sitzung.beteiligteREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Bet1 Fehler beim Abspeichern: " ,ex)
            Return -2
        End Try
    End Function

    Public Function Beteiligte_abspeichern_Neu(ByVal vid%, ByVal lpers As Person) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand

        Try
            'myGlobalz.sitzung.beteiligteREC.mydb.Tabelle ="Beteiligte"
            myGlobalz.sitzung.beteiligteREC.mydb.SQL =
             "insert into " & CLstart.myViewsNTabs.tabBeteiligte & " " & setSQLbody()

            myGlobalz.sitzung.beteiligteREC.dboeffnen(hinweis$)
            com = New MySqlCommand(myGlobalz.sitzung.beteiligteREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
            setSQLParams(com, vid, lpers)

            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            com.CommandText = "Select LAST_INSERT_ID()"
            newid = CLng(com.ExecuteScalar)
            myGlobalz.sitzung.beteiligteREC.dbschliessen(hinweis$)

            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernl:" & myGlobalz.sitzung.beteiligteREC.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Bet2 Fehler beim Abspeichern: " ,ex)
            Return -2
        End Try
    End Function





    'Public Shared Function Koppelung_Beteiligte_Vorgang(ByVal BeteiligteID%, ByVal VorgangsID%, ByVal status as integer) as  Integer
    '    Dim anzahlTreffer&
    '    Dim newid& = -1
    '    Try
    '        If BeteiligteID% > 0 And VorgangsID% > 0 Then
    '            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL)
    '            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '            myGlobalz.sitzung.tempREC.mydb.Tabelle ="Beteiligte2Vorgang"
    '            myGlobalz.sitzung.tempREC.mydb.SQL = _
    '             "insert into " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " set " & _
    '             " BeteiligteID=" & BeteiligteID% & _
    '             ",VorgangsID=" & VorgangsID% & _
    '             ",status=" & status%
    '            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
    '            If anzahlTreffer < 1 Then
    '                nachricht_und_Mbox("Problem beim Abspeicherno:" & myGlobalz.sitzung.tempREC.mydb.SQL)
    '                Return -1
    '            Else
    '                Return CInt(newid)
    '            End If
    '        Else
    '            nachricht("Koppelung Koppelung_Vorgang_Raumbezug / person nicht Möglich")
    '            Return -3
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Koppelung_Vorgang_Raumbezug Problem beim Abspeichernp: " &
    '                     ex.ToString & vbCrLf & myGlobalz.sitzung.tempREC.mydb.SQL)
    '        Return -2
    '    End Try
    'End Function


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
