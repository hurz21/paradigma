Namespace LaserScan

    Module mod3dtools
        Private Function buildeStats(ByVal teilErgebnis As String,
                                    ByRef zwert As Double,
                                    ByRef zmin As Double,
                                    ByRef zmax As Double,
                                    ByRef mittel As Double,
                                    ByRef diff As Double) As String
            Dim a() As String = teilErgebnis.Split("|"c)
            Dim b() As Double

            Dim izaehl As Integer
            ReDim b(a.Length - 1)
            Try
                For i = 0 To a.GetUpperBound(0) - 1
                    If a(i).Trim.IsNothingOrEmpty Then
                        Continue For
                    End If
                    If CDbl(a(i)) > 0 Then
                        b(i) = CDbl(a(i))
                    End If
                Next
                zmax = Double.MinValue
                zmin = Double.MaxValue
                For Each element As Double In b
                    zmax = Math.Max(zmax, element)
                    If element > 1 Then
                        zmin = Math.Min(zmin, element)
                        mittel = mittel + element
                        izaehl += 1
                    End If
                Next
                mittel = mittel / izaehl
                diff = (zmax - zmin)
                zwert = CDbl(a(4))
                Dim erg As String = ""
                Return erg
            Catch ex As Exception
                nachricht("fehler in buildeStats: ", ex)
                Return ""
            End Try
        End Function

        Private Function bildeInfoText(ByVal newpoint As myPoint,
                                       ByVal dommin As Double, ByVal dommax As Double, ByVal dommittel As Double, ByVal domdif As Double,
                                       ByVal dgmmin As Double, ByVal dgmmax As Double, ByVal dgmmittel As Double, ByVal dgmdif As Double,
                                       ByVal hoehe As Double, ByVal a As String(), hoehenmatrix As String) As String
            Try
                Return "DOM Werte für Punkt : " & newpoint.toString & " und Umgebung (+-1m)" & Environment.NewLine & Environment.NewLine &
                                      "Oberflächenhöhe:" & Environment.NewLine &
                                      a(0) &
                                      "Max,Min,Mittel,Diff.: " & Format(dommax, "###.###") & "; " & Format(dommin, "###.###") & "; " & Format(dommittel, "###.###") & "; " & Format(dgmdif, "###.###") & Environment.NewLine &
                                      "______________________________________________________" & Environment.NewLine &
                                      "Geländehöhe:" & Environment.NewLine &
                                      "Max,Min,Mittel,Diff.: " & Format(dgmmax, "###.###") & "; " & Format(dgmmin, "###.###") & "; " & Format(dgmmittel, "###.###") & "; " & Format(dgmdif, "###.###") & Environment.NewLine &
                                      "Höhe des Objektes: " & Format(hoehe, "###.###") & Environment.NewLine &
                                      "______________________________________________________" & Environment.NewLine &
                                      "Höhenverteilung:" & Environment.NewLine &
                                      hoehenmatrix
            Catch ex As Exception
                nachricht("fehler in bildeInfoText: ", ex)
                Return "fehler"
            End Try
        End Function

        Sub koordinateKlickBerechnenZWERT(ByVal KoordinateKLickpt As Point?)
            Dim newpoint2 As New myPoint
            Dim newpoint As New myPoint
            Dim domzwert, dommin, dommax, dommittel, domdif, hoehe As Double
            Dim dgmzwert, dgmmin, dgmmax, dgmmittel, dgmdif As Double
            Dim gesamtErgebnis, stats1, hoehenmatrix As String
            Dim a(), c() As String
            Try
                newpoint2.X = CDbl(KoordinateKLickpt.Value.X)
                newpoint2.Y = CDbl(KoordinateKLickpt.Value.Y)
                newpoint = clsAufrufgenerator.WINPOINTVonCanvasNachGKumrechnen(newpoint2, CLstart.myc.kartengen.aktMap.aktrange, CLstart.myc.kartengen.aktMap.aktcanvas)
                newpoint.SetToInteger()
                gesamtErgebnis = detailsTools.getZWert(newpoint)
                a = gesamtErgebnis.Split("#"c)
                stats1 = buildeStats(a(0), domzwert, dommin, dommax, dommittel, domdif)
                stats1 = buildeStats(a(1), dgmzwert, dgmmin, dgmmax, dgmmittel, dgmdif)
                c = bildeHoehenMatrix(a(0), a(1))
                hoehenmatrix = bildehoehenText(c)
                hoehe = domzwert - dgmzwert
                MsgBox(bildeInfoText(newpoint, dommin, dommax, dommittel, domdif, dgmmin, dgmmax, dgmmittel, dgmdif, hoehe, a, hoehenmatrix))
                newpoint2 = Nothing
                newpoint = Nothing
            Catch ex As Exception
                nachricht("fehler in bildeInfoText: ", ex)
            End Try

        End Sub

        Private Function bildeHoehenMatrix(dom As String, dgm As String) As String()
            Dim b(), a(), c() As String
            a = dom.Split("|"c)
            b = dgm.Split("|"c)
            ReDim c(a.Length - 1)
            For i = 0 To a.GetUpperBound(0) - 1
                If a(i).Trim.IsNothingOrEmpty Then
                    Continue For
                End If
                c(i) = CStr(CDbl(a(i)) - CDbl(b(i)))

            Next
            Return c
        End Function

        Private Function bildehoehenText(c As String()) As String
            Dim result As String = ""
            For i = 0 To c.GetUpperBound(0) - 1
                If c(i).Trim.IsNothingOrEmpty Then
                    Continue For
                End If
                result = result & Format((CDbl(c(i))).ToString("###.###")) & "|"
                If i = 2 Or i = 5 Or i = 8 Then
                    result = result & Environment.NewLine
                End If

            Next
            Return result
        End Function



    End Module
End Namespace