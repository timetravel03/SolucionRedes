using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio4//Comprob. archivos. lock en readpin. Rangos en del.
{
    internal class ShiftServer
    {
        private string[] users;
        private List<string> waitQueue;
        private int puertoServidor;
        private Socket socketServidor;
        private IPEndPoint endPoint;
        private List<Thread> subprocesos;
        private object l = new object();
        private Socket socketCliente;
        private string userprofile;

        public ShiftServer()
        {
            puertoServidor = 31416;
            subprocesos = new List<Thread>();
            userprofile = Environment.GetEnvironmentVariable("USERPROFILE");
            waitQueue = new List<string>();
            if (!File.Exists($"{userprofile}\\pin.bin"))
            {
                CambiarPin($"{userprofile}\\pin.bin", 4545);
            }
        }



        public void Init()
        {
            try
            {
                using (socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    BuscarPuertos();
                    ReadNames($"{userprofile}\\usuarios.txt");
                    socketServidor.Listen(10);
                    Console.WriteLine($"Servidor escuchando en {endPoint.Address}:{endPoint.Port}");
                    CargarLista();
                    while (true)
                    {
                        socketCliente = socketServidor.Accept();
                        Thread hilo = new Thread(FuncionCliente);
                        hilo.IsBackground = true;
                        hilo.Start(socketCliente);
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                GuardarLista();
            }
        }

        private void FuncionCliente(object socket)
        {
            bool admin = false;
            try
            {
                using (Socket localSocket = (Socket)socket)
                using (NetworkStream ns = new NetworkStream(localSocket))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    IPEndPoint ieClient = (IPEndPoint)localSocket.RemoteEndPoint;

                    //sw.WriteLine("Bienvenido al servidor, indica tu nombre:");
                    //sw.Flush();

                    string input = sr.ReadLine();
                    string nombre = input;
                    bool contiene = false;
                    int cont = 0;
                    bool toClose = false;

                    // lock (l)
                    {
                        while (!contiene && cont < users.Length)
                        {
                            contiene = users[cont] == input;
                            cont++;
                        }
                    }

                    if (!(contiene || input == "admin"))
                    {
                        sw.WriteLine("Usuario desconocido");
                        sw.Flush();
                    }
                    else
                    {
                        if (input == "admin")
                        {
                            sw.WriteLine("Introduce el pin:");
                            sw.Flush();
                            string pin = sr.ReadLine();
                            int pinLeido = ReadPin($"{userprofile}\\pin.bin");

                            pinLeido = pinLeido == -1 ? 1234 : pinLeido;

                            if (pin != pinLeido.ToString())
                            {
                                toClose = true;
                            }
                            else
                            {
                                admin = true;
                            }
                        }
                        if (!toClose)
                        {
                            do
                            {
                                input = sr.ReadLine();
                                if (input != null)
                                {
                                    if (input.StartsWith("del ") && admin)
                                    {
                                        string[] posicion = input.Split(' ');
                                        lock (l)
                                        {
                                            if (int.TryParse(posicion[1], out int p) && waitQueue.Count > p && p >= 0) // valores menos de 0
                                            {

                                                waitQueue.RemoveAt(p);
                                            }
                                            else
                                            {
                                                sw.WriteLine("delete error");
                                                sw.Flush();
                                            }
                                        }
                                    }
                                    else if (input.StartsWith("chpin ") && admin)
                                    {
                                        string[] posicion = input.Split(' ');
                                        lock (l)
                                        {
                                            if (int.TryParse(posicion[1], out int p) && p.ToString().Length == 4)
                                            {
                                                CambiarPin($"{userprofile}\\pin.bin", p);
                                            }
                                            else
                                            {
                                                sw.WriteLine("error: pin inválido");
                                                sw.Flush();
                                            }
                                        }
                                    }
                                    else if (input == "exit" && admin)
                                    {
                                        admin = false;
                                    }
                                    else if (input == "shutdown" && admin)
                                    {
                                        socketServidor.Close();
                                    }
                                    else if (input == "list")
                                    {
                                        string lista = "";
                                        lock (l)
                                        {
                                            for (int i = 0; i < waitQueue.Count; i++)
                                            {
                                                lista += (waitQueue[i] + ';');
                                            }
                                        }
                                        sw.WriteLine(lista);
                                        sw.Flush();
                                    }
                                    else if (input == "add")
                                    {
                                        lock (l)
                                        {
                                            if (!waitQueue.Contains(nombre))
                                            {
                                                waitQueue.Add(nombre);
                                            }
                                            else
                                            {
                                                sw.WriteLine("Usuario ya en lista");
                                            }
                                        }
                                        sw.WriteLine($"{nombre} añadido");
                                    }
                                    else
                                    {
                                        sw.WriteLine("Comando desconocido");
                                        sw.Flush();
                                    }
                                }
                            } while (admin);
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.ErrorCode);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void GuardarLista()
        {
            try
            {
                lock (l)
                {
                    using (StreamWriter sw = new StreamWriter($"{userprofile}\\lista.txt"))
                    {
                        foreach (string nombre in waitQueue)
                        {
                            sw.WriteLine(nombre);
                        }
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error al guardar la lista");
            }
        }

        private void CargarLista()
        {
            string nombreLeido;
            try
            {
                lock (l)
                {
                    using (StreamReader sr = new StreamReader($"{userprofile}\\lista.txt"))
                    {
                        while ((nombreLeido = sr.ReadLine()) != null)
                        {
                            waitQueue.Add(nombreLeido);
                        }
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error al leer la lista");
            }
        }

        private void BuscarPuertos()
        {
            bool encontrado = false;
            while ((IPEndPoint.MaxPort >= puertoServidor) && !encontrado)
            {
                try
                {
                    endPoint = new IPEndPoint(IPAddress.Any, puertoServidor);
                    socketServidor.Bind(endPoint);
                    encontrado = true;
                    Console.WriteLine($"Puerto: {endPoint.Port}");
                }
                catch (SocketException)
                {
                    puertoServidor++;
                }
            }
            if (!encontrado)
            {
                socketServidor.Close();
            }
        }

        public void ReadNames(string ruta)
        {
            List<string> nombresLeidos = new List<string>();
            string nombreActual = "";
            int caracterLeido;
            char letraLeida;
            try // try catch por si hay algun error de lectura
            {
                if (File.Exists(ruta))
                {
                    using (StreamReader streamReader = new StreamReader(ruta))
                    {
                        while ((caracterLeido = streamReader.Read()) != -1)
                        {
                            letraLeida = (char)caracterLeido;
                            if (letraLeida == ';')
                            {
                                if (nombreActual.Trim() != "")
                                {
                                    nombresLeidos.Add(nombreActual);
                                    nombreActual = "";
                                }
                                else
                                {
                                    Console.WriteLine("Nombre omitido por estar vacío");
                                }
                            }
                            else
                            {
                                nombreActual += letraLeida;
                            }
                        }
                    }
                    users = nombresLeidos.ToArray();
                    Console.WriteLine("Lectura Finalizada");
                }
                else
                {
                    Console.WriteLine("No se ha podido cargar el archivo: No existe");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de lectura de alrchivo de nombres");
            }
        }

        public int ReadPin(string ruta)
        {
            int pin;
            lock (l) // lock en readpin
            {
                if (File.Exists(ruta))
                {
                    try
                    {
                        using (BinaryReader br = new BinaryReader(new FileStream(ruta, FileMode.Open)))
                        {
                            pin = br.ReadInt32();
                        }
                        if (pin.ToString().Length == 4)
                        {
                            return pin;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    catch (IOException)
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
        }

        public void CambiarPin(string ruta, int numero)
        {
            try
            {
                lock (l)
                {
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(ruta, FileMode.Create)))
                    {
                        writer.Write(numero);
                    }
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error al crear el archivo binario: {ex.Message}");
            }
        }
    }
}
