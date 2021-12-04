Class Window1

	Private Sub neuaufnahme(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		aktStandort.clear()
		glob.editStandort(0)
	End Sub

	Private Sub suchen(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		Dim su As New WINsuchen
		su.ShowDialog()
	End Sub

	Private Sub abbruch(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
		Me.Close()
	End Sub

	Private Sub Window1_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
		If glob.KONFIGURATOR() Then
			starteAnwendung()
		Else
			MessageBox.Show("Initialisierung gescheitert!" & vbCrLf & "Bitte wenden Sie sich an den Hersteller: j.feinen@gmx.net" & vbCrLf & "Programm wird beendet !!!")
			End
		End If
	End Sub
End Class
