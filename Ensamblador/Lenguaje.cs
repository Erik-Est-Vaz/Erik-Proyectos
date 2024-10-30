using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


/*

    El código analiza un archivo en c++ y genera código en ensamblador en MASM, NASM, etc... ¡Excepto EMU 8086!

    1. Completar la asignación ++, +=, -=, etc LISTO
    2. Console.Write y Console.WriteLine
    3. Console.Read y Console.ReadLine
    4. Considerar el else en el if LSITO
    5. Programar el while
    6. Programar el for
    7. Se feliz :) PENDIENTE
    8. Tomar en cuenta todas las condiciones LISTO

*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        //private Stack<float> S;

        //Modificación 1
        private Variable.TipoDato tipoDatoExpresion;
        private int cIFs, cDOs, cWhiles, cFors;

        public Lenguaje()
        {
            log.WriteLine("Analisis Semantico");
            asm.WriteLine("Analisis Semantico");
            listaVariables = new List<Variable>();
            cIFs = 1;
            cDOs = 1;
            cWhiles = 1;
            cFors = 1;
        }


        public Lenguaje(string nombre) : base(nombre)
        {
            //Modificación 2.1
            log.WriteLine("Analisis Semantico");
            asm.WriteLine("Analisis Semantico");
            listaVariables = new List<Variable>();
            cIFs = 1;
            cDOs = 1;
            cWhiles = 1;
            cFors = 1;
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

            listaIdentificadores(tipo);
            //match(";");
            return var;

        }


        private void imprimeVariables()
        {
            // log.WriteLine("Lista de variables");

            char comillas = '"';

            asm.WriteLine("\nsegment .data");
            foreach (Variable v in listaVariables)
            {
                if (v.getTipo() == Variable.TipoDato.Char)
                {
                    if (v.getSmg() != "")
                        if (v.getEsWrite() == true)
                            asm.WriteLine("\t" + v.getNombre() + " db " + v.getSmg() + ", 0");
                        else
                            asm.WriteLine("\t" + v.getNombre() + " db " + v.getSmg() + ", 13, 0");
                }
            }

            
            foreach (Variable v in listaVariables)
            {
                if (v.getTipo() == Variable.TipoDato.Char)
                {
                    if (v.getSmg() == "")
                        asm.WriteLine("\t" + v.getNombre() + " db 0");
                }
                else if (v.getTipo() == Variable.TipoDato.Int)
                {
                    asm.WriteLine("\t" + v.getNombre() + " dd 0");
                }
                else
                {
                    asm.WriteLine("\t" + v.getNombre() + " dq 0 ");
                }
            }
        }



        // -->  Aqui hay que hacer una modificación
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == ""; });
            v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido && x.getTipo() == t; });

            if (v != null)
            {
                throw new Error(" Semantico, Linea " + linea + ": en " + linea + " Variable duplicada " + Contenido, log);
            }
            else
            {
                listaVariables.Add(new Variable(Contenido, t));

                string var = Contenido;

                if (Contenido != ";")
                {
                    match(Tipos.Identificador);
                    if (Contenido != "," && Contenido != ";")
                    {
                        Asignacion(var);
                        if (Contenido == ",")
                        {
                            match(",");
                            listaIdentificadores(t);
                        }
                        else if (Contenido == ";")
                        {

                        }
                    }
                    else if (Contenido == ",")
                    {
                        match(",");
                        listaIdentificadores(t);
                    }
                }

            }

        }


        //Modificación 3
        //BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones()
        {
            match("{");
            if (Contenido != "}")
            {
                listaInstrucciones();
            }
            match("}");
        }

        //Modificación 4
        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaInstrucciones()
        {
            Instruccion();
            if (Contenido != "}")
            {
                listaInstrucciones();
            }
        }

        //Modificación 5
        //Instruccion -> Console | If | While | do | For | Asignacion
        private void Instruccion()
        {
            if (Contenido == "Console")
            {
                console();
            }
            else if (Contenido == "if")
            {
                If();
            }
            else if (Contenido == "while")
            {
                While();
            }
            else if (Contenido == "do")
            {
                Do();
            }
            else if (Contenido == "for")
            {
                For();
            }
            else if (Clasificacion == Tipos.TipoDato)
            {
                Variables(true);
                match(";");
            }
            else/* (Clasificacion == Tipos.Identificador)*/
            {
                //Console.WriteLine("Estoy en identificador");
                string var = Contenido;
                match(Tipos.Identificador);
                Asignacion(var);
                match(";");
            }
        }

        //Modificación 6
        // Asignacion -> Identificador = Expresion;
        private float Asignacion(string variable)
        {
            //string variable = Contenido;
            //match(Tipos.Identificador);

            float nuevoValor = 0;

            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == variable; });

            if (v != null)
            {
                nuevoValor = v.getValor();
            }
            else
            {
                throw new Error(" Semantico, Linea " + linea + ": La variable " + variable + " no existe", log);
            }


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
                        //if (ejecutar)
                        {
                            float valor = Console.Read();
                        }
                        // 8
                    }
                    else
                    {
                        match("ReadLine");
                        try
                        {
                            nuevoValor = float.Parse("" + Console.ReadLine());
                        }
                        catch
                        {
                            throw new Error(" Semantico, Linea " + linea + ": no es un valor entero el de entrada", log);
                        }
                        // 8
                    }
                    match("(");
                    match(")");
                }
                else
                {
                    Expresion();
                    //nuevoValor = S.Pop();
                }
            }
            else if (Contenido == "++")
            {
                match("++");
                nuevoValor++;
                asm.WriteLine("\tmov eax, [" + variable + "]");
                asm.WriteLine("\tmov ebx, 1");
                asm.WriteLine("\tadd eax, ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "--")
            {
                match("--");
                nuevoValor--;
                asm.WriteLine("\tmov eax, [" + variable + "]");
                asm.WriteLine("\tmov ebx, 1");
                asm.WriteLine("\tsub eax, ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "+=")
            {
                match("+=");
                Expresion();
                asm.WriteLine("\tmov eax, [" + variable + "]");
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tadd eax, ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "-=")
            {
                match("-=");
                Expresion();
                asm.WriteLine("\tmov eax, [" + variable + "]");
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tsub eax, ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "*=")
            {
                asm.WriteLine("\tmov eax, [" + variable + "]");
                asm.WriteLine("\tpush eax");
                match("*=");
                Expresion();
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tmul ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "/=")
            {
                asm.WriteLine("\tmov eax, [" + variable + "]");
                asm.WriteLine("\tpush eax");
                match("/=");
                Expresion();
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tdiv ebx");
                asm.WriteLine("\tmov dword [" + variable + "], eax");
            }
            else if (Contenido == "%=")
            {
                asm.WriteLine("\tmov eax, [" + variable + "]");
                asm.WriteLine("\tpush eax");
                match("%=");
                Expresion();
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                asm.WriteLine("\tdiv ebx");
                asm.WriteLine("\tmov dword [" + variable + "], edx");
            }
            else
            {
                throw new Error(" Lexico, Linea " + linea + ", no se reconoce el comando: " + Contenido, log);
            }

            if (analisisSemantico(v, nuevoValor))
            {
                v.setValor(nuevoValor);
            }
            else
            {
                throw new Error(" Semantico, Linea " + linea + ": no puedo asignar un " + tipoDatoExpresion +
                                " a un " + v.getTipo(), log);
            }

            asm.WriteLine(";Termina asignación a " + v.getNombre());

            return nuevoValor;

        }

        private Variable.TipoDato valorToTipo(float valor)
        {
            if (valor % 1 != 0)
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
                return true;
            }
            else
            {
                if (v.getTipo() == Variable.TipoDato.Char || v.getTipo() == Variable.TipoDato.Int)
                    return false;
            }
            return true;
        }

        //If -> if (Condicion) bloqueInstrucciones | instruccion (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            asm.WriteLine("; if" + cIFs);

            //int contelseifs = 0;

            string etiqueta = "_if" + cIFs;
            string etiquetaElse = "_else" + cIFs;
            cIFs++;
            bool esDO = false;
            match("if");
            match("(");
            Condicion(etiqueta, esDO);
            match(")");
            if (Contenido == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }

            if (Contenido == "else")
            {
                asm.WriteLine("\tjmp " + etiquetaElse);
            }

            asm.WriteLine(etiqueta + ":");

            if (Contenido == "else")
            {
                match("else");
                if (Contenido == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
                asm.WriteLine(etiquetaElse + ":");
            }
        }

        //Condicion -> Expresion operadorRelacional Expresion
        private void Condicion(string etiqueta, bool esDo)
        {
            Expresion(); //Expresión número 1
            string operador = Contenido;
            match(Tipos.OpRelacional);
            Expresion(); //Expresión número 2

            asm.WriteLine("\tpop ebx");
            asm.WriteLine("\tpop eax");
            asm.WriteLine("\tcmp eax, ebx");

            if (esDo)
            {
                switch (operador)
                {
                    case ">":
                        operador = "<=";
                        break;
                    case ">=":
                        operador = "<";
                        break;
                    case "<":
                        operador = ">=";
                        break;
                    case "<=":
                        operador = ">";
                        break;
                    case "==":
                        operador = "!=";
                        break;
                    default:
                        operador = "==";
                        break;
                }
            }

            switch (operador)
            {
                case ">":
                    asm.WriteLine("\tjbe " + etiqueta);
                    break;
                case ">=":
                    asm.WriteLine("\tjb " + etiqueta);
                    break;
                case "<":
                    asm.WriteLine("\tjae " + etiqueta);
                    break;
                case "<=":
                    asm.WriteLine("\tja " + etiqueta);
                    break;
                case "==":
                    asm.WriteLine("\tjne " + etiqueta);
                    break;
                default:
                    asm.WriteLine("\tje " + etiqueta);
                    break;
            }
        }

        //While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            int cTemp = caracter - 6;
            int lTemp = linea;
            bool resultado = true;
            do
            {
                match("while");
                match("(");
                //resultado = Condicion("",false);
                //Condicion();
                match(")");
                if (Contenido == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }

                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);
        }

        //Do -> do bloqueInstrucciones | intruccion while(Condicion);
        private void Do()
        {
            int cTemp = caracter - 3;
            int lTemp = linea;
            bool resultado = false;

            do
            {
                match("do");
                if (Contenido == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion();
                }
                match("while");
                match("(");
                //resultado = Condicion();
                match(")");
                match(";");

                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }
            } while (resultado);

        }

        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Intruccion
        private void For()
        {

            int cTemp = caracter - 4;
            int lTemp = linea;
            bool resultado = true;
            string var;
            float nuevoValor;
            do
            {
                match("for");
                match("(");

                var = "";

                if (Clasificacion == Tipos.TipoDato)
                {
                    var = Variables(resultado);
                }
                else if (Clasificacion == Tipos.Identificador)
                {
                    var = Contenido;
                    //Console.WriteLine(var);
                    match(Tipos.Identificador);
                    Asignacion(var);
                }

                //ejecutar = false;

                match(";");

                //resultado = Condicion();

                match(";");

                var = Contenido;
                match(Tipos.Identificador);
                nuevoValor = Asignacion(var);

                match(")");
                if (Contenido == "{")
                {
                    bloqueInstrucciones();
                }
                else
                {
                    Instruccion(); //
                }



                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == var; });
                v.setValor(nuevoValor);

                if (resultado)
                {
                    caracter = cTemp;
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    archivo.BaseStream.Seek(cTemp, SeekOrigin.Begin);
                    nextToken();
                }

            }
            while (resultado);
        }


        //Console -> Console.(WriteLine|Write) (cadena?); | Console.(Read | ReadLine) ();
        private void console()
        {

            string cadena = "";
            string cadenaN = "";
            char comillas = '"';
            bool esWrite = false;

            match("Console");
            match(".");
            if (Contenido == "WriteLine")
            {
                match("WriteLine");
            }
            else
            {
                match("Write");
                esWrite = true;
            }
            match("(");
            if (Clasificacion == Tipos.Cadena)
            {

                cadena = Contenido;
                cadenaN = cadena.Replace(comillas.ToString(), "");
                cadena = cadenaN;


                //Console.WriteLine("esto tiene contenido en cadena = Contenido: " + Contenido + "\n");

                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    Console.Write(cadena);
                    listaConcatenacion(esWrite);
                }
                else
                {
                    if (!esWrite)
                        Console.WriteLine(cadena);
                    else
                        Console.Write(cadena);

                }
            }
            else if (Clasificacion == Tipos.Identificador)
            {

                string variable = Contenido;

                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido; });

                if (v != null)
                {

                    match(Tipos.Identificador);
                    if (Contenido == "+")
                    {
                        Console.Write(v.getValor());

                        listaConcatenacion(esWrite);
                    }
                    else
                    {
                        if (!esWrite)
                            Console.WriteLine(v.getValor());
                        else
                            Console.Write(v.getValor());

                    }
                }
                else
                {
                    throw new Error(" Semantico, Linea " + linea + ": La variable " + variable + " no existe", log);
                }
            }
            else
            {
                match("(");
                Expresion();
                match(")");
                //float nuevoValor = S.Pop();
                if (Contenido == "+")
                {
                    Console.Write("");

                    listaConcatenacion(esWrite);
                }
                else
                {
                    if (!esWrite)
                        Console.WriteLine("");
                    else
                        Console.Write("");

                }

            }
            //Console.WriteLine("esto tiene contenido  match()): " + Contenido + "\n");
            match(")");
            match(";");
        }


        string listaConcatenacion(bool esWrite)
        {
            match("+");
            char comillas = '"';
            string CadenaN = Contenido;
            Contenido = CadenaN.Replace(comillas.ToString(), "");
            if (Clasificacion == Tipos.Cadena)
            {

                if (esWrite)
                    Console.Write(Contenido);
                else
                    Console.WriteLine(Contenido);

                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    listaConcatenacion(esWrite);
                }
                return "";
            }
            else if (Clasificacion == Tipos.Identificador)
            {

                string variable = Contenido;

                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido; });

                if (v != null)
                {
                    if (esWrite)
                        Console.Write(v.getValor());
                    else
                        Console.WriteLine(v.getValor());


                    match(Tipos.Identificador);
                    if (Contenido == "+")
                    {
                        listaConcatenacion(esWrite);
                    }
                }
                else
                {
                    throw new Error(" Semantico, Linea " + linea + ": La variable " + variable + " no existe", log);
                }
                return "";
            }
            else
            {
                match("(");
                Expresion();
                match(")");
                //float nuevoValor = S.Pop();
                if (Contenido == "+")
                {
                    if (esWrite)
                        Console.Write("");
                    else
                        Console.WriteLine("");

                    listaConcatenacion(esWrite);
                }
                else
                {
                    if (esWrite)
                        Console.Write("");
                    else
                        Console.WriteLine("");
                    
                }
                return "";
            }


        }


        private void asm_Main()
        {
            asm.WriteLine();
            asm.WriteLine("extern fflush");
            asm.WriteLine("extern printf");
            asm.WriteLine("extern scanf");
            asm.WriteLine("extern stdout");
            asm.WriteLine("\nsegment .text");
            asm.WriteLine("\tglobal main");
            asm.WriteLine("\nmain:");
        }
        private void asm_endMain()
        {
            asm.WriteLine("\tadd esp, 4");
            asm.WriteLine("\tmov eax, 1");
            asm.WriteLine("\txor ebx, ebx");
            asm.WriteLine("\tint 0x80");
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

            asm_Main();
            bloqueInstrucciones();
            asm_endMain();
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
                //float R1 = S.Pop();
                //float R2 = S.Pop();
                switch (operador)
                {
                    case "+": //S.Push(R2 + R1); 
                    break;
                    case "-": //S.Push(R2 - R1); 
                    break;
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
                //float R1 = S.Pop();
                //float R2 = S.Pop();
                switch (operador)
                {
                    case "*": //S.Push(R2 * R1); 
                    break;
                    case "/": //S.Push(R2 / R1); 
                    break;
                    case "%": //S.Push(R2 % R1); 
                    break;
                }

            }
        }


        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (Clasificacion == Tipos.Numero)
            {
                //S.Push(float.Parse(Contenido));
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

                if (v != null)
                {
                    //S.Push(v.getValor());
                    if (tipoDatoExpresion < v.getTipo())
                    {
                        tipoDatoExpresion = v.getTipo();
                    }
                    match(Tipos.Identificador);
                }
                else
                {
                    throw new Error(" Semantico, Linea " + linea + ": La variable " + variable + " no existe", log);
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
                    //float valor = S.Pop();
                    if (aCastear == Variable.TipoDato.Char)
                    {
                        //valor %= 256;
                    }
                    else
                    {
                        //valor %= 65536;
                    }
                    //S.Push(valor);
                }
            }
        }
    }
}