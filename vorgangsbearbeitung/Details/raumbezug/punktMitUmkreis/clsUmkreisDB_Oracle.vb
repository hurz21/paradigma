''Imports System.Data.OracleClient
'Imports LibDB
'Imports System.Data
'Public Class clsUmkreisDB_Oracle
'  Implements IDisposable
'    Public MeineDBConnection As New OracleConnection
'    Sub New(ByVal conn As System.Data.Common.DbConnection)
'        MeineDBConnection = CType(conn, OracleConnection)
'    End Sub


'    Public Function RB_Umkreis_abspeichern_Neu() As Integer
'        '        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'        '        Dim com As OracleCommand
'        '        Try
'        '            glob2.initTemprecAusVorgangRecOracle()
'        '            myGlobalz.sitzung.tempREC.mydb.Tabelle ="Paraumkreis"

'        '            Dim SQLupdate$ =
'        'String.Format("INSERT INTO {0} (RADIUSM,BESCHREIBUNG) " +
'        '                      " VALUES (:RADIUSM,:BESCHREIBUNG)",
'        '                        myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
'        '            SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"

'        '            nachricht("nach setSQLbody : " & SQLupdate)
'        '            MeineDBConnection.Open()
'        '            nachricht("nach dboeffnen  ")
'        '            com = New OracleCommand(SQLupdate$, MeineDBConnection)
'        '            nachricht("vor setParams  ")
'        '            setSQLparamsUmkreisRB(com, 0)
'        '            'com.CommandText = SQLupdate$
'        '            'com.CommandType = CommandType.Text
'        '            'Dim p_theid As New OracleParameter

'        '            'p_theid.DbType = DbType.Decimal
'        '            'p_theid.Direction = ParameterDirection.ReturnValue
'        '            'p_theid.ParameterName = ":R1"
'        '            'com.Parameters.Add(p_theid)
'        '            'Dim rtn = CInt(com.ExecuteNonQuery)
'        '            'newid = CLng(p_theid.Value)

'        '            newid = clsOracleIns.GetNewid(com, SQLupdate)
'        '            MeineDBConnection.Close()
'        '            Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)


'        '            'If newid < 1 Then
'        '            '    nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'        '            '    Return -1
'        '            'Else
'        '            '    Return CInt(newid)
'        '            'End If
'        '        Catch ex As Exception
'        '            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
'        '            Return -2
'        '        End Try
'    End Function

'    Shared Function setsqlbodyUmkreisRB() As String
'        Return " set " & _
'         " RADIUSM=:RADIUSM" & _
'         ",BESCHREIBUNG=:BESCHREIBUNG"
'    End Function

'    Shared Sub setSQLparamsUmkreisRB(ByVal com As OracleCommand, ByVal sekid%)
'        '	com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.myconn)
'        com.Parameters.AddWithValue(":RADIUSM", myGlobalz.sitzung.aktPMU.Radius)
'        com.Parameters.AddWithValue(":BESCHREIBUNG", myGlobalz.sitzung.aktPMU.Name)
'        ' com.Parameters.AddWithValue(":ID", sekid)
'    End Sub
'    Public Function RB_Umkreis_abspeichern_Edit(ByVal sekid as integer) as  Integer
'        'Dim anzahlTreffer& = 0, hinweis$ = ""
'        'Dim com As OracleCommand
'        'Try
'        '    If sekid < 1 Then
'        '        nachricht_und_Mbox("fehler updateid<1)")
'        '        Return 0
'        '    End If
'        '    glob2.initTemprecAusVorgangRecOracle()
'        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="Paraumkreis"
'        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
'        '     "update " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'        '     setsqlbodyUmkreisRB() &
'        '      " where id=:ID"

'        '    MeineDBConnection.Open()
'        '    com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
'        '    setSQLparamsUmkreisRB(com, sekid)
'        '    com.Parameters.AddWithValue(":ID", sekid)
'        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)

'        '    MeineDBConnection.Close()

'        '    'anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
'        '    If anzahlTreffer < 1 Then
'        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'        '        Return -1
'        '    Else
'        '        Return CInt(anzahlTreffer&)
'        '    End If
'        'Catch ex As Exception
'        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
'        '    Return -2
'        'End Try
'    End Function
'    Public Function RB_Umkreis_loeschen(ByVal umkreisid As Integer) As Integer
'        'Dim anzahlTreffer As Long, newid As Long
'        'Try
'        '    glob2.initTemprecAusVorgangRecOracle()
'        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="Paraumkreis"
'        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
'        '     "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'        '     " where id=" & umkreisid.ToString
'        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'        '    If anzahlTreffer < 1 Then
'        '        nachricht_und_Mbox("Problem beim löschen:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'        '        Return -1
'        '    Else
'        '        Return CInt(anzahlTreffer)
'        '    End If
'        'Catch ex As Exception
'        '    nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
'        '    Return -2
'        'End Try
'    End Function
'    Public Sub Umkreis_undVorgang_Entkoppeln(ByRef erfolg%)
'        '    erfolg = DBactionParadigma.Entkoppelung_Raumbezug_Vorgang(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.VorgangsID)
'        'erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID)
'        'If erfolg > 0 Then
'        '    My.Log.WriteEntry("Adresse wurde erfolgreich gelöscht")
'        'Else
'        '    My.Log.WriteEntry("Adresse wurde erfolgreich gelöscht")
'        '    nachricht_und_Mbox("Problem beim Löschen des Raumbezugs aus dem Vorgang. Abbruch.")
'        'End If
'    End Sub

'    Public Function RB_ParaUmkreis_holen(ByVal sekid as string) as  Boolean
'        'dim hinweis as string 
'        'Try
'        '    glob2.initTemprecAusVorgangRecOracle()
'        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraumkreis"
'        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
'        '     "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'        '     " where id=" & sekid$
'        '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
'        '    Return True
'        'Catch ex As Exception
'        '    nachricht_und_Mbox("Problem beim RB_Paraumkreis_holen: " & ex.ToString)
'        '    Return False
'        'End Try
'    End Function


' #Region "IDisposable Support"
'    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
'    Protected     Overridable     Sub Dispose(disposing As Boolean)
'        If Not Me.disposedValue Then
'            If disposing Then
'                MeineDBConnection.Dispose
'            End If
'        End If
'        Me.disposedValue = True
'    End Sub
'    Public Sub Dispose() Implements IDisposable.Dispose
'        Dispose(True)
'        GC.SuppressFinalize(Me)
'    End Sub
'#End Region
'End Class
