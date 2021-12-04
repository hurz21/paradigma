Namespace LIBgemeinsames

    Public Class clsBiom

    Public Property iminternet() As Boolean
        Get
            Return _iminternet
        End Get
        Set(ByVal value As Boolean)
            _iminternet = value
            If value Then
                netz = "internet"
            Else
                netz = "intranet"
            End If
        End Set
    End Property
    Private _iminternet As Boolean
    Public extranet As Boolean
    Public netz As String
    Public daten_laufwerk$
    Public debuck As Boolean
    Public rettleit As Boolean
    Public controlDB$
    Public meininifile$
    Public sessionid$
    Public modus$
    Public apppfad$
    Public direktaufruf As Boolean
    Public mozilla As Boolean
    Public ajax As Boolean
    Public ajax_vControl$  'für ajaxdropdown
    Public ajax_response$  'für ajaxdropdown
    Public ajax_vAuswahl$  'für ajaxdropdown
    Public ajax_myfunction$  'für ajaxdropdown
    Public ajax_Tabs$    'Einrückung im Tehmenexplorer
    Public LOOKUP As Boolean ' entscheidet ob die dbkonfiguration aus der webgiscontrol eholt wird 
    ' oder direkt übergeben wird
    Public LOOKUP_layer$
    Public LOOKUP_ID$

    Public Sub New()
        Try
            sessionid$ = ""
            LOOKUP = False
            ajax_vControl$ = ""
            ajax_myfunction$ = ""
            ajax_Tabs$ = ""
            direktaufruf = False
            mozilla = False
            extranet = False
            iminternet = True
            debuck = False
            daten_laufwerk = "d:"
            netz = "internet"
            meininifile$ = "c:\mapshare\rheinmain.xml"
            apppfad = "d:/inetpub/wwwroot/profile/register/"
        Catch ex As Exception
            l("Fehler in NEW")
        End Try
    End Sub
    Public Overrides Function tostring() As String
        Dim sb As New System.Text.StringBuilder
        sb.Append(String.Format("in main iminternet:{0}", iminternet))
        sb.Append(String.Format("in main extranet:{0}", extranet))
        Return sb.ToString
    End Function
End Class

End Namespace