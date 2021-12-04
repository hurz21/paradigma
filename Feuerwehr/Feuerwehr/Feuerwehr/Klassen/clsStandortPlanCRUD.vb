Public Class clsStandortPlanCRUD
	Implements IFeuerwehrPlanStandort




	Public Function delete(ByVal pl As clsStandortPlan) As Integer Implements IFeuerwehrPlanStandort.delete
		Dim anzahl%
		Dim anzahl2% = loeschen_Adresse(pl.StammID)
		If anzahl2 > 0 Then
			anzahl% = loeschen_Standort(pl.StammID)
			Return anzahl
		Else
			Return anzahl2
		End If
	End Function

	Public Function getPlaene(ByVal SQL As String) As System.Data.DataTable Implements IFeuerwehrPlanStandort.getPlaene

	End Function

	Public Function getPlan(ByVal id As Integer) As clsStandortPlan Implements IFeuerwehrPlanStandort.getPlan

	End Function

#Region "update"
	Public Function update(ByVal pl As clsStandortPlan) As Integer Implements IFeuerwehrPlanStandort.update
		Dim anzahl%
		If pl.anyChange Then
			anzahl% = edit_speichern_Standort(pl.StammID)
			If pl.adr.anyChange Then
				Dim anzahl2% = edit_speichern_Adresse(pl.StammID)
			End If
		End If
		Return anzahl%
	End Function
	Public Function edit_speichern_Standort(ByVal sid%) As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand
		If sid% < 1 Then
			glob.nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
			Return -1
		End If
		Try
			glob.StandortDBREC.mydb.Tabelle = "stammdaten"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			 "update " & glob.StandortDBREC.mydb.Tabelle & _
			 setSQLbodyStandort() & _
			 " where ID=" & sid
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

    Private Function setSQLbodyStandort() As String
        Dim sb As New Text.StringBuilder
        sb.Append(" set ")
        sb.Append(" Titel='" & aktStandort.Titel & "'")
        sb.Append(",Hinweis1='" & aktStandort.Hinweis1 & "'")
        sb.Append(",Hinweis2='" & aktStandort.Hinweis2 & "'")
        sb.Append(",rechts=" & aktStandort.pt.X & "")
        sb.Append(",hoch=" & aktStandort.pt.Y & "")
        Return sb.ToString
    End Function

	Public Function edit_speichern_Adresse(ByVal sid%) As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand
		If sid% < 1 Then
			glob.nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
			Return -1
		End If
		Try
			glob.StandortDBREC.mydb.Tabelle = "adresse"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			"update " & glob.StandortDBREC.mydb.Tabelle & _
			setSQLbodyAdresseNeu() & _
			 " where stammID=" & sid

			glob.StandortDBREC.dboeffnen(hinweis$)
			com = New System.Data.OleDb.OleDbCommand(glob.StandortDBREC.mydb.SQL, glob.StandortDBREC.myconn)
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
#End Region




	Private Function setSQLbodyAdresseNeu() As String
		Dim sb As New Text.StringBuilder
		sb.Append(" set ")
		'	sb.Append(" StammID=@StammID")
		sb.Append(" Ort='" & aktStandort.adr.Gemeindename & "'")
		sb.Append(",Strasse='" & aktStandort.adr.Strassenname & "'")
		sb.Append(",Hausnr='" & aktStandort.adr.HausnrKombi & "'")
		Return sb.ToString
	End Function
#Region "create"
	Public Function create(ByVal pl As clsStandortPlan) As Integer Implements IFeuerwehrPlanStandort.create
		Dim newid = neu_speichern_Standort()
		pl.StammID = newid
		If pl.adr.anyChange Then
			Dim newid2 = neu_speichern_Adresse(pl.StammID)
		End If
	End Function
#End Region
	Public Function neu_speichern_Standort() As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand

		Try
			glob.StandortDBREC.mydb.Tabelle = "stammdaten"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			"insert into " & glob.StandortDBREC.mydb.Tabelle & _
			setSQLbodyStandort2()

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

	Private Function setSQLbodyStandort2() As String
		Dim sb As New Text.StringBuilder
		sb.Append("  (Titel,hinweis1,hinweis2,rechts,hoch) ")
		sb.Append(" values ('" & aktStandort.Titel & "','" & aktStandort.Hinweis1 & "','" & aktStandort.Hinweis2 & "'," & aktStandort.pt.X & "," & aktStandort.pt.Y & ")")
		Return sb.ToString
	End Function

	Public Function neu_speichern_Adresse(ByVal sid%) As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand
		If sid% < 1 Then
			glob.nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
			Return -1
		End If
		Try
			glob.StandortDBREC.mydb.Tabelle = "adresse"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			"insert into " & glob.StandortDBREC.mydb.Tabelle & _
			setSQLbodyAdresseNeu2(sid)

			glob.StandortDBREC.dboeffnen(hinweis$)
			com = New System.Data.OleDb.OleDbCommand(glob.StandortDBREC.mydb.SQL, glob.StandortDBREC.myconn)
			anzahlTreffer& = CInt(com.ExecuteNonQuery)

			com.CommandText = "SELECT @@IDENTITY"	 '"Select LAST_INSERT_ID()"
			Dim newid = CLng(com.ExecuteScalar)

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

	Private Function setSQLbodyAdresseNeu2(ByVal sid%) As String
		Dim sb As New Text.StringBuilder
		sb.Append("  (stammid,Ort,Strasse,Hausnr) values (" & sid.ToString & ",'" & aktStandort.adr.Gemeindename & "','" & aktStandort.adr.Strassenname & "','" & aktStandort.adr.HausnrKombi & "')")

		Return sb.ToString
    End Function

	Public Function loeschen_Standort(ByVal sid%) As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand
		If sid% < 1 Then
			glob.nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
			Return -1
		End If
		Try
			glob.StandortDBREC.mydb.Tabelle = "stammdaten"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			 "delete from " & glob.StandortDBREC.mydb.Tabelle & _
			 " where ID=" & sid
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
	Public Function loeschen_Adresse(ByVal sid%) As Integer	'myGlobalz.sitzung.Vorgang.Stammdaten.Wiedervorlage.WiedervorlageID
		Dim anzahlTreffer& = 0, hinweis$ = ""
		Dim com As New System.Data.OleDb.OleDbCommand
		If sid% < 1 Then
			glob.nachricht_und_Mbox("Fehler: ID ist null - kein update möglich.abbruch")
			Return -1
		End If
		Try
			glob.StandortDBREC.mydb.Tabelle = "adresse"			'"ohnemedien" 
			glob.StandortDBREC.mydb.SQL = _
			"delete from " & glob.StandortDBREC.mydb.Tabelle & _
			 " where stammID=" & sid

			glob.StandortDBREC.dboeffnen(hinweis$)
			com = New System.Data.OleDb.OleDbCommand(glob.StandortDBREC.mydb.SQL, glob.StandortDBREC.myconn)
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
End Class
