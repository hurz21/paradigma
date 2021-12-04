#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.IO
Imports paradigmaDetail
Module modrechtsdb
    Friend Sub getrechtsobjekte(mycoll As List(Of clsgesetzesManagerDok))
        Dim newone As New clsgesetzesManagerDok
        Try
            For i = 0 To myGlobalz.sitzung.gesetzesdbREC.dt.Rows.Count - 1
                newone = New clsgesetzesManagerDok
                newone.schlagworte = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("schlagworte")).Trim
                newone.artId = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("art")))
                newone.beschreibung = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("beschreibung")).Trim
                newone.dateinameohneext = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("dateinameohneext")).Trim
                newone.ordner = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("ordner")).Trim
                newone.dateityp = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("dateityp")).Trim
                newone.stammid = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("stid")))
                newone.quellentyp = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("quellentyp")).Trim
                newone.userInitial = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("quelle")).Trim
                newone.herkunftId = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("herkunft")))
                newone.wannveroeffentlicht = clsDBtools.fieldvalueDate(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("wannveroeffentlicht"))
                newone.url = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("url")).Trim
                newone.istgueltig = CBool(clsDBtools.toBool(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("istgueltig")))
                'newone.farbnummer = CInt(clsDBtools.fieldvalue(myglobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("farbnummer")))
                newone.sachgebietnr = (clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("SACHGEBIETNR"))).Trim
                newone.sachgebietheader = (clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("SGHEADER"))).Trim
                newone.userInitial = (clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("QUELLE"))).Trim
                newone.originalDateiName = (clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("ORIGINALNAME"))).Trim
                mycoll.Add(newone)
            Next
            Debug.Print(CType(mycoll.Count, String))
        Catch ex As Exception
            nachricht("fehler in getrechtsobjekte: ", ex)
        End Try
    End Sub

    Friend Sub alleSachgebieteZumGesetz(stammid As Integer, bestandsSachgebiete As List(Of AktenzeichenSachgebiet))
        Try
            myGlobalz.sitzung.gesetzesdbREC.mydb.SQL = "select sachgebietnr,sgheader from  t38 as rechtsdb_sachgebiet where stammid=" & stammid
            Dim hinweis As String = ""
            hinweis = myGlobalz.sitzung.gesetzesdbREC.getDataDT()
            bestandsSachgebiete.Clear()
            Dim newAZ As New AktenzeichenSachgebiet
            For i = 0 To myGlobalz.sitzung.gesetzesdbREC.dt.Rows.Count - 1
                newAZ.Zahl = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item(0))
                newAZ.Header = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item(1))
                bestandsSachgebiete.Add(newAZ)
            Next
        Catch ex As Exception
            nachricht("fehler in alleSachgebieteZumGesetz: ", ex)
        End Try
    End Sub

    Friend Function removeAllbutPDF(filenames() As String) As String()
        Dim ipdf As Integer
        For i = 0 To filenames.Count - 1
            If filenames(i).ToLower.EndsWith(".pdf") Then
                ipdf += 1
            End If
        Next
        Dim neufilenames As String()
        If ipdf > 0 Then
            ReDim neufilenames(ipdf - 1)
            For i = 0 To filenames.Count - 1
                If filenames(i).ToLower.EndsWith(".pdf") Then
                    neufilenames(i) = filenames(i)
                End If
            Next
            'If ipdf > 0 Then
            Return neufilenames
            'Else
            '    Return filenames
            'End If
        Else
#Disable Warning BC42104 ' Variable 'neufilenames' is used before it has been assigned a value. A null reference exception could result at runtime.
            Return neufilenames
