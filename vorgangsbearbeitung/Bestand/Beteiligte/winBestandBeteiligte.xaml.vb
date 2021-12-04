﻿Imports System.Data

Public Class winBestandBeteiligte
    Private _makeSQL As Boolean
    Private _nurZumKuckenModus As Boolean
    Public Property auswahlid As String

    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        ' TODO: Complete member initialization 
        _nurZumKuckenModus = nurZumKuckenModus
    End Sub

    Private Sub winBestandBeteiligte_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        myGlobalz.BestandsFensterIstgeoeffnet = True
        'gastLayout()
        System.Threading.Thread.Sleep(2000)
        initDGMaxHeight()
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisArt")
        initGemeindeCombo()
        glob2.initGemKRZCombo(Me)
        tbpersonenTreffer.DataContext = Me
        initStartPositionOnScreen
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
    Private Sub btnStammdatenFiltern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        clsStartup.FormularBestandStammdaten(False)
        e.Handled = True
    End Sub

    Private Sub abbruchclick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnClearBeteiligte_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        tbStadt.Text = ""
        tbName.Text = ""
        tbVorname.Text = ""
        tbStrasse.Text = ""
        dgBeteiligte.DataContext = ""
        e.Handled = True
    End Sub
    Private Sub btnSuchen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        dgBeteiligte.DataContext = Nothing 'tabelle_leer_machen
        suchenPersonenInVorgaengen(tbName.Text, tbVorname.Text, tbStadt.Text, tbStrasse.Text)
        e.Handled = True
    End Sub
    Private Shared Function suchenPersonenInVorgaengen_Eingabe_pruefungOK(ByVal suchname$, ByVal suchvorname As String) As Boolean
        Return True
    End Function

    Private Sub suchenPersonenInVorgaengen(ByVal suchname$, ByVal suchvorname$, ByVal stadt$, ByVal strasse$)
        If Not suchenPersonenInVorgaengen_Eingabe_pruefungOK(suchname, suchvorname) Then Exit Sub
        Dim bsql As New SQL_Beteiligte(myGlobalz.beteiligte_MYDB.dbtyp) _
                                         With {.name = suchname,
                                               .vorname = suchvorname,
                                               .stadt = stadt,
                                               .strasse = strasse}
        'If clsParadigmaRechte.istVollzugriff() Then
        bsql.compoze()
            'Else
            '    MsgBox("Die Suche wird aus datenschutzrechtlichen Gründen auf die eigenen Fälle beschränkt! (" &
            '           myGlobalz.sitzung.aktBearbeiter.Initiale & ")" & Environment.NewLine &
            '           "Falls dies nicht ausreicht wenden Sie sich bitte an das Vorzimmer oder an die FD-Leitung!", , "Datenschmutz")
            '    bsql.compoze(myGlobalz.sitzung.aktBearbeiter.Initiale)
            'End If
            bsql.result = bsql.result.ToUpper
        myglobalz.sitzung.VorgangREC.mydb.SQL = bsql.result
        bsql = Nothing
        nachricht(myGlobalz.sitzung.VorgangREC.mydb.SQL)
        bestandTools.zeigeVorgaenge.exe()
        If myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
            dgBeteiligte.DataContext = Nothing
        Else
            Dim adrtemp As New DataTable
            adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
            dgBeteiligte.DataContext = adrtemp
        End If
        tbpersonenTreffer.Text = myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString
    End Sub

    Private Sub dgBeteiligte_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If dgBeteiligte.SelectedItem Is Nothing Then Exit Sub
        Dim item As DataRowView
        Try
            item = CType(dgBeteiligte.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        If _nurZumKuckenModus Then
            myGlobalz.sitzung.BestandsAuswahlVID = CInt(clsDBtools.fieldvalue(item("vorgangsid")))
            e.Handled = True
            Close()
            Exit Sub
        End If


        Try
            item = CType(dgBeteiligte.SelectedItem, DataRowView)

            If item Is Nothing Then
                item = CType(dgBeteiligte.SelectedItem, DataRowView)
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
            'HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid$, beschreibung$, az2$)
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2, myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)


            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            '  dgBeteiligte.SelectedItem = Nothing
            'Me.Close()
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            e.Handled = True
            glob2.editVorgang(CInt(auswahlid), myglobalz.testmode)
        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
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

    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        myGlobalz.BestandsFensterIstgeoeffnet = False
        detailsTools.VorgangLocking("aus")
        savePosition()
    End Sub

    'Private Sub gastLayout()
    '    If glob2.userIstinGastModus Then
    '        Background = New SolidColorBrush(Colors.Red)
    '        stckp1.Background = New SolidColorBrush(Colors.Red)
    '    End If
    'End Sub

    Private Sub initDGMaxHeight()
        dgBeteiligte.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemeinden.xml")
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

    Private Sub btnBestandtoExcel_Click(sender As Object, e As RoutedEventArgs)
        'bestandTools.btnBestandtoExcel_ClickExtracted.exe()
        'detailsTools.BeteiligteExcel_ClickExtracted()

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
