// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Opcodes {
    using System.Numerics;
    
	using CSim.Core.Types;
	using CSim.Core.Exceptions;

	/// <summary>
	/// Access opcode with '*', i.e. int * v = &amp;x; => *x = 5;
	/// </summary>
	public class AccessOpcode: Opcode {
		/// <summary>The opcode id.</summary>
		public const byte OpcodeValue = 0xE0;

		/// <summary>
		/// Initializes a new <see cref="T:AccessOpcode"/>.
		/// </summary>
		/// <param name="m">
        /// The <see cref="Machine"/> this opcode will be executed in.
        /// </param>
		/// <param name="levels">
        /// Number of indirection levels, i.e. 1: *x, 2: **x, 3:***x...
        /// </param>
		public AccessOpcode(Machine m, int levels)
			:base(m)
		{
			this.Levels = levels;
		}

		/// <summary>
		/// Returns a new variable, accessing an address with '*'
		/// </summary>
		public override void Execute()
		{
            // Check arguments in stack
            if ( this.Machine.ExecutionStack.Count < 1 ) {
                throw new RuntimeException( L10n.Get( L10n.Id.ErrMissingArguments ) );
            }
            
			Variable vble = this.Machine.ExecutionStack.Pop().SolveToVariable();
            Variable orgVble = vble;

			if ( vble != null ) {
				for(int i = 0; i < this.Levels; ++i) {
					// Access the pointed value
					if ( vble.Type is Ptr vbleType ) {
						BigInteger address = vble.LiteralValue.Value.ToBigInteger();
                        
                        
						vble = Variable.CreateTempVariable(
                                                    vbleType.DerreferencedType );
						vble.Address = address;
					}
					else {
						throw new TypeMismatchException(
                            this.Machine.TypeSystem.GetPtrType( vble.Type, this.Levels )
                            + " != " + orgVble.Type
                            + " (" + vble + ")" );
					}
				}

				// Store the temp vble and end
				this.Machine.ExecutionStack.Push( vble );
			} else {
				throw new RuntimeException( "invalid rvalue" );
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
