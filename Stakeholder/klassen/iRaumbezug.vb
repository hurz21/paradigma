Public Interface iRaumbezug
    Property id() As Long
    Property typ() As RaumbezugsTyp
    Property name() As String
    Property box() As clsRange
    Property punkt() As myPoint
    Property abstract() As String
    Property SekID() As Long
    Property Status() As Integer
    Function PunktIsValid() As Boolean
End Interface


