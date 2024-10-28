using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

/*
    El proyecto generea código ASM en: nasm o masm o ... excerpto emu8086

    1. Completar la asignacion
    2. Console.Write & Console.WriteLine
    3. Console.Read & Console.ReadLine
    4. Considerar el else en el IF
    5. Programar el while
    6. Programar el for

*/
namespace Ensamblador
{
    public class Lenguaje : Sintaxis
    {
        private List<Variable> listaVariables;
        private int cIFs, cDos, cWhiles;
        public Lenguaje()
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("; Analizador Sintactico");
            asm.WriteLine("; Analizador Semantico");
            listaVariables = new List<Variable>();
            cIFs = cDos = 1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            log.WriteLine("Analizador Sintactico");
            asm.WriteLine("; Analizador Sintactico");
            asm.WriteLine("; Analizador Semantico");
            listaVariables = new List<Variable>();
            cIFs = cDos = 1;
        }
        // Programa  -> Librerias? Main
        public void Programa()
        {
            if (getContenido() == "using")
            {
                Librerias();
            }
            Main();
            imprimeVariables();
        }
        // Librerias -> using ListaLibrerias; Librerias?
        private void Librerias()
        {
            match("using");
            listaLibrerias();
            match(";");
            if (getContenido() == "using")
            {
                Librerias();
            }
        }
        // ListaLibrerias -> identificador (.ListaLibrerias)?
        private void listaLibrerias()
        {
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                listaLibrerias();
            }
        }
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
        // Variables -> tipo_dato Lista_identificadores;
        private void Variables()
        {
            Variable.TipoDato tipo = getTipo(getContenido());
            match(Tipos.TipoDato);
            listaIdentificadores(tipo);
            match(";");
        }
        private void imprimeVariables()
        {
            // log.WriteLine("Lista de variables");
            asm.WriteLine("\nsegment .data");
            foreach (Variable v in listaVariables)
            {
                // log.WriteLine(v.getNombre() + " (" + v.getTipo() + ") = " + v.getValor());
                if (v.getTipo() == Variable.TipoDato.Char)
                {
                    asm.WriteLine("\t" + v.getNombre() + " db 0");
                }
                else if (v.getTipo() == Variable.TipoDato.Int)
                {
                    asm.WriteLine("\t" + v.getNombre() + " dd 0");
                }
                else
                {
                    asm.WriteLine("\t" + v.getNombre() + " dw 0 ");
                }
            }
        }
        // ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void listaIdentificadores(Variable.TipoDato t)
        {
            listaVariables.Add(new Variable(getContenido(), t));
            match(Tipos.Identificador);
            if (getContenido() == "=")
            {
                match("=");
                Expresion();
            }
            if (getContenido() == ",")
            {
                match(",");
                listaIdentificadores(t);
            }
        }
        // BloqueInstrucciones -> { listaIntrucciones? }
        private void bloqueInstrucciones()
        {
            match("{");
            if (getContenido() != "}")
            {
                listaIntrucciones();
            }
            match("}");
        }
        // ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void listaIntrucciones()
        {
            Instruccion();
            if (getContenido() != "}")
            {
                listaIntrucciones();
            }
        }
        // Instruccion -> Console | If | While | do | For | Variables | Asignacion
        private void Instruccion()
        {
            if (getContenido() == "Console")
            {
                console();
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
            else if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            else
            {
                Asignacion();
                match(";");
            }
        }
        // Asignacion -> Identificador = Expresion;
        private void Asignacion()
        {
            string variable = getContenido();
            match(Tipos.Identificador);
            asm.WriteLine("; Asignacion a " + variable);
            var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == variable; });
            float nuevoValor = v.getValor();

            if (getContenido() == "=")
            {
                match("=");
                if (getContenido() == "Console")
                {
                    match("Console");
                    match(".");
                    if (getContenido() == "Read")
                    {
                        match("Read");
                    }
                    else
                    {
                        match("ReadLine");
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
            else if (getContenido() == "++")
            {
                match("++");
                asm.WriteLine("\tinc dword [" + variable+"]");
                nuevoValor++;
            }
            else if (getContenido() == "--")
            {
                match("--");
                asm.WriteLine("\tdec " + variable);
                nuevoValor--;
            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            else
            {
                match("%=");
                Expresion();
                asm.WriteLine("\tpop eax");
            }
            // match(";");            
            v.setValor(nuevoValor);
            // log.WriteLine(variable + " = " + nuevoValor);
            asm.WriteLine("; Termina asignacion a " + variable);
        }
        // If -> if (Condicion) bloqueInstrucciones | instruccion
        // (else bloqueInstrucciones | instruccion)?
        private void If()
        {
            asm.WriteLine("; if " + cIFs);
            string etiqueta = "_if" + cIFs++;
            match("if");
            match("(");
            Condicion(etiqueta);
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
            asm.WriteLine(etiqueta + ":");
            // Generar una etiqueta
        }
        // Condicion -> Expresion operadorRelacional Expresion
        private void Condicion(string etiqueta)
        {
            Expresion(); // E1
            string operador = getContenido();
            match(Tipos.OpRelacional);
            Expresion(); // E2
            asm.WriteLine("\tpop eax");
            asm.WriteLine("\tpop ebx");
            asm.WriteLine("\tcmp eax, ebx");
            switch (operador)
            {
                case ">": 
                case ">=": 
                case "<": 
                case "<=": 
                case "==": asm.WriteLine("\tjne "+etiqueta);
                           break;
                default:   asm.WriteLine("\tje "+etiqueta);
                           break;
                           
            }
        }
        // While -> while(Condicion) bloqueInstrucciones | instruccion
        private void While()
        {
            asm.WriteLine("; while " + ++cWhiles);
            string etiquetaIni = "_whileIni" + cWhiles; 
            string etiquetaFin = "_whileFin" + cWhiles; 
            match("while");
            match("(");
            asm.WriteLine(etiquetaIni + ":");
            Condicion(etiquetaFin);
            match(")");
            if (getContenido() == "{")
            {
                bloqueInstrucciones();
            }
            else
            {
                Instruccion();
            }
            asm.WriteLine("jmp "+etiquetaIni);
            asm.WriteLine(etiquetaFin + ":");
        }
        // Do -> do 
        //          bloqueInstrucciones | intruccion 
        //       while(Condicion);
        private void Do()
        {
            asm.WriteLine("; do " + cDos);
            string etiqueta = "_do" + cDos++;
            asm.WriteLine(etiqueta + ":");
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
            Condicion(etiqueta);
            match(")");
            match(";");
        }
        // For -> for(Asignacion Condicion; Incremento) 
        //          BloqueInstrucciones | Intruccion
        private void For()
        {
            match("for");
            match("(");
            Asignacion();
            match(";");
            Condicion("");
            match(";");
            Asignacion();
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
        // Console -> Console.(WriteLine|Write) (cadena?);
        private void console()
        {
            match("Console");
            match(".");
            if (getContenido() == "WriteLine")
            {
                match("WriteLine");
            }
            else
            {
                match("Write");
            }
            match("(");
            if (getClasificacion() == Tipos.Cadena)
            {
                match(Tipos.Cadena);
                if (getContenido() == "+")
                {
                    listaConcatenacion();
                }
            }
            match(")");
            match(";");
        }
        private string listaConcatenacion()
        {
            match("+");
            match(Tipos.Identificador); // Validar que exista la variable
            if (getContenido() == "+")
            {
                listaConcatenacion();
            }
            return "";
        }
        private void asm_Main()
        {
            asm.WriteLine();
            asm.WriteLine("extern fflush");
            asm.WriteLine("extern printf");
            asm.WriteLine("extern scanf");
            asm.WriteLine("extern stdout");
            asm.WriteLine("\nsegment .text");
            asm.WriteLine("\tglobal _main");
            asm.WriteLine("\n_main:");
        }
        private void asm_endMain()
        {
            asm.WriteLine("\tadd esp, 4\n");
            asm.WriteLine("\tmov eax, 1");
            asm.WriteLine("\txor ebx, ebx");
            asm.WriteLine("\tint 0x80");
        }
        // Main      -> static void Main(string[] args) BloqueInstrucciones 
        private void Main()
        {
            asm_Main();
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
            asm_endMain();
        }
        // Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        // MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OpTermino)
            {
                string operador = getContenido();
                match(Tipos.OpTermino);
                Termino();
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "+":
                        asm.WriteLine("\tadd eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "-":
                        asm.WriteLine("\tsub eax, ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                }
            }
        }
        // Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        // PorFactor -> (OperadorFactor Factor)?
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OpFactor)
            {
                string operador = getContenido();
                match(Tipos.OpFactor);
                Factor();
                asm.WriteLine("\tpop ebx");
                asm.WriteLine("\tpop eax");
                switch (operador)
                {
                    case "*":
                        asm.WriteLine("\tmul ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "/":
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush eax");
                        break;
                    case "%":
                        asm.WriteLine("\tdiv ebx");
                        asm.WriteLine("\tpush edx");
                        break;
                }
            }
        }
        // Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                asm.WriteLine("\tmov eax, " + getContenido());
                asm.WriteLine("\tpush eax");
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                var v = listaVariables.Find(delegate (Variable x) { return x.getNombre() == getContenido(); });
                asm.WriteLine("\tmov eax, " + getContenido());
                asm.WriteLine("\tpush eax");
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                Expresion();
                match(")");
            }
        }
    }
}