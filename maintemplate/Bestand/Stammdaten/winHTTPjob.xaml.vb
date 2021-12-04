Public Class winHTTPjob
    Property user As String


    Private Sub btnStart_Click(sender As Object, e As RoutedEventArgs)
        userbestimmen()

        tbINfo2.Text = " Job ist in Arbeit - bitte warten ... "
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

        LocalParameterFiles.erzeugeVorgangsListenDatei(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC.dt, user)
        Dim result As String
        result = glob2.neueKarteerstellen(user)
        Dim a As String()
        a = result.Trim.Split("#"c)
        result = a(0).Trim & Environment.NewLine &
            a(1).Trim & " Raumbezüge wurden verarbeitet ! " &
                 " Sie können das Webgis nun starten "
        tbINfo2.Text = result

        e.Handled = True
    End Sub

    Private Sub userbestimmen()
        If Environment.UserName = "nitsch" Then
            user = "Feinen_J"
        Else
            user = Environment.UserName
        End If
    End Sub

    Private Sub btnWebgisaufrufen_Click(sender As Object, e As RoutedEventArgs)
        Dim aktpoint As New myPoint
        aktpoint.X = 484294 : aktpoint.Y = 5540681 : aktpoint.z = 15000
        Dim themen As String = user & "&active_layer=" & user
        Dim gis As New clsGISfunctions()
        gis.mittelpunktsAufruf(aktpoint, initP.getValue("GisServer.GIS_WebServer"), themen)
        e.Handled = True
    End Sub

    Private Sub winHTTPjob_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        userbestimmen()
        Dim tsest As String = ""
        tsest &= "Was passiert jetzt ?" & Environment.NewLine & Environment.NewLine
        tsest &= "Die Ebene >>> " & user & " <<< wird nun neu erstellt." & Environment.NewLine
        tsest &= "In dieser Ebene finden Sie die Raumbezüge der ausgewählten Vorgänge." & Environment.NewLine
        tsest &= "Wenn der Job erfolgreich war, können Sie anschließend das Webgis starten und." & Environment.NewLine
        tsest &= "und die Ebene in der Karte prüfen." & Environment.NewLine
        tsest &= " Die Ebene wird nur temporär erstellt und ist nur kurz verfügbar." & Environment.NewLine
        tsest &= Environment.NewLine
        tsest &= Environment.NewLine
        tsest &= "Drücken Sie nun auf 'Start'" & Environment.NewLine

        tbINfo.Text = tsest
    End Sub
End Class
