'Imports JFvCard.MyProject.vCard


'Module Module1
'    Sub main()
'        Dim datei = "../../sample/gunilla weyers2.vcf"
'        datei = "../../sample/dr_j_feinen.vcf"
'        ' datei = "../../sample/Hans R. Schwarting.vcf" 
'        'datei = "../../sample/Marcus-Andre Sauer (mas@cookfactory.de).vcf"
'        '   datei = "../../sample/Markus Heidrich.vcf"
'        'datei = "../../sample/Michael Lapa.vcf"
'        ' datei = "../../sample/Verena Koettker.vcf"
'        vcardread(datei)
'    End Sub
'    Sub vcardread(ByVal datei As String)
'        '   Dim nachname, vorname, titelemail, telhome, telwork, telworkfax, telhomefax, org As String
'        Dim jfread As New vCardReader
'        Dim vstring As String = ""
'        Dim lines As String()
'        Dim test As String = ""
'        Dim imail As New Email
'        Dim phones As New List(Of Phone)
'        Dim adresses As New List(Of Address)
'        Dim enc As System.Text.Encoding = System.Text.Encoding.GetEncoding(1252)
'        Dim continueLinestring As String = "=0D=0A="
'        vstring = IO.File.ReadAllText(datei, enc)
'        vstring = vstring.Replace("LANGUAGE=de", "")
'        vstring = vstring.Replace("CHARSET = Windows - 1252", "")
'        vstring = vstring.Replace("ENCODING=QUOTED-PRINTABLE", "")
'        vstring = vstring.Replace("quoted-printable", "")
'        vstring = vstring.Replace(";:", ":")
'        lines = vstring.Split(CChar(vbCrLf))
'        'zeilen zusammnenfuegen
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)
'        lines = jfread.zeilen_zusammnenfuegen(lines, continueLinestring)

'        jfread.getnames(lines, jfread.familyname, jfread.Vorname, jfread.MiddleName, jfread.Prefix, jfread.Suffix)
'        jfread.getformattedname(lines, jfread.FormattedName)
'        jfread.getORG(lines, jfread.Org)
'        jfread.getTITLE(lines, jfread.Title)

'        jfread.getEMAIL(lines, test)
'        imail.address = test

'        jfread.getphones(lines, phones)
'        jfread.getadresses(lines, adresses)
'    End Sub



'End Module
