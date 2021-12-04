

Partial Public Class WINBox
    Public Property dokumentWurdeGeoeffnet As Boolean = False
    Shared Property DateitypeText As String
    Private _quelle$ = ""

    Sub New(ByVal quelle$)
        InitializeComponent()
        _quelle = quelle
    End Sub

    Public Property knopfnummer As Integer

        Private Sub WINBox_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Title = myGlobalz.sitzung.aktDokument.DateinameMitExtension
        myimagerefresh.DataContext = GetPicturePath()
        tbFilename.Text = GetText()
        'wbDokPreview.Navigate("C:\Users\Feinen_j\Desktop\Paradigma\Archiv_Checkout\24462\R0034210.JPG")
        'wbDokPreview.
        ''    wbDokPreview.Navigate("C:\Users\Feinen_j\Desktop\Paradigma\Archiv_Checkout\24570\rodgau - Google Maps 01.pdf")
        'wbDokPreview.Navigate(" C:\Users\Feinen_j\Downloads\350_716_1.doc")
       
        e.Handled = True
    End Sub

    Private Sub btnOffnen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnOffnen.Click
        e.Handled = True
        knopfnummer = 1
        Me.Close()
    End Sub

    Private Sub btnBeschreibung_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBeschreibung.Click
        e.Handled = True
        knopfnummer = 2
        Me.Close()
    End Sub



    Private Sub btnMailen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnMailen.Click
        e.Handled = True
        knopfnummer = 4
        Me.Close()
        '  Archiv_DokumentMailen()
    End Sub

    Private Sub btnnachOkopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnnachOkopieren.Click
        knopfnummer = 5
        '  NachZielKopieren("o:")
        Me.Close()
    End Sub

    Private Sub btnnachCkopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnnachCkopieren.Click
        Me.Close()
        knopfnummer = 6
        '  NachZielKopieren(System.Environment.SpecialFolder.MyDocuments.ToString)
    End Sub

    'Private Sub btnnachPkopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    MsgBox("Baustelle")
    'End Sub


    Private Sub Window_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing

    End Sub

    'Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    e.Handled = True
    '    knopfnummer = 3
    '    Me.Close()
    'End Sub


    Private Sub btnLoeschen_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnLoeschen.Click
        e.Handled = True
        knopfnummer = 3
        Me.Close()
    End Sub

    Private Shared Function GetPicturePath() As String
        'JPG = 1
        'PDF = 2
        'DOC = 3
        'RTF = 4
        'TXT = 5
        'ZIP = 6
        'DGN = 7
        'XLS = 8
        'VCF = 9
        'GIF = 10
        'MSG = 11
        'GA = 12 ' 3ga
        'AVI = 13
        'MPG = 14
        'UNKNOWN = 99

        Dim path As String = ""
        path = "/paradigma;component/details/icons/"
        Select Case CInt(myGlobalz.sitzung.aktDokument.DokTyp)
            Case 1
                path = path & "foto.jpg"
                DateitypeText = "Foto"
            Case 2
                path = path & "pdf.jpg"
                DateitypeText = "Portable Document Format"
            Case 3
                path = path & "doc.jpg"
                DateitypeText = "Textverarbeitungsformat"
            Case 4
                path = path & "rtf.jpg"
                DateitypeText = "Textverarbeitungsformat"
            Case 5
                path = path & "doc.jpg"
                DateitypeText = "Textverarbeitungsformat"
            Case 6
                path = path & "zip.jpg"
                DateitypeText = "Dateienpaket"
            Case 7
                path = path & "unbekannt.jpg"
            Case 8
                path = path & "xls.jpg"
                DateitypeText = "Tabellenkalkulationsformat"
            Case 9
                path = path & "vcf.jpg"
                DateitypeText = "Elektronische Visitenkarte"
            Case 10
                path = path & "foto.jpg"
                DateitypeText = "Foto / Grafik"
            Case 11
                path = path & "outlook.jpg"
                DateitypeText = "eMail"
            Case 12
                path = path & "microfon.jpg"
                DateitypeText = "Audiodatei"
            Case 13
                path = path & "avi.jpg"
                DateitypeText = "Videodatei"
            Case 14
                path = path & "avi.jpg"
                DateitypeText = "Videodatei"
            Case 15
                path = path & "xls.jpg"
                DateitypeText = "Tabellenkalkulationsformat"
        End Select
        Return path
    End Function
    Private Shared Function GetText() As String
        Return vbCrLf & "Typ des Dokumentes: " & DateitypeText & vbCrLf & vbCrLf &
                  myGlobalz.sitzung.aktDokument.DateinameMitExtension & vbCrLf &
                "Aufnahme ins Archiv: " & myGlobalz.sitzung.aktDokument.Checkindatum & vbCrLf &
                "Schlagworte: " & myGlobalz.sitzung.aktDokument.Beschreibung & vbCrLf &
                "Revisionssicher: " & GetTextExtracted() & vbCrLf &
                "Quelle: " & myGlobalz.sitzung.aktDokument.Initiale
    End Function
    Private Shared Function GetTextExtracted() As String
        If myGlobalz.sitzung.aktDokument.revisionssicher Then
            Return "ja"
        Else
            Return "nein"
        End If

    End Function


    Private Sub btnMailenAnBa_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        knopfnummer = 7
        Me.Close()
    End Sub

    Private Sub btnimArchivkopieren_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 8
        Me.Close()
    End Sub

    Private Sub btnsofortdruck_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        knopfnummer = 9
        Me.Close()
    End Sub

Private Sub btnRevisionssichern_Click(sender As Object , e As RoutedEventArgs)
         knopfnummer = 10
           e.Handled = True
           Me.Close()
    End Sub

'Private Sub wbMousedown(sender As Object , e As MouseButtonEventArgs)
'        MsgBox("md")
'e.Handled=true
'    End Sub
End Class
