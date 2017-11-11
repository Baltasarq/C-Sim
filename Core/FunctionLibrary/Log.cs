// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
    using CSim.Core.Types;
	using CSim.Core;

	/// <summary>
	/// This is the log function.
	/// Signature: double log(double arg);
	/// </summary>
	public sealed class Log: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "log";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Log(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), logFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Log Get(Machine m)
		{
			if ( instance == null ) {
				logFormalParams = new Variable[] {
					new Variable( new Id( m, @"x" ), m.TypeSystem.GetDoubleType() )
				};

				instance = new Log( m );
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

			var litResult = new DoubleLiteral(
				this.Machine,
				System.Math.Log( x.LiteralValue.ToDouble() )
			);
            
			this.Machine.ExecutionStack.Push(
                                    Variable.CreateTempVariable( litResult ) );
		}

		private static Log instance = null;
		private static Variable[] logFormalParams;
	}
}
