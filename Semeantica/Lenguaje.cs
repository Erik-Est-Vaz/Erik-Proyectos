using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
    1. Colocar el numero de linea en errores lexicos y sintaticos
    2. Cambiar la clase token por atributos publicos (get, set)
    3. Cambiar los constructoores de la clase lexico usando parametros por default

*/
namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> listaVariables;
        public Lenguaje()
        {
            listaVariables=new List<Variable>();
        }
        public Lenguaje(String nombre) : base(nombre)
        {
            listaVariables=new List<Variable>();
        }
        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
        }
        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            listaLibrerias();
            match(";");
            if(getContenido() == "using")
            {
                Librerias();
            }
        }
        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void listaLibrerias()
        {
            match(Tipos.Identificador);
            if(getContenido() == ".")
            {
                match(".");
                listaLibrerias();
            }
        }
        //Variables -> tipo_dato Lista_identificadores; Variables?
        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case "int": tipo = Variable.TipoDato.Int; break;
                case "float": tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }
        private void Variables()
        {
            //tipo_dato Lista_identificadores; Variables
            Variable.TipoDato tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
        }
        private void imprimeVariables()
        {
            foreach(Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre() + "  ()" + v.getTipo() + ") = " + v.getValor());
            }
        }
        //Lista_identificadores -> identificador (, Lista_identificadores)?
        private void listaIdentificadores(Variable.TipoDato t) 
        {
            listaVariables.Add(new Variable(getContenido(),t));
            match(Tipos.Identificador);
            if(getContenido() == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones()
        {
            match("{");
            if(getContenido()!="}")
            {
                listaInstrucciones();
            }
            match("{");
        }
        //listaIntrucciones -> Instruccion ListaInstrucciones?
        private void listaInstrucciones()
        {
            Instruccion();
            if(getContenido()!="}")
            {
                listaInstrucciones();
            }
        }
        //Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if(getContenido() == "Console")
            {
                Console();
            }
            else if(getContenido() == "if")
            {
                IF();
            }
            else if(getContenido() == "while")
            {
                While();
            }
            else if(getContenido() == "do")
            {
                Do();
            }
            else if(getContenido() == "for")
            {
                For();
            }
            if(getClasificacion()==Tipos.TipoDato)
            {
                Variables();
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
        //If -> if (Condicion) bloqueInstrucciones | instruccion
        //    (else bloqueInstrucciones | instruccion)?
        private void IF()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if(getContenido() == "else")
            {
                match("else");
                if(getContenido() == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
            }
        }
        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion()
        {
            Expresion();
            match(Tipos.OpRelacional);
            Expresion();
        }
        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if(getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }
        //Do -> do 
        //        bloqueInstrucciones | intruccion 
        //    while(Condicion);
        private void Do()
        {
            match("do");
            if(getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            match("while");
            match("(");
            Condicion();
            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) 
        //    BloqueInstrucciones | Intruccion 
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");   
            if(getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }
        //Incremento -> Identificador ++ | --
        private void Incremento()
        {
            match(Tipos.Identificador);
            if(getContenido() == "++")
            {
                match("++");
            }
            else if(getContenido() == "--")
            {
                match("--");
            }
        }
        //Console -> Console.(WriteLine|Write) (cadena)?; |
        //        Console.(Read | ReadLine) ();
        private void Console()
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine" || getContenido() == "Write")
            {
                match(getContenido());
                match("(");
                if (getClasificacion() == Tipos.Cadena)
                {
                    match(Tipos.Cadena);
                }
                match(")");
            }
            else 
            {
                if (getContenido() == "ReadLine")
                {
                    match("ReadLine");
                }
                else
                {
                    match("Read");
                }
                match("(");
                match(")");
            }
            match(";");
        }
        //Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            match("static");
            match("void");
            match("Main");
            match("(");
            match("string");
            match("[");
            match("]");
            match("args");
            match(")");
            bloqueInstrucciones();
        }
        //Expresion -> Termino MasTermino
        private void Expresion(){
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if(getClasificacion() == Tipos.OpTermino)
            {
                match(Tipos.OpTermino);
                Termino();
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if(getClasificacion()== Tipos.OpFactor)
            {
                match(Tipos.OpFactor);
                Factor();
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if(getClasificacion()== Tipos.Numero)
            {
                match(Tipos.Numero);
            }
            else if(getClasificacion()== Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if(getClasificacion()== Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
            }
        }

        //TODO: Agregar los métodos para parsear cada tipo de instrucción
        //Ejemplos:
        //private void ConsoleWriteLine()
        //private void ConsoleWrite()
        //private void ReadLine()
        //private void Read()
        //private void Asignacion()
        //private void IF()
        //private void While()
        //private void Do()
        //private void For()
        //private void Incremento()
        //private void Factor()
        //private void Expresion()
        //private void MasTermino
    }
}