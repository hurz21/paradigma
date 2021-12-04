Partial Public Class winlinken
    Public Property istConject As Boolean = False
    Shared Property sollRefreshen As Boolean = False
    Shared Property bisher As String = ", bisher: "
    Private vorschlag As String = ""
    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
    End Sub
    Sub New(_vorschlag As String)
        InitializeComponent()
        vorschlag = _vorschlag
    End Sub

    Private Function pruefeEingabeVerwandeterOK() As Boolean
        If String.IsNullOrEmpty(tbVid.Text) Then
            MessageBox.Show("Sie haben keine VorgangsNr. eingegeben. Abbruch.")
            Return False
        End If
        If Not IsNumeric(tbVid.Text.Trim) Then
            MessageBox.Show("Sie haben keine gültige VorgangsNr. eingegeben. Abbruch.")
            Return False
        End If
        Return True
    End Function

    Private Sub winlinken_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        btLinken.IsEnabled = False
        '   btKopieren.IsEnabled = False
        FocusManager.SetFocusedElement(Me, tbVid)
        Title = detailsTools.settitle("Linken mit verwandten Vorgängen")
        gastLayout()
        tbVid.Text = CType(vorschlag, String)
        e.Handled = True
    End Sub
    Private Sub verwandtenAnlegen()
        If Not pruefeEingabeVerwandeterOK() Then Exit Sub
        'verwandtenFuellenUndKoppeln
        Dim fremdVID As Integer = 0
        fremdVID = CInt(tbVid.Text)
        Dim fremdstamm As Stamm
        Dim erfolg As Boolean = Nothing
        erfolg = VerwandteTools.divers.verwandtenFuellenUndKoppeln(fremdVID, fremdstamm, myGlobalz.sitzung.aktVorgangsID)

        If Not erfolg Then Exit Sub
        If fremdstamm.az.gesamt.Contains("-1020-") Then
            istConject = True
        End If
        sollRefreshen = True
        bisher &= fremdVID & ","
        Title = detailsTools.settitle("Verwandten Vorgang hinzufügen" & bisher)
        ' DialogResult = sollRefreshen
    End Sub



    Private Sub btLinken_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        verwandtenAnlegen()
        tbVid.Text = ""
        e.Handled = True
    End Sub

    Private Sub btabbrechen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        DialogResult = sollRefreshen
        Me.Close()
        e.Handled = True
    End Sub


    Private Sub tbVid_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        If Not String.IsNullOrEmpty(tbVid.Text) Then
            btLinken.IsEnabled = True
            '   btKopieren.IsEnabled = True
        End If
        e.Handled = True
    End Sub



    Private Sub btnBestandsauswahl_Click_1(sender As Object, e As RoutedEventArgs)
        'clsStartup.initBestandsSQL()
        'Dim wz As New winBestandStammdaten(False, True)
        'wz.ShowDialog()
        'GC.Collect()
        tbVid.Text = CStr(myGlobalz.sitzung.BestandsAuswahlVID)
        e.Handled = True
    End Sub
End Class
