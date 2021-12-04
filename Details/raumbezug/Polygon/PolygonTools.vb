Imports System.Data

Public Class PolygonTools
    Public Shared Sub zum_dgPolygonEditmode()
        Dim sekid = CStr(myGlobalz.sitzung.aktPolygon.SekID)
        Dim wfst As Win_Polygon
        RB_ParaPolygon_holen(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
        If Not DTaufPolygonObjektabbilden(myGlobalz.sitzung.tempREC.dt) Then
            MsgBox("Es wurde kein Polygon gefunden")
        End If
        'form aufrufen
        If myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polygon Then
            myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Polygon
            wfst = New Win_Polygon("flaeche")
            wfst.ShowDialog()
        End If
        If myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polyline Then
            myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Polyline
            wfst = New Win_Polygon("strecke")
            wfst.ShowDialog()
        End If

    End Sub

    Public Shared Function RB_ParaPolygon_holen(ByVal rid As Long) As Boolean 'myGlobalz.sitzung.aktPolygon.RaumbezugsID
        Dim hinweis As String
        Try
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema

            myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug2geopolygon"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "select typ,areaqm,serialshape,vorgangsid,id from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             " where raumbezugsid=" & rid
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim RB_ParaPolygon_holen: " & ex.ToString)
            Return False
        End Try
    End Function

    Shared Function DTaufPolygonObjektabbilden(ByVal dasDT As DataTable) As Boolean
        Try
            If dasDT Is Nothing OrElse dasDT.Rows Is Nothing Then
                nachricht("DTaufPolygonObjektabbilden: datatable ist nothing!")
                Return False
            End If
            If dasDT.Rows.Count < 1 Then
                nachricht("DTaufPolygonObjektabbilden: datatable hat keien zeilen!")
                Return False
            End If
            With myGlobalz.sitzung.aktPolygon
                .Typ = CType(CInt(clsDBtools.fieldvalue(dasDT.Rows(0).Item("typ"))), RaumbezugsTyp)
                .Area = CDbl(clsDBtools.fieldvalue(dasDT.Rows(0).Item("areaqm")))
                .ShapeSerial = clsDBtools.fieldvalue(dasDT.Rows(0).Item("serialshape"))
                .gkstringausserial_generieren()
            End With
            Return True
        Catch ex As Exception
            nachricht("Fehler1: DTaufFotoObjektabbilden " & vbCrLf & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Shared Function DTaufPolylineObjektabbilden(ByVal dasDT As DataTable) As Boolean
        Try
            If dasDT Is Nothing OrElse dasDT.Rows Is Nothing Then
                nachricht("DTaufPolylineObjektabbilden: datatable ist nothing!")
                Return False
            End If
            If dasDT.Rows.Count < 1 Then
                nachricht("DTaufPolylineObjektabbilden: datatable hat keien zeilen!")
                Return False
            End If
            With myGlobalz.sitzung.aktPolyline
                .Typ = CType(CInt(clsDBtools.fieldvalue(dasDT.Rows(0).Item("typ"))), RaumbezugsTyp)
                .Distanz = CDbl(clsDBtools.fieldvalue(dasDT.Rows(0).Item("areaqm")))
                .ShapeSerial = clsDBtools.fieldvalue(dasDT.Rows(0).Item("serialshape"))
                .gkstringausserial_generieren()
            End With
            Return True
        Catch ex As Exception
            nachricht("Fehler1: DTaufPolylineObjektabbilden " & vbCrLf & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Public Shared Sub Polygon_Und_Vorgang_Entkoppeln(ByRef erfolg As Integer, rid As Integer)
        'erfolg = DBactionParadigma.Entkoppelung_Raumbezug_Vorgang(CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID), myGlobalz.sitzung.VorgangsID)
        erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(rid, myGlobalz.sitzung.aktVorgangsID)
        If erfolg > 0 Then
            My.Log.WriteEntry("Objekt wurde erfolgreich gelöscht")
        Else
            My.Log.WriteEntry("Objekt wurde erfolgreich gelöscht")
            nachricht_und_Mbox("Problem beim Löschen des Raumbezugs aus dem Vorgang. Abbruch.")
        End If
    End Sub

    Shared Sub polygonSeriallSpeichernImHintergrund(ByVal vid As Integer, ByVal rbid As Integer, ByVal rbtyp As Integer,
                                                    ByVal serialstring As String, ByVal Flaecheqm As Double)
        '(myGlobalz.sitzung.VorgangsID,myGlobalz.sitzung.aktPolygon.RaumbezugsID, RaumbezugsTyp.Polygon,  myGlobalz.sitzung.aktPolygon.ShapeSerial, myGlobalz.sitzung.aktPolygon.Area
        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            Dim zzz As New FST_serialShape_mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            zzz.serialInDbSpeichern(vid, CInt(rbid), rbtyp, serialstring, Flaecheqm)
            zzz.Dispose()
        End If
        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            Dim zzz As New FST_serialShape_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            zzz.serialInDbSpeichern(vid, CInt(rbid), rbtyp, serialstring, Flaecheqm)
            zzz.Dispose()
        End If
    End Sub

    Public Shared Function polygonLoeschen(raumbezugsid As Integer) As Boolean
        'TRANSACTION: alle drei löschungen in einer TRANSACTION
        Dim erfolg As Integer
        Dim tempoRID As Integer = raumbezugsid
        If CInt(myGlobalz.sitzung.aktPolygon.Status) = 0 Then
            erfolg = RBtoolsns.Raumbezug_loeschen_byid_alleDB.execute(tempoRID)
            If erfolg > 0 Then
                PolygonTools.Polygon_Und_Vorgang_Entkoppeln(erfolg, tempoRID)
                If RBtoolsns.RB_Flurstueck_Serial_loeschen.exe(CInt(tempoRID)) < 0 Then
                    nachricht("Fehler oder keine RBs vorhanden: vorgang: " & myGlobalz.sitzung.aktVorgangsID &
                                    " rbsekid:" & myGlobalz.sitzung.aktFST.SekID)
                    Return False
                Else
                    Return True
                End If
            Else
                nachricht("Problem beim Löschen des Raumbezugs. Abbruch.")
                nachricht_und_Mbox("Problem beim Löschen des Raumbezugs. Abbruch.")
                Return False
            End If
        Else
            PolygonTools.Polygon_Und_Vorgang_Entkoppeln(erfolg, CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID))
        End If
        Return True
    End Function

    'Shared Function polygonSchonInVorgangVorhanden(ByVal clsParapolygon As clsParapolygon, ByVal vorgangsid As Integer) As Boolean
    '    myGlobalz.sitzung.tempREC.mydb.SQL =
    '              "select * from raumbezug p, raumbezug2geopolygon s " &
    '              " where p.typ=3" &
    '              " and serialshape='" & clsParapolygon.ShapeSerial & "'" &
    '              " and areaqm=" & clsParapolygon.Area &
    '              " and s.vorgangsid= " '& vorgangsid &
    '    ' " and s.sekid=p.sekid"
    '    nachricht("polygonSchonInVorgangVorhanden sql: " & myGlobalz.sitzung.tempREC.mydb.SQL)
    '    Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
    '    If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
    '        nachricht("flurstueck ist schon in vorgang vorhanden")
    '        Return True
    '    Else
    '        nachricht("flurstueck ist noch nicht in vorgang vorhanden")
    '        Return False
    '    End If
    'End Function

    'Shared Function polylineSchonInVorgangVorhanden(ByVal clsParapolyline As clsParapolyline, ByVal vorgangsid As Integer) As Boolean
    '    myGlobalz.sitzung.tempREC.mydb.SQL =
    '              "select * from raumbezug p, raumbezug2geopolygon s " &
    '              " where p.typ=4" &
    '              " and serialshape='" & clsParapolyline.ShapeSerial & "'" & 
    '              " and s.vorgangsid= " & vorgangsid ' &
    '    '  " and s.sekid=p.sekid"
    '    nachricht("polylineSchonInVorgangVorhanden sql: " & myGlobalz.sitzung.tempREC.mydb.SQL)
    '    Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
    '    If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
    '        nachricht("polylineSchonInVorgangVorhanden ist schon in vorgang vorhanden")
    '        Return True
    '    Else
    '        nachricht("polylineSchonInVorgangVorhanden ist noch nicht in vorgang vorhanden")
    '        Return False
    '    End If
    'End Function

    Public Shared Sub PolygonNeuSpeichern(ByRef sekID As Integer)
        Try
            nachricht("PolygonNeuSpeichern: sekid: " & sekID)
            myGlobalz.sitzung.aktPolygon.serialAusGkstring_generieren(myGlobalz.sitzung.aktPolygon.Typ)
            myGlobalz.sitzung.aktPolygon.SekID = sekID
            myGlobalz.sitzung.aktPolygon.defineAbstract()

            myGlobalz.sitzung.aktPolygon.defineBboxfromGKstring(myGlobalz.sitzung.aktPolygon.box.xl,
                                                                myGlobalz.sitzung.aktPolygon.box.xh,
                                                                myGlobalz.sitzung.aktPolygon.box.yl,
                                                                myGlobalz.sitzung.aktPolygon.box.yh)
            myGlobalz.sitzung.aktPolygon.box.CalcCenter() : nachricht("PolygonNeuSpeichern: CalcCenter: ")
            myGlobalz.sitzung.aktPolygon.punkt.X = CInt(myGlobalz.sitzung.aktPolygon.box.xcenter)
            myGlobalz.sitzung.aktPolygon.punkt.Y = CInt(myGlobalz.sitzung.aktPolygon.box.ycenter)

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
            nachricht("PolygonNeuSpeichern: polygonSeriallSpeichernImHintergrund%: ok")
        Catch ex As Exception
            nachricht("Fehler in PolygonNeuSpeichern: " & ex.ToString)
        End Try
    End Sub


    Public Shared Sub editPolygonspeichernExtracted(ByVal sekID%)
        myGlobalz.sitzung.aktPolygon.SekID = sekID
        myGlobalz.sitzung.aktPolygon.defineAbstract()
        'neuer schwerpunkt
        myGlobalz.sitzung.aktPolygon.punkt.X = clstart.myc.kartengen.aktMap.aktrange.xl + clstart.myc.kartengen.aktMap.aktrange.xdif / 2
        myGlobalz.sitzung.aktPolygon.punkt.Y = clstart.myc.kartengen.aktMap.aktrange.yl + clstart.myc.kartengen.aktMap.aktrange.ydif / 2

        DBraumbezug_Mysql.defineBBOX(clstart.myc.kartengen.aktMap.aktrange.xdif / 2, myGlobalz.sitzung.aktPolygon)
        nachricht("raumbezugsID%: " & RBtoolsns.Raumbezug_edit_alleDB.execute(CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID),
                                                                                    myGlobalz.sitzung.aktPolygon).ToString)
        'nachricht("raumbezugsID%: " & DBraumbezug_Mysql.Raumbezug_abspeichern_Edit(CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID),
        '                                                                             myGlobalz.sitzung.aktPolygon).ToString)
        'das polygon bleibt unverändert
    End Sub

    Shared Function calcDistanz(winpt As Point, myPoint As myPoint) As Double
        Dim aa, bb, cc As Double
        If myPoint.X = 0 Then Return 0
        aa = winpt.X - myPoint.X
        bb = winpt.Y - myPoint.Y
        cc = Math.Sqrt((aa * aa) + (bb * bb))
        Return cc
    End Function

    Public Shared Sub polylineAufPolygonUmsetzen()
        myGlobalz.sitzung.aktPolygon.GKstring = myGlobalz.sitzung.aktPolyline.GKstring
        myGlobalz.sitzung.aktPolygon.Area = CInt(myGlobalz.sitzung.aktPolyline.Distanz)
        myGlobalz.sitzung.aktPolygon.LaengeM = CInt(myGlobalz.sitzung.aktPolyline.Distanz)
        myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polyline
    End Sub

End Class
