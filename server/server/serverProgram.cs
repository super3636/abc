using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace server
{
    class serverProgram
    {
        public delegate void SetSocketName(string name);
        public SetSocketName SetSocketFunction = null;
        public static List<client> ListClient = new List<client>();
        public delegate void SetDataControl(string data);
        public SetDataControl SetDataFunction = null;
        public delegate void RemoveSocket(string data);
        public RemoveSocket RemoveSocketFunction = null;
        byte[] buff = new byte[1024];
        int byteReceive = 0;
        public static Socket serverSocket = null;
        public Socket clientSocket = null;
        public static bool Loop = true;
      
        public void Listen()
        {
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(serverEP);
            SetDataFunction("Đang chờ kết nối");
            serverSocket.Listen(-1);
            Thread t1 = new Thread(Accept2);
            t1.IsBackground = true;
            t1.Start();
            Thread t2 = new Thread(CheckDisconect);
            
            t2.Start();
           
        }
     
        public void Accept(Socket serverSocket)
        {
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), serverSocket);
        }
        public void AcceptCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            Socket Client = s.EndAccept(ar);
            Thread t1 = new Thread(Accept2);
            t1.IsBackground = true;
            t1.Start();
            //ListClient.Add(Client);
            //Thread t1 = new Thread((obj) =>
            //{
            //    Accept((Socket)obj);
            //});
            //t1.IsBackground = true;
            //t1.Start(s);
            
            //Thread t2 = new Thread((obj) =>
            //{
            //    DoWork((Socket)obj);
            //});
            //t2.IsBackground = true;
            //t2.Start(Client);
            //Thread t3 = new Thread(CheckDisconect);
            //t3.IsBackground = true;
            //t3.Start();
            CheckDisconect();

        }
        public void Accept2()
        {
            while(Loop)
            {
                clientSocket =serverSocket.Accept();
                Thread t2 = new Thread((obj) =>
                {
                    DoWork((Socket)obj);
                });
                t2.IsBackground = true;
                t2.Start(clientSocket);
            }
        }
        public void CheckDisconect()
        {
            while(Loop)
            {
                if (ListClient.Count > 0)
                {
                    foreach (client cl in ListClient.ToList())
                    {
                        if (!IsConnected(cl.clientSocket))
                        {
                            string outMessage = cl.name + " đã thoát khỏi phòng";
                            SetDataFunction(outMessage);
                            byte[] buff2 = new byte[1024];
                            buff2 = Encoding.UTF8.GetBytes(outMessage);
                            //try
                            //{
                                string socketInfo = cl.name + " ("+cl.clientSocket.RemoteEndPoint.ToString()+")";
                                RemoveSocketFunction(socketInfo);
                            //}
                            //catch { MessageBox.Show("asd"); }
                            ListClient.Remove(cl);
                            foreach (client cl2 in ListClient.ToList())
                            {
                                cl2.clientSocket.Send(buff2);
                            }                  
                        }
                    }
                }
            
            }
        }
        public static bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
        public void DoWork(Socket client)
        {
            buff = new byte[1024];
            client.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(nameCallBack), client);
        }
        public void nameCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            byteReceive = s.EndReceive(ar);
            string name = Encoding.UTF8.GetString(buff, 0, byteReceive);
            client cls = new client(name, s);
            ListClient.Add(cls);
            SetSocketFunction(cls.name+" ("+cls.clientSocket.RemoteEndPoint.ToString()+")");            
            buff = new byte[1024];
            string name2 = name+ " đã vào phòng";
            SetDataFunction(name2);
            buff = Encoding.UTF8.GetBytes(name2);
            foreach (client cl in ListClient)
            { 
                    cl.clientSocket.Send(buff, 0, buff.Length, SocketFlags.None);
            }
            //buff = new byte[1024];
            //s.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), s);
            while(Loop)
            {
                try
                {
                    byte[] buff3 = new byte[1024];
                    int byteReceive2 = s.Receive(buff3);
                    if (byteReceive2 > 0)
                    {
                        client cl = CheckSocket(s);
                        byte[] buff4 = new byte[1024];
                        buff4 = Encoding.ASCII.GetBytes(cl.name + " :");
                        SetDataFunction(name + " :");
                        string message = Encoding.UTF8.GetString(buff3, 0, byteReceive2);
                        SetDataFunction(message);
                        foreach (client cl2 in ListClient)
                        {
                            if (cl2 != cl)
                            {
                                cl2.clientSocket.Send(buff4);
                                cl2.clientSocket.Send(buff3);
                            }
                        }
                    }
                }
                catch
                {
                    foreach (client cl3 in ListClient)
                    {
                        if (cl3.clientSocket == s)
                        {
                            ListClient.Remove(cl3);
                            break;
                        }
                    }
                    SetDataFunction("Server đóng kết nối");
                    s.Close();
                }
            }
            SetDataFunction("Server đóng kết nôi1");
            foreach(client cl4 in ListClient)
            {
                cl4.clientSocket.Close();
            }
          
            ListClient.Clear();
            serverSocket.Close();
        }
        public client CheckSocket(Socket s)
        {
            foreach(client cl in ListClient)
            {
                if(cl.clientSocket==s)
                {
                    return cl;
                }
            }
            return null;
        }


        private void ReceiveCallBack(IAsyncResult ar)
        {
            Socket s = (Socket)ar.AsyncState;
            byteReceive = s.EndReceive(ar);
            string message = Encoding.ASCII.GetString(buff,0,byteReceive);
            SetDataFunction(message);
            foreach(client cl in ListClient)
            {
                cl.clientSocket.Send(buff);
            }
        }
        
        //public void SendNameCallBack(IAsyncResult ar)
        //{
        //    Socket s = (Socket)ar.AsyncState;
        //    foreach (Socket client in ListClient)
        //    {
        //        client.EndSend(ar);
        //    }          
        //}
    }
}
