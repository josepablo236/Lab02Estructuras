
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Laboratorio1.CompresionLZ
{
    public class CompresionLZW
    {
        const int bufferLength = 4000000;
        public static int tamaño;
        public static Dictionary<string, int> dictionary = new Dictionary<string, int>();
        public static Dictionary<string, int> dictionaryoriginals = new Dictionary<string, int>();

        public void ComprimirLZW(string filename, string path)
        {
            dictionary.Clear();
            dictionaryoriginals.Clear();
            var Text_archivo = ReadFile(filename, path);
            var compressed = Compress(Text_archivo);
            var binary = StringToBinary(compressed, tamaño);
            var bytes = StringToBytes(binary);
            WriteCompress(bytes, path, filename);
        }


        public List<string> ReadFile(string filename, string path)
        {
            List<string> Text_archivo = new List<string>();

            using (var stream = new FileStream(path + "/" + filename, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var byteBuffer = new byte[bufferLength];
                    // while (reader.BaseStream.Position != reader.BaseStream.Length)
                    //{

                    byteBuffer = reader.ReadBytes(bufferLength);

                    foreach (var item in byteBuffer)
                    {
                        Text_archivo.Add(item.ToString());
                    }
                    //}
                }
            }
            return Text_archivo;
        }

        public static List<string> Compress(List<string> uncompressed)
        {

            foreach (var item in uncompressed)
            {
                if (!dictionary.ContainsKey(item))
                {
                    dictionary.Add(item, dictionary.Count + 1);
                    dictionaryoriginals.Add(item, dictionary.Count);
                }
            }
            string anterior = string.Empty;
            List<string> compressed = new List<string>();
            foreach (var actual in uncompressed)
            {
                string union = anterior + actual;
                if (dictionary.ContainsKey(union))
                {
                    anterior = union;
                }
                else
                {
                    compressed.Add(dictionary.FirstOrDefault(x => x.Key == anterior).Value.ToString());
                    dictionary.Add(union, dictionary.Count + 1);
                    anterior = actual;
                }
            }

            if (!string.IsNullOrEmpty(anterior))
                compressed.Add(dictionary.FirstOrDefault(x => x.Key == anterior).Value.ToString());

            var ttamaño = compressed.Select(s => int.Parse(s)).ToList().Max();
            tamaño = Convert.ToString(Convert.ToInt32(ttamaño), 2).Length;
            return compressed;

        }

        private string StringToBinary(List<string> data, int tamaño)
        {
            string byteList = "";
            foreach (var item in data)
            {
                var temp = Convert.ToString(Convert.ToInt32(item), 2);
                if (temp.Length < tamaño)
                {
                    temp = temp.PadLeft(tamaño, '0');
                    byteList += temp;
                }
                else { byteList += temp; }
            }
            return byteList;
        }


        private List<byte> StringToBytes(string data)
        {
            List<Byte> byteList = new List<Byte>();
            for (int i = 0; i < data.Length; i += 8)
            {
                if (data.Length - i >= 8)
                {
                    byteList.Add(Convert.ToByte(data.Substring(i, 8), 2));
                }
                else
                {
                    string temp = data.Substring(i, data.Length - i);
                    temp = temp.PadLeft(8, '0');
                    byteList.Add(Convert.ToByte(temp, 2));
                }
            }
            return byteList;
        }

        public void WriteCompress(List<byte> TextoCodificado, string FilePath, string textname)
        {

            var path = Path.Combine(FilePath, Path.GetFileNameWithoutExtension(textname) + ".lzw");
            using (var writeStream1 = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream1))
                {
                    foreach (var item in TextoCodificado)
                    {
                        writer.Write(item);
                    }
                    writer.Close();
                }
                writeStream1.Close();
            }
            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    using (var writer2 = new StreamWriter(stream))
                    {
                        var byteBufferLeer = new byte[bufferLength];
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBufferLeer = reader.ReadBytes(bufferLength);
                        }
                        if (reader.BaseStream.Position == reader.BaseStream.Length)
                        {
                            writer2.Write("|||");
                            foreach (var item in dictionaryoriginals)
                            {
                                writer2.Write(item);
                            }
                            writer2.Write(tamaño.ToString());
                        }
                    }
                }
            }
        }
    }
}
