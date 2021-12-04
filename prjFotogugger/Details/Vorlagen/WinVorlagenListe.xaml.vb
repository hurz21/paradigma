Public Class WinVorlagenListe
    Private pfadaufnahme As String = ""
    Private pfadausschreibung As String = ""

    'Private _p1 As String
    'Private _p2 As String
    'Private _p3 As Boolean
    'Private _p4 As Integer

    'Sub New(p1 As String, p2 As String, p3 As Boolean, p4 As Integer)
    '    ' TODO: Complete member initialization 
    '    _p1 = p1
    '    _p2 = p2
    '    _p3 = p3
    '    _p4 = p4
    'End Sub
    Property _eid As Integer = 0
    Property pfadAllgemein As String = ""
    Property pfadNurSachgebiet As String = ""
    Property pfadPermanent As String = ""
    Property _quelldatei As String = ""
    Property sgnr As String = ""
    Property sgHeader As String = ""
    Property _akteZiehenModus As Boolean = False

    Public Property war_abbruch As Boolean = False
    Private Sub gastLayout()
        Background = myglobalz.GetSecondBackground()
        grpvor.Background = myglobalz.GetSecondBackground()
    End Sub

    Sub New(_sgnr As String, _sgHeader As String, akteZiehenModus As Boolean, ereignisid As Integer, akteZiehenModusQuelldatei As String)
        InitializeComponent()
        sgnr = _sgnr
        sgHeader = _sgHeader
        _akteZiehenModus = akteZiehenModus
        _eid = ereignisid
        _quelldatei = akteZiehenModusQuelldatei
    End Sub

    Private Sub WinVorlagenListe_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        Dim divorlagenVerzeichnis As IO.DirectoryInfo = Nothing

        If _akteZiehenModus Then
            Dim item As String = _quelldatei ' "Bitte Akte ziehen.docx"
            ckbEditVorlagenDatei.IsChecked = False

            clsVorlagenTools.berechneVorlagenverzeichnis(divorlagenVerzeichnis, "allgemein", pfadAllgemein)
            dgVorlagenAllgemein_SelectionChangedExtracted(item)
        Else
            clsVorlagenTools.berechneVorlagenverzeichnis(divorlagenVerzeichnis, "aufnahme", pfadaufnahme)
            clsVorlagenTools.berechneVorlagenverzeichnis(divorlagenVerzeichnis, "ausschreibung", pfadausschreibung)
            refreshVorlagenListe(sgnr)
            SetTitle()
            If myGlobalz.sitzung.aktVorgangsID < 1 Then
                ckbEditVorlagenDatei.IsChecked = True
                ckbEditVorlagenDatei.IsEnabled = False
                MessageBox.Show(glob2.getMsgboxText("winVorlagenListeLoadedKeinVorgang", New List(Of String)(New String() {})),
                                "VorlagenDatei ändern",
                        MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
                tiAllgemein.IsSelected = True
            End If
        End If
        gastLayout()

    End Sub

    Private Sub SetTitle()
        tiSachgebiet.Header = "Nur für Sachgebiet: " & sgnr
        tiSachgebiet.ToolTip = "Nur für Sachgebiet: " & sgHeader 'myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header

        tiAllgemein.ToolTip = "Diese Vorlagen sind unabhängig vom Sachgebiet"
        tiPermanent.ToolTip = "Diese Vorlagen bleiben hier an Ort und Stelle. Sie dienen dazu um Informationen für das gesamte Sachgebiet - also unabhängig vom Einzelfall - zu sammeln und bereitzustellen "
        tbsgNR.Text = sgHeader
        btnExplorer.Content = sgnr & "-Verzeichnis öffnen"
        Title = detailsTools.settitle("Vorlagenliste, Sachgebiet: " & sgnr & ", " & sgHeader)
    End Sub

    Private Sub dgVorlagen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgVorlagen.SelectionChanged
        Dim item As String 'nurSachgebiet
        Try
            If dgVorlagen.SelectedItem Is Nothing Then Return
            item = dgVorlagen.SelectedItem.ToString
            dgVorlagen.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            pfadNurSachgebiet = pfadNurSachgebiet.Replace("\\", "\")
            Dim datei As String = IO.Path.Combine(pfadNurSachgebiet, item).Replace("\\", "\")
            nachricht("Vorlage gewählt: " & datei)
            If ckbEditVorlagenDatei.IsChecked Or datei.ToLower.Contains("_##permanent##_") Then
                'DateiOffnen
                MessageBox.Show(glob2.getMsgboxText("permanentDatei", New List(Of String)(New String() {datei})),
                                "Permanent-Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
                Process.Start(datei)
            Else
                If myglobalz.sitzung.aktVorgangsID > 0 Then
                    vorlageVerwursten(item, datei, pfadNurSachgebiet, False)
                End If
            End If
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagen_SelectionChanged: {0}", ex)))
        End Try
    End Sub


    Private Sub vorlageVerwursten(ByVal item As String, ByVal datei As String, pfad As String, akteZiehenmodus As Boolean)
        Dim testendung As New IO.FileInfo(item)
        If testendung.Extension.ToLower.Contains("xls") Or
            testendung.Extension.ToLower.Contains("pdf") Then
            Dim vsteuer As New winOtherVorlagensteuerung(item, pfad, _eid)
            vsteuer.ShowDialog()
            If vsteuer.abbruch Then
                If vsteuer.cmbModus.SelectedIndex = 1 Then
                    'arbeitsdokument ins archiv übernehmen
                    Exit Sub
                End If
                If vsteuer.cmbModus.SelectedIndex = 0 Then
                    'vorschaudokukment nur anschauen
                    glob2.OpenDocument(datei)
                Else
                End If
            Else
                myglobalz.sitzung.aktDokument.Beschreibung = vsteuer.dokumentschlagworte
                Dim outfile As String = "", erfolg As Boolean
            End If
        Else
            'word
            Dim vsteuer As New WinWordVorlageSteuerung(item, pfad, akteZiehenmodus, _eid)
            vsteuer.ShowDialog()
        End If
        testendung = Nothing
    End Sub



    Private Sub refreshVorlagenListe(sgnr As String)
        Dim caunt As Integer '= initVorlagenDatatable("allgemein")
        caunt = initVorlagenDatatable2("allgemein", dgVorlagenAllgemein, pfadAllgemein, sgnr)
        caunt = initVorlagenDatatable2("fuersachgebiet", dgVorlagen, pfadNurSachgebiet, sgnr)
        caunt = initVorlagenDatatable2("_##permanent##_", dgVorlagenPermanant, pfadPermanent, sgnr)
        caunt = initVorlagenDatatable2("aufnahme", dgVorlagenAufnahme,
                                      pfadaufnahme,
                                       sgnr)
        caunt = initVorlagenDatatable2("ausschreibung", dgVorlagenAusschreibung,
                                      pfadausschreibung,
                                       sgnr)

        If caunt% >= 0 Then
            '	dgVorlagen.DataContext = myGlobalz.sitzung.VorlagenREC.dt
            '	tabheaderVorlagen.SetValue(TextElement.FontWeightProperty, FontWeights.Bold)
            ' tabheaderVorlagen.Header = "Vorlagen " & caunt%
        Else
            '	dgVorlagen.DataContext = Nothing
            'tabheaderVorlagen.SetValue(TextElement.FontWeightProperty, FontWeights.normal)
            'tabheaderVorlagen.Header = "Vorlagen " '& 		myGlobalz.sitzung.VorlagenREC.rows.count 
        End If
    End Sub




    Private Sub tbTestmarkenliste_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        clsVorlagenTools.zeigeTextmarkenListe()
        e.Handled = True
    End Sub


    Private Sub btnRatgeber_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        MessageBox.Show(glob2.getMsgboxText("ratgeber", New List(Of String)(New String() {})), "Probleme mit Textmarken vermeiden",
                        MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Function initVorlagenDatatable2(modus As String, dataGrid As DataGrid,
                                            ByRef pfad As String, sgnr As String) As Integer
        Dim ListeAllerVorlagenImVerz As New List(Of IO.FileInfo)
        Dim ListeAllerVorlagenImVerzSort As New List(Of IO.FileInfo)

        Dim anzahlVorlagen As Integer
        Dim divorlagenVerzeichnis As IO.DirectoryInfo = Nothing
        nachricht("initVorlagenDatatable")
        ListeAllerVorlagenImVerz.Clear()
        Try
            anzahlVorlagen = clsVorlagenTools.presentVorlagenListe(ListeAllerVorlagenImVerz, divorlagenVerzeichnis, modus, pfad, sgnr)
            Dim results = From zeile As IO.FileInfo In ListeAllerVorlagenImVerz
                          Order By zeile.Name
            For Each strra As IO.FileInfo In results
                ListeAllerVorlagenImVerzSort.Add(strra)
            Next
            If ListeAllerVorlagenImVerz.Count > 0 Then
                dataGrid.DataContext = ListeAllerVorlagenImVerzSort
            Else
                dataGrid.DataContext = Nothing
            End If
            Return anzahlVorlagen
        Catch ex As Exception
            dgVorlagen.DataContext = Nothing
            Return -1
        End Try
    End Function

    Private Sub dgVorlagenAllgemein_SelectionChangedExtracted(ByVal item As String)
        pfadAllgemein = pfadAllgemein.Replace("\\", "\")
        Dim datei As String = IO.Path.Combine(pfadAllgemein, item).Replace("\\", "\")
        If ckbEditVorlagenDatei.IsChecked Then
            'DateiOffnen
            MessageBox.Show(glob2.getMsgboxText("permanentDatei", New List(Of String)(New String() {})),
                   "Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
            Process.Start(datei)
        Else
            If myglobalz.sitzung.aktVorgangsID > 0 Then
                Dim divorlagenVerzeichnis As IO.DirectoryInfo = Nothing
                'clsVorlagenTools.berechneVorlagenverzeichnisAllgemein(divorlagenVerzeichnis, "allgemein", pfad)
                If _akteZiehenModus Then
                    Close()
                End If
                vorlageVerwursten(item, datei, pfadAllgemein, _akteZiehenModus)
            End If
        End If
    End Sub
    Private Sub dgVorlagenAllgemein_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgVorlagenAllgemein.SelectionChanged
        Dim item As String
        Try
            If dgVorlagenAllgemein.SelectedItem Is Nothing Then Return
            item = dgVorlagenAllgemein.SelectedItem.ToString
            dgVorlagenAllgemein.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            '  dgVorlagenAllgemein_SelectionChangedExtracted(item, pfadAllgemein)

            dgVorlagenAllgemein_SelectionChangedExtracted(item)
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagen_SelectionChanged: {0}", ex)))
        End Try
    End Sub

    Private Sub dgVorlagenPermanant_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgVorlagenPermanant.SelectionChanged
        Dim item As String
        Try
            If dgVorlagenPermanant.SelectedItem Is Nothing Then Return
            item = dgVorlagenPermanant.SelectedItem.ToString
            dgVorlagenPermanant.SelectedItem = Nothing

            pfadPermanent = pfadPermanent.Replace("\\", "\")
            Dim datei As String = IO.Path.Combine(pfadPermanent, item).Replace("\\", "\")

            'DateiOffnen
            MessageBox.Show(glob2.getMsgboxText("permanentDatei", New List(Of String)(New String() {})),
                   "Permanent-Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
            Process.Start(datei)


            e.Handled = True
            Me.Close()
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagenPermanant_SelectionChanged: {0}", ex)))
        End Try
    End Sub

    Private Sub btnExplorer_Click(sender As Object, e As RoutedEventArgs)
        Dim vorlagenVerzeichnis As IO.DirectoryInfo = Nothing
        Dim pfad As String = ""
        '         berechneVorlagenverzeichnisAllgemein(di, modus, pfad)
        clsVorlagenTools.berechneVorlagenverzeichnisSachgebiet(vorlagenVerzeichnis, "fuersachgebiet", pfad, sgnr)
        '  berechneVorlagenverzeichnisPermanent(di, modus, pfad, sgnr)

        '   clsVorlagenTools.berechneVorlagenverzeichnis(vorlagenVerzeichnis, "fuersachgebiet", pfad, "")
        Dim diras As String = pfad
        Process.Start(pfad)
        e.Handled = True
    End Sub

    Private Sub btnSGexplorer_Click(sender As Object, e As RoutedEventArgs)
        If Not String.IsNullOrEmpty(tbzuSG.Text) Then
            sgnr = tbzuSG.Text
            sgHeader = ""
            tbzuSG.Text = ""
        Else
            Dim sgt As New win_sgtree(myglobalz.Paradigma_Sachgebietsdatei, "vierstellig")
            sgt.ShowDialog()
            sgnr = sgt.publicNR
            sgHeader = sgt.publicsgHeader
            sgt = Nothing
        End If
        refreshVorlagenListe(sgnr)
        SetTitle()

        GC.Collect()

        e.Handled = True
    End Sub

    Private Sub dgVorlagenAufanhme_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As String 'nurSachgebiet
        Try
            If dgVorlagenAufnahme.SelectedItem Is Nothing Then Return
            item = dgVorlagenAufnahme.SelectedItem.ToString
            dgVorlagenAufnahme.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            '  pfadNurSachgebiet = pfadNurSachgebiet.Replace("\\", "\")
            Dim datei As String = IO.Path.Combine(pfadaufnahme, item).Replace("\\", "\")
            nachricht("Vorlage gewählt: " & datei)
            If ckbEditVorlagenDatei.IsChecked Or datei.ToLower.Contains("_##permanent##_") Then
                'DateiOffnen
                MessageBox.Show(glob2.getMsgboxText("permanentDatei", New List(Of String)(New String() {datei})),
                                "Permanent-Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
                Process.Start(datei)
            Else
                If myGlobalz.sitzung.aktVorgangsID > 0 Then
                    vorlageVerwursten(item, datei, pfadaufnahme, False)
                End If
            End If
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagen_SelectionChanged: {0}", ex)))
        End Try
    End Sub

    Private Sub dgVorlagenAusschreibung_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As String 'nurSachgebiet
        Try
            If dgVorlagenAusschreibung.SelectedItem Is Nothing Then Return
            item = dgVorlagenAusschreibung.SelectedItem.ToString
            dgVorlagenAusschreibung.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            '  pfadNurSachgebiet = pfadNurSachgebiet.Replace("\\", "\")
            Dim datei As String = IO.Path.Combine(pfadausschreibung, item).Replace("\\", "\")
            nachricht("Vorlage gewählt: " & datei)
            If ckbEditVorlagenDatei.IsChecked Or datei.ToLower.Contains("_##permanent##_") Then
                'DateiOffnen
                MessageBox.Show(glob2.getMsgboxText("permanentDatei", New List(Of String)(New String() {datei})),
                                "Permanent-Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
                Process.Start(datei)
            Else
                If myGlobalz.sitzung.aktVorgangsID > 0 Then
                    vorlageVerwursten(item, datei, pfadausschreibung, False)
                End If
            End If
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagen_SelectionChanged: {0}", ex)))
        End Try
    End Sub

    Private Sub BtnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
        war_abbruch = True
    End Sub
End Class
