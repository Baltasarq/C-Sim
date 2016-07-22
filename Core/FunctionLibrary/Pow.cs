namespace CSim.Core.FunctionLibrary {
	using System;
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
	using CSim.Core;

	/// <summary>
	/// This is the fmod function.
	/// Signature: double pow(double x, double y);
	/// </summary>
	public sealed class Pow: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "pow";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Pow(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), absFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Pow Get(Machine m)
		{
			if ( instance == null ) {
				absFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetDoubleType(), m ),
					new Variable( new Id( @"y" ), m.TypeSystem.GetDoubleType(), m )
				};

				instance = new Pow( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable x = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			Variable y = this.Machine.TDS.SolveToVariable( realParams[ 1 ] );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetDoubleType() );

			if ( !( x.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( x.LiteralValue.ToString() + "?" );
			}

			if ( !( y.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( y.LiteralValue.ToString() + "?" );
			}

			result.LiteralValue = new DoubleLiteral(
				this.Machine,
				Convert.ToDouble(
					Math.Pow( 	Convert.ToDouble( x.LiteralValue.Value ),
								Convert.ToDouble( y.LiteralValue.Value ) ) )
			);
			this.Machine.ExecutionStack.Push( result );
		}

		private static Pow instance = null;
		private static Variable[] absFormalParams;
	}
}
