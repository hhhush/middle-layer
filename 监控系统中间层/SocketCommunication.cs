using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace 监控系统中间层
{
    class SocketCommunication
    {
        private IPAddress IP;
        private IPEndPoint Point;
        private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Thread thReceive;
        public SocketCommunication(string ip, int port)
        {
            IP = IPAddress.Parse(ip);
            Point = new IPEndPoint(IP, port);
        }

        //建立连接
        public void Connect()
        {
            client.Connect(Point);
            Console.WriteLine("连接成功");
            Console.WriteLine("OPC UA服务器:" + client.RemoteEndPoint.ToString());
            Console.WriteLine("协议转换中间层:" + client.LocalEndPoint.ToString());
        }

        //接收OPC UA服务器消息线程
        public void ReceiveMessage()
        {
            thReceive = new Thread(ReceiveMsg);
            thReceive.IsBackground = true;
            thReceive.Start();
        }

        //接收OPC UA服务器消息
        private void ReceiveMsg()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024];
                    int n = client.Receive(buffer);
                    string s = Encoding.UTF8.GetString(buffer, 0, n);
                    Console.WriteLine(client.RemoteEndPoint.ToString() + ":" + s);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }

        //给OPC UA服务器发消息
        public void SendMessage(string message)
        {
            if (client != null)
            {
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    client.Send(buffer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
