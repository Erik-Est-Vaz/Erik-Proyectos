using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


/*

    a

*/

namespace Compilador
{
    public class Lenguaje : Sintaxis
    {

        public Lenguaje()
        {
            
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            
        }
        private void esqueleto(string nspace)
        {
            lenguajecs.WriteLine("using System;");
            lenguajecs.WriteLine("using System.Collections.Generic;");
            lenguajecs.WriteLine("");
            lenguajecs.WriteLine("namespace " + nspace);
            
            lenguajecs.WriteLine("{");
            lenguajecs.WriteLine("");





        }
        public void genera()
        {
            match("namespace");
            match(":");

            esqueleto(Contenido);
            
            match(Tipos.SNT);
            match(";");

            lenguajecs.WriteLine("    }");
            lenguajecs.WriteLine("}");
        }

/*
*/        

    }
}