Module modGesetzSQL
    'hier werden alle trim( in sql statements entfernt
    Friend Function compose(sgzahl As String, Artid As Integer, Herkunftid As Integer, chkistgueltig As CheckBox,
                            datum As String, Stichwort As String, GesetzListefirstTime As Boolean) As String
        Dim sgZahllenge As Integer
        Dim dreistellig As String
        Dim zweistellig As String
        Dim einstellig As String
        Dim sql, sWhereAnfang, sFrom, sHerkunft, sArt, sStichwort As String
        Dim sSachgebiet4st, sSachgebiet3st, sSachgebiet2st, sSachgebiet1st, sSachgebietLike As String

        Dim sOrderBy As String = " sACHGEBIETNR order by SACHGEBIETNR asc"
        sgzahl = RTrim(sgzahl)
        sgZahllenge = Len(sgzahl)
        sStichwort = getsStichwort(Stichwort)
        sArt = getsArt(Artid)
        sHerkunft = getsherkunft(Herkunftid)
        Dim selectspalten, Sselect As String
        selectspalten = " schlagworte,beschreibung,art,dateityp,dateinameohneext,ordner,stid,quelle,herkunft,quellentyp," &
            "wannveroeffentlicht,url,istgueltig,SACHGEBIETNR,sgheader,originalname "
        Sselect = "select " & selectspalten
        sFrom = " rechtsdb1 "
        sWhereAnfang = " where Stammid>0 "

        Select Case sgZahllenge
            Case 4
                dreistellig = sgzahl.Substring(0, 3)
                zweistellig = sgzahl.Substring(0, 2)
                einstellig = sgzahl.Substring(0, 1)
            Case 3
                zweistellig = sgzahl.Substring(0, 2)
                einstellig = sgzahl.Substring(0, 1)
            Case 2
                einstellig = sgzahl.Substring(0, 1)
            Case 1
                einstellig = sgzahl.Substring(0, 1)
        End Select
        sSachgebiet4st = getsSachgebiet4st(sgzahl)
#Disable Warning BC42104 ' Variable 'dreistellig' is used before it has been assigned a value. A null reference exception could result at runtime.
        sSachgebiet3st = getsSachgebiet3st(sgzahl, dreistellig)
#Enable Warning BC42104 ' Variable 'dreistellig' is used before it has been assigned a value. A null reference exception could result at runtime.
#Disable Warning BC42104 ' Variable 'zweistellig' is used before it has been assigned a value. A null reference exception could result at runtime.
        sSachgebiet2st = getsSachgebiet2st(sgzahl, zweistellig)
#Enable Warning BC42104 ' Variable 'zweistellig' is used before it has been assigned a value. A null reference exception could result at runtime.
#Disable Warning BC42104 ' Variable 'einstellig' is used before it has been assigned a value. A null reference exception could result at runtime.
        sSachgebiet1st = getsSachgebiet1st(sgzahl, einstellig)
#Enable Warning BC42104 ' Variable 'einstellig' is used before it has been assigned a value. A null reference exception could result at runtime.

        sSachgebietLike = getsSachgebietLike(sgzahl)
        Select Case sgZahllenge
            Case 4

                If GesetzListefirstTime Then
                    sql = "select * From (" & (Sselect & " from " & sFrom & sWhereAnfang & sSachgebiet4st & sStichwort & " union " &
                  Sselect & " from " & sFrom & sWhereAnfang & sSachgebiet3st & sStichwort & " union " &
                   Sselect & " from " & sFrom & sWhereAnfang & sSachgebiet2st & sStichwort & " union " &
                   Sselect & " from " & sFrom & sWhereAnfang & sSachgebiet1st) & sStichwort & ")" & sOrderBy
                Else
                    sql = Sselect & " from " & sFrom & sWhereAnfang & sStichwort & sSachgebietLike & sArt & sHerkunft & sOrderBy
                End If

            Case 3
                zweistellig = sgzahl.Substring(0, 2)
                einstellig = sgzahl.Substring(0, 1)

                sql = Sselect & " from " & sFrom & sWhereAnfang & sStichwort & sSachgebietLike & sArt & sHerkunft & sOrderBy
            Case 2
                einstellig = sgzahl.Substring(0, 1)

                sql = Sselect & " from " & sFrom & sWhereAnfang & sStichwort & sSachgebietLike & sArt & sHerkunft & sOrderBy
            Case 1
                einstellig = sgzahl.Substring(0, 1)
                sql = Sselect & " from " & sFrom & sWhereAnfang & sStichwort & sSachgebietLike & sArt & sHerkunft & sOrderBy
            Case 0
                sql = Sselect & " from " & sFrom & sWhereAnfang & sStichwort & sArt & sHerkunft & sOrderBy
        End Select
#Disable Warning BC42104 ' Variable 'sql' is used before it has been assigned a value. A null reference exception could result at runtime.
        Return sql
#Enable Warning BC42104 ' Variable 'sql' is used before it has been assigned a value. A null reference exception could result at runtime.
    End Function

    Private Function getsherkunft(herkunftid As Integer) As String
        Dim temp As String = ""
        If herkunftid = 7 Or herkunftid = 0 Then
        Else
            temp = " and  (herkunft=" & herkunftid & ") "
        End If
        Return temp
    End Function

    Private Function getsArt(artid As Integer) As String
        Dim temp As String = ""
        If artid = 0 Or artid = 6 Then
        Else
            temp = " and  (art =" & artid & ") "
        End If
        Return temp
    End Function

    Private Function getsStichwort(stichwort As String) As String
        Dim temp As String = ""
        If stichwort.IsNothingOrEmpty Then
        Else
            stichwort = stichwort.Trim.ToLower
            temp = " and  ( " &
                "   (lower(rtrim(schlagworte)) like '%" & stichwort & "%')  or  " &
                "   (lower(rtrim(beschreibung)) like '%" & stichwort & "%')  or  " &
                "   (lower(rtrim(originalname)) like '%" & stichwort & "%')  or  " &
                "   (lower(rtrim(dateinameohneext)) like '%" & stichwort & "%') )   "
        End If
        Return temp
    End Function

    Private Function getsSachgebietLike(sgzahl As String) As String
        Dim temp As String = ""
        If sgzahl.IsNothingOrEmpty Then
        Else
            temp = " and   rtrim(sACHGEBIETNR) like '" & sgzahl & "%'   "
        End If
        Return temp
    End Function

    Private Function getsSachgebiet1st(sgzahl As String, einstellig As String) As String
        Dim temp As String = ""
        If sgzahl.IsNothingOrEmpty Then
        Else
            temp = "  and  rtrim(sACHGEBIETNR) = '" & einstellig & "'"
        End If
        Return temp
    End Function

    Private Function getsSachgebiet2st(sgzahl As String, zweistellig As String) As String
        Dim temp As String = ""
        If sgzahl.IsNothingOrEmpty Then
        Else
            temp = " and   rtrim(sACHGEBIETNR) = '" & zweistellig & "'"
        End If
        Return temp
    End Function

    Private Function getsSachgebiet3st(sgzahl As String, dreistellig As String) As String
        Dim temp As String = ""
        If sgzahl.IsNothingOrEmpty Then
        Else
            temp = "  and  rtrim(sACHGEBIETNR) = '" & dreistellig & "'"
        End If
        Return temp
    End Function

    Private Function getsSachgebiet4st(sgzahl As String) As String
        Dim temp As String = ""
        If sgzahl.IsNothingOrEmpty Then
        Else
            temp = " and rtrim(sACHGEBIETNR) = '" & sgzahl & "' "
        End If
        Return temp
    End Function
End Module
