namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
	using CSim.Core;

	/// <summary>
	/// This is the fmod function.
	/// Signature: double fmod(double x, double y);
	/// </summary>
	public sealed class Mod: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "fmod";

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Mod(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), modFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Mod Get(Machine m)
		{
			if ( instance == null ) {
				modFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetDoubleType(), m ),
					new Variable( new Id( @"y" ), m.TypeSystem.GetDoubleType(), m )
				};

				instance = new Mod( m );
			}

			return instance;
		}

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
			Variable x = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			Variable y = this.Machine.TDS.SolveToVariable( realParams[ 1 ] );
			Variable result = new NoPlaceTempVariable( this.Machine.TypeSystem.GetDoubleType() );

			if ( !( x.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( x.LiteralValue + "?" );
			}

			if ( !( y.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( y.LiteralValue + "?" );
			}

			result.LiteralValue = new DoubleLiteral(
				this.Machine,
				System.Convert.ToDouble( x.LiteralValue.Value )
							% System.Convert.ToDouble( y.LiteralValue.Value )
			);
			this.Machine.ExecutionStack.Push( result );
		}

		private static Mod instance = null;
		private static Variable[] modFormalParams;
	}
}
