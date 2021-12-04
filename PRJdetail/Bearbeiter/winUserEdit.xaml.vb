Public Class winUserEdit
    Public Property ladevorgangabgeschlossen As Boolean = False
    Private Sub winUserEdit_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tbEmail.Text = myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email
        tbTelefon.Text = myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Telefon1
        tbFax.Text = myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Fax1
        tbRaum.Text = myGlobalz.sitzung.aktBearbeiter.Raum
        tbKuerzel.Text = myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig
        e.Handled = True
        ladevorgangabgeschlossen = True
    End Sub
    Sub New()
        InitializeComponent()
    End Sub


    Private Sub btnsave_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email = tbEmail.Text
        myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Telefon1 = tbTelefon.Text
        myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Fax1 = tbFax.Text
        myGlobalz.sitzung.aktBearbeiter.Raum = tbRaum.Text
        myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig = tbKuerzel.Text
        If userEditHelper.save(myGlobalz.sitzung.aktBearbeiter) = 1 Then
            Close()
        End If
    End Sub

    Private Sub btnabbruch_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
    End Sub

    Private Sub tbTelefon_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbTelefon.TextChanged
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        btnsave.IsEnabled = True
    End Sub


    Private Sub tbFax_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbFax.TextChanged
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        btnsave.IsEnabled = True
    End Sub

    Private Sub tbRaum_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbRaum.TextChanged
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        btnsave.IsEnabled = True
    End Sub

    Private Sub tbEmail_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbEmail.TextChanged
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        btnsave.IsEnabled = True
    End Sub

    Private Sub tbKuerzel_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbKuerzel.TextChanged
        e.Handled = True
        If Not ladevorgangabgeschlossen Then Exit Sub
        btnsave.IsEnabled = True
    End Sub
End Class
