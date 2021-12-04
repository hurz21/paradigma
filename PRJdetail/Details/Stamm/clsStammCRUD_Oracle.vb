'Imports LibDB.LIBDB
'''Imports Devart.Data.Oracle



'Public Class clsStammCRUD_Oracle
'    Implements IDisposable
'    Public MeineDBConnection As New OracleConnection
'    Sub New(ByVal conn As System.Data.Common.DbConnection)
'        MeineDBConnection = CType(conn, OracleConnection)
'    End Sub

'    Private disposed As Boolean = False
'    'Implement IDisposable.
'    Public Overloads Sub Dispose() Implements IDisposable.Dispose
'        Dispose(True)
'        GC.SuppressFinalize(Me)
'    End Sub
'    Protected Overridable Overloads Sub Dispose(disposing As Boolean)
'        If disposed = False Then
'            If disposing Then
'                ' Free other state (managed objects).
'                'dt.Dispose
'                '_dt.Dispose
'                MeineDBConnection.Dispose()
'                disposed = True
'            End If
'            ' Free your own state (unmanaged objects).
'            ' Set large fields to null.
'        End If
'    End Sub
'    Protected Overrides Sub Finalize()
'        ' Simply call Dispose(False).
'        Dispose(False)
'    End Sub
'    'Private lokstamm As Stamm
'    Shared Function setSQLBody() As String
'        Return " set " & _
'        " VORGANGSID=:VORGANGSID" & _
'        ",BEARBEITER=:BEARBEITER" & _
'        ",BEMERKUNG=:BEMERKUNG " & _
'        ",BESCHREIBUNG=:BESCHREIBUNG " & _
'        ",EINGANG=:EINGANG " & _
'        ",AUFNAHME=:AUFNAHME " & _
'        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
'        ",ERLEDIGT=:ERLEDIGT " & _
'        ",ORTSTERMIN=:ORTSTERMIN " & _
'        ",STELLUNGNAHME=:STELLUNGNAHME " & _
'        ",PROBAUGAZ=:PROBAUGAZ " & _
'        ",ALTAZ=:ALTAZ " & _
'        ",GEMKRZ=:GEMKRZ " & _
'        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
'        ",AZ2=:AZ2 " & _
'        ",WEITEREBEARB=:WEITEREBEARB " & _
'        ",ARCDIR=:ARCDIR " & _
'        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
'        ",ABGABEBA=:ABGABEBA " &
'        ",GUTACHTENMIT=:GUTACHTENMIT " &
'        ",HATRAUMBEZUG=:HATRAUMBEZUG " &
'        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
'        ",STORAUMNR=:STORAUMNR " &
'        ",STOTITEL=:STOTITEL " &
'        ",PARAGRAF=:PARAGRAF " &
'        ",INTERNENR=:INTERNENR "
'    End Function

'    Shared Function setSQLBodySingleUpdate(SUfieldname As String) As String
'        If String.IsNullOrEmpty(SUfieldname) Then Return ""
'        Return " set " & " " & SUfieldname & "=:" & SUfieldname & ""
'    End Function
'    Shared Function setSQLBodyOHnePermanente() As String
'        Return " set " & _
'        " VORGANGSID=:VORGANGSID" & _
'        ",BEARBEITER=:BEARBEITER" & _
'        ",BEMERKUNG=:BEMERKUNG " & _
'        ",BESCHREIBUNG=:BESCHREIBUNG " & _
'        ",EINGANG=:EINGANG " & _
'        ",LETZTEBEARBEITUNG=:LETZTEBEARBEITUNG " & _
'        ",ERLEDIGT=:ERLEDIGT " & _
'        ",ORTSTERMIN=:ORTSTERMIN " & _
'        ",STELLUNGNAHME=:STELLUNGNAHME " & _
'        ",PROBAUGAZ=:PROBAUGAZ " & _
'        ",ALTAZ=:ALTAZ " & _
'        ",GEMKRZ=:GEMKRZ " & _
'        ",LASTACTIONHEROE=:LASTACTIONHEROE " &
'        ",AZ2=:AZ2 " & _
'        ",WEITEREBEARB=:WEITEREBEARB " & _
'        ",DARFNICHTVERNICHTETWERDEN=:DARFNICHTVERNICHTETWERDEN " &
'        ",ABGABEBA=:ABGABEBA " &
'        ",GUTACHTENMIT=:GUTACHTENMIT " &
'        ",HATRAUMBEZUG=:HATRAUMBEZUG " &
'        ",GUTACHTENDRIN=:GUTACHTENDRIN " &
'        ",STORAUMNR=:STORAUMNR " &
'        ",STOTITEL=:STOTITEL " &
'         ",PARAGRAF=:PARAGRAF " &
'        ",INTERNENR=:INTERNENR "
'        '  ",ARCDIR=:ARCDIR " & _  sollte nicht mehr verändert werden
'        '    ",AUFNAHME=:AUFNAHME " & _
'    End Function

'    Shared Sub setSQLParams(ByVal com As OracleCommand,
'                            ByVal vid%,
'                            ByVal stamm As Stamm,
'                            ByVal modus As String,
'                            ByVal zeitstempel As Date)     'myGlobalz.sitzung.Vorgang.Stammdaten
'        com.Parameters.AddWithValue(":VORGANGSID", vid%) 'MYGLObalz.sitzung.VorgangsID)
'        com.Parameters.AddWithValue(":BEARBEITER", stamm.hauptBearbeiter.Initiale.trim)
'        com.Parameters.AddWithValue(":BEMERKUNG", stamm.Bemerkung)
'        com.Parameters.AddWithValue(":BESCHREIBUNG", stamm.Beschreibung)
'        com.Parameters.AddWithValue(":EINGANG", stamm.Eingangsdatum)

'        com.Parameters.AddWithValue(":LETZTEBEARBEITUNG", zeitstempel)
'        com.Parameters.AddWithValue(":ERLEDIGT", Convert.ToInt16(stamm.erledigt))
'        com.Parameters.AddWithValue(":ORTSTERMIN", Convert.ToInt16(stamm.Ortstermin))
'        com.Parameters.AddWithValue(":STELLUNGNAHME", Convert.ToInt16(stamm.Stellungnahme))
'        com.Parameters.AddWithValue(":PROBAUGAZ", (stamm.Probaugaz))
'        com.Parameters.AddWithValue(":ALTAZ", (stamm.AltAz))
'        com.Parameters.AddWithValue(":GEMKRZ", stamm.GemKRZ)
'        com.Parameters.AddWithValue(":LASTACTIONHEROE", stamm.LastActionHeroe)
'        com.Parameters.AddWithValue(":AZ2", stamm.az.gesamt)
'        com.Parameters.AddWithValue(":WEITEREBEARB", stamm.WeitereBearbeiter)

'        com.Parameters.AddWithValue(":DARFNICHTVERNICHTETWERDEN", Convert.ToInt16(stamm.darfNichtVernichtetWerden))
'        com.Parameters.AddWithValue(":ABGABEBA", Convert.ToInt16(stamm.AbgabeBA))
'        com.Parameters.AddWithValue(":HATRAUMBEZUG", Convert.ToInt16(stamm.hatraumbezug))
'        com.Parameters.AddWithValue(":GUTACHTENMIT", Convert.ToInt16(stamm.meinGutachten.existiert))
'        com.Parameters.AddWithValue(":GUTACHTENDRIN", Convert.ToInt16(stamm.meinGutachten.UnterDokumente))
'        com.Parameters.AddWithValue(":STORAUMNR", stamm.Standort.RaumNr)
'        com.Parameters.AddWithValue(":STOTITEL", stamm.Standort.Titel)
'        com.Parameters.AddWithValue(":PARAGRAF", stamm.Paragraf)
'        com.Parameters.AddWithValue(":INTERNENR", stamm.InterneNr)
'        If modus = "neu" Then
'            com.Parameters.AddWithValue(":AUFNAHME", stamm.Aufnahmedatum)
'            com.Parameters.AddWithValue(":ARCDIR", stamm.ArchivSubdir)
'        End If
'    End Sub

'    'Public Function Neu_speichern_stammdaten(ByVal vorgangsREC As IDB_grundfunktionen,
'    '   ByVal vid As Integer,
'    '   ByVal stamm As Stamm,
'    '   ByVal zeitstempel As Date) As Boolean ' myGlobalz.sitzung.VorgangREC			 ,myGlobalz.sitzung.VorgangsID	 	,'myGlobalz.sitzung.Vorgang.Stammdaten
'    '    Dim newid&
'    '    Dim com As OracleCommand
'    '    Dim SQLupdate$ = ""
'    '    Dim hinweis As String = ""
'    '    Try
'    '        vorgangsREC.mydb.Tabelle ="stammdaten"
'    '        SQLupdate$ =
'    '           String.Format("INSERT INTO {0} (VORGANGSID,BEARBEITER,BEMERKUNG,BESCHREIBUNG,EINGANG,AUFNAHME," +
'    '                              "LETZTEBEARBEITUNG,ERLEDIGT,ORTSTERMIN,STELLUNGNAHME,PROBAUGAZ,ALTAZ," +
'    '                              "GEMKRZ,LASTACTIONHEROE,AZ2,WEITEREBEARB,ARCDIR,DARFNICHTVERNICHTETWERDEN,GUTACHTENMIT,GUTACHTENDRIN," &
'    '                              "STORAUMNR,STOTITEL,ABGABEBA,PARAGRAF,HATRAUMBEZUG,INTERNENR) " +
'    '                        " VALUES (:VORGANGSID,:BEARBEITER,:BEMERKUNG,:BESCHREIBUNG,:EINGANG,:AUFNAHME," +
'    '                             ":LETZTEBEARBEITUNG,:ERLEDIGT,:ORTSTERMIN,:STELLUNGNAHME,:PROBAUGAZ,:ALTAZ," +
'    '                             ":GEMKRZ,:LASTACTIONHEROE,:AZ2,:WEITEREBEARB,:ARCDIR,:DARFNICHTVERNICHTETWERDEN,:GUTACHTENMIT," &
'    '                             ":GUTACHTENDRIN,:STORAUMNR,:STOTITEL,:ABGABEBA,:PARAGRAF,:HATRAUMBEZUG,:INTERNENR)",
'    '                              vorgangsREC.mydb.Tabelle)
'    '        SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"

'    '        nachricht("nach setSQLbody : " & SQLupdate)
'    '        MeineDBConnection.Open()

'    '        com = New OracleCommand(SQLupdate$, MeineDBConnection)
'    '        nachricht("vor setParams  ")
'    '        setSQLParams(com, vid, stamm, "neu", zeitstempel)

'    '        newid = clsOracleIns.GetNewid(com, SQLupdate)
'    '        MeineDBConnection.Close()
'    '        Return CBool(clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate))
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox(String.Format("Stamm5 Fehler beim Abspeichern: {0}{1}", vbCrLf, ex))
'    '        Return False
'    '    End Try
'    'End Function

'    Public Function EDIT_singleupdate_stammdaten(ByVal vid As Integer,
'                         ByVal vorgangsREC As IDB_grundfunktionen,
'                         ByVal stamm As Stamm,
'                         ByVal zeitstempel As Date,
'                         singleUpdateFieldname As String) As Boolean    'myGlobalz.sitzung.VorgangsID,   myGlobalz.sitzung.VorgangREC			,'myGlobalz.sitzung.Vorgang.Stammdaten
'        Dim anzahlTreffer As Long
'        Dim com As OracleCommand
'        Dim SQLupdate As String = ""
'        Dim hinweis As String = ""
'        Try
'            If vid < 1 Then
'                nachricht_und_Mbox("Fehler: Updateid<1. abbruch")
'                Return False
'            End If
'            vorgangsREC.mydb.Tabelle ="stammdaten"
'            SQLupdate$ = String.Format("UPDATE  {0}{1}  where VorgangsID=:VorgangsID",
'                                       vorgangsREC.mydb.Tabelle,
'                                       setSQLBodySingleUpdate(singleUpdateFieldname))

'            MeineDBConnection.Open()
'            com = New OracleCommand(SQLupdate$, MeineDBConnection) ' vorgangsREC.myconn)
'            com.Parameters.AddWithValue(":VORGANGSID", vid%) 'MYGLObalz.sitzung.VorgangsID)



'            Select Case singleUpdateFieldname
'                Case "STELLUNGNAHME"
'                    com.Parameters.AddWithValue(":STELLUNGNAHME", Convert.ToInt16(stamm.Stellungnahme))
'                Case "ORTSTERMIN"
'                    com.Parameters.AddWithValue(":ORTSTERMIN", Convert.ToInt16(stamm.Ortstermin))

'                Case "BESCHREIBUNG"
'                    com.Parameters.AddWithValue(":BESCHREIBUNG", stamm.Beschreibung)
'                Case "HATRAUMBEZUG"
'                    com.Parameters.AddWithValue(":HATRAUMBEZUG", stamm.hatraumbezug)
'                Case "BEMERKUNG"
'                    com.Parameters.AddWithValue(":BEMERKUNG", stamm.Bemerkung)
'                Case "GEMKRZ"
'                    com.Parameters.AddWithValue(":GEMKRZ", stamm.GemKRZ)
'                Case "WEITEREBEARB"
'                    com.Parameters.AddWithValue(":WEITEREBEARB", stamm.WeitereBearbeiter)
'                Case "PROBAUGAZ"
'                    com.Parameters.AddWithValue(":PROBAUGAZ", stamm.Probaugaz)
'                Case "ALTAZ"
'                    com.Parameters.AddWithValue(":ALTAZ", stamm.AltAz)
'                Case "INTERNENR"
'                    com.Parameters.AddWithValue(":INTERNENR", stamm.InterneNr)
'                Case "PARAGRAF"
'                    com.Parameters.AddWithValue(":PARAGRAF", stamm.Paragraf)
'                Case "STORAUMNR"
'                    com.Parameters.AddWithValue(":STORAUMNR", stamm.Standort.RaumNr)
'                Case "GUTACHTENMIT"
'                    com.Parameters.AddWithValue(":GUTACHTENMIT", Convert.ToInt16(stamm.meinGutachten.existiert))
'                Case "GUTACHTENDRIN"
'                    com.Parameters.AddWithValue(":GUTACHTENDRIN", Convert.ToInt16(stamm.meinGutachten.UnterDokumente))
'                Case "DARFNICHTVERNICHTETWERDEN"
'                    com.Parameters.AddWithValue(":DARFNICHTVERNICHTETWERDEN", Convert.ToInt16(stamm.darfNichtVernichtetWerden))
'                Case "ERLEDIGT"
'                    com.Parameters.AddWithValue(":ERLEDIGT", Convert.ToInt16(stamm.erledigt))
'                Case "LETZTEBEARBEITUNG"
'                    com.Parameters.AddWithValue(":LETZTEBEARBEITUNG", zeitstempel)
'                Case "LASTACTIONHEROE"
'                    If stamm.LastActionHeroe.Length > 545 Then
'                        stamm.LastActionHeroe = stamm.LastActionHeroe.Substring(0, 540)
'                    End If
'                    com.Parameters.AddWithValue(":LASTACTIONHEROE", CStr(stamm.LastActionHeroe))
'                Case "EINGANG"
'                    com.Parameters.AddWithValue(":EINGANG", CDate(stamm.Eingangsdatum))
'            End Select

'            anzahlTreffer& = CInt(com.ExecuteNonQuery)
'            MeineDBConnection.Close()
'            If anzahlTreffer < 1 Then
'                nachricht_und_Mbox("Problem beim Abspeichern:" & vorgangsREC.mydb.SQL)
'                Return False
'            End If
'            Return True
'        Catch ex As OracleException
'            nachricht(String.Format("Fehler in EDIT_singleupdate_stammdaten : Stamm6 Fehler beim Abspeichern: {0}{1}: " &
'                                    singleUpdateFieldname, vbCrLf, ex))
'            Return False
'        End Try
'    End Function

'    Public Function EDIT_speichern_stammdaten(ByVal vid%,
'                             ByVal vorgangsREC As IDB_grundfunktionen,
'                             ByVal stamm As Stamm,
'                             ByVal zeitstempel As Date) As Boolean    'myGlobalz.sitzung.VorgangsID,   myGlobalz.sitzung.VorgangREC			,'myGlobalz.sitzung.Vorgang.Stammdaten
'        Dim anzahlTreffer&
'        Dim com As OracleCommand
'        Dim SQLupdate$ = ""
'        Dim hinweis As String = ""
'        Try
'            If vid < 1 Then
'                nachricht_und_Mbox("Fehler: Updateid<1. abbruch")
'                Return False
'            End If
'            If stamm.az.gesamt Is Nothing OrElse stamm.az.gesamt.Trim = String.Empty Then
'                nachricht_und_Mbox("Fehler: aktenzeichen ist leer. abbruch")
'                Return False
'            End If
'            If Not stamm.az.gesamt.Contains(vid.ToString) Then
'                nachricht_und_Mbox("Fehler: aktenzeichen enhält nicht die korrekte paradigmanummer (" & vid & "). abbruch " & stamm.az.gesamt)
'                Return False
'            End If
'            vorgangsREC.mydb.Tabelle ="stammdaten"
'            If stamm.anychange Or
'            stamm.erledigt Then
'                SQLupdate$ = String.Format("UPDATE  {0}{1}  where VorgangsID=:VorgangsID", vorgangsREC.mydb.Tabelle, setSQLBodyOHnePermanente())
'                MeineDBConnection.Open()
'                com = New OracleCommand(SQLupdate$, MeineDBConnection) ' vorgangsREC.myconn)
'                setSQLParams(com, vid%, stamm, "edit", zeitstempel)
'                anzahlTreffer& = CInt(com.ExecuteNonQuery)
'                MeineDBConnection.Close()
'            Else
'                Return True 'keine änderungen vorhanden
'            End If

'            If anzahlTreffer < 1 Then
'                nachricht_und_Mbox("Fehler beim Abspeichern:" & vorgangsREC.mydb.SQL)
'                Return False
'            Else
'                Return True
'            End If
'        Catch ex As OracleException
'            nachricht_und_Mbox(String.Format("Fehler beim Abspeichern: {0}{1} Stamm6 ", vbCrLf, ex))
'            Return False
'        End Try
'    End Function



'    Shared Sub nachricht(ByVal text$)
'        'MsgBox(text)
'        My.Log.WriteEntry(text)
'    End Sub

'    Shared Sub nachricht_und_Mbox(ByVal text$)
'        MsgBox(text)
'        My.Log.WriteEntry(text)
'    End Sub
'End Class
