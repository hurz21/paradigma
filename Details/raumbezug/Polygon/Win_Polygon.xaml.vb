Public Class Win_Polygon
    Property anyChange As Boolean
    Property _typ As String
    'Property _myPoly As Polygon
    Property polyauszugstext$
    ' Shared Property randPunkt As myPoint

    Private Sub Win_Polygon_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show("Sie haben Daten in dieser Maske geändert! " & vbCrLf &
                  "Wenn Sie diese Änderungen " & vbCrLf &
                  " - prüfen und ggf. speichern möchten wählen Sie 'JA'" & vbCrLf &
                  " - verwerfen möchten wählen Sie 'Nein'" & vbCrLf &
                  "Prüfen und abspeichern ?",
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
    Sub initFunktionCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxRBfunktion"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\RBfunktion.xml")
    End Sub

    Private Sub Win_Polygon_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initFunktionCombo()
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            'If _typ = "strecke" Then
            ' myGlobalz.sitzung.aktPolygon.clear()
            tbKoordinaten.Text = myGlobalz.sitzung.aktPolyline.GKstring
            tbArea.Text = myGlobalz.sitzung.aktPolyline.Distanz.ToString
            tbTyp.Text = RaumbezugsTyp.Polyline.ToString
            labelArea.Text = "Länge [m]:"
        End If
        If _typ = "flaeche" Then
            myGlobalz.sitzung.aktPolygon.clear()
            tbKoordinaten.Text = myGlobalz.sitzung.aktPolygon.GKstring
        End If

        btnSpeichern.IsEnabled = True
        btnLoeschen.IsEnabled = False

        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then

            If _typ = "strecke" Then
                ' myGlobalz.sitzung.aktPolygon.clear()
                tbKoordinaten.Text = myGlobalz.sitzung.aktPolygon.GKstring
                tbArea.Text = myGlobalz.sitzung.aktPolygon.Area.ToString
                tbTyp.Text = RaumbezugsTyp.Polyline.ToString
                labelArea.Text = "Länge [m]:"
            End If
            If _typ = "flaeche" Then

            End If
            tbKoordinaten.Text = "Die Geometrie läßt sich nicht ändern. " & Environment.NewLine &
                "Sie können aber das Polygon löschen und mit neuer Geometrie einfach neu anlegen."
            btnLoeschen.IsEnabled = True
            btnSpeichern.IsEnabled = False
        End If
        anyChange = False
        cmbFunktionsvorschlaege.IsDropDownOpen = True
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.raumbezugsmodus, "Raumbezug: Polygon")
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
        'Select Case myGlobalz.sitzung.akt_raumbezugsTyp
        '    Case RaumbezugsTyp.Polygon
        '        rid = CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
        '        If rid < 1 Then
        '            rid = CInt(myGlobalz.sitzung.aktPolyline.RaumbezugsID)
        '        End If
        '    Case RaumbezugsTyp.Polyline
        '        rid = CInt(myGlobalz.sitzung.aktPolyline.RaumbezugsID)
        '        If rid < 1 Then
        '            rid = CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
        '        End If
        '    Case RaumbezugsTyp.Flurstueck
        '        rid = CInt(myGlobalz.sitzung.aktFST.RaumbezugsID)
        '        If rid < 1 Then
        '            rid = CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
        '        End If
        '    Case Else
        '        rid = CInt(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
        'End Select
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
        speichernAktpolygon(_typ)
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

    Function speichernAktpolygon(typ As String) As Boolean
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

    Private Shared Sub NeuesPolygonspeichern()
        Dim sekID% = 0 ' bei polygonen gibts keine zusatzinfos , keine tabelle 'Parapolygon' also auch kein sekid
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            PolygonTools.PolygonNeuSpeichern(sekID)
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            PolygonTools.editPolygonspeichernExtracted(sekID)
        End If
        myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
             detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
       ' glob2.EDIT_VorgangStamm_2DBOk()
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



End Class
