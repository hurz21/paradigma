''Imports System.Data.OracleClient
'Imports LibDB
'Imports System.Data
'Imports paradigma

'Public Class clsEreignisDB_Oracle
'    Implements IDisposable
'    Public MeineDBConnection As New OracleConnection
'    Sub New(ByVal conn As System.Data.Common.DbConnection)
'        MeineDBConnection = CType(conn, OracleConnection)
'    End Sub

'    Shared Function setSQLbody$()
'        Return " SET " &
'         " VORGANGSID=:VORGANGSID" &
'         ",BESCHREIBUNG=:BESCHREIBUNG" &
'         ",DATUM=:DATUM " &
'         ",ART=:ART " &
'         ",RICHTUNG=:RICHTUNG " &
'         ",NOTIZ=:NOTIZ " &
'         ",DOKUMENTID=:DOKUMENTID " &
'         ",PERSONENID=:PERSONENID " &
'         ",ISTRTF=:ISTRTF " &
'         ",QUELLE=:QUELLE "
'    End Function

'    Private Shared Sub setParams(ByVal com As OracleCommand, ByVal eid%, ByVal zielvorgangsid As Integer, ereignis As clsEreignis)
'        nachricht("ereignis setParams ---------------------------------")
'        nachricht("ereignis setParams :")
'        nachricht("ereignis setParams :")
'        nachricht("ereignis setParams :")
'        Dim zeiger = 1

'        If myGlobalz.sitzung Is Nothing Then nachricht("1")
'        If ereignis Is Nothing Then nachricht("2")
'        If myGlobalz.sitzung.aktBearbeiter Is Nothing Then nachricht("3")
'        'datum umsetzen für oracle
'        With ereignis 'myGlobalz.sitzung.aktEreignis
'            If String.IsNullOrEmpty(.Notiz) Then .Notiz = " "
'            If String.IsNullOrEmpty(.Beschreibung) Then .Beschreibung = ""
'            If String.IsNullOrEmpty(.Quelle) Then .Quelle = ""
'            If String.IsNullOrEmpty(.Art) Then .Art = ""

'            If .Art.Length > 41 Then .Art = .Art.Substring(0, 40)
'            If .Quelle.Length > 41 Then .Art = .Quelle.Substring(0, 40)
'            If .Beschreibung.Length > 499 Then .Art = .Beschreibung.Substring(0, 498)
'        End With

'        Try
'            With ereignis 'myGlobalz.sitzung.aktEreignis
'                com.Parameters.AddWithValue(":VORGANGSID", zielvorgangsid) : zeiger = 2
'                com.Parameters.AddWithValue(":BESCHREIBUNG", .Beschreibung) : zeiger = 3
'                com.Parameters.AddWithValue(":DATUM", CDate(.Datum)) : zeiger = 4
'                com.Parameters.AddWithValue(":ART", .Art) : zeiger = 5
'                com.Parameters.AddWithValue(":RICHTUNG", .Richtung) : zeiger = 6
'                com.Parameters.AddWithValue(":NOTIZ", .Notiz) : zeiger = 7
'                com.Parameters.AddWithValue(":DOKUMENTID", (.DokumentID)) : zeiger = 8
'                com.Parameters.AddWithValue(":PERSONENID", myGlobalz.sitzung.aktBearbeiter.PersonenID) : zeiger = 9
'                com.Parameters.AddWithValue(":QUELLE", ereignis.Quelle) : zeiger = 10
'                com.Parameters.AddWithValue(":ISTRTF", .istRTF) : zeiger = 11
'            End With
'            ' com.Parameters.AddWithValue(":ID", eid%) : zeiger = 11
'            nachricht("ereignis setParams ---------------- ende ----------------- 11? " & zeiger)
'        Catch ex As Exception
'            nachricht("fehler inereignis setParams " & "zeiger: " & zeiger & vbCrLf & ex.ToString)
'        End Try
'    End Sub

