using System;

using CSim.Core;
using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Literals;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// This is the sizeof function.
	/// Signature: int sizeof(x); // x can be anything
	/// </summary>
	public sealed class SizeOf: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "sizeof";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// <param name="m">The Machine this function will be executed in.</param>
		/// </summary>
		private SizeOf(Machine m)
			: base( m, Name, null, printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static SizeOf Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), CSim.Core.Types.Any.Get(), m )
				};

				instance = new SizeOf( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable vble = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			Variable result = new NoPlaceTempVariable( this.Machine.TypeSystem.GetIntType() );
			result.LiteralValue = new IntLiteral( this.Machine, vble.Type.Size );
			this.Machine.ExecutionStack.Push( result );
		}

		private static SizeOf instance = null;
		private static Variable[] printFormalParams;
	}
}
