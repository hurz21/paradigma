Public Class Win_punktMitUmkreis
    Property anyChange As Boolean
    Property _minimapKoordinatenText$
    Private Sub Window_RB_Adresse_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        Dim red As MessageBoxResult
        If btnSpeichern.IsEnabled Then
            red = MessageBox.Show("Sie haben Daten in dieser Maske geändert! " & vbCrLf &
                  "Wenn Sie diese Änderungen " & vbCrLf &
                  " - prüfen und ggf. speichern möchten wählen Sie 'JA'" & vbCrLf &
                  " - verwerfen möchten wählen Sie 'Nein'" & vbCrLf &
                  "Prüfen und abspeichern ?",
                  "Ereignisdetails", _
                  MessageBoxButton.YesNo,
                  MessageBoxImage.Exclamation,
                  MessageBoxResult.OK)
            If Not red = MessageBoxResult.No Then
                'btnSpeichernEreignis.IsEnabled = False
                e.Cancel = True
            End If
        End If
        DialogResult = If(anyChange, True, False)
    End Sub

    Sub New(ByVal minimapKoordinatenText$)
        InitializeComponent()
        _minimapKoordinatenText$ = minimapKoordinatenText$
    End Sub

    Private Sub Win_punktMitUmkreis_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initFunktionCombo()
        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            myGlobalz.sitzung.aktPMU.clear()
            myGlobalz.sitzung.aktPMU.Radius = 100
            btnLoeschen.IsEnabled = False
            btnSpeichern.IsEnabled = False
            koordinateAusMiniMapUebernehmen()
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            btnLoeschen.IsEnabled = True
            btnSpeichern.IsEnabled = False
            btnWindrose.IsEnabled = True
        End If
        anyChange = False
        cmbFunktionsvorschlaege.IsDropDownOpen = True
        Title = StammToolsNs.setWindowTitel.exe(myGlobalz.sitzung.raumbezugsmodus, "Raumbezug: Punkt mit Umkreis")
    End Sub

    Sub initFunktionCombo()
        Dim existing As XmlDataProvider = TryCast(Me.Resources("XMLSourceComboBoxRBfunktion"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\RBfunktion.xml")
    End Sub

    Sub koordinateAusMiniMapUebernehmen()
        Try
            If String.IsNullOrEmpty(_minimapKoordinatenText) Then Exit Sub
            Dim a$() = _minimapKoordinatenText$.Split(","c)
            If a.Length > 0 Then
                myGlobalz.sitzung.aktPMU.punkt.X = CInt(a(0))
                myGlobalz.sitzung.aktPMU.punkt.Y = CInt(a(1))
                tbNachricht.Text = "Die Koordinaten wurden aus dem letzten Klickpunkt der Minimap übernommen."
                anyChange = True
            End If
        Catch ex As Exception
            nachricht("Fehler bei koordinateAusMiniMapUebernehmen: " & vbCrLf & ex.ToString)
        End Try
    End Sub

    Private Sub btnLoeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ParaUmkreisTools.loeschenAktPMU()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        speichernPMU()
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        btnSpeichern.IsEnabled = False
        Me.Close()
        e.Handled = True
    End Sub

    Sub speichernPMU()
        Try
            If CInt(myGlobalz.sitzung.aktPMU.Status) = 1 Then
                MsgBox("Es handelt sich um einen Punkt eines Verwandte Vorgangs. Er kann nicht geändert werden!")
                Exit Sub
            End If
            myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Umkreis
            If Not istEingabe_vorhanden() Then Exit Sub
            If Not ParaUmkreisTools.ParaPunktLiegtImKreisOffenbach(myGlobalz.sitzung.aktPMU.punkt, CLstart.myc.globalrange) Then
                MsgBox("Hinweis: Der Punkt liegt nicht im Kreis Offenbach!" & vbCrLf & "Er wird trotzdem gespeichert.")
            End If
            If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
                ParaUmkreisTools.Umkreis_Neu()
                btnSpeichern.IsEnabled = False
                Me.Close()
            End If
            If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
                Umkreis_Edit(CInt(myGlobalz.sitzung.aktPMU.SekID))
                btnSpeichern.IsEnabled = False
            End If
            myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
            'myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = Now
            'glob2.EDIT_VorgangStamm_2DBOk()
                 detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
        Catch ex As Exception
            nachricht("fehler in speichernPMU: " & ex.ToString)
        End Try
    End Sub

    Private Sub Umkreis_Edit(ByVal sekid%)
        'todo änderung beim raumbezug muss auch in die datenbank

        ' If ParaUmkreisTools.RB_Umkreis_abspeichern_Edit(sekid%) > 0 Then
        Dim anzahl As Integer = ParaUmkreisTools.umkreisEdit_alleDB(sekid%)
        If anzahl > 0 Then
            btnSpeichern.IsEnabled = False
            myGlobalz.sitzung.aktPMU.defineAbstract()
            DBraumbezug_Mysql.defineBBOX(myGlobalz.sitzung.aktPMU.Radius, myGlobalz.sitzung.aktPMU)

            RBtoolsns.Raumbezug_edit_alleDB.execute(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.aktPMU)

            ' DBraumbezug_Mysql.Raumbezug_abspeichern_Edit(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.aktPMU)
            ParaUmkreisTools.RB_Umkreis_append_PointShapeFile()

        Else
            nachricht("Problem beim Abspeichern!")
        End If
    End Sub

    Function istEingabe_vorhanden() As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPMU.punkt.X.ToString) OrElse myGlobalz.sitzung.aktPMU.punkt.X < 1000 Then
            MsgBox("Bitte Rechtswert eingeben")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktPMU.punkt.X) Then
            MsgBox("Bitte nur Zahlen >0 für den Rechtswert")
            Return False
        End If

        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPMU.punkt.Y.ToString) OrElse myGlobalz.sitzung.aktPMU.punkt.Y < 1000 Then
            MsgBox("Bitte Hochwert eingeben")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktPMU.punkt.Y) Then
            MsgBox("Bitte nur Zahlen >0  für den Hochwert")
            Return False
        End If
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPMU.Radius.ToString) OrElse myGlobalz.sitzung.aktPMU.Radius < 1 Then
            MsgBox("Bitte Radius eingeben")
            Return False
        End If
        If Not IsNumeric(myGlobalz.sitzung.aktPMU.RaumbezugsID) Then
            MsgBox("Bitte nur Zahlen >0  für den Radius")
            Return False
        End If
        Return True
    End Function

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox1.TextChanged
        anyChange = True
        If btnWindrose IsNot Nothing Then btnWindrose.IsEnabled = True
        If String.IsNullOrEmpty(TextBox1.Text) Then Exit Sub
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(20, TextBox1)
    End Sub

    Private Sub TextBox2_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox2.TextChanged
        anyChange = True
        If btnWindrose IsNot Nothing Then btnWindrose.IsEnabled = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(20, TextBox2)
    End Sub

    Private Sub TextBox3_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox3.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(20, TextBox3)
    End Sub


    Private Sub tbBeschreibung_TextChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles tbBeschreibung.TextChanged
        anyChange = True
        glob2.schliessenButton_einschalten(btnSpeichern)
        glob2.istTextzulang(200, TextBox3)
    End Sub

    Private Sub btnWindrose_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        adressToolsUI.windRoseAnzeigen(myGlobalz.sitzung.aktPMU.punkt.X, myGlobalz.sitzung.aktPMU.punkt.Y)
    End Sub

    Private Sub cmbFunktionsvorschlaege_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbFunktionsvorschlaege.SelectedItem Is Nothing Then Exit Sub
        'Dim myvali$ = CStr(cmbFunktionsvorschlaege.SelectedValue)
        Dim myvalx = CType(cmbFunktionsvorschlaege.SelectedItem, System.Xml.XmlElement)
        Dim myvals$ = myvalx.Attributes(1).Value.ToString
        If tbBeschreibung IsNot Nothing Then tbBeschreibung.Text = myvals
    End Sub
End Class
