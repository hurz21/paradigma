Public Class winOtherVorlagensteuerung
    Public Property quelldatei As String
    Property quellpfad As String
    Property ereignisanlegen As Boolean = False
    Property ereingistyp As String = ""
    Property ereingistitel As String = ""
    Property dokumentschlagworte As String = ""
    Property abbruch As Boolean = False
    Property _eid As Integer = 0
    Public Property war_abbruch As Boolean = False
    Sub New(ByVal _quelldatei As String, ByVal _pfad As String, eid As Integer)
        InitializeComponent()
        quelldatei = _quelldatei
        quellpfad = _pfad
        _eid = eid
    End Sub
    Private Sub winOtherVorlagensteuerung_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        detailsTools.initErgeinistypCombo(Me, "Vorlage_ereignisseTypen.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
        detailsTools.initErgeinistypCombo(Me, "vorlage_DokumentSchlagworte.xml", "XMLSourceComboBoxDokumentSchlagworte") : cmbVerlaufAuswahl.SelectedIndex = 0
        If _eid > 0 Then
            chkMiteereignisanlegen.IsChecked = False
            grpEreignisanlegen.IsEnabled = False

        End If
        tbQuelldatei.Text = quelldatei
        e.Handled = True
    End Sub



    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        If eingabeOK() Then
            If cmbModus.SelectedIndex = 0 Then
                abbruch = True
                Close()
            Else
                ereignisanlegen = CBool(chkMiteereignisanlegen.IsChecked)
                ereingistyp = tbEreignisArt.Text
                ereingistitel = tbEreignisTitel.Text
                dokumentschlagworte = tbSchlagworte.Text
                myGlobalz.sitzung.aktDokument.Beschreibung = dokumentschlagworte
                Dim erfolg As Boolean = False
                Dim outfile As String = "" : Dim datei As String : Dim testendung As IO.FileInfo

                testendung = New IO.FileInfo(IO.Path.Combine(quellpfad, quelldatei))
                erfolg = clsVorlagedokumente.DocxVorlageVerwenden(outdatei:=outfile,
                                                                          vdatei:=IO.Path.Combine(quellpfad, quelldatei),
                                                                          Schlagworte:=myGlobalz.sitzung.aktDokument.Beschreibung,
                                                                          quellpfad:="",
                                                                          dateityp:=testendung.Extension.ToLower,
                                                                            "")



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

                MessageBox.Show("Die Datei wurde in das Archiv übernommen. (siehe Reiter 'Dokumente')", "Vorlagendatei übernommen", MessageBoxButton.OK, MessageBoxImage.Information)

                abbruch = False
                Close()
            End If
        Else
            'eingabe is tnicht ok
        End If
    End Sub




    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        abbruch = True
        e.Handled = True
        war_abbruch = True
        Me.Close()
    End Sub

    Private Sub cmbDokumentTypen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If cmbDokumentTypen.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbDokumentTypen.SelectedValue, String)
            If String.IsNullOrEmpty(item) Then Exit Sub
            tbSchlagworte.Text = item & " "
            cmbDokumentTypen.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" ,ex)
        End Try
    End Sub
    Private Sub cmbVerlaufAuswahl_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            If String.IsNullOrEmpty(item) Then Exit Sub
            tbEreignisArt.Text = item & " "
            cmbVerlaufAuswahl.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" ,ex)
        End Try
    End Sub

    Private Sub winOtherVorlagensteuerung_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing

    End Sub



    Sub haupt()
        'Dim neuu As New clsVorlagedokumente(datei)
        'myGlobalz.sitzung.aktDokument.DateinameMitExtension = neuu.neuenNamenVerwendenBilden(testendung.Extension)
        'myGlobalz.sitzung.aktDokument.Beschreibung = testendung.Name.Replace(testendung.Extension, "")
        'myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.VorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
        'Dokument.createCheckoutDir(myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.VorgangsID)
        'neuu._VorlageDateiImArchiv.CopyTo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        'myGlobalz.sitzung.aktDokument.Filedatum = Now
        'ergebnis = clsVorlagenTools.XLSGetEinCheckErgebnis(myGlobalz.sitzung.aktDokument.Filedatum)
        'If Not String.IsNullOrEmpty(ergebnis) Then MsgBox(ergebnis)
    End Sub

    Private Function eingabeOK() As Boolean
        If chkMiteereignisanlegen.IsChecked Then
            If String.IsNullOrEmpty(tbEreignisArt.Text) Then
                MsgBox("Sie wollen ein Ereignis erzeugen, haben aber keine Ereignisart angegeben!", MsgBoxStyle.Critical, "Eingabe unvollständig")
                FocusManager.SetFocusedElement(Me, tbEreignisArt)
                Return False
            End If
            If String.IsNullOrEmpty(tbEreignisTitel.Text) Then
                MsgBox("Sie wollen ein Ereignis erzeugen, haben aber keinen Titel angegeben!", MsgBoxStyle.Critical, "Eingabe unvollständig")
                FocusManager.SetFocusedElement(Me, tbEreignisTitel)
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub chkMiteereignisanlegen_Checked(sender As Object, e As RoutedEventArgs) Handles chkMiteereignisanlegen.Checked
        If tbEreignisArt.Text.IsNothingOrEmpty Then
            cmbVerlaufAuswahl.IsDropDownOpen = True
        End If
        e.Handled = True
    End Sub

    Private Sub cmbModus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        nachricht("not implemented: cmbModus_SelectionChanged")
        e.Handled = True
    End Sub
End Class
