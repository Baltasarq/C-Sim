namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;
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
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Exp(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), expFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Exp Get(Machine m)
		{
			if ( instance == null ) {
				expFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetIntType() )
				};

				instance = new Exp( m );
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

			if ( !( x.Type is Primitive ) ) {
				throw new TypeMismatchException(
                                this.Machine.TypeSystem.GetDoubleType()
                                + " != " + x.Type );
			}

			var litResult = new DoubleLiteral(
				this.Machine,
				System.Math.Exp( x.LiteralValue.Value.ToDouble() )
			);
            
			this.Machine.ExecutionStack.Push( new NoPlaceTempVariable( litResult ) );
		}

		private static Exp instance = null;
		private static Variable[] expFormalParams;
	}
}
