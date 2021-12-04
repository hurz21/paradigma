Imports System.Data
Imports System.Text

Class MainWindow
    Public logfile As String
    Shared Property perso As New PocoPerson
    Dim vid As String, jahr As String, nummer, bid, bname, initial As String
    Sub New()
        InitializeComponent()
    End Sub
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        Dim kommando As String
        kommando = Environment.CommandLine.ToLower
#If DEBUG Then
        IO.Directory.SetCurrentDirectory("C:\kreisoffenbach\probaugview")
        kommando = "C:\kreisoffenbach\probaugview\probaugview.exe  /vid=9609#/jahr=1994#/nummer=3423/bid=1/bname=feinen_j/initial=feij"
        '63-02764-20-43
        '#/vid=52666 /jahr=21 /nummer=03233 /bid=1/bname=feinen_j
#End If
        logfile = "O:\UMWELT\B\GISDatenEkom\div\logging\probaugview\"
        setLogfile(logfile)
        l("kommando " & kommando)

        Dim tool As New clsTools
        tool.getallParams(kommando, vid, jahr, nummer, bid, bname, initial)
        'db-aufruf vorbereiten
        l("vid " & vid)
        l("jahr " & jahr)
        l("nummer " & nummer)
        l("bid " & bid)
        l("bname " & bname)
        l("initial " & initial)

        l("db-aufruf vorbereiten ")
        Dim obj As New clsMSSQL
        Dim mySQLconn As New SqlClient.SqlConnection
        mySQLconn = obj.getMSSQLCon("msql01", "paradigma", "sgis", "WinterErschranzt.74")
        mySQLconn = obj.getMSSQLCon("msql01", "Probaug", "sgis", "WinterErschranzt.74")
        'mySQLconn = obj.getMSSQLCon("msql01", "prosozbau", "sgis", "WinterErschranzt.74")
        obj = Nothing
        Dim sql As String = ""
        sql = "SELECT * FROM dokumente where  dokumentid<2000000 and dokumentid>0  " &
                  " and (revisionssicher=1) order by dokumentid desc "
        sql = "select * from dbo.GISVIEW1 where feld1 like '%" & jahr & "'  and feld3='" & nummer & "'"
        'sql = "select * from dbo.[GLAAnrede] "
        l("sql " & sql)
        Dim gv1DT As System.Data.DataTable = tool.initGisview1(mySQLconn, sql)
        If gv1DT Is Nothing Then
            MsgBox("gv1DT is nothing " & sql)
            End
        End If
        l("erstelleTextListe vorbereiten ")


        Dim probaugGemarkungsdict As New Dictionary(Of Integer, String)
        Dim katasterGemarkungslist As New List(Of myComboBoxItem)
        Dim gemeindedict As New Dictionary(Of Integer, String)
        tool.machDicts(probaugGemarkungsdict, katasterGemarkungslist, gemeindedict)

        Dim fst As New PocoFlurstueck
        fst.probaugGemcode = CInt(gv1DT.Rows(0).Item(11).ToString.Trim)
        fst.gemeindeNr = CInt(gv1DT.Rows(0).Item(10).ToString.Trim)
        fst.gemeindename = (gv1DT.Rows(0).Item(32).ToString.Trim)
        fst.flur = CInt(gv1DT.Rows(0).Item(12).ToString.Trim)
        fst.fstueckKombi = CStr((gv1DT.Rows(0).Item(13).ToString.Trim))

        fst.gemeindename = fst.gemparms.gemeindenr2gemeindetext(CStr(fst.gemeindeNr))
        fst.splitFstueckkombi()
        fst.gemcode = fst.getPROBAUGGemcode(fst.probaugGemcode.ToString)
        fst.gemarkungstext = fst.gemparms.gemcode2gemarkungstext(fst.gemcode)

        Dim adr As New PocoAdresse
        adr.gemeindeNr = CInt(gv1DT.Rows(0).Item(10).ToString.Trim)
        adr.gemeindeName = gv1DT.Rows(0).Item(32).ToString.Trim
        adr.gemeindeName = fst.gemparms.gemeindenr2gemeindetext(CStr(adr.gemeindeNr))
        adr.strasseName = gv1DT.Rows(0).Item(24).ToString.Trim
        adr.HausKombi = gv1DT.Rows(0).Item(25).ToString.Trim


        perso.Name = gv1DT.Rows(0).Item(19).ToString.Trim
        perso.Vorname = gv1DT.Rows(0).Item(18).ToString.Trim
        perso.akademischerTitel = gv1DT.Rows(0).Item(14).ToString.Trim
        perso.Kontakt.Anschrift.Strasse = gv1DT.Rows(0).Item(20).ToString.Trim
        perso.Kontakt.Anschrift.Hausnr = gv1DT.Rows(0).Item(21).ToString.Trim
        perso.Kontakt.Anschrift.PLZ = gv1DT.Rows(0).Item(22).ToString.Trim
        perso.Kontakt.Anschrift.Gemeindename = gv1DT.Rows(0).Item(23).ToString.Trim




        tbAS.Text = tool.makeASstring(perso)
        tbRB.Text = tool.makeFSTstring(fst) & Environment.NewLine &
                     tool.makeADRESSEstring(adr)
        tbgv1.Text = tool.erstelleTextListe(gv1DT)
        Title = tool.getTitle(vid, jahr, nummer) & ", Sachbearbeitung: " & gv1DT.Rows(0).Item(30).ToString.Trim
    End Sub



    Sub setLogfile(logfile As String)
        With My.Log.DefaultFileLogWriter
