using System;

namespace CSim.Core.Opcodes {
	/// <summary>
	/// Represents the rol of '*' at the left of a pointer.
	/// i. e., access the memory address contained by the pointer.
	/// Can apply to an RValue
	/// </summary>
	public class AccessTo: Operand {
		public const char OpcodeValue = (char) 0xE9;

		public AccessTo(Machine m, RValue rvalue)
			:base( m )
		{
			this.rvalue = rvalue;
		}

		public AccessTo(Machine m, Opcode opcode)
			:base( m )
		{
			this.opcode = opcode;
		}

		public override Variable Execute()
		{
			throw new NotImplementedException();
		}

		public RValue RValue {
			get {
				return this.rvalue;
			}
		}

		public Opcode Opcode {
			get {
				return this.opcode;
			}
		}

		private RValue rvalue;
		private Opcode opcode;
	}
}

