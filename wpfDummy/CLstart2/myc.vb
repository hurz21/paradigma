Namespace CLstart
    Public Class myViewsNTabs
        'die ersten drei bleiben erstmal beim alten namen
        'Public Shared ReadOnly tabALKIS_FS2EIGENTUEMER As String = "t01"
        'Public Shared ReadOnly tabALKIS_FS2EIGENTUEMER2201808 As String = "t02"
        'Public Shared ReadOnly tabALKIS_FS2EIGENTUEMERalt As String = "t03"
        Public Shared ReadOnly tabbankverbindung As String = "T04" 'DB "bankverbindung" ' "t04
        Public Shared ReadOnly tabBearbeiter As String = "T05" 'DB "t05" 
        Public Shared ReadOnly tabBeteiligte As String = "T06" 'DB   "beteiligte" ' "t06" ' 
        Public Shared ReadOnly tabKoppBeteiligteVorgang As String = "T07" 'DB "BETEILIGTE2VORGANG" ' "t07"
        '                                                            t08 war dokblob experimental
        'Public Shared ReadOnly tabKoppDokuEreignis As String = "t09" 'DOKUMENT2EREIGNIS #ist obsolet, wird über die docid  in dokumente abgebildet, tabelle wurde deaktiviert
        'Public Shared ReadOnly tabKoppDokuVorgang As String = "t10"  'DOKUMENT2VORGANG   #ist obsolet, wird über die vid in dokumente abgebildet , tabelle wurde deaktiviert
        Public Shared ReadOnly tabDokumente As String = "DOKUMENTE" '"t11"
        Public Shared ReadOnly tabDS_Gruppen As String = "DS_GRUPPEN" ' "t12"
        Public Shared ReadOnly tabDS_Standards As String = "DS_STANDARDS" '"t13"
        Public Shared ReadOnly tabDS_USER2GRUPPE As String = "DS_USER2GRUPPE" ' "t14"
        Public Shared ReadOnly tabDS_Vid2Gruppentext As String = "DS_VID2GRUPPENTEXT" '"t15"
        Public Shared ReadOnly tabEreignis As String = "T16" 'DB "ereignis" '"t16" 'tab16
        Public Shared ReadOnly tabIllegbau As String = "ILLEGBAU" '"t17"
        Public Shared ReadOnly tabKosten As String = "KOSTEN" ' "t18"
        'Public Shared ReadOnly tabMapcookie As String = "t19"
        Public Shared ReadOnly tabNatureg As String = "NATUREG" '"t20"
        '                               ONLINEVERFAHREN t21
        'Public Shared ReadOnly tabORGANISATION As String = "t22" '???? gibts nicht, vermutlich nicht
        Public Shared ReadOnly tabPARAADRESSE As String = "PARAADRESSE" '"T23"
        Public Shared ReadOnly TABPARAFLURSTUECK As String = "PARAFLURSTUECK" ' "T24"
        Public Shared ReadOnly TABPARAFOTO As String = "PARAFOTO" ' "T25"
        Public Shared ReadOnly TABPARAUMKREIS As String = "PARAUMKREIS" ' "T26"
        'PUBLIC SHARED READONLY _TABPL_BEARBEITER AS STRING = "T27_OBS"
        'PUBLIC SHARED READONLY _TABPL_DOKBLOB AS STRING = "T28_OBS"
        'PUBLIC SHARED READONLY _TABPL_FD AS STRING = "T29_OBS"
        'PUBLIC SHARED READONLY _TABPL_VORGANG AS STRING = "T30_OBS"
        Public Shared ReadOnly TABPROJEKT As String = "PROJEKT" ' "T31"
        Public Shared ReadOnly TABPROJEKT2VORGANG As String = "PROJEKT2VORGANG " '"T32"
        Public Shared ReadOnly TABRAUMBEZUG As String = "RAUMBEZUG" ' "T33"
        Public Shared ReadOnly TABRAUMBEZUG2GEOPOLYGON As String = "RAUMBEZUG2GEOPOLYGON" ' "T34"
        Public Shared ReadOnly TABRAUMBEZUG2VORGANG As String = "RAUMBEZUG2VORGANG" '"T35"
        'PUBLIC SHARED READONLY _TABRECHTSDB_ART AS STRING = "T36" DB
        'PUBLIC SHARED READONLY _TABRECHTSDB_HERKUNFT AS STRING = "T37" DB
        'PUBLIC SHARED READONLY _TABRECHTSDB_SACHGEBIET AS STRING = "T38" DB
        'PUBLIC SHARED READONLY _TABRECHTSDB_STAMM AS STRING = "T39" DB
        Public Shared ReadOnly TABSTAKEHOLDER As String = "T40" '
        Public Shared ReadOnly TABSTAMMDATEN As String = "T41" 'DB "STAMMDATEN" 'TAB41
        Public Shared ReadOnly TABCONJECTBASIERT As String = "T42" 'DB  
        Public Shared ReadOnly TABVORGANG As String = "T43" 'DB"VORGANG" 'TAB43
        Public Shared ReadOnly TABKOPPVORGANGFREMDVORGANG As String = "T44" 'DB  "WV" 'T45
        Public Shared ReadOnly TABWV As String = "T45" 'DB "WV" 'T45
        Public Shared ReadOnly TABZAHLUNGEN As String = "T46" 'DB "ZAHLUNGEN" 'T46 
        '  PUBLIC SHARED VIEW_VS_D AS STRING = "" 'WIRD NICHT BENÖTIGT  
        'Private Shared ReadOnly viewEreignis As String = "t16" 'tab16

        Public Shared view_vsk_d As String =
