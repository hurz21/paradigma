Module modRB
    Public Function Raumbezug_abspeichern_Neu(ByVal aktrb As iRaumbezug) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            vorgangrec.mydb.Tabelle = "raumbezug"

            Dim SQLupdate$ =
                String.Format("INSERT INTO {0} (TYP,SEKID,TITEL,ABSTRACT,RECHTS,HOCH," &
                                      " XMIN,XMAX,YMIN,YMAX,FREITEXT) " +
                                      " VALUES (:TYP,:SEKID,:TITEL,:ABSTRACT,:RECHTS,:HOCH," &
                                      ":XMIN,:XMAX,:YMIN,:YMAX,:FREITEXT)",
                                                   vorgangrec.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING RAUMBEZUGSID INTO :R1"

            MeineDBConnection.Open()

            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            setSQLParamsRB(com, aktrb, 0)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)
        Catch ex As Exception
            Return -2
        End Try
    End Function
    Function setRBSQLBody() As String
        Return " SET " & _
         " TYP=:TYP" & _
         ",SEKID=:SEKID" & _
         ",TITEL=:TITEL" & _
         ",ABSTRACT=:ABSTRACT" & _
         ",RECHTS=:RECHTS" & _
         ",HOCH=:HOCH" & _
         ",XMIN=:XMIN" & _
         ",XMAX=:XMAX" & _
         ",YMIN=:YMIN" & _
         ",YMAX=:YMAX" &
         ",FREITEXT=:FREITEXT"
    End Function

    Sub setSQLParamsRB(ByVal com As OracleCommand, ByVal aktrb As iRaumbezug, ByVal rid%)
        com.Parameters.AddWithValue(":TYP", aktrb.typ)
        com.Parameters.AddWithValue(":SEKID", aktrb.SekID)
        com.Parameters.AddWithValue(":TITEL", aktrb.name)
        com.Parameters.AddWithValue(":ABSTRACT", aktrb.abstract)
        com.Parameters.AddWithValue(":RECHTS", CInt(aktrb.punkt.X))
        com.Parameters.AddWithValue(":HOCH", CInt(aktrb.punkt.Y))
        com.Parameters.AddWithValue(":XMIN", CInt(aktrb.box.xl))
        com.Parameters.AddWithValue(":XMAX", CInt(aktrb.box.xh))
        com.Parameters.AddWithValue(":YMIN", CInt(aktrb.box.yl))
        com.Parameters.AddWithValue(":YMAX", CInt(aktrb.box.yh))
        com.Parameters.AddWithValue(":FREITEXT", CStr(aktrb.Freitext))
    End Sub


    Public Function Koppelung_Raumbezug_Vorgang(ByVal RaumbezugsID%, ByVal VorgangsID%, ByVal status%) As Integer
        Dim newid& = -1
        Try
            initconnection()
            If RaumbezugsID% > 0 And VorgangsID% > 0 Then
                vorgangrec.mydb.Tabelle = "Raumbezug2Vorgang"
                vorgangrec.mydb.SQL = _
                  "INSERT INTO " & vorgangrec.mydb.Tabelle & "   " &
                  " (RAUMBEZUGSID,VORGANGSID,STATUS) VALUES (:RAUMBEZUGSID,:VORGANGSID,:STATUS) " &
                  " RETURNING ID INTO :R1"
                Dim com As OracleCommand
                MeineDBConnection.Open()
                com = New OracleCommand(vorgangrec.mydb.SQL, MeineDBConnection)
                com.Parameters.AddWithValue(":VORGANGSID", VorgangsID)
                com.Parameters.AddWithValue(":RAUMBEZUGSID", RaumbezugsID)
                com.Parameters.AddWithValue(":STATUS", status)
                newid = LIBoracle.clsOracleIns.GetNewid(com, vorgangrec.mydb.SQL)
                MeineDBConnection.Close()
                Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, vorgangrec.mydb.SQL)
            Else
                Return -3
            End If
        Catch ex As Exception
            Return -2
        End Try
    End Function
End Module
