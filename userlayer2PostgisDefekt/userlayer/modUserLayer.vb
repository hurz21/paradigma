Module modUserLayer
    Friend Function getUserebeneAid(username As String, ByRef useridINtern As Integer) As Integer

        Return modPG.getUserebeneAid(username, useridINtern)
    End Function

    Friend Function userLayerErzeugen(ByRef tablename As String, vid As String, _modus As String) As Integer
        l("userLayerErzeugen")
        Dim aid = modPG.userLayerInStammErzeugenAid(tablename)
        l("aid wurde erzeugt: " & aid)
        getTablename(_modus, aid) : l("tablename: " & tablename)
        modPG.userLayerAttribErzeugenAid(tablename, aid)
        modPG.userLayerActiveDirErzeugen(tablename, aid)
        Return aid
    End Function

    Friend Function InsertInNutzertab(username As String, userEbeneAid As Integer) As Integer
        Return modPG.InsertInNutzertabAid(username, userEbeneAid)
    End Function

    Friend Function updateNutzerTab(useridINtern As Integer, userEbeneAid As Integer) As Boolean
        Return modPG.UpdateNutzertabAid(useridINtern, userEbeneAid)
    End Function
End Module
