Imports System.Data
Namespace DokArcTools
    ''' <summary>
    ''' einchecken
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dokSpeichernNeu
        Shared Function execute(ByVal aktjpg As clsMyJPG,
                                      ByVal relativpfad As String,
                                      ByVal Beschreibung As String,
                                      ByVal OriginalFullname As String,
                                      ByVal OriginalName As String,
                                          dateidatum As Date,
                                          zielVorgangsid As Integer, ereignisID As Integer,
                                          NEWSAVEMODE As Boolean, KOMPRESS As Boolean, bearbeiterid As Integer) As Integer
            Dim querie As String
            Dim result% = 0
            clsSqlparam.paramListe.Clear()
            Dim fi As New IO.FileInfo(aktjpg.fullname)
            Dim fo As New IO.FileInfo(OriginalFullname)
            DokArcOracle.seteFiledatum(fi, dateidatum)

            populateParamListeDokument(clsSqlparam.paramListe, relativpfad, fi, Beschreibung, aktjpg, OriginalFullname,
                                       OriginalName, fo, False, dateidatum, zielVorgangsid, ereignisID, NEWSAVEMODE, KOMPRESS, bearbeiterid)

            querie = "INSERT INTO " & CLstart.myViewsNTabs.tabDokumente & "  (RELATIVPFAD,DATEINAMEEXT,TYP,BESCHREIBUNG,CHECKINDATUM,FILEDATUM,EXIFDATUM,EXIFLONG,EXIFLAT,EXIFDIR," +
                            "EXIFHERSTELLER,ORIGINALFULLNAME,INITIAL_,REVISIONSSICHER,ORIGINALNAME,VID,EID,NEWSAVEMODE,KOMPRESS,BEARBEITERID,MB) " +
                 " VALUES (@RELATIVPFAD,@DATEINAMEEXT,@TYP,@BESCHREIBUNG,@CHECKINDATUM,@FILEDATUM,@EXIFDATUM,@EXIFLONG,@EXIFLAT,@EXIFDIR," +
                           "@EXIFHERSTELLER,@ORIGINALFULLNAME,@INITIAL_,@REVISIONSSICHER,@ORIGINALNAME,@VID,@EID,@NEWSAVEMODE,@KOMPRESS,@BEARBEITERID,@MB)"
            result = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "DOKUMENTID")
            fo = Nothing
            fi = Nothing
            Return result
        End Function

        Private Shared Sub populateParamListeDokument(paramListe As List(Of clsSqlparam), relativpfad As String, fi As IO.FileInfo, Beschreibung As String,
                                                      aktjpg As clsMyJPG, OriginalFullname As String,
                                                      OriginalName As String, fo As IO.FileInfo, revisionssicher As Boolean,
                                                      dateidatum As Date, VID As Integer, EID As Integer,
                                                      NEWSAVEMODE As Boolean, KOMPRESS As Boolean, BearbeiterID As Integer)
            Dim extension As String
            extension = DokArcOracle.GetExtension(fi)
            Dim sizeMB As Double
            sizeMB = dokScanPDF.GetFileSizeInMB(fo.FullName)
            'paramListe.Add(New clsSqlparam("VORGANGSID", zielvorgangsid))
            paramListe.Add(New clsSqlparam("RELATIVPFAD", relativpfad$.Replace("\", "/")))
            paramListe.Add(New clsSqlparam("DATEINAMEEXT", fi.Name))
            paramListe.Add(New clsSqlparam("TYP", extension))
            paramListe.Add(New clsSqlparam("BESCHREIBUNG", Beschreibung))
            paramListe.Add(New clsSqlparam("CHECKINDATUM", clsDBtools.makedateMssqlConform(DateTime.Now(), myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))
            paramListe.Add(New clsSqlparam("FILEDATUM", clsDBtools.makedateMssqlConform(dateidatum, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))
            paramListe.Add(New clsSqlparam("EXIFDATUM", clsDBtools.makedateMssqlConform(aktjpg.EXIFDateTimeOriginal, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))
            paramListe.Add(New clsSqlparam("EXIFLONG", aktjpg.Exifgpslongitude))
            paramListe.Add(New clsSqlparam("EXIFLAT", aktjpg.Exifgpslatitude))
            paramListe.Add(New clsSqlparam("EXIFDIR", aktjpg.ExifGpsImgDir))
            paramListe.Add(New clsSqlparam("EXIFHERSTELLER", aktjpg.EXIFhersteller))
            paramListe.Add(New clsSqlparam("ORIGINALFULLNAME", OriginalFullname))
            paramListe.Add(New clsSqlparam("INITIAL_", myGlobalz.sitzung.aktBearbeiter.Initiale))
            paramListe.Add(New clsSqlparam("BEARBEITERID", myGlobalz.sitzung.aktBearbeiter.ID))
            paramListe.Add(New clsSqlparam("REVISIONSSICHER", CInt(revisionssicher)))
            paramListe.Add(New clsSqlparam("NEWSAVEMODE", CInt(NEWSAVEMODE)))
            paramListe.Add(New clsSqlparam("ORIGINALNAME", OriginalName))
            paramListe.Add(New clsSqlparam("VID", VID))
            paramListe.Add(New clsSqlparam("EID", EID))
            paramListe.Add(New clsSqlparam("KOMPRESS", CInt(KOMPRESS)))
            paramListe.Add(New clsSqlparam("MB", CDbl(sizeMB)))

        End Sub
    End Class

    ''' <summary>
    ''' löscht nur dok in dokumente tabelle
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dokINDbLoeschen
        Shared Function execute(ByVal dokid As Integer) As Integer 'myGlobalz.sitzung.aktDokument.DocID
            Dim result% = 1 : Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabDokumente & "  where dokumentid=" & dokid.ToString
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return result
        End Function
    End Class
    Public Class dokUpdate
        Shared Function execute(ByVal dokumentid As Integer,
                                ByVal veraltet As Boolean,
                                ByVal Beschreibung As String,
                                ByVal revisionssicher As Boolean,
                                ByVal FILEDATUM As Date,
                                ByVal exiflat As String,
                                ByVal exiflong As String) As Integer
            Dim result As Integer = 0
            Dim querie As String
            clsSqlparam.paramListe.Clear()

            querie = "UPDATE " & CLstart.myViewsNTabs.tabDokumente & "  SET " &
                    " VERALTET=@VERALTET" &
                    ",BESCHREIBUNG=@BESCHREIBUNG " &
                    ",REVISIONSSICHER=@REVISIONSSICHER " &
                    ",EXIFLAT=@EXIFLAT " &
                    ",EXIFLONG=@EXIFLONG " &
                    ",INITIAL_=@INITIAL_" &
                    ",FILEDATUM=@FILEDATUM" &
                    " WHERE DOKUMENTID=@DOKUMENTID"
            If exiflat.IsNothingOrEmpty Then exiflat = ""
            If exiflong.IsNothingOrEmpty Then exiflong = ""
            clsSqlparam.paramListe.Add(New clsSqlparam("VERALTET", Convert.ToBoolean(veraltet)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BESCHREIBUNG", Beschreibung))
            clsSqlparam.paramListe.Add(New clsSqlparam("DOKUMENTID", dokumentid))
            clsSqlparam.paramListe.Add(New clsSqlparam("FILEDATUM", FILEDATUM))
            clsSqlparam.paramListe.Add(New clsSqlparam("INITIAL_", myGlobalz.sitzung.aktBearbeiter.Initiale))
            clsSqlparam.paramListe.Add(New clsSqlparam("EXIFLAT", exiflat))
            clsSqlparam.paramListe.Add(New clsSqlparam("EXIFLONG", exiflong))
            clsSqlparam.paramListe.Add(New clsSqlparam("REVISIONSSICHER", revisionssicher))

            result = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "DOKUMENTID")
            Return result
        End Function
    End Class

    ''' <summary>
    ''' modus kann sein=beides,nurfotos,keinefotos
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dokusVonVorgangHolen
        Shared Function execute(ByVal strVorgangsID As String, ByVal modus As String, alleBilder As Boolean, eid As Integer) As Boolean 'myGlobalz.Arc.ArcRec
            Return exekern(strVorgangsID, modus, alleBilder, eid)
        End Function
        Shared Async Function executeAsync(ByVal strVorgangsID As String, ByVal modus As String, alleBilder As Boolean, eid As Integer) As System.Threading.Tasks.Task(Of Boolean) 'myGlobalz.Arc.ArcRec
            Return exekern(strVorgangsID, modus, alleBilder, eid)
        End Function

        Private Shared Function exekern(strVorgangsID As String, modus As String, alleBilder As Boolean, eid As Integer) As Boolean
            Return DokArcOracle.initDokumente4VorgangDatatable(strVorgangsID, modus, alleBilder, eid)
        End Function
    End Class

    Public Class dokusVonEreignisHolen
        Shared Function execute(ByVal EreignisID As Integer) As Boolean
            Try
                l("dokusVonEreignisHolen---------------------- anfang")
                Dim result As Boolean : Dim hinweis As String = ""
                myGlobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.view_ereignis2dok2 & " as edee  where ID=" & EreignisID &
                                                 " and dateinameext is not null order by filedatum desc"

                'myGlobalz.Arc.ArcRec.mydb.SQL = "SELECT * FROM  ereignis2dok2   where ID=" & EreignisID &
                '                                 " and dateinameext is not null order by filedatum desc"

                DokArcOracle.bildesqlStringFuerdokumente("beides")
                myGlobalz.Arc.ArcRec.dt = getDT4Query(myGlobalz.Arc.ArcRec.mydb.SQL, myGlobalz.Arc.ArcRec, hinweis)
                l("count: " & myGlobalz.Arc.ArcRec.dt.Rows.Count)
                If myGlobalz.Arc.ArcRec.dt.Rows.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
                Return result
                l("dokusVonEreignisHolen---------------------- ende")
            Catch ex As Exception
                l("Fehler in dokusVonEreignisHolen: ", ex)
                Return False
            End Try
        End Function
    End Class
    Public Class KopplungMitEreignis
        Shared Function execute(ByVal Dokumentid As Integer, ByVal ereignisID As Integer) As Integer
            Dim dt As DataTable : Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "update " & CLstart.myViewsNTabs.tabDokumente & "  set eid=" & ereignisID & " where dokumentid=" & Dokumentid
            dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function
    End Class

    Public Class getID4Foto
        Shared Function execute(ByVal Dokumentid As Integer) As Integer 'myGlobalz.sitzung.aktDokument.DocID
            Dim hinweis As String = ""
            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabPARAFOTO & "   where dokumentid=" & Dokumentid
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                Return 0
            End If
            Return CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0)))
        End Function
    End Class

    Public Class dokusRevisionssichermachen
        ''' <summary>
        ''' liefert anzahl der geänderten dokumente zurück als integer
        ''' </summary>
        ''' <param name="vid"></param>
        ''' <returns>liefert anzahl der geänderten dokumente zurück als integer</returns>
        ''' <remarks></remarks>
        Shared Function fuerVorgangsID(vid As Integer) As Integer
            nachricht("dokusRevisionssichermachen ---------------------------------")
            'Dim sql$ = "update " & CLstart.myViewsNTabs.tabdokumente & "  set revisionssicher=1 where dokumentid in  " &
            '            "(select dokumentid from dok2vid where vorgangsid=" & vid & ")"
            Dim sql$ = "update " & CLstart.myViewsNTabs.tabDokumente & "  set revisionssicher=1 where dokumentid=" & vid & ")"
            Dim anzahl As Integer = DokArcOracle.changeRevisionssicher(sql)
            Return anzahl
        End Function
    End Class
End Namespace
