Imports Microsoft.Win32
Partial Public Class Window_email_sofort
    Private anhang As String
    Public abbruch As Boolean = True
    Public Sub New(ByVal anhangstring As String)
        InitializeComponent()
        anhang$ = anhangstring
    End Sub

    Private Sub Window_email_sofort_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub Window_email_sofort_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        tbAnhang.Text = anhang
        Title = StammToolsNs.setWindowTitel.exe("edit", "Email")
    End Sub

    Private Shared Sub anhangEinchecken()
        Dim trenner As String = myGlobalz.anhangtrenner
        If Not String.IsNullOrEmpty(myGlobalz.sitzung.SendMail.Anhang) Then
            With myGlobalz.sitzung.SendMail
                If .Anhang.EndsWith(trenner) Then .Anhang = .Anhang.Substring(0, .Anhang.Length - 1)
                Dim filenames$() = .Anhang.Split(CChar(trenner))
                glob2.Archiv_eingang(filenames, String.Format("{0}: {1}", .An, .Betreff), myGlobalz.sitzung.aktEreignis.ID, False, False, Now)
            End With
        End If
    End Sub

    Private Sub mailAbschicken(ByVal anhangtrenner As String, ByVal outlookAnzeigen As Boolean)
        Dim fehler As String = glob2.Email_verschicken(anhangtrenner, outlookAnzeigen)
        If Not fehler = "0" Then
            nachricht_und_Mbox("Beim Senden der Email trat ein Fehler auf: " & fehler)
        Else
            nachricht("Email wurde erfolgreich versendet.")
            NeuesEreigniserzeugen(anhangtrenner) 'myGlobalz.sitzung.aktEreignis
            ' Dim erfolg As Boolean
            'If VSTTools.editStammdaten_alleDB.exe(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten) Then
            '    nachricht("Stammdaten wurden angepasst")
            'End If

            myGlobalz.sitzung.Ereignismodus = "neu"
            clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", myGlobalz.sitzung.aktEreignis) '            If clsEreignisDB.Neu_speichern_Ereignis() Then nachricht("Daten wurden gespeichert!")
            CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = String.Format("{0} {1}: neu angelegt", myGlobalz.sitzung.aktEreignis.ID, myGlobalz.sitzung.aktEreignis.Beschreibung) : CLstart.myc.aLog.log()
            If chkAnhangeinchecken.IsChecked Then
                anhangEinchecken()
                nachricht("Email Anhang wurde eingecheckt")
            Else
                nachricht("Email Anhang wurde Nicht ins Archivb aufgenommen (auf Wunsch)")
            End If
        End If
    End Sub

    Private Shared Sub NeuesEreigniserzeugen(ByVal anhangtrenner As String)
        Try
            myGlobalz.sitzung.aktEreignis.clearValues()
            With myGlobalz.sitzung
                .aktEreignis.Datum = Now
                .aktEreignis.Art = "Email"
                .aktEreignis.Richtung = "Ausgang"
                .aktEreignis.Beschreibung = String.Format("{0}: {1}", myGlobalz.sitzung.SendMail.An, myGlobalz.sitzung.SendMail.Betreff)
                .aktEreignis.Notiz = clsString.changeHTML2text(myGlobalz.sitzung.SendMail.Message)
                If Not String.IsNullOrEmpty(myGlobalz.sitzung.SendMail.Anhang.Trim) Then
                    Dim einzeln As String() = myGlobalz.sitzung.SendMail.Anhang.Split(CChar(anhangtrenner))
                    .aktEreignis.Notiz &= vbCrLf & "Folgende Dateien wurden im Anhang verschickt: " & vbCrLf
                    For Each datei In einzeln
                        .aktEreignis.Notiz &= vbCrLf & datei & vbCrLf
                    Next
                End If
            End With
        Catch ex As Exception
            nachricht("Fehler in NeuesEreigniserzeugen" & ex.ToString)
        End Try
    End Sub

    'Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
    '    Button2_ClickExtracted()
    '    e.Handled = True
    'End Sub

    'Private Shared Sub Button2_ClickExtracted()
    '    'jetzt erst auschecken der Dokumente
    '    If myGlobalz.Arc.ArcRec.dt.Rows.Count > 10 Then
    '        MessageBox.Show("Es werden mehr als 10 Dokumente als möglicher Anhang vorbereitet! (" & myGlobalz.Arc.ArcRec.dt.Rows.Count & ")" & vbCrLf &
    '                        "Dies kann ein wenig dauern. " & vbCrLf & vbCrLf &
    '                        "Der Vorgang ist beendet sobald der Auswahldialog erscheint. " & vbCrLf & vbCrLf &
    '                        "Bitte haben Sie etwas Geduld.", "Dokumente auschecken", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
    '    End If
    '    myGlobalz.Arc.AllesAuscheckenVorgang(False, True)

    '    Dim vache As String = IO.Path.Combine(myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID.ToString)
    '    Dim ofd As New OpenFileDialog() With {.Title = "Bitte wählen Sie die Dateien aus!",
    '                                          .InitialDirectory = vache,
    '                                          .Multiselect = True,
    '                                          .Filter = "Alle(*.*)|*.*"}
    '    Dim summe$ = ""
    '    If ofd.ShowDialog Then
    '        For Each dokument As String In ofd.FileNames()
    '            summe$ &= dokument & myGlobalz.anhangtrenner
    '        Next
    '    End If
    '    myGlobalz.sitzung.SendMail.Anhang = String.Format("{0}" & myGlobalz.anhangtrenner & "{1}", myGlobalz.sitzung.SendMail.Anhang, summe)
    '    myGlobalz.sitzung.SendMail.Anhang = clsString.removeLeadingChar(myGlobalz.sitzung.SendMail.Anhang, myGlobalz.anhangtrenner)
    '    If String.IsNullOrEmpty(summe) Then
    '        MessageBox.Show("Es wurden keine Dateien für den Anhang ausgewählt!", "Anhang auswählen", MessageBoxButton.OK, MessageBoxImage.Information)
    '    End If
    'End Sub

    Public Sub auswahlBeteiligte(ByVal feldname As String)
        Dim winz As New winzugriff("beteiligteEmails")
        winz.ShowDialog()
        If feldname = "an" Then
            tbAN.Text = myGlobalz.TransferString
        End If
        If feldname = "cc" Then
            tbCC.Text = myGlobalz.TransferString
        End If
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
        e.Handled = True
        auswahlBeteiligte("an")
    End Sub

    Private Sub anhangMitFotosbilden()
        If mailTools.anhangEnthaeltFotos(anhang) Then
            Dim breite, hoehe As Int16
            Dim sizeString As String = fotosverkleinern.takeSizestring(cmbIMGSIZE.SelectedValue.ToString)
            If sizeString.IsNothingOrEmpty Then
                breite = 1280 : hoehe = 1024
            Else
                fotosverkleinern.sizeString2wh(sizeString, breite, hoehe)
                If breite < 100 Or hoehe < 100 Then
                    breite = 1280 : hoehe = 1024
                End If
            End If

            anhang = fotosverkleinern.bildethumbs(anhang, breite, hoehe) '1280, 1024)
            myGlobalz.sitzung.SendMail.Anhang = anhang
        End If
    End Sub
    Private Sub abschicken_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles abschicken.Click
        If Not isEingabeOK() Then
            e.Handled = True
            Exit Sub
        End If
        If chkFotosverkleinern.IsChecked Then
            anhangMitFotosbilden()
        End If
        If chkOutlookanzeigen.IsChecked Then
            CLstart.mycSimple.outlookAnzeigen = True
            CLstart.myc.userIniProfile.WertSchreiben("Outlook", "anzeigen", "True")
        Else
            CLstart.mycSimple.outlookAnzeigen = False
            CLstart.myc.userIniProfile.WertSchreiben("Outlook", "anzeigen", "False")
        End If
        If chkWorddoksZuPDFKonvertieren.IsChecked Then
            mailTools.worddokusNachPdfKonvertierenUndStringAnpassen(anhang, myGlobalz.anhangtrenner)
        End If
        mailAbschicken(myGlobalz.anhangtrenner, CLstart.mycSimple.outlookAnzeigen)
        abbruch = False
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub tbMessage_TextChanged_1(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbMessage.TextChanged
        glob2.istTextzulang(5500, tbMessage)
        e.Handled = True
    End Sub

    Private Sub btnCC_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCC.Click
        auswahlBeteiligte("cc")
        e.Handled = True
    End Sub

    Private Function isEingabeOK() As Boolean
        Try
            If tbAN.Text.Trim.IsNothingOrEmpty Then
                MsgBox("Adressat fehlt! Emailadresse angeben!")
                Return False
            End If
            If Not tbAN.Text.Trim.Contains("@") Then
                MsgBox("Emailadresse ist ungültig!")
                Return False
            End If
            If tbBetrifft.Text.Trim.IsNothingOrEmpty Then
                MsgBox("Betrifft fehlt! Betrifft  angeben!")
                Return False
            End If
            Return True
        Catch ex As Exception
            nachricht("fehler in mail isEingabeOK: " & ex.ToString)
        End Try
    End Function

    Private Sub abschicken_Loaded(sender As Object, e As RoutedEventArgs) Handles abschicken.Loaded

    End Sub

    Private Sub btnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        abbruch = True
        Close()
        e.Handled = True
    End Sub
End Class
