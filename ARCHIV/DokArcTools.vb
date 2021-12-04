Imports System.Data
Namespace DokArcTools
    ''' <summary>
    ''' einchecken
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dokSpeichernNeu
        Shared Function execute(ByVal aktjpg As clsMyJPG,
                                      ByVal relativpfad As String, _
                                      ByVal Beschreibung As String,
                                      ByVal OriginalFullname As String,
                                      ByVal OriginalName As String,
                                          dateidatum As Date) As Integer
            Dim result% = 0
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.checkin_Dokumente(aktjpg, relativpfad$, Beschreibung$, OriginalFullname$, OriginalName)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.checkin_Dokumente(aktjpg, relativpfad$, Beschreibung$, OriginalFullname$, OriginalName, dateidatum)
                    zzz.dispose
            End If
            Return result
        End Function
    End Class

    ''' <summary>
    ''' löscht nur dok in dokumente tabelle
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dokINDbLoeschen
        Shared Function execute(ByVal dokid As Integer) As Integer 'myGlobalz.sitzung.aktDokument.DocID
            Dim result% = 0
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.loescheDokumentInDb(dokid)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.loescheDokumentInDb(dokid)
                    zzz.dispose
            End If
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
                                ByVal exiflon As String) As Integer
            Dim result As Integer = 0
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.updateDokumentMetadata(dokumentid, veraltet, Beschreibung, revisionssicher)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.updateDokumentMetadata(dokumentid, veraltet, Beschreibung, revisionssicher, FILEDATUM, exiflat, exiflon)
                    zzz.dispose
            End If
            Return result
        End Function
    End Class

    ''' <summary>
    ''' modus kann sein=beides,nurfotos,keinefotos
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dokusVonVorgangHolen
        Shared Function execute(ByVal VorgangsID As Integer, ByVal modus As String) As Boolean 'myGlobalz.Arc.ArcRec
            Dim result As Boolean
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.initDokumente4VorgangDatatable(VorgangsID)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.initDokumente4VorgangDatatable(VorgangsID, modus)
                    zzz.dispose
            End If
            Return result
        End Function
    End Class

    Public Class dokusVonEreignisHolen
        Shared Function execute(ByVal EreignisID As Integer) As Boolean
            Dim result As Boolean
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
            '    result = zzz.initDokumente4EreignisDatatable(EreignisID)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.initDokumente4EreignisDatatable(EreignisID, "beides")
                    zzz.dispose
            End If
            Return result
        End Function
    End Class

    Public Class EntkopplungVonVorgang
        Shared Function execute(ByVal Dokumentid As Integer, ByVal Vorgangsid As Integer) As Integer
            Dim result As Integer
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.entKoppelung_Dokument_Vorgang(Dokumentid, Vorgangsid)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.entKoppelung_Dokument_Vorgang(Dokumentid, Vorgangsid)
                    zzz.dispose
            End If
            Return result
        End Function
    End Class


    Public Class KopplungMitVorgang
        Shared Function execute(ByVal Dokumentid As Integer, ByVal Vorgangsid As Integer) As Integer
            Dim result As Integer
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.Koppelung_Dokument_Vorgang(Dokumentid%, Vorgangsid%)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.Koppelung_Dokument_Vorgang(Dokumentid%, Vorgangsid%)
                    zzz.dispose
            End If
            Return result
        End Function
    End Class

    Public Class KopplungMitEreignis
        Shared Function execute(ByVal Dokumentid As Integer, ByVal ereignisID As Integer) As Integer
            Dim result As Integer
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DokArcMysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.Koppelung_Dokument_Ereignis(Dokumentid%, ereignisID%)
                    zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DokArcOracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.Koppelung_Dokument_Ereignis(Dokumentid, ereignisID)
                    zzz.dispose
            End If
            Return result
        End Function
    End Class

    Public Class EntKoppelung_Dokument_Ereignis_alledb
        Shared Function execute(ByVal Dokumentid%, ByVal EreignisID As Integer) As Integer 'myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktEreignis.ID
            Dim result As Integer
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsEreignisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.EntKoppelung_Dokument_Ereignis(Dokumentid%, EreignisID%)
                zzz.dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsEreignisDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.EntKoppelung_Dokument_Ereignis(Dokumentid%, EreignisID%)
                  zzz.dispose
            End If
            Return result
        End Function
    End Class

    Public Class getID4Foto
        Shared Function execute(ByVal Dokumentid As Integer) As Integer 'myGlobalz.sitzung.aktDokument.DocID
            Dim result As Integer
            If myGlobalz.ARC_MYDB.dbtyp = "mysql" Then
                Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.getID4Foto(Dokumentid%)
                zzz.Dispose
            End If
            If myGlobalz.ARC_MYDB.dbtyp = "oracle" Then
                Dim zzz As New DBraumbezug_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.ARC_MYDB))
                result = zzz.getID4Foto(Dokumentid%)
                zzz.Dispose
            End If
            Return result
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
            Dim sql$ = "update dokumente set revisionssicher=1 where dokumentid in  " &
                        "(select dokumentid from dok2vid where vorgangsid=" & vid & ")"
            Dim anzahl As Integer = DokArcOracle.changeRevisionssicher(sql)
            Return anzahl
        End Function
    End Class
End Namespace
