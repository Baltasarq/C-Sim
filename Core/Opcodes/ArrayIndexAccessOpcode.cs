namespace CSim.Core.Opcodes {
    using System.Numerics;
	using CSim.Core.Variables;
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	/// <summary>
	/// Array index access opcode.
	/// </summary>
	public class ArrayIndexAccessOpcode: Opcode {
		/// <summary>The opcode id.</summary>
		public const char OpcodeValue = (char) 0xE9;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Opcodes.ArrayIndexAccessOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		public ArrayIndexAccessOpcode(Machine m)
			:base(m)
		{
		}

		/// <summary>
		/// Returns the a variable, accessing its address plus offset with '[]'
		/// int v[6]; printf(v[2]); => v[2] == &amp;v + ( 2 * sizeof(int) )
		/// </summary>
		public override void Execute()
		{
			// Take ptr
			Variable vble = this.Machine.ExecutionStack.Pop().SolveToVariable();

			// Take offset
			Variable offset = this.Machine.ExecutionStack.Pop().SolveToVariable();

			if ( vble != null
			  && offset != null )
			{
				BigInteger address = 0;
				var ptrVble = vble.Type as Ptr;

				// Find the address of the pointed array
				if ( ptrVble != null ) {
					address = vble.LiteralValue.GetValueAsInteger();
				}
				else
				if ( vble is ArrayVariable ) {
					address = vble.Address;
				} else {
					throw new EngineException( vble.Name + "[x]??" );
				}

				// Chk
				if ( !( offset.Type is Primitive ) ) {
					throw new TypeMismatchException( offset.LiteralValue.ToString() );
				}		

				// Store in the ArrayElement vble and end
				Variable result = new ArrayElement(
										vble.Name.Name,
										address,
										(Ptr) vble.Type,
										offset.LiteralValue.GetValueAsInteger() );
		
				this.Machine.ExecutionStack.Push( result );
			} else {
				throw new EngineException( "missing or invalid rvalue" );
			}

			return;
		}
	}
}
