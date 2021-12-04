Imports System.Data

Partial Public Class Window_Ereignis_Detail
    Private Property _typ$ = ""
    Private Property _oldErledigtvalue As Boolean = False
    Property oldNotiz As String = ""
    Property oldBeschreibung As String = ""
    Property rtftext As String = ""
    Property ereignistextBox As clsRichtextbox
    Public Property FormIstSchonGeladen As Boolean = False
    Private dokumenteRitemousekeypressed As Boolean
    Private readOnlyDoxsInTxtCrtlOeffnen As Boolean = False
    Private ALTES_Dokumentname As String = ""
    Private ALTES_uhrzeit As DateTime = #2/14/2014 9:35:04 AM#

    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
        dockMAIN.Background = myGlobalz.GetSecondBackground()
        grdKopf.Background = myGlobalz.GetSecondBackground()
    End Sub

    Sub New(ByVal typp As String, _readOnlyDoxsInTxtCrtlOeffnen As Boolean)
        'Owner = Application.Current.Windows(0)
        Dim speichereingangdatum As Date = myGlobalz.sitzung.aktEreignis.Datum
        InitializeComponent()
        _typ = typp
        myGlobalz.sitzung.aktEreignis.Datum = speichereingangdatum
        readOnlyDoxsInTxtCrtlOeffnen = _readOnlyDoxsInTxtCrtlOeffnen
    End Sub

    'Private Sub gastLayout()
    '    If glob2.userIstinGastModus Then
    '        Background = New SolidColorBrush(Colors.Red)
    '        dockMAIN.Background = New SolidColorBrush(Colors.Red)
    '        grdKopf.Background = New SolidColorBrush(Colors.Red)
    '    End If
    'End Sub


    Private Sub Window_Ereignis_Detail_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        initStartPositionOnScreen()
        _oldErledigtvalue = myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt
        gastLayout()
        starteEreignisForm()
        Title = detailsTools.settitle("Ereignis " & myGlobalz.sitzung.Ereignismodus)
        tbNotiz.FontSize = 15
        btnArchivDokusZuEreignisHinzufuegenSICHTBARmachen()
        oldNotiz = myGlobalz.sitzung.aktEreignis.Notiz
        oldBeschreibung = myGlobalz.sitzung.aktEreignis.Beschreibung
        initWindowMaxheight()
        'If Application.zweiteInstanz Then
        '    btnDokusZuEreignisHinzufuegen2.IsEnabled = False
        '    btnNachVorlageFuerEreignis.IsEnabled = False
        '    btnPDFScan.IsEnabled = False
        '    btnArchivDokusZuEreignisHinzufuegen.IsEnabled = False
        'End If
        NotizHoeheEinstellen()
        dockMAIN.DataContext = myGlobalz.sitzung
        tbNotiz.Text = myGlobalz.sitzung.aktEreignis.Notiz
        dateTimePicker1.Value = myGlobalz.sitzung.aktEreignis.Datum
        If myGlobalz.PumuckelVersion = 2 Then
            btnNachVorlageFuerEreignis.Visibility = Visibility.Collapsed
        End If
        FormIstSchonGeladen = True
    End Sub



    Private Function getdatenwurdengeaendert() As Boolean
        'If myGlobalz.sitzung.aktEreignis.istRTF Then
        '    'If rtb1.dataChanged Then
        '    '    Return True
        '    'Else
        '    '    Return False
        '    'End If 
        'Else
        If oldNotiz = myGlobalz.sitzung.aktEreignis.Notiz And oldBeschreibung = myGlobalz.sitzung.aktEreignis.Beschreibung Then
            Return False
        Else
            Return True
        End If
        'End If

    End Function

    Private Sub Window_Ereignis_Detail_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        Dim dokusInArbeit As Boolean = False
        Dim mesres As New MessageBoxResult
        Dim dokInArbeit As String = ""
        Debug.Print("" & myGlobalz.sitzung.aktEreignis.ID)
        dokusInArbeit = detailsTools.sindEreignisDokumenteImZugriff(myGlobalz.sitzung.aktEreignis.ID)
        nachricht("sindDokumenteImZugriff " & dokusInArbeit)
        If dokusInArbeit Then
            mesres = detailsTools.schliessenAbbrechen1(mesres, dokInArbeit)
            If Not myGlobalz.sitzung.wordDateiImEditModus.FullnameCheckout.IsNothingOrEmpty Then
                Dim quell As New IO.FileInfo(myGlobalz.sitzung.wordDateiImEditModus.FullnameCheckout)
                Dim backupdatei As String = detailsTools.BackupAnlegen(quell, myGlobalz.sitzung.wordDateiImEditModus.DocID)
            End If
            If Not myGlobalz.sitzung.excelDateiImEditModus.FullnameCheckout.IsNothingOrEmpty Then
                Dim quell As New IO.FileInfo(myGlobalz.sitzung.excelDateiImEditModus.FullnameCheckout)
                Dim backupdatei As String = detailsTools.BackupAnlegen(quell, myGlobalz.sitzung.excelDateiImEditModus.DocID)
            End If
            If mesres = MessageBoxResult.Yes Then
                l("fehler hier wurde was verworfen")

                e.Cancel = False
            Else
                e.Cancel = True
                Exit Sub
            End If
        Else

        End If

        'Dim geloeschtedatei As String = ""

        'If myglobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.DOC Then
        '    geloeschtedatei = myglobalz.sitzung.wordDateiImEditModus.tempEditDatei
        'End If
        'If myglobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.XLS Then
        '    geloeschtedatei = myglobalz.sitzung.excelDateiImEditModus.tempEditDatei
        'End If

        'Dim dokupdate As New winDokumentUpdateArchiv(geloeschtedatei)
        'dokupdate.Show()
        'erfolg = detailsTools.wurdenDokumenteGeaendert(geloeschtedatei)
        'If erfolg Then
        '    MessageBox.Show("Die geänderte Datei wurde erfolgreich ins Archiv übernommen. " & Environment.NewLine & Environment.NewLine,
        '                                "Übernahme der geänderten Datei ins Archiv",
        '                                MessageBoxButton.OK, MessageBoxImage.Information)
        '    detailsTools.OfficemerkerLoeschen(myglobalz.sitzung.aktDokument)
        'End If

        Dim datenwurdengeaendert As Boolean = False
        datenwurdengeaendert = getdatenwurdengeaendert()
        If datenwurdengeaendert Then
            red = MessageBox.Show(glob2.getMsgboxText("MaskeGeaendert", New List(Of String)(New String() {})),
                  "Ereignisdetails",
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Exclamation,
                  MessageBoxResult.OK)
            If Not red = MessageBoxResult.No Then
                'btnSpeichernEreignis.IsEnabled = False
                e.Cancel = True
            End If
            If Not red = MessageBoxResult.Yes Then
                'btnSpeichernEreignis.IsEnabled = False
                e.Cancel = False
            End If
        End If
        savePosition()
    End Sub
    Private Sub savePosition()
        Try
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winEreignisFormpositiontop", CType(Me.Top, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winEreignisFormpositionleft", CType(Me.Left, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winEreignisFormpositionwidth", CType(Me.ActualWidth, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winEreignisFormpositionheight", CType(Me.ActualHeight, String))
        Catch ex As Exception
            l("fehler in saveposition  windb" ,ex)
        End Try
    End Sub
    Private Sub initStartPositionOnScreen()
        If myGlobalz.nureinbildschirm Then Exit Sub
        Dim topval = (CLstart.formposition.getPosition("diverse", "winEreignisFormpositiontop", Me.Top))
        If topval < 0 Then
            Me.Top = 0
        Else
            Me.Top = topval
        End If
        Me.Left = CLstart.formposition.getPosition("diverse", "winEreignisFormpositionleft", Me.Left)
        Me.Width = CLstart.formposition.getPosition("diverse", "winEreignisFormpositionwidth", Me.Width)
        Me.Height = CLstart.formposition.getPosition("diverse", "winEreignisFormpositionheight", Me.Height)
    End Sub

    Private Sub setzeCheckBoxen()
        If myGlobalz.sitzung.aktEreignis.Art.ToLower = "ortstermin" Then
            ckbistortstermin.IsChecked = True
        Else
            ckbistortstermin.IsChecked = False
        End If
        'Warum gibt es keine Eigenschaft "istStellungnahme" as boolean ?
        'Warum gibt es keine Eigenschaft "istOrtstermin" as boolean ?
        'If myGlobalz.sitzung.aktEreignis.Then Then
        '    ckbiststellungnahme.IsChecked = True
        'Else
        '    ckbiststellungnahme.IsChecked = False
        'End If
        'If myGlobalz.sitzung.aktEreignis.Art.ToLower.StartsWith("stellung") Then
        '    ckbiststellungnahme.IsChecked = True
        'Else
        '    ckbiststellungnahme.IsChecked = False
        'End If

        If myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt Then
            ckberledigt.IsChecked = True
        Else
            ckberledigt.IsChecked = False
        End If

        If myGlobalz.sitzung.aktEreignis.Art.ToLower = "abgabe an bauaufsicht" Then
            ckbistAbgabeBauaufsicht.IsChecked = True
        Else
            ckbistAbgabeBauaufsicht.IsChecked = False
        End If
    End Sub

    Private Sub NotizRTFSichtbarkeit()
        If myGlobalz.sitzung.aktEreignis.istRTF Then
            tbNotiz.Visibility = Windows.Visibility.Collapsed
            'rtb1.Visibility = Windows.Visibility.Visible
            BtnGoRtf.Visibility = Windows.Visibility.Collapsed
        Else
            tbNotiz.Visibility = Windows.Visibility.Visible
            'rtb1.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub starteEreignisForm()
        refreshEreignisDokumenteListe() 'DokumentenArchiv.initDokumente4EreignisDatatable()
        'tabDokumente.Header = "Dokumente zu diesem Ereignis (" & myGlobalz.Arc.ArcRec.dt.Rows.Count & ")"
        'If myGlobalz.Arc.ArcRec.dt.Rows.Count > 0 Then tabDokumente.FontWeight = FontWeights.Black
        If myGlobalz.zuhause Then
            detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ_Zuhause.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
        Else

            detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
        End If
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseTitel.xml", "XMLSourceComboBoxTitel") : cmbTitelVorschlag.SelectedIndex = 0
        dgEreignisDokumente.DataContext = myGlobalz.Arc.ereignisDocDt
        '  myGlobalz.sitzung.aktEreignis.i
        If myGlobalz.sitzung.aktEreignis.Art = "undefiniert" Then
            tbART.IsEnabled = True
        End If

        NotizRTFSichtbarkeit()
        If myGlobalz.sitzung.Ereignismodus = "neu" Then
            Me.AllowDrop = False
            myGlobalz.sitzung.aktEreignis.clearValues()
            myGlobalz.sitzung.aktEreignis.Art = _typ
            myGlobalz.sitzung.aktEreignis.Datum = Now
            'EreignisDokumente.Visibility = Windows.Visibility.Hidden
            setForm_KeineDokus()
            If myGlobalz.sitzung.aktEreignis.Art = "Email-Ausgang" Then
                myGlobalz.sitzung.aktEreignis.Richtung = "Ausgang"
            End If
            If myGlobalz.sitzung.aktEreignis.Art.ToLower = "ortstermin" Then
                ckbistortstermin.IsChecked = True
            End If
            If myGlobalz.sitzung.aktEreignis.Art.ToLower.StartsWith("stellungnahme") Then
                ckbiststellungnahme.IsChecked = True
            End If
            If myGlobalz.sitzung.aktEreignis.Art.ToLower.StartsWith("abgabe an bauaufsicht") Then
                ckbistAbgabeBauaufsicht.IsChecked = True
            End If
            cmbVerlaufAuswahl.IsDropDownOpen = True
        End If

        If myGlobalz.sitzung.Ereignismodus = "edit" Then
            'btnLoeschenEreignis.IsEnabled = False
            setzeCheckBoxen()
            Me.AllowDrop = True
            spEreignisDokumente.Visibility = Windows.Visibility.Visible
            dgEreignisDokumente.Visibility = Windows.Visibility.Visible
            btnSpeichernEreignis.IsEnabled = False
            refreshEreignisDokumenteListe()
            If myGlobalz.Arc.ereignisDocDt.Rows.Count < 1 Then
                setForm_KeineDokus()
            Else
                setForm_MitDokus()
            End If
            Debug.Print(myGlobalz.sitzung.aktEreignis.Quelle)
            hinzuOptionen.Visibility = Windows.Visibility.Visible
            hinzuOptionen.Visibility = Windows.Visibility.Visible
            If myGlobalz.sitzung.aktEreignis.istRTF Then
                rtftext = NotizRtfInhaltLaden()
                BtnGoRtf.IsEnabled = False
            End If
        End If
        initCombos()
        initRichtextbox()
        'DatePicker1.SelectedDate = myGlobalz.sitzung.aktEreignis.Datum     
        dateTimePicker1.Value = myGlobalz.sitzung.aktEreignis.Datum
        'Uhrzeitsetzen()
        ' clsParadigmaRechte.buttons_schalten(btnSpeichernEreignis, btnLoeschenEreignis)
        FocusManager.SetFocusedElement(Me, tbBeschreibung)
        LocalParameterFiles.erzeugeParameterDateiAktvorgang_txt(False, False)
    End Sub

    Sub setForm_KeineDokus()
        dgEreignisDokumente.Visibility = Windows.Visibility.Collapsed
        btnEreignischeckout.Visibility = Windows.Visibility.Hidden
        btnRefresh.Visibility = Windows.Visibility.Hidden
        hinzuOptionen.Visibility = Windows.Visibility.Hidden
        'NotizHoeheEinstellen()
    End Sub

    Sub setForm_MitDokus()
        dgEreignisDokumente.Visibility = Windows.Visibility.Visible
        btnEreignischeckout.Visibility = Windows.Visibility.Visible
        btnRefresh.Visibility = Windows.Visibility.Visible
        setDatagridHeight()
        'NotizHoeheEinstellen()
    End Sub

    Private Sub setDatagridHeight()
        If myGlobalz.Arc.ereignisDocDt.Rows.Count < 7 Then
            dgEreignisDokumente.Height = (myGlobalz.Arc.ereignisDocDt.Rows.Count * 25) + 50
        Else
            dgEreignisDokumente.Height = 300
        End If
    End Sub

    Function speichernEreignis(ByVal zeitstempel As Date, rtftext As String) As Boolean
        Dim kompress As Boolean = False
        If myGlobalz.sitzung.Ereignismodus = "neu" Then
            If Not NEU_eingabenOk_Ereignis() Then Return False
            If Not NEUform2objok_Ereignis() Then Return False
            '    myGlobalz.sitzung.aktEreignis.Datum = clsEreignisTools.GetGesammelteDatumUhrzeit(CDate(DatePicker1.SelectedDate).Date, tbStunde.Text, tbMinute.Text)
            myGlobalz.sitzung.aktEreignis.Datum = CDate(dateTimePicker1.Value)
            Dim lResult As Boolean = clsEreignisTools.speichernEreignisExtracted(myGlobalz.sitzung.aktVorgangsID, CBool(ckbOutlook.IsChecked))
            If myGlobalz.sitzung.aktEreignis.istRTF Then
                'If (Environment.UserName.ToLower = "feinen_j") Then
                '    'ist unabhängig vom savemode
                '    kompress = True
                'Else
                kompress = False
                'End If
                EreignisRTFhelp.RTFdateispeichern(rtftext, myGlobalz.sitzung.aktVorgangsID,
                                                         myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir, kompress)
            End If
            myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = myGlobalz.sitzung.aktEreignis.Datum 'Now
            '   Return lBoolean
        End If
        If myGlobalz.sitzung.Ereignismodus = "edit" Then
            If Not NEU_eingabenOk_Ereignis() Then Return False

            If Not NEUform2objok_Ereignis() Then Return False
            'rtb1.dataChanged = False
            Dim zielvorgangsid As Integer = myGlobalz.sitzung.aktVorgangsID
            'myGlobalz.sitzung.aktEreignis.Datum = clsEreignisTools.GetGesammelteDatumUhrzeit(CDate(DatePicker1.SelectedDate).Date, tbStunde.Text, tbMinute.Text)

            myGlobalz.sitzung.aktEreignis.Datum = CDate(dateTimePicker1.Value)
            If Not clsEreignisTools.EDITobj2DBOk_Ereignis_alledb(myGlobalz.sitzung.aktEreignis.ID, zielvorgangsid, myGlobalz.sitzung.aktEreignis) Then
                Return False
            End If
            '   Dim kompress As Boolean = False
            'If (Environment.UserName.ToLower = "feinen_j") Then
            '    'ist unabhängig vom savemode
            '    kompress = True
            'Else
            kompress = False
            'End If

            If myGlobalz.sitzung.aktEreignis.istRTF Then

                EreignisRTFhelp.RTFdateispeichern(rtftext, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir, kompress)
            End If

            clsEreignisTools.setLetztesEreignisText(myGlobalz.sitzung.aktEreignis)
            If myGlobalz.sitzung.aktEreignis.Art = "Ortstermin" Then myGlobalz.sitzung.aktVorgang.Stammdaten.Ortstermin = True
            If myGlobalz.sitzung.aktEreignis.Art = "Stellungnahme" Then myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = True
            myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = myGlobalz.sitzung.aktEreignis.Datum


            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "STELLUNGNAHME")
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "ORTSTERMIN")
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LASTACTIONHEROE")
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LETZTEBEARBEITUNG")

            ' VSTTools.editStammdaten_alleDB.exe(zielvorgangsid, myGlobalz.sitzung.aktVorgang.Stammdaten)

            clsEreignisTools.fallsErledigtDannSpeichern(zielvorgangsid)
            glob2.ggfTerminNachOutlookUebernehmen(zielvorgangsid, myGlobalz.sitzung.aktEreignis, CBool(ckbOutlook.IsChecked))
            CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " & myGlobalz.sitzung.aktEreignis.Beschreibung & ": editiert" : CLstart.myc.aLog.log()
        End If

        fallsAelterAlsStammeingangsdatum()
        If clsEreignisTools.istErstmalsErledigt(_oldErledigtvalue, myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt) Then
            'If Not myGlobalz.Arc.ArcRec.dt.IsNothingOrEmpty Then
            '    If myGlobalz.Arc.ArcRec.DT.Rows.Count > 0 Then
            If glob2.UserFragenObNach_PDFA_Kopieren() Then
                alleWordDokusNachPdfaKopieren()
                ' refreshDokumente(myGlobalz.sitzung.aktVorgangsID)
            End If
            'clsEreignisTools.AlleDokumentenRevisionssicherMachen(myGlobalz.sitzung.aktVorgangsID,
            '      "Der Vorgang ist nun erledigt. Es bietet sich an, jetzt die " & vbCrLf &
            '      "dazugehörigen Dokumente  revisionssicher zu machen." & vbCrLf)
            '    End If
            'End If
        End If
        btnSpeichernEreignis.IsEnabled = False
        Return True
    End Function


    Sub fallsAelterAlsStammeingangsdatum()
        'hier muss festgestellt werden ob das ereignis wirklich auch das älteste Ereignis darstellt
        'nur dann darf die prüfung stattfinden
        If istAmAltesten(myGlobalz.sitzung.aktEreignis.Datum) Then
            If myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum <> myGlobalz.sitzung.aktEreignis.Datum Then
                nachricht("Eingangsdatum muss angepasst werden")
                myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum = myGlobalz.sitzung.aktEreignis.Datum
                '  fallsAelterAlsStammeingangsdatumExtracted()
                '   speichernEreignisStammdaten()
                'VSTTools.editStammdaten_alleDB.exe(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten)

                detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "EINGANG")
            Else
                nachricht("Eingangsdatum muss nicht angepasst werden")
            End If
        Else
            nachricht("Eingangsdatum muss nicht angepasst werden weil es nicht am ältesten ist!")
        End If
    End Sub

    Function istAmAltesten(ByVal testdatum As DateTime) As Boolean
        nachricht("in istAmAltesten testdatum: " & vbCrLf & testdatum)
        Dim tdat As New DateTime
        For Each eitem As System.Data.DataRow In myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
            tdat = clsDBtools.fieldvalueDate(eitem.Item(3))
            nachricht("   tdat: " & tdat)
            If tdat < testdatum Then
                nachricht(String.Format("in istAmAltesten false. weil  tdat: {0}{1} ist älter", vbCrLf, tdat))
                Return False
            End If
        Next
        nachricht("in istAmAltesten true. ")
        Return True
    End Function

    Function NEU_eingabenOk_Ereignis() As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktEreignis.Beschreibung) Then
            MessageBox.Show("Sie müssen einen Titel eingeben!" & vbCrLf &
             "Warum ? " & vbCrLf &
             "Weil der Titel das Ereignis in der Verlaufsliste besonders kennzeichnet!")
            Return False
        End If
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktEreignis.Art) Then
            MessageBox.Show("Sie müssen die Art des Ereignisses angeben!" & vbCrLf
           )
            cmbVerlaufAuswahl.IsDropDownOpen = True
            Return False
        End If
        Return True
    End Function
    Shared Function NEUform2objok_Ereignis() As Boolean

        Return True
    End Function


    Private Sub refreshEreignisDokumenteListe()
        myGlobalz.Arc.ArcRec.dt.Clear()
        'DokumentenArchiv.initDokumente4EreignisDatatable()
        Dim bresult As Boolean = DokArcTools.dokusVonEreignisHolen.execute(myGlobalz.sitzung.aktEreignis.ID)
        If bresult Then
            myGlobalz.Arc.ereignisDocDt = myGlobalz.Arc.ArcRec.dt.Copy
            dgEreignisDokumente.DataContext = myGlobalz.Arc.ereignisDocDt
            setForm_MitDokus()
        Else
            nachricht("keine treffer: in Function  DokArcTools.dokusVonEreignisHolen.execute")
            myGlobalz.Arc.ereignisDocDt = myGlobalz.Arc.ArcRec.dt.Copy
            dgEreignisDokumente.DataContext = myGlobalz.Arc.ereignisDocDt
            setForm_MitDokus()
        End If
    End Sub


    Private Sub tbART_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbART.TextChanged
        If Not FormIstSchonGeladen Then Exit Sub
        glob2.istTextzulang(40, tbART)
        glob2.schliessenButton_einschalten(btnSpeichernEreignis)
    End Sub


    Private Sub dgEreignisDokumente_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) _
        Handles dgEreignisDokumente.SelectionChanged
        e.Handled = True
        ' dgEreignisDokumente_SelectionChanged_1Extracted()
        If dgEreignisDokumente.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgEreignisDokumente.SelectedItem, DataRowView)
        Catch ex As Exception
            nachricht(ex.ToString)
            Exit Sub
        End Try
        'verhindert bei doppelklick das doppelte öffnen der datei
        Dim difftime = Now - ALTES_uhrzeit
        If difftime < TimeSpan.FromSeconds(2) Then
            e.Handled = True
            Exit Sub
        End If
        ALTES_Dokumentname = CType((item("DOKUMENTID")), String)
        ALTES_uhrzeit = Now
        DokArc.Archiv_definiereAktdokument(item)
        dgEreignisDokumente.SelectedItem = Nothing

        If detailsTools.istDateiNameInordnung(myGlobalz.sitzung.aktDokument.DateinameMitExtension) Then
            l("nach istDateiNameInordnung")
            Dim darst As Boolean = detailsTools.Archiv_aktiviere_Dokument(dokumenteRitemousekeypressed, readOnlyDoxsInTxtCrtlOeffnen, "", alleBilder:=False, myGlobalz.sitzung.aktDokument.EreignisID)
            detailsTools.darstellen(darst)
            detailsTools.FSW_instantiieren(darst, myGlobalz.PumuckelVersion)
            refreshEreignisDokumenteListe()
        Else
            MsgBox(glob2.getMsgboxText("DateiNameIstNichtInOrdnung",
                                       New List(Of String)(New String() {myGlobalz.sitzung.aktDokument.DateinameMitExtension})
                                       ))
        End If
    End Sub



    'Private Sub dgEreignisDokumente_SelectionChanged_1Extracted()
    '    Dim item As DataRowView = Nothing
    '    Try
    '        item = CType(dgEreignisDokumente.SelectedItem, DataRowView)
    '    Catch ex As Exception
    '        nachricht(ex.ToString)
    '    End Try
    '    If item Is Nothing Then Return
    '    dgEreignisDokumente.SelectedItem = Nothing
    '    DokArc.Archiv_definiereAktdokument(item)
    '    If DokArc.machCheckout("zeige") Then
    '        DokArc.Archiv_aktiviere_dokument(myGlobalz.sitzung.aktDokument)
    '        DokArc.zulisteAddieren()
    '    End If
    'End Sub

    Private Sub btnEreignischeckout_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnEreignischeckout.Click
        myGlobalz.Arc.AuscheckenVorgangEreignis()
        e.Handled = True
    End Sub



    Private Sub btnSpeichernEreignis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichernEreignis.Click
        e.Handled = True
        Me.AllowDrop = True
        Dim rtfTEXT As String = ""
        btnSpeichernEreignis.IsEnabled = False
        myGlobalz.sitzung.aktEreignis.typnr = 1 '1=notiz,2wiedervorlage,3zahlung,4-weblink,5-textnotiz
        myGlobalz.sitzung.aktEreignis.Notiz = tbNotiz.Text
        hinzuOptionen.Visibility = Windows.Visibility.Visible
        If myGlobalz.sitzung.aktEreignis.istRTF Then
            'If rtb1.dataChanged Then
            '    btnSpeichernEreignis.IsEnabled = True
            '    myGlobalz.sitzung.aktEreignis.Notiz = rtb1.text
            '    rtfTEXT = rtb1.GetRTF
            'Else
            '    myGlobalz.sitzung.aktEreignis.Notiz = rtb1.text
            '    rtfTEXT = rtb1.GetRTF
            'End If
        End If
        If speichernEreignis(Now, rtfTEXT) Then
            spEreignisDokumente.Visibility = Windows.Visibility.Visible
            LocalParameterFiles.erzeugeParameterDateiAktvorgang_txt(False, False)
            myGlobalz.sitzung.Ereignismodus = "edit"
        Else
            hinzuOptionen.Visibility = Windows.Visibility.Hidden
        End If
        oldNotiz = myGlobalz.sitzung.aktEreignis.Notiz
        oldBeschreibung = myGlobalz.sitzung.aktEreignis.Beschreibung
    End Sub

    Private Sub tbBeschreibung_TextChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        If Not FormIstSchonGeladen Then Exit Sub
        glob2.istTextzulang(490, tbBeschreibung)
        glob2.schliessenButton_einschalten(btnSpeichernEreignis)
    End Sub

    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRefresh.Click
        refreshEreignisDokumenteListe()
    End Sub







    Private Sub btnLoeschenEreignis_ClickExtracted()
        If Not clsEreignisTools.Ereignis_und_Dokumente_entkoppeln(myGlobalz.sitzung.aktEreignis.ID) Then Exit Sub
        clsEreignisTools.ereignisLoeschen_alleDB(myGlobalz.sitzung.aktEreignis.ID)
        Me.Close()
    End Sub

    Private Sub btnLoeschenEreignis_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschenEreignis.Click
        If Not glob2.istloeschenErnstgemeint Then Exit Sub
        btnLoeschenEreignis_ClickExtracted()
        e.Handled = True
    End Sub

    'Private Sub btnEreignisnachWordumsetzenOLD_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    If btnSpeichernEreignis.IsEnabled Then
    '        MsgBox("Bitte zuerst das Ereignis abspeichern!")
    '        Exit Sub
    '    End If
    '    Dim wz As New clsRichtextbox("2")
    '    wz.init(tbBeschreibung.Text)
    '    DokArc.ausCheckoutlisteEntfernen(myglobalz.sitzung.aktDokument, myglobalz.sitzung.checkoutDokuList)
    '    refreshEreignisDokumenteListe()
    '    dgEreignisDokumente.DataContext = myglobalz.Arc.ereignisDocDt
    '    e.Handled = True
    'End Sub



    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnNachOutlookUebernehmen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        glob2.ggfTerminNachOutlookUebernehmen(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktEreignis, True)
        e.Handled = True
    End Sub

    Private Sub initCombos()
        cmbEreignisRichtung.Items.Add("")
        cmbEreignisRichtung.Items.Add("Eingang")
        cmbEreignisRichtung.Items.Add("Ausgang")
        cmbEreignisRichtung.SelectedValue = myGlobalz.sitzung.aktEreignis.Richtung
    End Sub

    Sub DragFeedback(ByVal e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effects = DragDropEffects.Move
            e.Handled = True
        Else
            e.Effects = DragDropEffects.None
            e.Handled = True
        End If
    End Sub

    Private Sub Window_Detail_PreviewDragEnter(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles Me.PreviewDragEnter
        DragFeedback(e)
    End Sub

    Private Sub Window_Detail_Drop(ByVal sender As Object, ByVal e As System.Windows.DragEventArgs) Handles Me.Drop
        e.Handled = True
        'https://social.msdn.microsoft.com/Forums/vstudio/de-DE/87423136-2fd5-4715-b514-64738414990d/drag-and-drop-von-outlook-to-c-applikation?forum=visualcsharpde
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filenames = CType(e.Data.GetData(DataFormats.FileDrop), String())
            If glob2.DokumentehinzuDragDrop(myGlobalz.sitzung.aktEreignis.ID, filenames, False) Then
                refreshEreignisDokumenteListe()
            End If
        Else
            Dim initialdir, a(), filenames() As String
            Dim dateien As New List(Of String)
            Dim fi As IO.FileInfo

            dateien = MeinOutlook.getEmailAnhangDateien(IO.Path.Combine(myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID.ToString))
            fi = New IO.FileInfo(dateien(0))
            initialdir = fi.DirectoryName
            filenames = LIBgemeinsames.clsString.ConvertListofString2ArrayOfString(dateien)
            Dim dcc As New winDokumenteEinchecken(filenames, myGlobalz.sitzung.aktEreignis.ID, initialdir, "", "", False)
            dcc.ShowDialog()
        End If

        refreshEreignisDokumenteListe()
    End Sub



    Private Sub cmbVerlaufAuswahl_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If Not FormIstSchonGeladen Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower = "hinzufügen" Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower.StartsWith("---") Then Exit Sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            tbART.Text = item

            If item.ToLower = "abgabe an bauaufsicht" Then ckbistAbgabeBauaufsicht.IsChecked = True
            If item.ToLower = "ortstermin" Then ckbistortstermin.IsChecked = True
            If item.ToLower.StartsWith("stellung") Then ckbiststellungnahme.IsChecked = True
            If item.ToLower.StartsWith("vorgang erledigt") Then ckberledigt.IsChecked = True
            cmbVerlaufAuswahl.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" ,ex)
        End Try
    End Sub

    Private Sub ckberledigt_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckberledigt.Checked, ckberledigt.Unchecked
        If ckberledigt.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = False
        End If
        btnSpeichernEreignis.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub ckbiststellungnahme_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbiststellungnahme.Checked, ckbiststellungnahme.Unchecked
        If ckbiststellungnahme.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = False
        End If
        btnSpeichernEreignis.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub ckbistortstermin_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbistortstermin.Checked, ckbistortstermin.Unchecked
        If ckbistortstermin.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.Ortstermin = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.Ortstermin = False
        End If
        btnSpeichernEreignis.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub ckbistAbgabeBauaufsicht_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles ckbistAbgabeBauaufsicht.Checked, ckbistAbgabeBauaufsicht.Unchecked
        If ckbistAbgabeBauaufsicht.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.AbgabeBA = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.AbgabeBA = False
        End If
        btnSpeichernEreignis.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbStunde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        If Not FormIstSchonGeladen Then Exit Sub
        btnSpeichernEreignis.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbMinute_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        If Not FormIstSchonGeladen Then Exit Sub
        btnSpeichernEreignis.IsEnabled = True
        e.Handled = True
    End Sub

    'Private Sub Uhrzeitsetzen()
    '    tbStunde.Text = myGlobalz.sitzung.aktEreignis.Datum.Hour.ToString
    '    tbMinute.Text = myGlobalz.sitzung.aktEreignis.Datum.Minute.ToString
    'End Sub
    Private Sub tbNotiz_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbNotiz.TextChanged
        e.Handled = True
        If Not FormIstSchonGeladen Then Exit Sub
        glob2.istTextzulang(5500, tbNotiz)
        glob2.schliessenButton_einschalten(btnSpeichernEreignis)
        NotizHoeheEinstellen()
        'myGlobalz.sitzung.aktEreignis.Datum = Now
    End Sub

    Private Sub NotizHoeheEinstellen()
        Dim datagridHeight As Double = dgEreignisDokumente.Height
        If dgEreignisDokumente.Visibility = Visibility.Collapsed Then datagridHeight = 0
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
            tbNotiz.Height = Height - spDokHinzu.Height - spEreignisDokumente.Height - datagridHeight - grdKopf.Height - 70
        Else
            tbNotiz.Height = Height - spDokHinzu.Height - spEreignisDokumente.Height - datagridHeight - grdKopf.Height - 60
        End If

    End Sub

    Private Sub initRichtextbox()
        '  _richTextBox.Text = myGlobalz.sitzung.aktEreignis.Notiz
    End Sub

    Private Sub cmbTitelVorschlag_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If Not FormIstSchonGeladen Then Exit Sub
            If cmbTitelVorschlag.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbTitelVorschlag.SelectedValue, String).Trim
            If Not String.IsNullOrEmpty(item) Then
                tbBeschreibung.Text = item
                cmbTitelVorschlag.SelectedValue = ""
            End If
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbTitelVorschlag_SelectionChanged" ,ex)
        End Try
        e.Handled = True
    End Sub




    Private Sub btnGoRtfclick(sender As Object, e As RoutedEventArgs)
        'bitte zuerst eine Überschrift eingeben
        'Dies läßt sich nicht rückgängig machen!!!

        If String.IsNullOrEmpty(tbBeschreibung.Text) Then
            MsgBox("Bitte zuerst einen Titel eingeben")
            Exit Sub
        End If
        myGlobalz.sitzung.aktEreignis.istRTF = True

        ereignistextBox = New clsRichtextbox("3")
        ereignistextBox.init(tbBeschreibung.Text)

        Dim rtftext As String = EreignisRTFhelp.NotizRtfInhaltLadenExtracted(ereignistextBox.originalfilename)
        'rtb1.SetRTF(rtftext)
        tbNotiz.Visibility = Windows.Visibility.Collapsed
        'rtb1.Visibility = Windows.Visibility.Visible
        btnSpeichernEreignis.IsEnabled = True
        BtnGoRtf.IsEnabled = False
        e.Handled = True
    End Sub

    Private Function NotizRtfInhaltLaden() As String
        Dim dokumentpfad As String = EreignisRTFhelp.getFokumenttextPfadVonRtfTextfromEreignis()
        If myGlobalz.sitzung.aktEreignis.istRTF Then
            'rtftext = EreignisRTFhelp.NotizRtfInhaltLadenExtracted(dokumentpfad)
            'rtb1.SetRTF(rtftext)
            'rtb1.dataChanged = False
            'Return rtftext
        Else
            nachricht("Fehler NotizRtfInhaltLaden: diese stelle darf nicht errreicht werden")
            Return ""
        End If
    End Function

    'Private Sub rtfboxMousedownEvent(sender As Object, e As MouseButtonEventArgs)
    '    btnSpeichernEreignis.IsEnabled = True
    '    e.Handled = True
    'End Sub

    'Private Sub fokus(sender As Object, e As RoutedEventArgs) Handles rtb1.GotFocus
    '    btnSpeichernEreignis.IsEnabled = True
    '    e.Handled = True
    'End Sub


    Private Sub btnDokusZuEreignisHinzufuegen2_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If glob2.FktDokumentehinzu(myGlobalz.sitzung.aktEreignis.ID, False) Then
            dgEreignisDokumente.Visibility = Windows.Visibility.Visible
            refreshEreignisDokumenteListe()
        End If
        NotizHoeheEinstellen()
    End Sub

    Private Sub btnArchivDokusZuEreignisHinzufuegenSICHTBARmachen()
        If detail_dokuauswahl.getAnzahlAusgewaehlterDokumente > 0 Then
            btnArchivDokusZuEreignisHinzufuegen.Visibility = Windows.Visibility.Visible
            btnArchivDokusZuEreignisHinzufuegen.Content = btnArchivDokusZuEreignisHinzufuegen.Content.ToString & ": " &
                detail_dokuauswahl.getAnzahlAusgewaehlterDokumente
        Else
            btnArchivDokusZuEreignisHinzufuegen.Visibility = Windows.Visibility.Collapsed
        End If
    End Sub

    Private Sub btnArchivDokusZuEreignisHinzufuegen2_Click(sender As Object, e As RoutedEventArgs)
        Dim hinweis As String = detail_dokuauswahl.ausgewDokusDemAktEreignisHinzufuegen(myGlobalz.sitzung.aktEreignis.ID, Psession.presDokus)
        hinweis &= detail_dokuauswahl.ausgewDokusDemAktEreignisHinzufuegen(myGlobalz.sitzung.aktEreignis.ID, Psession.presFotos)
        MessageBox.Show(hinweis, "Hinzufügen von Dokumenten aus dem Archiv", MessageBoxButton.OK, MessageBoxImage.Asterisk)
        refreshEreignisDokumenteListe()
        e.Handled = True
    End Sub

    Private Sub ckberledigt_Click(sender As Object, e As RoutedEventArgs)
        If ckberledigt.IsChecked Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = True
        Else
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = False
        End If
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "ERLEDIGT")
        e.Handled = True
    End Sub

    'Private Sub Btnlb_Click(sender As Object, e As RoutedEventArgs)
    '    Dim aaa As New winListboxEreignisTyp
    '    aaa.ShowDialog()
    '    e.Handled = True
    'End Sub

    'Private Sub btnLoeschenEreignis_Click_1(sender As Object, e As RoutedEventArgs)

    'End Sub

    'Private Sub btnEreignisDruckenMitWord_Click(sender As Object, e As RoutedEventArgs)
    '    If btnSpeichernEreignis.IsEnabled Then
    '        MsgBox("Bitte zuerst das Ereignis abspeichern!")
    '        Exit Sub
    '    End If
    '    Dim wz As New clsRichtextbox("22")
    '    wz.init(tbBeschreibung.Text)
    '    DokArc.ausCheckoutlisteEntfernen(myglobalz.sitzung.aktDokument, myglobalz.sitzung.checkoutDokuList)
    '    refreshEreignisDokumenteListe()
    '    dgEreignisDokumente.DataContext = myglobalz.Arc.ereignisDocDt
    '    e.Handled = True
    'End Sub

    Private Sub btnScan2Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        GC.Collect()
        e.Handled = True
        Dim PDF_TIFFdateiname As String
        PDF_TIFFdateiname = PDF_TIFFdateinameErzeugenUndUEbergeben()
        Dim scanexe As String
        scanexe = initP.getValue("ExterneAnwendungen.APPLICATION_Scanner")
        warteschleifeScanner(scanexe)
        GC.Collect()
        'dateiInsArchiv
        If dateiFromScanInsArchiv(PDF_TIFFdateiname, myGlobalz.sitzung.aktEreignis.ID) Then
            refreshEreignisDokumenteListe()
        End If
        NotizHoeheEinstellen()
    End Sub


    Private Sub btnNachVorlageFuerEreignis_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        nachricht("USERAKTION: vorlage hinzufügen in Ereignis")
        Dim vorl As New WinVorlagenListe(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl,
                                         myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header,
                                         akteZiehenModus:=False, myGlobalz.sitzung.aktEreignis.ID, akteZiehenModusQuelldatei:="")
        vorl.ShowDialog()
        refreshEreignisDokumenteListe()
        NotizHoeheEinstellen()
    End Sub
    Private Sub initWindowMaxheight()
        Dim maxheight As Integer = 700
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
            maxheight = 1008 '600
            Height = 1008 '600
            ' MaxWidth = 1356
        End If
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
            maxheight = 800 '500
            Height = 800 '500
            MaxWidth = 1356
        End If
        '  WindowState = Windows.WindowState.Maximized
    End Sub

    Private Sub btnEreignismitFotos_Click(sender As Object, e As RoutedEventArgs)
        If btnSpeichernEreignis.IsEnabled Then
            MsgBox("Bitte zuerst das Ereignis abspeichern!")
            Exit Sub
        End If
        If myGlobalz.sitzung.aktEreignis.istRTF Then
            Dim wz As New clsRichtextbox("2")
            wz.init("2")
        Else
            makeReportKoloman("modus=ereignisdossier")
        End If

        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " & myGlobalz.sitzung.aktEreignis.Beschreibung & ": neuer Koloreport angelegt" : CLstart.myc.aLog.log()

        refreshEreignisDokumenteListe()
        e.Handled = True
    End Sub


    'Private Sub btnEreignisnachWordumsetzen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    If btnSpeichernEreignis.IsEnabled Then
    '        MsgBox("Bitte zuerst das Ereignis abspeichern!")
    '        Exit Sub
    '    End If
    '    Dim wz As New clsRichtextbox("2")
    '    wz.init(tbBeschreibung.Text)
    '    DokArc.ausCheckoutlisteEntfernen(myglobalz.sitzung.aktDokument, myglobalz.sitzung.checkoutDokuList)
    '    refreshEreignisDokumenteListe()
    '    dgEreignisDokumente.DataContext = myglobalz.Arc.ereignisDocDt
    '    e.Handled = True
    'End Sub

    Private Sub cmbEreignisRichtung_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not FormIstSchonGeladen Then Exit Sub
        e.Handled = True
    End Sub

    Private Sub cmbEreignisRichtung_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbEreignisRichtung.SelectionChanged
        If Not FormIstSchonGeladen Then Exit Sub
        cmbEreignisRichtung_SelectionChangedExtracted()
        e.Handled = True
    End Sub

    Private Sub cmbEreignisRichtung_SelectionChangedExtracted()
        If Not FormIstSchonGeladen Then Exit Sub
        If cmbEreignisRichtung.SelectedItem Is Nothing Then Exit Sub

        Dim item2 As String = CStr(cmbEreignisRichtung.SelectedItem)
        If item2 Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktEreignis.Richtung = item2
        clsEreignisTools.setzeEreingisartfuerEmail(myGlobalz.sitzung.aktEreignis.Richtung, myGlobalz.sitzung.aktEreignis.Art)
        glob2.schliessenButton_einschalten(btnSpeichernEreignis)
    End Sub
    'Private Sub cmbEreignisRichtung_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    cmbEreignisRichtung_SelectionChangedExtracted()
    'End Sub
    Private Sub dgVorgangDokumente_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim dep As DependencyObject = DirectCast(e.OriginalSource, DependencyObject)
        While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridCell)
            dep = VisualTreeHelper.GetParent(dep)
        End While
        If dep Is Nothing Then
            Return
        End If

        If TypeOf dep Is DataGridCell Then
            Dim cell As DataGridCell = TryCast(dep, DataGridCell)
            cell.Focus()

            While (dep IsNot Nothing) AndAlso Not (TypeOf dep Is DataGridRow)
                dep = VisualTreeHelper.GetParent(dep)
            End While
            Dim row As DataGridRow = TryCast(dep, DataGridRow)
            dokumenteRitemousekeypressed = True
            dgEreignisDokumente.SelectedItem = row.DataContext
        End If
        e.Handled = True
    End Sub

    Private Sub btnWindowsPrintSupport_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True

        Dim erfolg As Boolean = clsWindowsFotodruck.haupt(myGlobalz.sitzung.aktEreignis.ID, myGlobalz.sitzung.aktVorgangsID,
                                                          myGlobalz.sitzung.aktEreignis.Beschreibung,
                                                          myGlobalz.Arc.lokalerCheckoutcache)
    End Sub

    Private Sub Window_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        e.Handled = True
        If Not FormIstSchonGeladen Then Exit Sub
        'Dim quotient As Double = 3 / 5
        'resizeWindow(quotient)
        NotizHoeheEinstellen()
    End Sub

    Private Sub BtnSetLastActionDate_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        myGlobalz.sitzung.aktEreignis.Datum = Now
        dateTimePicker1.Value = myGlobalz.sitzung.aktEreignis.Datum
        glob2.schliessenButton_einschalten(btnSpeichernEreignis)
    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
        e.Handled = True
        If Not FormIstSchonGeladen Then Exit Sub
        myGlobalz.sitzung.aktEreignis.Datum = CDate(dateTimePicker1.Value)

        glob2.schliessenButton_einschalten(btnSpeichernEreignis)
    End Sub

    Private Sub dgEreignisDokumente_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles dgEreignisDokumente.MouseDoubleClick
        e.Handled = True
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        System.Media.SystemSounds.Exclamation.Play()
        MessageBox.Show("Bitte keine Doppelklicks bei Dokumenten verwenden.")

    End Sub

    'Private Sub dgEreignisDokumente_PreviewMouseDown(sender As Object, e As MouseButtonEventArgs)
    '    If e.ClickCount > 2 Then
    '        e.Handled = True
    '    End If
    'End Sub
    'Private Sub DatePicker1_SelectedDateChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DatePicker1.SelectedDateChanged
    '    If Not FormIstSchonGeladen Then Exit Sub
    '    glob2.schliessenButton_einschalten(btnSpeichernEreignis)
    '    e.Handled = True
    'End Sub
End Class
