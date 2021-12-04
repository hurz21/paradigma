Public Class winOutlookEmailUebernehmen
    Private Property anzahlDateien As Integer
    Property _sendername As String
    Property _person_name As String
    Property _senderbetreff As String
    Property _senderemailAdress As String
    Property _organisation As String
    Public Property fotosalsRaumbezug As Boolean
    Property _recipientCount As Integer
    Property _recipientstring As String
    Private Property _VSFinfotext As String 
    Private Property _bescheibung As String
    Private Property _richtung As String
    Private Property _DatumErhalten As Date
    Private Property _anhangdateien As List(Of String)
    Public Property anhangsauswahlListe As New List(Of AnhangsdateiAuswahl)

    Sub New(ByVal anzahlDateien As Integer,
                ByVal sendername As String,
                ByVal senderbetreff As String,
                ByVal VSFinfotext As String,
                ByVal senderemailAdress As String,
                ByVal organisation As String,
                ByVal person_name As String,
                ByVal recipientCount As Integer,
                ByVal recipientString As String,
                beschreibung As String,
                richtung As String,
                Datumerhalten As Date,
                anhangdateien As List(Of String),
                aktvid As integer)
        InitializeComponent()
        _senderemailAdress = senderemailAdress
        _anzahlDateien = anzahlDateien
        _sendername = sendername
        _senderbetreff = senderbetreff
        _VSFinfotext = VSFinfotext
        _organisation = organisation
        _person_name = person_name
        _recipientCount = recipientCount
        _recipientstring = recipientString
        _bescheibung = beschreibung
        _richtung = richtung
        _DatumErhalten = Datumerhalten
        _anhangdateien = anhangdateien
        _aktvid=aktvid
    End Sub

Private Property formistgeladen As Boolean =false

