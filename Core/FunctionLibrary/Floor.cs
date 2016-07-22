using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Exceptions;
using CSim.Core.Functions;
using CSim.Core.Literals;
using CSim.Core.Variables;
using CSim.Core.Opcodes;

namespace CSim.Core.FunctionLibrary {
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
		/// Initializes a new instance of the <see cref="CSim.EmbeddedFunction"/> class.
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
					new PtrVariable( new Id( @"x" ), m.TypeSystem.GetDoubleType(), m )
				};

				instance = new Floor( m );
			}

			return instance;
		}

		public override void Execute(RValue[] realParams)
		{
			Variable param = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );

			if ( !( param.Type.IsArithmetic() ) ) {
				throw new TypeMismatchException( param.ToString() );
			}

			double value = Convert.ToDouble( param.LiteralValue.Value );
			Variable result = new TempVariable( this.Machine.TypeSystem.GetDoubleType() );
			result.LiteralValue = new DoubleLiteral( this.Machine, Math.Floor( value ) );
			this.Machine.ExecutionStack.Push( result );
		}

		private static Floor instance = null;
		private static Variable[] floorFormalParams;
	}
}


