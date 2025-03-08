using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio3
{
    internal class Run
    {
        static void Main(string[] args)
        {
            ServerChatroom server = new ServerChatroom();
            server.IniciarServidor();
        }
    }
}
