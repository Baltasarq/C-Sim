// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.FunctionLibrary {
	using Functions;
    using Literals;
    using Types;
    
    using System;

	/// <summary>
	/// This is the print function.
	/// Signature: void print(x); // x can be anything
	/// </summary>
	public sealed class Printf: EmbeddedFunction {
		/// <summary>
		/// The identifier for the function.
		/// </summary>
		public const string Name = "printf";

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Functions.EmbeddedFunction"/> class.
		/// This is not intended to be used directly.
		/// </summary>
		private Printf(Machine m)
			: base( m, Name, m.TypeSystem.GetIntType(), printFormalParams )
		{
		}

		/// <summary>
		/// Returns the only instance of this function.
		/// </summary>
		public static Printf Get(Machine m)
		{
			if ( instance == null ) {
                var ellipsis_id = new Id( m, @"x" );
                ellipsis_id.SetIdWithoutChecks( "..." );
                
				printFormalParams = new Variable[] {
                    new Variable( ellipsis_id, Any.Get( m ) )
				};

				instance = new Printf( m );
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
            if ( realParams.Length > 0 ) {
	            var param0 = realParams[ 0 ].SolveToVariable();
	            AType pchar_t = this.Machine.TypeSystem.GetPCharType();
                string data;
	            
	            if ( param0.Type == pchar_t ) {
                    var prms = new RValue[ Math.Max( 0, realParams.Length - 1 ) ];
                    Array.Copy( realParams, 1, prms, 0, prms.Length );
                    data = SPrintf.FormatParams( param0, prms );
	            } else {
                    data = param0.LiteralValue.Value.ToString();
	            }

                this.Machine.ExecutionStack.Push(
                    new IntLiteral( this.Machine, data.Length ).SolveToVariable()
                );
                this.Machine.Outputter( data );
            }
            
            return;
		}
        
		private static Printf instance;
		private static Variable[] printFormalParams;
	}
}
