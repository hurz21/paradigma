Imports System.Data
Imports System.Text
Class clsGoogleMein
	Public Function Googleadress(ByVal instreet$, ByVal incity$, ByVal instate$, ByVal inzip$) As String
		Try
			Dim street As String = String.Empty
			Dim city As String = String.Empty
			Dim state As String = String.Empty
			Dim zip As String = String.Empty
			Dim queryAddress As New StringBuilder()

			queryAddress.Append("http://maps.google.com/maps?size=512x512&q=")
			' build city part of query string	  
			If incity$ <> String.Empty Then
				city = incity$.Trim	'.Replace(" ", "+")	  
				'	queryAddress.Append(city + "," & "+")
				queryAddress.Append(city + "+")
			End If
			' build street part of query string		 
			If instreet$ <> String.Empty Then
				instreet$ = instreet$.ToLower.Replace("str.", "strasse")
				instreet$ = instreet$.ToLower.Replace(" ", "+")
				street = instreet$
				'queryAddress.Append(street + "," & "+")
				queryAddress.Append(street)	'+ "+")	  
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
			MessageBox.Show(ex.Message.ToString(), "Unable to Retrieve Map")
			Return "Fehler: " & ex.Message.ToString() & "Unable to Retrieve Map"
		End Try
	End Function

	Private Function googleLatLon(ByVal inlat$, ByVal inlon$) As String
		If inlat$ = String.Empty Or inlon$ = String.Empty Then
			MessageBox.Show("Supply a latitude and longitude value.", "Missing Data")
		End If
		Try
			Dim lat As String = String.Empty
			Dim lon As String = String.Empty
			Dim queryAddress As New StringBuilder()
			queryAddress.Append("http://maps.google.com/maps?q=")
			' build latitude part of query string
			If inlat$ <> String.Empty Then
				lat = inlat$
				queryAddress.Append(lat + "%2C")
			End If
			' build longitude part of query string
			If inlon$ <> String.Empty Then
				lon = inlon$
				queryAddress.Append(lon)
			End If
			'    webBrowser1.Navigate(queryAddress.ToString())	 
			Return queryAddress.ToString
		Catch ex As Exception
			'   MessageBox.Show(ex.Message.ToString(), "Error")	 
			Return ex.Message.ToString() & " Error"
		End Try
	End Function
End Class
