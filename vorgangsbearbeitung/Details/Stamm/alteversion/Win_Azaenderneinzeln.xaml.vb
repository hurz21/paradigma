Public Class Win_Azaenderneinzeln

    Dim gestartet% = 0


    Dim d$ = "-"

    Private Sub tbstamm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles tbstamm.Loaded
        'btnSpeichern.IsEnabled = False
        'tbstamm.Text = "II-67"
        'myglobalz.sitzung.aktVorgang.Stammdaten.az.stamm = "II-67"       'btnSpeichern.IsEnabled = False
        tbstamm.Text = "7"
        myglobalz.sitzung.aktVorgang.Stammdaten.az.stamm = "67"
        If myGlobalz.zuhause Then
            tbstamm.Text = ""
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.stamm = ""
        End If
        refreshAZ(CBool(laufNReinbez.IsChecked), myGlobalz.sitzung.aktVorgang.Stammdaten)
    End Sub
    'Private Sub zzzz()
    '    Try
    '        If gestartet <> 2 Then Exit Sub
    '        refreshAZ(CBool(laufNReinbez.IsChecked), myGlobalz.sitzung.aktVorgang.Stammdaten)
    '        btnSpeichern.IsEnabled = True
    '    Catch ex As Exception
    '        nachricht("FEhler in zzzz :" & ex.ToString)
    '    End Try
    'End Sub
    Private Sub tbstamm_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbstamm.TextChanged
        'zzzz()
    End Sub
    Sub refreshAZ(ByVal mitlaufenderNr As Boolean, ByVal vgang As Stamm) 'laufNReinbez.IsChecked
        Dim a$ = ""
        Try
            If Not mitlaufenderNr Then
                a$ = vgang.az.stamm & d$ &
                vgang.az.sachgebiet.Zahl & d$ &
                vgang.az.Prosa & d$ &
                vgang.hauptBearbeiter.Initiale & d$
            Else
                a$ = vgang.az.stamm & d$ &
                vgang.az.sachgebiet.Zahl & d$ &
                vgang.az.Vorgangsnummer & d$ &
                vgang.az.Prosa & d$ &
                vgang.hauptBearbeiter.Initiale & d$
            End If
        Catch ex As Exception
            a$ = vgang.az.stamm & d$ &
                vgang.az.sachgebiet.Zahl & d$ &
                vgang.az.Prosa & d$ &
                vgang.hauptBearbeiter.Initiale & d$
        Finally
            If a$.EndsWith("-") Then a = a.Substring(0, a.Length - 1)
            vgang.az.gesamt = a$
        End Try
    End Sub

    Private Sub tbSachgebiet_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbSachgebiet.TextChanged
        'zzzz()
    End Sub

    Private Sub tbBeschreibung_nummer_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung_nummer.TextChanged
        'zzzz()
    End Sub

    Private Sub tbBearbeiter_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBearbeiter.TextChanged
        'zzzz()
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        Me.Close()
    End Sub

    'Private Sub speichern(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    Stammdatenspeichern()
    'End Sub
    'Private Sub Stammdatenspeichern()
    '    Me.Close()
    '    If Not glob2.EDIT_VorgangStamm_2DBOk() Then
    '        Exit Sub
    '    End If

    'End Sub

    Private Sub laufNReinbez_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles laufNReinbez.Click
        'zzzz()  
    End Sub

    Private Sub tbVorgangsnummer_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVorgangsnummer.TextChanged
        'zzzz()
    End Sub


    Private Sub Win_Azaenderneinzeln_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        gestartet = 2
    End Sub
End Class
