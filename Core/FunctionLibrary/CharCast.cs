using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Exceptions;
using CSim.Core.Functions;
using CSim.Core.Literals;
using CSim.Core.Variables;
using CSim.Core.Opcodes;

namespace CSim.Core.FunctionLibrary {
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
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private CharCast(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), intCastFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static CharCast Get(Machine m)
		{
			if ( instance == null ) {
				intCastFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), CSim.Core.Types.Any.Get(), m )
				};

				instance = new CharCast( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );

			if ( !( param.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( param.ToString() );
			}

			char value = Convert.ToChar( param.LiteralValue.Value );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetCharType() );
			result.LiteralValue = new CharLiteral( this.Machine, value );
			this.Machine.ExecutionStack.Push( result );
		}

		private static CharCast instance = null;
		private static Variable[] intCastFormalParams;
	}
}
