Imports System.Data

Imports MySql.Data.MySqlClient

Public Class clsDBspecMYSQL
    Implements IDB_grundfunktionen
    Implements ICloneable

    'Property myconn As System.Data.Common.DbConnection
    Private _mydb As New clsDatenbankZugriff
    '	Private mylog As LIBgemeinsames.clsLogging
    Public Property myconn As MySqlConnection
    Public hinweis$ = ""
    Private _mycount As Long
    Function manipquerie(query As String, slqparamlist As List(Of clsSqlparam), ReturnIdentity As Boolean,
                             returnColumn As String) As Integer Implements IDB_grundfunktionen.manipquerie
        Return 1
    End Function
    Public Function sqlexecute(ByRef newID As Long) As Long Implements IDB_grundfunktionen.sqlexecute
        Dim retcode As Integer, Hinweis$ = ""
        Dim com As New MySqlCommand
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
        Catch myerror As MySqlException
            retcode = -1
            Hinweis &= "sqlexecute: Database connection error: " &
             myerror.Message & " " &
             myerror.Source & " " &
             myerror.StackTrace & " " &
             mydb.getDBinfo("")
            '	mylog.log(Hinweis)
            Return 0
        Catch e As Exception
            retcode = -2
            Hinweis &= "sqlexecute: Allgemeiner Fehler: " &
             e.Message & " " &
             e.Source & " " &
             mydb.Schema
            'mylog.log(Hinweis)
            Return 0
        Finally
            com.Dispose()
            dbschliessen(Hinweis)
        End Try
    End Function

    Shared Sub nachricht(ByVal text$)
        '	MsgBox(text$)			'   glob2.nachricht
        ' mylog.log(text)
        My.Log.WriteEntry("IN MYSQL: " & text)
    End Sub
    Shared Sub nachricht_Mbox(ByVal text$)
        'MsgBox(text$)           '   glob2.nachricht_mbox
        '	mylog.log(text)
        My.Log.WriteEntry("IN MYSQL: " & text)
    End Sub
    Public Function dboeffnen(ByRef resultstring As String) As Integer Implements IDB_grundfunktionen.dboeffnen
        Try
            If doConnection(hinweis$) Then
                '  nachricht(myconn.ConnectionString)
                myconn.Open()
            Else
                hinweis$ = "Fehler bei der Erstellung der connection:" & hinweis & myconn.ConnectionString
            End If

        Catch myerror As MySqlException
            hinweis$ &= "MySqlException, beim ÖFFNEN UU. ist die DB nicht aktiv. " & vbCrLf & "Fehler beim Öffnen der DB " &
             "Database connection error: " &
             myerror.Message & " " &
             mydb.Host & " " &
             mydb.Schema
            nachricht(String.Format("{0}-a Datenbank ist nicht aktiv!{1}{2}", mydb.getDBinfo(""), vbCrLf, myerror))
            'glob2.nachricht("Datenbank ist nicht aktiv!" & vbCrLf & mydb.tostring)
            Return -1
        Catch e As Exception
            hinweis$ &= "beim ÖFFNEN Database connection error: " &
             e.Message & " " &
             e.Source & " " &
             mydb.Schema
            nachricht_Mbox(mydb.Host & ", b Datenbank ist 2nicht aktiv!" & vbCrLf & mydb.getDBinfo("") & e.ToString)
            'glob2.nachricht("Datenbank ist nicht aktiv!" & vbCrLf & mydb.tostring)
            Return -2
        End Try
        Return 0
    End Function

    Public Function dbschliessen(ByRef resultstring As String) As Integer Implements IDB_grundfunktionen.dbschliessen
        Try
            myconn.Close()
            myconn.Dispose()
            Return 0
        Catch myerror As MySqlException
            resultstring$ &= "UU. ist die DB nicht aktiv. " & vbCrLf & "Fehler beim Schliessen der DB " &
                 "Database connection error: " &
                 myerror.Message & " " &
                 mydb.Host & " " &
                 mydb.Schema
            Return -1
        Catch e As Exception
            resultstring$ &= "Database connection error: schliessen" &
             e.Message & " " &
             e.Source & " " &
             mydb.Schema
            Return -1
        End Try
    End Function

    Public Shared Function getConnection(ByVal mydb As clsDatenbankZugriff) As MySqlConnection
        Dim csb As New MySqlConnectionStringBuilder
        csb.Server = mydb.Host
        csb.Database = mydb.Schema
        csb.UserID = mydb.username
        csb.Password = mydb.password

        csb.Pooling = False
        Dim lokmyconn As New MySqlConnection(csb.ConnectionString)
        Return lokmyconn
    End Function
    Public Function doConnection(ByRef hinweis As String) As Boolean Implements IDB_grundfunktionen.doConnection
        Try
            myconn = getConnection(mydb)

            'Dim lFormat As String = String.Format("Data Source={0};Initial Catalog={1};User ID={2};PWD={3};pooling=false", _
            '             mydb.MySQLServer, mydb.Schema, mydb.username, mydb.password)
            'myconn = New MySqlConnection(lFormat)
            Return True
        Catch ex As Exception
            nachricht(ex.ToString)
            Return False
        End Try
    End Function

    Public Function getDataDT() As String Implements IDB_grundfunktionen.getDataDT
        Dim retcode As Integer, hinweis As String = ""
        _mycount = 0
        retcode = dboeffnen(hinweis$)
        nachricht(retcode.ToString)
        If retcode < 0 Then
            hinweis$ &= String.Format("FEHLER, Datenbank in getDataDT  konnte nicht geöffnet werden! {0}{1}", vbCrLf, mydb.getDBinfo(""))
            nachricht(hinweis)
            Return hinweis
        End If
        Try
            nachricht(mydb.SQL)
            Dim com As New MySqlCommand(mydb.SQL, myconn)
            Dim da As New MySqlDataAdapter(com)
            'da.MissingSchemaAction = MissingSchemaAction.AddWithKey
            dt = New DataTable
            _mycount = da.Fill(dt)
            retcode = dbschliessen(hinweis$)
            If retcode < 0 Then
                hinweis$ &= "FEHLER, Datenbank in getDataDT konnte nicht geschlossen werden! " & vbCrLf & mydb.getDBinfo("")
            End If
            com.Dispose()
            da.Dispose()
            retcode = dbschliessen(hinweis$)
            Return hinweis
        Catch myerror As MySqlException
            retcode = -1
            hinweis &= "FEHLER, getDataDT Database connection error: " &
             myerror.Message & " " &
             myerror.Source & " " &
             myerror.StackTrace & " " &
             mydb.Host & " " & mydb.Schema
            Return hinweis
        Catch e As Exception
            retcode = -2
            hinweis &= "FEHLER, getDataDT Database connection error: " &
             e.Message & " " &
             e.Source & " " &
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

    End Sub

    Public Sub New(ByVal dbtypIn$)
        mydb.dbtyp = dbtypIn$
    End Sub

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return MemberwiseClone()
    End Function

    Public Property mycount() As Long Implements IDB_grundfunktionen.mycount
        Get
            Return _mycount
        End Get
        Set(ByVal value As Long)
            _mycount = value
        End Set
    End Property

    Private _dt As New DataTable
    Property dt() As DataTable Implements IDB_grundfunktionen.dt
        Get
            Return _dt
        End Get
        Set(ByVal value As DataTable)
            _dt = value
        End Set
    End Property

    Public Property mydb() As clsDatenbankZugriff Implements IDB_grundfunktionen.mydb
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