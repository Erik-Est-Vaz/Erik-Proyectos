using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semantica
{
    public class Sintaxis : Lexico
    {
        int errorLinea;
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
            if (getContenido() == espera)
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
            if (getClasificacion() == espera)
            {
                nextToken();
            }
            else
            {
                throw new Error("Sintaxis: se espera un "+espera,log);
            }
        }
    }
}