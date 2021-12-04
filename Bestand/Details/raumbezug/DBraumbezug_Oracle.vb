Imports Devart.Data.Oracle
Imports LibDB
Imports System.Data

Public Class DBraumbezug_Oracle
    
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
    Shared Function setFOTOSQLbody() As String
        Return " SET " & _
         " GKRECHTS=:GKRECHTS" & _
         ",GKHOCH=:GKHOCH" & _
         ",GPSLONGITUDE=:GPSLONGITUDE" & _
         ",GPSLATITUDE=:GPSLATITUDE" & _
         ",GPSDIR=:GPSDIR" & _
         ",UTMRECHTS=:UTMRECHTS" & _
         ",UTMHOCH=:UTMHOCH" & _
         ",EXIFDATUMORIGINAL=:EXIFDATUMORIGINAL" & _
         ",DOKUMENTID=:DOKUMENTID"
    End Function

    Shared Function setRbFotoParams(ByVal COM As OracleCommand, ByVal AKTJPG As clsMyJPG) As Boolean
        COM.Parameters.AddWithValue(":GKRECHTS", AKTJPG.rechts.ToString.Replace(",", "."))
        COM.Parameters.AddWithValue(":GKHOCH", AKTJPG.hoch.ToString.Replace(",", "."))
        COM.Parameters.AddWithValue(":GPSLONGITUDE", AKTJPG.Exifgpslongitude)
        COM.Parameters.AddWithValue(":GPSLATITUDE", AKTJPG.Exifgpslatitude)
        COM.Parameters.AddWithValue(":GPSDIR", AKTJPG.ExifGpsImgDir)
        COM.Parameters.AddWithValue(":UTMRECHTS", "0".ToString.Replace(",", "."))
        COM.Parameters.AddWithValue(":UTMHOCH", "0".ToString.Replace(",", "."))
        COM.Parameters.AddWithValue(":EXIFDATUMORIGINAL", AKTJPG.EXIFDateTimeOriginal)
        COM.Parameters.AddWithValue(":DOKUMENTID", AKTJPG.DokumentID)
    End Function

    Public Function RB_FOTO_abspeichern_Neu(ByVal aktJPG As clsMyJPG) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaFoto"

            Dim SQLupdate$ =
               String.Format("INSERT INTO {0} (GKRECHTS,GKHOCH,GPSLONGITUDE,GPSLATITUDE,GPSDIR,UTMRECHTS,UTMHOCH,EXIFDATUMORIGINAL,DOKUMENTID) " +
                                    " VALUES (:GKRECHTS,:GKHOCH,:GPSLONGITUDE,:GPSLATITUDE,:GPSDIR,:UTMRECHTS,:UTMHOCH,:EXIFDATUMORIGINAL,:DOKUMENTID)",
                                      myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"

            MeineDBConnection.Open()
            nachricht("nach dboeffnen  ")

            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            nachricht("vor setParams  ")
            setRbFotoParams(com, aktJPG)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)

            'If newid < 1 Then
            '    nachricht_und_Mbox("Fehler RB_FOTO_abspeichern_Neu Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
            '    Return -1
            'Else
            '    Return CInt(newid)
            'End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler RB_FOTO_abspeichern_Neu Problem beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function getID4Foto(ByVal dokumentID As Integer) As Integer
        nachricht("getID4Foto ---------------------------")
        ' Dim hinweis as string = ""

        glob2.initTemprecAusVorgangRecOracle()
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaFoto"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         String.Format("select id from {0} where dokumentid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, dokumentID%)
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            Return 0
        End If
        Return CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0)))
    End Function

    Public Function RB_FOTO_loeschen(ByVal dokumentID As Integer) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "PARAFOTO"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("DELETE FROM {0} WHERE DOKUMENTID={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, dokumentID%)
            MeineDBConnection.Open()
            com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()
            '	anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Fehler RB_FOTO_loeschen  :" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return 0
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler RB_FOTO_loeschen: " & ex.ToString)
            Return -1
        End Try
    End Function

    Shared Function setRBSQLBody() As String
        Return " SET " & _
         " TYP=:TYP" & _
         ",SEKID=:SEKID" & _
         ",TITEL=:TITEL" & _
         ",ABSTRACT=:ABSTRACT" & _
         ",RECHTS=:RECHTS" & _
         ",HOCH=:HOCH" & _
         ",XMIN=:XMIN" & _
         ",XMAX=:XMAX" & _
         ",YMIN=:YMIN" & _
         ",YMAX=:YMAX" &
         ",FREITEXT=:FREITEXT" &
         ",ISMAPENABLED=:ISMAPENABLED" & 
         ",FLAECHEQM=:FLAECHEQM" &
         ",LAENGEM=:LAENGEM"
    End Function

    Shared Sub setSQLParamsRB(ByVal com As OracleCommand, ByVal aktrb As iRaumbezug, ByVal rid As Integer)
        com.Parameters.AddWithValue(":TYP", aktrb.typ)
        com.Parameters.AddWithValue(":SEKID", aktrb.SekID)
        com.Parameters.AddWithValue(":TITEL", aktrb.name)
        com.Parameters.AddWithValue(":ABSTRACT", aktrb.abstract)
        com.Parameters.AddWithValue(":RECHTS", CInt(aktrb.punkt.X))
        com.Parameters.AddWithValue(":HOCH", CInt(aktrb.punkt.Y))
        com.Parameters.AddWithValue(":XMIN", CInt(aktrb.box.xl))
        com.Parameters.AddWithValue(":XMAX", CInt(aktrb.box.xh))
        com.Parameters.AddWithValue(":YMIN", CInt(aktrb.box.yl))
        com.Parameters.AddWithValue(":YMAX", CInt(aktrb.box.yh))
        com.Parameters.AddWithValue(":FREITEXT", CStr(aktrb.Freitext))
        com.Parameters.AddWithValue(":ISMAPENABLED", Convert.ToInt16(aktrb.isMapEnabled))
        com.Parameters.AddWithValue(":FLAECHEQM", CInt(aktrb.FLAECHEQM))
        com.Parameters.AddWithValue(":LAENGEM", CInt(aktrb.LAENGEM))
    End Sub

    Public Function Raumbezug_abspeichern_Edit(ByVal raumbezugsID%, ByVal aktrb As iRaumbezug) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        'Typ wird nicht verändert
        Try
            If raumbezugsID < 1 Then
                nachricht_und_Mbox("Fehler: Raumbezug_abspeichern_Edit Updateid<1. abbruch Raumbezug_abspeichern_Edit")
                Return 0
            End If
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("UPDATE   {0}{1} WHERE RAUMBEZUGSID=:RAUMBEZUGSID", myGlobalz.sitzung.tempREC.mydb.Tabelle, setRBSQLBody)
            MeineDBConnection.Open()
            com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
            setSQLParamsRB(com, aktrb, raumbezugsID)
            com.Parameters.AddWithValue(":RAUMBEZUGSID", raumbezugsID)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function Raumbezug_abspeichern_Neu(ByVal aktrb As iRaumbezug) As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug"

            Dim SQLupdate$ =
                String.Format("INSERT INTO {0} (TYP,SEKID,TITEL,ABSTRACT,RECHTS,HOCH," &
                                      " XMIN,XMAX,YMIN,YMAX,FREITEXT,ISMAPENABLED,FLAECHEQM,LAENGEM) " +
                                      " VALUES (:TYP,:SEKID,:TITEL,:ABSTRACT,:RECHTS,:HOCH," &
                                      ":XMIN,:XMAX,:YMIN,:YMAX,:FREITEXT,:ISMAPENABLED,:FLAECHEQM,:LAENGEM)",
                                        myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING RAUMBEZUGSID INTO :R1"

            nachricht("nach setSQLbody : " & SQLupdate)
            MeineDBConnection.Open()
            nachricht("nach dboeffnen  ")

            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            nachricht("vor setParams  ")
            setSQLParamsRB(com, aktrb, 0)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Shared Function SETSQLBODYADRESSERB() As String
        Return " SET " & _
         " GEMEINDENR=:GEMEINDENR" & _
         ",GEMEINDETEXT=:GEMEINDETEXT" & _
         ",STRASSENNAME=:STRASSENNAME" & _
         ",STRCODE=:STRCODE" & _
         ",FS=:FS" & _
         ",HAUSNRKOMBI=:HAUSNRKOMBI" & _
         ",PLZ=:PLZ" & _
         ",POSTFACH=:POSTFACH" & _
         ",ADRESSTYP=:ADRESSTYP"
    End Function
    Shared Sub SETSQLPARAMSADRESSERB(ByVal COM As OracleCommand, ByVal SEKID%)
        '	COM = NEW ORACLECOMMAND(MYGLOBALZ.SITZUNG.TEMPREC.MYDB.SQL, MYGLOBALZ.SITZUNG.TEMPREC.MYCONN)
        COM.Parameters.AddWithValue(":GEMEINDENR", myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        COM.Parameters.AddWithValue(":GEMEINDETEXT", myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName.Trim)
        COM.Parameters.AddWithValue(":STRASSENNAME", myGlobalz.sitzung.aktADR.Gisadresse.strasseName.Trim)
        COM.Parameters.AddWithValue(":STRCODE", myGlobalz.sitzung.aktADR.Gisadresse.strasseCode)
        COM.Parameters.AddWithValue(":FS", myGlobalz.sitzung.aktADR.FS)
        COM.Parameters.AddWithValue(":HAUSNRKOMBI", myGlobalz.sitzung.aktADR.Gisadresse.HausKombi)
        If myGlobalz.sitzung.aktADR.PLZ.IsNothingOrEmpty Then
            myGlobalz.sitzung.aktADR.PLZ = "0"
        End If
        COM.Parameters.AddWithValue(":PLZ", CInt(myGlobalz.sitzung.aktADR.PLZ))
        COM.Parameters.AddWithValue(":POSTFACH", myGlobalz.sitzung.aktADR.Postfach)
        COM.Parameters.AddWithValue(":ADRESSTYP", CInt(myGlobalz.sitzung.aktADR.Adresstyp))
        '  com.Parameters.AddWithValue(":ID", sekid)
    End Sub

    Public Function RB_Adresse_abspeichern_Edit(ByVal sekid As Integer) As Integer 'myGlobalz.sitzung.aktADR.SekID()
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        Try
            If sekid < 1 Then
                nachricht_und_Mbox("fehler updateid<1)")
                Return 0
            End If
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaAdresse"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("update {0}{1} where id=:ID", myGlobalz.sitzung.tempREC.mydb.Tabelle, SETSQLBODYADRESSERB())
            MeineDBConnection.Open()
            com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
            SETSQLPARAMSADRESSERB(com, sekid)
            com.Parameters.AddWithValue(":ID", sekid)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function RB_Adresse_abspeichern_Neu() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaAdresse"

            Dim SQLupdate$ =
            String.Format("INSERT INTO {0} (GEMEINDENR,GEMEINDETEXT,STRASSENNAME,STRCODE,FS,HAUSNRKOMBI,PLZ,POSTFACH,ADRESSTYP) " +
                                  " VALUES (:GEMEINDENR,:GEMEINDETEXT,:STRASSENNAME,:STRCODE,:FS,:HAUSNRKOMBI,:PLZ,:POSTFACH,:ADRESSTYP)",
                                    myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING ID INTO :R1"

            nachricht("nach setSQLbody : " & SQLupdate)
            MeineDBConnection.Open()
            nachricht("nach dboeffnen  ")
            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            nachricht("vor setParams  ")
            SETSQLPARAMSADRESSERB(com, 0)

            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function


    Public Function RB_loeschenBySekIDTyp(ByVal Sekid%, ByVal doktyp2 As String) As Integer
        nachricht("RB_loeschenBySekIDTyp-------------------------------------------")
        Dim anzahlTreffer&, newid&, doktyp%
        Try
            doktyp = CType(doktyp2, Integer)
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("delete from {0} where sekid={1} and typ={2}", myGlobalz.sitzung.tempREC.mydb.Tabelle, Sekid, doktyp)
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
            Return -2
        End Try
    End Function
    Public Function RB_loeschenByID(ByVal raumbezugsid As Integer) As Integer
        Dim anzahlTreffer&, newid&
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("delete from {0} where raumbezugsid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, raumbezugsid)
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
            'delete liefert meist 0 zurück 
            Return 1 
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
            Return -2
        End Try
    End Function
    Public Function RB_Adresse_loeschen(ByVal adressid As Integer) As Integer
        Dim anzahlTreffer&, newid&
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaAdresse"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("delete from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, adressid)
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
            Return -2
        End Try
    End Function

    Public Function getCoords4ID_Raumbezug(ByVal id As Integer) As myPoint
        Dim pt As New myPoint
        glob2.initTemprecAusVorgangRecOracle()
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
                            String.Format("select rechts,hoch from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, id)
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            pt.X = 0
            pt.Y = 0
        Else
            pt.X = CType(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0)), Double)
            pt.Y = CType(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(1)), Double)
        End If
        Return pt
    End Function

    Public Function RB_Flurstueck_holen(ByVal sekid As String) As Boolean
        Dim hinweis As String
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "Paraflurstueck"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
                             String.Format("select * from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid$)
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("fehler beim holen : " & ex.ToString)
            Return False
        End Try
    End Function

    Public Function RB_Adresse_holen_by_ID(ByVal sekid As String) As Boolean
        Dim hinweis As String
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "ParaAdresse"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("select * from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid$)
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return False
        End Try
    End Function

    Private Shared Function viaKopplung_VorgangID_zu_RaumbezuegeID(ByVal vorgangsid As String) As Boolean

        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.raumbezugsRec.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.raumbezugsRec.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "Raumbezug2vorgang"         ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         String.Format("SELECT * FROM {0} where VorgangsID={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, vorgangsid$)
        nachricht("hinweis = " & myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Ereignisse gespeichert d!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

    Public Shared Function viaKopplung_RaumbezugID_zu_VorgangID(ByVal RaumbezugsID As Integer) As Boolean
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.raumbezugsRec.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.raumbezugsRec.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "Raumbezug2vorgang"         ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         String.Format("SELECT * FROM {0} where RaumbezugsID={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, RaumbezugsID%)
        nachricht("hinweis = " & myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Ereignisse gespeichert g!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function
 
    ''' <summary>
    '''     myGlobalz.sitzung.RaumbezugsIDsDT  und   myGlobalz.sitzung.raumbezugsRec.dt werden geliefert
    ''' </summary>
    ''' <param name="vid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function initRaumbezugsDT(ByVal vid As Integer) As Boolean   'myGlobalz.sitzung.VorgangsID
        Try     
            myGlobalz.sitzung.RaumbezugsIDsDT = myGlobalz.sitzung.tempREC.dt.Copy
            Dim SQL As String = ""
            SQL = "select * from raumbezugplus where vorgangsid=" & vid
                nachricht("sql: " & sql)
            initRaumbezugsDT_by_SQLstring(SQL)
            nachricht("Es konnten  Raumbezuege zu diesem Vorgang gefunden werden!")
            Return True
        
        Catch ex As Exception
                    nachricht(String.Format("{0} initRaumbezugsDT :" & ex.ToString))
            Return True
        End Try
    End Function
 
 
    Public Shared Function initRaumbezugsDT(ByVal where_columnname As string, ByVal Where_Value As String) As Integer
        myGlobalz.sitzung.raumbezugsRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.raumbezugsRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle = "raumbezug"
        If IsNumeric(Where_Value) Then
            myGlobalz.sitzung.raumbezugsRec.mydb.SQL = _
             String.Format("SELECT * FROM {0} where {1}={2}", myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle, where_columnname$, Where_Value$)       'myGlobalz.sitzung.aktADR.SekID
        Else
            myGlobalz.sitzung.raumbezugsRec.mydb.SQL = _
             String.Format("SELECT * FROM {0} where {1}='{2}'", myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle, where_columnname$, Where_Value$)        'myGlobalz.sitzung.aktADR.SekID
        End If
        Dim hinweis = myGlobalz.sitzung.raumbezugsRec.getDataDT()
        My.Log.WriteEntry(CStr(hinweis))
        If myGlobalz.sitzung.raumbezugsRec.mycount < 1 Then
            nachricht("Keine Ereignisse gespeichert f!")
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))
        End If
        Return CInt(myGlobalz.sitzung.EreignisseRec.mycount)
    End Function

    Public Shared Sub initRaumbezugsDT_by_SQLstring(ByVal sql As String)
        nachricht("initRaumbezugsDT_by_SQLstring -------------------------")
        nachricht("sql$")
        myGlobalz.sitzung.raumbezugsRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.raumbezugsRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle = "raumbezug"
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
            .box.xl = .punkt.X - radius
            .box.xh = .punkt.X + radius
            .box.yl = .punkt.Y - radius
            .box.yh = .punkt.Y + radius
        End With
    End Sub
    Public Function RB_ParaFoto_holen(ByVal sekid As String) As Boolean
        Dim hinweis As String
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "parafoto"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("select * from {0} where id={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid$)
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim RB_ParaFoto_holen: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Function einzelDokument_holen(ByVal docID As String) As Boolean
        Dim hinweis As String
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "dokumente"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("select * from {0} where dokumentid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, docID$)
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim einzelDokument_holen: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Function getRaumbezugsCoords_2dokument(ByVal docID As Integer) As myPoint
        Dim hinweis As String
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "parafoto"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("select gkrechts,gkhoch from {0} where dokumentid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, docID)
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            Dim ap As New myPoint
            If myGlobalz.sitzung.tempREC.mycount < 1 Then
                nachricht("Es wurden keine Koordinaten zum Dokument gefunden!")
                ap.X = 0
                ap.Y = 0
            Else
                ap.X = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("gkrechts")))
                ap.Y = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("gkhoch")))
            End If
            Return ap
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim getRaumbezugsCoords_2dokument: " & ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Shared Function RBid_zuSekid_holen(ByVal sekid As Long, ByVal rbtyp As Integer) As Integer
        Dim hinweis As String
        Try
            glob2.initTemprecAusVorgangRecOracle()
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "raumbezug"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("select raumbezugsid from {0} where sekid={1} and typ={2}", myGlobalz.sitzung.tempREC.mydb.Tabelle, sekid, rbtyp%)
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0))
            Else
                Return CInt(0)
            End If

        Catch ex As Exception
            nachricht_und_Mbox("Problem beim RBid_zuSekid_holen: " & ex.ToString)
            Return -1
        End Try
    End Function

    '    Public Function verlinkenVonRaumbezuegen(ByVal quellvid%, ByVal zielVID As Integer) As Boolean
    '    nachricht("in kopierenVonRaumbezuegen ------------------------------------")
    '    Dim RBLinksholen_erfolgreich As Boolean = viaKopplung_VorgangID_zu_RaumbezuegeID(quellvid%.ToString) ' nach myGlobalz.sitzung.tempREC
    '    If RBLinksholen_erfolgreich Then
    '        If myGlobalz.sitzung.tempREC.mycount > 0 Then
    '            nachricht("Es wird kopiert")
    '            RBlinksKopieren(myGlobalz.sitzung.tempREC.dt, zielVID)
    '            Return True
    '        Else
    '            nachricht("Es wird nicht kopiert  weil keine treffer")
    '            Return False
    '        End If
    '    Else
    '        nachricht("FEHLER: Kopieren von RB nicht erfolgreich b! vorhandene RBs: " & myGlobalz.sitzung.tempREC.mycount)
    '        Return False
    '    End If
    'End Function
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
            nachricht("FEHLER: Kopieren von RB nicht erfolgreich b! vorhandene RBs: " & myGlobalz.sitzung.tempREC.mycount)
            Return False
        End If
    End Function

    Public Function kopierenVonRaumbezuegenDB(ByVal quellvid%, ByVal zielVID As Integer) As Boolean
        nachricht("in kopierenVonRaumbezuegen ------------------------------------")
        Dim RBLinksholen_erfolgreich As Boolean = viaKopplung_VorgangID_zu_RaumbezuegeID(quellvid%.ToString) ' nach myGlobalz.sitzung.tempREC
        If RBLinksholen_erfolgreich Then
            If myGlobalz.sitzung.tempREC.mycount > 0 Then
                nachricht("Es wird kopiert")
                RBKopieren(myGlobalz.sitzung.tempREC.dt, zielVID)
                Return True
            Else
                nachricht("Es wird nicht kopiert  weil quellvorgang keine raumbezüge hat")
                Return False
            End If
        Else
            nachricht("FEHLER: Kopieren von RB nicht erfolgreich  a! vorhandene RBs: " & myGlobalz.sitzung.tempREC.mycount)
            Return False
        End If
    End Function

    ''' kopiert die daten
    ''' <param name="rid_liste"></param>
    ''' <param name="zielvid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RBKopieren(ByVal rid_liste As DataTable, ByVal zielvid As Integer) As Boolean
        nachricht("in RBKopieren ------------------------------------") 'raumbezug2vorgang
        Dim aktRID, akttyp As Integer
        Dim erfolgreich As Boolean = False
        Try
            If alleRaumbezuegeAUfDTlesen(rid_liste, myGlobalz.sitzung.raumbezugsRec) Then
                clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.raumbezugsRec.dt, "Status", "System.Int16")
                clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.raumbezugsRec.dt, "STATUS", 0)
                For Each drr As DataRow In myGlobalz.sitzung.raumbezugsRec.dt.AsEnumerable
                    akttyp = CInt(drr.Item("TYP"))
                    nachricht("Type: " & akttyp)
                    myGlobalz.sitzung.raumbezugsmodus = "neu"
                    Select Case akttyp
                        Case RaumbezugsTyp.Polygon
                            myGlobalz.sitzung.aktPolygon.Typ = RaumbezugsTyp.Polygon
                            glob2.raumbezugsDataRow2OBJ(drr, myGlobalz.sitzung.aktPolygon)
                            PolygonTools.RB_ParaPolygon_holen(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
                            If PolygonTools.DTaufPolygonObjektabbilden(myGlobalz.sitzung.tempREC.dt) Then
                                '   If Not PolygonTools.polygonSchonInVorgangVorhanden(myGlobalz.sitzung.aktPolygon, myGlobalz.sitzung.VorgangsID) Then
                                myGlobalz.sitzung.aktPolygon.gkstringausserial_generieren()
                                PolygonTools.PolygonNeuSpeichern(0)
                                'End If
                            Else
                                nachricht("Fehler Tabelle liess sich nicht auf Obj abbilden")
                            End If
                        Case RaumbezugsTyp.Polyline
                            myGlobalz.sitzung.aktPolyline.Typ = RaumbezugsTyp.Polyline
                            glob2.raumbezugsDataRow2OBJ(drr, myGlobalz.sitzung.aktPolyline)
                            PolygonTools.RB_ParaPolygon_holen(myGlobalz.sitzung.aktPolyline.RaumbezugsID)
                            If PolygonTools.DTaufPolylineObjektabbilden(myGlobalz.sitzung.tempREC.dt) Then
                                '  If Not PolygonTools.polylineSchonInVorgangVorhanden(myGlobalz.sitzung.aktPolyline, myGlobalz.sitzung.VorgangsID) Then
                                myGlobalz.sitzung.aktPolygon.gkstringausserial_generieren()
                                PolygonTools.polylineAufPolygonUmsetzen()
                                PolygonTools.PolygonNeuSpeichern(0)
                                'End If
                            Else
                                nachricht("Fehler Tabelle liess sich nicht auf Obj abbilden")
                            End If

                        Case RaumbezugsTyp.Umkreis
                            myGlobalz.sitzung.aktPMU.Typ = RaumbezugsTyp.Umkreis
                            glob2.raumbezugsDataRow2OBJ(drr, myGlobalz.sitzung.aktPMU)
                            Dim erfolg As Boolean = ParaUmkreisTools.umkreisHOLEN_alleDB(CInt(myGlobalz.sitzung.aktPMU.SekID))
                            If ParaUmkreisTools.DTaufUmkreisObjektabbilden(myGlobalz.sitzung.tempREC.dt) Then
                                If Not ParaUmkreisTools.umkreisSchonInVorgangVorhanden(myGlobalz.sitzung.aktPMU, myGlobalz.sitzung.aktVorgangsID) Then
                                    ParaUmkreisTools.Umkreis_Neu()
                                End If
                            Else
                                nachricht("Fehler Tabelle liess sich nicht auf Obj abbilden")
                            End If
                        Case RaumbezugsTyp.Adresse
                            myGlobalz.sitzung.aktADR.Typ = RaumbezugsTyp.Adresse
                            glob2.raumbezugsDataRow2OBJ(drr, myGlobalz.sitzung.aktADR)
                            RBtoolsns.RB_Adresse_holen_by_ID_alleDB.exe(CInt(myGlobalz.sitzung.aktADR.SekID)) 'temprec
                            If AdressTools.DTaufAdressObjektabbilden(myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktADR) Then
                                If Not AdressTools.adresseSchonInVorgangVorhanden(myGlobalz.sitzung.aktADR, myGlobalz.sitzung.aktVorgangsID) Then
                                    glob2.Adresse_Neu(CDbl(initP.getValue("MiniMap.radiusAdresse")))
                                    ' myGlobalz.radiusAdresse) 'neu speichern paraadresse  raumbezug und kopplung()
                                End If
                            Else
                                nachricht("Fehler Tabelle liess sich nicht auf Obj abbilden")
                            End If
                        Case RaumbezugsTyp.Flurstueck
                            myGlobalz.sitzung.aktFST.typ = RaumbezugsTyp.Flurstueck
                            glob2.raumbezugsDataRow2OBJ(drr, myGlobalz.sitzung.aktFST)
                            RBtoolsns.RB_Flurstueck_holen_alleDB.exe(CStr(myGlobalz.sitzung.aktFST.SekID)) 'temprec
                            If FST_tools.DTaufFSTObjektabbilden(myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktFST) Then
                                If Not FST_tools.FSTSchonInVorgangVorhanden(myGlobalz.sitzung.aktFST, myGlobalz.sitzung.aktVorgangsID) Then
                                    FST_tools.NeuesFSTspeichern(CDbl(initP.getValue("MiniMap.radiusFlst")))
                                End If
                            End If
                    End Select
                Next
                Return erfolgreich
            Else
                'nicht erfolgreich
            End If





        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei RBKopieren: " & ex.ToString)
            nachricht_und_Mbox("Fehler bei RBKopieren: " & aktRID%)
            Return True
        End Try
    End Function

    ''' <summary>
    ''' kopiert nur die kopplung
    ''' </summary>
    ''' <param name="linksDT"></param>
    ''' <param name="zielvid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
    Public Function Koppelung_Raumbezug_Vorgang(ByVal RaumbezugsID As Integer, ByVal vorgangsid As Integer, ByVal status As Integer) As Integer
        Dim newid& = -1
        Try
            If RaumbezugsID > 0 And vorgangsid > 0 Then
                myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
                myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
                myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
                myGlobalz.sitzung.tempREC.mydb.Tabelle = "Raumbezug2Vorgang"

                myGlobalz.sitzung.tempREC.mydb.SQL = _
                  "INSERT INTO " & myGlobalz.sitzung.tempREC.mydb.Tabelle & "   " &
                  " (RAUMBEZUGSID,VORGANGSID,STATUS) VALUES (:RAUMBEZUGSID,:VORGANGSID,:STATUS) " &
                  " RETURNING ID INTO :R1"
                Dim com As OracleCommand
                MeineDBConnection.Open()
                nachricht("nach dboeffnen  ")
                com = New OracleCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
                com.Parameters.AddWithValue(":VORGANGSID", vorgangsid)
                com.Parameters.AddWithValue(":RAUMBEZUGSID", RaumbezugsID)
                com.Parameters.AddWithValue(":STATUS", status)

                newid = LIBoracle.clsOracleIns.GetNewid(com, myGlobalz.sitzung.tempREC.mydb.SQL)
                MeineDBConnection.Close()
                Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, myGlobalz.sitzung.tempREC.mydb.SQL)

            Else
                nachricht("Koppelung Koppelung_Vorgang_Raumbezug / person nicht Möglich. wwerte sind 0!!!")
                Return -3
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Koppelung_Vorgang_Raumbezug Problem beim Abspeichern: " &
                         ex.ToString & vbCrLf & myGlobalz.sitzung.tempREC.mydb.SQL)
            Return -2
        End Try
    End Function

    Public Function Entkoppelung_Raumbezug_Vorgang(ByVal RaumbezugsID As Integer, ByVal VorgangsID As Integer) As Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        nachricht("Entkoppelung_Raumbezug_Vorgang: --------------------------------------------------------")
        Try
            If RaumbezugsID% > 0 And VorgangsID% > 0 Then
                myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
                myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
                myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
                myGlobalz.sitzung.tempREC.mydb.Tabelle = "Raumbezug2Vorgang"
                myGlobalz.sitzung.tempREC.mydb.SQL = _
                 "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
                 " where  RaumbezugsID=" & RaumbezugsID% & _
                 " and VorgangsID=" & VorgangsID%
                anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
                Return 1
                'If anzahlTreffer < 1 Then
                '    nachricht("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                '    Return -1
                'Else
                '    Return CInt(anzahlTreffer)
                'End If
            Else
                nachricht("entKoppelung Koppelung_Vorgang_Raumbezug / person nicht Möglich")
                Return -3
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Koppelung_Vorgang_Raumbezug Problem beim Abspeichern: " & vbCrLf & ex.ToString)
            Return -2
        End Try
    End Function

    Private Shared Function alleRaumbezuegeAUfDTlesen(ByVal rid_liste As DataTable, ByVal iDB_grundfunktionen As IDB_grundfunktionen) As Boolean
        Dim Instring As String = ""
        Instring = clsDBtools.bildINstringSpaltenname(rid_liste, "raumbezugsid")
        iDB_grundfunktionen.mydb.SQL = "select * from raumbezug where raumbezugsid IN(" & Instring & ")"
        Dim hinerweis As String = iDB_grundfunktionen.getDataDT()
        If Not iDB_grundfunktionen.dt.IsNothingOrEmpty Then
            Return True
        Else
            Return False
        End If
    End Function

End Class
