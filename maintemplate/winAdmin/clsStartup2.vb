Public Class clsStartup2
    Public Shared Function getTitle(modul As String) As String
        Dim title As String

        CLstart.mycSimple.ParadigmaVersion = My.Resources.BuildDate.Trim.Replace(vbCrLf, "")

        title = modul & "; " & myGlobalz.sitzung.aktBearbeiter.Name & ", Version: " & CLstart.mycSimple.ParadigmaVersion
        Return title
    End Function
End Class
