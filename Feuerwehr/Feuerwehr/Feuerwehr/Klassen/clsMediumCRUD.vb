Public Class clsMediumCRUD
	Implements IMediumCrud

	Public Function getMedien(ByVal SQL As String) As System.Data.DataTable Implements IMediumCrud.getMedien

	End Function

	Public Function getMedium(ByVal id As Integer) As clsMedium Implements IMediumCrud.getMedium

	End Function



	Public Function Medium_delete(ByVal pl As clsMedium) As Integer Implements IMediumCrud.Medium_delete
		Return loeschen_Medium(pl.ID)
	End Function

	Public Function Medium_update(ByVal pl As clsMedium) As Integer Implements IMediumCrud.Medium_update

	End Function

	Public Function Medium_create(ByVal pl As clsMedium) As Integer Implements IMediumCrud.Medium_create
		Dim newid = neu_speichern_Medium()
		Return newid
	End Function
	Public Function neu_speichern_Medium() As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand

		Try
			glob.StandortDBREC.mydb.Tabelle = "Medien"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			"insert into " & glob.StandortDBREC.mydb.Tabelle & _
			setSQLbodyMedium()

			glob.StandortDBREC.dboeffnen(hinweis$)
			com = New System.Data.OleDb.OleDbCommand(glob.StandortDBREC.mydb.SQL, glob.StandortDBREC.myconn)
			anzahlTreffer& = CInt(com.ExecuteNonQuery)

			com.CommandText = "SELECT @@IDENTITY"	 '"Select LAST_INSERT_ID()"
			Dim newid = CLng(com.ExecuteScalar)


			glob.StandortDBREC.dbschliessen(hinweis$)
			Return CInt(newid)
			If anzahlTreffer < 1 Then
				glob.nachricht_und_Mbox("Problem beim Abspeichern:" & glob.StandortDBREC.mydb.SQL)
				Return -3
			Else
				Return -4
			End If
		Catch ex As Exception
			glob.nachricht_und_Mbox("Problem beim Abspeichern: " & ex.ToString)
			Return -5
		End Try
	End Function

	Private Function setSQLbodyMedium() As String
		Dim sb As New Text.StringBuilder
		sb.Append("  (StammID,laufnr,Dateiname,relativPfad,Titel) ")
		sb.Append(" values (" & aktMedium.StammID & "," & aktMedium.laufnr & ",'" & aktMedium.Dateiname & "','" & aktMedium.Relativpfad & "','" & aktMedium.Titel & "')")
		Return sb.ToString
	End Function

	Public Function loeschen_Medium(ByVal mid%) As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand
		If mid% < 1 Then
			glob.nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
			Return -1
		End If
		Try
			glob.StandortDBREC.mydb.Tabelle = "Medien"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			 "delete from " & glob.StandortDBREC.mydb.Tabelle & _
			 " where ID=" & mid%
			glob.StandortDBREC.dboeffnen(hinweis$)
			com = New System.Data.OleDb.OleDbCommand(glob.StandortDBREC.mydb.SQL, glob.StandortDBREC.myconn)
			'setParams(com, sid)
			anzahlTreffer& = CInt(com.ExecuteNonQuery)
			glob.StandortDBREC.dbschliessen(hinweis$)
			Return CInt(anzahlTreffer)
			If anzahlTreffer < 1 Then
				glob.nachricht_und_Mbox("Problem beim Abspeichern:" & glob.StandortDBREC.mydb.SQL)
				Return -3
			Else
				Return -4
			End If
		Catch ex As Exception
			glob.nachricht_und_Mbox("Problem beim Abspeichern: " & ex.ToString)
			Return -5
		End Try
	End Function

	'Public Function edit_speichern_Medium(ByVal sid%) As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
	'	Dim anzahlTreffer& = 0, hinweis$ = ""
	'	Dim com As New System.Data.OleDb.OleDbCommand
	'	If sid% < 1 Then
	'		glob.nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
	'		Return -1
	'	End If
	'	Try
	'		glob.planDBREC.mydb.Tabelle = "Medien"			'"ohnemedien" 
	'		glob.planDBREC.mydb.SQL = _
	'		 "update " & glob.planDBREC.mydb.Tabelle & _
	'		 setSQLbodyMedium() & _
	'		 " where ID=" & sid
	'		glob.planDBREC.dboeffnen(hinweis$)
	'		com = New System.Data.OleDb.OleDbCommand(glob.planDBREC.mydb.SQL, glob.planDBREC.myconn)
	'		'setParams(com, sid)
	'		anzahlTreffer& = CInt(com.ExecuteNonQuery)
	'		glob.planDBREC.dbschliessen(hinweis$)
	'		Return CInt(anzahlTreffer)
	'		If anzahlTreffer < 1 Then
	'			glob.nachricht_und_Mbox("Problem beim Abspeichern:" & glob.planDBREC.mydb.SQL)
	'			Return -3
	'		Else
	'			Return -4
	'		End If
	'	Catch ex As Exception
	'		glob.nachricht_und_Mbox("Problem beim Abspeichern: " & ex.ToString)
	'		Return -5
	'	End Try
	'End Function
End Class
