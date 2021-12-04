Imports System.Data

Public Class miniMapControl
    Public featuretab As DataTable
    Private vorschlagLayers, bestandlayers, summeLayers As String()
    Private templayerlist As New clsLayerListItem
    Private Shared Sub displayObjekts(ByVal ctlList As System.Windows.Controls.ListBox)
        ctlList.ItemsSource = myGlobalz.layerListControlObjekts
        TextSearch.SetTextPath(ctlList, "Name")
    End Sub

    Public Sub getbestandlayers()
        bestandlayers = (clstart.myc.kartengen.aktMap.Vgrund & clstart.myc.kartengen.aktMap.Hgrund).Split(";"c)
        kickEmptyMembers(bestandlayers)
    End Sub

    Public Sub zeigeAlleRefenrenziertenLayersExtracted()
        getbestandlayers()
        summeLayers = addLayerArray(vorschlagLayers, bestandlayers)
        getListOfLayerDetails(summeLayers)
    End Sub
    Sub zeigeAlleRefenrenziertenLayers(ByVal ctlList As System.Windows.Controls.ListBox,
                                       ByVal allLayersChecked As Boolean?)
        zeigeAlleRefenrenziertenLayersExtracted()
        zeigeListeLayers(ctlList, allLayersChecked)
    End Sub

    Private Sub getListOfLayerDetails(ByVal layers$())
        Dim sql$ = MakeSqlStatement(layers)
        If sql.Length > 0 Then
            getLayerDatatable(sql)
        End If
    End Sub

    Public Shared Sub kickEmptyMembers(ByRef layers$())
        Try
            If layers.Count = 0 Then
                Exit Sub
            End If
            If String.IsNullOrEmpty(layers(layers.GetUpperBound(0))) Then
                If layers.Length > 0 Then
                    ReDim Preserve layers(layers.GetUpperBound(0) - 1)
                Else
                    nachricht_und_Mbox("kickEmptyMembers : Keine Layers ")
                End If
            End If
        Catch ex As Exception
            nachricht_und_Mbox("FEhler in kickEmptyMembers: " & ex.ToString)
        End Try
    End Sub

    Private Shared Function MakeSqlStatement(ByVal layers$()) As String
        Try
            If layers.Count < 1 Then Return ""
            kickEmptyMembers(layers)
            Dim sqlt$ = "SELECT c.featureclass,c.rank,c.titel,r.mitimagemap,d.titel as dokuTitel,r.mitimagemap " &
                " FROM featureclasses c,featurerange r, doku d " &
                " where c.status = 1 " &
                " and c.featureclass=r.featureclass  and d.featureclass=r.featureclass and ("
            Dim summe$ = ""
            For i = 0 To layers.GetUpperBound(0)
                If Not String.IsNullOrEmpty(layers(i)) Then
                    If i = layers.GetUpperBound(0) Then
                        summe = summe & " r.featureclass ='" & layers(i) & "' "
                    Else
                        summe = summe & " r.featureclass ='" & layers(i) & "' " & " or "
                    End If
                End If
            Next
            sqlt = sqlt & summe & ")"
            Return sqlt
        Catch ex As Exception
            nachricht_und_Mbox("FEhler in MakeSqlStatement: " & ex.ToString)
            Return "Fehler"
        End Try
    End Function

    Public Function getLayerDatatable(ByVal sql As String) As Boolean
        featuretab = clsMiniMapTools.getDT4anySQL_ALLEDB_dballe(sql, myGlobalz.sitzung.webgisrec)
    End Function

    Private Function getTitel4layer(ByVal layers As String) As String
        Dim a As String
        If featuretab Is Nothing Then
            Return layers
        End If
        Try
            layers = layers.ToLower
            For Each zeile As DataRow In featuretab.AsEnumerable
                a = zeile.Item("featureclass").ToString.ToLower
                If a = layers Then
                    Return zeile.Item("TITEL").ToString
                End If
            Next
            Return layers
        Catch ex As Exception
            nachricht("Fehler in getTitel4layer: " & ex.ToString)
            Return "fehler"
        End Try
    End Function

    Private Function getIstaktivierbar4layer(ByVal layers As String) As Boolean
        Dim a As String
        Try
            layers = layers.ToLower
            If featuretab.IsNothingOrEmpty Then
                Return False
            End If
            For Each zeile As DataRow In featuretab.AsEnumerable
                a = zeile.Item("featureclass").ToString.ToLower
                If a = layers Then
                    Return CBool(zeile.Item("MITIMAGEMAP"))
                End If
            Next
            'kein treffer
            Return False
        Catch ex As Exception
            nachricht("Fehler in getdokuTitel4layer: " & ex.ToString)
            Return False
        End Try
    End Function

    Private Function getdokuTitel4layer(ByVal layers As String) As String
        Dim a As String
        If featuretab Is Nothing Then
            Return layers
        End If
        Try
            layers = layers.ToLower
            For Each zeile As DataRow In featuretab.AsEnumerable
                a = zeile.Item("featureclass").ToString.ToLower
                If a = layers Then
                    Return zeile.Item("DOKUTITEL").ToString
                End If
            Next
            'kein treffer
            Return layers
        Catch ex As Exception
            nachricht("Fehler in getdokuTitel4layer: " & ex.ToString)
            Return layers
        End Try
    End Function

    Public Shared Function istFilterOk(ByVal p1 As String) As Boolean
        If String.IsNullOrEmpty(p1) Then Return False
        Return True
    End Function

    Public Function getFilteredLayers(ByVal filterstring As String, ByVal ctlList As System.Windows.Controls.ListBox,
                                       ByVal allLayersChecked As Boolean?) As Boolean
        Dim sql As String = MakeFilterSqlStatement(filterstring, " and ")
        Try
            If sql.Length > 0 Then
                '  getLayerDatatable(sql)
                featuretab = clsMiniMapTools.getDT4anySQL_ALLEDB_dballe(sql, myGlobalz.sitzung.webgisREC)
            End If
            vorschlagLayers = getLayerArrayFromDT(featuretab)
            If vorschlagLayers Is Nothing Then
                Return False 'MsgBox("Nichts gefunden!")
            Else
                '   zeigefilterebenen(ctlList, allLayersChecked)
                Return True
            End If
        Catch ex As Exception
            nachricht("Fehler in getFilteredLayers: " & ex.ToString)
            Return False
        End Try
    End Function

    'Public Sub zeigefilterebenen(ByVal ctlList As System.Windows.Controls.ListBox, ByVal allLayersChecked As Boolean?)
    '    getbestandlayers()
    '    '  ReDim layers(0)
    '    kickEmptyMembers(bestandlayers)
    '    '   Array.Copy(bestandlayers, 0, layers, 0, bestandlayers.Count)
    '    zeigeListeLayers(ctlList, allLayersChecked, temp)
    'End Sub
    Public Function MakeFilterSqlStatement(ByVal filterstring As String, ByVal andOr As String) As String
        Try
            Dim filters As String() = Nothing
            Dim erfolg As Boolean = buildFilterarray(filterstring, filters)
            If filters.Count < 1 Then Return ""
            kickEmptyMembers(filters)
            Dim sqlt As String = "SELECT c.featureclass,c.titel,d.titel as dokutitel,r.mitimagemap FROM featureclasses c,featurerange r, doku d " &
                " where c.status = 1 " &
                " and c.featureclass=r.featureclass and d.featureclass=r.featureclass and "
            Dim summe$ = ""
            If filters.Count = 1 Then
                If Not String.IsNullOrEmpty(filters(0)) Then
                    summe = summe & " ((c.featureclass like '%" & filters(0) & "%') or "
                    summe = summe & "(c.titel like '%" & filters(0) & "%') " & "  "
                    summe = summe & " or "
                    summe = summe & " ((d.schlagwoerter like '%" & filters(0) & "%') or "
                    summe = summe & "(d.titel like '%" & filters(0) & "%') " & " ))"

                End If
            End If
            If filters.Count > 1 Then
                summe = New String("("c, 1)

                'filters.Count * "("
                For i = 0 To filters.GetUpperBound(0)
                    If Not String.IsNullOrEmpty(filters(i)) Then
                        If i > 0 Then summe = summe & andOr & "("
                        If i <= filters.GetUpperBound(0) Then
                            summe = summe & " c.featureclass like '%" & filters(i) & "%'  or "
                            summe = summe & " d.titel like '%" & filters(i) & "%'  or "
                            summe = summe & " d.schlagwoerter like '%" & filters(i) & "%'  or "
                            summe = summe & "c.titel like '%" & filters(i) & "%') " '& " ) "
                        End If
                    End If
                Next
                ' summe = summe & New String(")"c, 1)
            End If
            sqlt = sqlt & summe
            nachricht("MakeSqlStatement: sql=" & sqlt)
            Return sqlt
        Catch ex As Exception
            nachricht_und_Mbox("FEhler in MakeSqlStatement: " & ex.ToString)
            Return "Fehler"
        End Try
    End Function

    Private Function buildFilterarray(ByVal filterstring As String, ByRef filters As String()) As Boolean
        If filterstring.Trim.Contains(" ") Then
            'mehrere filtertexte
            filters = filterstring.Split(" "c)
        Else
            ReDim filters(0)
            filters(0) = filterstring.Trim
        End If
    End Function

    Public Sub zeigeListeLayers(ByVal ctlList As System.Windows.Controls.ListBox, ByVal allLayersChecked As Boolean?)
        myGlobalz.layerListControlObjekts.Clear()
        ctlList.ItemsSource = Nothing
        Dim anzahlVorschlaege As Integer = vorschlagsliste(allLayersChecked, vorschlagLayers, False)
        Dim anzahlbestand As Integer = vorschlagsliste(allLayersChecked, bestandlayers, True)
        displayObjekts(ctlList)
    End Sub

    Private Function vorschlagsliste(ByVal allLayersChecked As Boolean?, ByVal layerarray As String(), ByVal ischechekd As Boolean) As Integer
        If layerarray Is Nothing Then Return 0
        Dim temp As clsLayerListItem
        For i = 0 To layerarray.Count - 1
            temp = New clsLayerListItem
            temp.Id = CInt((i))
            temp.Name = getTitel4layer(layerarray(i))
            temp.IsChecked = ischechekd
            temp.dokuTitel = getdokuTitel4layer(layerarray(i))
            temp.istAktivierbar = CBool(getIstaktivierbar4layer(layerarray(i)))
            temp.Titel = CStr(layerarray(i))
            If layerarray(i) = clstart.myc.kartengen.aktMap.ActiveLayer Then
                temp.istAktiveEbene = True
            Else
                temp.istAktiveEbene = False
            End If
            If Not String.IsNullOrEmpty(temp.Name) Then myGlobalz.layerListControlObjekts.Add(temp)
        Next
        Return layerarray.GetUpperBound(0)
    End Function


    'Private Function vorschlagslisteALT(ByVal allLayersChecked As Boolean?) As Integer
    '    If vorschlagLayers Is Nothing Then Return 0
    '    Dim temp As clsLayerListItem
    '    For i = 0 To vorschlagLayers.GetUpperBound(0)
    '        temp = New clsLayerListItem
    '        temp.Id = CInt((i))
    '        temp.Name = getTitel4layer(vorschlagLayers(i))
    '        temp.IsChecked = False
    '        temp.dokuTitel = getdokuTitel4layer(vorschlagLayers(i))
    '        temp.istAktivierbar = CBool(getIstaktivierbar4layer(vorschlagLayers(i)))
    '        temp.IsChecked = False
    '        temp.Titel = CStr(vorschlagLayers(i))
    '        If vorschlagLayers(i) = clstart.myc.kartengen.aktMap.ActiveLayer Then
    '            temp.istAktiveEbene = True
    '        Else
    '            temp.istAktiveEbene = False
    '        End If
    '        If Not String.IsNullOrEmpty(temp.Name) Then myGlobalz.layerListControlObjekts.Add(temp)
    '    Next
    '    Return vorschlagLayers.GetUpperBound(0)
    'End Function

    'Private Function bestandssliste(ByVal allLayersChecked As Boolean?) As Integer
    '    If summeLayers Is Nothing Then
    '        Return 0
    '    End If
    '    Dim temp As clsLayerListItem
    '    For i = 0 To bestandlayers.GetUpperBound(0)
    '        temp = New clsLayerListItem
    '        temp.Id = CInt((i))
    '        temp.Name = getTitel4layer(bestandlayers(i)) ' "Titel " & i
    '        temp.IsChecked = False
    '        temp.dokuTitel = getdokuTitel4layer(bestandlayers(i)) ' "Titel " & i

    '        temp.istAktivierbar = CBool(getIstaktivierbar4layer(bestandlayers(i))) ' "Titel " & i
    '        temp.IsChecked = True
    '        temp.Titel = CStr(bestandlayers(i))
    '        If bestandlayers(i) = clstart.myc.kartengen.aktMap.ActiveLayer Then
    '            temp.istAktiveEbene = True
    '        Else
    '            temp.istAktiveEbene = False
    '        End If
    '        If Not String.IsNullOrEmpty(temp.Name) Then myGlobalz.layerListControlObjekts.Add(temp)
    '    Next
    '    Return bestandlayers.GetUpperBound(0)
    'End Function
    Private Function getLayerArrayFromDT(ByVal featuretab As DataTable) As String()
        Dim layers As String() = Nothing
        Dim count As Integer = -1
        If featuretab.IsNothingOrEmpty Then
            Return Nothing
        End If
        Try
            For Each zeile As DataRow In featuretab.AsEnumerable
                count += 1
                ReDim Preserve layers(count)
                layers(count) = zeile.Item("featureclass").ToString
            Next
            Return layers
        Catch ex As Exception
            nachricht("FEhler in getLayerFromDT:" & ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Function addLayerArray(ByVal vorschlagLayers As String(), ByVal bestandlayers As String()) As String()
        Dim summe As String() = Nothing, i As Integer = 0
        If Not vorschlagLayers Is Nothing Then
            If Not bestandlayers Is Nothing Then
                For i = 0 To bestandlayers.Count - 1
                    ReDim Preserve summe(i)
                    summe(i) = bestandlayers(i)
                Next
                Dim t As Integer = i
                For i = 0 To vorschlagLayers.Count - 1
                    ReDim Preserve summe(i + t)
                    summe(i + t) = vorschlagLayers(i)
                Next
                Return summe
            Else
                Return vorschlagLayers
            End If
        Else
            If Not bestandlayers Is Nothing Then
                Return bestandlayers
            Else
                Return Nothing
            End If
        End If
    End Function

End Class
