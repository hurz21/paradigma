Imports System.Data
 
Public Class winDokuFilter
    Property _nurZumKuckenModus As Boolean
    Public Property auswahlid As String

    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
    End Sub
    Private Sub btnDokListeRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        dokumentsuchen()
        e.Handled = True
    End Sub

    Private Sub dgDokumente_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
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
            Dim vid As Integer = detailsTools.vorgangzuDokidFinden(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktDokument.DocID)
            myGlobalz.sitzung.aktVorgangsID = CInt(vid)

            glob2.editVorgang(CInt(vid))

        Else
            myGlobalz.sitzung.aktVorgangsID = 0
            Dim darst As Boolean = detailsTools.Archiv_aktiviere_Dokument()
        End If
        e.Handled = True
    End Sub

    Private Sub NeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        clsStartup.NeuerVorgang2()
        e.Handled = True
    End Sub

    Private Sub ZuvorgangsNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim az As String = "", header As String = ""
        clsStartup.suchenNachVorgaengen(az, header)
        Close()
        e.Handled = True
    End Sub
    Private Function Eingabeok(ByVal p1 As String) As Boolean
        If String.IsNullOrEmpty(p1) Then
            MsgBox("Eingabe unvollständig. Bitte geben Sie einen Suchtext an!")
            Return False
        End If
        Return True
    End Function

    Private Shared Sub zeigeDokumente()
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
        Dim typfilter$ = ""
        Dim bsql As SQL_Dokumente
        Try
            If cmbDateitypen.SelectedValue IsNot Nothing Then
                typfilter = cmbDateitypen.SelectedValue.ToString
            End If

            If Not Eingabeok(tbDokfilter.Text) Then Exit Sub
            bsql = New SQL_Dokumente(myGlobalz.beteiligte_MYDB.dbtyp) With {.doktypfilter = typfilter,
                                                   .doktextfilter = tbDokfilter.Text}
            bsql.compoze()
            myGlobalz.sitzung.tempREC.mydb.SQL = bsql.result
            bsql = Nothing
            '	holeVorgaengeMitpersonen
            nachricht(myGlobalz.sitzung.tempREC.mydb.SQL)
            dgDokumente.DataContext = Nothing
            zeigeDokumente()
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
        If cmbDateitypen.SelectedItem Is Nothing Then Exit Sub
        Dim myvali$, myvals$
        myvali = CStr(cmbDateitypen.SelectedValue)
        Dim myvalx = CType(cmbDateitypen.SelectedItem, System.Xml.XmlElement)
        myvals = myvalx.Attributes(1).Value.ToString
        'myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali)
        ' myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = myvals
        e.Handled = True
    End Sub
    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        detailsTools.VorgangLocking("aus")
    End Sub
    Private Sub initDGMaxHeight()
        dgDokumente.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Private Sub gastLayout()
        If glob2.userIstinGastModus Then
            Background = New SolidColorBrush(Colors.Red)
            stckp1.Background = New SolidColorBrush(Colors.Red)
        End If
    End Sub

    Sub initDokumentDateiTypenCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceDateiTypen"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\DokumentDateiTypen.xml")
    End Sub

    Private Sub winDokuFilter_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        myGlobalz.BestandsFensterIstgeoeffnet = True
        gastLayout()
        initDGMaxHeight()
        initDokumentDateiTypenCombo()
        bestandTools.verschiedenes.beteiligteFilternAktivieren(btnBeteiligteFiltern)
        e.Handled = True
    End Sub
    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandWvFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub
End Class
