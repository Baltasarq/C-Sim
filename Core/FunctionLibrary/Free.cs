
namespace CSim.Core.FunctionLibrary {
	using CSim.Core.Functions;
	using CSim.Core.Types;
	using CSim.Core.Variables;
	using CSim.Core.Literals;
	using CSim.Core.Exceptions;

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

		/// <summary>
		/// Execute this <see cref="Function"/> with
		/// the specified parameters (<see cref="RValue"/>'s).
		/// </summary>
		/// <param name="realParams">The parameters.</param>
		public override void Execute(RValue[] realParams)
		{
			long address = this.Machine.Memory.Max;
			Variable vble = this.Machine.TDS.SolveToVariable( realParams[ 0 ] );
			var ptrVbleType = vble.Type as Ptr;

			if ( ptrVbleType != null ) {
				address = vble.LiteralValue.GetValueAsInt();
			}
			else
			if ( vble.Type == this.Machine.TypeSystem.GetIntType() ) {
				address = vble.LiteralValue.GetValueAsInt();
			}
			else {
				throw new TypeMismatchException(
					L18n.Get( L18n.Id.LblPointer ).ToLower()
					+ " (" + L18n.Get( L18n.Id.ErrNotAPointer )
					+ ": " + this.Id + ")"
					);
			}

			this.Machine.TDS.DeleteBlk( address );

			Variable result = new NoPlaceTempVariable( Machine.TypeSystem.GetIntType() );
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

