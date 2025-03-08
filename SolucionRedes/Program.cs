using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
namespace Networking
{
    class Program
    {
        static void ShowNetInformation(string name)
        {
            IPHostEntry hostInfo;
            //Trata de resolver el DNS
            hostInfo = Dns.GetHostEntry(name);
            // Muestra el nombre del equipo
            Console.WriteLine("Name: {0}", hostInfo.HostName);
            // Lista de IPs del equipo
            Console.WriteLine("IP list: ");
            foreach (IPAddress ip in hostInfo.AddressList)
            {
                //Para ver sólo las direcciones IPv4 se compara con
                //AddressFamily.InterNetwork
                //Para IPv6 se usaría AddressFamily.InterNetworkV6
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine("\t{0,16}", ip);
                }
            }
            Console.WriteLine("\n");
        }
        static void Main(string[] args)
        {
            //// Obtenemos el nombre del equipo local y lo mostramos
            //String localHost = Dns.GetHostName();
            //Console.WriteLine("Localhost name: {0} \n", localHost);
            ////Mostramos información del equipo local y de uno remoto
            //ShowNetInformation(localHost);
            //ShowNetInformation("www.google.es");
            //Console.ReadKey();

            //IPAddress ip = IPAddress.Loopback;
            //IPEndPoint ie = new IPEndPoint(ip, 1200);
            //Console.WriteLine("IPEndPoint: {0}", ie.ToString());
            //Console.WriteLine("AddressFamily: {0}", ie.AddressFamily);
            //Console.WriteLine("Address: {0}, Puerto: {1}", ie.Address, ie.Port);
            //Console.WriteLine("Ports range: {0}-{1}", IPEndPoint.MinPort,
            //IPEndPoint.MaxPort);
            //ie.Port = 80;
            //ie.Address = IPAddress.Parse("80.1.12.128");
            //Console.WriteLine("New End Point: {0}", ie.ToString());
            //Console.ReadKey();

            //int port = 80;
            //IPEndPoint ie = new IPEndPoint(IPAddress.Any, port);
            ////Creacion del Socket
            //using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            //{
            //    try
            //    {
            //        //Enlace de socket al puerto (y en cualquier interfaz de red)
            //        //Salta excepción si el puerto está ocupado
            //        s.Bind(ie);
            //        Console.WriteLine($"Port {port} free");
            //    }
            //    catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
            //    {
            //        // ó 10048
            //        Console.WriteLine($"Port {port} in use");
            //    }
            //}

            //IPEndPoint ie = new IPEndPoint(IPAddress.Any, 31416);
            ////Creacion del Socket
            //Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
            //ProtocolType.Tcp);
            ////Enlace de socket al puerto (y en cualquier interfaz de red)
            ////Salta excepción si el puerto está ocupado
            //s.Bind(ie);
            ////Esperando una conexión y estableciendo cola de clientes pendientes
            //s.Listen(10);
            ////Esperamos y aceptamos la conexion del cliente (socket bloqueante)
            //Socket sClient = s.Accept();
            ////Obtenemos la info del cliente. El casting es necesario ya que
            ////RemoteEndPoint es del tipo EndPoint mas genérico
            //IPEndPoint ieClient = (IPEndPoint)sClient.RemoteEndPoint;
            //Console.WriteLine("Client connected:{0} at port {1}", ieClient.Address,
            //ieClient.Port);
            //sClient.Close(); // Se puede usar using y nos ahorramos los close.
            //s.Close();
            //Console.ReadLine();
        }
    }
}