
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Exceptions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
    using CSim.Core.Types;

	/// <summary>
	/// This is the cos function.
	/// Signature: double cos(double x);
	/// </summary>
	public sealed class Cos: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "cos";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Cos(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), cosFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Cos Get(Machine m)
		{
			if ( instance == null ) {
				cosFormalParams = new Variable[] {
					new PtrVariable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Cos( m );
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
			var param = realParams[ 0 ].SolveToVariable();

			if ( !( param.Type is Primitive ) ) {
				throw new TypeMismatchException(
                                    this.Machine.TypeSystem.GetDoubleType()
                                    + " != " + param.Type );
			}

			var x = param.LiteralValue.Value.ToDouble();
			var litResult = new DoubleLiteral( this.Machine, System.Math.Cos( x ) );
			this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litResult ) );
		}

		private static Cos instance = null;
		private static Variable[] cosFormalParams;
	}
}
