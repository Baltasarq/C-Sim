
namespace CSim.Core.FunctionLibrary {
	using System;

	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;

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
		/// Initializes a new instance of the <see cref="CSim.Core.FunctionLibrary.Atof"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Atof(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), atofFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Atof Get(Machine m)
		{
			if ( instance == null ) {
				atofFormalParams = new Variable[] {
					new PtrVariable( new Id( @"x" ), m.TypeSystem.GetCharType(), m )
				};

				instance = new Atof( m );
			}

			return instance;
		}

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			double valueFromStr = Convert.ToDouble( param.LiteralValue.Value );
			Variable result = new NoPlaceTempVariable( this.Machine.TypeSystem.GetDoubleType() );
			result.LiteralValue = new DoubleLiteral( this.Machine, valueFromStr );

			this.Machine.ExecutionStack.Push( result );
		}

		private static Atof instance = null;
		private static Variable[] atofFormalParams;
	}
}
