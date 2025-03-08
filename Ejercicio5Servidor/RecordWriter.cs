using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio5Servidor
{
    internal class RecordWriter : BinaryWriter
    {
        public RecordWriter(Stream str) : base(str) { }

        public void WriteRecord(Record r)
        {
            base.Write(r.Nombre);
            base.Write(r.Segundos);
        }
    }
}
