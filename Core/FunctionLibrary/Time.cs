using System;

using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Literals;
using System.Collections.ObjectModel;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// An standard function that returns the time.
	/// </summary>
	public class Time: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "time";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.FunctionLibrary.Zero"/> class.
		/// </summary>
		private Time(Machine m)
			:base( m, Name, m.TypeSystem.GetIntType() )
		{
		}

		public override void Execute(RValue[] realParams)
		{
			var result = new NoPlaceTempVariable( this.Machine.TypeSystem.GetIntType() );
			result.LiteralValue = new IntLiteral(
				this.Machine,
				Convert.ToInt64( Math.Ceiling( DateTime.Now.TimeOfDay.TotalSeconds ) ) );

			this.Machine.ExecutionStack.Push( result );
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Time Get(Machine m)
		{
			if ( instance == null ) {
				instance = new Time( m );
			}

			return instance;
		}

		private static Time instance = null;
	}
}

