Imports System
Imports System.IO
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Public Class Chat
    Public c As TcpClient
    Public cr As StreamReader
    Public cw As StreamWriter
    Public cns As NetworkStream
    Private lis As Thread
    Dim i, ip, port As String
    Private Sub Chat_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False

        ip = Form1.ip_text.Text
        port = Form1.port_text.Text
        i = Form1.user
        c = New Net.Sockets.TcpClient
        Try
            'c.Connect("128.199.83.36", 34260)
            c.Connect(ip, Convert.ToInt32(port))
        Catch
            MessageBox.Show("Failed to connect: " & e.ToString, "Error")
        End Try
        cns = c.GetStream
        cr = New IO.StreamReader(cns, Encoding.ASCII, True)
        cw = New IO.StreamWriter(cns)
        cw.WriteLine(i + " Connected")
        cw.Flush()
        lis = New Thread(AddressOf list)
        lis.IsBackground = True
        lis.Start()
        Me.Text = "Chat with " + ip
        Label1.Text = "Now Chat with" + ip

    End Sub
    Private Sub Chat_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        lis.Abort()
        c.Close()
        cr.Close()
        cw.Close()
        cns.Close()


    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox1.AppendText("ME: " + TextBox2.Text & Environment.NewLine)
        cw.WriteLine(i + ": " + TextBox2.Text)
        cw.Flush()
        TextBox2.Clear()
    End Sub
    Private Sub list()
        Dim rec As String = ""
        While True
            Try
                rec = cr.ReadLine()
            Catch
                TextBox1.AppendText("Disconnected by host")
                Return
            End Try
            If Not rec Is Nothing Then
                TextBox1.AppendText(rec & Environment.NewLine)
            End If
            Thread.Sleep(100)
        End While
    End Sub

End Class