namespace CSim.Core.FunctionLibrary {
	using System;
	using CSim.Core.Functions;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;
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
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Log(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), absFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Log Get(Machine m)
		{
			if ( instance == null ) {
				absFormalParams = new Variable[] {
					new Variable( new Id( @"x" ), m.TypeSystem.GetDoubleType(), m )
				};

				instance = new Log( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable x = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetDoubleType() );

			if ( !( x.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( x.LiteralValue.ToString() + "?" );
			}

			result.LiteralValue = new DoubleLiteral(
				this.Machine,
				Math.Log( Convert.ToDouble( x.LiteralValue.Value ) )
			);
			this.Machine.ExecutionStack.Push( result );
		}

		private static Log instance = null;
		private static Variable[] absFormalParams;
	}
}
