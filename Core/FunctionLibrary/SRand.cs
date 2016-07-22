using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Exceptions;
using CSim.Core.Opcodes;

namespace CSim.Core.FunctionLibrary {
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
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private SRand(Machine m)
			: base( m, Name, null, printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static SRand Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetIntType(), m )
				};

				instance = new SRand( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );

			if ( param.Type != this.Machine.TypeSystem.GetIntType() ) {
				throw new TypeMismatchException( param.Name.Name );
			}

			this.Machine.SetRandomEngine( (int) param.LiteralValue.Value );
			this.Machine.ExecutionStack.Push( new TempVariable( this.Machine.TypeSystem.GetIntType() ) );
		}

		private static SRand instance = null;
		private static Variable[] printFormalParams;
	}
}
