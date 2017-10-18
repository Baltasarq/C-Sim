
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;

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

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
			var result = Variable.CreateTempVariable(
				this.Machine,
					System.Math.Ceiling(
						System.DateTime.Now.TimeOfDay.TotalSeconds )
                    .ToBigInteger() );

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

