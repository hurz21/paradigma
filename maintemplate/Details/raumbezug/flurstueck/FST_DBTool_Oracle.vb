''Imports System.Data.OracleClient
'Imports LibDB
'Imports System.Data

'Public Class FST_DBTool_Oracle

'  Implements IDisposable
'   #Region "IDisposable Support"
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
'    Public MeineDBConnection As New OracleConnection
'    Sub New(ByVal conn As System.Data.Common.DbConnection)
'        MeineDBConnection = CType(conn, OracleConnection)
'    End Sub
'    Shared Function setSQLBodyFLST() As String
'        Return " SET " & _
'         " GEMCODE=:GEMCODE" & _
'         ",FLUR=:FLUR" & _
'         ",ZAEHLER=:ZAEHLER" & _
'         ",NENNER=:NENNER" & _
'         ",ZNKOMBI=:ZNKOMBI" & _
'         ",GEMARKUNGSTEXT=:GEMARKUNGSTEXT" & _
'         ",FLAECHEQM=:FLAECHEQM" & _
'         ",FS=:FS"
'    End Function
'    Shared Sub SETSQLPARAMSFLST(ByVal COM As OracleCommand)
'        COM.Parameters.AddWithValue(":GEMCODE", myGlobalz.sitzung.aktFST.normflst.gemcode)
'        COM.Parameters.AddWithValue(":FLUR", myGlobalz.sitzung.aktFST.normflst.flur)
'        COM.Parameters.AddWithValue(":ZAEHLER", myGlobalz.sitzung.aktFST.normflst.zaehler)
'        COM.Parameters.AddWithValue(":NENNER", myGlobalz.sitzung.aktFST.normflst.nenner)
'        COM.Parameters.AddWithValue(":ZNKOMBI", myGlobalz.sitzung.aktFST.normflst.fstueckKombi)
'        COM.Parameters.AddWithValue(":GEMARKUNGSTEXT", myGlobalz.sitzung.aktFST.normflst.gemarkungstext)
'        COM.Parameters.AddWithValue(":FS", myGlobalz.sitzung.aktFST.normflst.FS)
'        COM.Parameters.AddWithValue(":FLAECHEQM", myGlobalz.sitzung.aktFST.normflst.flaecheqm)
'        ' com.Parameters.AddWithValue(":ID", sekid)
'    End Sub

'    Public Function RB_FLST_abspeichern_Edit(ByVal sekid as integer) as  Integer
'        'Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'        'Dim com As OracleCommand
'        'Try
'        '    If sekid < 1 Then
'        '        nachricht_und_Mbox("fehler updateid<1) RB_FLST_abspeichern_Edit")
'        '        Return 0
'        '    End If
'        '    glob2.initTemprecAusVorgangRecOracle()
'        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaFlurstueck"
'        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
'        '     "UPDATE " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'        '     setSQLBodyFLST() & _
'        '     " WHERE ID=:ID"
'        '    MeineDBConnection.Open()
'        '    com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
'        '    SETSQLPARAMSFLST(com)
'        '    com.Parameters.AddWithValue(":ID", sekid)
'        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
'        '    MeineDBConnection.Close()
'        '    '	anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
'        '    If anzahlTreffer < 1 Then
'        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'        '        Return -1
'        '    Else
'        '        Return CInt(anzahlTreffer)
'        '    End If
'        'Catch ex As Exception
'        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
'        '    Return -2
'        'End Try
'    End Function

'    Public Function RB_FLST_abspeichern_Neu() As Integer
'        'Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'        'Dim com As OracleCommand
'        'Try
'        '    glob2.initTemprecAusVorgangRecOracle()
'        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaFlurstueck"

'        '    Dim SQLUPDATE$ = String.Format("INSERT INTO {0} (GEMCODE,FLUR,ZAEHLER,NENNER,ZNKOMBI,GEMARKUNGSTEXT,FS,FLAECHEQM) " +
'        '                    " VALUES (:GEMCODE,:FLUR,:ZAEHLER,:NENNER,:ZNKOMBI,:GEMARKUNGSTEXT,:FS,:FLAECHEQM)",
'        '                      myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
'        '    SQLUPDATE$ = SQLUPDATE$ & " RETURNING ID INTO :R1"


'        '    MeineDBConnection.Open()
'        '    com = New OracleCommand(SQLUPDATE, MeineDBConnection)
'        '    SETSQLPARAMSFLST(com)
'        '    'com.CommandType = CommandType.Text
'        '    'Dim p_theid As New OracleParameter

'        '    'p_theid.DbType = DbType.Decimal
'        '    'p_theid.Direction = ParameterDirection.ReturnValue
'        '    'p_theid.ParameterName = ":r1"
'        '    'com.Parameters.Add(p_theid)
'        '    'Dim rtn = CInt(com.ExecuteNonQuery)
'        '    'newid = CLng(p_theid.Value)
'        '    'MeineDBConnection.Close()

'        '    newid = clsOracleIns.GetNewid(com, SQLUPDATE)
'        '    MeineDBConnection.Close()
'        '    Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)

'        '    'If newid < 1 Then
'        '    '    nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'        '    '    Return -1
'        '    'Else
'        '    '    Return CInt(newid)
'        '    'End If
'        'Catch ex As Exception
'        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
'        '    Return -2
'        'End Try
'    End Function

'    Public Shared Function RB_Flurstueck_loeschen(ByVal flurstuecksid as integer) as  Integer
'        'Dim anzahlTreffer&, newid&
'        'Try
'        '    glob2.initTemprecAusVorgangRecOracle()
'        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaFlurstueck"
'        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
'        '     "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'        '     " where id=" & flurstuecksid%
'        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'        '    If anzahlTreffer < 1 Then
'        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'        '        Return -1
'        '    Else
'        '        Return CInt(anzahlTreffer)
'        '    End If
'        'Catch ex As Exception
'        '    nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
'        '    Return -2
'        'End Try
'    End Function

'End Class
