Public Class winOtherVorlagensteuerung
    Public Property quelldatei As String
    Property quellpfad As String
    Property mitereignis As Boolean = False
    Property ereingistyp As String = ""
    Property ereingistitel As String = ""
    Property dokumentschlagworte As String = ""
    Property abbruch As Boolean = False

    Private Sub Button_Click_1(sender As Object, e As RoutedEventArgs)
        If eingabeOK() Then
            If cmbModus.SelectedIndex = 0 Then
                abbruch = True
                Close()
            Else
                mitereignis = CBool(chkMiteereignisanlegen.IsChecked)
                ereingistyp = tbEreignisArt.Text
                ereingistitel = tbEreignisTitel.Text
                dokumentschlagworte = tbSchlagworte.Text
                abbruch = False
                Close()
            End If
        Else
            'eingabe is tnicht ok
        End If
    End Sub

    Sub New(ByVal _quelldatei As String, ByVal _pfad As String)
        InitializeComponent()
        quelldatei = _quelldatei
        quellpfad = _pfad
    End Sub


    Private Sub Button_Click_2(sender As Object, e As RoutedEventArgs)
        abbruch = True
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
            nachricht("cmbVerlaufAuswahl_SelectionChanged" & ex.ToString)
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
            nachricht("cmbVerlaufAuswahl_SelectionChanged" & ex.ToString)
        End Try
    End Sub

    Private Sub winOtherVorlagensteuerung_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub winOtherVorlagensteuerung_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        detailsTools.initErgeinistypCombo(Me, "Vorlage_ereignisseTypen.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
        detailsTools.initErgeinistypCombo(Me, "vorlage_DokumentSchlagworte.xml", "XMLSourceComboBoxDokumentSchlagworte") : cmbVerlaufAuswahl.SelectedIndex = 0
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
