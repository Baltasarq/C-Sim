
namespace CSim.Core.Opcodes {
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;

	/// <summary>
	/// The Sub opcode, allowing operations such as 5-4.
	/// </summary>
	public class SubOpcode: Opcode {
		/// <summary>The opcode's representing value.</summary>
		public const byte OpcodeValue = 0xEA;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Opcodes.SubOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		public SubOpcode(Machine m)
			:base(m)
		{
		}

		/// <summary>
		/// Returns the result of a - b
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
			Literal litResult;

			if ( op1.Type is Core.Types.Primitives.Double
		      || op2.Type is Core.Types.Primitives.Double )
			{
				litResult = new DoubleLiteral( this.Machine,
				                              op1.LiteralValue.ToDouble()
				                              - op2.LiteralValue.ToDouble() );
			} else {
				litResult = new IntLiteral( this.Machine,
				                           op1.LiteralValue.GetValueAsInteger()
				                           - op2.LiteralValue.GetValueAsInteger() );
			}

			// Store in the temp vble and end
			this.Machine.ExecutionStack.Push( new NoPlaceTempVariable( litResult ) );
			return;
		}
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.SubOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.SubOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format( "[SubOpcode (0x{0,2:X}): rvalue(POP) - rvalue(POP)]",
                                    OpcodeValue );
        }
	}
}
