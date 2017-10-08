
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;

	/// <summary>
	/// This is the atoi function.
	/// Signature: int atoi(char * x);
	/// </summary>
	public sealed class Atoi: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "atoi";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Atoi(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), atoiFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Atoi Get(Machine m)
		{
			if ( instance == null ) {
				atoiFormalParams = new Variable[] {
					new PtrVariable( new Id( m, @"x" ), m.TypeSystem.GetCharType() )
				};

				instance = new Atoi( m );
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
			AType pchar_t = this.Machine.TypeSystem.GetPtrType(
                                        this.Machine.TypeSystem.GetCharType() );
            var param = realParams[ 0 ].SolveToVariable();
            
            if ( pchar_t != param.Type ) {
                throw new Exceptions.TypeMismatchException(
                                              pchar_t + " != " + param.Type );
            }
            
			var valueFromStr = param.LiteralValue.Value.ToBigInteger();
			var result = new NoPlaceTempVariable( new IntLiteral( this.Machine, valueFromStr ) );
			this.Machine.ExecutionStack.Push( result );
		}

		private static Atoi instance = null;
		private static Variable[] atoiFormalParams;
	}
}
