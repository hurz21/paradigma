Public Class winRechtsDBMan
    Property filenames As String()
    Private artArray(9) As String
    Private herkunftArray(9) As String
    Private formistgeladen As Boolean = False
    Property _modus As String = "neu"

    Property _gesetz As New clsgesetzesManagerDok
    'Property _oldSachgebiet As New List(Of AktenzeichenSachgebiet)

    Public bestandsSachgebiete As New List(Of AktenzeichenSachgebiet)

    Sub New()
        InitializeComponent()
    End Sub

    Sub New(gesetz As clsgesetzesManagerDok)
        ' This call is required by the designer.
        InitializeComponent()
        _gesetz.artId = gesetz.artId
        _gesetz.art_text = gesetz.art_text
        _gesetz.beschreibung = gesetz.beschreibung
        _gesetz.dateinameohneext = gesetz.dateinameohneext
        _gesetz.dateityp = gesetz.dateityp
        _gesetz.farbnummer = gesetz.farbnummer
        _gesetz.FullnameImArchiv = gesetz.FullnameImArchiv
        _gesetz.istgueltig = gesetz.istgueltig
        _gesetz.ordner = gesetz.ordner
        _gesetz.quellentyp = gesetz.quellentyp
        _gesetz.sachgebietnr = gesetz.sachgebietnr
        _gesetz.schlagworte = gesetz.schlagworte
        _gesetz.stammid = gesetz.stammid
        _gesetz.url = gesetz.url
        _gesetz.userInitial = gesetz.userInitial
        _gesetz.wannveroeffentlicht = gesetz.wannveroeffentlicht
        _gesetz.herkunftId = gesetz.herkunftId
        _gesetz.herkunft_text = gesetz.herkunft_text
        _modus = "edit"
    End Sub

    Private Sub winRechtsDBMan_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'initArtIndex()
        'initHerkunftarray()
        startupRechtsdbman()

        dgsachgebietZugeorndet.DataContext = bestandsSachgebiete
        btnSpeichern.IsEnabled = False

        Dim rrechtsdbARTcoll As New List(Of ClsSimpleCmb) : Dim rrechtsdbHerkunftcoll As New List(Of ClsSimpleCmb)
        initRechteDBControls(rrechtsdbARTcoll, "Select * from t36 as rechtsdb_art order by reihenf")
        initRechteDBControls(rrechtsdbHerkunftcoll, "Select * from t37 as rechtsdb_herkunft order by reihenf")

        'initRechteDBControls(rrechtsdbARTcoll, "Select * from t36 as rechtsdb_art  ")
        'initRechteDBControls(rrechtsdbHerkunftcoll, "Select * from t37 as rechtsdb_herkunft ")

        cmbArt.DataContext = rrechtsdbARTcoll : cmbHerkunft.DataContext = rrechtsdbHerkunftcoll
        cmbArt.SelectedValue = _gesetz.artId
        cmbHerkunft.SelectedValue = _gesetz.herkunftId
        If _modus = "neu" Then
            aktuellesSachgebiet()
            btnGesetzloeschen.IsEnabled = False
            Title = "RechtsgrundlagenManager " & " neu "
        End If
        If _modus = "edit" Then
            Title = "RechtsgrundlagenManager " & " edit "
            tbdateiHinweis.Text = ""
            btnDateienloeschen.IsEnabled = False
            dgNeueDateien.DataContext = Nothing
            'tbArt.Text = _gesetz.art
            'tbHerkunft.Text = _gesetz.herkunft
            tbBeschreibung.Text = _gesetz.beschreibung
            tbschlagworteRechtsdb.Text = _gesetz.schlagworte
            If _gesetz.istgueltig Then
                chkistgueltig.IsChecked = True
            Else
                chkistgueltig.IsChecked = False
            End If
            ' tbSeit.Text = _gesetz.wannveroeffentlicht.ToString
            If _gesetz.wannveroeffentlicht > CDate("1901.01.01") Then
                datepGueltig.SelectedDate = _gesetz.wannveroeffentlicht
            End If
            modrechtsdb.alleSachgebieteZumGesetz(_gesetz.stammid, bestandsSachgebiete)
            dgsachgebietZugeorndet.DataContext = bestandsSachgebiete
            ReDim filenames(0)
            filenames(0) = _gesetz.dateinameohneext & _gesetz.dateityp
            dgNeueDateien.DataContext = filenames
        End If

        formistgeladen = True
    End Sub

    'Private Sub initHerkunftarray()
    '    herkunftArray(0) = "unbekannt"
    '    herkunftArray(1) = "Hess. Staatsanzeiger"
    '    herkunftArray(2) = "Bundesgesetzblatt"
    '    herkunftArray(3) = "Fachdienstleitung"
    '    herkunftArray(4) = "Teambesprechung"
    '    herkunftArray(5) = "Hessischer Landkreistag"
    '    herkunftArray(6) = "Deutscher Landkreistag"
    'End Sub

    'Private Sub initArtIndex()
    '    artArray(0) = "unbekannt"
    '    artArray(1) = "Gesetz"
    '    artArray(2) = "Richtlinie"
    '    artArray(3) = "Ausweisung"
    '    artArray(4) = "Hinweis"
    '    artArray(5) = "Empfehlung"
    '    artArray(6) = ""
    'End Sub

    Friend Sub aktuellesSachgebiet()
        Dim newAZ As New AktenzeichenSachgebiet
        newAZ.Zahl = myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl
        newAZ.Header = myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header
        bestandsSachgebiete.Add(newAZ)
        dgsachgebietZugeorndet.DataContext = bestandsSachgebiete
    End Sub

    Private Sub startupRechtsdbman()

    End Sub

    Private Sub winRechtsDBMan_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            If _modus = "edit" Then Exit Sub
            filenames = CType(e.Data.GetData(DataFormats.FileDrop), String())
            If Not String.IsNullOrEmpty(filenames(0)) Then
                'Auswahldarstellen
                ' tbDateien.Text = bildeDateienSummenString()
                'neueDateien = bildeNeueDateienArray()
                filenames = modrechtsdb.removeAllbutPDF(filenames)
                If filenames Is Nothing Then
                    MsgBox("Es wurden keine PDF-Dateien ausgewählt")
                    Exit Sub
                End If
                dgNeueDateien.DataContext = filenames
                btnSpeichern.IsEnabled = speichernButtonAktivieren()
            End If
        End If
        e.Handled = True
    End Sub


    Private Sub dgsachgebietZugeorndet_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As AktenzeichenSachgebiet
        Try
            item = CType(dgsachgebietZugeorndet.SelectedItem, AktenzeichenSachgebiet)
            If item Is Nothing Then Return
            dgsachgebietZugeorndet.DataContext = Nothing
            bestandsSachgebiete.Remove(item)
            dgsachgebietZugeorndet.DataContext = bestandsSachgebiete
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        e.Handled = True
    End Sub

    Private Sub btnaddSachgebiet_Click(sender As Object, e As RoutedEventArgs)
        Dim nnn As New win_sgtree(myglobalz.Paradigma_Sachgebietsdatei, "einstellig")
        nnn.ShowDialog()
        dgsachgebietZugeorndet.DataContext = Nothing
        Dim newAZ As New AktenzeichenSachgebiet
        newAZ.Zahl = nnn.publicNR
        newAZ.Header = nnn.publicsgHeader
        bestandsSachgebiete.Add(newAZ)
        dgsachgebietZugeorndet.DataContext = bestandsSachgebiete
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub

    Private Function speichernButtonAktivieren() As Boolean
        If bestandsSachgebiete Is Nothing Then Return False
        If filenames Is Nothing Then Return False
        If bestandsSachgebiete.Count > 0 And filenames.Count > 0 Then
            Return True '  btnSpeichern.IsEnabled = True
        Else
            Return False '   btnSpeichern.IsEnabled = False
        End If
    End Function

    Private Sub btnabbrechen_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub dgNeueDateien_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As String
        Try
            item = CType(dgNeueDateien.SelectedItem, String)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        item = CType(dgNeueDateien.SelectedItem, String)
        If item Is Nothing Then Return
        If _modus = "neu" Then
            glob2.OpenDocument(item)
        End If
        If _modus = "edit" Then

        End If



        e.Handled = True
    End Sub

    Private Sub cmbArt_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        'tbArt.Text = artArray(cmbArt.SelectedIndex)
        glob2.schliessenButton_einschalten(btnSpeichern)
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub

    Private Sub cmbHerkunft_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        If Not formistgeladen Then Exit Sub
        'tbHerkunft.Text = herkunftArray(cmbHerkunft.SelectedIndex)
        glob2.schliessenButton_einschalten(btnSpeichern)
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub



    Private Sub datepGueltig_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        tbSeit.Text = datepGueltig.SelectedDate.ToString
        glob2.schliessenButton_einschalten(btnSpeichern)
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub
    'Sub DragFeedback(ByVal e As DragEventArgs)
    '    If e.Data.GetDataPresent(DataFormats.FileDrop) Then
    '        e.Effects = DragDropEffects.Move
    '        e.Handled = True
    '    Else
    '        e.Effects = DragDropEffects.None
    '        e.Handled = True
    '    End If
    'End Sub

    'Private Sub winRechtsDBMan_PreviewDragEnter(sender As Object, e As DragEventArgs) Handles Me.PreviewDragEnter
    '    DragFeedback(e)
    'End Sub

    'Private Sub btnDateioeffnen_Click(sender As Object, e As RoutedEventArgs)

    'End Sub

    Private Sub btnKillSachgebiete_Click(sender As Object, e As RoutedEventArgs)
        bestandsSachgebiete.Clear()
        dgsachgebietZugeorndet.DataContext = Nothing
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub

    Private Sub btnDateienloeschen_Click(sender As Object, e As RoutedEventArgs)
        ReDim filenames(0)
        dgNeueDateien.DataContext = Nothing
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub

    Private Sub btnSpeichern_Click(sender As Object, e As RoutedEventArgs)
        If Not btnSpeichern.IsEnabled Then
            MsgBox("Sie müssen mind. ein Sachgebiet UND eine PDF-Datei anwählen.")
            Exit Sub
        End If
        'For i = 0 To filenames.GetUpperBound(0)
        '    ' filenames(i) = LIBgemeinsames.clsString.normalize_Filename(filenames(i))
        'Next
        If _modus = "edit" And bestandsSachgebiete.Count > 1 Then
            MsgBox("Es darf hier nur EIN Sachgebiet zugeordnet werden!" & Environment.NewLine & "Bitte korrigieren")
            Exit Sub
        End If
        For i = 0 To filenames.GetUpperBound(0)
            For Each sg In bestandsSachgebiete
                ' aktgesetz = New clsgesetzesManagerDok
                _gesetz.herkunftId = CInt(cmbHerkunft.SelectedValue.ToString) 'tbHerkunft.Text
                _gesetz.wannveroeffentlicht = getDatum(datepGueltig.SelectedDate)
                _gesetz.artId = CInt(cmbArt.SelectedValue) 'tbArt.Text
                _gesetz.beschreibung = tbBeschreibung.Text
                _gesetz.schlagworte = tbschlagworteRechtsdb.Text
                _gesetz.istgueltig = CBool(chkistgueltig.IsChecked)
                _gesetz.sachgebietnr = sg.Zahl
                _gesetz.sachgebietheader = sg.Header
                If _modus = "neu" Then
                    If modrechtsdb.gesetzesDateiSpeichern(filenames(i).ToLower, CStr(sg.Zahl), _gesetz) Then
                        If modrechtsdb.gesetzesDBspeichern(filenames(i).ToLower,
                                                           myglobalz.sitzung.aktBearbeiter.username,
                                                           _gesetz) Then
                        Else
                            MsgBox("Problem beim Speicerhn der DB")
                        End If
                    Else
                        MsgBox("Problem beim Speicerhn der Datei")
                    End If
                End If
                If _modus = "edit" Then
                    '_gesetz.sachgebietnr = sg.Zahl
                    '_gesetz.sachgebietheader = sg.Header
                    If modrechtsdb.gesetzesDBspeichernEdit(myglobalz.sitzung.aktBearbeiter.username,
                                                          _gesetz) Then
                    Else
                        MsgBox("Problem beim Speicerhn der DB")
                    End If
                End If
            Next
        Next
        Close()
        e.Handled = True
    End Sub

    Private Function getDatum(datum As Date?) As Date
