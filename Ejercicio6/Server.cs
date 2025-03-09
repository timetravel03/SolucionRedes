using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Ejercicio6
{
    internal class Server
    {
        private Socket socketServer;
        private Socket socketCliente;
        private int clientesConectados;
        private IPEndPoint endPoint;
        private int contador;
        private System.Timers.Timer timer;
        private Random rn;

        // streamwriters para todos los clientes
        private List<StreamWriter> streamWriters;

        // todos los numeros que han recibido los clientes
        private List<int> numerosQueHanSalido;

        // numero mas alto que se ha generado
        private int numeroMasAlto;

        public Server()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += new ElapsedEventHandler(FuncTimer);
            endPoint = new IPEndPoint(IPAddress.Any, 31416);
            streamWriters = new List<StreamWriter>();
            numerosQueHanSalido = new List<int>();
            rn = new Random();
            contador = 20;
        }
        public void Init()
        {
            try
            {
                using (socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socketServer.Bind(endPoint);
                    socketServer.Listen(10);
                    timer.Start();
                    while (true)
                    {
                        while (contador > 0)
                        {
                            socketCliente = socketServer.Accept();
                            int numero = rn.Next(1, 21);
                            numerosQueHanSalido.Add(numero);
                            numeroMasAlto = numerosQueHanSalido.Max();
                            Thread hilo = new Thread(() => FuncionCliente(socketCliente, numero));
                            hilo.IsBackground = true;
                            hilo.Start();
                            clientesConectados++;
                            Console.WriteLine($"Se ha conectado un cliente, hay {clientesConectados}");
                        }
                    }
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Server Off");
            }
        }

        private void FuncTimer(object sender, ElapsedEventArgs e)
        {
            contador--;
            lock (this)
            {
                int numeroDeWriters = streamWriters.Count;
                for (int i = numeroDeWriters - 1; i >= 0; i--)
                {
                    try
                    {
                        if (contador > 0)
                        {
                            streamWriters[i].WriteLine(contador);
                            streamWriters[i].Flush();
                        }
                        else
                        {
                            contador = 10;
                            streamWriters.Clear();
                        }
                    }
                    catch (IOException)
                    {
                        clientesConectados--;
                        Console.WriteLine($"Se ha desconectado un cliente, hay{clientesConectados}");
                        streamWriters.Remove(streamWriters[i]);
                        numeroDeWriters--;
                    }
                }
            }
            Console.WriteLine(contador);
        }

        private void FuncionCliente(Socket s, int numero)
        {
            try
            {
                using (Socket esteSocket = s)
                using (NetworkStream ns = new NetworkStream(esteSocket))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    streamWriters.Add(sw);
                    sw.WriteLine($"Se te ha asignado el numero {numero}");
                    sw.Flush();
                    sw.WriteLine($"El juego empezara en");
                    sw.Flush();
                    while (contador > 0)
                    { }
                    if (numeroMasAlto == numero)
                    {
                        sw.WriteLine("Enhorabuena, has ganado");
                    }
                    else
                    {
                        sw.WriteLine($"Has perdido, el numero más alto ha sido {numeroMasAlto}");
                    }
                }
            }
            catch (SocketException)
            {
                Console.WriteLine("Un cliente ha sufrido un SOCKETEXCEPTION");
            }
            catch (IOException)
            {
                Console.WriteLine("Un cliente ha sufrido un IOEXCEPTION");
            }
        }
    }
}
