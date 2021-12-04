Imports System.Text.RegularExpressions
Imports System.Net
Imports System.IO
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

'wird zz nicht benutzt
'http://dotnet-snippets.de/dns/geo-daten-mit-google-maps-api-v3-anhand-der-adresse-abfragen-SID1608.aspx

Namespace LIBgoogle.Geo
    Public Class GeoData
        Public Latitude As Decimal
        Public Longitude As Decimal

        Public Sub New()
        End Sub

        ''' <summary>
        ''' Creates a GeoData object and retrieves the GeoData provided by an address from google maps api v3
        ''' </summary>
        ''' <param name="location">complete adress as would be typed in at google maps</param>
        ''' <returns>GeoData object with latitude and longitude</returns>
        Public Shared Function CreateGeoData(ByVal location As String, proxystring As String) As GeoData
            Dim geodata As New GeoData()
            Dim myProxy As WebProxy
            Dim newUri As Uri
            myProxy = New WebProxy()
            If Not (String.IsNullOrEmpty(proxystring) Or proxystring = "http://") Then
                newUri = New Uri(proxystring)
                myProxy.Address = newUri
                myProxy.Credentials = CredentialCache.DefaultCredentials
            End If
            'https://maps.googleapis.com/maps/api/geocode/xml?address=10+frankfurterstrasse,+63303&key=AIzaSyBErGgt82BKcngFHWnCVh4-OXrFwRfeaqo
            location = location.Trim.Replace(" ", "+")
            '   Dim googleapi As String = "http://maps.googleapis.com/maps/api/geocode/output?address="
            '   Dim googleapi As String = "http://maps.googleapis.com/maps/api/geocode/json?address="
            Dim googleapi As String = "https://maps.googleapis.com/maps/api/geocode/xml?address="
            Dim url As String = googleapi & location.Trim() & "&sensor=true&key=AIzaSyBErGgt82BKcngFHWnCVh4-OXrFwRfeaqo"
            l("CreateGeoData url: " & url)
            Dim myReq As HttpWebRequest = DirectCast(WebRequest.Create(url),
                                                     HttpWebRequest)
            myReq.Proxy = myProxy
            Dim webResponse As HttpWebResponse = Nothing
            Try
                webResponse = CType(myReq.GetResponse(), HttpWebResponse)
                ' webResponse = TryCast(myReq.GetResponse( ), HttpWebResponse)
            Catch ex As Exception
                Debug.Print(ex.ToString)
                webResponse.Close()
                Return Nothing
            End Try
            If webResponse IsNot Nothing Then
                If webResponse.StatusCode = HttpStatusCode.OK Then
                    Dim doc As New System.Xml.XmlDocument()
                    doc.Load(webResponse.GetResponseStream())
                    If doc IsNot Nothing Then
                        Dim geometry As System.Xml.XmlNode = doc.SelectSingleNode("GeocodeResponse/result/geometry/location")
                        If geometry IsNot Nothing Then
                            Dim ci As New System.Globalization.CultureInfo("en-US")
                            geodata.Latitude = Convert.ToDecimal(geometry.SelectSingleNode("lat").InnerText, ci)
                            geodata.Longitude = Convert.ToDecimal(geometry.SelectSingleNode("lng").InnerText, ci)
                        End If
                    End If
                End If
            End If
            webResponse.Close()
            Return geodata
        End Function
    End Class
End Namespace

