Imports System.Data

Public Class LocalParameterFiles
    Shared Function parameterEinlesen(ByRef header As String,
                                      ByRef az As String,
                                      ByRef alter_titel As String,
                                      ByRef alter_probaugAz As String,
                                      ByRef altergemKRZ As String) As String
        Try
            Dim datei$ = String.Format("{0}\aktvorgang.txt", clstart.mycsimple.Paradigma_local_root)
            Dim testdatei As New IO.FileInfo(datei)
            If testdatei.Exists Then
                Using fs As New IO.StreamReader(datei)
                    Dim vorgangsid As String = ""
                    vorgangsid$ = fs.ReadLine()
                    Try
                        header = fs.ReadLine()
                        az = fs.ReadLine()
                        header = fs.ReadLine()
                        az = fs.ReadLine()
                        Try
                            az = fs.ReadLine()
                            az = fs.ReadLine()
                            az = fs.ReadLine()
                            az = fs.ReadLine()
                            alter_titel = fs.ReadLine()
                            alter_probaugAz = fs.ReadLine()
                            altergemKRZ = fs.ReadLine()
                        Catch ex As Exception

                        End Try
                    Catch ex As Exception

                    End Try
                    fs.Close()
                    nachricht("vorgangsid$ " & vorgangsid$)
                    Return CInt(vorgangsid).ToString
                End Using
            Else
                nachricht("Es wurde keine ParameterDatei angelegt$ ")
                Return ""
            End If
            testdatei = Nothing
        Catch ex As Exception
            nachricht("Fehler in parameterEinlesen: " & ex.ToString)
            Return ""
        End Try
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>1 zeile vorgangsnr, 2. zeile sachgebietsnr, 3. zeile</remarks>
    Public Shared Sub erzeugeParameterDatei(ByVal verbose As Boolean, ByVal mitgisdarstellung As Boolean)
        Dim zahl As String
        Try
            Using fs As New IO.StreamWriter(String.Format("{0}\aktvorgang.txt", clstart.mycsimple.Paradigma_local_root))
                Try
                    fs.WriteLine(myGlobalz.sitzung.aktVorgangsID.ToString)
                    zahl = myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl
                    If zahl IsNot Nothing AndAlso zahl.Length > 0 Then
                        fs.WriteLine(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.Substring(0, 1))
                    Else
                        fs.WriteLine("")
                    End If
                    fs.WriteLine("") 'Logfile wird von karte eingelesen
                    fs.WriteLine(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header) ' 
                    fs.WriteLine(myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
                    If myGlobalz.sitzung.aktEreignis.ID > 0 Then
                        fs.WriteLine(myGlobalz.sitzung.aktEreignis.ID.ToString)
                        fs.WriteLine(String.Format("{0} {1}", myGlobalz.sitzung.aktEreignis.Art, myGlobalz.sitzung.aktEreignis.Beschreibung))
                    Else
                        fs.WriteLine("")
                        fs.WriteLine("")
                    End If
                    fs.WriteLine(If(verbose, "1", "0"))
                    fs.WriteLine(If(mitgisdarstellung, "1", "0"))
                    fs.WriteLine(myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung)
                    fs.WriteLine(myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz)
                    fs.WriteLine(myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)
                    fs.WriteLine("")
                Catch ex As Exception
                    nachricht(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl & " ist noch nicht definiert")
                End Try
                fs.Flush()
                fs.Close()
            End Using
        Catch ex As Exception
            nachricht("Fehler in erzeugeParameterDatei: " & ex.ToString)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="vliste"></param>
    ''' <remarks>10 leerzeilen (SQL),dann die vorgangsnummern</remarks>
    Public Shared Sub erzeugeVorgangsListenDatei(ByVal sql As string, ByVal vliste As DataTable)
        Using fs As New IO.StreamWriter(String.Format("{0}\aktvorgangsListe.txt", clstart.mycsimple.Paradigma_local_root))
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            fs.WriteLine(sql)
            Try
                For Each dings As DataRow In vliste.AsEnumerable : fs.WriteLine(dings.Item("vorgangsid").ToString) : Next
            Catch ex As Exception
                nachricht(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl & " ist noch nicht definiert")
            End Try
            fs.Flush()
            fs.Close()
        End Using
    End Sub
End Class
