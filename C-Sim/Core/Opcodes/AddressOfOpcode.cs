// CSim - (c) 2014-17 Baltasar MIT License <jbgarcia@uvigo.es>

namespace CSim.Core.Opcodes {
    using System.Numerics;
	
    using Exceptions;

	/// <summary>
	/// Represents the rol of ampersand at the left of a variable.
	/// i.e., returns the address of that variable in memory.
	/// Can apply to an id only.
	/// </summary>
	public class AddressOfOpcode: Opcode {
		/// <summary>The opcode value.</summary>
		public const byte OpcodeValue = 0xE0;

		/// <summary>
		/// Initializes a new instance of the <see cref="CSim.Core.Opcodes.AddressOfOpcode"/> class.
		/// </summary>
		/// <param name="m">The machine this opcode will be executed in.</param>
		public AddressOfOpcode(Machine m)
			:base( m )
		{
		}

		/// <summary>
		/// Returns the address of the variable popped from the stack.
		/// </summary>
		public override void Execute()
		{
			var vble = this.Machine.ExecutionStack.Pop().SolveToVariable();

			if ( vble != null
			  && !( vble.IsTemp() ) )
			{
				BigInteger address = vble.SolveToVariable().Address;

				// Store in the temp vble and end
				var toret = Variable.CreateTempVariable( this.Machine, address );
				this.Machine.ExecutionStack.Push( toret );
			} else {
				throw new RuntimeException( "rvalue should be a variable" );
			}

			return;
		}
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.AddressOfOpcode"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.Core.Opcodes.AddressOfOpcode"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                            "[AddressOfOpcode(0x{0,2:X}): &lvalue(POP)]",
                            OpcodeValue );
        }
	}
}

