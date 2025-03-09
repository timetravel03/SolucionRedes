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
        }

        public void LoopServidor()
        {
            try
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
            catch (SocketException ex)
            {
                Console.WriteLine($"El servidor se apagará ahora {ex.Message}");
            }

        }

        private void FuncionCliente(object socket)
        {
            using (Socket esteSocket = (Socket)socket)
            using (NetworkStream ns = new NetworkStream(esteSocket))
            using (StreamReader sr = new StreamReader(ns))
            using (StreamWriter sw = new StreamWriter(ns))
            {
                // leer input
                sw.AutoFlush = true;
                string input;
                if ((input = sr.ReadLine()) != null)
                {
                    switch (input)
                    {
                        case "getword":
                            string palabra = "";
                            lock (testigoPalabras)
                            {
                                palabra = listaPalabras[rnd.Next(listaPalabras.Count - 1)];
                            }
                            sw.WriteLine(palabra);
                            break;
                        case string c when c.StartsWith("sendword "):
                            sw.WriteLine(GuardarPalabra(c.Split(' ')[1]) ? "OK" : "ERROR");
                            CargarPalabras();
                            break;
                        case "getrecords":
                            string records = "";
                            lock (testigoRecords)
                            {
                                foreach (Record item in listaRecords)
                                {
                                    records += $"{item.Nombre}:{item.Segundos},";
                                }
                            }
                            sw.WriteLine(records);
                            break;
                        case string c when c.StartsWith("sendrecord "):
                            string datosRecord = c.Split(' ')[1];
                            string[] datos = datosRecord.Split(':');
                            Record record = new Record(datos[0], int.Parse(datos[1]));
                            bool valido = RecordValido(record);
                            sw.WriteLine(valido ? "ACCEPT" : "REJECT");
                            if (valido)
                            {
                                GuardarRecords(record);
                                CargarRecords();
                            }
                            break;
                        case string c when c.Contains("closeserver "):
                            string[] values = c.Split(' ');
                            if (values.Length == 2 && values[1] == "password")
                            {
                                socketServidor.Close();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        // funciones de coleccion de palabras
        private bool GuardarPalabra(string palabra)
        {
            try
            {
                if (palabra != "")
                {
                    lock (testigoPalabras)
                    {
                        using (StreamWriter sr = new StreamWriter(rutaArchivoPalabras, true))
                        {
                            sr.Write($"{palabra},");
                            listaPalabras.Add(palabra);
                        }
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
                Console.WriteLine("Error de escritura, no se ha podido añadir la palabra");
                return false;
            }
        }

        private void CargarPalabras()
        {
            try
            {
                lock (testigoPalabras)
                {
                    using (StreamReader sr = new StreamReader(rutaArchivoPalabras))
                    {
                        string[] palabrasLeidas = sr.ReadToEnd().Split(',');
                        for (int i = 0; i < palabrasLeidas.Length; i++)
                        {
                            palabrasLeidas[i] = palabrasLeidas[i].ToUpper();
                        }
                        listaPalabras.AddRange(palabrasLeidas);
                        listaPalabras.RemoveAt(listaPalabras.Count - 1);
                        Console.WriteLine("Lectura de palabras finalizada");
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de lectura del archivo de palabras");
            }
        }

        // funcion de coleccion de records
        private void GuardarRecords(Record record)
        {
            try
            {
                lock (testigoRecords)
                {
                    using (RecordWriter rw = new RecordWriter(new FileStream(rutaArchivoRecords, FileMode.Create)))
                    {
                        foreach (Record item in listaRecords)
                        {
                            rw.WriteRecord(item);
                        }
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine("Error de escritura del archivo de records");
            }
        }

        private bool RecordValido(Record record)
        {
            lock (testigoRecords)
            {
                listaRecords.Add(record);
                OrdernarRecords();
                if (listaRecords.Count > 3)
                {
                    listaRecords.RemoveAt(listaRecords.Count - 1);
                }

                return listaRecords.Contains(record);
            }
        }

        private void OrdernarRecords()
        {
            Record temp;
            lock (testigoRecords)
            {
                for (int i = 0; i < listaRecords.Count; i++)
                {
                    for (int j = 0; j < listaRecords.Count - 1; j++)
                    {
                        if (listaRecords[j].Segundos > listaRecords[j + 1].Segundos)
                        {
                            temp = listaRecords[j + 1];
                            listaRecords[j + 1] = listaRecords[j];
                            listaRecords[j] = temp;
                        }
                    }
                }
            }
        }

        private void CargarRecords()
        {
            try
            {
                lock (testigoRecords)
                {
                    listaRecords.Clear();
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
