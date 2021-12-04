'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Reflection

	NotInheritable Class AssemblyAttributes
		Private Sub New()
		End Sub

		Public Shared ReadOnly Property AssemblyTitle() As String
			Get
				Dim attributes As Object() = Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyTitleAttribute), False)
				If attributes.Length > 0 Then
					Dim titleAttribute As AssemblyTitleAttribute = DirectCast(attributes(0), AssemblyTitleAttribute)
					If titleAttribute.Title <> "" Then
						Return titleAttribute.Title
					End If
				End If
				Return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase)
			End Get
		End Property

		Public Shared ReadOnly Property AssemblyVersion() As Version
			Get
				Return Assembly.GetExecutingAssembly().GetName().Version
			End Get
		End Property

		Public Shared ReadOnly Property AssemblyDescription() As String
			Get
				Dim attributes As Object() = Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyDescriptionAttribute), False)
				If attributes.Length = 0 Then
					Return ""
				End If
				Return DirectCast(attributes(0), AssemblyDescriptionAttribute).Description
			End Get
		End Property

		Public Shared ReadOnly Property AssemblyProduct() As String
			Get
				Dim attributes As Object() = Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyProductAttribute), False)
				If attributes.Length = 0 Then
					Return ""
				End If
				Return DirectCast(attributes(0), AssemblyProductAttribute).Product
			End Get
		End Property

		Public Shared ReadOnly Property AssemblyCopyright() As String
			Get
				Dim attributes As Object() = Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(AssemblyCopyrightAttribute), False)
				If attributes.Length = 0 Then
					Return ""
				End If
				Return DirectCast(attributes(0), AssemblyCopyrightAttribute).Copyright
			End Get
		End Property

		Public Shared ReadOnly Property Is64BitAssembly() As Boolean
			Get
				Dim machine As ImageFileMachine
				Dim peKind As PortableExecutableKinds = 0

				Try
					Assembly.GetExecutingAssembly().ManifestModule.GetPEKind(peKind, machine)
				Catch
				End Try

				Return ((peKind And System.Reflection.PortableExecutableKinds.PE32Plus) <> 0)
			End Get
		End Property
	End Class
