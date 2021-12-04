Partial Public Class WINvorgangNeu2

    Private Sub WINvorgangNeu2_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        schliessenFormular()
        e.Cancel = True
    End Sub

    Private Sub WINvorgangNeu2_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        glob3.allAktobjReset.execute(myGlobalz.sitzung)
        tbSachgebiet.Text = ""
        tbBeschreibung_nummer.Text = ""
        tbSachgebiet.IsReadOnly = False
        tbBeschreibung_nummer.IsReadOnly = False
        myGlobalz.sitzung.aktVorgang.Stammdaten.az.stamm = "II-67"
        clsBaumbilden.ladeXML(myGlobalz.Paradigma_Sachgebietsdatei$, TreeView1)
        btnWeiter.IsEnabled = False
    End Sub

    Function BitteTitelEingeben(ByVal titel as string) as  String
        If String.IsNullOrEmpty(titel$) Then
            Return myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header
        Else
            Return titel
        End If
    End Function

    Private Function weiterWennOk(ByVal stammbeschreibung$, ByVal beschreibungNummer as string) as  Boolean 'tbStammbeschreibung.Text, tbBeschreibung_nummer.Text
        Dim titel$ = BitteTitelEingeben(stammbeschreibung)
        btnWeiter.IsEnabled = False
        If Not istEingabeok() Then
            nachricht_und_Mbox("  Eingabe war so nicht in Ordnung.")
        Else
            nachricht("Eingabe ist ok")
            myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum = Now
            myGlobalz.sitzung.aktVorgang.Stammdaten.Aufnahmedatum = Now
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa = beschreibungNummer        ' weil binding beim zweiten mal nicht funzt!!!
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt = tbSumme.Text
            If clsBaumbilden.AZistNeu() Then
                vorgangsnummer_ggf_Korrigieren()
                myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale = myGlobalz.sitzung.aktBearbeiter.Initiale
                myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = titel    

                glob2.NEU_VorgangStamm_2DBOk()  'abspeichern in der db
                myGlobalz.sitzung.modus = "edit"
                Return True
            Else
                nachricht_und_Mbox("Dieses Aktenzeichen " & vbCrLf &
                       myGlobalz.sitzung.aktVorgang.Stammdaten.az.AZ_concat_stamm() & vbCrLf &
                       "ist bereits vergeben worden!" & vbCrLf & _
                       "Bitte prüfen Sie v.a. die Vorgangsnummer/Beschreibung.")
            End If
        End If
        Return False
    End Function

    Sub vorgangsnummer_ggf_Korrigieren()
        Try
            Dim a% = myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer
            Dim b$ = myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa
            Dim test$()
            If b Is Nothing Then
                nachricht("Fehler: vorgangsbeschreibung ist NOTHING")
                Exit Sub
            End If
            If a < 1 Then
                nachricht("Fehler: Vorgangsnummer ist <1")
                Exit Sub
            End If
            If Not b.Contains("-") Then
                nachricht("vorgangsnummer_ggf_Korrigieren: ist nicht numerisch ohne -")
                Exit Sub
            End If


            test = b.Split("-"c)
            If test(0) <> a.ToString And IsNumeric(test(0)) And IsNumeric(test(1)) Then
                myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer = CInt(test(0))
                nachricht("vorgangsnummer_ggf_Korrigieren : Korrektur wurde durch geführt.")
            Else
                nachricht("vorgangsnummer_ggf_Korrigieren : Korrektur wurde NICHT durch geführt.")
            End If

        Catch ex As Exception
            nachricht_und_Mbox("Fehler: invorgangsnummer_ggf_Korrigieren. " & ex.ToString)
        End Try
    End Sub

    Function istEingabeok() As Boolean
        nachricht(" vorgang neu. istEingabeok")
        If String.IsNullOrEmpty((myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer.ToString)) Then
            MessageBox.Show("Dieses Aktenzeichen ist unvollständig!" & vbCrLf & _
             "Bitte prüfen Sie die Vorgangsnummer bzw. Ortsbeschreibung.", _
             "Neues Aktenzeichen: Problem", _
             MessageBoxButton.OK, _
             MessageBoxImage.Error)
            tbBeschreibung_nummer.Focus()
            Return False
        End If
        If String.IsNullOrEmpty((myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)) Then
            MessageBox.Show("Dieses Aktenzeichen ist unvollständig!" & vbCrLf & _
             "Bitte prüfen Sie das Sachgebiet.", _
             "Neues Aktenzeichen: Problem", _
             MessageBoxButton.OK, _
             MessageBoxImage.Error)
            Return False
        End If
        Return True
    End Function

    

    Sub TVgetclick()
        Try
            If TreeView1.SelectedItem Is Nothing Then Exit Sub
            tbSumme.Text = ""
            Dim tv As New TreeViewItem
            tv = CType(TreeView1.SelectedItem, TreeViewItem)
            If Not tv.Tag Is Nothing Then
                myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl = tv.Tag.ToString
                myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header = glob2.klammerraus(tv.Header.ToString)
                setLaufendeNummer()
                If myGlobalz.sitzung.aktBearbeiter.Bemerkung.ToLower.StartsWith("untere wasser") Then
                    tbBeschreibung_nummer.Text = "Xxx-" & myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer & " "
                    laufNReinbez.Visibility = Windows.Visibility.Collapsed
                Else
                    tbBeschreibung_nummer.Text = Now.Year.ToString
                    laufNReinbez.Visibility = Windows.Visibility.Visible
                End If
                myGlobalz.sitzung.aktVorgang.Stammdaten.az.AZ_concat(myGlobalz.sitzung.aktBearbeiter.Initiale)
                bildeNeueAzSumme(CBool(laufNReinbez.IsChecked), myGlobalz.sitzung.aktVorgang.Stammdaten) 'sonst kommt er mit den laufnr durcheinander
                btnWeiter.IsEnabled = True
                tv.IsSelected = False
                tv = Nothing
            End If
        Catch ex As Exception
            nachricht_und_Mbox("getclick in w aktenzeichen: " & vbCrLf & ex.ToString)
        End Try
    End Sub

    Shared Sub setLaufendeNummer()
        Dim test%
        If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsVorgangDB_Oracle(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
            test% = zzz.holeNeueVorgangsNummer(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
               zzz.dispose
        End If
        If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsVorgangDB_Oracle(clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
            test% = zzz.holeNeueVorgangsNummer(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
               zzz.dispose
        End If

        If test > 0 Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer = (CInt(test))
        End If
  

    End Sub

    Private Sub tbBeschreibung_nummer_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung_nummer.TextChanged
        glob2.istTextzulang(240, tbBeschreibung_nummer)
        btnWeiter.IsEnabled = True 
        bildeNeueAzSumme(CBool(laufNReinbez.IsChecked), myGlobalz.sitzung.aktVorgang.Stammdaten)
    End Sub
    Private Sub tbSachgebiet_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbSachgebiet.TextChanged
        btnWeiter.IsEnabled = True
        bildeNeueAzSumme(CBool(laufNReinbez.IsChecked), myGlobalz.sitzung.aktVorgang.Stammdaten)
    End Sub
    Private Sub schliessenFormular()
        Me.Hide()
        tbBeschreibung_nummer.Text = ""
        btnWeiter.IsEnabled = False
    End Sub
    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        schliessenFormular()
    End Sub



    Private Sub btnWeiter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If weiterWennOk(tbStammbeschreibung.Text, tbBeschreibung_nummer.Text) Then
            schliessenFormular()
            e.Handled = True
            glob2.holeDetailform(myGlobalz.sitzung.aktVorgangsID)
        End If
    End Sub

    Private Sub btnSuche_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSuche.Click
        trefferliste.Items.Clear()
        clsBaumbilden.sucheStichwortInXML(myGlobalz.Paradigma_Sachgebietsdatei$, tbSuchwort.Text.ToLower, trefferliste)
    End Sub

    Private Sub TreeView1_SelectedItemChanged(ByVal sender As System.Object, ByVal e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Object)) Handles TreeView1.SelectedItemChanged
        TVgetclick()
        e.Handled = True
    End Sub

    Sub bildeNeueAzSumme(ByVal mitlaufenderNr As Boolean, ByVal vgang As LIBstammdatenCRUD.Stamm)
        Dim d$ = "-", a$ = ""
        'If Not mitlaufenderNr Then
        '    tbSumme.Text = tbstamm.Text & d$ &
        '                    tbSachgebiet.Text & d$ &
        '                    tbBeschreibung_nummer.Text & d$ &
        '                    myGlobalz.sitzung.Bearbeiter.Kuerzel2stellig
        'Else
        '    tbSumme.Text = tbstamm.Text & d$ &
        '                    tbSachgebiet.Text & d$ &
        '                    tbVorgangsnummer.Text & d$ &
        '                    tbBeschreibung_nummer.Text & d$ &
        '                    myGlobalz.sitzung.Bearbeiter.Kuerzel2stellig
        'End If



        If Not mitlaufenderNr Then
            a$ = vgang.az.stamm & d$ &
            vgang.az.sachgebiet.Zahl & d$ &
            vgang.az.Prosa & d$ &
           myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig
        Else
            a$ = vgang.az.stamm & d$ &
            vgang.az.sachgebiet.Zahl & d$ &
            vgang.az.Vorgangsnummer & d$ &
            vgang.az.Prosa & d$ &
           myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig
        End If

        tbSumme.Text = a$
    End Sub

    Private Sub SucheNachSachgebieten_MouseLeftButtonDown(ByVal sender As System.Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles SucheNachSachgebieten.MouseLeftButtonDown
        btnSuche.IsDefault = True
    End Sub

    Private Sub laufNReinbez_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles laufNReinbez.Click
        bildeNeueAzSumme(CBool(laufNReinbez.IsChecked), myGlobalz.sitzung.aktVorgang.Stammdaten)
    End Sub

    Private Sub tbStammbeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbStammbeschreibung.TextChanged
        btnWeiter.IsEnabled = True

    End Sub

    Private Sub tbVorgangsnummer_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbVorgangsnummer.TextChanged

    End Sub


End Class