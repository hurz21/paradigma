
Imports System.Net
Imports System.IO
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Text.RegularExpressions

Namespace LIBgoogle.GmapsApi3
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
        Public Shared Function CreateGeoData(ByVal location As String) As GeoData
            Dim geodata As New GeoData()

            Dim myReq As HttpWebRequest = DirectCast(WebRequest.Create("http://maps.googleapis.com/maps/api/geocode/xml?address=" & location.Trim() & "&sensor=false"), HttpWebRequest)
            Dim webResponse As HttpWebResponse

            Try
                webResponse = TryCast(myReq.GetResponse(), HttpWebResponse)
            Catch
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
            Return geodata
        End Function

        Public Shared Function Googleadress(ByVal instreet$, ByVal incity$, ByVal instate$, ByVal inzip$) As String
            Try
                Dim street As String = String.Empty
                Dim city As String = String.Empty
                Dim state As String = String.Empty
                Dim zip As String = String.Empty
                Dim queryAddress As New Text.StringBuilder()

                queryAddress.Append("http://maps.google.com/maps?size=512x512&q=")
                ' build city part of query string	  
                If incity$ <> String.Empty Then
                    city = incity$.Trim '.Replace(" ", "+")	  
                    '	queryAddress.Append(city + "," & "+")
                    queryAddress.Append(city + "+")
                End If
                ' build street part of query string		 
                If instreet$ <> String.Empty Then
                    instreet$ = instreet$.ToLower.Replace("str.", "strasse")
                    instreet$ = instreet$.ToLower.Replace(" ", "+")
                    street = instreet$
                    'queryAddress.Append(street + "," & "+")
                    queryAddress.Append(street)
                End If
                ' build state part of query string	   
                If instate$ <> String.Empty Then
                    state = instate$ '.Replace(" ", "+") 
                    'queryAddress.Append(state + "," & "+")
                End If
                ' build zip code part of query string		  
                If inzip$ <> String.Empty Then
                    zip = inzip$ '.ToString()		 
                    queryAddress.Append(zip)
                End If
                '	queryAddress.Append("&size=512x512")
                ' pass the url with the query string to web browser control	  
                '	webBrowser1.Navigate(queryAddress.ToString())	 
                Return queryAddress.ToString
            Catch ex As Exception

                Return String.Format("Fehler: {0}Unable to Retrieve Map", ex.Message)
            End Try
        End Function
    End Class
End Namespace



