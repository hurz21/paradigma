Imports System.Data
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Module bvTools

    Function eingabeistok(clsBankverbindungSEPA As clsBankverbindungSEPA) As Boolean
        If Not clsBankverbindungSEPA.IBAN.Length = 22 Then
            MsgBox("Die IBAN muss 22 Stellen haben: " & clsBankverbindungSEPA.IBAN.Length)
            Return False
        End If
        'If Not clsBankverbindungSEPA.BIC.Length = 11 Then
        '    MsgBox("Die BIC muss 11 Stellen haben: " & clsBankverbindungSEPA.BIC.Length)
        '    Return False
        'End If
        If clsBankverbindungSEPA.BankName.IsNothingOrEmpty Then
            MsgBox("Bitte einen Namen für die Bank angeben! ")
            Return False
        End If


        Return True
    End Function


    Function bvNormalspeichernNEU(bv As clsBankverbindungSEPA) As Boolean
        Dim querie As String
        'werteDBsicherMachenEreignis(ereignis)
        bv.istVORLAGE1 = False
        clsSqlparam.paramListe.Clear()
        'populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        With bv
            clsSqlparam.paramListe.Add(New clsSqlparam("IBAN", .IBAN))
            clsSqlparam.paramListe.Add(New clsSqlparam("BIC", (.BIC)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BANKNAME", (.BankName)))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", (.Quelle)))
            clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENID", (.personenID)))
            clsSqlparam.paramListe.Add(New clsSqlparam("TITEL", (.Titel)))
            'com.Parameters.AddWithValue(:TS", (.tss))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORLAGE1", (.istVORLAGE1)))
        End With
        querie = "INSERT INTO " & CLstart.myViewsNTabs.tabbankverbindung & "  (IBAN,BIC,BANKNAME,QUELLE,PERSONENID,TITEL,VORLAGE1) " &
                               " VALUES (@IBAN,@BIC,@BANKNAME,@QUELLE,@PERSONENID,@TITEL,@VORLAGE1)"
        Dim bvid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "BANKKONTOID")
        If bvid < 1 Then
            Return False
        End If
        Return True

    End Function
    Function bvVorlagespeichernNEU(bv As clsBankverbindungSEPA) As Boolean
        Dim querie As String
        'werteDBsicherMachenEreignis(ereignis)
        clsSqlparam.paramListe.Clear()
        'populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
#Disable Warning BC42024 ' Unused local variable: 'natid'.
        Dim natid, pid As Integer
#Enable Warning BC42024 ' Unused local variable: 'natid'.
        pid = bv.personenID
        bv.istVORLAGE1 = True
        bv.personenID = 0
        With bv
            clsSqlparam.paramListe.Add(New clsSqlparam("IBAN", .IBAN))
            clsSqlparam.paramListe.Add(New clsSqlparam("BIC", (.BIC)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BANKNAME", (.BankName)))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", (.Quelle)))
            clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENID", (.personenID)))
            clsSqlparam.paramListe.Add(New clsSqlparam("TITEL", (.Titel)))
            'com.Parameters.AddWithValue(:TS", (.tss))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORLAGE1", (.istVORLAGE1)))
        End With

        querie = "INSERT INTO " & CLstart.myViewsNTabs.tabbankverbindung & "  (IBAN,BIC,BANKNAME,QUELLE,PERSONENID,TITEL,VORLAGE1) " &
                               " VALUES (@IBAN,@BIC,@BANKNAME,@QUELLE,@PERSONENID,@TITEL,@VORLAGE1)"
        Dim bvid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "BANKKONTOID")
        bv.personenID = pid
        If bvid < 1 Then
            Return False
        End If
        Return True
    End Function
    Function bvVORLAGEspeichernEdit(bv As clsBankverbindungSEPA) As Boolean
        Dim querie As String
        'werteDBsicherMachenEreignis(ereignis)
        clsSqlparam.paramListe.Clear()
        'populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
#Disable Warning BC42024 ' Unused local variable: 'natid'.
        Dim natid, pid As Integer