'    Public Function Edit_speichern_Ereignis(ByVal eid%, ByVal zielvorgangsid As Integer, ereignis As clsEreignis) As Boolean  'myGlobalz.sitzung.aktEreignis.ID
'        Dim anzahlTreffer& = 0, hinweis$ = ""
'        Dim com As OracleCommand
'        Dim SQLupdate$ = ""
'        Try
'            myGlobalz.sitzung.EreignisseRec.mydb.Tabelle ="ereignis"
'            If eid < 1 Then
'                nachricht_und_Mbox("FEHLER: Edit_speichern_Ereignis updateid =0. Abbruch. in Edit_speichern_Ereignis oracle")
'                Return False
'            End If

'            If myGlobalz.sitzung.aktEreignis.anychange Then
'                SQLupdate$ =
'                 "UPDATE " & myGlobalz.sitzung.EreignisseRec.mydb.Tabelle &
'                 setSQLbody() &
'                 " WHERE ID=:ID"

'                MeineDBConnection.Open()
'                com = New OracleCommand(SQLupdate$, MeineDBConnection) 'myGlobalz.sitzung.EreignisseRec.myconn)
'                writeParams(ereignis, zielvorgangsid)
'                setParams(com, eid, zielvorgangsid, ereignis)
'                com.Parameters.AddWithValue(":ID", eid%)
'                anzahlTreffer& = CInt(com.ExecuteNonQuery)
'                MeineDBConnection.Close()
'            Else
'                Return True 'keine änderungen vorhanden     
'            End If
'            If anzahlTreffer < 1 Then
'                nachricht("Problem beim Abspeichern:" & myGlobalz.sitzung.EreignisseRec.mydb.SQL)
'                Return False
'            Else
'                Return True
'            End If
'        Catch ex As Exception
'            nachricht("Fehler beim Abspeichern: " & ex.ToString)
'            Return False
'        End Try
'    End Function


'    Public Function Neu_speichern_Ereignis(ByVal zielvorgangsid As Integer, ByVal modus As String, ereignis As clsEreignis) As Integer ' myGlobalz.sitzung.Ereignismodus
'        nachricht("Neu_speichern_Ereignis -----------------------------------------------------")
'        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
'        Dim com As OracleCommand
'        Dim SQLupdate$ = ""
'        Try
'            myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="ereignis"
'            If String.IsNullOrEmpty(modus) Then
'                nachricht("Fehler: ereingismodus ist nicht nothing")
'                Return -3
'            End If
'            If modus.ToLower = "neu" Then
'                SQLupdate$ = String.Format("INSERT INTO {0} (VORGANGSID,BESCHREIBUNG,DATUM,ART,RICHTUNG,NOTIZ,DOKUMENTID,PERSONENID,QUELLE,ISTRTF) " +
'                                      " VALUES (:VORGANGSID,:BESCHREIBUNG,:DATUM,:ART,:RICHTUNG,:NOTIZ,:DOKUMENTID,:PERSONENID,:QUELLE,:ISTRTF)",
'                                        myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
'                SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"

'                nachricht("nach setSQLbody : " & SQLupdate)
'                MeineDBConnection.Open()
'                nachricht("nach dboeffnen  ")

'                com = New OracleCommand(SQLupdate$, MeineDBConnection)
'                nachricht("vor setParams  ")
'                writeParams(ereignis, zielvorgangsid)
'                setParams(com, 0, zielvorgangsid, ereignis)

'                newid = clsOracleIns.GetNewid(com, SQLupdate)
'                MeineDBConnection.Close()
'            End If
'            If newid < 1 Then
'                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
'                Return -1
'            Else
'                myGlobalz.sitzung.aktEreignis.ID = CInt(newid)
'                nachricht("Neu_speichern_Ereignis funzt")
'                ereignis.ID = CInt(newid)
'                Return CInt(newid)
'            End If
'        Catch ex As Exception
'            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
'            myGlobalz.sitzung.aktEreignis.ID = 0I
'            Return -2
'        End Try
'    End Function

