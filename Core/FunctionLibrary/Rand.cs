using System;

using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Literals;
using System.Collections.ObjectModel;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// An standard function that returns a random number.
	/// </summary>
	public class Rand: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "rand";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.FunctionLibrary.Rand"/> class.
		/// </summary>
		private Rand(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType() )
		{
		}

		public override void Execute(RValue[] realParams)
		{
			var toret = new TempVariable( this.Machine.TypeSystem.GetDoubleType() );
			toret.LiteralValue = new DoubleLiteral( this.Machine, this.Machine.Random.NextDouble() );

			this.Machine.ExecutionStack.Push( toret );
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Rand Get(Machine m)
		{
			if ( instance == null ) {
				instance = new Rand( m );
			}

			return instance;
		}

		private static Rand instance = null;
        public static Random Random = new Random();
	}
}

