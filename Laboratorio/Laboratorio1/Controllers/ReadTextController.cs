using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
using Laboratorio1.ArbolHuffman;
using Laboratorio1.CompresionLZ;

namespace Laboratorio1.Controllers
{
    public class ReadTextController : Controller
    {
        List<NodoHuffman> listadeNodos = new List<NodoHuffman>();
        public string FilePath = "";
        const int bufferLength = 320000000;

        // GET: ReadText
        public ActionResult Index()
        {
            return View();
        }

        //Guardaremos la letra y cuantas veces se repite
        public Dictionary<string, int> Diccionario_CaracteresHuff = new Dictionary<string, int>();
        //Recibo los datos de FileUploadController

        //-------------------COMPRESION DE HUFFMAN -------------------------------------
        public ViewResult Read(string filename)
        {
            List<string> Text_archivo = new List<string>();
            var path = Path.Combine(Server.MapPath("~/Archivo"), filename);
            FilePath = Server.MapPath("~/Archivo");

            
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var byteBuffer = new byte[bufferLength];
                        while (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            byteBuffer = reader.ReadBytes(bufferLength);
                        }
                        foreach (var item in byteBuffer)
                        {
                            if (Diccionario_CaracteresHuff.ContainsKey(Convert.ToString(item)) == true)
                            {
                                Diccionario_CaracteresHuff[Convert.ToString(item)] += 1;
                            }
                            else
                            {
                                Diccionario_CaracteresHuff.Add(Convert.ToString(item), 1);
                            }
                            Text_archivo.Add(Convert.ToString(item));
                        }
                    }
                    ArbolHuff arbol = new ArbolHuff();
                    //Manda a llamar el metodo del arbol en el que agrega a una lista de nodos, los distintos caracteres que existen
                    arbol.agregarNodos(Diccionario_CaracteresHuff, Text_archivo, listadeNodos, FilePath);
                }
                var items = FilesUploaded();
                return View(items);
        }


        //---------------------------------------------------------- COMPRESION LZW  --------------------------------------------------------------------------------------------- -
        public ViewResult ReadLZ(string filename)
        {
            var path = Path.Combine(Server.MapPath("~/Archivo"), filename);
            CompresionLZW compresionLZW = new CompresionLZW();
            compresionLZW.ComprimirLZW(filename, Server.MapPath("~/Archivo"));
            var items = FilesUploaded2();
            return View(items);
        }

        //------------------- MOSTRAR ARCHIVOS COMPRIMIDOS HUFFMAN-------------------------------------
        private List<string> FilesUploaded()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Archivo"));
            //Unicamente tome los archivos de text, ahorita lo puse como doc para probar pero al final lo podriamos dejar como .txt
            System.IO.FileInfo[] fileNames1 = dir.GetFiles("*.huff");
            //Creo una lista con los nombres de todos los archivos para luego poder mostrarlos
            List<string> filesupld = new List<string>();
            foreach (var file1 in fileNames1)
            {
                filesupld.Add(file1.Name);
            }
            //Devuelvo la lista
            return filesupld;
        }

        //------------------- MOSTRAR ARCHIVOS COMPRIMIDOS LZW -------------------------------------
        private List<string> FilesUploaded2()
        {
            var dir = new System.IO.DirectoryInfo(Server.MapPath("~/Archivo"));
            //Unicamente tome los archivos de text, ahorita lo puse como doc para probar pero al final lo podriamos dejar como .txt
            System.IO.FileInfo[] fileNames2 = dir.GetFiles("*.lzw");
            //Creo una lista con los nombres de todos los archivos para luego poder mostrarlos
            List<string> filesupld = new List<string>();
            foreach (var file2 in fileNames2)
            {
                filesupld.Add(file2.Name);
            }
            //Devuelvo la lista
            return filesupld;
        }

        public FileResult Download(string TxtName)
        {
            var FileVirtualPath = "Archivo/" + TxtName;
            return File(FileVirtualPath, "application/force- download", Path.GetFileName(FileVirtualPath));
        }
    }
}


