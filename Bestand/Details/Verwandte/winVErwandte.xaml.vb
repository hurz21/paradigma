Public Class winVErwandte
    'Private verwandteID%, vertext$, kopplungsid%
    Property returncode As String = ""
    Private _vertext As String
    Public Property vertext() As String
        Get
            Return _vertext
        End Get
        Set(ByVal value As String)
            _vertext = value
        End Set
    End Property

    Private _verwandteID As Integer
    Public Property verwandteID() As Integer
        Get
            Return _verwandteID
        End Get
        Set(ByVal value As Integer)
            _verwandteID = value
        End Set
    End Property

    Private _kopplungsid As Integer
    Public Property kopplungsid() As Integer
        Get
            Return _kopplungsid
        End Get
        Set(ByVal value As Integer)
            _kopplungsid = value
        End Set
    End Property

    Sub New(ByVal _kopplungsid As Integer, ByVal _verwandteID As Integer, ByVal _vertext As String)
        InitializeComponent()
        verwandteID = _verwandteID
        vertext = _vertext
        kopplungsid = _kopplungsid
    End Sub

    Private Sub btnZUVerwandtemWechseln_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        glob3.allAktobjReset.execute(myGlobalz.sitzung)
        'lockfile löschen
        detailsTools.VorgangLocking("aus")
        myGlobalz.sitzung.aktVorgangsID = verwandteID%
        DialogResult = True
        returncode = "wechseln"
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnLoeschenVerwandten_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim erfolg As Integer = VerwandteTools.Verwandten_loeschen.exe(kopplungsid%)
        If erfolg > 0 Then
            DialogResult = False
            ' returncode = 1 'wechseln zum verw
            returncode = "loeschen" '2 'löschen des verwandten
        End If
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = False
        ' returncode = 1 'wechseln zum verw
        '  returncode = 2 'löschen des verwandten
        returncode = "abbruch" ' keine verwantenaktion
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub winVErwandte_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        ' tbVerwandtenVID.Text = verwandteID.ToString
        tbVerwandtenAz.Text = verwandteID.ToString & ": " & vertext
        Title = StammToolsNs.setWindowTitel.exe("edit", "Verwandte")

        ' Dim aaa = myGlobalz.sitzung.aktVorgang.Stammdaten.az

        e.Handled = True
    End Sub

    Private Sub btnKopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim quellVorgangsid As Integer = verwandteID
        Dim zielVorgangsid As Integer = myGlobalz.sitzung.aktVorgangsID
        Dim dokumenteAuchKopieren As Boolean = GetDokumenteAuchKopieren()

        'Dim aaa = myGlobalz.sitzung.aktVorgang.Stammdaten.az
        Dim info As String = " Der Kopiervorgang kann etwas dauern. Am Ende wird dieser Dialog geschlossen. " & vbCrLf &
                        " Bitte haben Sie Geduld." & " Die Daten werden kopiert." & " Doppelte Einträge werden vermieden." & vbCrLf &
                        " " & vbCrLf
        tbfortschritt.Text = info
        tbfortschritt.Text &= vbCrLf & "Beteiligte kopieren ...."
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        detailsTools.AlleBeteiligtenKopieren(quellVorgangsid, myGlobalz.sitzung.aktVorgangsID)

        tbfortschritt.Text &= vbCrLf & "Raumbezüge kopieren ...."
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        If RBtoolsns.KopierenVonRaumbezuegen_alleDB.exe(quellVorgangsid, myGlobalz.sitzung.aktVorgangsID) Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
            ' Dim erfolg As Boolean = VSTTools.editStammdaten_alleDB.exe(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten)
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
        End If
        Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents

        tbfortschritt.Text &= vbCrLf & "Dokumente kopieren ....(plus Ereignisse mit Dokumenten)"
        If dokumenteAuchKopieren Then
            detailsTools.AlleDokumentenKopieren(quellVorgangsid, myGlobalz.sitzung.aktVorgangsID)
            Dispatcher.Invoke(Windows.Threading.DispatcherPriority.Background, Function() 0) 'Doevents
        End If
        Close()
        returncode = "kopieren"
        e.Handled = True
    End Sub
    Private Function GetDokumenteAuchKopieren() As Boolean
        Dim dokumenteAuchKopieren As Boolean
        dokumenteAuchKopieren = dokusauchkopieren()
        Return dokumenteAuchKopieren
    End Function
    Private Function dokusauchkopieren() As Boolean
        Dim dokumenteAuchKopieren As Boolean
        If chkMitDokumentenKoperen.IsChecked Then
            dokumenteAuchKopieren = True
        Else
            dokumenteAuchKopieren = False
        End If
        Return dokumenteAuchKopieren
    End Function

    Private Sub btnDokumenteeinsehen_Click_1(sender As Object, e As RoutedEventArgs)
        Close()
        Dim verwDok As New verwandteDokumente(verwandteID)
        verwDok.ShowDialog()
        e.Handled = True
    End Sub
End Class
