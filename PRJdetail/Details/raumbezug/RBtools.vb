Imports System.Data

Namespace RBtoolsns
    Public Class statusSpalteErgaenzenUndFuellen
        Public Shared Function execute(ByVal ZielTable As DataTable,
                                       ByVal KoppelungDT As DataTable,
                                       NeuSpaltenname As String,
                                       kopplelungsIDspalte As String) As Boolean

            VerwandtschaftsStatusSpalteErgaenzenUndMitStandardFuellen(ZielTable, NeuSpaltenname, 0)
            RBtoolsns.StatusSpalteFuellen.execute(ZielTable, KoppelungDT, NeuSpaltenname, kopplelungsIDspalte)
            Return True
        End Function

        Public Shared Sub VerwandtschaftsStatusSpalteErgaenzenUndMitStandardFuellen(ByVal ZielTable As DataTable,
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
                If ZielTable.IsNothingOrEmpty Then
                    Return False
                End If
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
                nachricht("fehler in StatusSpalteFuellen: " & NeuSpaltenname, ex)
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
        ''' <summary>
        ''' berechnet den aktrange aus dem raumbezugsrange
        ''' </summary>
        ''' <param name="maxRange"></param>
        ''' <param name="dietabelleDT"></param>
        Public Shared Sub execute(ByRef maxRange As clsRange, ByVal dietabelleDT As DataTable)     'myGlobalz.sitzung.raumbezugsRec.dt
            nachricht("berechneRaumbezugsrange ----------------------------")
            Dim KonstanterZuschlagX As Integer = 20
            Dim KonstanterZuschlagY As Integer = 20
            Dim aktRange As New clsRange
            Try
                If dietabelleDT.IsNothingOrEmpty Then
                    nachricht("-warnung in berechneRaumbezugsrange a: die tabelle ist nothing or empty. Abbruch ")
                    Exit Sub
                End If
                ' Dim maxRange As New clsRange
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
                nachricht("Fehler inberechneRaumbezugsrange " ,ex)
                nachricht("Fehler inberechneRaumbezugsrange tabname:" & dietabelleDT.TableName)
            End Try
        End Sub
    End Class

    'Public Class FotoNeuSpeichern_alleDB


    '    Private Shared Sub populateParafoto(aktJPG As clsMyJPG)
    '        clsSqlparam.paramListe.Add(New clsSqlparam("GKRECHTS", aktJPG.rechts.ToString.Replace(",", ".")))
    '        clsSqlparam.paramListe.Add(New clsSqlparam("GKHOCH", aktJPG.hoch.ToString.Replace(",", ".")))
    '        clsSqlparam.paramListe.Add(New clsSqlparam("GPSLONGITUDE", aktJPG.Exifgpslongitude))
    '        clsSqlparam.paramListe.Add(New clsSqlparam("GPSLATITUDE", aktJPG.Exifgpslatitude))
    '        clsSqlparam.paramListe.Add(New clsSqlparam("GPSDIR", aktJPG.ExifGpsImgDir))
    '        clsSqlparam.paramListe.Add(New clsSqlparam("UTMRECHTS", "0".ToString.Replace(",", ".")))
    '        clsSqlparam.paramListe.Add(New clsSqlparam("UTMHOCH", "0".ToString.Replace(",", ".")))
    '        clsSqlparam.paramListe.Add(New clsSqlparam("EXIFDATUMORIGINAL",
    '                                                                             clsDBtools.makedateMssqlConform(aktJPG.EXIFDateTimeOriginal, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))

    '        clsSqlparam.paramListe.Add(New clsSqlparam("DOKUMENTID", aktJPG.DokumentID))
    '    End Sub
    'End Class

    Public Class Raumbezug_abspeichern_Neu_alleDB
        Public Shared Function execute(ByVal fotto As iRaumbezug) As Integer ' myGlobalz.sitzung.aktParaFoto
            Dim raumbezugsID As Integer
            Dim querie As String
            '  werteDBsicherMachenEreignis(ereignis)
            clsSqlparam.paramListe.Clear()
            '   populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
            'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "INSERT INTO " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  (TYP,SEKID,TITEL,ABSTRACT,RECHTS,HOCH," &
                                      " XMIN,XMAX,YMIN,YMAX,FREITEXT,ISMAPENABLED,FLAECHEQM,LAENGEM,MITETIKETT) " +
                                      " VALUES (@TYP,@SEKID,@TITEL,@ABSTRACT,@RECHTS,@HOCH," &
                                      "@XMIN,@XMAX,@YMIN,@YMAX,@FREITEXT,@ISMAPENABLED,@FLAECHEQM,@LAENGEM,@MITETIKETT)"
            populateRB(fotto)
            raumbezugsID = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "RAUMBEZUGSID")

            Return raumbezugsID
            ' genMapServerEbene.exe()
        End Function

        Shared Sub populateRB(fotto As iRaumbezug)
            With fotto
                clsSqlparam.paramListe.Add(New clsSqlparam("TYP", .typ))
                clsSqlparam.paramListe.Add(New clsSqlparam("SEKID", .SekID))
                clsSqlparam.paramListe.Add(New clsSqlparam("TITEL", .name.Trim))
                clsSqlparam.paramListe.Add(New clsSqlparam("ABSTRACT", .abstract.Trim))
                clsSqlparam.paramListe.Add(New clsSqlparam("RECHTS", CInt(.punkt.X)))
                clsSqlparam.paramListe.Add(New clsSqlparam("HOCH", CInt(.punkt.Y)))
                clsSqlparam.paramListe.Add(New clsSqlparam("XMIN", CInt(.box.xl)))
                clsSqlparam.paramListe.Add(New clsSqlparam("XMAX", CInt(.box.xh)))
                clsSqlparam.paramListe.Add(New clsSqlparam("YMIN", CInt(.box.yl)))
                clsSqlparam.paramListe.Add(New clsSqlparam("YMAX", CInt(.box.yh)))
                clsSqlparam.paramListe.Add(New clsSqlparam("FREITEXT", CStr(.Freitext).Trim))
                clsSqlparam.paramListe.Add(New clsSqlparam("ISMAPENABLED", Convert.ToInt16(.isMapEnabled)))
                clsSqlparam.paramListe.Add(New clsSqlparam("FLAECHEQM", CInt(.FLAECHEQM)))
                clsSqlparam.paramListe.Add(New clsSqlparam("LAENGEM", CInt(.LAENGEM)))
                clsSqlparam.paramListe.Add(New clsSqlparam("MITETIKETT", CInt(.MITETIKETT)))
            End With
        End Sub
    End Class

    Public Class AdresseNeuSpeichern_alleDB
        Public Shared Function execute() As Integer
            Dim parafotoID As Integer
            Dim querie As String
            werteDBsicherMachenAdresse()
            clsSqlparam.paramListe.Clear()
            '   populateParamListeEreignis()
            '  clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "INSERT INTO " & CLstart.myViewsNTabs.tabPARAADRESSE & " (GEMEINDENR,GEMEINDETEXT,STRASSENNAME,STRCODE,FS,HAUSNRKOMBI,PLZ,POSTFACH,ADRESSTYP) " +
                                  " VALUES (@GEMEINDENR,@GEMEINDETEXT,@STRASSENNAME,@STRCODE,@FS,@HAUSNRKOMBI,@PLZ,@POSTFACH,@ADRESSTYP)"
            populateAdresse()
            parafotoID = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")

            Return parafotoID
        End Function

        Shared Sub populateAdresse()
            clsSqlparam.paramListe.Add(New clsSqlparam("GEMEINDENR", myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig()))
            clsSqlparam.paramListe.Add(New clsSqlparam("GEMEINDETEXT", myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("STRASSENNAME", myGlobalz.sitzung.aktADR.Gisadresse.strasseName.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("STRCODE", myGlobalz.sitzung.aktADR.Gisadresse.strasseCode))
            clsSqlparam.paramListe.Add(New clsSqlparam("FS", myGlobalz.sitzung.aktADR.FS))
            clsSqlparam.paramListe.Add(New clsSqlparam("HAUSNRKOMBI", myGlobalz.sitzung.aktADR.Gisadresse.HausKombi))
            clsSqlparam.paramListe.Add(New clsSqlparam("PLZ", CInt(myGlobalz.sitzung.aktADR.PLZ)))
            clsSqlparam.paramListe.Add(New clsSqlparam("POSTFACH", myGlobalz.sitzung.aktADR.Postfach))
            clsSqlparam.paramListe.Add(New clsSqlparam("ADRESSTYP", CInt(myGlobalz.sitzung.aktADR.Adresstyp)))
        End Sub

        Shared Sub werteDBsicherMachenAdresse()
            If myGlobalz.sitzung.aktADR.PLZ.IsNothingOrEmpty Then
                myGlobalz.sitzung.aktADR.PLZ = "0"
            End If
        End Sub
    End Class

    Public Class AdresseEdit_alleDB
        Public Shared Function execute(ByVal sekid As Integer) As Integer
            Dim parafotoID As Integer
            Dim querie As String
            AdresseNeuSpeichern_alleDB.werteDBsicherMachenAdresse()
            clsSqlparam.paramListe.Clear()
            '   populateParamListeEreignis()
            '  clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "update " & CLstart.myViewsNTabs.tabPARAADRESSE & "  " & " SET " &
                            " GEMEINDENR=@GEMEINDENR" &
                            ",GEMEINDETEXT=@GEMEINDETEXT" &
                            ",STRASSENNAME=@STRASSENNAME" &
                            ",STRCODE=@STRCODE" &
                            ",FS=@FS" &
                            ",HAUSNRKOMBI=@HAUSNRKOMBI" &
                            ",PLZ=@PLZ" &
                            ",POSTFACH=@POSTFACH" &
                            ",ADRESSTYP=@ADRESSTYP" &
                            " where id=@ID"
            AdresseNeuSpeichern_alleDB.populateAdresse()

            clsSqlparam.paramListe.Add(New clsSqlparam("ID", sekid))
            parafotoID = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")

            Return parafotoID
        End Function
    End Class
    Public Class AdresseLoeschen_alleDB
        Public Shared Function execute(ByVal sekid As Integer) As Integer
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = String.Format("delete from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where id={0}", sekid)
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function
    End Class
    Public Class RBFotoLoeschen_alleDB
        Public Shared Function execute(ByVal docid As Integer) As Integer
            Dim parafotoID% : Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabPARAFOTO & "   where dokumentid=" & docid.ToString
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return parafotoID%
        End Function
    End Class

    Public Class Raumbezug_edit_alleDB
        Public Shared Function execute(ByVal rid As Integer, ByVal aktRb As iRaumbezug) As Integer ' myGlobalz.sitzung.aktParaFoto
            Dim anzahlTreffer As Integer
            Dim querie As String
            Try
                l(" Raumbezug_edit_alleDB ---------------------- anfang")
                clsSqlparam.paramListe.Clear()
                querie = "UPDATE  " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  " & " SET " &
                                    " TYP=@TYP" &
                                    ",SEKID=@SEKID" &
                                    ",TITEL=@TITEL" &
                                    ",ABSTRACT=@ABSTRACT" &
                                    ",RECHTS=@RECHTS" &
                                    ",HOCH=@HOCH" &
                                    ",XMIN=@XMIN" &
                                    ",XMAX=@XMAX" &
                                    ",YMIN=@YMIN" &
                                    ",YMAX=@YMAX" &
                                    ",FREITEXT=@FREITEXT" &
                                    ",ISMAPENABLED=@ISMAPENABLED" &
                                    ",FLAECHEQM=@FLAECHEQM" &
                                    ",LAENGEM=@LAENGEM" &
                                    ",MITETIKETT=@MITETIKETT" &
                                    " WHERE RAUMBEZUGSID=@RAUMBEZUGSID"
                Raumbezug_abspeichern_Neu_alleDB.populateRB(aktRb)
            clsSqlparam.paramListe.Add(New clsSqlparam("RAUMBEZUGSID", rid))
            anzahlTreffer = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "RAUMBEZUGSID")
                If anzahlTreffer < 1 Then
                    nachricht_und_Mbox("Problem beim Abspeichern in Raumbezug_edit_alleDB:" & querie & ", rid: " & rid)
                    Return -1
                Else
                    Return CInt(anzahlTreffer)
                End If
                l(" Raumbezug_edit_alleDB ---------------------- ende")
            Catch ex As Exception
                l("Fehler in Raumbezug_edit_alleDB: " & querie & " rid:  " & rid & "  " ,ex)
                Return -2
            End Try
        End Function
    End Class

    Public Class Raumbezug_loeschen_byid_alleDB
        Public Shared Function execute(ByVal rid As Integer) As Integer ' myGlobalz.sitzung.aktParaFoto
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  where raumbezugsid=" & rid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function
    End Class

    Public Class Raumbezug_loeschen_bySEKid_alleDB
        Public Shared Function execute(ByVal rid As Integer, ByVal doktyp As String) As Integer ' myGlobalz.sitzung.aktParaFoto
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  where sekid=" & rid & " and typ=" & doktyp.ToString
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function
    End Class

    Public Class Koppelung_Raumbezug_Vorgang_alleDB
        Public Shared Function execute(ByVal rid As Integer, ByVal vorgangsid As Integer, ByVal status As Integer) As Integer ' myGlobalz.sitzung.aktParaFoto
            Dim koppelungsid As Integer
            Dim querie As String
            '  werteDBsicherMachenEreignis(ereignis)
            clsSqlparam.paramListe.Clear()
            '   populateParamListeEreignis()
            'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "INSERT INTO " & CLstart.myViewsNTabs.tabRaumbezug2vorgang & "    " &
                  " (RAUMBEZUGSID,VORGANGSID,STATUS) VALUES (@RAUMBEZUGSID,@vorgangsid,@status) "

            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", vorgangsid))
            clsSqlparam.paramListe.Add(New clsSqlparam("RAUMBEZUGSID", rid))
            clsSqlparam.paramListe.Add(New clsSqlparam("STATUS", status))

            koppelungsid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
            Return koppelungsid
        End Function
    End Class

    Public Class initraumbezugsDT_alleDB
        Public Shared Function exe(ByVal vid As Integer) As Boolean
            Dim erfolg As Boolean
            Dim SQL As String = "" : Dim hinweis As String = ""
            SQL = "select * from [Paradigma].[dbo].raumbezugplus where vorgangsid=" & vid & " order by abstract"
            nachricht("sql: " & SQL)
            myGlobalz.sitzung.raumbezugsRec.dt = getDT4Query(SQL, myGlobalz.sitzung.raumbezugsRec, hinweis)
            nachricht("Es konnten  Raumbezuege zu diesem Vorgang gefunden werden!")
            Return True
        End Function
    End Class

    Public Class RB_Adresse_holen_by_ID_alleDB
        Public Shared Function exe(ByVal sekid As Integer) As Boolean
            Dim hinweis As String = ""
            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where id=" & sekid.ToString
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
            Return True
        End Function
    End Class

    Public Class Entkoppelung_Raumbezug_Vorgang_alleDB
        Public Shared Function exe(ByVal RaumbezugsID%, ByVal VorgangsID As Integer) As Integer
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabRaumbezug2vorgang & "   " &
                 " where  RaumbezugsID=" & RaumbezugsID &
                 " and VorgangsID=" & VorgangsID
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)

            Return 1
        End Function
    End Class

    Public Class RB_Flurstueck_holen_alleDB
        Public Shared Function exe(ByVal sekid As String) As Integer
            Dim hinweis As String = ""
            Try
                l(" RB_Flurstueck_holen_alleDB ---------------------- anfang")
                myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & "  where id=" & sekid
                myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
                l(" RB_Flurstueck_holen_alleDB ---------------------- ende")
                Return 1
            Catch ex As Exception
                l("Fehler in RB_Flurstueck_holen_alleDB: " ,ex)
                Return -1
            End Try
        End Function
    End Class

    Public Class KopierenVonRaumbezuegen_alleDB
        Shared Function viaKopplung_VorgangID_zu_RaumbezuegeID(ByVal vorgangsid As String) As Boolean
            Dim hinweis As String = ""
            myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabRaumbezug2vorgang & "    where VorgangsID=" & vorgangsid
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
            l(hinweis)
            If myGlobalz.sitzung.tempREC.mycount < 1 Then
                nachricht("Keine Ereignisse gespeichert d!")
                Return False
            Else
                nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
                Return True
            End If
        End Function
        Public Shared Function exe(ByVal quellVid%, ByVal vid As Integer) As Boolean

            Dim RBLinksholen_erfolgreich As Boolean = viaKopplung_VorgangID_zu_RaumbezuegeID(quellVid.ToString) ' nach myGlobalz.sitzung.tempREC
            nachricht("RBLinksholen_erfolgreich: " & RBLinksholen_erfolgreich)
            If RBLinksholen_erfolgreich Then
                If myGlobalz.sitzung.tempREC.mycount > 0 Then
                    nachricht("Es wird kopiert")
                    RBKopieren(myGlobalz.sitzung.tempREC.dt, vid)
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
        Shared Function alleRaumbezuegeAUfDTlesen(ByVal rid_liste As DataTable, ByVal iDB_grundfunktionen As IDB_grundfunktionen) As Boolean
            Dim Instring As String = ""
            Instring = clsDBtools.bildINstringSpaltenname(rid_liste, "raumbezugsid")
            iDB_grundfunktionen.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabRAUMBEZUG & "  where raumbezugsid IN(" & Instring & ")"
            Dim hinerweis As String = iDB_grundfunktionen.getDataDT()
            If Not iDB_grundfunktionen.dt.IsNothingOrEmpty Then
                Return True
            Else
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
                                    myGlobalz.sitzung.aktPolygon.GKstring = polygonparser.gkstringausserial_generieren(myGlobalz.sitzung.aktPolygon.ShapeSerial)
                                    'Dim a = polygonparser.gkstringsausserial_generieren(myGlobalz.sitzung.aktPolygon.ShapeSerial)
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
                                    myGlobalz.sitzung.aktPolygon.GKstring = polygonparser.gkstringausserial_generieren(myGlobalz.sitzung.aktPolygon.ShapeSerial)
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
                    Return False
                End If





            Catch ex As Exception
                nachricht_und_Mbox("Fehler bei RBKopieren: " ,ex)
                nachricht_und_Mbox("Fehler bei RBKopieren: " & aktRID%)
                Return True
            End Try
        End Function


    End Class


    Public Class getCoords4ID_Raumbezug_alleDB
        Public Shared Function exe(ByVal id As Integer) As myPoint ' quellVid, myGlobalz.sitzung.VorgangsID
            Dim erfolg As New myPoint With {.X = 0, .Y = 0} : Dim hinweis As String = ""
            myGlobalz.sitzung.tempREC.mydb.SQL = "select rechts,hoch from {0} where id=" & id
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                erfolg.X = 0
                erfolg.Y = 0
            Else
                erfolg.X = CType(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0)), Double)
                erfolg.Y = CType(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(1)), Double)
            End If
            Return erfolg
        End Function
    End Class


    Public Class RB_ParaFoto_holen_alleDB
        Public Shared Function exe(ByVal sekid As String) As Boolean ' quellVid, myGlobalz.sitzung.VorgangsID
            Dim hinweis As String = ""
            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabPARAFOTO & "   where id=" & sekid
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
            Return True
        End Function
    End Class

    Public Class einzelDokument_holen_alleDB
        Public Shared Function exe(ByVal dokid As String) As Boolean 'einzelDokument_holen(myGlobalz.sitzung.aktDokument.DocID.ToString)
            Dim hinweis As String = ""
            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from {0} where dokumentid=" & dokid
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
            Return True
        End Function
    End Class

    Public Class RB_Flurstueck_Serial_loeschen
        Public Shared Function exe(ByVal rbid As Integer) As Integer 'myGlobalz.sitzung.aktPolygon.RaumbezugsID
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabRaumbezug2geopolygon & "    " &
             " where raumbezugsid=" & rbid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function
    End Class
    Public Class genMapServerEbene
        Shared Sub exe()
            LocalParameterFiles.erzeugeParameterDateiAktvorgang_txt(False, False)
            'Dim up$ = System.Environment.GetEnvironmentVariable("USERPROFILE")
            'Dim datei$ = up$ & "\Startmenü\Programme\Dr. Jörg Feinen\Paradigma-Karte\karte.appref-ms"
            glob2.neueKarteerstellen()
            '  System.Threading.Thread.Sleep(2000)
            ' LocalParameterFiles.erzeugeParameterDatei(False, True)
        End Sub

    End Class

    Public Class getRaumbezugsCoords_2dokument_alledb
        Public Shared Function exe(ByVal dokID As Integer) As myPoint ' myGlobalz.sitzung.aktDokument.DocID
            Dim erfolg As New myPoint With {.X = 0, .Y = 0}
            Dim hinweis As String = ""
            myGlobalz.sitzung.tempREC.mydb.SQL = "select gkrechts,gkhoch from " & CLstart.myViewsNTabs.tabPARAFOTO & "   where dokumentid=" & dokID
            myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
            If myGlobalz.sitzung.tempREC.mycount < 1 Then
                nachricht("Es wurden keine Koordinaten zum Dokument gefunden!")
                erfolg.X = 0
                erfolg.Y = 0
            Else
                erfolg.X = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("gkrechts")))
                erfolg.Y = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("gkhoch")))
            End If
            Return erfolg
        End Function
    End Class

    Class raumbezugsDT2Obj
        Private Sub New()
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
