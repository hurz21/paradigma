Imports System.Data
Imports MySql
Imports MySql.Data
Imports MySql.Data.MySqlClient
Imports MySql.Data.Types

Public Class clsDBspecMYSQL
	Implements db_grundfunktionen
	Implements ICloneable
	Private _mydb As New clsDatenbankZugriff
	Public myconn As MySqlConnection
	Public hinweis$ = ""
	Private _mycount As Long
	Public Function sqlexecute(ByRef newID As Long, _
	 ByVal mylog As clsLogging) As Long Implements db_grundfunktionen.sqlexecute

		Dim retcode As Integer, Hinweis$ = ""
		Dim com As New MySqlCommand()
		Dim anzahlTreffer&
		Try
			If mydb.dbtyp = "mysql" Then
				retcode = dboeffnen(Hinweis$)
			End If
			retcode = 0
			com.Connection = myconn
			com.CommandText = mydb.SQL
			anzahlTreffer& = CInt(com.ExecuteNonQuery)
			If mydb.SQL.StartsWith("insert".ToLower) Then
				com.CommandText = "Select LAST_INSERT_ID()"
				newID = CLng(com.ExecuteScalar)
			End If
			Return anzahlTreffer&
		Catch myerror As OleDb.OleDbException
			retcode = -1
			Hinweis &= "sqlexecute: Database connection error: " & _
			 myerror.Message & " " & _
			 myerror.Source & " " & _
			 myerror.StackTrace & " " & _
			 mydb.getDBinfo("")
			mylog.log(Hinweis)
			Return 0
		Catch e As Exception
			retcode = -2
			Hinweis &= "sqlexecute: Allgemeiner Fehler: " & _
			 e.Message & " " & _
			 e.Source & " " & _
			 mydb.Schema
			mylog.log(Hinweis)
			Return 0
		Finally
			com.Dispose()
			dbschliessen(Hinweis)
		End Try
	End Function

	Public Function dboeffnen(ByRef resultstring As String) As Integer Implements db_grundfunktionen.dboeffnen
		Dim retcode%
		Try
			retcode = 0
			If doConnection(hinweis$) Then
				myconn.Open()
			Else
				hinweis$ = "Fehler bei der erstellung der connection:" & hinweis
			End If

		Catch myerror As MySqlException
			hinweis$ &= "MySqlException, beim ÖFFNERN UU. ist die DB nicht aktiv. " & vbCrLf & "Fehler beim Öffnen der DB " & _
			 "Database connection error: " & _
			 myerror.Message & " " & _
			 mydb.MySQLServer & " " & _
			 mydb.Schema
			Return -1
		Catch e As Exception
			hinweis$ &= "beim ÖFFNEN Database connection error: " & _
			 e.Message & " " & _
			 e.Source & " " & _
			 mydb.Schema
			Return -2
		End Try
		Return retcode
	End Function

	Public Function dbschliessen(ByRef resultstring As String) As Integer Implements db_grundfunktionen.dbschliessen
		Try
			myconn.Close()
			myconn.Dispose()
			Return 0
		Catch myerror As MySqlException
			resultstring$ &= "UU. ist die DB nicht aktiv. " & vbCrLf & "Fehler beim schliessen der DB " & _
			 "Database connection error: " & _
			 myerror.Message & " " & _
			 mydb.MySQLServer & " " & _
			 mydb.Schema
			Return -1
		Catch e As Exception
			resultstring$ &= "Database connection error: schliessen" & _
			 e.Message & " " & _
			 e.Source & " " & _
			 mydb.Schema
			Return -1
		End Try
	End Function

	Public Function doConnection(ByRef hinweis As String) As Boolean Implements db_grundfunktionen.doConnection
		Try
			myconn = New MySqlConnection(String.Format("Data Source={0};Initial Catalog={1};User ID={2};PWD={3}", _
						mydb.MySQLServer, mydb.Schema, mydb.username, mydb.password))
			
			Return True
		Catch ex As Exception
			hinweis$ &= ex.Message & ex.Source
			Return False
		End Try
	End Function

	Public Function getDataDT() As String Implements db_grundfunktionen.getDataDT
		Dim retcode As Integer, hinweis As String = ""
		_mycount = 0
		retcode = dboeffnen(hinweis$)
		If retcode < 0 Then
			hinweis$ &= String.Format("FEHLER, Datenbank in getDataDT  konnte nicht geöffnet werden! {0}{1}", vbCrLf, mydb.getDBinfo(""))
			Return hinweis
		End If
		Try
			Dim com As New MySqlCommand(mydb.SQL, myconn)
			Dim da As New MySqlDataAdapter(com)
			da.MissingSchemaAction = MissingSchemaAction.AddWithKey
			dt = New DataTable
			_mycount = da.Fill(dt)
			retcode = dbschliessen(hinweis$)
			If retcode < 0 Then
				hinweis$ &= "FEHLER, Datenbank in getDataDT konnte nicht geschlossen werden! " & vbCrLf & mydb.getDBinfo("")
			End If
			com.Dispose()
			da.Dispose()
			Return hinweis
		Catch myerror As MySqlException
			retcode = -1
			hinweis &= "FEHLER, getDataDT Database connection error: " & _
			 myerror.Message & " " & _
			 myerror.Source & " " & _
			 myerror.StackTrace & " " & _
			 mydb.MySQLServer & " " & mydb.Schema
			Return hinweis
		Catch e As Exception
			retcode = -2
			hinweis &= "FEHLER, getDataDT Database connection error: " & _
			 e.Message & " " & _
			 e.Source & " " & _
			 mydb.Schema
			Return hinweis
		Finally
			retcode = dbschliessen(hinweis$)
			If retcode < 0 Then
				hinweis$ &= "FEHLER, 2 Datenbank konnte nicht geschlossen werden! " & vbCrLf & mydb.getDBinfo("")
			End If
		End Try
	End Function

	Public Sub New()
		MyClass.New("mysql")
	End Sub
	Public Sub New(ByVal dbtypIn$)
		mydb.dbtyp = dbtypIn$
	End Sub
	Public Function Clone() As Object Implements System.ICloneable.Clone
		Return MemberwiseClone()
	End Function

	Public Property mycount() As Long Implements db_grundfunktionen.mycount
		Get
			Return _mycount
		End Get
		Set(ByVal value As Long)
			_mycount = value
		End Set
	End Property

	Private _dt As New DataTable
	Property dt() As System.Data.DataTable Implements db_grundfunktionen.dt
		Get
			Return _dt
		End Get
		Set(ByVal value As DataTable)
			_dt = value
		End Set
	End Property

	Public Property mydb() As clsDatenbankZugriff Implements db_grundfunktionen.mydb
		Get
			Return _mydb
		End Get
		Set(ByVal value As clsDatenbankZugriff)
			_mydb = value
		End Set
	End Property

	Public Function ADOgetOneString_neu() As String
		Dim myMessage$ = "", hinweis$ = ""
		Try
			hinweis = getDataDT()
			If mycount > 0 Then
				Return dt.Rows(0).Item(0).ToString
			Else
				Return ""
			End If
		Catch e As Exception
			myMessage = "Error : " & _
			 e.Message & " " & _
			 e.Source & " " & hinweis
			Return myMessage
		End Try
	End Function

End Class