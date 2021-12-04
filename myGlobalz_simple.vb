Partial Public Class myGlobalz
    'simple
    Public Shared Paradigma_Sachgebietsdatei As String
    Public Shared OhneObsoletenDokus As Boolean
    Public Shared LOGFILEKOPIE, ActionLogDir As String
    Public Shared XMLserverConfigDir As String
    Public Shared XMLclientConfigDir As String
    Public Shared ClientCookieDir As String
    Public Shared Infotext_revisionssicherheit As String
    Public Const FS_POSITIION_In_ShapeFile As Integer = 1
    Public Shared anhangtrenner As String = ";"
    Public Shared Property WINDOWS_SYSTEM_ANZEIGE_FONT As Integer = 0
    Public Shared Property mailsender2Beteiligte As Boolean = True
    Public Shared Property mailCC2Beteiligte As Boolean = True
    Public Shared Property mailRecipients2Beteiligte As Boolean = True
    Public Shared Property BestandsFensterIstgeoeffnet As Boolean = False
    Public Shared Property einVorgangistgeoeffnet As Boolean = False

    Public Shared textmarkenUeberSichtsDatei As String
    'Public Shared paradigmaDateiServerRoot As String
    'Public Shared hautMenuHoehe As Integer
    Public Shared VorlagenRoot As String
    Public Shared Birdsviewpfad As String
    Public Shared TransferString As String   'für den datenaustausch zwischen fenstern
    Public Const FsPositionInShapeFile = 1

    Shared Property AdminErrorMailPw As String = "snoopy8"
    Shared Property DMSLaufWerkBuchstabe As String
    Shared Property mapEigentuemerAktiv As Boolean = False
    Public Shared ProxyString As String
    Public Shared appdataDir As String
    Public Shared Function GetSecondBackground() As SolidColorBrush
        If  Not glob2.userIstinGastModus Then
            Return New SolidColorBrush(Colors.LightGray)
        Else
            Return New SolidColorBrush(Colors.AliceBlue)
        End If
    End Function
End Class
