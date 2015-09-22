using System;

namespace CSim.Core.Opcodes {
	/// <summary>
	/// Fetches a variable from the symbol table.
	/// </summary>
	public class Fetch: Opcode {
		public Fetch(Machine m, Id id)
			: base( m )
		{
			this.Id = id;
		}

		public override Variable Execute()
		{
			return this.Machine.TDS.LookUp( this.Id.Value );
		}


		/// <summary>
		/// Gets or sets the identifier to fetch.
		/// </summary>
		/// <value>The identifier, as an Id object.</value>
		public Id Id {
			get; set;
		}
	}
}

