using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Lenguaje
{
	public class Lenguaje : Sintaxis
	{
		public Lenguaje()
		{
			
		}
		public Lenguaje(string nombre) : base(nombre)
		{
			
		}
		 
		public void Programa()
		{
			A();
			match("b");
			C();
			
		}
		private void A()
		{
			C();
			if (Clasificacion == Tipos.Numero)
			{
				match(Tipos.Numero);
				match("b");
				C();
				D();
			}
			D();
			
		}
		private void C()
		{
			D();
			if (Contenido == "a")
			{
				match("a");
			}
			else if (Contenido == "b")
			{
				match("b");
			}
			else if (Clasificacion == Tipos.D)
			{
				D();
			}
			else
			{
				throw new Error(" Sintaxis, Linea " + linea + ,": Se esperaba un SNT", log);
			}
			
		}
		private void D()
		{
			
		}
	}
}
