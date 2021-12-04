Public Class kfatools
    Public Shared KFAliste As New List(Of clsKfas)
    Public Shared KFAeingangsDir As String '= "O:\UMWELT\B\GISDatenEkom\kfas_onlineformulare_eingang"

    Private Function getXMLfileName(filenames() As String) As String
        Dim fi As IO.FileInfo
        Try
            l(" MOD enthaeltXML anfang")
            For Each datei In filenames
                fi = New IO.FileInfo(datei)
                If fi.Extension.ToLower = ".xml" Then
                    fi = Nothing
                    Return datei
                End If
            Next
            fi = Nothing
            l(" MOD enthaeltXML ende")
            Return ""
        Catch ex As Exception
            l("Fehler in enthaeltXML: " & ex.ToString())
            Return ""
        End Try
    End Function
    Friend Function transIdExtrahieren(xmldatei As String) As String
        Dim rec() As String
        Dim zeile As String
        Dim transid As String = ""
        Try
            rec = IO.File.ReadAllLines(xmldatei)
            For i = 0 To rec.Count - 1
                If rec(i).ToLower.Contains("transid") Then
                    transid = rec(i).Replace("</transId>", "")
                    transid = transid.Replace("<transId>", "")
                    Return transid.Trim
                End If
            Next

            Return ""
        Catch ex As Exception
            l("Fehler in transIdExtrahieren: " & ex.ToString())
            Return ""
        End Try
    End Function

    Friend Function KFAverzeichnisERrstellen(kfaName As String) As Boolean
        Try
            l(" MOD KFAverzeichnisERrstellen anfang")
            If IO.Directory.Exists(kfaName) Then
                Return False
            Else
                IO.Directory.CreateDirectory(kfaName)
            End If
            l(" MOD KFAverzeichnisERrstellen ende")
            Return True
        Catch ex As Exception
            l("Fehler in MOD: " & ex.ToString())
            Return False
        End Try
        Return True
    End Function

    Friend Function rueberkopieren(filenames() As String, zielVerzeichnis As String) As Boolean
        Dim zieldatei As String = ""
        Dim fi As IO.FileInfo
        Try
            l(" MOD rueberkopieren anfang")
            For Each datei As String In filenames
                fi = New IO.FileInfo(datei)
                zieldatei = IO.Path.Combine(zielVerzeichnis, fi.Name)
                IO.File.Copy(datei, zieldatei, True)

            Next
            l(" MOD rueberkopieren ende")
            Return True
        Catch ex As Exception
            l("Fehler in rueberkopieren: " & ex.ToString())
            Return False
        End Try
        Return True
    End Function

    Friend Function genKFAverzName(kfaName As String, kfaroot As String) As String
        Return IO.Path.Combine(kfatools.KFAeingangsDir, kfaName)
    End Function
    Public Sub handleDrop(e As DragEventArgs)
        Dim filenames As String()
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            filenames = CType(e.Data.GetData(DataFormats.FileDrop), String())
        End If
        If filenames.Count < 1 Then
            MessageBox.Show("Keine Dateien übergeben")
        End If
        'kfatools.eingangsDir)
        Dim xmldatei As String
        xmldatei = getXMLfileName(filenames)
        If xmldatei.IsNothingOrEmpty Then
            MessageBox.Show("Keine KFA-Datei (.xml) vorhanden! Abbruch.  Hinweis: Sie müssen alle zum Vorgang gehörigen Dateien gleichzeitig einchecken !")
        Else
            Dim transId, transIdVerzeichnisname As String
            'Dim www As New Paradigma_start.Win1Tools
            transId = transIdExtrahieren(xmldatei)
            transIdVerzeichnisname = genKFAverzName(transId, kfatools.KFAeingangsDir)
            If KFAverzeichnisERrstellen(transIdVerzeichnisname) Then
                If rueberkopieren(filenames, transIdVerzeichnisname) Then
                    MessageBox.Show("Die Daten wurden in das Vorgangs-Verzeichnis übertragen: " & transIdVerzeichnisname, "Kopie erfolgreich")
                Else
                    MessageBox.Show("Kopieren hat nicht gefunzt: " & transIdVerzeichnisname, "Kopieren gescheitert")
                End If
            Else
                MessageBox.Show("Verzeichnis lies sich nicht erstellen: " & transIdVerzeichnisname & Environment.NewLine &
                                "Ursache: Vermutlich existiert der Vorgang hier bereits. Bitte prüfen!" & Environment.NewLine &
                                "Abhilfe: Löschen Sie das Zielverzeichnis.", "Verzeichnis schon vorhanden ?")
            End If
        End If
    End Sub
    Friend Shared Function getDict(file As String) As Dictionary(Of String, String)
        Dim bla As New Dictionary(Of String, String)
        Try
            l(" MOD getDict anfang")
            If istValideXMLdatei(file) Then
            Else

            End If
            bla = CLstart.clsINIXML.XMLiniReader2(file)
            l(" MOD getDict ende")
            Return bla
        Catch ex As Exception
            l("Fehler in getgetDictPaare: ", ex)
            Return bla
        End Try
    End Function

    Private Shared Function istValideXMLdatei(file As String) As Boolean
        Dim recs() As String
        Dim strind As New Text.StringBuilder
        Try
            l(" MOD istValideXMLdatei anfang")
            recs = IO.File.ReadAllLines(file)
            If recs(0).ToLower.StartsWith("this xml file does no") Then
                For i = 1 To recs.Count - 1
                    strind.Append(recs(i) & Environment.NewLine)
                Next
                IO.File.WriteAllText(file, strind.ToString)
            End If
            l(" MOD istValideXMLdatei ende")
            Return True
        Catch ex As Exception
            l("Fehler in istValideXMLdatei: ", ex)
            Return False
        End Try
    End Function


    'Shared Sub ZeigeOnlineformListe()

    '    '# einlesen aus dem formular von frau klotz
    '    Dim bla As New Dictionary(Of String, String)
    '    'bla = CLstart.clsINIXML.XMLiniReader2("C:\Users\feinen_j\Desktop\formualr\klingler.xml")
    '    bla = CLstart.clsINIXML.XMLiniReader2("C:\Users\feinen_j\Desktop\formualr\klingler.xml")
    '    Dim summe As String = ""
    '    For Each it In bla
    '        If it.Key.ToLower = "fileurl" Then Continue For
    '        summe = summe & it.Key & ": " & it.Value & Environment.NewLine
    '    Next
    '    MsgBox(summe)
    'End Sub

    Friend Shared Function getPaare(bla As Dictionary(Of String, String)) As String
        Dim summe As New Text.StringBuilder
        Try
            l(" MOD getPaare anfang")
            For Each it In bla
                If it.Key.ToLower = "fileurl" Then Continue For
                summe.Append(it.Key & ": " & it.Value & Environment.NewLine)
            Next
            l(" MOD getPaare ende")
            Return summe.ToString
        Catch ex As Exception
            l("Fehler in getPaare: ", ex)
            Return ""
        End Try
    End Function

    Friend Shared Function getPerson(bla As Dictionary(Of String, String), prefix As String) As Person
        Dim pers As New Person
        pers.clear()

        Try
            l(" MOD getPerson anfang")
            'bla = CLstart.clsINIXML.XMLiniReader2("C:\Users\feinen_j\Desktop\formualr\klingler.xml")
            'bla = CLstart.clsINIXML.XMLiniReader2(file)
            If prefix.ToLower = "as_" Then
                pers.Rolle = "Antragsteller/in"
            End If
            For Each it In bla
                If it.Key.ToLower = "fileurl" Then Continue For
                If it.Key.ToLower = prefix.ToLower & "name" Then
                    pers.Name = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "anrede" Then
                    pers.Anrede = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "vorname" Then
                    pers.Vorname = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "strasse" Then
                    pers.Kontakt.Anschrift.Strasse = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "hausnummer" Then
                    pers.Kontakt.Anschrift.Hausnr = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "plz" Then
                    pers.Kontakt.Anschrift.PLZ = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "gemeinde" Then
                    pers.Kontakt.Anschrift.Gemeindename = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "e-mail" Then
                    pers.Kontakt.elektr.Email = it.Value.Trim
                End If
                If it.Key.ToLower = prefix.ToLower & "telefon" Then
                    pers.Kontakt.elektr.Telefon1 = it.Value.Trim
                End If
            Next
            l(" MOD getPerson ende")
            Return pers
        Catch ex As Exception
            l("Fehler in getPerson: ", ex)
            Return pers
        End Try
    End Function

    Public Function getvorgaenge(dirmy As String, rootdir As String) As List(Of clsKfas)
        Dim fi As IO.FileInfo
        Dim newkfas As clsKfas
        Dim lokaleListe As New List(Of clsKfas)
        Dim dirlist As String() = IO.Directory.GetDirectories(dirmy, "KFAS*")
        Try
            l(" MOD getForms anfang")
            For Each verz In dirlist
                Dim beilagenroh As String() = IO.Directory.GetFiles(verz, "*.*")

                newkfas = New clsKfas
                holeDateien(newkfas, beilagenroh)
                fi = New IO.FileInfo(newkfas.dateiname)
                newkfas.transId = getTransidFromVerzeichnis(verz, rootdir)
                newkfas.kfa_typ = getKfatyp(newkfas.transId)
                newkfas.kfa_typ_klartext = getKfatypKlartext(newkfas.kfa_typ)
                newkfas.verzeichnis = verz
                newkfas.dateikurzname = fi.Name
                newkfas.dateidatum = fi.CreationTime
                newkfas.dict = kfatools.getDict(newkfas.dateiname)
                newkfas.paare = kfatools.getPaare(newkfas.dict)
                newkfas.AS_person = kfatools.getPerson(newkfas.dict, "as_")
                lokaleListe.Add(newkfas)
                l(" MOD getForms ende")
            Next
            Return lokaleListe
        Catch ex As Exception
            l("Fehler in getForms: " & ex.ToString())
            Return lokaleListe
        End Try
    End Function

    Private Shared Function getKfatypKlartext(kfa_typ As String) As String
        Dim res As String = ""
        Try
            l(" MOD getKfatypKlartext anfang")
            Select Case kfa_typ.ToLower
                Case "kfas_67_006"
                    res = "Auskunft a. Altlastenv."
                Case "kfas_67_007"
                    res = "Baumfällung"
                Case "kfas_67_008"
                    res = "Lärmschutz"
                Case Else
                    res = "nummer unbekannt: " & kfa_typ
            End Select
            l(" MOD getKfatypKlartext ende")
            Return res
        Catch ex As Exception
            l("Fehler in getKfatypKlartext: " & ex.ToString())
            Return res
        End Try
    End Function

    Private Shared Function getKfatyp(transId As String) As String
        Dim res, a() As String
        Try
            l(" MOD getKfatyp anfang")
            a = transId.Split("-"c)
            res = a(0)
            l(" MOD getKfatyp ende")
            Return res
        Catch ex As Exception
            l("Fehler in getKfatyp: " & ex.ToString())
            Return ""
        End Try
    End Function

    Private Shared Function getTransidFromVerzeichnis(verz As String, rootdir As String) As String
        Dim result As String
        Try
            l(" MOD getTransidFromVerzeichnis anfang")
            result = verz.Replace(rootdir, "")
            'result = verz.ToLower.Replace(rootdir.ToLower, "")
            result = result.Replace("\", "")
            l(" MOD getTransidFromVerzeichnis ende")
            Return result
        Catch ex As Exception
            l("Fehler in getTransidFromVerzeichnis: " & ex.ToString())
            Return ""
        End Try
    End Function

    Private Shared Sub holeDateien(ByRef newkfas As clsKfas, beilagenroh() As String)
        Dim icnt As Integer = -1
        Try
            l(" MOD holeDateien anfang")
            For i = 0 To beilagenroh.Count - 1
                If beilagenroh(i).ToLower.EndsWith(".xml") Then
                    newkfas.dateiname = beilagenroh(i)
                Else
                    icnt += 1
                    ReDim Preserve newkfas.beilagen(icnt)
                    newkfas.beilagen(icnt) = beilagenroh(i)
                End If
            Next
            l(" MOD holeDateien ende")
        Catch ex As Exception
            l("Fehler in holeDateien: " & ex.ToString())
        End Try
    End Sub

End Class
