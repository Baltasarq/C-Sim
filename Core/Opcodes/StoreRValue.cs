
namespace CSim {
	using CSim.Core;

	/// <summary>Store RV value.</summary>
	public class StoreRValue: Opcode {
		/// <summary>The opcode's representing value.</summary>
		public static byte OpcodeValue = 0xE4;

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
        
        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:CSim.StoreRValue"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:CSim.StoreRValue"/>.</returns>
        public override string ToString()
        {
            return string.Format(
                            "[StoreRValue(0x{0,2:X}): RValue={1}]",
                            OpcodeValue,
                            this.RValue );
        }
	}
}

