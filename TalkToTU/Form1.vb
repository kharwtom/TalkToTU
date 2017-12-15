Imports System
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Public Class Form1
    Public sec As Integer = 5
    Public client As TcpClient
    Public sr As StreamReader
    Public sw As StreamWriter
    Public ns As NetworkStream
    Public user, ip, port As String
    Public ips, ports As String
    Private trd, trdd As Thread
    Public online As Boolean




    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        online = False
        Dim fileReader As System.IO.StreamReader
        Try
            fileReader = My.Computer.FileSystem.OpenTextFileReader("server.config")
            ips = fileReader.ReadLine()
            ports = fileReader.ReadLine()
        Catch
            MessageBox.Show("ERROR!!! server.config NOT FOUND!!!!!", "Error")
            Me.Close()
        End Try

    End Sub
    Private Sub Form1_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing
        If online Then
            client.Close()
            trd.Abort()
            trdd.Abort()
            ns.Close()
            sr.Close()
            sw.Close()
        End If
    End Sub
    Function GetIP() As String
        Dim hostName As String = My.Computer.Name
        Dim localIp As String = ""
        For Each address As System.Net.IPAddress In System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName).AddressList
            If address.AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                localIp = address.ToString()
                Exit For
            End If
        Next
        Return localIp
    End Function


    Private Sub chat_bt_Click(sender As Object, e As EventArgs) Handles chat_bt.Click
        Chat.Show()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim pass As String
        user = TextBox1.Text
        pass = TextBox2.Text
        port = TextBox3.Text
        'LOGIN
        If user Is "" Or pass Is "" Or port Is "" Then
            MessageBox.Show("Username,Password or Port is empty")
            Return
        End If

        ip = GetIP()

        'connecting to a server

        client = New Net.Sockets.TcpClient

        Try
            client.Connect(ips, Convert.ToInt32(ports)) ' Change the IP to your liking.

        Catch

            MessageBox.Show("Failed to connect: " & e.ToString, "Error")

        End Try
        ns = client.GetStream
        sr = New IO.StreamReader(ns, Encoding.ASCII, True) ' this sends our commands to the server
        sw = New IO.StreamWriter(ns)  ' and this will read server's response
        sw.WriteLine("USER:" + user)
        sw.WriteLine("PASS:" + pass)
        sw.WriteLine("IP:" + ip)
        sw.WriteLine("PORT:" + port)
        sw.Flush()
        Dim line As String
        line = sr.ReadLine()
        'Console.WriteLine(line)

        If line.Equals("200 SUCCESS") Then
            TextBox4.AppendText("Friend List" & Environment.NewLine)
            MessageBox.Show("Login Success")
            GroupBox1.Enabled = False
            GroupBox2.Enabled = True
            online = True
            Me.Height = 770
            '-----------------------------------------------------------------------------------------------
            'ReadData
            Dim ck As Boolean = True
            While ck
                line = sr.ReadLine()
                TextBox4.AppendText(line & Environment.NewLine)
                If line.Equals("END") Then
                    ck = False
                End If
            End While
            '-----------------------------------------------------------------------------------------------
            'ReadData
            trd = New Thread(AddressOf heartbeartTask)
            trd.IsBackground = True
            trd.Start()
            trdd = New Thread(AddressOf heartTask)
            trdd.IsBackground = True
            trdd.Start()
            Mainchat.Show()
        Else
            MessageBox.Show("Login Failed")
            Return
        End If

    End Sub
    Private Sub heartbeartTask()
        While True
            If Not online Then
                Return
            End If
            sw.WriteLine("Hello Server")
            sw.Flush()
            Console.WriteLine("RUN")
            Thread.Sleep(sec * 1000)
        End While
    End Sub
    Private Sub heartTask()
        While True
            If Not online Then
                Return
            End If
            Console.WriteLine(sr.ReadLine())
            Thread.Sleep(sec * 1000)
        End While
    End Sub
End Class
