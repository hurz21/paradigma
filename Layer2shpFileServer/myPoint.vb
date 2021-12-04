'Imports System.Drawing
''' <summary>
''' x,y, as double
''' </summary>
''' <remarks></remarks>
Public Class myPoint
    Public Property X() As Double
    Public Property Y() As Double
    Overrides Function toString() As String
        Return String.Format("{0}, {1}", X, Y)
    End Function

End Class
