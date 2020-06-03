using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class client
    {
        public string name;
        public Socket clientSocket;
        public client(string name,Socket ClientSocket)
        {
            this.name = name;
           this.clientSocket= ClientSocket;
        }
        public client()
        {

        }

    }
}
