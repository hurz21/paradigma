Public Class winStatisch
    Private pfadaufnahme As String = ""
    Private pfadausschreibung As String = ""
    Property _eid As Integer = 0
    Property pfadAllgemein As String = ""
    Property pfadNurSachgebiet As String = ""
    Property pfadPermanent As String = ""
    Property _wurzelverzeichnis As String = ""
    Property _zielverzeichnis As String = ""
    Property sgnr As String = ""
    Property sgHeader As String = ""
    Property _akteZiehenModus As Boolean = False

    Public Property war_abbruch As Boolean = False
    Sub New(_sgnr As String, _sgHeader As String, ereignisid As Integer, wurzelverz As String, zielverzeichnis As String)
        InitializeComponent()
        sgnr = _sgnr
        sgHeader = _sgHeader
        _eid = ereignisid
        _wurzelverzeichnis = wurzelverz
        _zielverzeichnis = zielverzeichnis
    End Sub

    Private Sub winStatisch_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        myGlobalz.VorlagenRoot = _wurzelverzeichnis
        Dim divorlagenVerzeichnis As IO.DirectoryInfo = Nothing
        pfad = _wurzelverzeichnis '& "\allgemein"
        divorlagenVerzeichnis = New IO.DirectoryInfo(pfad)
        '---------------darstellen
        tbSVorlagenRoot.Text = pfad
        tbSZielverzeichnis.Text = _zielverzeichnis

        refreshVorlagenListe(sgnr)
    End Sub

    Private Sub refreshVorlagenListe(sgnr As String)
        Dim caunt As Integer '= initVorlagenDatatable("allgemein")
        caunt = initVorlagenDatatable2("", dgVorlagen, pfadAllgemein, sgnr)
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

    Private Sub dgVorlagen_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As String 'nurSachgebiet
        Dim fi As IO.FileInfo
        Try
            If dgVorlagen.SelectedItem Is Nothing Then Return
            item = dgVorlagen.SelectedItem.ToString
            dgVorlagen.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            pfadNurSachgebiet = pfadNurSachgebiet.Replace("\\", "\")
            Dim zieldatei, quelldatei As String
            quelldatei = IO.Path.Combine(pfadAllgemein, item).Replace("\\", "\")
            fi = New IO.FileInfo(quelldatei)
            zieldatei = fi.Name.ToLower.Replace(fi.Extension.ToLower, "")
            zieldatei = Now.ToString("yyyyMMddHHmmss_") & myGlobalz.sitzung.aktVorgangsID & "_" & zieldatei & fi.Extension


            zieldatei = IO.Path.Combine(_zielverzeichnis, zieldatei).Replace("\\", "\")


            nachricht("zieldatei gewählt: " & zieldatei)
            If ckbEditVorlagenDatei.IsChecked Or zieldatei.ToLower.Contains("_##permanent##_") Then
                'DateiOffnen
                MessageBox.Show(glob2.getMsgboxText("permanentDatei", New List(Of String)(New String() {zieldatei})),
                                "Permanent-Vorlage", MessageBoxButton.OK, MessageBoxImage.Information)
                Process.Start(zieldatei)
            Else
                IO.File.Copy(quelldatei, zieldatei)
                Process.Start(zieldatei)
                Process.Start(_zielverzeichnis)
                'If myGlobalz.sitzung.aktVorgangsID > 0 Then
                '    vorlageVerwursten(item, zieldatei, pfadNurSachgebiet, False)
                'End If
            End If
        Catch ex As Exception
            nachricht_und_Mbox((String.Format("dgVorlagen_SelectionChanged: {0}", ex)))
        End Try
    End Sub

    Private Sub vorlageVerwursten(item As String, datei As String, pfadNurSachgebiet As String, v As Boolean)
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
                myGlobalz.sitzung.aktDokument.Beschreibung = vsteuer.dokumentschlagworte
                Dim outfile As String = "", erfolg As Boolean
            End If
        Else
            'word
            'Dim vsteuer As New WinWordVorlageSteuerung(item, pfad, akteZiehenmodus, _eid)
            'vsteuer.ShowDialog()
        End If
        testendung = Nothing
    End Sub

    Private Sub tbSZielverzeichnis_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        Process.Start(tbSZielverzeichnis.Text)
    End Sub

    Private Sub tbSVorlagenRoot_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        Process.Start(tbSVorlagenRoot.Text)
    End Sub
End Class
