Public Class clsEreignisTools



    Public Shared Sub fallsErledigtDannSpeichern(ByVal vid%)
        If myGlobalz.sitzung.aktEreignis.Art.ToLower = "vorgang erledigt" Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = True
            myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = Now
            '  Dim erfolg As Boolean = VSTTools.editStammdaten_alleDB.exe(vid, myGlobalz.sitzung.aktVorgang.Stammdaten)
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "ERLEDIGT")
            ' If erfolg Then
            nachricht_und_Mbox("Der Vorgang ist erledigt." & Environment.NewLine & Environment.NewLine &
                                     "______________________________________________________________________________ " & Environment.NewLine & Environment.NewLine &
                                     "Evtl. Wiedervorlagen bleiben erhalten: " & Environment.NewLine & Environment.NewLine &
                                     "  Wenn Sie evtl. Wiedervorlagen als erledigt markieren wollen, so müssen Sie " + Environment.NewLine & Environment.NewLine &
                                     "  - den Vorgang nochmal öffnen und" + Environment.NewLine &
                                     "  - die Wiedervorlage öffnen und erledigen")
            '  End If
        End If
    End Sub
    Public Shared Sub setLetztesEreignisText(ByVal ereigniss As clsEreignis) 'myGlobalz.sitzung.aktEreignis
        myGlobalz.sitzung.aktVorgang.Stammdaten.LastActionHeroe = ereigniss.Art & ": " & ereigniss.Beschreibung
    End Sub
    Public Shared Function ToObj_Ereignis() As Boolean
        If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            myGlobalz.sitzung.aktEreignis.Art = clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Art"))
            myGlobalz.sitzung.aktEreignis.Beschreibung = clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Beschreibung"))
            myGlobalz.sitzung.aktEreignis.Datum = clsDBtools.fieldvalueDate(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Datum"))
            myGlobalz.sitzung.aktEreignis.Richtung = clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Richtung"))
            myGlobalz.sitzung.aktEreignis.Notiz = clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Notiz"))
            myGlobalz.sitzung.aktEreignis.DokumentID = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("DokumentID")))
            myGlobalz.sitzung.aktEreignis.Quelle = clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("Quelle"))
            myGlobalz.sitzung.aktEreignis.ID = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("ID")))
            myglobalz.sitzung.aktEreignis.istRTF = CBool(clsDBtools.toBool(myglobalz.sitzung.tempREC.dt.Rows(0).Item("ISTRTF")))
        Else
            'todo ist das nicht .clear????
            myGlobalz.sitzung.aktEreignis.Art = ""
            myGlobalz.sitzung.aktEreignis.Beschreibung = ""
            myGlobalz.sitzung.aktEreignis.Datum = CDate("")
            myGlobalz.sitzung.aktEreignis.Richtung = ""
            myGlobalz.sitzung.aktEreignis.Notiz = ""
            myGlobalz.sitzung.aktEreignis.DokumentID = 0
            myGlobalz.sitzung.aktEreignis.Quelle = ""
            myGlobalz.sitzung.aktEreignis.istRTF = False
        End If
        myGlobalz.sitzung.aktEreignis.anychange = False
    End Function
    Public Shared Function leseEreignisByID(ByVal eid As Integer) As Boolean
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="Ereignis"
        'getDT_Ereignis(eid)

        myGlobalz.sitzung.tempREC.mydb.SQL =
         "select * from  " & CLstart.myViewsNTabs.tabEreignis & " " &
         " where id=" & eid
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            '  nachricht_und_Mbox("Fatal Error ID " & clsGlobalz.sitzung.VorgangsID & "konnte nicht gefunden werden!" & clsGlobalz.sitzung.VorgangREC.mydb.getDBinfo(""))
            nachricht("Fatal Error ID " & eid & "konnte nicht gefunden werden!" &
             myGlobalz.sitzung.tempREC.mydb.getDBinfo(""))
        End If
    End Function
    'Public Shared Function getDT_Ereignis(ByVal eid As Integer) As Boolean    'myGlobalz.sitzung.aktEreignis.ID
    '    Try
    '        myGlobalz.sitzung.tempREC.mydb.SQL =
    '         "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle &
    '         " where id=" & eid
    '        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    '        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
    '            '  nachricht_und_Mbox("Fatal Error ID " & clsGlobalz.sitzung.VorgangsID & "konnte nicht gefunden werden!" & clsGlobalz.sitzung.VorgangREC.mydb.getDBinfo(""))
    '            nachricht("Fatal Error ID " & eid & "konnte nicht gefunden werden!" &
    '             myGlobalz.sitzung.tempREC.mydb.getDBinfo(""))
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("FEhler: getDT_" & vbCrLf & ex.ToString)
    '    End Try
    'End Function



    'Shared Function NeuesEreignisSpeichern_alleDB(ByVal zielvorgangsid As Integer, ByVal modus As String, ereignis As clsEreignis) As Boolean

    '    If myGlobalz.Ereignisse_MYDB.dbtyp = "oracle" Then
    '        Dim zzz As New clsEreignisDB_Oracle(clsDBspecOracle.getConnection(myGlobalz.Ereignisse_MYDB))
    '        myGlobalz.sitzung.aktEreignis.ID = zzz.Neu_speichern_Ereignis(zielvorgangsid, modus, ereignis)
    '        zzz.Dispose()
    '        nachricht("Ereignis ID:" & myGlobalz.sitzung.aktEreignis.ID)
    '        If myGlobalz.sitzung.aktEreignis.ID > 0 Then
    '            nachricht("Neues Ereigniss wurde gespeichert!" & myGlobalz.sitzung.aktEreignis.ID)
    '            Return True
    '        Else
    '            nachricht("Neues Ereigniss wurde nicht gespeichert!")
    '            Return False
    '        End If
    '    End If
    '    Return True
    'End Function

    Public Shared Sub ereignisLoeschen_alleDB(ByVal eid%) 'myGlobalz.sitzung.aktEreignis.ID
        'If eid < 1 Then
        '    nachricht("Fehler: ereignis konnte nicht gelöscht !!! id: " & eid)
        '    Exit Sub
        'End If
        'If myGlobalz.Ereignisse_MYDB.dbtyp = "mysql" Then
        '    Dim zzz As New clsEreignisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.Ereignisse_MYDB))
        '    If zzz.Ereignis_loeschen(eid) < 1 Then
        '        nachricht("Ereignis konnte nicht gelöscht werden: " & eid)
        '    End If
        '    zzz.Dispose()
        'End If
        'If myGlobalz.Ereignisse_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New clsEreignisDB_Oracle(clsDBspecOracle.getConnection(myGlobalz.Ereignisse_MYDB))
        '    If zzz.Ereignis_loeschen(eid) < 1 Then
        '        nachricht("Ereignis konnte nicht gelöscht werden: " & eid)
        '    End If
        '    zzz.Dispose()
        'End If
    End Sub

    Shared Function EDITobj2DBOk_Ereignis_alledb(ByVal eid As Integer, ByVal zielvorgangsid As Integer, ereignis As clsEreignis) As Boolean
        ''todo verhindern, dass gleiches ereignis nochmal gepeichert wird
        'If myGlobalz.Ereignisse_MYDB.dbtyp = "mysql" Then
        '    Dim zzz As New clsEreignisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.Ereignisse_MYDB))
        '    If zzz.Edit_speichern_Ereignis(eid%, zielvorgangsid) Then
        '        nachricht("Ereignis: veränderte Daten  wurden gespeichert!")
        '    End If
        '    zzz.Dispose()
        'End If
        'If myGlobalz.Ereignisse_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New clsEreignisDB_Oracle(clsDBspecOracle.getConnection(myGlobalz.Ereignisse_MYDB))
        '    If zzz.Edit_speichern_Ereignis(eid, zielvorgangsid, ereignis) Then
        '        nachricht("Ereignis: veränderte Daten  wurden gespeichert!")
        '    End If
        '    zzz.Dispose()
        'End If
        'Return True
    End Function

    'Public Shared Function Ereignis_und_Dokumente_entkoppeln(ByVal eid As Integer) As Boolean 'myGlobalz.sitzung.aktEreignis.ID
    '    If eid < 1 Then
    '        nachricht("Fehler: Dokumente konnten nicht vom Ereignis entkoppelt werden !a!! id: " & eid)
    '        Return False
    '    End If
    '    If Not myGlobalz.Arc.ereignisDocDt.IsNothingOrEmpty Then
    '        If myGlobalz.Ereignisse_MYDB.dbtyp = "mysql" Then
    '            Dim zzz As New clsEreignisDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.Ereignisse_MYDB))
    '            If zzz.EntKoppelung_Dokumente_Ereignis(eid) < 1 Then
    '                nachricht("Fehler: Dokumente konnten nicht vom Ereignis entkoppelt werden !!c! id: " & eid)
    '                Return False
    '            Else
    '                nachricht_und_Mbox("Die Dokumente wurden vom Ereignis entkoppelt. " &
    '                                         vbCrLf &
    '                                         vbCrLf &
    '                                         "Hinweis: Die Dokumente wurden nicht gelöscht." & vbCrLf &
    '                                         "Löschen kann man sie unter dem Reiter 'Dokumente'.")
    '                Return True
    '            End If
    '            zzz.Dispose()
    '        End If
    '        'If myGlobalz.Ereignisse_MYDB.dbtyp = "oracle" Then
    '        '    Dim zzz As New clsEreignisDB_Oracle(clsDBspecOracle.getConnection(myGlobalz.Ereignisse_MYDB))
    '        '    If zzz.EntKoppelung_Dokumente_Ereignis(eid) < 1 Then
    '        '        nachricht("Fehler: Dokumente konnten nicht vom Ereignis entkoppelt werden !b!! id: " & eid)
    '        '        zzz.Dispose()
    '        '        Return False
    '        '    Else
    '        '        nachricht_und_Mbox("Die Dokumente wurden vom Ereignis entkoppelt. " &
    '        '                                 vbCrLf &
    '        '                                 vbCrLf &
    '        '                                 "Hinweis: Die Dokumente wurden nicht gelöscht." & vbCrLf &
    '        '                                 "Löschen kann man sie unter dem Reiter 'Dokumente'.")
    '        '        zzz.Dispose()
    '        '        Return True
    '        '    End If
    '        'End If
    '    Else
    '        Return True
    '    End If
    'End Function

    'Shared Function Ereigniskopieren(ByVal ereignisid As Integer, ByVal ZielvorgangsidInput As Integer,
    '                                 ByVal outlookischecked As Boolean) As Boolean
    '    Try
    '        'ereingisEilesen
    '        'ereignis auf OBJ legen
    '        clsEreignisTools.leseEreignisByID(ereignisid)
    '        clsEreignisTools.ToObj_Ereignis()

    '        Dim rtftext As String = ""

    '        Dim altEid As Integer = istEreignisSchonVorhanden(myGlobalz.sitzung.aktEreignis, ZielvorgangsidInput)
    '        If altEid < 1 Then
    '            'IHAHTODO
    '            rtftext = EreignisRTFhelp.getFokumenttextPfadVonRtfTextfromEreignis()

    '            Dim lResult As Boolean = speichernEreignisExtracted(ZielvorgangsidInput, outlookischecked)
    '            If myGlobalz.sitzung.aktEreignis.istRTF Then EreignisRTFhelp.RTFdateispeichern(rtftext, ZielvorgangsidInput,
    '                myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
    '        Else
    '            myGlobalz.sitzung.aktEreignis.ID = altEid
    '        End If
    '        'obj als neues ereignis anlegen

    '        Return True
    '    Catch ex As Exception
    '        nachricht("fehler in Ereigniskopieren: " & ex.ToString)
    '        Return False
    '    End Try
    'End Function

    'Public Shared Function speichernEreignisExtracted(ByVal zielvorgangsid As Integer,
    '                                                  ByVal outlookisChecked As Boolean) As Boolean

    '    'If Not clsEreignisTools.NeuesEreignisSpeichern_alleDB(zielvorgangsid, "neu", myGlobalz.sitzung.aktEreignis) Then Exit Function
    '    ''folgendes nur im NEU-Fall>> war blödsinn
    '    ''myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = myGlobalz.sitzung.aktEreignis.Beschreibung
    '    ''myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung = myGlobalz.sitzung.aktEreignis.Notiz
    '    'If myGlobalz.sitzung.aktEreignis.Art = "Ortstermin" Then myGlobalz.sitzung.aktVorgang.Stammdaten.Ortstermin = True
    '    'If myGlobalz.sitzung.aktEreignis.Art = "Stellungnahme" Then myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = True
    '    'myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = myGlobalz.sitzung.aktEreignis.Datum

    '    'clsEreignisTools.setLetztesEreignisText(myGlobalz.sitzung.aktEreignis)
    '    ''    VSTTools.editStammdaten_alleDB.exe(zielvorgangsid, myGlobalz.sitzung.aktVorgang.Stammdaten)
    '    'detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LASTACTIONHEROE")


    '    'clsEreignisTools.fallsErledigtDannSpeichern(zielvorgangsid)


    '    'glob2.ggfTerminNachOutlookUebernehmen(zielvorgangsid, myGlobalz.sitzung.aktEreignis, outlookisChecked)
    '    'CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
    '    '    myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
    '    'Return False
    'End Function

    'Private Shared Function istEreignisSchonVorhanden(ByVal clsEreignis As clsEreignis, ByVal ZielvorgangsidInput As Integer) As Integer
    '    Try
    '        Dim datumstring As String = " to_date('" & clsEreignis.Datum & "' ,'DD.MM.YYYY HH24:MI:SS') "
    '        myGlobalz.sitzung.tempREC.mydb.SQL = "select id from  " & CLstart.myViewsNTabs.tabEreignis & "  " &
    '            " where vorgangsid=" & ZielvorgangsidInput &
    '             " and beschreibung='" & clsEreignis.Beschreibung & "'" &
    '             " and datum=" & datumstring &
    '             " and art='" & clsEreignis.Art & "'" &
    '             " and richtung='" & clsEreignis.Richtung & "'" '" and quelle='" & clsEreignis.Quelle & "'"
    '        Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
    '        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
    '            Return 0
    '        Else
    '            Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0))
    '        End If
    '    Catch ex As Exception
    '        nachricht("fehler in istEreignisSchonVorhanden: " & ex.ToString)
    '        Return 0
    '    End Try
    'End Function

    'Shared Function istErstmalsErledigt(oldValue As Boolean, newvalue As Boolean) As Boolean
    '    If oldValue = newvalue Then Return False 'Keine Änderung
    '    If oldValue = False And newvalue = True Then Return True
    '    Return False
    'End Function

    'Shared Function AlleDokumentenRevisionssicherMachen(vid As Integer, zusatztext As String) As Integer
    '    Dim sollich As Boolean = wirklich(zusatztext)
    '    Dim anzahl As Integer = 0
    '    If sollich Then
    '        anzahl = DokArcTools.dokusRevisionssichermachen.fuerVorgangsID(vid)
    '        MessageBox.Show(anzahl & " Dokumente wurden revisionssicher gemacht!")
    '    End If
    '    Return anzahl
    'End Function

    'Private Shared Function wirklich(zusatztext As String) As Boolean
    '    Dim test As Boolean = False
    '    Dim mesres As New MessageBoxResult
    '    mesres = MessageBox.Show(zusatztext & vbCrLf & vbCrLf &
    '                    "Wenn Sie die Dokumente revisionssicher machen, können " &
    '                    "sie von niemandem mehr geändert oder gelöscht werden." & vbCrLf &
    '                    "Sie können natürlich jederzeit neue Versionen hinzufügen." & vbCrLf & vbCrLf &
    '                    "Machen sie von dieser Option Gebrauch wenn ein Vorgang " & vbCrLf &
    '                    "wirklich abgeschlossen ist." & vbCrLf &
    '                    "" & vbCrLf &
    '                    "Möchten Sie die Dokumente revisionssicher machen ? " & vbCrLf & vbCrLf &
    '                    "Ja   - durchführen" & vbCrLf &
    '                    "Nein - keine Änderung",
    '                    "Dokumente revisionssicher machen", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No)
    '    Return If(mesres = MessageBoxResult.Yes, True, False)
    'End Function

    'Shared Sub setzeEreingisartfuerEmail(richtung As String, art As String)
    '    If art.ToLower.StartsWith("email") Then
    '        If richtung.ToLower = "ausgang" Then
    '            art = "Email-Ausgang"
    '        Else
    '            art = "Email-Eingang"
    '        End If
    '    End If
    'End Sub

End Class
