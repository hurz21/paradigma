#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.ComponentModel
Imports System.Data

Public Class Psession
    Implements INotifyPropertyChanged
    Public BestandsAuswahlVID As Integer
    Public anychange As Boolean

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
    Implements INotifyPropertyChanged.PropertyChanged




    Protected Sub OnPropertyChanged(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    'Property KostenStatus As New clsKosten

    Private _textmarkeLiegenschaft As String
    Public Property textmarkeLiegenschaft() As String
        Get
            Return _textmarkeLiegenschaft
        End Get
        Set(ByVal value As String)
            _textmarkeLiegenschaft = value
            OnPropertyChanged("textmarkeLiegenschaft")
        End Set
    End Property

    Private _verwandteDTServer As DataTable
    Public Property VerwandteDTServer() As DataTable
        Get
            Return _verwandteDTServer
        End Get
        Set(ByVal Value As DataTable)
            _verwandteDTServer = Value
            OnPropertyChanged("VerwandteDTServer")
        End Set
    End Property

    Private _verwandteDT As DataTable
    Public Property VerwandteDT() As DataTable
        Get
            Return _verwandteDT
        End Get
        Set(ByVal Value As DataTable)
            _verwandteDT = Value
            OnPropertyChanged("VerwandteDT")
        End Set
    End Property
    Private _ereignisIDsDT As DataTable
    Public Property EreignisIDsDT() As DataTable
        Get
            Return _ereignisIDsDT
        End Get
        Set(ByVal Value As DataTable)
            _ereignisIDsDT = Value
            OnPropertyChanged("EreignisIDsDT")
        End Set
    End Property

    Private _dokumentIDsDT As DataTable
    Public Property DokumentIDsDT() As DataTable
        Get
            Return _dokumentIDsDT
        End Get
        Set(ByVal Value As DataTable)
            _dokumentIDsDT = Value
            OnPropertyChanged("DokumentIDsDT")
        End Set
    End Property

    Private _raumbezugsIDsDT As DataTable
    Public Property RaumbezugsIDsDT() As DataTable
        Get
            Return _raumbezugsIDsDT
        End Get
        Set(ByVal Value As DataTable)
            _raumbezugsIDsDT = Value
            OnPropertyChanged("RaumbezugsIDsDT")
        End Set
    End Property
    Public Property kontaktdatenDT() As DataTable

    Private _adressDT As DataTable
    Public Property AdressDT() As DataTable
        Get
            Return _adressDT
        End Get
        Set(ByVal Value As DataTable)
            _adressDT = Value
            OnPropertyChanged("AdressDT")
        End Set
    End Property

    Private _aktEreignis As New clsEreignis
    Public Property aktEreignis() As clsEreignis
        Get
            Return _aktEreignis
        End Get
        Set(ByVal Value As clsEreignis)
            _aktEreignis = Value
        End Set
    End Property



    Private _sendMail As New SendEmailTexte
    Public Property SendMail() As SendEmailTexte
        Get
            Return _sendMail
        End Get
        Set(ByVal Value As SendEmailTexte)
            _sendMail = Value
            OnPropertyChanged("SendMail")
        End Set
    End Property
    Private _nachrichtenText As String
    Public Property nachrichtenText() As String
        Get
            Return _nachrichtenText
        End Get
        Set(ByVal Value As String)
            _nachrichtenText = Value
            OnPropertyChanged("nachrichtenText")
        End Set
    End Property
    Private _beteiligteModus As String
    Public Property BeteiligteModus() As String
        Get
            Return _beteiligteModus
        End Get
        Set(ByVal Value As String)
            _beteiligteModus = Value
            OnPropertyChanged("BeteiligteModus")
        End Set
    End Property
    Public Property akt_raumbezugsTyp() As RaumbezugsTyp
    Public Property raumbezugsmodus() As String
    Public Property Wiedervorlagemodus() As String
    Public Property Ereignismodus() As String

    Private _bearbeiter As New clsBearbeiter
    Public Property aktBearbeiter() As clsBearbeiter
        Get
            Return _bearbeiter
        End Get
        Set(ByVal Value As clsBearbeiter)
            _bearbeiter = Value
        End Set
    End Property
    Public Property kontaktdatenREC() As IDB_grundfunktionen
    Public Property raumbezugsRec() As IDB_grundfunktionen
    Public Property AlbRec() As IDB_grundfunktionen
    Public Property EreignisseRec() As IDB_grundfunktionen
    Public Property DBWiedervorlageREC() As IDB_grundfunktionen
    Public Property probaugREC() As IDB_grundfunktionen
    Public Property vorgangsbeteiligteAuswahlREC() As IDB_grundfunktionen
    Public Property tempREC() As IDB_grundfunktionen
    Public Property tempREC2() As IDB_grundfunktionen    
    Public Property VorlagenREC() As IDB_grundfunktionen
    Public Property beteiligteREC() As IDB_grundfunktionen
    Public Property beteiligteRECVerwandt() As IDB_grundfunktionen
    Public Property zahlungsREC() As IDB_grundfunktionen 
    Public Property webgisREC() As IDB_grundfunktionen
    Public Property VorgangREC() As IDB_grundfunktionen
    Public Property BearbeiterREC() As IDB_grundfunktionen
    Public Property gesetzesdbREC() As IDB_grundfunktionen
    Public Property postgresREC() As IDB_grundfunktionen
    Public Property modus() As String
    Public Property aktVorgang() As Vorgang
    Public Property aktVorgangsID() As Integer
    Private _aktADR As ParaAdresse
    Public Property aktADR() As ParaAdresse
        Get
            Return _aktADR
        End Get
        Set(ByVal Value As ParaAdresse)
            _aktADR = Value
            OnPropertyChanged("aktADR")
        End Set
    End Property
    Public Property aktParaFoto() As clsParaFoto
    Public Property aktPolygon() As clsParapolygon
    Public Property aktPolyline As clsParapolyline
    Public Property aktPMU() As clsParaUmkreis
    Public Property aktFST() As ParaFlurstueck
    Public Property aktPerson() As Person
    Public Property aktZahlung() As clsZahlung
    Public Property aktDokument() As Dokument



    'Public dbcrudwv As clsWiedervorlageDB_CRUD_MYSQL 'war vororacle NEW  ihah
    Private _wiedervorlage As New clsWiedervorlage 'ihah
    Public Property aktWiedervorlage() As clsWiedervorlage
        Get
            Return _wiedervorlage
        End Get
        Set(ByVal Value As clsWiedervorlage)
            _wiedervorlage = Value
            OnPropertyChanged("Wiedervorlage")
        End Set
    End Property

    '  Public checkoutDokuList As New List(Of LIBArchiv.Dokument)
    ''' <summary>
    ''' Die collection der in benutzung befindlichen dokumente
    ''' </summary>
    ''' <remarks></remarks>
    'Private _checkoutDokuList As New List(Of LIBArchiv.Dokument)
    'Public Property checkoutDokuList() As List(Of LIBArchiv.Dokument)
    '    Get
    '        Return _checkoutDokuList
    '    End Get
    '    Set(ByVal value As List(Of LIBArchiv.Dokument))
    '        _checkoutDokuList = value
    '    End Set
    'End Property
    Public Property wordDateiImEditModus As New Dokument
    Public Property pptxDateiImEditModus As New Dokument
    Public Property excelDateiImEditModus As New Dokument

    Public Function defineArchivVorgangsDir(ByVal vid As Integer) As String
        Return "\" & aktVorgang.Stammdaten.Aufnahmedatum.Year & "\" & vid
    End Function

      Property w_detail As Window_Detail
    Shared Property presDokus As New List(Of clsPresDokumente)
    Shared Property presFotos As New List(Of clsPresDokumente)
    Shared Property presBeteiligte As New List(Of Person)
End Class

