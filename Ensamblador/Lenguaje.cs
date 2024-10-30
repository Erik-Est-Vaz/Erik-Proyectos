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
    5. Programar el while LISTO
    6. Programar el for LISTO
    7. Se feliz :) PENDIENTE
    8. Tomar en cuenta todas las condiciones LISTO

*/

namespace Semantica
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        //private Stack<float> S;
        private Variable.TipoDato tipoDatoExpresion;
        private int cIFs, cDOs, cWhiles, cFors, cMsgs;

        public Lenguaje()
        {
            log.WriteLine("Analisis Semantico");
            asm.WriteLine("Analisis Semantico");
            listaVariables = new List<Variable>();
            cIFs = 1;
            cDOs = 1;
            cWhiles = 1;
            cFors = 1;
            cMsgs = 1;
        }


        public Lenguaje(string nombre) : base(nombre)
        {
            log.WriteLine("Analisis Semantico");
            listaVariables = new List<Variable>();
            cIFs = 1;
            cDOs = 1;
            cWhiles = 1;
            cFors = 1;
            cMsgs = 1;
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
        private string Variables()
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
                Variables();
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

            asm.WriteLine(";Asignacion a " + v.getNombre());

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

                        asm.WriteLine("\tpush scanner");
                        asm.WriteLine("\tpush entero");
                        asm.WriteLine("\tcall scanf");
                        asm.WriteLine("\tadd esp, 8");
                        asm.WriteLine("\tmov eax, [scanner]");
                        asm.WriteLine("\tmov dword[" + v.getNombre() + "], eax");
                    }
                    else
                    {
                        match("ReadLine");
                        try
                        {
                            asm.WriteLine("\tpush scanner");
                            asm.WriteLine("\tpush enterowl");
                            asm.WriteLine("\tcall scanf");
                            asm.WriteLine("\tadd esp, 8");
                            asm.WriteLine("\tmov eax, [scanner]");
                            asm.WriteLine("\tmov dword[" + v.getNombre() + "], eax");
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
                    asm.WriteLine("\tpop eax");
                    asm.WriteLine("\tmov dword [" + variable + "], eax");
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
            int lTemp = linea;
            bool resultado = false;

            asm.WriteLine("; while" + cWhiles);
            string etiqueta = "_while" + cWhiles;
            string etiquetaEnd = "_whileEnd" + cWhiles;
            cWhiles++;
            bool esDO = false;
            asm.WriteLine(etiqueta + ":");

            do
            {
                match("while");
                match("(");
                Condicion(etiquetaEnd, esDO);
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
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    nextToken();
                }

                asm.WriteLine("\tjmp " + etiqueta);

            } while (resultado);

            asm.WriteLine(etiquetaEnd + ":");
        }

        //Do -> do bloqueInstrucciones | intruccion while(Condicion);
        private void Do()
        {
            asm.WriteLine("; do" + cDOs);
            string etiqueta = "_do" + cDOs++;
            asm.WriteLine(etiqueta + ":");
            int lTemp = linea;
            bool resultado = false;
            bool esDO = true;

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
                Condicion(etiqueta, esDO);
                match(")");
                match(";");

                if (resultado)
                {
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    nextToken();
                }
            } while (resultado);

        }


       //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Intruccion
        private void For()
        {
            int lTemp = linea;
            bool resultado = false;
            string var;
            float nuevoValor;

            asm.WriteLine("; for" + cFors);
            string etiqueta = "_for" + cFors;
            string etiquetaEnd = "_forEnd" + cFors;
            string etiquetaAsig = "_forAsig" + cFors;
            string etiquetaAsigEnd = "_forAsigEnd" + cFors;
            cFors++;
            bool esDO = false;
            //asm.WriteLine(etiqueta + ":");

            do
            {
                match("for");
                match("(");

                var = "";

                if (Clasificacion == Tipos.TipoDato)
                {
                    var = Variables();
                }
                else if (Clasificacion == Tipos.Identificador)
                {
                    var = Contenido;
                    match(Tipos.Identificador);
                    Asignacion(var);
                }

                match(";");

                asm.WriteLine(etiqueta + ":");

                Condicion(etiquetaEnd, esDO);

                asm.WriteLine("\tjmp " + etiquetaAsigEnd);

                match(";");

                asm.WriteLine(etiquetaAsig + ":");

                var = Contenido;
                match(Tipos.Identificador);
                nuevoValor = Asignacion(var);

                asm.WriteLine("\tjmp " + etiqueta);

                asm.WriteLine(etiquetaAsigEnd + ":");

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
                    linea = lTemp;
                    archivo.DiscardBufferedData();
                    nextToken();
                }

                asm.WriteLine("\tjmp " + etiquetaAsig);

            }
            while (resultado);

            asm.WriteLine(etiquetaEnd + ":");

            //asm.WriteLine("\tmov eax, [y]");
            //asm.WriteLine("\tmov eax, [i]");
        }



        //Console -> Console.(WriteLine|Write) (cadena?); | Console.(Read | ReadLine) ();
        private void console()
        {

            string cadena = "";
            string cadenaN = "";
            char comillas = '"';
            bool esWrite = false;
            string nombreMsg = "msg" + cMsgs++;

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
                /*cadenaN = cadena.Replace(comillas.ToString(),"");
                cadena  = cadenaN;*/

                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    Console.Write(cadena);

                    listaVariables.Add(new Variable(nombreMsg, Variable.TipoDato.Char));

                    var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == nombreMsg; });

                    v.setSmg(cadena);
                    v.setEsWrite(true);

                    asm.WriteLine("\tpush ebp");
                    asm.WriteLine("\tmov ebp, esp");
                    asm.WriteLine("\tpush " + nombreMsg);

                    asm.WriteLine("\tcall printf");
                    asm.WriteLine("\tmov esp, ebp");
                    asm.WriteLine("\tpop ebp");

                    listaConcatenacion(esWrite);
                }
                else
                {
                    if (!esWrite)
                    {
                        Console.WriteLine(cadena);

                        listaVariables.Add(new Variable(nombreMsg, Variable.TipoDato.Char));

                        var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == nombreMsg; });

                        v.setSmg(cadena);
                        v.setEsWrite(false);

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush " + nombreMsg);

                        asm.WriteLine("\tcall printf");

                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");


                    }
                    else
                    {
                        Console.Write(cadena);

                        listaVariables.Add(new Variable(nombreMsg, Variable.TipoDato.Char));

                        var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == nombreMsg; });

                        v.setSmg(cadena);
                        v.setEsWrite(true);

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush " + nombreMsg);

                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");
                    }
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

                        asm.WriteLine("\tpush dword [" + v.getNombre() + "]");
                        switch (v.getTipo())
                        {
                            case Variable.TipoDato.Char:
                                asm.WriteLine("\tpush caracter");
                                break;
                            case Variable.TipoDato.Int:
                                asm.WriteLine("\tpush entero");
                                break;
                            case Variable.TipoDato.Float:
                                asm.WriteLine("\tpush floatante");
                                break;

                        }
                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tadd esp, 8");

                        listaConcatenacion(esWrite);
                    }
                    else
                    {
                        if (!esWrite)
                        {
                            Console.WriteLine(v.getValor());

                            asm.WriteLine("\tpush dword [" + v.getNombre() + "]");
                            switch (v.getTipo())
                            {
                                case Variable.TipoDato.Char:
                                    asm.WriteLine("\tpush caracterwl");
                                    break;
                                case Variable.TipoDato.Int:
                                    asm.WriteLine("\tpush enterowl");
                                    break;
                                case Variable.TipoDato.Float:
                                    asm.WriteLine("\tpush floatantewl");
                                    break;

                            }
                            asm.WriteLine("\tcall printf");
                            asm.WriteLine("\tadd esp, 8");
                        }
                        else
                        {
                            Console.Write(v.getValor());

                            asm.WriteLine("\tpush dword [" + v.getNombre() + "]");
                            switch (v.getTipo())
                            {
                                case Variable.TipoDato.Char:
                                    asm.WriteLine("\tpush caracter");
                                    break;
                                case Variable.TipoDato.Int:
                                    asm.WriteLine("\tpush entero");
                                    break;
                                case Variable.TipoDato.Float:
                                    asm.WriteLine("\tpush floatante");
                                    break;

                            }
                            asm.WriteLine("\tcall printf");
                            asm.WriteLine("\tadd esp, 8");
                        }

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
                asm.WriteLine("\tpop ");
                if (Contenido == "+")
                {
                    Console.Write("");

                    listaVariables.Add(new Variable(nombreMsg, Variable.TipoDato.Char));
                    var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == nombreMsg; });
                    v.setSmg("");

                    asm.WriteLine("\tpush ebp");
                    asm.WriteLine("\tmov ebp, esp");
                    asm.WriteLine("\tpush vacio");

                    asm.WriteLine("\tcall printf");
                    asm.WriteLine("\tmov esp, ebp");
                    asm.WriteLine("\tpop ebp");

                    listaConcatenacion(esWrite);
                }
                else
                {
                    if (esWrite)
                    {
                        Console.Write("");

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush vacio");

                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");
                    }
                    else
                    {
                        Console.WriteLine("");

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush vaciowl");

                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");
                    }
                }
            }
            match(")");
            match(";");
        }


        string listaConcatenacion(bool esWrite)
        {
            match("+");
            char comillas = '"';
            string CadenaN = Contenido;
            Contenido = CadenaN.Replace(comillas.ToString(), "");

            string nombreMsg = "msg" + cMsgs++;

            if (Clasificacion == Tipos.Cadena)
            {

                /*
                if (esWrite)
                    Console.Write(Contenido);
                else
                    Console.WriteLine(Contenido);
                */
                if (esWrite)
                {
                    Console.Write(Contenido);

                    listaVariables.Add(new Variable(nombreMsg, Variable.TipoDato.Char));
                    var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == nombreMsg; });
                    v.setSmg(Contenido);
                    v.setEsWrite(true);

                    asm.WriteLine("\tpush ebp");
                    asm.WriteLine("\tmov ebp, esp");
                    asm.WriteLine("\tpush " + nombreMsg);

                    asm.WriteLine("\tcall printf");
                    asm.WriteLine("\tmov esp, ebp");
                    asm.WriteLine("\tpop ebp");

                }
                else
                {
                    Console.WriteLine(Contenido);

                    listaVariables.Add(new Variable(nombreMsg, Variable.TipoDato.Char));
                    var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == nombreMsg; });
                    v.setSmg(Contenido);
                    v.setEsWrite(false);

                    asm.WriteLine("\tpush ebp");
                    asm.WriteLine("\tmov ebp, esp");
                    asm.WriteLine("\tpush " + nombreMsg);

                    asm.WriteLine("\tcall printf");
                    asm.WriteLine("\tmov esp, ebp");
                    asm.WriteLine("\tpop ebp");
                }

                match(Tipos.Cadena);
                if (Contenido == "+")
                {
                    listaConcatenacion(esWrite);
                }
                return "";
            }
            else if (Clasificacion == Tipos.Identificador)
            {

                /*string variable = Contenido;

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
                }*/

                string variable = Contenido;
                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == Contenido; });

                if (v != null)
                {

                    match(Tipos.Identificador);
                    if (Contenido == "+")
                    {
                        Console.Write(v.getValor());

                        asm.WriteLine("\tpush dword [" + v.getNombre() + "]");
                        switch (v.getTipo())
                        {
                            case Variable.TipoDato.Char:
                                asm.WriteLine("\tpush caracter");
                                break;
                            case Variable.TipoDato.Int:
                                asm.WriteLine("\tpush entero");
                                break;
                            case Variable.TipoDato.Float:
                                asm.WriteLine("\tpush flotante");
                                break;

                        }
                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tadd esp, 8");

                        listaConcatenacion(esWrite);
                    }
                    else
                    {
                        if (!esWrite)
                        {
                            Console.WriteLine(v.getValor());

                            asm.WriteLine("\tpush dword [" + v.getNombre() + "]");
                            switch (v.getTipo())
                            {
                                case Variable.TipoDato.Char:
                                    asm.WriteLine("\tpush caracterwl");
                                    break;
                                case Variable.TipoDato.Int:
                                    asm.WriteLine("\tpush enterowl");
                                    break;
                                case Variable.TipoDato.Float:
                                    asm.WriteLine("\tpush floatantewl");
                                    break;

                            }
                            asm.WriteLine("\tcall printf");
                            asm.WriteLine("\tadd esp, 8");
                        }
                        else
                        {
                            Console.Write(v.getValor());

                            asm.WriteLine("\tpush dword [" + v.getNombre() + "]");
                            switch (v.getTipo())
                            {
                                case Variable.TipoDato.Char:
                                    asm.WriteLine("\tpush caracter");
                                    break;
                                case Variable.TipoDato.Int:
                                    asm.WriteLine("\tpush entero");
                                    break;
                                case Variable.TipoDato.Float:
                                    asm.WriteLine("\tpush floatante");
                                    break;

                            }
                            asm.WriteLine("\tcall printf");
                            asm.WriteLine("\tadd esp, 8");
                        }

                    }
                }
                else
                {
                    throw new Error(" Semantico, Linea " + linea + ": La variable " + variable + " no existe", log);
                }

                return "";
            }
            else //nada
            {
                match("(");
                Expresion();
                match(")");
                asm.WriteLine("\tpop ");
                if (Contenido == "+")
                {
                    if (esWrite)
                    {
                        Console.Write("");

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush vacio");

                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");
                    }
                    else
                    {
                        Console.WriteLine("");

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush vaciowl");

                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");
                    }

                    listaConcatenacion(esWrite);
                }
                else
                {
                    if (esWrite)
                    {
                        Console.Write("");

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush vacio");

                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");
                    }
                    else
                    {
                        Console.WriteLine("");

                        asm.WriteLine("\tpush ebp");
                        asm.WriteLine("\tmov ebp, esp");
                        asm.WriteLine("\tpush vaciowl");

                        asm.WriteLine("\tcall printf");
                        asm.WriteLine("\tmov esp, ebp");
                        asm.WriteLine("\tpop ebp");
                    }
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

                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "+": //S.Push(R2 + R1); 
                        asm.WriteLine("\tadd eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "-": //S.Push(R2 - R1); 
                        asm.WriteLine("\tsub eax, ebx");
                        asm.WriteLine("\tpush eax");
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

                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "*": //S.Push(R2 * R1); 
                        asm.WriteLine("\tmul ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "/": //S.Push(R2 / R1);
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "%": //S.Push(R2 % R1); 
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush edx");
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
                asm.WriteLine("\tmov eax, " + Contenido);
                asm.WriteLine("\tpush eax");

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

                    asm.WriteLine("\tmov eax, [" + Contenido + "]");
                    asm.WriteLine("\tpush eax");

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
                    //float valor = v.getValor();
                    asm.WriteLine("\tpop eax");
                    if (aCastear == Variable.TipoDato.Char)
                    {
                        //valor %= 256;
                    }
                    else
                    {
                        //valor %= 65536;
                    }
                    //S.Push(valor);
                    asm.WriteLine("\tmov eax, "/* + v.getNombre()*/);
                    asm.WriteLine("\tpush eax");
                }
            }
        }
    }
}