#Disable Warning BC42037 ' This expression will always evaluate to Nothing (due to null propagation from the equals operator). To check if the value is null consider using 'Is Nothing'.
        If datum = Nothing Then Return CDate("1901.01.01")
#Enable Warning BC42037 ' This expression will always evaluate to Nothing (due to null propagation from the equals operator). To check if the value is null consider using 'Is Nothing'.
        If datum > CDate("1901.01.01") Then
            Return CDate(datum)
        Else
            Return Nothing 'CDate("1901.01.01")
        End If
    End Function

    Private Sub tbschlagworteRechtsdb_SelectionChanged(sender As Object, e As RoutedEventArgs)
        glob2.schliessenButton_einschalten(btnSpeichern)
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub

    Private Sub tbBeschreibung_SelectionChanged(sender As Object, e As RoutedEventArgs)
        glob2.schliessenButton_einschalten(btnSpeichern)
        btnSpeichern.IsEnabled = speichernButtonAktivieren()
        e.Handled = True
    End Sub

    Private Sub btnGesetzloeschen_Click(sender As Object, e As RoutedEventArgs)
        If glob2.istloeschenErnstgemeint() Then
            'stammLoeschen
            If modrechtsdb.gesetz_loeschen_DB(_gesetz, myglobalz.sitzung.gesetzesdbREC) = 1 Then
                'erfolgreich gelöscht
                If modrechtsdb.gesetz_loeschen_Datei(_gesetz) Then
                    'optimal
                Else
                    'fehler
                    MsgBox("Beim Löschen der Rechtsgrundlagen-Datei ist ein Fehler aufgetreten.")
                End If
            Else
                'nicht gelöscht
                MsgBox("Beim Löschen des Eintrags in der Rechtsgrundlagendatenbank ist ein Fehler aufgetreten.")
            End If
            'sg loeschen
            Close()
        End If
        e.Handled = True
    End Sub

    'Private Sub cmbArt_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

    'End Sub
End Class
