
Imports paradigmaDetail

Public Class winDokumenteEinchecken
    Private Property collLokaleDokumente As New List(Of clsPresDokumente)
    Private _filenamen() As String, _eid As Integer, _dasdir As String
    Private _vorschlagsDatei As String
    Private dokumentmoven, fotoZuRaumbezug As Boolean
    Private allg_beschreibung As String
    Private _DateiMetaData As String
    Public istConject As Boolean = False
    'Private Property _NeuesEreignisdatum As Date

    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub

    Sub New(ByVal filenamen() As String, ByVal eid As Integer, ByVal dasdir As String, vorschlagsDatei As String, DateiMetaData As String, _istConject As Boolean)
        InitializeComponent()
        _filenamen = filenamen
        _eid = eid
        _dasdir = dasdir
        _vorschlagsDatei = vorschlagsDatei
        _DateiMetaData = DateiMetaData
        istConject = _istConject
    End Sub


    Private Sub winDokumenteEinchecken_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        'detailsTools.initErgeinistypCombo(Me, "detail_ereignisseTitel.xml", "XMLSourceComboBoxTitel") : cmbTitelVorschlag.SelectedIndex = 0
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
        'detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisseBetreff") : cmbVerlaufBetreff.SelectedIndex = 0
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseTitel.xml", "XMLSourceComboBoxEreignisseBetreff") : cmbVerlaufBetreff.SelectedIndex = 0
        If _eid < 1 Then
            grpEreignis.Visibility = Windows.Visibility.Visible
        Else
            grpEreignis.Visibility = Windows.Visibility.Collapsed
        End If
        If istConject Then
            chkEreignisMap.IsChecked = False
            istConject = False
            'diente nur der übergabe damit die checkbox augeschaltet werden kann
        End If
        If _vorschlagsDatei.IsNothingOrEmpty Then
        Else
            ReDim _filenamen(0)
            _filenamen(0) = _vorschlagsDatei
            rbMove.IsChecked = True
        End If
        initComboRichtung()
        setRichtungFallsScan(_vorschlagsDatei)
        FelderInit()
        DatePicker1.Value = Now
        'Uhrzeitsetzen()
        Title = StammToolsNs.setWindowTitel.exe("edit", "Dokumentenarchiv - Aufnahme")
        detailsTools.initErgeinistypCombo(Me, "dokument_schlagworte.xml", "XMLSourceComboBoxdokumentschlagworte")
        If String.IsNullOrEmpty(tbBeschreibung.Text) Then cmbTitelVorschlag.IsDropDownOpen = True

        FocusManager.SetFocusedElement(Me, BTNdateienauswaehlen)
        gastLayout()
        e.Handled = True
    End Sub

    Private Sub setRichtungFallsScan(vorschlagsDatei As String)
        If vorschlagsDatei.ToLower.Contains("\scan_") Then
            myGlobalz.sitzung.aktEreignis.Richtung = "Eingang"
            cmbEreignisRichtung.SelectedValue = myGlobalz.sitzung.aktEreignis.Richtung
        End If

    End Sub

    Private Shared Sub initEreignis(ByVal neuereignis As clsEreignis,
                                    Beschreibung As String,
                                    art As String,
                                    daite As Date,
                                    richtung As String)
        With neuereignis
            .Art = art
            .Beschreibung = Beschreibung
            .Datum = daite
            .istRTF = False
            .Notiz = ""
            .DokumentID = 0
            .typnr = 1
            .Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
            .Richtung = richtung
        End With
    End Sub

    Private Sub btnWeiter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        If chkEreignisMap.IsChecked And (tbTypEreignis.Text.Trim.IsNothingOrEmpty()) Then
            MsgBox("Sie müssen dem Ereignis einen Typ zuweisen!!! Abbruch.", MsgBoxStyle.Critical, "Ereignis anlegen")
            cmbVerlaufAuswahl.IsDropDownOpen = True
            e.Handled = True
            Exit Sub
        End If
        '   Dim dokumentdatum As Date = clsEreignisTools.GetGesammelteDatumUhrzeit(CDate(DatePicker1.SelectedDate).Date, tbStunde.Text, tbMinute.Text)
        Dim dokumentdatum As Date = CDate(DatePicker1.Value)
        If _eid < 1 Then
            'sonst wird das datum bei existierenden ereignissen verstellt
            myGlobalz.sitzung.aktEreignis.Datum = dokumentdatum
        End If

        allg_beschreibung = tbBeschreibung.Text
        If rbCopy.IsChecked Then dokumentmoven = False
        If rbMove.IsChecked Then dokumentmoven = True
        If chkFotoalsRaumbezug.IsChecked Then
            fotoZuRaumbezug = True
        Else
            fotoZuRaumbezug = False
        End If
        Dim neueliste As String()
        ReDim neueliste(_filenamen.Length - 1)
        Dim ftool As New clsWindokueincheckenTOOL
        Dim loeschliste As New List(Of String)
        neueliste = ftool.nurAusgwaehlteDokus(collLokaleDokumente)
        neueliste = ftool.fotoarrayUmwandeln(neueliste, loeschliste)
        If Not neueliste Is Nothing Then
            If CBool(chkEreignisMap.IsChecked) Then
                Dim neuereignis As New clsEreignis
                initEreignis(neuereignis, tbBeschreibungEreignis.Text, tbTypEreignis.Text,
                             myGlobalz.sitzung.aktEreignis.Datum, myGlobalz.sitzung.aktEreignis.Richtung)
                clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)
                CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
                    myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
                _eid = neuereignis.ID
            End If
            glob2.Archiv_eingang(neueliste, allg_beschreibung, _eid, dokumentmoven, fotoZuRaumbezug, dokumentdatum,
                                 CBool(chkFotoverkleinern.IsChecked))
        End If
        ftool.loesche_loeschliste(loeschliste)
        ftool = Nothing
        e.Handled = True
        DialogResult = True
        Me.Close()
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = False
        e.Handled = True
        Me.Close()
    End Sub
    Private Sub BTNdateienauswaehlen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        If glob2.DokumenteAuswaehlen(_filenamen$, _dasdir) Then
            FelderInit()
            Dim ftool As New clsWindokueincheckenTOOL
            ftool.quellverzeichnisspeichern(_dasdir)
            '   If esSindFotosEnthalten(_filenamen) Then
            'chkEreignisMap.IsChecked = True
            'tbTypEreignis.Text = "Ortstermin"
            'End If
        End If
    End Sub
    <Obsolete>
    Private Function esSindFotosEnthalten(_filenamen() As String) As Boolean
        Try
            For i = 0 To _filenamen.Count - 1
                If _filenamen(i).ToLower.EndsWith(".jpg") Or _filenamen(i).ToLower.EndsWith(".jpeg") Then
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub FelderInit()
        If _filenamen IsNot Nothing Then
            tbAnzahldateien.Text = _filenamen.Count & " Stück"
        End If
        tbHinweis.Text = String.Format("Die Dokumente werden dem Vorgang {0} hinzugefügt !!! (Az.: {1})",
                                     myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        Dim ftool As New clsWindokueincheckenTOOL
        Dim test$ = ftool.verzeichnis_isolieren(_filenamen)
        If String.IsNullOrEmpty(test$) Then
            ftool.QuellverzeichnisseEinlesen(_dasdir)
        Else
            _dasdir = test
        End If
        If tbBeschreibung.Text.IsNothingOrEmpty() Then
            tbBeschreibung.Text = _DateiMetaData
        End If
        '   lbDateien.DataContext = _filenamen
        collLokaleDokumente = makeLokalDokumentCollection(_filenamen)
        dgVorgangDokumente.DataContext = collLokaleDokumente
        tbQuellverzeichnis.Text = _dasdir
        If _dasdir.ToLower.Contains("conject") Then
            If Not istConject Then istConject = True
            chkEreignisMap.IsChecked = True
            tbBeschreibungEreignis.Text = "Aus: Bauantrag-Online"
            tbTypEreignis.Text = "Antragsunterlagen"

            cmbEreignisRichtung.SelectedValue = "Eingang"
        End If
    End Sub

    Private Function makeLokalDokumentCollection(filenamen() As String) As List(Of clsPresDokumente)
        Dim coll As New List(Of clsPresDokumente)
        Dim dok As New clsPresDokumente
        Dim fi As IO.FileInfo
        Try
            l(" MOD makeLokalDokumentCollection anfang")
            If filenamen Is Nothing Then Return coll
            For Each datei As String In filenamen
                dok = New clsPresDokumente
                fi = New IO.FileInfo(datei)
                dok.DateinameMitExtension = fi.Name
                dok.Filedatum = fi.LastAccessTime
                dok.dokumentPfad = fi.DirectoryName
                dok.ausgewaehlt = True
                dok.getDokTyp()
                dok.Initiale = myGlobalz.sitzung.aktBearbeiter.Initiale
                coll.Add(dok)
            Next
            l(" MOD makeLokalDokumentCollection ende")
            Return coll
        Catch ex As Exception
            l("Fehler in makeLokalDokumentCollection: ", ex)
            Return coll
        End Try
    End Function

    Private Sub cmbTitelVorschlag_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            e.Handled = True
            If cmbTitelVorschlag.SelectedValue Is Nothing OrElse
                String.IsNullOrEmpty(cmbTitelVorschlag.SelectedValue.ToString) Then Exit Sub
            Dim item As String = CType(cmbTitelVorschlag.SelectedValue, String).Trim
            If Not String.IsNullOrEmpty(item) Then
                tbBeschreibung.Text = item
                cmbTitelVorschlag.SelectedValue = ""
            End If
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbTitelVorschlag_SelectionChanged", ex)
        End Try
        e.Handled = True
    End Sub

    Private Sub chkEreignisMap_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        tbBeschreibungEreignis.IsEnabled = True
        tbTypEreignis.Text = "Ortstermin"
        cmbVerlaufAuswahl.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub cmbVerlaufAuswahl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            e.Handled = True
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower = "hinzufügen" Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower.StartsWith("---") Then Exit Sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            tbTypEreignis.Text = item
            cmbVerlaufAuswahl.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged", ex)
            e.Handled = True
        End Try
    End Sub




    Private Sub chkEreignisMap_Checked(sender As Object, e As RoutedEventArgs) Handles chkEreignisMap.Checked
        e.Handled = True
        If tbBeschreibungEreignis.Text.IsNothingOrEmpty() Then
            tbBeschreibungEreignis.Text = tbBeschreibung.Text
        End If
        If chkEreignisMap.IsChecked Then
            spEreignisAnlegen.IsEnabled = True
        Else
            spEreignisAnlegen.IsEnabled = False
        End If
        e.Handled = True
    End Sub



    Private Sub tbStunde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        'btnSpeicher.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub tbMinute_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        'btnSpeichernEreignis.IsEnabled = True
    End Sub



    'Private Sub Uhrzeitsetzen()
    '    tbStunde.Text = Now.Hour.ToString
    '    tbMinute.Text = Now.Minute.ToString
    'End Sub



    'Private Sub cmbEreignisRichtung_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
    '    If cmbEreignisRichtung.SelectedItem Is Nothing Then Exit Sub
    '    Dim item2 As String = CStr(cmbEreignisRichtung.SelectedItem)
    '    If item2 Is Nothing Then Exit Sub
    '    myGlobalz.sitzung.aktEreignis.Richtung = item2
    '    e.Handled = True
    'End Sub

    Private Sub initComboRichtung()
        cmbEreignisRichtung.Items.Add("")
        cmbEreignisRichtung.Items.Add("Eingang")
        cmbEreignisRichtung.Items.Add("Ausgang")
        cmbEreignisRichtung.SelectedValue = myGlobalz.sitzung.aktEreignis.Richtung
    End Sub




    Private Sub cmbEreignisRichtung_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If cmbEreignisRichtung.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As String = CStr(cmbEreignisRichtung.SelectedItem)
        If item2 Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktEreignis.Richtung = item2

    End Sub

    Private Sub dgVorgangDokumente_MouseRightButtonUp(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub dgVorgangDokumente_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

    End Sub

    Private Sub dgVorgangDokumente_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub cmbVerlaufBetreff_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            e.Handled = True
            If cmbVerlaufBetreff.SelectedValue Is Nothing OrElse
                String.IsNullOrEmpty(cmbVerlaufBetreff.SelectedValue.ToString) Then Exit Sub
            Dim item As String = CType(cmbVerlaufBetreff.SelectedValue, String).Trim
            If Not String.IsNullOrEmpty(item) Then
                tbBeschreibungEreignis.Text = item
                cmbVerlaufBetreff.SelectedValue = ""
            End If
        Catch ex As Exception
            nachricht("cmbTitelVorschlag_SelectionChanged", ex)
        Finally
            e.Handled = True
        End Try
    End Sub
    Private Sub DatePicker1_ValueChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
        e.Handled = True
    End Sub
    Private Sub btnsetzeDatumaufNow(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        DatePicker1.Value = Now

    End Sub


End Class
