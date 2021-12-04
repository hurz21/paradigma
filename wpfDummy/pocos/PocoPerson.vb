Public Class PocoPerson
    Property lastchange As Date
    Property PersonenVorlage As Integer = 0
    Public Property ausgewaehlt As Boolean
    Public anychange As Boolean
    Property ExpandHeaderInSachgebiet As String
    Public Property Kontakt As New Kontaktdaten
    Public Property Anrede As String
    Public Property Bezirk As String
    Public Property Name As String
    Public Property akademischerTitel As String
    Public Property Vorname As String
    Public Property Bemerkung As String
    Public Property Status As Integer
    Public Property PersonenID As Integer
    Public Property Rolle As String
    Public Property Namenszusatz As String
    Public Property Kassenkonto As String
    Public Property Quelle As String
    Public Property VERTRETENDURCH As String = ""

    Public Sub clear()
        PersonenID = 0
        Name = ""
        Vorname = ""
        Anrede = ""
        Bemerkung = ""
        Namenszusatz = ""
        Quelle = ""
        Kontakt.clear()
        Kassenkonto = ""
        Status = 0
        Rolle = ""
        Bezirk = ""
        VERTRETENDURCH = ""
        Kontakt = New Kontaktdaten
    End Sub
    Sub New()
        clear()
    End Sub

    Public Overrides Function tostring() As String
        Dim a As String = String.Format("Name: {0}{1}", Name, vbCrLf)
        a = String.Format("{0}Vorname: {1}{2}", a, Vorname, vbCrLf)
        a = String.Format("{0}Anrede: {1}{2}", a, Anrede, vbCrLf)
        a = String.Format("{0}Zusatz: {1}{2}", a, Namenszusatz, vbCrLf)
        a = String.Format("{0}Bemerkung: {1}{2}", a, Bemerkung, vbCrLf)
        Return a
    End Function

End Class
