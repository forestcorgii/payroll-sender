Imports System.Text.RegularExpressions

Namespace Configuration
    Public Class DatabaseConfiguration
        Public user As String = ""
        Public password As String = ""
        Public host As String = ""
        Public port As String = ""

        Public name As String = ""

        Sub New()
        End Sub

        Public Function Setup(envVar As String)
            Return ParseEnvVar(Environment.GetEnvironmentVariable(envVar))
        End Function

        Public Function ParseEnvVar(raw As String)
            Try
                If raw <> "" Then
                    Dim mysql_regex As New Regex("^([\w@]+):([\w@!]+)@([\w-.]+):([\d]+)\/([\w\-]+)$")
                    Dim mysql_match As Match = mysql_regex.Match(raw)
                    user = mysql_match.Groups(1).Value
                    password = mysql_match.Groups(2).Value
                    host = mysql_match.Groups(3).Value
                    port = mysql_match.Groups(4).Value
                    name = mysql_match.Groups(5).Value
                Else
                    Throw New Exception(String.Format("Parsing Failed, please check {0}.", raw))
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End Try
            Return True
        End Function

        Public ReadOnly Property ConnectionString
            Get
                Return String.Format("Server={0};Uid={1};Pwd={2};port={3}{4};Convert Zero Datetime=True;command Timeout=20000;SslMode=None",
                                     host, user, password, port, IIf(name = "", "", ";database=" & name))
            End Get
        End Property


        Public Overrides Function ToString() As String
            Return String.Format("{0}:{1}@{2}:{3}/{4}", user, password, host, port, name)
        End Function
    End Class
End Namespace