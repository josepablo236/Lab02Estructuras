using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Ajax.Utilities;

namespace Laboratorio1.CompresionLZ
{
    public class DescompresionLZW
    {
        public static int tamaño;
        public static Dictionary<int, string> dictionary = new Dictionary<int, string>();
        public string DescomprimirLZW(string filename, string path)
        {
            dictionary.Clear();
            var read = ReadFile(filename, path);
            var temp = DecimalToBinary(read);
            var agrupados = Agrupar(temp, tamaño);
            var descompress = Descompress(agrupados);
            WriteDescompress(descompress, path, filename);
            return Path.GetFileNameWithoutExtension(filename) + "DESCOMPRIMIDO.txt";
        }

        public List<string> ReadFile(string textname, string filepath)
        {
            string textocompleto;
            var path = Path.Combine(filepath, textname);

            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    textocompleto = reader.ReadToEnd();
                }
            }
            string[] palabras = textocompleto.Split(new string[] { "|||" }, StringSplitOptions.None);
            string codificado = palabras[0];
            textocompleto = textocompleto.Substring(codificado.Length + 3);
            char[] delimiters = new char[] { '[', ']', ',', ' ' };
            string[] parts = textocompleto.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[0].Substring(0, 1) == " ")
                {
                    parts[0] = parts[0].Substring(1, parts[0].Length - 1);
                }
            }

            for (int i = 0; i < parts.Length - 1; i += 2)
            {
                dictionary.Add(Convert.ToInt32(parts[i + 1]), parts[i]);
            }

            tamaño = Convert.ToInt32(parts[parts.Length - 1]);
            int bufferLength = codificado.Length;
            var byteBuffer = new byte[320000000];
            List<string> Text_archivo = new List<string>();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    byteBuffer = reader.ReadBytes(bufferLength);

                    foreach (var item in byteBuffer)
                    {
                        Text_archivo.Add(Convert.ToString(item));
                    }
                }
            }
            return Text_archivo;
        }

        //Convertir a Binario
        static string DecimalToBinary(List<string> n)
        {
            string binario = "";
            foreach (var item in n)
            {
                var temp = Convert.ToString(Convert.ToInt32(item), 2);
                if (temp.Length == 8)
                {
                    binario += temp;
                }
                else
                {
                    binario += temp.PadLeft(8, '0');
                }

            }
            return binario;
        }

        public List<int> Agrupar(string binario, int tamaño)
        {
            List<int> compress = new List<int>();
            for (int i = 0; i < binario.Length; i += tamaño)
            {
                if (i + tamaño <= binario.Length)
                {
                    compress.Add(Convert.ToInt32(binario.Substring(i, tamaño), 2));
                }
                else
                {
                    compress.Add(Convert.ToInt32(binario.Substring(i, binario.Length - i), 2));
                    break;
                }
            }
            return compress;
        }
        public static string[] Descompress(List<int> compressed)
        {
            string descom = "";
            string anterior = string.Empty;
            foreach (var actual in compressed)
            {
                string union = "";
                if (anterior == "")
                {
                    union = dictionary[actual];
                }
                else
                {
                    union = anterior + "|" + dictionary[actual];
                }
                if (dictionary.ContainsValue(union))
                {
                    anterior = union;
                }
                else
                {
                    if (anterior != "")
                    {
                        if (anterior.Substring(0, 1) == "|")
                            descom += anterior;
                        else
                            descom += "|" + anterior;
                    }
                    var temporal = dictionary[actual].Split('|');
                    dictionary.Add(dictionary.Count + 1, anterior + "|" + temporal[0]);
                    anterior = dictionary[actual];
                }
            }

            char[] delimiters = new char[] { '|' };
            string[] parts2 = descom.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            return parts2;
        }




        void WriteDescompress(string[] descompress, string filepath, string textname)
        {
            var path = Path.Combine(filepath, System.IO.Path.GetFileNameWithoutExtension(textname) + "DESCOMPRIMIDO.txt");
            using (var writeStream1 = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream1))
                {
                    foreach (var letra in descompress)
                    {
                        writer.Write(Convert.ToByte(letra));
                    }
                    writer.Close();
                }
                writeStream1.Close();
            }
        }
    }
}
