Public Class NaturegDetail
    Property aktNatureg As New clsNatureg

    Public _modus As String
    Private Sub gastLayout()
        Background = myglobalz.GetSecondBackground()
        grd1.Background = myglobalz.GetSecondBackground()
    End Sub

    Private Sub copyNaturegObject(alt As clsNatureg, neu As clsNatureg)
        With neu
            .ID = alt.ID
            .VorgangsID = alt.VorgangsID
            .notiz = alt.notiz
            .nummer = alt.nummer
            .Quelle = alt.Quelle
            .art = alt.art
            .typ = alt.typ
            .beschreibung = alt.beschreibung
            .timestamp = alt.timestamp
            .MassnahmenNr = alt.MassnahmenNr
        End With
    End Sub

    Sub New(modus As String, _aktNatureg As clsNatureg)
        InitializeComponent()
        _modus = modus
        copyNaturegObject(_aktNatureg, aktNatureg)
    End Sub
    Private Sub btnspeichern_Click_1(sender As Object, e As RoutedEventArgs)
        Dim erfolg As Boolean
        If eingabenOK() Then
            erfolg = naturegTools.speichernNatreg(aktNatureg, _modus, myglobalz.sitzung.aktVorgangsID)
        Else
            Exit Sub
        End If
        If erfolg Then
            Close()
        Else
        End If
        e.Handled = True
    End Sub

    Private Sub btnabbruch_Click_1(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub cmbTyp_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbTyp.SelectionChanged

        Dim item As System.Windows.Controls.ComboBoxItem = CType(cmbTyp.SelectedItem, System.Windows.Controls.ComboBoxItem)
        If String.IsNullOrEmpty(item.Content.ToString.Trim) Then Exit Sub
        txtTyp.Text = CStr(item.Content.ToString.Trim)
        e.Handled = True
    End Sub

    Private Sub txtTyp_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtTyp.TextChanged

    End Sub



    Private Sub NaturegDetail_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If aktNatureg.typ = "O" Then aktNatureg.typ = "Ökokonto"
        If aktNatureg.typ = "F" Then aktNatureg.typ = "Förderfläche"
        If aktNatureg.typ = "K" Then aktNatureg.typ = "Kompensation"
        If aktNatureg.art = "M" Then aktNatureg.art = "Maßnahme"
        If aktNatureg.art = "V" Then aktNatureg.art = "Verfahren"
        If _modus = "edit" Then
            btnloeschen.IsEnabled = True
        Else
            btnloeschen.IsEnabled = False
        End If
        grd1.DataContext = aktNatureg
        gastLayout()
        e.Handled = True
    End Sub

    Private Function eingabenOK() As Boolean
        If String.IsNullOrEmpty(aktNatureg.MassnahmenNr) Then
            MsgBox("Sie müssen die MassnahmenNr des NaturegObjektes angeben!")
            Return False
        End If
        If String.IsNullOrEmpty(aktNatureg.typ) Then
            MsgBox("Sie müssen den Typ des NaturegObjektes angeben!")
            Return False
        End If
        If String.IsNullOrEmpty(aktNatureg.nummer) Then
            MsgBox("Sie müssen die VerfahrensNummer des NaturegObjektes angeben!")
            Return False
        End If
        If String.IsNullOrEmpty(aktNatureg.beschreibung) Then aktNatureg.beschreibung = ""
        If String.IsNullOrEmpty(aktNatureg.notiz) Then aktNatureg.notiz = ""
        If String.IsNullOrEmpty(aktNatureg.Quelle) Then aktNatureg.Quelle = myglobalz.sitzung.aktBearbeiter.Initiale
        aktNatureg.art = "M"
        If aktNatureg.art = "Maßnahme" Then aktNatureg.art = "M"
        If aktNatureg.art = "Verfahren" Then aktNatureg.art = "V"
        If aktNatureg.typ = "Ökokonto" Then aktNatureg.typ = "O"
        If aktNatureg.typ = "Kompensation" Then aktNatureg.typ = "K"
        If aktNatureg.typ = "Förderfläche" Then aktNatureg.typ = "F"
        Return True
    End Function



    Private Sub btnloeschen_Click_1(sender As Object, e As RoutedEventArgs)
        Dim messi As New MessageBoxResult
        messi = MessageBox.Show(String.Format("NaturegObjekt wirklich löschen ?{0}{1}", vbCrLf, aktNatureg.ID & ", " & aktNatureg.nummer),
                                "NaturegObjekt löschen ?",
                                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
        If messi = MessageBoxResult.Yes Then
            naturegTools.loeschenNatreg(aktNatureg)
            'naturegOracle.Natureg_loeschen(myGlobalz.sitzung.aktPerson.PersonenID, myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.aktPerson.Status)
            'btn.IsEnabled = False
        End If
        Me.Close()
        e.Handled = True
    End Sub
End Class
