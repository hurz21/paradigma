Imports System.Security.Cryptography
Imports System.Runtime.InteropServices
Imports System.IO
Imports System
Imports System.Collections.Generic


'Class1.FileEncrypt("C:\\Users\\feinen_j\\downloads\\attribute_changer.exe",
'                   "C:\\Users\\feinen_j\\downloads\\attribute_changerENC.exe", "hurz")

'AesCrypt.FileDecrypt("C:\\Users\\feinen_j\\downloads\\attribute_changerENC.exe",
'                              "C:\\Users\\feinen_j\\downloads\\attribute_changerDEC.exe", "hurz")
Namespace CLstart
    Class AesCrypt
        Public Shared Property normpw As String = "$kldfKFSAK37236780!!*+++hHUDO723BNU!$hask+*jhds7!2929j$+jP*!hWrT$kldfKFSAK37236780!!*+++hHUDO723BNU!$hask+*jhds7!2929j$+jP*!hWrT"
        'Private EncryptionKey As String =        "$kldfKFSAK37236780!!*+++hHUDO723BNU!$hask+*jhds7!2929j$+jP*!hWrT$kldfKFSAK37236780!!*+++hHUDO723BNU!$hask+*jhds7!2929j$+jP*!hWrT"
        'https://ourcodeworld.com/articles/read/471/how-to-encrypt-and-decrypt-files-using-the-aes-encryption-algorithm-in-c-sharp
        Public Shared Sub FileEncrypt(ByVal inputFile As String, ByVal outfile As String, ByVal password As String)
            Dim salt As Byte() = GenerateRandomSalt()
            Dim fsCrypt As FileStream = New FileStream(outfile, FileMode.Create)
            Dim passwordBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(password)
            Dim AES As RijndaelManaged = New RijndaelManaged()
            AES.KeySize = 256
            AES.BlockSize = 128
            AES.Padding = PaddingMode.PKCS7
            Dim key = New Rfc2898DeriveBytes(passwordBytes, salt, 50000)
            AES.Key = key.GetBytes(CInt(AES.KeySize / 8))
            AES.IV = key.GetBytes(CInt(AES.BlockSize / 8))
            AES.Mode = CipherMode.CFB
            fsCrypt.Write(salt, 0, salt.Length)
            Dim cs As CryptoStream = New CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write)
            Dim fsIn As FileStream = New FileStream(inputFile, FileMode.Open)
            Dim buffer As Byte() = New Byte(1048575) {}
            Dim read As Integer
            Try
                While (CSharpImpl.__Assign(read, fsIn.Read(buffer, 0, buffer.Length))) > 0
                    cs.Write(buffer, 0, read)
                End While
                fsIn.Close()
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
            Finally
                cs.Close()
                fsCrypt.Close()
            End Try
        End Sub
        Public Shared Sub FileDecrypt(ByVal inputFile As String, ByVal outputFile As String, ByVal password As String)
            Dim passwordBytes As Byte() = System.Text.Encoding.UTF8.GetBytes(password)
            Dim salt As Byte() = New Byte(31) {}
            Dim fsCrypt As FileStream = New FileStream(inputFile, FileMode.Open)
            fsCrypt.Read(salt, 0, salt.Length)
            Dim AES As RijndaelManaged = New RijndaelManaged()
            AES.KeySize = 256
            AES.BlockSize = 128
            Dim key = New Rfc2898DeriveBytes(passwordBytes, salt, 50000)
            AES.Key = key.GetBytes(CInt(AES.KeySize / 8))
            AES.IV = key.GetBytes(CInt(AES.BlockSize / 8))
            AES.Padding = PaddingMode.PKCS7
            AES.Mode = CipherMode.CFB
            Dim cs As CryptoStream = New CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read)
            Dim fsOut As FileStream = New FileStream(outputFile, FileMode.Create)
            Dim read As Integer
            Dim buffer As Byte() = New Byte(1048575) {}

            Try

                While (CSharpImpl.__Assign(read, cs.Read(buffer, 0, buffer.Length))) > 0
                    fsOut.Write(buffer, 0, read)
                End While

            Catch ex_CryptographicException As CryptographicException
                Console.WriteLine("CryptographicException error: " & ex_CryptographicException.Message)
            Catch ex As Exception
                Console.WriteLine("Error: " & ex.Message)
            End Try

            Try
                cs.Close()
            Catch ex As Exception
                Console.WriteLine("Error by closing CryptoStream: " & ex.Message)
            Finally
                fsOut.Close()
                fsCrypt.Close()
            End Try
        End Sub

        Private Class CSharpImpl
            <Obsolete("Please refactor calling code to use normal Visual Basic assignment")>
            Shared Function __Assign(Of T)(ByRef target As T, value As T) As T
                target = value
                Return value
            End Function
        End Class

        Public Shared Function GenerateRandomSalt() As Byte()
            Dim data As Byte() = New Byte(31) {}

            Using rng As RNGCryptoServiceProvider = New RNGCryptoServiceProvider()

                For i As Integer = 0 To 10 - 1
                    rng.GetBytes(data)
                Next
            End Using

            Return data
        End Function
    End Class
End Namespace