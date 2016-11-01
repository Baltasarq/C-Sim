namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Variables;
	using CSim.Core.Types.Primitives;
	using CSim.Core.Literals;
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	public class ArrayIndexAccessOpcode: Opcode {
		public const char OpcodeValue = (char) 0xE9;

		public ArrayIndexAccessOpcode(Machine m)
			:base(m)
		{
		}

		/// <summary>
		/// Returns the a variable, accessing its address with '*'
		/// </summary>
		public override void Execute()
		{
			// Take id
			Variable vble = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			// Take offset
			Variable offset = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			if ( vble != null ) {
				var vbleAsPtr = vble as PtrVariable;

				// If the vble at the right is a reference, dereference it
				if ( vbleAsPtr != null  ) {
					vble = this.Machine.TDS.LookForAddress( vbleAsPtr.IntValue.Value );
				}

				// Chk
				var ptrType = vble.Type as Ptr;

				if ( ptrType == null ) {
					throw new TypeMismatchException( vble.Name.Name );
				}

				// Find the address
				long address = vble.Address + ( (long) offset.LiteralValue.Value );

				// Store in the temp vble and end
				Variable result = new TempVariable(
					this.Machine.Memory.CreateLiteral(
						address,
						ptrType.AssociatedType )
				);

				this.Machine.ExecutionStack.Push( result );
			} else {
				throw new EngineException( "invalid rvalue" );
			}

			return;
		}
	}
}
