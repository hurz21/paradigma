Imports System.Data

Namespace RBtoolsns
    Public Class statusSpalteErgaenzenUndFuellen
        Public Shared Function execute(ByVal ZielTable As DataTable,
                                       ByVal KoppelungDT As DataTable,
                                       NeuSpaltenname As String,
                                       kopplelungsIDspalte As String) As Boolean

            statusSpalteErgaenzenUndMitStandardFuellen(ZielTable, NeuSpaltenname, 0)
            RBtoolsns.StatusSpalteFuellen.execute(ZielTable, KoppelungDT, NeuSpaltenname, kopplelungsIDspalte)
        End Function

        Public Shared Sub statusSpalteErgaenzenUndMitStandardFuellen(ByVal ZielTable As DataTable,
                                                                      ByVal NeuSpaltenname As String,
                                                                      standardwert As Int16)
            clsDBtools.SpalteZuDatatableHinzufuegen(ZielTable, NeuSpaltenname, "System.Int16")

            For Each row As DataRow In ZielTable.Rows
                row.Item(NeuSpaltenname) = standardwert
            Next
        End Sub
    End Class

    Public Class StatusSpalteFuellen
        Public Shared Function execute(ByVal ZielTable As DataTable, ByVal KoppelungDT As DataTable,
                                       NeuSpaltenname As String, kopplelungsIDspalte As String) As Boolean
            Dim daten, kupplung As Integer
            Try
                For Each row As DataRow In ZielTable.Rows
                    daten = CInt(row.Item(kopplelungsIDspalte))
                    For Each link As DataRow In KoppelungDT.Rows
                        kupplung = CInt(link.Item(kopplelungsIDspalte))
                        If daten = kupplung Then
                            row.Item(NeuSpaltenname) = CInt(link.Item(NeuSpaltenname))
                            If CInt(link.Item(NeuSpaltenname)) = 1 Then
                                '  Debug.Print("")
                            End If
                        End If
                    Next
                Next
                Return True
            Catch ex As Exception
                nachricht("vehler in StatusSpalteFuellen: ", ex)
                Return False
            End Try
        End Function
    End Class

    Public Class berechneRaumbezugsrange
        Private Shared Sub GetKonstanterZuschlag(ByRef KonstanterZuschlagx As Integer, ByRef KonstanterZuschlagY As Integer,
                                                      ByVal aktRange As clsRange)
            Try
                KonstanterZuschlagx = CInt((aktRange.xh - aktRange.xl) * 0.2)
                KonstanterZuschlagY = CInt((aktRange.yh - aktRange.yl) * 0.2)
            Catch ex As Exception
                KonstanterZuschlagx = 20
                KonstanterZuschlagY = 20
            End Try
        End Sub
        Public Shared Sub execute(ByRef maxRange As clsRange, ByVal dietabelleDT As DataTable)     'myGlobalz.sitzung.raumbezugsRec.dt
            nachricht("berechneRaumbezugsrange ----------------------------")
            Dim KonstanterZuschlagX As Integer = 20
            Dim KonstanterZuschlagY As Integer = 20
            Dim aktRange As New clsRange
            Try
                If dietabelleDT.IsNothingOrEmpty Then
                    nachricht("warnung in berechneRaumbezugsrange: die tabelle ist nothing or empty. Abbruch ")
                    Exit Sub
                End If
                ' Dim maxRange As New LibGISmapgenerator.clsRange
                maxRange.xl = 100000000
                maxRange.yl = 100000000
                maxRange.xh = 0
                maxRange.yh = 0
                If dietabelleDT Is Nothing OrElse dietabelleDT.Rows.Count < 1 Then
                    nachricht("warnung in berechneRaumbezugsrange b: die tabelle ist nothing or empty. Abbruch ")
                    Exit Sub
                End If
                Dim isMapEnabled As Boolean = False
                For Each rau As DataRow In dietabelleDT.AsEnumerable
                    aktRange.xl = Convert.ToDouble(clsDBtools.fieldvalue(rau.Item("xmin")))
                    aktRange.xh = Convert.ToDouble(clsDBtools.fieldvalue(rau.Item("xmax")))
                    aktRange.yl = Convert.ToDouble(clsDBtools.fieldvalue(rau.Item("ymin")))
                    aktRange.yh = Convert.ToDouble(clsDBtools.fieldvalue(rau.Item("ymax")))
                    isMapEnabled = CBool(clsDBtools.toBool(rau.Item("ismapenabled")))
                    If aktRange.istBrauchbar And isMapEnabled Then
                        GetKonstanterZuschlag(KonstanterZuschlagX, KonstanterZuschlagY, aktRange)
                        If aktRange.xl < maxRange.xl Then maxRange.xl = aktRange.xl - KonstanterZuschlagX
                        If aktRange.yl < maxRange.yl Then maxRange.yl = aktRange.yl - KonstanterZuschlagY
                        If aktRange.xh > maxRange.xh Then maxRange.xh = aktRange.xh + KonstanterZuschlagX
                        If aktRange.yh > maxRange.yh Then maxRange.yh = aktRange.yh + KonstanterZuschlagY
                    End If
                Next
                nachricht("berechneRaumbezugsrange ----------------------------")
            Catch ex As Exception
                nachricht("Fehler inberechneRaumbezugsrange " & ex.ToString)
                nachricht("Fehler inberechneRaumbezugsrange tabname:" & dietabelleDT.TableName)
            End Try
        End Sub
    End Class

    Public Class FotoNeuSpeichern_alleDB
        Public Shared Function execute(ByVal aktJPG As clsMyJPG) As Integer
            'Dim parafotoID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_FOTO_abspeichern_Neu(aktJPG)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_FOTO_abspeichern_Neu(aktJPG)
            '    zzz.Dispose
            'End If
            'Return parafotoID%
        End Function
    End Class

    Public Class Raumbezug_abspeichern_Neu_alleDB
        Public Shared Function execute(ByVal fotto As iRaumbezug) As Integer ' myGlobalz.sitzung.aktParaFoto
            'Dim raumbezugsID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID% = zzz.Raumbezug_abspeichern_Neu(fotto)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID% = zzz.Raumbezug_abspeichern_Neu(fotto)
            '       zzz.Dispose
            'End If
            'Return raumbezugsID%
            '' genMapServerEbene.exe()
        End Function
    End Class

    Public Class AdresseNeuSpeichern_alleDB
        Public Shared Function execute() As Integer
            'Dim parafotoID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_Adresse_abspeichern_Neu()
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_Adresse_abspeichern_Neu()
            '    zzz.Dispose
            'End If
            'Return parafotoID%
        End Function
    End Class

    Public Class AdresseEdit_alleDB
        Public Shared Function execute(ByVal sekid As Integer) As Integer
            'Dim parafotoID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_Adresse_abspeichern_Edit(sekid)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_Adresse_abspeichern_Edit(sekid)
            '    zzz.Dispose
            'End If
            'Return parafotoID%
        End Function
    End Class
    Public Class AdresseLoeschen_alleDB
        Public Shared Function execute(ByVal sekid As Integer) As Integer
            'Dim parafotoID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_Adresse_loeschen(sekid)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_Adresse_loeschen(sekid)
            '       zzz.Dispose
            'End If
            'Return parafotoID%
        End Function
    End Class
    Public Class RBFotoLoeschen_alleDB
        Public Shared Function execute(ByVal docid As Integer) As Integer
            'Dim parafotoID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_FOTO_loeschen(docid)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    parafotoID = zzz.RB_FOTO_loeschen(docid)
            '    zzz.Dispose
            'End If
            'Return parafotoID%
        End Function
    End Class

    Public Class Raumbezug_edit_alleDB
        Public Shared Function execute(ByVal rid As Integer, ByVal aktRb As iRaumbezug) As Integer ' myGlobalz.sitzung.aktParaFoto
            'Dim raumbezugsID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID% = zzz.Raumbezug_abspeichern_Edit(rid, aktRb)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID% = zzz.Raumbezug_abspeichern_Edit(rid, aktRb)
            '    zzz.Dispose
            'End If
            'Return raumbezugsID%
        End Function
    End Class

    Public Class Raumbezug_loeschen_byid_alleDB
        Public Shared Function execute(ByVal rid As Integer) As Integer ' myGlobalz.sitzung.aktParaFoto
            'Dim raumbezugsID As Integer
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID = zzz.RB_loeschenByID(rid)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID = zzz.RB_loeschenByID(rid)
            '    zzz.Dispose
            'End If
            'Return raumbezugsID
        End Function
    End Class

    Public Class Raumbezug_loeschen_bySEKid_alleDB
        Public Shared Function execute(ByVal rid As Integer, ByVal doktyp As String) As Integer ' myGlobalz.sitzung.aktParaFoto
            'Dim raumbezugsID%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID% = zzz.RB_loeschenBySekIDTyp(rid, doktyp)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    raumbezugsID% = zzz.RB_loeschenBySekIDTyp(rid, doktyp)
            '    zzz.Dispose
            'End If
            'Return raumbezugsID%
        End Function
    End Class

    Public Class Koppelung_Raumbezug_Vorgang_alleDB
        Public Shared Function execute(ByVal rid As Integer, ByVal vorgangsid As Integer, ByVal status As Integer) As Integer ' myGlobalz.sitzung.aktParaFoto
            'Dim koppelungsid%
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    koppelungsid = zzz.Koppelung_Raumbezug_Vorgang(rid, vorgangsid, status)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    koppelungsid = zzz.Koppelung_Raumbezug_Vorgang(rid, vorgangsid, status)
            '    zzz.Dispose
            'End If
            'Return koppelungsid
        End Function
    End Class

    Public Class initraumbezugsDT_alleDB
        Public Shared Function exe(ByVal vid As Integer) As Boolean
            'Dim erfolg As Boolean
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = (zzz.initRaumbezugsDT(vid))
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.initRaumbezugsDT(vid)
            '       zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class

    Public Class RB_Adresse_holen_by_ID_alleDB
        Public Shared Function exe(ByVal sekid As Integer) As Boolean
            'Dim erfolg As Boolean
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.RB_Adresse_holen_by_ID(CStr(sekid))
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.RB_Adresse_holen_by_ID(CStr(sekid))
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class

    'Public Class Entkoppelung_Raumbezug_Vorgang_alleDB
    '    Public Shared Function exe(ByVal RaumbezugsID%, ByVal VorgangsID As Integer) As Integer
    '        Dim erfolg As Integer
    '        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
    '            Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
    '            erfolg = zzz.Entkoppelung_Raumbezug_Vorgang(RaumbezugsID%, VorgangsID%)
    '            zzz.Dispose
    '        End If
    '        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
    '            Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
    '            erfolg = zzz.Entkoppelung_Raumbezug_Vorgang(RaumbezugsID%, VorgangsID%)
    '            zzz.Dispose
    '        End If
    '        Return erfolg
    '    End Function
    'End Class

    Public Class RB_Flurstueck_holen_alleDB
        Public Shared Function exe(ByVal sekid As String) As Integer
            'Dim erfolg As Integer
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = CInt(zzz.RB_Flurstueck_holen(sekid$))
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = CInt(zzz.RB_Flurstueck_holen(sekid$))
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class

    Public Class verlinkenVonRaumbezuegen_alleDB
        Public Shared Function exe(ByVal quellVid%, ByVal vid As Integer) As Boolean ' quellVid, myGlobalz.sitzung.VorgangsID
            'Dim erfolg As Boolean
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.verlinkenVonRaumbezuegen(quellVid, vid)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.verlinkenVonRaumbezuegen(quellVid, vid)
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class

    Public Class KopierenVonRaumbezuegen_alleDB
        Public Shared Function exe(ByVal quellVid%, ByVal vid As Integer) As Boolean ' quellVid, myGlobalz.sitzung.VorgangsID
            'Dim erfolg As Boolean

            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.kopierenVonRaumbezuegenDB(quellVid, vid)
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class


    Public Class getCoords4ID_Raumbezug_alleDB
        Public Shared Function exe(ByVal id As Integer) As myPoint ' quellVid, myGlobalz.sitzung.VorgangsID
            'Dim erfolg As New myPoint With {.X = 0, .Y = 0}
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.getCoords4ID_Raumbezug(id%)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.getCoords4ID_Raumbezug(id%)
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class


    Public Class RB_ParaFoto_holen_alleDB
        Public Shared Function exe(ByVal sekid As String) As Boolean ' quellVid, myGlobalz.sitzung.VorgangsID
            'Dim erfolg As Boolean
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.RB_ParaFoto_holen(sekid$)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.RB_ParaFoto_holen(sekid$)
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class

    Public Class einzelDokument_holen_alleDB
        Public Shared Function exe(ByVal dokid As String) As Boolean 'einzelDokument_holen(myGlobalz.sitzung.aktDokument.DocID.ToString)
            'Dim erfolg As Boolean
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.einzelDokument_holen(dokid)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.einzelDokument_holen(dokid)
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class

    Public Class RB_Flurstueck_Serial_loeschen
        Public Shared Function exe(ByVal rbid As Integer) As Integer 'myGlobalz.sitzung.aktPolygon.RaumbezugsID
            'Dim erfolg As Integer
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New FST_serialShape_mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.RB_Flurstueck_Serial_loeschen(rbid%)
            '        zzz.dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New FST_serialShape_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.RB_Flurstueck_Serial_loeschen(rbid%)
            '        zzz.dispose
            'End If
            'Return erfolg
        End Function
    End Class
    'Public Class genMapServerEbene
    '    Shared Sub exe()
    '        LocalParameterFiles.erzeugeParameterDatei(False, False)
    '        'Dim up$ = System.Environment.GetEnvironmentVariable("USERPROFILE")
    '        'Dim datei$ = up$ & "\Startmenü\Programme\Dr. Jörg Feinen\Paradigma-Karte\karte.appref-ms"
    '        glob2.neueKarteerstellen()
    '        '  System.Threading.Thread.Sleep(2000)
    '        ' LocalParameterFiles.erzeugeParameterDatei(False, True)
    '    End Sub

    'End Class

    Public Class getRaumbezugsCoords_2dokument_alledb
        Public Shared Function exe(ByVal dokID As Integer) As myPoint ' myGlobalz.sitzung.aktDokument.DocID
            'Dim erfolg As New myPoint With {.X = 0, .Y = 0}
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New DBraumbezug_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.getRaumbezugsCoords_2dokument(dokID%)
            '    zzz.Dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New DBraumbezug_Oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.getRaumbezugsCoords_2dokument(dokID%)
            '    zzz.Dispose
            'End If
            'Return erfolg
        End Function
    End Class

    Class raumbezugsDT2Obj
           Private Sub new
        End Sub
        Public Shared Sub exe(ByVal zeile As DataRow, ByVal rb As iRaumbezug) 'myGlobalz.sitzung.aktADR
            With rb
                .id = CLng(clsDBtools.fieldvalue(zeile.Item("raumbezugsID")))
                .typ = CType(clsDBtools.fieldvalue(zeile.Item("typ")), RaumbezugsTyp)
                .name = clsDBtools.fieldvalue(zeile.Item("TITEL"))
                .SekID = CLng(clsDBtools.fieldvalue(zeile.Item("SekID")))
                .abstract = clsDBtools.fieldvalue(zeile.Item("abstract"))
                .punkt.X = CDbl(clsDBtools.fieldvalue(zeile.Item("rechts")))
                .punkt.Y = CDbl(clsDBtools.fieldvalue(zeile.Item("hoch")))
                .box.xl = CDbl(clsDBtools.fieldvalue(zeile.Item("xmin")))
                .box.xh = CDbl(clsDBtools.fieldvalue(zeile.Item("xmax")))
                .box.yl = CDbl(clsDBtools.fieldvalue(zeile.Item("ymin")))
                .box.yh = CDbl(clsDBtools.fieldvalue(zeile.Item("ymax")))
                Try
                    .Status = CInt((clsDBtools.fieldvalue(zeile.Item("status"))))
                Catch ex As Exception

                End Try

            End With
        End Sub
    End Class
End Namespace
