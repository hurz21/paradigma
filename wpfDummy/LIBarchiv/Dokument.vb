Imports System.ComponentModel

Public Class Dokument
    Implements INotifyPropertyChanged, ICloneable
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged
    Public Property DokTyp As New DokumentenTyp
    Protected Sub OnPropertyChanged(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
    Public anychange As Boolean
    Property sizeMb As Double = 0
    Property bearbeiterid As Integer = 0
#If DEBUG Then
    Property newSaveMode As Boolean = True
#Else
    Property newSaveMode As Boolean = False
#End If
    Property tempEditDatei As String
    Property nurzumlesen As Boolean = False
    ''' <summary>
    ''' dient der aufzeichnung von dateiöffnungen
    ''' </summary>
    ''' <remarks></remarks>
    Private _handlenr As Integer
    Public Property Handlenr() As Integer
        Get
            Return _handlenr
        End Get
        Set(ByVal value As Integer)
            _handlenr = value
        End Set
    End Property

    Public Property istNurVerwandt As Boolean = False


    Public Function hatKoordinaten() As Boolean
        If String.IsNullOrEmpty(EXIFlon) Or String.IsNullOrEmpty(EXIFlat) Then
            Return False
        End If
        If EXIFlon = "0#0#0#" Or EXIFlat = "0#0#0#" Then
            Return False
        End If
        Return True
    End Function



    Private _fullnameCheckout As String
    Public Property FullnameCheckout() As String
        Get
            Return _fullnameCheckout
        End Get
        Set(ByVal Value As String)
            _fullnameCheckout = Value
            OnPropertyChanged("Gesamt_lokal")
        End Set
    End Property
    Private _revisionssicher As Boolean
    Public Property revisionssicher As Boolean
        Get
            Return _revisionssicher
        End Get
        Set(ByVal Value As Boolean)
            _revisionssicher = Value
            OnPropertyChanged("revisionssicher")
        End Set
    End Property

    '''	<summary>
    '''	pfad name	im archiv	 ext wie c:\ini.ini
    '''	</summary>
    '''	<remarks></remarks>
    Private _fullnameImArchiv As String
    Public Property FullnameImArchiv() As String
        Get
            Return _fullnameImArchiv
        End Get
        Set(ByVal Value As String)
            _fullnameImArchiv = Value
            OnPropertyChanged("FullnameImArchiv")
        End Set
    End Property
    Private _dokumentPfad As String
    Public Property dokumentPfad() As String            'relativpfad?
        Get
            Return _dokumentPfad
        End Get
        Set(ByVal Value As String)
            _dokumentPfad = Value
            OnPropertyChanged("relativpfad")
        End Set
    End Property

    ''' <summary>
    ''' initial des users der das dokument eincheckt
    ''' </summary>
    ''' <remarks></remarks>
    Private _Initiale As String
    Public Property Initiale() As String
        Get
            Return _Initiale
        End Get
        Set(ByVal Value As String)
            _Initiale = Value
            OnPropertyChanged("Initiale")
        End Set
    End Property

    Private _dateinameMitExtension As String
    Public Property DateinameMitExtension() As String
        Get
            Return _dateinameMitExtension
        End Get
        Set(ByVal Value As String)
            _dateinameMitExtension = Value
            setDokTyp()
            OnPropertyChanged("DateinameMitExtension")
        End Set
    End Property
    Private _typ As String = ""
    Public Property Typ() As String
        Get
            Return _typ
        End Get
        Set(ByVal Value As String)
            _typ = Value
            OnPropertyChanged("Typ")
        End Set
    End Property
    Private _VorgangsID As Integer
    Public Property VorgangsID() As Integer
        Get
            Return _VorgangsID
        End Get
        Set(ByVal Value As Integer)
            _VorgangsID = Value
            OnPropertyChanged("VorgangsID")
        End Set
    End Property
    Private _ereignisID As Integer
    Public Property EreignisID() As Integer
        Get
            Return _ereignisID
        End Get
        Set(ByVal Value As Integer)
            _ereignisID = Value
            OnPropertyChanged("EreignisID")
        End Set
    End Property
    Private _docID As Integer
    Public Property DocID() As Integer
        Get
            Return _docID
        End Get
        Set(ByVal Value As Integer)
            _docID = Value
            OnPropertyChanged("DocID")
        End Set
    End Property
    Private _beschreibung As String
    Public Property Beschreibung() As String
        Get
            Return _beschreibung
        End Get
        Set(ByVal Value As String)
            _beschreibung = Value
            OnPropertyChanged("Beschreibung")
        End Set
    End Property

    Private _fDatum As Date
    Public Property ExifDatum() As Date
        Get
            Return _fDatum
        End Get
        Set(ByVal Value As Date)
            _fDatum = Value
            OnPropertyChanged("ExifDatum")
        End Set
    End Property
    Private _eXIFlon As String
    Public Property EXIFlon() As String
        Get
            Return _eXIFlon
        End Get
        Set(ByVal Value As String)
            _eXIFlon = Value
            OnPropertyChanged("EXIFlon")
        End Set
    End Property
    Private _eXIFlat As String
    Public Property EXIFlat() As String
        Get
            Return _eXIFlat
        End Get
        Set(ByVal Value As String)
            _eXIFlat = Value
            OnPropertyChanged("EXIFlat")
        End Set
    End Property
    Private _eXIFdir As String
    Public Property EXIFdir() As String
        Get
            Return _eXIFdir
        End Get
        Set(ByVal Value As String)
            _eXIFdir = Value
            OnPropertyChanged("EXIFdir")
        End Set
    End Property
    Private _eXIFhersteller As String
    Public Property EXIFhersteller() As String
        Get
            Return _eXIFhersteller
        End Get
        Set(ByVal Value As String)
            _eXIFhersteller = Value
            OnPropertyChanged("EXIFhersteller")
        End Set
    End Property


    Private _filedatum As Date
    Public Property Filedatum() As Date
        Get
            Return _filedatum
        End Get
        Set(ByVal Value As Date)
            _filedatum = Value
            OnPropertyChanged("Filedatum")
        End Set
    End Property

    Private _checkindatum As Date
    Public Property Checkindatum() As Date
        Get
            Return _checkindatum
        End Get
        Set(ByVal Value As Date)
            _checkindatum = Value
            OnPropertyChanged("Checkindatum")
        End Set
    End Property
    Private _istVeraltet As Boolean



    Public Property istVeraltet() As Boolean
        Get
            Return _istVeraltet
        End Get
        Set(ByVal Value As Boolean)
            _istVeraltet = Value
            OnPropertyChanged("istVeraltet")
        End Set
    End Property
    Public Property OriginalFullname() As String
    Public Property OriginalName() As String
    Public Property kompressed As Boolean = False

    Overloads Sub clear(ByVal MeinNULLDatumAlsDate As Date)         'CLstart.mycsimple.MeinNULLDatumAlsDate
        Beschreibung = ""
        Initiale = ""
        DocID = 0
        EreignisID = 0
        VorgangsID = 0
        newSaveMode = True
        Typ = ""
        DateinameMitExtension = ""
        dokumentPfad = ""
        anychange = False
        istVeraltet = False
        Checkindatum = MeinNULLDatumAlsDate 'CLstart.mycsimple.MeinNULLDatumAlsDate
        Filedatum = MeinNULLDatumAlsDate 'MeinNULLDatumAlsDate'CLstart.mycsimple.MeinNULLDatumAlsDate
        istNurVerwandt = False
        kompressed = False
    End Sub
    '''	<summary>
    '''	erzeugt	den	gesamtpfad mit allem drum	und	dran für dateien im	Archiv an
    '''	</summary>
    '''	<returns></returns>
    '''	<remarks></remarks>
    Function makeFullname_ImArchiv(ByVal rootDir As System.IO.DirectoryInfo) As String     'myGlobalz.Arc.rootDir
        Dim CleanDokpfad, CleandDateinameMitExtension As String
        'l("makeFullname_ImArchiv----------------------")
        If newSaveMode Then
            CleanDokpfad = dokumentPfad '.Trim(CChar("\")) 
            CleanDokpfad = CleanDokpfad '.Trim(CChar("/"))
            CleandDateinameMitExtension = CStr(DocID)
            FullnameImArchiv = rootDir.ToString & IO.Path.Combine(CleanDokpfad, CleandDateinameMitExtension)
            FullnameImArchiv = FullnameImArchiv.Replace("/", "\")
            Return FullnameImArchiv
        Else
            CleanDokpfad = dokumentPfad '.Trim(CChar("\")) 
            CleanDokpfad = CleanDokpfad '.Trim(CChar("/"))
            CleandDateinameMitExtension = DateinameMitExtension '.Trim(CChar("\"))
            CleandDateinameMitExtension = CleandDateinameMitExtension '.Trim(CChar("/"))
            FullnameImArchiv = rootDir.ToString & IO.Path.Combine(CleanDokpfad, CleandDateinameMitExtension)
            FullnameImArchiv = FullnameImArchiv.Replace("/", "\")
            Return FullnameImArchiv
        End If

    End Function

    Function getTimestamp() As String
        Return Now.ToString("yyyy-MM-dd_HHmmss")
    End Function
    '''	<summary>
    '''	erzeugt	den	gesamtpfad mit allem drum	und	dran	für	die	lokalen	dateien	an
    '''	</summary>
    '''	<returns></returns>
    '''	<remarks></remarks>
    Function makeFullname_Checkout(ByVal VorgangsID As Integer, ByRef lokalescacheverzeichnis As String, dokid As Integer, ByRef ausgabeVerzeichnis As String) As String     'myGlobalz.Arc.lokalerCheckoutcache

        FullnameCheckout = makeFullname_Checkoutdateiname(VorgangsID, lokalescacheverzeichnis, dokid, ausgabeVerzeichnis)
        Try
            Dim fi As New IO.FileInfo(FullnameCheckout)
            IO.Directory.CreateDirectory(fi.Directory.ToString)
            fi = Nothing
        Catch ex As Exception

        End Try

        Return FullnameCheckout
    End Function

    Function makeFullname_Checkoutdateiname(ByVal VorgangsID As Integer, ByVal lokalescacheverzeichnis As String, dokid As Integer, ByRef ausgabeVerzeichnis As String) As String     'myGlobalz.Arc.lokalerCheckoutcache
        'alt   FullnameCheckout = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, DateinameMitExtension)

        'Dim checkoutPfad As String
        'If CStr(dokid) = String.Empty Then
        '    checkoutPfad = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, dokid)
        'Else
        'checkoutPfad = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, dokid)
        'FullnameCheckout = String.Format("{0}{1}\{2}_{3}\{4}", lokalescacheverzeichnis, VorgangsID, dokid, getTimestamp(), DateinameMitExtension)
        If CStr(dokid) = String.Empty OrElse dokid = 0 Then
            FullnameCheckout = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, DateinameMitExtension)
            ausgabeVerzeichnis = String.Format("{0}{1}", lokalescacheverzeichnis, VorgangsID)
        Else
            FullnameCheckout = String.Format("{0}{1}\{2}\{3}", lokalescacheverzeichnis, VorgangsID, dokid, DateinameMitExtension)
            ausgabeVerzeichnis = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, dokid)
            'FullnameCheckout = String.Format("{0}{1}\{2}_{3}\{4}", lokalescacheverzeichnis, VorgangsID, dokid, getTimestamp(), DateinameMitExtension)
        End If


        Return FullnameCheckout
    End Function
    Function makeFullname_CheckoutPath(ByVal VorgangsID As Integer, ByVal lokalescacheverzeichnis As String, dokid As Integer) As String     'myGlobalz.Arc.lokalerCheckoutcache
        'alt   FullnameCheckout = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, DateinameMitExtension)
        Dim checkoutPfad, ausgabeVerzeichnis As String
        FullnameCheckout = makeFullname_Checkoutdateiname(VorgangsID, lokalescacheverzeichnis, dokid, ausgabeVerzeichnis)
        Dim fi As New IO.FileInfo(FullnameCheckout)
        checkoutPfad = fi.DirectoryName
        'If CStr(dokid) = String.Empty Then
        '    checkoutPfad = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, dokid)
        'Else
        '    checkoutPfad = String.Format("{0}{1}\{2}", lokalescacheverzeichnis, VorgangsID, dokid)
        '    'FullnameCheckout = String.Format("{0}{1}\{2}_{3}\{4}", lokalescacheverzeichnis, VorgangsID, dokid, getTimestamp(), DateinameMitExtension)
        'End If
        Return checkoutPfad
    End Function
    Public Shared Sub createCheckoutDir(ByVal pfad As String, docid As Integer, ByVal vorgangsid As Integer)
        'alte version
        Try
            IO.Directory.CreateDirectory(String.Format("{0}\{1}\{2}", pfad, vorgangsid, docid))
        Catch ex As Exception
            MsgBox(String.Format("createCheckoutDir: Verzeichnis konnte nicht erzeigt werden: {0}\{1}", pfad, vorgangsid))
        End Try

    End Sub

    Shared Function VorlageMakeFullname_Checkout(ByVal VorgangsID As Integer,
                                                 ByVal nuranschauen As Boolean,
                                                 ByVal dateinamemitext As String,
                                                 ByVal checkoutdir As String) As String
        Try
            Dim nuransch, result As String
            '\vorlagen_bearbeiten\0\
            If String.IsNullOrEmpty(checkoutdir) Then Return String.Empty
            If String.IsNullOrEmpty(dateinamemitext) Then Return String.Empty
            If VorgangsID < 1 Then Return String.Empty
            If nuranschauen Then
                nuransch = "\vorlagen_nuranschauen\"
            Else
                nuransch = "\vorlagen_bearbeiten\"
            End If
            result = String.Format("{0}{1}{2}\{3}", checkoutdir, nuransch, VorgangsID, dateinamemitext)
            If result.Contains(":") Then
                result = result.Replace("/", "\")
                result = result.Replace("\\", "\")
            End If
            Return result
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Private Shared Sub VorlagecreateCheckoutDirExtracted(ByVal cache As String)
        If Not IO.Directory.Exists(cache) Then IO.Directory.CreateDirectory(cache)
        If Not IO.Directory.Exists(String.Format("{0}\vorlagen_nuranschauen", cache)) Then IO.Directory.CreateDirectory(String.Format("{0}\vorlagen_nuranschauen", cache))
        If Not IO.Directory.Exists(String.Format("{0}\vorlagen_bearbeiten", cache)) Then IO.Directory.CreateDirectory(String.Format("{0}\vorlagen_bearbeiten", cache))
    End Sub

    Public Shared Function VorlagecreateCheckoutDir(ByVal FullnameCheckout As String, ByVal cache As String) As Boolean
        VorlagecreateCheckoutDirExtracted(cache)
        Dim test As New IO.FileInfo(FullnameCheckout)
        Dim pfad As String = test.DirectoryName
        test = Nothing
        Try
            IO.Directory.CreateDirectory(pfad)
            Return True
        Catch ex As Exception
            MsgBox("Fehler: VorlagecreateCheckoutDir " & pfad)
            Return False
        End Try
    End Function

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return MemberwiseClone()
    End Function

    Public Function isTypeEditable() As Boolean
        If DokTyp = DokumentenTyp.JPG Then Return False
        If DokTyp = DokumentenTyp.PDF Then Return False
        If DokTyp = DokumentenTyp.GIF Then Return False
        If DokTyp = DokumentenTyp.MSG Then Return False
        If DokTyp = DokumentenTyp.EML Then Return False
        If DokTyp = DokumentenTyp.DOC Then Return True
        If DokTyp = DokumentenTyp.XLS Then Return True
        If DokTyp = DokumentenTyp.GA Then Return False
        If DokTyp = DokumentenTyp.VCF Then Return False
        If DokTyp = DokumentenTyp.PNG Then Return False
        If DokTyp = DokumentenTyp.TIF Then Return False
        Return True
    End Function

    Private Sub setDokTyp()
        If DateinameMitExtension Is Nothing Then
            DokTyp = DokumentenTyp.UNKNOWN
            Exit Sub
        End If
        If DateinameMitExtension.ToLower.EndsWith("jpg") Then DokTyp = DokumentenTyp.JPG : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("pdf") Then DokTyp = DokumentenTyp.PDF : Exit Sub

        If DateinameMitExtension.ToLower.EndsWith("msg") Then DokTyp = DokumentenTyp.MSG : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("eml") Then DokTyp = DokumentenTyp.EML : Exit Sub

        If DateinameMitExtension.ToLower.EndsWith("doc") Then DokTyp = DokumentenTyp.DOC : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("docx") Then DokTyp = DokumentenTyp.DOC : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("xls") Then DokTyp = DokumentenTyp.XLS : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("xlsx") Then DokTyp = DokumentenTyp.XLS : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("gif") Then DokTyp = DokumentenTyp.GIF : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("vcf") Then DokTyp = DokumentenTyp.VCF : Exit Sub

        If DateinameMitExtension.ToLower.EndsWith("3ga") Then DokTyp = DokumentenTyp.GA : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("rtf") Then DokTyp = DokumentenTyp.RTF : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("avi") Then DokTyp = DokumentenTyp.AVI : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("mpg") Then DokTyp = DokumentenTyp.MPG : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("csv") Then DokTyp = DokumentenTyp.CSV : Exit Sub

        If DateinameMitExtension.ToLower.EndsWith("jpeg") Then DokTyp = DokumentenTyp.JPG : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("png") Then DokTyp = DokumentenTyp.PNG : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("tiff") Then DokTyp = DokumentenTyp.TIF : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("tif") Then DokTyp = DokumentenTyp.TIF : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("ppt") Then DokTyp = DokumentenTyp.PPT : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("pptx") Then DokTyp = DokumentenTyp.PPT : Exit Sub
        If DateinameMitExtension.ToLower.EndsWith("txt") Then DokTyp = DokumentenTyp.TXT : Exit Sub

        DokTyp = DokumentenTyp.UNKNOWN
    End Sub
    Public Function getDokTyp() As Integer
        setDokTyp()
        Return DokTyp
    End Function

    'Friend Function ispdf() As Boolean
    '    Try
    '        l(" MOD ---------------------- anfang")
    '        If DateinameMitExtension.ToLower.Trim.EndsWith(".pdf") Then
    '            Return True
    '        Else
    '            Return False
    '        End If
    '        l(" MOD ---------------------- ende")
    '        Return True
    '    Catch ex As Exception
    '        l("Fehler in MOD: " ,ex)
    '        Return False
    '    End Try
    'End Function
End Class

'Enum dokumentTyp
'    JPG
'    PDF
'    DOC
'    DOCX
'    XLS
'    XLSX
'    GIF
'    RTF
'    MSG
'    VCF
'    CSV
'    UNKNOWN
'End Enum
