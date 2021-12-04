Public Class clsIllegaleHuette
    Property vid As Integer
    Property illegID As Integer
    Property status As String
    Property gebiet As String
    Property raeumungsTyp As String
    Property anhoerung As Date
    Property raeumungBisDatum As Date
    Property raeumung As Date
    Property verfuegung As Date
    Property fallerledigt As Date
    Property vermerk As String
    Property eid_anhoerung As Integer = 0
    Property eid_raeumung As Integer = 0
    Property eid_verfuegung As Integer = 0
    Property quelle As String = ""

    Property ts As Date

    Sub New()
        status = "1" '0="",1=planmaessig,2=laufend,3=erledigt, 4=recherche
        gebiet = "0" '0="",1="Außenbereich",2= "LSG Kreis Offenbach",3="LSG Hess. Mainauen",4="LSG Zellerbruch",5="NSG"
        vermerk = ""
        quelle = ""
        raeumungsTyp = "0"  '0="",1=freiwillig,2=Abräumvertrag,3=Rechtsstreit
        eid_anhoerung = 0
        eid_raeumung = 0
        eid_verfuegung = 0
    End Sub





End Class
