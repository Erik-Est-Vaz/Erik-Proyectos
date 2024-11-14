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
            lenguajecs.WriteLine("using System.Linq;");
            lenguajecs.WriteLine("using System.Net.Http.Headers;");
            lenguajecs.WriteLine("using System.Reflection.Metadata.Ecma335;");
            lenguajecs.WriteLine("using System.Runtime.InteropServices;");
            lenguajecs.WriteLine("using System.Threading.Tasks;");
            lenguajecs.WriteLine("\nnamespace "+nspace);
            lenguajecs.WriteLine("{");
            lenguajecs.WriteLine("    public class Lenguaje : Sintaxis");
            lenguajecs.WriteLine("    {");
            lenguajecs.WriteLine("        public Lenguaje()");
            lenguajecs.WriteLine("        {");
            lenguajecs.WriteLine("        }");
            lenguajecs.WriteLine("        public Lenguaje(string nombre) : base(nombre)");
            lenguajecs.WriteLine("        {");
            lenguajecs.WriteLine("        }");
            lenguajecs.WriteLine("        public void ");




        }
        public void genera()
        {
            Console.WriteLine("estoy en genera");
            match("namespace");
            Console.WriteLine("DESPUES DEL NAME SPACE");
            match(":");
            Console.WriteLine("DESPUES DEL ':'");

            Console.WriteLine("ANTES DEL ESQUELETO");

            esqueleto(Contenido);

            Console.WriteLine("DESPUES DEL ESQUELETO");

            match(Tipos.SNT);
            match(";");
            lenguajecs.WriteLine("    }");
            lenguajecs.WriteLine("}");
        }

/*
*/        

    }
}