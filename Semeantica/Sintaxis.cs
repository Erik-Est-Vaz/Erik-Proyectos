using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Sintaxis : Lexico
    {
        public int errorLinea{get; set; }   
        public Sintaxis()
        {
            nextToken();
        }
        public Sintaxis(string nombre) : base(nombre)
        {
            errorLinea = nextToken();
        }
        public void match(string espera)
        {
            if (Contenido == espera)
            {
                errorLinea = nextToken();
            }
            else
            {
                throw new Error("Linea " + errorLinea + " Sintaxis: se espera un "+espera,log);
            }
        }
        public void match(Tipos espera)
        {
            if (Clasificacion == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Linea " + errorLinea + " Sintaxis: se espera un "+espera,log);
            }
        }
    }
}