'    Private Sub writeParams(ereignis As clsEreignis, zielid As Integer)
'        Try
'            With ereignis 'myGlobalz.sitzung.aktEreignis
'                nachricht("writeParams------------")
'                nachricht(":zielid" & zielid)
'                nachricht(":DATUM" & CDate(.Datum))
'                nachricht(":ART" & .Art)
'                nachricht(":RICHTUNG" & .Richtung)
'                nachricht(":NOTIZ" & .Notiz)
'                nachricht(":DOKUMENTID" & (.DokumentID))
'                nachricht(":PERSONENID" & myGlobalz.sitzung.aktBearbeiter.PersonenID)
'                nachricht(":QUELLE" & ereignis.Quelle)
'                nachricht(":ISTRTF" & .istRTF)
'            End With
'            nachricht("writeParams ---------------- ende ----------------- 11?")
'        Catch ex As Exception
'            nachricht("fehlerwriteParams " & " : " & vbCrLf & ex.ToString)
'        End Try
'    End Sub

'    Public Function Ereignis_loeschen(ByVal ereignisid As Integer) As Integer
'        Dim anzahlTreffer&, newid&
'        Try
'            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, IDB_grundfunktionen)
'            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
'            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
'            myGlobalz.sitzung.tempREC.mydb.Tabelle ="ereignis"
'            myGlobalz.sitzung.tempREC.mydb.SQL = _
'             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
'             " where id=" & ereignisid%.ToString
'            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'            If anzahlTreffer < 1 Then
'                nachricht_und_Mbox("Problem beim Ereignis_loeschen:" & myGlobalz.sitzung.tempREC.mydb.SQL)
'                Return -1
'            Else
'                nachricht("Ereignis wurde gelöscht! id: " & ereignisid%)
'                Return CInt(anzahlTreffer)
'            End If
'        Catch ex As Exception
'            nachricht_und_Mbox("Fehler beim Ereignis_loeschen: " & ex.ToString)
'            Return -2
'        End Try
'    End Function
'    ''' <summary>
'    ''' Entkoppelt alle dokumente vom ereignis
'    ''' </summary>
'    ''' <param name="ereignisID"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Function EntKoppelung_Dokumente_Ereignis(ByVal ereignisID As Integer) As Integer
'        Dim anzahlTreffer&
'        Dim newid& = -1
'        Try
'            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, IDB_grundfunktionen)
'            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
'            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
'            myGlobalz.sitzung.tempREC.mydb.Tabelle ="dokument2ereignis"
'            myGlobalz.sitzung.tempREC.mydb.SQL = _
'             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " where " & _
'             " ereignisID=" & ereignisID%
'            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'            If anzahlTreffer < 1 Then
'                nachricht("Problem beim EntKoppelung_Dokumente_Ereignis:" & myGlobalz.sitzung.tempREC.mydb.SQL & vbCrLf & "Evtl. konnten keine Dokumente zum Ereignis gefunden werden!" & anzahlTreffer)
'                Return -1
'            Else
'                Return CInt(anzahlTreffer)
'            End If
'        Catch ex As Exception
'            nachricht_und_Mbox("Fehler beim EntKoppelung_Dokumente_Ereignis: " & vbCrLf & ex.ToString)
'            Return -2
'        End Try
'    End Function
'    ''' <summary>
'    ''' entkoppelt nur ein dokument vom ereignis
'    ''' </summary>
'    ''' <param name="dokumentid"></param>
'    ''' <param name="ereignisID"></param>
'    ''' <returns></returns>
'    ''' <remarks></remarks>
'    Public Function EntKoppelung_Dokument_Ereignis(ByVal dokumentid%, ByVal ereignisID As Integer) As Integer
'        Dim anzahlTreffer&
'        Dim newid& = -1
'        Try
'            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, IDB_grundfunktionen)
'            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
'            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
'            myGlobalz.sitzung.tempREC.myd.Tabelle ="dokument2ereignis"
'            myGlobalz.sitzung.tempREC.mydb.SQL = _
'             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " where " & _
'             " ereignisID=" & ereignisID% &
'            " and  dokumentid=" & dokumentid%
'            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
'            If anzahlTreffer < 1 Then
'                nachricht("Darf auch 0 sein, weil nicht zu jedem ereignis auch ein dokument existiert.  :" & myGlobalz.sitzung.tempREC.mydb.SQL)
'                Return -1
'            Else
'                Return CInt(newid)
'            End If
'        Catch ex As Exception
'            nachricht_und_Mbox("Problem beim EntKoppelung_Dokument_Ereignis: " & vbCrLf & ex.ToString)
'            Return -2
'        End Try
'    End Function

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

'End Class
