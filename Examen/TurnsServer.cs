using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Examen
{
    internal class TurnsServer
    {
        private string[] students;
        private string teacherPass;
        private int[] ports = new int[] { 135, 0 };
        private List<string> queue = new List<string>();

        private string nameFileRoute = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\lista.txt";

        private IPEndPoint endPoint;
        private Socket socketServer;
        private object l = new object();

        private int TestPort(string port)
        {
            if (ushort.TryParse(port, out ushort validPort))
            {
                return (int)validPort;
            }
            else
            {
                return -1;
            }
        }

        private bool ReadData()
        {
            bool isOK = false;
            try
            {
                using (StreamReader sr = new StreamReader(nameFileRoute))
                {
                    students = sr.ReadLine().Split(',');
                    teacherPass = sr.ReadLine();
                    isOK = (ports[1] = TestPort(sr.ReadLine())) != -1;
                }
            }
            catch (IOException)
            {
                Console.WriteLine("IO ERROR");
            }
            return isOK;
        }

        public void Init()
        {
            try
            {
                if (ReadData())
                {
                    bool binded = false;
                    socketServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    endPoint = new IPEndPoint(IPAddress.Any, ports[0]);

                    try
                    {
                        socketServer.Bind(endPoint);
                        binded = true;
                    }
                    catch (SocketException e) when (e.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                    {
                        try
                        {
                            endPoint = new IPEndPoint(IPAddress.Any, ports[1]);
                            socketServer.Bind(endPoint);
                            binded = true;
                        }
                        catch (SocketException ex) when (ex.ErrorCode == (int)SocketError.AddressAlreadyInUse)
                        {
                            Console.WriteLine("All ports are occupied, the server will now turn off");
                        }
                    }

                    if (binded)
                    {
                        socketServer.Listen(10);
                        while (true)
                        {
                            Socket s = socketServer.Accept();
                            Thread hilo = new Thread(() => Client(s));
                            hilo.IsBackground = true;
                            hilo.Start();
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine($"AN EXCEPTION HAS OCURRED: {e.Message}"); }
            finally
            {
                Console.WriteLine("Server will now turn off");
            }
        }

        private void Client(Socket s)
        {
            IPAddress clientAddress = null;
            int clientPort = -1;
            try
            {
                IPEndPoint ep;
                bool teacher;
                using (Socket clientSocket = s)
                using (NetworkStream ns = new NetworkStream(clientSocket))
                using (StreamWriter sw = new StreamWriter(ns))
                using (StreamReader sr = new StreamReader(ns))
                {
                    ep = (IPEndPoint)clientSocket.RemoteEndPoint;
                    clientAddress = ep.Address;
                    clientPort = ep.Port;
                    sw.AutoFlush = true;
                    sw.WriteLine("Type in your username:");
                    string username = sr.ReadLine();
                    if (username != null)

                    {
                        teacher = username == teacherPass;

                        if (students.Contains(username))
                        {
                            string command = sr.ReadLine();
                            lock (l)
                            {
                                if (command != null && command == "add" && !queue.Contains(username))
                                {
                                    queue.Add(username);
                                }
                                sw.WriteLine($"User in queue position: {queue.IndexOf(username) + 1}");
                            }
                        }
                        else if (teacher)
                        {
                            bool connected = true;
                            string command;
                            while (connected && (command = sr.ReadLine()) != null)
                            {
                                switch (command)
                                {
                                    case "list":
                                        lock (l)
                                        {
                                            foreach (string student in queue)
                                            {
                                                sw.WriteLine(student);
                                            }
                                        }
                                        break;
                                    case string c when c.Contains("del "):
                                        string[] values = command.Split(' ');
                                        lock (l)
                                        {
                                            if (values.Length == 3 && uint.TryParse(values[1], out uint n1) && uint.TryParse(values[2], out uint n2) && n2 < queue.Count && n2 >= n1)
                                            {
                                                for (int i = (int)n2; i >= (int)n1; i--)
                                                {
                                                    queue.RemoveAt(i);
                                                }
                                            }
                                        }
                                        break;
                                    case "exit":
                                        connected = false;
                                        break;
                                    default:
                                        sw.WriteLine("Comando no reconocido");
                                        break;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"AN EXCEPTION HAS OCURRED: {e.Message}");
            }
            finally
            {
                if (clientAddress != null && clientPort != -1)
                {
                    Console.WriteLine($"A client has left {clientAddress}:{clientPort}");
                }
                else
                {
                    Console.WriteLine("A client has left (NO ADDRESS)");
                }
            }
        }
    }
}
