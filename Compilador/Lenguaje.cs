using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


/*

    1. Hacer que solo la primera producción sea publica, el resto es privada. --> LISTO
    2. Implementar la cerradura Epsilon.
    3. Implementar el operador OR.
    4. Implementar un sistema de identación automatica. --> LISTO

*/

namespace Compilador
{
    public class Lenguaje : Sintaxis
    {

        bool primera = true;
        int cont  = 0;

        public Lenguaje()
        {
            
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            
        }
        private void esqueleto(string nspace)
        {
            imprime("using System;", cont, true);
            imprime("using System.Collections.Generic;", cont, true);
            imprime("using System.Linq;", cont, true);
            imprime("using System.Net.Http.Headers;", cont, true);
            imprime("using System.Reflection.Metadata.Ecma335;", cont, true);
            imprime("using System.Runtime.InteropServices;", cont, true);
            imprime("using System.Threading.Tasks;", cont, true);
            imprime("\nnamespace " + nspace, cont, true);
            imprime("{", cont, true);
            cont++;
            imprime("public class Lenguaje : Sintaxis", cont, true);
            imprime("{", cont, true);
            cont++;
            imprime("public Lenguaje()", cont, true);
            imprime("{", cont, true);
            cont++;
            cont--;
            imprime("}", cont, true);
            
            imprime("public Lenguaje(string nombre) : base(nombre)", cont, true);
            imprime("{", cont, true);
            cont++;
            cont--;
            imprime("}", cont, true);
            
            imprime(" ", cont, true);
        }


        public void genera()
        {
            
            match("namespace");
            match(":");

            esqueleto(Contenido);

            match(Tipos.SNT);
            match(";");

            producciones();
            
            cont--;
            imprime("}", cont, true);
            /*cont--;
            imprime("}", cont, true);*/
        }

        private void producciones()
        {
            
            
            if (Clasificacion == Tipos.SNT && primera == true)
            {
                imprime("public void " + Contenido + "()", cont, true);
                imprime("{", cont, true);
                cont++;

                primera = false;
            }
            else if(Clasificacion == Tipos.SNT )
            {
                imprime("private void " + Contenido + "()", cont, true);
                imprime("{", cont, true);
                cont++;
            }
            match(Tipos.SNT);
            match(Tipos.Flecha);
            conjuntoTokens();
            match(Tipos.FinProduccion);
            
            cont--;
            imprime("}", cont, true);

            if (Clasificacion == Tipos.SNT)
            {
                producciones();
            }

            /*match(Tipos.SNT);
            match(Tipos.Flecha);

            if(Clasificacion == Tipos.SNT && primera == true)
            {
                lenguajecs.WriteLine("        public void " + Contenido + "()");
                lenguajecs.WriteLine("        {");

                primera = false;

            }
            else if(Clasificacion == Tipos.SNT )
            {
                lenguajecs.WriteLine("        private void " + Contenido + "()");
                lenguajecs.WriteLine("        {");
            }

            match(Tipos.SNT);
            
            lenguajecs.WriteLine("        }");

            if(Clasificacion == Tipos.FinProduccion)
            {
                match(Tipos.FinProduccion);
            }
            else if(Clasificacion == Tipos.SNT)
            {
                producciones();
            }*/
            
        }

        private void conjuntoTokens()
        {
            if(Clasificacion == Tipos.SNT)
            {
                imprime(Contenido + "();", cont, true);
                match(Tipos.SNT);
            }
            else if(Clasificacion == Tipos.ST)
            {
                imprime("match(\"" + Contenido + "\");", cont, true);
                match(Tipos.ST);
            }
            else if(Clasificacion == Tipos.Tipo)
            {
                imprime("match(Tipos." + Contenido + ");", cont, true);
                match(Tipos.Tipo);
            }
            else if (Clasificacion == Tipos.Izquierdo)
            {
                match(Tipos.Izquierdo);

                imprime("if (", cont, true);

                if (Clasificacion == Tipos.ST)
                {
                    imprime("Contenido() == \"" + Contenido + "\")", cont, true);
                    imprime("{", cont, true);
                    cont++;
                    imprime("match(\"" + Contenido + "\");", cont, true);
                    match(Tipos.ST);
                }
                else if (Clasificacion == Tipos.Tipo)
                {
                    imprime("Clasificacion == Tipos." + Contenido + ")", cont, true);
                    imprime("{", cont, true);
                    cont++;
                    imprime("match(Tipos." + Contenido + ");", cont, true);
                    match(Tipos.Tipo);
                }
                match(Tipos.Derecho);
                
                cont--;
                imprime("}", cont, true);
            }

            if(Clasificacion != Tipos.FinProduccion)
            {
                conjuntoTokens();
            }
        }

        private void imprime(string text, int cont, bool esWiriteLine)
        {

            for(int i = 0; i < cont; i++)
            {
                lenguajecs.Write("\t");
            }

            if(esWiriteLine)
            {
                lenguajecs.WriteLine(text);
            }
            else
            {
                lenguajecs.Write(text);
            }

        }



    }
}