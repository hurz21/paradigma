Public Class winTXT
    Property _text As String
    Property _aktdoku As Dokument
    Property _modus As String = "edit"
    Property _docid As Integer = 0
    Private Sub txtAbbruch_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub
    Sub New(text As String, aktdoku As Dokument, modus As String, docid As Integer)
        InitializeComponent()
        _text = text
        _aktdoku = CType(aktdoku.Clone, Dokument)
        _modus = modus
        _docid = docid
    End Sub
    Private Sub winTXT_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        tbDateiInhalt.Text = _text
        bildeTitel()
        txtSpeichern.IsEnabled = False
        If _modus = "edit" Then
            cbEreignisAnlegen.Visibility = Visibility.Collapsed
            tbEreignisTitel.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub bildeTitel()
        Title = "Textdatei: " & _aktdoku.DateinameMitExtension
        txtSpeichern.IsEnabled = False
    End Sub

    Private Sub txtSpeichern_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim neuerText As String
        'text = detailsTools.getTextINhalt(aktdoku.makeFullname_ImArchiv(myglobalz.Arc.rootDir))
        If tbDateiInhalt.Text.IsNothingOrEmpty Then
            neuerText = ""
        End If
        neuerText = tbDateiInhalt.Text.Trim
        Dim erfolg As Boolean
        erfolg = detailsTools.schreibeTextDateiInsArchiv(neuerText, _aktdoku)
        'filedatum wird in db nicht angepasst
        'braucht ausser mir sowieso keiner
        Close()
        If _modus = "neu" Then
            If cbEreignisAnlegen.IsChecked Then

                'MsgBox("")
                Dim neuereignis As New clsEreignis
                initEreignis(neuereignis, tbEreignisTitel.Text, "Textnotiz",
                             Now, "")
                clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)
                myGlobalz.sitzung.aktDokument.DocID = _docid
                detailsTools.DokumentDemEreignisHinzufuegen(neuereignis.ID, myGlobalz.sitzung.aktDokument)
                CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
                    myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
            End If
        End If
    End Sub
    Private Shared Sub initEreignis(ByVal neuereignis As clsEreignis,
                                    Beschreibung As String,
                                    art As String,
                                    daite As Date,
                                    richtung As String)
        With neuereignis
            .Art = art
            .Beschreibung = Beschreibung
            .Datum = daite
            .istRTF = False
            .typnr = 5
            .Notiz = ""
            .DokumentID = 0
            .Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
            .Richtung = richtung
        End With
    End Sub
    Private Sub txtclear_Click(sender As Object, e As RoutedEventArgs)
        tbDateiInhalt.Text = ""
        e.Handled = True
    End Sub

    Private Sub txturzustand_Click(sender As Object, e As RoutedEventArgs)
        tbDateiInhalt.Text = _text
        e.Handled = True
    End Sub

    Private Sub tbDateiInhalt_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbDateiInhalt.TextChanged
        txtSpeichern.IsEnabled = True
        e.Handled = True
    End Sub

    Private Sub txtbigger_Click(sender As Object, e As RoutedEventArgs)
        tbDateiInhalt.FontSize = tbDateiInhalt.FontSize + 2
        e.Handled = True
    End Sub

    Private Sub txtsmaller_Click(sender As Object, e As RoutedEventArgs)
        tbDateiInhalt.FontSize = tbDateiInhalt.FontSize - 2
        e.Handled = True
    End Sub
End Class
