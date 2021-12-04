Imports System.Data
Imports paradigmaDetail

Module userTools
    Friend Function getBearbeiterCollectionBA(sql As String) As List(Of clsBearbeiter)
        Dim colBearbeiter As New List(Of clsBearbeiter)
        Dim usi As New clsBearbeiter
        l("getBearbeiterCollectionBA---------")
        Try
            'userTools.initbearbeiterDT(sql)
            Dim hinweis As String = ""
            myGlobalz.sitzung.BearbeiterREC.dt = getDT4Query(sql, myGlobalz.sitzung.BearbeiterREC, hinweis)
            l("getBearbeiterCollectionBA---------1:   " & myGlobalz.sitzung.BearbeiterREC.dt.Rows.Count)
            For i = 0 To myGlobalz.sitzung.BearbeiterREC.dt.Rows.Count - 1
                usi = New clsBearbeiter
                bearbeiterDTzuOBJEKTBauaufsicht(usi, myGlobalz.sitzung.BearbeiterREC.dt, i)
                usi.buildtooltip()
                colBearbeiter.Add(usi)
            Next
            l("getBearbeiterCollectionBA---------5")
            Return colBearbeiter
        Catch ex As Exception
            nachricht("Fehler refreshBearbeiterliste ", ex)
            Return colBearbeiter
        End Try
    End Function
    Friend Function getBearbeiterCollection(sql As String) As List(Of clsBearbeiter)
        Dim colBearbeiter As New List(Of clsBearbeiter)
        Dim usi As New clsBearbeiter
        Try
            'userTools.initbearbeiterDT(sql)
            Dim hinweis As String = ""
            myGlobalz.sitzung.BearbeiterREC.dt = getDT4Query(sql, myGlobalz.sitzung.BearbeiterREC, hinweis)
            For i = 0 To myGlobalz.sitzung.BearbeiterREC.dt.Rows.Count - 1
                usi = New clsBearbeiter
                bearbeiterDTzuOBJEKT(usi, myGlobalz.sitzung.BearbeiterREC.dt, i)
                usi.buildtooltip()
                colBearbeiter.Add(usi)
            Next
            Return colBearbeiter
        Catch ex As Exception
            nachricht("Fehler refreshBearbeiterliste ", ex)
            Return colBearbeiter
        End Try
    End Function
    'Sub initbearbeiterDT(sql As String)
    '    ' Dim sql$ = "select LOWER(INITIAL_) as ""INITIALE"",NAME from " & CLstart.myViewsNTabs.tabBearbeiter & "  where aktiv=1 order by abteilung, NAME asc"

    '    'myGlobalz.sitzung.BearbeiterREC.dt = userTools.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql$).Copy
    '    Dim hinweis As String = ""
    '    myGlobalz.sitzung.BearbeiterREC.dt = getDT4Query(sql, myGlobalz.sitzung.BearbeiterREC, hinweis)
    'End Sub
    Public Function initKollegenDT() As DataTable
        Dim hinweis As String
        myGlobalz.sitzung.tempREC.mydb.Schema = "paradigma"
        'myGlobalz.sitzung.tempREC.mydb.Tabelle =CLstart.myViewsNTabs.tabBearbeiter
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT bearbeiterid as Id,initial_ as Titel, concat(NACHNAME,vorname) as Name,anrede FROM " & CLstart.myViewsNTabs.tabBearbeiter & "  " &
                                                  " order by nachname"
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return myGlobalz.sitzung.tempREC.dt
        End If
    End Function

    'Public Function initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(ByVal sql As String) As DataTable
    '    myglobalz.sitzung.BearbeiterREC.mydb.Tabelle =CLstart.myViewsNTabs.tabBearbeiter
    '    myglobalz.sitzung.BearbeiterREC.mydb.SQL = sql
    '    nachricht(myglobalz.sitzung.BearbeiterREC.getDataDT)
    '    Return myglobalz.sitzung.BearbeiterREC.dt
    'End Function

    Function initBearbeiterByUserid_ausParadigmadb(ByRef usi As clsBearbeiter) As Boolean
        'Todo Diese funktion sollte auf LINQ umgestellt werden
        Dim loDT As New DataTable
        Dim sql As String = ""
        Dim errorinfo As String = "" : Dim hinweis As String = ""
        Try
            errorinfo = errorinfo & " vorsql"
            If usi.ID < 1 Then
                sql = "select * from  " & CLstart.myViewsNTabs.tabBearbeiter & "  where lower(username)='" & usi.username.ToLower & "'"
            End If
            errorinfo = errorinfo & " sql: " & sql
            loDT = getDT4Query(sql, myGlobalz.sitzung.BearbeiterREC, hinweis)
            errorinfo = errorinfo & "nach lokrec "
            If loDT.Rows.Count < 1 Then
                nachricht("B FEHLER User ist unbekannt lokrec.Rows.Count < 1: " & usi.username & vbCrLf & " Bitte informieren sie den Admin. errorinfo: " & errorinfo)
                Return False
            End If
            bearbeiterDTzuOBJEKT(usi, loDT, 0)
            If usi.Status < 1 Then
                'MsgBox("User " & usi.username & " ist inaktiv: bitte bei FD-Leitung melden")
                ' Return False
            End If
            usi.Initiale = usi.getInitial
            ' usi.Kassenkonto = clsDBtools.fieldvalue(lokrec.Rows(0).Item("KASSENKONTO"))
            Return True
        Catch ex As Exception
            nachricht("FEHLER User ist unbekannt: " & usi.username & vbCrLf & " Bitte informieren sie den Admin. errorinfo: " & sql & errorinfo)
            Return False
        Finally
            loDT.Dispose()
        End Try
    End Function
    Function initBearbeiterByUserid_ausParadigmadbALT(ByRef usi As clsBearbeiter, ByVal spalteKey As String, ByVal spaltevalue As String) As Boolean
        'Todo Diese funktion sollte auf LINQ umgestellt werden
        Dim loDT As New DataTable
        Dim sql As String = ""
        Dim errorinfo As String = "" : Dim hinweis As String = ""
        Try
            errorinfo = errorinfo & " vorsql"
            errorinfo = errorinfo & " sql: " & sql
            loDT = getDT4Query(sql, myGlobalz.sitzung.BearbeiterREC, hinweis)
            errorinfo = errorinfo & "nach lokrec "
            If loDT.Rows.Count < 1 Then
                nachricht("B FEHLER User ist unbekannt lokrec.Rows.Count < 1: " & usi.username & vbCrLf & " Bitte informieren sie den Admin. errorinfo: " & errorinfo)
                Return False
            End If
            bearbeiterDTzuOBJEKT(usi, loDT, 0)
            If usi.Status < 1 Then
                'MsgBox("User " & usi.username & " ist inaktiv: bitte bei FD-Leitung melden")
                ' Return False
            End If
            usi.Initiale = usi.getInitial
            ' usi.Kassenkonto = clsDBtools.fieldvalue(lokrec.Rows(0).Item("KASSENKONTO"))
            Return True
        Catch ex As Exception
            nachricht("FEHLER User ist unbekannt: " & usi.username & vbCrLf & " Bitte informieren sie den Admin. errorinfo: " & errorinfo)
            Return False
        Finally
            loDT.Dispose()
        End Try
    End Function

    Public Sub bearbeiterDTzuOBJEKTBauaufsicht(ByVal usi As clsBearbeiter, ByVal lokrec As DataTable, ByVal index As Integer)
        'personenid,nachname,vorname,fftelefon1,ffemail,bezirk,orgzusatz 
        usi.ID = CInt(clsDBtools.fieldvalue(lokrec.Rows(index).Item("personenID")))
        'usi.username = clsDBtools.fieldvalue(lokrec.Rows(index).Item("username"))
        'usi.Namenszusatz = clsDBtools.fieldvalue(lokrec.Rows(index).Item("namenszusatz"))
        usi.Name = clsDBtools.fieldvalue(lokrec.Rows(index).Item("nachname"))
        usi.Vorname = clsDBtools.fieldvalue(lokrec.Rows(index).Item("vorname"))
        'usi.Rang = clsDBtools.fieldvalue(lokrec.Rows(index).Item("rang"))
        usi.Raum = clsDBtools.fieldvalue(lokrec.Rows(index).Item("bezirk"))
        'usi.STDGRANTS = clsDBtools.fieldvalue(lokrec.Rows(index).Item("STDGRANTS"))
        usi.Kontakt.elektr.Telefon1 = clsDBtools.fieldvalue(lokrec.Rows(index).Item("ffTelefon1"))
        'usi.Kontakt.elektr.Fax1 = clsDBtools.fieldvalue(lokrec.Rows(index).Item("Fax"))
        'usi.Kuerzel2Stellig = clsDBtools.fieldvalue(lokrec.Rows(index).Item("Kuerzel1"))
        usi.Kontakt.elektr.Email = clsDBtools.fieldvalue(lokrec.Rows(index).Item("ffemail"))
        'usi.Initiale = clsDBtools.fieldvalue(lokrec.Rows(index).Item("INITIAL_"))
        usi.Bemerkung = clsDBtools.fieldvalue(lokrec.Rows(index).Item("orgzusatz"))
        usi.Initiale = usi.Name.Substring(0, 3) & usi.Vorname.Substring(0, 1)
        'usi.Anrede = clsDBtools.fieldvalue(lokrec.Rows(index).Item("anrede"))
        'usi.Rolle = clsDBtools.fieldvalue(lokrec.Rows(index).Item("rolle"))
        'usi.Status = CInt(clsDBtools.fieldvalue(lokrec.Rows(index).Item("AKTIV")))
        'usi.ExpandHeaderInSachgebiet = clsDBtools.fieldvalue(lokrec.Rows(index).Item("ExpandHeaderInSachgebiet"))
    End Sub

    Public Sub bearbeiterDTzuOBJEKT(ByVal usi As clsBearbeiter, ByVal lokrec As DataTable, ByVal index As Integer)
        usi.ID = CInt(clsDBtools.fieldvalue(lokrec.Rows(index).Item("BEARBEITERID")))
        usi.username = clsDBtools.fieldvalue(lokrec.Rows(index).Item("username"))
        usi.Namenszusatz = clsDBtools.fieldvalue(lokrec.Rows(index).Item("namenszusatz"))
        usi.Name = clsDBtools.fieldvalue(lokrec.Rows(index).Item("nachname"))
        usi.Vorname = clsDBtools.fieldvalue(lokrec.Rows(index).Item("vorname"))
        usi.Rang = clsDBtools.fieldvalue(lokrec.Rows(index).Item("rang"))
        usi.Raum = clsDBtools.fieldvalue(lokrec.Rows(index).Item("rites"))
        usi.STDGRANTS = clsDBtools.fieldvalue(lokrec.Rows(index).Item("STDGRANTS"))
        usi.Kontakt.elektr.Telefon1 = clsDBtools.fieldvalue(lokrec.Rows(index).Item("Telefon"))
        usi.Kontakt.elektr.Fax1 = clsDBtools.fieldvalue(lokrec.Rows(index).Item("Fax"))
        usi.Kuerzel2Stellig = clsDBtools.fieldvalue(lokrec.Rows(index).Item("Kuerzel1"))
        usi.Kontakt.elektr.Email = clsDBtools.fieldvalue(lokrec.Rows(index).Item("email"))
        usi.Initiale = clsDBtools.fieldvalue(lokrec.Rows(index).Item("INITIAL_")).ToLower
        usi.Bemerkung = clsDBtools.fieldvalue(lokrec.Rows(index).Item("abteilung"))
        usi.Rolle = clsDBtools.fieldvalue(lokrec.Rows(index).Item("rolle"))
        usi.Status = CInt(clsDBtools.fieldvalue(lokrec.Rows(index).Item("AKTIV")))
        usi.Anrede = (clsDBtools.fieldvalue(lokrec.Rows(index).Item("anrede")))
        usi.ExpandHeaderInSachgebiet = clsDBtools.fieldvalue(lokrec.Rows(index).Item("ExpandHeaderInSachgebiet"))
    End Sub

    'Sub setzeAktuellenBearbeiter()
    '    Dim bearbeiterauswahlbox = New WinBearbeiterauswahl("einzelauswahl")
    '    bearbeiterauswahlbox.ShowDialog()
    '    If Not String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlInitiale) Then
    '        myGlobalz.sitzung.aktBearbeiter.username = bearbeiterauswahlbox.auswahlUSERNAME.ToString
    '        myGlobalz.sitzung.aktBearbeiter.Name = bearbeiterauswahlbox.auswahlNAchname.ToString
    '        myGlobalz.sitzung.aktBearbeiter.Rang = bearbeiterauswahlbox.auswahlRang.ToString
    '        myGlobalz.sitzung.aktBearbeiter.Vorname = bearbeiterauswahlbox.auswahlVorname.ToString
    '        myGlobalz.sitzung.aktBearbeiter.Initiale = bearbeiterauswahlbox.auswahlInitiale.ToString
    '    End If
    'End Sub
    Public Function getAllLockedUsers() As IO.FileInfo()
        Dim di As IO.DirectoryInfo
        Try
            di = IO.Directory.CreateDirectory(initP.getValue("Haupt.LOCKINGFile"))
            Dim templiste As IO.FileInfo()
            templiste = di.GetFiles("*.txt")
            Return templiste
        Catch ex As Exception
            nachricht("fehler in getAllLockedUsers", ex)
            Return Nothing
        End Try
    End Function
    Friend Function getOnlineStatus(colBearbeiter As List(Of clsBearbeiter)) As Boolean
        Dim initiale As IO.FileInfo()
        Dim t As String = ""
        Dim a() As String
        Try
            initiale = getAllLockedUsers()
            For Each user As clsBearbeiter In colBearbeiter
                For i = 0 To initiale.Count - 1
                    'Debug.Print(initiale(i).Name)
                    user.istOnline = False
                    t = initiale(i).Name.Replace(".txt", "")
                    a = t.Split("_"c)
                    If user.Initiale = a(1) Then
                        user.istOnline = True
                        Exit For
                    End If
                Next
            Next
            Return True
        Catch ex As Exception
            nachricht("Fehler in getOnlineStatus: ", ex)
            Return False
        End Try
    End Function

    Friend Function getEmailFromColBearbeiter(colBearbeiter As List(Of clsBearbeiter), userid As Integer) As String
        Try
            For Each user As clsBearbeiter In colBearbeiter
                If user.ID = userid Then
                    Return user.Kontakt.elektr.Email
                    Exit For
                End If
            Next
            Return ""
        Catch ex As Exception
            Return ""
        End Try
    End Function
    Friend Sub MakeKapitelsKontakte(liste As List(Of clsBearbeiter))
        For Each user As clsBearbeiter In liste
            user.Initiale = LIBgemeinsames.clsString.CapitalizeKontakte(user.Initiale)
        Next
    End Sub
End Module
