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
        Me.Top = CLstart.formposition.setPosition("diverse", "winKOOKIEositiontop", Me.Top)
        Me.Left = CLstart.formposition.setPosition("diverse", "winKOOKIEpositionleft", Me.Left)
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
End Class
