using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClienteEjercicio5
{
    public partial class Form1 : Form
    {
        string[] palabras;
        string palabra;
        string letras;
        List<Button> buttons;
        List<TextBox> textBoxes;
        Random rn = new Random();
        //bool win;
        public Form1()
        {
            InitializeComponent();

            //palabras = new string[] {
            //    "Desconocido",
            //    "Aislamiento",
            //    "Herejía",
            //    "Razón",
            //    "Transformadores",
            //    "Silo",
            //    "Oxidado"
            //};

            //win = false;
            //palabra = palabras[rn.Next(palabras.Length)].ToUpper();
            palabra =
            //#if DEBUG
            //            lblPalabra.Text += " " + palabra;
            //#endif

            letras = "aábcdeéfghiíjklmnñoópqrstuúüvwxyz";

            buttons = new List<Button>();
            textBoxes = new List<TextBox>();

            CreaBotones();
            CreaTextBoxes();
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

        private void btnMas_Click(object sender, EventArgs e)
        {
            dibujoAhorcado1.Errores++;
        }

        private void btnMenos_Click(object sender, EventArgs e)
        {
            dibujoAhorcado1.Errores--;
        }

        private void dibujoAhorcado1_CambiaError(object sender, EventArgs e)
        {
            lblErrores.Text = "Errores: " + dibujoAhorcado1.Errores;
        }

        private void dibujoAhorcado1_Ahorcado(object sender, EventArgs e)
        {
            lblErrores.Text = "HAS PERDIDO";
        }
    }
}
