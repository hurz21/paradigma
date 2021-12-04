Imports Microsoft.Win32
Partial Public Class Window_email_sofort
    Private anhang As String = ""
    Private an As String = ""
    Private cc As String = ""
    Public abbruch As Boolean = True
    Public templatename As String = ""
    Private Sub gastLayout()
        Background = myglobalz.GetSecondBackground()
    End Sub

    Public Sub New(ByVal anhangstring As String, _templatename As String, _an As String, _cc As String)
        InitializeComponent()
        anhang = anhangstring
        templatename = _templatename
        an = _an
        cc = _cc
    End Sub

    Private Sub Window_email_sofort_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing

    End Sub

    Private Sub Window_email_sofort_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        Dim anhangLength As Double = getanhangLength(anhang)
        Dim text As String = dokScanPDF.getFileSize4Length(anhangLength)
        tbAnhang.ToolTip = text
        tbanhangSize.Text = text
        handleanhangGroesse(text)
        tbAnhang.Text = anhang
        'tbAN.Text = myGlobalz.sitzung.SendMail.An
        tbAN.Text = an
        'tbCC.Text = myGlobalz.sitzung.SendMail.CC
        tbCC.Text = cc
        tbBetrifft.Text = myGlobalz.sitzung.SendMail.Betreff
        tbMessage.Text = myGlobalz.sitzung.SendMail.Message
        chkAnhangeinchecken.IsChecked = False 'will weyers so
        'chkOutlookanzeigen.IsChecked = myGlobalz.ou
        Title = StammToolsNs.setWindowTitel.exe("edit", "Email" & " Template: " & templatename)
        gastLayout()
    End Sub

    Private Sub handleanhangGroesse(text As String)
        Try
            l(" MOD handleanhangGroesse anfang")
            If text.EndsWith(" GB") Then
                tbanhangSize.Foreground = Brushes.DarkRed
                MessageBox.Show("Der Anhang hat eine Größe im GB-Bereich. Bitte überlegen Sie ob diese Größe verschickt werden kann.", "Wichtiger Hinweis zum Anhang", MessageBoxButton.OK, MessageBoxImage.Information)
            End If
            If text.EndsWith(" MB") Then
                Dim test As String
                test = text.Replace(" MB", "") '.Replace(",", ".")
                Dim a As Integer = CInt(test)
                If a > 10 Then
                    tbanhangSize.Foreground = Brushes.DarkOrange
                    MessageBox.Show("Der Anhang ist größer als 10MB. Es könnte Probleme mit Ihrem Outlook oder mit dem Briefkasten des Empfängers geben!", "Wichtiger Hinweis zum Anhang", MessageBoxButton.OK, MessageBoxImage.Information)
                End If
            End If
            l(" MOD handleanhangGroesse ende")
        Catch ex As Exception
            l("Fehler in handleanhangGroesse: " ,ex)
        End Try
    End Sub

    Private Function getanhangLength(anhang As String) As Double
        Dim a() As String
        Dim summe As Double = 0
        Dim temp As Double = 0
        Try
            l(" MOD getAnhangGroesse anfang: anhang: " & anhang)
            a = anhang.Trim.Split(";"c)
            For i = 0 To a.Count - 1
                temp = dokScanPDF.GetFileLength(a(i).Trim)
                summe += temp
            Next
            l(" MOD getAnhangGroesse ende")
            Return summe
        Catch ex As Exception
            l("Fehler in getAnhangGroesse: " & anhang ,ex)
            Return 0
        End Try
    End Function

    Private Shared Sub anhangEinchecken()
        Dim trenner As String = myGlobalz.anhangtrenner
        If Not String.IsNullOrEmpty(myGlobalz.sitzung.SendMail.Anhang) Then
            With myGlobalz.sitzung.SendMail
                If .Anhang.EndsWith(trenner) Then .Anhang = .Anhang.Substring(0, .Anhang.Length - 1)
                Dim filenames$() = .Anhang.Split(CChar(trenner))
                glob2.Archiv_eingang(filenames, String.Format("{0}: {1}", .An, .Betreff), myGlobalz.sitzung.aktEreignis.ID, False, False, Now, False)
            End With
        End If
    End Sub

    Private Sub mailAbschicken(ByVal anhangtrenner As String, ByVal outlookAnzeigen As Boolean)
        Dim fehler As String = glob2.Email_verschicken(anhangtrenner, outlookAnzeigen)
        If Not fehler = "0" Then
            nachricht_und_Mbox("Beim Senden der Email trat ein Fehler auf: " & fehler)
        Else
            nachricht("Email wurde erfolgreich versendet.")
            If outlookAnzeigen Then
                MessageBox.Show("Es wird kein Ereignis angelegt. Bitte übernehmen Sie die Email normal.")
            Else
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
        End If
    End Sub

    Private Shared Sub NeuesEreigniserzeugen(ByVal anhangtrenner As String)
        Try
            myglobalz.sitzung.aktEreignis.clearValues()
            With myglobalz.sitzung
                .aktEreignis.Datum = Now
                .aktEreignis.Art = "Email"
                .aktEreignis.Quelle = myglobalz.sitzung.aktBearbeiter.Initiale
                .aktEreignis.Richtung = "Ausgang"
                .aktEreignis.Beschreibung = String.Format("{0}: {1}", myglobalz.sitzung.SendMail.An, myglobalz.sitzung.SendMail.Betreff)
                .aktEreignis.Notiz = LIBgemeinsames.clsString.changeHTML2text(myglobalz.sitzung.SendMail.Message)
                If Not String.IsNullOrEmpty(myglobalz.sitzung.SendMail.Anhang.Trim) Then
                    Dim einzeln As String() = myglobalz.sitzung.SendMail.Anhang.Split(CChar(anhangtrenner))
                    .aktEreignis.Notiz &= vbCrLf & "Folgende Dateien wurden im Anhang verschickt: " & vbCrLf
                    For Each datei In einzeln
                        .aktEreignis.Notiz &= vbCrLf & datei & vbCrLf
                    Next
                End If
            End With
        Catch ex As Exception
            nachricht("Fehler in NeuesEreigniserzeugen" ,ex)
        End Try
    End Sub

    Public Sub auswahlBeteiligte(ByVal feldname As String)
        Dim winz As New winEmailListe("beteiligteEmails")
        winz.ShowDialog()
        If feldname = "an" Then
            tbAN.Text = winz.transferString
        End If
        If feldname = "cc" Then
            tbCC.Text = winz.transferString
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
            myglobalz.sitzung.SendMail.Anhang = anhang
        End If
    End Sub
    Private Sub abschicken_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles abschicken.Click
        e.Handled = True

        anhang = tbAnhang.Text
        myGlobalz.sitzung.SendMail.An = tbAN.Text
        myGlobalz.sitzung.SendMail.CC = tbCC.Text
        myGlobalz.sitzung.SendMail.Betreff = tbBetrifft.Text
        myGlobalz.sitzung.SendMail.Message = tbMessage.Text
        myGlobalz.sitzung.SendMail.Anhang = anhang

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
            nachricht("fehler in mail isEingabeOK: " ,ex)
            Return False
        End Try
    End Function
    Private Sub btnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        abbruch = True
        Close()
        e.Handled = True
    End Sub

    Private Sub btnAnhang_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        MessageBox.Show("Sie wollen Dokumente UND Fotos mailen???" & Environment.NewLine &
                        "Dann brechen Sie hier ab und " & Environment.NewLine &
                        "benutzen Sie die Mehrfachauswahl in den BEIDEN Listen." & Environment.NewLine &
                        "Anschließend drücken Sie den ' M ' - Knopf (= Mailen)", "Mehrfachauswahl?", MessageBoxButton.OK, MessageBoxImage.Question)
    End Sub
End Class
