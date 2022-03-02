#Region "Serialization"
Imports System.Xml.Serialization
Imports System.IO

Public Class XmlSerialization
    Public Shared Sub WriteToFile(filePath As String, _type As Object)
        Dim w As TextWriter = Nothing
        Dim s As New XmlSerializer(_type.GetType)
        w = New StreamWriter(filePath)
        s.Serialize(w, _type)
        w.Close()
    End Sub

    Public Shared Function ReadFromFile(ByVal filename As String, item As Object)
        Dim newTemplate As Object = Nothing
        Dim r As TextReader = Nothing
        Dim s As New XmlSerializer(item.GetType)
        r = New StreamReader(filename)
        newTemplate = s.Deserialize(r)
        r.Close()
        Return newTemplate
    End Function
End Class
#End Region

