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

namespace Ejercicio5Cliente
{
    public partial class Form1 : Form
    {
        string palabra;
        string letras;
        List<Button> buttons;
        List<TextBox> textBoxes;

        Socket server;
        IPEndPoint endPoint;
        Timer timer;
        int timerCounter;
        public Form1()
        {
            InitializeComponent();

            letras = "aábcdeéfghiíjklmnñoópqrstuúüvwxyz";

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(TickFunction);
            buttons = new List<Button>();
            textBoxes = new List<TextBox>();

            CreaBotones();
        }

        private void CreaBotones()
        {
            int x = 235;
            int y = 245;
            int incrementoX = 30;
            int incrementoY = 40;
            for (int i = 1; i <= letras.Length; i++)
            {
                Button b = new Button();
                b.Size = new Size(30, 30);
                b.Text = letras[i - 1].ToString().ToUpper();
                b.Location = new Point(x, y);
                b.Click += new EventHandler(BotonClick);
                this.Controls.Add(b);
                buttons.Add(b);
                if (i % 11 == 0)
                {
                    x = 235;
                    y += incrementoY;
                }
                else
                {
                    x += incrementoX;
                }
            }
        }

        private void CreaTextBoxes()
        {
            int x = lblPalabra.Left;
            int y = lblPalabra.Top + 50;
            int incrementoX = 30;
            //int incrementoY = 40;
            for (int i = 1; i <= palabra.Length; i++)
            {
                TextBox txt = new TextBox();
                txt.Size = new Size(30, 30);
                txt.Location = new Point(x, y);
                txt.Tag = palabra[i - 1];
                txt.Enabled = false;
                this.Controls.Add(txt);
                textBoxes.Add(txt);
                x += incrementoX;
            }
        }

        private void BotonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            bool hallado = false;
            bool completada = true;
            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (textBoxes[i].Tag.ToString() == b.Text.ToString())
                {
                    textBoxes[i].Text = textBoxes[i].Tag.ToString();
                    hallado = true;
                }
            }
            if (!hallado)
            {
                dibujoAhorcado1.Errores++;
            }
            foreach (TextBox item in textBoxes)
            {
                if (item.Text == "" || item.Text == null)
                {
                    completada = false;
                }
            }
            if (completada)
            {
                lblErrores.Text = "HAS GANADO";
            }
        }

        private void TickFunction(object sender, EventArgs e)
        {
            timerCounter++;
        }

        private void GetWord()
        {
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 31416);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(endPoint);
                using (NetworkStream ns = new NetworkStream(server))
                using (StreamWriter sw = new StreamWriter(ns))
                using (StreamReader sr = new StreamReader(ns))
                {
                    sw.WriteLine("getword");
                    sw.Flush();
                    palabra = sr.ReadLine();
                }
                CreaTextBoxes();
                timer.Start();
                timerCounter = 0;
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            GetWord();
            lblPalabra.Text = palabra;
        }
    }
}