#Enable Warning BC42024 ' Unused local variable: 'natid'.
        pid = bv.personenID
        bv.istVORLAGE1 = True
        bv.personenID = 0
        pid = bv.personenID
        bv.istVORLAGE1 = True
        bv.personenID = 0
        With bv
            clsSqlparam.paramListe.Add(New clsSqlparam("IBAN", .IBAN))
            clsSqlparam.paramListe.Add(New clsSqlparam("BIC", (.BIC)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BANKNAME", (.BankName)))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", (.Quelle)))
            clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENID", (.personenID)))
            clsSqlparam.paramListe.Add(New clsSqlparam("TITEL", (.Titel)))
            'com.Parameters.AddWithValue(:TS", (.tss))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORLAGE1", (.istVORLAGE1)))
        End With

        querie = "UPDATE  " & CLstart.myViewsNTabs.tabbankverbindung & "  " & " set IBAN=@IBAN" &
                    ",BIC=@BIC" &
                    ",BANKNAME=@BANKNAME " &
                    ",QUELLE=@QUELLE " &
                    ",PERSONENID=@PERSONENID " &
                    ",VORLAGE1=@VORLAGE1 " &
                    ",TITEL=@TITEL " &
                    " WHERE BANKKONTOID=@BANKKONTOID"
        Dim bvid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "BANKKONTOID")
        bv.personenID = pid
        If bvid < 1 Then
            Return False
        End If
        Return True
    End Function

    Function bvNORMALspeichernEdit(bv As clsBankverbindungSEPA) As Boolean
        Dim querie As String
        'werteDBsicherMachenEreignis(ereignis)
        bv.istVORLAGE1 = False
        clsSqlparam.paramListe.Clear()
        'populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        With bv
            clsSqlparam.paramListe.Add(New clsSqlparam("IBAN", .IBAN))
            clsSqlparam.paramListe.Add(New clsSqlparam("BIC", (.BIC)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BANKNAME", (.BankName)))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", (.Quelle)))
            clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENID", (.personenID)))
            clsSqlparam.paramListe.Add(New clsSqlparam("TITEL", (.Titel)))
            'com.Parameters.AddWithValue(:TS", (.tss))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORLAGE1", (.istVORLAGE1)))
        End With
        querie = "UPDATE  " & CLstart.myViewsNTabs.tabbankverbindung & "  " & " set IBAN=@IBAN" &
                ",BIC=@BIC" &
                ",BANKNAME=@BANKNAME " &
                ",QUELLE=@QUELLE " &
                ",PERSONENID=@PERSONENID " &
                ",VORLAGE1=@VORLAGE1 " &
                ",TITEL=@TITEL " &
                    " WHERE BANKKONTOID=@BANKKONTOID"
        clsSqlparam.paramListe.Add(New clsSqlparam("BANKKONTOID", bv.BANKKONTOID))
        Dim bvid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "BANKKONTOID")
        If bvid < 1 Then
            Return False
        End If
        Return True
    End Function

    Function getBankname(bname As String) As String
        If Not bname.IsNothingOrEmpty Then
            Return LIBgemeinsames.clsString.kuerzeTextauf(bname, 144)
        End If
        Return ""
    End Function

    Function getTitel(titel As String) As String
        If Not titel.IsNothingOrEmpty Then
            Return LIBgemeinsames.clsString.kuerzeTextauf(titel, 244)
        End If
        Return ""
    End Function

    Function BVsladen(personenID As Integer) As System.Data.DataTable
        Return BvORACLE.getbvDatatable(personenID)
    End Function

    Function BVvorlagenLaden() As System.Data.DataTable
        Return BvORACLE.getbvVorlagenDatatable()
    End Function
    Public Sub bvitem2Obj(item As DataRowView, aktbv As clsBankverbindungSEPA)
        Try
            With aktbv
                .IBAN = CStr(clsDBtools.fieldvalue(item("IBAN")))
                .BANKKONTOID = CInt(clsDBtools.fieldvalue(item("BANKKONTOID")))
                .BankName = CStr(clsDBtools.fieldvalue(item("BankName")))
                .BIC = CStr(clsDBtools.fieldvalue(item("BIC")))
                .personenID = CInt(clsDBtools.fieldvalue(item("personenID")))
                .Quelle = CStr(clsDBtools.fieldvalue(item("Quelle")))

                .istVORLAGE1 = CBool(clsDBtools.toBool(item("VORLAGE1")))
                .Titel = (clsDBtools.fieldvalue(item("TITEL")))
                '  .tss = CDate(clsDBtools.fieldvalue(item("TS")))
            End With
        Catch ex As Exception
            nachricht_und_Mbox("1 fehler in bvitem2Obj: " ,ex)
        End Try
    End Sub

    Function bvLoeschen(bv As clsBankverbindungSEPA) As Short
        Dim hinweis As String = ""
        myGlobalz.sitzung.VorgangREC.mydb.SQL = " delete from " & CLstart.myViewsNTabs.tabbankverbindung & "  where BANKKONTOID=" & bv.BANKKONTOID
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)


        Return 1
    End Function

    Function bv3PersonenID(persoId As Integer) As String
        Dim query, hinweis As String
        Dim iban As String
        query = "select iban + ' ' + bankname from " & CLstart.myViewsNTabs.tabbankverbindung & "  " &
                " where personenid=" & persoId
#Disable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
        myGlobalz.sitzung.tempREC.dt = getDT4Query(query, myGlobalz.sitzung.tempREC, hinweis)
#Enable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
        If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
            iban = ""
        Else
            iban = CStr(myGlobalz.sitzung.tempREC.dt.Rows(0).Item(0))
        End If
        Return CStr(iban)
    End Function

End Module
