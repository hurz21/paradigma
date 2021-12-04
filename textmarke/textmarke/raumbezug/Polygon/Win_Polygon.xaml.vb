Public Class Win_Polygon
    Property anyChange As Boolean
    Property _typ As String
    'Property _myPoly As Polygon
    Property polyauszugstext As String
    Property exportfile As String
    Private Property _freitext As String

    ' Shared Property randPunkt As myPoint

    Private Sub Win_Polygon_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        initFunktionCombo()
        neuesObjekt()
        altesObjekt()
        anyChange = False
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.raumbezugsmodus, "Raumbezug: Polygon")

    End Sub

    Private Sub Win_Polygon_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show(glob2.getMsgboxText("MaskeGeaendert", New List(Of String)(New String() {})),
                  "Polygon", _
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Exclamation,
                  MessageBoxResult.OK)
            If Not red = MessageBoxResult.No Then
                'btnSpeichernEreignis.IsEnabled = False
                e.Cancel = True
            End If
        End If
        DialogResult = If(anyChange, True, False)
    End Sub

    Sub New(typ As String)
        InitializeComponent()
        _typ = typ
    End Sub
    Sub New(typ As String, freitext As String)
        InitializeComponent()
        _typ = typ
        _freitext = freitext
    End Sub
    Sub initFunktionCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxRBfunktion"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\RBfunktion.xml")
    End Sub

    Private Sub neuesObjekt()
       ' MsgBox(myGlobalz.sitzung.aktPolygon.GKstring)
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            If _typ = "strecke" Then
                myGlobalz.sitzung.aktPolygon.ShapeSerial = LIBpostgistools.nondbtools.serialGKStringnachWKT("3;0;" & myGlobalz.sitzung.aktPolyline.GKstring)
                myGlobalz.sitzung.aktPolyline.GKstring = myGlobalz.sitzung.aktPolygon.ShapeSerial
                labelArea.Text = "Länge [m]:"
                tbKoordinaten.Text = myGlobalz.sitzung.aktPolyline.GKstring
                tbArea.Text = myGlobalz.sitzung.aktPolyline.Distanz.ToString
                tbTyp.Text = RaumbezugsTyp.Polyline.ToString
            End If
            If _typ = "flaeche" Then
                myGlobalz.sitzung.aktPolygon.ShapeSerial = LIBpostgistools.nondbtools.serialGKStringnachWKT("5;0;" & myGlobalz.sitzung.aktPolygon.GKstring)
                myGlobalz.sitzung.aktPolygon.GKstring = myGlobalz.sitzung.aktPolygon.ShapeSerial
                tbKoordinaten.Text = myGlobalz.sitzung.aktPolygon.ShapeSerial
                If myGlobalz.sitzung.aktPolygon.GKstring.IsNothingOrEmpty() Then myGlobalz.sitzung.aktPolygon.GKstring = myGlobalz.sitzung.aktPolygon.ShapeSerial
                tbTyp.Text = RaumbezugsTyp.Polygon.ToString
                tbArea.Text = CStr(myGlobalz.sitzung.aktPolygon.Area)
                labelArea.Text = "Fläche [qm]:"
                tbFreitext.Text = _freitext
            End If
            cmbFunktionsvorschlaege.IsDropDownOpen = True
            btnFSTausCSVliste.IsEnabled = False
            btnFSTinnerhalb.IsEnabled = False
            btnPufferbilden.IsEnabled = False
        End If
    End Sub

    Private Sub altesObjekt()
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            chkMapenabled.IsChecked = CBool(myGlobalz.sitzung.aktPolygon.isMapEnabled)
            If _typ = "strecke" Then
                ' myGlobalz.sitzung.aktPolygon.clear()
                myGlobalz.sitzung.aktPolygon.ShapeSerial = LIBpostgistools.nondbtools.serialGKStringnachWKT(myGlobalz.sitzung.aktPolygon.ShapeSerial)
                  myGlobalz.sitzung.aktPolyline.GKstring = myGlobalz.sitzung.aktPolygon.ShapeSerial
                tbKoordinaten.Text = myGlobalz.sitzung.aktPolygon.ShapeSerial
                tbArea.Text = myGlobalz.sitzung.aktPolygon.Area.ToString
                tbTyp.Text = RaumbezugsTyp.Polyline.ToString
                labelArea.Text = "Länge [m]:"
                chkMapenabled.IsChecked = CBool(myGlobalz.sitzung.aktPolygon.isMapEnabled)
            End If
            If _typ = "flaeche" Then
                myGlobalz.sitzung.aktPolygon.ShapeSerial = LIBpostgistools.nondbtools.serialGKStringnachWKT(myGlobalz.sitzung.aktPolygon.ShapeSerial)
                  myGlobalz.sitzung.aktPolyline.GKstring = myGlobalz.sitzung.aktPolygon.ShapeSerial
                tbKoordinaten.Text = "Die Geometrie läßt sich nicht ändern. " &
                                                "Sie können aber das Objekt löschen und mit neuer Geometrie einfach neu anlegen." & Environment.NewLine &
                                                 myGlobalz.sitzung.aktPolygon.ShapeSerial
                tbArea.Text = myGlobalz.sitzung.aktPolygon.Area.ToString
                tbTyp.Text = RaumbezugsTyp.Polygon.ToString
                labelArea.Text = "Fläche [qm]:"
                chkMapenabled.IsChecked = CBool(myGlobalz.sitzung.aktPolygon.isMapEnabled)
            End If
            btnSpeichern.IsEnabled = False
            btnLoeschen.IsEnabled = True
              btnFSTausCSVliste.IsEnabled = True
            btnFSTinnerhalb.IsEnabled = True
            btnPufferbilden.IsEnabled = True
        End If
    End Sub



    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If Not glob2.istloeschenErnstgemeint() Then Exit Sub
        'id feststellenb
        Dim rid As Integer
        If _typ = "strecke" Then
            rid = CInt(myGlobalz.sitzung.aktPolyline.RaumbezugsID)
            If rid < 1 Then rid = CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
        End If
        If _typ = "flaeche" Then
            rid = CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
        End If

        Dim messi As New MessageBoxResult
        messi = MessageBox.Show("Polygon wirklich löschen ?" & vbCrLf,
                          "Polygon wirklich löschen ?",
                          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
        If messi = MessageBoxResult.Yes Then
            PolygonTools.polygonLoeschen(rid)
        End If

        Me.Close()
    End Sub

    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If myGlobalz.sitzung.aktPolygon.name.IsNothingOrEmpty Then myGlobalz.sitzung.aktPolygon.name = ""
        If myGlobalz.sitzung.aktPolygon.abstract.IsNothingOrEmpty Then myGlobalz.sitzung.aktPolygon.abstract = ""
        If myGlobalz.sitzung.aktPolygon.Freitext.IsNothingOrEmpty Then myGlobalz.sitzung.aktPolygon.Freitext = ""

        speichernAktpolygon(_typ, CBool(chkMapenabled.IsChecked))
        anyChange = True
        btnSpeichern.IsEnabled = False
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnSpeichern.IsEnabled = False
        anyChange = False
        Me.Close()
    End Sub

    Function speichernAktpolygon(typ As String, ismapenabled As Boolean) As Boolean
        myGlobalz.sitzung.aktPolygon.isMapEnabled = ismapenabled
        If typ = "flaeche" Then
            myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Polygon
            NeuesPolygonspeichern()
        End If
        If typ = "strecke" Then
            PolygonTools.polylineAufPolygonUmsetzen()
            myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Polyline
            NeuesPolygonspeichern()
        End If
        Return True
    End Function

    Private Sub NeuesPolygonspeichern()
        Dim sekID% = 0 ' bei polygonen gibts keine zusatzinfos , keine tabelle 'Parapolygon' also auch kein sekid
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            PolygonTools.PolygonNeuSpeichern(sekID)
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            PolygonTools.editPolygonspeichernExtracted(sekID)
        End If
        myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
        CLstart.myc.aLog.komponente = "Raumbezug"
        CLstart.myc.aLog.aktion = "Neues Polygon wurde gespeichert "
        CLstart.myc.aLog.log()
        anyChange = True
        'detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
    End Sub


    Private Sub tbBeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        If btnSpeichern IsNot Nothing Then btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub


    Private Sub cmbFunktionsvorschlaege_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbFunktionsvorschlaege.SelectedItem Is Nothing Then Exit Sub
        'Dim myvali$ = CStr(cmbFunktionsvorschlaege.SelectedValue)
        Dim myvalx = CType(cmbFunktionsvorschlaege.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        If tbBeschreibung IsNot Nothing Then tbBeschreibung.Text = myvals
    End Sub

    Private Sub tbFreitext_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFreitext.TextChanged
        If btnSpeichern IsNot Nothing Then btnSpeichern.IsEnabled = True
        e.Handled = True
    End Sub


    Private Sub btnPufferbilden_Click(sender As Object, e As RoutedEventArgs)
        Dim pufferinMeter As Double = CDbl(tbpufferinmeter.Text)
        Dim puffererzeugt As Boolean = FST_tools.bildePufferFuerPolygon(myGlobalz.sitzung.aktPolygon, pufferinMeter)
        GC.Collect()
        If puffererzeugt Then
            MsgBox("Das Puffer-Objekt wurde erzeugt und unter 'Raumbezüge'' abgelegt.")
        End If
        btnSpeichern.IsEnabled = False
        Close()
        e.Handled = True
    End Sub

    Private Sub btnFSTinnerhalb_Click(sender As Object, e As RoutedEventArgs)
        Dim pufferinMeter As Double = 1 'CDbl(tbpufferinmeter.Text)
        exportfile = FST_tools.bildeFSTListeFuerPolygon(myGlobalz.sitzung.aktPolygon, pufferinMeter)
        GC.Collect()
        If exportfile.IsNothingOrEmpty Then
            MsgBox("Die Liste wurde nicht erzeugt  .")
        Else
            MsgBox("Das Exportfile " & exportfile & " wurde erzeugt")
            Process.Start(exportfile)
        End If
        btnSpeichern.IsEnabled = False
        Close()
        e.Handled = True
    End Sub

    Private Sub btnFSTausCSVliste_Click(sender As Object, e As RoutedEventArgs)
        Me.Close()
        Dim pufferinMeter As Double = 1 'CDbl(tbpufferinmeter.Text)
        exportfile = FST_tools.bildeFSTListeFuerPolygon(myGlobalz.sitzung.aktPolygon, pufferinMeter)
        glob2.MeinGarbage()

        If exportfile.IsNothingOrEmpty Then
            MsgBox("Die Liste wurde nicht erzeugt. Keine  Flurstücke im Polygon gefunden.")
        Else
            myGlobalz.sitzung.raumbezugsmodus = "neu"
            Dim csvlisteerstellen As New WinCsvliste(exportfile)
            csvlisteerstellen.ShowDialog()
        End If
        e.Handled = True
    End Sub

    Private Sub chkMapenabled_Click(sender As Object, e As RoutedEventArgs)
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub
End Class
