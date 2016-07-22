using System;
using System.Collections.ObjectModel;

using CSim.Core;
using CSim.Core.Exceptions;
using CSim.Core.FunctionLibrary;

namespace CSim.Core.Opcodes {
	/// <summary>
	/// Represents function calls.
	/// </summary>
	public class CallOpcode: Opcode {
		public static char OpcodeValue = (char) 0xE2;

		/// <summary>
		/// Function calls are defined by an id
		/// and a list of typed parameters.
		/// These are represented by local variables.
		/// </summary>
		/// <param name="id">The identifier of the function to call, as a string.</param>
        /// <param name="realParams">The parameters to pass to the function to call, as a vector.</param>
		public CallOpcode(Machine m, string id)
            : base( m )
		{
            if ( id != null ) {
                id = id.Trim();
            }

            if ( string.IsNullOrEmpty( id ) ) {
				throw new ArgumentException( "void id in fn. call" );
			}

			this.id = id;
		}

		/// <summary>
		/// Gets the identifier of the function to call.
		/// </summary>
		/// <value>The identifier, as a string.</value>
		public string Id {
			get {
				return this.id;
			}
		}

		/// <summary>
		/// Executes the function call.
		/// </summary>
		public override void Execute()
		{
			Function f = this.Machine.Api.Match( this.Id );

			if ( f != null ) {
				// Take params
				RValue[] args = new RValue[ f.FormalParams.Count ];

				for(int i = args.Length - 1; i >= 0; --i) {
					args[ i ] = this.Machine.TDS.SolveToVariable( this.Machine.ExecutionStack.Pop() );
				}

				// Check types
				for(int i = 0; i < args.Length; ++i) {
					CSim.Core.Type t1 = args[ i ].Type;
					CSim.Core.Type t2 = f.FormalParams[ i ].Type;

					if ( !t1.IsCompatibleWith( t2 ) ) {
						throw new TypeMismatchException(
							string.Format( L18n.Get( L18n.Id.ErrParamType ),
								i,
								f.FormalParams[ i ].Name.Name,
								id,
								t2.ToString(),
								t1.ToString()
							));
					}
				}


				// Execute
				f.Execute( args );
			}

			return;
		}

		private string id;
	}
}

