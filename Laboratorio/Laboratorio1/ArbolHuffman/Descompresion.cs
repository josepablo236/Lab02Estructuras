using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Laboratorio1.ArbolHuffman;
using Newtonsoft.Json;

namespace Laboratorio1.Controllers
{
    public class Descompresion
    {
        public string LeerArchivo(string textname, string filepath)
        {
            //Guardaremos la letra y cuantas veces se repite
            string[] texto;
            List<Byte> byteList = new List<Byte>();
            string textocompleto;
            Dictionary<string, string> Diccionario = new Dictionary<string, string>();
            string FileP = filepath;
            List<string> Text_archivo = new List<string>();
            var path = Path.Combine(FileP, textname);
            var result = new Dictionary<string, string>();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    textocompleto = reader.ReadToEnd();
                }
            }
            string[] palabras = textocompleto.Split(new string[] { "||" }, StringSplitOptions.None);
            string codificado = palabras[0];
            textocompleto = textocompleto.Substring(codificado.Length + 2);
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
                Diccionario.Add(parts[i], parts[i + 1]);
            }
            int bufferLength = codificado.Length;
            var byteBuffer = new byte[320000000];
            using (var stream = new FileStream(path, FileMode.Open))
            {
                List<string> Textarchivo = new List<string>();
                using (var reader = new BinaryReader(stream))
                {
                    byteBuffer = reader.ReadBytes(bufferLength);

                    foreach (var item in byteBuffer)
                    {//El delimitador entre el diccionario y el texto comprimido es una barra | 

                        Text_archivo.Add(Convert.ToString(item));
                    }
                }
            }
            string Text_Binario = "";
            //Obtengo el texto en binario
            foreach (var item in Text_archivo)
            {
                Text_Binario += DecimalToBinary(item);
            }
            int inicial = 0; //en que posicion del Text_Binario comenzara a comparar
            string Text_Descomprimido = "";
            int tamano = 1;

            while ((inicial + tamano) <= Text_Binario.Length) //Mientras no se finalice el Text_Binario
            {
                string temp = Text_Binario.Substring(inicial, tamano); //Si el primer caracter no se encuentra en el diccionario, voy tomando en cada vuelta 1 mas hasta encontra un similar
                if (Diccionario.ContainsValue(temp)) //si si se encuentra en el diccionario
                {
                    var tempo = Diccionario.FirstOrDefault(x => x.Value == temp).Key; //Tomo le valor en decimal
                    byteList.Add(Convert.ToByte(tempo));
                    //Text_Descomprimido += tempo; //Se concatena al Text_Descomprimido
                    inicial = inicial + temp.Length;  //Esto significa que ahora tendra que comenzr a comparar a partir de esa posicion en adelanta 
                    tamano = 1;
                }
                else { tamano++; }

            }
            return EscribirDescompresion(byteList, filepath, textname);
        }

        //Convertir a Binario
        static string DecimalToBinary(string n)
        {
            int N = Convert.ToInt32(n); //Lo convierto a un int
            string binario = Convert.ToString(N, 2); //lo convierto en un string de base 2
            int tamano = binario.Length;
            //Ya que cada numero en decimal debe de ocupar 8 posiciones, si el numero en binario es menor a ese tamaño, se le agregan 0 a la derecha
            if (binario.Length < 8)
            {
                for (int i = 0; i < (8 - tamano); i++)
                {
                    binario = "0" + binario;
                }
            }
            return binario;
        }

        public string EscribirDescompresion(List<byte> ListaBytes, string filepath, string textname)
        {
            var path = Path.Combine(filepath, System.IO.Path.GetFileNameWithoutExtension(textname) + ".txt");
            using (var writeStream1 = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream1))
                {
                    foreach (var item in ListaBytes)
                    {
                        writer.Write(item);
                    }
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(textname) + ".txt";
        }
    }
}