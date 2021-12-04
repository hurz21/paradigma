Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Feuerwehr



'''<summary>
'''Dies ist eine Testklasse für "clsGoogleMeinTest" und soll
'''alle clsGoogleMeinTest Komponententests enthalten.
'''</summary>
<TestClass()> _
Public Class clsGoogleMeinTest


	Private testContextInstance As TestContext

	'''<summary>
	'''Ruft den Testkontext auf, der Informationen
	'''über und Funktionalität für den aktuellen Testlauf bietet, oder legt diesen fest.
	'''</summary>
	Public Property TestContext() As TestContext
		Get
			Return testContextInstance
		End Get
		Set(ByVal value As TestContext)
			testContextInstance = value
		End Set
	End Property

#Region "Zusätzliche Testattribute"
	'

	'
#End Region


	'''<summary>
	'''Ein Test für "Googleadress"
	'''</summary>
	<TestMethod()> _
	Public Sub GoogleadressTest()
		Dim target As clsGoogleMein = New clsGoogleMein	' TODO: Passenden Wert initialisieren
		Dim instreet As String = String.Empty	' TODO: Passenden Wert initialisieren
		Dim incity As String = String.Empty	' TODO: Passenden Wert initialisieren
		Dim instate As String = String.Empty ' TODO: Passenden Wert initialisieren
		Dim inzip As String = String.Empty ' TODO: Passenden Wert initialisieren
		Dim expected As String = "Fehler"	' TODO: Passenden Wert initialisieren
		Dim actual As String
		actual = target.Googleadress(instreet, incity, instate, inzip)
		actual = actual.Substring(0, 6)
		'	Assert.(expected, actual)
		'	Assert.Inconclusive("Überprüfen Sie die Richtigkeit dieser Testmethode.")
	End Sub
End Class
