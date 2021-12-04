Imports paradigmaDetail

Partial Public Class WINdokumentMetaEdit
    Public dateigroesseinMB As String = ""


    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub btnSpeichern_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If myGlobalz.sitzung.aktDokument.revisionssicher Then
            MsgBox(glob2.getMsgboxText("infotextRevisionssicherheit", New List(Of String)(New String() {})))
            Exit Sub
        End If
        If chkbRevisionssicher.IsChecked Then
            myGlobalz.sitzung.aktDokument.revisionssicher = True
        Else
            myGlobalz.sitzung.aktDokument.revisionssicher = False
        End If
        myGlobalz.sitzung.aktDokument.Beschreibung = FileArchivTools.pruefeBeschreibung(myGlobalz.sitzung.aktDokument.Beschreibung)

        Dim result As Integer = DokArcTools.dokUpdate.execute(myGlobalz.sitzung.aktDokument.DocID,
                                                              myGlobalz.sitzung.aktDokument.istVeraltet,
                                                              myGlobalz.sitzung.aktDokument.Beschreibung,
                                                              myGlobalz.sitzung.aktDokument.revisionssicher,
                                                              myGlobalz.sitzung.aktDokument.Filedatum,
                                                              myGlobalz.sitzung.aktDokument.EXIFlat,
                                                              myGlobalz.sitzung.aktDokument.EXIFlon)
        If result > 0 Then
            ' MsgBox("Speichern erfolgreich. Formular schließen")
            CLstart.myc.aLog.komponente = "Dokumente" : CLstart.myc.aLog.aktion = String.Format("{0} {1}: metadaten geaendert a", myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.DateinameMitExtension) : CLstart.myc.aLog.log()
        Else
            MsgBox("Speichern nicht erfolgreich. Formular schließen")
        End If

        Me.Close()
    End Sub
    Private Sub WINdokumentMetaEdit_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        btnSpeichern.IsEnabled = False
        chkbRevisionssicher.IsEnabled = False
        clsParadigmaRechte.buttons_schalten(btnSpeichern, btnSpeichern)
        dateigroesseinMB = dokScanPDF.GetFileSize(myGlobalz.sitzung.aktDokument.FullnameImArchiv)
        tbsize.Text = dateigroesseinMB
        If myGlobalz.sitzung.aktDokument.revisionssicher Then
            chkbRevisionssicher.IsEnabled = True
            TextBox1.IsEnabled = False
            cmbTitelVorschlag.IsEnabled = False
        End If
        detailsTools.initErgeinistypCombo(Me, "dokument_schlagworte.xml", "XMLSourceComboBoxdokumentschlagworte")
        If String.IsNullOrEmpty(TextBox1.Text) Then cmbTitelVorschlag.IsDropDownOpen = True
        e.Handled = True
    End Sub

    Private Function getDateigroesse(fullpath As String) As Integer
        Dim size As Integer = 0
        Dim fi As IO.FileInfo
        Try
            l(" MOD getDateigroesse anfang")
            fi = New IO.FileInfo(fullpath)
            size = CInt(fi.Length)
            l(" MOD getDateigroesse ende")
        Catch ex As Exception
            l("Fehler in getDateigroesse: ", ex)
        End Try
    End Function

    Private Sub TextBox1_TextChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.TextChangedEventArgs) Handles TextBox1.TextChanged
        glob2.istTextzulang(240, TextBox1)
        glob2.schliessenButton_einschalten(btnSpeichern)
    End Sub

    Private Sub chkbRevisionssicher_Checked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles chkbRevisionssicher.Checked
        glob2.schliessenButton_einschalten(btnSpeichern)
        MsgBox("Revisionssicherheit läßt sich nicht rückgängig machen")
    End Sub

    Private Sub cmbTitelVorschlag_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        e.Handled = True
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
        Catch ex As Exception
            nachricht("cmbTitelVorschlag_SelectionChanged", ex)
        End Try
        e.Handled = True
    End Sub

    Private Sub btnabbruch_Click(sender As Object, e As RoutedEventArgs)

    End Sub
End Class
