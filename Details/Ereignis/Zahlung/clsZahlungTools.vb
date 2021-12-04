Imports LibDB.LIBDB
Imports System.Data

Namespace ZahlungToolsNs
    Public Class zahlungDTtoOBJ
        Public Shared Function execute(ByVal rau As DataRow, ByVal aktzahl As clsZahlung) As Boolean
            Try
                With myGlobalz.sitzung.tempREC.dt.Rows(0)
                    aktzahl.clear()
                    aktzahl.SachgebietNr = clsDBtools.fieldvalue(.Item("Sachgebietsnr"))
                    aktzahl.AZ = clsDBtools.fieldvalue(.Item("Aktenzeichen"))
                    aktzahl.Initiale = clsDBtools.fieldvalue(.Item("BearbeiterInitial"))
                    aktzahl.Typ = clsDBtools.fieldvalue(.Item("Typ"))
                    aktzahl.Eingang = CBool(clsDBtools.fieldvalue(.Item("Richtung")))
                    aktzahl.VerschicktAm = clsDBtools.fieldvalueDate(.Item("verschicktAm"))
                    aktzahl.AngeordnetAm = clsDBtools.fieldvalueDate(.Item("ANGEORDNETAM"))
                    aktzahl.EingangAm = clsDBtools.fieldvalueDate(.Item("eingangAm"))
                    aktzahl.Betrag = CDbl(clsDBtools.fieldvalue(.Item("Betrag")))
                    aktzahl.Zahler = clsDBtools.fieldvalue(.Item("Zahler"))
                    aktzahl.Notiz = clsDBtools.fieldvalue(.Item("Notiz"))
                    aktzahl.ZahlungsID = CInt(clsDBtools.fieldvalue(.Item("ZahlungsID")))
                    aktzahl.HausHaltsstelle = CStr(clsDBtools.fieldvalue(.Item("HHST")))
                    aktzahl.istAngeordnet = CBool(clsDBtools.fieldvalue(.Item("istAngeordnet")))
                    aktzahl.istAnordnungbestellt = CBool(clsDBtools.fieldvalue(.Item("istAnordnungbestellt")))
                End With
            Catch ex As Exception
                nachricht_und_Mbox("FEhler: zahlungDTtoOBJ" & vbCrLf & ex.ToString)
            End Try
        End Function
    End Class

    Public Class NeuesZahlungsEreigniserzeugen
        Private Shared Function GetBeschreibung() As String

            Return String.Format("{0}, {1}: {2}, " & getVonAn(myGlobalz.sitzung.aktEreignis.Richtung) & ": {3}",
                                   myGlobalz.sitzung.aktEreignis.Richtung,
                                    myGlobalz.sitzung.aktZahlung.Typ,
                                    myGlobalz.sitzung.aktZahlung.Betrag.ToString("c"),
                                    myGlobalz.sitzung.aktZahlung.Zahler
                                    )
        End Function
        Public Shared Sub execute(ByVal modus As String, datum As Date)
            'neues ereignis erzeugen
            'myGlobalz.sitzung.aktEreignis.clearValues()
            With myGlobalz.sitzung
                .Ereignismodus = modus
                .aktEreignis.Datum = datum
                .aktEreignis.Art = "Zahlung"
                zahlungsEingang2EreignisRichtung()
                .aktEreignis.DokumentID = 0
                .aktEreignis.Beschreibung = GetBeschreibung()
            End With
        End Sub

        Shared Sub zahlungsEingang2EreignisRichtung()
            If myGlobalz.sitzung.aktZahlung.Eingang = True Then
                myGlobalz.sitzung.aktEreignis.Richtung = "Eingang"
            Else
                myGlobalz.sitzung.aktEreignis.Richtung = "Ausgang"
            End If
        End Sub

        Private Shared Function getVonAn(richtung As String) As String
            If richtung.ToLower = "ausgang" Then
                Return " an "
            Else
                Return " von "
            End If
        End Function

    End Class
    Public Class ZahlungNeu_alleDB
        Public Shared Function execute() As Integer
            Dim parafotoID%
            If myGlobalz.zahlung_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsZAHLUNGDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.zahlung_MYDB))
                parafotoID = zzz.Neu_speichern_zahlung()
                    zzz.dispose
            End If
            If myGlobalz.zahlung_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsZAHLUNGDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.zahlung_MYDB))
                parafotoID = zzz.Neu_speichern_zahlung()
                    zzz.dispose
            End If
            Return parafotoID%
        End Function
    End Class

    Public Class ZahlungEdit_alleDB
        Public Shared Function execute(ByVal zid As Integer) As Integer 'myGlobalz.sitzung.aktZahlung.ZahlungsID
            Dim parafotoID%
            If myGlobalz.zahlung_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsZAHLUNGDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.zahlung_MYDB))
                parafotoID = zzz.Edit_speichern_zahlung(zid%)
                    zzz.dispose
            End If
            If myGlobalz.zahlung_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsZAHLUNGDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.zahlung_MYDB))
                parafotoID = zzz.Edit_speichern_zahlung(zid%)
                    zzz.dispose
            End If
            Return parafotoID%
        End Function
    End Class


    Public Class Zahlung_loeschen_alledb 'myGlobalz.sitzung.aktZahlung.ZahlungsID
        Public Shared Function execute(ByVal zid As Integer) As Integer 'myGlobalz.sitzung.aktZahlung.ZahlungsID
            Dim parafotoID%
            If myGlobalz.zahlung_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsZAHLUNGDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.zahlung_MYDB))
                parafotoID = zzz.Zahlung_loeschen(zid%)
                    zzz.dispose
            End If
            If myGlobalz.zahlung_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsZAHLUNGDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.zahlung_MYDB))
                parafotoID = zzz.Zahlung_loeschen(zid%)
                    zzz.dispose
            End If
            Return parafotoID%
        End Function
    End Class




    Public Class zahlung_und_ereignis_speichern_alleDB
        Shared Function execute(ByVal modus As String, ByVal zielvorgangsid As Integer, ereignis As clsEreignis) As Integer
            If modus = "neu" Then
                ZahlungToolsNs.NeuesZahlungsEreigniserzeugen.execute(modus, myGlobalz.sitzung.aktZahlung.VerschicktAm)
                clsEreignisTools.NeuesEreignisSpeichern_alleDB(zielvorgangsid, "neu", myGlobalz.sitzung.aktEreignis) '      If clsEreignisDB.Neu_speichern_Ereignis() Then nachricht("Ereignis wurden gespeichert!")
                ' Dim result As Integer = ZahlungNeu_alleDB.execute()
                myGlobalz.sitzung.aktZahlung.ZahlungsID = ZahlungToolsNs.ZahlungNeu_alleDB.execute()

                nachricht("Zahlung wurde erfolgreich gespeichert!")
            End If
            If modus = "edit" Then
                ZahlungToolsNs.NeuesZahlungsEreigniserzeugen.execute(modus, myGlobalz.sitzung.aktZahlung.VerschicktAm)
                clsEreignisTools.EDITobj2DBOk_Ereignis_alledb(myGlobalz.sitzung.aktEreignis.ID, zielvorgangsid, ereignis)   'If clsEreignisDB.Edit_speichern_Ereignis(myGlobalz.sitzung.aktEreignis.ID) Then nachricht("Ereignis wurden gespeichert!")
                'clsEreignisTools.NeuesEreignisSpeichern_alleDB(zielvorgangsid, "edit", myGlobalz.sitzung.aktEreignis) '  
                ' Edit_speichern_zahlung(myGlobalz.sitzung.aktZahlung.ZahlungsID)
                nachricht(" zahlungsid% = " & ZahlungEdit_alleDB.execute(myGlobalz.sitzung.aktZahlung.ZahlungsID))
            End If
        End Function
    End Class

    Public Class leseZahlung
        Shared Sub execute(ByVal ereignisid%, ByVal vorgangsID%)
            Dim sql$
            sql$ = String.Format("select * from zahlungen where ereignisid={0} and vorgangsid={1}", ereignisid, vorgangsID)
            If myGlobalz.zahlung_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsZAHLUNGDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.zahlung_MYDB))
                zzz.getDT_zahlung(sql)
                    zzz.dispose
            End If
            If myGlobalz.zahlung_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsZAHLUNGDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.zahlung_MYDB))
                zzz.getDT_zahlung(sql)
                    zzz.dispose
            End If

        End Sub
    End Class


    Public Class alleZahlungen
        Public Shared Sub execute()
            Dim sql$ = "select zahlungsID,Sachgebietsnr,Gruppe,VorgangsID,Aktenzeichen,BearbeiterInitial,Beschreibung," + _
             "Typ,Richtung,verschicktAm,eingangAm,Betrag," + _
             "Zahler,HHST,istAnordnungbestellt,istAngeordnet from zahlungen order by verschicktAm desc"
            'Dim sql$ = "select  * from vorgang order by sachgebietnr"
            If myGlobalz.zahlung_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsZAHLUNGDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.zahlung_MYDB))
                zzz.getDT_zahlung(sql)
                    zzz.dispose
            End If
            If myGlobalz.zahlung_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsZAHLUNGDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.zahlung_MYDB))
                zzz.getDT_zahlung(sql)
                    zzz.dispose
            End If
        End Sub
    End Class

    Public Class ExcelausgabeExtracted
        Public Shared Sub execute()
            alleZahlungen.execute()
            'Dim prefix$ = "zahlungen"
            'Dim exportfile$ = myGlobalz.Paradigma_root & "\Vorgang_" & myGlobalz.sitzung.VorgangsID & "_" & prefix$ & ".xls"
            'clsExcelTools.ExcelTools.ZahlungsDatatableToExcel(myGlobalz.sitzung.tempREC.dt, exportfile, "Alle")
            'clsExcelTools.ExcelTools.DatatableToExcel(myGlobalz.sitzung.tempREC.dt, exportfile, "Alle")
            '' Dim exportfile$ = myGlobalz.Paradigma_root & "\Vorgang_" & myGlobalz.sitzung.VorgangsID & "_" & prefix$ & ".csv"
            'todo ihah   glob2.export_excelEreignisse("Zahlungen", myGlobalz.sitzung.tempREC.dt, exportfile$) 
            Dim handcsv As New clsCSVausgaben("Zahlungen", myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktVorgangsID)
            nachricht(" exportfile$ = " & handcsv.ausgeben())
            handcsv.start()
             handcsv.Dispose
        End Sub
    End Class
End Namespace
