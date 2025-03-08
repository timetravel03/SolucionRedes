using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ServicioPrueba
{
    public partial class SimpleService : ServiceBase
    {
        private int t = 0;
        public SimpleService()
        {
            InitializeComponent();
            this.CanPauseAndContinue = true;
        }

        public void writeEvent(string mensaje)
        {
            string nombre = "SimpleService";
            string logDestino = "Application";

            if (!EventLog.SourceExists(nombre))
            {
                EventLog.CreateEventSource(nombre, logDestino);
            }
            EventLog.WriteEntry(nombre, mensaje);
        }

        protected override void OnStart(string[] args)
        {
            writeEvent("Running OnStart");
            Timer timer = new Timer();
            timer.Interval = 10000;
            timer.Elapsed += new ElapsedEventHandler(this.TimerTick);
            timer.Start();
        }

        protected override void OnPause()
        {
            writeEvent("Servicio en Pausa");
        }

        protected override void OnContinue()
        {
            writeEvent("Continuando servicio");
        }

        protected override void OnStop()
        {
            writeEvent("Deteniendo servicio");
            t = 0;
        }

        public void TimerTick(object sender, ElapsedEventArgs args)
        {
            writeEvent(string.Format($"SimpleService running about {t} seconds"));
            t += 10;
        }

    }
}
