
Public Class ds1Tools
    Public Shared Sub bildeGruppenString(ByRef summe As String, trenn As String, ByVal vorgangsREC As IDB_grundfunktionen)
        For i = 0 To vorgangsREC.dt.Rows.Count - 1
            summe = summe & CStr(vorgangsREC.dt.Rows(i).Item(0))
            If i <> vorgangsREC.dt.Rows.Count - 1 Then
                summe = summe & trenn
            End If
        Next
    End Sub


    Public Shared Sub GetGruppenDT4Gruppennummer(ByVal gruppennummer As String, ByVal vorgangsREC As IDB_grundfunktionen)
        vorgangsREC.mydb.SQL = "select b.username,g.beschreibung from  " & CLstart.myViewsNTabs.tabDS_USER2GRUPPE & "  ug," & CLstart.myViewsNTabs.tabBearbeiter & " b, " & CLstart.myViewsNTabs.tabDS_Gruppen & "  g " &
                                                                       " where ug.gruppenid=" & gruppennummer &
                                                                       " and ug.bearbeiterid=b.bearbeiterid " &
                                                                       " and b.aktiv=1" &
                                                                       " and g.gruppenid=ug.gruppenid " &
                                                                       " order by username"
        Dim hinweis As String = vorgangsREC.getDataDT()
    End Sub

    Public Shared Function istFachdienstLeitung(ByVal vorgangsREC As IDB_grundfunktionen, username As String, trenn As String) As Boolean
        Dim summe As String = ""
        'GetGruppenDT4Gruppennummer("9")
        ds1Tools.GetGruppenDT4Gruppennummer(CStr(9), vorgangsREC)
        ds1Tools.bildeGruppenString(summe, trenn, vorgangsREC)
        If LIBgemeinsames.clsString.isinarray(summe, CStr(username), trenn) Then
            Return True
        Else
            Return False
        End If
    End Function
End Class
