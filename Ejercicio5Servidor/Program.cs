﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio5Servidor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServidorAhorcado server = new ServidorAhorcado();
            server.LoopServidor();
        }
    }
}
