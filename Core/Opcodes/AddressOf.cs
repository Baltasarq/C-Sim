using System;

using CSim.Core.Variables;
using CSim.Core.Types.Primitives;
using CSim.Core.Literals;
using CSim.Core.Types;
using CSim.Core.Exceptions;

namespace CSim.Core.Opcodes.Operands {
	/// <summary>
	/// Represents the rol of '&' at the left of a variable.
	/// i.e., returns the address of that variable in memory.
	/// Can apply to an id only.
	/// </summary>
	public class AddressOf: Operand {
		public const char OpcodeValue = (char) 0xEA;

		public AddressOf(Machine m, Id id)
			:base( m )
		{
			this.id = id;
		}

		/// <summary>
		/// Returns the address of the variable, given its id
		/// </summary>
		public override Variable Execute()
		{
			var toret = new TempVariable( Int.Get() );
			var vble = this.Machine.TDS.LookUp( this.Id.Value );
			var vbleAsRef = vble as RefVariable;
			int address = vble.Address;

			// If the vble at the right is a reference, dereference it
			if ( vbleAsRef != null  ) {
				address = vbleAsRef.PointedVble.Address;
			}

			// Store in the temp vble and end
			toret.LiteralValue = new IntLiteral( address );
			return toret;
		}

		/// <summary>
		/// Gets the identifier of the variable.
		/// </summary>
		/// <value>The identifier.</value>
		public Id Id {
			get {
				return this.id;
			}
		}

		private Id id;
	}
}

