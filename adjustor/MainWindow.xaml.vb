Class MainWindow 
    Property dataPfad As String
    Property imagePfad As String

    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        dataPfad = "O:\UMWELT\B\GISDatenEkom\div\config\combos\"
        imagePfad = "O:\UMWELT\B\GISDatenEkom\div\images\combos\"
        MessageBox.Show("Bevor Sie Dateien ändern sollten Sie eine Kopie anlegen. Benutzen Sie hierzu den 'Backup' Knopf.", "Sicherungshinweis")
        e.Handled = True
    End Sub

    Private Sub btnstart1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
         zeigeListe(dataPfad, "detail_ereignisseTitel.xml")
        e.Handled = True
    End Sub 

    Private Sub showImage1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "ereignisseTitel.GIF"))
        e.Handled = True
    End Sub

    Private Sub showImage2(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "raumnr.GIF"))
        e.Handled = True
    End Sub

    Private Sub btnstart2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "raumnr.xml")
        e.Handled = True
    End Sub 
  
    Private Sub btnstart3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "paragraf.xml")
        e.Handled = True
    End Sub

    Private Sub showImage3(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "paragraf.GIF"))
        e.Handled = True
    End Sub

    Private Sub btnstart4_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeGEMKRZListe(dataPfad, "gemKRZn.xml")
        e.Handled = True
    End Sub

    Private Sub showImage4(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "gemkrz.GIF"))
        e.Handled = True
    End Sub

    Private Sub btnstart5_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "detail_beteiligte_rollen.xml")
        e.Handled = True
    End Sub

    Private Sub showImage5(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "detail_beteiligte_rollen.GIF"))
        e.Handled = True
    End Sub
    Private Sub btnstart8_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "beteiligte_funktion.xml")
        e.Handled = True
    End Sub
    Private Sub showImage8(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "beteiligte_funktion.gif"))
        e.Handled = True
    End Sub


    Private Sub btnstart6_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "dokument_schlagworte.xml")
        e.Handled = True
    End Sub

    Private Sub showImage6(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "dokument_schlagworte.GIF"))
        e.Handled = True
    End Sub

    Private Sub btnstart7_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "Stakeholder_Rollen.xml")
        e.Handled = True
    End Sub

    Private Sub showImage7(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "stakeholder_rollen.gif"))
        e.Handled = True
    End Sub

    Private Sub btnstart9_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "detail_gishintergrund.xml")
        e.Handled = True
    End Sub
    Private Sub showImage9(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "detail_gishintergrund.gif"))
        e.Handled = True
    End Sub


    Private Sub btnstart10_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "vorlage_dokumentschlagworte.xml")
        e.Handled = True
    End Sub
    Private Sub showImage10(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "vorlage_dokumentschlagworte.gif"))
        e.Handled = True
    End Sub



    Private Sub btnstart11_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "vorlage_ereignissetypen.xml")
        e.Handled = True
    End Sub
    Private Sub showImage11(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "vorlage_ereignissetypen.gif"))
        e.Handled = True
    End Sub


    Private Sub btnstart12_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "dokumentdateitypen.xml")
        e.Handled = True
    End Sub
    Private Sub showImage12(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "dokumentdateitypen.gif"))
        e.Handled = True
    End Sub
    Private Sub btnstart13_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "rbfunktion.xml")
        e.Handled = True
    End Sub
    Private Sub showImage13(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "rbfunktion.gif"))
        e.Handled = True
    End Sub


    Private Sub btnstart14_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "detail_ereignisse.xml")
        e.Handled = True
    End Sub
    Private Sub showImage14(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "detail_ereignisse.gif"))
        e.Handled = True
    End Sub

    Private Sub btnstart15_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "detail_ereignisseKURZ.xml")
        e.Handled = True
    End Sub
    Private Sub showImage15(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "ereignis_typauswahl.gif"))
        e.Handled = True
    End Sub


    Private Sub btnstart16_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "gemarkungen.xml")
        e.Handled = True
    End Sub
    Private Sub showImage16(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "gemarkungen.gif"))
        e.Handled = True
    End Sub

    Private Sub btnstart17_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "gemeinden.xml")
        e.Handled = True
    End Sub
    Private Sub showImage17(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "gemeinden.gif"))
        e.Handled = True
    End Sub

    Private Sub btnstart18_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeListe(dataPfad, "sachgebiet-neu-1.xml")
        e.Handled = True
    End Sub
    Private Sub showImage18(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        zeigeScreenshot(IO.Path.Combine(imagePfad, "sachgebiete.gif"))
        e.Handled = True
    End Sub

    Private Sub btnBackup_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim erfolg As Boolean
        erfolg = glob2.backupAllXML(dataPfad)
        e.Handled = True
    End Sub

    Private Sub showImage12(sender As Object, e As MouseEventArgs)

    End Sub

    Private Sub btnstart19_Click(sender As Object, e As RoutedEventArgs)
        zeigeListe(dataPfad, "zahlung_typ.xml")
        e.Handled = True
    End Sub

    Private Sub btnVerzeichnis_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Process.Start(dataPfad)
    End Sub

    Private Sub btnstart20_Click(sender As Object, e As RoutedEventArgs)
        zeigeListe(dataPfad, "vorlage_Dateivorname.xml")
        e.Handled = True
    End Sub
End Class
