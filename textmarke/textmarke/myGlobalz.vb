Imports System.ComponentModel

Partial Public Class myGlobalz
    Public Shared nureinbildschirm As Boolean = False
    Public Shared colBearbeiterFDU As New List(Of clsBearbeiter)
    Public Shared colBearbeiterBA As New List(Of clsBearbeiter)
    Public Shared zuhause As Boolean = False
    'Public Shared   historyMy as clstart.HistoryKookie. HistoryItem
    Public Shared availablePresentationObjects As New List(Of clsLayerListItem) 'nur in detail
    Public Shared layerListControlObjekts As New List(Of clsLayerListItem) 'nur in detail

    Private Shared _sitzung As New Psession
    Public Shared Property sitzung() As Psession
        Get
            Return _sitzung
        End Get
        Set(ByVal Value As Psession)
            _sitzung = Value

        End Set
    End Property
    Public Shared Property Arc() As DokArc
    Public Shared Property minErrorMessages As Boolean = False
    'Public Shared Property PumuckelInteropVersionNutzen As Integer = 0
    Public Shared Property PumuckelVersion As Integer = 0
    'Public Shared winfoto As Window_FotoEinzel
End Class
