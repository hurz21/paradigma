
Public Class winDokumenteEinchecken
    Private _filenamen() As String, _eid As Integer, _dasdir As String
    Private _vorschlagsDatei As String
    Private dokumentmoven, fotoZuRaumbezug As Boolean
    Private allg_beschreibung As String

    Sub New(ByVal filenamen() As String, ByVal eid As Integer, ByVal dasdir As String, vorschlagsDatei As String)
        InitializeComponent()
        _filenamen = filenamen
        _eid = eid
        _dasdir = dasdir
        _vorschlagsDatei = vorschlagsDatei
    End Sub

    Private Sub winDokumenteEinchecken_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0
        If _eid < 1 Then
            grpEreignis.Visibility = Windows.Visibility.Visible
        Else
            grpEreignis.Visibility = Windows.Visibility.Collapsed
        End If
        If Not _vorschlagsDatei.IsNothingOrEmpty Then
            ReDim _filenamen(0)
            _filenamen(0) = _vorschlagsDatei
            rbMove.IsChecked=true
        End If
        FelderInit()
        Title = StammToolsNs.setWindowTitel.exe("edit", "Dokumentenarchiv - Aufnahme")
        detailsTools.initErgeinistypCombo(Me, "dokument_schlagworte.xml", "XMLSourceComboBoxdokumentschlagworte")
        If String.IsNullOrEmpty(tbBeschreibung.Text) Then cmbTitelVorschlag.IsDropDownOpen = True
        
        FocusManager.SetFocusedElement(Me, BTNdateienauswaehlen)
        e.Handled = True
    End Sub

    Private Shared Sub initEreignis(ByVal neuereignis As clsEreignis, Beschreibung As String, art As String)
        With neuereignis
            .Art = art
            .Beschreibung = Beschreibung
            .Datum = Now
            .istRTF = False
            .Notiz = ""
            .DokumentID = 0
            .Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
            .Richtung = ""
        End With
    End Sub

    Private Sub btnWeiter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
          If chkEreignisMap.IsChecked And (tbTypEreignis.Text.Trim.IsNothingOrEmpty()) Then
            MsgBox("Sie müssen dem Ereignis einen Typ zuweisen!!! Abbruch.", MsgBoxStyle.Critical, "Ereignis anlegen")
            cmbVerlaufAuswahl.IsDropDownOpen=true
            e.Handled=true
            Exit sub
        
        End If

        allg_beschreibung = tbBeschreibung.Text
        If rbCopy.IsChecked Then dokumentmoven = False
        If rbMove.IsChecked Then dokumentmoven = True
        If chkFotoalsRaumbezug.IsChecked Then
            fotoZuRaumbezug = True
        Else
            fotoZuRaumbezug = False
        End If
        If Not _filenamen Is Nothing Then
            If CBool(chkEreignisMap.IsChecked) Then
                Dim neuereignis As New clsEreignis
                initEreignis(neuereignis, tbBeschreibungEreignis.Text, tbTypEreignis.Text)
                clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)
                clstart.myc.aLog.komponente = "Ereignis" : clstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
                    myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : clstart.myc.aLog.log()
                _eid = neuereignis.ID
            End If
            glob2.Archiv_eingang(_filenamen, allg_beschreibung, _eid, dokumentmoven, fotoZuRaumbezug, Now)
        End If
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
        If glob2.DokumenteAuswaehlen(_filenamen$, _dasdir) Then
            FelderInit()
            clsWindokueincheckenTOOL.quellverzeichnisspeichern(_dasdir)
        End If
    End Sub

    Private Sub FelderInit()
        If _filenamen IsNot Nothing Then
            tbAnzahldateien.Text = _filenamen.Count & " Stück"
        End If
        tbHinweis.Text = String.Format("Die Dokumente werden dem Vorgang {0} hinzugefügt !!! (Az.: {1})",
                                     myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        Dim test$ = clsWindokueincheckenTOOL.verzeichnis_isolieren(_filenamen)
        If String.IsNullOrEmpty(test$) Then
            clsWindokueincheckenTOOL.QuellverzeichnisseEinlesen(_dasdir)
        Else
            _dasdir = test
        End If
        lbDateien.DataContext = _filenamen
        tbQuellverzeichnis.Text = _dasdir
    End Sub


    Private Sub cmbTitelVorschlag_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If cmbTitelVorschlag.SelectedValue Is Nothing OrElse
                String.IsNullOrEmpty(cmbTitelVorschlag.SelectedValue.ToString) Then Exit Sub
            Dim item As String = CType(cmbTitelVorschlag.SelectedValue, String).Trim
            If Not String.IsNullOrEmpty(item) Then
                tbBeschreibung.Text = item
                cmbTitelVorschlag.SelectedValue = ""
            End If
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbTitelVorschlag_SelectionChanged" & ex.ToString)
        End Try
        e.Handled = True
    End Sub

    Private Sub chkEreignisMap_Click_1(sender As Object, e As RoutedEventArgs)
        tbBeschreibungEreignis.IsEnabled = True
        tbTypEreignis.Text = "Ortstermin"
        cmbVerlaufAuswahl.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub cmbVerlaufAuswahl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower = "hinzufügen" Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower.StartsWith("---") Then Exit Sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            tbTypEreignis.Text = item
            cmbVerlaufAuswahl.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" & ex.ToString)
            e.Handled = True
        End Try
    End Sub




    Private Sub chkEreignisMap_Checked(sender As Object, e As RoutedEventArgs) Handles chkEreignisMap.Checked
        If tbBeschreibungEreignis.Text.IsNothingOrEmpty() Then
            tbBeschreibungEreignis.Text = tbBeschreibung.Text
        End If
        e.Handled = True
    End Sub
End Class
