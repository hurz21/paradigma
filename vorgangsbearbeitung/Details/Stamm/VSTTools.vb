Imports System.Data
Namespace VSTTools
    Public Class editStammdaten_alleDB
        Public Shared Function exe(ByRef vid%, ByVal stamm As Stamm) As Boolean ', myGlobalz.sitzung.Vorgang.Stammdaten
            Dim erfolg As Boolean
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            Dim returnIdentity As Boolean = False
            'LibDB.clsSqlparam.paramListe.Add(New sqlparam("eid", 0))
            querie = "UPDATE " & CLstart.myViewsNTabs.TABSTAMMDATEN &
                            " set " &
                            " VORGANGSID=@VORGANGSID" &
                            ",BEARBEITER=@BEARBEITER" &
                            ",BEARBEITERID=@BEARBEITERID" &
                            ",BEMERKUNG=@BEMERKUNG " &
                            ",BESCHREIBUNG=@BESCHREIBUNG " &
                            ",EINGANG=@EINGANG " &
                            ",LETZTEBEARBEITUNG=@LETZTEBEARBEITUNG " &
                            ",ERLEDIGT=@ERLEDIGT " &
                            ",ORTSTERMIN=@ORTSTERMIN " &
                            ",STELLUNGNAHME=@STELLUNGNAHME " &
                            ",PROBAUGAZ=@PROBAUGAZ " &
                            ",ALTAZ=@ALTAZ " &
                            ",GEMKRZ=@GEMKRZ " &
                            ",LASTACTIONHEROE=@LASTACTIONHEROE " &
                            ",AZ2=@AZ2 " &
                            ",WEITEREBEARB=@WEITEREBEARB " &
                            ",DARFNICHTVERNICHTETWERDEN=@DARFNICHTVERNICHTETWERDEN " &
                            ",ABGABEBA=@ABGABEBA " &
                            ",GUTACHTENMIT=@GUTACHTENMIT " &
                            ",HATRAUMBEZUG=@HATRAUMBEZUG " &
                            ",GUTACHTENDRIN=@GUTACHTENDRIN " &
                            ",STORAUMNR=@STORAUMNR " &
                            ",STOTITEL=@STOTITEL " &
                            ",PARAGRAF=@PARAGRAF " &
                            ",INTERNENR=@INTERNENR " &
                            "  where VorgangsID=@VorgangsID"

            populateStammdaten(vid, stamm, returnIdentity)

            Dim ID As Integer = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
        End Function

        Shared Sub populateStammdaten(vid As Integer, stamm As Stamm, returnIdentity As Boolean)
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", vid%)) 'MYGLObalz.sitzung.VorgangsID)
            clsSqlparam.paramListe.Add(New clsSqlparam("BEARBEITER", stamm.hauptBearbeiter.Initiale.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEARBEITERID", stamm.hauptBearbeiter.ID))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEMERKUNG", stamm.Bemerkung))
            clsSqlparam.paramListe.Add(New clsSqlparam("BESCHREIBUNG", stamm.Beschreibung))
            clsSqlparam.paramListe.Add(New clsSqlparam("EINGANG",
                                          clsDBtools.makedateMssqlConform(stamm.Eingangsdatum, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))
            clsSqlparam.paramListe.Add(New clsSqlparam("LETZTEBEARBEITUNG", stamm.LetzteBearbeitung))
            clsSqlparam.paramListe.Add(New clsSqlparam("ERLEDIGT", Convert.ToInt16(stamm.erledigt)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORTSTERMIN", Convert.ToInt16(stamm.Ortstermin)))
            clsSqlparam.paramListe.Add(New clsSqlparam("STELLUNGNAHME", Convert.ToInt16(stamm.Stellungnahme)))
            clsSqlparam.paramListe.Add(New clsSqlparam("PROBAUGAZ", (stamm.Probaugaz)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ALTAZ", (stamm.AltAz)))
            clsSqlparam.paramListe.Add(New clsSqlparam("GEMKRZ", stamm.GemKRZ))
            clsSqlparam.paramListe.Add(New clsSqlparam("LASTACTIONHEROE", stamm.LastActionHeroe))
            clsSqlparam.paramListe.Add(New clsSqlparam("AZ2", stamm.az.gesamt))
            clsSqlparam.paramListe.Add(New clsSqlparam("WEITEREBEARB", stamm.WeitereBearbeiter))

            clsSqlparam.paramListe.Add(New clsSqlparam("DARFNICHTVERNICHTETWERDEN", Convert.ToInt16(stamm.darfNichtVernichtetWerden)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ABGABEBA", Convert.ToInt16(stamm.AbgabeBA)))
            clsSqlparam.paramListe.Add(New clsSqlparam("HATRAUMBEZUG", Convert.ToInt16(stamm.hatraumbezug)))
            clsSqlparam.paramListe.Add(New clsSqlparam("GUTACHTENMIT", Convert.ToInt16(stamm.meinGutachten.existiert)))
            clsSqlparam.paramListe.Add(New clsSqlparam("GUTACHTENDRIN", Convert.ToInt16(stamm.meinGutachten.UnterDokumente)))
            clsSqlparam.paramListe.Add(New clsSqlparam("STORAUMNR", stamm.Standort.RaumNr))
            clsSqlparam.paramListe.Add(New clsSqlparam("STOTITEL", stamm.Standort.Titel))
            clsSqlparam.paramListe.Add(New clsSqlparam("PARAGRAF", stamm.Paragraf))
            clsSqlparam.paramListe.Add(New clsSqlparam("INTERNENR", stamm.InterneNr))
            If returnIdentity Then ' nur beim insert werden diese beiden daten gespeichert. 
                'dürfen danach nicht merh überschrieben werden
                clsSqlparam.paramListe.Add(New clsSqlparam("AUFNAHME",
                                                                                     clsDBtools.makedateMssqlConform(stamm.Aufnahmedatum, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))

                clsSqlparam.paramListe.Add(New clsSqlparam("ARCDIR", stamm.ArchivSubdir))
            End If
        End Sub
        'Public Shared Sub speichernEreignisStammdaten()
        '    'If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
        '    '    Dim zzz As New clsStammCRUD_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
        '    '    If zzz.EDIT_speichern_stammdaten(myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten) Then
        '    '        nachricht("Stammdaten wurden angepasst")
        '    '    End If
        '    'End If
        '    If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
        '        Dim zzz As New clsStammCRUD_Oracle(clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
        '        If zzz.EDIT_speichern_stammdaten(myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC,
        '                                         myGlobalz.sitzung.Vorgang.Stammdaten) Then
        '            nachricht("Stammdaten wurden angepasst")
        '        End If
        '    End If
        'End Sub

    End Class

    Public Class NEU_StammSpeichern_alleDB
        Public Shared Function exe(ByVal zeitstempel As Date) As Boolean
            Dim erfolg As Boolean
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            Dim returnIdentity As Boolean = True
            querie = "INSERT INTO " & CLstart.myViewsNTabs.TABSTAMMDATEN & "
                                  (VORGANGSID,BEARBEITER,BEMERKUNG,BESCHREIBUNG,EINGANG,AUFNAHME," +
                                  "LETZTEBEARBEITUNG,ERLEDIGT,ORTSTERMIN,STELLUNGNAHME,PROBAUGAZ,ALTAZ," +
                                  "GEMKRZ,LASTACTIONHEROE,AZ2,WEITEREBEARB,ARCDIR,DARFNICHTVERNICHTETWERDEN,GUTACHTENMIT,GUTACHTENDRIN," &
                                  "STORAUMNR,STOTITEL,ABGABEBA,PARAGRAF,HATRAUMBEZUG,INTERNENR,BEARBEITERID) " +
                            " VALUES (@VORGANGSID,@BEARBEITER,@BEMERKUNG,@BESCHREIBUNG,@EINGANG,@AUFNAHME," +
                                 "@LETZTEBEARBEITUNG,@ERLEDIGT,@ORTSTERMIN,@STELLUNGNAHME,@PROBAUGAZ,@ALTAZ," +
                                 "@GEMKRZ,@LASTACTIONHEROE,@AZ2,@WEITEREBEARB,@ARCDIR,@DARFNICHTVERNICHTETWERDEN,@GUTACHTENMIT," &
                                 "@GUTACHTENDRIN,@STORAUMNR,@STOTITEL,@ABGABEBA,@PARAGRAF,@HATRAUMBEZUG,@INTERNENR,@BEARBEITERID)"

            editStammdaten_alleDB.populateStammdaten(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten, returnIdentity)

            Dim ID As Integer = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
            If ID > 0 Then Return True Else Return False

        End Function

    End Class


    Public Class leseAktenzeichen
        Public Shared Function exe(ByVal vorgangsid%, ByVal dbrec As IDB_grundfunktionen) As Boolean 'myGlobalz.sitzung.VorgangsID	 ,myGlobalz.sitzung.VorgangREC
            'dbrec.mydb.Tabelle ="Vorgang"
            'Return DB_Oracle_sharedfunctions.getDT_("", vorgangsid, dbrec)
            dbrec.mydb.SQL = "select * from  " & CLstart.myViewsNTabs.tabVorgang & " where vorgangsid=" & vorgangsid
            Dim hinweis As String = dbrec.getDataDT()
            If dbrec.dt.IsNothingOrEmpty Then
                l("Fatal Error ID " & "konnte nicht gefunden werden!" & dbrec.mydb.getDBinfo(""))
                Return False
            Else
                Return True
            End If

        End Function
    End Class

    Public Class holeFlureInVorgaengenDT
        Public Shared Function exe() As DataTable
            Dim resultDT As New DataTable
            myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct FLUR  from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
             " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
             " order by FLUR "
            Dim anzahl As Integer = selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultDT) ' "paraflurstueck",
            'If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            '    nachricht(myGlobalz.sitzung.tempREC.getDataDT())
            'End If
            Return resultDT
        End Function
    End Class


    Public Class LoescheStammdaten_alleDB
        Public Shared Function exe(ByVal vid%,
                                    ByVal vorgangsREC As IDB_grundfunktionen,
                                    ByVal stamm As Stamm) As Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten

            Dim hinweis As String
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "DELETE from " & CLstart.myViewsNTabs.tabStammdaten & "  where VorgangsID=" & vid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return True
        End Function
    End Class


    Public Class LoescheVorgang_alleDB
        Public Shared Function exe(ByVal vid As Integer) As Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            'Dim erfolg As Boolean
            'If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New clsVorgangDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
            '    erfolg = zzz.Delete_Vorgang(vid)
            '    zzz.Dispose()
            'End If
            'If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New clsVorgangDB_Oracle(clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
            '    erfolg = zzz.Delete_Vorgang(vid)
            '    zzz.Dispose()
            'End If
            'Return erfolg
        End Function
    End Class
    ''' <summary>
    ''' test
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SpeichernVorgang_alleDB
        Shared Function setSQLbody() As String
            Return " set " &
         " AZ=@AZ" &
         ",SACHGEBIETNR=@SACHGEBIETNR" &
         ",VORGANGSNR=@VORGANGSNR " &
         ",VORGANGSGEGENSTAND=@VORGANGSGEGENSTAND " &
         ",SACHGEBIETSTEXT=@SACHGEBIETSTEXT " &
         ",ISTUNB=@ISTUNB "
        End Function
        Public Shared Function exe(ByVal vid As Integer) As Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim erfolg As Boolean
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            querie = "UPDATE " & CLstart.myViewsNTabs.tabVorgang & " " & setSQLbody() &
                 "  WHERE VORGANGSID=@VORGANGSID"

            clsSqlparam.paramListe.Add(New clsSqlparam("AZ", myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETNR", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSNR", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSGEGENSTAND", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETSTEXT", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header))
            clsSqlparam.paramListe.Add(New clsSqlparam("ISTUNB", CBool(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB())))

            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", vid))

            Dim id As Integer = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")

            If id > 0 Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class

    Public Class EinfuegeVorgang_AlleDB
        Public Shared Function exe() As Boolean
            'Dim erfolg As Boolean
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            querie = "INSERT INTO " & CLstart.myViewsNTabs.tabVorgang &
                                    " (AZ,SACHGEBIETNR,VORGANGSNR,VORGANGSGEGENSTAND,SACHGEBIETSTEXT,ISTUNB) " +
                                    " VALUES (@AZ,@SACHGEBIETNR,@VORGANGSNR,@VORGANGSGEGENSTAND,@SACHGEBIETSTEXT,@ISTUNB)"

            clsSqlparam.paramListe.Add(New clsSqlparam("AZ", myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETNR", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSNR", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSGEGENSTAND", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETSTEXT", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header))
            clsSqlparam.paramListe.Add(New clsSqlparam("ISTUNB", CBool(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB())))

            myGlobalz.sitzung.aktVorgangsID = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "VORGANGSID")

            If myGlobalz.sitzung.aktVorgangsID > 0 Then
                Return True
            Else
                Return False
            End If

        End Function
    End Class

    Public Class selectFromParadigmaTabelle_alleDB
        Public Shared Function exe(ByVal SQL$, ByRef resultDT As DataTable) As Integer 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim hinweis As String
            resultDT = getDT4Query(SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            If resultDT.Rows.Count > 0 Then
                Return resultDT.Rows.Count
            Else
                Return 0
            End If
        End Function
    End Class
End Namespace
