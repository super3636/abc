using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace client
{
    public class Client
    {
        public string name;
        public IPEndPoint clIP;
        public Client(string name,IPEndPoint IP)
        {
            this.name = name;
            this.clIP = IP;
        }
    }
}
