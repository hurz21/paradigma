Namespace NSBearbeiter
    Public Class BearbeiterTools
        Public Shared Function istUserBekannt() As Boolean
            Dim bekannt As Boolean
            If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, "kuerzel1", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale) Then
                If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, "INITIAL_", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale) Then
                    bekannt = False
                Else
                    bekannt = True
                End If
            Else
                bekannt = True
            End If
            Return bekannt
        End Function
    End Class
End Namespace
