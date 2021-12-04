Imports System.Data
Public Class clsMiniMapTools

    'Private Shared Sub Polygon_MouseDownFS(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    Dim eee As System.Windows.Shapes.Polygon = DirectCast(e.Source, System.Windows.Shapes.Polygon)
    '    ' MsgBox(String.Format("Baustelle !!!!{0}{1}", vbCrLf, eee.Tag))

    '    Dim aa As New wininfo(eee.Tag.ToString)
    '    aa.ShowDialog()

    '    e.Handled = True
    'End Sub
    'Private Shared Sub Polygon_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    Dim eee As System.Windows.Shapes.Polygon = DirectCast(e.Source, System.Windows.Shapes.Polygon)
    '    ' MsgBox(String.Format("Baustelle !!!!{0}{1}", vbCrLf, eee.Tag))
    '    If CLstart.myc.kartengen.aktMap.ActiveLayer = "tk5" Then
    '        Dim aa As New wininfo(eee.Tag.ToString)
    '        aa.ShowDialog()
    '    Else
    '        dbabfrage(eee.Tag.ToString)
    '    End If
    '    e.Handled = True
    'End Sub







    'Private Shared Sub getMapshareUser()
    '    Dim pwtet As String
    '    If initP.getValue("MiniMap.MapStatus") = "mapshare" Then
    '        pwtet = holePasswordFuerGisUser_dballe(myglobalz.sitzung.aktBearbeiter.username, myglobalz.sitzung.webgisREC)
    '        nachricht("nach holePasswordFuerGisUser")
    '        If String.IsNullOrEmpty(pwtet) Then
    '            CLstart.myc.kartengen.mapcred.pw = "" ' md5("intranet")
    '            '  MessageBox.Show("Sie müssen im WebGIS registriert sein um die Minimap nutzen zu können!")
    '        Else
    '            CLstart.myc.kartengen.mapcred.pw = pwtet
    '        End If
    '    End If
    'End Sub
    'Public Shared Sub initMinimapParameter(ByVal globalrange As clsRange, ByVal rbrange As clsRange)
    '    nachricht("in initMinimap---------------------------------")
    '    Try
    '        CLstart.myc.kartengen.gis_serverD = initP.getValue("GisServer.gis_serverD")
    '        CLstart.myc.kartengen.mapcred.username = myglobalz.sitzung.aktBearbeiter.username
    '        'passowrd aus internuserdb holen
    '        nachricht("vor holePasswordFuerGisUser")

    '        'getMapshareUser()
    '        CLstart.myc.kartengen.FitGlobal.rangekopierenVon(globalrange)
    '        CLstart.myc.kartengen.FitRaumbezuege.rangekopierenVon(rbrange)
    '        nachricht("globalrange: " & globalrange.toString)
    '        nachricht("rbrange: " & rbrange.toString)
    '        ' Dim erfolg As Boolean = clsMiniMapTools.getMapCookie_alleDB(CLstart.myc.kartengen.aktMap, myglobalz.sitzung.aktVorgangsID)
    '        If Not CLstart.myc.kartengen.aktMap.aktrange.istBrauchbar Or fitRbistbesserAlsAktrange() Then
    '            If CLstart.myc.kartengen.FitRaumbezuege.istBrauchbar Then
    '                CLstart.myc.kartengen.aktMap.aktrange.rangekopierenVon(CLstart.myc.kartengen.FitRaumbezuege)
    '            Else
    '                CLstart.myc.kartengen.aktMap.aktrange.rangekopierenVon(CLstart.myc.kartengen.FitGlobal)
    '            End If
    '        End If


    '        nachricht("out initMinimap")
    '    Catch ex As Exception
    '        nachricht("Fehler in initMinimap ---------------------------" ,ex)
    '    End Try
    'End Sub

    'Sub defineRange(ByVal xl As Double, ByVal xh As Double, ByVal yl As Double, ByVal yh As Double, ByVal zielrange As clsRange) 'clstart.myc.kartengen.aktrange
    '    zielrange.xl = xl
    '    zielrange.xh = xh
    '    zielrange.yl = yl
    '    zielrange.yh = yh
    'End Sub


    'Public Shared Function makeOutfileschwanz() As String
    '    Dim a$ = Now.Year & Now.Day & Now.Hour & Now.Second & Now.Millisecond & "Paradigma"
    '    Return a$
    'End Function



    'Private Shared Sub handlePunkthafte(ByVal myCanvas As Canvas,
    '                                    ByVal dierange As clsRange,
    '                                    ByRef tag As String,
    '                                    ByVal zindex As Integer,
    '                                    ByVal aktpoint As myPoint,
    '                                    ByRef winPoint As System.Drawing.Point,
    '                                    ByVal kreiscanvas As clsCanvas,
    '                                    ByVal myFillColorBrush As SolidColorBrush,
    '                                    ByVal rau As DataRow)
    '    Try
    '        If Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Foto Or
    '            Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Adresse Or
    '            Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Umkreis Or
    '            Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Punkt Then
    '            'Alles was punkthaft dargestellt wird
    '            aktpoint.X = Convert.ToDouble(rau.Item("rechts"))
    '            aktpoint.Y = Convert.ToDouble(rau.Item("hoch"))

    '            winPoint.X = CInt(aktpoint.X)
    '            winPoint.Y = CInt(aktpoint.Y)
    '            If Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Foto Then
    '                tag = Convert.ToString(rau.Item("abstract")) & " # " & Convert.ToString(rau.Item("sekid"))
    '                'myFillColorBrush.Color = Colors.Orange erzeugt fehler
    '            Else
    '                tag = Convert.ToString(rau.Item("TITEL")) & " " & Convert.ToString(rau.Item("abstract"))
    '                ''  myFillColorBrush.Color = Colors.Red
    '            End If
    '            Dim name As String = Convert.ToString(rau.Item("freitext"))

    '            name = LIBgemeinsames.clsString.changeHTML2text(name)
    '            name = LIBgemeinsames.clsString.normalize(name)
    '            name = name.Replace(",", "_")
    '            name = name.Replace(".", "_")
    '            name = name.Replace("!", "_")
    '            name = name.Replace("'", "_")
    '            If aktpoint.X > 3000 And aktpoint.Y > 30000 Then
    '                If dierange.inside(winPoint) Then
    '                    addPointsGKfuerEllipse(aktpoint, 25, tag, myCanvas, dierange, kreiscanvas,
    '                                           Brushes.Red, Brushes.Red, name, zindex,
    '                                            Convert.ToBoolean(rau.Item("mitetikett")))
    '                End If
    '            End If
    '        End If
    '    Catch ex As Exception
    '        nachricht("Warnung in handlePunkthafte: " ,ex)
    '    End Try
    'End Sub


    'Private Shared Function overlayObjekteDarstellen(ByVal rau As DataRow) As Boolean
    '    Try
    '        Return CBool(rau.Item("ismapenabled"))
    '    Catch ex As Exception
    '        nachricht("Fehler in overlayobjektDarstellen:", ex)
    '        Return True
    '    End Try

    'End Function

    ''' <summary>
    ''' bezieht sich auf die tabelle raumbezugplus
    ''' diese sollte immer auf myGlobalz.sitzung.raumbezugsRec.dt  liegen
    '''    alle anderen werden ausgefiltert
    ''' </summary>
    ''' <param name="myCanvas"></param>
    ''' <param name="dierange"></param>
    ''' <param name="ableitungskreis"></param>
    ''' <remarks></remarks>
    'Public Shared Sub refreshMapOverlay(ByVal myCanvas As Canvas, ByVal dierange As clsRange,
    '                                    ByVal ableitungskreis As clsAbleitungskreis,
    '                                    eigentuemerFunktionAktiv As Boolean)
    '    Dim tag As String = ""
    '    Dim zindex As Integer
    '    Dim aktpoint As New myPoint, winPoint As New System.Drawing.Point
    '    Dim kreiscanvas As New clsCanvas
    '    Dim myFillColorBrush, myStrokeColorBrush As New SolidColorBrush
    '    Dim zaehler As Integer = 0
    '    Try
    '        kreiscanvas.w = CLng(myCanvas.Width)
    '        kreiscanvas.h = CLng(myCanvas.Height)
    '        myStrokeColorBrush = Brushes.DarkBlue
    '        '   myStrokeColorBrush = Brushes.Cyan
    '        If Not myglobalz.sitzung.raumbezugsRec.dt.IsNothingOrEmpty Then
    '            zindex = 3000
    '            If Not myglobalz.sitzung.raumbezugsRec.dt.TableName.ToLower = "raumbezugplus" Then
    '                Exit Sub
    '            End If
    '            For Each rau As DataRow In myglobalz.sitzung.raumbezugsRec.dt.AsEnumerable
    '                If Not overlayObjekteDarstellen(rau) Then Continue For
    '                handlePunkthafte(myCanvas, dierange, tag, zindex, aktpoint, winPoint, kreiscanvas, Brushes.DarkGreen, rau)
    '                handleLinienUndFlaechen(myCanvas, dierange, tag, zindex, kreiscanvas, myFillColorBrush, myStrokeColorBrush, zaehler, rau, eigentuemerFunktionAktiv)
    '            Next
    '        Else
    '            nachricht("refreshEllipsen: keine  raumbezüge vorhanden !!! ")
    '        End If
    '        'MaleAbleitungskreis(myCanvas, dierange, ableitungskreis, zindex, aktpoint, winPoint, kreiscanvas, myFillColorBrush, myStrokeColorBrush)
    '    Catch ex As Exception
    '        nachricht("fehler in refreshEllipsen: " ,ex)
    '    End Try
    'End Sub

    'Public Shared Sub addPointsGKfuerEllipse(ByVal aktpoint As myPoint,
    '                                ByRef radius As Integer,
    '                                ByVal tag As String,
    '                                ByVal myCanvas As Canvas,
    '                                ByVal dierange As clsRange,
    '                                ByVal kreiscanvas As clsCanvas,
    '                                ByVal myFillColorOrVordergrund As SolidColorBrush,
    '                                ByVal myStrokeColorOrBackground As SolidColorBrush,
    '                                ByVal name As String,
    '                                ByVal zindex As Integer,
    '                                mitEtikett As Boolean)
    '    Dim neupoint As New myPoint
    '    If radius < 1 Then radius = 1
    '    neupoint = punktvonGKnachCanvasUmrechnen(aktpoint, dierange, kreiscanvas, 1)

    '    If mitEtikett Then
    '        addEtikett(CInt(neupoint.X), CInt(neupoint.Y), radius, tag, myCanvas, myFillColorOrVordergrund, myStrokeColorOrBackground, name, zindex)
    '    Else
    '        addEllipse(CInt(neupoint.X), CInt(neupoint.Y), radius, tag, myCanvas, myFillColorOrVordergrund, myStrokeColorOrBackground, name, zindex)
    '    End If
    '    neupoint = Nothing
    'End Sub

    'Private Shared Sub addPolygonSchleifeKeypoints(ByVal clsParapolygon As clsParapolygon,
    '                                                ByVal tag As String,
    '                                                ByVal myCanvas As Canvas,
    '                                                ByVal name As String,
    '                                                ByVal zindex As Integer,
    '                                                ByVal zaehler As Integer,
    '                                                ByVal tooltip As String,
    '                                                ByVal neupointsCanvas As myPoint(),
    '                                                ByVal myPointCollection As PointCollection,
    '                                                ByVal multipolygonpointer As Integer(),
    '                                                ByVal koordCursor As Integer,
    '                                                ByVal anzahlKeyPoints As Integer,
    '                                                ByVal lokzaehler As Integer,
    '                                                ByVal myFillColorBrush As SolidColorBrush,
    '                                                ByVal myStrokeColorBrush As SolidColorBrush)
    '    Dim x As Double
    '    Dim y As Double
    '    Try
    '        Dim atest = neupointsCanvas.GetUpperBound(0)
    '        For i = 0 To neupointsCanvas.GetUpperBound(0)
    '            lokzaehler = lokzaehler + i
    '            If IsNothing(neupointsCanvas(i)) Then
    '                Continue For
    '            End If
    '            If IsNothing(neupointsCanvas(i).X) Then
    '                Continue For
    '            End If
    '            x = CInt(neupointsCanvas(i).X)
    '            y = CInt(neupointsCanvas(i).Y)
    '            myPointCollection.Add(New Point(x, y))
    '            'If i = anzahlKeyPoints -1  Then  raus, weil sonst fehlt ein keypoint in linien 26084
    '            If i = anzahlKeyPoints Then
    '                If clsParapolygon.Typ = RaumbezugsTyp.Polygon Or clsParapolygon.Typ = RaumbezugsTyp.Flurstueck Then
    '                    drawPolygon2Canvas(tag, name, lokzaehler, myPointCollection, myCanvas, zindex, tooltip, myFillColorBrush,
    '                                                 myStrokeColorBrush)
    '                End If
    '                If clsParapolygon.Typ = RaumbezugsTyp.Polyline Then
    '                    drawPolyline2Canvas(tag, name, zaehler, myPointCollection, myCanvas, 10000, myFillColorBrush,
    '                                                 myStrokeColorBrush)
    '                End If
    '                myPointCollection.Clear()
    '                koordCursor += 1
    '                If koordCursor <= multipolygonpointer.GetUpperBound(0) Then
    '                    anzahlKeyPoints = multipolygonpointer(koordCursor)
    '                End If
    '            End If
    '        Next
    '    Catch ex As Exception
    '        'nachricht("fehler in addPolygonSchleifeKeypoints: " & Environment.NewLine &   clsParapolygon.GKstring.tostring & Environment.NewLine ,ex)
    '        nachricht("fehler in addPolygonSchleifeKeypoints: " & Environment.NewLine & Environment.NewLine ,ex)
    '    End Try
    'End Sub
    'Private Shared Sub addPolygonFromDBToCanvas(clsParapolygon As clsParapolygon,
    '                            ByVal tag As String,
    '                            dierange As clsRange,
    '                            ByVal myCanvas As Canvas,
    '                            kreiscanvas As clsCanvas,
    '                            ByVal myFillColorBrush As SolidColorBrush,
    '                            ByVal myStrokeColorBrush As SolidColorBrush,
    '                            ByVal name As String,
    '                            ByVal zindex As Integer,
    '                            zaehler As Integer,
    '                            tooltip As String)
    '    Dim neupointsCanvas() As myPoint
    '    Dim dezimalTrenner As Char
    '    Dim myPointCollection As New PointCollection
    '    Dim multipolygonpointer() As Integer = Nothing
    '    Dim i As Integer
    '    Try
    '        If clsParapolygon.GKstringList Is Nothing Then
    '            Debug.Print("")
    '        End If
    '        If clsParapolygon.GKstringList.Count = 0 Then
    '            Debug.Print("")
    '        End If
    '        dezimalTrenner = bestimmeDezimalTrenner(clsParapolygon.GKstringList)
    '        For i = 0 To clsParapolygon.GKstringList.Count - 1 'multipolygonschleife
    '            clsParapolygon.GKstring = clsParapolygon.GKstringList(i)
    '            CLstart.myc.punktarrayInM = clsMiniMapTools.zerlegeInPunkte(clsParapolygon.GKstring, dezimalTrenner, multipolygonpointer,
    '                                                                              CInt(clsParapolygon.RaumbezugsID), clsParapolygon.ShapeSerialstringIstWKT)
    '            If CLstart.myc.punktarrayInM IsNot Nothing Then
    '                neupointsCanvas = clsMiniMapTools.polygonNachCanvasUmrechnen(CLstart.myc.punktarrayInM, dierange, kreiscanvas)
    '                ReDim Preserve multipolygonpointer(multipolygonpointer.GetUpperBound(0) + 1)
    '                multipolygonpointer(multipolygonpointer.GetUpperBound(0)) = neupointsCanvas.GetUpperBound(0)
    '                Dim koordCursor As Integer = 0
    '                Dim anzahlKeyPoints = multipolygonpointer(koordCursor)
    '                Dim lokzaehler As Integer = zaehler
    '                addPolygonSchleifeKeypoints(clsParapolygon, tag, myCanvas, name, zindex, zaehler, tooltip, neupointsCanvas, myPointCollection,
    '                                            multipolygonpointer, koordCursor, anzahlKeyPoints, lokzaehler, myFillColorBrush,
    '                                            myStrokeColorBrush)
    '                ' punktarrayInM = Nothing
    '                neupointsCanvas = Nothing
    '            End If
    '        Next
    '        Debug.Print("")
    '    Catch ex As Exception
    '        nachricht("fehler in addPolygonFromDB: " & Environment.NewLine &
    '                        clsParapolygon.GKstring & Environment.NewLine ,ex)
    '    End Try

    'End Sub
    'Public Shared Sub addEtikett(ByVal left As Integer,
    '                        ByVal top As Integer,
    '                        ByRef fontsaize As Integer,
    '                        ByVal tag As String,
    '                        ByVal myCanvas As Canvas,
    '                        ByVal vordergrundfarbe As SolidColorBrush,
    '                        ByVal hintergrundfarbe As SolidColorBrush,
    '                        ByVal name As String,
    '                        ByVal zindex As Integer)
    '    Dim etikett As New TextBlock
    '    'etikett.Foreground = Brushes.Red  
    '    'etikett.Background = Brushes.Black  
    '    etikett.Foreground = hintergrundfarbe 'Brushes.DarkBlue
    '    etikett.Background = Brushes.Transparent
    '    etikett.FontFamily = New FontFamily("Arial")
    '    etikett.FontSize = fontsaize '16
    '    etikett.Text = name '& Environment.NewLine & tag
    '    'etikett.Height = radius%       '311
    '    'etikett.Width = radius%        '204
    '    If Not String.IsNullOrEmpty(name) Then etikett.Name = name
    '    etikett.Tag = tag
    '    etikett.ToolTip = tag
    '    'AddHandler etikett.MouseDown, AddressOf elipsenHandleMousedown
    '    'AddHandler etikett.MouseEnter, AddressOf elipsenHandle2MouseEnter
    '    'AddHandler etikett.MouseMove, AddressOf elipsenHandle2MouseMove
    '    Canvas.SetZIndex(etikett, zindex)
    '    Canvas.SetLeft(etikett, left - (fontsaize / 2))
    '    Canvas.SetTop(etikett, top - (fontsaize / 2))
    '    myCanvas.Children.Add(etikett)
    'End Sub
    'Private Shared Sub setEllipseName(ByVal name As String, ByVal eli1 As Ellipse)
    '    Try
    '        If Not String.IsNullOrEmpty(name) Then
    '            If IsNumeric(name) Then
    '                eli1.Name = "L" & name
    '            Else
    '                eli1.Name = name
    '            End If

    '        End If
    '    Catch ex As Exception
    '        nachricht("warnung: setEllipseName: bei zuweisung des namens ist ein fehler aufgetreten: " & name)
    '        eli1.Name = ""
    '    End Try
    'End Sub
    'Public Shared Sub addEllipse(ByVal left As Integer,
    '                                ByVal top As Integer,
    '                                ByRef radius As Integer,
    '                                ByVal tag As String,
    '                                ByVal myCanvas As Canvas,
    '                                ByVal myFillColorBrush As SolidColorBrush,
    '                                ByVal myStrokeColorBrush As SolidColorBrush,
    '                                ByVal name As String,
    '                                ByVal zindex As Integer)
    '    Dim eli1 As New Ellipse
    '    '  eli1.Fill = myFillColorBrush
    '    eli1.Stroke = myStrokeColorBrush
    '    eli1.Height = radius%       '311
    '    eli1.Width = radius%        '204
    '    setEllipseName(name, eli1)

    '    eli1.Tag = tag
    '    eli1.ToolTip = tag
    '    AddHandler eli1.MouseDown, AddressOf elipsenHandleMousedown
    '    AddHandler eli1.MouseEnter, AddressOf elipsenHandle2MouseEnter
    '    AddHandler eli1.MouseMove, AddressOf elipsenHandle2MouseMove
    '    Canvas.SetZIndex(eli1, zindex)
    '    Canvas.SetLeft(eli1, left - (eli1.Width / 2))
    '    Canvas.SetTop(eli1, top - (eli1.Height / 2))
    '    myCanvas.Children.Add(eli1)
    'End Sub
    ''' <summary>
    ''' divisor wird benötigt, weil flächenkoordinaten immer mit 100 multipliziert wurden
    '''                             punktkoordinaten aber nicht
    ''' bei punkkoordinaten muss der divisor=1 angegeben werden sonst ist standard 100
    ''' </summary>
    ''' <param name="aktpoint"></param>
    ''' <param name="birdsrange"></param>
    ''' <param name="Kreiscanvas"></param>
    ''' <param name="divisor"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function punktvonGKnachCanvasUmrechnen(ByVal aktpoint As myPoint, ByVal birdsrange As clsRange,
    '                                                     ByVal Kreiscanvas As clsCanvas,
    '                                                     Optional divisor As Integer = 100) As myPoint
    '    Try
    '        Dim testr As Double, testh As Double
    '        Dim neupoint As New myPoint
    '        nachricht("divisor:" & divisor)
    '        nachricht("aktpoint.X:" & aktpoint.X)
    '        nachricht("aktpoint.y:" & aktpoint.Y)
    '        nachricht("divisor:" & divisor)
    '        nachricht("divisor:" & divisor)
    '        testr = ((aktpoint.X / divisor) - birdsrange.xl) / birdsrange.xdif
    '        testr = testr * Kreiscanvas.w
    '        testh = ((aktpoint.Y / divisor) - birdsrange.yl) / birdsrange.ydif
    '        testh = Kreiscanvas.h - (testh * Kreiscanvas.h)
    '        testr = Fix(testr)
    '        testh = Fix(testh)
    '        neupoint.X = CInt(testr)
    '        neupoint.Y = CInt(testh)
    '        Return neupoint
    '    Catch ex As Exception
    '        nachricht("-warnung: " ,ex)
    '        Return Nothing
    '    End Try
    'End Function


    'Shared Sub ausschnittNeuBerechnen(ByVal RubberbandStartpt As Point?, ByVal RubberbandEndpt As Point?)
    '    Try
    '        Dim newpoint As New myPoint
    '        newpoint.X = CDbl(RubberbandStartpt.Value.X)
    '        newpoint.Y = CDbl(RubberbandStartpt.Value.Y)
    '        Dim newpoint2 As New myPoint
    '        newpoint2.X = CDbl(RubberbandEndpt.Value.X)
    '        newpoint2.Y = CDbl(RubberbandEndpt.Value.Y)

    '        newpoint = clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(newpoint, CLstart.myc.kartengen.aktMap.aktrange, CLstart.myc.kartengen.aktMap.aktcanvas)
    '        newpoint2 = clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(newpoint2, CLstart.myc.kartengen.aktMap.aktrange, CLstart.myc.kartengen.aktMap.aktcanvas)
    '        If newpoint.X > newpoint2.X Then
    '            CLstart.myc.kartengen.aktMap.aktrange.xl = newpoint2.X
    '            CLstart.myc.kartengen.aktMap.aktrange.xh = newpoint.X
    '        Else
    '            CLstart.myc.kartengen.aktMap.aktrange.xl = newpoint.X
    '            CLstart.myc.kartengen.aktMap.aktrange.xh = newpoint2.X
    '        End If

    '        If newpoint.Y > newpoint2.Y Then
    '            CLstart.myc.kartengen.aktMap.aktrange.yl = newpoint2.Y
    '            CLstart.myc.kartengen.aktMap.aktrange.yh = newpoint.Y
    '        Else
    '            CLstart.myc.kartengen.aktMap.aktrange.yl = newpoint.Y
    '            CLstart.myc.kartengen.aktMap.aktrange.yh = newpoint2.Y
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Daneben. Bitte nochmal probieren!")
    '    End Try
    'End Sub

    'Public Shared Sub korrigiereAktrange(ByRef korrange As clsRange, ByVal lokcanvas As Canvas)
    '    'die aktrange muss auf das seitenverhältnis des canvas-steurelementes korrigiert werden
    '    Dim quotecanvas As Double = lokcanvas.Height / lokcanvas.Width
    '    Dim quoterange As Double = korrange.ydif() / korrange.xdif()
    '    Dim lenge As Double
    '    If quoterange > quotecanvas Then
    '        'x-erweitern
    '        lenge = korrange.ydif / quotecanvas
    '        korrange.xh = korrange.xl + (lenge / 2)
    '    Else
    '        'y-erweitern
    '        lenge = korrange.xdif * quotecanvas
    '        korrange.yh = korrange.yl + lenge
    '    End If
    'End Sub

    'Public Shared Function imageMapDateiVorhanden(ByVal imageMap As String) As Boolean
    '    If String.IsNullOrEmpty(imageMap) Then
    '        nachricht("Hinweis in imageMap2PolygonMap: mapdatei ist nicht definiert. Maßstab zu klein? ebene nicht aktivierbar?")
    '        Return False
    '    End If
    '    Return True
    'End Function



    'Private Shared Function imageMapDateigefunden(imapdatei As String, dauer As Integer, ByRef count As Integer) As Boolean

    '    For i = 1 To 10
    '        count = i
    '        Dim test As New IO.FileInfo(imapdatei)
    '        If Not test.Exists Then
    '            Threading.Thread.Sleep(dauer)
    '        Else
    '            l("dauer" & i * dauer)
    '            Return True
    '        End If
    '    Next
    '    Return False
    'End Function


    ''' <summary>
    ''' macht aus den beiden Punktcoordinaten ein quadrat
    ''' </summary>
    ''' <param name="coords"></param>
    ''' <returns>neuer coodinatenstring</returns>
    ''' <remarks></remarks>
    'Private Shared Function GetQuadrat(ByVal coords As String) As String
    '    Dim p As String(), quadrat As String

    '    Try
    '        If coords.IsNothingOrEmpty Then
    '            nachricht("GetQuadrat coords ist leer. exit")
    '            Return ""
    '        End If
    '        p = coords.Split(","c)
    '        quadrat = p(0) & "," & p(1) & "," & p(0) & "," & p(3) & "," & p(2) & "," & p(3) & "," & p(2) & "," & p(1) & "," & p(0) & "," & p(1)
    '        Return quadrat
    '    Catch ex As Exception
    '        nachricht("fehler in GetQuadrat coords : " ,ex)
    '        Return ""
    '    End Try
    'End Function
    'Private Shared Function GetTagValue(ByVal line As String, ByRef tag As String) As String
    '    Dim rest As String, pos As Integer, textMarker As String = "'"
    '    nachricht("GetTagValue --------  ")

    '    Try
    '        If line.IsNothingOrEmpty Then Return ""
    '        If tag.IsNothingOrEmpty Then Return ""

    '        nachricht("GetTagValue -------- eingabe ist gültig")
    '        If Not line.ToLower.Contains(tag.ToLower) Then Return ""

    '        If tag.ToLower = "shape=" Then
    '            If line.ToLower.Contains("shape=point") Then Return "point"
    '            If line.ToLower.Contains("shape=polygon") Then Return "polygon"
    '            If line.ToLower.Contains("shape=annotation") Then Return "annotation"
    '        End If

    '        textMarker = LIBgemeinsames.clsString.getTextDelimiter(line)
    '        'typen coord und title und href
    '        nachricht("GetTagValue -------- eingabe ist gültig")
    '        pos = line.IndexOf(tag)
    '        ' blank=
    '        If pos > 0 Then
    '            pos = pos + tag.Length + 1
    '            rest = line.Substring(pos, line.Length - pos)
    '            pos = rest.IndexOf(" ")
    '            If pos < 0 Then
    '                ' weil href am ende liegt gibts kein blank
    '                pos = rest.Length - 2
    '            End If
    '            rest = rest.Substring(0, pos)
    '            rest = rest.Replace(Chr(34), "").Replace("'", "")
    '        Else
    '            Return ""
    '        End If
    '        Return rest
    '        Return ""
    '    Catch ex As Exception
    '        nachricht("fehler in GetTagValue: " ,ex)
    '        Return ""
    '    End Try
    'End Function


    'Private Shared Function GetTagValueMS(ByVal line As String, ByRef tag As String) As String
    '    Dim rest As String, pos As Integer ', textMarker As String = "'"
    '    ' nachricht("GetTagValue --------  ")

    '    Try
    '        If line.IsNothingOrEmpty Then Return ""
    '        If tag.IsNothingOrEmpty Then Return ""

    '        '  nachricht("GetTagValue -------- eingabe ist gültig")
    '        If Not line.ToLower.Contains(tag.ToLower) Then Return ""
    '        'shape="poly"
    '        If tag.ToLower = "shape=" Then
    '            If line.ToLower.Contains("shape=point") Then Return "point"
    '            If line.ToLower.Contains("shape=polygon") Or
    '                line.ToLower.Contains("shape=""poly""") Then Return "polygon"
    '            If line.ToLower.Contains("shape=annotation") Then Return "annotation"
    '        End If

    '        'typen coord und title und href
    '        ' nachricht("GetTagValue -------- eingabe ist gültig")
    '        'coords="0,0 1022,0 1037,16 1121,38 1121,0 1121,624 1121,464 1066,428 904,283 750,157 582,83 396,20 174,3 55,0 0,0"
    '        pos = line.IndexOf(tag)
    '        If pos > 0 Then
    '            pos = pos + tag.Length + 1
    '            rest = line.Substring(pos, line.Length - pos)
    '            pos = rest.IndexOf("""")
    '            If pos < 0 Then
    '                ' weil href am ende liegt gibts kein blank
    '                pos = rest.Length - 2
    '            End If
    '            rest = rest.Substring(0, pos)
    '            rest = rest.Replace(Chr(34), "").Replace("'", "")
    '        Else
    '            Return ""
    '        End If
    '        rest = rest.Replace("href=", "")
    '        Return rest

    '        pos = line.IndexOf(tag)
    '        ' blank=
    '        Return ""
    '    Catch ex As Exception
    '        nachricht("fehler in GetTagValue: " ,ex)
    '        Return ""
    '    End Try
    'End Function
    'Private Shared Function GetTagValueAlt(ByVal line As String, ByRef tag As String) As String
    '    Dim rest As String, pos As Integer, textMarker As String = "'"
    '    nachricht("GetTagValue --------  ")

    '    Try
    '        If line.IsNothingOrEmpty Then Return ""
    '        If tag.IsNothingOrEmpty Then Return ""
    '        If line.Contains("'") Then textMarker = "'"
    '        If line.Contains(Chr(34)) Then textMarker = Chr(34)


    '        nachricht("GetTagValue -------- eingabe ist gültig")
    '        pos = line.IndexOf(tag)
    '        If pos > 0 Then
    '            pos = pos + tag.Length + 1
    '            rest = line.Substring(pos, line.Length - pos)
    '            pos = rest.IndexOf(textMarker)
    '            rest = rest.Substring(0, pos)
    '        Else
    '            Return ""
    '        End If
    '        Return rest
    '    Catch ex As Exception
    '        nachricht("fehler in GetTagValue: " ,ex)
    '        Return ""
    '    End Try
    'End Function
    'End Function


    'Shared Function bildeMyPointCollection(ByVal coords As String) As PointCollection
    '    Dim punkte() As String
    '    Dim myPointCollection As New PointCollection
    '    Dim x, y As Integer
    '    If coords.IsNothingOrEmpty Then Return Nothing
    '    Try
    '        punkte = coords.Split(","c)
    '        For i = 0 To punkte.GetUpperBound(0) Step 2
    '            x = CInt(punkte(i))
    '            y = CInt(punkte(i + 1))
    '            myPointCollection.Add(New Point(x, y))
    '        Next
    '        x = CInt(punkte(0))
    '        y = CInt(punkte(0 + 1))
    '        myPointCollection.Add(New System.Windows.Point(x, y))
    '        Return myPointCollection
    '    Catch ex As Exception
    '        nachricht("fehler in bildeMyPointCollection: " ,ex)
    '        Return Nothing
    '    End Try
    'End Function
    'Shared Function bildeMyPointCollectionMapserver(ByVal coords As String) As PointCollection
    '    Dim punkte() As String
    '    Dim myPointCollection As New PointCollection
    '    Dim x, y As Integer
    '    If coords.IsNothingOrEmpty Then Return Nothing
    '    Dim b() As String
    '    Try
    '        punkte = coords.Split(" "c)
    '        For i = 0 To punkte.GetUpperBound(0) Step 1
    '            b = punkte(i).Split(","c)
    '            x = CInt(b(0))
    '            y = CInt(b(1))
    '            myPointCollection.Add(New Point(x, y))
    '        Next
    '        Return myPointCollection
    '    Catch ex As Exception
    '        nachricht("fehler in bildeMyPointCollection: " ,ex)
    '        Return Nothing
    '    End Try
    'End Function

    'Shared Function bildeMyPointCollectionMAPSHARE(ByVal coords As String) As PointCollection
    '    Dim punkte() As String
    '    Dim myPointCollection As New PointCollection
    '    Dim x, y As Integer
    '    If coords.IsNothingOrEmpty Then Return Nothing
    '    Try
    '        punkte = coords.Split(","c)
    '        For i = 0 To punkte.GetUpperBound(0) Step 2
    '            x = CInt(punkte(i))
    '            y = CInt(punkte(i + 1))
    '            myPointCollection.Add(New Point(x, y))
    '        Next
    '        x = CInt(punkte(0))
    '        y = CInt(punkte(0 + 1))
    '        myPointCollection.Add(New System.Windows.Point(x, y))
    '        Return myPointCollection
    '    Catch ex As Exception
    '        nachricht("fehler in bildeMyPointCollection: " ,ex)
    '        Return Nothing
    '    End Try
    'End Function


    'Public Shared Sub VisibilityDerKinderschalten(ByVal meinImage As Canvas, ByVal vis As Windows.Visibility)
    '    'For Each kind As UIElement In meinImage.Children
    '    '    kind.Visibility = vis
    '    'Next
    'End Sub

    'Public Shared Function MyPointVonCanvasNachGKumrechnen(ByVal ptA As Point) As myPoint
    '    Dim ptTemp, ptTemp2 As New myPoint
    '    ptTemp.X = ptA.X
    '    ptTemp.Y = ptA.Y
    '    ptTemp2 = clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(ptTemp, CLstart.myc.kartengen.aktMap.aktrange, CLstart.myc.kartengen.aktMap.aktcanvas)
    '    ptA.X = ptTemp2.X
    '    ptA.Y = ptTemp2.Y
    '    Return ptTemp2
    'End Function
    'Public Shared Function calc_length(ByVal ptColl As PointCollection) As Single
    '    'fläche berechnen
    '    Dim produkt As Double, summe As Double, i%
    '    Dim ysumme As Double, xsumme As Double
    '    Dim anzahl = ptColl.Count
    '    Dim ptA() As Point
    '    If ptColl Is Nothing Then
    '        nachricht("Fehler ptColl is nothing. Länge kann nicht berechnet werden!")
    '        Return 0
    '    End If
    '    Try
    '        ptA = ptColl.ToArray
    '        ReDim Preserve ptA(ptA.Length)
    '        ptA(ptA.Length - 1).X = ptA(0).X
    '        ptA(ptA.Length - 1).Y = ptA(0).Y
    '        For i = 0 To ptA.GetUpperBound(0)
    '            MyPointVonCanvasNachGKumrechnen(ptA(i))
    '        Next
    '        nachricht("#################### calc_length  eingang")
    '        '  calc_length = -1
    '        summe = 0
    '        If anzahl > 2 Then
    '            For i = 0 To ptA.GetUpperBound(0) - 1
    '                xsumme = ptA(i).X - ptA(i + 1).X
    '                ysumme = ptA(i).Y + ptA(i + 1).Y
    '                nachricht("## " & CStr(ptA(i).X & " " & ptA(i).Y))
    '                produkt = ysumme * xsumme / 2
    '                summe = summe + produkt
    '            Next i
    '            Return CSng(Math.Abs(summe))
    '        End If
    '        '    nachricht("#################### calc_length ausgang  " & CStr(calc_length))
    '        Return 0
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler in calc_length: " & Environment.NewLine ,ex)
    '        Return -1
    '    End Try
    'End Function

    'Public Shared Function calc_area(ByVal ptColl As PointCollection) As Single
    '    'fläche berechnen
    '    Dim produkt As Double, summe As Double, i%
    '    Dim ysumme As Double, xsumme As Double
    '    Dim anzahl = ptColl.Count
    '    Dim ptA() As Point
    '    If ptColl Is Nothing Then
    '        nachricht("Fehler ptColl is nothing. Fläche kann nicht berechnet werden!")
    '        Return 0
    '    End If
    '    Try
    '        ptA = ptColl.ToArray
    '        ReDim Preserve ptA(ptA.Length)
    '        ptA(ptA.Length - 1).X = ptA(0).X
    '        ptA(ptA.Length - 1).Y = ptA(0).Y
    '        For i = 0 To ptA.GetUpperBound(0)
    '            MyPointVonCanvasNachGKumrechnen(ptA(i))
    '        Next
    '        nachricht("#################### calc_area  eingang")
    '        '  calc_area = -1
    '        summe = 0
    '        If anzahl > 2 Then
    '            For i = 0 To ptA.GetUpperBound(0) - 1
    '                xsumme = ptA(i).X - ptA(i + 1).X
    '                ysumme = ptA(i).Y + ptA(i + 1).Y
    '                nachricht("## " & CStr(ptA(i).X & " " & ptA(i).Y))
    '                produkt = ysumme * xsumme / 2
    '                summe = summe + produkt
    '            Next i
    '            Return CSng(Math.Abs(summe))
    '        End If
    '        ' nachricht("#################### calc_area ausgang  " & CStr(calc_area))
    '        Return -1
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler in calc_area: " & Environment.NewLine ,ex)
    '        Return -2
    '    End Try
    'End Function

    'Shared Sub setzeVHgrund(ByVal thema As String)
    '    If String.IsNullOrEmpty(CLstart.myc.kartengen.aktMap.Vgrund) Then
    '        '  clstart.myc.kartengen.aktMap.Vgrund = myGlobalz.sitzung.VorgangsID & ";"
    '        If Not CLstart.myc.kartengen.aktMap.Vgrund.Contains("gemarkung_") Then
    '            CLstart.myc.kartengen.aktMap.Vgrund = CLstart.myc.kartengen.aktMap.Vgrund & ";gemarkung_;"
    '        End If
    '        If Not CLstart.myc.kartengen.aktMap.Vgrund.Contains("flure") Then
    '            CLstart.myc.kartengen.aktMap.Vgrund = CLstart.myc.kartengen.aktMap.Vgrund & ";flure;"
    '        End If
    '        CLstart.myc.kartengen.aktMap.Vgrund = CLstart.myc.kartengen.aktMap.Vgrund.Replace(";;", ";")
    '    End If


    '    CLstart.myc.kartengen.aktMap.Vgrund = CLstart.myc.kartengen.aktMap.Vgrund.Replace(";;", ";")

    '    CLstart.myc.kartengen.aktMap.Hgrund = thema

    'End Sub

    ''' <summary>
    ''' punkte nach GK ueberführen
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Shared Function PolygonAufbereiten(ByVal polygonchen As clsParapolygon) As Boolean 'myGlobalz.sitzung.aktPolygon
    '    If polygonchen.myPoly.Points.Count < 1 Then
    '        Return False
    '    End If
    '    'gkstring generieren
    '    Dim delim As String = ";"
    '    Dim gkstring As New Text.StringBuilder
    '    Dim dx, dy As Double
    '    For Each punkt As Point In polygonchen.myPoly.Points
    '        dx = CInt((clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(punkt).X) * 100) / 100
    '        dy = CInt((clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(punkt).Y) * 100) / 100
    '        gkstring.Append(CDbl(dx) & delim & CDbl(dy) & delim)
    '    Next
    '    'anfangspunk nochmal an Ende wiederholen
    '    dx = CInt((clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(polygonchen.myPoly.Points(0)).X) * 100) / 100
    '    dy = CInt((clsMiniMapTools.MyPointVonCanvasNachGKumrechnen(polygonchen.myPoly.Points(0)).Y) * 100) / 100
    '    gkstring.Append((dx) & delim & (dy) & delim)
    '    polygonchen.GKstring = gkstring.ToString
    '    Return True
    'End Function



    'Public Shared Sub GK_FlaecheErmitteln()
    '    Dim newPoints As New PointCollection
    '    Dim a As String() = myglobalz.sitzung.aktPolygon.GKstring.Split(";"c)
    '    For i = 0 To a.GetUpperBound(0) - 2 Step 2
    '        Dim np As New Point
    '        np.X = CDbl(a(i))
    '        np.Y = CDbl(a(i + 1))
    '        newPoints.Add(np)
    '    Next
    '    myglobalz.sitzung.aktPolygon.Area = clsMiniMapTools.calc_area(newPoints)
    '    newPoints = Nothing
    'End Sub

    'Public Shared Sub GK_StreckeErmitteln()
    '    Dim newPoints As New PointCollection
    '    Dim a As String() = myGlobalz.sitzung.aktPolyline.GKstring.Split(";"c)
    '    For i = 0 To a.GetUpperBound(0) - 2 Step 2
    '        Dim np As New Point
    '        np.X = CDbl(a(i))
    '        np.Y = CDbl(a(i + 1))
    '        newPoints.Add(np)
    '    Next
    '    'myGlobalz.sitzung.aktPolyline.Distanz = clsMiniMapTools.calc_length(newPoints)
    '    newPoints = Nothing
    'End Sub

    'Shared Function setMapCookie(ByVal aktmap As clsMapSpec, ByVal vid As Integer) As Boolean
    '    Dim mcookie As New DB_Minimap_ORACLE(clsDBspecOracle.getConnection(myglobalz.raumbezug_MYDB))
    '    mcookie.savemapcookie(aktmap, vid)
    '    mcookie.Dispose()
    '    Return True
    'End Function

    'Shared Function getMapCookie_alleDB(ByVal aktmap As clsMapSpec, ByVal vid As Integer) As Boolean
    '    Dim mcookie As New DB_Minimap_ORACLE(clsDBspecOracle.getConnection(myglobalz.raumbezug_MYDB))
    '    Dim istvorhanden As Boolean = mcookie.getmapcookie(aktmap, vid)
    '    mcookie.Dispose()
    '    If Not istvorhanden Then
    '        setzeAbteilungsThemen(aktmap, myglobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Bemerkung.Trim)
    '    End If
    '    Return True
    'End Function



    'Private Shared Sub elipsenHandleMousedown(ByVal sender As Object, ByVal e As MouseButtonEventArgs)
    '    DieseFuhnktionIstObsoleteSobaldJederDavonWeiss(sender)
    '    e.Handled = True
    'End Sub

    'Private Shared Function extrahiereFotoID(ByVal dateiname As String) As Integer
    '    Dim fotoid As Integer
    '    Dim a As String() = dateiname.Split("#"c)
    '    fotoid = CInt(a(1).Trim)
    '    Return fotoid
    'End Function

    'Private Shared Sub bildeAktDokumentAusFotoID(ByRef fotoid As Integer)
    '    myglobalz.sitzung.VorgangREC.mydb.SQL = "select * from foto2dokument where id=" & fotoid
    '    Dim hinweis As String = myglobalz.sitzung.VorgangREC.getDataDT()
    '    detailsTools.DTaufFotoObjektabbilden(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.VorgangREC.dt)
    '    Dim ausgabeVerzeichnis As String = ""
    '    myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
    'End Sub

    'Private Shared Sub elipsenHandle2MouseEnter(ByVal sender As Object, ByVal e As MouseEventArgs)
    '    DieseFuhnktionIstObsoleteSobaldJederDavonWeiss(sender)
    '    e.Handled = True
    'End Sub

    '    Private Shared Sub DieseFuhnktionIstObsoleteSobaldJederDavonWeiss(ByVal sender As Object)

    '        'Diese fuhnktion ist obsolete sobald jeder davon weiss
    '        Dim aaa As Ellipse = CType(sender, Ellipse)
    '        Dim dateiname As String, fotoid As Integer
    '        dateiname = aaa.ToolTip.ToString.ToLower
    '        If dateiname.Contains(".jpg") Then
    '            fotoid = extrahiereFotoID(dateiname)
    '            bildeAktDokumentAusFotoID(fotoid)
    '            Dim lfehler As String
    '#Disable Warning BC42030 ' Variable 'lfehler' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
    '            If DokArc.machCheckout(lfehler) Then
    '
    '                myGlobalz.sitzung.aktDokument.nurzumlesen = False
    '                DokArc.Archiv_aktiviere_dokument(myGlobalz.sitzung.aktDokument)
    '                'DokArc.zulisteAddieren()
    '            End If
    '        End If
    '    End Sub
    Public Shared Function isoliereCGIparameter(ByRef original As String) As String()
        Dim params As String()
        original = original.Replace("javascript:", "")
        '  If original.StartsWith("show_db") Then
        original = original.Replace("top.show_MYDB", "").Replace("top.show_db", "").Replace("show_MYDB", "").Replace("show_db", "")
        original = original.Replace("'", "").Replace("(", "").Replace(")", "")
        params = original.Split(","c)
        Return params
    End Function
    'Private Shared Sub dbabfrage(ByVal original As String)
    '    If String.IsNullOrEmpty(original) Then Exit Sub
    '    Dim params As String()
    '    Dim aufruf As String
    '    nachricht("dbabfrage-----------------")
    '    Try
    '        params = isoliereCGIparameter(original)
    '        aufruf = bildeAufruf(objektid:=params(0), aktive_ebene:=params(1), templatefile:=params(2))

    '        nachricht("original:" & original)
    '        nachricht("aufruf:" & aufruf)
    '        Process.Start(aufruf)
    '    Catch ex As Exception
    '        nachricht("dbabfrage-----------------original:" & original, ex)
    '    End Try
    'End Sub
    'Public Shared Function makeFSoutofFMGML(fs As String) As String
    '    Try
    '        ' fs="060743001001670001__"
    '        fs = "FS" & fs
    '        fs = fs.Replace("_", "0")
    '        Dim p1, p2 As String
    '        p1 = fs.Substring(0, 16)
    '        p2 = fs.Substring(18, fs.Length - 18)
    '        fs = p1 & "0" & p2
    '        Return fs
    '    Catch ex As Exception
    '        l("fehler in makeFSoutofFMGML: " ,ex)
    '        Return ""
    '    End Try
    'End Function

    'Public Shared Function dbabfrageTK5(ByVal original As String, zeitInMS As Integer) As String
    '    If String.IsNullOrEmpty(original) Then Return ""
    '    Dim params As String()
    '    Dim aufruf As String
    '    nachricht("dbabfrage-----------------")
    '    Try
    '        params = isoliereCGIparameter(original)
    '        Dim afst As New ParaFlurstueck
    '        afst.normflst.FS = params(0)
    '        If Not afst.normflst.FS.StartsWith("FS") Then
    '            afst.normflst.fsgml = params(0)
    '            afst.normflst.FS = makeFSoutofFMGML(afst.normflst.fsgml)
    '        End If

    '        afst.normflst.SetPartFromOldFS(afst.normflst.FS)

    '        If Not FST_tools.sindFlurstuecksDatenVollstaendig(afst.normflst) Then
    '            FST_tools.hole_FSTKoordinaten_undZuweisePunkt(afst)
    '        End If

    '        aufruf = bildeNASAufruf(afst.normflst, params(1), params(2))
    '        nachricht("original:" & original)
    '        nachricht("aufruf:" & aufruf)

    '        Dim antwort As String = CLstart.meineHttpNet.sendjobExtracted(aufruf, CLstart.mycSimple.enc, zeitInMS)

    '        antwort = Environment.NewLine & "Ergebnis: " & Environment.NewLine & antwort

    '        Return antwort
    '        '  tbinfore.text = antwort

    '        '  Process.Start(aufruf)
    '    Catch ex As Exception
    '        nachricht("dbabfrage-----------------original:" & original, ex)
    '        Return ""
    '    End Try
    'End Function

    'Public Shared Function bildeAufruf(ByVal objektid As String, aktive_ebene As String, templatefile As String) As String
    '    Dim modul, param As String
    '    modul = initP.getValue("GisServer.URLgetrecord")
    '    Try
    '        param = "?lookup=true"
    '        param = param & "&user=" + myglobalz.sitzung.aktBearbeiter.username + "&passwort=" & clsMiniMapTools.holePasswordFuerGisUser_dballe(myglobalz.sitzung.aktBearbeiter.username, myglobalz.sitzung.webgisREC)
    '        param = param & "&aktive_ebene=" + aktive_ebene
    '        param = param & "&object_id=" + objektid
    '        param = param & "&templatefile=" + templatefile
    '        param = param & "&activelayer=" + aktive_ebene
    '        'p = p & "&aktive_ebene=" + params(1)
    '        'p = p & "&object_id=" + params(0)
    '        'p = p & "&templatefile=" + params(2)
    '        'p = p & "&activelayer=" + params(1)
    '        param = param & "&apppfad=/profile/register/"
    '        nachricht("bildeAufruf: " & modul)
    '        nachricht("bildeAufruf: " & param)
    '        nachricht("bildeAufruf: " & modul & param)
    '        Return modul & param
    '    Catch ex As Exception
    '        nachricht("fehler in bildeAufruf: " ,ex)
    '        Return "fehler"
    '    End Try
    'End Function

    'Private Shared Function calcPixelFromMeterRadius(ByVal RadiusInMeter As Double, ByVal dierange As clsRange, ByVal kreiscanvas As clsCanvas) As Integer
    '    Dim test As Integer
    '    Try
    '        test = CInt((kreiscanvas.w / dierange.xdif) * RadiusInMeter)
    '        Return test
    '    Catch ex As Exception
    '        nachricht("fehler in calcPixelFromMeterRadius: " ,ex)
    '        Return -1
    '    End Try
    'End Function

    'Public Shared Sub elipsenHandle2MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
    '    e.Handled = True
    'End Sub

    'Private Shared Function bestimmeDezimalTrenner(ByVal gkstring As List(Of String)) As Char
    '    Try
    '        For Each sssstring As String In gkstring
    '            If sssstring.Contains(".") Then
    '                Return "."c
    '            End If
    '            If sssstring.Contains(",") Then
    '                Return ","c
    '            End If
    '        Next
    '        Return CChar(",") '???
    '    Catch ex As Exception
    '        nachricht("fehler inbestimmeDezimalTrenner:", ex)
    '        Return CChar(",")
    '    End Try
    'End Function

    'Private Shared Function koords2PointArray(ByVal dezimalTrenner As Char,
    '                                                ByVal nurKoordinaten As String(),
    '                                                rid As Integer
    '                                                ) As myPoint()
    '    Dim i As Integer = 0
    '    Dim j As Integer = 0
    '    Dim b() As String
    '    Dim meinpointer As Integer
    '    Dim myp As myPoint()
    '    Dim oben As Integer
    '    Try
    '        oben = CInt((nurKoordinaten.GetUpperBound(0) + 1))
    '        ReDim myp(CInt(oben / 2) - 1)
    '        meinpointer = 6
    '        For i = 0 To oben Step 2
    '            If i > nurKoordinaten.GetUpperBound(0) Then
    '                'wg vid: 24291
    '                Continue For
    '            End If
    '            If nurKoordinaten(i).IsNothingOrEmpty Then
    '                Continue For
    '            End If
    '            If Not nurKoordinaten(i).Contains(dezimalTrenner) Then
    '                nurKoordinaten(i) = nurKoordinaten(i) & dezimalTrenner & "0"
    '            End If
    '            If nurKoordinaten(i).Contains(dezimalTrenner) Then
    '                If j > myp.GetUpperBound(0) Then
    '                    Continue For
    '                End If
    '                myp(j) = New myPoint
    '                If nurKoordinaten(i).IsNothingOrEmpty Then
    '                    Continue For
    '                End If
    '                'integeranteil isolieren. warum nicht cint()? weil dezimalpunkt aknn unterschiedlich sein
    '                Dim k = CStr(CDbl(nurKoordinaten(i).Replace(".", ",")) * 100)
    '                ' dezimalTrenner=","c
    '                b = k.Split(","c)
    '                myp(j).X = CDbl(b(0))
    '                ' myp(j).X = CDbl(nurKoordinaten(i).Replace(".",","))
    '                If i + 1 > nurKoordinaten.Length - 1 Then
    '                    Debug.Print("")
    '                    k = CStr(CDbl(nurKoordinaten(1).Replace(".", ",")) * 100)
    '                Else
    '                    k = CStr(CDbl(nurKoordinaten(i + 1).Replace(".", ",")) * 100)
    '                End If

    '                b = k.Split(","c)
    '                myp(j).Y = CDbl(b(0))
    '                '  myp(j).Y = CDbl(nurKoordinaten(i).Replace(".",","))

    '                meinpointer = 8
    '                j = j + 1
    '            End If
    '        Next
    '        meinpointer = 9
    '        Return myp
    '    Catch ex As Exception
    '        nachricht("fehler in koords2PointArray: (rid: " & rid & ")" ,ex)
    '        Return Nothing
    '    End Try
    'End Function
    'Shared Sub addiereAktvorgang(ByRef am As clsMapSpec, ByVal vid As Integer)
    '    If String.IsNullOrEmpty(am.Vgrund) Then
    '        am.Vgrund = vid.ToString & ";"
    '        Exit Sub
    '    End If
    '    If Not am.Vgrund.ToLower.Contains(vid.ToString) Then
    '        am.Vgrund = am.Vgrund & ";" & vid.ToString & ";"
    '    End If
    'End Sub

    'Public Shared Function zerlegeInPunkte(gkstring As String,
    '                                       dezimalTrenner As Char,
    '                                       ByRef multipolygonpointer() As Integer,
    '                                       rid As Integer,
    '                                       ShapeSerialstringIstPostGis As Boolean) As myPoint()
    '    'RID dient nur der identifikation im logfile
    '    'Beispiel
    '    '484988,846153846;5545527,50167224;487244,418060201;5544824,94648829;487059,535117057;5543234,95317726;483620,712374582;5542717,28093645;482400,484949833;5544159,36789298;482548,391304348;5545231,68896321;484988,846153846;5545527,50167224;
    '    '47;58;64;472796.678000;5542353.315000;472791.709000;5542355.204000;472795.378000;5542359.972000;472801.676000;5542374.085000;472849.947000;5542448.337000;472939.739000;5542567.159000;473027.217000;5542674.248000;473102.301000;5542756.857000;473107.289000;5542759.246000;473117.015000;5542772.111000;473132.710000;5542777.299000;473134.449000;5542779.308000;473167.706000;5542779.518000;473201.094000;5542817.953000;473196.636000;5542850.900000;473195.297000;5542864.035000;473197.566000;5542866.244000;473269.197000;5542642.031000;473270.716000;5542637.273000;473373.825000;5542313.638000;473385.381000;5542277.367000;473386.705000;5542273.210000;473408.418000;5542206.384000;473497.597000;5541924.431000;473492.989000;5541926.210000;473348.268000;5541977.377000;473207.394000;5542026.072000;473062.997000;5542076.820000;472977.061000;5542106.505000;472975.502000;5542102.185000;472973.733000;5542102.794000;472923.676000;5542120.950000;472913.049000;5542125.423000;472902.942000;5542130.965000;472893.406000;5542137.426000;472884.498000;5542144.747000;472875.722000;5542152.188000;472840.559000;5542189.371000;472815.074000;5542214.749000;472810.176000;5542220.307000;472786.808000;5542244.230000;472787.378000;5542244.760000;472781.106000;5542251.116000;472917.111000;5542252.473000;472937.110000;5542300.013000;472901.193000;5542313.781000;472796.678000;5542353.315000;472824.500000;5542206.780000;472823.750000;5542225.153000;472872.653000;5542227.181000;472871.902000;5542245.153000;472824.706000;5542243.202000;472824.585000;5542246.201000;472817.589000;5542245.911000;472817.720000;5542242.914000;472790.267000;5542241.772000;472810.726000;5542220.846000;472824.500000;5542206.780000;473399.952000;5542213.451000;473381.527000;5542271.440000;473239.496000;5542227.099000;473402.241000;5542205.924000;473401.191000;5542209.403000;473399.952000;5542213.451000;473380.008000;5542275.430000;473369.282000;5542309.767000;473140.801000;5542324.485000;473112.115000;5542246.073000;473024.381000;5542230.430000;473042.291000;5542169.467000;473229.809000;5542228.359000;473380.008000;5542275.430000;
    '    'hat 71 points

    '    Dim istart As Integer = 0

    '    Dim a(), nurKoordinaten() As String
    '    ' Dim multipolygonpointer() As Integer
    '    Dim myp() As myPoint = Nothing

    '    Dim meinpointer As Integer
    '    Dim errorout As String = "errorout"
    '    Try
    '        If String.IsNullOrEmpty(gkstring) Then
    '            nachricht("Fehler: gkstring ist leer!!!")
    '            Return Nothing
    '        End If
    '        errorout = errorout & ", gkstring: " & gkstring
    '        meinpointer = 2
    '        a = gkstring.Split(";"c)
    '        If ShapeSerialstringIstPostGis Then
    '            istart = 0
    '        Else
    '            istart = getKoordinatenstart(gkstring, dezimalTrenner)
    '            If istart < 0 Then
    '                Return Nothing
    '            End If
    '        End If

    '        leereFelderAbschneiden(a)
    '        'If rid= 26159 Then
    '        '    Debug.Print("")
    '        'End If
    '        nurKoordinaten = bildeNurKoordinatenArray(a, istart)
    '        multipolygonpointer = bildeTeilFlaechenPointer(a, istart)
    '        myp = koords2PointArray(dezimalTrenner, nurKoordinaten, rid)
    '        Return myp
    '    Catch ex As Exception
    '        nachricht("Fehler in zerlegeInPunkte (" & rid & "):" & " meinpointer: " & meinpointer & ": " & gkstring ,ex)
    '        Return myp
    '    End Try
    'End Function

    'Public Shared Function polygonNachCanvasUmrechnen(punktarrayGK As myPoint(),
    '                                                   ByVal birdsrange As clsRange,
    '                                                   ByVal Kreiscanvas As clsCanvas) As myPoint()
    '    Try
    '        Dim punkteCanvas(punktarrayGK.GetUpperBound(0)) As myPoint
    '        For i = 0 To punktarrayGK.GetUpperBound(0)
    '            If IsNothing(punktarrayGK(i)) Then
    '                Debug.Print("")
    '                Continue For
    '            End If
    '            punkteCanvas(i) = New myPoint
    '            punkteCanvas(i) = punktvonGKnachCanvasUmrechnen(punktarrayGK(i), birdsrange, Kreiscanvas)
    '            '   RandKorrektur(Kreiscanvas, punkteCanvas, i)
    '        Next
    '        Return punkteCanvas
    '    Catch ex As Exception
    '        nachricht("Fehler in polygonNachCanvasUmrechnen:" ,ex)
    '        Return Nothing
    '    End Try
    'End Function


    'Private Shared Sub drawPolygon2Canvas(ByVal href As String,
    '                                ByVal title As String,
    '                                ByVal zaehler As Integer,
    '                                ByVal myPointCollection As PointCollection,
    '                                ByVal canvas1 As Canvas,
    '                                zindex As Integer,
    '                                tooltip As String,
    '                                ByVal myFillColorBrush As SolidColorBrush,
    '                                ByVal myStrokeColorBrush As SolidColorBrush)
    '    'withevents muss auf klassenebene deklariert sein   Private WithEvents myPolygon As Polygon
    '    Dim myPolygon As New Polygon
    '    Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 100, 250)) 'transparenz ist der erste wert
    '    Try
    '        myPolygon.Name = "poly" & zaehler
    '        myPolygon.ToolTip = tooltip
    '        myPolygon.Tag = href
    '        myPolygon.Stroke = myStrokeColorBrush ' Brushes.DarkBlue
    '        myPolygon.StrokeThickness = 3
    '        myPolygon.Fill = myBrush
    '        myPolygon.Opacity = 90
    '        myPolygon.StrokeThickness = 1
    '        myPolygon.Cursor = Cursors.Hand
    '        myPolygon.Points = myPointCollection.Clone
    '        If href.Contains("javascript") Then
    '            AddHandler myPolygon.MouseDown, AddressOf Polygon_MouseDownFS
    '        Else
    '            ' AddHandler myPolygon.MouseDown, AddressOf Polygon_MouseDownPolygonRID
    '        End If

    '        canvas1.Children.Add(myPolygon)
    '        Canvas.SetZIndex(myPolygon, zindex)
    '        Canvas.SetLeft(myPolygon, 0)
    '        Canvas.SetTop(myPolygon, 0)


    '    Catch ex As Exception
    '        l("drawPolygon2Canvas" ,ex)
    '    End Try
    'End Sub

    'Private Shared Function enthaelt_nochDieAnzahlDerKoordinaten(a As String(), dezimalTrenner As String) As Boolean
    '    If a(0).ToString.Contains(".") Or a(0).ToString.Contains(",") Then
    '        Return False
    '    Else
    '        Return True
    '    End If
    'End Function

    'Private Shared Function getKoordinatenstart(gkstring As String, dezimalTrenner As Char) As Integer
    '    Try
    '        Dim a As String() = gkstring.Split(";"c)
    '        For i = 0 To a.GetUpperBound(0)
    '            If a(i).Contains(dezimalTrenner) Then
    '                Return i
    '            End If
    '        Next
    '        Return -1
    '    Catch ex As Exception
    '        nachricht("Fehler in getKoordinatenstart: " ,ex)
    '        Return -1
    '    End Try
    'End Function

    'Private Shared Sub drawPolyline2Canvas(ByVal href As String,
    '                                  ByVal title As String,
    '                                  ByVal zaehler As Integer,
    '                                  ByVal myPointCollection As PointCollection,
    '                                  ByVal canvas1 As Canvas,
    '                                  zindex As Integer,
    '                                    ByVal myFillColorBrush As SolidColorBrush,
    '                                    ByVal myStrokeColorBrush As SolidColorBrush)
    '    'withevents muss auf klassenebene deklariert sein   Private WithEvents myPolygon As Polygon
    '    Dim myPolygon As New Polyline
    '    '  Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 100, 250)) 'transparenz ist der erste wert
    '    Try
    '        myPolygon.Name = "poly" & zaehler
    '        myPolygon.ToolTip = href
    '        myPolygon.Tag = href
    '        myPolygon.Stroke = myStrokeColorBrush 'Brushes.DarkBlue
    '        '  myPolygon.Fill = myBrush
    '        myPolygon.Opacity = 90
    '        myPolygon.StrokeThickness = 4
    '        '    myPolygon.Cursor = Cursors.Hand
    '        myPolygon.Points = myPointCollection.Clone
    '        '  AddHandler myPolygon.MouseDown, AddressOf Polygon_MouseDown
    '        canvas1.Children.Add(myPolygon)
    '        Canvas.SetZIndex(myPolygon, zindex)
    '        Canvas.SetLeft(myPolygon, 0)
    '        Canvas.SetTop(myPolygon, 0)
    '    Catch ex As Exception
    '        l("drawPolyline2Canvas" ,ex)
    '    End Try
    'End Sub

    'Private Shared Sub leereFelderAbschneiden(ByRef neu As String())
    '    Try
    '        For i = neu.GetUpperBound(0) To 0 Step -1
    '            If neu(i).IsNothingOrEmpty Then
    '                ReDim Preserve neu(i - 1)
    '            End If
    '        Next
    '    Catch ex As Exception
    '        nachricht("Fehler in leereFelderAbschneiden_:" ,ex)
    '    End Try
    'End Sub
    'Private Shared Function bildeNurKoordinatenArray(a As String(), istart As Integer) As String()
    '    Dim neu As String()
    '    Try
    '        ReDim neu(a.Length - 1)
    '        If istart = 0 Then
    '            Return a
    '        Else
    '            Array.Copy(a, istart, neu, 0, (a.GetUpperBound(0) - (istart - 1)))
    '            leereFelderAbschneiden(neu)
    '            Return neu
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in bildeNurKoordinatenArray:" ,ex)
    '        Return Nothing
    '    End Try
    'End Function

    'Private Shared Function bildeTeilFlaechenPointer(a As String(), istart As Integer) As Integer()
    '    Dim neu As Integer()
    '    Try
    '        ReDim neu(a.Length - 1)
    '        ' Array.Copy(a, istart, neu, 0, 2)
    '        For i = 0 To istart - 1
    '            neu(i) = CInt(a(i))
    '        Next
    '        ReDim Preserve neu(istart - 1)
    '        ' leereFelderAbschneiden(neu)
    '        Return neu
    '    Catch ex As Exception
    '        nachricht("Fehler in bildeTeilFlaechenPointer:" ,ex)
    '        Return Nothing
    '    End Try
    'End Function

    'Shared Function imageMap2Eigentuemermap(ByRef Mapdatei As String) As String
    '    Return convertImagamap2Eigentuermap(Mapdatei)
    'End Function


    'Private Shared Sub NeuesImageFileKonstruieren(sb As System.Text.StringBuilder, shape As String, title As String, coords As String, href As String)
    '    sb.Append("<area " & shape & " title=""" & title & """" & " coords=""" & coords & """" &
    '              " href=""" & href & """>" & Environment.NewLine)
    'End Sub

    'Private Shared Function getFSfromHref(href As String) As String
    '    Dim params() As String
    '    Try
    '        params = isoliereCGIparameter(href)
    '        Return params(0)


    '    Catch ex As Exception
    '        nachricht("Fehler in getFSfromHref:", ex)
    '        Return ""
    '    End Try
    'End Function


    'Private Shared Function makeFS4Flurstuecke(sekid As Integer, typ As Integer, tag As String, ByRef tooltip As String, eigentuemerFunktionAktiv As Boolean) As String
    '    If typ = RaumbezugsTyp.Flurstueck Then
    '        Dim ttag As String = "javascript:top.show_MYDB('ÄÄÄ','tk5','MSKfstueck.htm','-3,-3,65,20')"
    '        Dim fs As String = getFS4sekid(sekid)
    '        ttag = ttag.Replace("ÄÄÄ", fs)
    '        If eigentuemerFunktionAktiv Then
    '            tooltip = getEigentuemerTitle4FS(fs, "select tooltip from fs2eigentuemer where fs='")
    '        Else
    '            tooltip = "Eigentümer ? Klick!"
    '        End If
    '        Return ttag
    '    End If
    '    Return tag
    'End Function

    'Private Shared Function getFS4sekid(sekid As Integer) As String
    '    Try
    '        myglobalz.sitzung.tempREC2.mydb.SQL = "select fs from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK   & "  where id=" & sekid
    '        Dim hinweis = myglobalz.sitzung.tempREC2.getDataDT()
    '        If myglobalz.sitzung.tempREC2.dt.Rows.Count < 1 Then
    '            Return ""
    '        Else
    '            Return clsDBtools.fieldvalue(myglobalz.sitzung.tempREC2.dt.Rows(0).Item("FS"))
    '        End If
    '    Catch ex As Exception
    '        nachricht("fehler in getFS4sekid: ", ex)
    '        Return ""
    '    End Try
    'End Function
    'Private Shared Function getFreitext4sekid(rid As Integer) As String
    '    Try
    '        myglobalz.sitzung.tempREC2.mydb.SQL = "select freitext from " & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  where raumbezugsid=" & rid
    '        Dim hinweis = myglobalz.sitzung.tempREC2.getDataDT()
    '        If myglobalz.sitzung.tempREC2.dt.Rows.Count < 1 Then
    '            Return ""
    '        Else
    '            Return clsDBtools.fieldvalue(myglobalz.sitzung.tempREC2.dt.Rows(0).Item(0))
    '        End If
    '    Catch ex As Exception
    '        nachricht("fehler in getFreitext4sekid: ", ex)
    '        Return ""
    '    End Try
    'End Function

    'Private Shared Function GetTagValueTitle(line As String) As String
    '    Dim tag = "title="
    '    Dim rest As String, pos As Integer
    '    ' nachricht("GetTagValueTitle --------  ")
    '    Try
    '        If line.IsNothingOrEmpty Then Return ""
    '        '       nachricht("GetTagValueTitle -------- eingabe ist gültig")
    '        If Not line.ToLower.Contains(tag.ToLower) Then Return ""
    '        'typen coord und title und href
    '        ' nachricht("GetTagValueTitle -------- eingabe ist gültig")
    '        pos = line.IndexOf(tag)
    '        Dim titeltrenner As String
    '        If pos > 0 Then
    '            pos = pos + tag.Length + 1
    '            rest = line.Substring(pos, line.Length - pos)
    '            If rest.StartsWith("'") Then
    '                titeltrenner = "'"
    '            Else
    '                titeltrenner = """"
    '            End If
    '            pos = rest.IndexOf(titeltrenner)
    '            If pos < 0 Then
    '                ' weil href am ende liegt gibts kein blank
    '                pos = rest.Length - 2
    '            End If
    '            rest = rest.Substring(0, pos)
    '            rest = rest.Replace(Chr(34), "").Replace("'", "")
    '        Else
    '            Return ""
    '        End If
    '        Return rest
    '    Catch ex As Exception
    '        nachricht("fehler in GetTagValue: " ,ex)
    '        Return ""
    '    End Try
    'End Function

    'Private Shared Function makeTag4Polygone(sekid As Integer, typ As Integer, ByRef tooltip As String) As String
    '    If typ = RaumbezugsTyp.Polygon Then
    '        Dim ttag As String = "rid=" & sekid
    '        tooltip = getFreitext4sekid(sekid)
    '        Return ttag
    '    End If
    '    Return ""
    'End Function

    'Shared Sub boundingboxComplettNeuErmitteln()
    '    Dim erfolg As Boolean = RBtoolsns.initraumbezugsDT_alleDB.exe(CInt(myglobalz.sitzung.aktVorgangsID))
    '    ''alles liegt auf myGlobalz.sitzung.raumbezugsRec.mydb                
    '    RBtoolsns.berechneRaumbezugsrange.execute(CLstart.myc.raumberange, myglobalz.sitzung.raumbezugsRec.dt)
    '    CLstart.myc.kartengen.aktMap.aktrange.rangekopierenVon(CLstart.myc.raumberange)
    '    erfolg = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myglobalz.sitzung.aktVorgangsID)
    'End Sub

    'Private Shared Function fitRbistbesserAlsAktrange() As Boolean
    '    Return (CLstart.myc.kartengen.FitRaumbezuege.xdif > 1 And CLstart.myc.kartengen.FitRaumbezuege.xdif < CLstart.myc.kartengen.aktMap.aktrange.xdif)
    'End Function



    'Private Shared Function getfsfromLine(line As String) As String
    '    Dim pos As Integer
    '    Dim fs As String
    '    Try
    '        pos = line.IndexOf("show_MYDB(")
    '        pos = pos + 10
    '        fs = line.Substring(pos, 22)
    '        fs = fs.Replace("'", "")
    '        Return fs
    '    Catch ex As Exception
    '        Return "o.A."
    '    End Try

    'End Function

    'Private Shared Function getEigentuemerTitle4FS(fs As String, selectStatement As String) As String
    '    Dim tooltipp As String = ""
    '    Dim sql As String = selectStatement & fs & "'"
    '    '  Dim myconn As OracleConnection

    '    Try
    '        'Dim csb As New OracleConnectionStringBuilder

    '        'klassisch
    '        'csb.Server = "ora-clu-vip-004" 'mydb.Host
    '        'csb.ServiceName = "gis.kreis-of.local"
    '        'csb.Port = 1521
    '        'csb.UserId = "gis"
    '        'csb.Password = "A604l6rrpn"
    '        'csb.Pooling = False
    '        'myconn = New OracleConnection(csb.ConnectionString)


    '        Dim myconn As OracleConnection = New OracleConnection("Data Source=(DESCRIPTION=" &
    '    "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-003)(PORT=1521)))" &
    '     "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" &
    '    "(LOAD_BALANCE=yes) (CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & "gis.kreis-of.local" & ")));" &
    '    "User Id=" & "gis" & ";Password=" & "A604l6rrpn" & ";DIRECT=yes;")
    '        myconn.Unicode = True
    '        myconn.Open()
    '        Dim com As New OracleCommand
    '        com = New OracleCommand

    '        com.Connection = myconn
    '        com.CommandText = sql
    '        com.CommandType = CommandType.Text
    '        'Dim p_theid As New OracleParameter

    '        tooltipp = CStr(com.ExecuteScalar())
    '        com.Dispose()
    '        myconn.Dispose()

    '        Return tooltipp
    '    Catch ex As Exception
    '        nachricht(ex.ToString)
    '        Return "error"
    '    End Try
    '    Return tooltipp
    'End Function




    'Public Shared Function imageMap2PolygonMap(ByVal Mapdatei As String,
    '                                           ByVal canvas1 As Canvas,
    '                                           eigentuemerFunktionAktiv As Boolean,
    '                                           useMapserverMapmode As Boolean) As Boolean
    '    ' malt ohne skalierung
    '    Dim line As String
    '    Try
    '        Dim test As New IO.FileInfo(Mapdatei)
    '        If Not test.Exists Then
    '            test = Nothing
    '            Return False
    '        End If
    '        test = Nothing
    '        Using sr As New IO.StreamReader(Mapdatei, CLstart.mycSimple.enc)
    '            Dim coords As String
    '            Dim shape As String
    '            Dim href As String
    '            Dim title As String
    '            Dim zaehler As Integer = 0
    '            Dim eigentuemerTitle As String = ""
    '            Dim myPointCollection As PointCollection
    '            'Dim eigentuemerFunktionAktiv As Boolean = True
    '            Do
    '                zaehler += 1
    '                line = sr.ReadLine
    '                If line.IsNothingOrEmpty Then Exit Do
    '                shape = GetTagValue(line, "shape=") : If shape.IsNothingOrEmpty Then Continue Do
    '                coords = GetTagValue(line, "coords=") : If coords.IsNothingOrEmpty Then Continue Do
    '                ' title = GetTagValueTitle(line) 'mapserver
    '                If useMapserverMapmode Then
    '                    '   title = GetTagValue(line, "title=")
    '                    title = GetTagValueTitle(line) 'mapserver
    '                Else
    '                    title = GetTagValue(line, "title=")
    '                End If

    '                href = GetTagValue(line, "href=")
    '                If eigentuemerFunktionAktiv Then
    '                    Dim fs As String
    '                    fs = getfsfromLine(line)
    '                    title = getEigentuemerTitle4FS(fs, "select tooltip from fs2eigentuemer where fsgml='")

    '                End If

    '                If shape = "point" Then
    '                    coords = GetQuadrat(coords)
    '                End If
    '                If useMapserverMapmode Then
    '                    myPointCollection = bildeMyPointCollectionMapserver(coords)
    '                Else
    '                    myPointCollection = bildeMyPointCollectionMAPSHARE(coords)
    '                End If

    '                If Not IsNothing(myPointCollection) Then
    '                    polygonmalen(href, title, zaehler, myPointCollection, canvas1)
    '                End If
    '            Loop
    '        End Using
    '        Return True
    '    Catch ex As Exception
    '        nachricht("Fehler in imageMap2PolygonMap: " ,ex)
    '        Return False
    '    End Try
    'End Function
    'Public Shared Function imageMap2PolygonMapMAPSERVER(ByVal imageMap As String,
    '                                           ByVal canvas1 As Canvas,
    '                                           eigentuemerFunktionAktiv As Boolean) As Boolean
    '    ' malt ohne skalierung
    '    Dim line As String
    '    Dim dauer As Integer = 500
    '    Dim coundauer As Integer = 1
    '    Try
    '        Dim a() As String
    '        a = imageMap.Split(CType(vbCr, Char()))
    '        Dim coords As String
    '        Dim shape As String
    '        Dim href As String
    '        Dim title As String
    '        Dim zaehler As Integer = 0
    '        Dim eigentuemerTitle As String = ""
    '        Dim myPointCollection As PointCollection
    '        Dim textMarker As String
    '        If a(0).Contains("'") Then textMarker = "'"
    '        If a(0).Contains(Chr(34)) Then textMarker = Chr(34)
    '        textMarker = Chr(34)
    '        'Dim eigentuemerFunktionAktiv As Boolean = True
    '        For i = 0 To a.GetUpperBound(0)
    '            zaehler += 1
    '            line = a(i)
    '            If line.IsNothingOrEmpty Then Exit For
    '            shape = GetTagValue(line, "shape=") : If shape.IsNothingOrEmpty Then Continue For
    '            coords = GetTagValue(line, "coords=") : If coords.IsNothingOrEmpty Then Continue For
    '            title = GetTagValueTitle(line)
    '            href = GetTagValue(line, "href=")
    '            If eigentuemerFunktionAktiv Then
    '                Dim fs As String
    '                fs = getfsfromLine(line)
    '                title = getEigentuemerTitle4FS(fs, "select tooltip from fs2eigentuemer where fsgml='")
    '            End If

    '            If shape = "point" Then
    '                coords = GetQuadrat(coords)
    '            End If

    '            myPointCollection = bildeMyPointCollection(coords)
    '            If Not IsNothing(myPointCollection) Then
    '                polygonmalen(href, title, zaehler, myPointCollection, canvas1)
    '            End If
    '        Next
    '        'End Using
    '        Return True
    '    Catch ex As Exception
    '        nachricht("Fehler in imageMap2PolygonMap: " ,ex)
    '        Return False
    '    End Try
    'End Function
    'Private Shared Sub polygonmalen(ByVal href As String,
    '                            ByVal title As String,
    '                            ByVal zaehler As Integer,
    '                            ByVal myPointCollection As PointCollection,
    '                            ByVal canvas1 As Canvas)
    '    'withevents muss auf klassenebene deklariert sein   Private WithEvents myPolygon As Polygon
    '    Dim myPolygon As New Polygon
    '    Try
    '        If IsNothing(myPointCollection) Then
    '            nachricht("warnung in polygonmalen: myPointCollection  ist nothing")
    '        End If
    '        '  myPolygon.Name = "poly" & zaehler
    '        myPolygon.ToolTip = "Klick mich, " & Environment.NewLine & title
    '        myPolygon.Tag = href
    '        myPolygon.Stroke = Brushes.Black
    '        myPolygon.Fill = Brushes.Transparent
    '        myPolygon.StrokeThickness = 0
    '        myPolygon.Cursor = Cursors.Hand
    '        myPolygon.ForceCursor = True

    '        myPolygon.Points = myPointCollection

    '        AddHandler myPolygon.MouseDown, AddressOf Polygon_MouseDown
    '        canvas1.Children.Add(myPolygon)
    '        Canvas.SetZIndex(myPolygon, 1000)
    '        Canvas.SetLeft(myPolygon, 0)
    '        Canvas.SetTop(myPolygon, 0)
    '        myPolygon.Cursor = Cursors.ArrowCD
    '    Catch ex As Exception
    '        nachricht("fehler in polygonmalen: " ,ex)
    '    End Try
    'End Sub
End Class
