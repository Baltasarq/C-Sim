
namespace CSim.Core.FunctionLibrary {
    using CSim.Core.Exceptions;
	using CSim.Core.Functions;
	using CSim.Core.Variables;
    using CSim.Core.Literals;
    using CSim.Core.Types;

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
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetIntType() )
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
			Variable param = realParams[ 0 ].SolveToVariable();

			if ( !( param.Type is Primitive ) ) {
				throw new TypeMismatchException( 
                                        this.Machine.TypeSystem.GetIntType()
                                        + " != " + param.Type );
			}
            
            var litSeed = new IntLiteral( this.Machine, param.LiteralValue.GetValueAsInteger() );

			this.Machine.SetRandomEngine( litSeed.Value );
			this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litSeed ) );
		}

		private static SRand instance;
		private static Variable[] srandFormalParams;
	}
}
