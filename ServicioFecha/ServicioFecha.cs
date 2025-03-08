using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServicioFecha
{
    public partial class ServicioFecha : ServiceBase
    {
        private ServerFecha serverFecha;
        private Thread hiloInit;
        public ServicioFecha()
        {
            InitializeComponent();
            serverFecha = new ServerFecha();
            serverFecha.LeerConfig();
        }

        protected override void OnStart(string[] args)
        {
            hiloInit = new Thread(serverFecha.InitServer);
            hiloInit.Start();
            WriteEvent($"Servicio Fecha iniciado, escuchando en {serverFecha.port}");
        }

        protected override void OnStop()
        {
            try
            {
                serverFecha.socket.Close();
            }
            catch (SocketException)
            {
                WriteEvent("Se ha cerrado el socket");
            }
            WriteEvent("Se ha terminado el servicio");
        }

        protected override void OnPause()
        {
            try
            {
                serverFecha.socket.Close();
            }
            catch (SocketException)
            {
                WriteEvent("Se ha cerrado el socket");
            }
            WriteEvent("Servicio en Pausa");
        }
        protected override void OnContinue()
        {
            hiloInit = new Thread(serverFecha.InitServer);
            hiloInit.Start();
            WriteEvent($"Continuando servicio, escuchando en {serverFecha.port}");
        }

        public void WriteEvent(string mensaje)
        {
            string nombre = "ServicioFecha";
            string logDestino = "Application";
            if (!EventLog.SourceExists(nombre))
            {
                EventLog.CreateEventSource(nombre, logDestino);
            }
            EventLog.WriteEntry(nombre, mensaje);
        }
    }
}
