Imports System
Imports System.IO
Namespace CLstart
    Public Class clsPlattenplatz
        ''' <summary>
        ''' ermittelt den plattenplatz  für das archiv DMS
        ''' </summary>
        ''' <param name="dreiveletta"></param>
        ''' <remarks></remarks>
        Public Shared Function clsPlattenplatz(dreiveletta As String) As String
            Try
                Dim di As DriveInfo = New DriveInfo(dreiveletta)
                Dim gb As Double '= CInt(di.AvailableFreeSpace / 1000000000)
                Dim mySize As Single = di.AvailableFreeSpace
                Dim neu As String
                Select Case mySize
                    Case 0 To 1023
                        neu = mySize & " Bytes"
                    Case 1024 To 1048575
                        neu = Format(mySize / 1024, "###0.00") & " KB"
                    Case 1048576 To 1043741824
                        neu = Format(mySize / 1024 ^ 2, "###0.00") & " MB"
                    Case Is > 1043741824
                        neu = Format(mySize / 1024 ^ 3, "###0.00") & " GB"
                End Select
                gb = CInt((mySize / 1024 ^ 3) * 100) / 100
                nachricht("Rest in Gigabyte: " & gb)
                If gb < 10 Then
                    If Environment.UserName.ToLower = "feinen_j" Then
                        MsgBox("fehler Der Plattenplatz wird langsam knapp (<10GB). Er sollte aufgestockt werden! (Hotline: file-paradigma um 20 GB erweitern)" & neu)
                        nachricht("fehler Der Plattenplatz wird langsam knapp (<10GB). Er sollte aufgestockt werden! (Hotline: file-paradigma um 20 GB erweitern)" & neu)
                    End If
                End If
                If gb < 5 Then
                    MsgBox("fehler Der Plattenplatz wird langsam knapp (<5GB). Er sollte aufgestockt werden! (Hotline: file-paradigma um 20 GB erweitern)" & neu)
                    nachricht("fehler Der Plattenplatz wird langsam knapp (<10GB). Er sollte aufgestockt werden! (Hotline: file-paradigma um 20 GB erweitern)" & neu)
                End If
                If gb < 1 Then
                    MsgBox("Im \\file-paradigma steht zu wenig Plattenplatz zur Verfügung. Bitte sofort bei IT melden und ca. 20GB aufstocken lassen." & neu)
                    nachricht("fehler Der plattenplatz für das DMS wird knapp (<1GB). Er sollte erweitert werden!" & neu)
                End If
                nachricht("plattenplatz ok: " & neu)
                Return neu
            Catch e As Exception
                nachricht("fehler The clsPlattenplatz DMS failed: drive: " & dreiveletta & ":  ! vermutlich nicht gemounted " & Environment.NewLine &
                            ".  unc-pfad prüfen!!" & Environment.NewLine &
                            e.ToString())
                Return "-1" '"Fehler bei Berechnung des Plattenplatzes"
            End Try
        End Function

        Shared Sub nachricht(text As String)
            '   My.Log.WriteEntry(text)
        End Sub
    End Class

End Namespace



