Imports System.Data

Public Class Win_punktMitUmkreis
    Property anyChange As Boolean
    Property _minimapKoordinatenText As String
    'Property RESULT_text_NSG As String
    'Private Property RESULT_dateien_NSG As New List(Of gisresult)

    'Property RESULT_text_WSG As String
    'Private Property RESULT_dateien_WSG As New List(Of gisresult)

    'Property RESULT_text_Bplan As String
    'Private Property RESULT_dateien_Bplan As New List(Of gisresult)

    'Property RESULT_text_kehr As String
    'Private Property RESULT_dateien_Kehr As New List(Of gisresult)

    'Property RESULT_text_ueg As String
    'Private Property RESULT_dateien_ueg As List(Of gisresult)

    Private Shared Property erfolg As Boolean
    'Property eigentumerKurzinfo As String
    'Property Eigentuemernameundadresse As String
    'Dim BPLANverordnungsdatei, BPLANbeschreibung As String
    'Dim NSGverordnungsdatei, NSGbeschreibung As String
    'Dim UEGverordnungsdatei, UEGbeschreibung As String
    'Dim KEHRbeschreibung As String
    'Dim Eigentbeschreibung As String
    'Private Property WSGverordnungsdatei As String
    'Private Property WSGbeschreibung As String



    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub

    Private Sub Window_RB_Adresse_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show(glob2.getMsgboxText("MaskeGeaendert", New List(Of String)(New String() {})),
                  "Ereignisdetails", _
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

    Sub New(ByVal minimapKoordinatenText$)
        InitializeComponent()
        _minimapKoordinatenText$ = minimapKoordinatenText$
    End Sub

    Private Sub Win_punktMitUmkreis_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        initFunktionCombo()

        Me.DataContext = myGlobalz.sitzung
        'Gisanalysesetzen()
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            myGlobalz.sitzung.aktPMU.clear()
            myGlobalz.sitzung.aktPMU.Radius = 100
            btnLoeschen.IsEnabled = False
            btnSpeichern.IsEnabled = False
            'koordinateAusMiniMapUebernehmen()
            btnPufferbilden.IsEnabled = False
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            btnLoeschen.IsEnabled = True
            btnSpeichern.IsEnabled = False
            btnWindrose.IsEnabled = True
            btnPufferbilden.IsEnabled = True
            chkMitEtikett.IsChecked = myGlobalz.sitzung.aktPMU.mitEtikett
        End If
        anyChange = False
        'cmbFunktionsvorschlaege.IsDropDownOpen = True
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.raumbezugsmodus, "Raumbezug: Punkt mit Umkreis")
        gastLayout()
    End Sub

    Sub initFunktionCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxRBfunktion"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\RBfunktion.xml")
    End Sub
    'Private Sub Gisanalysesetzen()
    '    Try
    '        Dim a As String = CLstart.myc.userIniProfile.WertLesen("GISANALYSE", "alleausfuehren")
    '        If a.IsNothingOrEmpty Then a = "0"
    '        If a.IsNothingOrEmpty Then
    '            chkGisanalyse.IsChecked = True
    '        End If
    '        If CInt(a) = 0 Then
    '            chkGisanalyse.IsChecked = False
    '        Else
    '            chkGisanalyse.IsChecked = True
    '        End If
    '    Catch ex As Exception
    '        nachricht("warnung in Gisanalysesetzen2: " ,ex)
    '        chkGisanalyse.IsChecked = True
    '    End Try
    'End Sub
    'Sub koordinateAusMiniMapUebernehmen()
    '    Try
    '        If String.IsNullOrEmpty(_minimapKoordinatenText) Then Exit Sub
    '        _minimapKoordinatenText = _minimapKoordinatenText.Replace("[m]", "").Trim
    '        Dim a$() = _minimapKoordinatenText$.Split(","c)
    '        If a.Length > 0 Then
    '            myGlobalz.sitzung.aktPMU.punkt.X = CInt(a(0))
    '            myGlobalz.sitzung.aktPMU.punkt.Y = CInt(a(1))
    '            tbNachricht.Text = "Die Koordinaten wurden aus dem letzten Klickpunkt der Minimap übernommen."
    '            anyChange = True
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler bei koordinateAusMiniMapUebernehmen: " & vbCrLf ,ex)
    '    End Try
    'End Sub

    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ParaUmkreisTools.loeschenAktPMU()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        speichernPMU()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnSpeichern.IsEnabled = False
        Me.Close()
        e.Handled = True
    End Sub

    Sub speichernPMU()
        Try
            If CInt(myGlobalz.sitzung.aktPMU.Status) = 1 Then
                MsgBox("Es handelt sich um einen Punkt eines Verwandte Vorgangs. Er kann nicht geändert werden!")
                Exit Sub
            End If
            myGlobalz.sitzung.aktPMU.punkt.X = CDbl(tbx.Text)
            myGlobalz.sitzung.aktPMU.punkt.Y = CDbl(tby.Text)
            myGlobalz.sitzung.aktPMU.Radius = CInt(tbradius.Text)
            myGlobalz.sitzung.aktPMU.Name = (tbBeschreibung.Text)
            myGlobalz.sitzung.aktPMU.Freitext = (tbFreitext.Text)
            myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Umkreis
            myGlobalz.sitzung.aktPMU.isMapEnabled = CBool(chkMapenabled.IsChecked)


            If Not istEingabe_vorhanden() Then Exit Sub
            If Not ParaUmkreisTools.ParaPunktLiegtImKreisOffenbach(myGlobalz.sitzung.aktPMU.punkt, CLstart.myc.globalrange) Then
                MsgBox("Hinweis: Der Punkt liegt nicht im Kreis Offenbach!" & vbCrLf & "Er wird trotzdem gespeichert.")
            End If
            myGlobalz.sitzung.aktPMU.mitEtikett = CBool(chkMitEtikett.IsChecked)
            If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
                ParaUmkreisTools.Umkreis_Neu()
                btnSpeichern.IsEnabled = False
                Me.Close()
            End If
            If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
                Umkreis_Edit(CInt(myGlobalz.sitzung.aktPMU.SekID))
                btnSpeichern.IsEnabled = False
            End If
            myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
            'myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = Now
            'glob2.EDIT_VorgangStamm_2DBOk()
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
        Catch ex As Exception
            nachricht("fehler in speichernPMU: " ,ex)
        End Try
    End Sub

    Private Sub Umkreis_Edit(ByVal sekid%)
        'todo änderung beim raumbezug muss auch in die datenbank

        ' If ParaUmkreisTools.RB_Umkreis_abspeichern_Edit(sekid%) > 0 Then
        Dim anzahl As Integer = ParaUmkreisTools.umkreisEdit_alleDB(sekid%)
        If anzahl > 0 Then
            btnSpeichern.IsEnabled = False
            myGlobalz.sitzung.aktPMU.defineAbstract()
            DBraumbezug_Mysql.defineBBOX(myGlobalz.sitzung.aktPMU.Radius, myGlobalz.sitzung.aktPMU)

            RBtoolsns.Raumbezug_edit_alleDB.execute(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.aktPMU)

            ' DBraumbezug_Mysql.Raumbezug_abspeichern_Edit(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.aktPMU)
            ParaUmkreisTools.RB_Umkreis_append_PointShapeFile()

        Else
            nachricht("Problem beim Abspeichern!Umkreis_Edit")
        End If
    End Sub

    Function istEingabe_vorhanden() As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPMU.punkt.X.ToString) OrElse myGlobalz.sitzung.aktPMU.punkt.X < 1000 Then
            MsgBox("Bitte Rechtswert eingeben")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktPMU.punkt.X) Then
            MsgBox("Bitte nur Zahlen >0 für den Rechtswert")
            Return False
        End If

        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPMU.punkt.Y.ToString) OrElse myGlobalz.sitzung.aktPMU.punkt.Y < 1000 Then
            MsgBox("Bitte Hochwert eingeben")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktPMU.punkt.Y) Then
            MsgBox("Bitte nur Zahlen >0  für den Hochwert")
            Return False
        End If
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPMU.Radius.ToString) OrElse myGlobalz.sitzung.aktPMU.Radius < 1 Then
            MsgBox("Bitte Radius eingeben")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktPMU.RaumbezugsID) Then
            MsgBox("Bitte nur Zahlen >0  für den Radius")
            Return False
        End If
        Return True
    End Function

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbx.TextChanged
        anyChange = True
        If btnWindrose IsNot Nothing Then btnWindrose.IsEnabled = True
        If String.IsNullOrEmpty(tbx.Text) Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(20, tbx)
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tby.TextChanged
        anyChange = True
        If btnWindrose IsNot Nothing Then btnWindrose.IsEnabled = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(20, tby)
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFreitext.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(200, tbFreitext)
    End Sub


    Private Sub tbBeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(200, tbBeschreibung)
    End Sub

    Private Sub btnWindrose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        adressToolsUI.windRoseAnzeigen(myGlobalz.sitzung.aktPMU.punkt.X, myGlobalz.sitzung.aktPMU.punkt.Y)
    End Sub

    Private Sub cmbFunktionsvorschlaege_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbFunktionsvorschlaege.SelectedItem Is Nothing Then Exit Sub
        'Dim myvali$ = CStr(cmbFunktionsvorschlaege.SelectedValue)
        Dim myvalx = CType(cmbFunktionsvorschlaege.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        If tbBeschreibung IsNot Nothing Then tbBeschreibung.Text = myvals
    End Sub

    Private Sub btnPufferbilden_Click(sender As Object, e As RoutedEventArgs)
        Dim pufferinMeter As Double = CDbl(tbpufferinmeter.Text)
        Dim puffererzeugt As Boolean = False
        puffererzeugt = FST_tools.bildePufferFuerPunkt(myGlobalz.sitzung.aktPMU.punkt, pufferinMeter)
        GC.Collect()
        If puffererzeugt Then
            MsgBox("Das Puffer-Objekt wurde erzeugt und unter 'Raumbezüge'' abgelegt.")
        End If
        btnSpeichern.IsEnabled = False
        Close()
        e.Handled = True
    End Sub

    Private Sub tbFreitext_TextChanged(sender As Object, e As TextChangedEventArgs)
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(500, tbFreitext)
        If tbFreitext.Text.Length > 0 Then
            chkMitEtikett.IsEnabled = True
        Else
            chkMitEtikett.IsEnabled = False
        End If
        e.Handled = True
    End Sub

    Private Sub chkMitEtikett_Click(sender As Object, e As RoutedEventArgs)
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub chkMapenabled_Click(sender As Object, e As RoutedEventArgs)
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub tbradius_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbradius.TextChanged

    End Sub
    'Private Sub btnAlleGisanalysen_Click(sender As Object, e As RoutedEventArgs)
    '    gisanalyse()
    '    e.Handled = True
    'End Sub
    'Private Sub gisanalyse()
    '    Dim Plannr As String = ""
    '    Dim neupunkt As New clsGEOPoint : neupunkt.X = myGlobalz.sitzung.aktPMU.punkt.X : neupunkt.Y = myGlobalz.sitzung.aktPMU.punkt.Y
    '    If NSpostgis.clsPostgis.getBplanInfo4point(neupunkt, RESULT_dateien_Bplan, Plannr, RESULT_text_Bplan) Then
    '        btnbplanaufruf.IsEnabled = True
    '        tbbplangueltig.Text = "B-Plan: " & Plannr.Trim
    '        tbbplangueltig.Background = Brushes.LightGreen
    '        BPLANverordnungsdatei = RESULT_dateien_Bplan.Item(0).datei.FullName.Trim
    '    Else
    '        btnbplanaufruf.IsEnabled = False
    '        tbbplangueltig.Text = Plannr
    '        tbbplangueltig.Text = "kein Bplan"
    '    End If

    '    zwischenInfo("B-Plan fertig")

    '    Dim bezirk As String = ""
    '    If NSpostgis.clsPostgis.getKehrbezirkInfo4point(neupunkt, RESULT_text_kehr, bezirk) Then
    '        btnKehraufruf.IsEnabled = True
    '        KEHRbeschreibung = RESULT_text_kehr
    '        tbKehrgueltig.Text = bezirk
    '        tbKehrgueltig.Background = Brushes.LightGreen
    '    Else
    '        btnKehraufruf.IsEnabled = False
    '        tbKehrgueltig.Text = "keine Info"
    '    End If
    '    zwischenInfo("Kehrbezirk fertig")


    '    Dim fs As String = ""
    '    If NSpostgis.clsPostgis.getFS4coordinates(myGlobalz.sitzung.aktPMU.punkt.X, myGlobalz.sitzung.aktPMU.punkt.Y, fs) Then
    '        Dim eigSDB As New clsEigentuemerschnell
    '        Dim dt As DataTable = Nothing
    '        Dim mycount As Integer
    '        eigSDB.oeffneConnectionEigentuemer()
    '        If eigSDB.getEigentuemerdata(fs, eigentumerKurzinfo, Eigentuemernameundadresse, mycount, dt) Then
    '            tbEigentgueltig.Text = Eigentuemernameundadresse
    '            tbEigentgueltig.Background = Brushes.LightGreen
    '            btnEigentaufruf.IsEnabled = True
    '        Else
    '            btnEigentaufruf.IsEnabled = False
    '            tbEigentgueltig.Text = "keine Info"
    '        End If
    '    End If
    '    zwischenInfo("Eigentümer fertig")

    '    Dim sgnr As String = ""
    '    Dim Zusatz As String = ""
    '    If NSpostgis.clsPostgis.getNSGInfo4Point(neupunkt, RESULT_dateien_NSG, sgnr) Then
    '        btnNSGaufruf.IsEnabled = True
    '        btnNSGaufruf.ToolTip = "zur nichtamtlichen Verordnung"
    '        tbNSGgueltig.Text = "NSG/GLB/LSG"
    '        If RESULT_dateien_NSG.Count = 1 Then
    '            NSGverordnungsdatei = RESULT_dateien_NSG.Item(0).datei.FullName
    '            NSGbeschreibung = RESULT_dateien_NSG.Item(0).dateibeschreibung
    '            tbNSGgueltig.Background = Brushes.LightGreen
    '            btnNSGaufruf.Visibility = Windows.Visibility.Visible
    '            cmbNSGauswahl.Visibility = Windows.Visibility.Collapsed
    '        End If
    '        If RESULT_dateien_NSG.Count > 1 Then
    '            btnNSGaufruf.Visibility = Windows.Visibility.Collapsed
    '            cmbNSGauswahl.Visibility = Windows.Visibility.Visible
    '            cmbNSGauswahl.DataContext = RESULT_dateien_NSG
    '            cmbNSGauswahl.SelectedIndex = 0
    '            Zusatz = " -> mehrere NSG-VO gültig!"
    '        End If
    '    Else

    '        btnNSGaufruf.IsEnabled = False
    '        tbNSGgueltig.Text = sgnr
    '        tbNSGgueltig.Text = "kein NSG/LSG/GLB"
    '    End If

    '    zwischenInfo("NSG/LSG/GLB Gebiete fertig" & Zusatz)


    '    Dim uenr As String = "" : Zusatz = ""
    '    If NSpostgis.clsPostgis.getUEGebietInfo4Point(neupunkt, RESULT_dateien_ueg, uenr) Then
    '        btnUESGaufruf.IsEnabled = True
    '        btnUESGaufruf.ToolTip = "zur nichtamtlichen Verordnung"
    '        tbUESGgueltig.Text = "Ü-Gebiet"
    '        tbUESGgueltig.Background = Brushes.LightGreen
    '        UEGverordnungsdatei = RESULT_dateien_ueg.Item(0).datei.FullName
    '        UEGbeschreibung = RESULT_dateien_ueg.Item(0).dateibeschreibung
    '    Else
    '        btnUESGaufruf.IsEnabled = False
    '        tbUESGgueltig.Text = uenr
    '        tbUESGgueltig.Text = "kein Ü-Gebiet"
    '    End If

    '    zwischenInfo("Überschwemmungsgebiete fertig")

    '    Dim WSGnr As String = ""
    '    If NSpostgis.clsPostgis.getWSGebietInfo4Point(neupunkt, RESULT_dateien_WSG, WSGnr) Then
    '        btnWSGaufruf.IsEnabled = True
    '        btnWSGaufruf.ToolTip = "zur nichtamtlichen Verordnung"
    '        tbWSGgueltig.Text = "WSG"
    '        tbWSGgueltig.Background = Brushes.LightGreen

    '        If RESULT_dateien_WSG.Count = 1 Then
    '            WSGverordnungsdatei = RESULT_dateien_WSG.Item(0).datei.FullName
    '            WSGbeschreibung = RESULT_dateien_WSG.Item(0).dateibeschreibung
    '            btnWSGaufruf.Visibility = Windows.Visibility.Visible
    '            cmbWSGauswahl.Visibility = Windows.Visibility.Collapsed
    '            Zusatz = ""
    '        End If
    '        If RESULT_dateien_WSG.Count > 1 Then
    '            btnWSGaufruf.Visibility = Windows.Visibility.Collapsed
    '            cmbWSGauswahl.Visibility = Windows.Visibility.Visible
    '            'WSGverordnungsdatei = RESULT_dateien.Item(0).datei.FullName
    '            'WSGbeschreibung = RESULT_dateien.Item(0).dateibeschreibung
    '            cmbWSGauswahl.DataContext = RESULT_dateien_WSG
    '            cmbWSGauswahl.SelectedIndex = 0
    '            Zusatz = " -> mehrere WSG-VO gültig!"
    '        End If
    '    Else
    '        btnWSGaufruf.IsEnabled = False
    '        tbWSGgueltig.Text = uenr
    '        tbWSGgueltig.Text = "kein WSG" '& WSGzusatz
    '    End If
    '    zwischenInfo("WSG fertig" & Zusatz)

    '    zwischenInfo("GIS-Analyse abgeschlossen")
    'End Sub

    'Private Sub btnEigentaufruf_Click(sender As Object, e As RoutedEventArgs)
    '    tbInfo.Text = eigentumerKurzinfo & " " & Eigentuemernameundadresse
    '    e.Handled = True
    'End Sub

    'Private Sub btnAlleGisanalysen_Click(sender As Object, e As RoutedEventArgs)
    '    tbInfo.Clear()
    '    gisanalyse()
    '    e.Handled = True
    'End Sub

    'Private Sub btnNSGaufruf_Click(sender As Object, e As RoutedEventArgs)
    '    If NSGverordnungsdatei.Trim.IsNothingOrEmpty Then
    '        MsgBox("Kein SG zur Adresse gefunden.")
    '    Else
    '        tbInfo.Text = NSGbeschreibung
    '        Process.Start(NSGverordnungsdatei)
    '    End If
    '    e.Handled = True
    'End Sub

    'Private Sub btnUESGaufruf_Click(sender As Object, e As RoutedEventArgs)
    '    If UEGverordnungsdatei.Trim.IsNothingOrEmpty Then
    '        MsgBox("Kein Ü-Gebiet zur Adresse gefunden.")
    '    Else
    '        tbInfo.Text = UEGbeschreibung
    '        Process.Start(UEGverordnungsdatei)
    '    End If
    '    e.Handled = True
    'End Sub

    'Private Sub btnWSGaufruf_Click(sender As Object, e As RoutedEventArgs)
    '    If WSGverordnungsdatei.Trim.IsNothingOrEmpty Then
    '        MsgBox("Kein WSG-Gebiet zur Adresse gefunden.")
    '    Else
    '        tbInfo.Text = WSGbeschreibung
    '        Process.Start(WSGverordnungsdatei)
    '    End If
    '    e.Handled = True
    'End Sub
    'Private Sub zwischenInfo(text As String)
    '    Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
    '    tbInfo.Text &= text & Environment.NewLine
    '    Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
    'End Sub

    'Private Sub btnKehraufruf_Click(sender As Object, e As RoutedEventArgs)
    '    tbInfo.Text = RESULT_text_kehr
    '    e.Handled = True
    'End Sub
    'Private Sub btnbplanaufruf_Click(sender As Object, e As RoutedEventArgs)
    '    If BPLANverordnungsdatei.IsNothingOrEmpty Then
    '        MsgBox("Kein Bplan zur Adresse gefunden.")
    '    Else
    '        Process.Start(BPLANverordnungsdatei)
    '        tbInfo.Text = RESULT_text_Bplan
    '    End If
    '    e.Handled = True
    'End Sub

    'Private Sub cmbWSGauswahl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    '    Dim auswahlStrasse As gisresult = CType(cmbWSGauswahl.SelectedItem, gisresult)
    '    If auswahlStrasse Is Nothing Then Exit Sub
    '    If auswahlStrasse.etikett = "!! mehrere Treffer !!" Then Exit Sub
    '    auswahlStrasse.etikett = auswahlStrasse.etikett.Trim
    '    auswahlStrasse.dateibeschreibung = auswahlStrasse.dateibeschreibung.Trim
    '    auswahlStrasse.datei = auswahlStrasse.datei
    '    auswahlStrasse.verordnung = auswahlStrasse.verordnung.Trim
    '    Process.Start(auswahlStrasse.datei.FullName)
    '    tbInfo.Text = RESULT_text_WSG
    'End Sub

    'Private Sub cmbNSGauswahl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    '    Dim auswahlStrasse As gisresult = CType(cmbNSGauswahl.SelectedItem, gisresult)
    '    If auswahlStrasse Is Nothing Then Exit Sub
    '    If auswahlStrasse.etikett = "!! mehrere Treffer !!" Then Exit Sub
    '    auswahlStrasse.etikett = auswahlStrasse.etikett.Trim
    '    auswahlStrasse.dateibeschreibung = auswahlStrasse.dateibeschreibung.Trim
    '    auswahlStrasse.datei = auswahlStrasse.datei
    '    auswahlStrasse.verordnung = auswahlStrasse.verordnung.Trim
    '    Process.Start(auswahlStrasse.datei.FullName)
    '    tbInfo.Text = auswahlStrasse.dateibeschreibung
    'End Sub
End Class
