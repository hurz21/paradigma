Imports System.Data

Partial Public Class Window_Flurstuecksauswahl

    Private Property anyChange As Boolean = False

    Private Sub Window_Flurstuecksauswahl_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initGemarkungsCombo()
        initFunktionCombo()
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            myGlobalz.sitzung.aktFST.clear()
            Title = "Raumbezug: Flurstück, Neues Flurstück anlegen"
            cmbgemarkung.IsDropDownOpen = True
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            'myGlobalz.sitzung.aktFST.clear()
            btnLoeschenEreignis.IsEnabled = True
            Title = "Raumbezug: Flurstück, Bestandsflurstück ändern"
            'If btnEigentuemer IsNot k Then btnEigentuemer.IsEnabled = True
            tbarea.Text=CStr(myGlobalz.sitzung.aktFST.normflst.flaecheqm)
            If btnEigentuemerALKIS IsNot Nothing Then btnEigentuemerALKIS.IsEnabled = True
            chkMapenabled.IsChecked = CBool(myGlobalz.sitzung.aktFST.isMapEnabled)
        End If
        btnSpeichernFlurstueck.IsEnabled = False
        anyChange = False
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.raumbezugsmodus, "Raumbezug: Flurstück")
        cmbFlur.IsEnabled = False
        cmbZaehler.IsEnabled = False
        cmbNenner.IsEnabled = False
    End Sub

    Sub initGemarkungsCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemarkungen"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemarkungen.xml")
    End Sub

    Private Sub Hyperlink_RequestNavigate(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
        Dim gis As New clsGISfunctions
        gis.flurstuecksAufruf(initP.getValue("GisServer.GIS_WebServer"))
        e.Handled = True
    End Sub

    
    Sub initFunktionCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxRBfunktion"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\RBfunktion.xml")
    End Sub

    Private Sub cmbgemarkung_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbgemarkung.SelectionChanged
        If cmbgemarkung.SelectedItem Is Nothing Then Exit Sub

        Dim myvali$ = CStr(cmbgemarkung.SelectedValue)
        Dim myvalx = CType(cmbgemarkung.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString

        tbGemarkung.Text = myvals
        myGlobalz.sitzung.aktFST.normflst.gemcode = CInt(myvali)
        myGlobalz.sitzung.aktFST.normflst.gemarkungstext = tbGemarkung.Text
        initFlureCombo()
        cmbFlur.IsEnabled = True
        cmbFlur.IsDropDownOpen = True
    End Sub
    Sub initFlureCombo()
        'gemeindeDT
        DB_Oracle_sharedfunctions.holeFlureDT()
        cmbFlur.DataContext = myGlobalz.sitzung.postgresREC.dt
    End Sub

    'Private Sub cmbFlur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbFlur.SelectionChanged
    '    Dim item2 As DataRowView = CType(cmbFlur.SelectedItem, DataRowView)
    '    If item2 Is Nothing Then Exit Sub

    '    cmbZaehler.IsEnabled = True
    '    Dim item3$ = item2.Row.ItemArray(0).ToString
    '    tbflur.Text = item2.Row.ItemArray(0).ToString
    '    'Me.tbStrasse.Text=item4
    '    myGlobalz.sitzung.aktFST.normflst.flur = CInt(item3$)
    '    ' myGlobalz.sitzung.aktFST.normflst.gemarkungstext = Me.tbGemarkung.Text
    '    initZaehlerCombo()
    '    cmbZaehler.IsDropDownOpen = True
    'End Sub
    'Sub initZaehlerCombo()
    '    'gemeindeDT
    '    DB_Oracle_sharedfunctions.holeZaehlerDT()
    '    cmbZaehler.DataContext = myGlobalz.sitzung.postgresREC.dt
    'End Sub

    Private Sub cmbZaehler_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbZaehler.SelectionChanged
        Dim item2 As DataRowView = CType(cmbZaehler.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        cmbNenner.IsEnabled = True
        tbZaehler.Text = item2.Row.ItemArray(0).ToString
        'Me.tbStrasse.Text=item4
        myGlobalz.sitzung.aktFST.normflst.zaehler = CInt(item3$)
        ' myGlobalz.sitzung.aktFST.normflst.gemarkungstext = Me.tbGemarkung.Text
        myGlobalz.sitzung.aktFST.normflst.nenner = Nothing
        initNennerCombo()
        If myGlobalz.sitzung.postgresREC.dt.Rows.Count = 1 Then
            '  MsgBox("nur ein Treffer: " & myGlobalz.sitzung.fstREC.dt.Rows(0).Item(0).ToString)
            tbNenner.Text = myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0).ToString
            NennerVerarbeiten()
            cmbFunktionsvorschlaege.IsDropDownOpen = True
        Else
            cmbNenner.IsDropDownOpen = True
        End If
    End Sub

    Sub initNennerCombo()
        'gemeindeDT
        DB_Oracle_sharedfunctions.holeNennerDT()
        cmbNenner.DataContext = myGlobalz.sitzung.postgresREC.dt
    End Sub

    Private Sub NennerVerarbeiten()
        'Dim fst_id% = 0
        myGlobalz.sitzung.aktFST.normflst.nenner = CInt(tbNenner.Text)
        FST_tools.nennerUndFSPruefen()
        tbCoords.Text = String.Format("{0},{1}", myGlobalz.sitzung.aktFST.punkt.X, myGlobalz.sitzung.aktFST.punkt.Y)
       ' tbFreitext.Text = String.Format("{0} qm", myGlobalz.sitzung.aktFST.normflst.flaecheqm)
        tbarea.Text = CStr(myGlobalz.sitzung.aktFST.normflst.flaecheqm)
        lblFS.Text = myGlobalz.sitzung.aktFST.normflst.FS
        btnSpeichernFlurstueck.IsEnabled = True
    End Sub

    Private Sub cmbNenner_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbNenner.SelectionChanged
        Dim item2 As DataRowView = CType(cmbNenner.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Try
            'Dim item3$ = item2.Row.ItemArray(0).ToString
        Catch ex As Exception
            Exit Sub
        End Try
        tbNenner.Text = item2.Row.ItemArray(0).ToString
        'Me.tbStrasse.Text=item4
        NennerVerarbeiten()
        cmbFunktionsvorschlaege.IsDropDownOpen = True
    End Sub

    Private Sub CheckBox1_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles CheckBox1.Checked
        If Not glob2.isfreieTexteingabeOK Then
            CheckBox1.IsChecked = False
            Exit Sub
        End If
        tbGemarkung.IsEnabled = True
        tbflur.IsEnabled = True
        tbZaehler.IsEnabled = True
        tbNenner.IsEnabled = True
        tbCoords.Text = "0,0"
        myGlobalz.sitzung.aktFST.punkt.X = 0
        myGlobalz.sitzung.aktFST.punkt.Y = 0
    End Sub
    Shared Function FLST_EingabeistOK() As Boolean
        If Not IsNumeric(myGlobalz.sitzung.aktFST.normflst.flur) Then
            MessageBox.Show("Die Flurnummer muss numerisch sein! Texte werden nicht akzeptiert!")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktFST.normflst.zaehler) Then
            MessageBox.Show("Die Zählernummer muss numerisch sein! Texte werden nicht akzeptiert!")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktFST.normflst.nenner) Then
            MessageBox.Show("Die Nennernummer muss numerisch sein! Texte werden nicht akzeptiert!")
            Return False
        End If
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktFST.normflst.gemarkungstext) Then
            MessageBox.Show("Sie müssen eine Gemarkung angeben!")
            Return False
        End If
        Return True
    End Function

    Private Shared Function istVerwandtesFlurstueck() As Boolean
        If CInt(myGlobalz.sitzung.aktFST.Status) = 1 Then
            MsgBox("Es handelt sich um den Raumbezug eines 'Verwandten Vorgangs'. Er kann nicht geändert werden!")
            Return True
        Else
            Return False
        End If
    End Function

    Sub flurstueck_speichern()
        Try
            If istVerwandtesFlurstueck() Then Exit Sub
            If Not FLST_EingabeistOK() Then Exit Sub
            If Not istFLSTEingabe_vorhanden() Then Exit Sub
            btnSpeichernFlurstueck.IsEnabled = False
            FST_tools.flurstueck_speichernExtracted(Radius, CBool(chkMapenabled.IsChecked))
        Catch ex As Exception
            nachricht("Fehler in flurstueck_speichern:  ", ex)
        End Try
    End Sub



    Shared Function istFLSTEingabe_vorhanden() As Boolean
        Return True
    End Function


    Private Sub tbGemarkung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbGemarkung.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        glob2.istTextzulang(145, tbGemarkung)
    End Sub

    'Private Sub btnSpeichernFlurstueck_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    flurstueck_speichern()
    'End Sub

    Private Sub tbflur_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbflur.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        'glob2.pruefeObZahl(tbflur)
    End Sub

    Private Sub tbZaehler_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbZaehler.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        'glob2.pruefeObZahl(tbZaehler)
    End Sub

    Private Sub tbNenner_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbNenner.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        'glob2.pruefeObZahl(tbNenner)
    End Sub

    Private Sub lblFS_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles lblFS.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        glob2.istTextzulang(21, lblFS)
        If btnEigentuemerALKIS IsNot Nothing Then
            btnEigentuemerALKIS.IsEnabled = True
        End If

    End Sub

    Private Sub tbCoords_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbCoords.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        glob2.istTextzulang(1200, tbKurz)
    End Sub

    Private Sub tbKurz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbKurz.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        glob2.istTextzulang(900, tbKurz)
    End Sub

    'Private Sub tbKurzd_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
    '    anyChange = True
    '    glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
    'End Sub
    Private Shared ReadOnly Property Radius() As Double
        Get
            Return CDbl(initP.getValue("MiniMap.radiusAdresse"))
        End Get
    End Property

    Private Sub Window_Flurstuecksauswahl_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult

        If btnSpeichernFlurstueck.IsEnabled Then
            red = MessageBox.Show("Sie haben Daten in dieser Maske geändert! " & vbCrLf &
               "Wenn Sie diese Änderungen " & vbCrLf &
               " - prüfen und ggf. speichern möchten wählen Sie 'JA'" & vbCrLf &
               " - verwerfen möchten wählen Sie 'Nein'" & vbCrLf &
               "Prüfen und abspeichern ?",
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

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        anyChange = False
        btnSpeichernFlurstueck.IsEnabled = False
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnLoeschenEreignis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschenEreignis.Click
        loeschenRBFlurstueck()
        e.Handled = True
    End Sub

    Sub loeschenRBFlurstueck()
        If Not glob2.istloeschenErnstgemeint() Then Exit Sub
        FST_tools.loeschenRBFlurstueckExtracted()
        Me.Close()
        ' todo: shapeFile: shape loeschen
    End Sub

    Private Sub btnSpeichernFlurstueck_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichernFlurstueck.Click
        flurstueck_speichern()
        e.Handled = True
    End Sub

    'Private Sub btnEigentuemer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    eigentuemerALBalt()
    '    e.Handled = True
    'End Sub

    Private Sub cmbFunktionsvorschlaege_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbFunktionsvorschlaege.SelectedItem Is Nothing Then Exit Sub
        'Dim myvali$ = CStr(cmbFunktionsvorschlaege.SelectedValue)
        Dim myvalx = CType(cmbFunktionsvorschlaege.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        If tbKurz IsNot Nothing Then tbKurz.Text = myvals
        e.Handled = True
    End Sub


    Private Sub btnEigentuemerALKIS_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnEigentuemerALKIS.Content = "Bitte warten ..."
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        'FST_tools.eigentuemerALKIS(myGlobalz.sitzung.aktFST, CBool(chkInsArchiv.IsChecked), CBool(chkEreignisMap.IsChecked), myGlobalz.sitzung.aktFST.defineAbstract)
        btnEigentuemerALKIS.Content = "start"
        e.Handled = True
    End Sub



    Private Sub btnFSTausCSVliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        startCSVListenImport()
        e.Handled = True
    End Sub

    Private Sub startCSVListenImport()
        Me.Close()
        Dim csvlisteerstellen As New WinCsvliste
        csvlisteerstellen.ShowDialog()

    End Sub

    Private Sub btnWardawas_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim datei As String = IO.Path.Combine(myGlobalz.Arc.lokalerCheckoutcache, "altevoraenge.txt")
        Dim erfolg As Boolean = FST_tools.AlleSekidsZuDiesermFlurstueckholen("flst")
        If erfolg Then
            AdressTools.schreibeVorgangslisteInDatei(datei)
            glob2.OpenDocument(datei)
        Else
            Dim messagetext As String = "Es wurden keine weiteren Vorgänge auf dieser Adresse gefunden! " & Environment.NewLine
            MessageBox.Show(messagetext)
        End If
        e.Handled = True
    End Sub

    Private Sub chkInsArchiv_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub chkEreignisMap_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub chkMapenabled_Checked(sender As Object, e As RoutedEventArgs) Handles chkMapenabled.Checked
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        e.Handled = True
    End Sub

    Private Sub chkMapenabled_Click(sender As Object, e As RoutedEventArgs)
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        e.Handled = True
    End Sub

    Private Sub tbFreitext_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbFreitext.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichernFlurstueck)
        glob2.istTextzulang(450, tbFreitext)
    End Sub
End Class
