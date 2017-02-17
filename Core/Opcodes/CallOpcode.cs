
namespace CSim.Core.Opcodes {
    using CSim.Core.Exceptions;

	/// <summary>
	/// Represents function calls.
	/// </summary>
	public class CallOpcode: Opcode {
		/// <summary>
		/// The opcode's representing value.
		/// </summary>
		public static char OpcodeValue = (char) 0xE2;

		/// <summary>
		/// Function calls are defined by an id
		/// and a list of typed parameters.
		/// These are represented by local variables.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/>.</param>
		/// <param name="id">The identifier of the function to call, as a string.</param>
		public CallOpcode(Machine m, string id)
            : base( m )
		{
            if ( id != null ) {
                id = id.Trim();
            }

            if ( string.IsNullOrEmpty( id ) ) {
				throw new System.ArgumentException( "void id in fn. call" );
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
			Function f = this.Machine.API.Match( this.Id );

			if ( f != null ) {
				// Take params
				RValue[] args = new RValue[ f.FormalParams.Count ];

				if ( this.Machine.ExecutionStack.Count < args.Length ) {
					throw new EngineException(
						L18n.Get( L18n.Id.ErrMissingArguments ) + ": " + this.Id
					);
				}

				for(int i = args.Length - 1; i >= 0; --i) {
					args[ i ] = this.Machine.ExecutionStack.Pop().SolveToVariable();
				}

				// Check types
				for(int i = 0; i < args.Length; ++i) {
                    var t1 = args[ i ].Type;
					var t2 = f.FormalParams[ i ].Type;

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

