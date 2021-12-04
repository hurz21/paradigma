Namespace NSBearbeiter
    Public Class BearbeiterTools
        Public Shared Function istHauptbearbeiterBekannt(usi As clsBearbeiter) As Boolean
            l("")
            Dim bekannt As Boolean
            Dim sql = String.Format("select * from  " & CLstart.myViewsNTabs.tabBearbeiter & "  where " &
                                "  bearbeiterid={0}  ", usi.ID)
            Dim errorinfo As String = ""
            Dim hinweis As String
            errorinfo = errorinfo & " sql: " & sql
            Dim loDT As System.Data.DataTable = getDT4Query(sql, myGlobalz.sitzung.BearbeiterREC, hinweis)
            errorinfo = errorinfo & "nach lokrec "
            If loDT.Rows.Count < 1 Then
                nachricht("B FEHLER User ist unbekannt lokrec.Rows.Count < 1: " & usi.username & vbCrLf &
                          " Bitte informieren sie den Admin. errorinfo: " & errorinfo)
                Return False
            End If
            bearbeiterDTzuOBJEKT(usi, loDT, 0)
            If usi.Status < 1 Then
                'MsgBox("User " & usi.username & " ist inaktiv: bitte bei FD-Leitung melden")
                ' Return False
            End If
            usi.Initiale = usi.getInitial
            ' usi.Kassenkonto = clsDBtools.fieldvalue(lokrec.Rows(0).Item("KASSENKONTO"))
            Return True
        End Function

        'Shared Sub getuserName()
        '    userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter,
        '                                                           "INITIAL_",
        '                                                           myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)

        '               userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter,
        '                                                           "INITIAL_",
        '                                                           myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.getInitial)
        'End Sub

    End Class
End Namespace
