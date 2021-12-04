Partial Public Class winlinken
    Shared Property sollRefreshen As Boolean = False
    Shared Property bisher As String = ", bisher: "
    'Private Sub btKopieren_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    If Not pruefeEingabeVerwandeterOK() Then Exit Sub
    'End Sub

    Private Function pruefeEingabeVerwandeterOK() As Boolean
        If String.IsNullOrEmpty(tbVid.Text) Then Return False
        If String.IsNullOrEmpty(tbVid.Text) Then
            MessageBox.Show("Sie haben keine VorgangsNr. eingegeben. Abbruch.")
            Return False
        End If
        If Not IsNumeric(tbVid.Text) Then
            MessageBox.Show("Sie haben keine gültige VorgangsNr. eingegeben. Abbruch.")
            Return False
        End If
        Return True
    End Function

    Private Sub verwandtenAnlegen()
        If Not pruefeEingabeVerwandeterOK() Then Exit Sub
        Dim fremdVID% = CInt(tbVid.Text)
        Dim fremdstamm As New Stamm(CLstart.mycsimple.MeinNULLDatumAlsDate)
        Dim fremdvorgangREC As IDB_grundfunktionen
        fremdvorgangREC = CType(myGlobalz.sitzung.VorgangREC, IDB_grundfunktionen)
        Dim erfolg As Boolean = clsVorgangCTRL.leseVorgangvonDBaufObjekt(fremdVID%, fremdstamm, fremdvorgangREC)
        If Not erfolg Then Exit Sub
        'koppeln
        Dim titel$ = String.Format("{0} {1}", fremdstamm.az.gesamt, fremdstamm.Beschreibung)
        'VerwandteTools.Koppelung_Vorgang_Fremdvorgang.exe(myGlobalz.sitzung.aktVorgangsID, fremdVID, titel)
        sollRefreshen = True
        bisher &= fremdVID & ","
        setTitle()
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

    Private Sub winlinken_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        btLinken.IsEnabled = False
        '   btKopieren.IsEnabled = False
        FocusManager.SetFocusedElement(Me, tbVid)
        setTitle()
        e.Handled = True
    End Sub

    Private Sub tbVid_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        If Not String.IsNullOrEmpty(tbVid.Text) Then
            btLinken.IsEnabled = True
            '   btKopieren.IsEnabled = True
        End If
        e.Handled = True
    End Sub

    Private Sub setTitle()
        Title = "Verwandten Vorgang hinzufügen" & bisher
    End Sub

    Private Sub btnBestandsauswahl_Click_1(sender As Object, e As RoutedEventArgs)
        clsStartup.initBestandsSQL()
        Dim wz As New winStammBestand(False, True)
        wz.ShowDialog()
        GC.Collect()
        tbVid.Text = CStr(myGlobalz.sitzung.BestandsAuswahlVID)
        e.Handled = True
    End Sub
End Class
