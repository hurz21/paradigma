﻿#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data
Imports System.Data.Common
Namespace bestandTools
    Public Class vorgangidListe
        Public Shared Function fuelleFaelligeMitAllenVorgaengen(ByVal meinDT As DataTable, ByRef faelligeWV As Integer()) As String
            nachricht("HoleFaelligeWiedervorlagen ---------------")
            If meinDT Is Nothing Then
                nachricht("Fehler in HoleFaelligeWiedervorlagen ---------------: die vorgangstabelle ist leer!")
                Return ""
            End If
            ReDim faelligeWV(meinDT.Rows.Count)
            Dim icount As Integer = 0

            Dim summe As String
            Dim sb As New Text.StringBuilder
            Try
                bildeSummeStringUndArray(meinDT, faelligeWV, icount, sb)
                summe = sb.ToString
                KorrigiereSummeString(summe)
                Return summe
            Catch ex As Exception
                nachricht("Fehler in HoleFaelligeWiedervorlagen ---------------" ,ex)
                Return ""
            End Try
        End Function

        Private Shared Sub bildeSummeStringUndArray(ByVal meinDT As DataTable,
                                                    ByRef faelligeWV As Integer(),
                                                    ByRef icount As Integer,
                                                    ByVal sb As Text.StringBuilder)
            For Each zeile As DataRow In meinDT.Rows
                sb.Append(zeile.Item("VORGANGSID").ToString & ",")
                faelligeWV(icount) = CInt(zeile.Item("VORGANGSID"))
                icount += 1
            Next
        End Sub

        Private Shared Sub KorrigiereSummeString(ByRef summe$)
            If String.IsNullOrEmpty(summe) Then
                summe = summe.Substring(0, summe.Length - 1)
            Else
                summe = summe.Substring(0, summe.Length - 1)
            End If
            summe = summe.Trim
        End Sub
    End Class



    Public Class holeWVfuerVorgangsids
        Public Shared Function exe(ByVal dbREC As IDB_grundfunktionen, ByVal dbzugriff As clsDatenbankZugriff) As Boolean
            Dim datumstring$ = ""
            nachricht(String.Format("holeWVfuerVorgangsids: ---------------------------"))
            Try

                datumstring = clsDBtools.makeDBdatumsString(Now, dbzugriff.dbtyp)

                nachricht("getWiedervorlageAkut: dbtyp, dtumsformat: " & dbREC.mydb.dbtyp & datumstring)
                dbREC.mydb.SQL =
                String.Format(" SELECT distinct w.VorgangsID " +
                              " FROM " & CLstart.myViewsNTabs.TABWV & "  w , " & CLstart.myViewsNTabs.TABSTAMMDATEN & " s " +
                              " where s.VorgangsID IN (select  distinct VorgangsID from (" & CLstart.myViewsNTabs.view_vsk_d & ")   ) " &
                              " and s.VorgangsID=w.vorgangsid and w.datum < {0} and w.erledigt < 1 " &
                              " order by w.vorgangsid ",
                              datumstring)



                nachricht("Hinweis: " & dbREC.getDataDT())

                nachricht(String.Format("Akute Wiedervorlagen: {0}{1}", vbCrLf, dbREC.mydb.SQL))
                Return True
            Catch ex As Exception
                nachricht(String.Format("Fehler beim holeWVfuerVorgangsids: {0}{1}", vbCrLf, ex))
                Return False
            End Try
        End Function


        Public Shared Sub korrigiereFaelligenArray(ByRef faelligeWV As Integer(), ByRef lokdt As DataTable) 'myGlobalz.sitzung.DBWiedervorlageREC.dt
            Dim isFaellig% = 0, istFaellig As Boolean, vid% = 0
            For i = 0 To faelligeWV.Length - 1
                istFaellig = False
                vid = faelligeWV(i)
                If Not lokdt.IsNothingOrEmpty Then
                    For Each izeile As DataRow In lokdt.Rows
                        isFaellig = CInt(izeile.Item(0))
                        If isFaellig = vid Then
                            istFaellig = True
                            Continue For
                        End If
                    Next
                End If
                setzeWV_auf_0(faelligeWV, istFaellig, i)
            Next
            nachricht("Akute Wiedervorlagen: " & lokdt.Rows.Count)
        End Sub

        Private Shared Sub setzeWV_auf_0(ByRef faelligeWV As Integer(), ByVal istFaellig As Boolean, ByVal i As Integer)
            If Not istFaellig Then
                faelligeWV(i) = 0
            Else
                faelligeWV(i) = 1
            End If
        End Sub
    End Class

    Public Class setzeWVfaelligTag
        Public Shared Sub istFealligMarkierenAlsEins(ByVal faelligeWV As Integer(), ByVal lokdat As DataTable) 'myGlobalz.sitzung.VorgangREC.dt
            Array.Sort(faelligeWV)
            Dim test% = 0, iiindex% = 0
            For Each drrr As DataRow In lokdat.Rows
                test = CInt(drrr.Item("vorgangsid"))
                iiindex% = 0
                iiindex = Array.BinarySearch(faelligeWV, test)
                If iiindex > -1 Then
                    drrr.Item("WVFAELLIG") = 1
                End If
            Next
        End Sub
    End Class

    Public Class btnBestandtoExcel_ClickExtracted
        Public Shared Sub exe()
            Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Vorgaenge", myGlobalz.sitzung.VorgangREC.dt, myGlobalz.sitzung.aktVorgangsID, "", CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
            nachricht(" exportfile$ = " & handcsv.CscDateiAusgeben())
            handcsv.start()
            handcsv.Dispose()
        End Sub
    End Class

    Class zeigeVorgaenge
        Private Sub New()

        End Sub
        'Public Shared Sub exe()
        '    Dim result As Long = initVorgaengeDatatable("")
        '    If result < 1 Then
        '        If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
        '            nachricht("Es wurden keine Vorgänge in der DB gefunden")
        '        End If
        '        Exit Sub
        '    End If
        'End Sub

        'Public Shared Function initVorgaengeDatatable(ByRef hinweis As String) As Long
        '    Try
        '        hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
        '        Dim retval As Long = myGlobalz.sitzung.VorgangREC.mycount
        '        nachricht("initVorgaengeDatatable Treffer: " & retval)
        '        Return retval
        '    Catch ex As Exception
        '        nachricht("initVorgaengeDatatable: " ,ex)
        '        Return -1
        '    End Try
        'End Function

        Public Shared Function VorgaengeAnzeigenFuerHausnummerExtracted() As Boolean
            Dim resultdt As New System.Data.DataTable
            Dim erfolg As Integer
            Dim instring As String
            Try
                'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"

                myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Adresse
                erstelleSQLfuerhausnr2sekid_alledb() 'sql wird erstellt       
                erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
                If erfolg < 1 Then
                    Return False
                End If
                myGlobalz.sitzung.VorgangREC.dt = resultdt.Copy
                'myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="PA_SEKID2VID"
                instring = clsDBtools.bildINstring(resultdt, 0)
                myGlobalz.sitzung.VorgangREC.mydb.SQL = "select vorgangsid from PA_SEKID2VID " &
                    " where SEKID IN(" & instring & ")"
                erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
                myGlobalz.sitzung.tempREC.dt = resultdt.Copy
                instring = clsDBtools.bildINstring(resultdt, 0)

                myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from (" & CLstart.myViewsNTabs.view_vsk_d & ") dummy where  vorgangsid IN (" & instring & ") "
                erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
                myGlobalz.sitzung.VorgangREC.dt = resultdt.Copy
                Return True
            Catch ex As Exception
                nachricht("VorgaengeAnzeigenFuerHausnummerExtracted : Keine Adressen gefunden bzw ein Fehl ist aufgetreten: " ,ex)
                Return False
            End Try
        End Function


        Private Shared Sub erstelleSQLfuerhausnr2sekid_alledb()
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Or myGlobalz.vorgang_MYDB.dbtyp = "sqls" Then
                myGlobalz.sitzung.tempREC.mydb.SQL =
             String.Format("select distinct id,hausnrkombi  from " & CLstart.myViewsNTabs.tabPARAADRESSE &
             "  where gemeindenr = {0} and RTRIM(LTRIM(lower(Strassenname))) = '{1}' and RTRIM(LTRIM(lower(hausnrkombi))) = '{2}' order by (hausnrkombi)",
                    myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower, myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.ToLower)
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                myGlobalz.sitzung.tempREC.mydb.SQL =
             String.Format("select distinct id,hausnrkombi  from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} and trim(lower(Strassenname)) = '{1}' and trim(lower(hausnrkombi)) = '{2}' " &
                           "order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                           myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower.Trim,
                            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.ToLower.Trim)
            End If
        End Sub

    End Class

End Namespace

