Namespace VerwandteTools

    'Public Class Koppelung_Vorgang_Fremdvorgang
    '    Public Shared Function exe(ByVal VorgangsID%, ByVal fremdVID%, ByVal titel as string) as  Integer 'einzelDokument_holen(myGlobalz.sitzung.aktDokument.DocID.ToString)
    '        Dim erfolg As Integer
    '        If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
    '            Dim zzz As New clsVerwandte_mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
    '            erfolg = zzz.Koppelung_Vorgang_Fremdvorgang(VorgangsID, fremdVID, titel)
    '                zzz.dispose
    '        End If
    '        If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
    '            Dim zzz As New clsVerwandte_oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
    '            erfolg = zzz.Koppelung_Vorgang_Fremdvorgang(VorgangsID, fremdVID, titel)
    '                zzz.dispose
    '        End If
    '        Return erfolg
    '    End Function
    'End Class
    Public Class erzeugeVerwandtenlistezuVorgang
        Public Shared Function exe(ByVal sql as string) as  Boolean 'einzelDokument_holen(myGlobalz.sitzung.aktDokument.DocID.ToString)
            'Dim erfolg As Boolean
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New clsVerwandte_mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.erzeugeVerwandtenlistezuVorgang(sql)
            '        zzz.dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New clsVerwandte_oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.erzeugeVerwandtenlistezuVorgang(sql)
            '        zzz.dispose
            'End If
            'Return erfolg
        End Function
    End Class

    Public Class Verwandten_loeschen
        Public Shared Function exe(ByVal kopplungsid as integer) as  Integer
            'Dim erfolg As Integer
            'If myGlobalz.raumbezug_MYDB.dbtyp = "mysql" Then
            '    Dim zzz As New clsVerwandte_mysql(clsDBspecMYSQL.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.Verwandten_loeschen(kopplungsid)
            '        zzz.dispose
            'End If
            'If myGlobalz.raumbezug_MYDB.dbtyp = "oracle" Then
            '    Dim zzz As New clsVerwandte_oracle(clsDBspecOracle.getConnection(myGlobalz.raumbezug_MYDB))
            '    erfolg = zzz.Verwandten_loeschen(kopplungsid)
            '        zzz.dispose
            'End If
            'Return erfolg
        End Function
    End Class
    'clsVerwandte_mysql.Verwandten_loeschen(kopplungsid%)
End Namespace

