Module modHuetten
    Sub huettenTDaufraeumen()
        'nach export alle ; rauschangen
        Dim roottdir, csvinput, csvprotokoll As String
        roottdir = "C:\Users\Feinen_j\Desktop\relaunch\"
        Dim inputfile As String = roottdir & "td2.txt"
        Dim outfile As String = roottdir & "tdout.csv"

        Dim line As String
        Dim lastline As String
        Dim enc As Text.Encoding
        enc = Text.Encoding.Default
        Try

            Dim sw As New IO.StreamWriter(outfile, False, enc)
            sw.AutoFlush = True
            Using sr As New IO.StreamReader(inputfile, enc)
                Dim asdasd = sr.ReadLine
                sw.WriteLine(asdasd.Replace("#", ";"))
                Do
                    line = sr.ReadLine
                    If line Is Nothing Then Exit Do
                    If line = "" Then Continue Do

                    Try
                        p = line.Split("#"c)
                    Catch ex2 As Exception
                        Debug.Print(ex2.ToString)
                    End Try

                    cnt += 1 : Console.WriteLine(cnt)
                    If p(0) = "erledigt" Or p(0) = "planmaessig" Or p(0) = "laufend" Or p(0) = "vorbereitet" Then
                        'ok weiter
                        If Not lastline.IsNothingOrEmpty Then
                            sw.WriteLine(lastline.Replace(";", "_").Replace("#", ";"))
                        End If
                        
                        lastline = line
                    Else
                        Debug.Print(cnt & "an letzte zeile anfügen")
                        lastline = lastline & line
                    End If
                Loop
                 sw.WriteLine(lastline.Replace(";", "_").Replace("#", ";"))
            End Using

            sw.Close()

        Catch ex As Exception
            Debug.Print(ex.ToString)
        End Try
    End Sub
End Module
