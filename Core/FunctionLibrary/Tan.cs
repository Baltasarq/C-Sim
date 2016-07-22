using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Functions;
using CSim.Core.Exceptions;
using CSim.Core.Variables;
using CSim.Core.Opcodes;
using CSim.Core.Literals;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// This is the tan function.
	/// Signature: double tan(double x);
	/// </summary>
	public sealed class Tan: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "tan";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Tan(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Tan Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), m.TypeSystem.GetDoubleType(), m )
				};

				instance = new Tan( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );

			if ( !( param.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( param.LiteralValue.ToString() + "?" );
			}

			double x = Convert.ToDouble( param.LiteralValue.Value );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetDoubleType() );
			result.LiteralValue = new DoubleLiteral( this.Machine, Math.Tan( x ) );

			this.Machine.ExecutionStack.Push( result );
		}

		private static Tan instance = null;
		private static Variable[] printFormalParams;
	}
}
