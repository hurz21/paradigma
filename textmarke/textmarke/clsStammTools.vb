Public Class clsStammTools
    Friend Shared Function getIstConjectVorgang(aktVorgangsID As Integer) As Boolean
        Dim hinweis As String
        Try
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from  " & CLstart.myViewsNTabs.tabConjectbasiert & " " &
                " where s12=" & aktVorgangsID
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            If myGlobalz.sitzung.VorgangREC.dt.Rows.Count < 1 Then
                Return False
            Else
                Return CBool(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item("t43"))
            End If
        Catch ex As Exception
            nachricht("fehler in getkostenDatatable : " ,ex)
            Return False
        End Try
    End Function

    Friend Shared Sub saveIstConject(aktVorgangsID As Integer, wert As Integer)
        l(" MOD saveIstConject anfang")
        Dim querie, hinweis As String
        clsSqlparam.paramListe.Clear()
        Dim returnIdentity As Boolean = True
        Try
            If wert = 1 Then
                querie = "INSERT INTO  " & CLstart.myViewsNTabs.tabConjectbasiert & " (S12,T43) " +
                       " VALUES (@S12,@T43)"
                clsSqlparam.paramListe.Add(New clsSqlparam("S12", aktVorgangsID)) 'MYGLObalz.sitzung.VorgangsID)
                clsSqlparam.paramListe.Add(New clsSqlparam("T43", wert))
                Dim ID As Integer = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
            Else
                querie = "delete from " & CLstart.myViewsNTabs.tabConjectbasiert & " where s12=" & aktVorgangsID
#Disable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
                myGlobalz.sitzung.VorgangREC.dt = getDT4Query(querie, myGlobalz.sitzung.VorgangREC, hinweis)
#Enable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
            End If
        Catch ex As Exception
            l("Fehler in saveIstConject: " ,ex)
        End Try
    End Sub
End Class
