
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Exceptions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;

	/// <summary>
	/// This is the sin function.
	/// Signature: double sin(double x);
	/// </summary>
	public sealed class Sin: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "sin";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Sin(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), sinFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Sin Get(Machine m)
		{
			if ( instance == null ) {
				sinFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Sin( m );
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
			Variable param = realParams[ 0 ].SolveToVariable();

			if ( !( param.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( param.LiteralValue + "?" );
			}

			double x = System.Convert.ToDouble( param.LiteralValue.Value );
			var litResult = new DoubleLiteral( this.Machine, System.Math.Sin( x ) );
			this.Machine.ExecutionStack.Push( new NoPlaceTempVariable( litResult ) );
		}

		private static Sin instance = null;
		private static Variable[] sinFormalParams;
	}
}
