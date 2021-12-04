Imports System.Data

Public Class WinDetailSucheFST
    'Private Property anyChange As Boolean = False
    Private modus$
    Sub New(ByVal _modus$)
        InitializeComponent()
        modus = _modus
    End Sub
    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub


    Private Sub Window_Flurstuecksauswahl_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initGemarkungsCombo()
        If modus = "ort" Then
              btnEigentuemerALKIS.Visibility = Windows.Visibility.Hidden
        End If
        If modus = "eigentümer" Then
            btnEigentuemerALKIS.Visibility = Windows.Visibility.Visible
        End If
        'anyChange = False
        Title = StammToolsNs.setWindowTitel.exe(modus, " ")
        gastLayout()
        cmbgemarkung.IsDropDownOpen=True
        e.Handled = True
    End Sub

    Sub initGemarkungsCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxgemarkungen"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\gemarkungen.xml")
    End Sub


    Private Sub cmbgemarkung_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbgemarkung.SelectionChanged
        If cmbgemarkung.SelectedItem Is Nothing Then Exit Sub

        Dim myvali$ = CStr(cmbgemarkung.SelectedValue)
        Dim myvalx = CType(cmbgemarkung.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        tbGemarkung.Text = myvals
        myGlobalz.sitzung.aktFST.normflst.gemcode = CInt(myvali)
        myGlobalz.sitzung.aktFST.normflst.gemarkungstext = tbGemarkung.Text
        initFlureCombo()
        cmbFlur.IsDropDownOpen = True
        e.Handled=true
    End Sub
    Sub initFlureCombo()
        'gemeindeDT
        DB_Oracle_sharedfunctions.holeFlureDT()
        cmbFlur.DataContext = myGlobalz.sitzung.postgresREC.dt        
    End Sub

    Private Sub cmbFlur_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbFlur.SelectionChanged
        Dim item2 As DataRowView = CType(cmbFlur.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbflur.Text = item2.Row.ItemArray(0).ToString
        myGlobalz.sitzung.aktFST.normflst.flur = CInt(item3$)
        initZaehlerCombo()
        cmbZaehler.IsDropDownOpen = True
        e.Handled=true
    End Sub
    Sub initZaehlerCombo()
        'gemeindeDT
        DB_Oracle_sharedfunctions.holeZaehlerDT()
        cmbZaehler.DataContext = myGlobalz.sitzung.postgresREC.dt
    End Sub

    Private Sub cmbZaehler_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbZaehler.SelectionChanged
        Dim item2 As DataRowView = CType(cmbZaehler.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Dim item3$ = item2.Row.ItemArray(0).ToString
        tbZaehler.Text = item2.Row.ItemArray(0).ToString
        myGlobalz.sitzung.aktFST.normflst.zaehler = CInt(item3$)
        myGlobalz.sitzung.aktFST.normflst.nenner = Nothing
        initNennerCombo()
        If myGlobalz.sitzung.postgresREC.dt.Rows.Count = 1 Then
            tbNenner.Text = myGlobalz.sitzung.postgresREC.dt.Rows(0).Item(0).ToString
            NennerVerarbeiten()
            btnEigentuemerALKIS.IsEnabled = True
              DialogResult = True
            e.Handled = True
            Me.Close()
        Else
            cmbNenner.IsDropDownOpen = True
        End If
        e.Handled=true
    End Sub

    Sub initNennerCombo()
        'gemeindeDT
        DB_Oracle_sharedfunctions.holeNennerDT()
        cmbNenner.DataContext = myGlobalz.sitzung.postgresREC.dt
    End Sub

    Private Sub NennerVerarbeiten()
        myGlobalz.sitzung.aktFST.normflst.nenner = CInt(tbNenner.Text)
        myGlobalz.sitzung.aktFST.normflst.FS = myGlobalz.sitzung.aktFST.normflst.buildFS()
        FST_tools.hole_FSTKoordinaten_undZuweisePunkt(myGlobalz.sitzung.aktFST)
        CLstart.myc.kartengen.aktMap.aktrange.point2range(myGlobalz.sitzung.aktFST.punkt, 200)
    End Sub

    Private Sub cmbNenner_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbNenner.SelectionChanged
        Dim item2 As DataRowView = CType(cmbNenner.SelectedItem, DataRowView)
        If item2 Is Nothing Then Exit Sub
        Try
        Catch ex As Exception
            Exit Sub
        End Try
        tbNenner.Text = item2.Row.ItemArray(0).ToString
        NennerVerarbeiten()
        If modus = "ort" Then
            btnEigentuemerALKIS.IsEnabled = False
            DialogResult = True
            e.Handled = True
            Me.Close()
        End If
        If modus = "eigentuemer" Then
            btnEigentuemerALKIS.IsEnabled = True
            btnEigentuemerALKIS.Visibility = Windows.Visibility.Visible
        End If
        e.Handled=true
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        Me.Close()
    End Sub

    Private Sub btnEigentuemerALKIS_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnEigentuemerALKIS.Content = "Bitte 20 sec. warten - ...."
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        FST_tools.eigentuemerALKIS(myGlobalz.sitzung.aktFST, CBool(chkInsArchiv.IsChecked), CBool(chkEreignisMap.IsChecked), myGlobalz.sitzung.aktFST.defineAbstract)
        DialogResult = True
        e.Handled = True
        Me.Close()
    End Sub

    Private Sub chkInsArchiv_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub chkEreignisMap_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

'Private Sub btnweiter_Click(sender As Object , e As RoutedEventArgs)
'          DialogResult = True
'           e.Handled = True
'    End Sub

Private Sub cmbFlur_SelectionChanged_1(sender As Object , e As SelectionChangedEventArgs)

    End Sub

Private Sub cmbZaehler_SelectionChanged_1(sender As Object , e As SelectionChangedEventArgs)

    End Sub

Private Sub cmbNenner_SelectionChanged_1(sender As Object , e As SelectionChangedEventArgs)

    End Sub
End Class
