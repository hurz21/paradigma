Imports System.Data

Module hausnummernKorrekturBeteiligte
    Public Function korr() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim strasseneu As String = "", hausnrNeu As String = ""
        Dim id As Integer
        Dim cnt As Integer = 0
        Dim com As OracleCommand
        initconnection()
        Try
            Dim SQLUPDATE$ = "select personenid,strasse,hausnr,bemerkung " &
                "from beteiligte " &
                "where (hausnr=' ' or hausnr is null) " &
                "and strasse is not null " &
                "and strasse <>' ' " &
                "and (strasse not like 'Postfach%')"

            beteiligterec.mydb.Tabelle = "BETEILIGTE"
            com = New OracleCommand(SQLUPDATE$, MeineDBConnection)
            com.CommandText = SQLUPDATE
            beteiligterec.mydb.SQL = SQLUPDATE
            hinweis = beteiligterec.getDataDT()
            For Each person As DataRow In beteiligterec.dt.Rows
                aktPerson = New LIBMensch.Person
                id = CInt(person.Item(0))
                aktPerson.Kontakt.Anschrift.Strasse = person.Item(1).ToString
                aktPerson.Kontakt.Anschrift.Hausnr = person.Item(2).ToString
                aktPerson.Kontakt.Anschrift.Bemerkung = person.Item(3).ToString
                If trennHausnrStrasse(aktPerson.Kontakt.Anschrift.Strasse, strasseneu, hausnrNeu) Then
                    aktPerson.Kontakt.Anschrift.Bemerkung = aktPerson.Kontakt.Anschrift.Bemerkung & ", " & aktPerson.Kontakt.Anschrift.Strasse ' zur sicherheit
                    aktPerson.Kontakt.Anschrift.Strasse = strasseneu
                    aktPerson.Kontakt.Anschrift.Hausnr = hausnrNeu
                    beteiligterec.mydb.SQL = "update beteiligte set " &
                        " strasse='" & aktPerson.Kontakt.Anschrift.Strasse & "'" &
                        ",hausnr='" & aktPerson.Kontakt.Anschrift.Hausnr & "'" &
                        ",bemerkung='" & aktPerson.Kontakt.Anschrift.Bemerkung & "'" &
                        " where personenid=" & id
                    cnt += 1
                    Console.WriteLine(person.Item(0).ToString & "," & person.Item(1).ToString &
                                        "," & person.Item(2).ToString &
                                        "," & person.Item(3).ToString & vbCrLf &
                                        beteiligterec.mydb.SQL)
                    MeineDBConnection.Open()
                    com = New OracleCommand(beteiligterec.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
                    Dim rtn = CInt(com.ExecuteNonQuery)
                    MeineDBConnection.Close()
                End If
            Next
            Return 0
        Catch ex As Exception
            Console.WriteLine("Bet5 Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function
End Module
