// Prys CSim - Copyright (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
	using Functions;
    using Variables;
    using Exceptions;
    using Literals;
    using Types;
	using Core;

	/// <summary>
	/// This is the typed malloc function.
	/// Signature: T * _new(T[x]);
	/// </summary>
	public sealed class TypedMalloc: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "tmalloc";

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
            if ( !( countVble.Type is Primitive ) ) {
                throw new RuntimeException( string.Format( "size == {0}??", countVble ) );
            }

            if ( typeVble.Type != Types.TypeType.Get( this.Machine ) ) {
                throw new RuntimeException( string.Format( "type == {0}??", typeVble ) );
            }

            // Build
            var count = countVble.LiteralValue.GetValueAsInteger();
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
