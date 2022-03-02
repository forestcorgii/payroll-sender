Namespace Configuration
    Public Class General
        Public Database As DatabaseConfiguration

        Public Valid As Boolean
        Sub New()
            Database = New DatabaseConfiguration
            Valid = True

            Try
                If Not Database.Setup("TIME_RECORDER_DB_URL") Then Throw New Exception("TIME_RECORDER_DB_URL")
            Catch : Valid = False
            End Try

            If Not Valid Then MessageBox.Show("Some Configurations are Missing or Invalid.", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Sub

        Public Overrides Function ToString() As String
            Return MyBase.ToString()
        End Function
    End Class
End Namespace
