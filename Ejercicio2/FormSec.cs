using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ejercicio2
{
    public partial class FormSec : Form
    {
        private bool t1;
        private bool t2;
        public IPAddress ip;
        public int puerto;
        private string ipText;
        private string puertoText;
        public IPAddress addressOut;
        public short puertoOut;
        public FormSec()
        {
            InitializeComponent();

            t1 = t2 = false;

            textBox1.TextChanged += new EventHandler(textBox_TextChanged);
            textBox2.TextChanged += new EventHandler(textBox_TextChanged);
            btnGuardar.Click += new EventHandler(btnGuardar_Click);
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ipText = textBox1.Text.Trim();
            puertoText = textBox2.Text.Trim();
            if (IPAddress.TryParse(ipText, out addressOut) && short.TryParse(puertoText, out puertoOut) && puertoOut > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            } else
            {
                MessageBox.Show("IP o Puerto no válidos","Aviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (txt == textBox1)
            {
                t1 = txt.Text.Trim() != "";
            }
            else
            {
                t2 = txt.Text.Trim() != "";
            }
            btnGuardar.Enabled = (t1 && t2);
        }

        private void FormSec_Load(object sender, EventArgs e)
        {
            textBox1.Text = ip.ToString();
            textBox2.Text = puerto.ToString();
            //IPEndPoint.MaxPort;
        }
    }
}
