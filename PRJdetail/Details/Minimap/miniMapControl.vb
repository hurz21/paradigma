Imports System.Data
Imports paradigmaDetail

Public Class miniMapControl
    Public featuretab As DataTable
    Private vorschlagLayers, bestandlayers, summeLayers As String()
    Private templayerlist As New clsLayerListItem


    'Private Function getTitel4layer(ByVal layers As String) As String
    '    Dim a As String
    '    If featuretab Is Nothing Then
    '        Return layers
    '    End If
    '    Try
    '        layers = layers.ToLower
    '        For Each zeile As DataRow In featuretab.AsEnumerable
    '            a = zeile.Item("featureclass").ToString.ToLower
    '            If a = layers Then
    '                Return zeile.Item("TITEL").ToString
    '            End If
    '        Next
    '        Return layers
    '    Catch ex As Exception
    '        nachricht("Fehler in getTitel4layer: " ,ex)
    '        Return "fehler"
    '    End Try
    'End Function

    'Private Function getIstaktivierbar4layer(ByVal layers As String) As Boolean
    '    Dim a As String
    '    Try
    '        layers = layers.ToLower
    '        If featuretab.IsNothingOrEmpty Then
    '            Return False
    '        End If
    '        For Each zeile As DataRow In featuretab.AsEnumerable
    '            a = zeile.Item("featureclass").ToString.ToLower
    '            If a = layers Then
    '                Return CBool(zeile.Item("MITIMAGEMAP"))
    '            End If
    '        Next
    '        'kein treffer
    '        Return False
    '    Catch ex As Exception
    '        nachricht("Fehler in getdokuTitel4layer: " ,ex)
    '        Return False
    '    End Try
    'End Function

    'Private Function getdokuTitel4layer(ByVal layers As String) As String
    '    Dim a As String
    '    If featuretab Is Nothing Then
    '        Return layers
    '    End If
    '    Try
    '        layers = layers.ToLower
    '        For Each zeile As DataRow In featuretab.AsEnumerable
    '            a = zeile.Item("featureclass").ToString.ToLower
    '            If a = layers Then
    '                Return zeile.Item("DOKUTITEL").ToString
    '            End If
    '        Next
    '        'kein treffer
    '        Return layers
    '    Catch ex As Exception
    '        nachricht("Fehler in getdokuTitel4layer: " ,ex)
    '        Return layers
    '    End Try
    'End Function

    'Public Shared Function istFilterOk(ByVal p1 As String) As Boolean
    '    If String.IsNullOrEmpty(p1) Then Return False
    '    Return True
    'End Function

    'Public Function getFilteredLayers(ByVal filterstring As String, ByVal ctlList As System.Windows.Controls.ListBox,
    '                                   ByVal allLayersChecked As Boolean?) As Boolean
    '    Dim sql As String = MakeFilterSqlStatement(filterstring, " and ")
    '    Try
    '        If sql.Length > 0 Then
    '            '  getLayerDatatable(sql)
    '            featuretab = clsMiniMapTools.getDT4anySQL_ALLEDB_dballe(sql, myglobalz.sitzung.webgisREC)
    '        End If
    '        vorschlagLayers = getLayerArrayFromDT(featuretab)
    '        If vorschlagLayers Is Nothing Then
    '            Return False
    '        Else
    '            '   zeigefilterebenen(ctlList, allLayersChecked)
    '            Return True
    '        End If
    '    Catch ex As Exception
    '        nachricht("Fehler in getFilteredLayers: " ,ex)
    '        Return False
    '    End Try
    'End Function

    'Public Sub zeigefilterebenen(ByVal ctlList As System.Windows.Controls.ListBox, ByVal allLayersChecked As Boolean?)
    '    getbestandlayers()
    '    '  ReDim layers(0)
    '    kickEmptyMembers(bestandlayers)
    '    '   Array.Copy(bestandlayers, 0, layers, 0, bestandlayers.Count)
    '    zeigeListeLayers(ctlList, allLayersChecked, temp)
    'End Sub
    'Public Function MakeFilterSqlStatement(ByVal filterstring As String, ByVal andOr As String) As String
    '    Try
    '        Dim filters As String() = Nothing
    '        Dim erfolg As Boolean = buildFilterarray(filterstring, filters)
    '        If filters.Count < 1 Then Return ""
    '        kickEmptyMembers(filters)
    '        Dim sqlt As String = "SELECT c.featureclass,c.titel,d.titel as dokutitel,r.mitimagemap FROM featureclasses c,featurerange r, doku d " &
    '            " where c.status = 1 " &
    '            " and c.featureclass=r.featureclass and d.featureclass=r.featureclass and "
    '        Dim summe$ = ""
    '        If filters.Count = 1 Then
    '            If Not String.IsNullOrEmpty(filters(0)) Then
    '                summe = summe & " ((c.featureclass like '%" & filters(0) & "%') or "
    '                summe = summe & "(c.titel like '%" & filters(0) & "%') " & "  "
    '                summe = summe & " or "
    '                summe = summe & " ((d.schlagwoerter like '%" & filters(0) & "%') or "
    '                summe = summe & "(d.titel like '%" & filters(0) & "%') " & " ))"

    '            End If
    '        End If
    '        If filters.Count > 1 Then
    '            summe = New String("("c, 1)
    '            For i = 0 To filters.GetUpperBound(0)
    '                If Not String.IsNullOrEmpty(filters(i)) Then
    '                    If i > 0 Then summe = summe & andOr & "("
    '                    If i <= filters.GetUpperBound(0) Then
    '                        summe = summe & " c.featureclass like '%" & filters(i) & "%'  or "
    '                        summe = summe & " d.titel like '%" & filters(i) & "%'  or "
    '                        summe = summe & " d.schlagwoerter like '%" & filters(i) & "%'  or "
    '                        summe = summe & "c.titel like '%" & filters(i) & "%') " '& " ) "
    '                    End If
    '                End If
    '            Next
    '        End If
    '        sqlt = sqlt & summe
    '        nachricht("MakeSqlStatement: sql=" & sqlt)
    '        Return sqlt
    '    Catch ex As Exception
    '        nachricht_und_Mbox("FEhler in MakeSqlStatement: " ,ex)
    '        Return "Fehler"
    '    End Try
    'End Function

    'Private Function buildFilterarray(ByVal filterstring As String, ByRef filters As String()) As Boolean
    '    If filterstring.Trim.Contains(" ") Then
    '        'mehrere filtertexte
    '        filters = filterstring.Split(" "c)
    '    Else
    '        ReDim filters(0)
    '        filters(0) = filterstring.Trim
    '    End If
    '    Return True
    'End Function

    'Public Sub zeigeListeLayers(ByVal ctlList As System.Windows.Controls.ListBox, ByVal allLayersChecked As Boolean?)
    '    myglobalz.layerListControlObjekts.Clear()
    '    ctlList.ItemsSource = Nothing
    '    Dim anzahlVorschlaege As Integer = vorschlagsliste(allLayersChecked, vorschlagLayers, False)
    '    Dim anzahlbestand As Integer = vorschlagsliste(allLayersChecked, bestandlayers, True)
    '    displayObjekts(ctlList)
    'End Sub

    'Private Function vorschlagsliste(ByVal allLayersChecked As Boolean?, ByVal layerarray As String(), ByVal ischechekd As Boolean) As Integer
    '    If layerarray Is Nothing Then Return 0
    '    Dim temp As clsLayerListItem
    '    For i = 0 To layerarray.Count - 1
    '        temp = New clsLayerListItem
    '        temp.Id = CInt((i))
    '        temp.Name = getTitel4layer(layerarray(i))
    '        temp.IsChecked = ischechekd
    '        temp.dokuTitel = getdokuTitel4layer(layerarray(i))
    '        temp.istAktivierbar = CBool(getIstaktivierbar4layer(layerarray(i)))
    '        temp.Titel = CStr(layerarray(i))
    '        If layerarray(i) = CLstart.myc.kartengen.aktMap.ActiveLayer Then
    '            temp.istAktiveEbene = True
    '        Else
    '            temp.istAktiveEbene = False
    '        End If
    '        If Not String.IsNullOrEmpty(temp.Name) Then myglobalz.layerListControlObjekts.Add(temp)
    '    Next
    '    Return layerarray.GetUpperBound(0)
    'End Function

    'Private Function getLayerArrayFromDT(ByVal featuretab As DataTable) As String()
    '    Dim layers As String() = Nothing
    '    Dim count As Integer = -1
    '    If featuretab.IsNothingOrEmpty Then
    '        Return Nothing
    '    End If
    '    Try
    '        For Each zeile As DataRow In featuretab.AsEnumerable
    '            count += 1
    '            ReDim Preserve layers(count)
    '            layers(count) = zeile.Item("featureclass").ToString
    '        Next
    '        Return layers
    '    Catch ex As Exception
    '        nachricht("FEhler in getLayerFromDT:" ,ex)
    '        Return Nothing
    '    End Try
    'End Function

    'Private Function addLayerArray(ByVal vorschlagLayers As String(), ByVal bestandlayers As String()) As String()
    '    Dim summe As String() = Nothing, i As Integer = 0
    '    If Not vorschlagLayers Is Nothing Then
    '        If Not bestandlayers Is Nothing Then
    '            For i = 0 To bestandlayers.Count - 1
    '                ReDim Preserve summe(i)
    '                summe(i) = bestandlayers(i)
    '            Next
    '            Dim t As Integer = i
    '            For i = 0 To vorschlagLayers.Count - 1
    '                ReDim Preserve summe(i + t)
    '                summe(i + t) = vorschlagLayers(i)
    '            Next
    '            Return summe
    '        Else
    '            Return vorschlagLayers
    '        End If
    '    Else
    '        If Not bestandlayers Is Nothing Then
    '            Return bestandlayers
    '        Else
    '            Return Nothing
    '        End If
    '    End If
    'End Function
    'Public Function getFilteredLayers2(kategorie As String, VGRUNDtemp As String) As List(Of presFeatureClass)
    '    'setzeAbteilungsThemen

    '    'die kategorie anhand setzeAbteilungsThemen festlegen
    '    If kategorie = String.Empty Then
    '        kategorie = holeAbteilungsKategorie(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Bemerkung.Trim)
    '    End If
    '    Dim sql As String = MakelisteSqlStatement(kategorie)
    '    Dim liste As New List(Of presFeatureClass)
    '    Try
    '        If sql.Length > 0 Then
    '            featuretab = clsMiniMapTools.getDT4anySQL_ALLEDB_dballe(sql, myGlobalz.sitzung.webgisREC)
    '            liste = featuretab2liste(featuretab)
    '            liste = BestandEntfernenen(liste, VGRUNDtemp)
    '            Return liste
    '        End If
    '        Return Nothing
    '    Catch ex As Exception
    '        nachricht("Fehler in getFilteredLayers: " ,ex)
    '        Return Nothing
    '    End Try
    'End Function

    'Private Function holeAbteilungsKategorie(abteilung As String) As String
    '    Select Case abteilung 'myGlobalz.sitzung.aktVorgang.Stammdaten.Bearbeiter.Bemerkung.Trim
    '        Case "Immisionsschutz", "Immissionsschutz"
    '            Return "Immissionsschutz"
    '        Case "Untere Naturschutzbehörde"
    '            Return "natur"
    '        Case "Untere Naturschutzbehörde/IB"
    '            Return "natur"
    '        Case "Untere Wasserbehörde"
    '            Return "uwb"
    '        Case "Schornsteinfegerwesen"
    '            Return "Immissionsschutz"
    '        Case "Graphische Datenverarbeitung"
    '            Return "natur"
    '        Case "Assistenz"
    '            Return "natur"
    '        Case "Stv. Fachdienstleitung", "Fachdienstleitung"
    '            Return "natur"
    '        Case Else
    '            Return "natur"
    '    End Select
    'End Function

    'Private Function MakelisteSqlStatement(kategorie As String) As String
    '    Dim sql As String
    '    If kategorie = String.Empty Then
    '        sql =
    '                "  SELECT distinct c.titel,c.featureclass ,c.georead " &
    '                " FROM  featureclasses c,featurezusachgebiete fs" &
    '                " where c.featureclass=fs.featureclass" &
    '                " and sachgebiet in(select sachgebiet from sachgebiete)" &
    '                " and ((georead like '%intranet%')" &
    '                " or (georead like '%internet%')" &
    '                " or (georead like '%umwelt%')" &
    '                " or (georead like '%extranet%')" &
    '                " or (georead like '%probaug%')" &
    '                " or (georead like '%intranet%')" &
    '                " or (georead like '%intranet%'))" &
    '                " and status=1" &
    '                " order by titel"
    '    Else
    '        sql =
    '                "  SELECT distinct c.titel,c.featureclass ,c.georead " &
    '                " FROM  featureclasses c,featurezusachgebiete fs" &
    '                " where c.featureclass=fs.featureclass" &
    '                " and sachgebiet in(select sachgebiet from sachgebiete)" &
    '                " and sachgebiet='" & kategorie & "'" &
    '                " and ((georead like '%intranet%')" &
    '                " or (georead like '%internet%')" &
    '                " or (georead like '%umwelt%')" &
    '                " or (georead like '%extranet%')" &
    '                " or (georead like '%probaug%')" &
    '                " or (georead like '%intranet%')" &
    '                " or (georead like '%intranet%'))" &
    '                " and status=1" &
    '                " order by titel"
    '    End If
    '    Return sql
    'End Function

    'Sub zeigeAlleRefenrenziertenLayers2(listBox As ListBox, allLayersChecked As Boolean?)
    '    summeLayers = addLayerArray(vorschlagLayers, bestandlayers)
    '    getListOfLayerDetails(summeLayers)
    '    zeigeListeLayers(listBox, allLayersChecked)
    'End Sub

    'Private Function BestandEntfernenen(qliste As List(Of presFeatureClass), VGRUNDtemp As String) As List(Of presFeatureClass)
    '    Dim neu As presFeatureClass
    '    Dim neuliste As New List(Of presFeatureClass)
    '    If qliste Is Nothing Then Return Nothing
    '    getbestandlayers(VGRUNDtemp)
    '    For Each zeile As presFeatureClass In qliste
    '        neu = New presFeatureClass
    '        neu.featureClass = zeile.featureClass
    '        neu.titel = zeile.titel
    '        neu.auswahl = zeile.auswahl
    '        If Not schonInBestand(neu.featureClass, bestandlayers) Then
    '            neuliste.Add(neu)
    '        End If
    '    Next
    '    Return neuliste
    'End Function


    'Private Function featuretab2liste(featuretab As DataTable) As List(Of presFeatureClass)
    '    Dim neu As presFeatureClass
    '    Dim neuliste As New List(Of presFeatureClass)
    '    If featuretab.IsNothingOrEmpty Then Return Nothing
    '    For Each zeile As DataRow In featuretab.AsEnumerable
    '        neu = New presFeatureClass
    '        neu.featureClass = CStr(zeile.Item("featureclass"))
    '        neu.auswahl = False
    '        neu.titel = CStr(zeile.Item("titel"))
    '        neuliste.Add(neu)
    '    Next
    '    Return neuliste
    'End Function

    'Private Function schonInBestand(p1 As String, bestandlayers As String()) As Boolean
    '    Try
    '        For i = 0 To bestandlayers.GetUpperBound(0)
    '            If bestandlayers(i).ToLower = p1.ToLower Then
    '                Return True
    '            End If
    '        Next
    '        Return False
    '    Catch
    '        Return False
    '    End Try
    'End Function

    'Friend Function getFilteredLayersStichwort(stichwort As String, vGRUNDtemp As String) As List(Of presFeatureClass)
    '    Dim sql As String = makeStichwortsucheSQL(stichwort)
    '    Dim liste As New List(Of presFeatureClass)
    '    Try
    '        If sql.Length > 0 Then
    '            '  getLayerDatatable(sql)
    '            featuretab = clsMiniMapTools.getDT4anySQL_ALLEDB_dballe(sql, myglobalz.sitzung.webgisREC)

    '            liste = featuretab2liste(featuretab)
    '            liste = BestandEntfernenen(liste, vGRUNDtemp)

    '            Return liste
    '        End If
    '        Return Nothing
    '    Catch ex As Exception
    '        nachricht("Fehler in getFilteredLayers: " ,ex)
    '        Return Nothing
    '    End Try
    'End Function

    'Private Function makeStichwortsucheSQL(stichwort As String) As String
    '    Dim sql As String = ""
    '    If stichwort = String.Empty Then
    '        sql =
    '                "  SELECT distinct c.titel,c.featureclass ,c.georead " &
    '                " FROM  featureclasses c,featurezusachgebiete fs" &
    '                " where c.featureclass=fs.featureclass" &
    '                " and sachgebiet in(select sachgebiet from sachgebiete)" &
    '                " and ((georead like '%intranet%')" &
    '                " or (georead like '%internet%')" &
    '                " or (georead like '%umwelt%')" &
    '                " or (georead like '%extranet%')" &
    '                " or (georead like '%probaug%')" &
    '                " or (georead like '%intranet%')" &
    '                " or (georead like '%intranet%'))" &
    '                " and status=1" &
    '                " order by titel"
    '    Else
    '        sql =
    '              "  SELECT distinct c.titel,c.featureclass ,c.georead " &
    '              " FROM  featureclasses c,featurezusachgebiete fs" &
    '              " where c.featureclass=fs.featureclass" &
    '              " and sachgebiet in(select sachgebiet from sachgebiete)" &
    '              " and ((georead like '%intranet%')" &
    '              " or (georead like '%internet%')" &
    '              " or (georead like '%umwelt%')" &
    '              " or (georead like '%extranet%')" &
    '              " or (georead like '%probaug%')" &
    '              " or (georead like '%intranet%')" &
    '              " or (georead like '%intranet%'))" &
    '              " and status=1" &
    '              " and (" &
    '              "(c.featureclass like '%" & stichwort.ToLower & "%') or " &
    '              "(sachgebiet like '%" & stichwort.ToLower & "%') or " &
    '              "(titel like '%" & stichwort.ToLower & "%')  " &
    '              ") " &
    '              " order by titel"
    '    End If
    '    Return sql
    'End Function
End Class
