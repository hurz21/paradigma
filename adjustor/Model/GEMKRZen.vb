Imports System.Collections.ObjectModel


Public Class GEMKRZen
    Inherits ObservableCollection(Of GEMKRZ)

    Public Shared Function LoadVariablen() As GEMKRZen
        Try
            Dim VariablenCollection As New GEMKRZen
            Dim doc = XDocument.Load(Application.fullpath) '"Data\detail_ereignisseTitel.xml")
            Dim query = From cust In doc...<GEMKRZ>
                      Select New GEMKRZ With {.Name = cust.@Name, .ID = cust.@ID, .GEME = cust.@GEME, .GEMA = cust.@GEMA}
            For Each cust In query
                VariablenCollection.Add(cust)
            Next
            Return VariablenCollection
        Catch ex As Exception
            MsgBox(ex.ToString)
            Return Nothing
        End Try
    End Function
End Class