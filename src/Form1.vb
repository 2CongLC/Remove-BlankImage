Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.IO.Compression
Imports System.Net
Imports System.Net.Sockets
Imports System.Net.WebSockets
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class Form1
    Private IsRun As Boolean
    Private TotalPage As Integer = 0

#Region "Khu vực xử lý Form1"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NotifyIcon1.Visible = True

    End Sub
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
            If IsRun = True Then
                NotifyIcon1.Icon = Icon.FromHandle(CType(ImageList1.Images(1), Bitmap).GetHicon)
            Else
                NotifyIcon1.Icon = Icon.FromHandle(CType(ImageList1.Images(0), Bitmap).GetHicon)
            End If
            NotifyIcon1.Visible = True
            BalloonTip("Phần mềm xóa ảnh trắng - Remove Blank Image", "2conglc.vn@gmail.com", ToolTipIcon.None)
        End If
    End Sub

#End Region
#Region "Khu vực xử lí NotifyIcon1"
    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        UnhideProcess()
    End Sub

    Private Sub NotifyIcon1_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        If e.Button = MouseButtons.Left Then
            If IsRun = True AndAlso BackgroundWorker1.IsBusy Then
                BackgroundWorker1.CancelAsync()
                IsRun = False
                NotifyIcon1.Icon = Icon.FromHandle(CType(ImageList1.Images(0), Bitmap).GetHicon)
            Else
                ToolStripProgressBar1.Value = ToolStripProgressBar1.Minimum
                Button1.Text = "Dừng lại"
                ListView1.Items.Clear()
                BackgroundWorker1.RunWorkerAsync()
                IsRun = True
                NotifyIcon1.Icon = Icon.FromHandle(CType(ImageList1.Images(1), Bitmap).GetHicon)
            End If
        End If
    End Sub
    Private Sub BalloonTip(title As String, text As String, icon As ToolTipIcon)
        NotifyIcon1.BalloonTipTitle = title
        NotifyIcon1.BalloonTipText = text
        NotifyIcon1.BalloonTipIcon = icon
        NotifyIcon1.ShowBalloonTip(10000)
    End Sub
    Private Sub ShowFormToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowFormToolStripMenuItem.Click
        UnhideProcess()
    End Sub
#End Region
#Region "Thành phần khác"
    Private Sub Start()
        IsRun = True
    End Sub
    Private Sub [Stop]()
        IsRun = False
    End Sub
    Private Sub HideProcess()
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
        NotifyIcon1.Visible = True
    End Sub
    Private Sub UnhideProcess()
        Me.WindowState = FormWindowState.Normal
        Me.ShowInTaskbar = True
        NotifyIcon1.Visible = True
    End Sub
#End Region
#Region "Tìm kiếm File"
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If BackgroundWorker1.IsBusy AndAlso IsRun = True Then
            BackgroundWorker1.CancelAsync()
            IsRun = False
            NotifyIcon1.Icon = Icon.FromHandle(CType(ImageList1.Images(0), Bitmap).GetHicon)
        Else
            If Directory.Exists(TextBox1.Text) Then
                ToolStripProgressBar1.Value = ToolStripProgressBar1.Minimum
                Button1.Text = "Dừng lại"
                ListView1.Items.Clear()
                BackgroundWorker1.RunWorkerAsync()
                IsRun = True
                NotifyIcon1.Icon = Icon.FromHandle(CType(ImageList1.Images(1), Bitmap).GetHicon)
            End If
        End If
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub
    ''' <summary>
    ''' Thêm tệp tin vào trong Listview
    ''' </summary>
    ''' <param name="file"></param>
    Private Sub AddToListView(ByVal file As String)

        Dim item As ListViewItem = New ListViewItem(file)

        item.SubItems.Add(Math.Ceiling(New FileInfo(file).Length / 1024.0F).ToString("0 KB"))
        If ImageExtension.IsBlank(file) = True Then
            IO.File.Delete(file)
            TotalPage += 1
            item.SubItems.Add("Đã xóa !")
        End If

        ListView1.Invoke(CType((Sub()
                                    ListView1.BeginUpdate()
                                    ListView1.Items.Add(item)
                                    ListView1.EndUpdate()
                                End Sub), Action))
    End Sub
    ''' <summary>
    ''' Tìm kiếm tệp tin
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="searchPattern"></param>
    ''' <returns></returns>
    Public Function ScanDirectory(ByVal dt As String, ByVal searchPattern As String) As List(Of String)
        Dim list As New List(Of String)
        If Directory.Exists(dt) AndAlso searchPattern <> "" Then
            Dim list2 As List(Of String) = Enumerable.ToList(Of String)(Directory.GetFiles(dt, searchPattern, SearchOption.AllDirectories))
            Dim num As Integer = 1
            Dim i As Integer
            For i = 0 To list2.Count - 1
                Application.DoEvents()
                Dim info As New FileInfo(list2(i))
                list.Add(list2(i))
                num += 1
            Next i
        End If
        Return list
    End Function

#End Region
#Region "xử lí BackgroundWorker1"
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If Directory.Exists(TextBox1.Text) Then
            Dim dirs As New List(Of String)
            dirs = ScanDirectory(TextBox1.Text, TextBox2.Text)
            Dim count As Integer = dirs.Count
            Dim i As Integer
            TotalPage = 0
            ListView1.Items.Clear()
            For i = 0 To count - 1
                'Tham chiếu tiến trình progressbar
                BackgroundWorker1.ReportProgress(CInt(i / count * 100))
                'Tham chiếu tệp đang trong quá trình sử lý
                Label3.Invoke(CType((Sub()
                                         Label3.Text = String.Format("Đang xóa ảnh : {0}", dirs(i))
                                     End Sub), Action))
                'Thêm tệp tin vào danh sách listview1
                AddToListView(dirs(i))
                'Tiến trình đếm trang          
                Label1.Invoke(CType(Sub()
                                        Label1.Text = "Tổng số ảnh đã xóa : " & TotalPage & "/" & ListView1.Items.Count & " Tệp."
                                    End Sub, Action))

            Next
            BackgroundWorker1.ReportProgress(100)
        End If
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        If Not BackgroundWorker1.CancellationPending Then
            ToolStripStatusLabel2.Text = e.ProgressPercentage & "%"
            NotifyIcon1.Text = "Tiến độ : " & e.ProgressPercentage & "%"
            ToolStripProgressBar1.PerformStep()
        End If
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        If ToolStripProgressBar1.Value = ToolStripProgressBar1.Maximum Then
            Label3.Text = "Đã xóa xong"
            Dim result As String = "Tổng số ảnh đã xóa : " & TotalPage & "/" & ListView1.Items.Count & " Tệp."
            BalloonTip("Đã đếm xong", result, ToolTipIcon.Info)
            NotifyIcon1.Text = Me.Text
            Button1.Text = "Thực hiện"
            UnhideProcess()
            IsRun = False
            NotifyIcon1.Icon = Icon.FromHandle(CType(ImageList1.Images(0), Bitmap).GetHicon)
        End If
    End Sub
#End Region
#Region "Quản lí tệp tìm được"
    Private Function GetRowListView(id As Integer) As String
        Dim result As String = ""
        Try
            result = ListView1.Items.Item(ListView1.FocusedItem.Index).SubItems.Item(id).Text
            Return result
        Catch ex As Exception
        End Try
    End Function

    Private Sub MơThưMucChưaTêpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MơThưMucChưaTêpToolStripMenuItem.Click

        Process.Start("explorer.exe", "/select," & GetRowListView(1) & "\" & GetRowListView(0))
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.Close
    End Sub

#End Region

End Class
