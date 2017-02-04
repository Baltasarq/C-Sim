
namespace CSim.Core.Opcodes {
	using CSim.Core.Variables;
	using CSim.Core.Literals;

	/// <summary>
	/// Represents the rol of ampersand at the left of a variable.
	/// i.e., returns the address of that variable in memory.
	/// Can apply to an id only.
	/// </summary>
	public class AddressOfOpcode: Opcode {
		/// <summary>The opcode value.</summary>
		public const char OpcodeValue = (char) 0xE0;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Opcodes.AddressOfOpcode"/> class.
		/// </summary>
		/// <param name="m">The machine this opcode will be executed in.</param>
		public AddressOfOpcode(Machine m)
			:base( m )
		{
		}

		/// <summary>
		/// Returns the address of the variable popped from the stack.
		/// </summary>
		public override void Execute()
		{
			Variable toret = null;
			long address = 0;
			var vble = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			if ( vble != null
			  && !( vble is NoPlaceTempVariable ) )
			{
				toret = new NoPlaceTempVariable( this.Machine.TypeSystem.GetIntType() );
				var vbleAsRef = vble as RefVariable;
				address = vble.Address;

				// If the vble at the right is a reference, dereference it
				if ( vbleAsRef != null  ) {
					vble = vbleAsRef.PointedVble;
					address = vble.Address;
				}

				// Store in the temp vble and end
				toret.LiteralValue = new IntLiteral( this.Machine, address );
				this.Machine.ExecutionStack.Push( toret );
			} else {
				throw new EngineException( "rvalue should be a variable" );
			}

			return;
		}
	}
}

