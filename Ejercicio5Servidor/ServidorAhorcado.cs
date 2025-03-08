using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ejercicio5Servidor
{
    internal class ServidorAhorcado
    {
        //archivo de palabras
        private string rutaArchivoPalabras;
        private List<string> listaPalabras;

        //archivo de records (binario)
        private string rutaArchivoRecords;
        private List<Record> listaRecords;

        //networking
        private Socket socketServidor;
        private Socket socketCliente;
        private IPEndPoint ipEndPoint;
        private int puertoServidor = 31416;

        //testigos
        private object testigoPalabras = new object();
        private object testigoRecords = new object();

        //otras cosas
        private Random rnd;

        public ServidorAhorcado()
        {
            rnd = new Random();

            rutaArchivoPalabras = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\palabras_ahorcado.txt";
            rutaArchivoRecords = $"{Environment.GetEnvironmentVariable("USERPROFILE")}\\records_ahorcado.bin";

            listaPalabras = new List<string>();
            listaRecords = new List<Record>();

            CargarPalabras();
            CargarRecords();
            while (listaRecords.Count < 3)
            {
                listaRecords.Add(new Record("---", 9999));
            }
        }

        public void LoopServidor()
        {
            using (socketServidor = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                BuscarPuertos();
                socketServidor.Listen(10);
                Console.WriteLine($"Servidor escuchando en {ipEndPoint.Address}:{ipEndPoint.Port}");
                while (true)
                {
                    socketCliente = socketServidor.Accept();
                    Thread thread = new Thread(FuncionCliente);
                    thread.IsBackground = true;
                    thread.Start(socketCliente);
                }
            }
        }

        private void FuncionCliente(object socket)
        {
            Socket esteSocket = (Socket)socket;
            using (NetworkStream ns = new NetworkStream(esteSocket))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                // leer input
                string input = sr.ReadLine();

                switch (input)
                {
                    case "getword":
                        string palabra = listaPalabras[rnd.Next(listaPalabras.Count - 1)];
                        sw.WriteLine(palabra);
                        break;
                    case string c when c.StartsWith("sendword "):
                        sw.WriteLine(GuardarPalabra(c.Split(' ')[1]) ? "OK" : "ERROR");
                        CargarPalabras();
                        break;
                    case "getrecords":
                        string records = "";
                        foreach (Record item in listaRecords)
                        {
                            records += $"{item.Nombre}:{item.Segundos},";
                        }
                        sw.WriteLine(records);
                        break;
                    case string c when c.StartsWith("sendrecord "):
                        // formato de records -> nombre:segundos,
                        string datosRecord = c.Split(' ')[1];
                        //datosRecord = datosRecord.Remove(',');
                        string[] datos = datosRecord.Split(':');
                        Record record = new Record(datos[0], int.Parse(datos[1]));
                        sw.WriteLine(GuardarRecord(record) ? "ACCEPT" : "REJECT");
                        CargarRecords();
                        break;
                    default:
                        break;
                }
            }
            esteSocket.Close();
        }

        // funciones de coleccion de palabras
        private bool GuardarPalabra(string palabra)
        {
            try
            {
                using (StreamWriter sr = new StreamWriter(rutaArchivoPalabras, true))
                {
                    sr.Write($"{palabra},");
                    listaPalabras.Add(palabra);
                }
                return true;
            }
            catch (IOException)
            {
                Console.WriteLine("Error de escritura, no se ha podido añadir la palabra");
                return false;
            }
        }

        private void CargarPalabras()
        {
            try
            {
                using (StreamReader sr = new StreamReader(rutaArchivoPalabras))
                {
                    listaPalabras.AddRange(sr.ReadToEnd().Split(','));
                    Array.ForEach(listaPalabras.ToArray(), palabra => palabra.ToUpper());
                    Console.WriteLine("Lectura de palabras finalizada");
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de lectura del archivo de palabras");
            }
        }

        // funcion de coleccion de records
        private bool GuardarRecord(Record record)
        {
            bool valido = false;
            int index = 0;
            Record temp;
            try
            {
                while (!valido && index < listaRecords.Count)
                {
                    valido = listaRecords[index].Segundos > record.Segundos;
                    index++;
                }

                if (valido)
                {
                    temp = listaRecords[index];
                    listaRecords[index] = record;
                    while (index < (listaRecords.Count - 1))
                    {
                        index++;
                        listaRecords[index] = temp;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de escritura del archivo de records");
                return false;
            }
        }

        private void OrdernarRecords()
        {
            Record temp;
            if (listaRecords[0].Segundos < listaRecords[1].Segundos)
            {
                temp = listaRecords[0];
                listaRecords[0] = listaRecords[1];
                listaRecords[1] = temp;

            }
            if (listaRecords[1].Segundos < listaRecords[2].Segundos)
            {
                temp = listaRecords[1];
                listaRecords[1] = listaRecords[2];
                listaRecords[2] = temp;
                OrdernarRecords();
            }
        }

        private void CargarRecords()
        {
            try
            {
                using (RecordReader rr = new RecordReader(new FileStream(rutaArchivoRecords, FileMode.Open)))
                {
                    try
                    {
                        while (true)
                        {
                            listaRecords.Add(rr.ReadRecord());
                        }
                    }
                    catch (EndOfStreamException)
                    {
                        Console.WriteLine("Lectura de Records finalizada");
                        OrdernarRecords();
                    }

                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de lectura del archivo de records");
            }
        }

        private void BuscarPuertos()
        {
            bool encontrado = false;
            while ((IPEndPoint.MaxPort >= puertoServidor) && !encontrado)
            {
                try
                {
                    ipEndPoint = new IPEndPoint(IPAddress.Any, puertoServidor);
                    socketServidor.Bind(ipEndPoint);
                    encontrado = true;
                    Console.WriteLine($"Puerto: {ipEndPoint.Port}");
                }
                catch (SocketException)
                {
                    puertoServidor++;
                }
            }
            if (!encontrado)
            {
                socketServidor.Close();
            }
        }
    }
}
