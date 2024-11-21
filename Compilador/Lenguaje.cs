using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;


/*

    1. Hacer que solo la primera producción sea publica, el resto es privada. --> LISTO
    2. Implementar la cerradura Epsilon. --> LIST0
    3. Implementar el operador OR. --> LISTO
    4. Implementar un sistema de identación automatica. --> LISTO              

*/

namespace Compilador
{
    public class Lenguaje : Sintaxis
    {

        bool primera = true;
        int cont = 0;

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
            imprime("", cont, true);
            cont--;
            imprime("}", cont, true);

            imprime("public Lenguaje(string nombre) : base(nombre)", cont, true);
            imprime("{", cont, true);
            cont++;
            imprime("", cont, true);
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
            cont--;
            imprime("}", cont, true);
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
            else if (Clasificacion == Tipos.SNT)
            {
                imprime("private void " + Contenido + "()", cont, true);
                imprime("{", cont, true);
                cont++;
            }
            match(Tipos.SNT);
            match(Tipos.Flecha);
            conjuntoTokens(false);
            match(Tipos.FinProduccion);

            imprime("", cont, true);

            cont--;
            imprime("}", cont, true);

            if (Clasificacion == Tipos.SNT)
            {
                producciones();
            }

        }

        private void conjuntoTokens(bool enOR)
        {
            Console.WriteLine("Cont -> " + Contenido + "  " + Clasificacion);

            if (Clasificacion == Tipos.Izquierdo || enOR == true)
            {
                if (enOR == false)
                {
                    match(Tipos.Izquierdo);
                    imprime("if (", cont, false);
                }
                else if (Clasificacion == Tipos.Izquierdo)
                {
                    match(Tipos.Izquierdo);
                    imprime("if (", 0, false);
                }  
                else
                {
                    imprime("if (", 0, false);
                }

                if (Clasificacion == Tipos.SNT || Clasificacion == Tipos.ST || Clasificacion == Tipos.Tipo)
                {
                    if(Clasificacion ==Tipos.SNT)
                    {
                        imprime("Clasificacion == Tipos." + Contenido + ")", 0, true);
                        imprime("{", cont, true);
                        cont++;
                        imprime(Contenido + "();", cont, true);
                        match(Tipos.SNT);
                    }
                    else if (Clasificacion ==Tipos.ST)
                    {
                        imprime("Contenido == \"" + Contenido + "\")", 0, true);
                        imprime("{", cont, true);
                        cont++;
                        imprime("match(\"" + Contenido + "\");", cont, true);
                        match(Tipos.ST);
                    }
                    else  if (Clasificacion ==Tipos.Tipo)
                    {
                        imprime("Clasificacion == Tipos." + Contenido + ")", 0, true);
                        imprime("{", cont, true);
                        cont++;
                        imprime("match(Tipos." + Contenido + ");", cont, true);
                        match(Tipos.Tipo);
                    }

                    if (Clasificacion == Tipos.OR)
                    {
                        match(Tipos.OR);
                        Console.WriteLine("matchee |");

                        if (Clasificacion == Tipos.SNT || Clasificacion == Tipos.ST || Clasificacion == Tipos.Tipo || Clasificacion == Tipos.Izquierdo)
                        {
                            cont--;
                            imprime("}", cont, true);
                            imprime("else ", cont, false);
                            

                            if(Clasificacion == Tipos.Izquierdo)
                            {
                                conjuntoTokens(true);
                            }
                            else
                            {
                                conjuntoTokens(true);
                            }

                        }
                        else
                        {
                            throw new Error(" Semantico, Linea " + linea + ": Falta de sentencia", log);
                        }
                    }

                }
            }
            else if (Clasificacion == Tipos.ST)
            {
                imprime("match(\"" + Contenido + "\");", cont, true);
                match(Tipos.ST);
            }
            else if (Clasificacion == Tipos.Tipo)
            {
                imprime("match(Tipos." + Contenido + ");", cont, true);
                match(Tipos.Tipo);
            }
            else if (Clasificacion == Tipos.SNT)
            {
                imprime(Contenido + "();", cont, true);
                match(Tipos.SNT);
            }
            else if (Clasificacion == Tipos.Derecho)
            {
                cont--;
                imprime("}", cont, true);

                match(Tipos.Derecho);

                if (Clasificacion == Tipos.Epsilon)
                {

                    match(Tipos.Epsilon);

                }
                else
                {
                    imprime("else", cont, true);
                    imprime("{", cont, true);
                    cont++;
                    imprime("throw new Error(\" Sintaxis, Linea \" + linea + ,\": Se esperaba un SNT\", log);", cont, true);
                    cont--;
                    imprime("}", cont, true);
                }
            }
            else if (Clasificacion == Tipos.OR)
            {
                match(Tipos.OR);

                if (Clasificacion == Tipos.SNT || Clasificacion == Tipos.ST || Clasificacion == Tipos.Tipo)
                {
                    cont--;
                    imprime("}", cont, true);
                    imprime("else ", cont, false);

                    conjuntoTokens(true);

                }
                else
                {
                    throw new Error(" Semantico, Linea " + linea + ": Falta de sentencia", log);
                }
            }

            if (Clasificacion != Tipos.FinProduccion)
            {
                conjuntoTokens(false);
            }

        }

        private void imprime(string text, int cont, bool esWiriteLine)
        {

            for (int i = 0; i < cont; i++)
            {
                lenguajecs.Write("\t");
            }

            if (esWiriteLine)
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