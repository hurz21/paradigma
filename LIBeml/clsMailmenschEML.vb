Public Class clsMailmenschEML
    Implements iMailmensch

    Public Property name As String Implements iMailmensch.name
    Public Property email As String Implements iMailmensch.email
    Public Property shortemail As String Implements iMailmensch.shortemail
    Public Property organization As String Implements iMailmensch.organization
    Public Property literalname As String Implements iMailmensch.literalname
    Public Property telefon As String Implements iMailmensch.telefon
    Public Sub New()
        name = ""
        email = ""
        shortemail = ""
        organization = ""
        literalname = ""
        telefon = ""
    End Sub
End Class
