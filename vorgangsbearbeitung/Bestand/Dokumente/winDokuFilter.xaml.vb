Imports System.Data
 
Public Class winDokuFilter
    Property _nurZumKuckenModus As Boolean
    Public Property auswahlid As String
    Public Property aktSachgebietnr As String = ""
    Public Property typwahl As String = "alle"
    Public Property ladevorgangabgeschlossen As Boolean = False
    Public Property fuerBearbeiterName As String
    Public Property fuerBearbeiterId As Integer
    Private Sub winDokuFilter_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        'myGlobalz.BestandsFensterIstgeoeffnet = True
        'gastLayout()
        'initDGMaxHeight()
        'initDokumentDateiTypenCombo()
        'bestandTools.verschiedenes.beteiligteFilternAktivieren(btnBeteiligteFiltern)
        initStartPositionOnScreen()
        btnBearbeiterauswahl.Content = myGlobalz.sitzung.aktBearbeiter.Name
        'btnBearbeiterauswahl.Content = bearbeiterauswahlbox.auswahlNAchname
        ladevorgangabgeschlossen = True
    End Sub
    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
    End Sub
    Private Sub btnDokListeRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        dokumentsuchen()
    End Sub

    Private Sub dgDokumente_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        e.Handled = True
        If dgDokumente.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgDokumente.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try

        Try
            item = CType(dgDokumente.SelectedItem, DataRowView)
        Catch ex As Exception
            nachricht(ex.ToString)
            Exit Sub
        End Try
        If _nurZumKuckenModus Then
            myGlobalz.sitzung.BestandsAuswahlVID = CInt(clsDBtools.fieldvalue(item("vorgangsid")))
            e.Handled = True
            Close()
            Exit Sub
        End If

        DokArc.Archiv_definiereAktdokument(item)
        If chkZUmVorgangspringen.IsChecked Then
            'Dim vid As Integer = detailsTools.vorgangzuDokidFinden(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktDokument.DocID)
            Dim vid As Integer = myGlobalz.sitzung.aktDokument.VorgangsID
            myGlobalz.sitzung.aktVorgangsID = CInt(vid)
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            glob2.editVorgang(CInt(vid), myGlobalz.testmode)

        Else
            myGlobalz.sitzung.aktVorgangsID = 0
            'Dim darst As Boolean = detailsTools.Archiv_aktiviere_Dokument()
        End If
        e.Handled = True
    End Sub

    Private Sub NeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        CLstart.mycSimple.neuerVorgang3("modus=normal")
        e.Handled = True
    End Sub

    Private Sub ZuvorgangsNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim az As String = "", header As String = ""
        clsStartup.suchenNachVorgaengen(az, header)
        Close()
        e.Handled = True
    End Sub
    Private Function Eingabeok(ByVal p1 As String) As Boolean
        'If String.IsNullOrEmpty(p1) Then
        '    MsgBox("Eingabe unvollständig. Bitte geben Sie einen Suchtext an!")
        '    Return False
        'End If
        Return True
    End Function

    Private Shared Sub getDokuDT()
        Dim result As Long = initDokumenteDatatable("")
        If result < 1 Then
            If myGlobalz.sitzung.tempREC.mycount < 1 Then
                nachricht("Es wurden keine Vorgänge in der DB gefunden")
            End If
            Exit Sub
        End If
    End Sub

    Public Shared Function initDokumenteDatatable(ByVal hinweis As String) As Long
        Try
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            Dim retval As Long = myGlobalz.sitzung.tempREC.mycount
            nachricht("initDokumenteDatatable Treffer: " & retval)
            Return retval
        Catch ex As Exception
            nachricht_und_Mbox("initDokumenteDatatable: " & ex.ToString)
            Return -1
        End Try
    End Function
    Private Sub dokumentsuchen()
        Dim bsql As SQL_Dokumente
        Try
            If Not Eingabeok(tbDokfilter.Text) Then Exit Sub
            If fuerBearbeiterName.IsNothingOrEmpty Then
                fuerBearbeiterName = myGlobalz.sitzung.aktBearbeiter.Initiale
                fuerBearbeiterId = myGlobalz.sitzung.aktBearbeiter.ID
            End If
            bsql = New SQL_Dokumente(myGlobalz.beteiligte_MYDB.dbtyp) With {.doktypfilter = typwahl,
                                                                            .doktextfilter = tbDokfilter.Text,
                                                                            .sachgebietnr = (tbsachgebietnr.Text),
                                                                            .inBeschreibungSuchen = CBool(chkInBeschreibung.IsChecked),
                                                                             .inDateinamesuchen = CBool(chkInDateiName.IsChecked),
                                                                             .fuerBearbeiterName = fuerBearbeiterName,
                                                                             .fuerBearbeiterId = fuerBearbeiterId,
                                                                             .undMitarbeiter = CBool(chkUndMitarbeiter.IsChecked)}
            bsql.compoze()
            myGlobalz.sitzung.tempREC.mydb.SQL = bsql.result
            bsql = Nothing
            '	holeVorgaengeMitpersonen
            nachricht(myGlobalz.sitzung.tempREC.mydb.SQL)
            dgDokumente.DataContext = Nothing
            getDokuDT()
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                dgDokumente.DataContext = Nothing
            Else
                dgDokumente.DataContext = myGlobalz.sitzung.tempREC.dt
            End If
            tbDokumenteTreffer.Text = myGlobalz.sitzung.tempREC.dt.Rows.Count.ToString
        Catch ex As Exception
            nachricht("fehler ind dokumentsuchen:" & ex.ToString)
        End Try
    End Sub
    Private Sub btnClearDokumente_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        tbDokfilter.Text = ""
        cmbDateitypen.SelectedValue = ""
        dgDokumente.DataContext = Nothing
        e.Handled = True
    End Sub
    Private Sub btnStammdatenFiltern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandStammdaten(False)
        e.Handled = True
    End Sub
    Private Sub btnBeteiligteFiltern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        clsStartup.FormularBestandBeteiligte(_nurZumKuckenModus)
        e.Handled = True
    End Sub


    Private Sub btnEreignisfilter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandEreignis(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnfstSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandFlurstueckfilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnadrSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandAdressFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub



    Private Sub btnProjektsuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandProjektFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub cmbDateitypen_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If Not ladevorgangabgeschlossen Then Exit Sub
        If cmbDateitypen.SelectedItem Is Nothing Then Exit Sub
        Dim myvali$, myvals$
        Dim test As New ComboBoxItem
        test = CType(cmbDateitypen.SelectedItem, ComboBoxItem)

        typwahl = CStr(test.Tag)
        'Dim myvalx = CType(cmbDateitypen.SelectedItem, System.Xml.XmlElement)
        'myvals = myvalx.Attributes(1).Value.ToString
        'myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali)
        ' myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = myvals
        e.Handled = True
    End Sub
    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        detailsTools.VorgangLocking("aus")
        savePosition()
    End Sub
    'Private Sub initDGMaxHeight()
    '    dgDokumente.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    'End Sub

    'Private Sub gastLayout()
    '    If glob2.userIstinGastModus Then
    '        Background = New SolidColorBrush(Colors.Red)
    '        stckp1.Background = New SolidColorBrush(Colors.Red)
    '    End If
    'End Sub

    Sub initDokumentDateiTypenCombo()

        'existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\DokumentDateiTypen.xml")
    End Sub


    Private Sub savePosition()
        If myGlobalz.nurEinBildschirm Then Exit Sub
        Try
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winbestandformpositiontop", CType(Me.Top, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winbestandformpositionleft", CType(Me.Left, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winbestandformpositionwidth", CType(Me.ActualWidth, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winbestandformpositionheight", CType(Me.ActualHeight, String))
        Catch ex As Exception
            l("fehler in saveposition  windb" & ex.ToString)
        End Try
    End Sub

    Private Sub initStartPositionOnScreen()
        If myGlobalz.nurEinBildschirm Then Exit Sub
        Dim topval = (CLstart.formposition.getPosition("diverse", "winbestandformpositiontop", Me.Top))
        If topval < 0 Then
            Me.Top = 0
        Else
            Me.Top = topval
        End If
        Me.Left = CLstart.formposition.getPosition("diverse", "winbestandformpositionleft", Me.Left)
        Me.Width = CLstart.formposition.getPosition("diverse", "winbestandformpositionwidth", Me.Width)
        Me.Height = CLstart.formposition.getPosition("diverse", "winbestandformpositionheight", Me.Height)
    End Sub




    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandWvFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnSgtree2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim sgt As New win_sgtree(myGlobalz.Paradigma_Sachgebietsdatei)
        sgt.ShowDialog()

        If sgt.publicNR IsNot Nothing Then
            CLstart.myc.AZauswahl.az.sachgebiet.Zahl = sgt.publicNR
            CLstart.myc.AZauswahl.az.sachgebiet.Header = sgt.publicsgHeader
            tbsachgebietnr.Text = CLstart.myc.AZauswahl.az.sachgebiet.Zahl.ToString
        End If
        sgt = Nothing
        GC.Collect()
        dgDokumente.DataContext = Nothing
        ' suchentaste()
    End Sub
    Private Sub tbsachgebietnr_TextChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbsachgebietnr.TextChanged
        If tbsachgebietnr Is Nothing Then Exit Sub
        Dim aktiv As Boolean = Not String.IsNullOrEmpty(tbsachgebietnr.Text)
        'textboxRahmenAktiveinfaerben(aktiv, tbsachgebietnr)
        aktSachgebietnr = tbsachgebietnr.Text
        'dgStammListeClear()
        dgDokumente.DataContext = Nothing
        e.Handled = True
    End Sub
    Private Sub cmbUserChange(auswahlBearbeiter As String)
        Dim item As String = CType(auswahlBearbeiter, String)
        If item Is Nothing Then
            fuerBearbeiterName = ""
            fuerAlleBearbeiter = True
        Else
            fuerBearbeiterName = item
            If fuerBearbeiterName.ToLower = "alle" Then
                fuerAlleBearbeiter = True
            Else
                fuerAlleBearbeiter = False
            End If
        End If
    End Sub
    Private Sub btnBearbeiterauswahl_Click_1(sender As Object, e As RoutedEventArgs)
        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl
        bearbeiterauswahlbox.ShowDialog()
        If Not String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlInitiale) Then
            If bearbeiterauswahlbox.auswahlNAchname.ToString = "alle" Then
                fuerBearbeiterName = "alle"
                fuerBearbeiterId = 0
            End If
            fuerBearbeiterId = bearbeiterauswahlbox.auswahlBearbeiterid

            'aktiviereGBZeitraumkontrolle(bearbeiterauswahlbox)
            cmbUserChange(bearbeiterauswahlbox.auswahlInitiale)
            If String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlVorname) Then
                btnBearbeiterauswahl.Content = bearbeiterauswahlbox.auswahlNAchname
            Else
                btnBearbeiterauswahl.Content = bearbeiterauswahlbox.auswahlNAchname & ", " & bearbeiterauswahlbox.auswahlVorname
            End If
            '  suchentaste()
        End If
        dgDokumente.DataContext = Nothing
        e.Handled = True
    End Sub
End Class
