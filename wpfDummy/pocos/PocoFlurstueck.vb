Public Class PocoFlurstueck

    Public Property probaugGemcode As Integer
    Public Property flaecheqm As Double
    Public Property zeigtauf As String
    Public Property weistauf As String
    Public Property gebucht As String
    Public Property fsgml As String
    Public Property GKrechts() As Double
    Public Property GKhoch() As Double
    Public Property gid As Integer = 0
    Public Property gueltig As String = ""
    Public Property genese As Integer = 1

    Public Property gemeindeNr As Integer
    Public Property gemeindename As String

    Public Property grundbuchblattnr As String
    Public Property gemarkungstext As String

    Public Property gemarkungstextNORM As String
    Public Property flur As Integer
    Public Property FS As String

    Public Property gemcode As Integer
    Public Property fstueckKombi As String
    Public Property zaehler As Integer
    Public Property nenner As Integer


    Property gemparms As New clsGemarkungsParams


    Public Function buildFS() As String
        Dim fs$, fuell$, fs1$, fs2$, fs3$, fs4$
        Try
            If _nenner > 9999 Or _zaehler > 9999 Then Return "-4712"
            fs1$ = "FS060" & _gemcode%.ToString
            fuell = "000"                                                        '_flur = CInt(Val(flur$)).ToString
            fs2$ = fuell.Substring(_flur.ToString.Length) & _flur
            fuell = "00000"
            fs3 = fuell.Substring((_zaehler.ToString).Length) + _zaehler.ToString
            fuell = "000"
            fs4 = fuell.Substring((_nenner.ToString).Length) + (_nenner.ToString) + "00"
            fs = fs1 + fs2 + fs3 + fs4
            Return fs
        Catch ex As Exception
            Return "-4711"
        End Try
    End Function
    Public Function getPROBAUGGemcode(ByVal gemarkung As String) As Integer
        Try
            Select Case CInt(Val(gemarkung))
                Case 2, 35, 60
                    Return 732 'bayerseich,egeksbach,im bruehl
                Case 4
                    Return 730 'Dreieichenhain 
                Case 5
                    Return 756 ' 756	Sprendlingen 
                Case 6
                    Return 752 '	Offenthal
                Case 7
                    Return 734  'Götzenhain
                Case 8
                    Return 726  'Buchschlag
                Case 9
                    Return 736  'Hainstadt 
                Case 10
                    Return 740  'Klein-Krotzenburg
                Case 11
                    Return 753  'Rembrücken 
                Case 12
                    Return 744  'Mainflingen
                Case 13
                    Return 759  'Zellhausen
                Case 14
                    Return 742  'Lämmerspiel      
                Case 15
                    Return 728  'Dietesheim       
                Case 16
                    Return 750  'Obertshausen     
                Case 17
                    Return 737  'Hausen           
                Case 18
                    Return 760  'Zeppelinheim     
                Case 19, 42
                    Return 748  'gravenbruch ,Neu-Isenburg
                Case 20
                    Return 739  'Jügesheim                
                Case 21
                    Return 731  'Dudenhofen               
                Case 22
                    Return 747  'Nieder-Roden             
                Case 23
                    Return 735  'Hainhausen               
                Case 24
                    Return 758  'Weiskirchen              
                Case 25
                    Return 757  'Urberach                 
                Case 26, 25
                    Return 749  'Ober-Roden               
                Case 28
                    Return 745  'Messenhausen             
                Case 29
                    Return 733 '	Froschhausen            
                Case 30
                    Return 741 '	Klein-Welzheim
                Case 32
                    Return 738  'Heusenstamm    
                Case 34
                    Return 755  'Seligenstadt   
                Case 36
                    Return 746 '	Mühlheim      
                Case 41, 23
                    Return 743  'Langen,oberlinden
                Case 40, 33
                    Return 729  'Dietzenbach die 33 ist meine persönl. vermutung
                Case 50
                    Return 751  'Offenbach 
                Case Else
                    'unbekannte_gemarkungen$ &= gemarkung & "; "
                    Return 0
            End Select
        Catch ex As Exception
            'mylog.log("ERROR: getGemcode: " & _
            '          ex.Message + " " + _
            '         ex.Source + " ")
            Return -4711
        End Try
    End Function
    Public Sub New()

    End Sub

    Public Function splitFstueckkombi() As Boolean
        Try
            Dim results = _fstueckKombi.Split("/"c)
            zaehler = CInt(results(0))
            If results.Length > 0 Then
                nenner = CInt(results(1))
            Else
                nenner = 0I
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function
    Public Function buildFstueckkombi() As String
        Try
            Return zaehler.ToString & "/" & nenner.ToString
        Catch ex As Exception
            Return "-1"
        End Try
    End Function

    Sub clear()
        gemarkungstext = ""
        gemcode = 0
        flur = 0
        zaehler = 0
        nenner = 0
        fstueckKombi = ""
        FS = ""
        flaecheqm = 0

    End Sub
End Class
