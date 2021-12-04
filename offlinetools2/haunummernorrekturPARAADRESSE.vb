Imports System.Data
Module haunummernorrekturPARAADRESSE

    Public Function korr() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim strasseneu As String = "", hausnrNeu As String = ""
        Dim id As Integer
        Dim cnt As Integer = 0
        Dim com As OracleCommand
        initconnection()
        Try
            Dim SQLUPDATE$ = "select  id,strassenname,hausnrkombi " &
                "from paraadresse " &
                "where (hausnrkombi=' ' or hausnrkombi is null) " &
                "and strassenname is not null " &
                "and strassenname <>' ' " &
                "and (strassenname not like 'Postfach%')"

            beteiligterec.mydb.Tabelle = "paraadresse"
            com = New OracleCommand(SQLUPDATE$, MeineDBConnection)
            com.CommandText = SQLUPDATE
            beteiligterec.mydb.SQL = SQLUPDATE
            hinweis = beteiligterec.getDataDT()
            For Each person As DataRow In beteiligterec.dt.Rows
                aktPerson = New LIBMensch.Person
                id = CInt(person.Item(0))
                aktPerson.Kontakt.Anschrift.Strasse = person.Item(1).ToString
                aktPerson.Kontakt.Anschrift.Hausnr = person.Item(2).ToString

                If trennHausnrStrasse(aktPerson.Kontakt.Anschrift.Strasse, strasseneu, hausnrNeu) Then
                       aktPerson.Kontakt.Anschrift.Strasse = strasseneu
                    aktPerson.Kontakt.Anschrift.Hausnr = hausnrNeu
                    beteiligterec.mydb.SQL = "update paraadresse set " &
                        " strassenname='" & aktPerson.Kontakt.Anschrift.Strasse & "'" &
                        ",hausnrkombi='" & aktPerson.Kontakt.Anschrift.Hausnr & "'" &
                        " where  id=" & id
                    cnt += 1
                    Console.WriteLine(person.Item(0).ToString & "," & person.Item(1).ToString &
                                        "," & person.Item(2).ToString &
                                         vbCrLf &
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

