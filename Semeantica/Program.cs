using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sintaxis_1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (Lexico L = new Lenxico("prueba.cpp"))
                {

                    while(!L.finArchivo())
                    {
                        L.nextToken();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
