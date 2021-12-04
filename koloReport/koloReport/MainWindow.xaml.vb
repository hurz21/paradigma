Imports System.ComponentModel
Imports koloReport

Class MainWindow
    Public vid As String
    Public ereignisid, bearbeiterid As String
    Public mitGISkarteFullName As String
    Public logfile As String
    Public dokArcPfad As String
    Public scalierfaktor As Double
    Public schonImArchiv As Boolean = True
    Public ausgabeDocx As String
    Public formisloaded As Boolean = False
    Public ausgabeDateiFormat As String = ".docx" ' oder .docx
    Public zweispaltig As Boolean = True
    Private fototitelSize As String
    Public az As String
    Public modusFotoUntertitel As String
    Public reportModus As String
    Public ueberschriftAktennotiz As String = ""


    Sub New()
        InitializeComponent()
    End Sub
    Function loesch() As Boolean
        Return True
    End Function

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
#If Not DEBUG Then
    IO.Directory.SetCurrentDirectory("C:\kreisoffenbach\koloreport")
#End If

        aktdoku = New Dokument
        Dim kommando As String
        Dim mitExtraZeilenumbruch As Boolean = False
        dokArcPfad = "\\file-paradigma\Paradigma\test\paradigmaArchiv\backup\archiv"
        dossierordner = "C:\Users\" & Environment.UserName & "\Desktop\Paradigma\dossier\"
        logfile = "O:\UMWELT\B\GISDatenEkom\div\logging\kolodossier\" & Environment.UserName & ".log"
        kommando = Environment.CommandLine.ToLower
#If DEBUG Then
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=33599#eid=103205#c:\feinen_j_merge_201681054275paradigma_.png".ToLower
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=33599#eid=103295#\\w2gis02\gdvell\cache\gis\feinen_j_merge_20169945183paradigma_.png"
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=33382#eid=107850#\\w2gis02\gdvell\cache\gis\feinen_j_merge_20169945183paradigma_.png"
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=30275#eid=84552#\\w2gis02\gdvell\cache\gis\feinen_j_merge_20169945183paradigma_.png"
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=9609#eid=108769#c:\feinen_j_merge_201681054275paradigma_.png".ToLower
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=34228#eid=109365#c:\feinen_j_merge_201681054275paradigma_.png".ToLower
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=31188#eid=107770#c:\feinen_j_merge_201681054275paradigma_.png#modus=ereignisdossier".ToLower
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=9609#eid=203458#c:\feinen_j_merge_201681054275paradigma_.png.ToLower#modus=aktennotiz"
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=9609#eid=0#\\w2gis02\gdvell\cache\gis\feinen_j_merge_20162771913paradigma_.png#modus=aktennotiz"
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=9609#eid=122174#c:\feinen_j_merge_201681054275paradigma_.png#modus=ereignisdossier".ToLower
        'kommando = "c:\kreisoffenbach\koloreport\koloreport.exe /vid=46229#eid=202752#c:\feinen_j_merge_201681054275paradigma_.png#modus=ereignisdossier".ToLower

        kommando = "c:\kreisoffenbach\koloreport\koloreport.exe  /vid=9609#eid=0##modus=aktennotiz#bid=1"
