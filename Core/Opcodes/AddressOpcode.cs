
namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Variables;
	using CSim.Core.Types.Primitives;
	using CSim.Core.Literals;
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	/// <summary>
	/// Represents the rol of '&' at the left of a variable.
	/// i.e., returns the address of that variable in memory.
	/// Can apply to an id only.
	/// </summary>
	public class AddressOpcode: Opcode {
		public const char OpcodeValue = (char) 0xE0;

		public AddressOpcode(Machine m, Id id)
			:base( m )
		{
			this.id = id;
		}

		/// <summary>
		/// Returns the address of the variable, given its id
		/// </summary>
		public override void Execute()
		{
			Variable toret = null;
			int address = 0;
			var vble = this.Machine.TDS.LookUp( this.Id.Name );

			if ( vble != null ) {
				toret = new TempVariable( this.Machine.TypeSystem.GetIntType() );
				var vbleAsRef = vble as RefVariable;
				address = vble.Address;

				// If the vble at the right is a reference, dereference it
				if ( vbleAsRef != null  ) {
					address = vbleAsRef.PointedVble.Address;
				}

				// Store in the temp vble and end
				toret.LiteralValue = new IntLiteral( this.Machine, address );
				this.Machine.ExecutionStack.Push( toret );
			} else {
				throw new EngineException( "lvalue should be a variable" );
			}

			return;
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

