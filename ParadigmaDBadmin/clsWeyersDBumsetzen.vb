Imports System.Data
Public Class clsWeyersDBumsetzen
  Public mylog As New clsLogging("c:\paraadmin.log")
  Public ipersonen, ikontakte, ianschriften, ibankkonten, iorg As Integer
  Public ipersonWiederverwendet%, iorgWiederverwendet%, ianschriftWiederverwendet%
  Public Const weyers$ = "WeyG"

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

  Private Shared Sub quellDBdefnierenUndGenerieren(ByRef weyersdbPersonalRec As clsDBspecMDB)
    weyersdbPersonalRec.mydb.MySQLServer = ""
    weyersdbPersonalRec.mydb.dbtyp = "mdb"
    weyersdbPersonalRec.mydb.Schema = "d:\Adressen.mdb"
    weyersdbPersonalRec.mydb.username = ""
    weyersdbPersonalRec.mydb.password = ""
    weyersdbPersonalRec.mydb.Tabelle = "adressen"
    weyersdbPersonalRec.mydb.SQL = "select * from " & weyersdbPersonalRec.mydb.Tabelle & ""
    Dim hinweis$ = weyersdbPersonalRec.getDataDT
  End Sub

  Sub fuellePersonenDB()
    'Dim logdatei$ = "c:\paraadmin.log"
    'mylog = New clsLogging(logdatei$)
    Dim weyersbPersonalRec As New clsDBspecMDB
    Dim paraPersnalREC As New clsDBspecMYSQL
    quellDBdefnierenUndGenerieren(weyersbPersonalRec)
    zielDBdefnierenUndGenerieren(paraPersnalREC)
    kopieren(weyersbPersonalRec, paraPersnalREC, "delete") '"delete")
    'System.Diagnostics.Process.Start(logdatei$)
  End Sub

  Public Shared Function kontaktSpeichern&(ByVal zielREC As clsDBspecMYSQL, ByRef newid&, ByVal aktkontakt As Kontaktdaten, ByVal mylog As clsLogging)
    Dim anzahlTreffer&
    zielREC.mydb.SQL = _
      "insert into " & " kontaktdaten " & " set " & _
           " gesellfunktion='" & aktkontakt.GesellFunktion & "'" & _
           ",FFtelefon1='" & aktkontakt.elektr.Telefon1 & "'" & _
           ",FFtelefon2='" & aktkontakt.elektr.Telefon2 & "'" & _
           ",FFFax1='" & aktkontakt.elektr.Fax1 & "'" & _
           ",FFFax2='" & aktkontakt.elektr.Fax2 & "'" & _
           ",FFmobilfon='" & aktkontakt.elektr.MobilFon & "'" & _
           ",FFemail='" & aktkontakt.elektr.Email & "'" & _
           ",FFhomepage='" & aktkontakt.elektr.Homepage & "'" & _
           ",Bemerkung='" & aktkontakt.Bemerkung & "'" & _
           ",orgid=" & aktkontakt.OrgID & _
           ",AnschriftID=" & aktkontakt.AnschriftID & _
           ",BankkontoID=" & aktkontakt.BankkontoID & _
           ",quelle='" & weyers$ & "'"
    anzahlTreffer = zielREC.sqlexecute(newid, mylog)
    If anzahlTreffer < 1 Then
      MessageBox.Show("Problem beim Abspeichern:" & zielREC.mydb.SQL)
      mylog.log("problem in sql:" & vbCrLf & _
       zielREC.mydb.SQL)
    End If
    Return newid
  End Function

  Public Shared Function orgSpeichern&(ByVal zielREC As clsDBspecMYSQL, ByRef newid&, ByVal aktkontakt As Kontaktdaten, ByVal mylog As clsLogging)
    Dim anzahlTreffer&
    zielREC.mydb.SQL = _
      "insert into " & " organisation " & " set " & _
           " name='" & aktkontakt.Org.Name & "'" & _
           ",zusatz='" & aktkontakt.Org.Zusatz & "'" & _
           ",typ1='" & aktkontakt.Org.Typ1 & "'" & _
           ",typ2='" & aktkontakt.Org.Typ2 & "'" & _
           ",eigentuemer='" & aktkontakt.Org.Eigentuemer & "'" & _
           ",bemerkung='" & aktkontakt.Org.Bemerkung & "'" & _
           ",anschriftid=" & aktkontakt.AnschriftID & _
           ",quelle='" & weyers$ & "'"
    anzahlTreffer = zielREC.sqlexecute(newid, mylog)
    If anzahlTreffer < 1 Then
      MessageBox.Show("Problem beim Abspeichern:" & zielREC.mydb.SQL)
      mylog.log("problem in sql:" & vbCrLf & _
       zielREC.mydb.SQL)
    End If
    Return newid
  End Function

  Public Shared Function anschriftSpeichern&(ByVal zielREC As clsDBspecMYSQL, ByRef newid&, ByVal aktkontakt As Kontaktdaten, ByVal mylog As clsLogging)
    Dim anzahlTreffer&
    zielREC.mydb.SQL = _
      "insert into " & " anschrift " & " set " & _
           " gemeindeName='" & aktkontakt.Anschrift.Gemeindename & "'" & _
           ",strasse='" & aktkontakt.Anschrift.Strasse & "'" & _
           ",hausnr='" & aktkontakt.Anschrift.Hausnr & "'" & _
           ",plz='" & aktkontakt.Anschrift.PLZ & "'" & _
           ",postfach='" & aktkontakt.Anschrift.Postfach & "'" & _
           ",bemerkung='" & aktkontakt.Anschrift.Bemerkung & "'" & _
            ",quelle='" & weyers$ & "'"
    anzahlTreffer = zielREC.sqlexecute(newid, mylog)
    If anzahlTreffer < 1 Then
      MessageBox.Show("Problem beim Abspeichern:" & zielREC.mydb.SQL)
      mylog.log("problem in sql:" & vbCrLf & _
       zielREC.mydb.SQL)
    End If
    Return newid
  End Function

  Private Function bankkontoSpeichern&(ByVal zielREC As clsDBspecMYSQL, ByRef newid&, ByVal aktkontakt As Kontaktdaten)
    Dim anzahlTreffer&
    zielREC.mydb.SQL = _
      "insert into " & " bankverbindung " & " set " & _
           " Name='" & aktkontakt.Bankkonto.Name & "'" & _
           ",blz='" & aktkontakt.Bankkonto.BLZ & "'" & _
           ",kontonr='" & aktkontakt.Bankkonto.KontoNr & "'" & _
           ",quelle='" & weyers$ & "'"
    anzahlTreffer = zielREC.sqlexecute(newid, mylog)
    If anzahlTreffer < 1 Then
      MessageBox.Show("Problem beim Abspeichern:" & zielREC.mydb.SQL)
      mylog.log("problem in sql:" & vbCrLf & _
       zielREC.mydb.SQL)
    End If
    Return newid
  End Function

  Public Shared Function personSpeichern&(ByVal zielREC As clsDBspecMYSQL, _
                                          ByRef newid&, _
                                          ByVal aktperson As Person, _
                                          ByVal mylog As clsLogging, _
                                          ByVal aktkontakt As Kontaktdaten)
    Dim anzahlTreffer&
    zielREC.mydb.SQL = _
      "insert into " & " personen " & " set " & _
           " Name='" & aktperson.Name & "'" & _
           ",Vorname='" & aktperson.Vorname & "'" & _
           ",Namenszusatz='" & aktperson.Namenszusatz & "'" & _
           ",FDkurz='" & aktkontakt.Org.Zusatz & "'" & _
           ",quelle='" & weyers$ & "'" & _
           ",Anrede='" & aktperson.Anrede & "'"
    anzahlTreffer = zielREC.sqlexecute(newid, mylog)
    If anzahlTreffer < 1 Then
      MessageBox.Show("Problem beim Abspeichern:" & zielREC.mydb.SQL)
      mylog.log("problem in sql:" & vbCrLf & _
       zielREC.mydb.SQL)
    End If
    Return newid
  End Function

  Private Shared Sub loeschenZielDBs(ByVal zielREC As clsDBspecMYSQL, ByVal modus$)
    If modus = "delete" Then
      Dim zr As New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "personen"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "bankverbindung"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "anschrift"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "organisation"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "kontaktdaten"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "person2kontakt"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "person2vorgang"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "person2paraadresse"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
      zielREC.mydb.Tabelle = "person2raumbezug"
      zr = New DBManipulation
      zr.zieltabelleLoeschen(zielREC)
    End If
  End Sub
  Public Shared Function ObjekteAnlegenUndInit(ByRef aktkontakt As Kontaktdaten, ByRef hasPerson As Boolean, ByRef hasORG As Boolean, ByRef hasAnschrift As Boolean, ByRef hasElek As Boolean, ByRef hasKontakt As Boolean, ByRef hasBankkonto As Boolean) As Person
    Dim aktperson As Person
    aktperson = New Person()
    aktkontakt = New Kontaktdaten

    hasPerson = False
    hasORG = False
    hasAnschrift = False
    hasElek = False
    hasKontakt = False
    hasBankkonto = False
    Return aktperson
  End Function
  Sub kopieren(ByVal quelleREC As clsDBspecMDB, ByVal zielREC As clsDBspecMYSQL, ByVal modus$)
    Dim  newid&
    Dim aktperson As Person
    Dim aktkontakt As Kontaktdaten		=nothing
    Dim hasPerson, hasORG, hasAnschrift, hasElek, hasKontakt, hasBankkonto As Boolean
    loeschenZielDBs(zielREC, modus)
    Try
      Dim a% = quelleREC.dt.Rows.Count
      For i = 0 To quelleREC.dt.Rows.Count - 1
        aktperson = ObjekteAnlegenUndInit(aktkontakt, hasPerson, hasORG, hasAnschrift, hasElek, hasKontakt, hasBankkonto)
        vondtaufOBJ(aktperson, aktkontakt, quelleREC, i)

        hasORG = checkifHasOrg(aktkontakt)
        hasAnschrift = checkifHasAnschrift(aktkontakt)
        hasBankkonto = checkifHasBankkonto(aktkontakt)
        hasPerson = checkifHasPerson(aktperson)
        hasKontakt = True  

        If hasBankkonto Then
          aktkontakt.BankkontoID = CInt(bankkontoSpeichern(zielREC, newid, aktkontakt))
          ibankkonten += 1
        Else
          aktkontakt.BankkontoID = 0
        End If

        If hasAnschrift Then
          Dim alteanschriftID% = istAnschriftBekannt(aktkontakt, zielREC)
          If alteanschriftID > 0 Then
            aktkontakt.AnschriftID = alteanschriftID
          Else
            aktkontakt.AnschriftID = CInt(anschriftSpeichern(zielREC, newid, aktkontakt, mylog))
            ianschriften += 1
          End If

        Else
          aktkontakt.AnschriftID = 0
        End If
        If hasORG Then
          Dim alteOrgID% = istOrgBekannt(aktkontakt, zielREC)
          If alteOrgID > 0 Then
            aktkontakt.OrgID = alteOrgID
          Else
            aktkontakt.OrgID = CInt(orgSpeichern(zielREC, newid, aktkontakt, mylog))
            iorg += 1
          End If
        Else
          aktkontakt.OrgID = 0
        End If
        If hasKontakt Then
          aktPerson.Kontakt.KontaktID = CInt(kontaktSpeichern(zielREC, newid, aktkontakt, mylog))

        Else
          aktPerson.Kontakt.KontaktID = CInt(kontaktSpeichern(zielREC, newid, aktkontakt, mylog))
          ikontakte += 1
        End If
        If hasPerson Then
          Dim personenID% = 0
          Dim altePersonenID% = istPersonBekannt(aktperson, zielREC)
          If altePersonenID% > 0 Then
            personenID% = altePersonenID%
          Else
            If aktkontakt.Org.Zusatz.Length > 40 Then
              aktkontakt.Org.Zusatz = aktkontakt.Org.Zusatz.Substring(0, 40)
            End If

            personenID% = CInt(personSpeichern(zielREC, newid, aktperson, mylog, aktkontakt))
            ipersonen += 1
          End If
          Dim koppelungsID% = Koppelung_Person_Kontakt(personenID%, aktPerson.Kontakt.KontaktID, zielREC, mylog)
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
      mylog.log("problem in sql:" & vbCrLf & hineis$ & _
       zielREC.mydb.SQL)
      MessageBox.Show(ex.ToString)
    End Try
  End Sub

  Shared Sub vondtaufOBJ(ByRef aktperson As Person, ByRef aktkontakt As Kontaktdaten, _
          ByVal quellerec As db_grundfunktionen, _
           ByVal i As Integer)
    aktperson.Name = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("nachName"))
    aktperson.Vorname = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Vorname"))
    aktperson.Namenszusatz = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Titel"))
    aktperson.Anrede = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("anrede"))
    aktperson.Name = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("nachName"))
    aktperson.Bemerkung = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Bemerkungen"))
    aktperson.Quelle = weyers
    aktkontakt = New Kontaktdaten

    aktkontakt.GesellFunktion = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Funktion"))
    aktkontakt.Bemerkung = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Verband1"))
    aktkontakt.Quelle = weyers

    aktkontakt.Org.Name = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Organisation"))
    aktkontakt.Org.Zusatz = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Zusatz"))
    aktkontakt.Org.Typ1 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Schulform"))
    aktkontakt.Org.Quelle = weyers

    aktkontakt.Anschrift.Strasse = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Straße"))
    aktkontakt.Anschrift.Hausnr = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("haus-Nr"))
    Try
      aktkontakt.Anschrift.PLZ = CInt(clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("plz")))
    Catch ex As Exception
      aktkontakt.Anschrift.PLZ = 0
    End Try

    aktkontakt.Anschrift.Gemeindename = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("ort"))
    aktkontakt.Anschrift.Quelle = weyers

    aktkontakt.elektr.Telefon1 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Telefon geschäftlich"))
    aktkontakt.elektr.Fax1 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Fax geschäftlich"))
    aktkontakt.elektr.Telefon2 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Telefon privat"))
    aktkontakt.elektr.Fax2 = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Fax geschäftlich"))
    aktkontakt.elektr.MobilFon = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("handy"))
    aktkontakt.elektr.Email = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("E-Mail"))
    aktkontakt.elektr.Homepage = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Internet"))
    aktkontakt.elektr.Quelle = weyers

    aktkontakt.Bankkonto.Name = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Bank"))
    aktkontakt.Bankkonto.BLZ = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("BLZ"))
    aktkontakt.Bankkonto.KontoNr = clsDBtools.fieldvalue(quellerec.dt.Rows(i).Item("Konto"))
    'aktkontakt.Bankkonto.Titel = clsDBtools.fieldvalue(quelleREC.dt.Rows(i).Item("Titel"))
  End Sub

  Shared Function checkifHasPerson(ByVal aktperson As Person) As Boolean
    If String.IsNullOrEmpty(aktperson.Name) Then
      Return False
    End If
    Return True
  End Function

  Shared Function checkifHasKontakt(ByVal aktkontakt As Kontaktdaten, ByVal hasORG As Boolean) As Boolean

    If String.IsNullOrEmpty(aktkontakt.elektr.Telefon1) And _
       String.IsNullOrEmpty(aktkontakt.elektr.Email) And _
       String.IsNullOrEmpty(aktkontakt.elektr.MobilFon) And _
       String.IsNullOrEmpty(aktkontakt.elektr.Fax1) Then
      If hasORG Then
        Return True
      Else
        Return False
      End If
    End If
    Return True
  End Function
  Shared Function checkifHasOrg(ByVal aktkontakt As Kontaktdaten) As Boolean
    If String.IsNullOrEmpty(aktkontakt.Org.Name) And String.IsNullOrEmpty(aktkontakt.Org.Zusatz) Then
      Return False
    End If
    Return True
  End Function
  Shared Function checkifHasAnschrift(ByVal aktkontakt As Kontaktdaten) As Boolean
    If String.IsNullOrEmpty(aktkontakt.Anschrift.Gemeindename) Then
      Return False
    End If
    Return True
  End Function

  Function checkifHasBankkonto(ByVal aktkontakt As Kontaktdaten) As Boolean
    If String.IsNullOrEmpty(aktkontakt.Bankkonto.KontoNr) Or _
      String.IsNullOrEmpty(aktkontakt.Bankkonto.BLZ) Then
      Return False
    End If
    Return True
  End Function

  Function istPersonBekannt(ByVal p As Person, ByVal zielREC As clsDBspecMYSQL) As Integer
    Dim lokrec As New clsDBspecMYSQL
    lokrec = CType(zielREC.Clone, clsDBspecMYSQL)		 
    lokrec.mydb.SQL = "select personenID from personen " & _
       " where name='" & p.Name & "'" & _
       " and vorname='" & p.Vorname & "'" & _
       " and namenszusatz='" & p.Namenszusatz & "'"

    Dim hinweis$ = lokrec.getDataDT()

    If lokrec.dt.Rows.Count < 1 Then
      'nicht vorhanden
      Return 0
    Else
      ipersonWiederverwendet += 1
      Return CInt(lokrec.dt.Rows(0).Item(0))
    End If
  End Function

  Function istOrgBekannt(ByVal o As Kontaktdaten, ByVal zielREC As clsDBspecMYSQL) As Integer
    Dim lokrec As New clsDBspecMYSQL
    lokrec = CType(zielREC.Clone, clsDBspecMYSQL)
    lokrec.mydb.SQL = "select orgID from organisation " & _
             " where name='" & o.Org.Name & "'" & _
             " and zusatz='" & o.Org.Zusatz & "'"
    Dim hinweis$ = lokrec.getDataDT()
    If lokrec.dt.Rows.Count < 1 Then
      Return 0
    Else
      iorgWiederverwendet += 1
      Return CInt(lokrec.dt.Rows(0).Item(0))
    End If
  End Function

  Function istAnschriftBekannt(ByVal o As Kontaktdaten, ByVal zielREC As clsDBspecMYSQL) As Integer
    Dim lokrec As New clsDBspecMYSQL
    lokrec = CType(zielREC.Clone, clsDBspecMYSQL)			 
    lokrec.mydb.SQL = "select anschriftID from anschrift " & _
       " where gemeindename='" & o.Anschrift.Gemeindename & "'" & _
       " and Strasse='" & o.Anschrift.Strasse & "'" & _
       " and Postfach='" & o.Anschrift.Postfach & "'" & _
       " and hausnr='" & o.Anschrift.Hausnr & "'"
    Dim hinweis$ = lokrec.getDataDT()
    If lokrec.dt.Rows.Count < 1 Then
      Return 0
    Else
      ianschriftWiederverwendet += 1
      Return CInt(lokrec.dt.Rows(0).Item(0))
    End If
  End Function

  Public Shared Function Koppelung_Person_Kontakt(ByVal personenID%, ByVal kontaktID%, ByVal zielREC As clsDBspecMYSQL, ByVal mylog As clsLogging) As Integer
    Dim anzahlTreffer&
    Dim newid& = -1
    Try
      With zielREC
        .mydb.Tabelle = "Person2kontakt"
        .mydb.SQL = _
         "insert into " & .mydb.Tabelle & " set " & _
           " kontaktID=" & kontaktID% & _
           ",PersonenID=" & personenID%
        anzahlTreffer = .sqlexecute(newid, mylog)
        If anzahlTreffer < 1 Then
          MessageBox.Show("Problem beim Abspeichern:" & .mydb.SQL)
          Return -1
        Else
          Return CInt(newid)
        End If
      End With
    Catch ex As Exception
      MessageBox.Show("Problem beim Abspeichern: " & ex.ToString)
      Return -2
    End Try
  End Function
End Class
