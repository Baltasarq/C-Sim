
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;
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
		/// Initializes a new instance of the <see cref="EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Pow(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), powFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Pow Get(Machine m)
		{
			if ( instance == null ) {
				powFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() ),
					new Variable( new Id( m, @"y" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Pow( m );
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
			Variable x = realParams[ 0 ].SolveToVariable();
			Variable y = realParams[ 1 ].SolveToVariable();

			if ( !( x.Type is Primitive ) ) {
				throw new TypeMismatchException(
                                this.Machine.TypeSystem.GetDoubleType()
                                + " != " + x.Type );
			}

			if ( !( y.Type is Primitive ) ) {
				throw new TypeMismatchException(
                                this.Machine.TypeSystem.GetDoubleType()
                                + " != " + y.Type );
			}

			var litResult = new DoubleLiteral(
				this.Machine,
				System.Math.Pow( x.LiteralValue.ToDouble(),
								 y.LiteralValue.ToDouble() )
			);
			this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litResult ) );
		}

		private static Pow instance = null;
		private static Variable[] powFormalParams;
	}
}
