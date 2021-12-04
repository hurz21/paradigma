Imports System.Data
Partial Public Class Window_RB_Adresse
    Private Property anyChange As Boolean = False

    Private Shared Property erfolg As Boolean

    Private Sub Window_RB_Adresse_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
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

    Private Sub starteAdressForm()
        initGemeindeCombo()
        initFunktionCombo()
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            myGlobalz.sitzung.aktADR.clear()
            btnLoeschenEreignis.IsEnabled = False
            cmbGemeinde.IsDropDownOpen = True
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            btnLoeschenEreignis.IsEnabled = True
            If myGlobalz.sitzung.aktADR.punkt.X > 1000 Then btnWindrosen.IsEnabled = True
            chkMapenabled.IsChecked = CBool(myGlobalz.sitzung.aktADR.isMapEnabled)
        End If
        btnSpeichern.IsEnabled = False
        anyChange = False
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.raumbezugsmodus, "Raumbezug: Adresse")
        'cmbStrasse.IsEnabled = False
        'cmbHausnr.IsEnabled = False
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemeinden.xml")
    End Sub

    Sub initFunktionCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxRBfunktion"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\RBfunktion.xml")
    End Sub

    'Private Sub Label4_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.KeyEventArgs)
    '    nachricht_und_Mbox("karte")
    'End Sub

    'Private Sub Label4_PreviewMouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    nachricht_und_Mbox("Label4_PreviewMouseDoubleClick")
    'End Sub

    'Private Sub Label4_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    nachricht_und_Mbox("Label4_MouseDoubleClick")
    'End Sub

    'Private Sub TextBox1_MouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
    '    nachricht_und_Mbox("TextBox1_MouseDown")
    'End Sub


    Private Sub Hyperlink_RequestNavigate(ByVal sender As System.Object, ByVal e As  _
     System.Windows.Navigation.RequestNavigateEventArgs)
        'Dim a = myGlobalz.sitzung.aktADR.punkt
        Dim gis As New clsGISfunctions
        gis.mittelpunktsAufruf(myGlobalz.sitzung.aktADR.punkt, initP.getValue("GisServer.GIS_WebServer"))
        e.Handled = True
    End Sub

    'Private Sub Hyperlink_RequestNavigateEigentumer(ByVal sender As System.Object, ByVal e As  _
    ' System.Windows.Navigation.RequestNavigateEventArgs)
    '    glob2.send_Shellbatch("paradigma_" & myGlobalz.sitzung.Bearbeiter.Initiale & "_.pdf", _
    '                            myGlobalz.sitzung.aktADR.FS)
    '    e.Handled = True
    'End Sub

    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbGemeinde.SelectionChanged
        If cmbGemeinde.SelectedItem Is Nothing Then Exit Sub
        gemeindechanged()
        e.Handled = True
    End Sub

    Sub gemeindechanged()
        Dim myvali As String = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals As String = myvalx.Attributes(1).Value.ToString
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali) - 438000


        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali) 'item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = myvals
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = myvals
        If myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = 438000 Then
            tbGemeinde.Text = ""
            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = ""
            myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = ""
            MsgBox("Geben Sie Gemeinde, Strasse und Hausnummer nun von Hand ein!")
            'freie strasseneingabe ermöglichen
            'hausnummer eingabe ermöglichen
            'koordinaten werden beim speichern über google ermittelt
            tbGemeinde.IsEnabled = True : tbGemeinde.Background = Brushes.PaleVioletRed
            tbHausnr.IsEnabled = True : tbHausnr.Background = Brushes.PaleVioletRed
            tbStrasse.IsEnabled = True : tbStrasse.Background = Brushes.PaleVioletRed
            FocusManager.SetFocusedElement(Me, tbGemeinde)
            myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "fehlt"
        Else
            myGlobalz.sitzung.aktADR.PLZ = (glob2.getPLZfromGemeinde(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName)) 'ddr
            myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PLZ = myGlobalz.sitzung.aktADR.PLZ
            stckBuchstaben1.IsEnabled = True
            stckBuchstaben2.IsEnabled = True
            tbGemeinde.Text = myvals ' item2.Row.ItemArray(1).ToString
        End If
    End Sub


    Sub initStrassenCombo(buchstabe As String)
        'gemeindeDT
        Dim strassseninstanz As New List(Of AdressTools.strassenUiObj)
        Dim att As New AdressTools
        att.buildStrassenListe(buchstabe, strassseninstanz)
        cmbStrasse.DataContext = strassseninstanz 'myGlobalz.sitzung.haloREC.dt
    End Sub

    Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStrasse.SelectionChanged
        Try
            Dim auswahlStrasse As AdressTools.strassenUiObj = CType(cmbStrasse.SelectedItem, AdressTools.strassenUiObj)
            If auswahlStrasse Is Nothing Then Exit Sub
            If auswahlStrasse.quelle = "mehr" Then Exit Sub

            If auswahlStrasse.quelle = "fehlt" Then
                cmbStrasse_SelectionChangedExtracted()
                Exit Sub
            End If

            If auswahlStrasse.quelle = "lage" Then
                tbStrasse.Text = auswahlStrasse.sname
                'keine hausnummern anzeigen
                'hausnummer eingabe ermöglichen
                'koordinaten werden beim speichern über google ermittelt
                MsgBox("Geben Sie die evtl. Hausnummer nun von Hand ein!" & Environment.NewLine & Environment.NewLine &
                        "Wenn sie die Nr nciht genau wissen geben sie einfach >0< ein!")
                myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "lage"
                myGlobalz.sitzung.aktADR.Gisadresse.strasseName = tbStrasse.Text
                tbHausnr.Text = "0" : tbHausnr.IsEnabled = True : tbHausnr.Background = Brushes.PaleVioletRed
                FocusManager.SetFocusedElement(Me, tbHausnr)
                Exit Sub
            End If
            If auswahlStrasse.quelle = "halo" Then
                myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "halo"
                tbStrasse.Text = auswahlStrasse.sname
                myGlobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(auswahlStrasse.strcode)
                myGlobalz.sitzung.aktADR.Gisadresse.strasseName = tbStrasse.Text
                initHausNRCombo()
                '   cmbHausnr.IsEnabled = False
                Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                cmbHausnr.IsDropDownOpen = True
            End If
        Catch ex As Exception
            nachricht("Fehler in cmbStrasse_SelectionChanged: " & ex.ToString)
        End Try
    End Sub

    Private Sub cmbStrasse_SelectionChangedExtracted()
        MsgBox("Geben Sie Strasse und Hausnummer nun von Hand ein!")
        'freie strasseneingabe ermöglichen
        'hausnummer eingabe ermöglichen
        'koordinaten werden beim speichern über google ermittelt
        If tbHausnr.Text.IsNothingOrEmpty Then tbHausnr.Text = "0"
        
        tbHausnr.IsEnabled = True : tbHausnr.Background = Brushes.PaleVioletRed
        tbStrasse.IsEnabled = True : tbStrasse.Background = Brushes.PaleVioletRed
        FocusManager.SetFocusedElement(Me, tbStrasse)
        myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "fehlt"
    End Sub
    Sub initHausNRCombo()
        DB_Oracle_sharedfunctions.DBholeHausnrDT()
        Dim alist As New List(Of hausnrOBJ)
        Dim hausnrinstanz As hausnrOBJ

        hausnrinstanz = New hausnrOBJ
        hausnrinstanz.id = -1
        hausnrinstanz.hausnrkombi = "!fehlt!"
        alist.Add(hausnrinstanz)

        For i = 0 To myGlobalz.sitzung.haloREC.dt.Rows.Count - 1
            hausnrinstanz = New hausnrOBJ
            hausnrinstanz.id = CInt(myGlobalz.sitzung.haloREC.dt.Rows(i).Item(0))
            hausnrinstanz.hausnrkombi = CStr(myGlobalz.sitzung.haloREC.dt.Rows(i).Item(1))
            alist.Add(hausnrinstanz)
        Next

        cmbHausnr.DataContext = alist 'myGlobalz.sitzung.haloREC.dt
    End Sub

    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbHausnr.SelectionChanged
        cmbHausnr_SelectionChangedExtracted()
        e.Handled = True
    End Sub

    Private Sub cmbHausnr_SelectionChangedExtracted()
        Dim item2 As hausnrOBJ = CType(cmbHausnr.SelectedItem, hausnrOBJ)
        If item2 Is Nothing Then Exit Sub

        tbHausnr.Text = item2.hausnrkombi
        Dim halo_id% = CInt(item2.id)

        If tbHausnr.Text = "!fehlt!" Then
            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = "0"
            myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "fehlt"
            tbHausnr.IsEnabled = True
            tbHausnr.Background = Brushes.PaleVioletRed
            FocusManager.SetFocusedElement(Me, tbHausnr)
        Else
            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text
            glob2.hole_AdressKoordinaten(halo_id)
            If myGlobalz.sitzung.aktADR.punkt.X < 1 Or myGlobalz.sitzung.aktADR.punkt.Y < 1 Then
                MsgBox("Ein Fall für Google")
            End If
            lblFS.Text = myGlobalz.sitzung.aktADR.FS
            lblCoords.Content = myGlobalz.sitzung.aktADR.punkt.X & " , " & myGlobalz.sitzung.aktADR.punkt.Y
            btnWindrosen.IsEnabled = True
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            cmbFunktionsvorschlaege.IsDropDownOpen = True
        End If
    End Sub

    Private Sub ckbFreieAdresseingabe_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbFreieAdresseingabe.Checked
        If Not glob2.isfreieTexteingabeOK() Then
            ckbFreieAdresseingabe.IsChecked = False
            Exit Sub
        End If
        tbGemeinde.IsEnabled = True
        tbStrasse.IsEnabled = True
        tbHausnr.IsEnabled = True
        lblCoords.Content = "0,0"
        myGlobalz.sitzung.aktADR.punkt.X = 0
        myGlobalz.sitzung.aktADR.punkt.Y = 0
        btnGoogleKoordinaten.Background = New SolidColorBrush(Colors.Red)
    End Sub

    Private Sub tbKurzbeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbKurzbeschreibung.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(900, tbKurzbeschreibung)
    End Sub



    'Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    adressToolsUI.Adresse_speichern(Me, btnSpeichern, tbGemeinde, tbStrasse, tbHausnr)
    '    e.Handled = True
    'End Sub

    Private Sub tbHausnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbHausnr.TextChanged
        If String.IsNullOrEmpty(tbGemeinde.Text) Then Exit Sub
        If String.IsNullOrEmpty(tbStrasse.Text) Then Exit Sub
        anyChange = True
        glob2.istTextzulang(40, tbGemeinde)
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub


    'Private Sub btnSpeichern_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    adressToolsUI.Adresse_speichern(Me, btnSpeichern, tbGemeinde, tbStrasse, tbHausnr)
    '    e.Handled = True
    'End Sub

    'Private Sub btnSpeichern_Click_2(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    adressToolsUI.Adresse_speichern(Me, btnSpeichern, tbGemeinde, tbStrasse, tbHausnr)
    '    e.Handled = True
    'End Sub



    Private Sub tbGemeinde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbGemeinde.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbGemeinde)
    End Sub

    Private Sub tbStrasse_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbStrasse.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(140, tbGemeinde)
    End Sub

    Private Sub lblFS_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles lblFS.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        '	glob2.istTextzulang(21, lblFS)
    End Sub


    Private Sub tbPLZ_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPLZ.TextChanged
        anyChange = True
        '  If glob2.pruefeObZahl(tbPLZ) Then
        glob2.schliessenButton_einschalten(btnSpeichern)
        'End If
    End Sub

    Private Sub tbPostfach_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbPostfach.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(45, tbPostfach)
    End Sub

    'Private Sub cmbRolleAdresse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    nachricht("not implemented:  cmbRolleAdresse_SelectionChanged()")
    '    'Dim item As String = CType(cmbRolleAdresse.SelectedIndex, String)
    '    'myGlobalz.sitzung.aktADR.Adresstyp = CType(item, adressTyp)
    '    'glob2.schliessenButton_einschalten(btnSpeichern)
    'End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        anyChange = False
        btnSpeichern.IsEnabled = False
        Me.Close()
    End Sub


    Private Sub btnLoeschenEreignis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschenEreignis.Click
        AdressTools.loeschenRBAdresse()
        Me.Close()
    End Sub

    Private Sub Window_RB_Adresse_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        starteAdressForm()
    End Sub

    Private Sub btnGoogleKoordinaten_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnGoogleKoordinaten_ClickExtracted(True)
        e.Handled = True
    End Sub


    Public Sub btnGoogleKoordinaten_ClickExtracted(oeffneGoogleMaps As Boolean)
        If Not AdressTools.eingabeOK() Then Exit Sub
        nachricht("btnGoogleKoordinaten_ClickExtracted Eingabe ist ok")
        Dim erfolg As Boolean
        nachrichtenzeile.Text = "Eingabe ist ok"
        Dim googlepunkt As clsGEOPoint
        erfolg = AdressTools.AdresseInGoogleKoordinatenUmrechnen(googlepunkt, myGlobalz.sitzung.aktADR.Gisadresse, myGlobalz.sitzung.aktADR.PLZ)
        If erfolg Then
            lblCoordsGoogle.Content = GoogleKoordinatenDarstellen(googlepunkt)
            ckbFreieAdresseingabe.IsChecked = True
            GoogleKoordinatenAlsTextUebernehmen_wenn_Freitext(googlepunkt)
            If oeffneGoogleMaps Then AdresseInGoogleMapsDarstellen()
            btnWindrosen.IsEnabled = True
            btnSpeichern.IsEnabled = True
        Else
            lblCoords.Content = "Google-Adressauflösung nicht möglich!"
            nachricht("Google-Adressauflösung nicht möglich! ")
        End If
    End Sub


    Private Sub GoogleKoordinatenAlsTextUebernehmen_wenn_Freitext(ByRef googlepunkt As clsGEOPoint)
        If ckbFreieAdresseingabe.IsChecked Then
            AdressTools.GoogleKoordinaten_uebernehmenNachAdresse(googlepunkt, myGlobalz.sitzung.aktADR)
            lblCoords.Content = googlepunkt.GKrechts & ", " & googlepunkt.GKhoch
        End If
    End Sub

    Private Function GoogleKoordinatenDarstellen(ByVal googlepunkt As clsGEOPoint) As String
        Dim neuresult As String = "GoogleKoordinaten: " & googlepunkt.GKrechts & ", " & googlepunkt.GKhoch
        nachricht("nach getgooglecoordinatenMitUmrechnung: " & neuresult$)
        Return neuresult
    End Function

    Private Shared Sub AdresseInGoogleMapsDarstellen()
        MessageBox.Show("Bitte prüfen Sie anhand der GoogleMaps-Darstellung die Qualität der Koordinate." &
                        "Sie sollten die Koordinate nur verwenden, wenn Sie sicher auch den Ort wiedergibt." &
                          Environment.NewLine & Environment.NewLine &
                        "Bitte warten Sie einen Moment bis GoogleMaps startet.")

        Dim googl$ = LIBgoogle.GmapsApi3.GeoData.Googleadress(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName, myGlobalz.sitzung.aktADR.Gisadresse.strasseName, "", "")
        Process.Start(googl)
    End Sub

    Private Sub btnWindrosen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        adressToolsUI.windRoseAnzeigen(myGlobalz.sitzung.aktADR.punkt.X, myGlobalz.sitzung.aktADR.punkt.Y)
        e.Handled = True
    End Sub

    Private Sub cmbFunktionsvorschlaege_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbFunktionsvorschlaege.SelectedItem Is Nothing Then Exit Sub
        'Dim myvali$ = CStr(cmbFunktionsvorschlaege.SelectedValue)
        Dim myvalx = CType(cmbFunktionsvorschlaege.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        If tbKurzbeschreibung IsNot Nothing Then tbKurzbeschreibung.Text = myvals
        btnWardawas.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub btnEigentuemer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        AdressTools.eigentuemer_fuer_adresse_holen(myGlobalz.sitzung.aktADR, CBool(chkInsArchiv.IsChecked), CBool(chkEreignisMap.IsChecked))
    End Sub



    Private Sub btnWardawas_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse"
        btnWardawas_ClickExtracted()
        e.Handled = True
    End Sub

    Private Shared Sub btnWardawas_ClickExtracted()
        Dim datei As String = IO.Path.Combine(myGlobalz.Arc.lokalerCheckoutcache, "altevoraenge.txt")
        erfolg = bestandTools.zeigeVorgaenge.VorgaengeAnzeigenFuerHausnummerExtracted()
        If erfolg Then
            AdressTools.schreibeVorgangslisteInDatei(datei)
            glob2.OpenDocument(datei)
        Else
            Dim messagetext As String = "Es wurden keine weiteren Vorgänge auf dieser Adresse gefunden! " & Environment.NewLine
            MessageBox.Show(messagetext)
        End If
    End Sub

    Private Sub tbFreitext_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbFreitext.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(500, tbFreitext)
    End Sub

    Private Sub btnBingMaps_Click_1(sender As Object, e As RoutedEventArgs)
        AdresseInBingapsDarstellen()
        e.Handled = True
    End Sub

    Private Shared Sub AdresseInBingapsDarstellen()
        Dim bing As String = makeBingAdressstring(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName, myGlobalz.sitzung.aktADR.Gisadresse.strasseName)
        Process.Start(bing)
    End Sub

    Private Shared Function makeBingAdressstring(gemeinde As String, strasse As String) As String
        'http://www.bing.com/maps/default.aspx?where1=1 Werner Hilpert dietzenbach
        Dim rumpf As String = "http://www.bing.com/maps/default.aspx?where1=" & gemeinde & " " & strasse & "&style=o&lvl=1&dir=0"
        Return rumpf
    End Function

    Private Sub btnstreet(sender As Object, e As RoutedEventArgs)
        Dim a As String = (sender.ToString)
        a = a.Replace("System.Windows.Controls.Button:", "")
        a = a.Trim.ToLower
        'strasseninit mit und ohne hausnummer
        '  MsgBox(a)
        initStrassenCombo(a)
        cmbStrasse.IsDropDownOpen = True
        e.Handled = True
    End Sub



    Private Sub chkInsArchiv_Checked(sender As Object, e As RoutedEventArgs) Handles chkInsArchiv.Checked
        e.Handled = True
    End Sub

    Private Sub chkEreignisMap_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub chkInsArchiv_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub btnSpeichern_Click_3(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichern.Click
        adressToolsUI.Adresse_speichern(Me, btnSpeichern, tbGemeinde, tbStrasse, tbHausnr, CBool(chkMapenabled.IsChecked))
        e.Handled = True
    End Sub

    Private Sub chkMapenabled_Checked(sender As Object, e As RoutedEventArgs) Handles chkMapenabled.Checked
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub clicked(sender As Object, e As RoutedEventArgs) Handles chkMapenabled.Click
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub btnStrassefehlt(sender As Object, e As RoutedEventArgs)
        cmbStrasse_SelectionChangedExtracted()
        
        e.Handled = True
    End Sub
End Class
