Public Class clsgesetzesManagerDok
    Property originalDateiName As String = ""

    Property schlagworte As String = ""
    Property beschreibung As String = ""
    Property ordner As String = ""
    Property dateinameohneext As String = ""
    Property dateityp As String = ""
    Property sachgebietnr As String = ""
    'Public Property art As String = ""
    Public Property artId As Integer = 0
    Public Property art_text As String = "unbekannt"
    Public Property stammid As Integer
    Public Property quellentyp As String = ""
    Public Property url As String = ""
    Property userInitial As String = ""
    'Public Property woveroeffentlicht As String = "" ' woveroeffentlicht
    Public Property herkunftId As Integer = 0 ' woveroeffentlicht
    Public Property herkunft_text As String = "unbekannt"
    Property wannveroeffentlicht As Date = CDate("1901.01.01")
    Public Property istgueltig As Boolean = True
    Public Property farbnummer As Integer
    Public Property sachgebietheader As String
    Property FullnameImArchiv As String
    Function getFullnameImArchiv(gesetzesRootdir As String) As String
        FullnameImArchiv = gesetzesRootdir & ordner & "\" & dateinameohneext & dateityp
        Return FullnameImArchiv
    End Function

End Class
