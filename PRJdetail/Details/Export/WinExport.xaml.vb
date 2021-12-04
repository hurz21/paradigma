Public Class WinExport
    Property vid As Integer
    Property rootdir As String
    Private Sub gastLayout()
        Background = myglobalz.GetSecondBackground()
    End Sub

    Sub New(_vid As Integer)
        InitializeComponent()
        vid = _vid
    End Sub

    Private Sub WinExport_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True

        rootdir = IO.Path.Combine(CLstart.mycSimple.Paradigma_local_root, "export")
        TBexportDir.Text = "Export nach: " & Environment.NewLine &
        rootdir
        gastLayout()
    End Sub

    Private Sub btnAbbruch(sender As Object, e As RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnOpenDestDir(sender As Object, e As RoutedEventArgs)
        Process.Start(rootdir)
        e.Handled = True
    End Sub

    Private Sub btnStart(sender As Object, e As RoutedEventArgs)
        exportTool.ExportpfadFeststellenUndVerzeichnisseErstellen(vid, rootdir)
        tbInfotext.Text &= "==============================================================================" & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        tbInfotext.Text &= "Export beginnt" & " ... Bitte warten ..." & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

        If CBool(chkExpVerlauf.IsChecked) Then
            tbInfotext.Text &= "Verlauf Übersicht" & " ... Bitte warten ..." & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            exportTool.DirLeeraeumen(pfad & "\Verlauf")
            expVerlauf.VERLAUF_CsvListeErstellen(pfad & "\Verlauf\UEBERSICHT.csv")
            tbInfotext.Text &= "Verlauf Einzelobjekte" & " ... Bitte warten ..." & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            expVerlauf.VERLAUF_einzelObjekteErstellen(pfad & "\Verlauf", "html")
        End If



        If CBool(chkExpBeteiligte.IsChecked) Then
            tbInfotext.Text &= "Beteiligte" & " ... Bitte warten ..." & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            exportTool.DirLeeraeumen(pfad & "\Beteiligte")
            expBeteiligte.BETEILIGTE_CsvListeErstellen(pfad & "\Beteiligte\UEBERSICHT.csv")
            expBeteiligte.BETEILIGTE_einzelObjekteErstellen(pfad & "\Beteiligte")
        End If


        If CBool(chkExpDokus.IsChecked) Then
            tbInfotext.Text &= "Dokumente" & " ... Bitte warten ..." & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            exportTool.DirLeeraeumen(pfad & "\Dokumente")
            exportTool.DirLeeraeumen(pfad & "\Raumbezug")
            expDokumente.dokumente_CsvListeErstellen(pfad & "\Dokumente\UEBERSICHT.csv")
            expDokumente.Dokumente_einzelObjekteErstellen(pfad & "\Dokumente", True,
                                                          pfad & "\Raumbezug",
                                                          CBool(chkmsg2HTML.IsChecked),
                                                          CBool(chkWord2PDF.IsChecked)
                                                          )
        End If

        If CBool(chkExpFotos.IsChecked) Then
            tbInfotext.Text &= "Fotos" & " ... Bitte warten ..." & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            exportTool.DirLeeraeumen(pfad & "\Fotos")
            expDokumente.Fotos_einzelObjekteErstellen(pfad & "\Fotos", vid, True)
        End If


        erzeugeBerichtsvorlage(pfad & "\Bericht_Vg_" & myglobalz.sitzung.aktVorgangsID & ".txt")

        tbInfotext.Text &= "verzeichnis anzeigen" & " ... Bitte warten ..." & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        tbInfotext.Text &= "FERTIG - EXPORT IST ABGESCHLOSSEN ==========================" & Environment.NewLine : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

        MsgBox("Benutzen Sie DiskAid um die Daten auf das Ipad zu kopieren!")
        Process.Start(pfad)
        '  Me.Close()
        e.Handled = True
    End Sub

    Private Sub erzeugeBerichtsvorlage(p1 As String)
        Dim fitest As New IO.FileInfo(p1)
        If fitest.Exists Then
            'bestehende datei darf nicht überschrieben werden
            fitest = Nothing
            Exit Sub
        End If
        fitest = Nothing
        Dim sb As New Text.StringBuilder
        sb.Append("Aktennotiz" & Environment.NewLine)
        sb.Append("Ortstermin bei: " & Environment.NewLine)
        sb.Append("am: " & LIBgemeinsames.clsString.date2string(Now, 4) & "  " & Environment.NewLine)
        sb.Append("Gespräch mit Herrn " & Environment.NewLine)
        sb.Append(" " & Environment.NewLine)
        My.Computer.FileSystem.WriteAllText(p1, sb.ToString, False)
        'IO.File.Create(p1)
    End Sub

End Class