"SELECT distinct TOP (100) PERCENT s.ID, s.VORGANGSID, s.BEARBEITER, s.EINGANG, s.BESCHREIBUNG, s.BEMERKUNG, s.ERLEDIGT, s.LETZTEBEARBEITUNG, s.ORTSTERMIN, s.STELLUNGNAHME, " &
"       s.LASTACTIONHEROE, s.ISTINVALID, s.PROBAUGAZ, s.GEMKRZ, s.AUFNAHME, s.ALTAZ, s.AZ2, s.WEITEREBEARB, s.ARCDIR, s.DARFNICHTVERNICHTETWERDEN, s.STORAUMNR, s.STOTITEL, " &
"       s.GUTACHTENMIT, s.GUTACHTENDRIN, s.ABGABEBA, s.PARAGRAF, s.HATRAUMBEZUG, s.INTERNENR, v.VORGANGSID AS mid, v.SACHGEBIETNR, v.VORGANGSNR, v.VORGANGSGEGENSTAND, " &
"       v.SACHGEBIETSTEXT, v.ISTUNB, v.AZ, v.TS, k.VERWALTUNGSGEBUEHR, k.VERWALTUNGSGEBUEHR_BEZAHLT, k.ERSATZGELD, k.ERSATZGELD_BEZAHLT, k.SICHERHEIT, k.SICHERHEIT_BEZAHLT, " &
"       k.INTERNEZAHLUNG, k.QUELLE AS kquelle, k.TS AS kts, k.VERWARNUNGSGELD, k.VERWARNUNGSGELD_BEZAHLT, k.BUSSGELD, k.BUSSGELD_BEZAHLT, k.ERSATZGELD_AUSGEZAHLT, " &
"       k.ZWANGSGELD, k.ZWANGSGELD_BEZAHLT, k.BEIHILFE, k.BEIHILFE_BEZAHLT, t.T43 " & TABVORGANG & ",BEARBEITERID " &
" FROM  " & TABSTAMMDATEN & " AS s LEFT OUTER JOIN" &
"       " & TABVORGANG & " AS v ON s.VORGANGSID = v.VORGANGSID LEFT OUTER JOIN" &
"       " & tabKosten & " AS k ON s.VORGANGSID = k.VORGANGSID LEFT OUTER JOIN" &
"       " & TABCONJECTBASIERT & " AS t ON s.VORGANGSID = t.S12" &
" ORDER BY s.LETZTEBEARBEITUNG DESC"


        Public Shared view_raumbezugundvorg As String =
            " (SELECT r.RAUMBEZUGSID, r.TYP, r.TITEL, r.SEKID, r.ABSTRACT, r.RECHTS, r.HOCH, r.XMIN, r.XMAX, r.YMIN, r.YMAX, r.FREITEXT, r.ISMAPENABLED, rv.RAUMBEZUGSID AS ridRV, rv.VORGANGSID, rv.STATUS, " &
"             r.FLAECHEQM, r.LAENGEM, r.MITETIKETT, v.SACHGEBIETNR, v.SACHGEBIETSTEXT" &
"      FROM    RAUMBEZUG2VORGANG AS rv INNER JOIN" &
"              " & TABRAUMBEZUG & " AS r ON rv.RAUMBEZUGSID = r.RAUMBEZUGSID INNER JOIN" &
"              " & TABVORGANG & " AS v ON rv.VORGANGSID = v.VORGANGSID) "

        Public Shared view_illegale2 As String =
