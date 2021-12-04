Imports System.Data
Module expBeteiligte
    
    Sub BETEILIGTE_CsvListeErstellen(datei As String)
        Dim handcsv As New clsCSVausgaben("Beteiligte", myGlobalz.sitzung.beteiligteREC.dt,
                                          myGlobalz.sitzung.aktVorgangsID,
                                         datei, CLstart.mycSimple.Paradigma_local_root, CLstart.mycSimple.enc)
        nachricht(" exportfile:" & handcsv.CscDateiAusgeben())
         handcsv.Dispose
    End Sub

    Sub BETEILIGTE_einzelObjekteErstellen(pfad_beteiligte As String)
        Dim dateiname As String
        '.ToString("yyyy-MM-dd_HH_mm_ss")
        For Each zeile As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
            dateiname = GetDateinameBeteiligte(pfad_beteiligte, zeile, ".txt")
            Using raus As New IO.StreamWriter(dateiname)

                raus.WriteLine(getBeteiligtenString(zeile))
            End Using
        Next
    End Sub

    Private Function GetDateinameBeteiligte(pfad_beteiligte As String, zeile As DataRow, endung As String) As String
        Dim dateiname As String
        dateiname = pfad_beteiligte & "\"
        dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(zeile.Item("ROLLE").ToString)
        dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(zeile.Item("NACHNAME").ToString)
        dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(zeile.Item("VORNAME").ToString)
        dateiname = dateiname & trennerInFileNames & clsString.normalize_Filename(zeile.Item("ORGNAME").ToString)
        dateiname = dateiname & endung
        Return dateiname
    End Function

    'Private Function getBeteiligtenString() As Object 
    'Dim a As New Text.StringBuilder
    '        a.append(zeile.Item("ROLLE").ToString)
    ' End Function 

    Private Function getBeteiligtenString(zeile As DataRow) As Object
        Dim a As New Text.StringBuilder
        Try
            Try
                a.Append("Rolle: " & clsDBtools.fieldvalue(zeile.Item("ROLLE")).ToString & Environment.NewLine)
                a.Append("NACHNAME: " & clsDBtools.fieldvalue(zeile.Item("NACHNAME")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("VORNAME")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("BEMERKUNG")).ToString & Environment.NewLine)
                a.Append("Anschrift: " & clsDBtools.fieldvalue(zeile.Item("GEMEINDENAME")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("STRASSE")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("HAUSNR")).ToString & Environment.NewLine)
                a.Append("Org: " & clsDBtools.fieldvalue(zeile.Item("ORGNAME")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("ORGZUSATZ")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("ORGBEMERKUNG")).ToString & Environment.NewLine)
                a.Append("GESELLFUNKTION: " & clsDBtools.fieldvalue(zeile.Item("GESELLFUNKTION")).ToString & Environment.NewLine)
                a.Append("Telefon: " & clsDBtools.fieldvalue(zeile.Item("FFTELEFON1")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("FFTELEFON2")).ToString & Environment.NewLine)
                a.Append("FAX: " & clsDBtools.fieldvalue(zeile.Item("FFFAX1")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("FFFAX2")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("FFMOBILFON")).ToString & Environment.NewLine)
                a.Append("Email: " & clsDBtools.fieldvalue(zeile.Item("FFEMAIL")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("FFHOMEPAGE")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("FFBEMERKUNG")).ToString & Environment.NewLine)
                a.Append("Konto: " & clsDBtools.fieldvalue(zeile.Item("BVNAME")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("BLZ")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("KONTONR")).ToString & Environment.NewLine)
                a.Append(" : " & clsDBtools.fieldvalue(zeile.Item("BVTITEL")).ToString & Environment.NewLine)
                Return a.ToString
            Catch ex As Exception
                Return a.ToString
            End Try
        Catch ex As Exception
            nachricht("fehler in getBeteiligtenString:",ex)
            return ""
        End Try
    End Function


End Module
