Imports System.Data
Partial Public Class Window_RB_Adresse
    Private Property anyChange As Boolean = False
    'Property RESULT_text As String
    'Private Property RESULT_dateien As New List(Of gisresult)
    'Property RESULT_text_NSG As String
    ''Private Property RESULT_dateien_NSG As New List(Of gisresult)

    'Property RESULT_text_WSG As String
    'Private Property RESULT_dateien_WSG As New List(Of gisresult)

    'Property RESULT_text_Bplan As String
    'Private Property RESULT_dateien_Bplan As New List(Of gisresult)

    'Property RESULT_text_kehr As String
    'Private Property RESULT_dateien_Kehr As New List(Of gisresult)

    'Property RESULT_text_ueg As String
    'Private Property RESULT_dateien_ueg As List(Of gisresult)

    'Private Shared Property erfolg As Boolean
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
        'setGisanalyse()
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show(glob2.getMsgboxText("MaskeGeaendert", New List(Of String)(New String() {})),
                                    "Ereignisdetails",
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
        Width = 600
        initGemeindeCombo()
        initFunktionCombo()
        'Gisanalysesetzen()
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            myGlobalz.sitzung.aktADR.clear()
            btnLoeschenEreignis.IsEnabled = False
            cmbGemeinde.IsDropDownOpen = True
            btnPufferbilden.IsEnabled = False
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            btnLoeschenEreignis.IsEnabled = True
            If myGlobalz.sitzung.aktADR.punkt.X > 1000 Then btnWindrosen.IsEnabled = True
            chkMapenabled.IsChecked = CBool(myGlobalz.sitzung.aktADR.isMapEnabled)
            btnPufferbilden.IsEnabled = True
            Me.DataContext = myGlobalz.sitzung
        End If

        tbKurzbeschreibung.Text = myGlobalz.sitzung.aktADR.Name
        tbFreitext.Text = myGlobalz.sitzung.aktADR.Freitext
        btnSpeichern.IsEnabled = False
        tbHausnr.Text = myGlobalz.sitzung.aktADR.Gisadresse.HausKombi
        anyChange = False

        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.raumbezugsmodus, "Raumbezug: Adresse")
        'cmbStrasse.IsEnabled = False
        'cmbHausnr.IsEnabled = False
    End Sub
    'Private Sub Gisanalysesetzen()
    '    Try
    '        Dim a As String = CLstart.myc.userIniProfile.WertLesen("GISANALYSE", "alleausfuehren")
    '        If a.IsNothingOrEmpty Then
    '            chkGisanalyse.IsChecked = True
    '        End If
    '        If a.IsNothingOrEmpty Then a = "0"
    '        If CInt(a) = 0 Then
    '            chkGisanalyse.IsChecked = False
    '        Else
    '            chkGisanalyse.IsChecked = True
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in Gisanalysesetzen1: " ,ex)
    '        chkGisanalyse.IsChecked = True
    '    End Try
    'End Sub

    'Private Sub setGisanalyse()
    '    Try
    '        If chkGisanalyse.IsChecked Then
    '            CLstart.myc.userIniProfile.WertSchreiben("GISANALYSE", "alleausfuehren", "1")
    '        Else
    '            CLstart.myc.userIniProfile.WertSchreiben("GISANALYSE", "alleausfuehren", "0")
    '        End If
    '    Catch ex As Exception
    '        nachricht("fehler in setMitDokumenten: " ,ex)
    '    End Try
    'End Sub

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


    'Private Sub Hyperlink_RequestNavigate(ByVal sender As System.Object, ByVal e As  _
    ' System.Windows.Navigation.RequestNavigateEventArgs)
    '    'Dim a = myGlobalz.sitzung.aktADR.punkt
    '    Dim gis As New clsGISfunctions
    '    gis.WebGISmittelpunktsAufruf(myGlobalz.sitzung.aktADR.punkt, initP.getValue("GisServer.GIS_WebServer"))
    '    e.Handled = True
    'End Sub

    'Private Sub Hyperlink_RequestNavigateEigentumer(ByVal sender As System.Object, ByVal e As  _
    ' System.Windows.Navigation.RequestNavigateEventArgs)
    '    glob2.send_Shellbatch("paradigma_" & myGlobalz.sitzung.Bearbeiter.Initiale & "_.pdf", _
    '                            myGlobalz.sitzung.aktADR.FS)
    '    e.Handled = True
    'End Sub

    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbGemeinde.SelectionChanged
        e.Handled = True
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
        cmbStrasse.DataContext = strassseninstanz
    End Sub

    Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbStrasse.SelectionChanged
        e.Handled = True
        Try
            Dim auswahlStrasse As AdressTools.strassenUiObj = CType(cmbStrasse.SelectedItem, AdressTools.strassenUiObj)
            If auswahlStrasse Is Nothing Then Exit Sub
            If auswahlStrasse.quelle = "mehr" Then Exit Sub
            auswahlStrasse.sname = auswahlStrasse.sname.Trim
            auswahlStrasse.quelle = auswahlStrasse.quelle.Trim
            auswahlStrasse.strcode = auswahlStrasse.strcode.Trim.Replace("""", "")

            If auswahlStrasse.quelle = "fehlt" Then
                cmbStrasse_SelectionChangedExtracted()
                Exit Sub
            End If

            If auswahlStrasse.quelle = "lage" Then
                tbStrasse.Text = auswahlStrasse.sname
                'keine hausnummern anzeigen
                'hausnummer eingabe ermöglichen
                'koordinaten werden beim speichern über google ermittelt
                MsgBox(glob2.getMsgboxText("hausnrVonHandEingaben", New List(Of String)(New String() {})))
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
            nachricht("Fehler in cmbStrasse_SelectionChanged: " ,ex)
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
        hausnrinstanz.id = "-1"
        hausnrinstanz.hausnrkombi = "!fehlt!"
        hausnrinstanz.rechts = "0"
        hausnrinstanz.hoch = "0"
        hausnrinstanz.gemeindetext = ""
        hausnrinstanz.gemarkungstext = ""
        hausnrinstanz.fs = ""
        alist.Add(hausnrinstanz)

        For i = 0 To myGlobalz.sitzung.postgresREC.dt.Rows.Count - 1
            hausnrinstanz = New hausnrOBJ
            hausnrinstanz.id = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(0))
            hausnrinstanz.hausnrkombi = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(1)).Trim
            hausnrinstanz.rechts = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(4))
            hausnrinstanz.hoch = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(5))
            hausnrinstanz.gemeindetext = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(6))
            hausnrinstanz.gemarkungstext = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(7))
            hausnrinstanz.fs = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(8))
            alist.Add(hausnrinstanz)
        Next

        cmbHausnr.DataContext = alist
    End Sub

    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbHausnr.SelectionChanged
        e.Handled = True
        cmbHausnr_SelectionChangedExtracted()
    End Sub

    Private Sub cmbHausnr_SelectionChangedExtracted()
        Dim item2 As hausnrOBJ = CType(cmbHausnr.SelectedItem, hausnrOBJ)
        If item2 Is Nothing Then Exit Sub

        tbHausnr.Text = item2.hausnrkombi
        Dim halo_id As String = CStr(item2.id)

        If tbHausnr.Text = "!fehlt!" Then
            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = "0"
            myGlobalz.sitzung.aktADR.Gisadresse.Quelle = "fehlt"
            tbHausnr.IsEnabled = True
            tbHausnr.Background = Brushes.PaleVioletRed
            FocusManager.SetFocusedElement(Me, tbHausnr)
        Else
            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text.Trim
            myGlobalz.sitzung.aktADR.Gisadresse.GKrechts = CInt(item2.rechts)
            myGlobalz.sitzung.aktADR.Gisadresse.GKhoch = CInt(item2.hoch)

            myGlobalz.sitzung.aktADR.punkt.X = CDbl(item2.rechts)
            myGlobalz.sitzung.aktADR.punkt.Y = CDbl(item2.hoch)

            myGlobalz.sitzung.aktADR.FS = CStr(item2.fs)
            'glob2.hole_AdressKoordinaten(halo_id)
            If myGlobalz.sitzung.aktADR.punkt.X < 1 Or myGlobalz.sitzung.aktADR.punkt.Y < 1 Then
                MsgBox("Ein Fall für Google")
            End If
            lblFS.Text = myGlobalz.sitzung.aktADR.FS
            lblCoords.Content = myGlobalz.sitzung.aktADR.punkt.X & " , " & myGlobalz.sitzung.aktADR.punkt.Y
            btnWindrosen.IsEnabled = True
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            cmbFunktionsvorschlaege.IsDropDownOpen = True
            'If CBool(chkGisanalyse.IsChecked) Then gisanalyse()' temporär raus bis geklärt
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
        myGlobalz.sitzung.aktADR.Name = tbKurzbeschreibung.Text
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
        myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text
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
        e.Handled = True

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
        e.Handled = True

        anyChange = False
        btnSpeichern.IsEnabled = False
        Me.Close()
    End Sub


    Private Sub btnLoeschenEreignis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschenEreignis.Click
        e.Handled = True
        btnSpeichern.IsEnabled = False
        AdressTools.loeschenRBAdresse()
        Me.Close()
    End Sub

    Private Sub Window_RB_Adresse_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        starteAdressForm()
        gastLayout()
    End Sub

    Private Sub btnGoogleKoordinaten_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        btnGoogleKoordinaten_ClickExtracted(True)
    End Sub


    Public Sub btnGoogleKoordinaten_ClickExtracted(oeffneGoogleMaps As Boolean)

        If myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName.IsNothingOrEmpty Then
            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = tbGemeinde.Text.Trim
        End If
        If myGlobalz.sitzung.aktADR.Gisadresse.strasseName.IsNothingOrEmpty Then
            myGlobalz.sitzung.aktADR.Gisadresse.strasseName = tbStrasse.Text.Trim
        End If
        If myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.IsNothingOrEmpty Then
            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text.Trim

        End If
        myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.Trim
        If Not AdressTools.eingabeOK() Then Exit Sub
        nachricht("btnGoogleKoordinaten_ClickExtracted Eingabe ist ok")
        Dim erfolg As Boolean
        nachrichtenzeile.Text = "Eingabe ist ok"
        Dim googlepunkt As clsGEOPoint = Nothing
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
        'btnWardawas.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub btnEigentuemer_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        AdressTools.eigentuemer_fuer_adresse_holen(myglobalz.sitzung.aktADR, CBool(chkInsArchiv.IsChecked), CBool(chkEreignisMap.IsChecked))
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
        adressToolsUI.Adresse_speichern(Me, btnSpeichern, tbGemeinde, tbStrasse, tbHausnr, tbKurzbeschreibung.Text, tbFreitext.Text, CBool(chkMapenabled.IsChecked))
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

    Private Sub btnPufferbilden_Click(sender As Object, e As RoutedEventArgs)
        Dim pufferinMeter As Double = CDbl(tbpufferinmeter.Text)
        Dim puffererzeugt As Boolean = FST_tools.bildePufferFuerPunkt(myGlobalz.sitzung.aktADR.punkt, pufferinMeter)
        GC.Collect()
        If puffererzeugt Then
            MsgBox("Das Puffer-Objekt wurde erzeugt und unter 'Raumbezüge' abgelegt.")
        End If
        btnSpeichern.IsEnabled = False
        Close()
        e.Handled = True
    End Sub

    Private Sub btnWardawas_Click(sender As Object, e As RequestNavigateEventArgs)
        e.Handled = True
        AdressTools.warDaWasAdresse(False)
    End Sub

    Private Sub BtnstarteGIS_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim newrange As New clsRange
        newrange.point2range(myGlobalz.sitzung.aktADR.punkt, 200)
        mgistools.startenGIS(newrange)

    End Sub



    'Private Sub btnKehraufruf_Click(sender As Object, e As RoutedEventArgs)
    '    'MsgBox()
    '    tbInfo.Text = RESULT_text_kehr
    '    e.Handled = True
    'End Sub

    'Private Sub zwischenInfo(text As String)
    '    Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
    '    tbInfo.Text &= text & Environment.NewLine
    '    Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
    'End Sub

    'Private Sub gisanalyse()
    'Dim Plannr As String = ""
    'Dim neupunkt As New clsGEOPoint : neupunkt.X = myGlobalz.sitzung.aktADR.punkt.X : neupunkt.Y = myGlobalz.sitzung.aktADR.punkt.Y
    'If NSpostgis.clsPostgis.getBplanInfo4point(neupunkt, RESULT_dateien_Bplan, Plannr, RESULT_text_Bplan) Then
    '    'If NSpostgis.clsPostgis.getBplanInfo4Adress(myGlobalz.sitzung.aktADR, RESULT_dateien_Bplan, Plannr, RESULT_text_Bplan) Then
    '    btnbplanaufruf.IsEnabled = True
    '    tbbplangueltig.Text = "B-Plan: " & Plannr
    '    tbbplangueltig.Background = Brushes.LightGreen
    '    BPLANverordnungsdatei = RESULT_dateien_Bplan.Item(0).datei.FullName.Trim
    'Else
    '    btnbplanaufruf.IsEnabled = False
    '    tbbplangueltig.Text = Plannr
    '    tbbplangueltig.Text = "kein Bplan"
    'End If

    'zwischenInfo("B-Plan fertig")

    'Dim bezirk As String = ""
    'If NSpostgis.clsPostgis.getKehrbezirkInfo4point(neupunkt, RESULT_text_kehr, bezirk) Then
    '    'If NSpostgis.clsPostgis.getKehrbezirkInfo4Adress(myGlobalz.sitzung.aktADR, RESULT_text_kehr, bezirk) Then
    '    btnKehraufruf.IsEnabled = True
    '    KEHRbeschreibung = RESULT_text_kehr
    '    tbKehrgueltig.Text = bezirk
    '    tbKehrgueltig.Background = Brushes.LightGreen
    'Else
    '    btnKehraufruf.IsEnabled = False
    '    tbKehrgueltig.Text = "keine Info"
    'End If
    'zwischenInfo("Kehrbezirk fertig")


    'Dim fs As String = ""
    'If NSpostgis.clsPostgis.getFS4coordinates(myGlobalz.sitzung.aktADR.punkt.X, myGlobalz.sitzung.aktADR.punkt.Y, fs) Then
    '    Dim eigSDB As New clsEigentuemerschnell
    '    Dim dt As DataTable = Nothing
    '    Dim mycount As Integer
    '    eigSDB.oeffneConnectionEigentuemer()
    '    If eigSDB.getEigentuemerdata(fs, eigentumerKurzinfo, Eigentuemernameundadresse, mycount, dt) Then
    '        tbEigentgueltig.Text = Eigentuemernameundadresse
    '        tbEigentgueltig.Background = Brushes.LightGreen
    '        btnEigentaufruf.IsEnabled = True
    '    Else
    '        btnEigentaufruf.IsEnabled = False
    '        tbEigentgueltig.Text = "keine Info"
    '    End If
    'End If
    'zwischenInfo("Eigentümer fertig")

    'Dim sgnr As String = ""
    'If NSpostgis.clsPostgis.getNSGInfo4Adress(myGlobalz.sitzung.aktADR, RESULT_dateien_NSG, sgnr) Then
    '    btnNSGaufruf.IsEnabled = True
    '    btnNSGaufruf.ToolTip = "zur nichtamtlichen Verordnung"
    '    tbNSGgueltig.Text = "NSG/GLB/LSG"
    '    NSGverordnungsdatei = RESULT_dateien_NSG.Item(0).datei.FullName
    '    NSGbeschreibung = RESULT_dateien_NSG.Item(0).dateibeschreibung
    '    tbNSGgueltig.Background = Brushes.LightGreen
    'Else
    '    btnNSGaufruf.IsEnabled = False
    '    tbNSGgueltig.Text = sgnr
    '    tbNSGgueltig.Text = "kein NSG/LSG/GLB"
    'End If

    'zwischenInfo("NSG/LSG/GLB Gebiete fertig")


    'Dim uenr As String = ""
    'If NSpostgis.clsPostgis.getUEGebietInfo4Adress(myGlobalz.sitzung.aktADR, RESULT_dateien_ueg, uenr) Then
    '    btnUESGaufruf.IsEnabled = True
    '    btnUESGaufruf.ToolTip = "zur nichtamtlichen Verordnung"
    '    tbUESGgueltig.Text = "Ü-Gebiet"
    '    tbUESGgueltig.Background = Brushes.LightGreen
    '    UEGverordnungsdatei = RESULT_dateien_ueg.Item(0).datei.FullName
    '    UEGbeschreibung = RESULT_dateien_ueg.Item(0).dateibeschreibung
    'Else
    '    btnUESGaufruf.IsEnabled = False
    '    tbUESGgueltig.Text = uenr
    '    tbUESGgueltig.Text = "kein Ü-Gebiet"
    'End If

    'zwischenInfo("Überschwemmungsgebiete fertig")

    '    Dim WSGnr As String = ""
    '    Dim WSGzusatz As String = ""
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
    '            WSGzusatz = ""
    '        End If
    '        If RESULT_dateien_WSG.Count > 1 Then
    '            btnWSGaufruf.Visibility = Windows.Visibility.Collapsed
    '            cmbWSGauswahl.Visibility = Windows.Visibility.Visible
    '            'WSGverordnungsdatei = RESULT_dateien.Item(0).datei.FullName
    '            'WSGbeschreibung = RESULT_dateien.Item(0).dateibeschreibung
    '            cmbWSGauswahl.DataContext = RESULT_dateien_WSG
    '            cmbWSGauswahl.SelectedIndex = 0
    '            WSGzusatz = " -> mehrere WSG-VO gültig!"
    '        End If
    '    Else
    '        btnWSGaufruf.IsEnabled = False
    '        tbWSGgueltig.Text = uenr
    '        tbWSGgueltig.Text = "kein WSG" & WSGzusatz
    '    End If
    '    zwischenInfo("WSG fertig")

    '    zwischenInfo("GIS-Analyse abgeschlossen")
    'End Sub

    'Private Sub btnEigentaufruf_Click(sender As Object, e As RoutedEventArgs)
    '    tbInfo.Text = eigentumerKurzinfo & " " & Eigentuemernameundadresse
    '    e.Handled = True
    'End Sub

    'Private Sub btnAlleGisanalysen_Click(sender As Object, e As RoutedEventArgs)
    '    'gisanalyse()
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



    'Private Sub cmbStrasse_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

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
    'Private Sub btnbplanaufruf_Click(sender As Object, e As RoutedEventArgs)
    '    e.Handled = True
    '    If BPLANverordnungsdatei.IsNothingOrEmpty Then
    '        MsgBox("Kein Bplan zur Adresse gefunden.")
    '    Else
    '        Process.Start(BPLANverordnungsdatei)
    '        tbInfo.Text = RESULT_text_Bplan
    '    End If
    'End Sub


    'Private Sub cmbWSGauswahl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    '    e.Handled = True
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
    Private Sub btnDossier_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim utmpt As New Point
        utmpt.X = myGlobalz.sitzung.aktADR.punkt.X
        utmpt.Y = myGlobalz.sitzung.aktADR.punkt.Y
        clsGISDossierPrep.StartGisDossierExtern(
                            utmpt,
                            253, 2, 2, 1, 1, 1, myGlobalz.sitzung.aktBearbeiter.username,
                            "umwelt", CType(myGlobalz.sitzung.aktVorgangsID, String),
                            myGlobalz.sitzung.aktFST.normflst.FS,
                            "punkt")
    End Sub

    Private Sub BtnLoeschenEreignis_Click_1(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub btnWardawasVerwandte_Click(sender As Object, e As RequestNavigateEventArgs)
        e.Handled = True
        AdressTools.warDaWasAdresse(True)
    End Sub
End Class
