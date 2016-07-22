namespace CSim.Core.FunctionLibrary {
	using System;
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
	using CSim.Core;

	/// <summary>
	/// This is the exp function.
	/// Signature: double exp(double arg);
	/// </summary>
	public sealed class Exp: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "exp";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Exp(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), absFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Exp Get(Machine m)
		{
			if ( instance == null ) {
				absFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetIntType(), m )
				};

				instance = new Exp( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable x = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetDoubleType() );

			if ( !( x.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( x.LiteralValue.ToString() + "?" );
			}

			result.LiteralValue = new DoubleLiteral(
				this.Machine,
				Math.Exp( Convert.ToDouble( x.LiteralValue.Value ) )
			);
			this.Machine.ExecutionStack.Push( result );
		}

		private static Exp instance = null;
		private static Variable[] absFormalParams;
	}
}
