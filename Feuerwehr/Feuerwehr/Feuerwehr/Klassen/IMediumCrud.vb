Imports System.Data

Public Interface IMediumCrud
	Function Medium_create(ByVal pl As clsMedium) As Integer
	Function Medium_update(ByVal pl As clsMedium) As Integer
	Function Medium_delete(ByVal pl As clsMedium) As Integer
	Function getMedium(ByVal id%) As clsMedium
	Function getMedien(ByVal SQL$) As DataTable
End Interface
