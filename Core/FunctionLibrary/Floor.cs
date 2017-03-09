
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Exceptions;
	using CSim.Core.Functions;
	using CSim.Core.Literals;
	using CSim.Core.Variables;
    using CSim.Core.Types;

	/// <summary>
	/// This is the floor math function.
	/// Signature: double floor(double x);
	/// </summary>
	public sealed class Floor: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "floor";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Floor(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), floorFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Floor Get(Machine m)
		{
			if ( instance == null ) {
				floorFormalParams = new Variable[] {
					new PtrVariable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Floor( m );
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
			var param = realParams[ 0 ].SolveToVariable();

			if ( !( param.Type is Primitive ) ) {
				throw new TypeMismatchException( param.ToString() );
			}

			var value = System.Convert.ToDouble( param.LiteralValue.Value );
			var result = new NoPlaceTempVariable( new DoubleLiteral( this.Machine, System.Math.Floor( value ) ) );
			this.Machine.ExecutionStack.Push( result );
		}

		private static Floor instance = null;
		private static Variable[] floorFormalParams;
	}
}


