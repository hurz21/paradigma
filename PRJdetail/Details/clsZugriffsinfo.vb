Imports System.Data

Public Class clsZugriffsinfo
    Private Shared Sub kollegenExtracted(ByVal ctlList As System.Windows.Controls.ListBox)
        ctlList.ItemsSource = myGlobalz.availablePresentationObjects
        TextSearch.SetTextPath(ctlList, "Name")
        '	multicolumnComboBox.ItemsSource = myGlobalz.availablePresentationObjects
        'TextSearch.SetTextPath(Me.multicolumnComboBox, "Name")
    End Sub
    'Shared Sub Map_ref_layers(ByVal ctlList As System.Windows.Controls.ListBox)
    '    'public List<CheckedListItem> AvailablePresentationObjects;
    '    '  DBactionParadigma.initKollegenDT("")
    '    Dim layers$() = clstart.myc.kartengen.aktMap.Vgrund.Split(";"c)
    '    tabAufObjumlegen()
    '    myGlobalz.availablePresentationObjects.Clear()
    '    For i = 0 To layers.GetUpperBound(0)
    '        Dim temp As New clsLayerListItem() With {.Id = CInt((i)),
    '                                                 .Name = CStr(layers(i)),
    '                                                 .IsChecked = True}
    '    Next
    '    kollegenExtracted(ctlList)
    'End Sub

    Shared Sub kollegen(ByVal ctlList As System.Windows.Controls.ListBox)
        'public List<CheckedListItem> AvailablePresentationObjects;
        userTools.initKollegenDT()
        tabAufObjumlegenIDalsString()
        kollegenExtracted(ctlList)
    End Sub

    Shared Sub tabAufObjumlegenIDalsString()
        For i = 0 To myGlobalz.sitzung.tempREC.dt.Rows.Count - 1
            Dim temp As New clsLayerListItem() With {.Id = CInt(myGlobalz.sitzung.tempREC.dt.Rows(i).Item("id")),
                                                     .Titel = CStr(myGlobalz.sitzung.tempREC.dt.Rows(i).Item("titel")),
                                                     .Name = CStr(myGlobalz.sitzung.tempREC.dt.Rows(i).Item("name")),
                                                     .IsChecked = False}
            myGlobalz.availablePresentationObjects.Add(temp)
        Next
    End Sub

    Shared Sub tabAufObjumlegen()
        For i = 0 To myGlobalz.sitzung.tempREC.dt.Rows.Count - 1
            Dim temp As New clsLayerListItem() With {.Id = CInt(myGlobalz.sitzung.tempREC.dt.Rows(i).Item("id")),
                                                     .Name = CStr(myGlobalz.sitzung.tempREC.dt.Rows(i).Item("name")),
                                                     .IsChecked = False}
            myGlobalz.availablePresentationObjects.Add(temp)
        Next
    End Sub

    'Shared Sub WorkflowDatatable_Verlauf(ByVal ctlList As System.Windows.Controls.ListBox)
    '    DB_Oracle_sharedfunctions.initWorkflowDatatable_verlauf()
    '    tabAufObjumlegen()
    '    kollegenExtracted(ctlList)
    'End Sub
    'Shared Sub WorkflowDatatable_Beteiligte(ByVal ctlList As System.Windows.Controls.ListBox)
    '    DB_Oracle_sharedfunctions.initWorkflowDatatable_beteiligte("")
    '    tabAufObjumlegen()
    '    kollegenExtracted(ctlList)
    'End Sub

    Shared Sub WorkflowDatatable_BeteiligteEmails(ByVal ctlList As System.Windows.Controls.ListBox)
        'DBactionParadigma.initWorkflowDatatable_beteiligte(hinweis)
        myGlobalz.sitzung.tempREC.dt.Clear()
        myGlobalz.sitzung.tempREC.dt = myGlobalz.sitzung.beteiligteREC.dt.Copy
       
        BeteiligteEmailstabAufObjumlegen()
        kollegenExtracted(ctlList)
    End Sub

    Shared Function BeteiligteEmailstabAufObjumlegen() As Int16
        'die email wird auf die eigenschaft name gesetzt
        'daher muss email vorhanden sein - sonst erscheint der eintrag nicht in der liste
        Dim iOhneEmail% = 0
        For i = 0 To myGlobalz.sitzung.tempREC.dt.Rows.Count - 1
            Dim temp As New clsLayerListItem() With {.Id = CInt(myGlobalz.sitzung.tempREC.dt.Rows(i).Item("personenid")),
                                                     .Name = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(i).Item("ffemail"))),
                                                     .IsChecked = False}
            If Not String.IsNullOrEmpty(temp.Name) Then
                myGlobalz.availablePresentationObjects.Add(temp)
            Else
                iOhneEmail += 1
            End If
        Next
        nachricht(iOhneEmail.ToString & " einträge ohne emailadresse. werden ignoriert!")
        nachricht(myGlobalz.availablePresentationObjects.Count.ToString & " einträge mit emailadresse!")
        Return CShort(iOhneEmail)
    End Function
End Class
