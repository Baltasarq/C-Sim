using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Opcodes;
using CSim.Core.Literals;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// This is the atof function.
	/// Signature: double atof(char * x);
	/// </summary>
	public sealed class Atof: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "atof";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Atof(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Atof Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), m.TypeSystem.GetCharType(), m )
				};

				instance = new Atof( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			double valueFromStr = Convert.ToDouble( param.LiteralValue.Value );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetDoubleType() );
			result.LiteralValue = new DoubleLiteral( this.Machine, valueFromStr );

			this.Machine.ExecutionStack.Push( result );
		}

		private static Atof instance = null;
		private static Variable[] printFormalParams;
	}
}
