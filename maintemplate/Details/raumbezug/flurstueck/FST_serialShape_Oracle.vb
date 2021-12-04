''Imports System.Data.OracleClient
'Imports LibDB
'Imports System.Data

'Public Class FST_serialShape_Oracle
'      Implements IDisposable
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
'    Public Function RB_Flurstueck_Serial_loeschen(ByVal raumbezugsid As Integer) As Integer
'        'Dim anzahlTreffer&, newid&
'        'Try
'        '    glob2.initTemprecAusVorgangRecOracle()
'        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabRaumbezug2geopolygon      & "   "
'        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
'        '     "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'        '     " where raumbezugsid=" & raumbezugsid% 'sollte eindeutig sein
'        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'        '    Return 1
'        '    'If anzahlTreffer < 1 Then
'        '    '    nachricht("Fehler: Problem beim Löschen: ggf. ist das Objekt bereits gelöscht b." & vbCrLf &
'        '    '                             myGlobalz.sitzung.tempREC.mydb.SQL)
'        '    '    Return -1
'        '    'Else
'        '    '    Return CInt(anzahlTreffer)
'        '    'End If
'        'Catch ex As Exception
'        '    nachricht_und_Mbox("Fehler: Problem beim löschen: " & ex.ToString)
'        '    Return -2
'        'End Try
'    End Function


'#Region "Serial"
'    Public Function RB_FLST_Serial_abspeichern_Neu(ByVal vid As Integer,
'                                                    ByVal rbid As Integer,
'                                                    ByVal serial As String,
'                                                    ByVal typ As Integer,
'                                                    ByVal area As Double) As Integer
'        '      Dim hinweis As String = ""
'        '      Dim newid As Long = 0
'        '      Dim com As OracleCommand
'        '      nachricht("RB_FLST_Serial_abspeichern_Neu -------------------------------------")
'        '      Try
'        '          myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecOracle)
'        '          myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
'        '          myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
'        '          myGlobalz.sitzung.tempREC.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabRaumbezug2geopolygon      & "   "
'        '          Dim SQLupdate As String =
'        'String.Format("INSERT INTO {0} (RAUMBEZUGSID,VORGANGSID,TYP,AREAQM,SERIALSHAPE) " +
'        '                      " VALUES (:RAUMBEZUGSID,:VORGANGSID,:TYP,:AREAQM,:SERIALSHAPE)",
'        '                        myGlobalz.sitzung.tempREC.mydb.Tabelle)
'        '          SQLupdate = SQLupdate & " RETURNING ID INTO :R1"
'        '          nachricht("nach setSQLbody : " & SQLupdate)
'        '          MeineDBConnection.Open()
'        '          nachricht("nach dboeffnen  ")
'        '          com = New OracleCommand(SQLupdate, MeineDBConnection)
'        '          nachricht("vor setParams  ")
'        '          setSQLParamsFLST_serial(com, vid, rbid, serial, 0, typ, area)
'        '          newid = clsOracleIns.GetNewid(com, SQLupdate)
'        '          MeineDBConnection.Close()
'        '          Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)
'        '      Catch mex As OracleException
'        '          nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu mex: " & vbCrLf & mex.ToString)
'        '          Return -2
'        '      Catch ex As Exception
'        '          nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu: " & vbCrLf & ex.ToString)
'        '          Return -2
'        '      End Try
'    End Function

'    Shared Function setSQLBodyFLST_serial() As String
'        Return " SET " & _
'         " RAUMBEZUGSID=:RAUMBEZUGSID" & _
'         ",VORGANGSID=:VORGANGSID" & _
'         ",TYP=:TYP" & _
'         ",AREAQM=:AREAQM" & _
'         ",SERIALSHAPE=:SERIALSHAPE"
'    End Function

'    Shared Sub setSQLParamsFLST_serial(ByVal com As OracleCommand, ByVal vid As Integer, ByVal rbid As Integer, ByVal serial As String, ByVal id As Integer, ByVal Typ As Integer, ByVal areaqm As Double)
'        com.Parameters.AddWithValue(":RAUMBEZUGSID", rbid)
'        com.Parameters.AddWithValue(":VORGANGSID", vid)
'        com.Parameters.AddWithValue(":SERIALSHAPE", serial)
'        com.Parameters.AddWithValue(":TYP", Typ)
'        com.Parameters.AddWithValue(":AREAQM", areaqm)
'        '   com.Parameters.AddWithValue(":ID", id)
'    End Sub

'    Sub serialInDbSpeichern(ByVal vid As Integer, ByVal rbid As Integer, ByVal typ As Integer, ByVal serial As String, ByVal Area As Double)
'        'vid,rbid%,typ%,serial$
'        nachricht("serialSpeichern: vid%:" & vid & "rbid: " & rbid & "serial: " & serial)
'        RB_FLST_Serial_abspeichern_Neu(vid, rbid, serial, typ, Area)
'    End Sub

'#End Region
'End Class
