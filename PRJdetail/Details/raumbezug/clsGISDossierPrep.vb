Public Class clsGISDossierPrep
    Shared Sub StartGisDossierExtern(winpt As Point, aktaid As Integer, cwidth As Integer,
                                          cheight As Integer, screenx As Double, screeny As Double,
                                             radiusInMeter As Integer, username As String,
                                             gruppe As String, vid As String, fs As String, geometrietyp As String)
        Dim strKoord As String = " koordinate=" & CStr(winpt.X).Replace(",", ".") & "," & CStr(winpt.Y).Replace(",", ".")
        Dim strIstAlb As String = " istalbberechtigt=0 "
        Dim strAktaid As String = " aktaid=" & aktaid & " "
        Dim strbreite As String = " breite=" & cwidth
        Dim strhoehe As String = " hoehe=" & cheight
        Dim strscreenx As String = " screenx=" & screenx
        Dim strscreeny As String = " screeny=" & screeny
        Dim strradius As String = " radiusinmeter=" & radiusInMeter
        Dim strusername As String = " username=" & username
        Dim strgruppe As String = " gruppe=" & gruppe
        Dim strFS As String = " fs=" & fs
        Dim strVID As String = " vid=" & vid
        Dim strGeometrie As String = " geometrie=" & geometrietyp ' punkt flurstueck


        strIstAlb = " istalbberechtigt=1 " ' alle kollegin sind berechtigt


        Dim params As String = strGeometrie & strVID & strFS & strKoord & " " & strIstAlb & " " & strAktaid &
            strbreite & strhoehe & strscreenx & strscreeny & strradius & strusername & strgruppe
        Try
            l("StartGisDossierExtern ---------------------- anfang")
            l("myglobalz.gisdossierexe   " & myGlobalz.gisdossierexe)
            l("params " & params)
            Process.Start(myGlobalz.gisdossierexe, params)
            l("StartGisDossierExtern ---------------------- ende")
        Catch ex As Exception
            l("Fehler in StartGisDossierExtern: " & params & " " ,ex)
        End Try
    End Sub

End Class
