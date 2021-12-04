Imports System.Data

Module personalDBumschichten
	Public mylog As New clsLogging("c:\paraadmin.log")
	Public ipersonen, ikontakte, ianschriften, ibankkonten, iorg As Integer
	Public ipersonWiederverwendet%, iorgWiederverwendet%, ianschriftWiederverwendet%
	Public Const weyers$ = "WeyG"
	Public person2vorgangREC As New clsDBspecMYSQL
	Public personenREC As New clsDBspecMYSQL
	Public kontaktdatenDT As New clsDBspecMYSQL
	Public orgRec As New clsDBspecMYSQL
	Public anschriftREC As New clsDBspecMYSQL
	Public beteiligteREC As New clsDBspecMYSQL
	Public tempREC As New clsDBspecMYSQL
	Public aktperson As New Person

	Private Sub zielDBdefnierenUndGenerieren(ByRef person2vorgangREC As clsDBspecMYSQL)
		person2vorgangREC.mydb.MySQLServer = "localhost"
		person2vorgangREC.mydb.dbtyp = "mysql"
		person2vorgangREC.mydb.Schema = "paradigma"
		person2vorgangREC.mydb.username = "root"
		person2vorgangREC.mydb.password = "lkof4"
		person2vorgangREC.mydb.Tabelle = "personen"
		'paraPersnalREC.mydb.SQL = "select * from " & paraPersnalREC.mydb.Tabelle & _
		' " where fdkurz='" & "67" & "'"
		'Dim hinweis$ = paraPersnalREC.getDataDT
	End Sub


	Sub ft()
		'  alle personen in vorgängen erfassten SELECT * FROM person2vorgang p;
		'kontaktdaten zur person bilden
		'in beteilite abspeichern

		zielDBdefnierenUndGenerieren(person2vorgangREC)
		zielDBdefnierenUndGenerieren(personenREC)
		zielDBdefnierenUndGenerieren(kontaktdatenDT)
		zielDBdefnierenUndGenerieren(orgRec)
		zielDBdefnierenUndGenerieren(anschriftREC)
		zielDBdefnierenUndGenerieren(beteiligteREC)
		zielDBdefnierenUndGenerieren(tempREC)
		Dim vorgangsid%
		Dim aaa As New DBManipulation

		beteiligteREC.mydb.Tabelle="beteiligte"
		aaa.zieltabelleLoeschen(beteiligteREC)

		person2vorgangREC.mydb.SQL = "SELECT * FROM person2vorgang "
		Dim hinweis$ = person2vorgangREC.getDataDT
		For Each vp As DataRow In person2vorgangREC.dt.AsEnumerable
			Console.WriteLine(vp.Item("personenID").ToString & " - " & vp.Item("vorgangsID").ToString)
			personenREC.mydb.SQL = "select * from personen where personenid=" & CInt(vp.Item("personenID").ToString)
			hinweis = personenREC.getDataDT()
			 vorgangsid=cint(vp.Item("vorgangsid"))
			dim rolle$=vp.Item("rolle").ToString 
			dim Bemerkung$=vp.Item("Bemerkung").ToString 
			If vorgangsid=239 Then
					Debug.print("")
			End If
			'Dim ipers% = 0
			For Each persRow As DataRow In personenREC.dt.AsEnumerable
				aktperson.clear()
				aktperson.PersonenID = CInt(vp.Item("personenID"))
				aktperson.rolle = rolle$	 
				aktperson.Name = clsDBtools.fieldvalue(persRow.Item("name"))
				aktperson.Vorname = clsDBtools.fieldvalue(persRow.Item("vorname"))
				aktperson.Namenszusatz = clsDBtools.fieldvalue(persRow.Item("namenszusatz"))
				aktperson.Bemerkung = clsDBtools.fieldvalue(persRow.Item("bemerkung"))
				aktperson.Anrede = clsDBtools.fieldvalue(persRow.Item("anrede"))
				If initKontaktdaten4personDatatable(aktperson.PersonenID) Then
					WeyersPersonInitialisieren(0)
				 
				End If
						Beteiligte_abspeichern_Neu(CInt(vp.Item("vorgangsID").ToString))
			Next
		Next
	End Sub

	Public Function initKontaktdaten4personDatatable(ByVal personenID%) As Boolean
		'koppelungstabelle abfragen> adressid-liste
		If KontaktIDlisteVonPersonErstellen(personenID%) Then
			kontaktdatenDT.dt = tempREC.dt.Copy
			tempREC.mydb.Tabelle = "Kontaktdaten"
			Dim SQL$ = UNION_SQL_erzeugen(kontaktdatenDT.dt, tempREC.mydb.Tabelle, 2, "kontaktid")  
			kontaktliste4KontaktIDListe(SQL$)
			kontaktdatenDT.dt = tempREC.dt.Copy
			Return True
		Else
			' glob2.nachricht_und_Mbox("Für personid: " & personenID% & " gibts keine Adressen")
			Return False
		End If
	End Function
	Public Function KontaktIDlisteVonPersonErstellen(ByVal personenID%) As Boolean
		Dim hinweis$
		tempREC.mydb.Tabelle = "person2kontakt"
		tempREC.mydb.SQL = "SELECT * FROM " & tempREC.mydb.Tabelle & _
		 " where personenid =" & personenID%
		hinweis = tempREC.getDataDT()
		If tempREC.mycount < 1 Then

			Return False
		Else

			Return True
		End If
	End Function
	Public Function UNION_SQL_erzeugen(ByVal anyDT As DataTable, ByVal tabellenname$, ByVal ausgabespaltenNr%, ByVal idspalte$) As String
		Try
			Dim summe$ = ""
			For i = 0 To anyDT.Rows.Count - 1
				summe$ = summe$ & "(SELECT * FROM " & tabellenname$ & _
				 " where " & idspalte$ & "=" & anyDT.Rows(i).Item(ausgabespaltenNr).ToString & ")"
				If i < anyDT.Rows.Count - 1 Then
					summe$ = summe$ & " union "
				End If
			Next
			Return summe$
		Catch ex As Exception
			Return "-1"
		End Try
	End Function

	Public Function kontaktliste4KontaktIDListe(ByVal sql$) As Boolean
		Dim hinweis$
		tempREC.mydb.Tabelle = "Kontaktdaten"
		tempREC.mydb.SQL = sql$
		hinweis = tempREC.getDataDT()
		If tempREC.mycount < 1 Then
			Return False
		Else
			Return True
		End If
	End Function

	Private Sub WeyersPersonInitialisieren(ByVal cursor%)
		'erste kontaktdaten holen und darstellen
		With kontaktdatenDT.dt.Rows(cursor%)
			Dim aktKontaktID% = CInt(clsDBtools.fieldvalue(.Item("kontaktid")))
			aktperson.Kontakt.GesellFunktion = clsDBtools.fieldvalue(.Item("GesellFunktion"))
			aktperson.Kontakt.elektr.Telefon1 = clsDBtools.fieldvalue(.Item("FFTelefon1"))
			aktperson.Kontakt.elektr.Telefon2 = clsDBtools.fieldvalue(.Item("FFTelefon2"))
			aktperson.Kontakt.elektr.Fax1 = clsDBtools.fieldvalue(.Item("FFFax1"))
			aktperson.Kontakt.elektr.Fax2 = clsDBtools.fieldvalue(.Item("FFFax2"))
			aktperson.Kontakt.elektr.MobilFon = clsDBtools.fieldvalue(.Item("FFMobilfon"))
			aktperson.Kontakt.elektr.Email = clsDBtools.fieldvalue(.Item("FFemail"))
			aktperson.Kontakt.elektr.Homepage = clsDBtools.fieldvalue(.Item("FFHomepage"))
			aktperson.Kontakt.OrgID = CInt(clsDBtools.fieldvalue(.Item("OrgID")))
			aktperson.Kontakt.AnschriftID = CInt(clsDBtools.fieldvalue(.Item("AnschriftID")))
			aktperson.Kontakt.BankkontoID = CInt(clsDBtools.fieldvalue(.Item("BankkontoID")))
		End With
		addORG2Kontakt(aktperson.Kontakt)
		addAnschrift2Kontakt(aktperson.Kontakt)
		addBankverbindung2Kontakt(aktperson.Kontakt)
	End Sub
	Function addORG2Kontakt(ByRef akontakt As Kontaktdaten) As Boolean
		If akontakt.OrgID < 1 Then
			Return True
		End If
		'	mylog.log("addORG2Kontakt" & akontakt.OrgID)
		If initOrgDT(akontakt.OrgID) Then
			mylog.log("initOrgDT")
			If OrgDT2Obj(akontakt, tempREC.dt) Then
				mylog.log("initOrgDT  OK")
				Return True
			End If
		End If
		mylog.log("initOrgDT  fehler")
		Return False
	End Function
	Function OrgDT2Obj(ByRef akontakt As Kontaktdaten, ByVal mydt As DataTable) As Boolean
		Try
			akontakt.Org.Name = clsDBtools.fieldvalue(mydt.Rows(0).Item("Name"))
			akontakt.Org.Zusatz = clsDBtools.fieldvalue(mydt.Rows(0).Item("Zusatz"))
			akontakt.Org.Typ1 = clsDBtools.fieldvalue(mydt.Rows(0).Item("Typ1"))
			akontakt.Org.Typ2 = clsDBtools.fieldvalue(mydt.Rows(0).Item("Typ2"))
			akontakt.Org.Eigentuemer = clsDBtools.fieldvalue(mydt.Rows(0).Item("Eigentuemer"))
			akontakt.Org.Quelle = clsDBtools.fieldvalue(mydt.Rows(0).Item("Quelle"))
			akontakt.Org.Anschriftid = CInt(clsDBtools.fieldvalue(mydt.Rows(0).Item("Anschriftid")))
			akontakt.Org.Bemerkung = clsDBtools.fieldvalue(mydt.Rows(0).Item("Bemerkung"))
			Return True
		Catch ex As Exception
			mylog.log("OrgDT2Obj fehler " & ex.ToString)
			Return False
		End Try
	End Function
	Function AnschriftDT2Obj(ByRef akontakt As Kontaktdaten, ByVal mydt As DataTable) As Boolean
		Try
			akontakt.Anschrift.Gemeindename = clsDBtools.fieldvalue(mydt.Rows(0).Item("Gemeindename"))
			akontakt.Anschrift.Strasse = clsDBtools.fieldvalue(mydt.Rows(0).Item("Strasse"))
			akontakt.Anschrift.Hausnr = clsDBtools.fieldvalue(mydt.Rows(0).Item("Hausnr"))
			akontakt.Anschrift.PLZ = CInt(clsDBtools.fieldvalue(mydt.Rows(0).Item("PLZ")))
			akontakt.Anschrift.Postfach = clsDBtools.fieldvalue(mydt.Rows(0).Item("Postfach"))
			akontakt.Anschrift.Bemerkung = clsDBtools.fieldvalue(mydt.Rows(0).Item("Bemerkung"))
			akontakt.Anschrift.Quelle = clsDBtools.fieldvalue(mydt.Rows(0).Item("Quelle"))
			Return True
		Catch ex As Exception
			mylog.log("AnschriftDT2Obj fehler " & ex.ToString)
			Return False
		End Try
	End Function

	Function addAnschrift2Kontakt(ByRef akontakt As Kontaktdaten) As Boolean
		If akontakt.AnschriftID < 1 Then
			Return True
		End If
		mylog.log("addAnschrift2Kontakt" & akontakt.AnschriftID)
		If initAnschriftDT(akontakt.AnschriftID) Then
			mylog.log("initOrgDT")
			If AnschriftDT2Obj(akontakt, tempREC.dt) Then
				mylog.log("initOrgDT  OK")
				Return True
			End If
		End If
		mylog.log("addAnschrift2Kontakt  fehler")
		Return False
	End Function

	Function addBankverbindung2Kontakt(ByRef akontakt As Kontaktdaten) As Boolean
		If akontakt.BankkontoID < 1 Then
			Return True
		End If
		mylog.log("addBankverbindung2Kontakt" & akontakt.BankkontoID)
		If initBankkontoDT(akontakt.BankkontoID) Then
			mylog.log("addBankverbindung2Kontakt")
			If bankverbindungDT2Obj(akontakt, tempREC.dt) Then
				mylog.log("addBankverbindung2Kontakt  OK")
				Return True
			End If
		End If
		mylog.log("initOrgDT  fehler")
		Return False
	End Function

	Function bankverbindungDT2Obj(ByRef akontakt As Kontaktdaten, ByVal mydt As DataTable) As Boolean
		Try
			akontakt.Bankkonto.BLZ = clsDBtools.fieldvalue(mydt.Rows(0).Item("BLZ"))
			akontakt.Bankkonto.Name = clsDBtools.fieldvalue(mydt.Rows(0).Item("name"))
			akontakt.Bankkonto.KontoNr = clsDBtools.fieldvalue(mydt.Rows(0).Item("kontonr"))
			akontakt.Bankkonto.Titel = clsDBtools.fieldvalue(mydt.Rows(0).Item("titel"))
			Return True
		Catch ex As Exception
			mylog.log("AnschriftDT2Obj fehler " & ex.ToString)
			Return False
		End Try
	End Function
	Function initOrgDT(ByVal OrgID%) As Boolean
		If OrgID < 1 Then
			mylog.log("	initOrgDT: ist ist kleiner 1")
			Return False
		End If
		Try
			tempREC.mydb.Tabelle = "Organisation"
			tempREC.mydb.SQL = _
			 "select * from " & tempREC.mydb.Tabelle & _
			 " where OrgID=" & OrgID%
			Dim hinweis$ = tempREC.getDataDT()
			If tempREC.dt.Rows.Count < 1 Then
				'	glob2.nachricht_und_Mbox("Problem beim initOrgDT:" & tempREC.mydb.SQL)
				Return False
			Else
				Return True
			End If
		Catch ex As Exception
			'glob2.nachricht_und_Mbox("Problem beim Abspeichern: " & vbCrLf & ex.ToString)
			Return False
		End Try
	End Function
	Function initBankkontoDT(ByVal BankkontoID%) As Boolean
		If BankkontoID% < 1 Then
			mylog.log("	initBankkontoDT: ist ist kleiner 1")
			Return False
		End If
		Try
			tempREC.mydb.Tabelle = "bankverbindung"
			tempREC.mydb.SQL = _
			 "select * from " & tempREC.mydb.Tabelle & _
			 " where BankkontoID=" & BankkontoID%
			Dim hinweis$ = tempREC.getDataDT()
			If tempREC.dt.Rows.Count < 1 Then
				'				glob2.nachricht_und_Mbox("Problem beim initBankkontoDT:" & tempREC.mydb.SQL)
				Return False
			Else
				Return True
			End If
		Catch ex As Exception
			'			glob2.nachricht_und_Mbox("Problem beim Abspeichern: " & vbCrLf & ex.ToString)
			Return False
		End Try
	End Function
	Function initAnschriftDT(ByVal AnschriftID%) As Boolean
		Try
			tempREC.mydb.Tabelle = "Anschrift"
			tempREC.mydb.SQL = _
			 "select * from " & tempREC.mydb.Tabelle & _
			 " where AnschriftID=" & AnschriftID%
			Dim hinweis$ = tempREC.getDataDT()
			If tempREC.dt.Rows.Count < 1 Then
				'glob2.nachricht_und_Mbox("Problem beim Abspeichern:Problem beim initAnschriftDT:" & tempREC.mydb.SQL)
				Return False
			Else
				Return True
			End If
		Catch ex As Exception
			'	glob2.nachricht_und_Mbox("Problem beim Abspeichern:Problem beim initAnschriftDT: " & vbCrLf & ex.ToString)
			Return False
		End Try
	End Function

	Public Function Beteiligte_abspeichern_Neu(ByVal vorgangsid%) As Integer
		Dim anzahlTreffer&, newid&
		Try
			personenREC.mydb.Tabelle = "Beteiligte"
			personenREC.mydb.SQL = _
			 "insert into " & personenREC.mydb.Tabelle & " set " & _
			 " Name='" & aktperson.Name & "'" & _
			 ",Vorname='" & aktperson.Vorname & "'" & _
			 ",Bemerkung='" & aktperson.Bemerkung & "'" & _
			 ",Namenszusatz='" & aktperson.Namenszusatz & "'" & _
			 ",Anrede='" & aktperson.Anrede & "'" & _
			 ",Quelle='" & aktperson.Quelle & "'" & _
			 ",Gemeindename='" & aktperson.Kontakt.Anschrift.Gemeindename & "'" & _
			 ",Strasse='" & aktperson.Kontakt.Anschrift.Strasse & "'" & _
			 ",Hausnr='" & aktperson.Kontakt.Anschrift.Hausnr & "'" & _
			 ",PLZ=" & aktperson.Kontakt.Anschrift.PLZ.ToString & _
			 ",Postfach='" & aktperson.Kontakt.Anschrift.Postfach & "'" & _
			 ",FFTelefon1='" & aktperson.Kontakt.elektr.Telefon1 & "'" & _
			 ",FFTelefon2='" & aktperson.Kontakt.elektr.Telefon2 & "'" & _
			 ",FFFax1='" & aktperson.Kontakt.elektr.Fax1 & "'" & _
			 ",FFFax2='" & aktperson.Kontakt.elektr.Fax2 & "'" & _
			 ",FFMobilfon='" & aktperson.Kontakt.elektr.MobilFon & "'" & _
			 ",FFemail='" & aktperson.Kontakt.elektr.Email & "'" & _
			 ",FFHomepage='" & aktperson.Kontakt.elektr.Homepage & "'" & _
			 ",GesellFunktion='" & aktperson.Kontakt.GesellFunktion & "'" & _
			 ",OrgName='" & aktperson.Kontakt.Org.Name & "'" & _
			 ",OrgZusatz='" & aktperson.Kontakt.Org.Zusatz & "'" & _
			 ",OrgTyp1='" & aktperson.Kontakt.Org.Typ1 & "'" & _
			 ",OrgTyp2='" & aktperson.Kontakt.Org.Typ2 & "'" & _
			 ",OrgEigentuemer='" & aktperson.Kontakt.Org.Eigentuemer & "'" & _
			 ",Rolle='" & aktperson.Rolle & "'" & _
			 ",VorgangsID=" & vorgangsid
			anzahlTreffer = personenREC.sqlexecute(newid, mylog)
			If anzahlTreffer < 1 Then
				'glob2.nachricht_und_Mbox("Problem beim Abspeichern:" & personenRec.mydb.SQL)
				Return -1
			Else
				Return CInt(newid)
			End If
		Catch ex As Exception
			'glob2.nachricht_und_Mbox("Problem beim Abspeichern: " & ex.ToString)
			Return -2
		End Try
	End Function

End Module
