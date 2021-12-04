
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.


'Public Class clsDS
'    'Implements IDisposable
'#Region "IDisposable Support"
'    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
'    Protected Overridable Sub Dispose(disposing As Boolean)
'        If Not Me.disposedValue Then
'            If disposing Then
'                MeineDBConnection.Dispose()
'            End If
'        End If
'        Me.disposedValue = True
'    End Sub
'    Public Sub Dispose() Implements IDisposable.Dispose
'        Dispose(True)
'        GC.SuppressFinalize(Me)
'    End Sub
'#End Region
'    'Public MeineDBConnection As New OracleConnection
'    'Sub New(ByVal conn As System.Data.Common.DbConnection)
'    '    MeineDBConnection = CType(conn, OracleConnection)
'    'End Sub

'    'Private Shared Sub avoidNUlls(ByVal nat As cls_ds_vorgangsgruppe)
'    '    'If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
'    '    'If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
'    '    'If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
'    '    'If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
'    '    'If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.Bearbeiter.Initiale
'    'End Sub
'    'Shared Function setSQLbody() As String
'    '    Return " SET VORGANGSID=:VORGANGSID" & _
'    '                ",gruppentext=:gruppentext"
'    'End Function
'    'Sub setSQLParams(ByRef com As OracleCommand, ByVal vgr As cls_ds_vorgangsgruppe)
'    '    avoidNUlls(vgr)
'    '    Try
'    '        With vgr
'    '            com.Parameters.AddWithValue(":VORGANGSID", .vid)
'    '            com.Parameters.AddWithValue(":gruppentext", (.VIDgruppentext))
'    '        End With
'    '    Catch ex As Exception
'    '        nachricht("Fehler in setSQLParams beteiligte: " ,ex)
'    '    End Try
'    'End Sub

'    'Public Function vorgangsgruppe_abspeichern_EditExtracted(ByVal vgr As cls_ds_vorgangsgruppe,
'    '                                                         ByVal vorgangsREC As IDB_grundfunktionen) As Integer
'    '    Dim anzahlTreffer& = 0, hinweis$ = ""
'    '    Dim com As OracleCommand
'    '    Try
'    '        If vgr.vid < 1 Then
'    '            nachricht_und_Mbox("FEHLER:vorgangsgruppe_abspeichern_EditExtracted updateid =0. Abbruch")
'    '            Return 0
'    '        End If
'    '        vorgangsREC.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & " "
'    '        vorgangsREC.mydb.SQL =
'    '         "UPDATE  " & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & "  " & " SET VORGANGSID=VORGANGSID" &
'    '                ",gruppentext=@gruppentext" & " WHERE ID=:ID"

'    '        MeineDBConnection.Open()
'    '        com = New OracleCommand(vorgangsREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
'    '        setSQLParams(com, vgr)
'    '        com.Parameters.AddWithValue(":ID", vgr.id)
'    '        anzahlTreffer = CInt(com.ExecuteNonQuery)
'    '        MeineDBConnection.Close()
'    '        If anzahlTreffer < 1 Then
'    '            nachricht_und_Mbox("Problem beim Abspeichern:" & vorgangsREC.mydb.SQL)
'    '            Return -1
'    '        Else
'    '            Return CInt(anzahlTreffer)
'    '        End If
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("Bet4 Fehler beim Abspeichern: " ,ex)
'    '        Return -2
'    '    End Try
'    'End Function


'    'Public Function vorgangsgruppe_abspeichern_Neu(ByVal vgr As cls_ds_vorgangsgruppe,
'    '                                               ByVal vorgangsREC As IDB_grundfunktionen) As Integer
'    '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'    '    Dim com As OracleCommand
'    '    Try
'    '        vorgangsREC.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & " "

'    '        Dim SQLUPDATE$ = _
'    '     String.Format("INSERT INTO {0} (VORGANGSID,gruppentext) " &
'    '                           " VALUES (:VORGANGSID,:gruppentext)",
'    '                              vorgangsREC.mydb.Tabelle)

'    '        SQLUPDATE$ = SQLUPDATE$ & " RETURNING ID INTO :R1"
'    '        MeineDBConnection.Open()
'    '        com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
'    '        setSQLParams(com, vgr)

'    '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
'    '        MeineDBConnection.Close()
'    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("vorgangsgruppe Fehler beim Abspeichern: " ,ex)
'    '        Return -2
'    '    End Try
'    'End Function

'    'Function getvorgangsgruppeDatatable(ByVal vid As Integer, ByVal vorgangsREC As IDB_grundfunktionen) As DataTable
'    '    Dim hinweis As String
'    '    Try
'    '        vorgangsREC.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & " "
'    '        vorgangsREC.mydb.SQL = "select * from " & vorgangsREC.mydb.Tabelle &
'    '             " where vorgangsid=" & vid
'    '        hinweis = vorgangsREC.getDataDT()
'    '        Return vorgangsREC.dt
'    '    Catch ex As Exception
'    '        nachricht("fehler in getvorgangsgruppeDatatable : " ,ex)
'    '        Return Nothing
'    '    End Try
'    'End Function

'    'Private Sub setSQLParamsSTANDARDS(com As OracleCommand, standard_gruppentext As String, bearbeiterid As Integer)

'    '    Try
'    '        com.Parameters.AddWithValue(":BEARBEITERID", bearbeiterid)
'    '        com.Parameters.AddWithValue(":gruppentext", (standard_gruppentext))
'    '    Catch ex As Exception
'    '        nachricht("Fehler in setSQLParams beteiligte: " ,ex)
'    '    End Try
'    'End Sub

'    'Function getgruppen4userDatatable(bearbeiterid As Integer, vorgangsREC As IDB_grundfunktionen) As DataTable
'    '    Dim hinweis As String
'    '    Try
'    '        vorgangsREC.mydb.Tabelle =""
'    '        vorgangsREC.mydb.SQL = "select g.name,u.bearbeiterid,u.gruppenid from  " & CLstart.myViewsNTabs.tabDS_USER2GRUPPE & "  u, " & CLstart.myViewsNTabs.tabDS_Gruppen & "  g " &
'    '                " where bearbeiterid= " & bearbeiterid &
'    '                " and u.gruppenid=g.gruppenid"
'    '        hinweis = vorgangsREC.getDataDT()
'    '        Return vorgangsREC.dt
'    '    Catch ex As Exception
'    '        nachricht("fehler in getDS_standard : " ,ex)
'    '        Return Nothing
'    '    End Try
'    'End Function

'    'Private Sub nachricht_und_Mbox(p1 As String)
'    '    Try
'    '        My.Log.WriteEntry(p1)
'    '    Catch ex As Exception

'    '    End Try

'    'End Sub
'    'Private Sub nachricht(p1 As String)
'    '    Try
'    '        My.Log.WriteEntry(p1)
'    '    Catch ex As Exception

'    '    End Try

'    'End Sub

'End Class
