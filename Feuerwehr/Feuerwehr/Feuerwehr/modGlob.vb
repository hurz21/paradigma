Module modGlob
	Public dbcrudpl As New clsStandortPlanCRUD
	Public dbcrudMedium As New clsMediumCRUD
	Private _aktStandort As clsStandortPlan
	Public Property aktStandort() As clsStandortPlan
		Get
			Return _aktStandort
		End Get
		Set(ByVal Value As clsStandortPlan)
			_aktStandort = Value
		End Set
	End Property
	Private _aktMedium As clsMedium
	Public Property aktMedium() As clsMedium
		Get
			Return _aktMedium
		End Get
		Set(ByVal Value As clsMedium)
			_aktMedium = Value
		End Set
	End Property
	Sub starteAnwendung()
		aktStandort = New clsStandortPlan(dbcrudpl)
		aktMedium = New clsMedium(dbcrudMedium)
	End Sub

End Module
