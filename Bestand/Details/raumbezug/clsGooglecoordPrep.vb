Public Class clsGooglecoordPrep
    Shared Function getgooglecoordinatenMitUmrechnung(ByVal adresse As String, ByRef googlepunkt As clsGEOPoint, ByVal ProxyString$, ByRef hinweis$, ByVal ziel As String) As Boolean
        Try
            Dim googlefehlertext As String = ""
            ' Dim handle As LIBgoogle.clsGoogleCoordinates = Nothing
            Dim gd As New LIBgoogle.Geo.GeoData
            Dim lResult As Boolean = getgooglecoordinaten(adresse, googlefehlertext, ProxyString, gd)
            hinweis$ = googlefehlertext
            nachricht(googlefehlertext)
            If lResult Then
                If getgooglecoordinatenUmrechnung(googlepunkt, gd, ziel$) Then
                    nachricht(" Umrechung auf GK erfolgreich.")
                Else
                    nachricht(" Umrechung auf GK nicht erfolgreich.")
                End If
                Return True
            Else
                nachricht_und_Mbox(googlefehlertext)
                Return False
            End If
        Catch ex As Exception
            nachricht_und_Mbox(String.Format("Fehler: getgooglecoordinatenMitUmrechnung: {0}{1}", vbCrLf, ex))
            Return False
        End Try
    End Function

    Shared Function getgooglecoordinaten(ByVal adresse As String,
                                         ByRef googlefehlertext As String,
                                         ByVal ProxyString As String,
                                         ByRef gd As LIBgoogle.Geo.GeoData) As Boolean
        googlefehlertext = ""
        nachricht("getgooglecoordinatenMitUmrechnung-------------------proxy: " & ProxyString)
        gd = LIBgoogle.Geo.GeoData.CreateGeoData(adresse, ProxyString)
        Dim result As String = gd.Latitude & "," & gd.Longitude
        googlefehlertext = "Ergebnis: " ' & result$ 
        Return True
    End Function

    Private Shared Sub utm32NachUtm(ByRef googlepunkt As clsGEOPoint)
        googlepunkt.GKrechts = googlepunkt.GKrechts - 32000000
    End Sub
    Shared Function getgooglecoordinatenUmrechnung(ByRef googlepunkt As clsGEOPoint,
                                            ByRef gd As LIBgoogle.Geo.GeoData,
                                            ByVal ziel As String) As Boolean
        Dim erfolg As Boolean
        Try
            googlepunkt.laenge_string = gd.Longitude.ToString().Replace(",", ".") '.lonstring
            googlepunkt.breite_string = gd.Latitude.ToString().Replace(",", ".") 'handle.latstring
            nachricht("vor Umrechnung in gausskrüger / UTM:")


            '  If ziel$ = "GK_google" Then erfolg = LIBcoordumrechnung.clsKoordumrechnung.berechneGaussKrueger_ausGoogleGeografisch(googlepunkt, 6)
            ' If ziel$ = "UTM_google" Then erfolg = LIBcoordumrechnung.clsKoordumrechnung.berechneUTM32_ausGoogleGeografisch(googlepunkt, 6)



            Dim newpunit As New myPoint
            newpunit.X = CDbl(googlepunkt.laenge_string.Replace(".", ","))
            newpunit.Y = CDbl(googlepunkt.breite_string.Replace(".", ","))

            Dim punktliste() As myPoint
            ReDim punktliste(0)
            punktliste(0) = newpunit
            Dim quellstring As String = modKoordTrans.bildeQuellKoordinatenString(punktliste)
            Dim aufruf As String = modKoordTrans.bildeaufruf(quellstring, punktliste.Count.ToString, "WINKEL_G", "UTM")
            Dim result As String = modKoordTrans.Mykoordtransform(myGlobalz.ProxyString, aufruf)

            Dim r As String=""
            Dim h As String = ""

            modKoordTrans.getLongLatFromResultSingle(result, r, h)
            googlepunkt.GKrechts = CDbl(r)
            googlepunkt.GKhoch = CDbl(h)
            utm32NachUtm(googlepunkt)
            If result.Trim.StartsWith("32") Then
                erfolg = True
            Else
                erfolg = False
            End If

            'erfolg = hCoords.berechneGaussKruege(googlepunkt, 6)

            If Not erfolg Then
                nachricht_und_Mbox("Die Umrechnung nach " & "UTM" & " war nicht erfolgreich!")
                Return False
            Else
                nachricht("Die Umrechnung nnach " & ziel$ & " war erfolgreich!")
                googlepunkt.GKrechts = CInt(googlepunkt.GKrechts)
                googlepunkt.GKhoch = CInt(googlepunkt.GKhoch)
                Return True
            End If
        Catch ex As Exception
            nachricht("Fehler in getgooglecoordinatenUmrechnung: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Private Shared Sub nachricht(ByVal Text As String)
        My.Log.WriteEntry(Text)
    End Sub
    Private Shared Sub nachricht_und_Mbox(ByVal Text As String)
        My.Log.WriteEntry(Text)
    End Sub
End Class
