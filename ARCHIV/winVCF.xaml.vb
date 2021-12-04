Imports JFvCard.MyProject.vCard
Public Class winVCF

    Private Property imail As String
    Private _adresses As List(Of Address)
    Private _jfread As vCardReader
    Private _phones As List(Of Phone)
    Private _DateinameFull As String

    Sub New(ByVal DateinameFull1 As String)
        ' TODO: Complete member initialization 
        InitializeComponent()
        _DateinameFull = DateinameFull1
    End Sub


    Private Sub winVCF_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Title = "Elektronische Visitenkarte: " & _DateinameFull
        vcardread(_DateinameFull)
        If myGlobalz.sitzung.aktVorgangsID < 1 Then
            btnuebernahme.IsEnabled = False
            btnuebernahme.ToolTip = "Es ist kein Vorgang aktiv."
        End If
    End Sub

    Sub vcardread(ByVal datei As String)
        _jfread = New vCardReader()
        Dim vstring As String
        Dim lines As String()
        Dim test As String = ""
        _phones = New List(Of Phone)()
        _adresses = New List(Of Address)()
        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(1252)
        '  Dim continueLinestring As String = "=0D=0A=" '=0D=0A
        vstring = IO.File.ReadAllText(datei, enc)
        winvcfTools.bereinigen(vstring)
        lines = vstring.Split(CChar(vbCrLf))
        winvcfTools.ZeilenZusammnenfuegen1(_jfread, lines, "=0D=0A=")
        winvcfTools.ZeilenZusammnenfuegen1(_jfread, lines, "=0D=0A")
        winvcfTools.trimmen(lines)

        _jfread.getnames(lines, _jfread.familyname, _jfread.Vorname, _jfread.MiddleName, _jfread.Prefix, _jfread.Suffix)
        _jfread.getformattedname(lines, _jfread.FormattedName)
        _jfread.getORG(lines, _jfread.Org)
        _jfread.getTITLE(lines, _jfread.Title)
        _jfread.getURL(lines, _jfread.URL)
        _jfread.getNote(lines, _jfread.Note)

        _jfread.getEMAIL(lines, test)
        imail = test

        _jfread.getphones(lines, _phones)
        _jfread.getadresses(lines, _adresses)
        'ausgabe
        tbTitel.Text = _jfread.Title
        tbEmail.Text = imail
        tbname.Text = _jfread.familyname
        tbVname.Text = _jfread.Vorname
        tbMname.Text = _jfread.MiddleName

        tbformattedName.Text = _jfread.FormattedName
        phonesSetzen(_phones)

        tbAnrede.Text = _jfread.Prefix & " " & _jfread.Suffix
        tbNotiz.Text = _jfread.Note
        tbOrg.Text = _jfread.Org
        tbURL.Text = _jfread.URL

        adressenSetzen(_adresses)
    End Sub

 

    Private Sub phonesSetzen(ByVal phones As List(Of Phone))
        Try
            tbTel1.Text = phones(0).number
            tbFax1.Text = phones(0).number
            tbMobil1.Text = phones(0).number
            For Each pho In phones
                If pho.homeWorkType = HomeWorkType.home Then
                    If pho.phoneType = PhoneType.VOICE Then
                        tbTel1.Text = pho.number
                    End If
                    If pho.phoneType = PhoneType.FAX Then
                        tbFax1.Text = pho.number
                    End If
                    If pho.phoneType = PhoneType.CELL Then
                        tbMobil1.Text = pho.number
                    End If
                End If
                If pho.homeWorkType = HomeWorkType.work Then
                    If pho.phoneType = PhoneType.VOICE Then
                        tbTel1.Text = pho.number
                    End If
                    If pho.phoneType = PhoneType.FAX Then
                        tbFax1.Text = pho.number
                    End If
                    If pho.phoneType = PhoneType.CELL Then
                        tbMobil1.Text = pho.number
                    End If
                End If
            Next
        Catch ex As Exception
            
        End Try
    End Sub


    Private Sub adressenSetzen(ByVal adresses As List(Of Address))
        Try
            tbPLZ.Text = adresses(0).po
            tbStrasse.Text = adresses(0).street
            tbstadt.Text = adresses(0).locality
            For Each pho In adresses
                If pho.homeWorkType = HomeWorkType.home Then
                    tbPLZ.Text = pho.po
                    tbStrasse.Text = pho.street
                    tbstadt.Text = pho.locality
                End If
                If pho.homeWorkType = HomeWorkType.work Then
                    tbPLZ.Text = pho.po
                    tbStrasse.Text = pho.street
                    tbstadt.Text = pho.locality
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub btnuebernahme_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If eingabeIstOk Then
            PersonUbernehmen()
            myGlobalz.sitzung.BeteiligteModus = "neu"
            Dim ee As New winBeteiligteDetail("neu")
            ee.ShowDialog()
        Else
            MsgBox("Die Eingaben sind nicht vollständig. Sie müssen wenigstens einen Namen nennen")
        End If


        e.Handled = True
    End Sub

    Private Sub btnabbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Public Sub PersonUbernehmen()
        Try
            myGlobalz.sitzung.aktPerson.clear()
            With myGlobalz.sitzung.aktPerson
                .clear()
                .Name = tbname.Text
                .Bezirk = ""
                .Vorname = tbVname.Text
                .Bemerkung = tbNotiz.Text
                .Namenszusatz = tbAnrede.Text
                .Anrede = tbAnrede.Text
                .Kontakt.clear()
                .Kontakt.GesellFunktion = tbTitel.Text
                .Kontakt.Bemerkung = "Quelle: Elektronische Visitenkarte"
                .Kontakt.Anschrift.Gemeindename = tbstadt.text
                .Kontakt.Anschrift.Strasse = tbStrasse.Text
                .Kontakt.Anschrift.Hausnr = ""
                .Kontakt.Anschrift.PLZ = (tbPLZ.Text)
                .Kontakt.elektr.Telefon1 = tbTel1.Text
                .Kontakt.elektr.Telefon2 = ""
                .Kontakt.elektr.Fax1 = tbFax1.Text
                .Kontakt.elektr.Fax2 = ""
                .Kontakt.elektr.MobilFon = tbMobil1.Text
                .Kontakt.elektr.Homepage = tbURL.Text
                .Kontakt.elektr.Email = tbEmail.Text
                .Kontakt.Org.Name = tbOrg.Text
                .Kontakt.Org.Zusatz = ""
                .changed_Anschrift = True
            End With
        Catch ex As Exception
            MsgBox("Fehler bei der Übernahme von Daten aus der Vorgangsdatenbank! in PersonAusVorgangsDBUebernehmen" & ex.ToString)
        End Try
    End Sub

    Private Function eingabeIstOk() As Boolean
        If String.IsNullOrEmpty(tbname.Text) Then
            Return False
        End If
        Return True
    End Function

End Class
