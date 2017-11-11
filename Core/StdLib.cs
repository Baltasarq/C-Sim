// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
	using System.Reflection;
	using System.Collections.ObjectModel;
	using System.Collections.Generic;
    
	using Functions;

	/// <summary>
	/// Represents the standard library for the machine.
	/// </summary>
	public sealed class StdLib {
		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.StdLib"/> class.
		/// </summary>
		public StdLib(Machine m)
		{
			this.Machine = m;
			this.Build();
		}

		private void BuildFunctionList()
		{
			var asm = typeof( AType ).Assembly;
			this.fns = new List<EmbeddedFunction>();

			foreach(System.Type t in asm.GetTypes()) {
				if ( t.IsClass
                  && t.IsSubclassOf( typeof( EmbeddedFunction ) ) )
				{
                    MethodInfo mthInfo = t.GetMethod( "Get" );

                    if ( mthInfo != null ) {
    					this.fns.Add(
                            (EmbeddedFunction) mthInfo.Invoke(
		                            null,
		                            new object[] { this.Machine } ) );
                    }
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
			get; private set;
		}

		private List<EmbeddedFunction> fns;
		private Dictionary<string, EmbeddedFunction> fnslkup;
	}
}

