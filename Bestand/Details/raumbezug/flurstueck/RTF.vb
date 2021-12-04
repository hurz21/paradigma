Namespace Eigentuemer_Tools
    Public Class RTF
        Private STR_Cachealb As String = initP.getValue("ALB.cache")
        Private INT_Wartezeit As Integer = CInt(initP.getValue("ALB.Wartezeit"))  '8000
        Public Property aktflurstueck As clsFlurstueck
        Public Property neuaktflurstueck As clsFlurstueck
        Sub New(ByVal neuflurstueck As clsFlurstueck)
            aktflurstueck = New clsFlurstueck(neuflurstueck.FS)
            aktflurstueck.weistauf = neuflurstueck.weistauf
            aktflurstueck.zeigtauf = neuflurstueck.zeigtauf
            aktflurstueck.gebucht = neuflurstueck.gebucht
            aktflurstueck.fsgml = neuflurstueck.fsgml
            aktflurstueck.FS = neuflurstueck.FS
            aktflurstueck.flaecheqm = neuflurstueck.flaecheqm
            '  aktflurstueck.weistauf = neuflurstueck.weistauf
        End Sub

        Private Function filename_Festlegen(ByVal namensteil2 As String,
                                            ByRef filenameImWebCache As String,
                                            ByRef filenameImLokalenCache As String,
                                            ByVal username As String) As Boolean
            filenameImLokalenCache = filenameImLokalenCache & username & namensteil2 & ".rtf"
            filenameImWebCache = STR_Cachealb & filenameImLokalenCache.Replace("\", "/")
            Dim mastermodus$ = "batch"
            If mastermodus.ToLower = "batch" Then
                filenameImLokalenCache = initP.getValue("ALB.serverUNC") & STR_Cachealb & filenameImLokalenCache
            Else
                filenameImLokalenCache = "d:" & STR_Cachealb & filenameImLokalenCache
            End If
            filenameImLokalenCache = filenameImLokalenCache.Replace("/", "\")

            nachricht("filenameImLokalenCache: " & filenameImLokalenCache)
            nachricht("filenameImWebCache: " & filenameImWebCache)
        End Function

        Private Function sindFlurstuecksDatenVollstaendig() As Boolean
            Dim voll As Boolean
            voll = Not String.IsNullOrEmpty(aktflurstueck.gebucht)
            Return voll
        End Function

        Public Function send_Shellbatch_EIGENTUEMER(ByVal mywebpdf As String,
                                                    ByRef dateisystemPDF As String,
                                                    ByVal username As String,
                                                    ByVal vorgangid As Integer) As Boolean
            nachricht("send_Shellbatch -----------------------------------------")
            nachricht("mywebpdf :" & mywebpdf)
            If myGlobalz.sitzung.aktBearbeiter.username = "FEINEN_DR" Then myGlobalz.sitzung.aktBearbeiter.username = "FEINEN_J"
            Dim datum, namensteil2, exeKutablePfad, Parameter, summa As String
            datum = Year(Now) & Month(Now) & Day(Now) & Hour(Now) & Minute(Now) & Second(Now)

            namensteil2 = datum & "_" & vorgangid & "_" & aktflurstueck.FS & "_"

            filename_Festlegen(namensteil2, mywebpdf, dateisystemPDF, username)

            nachricht("dateisystemPDF :" & dateisystemPDF)
            If IO.File.Exists(dateisystemPDF) Then IO.File.Delete(dateisystemPDF)

            exeKutablePfad = initP.getValue("ExterneAnwendungen.STR_albExe")

            If Not sindFlurstuecksDatenVollstaendig() Then Return False

            Parameter = GetParameter_Eigentuemer(mywebpdf, namensteil2, vorgangid)
            summa = exeKutablePfad & Parameter

            nachricht("shill: " & exeKutablePfad & Parameter)
            Try
                nachricht("pid:" & Microsoft.VisualBasic.Shell(summa).ToString)
                System.Threading.Thread.Sleep(INT_Wartezeit)
                If IO.File.Exists(dateisystemPDF) Then
                    nachricht("1 versuch ")
                    Return True
                Else
                    System.Threading.Thread.Sleep(INT_Wartezeit)
                    If IO.File.Exists(dateisystemPDF) Then
                        nachricht("2 versuch erfolgreich")
                        Return True
                    Else
                        System.Threading.Thread.Sleep(INT_Wartezeit)
                        If IO.File.Exists(dateisystemPDF) Then
                            nachricht("2 versuch erfolgreich")
                            Return True
                        Else
                            MsgBox("RTF-Datei konnte nicht erzeugt werden")
                            Return False
                        End If
                        MsgBox("RTF-Datei konnte nicht erzeugt werden")
                        Return False
                    End If
                End If

            Catch ex As Exception
                nachricht_und_Mbox("Fehler in send_Shellbatch " & ex.ToString)
                Return False
            End Try
        End Function

        'Private Function GetExeKutableDefinieren()           As string
        '    nachricht("Param$$$ :" &  INI_Databases.getALBExeFullPath())
        '    Return  INI_Databases.getALBExeFullPath()
        'End Function

        Private Function GetParameter_Eigentuemer(ByVal mywebpdf$, ByVal namensteil2$, ByVal vorgangid As Integer) As String
            Dim Parameter$ = " mastermodus=batch "
            Try
                Parameter &= "username=" & myGlobalz.sitzung.aktBearbeiter.username & " "
                Parameter &= "password=" & myGlobalz.sitzung.aktBearbeiter.GISPassword & " "
                Parameter &= "FS=" & aktflurstueck.FS & " "
                Parameter &= "FSGML=" & aktflurstueck.fsgml & " "
                Parameter &= "WEISTAUF=" & aktflurstueck.weistauf & " "
                Parameter &= "ZEIGTAUF=" & aktflurstueck.zeigtauf & " "
                Parameter &= "GEBUCHT=" & aktflurstueck.gebucht & " "
                Parameter &= "AREAQM=" & aktflurstueck.flaecheqm & " "
                Parameter &= "TEIL2=" & namensteil2$ & " "
                Parameter &= "AZ=" & vorgangid & " "
                Parameter &= "AUSGABEMODUS=" & "rtf" & " "
                Return Parameter
            Catch ex As Exception
                nachricht("Fehler: GetParameter_Eigentuemer: " & ex.ToString)
                Return "fehler GetParameter_Eigentuemer"
            End Try
        End Function
    End Class
End Namespace
