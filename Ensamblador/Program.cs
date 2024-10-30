using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Semeantica
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lenguaje L = new Lenguaje("prueba.cpp"))
                {
                    /*
                    while(!L.finArchivo())
                    {
                        L.nextToken();
                    }
                    */
                    L.Programa();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
