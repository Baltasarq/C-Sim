namespace CSim.Core.FunctionLibrary {
	using System;
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
	using CSim.Core;

	/// <summary>
	/// This is the abs function.
	/// Signature: int abs(x);
	/// </summary>
	public sealed class Abs: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "abs";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Abs(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), absFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Abs Get(Machine m)
		{
			if ( instance == null ) {
				absFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetIntType(), m )
				};

				instance = new Abs( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable x = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetIntType() );

			if ( !( x.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( x.LiteralValue.ToString() + "?" );
			}

			result.LiteralValue = new IntLiteral(
									this.Machine,
									Math.Abs( Convert.ToInt32( x.LiteralValue.Value ) )
			);
			this.Machine.ExecutionStack.Push( result );
		}

		private static Abs instance = null;
		private static Variable[] absFormalParams;
	}
}
