using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Java
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
			Librerias();
			Main();
		}
		private void Librerias()
		{
			match("#");
			match("include");
			match("<");
			match(Tipos.Identificador);
			if (
		}
		match("?");
		match(">");
	}
	private void Main()
	{
		match("void");
		Main();
		match("(");
		match(")");
		BloqueInstrucciones();
	}
	private void BloqueInstrucciones()
	{
	}
}
