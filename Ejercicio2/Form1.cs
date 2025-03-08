using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Ejercicio2
{
    public partial class Form1 : Form
    {
        public IPAddress IpServidor { set; get; }
        public int Puerto { set; get; }
        private IPEndPoint ie;
        private Socket conexionServidor;
        public Form1()
        {
            InitializeComponent();
            if (!File.Exists($"{Environment.GetEnvironmentVariable("USERPROFILE")}\\configuracion_turnos.bin"))
            {
                IpServidor = IPAddress.Parse("127.0.0.1");
                Puerto = 31416;
                txtUser.Text = "";
            } else
            {
                LeerDatos();
            }
            txtUser_TextChanged(this, EventArgs.Empty);
            btnAdd.Click += new EventHandler(SolicitudServidor);
            btnList.Click += new EventHandler(SolicitudServidor);
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
                    lblOutput.Text = "";
                    Button botonPulsado = (Button)sender;
                    string usuario = txtUser.Text.Trim();
                    sw.WriteLine(usuario);
                    sw.Flush();
                    string comando;
                    comando = botonPulsado.Tag.ToString();
                    sw.WriteLine(comando);
                    sw.Flush();
                    string respuesta = sr.ReadLine();
                    if (comando == btnList.Tag.ToString())
                    {
                        string[] list = respuesta.Split(';');
                        for (int i = 0; i < list.Length; i++)
                        {
                            lblOutput.Text += (list[i] + Environment.NewLine);
                        }
                    }
                    else
                    {
                        lblOutput.Text = respuesta;
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Error connection: {0}\nError code: {1}({2})", ex.Message, (SocketError)ex.ErrorCode, ex.ErrorCode);
                lblOutput.Text = "Error: No se ha podido conectar al servidor";
            }
            finally
            {
                conexionServidor.Close();
            }
        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = txtUser.Text.Trim() != "" && txtUser.Text != "admin";
            btnList.Enabled = btnAdd.Enabled;
        }

        private void GuardarDatos()
        {
            try
            {
                using (BinaryWriter bw = new BinaryWriter(new FileStream($"{Environment.GetEnvironmentVariable("USERPROFILE")}\\configuracion_turnos.bin", FileMode.Create)))
                {
                    bw.Write(IpServidor.ToString());
                    bw.Write(Puerto);
                    bw.Write(txtUser.Text);
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de escritura");
            }
        }

        private void LeerDatos()
        {
            try
            {
                using (BinaryReader br = new BinaryReader(new FileStream($"{Environment.GetEnvironmentVariable("USERPROFILE")}\\configuracion_turnos.bin", FileMode.Open)))
                {
                    IpServidor = IPAddress.Parse(br.ReadString());
                    Puerto = br.ReadInt32();
                    txtUser.Text = br.ReadString();
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de lectura");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            GuardarDatos();
        }
    }
}
