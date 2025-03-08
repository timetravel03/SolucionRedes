using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio5Servidor
{
    internal class RecordReader : BinaryReader
    {
        public RecordReader(Stream str) : base(str) { }

        public Record ReadRecord()
        {
            string nombre = base.ReadString();
            int segundos = base.ReadInt32();

            return new Record(nombre, segundos);
        }
    }
}
