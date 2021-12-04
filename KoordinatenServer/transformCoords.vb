Imports System.Runtime.InteropServices

Module transformCoords



    <DllImport("geodll32.dll", EntryPoint:="setunlockcode", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Function setunlockcode(ByVal pszFreischaltcode As String, ByVal pszLizenznehmer As String) As Boolean
    End Function

    <DllImport("geodll32.dll", EntryPoint:="setcoordarea", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Function setcoordarea(ByVal nSchalter As Integer) As Boolean
    End Function


    <DllImport("geodll32.dll", EntryPoint:="coordtrans", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Function coordtrans(ByVal nCoordXQ As Double, ByVal nCoordYQ As Double, ByVal pszKoordQ As String, ByVal nCoordSysQ As Integer, ByVal nRefSysQ As Integer, ByRef nCoordXZ As Double, _
   ByRef nCoordYZ As Double, ByRef pszKoordZ As String, ByVal nCoordSysZ As Integer, ByVal nRefSysZ As Integer, ByVal nStripZ As Integer) As Boolean
    End Function

    <DllImport("geodll32.dll", EntryPoint:="coordtrans3", ExactSpelling:=False, CharSet:=CharSet.Ansi, SetLastError:=True)> _
    Public Function coordtrans3(ByVal nKoordXQ As Double, _
        ByVal nKoordYQ As Double, _
        ByVal nKoordSysQ As Integer, _
        ByVal nEllipsoidQ As Integer, _
        ByVal nNotationQ As Integer, _
        ByRef nKoordXZ As Double, _
        ByRef nKoordYZ As Double, _
        ByVal nKoordSysZ As Integer, _
        ByVal nEllipsoidZ As Integer, _
        ByVal nNotationZ As Integer, _
        ByVal nStreifenZ As Integer) As Boolean
    End Function


    'Private Declare Function coordtrans3 Lib "GEODLL32" ( _
    '    ByVal nKoordXQ As Double, _
    '    ByVal nKoordYQ As Double, _
    '    ByVal nKoordSysQ As Integer, _
    '    ByVal nEllipsoidQ As Integer, _
    '    ByVal nNotationQ As Integer, _
    '    ByRef nKoordXZ As Double, _
    '    ByRef nKoordYZ As Double, _
    '    ByVal nKoordSysZ As Integer, _
    '    ByVal nEllipsoidZ As Integer, _
    '    ByVal nNotationZ As Integer, _
    '    ByVal nStreifenZ As Integer) As Integer

    Public Function freischalten() As String
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
            nachricht("Koordinaten-Modul ist nicht installiert!" & vbCrLf & ex.ToString)
            Return "nicht freigeschaltet"
        End Try
    End Function
    '---------------------------------------------------------------------------------------
    ' Procedure : TransformCoord
    ' DateTime  : 20.02.2007 14:46
    ' Author    : A670024
    ' Purpose   :
    '---------------------------------------------------------------------------------------
    '
    Sub ktransform(ByVal quellKoord$, ByVal zielKoord$, ByVal rechts_in$, ByVal hoch_in$, ByRef rechts_Out$, ByRef hoch_Out$, ByVal retcode%)
        '("UTM", "GK", rechts_In$, hoch_In$, rechts_Out$, hoch_Out$)
        On Error GoTo TransformCoord_Error
        ''mylogmsg "TransformCoord   #################################### beginn"
        ''mylogmsg "DAteneingang: ", quellKoord, zielKoord$, rechts_in$, hoch_in
        Dim rc As Boolean
        rechts_Out$ = rechts_in
        hoch_Out$ = hoch_in
        'Variablen deklarieren
        Dim nKoordXQ As Double
        Dim nKoordYQ As Double
        Dim nKoordSysQ As Integer
        Dim nEllipsoidQ As Integer
        Dim nNotationQ As Integer
        Dim nKoordXZ As Double
        Dim nKoordYZ As Double
        Dim nKoordSysZ As Integer
        Dim nEllipsoidZ As Integer
        Dim nNotationZ As Integer
        Dim nStreifenZ As Integer
        Dim nStreifenQ As Integer


        ''mylogmsg "rechts_in$"

        rechts_in$ = Replace(rechts_in$, ",", ".")
        hoch_in$ = Replace(hoch_in$, ",", ".")

        nKoordXQ = Val(rechts_in)
        nKoordYQ = Val(hoch_in)

        ''mylogmsg "Val nKoordXQ: ", CStr(nKoordXQ)

        nKoordXZ = 0
        nKoordYZ = 0

        If quellKoord = "UTM" Then
            nKoordSysQ = 3        '1-geogr koor, 2-GK 3-UTM
            nEllipsoidQ = 10       '1 war ?, 10 ist wgs84
            nNotationQ = 0
            If Not nKoordXQ.ToString.StartsWith("32") Then
                nKoordXQ = nKoordXQ + 32000000
            End If
        End If
        If zielKoord$ = "UTM" Then
            nKoordSysZ = 3        '1-geogr koor, 2-GK 3-UTM
            nEllipsoidZ = 10       '1 war ?, 10 ist wgs84
            nNotationZ = 0

        End If

        If quellKoord = "GK" Then
            nKoordSysQ = 2
            nEllipsoidQ = 1             '1 war ?, 10 ist wgs84
            nNotationQ = 0  '#####
            nStreifenQ = 3
        End If
        If zielKoord = "GK" Then
            nKoordSysZ = 2
            nEllipsoidZ = 1             '1 war ?, 10 ist wgs84
            nNotationZ = 0  '#####
            nStreifenZ = 3
        End If

        If Left$(zielKoord, 6) = "WINKEL" Then
            nKoordSysZ = 1               '1-geogr koor, 2-GK 3-UTM
            nEllipsoidZ = 10             '1 war ?, 10 ist wgs84
            If zielKoord$ = "WINKEL_G" Then nNotationZ = 0
            If zielKoord$ = "WINKEL_GMS" Then nNotationZ = 1
            If zielKoord$ = "WINKEL_GM" Then nNotationZ = 2
            If zielKoord$ = "WINKEL_S" Then nNotationZ = 3
            If zielKoord$ = "WINKEL_GON" Then nNotationZ = 4
        End If

        If Left$(quellKoord, 6) = "WINKEL" Then
            nKoordSysQ = 1
            nEllipsoidQ = 10             '1 war ?, 10 ist wgs84
            If quellKoord$ = "WINKEL_G" Then nNotationQ = 0
            If quellKoord$ = "WINKEL_GMS" Then nNotationQ = 1
            If quellKoord$ = "WINKEL_GM" Then nNotationQ = 2
            If quellKoord$ = "WINKEL_S" Then nNotationQ = 3
            If quellKoord$ = "WINKEL_GON" Then nNotationQ = 4
        End If

        ''mylogmsg "vor aufruf  von setunlock"
        Dim nRefSysQ = nEllipsoidQ
        Dim nRefSysZ = nEllipsoidZ
        Dim outstring$

        rc = setunlockcode("054512522-763942917", "Kreis Offenbach, Dr. Feinen")
        ''mylogmsg "rc  beim unlocken:     " + CStr(rc)

        '    coordtrans(rechtsUTM, hochUTM, "", 3, 10, rechts, hoch, outstring, 2, 1, 3)utm nach gk

        '  rc = coordtrans(nKoordXQ, nKoordYQ, "", 2,  0, nKoordxZ, nKoordYZ, outstring$, 3, 10, 0) gk nach utm


        'rc = coordtrans(nKoordXQ, nKoordYQ, "", nKoordSysQ, nRefSysQ, _
        '      nKoordXZ, nKoordYZ, outstring, nKoordSysZ, nRefSysZ, nStreifenZ)
        rc = coordtrans3(nKoordXQ, nKoordYQ, nKoordSysQ, nEllipsoidQ, nNotationQ, _
                      nKoordXZ, nKoordYZ, nKoordSysZ, nEllipsoidZ, nNotationZ, nStreifenZ)
        ''mylogmsg "rc   coordtrans3:    " + CStr(rc)
        rechts_Out$ = CStr(nKoordXZ)
        hoch_Out$ = CStr(nKoordYZ)
        ''mylogmsg "rechts_Out$     " + rechts_Out$
        ''mylogmsg "hoch_Out$     " + hoch_Out$

        ''mylogmsg "TransformCoord   #################################### ende"

        On Error GoTo 0
        Exit Sub

TransformCoord_Error:
        ' mylog("Error " & Err.Number & " (" & Err.Description & ") in procedure TransformCoord of Modul suchdb")
    End Sub











End Module
