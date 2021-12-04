Partial Public Class WINdokumentMetaEdit

    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If chkbRevisionssicher.IsChecked Then
            myGlobalz.sitzung.aktDokument.revisionssicher = True
        Else
            myGlobalz.sitzung.aktDokument.revisionssicher = False
        End If
        myGlobalz.sitzung.aktDokument.Beschreibung = FileArchivTools.pruefeBeschreibung(myGlobalz.sitzung.aktDokument.Beschreibung)
        '    Dim result As Integer = DokumentenArchiv.updateAktuellesDokument(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.istVeraltet, myGlobalz.sitzung.aktDokument.Beschreibung, myGlobalz.sitzung.aktDokument.revisionssicher)
        Dim result As Integer = DokArcTools.dokUpdate.execute(myGlobalz.sitzung.aktDokument.DocID,
                                                              myGlobalz.sitzung.aktDokument.istVeraltet,
                                                              myGlobalz.sitzung.aktDokument.Beschreibung,
                                                              myGlobalz.sitzung.aktDokument.revisionssicher,
                                                              myGlobalz.sitzung.aktDokument.Filedatum,
                                                              myGlobalz.sitzung.aktDokument.EXIFlat,
                                                              myGlobalz.sitzung.aktDokument.EXIFlon)
        If result > 0 Then
            ' MsgBox("Speichern erfolgreich. Formular schließen")
            clstart.myc.aLog.komponente = "Dokumente" : clstart.myc.aLog.aktion = String.Format("{0} {1}: metadaten geaendert", myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.DateinameMitExtension) : clstart.myc.aLog.log()
        Else
            MsgBox("Speichern nicht erfolgreich. Formular schließen")
        End If
        Me.Close()
    End Sub

    'Private Sub btnloeschen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    e.Handled = True
    '    Me.Close()
    'End Sub

    Private Sub WINdokumentMetaEdit_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        btnSpeichern.IsEnabled = False
        clsParadigmaRechte.buttons_schalten(btnSpeichern, btnSpeichern)
        If myGlobalz.sitzung.aktDokument.revisionssicher Then
            chkbRevisionssicher.IsEnabled = False
            TextBox1.IsEnabled = False
        End If
        detailsTools.initErgeinistypCombo(Me, "dokument_schlagworte.xml", "XMLSourceComboBoxdokumentschlagworte")
        If String.IsNullOrEmpty(TextBox1.Text) Then cmbTitelVorschlag.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox1.TextChanged
        glob2.istTextzulang(240, TextBox1)
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub chkbRevisionssicher_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkbRevisionssicher.Checked
        glob2.schliessenButton_einschalten(btnSpeichern)
        MsgBox("Revisionssicherheit läßt sich nicht rückgängig machen")
    End Sub

    Private Sub cmbTitelVorschlag_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            If cmbTitelVorschlag.SelectedValue Is Nothing Then Exit Sub
            Dim item As String = CType(cmbTitelVorschlag.SelectedValue, String).Trim
            If Not String.IsNullOrEmpty(item) AndAlso item.Trim <> "-" Then
                'es wurde was ausgewählt
                If String.IsNullOrEmpty(myGlobalz.sitzung.aktDokument.Beschreibung) Then
                    'es steht noch nichts da
                    TextBox1.Text = item
                    cmbTitelVorschlag.SelectedValue = ""
                Else
                    'es steht schon was da  , wird also hinzugefügt
                    TextBox1.Text = item & " / " & myGlobalz.sitzung.aktDokument.Beschreibung
                End If
            Else
                'es wurde nichts ausgewählt
            End If
            e.Handled = True
        Catch ex As Exception
            nachricht("cmbTitelVorschlag_SelectionChanged" & ex.ToString)
        End Try
        e.Handled = True
    End Sub
End Class
