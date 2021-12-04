Imports System.Data
Module mgistools
    'Public gisProcListe As New List(Of Process)
    Public paradigmaInstanzGISproc As Process
    Sub mgisStarten(allegissekillen As Boolean)
        Try
            l("mgisStarten---------------------- anfang")
            If paradigmaInstanzGISproc Is Nothing Then
                l("mgisStarten isnothing")
                paradigmaInstanzGISproc = startenGIS(calcNewMaxRange(myGlobalz.sitzung.aktVorgangsID))
            Else
                'dann neustarten
                l(" zuerst alten prozess killen")
                If Not paradigmaInstanzGISproc.HasExited Then
                    paradigmaInstanzGISproc.CloseMainWindow()
                End If

                l(" dann neustarten")
                paradigmaInstanzGISproc = startenGIS(calcNewMaxRange(myGlobalz.sitzung.aktVorgangsID))
            End If
            l("mgisStarten---------------------- ende")
        Catch ex As Exception
            l("Fehler in mgisStarten: " ,ex)
        End Try
    End Sub

    'Private Function istParadigmaHochfahren(paradigmaInstanzGISproc As Process) As Boolean
    '    If  Is Nothing Then
    '        Return True
    '    Else
    '        Return False
    '    End If
    'End Function

    'Private Function refreshGisProcListe() As List(Of Process)
    '    Dim bereinigteGisProcListe As New List(Of Process)
    '    For Each myProcess In gisProcListe
    '        If Not myProcess.HasExited Then
    '            bereinigteGisProcListe.Add(myProcess)
    '        End If
    '    Next
    '    Return bereinigteGisProcListe
    'End Function

    'Private Function killAltenGisProcess() As Boolean
    '    Try
    '        Return gisProcListe.Item(0).CloseMainWindow()
    '    Catch ex As Exception
    '        l(" in killAltenGisProcess " ,ex)
    '        Return False
    '    End Try
    'End Function

    Function startenGIS(lokrange As clsRange) As Process
        'Dim gisRoot = "C:\ptest\mgis\"
        Dim gisexe, params As String '= gisRoot & "mgisx64.exe"
        Dim count As Integer = 1
        Try
            l(" starten ---------------------- anfang")
            gisexe = initP.getValue("GisServer.mgis")
            'CLstart.myc.kartengen.aktMap.aktrange = calcNewMaxRange(myGlobalz.sitzung.aktVorgangsID)
            count = 2
            params = " modus=paradigma vorgangsid=" & myGlobalz.sitzung.aktVorgangsID &
                  " range=" &
                  CInt(lokrange.xl) & "," &
                  CInt(lokrange.xh) & "," &
                  CInt(lokrange.yl) & "," &
                  CInt(lokrange.yh) &
                  " beschreibung=" & Chr(34) & myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung.Trim & Chr(34) &
                  " az=" & Chr(34) & myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt & Chr(34)
            count = 3
            l("btnZumGis " & params & Environment.NewLine & gisexe)
            Dim startinfo As New ProcessStartInfo
            startinfo.FileName = gisexe
            startinfo.WorkingDirectory = "c:\kreisoffenbach\mgis"
            startinfo.Arguments = params
            Dim proc As New Process
            '  proc = Process.Start(gisexe, params)
            'l(startinfo.UseShellExecute.ToString)
            proc = Process.Start(startinfo)
            count = 4
            l(" starten ---------------------- ende")
            Return proc
        Catch ex As Exception
            l("Fehler in starten a: " & count & " " ,ex)
        End Try
#Disable Warning BC42105 ' Function 'starten' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.
    End Function
#Enable Warning BC42105 ' Function 'starten' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.



    Private Function calcNewMaxRange(aktVorgangsID As Integer) As clsRange
        'hier noch ein megaproblem

        Dim newrange As New clsRange
        Try
            l("calcNewMaxRange---------------------- anfang")
            Dim dt As System.Data.DataTable
            'Dim zzz As New clsDS(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
            'dt = zzz.getAktMaxRangeDatatable(aktVorgangsID, myGlobalz.sitzung.VorgangREC)
            Dim query, hinweis As String
            query = "select round(min(xmin),0) as xmin,round(max(xmax),0) as xmax,round(min(ymin),0) as ymin,round(max(ymax),0) as ymax  " &
                " from " & CLstart.myViewsNTabs.view_raumbezugundvorg & " rbv " &
                " where  xmin>0 and xmax>0 and ymin>0 and ymax>0 and ISMAPENABLED=1 and vorgangsid=" & aktVorgangsID

#Disable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
            dt = getDT4Query(query, myGlobalz.sitzung.VorgangREC, hinweis)
#Enable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
            'getAktMaxRangeDatatable(aktVorgangsID, myGlobalz.sitzung.VorgangREC)
            'zzz.Dispose()
            'If dt.Rows.Count > 0 Then
            '    Return dt
            'Else
            '    Return Nothing
            'End If
            If Not dt.IsNothingOrEmpty Then
                If clsDBtools.fieldvalue(dt.Rows(0).Item("xmin")).IsNothingOrEmpty Then
                    newrange.xl = CLstart.myc.globalrange.xl
                    newrange.xh = CLstart.myc.globalrange.xh
                    newrange.yl = CLstart.myc.globalrange.yl
                    newrange.yh = CLstart.myc.globalrange.yh
                Else
                    newrange.xl = CDbl(clsDBtools.fieldvalue(dt.Rows(0).Item("xmin")))
                    newrange.xh = CDbl(clsDBtools.fieldvalue(dt.Rows(0).Item("xmax")))
                    newrange.yl = CDbl(clsDBtools.fieldvalue(dt.Rows(0).Item("ymin")))
                    newrange.yh = CDbl(clsDBtools.fieldvalue(dt.Rows(0).Item("ymax")))
                End If
            Else
                'keine daten vorhanden
                newrange.xl = CLstart.myc.globalrange.xl
                newrange.xh = CLstart.myc.globalrange.xh
                newrange.yl = CLstart.myc.globalrange.yl
                newrange.yh = CLstart.myc.globalrange.yh
            End If
            dt = Nothing
            Return newrange
            l("calcNewMaxRange---------------------- ende")
        Catch ex As Exception
            l("Fehler in calcNewMaxRange: " ,ex)

        End Try
