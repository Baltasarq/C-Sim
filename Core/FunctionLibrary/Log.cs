namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;
	using CSim.Core;

	/// <summary>
	/// This is the log function.
	/// Signature: double log(double arg);
	/// </summary>
	public sealed class Log: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "log";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Log(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), logFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Log Get(Machine m)
		{
			if ( instance == null ) {
				logFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Log( m );
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
				throw new TypeMismatchException( x.LiteralValue + "?" );
			}

			var litResult = new DoubleLiteral(
				this.Machine,
				System.Math.Log( System.Convert.ToDouble( x.LiteralValue.Value ) )
			);
			this.Machine.ExecutionStack.Push( new NoPlaceTempVariable( litResult ) );
		}

		private static Log instance = null;
		private static Variable[] logFormalParams;
	}
}
