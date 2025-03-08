using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace ServicioFecha
{
    internal class ServerFecha
    {
        private bool on = true;
        public Socket socket;
        public int port;
        private int segundoPuerto = 31415;
        private IPEndPoint ipEnd;
        public bool pausa = false;
        public object l = new object();
        public void InitServer()
        {
            ipEnd = new IPEndPoint(IPAddress.Any, port);
            using (socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Bind(ipEnd);
                    socket.Listen(10);
                    Console.WriteLine("Servidor alojado en 127.0.0.1 {0}", port);
                    while (on)
                    {
                        Socket cliente = socket.Accept();
                        IPEndPoint ieClient = (IPEndPoint)cliente.RemoteEndPoint;
                        Thread hilo = new Thread(TimeFunction);
                        hilo.IsBackground = true;
                        hilo.Start(cliente);
                    }
                }
                catch (SocketException e) when (e.ErrorCode == ((int)SocketError.AddressAlreadyInUse))
                {
                    Console.WriteLine("Puerto {0} en uso", port);
                    if (port != segundoPuerto)
                    {
                        port = segundoPuerto;
                        InitServer();
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
            }
        }

        public void LeerConfig()
        {
            string ruta = $"{Environment.GetEnvironmentVariable("PROGRAMDATA")}\\fecha_config.txt";
            if (File.Exists(ruta))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(ruta))
                    {
                        string leido = sr.ReadToEnd();
                        if (int.TryParse(leido, out int puertoValido) && short.TryParse(leido, out short temp))
                        {
                            port = puertoValido;
                        }
                        else
                        {
                            port = segundoPuerto;
                        }

                    }
                }
                catch (IOException)
                {
                    Console.WriteLine("Ha habido un error de lectura, se revierte al puerto por defecto");
                    port = segundoPuerto;
                }
            } else
            {
                port = segundoPuerto;
            }
        }

        public void TimeFunction(object socket)
        {
            Socket socketCliente = (Socket)socket;
            IPEndPoint iEnd = (IPEndPoint)socketCliente.RemoteEndPoint;
            Console.WriteLine("Nuevo Cliente : {0}:{1}", iEnd.Address, iEnd.Port);
            string comando;
            using (NetworkStream ns = new NetworkStream(socketCliente))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                try
                {
                    //sw.Write("Introduce tu comando:");
                    //sw.Flush();
                    comando = sr.ReadLine();

                    if (comando != null)
                    {
                        switch (comando)
                        {
                            case "time":
                                sw.Write("Hora: {0:D2}:{1:D2}:{2:D2}", System.DateTime.Now.Hour, System.DateTime.Now.Minute, System.DateTime.Now.Second);
                                sw.Flush();
                                break;
                            case "date":
                                sw.Write("Fecha: {0}/{1:D2}/{2:D2}", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
                                sw.Flush();
                                break;
                            case "all":
                                sw.Write("Fecha y hora: {0}", System.DateTime.Now.ToString());
                                sw.Flush();
                                break;
                            default:
                                if (Regex.IsMatch(comando, @"close .+$"))
                                {
                                    try
                                    {
                                        using (StreamReader srLocal = new StreamReader(Path.Combine(Environment.GetEnvironmentVariable("PROGRAMDATA"), "password.txt").Trim()))
                                        {
                                            string[] parts = comando.Split(' ');
                                            string password = srLocal.ReadToEnd();
                                            on = parts[1] != password;
                                            sw.WriteLine(on ? "Contraseña Incorrecta" : "El servidor se apagará ahora");
                                            sw.Flush();
                                        }
                                    }
                                    catch (FileNotFoundException)
                                    {
                                        sw.WriteLine("Error: No se ha encontrado el archivo de la contraseña");
                                    }
                                }
                                else
                                {
                                    sw.Write("Comando no reconocido");
                                    sw.Flush();
                                }
                                break;
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("Error de IO: " + e.Message);
                }
                Console.WriteLine("Finished connection with {0}:{1}", iEnd.Address, iEnd.Port);
            }
            socketCliente.Close();
            if (!on)
            {
                this.socket.Close();
            }
        }
    }
}
