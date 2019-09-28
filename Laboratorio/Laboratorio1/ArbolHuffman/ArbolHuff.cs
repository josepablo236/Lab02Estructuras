using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Laboratorio1.Controllers;

namespace Laboratorio1.ArbolHuffman
{
    public class ArbolHuff
    {
        public double EPSILON { get; private set; }
        public object Covert { get; private set; }
        const int bufferLength = 8;
        const int bufferLeerLength = 1000;
        public static List<NodoHuffman> listaNodos;
        List<NodoHuffman> Arbol = new List<NodoHuffman>();
        List<NodoHuffman> ArbolCodigos = new List<NodoHuffman>();
        public static List<string> texto = new List<string>();
        Dictionary<string, string> Diccionariocodigos = new Dictionary<string, string>();
        public Dictionary<string, string> codigos = new Dictionary<string, string>();
        string TextoEnCodigo;
        List<string> TextoAscii = new List<string>();
        List<string> textocodificado = new List<string>();

        string FilePath;


        //Recibe el string de caracteres y los agrega a un dicctionario, con su respectiva probabilidad y caracter, para luego crear los nodos
        public void agregarNodos(Dictionary<string, int> Diccionario_Caracteres, List<string> Text_archivo, List<NodoHuffman> listadeNodos, string path)
        {
            texto = Text_archivo;
            FilePath = path;
            foreach (var item in Diccionario_Caracteres)
            {
                var nodotemp = new NodoHuffman();
                nodotemp.probabilidad = Math.Round((item.Value / Convert.ToDouble(Text_archivo.Count)), 5); //Calcula la probabilidad
                nodotemp.caracter = item.Key;                             //El caracter es la llave primaria
                listadeNodos.Add(nodotemp);                               //Agrega cada nuevo nodo a una lista de nodos
            }
            AgregarNodoAlArbol(listadeNodos);   //Llama a la funcion para agregar los nodos al arbol
        }
        //Ordena los nodos dependiendo de su probabilidadl, ascendentemente
        public void OrdenamientoListaNodos(List<NodoHuffman> listadeNodos)
        {
            listaNodos = listadeNodos.OrderBy(x => x.probabilidad).ToList();
        }
        //Recibe la lista de notos de agregarNodos
        private ActionResult View(string arbol)
        {
            throw new NotImplementedException();
        }

        //CREAR ARBOL EN BASE A SU PROBABILIDAD
        public void AgregarNodoAlArbol(List<NodoHuffman> listadeNodos)
        {
            OrdenamientoListaNodos(listadeNodos);
            //Ya que deben de existir minimo 2 nodos para poder emparejarlos
            if (listaNodos.Count > 2)
            {
                var nodotemp = new NodoHuffman();
                //La probabilidad del nodo es la suma de la probabilidad de sus nodos hijos, iz y derecha.
                nodotemp.probabilidad = (listaNodos[0].probabilidad + listaNodos[1].probabilidad);
                //Ya que cuando es un nodo, producto de la suma de 2 letras, no nos importa el caracter, solo su probabilidad
                nodotemp.caracter = Convert.ToString("x");
                //El nodo con menor probabilidad se convierte en el hijo derecha.
                if (listadeNodos[0].probabilidad < listadeNodos[1].probabilidad)
                {
                    nodotemp.HijoIzquierdo = listaNodos[1];
                    nodotemp.HijoDerecho = listaNodos[0];
                }
                else if (listadeNodos[1].probabilidad < listadeNodos[0].probabilidad)
                {
                    nodotemp.HijoIzquierdo = listaNodos[0];
                    nodotemp.HijoDerecho = listaNodos[1];
                }
                else
                {
                    nodotemp.HijoIzquierdo = listaNodos[1];
                    nodotemp.HijoDerecho = listaNodos[0];
                }
                //Eliminamos los 2 mas pequeños de la lista, ya que forman un nuevo nodo, y se vuelven hijos del nuevo nodo
                listaNodos.RemoveAt(0);
                listaNodos.RemoveAt(0);
                //Se agrega a la lista de nodos, el nuevo nodo creado
                listaNodos.Add(nodotemp);
                Arbol.Add(nodotemp);
                //Se ordena nuevamente encontrar nuevamente los 2 mas pequeños
                //La función es recursiva, hasta que en la lista unicamente queden menos de 2 nodos
                AgregarNodoAlArbol(listaNodos);

            }
            else
            {
                NodoHuffman nodotemp = new NodoHuffman();
                //La probabilidad del nodo es la suma de la probabilidad de sus nodos hijos, iz y derecha.
                nodotemp.probabilidad = (listaNodos[0].probabilidad + listaNodos[1].probabilidad);
                //Ya que cuando es un nodo, producto de la suma de 2 letras, no nos importa el caracter, solo su probabilidad
                nodotemp.caracter = Convert.ToString("x");
                nodotemp.HijoIzquierdo = listaNodos[1];
                nodotemp.HijoDerecho = listaNodos[0];
                Arbol.Add(nodotemp);
                string lado = "raiz";
                Codigo(nodotemp, nodotemp, lado);
                GenerarTextoCodigo(Diccionariocodigos);
                Agrupar();
            }
        }

