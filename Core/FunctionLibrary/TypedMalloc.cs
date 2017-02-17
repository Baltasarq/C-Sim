
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
    using CSim.Core.Variables;
    using CSim.Core.Literals;
    using CSim.Core.Types;
	using CSim.Core;

	/// <summary>
	/// This is the malloc function.
	/// Signature: void * malloc(x);
	/// </summary>
	public sealed class TypedMalloc: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "_new";

		/// <summary>
		/// Initializes a new instance of the <see cref="EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private TypedMalloc(Machine m)
            : base( m, Name, m.TypeSystem.GetPtrType( Any.Get( m ) ), typedMallocFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
        public static TypedMalloc Get(Machine m)
		{
			if ( instance == null ) {
				typedMallocFormalParams = new Variable[] {
                    new TypeVariable( new Id( m, "type" ) ),
					new Variable( new Id( m, @"size" ), m.TypeSystem.GetIntType() )
				};

				instance = new TypedMalloc( m );
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
            var countVble = realParams[ 0 ].SolveToVariable();
            var typeVble = realParams[ 1 ].SolveToVariable();

            // Chk
            if ( !countVble.Type.IsArithmetic() ) {
                throw new EngineException( string.Format( "size == {0}??", countVble ) );
            }

            if ( typeVble.Type != Types.TypeType.Get( this.Machine ) ) {
                throw new EngineException( string.Format( "type == {0}??", typeVble ) );
            }

            // Build
            var count = countVble.LiteralValue.GetValueAsLongInt();
            var type = ( (TypeLiteral) typeVble.Value ).Value;
            string blkId = SymbolTable.GetNextMemoryBlockName();

            
			var result = new ArrayVariable( new Id( this.Machine, blkId ), type, count );
            this.Machine.TDS.Add( result );
			this.Machine.ExecutionStack.Push( result );
		}

        private static TypedMalloc instance;
		private static Variable[] typedMallocFormalParams;
	}
}
