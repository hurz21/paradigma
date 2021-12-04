Module modStammCRUD

    Function setSQLBody() As String
        Return " set " & _
        " VORGANGSID=:VORGANGSID" & _
        ",BEARBEITER=:BEARBEITER" & _
        ",BEMERKUNG=:BEMERKUNG " & _
        ",BESCHREIBUNG=:BESCHREIBUNG " & _
        ",EINGANG=:EINGANG " & _
        ",AUFNAHME=:AUFNAHME " & _
        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
        ",ERLEDIGT=:ERLEDIGT " & _
        ",ORTSTERMIN=:ORTSTERMIN " & _
        ",STELLUNGNAHME=:STELLUNGNAHME " & _
        ",PROBAUGAZ=:PROBAUGAZ " & _
        ",ALTAZ=:ALTAZ " & _
        ",GEMKRZ=:GEMKRZ " & _
        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
        ",AZ2=:AZ2 " & _
        ",WEITEREBEARB=:WEITEREBEARB " & _
        ",ARCDIR=:ARCDIR " & _
        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
        ",ABGABEBA=:ABGABEBA " &
        ",GUTACHTENMIT=:GUTACHTENMIT " &
        ",HATRAUMBEZUG=:HATRAUMBEZUG " &
        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
        ",STORAUMNR=:STORAUMNR " &
        ",STOTITEL=:STOTITEL " &
        ",PARAGRAF=:PARAGRAF " &
        ",INTERNENR=:INTERNENR "
    End Function

    Function setSQLBodyOHnePermanente() As String
        Return " set " & _
        " VORGANGSID=:VORGANGSID" & _
        ",BEARBEITER=:BEARBEITER" & _
        ",BEMERKUNG=:BEMERKUNG " & _
        ",BESCHREIBUNG=:BESCHREIBUNG " & _
        ",EINGANG=:EINGANG " & _
        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
        ",ERLEDIGT=:ERLEDIGT " & _
        ",ORTSTERMIN=:ORTSTERMIN " & _
        ",STELLUNGNAHME=:STELLUNGNAHME " & _
        ",PROBAUGAZ=:PROBAUGAZ " & _
        ",ALTAZ=:ALTAZ " & _
        ",GEMKRZ=:GEMKRZ " & _
        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
        ",AZ2=:AZ2 " & _
        ",WEITEREBEARB=:WEITEREBEARB " & _
        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
        ",ABGABEBA=:ABGABEBA " &
        ",GUTACHTENMIT=:GUTACHTENMIT " &
        ",HATRAUMBEZUG=:HATRAUMBEZUG " &
        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
        ",STORAUMNR=:STORAUMNR " &
        ",STOTITEL=:STOTITEL " &
         ",PARAGRAF=:PARAGRAF " &
        ",INTERNENR=:INTERNENR "
        '  ",ARCDIR=:ARCDIR " & _  sollte nicht mehr verändert werden
        '    ",AUFNAHME=:AUFNAHME " & _
    End Function

    Sub setSQLParams(ByVal com As OracleCommand,
                            ByVal vid%,
                            ByVal stamm As Stamm,
                            ByVal modus As String )     'myGlobalz.sitzung.Vorgang.Stammdaten
        com.Parameters.AddWithValue(":VORGANGSID", vid%) 'MYGLObalz.sitzung.VorgangsID)
        com.Parameters.AddWithValue(":BEARBEITER", stamm.Bearbeiter)
        com.Parameters.AddWithValue(":BEMERKUNG", stamm.Bemerkung)
        com.Parameters.AddWithValue(":BESCHREIBUNG", stamm.Beschreibung)
        com.Parameters.AddWithValue(":EINGANG", stamm.Eingangsdatum)

        com.Parameters.AddWithValue(":LETZTEBEARBEITUNG", stamm.LetzteBearbeitung)
        com.Parameters.AddWithValue(":ERLEDIGT", Convert.ToInt16(stamm.erledigt))
        com.Parameters.AddWithValue(":ORTSTERMIN", Convert.ToInt16(stamm.Ortstermin))
        com.Parameters.AddWithValue(":STELLUNGNAHME", Convert.ToInt16(stamm.Stellungnahme))
        com.Parameters.AddWithValue(":PROBAUGAZ", (stamm.Probaugaz))
        com.Parameters.AddWithValue(":ALTAZ", (stamm.AltAz))
        com.Parameters.AddWithValue(":GEMKRZ", stamm.GemKRZ)
        com.Parameters.AddWithValue(":LASTACTIONHEROE", stamm.LastActionHeroe)
        com.Parameters.AddWithValue(":AZ2", stamm.az.gesamt)
        com.Parameters.AddWithValue(":WEITEREBEARB", stamm.WeitereBearbeiter)

        com.Parameters.AddWithValue(":DARFNICHTVERNICHTETWERDEN", Convert.ToInt16(stamm.darfNichtVernichtetWerden))
        com.Parameters.AddWithValue(":ABGABEBA", Convert.ToInt16(stamm.AbgabeBA))
        com.Parameters.AddWithValue(":HATRAUMBEZUG", Convert.ToInt16(stamm.hatraumbezug))
        com.Parameters.AddWithValue(":GUTACHTENMIT", Convert.ToInt16(stamm.meinGutachten.existiert))
        com.Parameters.AddWithValue(":GUTACHTENDRIN", Convert.ToInt16(stamm.meinGutachten.UnterDokumente))
        com.Parameters.AddWithValue(":STORAUMNR", stamm.Standort.RaumNr)
        com.Parameters.AddWithValue(":STOTITEL", stamm.Standort.Titel)
        com.Parameters.AddWithValue(":PARAGRAF", stamm.Paragraf)
        com.Parameters.AddWithValue(":INTERNENR", stamm.InterneNr)
        If modus = "neu" Then
            com.Parameters.AddWithValue(":AUFNAHME", stamm.Aufnahmedatum)
            com.Parameters.AddWithValue(":ARCDIR", stamm.ArchivSubdir)
        End If
    End Sub

    Public Function Neu_speichern_stammdaten(
       ByVal vid As Integer) As Boolean ' myGlobalz.sitzung.VorgangREC			 ,myGlobalz.sitzung.VorgangsID	 	,'myGlobalz.sitzung.Vorgang.Stammdaten
        Dim newid&
        Dim com As OracleCommand
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""
        Try
            initconnection()
            Module1.vorgangrec.mydb.Tabelle = "stammdaten"
            vorgangrec.mydb.Tabelle = "stammdaten"
            SQLupdate$ =
               String.Format("INSERT INTO {0} (VORGANGSID,BEARBEITER,BEMERKUNG,BESCHREIBUNG,EINGANG,AUFNAHME," +
                                     "LETZTEBEARBEITUNG,ERLEDIGT,ORTSTERMIN,STELLUNGNAHME,PROBAUGAZ,ALTAZ," +
                                     "GEMKRZ,LASTACTIONHEROE,AZ2,WEITEREBEARB,ARCDIR,DARFNICHTVERNICHTETWERDEN,GUTACHTENMIT,GUTACHTENDRIN," &
                                     "STORAUMNR,STOTITEL,ABGABEBA,PARAGRAF,HATRAUMBEZUG,INTERNENR) " +
                            " VALUES (:VORGANGSID,:BEARBEITER,:BEMERKUNG,:BESCHREIBUNG,:EINGANG,:AUFNAHME," +
                                ":LETZTEBEARBEITUNG,:ERLEDIGT,:ORTSTERMIN,:STELLUNGNAHME,:PROBAUGAZ,:ALTAZ," +
                                 ":GEMKRZ,:LASTACTIONHEROE,:AZ2,:WEITEREBEARB,:ARCDIR,:DARFNICHTVERNICHTETWERDEN,:GUTACHTENMIT," &
                                 ":GUTACHTENDRIN,:STORAUMNR,:STOTITEL,:ABGABEBA,:PARAGRAF,:HATRAUMBEZUG,:INTERNENR)",
                                  vorgangrec.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"
            MeineDBConnection.Open()
            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            setSQLParams(com, vid, kartei.stamm, "neu")
            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            stammid = CInt(newid)
            MeineDBConnection.Close()
            Return CBool(LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate))
        Catch ex As Exception
            'nachricht_und_Mbox(String.Format("Stamm5 Fehler beim Abspeichern: {0}{1}", vbCrLf, ex))
            Return False
        End Try
    End Function
End Module
