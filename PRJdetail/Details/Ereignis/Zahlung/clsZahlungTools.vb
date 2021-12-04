#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data


Namespace ZahlungToolsNs
    Public Class zahlungDTtoOBJ

        Public Shared Function execute(ByVal rau As DataRow, ByVal aktzahl As clsZahlung) As Boolean
            Try
                With rau
                    aktzahl.clear()
                    aktzahl.SachgebietNr = clsDBtools.fieldvalue(.Item("Sachgebietsnr"))
                    aktzahl.AZ = clsDBtools.fieldvalue(.Item("Aktenzeichen"))
                    aktzahl.Initiale = clsDBtools.fieldvalue(.Item("BearbeiterInitial"))
                    aktzahl.Typ = clsDBtools.fieldvalue(.Item("Typ"))
                    aktzahl.Eingang = CBool(clsDBtools.toBool(.Item("Richtung")))
                    aktzahl.VerschicktAm = clsDBtools.fieldvalueDate(.Item("verschicktAm"))
                    aktzahl.AngeordnetAm = clsDBtools.fieldvalueDate(.Item("ANGEORDNETAM"))
                    aktzahl.EingangAm = clsDBtools.fieldvalueDate(.Item("eingangAm"))
                    aktzahl.Betrag = CDbl(clsDBtools.fieldvalue(.Item("Betrag")))
                    aktzahl.Zahler = clsDBtools.fieldvalue(.Item("Zahler"))
                    aktzahl.Notiz = clsDBtools.fieldvalue(.Item("Notiz"))
                    aktzahl.ZahlungsID = CInt(clsDBtools.fieldvalue(.Item("ZahlungsID")))
                    aktzahl.HausHaltsstelle = CStr(clsDBtools.fieldvalue(.Item("HHST")))
                    aktzahl.istAngeordnet = CBool(clsDBtools.toBool(.Item("istAngeordnet")))
                    aktzahl.istAnordnungbestellt = CBool(clsDBtools.toBool(.Item("istAnordnungbestellt")))
                    aktzahl.VorgangsNR = CInt(clsDBtools.fieldvalue(.Item("vorgangsid")))
                    aktzahl.ereignisid = CInt(clsDBtools.fieldvalue(.Item("ereignisid")))
                    Return true
                End With
            Catch ex As Exception
                nachricht_und_Mbox("FEhler: zahlungDTtoOBJ" & vbCrLf ,ex)
                Return false
            End Try
        End Function
    End Class

    Public Class NeuesZahlungsEreigniserzeugen
        Private Shared Function GetBeschreibung() As String
            Dim resu As String = String.Format("{0}: {1}",
                                    myGlobalz.sitzung.aktZahlung.Typ,
                                    myGlobalz.sitzung.aktZahlung.Betrag.ToString("c"))

            If Not myGlobalz.sitzung.aktZahlung.VerschicktAm.ToString.Contains("0001") Then
                resu = resu & ", Festg. am: " & Format(myGlobalz.sitzung.aktZahlung.VerschicktAm, "dd.MM.yyyy")
            End If
            If Not myGlobalz.sitzung.aktZahlung.AngeordnetAm.ToString.Contains("0001") Then
                resu = resu & ", Ang. am: " & Format(myGlobalz.sitzung.aktZahlung.AngeordnetAm, "dd.MM.yyyy")
            End If
            If Not myGlobalz.sitzung.aktZahlung.EingangAm.ToString.Contains("0001") Then
                resu = resu & ", Bez. am: " & Format(myGlobalz.sitzung.aktZahlung.EingangAm, "dd.MM.yyyy")
            End If
            resu = resu & getVonAn(myGlobalz.sitzung.aktEreignis.Richtung) & ": "
            resu = resu & myGlobalz.sitzung.aktZahlung.Zahler & "; "
            resu = resu & myGlobalz.sitzung.aktZahlung.HausHaltsstelle
            Return resu
        End Function
        Private Shared Function setzeDatum() As Date
            Dim datum As Date = myGlobalz.sitzung.aktZahlung.VerschicktAm
            If Not myGlobalz.sitzung.aktZahlung.VerschicktAm.ToString.Contains("0001") Then
                datum = myGlobalz.sitzung.aktZahlung.VerschicktAm
            End If
            If Not myGlobalz.sitzung.aktZahlung.AngeordnetAm.ToString.Contains("0001") Then
                datum = myGlobalz.sitzung.aktZahlung.AngeordnetAm
            End If
            If Not myGlobalz.sitzung.aktZahlung.EingangAm.ToString.Contains("0001") Then
                datum = myGlobalz.sitzung.aktZahlung.EingangAm
            End If
            Return datum
        End Function
        Public Shared Sub execute(ByVal modus As String)
            'neues ereignis erzeugen
            'myGlobalz.sitzung.aktEreignis.clearValues()
            With myGlobalz.sitzung
                .Ereignismodus = modus
                .aktEreignis.Art = "Zahlung"
                .aktEreignis.typnr = 3
                zahlungsEingang2EreignisRichtung()
                .aktEreignis.DokumentID = 0
                .aktEreignis.Beschreibung = GetBeschreibung()
                .aktEreignis.Quelle = myGlobalz.sitzung.aktBearbeiter.getInitial
                .aktEreignis.Datum = setzeDatum()
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

        Shared Sub populateParamListeZahlung(paramListe As List(Of clsSqlparam))
            If String.IsNullOrEmpty(myglobalz.sitzung.aktZahlung.HausHaltsstelle) Then myglobalz.sitzung.aktZahlung.HausHaltsstelle = ""
            paramListe.Add(New clsSqlparam("SACHGEBIETSNR", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl))
            paramListe.Add(New clsSqlparam("GRUPPE", ""))

            paramListe.Add(New clsSqlparam("VORGANGSID", CInt(myglobalz.sitzung.aktVorgangsID)))
            paramListe.Add(New clsSqlparam("EREIGNISID", CInt(myglobalz.sitzung.aktEreignis.ID)))
            paramListe.Add(New clsSqlparam("AKTENZEICHEN", myglobalz.sitzung.aktVorgang.Stammdaten.az.gesamt))
            paramListe.Add(New clsSqlparam("BEARBEITERINITIAL", myglobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale))
            paramListe.Add(New clsSqlparam("TYP", (myglobalz.sitzung.aktZahlung.Typ)))
            paramListe.Add(New clsSqlparam("RICHTUNG", CBool(myglobalz.sitzung.aktZahlung.Eingang)))


            paramListe.Add(New clsSqlparam("NOTIZ", myglobalz.sitzung.aktEreignis.Notiz))
            paramListe.Add(New clsSqlparam("BESCHREIBUNG", myglobalz.sitzung.aktEreignis.Beschreibung))
            paramListe.Add(New clsSqlparam("ZAHLER", myglobalz.sitzung.aktZahlung.Zahler))
            paramListe.Add(New clsSqlparam("BETRAG", CDbl(myglobalz.sitzung.aktZahlung.Betrag)))
            paramListe.Add(New clsSqlparam("HHST", CStr(myglobalz.sitzung.aktZahlung.HausHaltsstelle)))
            paramListe.Add(New clsSqlparam("ISTANORDNUNGBESTELLT", CBool(myglobalz.sitzung.aktZahlung.istAnordnungbestellt)))
            paramListe.Add(New clsSqlparam("ISTANGEORDNET", CBool(myglobalz.sitzung.aktZahlung.istAngeordnet)))

            paramListe.Add(New clsSqlparam("VERSCHICKTAM", clsDBtools.makedateMssqlConform(myglobalz.sitzung.aktZahlung.VerschicktAm, myglobalz.sitzung.VorgangREC.mydb.dbtyp)
                                                     ))


            paramListe.Add(New clsSqlparam("ANGEORDNETAM", clsDBtools.makedateMssqlConform(myglobalz.sitzung.aktZahlung.AngeordnetAm, myglobalz.sitzung.VorgangREC.mydb.dbtyp)))
            paramListe.Add(New clsSqlparam("EINGANGAM", clsDBtools.makedateMssqlConform(myglobalz.sitzung.aktZahlung.EingangAm, myglobalz.sitzung.VorgangREC.mydb.dbtyp)))
        End Sub

        Public Shared Function execute() As Integer
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            populateParamListeZahlung(clsSqlparam.paramListe)
            querie = "INSERT INTO " & CLstart.myViewsNTabs.tabZahlungen & " (SACHGEBIETSNR,GRUPPE,VORGANGSID,EREIGNISID,AKTENZEICHEN,BEARBEITERINITIAL," +
                                            "TYP,RICHTUNG,VERSCHICKTAM,ANGEORDNETAM,EINGANGAM,NOTIZ,BESCHREIBUNG," +
                                            "ZAHLER,BETRAG,HHST,ISTANORDNUNGBESTELLT,ISTANGEORDNET) " +
                                        " VALUES (@SACHGEBIETSNR,@GRUPPE,@VORGANGSID,@EREIGNISID,@AKTENZEICHEN,@BEARBEITERINITIAL, " +
                                        "@TYP,@RICHTUNG,@VERSCHICKTAM,@ANGEORDNETAM,@EINGANGAM,@NOTIZ,@BESCHREIBUNG, " +
                                        "@ZAHLER,@BETRAG,@HHST,@ISTANORDNUNGBESTELLT,@ISTANGEORDNET)"
            myGlobalz.sitzung.aktZahlung.ZahlungsID = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ZAHLUNGSID")
            nachricht("aktZahlung ID:" & myglobalz.sitzung.aktZahlung.ZahlungsID)
            If myglobalz.sitzung.aktZahlung.ZahlungsID > 0 Then
                nachricht("Neues Ereigniss wurde gespeichert!" & myglobalz.sitzung.aktZahlung.ZahlungsID)
            End If
            Return myglobalz.sitzung.aktZahlung.ZahlungsID
        End Function
    End Class

    Public Class ZahlungEdit_alleDB
        Public Shared Function execute(ByVal zid As Integer) As Integer 'myGlobalz.sitzung.aktZahlung.ZahlungsID
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            ZahlungNeu_alleDB.populateParamListeZahlung(clsSqlparam.paramListe)
            querie = "update " & CLstart.myViewsNTabs.tabZahlungen & "  Set " &
                             " SACHGEBIETSNR=@SACHGEBIETSNR" &
                             ", GRUPPE =@GRUPPE" &
                             ", VORGANGSID =@VORGANGSID " &
                             ", EREIGNISID =@EREIGNISID " &
                             ", AKTENZEICHEN =@AKTENZEICHEN " &
                             ", BEARBEITERINITIAL =@BEARBEITERINITIAL " &
                             ", TYP =@TYP " &
                             ", RICHTUNG =@RICHTUNG " &
                             ", VERSCHICKTAM =@VERSCHICKTAM " &
                             ", ANGEORDNETAM =@ANGEORDNETAM " &
                             ", EINGANGAM =@EINGANGAM " &
                             ", NOTIZ =@NOTIZ " &
                             ", BESCHREIBUNG =@BESCHREIBUNG " &
                             ", ZAHLER =@ZAHLER " &
                             ", BETRAG =@BETRAG " &
                             ", HHST =@HHST " &
                             ", ISTANORDNUNGBESTELLT =@ISTANORDNUNGBESTELLT" &
                             ", ISTANGEORDNET =@ISTANGEORDNET " &
         "where zahlungsID=@zahlungsID"
            clsSqlparam.paramListe.Add(New clsSqlparam("ZAHLUNGSID", zid))
            Dim erfolg As Integer = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ZAHLUNGSID")
            nachricht("aktZahlung ID:" & myGlobalz.sitzung.aktZahlung.ZahlungsID)
            If erfolg > 0 Then
                nachricht("Neues Ereigniss wurde gespeichert!" & myGlobalz.sitzung.aktZahlung.ZahlungsID)
            End If
            Return 1
        End Function
    End Class


    Public Class Zahlung_loeschen_alledb 'myGlobalz.sitzung.aktZahlung.ZahlungsID
        Public Shared Function execute(ByVal zid As Integer) As Integer 'myGlobalz.sitzung.aktZahlung.ZahlungsID
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL =
             String.Format("delete from " & CLstart.myViewsNTabs.tabZahlungen & "  where zahlungsid=" & zid)
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function
    End Class




    Public Class zahlung_und_ereignis_speichern_alleDB
        Shared Function execute(ByVal modus As String, ByVal zielvorgangsid As Integer, ereignis As clsEreignis) As Integer
            If modus = "neu" Then
                ZahlungToolsNs.NeuesZahlungsEreigniserzeugen.execute(modus)
                clsEreignisTools.NeuesEreignisSpeichern_alleDB(zielvorgangsid, "neu", myGlobalz.sitzung.aktEreignis) '      If clsEreignisDB.Neu_speichern_Ereignis() Then nachricht("Ereignis wurden gespeichert!")
                ' Dim result As Integer = ZahlungNeu_alleDB.execute()
                myGlobalz.sitzung.aktZahlung.ZahlungsID = ZahlungToolsNs.ZahlungNeu_alleDB.execute()

                nachricht("Zahlung wurde erfolgreich gespeichert!")
                Return myGlobalz.sitzung.aktZahlung.ZahlungsID
            End If
            If modus = "edit" Then
                ZahlungToolsNs.NeuesZahlungsEreigniserzeugen.execute(modus)
                clsEreignisTools.EDITobj2DBOk_Ereignis_alledb(myGlobalz.sitzung.aktEreignis.ID, zielvorgangsid, ereignis)   'If clsEreignisDB.Edit_speichern_Ereignis(myGlobalz.sitzung.aktEreignis.ID) Then nachricht("Ereignis wurden gespeichert!")
                'clsEreignisTools.NeuesEreignisSpeichern_alleDB(zielvorgangsid, "edit", myGlobalz.sitzung.aktEreignis) '  
                ' Edit_speichern_zahlung(myGlobalz.sitzung.aktZahlung.ZahlungsID)
                nachricht(" zahlungsid% = " & ZahlungEdit_alleDB.execute(myGlobalz.sitzung.aktZahlung.ZahlungsID))
                 Return myGlobalz.sitzung.aktZahlung.ZahlungsID
            End If
            Return 0
        End Function
    End Class

    Public Class leseZahlung
        Shared Sub execute(ByVal ereignisid%, ByVal vorgangsID%)
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = String.Format("select * from [Paradigma].[dbo]." & CLstart.myViewsNTabs.TABZAHLUNGEN & "  where ereignisid={0} and vorgangsid={1}", ereignisid, vorgangsID)
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.tempREC, hinweis)
        End Sub
    End Class


    Public Class alleZahlungen
        Public Shared Sub execute()
            Dim hinweis As String = ""
            Dim sql$ = "select zahlungsID,Sachgebietsnr,Gruppe,VorgangsID,Aktenzeichen,BearbeiterInitial,Beschreibung," +
             "Typ,Richtung,verschicktAm,eingangAm,Betrag," +
             "Zahler,HHST,istAnordnungbestellt,istAngeordnet from " & CLstart.myViewsNTabs.tabZahlungen & "  order by verschicktAm desc"
            myGlobalz.sitzung.tempREC.dt = getDT4Query(sql, myglobalz.sitzung.tempREC, hinweis)


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
            Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Zahlungen", myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktVorgangsID, "", CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
            nachricht(" exportfile$ = " & handcsv.CscDateiAusgeben())
            handcsv.start()
            handcsv.Dispose()
        End Sub
    End Class
End Namespace
