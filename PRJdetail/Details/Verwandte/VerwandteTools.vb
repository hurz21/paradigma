Imports System.Data

Namespace VerwandteTools
    Public Class divers
        Public Sub startNewVorgang(vid As Integer)
            Dim paradigmadetail As String = "c:\kreisoffenbach\" & "\paradigmadetail\paradigmadetail.exe"
            Try
                l("paradigmavorgangaufrufen---------------------- anfang")
                Dim si As New ProcessStartInfo
                si.FileName = paradigmadetail
                si.WorkingDirectory = "c:\ptest\paradigmadetail"
                si.Arguments = " /vid=" & vid
                'Process.Start(neuervorgangstgring, "modus=neu")
                Process.Start(si)
                si = Nothing
                l("paradigmavorgangaufrufen---------------------- ende")
            Catch ex As Exception
                l("Fehler in paradigmavorgangaufrufen: " & ex.ToString())
            End Try
        End Sub
        Shared Function verwandtenFuellenUndKoppeln(fremdVID As Integer, ByRef fremdstamm As Stamm, aktvid As Integer) As Boolean
            Dim erfolg = False
            Dim titel As String = ""
            Dim fremdvorgangREC As IDB_grundfunktionen
            fremdstamm = New Stamm(CLstart.mycSimple.MeinNULLDatumAlsDate)
            fremdvorgangREC = CType(myGlobalz.sitzung.VorgangREC, IDB_grundfunktionen)
            erfolg = clsVorgangCTRL.leseVorgangvonDBaufObjekt(fremdVID%, fremdstamm, fremdvorgangREC)
            If erfolg Then
                'koppeln
                titel = String.Format("{0} {1} ({2}, {3})",
                                      fremdstamm.az.gesamt,
                                      fremdstamm.Beschreibung,
                                      Now.ToLongDateString,
                                      myGlobalz.sitzung.aktBearbeiter.Initiale)
                'titel = String.Format("{0} {1}", Now.ToLongDateString, myGlobalz.sitzung.aktBearbeiter.ID & ", " & myGlobalz.sitzung.aktBearbeiter.Initiale)
                VerwandteTools.Koppelung_Vorgang_Fremdvorgang.exe(aktvid, fremdVID, titel)
            End If
            Return erfolg
        End Function
        Friend Shared Function getReferenzvorgangsId(vorgangsID As Integer) As List(Of Integer)
            Dim sql As String '= "select * from  " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG & "  where vorgangsid=" & vorgangsID
            Dim vid, sachgebietnr As Integer
            Dim vidlist As New List(Of Integer)
            Try
                l(" MOD getReferenzvorgangsId anfang")
                sql = "select vorgangsid,sachgebietnr  FROM " & CLstart.myViewsNTabs.TABVORGANG &
                        " where vorgangsid in (  " &
                        " SELECT  FREMDVORGANGSID   FROM " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG &
                        "  where vorgangsid=" & vorgangsID & ")"
                'clsVerwandte_mysql.erzeugeVerwandtenlistezuVorgang(sql$)
                VerwandteTools.erzeugeVerwandtenlistezuVorgang.exe(sql, myGlobalz.sitzung.tempREC)
                'myGlobalz.sitzung.VerwandteDT.Clear()
                'myGlobalz.sitzung.VerwandteDT = myGlobalz.sitzung.tempREC.dt.Copy 
                For Each item As DataRow In myGlobalz.sitzung.tempREC.dt.AsEnumerable
                    vid = CInt(item("vorgangsid"))
                    If IsNumeric(item("sachgebietnr").ToString.Trim) Then
                        sachgebietnr = CInt(item("sachgebietnr"))
                    Else
                        'wg. Alte vorgangsnummern der wasserbehörde
                        sachgebietnr = 0
                    End If
                    If sachgebietnr = 1020 Then
                        vidlist.Add(vid)
                    End If
                Next
                l(" MOD getReferenzvorgangsId ende")
                Return vidlist
            Catch ex As Exception
                l("Fehler in getReferenzvorgangsId: ", ex)
                Return vidlist
            End Try
        End Function

        Friend Shared Function getVerwServerVorschau(verwandteDTServer As DataTable) As String
            Dim summe As String = ""
            Try
                For Each item In verwandteDTServer.AsEnumerable
                    summe = summe & " " & CInt(item("vorgangsid"))
                Next
                Return summe
            Catch ex As Exception
                Return summe
            End Try
        End Function
    End Class
    Public Class Koppelung_Vorgang_Fremdvorgang
        Public Shared Function exe(ByVal VorgangsID As Integer,
                                   ByVal fremdVID As Integer,
                                   ByVal titel As String) As Integer 'einzelDokument_holen(myGlobalz.sitzung.aktDokument.DocID.ToString)
            Dim erfolg As Integer

            Dim querie As String
            '  werteDBsicherMachenEreignis(ereignis)
            clsSqlparam.paramListe.Clear()
            '   populateParamListeEreignis()
            '   clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "INSERT INTO  " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG & "  (VORGANGSID,FREMDVORGANGSID,TITEL) " +
                                " VALUES (@VORGANGSID,@FREMDVORGANGSID,@TITEL)"
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", VorgangsID))
            clsSqlparam.paramListe.Add(New clsSqlparam("FREMDVORGANGSID", fremdVID))
            clsSqlparam.paramListe.Add(New clsSqlparam("TITEL", titel))

            erfolg = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
            Return erfolg
        End Function
    End Class
    Public Class erzeugeVerwandtenlistezuVorgang
        Public Shared Function exe(ByVal sql As String, REC As IDB_grundfunktionen) As Boolean
            Dim erfolg As Boolean : Dim hinweis As String = ""
            REC.dt = getDT4Query(sql, REC, hinweis)
            If REC.dt.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
            Return erfolg
        End Function
    End Class

    Public Class Verwandten_loeschen
        Public Shared Function exe(ByVal kopplungsid As Integer) As Integer
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from  " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG & "  " &
            " where id=" & kopplungsid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function

    End Class
    Public Class Alle_Verwandten_loeschen
        Public Shared Function exe(ByVal vid As Integer) As Integer
            Dim hinweis As String = ""
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from  " & CLstart.myViewsNTabs.TABKOPPVORGANGFREMDVORGANG & "  " &
            " where vorgangsid=" & vid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return 1
        End Function

    End Class

End Namespace

