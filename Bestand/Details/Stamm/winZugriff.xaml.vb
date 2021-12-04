Partial Public Class winzugriff
    Private _modus$, layers$()
    Private Sub winZugriff_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        myGlobalz.availablePresentationObjects.Clear()

        If _modus$ = "kollegen" Then
            Dim a As New clsZugriffsinfo
            clsZugriffsinfo.kollegen(multicolumnComboBox)
            lblTitel.Content = "Zugriff erlaubt für:"
        End If
        If _modus$ = "standardworkflow_verlauf" Then
            Dim a As New clsZugriffsinfo
            clsZugriffsinfo.WorkflowDatatable_Verlauf(multicolumnComboBox)
            lblTitel.Content = "Der Verlauf des Standardworkflow für Sachgebiet: " & myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header
        End If


        If _modus$ = "standardworkflow_beteiligte" Then
            Dim a As New clsZugriffsinfo
            clsZugriffsinfo.WorkflowDatatable_Beteiligte(multicolumnComboBox)
            lblTitel.Content = "Die Beteiligten des Standardworkflow für Sachgebiet: " & myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header
        End If
        If _modus = "beteiligteEmails" Then
            Dim a As New clsZugriffsinfo
            clsZugriffsinfo.WorkflowDatatable_BeteiligteEmails(multicolumnComboBox)
            lblTitel.Content = "Die Beteiligten des Vorgangs: " & myGlobalz.sitzung.aktVorgangsID
        End If

        If _modus$ = "maplayer_referenced" Then
            layers$ = clstart.myc.kartengen.aktMap.Vgrund.Split(";"c)
            Dim a As New clsZugriffsinfo
            clsZugriffsinfo.kollegen(multicolumnComboBox)
            lblTitel.Content = "Zugriff erlaubt für:"
        End If
    End Sub

    Sub New(ByVal modus$)
        InitializeComponent()
        _modus$ = modus
    End Sub

    Private Shared Function KollegenAuswahlGetroffen() As String
        Dim sb As New System.Text.StringBuilder
        For Each ele As clsLayerListItem In myGlobalz.availablePresentationObjects
            If ele.IsChecked Then
                If myGlobalz.availablePresentationObjects.Count = 1 Then
                    sb.Append(String.Format("{0}", ele.Titel))
                Else
                    sb.Append(String.Format("{0};", ele.Titel))
                End If
            End If
        Next
        If sb.ToString.Length > 0 Then
            nachricht("Auswahl: " & sb.ToString)
            Return sb.ToString
        End If
        Return ""
    End Function

    Private Sub AuswahlHaendeln()
        If _modus = "kollegen" Then
            Dim result$ = KollegenAuswahlGetroffen()
            '   myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = result$
            CLstart.myc.AZauswahl.WeitereBearbeiter = result
        End If
        If _modus = "beteiligteEmails" Then
            beteiligteEmailshaendeln()
        End If
        ' Me.Close()
    End Sub
    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    AuswahlHaendeln()
    'End Sub

    'Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
    '    AuswahlHaendeln()
    'End Sub
    Private Shared Sub beteiligteEmailshaendeln()
        Try
            Dim sb As New System.Text.StringBuilder
            For Each ele As clsLayerListItem In myGlobalz.availablePresentationObjects
                If ele.IsChecked Then
                    If Not String.IsNullOrEmpty(ele.Name.Trim) Then
                        sb.Append(ele.Name & ";")
                    End If
                End If
            Next
            Dim test$ = sb.ToString
            If test.EndsWith(";") Then
                test = test.Substring(0, test.Length - 1)
            End If
            myGlobalz.TransferString = test
        Catch ex As Exception
            nachricht(String.Format("beteiligteEmailshaendeln {0}{1}", vbCrLf, ex))
        End Try
    End Sub





    Private Sub btnWeiter_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        e.Handled = True
        AuswahlHaendeln()
        Me.Close()
    End Sub
End Class
