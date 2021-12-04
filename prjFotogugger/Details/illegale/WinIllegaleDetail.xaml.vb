Public Class WinIllegaleDetail
    Property oldIlleg As New clsIllegaleHuette
    Property newIlleg As New clsIllegaleHuette

    Private readOnlyDoxsInTxtCrtlOeffnen As Boolean = False
    Property modus As String
    'Property aktereignis As New clsEreignis
    Sub New(_readOnlyDoxsInTxtCrtlOeffnen As Boolean)
        InitializeComponent()
        readOnlyDoxsInTxtCrtlOeffnen = _readOnlyDoxsInTxtCrtlOeffnen
    End Sub

    Private Property formistgeladen As Boolean = False

    Private Sub WinIllegaleDetail_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        oldIlleg = modIllegaleHuette.getIllegale4Vid(myglobalz.sitzung.aktVorgangsID)
        modus = If(oldIlleg.illegID < 1, "neu", "edit")
        oldIlleg.vid = myglobalz.sitzung.aktVorgangsID
        If modus = "neu" Then
            ausfuellen()
            datumsfelderLeeren()
        End If
        If modus = "edit" Then
            'mapping? ihah
            datumsfelderLeeren()
            ausfuellen()
        End If
        EreignisButtonAnhoerungSchalten()
        EreignisButtonRaeumungSchalten()
        EreignisButtonVerfuegungSchalten()
        formistgeladen = True
        btnSpeichern.IsEnabled = False
        Title = getTitel("Illegale Bauten - Detailansicht, Vorgang: " & myglobalz.sitzung.aktVorgangsID & " / " & modus)
        e.Handled = True
    End Sub

    Private Sub EreignisButtonAnhoerungSchalten()
        If oldIlleg.eid_anhoerung > 0 Then
            If oldIlleg.illegID > 0 Then
                btnEreignisAnhoerung.IsEnabled = True
                btnEreignisAnhoerung.Foreground = Brushes.Gold
                btnEreignisAnhoerung.ToolTip = "zum Ereignis springen"
            Else
                btnEreignisRaeumung.IsEnabled = False
            End If
        Else
            If modus = "edit" Then
                If Not glob2.IstDatumSinnvoll((dpAnhoerung.SelectedDate)) Then
                    btnEreignisAnhoerung.IsEnabled = False
                Else
                    If oldIlleg.illegID > 0 Then btnEreignisAnhoerung.IsEnabled = True
                End If
            Else
                'modus neu
                btnEreignisAnhoerung.IsEnabled = False
            End If
        End If
    End Sub

    Private Sub EreignisButtonRaeumungSchalten()
        If oldIlleg.eid_raeumung > 0 Then
            If oldIlleg.illegID > 0 Then
                btnEreignisRaeumung.IsEnabled = True
                btnEreignisRaeumung.Foreground = Brushes.Gold
                btnEreignisRaeumung.ToolTip = "zum Ereignis springen"
            Else
                btnEreignisRaeumung.IsEnabled = False
            End If
        Else
            If modus = "edit" Then
                If Not glob2.IstDatumSinnvoll((dpRaeumung.SelectedDate)) Then
                    btnEreignisRaeumung.IsEnabled = False
                Else
                    If oldIlleg.illegID > 0 Then btnEreignisRaeumung.IsEnabled = True
                End If
            Else
                'modus neu
                btnEreignisRaeumung.IsEnabled = False
            End If
        End If
    End Sub

    Private Sub EreignisButtonVerfuegungSchalten()
        If oldIlleg.eid_verfuegung > 0 Then
            If oldIlleg.illegID > 0 Then
                btnEreignisVerfuegung.IsEnabled = True
                btnEreignisVerfuegung.Foreground = Brushes.Gold
                btnEreignisVerfuegung.ToolTip = "zum Ereignis springen"
            Else
                btnEreignisRaeumung.IsEnabled = False
            End If
        Else
            If modus = "edit" Then
                If Not glob2.IstDatumSinnvoll((dpVerfuegungVom.SelectedDate)) Then
                    btnEreignisVerfuegung.IsEnabled = False
                Else
                    If oldIlleg.illegID > 0 Then btnEreignisVerfuegung.IsEnabled = True
                End If
            Else
                'modus neu
                btnEreignisVerfuegung.IsEnabled = False
            End If
        End If
    End Sub
    Private Sub datumsfelderLeeren()
        dpAnhoerung.Text = ""
        dpAnhoerung.DisplayDate = Now

        dpRaeumung.Text = ""
        dpRaeumung.DisplayDate = Now

        dpRaeumungBis.Text = ""
        dpRaeumungBis.DisplayDate = Now

        dpVerfuegungVom.Text = ""
        dpVerfuegungVom.DisplayDate = Now

        dpFallErledigt.Text = ""
        dpFallErledigt.DisplayDate = Now
    End Sub



    Private Sub btnAbbrung_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub ausfuellen()
        'tbGebiet.Text = oldIlleg.gebiet
        tbVermerk.Text = oldIlleg.vermerk
        tbQuelle.Text = oldIlleg.quelle & ":"
        tbTS.Text = Format(oldIlleg.ts, "dd.MM.yyyy hh:mm")

        dpAnhoerung.SelectedDate = oldIlleg.anhoerung
        If Not glob2.IstDatumSinnvoll(oldIlleg.anhoerung) Then
            dpAnhoerung.Text = ""
            dpAnhoerung.DisplayDate = Now
        End If

        dpRaeumungBis.SelectedDate = oldIlleg.raeumungBisDatum
        If Not glob2.IstDatumSinnvoll(oldIlleg.raeumungBisDatum) Then
            dpRaeumungBis.Text = ""
            dpRaeumungBis.DisplayDate = Now
        End If

        dpFallErledigt.SelectedDate = oldIlleg.fallerledigt
        If Not glob2.IstDatumSinnvoll(oldIlleg.fallerledigt) Then
            dpFallErledigt.Text = ""
            dpFallErledigt.DisplayDate = Now
        End If
        dpRaeumung.SelectedDate = oldIlleg.raeumung
        If Not glob2.IstDatumSinnvoll(oldIlleg.raeumung) Then
            dpRaeumung.Text = ""
            dpRaeumung.DisplayDate = Now
        End If

        dpVerfuegungVom.SelectedDate = oldIlleg.verfuegung
        If Not glob2.IstDatumSinnvoll(oldIlleg.verfuegung) Then
            dpVerfuegungVom.Text = ""
            dpVerfuegungVom.DisplayDate = Now
        End If

        tbStatus.Text = clsIllegbauTools.statusIndexNachText(oldIlleg.status)
        tbGebiet.Text = clsIllegbauTools.gebietsIndexNachText(oldIlleg.gebiet)
        tbRaeumung.Text = clsIllegbauTools.RaeumungsTypIndexNachText(oldIlleg.raeumungsTyp)
    End Sub



    Private Function gebietPruefenVorhanden() As Boolean
        If modIllegaleHuette.NochKeinGebietErfasst(oldIlleg) Then
            MsgBox("Sie sollten noch das Gebiet prüfen und eingeben!")
            cbGebiet.IsDropDownOpen = True
            Return False
        Else
            Return True
        End If
    End Function
    Private Sub dpAnhoerung_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        gebietPruefenVorhanden()
        oldIlleg.anhoerung = CDate(dpAnhoerung.SelectedDate)
        EreignisButtonAnhoerungSchalten()
        glob2.schliessenButton_einschalten(btnSpeichern)
        If tbStatus.Text = "planmäßig" Or tbStatus.Text = "recherche" Then
            tbStatus.Text = "laufend"
            MsgBox("Status der Hütte wurde auf >laufend< geändert")
        End If

        e.Handled = True
    End Sub


    Private Sub cbStatus_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        oldIlleg.status = CStr(cbStatus.SelectedIndex)
        tbStatus.Text = clsIllegbauTools.statusIndexNachText(oldIlleg.status)
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub cbGebiet_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        oldIlleg.gebiet = CStr(cbGebiet.SelectedIndex)
        tbGebiet.Text = clsIllegbauTools.gebietsIndexNachText(oldIlleg.gebiet)
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub cbRaeumungsTyp_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If Not formistgeladen Then Exit Sub
        If cbRaeumungsTyp.SelectedIndex = 0 Then
            tbRaeumung.Text = "-"
            oldIlleg.raeumungsTyp = "0"
        Else
            oldIlleg.raeumungsTyp = CStr(cbRaeumungsTyp.SelectedIndex)
            tbRaeumung.Text = clsIllegbauTools.RaeumungsTypIndexNachText(oldIlleg.raeumungsTyp)
        End If
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub dpRaeumung_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        oldIlleg.raeumung = CDate(dpRaeumung.SelectedDate)
        gebietPruefenVorhanden()
        EreignisButtonRaeumungSchalten()
        glob2.schliessenButton_einschalten(btnSpeichern)
        cbRaeumungsTyp.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Private Sub dpVerfuegungVom_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        oldIlleg.verfuegung = CDate(dpVerfuegungVom.SelectedDate)
        gebietPruefenVorhanden()
        EreignisButtonVerfuegungSchalten()
        tbRaeumung.Text = "Rechtsstreit"
        oldIlleg.raeumungsTyp = "3"
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub dpRaeumungBis_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        oldIlleg.raeumungBisDatum = CDate(dpRaeumungBis.SelectedDate)
        gebietPruefenVorhanden()
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub

    Private Sub dpFallErledigt_SelectedDateChanged(sender As Object, e As SelectionChangedEventArgs)
        If Not formistgeladen Then Exit Sub
        oldIlleg.fallerledigt = CDate(dpFallErledigt.SelectedDate)
        gebietPruefenVorhanden()
        tbStatus.Text = "erledigt"
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True

    End Sub

    Private Sub tbVermerk_TextChanged(sender As Object, e As TextChangedEventArgs) Handles tbVermerk.TextChanged
        oldIlleg.vermerk = tbVermerk.Text
        glob2.schliessenButton_einschalten(btnSpeichern)
        e.Handled = True
    End Sub



    Private Function KeineTextDannAbbruch() As Boolean
        Dim shouldReturn As Boolean = False
        If tbRaeumung.Text.IsNothingOrEmpty() Then
            oldIlleg.raeumungsTyp = ""
            MsgBox("Sie müssen die Art der Räumung angeben!", MsgBoxStyle.Information, "Art der Räumung wurde nicht angegeben")
            cbRaeumungsTyp.IsDropDownOpen = True
            shouldReturn = True
        End If
        Return shouldReturn
    End Function
    Private Sub btnEreignisRaeumung_Click(sender As Object, e As RoutedEventArgs)
        If Not glob2.IstDatumSinnvoll(dpRaeumung.SelectedDate) Then
            MsgBox("Datumseingabe ungültig!")
            Exit Sub
        End If
        Dim beschreibung As String : If KeineTextDannAbbruch() Then Exit Sub
        If glob2.IstDatumSinnvoll(dpRaeumung.SelectedDate) Then
            oldIlleg.raeumung = CDate(dpRaeumung.SelectedDate)
        Else
            MsgBox("Datumseingabe ungültig!")
            Exit Sub
        End If
        oldIlleg.raeumungsTyp = clsIllegbauTools.TextNachRaeumungsTypIndex(tbRaeumung.Text)
        beschreibung = clsIllegbauTools.getRaeumungstitel(oldIlleg.raeumungsTyp)

        If oldIlleg.eid_raeumung < 1 Then
            myglobalz.sitzung.aktEreignis = modIllegaleHuette.EreignisErzeugen(dpRaeumung.SelectedDate, "Räumung", beschreibung)
            myglobalz.sitzung.aktEreignis.ID = modIllegaleHuette.Ereignisspeichern(myglobalz.sitzung.aktEreignis)
            oldIlleg.eid_raeumung = myglobalz.sitzung.aktEreignis.ID
            modIllegaleHuette.huettespeichern(modus, oldIlleg)
        Else
            myglobalz.sitzung.aktEreignis.clearValues()
            myglobalz.sitzung.aktEreignis.ID = oldIlleg.eid_raeumung
        End If
        Close()
        modIllegaleHuette.ereignisoeffnen(myGlobalz.sitzung.aktEreignis, readOnlyDoxsInTxtCrtlOeffnen)
        e.Handled = True
    End Sub

    Private Sub btnEreignisAnhoerung_Click(sender As Object, e As RoutedEventArgs)
        If Not glob2.IstDatumSinnvoll(dpAnhoerung.SelectedDate) Then
            MsgBox("Datumseingabe ungültig!")
            Exit Sub
        End If
        oldIlleg.anhoerung = CDate(dpAnhoerung.SelectedDate)
        If oldIlleg.eid_anhoerung < 1 Then
            myglobalz.sitzung.aktEreignis = modIllegaleHuette.EreignisErzeugen(oldIlleg.anhoerung, "Anhörung", "Anhörung illegaler Eingriff")
            myglobalz.sitzung.aktEreignis.ID = modIllegaleHuette.Ereignisspeichern(myglobalz.sitzung.aktEreignis)
            oldIlleg.eid_anhoerung = myglobalz.sitzung.aktEreignis.ID
            modIllegaleHuette.huettespeichern(modus, oldIlleg)
        Else
            myglobalz.sitzung.aktEreignis.clearValues()
            myglobalz.sitzung.aktEreignis.ID = oldIlleg.eid_anhoerung
        End If
        Close()
        modIllegaleHuette.ereignisoeffnen(myGlobalz.sitzung.aktEreignis, readOnlyDoxsInTxtCrtlOeffnen)
        e.Handled = True
    End Sub


    Private Sub btnEreignisVerfuegung_Click(sender As Object, e As RoutedEventArgs)
        If Not glob2.IstDatumSinnvoll(dpVerfuegungVom.SelectedDate) Then
            MsgBox("Datumseingabe ungültig!")
            Exit Sub
        End If
        oldIlleg.verfuegung = CDate(dpVerfuegungVom.SelectedDate)
        If oldIlleg.eid_verfuegung < 1 Then
            myglobalz.sitzung.aktEreignis = modIllegaleHuette.EreignisErzeugen(oldIlleg.verfuegung, "Verfügung", "Verfügung illegaler Eingriff")
            myglobalz.sitzung.aktEreignis.ID = modIllegaleHuette.Ereignisspeichern(myglobalz.sitzung.aktEreignis)
            oldIlleg.eid_verfuegung = myglobalz.sitzung.aktEreignis.ID
            modIllegaleHuette.huettespeichern(modus, oldIlleg)
        Else
            myglobalz.sitzung.aktEreignis.clearValues()
            myglobalz.sitzung.aktEreignis.ID = oldIlleg.eid_verfuegung
        End If
        Close()
        modIllegaleHuette.ereignisoeffnen(myGlobalz.sitzung.aktEreignis, readOnlyDoxsInTxtCrtlOeffnen)
        e.Handled = True
    End Sub

    Private Sub btnSpeichern_Click(sender As Object, e As RoutedEventArgs)
        oldIlleg.gebiet = clsIllegbauTools.TextNachGebietsIndex(tbGebiet.Text)

        oldIlleg.status = clsIllegbauTools.TextNachStatusIndex(tbStatus.Text)
        oldIlleg.raeumungsTyp = clsIllegbauTools.TextNachRaeumungsTypIndex(tbRaeumung.Text)
        oldIlleg.vermerk = tbVermerk.Text
        If glob2.IstDatumSinnvoll((dpAnhoerung.SelectedDate)) Then
            oldIlleg.anhoerung = CDate(dpAnhoerung.SelectedDate)
        End If
        If modIllegaleHuette.IstAnhoerungVorhanden(oldIlleg) And (Not gebietPruefenVorhanden()) Then
            MsgBox("Bitte ergänzen Sie die Gebietsangabe!")
            cbGebiet.IsDropDownOpen = True
            Exit Sub
        End If
        If glob2.IstDatumSinnvoll((dpRaeumung.SelectedDate)) Then
            oldIlleg.raeumung = CDate(dpRaeumung.SelectedDate)
            If Not modIllegaleHuette.IstAnhoerungVorhanden(oldIlleg) Then
                MsgBox("Sie haben noch keine Anhörung durchgeführt! Abbruch !")
                Exit Sub
            End If
        End If

        If glob2.IstDatumSinnvoll((dpRaeumungBis.SelectedDate)) Then
            oldIlleg.raeumungBisDatum = CDate(dpRaeumungBis.SelectedDate)
            If Not modIllegaleHuette.IstAnhoerungVorhanden(oldIlleg) Then
                MsgBox("Sie haben noch keine Anhörung durchgeführt! Abbruch !")
                Exit Sub
            End If
        End If

        oldIlleg.quelle = myglobalz.sitzung.aktBearbeiter.Initiale
        If glob2.IstDatumSinnvoll(oldIlleg.raeumung) Then
            If tbRaeumung.Text.IsNothingOrEmpty() Then
                oldIlleg.raeumungsTyp = ""
                MsgBox("Sie müssen die Art der Räumung angeben!", MsgBoxStyle.Information, "Art der Räumung wurde nicht angegeben")
                cbRaeumungsTyp.IsDropDownOpen = True
                Exit Sub
            End If
            If Not modIllegaleHuette.IstAnhoerungVorhanden(oldIlleg) Then
                MsgBox("Sie haben noch keine Anhörung durchgeführt! Abbruch !")
                Exit Sub
            End If
        End If
        If glob2.IstDatumSinnvoll((dpVerfuegungVom.SelectedDate)) Then
            oldIlleg.verfuegung = CDate(dpVerfuegungVom.SelectedDate)
            If Not modIllegaleHuette.IstAnhoerungVorhanden(oldIlleg) Then
                MsgBox("Sie haben noch keine Anhörung durchgeführt! Abbruch !")
                Exit Sub
            End If
        End If

        If glob2.IstDatumSinnvoll((dpFallErledigt.SelectedDate)) Then
            oldIlleg.fallerledigt = CDate(dpFallErledigt.SelectedDate)
            If Not modIllegaleHuette.IstAnhoerungVorhanden(oldIlleg) Then
                MsgBox("Sie haben noch keine Anhörung durchgeführt! Abbruch !")
                Exit Sub
            End If
        End If

        modIllegaleHuette.huettespeichern(modus, oldIlleg)
        glob2.generateIllegaleBautenInPostgis()
        Close()
        e.Handled = True
    End Sub

    Private Sub btnLoeschen_Click(sender As Object, e As RoutedEventArgs)
        Dim mressult As New MessageBoxResult
        mressult = MessageBox.Show("Soll dieser Fachdatensatz (Illegale Hüttn) wirklich gelöscht werden ?", "Fachdatensatz löschen",
                                  MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)

        If mressult = MessageBoxResult.Yes Then
            modIllegaleHuette.loeschen(oldIlleg.illegID)
            Close()
        End If

        e.Handled = True
    End Sub

    Private Sub btnGISebene_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        glob2.generateIllegaleBautenInPostgis()
        Close()
    End Sub

    Private Sub btnErledigtNUll_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        dpFallErledigt.SelectedDate = CDate("0001.01.01")
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub btnVerfuegNUll_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        dpVerfuegungVom.SelectedDate = CDate("0001.01.01")
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub btnRaeumungNUll_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        dpRaeumungBis.SelectedDate = CDate("0001.01.01")
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub btnVereinbNUll_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        dpRaeumung.SelectedDate = CDate("0001.01.01")
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub btnAnhoerungNUll_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        dpAnhoerung.SelectedDate = CDate("0001.01.01")
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub
End Class
