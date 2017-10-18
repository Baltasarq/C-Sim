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
		public const byte OpcodeValue = 0xE9;

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

				// Find the address of the pointed array
                if ( vble is ArrayVariable ) {
                    address = vble.Address;
                }
                else
				if ( vble.IsIndirection() ) {
					address = ( (IndirectVariable) vble ).PointedAddress;
				} else {
					throw new EngineException( vble.Name + "[x]??" );
				}

				// Chk
				if ( !( offset.Type is Primitive ) ) {
					throw new TypeMismatchException( 
                                        this.Machine.TypeSystem.GetIntType()
                                        + " != "
                                        + offset.LiteralValue );
				}		

				// Store in the ArrayElement vble and end
				Variable result = Variable.CreateTempVariableForArrayElement(
										vble.Name.Name,
										address,
										(Ptr) vble.Type,
										(int) offset.LiteralValue.ToBigInteger()
                );
		
				this.Machine.ExecutionStack.Push( result );
			} else {
				throw new EngineException( "missing or invalid rvalue" );
			}

			return;
		}
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.ArrayIndexAccessOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.ArrayIndexAccessOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                    "[ArrayIndexAccessOpcode(0x{0,2:X}): lvalue(POP)[rvalue(POP)]",
                    OpcodeValue );
        }
	}
}
