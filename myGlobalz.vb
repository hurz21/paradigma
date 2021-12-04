Partial Public Class myGlobalz
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
    Public Shared winfoto As Window_FotoEinzel




End Class