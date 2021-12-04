Public Class clsKosten
    Property id As Integer = 0
    Property vorgangsid As Integer = 0
    Property InterneZahlung As Boolean = False
    Property verwaltungsgebuehr As Boolean = False
    Property verwaltungsgebuehrBezahlt As Boolean = False

    Property ersatzgeld As Boolean = False
    Property ersatzgeldBezahlt As Boolean = False
    Property ersatzgeldAUSGEzahlt As Boolean = False

    Property sicherheit As Boolean = False
    Property sicherheitBezahlt As Boolean = False
    Property VerwarnungsgeldBezahlt As Boolean = False
    Property VERWARNUNGSGELD As Boolean = False
    Property BUSSGELD As Boolean = False
    Property BUSSGELDBezahlt As Boolean = False

    Property BEIHILFE As Boolean = False
    Property BEIHILFEBezahlt As Boolean = False

    Property ZWANGSGELD As Boolean = False
    Property ZWANGSGELDBezahlt As Boolean = False

    Property QUELLE As String = ""
    Property timestamp As Date
    Sub New()
        clear()
    End Sub



    Sub clear()
        id = 0
        vorgangsid = 0
        InterneZahlung = False
        verwaltungsgebuehr = False
        verwaltungsgebuehrBezahlt = False
        ersatzgeld = False
        ersatzgeldBezahlt = False
        sicherheit = False
        sicherheitBezahlt = False
        ersatzgeldAUSGEzahlt = False
        BEIHILFE = False
        BEIHILFEBezahlt = False
        ZWANGSGELD = False
        ZWANGSGELDBezahlt = False


        QUELLE = ""
        timestamp = CLstart.mycsimple.MeinNULLDatumAlsDate
    End Sub

End Class
