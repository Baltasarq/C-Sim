
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core;

	/// <summary>
	/// This is the malloc function.
	/// Signature: void * malloc(x);
	/// </summary>
	public sealed class Malloc: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "malloc";

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Malloc(Machine m)
			: base( m, Name, m.TypeSystem.GetPtrType( CSim.Core.Types.Any.Get() ), mallocFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Malloc Get(Machine m)
		{
			if ( instance == null ) {
				mallocFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetIntType(), m )
				};

				instance = new Malloc( m );
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
			Variable size = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );

			Variable result = this.Machine.TDS.AddArray(
								new Id( SymbolTable.GetNextMemoryBlockName() ),
								this.Machine.TypeSystem.GetCharType(),
								size.LiteralValue.GetValueAsInt()
			);

			this.Machine.ExecutionStack.Push( result );
		}

		private static Malloc instance = null;
		private static Variable[] mallocFormalParams;
	}
}
