Imports System.Data

Public Class IllegbauFilter
    Private _makeSQL As Boolean
    Property tablename As String
    Property _nurZumKuckenModus As Boolean
    Public Property auswahlid As String

    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
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
    Private Sub IllegbauFilter_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        myGlobalz.BestandsFensterIstgeoeffnet = True
        tablename = CLstart.myViewsNTabs.view_illegale2 ' "illegale2"
        initDGMaxHeight() : nachricht("winBestandStammdaten_Loaded 3")
        'bestandTools.verschiedenes.beteiligteFilternAktivieren(btnbeteiligteFiltern)
        sucheIllegale()
        initStartPositionOnScreen()
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
        dgIllegbau.DataContext = Nothing
        e.Handled = True
    End Sub

    Private Sub initDGMaxHeight()
        'dgStamm.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then
            dgIllegbau.MaxHeight = 780
        End If
        If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then
            dgIllegbau.MaxHeight = 550
        End If
        'dgStamm.Height = Height - 65 - 35 - 130 - 50
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

    Private Sub IllegbauFilter_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        savePosition()
    End Sub



    Private Sub dgIllegbau_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If dgIllegbau.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgIllegbau.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try

        Try
            item = CType(dgIllegbau.SelectedItem, DataRowView)
            If item Is Nothing Then
                item = CType(dgIllegbau.SelectedItem, DataRowView)
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
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2, myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            e.Handled = True
            glob2.editVorgang(CInt(auswahlid), myglobalz.testmode)

        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Private Sub btnSuchen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim erfolg As Boolean = sucheIllegale()
        dgIllegbau.Height = Me.ActualHeight - dpMain.ActualHeight - spButtonlist.ActualHeight - gbFilter.ActualHeight - 150 ' 65 - 35 - 130 - 50
        dgIllegbau.MaxHeight = dgIllegbau.Height
        'dgIllegbau.DataContext = Nothing 'tabelle_leer_machen
        '  suchenPersonenInVorgaengen(tbName.Text, tbVorname.Text, tbStadt.Text, tbStrasse.Text)

    End Sub

    Private Function sucheIllegale() As Boolean
        If radNurMitFlurstuecken.IsChecked Then
            tablename = CLstart.myViewsNTabs.view_illegale2 '"illegale2"
        Else
            tablename = CLstart.myViewsNTabs.view_illegalohnerb3 ' "illegaleohnerb3"
        End If
        Dim suchtext As String = ""
        If tbSuchtext.Text.Trim = String.Empty Then
            suchtext = ""
        Else
            suchtext = tbSuchtext.Text.Trim
        End If
        Dim bsql As New SQL_illegbau(tablename)
        'If clsParadigmaRechte.istVollzugriff() Then
        bsql.compoze(suchtext, CBool(chkplanmaessig.IsChecked), CBool(chklaufend.IsChecked),
                     CBool(chkerledigt.IsChecked), CBool(chkrecherche.IsChecked))

        'statusPlanmaessig As Boolean,
        '               statuslaufend As Boolean,
        '               statuserledigt As Boolean,
        '               statusrecherche As Boolean
        'Else
        '    MsgBox("Die Suche wird aus datenschutzrechtlichen Gründen auf die eigenen Fälle beschränkt! (" &
        '           myGlobalz.sitzung.aktBearbeiter.Initiale & ")" & Environment.NewLine &
        '           "Falls dies nicht ausreicht wenden Sie sich bitte an das Vorzimmer oder an die FD-Leitung!", , "Datenschmutz")
        '    bsql.compoze(suchtext)
        'End If
        myGlobalz.sitzung.VorgangREC.mydb.SQL = bsql.result
        bsql = Nothing
        nachricht(myGlobalz.sitzung.VorgangREC.mydb.SQL)
        bestandTools.zeigeVorgaenge.exe()
        If myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
            dgIllegbau.DataContext = Nothing
        Else
            dgIllegbau.DataContext = myGlobalz.sitzung.VorgangREC.dt
        End If
        tbpersonenTreffer.Text = myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString
        Return True
    End Function



    Private Sub ListeInsGIS_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub btnBestandtoExcel_Click(sender As Object, e As RoutedEventArgs)
        Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("beliebig",
                                          myGlobalz.sitzung.VorgangREC.dt,
                                          myGlobalz.sitzung.aktVorgangsID,
                                          "",
                                          CLstart.mycSimple.Paradigma_local_root,
                                          CLstart.mycSimple.enc)
        nachricht(" exportfile$ = " & handcsv.CscDateiAusgeben())
        handcsv.start()
        handcsv.Dispose()
        e.Handled = True
    End Sub
End Class
