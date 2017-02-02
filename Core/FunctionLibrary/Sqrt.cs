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
	/// This is the sqrt function.
	/// Signature: double sqrt(double x);
	/// </summary>
	public sealed class Sqrt: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "sqrt";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Sqrt(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Sqrt Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), m.TypeSystem.GetDoubleType(), m )
				};

				instance = new Sqrt( m );
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
			Variable result = new NoPlaceTempVariable( this.Machine.TypeSystem.GetDoubleType() );
			result.LiteralValue = new DoubleLiteral( this.Machine, Math.Sqrt( x ) );

			this.Machine.ExecutionStack.Push( result );
		}

		private static Sqrt instance = null;
		private static Variable[] printFormalParams;
	}
}