#End If
        logf = New IO.StreamWriter(logfile)
        logf.AutoFlush = False
        l("kommando " & kommando)

        If kommando.Contains("vid") Then
            getParams(kommando, vid, ereignisid, mitGISkarteFullName, reportModus, bearbeiterid)
            '  Debug.Print(">>>> " & Environment.CommandLine)
        Else
            'vid = "33599"
            'vid = "33382"
            'ereignisid = "108755"
            'ereignisid = "108769"
            'ereignisid = "103295"
            'ereignisid = "107850" 
            'vid = "30275"
            'ereignisid = "84552"
        End If
        l("vid " & vid)
        l("ereignisid " & ereignisid)
        l("bearbeiterid " & bearbeiterid)
        l("mitGISkarteFullName " & mitGISkarteFullName)
        l("reportModus " & reportModus)

        Title = "Erstelle Ereignis-Dossier   Vg.: " & vid & " / " & ereignisid & ". ReportModus=" & reportModus & ", db: " & ParadigmaDBtyp
        tbVorgangsinfo.Text = "Vorgang: " & vid & ", " & " Ereignisid: " & ereignisid & ", " & " bearbeiterid: " & bearbeiterid
        Dim jaEsgibtfotos As Boolean
        If reportModus <> "aktennotiz" Then
            jaEsgibtfotos = esGibtFotos()
        End If

        If jaEsgibtfotos Then
            chkMitFotos.IsChecked = True
        Else
            chkMitFotos.IsChecked = False
        End If
        If Not chkMitFotos.IsChecked Then
            setLayout("5")
        Else
            setLayout("3")
        End If

        If jaEsgibtfotos Then
            'Else
            chkZweispaltig.IsChecked = False
        End If
        If reportModus = "ereignisdossier" Then
            'nix
        End If
        If reportModus = "aktennotiz" Then
            chkMitFotos.IsChecked = False
            ueberschriftAktennotiz = getUeberschriftAktennotiz()
            If ueberschriftAktennotiz.Trim = String.Empty Then
                MessageBox.Show("Es wurde keine Überschrift angegeben. Abbruch")
                Close()
                ueberschriftAktennotiz = "Es wurde keine Überschrift angegeben"
            Else
                aktennotiz(ueberschriftAktennotiz, vid, mitExtraZeilenumbruch)
            End If

        End If
        '    chkMitFotos.Visibility = Visibility.Collapsed
        formisloaded = True
    End Sub



    Private Sub btnStart_Click(sender As Object, e As RoutedEventArgs)
        startKnopf()

        e.Handled = True
    End Sub

    Private Function esGibtFotos() As Boolean
        Dim dt As System.Data.DataTable = FotoDokumentDatenHolen(ereignisid)
        If dt.Rows.Count > 0 Then
            Return True
        Else
            Return False
        End If
        'e.Handled = True
    End Function

    Private Sub btnabbruch_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub btnVorschau_Click(sender As Object, e As RoutedEventArgs)
        tbProtokoll.Text = "Dateivorschau"
        If ausgabeDateiFormat = ".docx" Then
            Threading.Thread.Sleep(1000)
            Dim dok As New Dokument
            dok.FullnameCheckout = tbAusgabedatei.Text
            clsWord.WordReadonlyDokumentStarten(dok)
            'System.Diagnostics.Process.Start("WORDVIEW.exe", tbAusgabedatei.Text.Replace(".pdf", ".docx"))       
          '  System.Diagnostics.Process.Start(tbAusgabedatei.Text.Replace(".pdf", ".docx"))
        End If
        If ausgabeDateiFormat = ".pdf" Then
            Threading.Thread.Sleep(1000)
            System.Diagnostics.Process.Start(tbAusgabedatei.Text.Replace(".docx", ".pdf"))
        End If
        e.Handled = True
    End Sub

    Private Sub btnInsaArchiv_Click(sender As Object, e As RoutedEventArgs)
        '   clearInfofenster("InsArchiv ist noch nicht fertig")
        '   MsgBox("Baustelle")
        l("BUTTON insarchiv wurde gedrückt==========================================")
        dateiInsArchivSchieben("Dossier für Ereignis: ", bearbeiterid)
        e.Handled = True
    End Sub

    Private Sub dateiInsArchivSchieben(beschreibung As String, bearbeiterid As String)
        l("dateiInsArchivSchieben-----------------------------")
        Dim hinweis As String = insarchivDamit(ausgabeDocx, vid, ereignisid, dokArcPfad, beschreibung, bearbeiterid)
        If hinweis.ToLower = "ok" Then
            MessageBox.Show("Die Datei wurde dem Vorgang erfolgreich hinzugefügt. " & Environment.NewLine & Environment.NewLine &
                            " >>>   Es wird nun zurück ins Paradigma gewechselt. " & Environment.NewLine & Environment.NewLine &
                            "Die neue Worddatei liegt in der Liste ganz oben.")
            schonImArchiv = True
            l("dateiInsArchivSchieben-----------------fertif-----")
            Close()
        Else
            MessageBox.Show("Die Datei wurde dem Vorgang NICHT erfolgreich hinzugefügt. " & Environment.NewLine & Environment.NewLine &
                           " >>>   Bitte wenden Sie sich an den Admin." & Environment.NewLine &
                           hinweis, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error)
        End If
    End Sub

    Private Sub btnopendir_Click(sender As Object, e As RoutedEventArgs)
        tbProtokoll.Text = "Dossierverzeichnis wird geöffnet"
        System.Diagnostics.Process.Start(dossierordner)
        e.Handled = True
    End Sub

    Private Sub chkMitFotos_Click(sender As Object, e As RoutedEventArgs)
        clearInfofenster("")
        If chkMitFotos.IsChecked Then
            border1.IsEnabled = True
            chkZweispaltig.IsChecked = True
        Else
            border1.IsEnabled = False
            chkZweispaltig.IsChecked = False
        End If
        e.Handled = True
    End Sub

    Private Sub chkMitUnterTitelFuerFotos_Click(sender As Object, e As RoutedEventArgs)
        clearInfofenster("")
        e.Handled = True
    End Sub

    'Private Sub chkanzahlfotos_Click(sender As Object, e As RoutedEventArgs)
    '    clearInfofenster("")
    '    e.Handled = True
    'End Sub

    Private Sub chkmitKarte_Click(sender As Object, e As RoutedEventArgs)
        clearInfofenster("")
        e.Handled = True
    End Sub

    Private Sub clearInfofenster(v As String)
        tbProtokoll.Text = v
        stackResult.IsEnabled = False
    End Sub

    Private Sub cmbSkalier_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        'Dim stringValue = cmbSkalier.SelectedValue.ToString()
        If Not formisloaded Then Exit Sub
        Dim auswahl As String
        Dim text As String
        auswahl = DirectCast(cmbSkalier.SelectedItem, ComboBoxItem).Content.ToString()
        Select Case auswahl
            Case "groß"
                text = "5"
            Case "mittel"
                text = "3"
            Case "klein"
                text = "2"
        End Select
        '   Dim text As String = DirectCast(cmbSkalier.SelectedItem, ComboBoxItem).Content.ToString()
        setLayout(text)
        clearInfofenster("")
        e.Handled = True
    End Sub

    Private Sub setLayout(text As String)
        Select Case text
            Case "5"
                tbSkalierefaktor.Text = "5"
                tbSkalierinfo.Text = "entspr. etwa 2 Fotos/Seite"
                scalierfaktor = 5
                '  zweispaltig = False
                chkZweispaltig.IsChecked = False
            Case "3"
                tbSkalierefaktor.Text = "3"
                tbSkalierinfo.Text = "entspr. etwa 4 Fotos/Seite"
                scalierfaktor = 3
                'zweispaltig = True
                chkZweispaltig.IsChecked = True
            Case "2.5"
                tbSkalierefaktor.Text = "2.5"
                tbSkalierinfo.Text = "entspr. etwa 4 Fotos/Seite"
                scalierfaktor = 2.5
                'zweispaltig = True
                chkZweispaltig.IsChecked = True
            Case "2"
                tbSkalierefaktor.Text = "2"
                tbSkalierinfo.Text = "entspr. etwa 6 Fotos/Seite"
                scalierfaktor = 2
                'zweispaltig = True
                chkZweispaltig.IsChecked = True
            Case "1"
                tbSkalierefaktor.Text = "1"
                tbSkalierinfo.Text = "entspr. etwa 9 Fotos/Seite"
                scalierfaktor = 1
                'zweispaltig = True
                chkZweispaltig.IsChecked = True
        End Select
    End Sub

    Private Sub MainWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim myval As MessageBoxResult
        If Not schonImArchiv Then
            myval = MessageBox.Show("Sie haben Ihr Ergebnis noch nicht im Archiv abgespeichert!" & Environment.NewLine &
                            "Möchten Sie trotzdem das Formular verlassen!", "Nicht gespeichert", MessageBoxButton.OKCancel, MessageBoxImage.Question)
            If myval = MessageBoxResult.OK Then
                End
            Else
                e.Cancel = True
                Exit Sub
            End If
        End If
    End Sub

    Private Sub chkPDFDoc_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim text As String
        If formisloaded Then
            text = DirectCast(cmbDFDoc.SelectedItem, ComboBoxItem).Content.ToString()
            ausgabeDateiFormat = text
            If text.ToLower = ".pdf" Then
                'ausgabeDateiFormat = ".pdf"
                btnDrucken.IsEnabled = False
            End If
            If text.ToLower = ".docx" Then
                'ausgabeDateiFormat = ".docx"
                btnDrucken.IsEnabled = True
            End If
        End If
        e.Handled = True
    End Sub

    Private Sub chkZweispaltig_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    'Private Sub btnInsaArchivUndWord_Click(sender As Object, e As RoutedEventArgs)
    '    MsgBox("Baustelle")
    '    e.Handled = True
    'End Sub

    Private Sub btnWord_Click(sender As Object, e As RoutedEventArgs)
        oeffneWordDatei()
        e.Handled = True
    End Sub

    Private Sub oeffneWordDatei()
        tbProtokoll.Text = "Dateibearbeiten"
        If ausgabeDateiFormat = ".docx" Then
            Threading.Thread.Sleep(1000)
            System.Diagnostics.Process.Start("WINWORD.exe", tbAusgabedatei.Text.Replace(".pdf", ".docx"))
            tbProtokoll.Text = "Sie werden die Datei bearbeiten! " &
                " >>> Wichtig  <<<" & Environment.NewLine &
                " Wenn Sie die Datei ins Archiv übernehmen wollen: " & Environment.NewLine &
                "   1. Sobald Sie mit der Bearbeitung fertig sind, speichern und schließen sie Word! " & Environment.NewLine &
                "   2. Drücken Sie die 'Ins Archiv übernehmen' -Taste !!!"
            schonImArchiv = False
        Else
            MsgBox("PDF-dateien lassen sich nicht bearbeiten")
        End If
    End Sub

    Private Sub btnDrucken_Click(sender As Object, e As RoutedEventArgs)
        l("BUTTON drucken wurde gedrückt==========================================")
        If ausgabeDateiFormat = ".docx" Then
            l("vor drucken")
            nachZielKopieren.dokumentdrucken.exe(ausgabeDocx)
        Else
            MsgBox("funzt nicht bei PDF!!!!")
        End If

        e.Handled = True
    End Sub
    Sub startKnopf()
        Dim fotosMitUnterTitel As Boolean
        Dim mitExtraZeilenumbruch As Boolean = False
        Dim anzahlfotosproseite As Integer = 3 ' 2 oder 3
        Dim standardfontsize As String = "20"
        If chkMitextraZeilenumbruch.IsChecked Then
            mitExtraZeilenumbruch = True
        Else
            mitExtraZeilenumbruch = False
        End If
        tbProtokoll.Text &= "Dossier wird erzeugt" & " ... Bitte warten ..." & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

        tbAusgabedatei.Text = dossierordner & vid & "_" & ereignisid & Now.ToString("-TC_yyMMddhhmmss") & ".docx"
        ausgabeDocx = tbAusgabedatei.Text
        If Not chkmitKarte.IsChecked Then
            mitGISkarteFullName = ""
        End If
        schonImArchiv = False
        l("  mitGISkarteFullName " & mitGISkarteFullName)
        fotosMitUnterTitel = CBool(chkMitUnterTitelFuerFotos.IsChecked)
        l("  fotosMitUnterTitel " & fotosMitUnterTitel)
        If IsNumeric(tbSkalierefaktor.Text) Then
            'scalierfaktor = Val(tbSkalierefaktor.Text)
            setLayout(tbSkalierefaktor.Text)
        Else
            MsgBox("Der Skalierfaktor ist keine Zahl. bitte ändern")
            Exit Sub
        End If
        If chkZweispaltig.IsChecked = True Then
            zweispaltig = True
        Else
            zweispaltig = False
        End If
        l("  anzahlfotosproseite " & anzahlfotosproseite)
        l("mit fotos " & CBool(chkMitFotos.IsChecked))
        l("dokArcPfad " & dokArcPfad)
        IO.Directory.CreateDirectory(dossierordner)
        l("dossierordner " & dossierordner)
        l("dossierordner erzeugt ")
        fototitelSize = "10"
        modusFotoUntertitel = "kurz"

        If koloReport(ereignisid, vid, fotosMitUnterTitel, anzahlfotosproseite, mitGISkarteFullName,
                      ausgabeDocx, CBool(chkMitFotos.IsChecked), dokArcPfad, scalierfaktor,
                       zweispaltig, fototitelSize, modusFotoUntertitel,
                      standardfontsize, mitExtraZeilenumbruch) Then
            tbProtokoll.Text = "Das Dossier wurde erzeugt. Es wurde aber nicht ins Archiv übernommen. Der Ordner " & Environment.NewLine &
                                Environment.NewLine &
                                dossierordner & Environment.NewLine &
                                Environment.NewLine &
                               "wird geöffnet. Sie können die Datei bei Interesse händisch ins Archiv übernehmen."
            If ausgabeDateiFormat = ".pdf" Then
                If erzeugePDFA(ausgabeDocx, ausgabeDocx.Replace(".docx", ".pdf")) Then
                Else
                    MsgBox("Bei der Erzeugung der PDF-Datei gab es ein Problem. " & Environment.NewLine &
                           "Es wird die DOCX-Datei geöffnet")
                    ausgabeDateiFormat = ".docx"
                    cmbDFDoc.SelectedIndex = 1
                End If
            End If

            stackResult.IsEnabled = True
        Else
            MsgBox("Es trat ein Problem auf. Bitte an den Admin wenden!")
        End If
    End Sub
    Private Sub aktennotiz(ueberschrift As String, vid As String, mitExtraZeilenumbruch As Boolean)
        Dim fotosMitUnterTitel As Boolean
        Dim anzahlfotosproseite As Integer = 3 ' 2 oder 3
        Dim standardfontsize As String = "26"
        tbProtokoll.Text &= "Dossier wird erzeugt" & " ... Bitte warten ..." & Environment.NewLine
        'Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        tbProtokoll.Text &= "Neues Ereignis wird erzeugt" & " ... Bitte warten ..." & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dim aktEreignis As New clsEreignis
        aktEreignis = NeuesEreigniserzeugen(ueberschrift) 'myGlobalz.sitzung.aktEreignis
        'If ParadigmaDBtyp = "oracle" Then
        '    Neu_speichern_EreignisOracle(CInt(vid), "neu", aktEreignis)
        'End If
        If ParadigmaDBtyp = "sqls" Then
            clsSQLS.Neu_speichern_EreignisSQLS(CInt(vid), "neu", aktEreignis)
        End If
        ereignisid = CType(aktEreignis.ID, String)
        MeinGarbage()
        tbAusgabedatei.Text = dossierordner & "Aktennotiz_" & ereignisid & Now.ToString("_yyMMddhhmmss") & "-TC_.docx"
        ausgabeDocx = tbAusgabedatei.Text
        schonImArchiv = False
        l("  mitGISkarteFullName " & mitGISkarteFullName)
        fotosMitUnterTitel = False
        l("  fotosMitUnterTitel " & fotosMitUnterTitel)
        mitGISkarteFullName = ""
        schonImArchiv = False
        zweispaltig = False
        IO.Directory.CreateDirectory(dossierordner)
        l("dossierordner " & dossierordner)
        l("dossierordner erzeugt ")
        fototitelSize = "10"
        modusFotoUntertitel = "kurz"
        If koloReport(ereignisid, vid, fotosMitUnterTitel, anzahlfotosproseite, mitGISkarteFullName,
                      ausgabeDocx, False, dokArcPfad, scalierfaktor,
                      zweispaltig, fototitelSize, modusFotoUntertitel,
                      standardfontsize, mitExtraZeilenumbruch) Then
            tbProtokoll.Text = "Das Dossier wurde erzeugt. Es wurde aber nicht ins Archiv übernommen. Der Ordner " & Environment.NewLine &
                                Environment.NewLine &
                                dossierordner & Environment.NewLine &
                                Environment.NewLine &
                               "wird geöffnet. Sie können die Datei bei Interesse händisch ins Archiv übernehmen."

            stackResult.IsEnabled = True
            'oeffneWordDatei()     
            dateiInsArchivSchieben("Aktennotiz " & ueberschrift, bearbeiterid)
            'MsgBox("Die aktennotiz wurde ins archiv geschoben. sie können sie unter dem eiter 'Dokumente' aufrufen und bearbeiten")
            End
            btnStart.IsEnabled = False
        Else
            MsgBox("Es trat ein Problem auf. Bitte an den Admin wenden!")
        End If
    End Sub



    Private Shared Function getUeberschriftAktennotiz() As String
        Dim ueb As String = Microsoft.VisualBasic.Interaction.InputBox("Bitte geben Sie eine Überschrift ein:  (Mind. 4 Zeichen)" & Environment.NewLine &
                              Environment.NewLine &
                            "z.Beisp. 'Zustimmung AntragstellerIn'" & Environment.NewLine &
                            "oder     'Ablehnung AntragstellerIn'" & Environment.NewLine &
                            "oder:    'Gespräch mit ...'" & Environment.NewLine &
                            "oder:    'Begehung mit ...'" & Environment.NewLine &
                            "oder:    'Telefongespräch AntragstellerIn'", "Bitte eine Überschrift eingeben", "")
        If ueb.Trim.Length > 3 Then
            Return ueb
        Else
            Return ""
        End If
    End Function

    Private Sub chkMitextraZeilenumbruch_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
