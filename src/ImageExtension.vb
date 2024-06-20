Imports System
Imports System.IO
Imports System.Text

''' <summary>
''' Ngày tạo 2020.
''' Ngày chỉnh sửa và đưa lên github 06/2024
''' Email : 2conglc.vn@gmail.com
''' </summary>
Public Class ImageExtension

    ''' <summary>
    ''' Kiểm tra hình ảnh kiểu byte
    ''' </summary>
    ''' <param name="src"></param>
    ''' <returns></returns>
    Friend Shared Function IsBlank(src As Byte()) As Boolean
        Dim ms As MemoryStream = New MemoryStream(src)
        Dim img As Drawing.Image = Drawing.Image.FromStream(ms)
        Dim bitmap As Bitmap = New Bitmap(img)
        For i As Integer = 0 To bitmap.Width - 1
            For j As Integer = 0 To bitmap.Height - 1
                Dim pixel As Color = bitmap.GetPixel(i, j)
                If pixel.R < 240 OrElse pixel.G < 240 OrElse pixel.B < 240 Then
                    Return False
                End If
            Next
        Next
        Return True
        ms.Close()
    End Function

    ''' <summary>
    ''' Kiểm tra hình ảnh kiểu image
    ''' </summary>
    ''' <param name="src"></param>
    ''' <returns></returns>
    Friend Shared Function IsBlank(src As Drawing.Image) As Boolean
        Dim bitmap As Bitmap = New Bitmap(src)
        For i As Integer = 0 To bitmap.Width - 1
            For j As Integer = 0 To bitmap.Height - 1
                Dim pixel As Color = bitmap.GetPixel(i, j)
                If pixel.R < 240 OrElse pixel.G < 240 OrElse pixel.B < 240 Then
                    Return False
                End If
            Next
        Next
        Return True
    End Function

    ''' <summary>
    ''' Kiểm tra hình ảnh kiểu đường dẫn
    ''' </summary>
    ''' <param name="src"></param>
    ''' <returns></returns>
    Friend Shared Function IsBlank(src As String) As Boolean
        Using fs1 As Stream = File.OpenRead(src)
            Dim buffer(fs1.Length) As Byte
            fs1.Read(buffer, 0, buffer.Length)
            Using fs2 As Stream = New MemoryStream(buffer)
                Dim img As Drawing.Image = Drawing.Image.FromStream(fs2)
                Dim bitmap As Bitmap = New Bitmap(img)
                For i As Integer = 0 To bitmap.Width - 1
                    For j As Integer = 0 To bitmap.Height - 1
                        Dim pixel As Color = bitmap.GetPixel(i, j)
                        If pixel.R < 240 OrElse pixel.G < 240 OrElse pixel.B < 240 Then
                            Return False
                        End If
                    Next
                Next
                Return True
            End Using
        End Using
    End Function

End Class
