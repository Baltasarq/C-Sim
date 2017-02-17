
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
    using CSim.Core.Types;

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
                    new Variable( new Id( m, @"x" ), Any.Get( m ) )
				};

				instance = new Printf( m );
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
			this.Machine.ExecutionStack.Push( realParams[ 0 ].SolveToVariable() );
		}

		private static Printf instance = null;
		private static Variable[] printFormalParams;
	}
}
