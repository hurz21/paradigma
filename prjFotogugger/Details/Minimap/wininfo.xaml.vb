Public Class wininfo
    Private _info As String
    Private _Beschreibung As String

    'Private Sub gastLayout()
    '    Background = myglobalz.GetSecondBackground()
    'End Sub

    Sub New(info As String)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()
        _info = info
        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub


    Private Sub wininfo_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Title = "Bitte warten ... DB-Abfrage läuft, Timeout bei 5min."
        'tbinfo.Text = "Die DB-Abfrage kann bis zu 5min. beschäftigt sein, z.B. bei Hochhäusern mit vielen Eigentümern." & Environment.NewLine &
        '             "Paradigma ist solange blockiert!"
        'Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        'Dim antwort = clsMiniMapTools.dbabfrageTK5(_info, 60 * 5 * 1000)
        'tbinfo.Text = antwort
        'gastLayout()
        'e.Handled = True
    End Sub

    Private Sub btnabbruch_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub btnClipboard_Click(sender As Object, e As RoutedEventArgs)
        Clipboard.Clear()
        Clipboard.SetText(tbinfo.Text)

        tbhinweis.Text = "Die Information befindet sich nun in Ihrer Windows-Zwischenablage. " &
       "Sie können Sie mit 'Strg-v'' in Ihr Dokument einfügen."


        e.Handled = True
    End Sub

    Private Sub btnAlsDokument_ClickExtracted()
        Dim text As String = tbinfo.Text
        Dim fi As IO.FileInfo
        Dim fs As String
        Dim flst As New clsFlurstueck()
        Dim gemarkungstext As String
        fs = getfsFrom(_info)
        fs = clsFlurstueck.NewFS2OldFS(fs)
        flst = New clsFlurstueck(fs)
        flst.splitOldFS(fs)
        gemarkungstext = bildeGemarkungstextTextFuerSchlagworte(flst)

        _Beschreibung = "Eigent_" & gemarkungstext & "_Flur_" & flst.flur & "_" & flst.zaehler & "_" & flst.nenner
        If Not text.IsNothingOrEmpty Then
            'datei erzeugen
            myglobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            myglobalz.sitzung.aktDokument.Beschreibung = _Beschreibung
            myGlobalz.sitzung.aktDokument.DateinameMitExtension = fs & ".txt" ' LIBgemeinsames.clsString.kuerzeTextauf(text,50) & ".txt"
            Dim ausgabeVerzeichnis As String = ""
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
            fi = New IO.FileInfo(myglobalz.sitzung.aktDokument.FullnameCheckout)
            IO.Directory.CreateDirectory(fi.Directory.ToString)
            fi = Nothing
            Using raus As New IO.StreamWriter(myglobalz.sitzung.aktDokument.FullnameCheckout)
                raus.WriteLine(text)
            End Using
            If insarchivUebernehmen(myglobalz.sitzung.aktDokument.FullnameCheckout, myglobalz.sitzung.aktDokument.Beschreibung) Then
            End If
        End If
    End Sub

    Private Shared Function bildeGemarkungstextTextFuerSchlagworte(flst As clsFlurstueck) As String
        Dim gemarkungstext As String = LIBgemeinsames.clsString.umlaut2ue(flst.gemarkungstext)
        gemarkungstext = gemarkungstext.Replace("-", "_")
        gemarkungstext = LIBgemeinsames.clsString.normalize_Filename(gemarkungstext)
        Return gemarkungstext
    End Function

    Private Sub btnAlsDokument_Click(sender As Object, e As RoutedEventArgs)
        btnAlsDokument_ClickExtracted()
        Close()
        e.Handled = True
    End Sub

    Private Function getfsFrom(info As String) As String
        Dim a As String()
        Dim temp As String
        Try
            a = info.Split("("c)
            temp = a(1)
            a = temp.Split(","c)
            temp = a(0).Replace("'", "")
            Return temp
        Catch ex As Exception
            Return "_"
        End Try
    End Function

End Class
