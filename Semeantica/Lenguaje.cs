using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


/*

    1. Usar find en lugar del for each :)
    2. Valiar que no existan varibles duplicadas ;)
    3. Validar que existan las variables en las expressions matematicas :)
       Asignacion
    4. Asinar una expresion matematica a la variable al momento de declararla :)
       verificando la semantica
    5. Validar que en el ReadLine se capturen solo numeros (Excepcion)
    6. listaConcatenacion: 30, 14, 15 ,12, 0
    7. Quitar comillas y considerar el Write
    8. Emular el for -- 15 puntos
    9. Emular el while -- 15 puntos

*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private Stack<float> S;

        //Modificación 1
        private Variable.TipoDato tipoDatoExpresion;

        public Lenguaje()
        {
            //Modificación 2
            log.WriteLine("Analisis Semantico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }


        public Lenguaje(string nombre) : base(nombre)
        {
            //Modificación 2.1
            log.WriteLine("Analisis Semantico");
            listaVariables = new List<Variable>();
            S = new Stack<float>();
        }


        // Program -> Librerias? Variables? Main
        public void Programa()
        {
            if (Contenido == "using")
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
            ListaLibrerias();
            match(";");
            if (Contenido == "using")
            {
                Librerias();
            }
        }


        //ListaLibrerias -> identificador (.ListaLibrerias)?
        private void ListaLibrerias()
        {
            match(Tipos.Identificador);
            if (Contenido == ".")
            {
                match(".");
                ListaLibrerias();
            }
        }


        Variable.TipoDato getTipo(string TipoDato)
        {
            Variable.TipoDato tipo = Variable.TipoDato.Char;
            switch (TipoDato)
            {
                case ("int"): tipo = Variable.TipoDato.Int; break;
                case ("float"): tipo = Variable.TipoDato.Float; break;
            }
            return tipo;
        }


        //Variables -> tipo_dato Lista_identificadores; Variables?
        private string Variables(bool ejecutar)
        {
            //listaVariables.Add(new Variable("efnhjesflo4hesf", Variable.TipoDato.Char));
            Variable.TipoDato tipo = getTipo(Contenido);
            match(Tipos.TipoDato);

            string var = Contenido;

            listaIdentificadores(tipo,ejecutar);
            //match(";");
            return var;
                
        }


        private void imprimeVariables()
        {
            foreach (Variable v in listaVariables)
            {
                log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
            }
        }



        // -->  Aqui hay que hacer una modificación
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t, bool ejecutar)
        {
            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == ""; });
            if(ejecutar)
            {
                v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido && x.getTipo() == t; });
            }
            else
                v = null;
                
            if (v != null)
            {
                throw new Exception("Error Semantico: en " + linea +" Variable duplicada " + Contenido);
            }
            else
            {
                listaVariables.Add(new Variable(Contenido, t));

                string var = Contenido;

                match(Tipos.Identificador);
                if(Contenido != "," && Contenido != ";")
                {
                    Asignacion(ejecutar,var);
                }
                else if(Contenido == ",")
                {
                    match(",");
                    listaIdentificadores(t,true);
                }
            }
            
        }


        //Modificación 3
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones(bool ejecutar)
        {
            match("{");
            if (Contenido != "}")
            {
                listaInstrucciones(ejecutar);
            }
            match("}");
        }

        //Modificación 4
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaInstrucciones(bool ejecutar)
        {
            Instruccion(ejecutar);
            if (Contenido != "}")
            {
                listaInstrucciones(ejecutar);
            }
        }

        //Modificación 5
        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion(bool ejecutar)
        {
            if (Contenido == "Console")
            {
                console(ejecutar);
            }
            else if (Contenido == "if")
            {
                If(ejecutar);
            }
            else if (Contenido == "while")
            {
                While(ejecutar);
            }
            else if (Contenido == "do")
            {
                Do(ejecutar);
            }
            else if (Contenido == "for")
            {
                For(ejecutar);
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables(true);
                match(";");
            }
            else if (Contenido != "}")
            {
                match(Tipos.Identificador);
                Asignacion(ejecutar, Contenido);
                match(";");
            }
        }

        //Modificación 6
        // Asignacion -> Identificador = Expresion;
        private void Asignacion(bool ejecutar, string variable)
        {
            //string variable = Contenido;
            //match(Tipos.Identificador);
            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == variable; });
            float nuevoValor = v.getValor();

            tipoDatoExpresion = Variable.TipoDato.Char;
            
            if (Contenido == "=")
            {
                match("=");
                if (Contenido == "Console")
                {
                    match("Console");
                    match(".");
                    if (Contenido == "Read")
                    {
                        match("Read");
                        if (ejecutar)
                        {
                            float valor = Console.Read();
                        }
                        // 8
                    }
                    else
                    {
                        match("ReadLine");
                        nuevoValor = float.Parse("" + Console.ReadLine());
                        // 8
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    Expresion();
                    nuevoValor = S.Pop();
                }
            }
            else if (Contenido == "++")
            {
                //Console.WriteLine(Contenido + "esto tiene contenido");
                match("++");
                nuevoValor++;
            }
            else if (Contenido == "--")
            {
                match("--");
                nuevoValor--;
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                nuevoValor += S.Pop();
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                nuevoValor -= S.Pop();
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
                nuevoValor *= S.Pop();
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
                nuevoValor /= S.Pop();
            }
            else
            {
                match("%=");
                Expresion();
                nuevoValor %= S.Pop();
            }
            // match(";");
            if (analisisSemantico(v, nuevoValor))
            {
                //Console.WriteLine("estoy asignando nuevovalor");
                if (ejecutar)
                    v.setValor(nuevoValor);
                    //Console.WriteLine(v.getNombre() + " " + v.getValor());
                //Console.WriteLine("**********************************************" + variable + " = " + nuevoValor);
            }
            else
            {
                //Modificar tipoDatoExpresion
                throw new Error("Semantico, no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.getTipo(), log);
            }
            
        }


        //Modificación 7
        //Aquí mi muy queridisimo profesor hizo su propia funcion d elimite de variables la cual considero mejor que la nuestra
        private Variable.TipoDato valorToTipo(float valor)
        {
            if (valor % 1 != 0) //Aqui se verifica que no tenga punto decimal
            {
                return Variable.TipoDato.Float;
            }
            else if (valor <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (valor <= 65535)
            {
                return Variable.TipoDato.Int;
            }

            return Variable.TipoDato.Float;
        }


        bool analisisSemantico(Variable v, float valor)
        {
            //Sigue marcando error
            if (tipoDatoExpresion > v.getTipo())
            {
                return false;
            }
            else if (valor % 1 == 0)
            {
                if (v.getTipo() == Variable.TipoDato.Char)
                {
                    if (valor <= 255)
                        return true;
                }
                else if (v.getTipo() == Variable.TipoDato.Int)
                {
                    if (valor <= 65535)
                        return true;
                }
                return false;
            }
            else
            {
                if (v.getTipo() == Variable.TipoDato.Char || v.getTipo() == Variable.TipoDato.Int)
                    return false;
            }
            return true;
        }


        private void limiteVariables(float stack, string variable)
        {
            foreach (Variable v in listaVariables)
            {
                if (v.getNombre() == variable)
                {
                    String s = v.getTipo().ToString();
                    switch (s)
                    {
                        case ("Int"):
                            if (Math.Abs(stack) > 65535)
                            {
                                throw new Error("Linea " + linea + " Semantico: La variable " + variable + " de tipo (" + s + ") excedio su limite de memoria", log);
                            }
                            v.setValor(stack); break;
                        case ("Float"): v.setValor(stack); break;
                        default:
                            if (Math.Abs(stack) > 255)
                            {
                                throw new Error("Linea " + linea + " Semantico: La variable " + variable + " de tipo (" + s + ") excedio su limite de memoria", log);
                            }
                            v.setValor(stack); break;
                    }
                }
            }
        }

        //If -> if (Condicion) bloqueInstrucciones | instruccion (else bloqueInstrucciones | instruccion)?
        private void If(bool ejecutar)
        {
            match("if");
            match("(");
            bool resultado = Condicion();
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones(resultado && ejecutar);
            }
            else
            {
                Instruccion(resultado && ejecutar);
            }
            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    bloqueInstrucciones(!resultado && ejecutar);
                }
                else
                {
                    Instruccion(!resultado && ejecutar);
                }
            }
        }

        //Condicion -> Expresion operadorRelacional Expresion
        private bool Condicion()
        {
            Expresion(); //Expresión número 1
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); //Expresión número 2

            float R2 = S.Pop();
            float R1 = S.Pop();
            
            switch (operador)
            {
                case ">" : return R1 > R2;
                case ">=": return R1 >= R2;
                case "<" : return R1 < R2;
                case "<=": return R1 <= R2;
                case "==": return R1 == R2;
                default  : return R1 != R2;
            }
        }

        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While(bool ejecutar)
        {
            match("while");
            match("(");
            Condicion();
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones(ejecutar);
            }
            else
            {
                Instruccion(ejecutar);
            }
        }

        //Do -> do bloqueInstrucciones | intruccion while(Condicion);
        private void Do(bool ejecutar)
        {
            int cTemp = caracter -3;
            int lTemp = linea;
            bool resultado = false;

            do
            {
                match("do");
                if (Contenido == "{")
                {
                    bloqueInstrucciones(ejecutar);
                }
                else
                {
                    Instruccion(ejecutar);
                }
                match("while");
                match("(");
                resultado = Condicion() && ejecutar;
                match(")");
                match(";");

                if(resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            }while(resultado);
            
        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Intruccion
        private void For(bool ejecutar)
        {
            
            int cTemp = caracter - 4;
            int lTemp = linea;
            bool resultado = true;
            do
            {
                match("for");
                match("(");
                
                string var = "";

                if (Clasificacion == Tipos.TipoDato)
                {
                    var = Variables(resultado && ejecutar);
                }
                else /*if (Clasificacion == Tipos.Identificador)*/
                {
                    var = Contenido;
                    match(Tipos.Identificador);
                    Asignacion(resultado && ejecutar, Contenido);
                }

                ejecutar = false;

                match(";");

                resultado = Condicion();

                if(!resultado)
                {
                    ejecutar = false;
                }

                match(";");

                match(Tipos.Identificador);
                Asignacion(resultado,var);
                
                match(")");
                if (Contenido == "{")
                {
                    bloqueInstrucciones(resultado);
                }
                else
                {
                    Instruccion(resultado);
                }

                if(resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }

            }
            while(resultado);
        }

        /*Incremento -> Identificador ++ | --
        private void Incremento()
        {
            if (Contenido == "++")
            {
                match("++");
            }
            else if (Contenido == "--")
            {
                match("--");
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
            }
            else if (Contenido == "*=")
            {
                match("*=");
                Expresion();
            }
            else if (Contenido == "/=")
            {
                match("/=");
                Expresion();
            }
            else if (Contenido == "%=")
            {
                match("%=");
                Expresion();
            }
        }
        */

        //Console -> Console.(WriteLine|Write) (cadena?); | Console.(Read | ReadLine) ();
        private void console(bool ejecutar)
        {
            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
            }
            else
            {
                match("Write");
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {
                if (ejecutar)
                {
                    Console.WriteLine(Contenido);
                }
                // Considerar el Write
                // Quitar las comillas
                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    listaConcatenacion();
                }
            }
            else if(/*xd*/true)
            {

            }
            match(")");
            match(";");
        }


        string listaConcatenacion()
        {
            match("+");
            match(Tipos.Identificador); // Validar que exista la variable
            if(Contenido == "+")
            {
                listaConcatenacion();
            }
            return "";
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
            bloqueInstrucciones(true);
        }


        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }


        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (Clasificacion == Tipos.OpTermino)
            {
                string operador = Contenido;
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


        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        

        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (Clasificacion == Tipos.OpFactor)
            {
                string operador = Contenido;
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
        

        private void imprimeStack()
        {
            log.WriteLine("ESTO DICE QUE TIENE EL STACK");
            foreach (float e in S.Reverse())
            {
                log.Write(e + " ");
            }
            log.WriteLine();
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                S.Push(float.Parse(Contenido));
                if (tipoDatoExpresion < valorToTipo(float.Parse(Contenido)))
                {
                    tipoDatoExpresion = valorToTipo(float.Parse(Contenido));
                }
                match(Tipos.Numero);
            }
            else if (Clasificacion == Tipos.Identificador)
            {
                
                string variable = Contenido;

                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido; });

                if(v != null)
                {
                    S.Push(v.getValor());
                    if (tipoDatoExpresion < v.getTipo())
                    {
                        tipoDatoExpresion = v.getTipo();
                    }
                    match(Tipos.Identificador);
                }
                else
                {
                    throw new Error("Linea " + linea + " Semantico: La variable " + variable + " no existe", log);
                }
                
            }
            else
            {
                bool huboCast = false;
                Variable.TipoDato aCastear = Variable.TipoDato.Char;
                match("(");
                if (Clasificacion == Tipos.TipoDato)
                {
                    huboCast = true;
                    aCastear = getTipo(Contenido);
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion();
                match(")");
                if (huboCast && aCastear != Variable.TipoDato.Float)
                {
                    tipoDatoExpresion = aCastear;
                    float valor = S.Pop();
                    if (aCastear == Variable.TipoDato.Char)
                    {
                        valor %= 256;
                    }
                    else
                    {
                        valor %= 65536;
                    }
                    S.Push(valor);
                }
            }
        }
    }
}