'Imports LibDB
'Imports System.Data
'Public Class bearbeiterORACLE
'    Implements IDisposable

'    Public MeineDBConnection As New OracleConnection
'    Sub New(ByVal conn As System.Data.Common.DbConnection)
'        MeineDBConnection = CType(conn, OracleConnection)
'    End Sub

'    'Private Shared Sub avoidNUlls()
'    '    If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PostfachPLZ) Then myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.PostfachPLZ = ""
'    '    If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Name) Then myGlobalz.sitzung.aktPerson.Name = " "
'    '    If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Vorname) Then myGlobalz.sitzung.aktPerson.Vorname = " "
'    '    If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Bezirk) Then myGlobalz.sitzung.aktPerson.Bezirk = " "
'    'End Sub
'    Shared Function setSQLbody() As String
'        Return " SET USERNAME=:USERNAME" & _
'         ",VORNAME=:VORNAME" & _
'         ",NACHNAME=:NACHNAME " & _
'         ",RANG=:RANG " & _
'         ",RITES=:RITES " & _
'         ",STDGRANTS=:STDGRANTS " & _
'         ",INITIAL_=:INITIAL_ " & _
'         ",AKTIV=:AKTIV " & _
'         ",ABTEILUNG=:ABTEILUNG " & _
'         ",TELEFON=:TELEFON " & _
'         ",FAX=:FAX " & _
'         ",KUERZEL1=:KUERZEL1 " & _
'         ",NAMENSZUSATZ=:NAMENSZUSATZ " & _
'         ",EMAIL=:EMAIL " & _
'         ",ROLLE=:ROLLE " & _
'         ",EXPANDHEADERINSACHGEBIET=:EXPANDHEADERINSACHGEBIET "
'    End Function
'    Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal lpers As clsBearbeiter)
'        Try
'            With lpers
'                If .STDGRANTS.Trim.IsNothingOrEmpty Then
'                    .STDGRANTS = " "
'                End If
'                If .Rites.Trim.IsNothingOrEmpty Then
'                    .Rites = " "
'                End If
'                com.Parameters.AddWithValue("USERNAME", .username.Trim)
'                com.Parameters.AddWithValue("NACHNAME", .Name.Trim)
'                com.Parameters.AddWithValue("VORNAME", .Vorname.Trim)
'                com.Parameters.AddWithValue("RANG", .Rang.Trim)
'                com.Parameters.AddWithValue("RITES", .Rites.Trim)
'                com.Parameters.AddWithValue("STDGRANTS", .STDGRANTS.Trim)
'                com.Parameters.AddWithValue("INITIAL_", .Initiale.Trim)
'                com.Parameters.AddWithValue("AKTIV", CInt(.Status))
'                com.Parameters.AddWithValue("ABTEILUNG", .Bemerkung)
'                com.Parameters.AddWithValue("TELEFON", .Kontakt.elektr.Telefon1.Trim)
'                com.Parameters.AddWithValue("FAX", .Kontakt.elektr.Fax1.Trim)
'                com.Parameters.AddWithValue("KUERZEL1", .Kuerzel2Stellig.Trim)
'                com.Parameters.AddWithValue("NAMENSZUSATZ", .Namenszusatz.Trim)
'                com.Parameters.AddWithValue("EMAIL", .Kontakt.elektr.Email.Trim)
'                com.Parameters.AddWithValue("ROLLE", .Rolle.Trim)
'                com.Parameters.AddWithValue("EXPANDHEADERINSACHGEBIET", .ExpandHeaderInSachgebiet.Trim)
'            End With
'        Catch ex As Exception
'            nachricht("Fehler in setSQLParams bearbeiter: " & ex.ToString)
'        End Try

'    End Sub

'    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:SQL-Abfragen auf Sicherheitsrisiken überprüfen")>Public Function bearbeiter_abspeichern_EditExtracted(ByVal lpers As clsBearbeiter) As Integer  'myGlobalz.sitzung.aktclsBearbeiter.clsBearbeiterenID
'        Dim anzahlTreffer& = 0 
'        Dim com As OracleCommand
'        Try

'            myGlobalz.sitzung.tempREC.mydb.Tabelle =CLstart.myViewsNTabs.tabBearbeiter
'            myGlobalz.sitzung.tempREC.mydb.SQL = _
'             "UPDATE  " & myGlobalz.sitzung.tempREC.mydb.Tabelle & setSQLbody() & " WHERE BEARBEITERID=:BEARBEITERID"  'MYGLOBALZ.SITZUNG.AKTPERSON.PERSONENID

'            MeineDBConnection.Open()
'            com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
'            setSQLParams(com, lpers)
'            com.Parameters.AddWithValue(":BEARBEITERID", CInt(lpers.ID))
'            anzahlTreffer& = CInt(com.ExecuteNonQuery)
'            MeineDBConnection.Close()

'            If anzahlTreffer < 1 Then
'                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'                Return -1
'            Else
'                Return CInt(anzahlTreffer)
'            End If
'        Catch ex As Exception
'            nachricht_und_Mbox("Bet4 Fehler beim Abspeichern: " & ex.ToString)
'            Return -2
'        End Try
'    End Function

'    Public Function Bearbeiter_abspeichern_Neu(ByVal lpers As clsBearbeiter) As Integer
'        Dim   newid& = 0
'        Dim com As OracleCommand
'        Try
'            myGlobalz.sitzung.tempREC.mydb.Tabelle =CLstart.myViewsNTabs.tabBearbeiter
'            Dim SQLUPDATE$ =
'         String.Format("INSERT INTO {0} (USERNAME,NACHNAME,VORNAME,RANG,RITES,STDGRANTS,INITIAL_,AKTIV,ABTEILUNG,TELEFON,FAX," +
'                       "KUERZEL1,NAMENSZUSATZ,EMAIL,ROLLE,EXPANDHEADERINSACHGEBIET) " +
'                               " VALUES (:USERNAME,:NACHNAME,:VORNAME,:RANG,:RITES,:STDGRANTS,:INITIAL_,:AKTIV,:ABTEILUNG,:TELEFON,:FAX," +
'                       ":KUERZEL1,:NAMENSZUSATZ,:EMAIL,:ROLLE,:EXPANDHEADERINSACHGEBIET)",
'                                 myglobalz.sitzung.tempREC.mydb.Tabelle)

'            SQLUPDATE$ = SQLUPDATE$ & " RETURNING BEARBEITERID INTO :R1"
'            MeineDBConnection.Open()
'            com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
'            setSQLParams(com, lpers)

'            newid = clsOracleIns.GetNewid(com, SQLUPDATE)
'            MeineDBConnection.Close()
'            Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
'        Catch ex As Exception
'            nachricht_und_Mbox("Bet5 Fehler beim Abspeichern: " & ex.ToString)
'            Return -2
'        End Try
'    End Function     



'#Region "IDisposable Support"
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


