
namespace CSim {
	using CSim.Core;

	/// <summary>Store RV value.</summary>
	public class StoreRValue: Opcode {
		/// <summary>The opcode's representing value.</summary>
		public static char OpcodeValue = (char) 0xE4;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:CSim.StoreRValue"/> class.
		/// </summary>
		/// <param name="rvalue">The <see cref="RValue"/>to store
		/// in the <see cref="ExecutionStack"/>.</param>
		public StoreRValue(RValue rvalue)
			:base( rvalue.Machine )
		{
			this.RValue = rvalue;
		}

		/// <summary>
		/// Execute this opcode.
		/// </summary>
		public override void Execute() {
			this.Machine.ExecutionStack.Push( this.RValue );
		}

		/// <summary>
		/// Gets or sets the RValue to store.
		/// </summary>
		/// <value>The <see cref="RValue"/>.</value>
		public RValue RValue {
			get; set;
		}
	}
}

