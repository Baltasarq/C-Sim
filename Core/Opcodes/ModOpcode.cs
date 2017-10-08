namespace CSim.Core.Opcodes {
    using System.Numerics;

	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;

	/// <summary>
	/// Mod opcode, allowing operations like 10%3.
	/// </summary>
	public class ModOpcode: Opcode {
		/// <summary>The opcode's representing value./// </summary>
		public const byte OpcodeValue = 0xEA;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Opcodes.ModOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		public ModOpcode(Machine m)
			:base(m)
		{
		}

		/// <summary>
		/// Returns the result of a % b
		/// </summary>
		public override void Execute()
		{
			// Check arguments in stack
			if ( this.Machine.ExecutionStack.Count < 2 ) {
				throw new EngineException( L18n.Get( L18n.Id.ErrMissingArguments ) );
			}

			// Take ops
			Variable op2 = this.Machine.ExecutionStack.Pop().SolveToVariable();
			Variable op1 = this.Machine.ExecutionStack.Pop().SolveToVariable();

			// Check ops
			if ( op1 == null
	  	      || !( op1.Type is Primitive ) )
			{
				throw new TypeMismatchException( ": op1: " + op1.Type );
			}

			if ( op2 == null
	   	      || !( op2.Type is Primitive ) )
			{
				throw new TypeMismatchException( ": op2: " + op2.Type );
			}

			// If the operands are references, dereference it
			var refOp1 = op1 as RefVariable;
			var refOp2 = op2 as RefVariable;

			if ( refOp1 != null ) {
				op1 = refOp1.PointedVble;
			}

			if ( refOp2 != null ) {
				op2 = refOp2.PointedVble;
			}

			// Now yes, do it
			BigInteger op2Value = op2.LiteralValue.GetValueAsInteger();

			if ( op2Value == 0 ) {
				throw new EngineException( "/0??" );
			}

			BigInteger modRes = op1.LiteralValue.GetValueAsInteger() % op2Value;

			// Store in the temp vble and end
			Variable result = new NoPlaceTempVariable( new IntLiteral( this.Machine, modRes ) );
			this.Machine.ExecutionStack.Push( result );
			return;
		}
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.ModOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.ModOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format( "[ModOpcode (0x{0,2:X}): rvalue(POP) % rvalue(POP)]",
                                    OpcodeValue );
        }
	}
}
