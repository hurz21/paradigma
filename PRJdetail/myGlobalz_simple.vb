Partial Public Class myGlobalz
    'simple
    Public Shared Property dokumenteManuellSichern As Boolean = False
    Public Shared Property WordSperreeschonAktiv As Boolean
    Public Shared Property ExcelSperreschonAktiv As Boolean
    Public Shared Paradigma_Sachgebietsdatei As String = ""
    Public Shared OhneObsoletenDokus As Boolean
    Public Shared LOGFILEKOPIE As String = ""
    Public Shared Property ActionLogDir As String = ""
    Public Shared XMLserverConfigDir As String = ""
    Public Shared XMLclientConfigDir As String = ""
    Public Shared ClientCookieDir As String = ""
    Public Const FS_POSITIION_In_ShapeFile As Integer = 1
    Public Shared anhangtrenner As String = ";"
    Public Shared Property WINDOWS_SYSTEM_ANZEIGE_FONT As Integer = 0
    Public Shared Property mailsender2Beteiligte As Boolean = True
    Public Shared Property mailCC2Beteiligte As Boolean = True
    Public Shared Property mailRecipients2Beteiligte As Boolean = True
    'Public Shared Property BestandsFensterIstgeoeffnet As Boolean = False
    Public Shared Property einVorgangistgeoeffnet As Boolean = False


    Public Shared textmarkenUeberSichtsDatei As String = ""
    'Public Shared paradigmaDateiServerRoot As String
    'Public Shared hautMenuHoehe As Integer
    Public Shared VorlagenRoot As String = ""
    Public Shared Birdsviewpfad As String = ""
    'Public Shared TransferString As String = "" 'für den datenaustausch zwischen fenstern
    Public Const FsPositionInShapeFile = 1

    Shared Property AdminErrorMailPw As String = "snoopy8"
    Shared Property DMSLaufWerkBuchstabe As String = ""
    Shared Property mapEigentuemerAktiv As Boolean = False
    Public Shared ProxyString As String = ""
    Public Shared appdataDir As String = ""
    Public Shared gisdossierexe As String = "C:\kreisoffenbach\gisdossier\gisdossier.exe "
    Public Shared Function GetSecondBackground() As SolidColorBrush
        If  not  glob2.userIstinGastModus Then
            Return New SolidColorBrush(Colors.LightGray)
        Else
            Return New SolidColorBrush(Color.FromRgb(&HDD, &HF8, &HB0))
        End If
    End Function
End Class
