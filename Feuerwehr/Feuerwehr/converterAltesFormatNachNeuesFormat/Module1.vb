Module Module1
	Public Function GetPlanDBREC() As clsDBspecMDB
		Dim planDBREC As New clsDBspecMDB
		Return planDBREC
	End Function
	Sub Main()
		Dim planDBREC As clsDBspecMDB = GetPlanDBREC()
		Dim zielTab As DataTable
		planDBREC.mydb.MySQLServer = "D:\fkatbig\sicherheit\brandschutz\plaene.mdb"
		planDBREC.mydb.Schema = "D:\fkatbig\sicherheit\brandschutz\plaene.mdb"
		Dim pdfStammroot$ = "/fkatbig/sicherheit/brandschutz/"
		planDBREC.mydb.Tabelle = "brandschutzplaene"
		planDBREC.mydb.dbtyp = "mdb"
		planDBREC.mydb.SQL = "select * from " & planDBREC.mydb.Tabelle

		Dim hinweis$ = planDBREC.getDataDT()
		Dim stammid% = 0
		Dim laufnr%

		Dim newid&, anzahltreffer&
		Dim mylog As New clsLogging("D:\fkatbig\sicherheit\brandschutz\l.log")

		Dim pdfNR% = 0
		Dim pdfstring$ = ""
		Dim pdfdateiname$, relativpfad$
		Dim a$()
		planDBREC.mydb.Tabelle = " Adresse"
		For i = 0 To planDBREC.dt.Rows.Count - 1

			stammid% = CInt(planDBREC.dt.Rows(i).Item("ID"))

			planDBREC.mydb.SQL = "insert into Stammdaten" & _
			 " (ID,Titel,Hinweis1,Hinweis2,rechts,hoch) values (" & _
			 stammid%.ToString & _
			 ",   	" & Chr(34) & planDBREC.dt.Rows(i).Item("Zusatz").ToString.Replace(Chr(34), "_") & Chr(34) & _
			 ",  " & Chr(34) & planDBREC.dt.Rows(i).Item("Hinweis1").ToString.Replace(Chr(34), "_") & Chr(34) & _
			 ",  " & Chr(34) & planDBREC.dt.Rows(i).Item("Hinweis2").ToString.Replace(Chr(34), "_") & Chr(34) & _
			 ",  " & planDBREC.dt.Rows(i).Item("rechts").ToString & _
			 ",  " & planDBREC.dt.Rows(i).Item("Hoch").ToString & ")"
			anzahltreffer = planDBREC.sqlexecute(newid, mylog)
			If anzahltreffer < 1 Then
				MsgBox("Fehler bei Stammdaten insert:   " & planDBREC.mydb.SQL)
			End If



			planDBREC.mydb.SQL = "insert into Adresse" & _
			 " (StammID,Ort,Strasse,Hausnr) values (" & stammid%.ToString & _
			 ",   	" & Chr(34) & planDBREC.dt.Rows(i).Item("Gemeindename").ToString & Chr(34) & _
			 ",  " & Chr(34) & planDBREC.dt.Rows(i).Item("Strassenname").ToString.Replace(Chr(34), "_") & Chr(34) & _
			 ",    " & Chr(34) & planDBREC.dt.Rows(i).Item("Hausnummer").ToString.Replace(Chr(34), "_") & Chr(34) & ")"
			anzahltreffer = planDBREC.sqlexecute(newid, mylog)
			If anzahltreffer < 1 Then
				MsgBox("Fehler bei adresse insert:   " & planDBREC.mydb.SQL)
			End If

			For j = 1 To 31
				pdfstring$ = "pdf" & CStr(j)
				If String.IsNullOrEmpty(planDBREC.dt.Rows(i).Item(pdfstring$).ToString) Then
					Continue For
				End If
				pdfdateiname$ = planDBREC.dt.Rows(i).Item(pdfstring$).ToString
				pdfdateiname$ = pdfdateiname$.Replace(pdfStammroot, "").Replace("/", "\").Trim
				If String.IsNullOrEmpty(pdfdateiname.Trim) Then
					relativpfad$ = ""
					pdfdateiname$ = ""
					Continue For
				Else
					a = pdfdateiname.Split("\"c)
					relativpfad$ = a(0) & "\" & a(1)
					pdfdateiname$ = a(2)
					If a.Length > 3 Then
						pdfdateiname$ = a(3)
					End If
					If a.Length > 4 Then
						pdfdateiname$ = a(4)
					End If
				End If

				''zahlrauslösen
				laufnr = j
				'If pdfdateiname.Contains("-") Then
				'	a = pdfdateiname.Split("-"c)
				'	If IsNumeric(a(0)) Then
				'		'laufnr = CInt(a(0))
				'		'pdfdateiname = a(1)
				'	Else
				'		laufnr = j
				'	End If
				'End If				  

				planDBREC.mydb.SQL = "insert into Medien" & _
				" (StammID,laufnr,Dateiname,Relativpfad,Titel) values (" & _
				stammid%.ToString & _
				",   	" & laufnr.ToString & _
				",  " & Chr(34) & pdfdateiname$ & Chr(34) & _
				",  " & Chr(34) & relativpfad$ & Chr(34) & _
				",  " & Chr(34) & planDBREC.dt.Rows(i).Item(pdfstring$).ToString & Chr(34) & ")"

				anzahltreffer = planDBREC.sqlexecute(newid, mylog)
				If anzahltreffer < 1 Then
					MsgBox("Fehler bei Stammdaten insert:   " & planDBREC.mydb.SQL)
				End If
			Next j
			Console.WriteLine(i.ToString)
		Next
	End Sub
End Module
