using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semeantica
{
    //Requerimiento: Número de linea donde se encuentra el error
    public class Error : Exception
    {
        public Error(string mensaje, StreamWriter log) : base("\nError: " + mensaje)
        {
            log.WriteLine("Error:" + mensaje);
        }
    }
}