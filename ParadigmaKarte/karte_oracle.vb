Public Class karte_oracle
    Public Shared Function erzeugeVerwandtenlistezuVorgang(ByVal sql$) As Boolean
        Dim hinweis$
        glob2.nachricht("erzeugeVerwandtenlistezuVorgang --------------------------------")
        myGlobalz.tempREC.mydb.Tabelle = "vorgang2fremdvorgang"
        myGlobalz.tempREC.mydb.SQL = sql$
        hinweis = myGlobalz.tempREC.getDataDT()
        If myGlobalz.tempREC.mycount < 1 Then
            glob2.nachricht("erzeugeVerwandtenlistezuVorgang  Keine beteiligte gespeichert!")
            Return False
        Else
            glob2.nachricht(String.Format("erzeugeVerwandtenlistezuVorgang {0} beteiligte vorhanden", myGlobalz.tempREC.mycount))
            Return True
        End If
    End Function
    Public Shared Sub initRaumbezugsDT_by_SQLstring(ByVal sql$)
        myGlobalz.raumbezugsRec.mydb.Tabelle = "raumbezug"
        myGlobalz.raumbezugsRec.mydb.SQL = sql$
        glob2.nachricht(" hinweis$ = " & myGlobalz.raumbezugsRec.getDataDT())
        If myGlobalz.raumbezugsRec.mycount < 1 Then
            glob2.nachricht("Keine raumbezugsRec gespeichert!")
        Else
            glob2.nachricht(String.Format("{0} raumbezugsRec vorhanden", myGlobalz.raumbezugsRec.mycount))
        End If
    End Sub
    Private Shared Function viaKopplung_RaumbezuegeID_VorgangID(ByVal vorgangsid$) As Boolean
        myGlobalz.tempREC.mydb.Host = myGlobalz.raumbezugsRec.mydb.Host
        myGlobalz.tempREC.mydb.Schema = myGlobalz.raumbezugsRec.mydb.Schema
        myGlobalz.tempREC.mydb.username = myGlobalz.raumbezugsRec.mydb.username
        myGlobalz.tempREC.mydb.password = myGlobalz.raumbezugsRec.mydb.password
        myGlobalz.tempREC.mydb.Tabelle = "Raumbezug2vorgang"     ''& " order by ts desc"
        myGlobalz.tempREC.mydb.SQL = _
         String.Format("SELECT * FROM {0} where VorgangsID={1}", myGlobalz.tempREC.mydb.Tabelle, vorgangsid$)
        glob2.nachricht("hinweis: " & myGlobalz.tempREC.getDataDT())
        If myGlobalz.tempREC.mycount < 1 Then
            glob2.nachricht("Keine Ereignisse gespeichert!")
            Return False
        Else
            glob2.nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.tempREC.mycount))
            Return True
        End If
    End Function
    Public Shared Function initRaumbezugsDT(ByVal vid%) As Boolean
        'zuerst die personenIDs holen	  
        If viaKopplung_RaumbezuegeID_VorgangID(vid.ToString) Then
            myGlobalz.RaumbezugsIDsDT = myGlobalz.tempREC.dt.Copy
            Dim SQL$ = ""
            SQL = glob2.UNION_SQL_erzeugen(myGlobalz.RaumbezugsIDsDT, "raumbezug", 1, "raumbezugsid")
            initRaumbezugsDT_by_SQLstring(SQL$)
            glob2.nachricht("Es konnten  Raumbezuege zu diesem Vorgang gefunden werden!")
            Return True
        Else
            glob2.nachricht("Es konnten keine Raumbezuege zu diesem Vorgang gefunden werden!")
            Return False
        End If
    End Function
    Public Shared Function viaKopplung_DokumentIDs_VorgangID(ByVal vorgangsid$) As Boolean
        myGlobalz.tempREC.mydb.Host = myGlobalz.VorgangREC.mydb.Host
        myGlobalz.tempREC.mydb.Schema = myGlobalz.VorgangREC.mydb.Schema
        myGlobalz.tempREC.mydb.Tabelle = "dokument2vorgang"  ''& " order by ts desc"
        myGlobalz.tempREC.mydb.SQL = _
         String.Format("SELECT * FROM {0} where VorgangsID={1}", myGlobalz.tempREC.mydb.Tabelle, vorgangsid$)
        glob2.nachricht("  hinweis = " & myGlobalz.tempREC.getDataDT())
        If myGlobalz.tempREC.mycount < 1 Then
            glob2.nachricht("Keine Ereignisse gespeichert!")
            Return False
        Else
            glob2.nachricht(String.Format("{0} Dokumente vorhanden", myGlobalz.tempREC.mycount))
            Return True
        End If
    End Function
End Class
