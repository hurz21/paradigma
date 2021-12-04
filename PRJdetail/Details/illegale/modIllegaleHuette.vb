Imports System.Data
Module modIllegaleHuette
    Private Sub MappingDB2obj(ByVal ntmphuette As clsIllegaleHuette, ByVal dt As DataTable)
        ntmphuette.illegID = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("illegid")))
        ntmphuette.gebiet = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("gebiet")))
        ntmphuette.status = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("status")))
        ntmphuette.vermerk = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("vermerk")))
        ntmphuette.anhoerung = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("anhoerung")))
        ntmphuette.raeumungBisDatum = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("raeumungBisDatum")))
        ntmphuette.raeumung = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("raeumung")))
        ntmphuette.raeumungsTyp = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("raeumungstyp")))
        ntmphuette.verfuegung = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("verfuegung")))
        ntmphuette.fallerledigt = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("fallerledigt")))
        ntmphuette.eid_anhoerung = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("eid_anhoerung")))
        ntmphuette.eid_raeumung = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("eid_raeumung")))
        ntmphuette.eid_verfuegung = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("eid_verfuegung")))
        ntmphuette.quelle = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("quelle")))
        ntmphuette.ts = CDate(clsDBtools.fieldvalueDate(dt.Rows(0).Item("ts")))
    End Sub
    Function getIllegale4Vid(vid As Integer) As clsIllegaleHuette
        Dim ntmphuette As New clsIllegaleHuette : Dim hinweis As String = ""
        Dim dt As DataTable = Nothing
        'myGlobalz.sitzung.VorgangREC.mydb.Tabelle =" " & CLstart.myViewsNTabs.tabIllegbau & " "
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from  " & CLstart.myViewsNTabs.tabIllegbau & "  where vorgangsid=" & vid
        dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)

        If dt.Rows.Count < 1 Then
            Return ntmphuette
        Else
            MappingDB2obj(ntmphuette, dt)
        End If
        Return ntmphuette
    End Function


    Sub speichernAnhoerungsDatum(nullable As Date?)
        'ihah
    End Sub

    Function EreignisErzeugen(datum As Date?, Art As String, Titel As String) As clsEreignis
        Dim aaa As New clsEreignis
        aaa.Art = Art
        aaa.Beschreibung = Titel
        aaa.Datum = CDate(datum)
        aaa.istRTF = False
        aaa.Notiz = ""
        aaa.Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
        aaa.Richtung = "Ausgang"
        Return aaa
    End Function

    Function Ereignisspeichern(clsEreignis As clsEreignis) As Integer
        If Not clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", clsEreignis) Then
            Debug.Print("scheisse ")
            Return 0
        Else
            Debug.Print("gutt " & myGlobalz.sitzung.aktEreignis.ID)
            Return myGlobalz.sitzung.aktEreignis.ID
        End If
    End Function

    Sub ereignisoeffnen(clsEreignis As clsEreignis, readOnlyDoxsInTxtCrtlOeffnen As Boolean)
        myGlobalz.sitzung.aktEreignis.ID = CInt(clsEreignis.ID)
        myGlobalz.sitzung.Ereignismodus = "edit"
        glob2._Ergeignis_edit(readOnlyDoxsInTxtCrtlOeffnen)
    End Sub

    Sub speichernRaeumungsDatum(nullable As Date?)
        'ihah
    End Sub

    Sub speichernVerfuegungsDatum(nullable As Date?)
        'ihah
    End Sub

    'Sub huettespeichern(p1 As String, clsIllegaleHuette As LIBillegBau.clsIllegaleHuette)
    Public Function huettespeichern(modus As String, clsIllegaleHuette As clsIllegaleHuette) As Integer
        Dim illegID As Integer
        If modus = "neu" Then
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            querie = "INSERT INTO  " & CLstart.myViewsNTabs.tabIllegbau & "  (VORGANGSID,VERMERK,Status,GEBIET,RAEUMUNGSTYP,ANHOERUNG,RAEUMUNGBISDATUM," &
                                          "RAEUMUNG,VERFUEGUNG,FALLERLEDIGT," &
                                          "EID_ANHOERUNG,EID_RAEUMUNG,EID_VERFUEGUNG,QUELLE) " &
                               " VALUES (@VORGANGSID,@VERMERK,@Status,@GEBIET,@RAEUMUNGSTYP,@ANHOERUNG,@RAEUMUNGBISDATUM," &
                                          "@RAEUMUNG,@VERFUEGUNG,@FALLERLEDIGT," &
                                          "@EID_ANHOERUNG,@EID_RAEUMUNG,@EID_VERFUEGUNG,@QUELLE)"
            populateHuette(clsIllegaleHuette)
            illegID = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ILLEGID")

            Return illegID
        End If
        If modus = "edit" Then
            Dim querie As String
            Dim anzahlTreffer As Integer
            clsSqlparam.paramListe.Clear()
            querie = "UPDATE   " & CLstart.myViewsNTabs.tabIllegbau & "  " & " SET VORGANGSID=@VORGANGSID" &
                    ",VERMERK=@VERMERK" &
                    ",GEBIET=@GEBIET" &
                    ",Status=@Status" &
                    ",RAEUMUNGSTYP=@RAEUMUNGSTYP" &
                    ",ANHOERUNG=@ANHOERUNG" &
                    ",RAEUMUNGBISDATUM=@RAEUMUNGBISDATUM" &
                    ",RAEUMUNG=@RAEUMUNG" &
                    ",VERFUEGUNG=@VERFUEGUNG" &
                    ",FALLERLEDIGT=@FALLERLEDIGT" &
                    ",EID_ANHOERUNG=@EID_ANHOERUNG" &
                    ",EID_RAEUMUNG=@EID_RAEUMUNG" &
                    ",EID_VERFUEGUNG=@EID_VERFUEGUNG" &
                    ",QUELLE=@QUELLE" &
                    " WHERE ILLEGID=@ILLEGID"
            populateHuette(clsIllegaleHuette)
            clsSqlparam.paramListe.Add(New clsSqlparam("ILLEGID", clsIllegaleHuette.illegID))
            anzahlTreffer = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ILLEGID")

            Return anzahlTreffer
        End If
        Return 0
    End Function

    Private Sub populateHuette(clsIllegaleHuette As clsIllegaleHuette)
        With clsIllegaleHuette
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", .vid))
            clsSqlparam.paramListe.Add(New clsSqlparam("VERMERK", (.vermerk)))
            clsSqlparam.paramListe.Add(New clsSqlparam("GEBIET", (.gebiet)))
            clsSqlparam.paramListe.Add(New clsSqlparam("Status", (.status)))
            clsSqlparam.paramListe.Add(New clsSqlparam("RAEUMUNGSTYP", (.raeumungsTyp)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ANHOERUNG", (.anhoerung)))
            clsSqlparam.paramListe.Add(New clsSqlparam("RAEUMUNGBISDATUM", (.raeumungBisDatum)))
            clsSqlparam.paramListe.Add(New clsSqlparam("RAEUMUNG", (.raeumung)))
            clsSqlparam.paramListe.Add(New clsSqlparam("VERFUEGUNG", (.verfuegung)))
            clsSqlparam.paramListe.Add(New clsSqlparam("FALLERLEDIGT", (.fallerledigt)))
            clsSqlparam.paramListe.Add(New clsSqlparam("EID_ANHOERUNG", (.eid_anhoerung)))
            clsSqlparam.paramListe.Add(New clsSqlparam("EID_RAEUMUNG", (.eid_raeumung)))
            clsSqlparam.paramListe.Add(New clsSqlparam("EID_VERFUEGUNG", (.eid_verfuegung)))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", (.quelle)))
        End With
    End Sub

    Function loeschen(illegid As Integer) As Boolean
        Dim hinweis As String = ""
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from  " & CLstart.myViewsNTabs.tabIllegbau & "  where illegid=" & illegid
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
        Return True
    End Function

    Function NochKeinGebietErfasst(clsIllegaleHuette As clsIllegaleHuette) As Boolean
        If clsIllegaleHuette.gebiet.IsNothingOrEmpty() OrElse clsIllegaleHuette.gebiet = "0" Then
            Return True
        Else
            Return False
        End If
    End Function

    Function IstAnhoerungVorhanden(clsIllegaleHuette As clsIllegaleHuette) As Boolean
        If glob2.IstDatumSinnvoll(clsIllegaleHuette.anhoerung) Then
            Return True
        Else
            Return False
        End If
    End Function






End Module
