using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;

namespace Compilador
{
    public class Token
    {
        public enum Tipos
        {
            ST,
            SNT,
            Flecha,
            FinProduccion,
            Epsilon,
            OR,
            Derecho,
            Izquierdo,
            Tipo
        };
        public string Contenido{ get; set;}
        public Tipos Clasificacion{ get; set;}
        public Token()
        {
            Contenido = "";
        }
    }
}