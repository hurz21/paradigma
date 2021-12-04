'Imports System.Data
'Imports LibDB.LIBDB
'Module bvTools

'    'Function eingabeistok(clsBankverbindungSEPA As clsBankverbindungSEPA) As Boolean
'    '    If Not clsBankverbindungSEPA.IBAN.Length = 22 Then
'    '        MsgBox("Die IBAN muss 22 Stellen haben: " & clsBankverbindungSEPA.IBAN.Length)
'    '        Return False
'    '    End If
'    '    'If Not clsBankverbindungSEPA.BIC.Length = 11 Then
'    '    '    MsgBox("Die BIC muss 11 Stellen haben: " & clsBankverbindungSEPA.BIC.Length)
'    '    '    Return False
'    '    'End If
'    '    If clsBankverbindungSEPA.BankName.IsNothingOrEmpty Then
'    '        MsgBox("Bitte einen Namen für die Bank angeben! ")
'    '        Return False
'    '    End If


'    '    Return True
'    'End Function


'    'Function bvNormalspeichernNEU(bv As clsBankverbindungSEPA) As Boolean
'    '    If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
'    '        Dim zzz As New BvORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
'    '        Dim natid As Integer
'    '        bv.istVORLAGE1 = False
'    '        natid = zzz.bv_abspeichern_Neu(bv)
'    '        zzz.Dispose()
'    '        If natid < 1 Then
'    '            Return False
'    '        End If
'    '        Return True
'    '    End If
'    'End Function
'    'Function bvVorlagespeichernNEU(bv As clsBankverbindungSEPA) As Boolean
'    '    If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
'    '        Dim zzz As New BvORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
'    '        Dim natid, pid As Integer
'    '        pid = bv.personenID
'    '        bv.istVORLAGE1 = True
'    '        bv.personenID = 0
'    '        natid = zzz.bv_abspeichern_Neu(bv)
'    '        zzz.Dispose()
'    '        bv.personenID = pid
'    '        If natid < 1 Then
'    '            Return False
'    '        End If
'    '        bv.istVORLAGE1 = False
'    '        Return True
'    '    End If
'    'End Function



'    'Function bvVORLAGEspeichernEdit(bv As clsBankverbindungSEPA) As Boolean
'    '    If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
'    '        Dim zzz As New BvORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
'    '        Dim natid, pid As Integer
'    '        pid = bv.personenID
'    '        bv.istVORLAGE1 = True
'    '        bv.personenID = 0
'    '        natid = zzz.bv_abspeichern_Edit(bv)
'    '        zzz.Dispose()
'    '        bv.personenID = pid
'    '        If natid < 1 Then
'    '            Return False
'    '        End If
'    '        bv.istVORLAGE1 = False
'    '        Return True
'    '    End If
'    'End Function

'    'Function bvNORMALspeichernEdit(bv As clsBankverbindungSEPA) As Boolean
'    '    If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
'    '        Dim zzz As New BvORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
'    '        Dim natid As Integer

'    '        bv.istVORLAGE1 = False
'    '        natid = zzz.bv_abspeichern_Edit(bv)
'    '        zzz.Dispose()

'    '        If natid < 1 Then
'    '            Return False
'    '        End If
'    '    End If
'    '    Return True
'    'End Function

'    'Function getBankname(bname As String) As String
'    '    If Not bname.IsNothingOrEmpty Then
'    '        Return LIBgemeinsames.clsString.kuerzeTextauf(bname, 144)
'    '    End If
'    '    Return ""
'    'End Function

'    'Function getTitel(titel As String) As String
'    '    If Not titel.IsNothingOrEmpty Then
'    '        Return LIBgemeinsames.clsString.kuerzeTextauf(titel, 244)
'    '    End If
'    '    Return ""
'    'End Function

'    'Function BVsladen(personenID As Integer) As System.Data.DataTable
'    '    Return BvORACLE.getbvDatatable(personenID)
'    'End Function

'    'Function BVvorlagenLaden() As System.Data.DataTable
'    '    'Return BvORACLE.getbvVorlagenDatatable()
'    'End Function
'    'Public Sub bvitem2Obj(item As DataRowView, aktbv As clsBankverbindungSEPA)
'    '    Try
'    '        With aktbv
'    '            .IBAN = CStr(clsDBtools.fieldvalue(item("IBAN")))
'    '            .BANKKONTOID = CInt(clsDBtools.fieldvalue(item("BANKKONTOID")))
'    '            .BankName = CStr(clsDBtools.fieldvalue(item("BankName")))
'    '            .BIC = CStr(clsDBtools.fieldvalue(item("BIC")))
'    '            .personenID = CInt(clsDBtools.fieldvalue(item("personenID")))
'    '            .Quelle = CStr(clsDBtools.fieldvalue(item("Quelle")))
'    '            .tss = CDate(clsDBtools.fieldvalue(item("TS")))
'    '            .istVORLAGE1 = CBool(clsDBtools.toBool(item("VORLAGE1")))
'    '            .Titel = (clsDBtools.fieldvalue(item("TITEL")))
'    '        End With
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("1 fehler in bvitem2Obj: " & ex.ToString)
'    '    End Try
'    'End Sub

'    'Public Sub bvitem2obj2(ByVal i As Integer,byref aktbv As clsBankverbindungSEPA)
'    '    Try
'    '        With myGlobalz.sitzung.tempREC2.dt
'    '            aktbv.BANKKONTOID = CInt(clsDBtools.fieldvalue(.Rows(i).Item("BANKKONTOID")))
'    '            aktbv.BankName = (clsDBtools.fieldvalue(.Rows(i).Item("BANKNAME")))
'    '            aktbv.Titel = (clsDBtools.fieldvalue(.Rows(i).Item("TITEL")))
'    '            aktbv.Quelle = (clsDBtools.fieldvalue(.Rows(i).Item("QUELLE")))
'    '            aktbv.IBAN = (clsDBtools.fieldvalue(.Rows(i).Item("IBAN")))
'    '            aktbv.BIC = (clsDBtools.fieldvalue(.Rows(i).Item("BIC")))
'    '            aktbv.tss = CDate(clsDBtools.fieldvalueDate(.Rows(i).Item("TS")))
'    '            aktbv.istVORLAGE1 = CBool(clsDBtools.tobool(.Rows(i).Item("VORLAGE1")))
'    '        End With
'    '    Catch ex As Exception
'    '        nachricht_und_Mbox("1 fehler in bvitem2obj2: " & ex.ToString)
'    '    End Try
'    'End Sub

'    'Function bvLoeschen(bv As clsBankverbindungSEPA) As Short
'    '    Dim zzz As New BvORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
'    '    Dim killed As Integer = zzz.bv_loeschen(bv.BANKKONTOID)
'    '       zzz.dispose
'    '    Return CShort(killed)

'    'End Function

'End Module