#Disable Warning BC42105 ' Function 'calcNewMaxRange' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.
    End Function
#Enable Warning BC42105 ' Function 'calcNewMaxRange' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.

    'Private Function paradigmaGisLaeuftschon() As Boolean
    '    Dim myProcesses() As Process
    '    'Hier wird geprüft ob ein eigener processläuft
    '    If gisProcListe.Count < 1 Then
    '        Return False
    '    Else
    '        Return True
    '    End If
    '    '' Returns array containing all instances of "Notepad".
    '    'myProcesses = Process.GetProcessesByName(prozessname)
    '    'If myProcesses.Count > 0 Then
    '    '    Return True
    '    'Else
    '    '    Return False
    '    'End If
    'End Function
    ''' <summary>
    ''' liefert die liste der gekillten procs für die bereinigung, killedProcs
    ''' </summary>
    ''' <param name="processname"></param>
    ''' <returns></returns>
    Function alleFremdGISSEAbschiessen(processname As String) As List(Of Process)
        Dim myProcesses() As Process
        Dim myProcess As Process
        Dim killedProcs As New List(Of Process)
        'Dim bereinigteGisProcListe As New List(Of Process)
        Try
            l("abschiessen---------------------- anfang")
            ' Returns array containing all instances of "Notepad".
            myProcesses = Process.GetProcessesByName(processname)
            l("abschiessen " & myProcesses.Count)
            killedProcs = listeAlleGISProcsBilden(myProcesses)
            l("abschiessen " & myProcesses.Count)

            For Each myProcess In killedProcs

                If paradigmaInstanzGISproc Is Nothing OrElse (Not myProcess.Id = paradigmaInstanzGISproc.Id) Then
                    'alle ausser der paradigmainstanz werden gekilled
                    myProcess.CloseMainWindow()
                End If
            Next
            l("---------------------- ende")
        Catch ex As Exception
            l("Fehler in : " ,ex)
            Return Nothing
        End Try
#Disable Warning BC42105 ' Function 'alleFremdGISSEAbschiessen' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.
    End Function
#Enable Warning BC42105 ' Function 'alleFremdGISSEAbschiessen' doesn't return a value on all code paths. A null reference exception could occur at run time when the result is used.

    Private Function listeAlleGISProcsBilden(myProcesses() As Process) As List(Of Process)
        Dim killedProcs As New List(Of Process)
        For Each myProcess In myProcesses
            'If gisProcListe IsNot Nothing Then
            '    If isIngisProcList(myProcess, gisProcListe) Then
            ' myProcess.CloseMainWindow()
            killedProcs.Add(myProcess)
            '    End If
            'End If
        Next
        Return killedProcs
    End Function

    'Private Function isIngisProcList(kandidat As Process, tProcList As List(Of Process)) As Boolean
    '    For Each myProcess In tProcList
    '        If myProcess.Id = kandidat.Id Then
    '            Return True
    '        End If
    '    Next
    '    Return False
    'End Function

    Friend Sub mgisSchliessen()
        Try
            l("mgisSchliessen---------------------- anfang")
            Dim ival As String
            ival = CLstart.myc.userIniProfile.WertLesen("GISSTART", "immerbeenden")
            l("ival " & ival)
            If CInt(ival) = 1 Then
                l("immerbeenden)) = 1")
                ProzesskillenFallsvorhanden()

            Else
                'If Not paradigmaInstanzGISproc.HasExited Then
                '    paradigmaInstanzGISproc.CloseMainWindow()
                'End If
            End If
            l("mgisSchliessen---------------------- ende")
        Catch ex As Exception
            l("Fehler in mgisSchliessen: " ,ex)
        End Try
    End Sub

    Private Sub ProzesskillenFallsvorhanden()
        Try
            l("gisProzesskillenFallsvorhanden")
            If paradigmaInstanzGISproc IsNot Nothing Then
                l("isnot nothing")
                If paradigmaInstanzGISproc.HasExited Then
                    l("  paradigmaInstanzGISproc.HasExited")
                Else
                    l("Not paradigmaInstanzGISproc.HasExited")
                    paradigmaInstanzGISproc.CloseMainWindow()
                End If
            Else
                l("is nothing")
            End If

        Catch ex As Exception
            l("fehler in gisProzesskillenFallsvorhanden" ,ex)
        End Try
    End Sub

    Friend Sub mgisAktualisieren()
        Dim batch As String = "\\gis\gdvell\apps\mgisupdate.bat"
        Try
            l(" mgisAktualisieren ---------------------- anfang" & batch)
            If myGlobalz.zuhause Then Exit Sub
            Process.Start(batch)
            l(" mgisAktualisieren ---------------------- ende")
        Catch ex As Exception
            l("Fehler in mgisAktualisieren: " ,ex)
        End Try
    End Sub
End Module
