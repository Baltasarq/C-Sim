
namespace CSim.Core.Opcodes {
    using CSim.Core.Exceptions;

	/// <summary>
	/// Represents function calls.
	/// </summary>
	public class CallOpcode: Opcode {
		/// <summary>
		/// The opcode's representing value.
		/// </summary>
		public static byte OpcodeValue = 0xE2;

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
		/// Executes the function call.
		/// </summary>
		public override void Execute()
		{
			Function f = this.Function;

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
        
        /// <summary>
        /// Gets the function to be called.
        /// </summary>
        /// <value>The <see cref="Function"/>.</value>
        public Function Function {
            get {
                if ( this.fn == null ) {
                    this.fn = this.Machine.API.Match( this.Id );
                    
                    if ( fn == null ) {
                        throw new InvalidIdException(
                                    L18n.Get( L18n.Id.ErrFunctionNotFound )
                                    + ": " + this.id );
                    }
                }
                
                return this.fn;
            }
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
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.CallOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.CallOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                            "[CallOpcode(0x{0,2:X}): Id={1}({2} x rvalue(POP)) ]",
                            OpcodeValue,
                            this.Id,
                            this.Function.FormalParams.Count );
        }

		private string id;
        private Function fn;
	}
}
