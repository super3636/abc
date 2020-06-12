using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    class clientProgram
    {
        public string name = "";
        public delegate void Show();
        public Show show = null;
        public delegate void Close();
        public Close close=null;
        public delegate void SetDataControl(string data);
        public SetDataControl SetDataFunction = null;
        public delegate void SetSocketData(Client cl);
        public SetSocketData SetSocketFunction = null;
        public delegate void RemoveSocketData(Client cl);
        public RemoveSocketData RemoveSocketFunction = null;
        Socket clientSocket = null;
        IPEndPoint serverEP = null;
        public static bool loop = true;
        byte[] buff = null;
        int byteReceive = 0;
        

       
        public void setName(string name)
        {
            this.name = name;
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
                MessageBox.Show("Server chưa mở kết nối");
                return;
            }
            //Thread t1 = new Thread(CheckConnect);
            //t1.IsBackground = true;
            //t1.Start();
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
            receiveThread.IsBackground = true;
            receiveThread.Start(s);
 
        }
        public void ReceiveThread(Socket s)
        {
            while (loop)
            {
                try
                {
                    byte[] buff2 = new byte[1024];
                    int Receivebyte = s.Receive(buff2);
                    if (Receivebyte > 0)
                    {
                        string message2 = Encoding.UTF8.GetString(buff2);
                        string[] message3 = message2.Split(' ');
                        int n = message3.Count();                      
                        if (message3[0]=="Connected:")
                        {
                            string ten = message3[1];
                            string[] message4=message3[n-1].Split(':');                     
                            string IP=message4[0].Substring(1,message4[0].Length-1);
                            string[] message5 = message4[1].Split(')');
                            int Port=Convert.ToInt32(message5[0]);
                            IPEndPoint IPEP = new IPEndPoint(IPAddress.Parse(IP),Port);
                            Client cl = new Client(ten, IPEP);
                            SetSocketFunction(cl);
                        }
                        else if (message3[0]=="Disconnected:")
                        {
                            string ten = message3[1];
                            string[] message4 = message3[n - 1].Split(':');
                            string IP = message4[0].Substring(1, message4[0].Length - 1);
                            string[] message5 = message4[1].Split(')');
                            int Port = Convert.ToInt32(message5[0]);
                            IPEndPoint IPEP = new IPEndPoint(IPAddress.Parse(IP), Port);
                            Client cl = new Client(ten, IPEP);
                            RemoveSocketFunction(cl);
                        }

                        else if (message2.Substring(0, 14) == "Closing Server")
                        {
                            SetDataFunction("Server ngắt kết nối");
                            clientSocket.Close();
                            break;
                        }
                        else
                        {
                            SetDataFunction(message2);
                        }
                    }
                }
                catch
                {
                    SetDataFunction("Server đóng kết nối");
                    return;
                }
            }
        }
        //public void exit()
        //{
        //    clientSocket.Close();     
        //}
        public void SendMessage(string message)
        {
            try
            {
                buff = new byte[1024];
                buff = Encoding.UTF8.GetBytes(message);
                string[] ms = message.Split(' ');
                if (ms[0] == "(Private)")
                {
                    SetDataFunction("(Private) " + name + " gửi đến " + ms[ms.Count() - 2] + " :");
                    SetDataFunction(message.Substring(10, message.Length - ms[ms.Count() - 1].Length - ms[ms.Count() - 2].Length - 11));
                }
                else
                {
                    SetDataFunction(name + " :");
                    SetDataFunction(message);
                }
                clientSocket.Send(buff, 0, buff.Length, SocketFlags.None);
            }
            catch
            {
                SetDataFunction("Server đã đóng kết nối");
                return;
            }
        }

        //private void SendCallBack(IAsyncResult ar)
        //{
        //    Socket s = (Socket)ar.AsyncState;
        //    s.EndSend(ar);
        //    buff = new byte[1024];
        //    s.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), s);
        //}

        //private void ReceiveCallBack(IAsyncResult ar)
        //{
        //    Socket s = (Socket)ar.AsyncState;
        //    byteReceive = s.EndReceive(ar);
        //    string message = Encoding.ASCII.GetString(buff, 0, byteReceive);
        //    SetDataFunction(message);
        //}
        //public bool IsConnected(Socket socket)
        //{
        //        return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);   
        //}
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
        //public static bool IsConnected(Socket socket)
        //{
        //    try
        //    {
        //        return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
        //    }
        //    catch (SocketException) { return false; }
        //}
        //public void CheckConnect()
        //{
        //    while (true)
        //    {
        //        if (!IsConnected(clientSocket))
        //        {
        //            close();
        //            MessageBox.Show("Server đã đóng kết nối");
        //            Form1 f = new Form1();
        //            f.Show();
        //            break;
        //        }
        //    }
        //}


    }
}
