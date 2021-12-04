Imports System.Data
Imports System.Threading
Imports LibDB.LibDB
''' <summary>
''' Dokumentenarchiv
''' Beim neuanlegen einen Vorganges wird der ARCDIR des vorgangs festgelegt (vorgangsdir)
'''           das ARCDIR wird in den Stammdaten gespeichert.
''' Sobald ein Dokument gespeichert wird, wird das ARCDIR aus den Stammdaten verwendet um einen neuen Pfad zu bilden
'''       dieser Dokumentpfad wird in der TAB Dokument (um das numdir ergänzt) gespeichert als RELATIVPFAD
''' zum laden der dokus wird einfach addiert: rootdir+relativpfad(aus dokumentetab) + dateiname (aus dokumentetab)
''' 
''' </summary>
''' <remarks></remarks>
 Public Class DokArc
    Implements IDisposable
#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                _vorgangFotoDt.Dispose()
                _vorgangDocDt.Dispose()
                _ereignisDocDt.Dispose()
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
    Private _aktTiefe As Integer
    Private _relativpfad As String
    Private _verz_max_max As Integer = 500 ' 200
    Private _datei_max As Integer = 10000 '500	'200
    Private Shared Radius As Integer = 200
    Public hinweis As String = ""
    Public Property lokalerCheckoutcache() As String
    Sub New()
    End Sub
    Public Sub New(ByVal rootDirtext As String, ByVal lokcheckoutcache As String)
        rootDir = New IO.DirectoryInfo(rootDirtext)
        lokalerCheckoutcache = lokcheckoutcache
    End Sub

    Public Property rootDir() As System.IO.DirectoryInfo
    Public Property subdirmax() As Integer
    Public Property ArcRec() As IDB_grundfunktionen

    Private _vorgangFotoDt As New DataTable
    Public Property vorgangFotoDt() As DataTable
        Get
            Return _vorgangFotoDt
        End Get
        Set(ByVal Value As DataTable)
            _vorgangFotoDt = Value
        End Set
    End Property



    Private _vorgangDocDt As New DataTable
    Public Property vorgangDocDt() As DataTable
        Get
            Return _vorgangDocDt
        End Get
        Set(ByVal Value As DataTable)
            _vorgangDocDt = Value
        End Set
    End Property

    Private _ereignisDocDt As New DataTable
    Public Property ereignisDocDt() As DataTable
        Get
            Return _ereignisDocDt
        End Get
        Set(ByVal Value As DataTable)
            _ereignisDocDt = Value
        End Set
    End Property

    'Private Property korrigiere3ga23gp As Boolean

    Private Function getNeuenNamenFuerDouble(ByRef dateiname As String, ByVal ZielGesamtpfad As String) As String ', ByRef ZielDateiFullName As String
        Try
            Dim testname As String = dateiname
            Dim f As IO.FileInfo
            Dim ZielDateiFullName As String
            For i = 1 To 10000
                f = New IO.FileInfo(testname)
                ZielDateiFullName = ZielGesamtpfad & "\v" & i.ToString & "_" & f.Name
                Dim testt As New IO.FileInfo(ZielDateiFullName)
                If testt.Exists Then
                    Continue For
                Else
                    Return ZielDateiFullName
                End If
            Next
            nachricht("fehler in getNeuenNamenFuerDouble: schleife mit mehr als 10000 turns")
            Return dateiname
        Catch ex As Exception
            nachricht("Fehler: in	getNeuenNamenFuerDouble: " & dateiname & " " & vbCrLf & ex.ToString)
            Return dateiname
        End Try
    End Function

    'Private Shared Function QuelleLoeschenFallsMoven(ByVal Dokumente_moven As Boolean, ByVal OriginalFullname As String) As Boolean
    '    Try
    '        If Dokumente_moven Then
    '            'löschen der originaldatei
    '            Dim ds As New IO.FileInfo(OriginalFullname)
    '            ds.Delete()
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Die Datei wurde ins Archiv kopiert." & Environment.NewLine &
    '                                 "Die Quelldatei ließ sich nicht löschen! Ggf. war sie noch geöffnet !?." & Environment.NewLine &
    '                                 "Sie können die Datei von Hand löschen: " & OriginalFullname)
    '        nachricht("Fehler: in	loeschenFallsMoven: " & vbCrLf & ex.ToString)
    '        Return False
    '    End Try
    'End Function

    Private Sub erzeugeUnterverzeichnis(ByVal relativpfad$)
        Try
            IO.Directory.CreateDirectory(relativpfad)
        Catch ex As Exception
            nachricht("Fehler in: erzeugeUnterverzeichnis: " & ex.ToString)
        End Try
    End Sub


    ''' <summary>
    ''' 1. Speichert die Datei Und
    ''' 2. liefert den Dateinamen im archiv zurück und
    ''' 3. liefert den relativen pfad zurück
    ''' archivname ist beim eintrill noch leer, wird hier erst gebildet
    ''' </summary>
    ''' <param name="Archivname"></param> 
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function checkIN_FileArchiv(ByVal QuelleFullname As String, _
                                     ByRef Archivname As String, _
                                     ByRef erfolgreich As Boolean,
                                     ByRef NumDir As String,
                                     ByVal ArchivSubdir As String) As String
        nachricht("checkIN_FileArchiv: input  OriginalFullname: " & QuelleFullname)
        nachricht("checkIN_FileArchiv: input  Archivname: " & Archivname)
        nachricht("checkIN_FileArchiv: input  erfolgreich: " & erfolgreich)
        nachricht("checkIN_FileArchiv: input  NumDir: " & NumDir)
        Dim result As MessageBoxResult
        Dim dokumentpfad As String
        Dim ZielGesamtpfad As String
        Dim ZielDateiFullName As String = ""
        Dim Fquell As IO.FileInfo
        dokumentpfad = ArchivSubdir & NumDir 'myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir
        ZielGesamtpfad = myGlobalz.Arc.rootDir.ToString & dokumentpfad'myGlobalz.Arc.rootDir

        erfolgreich = False
        erzeugeUnterverzeichnis(ZielGesamtpfad)
        nachricht("in checkIN_FileArchiv")
        Try
            Fquell = New IO.FileInfo(QuelleFullname)
            Dim normname As String = LIBgemeinsames.clsString.normalize_Filename(Fquell.Name)
            ZielDateiFullName = ZielGesamtpfad & "\" & normname
            Dim Fziel As New IO.FileInfo(ZielDateiFullName)
            If Fziel.Exists Then
                nachricht("Datei existiert schon:" & Fziel.FullName)
                ZielDateiFullName = getNeuenNamenFuerDouble(normname, ZielGesamtpfad)
                Fziel = New IO.FileInfo(ZielDateiFullName)
                nachricht(Fquell.FullName & ": Datei existiert schon im Archiv. Wurde umbenannt: " & Fziel.FullName)
                Try
                    If Fquell.Exists Then
                        ' IO.File.Copy(OriginalFullname, ZielDateiFullName)
                        Fquell.CopyTo(Fziel.FullName, True)
                        erfolgreich = True
                        nachricht("Kopieren ins Archiv: " & Fquell.FullName & " mit neuem Namen:" & Fziel.FullName)
                        Archivname = Fziel.FullName
                    Else
                        nachricht_und_Mbox("Quelldatei existiert nicht: " & Fquell.FullName & " Keine Aktion")
                    End If
                Catch ex As Exception
                    nachricht("FEhler 1Kopieren ins Archiv gescheitert!")
                    Archivname = ""
                End Try
            Else
                Try
                    IO.File.Copy(Fquell.FullName, Fziel.FullName)
                    erfolgreich = True
                    nachricht("Kopieren ins Archiv: " & Fquell.FullName)
                    Archivname = Fziel.FullName
                Catch ex As Exception
                    nachricht("FEhler 2Kopieren ins Archiv gescheitert!")
                    Archivname = ""
                End Try
            End If
            nachricht("checkIN_FileArchiv: output  Archivname: " & Archivname)
            nachricht("checkIN_FileArchiv: output  dokumentpfad: " & dokumentpfad)
            Return dokumentpfad 'ist unverändert: warum wird das wieder zurückgeliefert?
        Catch ex As Exception
            nachricht("FEhler checkIN_FileArchiv: FEHLER  OriginalFullname: " & Fquell.FullName)
            result = MessageBox.Show("Diese Datei existiert schon: " & Fquell.FullName & vbCrLf & "Kopie anlegen ?", _
             "Einchecken von Dokumenten", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
            If result = MessageBoxResult.Yes Then
                Try
                    ' IO.File.Copy(dateiname , dateiganz, True)
                Catch ex2 As Exception
                    nachricht("FEhler checkIN_FileArchiv: FEHLER2  OriginalFullname" & Fquell.FullName)
                    MessageBox.Show("Fehler beim überschreiben. Die Datei wird ggf. von einem anderen Programm benutzt!" & vbCrLf & ex2.ToString)
                    Return "Fehler"
                End Try
            End If
            Archivname = ""
            Return dokumentpfad
        End Try
    End Function

    'Public Overloads Function checkINDoku(ByVal dateiname As String,
    '                                      ByVal ereignisID As Integer,
    '                                     ByVal Beschreibung As String,
    '                                     ByVal zielVID As Integer,
    '                                     ByVal fotoZuRaumbezug As Boolean,
    '                                     ByRef NumDir As String,
    '                                     ByVal dateidatum As Date,
    '                                     ByRef dokid As Integer,
    '                                     ByVal archivsubdir As String) As Boolean
    '    nachricht("archiv-checkIN ------------------------------------------")
    '    nachricht("dateiname$" & " - " & ereignisID)
    '    Try
    '        'If checkINDoku(dateiname, Beschreibung, ereignisID, False, "", myGlobalz.sitzung.getArchivSubdir(zielVID), zielVID, fotoZuRaumbezug) Then
    '        If checkINDoku(dateiname, Beschreibung, ereignisID, False, "", NumDir, zielVID, fotoZuRaumbezug, dateidatum, dokid, archivsubdir) Then
    '            nachricht("checkin erfolgreich")
    '            Return True
    '        Else
    '            nachricht("checkin nicht erfolgreich")
    '            Return False
    '        End If
    '    Catch ex As Exception
    '        nachricht("checkIN dateiname")
    '        Return False
    '    End Try
    'End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="OriginalFullname"> mit pfad und allem schnickschnack</param>
    ''' <param name="Beschreibung"></param>
    ''' <param name="ereignisID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    'Public Overloads Function checkINDoku(ByVal OriginalFullname As String, _
    '                                    ByVal Beschreibung As String, _
    '                                    ByVal ereignisID As Integer, _
    '                                    ByVal Dokumente_moven As Boolean, _
    '                                    ByRef archivDateiFullname As String,
    '                                    ByRef NumDir As String,
    '                                    ByVal lokVorgangsID As Integer,
    '                                    ByVal fotoZuRaumbezug As Boolean,
    '                                        dateidatum As Date,
    '                                        ByRef neudokid As Integer,
    '                                        ByVal ArchivSubdir As String) As Boolean
    '    Dim test, erfolgreich As Boolean
    '    '  Dim ArchivDateiFullname =  ""
    '    nachricht("IN Checkin: ---------------------")
    '    Try
    '        _relativpfad = checkIN_FileArchiv(OriginalFullname, archivDateiFullname, erfolgreich,
    '                                          NumDir, ArchivSubdir)
    '        '_relativpfad = checkIN_FileArchivNEU(OriginalFullname, archivDateiFullname, erfolgreich,
    '        '                                subdircount)

    '        If erfolgreich Then QuelleLoeschenFallsMoven(Dokumente_moven, OriginalFullname)

    '        myGlobalz.sitzung.aktDokument.dokumentPfad = _relativpfad
    '        If String.IsNullOrEmpty(_relativpfad) Or archivDateiFullname = "" Then
    '            nachricht_und_Mbox(String.Format("FEHLERin Checkin: relativpfad oder Archivdateifullname sind unbrauchbar:  {0}rel:{1}{0}archivfullname:{2}", vbCrLf, _relativpfad,
    '                                                   archivDateiFullname))
    '            'kein schreibzugriff 
    '            Return False
    '        Else
    '            Dim aktJPG As New clsMyJPG
    '            aktJPG.fullname = archivDateiFullname
    '            test = checkIN_Database(aktJPG, _relativpfad, Beschreibung, ereignisID, OriginalFullname, lokVorgangsID, fotoZuRaumbezug, dateidatum, neudokid)
    '            Return test
    '        End If
    '        nachricht("IN Checkin: ------------------ raus")
    '    Catch ex As Exception
    '        nachricht_und_Mbox(String.Format("Fehler in	checkIN	dateiname${0}{1}", vbCrLf, ex))
    '    End Try
    'End Function

    Shared Sub BeschreibungInRB_mappen(ByVal aktJPG As clsMyJPG, ByVal Beschreibung$)
        If String.IsNullOrEmpty(aktJPG.name) Then aktJPG.name = Beschreibung
    End Sub

    'Private Function FotosExifpruefen(ByVal aktJPG As clsMyJPG, ByVal Beschreibung As String) As Boolean
    '    Try
    '        Dim test As New IO.FileInfo(aktJPG.fullname)
    '        If test.Extension.ToLower = ".jpg" Then
    '            Dim ExifQ As ExifWorksQuick = Nothing
    '            If hatExif(aktJPG, ExifQ) Then
    '                If aktJPG.EXIFDateTimeOriginal = #12:00:00 AM# Then aktJPG.EXIFDateTimeOriginal = test.CreationTime
    '                BeschreibungInRB_mappen(aktJPG, Beschreibung$)
    '            Else
    '                'damit NUr JPGS als Raumbezug gewertet werden
    '                aktJPG.Exifgpslatitude = ""
    '                aktJPG.Exifgpslongitude = ""
    '                Return False
    '            End If
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht(String.Format("	FotosExifpruefen	{0}{1}", vbCrLf, ex))
    '        Return False
    '    End Try
    'End Function

    Shared Function inRaumbezuguebernehmen(ByVal aktJPG As clsMyJPG, ByVal lokvid As Integer) As Boolean
        Dim r = "", h As String = ""
        Dim quellnotation As Integer = 1
        If aktJPG.EXIFhersteller.ToLower = "apple" Then
            quellnotation = 32
        End If

        If aktJPG.EXIFhersteller.ToLower = "ricoh" Then
            quellnotation = 1
        End If
        'ihah
        'If LIBcoordumrechnung.clsKoordumrechnung.umrechneninUTM32_ausWGS84(aktJPG.Exifgpslongitude, aktJPG.Exifgpslatitude, r, h, quellnotation) Then
        '    aktJPG.rechts = r
        '    aktJPG.hoch = h
        '    FotoDok_alsRaumbezugSpeichernNeu(aktJPG, lokvid)
        '    Return True
        'Else
        '    Return False
        'End If
        Return True
    End Function



    'Private Shared Function FotoDok_alsRaumbezugSpeichernNeu(ByVal aktJPG As clsMyJPG, ByVal lokvid As Integer) As Boolean
    '    Dim parafotoID%
    '    Try
    '        myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Foto
    '        myGlobalz.sitzung.aktParaFoto.typ = RaumbezugsTyp.Foto
    '        myGlobalz.sitzung.raumbezugsmodus = "neu"
    '        myGlobalz.sitzung.aktParaFoto.clear()
    '        parafotoID = RBtoolsns.FotoNeuSpeichern_alleDB.execute(aktJPG)
    '        If parafotoID > 0 Then
    '            myGlobalz.sitzung.aktParaFoto.SekID = parafotoID
    '            myGlobalz.sitzung.aktParaFoto.defineAbstract(aktJPG)
    '            setPointsFoto(aktJPG)
    '            myGlobalz.sitzung.aktParaFoto.name = aktJPG.name
    '            DBraumbezug_Mysql.defineBBOX(Radius, myGlobalz.sitzung.aktParaFoto)
    '            Dim raumbezugsID% = RBtoolsns.Raumbezug_abspeichern_Neu_alleDB.execute(myGlobalz.sitzung.aktParaFoto)
    '            myGlobalz.sitzung.aktParaFoto.RaumbezugsID = raumbezugsID
    '            Dim koppelungsID4% = RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(CInt(myGlobalz.sitzung.aktParaFoto.RaumbezugsID), lokvid%, 0)
    '            My.Log.WriteEntry(" Koppelung_Raumbezug_Vorgang:" & koppelungsID4% & " ")
    '            'RB_append_PointShapeFile(myGlobalz.sitzung.VorgangsID,myGlobalz.sitzung.aktfoto.RaumbezugsID,myGlobalz.sitzung.aktfoto.SekID)
    '            Return True
    '        Else
    '            nachricht("Problem beim FotoDok_alsRaumbezugSpeichernNeu!")
    '        End If
    '    Catch ex As Exception
    '        nachricht(String.Format("Problem beim FotoDok_alsRaumbezugSpeichernNeu!{0}{1}", vbCrLf, ex))
    '        Return False
    '    End Try
    'End Function

    Shared Sub setPointsFoto(ByVal akJPG As clsMyJPG)
        Try
            myGlobalz.sitzung.aktParaFoto.punkt.X = CDbl(akJPG.rechts)
            myGlobalz.sitzung.aktParaFoto.punkt.Y = CDbl(akJPG.hoch)
        Catch ex As Exception
            nachricht(String.Format("Fehler setPointsFoto!{0}{1}", vbCrLf, ex))
        End Try
    End Sub




    Function hatExif(ByVal akJPG As clsMyJPG, ByRef ExifQ As ExifWorksQuick) As Boolean
        Return If(getplainExifdata(akJPG, ExifQ), True, False)
    End Function

    Shared Function hasCoords(ByVal akJPG As clsMyJPG) As Boolean
        Try
            If akJPG.Exifgpslongitude = "0#0#0#" Or akJPG.Exifgpslatitude = "0#0#0#" Then
                Return False
            End If
            Return True
        Catch ex As Exception
            nachricht(String.Format("in hatCoords: {0}{1}", vbCrLf, ex))
        End Try
    End Function
    'Private Shared Sub kopplenMitVorgang(ByVal zielVorgangsid As Integer, ByVal dokumentID As Integer)
    '    Try
    '        Dim koppvorg As Integer = DokArcTools.KopplungMitVorgang.execute(dokumentID, zielVorgangsid)
    '        If koppvorg > 0 Then
    '            nachricht("in		checkIN_Database KopplungMitVorgang,  erfolgreich")
    '        Else
    '            nachricht("in		checkIN_Database KopplungMitVorgang, NICHT erfolgreich")
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in kopplenMitVorgang: ", ex)
    '    End Try
    'End Sub
    'Private Shared Sub kopplenMitEreignis(ByVal ereignisID As Integer, ByVal dokumentID As Integer)
    '    Try
    '        If ereignisID > 0 Then
    '            nachricht("in		checkIN_Database KopplungMitEreignis-----------------------------------------------------")
    '            Dim koppereignis As Integer = DokArcTools.KopplungMitEreignis.execute(dokumentID, ereignisID)
    '            If koppereignis > 0 Then
    '                nachricht("in		checkIN_Database KopplungMitEreignis erfolgreich")
    '            Else
    '                nachricht("in		checkIN_Database KopplungMitEreignis NICHT erfolgreich")
    '            End If
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in kopplenMitEreignis: ", ex)
    '    End Try
    'End Sub
    'Private Shared Sub FotoMitExifKoordinate(ByVal aktjpg As clsMyJPG, ByVal zielVorgangsid As Integer, ByVal fotoZuRaumbezug As Boolean, ByVal test As IO.FileInfo)
    '    Try
    '        If test.Extension.ToLower = ".jpg" And hasCoords(aktjpg) Then
    '            If fotoZuRaumbezug Then
    '                inRaumbezuguebernehmen(aktjpg, zielVorgangsid)
    '            End If
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in FotoMitExifKoordinate: ", ex)
    '    End Try
    'End Sub
    'Private Overloads Shared Function checkin_Dokumente(ByVal dateiname$, ByVal relativpfad$, ByVal Beschreibung$,OriginalFullname$,OriginalName as string) as  Integer
    '	Try
    '		Dim aktjpg As New clsMyJPG
    '		aktjpg.fullname = dateiname
    '		checkin_Dokumente(aktjpg, relativpfad, Beschreibung,OriginalFullname$,OriginalName)
    '	Catch ex As Exception
    '		nachricht("checkin_Dokumente dateiname$ " & vbCrLf & ex.ToString)
    '	End Try
    'End Function

    'Public Function checkIN_Database(ByVal aktjpg As clsMyJPG, _
    '                                    ByVal relativpfad As String, _
    '                                    ByVal Beschreibung As String, _
    '                                    ByVal ereignisID As Integer, _
    '                                    ByVal OriginalFullname As String,
    '                                    ByVal zielVorgangsid As Integer,
    '                                    ByVal fotoZuRaumbezug As Boolean,
    '                                        dateidatum As Date,
    '                                        ByRef neudokumentID As Integer) As Boolean
    '    Dim fotodatei As New IO.FileInfo(aktjpg.fullname)
    '    Dim oritest As New IO.FileInfo(OriginalFullname)
    '    Dim OriginalName = oritest.Name
    '    nachricht("in		checkIN_Database ---------------------------")
    '    FotosExifpruefen(aktjpg, Beschreibung)
    '    neudokumentID = DokArcTools.dokSpeichernNeu.execute(aktjpg, relativpfad, Beschreibung, OriginalFullname, OriginalName, dateidatum)
    '    If neudokumentID > 0 Then
    '        nachricht("in		checkIN_Database ---------------------------   erfolgreich")
    '        aktjpg.DokumentID = neudokumentID
    '        FotoMitExifKoordinate(aktjpg, zielVorgangsid, fotoZuRaumbezug, fotodatei)
    '        nachricht("in		checkIN_Database KopplungMitVorgang-----------------------------------------------------")
    '        kopplenMitVorgang(zielVorgangsid, neudokumentID)
    '        kopplenMitEreignis(ereignisID, neudokumentID)
    '        Return True
    '    Else
    '        nachricht("in		checkIN_Database --------------------------- , NICHT erfolgreich")
    '        Return False
    '    End If
    'End Function




    Public Shared Sub DokumentStarten(ByVal aktdoku As Dokument)
        System.Threading.Thread.Sleep(1000)
        Try
            System.Diagnostics.Process.Start(aktdoku.FullnameCheckout)
        Catch ex1 As Exception
            System.Threading.Thread.Sleep(2000)
            Try
                System.Diagnostics.Process.Start(aktdoku.FullnameCheckout)
            Catch ex As Exception
                nachricht("Fehler in DateiStarten: " & aktdoku.FullnameCheckout & Environment.NewLine &
                                aktdoku.FullnameImArchiv & Environment.NewLine &
                                ex.ToString)
            End Try
        End Try
    End Sub





    'Function get_relativenpfad() As String
    '    nachricht("in get_relativenpfad -----------------------------")
    '    _aktTiefe = 1
    '    'leseZeigerdatei()
    '    For i = 0 To _verz_max_max%
    '        If istvoll(_aktTiefe, i) Then
    '            Continue For
    '        Else
    '            _relativpfad =  "\" & _aktTiefe & "\" & i
    '            nachricht("in get_relativenpfad -----------------------------" & _relativpfad$)
    '            Return _relativpfad$
    '        End If
    '    Next
    '    nachricht("Fehler in get_relativenpfad nichts gefunden:" & _relativpfad$)
    '    Return ""
    'End Function

    'Function istvoll(ByVal tiefe%, ByVal spalte as integer) as  Boolean 'rootDir.FullName
    '    nachricht("in istvoll ----------------------------------------------")
    '    Try
    '        Dim relativerPfad = "\" & tiefe & "\" & spalte%
    '        Dim gesamtpfad =  myGlobalz.Arc.rootDir.FullName$ & relativerPfad
    '        If Not IO.Directory.Exists(gesamtpfad) Then IO.Directory.CreateDirectory(gesamtpfad)
    '        Dim di As New IO.DirectoryInfo(gesamtpfad)
    '        Dim files = di.GetFiles()
    '        If files.Length < _datei_max% Then
    '            Return False
    '        Else
    '            Return True
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in istvoll:" & ex.ToString)
    '    End Try
    'End Function

    Function leseZeigerdatei() As Boolean
        nachricht("in leseZeigerdatei")
        Return False
    End Function

    Public Overloads Function DataTable_auschecken() As Boolean
        DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, True)
        My.Log.WriteEntry("a0")
    End Function

    Public Overloads Function DataTable_auschecken(ByVal zielVerzeichnis As String,
                                                   ByVal verzeichnisOeffnen As Boolean) As Boolean
        Dim quelle As String = "", ziel As String = ""
        'Dim dokview As DataRowView
        For Each dok As DataRow In myGlobalz.Arc.ArcRec.dt.AsEnumerable
            Try
                'Quelldatei_definieren
                quelle = myGlobalz.Arc.rootDir.ToString & dok.Item("relativpfad").ToString.Replace("/", "\")
                quelle = quelle & "\" & dok.Item("dateinameext").ToString
                nachricht("  quelle: " & quelle)
                'Zieldatei_definieren
                ziel = zielVerzeichnis & "\" & dok.Item("dateinameext").ToString
                nachricht("  ziel: " & ziel)
                'Zielverzeichnis_erstellen
                nachricht("Erzeuge Zielverzeichnis: zielVerzeichnis$: " & zielVerzeichnis)
                Dim erfolg As IO.DirectoryInfo = IO.Directory.CreateDirectory(zielVerzeichnis)
                If erfolg.Exists Then
                    nachricht("zielVerzeichnis wurde erzeugt!")
                Else
                    nachricht("zielVerzeichnis wurde NICHT erzeugt!")
                End If
                'wenn quelldateifehlt_exit
                Dim FIquelle, FIziel As IO.FileInfo
                Try
                    FIquelle = New IO.FileInfo(quelle)
                Catch ex As Exception
                    nachricht("fehler in DataTable_auschecken: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                     ex.ToString)
                    Continue For
                End Try

                If Not FIquelle.Exists Then
                    nachricht("Quelle existiert nicht: " & quelle)

                    Continue For
                Else
                    nachricht("Quelle existiert!")
                End If
                Try
                    FIziel = New IO.FileInfo(ziel)
                Catch ex As Exception
                    nachricht("fehler in DataTable_auschecken: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                     ex.ToString)
                    Continue For
                End Try

                If FIziel.Exists Then
                    nachricht("ziel existiert b!")
                    ziel = neuerdateinameFuerZiel(ziel, zielVerzeichnis, dok)
                    dok.Item("dateinamemitextension") = ziel
                End If

                'kopieren
                nachricht("  vor zieltest: " & ziel)

                ueberkopieren(quelle, ziel, FIquelle)

                My.Log.WriteEntry("a1") : Dim aktdo As New Dokument
                My.Log.WriteEntry("a2") : aktdo.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
                My.Log.WriteEntry("a3") : myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
                My.Log.WriteEntry("a4") : DokumentDatarow2Obj(dok, aktdo)                        'addieren
                My.Log.WriteEntry("a5") : zuCheckoutlisteAddieren(aktdo, myGlobalz.sitzung.checkoutDokuList)
                My.Log.WriteEntry("a6")
            Catch ex As Exception
                nachricht_und_Mbox("1Problem beim Auschecken von:  " & ziel & vbCrLf & ex.ToString)
                Return False
            End Try
        Next
        Oeffneverzeichnis(verzeichnisOeffnen, zielVerzeichnis)
        Return True
    End Function

    Private Shared Sub Oeffneverzeichnis(ByVal verzeichnisOeffnen As Boolean, ByVal zielVerzeichnis As String)
        Try
            Dim testdatei = IO.Path.Combine(zielVerzeichnis)
            nachricht(testdatei)
            If verzeichnisOeffnen Then System.Diagnostics.Process.Start(testdatei)
        Catch ex As Exception
            nachricht("Fehler in Oeffneverzeichnis:   " & ex.ToString)
        End Try
    End Sub

    Public Shared Function ueberkopieren(ByVal quelle As String, ByVal ziel As String, ByVal FIquelle As IO.FileInfo) As Int16
        Try
            nachricht("Quelle: " & quelle)
            nachricht("Ziel: " & ziel)
            FIquelle.CopyTo(ziel, True)
            System.Threading.Thread.Sleep(500)
            Return 0
        Catch ex As Exception
            nachricht("FEHLER beim ueberkopieren Kopieren von:  " & quelle & ", nach: " & ziel & " (zieldatei ist bestimmt geöffnet by user)")
            Return 1
        End Try
    End Function

    'Public Sub AllesAuscheckenVorgang(ByVal verzeichnisOeffnen As Boolean, BOOLverzeichnisVORHERloeschen As Boolean)
    '    Dim zieldir As String = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID
    '    verzeichnisloeschen(zieldir, BOOLverzeichnisVORHERloeschen)
    '    Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides")
    '    If bresult Then
    '        myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
    '        If myGlobalz.Arc.DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, verzeichnisOeffnen) Then
    '            IO.Directory.CreateDirectory(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
    '            If verzeichnisOeffnen Then System.Diagnostics.Process.Start(String.Format("{0}\{1}",
    '                    myGlobalz.Arc.lokalerCheckoutcache,
    '                    myGlobalz.sitzung.aktVorgangsID))
    '        End If
    '    End If
    'End Sub

    Public Sub AllesAuscheckenVorgangOBJ(ByVal verzeichnisOeffnen As Boolean,
                                      BOOLverzeichnisVORHERloeschen As Boolean,
                                      dliste As List(Of clsPresDokumente))
        Dim zieldir As String = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID
        verzeichnisloeschen(zieldir, BOOLverzeichnisVORHERloeschen)
        'Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides")
        'If bresult Then
        myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
        'dliste = New List(Of clsPresDokumente)()
        'If myGlobalz.Arc.DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, verzeichnisOeffnen) Then
        If myGlobalz.Arc.Dokliste_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, verzeichnisOeffnen, dliste) Then
            IO.Directory.CreateDirectory(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
            If verzeichnisOeffnen Then System.Diagnostics.Process.Start(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
        End If
        'End If
    End Sub

    'Public Sub AuscheckenVorgangEreignis()
    '    Dim bresult As Boolean = DokArcTools.dokusVonEreignisHolen.execute(myGlobalz.sitzung.aktEreignis.ID)
    '    If bresult Then
    '        myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
    '        If myGlobalz.Arc.DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, True) Then
    '            Process.Start(myGlobalz.Arc.lokalerCheckoutcache & "\" & myGlobalz.sitzung.aktVorgangsID)
    '        End If
    '    End If
    'End Sub

    'Public Shared Sub editDokumentMetadata()
    '    ' If revisionssicher() Then Exit Sub
    '    Dim metadoku As New WINdokumentMetaEdit
    '    metadoku.ShowDialog()
    'End Sub
    Public Shared Sub DokumentDatarow2Obj(ByVal item As DataRow, ByVal dokulok As Dokument)
        Try

            dokulok.DocID = CInt(item("DOKUMENTID"))
            dokulok.dokumentPfad = item("RELATIVPFAD").ToString
            dokulok.DateinameMitExtension = item("DATEINAMEEXT").ToString
            dokulok.Typ = item("TYP").ToString
            dokulok.Beschreibung = item("BESCHREIBUNG").ToString
            dokulok.Filedatum = CDate(item("FILEDATUM").ToString)
            dokulok.Checkindatum = CDate(item("CHECKINDATUM").ToString)


            dokulok.istVeraltet = CBool(item("VERALTET").ToString)
            dokulok.ExifDatum = CDate(item("EXIFDATUM").ToString)
            dokulok.EXIFlon = CStr(item("EXIFLONG").ToString)
            dokulok.EXIFlat = CStr(item("EXIFLAT").ToString)
            dokulok.EXIFdir = CStr(item("EXIFDIR").ToString)
            dokulok.EXIFhersteller = CStr(item("EXIFHERSTELLER").ToString)
            dokulok.revisionssicher = CBool(item("revisionssicher"))
            dokulok.Initiale = CStr(clsDBtools.fieldvalue(item("INITIAL_")))
            Try
                dokulok.EreignisID = CInt(item("ID").ToString)
            Catch ex As Exception

            End Try
            Try
                dokulok.istNurVerwandt = CBool(item("status"))
            Catch ex As Exception

            End Try
        Catch ex As Exception
            nachricht("DokumentDatarowView2Obj " & vbCrLf & vbCrLf & ex.ToString)
        End Try
    End Sub
    Public Shared Sub DokumentDatarowView2Obj(ByVal item As DataRowView, ByVal dokulok As Dokument)
        Try
            dokulok.DocID = CInt(item("DOKUMENTID")) '0
            dokulok.dokumentPfad = item("RELATIVPFAD").ToString '1
            dokulok.DateinameMitExtension = item("DATEINAMEEXT").ToString '2
            dokulok.Typ = item("TYP").ToString '3
            dokulok.Beschreibung = item("BESCHREIBUNG").ToString '4
            dokulok.Filedatum = CDate(item("FILEDATUM").ToString) '5
            dokulok.Checkindatum = CDate(item("CHECKINDATUM").ToString) '6
            'dokulok.istVeraltet = CBool(item("VERALTET").ToString) '7
            dokulok.ExifDatum = CDate(item("EXIFDATUM").ToString) '8
            dokulok.EXIFlon = CStr(item("EXIFLONG").ToString) '9
            dokulok.EXIFlat = CStr(item("EXIFLAT").ToString) '10
            dokulok.EXIFdir = CStr(item("EXIFDIR").ToString) '11
            dokulok.EXIFhersteller = CStr(item("EXIFHERSTELLER").ToString) '12
            dokulok.revisionssicher = CBool(item("revisionssicher"))
            dokulok.Initiale = CStr(clsDBtools.fieldvalue(item("INITIAL_")))
            dokulok.VorgangsID = CInt(clsDBtools.fieldvalue(item("VID")))
            Try
                dokulok.istNurVerwandt = CBool(item("status"))
            Catch ex As Exception

            End Try

        Catch ex As Exception
            nachricht("DokumentDatarowView2Obj " & vbCrLf & vbCrLf & ex.ToString)
        End Try
    End Sub
    'Public Shared Sub GIF_handeln(ByVal aktdoku As Dokument)
    '    If Not myGlobalz.winfoto Is Nothing Then myGlobalz.winfoto.Close()
    '    myGlobalz.winfoto = New Window_FotoEinzel(aktdoku)
    '    myGlobalz.winfoto.Show()
    'End Sub

    'Public Shared Sub JPG_handeln(ByVal aktdoku As Dokument)
    '    Try
    '        'If Not myGlobalz.winfoto Is Nothing Then myGlobalz.winfoto.Close()
    '        'myGlobalz.winfoto = New Window_FotoEinzel(aktdoku)
    '        'myGlobalz.winfoto.Show()


    '        If DokArc.aktiviereFotoGucker(myGlobalz.sitzung.aktVorgangsID, myGlobalz.OhneObsoletenDokus, myGlobalz.sitzung.aktEreignis.ID) > 0 Then
    '            Dim winfotoguck = New winFotoGucker(aktdoku)
    '            winfotoguck.ShowDialog()

    '        Else
    '            nachricht_und_Mbox("Keine Fotos gefunden")
    '        End If



    '    Catch ex As Exception
    '        nachricht("JPG " & vbCrLf & vbCrLf & ex.ToString)
    '    End Try
    'End Sub

    'Public Shared Sub Archiv_aktiviere_dokument(ByVal aktdoku As Dokument)
    '    If Not String.IsNullOrEmpty(aktdoku.FullnameImArchiv) Then
    '        doktypOeffnen(aktdoku)
    '    Else
    '        nachricht_und_Mbox("Dateiname ist ungültig:" & aktdoku.FullnameImArchiv)
    '    End If
    'End Sub

    'Private Shared Sub doktypOeffnen(ByVal aktdoku As Dokument)
    '    Dim test As New IO.FileInfo(aktdoku.FullnameImArchiv)
    '    'If test.Exists Then
    '    Select Case test.Extension.ToLower
    '        Case ".vcf"
    '            Dim diares As New MessageBoxResult
    '            diares = MessageBoxResult.Yes
    '            'diares = MessageBox.Show("Soll die elektronische Visitenkarte (vcf-Datei) von Paradigma interpretiert werden?" & vbCrLf &
    '            '                         " " & vbCrLf &
    '            '                         "Ja       - Ja, mit Paradigma öffnen, ggf. Beteiligtendaten übernehmen" & vbCrLf &
    '            '                         "Nein     - Nein, lieber  mit Outlook anschauen." & vbCrLf &
    '            '                         "Abbruch  - Formular verlassen" & vbCrLf &
    '            '                         "" & vbCrLf,
    '            '                          "Elektronischer Visitenkarte", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.OK
    '            '                         )
    '            If diares = MessageBoxResult.Yes Then
    '                Dim vsf As New winVCF(aktdoku.FullnameCheckout)
    '                vsf.ShowDialog()
    '            ElseIf diares = MessageBoxResult.No Then
    '                DokArc.DokumentStarten(aktdoku)
    '            Else
    '                'gornix
    '            End If
    '        Case ".png", ".gif"
    '            GIF_handeln(aktdoku)
    '        Case ".jpg" 'foto
    '            JPG_handeln(aktdoku)
    '            'Case ".rtf", ".doc", ".xls"
    '            '    RTF_handeln(aktdoku)
    '        Case ".3ga"
    '            If myGlobalz.sitzung.aktDokument.FullnameCheckout.Trim.ToLower.EndsWith(".3ga") Then
    '                Dim ziel As String = kopiere3ganach3gp()
    '                sounddateiStarten(ziel)
    '            End If
    '        Case Else
    '            If aktdoku.revisionssicher Then
    '                MessageBox.Show(myGlobalz.Infotext_revisionssicherheit, aktdoku.FullnameCheckout, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
    '            End If
    '            DokArc.DokumentStarten(aktdoku) 'im zweifel immer den checkout verwenden
    '    End Select
    'End Sub

    'Private Shared Function kopiere3ganach3gp() As String
    '    Dim quelle As IO.FileInfo
    '    Dim ziel As String = ""
    '    Try
    '        quelle = New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
    '        ziel = myGlobalz.sitzung.aktDokument.FullnameCheckout.ToLower.Replace(".3ga", ".3gp")
    '        quelle.CopyTo(ziel, True)
    '        Return ziel
    '    Catch ex As Exception
    '        nachricht("fehler in kopiere3ganach3gp: " & ex.ToString)
    '        Return ziel
    '    End Try
    'End Function

    'Private Shared Sub sounddateiStarten(ByVal ziel As String)
    '    System.Threading.Thread.Sleep(1000)
    '    Try
    '        System.Diagnostics.Process.Start(ziel)
    '    Catch ex As Exception
    '        nachricht("Fehler in DateiStarten: " & ex.ToString)
    '    End Try
    'End Sub
    Public Shared Sub Archiv_definiereAktdokument(ByVal item As DataRowView)

        myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
        DokumentDatarowView2Obj(item, myGlobalz.sitzung.aktDokument)
        myGlobalz.sitzung.aktVorgangsID = myGlobalz.sitzung.aktDokument.VorgangsID
        setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktVorgangsID)
        myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        Dim ausgabeverzeichnis As String = ""
        myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, 0, ausgabeverzeichnis)
    End Sub

    'Public Shared Sub zulisteAddieren()
    '    Dim newdog As New Dokument
    '    newdog = CType(myGlobalz.sitzung.aktDokument.Clone, Dokument)
    '    zuCheckoutlisteAddieren(newdog, myGlobalz.sitzung.checkoutDokuList)
    'End Sub

    '''' <summary>
    '''' arbeitet mit aktDokument
    '''' </summary>
    '''' <param name="modus">zeige oder metaedit</param>
    '''' <remarks></remarks>
    'Public Shared Function machCheckout(ByVal modus As String) As Boolean

    '    If myGlobalz.sitzung.aktDokument Is Nothing Then
    '        nachricht("FEHLER: Dokument ist nothing")
    '        Return False
    '    End If
    '    If DokArc.istInCheckoutListe(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.checkoutDokuList) Then
    '        MessageBox.Show("Dieses Dokument ist vermutlich schon in Bearbeitung !" & vbCrLf &
    '                        "Bitte schließen Sie das Dokument bzw. übernehmen Sie die Änderungen!" & vbCrLf &
    '                        "" & vbCrLf &
    '                        ".XLS - sollte das Problem bei XLS- Dateien auftreten, so konvertieren Sie bitte die " & vbCrLf &
    '                        "    xls-Datei in das moderne Dateiformat .XLSX (=> Datei-Speichern unter)" & vbCrLf &
    '                        " So läßt sich der Fehler vermeiden!" & vbCrLf &
    '                        " Bei Unklarheiten wenden Sie sich bitte an das Sekretariat.", "Office-Dokument in Bearbeitung")
    '        Return False
    '    End If
    '    If checkout.checkout(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID) = "fehler" Then   'checkout findet IMMER statt
    '        Return False
    '    End If
    '    Return True
    'End Function

    'Public Shared Function istInCheckoutListe(ByVal dok As Dokument, ByRef dieliste As List(Of Dokument)) As Boolean
    '    If dok Is Nothing Then Return False
    '    If dieliste Is Nothing Then Return False
    '    If listeEnthaeltDokument(dok, dieliste) Then
    '        Return True
    '    Else
    '        Return False
    '    End If
    '    Return False
    'End Function

    Public Shared Sub zuCheckoutlisteAddieren(ByVal dok As Dokument, ByRef dieliste As List(Of Dokument))
        If dok Is Nothing Then Exit Sub
        If dieliste Is Nothing Then Exit Sub
        If Not dok.isTypeEditable Then
            Exit Sub
        End If
        If Not listeEnthaeltDokument(dok, dieliste) Then
            dieliste.Add(dok)
        End If
    End Sub

    Public Shared Sub ausCheckoutlisteEntfernen(ByVal dok As Dokument, ByRef dieliste As List(Of Dokument))
        If dok Is Nothing Then Exit Sub
        If dieliste Is Nothing Then Exit Sub
        If listeEnthaeltDokument(dok, dieliste) Then
            dieliste.Remove(dok)
        End If
    End Sub

    'Shared Function aktiviereFotoGucker(ByVal vorgangsID%, ByVal OhneObsoletenDoku As Boolean, eid As Integer) As Integer
    '    'alle Fotos in Objektliste packen
    '    Dim auchVeraltete = True
    '    Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(vorgangsID, "nurfotos")
    '    'initDokumente4VorgangDatatable(vorgangsID, OhneObsoletenDoku)  'alle dokus
    '    Return alleFotosinCollectionPacken(auchVeraltete, vorgangsID)
    'End Function

    Public Shared Sub DokuZeile2OBJ(ByVal i As Integer, ByVal zieldokulok As Dokument, ByVal quelldoktab As DataTable)
        Try
            With quelldoktab
                zieldokulok.DocID = CInt(clsDBtools.fieldvalue(.Rows(i).Item("DOKUMENTID")))
                zieldokulok.dokumentPfad = clsDBtools.fieldvalue(.Rows(i).Item("RELATIVPFAD")).ToString
                zieldokulok.DateinameMitExtension = clsDBtools.fieldvalue(.Rows(i).Item("DATEINAMEEXT")).ToString
                zieldokulok.Typ = clsDBtools.fieldvalue(.Rows(i).Item("TYP")).ToString
                zieldokulok.Beschreibung = clsDBtools.fieldvalue(.Rows(i).Item("BESCHREIBUNG")).ToString
                zieldokulok.Filedatum = CDate(clsDBtools.fieldvalue(.Rows(i).Item("FILEDATUM")).ToString)
                zieldokulok.Checkindatum = CDate(clsDBtools.fieldvalue(.Rows(i).Item("CHECKINDATUM")).ToString)
                zieldokulok.istVeraltet = CBool(clsDBtools.fieldvalue(.Rows(i).Item("VERALTET")).ToString)

                If String.IsNullOrEmpty(clsDBtools.fieldvalue(.Rows(i).Item("EXIFDATUM")).ToString.Trim) OrElse
                    clsDBtools.fieldvalue(.Rows(i).Item("EXIFDATUM")).ToString.Trim.Length < 3 Then
                    zieldokulok.ExifDatum = CLstart.mycsimple.MeinNULLDatumAlsDate
                Else
                    zieldokulok.ExifDatum = CDate(clsDBtools.fieldvalue(.Rows(i).Item("EXIFDATUM")))
                End If
                zieldokulok.EXIFlon = clsDBtools.fieldvalue(.Rows(i).Item("EXIFLONG")).ToString
                zieldokulok.EXIFlat = clsDBtools.fieldvalue(.Rows(i).Item("EXIFLAT")).ToString
                zieldokulok.EXIFdir = clsDBtools.fieldvalue(.Rows(i).Item("EXIFDIR")).ToString
                zieldokulok.EXIFhersteller = clsDBtools.fieldvalue(.Rows(i).Item("EXIFHERSTELLER")).ToString
                'zieldokulok.OriginalFullname = clsDBtools.fieldvalue(.Rows(i).Item("ORIGINALFULLNAME")).ToString
                'zieldokulok.OriginalName = clsDBtools.fieldvalue(.Rows(i).Item("ORIGINALNAME")).ToString
                zieldokulok.revisionssicher = CBool(clsDBtools.fieldvalue(.Rows(i).Item("REVISIONSSICHER")).ToString)
                zieldokulok.Initiale = clsDBtools.fieldvalue(.Rows(i).Item("INITIAL_")).ToString
            End With
        Catch ex As Exception
            nachricht("Fehler in DokuZeile2OBJ:" & ex.ToString)
        End Try
    End Sub

    Public Shared Function alleFotosinCollectionPacken(ByVal auchVeraltete As Boolean, ByVal VorgangsID As Integer) As Integer
        Dim ihinzu% = 0
        Dim ausgabeverzeichnis As String = ""
        Try
            clstart.myc.collFotos.Clear()
            For i = 0 To myGlobalz.Arc.ArcRec.dt.Rows.Count - 1
                If myGlobalz.Arc.ArcRec.dt.Rows(i).Item("Typ").ToString.ToLower = "jpg" Then
                    Dim dokulok As New clsFotoDokument
                    DokuZeile2OBJ(i, dokulok, myGlobalz.Arc.ArcRec.dt)
                    If Not dokulok.istVeraltet Or auchVeraltete Then
                        dokulok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                        dokulok.makeFullname_Checkout(VorgangsID%, myGlobalz.Arc.lokalerCheckoutcache, 0, ausgabeverzeichnis)
                        CLstart.myc.collFotos.Add(dokulok)
                        ihinzu += 1
                    End If
                End If
            Next
            Return ihinzu
        Catch ex As Exception
            Return -1
        End Try
    End Function
    ''' <summary>
    ''' Exif-Daten werden auf MYJPG-Abgebildet
    ''' </summary>
    ''' <param name="aktJPG"></param>
    ''' <param name="exifQ"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getplainExifdata(ByRef aktJPG As clsMyJPG, _
     ByRef exifQ As ExifWorksQuick) As Boolean
        Try

            exifQ = New ExifWorksQuick(aktJPG.fullname)

            If exifQ Is Nothing Then
                nachricht("datei im eimer, exifprüfung: " & aktJPG.fullname)
                Return False
            End If
            'aktJPG.ExifQ.Width
            If exifQ.Width < 0 Then
                nachricht("datei de   dateidefekt += 1 ")
                Return False
            End If

            aktJPG.EXIFhersteller = exifQ.EquipmentMaker

            aktJPG.Exifgpslatitude = exifQ.gpslatitude
            aktJPG.Exifgpslongitude = exifQ.gpslongitude

            aktJPG.ExifGpsImgDir = exifQ.GpsImgDir
            '  enthält ggf die GK koordinaten
            aktJPG.EXIFdescription = exifQ.Description

            aktJPG.ExifWidth = exifQ.Width
            aktJPG.ExifHeight = exifQ.Height
            aktJPG.EXIFartist = clsMedium.EXIF_kuerzen(exifQ.Artist)

            aktJPG.EXIFusercomment = clsMedium.EXIF_kuerzen(exifQ.UserComment).Trim
            If aktJPG.EXIFusercomment.Length = 255 Then
                'workaround für exifcommand.
                'liefert ungültigen string
                aktJPG.EXIFusercomment = "-"
            End If
            aktJPG.EXIFtitle = clsMedium.EXIF_kuerzen(exifQ.Title).Trim
            aktJPG.EXIFDateTimeOriginal = exifQ.DateTimeOriginal
            Return True
        Catch ex As Exception
            nachricht("getplainExifdata " & ex.Message)
            Return False
        End Try
    End Function

    'Private Shared Function loescheDokumentImFilesystem(ByVal aktDokument As Dokument) As Boolean
    '    nachricht("loescheDokumentImFilesystem -----------------------------")
    '    Dim atest As String = aktDokument.makeFullname_ImArchiv(myglobalz.Arc.rootDir)
    '    Try
    '        nachricht("datei:  " & atest)
    '        Dim testdatei As New IO.FileInfo(atest)
    '        If Not testdatei.Exists() Then
    '            nachricht("datei existiert nicht:  " & atest & ". löschen wird ignoriert.")
    '            Return True
    '        End If
    '        testdatei.Delete()
    '        nachricht("datei existiert :  " & atest & ". löschen erfolgreich.")
    '        Return True
    '    Catch ex As Exception
    '        nachricht("Fehler: Datei konnte nicht gelöscht werden: " & atest)
    '    End Try
    'End Function

    'Private Shared Function revisionssicher(dok As Dokument) As Boolean
    '    If dok.revisionssicher Then
    '        MessageBox.Show("Das Dokument ist revisionssicher. Es kann nicht gelöscht werden! (" & dok.DateinameMitExtension & ")", "Dokument löschen",
    '                        MessageBoxButton.OK,
    '                        MessageBoxImage.Exclamation)
    '        Return True
    '    End If
    '    Return False
    'End Function

    'Public Shared Function aktDokumentLoschen(adok As Dokument) As Boolean
    '    Try
    '        If revisionssicher(adok) Then
    '            nachricht("Das Dokument ist revisionssicher und wird deshalb nicht gelöscht!")
    '            Exit Function
    '        End If

    '        'auschecken
    '        Dim hinweis As String = checkout.checkout(adok, myglobalz.sitzung.aktVorgangsID)
    '        nachricht("#: " & hinweis$)

    '        ' datei im archiv löschen
    '        Dim btest As Boolean = loescheDokumentImFilesystem(adok)
    '        nachricht("ARCHIV: loescheDokumentImFilesystem: true ist erfolg: " & btest)

    '        'dateieintrag in der tabelle dokumente löschen
    '        ' Dim itest% = DokumentenArchiv.loescheDokument(myGlobalz.sitzung.aktDokument.DocID)
    '        Dim itest% = DokArcTools.dokINDbLoeschen.execute(adok.DocID)
    '        nachricht("ARCHIV: aktDokumentLoschen: 1 ist erfolg: " & itest%)

    '        'dateieintrag in der tabelle dokument2vorgang löschen
    '        'itest = entKoppelung_Dokument_Vorgang(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.VorgangsID)
    '        itest = DokArcTools.EntkopplungVonVorgang.execute(adok.DocID, myglobalz.sitzung.aktVorgangsID)
    '        nachricht("ARCHIV: entKoppelung_Dokument_Vorgang: 1 ist erfolg: " & itest%)


    '        itest = DokArcTools.EntKoppelung_Dokument_Ereignis_alledb.execute(adok.DocID, myglobalz.sitzung.aktEreignis.ID)
    '        'dateieintrag in der tabelle dokument2ereignis löschen
    '        '  itest = clsEreignisDB_Mysql.EntKoppelung_Dokument_Ereignis(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktEreignis.ID)
    '        nachricht("ARCHIV: EntKoppelung_Dokument_Ereignis: 0 oder 1 ist erfolg: " & itest%)

    '        'id aus parafoto holen - entspricht der sekid in raumbezugstabelle
    '        'nicht vergessen daß der typ (=5) noch mit angegeben werden muss damit das löchen 
    '        '       in der raumbezugstabelle eindeutig ist
    '        '  Dim sekid% = DBraumbezug_Mysql.getID4Foto(myGlobalz.sitzung.aktDokument.DocID)
    '        Dim sekid% = DokArcTools.getID4Foto.execute(adok.DocID)
    '        nachricht("ARCHIV: getID4Foto: >0 ist erfolg: " & sekid%)
    '        If sekid > 0 Then
    '            itest = RBtoolsns.RBFotoLoeschen_alleDB.execute(adok.DocID)
    '            'itest = DBraumbezug_Mysql.RB_FOTO_loeschen(myGlobalz.sitzung.aktDokument.DocID)
    '            nachricht("ARCHIV: RB_FOTO_loeschen:  " & itest%)
    '            itest = RBtoolsns.Raumbezug_loeschen_bySEKid_alleDB.execute(sekid, "5")
    '            ' itest = DBraumbezug_Mysql.RB_loeschenBySekIDTyp(sekid, "5") 'gilt nur für Fotos besser wäre eine umwandlung des enums in integer: todo
    '            nachricht("ARCHIV: RB_loeschenBySekIDTyp:  : " & itest%)
    '            Dim erfolg As Integer
    '            erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myglobalz.sitzung.aktParaFoto.RaumbezugsID), myglobalz.sitzung.aktVorgangsID)
    '        End If
    '        nachricht("Dokument wurde gelöscht. Bitte führen Sie einen Refresh durch.")

    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler beim Löschen des Dokumentes: " & ex.ToString)
    '    End Try
    'End Function

    Public Shared Function listeEnthaeltDokument(ByVal dok As Dokument, ByVal dieliste As List(Of Dokument)) As Boolean
        For Each ddog As Dokument In dieliste
            If ddog.FullnameCheckout = dok.FullnameCheckout Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Shared Function setzeVerwandschaftsstatus(ByVal dokid As Integer, ByVal vorgangsid As Integer) As Boolean
        Dim result As Boolean = False
        Dim dt As DataTable
        Dim hinweis As String
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from dokument2vorgang " &
                                                        " where dokumentid=" & dokid &
                                                        " and vorgangsid=" & vorgangsid
        dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
        'If myglobalz.ARC_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New DokArcOracle(clsDBspecOracle.getConnection(myglobalz.ARC_MYDB))
        '    result = zzz.gehoertDokumentZumVorgang(dokid, vorgangsid)
        '    zzz.Dispose()
        'End If
        If dt.IsNothingOrEmpty Then
            myGlobalz.sitzung.aktDokument.istNurVerwandt = False
            Return False
        Else
            myGlobalz.sitzung.aktDokument.istNurVerwandt = True
            Return True
        End If
    End Function



    Function getFreshNumDir(ByVal archivsubdir As String) As String
        Dim ZielGesamtpfad = rootDir.ToString & archivsubdir
        Dim testdir As IO.DirectoryInfo
        For i = 0 To 12999
            testdir = New IO.DirectoryInfo(ZielGesamtpfad$ & "\" & i.ToString)
            If Not testdir.Exists Then
                'treffer
                Return "\" & i.ToString
            End If
        Next
        Return ""
    End Function

    Private Function neuerdateinameFuerZiel(ziel As String, zieldir As String, dok As DataRow) As String
        Dim neuername As String
        Try
            neuername = dok.Item("dateinameext").ToString.Replace(dok.Item("typ").ToString, "")
            neuername = zieldir & "\" &
                        neuername & "_" &
                        dok.Item("filedatum").ToString.Replace(".", "_").Replace(":", "_") & "." &
                        dok.Item("typ").ToString
            Return neuername
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim neuerdateinameFuerZiel des Dokumentes: " & ex.ToString)
            Return ""
        End Try
    End Function
    Private Function neuerdateinameFuerZielOBJ(ziel As String, zieldir As String, dok As clsPresDokumente) As String
        Dim neuername As String
        Try
            neuername = dok.DateinameMitExtension.Replace(dok.Typ.ToString, "")
            neuername = zieldir & "\" &
                        neuername & "_" &
                        dok.Filedatum.ToString.Replace(".", "_").Replace(":", "_") & "." &
                        dok.Typ.ToString
            Return neuername
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim neuerdateinameFuerZiel des Dokumentes: " & ex.ToString)
            Return ""
        End Try
    End Function

    Private Sub verzeichnisloeschen(zieldir As String, BOOLverzeichnisVORHERloeschen As Boolean)
        Try
            Dim kills As String()
            kills = getfilesAusDir(zieldir)
            If kills Is Nothing Then
                'dir existiert nicht
                Exit Sub
            End If
            For Each ele In kills
                Try
                    IO.File.Delete(ele)
                Catch ex As Exception
                    nachricht("Fehler beim verzeichnisLoeschen1 des Dokumentes: " & ele & Environment.NewLine &
                                    ex.ToString)
                End Try
            Next
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim verzeichnisLoeschen2 des Dokumentes: " & ex.ToString)
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

    Private Function Dokliste_auschecken(zielverzeichnis As String, verzeichnisOeffnen As Boolean, dliste As List(Of clsPresDokumente)) As Boolean
        Dim quelle As String = "", ziel As String = ""
        'Dim dokview As DataRowView
        For Each dok As clsPresDokumente In dliste
            If Not dok.ausgewaehlt Then Continue For
            Try
                'Quelldatei_definieren
                quelle = myGlobalz.Arc.rootDir.ToString & dok.dokumentPfad.ToString.Replace("/", "\")
                quelle = quelle & "\" & dok.DateinameMitExtension.ToString
                nachricht("  quelle: " & quelle)
                'Zieldatei_definieren
                ziel = zielverzeichnis & "\" & dok.DateinameMitExtension
                nachricht("  ziel: " & ziel)
                'Zielverzeichnis_erstellen
                nachricht("Erzeuge Zielverzeichnis: zielVerzeichnis$: " & zielverzeichnis)
                Dim erfolg As IO.DirectoryInfo = IO.Directory.CreateDirectory(zielverzeichnis)
                If erfolg.Exists Then
                    nachricht("zielVerzeichnis wurde erzeugt!")
                Else
                    nachricht("zielVerzeichnis wurde NICHT erzeugt!")
                End If
                'wenn quelldateifehlt_exit
                Dim FIquelle, FIziel As IO.FileInfo
                Try
                    FIquelle = New IO.FileInfo(quelle)
                Catch ex As Exception
                    nachricht("fehler in DataTable_auschecken: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                     ex.ToString)
                    Continue For
                End Try

                If Not FIquelle.Exists Then
                    nachricht("Quelle existiert nicht: " & quelle)

                    Continue For
                Else
                    nachricht("Quelle existiert!")
                End If
                Try
                    FIziel = New IO.FileInfo(ziel)
                Catch ex As Exception
                    nachricht("fehler in DataTable_auschecken: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                     ex.ToString)
                    Continue For
                End Try

                If FIziel.Exists Then
                    nachricht("ziel existiert c!")
                    ziel = neuerdateinameFuerZielOBJ(ziel, zielverzeichnis, dok)
                End If

                'kopieren
                nachricht("  vor zieltest: " & ziel)

                ueberkopieren(quelle, ziel, FIquelle)

                My.Log.WriteEntry("a1") : Dim aktdo As New Dokument
                My.Log.WriteEntry("a2") : aktdo.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
                My.Log.WriteEntry("a3") : myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
                'My.Log.WriteEntry("a4") : DokumentDatarow2Obj(dok, aktdo)                        'addieren
                My.Log.WriteEntry("a5") : zuCheckoutlisteAddieren(aktdo, myGlobalz.sitzung.checkoutDokuList)
                My.Log.WriteEntry("a6")
            Catch ex As Exception
                nachricht_und_Mbox("1Problem beim Auschecken von:  " & ziel & vbCrLf & ex.ToString)
                Return False
            End Try
        Next
        Oeffneverzeichnis(verzeichnisOeffnen, zielverzeichnis)
        Return True
    End Function

    Public Function einzeldokument_auschecken(ByVal zielVerzeichnis As String, dok As Dokument) As Boolean
        Dim quelle As String = "", ziel As String = ""

        Try
            'Quelldatei_definieren
            quelle = myGlobalz.Arc.rootDir.ToString & dok.dokumentPfad.ToString.Replace("/", "\")
            quelle = quelle & "\" & dok.DateinameMitExtension.ToString
            nachricht("  quelle: " & quelle)
            'Zieldatei_definieren
            ziel = zielVerzeichnis & "\" & dok.DateinameMitExtension.ToString
            nachricht("  ziel: " & ziel)
            'Zielverzeichnis_erstellen
            nachricht("Erzeuge Zielverzeichnis: zielVerzeichnis$: " & zielVerzeichnis)
            Dim erfolg As IO.DirectoryInfo = IO.Directory.CreateDirectory(zielVerzeichnis)
            If erfolg.Exists Then
                nachricht("zielVerzeichnis wurde erzeugt!")
            Else
                nachricht("zielVerzeichnis wurde NICHT erzeugt!")
            End If
            'wenn quelldateifehlt_exit
            Dim FIquelle, FIziel As IO.FileInfo
            Try
                FIquelle = New IO.FileInfo(quelle)
            Catch ex As Exception
                nachricht("fehler in DataTable_auschecken: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                 ex.ToString)
                Return False
            End Try

            If Not FIquelle.Exists Then
                nachricht("Quelle existiert nicht: " & quelle)
                Return False
            Else
                nachricht("Quelle existiert!")
            End If
            Try
                FIziel = New IO.FileInfo(ziel)
            Catch ex As Exception
                nachricht("fehler in DataTable_auschecken: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                 ex.ToString)
                Return False
            End Try

            If FIziel.Exists Then
                nachricht("ziel existiert a!")
                ziel = neuerdateinameFuerZielOBJ(ziel, zielVerzeichnis, CType(dok, clsPresDokumente))
                dok.DateinameMitExtension = ziel
            End If

            'kopieren
            nachricht("  vor zieltest: " & ziel)

            If ueberkopieren(quelle, ziel, FIquelle) = 1 Then
                nachricht("zieldatei ist im zugriff: " & ziel)
                Return False
            End If

            'My.Log.WriteEntry("a1") : Dim aktdo As New Dokument
            'My.Log.WriteEntry("a2") : aktdo.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
            'My.Log.WriteEntry("a3") : myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
            'My.Log.WriteEntry("a4") : DokumentDatarow2Obj(dok, aktdo)                        'addieren
            'My.Log.WriteEntry("a5") : zuCheckoutlisteAddieren(aktdo, myGlobalz.sitzung.checkoutDokuList)
            'My.Log.WriteEntry("a6")
        Catch ex As Exception
            nachricht_und_Mbox("fehler beim Auschecken von:  " & ziel & vbCrLf & ex.ToString)
            Return False
        End Try
        Return True
    End Function

End Class