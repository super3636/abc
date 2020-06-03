using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace client
{
    class clientProgram
    {
        public string name = "";
        public delegate void SetDataControl(string data);
        public SetDataControl SetDataFunction = null;
        Socket clientSocket = null;
        IPEndPoint serverEP = null;
        public static bool loop = true;
        byte[] buff = null;
        int byteReceive = 0;
       
        public void setName(string name)
        {
            this.name = name+":";
        }
        public void Connect(string serverIP, int port)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverEP = new IPEndPoint(IPAddress.Parse(serverIP), port);
            clientSocket.BeginConnect(serverEP, new AsyncCallback(ConnectCallBack), clientSocket);
        }
      
        public void ConnectCallBack(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            try
            {  
                client.EndConnect(ar);  
            }
            catch
            {
                SetDataFunction("Kết nối thất bại");
                return;
            }
        }
        public void SendName(string name)
        {
            buff = new Byte[1024];
            buff = Encoding.UTF8.GetBytes(name);
            clientSocket.BeginSend(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(SendNameCallBack), clientSocket);
        }
        public void SendNameCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            s.EndSend(ar);
            buff = new byte[1024];
            s.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(ReceiveNameCallBack), s);
        }
        public void ReceiveNameCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            byteReceive = s.EndReceive(ar);
            string message = Encoding.UTF8.GetString(buff, 0, byteReceive);
            SetDataFunction(message);
            //while(true)
            //{
            //    buff = new byte[1024];
            //    s.Receive(buff, 0, buff.Length, SocketFlags.None);
            //    string message2 = Encoding.ASCII.GetString(buff);
            //    SetDataFunction(message2);
            //}
            Thread receiveThread = new Thread((obj) =>
            {
                ReceiveThread((Socket)obj);
            });
            receiveThread.Start(s);
 
        }
        public void ReceiveThread(Socket s)
        {
            while (loop)
            {
                try
                {
                  
                    buff = new byte[1024];
                    s.Receive(buff, 0, buff.Length, SocketFlags.None);
                    string message2 = Encoding.UTF8.GetString(buff);
                    SetDataFunction(message2);
                }
                catch
                {
                    SetDataFunction("Server đóng kết nối");
                    return;
                }
                
            }
        }
        public void exit()
        {
            clientSocket.Close();     
        }
        public void SendMessage(string message)
        {
            buff = new byte[1024];
            buff = Encoding.UTF8.GetBytes(message);
            clientSocket.Send(buff);
        }

        private void SendCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            s.EndSend(ar);
            buff = new byte[1024];
            s.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), s);
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            byteReceive = s.EndReceive(ar);
            string message = Encoding.ASCII.GetString(buff, 0, byteReceive);
            SetDataFunction(message);
        }
        public bool Disconect()
        {
            try
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
