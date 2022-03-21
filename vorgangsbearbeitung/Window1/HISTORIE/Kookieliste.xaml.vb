Imports System.Data

Public Class Kookieliste

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub Kookieliste_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing

        If Window1.kookieFenster IsNot Nothing Then Window1.kookieFenster = Nothing
        savePosition()
    End Sub

    Private Sub Kookieliste_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        ShowHistory()
        If myGlobalz.nurEinBildschirm Then Exit Sub
        Me.Top = CLstart.formposition.getPosition("diverse", "winKOOKIEositiontop", Me.Top)
        Me.Left = CLstart.formposition.getPosition("diverse", "winKOOKIEpositionleft", Me.Left)
    End Sub
    Private Sub savePosition()
        If myGlobalz.nurEinBildschirm Then Exit Sub
        Try
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winKOOKIEositiontop", CType(Me.Top, String))
            CLstart.myc.userIniProfile.WertSchreiben("diverse", "winKOOKIEpositionleft", CType(Me.Left, String))
        Catch ex As Exception
            l("fehler in saveposition  windb" & ex.ToString)
        End Try
    End Sub

    Private Sub ShowHistory()
        Dim collHistory As List(Of CLstart.HistoryKookie.HistoryItem)
        collHistory = CLstart.HistoryKookie.VerlaufsCookieLesen.exe(myGlobalz.ClientCookieDir & "verlaufscookies")
        dgHistory.DataContext = collHistory
    End Sub

    Private Sub dgHistory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True
        Try
            Dim item As CLstart.HistoryKookie.HistoryItem
            Try
                item = CType(dgHistory.SelectedItem, CLstart.HistoryKookie.HistoryItem)
            Catch ex As Exception
                e.Handled = True
                Exit Sub
            End Try
            item = CType(dgHistory.SelectedItem, CLstart.HistoryKookie.HistoryItem)
            If item Is Nothing Then
                item = CType(dgHistory.SelectedItem, CLstart.HistoryKookie.HistoryItem)
                If item Is Nothing Then Return
            End If
            myGlobalz.sitzung.aktVorgangsID = item.ID
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            glob3.allAktobjReset.execute(myGlobalz.sitzung)

            Dim auswahlid As String = CStr(item.ID) ' (clsDBtools.fieldvalue(item("vorgangsid")))
            Dim beschreibung As String = item.Titel ' item("BESCHREIBUNG").ToString()
            Dim az2 As String = item.AZ ' item("AZ2").ToString()
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2, myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

            myGlobalz.sitzung.aktVorgangsID = CInt(auswahlid)

            e.Handled = True
            Me.Close()

            glob2.editVorgang(CInt(auswahlid), myGlobalz.testmode)
            Window1.kookieFenster = Nothing
        Catch ex As Exception
            nachricht_und_Mbox("" & ex.ToString)
        End Try
    End Sub

    Private Sub btnstammdaten_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        nachricht("BESTAND")
        'KillKookieFenster()
        '  Dim procExists as Boolean = Process.GetProcesses().Any(Function(p) p.ProcessName.Contains("Bestand-"))
        If myglobalz.BestandsFensterIstgeoeffnet Then
            ' MsgBox("Es ist bereits eine Bestandsübersicht geöffnet. Sie können Sie direkt über die Taskbar von Windows aufrufen.", MsgBoxStyle.Information, "Zur Bestandsliste")
            Exit Sub
        End If
        nachricht("BESTAND2")
        clsStartup.FormularBestandStammdaten(False, False)
    End Sub

    Private Sub btnDokumente_Click(sender As Object, e As RoutedEventArgs)
        nachricht("Dokumente")
        Dim wz As New winDokuFilter(False)
        Dim modalOeffnen = False
        If modalOeffnen Then
            wz.ShowDialog()
        Else
            wz.Show()
        End If
        nachricht("FormularBestandStammdaten3")
    End Sub

    Private Sub btnAdresse_Click(sender As Object, e As RoutedEventArgs)
        nachricht("Dokumente")
        Dim wz As New winAdressBestand(False)
        Dim modalOeffnen = False
        If modalOeffnen Then
            wz.ShowDialog()
        Else
            wz.Show()
        End If
        nachricht("FormularBestandStammdaten3")
    End Sub

    Private Sub btnFST_Click(sender As Object, e As RoutedEventArgs)
        nachricht("Dokumente")
        Dim wz As New winFlurstueckFilter(False)
        Dim modalOeffnen = False
        If modalOeffnen Then
            wz.ShowDialog()
        Else
            wz.Show()
        End If
        nachricht("FormularBestandStammdaten3")
    End Sub

    Private Sub btnBeteiligte_Click(sender As Object, e As RoutedEventArgs)
        nachricht("Dokumente")
        Dim wz As New winBestandBeteiligte(False)
        Dim modalOeffnen = False
        If modalOeffnen Then
            wz.ShowDialog()
        Else
            wz.Show()
        End If
        nachricht("FormularBestandStammdaten3")
    End Sub

    Private Sub btnIllegale_Click(sender As Object, e As RoutedEventArgs)
        nachricht("Dokumente")
        Dim wz As New IllegbauFilter(False)
        Dim modalOeffnen = False
        If modalOeffnen Then
            wz.ShowDialog()
        Else
            wz.Show()
        End If
        nachricht("FormularBestandStammdaten3")
    End Sub
End Class
