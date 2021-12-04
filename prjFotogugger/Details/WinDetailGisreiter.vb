Imports System.Data
Imports System.IO
Partial Public Class Window_Detail
    'Private Sub chkBoxPan2_Click(sender As Object, e As RoutedEventArgs)
    '    If Not formWindetailIsLoaded Then Exit Sub
    '    eigentuemerfunktionAusschalten()
    '    If chkBoxPan2.IsChecked Then
    '        zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '        gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '        CanvasClickModus = "pan"
    '        Mouse.OverrideCursor = Cursors.Hand
    '    End If
    '    If Not chkBoxPan2.IsChecked Then
    '        zeichneOverlaysGlob = True : zeichneImageMapGlob = True
    '        Mouse.OverrideCursor = Cursors.Arrow
    '        gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '        CanvasClickModus = ""
    '        Mouse.OverrideCursor = Cursors.Arrow
    '    End If

    '    e.Handled = True
    'End Sub

    'Private Sub cmbGISHintergrund_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    Try
    '        Dim item As String
    '        If chkBIGGIS.IsChecked Then
    '            If cmbGISHintergrund2.SelectedValue Is Nothing Then Exit Sub
    '            item = CType(cmbGISHintergrund2.SelectedValue, String)
    '        Else
    '            If cmbGISHintergrund.SelectedValue Is Nothing Then Exit Sub
    '            item = CType(cmbGISHintergrund.SelectedValue, String)
    '        End If

    '        If String.IsNullOrEmpty(item) Then Exit Sub
    '        nachricht("USERAKTION: hintergrund ändern ")
    '        Dim a$() = item.Split("#"c)
    '        panningAusschalten()
    '        If a(0) = "nullover" Then
    '            CLstart.myc.kartengen.aktMap.Hgrund = "" & ";"
    '            tbHGRUND2.Text = "Kein Hintergrund"
    '            tbHGRUND.Text = "Kein Hintergrund"
    '        Else
    '            tbHGRUND2.Text = a(1)
    '            tbHGRUND.Text = a(1)
    '            CLstart.myc.kartengen.aktMap.Hgrund = a(0) & ";"
    '        End If
    '        If a(0) = "tk5" Then
    '            CLstart.myc.kartengen.aktMap.ActiveLayer = a(0)
    '            CLstart.myc.kartengen.aktMap.ActiveLayerTitel = "Flurkarte aktuell"
    '        End If
    '        CLstart.myc.kartengen.aktMap.Hgrund = CLstart.myc.kartengen.aktMap.Hgrund.Replace("nullover", "")
    '        CLstart.myc.kartengen.aktMap.Vgrund = detailsTools.getNewVgrundString(CLstart.myc.kartengen.aktMap.Vgrund)
    '        myglobalz.availablePresentationObjects.Clear()
    '        presentMapOLD()
    '        e.Handled = True
    '    Catch ex As Exception
    '        nachricht("cmbGISHintergrund_SelectionChanged" ,ex)
    '    End Try
    'End Sub
    'Private Sub cmbkartenbreiteinmeter_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    '    Dim itemcb As System.Windows.Controls.ComboBoxItem
    '    If chkBIGGIS IsNot Nothing AndAlso chkBIGGIS.IsChecked Then
    '        If cmbKartenbreiteinMeterchen Is Nothing Then Exit Sub
    '        If cmbKartenbreiteinMeterchen.SelectedIndex < 0 Then Exit Sub
    '        itemcb = CType(cmbKartenbreiteinMeterchen.SelectedItem, System.Windows.Controls.ComboBoxItem)
    '    Else
    '        If cmbkartenbreiteinmeter Is Nothing Then Exit Sub
    '        If cmbkartenbreiteinmeter.SelectedIndex < 0 Then Exit Sub
    '        itemcb = CType(cmbkartenbreiteinmeter.SelectedItem, System.Windows.Controls.ComboBoxItem)
    '    End If

    '    If itemcb.Content Is Nothing Then Exit Sub
    '    panningAusschalten()
    '    Dim item As String = itemcb.Content.ToString
    '    Select Case item
    '        Case "k"
    '            Exit Sub
    '        Case Else
    '            If IsNumeric(item) Then
    '                setzeKartenbreiteAuf(item)
    '            Else
    '                Exit Sub
    '            End If
    '    End Select
    '    cmbKartenbreiteinMeterchen.SelectedIndex = 0
    '    cmbkartenbreiteinmeter.SelectedIndex = 0
    '    e.Handled = True
    'End Sub
    'Private Sub btnThemenauswahl(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    nachricht("USERAKTION: themen ändern ")
    '    eigentuemerfunktionAusschalten()
    '    panningAusschalten()
    '    starteThemenauswahl()
    '    e.Handled = True
    'End Sub

    'Private Sub btnZumbiggis(sender As Object, e As RoutedEventArgs)
    '    chkBIGGIS.IsChecked = True
    '    tiMap.IsSelected = True
    '    e.Handled = True
    'End Sub

    'Private Sub cmbSuchen_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    '    Dim itemcb As System.Windows.Controls.ComboBoxItem
    '    If chkBIGGIS IsNot Nothing AndAlso chkBIGGIS.IsChecked Then
    '        If cmbSuchen Is Nothing Then Exit Sub
    '        If cmbSuchen.SelectedIndex < 0 Then Exit Sub
    '        itemcb = CType(cmbSuchen.SelectedItem, System.Windows.Controls.ComboBoxItem)
    '    Else
    '        If cmbSuchen Is Nothing Then Exit Sub
    '        If cmbSuchen.SelectedIndex < 0 Then Exit Sub
    '        itemcb = CType(cmbSuchen.SelectedItem, System.Windows.Controls.ComboBoxItem)
    '    End If
    '    If itemcb.Content Is Nothing Then Exit Sub
    '    Dim item As String = itemcb.Content.ToString
    '    Select Case item.ToLower
    '        Case "suche"
    '        Case "flurstück"
    '            btnFstsucheExtracted()
    '            eigentuemerfunktionAusschalten()
    '        Case "adresse"
    '            btnAdrSucheExtracted()
    '            eigentuemerfunktionAusschalten()
    '        Case Else
    '    End Select
    '    e.Handled = True
    'End Sub

    'Private Sub eigentuemerfunktionAusschalten()
    '    ckbeigentuemerFunktion.IsChecked = False
    'End Sub

    'Private Sub globalfit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    panningAusschalten()
    '    eigentuemerfunktionAusschalten()
    '    CLstart.myc.kartengen.aktMap.aktrange.rangekopierenVon(CLstart.myc.kartengen.FitGlobal)
    '    setBoundingRefresh(CLstart.myc.kartengen.aktMap.aktrange)
    '    'Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myGlobalz.sitzung.aktVorgangsID)
    '    e.Handled = True
    'End Sub


    'Private Sub zoomin_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    'neue range berechnen
    '    'darstellen

    '    panningAusschalten()
    '    Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
    '    CLstart.myc.kartengen.aktMap.aktrange.xl = CLstart.myc.kartengen.aktMap.aktrange.xl + (breite / 3)
    '    CLstart.myc.kartengen.aktMap.aktrange.xh = CLstart.myc.kartengen.aktMap.aktrange.xh - (breite / 3)
    '    Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()
    '    CLstart.myc.kartengen.aktMap.aktrange.yl = CLstart.myc.kartengen.aktMap.aktrange.yl + (hohe / 3)
    '    CLstart.myc.kartengen.aktMap.aktrange.yh = CLstart.myc.kartengen.aktMap.aktrange.yh - (hohe / 3)
    '    presentMapOLD()
    '    'Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myGlobalz.sitzung.aktVorgangsID)
    '    e.Handled = True
    'End Sub

    'Private Sub zoomout_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    panningAusschalten()
    '    eigentuemerfunktionAusschalten()
    '    Dim breite As Double = CLstart.myc.kartengen.aktMap.aktrange.xdif()
    '    CLstart.myc.kartengen.aktMap.aktrange.xl = CLstart.myc.kartengen.aktMap.aktrange.xl - (breite / 3)
    '    CLstart.myc.kartengen.aktMap.aktrange.xh = CLstart.myc.kartengen.aktMap.aktrange.xh + (breite / 3)
    '    Dim hohe As Double = CLstart.myc.kartengen.aktMap.aktrange.ydif()
    '    CLstart.myc.kartengen.aktMap.aktrange.yl = CLstart.myc.kartengen.aktMap.aktrange.yl - (hohe / 3)
    '    CLstart.myc.kartengen.aktMap.aktrange.yh = CLstart.myc.kartengen.aktMap.aktrange.yh + (hohe / 3)
    '    presentMapOLD()
    '    'Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myGlobalz.sitzung.aktVorgangsID)
    '    e.Handled = True
    'End Sub

    'Private Sub btnAuschnitt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '    panningAusschalten()
    '    gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))

    '    Dim test As String = CLstart.myc.userIniProfile.WertLesen("Minimap", "Ausschnitt_info")
    '    If String.IsNullOrEmpty(test) OrElse test = "1" Then
    '        MsgBox("Wählen sie den Ausschnitt in der Karte (Maustaste drücken und ziehen, loslassen)")
    '    End If
    '    CLstart.myc.userIniProfile.WertSchreiben("Minimap", "Ausschnitt_info", "0")
    '    CanvasClickModus = "Ausschnitt"
    '    If chkBIGGIS.IsChecked Then
    '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Collapsed)
    '        DrawRectangle(myCanvas2)
    '    Else
    '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
    '        DrawRectangle(myCanvas)
    '    End If
    '    'Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myGlobalz.sitzung.aktVorgangsID)
    '    e.Handled = True
    'End Sub

    'Private Sub rbfit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    panningAusschalten()
    '    eigentuemerfunktionAusschalten()
    '    clsMiniMapTools.boundingboxComplettNeuErmitteln()
    '    Dim erfolg As Boolean = clsMiniMapTools.setMapCookie(CLstart.myc.kartengen.aktMap, myglobalz.sitzung.aktVorgangsID)
    '    INITMiniMapPresentation(myglobalz.sitzung.raumbezugsRec.dt, False)
    '    e.Handled = True
    'End Sub

    'Sub setzeVordergrundThemaUndRefresh(ByVal thema As String)
    '    nachricht("setzeVordergrundThemaUndRefresh: ---------------------- ")
    '    Try
    '        clsMiniMapTools.setzeVHgrund(thema)
    '        Dim erfolg As Boolean = clsMiniMapTools.getMapCookie_alleDB(CLstart.myc.kartengen.aktMap, myglobalz.sitzung.aktVorgangsID)
    '        setzeHintergrundTextInMiniMap()
    '        zeigeKartenbreiteTextInMinimap()
    '        presentMapOLD()
    '    Catch ex As Exception
    '        nachricht("Fehler in setzeVordergrundThemaUndRefresh: " ,ex)
    '    End Try
    'End Sub


    'Private Sub refreshminimap(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    panningAusschalten()
    '    INITMiniMapPresentation(myglobalz.sitzung.raumbezugsRec.dt, True)
    '    e.Handled = True
    'End Sub

    'Private Sub RubberbandMove(ByVal e As System.Windows.Input.MouseEventArgs)
    '    If e.LeftButton = MouseButtonState.Pressed And RubberbandStartpt.HasValue Then
    '        Dim endpt As System.Windows.Point
    '        If chkBIGGIS.IsChecked Then
    '            endpt = e.GetPosition(myCanvas2)
    '        Else
    '            endpt = e.GetPosition(myCanvas)
    '        End If

    '        RubberbandEndpt = endpt
    '        Dim x0, y0, w, h As Double
    '        x0 = Math.Min(RubberbandStartpt.Value.X, endpt.X)
    '        y0 = Math.Min(RubberbandStartpt.Value.Y, endpt.Y)
    '        w = Math.Abs(endpt.X - RubberbandStartpt.Value.X)
    '        h = Math.Abs(endpt.Y - RubberbandStartpt.Value.Y)
    '        Canvas.SetLeft(rubberbox, x0)
    '        Canvas.SetTop(rubberbox, y0)
    '        Canvas.SetZIndex(rubberbox, 1000)
    '        rubberbox.Width = w
    '        rubberbox.Height = h
    '    End If
    'End Sub

    'Private Sub RubberbandStart(ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    If chkBIGGIS.IsChecked Then
    '        RubberbandStartpt = e.GetPosition(myCanvas2)
    '    Else
    '        RubberbandStartpt = e.GetPosition(myCanvas)
    '    End If

    '    Me.Cursor = Cursors.Cross
    'End Sub
    'Private Sub btnGoogleMaps(sender As Object, e As RoutedEventArgs)
    '    nachricht("USERAKTION: googlekarte ")
    '    MsgBox("Klicken Sie oben auf den Reiter 'Googlemaps: Vogelperspektive'")
    '    'Dim gis As New clsGISfunctions
    '    'Dim result As String
    '    'gis.GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(CLstart.myc.kartengen.aktMap.aktrange)
    '    'If result = "fehler" Or result = "" Then
    '    'Else
    '    '    '  gis.starten(result)
    '    '    GMtemplates.templateStarten(result)
    '    'End If
    '    'gis = Nothing
    '    e.Handled = True
    'End Sub
    'Private Sub btnZWERT_Click(sender As Object, e As RoutedEventArgs)
    '    nachricht("USERAKTION: zwert messen ")
    '    panningAusschalten()
    '    MsgBox("Wählen sie den Punkt in der Karte (Linke Maustaste drücken)")
    '    CanvasClickModus = "ZWERT"
    '    zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '    gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '    'clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
    '    e.Handled = True
    'End Sub



    'Private Sub pdfTestClick(sender As Object, e As RoutedEventArgs)
    '    nachricht("USERAKTION: pdf erzeugen ")
    '    Dim aaa As New winPDFDruck
    '    aaa.ShowDialog()
    '    If aaa.chkEreignisMap.IsChecked Then
    '        refreshEreignisseListe(myglobalz.sitzung.aktVorgangsID)
    '    End If
    '    e.Handled = True
    'End Sub



    'Private Shared Sub alterWebgisAufruf()
    '    nachricht("USERAKTION: zum webgis ")
    '    Dim layer As String
    '    Dim aktmap As New clsMapSpec
    '    clsMiniMapTools.setzeAbteilungsThemen(aktmap, myGlobalz.sitzung.aktBearbeiter.Bemerkung.Trim)
    '    layer = aktmap.Vgrund & ";" & aktmap.Hgrund
    '    Debug.Print(CLstart.myc.kartengen.aktMap.aktrange.toString)
    '    Dim gis As New clsGISfunctions
    '    Dim http As String = gis.WebGISmittelpunktsAufruf(CLstart.myc.kartengen.aktMap.aktrange, layer, initP.getValue("GisServer.GIS_WebServer"))
    '    'starten als unabhängigen prozess
    '    clsGISfunctions.starten(http)
    'End Sub

    'Private Sub fstPaint_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    nachricht("USERAKTION: paint ")
    '    paintTools.DateiFeststellenUndPainten()
    '    e.Handled = True
    'End Sub



    'Private Sub btnGetCoordinates_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    nachricht("USERAKTION: koordinate messen ")
    '    panningAusschalten()
    '    MsgBox("Wählen sie den Punkt in der Karte (Linke Maustaste drücken)")
    '    CanvasClickModus = "Koordinate"
    '    'clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
    '    zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '    gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '    e.Handled = True
    'End Sub

    'Private Sub inputGetFlaeche()
    '    nachricht("USERAKTION: fläche messen ")
    '    MsgBox(glob2.getMsgboxText("polygonErfassen", New List(Of String)(New String() {})))
    '    btnGetFlaecheEnde.Background = Brushes.Red
    '    btnGetFlaecheEnde2.Background = Brushes.Red

    '    CanvasClickModus = "Flaeche"
    '    btnGetFlaeche.IsEnabled = False
    '    btnGetFlaeche2.IsEnabled = False
    '    btnGetFlaecheEnde.IsEnabled = True
    '    btnGetFlaecheEnde2.IsEnabled = True
    '    'clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
    '    zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '    gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '    If chkBIGGIS.IsChecked Then
    '        DrawPolygon(myCanvas2)
    '    Else
    '        DrawPolygon(myCanvas)
    '    End If

    'End Sub
    'Private Sub btnGetFlaeche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    nachricht("USERAKTION: FlächeMessen angeklickt")
    '    panningAusschalten()
    '    inputGetFlaeche()
    '    e.Handled = True
    'End Sub



    'Private Sub myPolyFinish(typ As String)
    '    btnAusschnitt.IsChecked = False
    '    If typ = "flaeche" Then
    '        myglobalz.sitzung.aktPolygon.myPoly.Width = 0
    '        myglobalz.sitzung.aktPolygon.myPoly.Height = 0
    '    End If
    '    If typ = "strecke" Then
    '        myglobalz.sitzung.aktPolyline.myLine.Width = 0
    '        myglobalz.sitzung.aktPolyline.myLine.Height = 0
    '    End If
    '    Mouse.Capture(Nothing)
    '    Me.Cursor = Nothing
    '    If chkBIGGIS.IsChecked Then
    '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Visible)
    '    Else
    '        clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Visible)
    '    End If
    'End Sub

    'Private Sub btnGetFlaecheEnde_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    If CanvasClickModus.ToLower = "flaeche" Then
    '        btnGetFlaeche.IsEnabled = True
    '        btnGetFlaecheEnde.IsEnabled = False
    '        btnGetFlaecheEnde.Background = Brushes.Black
    '        btnGetFlaeche2.IsEnabled = True
    '        btnGetFlaecheEnde2.IsEnabled = False
    '        btnGetFlaecheEnde2.Background = Brushes.Black
    '        btnGetFlaecheEnde_ClickExtracted()
    '    End If

    '    If CanvasClickModus.ToLower = "strecke" Then
    '        btnGetLinie.IsEnabled = True
    '        btnGetFlaecheEnde.IsEnabled = False
    '        btnGetFlaecheEnde2.IsEnabled = False
    '        btnGetLinie2.IsEnabled = True
    '        btnGetlinieEnde_ClickExtracted()
    '    End If
    '    e.Handled = True
    'End Sub



    'Private Sub btnGetFlaecheEnde_ClickExtracted()
    '    Dim anyChange As Boolean
    '    If myPolyVertexCount% > 2 Then
    '        If clsMiniMapTools.PolygonAufbereiten(myglobalz.sitzung.aktPolygon) Then
    '            clsMiniMapTools.GK_FlaecheErmitteln()
    '        End If
    '        tbMinimapFlaeche.Text = CLng(myglobalz.sitzung.aktPolygon.Area).ToString
    '        tbMinimapCoordinate2.Text = CLng(myglobalz.sitzung.aktPolygon.Area).ToString & " [qm}"
    '        If FlaecheAlsNeuenRaumbezugAnbieten("Fläche") Then
    '            ' MsgBox("Baustelle")
    '            myglobalz.sitzung.raumbezugsmodus = "neu"
    '            'myGlobalz.sitzung.aktPolygon.clear() sonst geht fläche verloren
    '            myglobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polygon
    '            Dim geoedit As New Win_Polygon("flaeche") 'myGlobalz.sitzung.aktPolygon.myPoly)
    '            geoedit.ShowDialog()
    '            anyChange = CBool(geoedit.DialogResult)
    '        End If
    '    Else
    '        MsgBox("Zu wenig Punkte für eine Flächenberechnung. Mind. 3 Punkte sind erforderlich!")
    '    End If
    '    myPolyFinish("flaeche")
    '    If anyChange Then refreshRaumbezugsListe(myglobalz.sitzung.aktVorgangsID, False, True)
    'End Sub



    'Private Sub btnGetLinie_Click(sender As Object, e As RoutedEventArgs)
    '    btnGetLinie_ClickExtracted()
    '    e.Handled = True
    'End Sub

    'Private Sub btnGetLinie_ClickExtracted()
    '    nachricht("USERAKTION: strecke messen ")
    '    panningAusschalten()
    '    MsgBox("Wählen sie die Strecke in der Karte indem Sie die Punkte anklicken (Linke Maustaste drücken)")
    '    CanvasClickModus = "Strecke"
    '    btnGetLinie.IsEnabled = False
    '    btnGetFlaecheEnde.IsEnabled = True
    '    btnGetFlaecheEnde2.IsEnabled = True
    '    btnGetFlaecheEnde.Background = Brushes.Red
    '    btnGetFlaecheEnde2.Background = Brushes.Red

    '    btnGetLinie2.IsEnabled = False
    '    myglobalz.sitzung.aktPolyline.clear()
    '    If chkBIGGIS.IsChecked Then
    '        'clsMiniMapTools.VisibilityDerKinderschalten(myCanvas2, Windows.Visibility.Collapsed)
    '        zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '        gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '        DrawPolylinie(myCanvas2)
    '    Else
    '        'clsMiniMapTools.VisibilityDerKinderschalten(myCanvas, Windows.Visibility.Collapsed)
    '        zeichneOverlaysGlob = True : zeichneImageMapGlob = False
    '        gisDarstellenAlleEbenen(zeichneOverlaysGlob, zeichneImageMapGlob, CBool(ckbeigentuemerFunktion.IsChecked))
    '        DrawPolylinie(myCanvas)
    '    End If
    'End Sub
    'Private Sub DrawPolygon(ByVal mycanvas As Canvas)
    '    myglobalz.sitzung.aktPolygon.myPoly = New Polygon
    '    myglobalz.sitzung.aktPolygon.myPoly.Name = "myPoly"
    '    Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 100, 250)) 'transparenz ist der erste wert
    '    myglobalz.sitzung.aktPolygon.myPoly.Stroke = Brushes.DarkBlue
    '    myglobalz.sitzung.aktPolygon.myPoly.StrokeThickness = 2
    '    myglobalz.sitzung.aktPolygon.myPoly.Opacity = 90
    '    myglobalz.sitzung.aktPolygon.myPoly.Fill = myBrush
    '    Panel.SetZIndex(myglobalz.sitzung.aktPolygon.myPoly, 100)
    '    Canvas.SetZIndex(myglobalz.sitzung.aktPolygon.myPoly, 100)
    '    mycanvas.Children.Add(myglobalz.sitzung.aktPolygon.myPoly)
    '    Panel.SetZIndex(myglobalz.sitzung.aktPolygon.myPoly, 100)
    '    Canvas.SetZIndex(myglobalz.sitzung.aktPolygon.myPoly, 100)
    '    myPolyVertexCount% = 0
    'End Sub

    'Private Sub DrawPolylinie(ByVal mycanvas As Canvas)
    '    myglobalz.sitzung.aktPolyline.myLine = New Polyline
    '    myglobalz.sitzung.aktPolyline.myLine.Name = "myLine"
    '    Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 100, 250)) 'transparenz ist der erste wert
    '    myglobalz.sitzung.aktPolyline.myLine.Stroke = Brushes.DarkBlue
    '    myglobalz.sitzung.aktPolyline.myLine.StrokeThickness = 4
    '    myglobalz.sitzung.aktPolyline.myLine.Opacity = 90
    '    myglobalz.sitzung.aktPolyline.myLine.Fill = myBrush
    '    Panel.SetZIndex(myglobalz.sitzung.aktPolyline.myLine, 100)
    '    Canvas.SetZIndex(myglobalz.sitzung.aktPolyline.myLine, 100)
    '    mycanvas.Children.Add(myglobalz.sitzung.aktPolyline.myLine)
    '    Panel.SetZIndex(myglobalz.sitzung.aktPolyline.myLine, 100)
    '    Canvas.SetZIndex(myglobalz.sitzung.aktPolyline.myLine, 100)
    '    myPolyVertexCount = 0
    'End Sub



    Private Sub btnZumGis_Click(sender As Object, e As RoutedEventArgs)
        mgisStarten(CBool(paradigmaKILLALLGIS.IsChecked))
        e.Handled = True
    End Sub


End Class
