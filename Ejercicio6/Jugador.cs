using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio6
{
    internal class Jugador
    {
        public int numero;
        public IPEndPoint ip;
        public Socket socket;
        public Jugador(int numero, IPEndPoint ip, Socket socket)
        {
            this.numero = numero;
            this.ip = ip;
            this.socket = socket;
        }
    }
}
