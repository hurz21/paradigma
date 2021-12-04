Imports System.Data
Imports paradigmaDetail.CLstart

Namespace NSprojekt

    Public Class Kopplung
        Public Shared Function aufheben(ByVal prj As CLstart.clsProjektAllgemein) As Boolean
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabPROJEKT2VORGANG & " where  vorgangsid=" & prj.vorgangsid & " and projektid=" & prj.id
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
            Return True
        End Function

        Public Shared Function byvorgangsidtId(ByVal prj As CLstart.clsProjektAllgemein) As Boolean
            myGlobalz.sitzung.VorgangREC.mydb.SQL$ = "select * from " & CLstart.myViewsNTabs.tabPROJEKT2VORGANG & " where  vorgangsid=" & prj.vorgangsid
            myGlobalz.sitzung.VorgangREC.getDataDT()
            If Not myglobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                If myglobalz.sitzung.VorgangREC.dt.Rows.Count > 1 Then
                    MsgBox(glob2.getMsgboxText("anzahlTreffer", New List(Of String)(New String() {myglobalz.sitzung.VorgangREC.dt.Rows.Count.ToString})))
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
                    MsgBox(glob2.getMsgboxText("anzahlTreffer",
                                                New List(Of String)(New String() {CStr(myglobalz.sitzung.VorgangREC.dt.Rows.Count)})))
                End If
                dt2obj(prj, myglobalz.sitzung.VorgangREC.dt, 0)
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function alleProjekte(sql As String) As Boolean
            myGlobalz.sitzung.VorgangREC.mydb.SQL = sql
            myGlobalz.sitzung.VorgangREC.getDataDT()
            If Not myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
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
                    prj.refnr = CStr(CInt(clsDBtools.fieldvalue(.Item("REFNR"))))
                End With
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function
    End Class

    Class speichern
        Private Sub New()
        End Sub
        Shared Function exe(ByVal modus As String, ByVal aktProjekt As CLstart.clsProjektAllgemein) As Integer
            Dim projektid As Integer
            If modus = "neu" Then
                Dim querie As String
                werteDBsicherMachenProjekt(aktProjekt)
                clsSqlparam.paramListe.Clear()
                populateParamListeEreignis(aktProjekt, clsSqlparam.paramListe)
                'paramListe.Add(New sqlparam("eid", 0))
                querie = "INSERT INTO " & CLstart.myViewsNTabs.tabPROJEKT & "    (KATEGORIE1,KATEGORIE2,BEZEICHNUNGKURZ,BEZEICHNUNGLAN,VONDATUM,BISDATUM,QUELLE,GEMEINDE,WIEDERVORLAGEID,REFNR,TS) " +
                               " VALUES (@KATEGORIE1,@KATEGORIE2,@BEZEICHNUNGKURZ,@BEZEICHNUNGLAN,@VONDATUM,@BISDATUM,@QUELLE,@GEMEINDE,@WIEDERVORLAGEID,@REFNR,@TS)"
                projektid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")

            End If
            If modus = "edit" Then
                Dim querie As String
                werteDBsicherMachenProjekt(aktProjekt)
                clsSqlparam.paramListe.Clear()
                populateParamListeEreignis(aktProjekt, clsSqlparam.paramListe)
                'paramListe.Add(New sqlparam("eid", 0))
                querie = "UPDATE  " & CLstart.myViewsNTabs.tabPROJEKT & "    " & " SET KATEGORIE1=@KATEGORIE1" &
                                                    ",KATEGORIE2=@KATEGORIE2" &
                                                    ",BEZEICHNUNGKURZ=@BEZEICHNUNGKURZ " &
                                                    ",BEZEICHNUNGLAN=@BEZEICHNUNGLAN " &
                                                    ",VONDATUM=@VONDATUM " &
                                                    ",BISDATUM=@BISDATUM " &
                                                    ",QUELLE=@QUELLE " &
                                                    ",WIEDERVORLAGEID=@WIEDERVORLAGEID " &
                                                    ",REFNR=@REFNR " &
                                                    ",GEMEINDE=@GEMEINDE " &
                                                    " WHERE ID=@ID"
                clsSqlparam.paramListe.Add(New clsSqlparam("ID", aktProjekt.id))
                projektid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")

            End If
            Return projektid
        End Function

        Private Shared Sub populateParamListeEreignis(aktProjekt As clsProjektAllgemein, paramListe As List(Of clsSqlparam))
            paramListe.Add(New clsSqlparam("KATEGORIE1", aktProjekt.Kategorie1))
            paramListe.Add(New clsSqlparam("KATEGORIE2", aktProjekt.Kategorie2))
            paramListe.Add(New clsSqlparam("BEZEICHNUNGKURZ", aktProjekt.BezeichnungKurz))
            paramListe.Add(New clsSqlparam("BEZEICHNUNGLAN", aktProjekt.BezeichnungLang))
            paramListe.Add(New clsSqlparam("VONDATUM", clsDBtools.makedateMssqlConform(aktProjekt.von, myglobalz.sitzung.VorgangREC.mydb.dbtyp)))
            paramListe.Add(New clsSqlparam("BISDATUM", clsDBtools.makedateMssqlConform(aktProjekt.bis, myglobalz.sitzung.VorgangREC.mydb.dbtyp)))
            paramListe.Add(New clsSqlparam("QUELLE", aktProjekt.Quelle))
            paramListe.Add(New clsSqlparam("WIEDERVORLAGEID", aktProjekt.WiedervorlageID))
            paramListe.Add(New clsSqlparam("GEMEINDE", aktProjekt.Gemeinde))
            paramListe.Add(New clsSqlparam("REFNR", aktProjekt.refnr))
            paramListe.Add(New clsSqlparam("TS", aktProjekt.TS))
        End Sub

        Private Shared Sub werteDBsicherMachenProjekt(aktProjekt As clsProjektAllgemein)
            If String.IsNullOrEmpty(aktProjekt.Gemeinde) Then aktProjekt.Gemeinde = ""
            If String.IsNullOrEmpty(aktProjekt.Kategorie1) Then aktProjekt.Kategorie1 = ""
            If String.IsNullOrEmpty(aktProjekt.Kategorie2) Then aktProjekt.Kategorie2 = ""
            If String.IsNullOrEmpty(aktProjekt.BezeichnungKurz) Then aktProjekt.BezeichnungKurz = ""
            If String.IsNullOrEmpty(aktProjekt.BezeichnungLang) Then aktProjekt.BezeichnungLang = ""
            If String.IsNullOrEmpty(aktProjekt.Quelle) Then aktProjekt.Quelle = ""
            If String.IsNullOrEmpty(aktProjekt.refnr) Then aktProjekt.refnr = ""
            aktProjekt.TS = Now
        End Sub
    End Class

    Class loeschen
        Private Sub New()
        End Sub
        Shared Sub exe(ByVal aktProjekt As CLstart.clsProjektAllgemein)
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabPROJEKT & "    " &
             "  where id=" & aktProjekt.id
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
        End Sub
    End Class


    Class projektMitVorgangKoppeln
        Private Sub New()
        End Sub
        Shared Function exe(ByVal aktProjekt As CLstart.clsProjektAllgemein) As Boolean
            Dim querie As String
            clsSqlparam.paramListe.Clear()

            clsSqlparam.paramListe.Add(New clsSqlparam("PROJEKTID", aktProjekt.id))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", aktProjekt.vorgangsid))
            querie = "INSERT INTO " & CLstart.myViewsNTabs.tabPROJEKT2VORGANG & " (PROJEKTID,VORGANGSID) " +
                               " VALUES (@PROJEKTID,@VORGANGSID)"
            Dim id = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
            If id > 0 Then
                Return True
            End If
            'End If
            Return False
        End Function
    End Class
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
            Dim hinweis As String = ""
            'myglobalz.sitzung.VorgangREC.mydb.Tabelle ="projekt2vorgang"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select vorgangsid from " & CLstart.myViewsNTabs.tabPROJEKT2VORGANG & " where  projektid=" & aktProjekt.id &
                " and vorgangsid<>" & aktProjekt.vorgangsid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)

            If myglobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                Return ""
            Else
                hinweis = ""
                bildeHinweis(hinweis)
                If hinweis.EndsWith(", ") Then hinweis = hinweis.Substring(0, hinweis.Length - 2)
                Return hinweis
            End If

        End Function
        Private Shared Sub bildeHinweis(ByRef hinweis$)
            For Each ritem As DataRow In myglobalz.sitzung.VorgangREC.dt.Rows
                hinweis &= ritem.Item(0).ToString & ", "
            Next
        End Sub
    End Class

    Class Projekt_holeprojektnummer
        Private Sub New()
        End Sub
        Shared Function exe(ByVal vid As Integer, ByVal aktProjekt As CLstart.clsProjektAllgemein) As String
            Dim liste As String = "" : Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select projektid from " & CLstart.myViewsNTabs.tabPROJEKT2VORGANG & " where vorgangsid =" & vid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
            If myglobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                Return "0"
            Else
                Return CStr(myglobalz.sitzung.VorgangREC.dt.Rows(0).Item(0))
            End If

        End Function

        'Public Shared Sub ProjektlisteAnzeigen(ByVal dg As System.Windows.Controls.DataGrid) 'dgProjekte
        '    dg.DataContext = Nothing
        '    Dim aktprojekt As New CLstart.clsProjektAllgemein(0)
        '    If NSprojekt.ProjektAusgewaehlt.exe(aktprojekt) Then
        '        ProjektlisteAnzeigenExtracted(dg, aktprojekt)
        '    End If
        'End Sub

        Public Shared Sub ProjektlisteAnzeigenExtracted(ByVal dg As System.Windows.Controls.DataGrid, ByVal aktprojekt As CLstart.clsProjektAllgemein)
            Dim liste As String = NSprojekt.Projekt_gekoppelteListe.exe(aktprojekt)
            Dim hinweis As String = ""
            If Not String.IsNullOrEmpty(liste.Trim) AndAlso liste.Trim <> "0" Then
                myGlobalz.sitzung.VorgangREC.mydb.SQL =
                    "select * from " & CLstart.myViewsNTabs.tabVorgang & " v, " & CLstart.myViewsNTabs.tabStammdaten & " s " &
                    " where s.vorgangsid=v.vorgangsid" &
                    " and s.vorgangsid IN (" & liste & ") "


                'bestandTools.zeigeVorgaenge.exe()
                Dim adrtemp As New DataTable
                '    adrtemp = myglobalz.sitzung.VorgangREC.dt.Copy()
                adrtemp = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
                If adrtemp.Rows.Count < 1 Then
                    nachricht("Es wurden keine Vorgänge in der DB gefunden")
                End If
                dg.DataContext = adrtemp
            Else
                dg.DataContext = Nothing
            End If
        End Sub
    End Class
End Namespace
