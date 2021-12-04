Imports System.Data
Public Interface IWiedervorlageCRUD
    Function create(ByVal wv As clsWiedervorlage) As Integer
    Function update(ByVal wv As clsWiedervorlage) As Integer
    Function getWV(ByVal id As Integer) As clsWiedervorlage
    Function getWVs(ByVal SQL As String) As DataTable
End Interface