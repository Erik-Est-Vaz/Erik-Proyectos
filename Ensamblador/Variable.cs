using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Semeantica
{
    public class Variable
    {
        public enum TipoDato
        {
            Char,Int,Float
        }
        private string nombre;
        private TipoDato tipo;
        private float valor;
        private string smg = "";
        private bool esWrite = false;
        public Variable(string nombre,TipoDato tipo)
        {
            this.nombre = nombre;
            this.tipo = tipo;
        }
        public void setValor(float valor)
        {
            this.valor = valor;
        }
        public void setSmg(string smg)
        {
            this.smg = smg;
        }
        public string getSmg()
        {
            return this.smg;
        }
        public void setEsWrite(bool esWrite)
        {
            this.esWrite = esWrite;
        }
        public bool getEsWrite()
        {
            return this.esWrite;
        }
        public string getNombre()
        {
            return this.nombre;
        }
        public TipoDato getTipo()
        {
            return this.tipo;
        }
        public float getValor()
        {
            return this.valor;
        }
    }
}