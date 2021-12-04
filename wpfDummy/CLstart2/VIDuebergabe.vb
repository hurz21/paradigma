Namespace CLstart
    Public Class VIDuebergabe
        'aufruf CLstart.VIDuebergabe.erzeugeParameterDatei(vorgangsnr, myGlobalz.sitzung.aktBearbeiter.Initiale, CLstart.mycSimple.Paradigma_local_root)
        Public Shared Sub holedetailVonVorgang(vorgangsnr As Integer, userid As String)
            erzeugeParameterDatei(vorgangsnr, userid, mycSimple.Paradigma_local_root, "aktvorgang2")
            ' Dim modul As String = "O:\UMWELT -PARADIGMA\div\deployxcdetail\bin\debug\paradigmadetail.exe "
            'Dim modul As String = mycSimple.getModuleParadigmaDetail()
            'Dim param As String = " /vid=" & vorgangsnr
            'Process.Start(modul, param) 
            Dim si As New ProcessStartInfo
            si.FileName = mycSimple.getModuleParadigmaDetail()
            si.WorkingDirectory = "c:\kreisoffenbach\paradigmadetail"

            si.FileName = "c:\kreisoffenbach\paradigmadetail\paradigmadetail.exe "
            si.WorkingDirectory = "c:\kreisoffenbach\paradigmadetail"
            si.Arguments = " /vid=" & vorgangsnr
            Process.Start(si)
            si = Nothing
        End Sub

        Public Shared Sub erzeugeParameterDatei(ByVal vorgangsid As Integer, userid As String, verz As String, dateivorName As String)
            Try
                Using fs As New IO.StreamWriter(String.Format("{0}\" & dateivorName & ".txt", verz)) 'clstart.mycsimple.Paradigma_local_root))
                    fs.WriteLine(vorgangsid.ToString)
                    fs.WriteLine(userid.ToString)
                End Using
            Catch ex As Exception

            End Try
        End Sub

        Shared Function parameterEinlesen(ByRef userid As String, verz As String, dateivorname As String) As Integer
            Try
                Dim vorgangsid As Integer = 0
                Dim datei$ = String.Format("{0}\" & dateivorname & ".txt", verz) 'clstart.mycsimple.Paradigma_local_root))
                Dim testdatei As New IO.FileInfo(datei)
                If testdatei.Exists Then
                    Using fs As New IO.StreamReader(datei)
                        vorgangsid = CInt(fs.ReadLine())
                        userid = (fs.ReadLine())
#If DEBUG Then
                        '    userid = "schj"
#End If
                    End Using
                    testdatei = Nothing
                    Return vorgangsid
                Else
                    testdatei = Nothing
                    Return -1
                End If
            Catch ex As Exception
                Return 0
            End Try
        End Function
    End Class
End Namespace