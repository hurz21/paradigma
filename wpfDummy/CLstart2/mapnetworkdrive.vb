Namespace CLstart
    Public Class mapnetworkdrive
        ''' <summary>
        ''' Disconnects a network drive
        ''' </summary>
        ''' <param name="drive">Drive (z.B. L:)</param>
        Public Shared Sub MapNetworkDriveDisconnect(ByVal drive As String)
            Using p As New Process()
                p.StartInfo.FileName = "net"
                p.StartInfo.Arguments = String.Format("use {0} /DELETE", drive)
                p.StartInfo.UseShellExecute = False
                p.Start()
            End Using
        End Sub
        ''' <summary>
        ''' Connects a network drive
        ''' </summary>
        ''' <param name="drive">The drive letter (e.g. L:)</param>
        ''' <param name="server">The UNC path to the remote drive (e.g. \\MyServer\MyPrinter)</param>
        ''' <param name="user">The User</param>
        ''' <param name="password">The Password Used For Login</param>
        Public Overloads Shared Sub MapNetworkDriveConnect(ByVal drive As String, ByVal server As String, ByVal user As String, ByVal password As String)
            Using p As New Process()
                p.StartInfo.FileName = "net"
                p.StartInfo.Arguments = String.Format("use {0} {1} /user:{2} {3}", drive, server, "", "")
                p.StartInfo.UseShellExecute = False
                p.Start()
            End Using
        End Sub
        Public Overloads Shared Sub MapNetworkDriveConnect(ByVal drive As String, ByVal server As String)
            Using p As New Process()
                p.StartInfo.FileName = "net"
                p.StartInfo.Arguments = String.Format("use {0} {1} /persistent:yes ", drive, server)
                p.StartInfo.UseShellExecute = False
                p.Start()
            End Using
        End Sub

        ' •————————————————————————————————————————————————————————————————————————————————————————————————•
        ' | Beschreibung:                                                                              |
        ' |                                                                                            |
        ' | Usings                                                                                         |
        ' | using System.Diagnostics;                                                                      |
        ' |                                                                                            |
        ' | Erklärung                                                                                      |
        ' | Mit Hilfe dieses Snippets können Sie ganz leich Netzlaufwerke verbinden und trennen,           |
        ' | auch wenn diese ein Passwort zur Auth. brauchen. (z.B. FTP oder WebDAV)                        |
        ' |                                                                                            |
        ' | Syntax                                                                                         |
        ' | //Nur den Laufwerksbuchstaben angeben und fertig                                               |
        ' | MapNetworkDriveDisconnect("L:");                                                               |
        ' |                                                                                            |
        ' | //Bei einer Verbindung die einen Username und PW braucht                                       |
        ' | MapNetworkDriveConnect("N:", "https://webdav.myserver.ms", "user", "pw");                      |
        ' |                                                                                            |
        ' | //Bei einer nicht passwort geschützten Verbindung                                              |
        ' | MapNetworkDriveConnect("N:", "\\server\freigabe", string.Empty, string.Empty);                 |
        ' | ' •——————————————————————————————————————————————————————————————————————————————————————• |

    End Class
End Namespace