        //ASIGNAR A CADA NODO SU CODIGO EN BASE A SU POSICION EN EL ARBOL, EN UN DICCIONARIO SE GUARDA EL CODIGO DE CADA HOJA
        public void Codigo(NodoHuffman nodo, NodoHuffman nodopadre, string lado)
        {
            if (nodo.HijoIzquierdo != null)
            {
                nodo.HijoIzquierdo.codigo = nodo.codigo + "0";
                Codigo(nodo.HijoIzquierdo, nodo, "iz");
            }
            if (nodo.HijoDerecho != null)
            {
                nodo.HijoDerecho.codigo = nodo.codigo + "1";
                Codigo(nodo.HijoDerecho, nodo, "der");
            }
            if (EsHoja(nodo))
            {
                if (lado == "iz")
                {
                    nodo.codigo = nodopadre.codigo + "0";
                    Diccionariocodigos.Add(nodo.caracter, nodo.codigo);
                }
                if (lado == "der")
                {
                    nodo.codigo = nodopadre.codigo + "1";
                    Diccionariocodigos.Add(nodo.caracter, nodo.codigo);
                }
            }
        }

        public bool EsHoja(NodoHuffman nodo)
        {
            if (nodo.HijoIzquierdo == null && nodo.HijoIzquierdo == null)
            { return true; }
            else { return false; }

        }

        //Agarro cada codigo en binario que fue generado en el arbol, en base al texto orginal, se replaza cada caracter con su codigo
        public void GenerarTextoCodigo(Dictionary<string, string> Diccionariocodigos)
        {
            foreach (string letra in texto)
            {
                TextoEnCodigo += Diccionariocodigos.FirstOrDefault(x => x.Key == letra).Value;
            }
        }

        //ESCRIBIR Y CREAR ARCHIVO .HUFF CON EL TEXTO COMPRIMIDO Y DICCIONARIO
        public void Agrupar()
        {
            var path = Path.Combine(FilePath, "ArchivoComprimido.huff");
            using (var writeStream1 = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var writer = new BinaryWriter(writeStream1))
                {
                    List<Byte> listabytes = StringToBytes(TextoEnCodigo);
                    foreach (var item in listabytes)
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
                        var byteBufferLeer = new byte[bufferLeerLength];
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBufferLeer = reader.ReadBytes(bufferLeerLength);
                        }
                        if (reader.BaseStream.Position == reader.BaseStream.Length)
                        {
                            writer2.Write("||");
                            foreach (var item in Diccionariocodigos)
                            {
                                writer2.Write(item);
                            }
                        }
                    }
                }
            }
        }

        //CONVERTIR EL TEXTO CON SU CODIGO A BYTES 
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
                    string binario = "0000000";
                    string temp = binario.Substring(0, 8 - (data.Length - i)) + data.Substring(i, data.Length - i);
                    byteList.Add(Convert.ToByte(temp, 2));
                }
            }
            return byteList;
        }

        public ActionResult GuardarArchivo()
        {
            string archivo = "Archivo.huff";
            return View(archivo);
        }
    }
}


