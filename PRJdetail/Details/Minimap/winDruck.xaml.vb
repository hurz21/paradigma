Imports System.Data

Public Class winDruck
    Property _canwas As Canvas
    Property _ableitungskreis As clsAbleitungskreis
        Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground() 
    End Sub

    Sub New(ByVal mycanvas As Canvas, ByVal myableitungskreis As clsAbleitungskreis)
        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        ' ByVal myCanvas As Canvas, ByVal ableitungskreis As clsAbleitungskreis
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        _canwas = mycanvas
        _ableitungskreis = myableitungskreis
    End Sub
    Private Sub btnSchnelldruck_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        clsBerichte.erstelleKartendruck(_canwas, _ableitungskreis)
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnA0druck(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'MessageBox.Show("Bitte teilen Sie Frau Krömmelbein (Tel. 4440) die Paradigmanummer (" & myGlobalz.sitzung.VorgangsID & ") mit. Sprechen Sie mit Ihr das Layout ab.")
        stpDruckdetails.Visibility = Windows.Visibility.Visible
        A4Schnell.Visibility = Windows.Visibility.Collapsed
        stpAuswahlA0.Visibility = Windows.Visibility.Collapsed
        Title = "Drucken der Karte auf dem Großplotter"
        If drucktools.istBilddateiSchonda(clstart.myc.kartengen.gifKartenDateiFullName) Then
            kartenbilddarstellen()
        Else
            System.Threading.Thread.Sleep(5000)
            kartenbilddarstellen()
        End If
        e.Handled = True
    End Sub

    Private Sub btnAbbruchroutine(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
        e.Handled = True
    End Sub


    Private Sub btnSendJob_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim Telnr As String = myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Telefon1
        If drucktools.mehlen(bildemessage()) = "0" Then
            MessageBox.Show("Der Auftrag wurde verschickt. " & Environment.NewLine &
                        "Für evtl. Rückfragen wird sich das GIS-Team bei Ihnen (Tel: " & Telnr & ") melden!")
        Else
            MessageBox.Show("Der Auftrag wurde verschickt. " & Environment.NewLine &
                        "Es gab ein Problem Bitte beim Admin melden!")
        End If
        e.Handled = True
    End Sub

    Private Function bildemessage() As String
        Dim zeilentrenner As String = " " & Environment.NewLine
        Dim Telnr As String = myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Telefon1
        Dim text As String = ""
        text = text & "Job von: " & myGlobalz.sitzung.aktBearbeiter.Name & ", " & myGlobalz.sitzung.aktBearbeiter.Vorname &
          ". Telefonummer: " & Telnr & Environment.NewLine & zeilentrenner
        text = text & "Datum: " & Now.ToString() & Environment.NewLine & zeilentrenner
        text = text & "Bis Termin: " & tbTermin.Text.ToString() & Environment.NewLine & zeilentrenner
        text = text & "ParadigmaNr: " & myGlobalz.sitzung.aktVorgangsID.ToString() & Environment.NewLine & zeilentrenner
        text = text & "Maßstab: " & tbmasstab.Text & Environment.NewLine & zeilentrenner
        text = text & "Format: " & tbForat.Text & Environment.NewLine & zeilentrenner
        text = text & "Bereich: " & tbBereich.Text & Environment.NewLine & zeilentrenner
        text = text & "Vordergrund: " & tbVordergrund.Text & Environment.NewLine & zeilentrenner
        text = text & "Hintergrund: " & tbhintergrund.Text & Environment.NewLine & zeilentrenner
        text = text & "Bemerkung: " & tbBemerkung.Text & Environment.NewLine & zeilentrenner
        text = text & Environment.NewLine & zeilentrenner & Environment.NewLine & zeilentrenner
        text = text & "Der Anwender hat zur Zeit folgenden Hintergrund geladen: " & Environment.NewLine &
            clstart.myc.kartengen.aktMap.Hgrund & Environment.NewLine & zeilentrenner
        text = text & "Der Anwender hat zur Zeit folgenden Vordergrund geladen: " & Environment.NewLine &
            clstart.myc.kartengen.aktMap.Vgrund & Environment.NewLine & zeilentrenner
        text = text & " " & clstart.myc.kartengen.aktMap.HgrundTitel & Environment.NewLine & zeilentrenner
        text = text & "Die akt. Kartenansicht des Users findet sich im Anhang dieser Mail. " & Environment.NewLine & zeilentrenner
        Return text
    End Function

    Private Sub vordergrundtabelleFuellen(VGRUNDtemp As String, hGRUNDtemp As String)
        Dim summe As String = ""
        Dim a As String = ""
        Dim ab As New miniMapControl
        ab.zeigeAlleRefenrenziertenLayersExtracted(VGRUNDtemp , hGRUNDtemp )
        If ab.featuretab.IsNothingOrEmpty Then
            tbVordergrund.Text = "niente"
        Else
            For Each zeile As DataRow In ab.featuretab.AsEnumerable
                a = zeile.Item("TITEL").ToString
                summe = summe & a & Environment.NewLine
            Next
            tbVordergrund.Text = summe
        End If
    End Sub
    Private Sub winDruck_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        vordergrundtabelleFuellen(CLstart.myc.kartengen.aktMap.Vgrund, CLstart.myc.kartengen.aktMap.hgrund)
          gastLayout()
        e.Handled = True
    End Sub

    Private Sub kartenbilddarstellen()
        Dim myBitmapImage As New BitmapImage()
        Try
            myBitmapImage.BeginInit()
            myBitmapImage.UriSource = New Uri(clstart.myc.kartengen.gifKartenDateiFullName)
            '		myBitmapImage.DecodePixelWidth = 200
            myBitmapImage.EndInit()
            Dim brusch As New ImageBrush
            brusch.ImageSource = myBitmapImage
            minicanvas.Background = brusch ' myBitmapImage	 
            '   imgKarte.Source = myBitmapImage
            '  myBitmapImage = Nothing
            brusch = Nothing
        Catch ex As Exception
            nachricht("warnung: bilddatei konnte nicht gefunden werden! " & ex.ToString)
        End Try
    End Sub

    Private Sub cmbFormat_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbFormat.SelectionChanged
        If cmbFormat.SelectedValue Is Nothing Then Exit Sub
        Dim item As String = CType(cmbFormat.SelectedValue.ToString, String)
        If String.IsNullOrEmpty(item) Then Exit Sub
        Dim a() As String = item.Split(":"c)
        tbForat.Text = a(1)
    
        e.Handled = True
    End Sub

    Private Sub cmbScale_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbScale.SelectionChanged
        If cmbScale.SelectedValue Is Nothing Then Exit Sub
        Dim item As String = CType(cmbScale.SelectedValue.ToString, String)
        If String.IsNullOrEmpty(item) Then Exit Sub
        Dim a() As String = item.Split(":"c)
        tbmasstab.Text = a(1)
        If a.Length > 2 Then
            tbmasstab.Text = tbmasstab.Text & a(2)
        End If
        e.Handled = True
    End Sub

    Private Sub cmbTermin_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cmbTermin.SelectionChanged
        If cmbTermin.SelectedValue Is Nothing Then Exit Sub
        Dim item As String = cmbTermin.SelectedValue.ToString 'CType(cmbTermin.SelectedValue, String)
        If String.IsNullOrEmpty(item) Then Exit Sub
        Dim a() As String = item.Split(":"c)
        tbTermin.Text = a(1)
        e.Handled = True
    End Sub
End Class
