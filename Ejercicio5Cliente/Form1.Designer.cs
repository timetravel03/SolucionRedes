namespace Ejercicio5Cliente
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.dibujoAhorcado1 = new SolucionTema5.DibujoAhorcado();
            this.lblPalabra = new System.Windows.Forms.Label();
            this.lblErrores = new System.Windows.Forms.Label();
            this.lblTiempo = new System.Windows.Forms.Label();
            this.btnInicio = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dibujoAhorcado1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 426);
            this.panel1.TabIndex = 0;
            // 
            // dibujoAhorcado1
            // 
            this.dibujoAhorcado1.Errores = 0;
            this.dibujoAhorcado1.Location = new System.Drawing.Point(3, 3);
            this.dibujoAhorcado1.Name = "dibujoAhorcado1";
            this.dibujoAhorcado1.Size = new System.Drawing.Size(294, 420);
            this.dibujoAhorcado1.TabIndex = 0;
            this.dibujoAhorcado1.Text = "dibujoAhorcado1";
            // 
            // lblPalabra
            // 
            this.lblPalabra.AutoSize = true;
            this.lblPalabra.Location = new System.Drawing.Point(318, 70);
            this.lblPalabra.Name = "lblPalabra";
            this.lblPalabra.Size = new System.Drawing.Size(69, 16);
            this.lblPalabra.TabIndex = 1;
            this.lblPalabra.Text = "PALABRA";
            // 
            // lblErrores
            // 
            this.lblErrores.AutoSize = true;
            this.lblErrores.Location = new System.Drawing.Point(318, 40);
            this.lblErrores.Name = "lblErrores";
            this.lblErrores.Size = new System.Drawing.Size(82, 16);
            this.lblErrores.TabIndex = 2;
            this.lblErrores.Text = "Errores: 0/11";
            // 
            // lblTiempo
            // 
            this.lblTiempo.AutoSize = true;
            this.lblTiempo.Location = new System.Drawing.Point(318, 12);
            this.lblTiempo.Name = "lblTiempo";
            this.lblTiempo.Size = new System.Drawing.Size(67, 16);
            this.lblTiempo.TabIndex = 3;
            this.lblTiempo.Text = "Tiempo: 0";
            // 
            // btnInicio
            // 
            this.btnInicio.Location = new System.Drawing.Point(686, 12);
            this.btnInicio.Name = "btnInicio";
            this.btnInicio.Size = new System.Drawing.Size(102, 47);
            this.btnInicio.TabIndex = 4;
            this.btnInicio.Text = "Empezar Partida";
            this.btnInicio.UseVisualStyleBackColor = true;
            this.btnInicio.Click += new System.EventHandler(this.btnInicio_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnInicio);
            this.Controls.Add(this.lblTiempo);
            this.Controls.Add(this.lblErrores);
            this.Controls.Add(this.lblPalabra);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private SolucionTema5.DibujoAhorcado dibujoAhorcado1;
        private System.Windows.Forms.Label lblPalabra;
        private System.Windows.Forms.Label lblErrores;
        private System.Windows.Forms.Label lblTiempo;
        private System.Windows.Forms.Button btnInicio;
    }
}

