#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Public Class clsGesetzesOracle
    Implements IDisposable
#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose()
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, OracleConnection)
    End Sub

    Private Shared Sub avoidNUlls(gesetz As clsgesetzesManagerDok)
        'If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
        'If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
        'If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
        'If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
        'If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.Bearbeiter.Initiale
    End Sub
    Shared Function setSQLbody() As String
        Return " SET DATEINAMEOHNEEXT=:DATEINAMEOHNEEXT" &
                    ",ART=:ART" &
                    ",DATEITYP=:DATEITYP" &
                    ",ORDNER=:ORDNER" &
                    ",BESCHREIBUNG=:BESCHREIBUNG" &
                    ",QUELLENTYP=:QUELLENTYP" &
                    ",herkunft=:herkunft" &
                    ",WANNVEROEFFENTLICHT=:WANNVEROEFFENTLICHT" &
                    ",SCHLAGWORTE=:SCHLAGWORTE" &
                    ",URL=:URL" &
                    ",QUELLE=:QUELLE" &
                    ",ORIGINALNAME=:ORIGINALNAME" &
                    ",ISTGUELTIG=:ISTGUELTIG"
    End Function
    Sub setSQLParams(ByRef com As OracleCommand, ByVal gesetz As clsgesetzesManagerDok)
        avoidNUlls(gesetz)
        Try
            ' QUELLENTYP,: ART,: WANNVEROEFFENTLICHT,: URL,: TITEL
            'com.Parameters.AddWithValuE(":ART", GESETZ.ART)
            com.Parameters.AddWithValue(":BESCHREIBUNG", (gesetz.beschreibung))
            com.Parameters.AddWithValue(":QUELLENTYP", (gesetz.quellentyp))
            com.Parameters.AddWithValue(":DATEINAMEOHNEEXT", (gesetz.dateinameohneext))
            com.Parameters.AddWithValue(":DATEITYP", (gesetz.dateityp))
            com.Parameters.AddWithValue(":ART", (gesetz.artId))
            com.Parameters.AddWithValue(":ORDNER", (gesetz.ordner))
            com.Parameters.AddWithValue(":QUELLE", (gesetz.userInitial))
            com.Parameters.AddWithValue(":HERKUNFT", (gesetz.herkunftId))
            com.Parameters.AddWithValue(":WANNVEROEFFENTLICHT", (gesetz.wannveroeffentlicht))
            com.Parameters.AddWithValue(":SCHLAGWORTE", (gesetz.schlagworte))
            com.Parameters.AddWithValue(":URL", (gesetz.url))
            com.Parameters.AddWithValue(":ISTGUELTIG", (gesetz.istgueltig))
            com.Parameters.AddWithValue(":ORIGINALNAME", (gesetz.originalDateiName))
        Catch ex As Exception
            nachricht("Fehler in setSQLParams beteiligte: " ,ex)
        End Try
    End Sub




    'Public Function rechtsdb_stamm_abspeichern_EditExtracted(ByVal gesetz As clsgesetzesManagerDok,
    '                                                         ByVal vorgangsREC As IDB_grundfunktionen) As Integer
    '    Dim anzahlTreffer& = 0, hinweis$ = ""
    '    Dim com As OracleCommand
    '    Try
    '        If gesetz.stammid < 1 Then
    '            nachricht_und_Mbox("FEHLER:vorgangsgruppe_abspeichern_EditExtracted updateid =0. Abbruch")
    '            Return 0
    '        End If
    '        vorgangsREC.mydb.Tabelle ="rechtsdb_stamm"
    '        vorgangsREC.mydb.SQL = "UPDATE  rechtsdb_stamm " & vorgangsREC.mydb.Tabelle & setSQLbody() & " WHERE STID=:STID"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(vorgangsREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
    '        setSQLParams(com, gesetz)
    '        com.Parameters.AddWithValue(":STID", gesetz.stammid)
    '        anzahlTreffer = CInt(com.ExecuteNonQuery)
    '        MeineDBConnection.Close()
    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern:" & vorgangsREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(anzahlTreffer)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Bet4 Fehler beim Abspeichern: " ,ex)
    '        Return -2
    '    End Try
    'End Function


    'Public Function rechtsdb_sachgebiet_abspeichern_edit(ByVal stammid As Integer, sachgebietnr As String, sgheader As String,
    '                                                         ByVal vorgangsREC As IDB_grundfunktionen) As Integer
    '    Dim anzahlTreffer& = 0, newid& = 0
    '    Dim com As OracleCommand
    '    Try
    '        If stammid < 1 Then
    '            nachricht_und_Mbox("FEHLER:rechtsdb_sachgebiet_abspeichern_edit updateid =0. Abbruch")
    '            Return 0
    '        End If

    '        vorgangsREC.mydb.Tabelle ="RECHTSDB_SACHGEBIET"
    '        vorgangsREC.mydb.SQL = "UPDATE RECHTSDB_SACHGEBIET SET SACHGEBIETNR=:SACHGEBIETNR" &
    '                                        ",SGHEADER=:SGHEADER" &
    '                                        " WHERE STAMMID=:STAMMID"
    '        'vorgangsREC.mydb.SQL = "UPDATE  " & vorgangsREC.mydb.Tabelle & SQLbody & " WHERE STAMMID=:STAMMID"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(vorgangsREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
    '        setSQLParamsSG(com, stammid, sachgebietnr, sgheader)
    '        anzahlTreffer = CInt(com.ExecuteNonQuery)
    '        'newid = clsOracleIns.GetNewid(com, SQLbody)
    '        MeineDBConnection.Close()
    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern:" & vorgangsREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(anzahlTreffer)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("rechtsdb_sachgebiet_abspeichern_edit Fehler beim Abspeichern: " ,ex)
    '        Return -2
    '    End Try
    'End Function


    'Public Function rechtsdb_sachgebiet_abspeichern_Neu(ByVal stammid As Integer,
    '                                                    sachgebietnr As String,
    '                                                    sgheader As String) As Integer
    '    Dim anzahlTreffer& = 0, newid& = 0
    '    Dim com As OracleCommand
    '    Try

    '        Dim SQLUPDATE As String = "INSERT INTO RECHTSDB_SACHGEBIET (STAMMID,SACHGEBIETNR,SGHEADER ) " &
    '                                                 " VALUES (:STAMMID,:SACHGEBIETNR,:SGHEADER )"
    '        SQLUPDATE = SQLUPDATE & " RETURNING SID INTO :R1"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(SQLUPDATE, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
    '        setSQLParamsSG(com, stammid, sachgebietnr, sgheader)
    '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
    '        MeineDBConnection.Close()
    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
    '    Catch ex As Exception
    '        nachricht_und_Mbox("vorgangsgruppe Fehler beim Abspeichern: " ,ex)
    '        Return -2
    '    End Try
    'End Function

    'Private Sub setSQLParamsSG(com As OracleCommand, stammid As Integer, sachgebietnr As String, sgheader As String)
    '    com.Parameters.AddWithValue(":STAMMID", stammid)
    '    com.Parameters.AddWithValue(":SACHGEBIETNR", sachgebietnr)
    '    com.Parameters.AddWithValue(":SGHEADER", sgheader)
    '    'com.Parameters.AddWithValue(":SID", 41)
    'End Sub

    'Public Function rechtsdb_stamm_abspeichern_Neu(ByVal gesetz As clsgesetzesManagerDok) As Integer
    '    Dim anzahlTreffer& = 0, newid& = 0
    '    Dim com As OracleCommand
    '    Try
    '        Dim SQLUPDATE As String = "INSERT INTO RECHTSDB_STAMM (DATEINAMEOHNEEXT,DATEITYP,ORDNER,QUELLE,BESCHREIBUNG,QUELLENTYP," &
    '                                                              "ART,WANNVEROEFFENTLICHT,URL,SCHLAGWORTE,HERKUNFT,ISTGUELTIG,ORIGINALNAME) " &
    '                           " VALUES (:DATEINAMEOHNEEXT,:DATEITYP,:ORDNER,:QUELLE,:BESCHREIBUNG,:QUELLENTYP," &
    '                                    ":ART,:WANNVEROEFFENTLICHT,:URL,:SCHLAGWORTE,:HERKUNFT,:ISTGUELTIG,:ORIGINALNAME)"
    '        SQLUPDATE = SQLUPDATE & " RETURNING STID INTO :R1"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(SQLUPDATE, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
    '        setSQLParams(com, gesetz)
    '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
    '        MeineDBConnection.Close()
    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
    '    Catch ex As Exception
    '        nachricht_und_Mbox("vorgangsgruppe Fehler beim Abspeichern: " ,ex)
    '        Return -2
    '    End Try
    'End Function

    Function getgesetzDatatable(ByVal vid As Integer, ByVal vorgangsREC As IDB_grundfunktionen) As DataTable
        Dim hinweis As String
        Try
            'vorgangsREC.mydb.Tabelle ="rechtsdb_stamm"
            vorgangsREC.mydb.SQL = "select * from t39 as rechtsdb_stamm where vorgangsid=" & vid
            hinweis = vorgangsREC.getDataDT()
            Return vorgangsREC.dt
        Catch ex As Exception
            nachricht("fehler in getvorgangsgruppeDatatable : " ,ex)
            Return Nothing
        End Try
    End Function


    'Private Sub nachricht_und_Mbox(p1 As String)
    '    My.Log.WriteEntry(p1)
    'End Sub
    Private Sub nachricht(p1 As String)
        My.Log.WriteEntry(p1)
    End Sub

    Public Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = text & ToLogString(ex, text)
        'myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        'mitFehlerMail(text, anhang)
    End Sub
End Class


