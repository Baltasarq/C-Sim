
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Exceptions;
	using CSim.Core.Functions;
	using CSim.Core.Literals;
	using CSim.Core.Variables;
    using CSim.Core.Types;

	/// <summary>
	/// This is the ceil math function.
	/// Signature: double ceil(double x);
	/// </summary>
	public sealed class Ceil: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "ceil";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Ceil(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), ceilFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Ceil Get(Machine m)
		{
			if ( instance == null ) {
				ceilFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Ceil( m );
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
				throw new TypeMismatchException( param.ToString() );
			}

			double value = System.Convert.ToDouble( param.LiteralValue.Value );
			Variable result = new NoPlaceTempVariable( new DoubleLiteral( this.Machine, System.Math.Ceiling( value ) ) );
			this.Machine.ExecutionStack.Push( result );
		}

		private static Ceil instance = null;
		private static Variable[] ceilFormalParams;
	}
}


