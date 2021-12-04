Imports System.Data

Public Class fremdRBtools
    Friend Shared Function genFremdRB(gemkrz As String, textfilter As String, geometrieTyp As String, topstring As String, bearbeiter As String) As String
        Dim selectstring As String = ""
        Dim fromstring As String = ""
        Dim wherestring As String = ""
        Dim orderstring As String = ""
        Dim summe As String = ""
        Dim gemeindestring As String = ""
        Dim bearbeiterstring As String = ""
        Dim geometrietypstring As String = ""
        Dim textfilterstring As String = ""
        Try
            l(" MOD genFremdRB anfang")
            selectstring = genselectstring(topstring)
            '    Return CLstart.myViewsNTabs.view_raumbezugPolygone
            gemeindestring = gengemeindestring(gemkrz)
            fromstring = genFromstring()
            geometrietypstring = gengeometrietypstring(geometrieTyp)
            textfilterstring = gentextfilterstring(textfilter.Trim)
            bearbeiterstring = genBearbeiterstring(bearbeiter)
            wherestring = genWherestring(geometrietypstring, textfilterstring, bearbeiterstring, gemeindestring)
            orderstring = genOrderstring()
            summe = selectstring & fromstring & wherestring & orderstring
            l(" MOD genFremdRB ende")
            Return summe
        Catch ex As Exception
            l("Fehler in genFremdRB: ", ex)
            Return summe
        End Try
    End Function

    Private Shared Function gengemeindestring(gemkrz As String) As String
        Dim result As String = ""
        Try
            l(" MOD gengemeindestring anfang")
            If gemkrz.IsNothingOrEmpty Then Return ""
            If gemkrz.Trim.Count = 2 Then
                result = " and ( LTRIM(RTRIM(stamm.gemkrz)) like '" & gemkrz.Trim & "%') "
            End If
            If gemkrz.Trim.Count = 5 Then
                result = " and ( LTRIM(RTRIM(stamm.gemkrz)) = '" & gemkrz.Trim & "') "
            End If
            If gemkrz.Trim = "ALLE-" Then Return ""
            l(" MOD gengemeindestring ende")
            Return result
        Catch ex As Exception
            l("Fehler in gengemeindestring: ", ex)
            Return result
        End Try
    End Function

    Friend Shared Sub handleChosenRB(item As DataRowView)
        'Dim item As DataRowView = CType(dgRaumbezug.SelectedItem, DataRowView)
        If item Is Nothing Then Return
        '    myGlobalz.sitzung.raumbezugsmodus = "edit"
        Dim rbtyp As String = item("TYP").ToString

        Select Case rbtyp
            'Case CInt(RaumbezugsTyp.Adresse).ToString
            '    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktADR)
            '    myGlobalz.sitzung.aktADR.setcoordsAbstract()
            '    glob2.zum_dgAdresse_Editmode()
            Case CInt(RaumbezugsTyp.Flurstueck).ToString
                glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktFST)
                myGlobalz.sitzung.aktFST.setcoordsAbstract()
                Dim sekid% = CInt((myGlobalz.sitzung.aktFST.SekID))
                ' DBraumbezug_Mysql.RB_Flurstueck_holen(sekid$)  'auf temprec
                RBtoolsns.RB_Flurstueck_holen_alleDB.exe(CType(sekid, String))
                FST_tools.DTaufFSTObjektabbilden(myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktFST)

                sekid = FST_tools.RBFlstNeu_alleDB()
                If sekid > 0 Then
                    myGlobalz.sitzung.aktFST.SekID = sekid
                    myGlobalz.sitzung.aktFST.defineAbstract()
                    DBraumbezug_Mysql.defineBBOX(200, myGlobalz.sitzung.aktFST)
                    Dim raumbezugsID% = RBtoolsns.Raumbezug_abspeichern_Neu_alleDB.execute(myGlobalz.sitzung.aktFST)
                    nachricht("kooplungsid: " & RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(raumbezugsID, myGlobalz.sitzung.aktVorgangsID, 0).ToString)
                    myGlobalz.sitzung.aktFST.RaumbezugsID = raumbezugsID
                    glob2.generateAndSaveSerialShapeInDb()
                    MsgBox("Flurstück wurde übernommen")
                Else
                    nachricht("NeuesFSTspeichern: Problem beim Abspeichernd!")
                End If
            'Case CInt(RaumbezugsTyp.Foto).ToString
            '    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktParaFoto)
            '    '    myGlobalz.sitzung.aktParaFoto.dokumentid = CStr(item("DOKUMENTID"))
            '    myGlobalz.sitzung.aktParaFoto.setcoordsAbstract()
            '    zum_dgFotoEditmode()
            'Case CInt(RaumbezugsTyp.Umkreis).ToString
            '    glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktPMU)
            '    myGlobalz.sitzung.aktParaFoto.setcoordsAbstract()
            '    ParaUmkreisTools.zum_dgUmkreisEditmode("")'UTMCoordinate.Text)
            Case CInt(RaumbezugsTyp.Polygon).ToString, CInt(RaumbezugsTyp.Polyline).ToString
                glob2.raumbezugsDataRowView2OBJ(item, myGlobalz.sitzung.aktPolygon)
                'PolygonTools.zum_dgPolygonEditmode()

                Dim sekid = CStr(myGlobalz.sitzung.aktPolygon.SekID)
                Dim wfst As Win_Polygon
                PolygonTools.RB_ParaPolygon_holen(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
                If Not PolygonTools.DTaufPolygonObjektabbilden(myGlobalz.sitzung.tempREC.dt) Then
                    MsgBox("Es wurde kein Polygon gefunden")
                End If
                'form aufrufen
                If myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polygon Then
                    myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Polygon
                    Dim raumbezugsID As Integer = RBtoolsns.Raumbezug_abspeichern_Neu_alleDB.execute(myGlobalz.sitzung.aktPolygon)
                    nachricht("PolygonNeuSpeichern: raumbezugsID%: " & raumbezugsID)
                    nachricht("koppelungsid: " & RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(raumbezugsID,
                                                                                                            myGlobalz.sitzung.aktVorgangsID,
                                                                                                            0).ToString)
                    myGlobalz.sitzung.aktPolygon.RaumbezugsID = raumbezugsID
                    PolygonTools.polygonSeriallSpeichernImHintergrund(myGlobalz.sitzung.aktVorgangsID,
                                                                      CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID),
                                                                      myGlobalz.sitzung.aktPolygon.Typ,
                                                                      myGlobalz.sitzung.aktPolygon.ShapeSerial,
                                                                      myGlobalz.sitzung.aktPolygon.Area)
                    MsgBox("Polygon wurde übernommen")
                End If
                If myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polyline Then
                    myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Polyline
                    wfst = New Win_Polygon("strecke")
                    wfst.ShowDialog()
                End If
        End Select

    End Sub

    Private Shared Function genBearbeiterstring(bearbeiter As String) As String
        Dim result As String = ""
        Try
            l(" MOD genBearbeiterstring anfang")
            If bearbeiter.IsNothingOrEmpty Then Return ""
            result = " and ( stamm.bearbeiter = '" & bearbeiter.Trim & "') "
            l(" MOD genBearbeiterstring ende")
            Return result
        Catch ex As Exception
            l("Fehler in genBearbeiterstring: ", ex)
            Return result
        End Try
    End Function

    Private Shared Function gentextfilterstring(textfilter As String) As String
        Dim result As String = ""
        Try
            l(" MOD gentextfilterstring anfang")
            If textfilter.IsNothingOrEmpty Then Return ""
            result = " and ( r.ABSTRACT like '%" & textfilter.Trim & "%' or r.Freitext like '%" & textfilter.Trim & "%' or r.titel like '%" & textfilter.Trim & "%' or STAMM.BESCHREIBUNG like '%" & textfilter.Trim & "%') "
            l(" MOD gentextfilterstring ende")
            Return result
        Catch ex As Exception
            l("Fehler in gentextfilterstring: ", ex)
            Return result
        End Try
    End Function

    Private Shared Function genOrderstring() As String
        Dim result As String = ""
        Try
            l(" MOD genOrderstring anfang")

            result = " order by p.VORGANGSID desc, r.ABSTRACT   "
            l(" MOD genOrderstring ende")
            Return result
        Catch ex As Exception
            l("Fehler in genOrderstring: ", ex)
            Return result
        End Try
    End Function

    Private Shared Function gengeometrietypstring(geometrieTyp As String) As String
        Dim result As String = ""
        Try
            l(" MOD gengeometrietypstring anfang")
            If geometrieTyp.IsNothingOrEmpty Then Return ""
            result = " t.typ=" & geometrieTyp & " "
            result = "  and ( p.typ=" & geometrieTyp & ")    "
            l(" MOD gengeometrietypstring ende")
            Return result
        Catch ex As Exception
            l("Fehler in gengeometrietypstring: ", ex)
            Return result
        End Try
    End Function

    Private Shared Function genWherestring(geometrietypstring As String, textfilterstring As String, bearbeiterstring As String, gemeindestring As String) As String
        Dim result As String = ""
        Try
            l(" MOD genWherestring anfang")
            result = " where p.VORGANGSID=stamm.VORGANGSID and  stamm.VORGANGSID=vorgang.vorgangsid and   r.RAUMBEZUGSID=p.RAUMBEZUGSID    " & geometrietypstring & textfilterstring & bearbeiterstring & gemeindestring
            l(" MOD genWherestring ende")
            Return result
        Catch ex As Exception
            l("Fehler in genWherestring: ", ex)
            Return result
        End Try
    End Function

    Private Shared Function genFromstring() As String
        Dim result As String = ""
        Try
            l(" MOD genFromstring anfang")
            result = " FROM [Paradigma].[dbo].[" & CLstart.myViewsNTabs.tabRaumbezug2geopolygon & "   ] p, " & CLstart.myViewsNTabs.tabStammdaten & " stamm, " & CLstart.myViewsNTabs.tabVorgang & " vorgang,raumbezug r  "
            l(" MOD genFromstring ende")
            Return result
        Catch ex As Exception
            l("Fehler in genFromstring: ", ex)
            Return result
        End Try
    End Function

    Private Shared Function genselectstring(topstring As String) As String
        Dim result As String = ""
        Try
            l(" MOD genselectstring anfang")
            If topstring.IsNothingOrEmpty Then topstring = "100"
            'result = "SELECT TOP (" & topstring & ") p.[ID],p.[RAUMBEZUGSID],p.[VORGANGSID],r.FREITEXT,r.ABSTRACT,r.TITEL,stamm.AZ2,[SERIALSHAPE],p.[TYP] as TYP,r.[TYP] as RTYP,[AREAQM],[SERIALUTM]," &
            '    "stamm.[GEMKRZ],stamm.[BEARBEITER],stamm.[BESCHREIBUNG] as STAMMBESCHREIBUNG "
            result = "SELECT TOP (" & topstring & ") p.[ID],p.[RAUMBEZUGSID],p.[VORGANGSID],r.*,0 as STATUS,stamm.AZ2,[SERIALSHAPE],p.[TYP] as TYP,[AREAQM],[SERIALUTM]," &
                "stamm.[GEMKRZ],stamm.[BEARBEITER],stamm.[BESCHREIBUNG] as STAMMBESCHREIBUNG "
            l(" MOD genselectstring ende")
            Return result
        Catch ex As Exception
            l("Fehler in genselectstring: ", ex)
            Return result
        End Try
    End Function
End Class
