using System;

using CSim.Core.Functions;
using CSim.Core.Variables;
using CSim.Core.Literals;
using System.Collections.ObjectModel;

using CSim.Core.Exceptions;

namespace CSim.Core.FunctionLibrary {
	/// <summary>
	/// An standard function that erases a reserved block.
	/// </summary>
	public class Free: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "free";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.FunctionLibrary.Free"/> class.
		/// </summary>
		private Free(Machine m, Variable[] formalParams)
			:base( m, Name, null, formalParams )
		{
		}

		public override void Execute(RValue[] realParams)
		{
			int address = this.Machine.Memory.Max;
			Variable vble = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			var ptrVble = vble as PtrVariable;

			if ( ptrVble != null ) {
				address = ptrVble.LiteralValue.Value;
			}
			else
			if ( vble.Type == this.Machine.TypeSystem.GetIntType() ) {
				address = (int) vble.LiteralValue.Value;
			}
			else {
				throw new TypeMismatchException(
					L18n.Get( L18n.Id.LblPointer ).ToLower()
					+ " (" + L18n.Get( L18n.Id.ErrNotAPointer )
					+ ": " + this.Id + ")"
					);
			}

			this.Machine.TDS.DeleteBlk( address );

			Variable result = new TempVariable( Machine.TypeSystem.GetIntType() );
			result.LiteralValue = new IntLiteral( this.Machine, 0 );
			this.Machine.ExecutionStack.Push( result );
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Free Get(Machine m)
		{
			if ( instance == null ) {
				instance = new Free( m, new Variable[] {
										new VoidPtrVariable( new Id( @"ptr" ), m )
									} );
			}

			return instance;
		}

		private static Free instance = null;
	}
}

