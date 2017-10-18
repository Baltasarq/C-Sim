
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;

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

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
			var toret = Variable.CreateTempVariable( this.Machine,
                                        this.Machine.Random.NextDouble() );
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

		/// <summary>
		/// The random engine to use in the Rand <see cref="Opcode"/>.
		/// </summary>
        public static System.Random Random = new System.Random();
		private static Rand instance;
	}
}