Private Property _aktvid As Integer 




    Private Sub winOutlookEmailUebernehmen_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        tbanzahl.Text = _anzahlDateien.ToString & " Stück"
        'tbSender.Text = _sendername
        'tbTitel.Text = _senderbetreff
        tbpersonname.Text = _person_name
        tbpersonEmail.Text = _senderemailAdress
        tbpersonorg.Text = _organisation
        tbRecipientsAdresses.Text = CStr(_recipientstring)
        tbanzahlRecipients.Text = CStr(_recipientCount)
        If String.IsNullOrEmpty(_person_name) Then
            deaktiviereSenderCheckboxen()
        End If
        initComboRichtung()
        detailsTools.initErgeinistypCombo(Me, "detail_ereignisseKURZ.xml", "XMLSourceComboBoxEreignisse") : cmbVerlaufAuswahl.SelectedIndex = 0

        chkRecipientsBeteiligtenanlegen.IsChecked = CBool(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailRecipients2Beteiligte"))
        chkSenderBeteiligtenanlegen.IsChecked = CBool(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailsender2Beteiligte"))

        If mailTools.emailIstSchonVorhanden(_senderemailAdress) Then
            deaktiviereSenderCheckboxen()
        End If

        If _senderemailAdress.ToLower.Trim.Contains("@kreis-offenbach.de") Then
            deaktiviereSenderCheckboxen()
        End If
        tbBeschreibung.Text = GetBeschreibung()

        If String.IsNullOrEmpty(_VSFinfotext) Then
            tbvcfinfotext.Text = ""
            evcard.Visibility = Windows.Visibility.Hidden
        Else
            tbvcfinfotext.Text = _VSFinfotext
            evcard.Visibility = Windows.Visibility.Visible
        End If 
        If anzahlDateien > 0 Then
            tbschlagwortintro.FontWeight = FontWeights.Bold
            FocusManager.SetFocusedElement(Me, tbSchlagworte)
            tbSchlagworte.Text = tbBeschreibung.Text
        End If
        If String.IsNullOrEmpty(tbSchlagworte.Text) Then
            If Not String.IsNullOrEmpty(tbBeschreibung.Text) Then
                tbSchlagworte.Text = tbBeschreibung.Text
            End If
        End If
        tbRichtung.Text = _richtung
        DatePicker1.SelectedDate = _DatumErhalten
        Uhrzeitsetzen()

        anhangsauswahlListe = anhangsdateienNachObj(_anhangdateien)
        dgAnhaenge.DataContext = anhangsauswahlListe
         formistgeladen =true
        Title="Outlook - Email übernehmen für Vorgang: " & _aktvid
        e.Handled = True
    End Sub
    Private Sub initComboRichtung()
        cmbEreignisRichtung.Items.Add("")
        cmbEreignisRichtung.Items.Add("Eingang")
        cmbEreignisRichtung.Items.Add("Ausgang")
        cmbEreignisRichtung.SelectedValue = myGlobalz.sitzung.aktEreignis.Richtung
    End Sub
    Private Sub Uhrzeitsetzen()
        tbStunde.Text = myGlobalz.sitzung.aktEreignis.Datum.Hour.ToString
        tbMinute.Text = myGlobalz.sitzung.aktEreignis.Datum.Minute.ToString
    End Sub 

    Private Sub cmbEreignisRichtung_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If cmbEreignisRichtung.SelectedItem Is Nothing Then Exit Sub
        Dim item2 As String = CStr(cmbEreignisRichtung.SelectedItem)
        If item2 Is Nothing Then Exit Sub
        myGlobalz.sitzung.aktEreignis.Richtung = item2
        If myGlobalz.sitzung.aktEreignis.Art.ToLower.StartsWith("email") Then
            tbArt.Text = If(myGlobalz.sitzung.aktEreignis.Richtung.ToLower = "ausgang", "Email-Ausgang", "Email-Eingang")
        End If
        tbRichtung.Text = item2
        myGlobalz.sitzung.aktEreignis.Richtung = item2
        e.Handled = True
    End Sub

    Private Sub cmbVerlaufAuswahl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            If cmbVerlaufAuswahl.SelectedValue Is Nothing Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower = "hinzufügen" Then Exit Sub
            If cmbVerlaufAuswahl.SelectedValue.ToString.ToLower.StartsWith("---") Then Exit Sub
            If not formistgeladen Then Exit sub
            Dim item As String = CType(cmbVerlaufAuswahl.SelectedValue, String)
            tbArt.Text = item
            myGlobalz.sitzung.aktEreignis.Art = item
            'If item.ToLower = "abgabe an bauaufsicht" Then ckbistAbgabeBauaufsicht.IsChecked = True
            'If item.ToLower = "ortstermin" Then ckbistortstermin.IsChecked = True
            'If item.ToLower.StartsWith("stellung") Then ckbiststellungnahme.IsChecked = True
            'If item.ToLower.StartsWith("vorgang erledigt") Then ckberledigt.IsChecked = True
            cmbVerlaufAuswahl.SelectedValue = ""
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbVerlaufAuswahl_SelectionChanged" & ex.ToString)
        End Try
    End Sub

    Private Sub tbStunde_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        ' btnSpeichernEreignis.IsEnabled = True
    End Sub

    Private Sub tbMinute_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs)
        'btnSpeichernEreignis.IsEnabled = True
    End Sub

    Private Sub btnsetzeDatumaufNow(sender As Object, e As RoutedEventArgs)
        DatePicker1.SelectedDate = Now
    End Sub
    Private Sub bnWeiter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles bnWeiter.Click
        'For Each ele In anhangsauswahlListe

        'Next
        fotosalsRaumbezug = If(chkFotozuRaumbzug.IsChecked, True, False)
        CLstart.myc.userIniProfile.WertSchreiben("Outlook", "mailsender2Beteiligte", If(chkSenderBeteiligtenanlegen.IsChecked, "True", "False"))
        CLstart.myc.userIniProfile.WertSchreiben("Outlook", "mailRecipients2Beteiligte", If(chkRecipientsBeteiligtenanlegen.IsChecked, "True", "False"))
        DialogResult = True
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbbruch.Click
        Me.Close()
        e.Handled = True
    End Sub

    Private Function GetBeschreibung() As String
        If String.IsNullOrEmpty(_bescheibung) Then
            If (_sendername & ", " & _senderbetreff).Length > 199 Then
                Return (_sendername & ", " & _senderbetreff).Substring(0, 200)
            End If
            Return _sendername & ", " & _senderbetreff
        Else
            Return _bescheibung
        End If
    End Function

    Private Sub deaktiviereSenderCheckboxen()
        chkSenderBeteiligtenanlegen.IsChecked = False
        '  chkSenderBeteiligtenanlegen.IsEnabled = False
    End Sub
End Class
