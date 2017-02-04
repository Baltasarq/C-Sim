using CSim.Core.Types.Primitives;
namespace CSim.Core.Opcodes {
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;

	/// <summary>
	/// The opcode for additions: '+'
	/// </summary>
	public class AddOpcode: Opcode {
		/// <summary>An identifier for this opcode.</summary>
		public const char OpcodeValue = (char) 0xEA;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Opcodes.AddOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		public AddOpcode(Machine m)
			:base(m)
		{
		}

		/// <summary>
		/// Returns the result of a + b
		/// </summary>
		public override void Execute()
		{
			// Check arguments in stack
			if ( this.Machine.ExecutionStack.Count < 2 ) {
				throw new EngineException( L18n.Get( L18n.Id.ErrMissingArguments ) );
			}

			// Take ops
			Variable op1 = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );
			Variable op2 = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );

			// Check ops
			if ( op1 == null
			  || !( op1.Type.IsArithmetic() ) )
			{
				throw new TypeMismatchException( ": op1: " + op1.Type );
			}

			if ( op2 == null
			  || !( op2.Type.IsArithmetic() ) )
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
				                              System.Convert.ToDouble( op1.LiteralValue.Value )
				                              + System.Convert.ToDouble( op2.LiteralValue.Value ) );
			} else {
				litResult = new IntLiteral( this.Machine,
				                           op1.LiteralValue.GetValueAsInt()
				                           + op2.LiteralValue.GetValueAsInt() );
			}

			// Store in the temp vble and end
			Variable result = new NoPlaceTempVariable( litResult );
			this.Machine.ExecutionStack.Push( result );
			return;
		}
	}
}
