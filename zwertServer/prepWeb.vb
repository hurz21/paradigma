
Class prepWeb

    Private Shared Sub bereinigeTilenameName(ByRef tilename As String)
        tilename = tilename.Replace(vbCrLf, "")
        tilename = tilename.Replace(vbLf, "")
        tilename = tilename.Replace(vbCr, "")
        tilename = tilename.Replace(vbTab, "").Trim
    End Sub
    Shared Function getZwert(quellKoordinate As clsGEOPoint, defaultDBrec As clsDBspecMYSQL, indexneu As String, quelldir As String) As String
        'indextile_Feststellen
        Dim tilename, zett As String
        Dim verboten As String = ""
nochmal:
        l("getZwert: ´---------------------------------------")
        l("getZwert: gkrechts: " & quellKoordinate.GKrechts)
        l("getZwert: gkhoch: " & quellKoordinate.GKhoch)
        Try
            indexneu = IO.Path.Combine(quelldir, indexneu)
            tilename = getTilename(quellKoordinate, indexneu, verboten)
            bereinigeTilenameName(tilename)
            verboten = tilename
            l("tilename:" & tilename)
            l("verboten:" & verboten)
            If tilename = "ErrorToString" Then
                'kein ergenis
                Return "fehler"
            End If
            Dim zm(9) As clsGEOPoint
            zm = bildeZmatrix(quellKoordinate)
            'zett = getzfromDB(defaultDBrec, tilename, quellKoordinate, verboten)
            zett = getzfromFiles(quelldir, tilename, quellKoordinate, verboten, zm)
            Dim result As String = makeMaktrix(zm)
            l("getZwert vor return: " & result)
            Return result
        Catch ex As Exception
            l("fehler in zwertserver: getZwert: " & ex.ToString)
            Return "fehler"
        End Try
    End Function

    Private Shared Function mapStrings(ByVal a As String(), ByRef xmin As Double, ByRef xmax As Double, ByRef ymin As Double, ByRef ymax As Double) As String
        Dim tilename As String
        l("mapStrings--------------")
        Try
            tilename = a(0)
            xmin = CDbl(a(1).Replace(".", ","))
            xmax = CDbl(a(2).Replace(".", ","))
            ymin = CDbl(a(3).Replace(".", ","))
            ymax = CDbl(a(4).Replace(".", ","))
            Return tilename

        Catch ex As Exception
            l("fehler inmapStrings--------------" & ex.ToString)
            Return "fehler"
        End Try
    End Function

    Private Shared Function getTilename(quellKoordinate As clsGEOPoint, indexneu As String, verboten As String) As String
        'einlesen
        'zerhacken
        'durchsuchen
        l("getTilename:" & quellKoordinate.GKrechts & quellKoordinate.GKhoch)
        l("getTilename: " & indexneu)
        l("getTilename: verboten" & verboten)
        l("getTilename: gkrechts: " & quellKoordinate.GKrechts)
        l("getTilename: gkhoch: " & quellKoordinate.GKhoch)
        Dim summe, recs(), a(), zeile, tilename As String
        Dim xmin, xmax, ymin, ymax As Double
        Dim i = 0
        Using datei As New IO.StreamReader(indexneu)
            summe = datei.ReadToEnd
        End Using
        l("getTilename: summe " & summe)
        recs = summe.Split(CChar(vbCrLf))
        l("runden: " & recs.Count)
        For Each zeile In recs
            l("i: " & i & ": " & zeile)
            i += 1
            a = zeile.Split("#"c)
            tilename = mapStrings(a, xmin, xmax, ymin, ymax)
            If liegtImTile(quellKoordinate, xmin, xmax, ymin, ymax) Then
                l("getTilename: treffer: " & tilename)
                If Not verboten.Contains(tilename) Then
                    l("getTilename: tilename gefunden: " & tilename)
                    Return tilename
                End If
            End If
        Next
        l("getTilename: fehler " & summe)
        Return "ErrorToString"
    End Function

    Private Shared Function liegtImTile(quellKoordinate As clsGEOPoint, xmin As Double, xmax As Double, ymin As Double, ymax As Double) As Boolean
        l("liegtImTile:   anfang ")
        l("liegtImTile: gkrechts: " & quellKoordinate.GKrechts)

        l("liegtImTile: gkhoch: " & quellKoordinate.GKhoch)
        l("liegtImTile: xmin...: " & xmin & " " & xmax & " " & ymin & " " & ymax)

        Try
            If quellKoordinate.GKrechts >= CInt(xmin) And
                quellKoordinate.GKrechts <= CInt(xmax) And
                quellKoordinate.GKhoch >= CInt(ymin) And
                quellKoordinate.GKhoch <= CInt(ymax) Then
                l("liegtImTile:   ende innerhalb")
                Return True
            End If
            l("liegtImTile:   ende ausserhalb")
            Return False
        Catch ex As Exception
            l("fehler inliegtImTile:   -----------" & ex.ToString)
            Return False
        End Try
    End Function

    Private Shared Function getTabellenameAusTilename(tilename As String) As String

        l("getTabellenameAusTilename:   " & tilename)
        Try
            Dim fi As New IO.FileInfo(tilename)
            Return fi.Name.Replace(".xyz", "")
        Catch ex As Exception
            l("fehler getTabellenameAusTilename:   -----------" & ex.ToString)
            Return "fehler"
        End Try

    End Function

    Private Shared Function getzfromDB(defaultDBrec As clsDBspecMYSQL, tilename As String, quellKoordinate As clsGEOPoint, verboten As String) As String
        Dim tabellenname, hinweis, zett As String
        tabellenname = getTabellenameAusTilename(tilename)
