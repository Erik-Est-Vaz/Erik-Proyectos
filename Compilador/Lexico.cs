using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Compilador
{
    public class Lexico : Token, IDisposable
    {
        protected StreamReader archivo;
        protected StreamWriter log;
        protected StreamWriter lenguajecs;
        protected int linea, caracter;
        const int F = -1;
        const int E = -2;
        int[,] TRAND =
        {
        //   WS   L    -    >    \    ;    ?    |    (    )    La
            {0  , 1  , 2  , 10 , 4  , 10 , 10 , 10 , 10 , 10 , 10}, // 1 
            {F  , 1  , F  , F  , F  , F  , F  , F  , F  , F  , F }, // 2
            {F  , F  , F  , 3  , F  , F  , F  , F  , F  , F ,  F }, // 3
            {F  , F  , F  , F  , F  , F  , F  , F  , F  , F ,  F }, // 4
            {F  , F  , F  , F  , F  , 5  , 6  , 7  , 8  , 9 ,  F }, // 5
            {F  , F  , F  , F  , F  , F  , F  , F  , F  , F ,  F }, // 6
            {F  , F  , F  , F  , F  , F  , F  , F  , F  , F ,  F }, // 7
            {F  , F  , F  , F  , F  , F  , F  , F  , F  , F ,  F }, // 8
            {F  , F  , F  , F  , F  , F  , F  , F  , F  , F ,  F }, // 9
            {F  , F  , F  , F  , F  , F  , F  , F  , F  , F ,  F }, // 10
            {F  , F  , F  , F  , F  , F  , F  , F  , F  , F ,  F }  // 11
        };
        public Lexico(string nombre = "Gramatica.txt") // Constructor
        {
            linea = caracter = 1;
            log = new StreamWriter(Path.GetFileNameWithoutExtension(nombre) + ".log");
            log.AutoFlush = true;

            log.WriteLine("Analizador Lexico");

            if (!File.Exists(nombre))
            {
                throw new Error("El archivo " + nombre + " no existe", log);
            }

            archivo = new StreamReader(nombre);

            lenguajecs = new StreamWriter("Lenguaje2.cpp");
        }
        public void Dispose() // Destructor
        {
            archivo.Close();
            log.Close();
            lenguajecs.Close();
        }
        int Columna(char c)
        {
            //   WS   L    -    >    \    ;    ?    |    (    )    La

            if (char.IsWhiteSpace(c))
                return 0;
            else if(char.IsLetter(c))
                return 1;
            else if (c == '-')
                return 2;
            else if (c == '>')
                return 3;
            else if (c == '\\')
                return 4;
            else if (c == ';')
                return 5;
            else if (c == '?')
                return 6;
            else if (c == '|')
                return 7;
            else if (c == '(')
                return 8;
            else if (c == ')')
                return 9;
            else
                return 10;
        }
        private void Clasificar(int Estado)
        {
            switch (Estado)
            {
                case 01:
                case 02: 
                case 04: 
                case 10: Clasificacion = Tipos.ST;              break;
                case 03: Clasificacion = Tipos.Flecha;          break;
                case 05: Clasificacion = Tipos.FinProduccion;   break;
                case 06: Clasificacion = Tipos.Epsilon;         break;
                case 07: Clasificacion = Tipos.OR;              break;
                case 08: Clasificacion = Tipos.Izquierdo;       break;
                case 09: Clasificacion = Tipos.Derecho;         break;
            }
        }
        public void nextToken()
        {
            char c;
            string buffer = "";
            int Estado = 0;
            while (Estado >= 0)
            {
                c = (char)archivo.Peek();

                Estado = TRAND[Estado, Columna(c)];
                Clasificar(Estado);

                if (Estado >= 0)
                {
                    if (Estado > 0)
                    {
                        buffer += c;
                    }
                    if (c == '\n')
                    {
                        linea++;
                    }
                    caracter++;
                    archivo.Read();
                }
            }

            Contenido = buffer;

            if(EsTipo(Contenido))
            {
                Clasificacion = Tipos.Tipo;
            }
            else if(char.IsUpper(Contenido[0]))
            {
                Clasificacion = Tipos.SNT;
            }

            log.WriteLine(Contenido + " = " + Clasificacion);

        }

        private bool EsTipo(string tipo)
        {

            switch(tipo)
            {
                case "Identificador": 
                case "Numero":
                case "FinSentencia":
                case "OpTermino":
                case "OpFactor":
                case "OpLogico":
                case "OpRelacional":
                case "OpTernario":
                case "IncTermino":
                case "IncFactor":
                case "Cadena":
                case "Inicio":
                case "Fin":
                case "Caracter":
                case "TipoDato":
                case "Ciclo":
                case "Condicion": return true;
            }

            return false;
        }


        public bool finArchivo()
        {
            return archivo.EndOfStream;
        }
    }
}