
Imports System.Net
Imports System.Data
Imports System.IO
Imports System.Windows.Threading
Module clientTools

    Property exchangeRoot As String
    Property VIDdir As String
    Property DMSserverUrl As String
    'Property DMSserverDir As String
    Public Property aktDMSJob As New clsDMSJobDef

    Public Property DMSergebnis As New clsDMSErgebnis
    Public Sub l(text As String)
        My.Log.WriteEntry(text)
    End Sub
    Function builduserdirName(ByVal Root As String,ByVal  username As String) As String
         
        Return IO.Path.Combine(Root, username)
    End Function
    'Private Sub dateiloeschen(ByVal infodateiname As String)
    '    l("dateiloeschen -------------- " & infodateiname)
    '    Dim fi As IO.FileInfo
    '    Try
    '        fi = New IO.FileInfo(infodateiname)
    '        fi.Delete()
    '        l("dateiloeschen -------------- ok ")
    '    Catch ex As Exception
    '        l("fehler in dateilöschen: " & ex.ToString)
    '    End Try
    'End Sub

    Public Sub createAntwortDatei(betreff As String, body As String, resultdir As String)
        Dim infodateiname As String
        l("createInfodatei-----------------------------")
        Try
            infodateiname = resultdir & "\antwort.txt"
            l("infodateiname " & infodateiname)
            Using sr As New IO.StreamWriter(infodateiname)
                sr.WriteLine(betreff)
                sr.WriteLine(body)
            End Using
            l("createInfodateiok ok")
        Catch ex As Exception
            l(" fehler in createInfodatei:  " & ex.ToString)
        End Try
    End Sub

    Friend Function getfilesAusDir(verz As String) As String()
        Dim filesAusDir As String()
        Dim syn As String = verz
        Try
            filesAusDir = IO.Directory.GetFiles(syn)
            If filesAusDir Is Nothing Then
                Return Nothing
            End If
            Return filesAusDir
        Catch ex As Exception
            l("Fehler in getfilesAusDir " & ex.ToString)
            Return Nothing
        End Try
    End Function
    Public Function buildInputfilesArrayPut() As String()
        ReDim aktDMSJob.inputfiles(1)
        aktDMSJob.inputfiles(0) = "C:\Users\Feinen_j\Desktop\Paradigma\Archiv_Checkout\test\Liebe Kolleginnen und Kollegen.docx"
        aktDMSJob.inputfiles(1) = "C:\Users\Feinen_j\Desktop\Paradigma\Archiv_Checkout\test\oracleInfo.docx"
        Return aktDMSJob.inputfiles
        'l:\paradigma\div\DMSexchange\feinen_j\4711\Liebe Kolleginnen und Kollegen.docx
        'l:\paradigma\div\DMSexchange\feinen_j\4711\oracleInfo.docx

    End Function

    Public Function buildInputfilesArrayDelete(relativdir As String, numericDir As String) As String()
        ReDim aktDMSJob.inputfiles(1)
        aktDMSJob.inputfiles(0) = IO.Path.Combine(relativdir, numericDir, "Liebe Kolleginnen und Kollegen.docx").Replace("/", "\")
        aktDMSJob.inputfiles(1) = IO.Path.Combine(relativdir, numericDir, "oracleInfo.docx").Replace("/", "\")
        Return aktDMSJob.inputfiles
        '2013\4711\9\Liebe Kolleginnen und Kollegen.docx
        '2013\4711\9\oracleInfo.docx

    End Function

    Public Function buildInputfilesArrayGET() As String()
        ReDim aktDMSJob.inputfiles(1)
        aktDMSJob.inputfiles(0) = "2013\4711\6\Liebe Kolleginnen und Kollegen.docx"
        aktDMSJob.inputfiles(1) = "2013\4711\6\oracleInfo.docx"
        Return aktDMSJob.inputfiles
        '2013\4711\6\Liebe Kolleginnen und Kollegen.docx
        '2013\4711\6\oracleInfo.docx
    End Function
    Public Function ExchangeDirLeeraeumen(Verzeichnis As String) As Boolean
        l("ExchangeDirLeeraeumen--------------------------------------------")
        Dim fii As IO.FileInfo
        Try
            Dim dateiein As String() = getfilesAusDir(Verzeichnis)
            For Each datei In dateiein
                fii = New IO.FileInfo(datei)
                fii.Delete()
            Next
            Return True
        Catch ex As Exception
            l("fehler in ExchangeDirLeeraeumen: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Function sendjobExtracted(url As String) As String
        Try
            l("sendjobExtracted -----------------------")
            l("url-: " & url)
            Dim myWebRequest As HttpWebRequest = DirectCast(HttpWebRequest.Create(url), HttpWebRequest)
            myWebRequest.Method = "GET"
            Dim myWebResponse As HttpWebResponse = DirectCast(myWebRequest.GetResponse(), HttpWebResponse)
            Dim myWebSource As New StreamReader(myWebResponse.GetResponseStream())
            Dim myPageSource As String = String.Empty
            myPageSource = myWebSource.ReadToEnd()
            myWebResponse.Close()
            l("ergebnis:" & myPageSource)
            l("sendjobExtracted -----------------------Ende")
            Return myPageSource
        Catch ex As Exception
            l("Fehler in sendjobExtracted: " & ex.ToString)
            '  MsgBox("Fehler beim Abspeichern der Dokumente. Der DMS-Server (w2gis02) ist nicht aktiv. Bitte Admin benachrichtigen!") 
            Return "Fehler - server busy?"
        End Try
    End Function

    Public Function sendjobURL(url As String) As String
        Return sendjobExtracted(url)
    End Function
    Public Sub cleandokumentpfadString()
        If aktDMSJob.relativdir.StartsWith("/") Then
            aktDMSJob.relativdir = aktDMSJob.relativdir.Substring(1, aktDMSJob.relativdir.Length - 1)
        End If
        'If FileArchiv.aktJob.relativdir.Contains("/") Then
        '    FileArchiv.aktJob.relativdir = FileArchiv.aktJob.relativdir.Replace("/", "\")
        'End If
    End Sub
    Public Sub createVIDverzeichnis()
        Try
            VIDdir = IO.Path.Combine(exchangeRoot, aktDMSJob.username)
            If Not IO.Directory.Exists(VIDdir) Then IO.Directory.CreateDirectory(VIDdir)
            VIDdir = IO.Path.Combine(exchangeRoot, aktDMSJob.username, aktDMSJob.vid.ToString)
            If Not IO.Directory.Exists(VIDdir) Then IO.Directory.CreateDirectory(VIDdir)
        Catch ex As Exception

        End Try
    End Sub

    Function KopiereDokumenteNachExchange(quellverz As String, exchangedir As String) As Boolean
        If aktDMSJob.aktion <> "put" And aktDMSJob.aktion <> "update" Then Return False
        Dim ifile As IO.FileInfo
        Dim ziel As String
        Try
            If aktDMSJob.inputfiles Is Nothing Then
                l("Fehler keine inpoutfiles bei put oder update")
                Return False
            End If
            For i = 0 To aktDMSJob.inputfiles.GetUpperBound(0)
                ifile = New IO.FileInfo(aktDMSJob.inputfiles(i))
                ziel = IO.Path.Combine(exchangedir, ifile.Name)
                ifile.CopyTo(ziel, True)
            Next
            'dateien kopieren
            Return True
        Catch ex As Exception
            l("Fehler in KopiereDokumenteNachExchange: " & ex.ToString)
            Return False
        End Try
    End Function

    Function erzeugeFileListe(files As String(), modus As String) As Boolean
        Dim listfile As String
        Dim fi As IO.FileInfo
        Try
            listfile = IO.Path.Combine(VIDdir, "_files_.txt")
            Using sw As New IO.StreamWriter(listfile)
                For i = 0 To files.GetUpperBound(0)
                    fi = New IO.FileInfo(aktDMSJob.inputfiles(i))
                    If modus = "delete" Then
                        'zeigt den vollen name ohne pfad der Zieldateien -
                        ' es fehlt noch das Rootverzeichnis =der Srver kennt es als konstante
                        ' das relativdir kann nicht als cgi_parameter übergeben werden, weil 
                        ' bei mehreren dateien immer das relativ-dir unterschiedlich sein kann
                        Dim test As String = IO.Path.Combine(aktDMSJob.relativdir.Replace("/", "\"), aktDMSJob.numericSubDir, fi.Name)
                        '  sw.WriteLine(IO.Path.Combine(resultdir, "files") & "\" & fi.Name)
                        sw.WriteLine(test)
                    End If
                    If modus = "get" Then
                        '   Dim test As String = IO.Path.Combine(aktJob.archivRootDir, aktJob.relativdir)
                        'sw.WriteLine(IO.Path.Combine(responseDir, "files") & "\" & fi.Name)
                        sw.WriteLine(aktDMSJob.inputfiles(i))
                    End If
                    If modus = "put" Then
                        sw.WriteLine(IO.Path.Combine(VIDdir, fi.Name))
                    End If
                    If modus = "update" Then
                        Dim test As String = IO.Path.Combine(aktDMSJob.relativdir.Replace("/", "\"), aktDMSJob.numericSubDir, fi.Name)
                        '  sw.WriteLine(IO.Path.Combine(resultdir, "files") & "\" & fi.Name)
                        sw.WriteLine(test)


                    End If
                Next
            End Using
            Return True
        Catch ex As Exception
            l("Fehler keine erzeugeFileListe" & ex.ToString)
            Return False
        End Try
    End Function


End Module


