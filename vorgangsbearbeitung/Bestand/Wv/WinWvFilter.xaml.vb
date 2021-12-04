Imports System.ComponentModel
Imports System.Data

Public Class WinWvFilter
    Property _nurZumKuckenModus As Boolean
    Sub New(nurZumKuckenModus As Boolean)
        InitializeComponent()
        _nurZumKuckenModus = nurZumKuckenModus
    End Sub
    Private Sub WinWvFilter_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.DBWiedervorlageREC.dt, "faelligSymbol", "System.Int16")
        einfaerbenDerRowsvorbereiten()
        clsDBtools.TabellenKopfausgeben(myGlobalz.sitzung.DBWiedervorlageREC.dt)
        DataContext = myGlobalz.sitzung.DBWiedervorlageREC.dt
        dgWVliste.CanUserAddRows = False
        initDGMaxHeight()
        nichtNervenchekboxInit()
        initStartPositionOnScreen()
    End Sub

    Private Sub savePosition(nameDerForm As String) 'winbestandform
        If myGlobalz.nurEinBildschirm Then Exit Sub
        Try
            CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positiontop", CType(Me.Top, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positionleft", CType(Me.Left, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positionwidth", CType(Me.ActualWidth, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positionheight", CType(Me.ActualHeight, String))
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
    Shared Sub einfaerbenDerRowsvorbereiten()
        For i = 0 To myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows.Count - 1
            Dim lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem As Object = myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("datum")
            Dim lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem1 As Date = CType(lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem, Date)
            If lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem1 < Now Then
                If CType(clsDBtools.fieldvalueDate(myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("erledigtAm")), Date) > CLstart.mycSimple.MeinNULLDatumAlsDate Then
                    myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 2
                Else
                    myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 1
                End If

            Else
                myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 0
            End If
        Next
    End Sub



    Private Sub dgWVliste_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgWVliste.SelectionChanged
        Try
            e.Handled = True
            Dim item As DataRowView = CType(dgWVliste.SelectedItem, DataRowView)
            'Dim item = dg.SelectedItem
            If item Is Nothing Then Return
            Dim auswahlid$ = item(0).ToString()
            ' nachricht_und_Mbox(auswahlid$)
            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            'Me.Close()

            Me.Close()
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            glob2.editVorgang(myGlobalz.sitzung.aktVorgangsID, myGlobalz.testmode)
            dgWVliste.SelectedItem = Nothing
            e.Handled = True
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



    Private Sub WVExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Wiedervorlagen", myGlobalz.sitzung.DBWiedervorlageREC.dt, 0, "", CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        nachricht(" exportfile$ = " & handcsv.CscDateiAusgeben())
        handcsv.start()
        handcsv.Dispose()
        e.Handled = True
    End Sub


    Private Sub initDGMaxHeight()
        dgWVliste.MaxHeight = bestandTools.verschiedenes.GetMaxheight()




        'WindowState = Windows.WindowState.Maximized

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

    Private Sub ckbNichtNerven_Click(sender As Object, e As RoutedEventArgs)

    End Sub

    Private Sub nichtNervenchekboxInit()
        ckbNichtNerven.IsChecked = CBool(CLstart.myc.userIniProfile.WertLesen("Boot", "wiedervorlagenpoppen"))
    End Sub

    Private Sub ckbNichtNerven_Checked(sender As Object, e As RoutedEventArgs) Handles ckbNichtNerven.Checked
        'If ckbNichtNerven.IsChecked Then
        '      CLstart.myc.userIniProfile.WertSchreiben("Boot", "wiedervorlagenpoppen", If(ckbNichtNerven.IsChecked, "True", "False"))   
        '    Else
        '      CLstart.myc.userIniProfile.WertSchreiben("Boot", "wiedervorlagenpoppen", If(ckbNichtNerven.IsChecked, "True", "False"))              
        'End If
        e.Handled = True
    End Sub

    Private Sub ckbNichtNerven_Click1(sender As Object, e As RoutedEventArgs) Handles ckbNichtNerven.Click
        If ckbNichtNerven.IsChecked Then
            CLstart.myc.userIniProfile.WertSchreiben("Boot", "wiedervorlagenpoppen", If(ckbNichtNerven.IsChecked, "True", "False"))
        Else
            CLstart.myc.userIniProfile.WertSchreiben("Boot", "wiedervorlagenpoppen", If(ckbNichtNerven.IsChecked, "True", "False"))
        End If
        e.Handled = True
    End Sub

    Private Sub WinWvFilter_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        savePosition("winbestandform")
    End Sub
End Class
