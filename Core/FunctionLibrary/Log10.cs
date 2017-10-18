namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;
	using CSim.Core;

	/// <summary>
	/// This is the log10 function.
	/// Signature: double log10(double arg);
	/// </summary>
	public sealed class Log10: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "log10";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Log10(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), log10FormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Log10 Get(Machine m)
		{
			if ( instance == null ) {
				log10FormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Log10( m );
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
				System.Math.Log10( x.LiteralValue.ToDouble() )
			);
            
			this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litResult ) );
		}

		private static Log10 instance = null;
		private static Variable[] log10FormalParams;
	}
}
