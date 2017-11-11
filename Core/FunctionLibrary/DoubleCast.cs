// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>


namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Exceptions;
	using CSim.Core.Functions;
	using CSim.Core.Literals;
	using CSim.Core.Variables;
    using CSim.Core.Types;

	/// <summary>
	/// This is the double cast.
	/// Signature: double double(x); // x can be anything numeric
	/// </summary>
	public sealed class DoubleCast: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "double";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private DoubleCast(Machine m)
			: base( m, Name, m.TypeSystem.GetDoubleType(), doubleCastFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static DoubleCast Get(Machine m)
		{
			if ( instance == null ) {
				doubleCastFormalParams = new Variable[] {
                    new Variable( new Id( m, @"x" ), CSim.Core.Types.Any.Get( m ) )
				};

				instance = new DoubleCast( m );
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
			Variable param = realParams[ 0 ].SolveToVariable();

			if ( !( param.Type is Primitive ) ) {
				throw new TypeMismatchException( param.ToString() );
			}

			double value = param.LiteralValue.Value.ToDouble();
			var result = Variable.CreateTempVariable( this.Machine, value );
			this.Machine.ExecutionStack.Push( result );
		}

		private static DoubleCast instance = null;
		private static Variable[] doubleCastFormalParams;
	}
}


