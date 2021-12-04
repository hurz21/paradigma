Imports System.Data
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Namespace VSTTools
    Public Class editStammdaten_alleDB
        Public Shared Function exe(ByRef vid%, ByVal stamm As Stamm) As Boolean ', myGlobalz.sitzung.Vorgang.Stammdaten

            Dim erfolg As Boolean

            Dim querie As String
            clsSqlparam.paramListe.Clear()
            Dim returnIdentity As Boolean = False
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
            l("editStammdaten_alleDB exe id: " & ID)
            If ID > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Shared Sub populateStammdaten(vid As Integer, stamm As Stamm, returnIdentity As Boolean)
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", vid%)) 'MYGLObalz.sitzung.VorgangsID)
            clsSqlparam.paramListe.Add(New clsSqlparam("BEARBEITER", stamm.hauptBearbeiter.Initiale.Trim))
            l(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
            l(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID.ToString)

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
    End Class

    Public Class NEU_StammSpeichern_alleDB
        Public Shared Function exe(ByVal zeitstempel As Date) As Boolean

            Dim erfolg As Boolean

            Dim querie As String
            clsSqlparam.paramListe.Clear()
            Dim returnIdentity As Boolean = True
            If myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID = 0 Then
                l("fehler NEU_StammSpeichern_alleDB exe hauptBearbeiter.ID = 0 ")
                'If 
                'myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID = myGlobalz.sitzung.aktBearbeiter.ID
            End If

            querie = "INSERT INTO " & CLstart.myViewsNTabs.TABSTAMMDATEN &
                                " (VORGANGSID,BEARBEITER,BEMERKUNG,BESCHREIBUNG,EINGANG,AUFNAHME," +
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
            'Return DB_Oracle_sharedfunctions.getDT_("",vorgangsid, dbrec)
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
        Public Shared Sub exe()
            Dim resultDT As New DataTable
            myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct flur  from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
             " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
             " order by flur "
            Dim anzahl As Integer = selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultDT)
            myGlobalz.sitzung.tempREC.dt = resultDT.Copy
            'If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            '    nachricht(myGlobalz.sitzung.tempREC.getDataDT())
            'End If
        End Sub
    End Class


    Public Class LoescheStammdaten_alleDB
        Public Shared Function exe(ByVal vid%,
                                    ByVal vorgangsREC As IDB_grundfunktionen,
                                    ByVal stamm As Stamm) As Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "DELETE from " & CLstart.myViewsNTabs.tabStammdaten & "  where VorgangsID=" & vid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return True
        End Function
    End Class


    Public Class LoescheVorgang_alleDB
        Public Shared Function exe(ByVal vid As Integer) As Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "DELETE FROM " & CLstart.myViewsNTabs.tabVorgang & "  WHERE VORGANGSID=" & vid

            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return True
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
#Disable Warning BC42024 ' Unused local variable: 'erfolg'.
            Dim erfolg As Boolean
#Enable Warning BC42024 ' Unused local variable: 'erfolg'.
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            querie = "UPDATE " & CLstart.myViewsNTabs.tabVorgang & " " & setSQLbody() &
                 "  WHERE VORGANGSID=@VORGANGSID"

            clsSqlparam.paramListe.Add(New clsSqlparam("AZ", myglobalz.sitzung.aktVorgang.Stammdaten.az.gesamt))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETNR", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSNR", myglobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSGEGENSTAND", myglobalz.sitzung.aktVorgang.Stammdaten.az.Prosa))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETSTEXT", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header))
            clsSqlparam.paramListe.Add(New clsSqlparam("ISTUNB", CBool(myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB())))

            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", vid))

            Dim id As Integer = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")

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

            clsSqlparam.paramListe.Add(New clsSqlparam("AZ", myglobalz.sitzung.aktVorgang.Stammdaten.az.gesamt))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETNR", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSNR", myglobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSGEGENSTAND", myglobalz.sitzung.aktVorgang.Stammdaten.az.Prosa))
            clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETSTEXT", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header))
            clsSqlparam.paramListe.Add(New clsSqlparam("ISTUNB", CBool(myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB())))

            myglobalz.sitzung.aktVorgangsID = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "VORGANGSID")

            If myglobalz.sitzung.aktVorgangsID > 0 Then
                Return True
            Else
                Return False
            End If

        End Function
    End Class

    Public Class selectFromParadigmaTabelle_alleDB
        Public Shared Function exe(ByVal SQL$, ByRef resultDT As DataTable) As Integer 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = SQL$
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            resultDT = myGlobalz.sitzung.VorgangREC.dt.Copy
            Return 1
        End Function
    End Class
End Namespace
