Public Class WinDS_gruppen
    Property modus As String = "neu"
    Property VIDgruppentext As String = ""
    Property aktVGR As New cls_ds_vorgangsgruppe
    Private Property USERgruppentext As String

    Private Property BearbeiterIDSumme As String

    Private Sub DS_gruppen_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        gastLayout()
        Title = "Datenschutzeinstellung für Vorgang Nr: " & myGlobalz.sitzung.aktVorgangsID
        Dim HauptBearbeiter As String = ""
        Dim erlaubt As Short = DS_Tools.DS_formularZugriffErlaubt(HauptBearbeiter)
        If erlaubt = 0 Then
            Close()
            'MsgBox(glob2.getMsgboxText("keinZugriffAufFormular", HauptBearbeiter), , "Zugriff verweigert")
            ' MsgBox(glob2.getMsgboxText("keinZugriffAufFormular", New List(Of String)(New String() {"one", "two", "three"})), , "Zugriff verweigert")
            MsgBox(glob2.getMsgboxText("keinZugriffAufFormular", New List(Of String)(New String() {HauptBearbeiter})),  , "Zugriff verweigert")
            nachricht("zugriff verweitert ")
            e.Handled = True
        Else
            nachricht("zugriff erlaubt wg. " & erlaubt)
        End If
        aktVGR.vid = myGlobalz.sitzung.aktVorgangsID
        VIDgruppentext = DS_Tools.getgruppentext4Vid(aktVGR, myGlobalz.sitzung.VorgangREC)
        If VIDgruppentext.IsNothingOrEmpty Then
            VIDgruppentext = DS_Tools.getDS_userStandard(myGlobalz.sitzung.aktBearbeiter.ID, myGlobalz.sitzung.VorgangREC)
            If VIDgruppentext.IsNothingOrEmpty Then VIDgruppentext = "1"
        End If
        If ds1Tools.istFachdienstLeitung(myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.aktBearbeiter.username, trenn) Then ckFDASS.IsEnabled = True
        displayGruppenChecks(VIDgruppentext)
        displayGruppenzugehoerigkeit()
        setzeModus()
        e.Handled = True
    End Sub
    Private Sub ckFDU_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    'Private Sub btnSpeichernStandard_Click(sender As Object, e As RoutedEventArgs)
    '    Dim standard_gruppentext As String = gruppentext_bilden()
    '    Dim HauptBearbeiter As String = ""
    '    If DS_Tools.istHauptBearbeiter(HauptBearbeiter) Then
    '        If myglobalz.sitzung.aktBearbeiter.username.ToLower.Trim = Environment.UserName.ToLower.Trim Then
    '            Dim erfolg As Boolean = DS_Tools.userStandardGruppeSpeichern(standard_gruppentext, myglobalz.sitzung.aktBearbeiter.ID, modus)
    '            If erfolg Then MsgBox("Standard wurde gespeichert!")
    '        Else
    '            MsgBox(glob2.getMsgboxText("DS_hauptanwender", New List(Of String)(New String() {HauptBearbeiter})), , "Zugriff verweigert")
    '        End If
    '    Else
    '        MsgBox(glob2.getMsgboxText("DS_hauptanwender", New List(Of String)(New String() {"_"})), , "Zugriff verweigert")
    '    End If
    '    e.Handled = True
    'End Sub

    Private Sub ckSys_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckSchorn_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckOeff_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckAlt_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckBauantrag_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckGIS_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckImm_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckUWB_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub ckUNB_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub


    Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
        'Background = myGlobalz.GetSecondBackground()
    End Sub


    Private Sub btnwhoisinFDU_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "1" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub


    Private Sub btnwhoisinUNB_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "2" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinUWB_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "3" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinImm_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "4" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinGIS_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "5" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinBauantrag_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "6" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinAlt_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "7" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinOeff_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "8" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinSchorn_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "10" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinSYS_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "11" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub


    Private Function gruppentext_bilden() As String
        Dim s As String = ""
        Dim trenn As String = ";"
        If ckFDU.IsChecked Then s = s & "1" & trenn
        If ckUNB.IsChecked Then s = s & "2" & trenn
        If ckUWB.IsChecked Then s = s & "3" & trenn
        If ckImm.IsChecked Then s = s & "4" & trenn
        If ckGIS.IsChecked Then s = s & "5" & trenn
        If ckBauantrag.IsChecked Then s = s & "6" & trenn
        If ckAlt.IsChecked Then s = s & "7" & trenn
        If ckOeff.IsChecked Then s = s & "8" & trenn
        If ckSchorn.IsChecked Then s = s & "10" & trenn
        If ckSys.IsChecked Then s = s & "11" & trenn
        If ckFDASS.IsChecked Then s = s & "13" & trenn
        'If ckIll.IsChecked Then s = s & "12" & trenn
        If s.IsNothingOrEmpty Then
            s = ""
        Else
            s = s.Substring(0, s.Count - 1)
        End If
        Return s
    End Function

    'Private Sub btnwhoisinIll_Click(sender As Object, e As RoutedEventArgs)
    '    Dim gruppennummer As String = "11" : DS_Tools.gruppenzusammensetzung(gruppennummer)
    '    e.Handled = True
    'End Sub

    Private Sub btnSpeichern_Click(sender As Object, e As RoutedEventArgs)
        Dim g1 As String = gruppentext_bilden()
        Dim einzel As String = DS_Tools.einzelstring_bilden(tbEinzelpersonen.Text)
        aktVGR.VIDgruppentext = g1.Trim & einzel.Trim
        Dim erfolg As Boolean = DS_Tools.vorgangsgruppeSpeichern(aktVGR, modus)
        Dim HauptBearbeiter As String = ""
        If Not DS_Tools.istHauptBearbeiter(HauptBearbeiter) Then
            DS_Tools.MailanHauptbearbeiterWgDS_aenderung(myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email, aktVGR, myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter)
        End If
        Close()
        e.Handled = True
    End Sub

    Private Sub ckalleeinaus_Click(sender As Object, e As RoutedEventArgs)
        If ckalleeinaus.IsChecked Then
            alleCheckboxeneinaus("ein")
        Else
            alleCheckboxeneinaus("aus")
        End If
        e.Handled = True
    End Sub

    Private Sub alleCheckboxeneinaus(einaus As String)
        Dim einschalten As Boolean
        If einaus = "ein" Then einschalten = True
        If einaus = "aus" Then einschalten = False
        ckAlt.IsChecked = einschalten
        ckBauantrag.IsChecked = einschalten
        ckFDU.IsChecked = einschalten
        ckGIS.IsChecked = einschalten
        'ckIll.IsChecked = einschalten
        ckImm.IsChecked = einschalten
        ckOeff.IsChecked = einschalten
        ckSchorn.IsChecked = einschalten
        ckSys.IsChecked = einschalten
        ckUNB.IsChecked = einschalten
        ckUWB.IsChecked = einschalten
        ckFDASS.IsChecked = True
    End Sub

    Private Sub ckill_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub displayGruppenChecks(gruppentext As String)
        Dim trenn As String = ";"
        Dim einzelpersonenString As String = ""
        If gruppentext.IsNothingOrEmpty Then
            MsgBox("bitte die Datenschutzeinstellungen speichern")
            Exit Sub
        End If
        Dim a As String() = gruppentext.Split(CChar(trenn))
        alleCheckboxeneinaus("aus")
        'die checks setzen
        For i = 0 To a.GetUpperBound(0)
            If IsNumeric(a(i)) Then
                If a(i) = "1" Then ckFDU.IsChecked = True
                If a(i) = "2" Then ckUNB.IsChecked = True
                If a(i) = "3" Then ckUWB.IsChecked = True
                If a(i) = "4" Then ckImm.IsChecked = True
                If a(i) = "5" Then ckGIS.IsChecked = True
                If a(i) = "6" Then ckBauantrag.IsChecked = True
                If a(i) = "7" Then ckAlt.IsChecked = True
                If a(i) = "8" Then ckOeff.IsChecked = True
                'If a(i) = "9" Then ck.IsChecked = True 'fdleitung
                If a(i) = "10" Then ckSchorn.IsChecked = True
                If a(i) = "11" Then ckSys.IsChecked = True
                If a(i) = "13" Then ckFDASS.IsChecked = True
                'If a(i) = "12" Then ckIll.IsChecked = True
            Else
                einzelpersonenString = einzelpersonenString & a(i) & "  "

            End If
        Next

        tbEinzelpersonen.Text = einzelpersonenString
    End Sub

    Private Sub setzeModus()
        modus = "neu"
        If aktVGR.id > 0 Then
            modus = "edit"
        End If
    End Sub

    'Private Sub btnrecallStandard_Click(sender As Object, e As RoutedEventArgs)
    '    VIDgruppentext = DS_Tools.getDS_userStandard(myGlobalz.sitzung.aktBearbeiter.ID, myGlobalz.sitzung.VorgangREC)
    '    If VIDgruppentext.IsNothingOrEmpty Then VIDgruppentext = "1"
    '    displayGruppenChecks(VIDgruppentext)
    '    e.Handled = True
    'End Sub

    Private Sub ckFDL_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
    End Sub

    Private Sub btnwhoisinFDL_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "9" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub

    Private Sub displayGruppenzugehoerigkeit()
        USERgruppentext = DS_Tools.getGruppen4user(myGlobalz.sitzung.aktBearbeiter.ID, myGlobalz.sitzung.VorgangREC)
        'tbInfo.Text = "Sie (" & myGlobalz.sitzung.aktBearbeiter.username & ")  sind folgenden Gruppen zugeordnet: " & Environment.NewLine & Environment.NewLine &
        '    USERgruppentext & Environment.NewLine & Environment.NewLine &
        '    "Hinweis:" & Environment.NewLine &
        '    "Sie sollten Ihren Standard mindestens so definieren, dass " & Environment.NewLine &
        '    "er Ihrer Gruppenzugehörigkeit entspricht!" & Environment.NewLine
        'tbStand.Text = "Der Standard gilt automatisch bei den " & Environment.NewLine &
        '    "Vorgängen, für die keine Erlaubnisse definiert sind."
    End Sub

    Private Sub btnEinzelpersonen_Click(sender As Object, e As RoutedEventArgs)
        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl("mehrfachauswahl", VIDgruppentext)
        bearbeiterauswahlbox.ShowDialog()
        ' getMehrereBearbeiter(bearbeiterauswahlbox)
        tbEinzelpersonen.Text = getMehrereBearbeiter(bearbeiterauswahlbox.mehrfachauswahlsumme, tbEinzelpersonen.Text)
        e.Handled = True
    End Sub



    Private Sub btnwhoisinFDASS_Click(sender As Object, e As RoutedEventArgs)
        Dim gruppennummer As String = "13" : DS_Tools.gruppenzusammensetzung(gruppennummer)
        e.Handled = True
    End Sub
End Class
