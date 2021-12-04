Public Class wininfo
    Private _info As String
    Sub New(info As String)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        _info = info
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub


    Private Sub wininfo_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Title = "Bitte warten ... DB-Abfrage läuft, Timeout bei 5min."
        tbinfo.Text = "Die DB-Abfrage kann bis zu 5min. beschäftigt sein, z.B. bei Hochhäusern mit vielen Eigentümern." & Environment.NewLine &
                     "Paradigma ist solange blockiert!"
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        Dim antwort = clsMiniMapTools.dbabfrageTK5(_info, 60 * 5 * 1000)
        tbinfo.Text = antwort
        e.Handled = True
    End Sub

    Private Sub btnabbruch_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub btnClipboard_Click(sender As Object, e As RoutedEventArgs)
        Clipboard.Clear()
        Clipboard.SetText(tbinfo.Text)
        'MsgBox("Die Information befindet sich nun in Ihrer Windows-Zwischenablage. " & vbCrLf & _
        ' "Sie können Sie mit 'Strg-v'' in Ihr Dokument einfügen" & vbCrLf &
        ' "oder über die Knopf 'Zwischenablage' ein neues Dokument erzeugen. " & vbCrLf)
        tbhinweis.Text = "Die Information befindet sich nun in Ihrer Windows-Zwischenablage. " &
       "Sie können Sie mit 'Strg-v'' in Ihr Dokument einfügen."
        ' MsgBox("Die Information befindet sich nun in Ihrer Windows-Zwischenablage. " & vbCrLf & vbCrLf & _
        '"Sie können Sie mit 'Strg-v'' in Ihr Dokument einfügen" & vbCrLf & vbCrLf)
        
        e.Handled = True
    End Sub

Private Sub btnAlsDokument_Click(sender As Object , e As RoutedEventArgs)
        Dim text As String=tbinfo.text
         Dim fi As  IO.FileInfo
           If Not text.IsNothingOrEmpty Then
                    'datei erzeugen
                    myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
                    myGlobalz.sitzung.aktDokument.DateinameMitExtension ="Eigentuemerauskunft.txt"' clsString.kuerzeTextauf(text,50) & ".txt"
                    myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                      fi=New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                    IO.Directory.CreateDirectory(fi.Directory.ToString)
                    Using raus As New IO.StreamWriter(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                        raus.WriteLine(text)
                    End Using
                    If insarchivUebernehmen(myGlobalz.sitzung.aktDokument.FullnameCheckout) Then
                    End If
                End If
        close
        e.Handled=true
    End Sub
End Class
