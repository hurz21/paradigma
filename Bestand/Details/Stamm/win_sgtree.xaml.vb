Public Class win_sgtree
    Private Property xmlfile As String
    '    Private Property Zaehler% = 0
    Private Property Meingebiet$ = ""

    Public Property publicNR As String
    Public Property publicsgHeader As String

    Private Sub win_sgtree_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        clsBaumbilden.ladeXML(xmlfile, TreeView1)
        Meingebiet = myGlobalz.sitzung.aktBearbeiter.ExpandHeaderInSachgebiet
        Title = StammToolsNs.setWindowTitel.exe("edit", "Aktenplan")
        If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
            tbsgNr.IsEnabled = True
            ' tbsgNr.Background = Brushes.Aquamarine
            FocusManager.SetFocusedElement(Me, tbsgNr)
        Else
            tbsgNr.IsEnabled = False
        End If
    End Sub

    Private Sub expandtree(ByVal einaus As Boolean)
        For Each item As Object In TreeView1.Items
            Dim treeItem As TreeViewItem = CType(TreeView1.ItemContainerGenerator.ContainerFromItem(item), TreeViewItem)
            treeItem.IsExpanded = True
            If treeItem IsNot Nothing Then
                expandAll(treeItem, einaus)
            End If
        Next
    End Sub

    Sub expandAll(ByVal meineitems As ItemsControl, ByVal expand As Boolean)
        For Each item As Object In meineitems.Items
            Dim childControl As ItemsControl = CType(meineitems.ItemContainerGenerator.ContainerFromItem(item), ItemsControl)
            If childControl IsNot Nothing Then
                expandAll(childControl, expand)
            End If
            Dim mitem As TreeViewItem = CType(childControl, TreeViewItem)
            If mitem IsNot Nothing Then
                mitem.IsExpanded = expand
            End If
        Next
    End Sub
    Private Sub expandtreeHEADER(ByVal einaus As Boolean, ByVal Headertext$)
        If String.IsNullOrEmpty(Headertext) Then Exit Sub
        For Each item As Object In TreeView1.Items
            Dim treeItem As TreeViewItem = CType(TreeView1.ItemContainerGenerator.ContainerFromItem(item), TreeViewItem)
            treeItem.IsExpanded = True
            If treeItem IsNot Nothing Then
                expandAllHEADER(treeItem, einaus, Headertext$)
            End If
        Next
    End Sub

    Sub expandAllHEADER(ByVal meineitems As ItemsControl, ByVal expand As Boolean, ByVal Headertext$)
        If String.IsNullOrEmpty(Headertext) Then Exit Sub
        For Each item As Object In meineitems.Items
            Dim childControl As ItemsControl = CType(meineitems.ItemContainerGenerator.ContainerFromItem(item), ItemsControl)
            If childControl IsNot Nothing Then
                expandAll(childControl, expand)
            End If
            Dim mitem As TreeViewItem = CType(childControl, TreeViewItem)
            If mitem IsNot Nothing Then
                If mitem.Tag IsNot Nothing Then
                    If mitem.Header.ToString = Headertext$ Then
                        ' MsgBox("huhu")
                        mitem.IsExpanded = expand
                    End If
                End If
                ' mitem.IsExpanded = expand
            End If
        Next
    End Sub


    Sub TVgetclick() Handles TreeView1.SelectedItemChanged
        Try
            If TreeView1.SelectedItem Is Nothing Then Exit Sub

            Dim tv As New TreeViewItem
            tv = CType(TreeView1.SelectedItem, TreeViewItem)
            If Not tv.Tag Is Nothing Then
                If tv.Tag.ToString.Length < 4 Then Exit Sub
                tbsgNr.Text = tv.Tag.ToString
                publicNR = tv.Tag.ToString
                tbsgtext.Text = glob2.klammerraus(tv.Header.ToString)
                  publicNR =  tbsgtext.Text
                publicsgHeader = tbsgtext.Text

                If tv.ToolTip IsNot Nothing Then
                    tbsgnrAlt.Text = tv.ToolTip.ToString
                Else
                    tbsgnrAlt.Text = ""
                End If

                btnWeiter.IsEnabled = True
                tv.IsSelected = False
                tv = Nothing
            End If
        Catch ex As Exception
            nachricht_und_Mbox("getclick in w aktenzeichen: " & vbCrLf & ex.ToString)
        End Try
    End Sub





    Public Sub New(ByVal IN_xml_file$)
        InitializeComponent()
        'xmlfile = String.Format("{0}\Paradigma\config\combos\{1}",
        '                        Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
        '                        IN_xml_file$)
        xmlfile = IN_xml_file$
    End Sub

    Private Sub btnAllesAusklappen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        expandtree(True)
    End Sub

    Private Sub btnAllesEinklappen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Meingebiet = ""
        expandtree(False)
    End Sub

    Private Sub TreeView1_LayoutUpdated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TreeView1.LayoutUpdated
        ' If Zaehler = 0 Then Zaehler += 1
        expandtreeHEADER(True, Meingebiet) ' "1-Abfallwirtschaft")

    End Sub



    Private Sub btnSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        trefferliste.Items.Clear()
        clsBaumbilden.sucheStichwortInXML(xmlfile, tbSuchwort.Text.ToLower, trefferliste)
        e.Handled=true
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Me.Close()
    End Sub


    Private Sub btnWeiter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnWeiter.Click
        Dim sgtext As String
        sgtext = StammToolsNs.test.getSGtextZuSgNr(tbsgNr.Text, xmlfile)
        If String.IsNullOrEmpty(sgtext) Then
            MessageBox.Show("Die Sachgebietsnummer <" & tbsgNr.Text & "> ist nicht gültig!!!")
            tbsgNr.Background = Brushes.Red
        Else
            tbsgtext.Text = sgtext
            'CLstart.myc.AZauswahl.az.sachgebiet.Zahl = tbsgNr.Text
            'CLstart.myc.AZauswahl.az.sachgebiet.Header = glob2.klammerraus(tbsgtext.Text)

            'CLstart.myc.AZauswahl.az.sachgebiet.Zahl = publicNR
            'CLstart.myc.AZauswahl.az.sachgebiet.Header = publicsgHeader
            publicNR = tbsgNr.Text
            publicsgHeader = glob2.klammerraus(tbsgtext.Text)
            'myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl = tbsgNr.Text
            'myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header = glob2.klammerraus(tbsgtext.Text)
            Me.Close()
        End If
    End Sub

 
End Class
