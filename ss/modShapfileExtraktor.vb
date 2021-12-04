Module modShapfileExtraktor
    Public Property shapeobject As MapWinGIS.Shape
    Public Property serializedShape As String
    Public Property aktShapeType As String
    Function getSerialFromShapeOHneDLL(ByVal shapefile As String,
                                            ByVal Key As String,
                                            ByVal Value As String,
                                            ByRef serial As String,
                                            ByRef area As Double) As Boolean
        Try
            l("getSerialFromShapeOHneDLL---------------------------------")

            Dim erfolg As Boolean = getSerialFromShape(shapefile, Key, Value) '"D:\fkatbig\alk\shape\data\basis.shp", CStr(3), "FS0607490100004204400")
            If erfolg Then
                'td.getShapeFromSerial(td.serializedShape)
                serial = serializedShape
                area = area
                l("getSerialFromShapeOHneDLL true")
                Return True
            Else
                '  MsgBox("fehler")
                serial = ""
                l("getSerialFromShapeOHneDLL false")
                Return False
            End If
            l("getSerialFromShapeOHneDLL?????????????")
        Catch ex As Exception
            l("1Fehler in getSerialFromShape:" & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function
    ''' <param name="SearchKey">spaltennummer im shapefile , 0-basiert</param>
    ''' <param name="searchValue">string</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function getSerialFromShape(ByVal Shapefile As String,
                                       ByVal SearchKey As String, ByRef searchValue As String) As Boolean
        Dim erroroout As String = "ferroroout"
        Try
            l("getSerialFromShape-----------------------")
            l("0")
            Dim sf As New MapWinGIS.Shapefile
            l("1")
            Dim res As Boolean
            Dim copyField As Integer
            Dim copyValue As Object
            l("1")
            erroroout = erroroout & ",Shapefile : " & Shapefile
            erroroout = erroroout & ",SearchKey : " & SearchKey
            erroroout = erroroout & ",searchValue : " & searchValue
            l(erroroout)

            copyField = CInt(SearchKey)
            copyValue = searchValue
            res = sf.Open(Shapefile)

            erroroout = erroroout & ", Shapefile open : "
            l(erroroout)
            If res = False Then
                erroroout = erroroout & ",res"
                l(erroroout)
                serializedShape = String.Format("Fehler beim öffnen der INputshpdatei:{0}{1}{2}", Shapefile, vbCrLf, sf.ErrorMsg(sf.LastErrorCode))
                Return False
            End If
            Dim returncode As Boolean = False
            serializedShape = ""
            erroroout = erroroout & ", vor cycleShape"
            l(erroroout)
            cycleShape(sf, copyField, copyValue, returncode, area)
            aktShapeType = CStr(sf.ShapefileType)
            erroroout = erroroout & ", aktShapeType: " & aktShapeType
            l(erroroout)
            'Dim extends As MapWinGIS.Extents = sf.Extents
            'Dim a = extends.mMax
            sf.Close()
            Return returncode
        Catch ex As Exception
            l(String.Format("2Fehler in getSerialFromShape: erroroout {0}{1}{2}", erroroout, vbCrLf, ex))
            Return False
        End Try
    End Function
    Private Sub cycleShape(ByVal sf As MapWinGIS.Shapefile,
                              ByVal copyField As Integer, ByVal copyValue As Object,
                              ByRef returncode As Boolean, ByRef area As Double)
        Try
            l("cycleShape-----------------------")
            Dim shp As Integer
            Dim test As Object
            returncode = False
            For shp = 0 To sf.NumShapes - 1
                ' Dim a$ = sf.CellValue(copyField, shp)
                If sf.CellValue(copyField, shp) = copyValue Then
                    test = sf.CellValue(copyField, shp)
                    shapeobject = sf.Shape(shp)
                    serializedShape = shapeobject.SerializeToString()
                    area = shapeobject.Area
                    ' Dim envelope = shapeobject.Centroid
                    returncode = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            l("Fehler" & String.Format("Fehler in cycleShape: {0}{1}", vbCrLf, ex))
            returncode = False
        End Try
    End Sub

    Function ShowError(ByVal Message As String, ByVal Shape As Integer) As String
        'This function shows the error, and returns true if they decide to abort
        Return (String.Format("Fehler: Serializing Error. The following error occured on shape {0}.{1}{2}", Shape, vbNewLine, Message))
    End Function
End Module
