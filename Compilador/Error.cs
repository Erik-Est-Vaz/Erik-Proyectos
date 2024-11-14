using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//ESTE ASEGÚN ESTA CORRECTO a

namespace Compilador
{
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log) : base("\nError: " + mensaje)
        {
            log.WriteLine("Error:" + mensaje);
        }
    }
}