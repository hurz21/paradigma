Imports System.Data
Imports System.Threading
Imports paradigmaDetail

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
    'Private _verz_max_max As Integer = 500 ' 200
    'Private _datei_max As Integer = 10000 '500	'200
    'Private Shared Radius As Integer = 200
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
                    f = Nothing
                    Return ZielDateiFullName
                End If
            Next
            nachricht("fehler in getNeuenNamenFuerDouble: schleife mit mehr als 10000 turns")
            f = Nothing
            Return dateiname
        Catch ex As Exception
            nachricht("Fehler: in	getNeuenNamenFuerDouble: " & dateiname & " " & vbCrLf, ex)
            Return dateiname
        End Try
    End Function

    Private Shared Function QuelleLoeschenFallsMoven(ByVal Dokumente_moven As Boolean, ByVal OriginalFullname As String) As Boolean
        Return True
        'Try
        '    If Dokumente_moven Then
        '        'löschen der originaldatei
        '        Dim ds As New IO.FileInfo(OriginalFullname)
        '        ds.Delete()
        '        ds = Nothing
        '    End If
        '    Return True
        'Catch ex As Exception
        '    'nachricht()
        '    nachricht("nörmal: in loeschenFallsMoven: " & vbCrLf ,ex)
        '    Return False
        'End Try
    End Function

    Private Sub erzeugeUnterverzeichnis(ByVal relativpfad$)
        Try
            IO.Directory.CreateDirectory(relativpfad)
        Catch ex As Exception
            nachricht("Fehler in: erzeugeUnterverzeichnis: ", ex)
        End Try
    End Sub


    ''' <summary>
    ''' 1. Speichert die Datei Und
    ''' 2. liefert den Dateinamen im archiv zurück und
    ''' 3. liefert den relativen pfad zurück
    ''' immer hinterher das readonly entfernen!!!!  FileArchivTools.inputFileReadonlyEntfernen(archivDateiFullname)
    ''' archivname ist beim eintrill noch leer, wird hier erst gebildet
    ''' </summary>
    ''' <param name="Archivname"></param> 
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function checkIN_FileArchiv(ByVal QuelleFullname As String,
                                     ByRef Archivname As String,
                                     ByRef erfolgreich As Boolean,
                                     ByRef NumDir As String,
                                     ByVal ArchivSubdir As String,
                                     ByVal NEWSAVEMODE As Boolean,
                                     ByVal dokid As Integer,
                                       kompress As Boolean) As String
        nachricht("checkIN_FileArchiv: input  OriginalFullname: " & QuelleFullname)
        nachricht("checkIN_FileArchiv: input  Archivname: " & Archivname)
        nachricht("checkIN_FileArchiv: input  erfolgreich: " & erfolgreich)
        nachricht("checkIN_FileArchiv: input  NumDir: " & NumDir)
        nachricht("checkIN_FileArchiv: input  NEWSAVEMODE: " & NEWSAVEMODE)
        nachricht("checkIN_FileArchiv: input  dokid: " & dokid)
        Dim result As MessageBoxResult
        Dim dokumentpfad As String
        Dim ZielGesamtpfad As String
        Dim ZielDateiFullName As String = ""
        Dim Fquell As IO.FileInfo = Nothing
        Dim Fziel As IO.FileInfo = Nothing
        dokumentpfad = ArchivSubdir & NumDir
        ZielGesamtpfad = myGlobalz.Arc.rootDir.ToString & dokumentpfad 'myGlobalz.Arc.rootDir
        erfolgreich = False
        erzeugeUnterverzeichnis(ZielGesamtpfad)
        nachricht("in checkIN_FileArchiv")
        Try
            Fquell = New IO.FileInfo(QuelleFullname)
            Dim normname As String = ""
            If NEWSAVEMODE Then
                ZielDateiFullName = ZielGesamtpfad & "\" & dokid
            Else
                normname = LIBgemeinsames.clsString.normalize_Filename(Fquell.Name)
                ZielDateiFullName = ZielGesamtpfad & "\" & normname
            End If

            Fziel = New IO.FileInfo(ZielDateiFullName)
            If Fziel.Exists Then
                nachricht("zielDatei existiert schon:" & Fziel.FullName)
                ZielDateiFullName = getNeuenNamenFuerDouble(normname, ZielGesamtpfad)
                Fziel = New IO.FileInfo(ZielDateiFullName)
                nachricht(Fquell.FullName & ": Datei existiert schon im Archiv. Wurde umbenannt: " & Fziel.FullName)
                Try
                    If Fquell.Exists Then
                        If copyOrCopyKompress(Fquell, Fziel, kompress) Then
                            erfolgreich = True
                        Else
                            erfolgreich = False
                        End If

                        nachricht("Kopieren ins Archiv: " & Fquell.FullName & " mit neuem Namen:" & Fziel.FullName)
                        Archivname = Fziel.FullName
                    Else
                        nachricht_und_Mbox("Quelldatei existiert nicht: " & Fquell.FullName & " Keine Aktion")
                    End If
                Catch ex2 As Exception
                    nachricht("FEhler 1Kopieren ins Archiv gescheitert!" & ex2.ToString)
                    Archivname = ""
                End Try
            Else
                nachricht("zielDatei existiert noch nicht:" & Fziel.FullName)
                Try
                    If copyOrCopyKompress(Fquell, Fziel, kompress) Then
                        erfolgreich = True
                    Else
                        erfolgreich = False
                    End If
                    nachricht("Kopieren ins Archiv: " & Fquell.FullName)
                    Archivname = Fziel.FullName
                Catch ex3 As Exception
                    nachricht("FEhler 2Kopieren ins Archiv gescheitert! von " & Fquell.FullName & Environment.NewLine &
                               "nach: " & Fziel.FullName & ex3.ToString)

                    Archivname = ""
                End Try
            End If
            Fquell = Nothing
            Fziel = Nothing
            nachricht("checkIN_FileArchiv: output  Archivname: " & Archivname)
            nachricht("checkIN_FileArchiv: output  dokumentpfad: " & dokumentpfad)
            Return dokumentpfad 'ist unverändert: warum wird das wieder zurückgeliefert?
        Catch ex4 As Exception
            erfolgreich = False
            Fquell = Nothing
            nachricht("FEhler checkIN_FileArchiv: FEHLER  OriginalFullname: " & QuelleFullname & ex4.ToString)
            result = MessageBox.Show("Diese Datei existiert schon: " & QuelleFullname & vbCrLf & "Kopie anlegen ?",
             "Einchecken von Dokumenten", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
            If result = MessageBoxResult.Yes Then
                Try
                    ' IO.File.Copy(dateiname , dateiganz, True)
                Catch ex5 As Exception
                    nachricht("FEhler checkIN_FileArchiv: FEHLER2  OriginalFullname" & QuelleFullname & ex5.ToString)
                    MessageBox.Show("Fehler beim überschreiben. Die Datei wird ggf. von einem anderen Programm benutzt!" & vbCrLf & ex5.ToString)
                    Return "Fehler"
                End Try
            End If
            Archivname = ""
            Return dokumentpfad
        End Try
    End Function



    Public Shared Function istKomprimierbarerTyp(fullName As String) As Boolean
        Try
            l(" MOD istKomprimierbarerTyp anfang")
            If fullName.ToLower.EndsWith(".pdf") Or
                fullName.ToLower.EndsWith(".doc") Or
                fullName.ToLower.EndsWith(".docx") Or
                fullName.ToLower.EndsWith(".rtf") Then
                Return True
            End If
            l(" MOD istKomprimierbarerTyp ende")
            Return False
        Catch ex As Exception
            l("Fehler in istKomprimierbarerTyp: ", ex)
            Return False
        End Try
    End Function

    Public Overloads Function checkINDoku(ByVal dateiname As String,
                                          ByVal ereignisID As Integer,
                                          ByVal Beschreibung As String,
                                          ByVal zielVID As Integer,
                                          ByVal fotoZuRaumbezug As Boolean,
                                          ByRef NumDir As String,
                                          ByVal dateidatum As Date,
                                          ByRef dokid As Integer,
                                          ByVal archivsubdir As String,
                                          fotosverkleinern As Boolean, KOMPRESS As Boolean, bearbeiterid As Integer) As Boolean
        nachricht("archiv-checkIN ------------------------------------------")
        nachricht("dateiname$" & " - " & ereignisID)
        Try
            'If checkINDoku(dateiname, Beschreibung, ereignisID, False, "", myGlobalz.sitzung.getArchivSubdir(zielVID), zielVID, fotoZuRaumbezug) Then
            If checkINDoku(dateiname, Beschreibung, ereignisID, False, "", NumDir, zielVID, fotoZuRaumbezug, dateidatum, dokid,
                           archivsubdir, myGlobalz.sitzung.aktDokument.newSaveMode,
                                          fotosverkleinern, KOMPRESS, bearbeiterid) Then
                nachricht("checkin erfolgreich")
                Return True
            Else
                nachricht("checkin nicht erfolgreich")
                Return False
            End If
        Catch ex As Exception
            nachricht("checkIN dateiname")
            Return False
        End Try
    End Function


    Public Overloads Function checkINDoku(ByVal OriginalFullname As String,
                                        ByVal Beschreibung As String,
                                        ByVal ereignisID As Integer,
                                        ByVal Dokumente_moven As Boolean,
                                        ByRef archivDateiFullname As String,
                                        ByRef NumDir As String,
                                        ByVal lokVorgangsID As Integer,
                                        ByVal fotoZuRaumbezug As Boolean,
                                        dateidatum As Date,
                                        ByRef neudokid As Integer,
                                        ByVal ArchivSubdir As String,
                                        NEWSAVEMODE As Boolean,
                                          fotosverkleinern As Boolean,
                                          KOMPRESS As Boolean,
                                          bearbeiterid As Integer) As Boolean
        Dim test, erfolgreich As Boolean
        nachricht("IN Checkin: ---------------------")
        'MsgBox("NEWSAVEMODE " & NEWSAVEMODE)
        If (istKomprimierbarerTyp(OriginalFullname)) Then
            'ist unabhängig vom savemode
            If Environment.UserName.ToLower = "feinen_j" Then
                KOMPRESS = False
            Else
                KOMPRESS = False
            End If

        Else
            KOMPRESS = False
        End If

        If NEWSAVEMODE Then
            nachricht("IN Checkin: ---------------------")
            Try
                If OriginalFullname.ToLower.EndsWith(".xls") Then
                    MsgBox("Exceldateien im alten Format (.xls) können nicht mehr aufgenommen werden. Bitte wandeln Sie die Datei zuerst in das neue Format (.xlsx) um!", MsgBoxStyle.Exclamation, "Wichtiger Hinweis")
                    Return False
                End If
                NumDir = ""
                Dim aktJPG As New clsMyJPG
                Dim ZielGesamtpfad As String = myGlobalz.Arc.rootDir.ToString & ArchivSubdir & NumDir
                _relativpfad = ArchivSubdir & NumDir
                erzeugeUnterverzeichnis(ZielGesamtpfad)
                'erzeugeUnterverzeichnis(ArchivSubdir & NumDir)
                Dim quelle As New IO.FileInfo(OriginalFullname)
                aktJPG.fullname = OriginalFullname
                If istFoto(OriginalFullname) Then
                    FotosExifpruefen(aktJPG, Beschreibung, OriginalFullname)
                End If

                aktJPG.fullname = ZielGesamtpfad & "\" & quelle.Name ' der dateiname wird nicht gespeichert


                test = checkIN_Database(aktJPG, _relativpfad, Beschreibung, ereignisID, OriginalFullname, lokVorgangsID, fotoZuRaumbezug,
                                        dateidatum, neudokid, NEWSAVEMODE, KOMPRESS, bearbeiterid)
                If test Then
                    Dim fotozwischenDatei As String
                    aktJPG.fullname = OriginalFullname
                    If istFoto(OriginalFullname) Then
                        If fotosverkleinern Then
                            fotozwischenDatei = IO.Path.GetTempPath() & "\" & glob2.getTimestamp & ".jpg"
                            If doFotoSizekorrektur(OriginalFullname, fotozwischenDatei, 2272) Then
                                OriginalFullname = fotozwischenDatei
                            Else

                            End If
                        End If
                    End If

                    _relativpfad = checkIN_FileArchiv(OriginalFullname, archivDateiFullname, erfolgreich,
                                                                NumDir, ArchivSubdir, NEWSAVEMODE, neudokid, KOMPRESS)
                    FileArchivTools.inputFileReadonlyEntfernen(archivDateiFullname)

                    If erfolgreich Then
                        ' QuelleLoeschenFallsMoven(Dokumente_moven, OriginalFullname)
                    Else
                        MsgBox("Fehler beim Kopieren der Datei ins Archiv." & Environment.NewLine &
                               "ServerPlatte voll ???." & Environment.NewLine &
                               "Die Datei wurde ins Archiv kopiert." & Environment.NewLine &
                               "Die Quelldatei ließ sich nicht löschen! Ggf. war sie noch geöffnet !?." & Environment.NewLine &
                               "Sie können die Datei von Hand löschen: " & OriginalFullname)
                    End If
                    myGlobalz.sitzung.aktDokument.dokumentPfad = _relativpfad
                    quelle = Nothing
                    Return True
                End If
                nachricht(String.Format("FEHLERin Checkin1: relativpfad oder Archivdateifullname sind unbrauchbar:  {0}rel:{1}{0}archivfullname:{2}", vbCrLf, _relativpfad,
                                                       archivDateiFullname & ", ori " & OriginalFullname))
                quelle = Nothing
                Return False
                nachricht("IN Checkin: ------------------ raus")
            Catch ex As Exception
                nachricht(String.Format("Fehler in	checkIN	dateiname${0}{1}", vbCrLf, ex))
                Return False
            End Try
        Else
            Try
                If OriginalFullname.ToLower.EndsWith(".xls") Then
                    MsgBox("Exceldateien im alten Format (.xls) können nicht mehr aufgenommen werden. Bitte wandeln Sie die Datei zuerst in das neue Format (.xlsx) um!")
                    Return False
                End If
                _relativpfad = checkIN_FileArchiv(OriginalFullname, archivDateiFullname, erfolgreich,
                                                  NumDir, ArchivSubdir, NEWSAVEMODE, 0, KOMPRESS)
                FileArchivTools.inputFileReadonlyEntfernen(archivDateiFullname)
                If erfolgreich Then
                    QuelleLoeschenFallsMoven(Dokumente_moven, OriginalFullname)
                Else
                    MsgBox("Fehler beim Kopieren der Datei ins Archiv!!!" & Environment.NewLine &
                           "Serverplatte voll ?" & Environment.NewLine &
                           "" & Environment.NewLine)
                    nachricht("Fehler beim Kopieren der Datei ins Archiv (a)!!!" & Environment.NewLine &
                           "Serverplatte voll ?" & Environment.NewLine &
                           "" & Environment.NewLine)
                    Return False
                End If
                myGlobalz.sitzung.aktDokument.dokumentPfad = _relativpfad
                If String.IsNullOrEmpty(_relativpfad) Or archivDateiFullname = "" Then
                    nachricht_und_Mbox(String.Format("FEHLERin Checkin1: relativpfad oder Archivdateifullname sind unbrauchbar:  {0}rel:{1}{0}archivfullname:{2}", vbCrLf, _relativpfad,
                                                           archivDateiFullname))
                    'kein schreibzugriff 
                    Return False
                Else
                    Dim aktJPG As New clsMyJPG
                    aktJPG.fullname = archivDateiFullname
                    test = checkIN_Database(aktJPG, _relativpfad, Beschreibung, ereignisID, OriginalFullname, lokVorgangsID, fotoZuRaumbezug,
                                            dateidatum, neudokid, NEWSAVEMODE, KOMPRESS, bearbeiterid)
                    Return test
                End If
                nachricht("IN Checkin: ------------------ raus")
            Catch ex As Exception
                nachricht_und_Mbox(String.Format("Fehler in	checkIN	dateiname${0}{1}", vbCrLf, ex))
                Return False
            End Try
        End If
    End Function

    Public Function ResizeImage(ByVal image As System.Drawing.Image, ByVal size As System.Drawing.Size,
                                Optional ByVal preserveAspectRatio As Boolean = True) As System.Drawing.Image
        Try
            Dim newWidth As Integer
            Dim newHeight As Integer

            If preserveAspectRatio Then
                Dim originalWidth As Integer = CInt(image.Width)
                Dim originalHeight As Integer = CInt(image.Height)
                Dim percentWidth As Single = CSng(size.Width) / CSng(originalWidth)
                Dim percentHeight As Single = CSng(size.Height) / CSng(originalHeight)
                Dim percent As Single = CSng(IIf(percentHeight < percentWidth, percentHeight, percentWidth))
                newWidth = CInt(originalWidth * percent)
                newHeight = CInt(originalHeight * percent)
            Else
                newWidth = CInt(size.Width)
                newHeight = CInt(size.Height)
            End If

            Dim newImage As System.Drawing.Image = New System.Drawing.Bitmap(newWidth, newHeight)
            Using graphicsHandle As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(newImage)
                graphicsHandle.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight)
            End Using

            Return newImage

        Catch ex As Exception
            Return image
        End Try
    End Function

    Private Function doFotoSizekorrektur(
                                    originalFullname As String,
                                    zielname As String, grenzwert As Integer) As Boolean
        ' 4mp 2272 × 1704
        'Dim sourceBM, target As New BitmapImage()
        Dim quelle, ziel As System.Drawing.Image
        Dim ratio As Double
        Dim newsize As New System.Drawing.Size
        Try
            quelle = System.Drawing.Image.FromFile(originalFullname)
            If quelle.Width <= grenzwert And quelle.Height <= grenzwert Then
                Return False
            End If
            If quelle.Width > grenzwert Then
                ratio = quelle.Height / quelle.Width
                newsize.Width = grenzwert
                newsize.Height = CInt(grenzwert * ratio)
            End If
            If quelle.Height > grenzwert Then
                ratio = quelle.Width / quelle.Height
                newsize.Height = grenzwert
                newsize.Width = CInt(grenzwert * ratio)
            End If
            ziel = ResizeImage(quelle, newsize, True)
            ziel.Save(zielname, System.Drawing.Imaging.ImageFormat.Jpeg)
            Return True
        Catch ex As Exception
            nachricht("Fehler in : doFotoSizekorrektur ---------ende-----------------", ex)
            Return False
        Finally
            If ziel IsNot Nothing Then
                ziel.Dispose()
                quelle.Dispose()
            End If

            ziel = Nothing
            quelle = Nothing
        End Try
    End Function

    Shared Function istFoto(originalFullname As String) As Boolean
        Try
            'l("istFoto---------------------- anfang")

            'test.Extension.ToLower = ".gif" Or
            '    test.Extension.ToLower = ".png"
            If originalFullname.IsNothingOrEmpty Then
                l("fehler Dateiname ist leer")
                Return False
            End If
            Dim test As New IO.FileInfo(originalFullname)
            If test.Extension.ToLower = ".jpg" Or
                test.Extension.ToLower = ".jpeg" Or
                test.Extension.ToLower = ".heic" Then
                Return True
            Else
                Return False
            End If
            l("istFoto---------------------- ende")
        Catch ex As Exception
            l("Fehler in istFoto : " & originalFullname, ex)
            Return False
        End Try
    End Function
    Shared Function istFotoTyp(typ As String) As Boolean
        Try
            l("istFotoTyp---------------------- anfang")
            If typ.ToLower = "jpg" Or
                typ.ToLower = "jpeg" Or
                typ.ToLower = "gif" Or
                typ.ToLower = "png" Then
                Return True
            Else
                Return False
            End If
            l("istFotoTyp---------------------- ende")
        Catch ex As Exception
            l("Fehler in istFotoTyp : ", ex)
            Return False
        End Try
    End Function

    'Public Overloads Function checkINDoku(ByVal OriginalFullname As String, _
    '                                    ByVal Beschreibung As String, _
    '                                    ByVal ereignisID As Integer, _
    '                                    ByVal Dokumente_moven As Boolean, _
    '                                    ByRef archivDateiFullname As String,
    '                                    ByRef NumDir As String,
    '                                    ByVal lokVorgangsID As Integer,
    '                                    ByVal fotoZuRaumbezug As Boolean,
    '                                    dateidatum As Date,
    '                                    ByRef neudokid As Integer,
    '                                    ByVal ArchivSubdir As String,
    '                                    NEWSAVEMODE As Boolean) As Boolean
    '    Dim test, erfolgreich As Boolean
    '    nachricht("IN Checkin: ---------------------")
    '    Try
    '        Dim aktJPG As New clsMyJPG
    '        aktJPG.fullname = archivDateiFullname
    '        test = checkIN_Database(aktJPG, _relativpfad, Beschreibung, ereignisID, OriginalFullname, lokVorgangsID, fotoZuRaumbezug,
    '                                dateidatum, neudokid, NEWSAVEMODE)
    '        If test > 0 Then
    '            _relativpfad = checkIN_FileArchiv(OriginalFullname, archivDateiFullname, erfolgreich,
    '                                                        NumDir, ArchivSubdir, NEWSAVEMODE)
    '            If erfolgreich Then QuelleLoeschenFallsMoven(Dokumente_moven, OriginalFullname)
    '            myGlobalz.sitzung.aktDokument.dokumentPfad = _relativpfad
    '        End If


    '        If String.IsNullOrEmpty(_relativpfad) Or archivDateiFullname = "" Then
    '            nachricht_und_Mbox(String.Format("FEHLERin Checkin: relativpfad oder Archivdateifullname sind unbrauchbar:  {0}rel:{1}{0}archivfullname:{2}", vbCrLf, _relativpfad,
    '                                                   archivDateiFullname))
    '            'kein schreibzugriff 
    '            Return False
    '        Else

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

    Private Function FotosExifpruefen(ByVal aktJPG As clsMyJPG, ByVal Beschreibung As String, OriginalFullname As String) As Boolean
        Try
            Dim test As New IO.FileInfo(aktJPG.fullname)


            If istFoto(aktJPG.fullname.ToLower) Then
                Dim ExifQ As ExifWorksQuick = Nothing
                If hatExif(aktJPG, ExifQ) Then
                    If aktJPG.EXIFDateTimeOriginal = #12:00:00 AM# Then aktJPG.EXIFDateTimeOriginal = test.CreationTime
                    BeschreibungInRB_mappen(aktJPG, Beschreibung$)
                Else
                    'damit NUr JPGS als Raumbezug gewertet werden
                    aktJPG.Exifgpslatitude = ""
                    aktJPG.Exifgpslongitude = ""
                    test = Nothing
                    Return False
                End If
            End If
            test = Nothing
            Return True
        Catch ex As Exception
            nachricht(String.Format("	FotosExifpruefen	{0}{1}", vbCrLf, ex))
            Return False
        End Try
    End Function

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
            Return False
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
    Private Shared Sub FotoMitExifKoordinate(ByVal aktjpg As clsMyJPG, ByVal zielVorgangsid As Integer,
                                             ByVal fotoZuRaumbezug As Boolean, ByVal test As IO.FileInfo)
        Try
            If istFoto(test.FullName.ToLower) And hasCoords(aktjpg) Then
                If fotoZuRaumbezug Then
                    inRaumbezuguebernehmen(aktjpg, zielVorgangsid)
                End If
            End If
        Catch ex As Exception
            nachricht("Fehler in FotoMitExifKoordinate: ", ex)
        End Try
    End Sub
    'Private Overloads Shared Function checkin_Dokumente(ByVal dateiname$, ByVal relativpfad$, ByVal Beschreibung$,OriginalFullname$,OriginalName as string) as  Integer
    '	Try
    '		Dim aktjpg As New clsMyJPG
    '		aktjpg.fullname = dateiname
    '		checkin_Dokumente(aktjpg, relativpfad, Beschreibung,OriginalFullname$,OriginalName)
    '	Catch ex As Exception
    '		nachricht("checkin_Dokumente dateiname$ " & vbCrLf ,ex)
    '	End Try
    'End Function

    Public Function checkIN_Database(ByVal aktjpg As clsMyJPG,
                                        ByVal relativpfad As String,
                                        ByVal Beschreibung As String,
                                        ByVal ereignisID As Integer,
                                        ByVal OriginalFullname As String,
                                        ByVal zielVorgangsid As Integer,
                                        ByVal fotoZuRaumbezug As Boolean,
                                        ByVal dateidatum As Date,
                                        ByRef neudokumentID As Integer,
                                        ByVal NEWSAVEMODE As Boolean,
                                        ByVal KOMPRESS As Boolean,
                                        bearbeiterid As Integer) As Boolean
        Dim fotodatei As New IO.FileInfo(aktjpg.fullname)
        Dim oritest As New IO.FileInfo(OriginalFullname)
        Dim OriginalName = oritest.Name
        oritest = Nothing
        nachricht("in		checkIN_Database ---------------------------")
        'FotosExifpruefen(aktjpg, Beschreibung,OriginalFullname)
        neudokumentID = DokArcTools.dokSpeichernNeu.execute(aktjpg, relativpfad, Beschreibung, OriginalFullname, OriginalName, dateidatum,
                                                            zielVorgangsid, ereignisID, NEWSAVEMODE, KOMPRESS, bearbeiterid)
        If neudokumentID > 0 Then
            nachricht("in		checkIN_Database ---------------------------   erfolgreich")
            aktjpg.DokumentID = neudokumentID
            FotoMitExifKoordinate(aktjpg, zielVorgangsid, fotoZuRaumbezug, fotodatei)
            nachricht("in		checkIN_Database KopplungMitVorgang-----------------------------------------------------")
            'kopplenMitVorgang(zielVorgangsid, neudokumentID)
            'kopplenMitEreignis(ereignisID, neudokumentID)
            fotodatei = Nothing
            Return True
        Else
            nachricht("in		checkIN_Database --------------------------- , NICHT erfolgreich")
            Return False
        End If

    End Function










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
    '        nachricht("Fehler in istvoll:" ,ex)
    '    End Try
    'End Function

    Function leseZeigerdatei() As Boolean
        nachricht("in leseZeigerdatei")
        Return False
    End Function

    'Public Overloads Function DataTable_auschecken() As Boolean
    '    Dim res As Boolean = DataTable_auschecken(myglobalz.Arc.lokalerCheckoutcache & myglobalz.sitzung.aktVorgangsID, True)
    '    My.Log.WriteEntry("a0")
    '    Return res
    'End Function

    Public Overloads Function DataTable_auschecken(ByVal zielVerzeichnis As String,
                                                   ByVal verzeichnisOeffnen As Boolean) As Boolean
        Dim quelle As String = "", ziel As String = ""
        Dim anzahl As Integer = myGlobalz.Arc.ArcRec.dt.Rows.Count
        Dim i As Integer = 0
        Dim FIquelle, FIziel As IO.FileInfo
        For Each dok As DataRow In myGlobalz.Arc.ArcRec.dt.AsEnumerable
            Try
                'Quelldatei_definieren
                quelle = myGlobalz.Arc.rootDir.ToString & dok.Item("relativpfad").ToString.Replace("/", "\")
                Dim NewSaveMode As Boolean = CBool(dok.Item("NEWSAVEMODE"))
                If NewSaveMode Then
                    quelle = quelle & "\" & dok.Item("DOKUMENTID").ToString
                Else
                    quelle = quelle & "\" & dok.Item("dateinameext").ToString
                End If
                nachricht("  quelle: " & quelle)
                'Zieldatei_definieren
                ziel = zielVerzeichnis & "\" & dok.Item("dateinameext").ToString
                nachricht("  ziel: " & ziel)
                'Zielverzeichnis_erstellen
                nachricht("Erzeuge Zielverzeichnis: zielVerzeichnis$: " & zielVerzeichnis)
                Dim erfolg As IO.DirectoryInfo = IO.Directory.CreateDirectory(zielVerzeichnis)
                If erfolg.Exists Then
                    nachricht("zielVerzeichnis wurde erzeugtb!")
                Else
                    nachricht("zielVerzeichnis wurde NICHT erzeugt!")
                End If
                'wenn quelldateifehlt_exit
                Try
                    FIquelle = New IO.FileInfo(quelle)
                Catch ex As Exception
                    nachricht("fehler in DataTable_auscheckenc: Datei konnte nicht gefunden werden. " &
                              quelle & Environment.NewLine &
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
                    nachricht("fehler in DataTable_auscheckend: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                     ex.ToString)
                    Continue For
                End Try

                If FIziel.Exists Then
                    nachricht("ziel existiert!")
                    ziel = neuerdateinameFuerZiel(ziel, zielVerzeichnis, dok)
                    dok.Item("dateinameext") = ziel
                End If

                'kopieren
                nachricht("  vor zieltestb: " & ziel)
                i += 1
                ueberkopierenNachCheckout(ziel, FIquelle, CBool(dok.Item("kompress")))

                My.Log.WriteEntry("a1") : Dim aktdo As New Dokument
                My.Log.WriteEntry("a2") : aktdo.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                My.Log.WriteEntry("a3") : myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                'My.Log.WriteEntry("a4") : DokumentDatarow2Obj(dok, aktdo)                        'addieren
                'My.Log.WriteEntry("a5") : zuCheckoutlisteAddieren(aktdo, myglobalz.sitzung.checkoutDokuList)
                My.Log.WriteEntry("a6")
            Catch ex As Exception
                nachricht("fehler Problem beim Auschecken von:  " & ziel & vbCrLf, ex)
                Return False
            End Try
        Next
        FIquelle = Nothing : FIziel = Nothing
        Oeffneverzeichnis(verzeichnisOeffnen, zielVerzeichnis)
        Return True
    End Function

    Private Shared Sub Oeffneverzeichnis(ByVal verzeichnisOeffnen As Boolean, ByVal zielVerzeichnis As String)
        Try
            Dim testdatei = IO.Path.Combine(zielVerzeichnis)
            nachricht(testdatei)
            If verzeichnisOeffnen Then System.Diagnostics.Process.Start(testdatei)
        Catch ex As Exception
            nachricht("Fehler in Oeffneverzeichnis:   ", ex)
        End Try
    End Sub

    Public Shared Function ueberkopierenNachCheckout(ByVal ziel As String, ByVal FIquelle As IO.FileInfo, kompress As Boolean) As Int16
        Try
            nachricht("Quelle: " & FIquelle.FullName)
            nachricht("Ziel: " & ziel)
            DokArc.copyOrDekompresscopy(FIquelle, ziel, kompress)
            'FIquelle.CopyTo(ziel, True)

            System.Threading.Thread.Sleep(500)
            Return 0
        Catch ex As Exception
            nachricht("FEHLER beim ueberkopieren Kopieren von:  " & FIquelle.FullName & ", nach: " & ziel & " (zieldatei ist bestimmt geöffnet by user)")
            Return 1
        End Try
    End Function

    Public Sub AllesAuscheckenVorgang(ByVal verzeichnisOeffnen As Boolean, BOOLverzeichnisVORHERloeschen As Boolean)
        Dim bresult As Boolean
        Dim zieldir As String = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID
        verzeichnisInhaltLoeschen(zieldir, BOOLverzeichnisVORHERloeschen)
        'Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides")
        bresult = DokArcTools.dokusVonVorgangHolen.execute(CStr(myGlobalz.sitzung.aktVorgangsID), "beides", alleBilder:=True, 0)
        'Dim task As System.Threading.Tasks.Task(Of Boolean) = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides")
        'bresult = Await task
        If bresult Then
            myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
            If myGlobalz.Arc.DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, verzeichnisOeffnen) Then
                IO.Directory.CreateDirectory(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
                If verzeichnisOeffnen Then System.Diagnostics.Process.Start(String.Format("{0}\{1}",
                        myGlobalz.Arc.lokalerCheckoutcache,
                        myGlobalz.sitzung.aktVorgangsID))
            End If
        End If
    End Sub

    Public Sub AllesAuscheckenVorgangOBJ(ByVal verzeichnisOeffnen As Boolean,
                                      BOOLverzeichnisVORHERloeschen As Boolean,
                                      dliste As List(Of clsPresDokumente))
        Dim zieldir As String = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID
        verzeichnisInhaltLoeschen(zieldir, BOOLverzeichnisVORHERloeschen)
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

    Public Sub AuscheckenVorgangEreignis()
        Dim bresult As Boolean = DokArcTools.dokusVonEreignisHolen.execute(myGlobalz.sitzung.aktEreignis.ID)
        If bresult Then
            myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
            If myGlobalz.Arc.DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, True) Then
                Process.Start(myGlobalz.Arc.lokalerCheckoutcache & "\" & myGlobalz.sitzung.aktVorgangsID)
            End If
        End If
    End Sub



    Public Shared Function DokumentDatarow2Obj(ByVal item As DataRow) As clsPresDokumente
        Dim dokulok As New clsPresDokumente
        Try
            dokulok.DocID = CInt(item("DOKUMENTID"))
            dokulok.dokumentPfad = item("RELATIVPFAD").ToString
            dokulok.DateinameMitExtension = item("DATEINAMEEXT").ToString
            dokulok.Typ = item("TYP").ToString
            dokulok.Beschreibung = item("BESCHREIBUNG").ToString
            dokulok.Filedatum = clsDBtools.fieldvalueDate(item("FILEDATUM"))
            dokulok.Checkindatum = clsDBtools.fieldvalueDate(item("CHECKINDATUM"))

            dokulok.revisionssicher = CBool(clsDBtools.toBool(item("revisionssicher")))
            Try

                dokulok.sizeMb = CDbl((clsDBtools.fieldvalue(item("MB"))))
            Catch ex As Exception
                dokulok.sizeMb = 0
            End Try
            dokulok.kompressed = CBool(clsDBtools.toBool(item("KOMPRESS")))
            dokulok.Initiale = CStr(clsDBtools.fieldvalue(item("INITIAL_")))
            Try
                dokulok.ExifDatum = clsDBtools.fieldvalueDate(item("EXIFDATUM"))
                dokulok.EreignisID = CInt(item("EID").ToString)
            Catch ex As Exception
                nachricht("fehler in DokumentDatarowView2Obj ExifDatum" & vbCrLf & vbCrLf, ex)
            End Try
            dokulok.VorgangsID = CInt(item("VID"))
            dokulok.EreignisID = CInt(item("EID"))
            dokulok.newSaveMode = CBool(clsDBtools.toBool(item("NEWSAVEMODE")))
            Try
                dokulok.istNurVerwandt = CBool(clsDBtools.toBool(item("status")))
            Catch ex As Exception
                nachricht("fehler in DokumentDatarowView2Obj istNurVerwandt" & vbCrLf & vbCrLf, ex)
            End Try
            dokulok.istVeraltet = CBool(clsDBtools.toBool(item("VERALTET")))
            dokulok.ExifDatum = clsDBtools.fieldvalueDate(item("EXIFDATUM"))
            dokulok.EXIFlon = CStr(item("EXIFLONG").ToString)
            dokulok.EXIFlat = CStr(item("EXIFLAT").ToString)
            dokulok.EXIFdir = CStr(item("EXIFDIR").ToString)
            dokulok.EXIFhersteller = CStr(item("EXIFHERSTELLER").ToString)

            Return dokulok
        Catch ex As Exception
            nachricht("fehler in DokumentDatarowView2Obj a" & vbCrLf & vbCrLf, ex)
            Return dokulok
        End Try
    End Function
    Public Shared Sub DokumentDatarowView2Obj(ByVal item As DataRowView, ByVal dokulok As Dokument)
        Try
            dokulok.DocID = CInt(item("DOKUMENTID")) '0
            dokulok.dokumentPfad = item("RELATIVPFAD").ToString '1
            dokulok.DateinameMitExtension = item("DATEINAMEEXT").ToString '2
            dokulok.Typ = item("TYP").ToString '3
            dokulok.Beschreibung = item("D_BESCHREIBUNG").ToString '4
            '    dokulok.Beschreibung = item("BESCHREIBUNG").ToString '4
            dokulok.Filedatum = CDate(item("FILEDATUM").ToString) '5
            dokulok.Checkindatum = CDate(item("CHECKINDATUM").ToString) '6
            dokulok.istVeraltet = CBool(clsDBtools.toBool(item("VERALTET"))) '7
            dokulok.ExifDatum = CDate(item("EXIFDATUM").ToString) '8
            dokulok.EXIFlon = CStr(item("EXIFLONG").ToString) '9
            dokulok.EXIFlat = CStr(item("EXIFLAT").ToString) '10
            dokulok.EXIFdir = CStr(item("EXIFDIR").ToString) '11
            dokulok.EXIFhersteller = CStr(item("EXIFHERSTELLER").ToString) '12
            dokulok.revisionssicher = CBool(clsDBtools.toBool(item("revisionssicher")))
            Try

                dokulok.sizeMb = CDbl((clsDBtools.fieldvalue(item("MB"))))
            Catch ex As Exception
                dokulok.sizeMb = 0
            End Try
            dokulok.kompressed = CBool(clsDBtools.toBool(item("KOMPRESS")))
            dokulok.Initiale = CStr(clsDBtools.fieldvalue(item("INITIAL_")))
            dokulok.EreignisID = CInt(clsDBtools.fieldvalue(item("EID")))
            dokulok.VorgangsID = CInt(clsDBtools.fieldvalue(item("VID")))
            dokulok.newSaveMode = CBool(clsDBtools.toBool(item("NEWSAVEMODE")))
            Try
                dokulok.istNurVerwandt = CBool(clsDBtools.toBool(item("status")))
            Catch ex As Exception

            End Try

        Catch ex As Exception
            nachricht("fehler in DokumentDatarowView2Obj b" & vbCrLf & vbCrLf, ex)
        End Try
    End Sub
    Public Shared Sub JPG_handeln(ByVal aktdoku As Dokument, allebilder As Boolean, eid As Integer)
        Try
            'If DokArc.aktiviereFotoGucker(myGlobalz.sitzung.aktVorgangsID, myGlobalz.OhneObsoletenDokus, eid, allebilder) > 0 Then
            '    Dim winfotoguck = New winFotoGucker(aktdoku)
            '    winfotoguck.ShowDialog()
            '    glob2.MeinGarbage()
            'Else
            '    nachricht_und_Mbox("Keine Fotos gefunden")
            'End If
            If allebilder Then
                eid = 0
            End If
            nsDivers.clsdivers.externerFotogugger(CStr(myGlobalz.sitzung.aktVorgangsID), eid, aktdoku.DocID, 0)
        Catch ex As Exception
            nachricht("JPG " & vbCrLf & vbCrLf, ex)
        End Try
    End Sub

    Public Shared Function Archiv_aktiviere_dokument(ByVal aktdoku As Dokument, readOnlyDoxsInTxtCrtlOeffnen As Boolean, allebilder As Boolean, eid As Integer) As Boolean
        aktdoku.FullnameImArchiv = aktdoku.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        If Not String.IsNullOrEmpty(aktdoku.FullnameImArchiv) Then
            Return doktypOeffnen(aktdoku, readOnlyDoxsInTxtCrtlOeffnen, allebilder, eid)
        Else
            nachricht_und_Mbox("Dateiname ist ungültig:" & aktdoku.FullnameImArchiv)
        End If
    End Function

    Public Shared Function doktypOeffnen(ByVal aktdoku As Dokument, readOnlyDoxsInTxtCrtlOeffnen As Boolean, allebilder As Boolean, eid As Integer) As Boolean
        Dim test As New IO.FileInfo(aktdoku.FullnameCheckout)
        Dim WordStartIsOK As Boolean = False
        'If test.Exists Then
        Select Case test.Extension.ToLower
            Case ".vcf"
                Dim diares As New MessageBoxResult
                diares = MessageBoxResult.Yes
                If diares = MessageBoxResult.Yes Then
                    Dim vsf As New winVCF(aktdoku.FullnameCheckout)
                    vsf.ShowDialog()
                ElseIf diares = MessageBoxResult.No Then
                    DokArc.DokumentStarten(aktdoku)
                Else
                    'gornix
                End If
            Case ".txt", ".log", ".bat", ".vb", ".xaml", ".sql"
                TXT_handeln(aktdoku, "edit")
            'Case ".png", ".gif"
            '    GIF_handeln(aktdoku)
            Case ".png", ".gif", ".jpg", ".jpeg"  'foto
                JPG_handeln(aktdoku, allebilder, eid)

            Case ".3ga"
                If myGlobalz.sitzung.aktDokument.FullnameCheckout.Trim.ToLower.EndsWith(".3ga") Then
                    Dim ziel As String = kopiere3ganach3gp()
                    sounddateiStarten(ziel)
                End If
            Case ".rtf"
                l("nurzumlesen:" & aktdoku.nurzumlesen)
                l("aktdoku.revisionssicher:" & aktdoku.revisionssicher)
                If aktdoku.nurzumlesen Or aktdoku.revisionssicher Then
                    WordStartIsOK = DokArc.WordReadonlyDokumentStarten(aktdoku.FullnameCheckout, readOnlyDoxsInTxtCrtlOeffnen) 'im zweifel immer den checkout verwenden
                    Return WordStartIsOK
                Else
                    myGlobalz.didEverOpenAWordDocInSession = True
                    MessageBox.Show("RTF-Dokumente werden nach Änderung nicht ins Archiv zurückgeführt!!! " & Environment.NewLine &
                                    "Änderungen werden verloren gehen !!!", "Datenverlust ist bei RTF sicher", MessageBoxButton.OK, MessageBoxImage.Error)
                    DokArc.DokumentStarten(aktdoku) 'im zweifel immer den checkout verwenden 
                End If
            Case ".doc", ".docx"
                l("nurzumlesen:" & aktdoku.nurzumlesen)
                l("aktdoku.revisionssicher:" & aktdoku.revisionssicher)
                If aktdoku.nurzumlesen Or aktdoku.revisionssicher Then
                    If aktdoku.FullnameCheckout.ToLower.EndsWith(".doc") Or aktdoku.FullnameCheckout.ToLower.EndsWith(".docx") Then
                        l("doc oder docx die nurzumlesen geöffnet werden: " & aktdoku.FullnameCheckout)
                        'If docInDocxUmbenennenOK(aktdoku) Then
                        'Else
                        '    MsgBox("Problem beim Bereitstellen der Datei. Netzwerk zu langsam? Rechner überlastet?")
                        'End If
                    Else
                        l("Fehler in doktypoeffnen problem mit der endung")
                    End If
                    WordStartIsOK = DokArc.WordReadonlyDokumentStarten(aktdoku.FullnameCheckout, readOnlyDoxsInTxtCrtlOeffnen) 'im zweifel immer den checkout verwenden
                    Return WordStartIsOK
                Else
                    myGlobalz.didEverOpenAWordDocInSession = True
                    If myGlobalz.PumuckelVersion = 1 Then
                        'wird von pumuckel gestartet
                    End If
                    If myGlobalz.PumuckelVersion = 0 Then
                        DokArc.DokumentStarten(aktdoku)
                    End If
                End If
            Case Else
                If aktdoku.revisionssicher Then
                    MessageBox.Show(glob2.getMsgboxText("infotextRevisionssicherheit", New List(Of String)(New String() {})),
                                    aktdoku.FullnameCheckout, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
                End If
                DokArc.DokumentStarten(aktdoku) 'im zweifel immer den checkout verwenden 
        End Select
        test = Nothing
        Return True
    End Function

    Public Shared Sub TXT_handeln(aktdoku As Dokument, modus As String)
        Dim text As String
        text = detailsTools.getTextINhalt(aktdoku.makeFullname_ImArchiv(myGlobalz.Arc.rootDir))
        Dim txtedit As New winTXT(text, aktdoku, modus, aktdoku.DocID)
        txtedit.ShowDialog()
    End Sub

    Private Shared Function docInDocxUmbenennenOK(aktdoku As Dokument) As Boolean
        Dim doxnameString As String
        l("docInDocxUmbenennenOK -----------------")
        Try
            doxnameString = DocXname(aktdoku.FullnameCheckout, aktdoku.DocID)
            l("neuername: " & doxnameString)
            FileArchivTools.inputFileReadonlyEntfernen(aktdoku.FullnameCheckout)
            Dim erfolg As Boolean = dateiloeschen(doxnameString)
            If dateiUmbenennenOK(aktdoku, doxnameString) Then
                aktdoku.FullnameCheckout = doxnameString
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("fehler in docInDocxUmbenenne; ", ex)
            Return False
        End Try
    End Function

    Private Shared Function dateiUmbenennenOK(aktdoku As Dokument, NeuDoxnameString As String) As Boolean
        Dim fi As IO.FileInfo
        Threading.Thread.Sleep(100) ' netzwerkprobleme bei rüberkopieren
        Try
            fi = New IO.FileInfo(aktdoku.FullnameCheckout)
            fi.MoveTo(NeuDoxnameString)
            fi = Nothing
            Return True
        Catch ex As Exception
            nachricht("fehler in dateiUmbenennenOK; " & aktdoku.FullnameCheckout & "=> " & NeuDoxnameString & ": ", ex)
            Return False
        End Try
    End Function



    Public Shared Function DocXname(fullnameCheckout As String, dokid As Integer) As String
        Dim newname, vorname, pfad, ts As String
        Dim fi As New IO.FileInfo(fullnameCheckout)
        Try
            pfad = fi.DirectoryName
            vorname = dokid.ToString
            ts = Now.ToString("yyyyMMddHHmmss")
            newname = IO.Path.Combine(pfad, vorname & "_" & ts & ".docx")
            fi = Nothing
            Return newname
        Catch ex As Exception
            nachricht("fehler in docxname ", ex)
            Return ""
        End Try

    End Function

    Private Shared Function kopiere3ganach3gp() As String
        Dim quelle As IO.FileInfo
        Dim ziel As String = ""
        Try
            quelle = New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
            ziel = myGlobalz.sitzung.aktDokument.FullnameCheckout.ToLower.Replace(".3ga", ".3gp")
            quelle.CopyTo(ziel, True)
            quelle = Nothing
            Return ziel
        Catch ex As Exception
            nachricht("fehler in kopiere3ganach3gp: ", ex)
            Return ziel
        End Try
    End Function

    Private Shared Sub sounddateiStarten(ByVal ziel As String)
        System.Threading.Thread.Sleep(1000)
        Try
            System.Diagnostics.Process.Start(ziel)
        Catch ex As Exception
            nachricht("Fehler in DateiStarten: ", ex)
        End Try
    End Sub
    Public Shared Sub Archiv_definiereAktdokument(ByVal item As DataRowView)
        myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
        DokumentDatarowView2Obj(item, myGlobalz.sitzung.aktDokument)
        setzeVerwandschaftsstatus(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID)
        myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        Dim ausgabeVerzeichnis As String = ""
        myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
    End Sub

    'Public Shared Function zulisteAddieren() As Boolean
    '    Dim newdog As New Dokument
    '    l("zulisteAddieren-------------------------------------- anfang")
    '    'newdog = CType(myglobalz.sitzung.aktDokument.Clone, Dokument)
    '    newdog = clsPresDokumente.dokument2Presdokument(myglobalz.sitzung.aktDokument)

    '    Dim erfolg As Boolean
    '    erfolg = zuCheckoutlisteAddieren(newdog, myglobalz.sitzung.checkoutDokuList)
    '    If myglobalz.sitzung.aktDokument.getDokTyp = DokumentenTyp.DOC Then
    '        If erfolg Then
    '            myglobalz.sitzung.aktDokument.nurzumlesen = True
    '        End If
    '    Else

    '    End If
    '    l("nurzumlesen2 : " & myglobalz.sitzung.aktDokument.nurzumlesen)
    '    l("zulisteAddieren-------------------------------------- ende")
    '    Return erfolg
    'End Function

    ''' <summary>
    ''' arbeitet mit aktDokument
    ''' </summary>
    ''' <param name="modus">zeige oder metaedit</param>
    ''' <remarks></remarks>
    Public Shared Function machCheckout(ByRef lfehler As String) As Boolean
        l("machCheckout------------------------")
        If myGlobalz.sitzung.aktDokument Is Nothing Then
            nachricht("FEHLER: Dokument ist nothing")
            Return False
        End If
        If detailsTools.dokumentIstGeoeffnet(myGlobalz.sitzung.aktDokument) Then
            If tempOfficeDateiNochVorhanden(myGlobalz.sitzung.aktDokument) Then
                MessageBox.Show(glob2.getMsgboxText("machcheckout2", New List(Of String)(New String() {})),
                               "Office-Dokument in Bearbeitung")
                Return False
            Else
                Return True
            End If
            Return False
        End If
        lfehler = checkout.checkout(myGlobalz.sitzung.aktDokument, myGlobalz.sitzung.aktVorgangsID)
        If lfehler.ToLower.StartsWith("fehler") Then   'checkout findet IMMER statt
            Return False
        End If
        Return True
    End Function

    'Public Shared Function istInCheckoutListe(ByVal dok As Dokument, ByRef dieliste As List(Of Dokument)) As Boolean
    '    If dok Is Nothing Then Return False
    '    If dieliste Is Nothing Then Return False
    '    If listeEnthaeltBereitsDiesesDokument(dok, dieliste) Then
    '        Return True
    '    Else
    '        Return False
    '    End If
    '    Return False
    'End Function

    'Public Shared Function zuCheckoutlisteAddieren(ByVal dok As Dokument, ByRef dieliste As List(Of Dokument)) As Boolean
    '    If dok Is Nothing Then Return True
    '    If dieliste Is Nothing Then Return True
    '    If Not dok.isTypeEditable Then Return True
    '    If dok.revisionssicher Then
    '        'soll ja nicht editiert werden
    '        l("ist revisionssicher also nur lesen")
    '        Return True
    '    End If
    '    If dok.DokTyp = DokumentenTyp.DOC Then
    '        If listeEnthaeltWordDokument(dieliste) Then
    '            MsgBox("Es ist bereits eine Worddatei zum Editieren geladen. " &
    '                   "Entweder Sie entladen die alte Worddatei " &
    '                   "oder sie laden die neue Worddatei mit dem Wordviewer !!! " & Environment.NewLine &
    '                   "--------------------" & Environment.NewLine &
    '                   "Falls Paradigma über das Schließen der Datei nicht unterrichtet wurde: " & Environment.NewLine &
    '                   ">>> Schließen und Öffnen Sie den Vorgang. "
    '                   )
    '            l("Es ist bereits eine Worddatei zum Editieren geladen:  also nur lesen ")
    '            Return True
    '        Else
    '            ' MsgBox("aa")
    '        End If
    '    End If
    '    If Not listeEnthaeltBereitsDiesesDokument(dok, dieliste) Then
    '        l("not listeEnthaeltDokument, also addiert:" & dok.FullnameCheckout)
    '        dieliste.Add(dok)
    '    Else
    '        l("Es ist bereits eine allg.datei zum Editieren geladen:  also nur lesen ")
    '    End If
    '    Return False
    'End Function

    Public Shared Sub ausCheckoutlisteEntfernen(ByVal dok As Dokument, ByRef dieliste As List(Of Dokument))
        If dok Is Nothing Then Exit Sub
        If dieliste Is Nothing Then Exit Sub
        If listeEnthaeltBereitsDiesesDokument(dok, dieliste) Then
            dieliste.Remove(dok)
        End If
    End Sub

    Shared Function aktiviereFotoGucker(ByVal vorgangsID%, ByVal OhneObsoletenDoku As Boolean, eid As Integer, alleBilder As Boolean) As Integer
        'alle Fotos in Objektliste packen
        Dim auchVeraltete = True
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(vorgangsID), "nurfotos", alleBilder, eid)

        'Dim task As System.Threading.Tasks.Task(Of Boolean) = DokArcTools.dokusVonVorgangHolen.execute(vorgangsID, "nurfotos")
        'Dim bresult As Boolean = Await task
        'initDokumente4VorgangDatatable(vorgangsID, OhneObsoletenDoku)  'alle dokus
        Return alleFotosinCollectionPacken(auchVeraltete, vorgangsID)
    End Function

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
                zieldokulok.istVeraltet = CBool(clsDBtools.toBool(.Rows(i).Item("VERALTET")).ToString)

                If String.IsNullOrEmpty(clsDBtools.fieldvalue(.Rows(i).Item("EXIFDATUM")).ToString.Trim) OrElse
                    clsDBtools.fieldvalue(.Rows(i).Item("EXIFDATUM")).ToString.Trim.Length < 3 Then
                    zieldokulok.ExifDatum = CLstart.mycSimple.MeinNULLDatumAlsDate
                Else
                    zieldokulok.ExifDatum = CDate(clsDBtools.fieldvalue(.Rows(i).Item("EXIFDATUM")))
                End If
                zieldokulok.EXIFlon = clsDBtools.fieldvalue(.Rows(i).Item("EXIFLONG")).ToString
                zieldokulok.EXIFlat = clsDBtools.fieldvalue(.Rows(i).Item("EXIFLAT")).ToString
                zieldokulok.EXIFdir = clsDBtools.fieldvalue(.Rows(i).Item("EXIFDIR")).ToString
                zieldokulok.EXIFhersteller = clsDBtools.fieldvalue(.Rows(i).Item("EXIFHERSTELLER")).ToString
                'zieldokulok.OriginalFullname = clsDBtools.fieldvalue(.Rows(i).Item("ORIGINALFULLNAME")).ToString
                'zieldokulok.OriginalName = clsDBtools.fieldvalue(.Rows(i).Item("ORIGINALNAME")).ToString
                zieldokulok.revisionssicher = CBool(clsDBtools.toBool(.Rows(i).Item("REVISIONSSICHER")).ToString)
                zieldokulok.sizeMb = CDbl((clsDBtools.fieldvalue(.Rows(i).Item("MB"))))
                zieldokulok.kompressed = CBool(clsDBtools.toBool(.Rows(i).Item("KOMPRESS"))) 'ihah
                zieldokulok.Initiale = clsDBtools.fieldvalue(.Rows(i).Item("INITIAL_")).ToString
                zieldokulok.EreignisID = CInt(clsDBtools.fieldvalue(.Rows(i).Item("EID"))) 'ihah
                zieldokulok.VorgangsID = CInt(clsDBtools.fieldvalue(.Rows(i).Item("VID"))) 'ihah
                zieldokulok.newSaveMode = CBool(clsDBtools.toBool(.Rows(i).Item("NEWSAVEMODE"))) 'ihah
            End With
        Catch ex As Exception
            nachricht("Fehler in DokuZeile2OBJ:", ex)
        End Try
    End Sub

    Public Shared Function alleFotosinCollectionPacken(ByVal auchVeraltete As Boolean, ByVal VorgangsID As Integer) As Integer
        Dim ihinzu% = 0
        Try
            CLstart.myc.collFotos.Clear()
            For i = 0 To myGlobalz.Arc.ArcRec.dt.Rows.Count - 1
                If istFotoTyp(myGlobalz.Arc.ArcRec.dt.Rows(i).Item("Typ").ToString.ToLower) Then
                    Dim dokulok As New clsFotoDokument
                    DokuZeile2OBJ(i, dokulok, myGlobalz.Arc.ArcRec.dt)
                    If dokulok.ExifDatum < CDate("1970-01-01") Then dokulok.ExifDatum = dokulok.Filedatum
                    If Not dokulok.istVeraltet Or auchVeraltete Then
                        dokulok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                        Dim ausgabeVerzeichnis As String = ""
                        dokulok.makeFullname_Checkout(VorgangsID, myGlobalz.Arc.lokalerCheckoutcache, dokulok.DocID, ausgabeVerzeichnis)
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
    Public Function getplainExifdata(ByRef aktJPG As clsMyJPG,
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
                exifQ.Dispose()
                exifQ = Nothing
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
            exifQ.Dispose()
            ' exifQ = Nothing
            Return True
        Catch ex As Exception
            exifQ.Dispose()
            nachricht("fehler in getplainExifdata: " & ex.Message)
            Return False
        End Try
    End Function
    Shared Function dateiloeschen(datei As String) As Boolean
        Dim testdatei As IO.FileInfo
        l("in docInDocxUmbenenne----------------------------")
        Try
            testdatei = New IO.FileInfo(datei)
            If Not testdatei.Exists() Then
                nachricht("datei existiert nicht:  " & datei & ". löschen wird ignoriert.")
                testdatei = Nothing
                Return True
            End If
            testdatei.Delete()
            testdatei = Nothing
            Return True
        Catch ex As Exception
            nachricht("fehler in dateiloeschen: " & ex.Message)
            Return False
        End Try
    End Function


    Private Shared Function loescheDokumentImFilesystem(ByVal aktDokument As Dokument) As Boolean
        glob2.MeinGarbage()
        nachricht("loescheDokumentImFilesystem -----------------------------")
        Dim atest As String = aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        nachricht("datei:  " & atest)
        Dim testdatei As New IO.FileInfo(atest)
        Try
            If Not testdatei.Exists() Then
                nachricht("datei existiert nicht:  " & atest & ". löschen wird ignoriert.")
                testdatei = Nothing
                Return True
            End If
            testdatei.Delete()
            testdatei = Nothing
            nachricht("datei existiert :  " & atest & ". löschen erfolgreich.")
            Return True
        Catch ex As Exception
            nachricht("Fehler: loescheDokumentImFilesystem Datei konnte nicht gelöscht werden: " & "datei existiert nicht:  " & atest, ex)
            testdatei = Nothing
            Return False
        End Try
    End Function

    Private Shared Function revisionssicher(dok As Dokument) As Boolean
        If dok.revisionssicher Then
            MessageBox.Show("Das Dokument ist revisionssicher. Es kann nicht gelöscht werden! (" & dok.DateinameMitExtension & ")", "Dokument löschen",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation)
            Return True
        End If
        Return False
    End Function

    Public Shared Function aktDokumentLoschen(adok As Dokument) As Boolean
        Try
            If revisionssicher(adok) Then
                nachricht("Das Dokument ist revisionssicher und wird deshalb nicht gelöscht!")
                Return False
            End If

            'auschecken
            Dim hinweis As String = checkout.checkout(adok, myGlobalz.sitzung.aktVorgangsID)
            nachricht("#: " & hinweis$)

            ' datei im archiv löschen
            Dim btest As Boolean = loescheDokumentImFilesystem(adok)
            nachricht("ARCHIV: loescheDokumentImFilesystem: true ist erfolg: " & btest)

            'dateieintrag in der tabelle dokumente löschen
            ' Dim itest% = DokumentenArchiv.loescheDokument(myGlobalz.sitzung.aktDokument.DocID)
            Dim itest% = DokArcTools.dokINDbLoeschen.execute(adok.DocID)
            nachricht("ARCHIV: aktDokumentLoschen: 1 ist erfolg: " & itest%)

            'dateieintrag in der tabelle dokument2vorgang löschen
            'itest = entKoppelung_Dokument_Vorgang(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.VorgangsID)
            'itest = DokArcTools.EntkopplungVonVorgang.execute(adok.DocID, myGlobalz.sitzung.aktVorgangsID)
            'nachricht("ARCHIV: entKoppelung_Dokument_Vorgang: 1 ist erfolg: " & itest%)


            ' itest = DokArcTools.EntKoppelung_Dokument_Ereignis_alledb.execute(adok.DocID, myGlobalz.sitzung.aktEreignis.ID)
            'dateieintrag in der tabelle dokument2ereignis löschen
            '  itest = clsEreignisDB_Mysql.EntKoppelung_Dokument_Ereignis(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktEreignis.ID)
            '  nachricht("ARCHIV: EntKoppelung_Dokument_Ereignis: 0 oder 1 ist erfolg: " & itest%)

            'id aus parafoto holen - entspricht der sekid in raumbezugstabelle
            'nicht vergessen daß der typ (=5) noch mit angegeben werden muss damit das löchen 
            '       in der raumbezugstabelle eindeutig ist
            '  Dim sekid% = DBraumbezug_Mysql.getID4Foto(myGlobalz.sitzung.aktDokument.DocID)
            Dim sekid% = DokArcTools.getID4Foto.execute(adok.DocID)
            nachricht("ARCHIV: getID4Foto: >0 ist erfolg: " & sekid%)
            If sekid > 0 Then
                itest = RBtoolsns.RBFotoLoeschen_alleDB.execute(adok.DocID)

                nachricht("ARCHIV: RB_FOTO_loeschen:  " & itest%)
                itest = RBtoolsns.Raumbezug_loeschen_bySEKid_alleDB.execute(sekid, "5")
                ' itest = DBraumbezug_Mysql.RB_loeschenBySekIDTyp(sekid, "5") 'gilt nur für Fotos besser wäre eine umwandlung des enums in integer: todo
                nachricht("ARCHIV: RB_loeschenBySekIDTyp:  : " & itest%)
                Dim erfolg As Integer
                erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myGlobalz.sitzung.aktParaFoto.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID)
            End If
            nachricht("Dokument wurde gelöscht. Bitte führen Sie einen Refresh durch.")
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Löschen des Dokumentes: ", ex)
            Return False
        End Try
    End Function

    Public Shared Function listeEnthaeltBereitsDiesesDokument(ByVal dok As Dokument, ByVal dieliste As List(Of Dokument)) As Boolean
        For Each ddog As Dokument In dieliste
            If ddog.FullnameCheckout = dok.FullnameCheckout Then
                Return True
            End If
        Next
        Return False
    End Function
    Public Shared Function listeEnthaeltWordDokument(ByVal dieliste As List(Of Dokument)) As Boolean
        For Each ddog As Dokument In dieliste
            If ddog.DokTyp = DokumentenTyp.DOC Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Shared Function setzeVerwandschaftsstatus(ByVal dok As Dokument, ByVal fremdVorgangsId As Integer) As Boolean
        Dim result As Boolean = False 'ihah
        If dok.VorgangsID = fremdVorgangsId Then
            myGlobalz.sitzung.aktDokument.istNurVerwandt = False
        Else
            myGlobalz.sitzung.aktDokument.istNurVerwandt = True
        End If
        Return True
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
            neuername = dok.Item("dateinameext").ToString.Replace(dok.Item("typ").ToString, "").Replace(".", "_")
            neuername = zieldir & "\" &
                        neuername & "_" &
                        dok.Item("filedatum").ToString.Replace(".", "_").Replace(":", "_") & "." &
                        dok.Item("typ").ToString
            Return neuername
        Catch ex As Exception
            nachricht("Fehler beim neuerdateinameFuerZiel des Dokumentes: ", ex)
            Return ""
        End Try
    End Function
    Private Function neuerdateinameFuerZielOBJ(ziel As String, zieldir As String, dok As clsPresDokumente, ByRef dateinamemitextensdion As String) As String
        Dim neuername As String
        'Dim dateinamemitextensdion As string
        Try
            neuername = dok.DateinameMitExtension.Replace(dok.Typ.ToString, "").Replace(".", "_")
            dateinamemitextensdion = neuername & "_" &
                        dok.Filedatum.ToString.Replace(".", "_").Replace(":", "_") & "." &
                        dok.Typ.ToString
            neuername = zieldir & "\" &
                        neuername & "_" &
                        dok.Filedatum.ToString.Replace(".", "_").Replace(":", "_") & "." &
                        dok.Typ.ToString
            Return neuername
        Catch ex As Exception
            nachricht("Fehler beim neuerdateinameFuerZiel des Dokumentes: ", ex)
            Return ""
        End Try
    End Function

    Private Sub verzeichnisInhaltLoeschen(zieldir As String, BOOLverzeichnisVORHERloeschen As Boolean)
        Try
            Dim kills As String()
            If IO.Directory.Exists(zieldir) Then
                kills = getfilesAusDir(zieldir)
                If kills Is Nothing Then
                    'dir existiert nicht
                    Exit Sub
                End If
                For Each ele In kills
                    Try
                        IO.File.Delete(ele)
                    Catch ex As Exception
                        nachricht("  beim verzeichnisLoeschen1 des Dokumentes: " & ele & Environment.NewLine &
                                        ex.ToString)
                    End Try
                Next
            End If
        Catch ex As Exception
            nachricht("  beim verzeichnisLoeschen2 des Dokumentes: ", ex)
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
            l("Fehler in getfilesAusDir filter:" & syn & ",", ex)
            Return Nothing
        End Try
    End Function

    Private Function Dokliste_auschecken(zielverzeichnis As String, verzeichnisOeffnen As Boolean, dliste As List(Of clsPresDokumente)) As Boolean
        Dim quelle As String = "", ziel As String = ""
        Dim FIquelle, FIziel As IO.FileInfo
        'Dim dokview As DataRowView
        For Each dok As clsPresDokumente In dliste
            If Not dok.ausgewaehlt Then Continue For
            Try
                'Quelldatei_definieren
                quelle = myGlobalz.Arc.rootDir.ToString & dok.dokumentPfad.ToString.Replace("/", "\")
                quelle = quelle & "\" & dok.DateinameMitExtension.ToString
                If dok.newSaveMode Then
                    dok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
                    quelle = dok.FullnameImArchiv
                End If
                nachricht("  quelle: " & quelle)
                'Zieldatei_definieren
                ziel = zielverzeichnis & "\" & dok.DateinameMitExtension
                nachricht("  ziel: " & ziel)
                'Zielverzeichnis_erstellen
                nachricht("Erzeuge Zielverzeichnis: zielVerzeichnis$: " & zielverzeichnis)
                Dim erfolg As IO.DirectoryInfo = IO.Directory.CreateDirectory(zielverzeichnis)
                If erfolg.Exists Then
                    nachricht("zielVerzeichnis wurde erzeugtc!")
                Else
                    nachricht("zielVerzeichnis wurde NICHT erzeugt!")
                End If
                'wenn quelldateifehlt_exit

                Try
                    FIquelle = New IO.FileInfo(quelle)
                Catch ex As Exception
                    nachricht("fehler in DataTable_auscheckene: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
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
                    nachricht("fehler in DataTable_auscheckenf: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                     ex.ToString)
                    Continue For
                End Try

                If FIziel.Exists Then
                    nachricht("ziel existiert!")
                    ziel = neuerdateinameFuerZielOBJ(ziel, zielverzeichnis, dok, "")
                End If

                'kopieren
                nachricht("  vor zieltestc: " & ziel)

                ueberkopierenNachCheckout(ziel, FIquelle, CBool(dok.kompressed))

                My.Log.WriteEntry("a1") : Dim aktdo As New Dokument
                My.Log.WriteEntry("a2") : aktdo.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                My.Log.WriteEntry("a3") : myGlobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                'My.Log.WriteEntry("a4") : DokumentDatarow2Obj(dok, aktdo)                        'addieren
                'My.Log.WriteEntry("a5") : zuCheckoutlisteAddieren(aktdo, myglobalz.sitzung.checkoutDokuList) ihah
                My.Log.WriteEntry("a6")
            Catch ex As Exception
                nachricht_und_Mbox("fehler1 beim Auschecken von:  " & ziel & vbCrLf, ex)
                Return False
            End Try
        Next
        FIquelle = Nothing : FIziel = Nothing
        Oeffneverzeichnis(verzeichnisOeffnen, zielverzeichnis)
        Return True
    End Function

    Public Function einzeldokument_auschecken(ByVal zielVerzeichnis As String, dok As Dokument) As Boolean
        Dim quelle As String = "", ziel As String = ""
        Dim FIquelle, FIziel As IO.FileInfo
        Try
            'Quelldatei_definieren
            quelle = dok.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
            'quelle = myGlobalz.Arc.rootDir.ToString & dok.dokumentPfad.ToString.Replace("/", "\")
            'quelle = quelle & "\" & dok.DateinameMitExtension.ToString


            nachricht("  quelle: " & quelle)
            'Zieldatei_definieren
            ziel = zielVerzeichnis & "\" & dok.DateinameMitExtension.ToString
            nachricht("  ziel: " & ziel)
            'Zielverzeichnis_erstellen
            nachricht("Erzeuge Zielverzeichnis: zielVerzeichnis$: " & zielVerzeichnis)
            Dim erfolg As IO.DirectoryInfo = IO.Directory.CreateDirectory(zielVerzeichnis)
            If erfolg.Exists Then
                nachricht("zielVerzeichnis wurde erzeugta!")
            Else
                nachricht("zielVerzeichnis wurde NICHT erzeugt!")
            End If
            'wenn quelldateifehlt_exit

            Try
                FIquelle = New IO.FileInfo(quelle)
            Catch ex As Exception
                nachricht("fehler in DataTable_auscheckena: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                 ex.ToString)
                Return False
            End Try

            If Not FIquelle.Exists Then
                nachricht("Quelle existiert nicht: " & quelle)
                FIziel = Nothing : FIquelle = Nothing
                Return False
            Else
                nachricht("Quelle existiert!")
            End If
            Try
                FIziel = New IO.FileInfo(ziel)
            Catch ex As Exception
                nachricht("fehler in DataTable_auscheckenb: Datei konnte nicht gefunden werden. " & quelle & Environment.NewLine &
                                 ex.ToString)
                FIziel = Nothing : FIquelle = Nothing
                Return False
            End Try

            If FIziel.Exists Then
                Try
                    FileArchivTools.inputFileReadonlyEntfernen(ziel)
                    FIziel.Delete()
                Catch ex As Exception
                    nachricht("fehler: ziel existiert! " & FIziel.FullName, ex)
                    Dim dateinamemitextension As String = ""
                    'Hier könnte man besser einfach löschen und überschreiben
                    ziel = neuerdateinameFuerZielOBJ(ziel, zielVerzeichnis, detail_dokuauswahl.dokumentNachPresDokumentKonvertieren(dok), dateinamemitextension)
                    dok.DateinameMitExtension = dateinamemitextension
                    dok.FullnameCheckout = ziel
                End Try

            End If

            'kopieren
            nachricht("  vor zieltestd: " & ziel)

            If ueberkopierenNachCheckout(ziel, FIquelle, CBool(dok.kompressed)) = 1 Then
                nachricht("zieldatei ist im zugriff: " & ziel)
                FIziel = Nothing : FIquelle = Nothing
                Return False
            End If
        Catch ex As Exception
            nachricht_und_Mbox("fehler beim Auschecken von:  " & ziel & vbCrLf, ex)
            Return False
        Finally
            FIziel = Nothing : FIquelle = Nothing
        End Try
        Return True
    End Function
    Public Sub AlleFotosAuscheckenVorgang(ByVal verzeichnisOeffnen As Boolean, BOOLverzeichnisVORHERloeschen As Boolean)
        Dim zieldir As String = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID
        verzeichnisInhaltLoeschen(zieldir, BOOLverzeichnisVORHERloeschen)
        'Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides")
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(myGlobalz.sitzung.aktVorgangsID), "nurfotos", alleBilder:=True, 0)
        If bresult Then
            myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
            If myGlobalz.Arc.DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, verzeichnisOeffnen) Then
                IO.Directory.CreateDirectory(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
                If verzeichnisOeffnen Then System.Diagnostics.Process.Start(String.Format("{0}\{1}",
                        myGlobalz.Arc.lokalerCheckoutcache,
                        myGlobalz.sitzung.aktVorgangsID))
            End If
        End If
    End Sub

    Public Sub AllesDokusAuscheckenVorgang(ByVal verzeichnisOeffnen As Boolean, BOOLverzeichnisVORHERloeschen As Boolean)
        Dim zieldir As String = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID
        verzeichnisInhaltLoeschen(zieldir, BOOLverzeichnisVORHERloeschen)
        'Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "beides")
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(myGlobalz.sitzung.aktVorgangsID), "keinefotos", alleBilder:=True, 0)
        If bresult Then
            myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
            If myGlobalz.Arc.DataTable_auschecken(myGlobalz.Arc.lokalerCheckoutcache & myGlobalz.sitzung.aktVorgangsID, verzeichnisOeffnen) Then
                IO.Directory.CreateDirectory(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
                If verzeichnisOeffnen Then System.Diagnostics.Process.Start(String.Format("{0}\{1}",
                        myGlobalz.Arc.lokalerCheckoutcache,
                        myGlobalz.sitzung.aktVorgangsID))
            End If
        End If
    End Sub

    Private Shared Function GetTempOfficeName(ByVal test As IO.FileInfo, dtyp As Integer) As String
        Dim tempOfficeName As String
        If dtyp = DokumentenTyp.DOC Then
            tempOfficeName = test.Name
            tempOfficeName = tempOfficeName.Substring(2, tempOfficeName.Length - 2)
            tempOfficeName = test.DirectoryName & "\~$" & tempOfficeName
        End If
        If dtyp = DokumentenTyp.XLS Then
            tempOfficeName = test.Name
            'tempOfficeName = tempOfficeName.Substring(2, tempOfficeName.Length - 2)
            tempOfficeName = test.DirectoryName & "\~$" & test.Name
        End If
#Disable Warning BC42104 ' Variable 'tempOfficeName' is used before it has been assigned a value. A null reference exception could result at runtime.
        Return tempOfficeName
#Enable Warning BC42104 ' Variable 'tempOfficeName' is used before it has been assigned a value. A null reference exception could result at runtime.
    End Function
    Private Shared Function tempOfficeDateiNochVorhanden(dokument As Dokument) As Boolean
        Dim test As IO.FileInfo
        test = New IO.FileInfo(dokument.FullnameCheckout)
        Dim tempOfficeName As String
        tempOfficeName = GetTempOfficeName(test, dokument.getDokTyp)
        test = New IO.FileInfo(tempOfficeName)
        If test.Exists Then
            test = Nothing
            Return True
        Else
            test = Nothing
            Return False
        End If
    End Function
    Public Shared Function DokumentStarten(ByVal aktdoku As Dokument) As Boolean
        System.Threading.Thread.Sleep(1000)
        Try
            System.Diagnostics.Process.Start(aktdoku.FullnameCheckout)
            Return True
        Catch ex1 As Exception
            System.Threading.Thread.Sleep(2000)
            Try
                System.Diagnostics.Process.Start(aktdoku.FullnameCheckout)
            Catch ex As Exception
                MessageBox.Show("Der angegebenen Datei ist keine Anwendung zugeordnet: " & Environment.NewLine &
                                aktdoku.FullnameCheckout,
                                "Der angegebenen Datei ist keine Anwendung zugeordnet", MessageBoxButton.OK, MessageBoxImage.Error)
                nachricht("Fehler in DateiStarten:1 " & aktdoku.FullnameCheckout & Environment.NewLine &
                                aktdoku.FullnameImArchiv & Environment.NewLine &
                                ex.ToString)
            End Try
            Return False
        End Try
    End Function

    Public Shared Function WordReadonlyDokumentStarten(ByVal [aktdokuString] As String, readOnlyDoxsInTxtCrtlOeffnen As Boolean) As Boolean
        System.Threading.Thread.Sleep(1000)
        Dim myproc As New System.Diagnostics.Process
        Try
            Dim zieldatei As String
            zieldatei = [aktdokuString].ToLower.Replace(".docx", ".pdf").Replace(".doc", ".pdf")
            'zieldatei = zieldatei.ToLower.Replace(".docx", ".pdf").Replace(".docx", ".pdf")
            Dim lw = New WordReplaceTextmarken([aktdokuString], "", Nothing)
            FileArchivTools.inputFileReadonlyEntfernen([aktdokuString])
            'Dim readOnlyDoxsInTxtCrtlOeffnen As Boolean
#If DEBUG Then
            REM readOnlyDoxsInTxtCrtlOeffnen = True
#End If
            If readOnlyDoxsInTxtCrtlOeffnen Then
                readOnlyDoxsInTextCrtlOeffnen(aktdokuString)
            Else
                If wordInterop.dok2pdfA([aktdokuString], zieldatei) Then
                    lw = Nothing
                    Process.Start(zieldatei)
                End If
            End If

            lw = Nothing
            Return True
        Catch ex1 As Exception
            MsgBox("Der wordviewere ist noch nicht installiert. bitte beim admin melden !!! ")
            System.Threading.Thread.Sleep(2000)
            Try
                System.Diagnostics.Process.Start([aktdokuString])
            Catch ex As Exception
                nachricht("Fehler in WordReadonlyDokumentStarten: " & [aktdokuString] & Environment.NewLine &
                                [aktdokuString] & Environment.NewLine &
                                ex.ToString)
            End Try
            Return False
        End Try
    End Function

    Public Shared Sub readOnlyDoxsInTextCrtlOeffnen(aktdokuString As String)
        Dim txtcrtlExe As String = "C:\kreisoffenbach\txtctrlNEU\ParadigmaTextControl.exe "
        Dim Process As Process = New Process()
        Process.StartInfo.FileName = txtcrtlExe
        Process.StartInfo.Arguments = aktdokuString
        Process.StartInfo.ErrorDialog = True
        Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        l("vor start")
        Process.Start()
    End Sub

    Public Shared Function copyOrCopyKompress(Fquell As IO.FileInfo, Fziel As IO.FileInfo, kompress As Boolean) As Boolean
        Try
            l(" MOD copyOrCopyKompress anfang")
            If kompress And Environment.UserName.ToLower = "feinen_j" Then
                'CLstart.AesCrypt.FileEncrypt(Fquell.FullName, Fziel.FullName, CLstart.AesCrypt.normpw)
                IO.File.Copy(Fquell.FullName, Fziel.FullName)
            Else
                IO.File.Copy(Fquell.FullName, Fziel.FullName)
            End If
            l(" MOD copyOrCopyKompress ende")
            Return True
        Catch ex As Exception
            l("Fehler in copyOrCopyKompress: ", ex)
            Return False
        End Try
    End Function
    Public Shared Sub copyOrDekompresscopy(fiQuelle As IO.FileInfo, zielDoku_FullnameCheckout As String, kompressed As Boolean)
        If kompressed Then '   Environment.UserName.ToLower = "feinen_j"
            'fiQuelle.CopyTo(zielDoku_FullnameCheckout, True)
            CLstart.AesCrypt.FileDecrypt(fiQuelle.FullName, zielDoku_FullnameCheckout, CLstart.AesCrypt.normpw)
        Else
            fiQuelle.CopyTo(zielDoku_FullnameCheckout, True)
        End If
    End Sub


End Class