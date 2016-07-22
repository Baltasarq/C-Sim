using System;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using CSim.Core.Exceptions;
using CSim.Core.Functions;
using CSim.Core.FunctionLibrary;

namespace CSim.Core {
	/// <summary>
	/// Represents the standard library for the machine.
	/// </summary>
	public sealed class StdLib {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.StdLib"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		public StdLib(Machine m)
		{
			this.machine = m;
			this.Build();
		}

		private void BuildFunctionList()
		{
			var asm = typeof( Type ).Assembly;
			this.fns = new List<EmbeddedFunction>();

			foreach(System.Type t in asm.GetTypes()) {
				if ( t.IsClass
				  && t.FullName.Contains( "FunctionLibrary" ) )
				{
					this.fns.Add(
						(EmbeddedFunction)
						t.InvokeMember( "Get",
					               	    BindingFlags.Public
					               			| BindingFlags.InvokeMethod
					               			| BindingFlags.Static,
					                    null, null, new object[] { this.Machine } )
					);
				}
			}

			return;
		}

		private void BuildFunctionDictionary()
		{
			this.fnslkup = new Dictionary<string, EmbeddedFunction>();

			foreach(EmbeddedFunction f in this.fns) {
				this.fnslkup.Add( f.Id, f );
			}

			return;
		}

		private void Build()
		{
			this.BuildFunctionList();
			this.BuildFunctionDictionary();
		}

		/// <summary>
		/// Match the function with the specified id and args.
		/// </summary>
		/// <param name="id">The identifier of the function, as a string.</param>
		/// <param name="args">The real arguments, as a collection of Variable.</param>
		public EmbeddedFunction Match(string id)
		{
			EmbeddedFunction toret = null;

			// Look up id in function dictionary
			if ( !this.fnslkup.TryGetValue( id, out toret ) ) {
			}

			return toret;
		}

		/// <summary>
		/// Gets all the functions.
		/// </summary>
		/// <value>The functions, as an EmbeddedFunction collection.</value>
		public ReadOnlyCollection<EmbeddedFunction> Functions {
			get {
				return new ReadOnlyCollection<EmbeddedFunction>( this.fns );
			}
		}

		/// <summary>
		/// Gets the machine this lib pertains to.
		/// </summary>
		/// <value>The machine, as a <see cref="Machine"/> object.</value>
		public Machine Machine {
			get {
				return this.machine;
			}
		}

		private Machine machine;
		private List<EmbeddedFunction> fns;
		private Dictionary<string, EmbeddedFunction> fnslkup;
	}
}