" (SELECT TOP (100) PERCENT sd.BESCHREIBUNG, sd.AZ2, pf.GEMCODE, sd.GEMKRZ, pf.GEMARKUNGSTEXT, pf.FLUR, pf.ZAEHLER, pf.NENNER, pf.ZNKOMBI, rbv.TITEL, i.ILLEGID, i.VORGANGSID, i.STATUS, " &
"             i.GEBIET, i.RAEUMUNGSTYP, i.ANHOERUNG, i.RAEUMUNGBISDATUM, i.RAEUMUNG, i.VERFUEGUNG, i.FALLERLEDIGT, i.VERMERK, i.EID_ANHOERUNG, i.EID_RAEUMUNG, i.EID_VERFUEGUNG, i.QUELLE, " &
"             i.TS, sd.LETZTEBEARBEITUNG, sd.LASTACTIONHEROE, sd.BEARBEITER" &
" FROM        " & view_raumbezugundvorg & " AS rbv INNER JOIN" &
"             " & tabIllegbau & " AS i ON rbv.VORGANGSID = i.VORGANGSID INNER JOIN" &
"             " & TABPARAFLURSTUECK & " AS pf ON rbv.SEKID = pf.ID INNER JOIN" &
"             " & TABSTAMMDATEN & " AS sd ON rbv.VORGANGSID = sd.VORGANGSID" &
" ORDER BY i.STATUS, i.VORGANGSID DESC) "

        Public Shared view_illegalohnerb3 As String =
