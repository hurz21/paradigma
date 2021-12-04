Public Class clsVorgangLocking
    Property lockdir As String
    Property vid As Integer

    Property lockfilemaske As String
    Property userid As String
    Property eigenes_lockfile As String
    Property gefundenes_lockfile As String

    Sub New(ByVal _lockdir As String, ByVal _vid As Integer, ByVal _userid As String)
        lockdir = _lockdir
        vid = _vid
        userid = _userid
        eigenes_lockfile = String.Format("{0}{1}_{2}.txt", lockdir, vid, userid)
        lockfilemaske = String.Format("{0}_*.txt", vid)
        '  testfile = New IO.FileInfo(lockfile)
    End Sub
    Function lockingPruefen() As String
        Try
            'Dim lockuser$ = ""
            Dim mydir As New IO.DirectoryInfo(lockdir)
            If Not mydir.Exists Then
                nachricht("Fehler in lockingPruefen : Verzeichnis fehlt! " & lockdir)
                Return ""
            End If
            Dim a() As IO.FileInfo
            a = mydir.GetFiles(lockfilemaske)
            If a.Length < 1 Then
                Return "" 'kein lockuing vorhanden
            Else
                '  Dim lockfile As String
                gefundenes_lockfile = String.Format("{0}{1}", lockdir, a(0))
                Dim testfile As New IO.FileInfo(gefundenes_lockfile)
                If istLockdateiVeraltet(testfile) Then
                    loescheTestfile(testfile)
                    Return ""
                End If
                Dim b As String() = a(0).Name.Replace(".txt", "").Split("_"c)
                Return b(1)
            End If
            'Dim testfile As New IO.FileInfo(lockfilemaske)

        Catch ex As Exception
            nachricht("Fehler: lockingPruefen: " & ex.ToString)
            Return "Fehler: " & ex.ToString
        End Try
    End Function

    Private Sub loescheTestfile(ByVal testfile As IO.FileInfo)
        If testfile.Exists Then
            testfile.Delete()
        End If
    End Sub

    Private Sub lockDateierzeugen(ByVal testfile As IO.FileInfo)
        Try
            Dim sw As New IO.StreamWriter(testfile.FullName)
            sw.WriteLine(userid)
            sw.Close()
            'sw.Dispose()
        Catch ex As Exception
            nachricht(String.Format("Fehler in lockDateierzeugen: {0}{1}{2}. Vermutlich exitiert das verwzcihnis nicht: ", testfile, vbCrLf, ex))
        End Try
    End Sub

    Function lockingSetzen() As Boolean
        Try
            If String.IsNullOrEmpty(userid) Then
                Return False
            End If
            Dim testfile As New IO.FileInfo(eigenes_lockfile)
            If testfile.Exists Then
                'einfach löäschen ist logisdcher weil Exit Subsich um die eigneen datei handelt
                Return False
            Else
                lockDateierzeugen(testfile)
                Return True
            End If
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function LockingLoesen() As Boolean
        Try
            Dim testfile As New IO.FileInfo(eigenes_lockfile)
            If testfile.Exists Then
                testfile.Delete()
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function istLockdateiVeraltet(ByVal testfile As IO.FileInfo) As Boolean
        If testfile.CreationTime.Day <> Now.Day Or
            testfile.CreationTime.Month <> Now.Month Or
            testfile.CreationTime.Year <> Now.Year Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' darf nur vom startprogramm aufgerufen werden
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared Sub alleLocksDesUsersLoesen()
        Dim datei As String = initP.getValue("Haupt.LOCKINGFile")
        Dim filter As String
        Try
            filter = String.Format("*{0}.txt", myGlobalz.sitzung.aktBearbeiter.Initiale)
            ' filter = String.Format("{0}_{1}.txt", vid, myGlobalz.sitzung.aktBearbeiter.Initiale)
            Dim lfiles() As String
            lfiles = IO.Directory.GetFiles(datei, filter)
            For Each datei In lfiles
                Try
                    Dim testfile As New IO.FileInfo(datei)
                    If testfile.Exists Then
                        testfile.Delete()
                    End If
                Catch ex As Exception
                    'next
                End Try
            Next
        Catch ex As Exception

        End Try
    End Sub

End Class
