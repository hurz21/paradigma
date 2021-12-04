Imports System.Data
Imports paradigma.CLstart

Namespace NSprojekt

    Public Class Kopplung
        Public Shared Function aufheben(ByVal prj As CLstart.clsProjektAllgemein) As Boolean
            'Dim erfolg As Boolean
            'If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
            'End If
            'If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
            '    Dim zzz As New clsProjektCRUD_Oracle(clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), prj)
            '    erfolg = zzz.Projekt_entkoppeln()
            '    zzz.Dispose()
            'End If
            'Return erfolg
        End Function

        Public Shared Function byvorgangsidtId(ByVal prj As CLstart.clsProjektAllgemein) As Boolean
            myGlobalz.sitzung.VorgangREC.mydb.SQL$ = "select * from " & CLstart.myViewsNTabs.tabPROJEKT2VORGANG & " where  vorgangsid=" & prj.vorgangsid
            myGlobalz.sitzung.VorgangREC.getDataDT()
            If Not myglobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                If myglobalz.sitzung.VorgangREC.dt.Rows.Count > 1 Then
                    MsgBox("Es gibt insgesamt " & myglobalz.sitzung.VorgangREC.dt.Rows.Count & " Treffer. Nur der erste wird verwendet!")
                End If
                If Not Kopplungdt2obj(prj, myglobalz.sitzung.VorgangREC.dt, 0) Then
                    Return False
                End If
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function Kopplungdt2obj(ByVal prj As CLstart.clsProjektAllgemein, ByVal dataTable As System.Data.DataTable, ByVal index As Integer) As Boolean 'myGlobalz.sitzung.VorgangREC.dt
            Try
                With dataTable.Rows(index)
                    prj.id = CInt(clsDBtools.fieldvalue(.Item("PROJEKTID")))
                    Return True
                End With
            Catch ex As Exception
                Return False
            End Try
        End Function
    End Class

    Public Class ladeProjekt
        Public Shared Function byProjektId(ByVal prj As CLstart.clsProjektAllgemein) As Boolean
            myGlobalz.sitzung.VorgangREC.mydb.SQL$ = "select * from " & CLstart.myViewsNTabs.tabPROJEKT & "    where  id=" & prj.id
            myGlobalz.sitzung.VorgangREC.getDataDT()
            If Not myglobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                If myglobalz.sitzung.VorgangREC.dt.Rows.Count > 1 Then
                    MsgBox("Es gibt insgesamt " & myglobalz.sitzung.VorgangREC.dt.Rows.Count & " Treffer. Nur der erste wird verwendet!")
                End If
                dt2obj(prj, myglobalz.sitzung.VorgangREC.dt, 0)
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function alleProjekte(sql As String) As Boolean
            myglobalz.sitzung.VorgangREC.mydb.SQL = sql
            myglobalz.sitzung.VorgangREC.getDataDT()
            If Not myglobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                Return True
            Else
                Return False
            End If
        End Function




        Public Shared Function dt2obj(ByVal prj As CLstart.clsProjektAllgemein, ByVal dataTable As System.Data.DataTable, ByVal index As Integer) As Boolean 'myGlobalz.sitzung.VorgangREC.dt
            Try
                With dataTable.Rows(index)
                    prj.BezeichnungLang = clsDBtools.fieldvalue(.Item("bezeichnunglan")).ToString
                    prj.BezeichnungKurz = clsDBtools.fieldvalue(.Item("bezeichnungkurz")).ToString
                    prj.Kategorie1 = clsDBtools.fieldvalue(.Item("KATEGORIE1")).ToString
                    prj.Kategorie2 = clsDBtools.fieldvalue(.Item("KATEGORIE2")).ToString
                    prj.von = CDate(clsDBtools.fieldvalue(.Item("vonDatum")))
                    prj.bis = CDate(clsDBtools.fieldvalue(.Item("bisDatum")))
                    prj.Quelle = clsDBtools.fieldvalue(.Item("QUELLE")).ToString
                    prj.WiedervorlageID = CInt(clsDBtools.fieldvalue(.Item("WIEDERVORLAGEID")))
                    prj.Gemeinde = clsDBtools.fieldvalue(.Item("GEMEINDE")).ToString
                    prj.id = CInt(clsDBtools.fieldvalue(.Item("ID")))
                    prj.refnr = CStr(clsDBtools.fieldvalue(.Item("REFNR")))
                End With
            Catch ex As Exception

            End Try
        End Function
    End Class

    Class speichern
        Private Sub New()
        End Sub
        Shared Function exe(ByVal modus As String, ByVal aktProjekt As CLstart.clsProjektAllgemein) As Integer
            Dim projektid%
            If modus = "neu" Then
                projektSpeichernNeu(aktProjekt)

            End If
            If modus = "edit" Then
                ProjektspeichernEdit(aktProjekt)
            End If
            Return projektid
        End Function

        Private Shared Function projektSpeichernNeu(aktProjekt As clsProjektAllgemein) As Integer
            Try
                l(" ProjektspeichernEdit ---------------------- anfang")
                Dim querie As String
                querie = "INSERT INTO " & CLstart.myViewsNTabs.tabPROJEKT &
                "    (KATEGORIE1,KATEGORIE2,BEZEICHNUNGKURZ,BEZEICHNUNGLAN,VONDATUM,BISDATUM,QUELLE,GEMEINDE,WIEDERVORLAGEID,REFNR,TS) " +
                               " VALUES (@KATEGORIE1,@KATEGORIE2,@BEZEICHNUNGKURZ,@BEZEICHNUNGLAN,@VONDATUM,@BISDATUM,@QUELLE,@GEMEINDE,@WIEDERVORLAGEID,@REFNR,@TS)"


                clsSqlparam.paramListe.Clear()
                If aktProjekt.Kategorie1.IsNothingOrEmpty Then aktProjekt.Kategorie1 = ""
                If aktProjekt.Kategorie2.IsNothingOrEmpty Then aktProjekt.Kategorie2 = ""
                If aktProjekt.Quelle.IsNothingOrEmpty Then aktProjekt.Quelle = ""
                If aktProjekt.BezeichnungLang.IsNothingOrEmpty Then aktProjekt.BezeichnungLang = ""
                If aktProjekt.BezeichnungKurz.IsNothingOrEmpty Then aktProjekt.BezeichnungKurz = ""
                If aktProjekt.refnr.IsNothingOrEmpty Then aktProjekt.refnr = ""
                aktProjekt.TS = Now

                pupulateProject(aktProjekt)
                clsSqlparam.paramListe.Add(New clsSqlparam("id", aktProjekt.id))
                Dim newid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")

                If newid > 0 Then
                    Return CInt(newid)
                Else
                    Return 0
                End If
                l(" ProjektspeichernEdit ---------------------- ende")
            Catch ex As Exception
                l("Fehler in ProjektspeichernEdit: " & ex.ToString())
                Return -1
            End Try
        End Function

        Private Shared Function ProjektspeichernEdit(aktProjekt As clsProjektAllgemein) As Integer
            Try
                l(" ProjektspeichernEdit ---------------------- anfang")
                Dim querie As String
                querie = "UPDATE  " & CLstart.myViewsNTabs.tabPROJEKT & "    " &
                                " SET KATEGORIE1=@KATEGORIE1" &
                                ",KATEGORIE2=@KATEGORIE2" &
                                ",BEZEICHNUNGKURZ=@BEZEICHNUNGKURZ " &
                                ",BEZEICHNUNGLAN=@BEZEICHNUNGLAN " &
                                ",VONDATUM=@VONDATUM " &
                                ",BISDATUM=@BISDATUM " &
                                ",QUELLE=@QUELLE " &
                                ",WIEDERVORLAGEID=@WIEDERVORLAGEID " &
                                ",REFNR=@REFNR " &
                                ",GEMEINDE=@GEMEINDE " &
                                ",TS=@TS " &
                                " WHERE ID=@ID"

                clsSqlparam.paramListe.Clear()
                If aktProjekt.Kategorie1.IsNothingOrEmpty Then aktProjekt.Kategorie1 = ""
                If aktProjekt.Kategorie2.IsNothingOrEmpty Then aktProjekt.Kategorie2 = ""
                If aktProjekt.Quelle.IsNothingOrEmpty Then aktProjekt.Quelle = ""
                If aktProjekt.BezeichnungLang.IsNothingOrEmpty Then aktProjekt.BezeichnungLang = ""
                If aktProjekt.BezeichnungKurz.IsNothingOrEmpty Then aktProjekt.BezeichnungKurz = ""
                If aktProjekt.refnr.IsNothingOrEmpty Then aktProjekt.refnr = ""

                pupulateProject(aktProjekt)
                clsSqlparam.paramListe.Add(New clsSqlparam("id", aktProjekt.id))
                Dim anzahlTreffer = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")

                If anzahlTreffer < 1 Then
                    nachricht_und_Mbox("Problem beim Abspeichern:" & myglobalz.sitzung.VorgangREC.mydb.SQL)
                    Return -1
                Else
                    Return CInt(anzahlTreffer)
                End If
                l(" ProjektspeichernEdit ---------------------- ende")
            Catch ex As Exception
                l("Fehler in ProjektspeichernEdit: " & ex.ToString())
                Return -1
            End Try
        End Function
        Shared Sub pupulateProject(aktProjekt As clsProjektAllgemein)
            clsSqlparam.paramListe.Add(New clsSqlparam("KATEGORIE1", aktProjekt.Kategorie1))
            clsSqlparam.paramListe.Add(New clsSqlparam("KATEGORIE2", aktProjekt.Kategorie2))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEZEICHNUNGKURZ", aktProjekt.BezeichnungKurz))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEZEICHNUNGLAN", aktProjekt.BezeichnungLang))
            clsSqlparam.paramListe.Add(New clsSqlparam("VONDATUM", aktProjekt.von))
            clsSqlparam.paramListe.Add(New clsSqlparam("BISDATUM", aktProjekt.bis))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", aktProjekt.Quelle))
            clsSqlparam.paramListe.Add(New clsSqlparam("WIEDERVORLAGEID", aktProjekt.WiedervorlageID))
            clsSqlparam.paramListe.Add(New clsSqlparam("GEMEINDE", aktProjekt.Gemeinde))
            clsSqlparam.paramListe.Add(New clsSqlparam("REFNR", aktProjekt.refnr))
            clsSqlparam.paramListe.Add(New clsSqlparam("TS", aktProjekt.TS))
        End Sub

    End Class

    Class loeschen
        Private Sub New()
        End Sub
        Shared Sub exe(ByVal aktProjekt As CLstart.clsProjektAllgemein)
            Dim hinweis As String = ""

            If aktProjekt.id < 1 Then
                Exit Sub
            End If
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabPROJEKT & "    where id=" & aktProjekt.id.ToString
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)

            'If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
            'End If
            'If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
            '    Dim zzz As New clsProjektCRUD_Oracle(clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
            '    Dim erfolg As Boolean = zzz.Projekt_loeschen()
            '    zzz.Dispose()
            'End If
        End Sub
    End Class


    'Class projektMitVorgangKoppeln
    '    'Private Sub New()
    '    'End Sub
    '    'Shared Function exe(ByVal aktProjekt As CLstart.clsProjektAllgemein) As Boolean
    '    '    'If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
    '    '    'End If
    '    '    'If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
    '    '    '    Dim zzz As New clsProjektCRUD_Oracle(clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
    '    '    '    Dim erfolg As Integer = zzz.Projekt_koppeln()
    '    '    '    zzz.Dispose()
    '    '    '    If erfolg > 0 Then
    '    '    '        Return True
    '    '    '    End If
    '    '    'End If
    '    '    'Return False
    '    'End Function
    'End Class
    Class ProjektAusgewaehlt
        Private Sub New()
        End Sub
        Public Shared Function exe(ByRef aktprojekt As CLstart.clsProjektAllgemein) As Boolean
            Dim prjl As New winProjektListe(aktprojekt.vorgangsid)
            prjl.ShowDialog()
            If prjl.DialogResult.HasValue Then
                If prjl.DialogResult.Value Then
                    'auswahlaktivieren
                    aktprojekt = prjl.aktprojekt
                    Return True
                End If
            End If
            Return False
        End Function
    End Class

    Class Projekt_gekoppelteListe
        Private Sub New()
        End Sub
        Shared Function exe(ByVal aktProjekt As CLstart.clsProjektAllgemein) As String
            Dim liste As String = ""
            If myglobalz.vorgang_MYDB.dbtyp = "sqls" Then
                Dim hinweis As String
                Dim newid& = -1
                Try
                    'myglobalz.sitzung.VorgangREC.mydb.Tabelle ="projekt2vorgang"
                    myGlobalz.sitzung.VorgangREC.mydb.SQL = "select vorgangsid from " & CLstart.myViewsNTabs.tabPROJEKT2VORGANG & "    where  projektid=" & aktProjekt.id &
                        " and vorgangsid<>" & aktProjekt.vorgangsid
                    hinweis = myglobalz.sitzung.VorgangREC.getDataDT()
                    If myglobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                        Return ""
                    Else
                        hinweis = ""
                        bildeHinweis(hinweis)
                        If hinweis.EndsWith(", ") Then hinweis = hinweis.Substring(0, hinweis.Length - 2)
                        Return hinweis
                    End If
                Catch ex As Exception
                    nachricht_und_Mbox("Projekt_loeschen: " & vbCrLf & ex.ToString)
                    Return ex.ToString
                End Try
            End If
            If (myglobalz.vorgang_MYDB.dbtyp = "mysql") Then
            End If
            If (myglobalz.vorgang_MYDB.dbtyp = "oracle") Then
                'Dim zzz As New clsProjektCRUD_Oracle(clsDBspecOracle.getConnection(myglobalz.vorgang_MYDB), aktProjekt)
                'liste = zzz.Projekt_status()
                'zzz.Dispose()
            End If
            Return liste
        End Function
        Shared Sub bildeHinweis(ByRef hinweis$)
            For Each ritem As DataRow In myglobalz.sitzung.VorgangREC.dt.Rows
                hinweis &= ritem.Item(0).ToString & ", "
            Next
        End Sub
    End Class

    Class Projekt_holeprojektnummer
        'Private Sub New()
        'End Sub
        'Shared Function exe(ByVal vid As Integer, ByVal aktProjekt As CLstart.clsProjektAllgemein) As String
        '    'Dim liste As String = ""
        '    'If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
        '    'End If
        '    'If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
        '    '    Dim zzz As New clsProjektCRUD_Oracle(clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
        '    '    liste = zzz.Projekt_fuerVorgang(vid)
        '    '    zzz.Dispose()
        '    'End If
        '    'Return liste
        'End Function

        Public Shared Sub ProjektlisteAnzeigen(ByVal dg As System.Windows.Controls.DataGrid) 'dgProjekte
            dg.DataContext = Nothing
            Dim aktprojekt As New CLstart.clsProjektAllgemein(0)
            If NSprojekt.ProjektAusgewaehlt.exe(aktprojekt) Then
                ProjektlisteAnzeigenExtracted(dg, aktprojekt)
            End If
        End Sub

        Public Shared Sub ProjektlisteAnzeigenExtracted(ByVal dg As System.Windows.Controls.DataGrid, ByVal aktprojekt As CLstart.clsProjektAllgemein)
            Dim liste As String = NSprojekt.Projekt_gekoppelteListe.exe(aktprojekt)
            If Not String.IsNullOrEmpty(liste.Trim) AndAlso liste.Trim <> "0" Then
                myGlobalz.sitzung.VorgangREC.mydb.SQL =
                    "select * from " & CLstart.myViewsNTabs.tabVorgang & " v, " & CLstart.myViewsNTabs.tabStammdaten & " s " &
                    " where s.vorgangsid=v.vorgangsid" &
                    " and s.vorgangsid IN (" & liste & ") "
                bestandTools.zeigeVorgaenge.exe()
                Dim adrtemp As New DataTable
                adrtemp = myGlobalz.sitzung.VorgangREC.dt.Copy()
                dg.DataContext = adrtemp
            Else
                dg.DataContext = Nothing
            End If
        End Sub
    End Class
End Namespace
