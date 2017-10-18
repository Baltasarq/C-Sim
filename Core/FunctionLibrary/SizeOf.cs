
namespace CSim.Core.FunctionLibrary {
    using System.Numerics;
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
    using CSim.Core.Types;

	/// <summary>
	/// This is the sizeof function.
	/// Signature: int sizeof(x); // x can be anything
	/// </summary>
	public sealed class SizeOf: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "sizeof";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// <param name="m">The Machine this function will be executed in.</param>
		/// </summary>
		private SizeOf(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), sizeofFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static SizeOf Get(Machine m)
		{
			if ( instance == null ) {
				sizeofFormalParams = new Variable[] {
                    new Variable( new Id( m, @"x" ), Any.Get( m ) )
				};

				instance = new SizeOf( m );
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
            var argVble = realParams[ 0 ].SolveToVariable();
            var argType = argVble.Type as TypeType;

            BigInteger result = argVble.LiteralValue.GetValueAsInteger();
            if ( argType == null ) {
                result = argVble.Type.Size;
            }

			var litResult = new IntLiteral( this.Machine, result );
			this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litResult ) );
		}

		private static SizeOf instance;
		private static Variable[] sizeofFormalParams;
	}
}