nochmal:
        l("tabellenname:" & tabellenname)
        defaultDBrec.mydb.SQL = "select z from 3d." & tabellenname & " where r=" & quellKoordinate.GKrechts &
            " and h=" & quellKoordinate.GKhoch
        l(" defaultDBrec.mydb.SQL:" & defaultDBrec.mydb.SQL)
        hinweis = defaultDBrec.getDataDT()
        If defaultDBrec.dt.Rows.Count < 1 Then
            zett = "0"
            verboten = verboten & " " & tilename
            l("count <1")
            GoTo nochmal
        Else
            l("count >1")
            For i = 0 To defaultDBrec.dt.Rows.Count

                zett = zett & "#" & clsDBtools.fieldvalue(defaultDBrec.dt.Rows(i).Item("z")).ToString
                l("zett: " & zett)
            Next
        End If
        Return zett
    End Function

    Private Shared Function getzfromFiles(quelldir As String, tilename As String,
                                          quellKoordinate As clsGEOPoint, verboten As String,
                                         ByRef zm() As clsGEOPoint) As String
        l("getzfromFiles: " & quelldir)
        l("getzfromFiles: gkrechts: " & quellKoordinate.GKrechts)
        l("getzfromFiles: gkhoch: " & quellKoordinate.GKhoch)
        Dim zeile, datei, a() As String
        Dim rechts, hoch As integer
        Dim zett As double
        Dim filled As Integer = 0
        Try
            datei = IO.Path.Combine(quelldir, tilename)
            l("datei: " & datei)
            Using adatei As New IO.StreamReader(datei)
                Do Until adatei.EndOfStream
                    zeile = adatei.ReadLine
                    If zeile.Trim.Length < 1 Then
                        Return "fehler1"
                    End If
                    a = zeile.Split(" "c)
                    Try
                        rechts = CInt(Math.round(CDbl(a(0).Replace(".", ","))))
                       ' If  rechts <> Math.round(quellKoordinate.GKrechts Then Continue Do
                        hoch = CInt(Math.round(CDbl(a(1).Replace(".", ","))))
                        zett = CDbl(a(2).Replace(".", ","))
                    Catch ex As Exception
                        Debug.Print("fehler in datei" & datei & " bei pos: ")
                        Return "fehler2"
                    End Try
                    For i = 1 To zm.GetUpperBound(0)
                        If Math.round(rechts) = Math.round(zm(i).GKrechts) And
                               Math.round(hoch) = Math.round(zm(i).GKhoch) Then
                            zm(i).infotext = zett.ToString
                            filled += 1
                            If filled = 9 Then
                                Return "prima"
                            End If
                        End If
                    Next
                    'If CInt(rechts) <> CInt(quellKoordinate.GKrechts) Or
                    '    CInt(hoch) <> CInt(quellKoordinate.GKhoch) Then
                    'Else
                    '    l("getzfromFiles treffer: " & zett)
                    '    Return zett.ToString
                    'End If
                Loop
                Return "nicht gefunden"
            End Using
        Catch ex As Exception
            l("fehler in getzfromFiles: " & ex.ToString)
            Return "fehler3"
        End Try

    End Function

    Private Shared Function bildeZmatrix(quellKoordinate As clsGEOPoint) As clsGEOPoint()
        Dim a(9) As clsGEOPoint
        For i = 0 To a.GetUpperBound(0)
            a(i) = New clsGEOPoint
            a(i).infotext = "___,___"
        Next

        a(1).GKrechts = quellKoordinate.GKrechts - 1
        a(1).GKhoch = quellKoordinate.GKhoch + 1

        a(2).GKrechts = quellKoordinate.GKrechts
        a(2).GKhoch = quellKoordinate.GKhoch + 1

        a(3).GKrechts = quellKoordinate.GKrechts + 1
        a(3).GKhoch = quellKoordinate.GKhoch + 1

        a(4).GKrechts = quellKoordinate.GKrechts - 1
        a(4).GKhoch = quellKoordinate.GKhoch

        a(5).GKrechts = quellKoordinate.GKrechts
        a(5).GKhoch = quellKoordinate.GKhoch

        a(6).GKrechts = quellKoordinate.GKrechts + 1
        a(6).GKhoch = quellKoordinate.GKhoch

        a(7).GKrechts = quellKoordinate.GKrechts - 1
        a(7).GKhoch = quellKoordinate.GKhoch - 1

        a(8).GKrechts = quellKoordinate.GKrechts
        a(8).GKhoch = quellKoordinate.GKhoch - 1
        a(9).GKrechts = quellKoordinate.GKrechts + 1
        a(9).GKhoch = quellKoordinate.GKhoch - 1
        For i = 0 To a.GetUpperBound(0)
            l("i: " & i & "," & CInt(a(i).GKrechts ) & "," & CInt(a(i).GKhoch) & "," & a(i).infotext)

        Next
        Return a
    End Function

    Private Shared Function makeMaktrix(zm As clsGEOPoint()) As String
        Dim summe As String = ""

        summe = summe & zm(1).infotext & " | " & zm(2).infotext & " | " & zm(3).infotext & " | " & Environment.NewLine
        summe = summe & zm(4).infotext & " | " & zm(5).infotext & " | " & zm(6).infotext & " | " & Environment.NewLine
        summe = summe & zm(7).infotext & " | " & zm(8).infotext & " | " & zm(9).infotext & " | " & Environment.NewLine

        Return summe
    End Function







End Class
