Imports System.Data

Imports Microsoft.VisualStudio.TestTools.UnitTesting

Imports Feuerwehr



'''<summary>
'''Dies ist eine Testklasse für "globTest" und soll
'''alle globTest Komponententests enthalten.
'''</summary>
<TestClass()> _
Public Class globTest


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
	'''Ein Test für "OpenDocument"
	'''</summary>
	<TestMethod()> _
	Public Sub OpenDocumentTest()
		Dim DocumentFile As String = String.Empty	' TODO: Passenden Wert initialisieren
		Dim expected As Boolean = False	' TODO: Passenden Wert initialisieren
		Dim actual As Boolean
		actual = glob.OpenDocument(DocumentFile)
		Assert.AreEqual(expected, actual)
		'	Assert.Inconclusive("Überprüfen Sie die Richtigkeit dieser Testmethode.")
	End Sub
	'''<summary>
	'''Ein Test für "OpenDocument"
	'''</summary>
	<TestMethod()> _
	Public Sub OpenDocumentTestMitText()
		Dim DocumentFile As String = "fehlt" ' TODO: Passenden Wert initialisieren
		Dim expected As Boolean = False	' TODO: Passenden Wert initialisieren
		Dim actual As Boolean
		actual = glob.OpenDocument(DocumentFile)
		Assert.AreEqual(expected, actual)
		'	Assert.Inconclusive("Überprüfen Sie die Richtigkeit dieser Testmethode.")
	End Sub

	'''<summary>
	'''Ein Test für "googlemaps"
	'''</summary>
	<TestMethod()> _
	Public Sub googlemapsTest()
		Dim item As DataRowView = Nothing	' TODO: Passenden Wert initialisieren
		Dim expected As String = String.Empty	' TODO: Passenden Wert initialisieren
		Dim actual As String
		actual = glob.googlemaps(item)
		Assert.AreEqual(expected, actual)
		Assert.Inconclusive("Überprüfen Sie die Richtigkeit dieser Testmethode.")
	End Sub
End Class
