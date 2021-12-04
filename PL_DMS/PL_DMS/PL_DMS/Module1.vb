Module Module1
    Public Property aktJob As New clsJobDef
    Property ExchangeRoot As String = "d:\Paradigmacache\pl_cache\"
    'Property archivRootDir As String = "\\file-paradigma\paradigma\test\paradigmaArchiv\pl_backup\"
    Property archivRootDir As String = "d:\Paradigma_Archiv\"
    Property mycgi As clsCGI4VBNET
    'http://2w2gis02.kreis-of.local/cgi-bin/apps/neugis/dbgrab/dbgrab.cgi?user=weinachtsmann&modus=biniminternet

    'http://2w2gis02.kreis-of.local/cgi-bin/apps\paradigmaex\dmsserver\dmsserver.cgi?aktion=putinarchiv&usernr=1&anlagejahrmonat=201301&dokid=11&vid=10
    'http://gis02.kreis-of.local/cgi-bin/apps\paradigmaex\dmsserver\dmsserver.cgi?aktion=putinarchiv&usernr=1&anlagejahrmonat=201301&dokid=11&vid=10
    Sub Main()
        Dim result As String = ""
        Dim erfolg As Boolean
        Dim fehler As String = ""
        mycgi = New clsCGI4VBNET("dr.j.feinen@kreis-offenbach.de")
        protokoll()
        l(CType(Now, String))
        l("start :" & Now.ToString)

        getCgiParams()
        '###################
        Dim ArchivZieldir, Archivzieldatei, quelldatei As String
        Dim newdir As IO.DirectoryInfo
        Dim fi As IO.FileInfo

        aktJob.relativdir = aktJob.anlageJahrmonat & "\" & aktJob.usernr & "\" & aktJob.vid & "\" ' "201301/1/1/" '

        l("aktJob.relativdir " & aktJob.relativdir)
        If aktJob.aktion = "putinarchiv" Then
            putinarchiv(result, fehler, ArchivZieldir, Archivzieldatei, quelldatei, newdir, fi)
        End If

        fi = Nothing
        '---------------------
        mycgi.SendHeaderAJAX()
        l("nach SendHeaderAJAX")
        mycgi.Send(result)
        l("ende")
        l(CType(Now, String))
    End Sub

    Private Sub putinarchiv(ByRef result As String, ByRef fehler As String, ByRef ArchivZieldir As String,
                            ByRef ArchivZieldatei As String, ByRef quelldatei As String,
                            ByRef newdir As IO.DirectoryInfo, ByRef fquell As IO.FileInfo)
        Try
            l("putinarchiv---------------------- anfang")
            'verzeichnisse

            ArchivZieldir = archivRootDir & aktJob.relativdir
            l("zieldir " & ArchivZieldir)
            If IO.Directory.Exists(ArchivZieldir) Then
                newdir = New IO.DirectoryInfo(ArchivZieldir)
            Else
                newdir = IO.Directory.CreateDirectory(ArchivZieldir)
            End If
            'dateien
            ArchivZieldatei = ArchivZieldir & aktJob.dokumentid
            quelldatei = ExchangeRoot & aktJob.usernr & "\" & aktJob.dokumentid
            l("zieldatei " & ArchivZieldatei)
            l("quelldatei " & quelldatei)

            If newdir.Exists Then
                l("newdir.Exists  ")
                fquell = New IO.FileInfo(quelldatei)
                If fquell.Exists Then
                    l("Quelldatei existiert")
                    fehler &= overwrite(ArchivZieldatei, fquell)
                    If fehler = "" Then
                        'quelldatei löschen
                        '  fquell.Delete()
                        fquell = Nothing
                        result = "ok gesichert"
                    End If
                Else
                    l("Quelldatei existiert NICHT fehler")
                    fehler &= "fehler Quelldatei existiert nicht "
                    result = "Die Quelldatei fehlt " & fquell.FullName
                End If
            Else
                l("fehler newdir.Exists NICHT ")
                result = "Verzsichnis konnte nicht angelgt werden"
            End If
            l("putinarchiv---------------------- ende")
        Catch ex As Exception
            l("Fehler in putinarchiv: " & ex.ToString())
        End Try
    End Sub

    Private Function overwrite(zieldatei As String, fi As IO.FileInfo) As String
        Dim fehler As String = ""
        Try
            l("overwrite---------------------- anfang")
            fi.CopyTo(zieldatei, True)
            l("overwrite---------------------- ende")
            Return fehler
        Catch ex As Exception
            l("Fehler in overwrite: " & ex.ToString())
            fehler = fehler & "Fehler in overwrite: " & ex.ToString
            Return fehler
        Finally

        End Try
    End Function

    Private Sub getCgiParams()
#If DEBUG Then
        aktJob.aktion = "putinarchiv"
        aktJob.usernr = 1 '"feinen_j"
        aktJob.dokumentid = 1
        aktJob.anlageJahrmonat = "201301"
      

        '############################################
        '            aktJob.aktion = "deleteinarchiv"
        '            aktJob.usernr = "feinen_j"
        '            aktJob.vid = "4711"
        '            aktJob.relativdir = "2013/4711"
        '            aktJob.numericSubDir = "1"
        '            '############################################
        '            aktJob.aktion = "updateinarchiv"
        '            aktJob.usernr = "feinen_j"
        '            aktJob.vid = "4711"
        '            aktJob.relativdir = "2013/4711"
        '            aktJob.numericSubDir = "6"
        '############################################
        '            aktJob.aktion = "getfromarchiv"
        '            aktJob.usernr = "feinen_j"
        '            aktJob.vid = "4711"
        '            aktJob.relativdir = "2013/4711"
        '            aktJob.numericSubDir = "6"

#Else
        aktJob.aktion = mycgi.GetCgiValue("aktion")
        aktJob.usernr = CInt(mycgi.GetCgiValue("usernr"))
        aktJob.vid = (mycgi.GetCgiValue("vid"))
        aktJob.dokumentid = CInt((mycgi.GetCgiValue("dokid")))
        aktJob.anlageJahrmonat = (mycgi.GetCgiValue("anlagejahrmonat"))
        'aktJob.relativdir = (mycgi.GetCgiValue("relativdir"))
        'aktJob.numericSubDir = (mycgi.GetCgiValue("numericSubDir"))
        l(" aktJob.aktion " & aktJob.aktion)
        l(" aktJob.usernr " & aktJob.usernr)
        l(" aktJob.vid " & aktJob.vid)
        l(" aktJob.dokumentid " & aktJob.dokumentid)
        l(" aktJob.anlageJahrmonat " & aktJob.anlageJahrmonat)
        l(" aktJob.relativdir " & aktJob.relativdir)

#End If

    End Sub
    Public Sub l(text As String)
        My.Application.Log.WriteEntry(text)
    End Sub
    Private Sub protokoll()
        With My.Application.Log.DefaultFileLogWriter
#If DEBUG Then
            .CustomLocation = "c:\" & "protokoll"
#Else
            .CustomLocation = "d:\websys\" & "protokoll"
#End If
            .BaseFileName = "PL_dms_" & mycgi.GetCgiValue("dokid")
            .AutoFlush = True
            .Append = False
        End With
    End Sub

End Module
