﻿Imports System
Imports System.Data
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Public Class clsWVTOOLS
    Public Shared Function WVEreignisAnpassen(ByVal wv As clsWiedervorlage, ByVal eid As Integer) As clsEreignis
        Try
            Dim ereig As New clsEreignis
            If eid > 0 Then ereig.ID = eid
            'Dim erledigttext$ = ""
            ereig.Datum = Now
            ereig.Art = "Wiedervorlage"
            ereig.Richtung = ""
            If wv Is Nothing Then
                'nachricht_und_Mbox("Fehler in NeuesEreigniserzeugen wv is nothing: ")
                ereig.Notiz = "Fehler in NeuesEreigniserzeugen wv is nothing:"
            Else
                ereig.Richtung = ""
                ereig.DokumentID = wv.WiedervorlageID
                ereig.Beschreibung = wv.bildeErgeignisBeschreibung()
                ereig.typnr = 2
            End If
            Return ereig
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Shared Sub WVneuOKExtracted(ByVal wv As clsWiedervorlage, ByVal eid%)
        Dim test = WVEreignisAnpassen(wv, eid)
        If test Is Nothing Then
            MsgBox("Fehler ereignis  wurde nicht erzeugt")
        Else
            myGlobalz.sitzung.aktEreignis = test
        End If
    End Sub

    Public Shared Function setzeNeuesWVDatum(ByVal cmbString As String) As DateTime
        Dim faelligAm As Date
        Select Case cmbString
            Case "In 10 Jahren"
                faelligAm = DateAdd("yyyy", 10, Now)
            Case "In 5 Jahren"
                faelligAm = DateAdd("yyyy", 5, Now)
            Case "In 1 Jahr"
                faelligAm = DateAdd("yyyy", 1, Now)
            Case "In 4 Wochen"
                faelligAm = DateAdd("m", 1, Now)
            Case "In 3 Wochen"
                faelligAm = DateAdd("ww", +3, Now)
            Case "In 2 Wochen"
                faelligAm = DateAdd("ww", +2, Now)
            Case "In 1 Wochen"
                faelligAm = DateAdd("ww", +1, Now)
            Case "In 3 Tagen"
                faelligAm = DateAdd("d", +3, Now)
            Case Else
                faelligAm = Nothing
        End Select
        Return faelligAm
    End Function


    Shared Function WVrecord2OBJ(ByRef wvl As clsWiedervorlage, ByRef REC As DataTable) As Boolean  'myGlobalz.sitzung.aktWiedervorlage		'REC
        Try
            wvl.Bemerkung = clsDBtools.fieldvalue(REC.Rows(0).Item("Bemerkung"))
            wvl.ToDo = clsDBtools.fieldvalue(REC.Rows(0).Item("ToDo"))
            wvl.WartenAuf = clsDBtools.fieldvalue(REC.Rows(0).Item("WartenAuf"))
            wvl.datum = clsDBtools.fieldvalueDate(REC.Rows(0).Item("datum"))
            wvl.erledigtAm = clsDBtools.fieldvalueDate(REC.Rows(0).Item("erledigtAm"))
            wvl.Erledigt = CBool(REC.Rows(0).Item("Erledigt"))
            wvl.WiedervorlageID = CInt(REC.Rows(0).Item("id"))
            wvl.Bearbeiter = CStr(REC.Rows(0).Item("Bearbeiter"))
            wvl.BearbeiterID = CInt(REC.Rows(0).Item("Bearbeiterid"))
            Return True
        Catch ex As Exception
            nachricht("Fehler in WVrecord2OBJ: " ,ex)
            wvl = Nothing
            Return False
        End Try
    End Function
    Private Property selectString As String = "SELECT w.VorgangsID,w.Datum,w.WartenAuf,w.Bemerkung , w.Erledigtam,w.Erledigt,w.ToDo,w.Bearbeiter,v.az,s.Beschreibung,w.bearbeiterid"
    Private Property fromstring As String = " FROM " & CLstart.myViewsNTabs.tabWV & "  w , " & CLstart.myViewsNTabs.tabStammdaten & " s, " & CLstart.myViewsNTabs.tabVorgang & " v "
    Private Property wherestring As String = " where s.VorgangsID=w.vorgangsid "
    Public Sub getWiedervorlageDT(ByVal BearbeiterID As Integer)
        Dim bearbeiterString$ = ""
        Try
            If BearbeiterID = 0 Then
                bearbeiterString$ = ""
            Else
                bearbeiterString$ = String.Format(" and lower(w.Bearbeiterid)={0} ", BearbeiterID)
            End If
            myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL =
             String.Format("{0}{1}{2}{3} and s.VorgangsID=v.vorgangsid  and w.erledigt < 1  order by w.Datum asc",
                           selectString, fromstring, wherestring, bearbeiterString$)
            '" and w.erledigt=false " & _
            l(String.Format("  getWiedervorlageDT: {0}{1}", vbCrLf, myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL))
            l("Hinweis: " & myGlobalz.sitzung.DBWiedervorlageREC.getDataDT())
        Catch ex As Exception
            nachricht_und_Mbox(String.Format("Problem beim getWiedervorlageDT: {0}{1}", vbCrLf, ex))
        End Try
    End Sub


    Public Shared Function getWiedervorlageAkut(ByVal Bearbeiter As String) As Integer
        Dim bearbeiterString As String = ""
        Dim datumstring As String = ""
        Try
            If String.IsNullOrEmpty(Bearbeiter.Trim) OrElse Bearbeiter.Trim.ToLower = "alle" Then
                bearbeiterString$ = ""
            Else
                bearbeiterString$ = String.Format(" and lower(w.Bearbeiter)='{0}' ", Bearbeiter.ToLower)
            End If
            datumstring = clsDBtools.makeDBdatumsString(Now, myGlobalz.wiedervorlage_MYDB.dbtyp)

            l("getWiedervorlageAkut: dbtyp, dtumsformat: " & myGlobalz.sitzung.DBWiedervorlageREC.mydb.dbtyp & datumstring)
            myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL =
            String.Format(" SELECT w.VorgangsID,w.Datum,s.Beschreibung as Beschreibung, w.Erledigtam as ErledigtAm, w.Erledigt as Erledigt,w.ToDo as ToDo,w.WartenAuf as WartenAuf ,w.Bearbeiter  as Bearbeiter " +
                          " FROM " & CLstart.myViewsNTabs.tabWV & "  w , " & CLstart.myViewsNTabs.tabStammdaten & " s " +
                          " where s.VorgangsID=w.vorgangsid {0} and w.datum < {1} and w.erledigt < 1 " +
                          " order by Datum asc",
                          bearbeiterString$,
                          datumstring)

            l("Hinweis: " & myGlobalz.sitzung.DBWiedervorlageREC.getDataDT())
            l(String.Format("Akute Wiedervorlagen: {0}{1}", vbCrLf, myGlobalz.sitzung.DBWiedervorlageREC.mydb.SQL))
            l("Akute Wiedervorlagen: " & myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows.Count)
            Return myGlobalz.sitzung.DBWiedervorlageREC.dt.Rows.Count
        Catch ex As Exception
            nachricht_und_Mbox(String.Format("Problem beim getWiedervorlageAkut: {0}{1}", vbCrLf, ex))
            Return -1
        End Try
    End Function

    Shared Function ggfTerminNachOutlookUebernehmen(ByVal wv As clsWiedervorlage, ByVal ereig As clsEreignis,
                                                    ByVal nachOutlookUebernehmen As Boolean) As Boolean
        If Not nachOutlookUebernehmen Then Return True
        l("ggfTerminNachOutlookUebernehmen -------------------------------------")

        Dim start As New Date(Year(wv.datum), Month(wv.datum), Day(wv.datum), 10, 0, 0, 0)
        Dim neuu As New MeinOutlook
        Dim erfolg As Boolean = neuu.OutlookTerminPersoenlich(String.Format("Wiedervorlage: Vorgang {0}, {1}", myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung),
                                                   ereig.Beschreibung,
                                                   start,
                                                   60,
                                                     True, False, False,
                                                   True)
        If erfolg Then
            MsgBox("Der Termin wurde im persönlichen Kalender als ganztägiges Ereignis angelegt.")
        Else
            MsgBox("Der Termin konnte nicht angelegt werden.")
        End If
        neuu = Nothing
        Return erfolg
    End Function



    Public Shared Function WVneuOKExtracted(ByVal zielvorgangsid As Integer) As Boolean
        Dim lWV_NEUobj2DBOk As Boolean = WV_NEUobj2DBOk(myGlobalz.sitzung.aktWiedervorlage)
        If lWV_NEUobj2DBOk Then
            myGlobalz.sitzung.Ereignismodus = "neu"

            clsWVTOOLS.WVneuOKExtracted(myGlobalz.sitzung.aktWiedervorlage, 0)
            myGlobalz.sitzung.aktEreignis.typnr = 2
            clsEreignisTools.NeuesEreignisSpeichern_alleDB(zielvorgangsid, "neu", myGlobalz.sitzung.aktEreignis) '            If clsEreignisDB.Neu_speichern_Ereignis() Then nachricht("Daten wurden gespeichert!")
            CLstart.myc.aLog.komponente = "Wiedervorlage" : CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID & ": neu angelegt" : CLstart.myc.aLog.log()

            Return True
        Else
            nachricht_und_Mbox("Problem beim Abspeichern der Wiedervorlage")
            Return False
        End If
    End Function

    Shared Function WV_NEUobj2DBOk(ByVal wv As clsWiedervorlage) As Boolean
        Return If(wv.createWV() > 0, True, False)
    End Function
End Class
