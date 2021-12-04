Imports System.Data
Public Interface IWiedervorlageCRUD
	Function create(ByVal wv As clsWiedervorlage) As Integer
    Function update(ByVal wv As clsWiedervorlage) As Integer
    Function getWV(ByVal id as integer) as  clsWiedervorlage
	Function getWVs(ByVal SQL as string) as  DataTable
End Interface