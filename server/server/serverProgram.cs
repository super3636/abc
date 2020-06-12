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
        public List<string> ListMessage = new List<string>();
       byte[] buff = new byte[1024];
        int byteReceive = 0;
        public static Socket serverSocket = null;
        public Socket clientSocket = null;
        public static bool Loop = true;
        Thread t1,t2,t3;
        public void Listen()
        {
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(serverEP);
            SetDataFunction("Đang chờ kết nối");
            serverSocket.Listen(-1);
            t1 = new Thread(Accept2);
            t1.IsBackground = true;
            t1.Start();
            t2 = new Thread(CheckDisconect);
            t2.IsBackground = true;
            t2.Start();
        }
        
        //public void Accept(Socket serverSocket)
        //{
        //    serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), serverSocket);
        //}
        //public void AcceptCallBack(IAsyncResult ar)
        //{
        //    Socket s = (Socket)ar.AsyncState;
        //    Socket Client = s.EndAccept(ar);
        //    Thread t1 = new Thread(Accept2);
        //    t1.IsBackground = true;
        //    t1.Start();
        //    //ListClient.Add(Client);
        //    //Thread t1 = new Thread((obj) =>
        //    //{
        //    //    Accept((Socket)obj);
        //    //});
        //    //t1.IsBackground = true;
        //    //t1.Start(s);
            
        //    //Thread t2 = new Thread((obj) =>
        //    //{
        //    //    DoWork((Socket)obj);
        //    //});
        //    //t2.IsBackground = true;
        //    //t2.Start(Client);
        //    //Thread t3 = new Thread(CheckDisconect);
        //    //t3.IsBackground = true;
        //    //t3.Start();
        //    CheckDisconect();

        //}
        public void Accept2()
        {
            while(Loop)
            {
                try
                {
                    clientSocket = serverSocket.Accept();
                    t3 = new Thread((obj) =>
                    {
                        DoWork((Socket)obj);
                    });
                    t3.IsBackground = true;
                    t3.Start(clientSocket);
                 
                }
                catch
                {
                    break;
                    
                }
            }
        }
      
        public void CheckDisconect()
        {
            while(Loop)
            {
                if (ListClient.Count > 0)
                {
                    try
                    {
                        foreach (client cl in ListClient)
                        {
                            if (!IsConnected(cl.clientSocket))
                            {
                                string outMessage2 = "Disconnected: " + cl.name + " (" + cl.clientSocket.RemoteEndPoint.ToString()+")";
                                string outMessage = cl.name + " đã thoát khỏi phòng";
                                SetDataFunction(outMessage);
                                byte[] buff2 = new byte[1024];
                                buff2 = Encoding.UTF8.GetBytes(outMessage);
                                byte[] buff3 = new byte[1024];
                                buff3 = Encoding.UTF8.GetBytes(outMessage2);
                                try
                                {
                                    string socketInfo = cl.name + " (" + cl.clientSocket.RemoteEndPoint.ToString() + ")";
                                    RemoveSocketFunction(socketInfo);
                                }
                                catch { MessageBox.Show("asd"); }
                                ListClient.Remove(cl);

                                foreach (client cl2 in ListClient.ToList())
                                {
                                    cl2.clientSocket.Send(buff2);     
                                }
                                foreach (client cl2 in ListClient.ToList())
                                {
                                    cl2.clientSocket.Send(buff3);

                                }
                            }
                        }
                    }
                    catch { }
            
                }
               
            }
        }
        public static bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch { return false; }
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
            byte[]buff7 = new byte[1024];
            buff7 = Encoding.UTF8.GetBytes("Connected: "+cls.name + " (" + cls.clientSocket.RemoteEndPoint.ToString() + ")");    
            foreach (client cl in ListClient)
            {
                cl.clientSocket.Send(buff);
      
            }
            Thread.Sleep(500);
            foreach (client cl in ListClient)
            {       
                cl.clientSocket.Send(buff7);
            }
            byte[] buff8 = new byte[1024];
            foreach (client cl3 in ListClient)
            {
                if (cl3 != cls)
                {       
                    buff8 = new byte[1024];
                    buff8 = Encoding.UTF8.GetBytes("Connected: "+cl3.name + " (" + cl3.clientSocket.RemoteEndPoint.ToString() + ")");
                    cls.clientSocket.Send(buff8);
                    Thread.Sleep(100);
                }
            }
            Thread.Sleep(500);
            foreach(string str in ListMessage)
            {
                byte[] buff9 = new byte[1024];
                buff9 = Encoding.UTF8.GetBytes(str);
                cls.clientSocket.Send(buff9);
                Thread.Sleep(100);
            }
            while(Loop)
            {
                try
                {
                    byte[] buff3 = new byte[1024];
                    int byteReceive2 = s.Receive(buff3);
                    if (byteReceive2 > 0)
                    {
                        string message4 = Encoding.UTF8.GetString(buff3,0,byteReceive2);
                        string[] ms = message4.Split(' ');
                        if (ms[0]!="(Private)")
                        {
                            client cl = CheckSocket(s);
                            byte[] buff4 = new byte[1024];
                            ListMessage.Add(cl.name + " :");
                            buff4 = Encoding.UTF8.GetBytes(cl.name + " :");
                            SetDataFunction(name + " :");
                            string message = Encoding.UTF8.GetString(buff3, 0, byteReceive2);
                            ListMessage.Add(message);
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
                        else
                        {                      
                            int n = ms.Count();
                            string IPEP = ms[n-1];
                            client cl5 = new client();
                            foreach(client cl in ListClient)
                            {
                                if (cl.clientSocket.RemoteEndPoint.ToString() == IPEP)
                                {
                                    cl5 = cl;
                                    break;
                                }
                            }
                            client cl6 = CheckSocket(s);
                            SetDataFunction("(Private) "+cl6.name + " gửi tới " + cl5.name+" :");
                            byte[] buff4 = new byte[1024];
                            buff4 = Encoding.UTF8.GetBytes("(Private) "+cl6.name +  " :");
                           
                            string message5 = "";
                            for(int i = 1;i<n-2;i++)
                            {
                                message5 += ms[i]+" ";
                            }
                            SetDataFunction(message5);
                            byte[] buff5 = Encoding.UTF8.GetBytes(message5);
                            cl5.clientSocket.Send(buff4);
                            cl5.clientSocket.Send(buff5);
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
                    s.Close();
                    break;
                }
            }
          
            foreach(client cl4 in ListClient)
            {
                RemoveSocketFunction(cl4.name + " (" + cl4.clientSocket.RemoteEndPoint.ToString() + ")");
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


        //private void ReceiveCallBack(IAsyncResult ar)
        //{
        //    Socket s = (Socket)ar.AsyncState;
        //    byteReceive = s.EndReceive(ar);
        //    string message = Encoding.ASCII.GetString(buff,0,byteReceive);
        //    SetDataFunction(message);
        //    foreach(client cl in ListClient)
        //    {
        //        cl.clientSocket.Send(buff);
        //    }
        //}
        public void Disconect()
        {

            foreach(client cl in ListClient)
            {
                SetDataFunction(cl.name + " đã rời khỏi phòng");
                string outMessage = "Closing Server";
                byte[] buff7 = new byte[1024];
                buff7 = Encoding.UTF8.GetBytes(outMessage);
                cl.clientSocket.Send(buff7);  
            }
             foreach(client cl in ListClient)
            {
            string socketInfo = cl.name + " (" + cl.clientSocket.RemoteEndPoint.ToString() + ")";
            RemoveSocketFunction(socketInfo);
            }
            //t3.Abort();
            //t2.Abort();
            //t1.Abort();
            ListClient.Clear();
            serverSocket.Close();
        }
        
        //public void SendNameCallBack(IAsyncResult ar)
        //{
        //    Socket s = (Socket)ar.AsyncState;
        //    foreach (Socket client in ListClient)
        //    {
        //        client.EndSend(ar);
        //    }          
        //}
        public void outClient(string clientInfo)
        {
            string[] clientInfo2 = clientInfo.Split(' ');
            string[] clientInfo3 = clientInfo2[1].Split('(');
            string[] clientInfo4 = clientInfo3[1].Split(')');
            string name = clientInfo2[0];
            string IPEP = clientInfo4[0];
            client cls = null;
            foreach(client cl in ListClient)
            {
                if(cl.name==name&& cl.clientSocket.RemoteEndPoint.ToString()==IPEP)
                {
                    cls = cl;
                    break;
                }
            }
            RemoveSocketFunction(cls.name + " (" + cls.clientSocket.RemoteEndPoint.ToString() + ")");
            cls.clientSocket.Close();
            ListClient.Remove(cls);
        }
    }
}
