
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;

    using System.Numerics;
    
	/// <summary>
	/// An standard function that always returns zero.
	/// </summary>
	public class Zero: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "zero";

		/// <summary>
		/// Initializes a new instance of the <see cref="Zero"/> class.
		/// </summary>
		private Zero(Machine m)
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
			this.Machine.ExecutionStack.Push(
                                Variable.CreateTempVariable( this.Machine, 
                                                             BigInteger.Zero ) );
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

