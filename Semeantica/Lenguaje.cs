using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


/*
    1. Colocar la linea en los errores lexicos y sintaxicos
    --- ESTA NO --- 2. Cambiar clase token por atributos publicos usando get y set
    3. Cambiar los contructores de la calse lexico usando parametros por default
    4.Errror Semantico,camb
*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List <Variable> listaVariables;
        private Stack <float> S;
        public Lenguaje()
        {
            listaVariables = new List <Variable>();
            S = new Stack<float>();
        }

        public Lenguaje(string nombre) : base(nombre)
        {
            listaVariables = new List <Variable>();
            S = new Stack <float>();
        }

        // Program -> Librerias? Variables? Main
        public void Programa()
        {

            if (getContenido() == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
            //:D
        }

        //Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            ListaLibrerias();
            match(";");
            if (getContenido() == "using")
            {
                Librerias();
            }
        }

        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }

        //--------------------------------------------------------------------------------------------

        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch(TipoDato)
            {
                case("int"):tipo = Variable.TipoDato.Int; break;
                case("float"):tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
        }

        private void imprimeVariables()
        {
            foreach(Variable v in listaVariables)
            {
            log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
            }
        }

        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(getContenido(),t));
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }

        //BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                listaInstrucciones();
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaInstrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                listaInstrucciones();
            }
        }

        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                Console();
            }
            else if (getContenido() == "if")
            {
                If();
            }
            else if (getContenido() == "while")
            {
                While();
            }
            else if (getContenido() == "do")
            {
                Do();
            }
            else if (getContenido() == "for")
            {
                For();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
            }
        }

        // Asignacion -> Identificador = Expresion;
        
        private void Asignacion()
        {
            string variable = getContenido();
            match(Tipos.Identificador);
            if(getContenido() != "=")
            {
                Incremento();
            }
            else
            {
                match("=");
                Expresion();
                float stack = S.Pop();
                limiteVariables(stack, variable);
                imprimeStack();
                log.WriteLine(variable + " = " + stack);
            }
            match(";");
        }
    
        private void limiteVariables(float stack, string variable)
        {
            foreach(Variable v in listaVariables)
            {
                if(v.getNombre() == variable)
                {
                    String s =  v.getTipo().ToString();
                    switch(s)
                    {
                        case("Int"): if(Math.Abs(stack) > 65535)
                        {
                            throw new Error("Semantico: La variable " + variable + " de tipo (" + s + ") excedio su limite de memoria",log);
                        }
                        v.setValor(stack);break;
                        case("Float"):v.setValor(stack);break;
                        default: if(Math.Abs(stack) > 255){ 
                            throw new Error("Semantico: La variable " + variable + " de tipo (" + s + ") excedio su limite de memoria",log);
                        }
                        v.setValor(stack);break;
                    }
                }
            }
        }

        //If -> if (Condicion) bloqueInstrucciones | instruccion (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            match("if");
            match("(");
            Condicion();
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            if (getContenido() == "else")
            {
                match("else");
                if (getContenido() == "{")
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
            if (getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
        }

        //Do -> do bloqueInstrucciones | intruccion while(Condicion);
        private void Do()
        {
            match("do");
            if (getContenido() == "{")
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

        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Intruccion
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            Condicion();
            match(";");
            Incremento();
            match(")");
            if (getContenido() == "{")
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
            if (getContenido() == "++")
            {
                match("++");
            }
            else if (getContenido() == "--")
            {
                match("--");
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();
            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion();
            }
        }

        //Console -> Console.(WriteLine|Write) (cadena?); | Console.(Read | ReadLine) ();
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
                match(";");
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

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }

        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }

        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OpTermino)
            {
                string operador = getContenido();
                match(Tipos.OpTermino);
                Termino();
                float R1 = S.Pop();
                float R2 = S.Pop();
                switch (operador)
                {
                    case "+": S.Push(R2 + R1); break;
                    case "-": S.Push(R2 - R1); break;
                }
    
            }
        }

        private void imprimeStack()
        {
            log.WriteLine("ESTO DICE QUE TIENE EL STACK");
            foreach(float e in S.Reverse())
            {
                log.Write(e + " ");
            }
            log.WriteLine();
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                S.Push(float.Parse(getContenido()));
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                if(getClasificacion() == Tipos.TipoDato)
                {
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
            }
        }

        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OpFactor)
            {
                string operador = getContenido();
                match(Tipos.OpFactor);
                Factor();
                float R1 = S.Pop();
                float R2 = S.Pop();
                switch (operador)
                {
                    case "*": S.Push(R2 * R1); break;
                    case "/": S.Push(R2 / R1); break;
                    case "%": S.Push(R2 % R1); break;
                }
                  
            }
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

    }

}