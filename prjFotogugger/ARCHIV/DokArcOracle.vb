'Imports Devart.Data.Oracle
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

Public Class DokArcOracle

    'Implements IDisposable

    'Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
    'Protected Overridable Sub Dispose(disposing As Boolean)
    '    If Not Me.disposedValue Then
    '        If disposing Then
    '            MeineDBConnection.Dispose()
    '        End If
    '    End If
    '    Me.disposedValue = True
    'End Sub
    'Public Sub Dispose() Implements IDisposable.Dispose
    '    Dispose(True)
    '    GC.SuppressFinalize(Me)
    'End Sub

    'Public MeineDBConnection As New OracleConnection
    'Sub New(ByVal conn As System.Data.Common.DbConnection)
    '    MeineDBConnection = CType(conn, OracleConnection)
    'End Sub

    'Public Function initDokumente4EreignisDatatable(ByVal eid As Integer, ByVal modus As String) As Boolean
    '    myglobalz.Arc.ArcRec.mydb.Host = myglobalz.sitzung.VorgangREC.mydb.Host
    '    myglobalz.Arc.ArcRec.mydb.Schema = myglobalz.sitzung.VorgangREC.mydb.Schema
    '    myglobalz.Arc.ArcRec.mydb.Tabelle ="ereignis2dok2"
    '    myGlobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM ereignis2dok2 " & " where ID=" & eid &
    '                                         " and dateinameext is not null order by filedatum desc"
    '    bildesqlStringFuerdokumente(modus)
    '    myGlobalz.Arc.ArcRec.dt = getDT4Query(myGlobalz.Arc.ArcRec.mydb.SQL, myGlobalz.Arc.ArcRec)
    '    If myGlobalz.Arc.ArcRec.dt.Rows.Count > 0 Then
    '        Return True
    '    Else
    '        Return False
    '    End If
    '    'Return 'initDokumente4VorgangDatatableExtracted(modus)
    'End Function

    'Public Function updateDokumentMetadata(ByVal dokumentid As Integer,
    '                                        ByVal veraltet As Boolean,
    '                                        ByVal Beschreibung As String,
    '                                        ByVal revisionssicher As Boolean,
    '                                        ByVal FILEDATUM As Date,
    '                                        ByVal exiflat As String,
    '                                        ByVal exiflong As String) As Integer
    '    Dim anzahlTreffer& = 0
    '    Dim com As OracleCommand
    '    Dim SQLupdate As String = ""
    '    Dim hinweis As String = ""
    '    Dim aaa As New Text.StringBuilder

    '    Try
    '        If dokumentid < 1 Then
    '            nachricht("FEHLER updateAktuellesDokument : updateid =0. Abbruch")
    '            Return 0
    '        End If
    '        SQLupdate = "UPDATE " & CLstart.myViewsNTabs.tabdokumente & "  SET " &
    '                " VERALTET=:VERALTET" &
    '                ",BESCHREIBUNG=:BESCHREIBUNG " &
    '                ",REVISIONSSICHER=:REVISIONSSICHER " &
    '                ",EXIFLAT=:EXIFLAT " &
    '                ",EXIFLONG=:EXIFLONG " &
    '                ",INITIAL_=:INITIAL_" &
    '                ",FILEDATUM=:FILEDATUM" &
    '                " WHERE DOKUMENTID=:DOKUMENTID"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(SQLupdate, MeineDBConnection)
    '        com.Parameters.AddWithValue(":VERALTET", Convert.ToBoolean(veraltet)) 'MYGLOBALZ.SITZUNG.AKTDOKUMENT.ISTVERALTET,MYGLOBALZ.SITZUNG.AKTDOKUMENT.BESCHREIBUNG	
    '        com.Parameters.AddWithValue(":BESCHREIBUNG", Beschreibung)           'MYGLOBALZ.SITZUNG.AKTDOKUMENT.BESCHREIBUNG		
    '        com.Parameters.AddWithValue(":DOKUMENTID", dokumentid)
    '        com.Parameters.AddWithValue(":FILEDATUM", FILEDATUM)
    '        com.Parameters.AddWithValue(":INITIAL_", myglobalz.sitzung.aktBearbeiter.Initiale)
    '        com.Parameters.AddWithValue(":EXIFLAT", exiflat)
    '        com.Parameters.AddWithValue(":EXIFLONG", exiflong)
    '        com.Parameters.AddWithValue(":REVISIONSSICHER", revisionssicher)


    '        aaa.Append(" Convert.ToBoolean(veraltet): " & Convert.ToBoolean(veraltet))
    '        aaa.Append(" Beschreibung: " & Beschreibung)
    '        aaa.Append(" DOKUMENTID: " & dokumentid)
    '        aaa.Append(" FILEDATUM: " & FILEDATUM)
    '        aaa.Append(" myGlobalz.sitzung.aktBearbeiter.Initiale: " & myglobalz.sitzung.aktBearbeiter.Initiale)
    '        aaa.Append(" exiflat: " & exiflat)
    '        aaa.Append(" exiflong: " & exiflong)
    '        aaa.Append(" revisionssicher: " & revisionssicher)


    '        anzahlTreffer& = CInt(com.ExecuteNonQuery)
    '        MeineDBConnection.Close()
    '        If anzahlTreffer < 1 Then
    '            nachricht("fehler Problem beim update1:" & SQLupdate & aaa.ToString)
    '            Return 0
    '        Else
    '            Return CInt(anzahlTreffer&)
    '        End If



    '    Catch ex As Exception
    '        nachricht("fehler dok2 Fehler beim update: " & aaa.ToString & vbCrLf ,ex)
    '        nachricht("Fehler update   : ", ex)
    '        Return -2
    '    End Try
    'End Function



    'Public Function Koppelung_Dokument_Ereignis(ByVal dokumentID%, ByVal ereignisID As Integer) As Integer
    '    Dim newid& = -1
    '    '  Dim com As OracleCommand
    '    Try
    '        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecOracle)
    '        myGlobalz.sitzung.tempREC.mydb = CType(myGlobalz.sitzung.VorgangREC.mydb.Clone, clsDatenbankZugriff)
    '        myGlobalz.sitzung.tempREC.mydb.Host = myglobalz.sitzung.VorgangREC.mydb.Host
    '        myglobalz.sitzung.tempREC.mydb.Schema = myglobalz.sitzung.VorgangREC.mydb.Schema
    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="dokumente" 'ihah dokument_2ereignis
    '        myGlobalz.sitzung.tempREC.mydb.SQL = "update " & CLstart.myViewsNTabs.tabdokumente & "  set eid=" & ereignisID & " where dokumentid=" & dokumentID
    '        myglobalz.sitzung.tempREC.sqlexecute(newid)
    '        If newid < 1 Then
    '            nachricht("  Koppelung_Dokument_Ereignis gespeichert!")
    '        Else
    '            nachricht(String.Format("Koppelung_Dokument_Ereignis: {0} geändert", newid))
    '        End If
    '        Return 1
    '    Catch ex As Exception
    '        nachricht("fehler Dok4 Fehler beim Abspeichern: " & vbCrLf ,ex)
    '        nachricht("Fehler Dok4  beim Abspeichern: ", ex)
    '        Return -2
    '    End Try
    'End Function

    'Public Function Koppelung_Dokument_Ereignis(ByVal dokumentID%, ByVal ereignisID As Integer) As Integer
    '    Dim newid& = -1
    '    Dim com As OracleCommand
    '    Try
    '        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecOracle)

    '        myGlobalz.sitzung.tempREC.mydb = CType(myGlobalz.sitzung.VorgangREC.mydb.Clone, clsDatenbankZugriff)

    '        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="dokument2ereignis"


    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
    '             "INSERT INTO " & myGlobalz.sitzung.tempREC.mydb.Tabelle & "   " &
    '             " (EREIGNISID,DOKUMENTID) VALUES (:EREIGNISID,:DOKUMENTID) " &
    '             " RETURNING ID INTO :R1"

    '        MeineDBConnection.Open()

    '        com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
    '        com.Parameters.AddWithValue(":EREIGNISID", ereignisID)
    '        com.Parameters.AddWithValue(":DOKUMENTID", dokumentID)
    '        ' & vorgangID% & _
    '        '  ",dokumentID=" & dokumentID%
    '        newid = clsOracleIns.GetNewid(com, myGlobalz.sitzung.tempREC.mydb.SQL)
    '        MeineDBConnection.Close()

    '        If newid < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichernc:" & myGlobalz.sitzung.tempREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(newid)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("fehler Dok4 Fehler beim Abspeichern: " & vbCrLf ,ex)
    '        nachricht("Fehler Dok4  beim Abspeichern: ", ex)
    '        Return -2
    '    End Try
    'End Function

    'Public Function Koppelung_Dokument_Vorgang(ByVal dokumentID%, ByVal vorgangID As Integer) As Integer
    '    Dim newid& = -1

    '    Try
    '        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecOracle)
    '        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="dokument2vorgang"

    '        myGlobalz.sitzung.tempREC.mydb.SQL = "INSERT INTO " & myGlobalz.sitzung.tempREC.mydb.Tabelle &
    '                " (VORGANGSID,DOKUMENTID) VALUES (:VORGANGSID,:DOKUMENTID) " &
    '                " RETURNING ID INTO :R1"
    '        Dim com As OracleCommand
    '        MeineDBConnection.Open()

    '        com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
    '        com.Parameters.AddWithValue(":VORGANGSID", vorgangID)
    '        com.Parameters.AddWithValue(":DOKUMENTID", dokumentID)

    '        newid = clsOracleIns.GetNewid(com, myGlobalz.sitzung.tempREC.mydb.SQL)
    '        MeineDBConnection.Close()
    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, myGlobalz.sitzung.tempREC.mydb.SQL)
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler beim Abspeichern: " & vbCrLf ,ex)
    '        nachricht("Fehler Dok89  beim Abspeichern: ", ex)
    '        Return -2
    '    End Try
    'End Function

    'Public Function checkin_Dokumente(ByVal aktjpg As clsMyJPG,
    '                                    ByVal relativpfad As String,
    '                                    ByVal Beschreibung As String,
    '                                    ByVal OriginalFullname As String,
    '                                    ByVal OriginalName As String,
    '                                    ByVal dateidatum As Date,
    '                                    ByVal VID As Integer, EID As Integer,
    '                                    ByVal NEWSAVEMODE As Boolean) As Integer
    '    Dim anzahlTreffer& = 0
    '    Dim com As OracleCommand
    '    Dim SQLupdate$ = ""
    '    Dim hinweis As String = ""

    '    Dim newid& = -1
    '    Dim fi As New IO.FileInfo(aktjpg.fullname)
    '    Dim fo As New IO.FileInfo(OriginalFullname$)

    '    myglobalz.Arc.ArcRec.mydb.Tabelle ="dokumente"
    '    Try
    '        SQLupdate$ =
    '     String.Format("INSERT INTO {0} (RELATIVPFAD,DATEINAMEEXT,TYP,BESCHREIBUNG,CHECKINDATUM,FILEDATUM,EXIFDATUM,EXIFLONG,EXIFLAT,EXIFDIR," +
    '                        "EXIFHERSTELLER,ORIGINALFULLNAME,INITIAL_,REVISIONSSICHER,ORIGINALNAME,VID,EID,NEWSAVEMODE) " +
    '             " VALUES (:RELATIVPFAD,:DATEINAMEEXT,:TYP,:BESCHREIBUNG,:CHECKINDATUM,:FILEDATUM,:EXIFDATUM,:EXIFLONG,:EXIFLAT,:EXIFDIR," +
    '                       ":EXIFHERSTELLER,:ORIGINALFULLNAME,:INITIAL_,:REVISIONSSICHER,:ORIGINALNAME,:VID,:EID,:NEWSAVEMODE)",
    '                             myglobalz.Arc.ArcRec.mydb.Tabelle)
    '        SQLupdate$ = SQLupdate$ & " RETURNING DOKUMENTID INTO :R1"

    '        nachricht("nach setSQLbody : " & SQLupdate)
    '        MeineDBConnection.Open()

    '        com = New OracleCommand(SQLupdate, MeineDBConnection)
    '        nachricht("vor setParams  ")
    '        seteFiledatum(fi, dateidatum)
    '        setSQLParams(com, relativpfad, fi, Beschreibung, aktjpg, OriginalFullname, OriginalName, fo, False, dateidatum, VID, EID, NEWSAVEMODE)
    '        newid = clsOracleIns.GetNewid(com, SQLupdate)
    '        MeineDBConnection.Close()
    '        fi = Nothing
    '        fo = Nothing
    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)
    '    Catch ex As Exception
    '        nachricht("Fehler Dok1  beim Abspeichern: " & vbCrLf ,ex)
    '        nachricht("Fehler Dok1  beim Abspeichern: ", ex)
    '        Return -2
    '    End Try
    'End Function

    Shared Sub seteFiledatum(ByVal fi As IO.FileInfo, ByRef dateidatum As Date)
        Try
            If dateidatum = CLstart.mycSimple.MeinNULLDatumAlsDate OrElse
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

    'Public Function alleDokuszuVorgangsid_inDatatable() As Boolean
    '    Try
    '        myglobalz.Arc.ArcRec.mydb.Tabelle ="dokumente"
    '        myglobalz.Arc.ArcRec.mydb.SQL = "select * from " & myglobalz.Arc.ArcRec.mydb.Tabelle &
    '         " where vorgangsid=" & myglobalz.sitzung.aktVorgangsID
    '        nachricht(myglobalz.Arc.ArcRec.getDataDT())
    '        If myglobalz.Arc.ArcRec.dt.IsNothingOrEmpty Then
    '            nachricht_und_Mbox(String.Format("Keine Dokumente zur VorgangsID:{0} gefunden!", myglobalz.sitzung.aktVorgangsID))
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht("Fehler alleDokuszuVorgangsid_inDatatable  beim Abspeichern: ", ex)
    '        Return False
    '    End Try
    'End Function

    'Public Function gehoertDokumentZumVorgang(ByVal dokumentid As Integer, ByVal vorgangsid As Integer) As Boolean
    '    Try
    '        myGlobalz.Arc.ArcRec.mydb.Tabelle ="dokument2vorgang"
    '        myGlobalz.Arc.ArcRec.mydb.SQL = "select * from " & myGlobalz.Arc.ArcRec.mydb.Tabelle & _
    '         " where dokumentid=" & dokumentid &
    '          " and vorgangsid=" & vorgangsid
    '        nachricht(myGlobalz.Arc.ArcRec.getDataDT())
    '        If myGlobalz.Arc.ArcRec.dt.IsNothingOrEmpty Then
    '            Return False
    '        Else
    '            Return True
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler: gehoertDokumentZumVorgang :gefunden!" ,ex)
    '        nachricht("Fehler gehoertDokumentZumVorgang   : ", ex)
    '        Return False
    '    End Try
    'End Function
    'Public Function alleDokuszuEreignis_inDatatable() As Boolean
    '    Try
    '        myglobalz.Arc.ArcRec.mydb.Tabelle ="dokumente"
    '        myglobalz.Arc.ArcRec.mydb.SQL = "select * from " & myglobalz.Arc.ArcRec.mydb.Tabelle &
    '         " where ereignisid=" & myglobalz.sitzung.aktVorgangsID
    '        Dim hinweis As String = myglobalz.Arc.ArcRec.getDataDT()
    '        If myglobalz.Arc.ArcRec.dt.IsNothingOrEmpty Then
    '            nachricht_und_Mbox(String.Format("Keine Dokumente zur VorgangsID:{0} gefunden!", myglobalz.sitzung.aktVorgangsID))
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht("Fehler alleDokuszuEreignis_inDatatable   : ", ex)
    '        Return False
    '    End Try
    'End Function

    'Shared Function setSQLBody() As String
    '    Return " set " &
    '    " RELATIVPFAD=:RELATIVPFAD" &
    '    ",DATEINAMEEXT=:DATEINAMEEXT" &
    '    ",TYP=:TYP " &
    '    ",BESCHREIBUNG=:BESCHREIBUNG " &
    '    ",CHECKINDATUM=:CHECKINDATUM " &
    '    ",FILEDATUM=:FILEDATUM " &
    '    ",EXIFDATUM=:EXIFDATUM " &
    '    ",EXIFLONG=:EXIFLONG " &
    '    ",EXIFLAT=:EXIFLAT " &
    '    ",EXIFDIR=:EXIFDIR " &
    '    ",EXIFHERSTELLER=:EXIFHERSTELLER " &
    '    ",ORIGINALFULLNAME=:ORIGINALFULLNAME " &
    '    ",INITIAL_=:INITIAL_ " &
    '    ",REVISIONSSICHER=:REVISIONSSICHER " &
    '    ",ORIGINALNAME=:ORIGINALNAME " &
    '    ",VID=:VID " &
    '    ",EID=:EID "
    '    'VORGANGSID,EREIGNISID
    'End Function

    Shared Function GetExtension(ByVal fi As IO.FileInfo) As String
        Dim extension As String
        Try
            extension = fi.Extension.Replace(".", "")
            If extension.IsNothingOrEmpty Then extension = "txt"
            Return extension
        Catch ex As Exception
            Return "err"
        End Try
    End Function
    'Shared Sub setSQLParams(ByVal com As OracleCommand,
    '                            ByVal relativpfad As String,
    '                        ByVal fi As IO.FileInfo,
    '                        ByVal Beschreibung As String,
    '                        ByVal aktjpg As clsMyJPG,
    '                        ByVal OriginalFullname As String,
    '                        ByVal OriginalName As String,
    '                        ByVal fo As IO.FileInfo,
    '                        ByVal revisionssicher As Boolean,
    '                        ByVal dateidatum As Date,
    '                                VID As Integer, EID As Integer,
    '                                NEWSAVEMODE As Boolean)
    '    Dim extension As String
    '    extension = GetExtension(fi)
    '    com.Parameters.AddWithValue(":RELATIVPFAD", relativpfad$.Replace("\", "/"))
    '    com.Parameters.AddWithValue(":DATEINAMEEXT", fi.Name)
    '    com.Parameters.AddWithValue(":TYP", extension)
    '    com.Parameters.AddWithValue(":BESCHREIBUNG", Beschreibung)
    '    com.Parameters.AddWithValue(":CHECKINDATUM", DateTime.Now())
    '    com.Parameters.AddWithValue(":FILEDATUM", dateidatum)
    '    com.Parameters.AddWithValue(":EXIFDATUM", aktjpg.EXIFDateTimeOriginal)
    '    com.Parameters.AddWithValue(":EXIFLONG", aktjpg.Exifgpslongitude)
    '    com.Parameters.AddWithValue(":EXIFLAT", aktjpg.Exifgpslatitude)
    '    com.Parameters.AddWithValue(":EXIFDIR", aktjpg.ExifGpsImgDir)
    '    com.Parameters.AddWithValue(":EXIFHERSTELLER", aktjpg.EXIFhersteller)
    '    com.Parameters.AddWithValue(":ORIGINALFULLNAME", OriginalFullname)
    '    com.Parameters.AddWithValue(":INITIAL_", myglobalz.sitzung.aktBearbeiter.Initiale)
    '    com.Parameters.AddWithValue(":REVISIONSSICHER", CInt(revisionssicher))
    '    com.Parameters.AddWithValue(":NEWSAVEMODE", CInt(NEWSAVEMODE))
    '    com.Parameters.AddWithValue(":ORIGINALNAME", OriginalName)
    '    com.Parameters.AddWithValue(":VID", VID)
    '    com.Parameters.AddWithValue(":EID", EID)
    'End Sub



    'Private Shared Function viaKopplung_EreignisIDs_DokumentID2(ByVal dokid As Integer) As Boolean
    '    Dim hinweis As String
    '    myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '    myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="dokument2ereignis"
    '    myGlobalz.sitzung.tempREC.mydb.SQL = _
    '     String.Format("SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
    '     " where dokumentid=" & dokid)
    '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
    '    If myGlobalz.sitzung.tempREC.mycount < 1 Then
    '        nachricht("Keine Ereignisse gespeichertr!")
    '        Return False
    '    Else
    '        nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
    '        Return True
    '    End If
    'End Function

    'Private Shared Function initDokumente4VorgangDatatableExtracted(ByVal modus As String) As Boolean
    '    Try
    '        bildesqlStringFuerdokumente(modus)

    '        nachricht(myGlobalz.Arc.ArcRec.getDataDT())
    '        If myGlobalz.Arc.ArcRec.mycount < 1 Then
    '            nachricht("Keine raumbezugsRec gespeichert!")
    '        Else
    '            nachricht(String.Format("initdokumentDT_by_SQLstring: {0} Dokumente vorhanden", myGlobalz.Arc.ArcRec.mycount))
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht(String.Format("fehler {0} initDokumente4VorgangDatatableExtracted", ex))
    '        Return False
    '    End Try
    'End Function

    Shared Function bildesqlStringFuerdokumente(modus As String, sql As String) As String
        If modus = "keinefotos" Then
            If Not String.IsNullOrEmpty(sql) Then
                sql = sql &
                    " and ((UPPER(typ) <>'JPG') and (UPPER(typ) <>'JPEG') and (UPPER(typ) <>'PNG') and (UPPER(typ) <>'GIF'))"
            End If
        End If
        If modus = "beides" Then
        End If
        If modus = "nurfotos" Then
            If Not String.IsNullOrEmpty(sql) Then
                sql = sql &
                    " and ((UPPER(typ) ='JPG') or (UPPER(typ) ='JPEG') or (UPPER(typ) ='PNG')  or (UPPER(typ) ='GIF'))"
                sql = sql & " order by filedatum desc, exifdatum desc "

            End If
        Else
            If Not sql.ToUpper.Contains("EREIGNIS2DOK2") Then
                If Not sql.ToLower.Contains(" order ") Then
                    sql = sql & " Order  by filedatum desc"
                End If
                'ihah
            End If
        End If
        Return sql
    End Function

    Public Shared Function initDokumente4EreignisDatatable(ByVal ereignisid As Integer, ByVal modus As String) As Boolean
        myGlobalz.Arc.ArcRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.Arc.ArcRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        ' myGlobalz.Arc.ArcRec.mydb.Tabelle ="dok2vid"
        'myGlobalz.Arc.ArcRec.mydb.Tabelle ="dokumente"
        myGlobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabDokumente & "  " &
                                        " where EID=" & ereignisid &
                                        " and dateinameext is not null "
        myglobalz.Arc.ArcRec.mydb.SQL = bildesqlStringFuerdokumente(modus, (myglobalz.Arc.ArcRec.mydb.SQL)) : Dim hinweis As String = ""
        myGlobalz.Arc.ArcRec.dt = getDT4Query(myGlobalz.Arc.ArcRec.mydb.SQL, myGlobalz.Arc.ArcRec, hinweis)
        If myGlobalz.Arc.ArcRec.dt IsNot Nothing Then
            If myGlobalz.Arc.ArcRec.dt.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
        'Return initDokumente4VorgangDatatableExtracted(modus)
    End Function

    Public Shared Function initDokumente4VorgangDatatable(ByVal strVorgangsID As String,
                                                          ByVal modus As String,
                                                          allebilder As Boolean,
                                                          eid As Integer) As Boolean
        myglobalz.Arc.ArcRec.mydb.Host = myglobalz.sitzung.VorgangREC.mydb.Host
        myglobalz.Arc.ArcRec.mydb.Schema = myglobalz.sitzung.VorgangREC.mydb.Schema
        If allebilder Then
            myglobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabDokumente & " " &
                                  " where VID in (" & strVorgangsID & ")" &
                                  " and dateinameext is not null "
        Else
            myglobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabDokumente & " " &
                                  " where VID in (" & strVorgangsID & ")  and eid=" & eid & " " &
                                  " and dateinameext is not null "
        End If

        myglobalz.Arc.ArcRec.mydb.SQL = bildesqlStringFuerdokumente(modus, myglobalz.Arc.ArcRec.mydb.SQL) : Dim hinweis As String = ""
        myglobalz.Arc.ArcRec.dt = getDT4Query(myglobalz.Arc.ArcRec.mydb.SQL, myglobalz.Arc.ArcRec, hinweis)
        If myglobalz.Arc.ArcRec.dt IsNot Nothing Then
            If myglobalz.Arc.ArcRec.dt.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If
        'Return initDokumente4VorgangDatatableExtracted(modus)
    End Function

    'Public Shared Function getEreignisID4DokId(ByVal dokid As Integer) As Integer
    '    Try
    '        If viaKopplung_EreignisIDs_DokumentID2(dokid) Then
    '            Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("ereignisid"))
    '        Else
    '            nachricht("Es konnten keine Personen zu diesem Vorgang gefunden werden!")
    '            Return 0
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler bei ermittlung der ereignisid für ein dokument: " & dokid)
    '        Return 0
    '    End Try
    'End Function

    Shared Function changeRevisionssicher(sql As String) As Integer
        Try
            myglobalz.Arc.ArcRec.mydb.Host = myglobalz.Arc.ArcRec.mydb.Host
            myglobalz.Arc.ArcRec.mydb.Schema = myglobalz.Arc.ArcRec.mydb.Schema
            'myglobalz.Arc.ArcRec.mydb.Tabelle ="dokumente"
            myGlobalz.Arc.ArcRec.mydb.SQL = sql
            Dim newid As Long
            Dim anzahl As Long = myglobalz.Arc.ArcRec.sqlexecute(newid)
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
