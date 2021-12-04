Public Class clsLogging
	Private Property Logg1() As IO.StreamWriter
		Get
			Return logg
		End Get
		Set(ByVal value As IO.StreamWriter)
			logg = value
		End Set
	End Property
	'Public enc As Text.Encoding = Text.Encoding.GetEncoding("8859")
	''Public myLog As New clsLogging

	' das logging verursacht probleme bei schnell aufeinanderfolgenden programmaufrufen
	' daher immer nur über   mylog aufrufen
	Private logg As IO.StreamWriter	'das log-objekt
	Public Shared writeLog As Boolean	 'ob ueberhaupt geloggt wird
	'Public Function showstring(ByVal delim$) As String
	'	Dim a$ = "", wert$ = "", summe$ = ""
	'	For Each pi As System.Reflection.PropertyInfo In Me.GetType().GetProperties()
	'		a$ = pi.Name
	'		wert$ = "=" & pi.GetValue(Me, Nothing).ToString
	'		summe = a$ & wert$ & vbCrLf & delim$
	'	Next
	'	Return summe
	'End Function
	Public Streamfilepub$		'die offentl version
	Public Sub fehlerReport(ByVal ex As Exception)
		Dim lText$ = String.Empty
		fehlerReport(ex, lText)
	End Sub
	Public Sub fehlerReport(ByVal ex As Exception, ByVal text$)
		Dim FehlerHinweis$ = "Fehler: " & vbCrLf & _
		 ex.Message & vbCrLf & _
		 ex.StackTrace & vbCrLf & _
		 ex.Source
		log("ERROR / Fehler: " & text$ & vbCrLf & FehlerHinweis$)
	End Sub
	Public Sub New(ByVal Streamfile$)
#If DEBUG Then
		Try
			If (String.IsNullOrEmpty(Streamfile)) Then
				writeLog = False
			Else
				writeLog = True
				Streamfilepub$ = Streamfile
				Logg1 = New IO.StreamWriter(Streamfile$)

				Logg1.AutoFlush = True
				Logg1.WriteLine("Anfang: " & Now.Date.ToString & " " & Now.TimeOfDay.ToString)
				setbeginn()
			End If
		Catch e As Exception
		End Try
#End If
	End Sub
	Friend Function setbeginn() As String
#If DEBUG Then
		Static Dim mybeginn As DateTime = Now
		Dim ts As TimeSpan = New TimeSpan(Now.Ticks - mybeginn.Ticks)
		Return ts.TotalSeconds.ToString
#End If
		Return ""
	End Function
	Public Function log(ByRef txt As String) As Boolean
#If DEBUG Then
		Try
			If writeLog Then Logg1.WriteLine(txt)
		Catch e As Exception
			Dim FehlerHinweis$
			FehlerHinweis$ = "Fehler in Main(): " & vbCrLf & "<br>" & _
			 e.Message & " " & vbCrLf & "<br>" & _
			 e.StackTrace & " " & vbCrLf & "<br>" & _
			 e.Source & "<br>" & " "
			Return False
		End Try
#End If
		Return True
	End Function
	Public Function logException(ByVal e As Exception, ByRef txt As String) As Boolean
#If DEBUG Then
		Dim FehlerHinweis$
		FehlerHinweis$ = "Fehler: " & vbCrLf & "<br>" & _
		e.Message & " " & vbCrLf & "<br>" & _
		e.StackTrace & " " & vbCrLf & "<br>" & _
		e.Source & "<br>" & " "
		Return False
		If writeLog Then
#If TARGET = "winexe" Then
			MsgBox(FehlerHinweis$ & txt)
#End If
#If TARGET = "exe" Then	 'console
      logg.WriteLine(FehlerHinweis$ & txt)
#End If
		End If
#End If
		Return True
	End Function
	Public Function log2(ByRef txt As String) As Boolean
#If DEBUG Then
		Try
			If writeLog Then
#If TARGET = "winexe" Then
				MsgBox(txt)
#End If
#If TARGET = "exe" Then	 'console
        logg.WriteLine(txt)
#End If
			End If
		Catch e As Exception
			Dim FehlerHinweis$
			FehlerHinweis$ = "Fehler: " & vbCrLf & "<br>" & _
			 e.Message & " " & vbCrLf & "<br>" & _
			 e.StackTrace & " " & vbCrLf & "<br>" & _
			 e.Source & "<br>" & " "
			Return False
		End Try
#End If
		Return True
	End Function
	Public Function Endlog(ByRef txt As String) As Boolean
#If DEBUG Then
		Try
			If writeLog Then
				'#If debug Then
				Logg1.WriteLine(txt)
				Logg1.WriteLine("Ende  : " & Now.Date.ToString & " " & Now.TimeOfDay.ToString)
				Logg1.WriteLine("Dauer  : " & setbeginn())
				Logg1.Close()
				'#End If
				'logg.Dispose()
				Return True
			Else
				Return False
			End If
		Catch e As Exception
			Return False
		End Try
#End If
		Return True
	End Function
End Class
