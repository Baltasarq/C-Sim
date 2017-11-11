// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Opcodes {
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;

	/// <summary>
	/// The Div opcode (mathematically divides numbers).
	/// </summary>
	public class DivOpcode: Opcode {
		/// <summary>The opcode's representing value.</summary>
		public const byte OpcodeValue = 0xEA;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Opcodes.DivOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		public DivOpcode(Machine m)
			:base(m)
		{
		}

		/// <summary>
		/// Returns the result of a / b
		/// </summary>
		public override void Execute()
		{
			// Check arguments in stack
			if ( this.Machine.ExecutionStack.Count < 2 ) {
				throw new RuntimeException( L18n.Get( L18n.Id.ErrMissingArguments ) );
			}

			// Take ops
			Variable op2 = this.Machine.ExecutionStack.Pop().SolveToVariable();
			Variable op1 = this.Machine.ExecutionStack.Pop().SolveToVariable();
            
            ChkTypeCompatibility( op1, op2 );

			// Chk
			if ( op2.LiteralValue.GetValueAsInteger() == 0 ) {
				throw new RuntimeException( "/0!!" );
			}

			// Now yes, do it
			Literal litResult;

			if ( op1.Type is Types.Primitives.Double
			  || op2.Type is Types.Primitives.Double )
			{
				litResult = new DoubleLiteral(
                                    this.Machine,
				                    op1.LiteralValue.Value.ToDouble()
				                    / op2.LiteralValue.Value.ToDouble() );
			} else {
				litResult = new IntLiteral( this.Machine,
				                           op1.LiteralValue.GetValueAsInteger()
				                           / op2.LiteralValue.GetValueAsInteger() );
			}

			// Store in the temp vble and end
			this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litResult ) );
			return;
		}
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.DivOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.DivOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format( "[DivOpcode (0x{0,2:X}): rvalue(POP) / rvalue(POP)]",
                                    OpcodeValue );
        }
	}
}
