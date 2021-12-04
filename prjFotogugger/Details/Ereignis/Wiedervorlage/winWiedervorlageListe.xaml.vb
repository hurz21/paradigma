Imports System.Data
Partial Public Class Window_Wiedervorlage
        Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
        mnuMenu.Background = myGlobalz.GetSecondBackground() 
    End Sub


    Private Sub Window1_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.DBWiedervorlageREC.dt, "faelligSymbol", "System.Int16")
        einfaerbenDerRowsvorbereiten()
        clsDBtools.TabellenKopfausgeben(myGlobalz.sitzung.DBWiedervorlageREC.dt)
        DataContext = myGlobalz.sitzung.DBWiedervorlageREC.dt
        dgWVliste.CanUserAddRows = False
          gastLayout()
        e.Handled=true
    End Sub

    Shared Sub einfaerbenDerRowsvorbereiten()
        For i = 0 To myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows.Count - 1
            Dim lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem As Object = myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("datum")
            Dim lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem1 As Date = CType(lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem, Date)
            If lMyGlobalzsitzungDBWiedervorlageRECdtRowsItem1 < Now Then
                If CType(myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("erledigtAm"), Date) > CLstart.mycsimple.MeinNULLDatumAlsDate Then
                    myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 2
                Else
                    myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 1
                End If

            Else
                myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows(i).Item("faelligSymbol") = 0
            End If
        Next
    End Sub



    Private Sub dgWVliste_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles dgWVliste.SelectionChanged
        Try
            e.Handled = True
            Dim item As DataRowView = CType(dgWVliste.SelectedItem, DataRowView)
            'Dim item = dg.SelectedItem
            If item Is Nothing Then Return
            Dim auswahlid$ = item(0).ToString()
            ' nachricht_und_Mbox(auswahlid$)
            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)
            'Me.Close()

            Me.Close()
            glob2.editVorgang(myGlobalz.sitzung.aktVorgangsID)
            dgWVliste.SelectedItem = Nothing
        Catch ex As Exception
            nachricht_und_Mbox("" ,ex)
        End Try
    End Sub

    Private Sub abbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles abbruch.Click
        Me.close()
    End Sub



    Private Sub WVExcel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        Dim handcsv As New libcsvausgabe.clsCSVausgaben("Wiedervorlagen", myGlobalz.sitzung.DBWiedervorlageREC.dt, 0,"",CLstart.mycSimple.Paradigma_local_root,CLstart.mycSimple.enc)
        nachricht(" exportfile$ = " & handcsv.CscDateiAusgeben())
        handcsv.start()
         handcsv.Dispose
        e.Handled=true
    End Sub
End Class
