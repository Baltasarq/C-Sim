namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Variables;
	using CSim.Core.Types.Primitives;
	using CSim.Core.Literals;
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
			Variable arrayVble = null;

			// Take ptr
			Variable vble = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			// Take offset
			Variable offset = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			if ( vble != null
			  && offset != null )
			{
				var vbleAsPtr = vble as PtrVariable;

				// Find the address of the pointed array
				if ( vbleAsPtr != null  ) {
					arrayVble = this.Machine.TDS.LookForAddress( vbleAsPtr.IntValue.Value );
				}

				if ( arrayVble != null) {
					// Chk
					var ptrType = arrayVble.Type as Ptr;

					if ( ptrType == null ) {
						throw new TypeMismatchException( vble.Name.Name );
					}

					if ( !offset.Type.IsArithmetic() ) {
						throw new TypeMismatchException( offset.LiteralValue.ToString() );
					}		

					// Store in the ArrayElement vble and end
					Variable result = new ArrayElement(
											arrayVble,
											(Ptr) vbleAsPtr.Type,
											offset.LiteralValue.GetValueAsInt(),
						                   	this.Machine );
			
					this.Machine.ExecutionStack.Push( result );
				} else {
					throw new EngineException( vble.Name.Name + " == " + vble.Address + "??" );
				}
			} else {
				throw new EngineException( "missing or invalid rvalue" );
			}

			return;
		}
	}
}
