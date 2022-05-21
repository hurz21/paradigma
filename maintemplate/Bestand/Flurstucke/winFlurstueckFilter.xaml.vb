Imports System.Data

Public Class winFlurstueckFilter
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
    Private Sub btnClearFlurstueck_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        cmbgemarkung.SelectedValue = ""
        cmbFlur.SelectedValue = Nothing
        cmbZaehler.SelectedValue = Nothing
        cmbNenner.SelectedValue = Nothing
        dgFlur.DataContext = Nothing
    End Sub
    Private Sub cmbgemarkung_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbgemarkung.SelectedItem Is Nothing Then Exit Sub
        Auswahlzuruecksetzen()
        SetzeGemcodeUndGemtextFromCombo()
        initFlureCombo()
        cmbFlur.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Private Sub SetzeGemcodeUndGemtextFromCombo()
        Dim myvali$, myvals$
        myvali = CStr(cmbgemarkung.SelectedValue)
        Dim myvalx = CType(cmbgemarkung.SelectedItem, System.Xml.XmlElement)
        myvals = myvalx.Attributes(1).Value.ToString
        myGlobalz.sitzung.aktFST.normflst.gemcode = CInt(myvali$)
        myGlobalz.sitzung.aktFST.normflst.gemarkungstext = myvals$
    End Sub
    Sub initFlureCombo()
        'gemeindeDT
        myGlobalz.sitzung.tempREC.dt = VSTTools.holeFlureInVorgaengenDT.exe()
        cmbFlur.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub



    Private Sub cmbFlur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbFlur.SelectedItem Is Nothing Then Exit Sub
        SetzeGemcodeUndGemtextFromCombo()
        setzeFlurnummerFromCombo()
        initZaehlerCombo()
        cmbZaehler.IsDropDownOpen = True

        FST_tools.AlleSekidsZuDiesermFlurstueckholen("flur")
        Dim adrtemp As New DataTable
        adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
        dgFlur.DataContext = adrtemp

        e.Handled = True
    End Sub
    Private Sub Auswahlzuruecksetzen()
        myGlobalz.sitzung.aktFST.normflst.clear()
        cmbFlur.DataContext = Nothing
        cmbZaehler.DataContext = Nothing
        cmbNenner.DataContext = Nothing
    End Sub

    Private Sub setzeFlurnummerFromCombo()
        Dim item2 As DataRowView = CType(cmbFlur.SelectedItem, DataRowView)
        Dim item3$ = item2.Row.ItemArray(0).ToString
        myGlobalz.sitzung.aktFST.normflst.flur = CInt(item3$)
    End Sub

    Sub initZaehlerCombo()
        DB_Oracle_sharedfunctions.holeZaehlerDTinvorgaengen()
        cmbZaehler.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub

    Private Sub cmbZaehler_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbZaehler.SelectedItem Is Nothing Then Exit Sub
        SetzeGemcodeUndGemtextFromCombo()
        setzeFlurnummerFromCombo()
        setzeZaehlerFromCombo()
        initNennerCombo()
        cmbNenner.IsDropDownOpen = True

        FST_tools.AlleSekidsZuDiesermFlurstueckholen("zaehler")
        Dim adrtemp As New DataTable
        adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
        dgFlur.DataContext = adrtemp

        e.Handled = True
    End Sub
    Private Sub setzeZaehlerFromCombo()
        Dim item2 As DataRowView = CType(cmbZaehler.SelectedItem, DataRowView)
        Dim item3$ = item2.Row.ItemArray(0).ToString
        myGlobalz.sitzung.aktFST.normflst.zaehler = CInt(item3$)
    End Sub
    Sub initNennerCombo()
        DB_Oracle_sharedfunctions.holeNennerDTinVorgaengen()
        cmbNenner.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub

    Private Sub cmbNenner_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbNenner.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As DataRowView = CType(cmbNenner.SelectedItem, DataRowView)
        Try
            'Dim item3$ = item2.Row.ItemArray(0).ToString
        Catch ex As Exception
            Exit Sub
        End Try
        'Dim fst_id% = 0
        dgFlur.DataContext = Nothing
        myGlobalz.sitzung.aktFST.normflst.nenner = CInt(item2.Row.ItemArray(0).ToString)
        'myGlobalz.sitzung.aktFST.normflst.FS = myGlobalz.sitzung.aktFST.normflst.buildFS()
        'myGlobalz.sitzung.aktFST.SekID = CInt(item2.Row.ItemArray(1).ToString)
        '   VorgaengeAnzeigenFuerFlurstueck(myGlobalz.sitzung.aktFST.SekID)
        VorgaengeAnzeigenFuerFlurstueckNeu(myGlobalz.sitzung.aktFST)
        e.Handled = True
    End Sub

    Sub VorgaengeAnzeigenFuerFlurstueck(ByVal sekid As Long)
        myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Flurstueck
        myGlobalz.sitzung.aktFST.RaumbezugsID = RBid_zuSekid_holen_alledb(sekid, myGlobalz.sitzung.akt_raumbezugsTyp)
        If myGlobalz.sitzung.aktFST.RaumbezugsID > 0 Then
            'holevorgaengezuRaumbezug(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
            initVorgangsDT(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
            dgFlur.DataContext = myGlobalz.sitzung.VorgangREC.dt
        Else
            nachricht_und_Mbox(String.Format("Es konnte kein gültiger Raumbezug gefunden werden! ({0})", sekid))
            Exit Sub
        End If
    End Sub
    Public Shared Function RBid_zuSekid_holen_alledb(ByVal sekid As Long, ByVal rbtyp As Integer) As Integer
        Dim resultDT As New DataTable
        Try
            'myGlobalz.sitzung.tempREC.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
            myGlobalz.sitzung.tempREC.mydb.SQL =
             String.Format("select raumbezugsid from " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  where sekid={0} and typ={1}", sekid, rbtyp%)
            VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultDT)
            myGlobalz.sitzung.tempREC.dt = resultDT.Copy
            ' hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0))
            Else
                Return CInt(0)
            End If

        Catch ex As Exception
            nachricht_und_Mbox("Problem beim RBid_zuSekid_holen: " & ex.ToString)
            Return -1
        End Try
    End Function
    Public Shared Function initVorgangsDT(ByVal RaumbezugsID As Integer) As Boolean
        If viaKopplung_RaumbezugID_zu_VorgangID_alledb(RaumbezugsID) Then
            myGlobalz.sitzung.VorgangREC.dt = myGlobalz.sitzung.tempREC.dt.Copy                         'alle vorgangsnummern
            ' Dim SQL$ = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = LIBstammdatenCRUD.clsStammdatenTool.UNION_SQL_VST_erzeugen(myGlobalz.sitzung.VorgangREC.dt, 2, "s.vorgangsid")
            'clsVorgangDB_Oracle.initvorgangsDT_by_SQLstring(SQL$)
            Dim resultdt As New System.Data.DataTable
            Dim anzahl As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
            myGlobalz.sitzung.VorgangREC.dt = resultdt.Copy
            nachricht("Es konnten  Vorgänge zu diesem Raumbezug gefunden werden!")
            Return True
        Else
            nachricht("Es konnten keine  Vorgänge zu diesem Raumbezug gefunden werden!")
            Return False
        End If
    End Function
    Public Shared Function viaKopplung_RaumbezugID_zu_VorgangID_alledb(ByVal RaumbezugsID As Integer) As Boolean
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.raumbezugsRec.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.raumbezugsRec.mydb.Schema
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="Raumbezug2vorgang"
        myGlobalz.sitzung.tempREC.mydb.SQL =
         String.Format("SELECT * FROM " & CLstart.myViewsNTabs.tabRaumbezug2vorgang & "    where RaumbezugsID={0}", RaumbezugsID)
        '
        Dim resultdt As New System.Data.DataTable
        VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy
        'nachricht("hinweis = " & myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            nachricht("Keine Ereignisse gespeichert g!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function
    Private Sub dgFlur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            Dim item As DataRowView
            Try
                item = CType(dgFlur.SelectedItem, DataRowView)
            Catch ex As Exception
                e.Handled = True
                Exit Sub
            End Try
            item = CType(dgFlur.SelectedItem, DataRowView)
            If item Is Nothing Then
                item = CType(dgFlur.SelectedItem, DataRowView)
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
            '    HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid$, beschreibung$, az2$)
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2, myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            e.Handled = True
            glob2.editVorgang(CInt(auswahlid), True)

        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
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

    Private Sub VorgaengeAnzeigenFuerFlurstueckNeu(ByVal paraFlurstueck As ParaFlurstueck)
        FST_tools.AlleSekidsZuDiesermFlurstueckholen("flst")
        Dim adrtemp As New DataTable
        adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
        dgFlur.DataContext = adrtemp
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
        dgFlur.MaxHeight = bestandTools.verschiedenes.GetMaxheight()
    End Sub

    Sub initGemarkungsCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemarkungen"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemarkungen.xml")
    End Sub

    Private Sub winFlurstueckFilter_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        myGlobalz.BestandsFensterIstgeoeffnet = True
        'gastLayout()
        initDGMaxHeight()
        initGemarkungsCombo()
        bestandTools.verschiedenes.beteiligteFilternAktivieren(btnBeteiligteFiltern)
        initStartPositionOnScreen()
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
End Class
