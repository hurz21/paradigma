Imports System.Data

Namespace NSprojekt

    Public Class Kopplung
        Public Shared Function aufheben(ByVal prj As CLstart.clsProjektAllgemein) As Boolean
            Dim erfolg As Boolean
            If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
            End If
            If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
                Dim zzz As New clsProjektCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), prj)
                erfolg = zzz.Projekt_entkoppeln()
                zzz.Dispose()
            End If
            Return erfolg
        End Function

        Public Shared Function byvorgangsidtId(ByVal prj As CLstart.clsProjektAllgemein) As Boolean
            myGlobalz.sitzung.VorgangREC.mydb.SQL$ = "select * from projekt2vorgang where  vorgangsid=" & prj.vorgangsid
            myGlobalz.sitzung.VorgangREC.getDataDT()
            If Not myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                If myGlobalz.sitzung.VorgangREC.dt.Rows.Count > 1 Then
                    MsgBox("Es gibt insgesamt " & myGlobalz.sitzung.VorgangREC.dt.Rows.Count & " Treffer. Nur der erste wird verwendet!")
                End If
                If Not Kopplungdt2obj(prj, myGlobalz.sitzung.VorgangREC.dt, 0) Then
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
            myGlobalz.sitzung.VorgangREC.mydb.SQL$ = "select * from projekt where  id=" & prj.id
            myGlobalz.sitzung.VorgangREC.getDataDT()
            If Not myGlobalz.sitzung.VorgangREC.dt.IsNothingOrEmpty Then
                If myGlobalz.sitzung.VorgangREC.dt.Rows.Count > 1 Then
                    MsgBox("Es gibt insgesamt " & myGlobalz.sitzung.VorgangREC.dt.Rows.Count & " Treffer. Nur der erste wird verwendet!")
                End If
                dt2obj(prj, myGlobalz.sitzung.VorgangREC.dt, 0)
                Return True
            Else
                Return False
            End If
        End Function

        Public Shared Function alleProjekte() As Boolean
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from projekt order by ts desc"
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
                If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
                End If
                If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
                    Dim zzz As New clsProjektCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
                    projektid% = zzz.Projekt_abspeichern_Neu()
                    zzz.Dispose()
                End If
            End If
            If modus = "edit" Then
                If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
                End If
                If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
                    Dim zzz As New clsProjektCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
                    Dim anzahl% = zzz.Projekt_abspeichern_Edit()
                    zzz.Dispose()
                End If
            End If
            Return projektid
        End Function
    End Class

    Class loeschen
        Private Sub New()
        End Sub
        Shared Sub exe(ByVal aktProjekt As CLstart.clsProjektAllgemein)
            If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
            End If
            If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
                Dim zzz As New clsProjektCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
                Dim erfolg As Boolean = zzz.Projekt_loeschen()
                zzz.Dispose()
            End If
        End Sub
    End Class


    Class projektMitVorgangKoppeln
        Private Sub New()
        End Sub
        Shared Function exe(ByVal aktProjekt As CLstart.clsProjektAllgemein) As Boolean
            If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
            End If
            If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
                Dim zzz As New clsProjektCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
                Dim erfolg As Integer = zzz.Projekt_koppeln()
                zzz.Dispose()
                If erfolg > 0 Then
                    Return True
                End If
            End If
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
            If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
            End If
            If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
                Dim zzz As New clsProjektCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
                liste = zzz.Projekt_status()
                zzz.Dispose()
            End If
            Return liste
        End Function
    End Class

    Class Projekt_holeprojektnummer
        Private Sub New()
        End Sub
        Shared Function exe(ByVal vid As Integer, ByVal aktProjekt As CLstart.clsProjektAllgemein) As String
            Dim liste As String = ""
            If (myGlobalz.vorgang_MYDB.dbtyp = "mysql") Then
            End If
            If (myGlobalz.vorgang_MYDB.dbtyp = "oracle") Then
                Dim zzz As New clsProjektCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB), aktProjekt)
                liste = zzz.Projekt_fuerVorgang(vid)
                zzz.Dispose()
            End If
            Return liste
        End Function

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
                    "select * from vorgang v, stammdaten s " &
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
