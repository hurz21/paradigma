Class MainWindow 

    Private aktbearbeiter As New clsBearbeiter
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        clsBearbeiter.speichern_update(aktbearbeiter.PersonenID, myGlobalz.tempREC, aktbearbeiter)
        refreshTabelle()
    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        loeschen()
    End Sub

    Private Sub btnAdd_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        LeerenEintragHinzufuegen()
        refreshTabelle()
    End Sub

    Private Sub initTablleBearbeiter()
        If myGlobalz.bearbeiterDT IsNot Nothing AndAlso myGlobalz.bearbeiterDT.Rows.Count > 0 Then
            DataGrid1.DataContext = myGlobalz.bearbeiterDT
        Else
            DataGrid1.DataContext = Nothing
        End If
    End Sub
    Private Sub WINadmin_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

        Try
            '  getAllBearbeiter()
            initTablleBearbeiter()
            glob2.nachricht("setDatacontext--------------ende")
        Catch ex As System.Exception
            glob2.nachricht_und_Mbox("fehler in setDatacontext--------------" & ex.ToString)
            'MsgBox(ex.ToString)
        End Try
    End Sub

    Private Sub DataGrid1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles DataGrid1.SelectionChanged
        If DataGrid1.SelectedValue Is Nothing Then Exit Sub
        If DataGrid1.SelectedItem Is Nothing Then Exit Sub
        If DataGrid1.SelectedIndex < 0 Then Exit Sub
        Dim item As DataRowView
        item = CType(DataGrid1.SelectedItem, DataRowView)
        aktbearbeiter = New clsBearbeiter()
        dg2ereignis(item, aktbearbeiter)
    End Sub

    Sub dg2ereignis(ByVal item As DataRowView, ByRef bearbeiter As clsBearbeiter) 'ereig
        bearbeiter.clear()
        bearbeiter.PersonenID = CInt(item("id"))
        bearbeiter.username = (clsDBtools.fieldvalue(item("username"))).ToString
        bearbeiter.Name = clsDBtools.fieldvalue((item("name"))).ToString
        bearbeiter.Vorname = CStr((clsDBtools.fieldvalue(item("vorname"))))
        bearbeiter.Rang = (clsDBtools.fieldvalue(item("rang"))).ToString
        bearbeiter.Rites = CStr((clsDBtools.fieldvalue(item("rites"))))
        bearbeiter.Kontakt.elektr.Telefon1 = CStr((clsDBtools.fieldvalue(item("Telefon"))))
        bearbeiter.Bemerkung = CStr((clsDBtools.fieldvalue(item("abteilung"))))
        bearbeiter.Kontakt.elektr.Email = CStr(clsDBtools.fieldvalue(item("email")))
        Details.DataContext = bearbeiter
    End Sub

    Private Sub LeerenEintragHinzufuegen()
        Dim result As Boolean = clsBearbeiter.leerHinzufuegen(myGlobalz.tempREC)
    End Sub

    Private Sub refreshTabelle()
        callmanagerDAL.glob2.initBearbeiterDT(myGlobalz.bearbeiterDT, myGlobalz.callREC)
        initTablleBearbeiter()
    End Sub
    Sub loeschen()
        If aktbearbeiter.PersonenID < 1 Then
            MsgBox("Bitte wählen Sie einen Eintrag aus! ")
            Exit Sub
        End If
        Dim result As Boolean = clsBearbeiter.IDloeschen(myGlobalz.tempREC, aktbearbeiter.PersonenID)
        refreshTabelle()
    End Sub
End Class
