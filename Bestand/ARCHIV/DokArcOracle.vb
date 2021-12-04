'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data

Public Class DokArcOracle
    
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
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, OracleConnection)
    End Sub

    Public Function initDokumente4EreignisDatatable(ByVal eid As Integer, ByVal modus As String) As Boolean
        myGlobalz.Arc.ArcRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.Arc.ArcRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "ereignis2dok"
        myGlobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM ereignis2dok " & " where ID=" & eid &
                                             " and dateinameext is not null "
        Return initDokumente4VorgangDatatableExtracted(modus)
    End Function

    Public Function updateDokumentMetadata(ByVal dokumentid As Integer,
                                            ByVal veraltet As Boolean,
                                            ByVal Beschreibung As String,
                                            ByVal revisionssicher As Boolean,
                                            ByVal FILEDATUM As Date,
                                            ByVal exiflat As String,
                                            ByVal exiflong As String) As Integer
        Dim anzahlTreffer& = 0
        Dim com As OracleCommand
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
        Try
            If dokumentid < 1 Then
                nachricht_und_Mbox("FEHLER updateAktuellesDokument : updateid =0. Abbruch")
                Return 0
            End If
            SQLupdate$ = _
             "UPDATE " & myGlobalz.Arc.ArcRec.mydb.Tabelle & " SET " & _
                    " VERALTET=:VERALTET" & _
                    ",BESCHREIBUNG=:BESCHREIBUNG " & _
                    ",REVISIONSSICHER=:REVISIONSSICHER " &
                    ",EXIFLAT=:EXIFLAT " &
                    ",EXIFLONG=:EXIFLONG " &
                    ",INITIAL_=:INITIAL_" &
                    ",FILEDATUM=:FILEDATUM" &
                    " WHERE DOKUMENTID=:DOKUMENTID"
            MeineDBConnection.Open()
            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            com.Parameters.AddWithValue(":VERALTET", Convert.ToBoolean(veraltet)) 'MYGLOBALZ.SITZUNG.AKTDOKUMENT.ISTVERALTET,MYGLOBALZ.SITZUNG.AKTDOKUMENT.BESCHREIBUNG	
            com.Parameters.AddWithValue(":BESCHREIBUNG", Beschreibung)           'MYGLOBALZ.SITZUNG.AKTDOKUMENT.BESCHREIBUNG		
            com.Parameters.AddWithValue(":DOKUMENTID", dokumentid)
            com.Parameters.AddWithValue(":FILEDATUM", FILEDATUM)
            com.Parameters.AddWithValue(":INITIAL_", myGlobalz.sitzung.aktBearbeiter.Initiale)
            com.Parameters.AddWithValue(":EXIFLAT", exiflat)
            com.Parameters.AddWithValue(":EXIFLONG", exiflong)
            com.Parameters.AddWithValue(":REVISIONSSICHER", revisionssicher)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()
            If anzahlTreffer < 1 Then
                '  nachricht_und_Mbox("Problem beim update:" & myGlobalz.Arc.ArcRec.mydb.SQL)
                nachricht("Problem beim update:" & myGlobalz.Arc.ArcRec.mydb.SQL)
                Return 0
            Else
                Return CInt(anzahlTreffer&)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("fehler dok2 Fehler beim update: " & vbCrLf & ex.ToString)
            nachricht("Fehler update   : ", ex)
            Return -2
        End Try
    End Function
    Public Function loescheDokumentInDb(ByVal dokumentid As Integer) As Integer
        Dim anzahlTreffer& = 0
        Dim com As OracleCommand
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""
        nachricht("dokumentenarchiv loescheDokument -------------------------------" & vbCrLf)
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
        Try
            If dokumentid < 1 Then
                nachricht_und_Mbox("FEHLER loescheDokument : updateid =0. Abbruch")
                Return 0
            End If
            SQLupdate = _
             "delete from " & myGlobalz.Arc.ArcRec.mydb.Tabelle &
             " where dokumentid=:dokumentid"
            MeineDBConnection.Open()
            com = New OracleCommand(SQLupdate, MeineDBConnection)
            com.Parameters.AddWithValue(":dokumentid", dokumentid)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()
           '  Return 0 manchmal wird im erfolgsfall keine 1 zurückgeliefert, 
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim löschen:" & SQLupdate)
                Return 0
            Else
                Return CInt(anzahlTreffer&)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("fehler Problem beim löschen: " & vbCrLf & ex.ToString)
            nachricht("Fehler delete: ", ex)
            Return -2
        End Try
    End Function


    Public Function entKoppelung_Dokument_Vorgang(ByVal dokumentID%, ByVal vorgangID As Integer) As Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokument2vorgang"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " where  " & _
             " Vorgangsid=" & vorgangID% & _
             " and dokumentID=" & dokumentID%
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernb:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("fehler Dok3 Fehler beim löschen: " & vbCrLf & ex.ToString)
            nachricht("Fehler Dok3  beim löschen: ", ex)
            Return -2
        End Try
    End Function

    Public Function Koppelung_Dokument_Ereignis(ByVal dokumentID%, ByVal ereignisID As Integer) As Integer
        Dim newid& = -1
        Dim com As OracleCommand
        Try
            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)

            myGlobalz.sitzung.tempREC.mydb = CType(myGlobalz.sitzung.VorgangREC.mydb.Clone, clsDatenbankZugriff)

            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokument2ereignis"


            myGlobalz.sitzung.tempREC.mydb.SQL = _
                 "INSERT INTO " & myGlobalz.sitzung.tempREC.mydb.Tabelle & "   " &
                 " (EREIGNISID,DOKUMENTID) VALUES (:EREIGNISID,:DOKUMENTID) " &
                 " RETURNING ID INTO :R1"

            MeineDBConnection.Open()

            com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
            com.Parameters.AddWithValue(":EREIGNISID", ereignisID)
            com.Parameters.AddWithValue(":DOKUMENTID", dokumentID)
            ' & vorgangID% & _
            '  ",dokumentID=" & dokumentID%
            newid = LIBoracle.clsOracleIns.GetNewid(com, myGlobalz.sitzung.tempREC.mydb.SQL)
            MeineDBConnection.Close()

            If newid < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernc:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("fehler Dok4 Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            nachricht("Fehler Dok4  beim Abspeichern: ", ex)
            Return -2
        End Try
    End Function

    Public Function Koppelung_Dokument_Vorgang(ByVal dokumentID%, ByVal vorgangID As Integer) As Integer
        Dim newid& = -1

        Try
            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokument2vorgang"

            myGlobalz.sitzung.tempREC.mydb.SQL = "INSERT INTO " & myGlobalz.sitzung.tempREC.mydb.Tabelle &
                    " (VORGANGSID,DOKUMENTID) VALUES (:VORGANGSID,:DOKUMENTID) " &
                    " RETURNING ID INTO :R1"
            Dim com As OracleCommand
            MeineDBConnection.Open()

            com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
            com.Parameters.AddWithValue(":VORGANGSID", vorgangID)
            com.Parameters.AddWithValue(":DOKUMENTID", dokumentID)

            newid = LIBoracle.clsOracleIns.GetNewid(com, myGlobalz.sitzung.tempREC.mydb.SQL)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, myGlobalz.sitzung.tempREC.mydb.SQL)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            nachricht("Fehler Dok89  beim Abspeichern: ", ex)
            Return -2
        End Try
    End Function

    Public Function checkin_Dokumente(ByVal aktjpg As clsMyJPG,
                                ByVal relativpfad As String, _
                                ByVal Beschreibung As String,
                                ByVal OriginalFullname As String,
                                ByVal OriginalName As String,
                                    dateidatum As Date) As Integer
        Dim anzahlTreffer& = 0
        Dim com As OracleCommand
        Dim SQLupdate$ = ""
        Dim hinweis As String = ""

        Dim newid& = -1
        Dim fi As New IO.FileInfo(aktjpg.fullname)
        Dim fo As New IO.FileInfo(OriginalFullname$)

        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
        Try
            SQLupdate$ =
         String.Format("INSERT INTO {0} (RELATIVPFAD,DATEINAMEEXT,TYP,BESCHREIBUNG,CHECKINDATUM,FILEDATUM,EXIFDATUM,EXIFLONG,EXIFLAT,EXIFDIR," +
                            "EXIFHERSTELLER,ORIGINALFULLNAME,INITIAL_,REVISIONSSICHER,ORIGINALNAME) " +
                 " VALUES (:RELATIVPFAD,:DATEINAMEEXT,:TYP,:BESCHREIBUNG,:CHECKINDATUM,:FILEDATUM,:EXIFDATUM,:EXIFLONG,:EXIFLAT,:EXIFDIR," +
                           ":EXIFHERSTELLER,:ORIGINALFULLNAME,:INITIAL_,:REVISIONSSICHER,:ORIGINALNAME)",
                                 myGlobalz.Arc.ArcRec.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING DOKUMENTID INTO :R1"

            nachricht("nach setSQLbody : " & SQLupdate)
            MeineDBConnection.Open()

            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            nachricht("vor setParams  ")
            seteFiledatum(fi, dateidatum)
            setSQLParams(com, relativpfad$, fi, Beschreibung$, aktjpg, OriginalFullname$, OriginalName$, fo, False, dateidatum)
            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler Dok1  beim Abspeichern: " & vbCrLf & ex.ToString)
            nachricht("Fehler Dok1  beim Abspeichern: ", ex)
            Return -2
        End Try
    End Function

    Private Shared Sub seteFiledatum(ByVal fi As IO.FileInfo, ByRef dateidatum As Date)
        Try
            If dateidatum = CLstart.mycsimple.MeinNULLDatumAlsDate OrElse
                dateidatum = #12:00:00 AM# Then
                If fi.Extension <> ".doc" And fi.Extension <> ".docx" Then
                    dateidatum = fi.LastWriteTime
                Else
                    dateidatum = DateTime.Now() ' neues Dokument hat immer datum von jetzt
                End If
            End If
        Catch ex As Exception
            nachricht("Fehler beim setzen des Dateidatums vom " & fi.FullName)
        End Try
    End Sub

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
            nachricht("Fehler alleDokuszuVorgangsid_inDatatable  beim Abspeichern: ", ex)
            Return False
        End Try
    End Function

    Public Function gehoertDokumentZumVorgang(ByVal dokumentid As Integer, ByVal vorgangsid As Integer) As Boolean
        Try
            myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokument2vorgang"
            myGlobalz.Arc.ArcRec.mydb.SQL = "select * from " & myGlobalz.Arc.ArcRec.mydb.Tabelle & _
             " where dokumentid=" & dokumentid &
              " and vorgangsid=" & vorgangsid
            nachricht(myGlobalz.Arc.ArcRec.getDataDT())
            If myGlobalz.Arc.ArcRec.dt.IsNothingOrEmpty Then
                Return False
            Else
                Return True
            End If
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("Fehler: gehoertDokumentZumVorgang :gefunden!" & ex.ToString)
            nachricht("Fehler gehoertDokumentZumVorgang   : ", ex)
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
            nachricht("Fehler gehoertDokumentZumVorgang   : ", ex)
            Return False
        End Try
    End Function

    Shared Function setSQLBody() As String
        Return " set " & _
        " RELATIVPFAD=:RELATIVPFAD" & _
        ",DATEINAMEEXT=:DATEINAMEEXT" & _
        ",TYP=:TYP " & _
        ",BESCHREIBUNG=:BESCHREIBUNG " & _
        ",CHECKINDATUM=:CHECKINDATUM " & _
        ",FILEDATUM=:FILEDATUM " & _
        ",EXIFDATUM=:EXIFDATUM " & _
        ",EXIFLONG=:EXIFLONG " & _
        ",EXIFLAT=:EXIFLAT " & _
        ",EXIFDIR=:EXIFDIR " & _
        ",EXIFHERSTELLER=:EXIFHERSTELLER " & _
        ",ORIGINALFULLNAME=:ORIGINALFULLNAME " & _
        ",INITIAL_=:INITIAL_ " &
        ",REVISIONSSICHER=:REVISIONSSICHER " & _
        ",ORIGINALNAME=:ORIGINALNAME "
    End Function

    Shared Sub setSQLParams(ByVal com As OracleCommand,
                            ByVal relativpfad As String,
                            ByVal fi As IO.FileInfo,
                            ByVal Beschreibung As String,
                            ByVal aktjpg As clsMyJPG,
                            ByVal OriginalFullname As String,
                            ByVal OriginalName As String,
                            ByVal fo As IO.FileInfo,
                            ByVal revisionssicher As Boolean,
                            ByVal dateidatum As Date)
        com.Parameters.AddWithValue(":RELATIVPFAD", relativpfad$.Replace("\", "/"))
        com.Parameters.AddWithValue(":DATEINAMEEXT", fi.Name)
        com.Parameters.AddWithValue(":TYP", fi.Extension.Replace(".", ""))
        com.Parameters.AddWithValue(":BESCHREIBUNG", Beschreibung)
        com.Parameters.AddWithValue(":CHECKINDATUM", DateTime.Now())
        com.Parameters.AddWithValue(":FILEDATUM", dateidatum)
        com.Parameters.AddWithValue(":EXIFDATUM", aktjpg.EXIFDateTimeOriginal)
        com.Parameters.AddWithValue(":EXIFLONG", aktjpg.Exifgpslongitude)
        com.Parameters.AddWithValue(":EXIFLAT", aktjpg.Exifgpslatitude)
        com.Parameters.AddWithValue(":EXIFDIR", aktjpg.ExifGpsImgDir)
        com.Parameters.AddWithValue(":EXIFHERSTELLER", aktjpg.EXIFhersteller)
        com.Parameters.AddWithValue(":ORIGINALFULLNAME", OriginalFullname)
        com.Parameters.AddWithValue(":INITIAL_", myGlobalz.sitzung.aktBearbeiter.Initiale)
        com.Parameters.AddWithValue(":REVISIONSSICHER", CInt(revisionssicher))
        com.Parameters.AddWithValue(":ORIGINALNAME", OriginalName)
    End Sub

 

    Private Shared Function viaKopplung_EreignisIDs_DokumentID2(ByVal dokid As Integer) As Boolean
        Dim hinweis As String
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokument2ereignis"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         String.Format("SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " where dokumentid=" & dokid)
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Ereignisse gespeichertr!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

    Private Shared Function initDokumente4VorgangDatatableExtracted(ByVal modus As String) As Boolean
        Try
            If modus = "keinefotos" Then
                If Not String.IsNullOrEmpty(myGlobalz.Arc.ArcRec.mydb.SQL) Then
                    myGlobalz.Arc.ArcRec.mydb.SQL = myGlobalz.Arc.ArcRec.mydb.SQL & " and UPPER(typ) <>'JPG'"
                End If
            End If
            If modus = "beides" Then
            End If
            If modus = "nurfotos" Then
                If Not String.IsNullOrEmpty(myGlobalz.Arc.ArcRec.mydb.SQL) Then
                    myGlobalz.Arc.ArcRec.mydb.SQL = myGlobalz.Arc.ArcRec.mydb.SQL & " and UPPER(typ) ='JPG'"
                End If
            End If
            If Not myGlobalz.Arc.ArcRec.mydb.SQL.ToUpper.Contains("EREIGNIS2DOK") Then
                myGlobalz.Arc.ArcRec.mydb.SQL = myGlobalz.Arc.ArcRec.mydb.SQL & " Order  by filedatum desc"
            End If

            nachricht(myGlobalz.Arc.ArcRec.getDataDT())
            If myGlobalz.Arc.ArcRec.mycount < 1 Then
                nachricht("Keine raumbezugsRec gespeichert!")
            Else
                nachricht(String.Format("initdokumentDT_by_SQLstring: {0} Dokumente vorhanden", myGlobalz.Arc.ArcRec.mycount))
            End If
            Return True
        Catch ex As Exception
            nachricht(String.Format("fehler {0} initDokumente4VorgangDatatableExtracted", ex))
            Return False
        End Try
    End Function

    Public Function initDokumente4VorgangDatatable(ByVal VorgangsID As Integer, ByVal modus As String) As Boolean
        myGlobalz.Arc.ArcRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.Arc.ArcRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.Arc.ArcRec.mydb.Tabelle = "dok2vid"
        myGlobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM dok2vid " & " where VorgangsID=" & VorgangsID &
                                             " and dateinameext is not null "
            'myGlobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM ereignis2dok " & " where VorgangsID=" & VorgangsID &
            '                                 " and dateinameext is not null "
        Return initDokumente4VorgangDatatableExtracted(modus)
    End Function



    ' ''' <summary>
    ' ''' über die kopplung dann union dann dt von sql
    ' ''' </summary>
    ' ''' <param name="ereignisid"></param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>

    Public Shared Function getEreignisID4DokId(ByVal dokid As Integer) As Integer
        Try
            If viaKopplung_EreignisIDs_DokumentID2(dokid) Then
                Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("ereignisid"))
            Else
                nachricht("Es konnten keine Personen zu diesem Vorgang gefunden werden!")
                Return 0
            End If
        Catch ex As Exception
            nachricht("Fehler bei ermittlung der ereignisid für ein dokument: " & dokid)
            Return 0
        End Try
    End Function

    Shared Function changeRevisionssicher(sql As String) As Integer
        Try
            myGlobalz.Arc.ArcRec.mydb.Host = myGlobalz.Arc.ArcRec.mydb.Host
            myGlobalz.Arc.ArcRec.mydb.Schema = myGlobalz.Arc.ArcRec.mydb.Schema
            myGlobalz.Arc.ArcRec.mydb.Tabelle = "dokumente"
            myGlobalz.Arc.ArcRec.mydb.SQL = sql
            Dim newid As Long
            Dim anzahl As Long = myGlobalz.Arc.ArcRec.sqlexecute(newid)
            If anzahl < 1 Then
                nachricht("Keine changeRevisionssicher gespeichert!")
            Else
                nachricht(String.Format("changeRevisionssicher: {0} geändert", anzahl))
            End If
            Return CInt(anzahl)
        Catch ex As Exception
            nachricht("Fehler bei changeRevisionssicher: " & sql)
            Return 0
        End Try
    End Function

End Class
