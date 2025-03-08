using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoomServicio
{
    public partial class ChatRoomServicio : ServiceBase
    {
        private const short puertoPorDefecto = 31416;
        private IPEndPoint endPoint;
        private short[] arr_puertos;
        private ServerChatroom server;
        public ChatRoomServicio()
        {
            InitializeComponent();
            CanPauseAndContinue = false;
            try
            {
                if ((arr_puertos = CargarPuertos()) != null && arr_puertos.Length > 0)
                {
                    ProbarPuertos(arr_puertos);
                }
                else
                {
                    endPoint = new IPEndPoint(IPAddress.Any, puertoPorDefecto);
                }

                server = new ServerChatroom(endPoint);
            }
            catch (Exception ex)
            {
                WriteEvent($"Error en el constructor: {ex.Message}");
                throw;
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Thread hilo = new Thread(server.IniciarServidor);
                hilo.Start();
            }
            catch (Exception e)
            {
                WriteEvent("Error en Onstart: " + e.Message);
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                server.ApagarServidor();
            }
            catch (Exception e)
            {
                WriteEvent("Error al apagar: " + e.Message);
            }
        }

        public static void WriteEvent(string mensaje)
        {
            string nombre = "ChatRoomService";
            string logDestino = "Application";

            if (!EventLog.SourceExists(nombre))
            {
                EventLog.CreateEventSource(nombre, logDestino);
            }
            EventLog.WriteEntry(nombre, mensaje);
        }

        private short[] CargarPuertos()
        {
            try
            {
                List<short> list_puertos = new List<short>();
                int caracterLeidoInt;
                char caracterLeido;
                string bufferCaracteres = "";
                string ruta = Path.Combine(Environment.GetEnvironmentVariable("PROGRAMDATA"), "puertos.txt");
                using (StreamReader streamReader = new StreamReader(ruta))
                {
                    while ((caracterLeidoInt = streamReader.Read()) != -1)
                    {
                        caracterLeido = (char)caracterLeidoInt;
                        if (caracterLeido != ';')
                        {
                            bufferCaracteres += caracterLeido;
                        }
                        else
                        {
                            if (short.TryParse(bufferCaracteres.Trim(), out short puerto))
                            {
                                list_puertos.Add(puerto);
                            }
                            bufferCaracteres = "";
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(bufferCaracteres))
                    {
                        if (short.TryParse(bufferCaracteres.Trim(), out short puerto))
                        {
                            list_puertos.Add(puerto);
                        }
                    }
                }
                return list_puertos.ToArray();
            }
            catch (IOException ex)
            {
                WriteEvent("Error al cargar puertos del archivo: " + ex.Message);
                return null;
            }
        }


        private void ProbarPuertos(short[] puertos)
        {
            bool libre = false;
            int i = 0;
            Socket s;
            IPEndPoint ie;
            while (!libre && i < arr_puertos.Length)
            {
                try
                {
                    ie = new IPEndPoint(IPAddress.Any, arr_puertos[i]);
                    using (s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        s.Bind(ie);
                    }
                    endPoint = ie;
                    libre = true;
                }
                catch (SocketException e) when (e.ErrorCode == ((int)SocketError.AddressAlreadyInUse))
                {
                    i++;
                }
                catch (SocketException e)
                {
                    WriteEvent(e.SocketErrorCode.ToString());
                    i++;
                }
            }
            if (!libre)
            {
                WriteEvent("No se pudo enlazar a ningún puerto especificado.");
                throw new Exception("No se pudo enlazar a ningún puerto especificado.");
            }
        }
    }
}
