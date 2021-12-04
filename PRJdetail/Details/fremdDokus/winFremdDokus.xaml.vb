Imports System.ComponentModel
Imports System.Data

Public Class winFremdDokus
    Private presdoks As New List(Of clsPresDokumente)
    Private LOKreignisDokListe As New List(Of clsEreignisDok)
    Private Sub winFremdDokus_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        'refreshDokus
        dgVerwandte.DataContext = myGlobalz.sitzung.VerwandteDT
        ShowHistory()
    End Sub
    Sub dgVorgangDokumente_SelectionChanged_1Extracted()
        If detailsTools.istDateiNameInordnung(myGlobalz.sitzung.aktDokument.DateinameMitExtension) Then
            myGlobalz.Arc.einzeldokument_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID,
                                                  myGlobalz.sitzung.aktDokument)


            myGlobalz.sitzung.aktDokument.nurzumlesen = True
            DokArc.Archiv_aktiviere_dokument(myGlobalz.sitzung.aktDokument, True, True, 0)
        Else
            MsgBox(glob2.getMsgboxText("DateiNameIstNichtInOrdnung", New List(Of String)(New String() {myGlobalz.sitzung.aktDokument.DateinameMitExtension})
                                      ))
        End If
    End Sub

    Private Sub dgVorgangDokumente_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        If dgVorgangDokumente.SelectedItem Is Nothing Then Exit Sub
        Dim item As New clsPresDokumente
        Try
            item = CType(dgVorgangDokumente.SelectedItem, clsPresDokumente)

        Catch ex As Exception
            nachricht(ex.ToString)
            Exit Sub
        End Try
        'If auswahlspalteDokus.Visibility = Windows.Visibility.Visible Then
        If cbKopiermodus.IsChecked Then
            myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            myGlobalz.sitzung.aktDokument = CType(item.Clone, Dokument)
            DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID)
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)

            Dim ausgabeVerzeichnis As String = ""
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
            myGlobalz.Arc.einzeldokument_auschecken(ausgabeVerzeichnis,
                                                  myGlobalz.sitzung.aktDokument)



            Dim NumDir As String = ""
            'Dim NumDir As String = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)

            If myGlobalz.Arc.checkINDoku(myGlobalz.sitzung.aktDokument.FullnameCheckout,
                                     myGlobalz.sitzung.aktDokument.Beschreibung,
                                     0,
                                     False,
                                      myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
                                     NumDir,
                                     myGlobalz.sitzung.aktVorgangsID,
                                     False, Now, myGlobalz.sitzung.aktDokument.DocID,
                                     myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                     myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID) Then



            End If
            MessageBox.Show("Datei " & myGlobalz.sitzung.aktDokument.DateinameMitExtension & " wurde in den aktuellen Vorgang kopiert!", myGlobalz.sitzung.aktDokument.DateinameMitExtension & " nach: " & myGlobalz.sitzung.aktVorgangsID)
            Else
                myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            myGlobalz.sitzung.aktDokument = CType(item.Clone, Dokument)
            DokArc.setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID)
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)


            Dim ausgabeVerzeichnis As String = ""
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
            myGlobalz.Arc.einzeldokument_auschecken(ausgabeVerzeichnis,
                                                  myGlobalz.sitzung.aktDokument)
            dgVorgangDokumente_SelectionChanged_1Extracted()
        End If
    End Sub


    Sub refreshDokus(vid As Integer)
        Try
            Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(vid), "keinefotos", True, 0) ' nach myGlobalz.Arc.ArcRec
            If bresult Then
                presdoks = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
                detailsTools.thumbNailsHinzuFuegen(presdoks, initP.getValue("Haupt.ThumbNailsRoot"), LOKreignisDokListe)

                dgVorgangDokumente.DataContext = presdoks 'myGlobalz.Arc.vorgangDocDt 
            Else
                presdoks = Nothing
                myGlobalz.Arc.vorgangDocDt = Nothing
                dgVorgangDokumente.DataContext = Nothing
            End If
        Catch ex As Exception
            nachricht("fehler in refreshDokumente: " ,ex)
        End Try
    End Sub

    Private Sub btnStart_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        refreshDokus(CInt(tbfremdVorgang.Text))

    End Sub

    Private Sub btnClear_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        presdoks = Nothing
        myGlobalz.Arc.vorgangDocDt = Nothing
        dgVorgangDokumente.DataContext = Nothing
        e.Handled = True
    End Sub

    Private Sub btnabbruchclick(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
        e.Handled = True
    End Sub

    Private Sub dgVerwandte_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            e.Handled = True
            If dgVerwandte.SelectedItem Is Nothing Then Exit Sub
            Dim item As DataRowView = CType(dgVerwandte.SelectedItem, DataRowView)

            'geaenderteStammdatenSpeichern()
            Dim vid$ = item("FREMDVORGANGSID").ToString
            Dim koppelingsid As Integer = CInt(item("ID").ToString)
            'Verwandte_verarbeiten(koppelingsid, CInt(vid$), item("Titel").ToString)
            tbfremdVorgang.Text = vid
            refreshDokus(CInt(tbfremdVorgang.Text))
        Catch ex As Exception
            nachricht("Sie haben in eine leere Zeile geklickt. Bitte versuchen Sie es nochmal." ,ex)
        End Try
    End Sub

    Private Sub dgHistory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Try
            Dim item As CLstart.HistoryKookie.HistoryItem
            Try
                item = CType(dgHistory.SelectedItem, CLstart.HistoryKookie.HistoryItem)
            Catch ex As Exception
                e.Handled = True
                Exit Sub
            End Try
            item = CType(dgHistory.SelectedItem, CLstart.HistoryKookie.HistoryItem)
            If item Is Nothing Then
                item = CType(dgHistory.SelectedItem, CLstart.HistoryKookie.HistoryItem)
                If item Is Nothing Then Return
            End If
            tbfremdVorgang.Text = CType(item.ID, String)
            dgVorgangDokumente.DataContext = Nothing
            refreshDokus(item.ID)

            e.Handled = True

        Catch ex As Exception
            nachricht_und_Mbox("" ,ex)
        End Try
    End Sub
    Private Sub ShowHistory()
        Dim collHistory As List(Of CLstart.HistoryKookie.HistoryItem)
        collHistory = CLstart.HistoryKookie.VerlaufsCookieLesen.exe(myGlobalz.ClientCookieDir & "verlaufscookies")
        dgHistory.DataContext = collHistory
    End Sub

    Private Sub winFremdDokus_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub
End Class
