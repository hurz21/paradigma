Imports System.Data

Namespace NSpostgis

    Class clsPostgis
        'Public Shared Function bildeTextinfo(aktGID As Integer, aktTabelle As String, aktSchema As String, Optional ByVal fromview As Boolean = True) As String
        '    Dim summe As String = ""
        '    Dim prefix As String = ".v_" : If Not fromview Then prefix = "."
        '    Dim trenner As String = Environment.NewLine
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT * FROM " & aktSchema & prefix & aktTabelle & " where gid=" & aktGID
        '    Try
        '        Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
        '        For i = 1 To myGlobalz.sitzung.postgresREC.dt.Columns.Count - 2
        '            summe = summe & myGlobalz.sitzung.postgresREC.dt.Columns(i).ColumnName & ": " & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(i)) & trenner
        '        Next
        '        Return summe
        '    Catch ex As Exception
        '        nachricht("Fehler in bildeTextinfo: ", ex)
        '        Return ""
        '    End Try
        'End Function


        Public Shared Function holeKoordinatenFuerGID(aktGID As Integer, aktTabelle As String, aktSchema As String, Optional ByVal fromview As Boolean = True) As String
            Dim prefix As String = ".v_" : If Not fromview Then prefix = "."
            myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT ST_EXTENT(geom) FROM " & aktSchema & prefix & aktTabelle & " where gid=" & aktGID
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0))
            Catch ex As Exception
                nachricht("Fehler in holeKoordinatenFuerGID: ", ex)
                Return ""
            End Try
        End Function



        Public Shared Function holeGID4Fs(aktTabelle As String, aktSchema As String, fs As String) As Integer
            myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT gid FROM " & aktSchema & "." & aktTabelle & " where fs='" & fs & "'"
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0)))
            Catch ex As Exception
                nachricht("Fehler in holeGID4Fs: ", ex)
                Return -1
            End Try
        End Function
        'Public Shared Function holePolygonFuerGID(aktGID As Integer, aktTabelle As String, aktSchema As String, Optional ByVal fromview As Boolean = True) As String
        '    Dim prefix As String = ".v_" : If Not fromview Then prefix = "."
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECt ST_AsText(geom)  FROM " & aktSchema & prefix & aktTabelle & " where gid=" & aktGID
        '    Try
        '        Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
        '        Return clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0))
        '    Catch ex As Exception
        '        nachricht("Fehler in holePolygonFuerGID: ", ex)
        '        Return ""
        '    End Try
        'End Function

        Public Shared Function holeAreaFuerGID(aktGID As Integer, aktTabelle As String, aktSchema As String, Optional ByVal fromview As Boolean = True) As Double
            Dim prefix As String = ".v_" : If Not fromview Then prefix = "."
            myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECt area(geom)  FROM " & aktSchema & prefix & aktTabelle & " where gid=" & aktGID
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return CDbl(clsDBtools.fieldvalue(clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0))))
            Catch ex As Exception
                nachricht("Fehler in holeAreaFuerGID: ", ex)
                Return -1
            End Try
        End Function

        Public Shared Function holePUFFERPolygonFuerGID(aktGID As Integer, aktTabelle As String, aktSchema As String, pufferinmeter As Double, Optional ByVal fromview As Boolean = True) As String
            Dim prefix As String = ".v_" : If Not fromview Then prefix = "."
            Try
                myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT ST_AsText(SetSRID(ST_Buffer(geom," & pufferinmeter.ToString.Replace(",", ".") &
                    ",'endcap=flat join=mitre')," & initP.getValue("GisServer.PostgisDBcoordinatensystem") & "))  FROM " & aktSchema & prefix & aktTabelle & " where gid=" & aktGID
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0))
            Catch ex As Exception
                nachricht("Fehler in holePUFFERPolygonFuerGID: ", ex)
                Return ""
            End Try
        End Function



        '31467

        Shared Function holePUFFERPolygonFuerPoint(myPoint As myPoint, pufferinMeter As Double) As String
            'endcap=flat join=mitre' muss weggelassen werden, sonst ist polygonempty
            myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECt ST_AsText(SetSRID(ST_Buffer(ST_MakePoint(" & myPoint.X &
                ", " & myPoint.Y &
                ") ," & pufferinMeter.ToString.Replace(",", ".") & ")," & initP.getValue("GisServer.PostgisDBcoordinatensystem") & "))"
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return (clsDBtools.fieldvalue(clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0))))
            Catch ex As Exception
                nachricht("Fehler in holePUFFERPolygonFuerPoint: ", ex)
                Return ""
            End Try
        End Function

        Shared Function holePUFFERPolygonFuerPolygon(mypoly As String, pufferinMeter As Double) As String
            'endcap=flat join=mitre' muss weggelassen werden, sonst ist polygonempty
            myGlobalz.sitzung.postgresREC.mydb.SQL =
                "SELECt ST_AsText(SetSRID(ST_Buffer(ST_GeomFromText('" & mypoly & "', " & initP.getValue("GisServer.PostgisDBcoordinatensystem") & ") ," &
                pufferinMeter.ToString.Replace(",", ".") & ")," & initP.getValue("GisServer.PostgisDBcoordinatensystem") & "))"
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return (clsDBtools.fieldvalue(clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0))))
            Catch ex As Exception
                nachricht("Fehler in holePUFFERPolygonFuerPoint: ", ex)
                Return ""
            End Try
        End Function

        Shared Function holeKoordinatenFuerUmkreis(aktPolygon As String) As String
            myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT ST_EXTENT(ST_GeomFromText('" & aktPolygon & "', " & initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")) "
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0))
            Catch ex As Exception
                nachricht("Fehler in holeKoordinatenFuerUmkreis: ", ex)
                Return ""
            End Try
        End Function

        Shared Function holeAreaFuerUmkreis(aktPolygon As String) As Double
            myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT ST_AREA(ST_GeomFromText('" & aktPolygon & "', " & initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")) "
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return CDbl(clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0)))
            Catch ex As Exception
                nachricht("Fehler in holeKoordinatenFuerUmkreis: ", ex)
                Return -1
            End Try
        End Function

        Public Shared Function ObjektAlsRaumbezugspeichern(aktPolygon As String, aktarea As Double, aktBOX As String, neuertitel As String) As Boolean
            myGlobalz.sitzung.aktPolygon.GKstring = aktPolygon
            myGlobalz.sitzung.aktPolygon.ShapeSerial = aktPolygon
            myGlobalz.sitzung.aktPolygon.Area = aktarea
            myGlobalz.sitzung.aktPolygon.FlaecheQm = aktarea
            myGlobalz.sitzung.aktPolygon.box.BBOX = aktBOX
            myGlobalz.sitzung.aktPolygon.box.bbox_split()
            myGlobalz.sitzung.aktPolygon.name = "GISObjekt"
            myGlobalz.sitzung.aktPolygon.Freitext = neuertitel
            Dim sekID% = 0 ' bei polygonen gibts keine zusatzinfos , keine tabelle 'Parapolygon' also auch kein sekid
            Dim anyChange As Boolean
            myGlobalz.sitzung.raumbezugsmodus = "neu"
            If myGlobalz.sitzung.aktPolygon.box.xl = myGlobalz.sitzung.aktPolygon.box.xh Then
                'punkte              
                myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Punkt
            Else
                'polygone
                myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polygon
            End If
            
            If myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Punkt Then
                myGlobalz.sitzung.aktPMU.Name = myGlobalz.sitzung.aktPolygon.name
                myGlobalz.sitzung.aktPMU.coordsAbstract = myGlobalz.sitzung.aktPolygon.abstract
                myGlobalz.sitzung.aktPMU.box = myGlobalz.sitzung.aktPolygon.box
                myGlobalz.sitzung.aktPMU.Freitext = myGlobalz.sitzung.aktPolygon.Freitext
                myGlobalz.sitzung.aktPMU.punkt.X = myGlobalz.sitzung.aktPolygon.box.xl
                myGlobalz.sitzung.aktPMU.punkt.Y = myGlobalz.sitzung.aktPolygon.box.Yl
                myGlobalz.sitzung.aktPMU.Radius = 500
                myGlobalz.sitzung.aktPMU.Typ = RaumbezugsTyp.Punkt
                ParaUmkreisTools.Umkreis_Neu()
            End If
            If myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polygon Then
                PolygonTools.PolygonNeuSpeichern(sekID)
            End If

            myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
            CLstart.myc.aLog.komponente = "Raumbezug"
            CLstart.myc.aLog.aktion = "Neues Polygon wurde gespeichert "
            CLstart.myc.aLog.log()
            anyChange = True
            Return anyChange
        End Function



        Shared Function holeFSTlistFuerPolygon(pgPolygon As String, pufferinmeter As Double) As DataTable
            myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT * " &
                              "FROM flurkarte.basis_f " &
                              "WHERE ST_Within(flurkarte.basis_f.geom, " &
                              "ST_GeomFromText('" &
                              pgPolygon &
                              "', " & initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")) "
            Try
                Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
                Return myGlobalz.sitzung.postgresREC.dt
            Catch ex As Exception
                nachricht("Fehler in holeKoordinatenFuerUmkreis: ", ex)
                Return Nothing
            End Try
        End Function

        'Private Shared Function BplanPDFanbieten(ByRef RESULT_dateien As List(Of gisresult), ByRef plannr As String) As Boolean
        '    If Not String.IsNullOrEmpty(clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("pdf")).ToString) Then
        '        l("PDF ist vorhanden")
        '        'prüfen ob zusatzDokus vorhanden sind
        '        Dim winpfad_start As String = ""
        '        Dim winpfad As String = ""
        '        Dim relativpfad As String = ""
        '        winpfad_start = GetWinpfad_start(0, winpfad, relativpfad)
        '        Dim newgis As New gisresult
        '        Dim fi As New IO.FileInfo(winpfad_start)
        '        newgis.datei = fi
        '        newgis.dateibeschreibung = "Bplan: " & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("pdf")).ToString.Trim
        '        RESULT_dateien.Add(newgis)
        '        plannr = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("pdf")).ToString.Trim
        '        Return True
        '    End If
        '    Return False
        'End Function

        'Private Shared Function getBplanInfoErmitteln(ByRef RESULT_dateien As List(Of gisresult), ByRef plannr As String, ByVal innerSQL As String,
        '                                              ByRef result_text As String) As Boolean
        '    l(innerSQL)
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT GEMARKUNG,nr,NR,PDF,titel,RECHTS,HOCH,gemeinde,baulnutz,aufstellun,rechtswirk,wird_ueber,bemerkung,flaeche_qm " &
        '            "  FROM planung.v_bebauungsplan_f " &
        '            "  WHERE ST_contains( planung.v_bebauungsplan_f.geom,(" & innerSQL & "  )" & "  );"
        '    l("sql: " & myGlobalz.sitzung.postgresREC.mydb.SQL)
        '    Dim hinweis = myGlobalz.sitzung.postgresREC.getDataDT()
        '    l("Anzahl=" & myGlobalz.sitzung.postgresREC.mycount)
        '    If myGlobalz.sitzung.postgresREC.mycount < 1 Then
        '        l("kein bplan")
        '        Return False
        '    Else
        '        l("pdf=" & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("pdf")).ToString)
        '        Dim lResult As Boolean = BplanPDFanbieten(RESULT_dateien, plannr)
        '        result_text = buildBplanresulttext(myGlobalz.sitzung.postgresREC.dt.Rows)
        '        Return lResult
        '    End If
        'End Function
        'Shared Function getBplanInfo4Adress(paraAdresse As ParaAdresse, ByRef RESULT_dateien As List(Of gisresult), ByRef plannr As String,
        '                                    ByRef result_text As String) As Boolean
        '    Dim innerSQL As String = String.Format("  SELECT geom  " &
        '                         "  FROM public.halofs where gemeindenr={0} AND strcode={1} AND Hausnr='{2}' and lower(zusatz)='{3}'",
        '                      paraAdresse.Gisadresse.gemeindeNrBig, paraAdresse.Gisadresse.strasseCode, paraAdresse.Gisadresse.hausNr,
        '                      paraAdresse.Gisadresse.hausZusatz.ToLower.Trim)
        '    Return getBplanInfoErmitteln(RESULT_dateien, plannr, innerSQL, result_text)
        'End Function

        'Shared Function GetWinpfad_start(ByVal i As Integer, ByRef winpfad$, ByRef relativpfad$) As String
        '    Dim winpfad_start$
        '    winpfad_start = "\\w2gis02\gdvell" & "\fkat\bplan"
        '    relativpfad = CStr(myglobalz.sitzung.postgresREC.dt.Rows(i).Item("gemarkung")).Trim & "\" &
        '        CStr(myglobalz.sitzung.postgresREC.dt.Rows(i).Item("pdf")).Trim & "\" &
        '               CStr(myglobalz.sitzung.postgresREC.dt.Rows(i).Item("pdf")).Trim
        '    l("winpfad_start$: " & winpfad_start$)
        '    l("relativpfad$: " & relativpfad)

        '    winpfad = (winpfad_start$ & relativpfad).ToLower & ".pdf"

        '    l("winpfad: " & winpfad)
        '    Return winpfad
        'End Function

        'Private Shared Sub fegerinfo(ByRef resulttext As String)
        '    Dim trenn As String = " # " & Environment.NewLine
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("AID")).ToString.Trim & trenn
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("Nachname")).ToString.Trim & " "
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("vorname")).ToString.Trim & trenn
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("strasse")).ToString.Trim & " "
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("hnr")).ToString.Trim & trenn
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("plz")).ToString.Trim & " "
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("ort")).ToString.Trim & trenn
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("tel")).ToString.Trim & trenn
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("mobil")).ToString.Trim & trenn
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("email")).ToString.Trim & trenn
        '    resulttext = resulttext & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("bemerkung")).ToString.Trim & trenn
        'End Sub

        'Shared Function getKehrbezirkInfo4Adress(paraAdresse As ParaAdresse, ByRef RESULT_text As String, ByRef bezirk As String) As Boolean
        '    Dim resulttext As String = ""
        '    Dim innerSQL As String = String.Format("SELECT geom  " &
        '                                     "  FROM public.halofs where gemeindenr={0} AND strcode={1} AND Hausnr='{2}' and lower(zusatz)='{3}'",
        '                                  paraAdresse.Gisadresse.gemeindeNrBig, paraAdresse.Gisadresse.strasseCode, paraAdresse.Gisadresse.hausNr,
        '                                  paraAdresse.Gisadresse.hausZusatz.ToLower.Trim)
        '    l(innerSQL)
        '    Return getKehrbezirkInfo4AdressExtracted(RESULT_text, bezirk, resulttext, innerSQL)
        'End Function

        'Private Shared Function getKehrbezirkInfo4AdressExtracted(ByRef RESULT_text As String, ByRef bezirk As String, ByVal resulttext As String, ByVal innerSQL As String) As Boolean
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "SELECT * " &
        '                        "  FROM klima.kehrbezirk_f " &
        '                        "  WHERE ST_contains( klima.kehrbezirk_f.geom,(" & innerSQL & "  )" & "  );"
        '    l("sql: " & myGlobalz.sitzung.postgresREC.mydb.SQL)
        '    Dim hinweis = myGlobalz.sitzung.postgresREC.getDataDT()
        '    l("Anzahl=" & myGlobalz.sitzung.postgresREC.mycount)
        '    If myGlobalz.sitzung.postgresREC.mycount < 1 Then
        '        l("kein bplan")
        '        Return False
        '    Else
        '        Dim aid As String = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("name")).ToString.Trim
        '        l("pdf=" & clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("name")).ToString)
        '        myglobalz.sitzung.postgresREC.mydb.SQL = "SELECT * FROM klima.kehrbezirk_a where name='" & aid & "'" '
        '        l("sql: " & myGlobalz.sitzung.postgresREC.mydb.SQL)
        '        hinweis = myGlobalz.sitzung.postgresREC.getDataDT()
        '        l("Anzahl=" & myGlobalz.sitzung.postgresREC.mycount)
        '        bezirk = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("AID")).ToString.Trim
        '        fegerinfo(resulttext)
        '        RESULT_text = resulttext
        '        Return True
        '    End If
        'End Function
        'Shared Function getFS4coordinates(x As Double, y As Double, ByRef fs As String) As Boolean
        '    Dim innerSQL As String = " SELECT ST_GeomFromText('POINT(" & x & " " & y & ")'," &
        '                                            initP.getValue("GisServer.PostgisDBcoordinatensystem") &
        '                                           ")"

        '    l(innerSQL)
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "  SELECT * " &
        '            "  FROM flurkarte.basis_f " &
        '            "  WHERE ST_contains( flurkarte.basis_f.geom,(" & innerSQL & "  )" & "  );"

        '    l("sql: " & myGlobalz.sitzung.postgresREC.mydb.SQL)
        '    Dim hinweis = myGlobalz.sitzung.postgresREC.getDataDT()
        '    l("Anzahl=" & myGlobalz.sitzung.postgresREC.mycount)

        '    If myGlobalz.sitzung.postgresREC.mycount < 1 Then
        '        Return False
        '    Else
        '        fs = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("fs")).ToString.Trim
        '        Return True
        '    End If
        'End Function

        'Shared Function getNSGInfo4Adress(paraAdresse As ParaAdresse, ByRef RESULT_dateien As List(Of gisresult), ByRef sgnr As String) As Boolean
        '    Dim innerSQL As String = String.Format("  SELECT geom  " &
        '                         "  FROM public.halofs where gemeindenr={0} AND strcode={1} AND Hausnr='{2}' and lower(zusatz)='{3}'",
        '                      paraAdresse.Gisadresse.gemeindeNrBig, paraAdresse.Gisadresse.strasseCode, paraAdresse.Gisadresse.hausNr,
        '                      paraAdresse.Gisadresse.hausZusatz.ToLower.Trim)
        '    l(innerSQL)
        '    Return getNSGInfo4AdressExtracted(RESULT_dateien, sgnr, innerSQL)
        'End Function

        'Private Shared Function getNSGInfo4AdressExtracted(ByRef RESULT_dateien As List(Of gisresult),
        '                                                    ByRef sgnr As String,
        '                                                    ByVal innerSQL As String) As Boolean
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "  SELECT * " &
        '                        "  FROM natur.v_natlanplusnd " &
        '                        "  WHERE ST_contains( natur.v_natlanplusnd.geom,(" & innerSQL & "  )" & "  );"

        '    l("sql: " & myGlobalz.sitzung.postgresREC.mydb.SQL)
        '    Dim hinweis = myGlobalz.sitzung.postgresREC.getDataDT()
        '    l("Anzahl=" & myGlobalz.sitzung.postgresREC.mycount)
        '    Dim newgis As New gisresult
        '    If myGlobalz.sitzung.postgresREC.mycount < 1 Then
        '        l("kein NSG/LSG/GLB")
        '        Return False
        '    Else
        '        RESULT_dateien.Clear()

        '        l(CStr(myGlobalz.sitzung.postgresREC.dt.Rows.Count))
        '        If myGlobalz.sitzung.postgresREC.dt.Rows.Count > 1 Then
        '            newgis.etikett = "!! mehrere Treffer !!"
        '            RESULT_dateien.Add(newgis)
        '        End If
        '        Dim verordnung As String = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("verordnung")).ToString.Trim
        '        l("pdf=" & verordnung)
        '        Dim art As String
        '        For i = 0 To myGlobalz.sitzung.postgresREC.dt.Rows.Count - 1
        '            verordnung = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item("verordnung")).ToString.Trim
        '            art = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item("art")).ToString.Trim
        '            l("pdf=" & verordnung)
        '            If Not String.IsNullOrEmpty(verordnung) Then
        '                l("PDF ist vorhanden")
        '                newgis = New gisresult
        '                'prüfen ob zusatzDokus vorhanden sind
        '                Dim winpfad_start As String = ""
        '                Dim winpfad As String = ""
        '                Dim relativpfad As String = ""
        '                If art = "2" Then 'naturdenkmal
        '                    winpfad_start = "\\w2gis02\gdvell\fkat\natur\natlandgeb\texte\naturdenkmal.pdf"
        '                    newgis.dateibeschreibung = "Naturdenkmal" & Environment.NewLine &
        '                        bildeNSGINFO(myGlobalz.sitzung.postgresREC.dt.Rows(i))
        '                Else '
        '                    winpfad_start = "\\w2gis02\gdvell\fkat\natur\natlandgeb\texte\" & verordnung & ".pdf"
        '                    newgis.dateibeschreibung = bildeNSGINFO(myGlobalz.sitzung.postgresREC.dt.Rows(i)).Replace("[ha]", "[qm]")
        '                End If

        '                Dim fi As New IO.FileInfo(winpfad_start)
        '                newgis.datei = fi
        '                newgis.etikett = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item("kurz")).ToString.Trim & ": " &
        '                                 clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item("titel")).ToString.Trim
        '                newgis.verordnung = verordnung

        '                RESULT_dateien.Add(newgis)
        '                sgnr = verordnung
        '            End If
        '        Next
        '        If myGlobalz.sitzung.postgresREC.dt.Rows.Count > 0 Then
        '            Return True
        '        Else
        '            Return False
        '        End If
        '    End If
        'End Function
        'Private Shared Function bildeNSGINFO(dataRow As DataRow) As String
        '    Try
        '        Dim summe As String = ""
        '        Dim trenn As String = ", " & Environment.NewLine
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("art").ToString.Trim & trenn)
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("titel").ToString.Trim & trenn)
        '        summe = summe & "ausg.: " & clsDBtools.fieldvalue(dataRow.Item("ausgewiesen").ToString.Trim & trenn)
        '        summe = summe & "Fläche [ha]: " & clsDBtools.fieldvalue(dataRow.Item("flaeche_qm").ToString.Trim & trenn)
        '        summe = summe & "veröff.: " & clsDBtools.fieldvalue(dataRow.Item("veroeffentlicht").ToString.Trim & trenn)
        '        summe = summe & "url.: " & clsDBtools.fieldvalue(dataRow.Item("url").ToString.Trim & trenn)
        '        Return summe
        '    Catch ex As Exception
        '        nachricht("fehler in bildeNSGINFO: ", ex)
        '        Return "keine info"
        '    End Try
        'End Function
        'Private Shared Function getUEGebietInfo4AdressExtracted(ByRef RESULT_dateien As List(Of gisresult), ByRef uenr As String, ByVal innerSQL As String) As Boolean
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "  SELECT * " &
        '                        "  FROM wasser.v_ueberschwemmungsgebiet_f " &
        '                        "  WHERE ST_contains(wasser.v_ueberschwemmungsgebiet_f.geom,(" & innerSQL & "  )" & "  );"

        '    l("sql: " & myGlobalz.sitzung.postgresREC.mydb.SQL)
        '    Dim hinweis = myGlobalz.sitzung.postgresREC.getDataDT()
        '    l("Anzahl=" & myGlobalz.sitzung.postgresREC.mycount)
        '    Try
        '        If myGlobalz.sitzung.postgresREC.mycount < 1 Then
        '            l("kein NSG/LSG/GLB")
        '            Return False
        '        Else
        '            RESULT_dateien.Clear()
        '            Dim verordnung As String = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(0).Item("verordnung")).ToString.Trim
        '            l("pdf=" & verordnung)
        '            If Not String.IsNullOrEmpty(verordnung) Then
        '                l("PDF ist vorhanden")
        '                'prüfen ob zusatzDokus vorhanden sind
        '                Dim winpfad_start As String = ""
        '                Dim winpfad As String = ""
        '                Dim relativpfad As String = ""
        '                winpfad_start = "\\w2gis02\gdvell\fkat\wasser\ueberschw\texte\" & verordnung & ".pdf"
        '                Dim newgis As New gisresult
        '                Dim fi As New IO.FileInfo(winpfad_start)
        '                newgis.datei = fi
        '                newgis.dateibeschreibung = bildeUEBINFO(myGlobalz.sitzung.postgresREC.dt.Rows(0))
        '                RESULT_dateien.Add(newgis)
        '                uenr = verordnung
        '                Return True
        '            End If
        '            Return False
        '        End If
        '    Catch ex As Exception
        '        nachricht("fehler in getUEGebietInfo4Adress: ", ex)
        '        Return False
        '    End Try
        'End Function
        'Shared Function getUEGebietInfo4Adress(paraAdresse As ParaAdresse, ByRef RESULT_dateien As List(Of gisresult), ByRef uenr As String) As Boolean
        '    Dim innerSQL As String = String.Format("  SELECT geom  " &
        '                         "  FROM public.halofs where gemeindenr={0} AND strcode={1} AND Hausnr='{2}' and lower(zusatz)='{3}'",
        '                      paraAdresse.Gisadresse.gemeindeNrBig, paraAdresse.Gisadresse.strasseCode, paraAdresse.Gisadresse.hausNr,
        '                      paraAdresse.Gisadresse.hausZusatz.ToLower.Trim)
        '    l(innerSQL)
        '    Return getUEGebietInfo4AdressExtracted(RESULT_dateien, uenr, innerSQL)
        'End Function

        'Private Shared Function bildeUEBINFO(dataRow As DataRow) As String
        '    Try
        '        Dim summe As String = ""
        '        Dim trenn As String = ", " & Environment.NewLine
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("titel").ToString.Trim & trenn)
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("name2").ToString.Trim & trenn)
        '        summe = summe & "ausg.: " & clsDBtools.fieldvalue(dataRow.Item("ausgewiesen").ToString.Trim & trenn)
        '        summe = summe & "fläche [ha]: " & clsDBtools.fieldvalue(dataRow.Item("flaeche_ha").ToString.Trim & trenn)
        '        summe = summe & "veröff.: " & clsDBtools.fieldvalue(dataRow.Item("veroeffentlicht").ToString.Trim & trenn)
        '        summe = summe & "url.: " & clsDBtools.fieldvalue(dataRow.Item("link").ToString.Trim & trenn)
        '        Return summe
        '    Catch ex As Exception
        '        nachricht("fehler in bildeNSGINFO: ", ex)
        '        Return "keine info"
        '    End Try
        'End Function

        'Private Shared Function getWSGebietInfo4AdressExtracted(ByVal RESULT_dateien As List(Of gisresult),
        '                                                        ByVal WSGnr As String,
        '                                                        ByVal innerSQL As String) As Boolean
        '    myGlobalz.sitzung.postgresREC.mydb.SQL = "  SELECT * " &
        '                        "  FROM wasser.v_wasserschutzgebiet_f " &
        '                        "  WHERE ST_contains(wasser.v_wasserschutzgebiet_f.geom,(" & innerSQL & "  )" & "  );"

        '    l("sql: " & myGlobalz.sitzung.postgresREC.mydb.SQL)
        '    Dim hinweis = myGlobalz.sitzung.postgresREC.getDataDT()
        '    l("Anzahl=" & myGlobalz.sitzung.postgresREC.mycount)
        '    Try
        '        If myGlobalz.sitzung.postgresREC.mycount < 1 Then
        '            l("kein NSG/LSG/GLB")
        '            Return False
        '        Else
        '            RESULT_dateien.Clear()
        '            Dim newgis As New gisresult
        '            Dim verordnung As String
        '            l(CStr(myGlobalz.sitzung.postgresREC.dt.Rows.Count))
        '            If myGlobalz.sitzung.postgresREC.dt.Rows.Count > 1 Then
        '                newgis.etikett = "!! mehrere Treffer !!"
        '                RESULT_dateien.Add(newgis)
        '            End If
        '            For i = 0 To myGlobalz.sitzung.postgresREC.dt.Rows.Count - 1
        '                verordnung = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item("link")).ToString.Trim
        '                l("pdf=" & verordnung)
        '                If Not String.IsNullOrEmpty(verordnung) Then
        '                    l("PDF ist vorhanden")
        '                    'prüfen ob zusatzDokus vorhanden sind
        '                    Dim winpfad_start As String = ""
        '                    Dim winpfad As String = ""
        '                    Dim relativpfad As String = ""
        '                    winpfad_start = ("\\w2gis02\gdvell\" & verordnung).Replace("/", "\")
        '                    newgis = New gisresult
        '                    Dim fi As New IO.FileInfo(winpfad_start)
        '                    newgis.etikett = clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item("schutzzone")).ToString.Trim & ": " &
        '                        clsDBtools.fieldvalue(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item("name")).ToString.Trim
        '                    newgis.verordnung = verordnung
        '                    newgis.datei = fi
        '                    newgis.dateibeschreibung = bildewsgINFO(myGlobalz.sitzung.postgresREC.dt.Rows(i))
        '                    RESULT_dateien.Add(newgis)
        '                    WSGnr = verordnung
        '                    'Return True
        '                End If
        '            Next
        '            If myGlobalz.sitzung.postgresREC.dt.Rows.Count > 0 Then
        '                Return True
        '            Else
        '                Return False
        '            End If

        '        End If
        '    Catch ex As Exception
        '        nachricht("fehler in getWSGebietInfo4Adress: ", ex)
        '        Return False
        '    End Try
        'End Function
        'Shared Function getWSGebietInfo4Adress(paraAdresse As ParaAdresse, RESULT_dateien As List(Of gisresult), WSGnr As String) As Boolean
        '    Dim innerSQL As String = String.Format("  SELECT geom  " &
        '                                   "  FROM public.halofs where gemeindenr={0} AND strcode={1} AND Hausnr='{2}' and lower(zusatz)='{3}'",
        '                                paraAdresse.Gisadresse.gemeindeNrBig, paraAdresse.Gisadresse.strasseCode, paraAdresse.Gisadresse.hausNr,
        '                                paraAdresse.Gisadresse.hausZusatz.ToLower.Trim)
        '    l(innerSQL)
        '    Return getWSGebietInfo4AdressExtracted(RESULT_dateien, WSGnr, innerSQL)
        'End Function

        'Private Shared Function bildewsgINFO(dataRow As DataRow) As String
        '    Try
        '        Dim summe As String = ""
        '        Dim TRENN As String = ", " & Environment.NewLine
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("ART").ToString.Trim & TRENN)
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("NAME").ToString.Trim & TRENN)
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("SCHUTZZONE").ToString.Trim & TRENN)
        '        summe = summe & "BETREIBER.: " & clsDBtools.fieldvalue(dataRow.Item("BETREIBER").ToString.Trim & TRENN)
        '        summe = summe & clsDBtools.fieldvalue(dataRow.Item("HLUG").ToString.Trim & TRENN)
        '        summe = summe & "AUSG.: " & clsDBtools.fieldvalue(dataRow.Item("AUSGEWIESEN").ToString.Trim & TRENN)
        '        'SUMME = SUMME & "FLÄCHE [HA]: " & CLSDBTOOLS.FIELDVALUE(DATAROW.ITEM("FLAECHE_HA").TOSTRING.TRIM & TRENN)
        '        summe = summe & "VERÖFF.: " & clsDBtools.fieldvalue(dataRow.Item("VEROEFF_GEAENDERT").ToString.Trim & TRENN)
        '        summe = summe & "URL.: " & clsDBtools.fieldvalue(dataRow.Item("LINK").ToString.Trim & TRENN)
        '        Return summe
        '    Catch ex As Exception
        '        nachricht("fehler in bildewSGINFO: ", ex)
        '        Return "keine info"
        '    End Try
        'End Function

        'Shared Function getBplanInfo4point(clsParaUmkreis As clsGEOPoint, RESULT_dateien As List(Of gisresult), ByRef Plannr As String, ByRef result_text As String) As Boolean
        '    Dim innerSQL As String
        '    innerSQL = "select SetSRID(ST_MakePoint(" & clsParaUmkreis.X & "," & clsParaUmkreis.Y & ")," &
        '        initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")"
        '    Return getBplanInfoErmitteln(RESULT_dateien, Plannr, innerSQL, result_text)
        'End Function

        'Shared Function getKehrbezirkInfo4point(clsParaUmkreis As clsGEOPoint, ByRef RESULT_text_kehr As String, ByRef bezirk As String) As Boolean
        '    Dim resulttext As String = ""
        '    Dim innerSQL As String = "select SetSRID(ST_MakePoint(" & clsParaUmkreis.X & "," & clsParaUmkreis.Y & ")," &
        '          initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")"
        '    l(innerSQL)
        '    Return getKehrbezirkInfo4AdressExtracted(RESULT_text_kehr, bezirk, resulttext, innerSQL)
        'End Function

        'Shared Function getNSGInfo4Point(clsParaUmkreis As clsGEOPoint, RESULT_dateien As List(Of gisresult), ByRef sgnr As String) As Boolean
        '    Dim innerSQL As String = "select SetSRID(ST_MakePoint(" & clsParaUmkreis.X & "," & clsParaUmkreis.Y & ")," &
        '                    initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")"
        '    l(innerSQL)
        '    Return getNSGInfo4AdressExtracted(RESULT_dateien, sgnr, innerSQL)
        'End Function

        'Shared Function getUEGebietInfo4Point(clsParaUmkreis As clsGEOPoint, RESULT_dateien As List(Of gisresult), ByRef uenr As String) As Boolean
        '    Dim innerSQL As String = "select SetSRID(ST_MakePoint(" & clsParaUmkreis.X & "," & clsParaUmkreis.Y & ")," &
        '                              initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")"
        '    l(innerSQL)
        '    Return getUEGebietInfo4AdressExtracted(RESULT_dateien, uenr, innerSQL)
        'End Function

        'Shared Function getWSGebietInfo4Point(clsParaUmkreis As clsGEOPoint, RESULT_dateien As List(Of gisresult), ByRef WSGnr As String) As Boolean
        '    Dim innerSQL As String = "select SetSRID(ST_MakePoint(" & clsParaUmkreis.X & "," & clsParaUmkreis.Y & ")," &
        '                             initP.getValue("GisServer.PostgisDBcoordinatensystem") & ")"
        '    l(innerSQL)
        '    Return getWSGebietInfo4AdressExtracted(RESULT_dateien, WSGnr, innerSQL)
        'End Function

        'Private Shared Function buildBplanresulttext(drc As DataRowCollection) As String
        '    Dim sb As New Text.StringBuilder
        '    Dim trenn As String = " " & Environment.NewLine
        '    sb.Append("Bebauungsplan Nr: " & clsDBtools.fieldvalue(drc.Item(0).Item("nr")) & trenn)
        '    sb.Append("-------------------" & trenn)
        '    sb.Append(clsDBtools.fieldvalue(drc.Item(0).Item("titel")) & trenn)
        '    sb.Append(" Gemeinde: " & clsDBtools.fieldvalue(drc.Item(0).Item("gemeinde")) & trenn)
        '    sb.Append(" Gemarkung: " & clsDBtools.fieldvalue(drc.Item(0).Item("gemarkung")) & trenn)
        '    sb.Append(" Baul.Nutz.: " & clsDBtools.fieldvalue(drc.Item(0).Item("baulnutz")) & trenn)
        '    sb.Append(" Aufstellung: " & clsDBtools.fieldvalue(drc.Item(0).Item("aufstellun")) & trenn)
        '    sb.Append(" Rechtswirksam: " & clsDBtools.fieldvalue(drc.Item(0).Item("rechtswirk")) & trenn)
        '    sb.Append(clsDBtools.fieldvalue(drc.Item(0).Item("bemerkung")) & trenn)
        '    If clsDBtools.fieldvalue(drc.Item(0).Item("wird_ueber")).ToString.Trim.Length > 0 Then
        '        sb.Append(" Achtung B-Plan wird überlagert von B-Plan" & clsDBtools.fieldvalue(drc.Item(0).Item("wird_ueber")) & trenn)
        '    End If
        '    sb.Append(" Fläche[qm]: " & clsDBtools.fieldvalue(drc.Item(0).Item("flaeche_qm")) & trenn)
        '    Return sb.ToString
        'End Function

    End Class

End Namespace