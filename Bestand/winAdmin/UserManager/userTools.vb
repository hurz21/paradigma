Imports System.Data

Module userTools

    Sub initbearbeiterDT()
        ' Dim sql$ = "select LOWER(INITIAL_) as ""INITIALE"",NAME from Bearbeiter where aktiv=1 order by abteilung, NAME asc"
        Dim sql$ = "select * from Bearbeiter order by  aktiv desc, nachNAME asc"
        myGlobalz.sitzung.BearbeiterREC.dt = userTools.initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql$).Copy
    End Sub
    Public Function initKollegenDT() As DataTable
        Dim hinweis As String
        myGlobalz.sitzung.tempREC.mydb.Schema = "paradigma"
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "bearbeiter"
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT bearbeiterid as Id,initial_ as Titel, concat(NACHNAME,vorname) as Name FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
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

    Public Function initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(ByVal sql As String) As DataTable
        myGlobalz.sitzung.BearbeiterREC.mydb.Tabelle = "bearbeiter"
        myGlobalz.sitzung.BearbeiterREC.mydb.SQL = sql
        nachricht(myGlobalz.sitzung.BearbeiterREC.getDataDT)
        Return myGlobalz.sitzung.BearbeiterREC.dt
    End Function


    Function initBearbeiterByUserid_ausParadigmadb(ByRef usi As clsBearbeiter, ByVal spalteKey As String, ByVal spaltevalue As String) As Boolean
        'Todo Diese funktion sollte auf LINQ umgestellt werden
        Dim lokrec As New DataTable
        Dim sql As String = ""
        Dim errorinfo As String = ""
        Try
            errorinfo = errorinfo & " vorsql"
            sql = String.Format("select * from  bearbeiter where lower({0})='{1}' or kuerzel1='{1}' or lower(initial_)='{1}'", spalteKey, spaltevalue$.ToLower)
            errorinfo = errorinfo & " sql: " & sql
            lokrec = initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql).Copy
            errorinfo = errorinfo & "nach lokrec "
            If lokrec.Rows.Count < 1 Then
                nachricht("B FEHLER User ist unbekannt lokrec.Rows.Count < 1: " & usi.username & vbCrLf & " Bitte informieren sie den Admin. errorinfo: " & errorinfo)
                Return False
            End If
            usi.ID = CInt(clsDBtools.fieldvalue(lokrec.Rows(0).Item("BEARBEITERID")))
            usi.username = clsDBtools.fieldvalue(lokrec.Rows(0).Item("username"))
            usi.Namenszusatz = clsDBtools.fieldvalue(lokrec.Rows(0).Item("namenszusatz"))
            usi.Name = clsDBtools.fieldvalue(lokrec.Rows(0).Item("nachname"))
            usi.Vorname = clsDBtools.fieldvalue(lokrec.Rows(0).Item("vorname"))
            usi.Rang = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rang"))
            usi.Rites = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rites"))
            usi.STDGRANTS = clsDBtools.fieldvalue(lokrec.Rows(0).Item("STDGRANTS"))
            usi.Kontakt.elektr.Telefon1 = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Telefon"))
            usi.Kontakt.elektr.Fax1 = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Fax"))
            usi.Kuerzel2Stellig = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Kuerzel1"))
            usi.Kontakt.elektr.Email = clsDBtools.fieldvalue(lokrec.Rows(0).Item("email"))
            usi.Bemerkung = clsDBtools.fieldvalue(lokrec.Rows(0).Item("abteilung"))

            usi.Rolle = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rolle"))
            usi.Status = CInt(clsDBtools.fieldvalue(lokrec.Rows(0).Item("AKTIV")))
            usi.ExpandHeaderInSachgebiet = clsDBtools.fieldvalue(lokrec.Rows(0).Item("ExpandHeaderInSachgebiet"))
            usi.Initiale = usi.getInitial
            ' usi.Kassenkonto = clsDBtools.fieldvalue(lokrec.Rows(0).Item("KASSENKONTO"))
            Return True
        Catch ex As Exception
            nachricht("FEHLER User ist unbekannt: " & usi.username & vbCrLf & " Bitte informieren sie den Admin. errorinfo: " & errorinfo)
            Return False
        Finally
            lokrec.Dispose()
        End Try
    End Function

    Sub setzeAktuellenBearbeiter()
        Dim bearbeiterauswahlbox = New WinBearbeiterauswahl
        bearbeiterauswahlbox.ShowDialog()
        If Not String.IsNullOrEmpty(bearbeiterauswahlbox.auswahlInitiale) Then
            myGlobalz.sitzung.aktBearbeiter.username = bearbeiterauswahlbox.auswahlUSERNAME.ToString
            myGlobalz.sitzung.aktBearbeiter.Name = bearbeiterauswahlbox.auswahlNAchname.ToString
            myGlobalz.sitzung.aktBearbeiter.Rang = bearbeiterauswahlbox.auswahlRang.ToString
            myGlobalz.sitzung.aktBearbeiter.Vorname = bearbeiterauswahlbox.auswahlVorname.ToString
            myGlobalz.sitzung.aktBearbeiter.Initiale = bearbeiterauswahlbox.auswahlInitiale.ToString
        End If
    End Sub

End Module
