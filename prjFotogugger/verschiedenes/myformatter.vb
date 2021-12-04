


Public Class MyFormatter
    Implements IValueConverter
    Public Function Convert(ByVal value As Object,
                 ByVal targetType As System.Type,
                 ByVal parameter As Object,
                 ByVal culture As System.Globalization.CultureInfo) As Object _
                 Implements System.Windows.Data.IValueConverter.Convert
        If parameter IsNot Nothing Then
            If TypeOf value Is Date Then
                If CType(value, Date) = #1:01:01 AM# Then
                    Return ""
                End If
            End If
            If value Is Nothing Then Return ""
            Return Format(value, parameter.ToString())
        End If
        Return value
    End Function

    Public Function ConvertBack(ByVal value As Object,
                ByVal targetType As System.Type,
                ByVal parameter As Object,
                ByVal culture As System.Globalization.CultureInfo) As Object _
                Implements System.Windows.Data.IValueConverter.ConvertBack

        If targetType Is GetType(Date) OrElse targetType Is GetType(Nullable(Of Date)) Then
            If IsDate(value) Then
                Return CDate(value)
            ElseIf value.ToString() = "" Then
                Return Nothing
            Else
                Return Now() 'invalid type was entered so just give a default.
            End If
        ElseIf targetType Is GetType(Decimal) Then
            If IsNumeric(value) Then
                Return CDec(value)
            Else
                Return 0
            End If
        End If
        Return value
    End Function
End Class

