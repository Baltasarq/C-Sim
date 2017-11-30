// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

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
        /// <param name="numArgs">The number of parameters for this function call.</param>
		public CallOpcode(Machine m, string id, int numArgs)
            : base( m )
		{
            if ( id != null ) {
                id = id.Trim();
            }

            if ( string.IsNullOrEmpty( id ) ) {
				throw new System.ArgumentException( "void id in fn. call" );
			}

			this.Id = id;
            this.NumArgs = numArgs;
		}

		/// <summary>
		/// Executes the function call.
		/// </summary>
		public override void Execute()
		{
			Function f = this.Function;
            int numArgs = f.FormalParams.Count;
            
            // Decide how many parameters
            if ( numArgs > 0
              && f.FormalParams[ 0 ].Name.Text == "..." )
            {
                numArgs = this.NumArgs;
            }
            else
            if ( this.NumArgs != numArgs ) {
                throw new RuntimeException(
                    string.Format(
                        L10n.Get( L10n.Id.ErrParamCount ),
                        this.Id, this.NumArgs, f.FormalParams.Count )
                );
            }

			// Take params
			RValue[] args = new RValue[ numArgs ];
            
			if ( this.Machine.ExecutionStack.Count < numArgs ) {
				throw new RuntimeException(
					L10n.Get( L10n.Id.ErrMissingArguments ) + ": " + this.Id
				);
			}

			for(int i = args.Length - 1; i >= 0; --i) {
				args[ i ] = this.Machine.ExecutionStack.Pop().SolveToVariable();
			}

			// Check types
            if ( this.NumArgs == f.FormalParams.Count ) {
				for(int i = 0; i < args.Length; ++i) {
	                var t1 = args[ i ].Type;
					var t2 = f.FormalParams[ i ].Type;
	
					if ( !t1.IsCompatibleWith( t2 ) ) {
						throw new TypeMismatchException(
							string.Format( L10n.Get( L10n.Id.ErrParamType ),
								i,
								f.FormalParams[ i ].Name.Text,
								this.Id,
								t2.ToString(),
								t1.ToString()
							));
					}
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
                                    L10n.Get( L10n.Id.ErrFunctionNotFound )
                                    + ": " + this.Id );
                    }
                }
                
                return this.fn;
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
        
        /// <summary>
        /// Gets the identifier of the function to call.
        /// </summary>
        /// <value>The identifier, as a string.</value>
        public string Id {
            get; private set;
        }
        
        /// <summary>
        /// Gets the number of arguments that were pushed into the stack.
        /// </summary>
        /// <value>The number of arguments, as an int.</value>
        public int NumArgs {
            get; private set;
        }

        private Function fn;
	}
}
