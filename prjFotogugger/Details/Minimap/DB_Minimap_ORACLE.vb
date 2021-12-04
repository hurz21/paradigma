
''Imports Devart.Data.Oracle
'#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
'Imports LibDB
'#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
'Imports System.Data

'Public Class DB_Minimap_ORACLE

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

'    Sub New()
'        ' TODO: Complete member initialization 
'    End Sub

'    Shared Function setSQLBodyMAPCOOKIE() As String
'        Return " SET " & _
'         " VORGANGSID=:VORGANGSID" & _
'         ",VGRUND=:VGRUND" & _
'         ",HGRUND=:HGRUND" & _
'         ",AKTIVE_EBENE=:AKTIVE_EBENE" & _
'         ",XMIN=:XMIN" & _
'         ",XMAX=:XMAX" & _
'         ",YMIN=:YMIN" & _
'         ",YMAX=:YMAX"
'    End Function

'    Shared Sub SETSQLPARAMSMAPCOOKIE(ByVal COM As OracleCommand)
'        If String.IsNullOrEmpty(clstart.myc.kartengen.aktMap.ActiveLayer) Then
'            clstart.myc.kartengen.aktMap.ActiveLayer = ""
'        End If
'        COM.Parameters.AddWithValue(":VORGANGSID", myGlobalz.sitzung.aktVorgangsID)
'        COM.Parameters.AddWithValue(":VGRUND", clstart.myc.kartengen.aktMap.Vgrund)
'        COM.Parameters.AddWithValue(":HGRUND", clstart.myc.kartengen.aktMap.Hgrund)
'        COM.Parameters.AddWithValue(":AKTIVE_EBENE", clstart.myc.kartengen.aktMap.ActiveLayer)
'        COM.Parameters.AddWithValue(":XMIN", CLng(clstart.myc.kartengen.aktMap.aktrange.xl))
'        COM.Parameters.AddWithValue(":XMAX", CLng(clstart.myc.kartengen.aktMap.aktrange.xh))
'        COM.Parameters.AddWithValue(":YMIN", CLng(clstart.myc.kartengen.aktMap.aktrange.yl))
'        COM.Parameters.AddWithValue(":YMAX", CLng(clstart.myc.kartengen.aktMap.aktrange.yh))
'        ' com.Parameters.AddWithValue(":ID", sekid)
'    End Sub

'    'Public Function MAPCOOKIE_abspeichern_Edit(ByVal vorgangid As Integer) As Integer
'    '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'    '    Dim com As OracleCommand
'    '    Try
'    '        If vorgangid < 1 Then
'    '            nachricht_und_Mbox("fehler updateid<1) RB_MAPCOOKIE_abspeichern_Edit")
'    '            Return 0
'    '        End If

'    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="MAPCOOKIE"
'    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
'    '         "UPDATE " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'    '         setSQLBodyMAPCOOKIE() & _
'    '         " WHERE VORGANGSID=:VORGANGSID"
'    '        MeineDBConnection.Open()
'    '        com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
'    '        SETSQLPARAMSMAPCOOKIE(com)
'    '        com.Parameters.AddWithValue(":VORGANGSID", vorgangid)
'    '        anzahlTreffer = CInt(com.ExecuteNonQuery)
'    '        MeineDBConnection.Close()
'    '        '	anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
'    '        If anzahlTreffer < 1 Then
'    '            nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'    '            Return -1
'    '        Else
'    '            Return CInt(anzahlTreffer)
'    '        End If
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("Fehler beim Abspeichern: " ,ex)
'    '        Return -2
'    '    End Try
'    'End Function

'    'Public Function MAPCOOKIE_abspeichern_Neu() As Integer
'    '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'    '    Dim com As OracleCommand
'    '    Try

'    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="MAPCOOKIE"
'    '        Dim SQLUPDATE$ = String.Format("INSERT INTO {0} (VORGANGSID,VGRUND,HGRUND,AKTIVE_EBENE,XMIN,XMAX,YMIN,YMAX) " +
'    '                        " VALUES (:VORGANGSID,:VGRUND,:HGRUND,:AKTIVE_EBENE,:XMIN,:XMAX,:YMIN,:YMAX)",
'    '                          myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
'    '        SQLUPDATE$ = SQLUPDATE$ & " RETURNING ID INTO :R1"
'    '        MeineDBConnection.Open()
'    '        com = New OracleCommand(SQLUPDATE, MeineDBConnection)
'    '        SETSQLPARAMSMAPCOOKIE(com)
'    '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
'    '        MeineDBConnection.Close()
'    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("Fehler beim Abspeichern: " ,ex)
'    '        Return -2
'    '    End Try
'    'End Function



'    'Sub savemapcookie(ByVal clsMapSpec As clsMapSpec, ByVal vid As Integer)
'    '    If Mapcookieistschonvorhanden(vid) Then
'    '        Dim anzahl As Integer = MAPCOOKIE_abspeichern_Edit(vid)
'    '    Else
'    '        Dim newid As Integer = MAPCOOKIE_abspeichern_Neu()
'    '    End If
'    'End Sub

'    Private Shared Function MapcookieIstSchonVorhanden(ByVal vid As Integer) As Boolean
'        myGlobalz.sitzung.tempREC.mydb.SQL = "select * from mapcookie where vorgangsid=" & vid
'        Dim hinweis As String
'        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
'        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
'            Return False
'        Else
'            Return True
'        End If
'    End Function

'    Function getmapcookie(ByVal aktmap As clsMapSpec, ByVal vid As Integer) As Boolean
'        If MapcookieIstSchonVorhanden(vid) Then
'            ' Dim anzahl As Integer = MAPCOOKIE_abspeichern_Edit(vid)
'            ' die koordinaten wurde wg. ressel ausgeschaltet
'            'aktmap.aktrange.xl = CDbl(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("xmin")))
'            'aktmap.aktrange.xh = CDbl(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("xmax")))
'            'aktmap.aktrange.yl = CDbl(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("ymin")))
'            'aktmap.aktrange.yh = CDbl(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("ymax")))
'            aktmap.Vgrund = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("vgrund")))
'            aktmap.Hgrund = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Hgrund")))
'            aktmap.ActiveLayer = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("AKTIVE_EBENE")))
'            Return True
'        Else
'            Return False
'        End If
'    End Function





'End Class


