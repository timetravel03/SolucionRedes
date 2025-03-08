using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClienteE1
{
    public partial class Form1 : Form // Puerto válido.
    {
        public IPAddress IpServidor { set; get; }
        public int Puerto { set; get; }
        private IPEndPoint ie;
        private Socket conexionServidor;
        public Form1()
        {
            InitializeComponent();

            IpServidor = IPAddress.Parse("127.0.0.1");
            Puerto = 31416;

            btnTime.Click += new EventHandler(SolicitudServidor);
            btnDate.Click += new EventHandler(SolicitudServidor);
            btnAll.Click += new EventHandler(SolicitudServidor);
            btnClose.Click += new EventHandler(SolicitudServidor);
            btnClose.Enabled = false;
            lblOutput.Text = "";
        }
        public void SolicitudServidor(object sender, EventArgs e)
        {
            try
            {
                ie = new IPEndPoint(IpServidor, Puerto);
                conexionServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                conexionServidor.Connect(ie);
                using (NetworkStream ns = new NetworkStream(conexionServidor))
                using (StreamReader sr = new StreamReader(ns))
                using (StreamWriter sw = new StreamWriter(ns))
                {
                    Button botonPulsado = (Button)sender;
                    string comando;
                    if (btnClose.Enabled && botonPulsado == btnClose)
                    {
                        comando = botonPulsado.Text + " " + textBox1.Text.Trim();
                    }
                    else
                    {
                        comando = botonPulsado.Text;
                    }
                    sw.WriteLine(comando);
                    sw.Flush();
                    string respuesta = sr.ReadLine();
                    lblOutput.Text = respuesta;
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error connection: {0}\nError code: {1}({2})", ex.Message, (SocketError)ex.ErrorCode, ex.ErrorCode);
                lblOutput.Text = "Error: No se ha podido conectar al servidor";
            }
            conexionServidor.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() != "")
            {
                btnClose.Enabled = true;
            }
            else
            {
                btnClose.Enabled = false;
            }
        }

        private void btnAjustes_Click(object sender, EventArgs e)
        {
            FormSec sec = new FormSec();
            sec.ip = IpServidor;
            sec.puerto = Puerto;
            DialogResult dr = sec.ShowDialog();
            if (dr == DialogResult.OK)
            {
                IpServidor = sec.addressOut;
                Puerto = sec.puertoOut;
            }
        }
    }
}
