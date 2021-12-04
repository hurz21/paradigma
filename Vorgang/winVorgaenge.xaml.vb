Imports System.ComponentModel
Imports System.Data
Imports System.Windows.Forms
Partial Public Class Window_Zuletzt
    Implements INotifyPropertyChanged
    Private text_alle As String = "ALLE-"
    Public odlsel$
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged

    Private Property geladen As Boolean

    Protected Sub OnPropertyChanget(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
    Public anychange As Boolean
    Private LIMIT_NR$ = "50"

    Private mittextfilter As Boolean
    Private fuerBearbeiterName$ = "alle", fuerAlleBearbeiter As Boolean = True, fuerBearbeiterKuerzel$ = ""
    Private erledigteauswahl$ = "beides"
    Private _makeSQL As Boolean
    Private _treffer As Integer
    Private aktSachgebietnr$ = text_alle, aktGMZ$ = text_alle
    Public Property Treffer() As Integer
        Get
            Return _treffer
        End Get
        Set(ByVal Value As Integer)
            _treffer = Value
            OnPropertyChanget("Treffer")
        End Set
    End Property

    Private Sub Window_Zuletzt_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        detailsTools.locking("aus")
    End Sub

    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        initLimitCombo()
        initErledigtCombo()
        comboBearbeiterInit()
        initGemarkungsCombo()

        initGemeindeCombo()
        glob2.initGemKRZCombo(Me)

        refresh_WINvorgaengeListe(_makeSQL)
        initSachgebietnrCombo() 'ueberflüssig
        '  init_WINVORGAENGECombos()

        tbTreffer.DataContext = Me
        aktSachgebietnr = text_alle
        aktGMZ = text_alle
        geladen = True

        AddHandler cmbSachgebietnr.SelectionChanged, AddressOf cmbSachgebietnr_SelectionChanged
        AddHandler cmbLimit.SelectionChanged, AddressOf cmbLimit_SelectionChanged_1
        AddHandler cmbErledigt.SelectionChanged, AddressOf cmbErledigt_SelectionChanged_2
        AddHandler tbsachgebietnr.TextChanged, AddressOf tbsachgebietnr_TextChanged
        AddHandler cmbUserInitial.SelectionChanged, AddressOf cmbUserInitial_SelectionChanged_1

        e.Handled = True
    End Sub


    Private Sub initLimitCombo()
        Dim limit As New Dictionary(Of String, String)
        limit.Add("50", "50 Zeilen")
        limit.Add("100", "100 Zeilen")
        limit.Add("500", "500 Zeilen")
        cmbLimit.ItemsSource = limit
        cmbLimit.SelectedIndex = If(clsParadigmaRechte.istUser_admin_oder_vorzimmer(), 1, 0)
        LIMIT_NR = If(clsParadigmaRechte.istUser_admin_oder_vorzimmer(), "100", "50")
    End Sub

    Private Sub initErledigtCombo()
        cmbErledigt.Items.Add("unerledigte")
        cmbErledigt.Items.Add("erledigte")
        cmbErledigt.Items.Add("beides")
        cmbErledigt.SelectedValue = If(clsParadigmaRechte.istUser_admin_oder_vorzimmer(), "beides", "beides")
    End Sub


    Sub initGemarkungsCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemarkungen"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemarkungen.xml")
    End Sub

    Sub initSachgebietnrCombo()
        cmbSachgebietnr.Items.Clear()
        Dim results = From datar As DataRow In myGlobalz.sitzung.VorgangREC.dt.AsEnumerable
                      Order By datar.Field(Of String)("Sachgebietnr")
                      Select datar.Field(Of String)("Sachgebietnr") Distinct
        cmbSachgebietnr.Items.Add(text_alle)
        For Each strra As String In results
            cmbSachgebietnr.Items.Add(strra.ToString)
        Next
        cmbSachgebietnr.SelectedValue = aktSachgebietnr
    End Sub

    Sub initGemeindeCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemeinden"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemeinden.xml")
    End Sub


    Sub New(ByVal makeSQL As Boolean)
        InitializeComponent()
        'Me.Title = Titel
        'If Titel <> "Aktuelle Vorgänge im FD Umwelt" Then
        '	'mnuMenu.Visibility = Windows.Visibility.Collapsed
        'Else
        '	'comboBearbeiterInit()
        'End If

        _makeSQL = makeSQL
    End Sub

    Private Sub dg_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dg.SelectionChanged, dg2.SelectionChanged
        Try
            Dim item As DataRowView = CType(dg.SelectedItem, DataRowView)
            'Dim item = dg.SelectedItem
            If item Is Nothing Then
                item = CType(dg2.SelectedItem, DataRowView)
                If item Is Nothing Then Return
            End If

            glob3.allAktobjReset.execute(myGlobalz.sitzung)

            Dim auswahlid$ = item("vorgangsid").ToString()
            Dim beschreibung$ = item("BESCHREIBUNG").ToString()
            Dim az2$ = item("AZ2").ToString()
            HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid$, beschreibung$, az2$)

            myGlobalz.sitzung.VorgangsID = CInt(auswahlid)
            dg.SelectedItem = Nothing
            'Me.Close()
            e.Handled = True
            glob2.editVorgang(CInt(auswahlid))
        Catch ex As Exception
            glob2.nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Function comboBearbeiterInit() As Boolean
        Try
            Dim sql$ = "select LOWER(INITIAL_) as ""INITIALE"",NAME from " & "Bearbeiter" & " order by NAME asc"
            myGlobalz.sitzung.BearbeiterREC.dt = DB_Oracle.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql$).Copy
            myGlobalz.sitzung.BearbeiterREC.dt.Rows.Add("alle", "alle")
            cmbUserInitial.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
            'For Each ddd As DataRow In myGlobalz.sitzung.BearbeiterREC.dt.AsEnumerable
            '    Console.WriteLine(ddd.Item("Initiale").ToString & " " & ddd.Item("Name").ToString)
            'Next
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                cmbUserInitial.SelectedValue = "alle"
            Else
                cmbUserInitial.SelectedValue = myGlobalz.sitzung.Bearbeiter.Initiale.ToLower
            End If

        Catch ex As Exception
            glob2.nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Function

    Private Shared Sub zeigeVorgaenge()
        Dim result As Long = initVorgaengeDatatable("")
        If result < 1 Then
            If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
                glob2.nachricht("Es wurden keine Vorgänge in der DB gefunden")
            End If
            Exit Sub
        End If
    End Sub

    Public Shared Function initVorgaengeDatatable(ByVal hinweis$) As Long
        Try
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            Dim retval As Long = myGlobalz.sitzung.VorgangREC.mycount
            glob2.nachricht("initVorgaengeDatatable Treffer: " & retval)
            Return retval
        Catch ex As Exception
            glob2.nachricht_und_Mbox("initVorgaengeDatatable: " & ex.ToString)
            Return -1
        End Try
    End Function

    Private Sub cmbUserChange()
        Dim item As String = CType(cmbUserInitial.SelectedValue, String)
        If item Is Nothing Then
            fuerBearbeiterName$ = ""
            fuerAlleBearbeiter = True
        Else
            fuerBearbeiterName$ = item
            If fuerBearbeiterName.ToLower = "alle" Then
                fuerAlleBearbeiter = True
            Else
                fuerAlleBearbeiter = False
            End If
        End If
    End Sub

    Private Sub suchentaste()
        If Not geladen Then Exit Sub
        dg.DataContext = Nothing 'tabelle_leer_machen
        mittextfilter = If(String.IsNullOrEmpty(tbFilter.Text), False, True)
        refresh_WINvorgaengeListe(True)
    End Sub

    Function refresh_WINvorgaengeListe(ByVal makesqlstring As Boolean) As Boolean
        Try
            fuerBearbeiterKuerzel = getKuerzelForInitial(fuerBearbeiterName)
            If makesqlstring Then
                Dim sql As New SQL_Stamm(myGlobalz.vorgang_MYDB.dbtyp) With {.GEMKRZ_selitem = cmbGemKRZ.SelectedItem,
                                                 .GEMKRZ_selval = cmbGemKRZ.SelectedValue,
                                                 .text_alle = text_alle,
                                                 .tbsachgebietnr_Text = tbsachgebietnr.Text,
                                                 .LIMIT_NR = LIMIT_NR,
                                                 .fuerAlleBearbeiter = fuerAlleBearbeiter,
                                                 .fuerBearbeiterInitiale = fuerBearbeiterName,
                                                 .fuerBearbeiterKuerzel = fuerBearbeiterKuerzel,
                                                 .erledigteauswahl = erledigteauswahl,
                                                 .mittextfilter = mittextfilter,
                                                 .tbFilter_Text = tbFilter.Text}
                sql.compoze()
                myGlobalz.sitzung.VorgangREC.mydb.SQL = sql.result
                sql = Nothing
            Else
                glob2.nachricht("" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
            End If
            glob2.nachricht(" " & myGlobalz.sitzung.VorgangREC.mydb.SQL)
            zeigeVorgaenge()
            dg.DataContext = myGlobalz.sitzung.VorgangREC.dt
            dg2.DataContext = myGlobalz.sitzung.VorgangREC.dt
            Treffer = myGlobalz.sitzung.VorgangREC.dt.Rows.Count
            dg.CanUserAddRows = False       'verhindert eine reihe von Fehlermwldungen  
            'initSachgebietnrCombo()
            ListeInsGIS.Visibility = Windows.Visibility.Visible
        Catch ex As Exception
            glob2.nachricht_und_Mbox("Fehler in refresh: " & ex.ToString)
        End Try
    End Function

    Private Sub cmbErledigt_SelectionChangedExtracted()
        Try
            If cmbErledigt.SelectedItem Is Nothing Then Exit Sub
            If cmbErledigt.SelectedItem.ToString = "erledigte" Then erledigteauswahl = "erledigte"
            If cmbErledigt.SelectedItem.ToString = "unerledigte" Then erledigteauswahl = "unerledigte"
            If cmbErledigt.SelectedItem.ToString = "beides" Then erledigteauswahl = "beides"

            'refresh(True)
        Catch ex As Exception
            glob2.nachricht_und_Mbox("Fehler in cmbErledigt_SelectionChanged: " & ex.ToString)
        End Try
    End Sub

    'Private Sub cmbErledigt_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    cmbErledigt_SelectionChangedExtracted()
    'End Sub


    'Private Sub Such_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    suchentaste()
    'End Sub

    Private Sub SetLIMIT_NR()
        Dim selob As New KeyValuePair(Of String, String)
        selob = CType(cmbLimit.SelectedItem, KeyValuePair(Of String, String))
        'Dim selKey$ = selob.Key
        'Dim selvalue$ = selob.Value
        LIMIT_NR$ = selob.Key
    End Sub
    'Private Sub cmbLimit_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    SetLIMIT_NR()
    'End Sub

    'Private Sub cmbErledigt_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
    '    cmbErledigt_SelectionChangedExtracted()
    'End Sub

    Private Sub btnSuchen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        dg.DataContext = Nothing 'tabelle_leer_machen
        suchenPersonenInVorgaengen(tbName.Text, tbVorname.Text, tbStadt.Text, tbStrasse.Text)

        tbAnsicht2.IsSelected = True
    End Sub

    Private Shared Function suchenPersonenInVorgaengen_Eingabe_pruefungOK(ByVal suchname$, ByVal suchvorname$) As Boolean
        'If String.IsNullOrEmpty(suchname) And 
        '    String.IsNullOrEmpty(suchvorname) and
        '    And String.IsNullOrEmpty(suchstadt) and
        '    And String.IsNullOrEmpty(suchvorname)Then
        '    MessageBox.Show("Sie müssen Suchfilter angeben!", "Suchen nach Personen", MessageBoxButtons.OK) ', MessageBoxImage.Information)
        '    Return False
        'End If
        Return True
    End Function

    Private Sub suchenPersonenInVorgaengen(ByVal suchname$, ByVal suchvorname$, ByVal stadt$, ByVal strasse$)
        If Not suchenPersonenInVorgaengen_Eingabe_pruefungOK(suchname, suchvorname) Then Exit Sub
        Dim bsql As New SQL_Beteiligte(myGlobalz.beteiligte_MYDB.dbtyp) With {.name = suchname$,
                                               .vorname = suchvorname$,
                                               .stadt = stadt,
                                               .strasse = strasse}
        bsql.compoze()
        myGlobalz.sitzung.VorgangREC.mydb.SQL = bsql.result
        bsql = Nothing
        '	holeVorgaengeMitpersonen
        glob2.nachricht(myGlobalz.sitzung.VorgangREC.mydb.SQL)
        zeigeVorgaenge()
        If myGlobalz.sitzung.VorgangREC.dt.Rows.Count < 1 Then
            dg.DataContext = Nothing
            dg2.DataContext = Nothing
        Else
            dg.DataContext = myGlobalz.sitzung.VorgangREC.dt
            dg2.DataContext = myGlobalz.sitzung.VorgangREC.dt
        End If
        tbpersonenTreffer.Text = myGlobalz.sitzung.VorgangREC.dt.Rows.Count.ToString
    End Sub

    Private Sub abbruchclick(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
    End Sub

    Private Sub cmbUserInitial_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) ' Handles cmbUserInitial.SelectionChanged
        If cmbUserInitial.SelectedItem Is Nothing Then Exit Sub
        'If cmbUserInitial.SelectedValue.ToString.ToLower = fuerBearbeiterName$.ToLower And
        '    (Not fuerBearbeiterName$.ToLower.Contains("alle")) Then
        '    Exit Sub
        'End If
        'If cmbUserInitial.SelectedValue.ToString.ToLower = myGlobalz.sitzung.Bearbeiter.Initiale.ToLower Then
        '    Exit Sub
        'End If
        cmbUserChange()
        suchentaste()
        e.Handled = True
    End Sub

    Private Sub cmbErledigt_SelectionChanged_2(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) 'Handles cmbErledigt.SelectionChanged
        If cmbErledigt.SelectedItem Is Nothing Then Exit Sub
        cmbErledigt_SelectionChangedExtracted()
        suchentaste()
        e.Handled = True
    End Sub

    Private Sub cmbLimit_SelectionChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) 'Handles cmbLimit.SelectionChanged
        If cmbLimit.SelectedItem Is Nothing Then Exit Sub
        SetLIMIT_NR()
        suchentaste()
        e.Handled = True
    End Sub

    Private Sub Auswahlzuruecksetzen()
        myGlobalz.sitzung.aktFST.normflst.clear()
        cmbFlur.DataContext = Nothing
        cmbZaehler.DataContext = Nothing
        cmbNenner.DataContext = Nothing
        dg.DataContext = Nothing
        dg2.DataContext = Nothing
    End Sub
    Private Sub cmbgemarkung_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbgemarkung.SelectedItem Is Nothing Then Exit Sub

        'Dim selob As New KeyValuePair(Of String, String)
        'selob = CType(cmbgemarkung.SelectedItem, KeyValuePair(Of String, String))
        'Dim selKey$ = selob.Key
        'Dim selvalue$ = selob.Value

        Dim myvali$ = CStr(cmbgemarkung.SelectedValue)
        Dim myvalx = CType(cmbgemarkung.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        dg.DataContext = Nothing 'tabelle_leer_machen

        'Dim item2 As DataRowView = CType(cmbgemarkung.SelectedItem, DataRowView)
        'Dim item3$ = item2.Row.ItemArray(0).ToString
        Auswahlzuruecksetzen()
        myGlobalz.sitzung.aktFST.normflst.gemcode = CInt(myvali$)
        myGlobalz.sitzung.aktFST.normflst.gemarkungstext = myvals$
        initFlureCombo()
        cmbFlur.IsDropDownOpen = True
    End Sub

    Sub initFlureCombo()
        'gemeindeDT
        VSTTools.holeFlureInVorgaengenDT.exe()
        cmbFlur.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub



    Private Sub cmbFlur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbFlur.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As DataRowView = CType(cmbFlur.SelectedItem, DataRowView)
        Dim item3$ = item2.Row.ItemArray(0).ToString
        myGlobalz.sitzung.aktFST.normflst.flur = CInt(item3$)
        initZaehlerCombo()
        cmbZaehler.IsDropDownOpen = True
    End Sub
    Sub initZaehlerCombo()
        DB_Oracle.holeZaehlerDTinvorgaengen()
        cmbZaehler.DataContext = myGlobalz.sitzung.tempREC.dt
        '	For Each zz As DataRow In myGlobalz.sitzung.tempREC.dt.Rows
        '		MsgBox(zz.item("zaehler").tostring)
        'Next
    End Sub

    Private Sub cmbZaehler_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbZaehler.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As DataRowView = CType(cmbZaehler.SelectedItem, DataRowView)
        Dim item3$ = item2.Row.ItemArray(0).ToString
        'Me.tbZaehler.Text = item2.Row.ItemArray(0).ToString
        'Me.tbStrasse.Text=item4
        myGlobalz.sitzung.aktFST.normflst.zaehler = CInt(item3$)
        ' myGlobalz.sitzung.aktFST.normflst.gemarkungstext = Me.tbGemarkung.Text
        initNennerCombo()
        cmbNenner.IsDropDownOpen = True
    End Sub
    Sub initNennerCombo()
        'gemeindeDT
        DB_Oracle.holeNennerDTinVorgaengen()
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
        myGlobalz.sitzung.aktFST.normflst.nenner = CInt(item2.Row.ItemArray(0).ToString)
        myGlobalz.sitzung.aktFST.normflst.FS = myGlobalz.sitzung.aktFST.normflst.buildFS()
        myGlobalz.sitzung.aktFST.SekID = CInt(item2.Row.ItemArray(1).ToString)
        VorgaengeAnzeigenFuerFlurstueck(myGlobalz.sitzung.aktFST.SekID)
    End Sub

    Sub VorgaengeAnzeigenFuerFlurstueck(ByVal sekid As Long)
        myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Flurstueck
        myGlobalz.sitzung.aktFST.RaumbezugsID = RBid_zuSekid_holen_alledb(sekid, myGlobalz.sitzung.akt_raumbezugsTyp)
        If myGlobalz.sitzung.aktFST.RaumbezugsID > 0 Then
            'holevorgaengezuRaumbezug(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
            initVorgangsDT(CInt(myGlobalz.sitzung.aktFST.RaumbezugsID))
            dg.DataContext = myGlobalz.sitzung.VorgangREC.dt
            dg2.DataContext = myGlobalz.sitzung.VorgangREC.dt
        Else
            glob2.nachricht_und_Mbox(String.Format("Es konnte kein gültiger Raumbezug gefunden werden! ({0})", sekid))
            Exit Sub
        End If
    End Sub

    Public Shared Function RBid_zuSekid_holen_alledb(ByVal sekid As Long, ByVal rbtyp%) As Integer
        Dim resultDT As New DataTable
        Try

            myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("select raumbezugsid from {0} where sekid={1} and typ={2}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid, rbtyp%)
            VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.mydb.Tabelle, resultDT)
            myGlobalz.sitzung.tempREC.dt = resultDT.Copy
            ' hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            If myGlobalz.sitzung.tempREC.dt.Rows.Count > 0 Then
                Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0))
            Else
                Return CInt(0)
            End If

        Catch ex As Exception
            glob2.nachricht_und_Mbox("Problem beim RBid_zuSekid_holen: " & ex.ToString)
            Return -1
        End Try
    End Function

    Public Shared Function initVorgangsDT(ByVal RaumbezugsID%) As Boolean
        If viaKopplung_RaumbezugID_zu_VorgangID_alledb(RaumbezugsID) Then
            myGlobalz.sitzung.VorgangREC.dt = myGlobalz.sitzung.tempREC.dt.Copy                         'alle vorgangsnummern
            ' Dim SQL$ = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = DB_Oracle.UNION_SQL_VST_erzeugen(myGlobalz.sitzung.VorgangREC.dt, 2, "s.vorgangsid")
            'clsVorgangDB_Oracle.initvorgangsDT_by_SQLstring(SQL$)
            Dim resultdt As New System.Data.DataTable
            Dim anzahl As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, "vorgang", resultdt)
            myGlobalz.sitzung.VorgangREC.dt = resultdt.Copy
            glob2.nachricht("Es konnten  Vorgänge zu diesem Raumbezug gefunden werden!")
            Return True
        Else
            glob2.nachricht("Es konnten keine  Vorgänge zu diesem Raumbezug gefunden werden!")
            Return False
        End If
    End Function

    Public Shared Function viaKopplung_RaumbezugID_zu_VorgangID_alledb(ByVal RaumbezugsID%) As Boolean
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.raumbezugsRec.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.raumbezugsRec.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "Raumbezug2vorgang"         ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         String.Format("SELECT * FROM {0} where RaumbezugsID={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, RaumbezugsID%)
        '
        Dim resultdt As New System.Data.DataTable
        VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.mydb.Tabelle, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy
        'glob2.nachricht("hinweis = " & myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.dt.Rows.Count < 1 Then
            glob2.nachricht("Keine Ereignisse gespeichert g!")
            Return False
        Else
            glob2.nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

    Private Sub cmbGemeinde_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbGemeinde.SelectedItem Is Nothing Then Exit Sub
        gemeindechanged()
        dg.DataContext = Nothing 'tabelle_leer_machen
        cmbStrasse.IsDropDownOpen = True
    End Sub

    Sub gemeindechanged()
        Dim myvali$ = CStr(cmbGemeinde.SelectedValue)
        Dim myvalx = CType(cmbGemeinde.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali) - 438000

        'Dim selob As New KeyValuePair(Of String, String)
        'selob = CType(cmbGemeinde.SelectedItem, KeyValuePair(Of String, String))
        'Dim selKey$ = selob.Key
        'Dim selvalue$ = selob.Value

        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(myvali$)
        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = myvals$ ' b(1).Replace("}", "").trim
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename = myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName
        initStrassenCombo()
        myGlobalz.sitzung.aktADR.PLZ = CInt(glob2.getPLZfromGemeinde(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName))
        myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PLZ = myGlobalz.sitzung.aktADR.PLZ
    End Sub

    Sub initStrassenCombo()
        'gemeindeDT
        DB_Oracle.holeStrasseDT4Vorgaenge()
        cmbStrasse.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub

    Private Sub cmbStrasse_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbStrasse.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As DataRowView = CType(cmbStrasse.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub

        ' Dim item3$ = item2.Row.ItemArray(0).ToString
        ' myGlobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.strasseName = item2.Row.ItemArray(0).ToString

        initHausNRCombo()
        cmbHausnr.IsDropDownOpen = True
        'todo liste erzeugen
    End Sub

    Sub initHausNRCombo()
        'gemeindeDT
        DB_Oracle.DBholeHausnrDTVorgaenge_alledb()
        cmbHausnr.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub

    Private Sub cmbHausnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbHausnr.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As DataRowView = CType(cmbHausnr.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        'Dim item3$ = item2.Row.ItemArray(0).ToString
        'Me.tbHausnr.Text = item2.Row.ItemArray(1).ToString
        'Dim halo_id% = CInt(item3$)
        myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = item2.Row.ItemArray(1).ToString
        myGlobalz.sitzung.aktADR.SekID = CLng(item2.Row.ItemArray(0))
        VorgaengeAnzeigenFuerAdresse(CInt(myGlobalz.sitzung.aktADR.SekID))
        'hole_AdressKoordinaten(halo_id%)
        'lblCoords.Content = myGlobalz.sitzung.aktADR.punkt.X & " , " & myGlobalz.sitzung.aktADR.punkt.Y
        'lblFS.Text = myGlobalz.sitzung.aktADR.FS
    End Sub

    Sub VorgaengeAnzeigenFuerAdresse(ByVal sekid%)
        myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Adresse
        myGlobalz.sitzung.aktADR.RaumbezugsID = RBid_zuSekid_holen_alledb(sekid, myGlobalz.sitzung.akt_raumbezugsTyp)
        If myGlobalz.sitzung.aktADR.RaumbezugsID > 0 Then
            'holevorgaengezuRaumbezug
            initVorgangsDT(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID))
            dg.DataContext = myGlobalz.sitzung.VorgangREC.dt
            dg2.DataContext = myGlobalz.sitzung.VorgangREC.dt
        Else
            glob2.nachricht_und_Mbox(String.Format("Es konnte kein gültiger Raumbezug gefunden werden! ({0})", sekid))
            Exit Sub
        End If
    End Sub

    Private Sub cmbGemKRZ_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbGemKRZ.SelectedItem Is Nothing Then Exit Sub
        suchentaste()
        e.Handled = True
    End Sub

    Private Sub cmbSachgebietnr_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) ' Handles cmbSachgebietnr.SelectionChanged
        If cmbSachgebietnr.SelectedValue Is Nothing Then Exit Sub
        glob2.nachricht("Ausgewählte Sachgebietsnr" & cmbSachgebietnr.SelectedValue.ToString)
        ' aktSachgebietnr = cmbSachgebietnr.SelectedValue.ToString
        tbsachgebietnr.Text = cmbSachgebietnr.SelectedValue.ToString

        suchentaste()
        e.Handled = True
    End Sub

    Private Sub ListeInsGIS_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ' MsgBox("Baustelle")
        LocalParameterFiles.erzeugeVorgangsListenDatei(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC.dt)
        glob2.neueKarteerstellen()
        e.Handled = True
    End Sub

    Private Sub tbsachgebietnr_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) 'Handles tbsachgebietnr.TextChanged
        aktSachgebietnr = tbsachgebietnr.Text
    End Sub


    Private Sub btnSgtree2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim sgt As New win_sgtree("sachgebiet-neu-1.xml")
        sgt.ShowDialog()
        tbsachgebietnr.Text = myGlobalz.AZauswahl.az.sachgebiet.Zahl.ToString
        suchentaste()
    End Sub

    Private Function getKuerzelForInitial(ByVal Initiale$) As String
        If String.IsNullOrEmpty(Initiale.ToLower) Then Return ""
        If Initiale.ToLower = "alle" Then Return ""
        Dim testbearbeiter As New clsBearbeiter
        If DB_Oracle.initBearbeiterByUserid_ausParadigmadb(testbearbeiter, "INITIAL_", Initiale) Then
            Return testbearbeiter.Kuerzel2Stellig
        Else
            Return ""
        End If

    End Function




End Class
