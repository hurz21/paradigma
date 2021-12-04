#Disable Warning BC40056 ' Namespace or type specified in the Imports 'JFvCard.MyProject.vCard' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports JFvCard.MyProject.vCard
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'JFvCard.MyProject.vCard' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.

Public Class winvcfTools

    Shared Sub trimmen(ByRef lines As String())
        For i = 0 To lines.GetUpperBound(0)
            lines(i) = LIBgemeinsames.clsString.noWhiteSpace(lines(i))
        Next
        For i = 0 To lines.GetUpperBound(0)
            lines(i) = Trim(lines(i))
        Next
    End Sub
    Public Shared Sub ZeilenZusammnenfuegen1(ByVal jfread As jfcardreader.vCard.vCardReader, ByRef lines As String(), ByVal continueLinestring As String)
        'zeilen zusammnenfuegen
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
    End Sub

    Public Shared Sub bereinigen(ByRef vstring As String)
        vstring = vstring.Replace("LANGUAGE=de", "")
        vstring = vstring.Replace("CHARSET = Windows - 1252", "")
        vstring = vstring.Replace("CHARSET=Windows-1252", "")
        vstring = vstring.Replace("ENCODING=QUOTED-PRINTABLE", "")
        vstring = vstring.Replace("quoted-printable", "")
        vstring = vstring.Replace(";:", ":")
        vstring = vstring.Replace("=E4", "ä")
        vstring = vstring.Replace("=E4", "ä")

    End Sub
End Class
