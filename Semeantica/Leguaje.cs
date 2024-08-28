using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*
    1.Colocar el numero de linea  en errores lexivos y sintacticos
    2. Cambiar la clase token por atributos publicos(get,set)
    3.Cabiar los constructores de la clase lexocp usando parametros por default
*/

namespace Semantica
{
    public class Leguaje : Sintaxis
    {
        public Leguaje()
        {

        }
        public Leguaje(string nombre): base(nombre)
        {
            
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            Librerias();
            Variables();
            Main();
        }

        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if(getContenido()=="using")
            {
                Librerias();
            }
        }      

        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido()==".")
            {
                match(".");
                ListaLibrerias();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            match(Tipos.TipoDato);
            ListaIdentificadores();
            match(";");
            if (getContenido() == (Tipos.Identificador))
            {
                Variables();
            }
        }

        //ListaIdentificadores -> identificador (, ListaIdentificadores)?
        private void ListaIdentificadores()
        {
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores();
            }
        }

        //Main -> Main (Parametros)? { BloqueInstrucciones }
        private void Main()
        {
            match("Main");
            Parametros();
            match("{");
            BloqueInstrucciones();
            match("}");
        }
        
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void BloqueInstrucciones()
        {
            match("{");
            if (getContenido()!= "}")
            {
                ListaInstrucciones();
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones()
        {
            Instruccion();
            if (getContenido()!= "}")
            {
                match(";");
                ListaInstrucciones();
            }
        }

        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                Console();
            }
            else if (getContenido() == "If")
            {
                If();
            }
            else if (getContenido() == "While")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                DoWhile();
            }
            else if (getContenido() == "For")
            {
                For();
            }
            else
            {
                Asignacion();
            }
        }
        //Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            match(Tipos.Identificador);
            match("=");
            Expresion();
            match(";");
        }
    }
}