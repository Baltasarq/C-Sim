using System;

using CSim.Core.Variables;

namespace CSim.Core.Opcodes {
	/// <summary>
	/// This opcode is used in order to "load" a literal in the VM.
	/// </summary>
	public class Load: Opcode {
		public Load(Machine m, Literal l)
			:base( m )
		{
			this.Literal = l;
		}

		public override Variable Execute()
		{
			var temp = new TempVariable( this.Literal.Type );

			temp.LiteralValue = this.Literal;
			return temp;
		}

		/// <summary>
		/// Gets or sets the literal for this opcode.
		/// </summary>
		/// <value>The literal, as a Literal object.</value>
		public Literal Literal {
			get; set;
		}
	}
}