" (SELECT TOP (100) PERCENT sd.BESCHREIBUNG, sd.AZ2, sd.GEMKRZ, i.ILLEGID, i.VORGANGSID, i.STATUS, i.GEBIET, i.RAEUMUNGSTYP, i.ANHOERUNG, i.RAEUMUNGBISDATUM, i.RAEUMUNG, i.VERFUEGUNG, " &
"             i.FALLERLEDIGT, i.VERMERK, i.EID_ANHOERUNG, i.EID_RAEUMUNG, i.EID_VERFUEGUNG, i.QUELLE, i.TS, sd.EINGANG, sd.LETZTEBEARBEITUNG, sd.LASTACTIONHEROE, sd.BEARBEITER" &
" FROM        " & tabIllegbau & " AS i INNER JOIN" &
"             " & TABSTAMMDATEN & " AS sd ON i.VORGANGSID = sd.VORGANGSID" &
" ORDER BY i.STATUS, i.VORGANGSID DESC) "



        Public Shared view_ereignis2dok2 As String =
                " (SELECT e.ID, e.VORGANGSID, e.BESCHREIBUNG, e.DATUM, e.ART, e.RICHTUNG, e.NOTIZ, d.DOKUMENTID, e.WVFAELLIG, e.QUELLE, e.PERSONENID, e.ISTRTF, d.DATEINAMEEXT, " &
                " d.BESCHREIBUNG AS D_BESCHREIBUNG, d.RELATIVPFAD, d.TYP, d.FILEDATUM, d.CHECKINDATUM, d.VERALTET, d.EXIFDATUM, d.EXIFLONG, d.EXIFLAT, d.EXIFDIR, " &
                "    d.EXIFHERSTELLER, d.REVISIONSSICHER, d.INITIAL_, d.EID, d.VID, d.NEWSAVEMODE,d.kompress,d.MB " &
                " FROM   " & tabEreignis & " AS e INNER JOIN" &
                " " & tabDokumente & " AS d ON e.ID = d.EID) "

        Public Shared view_anzahldoksproereignis2 As String =
                " (SELECT COUNT(EID) AS anzahl, EID As id, VID As vorgangsid " &
                " FROM  " & tabDokumente & " " &
                " WHERE (EID > 0) " &
                " GROUP BY EID, VID ) "

        Public Shared view_VORG2DOKEREIGNIS2 As String =
            "(SELECT TOP (100) PERCENT e.ID, e.VORGANGSID," &
                  " e.BESCHREIBUNG, e.DATUM, e.ART, e.RICHTUNG, " &
                " e.NOTIZ, e.WVFAELLIG, e.QUELLE, e.PERSONENID, e.ISTRTF,  " & tabDokumente & ".DOKUMENTID, " &
                " " & tabDokumente & ".RELATIVPFAD,  " & tabDokumente & ".DATEINAMEEXT,  " & tabDokumente & ".TYP,  " & tabDokumente & ".BESCHREIBUNG AS d_beschreibung, " &
                " " & tabDokumente & ".FILEDATUM,  " & tabDokumente & ".CHECKINDATUM,  " & tabDokumente & ".VERALTET,  " & tabDokumente & ".EXIFDATUM,  " & tabDokumente & ".EXIFLONG, " &
                " " & tabDokumente & ".EXIFLAT,  " & tabDokumente & ".EXIFDIR,  " & tabDokumente & ".EXIFHERSTELLER,  " & tabDokumente & ".REVISIONSSICHER,  " & tabDokumente & ".INITIAL_, " &
                " " & tabDokumente & ".VID,  " & tabDokumente & ".EID,  " & tabDokumente & ".NEWSAVEMODE,  " & tabDokumente & ".MB,  " &
          tabDokumente & ".kompress, e.typnr " &
                " FROM " & tabEreignis & " as e LEFT OUTER JOIN" &
                " " & tabDokumente & " ON " & tabDokumente & ".EID = e.ID" &
                " ORDER BY e.DATUM DESC) "
        Public Shared view_raumbezugPolygone As String =
            "SELECT TOP (1000) p.[ID]" &
                "      ,p.[RAUMBEZUGSID]" &
                "      ,p.[VORGANGSID]" &
                "	   ,r.FREITEXT" &
                "	   ,r.ABSTRACT" &
                "	   ,r.TITEL" &
                "	   ,stamm.AZ2" &
                "      ,[SERIALSHAPE]" &
                "      ,p.[TYP] as TYP" &
                "	   ,r.[TYP] as RTYP" &
                "      ,[AREAQM]" &
                "      ,[SERIALUTM]" &
                "      ,stamm.[GEMKRZ]" &
                "      ,stamm.[BEARBEITER]" &
                "  FROM [Paradigma].[dbo].[" & TABRAUMBEZUG2GEOPOLYGON & "] p, " & TABSTAMMDATEN & " stamm, " & TABVORGANG & "  vorgang," & TABRAUMBEZUG & " r" &
                "  where p.VORGANGSID=stamm.VORGANGSID and" &
                "  stamm.VORGANGSID=vorgang.vorgangsid and " &
                "  r.RAUMBEZUGSID=p.RAUMBEZUGSID and " &
                "  (r.TYP=3 or r.typ=2)" &
                "  order by p.VORGANGSID desc, r.ABSTRACT"

        Public Shared view_dokumentplussg As String =
             "Select  d.*,t.VORGANGSID " &
               ",SACHGEBIETNR   " &
               " FROM [Paradigma].[dbo].[t43] t,dokumente d  " &
               " where d.vid=t.vorgangsid"


    End Class

    Public Class myc
        Public Shared aktprojekt As clsProjektAllgemein
        Public Shared collFotos As New List(Of clsFotoDokument)
        Public Shared globalrange As New clsRange
        Public Shared raumberange As New clsRange
        Public Shared kartengen As clsAufrufgenerator
        Public Shared punktarrayInM() As myPoint
        Public Shared AZauswahl As New Stamm(mycSimple.MeinNULLDatumAlsDate)
        Public Shared userIniProfile As clsINIDatei
        'Public Shared MAINiniFile As clsINIDatei
        'hurz

        Public Shared aLog As ActionLog
    End Class
    Public Class formposition
        Shared Function getPosition(kategorie As String, eintrag As String, aktval As Double) As Double
            'Me.Top = clsToolsAllg.setPosition("diverse", "dbabfrageformpositiontop", Me.Top)
            'Me.Left = clsToolsAllg.setPosition("diverse", "dbabfrageformpositionleft", Me.Left)
            Dim retval As Double
            Try
                l(" setPosition ---------------------- anfang")
                Dim topf As String = CLstart.myc.userIniProfile.WertLesen(kategorie, eintrag)
                If String.IsNullOrEmpty(topf) Then
                    CLstart.myc.userIniProfile.WertSchreiben(kategorie, eintrag, CType(aktval, String))

                    retval = aktval
                Else
                    retval = CDbl(topf)
                End If
                l(" getIniDossier ---------------------- ende")
                Return retval
            Catch ex As Exception
                l("Fehler in setPosition: " & kategorie & " " & eintrag & " " & aktval & " ", ex)
                Return aktval
            End Try
        End Function
    End Class
End Namespace