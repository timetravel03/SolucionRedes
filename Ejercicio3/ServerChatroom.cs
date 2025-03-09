using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio3
{
    internal class ServerChatroom//Revisar locks y desconexión abrupta el aviso a otros (flush)
    {
        private Socket socket;
        private int port;
        private int segundoPuerto;
        private IPEndPoint ipEnd;
        private List<StreamWriter> writers;
        private List<Thread> hilosClientes;
        private List<string> listaUsuarios;
        private Socket cliente;
        private object l = new object();
        public ServerChatroom()
        {
            port = 31416;
            segundoPuerto = 31415;
            writers = new List<StreamWriter>();
            hilosClientes = new List<Thread>();
            listaUsuarios = new List<string>();
        }

        public void IniciarServidor()
        {
            ipEnd = new IPEndPoint(IPAddress.Any, port);
            using (socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Bind(ipEnd);
                    socket.Listen(10);
                    Console.WriteLine("Servidor alojado en 127.0.0.1 {0}", port);
                    while (true)
                    {
                        cliente = socket.Accept();
                        IPEndPoint ieClient = (IPEndPoint)cliente.RemoteEndPoint;
                        Thread hilo = new Thread(FuncionChat);
                        hilo.IsBackground = true;
                        hilo.Start(cliente);
                        hilosClientes.Add(hilo);
                    }
                }
                catch (SocketException e) when (e.ErrorCode == ((int)SocketError.AddressAlreadyInUse))
                {
                    Console.WriteLine("Puerto {0} en uso", port);
                    if (port != segundoPuerto)
                    {
                        port = segundoPuerto;
                        IniciarServidor();
                    }
                    else
                    {
                        Console.WriteLine("No hay puertos disponibles, el servidor se apagará ahora");
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("El servidor se apagará ahora");
                }
                finally
                {
                    lock (l)
                    {
                        foreach (StreamWriter sw in writers)
                        {
                            sw.Close();
                        }
                    }
                }
            }
        }

        public void FuncionChat(object socket)
        {
            string mensaje = "";
            string nombreUsuario = "";
            using (Socket socketCliente = (Socket)socket)
            using (NetworkStream ns = new NetworkStream(socketCliente))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw_server_m = new StreamWriter(ns))
            {
                bool salida = false;
                IPEndPoint iEnd = (IPEndPoint)socketCliente.RemoteEndPoint;
                Console.WriteLine("Nuevo Cliente : {0}:{1}", iEnd.Address, iEnd.Port);

                //programa
                try
                {
                    nombreUsuario = ElegirNombre(sw_server_m, sr) + "@" + iEnd.Address;
                    lock (l)
                    {
                        writers.Add(sw_server_m);
                        listaUsuarios.Add(nombreUsuario);
                        foreach (StreamWriter sw in writers) //meter en lock
                        {
                            sw.WriteLine(nombreUsuario + " se ha conectado");
                            sw.Flush();
                        }
                    }
                    do
                    {
                        mensaje = sr.ReadLine();
                        if (mensaje != null)
                        {
                            if (mensaje == "#lista")
                            {
                                sw_server_m.WriteLine("Lista de usuarios conectados:");
                                foreach (string nombre in listaUsuarios)
                                {
                                    sw_server_m.WriteLine(nombre);
                                    sw_server_m.Flush();
                                }
                            }
                            else if (mensaje == "#exit")
                            {
                                salida = true;
                            }
                            else
                            {
                                foreach (StreamWriter sw in writers)
                                {
                                    sw.WriteLine(nombreUsuario + ": " + mensaje);
                                    sw.Flush();
                                }
                            }
                        } else
                        {
                            salida = true;
                        }
                    }
                    while (!salida);
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error de IO: " + e.Message);
                }

                //gestion de salida
                lock (l)
                {
                    if (listaUsuarios.Contains(nombreUsuario))
                    {
                        listaUsuarios.Remove(nombreUsuario);
                    }
                    if (writers.Contains(sw_server_m))
                    {
                        writers.Remove(sw_server_m);
                    }
                }
                Console.WriteLine("Finished connection with {0}:{1}", iEnd.Address, iEnd.Port);

                try
                {
                    lock (l)
                    {
                        foreach (StreamWriter sw in writers) // PELIGRO añadir try catch y lock
                        {
                            sw.WriteLine(nombreUsuario + " se ha ido");
                            sw.Flush();
                        }
                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Segunda ex");
                }
            }
        }

        public string ElegirNombre(StreamWriter sw, StreamReader sr)
        {
            string nombreUsuario = null;
            while (nombreUsuario == null)
            {
                sw.WriteLine("Elige un nombre de usuario:");
                sw.Flush();
                nombreUsuario = sr.ReadLine();
            }
            return nombreUsuario;
        }
    }
}
