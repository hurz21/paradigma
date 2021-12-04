Public Interface iRaumbezug
    Property id() As Long
    Property typ() As RaumbezugsTyp
    Property name() As String
    Property box() As LibGISmapgenerator.clsRange
    Property punkt() As LibGISmapgenerator.myPoint
    Property abstract() As String
    Property SekID() As Long
    Property Status() As Integer
    Property Freitext As String
    Function PunktIsValid() As Boolean
End Interface


