
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Exceptions;

	/// <summary>
	/// This is the srand function.
	/// Signature: void srand(x); // x can be anything
	/// </summary>
	public sealed class SRand: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "srand";

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private SRand(Machine m)
			: base( m, Name, null, srandFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static SRand Get(Machine m)
		{
			if ( instance == null ) {
				srandFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetIntType(), m )
				};

				instance = new SRand( m );
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
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );

			if ( param.Type != this.Machine.TypeSystem.GetIntType() ) {
				throw new TypeMismatchException( param.Name.Name );
			}

			this.Machine.SetRandomEngine( param.LiteralValue.GetValueAsInt() );
			this.Machine.ExecutionStack.Push( new NoPlaceTempVariable( this.Machine.TypeSystem.GetIntType() ) );
		}

		private static SRand instance = null;
		private static Variable[] srandFormalParams;
	}
}
