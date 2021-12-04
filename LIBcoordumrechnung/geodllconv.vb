
' ########################################################################
' #########  Klasse class_kootrans zur Kapselung der geoDLL32 ############
' ########################################################################

Imports System.Runtime.InteropServices

'Namespace beispielprojekt


Public Class class_kootrans
    Shared Sub nachricht(ByVal text$)
        My.Application.Log.WriteEntry(text)
    End Sub
    Shared Sub nachricht_und_Mbox(ByVal text$)
        My.Application.Log.WriteEntry(text)
    End Sub


    ' Programmiert von Dr.M.Reichert, ARCADIS Consult Freiberg 
    ' (www.fg.arcadis.de)
    ' August 2003
    ' Koordinatentransformationen unter Verwendung der geodll32.dll 
    ' von C. Killet Softwareentwicklung GbR, Kempen (www.killetsoft.de)

    ' Das Vorhandensein der Dateien geodll32.dll und vo27run.dll wird 
    ' in einem Verzeichnis vorausgesetzt, auf welches in der 
    ' $PATH-Umgebungsvariable verwiesen wird, im einfachsten Falle also 

    ' das Systemverzeichnis 

    ' Import der geodll32 mit Platform Invoke, damit wird diese DLL mit 
    ' der Klasse class_kootrans gekapselt 
    ' Jede benötigte DLL-Funktion muss separat importiert werden 
    ' Um Code-Übereinstimmung mit der geodll32 zu erzielen, muss 
    ' CharSet=CharSet.Ansi eingestellt sein!


    <DllImport("geodll32.dll", EntryPoint:="setunlockcode", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Shared Function setunlockcode(ByVal pszFreischaltcode As String, ByVal pszLizenznehmer As String) As Boolean
    End Function

    <DllImport("geodll32.dll", EntryPoint:="setcoordarea", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Shared Function setcoordarea(ByVal nSchalter As Integer) As Boolean
    End Function


    <DllImport("geodll32.dll", EntryPoint:="coordtrans", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Shared Function coordtrans(ByVal nCoordXQ As Double, ByVal nCoordYQ As Double, ByVal pszKoordQ As String, ByVal nCoordSysQ As Integer, ByVal nRefSysQ As Integer, ByRef nCoordXZ As Double, _
   ByRef nCoordYZ As Double, ByRef pszKoordZ As String, ByVal nCoordSysZ As Integer, ByVal nRefSysZ As Integer, ByVal nStripZ As Integer) As Boolean
    End Function

    <DllImport("geodll32.dll", EntryPoint:="coordtrans3", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Shared Function coordtrans3( _
        ByVal nKoordXQ As Double, _
        ByVal nKoordYQ As Double, _
        ByVal nKoordSysQ As Integer, _
        ByVal nEllipsoidQ As Integer, _
        ByVal nNotationQ As Integer, _
        ByRef nKoordXZ As Double, _
        ByRef nKoordYZ As Double, _
        ByVal nKoordSysZ As Integer, _
        ByVal nEllipsoidZ As Integer, _
        ByVal nNotationZ As Integer, _
        ByVal nStreifenZ As Integer) As Integer
    End Function

    Public Shared Function freischalten() As String
        ' gibt die Funktion setunlockcode nach außen
        '("054512522-763942917", "Kreis Offenbach, Dr. Feinen")
        Try
            nachricht("freischalten ------------------------------")
            If setunlockcode("054512522-763942917", "Kreis Offenbach, Dr. Feinen") Then
                nachricht("freischalten ------------------------------ ende ok")
                Return "ok"
            Else
                nachricht("freischalten ------------------------------ ende nicht ok fehler")
                Return "nicht freigeschaltet"
            End If
            nachricht("freischalten ------------------------------ ende")
        Catch ex As Exception
            nachricht(String.Format("Koordinaten-Modul ist nicht installiert!{0}{1}", vbCrLf, ex))
            Return "nicht freigeschaltet"
        End Try
    End Function

    'Public Shared Function gausskrueger3GradZuGeografisch(ByVal rechts As Double, ByVal hoch As Double, ByRef laenge As Double, ByRef breite As Double) As String

    '    ' gibt die DLL-Funktion coordtrans in einer bestimmten, auf den
    '    ' jeweiligen Einsatzzweck abgestimmten, Parameterkonfiguration nach
    '    ' außen

    '    ' Rechnet die übergebenen Rechts- und Hochwerte des
    '    ' Koordinatensystems 3° Gauss-Krüger, Bezugssystem DHDN
    '    ' in geografische Koordinaten der Formatierung gggmm (Grad, Minute),
    '    ' Bezugssystem WGS84, um.

    '    Dim ergebnis As String = ""
    '    Dim outstring As String = ""
    '    ' Dimensionierung des out-Parameters, da nicht im Funktionsaufruf

    '    ' vorhanden, weil er hier eigentlich nicht gebraucht wird

    '    laenge = InlineAssignHelper(breite, 0)
    '    ' out-Werte zuweisen, falls es wegen Dateneingabefehler nicht zu
    '    ' einer Berechnung kommt

    '    ' eigene Koordinatenprüfung
    '    If rechts > 9999999 OrElse rechts < 0 Then
    '        Return InlineAssignHelper(ergebnis, "Fehler in Rechtswerteingabe")
    '    End If
    '    If hoch > 9999999 OrElse hoch < -9999999 Then
    '        Return InlineAssignHelper(ergebnis, "Fehler in Hochwerteingabe")
    '    End If

    '    setcoordarea(1)

    '    'Koordinatenüberprüfung der DLL einschalten

    '    If coordtrans(rechts, hoch, "", 2, 0, laenge, breite, outstring, 32, 0, 0) Then
    '        ergebnis = "ok"
    '    Else
    '        ergebnis = "fehler bei berechnung"
    '    End If

    '    Return ergebnis
    'End Function

    'Public Shared Function geografischZuGausskrueger3Grad(ByVal laenge As Double, _
    '                                               ByVal breite As Double, _
    '                                               ByVal zielStreifen As Integer, _
    '                                               ByRef rechts As Double, ByRef hoch As Double, _
    '                                               ByRef quelleNotation%) As String
    '    ' gibt die DLL-Funktion coordtrans in einer bestimmten, auf den
    '    ' jeweiligen Einsatzzweck abgestimmten, Parameterkonfiguration nach

    '    ' außen

    '    'der 4 parameter quelleNotation% regelt die art der notation der koordinaten
    '    'ist normal 32
    '    'bei ricoh aber 1
    '    '1          Geographische Koordinaten (Greenwich) [Grad, Min, Sek]
    '    '6          Geographische Koordinaten (Greenwich) [Grad]
    '    '32          Geographische Koordinaten (Greenwich) [Grad, Min] 
    '    '33          Geographische Koordinaten (Greenwich) [Sek]


    '    ' Rechnet die übergebenen geografischen Längen- und Breitenangaben
    '    ' der Formatierung gggmm (Grad, Minute) , Bezugssystem WGS84
    '    ' in Koordinaten des Koordinatensystems 3° Gauss-Krüger,
    '    ' Bezugssystem DHDN, um.
    '    ' Der Parameter zielStreifen bestimmt den Streifen, für welchen die
    '    ' Ergebniskoordinaten errechnet werden sollen 

    '    'ergebnis=koordinatentransformation.geografischZuGausskrueger3Grad(
    '    '1152.87,  5413.91   ,3,out rechts, out hoch);
    '    ' glob2.nachricht_und_Mbox("Ergebnis: " + ergebnis + "   MS: 3   RW: " + rechts
    '    ' + "     HW: " + hoch);

    '    Dim ergebnis As String = ""
    '    Dim outstring As String = ""
    '    rechts = InlineAssignHelper(hoch, 0)
    '    If zielStreifen < 0 OrElse zielStreifen > 120 Then
    '        Return InlineAssignHelper(ergebnis, "Fehler in Meridianstreifeneingabe")
    '    End If

    '    setcoordarea(1)
    '    If coordtrans(laenge, breite, "", quelleNotation%, 0, rechts, hoch, outstring, 2, 0, zielStreifen) Then
    '        ergebnis = "ok"
    '    Else
    '        ergebnis = "fehler bei berechnung"
    '    End If

    '    Return ergebnis
    'End Function


    Public Shared Function InlineAssignHelper(Of T)(ByRef target As T, ByVal value As T) As T
        target = value
        Return value
    End Function

    'Sub gk2UTM(ByRef nKoordXQ As Double, ByRef nKoordYQ As Double, ByRef nKoordxZ As Double, ByRef nKoordYZ As Double)
    '    nachricht("gk2UTM ------------------------")
    '    nachricht("nKoordXQ " & nKoordXQ)
    '    nachricht("nKoordYQ " & nKoordYQ)
    '    Dim outstring$ = ""
    '    Dim rc As Boolean
    '    Try
    '        nachricht("geodllconv gk2UTM -------------------------------------")
    '        rc = coordtrans(nKoordXQ, nKoordYQ, "", 2, 0, nKoordxZ, nKoordYZ, outstring$, 3, 10, 0)
    '        nachricht("danach ------------------------")
    '        nachricht("nKoordXQ " & nKoordxZ)
    '        nachricht("nKoordYQ " & nKoordYZ)
    '    Catch ex As Exception
    '        nachricht("gk2UTM rc  " & ex.ToString)
    '    End Try
    'End Sub

    Sub geografischgoogle2UTM(ByRef nKoordXQ As Double, ByRef nKoordYQ As Double, ByRef nKoordxZ As Double, ByRef nKoordYZ As Double, ByVal quellenotation%)
        nachricht("geografischgoogle2UTM ------------------------")
        nachricht("nKoordXQ " & nKoordXQ)
        nachricht("nKoordYQ " & nKoordYQ)
        Dim outstring$ = ""
        Dim rc As Boolean
        Try
            nachricht("geodllkonv geografischgoogle2UTM------------------------------------------")
            rc = coordtrans(nKoordXQ, nKoordYQ, "", quellenotation%, 0, nKoordxZ, nKoordYZ, outstring$, 3, 10, 0)
            nachricht("danach ------------------------")
            nachricht("nKoordXQ " & nKoordxZ)
            nachricht("nKoordYQ " & nKoordYZ)
            nachricht("geodllkonv geografischgoogle2UTM ende")
        Catch ex As Exception
            nachricht("geodllkonv geografischgoogle2UTM rc  " & ex.ToString)
        End Try
        'der 4 parameter quelleNotation% regelt die art der notstion der koordinaten
        'ist normal 32
        'bei ricoh aber 1
        '1          Geographische Koordinaten (Greenwich) [Grad, Min, Sek]
        '6          Geographische Koordinaten (Greenwich) [Grad]
        '32          Geographische Koordinaten (Greenwich) [Grad, Min] 
        '33          Geographische Koordinaten (Greenwich) [Sek]
    End Sub

    Sub UTM2geografischgoogle(ByRef nKoordXQ As Double, ByRef nKoordYQ As Double, ByRef nKoordxZ As Double, ByRef nKoordYZ As Double,
                              ByVal quellenotation%)
        nachricht("UTM2geografischgoogle ------------------------")
        nachricht("nKoordXQ " & nKoordXQ)
        nachricht("nKoordYQ " & nKoordYQ)
        'Dim pszCoordZ, pszKoordQ As String  'alfanum zeielkoordinate
        Dim rc As Boolean

        Dim nKoordSysQ As Integer
        Dim nEllipsoidQ As Integer
        Dim nNotationQ As Integer

        Dim nKoordSysZ As Integer
        Dim nEllipsoidZ As Integer
        Dim nNotationZ As Integer
        Dim nStreifenZ As Integer
        Dim nrefsysq, nrefsysz As Integer
        Dim nstripZ As Integer
        Try
            nachricht("geodllkonv UTM2geografischgoogle------------------------------------------")
            'Quelle
            If Not nKoordXQ.ToString.StartsWith("32") Then
                nKoordXQ = nKoordXQ + 32000000
            End If
            nKoordSysQ = 3
            nEllipsoidQ = 10
            nNotationQ = 0
            nrefsysq = 0
            'Ziel
            nKoordSysZ = 1
            nEllipsoidZ = 10
            nNotationZ = 0
            nStreifenZ = 3
            nrefsysz = 0
            nstripZ = 0

           ' rc = coordtrans3(nKoordXQ, nKoordYQ, pszKoordQ, nKoordSysQ, nrefsysq, nKoordxZ, nKoordYZ, pszCoordZ, nKoordSysZ, nrefsysz, nstripZ)

            rc = CBool(coordtrans3(nKoordXQ, nKoordYQ, nKoordSysQ, nEllipsoidQ, nNotationQ, 
                            nKoordXZ, nKoordYZ, nKoordSysZ, nEllipsoidZ, nNotationZ, nStreifenZ))
            nachricht("danach ------------------------")
            nachricht("nKoordXQ " & nKoordxZ)
            nachricht("nKoordYQ " & nKoordYZ)
            nachricht("geodllkonv geografischgoogle2UTM ende")
        Catch ex As Exception
            nachricht("geodllkonv geografischgoogle2UTM rc  " & ex.ToString)
        End Try
        'der 4 parameter quelleNotation% regelt die art der notstion der koordinaten
        'ist normal 32
        'bei ricoh aber 1
        '1          Geographische Koordinaten (Greenwich) [Grad, Min, Sek]
        '6          Geographische Koordinaten (Greenwich) [Grad]
        '32          Geographische Koordinaten (Greenwich) [Grad, Min] 
        '33          Geographische Koordinaten (Greenwich) [Sek]
    End Sub

    Public Function UTMZuGausskrueger3Grad(ByVal rechtsUTM As Double, ByVal hochUTM As Double, ByVal zielStreifen As Integer, ByRef rechts As Double, ByRef hoch As Double) As String
        nachricht("geodllconv UTMZuGausskrueger3Grad----------------------------------------")
        '  MsgBox("huhu")
        Dim ergebnis As String = ""
        Dim outstring As String = ""
        Try
            rechts = InlineAssignHelper(hoch, 0)
            If zielStreifen < 0 OrElse zielStreifen > 120 Then
                Return InlineAssignHelper(ergebnis, "Fehler in Meridianstreifeneingabe")
            End If
            If rechtsUTM < 32000000 Then
                rechtsUTM = 32000000 + rechtsUTM
            End If
            ' setcoordarea(1)
            If coordtrans(rechtsUTM, hochUTM, "", 3, 10, rechts, hoch, outstring, 2, 10, 3) Then
                ergebnis = "ok"
            Else
                ergebnis = "fehler bei berechnung"
            End If
            'MsgBox("ergebnis:" & ergebnis.ToLower)
            nachricht("geodllconv UTMZuGausskrueger3Grad ende")
            Return ergebnis
        Catch ex As Exception
            nachricht("Fehler in  geodllconv UTMZuGausskrueger3Grad: " & ex.ToString)
            Return ""
        End Try
    End Function
End Class