#Enable Warning BC42104 ' Variable 'neufilenames' is used before it has been assigned a value. A null reference exception could result at runtime.
        End If

    End Function



    Friend Function gesetzesDateiSpeichern(datei As String, sg As String, _gesetz As clsgesetzesManagerDok) As Boolean
        Try
            fillGesetzObj(sg, datei, _gesetz)
            If gesetzesDateiInsArchivKopieren(_gesetz, datei) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("fehler in ", ex)
            Return False
        End Try
    End Function

    Private Sub fillGesetzObj(sg As String, datei As String, _gesetz As clsgesetzesManagerDok)
        Dim fi As IO.FileInfo
        Try
            fi = New IO.FileInfo(datei)
            _gesetz.sachgebietnr = sg
            _gesetz.ordner = getOrdner(sg)
            _gesetz.dateinameohneext = getDateinameohneext(fi).ToLower
            _gesetz.originalDateiName = fi.Name.ToLower
            _gesetz.dateityp = getDateitypt(fi).ToLower
            '_gesetz.fullnameImArchiv = initP.getValue("Rechtsdb.rootdir") &
            '        _gesetz.ordner & "\" &
            '        _gesetz.dateinameohneext &
            '        _gesetz.dateityp
            _gesetz.FullnameImArchiv = _gesetz.getFullnameImArchiv(initP.getValue("Rechtsdb.rootdir"))
            _gesetz.userInitial = myGlobalz.sitzung.aktBearbeiter.Initiale
            fi = Nothing
        Catch ex As Exception
            nachricht("fehler in fillGesetzObj: ", ex)
        End Try
    End Sub

    Private Function gesetzesDateiInsArchivKopieren(aktgesetz As clsgesetzesManagerDok, datei As String) As Boolean
        Dim Source, Destination As String
        Try
            Dim dir As String
            Dim fi As IO.FileInfo
            fi = New IO.FileInfo(aktgesetz.FullnameImArchiv)
            dir = aktgesetz.FullnameImArchiv.Replace(fi.Name, "").ToLower
            IO.Directory.CreateDirectory(dir)
            '    fi.CopyTo(aktgesetz.fullnameImArchiv)
            Source = datei
            Destination = aktgesetz.FullnameImArchiv
            System.IO.File.Copy(Source, Destination, True)
            fi = Nothing
            Return True
        Catch ex As Exception
            nachricht("fehler in gesetzesDateiInsArchivKopieren", ex)
            Return False
        End Try
    End Function

    Private Function getDateitypt(fi As FileInfo) As String
        Return fi.Extension
    End Function

    Private Function getDateinameohneext(fi As FileInfo) As String
        Dim temp As String
        temp = LIBgemeinsames.clsString.normalize_Filename(fi.Name.ToLower)
        temp = temp.ToLower.Replace(fi.Extension.ToLower, "")
        Return temp
    End Function

    Private Function getOrdner(sg As String) As String
        Try
            Dim ordner = sg '& "\" & Now.ToString("yyyyMM")
            Return ordner
        Catch ex As Exception
            nachricht("fehler in getOrdner ", ex)
            Return ""
        End Try
    End Function





    Friend Function gesetzesDBspeichern(datei As String,
                                        username As String, gesetz As clsgesetzesManagerDok) As Boolean
        Try
            If gesetz_abspeichern_Neu(gesetz, myGlobalz.sitzung.gesetzesdbREC, "neu") > 0 Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            nachricht("fehler in ", ex)
            Return False
        End Try
    End Function

    Public Function gesetz_abspeichern_Neu(ByVal gesetz As clsgesetzesManagerDok,
                                           ByVal vREC As IDB_grundfunktionen,
                                           ByVal modus As String) As Integer
        Dim stammid As Integer
        't38 as 
        If modus = "neu" Then

            Dim querie As String
            clsSqlparam.paramListe.Clear()
            '   clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "INSERT INTO RECHTSDB_STAMM (DATEINAMEOHNEEXT,DATEITYP,ORDNER,QUELLE,BESCHREIBUNG,QUELLENTYP," &
                                                                  "ART,WANNVEROEFFENTLICHT,URL,SCHLAGWORTE,HERKUNFT,ISTGUELTIG,ORIGINALNAME) " &
                               " VALUES (@DATEINAMEOHNEEXT,@DATEITYP,@ORDNER,@QUELLE,@BESCHREIBUNG,@QUELLENTYP," &
                                        "@ART,@WANNVEROEFFENTLICHT,@URL,@SCHLAGWORTE,@HERKUNFT,@ISTGUELTIG,@ORIGINALNAME)"
            populateStammGesetz(gesetz)
            stammid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "STID")
            clsSqlparam.paramListe.Clear() '----------------------------------------
            querie = "INSERT INTO RECHTSDB_SACHGEBIET (STAMMID,SACHGEBIETNR,SGHEADER ) " &
                                                     " VALUES (@STAMMID,@SACHGEBIETNR,@SGHEADER )"
            populateSGGesetz(gesetz, stammid)
            stammid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "STID")

            Return stammid
        End If
        If modus = "edit" Then
            Dim querie As String
            clsSqlparam.paramListe.Clear()
            '   clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
            querie = "UPDATE  rechtsdb_stamm " & " SET DATEINAMEOHNEEXT=@DATEINAMEOHNEEXT" &
                    ",ART=@ART" &
                    ",DATEITYP=@DATEITYP" &
                    ",ORDNER=@ORDNER" &
                    ",BESCHREIBUNG=@BESCHREIBUNG" &
                    ",QUELLENTYP=@QUELLENTYP" &
                    ",herkunft=@herkunft" &
                    ",WANNVEROEFFENTLICHT=@WANNVEROEFFENTLICHT" &
                    ",SCHLAGWORTE=@SCHLAGWORTE" &
                    ",URL=@URL" &
                    ",QUELLE=@QUELLE" &
                    ",ORIGINALNAME=@ORIGINALNAME" &
                    ",ISTGUELTIG=@ISTGUELTIG" &
                    " WHERE STID=@STID"
            populateStammGesetz(gesetz)
            clsSqlparam.paramListe.Add(New clsSqlparam("STID", gesetz.stammid))

            stammid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "STID")
            clsSqlparam.paramListe.Clear() '----------------------------------------
            querie = "UPDATE RECHTSDB_SACHGEBIET SET SACHGEBIETNR=@SACHGEBIETNR" &
                                            ",SGHEADER=@SGHEADER" &
                                            " WHERE STAMMID=@STAMMID"
            populateSGGesetz(gesetz, stammid)
            clsSqlparam.paramListe.Add(New clsSqlparam("STAMMID", gesetz.stammid))
            stammid = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "STAMMID")



            Return stammid
        End If
        Return 0
    End Function

    Private Sub populateSGGesetz(gesetz As clsgesetzesManagerDok, stammid As Integer)
        clsSqlparam.paramListe.Add(New clsSqlparam("STAMMID", stammid))
        clsSqlparam.paramListe.Add(New clsSqlparam("SACHGEBIETNR", gesetz.sachgebietnr))
        clsSqlparam.paramListe.Add(New clsSqlparam("SGHEADER", gesetz.sachgebietheader))
    End Sub

    Private Sub populateStammGesetz(gesetz As clsgesetzesManagerDok)
        clsSqlparam.paramListe.Add(New clsSqlparam(":BESCHREIBUNG", (gesetz.beschreibung)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":QUELLENTYP", (gesetz.quellentyp)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":DATEINAMEOHNEEXT", (gesetz.dateinameohneext)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":DATEITYP", (gesetz.dateityp)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":ART", (gesetz.artId)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":ORDNER", (gesetz.ordner)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":QUELLE", (gesetz.userInitial)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":HERKUNFT", (gesetz.herkunftId)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":WANNVEROEFFENTLICHT", (gesetz.wannveroeffentlicht)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":SCHLAGWORTE", (gesetz.schlagworte)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":URL", (gesetz.url)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":ISTGUELTIG", (gesetz.istgueltig)))
        clsSqlparam.paramListe.Add(New clsSqlparam(":ORIGINALNAME", (gesetz.originalDateiName)))
    End Sub

    Friend Function gesetzesDBspeichernEdit(username As String, gesetz As clsgesetzesManagerDok) As Boolean
        If gesetz_abspeichern_Neu(gesetz, myGlobalz.sitzung.gesetzesdbREC, "edit") > 0 Then
            Return True
        Else
            Return False
        End If
    End Function


    Public Function gesetz_loeschen_DB(ByVal gesetz As clsgesetzesManagerDok,
                                       ByVal vREC As IDB_grundfunktionen) As Integer
        Dim hinweis As String = ""
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from  t39 as rechtsdb_stamm  where stid=" & gesetz.stammid
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from t38 as rechtsdb_sachgebiet  where stammid=" & gesetz.stammid
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
        Return 1
    End Function
    Friend Function gesetz_loeschen_Datei(_gesetz As clsgesetzesManagerDok) As Boolean
        'archivfullname feststellen 
        Dim datei As String
        datei = _gesetz.getFullnameImArchiv(initP.getValue("Rechtsdb.rootdir"))
        Try
            IO.File.Delete(datei)
            Return True
        Catch ex As Exception
            nachricht("fehler in gesetz_loeschen_DB: ", ex)
            Return False
        End Try

    End Function

    Friend Function GesetzesDateiausChecken(item As clsgesetzesManagerDok) As String
        Try
            Dim quelldatei, zieldatei, checkoutRoot As String
            quelldatei = item.getFullnameImArchiv(initP.getValue("Rechtsdb.rootdir"))
            checkoutRoot = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) & "\paradigma\Gesetze\"
            IO.Directory.CreateDirectory(checkoutRoot)
            zieldatei = checkoutRoot & "\" & item.dateinameohneext.Trim & Now.ToString("_yyMMddhhmmss") & item.dateityp
            IO.File.Copy(quelldatei, zieldatei)
            Return zieldatei
        Catch ex As Exception
            nachricht("fehler in GesetzesDateiOeffnen: ", ex)
            Return ""
        End Try
    End Function
    Friend Sub getrechtsDT(sql As String)
        myGlobalz.sitzung.gesetzesdbREC.mydb.SQL = sql
        Dim hinweis As String = ""
        hinweis = myGlobalz.sitzung.gesetzesdbREC.getDataDT()
    End Sub

    Friend Sub getrechtsCmb(rrechtsdbARTcoll As List(Of ClsSimpleCmb))
        Dim newone As New ClsSimpleCmb
        Try
            For i = 0 To myGlobalz.sitzung.gesetzesdbREC.dt.Rows.Count - 1
                newone = New ClsSimpleCmb
                newone.id = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("id")))
                newone.text = clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("text"))
                newone.reihenfolge = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.gesetzesdbREC.dt.Rows(i).Item("reihenf")))
                rrechtsdbARTcoll.Add(newone)
            Next

        Catch ex As Exception
            nachricht("fehler in getrechtsCmb: ", ex)
        End Try
    End Sub
    Friend Sub initRechteDBControls(rrechtsdbARTcoll As List(Of ClsSimpleCmb), sql As String)
        modrechtsdb.getrechtsDT(sql)
        modrechtsdb.getrechtsCmb(rrechtsdbARTcoll)
    End Sub

    Friend Sub artUndHerkunftWandeln(gesetzobejkte As List(Of clsgesetzesManagerDok),
                                     rrechtsdbARTcoll As List(Of ClsSimpleCmb),
                                     rrechtsdbHerkunftcoll As List(Of ClsSimpleCmb))
        For Each rdok In gesetzobejkte
            For i = 0 To rrechtsdbARTcoll.Count - 1
                If rdok.artId = rrechtsdbARTcoll(i).id Then
                    rdok.art_text = rrechtsdbARTcoll(i).text
                    Continue For
                End If
            Next
        Next
        For Each rdok In gesetzobejkte
            For i = 0 To rrechtsdbHerkunftcoll.Count - 1
                If rdok.herkunftId = rrechtsdbHerkunftcoll(i).id Then
                    rdok.herkunft_text = rrechtsdbHerkunftcoll(i).text
                    Continue For
                End If
            Next
        Next

    End Sub
End Module
