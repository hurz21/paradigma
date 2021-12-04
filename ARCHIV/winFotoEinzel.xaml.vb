Partial Public Class Window_FotoEinzel
    Private outfile$
    Private aktpoint As myPoint
    Private myBitmapImage As New BitmapImage()
    Private Sub Window_FotoEinzel_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            refresh()
            'pruefen ob ein raumbezug vorliegt
            aktpoint = New myPoint
            'Dim test = DBraumbezug_Mysql.getRaumbezugsCoords_2dokument(myGlobalz.sitzung.aktDokument.DocID)
            Dim test = RBtoolsns.getRaumbezugsCoords_2dokument_alledb.exe(myGlobalz.sitzung.aktDokument.DocID)
            If Not test Is Nothing Then
                aktpoint = CType(test, myPoint)
                'raumbezug ist vorhanden
                'label einschalten
                'gk-koordinaten holen
                'Else
                'raumbezug ist nicht vorhanden
                'label ausschalten
            End If
            Title = StammToolsNs.setWindowTitel.exe("edit", "Foto")
        Catch ex As Exception
            nachricht_und_Mbox("Fehler: bei startroutinge fotoeinzel:" & vbCrLf & ex.ToString)
        End Try
    End Sub


    Sub New(ByVal _aktdoku As Dokument)
        InitializeComponent()
        outfile$ = _aktdoku.FullnameCheckout
    End Sub

    Sub refresh()
        Try
            If outfile Is Nothing Then
                nachricht_und_Mbox("Datei existiert nicht: " & outfile)
                Exit Sub
            End If
            Dim test As New IO.FileInfo(outfile)
            If Not test.Exists Then
                nachricht_und_Mbox("Datei existiert nicht: " & outfile)
                Exit Sub
            End If

            myBitmapImage.BeginInit()
            myBitmapImage.UriSource = New Uri(outfile$)
            '		myBitmapImage.DecodePixelWidth = 200
            myBitmapImage.EndInit()
            Image1.Source = myBitmapImage

            myBitmapImage.UriSource = Nothing
            myBitmapImage = Nothing
        Catch ex As Exception
            nachricht_und_Mbox("Window_FotoEinzel_refresh: Datei existiert nicht: " & outfile)
            Exit Sub
        End Try
    End Sub
    Private Sub Image1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Image1.MouseDown
        '  DokArc.DateiStarten(myGlobalz.sitzung.aktDokument)
    End Sub

    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSpeichern.Click
        abspeichern()
    End Sub
    Sub abspeichern()
        If EingabeistOK() Then
            speichernDokudetail()
        End If
        Me.Close()
    End Sub
    Function EingabeistOK() As Boolean
        Return True
    End Function
    Function speichernDokudetail() As Boolean
        'DokumentenArchiv.updateAktuellesDokument(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.istVeraltet, myGlobalz.sitzung.aktDokument.Beschreibung, myGlobalz.sitzung.aktDokument.revisionssicher)
        Dim result As Integer = DokArcTools.dokUpdate.execute(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.istVeraltet, myGlobalz.sitzung.aktDokument.Beschreibung,
                                                              myGlobalz.sitzung.aktDokument.revisionssicher, myGlobalz.sitzung.aktDokument.Filedatum,
                                                              myGlobalz.sitzung.aktDokument.EXIFlat,
                                                              myGlobalz.sitzung.aktDokument.EXIFlon)

    End Function


    Private Sub chkIstveraltet_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkIstveraltet.Checked
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Image1.Source = Nothing
        If myBitmapImage IsNot Nothing Then
            myBitmapImage.UriSource = Nothing
            myBitmapImage = Nothing
        End If
        myGlobalz.winfoto.Close()
        myGlobalz.winfoto = Nothing
        Me.Close()
    End Sub


    Private Sub Hyperlink_RequestNavigate(ByVal sender As System.Object, ByVal e As System.Windows.Navigation.RequestNavigateEventArgs)
        Dim gis As New clsGISfunctions()
        gis.mittelpunktsAufruf(aktpoint, initP.getValue("GisServer.GIS_WebServer"))
        e.Handled = True
    End Sub


    Private Sub tbBeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(240, tbBeschreibung)
    End Sub

    Private Sub btnKillCoords_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If jawirklich() Then
            'Koordinaten in dokumente löschen
            myGlobalz.sitzung.aktDokument.EXIFlat = "0#0#0#"
            myGlobalz.sitzung.aktDokument.EXIFlon = "0#0#0#"
            speichernDokudetail()
            'ojekt in Parafoto löschen
            Dim itest As Integer
            Dim sekid% = DokArcTools.getID4Foto.execute(myGlobalz.sitzung.aktDokument.DocID)
            nachricht("ARCHIV: getID4Foto: >0 ist erfolg: " & sekid%)
            If sekid > 0 Then
                itest = RBtoolsns.RBFotoLoeschen_alleDB.execute(myGlobalz.sitzung.aktDokument.DocID)
                'itest = DBraumbezug_Mysql.RB_FOTO_loeschen(myGlobalz.sitzung.aktDokument.DocID)
                nachricht("ARCHIV: RB_FOTO_loeschen:  " & itest%)
                itest = RBtoolsns.Raumbezug_loeschen_bySEKid_alleDB.execute(sekid, "5")
                ' itest = DBraumbezug_Mysql.RB_loeschenBySekIDTyp(sekid, "5") 'gilt nur für Fotos besser wäre eine umwandlung des enums in integer: todo
                nachricht("ARCHIV: RB_loeschenBySekIDTyp:  : " & itest%)
            End If
            nachricht("foto wurde als raumbzug gelöscht wurde gelöscht. Bitte führen Sie einen Refresh durch.")

            'objekt in raumbezug löschen
            'refresh
        End If
        e.Handled = True
    End Sub

    Private Function jawirklich() As Boolean
        Dim res As New MessageBoxResult
        res = MessageBox.Show(
                    "Möchten Sie die Koordinate wirklich löschen ? " & vbCrLf &
                    "  ", "Koordinaten es Fotos löschen' ?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Cancel)
        Return If(res = MessageBoxResult.No, False, True)
    End Function

    Private Sub Image1_MouseDown_1(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        Try
            Process.Start(outfile)
        Catch ex As Exception
            MessageBox.Show("Problem beim Aufrud der Datei. Ggf. ist sie nicht vorhanden!" & Environment.NewLine & ex.ToString, outfile, MessageBoxButton.OK)
        End Try
        e.Handled = True
    End Sub
End Class
