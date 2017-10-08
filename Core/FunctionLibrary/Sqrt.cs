
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Exceptions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
    using CSim.Core.Types;

	/// <summary>
	/// This is the sqrt function.
	/// Signature: double sqrt(double x);
	/// </summary>
	public sealed class Sqrt: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "sqrt";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Sqrt(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), sqrtFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Sqrt Get(Machine m)
		{
			if ( instance == null ) {
				sqrtFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Sqrt( m );
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
				throw new TypeMismatchException(
                                this.Machine.TypeSystem.GetDoubleType()
                                + " != " + param.Type );
			}

			double x = param.LiteralValue.ToDouble();
			var litResult = new DoubleLiteral( this.Machine, System.Math.Sqrt( x ) );
			this.Machine.ExecutionStack.Push( new NoPlaceTempVariable( litResult ) );
		}

		private static Sqrt instance = null;
		private static Variable[] sqrtFormalParams;
	}
}
