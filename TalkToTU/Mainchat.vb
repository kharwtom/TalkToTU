Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading

Public Class Mainchat
    Public ip, user, port As String
    Dim lis As New Thread(AddressOf listen)
    Public cc As TcpClient
    Public ccr As StreamReader
    Public ccw As StreamWriter
    Public ccns As NetworkStream
    Public online As Boolean
    Public serverSocket As TcpListener

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox2.AppendText("ME: " + TextBox1.Text & Environment.NewLine)
        ccw.WriteLine(user + ": " + TextBox1.Text)
        'ccw.WriteLine(TextBox1.Text)
        ccw.Flush()
        TextBox1.Clear()
    End Sub
    Private Sub Mainchat_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If online Then

            lis.Abort()
            cc.Close()
            ccr.Close()
            ccw.Close()
            ccns.Close()

        End If

    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        TextBox1.Enabled = True
        TextBox2.Enabled = True
        Button1.Enabled = True
        online = True
        Label2.BackColor = Color.Green
        Label2.Text = "Status : ONLINE"

        Button2.Visible = False


#Disable Warning BC40000 ' Type or member is obsolete
        serverSocket = New TcpListener(Convert.ToInt32(port))
#Enable Warning BC40000 ' Type or member is obsolete
        serverSocket.Start()
        cc = serverSocket.AcceptTcpClient
        ccns = cc.GetStream
        ccr = New IO.StreamReader(ccns, Encoding.ASCII, True)
        ccw = New IO.StreamWriter(ccns)
        lis.IsBackground = True
        lis.Start()
    End Sub


    Private Sub Mainchat_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        ip = Form1.ip
        port = Form1.port
        user = Form1.user
        online = False
    End Sub
    Sub listen()
        Dim rec As String = ""
        While True
            Try
                rec = ccr.ReadLine()
            Catch
                TextBox2.AppendText("Disconnected by host")
                Return
            End Try
            If Not rec Is Nothing Then
                    TextBox2.AppendText(rec & Environment.NewLine)
                End If
            Thread.Sleep(100)


        End While
    End Sub
End Class