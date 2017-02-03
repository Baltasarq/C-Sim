
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;

	/// <summary>
	/// This is the print function.
	/// Signature: void print(x); // x can be anything
	/// </summary>
	public sealed class Printf: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "printf";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Printf(Machine m)
			: base( m, Name, null, printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Printf Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), CSim.Core.Types.Any.Get(), m )
				};

				instance = new Printf( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable result = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			this.Machine.ExecutionStack.Push( result );
		}

		private static Printf instance = null;
		private static Variable[] printFormalParams;
	}
}
