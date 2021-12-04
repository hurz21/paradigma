Namespace NSBearbeiter
    Public Class BearbeiterTools
        Public Shared Function istHauptbearbeiterBekannt() As Boolean
            Dim bekannt As Boolean
            'If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, "kuerzel1", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale) Then
            If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter) Then
                'If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, "INITIAL_", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale) Then
                '    bekannt = False
                'Else
                '    bekannt = True
                'End If
            Else
                bekannt = True
            End If
            Return bekannt
        End Function

        Friend Shared Function VorgaengeUebertragen(sql As String) As Integer
            Try
                Dim hinweis As String = ""
                myGlobalz.sitzung.VorgangREC.mydb.SQL = sql
                myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
                Return 1
            Catch ex As Exception

            End Try
        End Function
    End Class
End Namespace
