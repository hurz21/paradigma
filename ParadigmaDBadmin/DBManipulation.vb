Imports System.Windows.Media.Drawing
Public Class DBManipulation
  Public Const weyers$ = "WeyG"
  Public mylog As New clsLogging("c:\paraadmin.log")
  Public ipersonen, ikontakte, ianschriften, ibankkonten, iorg As Integer
  Public ipersonWiederverwendet%, iorgWiederverwendet%, ianschriftWiederverwendet%


  Private Shared Sub zielDBdefnierenUndGenerieren(ByRef paraPersnalREC As clsDBspecMYSQL)
    paraPersnalREC.mydb.MySQLServer = "KIS"
    paraPersnalREC.mydb.dbtyp = "mysql"
    paraPersnalREC.mydb.Schema = "paradigma"
    paraPersnalREC.mydb.username = "root"
    paraPersnalREC.mydb.password = "lkof4"
    paraPersnalREC.mydb.Tabelle = "personen"
    'paraPersnalREC.mydb.SQL = "select * from " & paraPersnalREC.mydb.Tabelle & _
    ' " where fdkurz='" & "67" & "'"
    'Dim hinweis$ = paraPersnalREC.getDataDT
  End Sub
  Private Shared Sub quellDBdefnierenUndGenerieren(ByRef raumdbPersonalRec As clsDBspecMYSQL)
    raumdbPersonalRec.mydb.MySQLServer = "KIS"
    raumdbPersonalRec.mydb.dbtyp = "mysql"
    raumdbPersonalRec.mydb.Schema = "raumdatenbank"
    raumdbPersonalRec.mydb.username = "root"
    raumdbPersonalRec.mydb.password = "lkof4"
    raumdbPersonalRec.mydb.Tabelle = "personal"
    ''raumdbPersonalRec.mydb.SQL = "select * from " & raumdbPersonalRec.mydb.Tabelle & _
    ' " where fdkurz='" & "67" & "'"
    raumdbPersonalRec.mydb.SQL = "select * from " & raumdbPersonalRec.mydb.Tabelle
    Dim hinweis$ = raumdbPersonalRec.getDataDT
  End Sub
  Sub fuellePersonenDB()
    Dim logdatei$ = "d:\paraadmin.log"
    mylog = New clsLogging(logdatei$)
    Dim quelleREC As New clsDBspecMYSQL
    Dim paraPersnalREC As New clsDBspecMYSQL
    quellDBdefnierenUndGenerieren(quelleREC)
    zielDBdefnierenUndGenerieren(paraPersnalREC)
    kopieren(quelleREC, paraPersnalREC, "delete")
    System.Diagnostics.Process.Start(mylog.Streamfilepub)
  End Sub
  Sub kopieren(ByVal quelleREC As clsDBspecMYSQL, ByVal zielREC As clsDBspecMYSQL, ByVal modus$)
    Dim  newid&
    Dim aktperson As Person
    Dim aktkontakt As Kontaktdaten	 =nothing
    Dim hasPerson, hasORG, hasAnschrift, hasElek, hasKontakt, hasBankkonto As Boolean
    
    Dim personenID% = 0
    Dim anschriftkreisoffenbach As Integer = -1

    If modus = "delete" Then
      '  zieltabelleLoeschen(zielREC)
    End If
    Try
      For i = 0 To quelleREC.dt.Rows.Count - 1
        aktperson = clsWeyersDBumsetzen.ObjekteAnlegenUndInit(aktkontakt, hasPerson, hasORG, hasAnschrift, hasElek, hasKontakt, hasBankkonto)
        vondtaufOBJ(aktperson, aktkontakt, quelleREC, i)

        hasORG = clsWeyersDBumsetzen.checkifHasOrg(aktkontakt)
        hasAnschrift = clsWeyersDBumsetzen.checkifHasAnschrift(aktkontakt)
        hasPerson = clsWeyersDBumsetzen.checkifHasPerson(aktperson)

        If anschriftkreisoffenbach > 0 Then
          aktkontakt.AnschriftID = anschriftkreisoffenbach
        Else
          aktkontakt.AnschriftID = CInt(clsWeyersDBumsetzen.anschriftSpeichern(zielREC, newid, aktkontakt, mylog))
          anschriftkreisoffenbach = aktkontakt.AnschriftID
          ianschriften += 1
        End If

        If hasORG Then
          aktkontakt.OrgID = CInt(clsWeyersDBumsetzen.orgSpeichern(zielREC, newid, aktkontakt, mylog))
          iorg += 1
        End If

        If clsWeyersDBumsetzen.checkifHasKontakt(aktkontakt, hasORG) Then
          aktperson.Kontakt.KontaktID = CInt(clsWeyersDBumsetzen.kontaktSpeichern(zielREC, newid, aktkontakt, mylog))
          ikontakte += 1
        End If
        If clsWeyersDBumsetzen.checkifHasPerson(aktperson) Then
          personenID% = CInt(clsWeyersDBumsetzen.personSpeichern(zielREC, newid, aktperson, mylog, aktkontakt))
          ipersonen += 1
          Dim koppelungsID% = clsWeyersDBumsetzen.Koppelung_Person_Kontakt(personenID%, aktperson.Kontakt.KontaktID, zielREC, mylog)
        Else
          Debug.Print(aktperson.Name)
        End If

      Next

      mylog.log("Filter: " & quelleREC.mydb.SQL)
      mylog.log(quelleREC.dt.Rows.Count & " Records gelesen!")
      mylog.log("-----------------------------------------------")
      mylog.log("Bankkonten: " & ibankkonten)
      mylog.log("Anschriften: " & ianschriften)
      mylog.log("Organisatio: " & iorg)
      mylog.log("ikontakte  : " & ikontakte)
      mylog.log("ipersonen: " & ipersonen)
      mylog.log("ipersonWiederverwendet: " & ipersonWiederverwendet)
      mylog.log("iorgWiederverwendet: " & iorgWiederverwendet)
      mylog.log("ianschriftWiederverwendet: " & ianschriftWiederverwendet)

      System.Diagnostics.Process.Start(mylog.Streamfilepub)
      End
    Catch ex As Exception
      Dim hineis$ = ex.ToString
      MessageBox.Show("problem in sql:" & vbCrLf & _
    zielREC.mydb.SQL)
    End Try
  End Sub

  Shared Sub vondtaufOBJ(ByRef aktperson As Person, ByRef aktkontakt As Kontaktdaten, _
    ByVal quellerec As db_grundfunktionen, _
     ByVal i As Integer)
    Try
      aktperson.Name = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Name"))
      aktperson.Vorname = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Vorname"))
      aktperson.Namenszusatz = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Titel"))
      aktperson.Bemerkung = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Kommentar"))
      aktperson.Quelle = "ADMIN_KIS"
      aktkontakt = New Kontaktdaten

      aktkontakt.GesellFunktion = "Mitarbeiter/in"
      aktkontakt.Quelle = weyers

      aktkontakt.Org.Name = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Arbeitgeber"))
      aktkontakt.Org.Zusatz = "Fachdienst: " & clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("fdkurz"))
      aktkontakt.Org.Typ1 = "Gebäudenr: " & clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Objekt"))
      aktkontakt.Org.Typ1 = "RaumNr: " & clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("raumid"))
      aktkontakt.Org.Quelle = aktperson.Quelle

      aktkontakt.Anschrift.Strasse = "Werner Hilpert Straße"
      aktkontakt.Anschrift.Hausnr = "1"

      Try
        aktkontakt.Anschrift.PLZ = 63128
      Catch ex As Exception
        aktkontakt.Anschrift.PLZ = 0
      End Try

      aktkontakt.Anschrift.Gemeindename = "Dietzenbach"
      aktkontakt.Anschrift.Quelle = aktperson.Quelle

      aktkontakt.elektr.Telefon1 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Telefon_1"))
      aktkontakt.elektr.Fax1 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Fax"))
      aktkontakt.elektr.Telefon2 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Telefon_2"))
      'aktkontakt.elektr.Fax2 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Fax geschäftlich"))
      aktkontakt.elektr.MobilFon = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("mobil"))
      aktkontakt.elektr.Email = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("email"))
      aktkontakt.elektr.Homepage = "www.kreis-offenbach.de"
      aktkontakt.elektr.Quelle = aktperson.Quelle
    Catch ex As Exception
      MsgBox(ex.ToString)
    End Try
  End Sub

  Public  Sub zieltabelleLoeschen(ByVal zielREC As clsDBspecMYSQL)
    Dim anzahlTreffer&, newid&
    zielREC.mydb.SQL = "delete from " & zielREC.mydb.Tabelle
    anzahlTreffer = zielREC.sqlexecute(newid, mylog)
    If anzahlTreffer < 1 Then
      mylog.log("Problem beim Löschen des Tabelleinhaltes: es wurden keine objekte gelöscht ggf. war die tabelle leer" & zielREC.mydb.SQL)
      'mylog.log("problem in sql:" & vbCrLf & _
      ' zielREC.mydb.SQL)
    End If
    zielREC.mydb.SQL = "ALTER TABLE " & zielREC.mydb.Tabelle & "  AUTO_INCREMENT = 1;"
    anzahlTreffer = zielREC.sqlexecute(newid, mylog)
    'If anzahlTreffer < 1 Then
    '	MessageBox.Show("Problem beim ALTER der tabelle: es wurden keine objekte gelöscht ggf. war die tabelle leer" & zielREC.mydb.SQL)
    '	'mylog.log("problem in sql:" & vbCrLf & _
    '	' zielREC.mydb.SQL)
    'End If
  End Sub
End Class
