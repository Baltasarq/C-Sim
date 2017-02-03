
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Exceptions;
	using CSim.Core.Functions;
	using CSim.Core.Literals;
	using CSim.Core.Variables;

	/// <summary>
	/// This is the char cast.
	/// Signature: char char(x); // x can be anything numeric
	/// </summary>
	public sealed class CharCast: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "char";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private CharCast(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), charCastFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static CharCast Get(Machine m)
		{
			if ( instance == null ) {
				charCastFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), CSim.Core.Types.Any.Get(), m )
				};

				instance = new CharCast( m );
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

			if ( !( param.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( param.ToString() );
			}

			char value = System.Convert.ToChar( param.LiteralValue.Value );
			Variable result = new NoPlaceTempVariable( this.Machine.TypeSystem.GetCharType() );
			result.LiteralValue = new CharLiteral( this.Machine, value );
			this.Machine.ExecutionStack.Push( result );
		}

		private static CharCast instance = null;
		private static Variable[] charCastFormalParams;
	}
}
