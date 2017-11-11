// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
    using System.Numerics;
	using CSim.Core.Exceptions;
	using CSim.Core.Functions;
	using CSim.Core.Literals;
	using CSim.Core.Variables;
    using CSim.Core.Types;

	/// <summary>
	/// This is the int cast.
	/// Signature: int int(x); // x can be anything numeric
	/// </summary>
	public sealed class IntCast: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "int";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private IntCast(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), intCastFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static IntCast Get(Machine m)
		{
			if ( instance == null ) {
				intCastFormalParams = new Variable[] {
                    new Variable( new Id( m, @"x" ), Any.Get( m ) )
				};

				instance = new IntCast( m );
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
				throw new TypeMismatchException(
                                        this.Machine.TypeSystem.GetIntType()
                                        + " != " + param.Type );
			}

			BigInteger value = param.LiteralValue.GetValueAsInteger();
			this.Machine.ExecutionStack.Push( Variable.CreateTempVariable(
                                                        this.Machine, value ) );
		}

		private static IntCast instance = null;
		private static Variable[] intCastFormalParams;
	}
}
