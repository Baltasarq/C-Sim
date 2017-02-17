
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
    using CSim.Core.Variables;
    using CSim.Core.Types;
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
            : base( m, Name, m.TypeSystem.GetPtrType( Any.Get( m ) ), mallocFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Malloc Get(Machine m)
		{
			if ( instance == null ) {
				mallocFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetIntType() )
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
			var size = realParams[ 0 ].SolveToVariable();
			var result = new ArrayVariable(
								new Id( this.Machine, SymbolTable.GetNextMemoryBlockName() ),
								this.Machine.TypeSystem.GetCharType(),
								size.LiteralValue.GetValueAsLongInt()
			);

            this.Machine.TDS.Add( result );
			this.Machine.ExecutionStack.Push( result );
		}

		private static Malloc instance = null;
		private static Variable[] mallocFormalParams;
	}
}
