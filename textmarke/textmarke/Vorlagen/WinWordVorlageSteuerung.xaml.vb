Public Class WinWordVorlageSteuerung

    Private localCacheOutfile As String = ""
    Private loklist As List(Of Person)
    Private beteiligterDict As Dictionary(Of String, String)
    Private liegenschaftenDict As Dictionary(Of String, String)
    Private typdict As Dictionary(Of String, Integer)
    Private fluerstuecktext As String
    Public Property RaumbezugsID_Auswahl As New List(Of raumbezugsauswahl)
    Public Property quelldatei As String
    Property quellpfad As String
    Private auswahlPID As Integer ', RBID%
    Private FormIstGeladen As Boolean = False
    Private _akteZiehenmodus As Boolean = False
    Private dokumentBookmarks As New List(Of String)
    Private Property _eid As Integer

    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub
    Private Sub WinVorlageSteuerung_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        VorlagenStartroutine()
        Title = StammToolsNs.setWindowTitel.exe("edit", "Vorlagen")
        gastLayout()
        btnOk.Visibility = Visibility.Collapsed
        btnabbruch.Visibility = Visibility.Collapsed

        Dim quelle As String = quellpfad & "\" & localCacheOutfile & "\" & quelldatei
        quelle = quelle.Replace("\\", "\")
        Dim schlagworte, dateityp, dateiVorname As String
        dateiVorname = quelldatei
        dateityp = LIBgemeinsames.clsString.getDateiEndung(quelldatei)
        'Dim bbb As New clsVorlagedokumente
        Dim erfolg = clsVorlagedokumente.DocxVorlageVerwenden(localCacheOutfile, quelle, schlagworte, quellpfad, dateityp, dateiVorname)
        Dim aaa As New wordInterop
        dokumentBookmarks = aaa.getBookmarksFromDocument(localCacheOutfile)
        dokumentBookmarks = aaa.cleanBookmarksFromDocument(dokumentBookmarks)
        aaa._dokumentbookmarks.Clear()

        For Each ele As String In dokumentBookmarks
            aaa._dokumentbookmarks.Add(ele)
        Next
        aaa = Nothing
        tbDokumentBookmarks.ToolTip = makeDokumentBookmarksString()
        cmbVerlaufAuswahl.IsDropDownOpen = True
        FormIstGeladen = True
    End Sub

    Private Function makeDokumentBookmarksString() As String
        Dim summe As String = ""
        Try
            l(" MOD ---------------------- anfang")
            For Each ele As String In dokumentBookmarks
                summe = summe & ele & ", " & Environment.NewLine
            Next
            Return summe
            l(" MOD ---------------------- ende")
            Return summe
        Catch ex As Exception
            l("Fehler in MOD: " & ex.ToString())
            Return summe
        End Try
    End Function

    Private Sub VorlagenStartroutine()
        Try
            If _eid > 0 Then
                chkMiteereignisanlegen.IsChecked = False
                grpEreignisanlegen.IsEnabled = False
            End If
            tbSchlagworte.Text = schlagworteAusDateinamenbilden()

            clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(myGlobalz.sitzung.aktVorgangsID)
            initComboAdressaten(myGlobalz.sitzung.aktVorgangsID)
            '  ' initErgeinistypCombo()
            detailsTools.initErgeinistypCombo(Me, "Vorlage_ereignisseTypen.xml", "XMLSourceComboBoxEreignisse")
            detailsTools.initErgeinistypCombo(Me, "Vorlage_DateiVorname.xml", "XMLSourceComboBoxDateivorname")
            detailsTools.initErgeinistypCombo(Me, "vorlage_DokumentSchlagworte.xml", "XMLSourceComboBoxDokumentSchlagworte")
            tbTextmarken.Text = "Adressaten initialisiert ..." & Environment.NewLine
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            cmbAdressat.ItemsSource = beteiligterDict
            tbQuelldatei.DataContext = quelldatei

            liegenschaftenDict = New Dictionary(Of String, String)
            typdict = New Dictionary(Of String, Integer)

            clsVorlagenTools.getliegenschaften(liegenschaftenDict, typdict)
            tbTextmarken.Text &= "Liegenschaften initialisiert ..." & Environment.NewLine
            If _eid = 0 Then
                initEreignisTitel()
            End If
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            cmbLiegenschaft.ItemsSource = liegenschaftenDict
            If _akteZiehenmodus Then
                chkMiteereignisanlegen.IsChecked = False
                cmbModus.SelectedIndex = 0 '"unverbindliche Vorschau"
                cmbAdressat.SelectedIndex = 0
                cmbLiegenschaft.SelectedIndex = 0
            End If
        Catch ex As Exception
            nachricht("fehler Es wurden keine Adressaten gefunden!!!", ex)
        End Try
    End Sub

    Private Function schlagworteAusDateinamenbilden() As String
        Dim schlagw As String
        Try
            schlagw = quelldatei.ToLower.Replace(".docx", "").Replace(".doc", "")
            schlagw = schlagw.Replace("_", " ").Replace("-", " ").Replace(":", " ").Replace(",", " ").Replace(":", " ").Replace("  ", " ")
            Return schlagw
        Catch ex As Exception
            nachricht("fehler schlagworteAusDateinamenbilden", ex)
            Return ""
        End Try
    End Function

    Private Sub initEreignisTitel()
        tbEreignisTitel.Text = quelldatei.Replace(".docx", "").Replace(".doc", "").Replace(".rtf", "") ' neuername$
    End Sub
    Sub New(ByVal _quelldatei As String, ByVal _pfad As String, akteZiehenmodus As Boolean, ereignisid As Integer)
        InitializeComponent()
        quelldatei = _quelldatei
        quellpfad = _pfad
        _akteZiehenmodus = akteZiehenmodus
        _eid = ereignisid
    End Sub

    Function initComboAdressaten(ByVal vid As Integer) As Boolean 'myGlobalz.sitzung.VorgangsID
        loklist = clsVorlagenTools.holePersonenliste(vid)
        beteiligterDict = New Dictionary(Of String, String)()
        If loklist IsNot Nothing Then
            For i = 0 To loklist.Count - 1
                beteiligterDict.Add(CStr(loklist(i).PersonenID), String.Format("{0}: {1}, {2}", loklist(i).Rolle, loklist(i).Name, loklist(i).Vorname))
            Next
        Else
            nachricht("Es wurden keine Adressaten gefunden!!!")
        End If
        Return True
    End Function



    Private Sub cmbModus_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If Not FormIstGeladen Then Exit Sub
        nachricht("not implemented: cmbModus_SelectionChanged")
        If cmbModus.SelectedIndex = 0 Then
            chkMiteereignisanlegen.IsChecked = False
        Else
            chkMiteereignisanlegen.IsChecked = True
        End If
        e.Handled = True
    End Sub

    Private Sub btnWeiter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnWeiter.Click
        e.Handled = True
        If tbSchlagworte.Text.IsNothingOrEmpty Then
            tbSchlagworte.Text = ""
        End If
        If chkMiteereignisanlegen.IsChecked And
            (tbEreignisArt.Text.Trim.IsNothingOrEmpty()) Then
            MsgBox("Sie müssen dem Ereignis einen Typ zuweisen!!! Abbruch.", MsgBoxStyle.Critical, "Ereignis anlegen")
            cmbVerlaufAuswahl.IsDropDownOpen = True

            Exit Sub
        Else
            weiter(dokumentBookmarks)
        End If
        e.Handled = True
    End Sub

    Private Function GetInsarchiv() As Boolean
        Dim insarchiv As Boolean
        If cmbModus.SelectedIndex = 0 Then
            insarchiv = False
        Else
            insarchiv = True
        End If
        Return insarchiv
    End Function

    Private Sub UIaender()
        stckPanel.Visibility = Visibility.Collapsed
        btnWeiter.Visibility = Visibility.Collapsed
        Button2.Visibility = Visibility.Collapsed
        tbTextmarken.Visibility = Visibility
        btnOk.Visibility = Visibility
        btnabbruch.Visibility = Visibility.Visible
    End Sub

    Sub weiter(documentbookmarks As List(Of String))
        Dim ereignisanlegen As Boolean = True
        nachricht("WinVorlageSteuerung weiter: ")
        myGlobalz.sitzung.textmarkeLiegenschaft = tbLiegenschaft.Text
        tbTextmarken.Text = "Adressat bestimmen. Bitte warten ..." & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

        nachricht("WinVorlageSteuerung weiter: 0")
        If Not adressat_bestimmen(myGlobalz.sitzung.aktPerson) Then
            myGlobalz.sitzung.aktPerson.clear()
        End If
        nachricht("WinVorlageSteuerung weiter: 1")
        tbTextmarken.Text &= "Liegenschaft bestimmen. Bitte warten ..." & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

        nachricht("WinVorlageSteuerung weiter: 2")

        If myGlobalz.sitzung.textmarkeLiegenschaft Is Nothing Then myGlobalz.sitzung.textmarkeLiegenschaft = ""
        UIaender()
        nachricht("WinVorlageSteuerung weiter: 3")
        Dim insarchiv As Boolean = GetInsarchiv()
        nachricht("insarchiv: " & insarchiv)
        Dim resultstring$ = ""

        'Dim ereignisid As Integer
        If chkMiteereignisanlegen.IsChecked Then
            ereignisanlegen = True
            _eid = myGlobalz.sitzung.aktEreignis.ID
        Else
            ereignisanlegen = False
            '  _eid = _eid'myGlobalz.sitzung.aktEreignis.ID
        End If
        nachricht("WinVorlageSteuerung weiter: 4")

        tbTextmarken.Text &= "Werte für Textmarken ermitteln. Bitte warten ..." & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dim replaceTextMarkenDict As New Dictionary(Of String, String)
        If String.IsNullOrEmpty(fluerstuecktext) Then fluerstuecktext = ""


        nachricht("WinVorlageSteuerung weiter: vor holeRBdatenFuerTextmarken")

        holeRBdatenFuerTextmarken()

        nachricht("WinVorlageSteuerung weiter: vor TM_fuelleMarkenDictionary")
        clsVorlagedokumente.TM_fuelleMarkenDictionary(replaceTextMarkenDict, fluerstuecktext, documentbookmarks)
        nachricht("WinVorlageSteuerung weiter: vor TM_ausgebenMarkenDictionary")
        clsVorlagedokumente.TM_ausgebenMarkenDictionary(replaceTextMarkenDict)


        tbTextmarken.Text &= "Vorlage kopieren und vorbereiten. Bitte warten ..." & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents


        nachricht("WinVorlageSteuerung weiter: vor prepareSelectetedVorlageDokument")
        Dim tauschergebnis As String = ""
        Dim endung As String = LIBgemeinsames.clsString.getDateiEndung(quelldatei)
        Dim vorname As String
        vorname = tbDateiVorname.Text.Trim

        Dim erfolg As Boolean = clsVorlagenTools.prepareSelectetedVorlageDokument(insarchiv, quelldatei, localCacheOutfile, resultstring,
                                                                                  replaceTextMarkenDict, ereignisanlegen, tbSchlagworte.Text,
                                                                                  quellpfad, tbEreignisArt.Text, tbEreignisTitel.Text,
                                                                                  tauschergebnis,
                                                                                  endung, _eid, vorname, dokumentBookmarks)
        myGlobalz.sitzung.aktDokument.Beschreibung = tbSchlagworte.Text

        nachricht("WinVorlageSteuerung weiter: 7")
        If erfolg Then
            'jetzt checkin
            If insarchiv Then
                _eid = clsVorlagedokumente.vorlageEreignisAnlegen(ereignisanlegen, tbEreignisArt.Text, tbEreignisTitel.Text, _eid)

                'Dim NumDir As String = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
                erfolg = myGlobalz.Arc.checkINDoku(myGlobalz.sitzung.aktDokument.FullnameCheckout,
                                         myGlobalz.sitzung.aktDokument.Beschreibung,
                                         _eid,
                                         False,
                                         myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
                                         "",
                                         myGlobalz.sitzung.aktVorgangsID,
                                         False, Now, myGlobalz.sitzung.aktDokument.DocID,
                                         myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                         myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                         myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID)
                If erfolg Then
                    myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                    'vdatei = myGlobalz.sitzung.aktDokument.FullnameImArchiv
                    'outdatei = myGlobalz.sitzung.aktDokument.FullnameImArchiv
                    'Return True
                    tbTextmarken.Text = String.Format("Fertig{0}{1}", Environment.NewLine, makeDokumentBookmarksString()) '
                    'Dokument wurde geändert
                    btnOk.Visibility = Visibility.Visible
                    MessageBox.Show("Die Datei wurde in das Archiv übernommen. (siehe Reiter 'Dokumente')", "Vorlagendatei übernommen", MessageBoxButton.OK, MessageBoxImage.Information)
                Else
                    nachricht_und_Mbox("Fehler beim Speichern im Archiv!")
                    'Return False
                End If
            Else
                'nicht ins archiv
                glob2.OpenDocument(myGlobalz.sitzung.aktDokument.FullnameCheckout)
            End If
        Else
            tbTextmarken.Text = "Probleme bei der Bearbeitung der Vorlage !!!"
            Button2.Visibility = Visibility.Visible
            Button2.IsEnabled = True
            'nachricht_und_Mbox("Fehler bei der Verarbeitung der Vorlage")
        End If
        nachricht("WinVorlageSteuerung weiter: ENDE")
    End Sub

    Function adressat_bestimmen(ByRef adressat As Person) As Boolean 'myGlobalz.sitzung.aktPerson
        Try
            nachricht("adressat_bestimmen: ")
            For i = 0 To loklist.Count - 1
                If loklist(i).PersonenID = auswahlPID Then
                    adressat = loklist(i)
                    nachricht("adressat_bestimmen:  erfolgreich")
                    Return True
                End If
            Next
            nachricht("adressat_bestimmen: nicht erfolgreich")
            Return False
        Catch ex As Exception
            nachricht("Fehler adressat_bestimmen: " ,ex)
            Return False
        End Try
    End Function

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub cmbAdressat_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Dim selob As New KeyValuePair(Of String, String)
        selob = CType(cmbAdressat.SelectedItem, KeyValuePair(Of String, String))
        Dim selKey$ = selob.Key
        Dim selvalue$ = selob.Value
        auswahlPID = CInt(selob.Key)
    End Sub

    Private Sub cmbLiegenschaft_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If cmbLiegenschaft.SelectedItem Is Nothing Then Exit Sub

            Dim selob As New KeyValuePair(Of String, String)
            selob = CType(cmbLiegenschaft.SelectedItem, KeyValuePair(Of String, String))
            'Dim selKey$ = selob.Key
            'Dim selvalue$ = selob.Value
            Dim temp$ = setNeuenLiegenschaftstextBox(CInt(selob.Key), selob.Value)
            Dim RBauswahlitem As New raumbezugsauswahl
            RBauswahlitem.rbid = CInt(clsDBtools.fieldvalue(myglobalz.sitzung.raumbezugsRec.dt.Rows(CInt(selob.Key)).Item("raumbezugsid")))
            RBauswahlitem.typ = CInt(clsDBtools.fieldvalue(myglobalz.sitzung.raumbezugsRec.dt.Rows(CInt(selob.Key)).Item("typ")))
            RBauswahlitem.sekid = CInt(clsDBtools.fieldvalue(myglobalz.sitzung.raumbezugsRec.dt.Rows(CInt(selob.Key)).Item("sekid")))
            RBauswahlitem.abstract = CStr(clsDBtools.fieldvalue(myglobalz.sitzung.raumbezugsRec.dt.Rows(CInt(selob.Key)).Item("abstract")))
            RaumbezugsID_Auswahl.Add(RBauswahlitem)

            If String.IsNullOrEmpty(tbLiegenschaft.Text) Then
                tbLiegenschaft.Text = temp
                If temp.Contains(", Flur:") Then fluerstuecktext = temp
            Else
                tbLiegenschaft.Text = String.Format("{0}, {1}", tbLiegenschaft.Text, temp)
                If temp.Contains(", Flur:") Then fluerstuecktext = temp
            End If
            cmbLiegenschaft.SelectedItem = Nothing
        Catch ex As Exception
            nachricht("cmbLiegenschaft_SelectionChanged: " ,ex)
        End Try
    End Sub

    Shared Function setNeuenLiegenschaftstextBox(ByVal laufnr%, ByVal text As String) As String
        nachricht("setNeuenLiegenschaftstextBox  -------------------------")
        'prüfen ob es ein typ 1 (adresse ) ist
        'nur auf adresse anwenden
        Dim funk$ = "", abstrakt$ = "", str$ = "", gemeinde$ = ""
        Dim temp$ = text
        Dim typ% = CInt(clsDBtools.fieldvalue(myglobalz.sitzung.raumbezugsRec.dt.Rows(laufnr).Item("typ")))
        ' myGlobalz.sitzung.textmarkeLiegenschaft = text$ 'clsDBtools.fieldvalue(myGlobalz.sitzung.raumbezugsRec.dt.Rows(RBID).Item("abstract")) 'standardwert
        Try
            If typ = 1 Then
                If text.Contains(":") Then
                    Dim a$() = text.Split(":"c)
                    funk = a(0).Trim
                    abstrakt = a(1).Trim
                Else
                    funk = ""
                    abstrakt = text
                End If
                If abstrakt.Contains(",") Then
                    Try
                        Dim a$() = abstrakt.Split(","c)
                        str = a(1).Trim
                        gemeinde = a(0).Trim
                    Catch ex As Exception
                        nachricht("Fehler 1 beim splitten eines liegenschaft-abstracts: " & text)
                    End Try
                End If
                temp = String.Format("{0}, {1}", str, gemeinde)

            End If
            Return temp.Trim
            nachricht("temp: " & temp)
        Catch ex As Exception
            nachricht("Fehler 2 beim splitten eines liegenschaft-abstracts: " & text)
            Return "Fehler"
        End Try
    End Function

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnOk.Click
        e.Handled = True
        btnOk.IsEnabled = False
        btnabbruch.Content = "Fertig"
        If cmbModus.SelectedIndex = 0 Then
            glob2.OpenDocument(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        Else
            MsgBox("Das Dokument wurde erstellt und der Reiterliste 'Dokumente' hinzugefügt.")
        End If
        Close()
    End Sub

    Private Sub btnabbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Me.Close()
    End Sub

    Private Sub btnClear_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        tbLiegenschaft.Text = ""
        RaumbezugsID_Auswahl.Clear()
        e.Handled = True
    End Sub

    Private Sub holeRBdatenFuerTextmarken()
        myglobalz.sitzung.aktADR.clear() : myglobalz.sitzung.aktFST.clear()
        For Each rbauswahlItem As raumbezugsauswahl In RaumbezugsID_Auswahl
            If rbauswahlItem.typ = RaumbezugsTyp.Adresse Then
                'paraadresseHolen
                RBtoolsns.RB_Adresse_holen_by_ID_alleDB.exe(CInt(rbauswahlItem.sekid))
                If AdressTools.DTaufAdressObjektabbilden(myglobalz.sitzung.tempREC.dt, myglobalz.sitzung.aktADR) Then
                    nachricht("adresse auf textmarke abbilden erfolgreich")
                Else
                    nachricht("adresse auf textmarke abbilden NICHT erfolgreich")
                End If
                'datenzwischenspeicehrn auf  myGlobalz.sitzung.aktADR
            End If
            If rbauswahlItem.typ = RaumbezugsTyp.Flurstueck Then
                'paraflurstueckHolen
                RBtoolsns.RB_Flurstueck_holen_alleDB.exe(CStr((rbauswahlItem.sekid)))
                FST_tools.DTaufFSTObjektabbilden(myglobalz.sitzung.tempREC.dt, myglobalz.sitzung.aktFST)
                'datenzwischenspeicehrn
            End If
        Next
    End Sub

    Private Sub tbTestmarkenliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        clsVorlagenTools.zeigeTextmarkenListe()
    End Sub

    Private Sub cmbVerlaufAuswahl_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        e.Handled = True
        If Not FormIstGeladen Then Exit Sub
        Try
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            If String.IsNullOrEmpty(item) Then Exit Sub
            tbEreignisArt.Text = item & " "
            tbDateiVorname.Text = item.Trim.Replace(" ", "_")
            cmbVerlaufAuswahl.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" ,ex)
        End Try
    End Sub

    Private Sub cmbDokumentTypen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If Not FormIstGeladen Then Exit Sub
            If cmbDokumentTypen.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbDokumentTypen.SelectedValue, String)
            If String.IsNullOrEmpty(item) Then Exit Sub
            'tbSchlagworte.Text = tbSchlagworte.Text & " " & item & " "
            tbSchlagworte.Text = item & " "
            '    tbDateiVorname.Text = (item & " ").Replace(" ", "_")
            cmbDokumentTypen.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" ,ex)
        End Try
    End Sub

    Private Sub cmbDateiVorname_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            If Not FormIstGeladen Then Exit Sub
            If cmbDateiVorname.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbDateiVorname.SelectedValue, String)
            tbDateiVorname.Text = (item.Replace(" ", "_") & " ").Trim
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbDateiVorname_SelectionChanged" ,ex)
        End Try
    End Sub

    Private Sub tbDokumentBookmarks_MouseMove(sender As Object, e As MouseEventArgs)
        e.Handled = True

    End Sub
End Class

Public Class raumbezugsauswahl
    Public Property rbid%
    Public Property sekid%
    Public Property abstract$ = ""
    Public Property typ%
End Class
