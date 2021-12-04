Imports System.Data

Module modThumbnail
    Friend Sub getAllTn(vid As Integer)
        '    If Not IsNumeric(vid) Then Exit Sub
        Dim CDTIexeFile, inputfile, outfile, parameter, kommando, fschalter As String
        Dim innDir, outDir As String
        CDTIexeFile = Chr(34) & "C:\Program Files (x86)\Softinterface, Inc\Convert Document To Image\cdti.exe " & Chr(34)
        parameter = " /1 1"
        innDir = "J:\test\paradigmaArchiv\backup\archiv" '"j:\test\paradigmaArchiv\backup\archiv"
        outDir = "J:\test\thumbnails\"
        REM exe% /S"j:\test\paradigmaArchiv\backup\archiv\2012\9609\142217" /F13 /T"J:\test\thumbnails\Kreiskopf-2015.png" /1 1
        REM /F switch: 9doc,13docx,12 is pdf,5rtf
        'C:\Program Files (x86)\Softinterface, Inc\Convert Document To Image\cdti.exe  /S 'J:\test\paradigmaArchiv\backup\archiv/2012/9609/2012/9609\142219' /F12 'J:\test\thumbnails\9609\142219.png' /1 1


        DBfestlegen()

        vorgangrec.mydb.SQL = "SELECT * FROM dokumente where vid=" & vid
        Dim hinweis As String = vorgangrec.getDataDT()
        Dim relativpfad As String = "", dateinameext As String = "", typ As String, batchfile As String
        Dim newsavemode As Boolean

        Dim dokumentid As Integer = 0
        IO.Directory.CreateDirectory(outDir & vid.ToString)
        batchfile = outDir & vid.ToString & "\tnmaker_" & vid & "gen.bat"
        Using sw As New IO.StreamWriter(batchfile)

            For Each drr As DataRow In vorgangrec.dt.Rows
                dokumentid = CInt(drr.Item("dokumentid"))
                relativpfad = CStr(drr.Item("relativpfad"))
                dateinameext = CStr(drr.Item("dateinameext"))
                newsavemode = CBool(drr.Item("newsavemode"))
                typ = CStr(drr.Item("typ"))
                If newsavemode Then
                    inputfile = innDir & IO.Path.Combine(relativpfad, CType(dokumentid, String))
                    inputfile = inputfile.Replace("/", "\")
                    inputfile = Chr(34) & inputfile & Chr(34)
                    outfile = outDir & IO.Path.Combine(vid.ToString, CType(dokumentid, String)) & ".png"
                    outfile = outfile.Replace("/", "\")
                    outfile = Chr(34) & outfile & Chr(34)
                    '  Dim sw As New IO.StreamWriter(batchfile)
                    Select Case typ.ToLower
                        Case "pdf"
                            fschalter = " /F12 "
                        Case "doc"
                            fschalter = " /F9 "
                        Case "docx"
                            fschalter = " /F13 "
                        Case "rtf"
                            fschalter = " /F5 "
                        Case Else
                            fschalter = "ungültig"
                    End Select
                    If fschalter = "ungültig" Then Continue For
                    kommando = CDTIexeFile & " /S" & inputfile & fschalter & " /T" & outfile & parameter
                    sw.WriteLine(kommando)
                End If
            Next
        End Using
        Try
            Process.Start(batchfile)
        Catch ex As Exception

        End Try
    End Sub
    Private Sub DBfestlegen()
        vorgangrec = New LIBoracle.clsDBspecOracle
        ereignisRec = New LIBoracle.clsDBspecOracle
        rbrec = New LIBoracle.clsDBspecOracle

        vorgangrec.mydb.Host = "ora-clu-vip-003"
        vorgangrec.mydb.Schema = "paradigma"
        vorgangrec.mydb.ServiceName = "paradigma.kreis-of.local"
        vorgangrec.mydb.username = "paradigma"
        vorgangrec.mydb.password = "luftikus12"

        ereignisRec.mydb.Host = "ora-clu-vip-003"
        ereignisRec.mydb.Schema = "paradigma"
        ereignisRec.mydb.ServiceName = "paradigma.kreis-of.local"
        ereignisRec.mydb.username = "paradigma"
        ereignisRec.mydb.password = "luftikus12"


        rbrec.mydb.Host = "ora-clu-vip-003"
        rbrec.mydb.Schema = "paradigma"
        rbrec.mydb.ServiceName = "paradigma.kreis-of.local"
        rbrec.mydb.username = "paradigma"
        rbrec.mydb.password = "luftikus12"

        MeineDBConnection = New OracleConnection("Data Source=(DESCRIPTION=" &
     "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & vorgangrec.mydb.Host & ")(PORT=1521)))" &
      "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=ora-clu-vip-004)(PORT=1521)))" &
     "(LOAD_BALANCE=yes)(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & vorgangrec.mydb.ServiceName & ")));" &
     "User Id=" & vorgangrec.mydb.username & ";Password=" & vorgangrec.mydb.password & ";")
    End Sub
End Module
