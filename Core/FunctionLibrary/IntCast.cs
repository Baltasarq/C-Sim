﻿
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Exceptions;
	using CSim.Core.Functions;
	using CSim.Core.Literals;
	using CSim.Core.Variables;

	/// <summary>
	/// This is the int cast.
	/// Signature: int int(x); // x can be anything numeric
	/// </summary>
	public sealed class IntCast: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "int";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private IntCast(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), intCastFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static IntCast Get(Machine m)
		{
			if ( instance == null ) {
				intCastFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), CSim.Core.Types.Any.Get(), m )
				};

				instance = new IntCast( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );

			if ( !( param.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( param.ToString() );
			}

			long value = param.LiteralValue.GetValueAsInt();
			Variable result = new NoPlaceTempVariable( this.Machine.TypeSystem.GetIntType() );
			result.LiteralValue = new IntLiteral( this.Machine, value );
			this.Machine.ExecutionStack.Push( result );
		}

		private static IntCast instance = null;
		private static Variable[] intCastFormalParams;
	}
}
