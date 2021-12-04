Public Class winToConject
    Private conjectName As String = ""
    Private typ As String = "Stellungnahme_FD_Umwelt"
    Private ladevorgangabgeschlossen As Boolean = False
    Private endung As String = ""
    Private abteilung As String = "Immissionsschutz"
    Sub New()
        InitializeComponent()
    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        tbAltername.Text = myGlobalz.sitzung.aktDokument.DateinameMitExtension
        tbNeuername.Text = myGlobalz.sitzung.aktDokument.DateinameMitExtension
        If myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.EndsWith(".docx") Then
            endung = ".pdf"
            'tbNeuername.Text = tbNeuername.Text.Replace("docx", "pdf")
            tbNeuername.Text = calcConjectdateiname(typ, abteilung, glob2.getTimestamp, endung)
        End If
        If myGlobalz.sitzung.aktDokument.DateinameMitExtension.ToLower.EndsWith(".pdf") Then
            cbOriginalNameVerwenden.IsChecked = True
            cbOriginalNameVerwenden.IsEnabled = False
            cmbConjectDokumentAbteilung.IsEnabled = False
            cmbConjectDokumentTyp.IsEnabled = False
        End If
        ladevorgangabgeschlossen = True
    End Sub
    Private Sub btnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
    End Sub
    Private Sub cmbConjectDokumentAbteilung_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        If Not ladevorgangabgeschlossen Then Exit Sub

        abteilung = cmbConjectDokumentAbteilung.SelectedValue.ToString.Replace("System.Windows.Controls.ComboBoxItem: ", "")
        tbNeuername.Text = calcConjectdateiname(typ, abteilung, glob2.getTimestamp, endung)
        cbOriginalNameVerwenden.IsChecked = False
    End Sub

    Private Sub cmbConjectDokumentTyp_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        If cmbConjectDokumentTyp.SelectedIndex = 0 Then
            typ = "Stellungnahme_FD_Umwelt"
        End If
        If cmbConjectDokumentTyp.SelectedIndex = 1 Then
            typ = "Nachforderung_FD_Umwelt"
        End If
        If cmbConjectDokumentTyp.SelectedIndex = 2 Then
            typ = "Anlagen_FD_Umwelt"
        End If
        If cmbConjectDokumentTyp.SelectedIndex = 3 Then
            typ = "Hinweis_FD_Umwelt"
        End If
        cbOriginalNameVerwenden.IsChecked = False
        tbNeuername.Text = calcConjectdateiname(typ, abteilung, glob2.getTimestamp, endung)
    End Sub

    Private Function calcConjectdateiname(typ As String, abteilung As String, dateum As String, endung As String) As String
        Return typ & "_" & abteilung & "_vom_" & dateum & "_" & myGlobalz.sitzung.aktVorgangsID & endung
    End Function



    Private Sub cbOriginalNameVerwenden_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If cbOriginalNameVerwenden.IsChecked Then
            MsgBox("Bitte verwenden Sie diese Option nur dann, wenn die Datei bereits im albschliessendem / PDF-format vorliegt!")
            tbNeuername.Text = tbAltername.Text
        Else
            tbNeuername.Text = calcConjectdateiname(typ, abteilung, glob2.getTimestamp, endung)
        End If
    End Sub
    Private Sub btnOk_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If nachZielKopieren.AlsPDFAkopieren.exe(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgang.istConjectVorgang, tbNeuername.Text.Trim) Then
            If myGlobalz.sitzung.aktVorgang.istConjectVorgang Then
                MessageBox.Show("Stellungnahme-PDF wurde unter 'Dokumente' abgelegt. Bitte auffrischen!" & Environment.NewLine &
                              "                  Verzeichnis wird geöffnet, " & Environment.NewLine &
                              "                  Verzeichnisname wurde in Zwischenablage kopiert !", "Übernahme nach Conject vorbereitet")
            Else
                MsgBox("PDF wurde unter 'Dokumente' abgelegt. Bitte auffrischen!")
            End If
        Else
            MessageBox.Show("Das Kopieren nach PDF hat nicht geklappt. Vermutlich ist die Datei geöffnet, schreibgeschützt oder sonstwas. ", "Hurz", MessageBoxButton.OK, MessageBoxImage.Error)
        End If
        FileArchivTools.inputFileReadonlyEntfernen(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        myGlobalz.sitzung.aktDokument.nurzumlesen = False
        l("nurzumlesen1:  " & myGlobalz.sitzung.aktDokument.nurzumlesen & myGlobalz.sitzung.aktDokument.FullnameCheckout)
        Dim fi As New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        System.Diagnostics.Process.Start(fi.DirectoryName)
        fi = Nothing
        Clipboard.Clear()
        Clipboard.SetText(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        fi = Nothing
        Close()
    End Sub

    Private Sub BtnZuConject_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim chromeFile As String = "CHROME.EXE" '"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
        Dim link As String = "https://ng.conject.com/ng"
        Process.Start(chromeFile, link)
    End Sub
End Class
