
namespace CSim {
	using System;
	using CSim.Core;

	public class StoreRValue: Opcode {
		public static char OpcodeValue = (char) 0xE4;

		public StoreRValue(Machine m, RValue rvalue)
			:base(m)
		{
			this.RValue = rvalue;
		}

		public override void Execute() {
			this.Machine.ExecutionStack.Push( this.RValue );
		}

		public RValue RValue {
			get; set;
		}
	}
}

