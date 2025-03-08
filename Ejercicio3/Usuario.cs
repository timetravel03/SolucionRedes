using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio3
{
    public struct Usuario
    {
        public StreamWriter sw;
        public string nombre;
        public string ip;
        public IPEndPoint iPEnd;
    }
}
