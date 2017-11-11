// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core {
    using Exceptions;

	/// <summary>
	/// Represents the opcodes of the virtual machine.
	/// </summary>
    public abstract class Opcode {
		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.Core.Opcode"/> class.
		/// </summary>
		/// <param name="m">The <see cref="Machine"/> this opcode is going to be executed in.</param>
        protected Opcode(Machine m) {
            this.Machine = m;
        }

		/// <summary>
		/// Execute this opcode.
		/// </summary>
        public abstract void Execute();
        
        /// <summary>
        /// Chks the type compatibility for the given variables.
        /// </summary>
        /// <param name="op1">The first variable.</param>
        /// <param name="op2">The second variable.</param>
        public static void ChkTypeCompatibility(Variable op1, Variable op2)
        {
            // Check ops
            if ( op1 == null ) {
                throw new TypeMismatchException( "op1 == null!!" );
            }
            
            if ( op2 == null ) {
                throw new TypeMismatchException( "op2 == null!!" );
            }
            
            if ( !op1.Type.IsCompatibleWith( op2.Type ) ) {
                throw new TypeMismatchException(
                                        ": " + op1.Type + " != " + op2.Type );
            }
        }

        /// <summary>
        /// Gets or sets the machine this opcode is going to be executed in.
        /// </summary>
		/// <value>The <see cref="Machine"/>.</value>
        public Machine Machine {
            get; set;
        }
    }
}
