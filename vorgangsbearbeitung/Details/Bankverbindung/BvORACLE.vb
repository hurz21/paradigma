
Imports LibDB
Imports System.Data

Public Class BvORACLE
    Shared Function getbvDatatable(personenid As Integer) As DataTable

        Try
            'myGlobalz.sitzung.tempREC2.mydb.Tabelle ="bankverbindung"
            myGlobalz.sitzung.tempREC2.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabbankverbindung & "  " &
                " where personenid=" & personenid &
                " and vorlage1<1"
            myGlobalz.sitzung.tempREC2.getDataDT()
            Return myGlobalz.sitzung.tempREC2.dt
        Catch ex As Exception
            nachricht("fehler in getkostenDatatable : " & ex.ToString)
            Return Nothing
        End Try
    End Function
End Class
'    'Implements IDisposable
'    'Public MeineDBConnection As New OracleConnection
'    'Sub New(ByVal conn As System.Data.Common.DbConnection)
'    '    MeineDBConnection = CType(conn, OracleConnection)
'    'End Sub

'    Private Shared Sub avoidNUlls(ByVal nat As clsBankverbindungSEPA)
'        'If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
'        'If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
'        'If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
'        'If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
'        'If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.Bearbeiter.Initiale
'    End Sub
'        Shared Function setSQLbodyAt() As String 
'        Return " set IBAN=@IBAN" & _
'                ",BIC=@BIC" & _
'                ",BANKNAME=@BANKNAME " & _
'                ",QUELLE=@QUELLE " & _
'                ",PERSONENID=@PERSONENID " & _   
'                ",TS=@TS "  &
'                ",VORLAGE1=@VORLAGE1 "  &
'                ",TITEL=@TITEL "
'    End Function
'    Shared Function setSQLbody() As String 
'        Return " set IBAN=:IBAN" & _
'                ",BIC=:BIC" & _
'                ",BANKNAME=:BANKNAME " & _
'                ",QUELLE=:QUELLE " & _
'                ",PERSONENID=:PERSONENID " & _   
'                ",TS=:TS "  &
'                ",VORLAGE1=:VORLAGE1 "  &
'                ",TITEL=:TITEL "
'    End Function
'    Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal nat As clsBankverbindungSEPA)
'        avoidNUlls(nat)
'        Try
'            With nat
'                com.Parameters.AddWithValue(":IBAN", .IBAN)
'                com.Parameters.AddWithValue(":BIC", (.BIC))
'                com.Parameters.AddWithValue(":BANKNAME", (.BankName))
'                com.Parameters.AddWithValue(":QUELLE", (.Quelle))
'                com.Parameters.AddWithValue(":PERSONENID", (.personenID))
'                com.Parameters.AddWithValue(":TITEL", (.Titel))
'                com.Parameters.AddWithValue(":TS", (.tss))
'                 com.Parameters.AddWithValue(":VORLAGE1", (.istVORLAGE1))
'            End With
'        Catch ex As Exception
'            nachricht("Fehler in setSQLParams bv: " & ex.ToString)
'        End Try

'    End Sub

'    'Public Function bv_loeschen(ByVal BANKKONTOID As Integer) As Integer
'    '    Dim anzahlTreffer&
'    '    Dim newid& = -1
'    '    Try
'    '        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecOracle)
'    '        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
'    '        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
'    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="bankverbindung"
'    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
'    '         "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'    '         "  where BANKKONTOID=" & BANKKONTOID
'    '        anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'    '        'anzahlTreffer = 1 ' die tab erzeugt keinen return
'    '        If anzahlTreffer < 1 Then
'    '            nachricht_und_Mbox("Problem beim kosten_loeschen:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'    '            Return -1
'    '        Else
'    '            Return CInt(anzahlTreffer)
'    '        End If
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("kosten_loeschen: " & vbCrLf & ex.ToString)
'    '        Return -2
'    '    End Try
'    'End Function

'    'Public Function bv_abspeichern_Edit(ByVal lnat As clsBankverbindungSEPA) As Integer
'    '    Dim anzahlTreffer& = 0
'    '    Dim com As OracleCommand
'    '    Try
'    '        If lnat.BANKKONTOID < 1 Then
'    '            nachricht_und_Mbox("FEHLER:bv_abspeichern_Edit updateid =0. Abbruch")
'    '            Return 0
'    '        End If
'    '        myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="bankverbindung"
'    '        myGlobalz.sitzung.VorgangREC.mydb.SQL = "UPDATE  " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle & setSQLbody() &
'    '                " WHERE BANKKONTOID=:BANKKONTOID"
'    '         MeineDBConnection.Open()
'    '        com = New OracleCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)  
'    '        setSQLParams(com, lnat)
'    '        com.Parameters.AddWithValue(":BANKKONTOID", lnat.BANKKONTOID)
'    '        anzahlTreffer = CInt(com.ExecuteNonQuery)
'    '        MeineDBConnection.Close()

'    '        If anzahlTreffer < 1 Then
'    '            nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
'    '            Return -1
'    '        Else
'    '            Return CInt(anzahlTreffer)
'    '        End If
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("bv_abspeichern_Edit Fehler beim Abspeichern: " & ex.ToString)
'    '        Return -2
'    '    End Try
'    'End Function

'    'Public Function bv_abspeichern_Neu(ByVal lnat As clsBankverbindungSEPA) As Integer
'    '    Dim    newid& = 0
'    '    Dim com As OracleCommand
'    '    Try
'    '        myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="bankverbindung"

'    '        Dim SQLUPDATE$ = _
'    '     String.Format("INSERT INTO {0} (IBAN,BIC,BANKNAME,QUELLE,PERSONENID,TITEL,TS,VORLAGE1) " &
'    '                           " VALUES (:IBAN,:BIC,:BANKNAME,:QUELLE,:PERSONENID,:TITEL,:TS,:VORLAGE1)",
'    '                             myGlobalz.sitzung.VorgangREC.mydb.Tabelle)

'    '        SQLUPDATE$ = SQLUPDATE$ & " RETURNING BANKKONTOID INTO :R1"
'    '        MeineDBConnection.Open()
'    '        com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
'    '        setSQLParams(com, lnat)

'    '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
'    '        MeineDBConnection.Close()
'    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("kosten Fehler beim Abspeichern: " & ex.ToString)
'    '        Return -2
'    '    End Try
'    'End Function



'Shared Function getbvVorlagenDatatable() As DataTable 

'        Try
'            myGlobalz.sitzung.tempREC.mydb.Tabelle ="bankverbindung"
'            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle &
'                " where vorlage1 !=0" 
'             myGlobalz.sitzung.tempREC.getDataDT()
'            Return myGlobalz.sitzung.tempREC.dt
'        Catch ex As Exception
'            nachricht("fehler in getkostenDatatable : " & ex.ToString)
'            Return Nothing
'        End Try
' End Function
'    '#Region "IDisposable Support"
'    '    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
'    '    Protected     Overridable     Sub Dispose(disposing As Boolean)
'    '        If Not Me.disposedValue Then
'    '            If disposing Then
'    '                MeineDBConnection.Dispose
'    '            End If
'    '        End If
'    '        Me.disposedValue = True
'    '    End Sub
'    'Public Sub Dispose() Implements IDisposable.Dispose
'    '    Dispose(True)
'    '    GC.SuppressFinalize(Me)
'    'End Sub

'End Class
