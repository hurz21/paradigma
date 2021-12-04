Imports System.Data
Module modDS_tools
    Private com As SqlClient.SqlCommand

    Friend Function DS_users_Add_gruppe(bearbeiterid As Integer, gruppenid As String) As Boolean
        Dim newid As Long
        Dim myconn As New SqlClient.SqlCommand
        Try
            Dim erfolg As Boolean
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            Dim returnIdentity As Boolean = True


            querie = "insert into ds_user2gruppe   " &
                                             " (bearbeiterid,gruppenid) values (@bearbeiterid,@gruppenid)"
            clsSqlparam.paramListe.Add(New clsSqlparam("BEARBEITERID", bearbeiterid)) 'MYGLObalz.sitzung
            clsSqlparam.paramListe.Add(New clsSqlparam("GRUPPENID", gruppenid)) 'MYGLObalz.sitzung
            newid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, returnIdentity, "ID")

            'clsDBspecOracle.getOracleconnectionString(myglobalz.sitzung.tempREC.mydb)
            'myconn = New OracleConnection(clsDBspecOracle.getOracleconnectionString(myglobalz.sitzung.tempREC.mydb))
            'myconn.Open()
            'com = New OracleCommand(myglobalz.sitzung.tempREC.mydb.SQL, myconn)
            'com.Parameters.AddWithValue(":BEARBEITERID", bearbeiterid)
            'com.Parameters.AddWithValue(":GRUPPENID", gruppenid)
            'newid = clsOracleIns.GetNewid(com, myglobalz.sitzung.tempREC.mydb.SQL)
            'myconn.Close()
            'myconn.Dispose()
            'myconn = Nothing

            If newid < 1 Then
                nachricht("Keine rechte gespeichert!")
                Return False
            Else
                nachricht(String.Format("{0} neue id in ds_user2gruppe vorhanden", newid))
                Return True
            End If
            Return True
        Catch ex As Exception
            nachricht("fehler in DS_users_Add_gruppe: ", ex)
            Return False
        End Try
    End Function

    Friend Function DS_users_Remove_gruppe(bearbeiterid As Integer, gruppenid As String) As Boolean
        Dim newid As Long
        Dim erfolg As Boolean
        Dim querie As String
        clsSqlparam.paramListe.Clear()
        Dim returnIdentity As Boolean = False
        If gruppenid = Nothing Then Return False
        Try
            myGlobalz.sitzung.tempREC.mydb.Schema = "paradigma"

            'myGlobalz.sitzung.tempREC.mydb.Tabelle =CLstart.myViewsNTabs.tabBearbeiter
            querie = "delete from ds_user2gruppe " &
                        "  where bearbeiterid=" & bearbeiterid &
                        " and gruppenid=" & gruppenid

            'clsSqlparam.paramListe.Add(New clsSqlparam("BEARBEITERID", bearbeiterid)) 'MYGLObalz.sitzung
            'clsSqlparam.paramListe.Add(New clsSqlparam("GRUPPENID", gruppenid)) 'MYGLObalz.sitzung
            newid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, returnIdentity, "ID")



            Dim anz As Long
            anz = myGlobalz.sitzung.tempREC.sqlexecute(newid)
            'clsDBspecOracle.getOracleconnectionString(myGlobalz.sitzung.tempREC.mydb)
            'myconn = New OracleConnection(clsDBspecOracle.getOracleconnectionString(myGlobalz.sitzung.tempREC.mydb))
            'myconn.Open()
            'com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, myconn)
            'com.Parameters.AddWithValue(":BEARBEITERID", bearbeiterid)
            'com.Parameters.AddWithValue(":GRUPPENID", gruppenid)
            'newid = clsOracleIns.GetNewid(com, myGlobalz.sitzung.tempREC.mydb.SQL)
            'myconn.Close()
            'myconn.Dispose()
            'myconn = Nothing

            If anz < 1 Then
                nachricht("Keine rechte gespeichert!")
                Return False
            Else
                nachricht(String.Format("{0} neue id in DS_users_Remove_gruppe vorhanden", newid))
                Return True
            End If
            Return True
        Catch ex As Exception
            nachricht("fehler in DS_users_Remove_gruppe: ", ex)
            Return False
        End Try
    End Function
End Module
