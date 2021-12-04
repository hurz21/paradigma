Public Class clsWindowsFotodruck
    Shared Property ausgabeverzeichnis As String
    Friend Shared Function haupt(ereignisiD As Integer, aktVorgangsID As Integer, beschreibung As String, lokalerCheckoutcache As String) As Boolean
        'ausgabeverzeichnis bestimmen
        '
        Try
            l("clsWindowsFotodruck.haupt ---------------------- anfang")
            ausgabeverzeichnis = lokalerCheckoutcache & "\" & aktVorgangsID & "\" & "WindowsFotoDruck_" & glob2.getTimestamp
            If glob2.erzeugeVerzeichnis(ausgabeverzeichnis) Then

                'Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(myGlobalz.sitzung.aktVorgangsID, "nurfotos")

                Dim bresult As Boolean
                bresult = DokArcOracle.initDokumente4EreignisDatatable(ereignisiD, "nurfotos")
                Dim verzeichnisOeffnen As Boolean = True

                If bresult Then
                    myGlobalz.Arc.vorgangDocDt = myGlobalz.Arc.ArcRec.dt.Copy
                    If myGlobalz.Arc.DataTable_auschecken(ausgabeverzeichnis, verzeichnisOeffnen) Then
                        IO.Directory.CreateDirectory(String.Format("{0}\{1}", myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktVorgangsID))
                        If verzeichnisOeffnen Then
                            System.Diagnostics.Process.Start(String.Format("{0}\{1}",
                                            myGlobalz.Arc.lokalerCheckoutcache,
                                            myGlobalz.sitzung.aktVorgangsID))
                        End If
                    End If
                End If
                Return True
            Else
                l("fehler in clsWindowsFotodruck")
                MsgBox("fehler in clsWindowsFotodruck")
            End If
            l("clsWindowsFotodruck.haupt---------------------- ende")
        Catch ex As Exception
            l("Fehler in clsWindowsFotodruck.haupt: ", ex)
            Return False
        End Try
#Disable Warning BC42353 ' Function 'haupt' doesn't return a value on all code paths. Are you missing a 'Return' statement?
    End Function
#Enable Warning BC42353 ' Function 'haupt' doesn't return a value on all code paths. Are you missing a 'Return' statement?


End Class
