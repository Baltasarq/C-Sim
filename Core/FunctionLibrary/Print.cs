using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Opcodes;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// This is the print function.
	/// Signature: void print(x); // x can be anything
	/// </summary>
	public sealed class Print: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "print";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Print(Machine m)
			: base( m, Name, null, printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Print Get(Machine m)
		{
			if ( instance == null ) {
				printFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), CSim.Core.Types.Any.Get(), m )
				};

				instance = new Print( m );
			}

			return instance;
		}

		public override Variable Execute(ReadOnlyCollection<RValue> realParams)
		{
			return this.Machine.TDS.LookForRValue( null, realParams[ 0 ] );
		}

		private static Print instance = null;
		private static Variable[] printFormalParams;
	}
}
