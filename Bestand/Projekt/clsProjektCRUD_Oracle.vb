'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data
Public Class clsProjektCRUD_Oracle
      Implements IDisposable
    Public aktProjekt As clstart.clsProjektAllgemein
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection, ByVal _ap As clstart.clsProjektAllgemein)
        MeineDBConnection = CType(conn, OracleConnection)
        aktProjekt = _ap
    End Sub
    Shared Function setSQLbody() As String
        Return " SET KATEGORIE1=:KATEGORIE1" & _
         ",KATEGORIE2=:KATEGORIE2" & _
         ",BEZEICHNUNGKURZ=:BEZEICHNUNGKURZ " & _
         ",BEZEICHNUNGLAN=:BEZEICHNUNGLAN " & _
         ",VONDATUM=:VONDATUM " & _
         ",BISDATUM=:BISDATUM " & _
         ",QUELLE=:QUELLE " & _
         ",WIEDERVORLAGEID=:WIEDERVORLAGEID " & _
         ",REFNR=:REFNR " & _
         ",GEMEINDE=:GEMEINDE "
    End Function

    Private Sub defaultwertesetzen()
        If String.IsNullOrEmpty(aktProjekt.Gemeinde) Then aktProjekt.Gemeinde = ""
        If String.IsNullOrEmpty(aktProjekt.Kategorie1) Then aktProjekt.Kategorie1 = ""
        If String.IsNullOrEmpty(aktProjekt.Kategorie2) Then aktProjekt.Kategorie2 = ""
        If String.IsNullOrEmpty(aktProjekt.BezeichnungKurz) Then aktProjekt.BezeichnungKurz = ""
        If String.IsNullOrEmpty(aktProjekt.BezeichnungLang) Then aktProjekt.BezeichnungLang = ""
        If String.IsNullOrEmpty(aktProjekt.Quelle) Then aktProjekt.Quelle = ""
        If String.IsNullOrEmpty(aktProjekt.REFNR) Then aktProjekt.REFNR = ""
    End Sub
    Sub setSQLParams(ByRef com As OracleCommand)
        Try
            defaultwertesetzen()

            com.Parameters.AddWithValue(":KATEGORIE1", aktProjekt.Kategorie1)
            com.Parameters.AddWithValue(":KATEGORIE2", aktProjekt.Kategorie2)
            com.Parameters.AddWithValue(":BEZEICHNUNGKURZ", aktProjekt.BezeichnungKurz)
            com.Parameters.AddWithValue(":BEZEICHNUNGLAN", aktProjekt.BezeichnungLang)
            com.Parameters.AddWithValue(":VONDATUM", aktProjekt.von)
            com.Parameters.AddWithValue(":BISDATUM", aktProjekt.bis)
            com.Parameters.AddWithValue(":QUELLE", aktProjekt.Quelle)
            com.Parameters.AddWithValue(":WIEDERVORLAGEID", aktProjekt.WiedervorlageID) ') 
            com.Parameters.AddWithValue(":GEMEINDE", aktProjekt.Gemeinde) 
            com.Parameters.AddWithValue(":REFNR", aktProjekt.REFNR) 
        Catch ex As Exception
            nachricht("Fehler in setSQLParams beteiligte: " & ex.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' erwartet vorgangsid und quelle bereits auf dem objekt
    ''' </summary>
    ''' <returns>NEWID</returns>
    ''' <remarks></remarks>
    Public Function Projekt_abspeichern_Neu() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "PROJEKT"
            Dim SQLUPDATE$ = _
         String.Format("INSERT INTO {0} (KATEGORIE1,KATEGORIE2,BEZEICHNUNGKURZ,BEZEICHNUNGLAN,VONDATUM,BISDATUM,QUELLE,GEMEINDE,WIEDERVORLAGEID,REFNR) " +
                               " VALUES (:KATEGORIE1,:KATEGORIE2,:BEZEICHNUNGKURZ,:BEZEICHNUNGLAN,:VONDATUM,:BISDATUM,:QUELLE,:GEMEINDE,:WIEDERVORLAGEID,:REFNR)",
                                 myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
            SQLUPDATE$ = SQLUPDATE$ & " RETURNING ID INTO :R1"
            MeineDBConnection.Open()
            com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
            setSQLParams(com)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLUPDATE)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

 

    Public Function Projekt_abspeichern_Edit() As Integer  'myGlobalz.sitzung.aktPerson.PersonenID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        Try
            If aktProjekt.id < 1 Then
                nachricht_und_Mbox("FEHLER: Projekt_abspeichern_Edit updateid =0. Abbruch")
                Return 0
            End If
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "Projekt"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = _
             "UPDATE  " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle & _
             setSQLbody() & _
             " WHERE ID=:ID"  'MYGLOBALZ.SITZUNG.AKTPERSON.PERSONENID

            MeineDBConnection.Open()
            com = New OracleCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
            setSQLParams(com)
            com.Parameters.AddWithValue(":ID", aktProjekt.id)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()

            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function



 
 


    Function Projekt_koppeln() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "PROJEKT2VORGANG"
            Dim SQLUPDATE$ = _
         String.Format("INSERT INTO {0} (PROJEKTID,VORGANGSID) " +
                               " VALUES (:PROJEKTID,:VORGANGSID)",
                                 myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
            SQLUPDATE$ = SQLUPDATE$ & " RETURNING ID INTO :R1"
            MeineDBConnection.Open()
            com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
            com.Parameters.AddWithValue(":PROJEKTID", aktProjekt.id)
            com.Parameters.AddWithValue(":VORGANGSID", aktProjekt.vorgangsid)


            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLUPDATE)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Function Projekt_loeschen() As Boolean
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "projekt"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = _
             "delete from projekt " &
             "  where id=" & aktProjekt.id
            anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid) ')
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Beteiliten_loeschen:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Projekt_loeschen: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Function Projekt_entkoppeln() As Boolean
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "projekt2vorgang"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from projekt2vorgang where  vorgangsid=" & aktProjekt.vorgangsid & " and projektid=" & aktProjekt.id

            '"delete from projekt " &
            '"  where id=" & aktProjekt.id
            anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid) ')
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Beteiliten_loeschen:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Projekt_loeschen: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Function Projekt_status() As String
        dim hinweis as string 
        Dim newid& = -1
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "projekt2vorgang"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select vorgangsid from projekt2vorgang where  projektid=" & aktProjekt.id &
                " and vorgangsid<>" & aktProjekt.vorgangsid
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            If myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                Return ""
            Else
                hinweis = ""
                bildeHinweis(hinweis)
                If hinweis.EndsWith(", ") Then hinweis = hinweis.Substring(0, hinweis.Length - 2)
                Return hinweis
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Projekt_loeschen: " & vbCrLf & ex.ToString)
            Return ex.ToString
        End Try
    End Function
    Private Shared Sub bildeHinweis(ByRef hinweis$)
        For Each ritem As DataRow In myGlobalz.sitzung.VorgangREC.dt.Rows
            hinweis &= ritem.Item(0).ToString & ", "
        Next
    End Sub

    Function Projekt_fuerVorgang(ByVal vid As Integer) As String
        dim hinweis as string 
        Dim newid& = -1
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "projekt2vorgang"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select projektid from projekt2vorgang where vorgangsid =" & vid
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            If myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                Return "0"
            Else
                'hinweis = ""
                'bildeHinweis(hinweis)
                'If hinweis.EndsWith(", ") Then hinweis = hinweis.Substring(0, hinweis.Length - 2)
                'Return hinweis
                Return CStr(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item(0))
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Projekt_fuerVorgang: " & vbCrLf & ex.ToString)
            Return "0"
        End Try
    End Function
   
 #Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose
                aktProjekt = nothing
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class


