using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboratorio1.ArbolHuffman
{
    public class NodoHuffman
    {
        public string caracter { get; set; }
        public double probabilidad { get; set; }
        public string codigo { get; set; }
        public NodoHuffman HijoDerecho { get; set; }
        public NodoHuffman HijoIzquierdo { get; set; }
    }
}