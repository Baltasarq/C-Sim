namespace CSim.Core.Opcodes {
	using System;

	using CSim.Core.Variables;
	using CSim.Core.Types.Primitives;
	using CSim.Core.Literals;
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	public class DivOpcode: Opcode {
		public const char OpcodeValue = (char) 0xEA;

		public DivOpcode(Machine m)
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
			long op1Value = ( (long) op1.LiteralValue.Value );

			if ( op1Value == 0 ) {
				throw new EngineException( "/0??" );
			}

			long divRes = ( (long) op2.LiteralValue.Value ) / op1Value;

			// Store in the temp vble and end
			Variable result = new NoPlaceTempVariable( new IntLiteral( this.Machine, divRes ) );
			this.Machine.ExecutionStack.Push( result );
			return;
		}
	}
}
