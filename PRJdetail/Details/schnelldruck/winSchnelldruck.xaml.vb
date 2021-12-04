Public Class winSchnelldruck
    Property msgFilesDrucken As Boolean = False
    Property druckdokumente As New List(Of clsPresDokumente)
    Property dauerInSekunden As Integer = 30
    Sub New()
        InitializeComponent()
    End Sub


    Private Sub winSchnelldruck_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True

        'If schnelldruckTools.hatMSGFiles(Psession.presDokus) Then
        '    msgFilesDrucken = schnelldruckTools.sollMSGGedrucktWerden(msgFilesDrucken)
        'End If
        druckdokumente = schnelldruckTools.reduziereAufAusgwaehlte(Psession.presDokus)
        druckdokumente = druckdokumente.OrderBy(Function(x) x.Checkindatum).ToList()
        Dim dauerInSekunden As Integer = CInt(tbSekunden.Text)

        tbinfo.Text = schnelldruckTools.getInfoDokues(druckdokumente) &
        "Anzahl Dokumente:  " & druckdokumente.Count & ". Dauer ca. " & druckdokumente.Count * CInt(tbSekunden.Text) & " Sekunden"
        tbinfo.Text = tbinfo.Text & Environment.NewLine
        tbinfo.Text = tbinfo.Text & " ------------------------------------" & Environment.NewLine
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        tbinfo.Text = tbinfo.Text & schnelldruckTools.Result
    End Sub
    Async Sub btnDruck_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        msgFilesDrucken = CBool(cbMitMsg.IsChecked)
        dauerInSekunden = CInt(tbSekunden.Text)
        Dim task As System.Threading.Tasks.Task(Of Integer) = Dokus_MehrfachDruckenAsync(druckdokumente, msgFilesDrucken, dauerInSekunden)

    End Sub
    Async Function Dokus_MehrfachDruckenAsync(druckdokumente As List(Of clsPresDokumente), msgFilesDrucken As Boolean, dauerInSekunden As Integer) As System.Threading.Tasks.Task(Of Integer)
        Dim ausgabeVerzeichnis As String = ""
        Dim icount As Integer = 0
        Dim result As String = ""
        Try
            l("Dokus_MehrfachDrucken---------------------- anfang")
            'Psession.presDokus = Psession.presDokus.OrderByDescending(Function(x) x.Filedatum).ToList() 'reihenfolge zeitlich umkehren 
            For Each dockument As clsPresDokumente In druckdokumente
                einzeldoku(result, dockument, ausgabeVerzeichnis, icount)
                'Dim task As System.Threading.Tasks.Task(Of Boolean) = einzeldoku(result, dockument, ausgabeVerzeichnis, icount)
                'Dim resultsss As Boolean = Await task
            Next
            result = result & "Mehrfachdruck von Dokumenten: Es wurden " & icount & " Dokumente gedruckt " & Environment.NewLine
            result = result & "--------------- F E R T I G -------------------------------------------- " & Environment.NewLine
            tbinfo.Text = result

            nachricht("Kopieren von Dokumenten: Es wurden " & icount & " Dokumente kopiert")
            l("Dokus_MehrfachDrucken---------------------- ende")
            Return icount
        Catch ex As Exception
            l("Fehler in Dokus_MehrfachDrucken: ", ex)
            Return -1
        End Try
    End Function


    Function einzeldoku(ByRef result As String, dockument As clsPresDokumente, ausgabeVerzeichnis As String, ByRef icount As Integer) As Boolean
        Try
            l(" MOD einzeldoku anfang")
            dockument.makeFullname_Checkout(dockument.VorgangsID, myGlobalz.Arc.lokalerCheckoutcache, dockument.DocID, ausgabeVerzeichnis)
            Dim checkoutResult As String = checkout.checkout(dockument, dockument.VorgangsID)   'checkout findet IMMER statt
            If checkoutResult.StartsWith("fehler") Then
                l("fehler beim checkout")
                MessageBox.Show("Wird übersprungen. Fehlt Datei? Bitte prüfen: " & dockument.DateinameMitExtension, "Fehler beim Checkout")
                Return False
            End If
            result = result & " " & "------------------------" & Environment.NewLine
            result = result & " " & dockument.DateinameMitExtension & Environment.NewLine
            tbinfo.Text = result : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            Dim erfolgreich As Boolean = False
            If dockument.DokTyp = DokumentenTyp.DOC Then
                nachZielKopieren.dokumentdrucken.printbatchDOCX(dockument,
                                                "WINWORD",
                                        System.Text.Encoding.ASCII,
                                        dockument.FullnameCheckout)
                result = result & "----------------------- " & " job erstellt " & Environment.NewLine
                'tbinfo.Text = result : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                icount += 1
            End If
            If dockument.DokTyp = DokumentenTyp.PDF Then
                nachZielKopieren.dokumentdrucken.printbatchPDF(dockument,
                                                "C:\Program Files (x86)\Adobe\Acrobat Reader DC\Reader\AcroRd32.exe",
                                        System.Text.Encoding.ASCII,
                                       dockument.FullnameCheckout)
                result = result & "----------------------- " & " job erstellt " & Environment.NewLine
                'tbinfo.Text = result : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
                icount += 1
            End If
            If dockument.DokTyp = DokumentenTyp.MSG And
               msgFilesDrucken = True Then
                Dim outfile As String
                Dim fi As New IO.FileInfo(dockument.FullnameCheckout)
                outfile = fi.DirectoryName & "\" & dockument.DocID & ".rtf"
                fi = Nothing
                If MeinOutlook.msg2rtf(dockument.FullnameCheckout, outfile) Then
                    IO.File.Delete(dockument.FullnameCheckout)
                    result = result & " " & " aufgeräumt " & Environment.NewLine
                End If
                nachZielKopieren.dokumentdrucken.printbatchRTF(dockument,
                                                    "WORDPAD",
                                            System.Text.Encoding.ASCII,
                                           outfile)
                result = result & "----------------------- " & " job erstellt " & Environment.NewLine
                icount += 1
            Else
                Return False
            End If
            CLstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
            CLstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
            CLstart.myc.aLog.komponente = "detail"
            'CLstart.myc.aLog.aktion = "dokument in anderen Vorgang (" & zielvorgang.Stammdaten.ID & ") kopieren: " &
            '    dockument.DateinameMitExtension
            CLstart.myc.aLog.log()
            result = result & "... warte  ... " & dauerInSekunden & " " & Environment.NewLine
            tbinfo.Text = result : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            tbinfo.Text = result : Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            Threading.Thread.Sleep(dauerInSekunden * 1000)


            l(" MOD einzeldoku ende")
            Return True
        Catch ex As Exception
            l("Fehler in einzeldoku: " & ex.ToString())
            Return False
        End Try
    End Function
End Class
