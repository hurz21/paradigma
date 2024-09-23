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
                nachricht("Fehler in HoleFaelligeWiedervorlagen ---------------" & ex.ToString)
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
                '  summe = summe.Substring(0, summe.Length - 1)
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
                              " FROM " & CLstart.myViewsNTabs.tabWV & "  w , " & CLstart.myViewsNTabs.tabStammdaten & " s " +
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
                '  faelligeWV(i) = 1
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
            Dim handcsv As New LIBcsvAusgabe.clsCSVausgaben("Vorgaenge", myglobalz.sitzung.VorgangREC.dt, myglobalz.sitzung.aktVorgangsID, "",
                                                            CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
            nachricht(" exportfile$ = " & handcsv.CscDateiAusgeben())
            handcsv.start()
            handcsv.Dispose()
        End Sub
    End Class

    Class zeigeVorgaenge
        Private Sub New()

        End Sub
        Public Shared Sub exe()
            Dim result As Long = initVorgaengeDatatable("")
            If result < 1 Then
                If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
                    nachricht("Es wurden keine Vorgänge in der DB gefunden")
                End If
                Exit Sub
            End If
        End Sub

        Public Shared Function initVorgaengeDatatable(Byref hinweis As String) As Long
            Try
                hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
                Dim retval As Long = myGlobalz.sitzung.VorgangREC.mycount
                nachricht("initVorgaengeDatatable Treffer: " & retval)
                Return retval
            Catch ex As Exception
                nachricht_und_Mbox("initVorgaengeDatatable: " & ex.ToString)
                Return -1
            End Try
        End Function

        'Public Shared Function VorgaengeAnzeigenFuerHausnummerExtracted() As Boolean
        '    Dim resultdt As New System.Data.DataTable
        '    Dim erfolg As Integer
        '    Dim instring As String
        '    'myGlobalz.sitzung.tempREC.mydb.Tabelle, ???
        '    Try
        '        myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Adresse
        '        erstelleSQLfuerhausnr2sekid_alledb() 'sql wird erstellt       
        '        erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
        '        If erfolg < 1 Then
        '            Return False
        '        End If
        '        myGlobalz.sitzung.VorgangREC.dt = resultdt.Copy
        '        'myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="PA_SEKID2VID"
        '        instring = clsDBtools.bildINstring(resultdt, 0)
        '        myGlobalz.sitzung.VorgangREC.mydb.SQL = "select vorgangsid from PA_SEKID2VID " &
        '            " where SEKID IN(" & instring & ")"
        '        erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
        '        myGlobalz.sitzung.tempREC.dt = resultdt.Copy
        '        instring = clsDBtools.bildINstring(resultdt, 0)

        '        myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from (" & CLstart.myViewsNTabs.view_vsk_d & ") where  vorgangsid IN (" & instring & ") "
        '        erfolg = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.VorgangREC.mydb.SQL, resultdt)
        '        myGlobalz.sitzung.VorgangREC.dt = resultdt.Copy
        '        Return True
        '    Catch ex As Exception
        '        nachricht("VorgaengeAnzeigenFuerHausnummerExtracted : Keine Adressen gefunden bzw ein Fehl ist aufgetreten: " & ex.ToString)
        '        Return False
        '    End Try
        'End Function


        'Private Shared Sub erstelleSQLfuerhausnr2sekid_alledb()
        '    'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
        '    If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
        '        myGlobalz.sitzung.tempREC.mydb.SQL =
        '     String.Format("select distinct id  from " & CLstart.myViewsNTabs.tabPARAADRESSE  & "  where gemeindenr = {0} and lower(Strassenname) = '{1}' and lower(hausnrkombi) = '{2}' order by abs(hausnrkombi)",
        '                    myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower, myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.ToLower)
        '    End If
        '    If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
        '        myGlobalz.sitzung.tempREC.mydb.SQL =
        '     String.Format("select distinct id  from " & CLstart.myViewsNTabs.tabPARAADRESSE  & "  where gemeindenr = {0} and lower(Strassenname) = '{1}' and lower(hausnrkombi) = '{2}' " &
        '                   "order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
        '                     myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower, myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.ToLower)
        '    End If
        'End Sub

    End Class
    Class verschiedenes
        Private Sub New()

        End Sub
        'Shared Sub savePosition(nameDerForm As String) 'winbestandform
        '    Try
        '        CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positiontop", CType(Me.Top, String))
        '        CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positionleft", CType(Me.Left, String))
        '        CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positionwidth", CType(Me.ActualWidth, String))
        '        CLstart.myc.userIniProfile.WertSchreiben("diverse", nameDerForm & "positionheight", CType(Me.ActualHeight, String))
        '    Catch ex As Exception
        '        l("fehler in saveposition  windb" & ex.ToString)
        '    End Try
        'End Sub
        Public Shared Function GetMaxheight() As Integer
            Dim maxheight As Integer = 680
            If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 Then 'kleine schriftart
                maxheight = 780
            End If
            If myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 1 Then 'mittlere
                maxheight = 680
            End If
            Return maxheight
        End Function
        ' ''' <summary>
        ' ''' Diese Funktion löscht mit linearem Aufwand doppelte Einträge aus einem List(Of T) Array
        ' ''' </summary>
        ' ''' <param name="List">Das Array dessen doppelte Einträge gelöscht werden sollen</param>
        'Public Shared Function RemoveDoubleItems(ByVal List As List(Of String)) As List(Of String)
        '    Dim KeyList As New Generic.Dictionary(Of String, String)
        '    Dim NewList As New List(Of String)
        '    For Each Item As String In List
        '        If KeyList.ContainsKey(Item) = False Then
        '            KeyList.Add(Item, String.Empty)
        '            NewList.Add(Item)
        '        End If
        '    Next
        '    Return NewList
        'End Function

        Public Shared Function mitZusatzSuche(p1 As Boolean, p2 As Boolean) As Boolean
            If p1 Or p2 Then Return True
            Return False
        End Function

        Shared Sub beteiligteFilternAktivieren(button As Button)
            'If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
            '    button.Visibility = Windows.Visibility.Visible
            'Else
            '    button.Visibility = Windows.Visibility.Collapsed
            'End If
        End Sub

        Friend Shared Sub datumFilternAktivieren(gbZeitraumKontrolle As GroupBox, fuerbearbeiterid As Integer)
            If clsParadigmaRechte.istUser_admin_oder_vorzimmer() Then
                gbZeitraumKontrolle.Visibility = Windows.Visibility.Visible
            Else
                If myGlobalz.sitzung.aktBearbeiter.ID = fuerbearbeiterid Then
                    gbZeitraumKontrolle.Visibility = Windows.Visibility.Visible
                Else

                    gbZeitraumKontrolle.Visibility = Windows.Visibility.Collapsed
                End If
            End If
        End Sub

        'Friend Shared Function textFilterIstParadigmanummer(text As String) As Boolean
        '    Try
        '        l(" MOD ---------------------- anfang")
        '        If IsNumeric(text) Then
        '            Return True
        '        Else
        '            Return False
        '        End If
        '        l(" MOD ---------------------- ende")
        '        Return True
        '    Catch ex As Exception
        '        l("Fehler in MOD: " & ex.ToString())
        '        Return False
        '    End Try
        'End Function
    End Class
End Namespace

