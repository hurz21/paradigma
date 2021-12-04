using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csSerialserv
{
    class clsCGI4cs
    {
        public Encoding enc = Encoding.GetEncoding("iso-8859-1");
        public string CGI_Accept;
        public string CGI_AuthType;
        public string CGI_ContentLength;
        public string CGI_ContentType;
        public string CGI_Cookie;
        public string CGI_GatewayInterface;
        public string CGI_PathInfo;
        public string CGI_PathTranslated;
        public string CGI_QueryString;
        public string CGI_Referer;
        public string CGI_RemoteAddr;
        public string CGI_RemoteHost;
        public string CGI_RemoteIdent;
        public string CGI_RemoteUser;
        public string CGI_RequestMethod;
        public string CGI_ScriptName;
        public string CGI_ServerSoftware;
        public string CGI_ServerName;
        public string CGI_ServerPort;
        public string CGI_ServerProtocol;
        public string CGI_UserAgent;
        public long lContentLength;     // CGI_ContentLength converted to Long
        public string sErrorDesc;   // constructed error message
        public string sFormData; // url-encoded data sent by the server

        internal partial struct pair
        {
            public string Name;
            public string Value;
        } 
        // array of name=value pairs
        public pair[] tPair;
        // Private sEmailValue As String ' webmaster's/your email address
        public string sEmail;
        public bool Alive;
        private void InitCgi()
        {
            try // ==============================
            {
                // Get the environment variables
                // ==============================
                // 
                // Environment variables will vary depending on the server.
                // Replace any variables below with the ones used by your server.
                // 
                CGI_Accept = Environment.GetEnvironmentVariable("HTTP_ACCEPT");
                CGI_AuthType = Environment.GetEnvironmentVariable("AUTH_TYPE");
                CGI_ContentLength = Environment.GetEnvironmentVariable("CONTENT_LENGTH");
                CGI_ContentType = Environment.GetEnvironmentVariable("CONTENT_TYPE");
                CGI_Cookie = Environment.GetEnvironmentVariable("HTTP_COOKIE");
                CGI_GatewayInterface = Environment.GetEnvironmentVariable("GATEWAY_INTERFACE");
                CGI_PathInfo = Environment.GetEnvironmentVariable("PATH_INFO");
                CGI_PathTranslated = Environment.GetEnvironmentVariable("PATH_TRANSLATED");
                CGI_QueryString = Environment.GetEnvironmentVariable("QUERY_STRING");
                CGI_Referer = Environment.GetEnvironmentVariable("HTTP_REFERER");
                CGI_RemoteAddr = Environment.GetEnvironmentVariable("REMOTE_ADDR");
                CGI_RemoteHost = Environment.GetEnvironmentVariable("REMOTE_HOST");
                CGI_RemoteIdent = Environment.GetEnvironmentVariable("REMOTE_IDENT");
                CGI_RemoteUser = Environment.GetEnvironmentVariable("REMOTE_USER");
                CGI_RequestMethod = Environment.GetEnvironmentVariable("REQUEST_METHOD");
                CGI_ScriptName = Environment.GetEnvironmentVariable("SCRIPT_NAME");
                CGI_ServerSoftware = Environment.GetEnvironmentVariable("SERVER_SOFTWARE");
                CGI_ServerName = Environment.GetEnvironmentVariable("SERVER_NAME");
                CGI_ServerPort = Environment.GetEnvironmentVariable("SERVER_PORT");
                CGI_ServerProtocol = Environment.GetEnvironmentVariable("SERVER_PROTOCOL");
                CGI_UserAgent = Environment.GetEnvironmentVariable("HTTP_USER_AGENT");
                lContentLength = Convert.ToInt64(CGI_ContentLength);     // convert to long
                                                                         //   global::My.Application.Log.WriteEntry("CGI_RequestMethod CGI_RequestMethod " + CGI_RequestMethod);
            }
            catch (Exception e)
            {
                sErrorDesc = ", Fehler: " + Environment.NewLine + e.Message + " " + Environment.NewLine + e.StackTrace + " " + Environment.NewLine + e.Source + " ";


                // If Logging Then My.Application.Log.WriteEntry(sErrorDesc)
            }
        }

        public clsCGI4cs(string sEmailValue)
        {
            try
            {
                InitCgi();                    // Load environment vars and perform other initialization
                GetFormData();            // Read data sent by the server 
                sEmail = sEmailValue;
            }
            catch (Exception e)
            {
                sErrorDesc = ", Fehler: " + Environment.NewLine + e.Message + " " + Environment.NewLine + e.StackTrace + " " + Environment.NewLine + e.Source + " ";


                // If Logging Then My.Application.Log.WriteEntry(sErrorDesc)
            }
        }


       
            public bool Send(string s)
            {
                // ======================
                // myCGI.Send output to STDOUT
                // ======================
                s = s + Environment.NewLine;
                Console.WriteLine(s);
                return true;
            }

            public bool Send(double s)
            {
                // ======================
                // myCGI.Send output to STDOUT
                // ======================
                string a = s.ToString() + Environment.NewLine;
                Console.WriteLine(a);
                return true;
            }

            public bool SendHeaderAJAX()
            {
                // Console.WriteLine("Status: 200 OK" & vbCrLf)
                Console.WriteLine("Content-type: text/html; charset=ISO-8859-1" + Environment.NewLine);
                return true;
            }

            public bool SendHeader(string sTitle)
            {
                Console.WriteLine("Status: 200 OK");
                Console.WriteLine("Content-type: text/html" + Environment.NewLine);
                Console.WriteLine("<HTML><HEAD><TITLE>" + sTitle + "</TITLE></HEAD>");
                Console.WriteLine("<BODY>");
                return true;
            }

            public bool SendFooter()
            {
                // ==================================
                // standardized footers can be added
                // ==================================
                Console.WriteLine("</BODY></HTML>");
                return true;
            }

            public bool SendB(string s)
            {
                // ============================================
                // Send output to STDOUT without vbCrLf.
                // Use whenmyCGI.Sending binary data. For example,
                // images sent with "Content-type image/jpeg".
                // ============================================
                Console.Write(s);
                return true;
            }
        public string GetCgiValue(string cgiName)
        {
            string GetCgiValueRet = default;
            // ====================================================================
            // Accept the name of a pair
            // Return the value matching the name
            // 
            // tPair(0) is always empty.
            // An empty string will be returned
            // if cgiName is not defined in the form (programmer error)
            // or, a select type form item was used, but no item was selected.
            // 
            // Multiple values, separated by a semi-colon, will be returned
            // if the form item uses the "multiple" option
            // and, more than one selection was chosen.
            // The calling procedure must parse this string as needed.
            // ====================================================================
            int n;
            try
            {
                // GetCgiValue = cgiName
                GetCgiValueRet = "";
                if (tPair is null)
                    return "";
                var loopTo = tPair.GetUpperBound(0);
                for (n = 1; n <= loopTo; n++)
                {
                    if ( cgiName.ToUpper() ==  tPair[n].Name.ToUpper())
                    {
                        if (string.IsNullOrEmpty(GetCgiValueRet))
                        {
                            GetCgiValueRet = tPair[n].Value;
                        }
                        else                         // allow for multiple selections
                        {
                            GetCgiValueRet = GetCgiValueRet + ";" + tPair[n].Value;
                        }
                    }
                }
            }
            // GetCgiValue = cgiName
            catch (Exception e)
            {
                sErrorDesc = ", Fehler: " + Environment.NewLine + e.Message + " " + Environment.NewLine + e.StackTrace + " " + Environment.NewLine + e.Source + " ";


                GetCgiValueRet = "";
            }

            return GetCgiValueRet;
        }
        private string UrlDecode(string sEncoded)
        {
            string UrlDecodeRet = default;
            UrlDecodeRet = sEncoded;
            // ========================================================
            // Accept url-encoded string
            // Return decoded string
            // ========================================================

            int pos;            // position of InStr target
            if (string.IsNullOrEmpty(sEncoded))
                return "";

            // convert "+" to space
            // pos = 0
            // Do
            // pos = InStr(pos + 1,  sEncoded,   "+")
            // If pos = 0 Then Exit Do
            // Mid$(sEncoded, pos, 1) = " "
            // Loop
            sEncoded = sEncoded.Replace("+", " ");
            // convert "%xx" to character
            pos = 0;
            try
            {
                do
                {
                    pos = Strings.InStr(pos + 1, sEncoded, "%");
                    if (pos == 0)
                        break;
                    ;
#error Cannot convert AssignmentStatementSyntax - see comment for details
                    /* Cannot convert AssignmentStatementSyntax, System.InvalidCastException: Unable to cast object of type 'Microsoft.CodeAnalysis.CSharp.Syntax.EmptyStatementSyntax' to type 'Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax'.
                       at ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.VisitAssignmentStatement(AssignmentStatementSyntax node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\MethodBodyExecutableStatementVisitor.cs:line 134
                       at ICSharpCode.CodeConverter.CSharp.ByRefParameterVisitor.CreateLocals(VisualBasicSyntaxNode node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\ByRefParameterVisitor.cs:line 53
                       at ICSharpCode.CodeConverter.CSharp.ByRefParameterVisitor.AddLocalVariables(VisualBasicSyntaxNode node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\ByRefParameterVisitor.cs:line 43
                       at ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.DefaultVisitInnerAsync(SyntaxNode node) in D:\GitWorkspace\CodeConverter\CodeConverter\CSharp\CommentConvertingMethodBodyVisitor.cs:line 29

                    Input:

                                    Mid$(sEncoded, pos, 1) = Global.Microsoft.VisualBasic.Strings.Chr(CInt("&H" & (Global.Microsoft.VisualBasic.Strings.[Mid](sEncoded, pos + 1, 2))))

                     */
                    sEncoded = Strings.Left(sEncoded, pos) + Strings.Mid(sEncoded, pos + 3);
                }
                while (true);
                return sEncoded;
            }
            catch (Exception e)
            {
                sErrorDesc = ", Fehler: " + Constants.vbCrLf + e.Message + " " + Constants.vbCrLf + e.StackTrace + " " + Constants.vbCrLf + e.Source + " ";


            }

            return UrlDecodeRet;
        }


    }
}
