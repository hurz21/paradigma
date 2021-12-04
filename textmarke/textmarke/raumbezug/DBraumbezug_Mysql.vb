Imports MySql.Data.MySqlClient
Imports System.Data

Public Class DBraumbezug_Mysql

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
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub
    Shared Function setFOTOSQLbody() As String
        Return " set " &
         " GKrechts=@GKrechts" &
         ",GKhoch=@GKhoch" &
         ",GPSlongitude=@GPSlongitude" &
         ",GPSlatitude=@GPSlatitude" &
         ",GPSdir=@GPSdir" &
         ",UTMrechts=@UTMrechts" &
         ",UTMhoch=@UTMhoch" &
         ",EXIFDatumOriginal=@EXIFDatumOriginal" &
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
        Return True
    End Function



    Shared Function setSQLParamsRB() As String
        Return " set " &
         " typ=@typ" &
         ",Sekid=@Sekid" &
         ",Name=@Name" &
         ",Abstract=@Abstract" &
         ",rechts=@rechts" &
         ",hoch=@hoch" &
         ",xmin=@xmin" &
         ",xmax=@xmax" &
         ",ymin=@ymin" &
         ",ymax=@ymax"
    End Function
    Shared Sub setSQLParamsRB(ByVal com As MySqlCommand, ByVal aktrb As iRaumbezug, ByVal rid%)
        com.Parameters.AddWithValue("@typ", myglobalz.sitzung.akt_raumbezugsTyp)
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


    Shared Function setsqlbodyAdresseRB() As String
        Return " set " &
         " Gemeindenr=@Gemeindenr" &
         ",Gemeindetext=@Gemeindetext" &
         ",Strassenname=@Strassenname" &
         ",strcode=@strcode" &
         ",fs=@fs" &
         ",hausnrkombi=@hausnrkombi" &
         ",PLZ=@PLZ" &
         ",Postfach=@Postfach" &
         ",adresstyp=@adresstyp"
    End Function
    Shared Sub setSQLparamsAdresseRB(ByVal com As MySqlCommand, ByVal sekid%)
        '	com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.myconn)
        com.Parameters.AddWithValue("@Gemeindenr", myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        com.Parameters.AddWithValue("@Gemeindetext", myglobalz.sitzung.aktADR.Gisadresse.gemeindeName)
        com.Parameters.AddWithValue("@Strassenname", myglobalz.sitzung.aktADR.Gisadresse.strasseName)
        com.Parameters.AddWithValue("@strcode", myglobalz.sitzung.aktADR.Gisadresse.strasseCode)
        com.Parameters.AddWithValue("@fs", myglobalz.sitzung.aktADR.FS)
        com.Parameters.AddWithValue("@hausnrkombi", myglobalz.sitzung.aktADR.Gisadresse.HausKombi)
        com.Parameters.AddWithValue("@PLZ", CInt(myglobalz.sitzung.aktADR.PLZ))
        com.Parameters.AddWithValue("@Postfach", myglobalz.sitzung.aktADR.Postfach)
        com.Parameters.AddWithValue("@adresstyp", CInt(myglobalz.sitzung.aktADR.Adresstyp))
        com.Parameters.AddWithValue("@ID", sekid)
    End Sub





    'Private Shared Function viaKopplung_VorgangID_zu_RaumbezuegeID(ByVal vorgangsid As String) As Boolean

    '    myglobalz.sitzung.tempREC.mydb.Host = myglobalz.sitzung.raumbezugsRec.mydb.Host
    '    myglobalz.sitzung.tempREC.mydb.Schema = myglobalz.sitzung.raumbezugsRec.mydb.Schema
    '    'myglobalz.sitzung.tempREC.mydb.Tabelle ="Raumbezug2vorgang"         ''& " order by ts desc"
    '    myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabraumbezug2vorgang      & "    where VorgangsID=" & vorgangsid
    '    nachricht("hinweis = " & myglobalz.sitzung.tempREC.getDataDT())
    '    If myglobalz.sitzung.tempREC.mycount < 1 Then
    '        nachricht("Keine Ereignisse gespeichert d!")
    '        Return False
    '    Else
    '        nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.tempREC.mycount))
    '        Return True
    '    End If
    'End Function


    'Public Shared Function initRaumbezugsDT(ByVal where_columnname$, ByVal Where_Value As String) As Integer
    '    myglobalz.sitzung.raumbezugsRec.mydb.Host = myglobalz.sitzung.VorgangREC.mydb.Host
    '    myglobalz.sitzung.raumbezugsRec.mydb.Schema = myglobalz.sitzung.VorgangREC.mydb.Schema
    '    'myglobalz.sitzung.raumbezugsRec.mydb.Tabelle =" & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  
    '    If IsNumeric(Where_Value) Then
    '        myGlobalz.sitzung.raumbezugsRec.mydb.SQL =
    '         String.Format("SELECT * FROM " & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  where {0}={1}", where_columnname$, Where_Value$)       'myGlobalz.sitzung.aktADR.SekID
    '    Else
    '        myGlobalz.sitzung.raumbezugsRec.mydb.SQL =
    '         String.Format("SELECT * FROM " & CLstart.myViewsNTabs.tabRAUMBEZUG     & "  where {0}='{1}'", where_columnname$, Where_Value$)        'myGlobalz.sitzung.aktADR.SekID
    '    End If
    '    Dim hinweis = myglobalz.sitzung.raumbezugsRec.getDataDT()
    '    My.Log.WriteEntry(CStr(hinweis))
    '    If myglobalz.sitzung.raumbezugsRec.mycount < 1 Then
    '        nachricht("Keine Ereignisse gespeichert f!")
    '    Else
    '        nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.EreignisseRec.mycount))
    '    End If
    '    Return CInt(myglobalz.sitzung.EreignisseRec.mycount)
    'End Function



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








    'Public Function verlinkenVonRaumbezuegen(ByVal quellvid%, ByVal zielVID As Integer) As Boolean
    '    nachricht("in kopierenVonRaumbezuegen ------------------------------------")
    '    Dim RBLinksholen_erfolgreich As Boolean = viaKopplung_VorgangID_zu_RaumbezuegeID(quellvid%.ToString) ' nach myGlobalz.sitzung.tempREC
    '    If RBLinksholen_erfolgreich Then
    '        If myglobalz.sitzung.tempREC.mycount > 0 Then
    '            nachricht("Es wird kopiert")
    '            RBlinksKopieren(myglobalz.sitzung.tempREC.dt, zielVID)
    '            Return True
    '        Else
    '            nachricht("Es wird nicht kopiert  weil keine treffer")
    '            Return False
    '        End If
    '    Else
    '        nachricht("FEHLER: Kopieren von RB nicht erfolgreich c! vorhandene RBs: " & myglobalz.sitzung.tempREC.mycount)
    '        Return False
    '    End If
    'End Function

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
            nachricht_und_Mbox("Fehler bei RBlinksKopieren: " ,ex)
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


    Public Shared Function hole_Weistauf_GMLid(ByVal adr As ParaAdresse) As String
        Dim neuRec As IDB_grundfunktionen
        Dim lok_mydb As New clsDatenbankZugriff
        Try
            lok_mydb.Host = "gis"
            lok_mydb.Schema = "albnas"
            'lok_mydb.Tabelle ="ax_lagebezeichnungmithausnummer"
            lok_mydb.username = "root"
            lok_mydb.password = "lkof4"
            lok_mydb.dbtyp = "mysql"
            neuRec = initP.setDbRecTyp(lok_mydb)
            neuRec.mydb = CType(lok_mydb.Clone, clsDatenbankZugriff)
            neuRec.mydb.SQL = "SELECT gml_id  FROM public.""ax_lagebezeichnungmithausnummer"" " &
                " where gemeinde='" & adr.Gisadresse.gemeindeNrBig().Replace("438", "") &
                "' and regbez='4' and kreis='38' and land='06' " &
                "  and lage='" & adr.Gisadresse.strasseCode &
                "' and lower(hausnummer)='" & adr.Gisadresse.HausKombi.ToLower.Trim & "'"
            Dim hinweis = neuRec.getDataDT()
            nachricht(hinweis)
            If Not neuRec.dt.IsNothingOrEmpty Then
                'treffer
                Return neuRec.dt.Rows(0).Item(0).ToString
            Else
                'keine Treffer
                Return ""
            End If
        Catch ex As Exception
            nachricht("Fehler in holeWeistaufGMLid: " ,ex)
            Return ""
        End Try
    End Function


    Public Shared Function FlurstuecksDatenAusBasisHolen(ByVal adr As clsAdress, ByRef flst As ParaFlurstueck) As Boolean
        Dim neuRec As IDB_grundfunktionen
        Dim lok_mydb As New clsDatenbankZugriff
        Try
            lok_mydb.Host = "gis"
            lok_mydb.Schema = "postgis20"
            'lok_mydb.Tabelle ="flurkarte.basis_f"
            lok_mydb.username = "postgres"
            lok_mydb.password = "lkof4"
            lok_mydb.dbtyp = "postgres"
            neuRec = initP.setDbRecTyp(lok_mydb)
            neuRec.mydb = CType(lok_mydb.Clone, clsDatenbankZugriff)

            Dim innerSQL As String
            innerSQL = String.Format("  SELECT geom  " &
                                 "  FROM flurkarte.halofs where gemeindenr={0} AND strcode={1} AND Hausnr='{2}' and lower(zusatz)='{3}'",
                              adr.gemeindeNrBig, adr.strasseCode, adr.hausNr,
                              adr.hausZusatz.ToLower.Trim)
            l(innerSQL)

            neuRec.mydb.SQL = "  SELECT * " &
                        "  FROM flurkarte.basis_f " &
                        "  WHERE ST_contains( flurkarte.basis_f.geom,(" & innerSQL & "  )" & "  );"
            nachricht(neuRec.getDataDT())
            If Not neuRec.dt.IsNothingOrEmpty Then
                'treffer
                flst.normflst.gebucht = CStr(neuRec.dt.Rows(0).Item("istgebucht"))
                FST_tools.BASIS_vonDTnachObjekt(flst, (neuRec.dt))

                Return True
            Else
                'keine Treffer
                Return False
            End If
        Catch ex As Exception
            nachricht("Fehler in FlurstuecksDatenAusBasisHolen: " ,ex)
            Return False
        End Try

    End Function
End Class
