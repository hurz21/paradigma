#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports Devart.Data.Oracle
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data
Public Class DBraumbezug_Oracle
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
    Shared Function setFOTOSQLbody() As String
        Return " SET " &
         " GKRECHTS=:GKRECHTS" &
         ",GKHOCH=:GKHOCH" &
         ",GPSLONGITUDE=:GPSLONGITUDE" &
         ",GPSLATITUDE=:GPSLATITUDE" &
         ",GPSDIR=:GPSDIR" &
         ",UTMRECHTS=:UTMRECHTS" &
         ",UTMHOCH=:UTMHOCH" &
         ",EXIFDATUMORIGINAL=:EXIFDATUMORIGINAL" &
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
        Return True
    End Function


    Shared Function setRBSQLBody() As String
        Return " SET " &
         " TYP=:TYP" &
         ",SEKID=:SEKID" &
         ",TITEL=:TITEL" &
         ",ABSTRACT=:ABSTRACT" &
         ",RECHTS=:RECHTS" &
         ",HOCH=:HOCH" &
         ",XMIN=:XMIN" &
         ",XMAX=:XMAX" &
         ",YMIN=:YMIN" &
         ",YMAX=:YMAX" &
         ",FREITEXT=:FREITEXT" &
         ",ISMAPENABLED=:ISMAPENABLED" &
         ",FLAECHEQM=:FLAECHEQM" &
         ",LAENGEM=:LAENGEM" &
         ",MITETIKETT=:MITETIKETT"
    End Function

    Shared Sub setSQLParamsRB(ByVal com As OracleCommand, ByVal aktrb As iRaumbezug, ByVal rid As Integer)
        com.Parameters.AddWithValue(":TYP", aktrb.typ)
        com.Parameters.AddWithValue(":SEKID", aktrb.SekID)
        com.Parameters.AddWithValue(":TITEL", aktrb.name.Trim)
        com.Parameters.AddWithValue(":ABSTRACT", aktrb.abstract.Trim)
        com.Parameters.AddWithValue(":RECHTS", CInt(aktrb.punkt.X))
        com.Parameters.AddWithValue(":HOCH", CInt(aktrb.punkt.Y))
        com.Parameters.AddWithValue(":XMIN", CInt(aktrb.box.xl))
        com.Parameters.AddWithValue(":XMAX", CInt(aktrb.box.xh))
        com.Parameters.AddWithValue(":YMIN", CInt(aktrb.box.yl))
        com.Parameters.AddWithValue(":YMAX", CInt(aktrb.box.yh))
        com.Parameters.AddWithValue(":FREITEXT", CStr(aktrb.Freitext).Trim)
        com.Parameters.AddWithValue(":ISMAPENABLED", Convert.ToInt16(aktrb.isMapEnabled))
        com.Parameters.AddWithValue(":FLAECHEQM", CInt(aktrb.FLAECHEQM))
        com.Parameters.AddWithValue(":LAENGEM", CInt(aktrb.LAENGEM))
        com.Parameters.AddWithValue(":MITETIKETT", CInt(aktrb.MITETIKETT))
    End Sub



    Shared Function SETSQLBODYADRESSERB() As String
        Return " SET " &
         " GEMEINDENR=:GEMEINDENR" &
         ",GEMEINDETEXT=:GEMEINDETEXT" &
         ",STRASSENNAME=:STRASSENNAME" &
         ",STRCODE=:STRCODE" &
         ",FS=:FS" &
         ",HAUSNRKOMBI=:HAUSNRKOMBI" &
         ",PLZ=:PLZ" &
         ",POSTFACH=:POSTFACH" &
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



    Public Shared Function viaKopplung_RaumbezugID_zu_VorgangID(ByVal RaumbezugsID As Integer) As Boolean
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.raumbezugsRec.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.raumbezugsRec.mydb.Schema
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="Raumbezug2vorgang"         ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabRaumbezug2vorgang & "    where RaumbezugsID=" & RaumbezugsID%
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
            'myGlobalz.sitzung.RaumbezugsIDsDT = myGlobalz.sitzung.tempREC.dt.Copy
            Dim SQL As String = ""
            SQL = "select * from [Paradigma].[dbo].raumbezugplus where vorgangsid=" & vid & " order by abstract"
            nachricht("sql: " & SQL)
            initRaumbezugsDT_by_SQLstring(SQL)
            nachricht("Es konnten  Raumbezuege zu diesem Vorgang gefunden werden!")
            Return True

        Catch ex As Exception
            nachricht(String.Format("{0} initRaumbezugsDT :" ,ex))
            Return True
        End Try
    End Function


    Public Shared Function initRaumbezugsDT(ByVal where_columnname$, ByVal Where_Value As String) As Integer
        myGlobalz.sitzung.raumbezugsRec.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.raumbezugsRec.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        'myGlobalz.sitzung.raumbezugsRec.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
        If IsNumeric(Where_Value) Then
            myGlobalz.sitzung.raumbezugsRec.mydb.SQL =
             String.Format("SELECT * FROM " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  where {0}={1}", where_columnname$, Where_Value$)       'myGlobalz.sitzung.aktADR.SekID
        Else
            myGlobalz.sitzung.raumbezugsRec.mydb.SQL =
             String.Format("SELECT * FROM " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  where {0}='{1}'", where_columnname$, Where_Value$)        'myGlobalz.sitzung.aktADR.SekID
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
            .box.xl = .punkt.X - radius
            .box.xh = .punkt.X + radius
            .box.yl = .punkt.Y - radius
            .box.yh = .punkt.Y + radius
        End With
    End Sub



    Public Shared Function RBid_zuSekid_holen(ByVal sekid As Long, ByVal rbtyp As Integer) As Integer
        Dim hinweis As String
        Try

            'myGlobalz.sitzung.tempREC.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
            myGlobalz.sitzung.tempREC.mydb.SQL =
             String.Format("select raumbezugsid from " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  where sekid={0} and typ={1}",
                          sekid, rbtyp%)
            hinweis = myGlobalz.sitzung.tempREC.getDataDT()
            If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0))
            Else
                Return CInt(0)
            End If

        Catch ex As Exception
            nachricht_und_Mbox("Problem beim RBid_zuSekid_holen: " ,ex)
            Return -1
        End Try
    End Function
End Class
