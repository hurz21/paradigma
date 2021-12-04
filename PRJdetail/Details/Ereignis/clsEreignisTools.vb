Imports System.Data
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.

Public Class clsEreignisTools


    Public Shared Function GetGesammelteDatumUhrzeit(datum As Date, stunde As String, minute As String) As Date
        Dim gesammelteDatumUhrzeit As Date
        gesammelteDatumUhrzeit = datum '(CDate(DatePicker1.SelectedDate).Date)
        Dim dstunde As Double = CDbl(stunde)
        gesammelteDatumUhrzeit = gesammelteDatumUhrzeit.AddHours(dstunde)
        Dim dminute As Double = CDbl(minute)
        gesammelteDatumUhrzeit = gesammelteDatumUhrzeit.AddMinutes(dminute)
        Return gesammelteDatumUhrzeit
    End Function

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
    Public Shared Function ToObj_Ereignis(datentab As DataTable) As Boolean
        With datentab
            If Not .IsNothingOrEmpty Then
                myGlobalz.sitzung.aktEreignis.Art = clsDBtools.fieldvalue(.Rows(0).Item("Art"))
                myGlobalz.sitzung.aktEreignis.Beschreibung = clsDBtools.fieldvalue(.Rows(0).Item("Beschreibung"))
                myGlobalz.sitzung.aktEreignis.Datum = clsDBtools.fieldvalueDate(.Rows(0).Item("Datum"))
                myGlobalz.sitzung.aktEreignis.Richtung = clsDBtools.fieldvalue(.Rows(0).Item("Richtung"))
                myGlobalz.sitzung.aktEreignis.Notiz = clsDBtools.fieldvalue(.Rows(0).Item("Notiz"))
                myGlobalz.sitzung.aktEreignis.DokumentID = CInt(clsDBtools.fieldvalue(.Rows(0).Item("DokumentID")))
                myGlobalz.sitzung.aktEreignis.Quelle = clsDBtools.fieldvalue(.Rows(0).Item("Quelle"))
                myGlobalz.sitzung.aktEreignis.ID = CInt(clsDBtools.fieldvalue(.Rows(0).Item("ID")))
                myGlobalz.sitzung.aktEreignis.istRTF = CBool(clsDBtools.toBool(.Rows(0).Item("ISTRTF")))
                myGlobalz.sitzung.aktEreignis.typnr = CInt(CBool(clsDBtools.toBool(.Rows(0).Item("TYPNR"))))
                Return True
            Else
                'todo ist das nicht .clear????
                myGlobalz.sitzung.aktEreignis.Art = ""
                myGlobalz.sitzung.aktEreignis.Beschreibung = ""
                myGlobalz.sitzung.aktEreignis.Datum = CDate("1800-01-01")
                myGlobalz.sitzung.aktEreignis.Richtung = ""
                myGlobalz.sitzung.aktEreignis.Notiz = ""
                myGlobalz.sitzung.aktEreignis.DokumentID = 0
                myGlobalz.sitzung.aktEreignis.Quelle = myGlobalz.sitzung.aktBearbeiter.getInitial
                myGlobalz.sitzung.aktEreignis.istRTF = False
                myGlobalz.sitzung.aktEreignis.typnr = CInt(CBool(clsDBtools.toBool(.Rows(0).Item("TYPNR"))))
                Return False
            End If
        End With
        myGlobalz.sitzung.aktEreignis.anychange = False
    End Function
    Public Shared Function leseEreignisByID(ByVal eid As Integer) As Boolean
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="Ereignis"
        'Return getDT_Ereignis(eid)

        myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabEreignis & "  where id=" & eid
        Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            l("fehler Fatal Error ID " & "konnte nicht gefunden werden!" & myGlobalz.sitzung.tempREC.mydb.getDBinfo(""))
            Return False
        Else
            Return True
        End If

    End Function
    'Public Shared Function getDT_Ereignis(ByVal eid As Integer) As Boolean    'myGlobalz.sitzung.aktEreignis.ID
    '    Try
    '        myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabEreignis & "  where id=" & eid
    '        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    '        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
    '            '  nachricht_und_Mbox("Fatal Error ID " & clsGlobalz.sitzung.VorgangsID & "konnte nicht gefunden werden!" & clsGlobalz.sitzung.VorgangREC.mydb.getDBinfo(""))
    '            nachricht("Fatal Error ID " & eid & "konnte nicht gefunden werden!" &
    '             myGlobalz.sitzung.tempREC.mydb.getDBinfo(""))
    '            Return False
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht_und_Mbox("FEhler: getDT_" & vbCrLf ,ex)
    '        Return False
    '    End Try
    'End Function


    Shared Sub werteDBsicherMachenEreignis(ereignis As clsEreignis)
        If myGlobalz.sitzung Is Nothing Then nachricht("1")
        If ereignis Is Nothing Then nachricht("2")
        If myGlobalz.sitzung.aktBearbeiter Is Nothing Then nachricht("3")
        'datum umsetzen für oracle
        With ereignis 'myGlobalz.sitzung.aktEreignis
            If String.IsNullOrEmpty(.Notiz) Then .Notiz = " "
            If String.IsNullOrEmpty(.Beschreibung) Then .Beschreibung = ""
            If String.IsNullOrEmpty(.Quelle) Then .Quelle = myGlobalz.sitzung.aktBearbeiter.getInitial
            If String.IsNullOrEmpty(.Art) Then .Art = ""

            If .Art.Length > 41 Then .Art = .Art.Substring(0, 40)
            If .Quelle.Length > 41 Then .Art = .Quelle.Substring(0, 40)
            If .Beschreibung.Length > 399 Then .Beschreibung = .Beschreibung.Substring(0, 398)
            nachricht(" Len(.Notiz) " & Len(ereignis.Notiz))
            'nachricht("zielvorgangsid " & ereignis.zielvorgangsid)
            nachricht("BESCHREIBUNG " & ereignis.Beschreibung)
            nachricht("DATUM " & ereignis.Datum)
            nachricht("DOKUMENTID " & ereignis.DokumentID)
            nachricht("PERSONENID " & myGlobalz.sitzung.aktBearbeiter.PersonenID)
            nachricht("CInt(.istRTF) " & CInt(ereignis.istRTF))
            'nachricht("zielvorgangsid " & zielvorgangsid) 
        End With
    End Sub





    Public Shared Sub ereignisLoeschen_alleDB(ByVal eid%) 'myGlobalz.sitzung.aktEreignis.ID
        If eid < 1 Then
            nachricht("Fehler: ereignis konnte nicht gelöscht !!! id: " & eid)
            Exit Sub
        End If
        Dim hinweis As String = ""
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabEreignis & "  where id=" & eid.ToString
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)

    End Sub

    Shared Function NeuesEreignisSpeichern_alleDB(ByVal zielvorgangsid As Integer, ByVal modus As String,
                                                  ByRef ereignis As clsEreignis) As Boolean
        Dim querie As String
        werteDBsicherMachenEreignis(ereignis)
        clsSqlparam.paramListe.Clear()
        populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'paramListe.Add(New sqlparam("eid", 0))
        querie = "INSERT INTO " & CLstart.myViewsNTabs.tabEreignis & "  (VORGANGSID,BESCHREIBUNG,DATUM,ART,RICHTUNG,NOTIZ,DOKUMENTID,PERSONENID,QUELLE,ISTRTF,TYPNR) " +
                 "             VALUES (@VORGANGSID,@BESCHREIBUNG,@DATUM,@ART,@RICHTUNG,@NOTIZ,@DOKUMENTID,@PERSONENID,@QUELLE,@ISTRTF,@TYPNR)"
        myGlobalz.sitzung.aktEreignis.ID = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
        nachricht("Ereignis ID:" & myGlobalz.sitzung.aktEreignis.ID)
        ereignis.ID = myGlobalz.sitzung.aktEreignis.ID
        If myGlobalz.sitzung.aktEreignis.ID > 0 Then
            nachricht("Neues Ereigniss wurde gespeichert!" & myGlobalz.sitzung.aktEreignis.ID)
        End If
        Return True
    End Function
    Shared Function EDITobj2DBOk_Ereignis_alledb(ByVal eid As Integer, ByVal zielvorgangsid As Integer, ereignis As clsEreignis) As Boolean
        Dim querie As String
        querie = "update " & CLstart.myViewsNTabs.tabEreignis & " " & " SET " &
         " VORGANGSID=@VORGANGSID" &
         ",BESCHREIBUNG=@BESCHREIBUNG" &
         ",DATUM=@DATUM " &
         ",ART=@ART " &
         ",RICHTUNG=@RICHTUNG " &
         ",NOTIZ=@NOTIZ " &
         ",DOKUMENTID=@DOKUMENTID " &
         ",PERSONENID=@PERSONENID " &
         ",ISTRTF=@ISTRTF " &
         ",QUELLE=@QUELLE " &
         " where id=@id"
        werteDBsicherMachenEreignis(ereignis)
        clsSqlparam.paramListe.Clear()
        populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        clsSqlparam.paramListe.Add(New clsSqlparam("id", eid))
        '--------------------------------
        Dim erolg = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
        Return True
    End Function
    Shared Sub populateParamListeEreignis(zielvorgangsid As Integer, ereignis As clsEreignis, paramListe As List(Of clsSqlparam))
        paramListe.Add(New clsSqlparam("VORGANGSID", zielvorgangsid))
        paramListe.Add(New clsSqlparam("BESCHREIBUNG", ereignis.Beschreibung))
        Dim cand As Date = clsDBtools.makedateMssqlConform(CDate(ereignis.Datum), myGlobalz.sitzung.VorgangREC.mydb.dbtyp)
        paramListe.Add(New clsSqlparam("DATUM", cand))
        paramListe.Add(New clsSqlparam("ART", ereignis.Art))
        paramListe.Add(New clsSqlparam("RICHTUNG", ereignis.Richtung))
        paramListe.Add(New clsSqlparam("NOTIZ", ereignis.Notiz))
        paramListe.Add(New clsSqlparam("DOKUMENTID", (ereignis.DokumentID)))
        paramListe.Add(New clsSqlparam("PERSONENID", myGlobalz.sitzung.aktBearbeiter.PersonenID))
        paramListe.Add(New clsSqlparam("QUELLE", ereignis.Quelle))
        paramListe.Add(New clsSqlparam("ISTRTF", CInt(ereignis.istRTF)))
        paramListe.Add(New clsSqlparam("TYPNR", CInt(ereignis.typnr)))
    End Sub

    Public Shared Function Ereignis_und_Dokumente_entkoppeln(ByVal eid As Integer) As Boolean 'myGlobalz.sitzung.aktEreignis.ID
        Dim hinweis As String = ""
        If eid < 1 Then
            nachricht("Fehler: Dokumente konnten nicht vom Ereignis entkoppelt werden !a!! id: " & eid)
            Return False
        End If
        If Not myGlobalz.Arc.ereignisDocDt.IsNothingOrEmpty Then
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "update  " & CLstart.myViewsNTabs.tabDokumente & "  set eid=0 where eid=" & eid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
        Else
            Return True
        End If
        Return True

    End Function

    Shared Function Ereigniskopieren(ByVal ereignisid As Integer, ByVal ZielvorgangsidInput As Integer,
                                     ByVal outlookischecked As Boolean) As Boolean
        Dim lResult As Boolean
        Try
            'ereingisEilesen
            'ereignis auf OBJ legen
            If Not clsEreignisTools.leseEreignisByID(ereignisid) Then
                nachricht("kein ereignis gefunden")
                Return False
            End If
            clsEreignisTools.ToObj_Ereignis(myGlobalz.sitzung.tempREC.dt)

            Dim rtftext As String = ""

            Dim altEid As Integer = istEreignisSchonVorhanden(myGlobalz.sitzung.aktEreignis, ZielvorgangsidInput)
            If altEid < 1 Then
                rtftext = EreignisRTFhelp.getFokumenttextPfadVonRtfTextfromEreignis()
                lResult = speichernEreignisExtracted(ZielvorgangsidInput, outlookischecked)
                Dim kompress As Boolean
                'If (Environment.UserName.ToLower = "feinen_j") Then
                '    'ist unabhängig vom savemode
                '    kompress = True
                'Else
                kompress = False
                'End If
                If myGlobalz.sitzung.aktEreignis.istRTF Then

                    EreignisRTFhelp.RTFdateispeichern(rtftext, ZielvorgangsidInput,
                                                      myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir, kompress)
                End If
            Else
                MessageBox.Show("Das Ereignis ist schon vorhanden. Es wird weiterbenutzt!", "Hinweis")
                myGlobalz.sitzung.aktEreignis.ID = altEid
            End If
            'If altEid < 1 Then
            '    'Ereignis noch nicht vorhanden

            'Else
            '    If einraster Then

            '    Else
            '        MessageBox.Show("Das Ereignis ist schon vorhanden. Wir weiterbenutzt!", "Hinweis")
            '        'myGlobalz.sitzung.aktEreignis.Beschreibung = myGlobalz.sitzung.aktEreignis.Beschreibung & " " &
            '        '    glob2.getTimestamp
            '        einraster = True
            '    End If
            'End If
            'obj als neues ereignis anlegen
            'rtftext = EreignisRTFhelp.getFokumenttextPfadVonRtfTextfromEreignis()

            'Dim lResult As Boolean = speichernEreignisExtracted(ZielvorgangsidInput, outlookischecked)
            'If myGlobalz.sitzung.aktEreignis.istRTF Then
            '    EreignisRTFhelp.RTFdateispeichern(rtftext, ZielvorgangsidInput,
            '                                          myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
            'End If
            Return True
        Catch ex As Exception
            nachricht("fehler in Ereigniskopieren: " ,ex)
            Return False
        End Try
    End Function

    Public Shared Function speichernEreignisExtracted(ByVal zielvorgangsid As Integer,
                                                      ByVal outlookisChecked As Boolean) As Boolean
        If Not clsEreignisTools.NeuesEreignisSpeichern_alleDB(zielvorgangsid, "neu", myGlobalz.sitzung.aktEreignis) Then
            Return False
        End If
        'folgendes nur im NEU-Fall>> war blödsinn
        'myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = myGlobalz.sitzung.aktEreignis.Beschreibung
        'myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung = myGlobalz.sitzung.aktEreignis.Notiz
        If myGlobalz.sitzung.aktEreignis.Art = "Ortstermin" Then myGlobalz.sitzung.aktVorgang.Stammdaten.Ortstermin = True
        If myGlobalz.sitzung.aktEreignis.Art = "Stellungnahme" Then myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = True
        myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = myGlobalz.sitzung.aktEreignis.Datum

        clsEreignisTools.setLetztesEreignisText(myGlobalz.sitzung.aktEreignis)
        '    VSTTools.editStammdaten_alleDB.exe(zielvorgangsid, myGlobalz.sitzung.aktVorgang.Stammdaten)
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LASTACTIONHEROE")
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "LETZTEBEARBEITUNG")
        clsEreignisTools.fallsErledigtDannSpeichern(zielvorgangsid)
        glob2.ggfTerminNachOutlookUebernehmen(zielvorgangsid, myGlobalz.sitzung.aktEreignis, outlookisChecked)
        CLstart.myc.aLog.komponente = "Ereignis" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
            myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : CLstart.myc.aLog.log()
        Return True
    End Function

    Private Shared Function istEreignisSchonVorhanden(ByVal clsEreignis As clsEreignis, ByVal ZielvorgangsidInput As Integer) As Integer
        Try
            Dim datumstring As String 'oracle: = " to_date('" & clsEreignis.Datum & "' ,'DD.MM.YYYY HH24:MI:SS') " 
            datumstring = (Format(CDate(clsEreignis.Datum), "dd/MM/yyyy hh:mm:ss"))
            datumstring = (Format(CDate(clsEreignis.Datum), "yyyy-MM-dd hh:mm:ss.  0000000"))
            'datumstring = CDate(clsEreignis.Datum).ToShortDateString
            'datumstring = CDate(clsEreignis.Datum).ToShortTimeString
            'datumstring = CDate(clsEreignis.Datum).ToLongDateString
            'datumstring = CDate(clsEreignis.Datum).ToLongTimeString
            myGlobalz.sitzung.tempREC.mydb.SQL = "select id from " & CLstart.myViewsNTabs.tabEreignis & "  " &
                " where vorgangsid=" & ZielvorgangsidInput &
                 " and beschreibung='" & clsEreignis.Beschreibung & "'" &
                 " and cast(datum AS DATETIME2(0))='" & datumstring & "'" &
                 " and art='" & clsEreignis.Art & "'" &
                 " and richtung='" & clsEreignis.Richtung & "'"

            myGlobalz.sitzung.tempREC.mydb.SQL = "select id from " & CLstart.myViewsNTabs.tabEreignis & "  " &
                " where vorgangsid=" & ZielvorgangsidInput &
                 " and beschreibung='" & clsEreignis.Beschreibung & "'" &
                 " and art='" & clsEreignis.Art & "'" &
                 " and richtung='" & clsEreignis.Richtung & "'"

            Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                Return 0
            Else
                Return CInt(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0))
            End If
        Catch ex As Exception
            nachricht("fehler in istEreignisSchonVorhanden: " ,ex)
            Return 0
        End Try
    End Function

    Shared Function istErstmalsErledigt(oldValue As Boolean, newvalue As Boolean) As Boolean
        If oldValue = newvalue Then Return False 'Keine Änderung
        If oldValue = False And newvalue = True Then Return True
        Return False
    End Function

    Shared Function AlleDokumentenRevisionssicherMachen(vid As Integer, zusatztext As String) As Integer
        Dim sollich As Boolean = wirklich(zusatztext)
        Dim anzahl As Integer = 0
        If sollich Then
            anzahl = DokArcTools.dokusRevisionssichermachen.fuerVorgangsID(vid)
            MessageBox.Show(anzahl & " Dokumente wurden revisionssicher gemacht!")
        End If
        Return anzahl
    End Function

    Private Shared Function wirklich(zusatztext As String) As Boolean
        Dim test As Boolean = False
        Dim mesres As New MessageBoxResult
        mesres = MessageBox.Show(zusatztext & glob2.getMsgboxText("wirklichZusatztext", New List(Of String)(New String() {})),
                        "Dokumente revisionssicher machen", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.No)
        Return If(mesres = MessageBoxResult.Yes, True, False)
    End Function

    Shared Sub setzeEreingisartfuerEmail(richtung As String, art As String)
        If art.ToLower.StartsWith("email") Then
            If richtung.ToLower = "ausgang" Then
                art = "Email-Ausgang"
            Else
                art = "Email-Eingang"
            End If
        End If
    End Sub

End Class
