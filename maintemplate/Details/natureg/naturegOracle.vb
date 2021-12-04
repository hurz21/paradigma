'Imports LibDB
'Imports System.Data

'Public Class naturegOracle

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

'    Private Shared Sub avoidNUlls(ByVal nat As clsNatureg)
'        'If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
'        'If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
'        'If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
'        'If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
'        'If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.Bearbeiter.Initiale
'    End Sub
'    Shared Function setSQLbody() As String
'        Return " SET VORGANGSID=:VORGANGSID" & _
'             ",NUMMER=:NUMMER" & _
'             ",ART=:ART " & _
'             ",TYP=:TYP " & _
'             ",QUELLE=:QUELLE " & _
'             ",NOTIZ=:NOTIZ " & _
'             ",MASSNAHMENNR=:MASSNAHMENNR " & _
'             ",BESCHREIBUNG=:BESCHREIBUNG "
'    End Function
'    Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal nat As clsNatureg)
'        avoidNUlls(nat)
'        Try
'            With nat
'                com.Parameters.AddWithValue(":VORGANGSID", .VorgangsID)
'                com.Parameters.AddWithValue(":NUMMER", .nummer)
'                com.Parameters.AddWithValue(":ART", .art)
'                com.Parameters.AddWithValue(":TYP", .typ)
'                com.Parameters.AddWithValue(":BESCHREIBUNG", .beschreibung)
'                com.Parameters.AddWithValue(":MASSNAHMENNR", .MassnahmenNr)
'                com.Parameters.AddWithValue(":QUELLE", .Quelle.Trim)
'                com.Parameters.AddWithValue(":NOTIZ", .notiz.Trim)

'            End With
'            '  com.Parameters.AddWithValue(":VORGANGSID", vid)
'        Catch ex As Exception
'            nachricht("Fehler in setSQLParams beteiligte: " & ex.ToString)
'        End Try

'    End Sub

'    'Public Function Natureg_loeschen(ByVal natID As Integer) As Integer
'    '    Dim anzahlTreffer&
'    '    Dim newid& = -1
'    '    Try
'    '        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecOracle)
'    '        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
'    '        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
'    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="natureg"
'    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
'    '         "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'    '         "  where id=" & natID
'    '        anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'    '        'anzahlTreffer = 1 ' die tab erzeugt keinen return
'    '        If anzahlTreffer < 1 Then
'    '            nachricht_und_Mbox("Problem beim Natureg_loeschen:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'    '            Return -1
'    '        Else
'    '            Return CInt(anzahlTreffer)
'    '        End If
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("Beteiliten_loeschen: " & vbCrLf & ex.ToString)
'    '        Return -2
'    '    End Try
'    'End Function

'    Public Function Natureg_abspeichern_EditExtracted(ByVal lnat As clsNatureg) As Integer
'        Dim anzahlTreffer& = 0, hinweis$ = ""
'        Dim com As OracleCommand
'        Try
'            If lnat.ID < 1 Then
'                nachricht_und_Mbox("FEHLER:Natureg_abspeichern_EditExtracted updateid =0. Abbruch")
'                Return 0
'            End If
'            myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="natureg"
'            myGlobalz.sitzung.VorgangREC.mydb.SQL = _
'             "UPDATE  " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle & setSQLbody() & " WHERE ID=:ID"

'            MeineDBConnection.Open()
'            com = New OracleCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
'            setSQLParams(com, lnat)
'            com.Parameters.AddWithValue(":ID", lnat.ID)
'            anzahlTreffer = CInt(com.ExecuteNonQuery)
'            MeineDBConnection.Close()

'            If anzahlTreffer < 1 Then
'                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
'                Return -1
'            Else
'                Return CInt(anzahlTreffer)
'            End If
'        Catch ex As Exception
'            nachricht_und_Mbox("Bet4 Fehler beim Abspeichern: " & ex.ToString)
'            Return -2
'        End Try
'    End Function

'    Public Function natureg_abspeichern_Neu(ByVal lnat As clsNatureg) As Integer
'        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'        Dim com As OracleCommand
'        Try
'            myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="NATUREG"
'            Dim SQLUPDATE$ = _
'         String.Format("INSERT INTO {0} (VORGANGSID,NUMMER,ART,TYP,BESCHREIBUNG,QUELLE,NOTIZ,MASSNAHMENNR) " +
'                               " VALUES (:VORGANGSID,:NUMMER,:ART,:TYP,:BESCHREIBUNG,:QUELLE,:NOTIZ,:MASSNAHMENNR)",
'                                 myGlobalz.sitzung.VorgangREC.mydb.Tabelle)

'            SQLUPDATE$ = SQLUPDATE$ & " RETURNING ID INTO :R1"
'            MeineDBConnection.Open()
'            com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
'            setSQLParams(com, lnat)

'            newid = clsOracleIns.GetNewid(com, SQLUPDATE)
'            MeineDBConnection.Close()
'            Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
'        Catch ex As Exception
'            nachricht_und_Mbox("Bet5 Fehler beim Abspeichern: " & ex.ToString)
'            Return -2
'        End Try
'    End Function

'    Shared Function getNaturegDatatable(vorgangsid As Integer) As DataTable
'        Dim hinweis As String
'        Try
'            myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="natureg"
'            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle &
'                " where vorgangsid=" & vorgangsid
'            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
'            Return myGlobalz.sitzung.VorgangREC.dt
'        Catch ex As Exception
'            nachricht("fehler in getNaturegDatatable : " & ex.ToString)
'            Return Nothing
'        End Try
'    End Function

'End Class


