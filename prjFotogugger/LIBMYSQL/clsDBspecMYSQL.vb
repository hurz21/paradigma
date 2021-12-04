#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data
Imports MySql.Data.MySqlClient

Public Class clsDBspecMYSQL
    Implements IDB_grundfunktionen
    Implements ICloneable
    Implements IDisposable

    'Property myconn As System.Data.Common.DbConnection
    Private _mydb As New clsDatenbankZugriff
    '	Private mylog As LIBgemeinsames.clsLogging
    Public Property myconn As MySqlConnection
    Public hinweis$ = ""
    Private _mycount As Long

    Private disposed As Boolean = False  
    'Implement IDisposable.
   Public Overloads Sub Dispose() Implements IDisposable.Dispose
      Dispose(True)
      GC.SuppressFinalize(Me)
   End Sub  
   Protected Overloads Overridable Sub Dispose(disposing As Boolean)
      If disposed = False Then
          If disposing Then
             ' Free other state (managed objects).
                dt.Dispose
                 _dt.Dispose
             disposed = True

          End If
             ' dt.Dispose
             '_dt.Dispose
          ' Free your own state (unmanaged objects).
          ' Set large fields to null.
      End If
   End Sub
       Protected Overrides Sub Finalize()
      ' Simply call Dispose(False).
      Dispose (False)
   End Sub
    Public Function manipquerie(query As String, slqparamlist As List(Of clsSqlparam), ReturnIdentity As Boolean,
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
            Hinweis &= "sqlexecute: Database connection error: " & _
             myerror.Message & " " & _
             myerror.Source & " " & _
             myerror.StackTrace & " " & _
             mydb.getDBinfo("")
            '	mylog.log(Hinweis)
            Return 0
        Catch e As Exception
            retcode = -2
            Hinweis &= "sqlexecute: Allgemeiner Fehler: " & _
             e.Message & " " & _
             e.Source & " " & _
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
        MsgBox(text$)           '   glob2.nachricht_mbox
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
            hinweis$ &= "MySqlException, beim ÖFFNEN UU. ist die DB nicht aktiv. " & vbCrLf & "Fehler beim Öffnen der DB " & _
             "Database connection error: " & _
             myerror.Message & " " & _
             mydb.Host & " " & _
             mydb.Schema
            nachricht(String.Format("{0}-Datenbank ist nicht aktiv!{1}{2}", mydb.Host, vbCrLf, myerror))
            'glob2.nachricht("Datenbank ist nicht aktiv!" & vbCrLf & mydb.tostring)
            Return -1
        Catch e As Exception
            hinweis$ &= "beim ÖFFNEN Database connection error: " & _
             e.Message & " " & _
             e.Source & " " & _
             mydb.Schema
            nachricht_Mbox(mydb.Host & ", Datenbank ist nicht aktiv!" & vbCrLf & e.ToString)
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
            resultstring$ &= "UU. ist die DB nicht aktiv. " & vbCrLf & "Fehler beim Schliessen der DB " & _
                 "Database connection error: " & _
                 myerror.Message & " " & _
                 mydb.Host & " " & _
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
            da.MissingSchemaAction = MissingSchemaAction.AddWithKey
            dt = New DataTable
            _mycount = da.Fill(dt)
            retcode = dbschliessen(hinweis)
            If retcode < 0 Then
                hinweis$ &= "FEHLER, Datenbank in getDataDT konnte nicht geschlossen werden! " & vbCrLf & mydb.getDBinfo("")
            End If
            com.Dispose()
            da.Dispose()
            retcode = dbschliessen(hinweis)
            Return hinweis
        Catch myerror As MySqlException
            retcode = -1
            hinweis &= "FEHLER, getDataDT Database connection error: " & _
             myerror.Message & " " & _
             myerror.Source & " " & _
             myerror.StackTrace & " " & _
             mydb.Host & " " & mydb.Schema & "/" & mydb.SQL
            Return hinweis
        Catch e As Exception
            retcode = -2
            hinweis &= "FEHLER, getDataDT Database connection error: " & _
             e.Message & " " & _
             e.Source & " " & _
              mydb.Host & " " & mydb.Schema & "/" & mydb.SQL
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