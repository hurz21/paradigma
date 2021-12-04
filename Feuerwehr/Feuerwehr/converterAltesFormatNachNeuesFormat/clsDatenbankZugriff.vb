Public Class clsDatenbankZugriff
  Implements ICloneable
  Public Function setDBTYP() As Boolean
    dbtyp = getDBTYP()
  End Function

  Public Function getDBTYP() As String
    Dim dbtyptest = "mysql"
    If Tabelle.ToLower.EndsWith(".dbf") Then dbtyptest = "dbf"
    If Schema.ToLower.EndsWith(".mdb") Then dbtyptest = "mdb"
    Return dbtyptest
  End Function
  Public Overrides Function tostring() As String
    Return getDBinfo("")
  End Function
  Private Sub getDBinfoString(ByRef trenn$, ByRef info As System.Text.StringBuilder)
    info.Append("clsDatenbankZugriff ++++++++++++ Objektbeginn" & trenn)
    info.Append(String.Format("   dbtyp: {0}{1}", dbtyp, trenn))
    info.Append(String.Format("   Server: {0}{1}", MySQLServer, trenn))
    info.Append(String.Format("   Schema: {0}{1}", Schema, trenn))
    info.Append(String.Format("   tabelle: {0}{1}", Tabelle, trenn))
    info.Append(String.Format("   SQL: {0}{1}", SQL, trenn))
    info.Append(String.Format("   SQLWhereValue: {0}{1}", SQLWhereValue, trenn))
    info.Append(String.Format("   username: {0}{1}", username, trenn))
    info.Append(String.Format("   password: {0}{1}", password, trenn))
    info.Append("clsDatenbankZugriff ++++++++++++ Objektende")
  End Sub
  Public Function getDBinfo(ByVal trenn$) As String
    Dim info As New System.Text.StringBuilder
    If String.IsNullOrEmpty(trenn) Then
      trenn$ = vbCrLf
    Else
      trenn$ = trenn$ '+ vbCrLf
    End If
    getDBinfoString(trenn, info)
    Return info.ToString
  End Function

  Private SQLWhereValue As String
  Public Property SQLWhere() As String


    Get
      Return SQLWhereValue
    End Get
    Set(ByVal Value As String)
      SQLWhereValue = Value
    End Set
  End Property
  Public Function cleanSQL() As String
    'Beseitigt reste aus access-abfragen
    If SQLValue Is Nothing Then
      Return Nothing
    End If
    SQLValue$ = SQLValue$.Replace("[", "`")
    SQLValue$ = SQLValue$.Replace("]", "`")
    If SQLValue$.ToLower.Contains("like") Then
      If SQLValue$.Contains("*'") Then
        SQLValue$ = SQLValue$.Replace("*'", "%'")
      End If
      If SQLValue$.Contains("'*") Then
        SQLValue$ = SQLValue$.Replace("'*", "'%")
      End If
    End If
    Return SQLValue$
  End Function
  Private passwordValue As String
  Public Property password() As String
    Get

      Return passwordValue
    End Get
    Set(ByVal Value As String)

      passwordValue = Value
    End Set
  End Property
  Private usernameValue As String
  Public Property username() As String
    Get
      Return usernameValue
    End Get
    Set(ByVal Value As String)
      usernameValue = Value
    End Set
  End Property

  Private TabelleValue As String
  Public Property Tabelle() As String
    Get
      Return TabelleValue
    End Get
    Set(ByVal Value As String)
      TabelleValue = Value
    End Set
  End Property
  Private MySQLServerValue As String
  Public Property MySQLServer() As String
    Get
      Return MySQLServerValue
    End Get
    Set(ByVal Value As String)
      MySQLServerValue = Value
    End Set
  End Property
  Private SQLValue As String
  Public Property SQL() As String
    Get
      Return SQLValue
    End Get
    Set(ByVal Value As String)
      SQLValue = Value
    End Set
  End Property
  Private _dbtyp As String
  ''' <summary>
  ''' "mdb" oder "dbf" oder "mysql"  
  ''' </summary>
  ''' <value></value>
  ''' <remarks></remarks>
  Public Property dbtyp() As String
    Get
      Return _dbtyp
    End Get
    Set(ByVal Value As String)
      _dbtyp = Value
    End Set
  End Property
  Private SchemaValue As String
  Public Property Schema() As String
    Get

      Return SchemaValue
    End Get
    Set(ByVal Value As String)

      SchemaValue = Value
    End Set
  End Property
  Private orderValue As String
  Public Property order() As String
    Get

      Return orderValue
    End Get
    Set(ByVal Value As String)

      orderValue = Value
    End Set
  End Property
  Private _link_spalte_name$
  Public Property link_spalte_name$()
    Get
      Return _link_spalte_name$
    End Get
    Set(ByVal value$)
      _link_spalte_name$ = value
    End Set
  End Property
  Public Function Clone() As Object Implements System.ICloneable.Clone
    Return MemberwiseClone()
  End Function
End Class

