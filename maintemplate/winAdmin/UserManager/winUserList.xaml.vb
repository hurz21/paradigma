Imports System.Data
Public Class winUserList

    Property QuellUser As New clsBearbeiter
    Property ZielUser As New clsBearbeiter
    Private Sub winUserList_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        userTools.initbearbeiterDT()
        checkumweltamt()
        DataGrid1.DataContext = myGlobalz.sitzung.BearbeiterREC.dt
        e.Handled = True
    End Sub

    Private Sub DataGrid1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DataGrid1.SelectionChanged
        e.Handled = True
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
        Dim Initial = item("INITIAL_").ToString()
        If cbuserauswaehlen.IsChecked Then
            If QuellUser.ID < 1 Then
                QuellUser.ID = CInt(auswahlid)
                QuellUser.Initiale = Initial
                QuellUser.username = beschreibung
            Else
                ZielUser.ID = CInt(auswahlid)
                ZielUser.Initiale = Initial
                ZielUser.username = beschreibung
            End If
            tbUserAuswaehlErgebnis.Text = makeSummary()
            If QuellUser.ID > 0 And ZielUser.ID > 0 Then
                btnUserAuswahlExe.IsEnabled = True
            End If
            Exit Sub
        End If
        Dim testbearbeiter As New clsBearbeiter
        testbearbeiter.ID = CInt(auswahlid)
        testbearbeiter.username = (beschreibung)
        testbearbeiter.Initiale = (Initial)
        'If userTools.initBearbeiterByUserid_ausParadigmadb(testbearbeiter, "BEARBEITERID", auswahlid) Then
        If userTools.initBearbeiterByUserid_ausParadigmadb(testbearbeiter) Then
            Dim edit As New winUserDetail(testbearbeiter)
            edit.ShowDialog()
            userTools.initbearbeiterDT()
        Else
            MsgBox("not found")
        End If
        DataGrid1.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Function makeSummary() As String
        Return "Von: " & QuellUser.username & "(" & QuellUser.Initiale & ", " & QuellUser.ID & ")  " &
                         "Nach: " & ZielUser.username & "(" & ZielUser.Initiale & ", " & ZielUser.ID & ")  "
    End Function

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
        Dim realDepartment As String = ""
        For i = 0 To myGlobalz.sitzung.BearbeiterREC.dt.Rows.Count - 1
            Dim lokdt As New DataTable
            Dim filter As String = myGlobalz.sitzung.BearbeiterREC.dt.Rows(i).Item("USERNAME").ToString
            If filter.IsNothingOrEmpty Then Continue For
            filter = LIBgemeinsames.clsString.umlaut2ue(filter)
            lokdt = JFactiveDirectory.clsActiveDir.sucheperson(filter)
            If Not CBool(myGlobalz.sitzung.BearbeiterREC.dt.Rows(i).Item("AKTIV")) Then Continue For
            If lokdt Is Nothing OrElse lokdt.Rows.Count < 1 Then
                ' dgPersonal.DataContext = Nothing
            Else
                Select Case JFactiveDirectory.umweltamt.istImUmweltamt(lokdt, realDepartment)
                    Case -1
                        namen = namen & Environment.NewLine & myGlobalz.sitzung.BearbeiterREC.dt.Rows(i).Item("NACHNAME").ToString & " (" & realDepartment & ") "
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
            '  filter = LIBgemeinsames.clsString.umlaut2ue(filter)
            lokdt = JFactiveDirectory.clsActiveDir.sucheperson(filter)
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

    Private Sub btnuserauswaehlen_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True

    End Sub

    Private Sub btnUserAuswahlExe_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim sql As String
        sql = " update t41 set bearbeiterid=" & ZielUser.ID & " where bearbeiterid=" & QuellUser.ID & ";" & Environment.NewLine
        sql = sql & " update t41 set initial=" & ZielUser.Initiale & " where bearbeiterid=" & QuellUser.ID
        Dim mesres As New MessageBoxResult
        mesres = MessageBox.Show("Wollen Sie dies wirklich umsetzen ? " & Environment.NewLine &
                        "  " & Environment.NewLine &
                        makeSummary() & Environment.NewLine &
                        "" & Environment.NewLine &
                        sql & Environment.NewLine, "Vorgänge wirklich übertragen????", MessageBoxButton.YesNo, MessageBoxImage.Question)
        If mesres = MessageBoxResult.Yes Then
            Dim anzahl As Integer = NSBearbeiter.BearbeiterTools.VorgaengeUebertragen(sql)
            MessageBox.Show("Es wurden " & anzahl & " Vorgänge übertragen")

        Else
            Exit Sub
        End If
    End Sub

    'Private Sub DataGrid1_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

    'End Sub
End Class
