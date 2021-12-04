Imports System.Data

Partial Public Class WINzahlungenListe

    Private Sub WINzahlungenListe_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        ZahlungToolsNs.alleZahlungen.execute()
        dgZahlungen.DataContext = myGlobalz.sitzung.tempREC.dt
        dgZahlungen.CanUserAddRows = False
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
        Me.Close()
    End Sub


    Private Sub dgZahlungen_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgZahlungen.SelectionChanged
        Try
            Dim item As DataRowView = CType(dgZahlungen.SelectedItem, DataRowView)
            'Dim item = dg.SelectedItem
            If item Is Nothing Then Return
            Dim auswahlid$ = item("vorgangsid").ToString()
            ' nachricht_und_Mbox(auswahlid$)
            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            dgZahlungen.SelectedItem = Nothing
            e.Handled = True
            Me.Close()
            glob2.editVorgang(CInt(auswahlid))

        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Private Sub EreignisExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        ' clsZAHLUNGDB_Mysql.ExcelausgabeExtracted()
        ZahlungToolsNs.ExcelausgabeExtracted.execute()
    End Sub
End Class
