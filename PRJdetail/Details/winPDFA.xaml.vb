Public Class winPDFA

    Private _list As List(Of clsPresDokumente)
    Private _lokalerCheckoutcache As String
    Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub
    Sub New(list As List(Of clsPresDokumente), lokalerCheckoutcache As String)
        ' TODO: Complete member initialization 
        InitializeComponent()
        _list = list
        _lokalerCheckoutcache = lokalerCheckoutcache
    End Sub

    Private Sub winPDFA_LoadedExtracted()
        Dim erfolg As Boolean
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dim count As Integer = 0
        For Each dok As clsPresDokumente In _list
            'myGlobalz.Arc.einzeldokument_auschecken(_lokalerCheckoutcache & "\" & dok.VorgangsID & "\" & dok.DocID, dok)
            myGlobalz.Arc.einzeldokument_auschecken(dok.makeFullname_CheckoutPath(dok.VorgangsID, _lokalerCheckoutcache, dok.DocID), dok)
            erfolg = nachZielKopieren.AlsPDFAkopieren.exe(dok, False, "Stellungnahme_FD_Umwelt_") 'myGlobalz.sitzung.aktVorgang.istConjectVorgang)
            count += 1
            tbArbeit.Text = tbArbeit.Text & count & ":  " & dok.DateinameMitExtension & Environment.NewLine
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
            If Not erfolg Then
                tbArbeit.Text = tbArbeit.Text & "---- PDFA-Konvertierung gescheitert: " & dok.DateinameMitExtension & Environment.NewLine
            End If
        Next
        tbArbeit.Text = tbArbeit.Text & "--------------------------- "
        tbArbeit.Text = tbArbeit.Text & "----- Fertig          ----- "
        tbArbeit.Text = tbArbeit.Text & "--------------------------- "
        Background = Brushes.LightGreen
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
    End Sub

    Private Sub winPDFA_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tbINfo.Text = " Es werden " & _list.Count & " Worddokumente nach PDF/A gesichert.  "
    End Sub

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        winPDFA_LoadedExtracted()
        e.Handled = True
    End Sub

    Private Sub clickAbbruch(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub
End Class