#If DEBUG Then
            '.CustomLocation = mgisUserRoot & "logs\"
            'logfile = "d:\" & "" ' & Environment.UserName & "_"
#Else
#End If
            '.CustomLocation = My.Computer.FileSystem.SpecialDirectories.Temp & "\mgis_logs\"
            .CustomLocation = logfile '
            '.BaseFileName = GisUser.username & "_" & Format(Now, "yyyyMMddhhmmss")
            '.BaseFileName = Environment.UserName & "__" & Format(Now, "yyyyMMddhhmmss")
            .BaseFileName = Environment.UserName & "_" & Now.ToString("yyyy_MM_dd_HH_mm_ss")
            .AutoFlush = True
            .Append = False
        End With
    End Sub
    Sub l(text As String)
        ' Debug.Print(text)
        My.Log.WriteEntry(text)
    End Sub
    Private Sub btnstartProbaug_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Dim si As New ProcessStartInfo
        si.FileName = "I:\pro-baug\exe\ProBAUG1.EXE  "
        'si.WorkingDirectory = "C:\kreisoffenbach\probaugView"
        si.Arguments = "I:\pro-baug\exe\ProBAUG.ini"
        '" /vid=" & myGlobalz.sitzung.aktVorgangsID
        Process.Start(si)
    End Sub

    Private Sub btnabbruchg_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        End
    End Sub

    Private Sub btnAntragsteller_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        If eingabeOK(perso) Then
            perso.Rolle = "Antragsteller/in"
            Dim persoid As Integer = clsTools.beteiligtenSpeichernNEU(perso, initial, CInt(vid), "sqls")
            If persoid < 1 Then
                MessageBox.Show("Fehler beim Schreiben der PersonenDaten aus Probaug!" & Environment.NewLine &
                perso.tostring(), "Übernehmen Sie die Daten innerhalb von Paradigma!")
            End If
        End If
    End Sub

    Private Function eingabeOK(perso As PocoPerson) As Boolean
        If perso.Name.Trim.Length < 3 Then
            MsgBox("Keine  Name angegeben!")
            Return False
        End If

        Return True
    End Function

    Private Sub btnRaumbezug_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        MsgBox("in Arbeit")
    End Sub
End Class
