Public Class clsKfas
    Public Property datum As Date
    Public Property dateiname As String = ""
    Public Property beilagen As String()
    Public Property transId As String = ""
    Public Property typ As String = ""
    Public Property Titel As String = ""
    Public Property Ort As String = ""
    Public Property Ortsteil As String = ""
    Public Property summe As String = ""
    Public Property dateidatum As Date
    Public Property paare As String = ""
    Public Property AS_person As New Person
    Public Property dict As New Dictionary(Of String, String)
    Public Property dateikurzname As String = ""
    Public Property verzeichnis As String = ""
    Public Property kfa_typ As String = ""
    Public Property kfa_typ_klartext As String
    Sub New()

    End Sub
End Class
