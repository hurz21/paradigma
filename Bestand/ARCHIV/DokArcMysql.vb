
Imports MySql.Data.MySqlClient
Public Class DokArcMysql
    
  Implements IDisposable
   #Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub
    Public Function updateDokumentMetadata(ByVal dokumentid%,
                                            ByVal veraltet As Boolean,
                                            ByVal Beschreibung$,
                                            ByVal revisionssicher As Boolean) As Integer
        Dim anzahlTreffer& = 0
        Dim com As MySqlCommand
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
        Try
            If dokumentid < 1 Then
                nachricht_und_Mbox("1FEHLER: updateAktuellesDokument updateid =0. Abbruch")
                Return 0
            End If
            SQLupdate$ = _
             "Update " & myGlobalz.Arc.ArcRec.mydb.Tabelle & " set " & _
             " veraltet=@veraltet" & _
             ",Beschreibung=@Beschreibung " & _
             ",revisionssicher=@revisionssicher " &
              ",Initial=@Initial" &
             " where dokumentid=@dokumentid"
            myGlobalz.Arc.ArcRec.dboeffnen(hinweis$)
            com = New MySqlCommand(SQLupdate$, MeineDBConnection)
            com.Parameters.AddWithValue("@veraltet", Convert.ToBoolean(veraltet)) 'myGlobalz.sitzung.aktDokument.istVeraltet,myGlobalz.sitzung.aktDokument.Beschreibung	
            com.Parameters.AddWithValue("@Beschreibung", Beschreibung$)           'myGlobalz.sitzung.aktDokument.Beschreibung		
            com.Parameters.AddWithValue("@dokumentid", dokumentid)
            com.Parameters.AddWithValue("@Initial", myGlobalz.sitzung.aktBearbeiter.Initiale)
            com.Parameters.AddWithValue("@revisionssicher", revisionssicher)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            myGlobalz.Arc.ArcRec.dbschliessen(hinweis$)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernf:" & myGlobalz.Arc.ArcRec.mydb.SQL)
                Return 0
            Else
                Return CInt(anzahlTreffer&)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Dok11 Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function
    Public Function loescheDokumentInDb(ByVal dokumentid As Integer) As Integer
        Dim anzahlTreffer& = 0
        Dim com As MySqlCommand
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""
        nachricht("dokumentenarchiv loescheDokument -------------------------------" & vbCrLf)
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
        Try
            If dokumentid < 1 Then
                nachricht_und_Mbox("2FEHLER: loescheDokument updateid =0. Abbruch")
                Return 0
            End If
            SQLupdate$ = _
             "delete from " & myGlobalz.Arc.ArcRec.mydb.Tabelle &
             " where dokumentid=@dokumentid"
            myGlobalz.Arc.ArcRec.dboeffnen(hinweis$)
            com = New MySqlCommand(SQLupdate$, MeineDBConnection)
            com.Parameters.AddWithValue("@dokumentid", dokumentid)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            myGlobalz.Arc.ArcRec.dbschliessen(hinweis$)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim löschen:" & myGlobalz.Arc.ArcRec.mydb.SQL)
                Return 0
            Else
                Return CInt(anzahlTreffer&)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim löschen: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function


    Public Function entKoppelung_Dokument_Vorgang(ByVal dokumentID%, ByVal vorgangID As Integer) As Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.tempREC = CType(CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL), IDB_grundfunktionen)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokument2vorgang"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " where  " & _
             " Vorgangsid=" & vorgangID% & _
             " and dokumentID=" & dokumentID%
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeicherng:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Dok12 Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function Koppelung_Dokument_Ereignis(ByVal dokumentID%, ByVal ereignisID As Integer) As Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.tempREC = CType(CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL), IDB_grundfunktionen)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokument2ereignis"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "insert into " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " set " & _
             " dokumentID=" & dokumentID% & _
             ",ereignisID=" & ereignisID%
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernh:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Dok13Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function
    Public Function Koppelung_Dokument_Vorgang(ByVal dokumentID%, ByVal vorgangID As Integer) As Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.tempREC = CType(CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL), IDB_grundfunktionen)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokument2vorgang"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "insert into " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " set " & _
             " Vorgangsid=" & vorgangID% & _
             ",dokumentID=" & dokumentID%
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeicherni:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Dok14 Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function checkin_Dokumente(ByVal aktjpg As clsMyJPG,
                                      ByVal relativpfad$, _
                                      ByVal Beschreibung$,
                                      ByVal OriginalFullname$,
                                      ByVal OriginalName As String) As Integer
        Dim anzahlTreffer& = 0
        Dim com As MySqlCommand
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""

        Dim newid& = -1
        Dim fi As New IO.FileInfo(aktjpg.fullname)
        Dim fo As New IO.FileInfo(OriginalFullname$)
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
        Try
            'If myGlobalz.sitzung.modus = "neu" Then
            'mit firstuser		
            SQLupdate$ = "insert into " & myGlobalz.Arc.ArcRec.mydb.Tabelle & _
             setSQLBody()
            myGlobalz.Arc.ArcRec.dboeffnen(hinweis$)
            com = New MySqlCommand(SQLupdate$, MeineDBConnection)
            setSQLParams(com, relativpfad$, fi, Beschreibung$, aktjpg, OriginalFullname$, OriginalName$, fo, False)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            com.CommandText = "Select LAST_INSERT_ID()"
            newid = CLng(com.ExecuteScalar)
            myGlobalz.Arc.ArcRec.dbschliessen(hinweis$)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernj:" & myGlobalz.Arc.ArcRec.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Dok15 Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function



    Public Function alleDokuszuVorgangsid_inDatatable() As Boolean
        Try
            myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
            myGlobalz.Arc.ArcRec.mydb.SQL = "select * from " & myGlobalz.Arc.ArcRec.mydb.Tabelle & _
             " where vorgangsid=" & myGlobalz.sitzung.aktVorgangsID
            nachricht(myGlobalz.Arc.ArcRec.getDataDT())
            If myGlobalz.Arc.ArcRec.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox(String.Format("Keine Dokumente zur VorgangsID:{0} gefunden!", myGlobalz.sitzung.aktVorgangsID))
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function alleDokuszuEreignis_inDatatable() As Boolean
        Try
            myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
            myGlobalz.Arc.ArcRec.mydb.SQL = "select * from " & myGlobalz.Arc.ArcRec.mydb.Tabelle & _
             " where ereignisid=" & myGlobalz.sitzung.aktVorgangsID
            Dim hinweis As String = myGlobalz.Arc.ArcRec.getDataDT()
            If myGlobalz.Arc.ArcRec.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox(String.Format("Keine Dokumente zur VorgangsID:{0} gefunden!", myGlobalz.sitzung.aktVorgangsID))
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Shared Function setSQLBody() As String
        Return " set " & _
        " relativpfad=@relativpfad" & _
        ",Dateinameext=@Dateinameext" & _
        ",Typ=@Typ " & _
        ",Beschreibung=@Beschreibung " & _
        ",checkindatum=@checkindatum " & _
        ",Filedatum=@Filedatum " & _
        ",EXIFdatum=@EXIFdatum " & _
        ",EXIFlong=@EXIFlong " & _
        ",EXIFlat=@EXIFlat " & _
        ",EXIFdir=@EXIFdir " & _
        ",EXIFhersteller=@EXIFhersteller " & _
        ",OriginalFullname=@OriginalFullname " & _
        ",Initial=@Initial " &
        ",revisionssicher=@revisionssicher " & _
        ",OriginalName=@OriginalName "
    End Function

    Shared Sub setSQLParams(ByVal com As MySqlCommand,
                            ByVal relativpfad As string,
                            ByVal fi As IO.FileInfo,
                            ByVal Beschreibung  As string,
                            ByVal aktjpg As clsMyJPG,
                            ByVal OriginalFullname As string,
                            ByVal OriginalName As string,
                            ByVal fo As IO.FileInfo,
                            ByVal revisionssicher As Boolean)
        com.Parameters.AddWithValue("@relativpfad", relativpfad$.Replace("\", "/"))
        com.Parameters.AddWithValue("@Dateinameext", fi.Name)
        com.Parameters.AddWithValue("@Typ", fi.Extension.Replace(".", ""))
        com.Parameters.AddWithValue("@Beschreibung", Beschreibung)
        com.Parameters.AddWithValue("@checkindatum", Convert.ToDateTime(Format(DateTime.Now(), "yyyy-MM-dd HH:mm:ss")))
        com.Parameters.AddWithValue("@Filedatum", Convert.ToDateTime(Format(fo.CreationTime, "yyyy-MM-dd HH:mm:ss")))
        com.Parameters.AddWithValue("@EXIFdatum", Convert.ToDateTime(Format(aktjpg.EXIFDateTimeOriginal, "yyyy-MM-dd HH:mm:ss")))
        com.Parameters.AddWithValue("@EXIFlong", aktjpg.Exifgpslongitude)
        com.Parameters.AddWithValue("@EXIFlat", aktjpg.Exifgpslatitude)
        com.Parameters.AddWithValue("@EXIFdir", aktjpg.ExifGpsImgDir)
        com.Parameters.AddWithValue("@EXIFhersteller", aktjpg.EXIFhersteller)
        com.Parameters.AddWithValue("@OriginalFullname", OriginalFullname)
        com.Parameters.AddWithValue("@Initial", myGlobalz.sitzung.aktBearbeiter.Initiale)
        com.Parameters.AddWithValue("@OriginalName", OriginalName)
        com.Parameters.AddWithValue("@revisionssicher", revisionssicher)
    End Sub



 
 


    Public Sub initdokumentDT_by_SQLstring(ByVal sql$)
        myGlobalz.Arc.ArcRec.mydb.Host = myGlobalz.Arc.ArcRec.mydb.Host
        myGlobalz.Arc.ArcRec.mydb.Schema = myGlobalz.Arc.ArcRec.mydb.Schema
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
        myGlobalz.Arc.ArcRec.mydb.SQL = sql$
        nachricht(myGlobalz.Arc.ArcRec.getDataDT())
        If myGlobalz.Arc.ArcRec.mycount < 1 Then
            nachricht("Keine raumbezugsRec gespeichert!")
        Else
            nachricht(String.Format("initdokumentDT_by_SQLstring: {0} Dokumente vorhanden", myGlobalz.Arc.ArcRec.mycount))
        End If
    End Sub

    Public Function initDokumente4VorgangDatatable(ByVal VorgangsID As Integer) As Boolean     'myGlobalz.sitzung.VorgangsID
        'zuerst die personenIDs holen	  
        'If viaKopplung_DokumentIDs_VorgangID(VorgangsID.ToString) Then
        '    myGlobalz.sitzung.DokumentIDsDT = myGlobalz.sitzung.tempREC.dt.Copy
        Dim SQL$ = "SELECT * from dok2vid where VorgangsID=" & VorgangsID
        '  SQL = clsDBtools.UNION_SQL_erzeugenInn(myGlobalz.sitzung.DokumentIDsDT, "dokumente", 1, "dokumentID")
        initdokumentDT_by_SQLstring(SQL$)
        Return True
        'Else
        '    nachricht("Es konnten keine Dokumente zu diesem Vorgang gefunden werden!")
        '    Return False
        'End If
    End Function

    'die veraltet siche muss raus

 
End Class
