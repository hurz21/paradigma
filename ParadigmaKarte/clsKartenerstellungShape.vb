Imports System.Data

Public Class clsKartenerstellungShape
    Public Property KartenRoot As String
    Public Property Kartenprojektdir As String
    Public Shared Property kartenDatadir As String
    Public Property KartenMAPfile As String
    Public Property Kartenshapefile As String
    Public Property KartenVorgangsDir As String
    Public Property KartenJPGDir As String
    Public Property KartenIMGDir As String
    Public Property KartenMapfileTemplate As String
    Public Property KartenHTMfileTemplate As String
    Public Property appendix As String
    Public Property aktBox As New clsRange


    'Function exekuteEinzelVorgang(ByVal vid%, ByVal sgnummer$, ByVal Mitverwandten As Boolean, ByVal modus$, ByVal ebenen%()) As String

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="aktBox"></param>
    ''' <param name="ebenen"></param>
    ''' <returns>fehlercode. sollte string.empty sein falls alle ok</returns>
    ''' <remarks></remarks>
    Public Function point_shpfile_erzeugen(ByRef aktBox As clsRange, ByVal ebenen() As Integer) As String
        Dim f As New MapWinGIS.Field 'installation von MapWinGIS47SRa-x86-Setup.exe beseitigt die fehlermeldung auf x64
        Dim shape As New MapWinGIS.Shape
        Dim ishp% = 0
        Dim pt As New MapWinGIS.Point()
        Dim Result As Boolean
        Dim hinweis$
        Dim sf As New MapWinGIS.Shapefile

        Try
            glob2.nachricht("point_shpfile_erzeugen ------------------------------------------")
            Kartenshapefile = kartenDatadir$ & "\rbpoint"   ' & vorgangsid%
            If System.IO.File.Exists(Kartenshapefile & ".dbf") Then alteVersionLoeschen(Kartenshapefile)
            glob2.nachricht("Kartenshapefile: " & Kartenshapefile)

            glob2.nachricht("vor CreateNew")
            Result = sf.CreateNew(Kartenshapefile & ".shp", MapWinGIS.ShpfileType.SHP_POINT)
            If Result = False Then
                Return SHP_Fehlerbehandlung(sf, pt, shape, f)
            End If
            glob2.nachricht("Start Editing it...")
            Result = sf.StartEditingShapes(True)
            If Result = False Then
                Return SHP_Fehlerbehandlung(sf, pt, shape, f)
            End If

            glob2.nachricht("vor SHPFile_Spaltenanlegen")
            If Not SHPFile_Spaltenanlegen(f, pt, shape, Result, sf) Then
                Return "fehler beim spaltenanlegen"
            End If

            glob2.nachricht("alle ebenen durchlaufen um kartenobjekte zu generieren: " & ebenen.Count)
            For I = 0 To ebenen.Count - 1
                hinweis$ = genKartenObjektePoint(ebenen(I), aktBox, sf, ishp)
                glob2.nachricht(String.Format("Ebene-Nr: {0}: {1}", I, hinweis))
            Next

            glob2.nachricht("vor stopediting")
            Result = sf.StopEditingShapes(True, True)
            glob2.nachricht("point_shpfile_erzeugen ################ ende ##########")
            Return String.Empty
        Catch ex As Exception
            glob2.nachricht("Fehler in point_shpfile_erzeugen: " & ex.ToString)
        End Try
    End Function



    Private Sub dokumenteauschecken(ByVal aktvid%)
        'glob2.nachricht("dokumenteauschecken -----------------------------------------------")
        'glob2.initDokumente4VorgangDatatable(aktvid)
        'glob2.DataTable_auschecken(KartenJPGDir$, aktvid%)
    End Sub
    'für jede ebene:
    '- prüfen ob raumbezüge
    '- bbox	 festlegen und merken
    '- dokus auschecken
    '- shapefile erstellen (appenden)

    ' unabhängig von der ebene
    '- mapfile erstellen
    '- htmfile erstellen
    '- ebene in webgiscontol anlegen
    '- gisaufrufen
    Function genKartenObjektePoint(ByVal vorgangsid%, ByRef aktBox As clsRange, ByVal sf As MapWinGIS.Shapefile, ByRef ishp%) As String
        'boundingbox fixen
        Dim hinweis$ = ""
        Dim rbok As Boolean
        Try
            rbok = holeRaumbezuge(vorgangsid, hinweis)
            glob2.nachricht("genKartenObjektePoint -----------------------------------------------")
            If Not rbok Then Return hinweis

            Dim lokbbox As New clsRange
            lokbbox = getbboxOfDt(myGlobalz.raumbezugsRec.dt)
            aktBox.expand(lokbbox)

            If vorgangsid = 1399 Then
                Debug.Print("")
            End If
            ' dokumenteauschecken(vorgangsid%)

            glob2.nachricht("vor Raumbezug2shp")
            hinweis = ""
            Raumbezug2Pointshp(vorgangsid%, myGlobalz.raumbezugsRec.dt, KartenJPGDir$.Replace(myGlobalz.gis_serverD, ""), hinweis, sf, ishp)
            If Not String.IsNullOrEmpty(hinweis) Then Return hinweis

            glob2.nachricht("nach Raumbezug2Pointshp")
            glob2.nachricht("genKartenObjektePoint ############### ende ")
            Return hinweis
        Catch ex As Exception
            glob2.nachricht("fehler in genKartenObjektePoint: ")
        End Try
    End Function

    Function genKartenObjektePolygon(ByVal vorgangsid As Integer,
                                        ByRef aktBox As clsRange,
                                        ByVal sf As MapWinGIS.Shapefile,
                                        ByRef ishp As Integer,
                                        paradigmaRBtyp As RaumbezugsTyp) As String
        'boundingbox fixen
        Dim hinweis$ = ""
        Dim hatRB As Boolean = holeRaumbezuge(vorgangsid, hinweis)
        glob2.nachricht("genKartenObjektePolygon -----------------------------------------------")
        If Not hatRB Then Return hinweis
        Dim lokbbox As New clsRange
        lokbbox = getbboxOfDt(myGlobalz.raumbezugsRec.dt)
        aktBox.expand(lokbbox)
        ' dokumenteauschecken()

        glob2.nachricht("vor Raumbezug2shp")
        hinweis = ""
        'hier muss eine neue routine hin die davon abhängt wo die serialisierten shps liegen sind
        Raumbezug2Polygonshp(vorgangsid%, myGlobalz.raumbezugsRec.dt, KartenJPGDir$.Replace(myGlobalz.gis_serverD, ""), hinweis, sf, ishp,
                                            paradigmaRBtyp)
        If Not String.IsNullOrEmpty(hinweis) Then Return hinweis

        glob2.nachricht("genKartenObjektePolygon ############### ende ")
        Return hinweis
    End Function


    Private Shared Function holeRaumbezuge(ByRef vorgangsid%, ByRef hinweis$) As Boolean
        glob2.nachricht("raumbezugePruefen -----------------------------------------------")
        myGlobalz.VorgangsID = vorgangsid
        Dim erfolg As Boolean

        DB_fork.forkiniraumbezugsDT(erfolg)

        If Not erfolg Then
            hinweis = "Zu diesem Vorgang existieren keine Raumbezüge: vorgangsid: " & vorgangsid
            glob2.nachricht("raumbezugePruefen " & hinweis)
            Return False
        End If
        glob2.nachricht("raumbezugePruefen erfolgreich")
        Return True
    End Function

    Shared Function getbboxOfDt(ByVal dt As DataTable) As clsRange
        glob2.nachricht("getbboxOfDt -----------------------------------------------")
        Try
            Dim xl As Double = 1000000000
            Dim xh As Double = 0
            Dim yl As Double = 1000000000
            Dim yh As Double = 0
            For Each rau As DataRow In dt.AsEnumerable
                If CDbl(rau.Item("rechts")) > 10 Then
                    If CDbl(rau.Item("rechts")) < xl Then xl = CDbl(rau.Item("rechts"))
                    If CDbl(rau.Item("rechts")) > xh Then xh = CDbl(rau.Item("rechts"))
                End If
                If CDbl(rau.Item("hoch")) > 10 Then
                    If CDbl(rau.Item("hoch")) < yl Then yl = CDbl(rau.Item("hoch"))
                    If CDbl(rau.Item("hoch")) > yh Then yh = CDbl(rau.Item("hoch"))
                End If
            Next
            Dim box As New clsRange() With {.xl = xl, .xh = xh, .yl = yl, .yh = yh}
            glob2.nachricht("getbboxOfDt -----------------------------------------------" & box.toString)
            Return box
        Catch ex As Exception
            glob2.nachricht("Fehler in getbboxOfDt" & ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Shared Sub makeHtmFile(ByVal kartenHTMfileTemplate As String,
                                   ByVal zielHTMfile As String)
        glob2.nachricht("makeHtmFile -----------------------------------------------")
        Dim test As New IO.FileInfo(kartenHTMfileTemplate)
        If test.Exists Then
            glob2.nachricht("htm vorlage existiert")
            Dim test2 As New IO.FileInfo(zielHTMfile)
            If test2.Exists Then
                test2.Delete()
            End If
            test.CopyTo(zielHTMfile)
        Else
            glob2.nachricht(String.Format("{0}: makeHtmFile. FEHLER: htm vorlage existiert NICHT vorlage: {1}", zielHTMfile, kartenHTMfileTemplate))
        End If
    End Sub





    Public Function webgisPreparieren(ByVal mitetikett As Boolean,GISusername As string) As String
        glob2.nachricht("webgisPreparieren------------------------------------")
        'ebene in webgiscontrol anlegen
        Dim KartenEbenenName$ = appendix
        glob2.nachricht("vor makeMapFile")
        LibGISmapgenerator.tools.makeMapFile(KartenMapfileTemplate, KartenMAPfile, KartenEbenenName, mitetikett, myGlobalz.enc,GISusername)
        glob2.nachricht("nach makeMapFile")

        Dim ZielHTMfile$ = KartenMAPfile.Replace(".map", ".htm").Replace("d:", myGlobalz.gis_serverD)
        makeHtmFile(KartenHTMfileTemplate, ZielHTMfile)
        glob2.nachricht("nach makeHtmFile")

        Dim directory$ = KartenMAPfile.Replace(myGlobalz.gis_serverD$, "d:")
        Dim dbpfad$ = kartenDatadir$.Replace(myGlobalz.gis_serverD$, "")

        DB_fork.insertFeatureClassIntoWebgiscontrolDB_alledb(KartenEbenenName, appendix$, directory, dbpfad, myGlobalz.haloREC)

        glob2.nachricht("webgisPreparieren ################ endee #")
        Return KartenEbenenName
    End Function


    Private Shared Sub Raumbezug2Pointshp(ByVal vid%,
                                            ByVal rbTable As DataTable,
                                            ByVal KartenJPGDir$,
                                            ByRef shapeFehler$,
                                            ByVal sf As MapWinGIS.Shapefile,
                                            ByRef ishp%)
        glob2.nachricht("Raumbezug2Pointshp------------------------------------")
        Dim pt As New MapWinGIS.Point
        Dim shape As New MapWinGIS.Shape
        Dim irec As Long
        Try
            Dim dateiname$ = "", dateititel$ = "", abstract$ = ""
            Dim suchkette$ = " / "
            Dim ipos%, typ%, rbid%
            Dim f As New MapWinGIS.Field
            Dim result As Boolean
            glob2.nachricht("vor Raumbezug2shp3")
            For irec = 0 + ishp To rbTable.Rows.Count - 1 + ishp
                'initialize the shape
                shape = New MapWinGIS.Shape
                shape.ShapeType = MapWinGIS.ShpfileType.SHP_POINT
                'Set up the points
                glob2.nachricht("vor Raumbezug2shp6")
                pt = New MapWinGIS.Point
                pt.x = CDbl(rbTable.Rows(CInt(irec)).Item("rechts"))
                pt.y = CDbl(rbTable.Rows(CInt(irec)).Item("hoch"))
                dateititel$ = CStr(clsDBtools.fieldvalue(rbTable.Rows(CInt(irec)).Item("titel")))
                abstract$ = CStr(rbTable.Rows(CInt(irec)).Item("abstract"))
                typ = CInt(rbTable.Rows(CInt(irec)).Item("typ"))
                rbid = CInt(rbTable.Rows(CInt(irec)).Item("raumbezugsid"))

                ipos% = InStr(abstract$, suchkette$, CompareMethod.Text)
                dateiname$ = abstract$.Substring(0, ipos + suchkette$.Length - 3).Trim
                dateiname = String.Format("{0}\{1}", KartenJPGDir$, dateiname)
                dateiname = dateiname.Replace("\", "/")

                If pt.x < 1 Or pt.y < 1 Then
                    glob2.nachricht("Koordinate kleiner 1! Wird ignoriert!")
                    Continue For
                End If
                ishp += 1
                'Add the points to a shape
                result = shape.InsertPoint(pt, ishp)
                If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Exit Sub

                glob2.nachricht("vor Raumbezug2shp7")
                'Add the shape to the shapefile
                result = sf.EditInsertShape(shape, ishp)
                If result = False Then
                    shapeFehler$ = SHP_Fehlerbehandlung(sf, pt, shape, f)
                    Exit Sub
                End If
                glob2.nachricht("vor Raumbezug2shp8")
                Spaltenwerte_Setzen(result, sf, ishp, irec, rbTable, pt, shape, f, shapeFehler, dateiname, dateititel, vid, rbid, typ)
            Next irec
            glob2.nachricht("Raumbezug2Pointshp ################# ende ok ########")
        Catch ex As Exception
            glob2.nachricht_und_Mbox(String.Format("FEHLER im Kartenmodul(Raumbezug2Pointshp): {0}{1}", vbCrLf, ex))
        End Try
    End Sub

    Shared Function Spaltenwerte_Setzen(ByRef result As Boolean,
                                 ByRef sf As MapWinGIS.Shapefile,
                                 ByRef ishp%,
                                 ByRef irec As Long,
                                 ByRef rbTable As DataTable, ByVal pt As MapWinGIS.Point,
                                 ByRef shape As MapWinGIS.Shape,
                                 ByRef f As MapWinGIS.Field,
                                 ByVal shapeFehler$,
                                 ByVal dateiname$,
                                 ByVal dateititel$,
                                 ByVal vid%,
                                 ByVal rbid%,
                                 ByVal typ%) As Boolean
        'Spaltenwerte_setzen
        'Edit the attributes for this shape record
        result = sf.EditCellValue(0, ishp,
                                  String.Format("{0} / {1}{2}",
                                                         CStr(rbTable.Rows(CInt(irec)).Item("abstract")),
                                                         CStr(clsDBtools.fieldvalue(rbTable.Rows(CInt(irec)).Item("titel"))), irec)) 'insert some string value
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False

        result = sf.EditCellValue(1, ishp, irec)
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False

        result = sf.EditCellValue(2, ishp, CDbl(pt.x * pt.y))
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False

        'Edateiname
        result = sf.EditCellValue(3, ishp, dateiname)
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False

        'Edateititel
        result = sf.EditCellValue(4, ishp, dateititel)
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False

        'typ
        result = sf.EditCellValue(5, ishp, typ)
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False

        'vorgang
        result = sf.EditCellValue(6, ishp, vid%)    ' 
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False

        'rbid
        result = sf.EditCellValue(7, ishp, rbid)    ' 
        If Not ShapeOperationGelungen(sf, pt, shape, f, result, shapeFehler) Then Return False
        Return True
    End Function

    Private Shared Function ShapeOperationGelungen(ByVal sf As MapWinGIS.Shapefile, _
                                            ByVal pt As MapWinGIS.Point, _
                                            ByVal shape As MapWinGIS.Shape, _
                                            ByVal f As MapWinGIS.Field, _
                                            ByVal result As Boolean,
                                            ByRef shapeFehler$) As Boolean
        If result = False Then
            shapeFehler$ = SHP_Fehlerbehandlung(sf, pt, shape, f)
            glob2.nachricht("shapeFehler$")
            Return False
        Else
            Return True
        End If
    End Function

    Private Shared Sub alteVersionLoeschen(ByVal newshapefile$)
        glob2.nachricht("alteVersionLoeschen --------------------------")
        Try
            System.IO.File.Delete(newshapefile$ & ".shp")
            System.IO.File.Delete(newshapefile$ & ".dbf")
            System.IO.File.Delete(newshapefile$ & ".shx")
            System.IO.File.Delete(newshapefile$ & ".qix")
            glob2.nachricht("alteVersionLoeschen ende")
        Catch ex As Exception
            glob2.nachricht("Fehler in alteVersionLoeschen")
        End Try
    End Sub

    Private Shared Function SHPFile_Spaltenanlegen(ByRef f As MapWinGIS.Field, ByRef pt As MapWinGIS.Point, ByRef shape As MapWinGIS.Shape, ByRef Result As Boolean, ByRef sf As MapWinGIS.Shapefile) As Boolean
        Dim Isp% = 0
        glob2.nachricht("SHPFile_Spaltenanlegen --------------------------")
        Try
            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.STRING_FIELD
            f.Name = "MYTEXT"
            f.Width = 250
            Result = sf.EditInsertField(f, Isp%)
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1

            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.INTEGER_FIELD
            f.Name = "SHAPEID"
            Result = sf.EditInsertField(f, Isp%)
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1

            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.DOUBLE_FIELD
            f.Name = "MYDOUBLE"
            f.Precision = 8
            Result = sf.EditInsertField(f, Isp%)
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1


            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.STRING_FIELD
            f.Name = "RBDATEI"
            f.Width = 250
            Result = sf.EditInsertField(f, Isp%)
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1

            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.STRING_FIELD
            f.Name = "RBTITEL"
            f.Width = 250
            Result = sf.EditInsertField(f, Isp%)    '4
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1

            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.INTEGER_FIELD
            f.Name = "RBTYP"
            Result = sf.EditInsertField(f, Isp%)
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1

            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.INTEGER_FIELD
            f.Name = "VORGANG"
            Result = sf.EditInsertField(f, Isp%)
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1

            f = New MapWinGIS.Field
            f.Type = MapWinGIS.FieldType.INTEGER_FIELD
            f.Name = "RBID"
            Result = sf.EditInsertField(f, Isp%)
            If Result = False Then
                SHP_Fehlerbehandlung(sf, pt, shape, f)
                Return False
            End If
            Isp% += 1

            glob2.nachricht("SHPFile_Spaltenanlegen ################## ende ")
        Catch ex As Exception
            glob2.nachricht_und_Mbox(String.Format("Fehler beim Anlegen der Spalte: {0}{1}{2}", Isp, vbCrLf, ex))
        End Try
        Return True
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sf"></param>
    ''' <param name="pt"></param>
    ''' <param name="shape"></param>
    ''' <param name="f"></param>
    Shared Function SHP_Fehlerbehandlung(ByVal sf As MapWinGIS.Shapefile, ByVal pt As MapWinGIS.Point, ByVal shape As MapWinGIS.Shape, ByVal f As MapWinGIS.Field) As String
        Dim lNewVariable As String = String.Format("Fehler: Shapefile Error: {0}{1}Shape Error: {2}{1}Point Error: {3}{1}Field Error: {4}", sf.ErrorMsg(sf.LastErrorCode), vbCrLf, shape.ErrorMsg(shape.LastErrorCode), pt.ErrorMsg(pt.LastErrorCode), f.ErrorMsg(f.LastErrorCode))
        glob2.nachricht(lNewVariable)
        Return lNewVariable
    End Function



    Public Function polygon_shpfile_erzeugen(ByRef aktBox As clsRange, ByVal ebenen() As Integer, paradigmaRBtyp As RaumbezugsTyp) As String
        Dim f As New MapWinGIS.Field
        Dim shape As New MapWinGIS.Shape
        Dim ishp% = 0
        Dim pt As New MapWinGIS.Point()
        Dim Result As Boolean
        Dim hinweis$
        Dim sf As New MapWinGIS.Shapefile

        Try
            glob2.nachricht("point_shpfile_erzeugen ------------------------------------------")
            Kartenshapefile = kartenDatadir$ & "\rbpoly"   ' & vorgangsid%
            If System.IO.File.Exists(Kartenshapefile & ".dbf") Then alteVersionLoeschen(Kartenshapefile)
            glob2.nachricht("Kartenshapefile: " & Kartenshapefile)

            glob2.nachricht("vor CreateNew")
            Result = sf.CreateNew(Kartenshapefile & ".shp", MapWinGIS.ShpfileType.SHP_POLYGON)
            If Result = False Then
                Return SHP_Fehlerbehandlung(sf, pt, shape, f)
            End If
            glob2.nachricht("Start Editing it...")
            Result = sf.StartEditingShapes(True)
            If Result = False Then
                Return SHP_Fehlerbehandlung(sf, pt, shape, f)
            End If

            glob2.nachricht("vor SHPFile_Spaltenanlegen")
            If Not SHPFile_Spaltenanlegen(f, pt, shape, Result, sf) Then
                Return "fehler beim spaltenanlegen"
            End If

            glob2.nachricht("alle ebenen durchlaufen um kartenobjekte zu generieren: " & ebenen.Count)
            For I = 0 To ebenen.Count - 1
                glob2.nachricht(String.Format("vor {0}, Ebene-Nr: {1}----------------------", ebenen(I), I))
                hinweis$ = genKartenObjektePolygon(ebenen(I), aktBox, sf, ishp, paradigmaRBtyp)
                glob2.nachricht(String.Format("nach {0}, Ebene-Nr: {1}: {2}", ebenen(I), I, hinweis))
            Next

            ' glob2.nachricht(genKartenObjektePolygon(ebenen(I), aktBox, sf, ishp)'''''''''''''''''''''war glob2.nachrichtgenKa also deaktiviert
            Result = sf.StopEditingShapes(True, True)
            glob2.nachricht("polygon_shpfile_erzeugen ################ ende ##########")
            Return String.Empty
        Catch ex As Exception
            glob2.nachricht("Fehler in polygon_shpfile_erzeugen: " & ex.ToString)
            Return "Fehler in polygon_shpfile_erzeugen: " & ex.ToString
        End Try
    End Function
    Public Function polyline_shpfile_erzeugen(ByRef aktBox As clsRange, ByVal ebenen() As Integer, paradigmaRBtyp As RaumbezugsTyp) As String
        Dim f As New MapWinGIS.Field
        Dim shape As New MapWinGIS.Shape
        Dim ishp% = 0
        Dim pt As New MapWinGIS.Point()
        Dim Result As Boolean
        Dim hinweis$
        Dim sf As New MapWinGIS.Shapefile

        Try
            glob2.nachricht("point_shpfile_erzeugen ------------------------------------------")
            Kartenshapefile = kartenDatadir$ & "\rbpline"   ' & vorgangsid%
            If System.IO.File.Exists(Kartenshapefile & ".dbf") Then alteVersionLoeschen(Kartenshapefile)
            glob2.nachricht("Kartenshapefile: " & Kartenshapefile)

            glob2.nachricht("vor CreateNew")
            Result = sf.CreateNew(Kartenshapefile & ".shp", MapWinGIS.ShpfileType.SHP_POLYLINE)
            If Result = False Then
                Return SHP_Fehlerbehandlung(sf, pt, shape, f)
            End If
            glob2.nachricht("Start Editing it...")
            Result = sf.StartEditingShapes(True)
            If Result = False Then
                Return SHP_Fehlerbehandlung(sf, pt, shape, f)
            End If

            glob2.nachricht("vor SHPFile_Spaltenanlegen")
            If Not SHPFile_Spaltenanlegen(f, pt, shape, Result, sf) Then
                Return "fehler beim spaltenanlegen"
            End If

            glob2.nachricht("alle ebenen durchlaufen um kartenobjekte zu generieren: " & ebenen.Count)
            For I = 0 To ebenen.Count - 1
                glob2.nachricht(String.Format("vor {0}, Ebene-Nr: {1}----------------------", ebenen(I), I))
                hinweis$ = genKartenObjektePolygon(ebenen(I), aktBox, sf, ishp, paradigmaRBtyp)
                glob2.nachricht(String.Format("nach {0}, Ebene-Nr: {1}: {2}", ebenen(I), I, hinweis))
            Next

            ' glob2.nachricht(genKartenObjektePolygon(ebenen(I), aktBox, sf, ishp)'''''''''''''''''''''war glob2.nachrichtgenKa also deaktiviert
            Result = sf.StopEditingShapes(True, True)
            glob2.nachricht("polygon_shpfile_erzeugen ################ ende ##########")
            Return String.Empty
        Catch ex As Exception
            glob2.nachricht("Fehler in polygon_shpfile_erzeugen: " & ex.ToString)
            Return "Fehler in polygon_shpfile_erzeugen: " & ex.ToString
        End Try
    End Function




    Private Shared Sub Raumbezug2Polygonshp(ByVal vid As Integer,
                                            ByVal rbTable As DataTable,
                                            ByVal KartenJPGDir As String,
                                            ByRef shapeFehler As String,
                                            ByVal sf As MapWinGIS.Shapefile,
                                            ByRef ishp As Integer,
                                            SuchParadigmaRBtyp As RaumbezugsTyp)
        glob2.nachricht("Raumbezug2Polygonshp------------------------------------")
        Dim pt As New MapWinGIS.Point
        Dim shape As New MapWinGIS.Shape
        Dim irec As Long
        Try
            Dim dateiname As String = ""
            Dim dateititel As String = ""
            Dim abstract As String = ""
            Dim suchkette As String = " / "
            Dim ipos As Integer
            Dim gefundenerRBTyp As RaumbezugsTyp
            Dim rbid% = 0
            Dim f As New MapWinGIS.Field
            Dim result As Boolean
            glob2.nachricht("vor Raumbezug2shp3")
            For irec = 0 + ishp To rbTable.Rows.Count - 1 + ishp
                'initialize the shape
                shape = New MapWinGIS.Shape
                If SuchParadigmaRBtyp = RaumbezugsTyp.Polyline Then
                    shape.ShapeType = MapWinGIS.ShpfileType.SHP_POLYLINE
                End If
                If SuchParadigmaRBtyp = RaumbezugsTyp.Flurstueck Or SuchParadigmaRBtyp = RaumbezugsTyp.Polygon Then
                    shape.ShapeType = MapWinGIS.ShpfileType.SHP_POLYGON
                End If
                'Set up the points
                glob2.nachricht("vor Raumbezug2shp6")
                pt = New MapWinGIS.Point

                Try
                    gefundenerRBTyp = CType((clsDBtools.fieldvalue(rbTable.Rows(CInt(irec)).Item("typ"))), RaumbezugsTyp)
                Catch ex As Exception
                    glob2.nachricht("Fehler bei der zuweisung der typspalte!")
                    Continue For
                End Try

                If gefundenerRBTyp <> RaumbezugsTyp.Polyline And
                    gefundenerRBTyp <> RaumbezugsTyp.Polygon And
                    gefundenerRBTyp <> RaumbezugsTyp.Flurstueck Then Continue For



                rbid = CInt(rbTable.Rows(CInt(irec)).Item("raumbezugsid"))
                pt.x = CDbl(clsDBtools.fieldvalue(rbTable.Rows(CInt(irec)).Item("rechts")))
                pt.y = CDbl(clsDBtools.fieldvalue(rbTable.Rows(CInt(irec)).Item("hoch")))
                dateititel = CStr(clsDBtools.fieldvalue(rbTable.Rows(CInt(irec)).Item("titel")))
                abstract = CStr(clsDBtools.fieldvalue(rbTable.Rows(CInt(irec)).Item("abstract")))

                ipos = InStr(abstract, suchkette, CompareMethod.Text)
                dateiname = abstract.Substring(0, ipos + suchkette.Length - 3).Trim
                dateiname = String.Format("{0}\{1}", KartenJPGDir, dateiname)
                dateiname = dateiname.Replace("\", "/")

                If pt.x < 1 Or pt.y < 1 Then
                    glob2.nachricht("Koordinate kleiner 1! Wird ignoriert!")
                    Continue For
                End If
                ishp += 1
                ''Add the points to a shape
                Dim serial As String = ""
                Dim erfolgreich As Boolean = holeSerialShape(rbid, "", serial) ' holeRaumbezuge(vorgangsid, hinweis)
                If SuchParadigmaRBtyp = RaumbezugsTyp.Polygon Or SuchParadigmaRBtyp = RaumbezugsTyp.Flurstueck Then
                    If serial.StartsWith("3") Then
                        'ist polyline
                        Continue For
                    End If
                End If

                If SuchParadigmaRBtyp = RaumbezugsTyp.Polyline Then
                    If serial.StartsWith("4") Then
                        'ist polygon
                        Continue For
                    End If
                End If

                If erfolgreich Then
                    If Not MyMapWinGisTools.getShapeFromSerial(serial, shape) Then
                        glob2.nachricht("umwandlung serial in shape nicht gelungen")
                        Exit Sub
                    End If

                    glob2.nachricht("vor Raumbezug2shp7")
                    'Add the shape to the shapefile
                    result = sf.EditInsertShape(shape, ishp)
                    If result = False Then
                        shapeFehler = SHP_Fehlerbehandlung(sf, pt, shape, f)
                        Exit Sub
                    End If
                    Spaltenwerte_Setzen(result, sf, ishp, irec, rbTable, pt, shape, f, shapeFehler, dateiname, dateititel, vid, rbid, gefundenerRBTyp)
                End If
            Next irec
            glob2.nachricht("Raumbezug2Pointshp ################# ende ok ########")
        Catch ex As Exception
            glob2.nachricht_und_Mbox(String.Format("FEHLER im Kartenmodul(Raumbezug2Pointshp): {0}{1}", vbCrLf, ex))
        End Try
    End Sub

    Public Shared  Function gkstringsausserial_generieren(ShapeSerial As String) As List(Of String)
        'esrishape oder postgis?
        'kann jetzt aber noch die Anzahl der punkte als 3 byte enthalten zb. "5;0;18;48..."
        Dim out As New List(Of String)
        Dim gkstring, teile() As String
        Dim ipos As Integer
        Dim teileZaehl As Integer = 0

        Dim polytrenner As String = ")),(("
        If ShapeSerial IsNot Nothing Then
            Dim g As String = ShapeSerial
            If IsNumeric(ShapeSerial.Substring(0, 1)) Then
                'aus shapefile
                g = g.Replace("|", ";")
                gkstring = g.Substring(4, g.Length - 4)
                out.Add(gkstring)
            Else
                'aus postgis
                If g.StartsWith("MULTIPOLYGON") Then
                    g = MULTIPOLGONanfangEntfernen(g)
                    g = MULTIPOLGONendeEntfernen(g)
start:
                    ipos = g.IndexOf(polytrenner)
                    If g.Length > 4 Then
                        If ipos < 1 Then ipos = g.Length
                        teileZaehl += 1
                        ReDim Preserve teile(teileZaehl)
                        teile(teileZaehl) = g.Substring(0, ipos)
                        If (ipos + 3) < g.Length Then
                            g = g.Substring(ipos + 3)
                        Else
                            g = ""
                        End If
                        teile(teileZaehl) = POLYGONklammernWeg(teile(teileZaehl))
                        out.Add(teile(teileZaehl))
                        GoTo start
                    Else
                        Return out
                    End If

                Else
                    g = POLYGONklammernWeg(g)
                    gkstring = g
                    out.Add(g)
                    Return out
                End If

            End If
        End If
        Return out
    End Function
      Private shared Function MULTIPOLGONanfangEntfernen(g As String) As String
        g = g.Replace("MULTIPOLYGON(", "")
        Return g
    End Function

    Private shared  Function MULTIPOLGONendeEntfernen(g As String) As String
        g = g.Substring(0, Len(g) - 1)
        Return g
    End Function

    Private  shared Function POLYGONklammernWeg(g As String) As String
        g = g.Replace("MULTIPOLYGON", "")
        g = g.Replace("POLYGON", "")
        g = g.Replace("LINESTRING", "")
        g = g.Replace("CURVECOMPOUNDCURVE", "")
        g = g.Replace("CURVECOMPOUNDCURVE", "")
        g = g.Replace("CIRCULARSTRING", "")
        g = g.Replace("COMPOUNDCURVE", "")
        g = g.Replace("CURVE", "")
        g = g.Replace("POINT", "")
        g = g.Replace("(", "")
        g = g.Replace(")", "")
        g = g.Replace(",", ";")
        g = g.Replace(" ", ";")
        Return g
    End Function
            Public shared Sub serialAusGkstring_generieren(GKstring As string,ByRef ShapeSerial As string)
        If GKstring IsNot Nothing Then
            Dim s$ = GKstring.Replace(";", "|")
            Dim a As String()=s.Split("|"c)
            Dim erstesPaar As String= "|" & a(0) & "|" & a(1) & "|" 
            s=s.Replace(".",",")
             
            ShapeSerial = "5;0;" & s$ & erstesPaar
        End If
    End Sub
    	Public shared Function ShapeSerialstringIstWKT(ShapeSerial As string) As Boolean
        Try
            If IsNumeric(ShapeSerial.Substring(0, 1)) Then
                Return False
            Else
                Return True
            End If 
        Catch ex As Exception
        End Try
    End Function
    Shared Function holeSerialShape(ByVal raumbezugsid As Integer, ByVal hinweis As String, ByRef serial As String) As Boolean
        glob2.nachricht("holeSerialShape")
        Dim retvalue As Boolean = False
        Try
            If raumbezugsid < 1 Then
                glob2.nachricht("raumbezugsid < 1 ")
                Return False
            End If
            Dim neurec As IDB_grundfunktionen
            neurec = myGlobalz.raumbezugsRec
            neurec.mydb.SQL = "select * from raumbezug2geopolygon where RaumbezugsID=" & raumbezugsid
            glob2.nachricht(neurec.mydb.SQL)
            hinweis$ = neurec.getDataDT()
            glob2.nachricht(hinweis)
            glob2.nachricht("Treffer: " & neurec.dt.Rows.Count.ToString)
            If neurec.dt.Rows.Count > 0 Then
                serial = CStr(clsDBtools.fieldvalue(neurec.dt.Rows(0).Item("SerialShape")))
                If   ShapeSerialstringIstWKT(serial) then
                     Dim a As  List(Of String)= gkstringsausserial_generieren(serial)
                    serialAusGkstring_generieren( a(0),serial )
                End If
               
                retvalue = True
            Else
                serial = ""
                retvalue = False
            End If
            If String.IsNullOrEmpty(serial) Then
                glob2.nachricht("FEHLER im Kartenmodul(holeSerialShape): serial ist NULL")
                retvalue = False
            End If

            neurec = Nothing
            Return retvalue

        Catch ex As Exception
            glob2.nachricht_und_Mbox(String.Format("FEHLER im Kartenmodul(holeSerialShape): {0}{1}", vbCrLf, ex))
            Return False
        End Try
    End Function
End Class
