using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio5Servidor
{
    internal class Record
    {
        private string nombre;
        public string Nombre
        {
            set
            {
                if (value.Length == 3)
                {
                    nombre = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            get
            {
                return nombre;
            }
        }

        private int segundos;
        public int Segundos
        {
            set
            {
                if (value >= 0)
                {
                    segundos = value;
                } else
                {
                    throw new ArgumentException();
                }
            }

            get
            {
                return segundos;
            }
        }

        public Record(string nombre, int segudos)
        {
            Nombre = nombre;
            Segundos = segudos;
        }
    }
}
