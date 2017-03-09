
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Exceptions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
    using CSim.Core.Types;

	/// <summary>
	/// This is the tan function.
	/// Signature: double tan(double x);
	/// </summary>
	public sealed class Tan: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "tan";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Tan(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), tanFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Tan Get(Machine m)
		{
			if ( instance == null ) {
				tanFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Tan( m );
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

			if ( !( param.Type is Primitive ) ) {
				throw new TypeMismatchException( param.LiteralValue + "?" );
			}

			double x = System.Convert.ToDouble( param.LiteralValue.Value );
			var litResult = new DoubleLiteral( this.Machine, System.Math.Tan( x ) );
			this.Machine.ExecutionStack.Push( new NoPlaceTempVariable( litResult ) );
		}

		private static Tan instance = null;
		private static Variable[] tanFormalParams;
	}
}
