using System;

namespace CSim.Core.Opcodes {
	public abstract class Operand: Opcode {
		public Operand(Machine m)
			:base( m )
		{
		}
	}
}

