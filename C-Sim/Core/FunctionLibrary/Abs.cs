// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
    using System.Numerics;
	using CSim.Core.Functions;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;
	using CSim.Core;

	/// <summary>
	/// This is the abs function.
	/// Signature: int abs(x);
	/// </summary>
	public sealed class Abs: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "abs";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Abs(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), absFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Abs Get(Machine m)
		{
			if ( instance == null ) {
				absFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetIntType() )
				};

				instance = new Abs( m );
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
			Variable x = realParams[ 0 ].SolveToVariable();

			if ( !( x.Type is Primitive ) ) {
				throw new TypeMismatchException(
                                this.Machine.TypeSystem.GetDoubleType()
                                + " != " + x.Type );
            }

			Variable result = Variable.CreateTempVariable(
								this.Machine,
								BigInteger.Abs( x.LiteralValue.ToBigInteger() )
            );
            
			this.Machine.ExecutionStack.Push( result );
		}

		private static Abs instance;
		private static Variable[] absFormalParams;
	}
}
