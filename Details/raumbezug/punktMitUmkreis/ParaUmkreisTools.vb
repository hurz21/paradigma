
Imports System.Data

Public Class ParaUmkreisTools
    Public Shared Function umkreisNeu_alleDB() As Integer
        Dim result% = 0
        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsUmkreisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            result% = zzz.RB_Umkreis_abspeichern_Neu()
            zzz.dispose
        End If
        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsUmkreisDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            result% = zzz.RB_Umkreis_abspeichern_Neu()
                zzz.dispose
        End If
        Return result%
    End Function

    Public Shared Function umkreisEdit_alleDB(ByVal sekid As Integer) As Integer
        Dim result% = 0
        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsUmkreisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            result% = zzz.RB_Umkreis_abspeichern_Edit(sekid%)
            zzz.dispose
        End If
        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsUmkreisDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            result% = zzz.RB_Umkreis_abspeichern_Edit(sekid%)
                zzz.dispose
        End If
        Return result%
    End Function

    Public Shared Function umkreisLoeschen_alleDB(ByVal Umkreisid As Integer) As Integer
        Dim result% = 0
        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsUmkreisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            result% = zzz.RB_Umkreis_loeschen(Umkreisid)
            zzz.dispose
        End If
        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsUmkreisDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            result% = zzz.RB_Umkreis_loeschen(Umkreisid)
                zzz.dispose
        End If
        Return result%
    End Function



    Public Shared Function umkreisHOLEN_alleDB(ByVal sekid As Integer) As Boolean
        Dim result As Boolean
        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsUmkreisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            result = zzz.RB_ParaUmkreis_holen(CStr(sekid))
            zzz.dispose
        End If
        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsUmkreisDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            result = zzz.RB_ParaUmkreis_holen(CStr(sekid))
                zzz.dispose
        End If
        Return result
    End Function

    Public Shared Function umkreisEntkoppeln_alleDB() As Integer
        Dim result As Integer
        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsUmkreisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            zzz.Umkreis_undVorgang_Entkoppeln(result)
            zzz.dispose
        End If
        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsUmkreisDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            zzz.Umkreis_undVorgang_Entkoppeln(result)
                zzz.dispose
        End If
        Return result
    End Function

    Public Shared Sub Umkreis_Neu()
        ' myGlobalz.sitzung.aktPMU.Name = myGlobalz.sitzung.aktPMU.abstract
        Dim umkreisID% = umkreisNeu_alleDB()
        'Dim umkreisID% = RB_Umkreis_abspeichern_Neu()
        If umkreisID > 0 Then
            myGlobalz.sitzung.aktPMU.Typ = RaumbezugsTyp.Umkreis
            myGlobalz.sitzung.aktPMU.SekID = umkreisID
            myGlobalz.sitzung.aktPMU.defineAbstract()
            DBraumbezug_Mysql.defineBBOX(myGlobalz.sitzung.aktPMU.Radius, myGlobalz.sitzung.aktPMU)
            Dim raumbezugsID% = RBtoolsns.Raumbezug_abspeichern_Neu_alleDB.execute(myGlobalz.sitzung.aktPMU)
            '     Dim raumbezugsID% = DBraumbezug_Mysql.Raumbezug_abspeichern_Neu(myGlobalz.sitzung.aktPMU)
            myGlobalz.sitzung.aktPMU.RaumbezugsID = raumbezugsID
            Dim koppelungsID4% = RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID, 0)
            My.Log.WriteEntry(" Koppelung_Raumbezug_Vorgang:" & koppelungsID4% & " ")
            RB_Umkreis_append_PointShapeFile()
        Else
            nachricht("Problem beim Abspeichern!")
        End If
    End Sub


    Private Shared Function punktZuWeisen(ByVal punkt As LibGISmapgenerator.myPoint, ByRef alpoint As System.Drawing.Point) As Boolean
        Try
            If punkt Is Nothing Then
                Return False
            End If
            alpoint.X = CInt(punkt.X)
            alpoint.Y = CInt(punkt.Y)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Shared Function ParaPunktLiegtImKreisOffenbach(ByVal punkt As LibGISmapgenerator.myPoint, ByVal globalrange As LibGISmapgenerator.clsRange) As Boolean
        Try
            Dim alpoint As System.Drawing.Point
            Dim erfolg As Boolean = punktZuWeisen(punkt, alpoint)
            If Not erfolg Then
                nachricht_und_Mbox("Die Koordinaten waren nicht in Ordnung. Ggf. zu groß?")
                punkt.X = 0
                punkt.Y = 0
                Return False
            End If
            If globalrange.inside(alpoint) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler: ParaPunktLiegtImKreisOffenbach" & ex.ToString)
            Return False
        End Try
    End Function

    Public Shared Sub zum_dgUmkreisEditmode(ByVal tbMinimapCoordinate_Text$)
        Dim sekid = CStr(myGlobalz.sitzung.aktPMU.SekID)
        'RB_ParaUmkreis_holen(sekid$)
        Dim erfolg As Boolean = umkreisHOLEN_alleDB(CInt(sekid$))

        DTaufUmkreisObjektabbilden(myGlobalz.sitzung.tempREC.dt)
        'form aufrufen
        Dim wfst As New Win_punktMitUmkreis(tbMinimapCoordinate_Text)
        wfst.ShowDialog()
    End Sub





    Shared Function DTaufUmkreisObjektabbilden(ByVal dasDT As DataTable) As Boolean
        Try
            With myGlobalz.sitzung.aktPMU
                .Radius = CInt(clsDBtools.fieldvalue(dasDT.Rows(0).Item("radiusM")))
                .Name = CStr(clsDBtools.fieldvalue(dasDT.Rows(0).Item("beschreibung")))
            End With
            Return True
        Catch ex As Exception
            nachricht("Fehler3: DTaufFotoObjektabbilden " & vbCrLf & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function


    Shared Sub RB_Umkreis_append_PointShapeFile()
        nachricht("RB_Umkreis_append_PointShapeFile not implementet  :")
    End Sub


    Shared Sub loeschenAktPMU()
        Dim erfolg%
        If Not glob2.istloeschenErnstgemeint() Then Exit Sub
        If CInt(myGlobalz.sitzung.aktPMU.Status) = 0 Then
            erfolg% = ParaUmkreisTools.umkreisLoeschen_alleDB(CInt(myGlobalz.sitzung.aktPMU.SekID))
            If erfolg > 0 Then
                erfolg = RBtoolsns.Raumbezug_loeschen_byid_alleDB.execute(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID))
                If erfolg > 0 Then
                    ParaUmkreisTools.umkreisEntkoppeln_alleDB()
                Else
                    My.Log.WriteEntry("Problem beim Löschen des Raumbezugs. Abbruch.")
                    nachricht_und_Mbox("Problem beim Löschen des Raumbezugs. Abbruch.")
                End If
            Else
                My.Log.WriteEntry("Problem beim Löschen der Raumbezugs-Umkreis. Abbruch.")
                nachricht_und_Mbox("Problem beim Löschen der Raumbezugs umkreis. Abbruch.")
            End If
        Else
            ParaUmkreisTools.umkreisEntkoppeln_alleDB()
        End If
    End Sub


    Shared Function umkreisSchonInVorgangVorhanden(ByVal clsParaUmkreis As clsParaUmkreis, ByVal vorgangsid As Integer) As Boolean
        myGlobalz.sitzung.tempREC.mydb.SQL =
                  "select * from raumbezug p, pu_sekid2vid s " &
                  " where typ=7" &
                  " and rechts=" & clsParaUmkreis.punkt.X &
                  " and hoch=" & clsParaUmkreis.punkt.Y &
                  " and s.vorgangsid= " & vorgangsid &
                  " and s.sekid=p.sekid"
        nachricht("umkreisSchonInVorgangVorhanden sql: " & myGlobalz.sitzung.tempREC.mydb.SQL)
        Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
        If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            nachricht("umkreisSchonInVorgangVorhanden ist schon vorhanden")
            Return True
        Else
            nachricht("umkreisSchonInVorgangVorhanden ist noch nicht vorhanden")
            Return False
        End If
    End Function
    Shared Sub loeschenAktPolygon(rid As Integer)
        'RBtoolsns.raumbezugsDT2Obj.exe(zeile, myGlobalz.sitzung.aktPolygon)
        nachricht("in loeschenAllePolygone")
        If rid < 1 Then
            nachricht("Fehler in loeschenAktPolygon: rid<1" & rid)
            Exit Sub
        End If
        Dim erfolg As Boolean
        'If Not glob2.istloeschenErnstgemeint() Then Exit Sub
        If CInt(myGlobalz.sitzung.aktPolygon.Status) = 0 Then
            erfolg = PolygonTools.polygonLoeschen(rid)
            If erfolg Then
                nachricht("polygon gelöscht")
            Else
                nachricht("Fehler Problem beim Löschen der Raumbezugs umkreis. Abbruch.")
            End If
        End If
        nachricht("in loeschenAllePolygone---------------------- ende")
    End Sub
End Class
