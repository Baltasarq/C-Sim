using System;

using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Literals;
using System.Collections.ObjectModel;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// An standard function that always returns zero.
	/// </summary>
	public class Zero: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "zero";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.FunctionLibrary.Zero"/> class.
		/// </summary>
		private Zero(Machine m)
			:base( m, Name, m.TypeSystem.GetIntType() )
		{
		}

		public override Variable Execute(ReadOnlyCollection<RValue> realParams)
		{
			var toret = new TempVariable( this.Machine.TypeSystem.GetIntType() );
			toret.LiteralValue = new IntLiteral( this.Machine, 0 );

			return toret;
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Zero Get(Machine m)
		{
			if ( instance == null ) {
				instance = new Zero( m );
			}

			return instance;
		}

		private static Zero instance = null;
	}
}

