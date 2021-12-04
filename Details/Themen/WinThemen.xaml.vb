Public Class WinThemen
    Private Property _modus As String
    Sub New(ByVal modus As String)
        InitializeComponent()
        _modus = modus
    End Sub
    Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        initThemen(True)
        Title = StammToolsNs.setWindowTitel.exe("edit", "GIS-Themen")
        FocusManager.SetFocusedElement(Me, tbThemenSuchfilter)
    End Sub

    Private Sub initThemen(ByVal allLayersChecked As Boolean?)
         ' initHgrund()
        initVgrund()
        If _modus = "maplayer_referenced" Then
            Dim a As New miniMapControl
            a.zeigeAlleRefenrenziertenLayers(multicolumnComboBox, allLayersChecked)
            '   lblTitel.Content = "Zugriff erlaubt für:"
        End If
    End Sub
    Sub initVgrund()
        Dim layers$() = (clstart.myc.kartengen.aktMap.Vgrund & clstart.myc.kartengen.aktMap.Hgrund) .Split(";"c)
        'obsolet?????
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = False
        myGlobalz.availablePresentationObjects.Clear()
        multicolumnComboBox = Nothing
        listeUUnchecken()
        Me.Close()
    End Sub


    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        '  HgrundAuswerten()
        ' If String.IsNullOrEmpty(detailsTools.getNewVgrundString()) Then
        clstart.myc.kartengen.aktMap.Vgrund = detailsTools.getNewVgrundString()
        'End If
        DialogResult = True
        myGlobalz.availablePresentationObjects.Clear()
        e.Handled = False
        Me.Close()
    End Sub

    Private Sub multicolumnComboBox_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles multicolumnComboBox.SelectionChanged
        '    MsgBox("daneben")
        e.Handled = True
    End Sub

    Private Shared Sub listeUUnchecken()
        For Each ding In myGlobalz.layerListControlObjekts
            ding.IsChecked = False
        Next
    End Sub
    Private Sub keineVgrund_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles keineVgrund.Click
        clstart.myc.kartengen.aktMap.Vgrund = ""
        ' multicolumnComboBox.ItemsSource = Nothing
        ' initThemen(False)
        listeUUnchecken()
    End Sub



    Private Sub alleVorgaengeDazu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim lNewVariable As String = "raumbez" & ";"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub

    Private Sub alleUNB_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim lNewVariable As String = "natlandgeb;lsgschilder;waldabteilung;kompensation;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub

    Private Sub alleUWB_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim lNewVariable As String = "wsgeb;wsgeb3;oberfl;ueberschw;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub

    Private Sub alleIMMI_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim lNewVariable As String = "altpro09;altflaeche;kehrbezirk;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub

    Private Sub btnThemensucheStarten_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim allLayersChecked As Boolean? = False
        If miniMapControl.istFilterOk(tbThemenSuchfilter.Text) Then
            Dim a As New miniMapControl
            a.getFilteredLayers(tbThemenSuchfilter.Text, multicolumnComboBox, allLayersChecked)
            a.getbestandlayers()
            a.zeigeAlleRefenrenziertenLayers(multicolumnComboBox, allLayersChecked)
        End If
    End Sub

    Private Sub alleBauen_click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim lNewVariable As String = "bplanrechtsw;regfnpB1;regfnpB2;regfnpHK;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub



    Private Sub flure_Click(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
        Dim lNewVariable As String = "flure;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub

    'Private Sub aktVorgangDazu_Click(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)

    'End Sub
    'Private Sub aktVorgangDazu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    'Dim lNewVariable As String = "Paradigma_" & myGlobalz.sitzung.VorgangsID & ";"
    '    Dim lNewVariable As String = myGlobalz.sitzung.aktVorgangsID & ";"
    '    If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
    '        clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
    '        initThemen(True)
    '    End If
    '    e.Handled = True
    'End Sub

    Private Sub alleUNB_Click(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
        Dim lNewVariable As String = "natlandgeb;lsgschilder;waldabteilung;kompensation;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub

    Private Sub weisseFlurkarte_Click(sender As Object, e As RequestNavigateEventArgs)
        Dim lNewVariable As String = "grundweiss;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub
    Private Sub weisseFlurkarte_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim lNewVariable As String = "grundweiss;"
        If Not clstart.myc.kartengen.aktMap.Vgrund.Contains(lNewVariable) Then
            clstart.myc.kartengen.aktMap.Vgrund &= lNewVariable
            initThemen(True)
        End If
        e.Handled = True
    End Sub
End Class
