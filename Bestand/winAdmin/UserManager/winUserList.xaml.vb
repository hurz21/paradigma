Imports System.Data
Public Class winUserList
    Private Sub winUserList_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        userTools.initbearbeiterDT()
        checkumweltamt()
        DataGrid1.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
        e.Handled = True
    End Sub

    Private Sub DataGrid1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DataGrid1.SelectionChanged
        Dim item As DataRowView
        Try
            item = CType(DataGrid1.SelectedItem, DataRowView)
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        item = CType(DataGrid1.SelectedItem, DataRowView)
        'Dim item = dgStamm.SelectedItem
        If item Is Nothing Then
            item = CType(DataGrid1.SelectedItem, DataRowView)
            If item Is Nothing Then Return
        End If

        Dim auswahlid$ = item("BEARBEITERID").ToString()
        Dim beschreibung$ = item("USERNAME").ToString()
        Dim testbearbeiter As New clsBearbeiter
        If userTools.initBearbeiterByUserid_ausParadigmadb(testbearbeiter, "BEARBEITERID", auswahlid) Then
            Dim edit As New winUserDetail(testbearbeiter)
            edit.ShowDialog()
            userTools.initbearbeiterDT()
        Else
            MsgBox("not found")
        End If
        DataGrid1.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub btnNeuerUserAnlegen_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        '  Dim testbearbeiter As New clsBearbeiter
        Dim edit As New winUserDetail()
        edit.ShowDialog()
        userTools.initbearbeiterDT()
        e.Handled = True
    End Sub

    Private Sub btnabbruch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub btnRefresh_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        userTools.initbearbeiterDT()
        DataGrid1.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
        e.Handled = True
    End Sub

    Private Sub checkumweltamt()
        Dim username As String = ""
        Dim summe As String = ""
        Dim namen As String = ""
        For i = 0 To myGlobalz.sitzung.BearbeiterREC.dt.Rows.Count - 1
            Dim lokdt As New DataTable
            Dim filter As String = myGlobalz.sitzung.BearbeiterREC.dt.Rows(i).Item("USERNAME").ToString
            filter = clsString.umlaut2ue(filter)
            lokdt = clsActiveDir.sucheperson(filter)
            If Not CBool(myGlobalz.sitzung.BearbeiterREC.dt.Rows(i).Item("AKTIV")) Then Continue For
            If lokdt Is Nothing OrElse lokdt.Rows.Count < 1 Then
                ' dgPersonal.DataContext = Nothing
            Else
                Select Case JFactiveDirectory.umweltamt.istImUmweltamt(lokdt)
                    Case -1
                        namen = namen & Environment.NewLine & myGlobalz.sitzung.BearbeiterREC.dt.Rows(i).Item("NACHNAME").ToString
                    Case 0
                        ' MsgBox("Der Bearbeiter  " & filter & " ist noch nicht erfasst")
                    Case 1
                        'alles ok
                End Select
            End If
        Next
        If namen.Trim.Length > 0 Then
            MsgBox("Die Bearbeiter " & namen & Environment.NewLine &
                   " sind laut ActiveDirectory der IT nicht Teil des Umweltamtes. " & Environment.NewLine &
                   " Daher haben sie keine vollen Rechte. (z.B. Eigentümerabfrage)" & Environment.NewLine &
                   " Wenn gewünscht bitte Email an IT, mit der Bitte um Zuweisung der Personen an den FD Umwelt im ActiveDirectory.")
        End If
    End Sub
    Private Sub checkAlleumweltaemtler()
        Dim username As String = ""
        Dim summe As String = ""
        Dim namen As String = ""
        For i = 0 To myGlobalz.sitzung.BearbeiterREC.dt.Rows.Count - 1
            Dim lokdt As New DataTable
            Dim filter As String = "umwelt"
            '  filter = clsString.umlaut2ue(filter)
            lokdt = clsActiveDir.sucheperson(filter)
           ' If Not CBool(myGlobalz.sitzung.BearbeiterREC.dt.Rows(i).Item("AKTIV")) Then Continue For
            If lokdt Is Nothing OrElse lokdt.Rows.Count < 1 Then
                ' dgPersonal.DataContext = Nothing
            Else
                'Select Case JFactiveDirectory.umweltamt.istImUmweltamt(lokdt)
                '    Case -1
                        namen = namen & Environment.NewLine & lokdt.Rows(i).Item("SN").ToString
                '    Case 0
                '        ' MsgBox("Der Bearbeiter  " & filter & " ist noch nicht erfasst")
                '    Case 1
                '        'alles ok
                'End Select
            End If
        Next
        If namen.Trim.Length > 0 Then
            MsgBox("Die Bearbeiter " & namen & Environment.NewLine &
                   " sind laut ActiveDirectory der IT Teil des Umweltamtes. " & Environment.NewLine &
                   " Bitte auf Korrektheit prüfen.")
        End If
    End Sub

    Private Sub btnallechecken_Click(sender As Object, e As RoutedEventArgs)
        checkAlleumweltaemtler()
        e.Handled = True
    End Sub
End Class
