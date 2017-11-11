// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
    using System.Numerics;
    
	using CSim.Core.Functions;
	using CSim.Core.Types;
	using CSim.Core.Variables;
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
			Variable vble = realParams[ 0 ].SolveToVariable();
            
            if ( !( vble.Type is Ptr )
              && !( vble.Type is Primitive ) )
            {
                throw new TypeMismatchException( 
                    L18n.Get( L18n.Id.LblPointer ).ToLower()
                    + " (" + L18n.Get( L18n.Id.ErrNotAPointer )
                    + ": " + vble.Name.Text + ")" );
            }
            
			this.Machine.TDS.DeleteBlk( vble.Value.ToBigInteger() );
			this.Machine.ExecutionStack.Push(
                Variable.CreateTempVariable( this.Machine, BigInteger.Zero ) );
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Free Get(Machine m)
		{
			if ( instance == null ) {
				instance = new Free( m,
                                     new Variable[] {
								            new VoidPtrVariable(
                                                    new Id( m, @"ptr" ) )
                                     });
			}

			return instance;
		}

		private static Free instance = null;
	}
}

