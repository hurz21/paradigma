
Imports System.Data.Common
Imports Npgsql
Public Class clsCRUDpostgres
    ''Imports System.Data.OracleClient



 
    Implements IDisposable
    Public MeineDBConnection As New npgsqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, npgsqlConnection)
    End Sub

    Private disposed As Boolean = False
    'Implement IDisposable.
    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
    Protected Overridable Overloads Sub Dispose(disposing As Boolean)
        If disposed = False Then
            If disposing Then
                ' Free other state (managed objects).
                'dt.Dispose
                '_dt.Dispose
                MeineDBConnection.Dispose()
                disposed = True
            End If
            ' Free your own state (unmanaged objects).
            ' Set large fields to null.
        End If
    End Sub
    Protected Overrides Sub Finalize()
        ' Simply call Dispose(False).
        Dispose(False)
    End Sub
    'Private lokstamm As Stamm
    Shared Function setSQLBody() As String
        Return " set " & _
        " VORGANGSID=:VORGANGSID" & _
        ",BEARBEITER=:BEARBEITER" & _
        ",BEMERKUNG=:BEMERKUNG " & _
        ",BESCHREIBUNG=:BESCHREIBUNG " & _
        ",EINGANG=:EINGANG " & _
        ",AUFNAHME=:AUFNAHME " & _
        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
        ",ERLEDIGT=:ERLEDIGT " & _
        ",ORTSTERMIN=:ORTSTERMIN " & _
        ",STELLUNGNAHME=:STELLUNGNAHME " & _
        ",PROBAUGAZ=:PROBAUGAZ " & _
        ",ALTAZ=:ALTAZ " & _
        ",GEMKRZ=:GEMKRZ " & _
        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
        ",AZ2=:AZ2 " & _
        ",WEITEREBEARB=:WEITEREBEARB " & _
        ",ARCDIR=:ARCDIR " & _
        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
        ",ABGABEBA=:ABGABEBA " &
        ",GUTACHTENMIT=:GUTACHTENMIT " &
        ",HATRAUMBEZUG=:HATRAUMBEZUG " &
        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
        ",STORAUMNR=:STORAUMNR " &
        ",STOTITEL=:STOTITEL " &
        ",PARAGRAF=:PARAGRAF " &
        ",INTERNENR=:INTERNENR "
    End Function

    Shared Function setSQLBodySingleUpdate(SUfieldname As String) As String
        If String.IsNullOrEmpty(SUfieldname) Then Return ""
        Return " set " & " " & SUfieldname & "=:" & SUfieldname & ""
    End Function
    Shared Function setSQLBodyOHnePermanente() As String
        Return " set " & _
        " VORGANGSID=:VORGANGSID" & _
        ",BEARBEITER=:BEARBEITER" & _
        ",BEMERKUNG=:BEMERKUNG " & _
        ",BESCHREIBUNG=:BESCHREIBUNG " & _
        ",EINGANG=:EINGANG " & _
        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
        ",ERLEDIGT=:ERLEDIGT " & _
        ",ORTSTERMIN=:ORTSTERMIN " & _
        ",STELLUNGNAHME=:STELLUNGNAHME " & _
        ",PROBAUGAZ=:PROBAUGAZ " & _
        ",ALTAZ=:ALTAZ " & _
        ",GEMKRZ=:GEMKRZ " & _
        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
        ",AZ2=:AZ2 " & _
        ",WEITEREBEARB=:WEITEREBEARB " & _
        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
        ",ABGABEBA=:ABGABEBA " &
        ",GUTACHTENMIT=:GUTACHTENMIT " &
        ",HATRAUMBEZUG=:HATRAUMBEZUG " &
        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
        ",STORAUMNR=:STORAUMNR " &
        ",STOTITEL=:STOTITEL " &
         ",PARAGRAF=:PARAGRAF " &
        ",INTERNENR=:INTERNENR "
        '  ",ARCDIR=:ARCDIR " & _  sollte nicht mehr verändert werden
        '    ",AUFNAHME=:AUFNAHME " & _
    End Function

    Shared Sub setSQLParams(ByVal com As npgsqlCommand,
                            ByVal vid%, 
                            ByVal modus As String,
                            ByVal zeitstempel As Date)     'myGlobalz.sitzung.Vorgang.Stammdaten
       
        'com.Parameters.AddWithValue(":PARAGRAF", stamm.Paragraf)
        'com.Parameters.AddWithValue(":INTERNENR", stamm.InterneNr)
        'If modus = "neu" Then
        '    com.Parameters.AddWithValue(":AUFNAHME", stamm.Aufnahmedatum)
        '    com.Parameters.AddWithValue(":ARCDIR", stamm.ArchivSubdir)
        'End If
    End Sub

    'Public Function Neu_speichern_stammdaten(ByVal vorgangsREC As IDB_grundfunktionen,
    '   ByVal vid As Integer,
    '   ByVal zeitstempel As Date) As Boolean ' myGlobalz.sitzung.VorgangREC			 ,myGlobalz.sitzung.VorgangsID	 	,'myGlobalz.sitzung.Vorgang.Stammdaten
    '    Dim newid&
    '    Dim com As NpgsqlCommand
    '    Dim SQLupdate$ = ""
    '    Dim hinweis As String = ""
    '    Try
    '        'vorgangsREC.mydb.Tabelle ="stammdaten"
    '        SQLupdate$ =
    '           String.Format("INSERT INTO " & CLstart.myViewsNTabs.tabStammdaten &
    '                        " (VORGANGSID,BEARBEITER,BEMERKUNG,BESCHREIBUNG,EINGANG,AUFNAHME," +
    '                              "LETZTEBEARBEITUNG,ERLEDIGT,ORTSTERMIN,STELLUNGNAHME,PROBAUGAZ,ALTAZ," +
    '                              "GEMKRZ,LASTACTIONHEROE,AZ2,WEITEREBEARB,ARCDIR,DARFNICHTVERNICHTETWERDEN,GUTACHTENMIT,GUTACHTENDRIN," &
    '                              "STORAUMNR,STOTITEL,ABGABEBA,PARAGRAF,HATRAUMBEZUG,INTERNENR) " +
    '                        " VALUES (:VORGANGSID,:BEARBEITER,:BEMERKUNG,:BESCHREIBUNG,:EINGANG,:AUFNAHME," +
    '                             ":LETZTEBEARBEITUNG,:ERLEDIGT,:ORTSTERMIN,:STELLUNGNAHME,:PROBAUGAZ,:ALTAZ," +
    '                             ":GEMKRZ,:LASTACTIONHEROE,:AZ2,:WEITEREBEARB,:ARCDIR,:DARFNICHTVERNICHTETWERDEN,:GUTACHTENMIT," &
    '                             ":GUTACHTENDRIN,:STORAUMNR,:STOTITEL,:ABGABEBA,:PARAGRAF,:HATRAUMBEZUG,:INTERNENR)",
    '                              vorgangsREC.mydb.Tabelle)
    '        SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"

    '        nachricht("nach setSQLbody : " & SQLupdate)
    '        MeineDBConnection.Open()

    '        com = New NpgsqlCommand(SQLupdate$, MeineDBConnection)
    '        nachricht("vor setParams  ")
    '        setSQLParams(com, vid, "neu", zeitstempel)

    '        newid = clsPgsqlIns.GetNewid(com, SQLupdate)
    '        MeineDBConnection.Close()
    '        Return CBool(clsPgsqlIns.gebeNeuIDoderFehler(newid, SQLupdate))
    '    Catch ex As Exception
    '        nachricht_und_Mbox(String.Format("Stamm5 Fehler beim Abspeichern: {0}{1}", vbCrLf, ex))
    '        Return False
    '    End Try
    'End Function

    'Public Function EDIT_singleupdate_stammdaten(ByVal vid As Integer,
    '                     ByVal vorgangsREC As IDB_grundfunktionen, 
    '                     ByVal zeitstempel As Date,
    '                     singleUpdateFieldname As String) As Boolean    'myGlobalz.sitzung.VorgangsID,   myGlobalz.sitzung.VorgangREC			,'myGlobalz.sitzung.Vorgang.Stammdaten
    '    Dim anzahlTreffer As Long
    '    Dim com As npgsqlCommand
    '    Dim SQLupdate As String = ""
    '    Dim hinweis As String = ""
    '    Try
    '        If vid < 1 Then
    '            nachricht_und_Mbox("Fehler: Updateid<1. abbruch")
    '            Return False
    '        End If
    '        vorgangsREC.mydb.Tabelle ="stammdaten"
    '        SQLupdate$ = String.Format("UPDATE  {0}{1}  where VorgangsID=:VorgangsID",
    '                                   vorgangsREC.mydb.Tabelle,
    '                                   setSQLBodySingleUpdate(singleUpdateFieldname))

    '        MeineDBConnection.Open()
    '        com = New npgsqlCommand(SQLupdate$, MeineDBConnection) ' vorgangsREC.myconn)
    '        com.Parameters.AddWithValue(":VORGANGSID", vid%) 'MYGLObalz.sitzung.VorgangsID)



    '        'Select Case singleUpdateFieldname

    '        '    Case "ERLEDIGT"
    '        '        com.Parameters.AddWithValue(":ERLEDIGT", Convert.ToInt16(stamm.erledigt))
    '        '    Case "LASTACTIONHEROE"
    '        '        If stamm.LastActionHeroe.Length > 545 Then
    '        '            stamm.LastActionHeroe = stamm.LastActionHeroe.Substring(0, 540)
    '        '        End If
    '        '        com.Parameters.AddWithValue(":LASTACTIONHEROE", CStr(stamm.LastActionHeroe))
    '        '    Case "EINGANG"
    '        '        com.Parameters.AddWithValue(":EINGANG", CDate(stamm.Eingangsdatum))
    '        'End Select

    '        anzahlTreffer& = CInt(com.ExecuteNonQuery)
    '        MeineDBConnection.Close()
    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern:" & vorgangsREC.mydb.SQL)
    '            Return False
    '        End If
    '        Return True
    '    Catch ex As npgsqlException
    '        nachricht(String.Format("Fehler in EDIT_singleupdate_stammdaten : Stamm6 Fehler beim Abspeichern: {0}{1}: " &
    '                                singleUpdateFieldname, vbCrLf, ex))
    '        Return False
    '    End Try
    'End Function

    Public Function EDIT_speichern_stammdaten(ByVal vid%,
                             ByVal vorgangsREC As IDB_grundfunktionen, 
                             ByVal zeitstempel As Date) As Boolean    'myGlobalz.sitzung.VorgangsID,   myGlobalz.sitzung.VorgangREC			,'myGlobalz.sitzung.Vorgang.Stammdaten
        Dim anzahlTreffer&
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""
        Try
            If vid < 1 Then
                nachricht_und_Mbox("Fehler: Updateid<1. abbruch")
                Return False
            End If
            'If stamm.az.gesamt Is Nothing OrElse stamm.az.gesamt.Trim = String.Empty Then
            '    nachricht_und_Mbox("Fehler: aktenzeichen ist leer. abbruch")
            '    Return False
            'End If
            'If Not stamm.az.gesamt.Contains(vid.ToString) Then
            '    nachricht_und_Mbox("Fehler: aktenzeichen enhält nicht die korrekte paradigmanummer (" & vid & "). abbruch " & stamm.az.gesamt)
            '    Return False
            'End If
            'vorgangsREC.mydb.Tabelle ="stammdaten"
            'If stamm.anychange Or _
            'stamm.erledigt Then
            '    SQLupdate$ = String.Format("UPDATE  {0}{1}  where VorgangsID=:VorgangsID", vorgangsREC.mydb.Tabelle, setSQLBodyOHnePermanente())

            '    MeineDBConnection.Open()
            '    com = New OracleCommand(SQLupdate$, MeineDBConnection) ' vorgangsREC.myconn)
            '    setSQLParams(com, vid%, stamm, "edit", zeitstempel)

            '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
            '    MeineDBConnection.Close()

            'Else
            '    Return True 'keine änderungen vorhanden
            'End If

            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & vorgangsREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As NpgsqlException
            nachricht_und_Mbox(String.Format("fehler Stamm6 Fehler beim Abspeichern: {0}{1}", vbCrLf, ex))
            Return False
        End Try
    End Function

    Public Function DELETE_stammdaten(ByVal vid%,
                                        ByVal vorgangsREC As IDB_grundfunktionen) As Boolean    'myGlobalz.sitzung.VorgangsID,   myGlobalz.sitzung.VorgangREC			,'myGlobalz.sitzung.Vorgang.Stammdaten
        Dim anzahlTreffer&
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""
        Try
            If vid < 1 Then
                nachricht_und_Mbox("Fehler: Updateid<1. abbruch")
                Return False
            End If
            'vorgangsREC.mydb.Tabelle ="stammdaten"
            'If stamm.anychange Then
            '    SQLupdate$ = String.Format("DELETE from {0}  where VorgangsID=:VorgangsID", vorgangsREC.mydb.Tabelle)
            '    MeineDBConnection.Open()
            '    com = New OracleCommand(SQLupdate$, MeineDBConnection)
            '    '  setSQLParams(com, vid%, stamm)
            '    com.Parameters.AddWithValue(":VORGANGSID", vid%) 'MYGLObalz.sitzung.VorgangsID)
            '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
            '    MeineDBConnection.Close()
            'Else
            '    Return True 'keine änderungen vorhanden
            'End If

            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & vorgangsREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As npgsqlException
            nachricht_und_Mbox(String.Format("Stamm7 Fehler beim Abspeichern: {0}{1}", vbCrLf, ex))
            Return False
        End Try
    End Function

    Shared Sub nachricht(ByVal text$)
        'MsgBox(text)
        My.Log.WriteEntry(text)
    End Sub

    Shared Sub nachricht_und_Mbox(ByVal text$)
        MsgBox(text)
        My.Log.WriteEntry(text)
    End Sub
End Class
