
Imports System.Data

Namespace LIBstammdatenCRUD

    Public Class clsStammdatenTool
        Public Shared Function UNION_SQL_VST_erzeugen(ByVal anyDT As DataTable, ByVal ausgabespaltenNr%, ByVal idspalte$) As String
            Try
                'SELECT * FROM  " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & "  s where v.vorgangsid=s.vorgangsid  and v.vorgangsid=1356
                Dim summe$ = "", INNN$ = ""
                If anyDT.Rows.Count = 1 Then
                    summe$ = summe$ & "SELECT * FROM " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & "  s where   v.vorgangsid=s.vorgangsid  and " &
                     idspalte$ & "=" & anyDT.Rows(0).Item(ausgabespaltenNr).ToString & ""
                    Return summe
                Else
                    summe$ = summe$ & "SELECT * FROM " & CLstart.myViewsNTabs.tabVorgang & " v," & CLstart.myViewsNTabs.tabStammdaten & "  s where   v.vorgangsid=s.vorgangsid  and " & idspalte$ & " in ("

                    For i = 0 To anyDT.Rows.Count - 1
                        INNN$ = INNN$ & anyDT.Rows(i).Item(ausgabespaltenNr).ToString & ","
                        'If i < anyDT.Rows.Count - 1 Then
                        '    summe$ = summe$ & " union "
                        'End If
                    Next
                End If
                INNN$ = INNN.Substring(0, INNN.Length - 1)
                summe = summe & INNN & ")"
                Return summe$
            Catch ex As Exception
                Return "-1"
            End Try
        End Function
    End Class
End Namespace