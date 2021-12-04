
Class prepWeb
    Shared Property PaareTrenner As Char = CChar("_")
    Shared Property xyTrenner As Char = CChar(";")
    Shared Function doKonvertierung(Q_CoordString As String,
                                    Q_CoordCount As String,
                                    quellKoord As String,
                                    zielKoord As String,
                                    ByRef Z_CoordString As String
                                    ) As Boolean
        Dim qpunkte As List(Of clsGEOPoint)
        Dim retpoints As New Text.StringBuilder
        Dim r, h As String
        Dim rc As Integer
        Try
            l("doKonvertierung----------------------------------------------v")
            l(Q_CoordString)
            qpunkte = makePunkteListe(Q_CoordString, quellKoord)
            Dim freischalten As String = transformCoords.freischalten()
            nachricht("berechneGRADpunkt freischalten:" & freischalten)
            Dim temp As clsGEOPoint
            For Each p As clsGEOPoint In qpunkte
                temp = New clsGEOPoint
                transformCoords.ktransform(quellKoord, zielKoord, p.GKrechts.ToString, p.GKhoch.ToString, r, h, rc)
                p.X = CDbl(r)
                p.Y = CDbl(h)
                retpoints.Append(p.X & xyTrenner & p.Y & PaareTrenner)
            Next
            Z_CoordString = retpoints.ToString
            Z_CoordString = Z_CoordString.Substring(0, Z_CoordString.Length - 1)
            l("Z_CoordString:")
            l(Z_CoordString)
            retpoints = Nothing
            Return True
        Catch ex As Exception
            nachricht("Fehler in: berechneGRADpunkt" & ex.ToString)
            retpoints = Nothing
            Return False
        End Try
    End Function

    Private Shared Function makePunkteListe(Q_CoordString As String, quellKoord As String) As List(Of clsGEOPoint)
        Dim paare(), einzel() As String
        Dim punkte As New List(Of clsGEOPoint)
        Dim neu As New clsGEOPoint
        l("in makePunkteListe: -----------------------------------------")
        l(Q_CoordString)
        l(Q_CoordString)
        Try
            paare = Q_CoordString.Split(PaareTrenner)
            For i = 0 To paare.Length - 1
                neu = New clsGEOPoint
                einzel = paare(i).Split(xyTrenner)
                wandlePunktNachKomma(einzel(0))
                wandlePunktNachKomma(einzel(1))

                neu.GKrechts = CDbl(einzel(0))
                neu.GKhoch = CDbl(einzel(1))

                neu.X = CDbl(einzel(0))
                neu.Y = CDbl(einzel(1))


                punkte.Add(neu)
            Next
            l("schleife fertig")
            Return punkte
        Catch ex As Exception
            l("fehler in makePunkteListe: " & ex.ToString)
        End Try
    End Function

    Private Shared Sub wandlePunktNachKomma(ByRef einzel As String)
        If einzel.Contains(".") Then
            einzel = einzel.Trim.Replace(".", ",")
        End If
    End Sub
    Class punkt
        Property x As Double
        Property y As Double
        Sub New()
            x = 0
            y = 0
        End Sub
    End Class
End Class
