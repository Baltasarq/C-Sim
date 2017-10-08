
namespace CSim.Core.Opcodes {
    using System.Numerics;
    
	using CSim.Core.Variables;
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	/// <summary>
	/// Access opcode with '*', i.e. int * v = &amp;x; => *x = 5;
	/// </summary>
	public class AccessOpcode: Opcode {
		/// <summary>The opcode id.</summary>
		public const byte OpcodeValue = 0xE0;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Opcodes.AccessOpcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode will be executed in.</param>
		/// <param name="levels">Number of indirection levels, i.e. **x, *X or ***x</param>
		public AccessOpcode(Machine m, int levels)
			:base(m)
		{
			this.Levels = levels;
		}

		/// <summary>
		/// Returns the a variable, accessing its address with '*'
		/// </summary>
		public override void Execute()
		{
            // Check arguments in stack
            if ( this.Machine.ExecutionStack.Count < 1 ) {
                throw new EngineException( L18n.Get( L18n.Id.ErrMissingArguments ) );
            }
            
			Variable vble = this.Machine.ExecutionStack.Pop().SolveToVariable();

			if ( vble != null ) {
				for(int i = 0; i < this.Levels; ++i) {
					var vbleAsRef = vble as RefVariable;
					var vbleType = vble.Type as Ptr;

					// If the vble at the right is a reference, dereference it
					if ( vbleAsRef != null  ) {
						vble = vbleAsRef.PointedVble;
						vbleType = vble.Type as Ptr;
					}

					// Access the pointed value
					if ( vbleType != null ) {
						BigInteger address = vble.LiteralValue.Value.ToBigInteger();
						vble = new InPlaceTempVariable( vbleType.DerreferencedType );
						vble.Address = address;
						this.Machine.TDS.AddVariableInPlace( vble );
					}
					else {
						throw new TypeMismatchException( vble.ToString() );
					}
				}

				// Store in the temp vble and end
				this.Machine.ExecutionStack.Push( vble );
			} else {
				throw new EngineException( "invalid rvalue" );
			}

			return;
		}

		/// <summary>
		/// Gets the number of access levels
		/// </summary>
		/// <value>The levels of accessing, as an int.</value>
		public int Levels {
			get; set;
		}
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.AccessOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.AccessOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                            "[AccessOpcode (0x{0,2:X}): *(Levels={1})(rvalue(POP)]",
                            OpcodeValue,
                            Levels );
        }
	}
}
