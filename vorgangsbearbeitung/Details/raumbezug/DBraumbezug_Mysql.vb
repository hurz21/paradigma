Imports MySql.Data.MySqlClient
Imports System.Data

Public Class DBraumbezug_Mysql
    
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
    Shared Function setFOTOSQLbody() As String
        Return " set " & _
         " GKrechts=@GKrechts" & _
         ",GKhoch=@GKhoch" & _
         ",GPSlongitude=@GPSlongitude" & _
         ",GPSlatitude=@GPSlatitude" & _
         ",GPSdir=@GPSdir" & _
         ",UTMrechts=@UTMrechts" & _
         ",UTMhoch=@UTMhoch" & _
         ",EXIFDatumOriginal=@EXIFDatumOriginal" & _
         ",DokumentID=@DokumentID"
    End Function

    Shared Function setFOTOSQLParams(ByVal com As MySqlCommand, ByVal aktJPG As clsMyJPG) As Boolean
        com.Parameters.AddWithValue("@GKrechts", aktJPG.rechts.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@GKhoch", aktJPG.hoch.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@GPSlongitude", aktJPG.Exifgpslongitude)
        com.Parameters.AddWithValue("@GPSlatitude", aktJPG.Exifgpslatitude)
        com.Parameters.AddWithValue("@GPSdir", aktJPG.ExifGpsImgDir)
        com.Parameters.AddWithValue("@UTMrechts", "0".ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@UTMhoch", "0".ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@EXIFDatumOriginal", Convert.ToDateTime(Format(aktJPG.EXIFDateTimeOriginal, "yyyy-MM-dd HH:mm:ss")))
        com.Parameters.AddWithValue("@DokumentID", aktJPG.DokumentID)
    End Function

    Public Function RB_FOTO_abspeichern_Neu(ByVal aktJPG As clsMyJPG) As Integer
        'Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        'Dim com As MySqlCommand
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaFoto"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("insert into {0}{1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, setFOTOSQLbody())

        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    setFOTOSQLParams(com, aktJPG)
        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '    com.CommandText = "Select LAST_INSERT_ID()"
        '    newid = CLng(com.ExecuteScalar)
        '    myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)

        '    '	anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Fehler RB_FOTO_abspeichern_Neu Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(newid)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler RB_FOTO_abspeichern_Neu Problem beim Abspeichern: " & ex.ToString)
        '    Return -2
        'End Try
    End Function

    Public Function getID4Foto(ByVal dokumentID As Integer) As Integer
        'nachricht("getID4Foto ---------------------------")
        '' Dim hinweis as string = ""

        'glob2.initTemprecAusVorgangRecMysql()
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaFoto"
        'myGlobalz.sitzung.tempREC.mydb.SQL = _
        ' String.Format("select id from {0} where dokumentid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, dokumentID%)
        'nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        'If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
        '    Return 0
        'End If
        'Return CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0)))
    End Function

    Public Function RB_FOTO_loeschen(ByVal dokumentID As Integer) As Integer
        'Dim anzahlTreffer& = 0, hinweis$ = ""
        'Dim com As MySqlCommand
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaFoto"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("delete from {0} where dokumentid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, dokumentID%)
        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    'setFOTOSQLParams(com, aktJPG)
        '    com.Parameters.AddWithValue("@DokumentID", dokumentID)
        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '    myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)
        '    '	anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Fehler RB_FOTO_loeschen  :" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return 0
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler RB_FOTO_loeschen: " & ex.ToString)
        '    Return -1
        'End Try
    End Function
    Shared Function setSQLParamsRB() As String
        Return " set " & _
         " typ=@typ" & _
         ",Sekid=@Sekid" & _
         ",Name=@Name" & _
         ",Abstract=@Abstract" & _
         ",rechts=@rechts" & _
         ",hoch=@hoch" & _
         ",xmin=@xmin" & _
         ",xmax=@xmax" & _
         ",ymin=@ymin" & _
         ",ymax=@ymax"
    End Function
    Shared Sub setSQLParamsRB(ByVal com As MySqlCommand, ByVal aktrb As iRaumbezug, ByVal rid%)
        com.Parameters.AddWithValue("@typ", myGlobalz.sitzung.akt_raumbezugsTyp)
        com.Parameters.AddWithValue("@Sekid", aktrb.SekID)
        com.Parameters.AddWithValue("@Name", aktrb.name)
        com.Parameters.AddWithValue("@Abstract", aktrb.abstract)
        com.Parameters.AddWithValue("@rechts", aktrb.punkt.X.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@hoch", aktrb.punkt.Y.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@xmin", aktrb.box.xl.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@xmax", aktrb.box.xh.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@ymin", aktrb.box.yl.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@ymax", aktrb.box.yh.ToString.Replace(",", "."))
        com.Parameters.AddWithValue("@RaumbezugsID", rid%)
    End Sub
    Public Function Raumbezug_abspeichern_Edit(ByVal raumbezugsID%, ByVal aktrb As iRaumbezug) As Integer
        'Dim anzahlTreffer& = 0, hinweis$ = ""
        'Dim com As MySqlCommand
        ''Typ wird nicht verändert
        'Try
        '    If raumbezugsID < 1 Then
        '        nachricht_und_Mbox("Fehler: Raumbezug_abspeichern_Edit Updateid<1. abbruch Raumbezug_abspeichern_Edit")
        '        Return 0
        '    End If
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("update   {0}{1} where RaumbezugsID=@RaumbezugsID", myGlobalz.sitzung.tempREC.mydb.Tabelle, setSQLParamsRB())
        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    setSQLParamsRB(com, aktrb, raumbezugsID)

        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
        '    Return -2
        'End Try
    End Function

    Public Function Raumbezug_abspeichern_Neu(ByVal aktrb As iRaumbezug) As Integer
        'Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        'Dim com As MySqlCommand
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("insert into {0}{1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, setSQLParamsRB())
        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    setSQLParamsRB(com, aktrb, 0)
        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '    com.CommandText = "Select LAST_INSERT_ID()"
        '    newid = CLng(com.ExecuteScalar)
        '    myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)

        '    'anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(newid)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
        '    Return -2
        'End Try
    End Function

    Shared Function setsqlbodyAdresseRB() As String
        Return " set " & _
         " Gemeindenr=@Gemeindenr" & _
         ",Gemeindetext=@Gemeindetext" & _
         ",Strassenname=@Strassenname" & _
         ",strcode=@strcode" & _
         ",fs=@fs" & _
         ",hausnrkombi=@hausnrkombi" & _
         ",PLZ=@PLZ" & _
         ",Postfach=@Postfach" & _
         ",adresstyp=@adresstyp"
    End Function
    Shared Sub setSQLparamsAdresseRB(ByVal com As MySqlCommand, ByVal sekid%)
        '	com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.myconn)
        com.Parameters.AddWithValue("@Gemeindenr", myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        com.Parameters.AddWithValue("@Gemeindetext", myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName)
        com.Parameters.AddWithValue("@Strassenname", myGlobalz.sitzung.aktADR.Gisadresse.strasseName)
        com.Parameters.AddWithValue("@strcode", myGlobalz.sitzung.aktADR.Gisadresse.strasseCode)
        com.Parameters.AddWithValue("@fs", myGlobalz.sitzung.aktADR.FS)
        com.Parameters.AddWithValue("@hausnrkombi", myGlobalz.sitzung.aktADR.Gisadresse.HausKombi)
        com.Parameters.AddWithValue("@PLZ", CInt(myGlobalz.sitzung.aktADR.PLZ))
        com.Parameters.AddWithValue("@Postfach", myGlobalz.sitzung.aktADR.Postfach)
        com.Parameters.AddWithValue("@adresstyp", CInt(myGlobalz.sitzung.aktADR.Adresstyp))
        com.Parameters.AddWithValue("@ID", sekid)
    End Sub

    Public Function RB_Adresse_abspeichern_Edit(ByVal sekid As Integer) As Integer 'myGlobalz.sitzung.aktADR.SekID()
        'Dim anzahlTreffer& = 0, hinweis$ = ""
        'Dim com As MySqlCommand
        'Try
        '    If sekid < 1 Then
        '        nachricht_und_Mbox("fehler updateid<1)")
        '        Return 0
        '    End If
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaAdresse"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("update {0}{1} where id=@ID", myGlobalz.sitzung.tempREC.mydb.Tabelle, setsqlbodyAdresseRB())
        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    setSQLparamsAdresseRB(com, sekid)
        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
        '    Return -2
        'End Try
    End Function

    Public Function RB_Adresse_abspeichern_Neu() As Integer
        'Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        'Dim com As MySqlCommand
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaAdresse"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("insert into {0}{1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, setsqlbodyAdresseRB())

        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    setSQLparamsAdresseRB(com, 0)

        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '    com.CommandText = "Select LAST_INSERT_ID()"
        '    newid = CLng(com.ExecuteScalar)
        '    myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)

        '    'anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(newid)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
        '    Return -2
        'End Try
    End Function


    Public Function RB_loeschenBySekIDTyp(ByVal Sekid%, ByVal doktyp2 As String) As Integer
        'nachricht("RB_loeschenBySekIDTyp-------------------------------------------")
        'Dim anzahlTreffer&, newid&, doktyp%
        'Try
        '    doktyp = CType(doktyp2, Integer)
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("delete from {0} where sekid={1} and typ={2}", myGlobalz.sitzung.tempREC.mydb.Tabelle, Sekid, doktyp)
        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
        '    Return -2
        'End Try
    End Function
    Public Function RB_loeschenByID(ByVal raumbezugsid As Integer) As Integer
        'Dim anzahlTreffer&, newid&
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("delete from {0} where raumbezugsid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, raumbezugsid)
        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
        '    Return -2
        'End Try
    End Function
    Public Function RB_Adresse_loeschen(ByVal adressid As Integer) As Integer
        'Dim anzahlTreffer&, newid&
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaAdresse"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("delete from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, adressid)
        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
        '    Return -2
        'End Try
    End Function




    Public Function getCoords4ID_Raumbezug(ByVal id As Integer) As myPoint
        'Dim pt As New myPoint
        'glob2.initTemprecAusVorgangRecMysql()
        'myGlobalz.sitzung.tempREC.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
        'myGlobalz.sitzung.tempREC.mydb.SQL = _
        '                    String.Format("select rechts,hoch from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, id)
        'nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        'If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
        '    pt.X = 0
        '    pt.Y = 0
        'Else
        '    pt.X = CType(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0)), Double)
        '    pt.Y = CType(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(1)), Double)
        'End If
        'Return pt
    End Function

    Public Function RB_Flurstueck_holen(ByVal sekid As String) As Boolean
        'Dim hinweis As String
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="Paraflurstueck"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '                     String.Format("select * from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid$)
        '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        '    Return True
        'Catch ex As Exception
        '    nachricht_und_Mbox("fehler beim holen : " & ex.ToString)
        '    Return False
        'End Try
    End Function

    Public Function RB_Adresse_holen_by_ID(ByVal sekid As String) As Boolean
        'Dim hinweis As String
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="ParaAdresse"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("select * from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid$)
        '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        '    Return True
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
        '    Return False
        'End Try
    End Function

    Private Shared Function viaKopplung_VorgangID_zu_RaumbezuegeID(ByVal vorgangsid As String) As Boolean

        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.raumbezugsRec.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.raumbezugsRec.mydb.Schema
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="Raumbezug2vorgang"         ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL =
         String.Format("SELECT * FROM " & CLstart.myViewsNTabs.tabRaumbezug2vorgang & "    where VorgangsID={0}", vorgangsid$)
        nachricht("hinweis = " & myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Ereignisse gespeichert d!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

 

    Public Function initRaumbezugsDT(ByVal vid As Integer) As Boolean   'myGlobalz.sitzung.VorgangsID
        'zuerst die personenIDs holen	  

        Dim SQL As String = ""
        SQL = "select * from raumbezugplus where vorgangsid=" & vid
        nachricht("sql: " & SQL)
        initRaumbezugsDT_by_SQLstring(sql:=SQL)
        nachricht("Es konnten  Raumbezuege zu diesem Vorgang gefunden werden!")
        Return True

    End Function

    ' 
    'Public Shared Function initRaumbezugsDT(ByVal where_columnname$, ByVal Where_Value As String) As Integer
    '    myGlobalz.sitzung.raumbezugsRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '    myGlobalz.sitzung.raumbezugsRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '    myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
    '    If IsNumeric(Where_Value) Then
    '        myGlobalz.sitzung.raumbezugsRec.mydb.SQL = _
    '         String.Format("SELECT * FROM {0} where {1}={2}", myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle, where_columnname$, Where_Value$)       'myGlobalz.sitzung.aktADR.SekID
    '    Else
    '        myGlobalz.sitzung.raumbezugsRec.mydb.SQL = _
    '         String.Format("SELECT * FROM {0} where {1}='{2}'", myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle, where_columnname$, Where_Value$)        'myGlobalz.sitzung.aktADR.SekID
    '    End If
    '    Dim hinweis = myGlobalz.sitzung.raumbezugsRec.getDataDT()
    '    My.Log.WriteEntry(CStr(hinweis))
    '    If myGlobalz.sitzung.raumbezugsRec.mycount < 1 Then
    '        nachricht("Keine Ereignisse gespeichert f!")
    '    Else
    '        nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))
    '    End If
    '    Return CInt(myGlobalz.sitzung.EreignisseRec.mycount)
    'End Function

    Public Shared Sub initRaumbezugsDT_by_SQLstring(ByVal sql As String)
        nachricht("initRaumbezugsDT_by_SQLstring -------------------------")
        nachricht("sql$")
        myGlobalz.sitzung.raumbezugsRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.raumbezugsRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        'myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
        myGlobalz.sitzung.raumbezugsRec.mydb.SQL = sql
        nachricht(myGlobalz.sitzung.raumbezugsRec.getDataDT())
        If myGlobalz.sitzung.raumbezugsRec.mycount < 1 Then
            nachricht("Keine raumbezugsRec gespeichert!")
        Else
            nachricht(String.Format("{0} raumbezugsRec vorhanden", myGlobalz.sitzung.raumbezugsRec.mycount))
        End If
    End Sub

    Shared Sub defineBBOX(ByVal radius As Double, ByVal rb As iRaumbezug)
        With rb
            If rb.isMapEnabled Then
                .box.xl = .punkt.X - radius
                .box.xh = .punkt.X + radius
                .box.yl = .punkt.Y - radius
                .box.yh = .punkt.Y + radius
            Else
                .box.xl = 0
                .box.xh = 0
                .box.yl = 0
                .box.yh = 0
            End If

        End With
    End Sub
    Public Function RB_ParaFoto_holen(ByVal sekid As String) As Boolean
        'Dim hinweis As String
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="parafoto"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("select * from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid$)
        '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        '    Return True
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim RB_ParaFoto_holen: " & ex.ToString)
        '    Return False
        'End Try
    End Function

    Public Function einzelDokument_holen(ByVal docID As String) As Boolean
        'Dim hinweis As String
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="dokumente"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("select * from {0} where dokumentid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, docID$)
        '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        '    Return True
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim einzelDokument_holen: " & ex.ToString)
        '    Return False
        'End Try
    End Function

    Public Function getRaumbezugsCoords_2dokument(ByVal docID As Integer) As myPoint
        'Dim hinweis As String
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="parafoto"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     String.Format("select gkrechts,gkhoch from {0} where dokumentid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, docID)
        '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        '    Dim ap As New myPoint
        '    If myGlobalz.sitzung.tempREC.mycount < 1 Then
        '        nachricht("Es wurden keine Koordinaten zum Dokument gefunden!")
        '        ap.X = 0
        '        ap.Y = 0
        '    Else
        '        ap.X = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("gkrechts")))
        '        ap.Y = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("gkhoch")))
        '    End If
        '    Return ap
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim getRaumbezugsCoords_2dokument: " & ex.ToString)
        '    Return Nothing
        'End Try
    End Function



    Public Function verlinkenVonRaumbezuegen(ByVal quellvid%, ByVal zielVID As Integer) As Boolean
        nachricht("in kopierenVonRaumbezuegen ------------------------------------")
        Dim RBLinksholen_erfolgreich As Boolean = viaKopplung_VorgangID_zu_RaumbezuegeID(quellvid%.ToString) ' nach myGlobalz.sitzung.tempREC
        If RBLinksholen_erfolgreich Then
            If myGlobalz.sitzung.tempREC.mycount > 0 Then
                nachricht("Es wird kopiert")
                RBlinksKopieren(myGlobalz.sitzung.tempREC.dt, zielVID)
                Return True
            Else
                nachricht("Es wird nicht kopiert  weil keine treffer")
                Return False
            End If
        Else
            nachricht("FEHLER: Kopieren von RB nicht erfolgreich c! vorhandene RBs: " & myGlobalz.sitzung.tempREC.mycount)
            Return False
        End If
    End Function

    Public Shared Function RBlinksKopieren(ByVal linksDT As DataTable, ByVal zielvid As Integer) As Boolean
        nachricht("in RBlinksKopieren ------------------------------------") 'raumbezug2vorgang
        Dim aktRID%
        Dim newid%
        Dim erfolgreich As Boolean = False
        Try
            For Each drr As DataRow In linksDT.AsEnumerable
                aktRID% = CInt(drr.Item("RaumbezugsID"))
                newid% = RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(aktRID, zielvid, 1)
                If newid < 1 Then
                    nachricht_und_Mbox("Kopie konnte nicht angelegt werden: ")
                    erfolgreich = False
                Else
                    nachricht("Kopie konnte angelegt werden: ")
                    erfolgreich = True
                End If
            Next
            Return erfolgreich
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei RBlinksKopieren: " & ex.ToString)
            nachricht_und_Mbox("Fehler bei RBlinksKopieren: " & aktRID%)
            Return True
        End Try
    End Function


    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="RaumbezugsID"></param>
    ''' <param name="VorgangsID"></param>
    ''' <param name="status">0-original(normal) 1-Kopie von einem verwandten vorgang</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Function Koppelung_Raumbezug_Vorgang(ByVal RaumbezugsID%, ByVal VorgangsID%, ByVal status As Integer) As Integer
    '    Dim anzahlTreffer&
    '    Dim newid& = -1
    '    Try
    '        If RaumbezugsID% > 0 And VorgangsID% > 0 Then
    '            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL)
    '            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '            myGlobalz.sitzung.tempREC.mydb.Tabelle ="Raumbezug2Vorgang"
    '            myGlobalz.sitzung.tempREC.mydb.SQL = _
    '             String.Format("insert into {0} set  RaumbezugsID={1},VorgangsID={2},status={3}",
    '                           myGlobalz.sitzung.tempREC.mydb.Tabelle,
    '                           RaumbezugsID%,
    '                           VorgangsID%,
    '                           status%)
    '            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
    '            If anzahlTreffer < 1 Then
    '                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
    '                Return -1
    '            Else
    '                Return CInt(newid)
    '            End If
    '        Else
    '            nachricht("Koppelung Koppelung_Vorgang_Raumbezug / person nicht Möglich")
    '            Return -3
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Koppelung_Vorgang_Raumbezug Problem beim Abspeichern: " &
    '                     ex.ToString & vbCrLf & myGlobalz.sitzung.tempREC.mydb.SQL)
    '        Return -2
    '    End Try
    'End Function

    'Public Function Entkoppelung_Raumbezug_Vorgang(ByVal RaumbezugsID%, ByVal VorgangsID As Integer) As Integer
    '    Dim anzahlTreffer&
    '    Dim newid& = -1
    '    nachricht("Entkoppelung_Raumbezug_Vorgang: --------------------------------------------------------")
    '    Try
    '        If RaumbezugsID% > 0 And VorgangsID% > 0 Then
    '            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL)
    '            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '            myGlobalz.sitzung.tempREC.mydb.Tabelle ="Raumbezug2Vorgang"
    '            myGlobalz.sitzung.tempREC.mydb.SQL = _
    '             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
    '             " where  RaumbezugsID=" & RaumbezugsID% & _
    '             " and VorgangsID=" & VorgangsID%
    '            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
    '            If anzahlTreffer < 1 Then
    '                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
    '                Return -1
    '            Else
    '                Return CInt(anzahlTreffer)
    '            End If
    '        Else
    '            nachricht("entKoppelung Koppelung_Vorgang_Raumbezug / person nicht Möglich")
    '            Return -3
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Koppelung_Vorgang_Raumbezug Problem beim Abspeichern: " & vbCrLf & ex.ToString)
    '        Return -2
    '    End Try
    'End Function
    'Public Shared Function hole_Weistauf_GMLid(ByVal adr As ParaAdresse) As String
    '    Dim neuRec As IDB_grundfunktionen
    '    Dim lok_mydb As New clsDatenbankZugriff
    '    Try
    '        lok_mydb.Host = "kis"
    '        lok_mydb.Schema = "albnas"
    '        'lok_mydb.Tabelle ="ax_lagebezeichnungmithausnummer"
    '        lok_mydb.username = "root"
    '        lok_mydb.password = "lkof4"
    '        lok_mydb.dbtyp = "mysql"
    '        neuRec = initP.setDbRecTyp(lok_mydb)
    '        neuRec.mydb = CType(lok_mydb.Clone, clsDatenbankZugriff)
    '        neuRec.mydb.SQL = "SELECT gml_id  FROM ax_lagebezeichnungmithausnummer " &
    '            " where gemeinde='" & adr.Gisadresse.gemeindeNrBig().Replace("438", "") &
    '            "' and regbez='4' and kreis='38' and land='06' " &
    '            "  and lage='" & adr.Gisadresse.strasseCode &
    '            "' and lower(hausnummer)='" & adr.Gisadresse.HausKombi.ToLower & "'"
    '        nachricht(neuRec.getDataDT())
    '        If Not neuRec.dt.IsNothingOrEmpty Then
    '            'treffer
    '            Return neuRec.dt.Rows(0).Item(0).ToString
    '        Else
    '            'keine Treffer
    '            Return ""
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in holeWeistaufGMLid: " & ex.ToString)
    '        Return ""
    '    End Try
    'End Function

    Public Shared Function FlurstuecksDatenAusBasisHolen(ByVal weistauf$, ByRef flst As ParaFlurstueck) As Boolean
        Dim neuRec As IDB_grundfunktionen
        Dim lok_mydb As New clsDatenbankZugriff
        Try
            lok_mydb.Host = "gis"
            lok_mydb.Schema = "basis"
            'lok_mydb.Tabelle ="basis"
            lok_mydb.username = "root"
            lok_mydb.password = "lkof4"
            lok_mydb.dbtyp = "mysql"
            neuRec = initP.setDbRecTyp(lok_mydb)
            neuRec.mydb = CType(lok_mydb.Clone, clsDatenbankZugriff)
            neuRec.mydb.SQL = " SELECT *  FROM basis.basis " &
                              " where binary weistauf='" & weistauf & "'"
            nachricht(neuRec.getDataDT())
            If Not neuRec.dt.IsNothingOrEmpty Then
                'treffer
                flst.normflst.gebucht = CStr(neuRec.dt.Rows(0).Item("gebucht"))
                FST_tools.BASIS_vonDTnachObjekt(flst, (neuRec.dt))

                Return True
            Else
                'keine Treffer
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler in FlurstuecksDatenAusBasisHolen: " & ex.ToString)
            Return False
        End Try

    End Function
End Class
