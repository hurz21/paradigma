Module modKarteiVORGANGCRUD

    Function setSQLbody() As String
        Return " set " & _
         " AZ=:AZ" & _
         ",SACHGEBIETNR=:SACHGEBIETNR" & _
         ",VORGANGSNR=:VORGANGSNR " & _
         ",VORGANGSGEGENSTAND=:VORGANGSGEGENSTAND " & _
         ",SACHGEBIETSTEXT=:SACHGEBIETSTEXT " & _
         ",ISTUNB=:ISTUNB "
    End Function
    Function setSQLParams(ByVal com As OracleCommand, ByVal vid%) As Boolean
        Try
            com.Parameters.AddWithValue(":AZ", kartei.stamm.az.gesamt)
            com.Parameters.AddWithValue(":SACHGEBIETNR", kartei.stamm.az.sachgebiet.Zahl)
            com.Parameters.AddWithValue(":VORGANGSNR", kartei.stamm.az.Vorgangsnummer)
            com.Parameters.AddWithValue(":VORGANGSGEGENSTAND", kartei.stamm.az.Prosa)
            com.Parameters.AddWithValue(":SACHGEBIETSTEXT", kartei.stamm.az.sachgebiet.Header)
            com.Parameters.AddWithValue(":ISTUNB", CBool(kartei.stamm.az.sachgebiet.isUNB()))
            Return True
        Catch ex As Exception
            Return False
        End Try
        '  com.Parameters.AddWithValue(":VORGANGSID", vid)
    End Function
    Public Sub initconnection()
        vorgangrec = New LIBoracle.clsDBspecOracle
        ereignisRec = New LIBoracle.clsDBspecOracle
        beteiligterec = New LIBoracle.clsDBspecOracle

        vorgangrec.mydb.Host = "ora-clu-vip-003"
        vorgangrec.mydb.Schema = "paradigma"
        vorgangrec.mydb.ServiceName = "paradigma.kreis-of.local"
        vorgangrec.mydb.username = "paradigma"
        vorgangrec.mydb.password = "luftikus12"

        ereignisRec.mydb.Host = "ora-clu-vip-003"
        ereignisRec.mydb.Schema = "paradigma"
        ereignisRec.mydb.ServiceName = "paradigma.kreis-of.local"
        ereignisRec.mydb.username = "paradigma"
        ereignisRec.mydb.password = "luftikus12"

        beteiligterec.mydb.Host = "ora-clu-vip-003"
        beteiligterec.mydb.Schema = "paradigma"
        beteiligterec.mydb.ServiceName = "paradigma.kreis-of.local"
        beteiligterec.mydb.username = "paradigma"
        beteiligterec.mydb.password = "luftikus12"

        MeineDBConnection = New OracleConnection("Data Source=(DESCRIPTION=" & _
     "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & vorgangrec.mydb.Host & ")(PORT=1521)))" & _
      "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" & _
     "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & vorgangrec.mydb.ServiceName & ")));" & _
     "User Id=" & vorgangrec.mydb.username & ";Password=" & vorgangrec.mydb.password & ";")
    End Sub
    Public Function Neu_speichern_Vorgang() As Boolean
        initconnection()
        'hier wird die vorgangsnummer angelegt
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            vorgangrec.mydb.Tabelle = "Vorgang"

            Dim SQLupdate$ =
              String.Format("INSERT INTO {0} (AZ,SACHGEBIETNR,VORGANGSNR,VORGANGSGEGENSTAND,SACHGEBIETSTEXT,ISTUNB) " +
                                    " VALUES (:AZ,:SACHGEBIETNR,:VORGANGSNR,:VORGANGSGEGENSTAND,:SACHGEBIETSTEXT,:ISTUNB)",
                                      vorgangrec.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING VORGANGSID INTO :R1"

            'glob2.nachricht("nach setSQLbody : " & SQLupdate)
            MeineDBConnection.Open()
            'glob2.nachricht("nach dboeffnen  ")

            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            'glob2.nachricht("vor setParams  ")
            setSQLParams(com, 0)


            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()
            If newid < 1 Then
                'glob2.nachricht_und_Mbox(String.Format("Problem beim abspeichern:{0}", myGlobalz.sitzung.VorgangREC.mydb.SQL))
                Return False
            Else
                vorgangid = CInt(newid)
                ' glob2.nachricht("Neue Paradigmanr: " & newid)
                Return True
            End If
        Catch ex As Exception
            ' glob2.nachricht_und_Mbox("Problem beim Abspeichern des Vorgangs: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function



End Module
