namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Variables;
	using CSim.Core.Types.Primitives;
	using CSim.Core.Literals;
	using CSim.Core.Types;
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
				throw new TypeMismatchException( ": op1" );
			}

			if ( op2 == null
			  || !( op2.Type.IsArithmetic() ) )
			{
				throw new TypeMismatchException( ": op2" );
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
			long sum = Convert.ToInt64( op1.LiteralValue.Value ) + Convert.ToInt64( op2.LiteralValue.Value );

			// Store in the temp vble and end
			Variable result = new NoPlaceTempVariable( new IntLiteral( this.Machine, sum ) );
			this.Machine.ExecutionStack.Push( result );
			return;
		}
	}
}
