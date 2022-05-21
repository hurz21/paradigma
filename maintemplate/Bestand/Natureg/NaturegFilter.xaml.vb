Imports System.Data

Public Class NaturegFilter
    Private _makeSQL As Boolean
    Property _nurZumKuckenModus As Boolean
    Public Property auswahlid As String

    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
    End Sub
    Private Sub btnStammdatenFiltern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandStammdaten(False)
        e.Handled = True
    End Sub

    Private Sub abbruchclick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
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
    Private Sub btnEreignisfilter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandEreignis(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnadrSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandAdressFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnfstSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandFlurstueckfilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnDokusuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandDokuFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnProjektsuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandProjektFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub
    Private Sub btnWiedervorlage_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandWvFilter(_nurZumKuckenModus)
        e.Handled = True
    End Sub

    Private Sub btnClearNatureg_Click_1(sender As Object, e As RoutedEventArgs)
        dgNatureg.DataContext = ""
        e.Handled = True
    End Sub

    Private Sub dgNatureg_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If dgNatureg.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgNatureg.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try

        Try
            item = CType(dgNatureg.SelectedItem, DataRowView)

            If item Is Nothing Then
                item = CType(dgNatureg.SelectedItem, DataRowView)
                If item Is Nothing Then Return
            End If
            If _nurZumKuckenModus Then
                myGlobalz.sitzung.BestandsAuswahlVID = CInt(clsDBtools.fieldvalue(item("vorgangsid")))
                e.Handled = True
                Close()
                Exit Sub
            End If


            glob3.allAktobjReset.execute(myGlobalz.sitzung)

            Dim auswahlid$ = item("vorgangsid").ToString()
            Dim beschreibung$ = item("BESCHREIBUNG").ToString()
            Dim az2$ = item("AZ2").ToString()
            ' HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid$, beschreibung$, az2$)
            CLstart. HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2,myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            '  dgBeteiligte.SelectedItem = Nothing
            'Me.Close()
                  LocalParameterFiles.erzeugeParameterDatei( False, False)
            e.Handled = True
            glob2.editVorgang(CInt(auswahlid), myglobalz.testmode)

        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Private Sub btnSuchen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        dgNatureg.DataContext = Nothing 'tabelle_leer_machen

        myGlobalz.sitzung.VorgangREC.mydb.SQL = "select n.*,v.az2 as AZ2,v.sachgebietnr as SACHGEBIETNR,v.paragraf as PARAGRAF " &
            " from natureg n, " &
            " (" & CLstart.myViewsNTabs.view_vsk_d & ") v " &
            " where v.vorgangsid=n.vorgangsid order by vorgangsid desc"
        Dim hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
        dgNatureg.DataContext = myGlobalz.sitzung.VorgangREC.dt
        tbpersonenTreffer.Text = myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString
    End Sub


    Private Sub NeuerVorgang_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        clsStartup.NeuerVorgang2("normal")
        e.Handled = True
    End Sub

    Private Sub ZuvorgangsNr_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim az As String = "", header As String = ""
        clsStartup.suchenNachVorgaengen(az, header)
        Close()
        e.Handled = True
    End Sub

    Private Sub NaturegFilter_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        
    End Sub

    Private Sub NaturegFilter_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        myGlobalz.BestandsFensterIstgeoeffnet = True
        bestandTools.verschiedenes.beteiligteFilternAktivieren(btnbeteiligteFiltern)
        initStartPositionOnScreen()
        savePosition()
    End Sub
End Class
