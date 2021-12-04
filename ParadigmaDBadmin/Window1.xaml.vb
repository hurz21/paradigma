Class Window1
	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button1.Click
		Dim dddd As New DBManipulation
		dddd.fuellePersonenDB()
	End Sub

	Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

	End Sub

	Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button2.Click
		Dim dddd As New clsWeyersDBumsetzen
		dddd.fuellePersonenDB()
	End Sub

	Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button3.Click
		ft()
	End Sub

	Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Button4.Click
		zielDBdefnierenUndGenerieren(person2vorgangREC)
	End Sub
	Private Sub zielDBdefnierenUndGenerieren(ByRef person2vorgangREC As clsDBspecMYSQL)
		Dim id%, nr$, a$() , zaehler%
		person2vorgangREC.mydb.MySQLServer = "kis"
		person2vorgangREC.mydb.dbtyp = "mysql"
		person2vorgangREC.mydb.Schema = "paradigma"
		person2vorgangREC.mydb.username = "root"
		person2vorgangREC.mydb.password = "lkof4"
		person2vorgangREC.mydb.Tabelle = "vorgang"
		person2vorgangREC.mydb.SQL = "select * from " & person2vorgangREC.mydb.Tabelle ' & _
		'  
		Dim hinweis$ = person2vorgangREC.getDataDT
		If person2vorgangREC.dt.Rows.Count > 1 Then
			For i = 0 To person2vorgangREC.dt.Rows.Count - 1
				If person2vorgangREC.dt.Rows(i).Item("AZ").ToString.EndsWith("-rots") Then
					a = (person2vorgangREC.dt.Rows(i).Item("VorgangsGegenstand").ToString).Split("-"c)
					id% = CInt(person2vorgangREC.dt.Rows(i).Item("VorgangsID"))
					nr$ = a(0)
					If IsNumeric(nr) Then
						setzeLaufNr(id, nr, person2vorgangREC)
						zaehler%+=1
					End If
				End If
			Next
		End If
		MsgBox(zaehler)
	End Sub
	Sub setzeLaufNr(ByVal id%, ByVal nr$, ByVal person2vorgangREC As clsDBspecMYSQL)
		Dim newid As long,anZ As long
		Dim mylog As New clsLogging("c:\test.txt")
		person2vorgangREC.mydb.SQL = "update " & person2vorgangREC.mydb.Tabelle & _
		" set vorgangsnr=" & nr & _
		" where vorgangsid=" & id
	anz= person2vorgangREC.sqlexecute(newID,mylog)


	End Sub
End Class
