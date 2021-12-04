Imports System.Collections.ObjectModel

Public Class Variablen
    Inherits ObservableCollection(Of Variable)

    Public Shared Function LoadVariablen() As Variablen
        Try
            Dim VariablenCollection As New Variablen
            Dim doc = XDocument.Load(Application.fullpath) '"Data\detail_ereignisseTitel.xml")
            Dim query = From cust In doc...<Variable>
                      Select New Variable With {.Name = cust.@Name, .ID = cust.@ID}
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
