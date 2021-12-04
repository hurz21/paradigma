Imports System.Data
Public Interface IFeuerwehrPlanStandort
	Function create(ByVal pl As clsStandortPlan) As Integer
	Function update(ByVal pl As clsStandortPlan) As Integer
	Function delete(ByVal pl As clsStandortPlan) As Integer
	Function getPlan(ByVal id%) As clsStandortPlan
	Function getPlaene(ByVal SQL$) As DataTable
End Interface