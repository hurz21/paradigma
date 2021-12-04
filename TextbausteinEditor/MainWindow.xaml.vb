Imports System.Text

Class MainWindow
    Public Property TB_RootPath As String = "O:\UMWELT\B\GISDatenEkom\Textbausteine"
    Public Property username As String = Environment.GetEnvironmentVariable("username")
    Public Property aktTB As New TB_Auswahl(TB_RootPath)
    Private Sub MainWindow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        initGruppenCombo()
        '  rtf2Box()
        initAdmin()
        e.Handled = True
    End Sub

    Private Sub initGruppenCombo()
        Dim startDir As New IO.DirectoryInfo(TB_RootPath)
        Dim dir As IO.DirectoryInfo
        For Each dir In startDir.GetDirectories
            Console.WriteLine(dir.Name)
            cmbGruppe.Items.Add(dir.Name)
        Next
        cmbGruppe.IsDropDownOpen = True 'SelectedValue = "Kostenfestsetzung"
    End Sub

    Private Sub cmbGruppe_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbGruppe.SelectedItem Is Nothing Then Exit Sub
        Dim tttt As String
        tttt = CStr(cmbGruppe.SelectedValue.ToString)
        aktTB.Gruppe = tttt.Replace("System.Windows.Controls.ComboBoxItem: ", "")
        initsubdir()
        e.Handled = True
    End Sub

    Private Sub rtf2Box()
        Clipboard.Clear()
        aktTB.datei = aktTB.buildFullpath()
        Dim sr As New IO.StreamReader(aktTB.datei)
        aktTB.inhalt = sr.ReadToEnd()
        sr.Close()
        sr.Dispose()
        Dim stream As New IO.MemoryStream(ASCIIEncoding.Default.GetBytes(aktTB.inhalt))
        rtfbox.Selection.Load(stream, DataFormats.Rtf)
        Clipboard.SetText(aktTB.inhalt, TextDataFormat.Rtf)
    End Sub

    Private Sub initsubdir()
        Dim startDir As New IO.DirectoryInfo(IO.Path.Combine(TB_RootPath, aktTB.Gruppe))
        Dim datei As IO.FileInfo
        cmbSubdir.Items.Clear()
        For Each datei In startDir.GetFiles
            Console.WriteLine(datei.Name)
            cmbSubdir.Items.Add(datei.Name.Replace(".rtf", "").Replace(".RTF", "").Replace(".Rtf", ""))
        Next
        tbInfo.Text = "Der Textbaustein befindet sich nun in der Zwischenablage. Mit Strg-v fügen Sie Ihn in das Word-Dokument ein!"
        cmbSubdir.IsDropDownOpen = True
    End Sub

    Private Sub cmbSubdir_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
        If cmbSubdir.SelectedItem Is Nothing Then Exit Sub
        Dim tttt As String
        tttt = CStr(cmbSubdir.SelectedValue.ToString)
        aktTB.subdir = tttt.Replace("System.Windows.Controls.ComboBoxItem: ", "").Replace(".rtf", "").Replace(".RTF", "").Replace(".Rtf", "")

        initTb()
        e.Handled = True
    End Sub

    Private Sub initTb()
        rtf2Box()
    End Sub

    Private Sub initAdmin()
        If isAdmin() Then
            stckAdmin.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Function isAdmin() As Boolean
        If username.ToLower = "weyers_g" Or
            username.ToLower = "nehler_u" Or
                 username.ToLower = "feinen_js" Or
            username.ToLower = "kuhn_p" Then
            Return True
        End If
        Return False
    End Function

    Private Sub neu_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Process.Start(aktTB._rootpath)
        e.Handled = True
    End Sub

    Private Sub edit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        If String.IsNullOrEmpty(aktTB.datei.Trim) Then
            MsgBox("Es wurde noch keine Datei ausgewählt")
            Exit Sub
        End If
        Process.Start(aktTB.datei)
        e.Handled = True
    End Sub

    Private Sub exit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        End
    End Sub
End Class

Public Class TB_Auswahl
    Public Property Gruppe As String = ""
    Public Property subdir As String = ""
    Public Property inhalt As String = ""
    Public Property header As String = ""
    Public Property datei As String = ""
    Public Property _rootpath As String
    Sub New(ByVal rootpath As String)
        _rootpath = rootpath
    End Sub
    Function buildSubdirPath() As String
        Dim sd As String
        sd = IO.Path.Combine(_rootpath, Gruppe, subdir)
        Return sd
    End Function
    Function buildFullpath() As String
        Dim sd As String
        sd = IO.Path.Combine(_rootpath, Gruppe, subdir)
        sd = sd & ".rtf"
        Return sd
    End Function
End Class
